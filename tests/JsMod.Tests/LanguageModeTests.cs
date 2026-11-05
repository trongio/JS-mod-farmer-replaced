using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
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
    [InlineData("\uFEFF# @py\n", true)]
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
    }

    [Fact]
    public void CodeWithNoWindowIsColouredInTheLanguageOfTheSetting()
    {
        const string code = "x = 1";
        Assert.Equal("original", JsHighlighter.Select("original", code));
        UseSetting(LanguageMode.Python);
        Assert.Equal("original", JsHighlighter.Select("original", code));
        UseSetting(LanguageMode.JavaScript);
        Assert.Equal(JsHighlighter.Pattern, JsHighlighter.Select("original", code));
        Assert.Equal("original", JsHighlighter.Select("original", "# @py\nx = 1"));
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
        LanguageMode.ExitTokenizing(outer);
        // A docs example is coloured as Python while it has no JS code, even though the setting is JS.
        CodeWindow outerColour = LanguageMode.EnterColouring(DocsExampleWindow());
        Assert.False(LanguageMode.IsJsWhileColouring("x = 1;"));
        LanguageMode.ExitColouring(outerColour);
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
    [InlineData("KA", "Python", null)]
    [InlineData("KA", "JavaScript", null)]
    [InlineData("EN", "JavaScript", null)]
    [InlineData("KA", "language", null)]
    [InlineData("EN", "python", null)]
    [InlineData("EN", null, null)]
    public void OptionTextByLanguage(string language, string key, string expected)
    {
        Assert.Equal(expected, ModStrings.Lookup(key, language));
    }

    [Fact]
    public void WordListFollowsWhatIsUnlocked()
    {
        List<string> early = Display.JsWordList(new[] { "harvest", "while", "if" });
        Assert.Contains("while", early);
        Assert.Contains("let", early);
        Assert.Contains("const", early);
        Assert.DoesNotContain("function", early);
        Assert.DoesNotContain("true", early);
        Assert.DoesNotContain("of", early);
        List<string> later = Display.JsWordList(new[] { "def", "True", "False", "None", "for", "elif", "and", "import", "from", "my_var" });
        Assert.Contains("function", later);
        Assert.Contains("true", later);
        Assert.Contains("false", later);
        Assert.Contains("null", later);
        Assert.Contains("of", later);
        Assert.Contains("import", later);
        Assert.Contains("from", later);
        Assert.Contains("my_var", later);
        Assert.DoesNotContain("def", later);
        Assert.DoesNotContain("True", later);
        Assert.DoesNotContain("elif", later);
        Assert.DoesNotContain("and", later);
        Assert.Equal(later.OrderBy(w => w, StringComparer.CurrentCulture), later);
    }

    [Theory]
    [InlineData("    }", 5, "}", 1)]
    [InlineData("if (a) {\n        }", 18, "if (a) {\n    }", 14)]
    [InlineData("\t}", 2, "}", 1)]
    [InlineData("\t\t}", 3, "\t}", 2)]
    [InlineData("  }", 3, "}", 1)]
    public void ClosingBraceMovesBackOneLevel(string text, int caret, string expected, int expectedCaret)
    {
        Assert.True(Display.OutdentClosingBrace(ref text, ref caret));
        Assert.Equal(expected, text);
        Assert.Equal(expectedCaret, caret);
    }

    [Theory]
    [InlineData("}", 1)]
    [InlineData("a }", 3)]
    [InlineData("    x", 5)]
    [InlineData("    {", 5)]
    public void ClosingBraceIsLeftAloneWhenNotFirstOnItsLine(string text, int caret)
    {
        string before = text;
        Assert.False(Display.OutdentClosingBrace(ref text, ref caret));
        Assert.Equal(before, text);
    }

    [Theory]
    [InlineData("import \"", true)]
    [InlineData("import ", true)]
    [InlineData("import * from \"", true)]
    [InlineData("import { a } from \"", true)]
    [InlineData("import * as ", true)]
    [InlineData("from ", true)]
    [InlineData("let x = ", false)]
    [InlineData("", false)]
    [InlineData("\"", false)]
    public void FileNamesAreOfferedInJsImports(string before, bool expected)
    {
        Assert.Equal(expected, Display.IsJsImportPosition(before, before.Length));
    }

    [Fact]
    public void SignatureIncludesTheCommentsAboveIt()
    {
        const string source = "let a = 1;\n// first\n// second\nfunction f() {\n}";
        int start = source.IndexOf("function", StringComparison.Ordinal);
        string nl = Environment.NewLine;
        Assert.Equal("// first" + nl + "// second" + nl + "function f() {", Display.SignatureWithComments(source, "function f() {", start));
        Assert.Equal("function g() {", Display.SignatureWithComments("function g() {\n}", "function g() {", 0));
        Assert.Equal("function h() {", Display.SignatureWithComments("let a = 1;\nfunction h() {\n}", "function h() {", 11));
    }

    [Fact]
    public void ErrorsAreEnglishExceptInGeorgian()
    {
        const string message = "Expected ';' or a new line, found ':'.";
        Assert.Equal(message, JsErrors.Translate(message, "EN"));
        Assert.Equal(message, JsErrors.Translate(message, null));
        Assert.Equal("აქ ';' ან ახალი ხაზი უნდა იყოს, მაგრამ აქ არის ':'.", JsErrors.Translate(message, "KA"));
        Assert.Equal("აქ სახელი უნდა იყოს, მაგრამ კოდი დასრულდა.", JsErrors.Translate("Expected a name, but the code ended.", "KA"));
        Assert.Equal("აქ სახელი უნდა იყოს, მაგრამ ხაზი დასრულდა.", JsErrors.Translate("Expected a name, but the line ended.", "KA"));
        Assert.Equal("harvest ფრჩხილების გარეშე არაფერს აკეთებს. ხომ არ გულისხმობდი harvest()-ს?", JsErrors.Translate("harvest does nothing without brackets. Did you mean harvest()?", "KA"));
        Assert.Equal("Something else.", JsErrors.Translate("Something else.", "KA"));
    }

    /// <summary>Reads the parser's source, so a new error message without Georgian text fails here.</summary>
    [Fact]
    public void EveryErrorInTheParserHasGeorgianText()
    {
        string dir = AppContext.BaseDirectory;
        while (!File.Exists(Path.Combine(dir, "src", "JsMod", "Lang", "JsParser.cs")))
        {
            dir = Path.GetDirectoryName(dir);
        }
        string source = File.ReadAllText(Path.Combine(dir, "src", "JsMod", "Lang", "JsParser.cs"));
        var missing = new List<string>();
        foreach (Match match in Regex.Matches(source, "(?:new ParseException\\(|Unexpected\\(|return |\\{ \"[^\"]+\", )(\"(?:[^\"\\\\]|\\\\.)*\")(?=,|\\)|;| \\})"))
        {
            string text = Regex.Unescape(match.Groups[1].Value.Trim('"'));
            if (text.Length > 6 && JsErrors.Translate(text, "KA") == text && !text.StartsWith("Expected a name") )
            {
                missing.Add(text);
            }
        }
        foreach (string built in new[]
        {
            "x does nothing without brackets. Did you mean x()?", "x is a const and can't be reassigned.", "x is a reserved name in this game.",
            "A file must be imported under its own name, so write: import * as a from \"a\".", "includes() takes exactly one value.",
            "Math.sin is not available here. Math has abs, floor, max, min and random.", "'class' is not supported here.", "Unexpected ')'.",
            "Expected ')'", "Invalid number '1x'.", "Unexpected character '@'."
        })
        {
            if (JsErrors.Translate(built, "KA") == built)
            {
                missing.Add(built);
            }
        }
        Assert.True(missing.Count == 0, string.Join("\n", missing));
    }
}
