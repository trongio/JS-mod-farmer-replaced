using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using JsMod.Lang;
using Xunit;

/// <summary>The rules behind the setting. Harmony isn't loaded here, so these call what the patches call.</summary>
public class LanguageModeTests : IDisposable
{
    public void Dispose()
    {
        LanguageMode.ReadSetting = null;
    }

    private static CodeWindow PlayerWindow(string name = "main")
    {
        var workspace = (Workspace)RuntimeHelpers.GetUninitializedObject(typeof(Workspace));
        workspace.codeWindows = new ConcurrentDictionary<string, CodeWindow>();
        CodeWindow window = TestSupport.FakeWindow();
        window.workspace = workspace;
        workspace.codeWindows[name] = window;
        return window;
    }

    /// <summary>What CodeExample builds: a CodeWindow pointing at the real workspace, which never registers it.</summary>
    private static CodeWindow DocsExampleWindow()
    {
        CodeWindow window = TestSupport.FakeWindow();
        window.workspace = PlayerWindow().workspace;
        return window;
    }

    private static void UseSetting(string value)
    {
        LanguageMode.ReadSetting = () => value;
    }

    [Theory]
    [InlineData("Python", "x = 1", false)]
    [InlineData("Python", "// @js\nx = 1;", true)]
    [InlineData("Python", "# @py\nx = 1", false)]
    [InlineData("JavaScript", "x = 1;", true)]
    [InlineData("JavaScript", "", true)]
    [InlineData("JavaScript", "// @js\nx = 1;", true)]
    [InlineData("JavaScript", "# @py\nx = 1", false)]
    [InlineData("JavaScript", "# just a comment\nx = 1", true)]
    [InlineData("JavaScript", "# @python\nx = 1", true)]
    [InlineData("JavaScript", "x = 1\n# @py", true)]
    [InlineData("Python", "x = 1\n// @js", false)]
    [InlineData("JavaScript", "# @py\n// @js", false)]
    [InlineData("Python", "// @js\n# @py", true)]
    [InlineData("Lua", "x = 1", false)]
    [InlineData("", "x = 1", false)]
    [InlineData(null, "x = 1", false)]
    public void SettingAndMarkerDecideForAPlayerWindow(string setting, string code, bool expected)
    {
        UseSetting(setting);
        Assert.Equal(expected, LanguageMode.IsJs(code, PlayerWindow()));
    }

    [Fact]
    public void WithoutTheSettingWiredUpPlainCodeIsPythonAndTheMarkerStillWorks()
    {
        LanguageMode.ReadSetting = null;
        Assert.False(LanguageMode.IsJs("x = 1;", PlayerWindow()));
        Assert.True(LanguageMode.IsJs("// @js\nx = 1;", PlayerWindow()));
    }

    [Theory]
    [InlineData("# @py", true)]
    [InlineData("#@py", true)]
    [InlineData("#\t@py\n", true)]
    [InlineData("  # @py\nfoo()", true)]
    [InlineData("\n\n# @py\nfoo()", true)]
    [InlineData("﻿# @py\n", true)]
    [InlineData("# @py some note\n", true)]
    [InlineData("# @python\n", false)]
    [InlineData("# @pyx\n", false)]
    [InlineData("# @PY\n", false)]
    [InlineData("# x @py\n", false)]
    [InlineData("## @py\n", false)]
    [InlineData("// # @py\n", false)]
    [InlineData("x = 1\n# @py\n", false)]
    [InlineData("#", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void DetectsPythonMarker(string code, bool expected)
    {
        Assert.Equal(expected, JsMarker.IsPy(code));
    }

    [Fact]
    public void PythonMarkerIsAnOrdinaryCommentToPython()
    {
        Assert.Equal(TestSupport.Dump(TestSupport.ParsePy("x = 1").syntaxTree), TestSupport.Dump(TestSupport.ParsePy("x = 1 # @py").syntaxTree));
        Assert.Equal(7, TestSupport.RunPy("# @py\nx = 7\n").Number("x"));
    }

    [Fact]
    public void HashLineIsAnErrorInJavaScriptSoAPythonMarkerCantHideValidCode()
    {
        Assert.Throws<ParseException>(() => JsParser.Parse(JsTokenizer.Tokenize("# @python\nx = 1;"), TestSupport.FakeWindow()));
    }

    [Theory]
    [InlineData("")]
    [InlineData("\n  \n")]
    [InlineData("// a comment only")]
    public void EmptyWindowParsesInJsMode(string code)
    {
        Assert.NotNull(JsParser.Parse(JsTokenizer.Tokenize(code), TestSupport.FakeWindow()).syntaxTree);
    }

    [Fact]
    public void PlainJavaScriptRunsWithoutAMarker()
    {
        Program program = JsParser.Parse(JsTokenizer.Tokenize("let x = 2; x += 3;"), TestSupport.FakeWindow());
        Assert.Equal(5, TestSupport.Execute(program.syntaxTree).Number("x"));
    }

    [Fact]
    public void PythonSourceInJsModeFailsWithAPositionNotACrash()
    {
        Assert.Throws<ParseException>(() => JsParser.Parse(JsTokenizer.Tokenize("def f(a):\n    return a\nwhile True:\n    f(1)\n"), TestSupport.FakeWindow()));
    }

    [Fact]
    public void InternalPythonStaysPythonInJsMode()
    {
        UseSetting(LanguageMode.JavaScript);
        const string code = "x = 1\nwhile True:\n    pass\n";
        Assert.False(LanguageMode.IsJs(code, null));
        Assert.False(LanguageMode.IsJs(code, DocsExampleWindow()));
        Assert.False(LanguageMode.IsJs(code, TestSupport.FakeWindow()));
        // UISim.SetupSim and any other caller with no patched window around it.
        Assert.False(LanguageMode.IsJsWhileTokenizing(code));
        Assert.False(LanguageMode.IsJsWhileColouring(code));
        Assert.Equal("original", JsHighlighter.Select("original", code));
    }

    [Fact]
    public void DocsExampleWindowStaysPythonInsideItsOwnScope()
    {
        UseSetting(LanguageMode.JavaScript);
        CodeWindow example = DocsExampleWindow();
        CodeWindow outerTokens = LanguageMode.EnterTokenizing(example);
        CodeWindow outerColours = LanguageMode.EnterColouring(example);
        Assert.False(LanguageMode.IsJsWhileTokenizing("x = 1"));
        Assert.Equal("original", JsHighlighter.Select("original", "x = 1"));
        LanguageMode.ExitColouring(outerColours);
        LanguageMode.ExitTokenizing(outerTokens);
    }

    [Fact]
    public void WindowStopsCountingOnceClosedAndKeepsCountingWhenRenamed()
    {
        CodeWindow window = PlayerWindow("old");
        var files = window.workspace.codeWindows;
        Assert.True(LanguageMode.IsPlayerWindow(window));
        files.TryRemove("old", out _);
        files["new"] = window;
        Assert.True(LanguageMode.IsPlayerWindow(window));
        files.TryRemove("new", out _);
        Assert.False(LanguageMode.IsPlayerWindow(window));
    }

    [Theory]
    [InlineData("Python", "x = 1")]
    [InlineData("Python", "// @js\nx = 1;")]
    [InlineData("Python", "# @py\nx = 1")]
    [InlineData("JavaScript", "x = 1;")]
    [InlineData("JavaScript", "// @js\nx = 1;")]
    [InlineData("JavaScript", "# @py\nx = 1")]
    public void ParserHighlighterAndEditorAgreeForTheSameWindow(string setting, string code)
    {
        UseSetting(setting);
        CodeWindow window = PlayerWindow();
        CodeWindow outerTokens = LanguageMode.EnterTokenizing(window);
        CodeWindow outerColours = LanguageMode.EnterColouring(window);
        bool editor = LanguageMode.IsJs(code, window);
        Assert.Equal(editor, LanguageMode.IsJsWhileTokenizing(code));
        Assert.Equal(editor, JsHighlighter.Select("original", code) == JsHighlighter.Pattern);
        LanguageMode.ExitColouring(outerColours);
        LanguageMode.ExitTokenizing(outerTokens);
    }

    [Fact]
    public void TokenizingScopeDoesNotColourAndColouringScopeDoesNotTokenize()
    {
        // The game can reach other highlighting while UpdateTokens runs, such as code tags in localized text.
        UseSetting(LanguageMode.JavaScript);
        CodeWindow outer = LanguageMode.EnterTokenizing(PlayerWindow());
        Assert.True(LanguageMode.IsJsWhileTokenizing("x = 1;"));
        Assert.False(LanguageMode.IsJsWhileColouring("x = 1;"));
        LanguageMode.ExitTokenizing(outer);
        outer = LanguageMode.EnterColouring(PlayerWindow());
        Assert.True(LanguageMode.IsJsWhileColouring("x = 1;"));
        Assert.False(LanguageMode.IsJsWhileTokenizing("x = 1;"));
        LanguageMode.ExitColouring(outer);
    }

    [Fact]
    public void NestedScopesRestoreTheOuterWindow()
    {
        UseSetting(LanguageMode.JavaScript);
        CodeWindow first = LanguageMode.EnterTokenizing(PlayerWindow());
        CodeWindow second = LanguageMode.EnterTokenizing(DocsExampleWindow());
        Assert.False(LanguageMode.IsJsWhileTokenizing("x"));
        LanguageMode.ExitTokenizing(second);
        Assert.True(LanguageMode.IsJsWhileTokenizing("x"));
        LanguageMode.ExitTokenizing(first);
        Assert.False(LanguageMode.IsJsWhileTokenizing("x"));
    }

    [Fact]
    public async Task ScopeDoesNotLeakToOtherThreads()
    {
        UseSetting(LanguageMode.JavaScript);
        CodeWindow outer = LanguageMode.EnterTokenizing(PlayerWindow());
        bool onOtherThread = await Task.Factory.StartNew(() => LanguageMode.IsJsWhileTokenizing("x"), TaskCreationOptions.LongRunning);
        LanguageMode.ExitTokenizing(outer);
        Assert.False(onOtherThread);
    }

    /// <summary>JS has no import yet, so a mixed project is a `# @py` file importing a module. Each resolves from its own text.</summary>
    [Fact]
    public void ImporterAndImportedFileEachFollowTheirOwnLanguage()
    {
        UseSetting(LanguageMode.JavaScript);
        CodeWindow importer = PlayerWindow("main");
        CodeWindow library = TestSupport.FakeWindow();
        library.workspace = importer.workspace;
        importer.workspace.codeWindows["lib"] = library;
        const string importerCode = "# @py\nimport lib\nlib.f()\n";
        const string libraryCode = "function f() { return 1; }";

        Assert.False(LanguageMode.IsJs(importerCode, importer));
        Assert.True(LanguageMode.IsJs(libraryCode, library));

        Tokenizer.Tokenize(importerCode, out TokenStream importerTokens);
        Program importerProgram = Parser.Parse(importerTokens, importer);
        Assert.Contains("lib", importerProgram.allVars);
        Program libraryProgram = JsParser.Parse(JsTokenizer.Tokenize(libraryCode), library);
        Assert.Contains("f", libraryProgram.allVars);
        Assert.True(library.parsedFunctions.ContainsKey("f"));
    }

    [Fact]
    public void JavaScriptImportStillReportsItsError()
    {
        UseSetting(LanguageMode.JavaScript);
        const string code = "import lib\n";
        Assert.True(LanguageMode.IsJs(code, PlayerWindow()));
        Assert.Throws<ParseException>(() => JsParser.Parse(JsTokenizer.Tokenize(code), TestSupport.FakeWindow()));
    }

    /// <summary>OptionHolder loads names it has no asset for as Unknown. A value that looks like a number or a key would stop being a string.</summary>
    [Theory]
    [InlineData(LanguageMode.Python)]
    [InlineData(LanguageMode.JavaScript)]
    public void SavedValuesLoadBackAsStrings(string value)
    {
        Assert.Equal(value, OptionHolder.ValueOfString(value, OptionValueType.Unknown));
    }

    [Theory]
    [InlineData("KA", LanguageMode.OptionName, "პროგრამირების ენა")]
    [InlineData("EN", LanguageMode.OptionName, "Programming language")]
    [InlineData("DE", LanguageMode.OptionName, "Programming language")]
    [InlineData(null, LanguageMode.OptionName, "Programming language")]
    [InlineData("KA", ModStrings.TooltipKey, "აირჩიე, რომელ ენაზე დაწერ კოდს. ფაილი შეიძლება ამ არჩევანს პირველი ხაზით შეცვალოს: // @js ან # @py.")]
    [InlineData("EN", ModStrings.TooltipKey, "Choose which language you write code in. A file can override this choice with its first line: // @js or # @py.")]
    [InlineData("ZH", ModStrings.TooltipKey, "Choose which language you write code in. A file can override this choice with its first line: // @js or # @py.")]
    [InlineData("KA", "Python", "Python")]
    [InlineData("KA", "JavaScript", "JavaScript")]
    [InlineData("EN", "JavaScript", "JavaScript")]
    [InlineData("KA", "language", null)]
    [InlineData("EN", "python", null)]
    [InlineData("EN", null, null)]
    public void OptionTextByLanguage(string language, string key, string expected)
    {
        Assert.Equal(expected, ModStrings.Lookup(key, language));
    }
}
