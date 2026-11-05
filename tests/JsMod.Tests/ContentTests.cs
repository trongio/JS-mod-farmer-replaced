using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using JsMod.Lang;
using Xunit;
using static TestSupport;

/// <summary>
/// The gate for the JS content: every piece of code in the game's English and Georgian text has a JS twin that parses and,
/// where it needs nothing from the farm, runs. Fails when a game update changes the text.
/// </summary>
public class ContentTests : IDisposable
{
    private static readonly string Root = FindRoot();
    internal static readonly string ContentFolder = Path.Combine(Root, "src", "JsMod", "js");
    private static readonly string Languages = Path.Combine(GameAssemblies.ManagedDir, "..", "StreamingAssets", "Languages");
    private static readonly string[] Packs = { Path.Combine(Languages, "EN"), Path.Combine(Root, "translations", "KA") };

    private static readonly List<JsContent.Pair> Examples = JsContent.ParsePairs(File.ReadAllText(Path.Combine(ContentFolder, JsContent.ExamplesFile)));
    private static readonly List<JsContent.Pair> Spans = JsContent.ParsePairs(File.ReadAllText(Path.Combine(ContentFolder, JsContent.SpansFile)));

    // Examples that are meant to fail, to show an error.
    private static readonly HashSet<string> MeantToFail = new HashSet<string> { "docs/scripting/functions.md#2" };

    // Written differently on purpose, so the tick costs don't match the Python: arrays instead of tuples, and string dict keys.
    private static readonly HashSet<string> CostsDiffer = new HashSet<string>
    {
        "docs/scripting/dicts.md#1", "docs/scripting/dicts.md#3", "docs/scripting/tuples.md#0", "docs/scripting/tuples.md#1",
        "docs/scripting/tuples.md#3", "docs/scripting/tuples.md#4"
    };

    private static readonly Regex FarmCall = new Regex(@"\b(?!print\b|len\b|str\b|range\b|set\b|dict\b|list\b|min\b|max\b|abs\b)[a-z_][a-z0-9_]*\(");

    private static readonly Regex PythonSyntax = new Regex(
        @"\b(def|elif|pass|True|False|None|not|and|or|global)\b|^#|\s#|:\s*$|:\n|\bfor \w+ in\b|\*args|\bimport \w|\bfrom \w",
        RegexOptions.Multiline);

    // Not code being spelled in Python, or for pages rewritten by hand: file names, paths, prose and a colon on its own.
    private static readonly HashSet<string> NotPython = new HashSet<string>
    {
        "#", ":", "from", "import", "%USERPROFILE%\\AppData\\LocalLow\\TheFarmerWasReplaced\\TheFarmerWasReplaced"
    };

    public ContentTests()
    {
        JsContent.Warn = null;
        JsContent.Load(ContentFolder);
    }

    public void Dispose()
    {
        LanguageMode.ReadSetting = null;
    }

    private static readonly Regex GameConstant = new Regex(@"\b(Items|Entities|Grounds|Unlocks|Hats|Leaderboards)\b(\.\w+)?");

    /// <summary>Those constants come from Unity assets, which aren't here, so each becomes a plain name. The tree keeps its shape.</summary>
    private static string WithoutAssets(string code)
    {
        return GameConstant.Replace(code, m => m.Value.Replace('.', '_') + "_");
    }

    private static List<string> AssetNames(string code)
    {
        return GameConstant.Matches(code).Select(m => m.Value.Replace('.', '_') + "_").Distinct().ToList();
    }

    private static string FindRoot()
    {
        string dir = AppContext.BaseDirectory;
        while (dir != null && !Directory.Exists(Path.Combine(dir, "src", "JsMod", "js")))
        {
            dir = Path.GetDirectoryName(dir);
        }
        return dir ?? throw new DirectoryNotFoundException("Repository root");
    }

    private static string Read(string path)
    {
        return File.ReadAllText(path).Replace("\r\n", "\n");
    }

    private static IEnumerable<(string name, string text)> Docs(string pack)
    {
        foreach (string file in Directory.EnumerateFiles(Path.Combine(pack, "docs"), "*.md", SearchOption.AllDirectories))
        {
            yield return (Path.GetRelativePath(pack, file).Replace('\\', '/'), Read(file));
        }
    }

    private static IEnumerable<string> CodeSpans(string text)
    {
        return Regex.Split(Regex.Replace(text, @"\{\{codeexample.*?\}\}", "", RegexOptions.Singleline), "\r\n```|\n```|```|`").Where((_, i) => i % 2 == 1);
    }

    private static IEnumerable<string> Strings(string pack)
    {
        return Directory.EnumerateFiles(Path.Combine(pack, "Strings"), "*.txt").Select(Read);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void EveryExampleInThePackHasAJsTwin(int pack)
    {
        string name = Packs[pack];
        int blocks = 0;
        foreach ((string doc, string text) in Docs(name))
        {
            var missed = new List<string>();
            JsContent.Warn = missed.Add;
            string mapped = JsContent.MapExamples(text, doc + pack);
            JsContent.Warn = null;
            Assert.Empty(missed);
            foreach (Match block in Regex.Matches(mapped, @"\{\{codeexample\b.*?#CODE[ \t]*\r?\n(.*?)\s*\}\}", RegexOptions.Singleline))
            {
                Assert.True(JsContent.IsJsExample(block.Groups[1].Value), doc + ": " + block.Groups[1].Value);
                blocks++;
            }
        }
        Assert.Equal(146, blocks);
    }

    [Fact]
    public void EveryExampleParsesAndContainsNothingTheDocsCantShow()
    {
        Assert.Equal(146, Examples.Count);
        foreach (JsContent.Pair example in Examples)
        {
            Assert.DoesNotContain("`", example.Js);
            Assert.DoesNotContain("}}", example.Js);
            Assert.DoesNotContain("{{", example.Js);
            ParseJs(WithoutAssets(example.Js));
        }
    }

    [Fact]
    public void ExamplesThatNeedNoFarmRunWithTheCostOfTheirPython()
    {
        // print needs the farm too, so a function of the same cost in both languages stands in for it.
        const string jsPrint = "function print(a = 0, b = 0, c = 0, d = 0) {}\n";
        const string pyPrint = "def print(a = 0, b = 0, c = 0, d = 0):\n    pass\n";
        int ran = 0;
        foreach (JsContent.Pair example in Examples)
        {
            if (MeantToFail.Contains(example.Name) || FarmCall.IsMatch(example.Js) || Regex.IsMatch(example.Js, @"\bwhile \(true\)"))
            {
                continue;
            }
            List<string> names = AssetNames(example.Js);
            string jsNames = string.Concat(names.Select(n => "let " + n + " = 0;\n"));
            string pyNames = string.Concat(names.Select(n => n + " = 0\n"));
            Run js = RunJs(jsPrint + jsNames + WithoutAssets(example.Js));
            if (!CostsDiffer.Contains(example.Name))
            {
                double python = RunPy(pyPrint + pyNames + WithoutAssets(example.Python)).Ops;
                Assert.True(python == js.Ops, example.Name + " costs " + js.Ops + ", Python " + python);
            }
            ran++;
        }
        Assert.True(ran >= 43, "Only " + ran + " examples ran");
    }

    [Fact]
    public void SpansThatAreWholeProgramsParse()
    {
        var failed = new List<string>();
        foreach (JsContent.Pair span in Spans.Where(s => !s.IsFragment))
        {
            try
            {
                ParseJs(WithoutAssets(span.Js));
            }
            catch (ParseException ex)
            {
                failed.Add(span.Js + " => " + ex.Message);
            }
        }
        Assert.True(failed.Count == 0, string.Join("\n---\n", failed));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void NoPythonSyntaxIsLeftInTheCodeSpansOfThePack(int pack)
    {
        var left = new List<string>();
        foreach (string text in Docs(Packs[pack]).Select(d => d.text).Concat(Strings(Packs[pack])))
        {
            foreach (string span in CodeSpans(JsContent.MapCodeTags(text)))
            {
                if (!NotPython.Contains(span) && PythonSyntax.IsMatch(span) && !Spans.Any(s => s.Js == span.Replace("{{", "{").Replace("}}", "}")))
                {
                    left.Add(span);
                }
            }
        }
        Assert.True(left.Count == 0, string.Join("\n---\n", left.Distinct()));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void MappedTextThatTheGameFormatsStillFormats(int pack)
    {
        foreach (string text in Strings(Packs[pack]))
        {
            foreach (string entry in text.Split("\n@"))
            {
                if (Regex.IsMatch(entry, @"\{\d\}"))
                {
                    string value = entry.Substring(entry.IndexOf(" = ", StringComparison.Ordinal) + 3);
                    if (Formats(value))
                    {
                        Assert.True(Formats(JsContent.MapCodeTags(value)), value);
                    }
                }
            }
        }
    }

    private static bool Formats(string value)
    {
        try
        {
            string.Format(value, "a", "b", "c", "d");
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData(LanguageMode.Python)]
    public void EverythingIsUntouchedInPythonMode(string setting)
    {
        LanguageMode.ReadSetting = setting == null ? null : () => setting;
        const string tags = "returns `True` when `def` runs";
        const string doc = "{{codeexample {}\n#CODE\ndo_a_flip()\nharvest()\nharvest()\n}}";
        Assert.Same(tags, Display.CodeTags(tags));
        Assert.Same(doc, Display.Doc("EN", "docs/first_program.md", doc));
        Assert.Equal("def f():", Display.UnlockBoxCode("def f():"));
        Assert.False(Display.NiceString(new PyBool(true), out _));
        Assert.False(Display.NiceString(new PyNone(), out _));
        Assert.Equal("function", Display.TooltipKeyword("function"));
    }

    [Fact]
    public void JsModeShowsJs()
    {
        LanguageMode.ReadSetting = () => LanguageMode.JavaScript;
        Assert.Equal("returns `true` when `function` runs, `|| ` too, `harvest()` stays", Display.CodeTags("returns `True` when `def` runs, `or ` too, `harvest()` stays"));
        string doc = "x\n{{codeexample {}\n#SETUP\nmove(East)\n#CODE\ndo_a_flip()\nharvest()\nharvest()\n}}\ny";
        Assert.Equal("x\n{{codeexample {}\n#SETUP\nmove(East)\n#CODE\ndo_a_flip();\nharvest();\nharvest();\n}}\ny", Display.Doc("EN", "d", doc));
        Assert.Equal("function f()", Display.UnlockBoxCode("def f():"));
        Assert.True(Display.NiceString(new PyBool(true), out string yes));
        Assert.Equal("true", yes);
        Assert.True(Display.NiceString(new PyBool(false), out string no));
        Assert.Equal("false", no);
        Assert.True(Display.NiceString(new PyNone(), out string none));
        Assert.Equal("null", none);
        Assert.False(Display.NiceString(new PyNumber(1), out _));
        Assert.Equal("def", Display.TooltipKeyword("function"));
        Assert.Equal("None", Display.TooltipKeyword("null"));
        Assert.Equal("harvest", Display.TooltipKeyword("harvest"));
    }

    [Fact]
    public void AMissLeavesThePythonAndIsLoggedOnce()
    {
        LanguageMode.ReadSetting = () => LanguageMode.JavaScript;
        var warnings = new List<string>();
        JsContent.Warn = warnings.Add;
        const string doc = "{{codeexample {}\n#CODE\nprint(\"hola\")\n}}";
        Assert.Equal(doc, Display.Doc("ES", "miss.md", doc));
        Assert.Equal(doc, Display.Doc("ES", "miss.md", doc));
        JsContent.Warn = null;
        Assert.Single(warnings);
        Assert.False(JsContent.IsJsExample("print(\"hola\")"));
    }

    [Fact]
    public void MissingContentFilesLeaveEverythingPython()
    {
        var warnings = new List<string>();
        JsContent.Warn = warnings.Add;
        JsContent.Load(Path.Combine(ContentFolder, "does-not-exist"));
        JsContent.Warn = null;
        LanguageMode.ReadSetting = () => LanguageMode.JavaScript;
        Assert.Contains("does-not-exist", Assert.Single(warnings));
        Assert.Equal("`True`", Display.CodeTags("`True`"));
        Assert.False(JsContent.IsJsExample("harvest();"));
        Assert.False(LanguageMode.IsJsWhileColouring("let x = 1;"));
    }

    [Fact]
    public void AMissingContentFileIsNamed()
    {
        string folder = Directory.CreateTempSubdirectory().FullName;
        try
        {
            var warnings = new List<string>();
            JsContent.Warn = warnings.Add;
            JsContent.Load(folder);
            JsContent.Warn = null;
            Assert.Equal(2, warnings.Count);
            Assert.Contains(warnings, w => w.Contains(JsContent.SpansFile));
            Assert.Contains(warnings, w => w.Contains(JsContent.ExamplesFile));
        }
        finally
        {
            Directory.Delete(folder, true);
        }
    }

    [Theory]
    [InlineData("function f() {", true)]
    [InlineData("let x = 1;", true)]
    [InlineData("harvest()", true)]
    [InlineData("print(\"and\")", true)]
    [InlineData("let a = {k: 1};", true)]
    [InlineData("import \"lib\";", true)]
    [InlineData("import { a } from \"lib\";", true)]
    [InlineData("def f():", false)]
    [InlineData("x = None", false)]
    [InlineData("if a and b:", false)]
    [InlineData("while True:\n    pass", false)]
    [InlineData("for i in range(3):", false)]
    [InlineData("# a comment", false)]
    [InlineData("x = 1 # a comment", false)]
    [InlineData("import lib", false)]
    [InlineData("from lib import f", false)]
    public void CodeOutsideAWindowColoursByWhatItLooksLike(string code, bool expected)
    {
        LanguageMode.ReadSetting = () => LanguageMode.JavaScript;
        Assert.Equal(expected, LanguageMode.IsJsWhileColouring(code));
    }

    [Fact]
    public void CodeOutsideAWindowIsPythonInPythonMode()
    {
        LanguageMode.ReadSetting = () => LanguageMode.Python;
        Assert.False(LanguageMode.IsJsWhileColouring("let x = 1;"));
    }

    [Fact]
    public void APageOnlyWeHaveStillLooksMissingToTheGame()
    {
        string folder = Directory.CreateTempSubdirectory().FullName;
        try
        {
            Directory.CreateDirectory(Path.Combine(folder, "EN", "docs"));
            File.WriteAllText(Path.Combine(folder, "EN", "docs", "a.md"), "en");
            JsContent.Warn = null;
            JsContent.Load(folder);
            LanguageMode.ReadSetting = () => LanguageMode.JavaScript;
            Assert.Equal("", Display.Doc("EN", "docs/a.md", ""));
            Assert.Null(Display.Doc("EN", "docs/a.md", null));
            Assert.Equal("en", Display.Doc("EN", "docs/a.md", "x"));
        }
        finally
        {
            Directory.Delete(folder, true);
        }
    }

    [Fact]
    public void PagesAreListedWhenLoaded()
    {
        string folder = Directory.CreateTempSubdirectory().FullName;
        try
        {
            Directory.CreateDirectory(Path.Combine(folder, "EN", "docs"));
            JsContent.Warn = null;
            JsContent.Load(folder);
            LanguageMode.ReadSetting = () => LanguageMode.JavaScript;
            File.WriteAllText(Path.Combine(folder, "EN", "docs", "later.md"), "late");
            Assert.Equal("x", Display.Doc("EN", "docs/later.md", "x"));
        }
        finally
        {
            Directory.Delete(folder, true);
        }
    }

    [Fact]
    public void WholePageOverridesWinAndFallBackToEnglish()
    {
        string folder = Directory.CreateTempSubdirectory().FullName;
        try
        {
            Directory.CreateDirectory(Path.Combine(folder, "EN", "docs"));
            Directory.CreateDirectory(Path.Combine(folder, "KA", "docs"));
            File.WriteAllText(Path.Combine(folder, "EN", "docs", "a.md"), "en");
            File.WriteAllText(Path.Combine(folder, "EN", "docs", "b.md"), "en b");
            File.WriteAllText(Path.Combine(folder, "KA", "docs", "a.md"), "ka");
            JsContent.Warn = null;
            JsContent.Load(folder);
            LanguageMode.ReadSetting = () => LanguageMode.JavaScript;
            Assert.Equal("ka", Display.Doc("KA", "docs/a.md", "x"));
            Assert.Equal("en", Display.Doc("DE", "docs/a.md", "x"));
            Assert.Equal("en b", Display.Doc("KA", "docs/b.md", "x"));
            Assert.Equal("x", Display.Doc("KA", "docs/c.md", "x"));
        }
        finally
        {
            Directory.Delete(folder, true);
        }
    }

    [Fact]
    public void ConcurrentTagMappingIsSafe()
    {
        System.Threading.Tasks.Parallel.For(0, 2000, i =>
        {
            Assert.Equal("`true` " + (i % 50), JsContent.MapCodeTags("`True` " + (i % 50)));
        });
    }
}
