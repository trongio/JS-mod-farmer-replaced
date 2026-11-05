using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using JsMod.Editor;
using JsMod.Lang;
using UnityEngine;
using Xunit;
using Xunit.Abstractions;
using static TestSupport;

/// <summary>The formatter and line actions. Formatting a program must never change what it does, so the corpus runs both versions.</summary>
public class EditorTests
{
    private readonly ITestOutputHelper output;

    public EditorTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    private static string FormatPy(string code, bool tabs = false)
    {
        GateResult gate = FormatGate.Run(code, false, tabs);
        Assert.True(gate.Outcome == FormatOutcome.Formatted || gate.Outcome == FormatOutcome.Unchanged, code + " => " + gate.Outcome);
        return gate.Result.Text;
    }

    private static string FormatJs(string code, bool tabs = false)
    {
        GateResult gate = FormatGate.Run(code, true, tabs);
        Assert.True(gate.Outcome == FormatOutcome.Formatted || gate.Outcome == FormatOutcome.Unchanged, code + " => " + gate.Outcome);
        return gate.Result.Text;
    }

    [Theory]
    [InlineData("x=1+2", "x = 1 + 2")]
    [InlineData("x  =  foo( 1,2 )", "x = foo(1, 2)")]
    [InlineData("y=-1", "y = -1")]
    [InlineData("y=x-1", "y = x - 1")]
    [InlineData("z=[1,2 ,3]", "z = [1, 2, 3]")]
    [InlineData("z=a [0]", "z = a[0]")]
    [InlineData("d={1:2,3:4}", "d = {1: 2, 3: 4}")]
    [InlineData("a=b**2", "a = b ** 2")]
    [InlineData("a=b//2", "a = b // 2")]
    [InlineData("a=x if_y", null)]
    [InlineData("b=not(x)", "b = not (x)")]
    [InlineData("b=x   not   in y", "b = x not in y")]
    [InlineData("b=x and(y or z)", "b = x and (y or z)")]
    [InlineData("s = 'a  ,b'   # note  x", "s = 'a  ,b'  # note  x")]
    [InlineData("n=.5", "n = .5")]
    [InlineData("a.b( c )", "a.b(c)")]
    [InlineData("from x import *", "from x import *")]
    [InlineData("def f( a,b=1 ):\n  return a", "def f(a, b=1):\n    return a")]
    [InlineData("if x==1 :\n\tpass", "if x == 1:\n    pass")]
    public void PythonSpacing(string code, string expected)
    {
        if (expected == null)
        {
            Assert.Equal(FormatOutcome.DoesNotParse, FormatGate.Run(code, false, false).Outcome);
            return;
        }
        Assert.Equal(expected, FormatPy(code));
    }

    [Theory]
    [InlineData("let x=1+2;", "let x = 1 + 2;")]
    [InlineData("let  y  =  foo( 1,2 ) ;", "let y = foo(1, 2);")]
    [InlineData("y=-1;", "y = -1;")]
    [InlineData("y=x-1;", "y = x - 1;")]
    [InlineData("i ++;", "i++;")]
    [InlineData("++ i;", "++i;")]
    [InlineData("a=!b&&c;", "a = !b && c;")]
    [InlineData("a=x===y;", "a = x === y;")]
    [InlineData("if(x){y();}", "if (x) { y(); }")]
    [InlineData("while(true){}", "while (true) {}")]
    [InlineData("for(let i=0;i<4;i++){f(i);}", "for (let i = 0; i < 4; i++) { f(i); }")]
    [InlineData("for(const x of y){}", "for (const x of y) {}")]
    [InlineData("let o={a:1,b:2};", "let o = { a: 1, b: 2 };")]
    [InlineData("let a=[1,2 ,3];", "let a = [1, 2, 3];")]
    [InlineData("let s='a  ,b';// note  x", "let s = 'a  ,b';  // note  x")]
    [InlineData("function f( a,b=1 ){return a;}", "function f(a, b = 1) { return a; }")]
    [InlineData("import {a,b} from 'x';", "import { a, b } from 'x';")]
    [InlineData("function f(){\nreturn 1;\n}", "function f() {\n    return 1;\n}")]
    [InlineData("if (a) {\n  b();\n} else {\n      c();\n  }", "if (a) {\n    b();\n} else {\n    c();\n}")]
    [InlineData("f([\n1,\n2\n]);", "f([\n    1,\n    2\n]);")]
    [InlineData("let d = a\n- b;", "let d = a\n    - b;")]
    [InlineData("x = 1 /* a\n  b  */ + 2;", "x = 1 /* a\n  b  */ + 2;")]
    public void JsSpacing(string code, string expected)
    {
        Assert.Equal(expected, FormatJs(code));
    }

    [Fact]
    public void IndentUnitFollowsTheOption()
    {
        Assert.Equal("if x:\n\ty()", FormatPy("if x:\n    y()", tabs: true));
        Assert.Equal("if x:\n    y()", FormatPy("if x:\n\ty()", tabs: false));
        Assert.Equal("if (x) {\n\ty();\n}", FormatJs("if (x) {\n    y();\n}", tabs: true));
    }

    [Fact]
    public void NestedPythonBlocksReindentAndDedent()
    {
        string code = "def f():\n  if x:\n      y()\n  z()\nw()";
        Assert.Equal("def f():\n    if x:\n        y()\n    z()\nw()", FormatPy(code));
    }

    [Fact]
    public void CommentsAndBlankLinesAreKept()
    {
        string py = "# top\nx=1  # one\n\n\n\n\ndef f():\n\n\n\n  # inside\n  y=2\n\n\n\nz=3\n";
        Assert.Equal("# top\nx = 1  # one\n\n\ndef f():\n\n    # inside\n    y = 2\n\n\nz = 3\n", FormatPy(py));
        string js = "// top\nlet x=1; // one\n\n\n\nfunction f(){\n\n\n  // inside\n  y=2;\n}\n";
        Assert.Equal("// top\nlet x = 1;  // one\n\n\nfunction f() {\n\n    // inside\n    y = 2;\n}\n", FormatJs(js));
    }

    [Fact]
    public void MarkerLinesAreLeftWhereTheyAre()
    {
        Assert.StartsWith("// @js\n", FormatJs("// @js\nlet x=1;"));
        Assert.StartsWith("# @py\n", FormatPy("# @py\nx=1"));
    }

    [Theory]
    [InlineData("x =", false)]
    [InlineData("let = 3;", true)]
    [InlineData("if (", true)]
    [InlineData("let s = 'abc", true)]
    [InlineData("/* never closed", true)]
    public void CodeThatDoesNotParseIsRefusedAndUntouched(string code, bool isJs)
    {
        GateResult gate = FormatGate.Run(code, isJs, false);
        Assert.Equal(FormatOutcome.DoesNotParse, gate.Outcome);
        Assert.Null(gate.Result);
    }

    [Fact]
    public void AnExceptionOfAnyKindAlsoLeavesTheCodeAlone()
    {
        // Without Unity, this syntax error fails while localizing its message, which isn't a ParseException here.
        GateResult gate = FormatGate.Run("if x", false, false);
        Assert.NotEqual(FormatOutcome.Formatted, gate.Outcome);
        Assert.NotEqual(FormatOutcome.Unchanged, gate.Outcome);
        Assert.Null(gate.Result);
    }

    [Fact]
    public void ALineCountCheckKeepsBreakpointLinesInPlace()
    {
        string code = "x=1\n\n\n\n\ny=2\n";
        FormatResult result = CodeFormatter.Format(code, false, false, collapseBlankLines: false);
        Assert.Equal("x = 1\n\n\n\n\ny = 2\n", result.Text);
        Assert.Equal(code.Count(c => c == '\n'), result.Text.Count(c => c == '\n'));
        Assert.Equal(Enumerable.Range(0, 7), result.LineMap);
    }

    [Fact]
    public void ASelectionFormatsOnlyItsLinesWithTheirRealIndent()
    {
        string code = "x=1\nif (a) {\ny=2;\nz=3;\n}";
        FormatResult result = CodeFormatter.FormatLines(code, true, false, 2, 2);
        Assert.Equal("x=1\nif (a) {\n    y = 2;\nz=3;\n}", result.Text);
    }

    [Fact]
    public void TheCaretStaysOnItsCharacter()
    {
        string code = "x=foo( 1,2 )\n\n\n\n\n\ny=2";
        FormatResult result = CodeFormatter.Format(code, false, false);
        Assert.Equal("x = foo(1, 2)\n\n\ny = 2", result.Text);
        int caret = code.IndexOf('2');
        Assert.Equal('2', result.Text[CodeFormatter.MapPosition(code, result, caret)]);
        int second = code.LastIndexOf('y');
        Assert.Equal('y', result.Text[CodeFormatter.MapPosition(code, result, second)]);
        Assert.Equal(result.Text.Length, CodeFormatter.MapPosition(code, result, code.Length));
    }

    [Fact]
    public void AVerticalTabIsALineBreakToo()
    {
        Assert.Equal("x = 1\ny = 2", FormatPy("x=1\vy=2"));
    }

    [Fact]
    public void UnreadableTextGivesNoResult()
    {
        Assert.Null(CodeFormatter.Format("x = @", false, false));
        Assert.Null(CodeFormatter.Format("if x:\n    y\n  z", false, false));
    }

    // Corpus: every docs example in both languages.

    private static readonly List<JsContent.Pair> Examples = JsContent.ParsePairs(File.ReadAllText(Path.Combine(ContentTests.ContentFolder, JsContent.ExamplesFile)));
    private static readonly Regex GameConstant = new Regex(@"\b(Items|Entities|Grounds|Unlocks|Hats|Leaderboards)\b(\.\w+)?");
    private static readonly Regex FarmCall = new Regex(@"\b(?!print\b|len\b|str\b|range\b|set\b|dict\b|list\b|min\b|max\b|abs\b)[a-z_][a-z0-9_]*\(");
    private static readonly HashSet<string> MeantToFail = new HashSet<string> { "docs/scripting/functions.md#2" };

    private static string Stub(string code)
    {
        return GameConstant.Replace(code, m => m.Value.Replace('.', '_') + "_");
    }

    /// <summary>Scrambles spacing the way a person might, without changing any token, so the formatter has real work to do.</summary>
    private static string Scramble(string code, bool isJs)
    {
        return string.Join("\n", code.Split('\n').Select((line, i) =>
        {
            string body = line.TrimStart(' ', '\t');
            if (body.StartsWith(isJs ? "//" : "#") || body.Contains('"') || body.Contains('\'') || body.Contains('#') || body.Contains("//") || body.Contains("not in") || body.Contains("/*"))
            {
                return line + "  ";
            }
            string spaced = Regex.Replace(body, @"\s*,\s*", ",");
            spaced = Regex.Replace(spaced, @"\s*(==|!=|<=|>=|\+=|-=)\s*", " $1  ");
            return line.Substring(0, line.Length - body.Length) + spaced + " ";
        })) + "\n\n\n\n\n";
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void EveryExampleFormatsSafely(bool isJs)
    {
        var refused = new List<string>();
        int formatted = 0;
        int unchanged = 0;
        foreach (JsContent.Pair example in Examples.Where(e => !MeantToFail.Contains(e.Name)))
        {
            string code = Stub(isJs ? example.Js : example.Python);
            foreach (string source in new[] { code, Scramble(code, isJs) })
            {
                foreach (bool tabs in new[] { false, true })
                {
                    GateResult gate = FormatGate.Run(source, isJs, tabs);
                    if (gate.Outcome == FormatOutcome.Formatted)
                    {
                        formatted++;
                        Assert.Equal(Dump(Parse(source, isJs)), Dump(Parse(gate.Result.Text, isJs)));
                        Assert.Equal(gate.Result.Text, CodeFormatter.Format(gate.Result.Text, isJs, tabs).Text);
                    }
                    else if (gate.Outcome == FormatOutcome.Unchanged)
                    {
                        unchanged++;
                    }
                    else
                    {
                        refused.Add(example.Name + " " + gate.Outcome + ":\n" + source);
                    }
                }
            }
        }
        output.WriteLine((isJs ? "JS" : "Python") + " formatted " + formatted + ", unchanged " + unchanged + ", refused " + refused.Count);
        Assert.True(refused.Count == 0, string.Join("\n---\n", refused));
    }

    private static Node Parse(string code, bool isJs)
    {
        return isJs ? ParseJs(code).syntaxTree : ParsePy(code).syntaxTree;
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void FormattingAnExampleKeepsItsExactOpCount(bool isJs)
    {
        // print needs the farm, so a function stands in for it, as in the content tests.
        string print = isJs ? "function print(a = 0, b = 0, c = 0, d = 0) {}\n" : "def print(a = 0, b = 0, c = 0, d = 0):\n    pass\n";
        int ran = 0;
        foreach (JsContent.Pair example in Examples.Where(e => !MeantToFail.Contains(e.Name)))
        {
            if (FarmCall.IsMatch(example.Js) || Regex.IsMatch(example.Js, @"\bwhile \(true\)"))
            {
                continue;
            }
            string code = isJs ? example.Js : example.Python;
            List<string> names = GameConstant.Matches(code).Select(m => m.Value.Replace('.', '_') + "_").Distinct().ToList();
            string prefix = print + string.Concat(names.Select(n => isJs ? "let " + n + " = 0;\n" : n + " = 0\n"));
            string source = Stub(code);
            string formatted = FormatGate.Run(Scramble(source, isJs), isJs, false).Result.Text;
            Assert.NotEqual(Scramble(source, isJs), formatted);
            double before = isJs ? RunJs(prefix + source).Ops : RunPy(prefix + source).Ops;
            double after = isJs ? RunJs(prefix + formatted).Ops : RunPy(prefix + formatted).Ops;
            Assert.True(before == after, example.Name + ": " + before + " became " + after);
            ran++;
        }
        output.WriteLine((isJs ? "JS" : "Python") + " op parity checked on " + ran + " examples");
        Assert.True(ran >= 43, "Only " + ran + " examples ran");
    }

    // Line actions.

    [Fact]
    public void DuplicateCopiesTheLineOrTheSelection()
    {
        EditResult line = LineOps.Duplicate("ab\ncd", 1, 1);
        Assert.Equal("ab\nab\ncd", line.Text);
        Assert.Equal(4, line.SelectionStart);
        EditResult selection = LineOps.Duplicate("abcd", 1, 3);
        Assert.Equal("abcbcd", selection.Text);
        Assert.Equal(3, selection.SelectionStart);
        Assert.Equal(5, selection.SelectionEnd);
        Assert.Equal("ab\ncd\ncd", LineOps.Duplicate("ab\ncd", 4, 4).Text);
    }

    [Fact]
    public void DeleteRemovesWholeLines()
    {
        Assert.Equal("b\nc", LineOps.DeleteLines("a\nb\nc", 0, 0).Text);
        Assert.Equal("a\nc", LineOps.DeleteLines("a\nb\nc", 2, 2).Text);
        Assert.Equal("a\nb", LineOps.DeleteLines("a\nb\nc", 4, 4).Text);
        Assert.Equal("", LineOps.DeleteLines("only", 2, 2).Text);
        Assert.Equal("c", LineOps.DeleteLines("a\nb\nc", 0, 3).Text);
        Assert.Equal("b", LineOps.DeleteLines("a\nb", 0, 2).Text);
        Assert.Null(LineOps.DeleteLines("", 0, 0));
    }

    [Fact]
    public void MovingLinesSwapsWithTheNeighbour()
    {
        EditResult up = LineOps.MoveLines("a\nbb\nc", 3, 3, true);
        Assert.Equal("bb\na\nc", up.Text);
        Assert.Equal(1, up.SelectionStart);
        EditResult down = LineOps.MoveLines("a\nbb\nc", 3, 3, false);
        Assert.Equal("a\nc\nbb", down.Text);
        Assert.Equal(5, down.SelectionStart);
        Assert.Null(LineOps.MoveLines("a\nb", 0, 0, true));
        Assert.Null(LineOps.MoveLines("a\nb", 2, 2, false));
        Assert.Equal("c\na\nb", LineOps.MoveLines("a\nb\nc", 0, 3, false).Text);
    }

    [Theory]
    [InlineData("x = 1", "# x = 1", false)]
    [InlineData("# x = 1", "x = 1", false)]
    [InlineData("#x = 1", "x = 1", false)]
    [InlineData("x = 1", "// x = 1", true)]
    [InlineData("// x = 1", "x = 1", true)]
    public void CommentTogglesUseTheFilesLanguage(string code, string expected, bool isJs)
    {
        Assert.Equal(expected, LineOps.ToggleComment(code, 0, 0, isJs).Text);
    }

    [Fact]
    public void CommentingLinesKeepsThemAlignedAndSkipsBlanks()
    {
        string code = "if x:\n    y\n\n    z";
        EditResult on = LineOps.ToggleComment(code, 0, code.Length, false);
        Assert.Equal("# if x:\n#     y\n\n#     z", on.Text);
        Assert.Equal(code, LineOps.ToggleComment(on.Text, 0, on.Text.Length, false).Text);
        Assert.Equal("    # y", LineOps.ToggleComment("    y", 4, 4, false).Text);
        Assert.Equal(6, LineOps.ToggleComment("    y", 4, 4, false).SelectionStart);
        Assert.Null(LineOps.ToggleComment("   ", 0, 0, false));
    }

    [Fact]
    public void NewLineBelowKeepsIndentAndOpensBlocks()
    {
        EditResult plain = LineOps.NewLineBelow("    a\nb", 5, 5, false);
        Assert.Equal("    a\n    \nb", plain.Text);
        Assert.Equal(10, plain.SelectionStart);
        Assert.Equal("if x:\n\t", LineOps.NewLineBelow("if x:", 2, 2, false).Text);
        Assert.Equal("    if (x) {\n        ", LineOps.NewLineBelow("    if (x) {", 2, 2, true).Text);
    }

    [Fact]
    public void JoinLinesTrimsAndSpaces()
    {
        Assert.Equal("a b\nc", LineOps.JoinLines("a\n    b\nc", 0, 0).Text);
        Assert.Equal("f(a)", LineOps.JoinLines("f(\n    a\n)", 0, 10).Text);
        Assert.Null(LineOps.JoinLines("last", 1, 1));
    }

    [Fact]
    public void ToggleCaseUsesTheSelectionOrWord()
    {
        EditResult result = LineOps.ToggleCase("abc def", 5, 5);
        Assert.Equal("abc DEF", result.Text);
        Assert.Equal("abc def", LineOps.ToggleCase("abc DEF", 4, 7).Text);
        Assert.Null(LineOps.ToggleCase("  ", 1, 1));
    }

    [Fact]
    public void WordAndCalleeLookups()
    {
        Assert.Equal("Items.Carrot", LineOps.WordAt("plant(Items.Carrot)", 8, out int start));
        Assert.Equal(6, start);
        Assert.Equal("plant", LineOps.CalleeAt("plant(Items.Carrot)", 8, out start));
        Assert.Equal(0, start);
        Assert.Equal("g", LineOps.CalleeAt("f(1, g(2, 3", 10, out _));
        Assert.Equal("f", LineOps.CalleeAt("f(g(1), ", 8, out _));
        Assert.Equal("f", LineOps.CalleeAt("f('(', ", 7, out _));
        Assert.Equal("", LineOps.CalleeAt("x = 1", 3, out _));
    }

    // Bindings and text.

    [Fact]
    public void DefaultsMatchThePlanAndAvoidTheGamesKeys()
    {
        string[] stock = { "Ctrl Space", "Ctrl Alt H", "Tab", "Escape", "S", "A", "D", "W", "F11", "Ctrl F5", "Ctrl Shift Z", "Ctrl Y", "Ctrl S", "Ctrl F", "F5", "Shift F5", "Ctrl Z", "Shift Tab" };
        foreach (EditorBindings.Binding binding in EditorBindings.All)
        {
            Assert.DoesNotContain(binding.DefaultKeys, stock);
            Assert.DoesNotContain("Ctrl Alt UpArrow", binding.DefaultKeys);
            if (binding.DefaultKeys.Length > 0)
            {
                Assert.NotEqual(KeyCode.None, EditorBindings.Parse(binding.DefaultKeys).key);
            }
        }
        Assert.Empty(EditorBindings.All.Single(b => b.Action == EditorAction.DeleteLine).DefaultKeys);
        Assert.Equal(KeyCode.L, EditorBindings.Parse("Ctrl Alt L").key);
        Assert.Equal(EditorBindings.All.Count, EditorBindings.All.Select(b => b.DefaultKeys).Where(k => k != "").Distinct().Count() + 1);
    }

    [Fact]
    public void FindMatchesExactModifiers()
    {
        Func<EditorBindings.Binding, KeyCombination> defaults = b => EditorBindings.Parse(b.DefaultKeys);
        Assert.Equal(EditorAction.Run, EditorBindings.Find(defaults, KeyCode.Return, true, false, false).Action);
        Assert.Equal(EditorAction.NewLineBelow, EditorBindings.Find(defaults, KeyCode.Return, false, false, true).Action);
        Assert.Null(EditorBindings.Find(defaults, KeyCode.Return, false, false, false));
        Assert.Equal(EditorAction.Format, EditorBindings.Find(defaults, KeyCode.L, true, true, false).Action);
        Assert.Null(EditorBindings.Find(defaults, KeyCode.L, true, false, false));
        Assert.Equal(EditorAction.Duplicate, EditorBindings.Find(defaults, KeyCode.D, true, false, false).Action);
        Assert.Null(EditorBindings.Find(defaults, KeyCode.D, false, false, false));
        Assert.Equal(EditorAction.MoveUp, EditorBindings.Find(defaults, KeyCode.UpArrow, false, true, true).Action);
        Assert.Null(EditorBindings.Find(defaults, KeyCode.Y, true, false, false));
        Assert.Null(EditorBindings.Find(defaults, KeyCode.None, false, false, false));
    }

    [Fact]
    public void ThePresetSwapsRedoAndDeleteLine()
    {
        var redo = EditorBindings.Preset.Single(p => p.option == "redo 2");
        var delete = EditorBindings.Preset.Single(p => p.option == EditorBindings.DeleteLineOption);
        Assert.Equal(redo.game, delete.phpStorm);
        Assert.Equal("", redo.phpStorm);
        Assert.Equal("", delete.game);
    }

    [Theory]
    [InlineData("EN")]
    [InlineData("KA")]
    [InlineData("DE")]
    public void EveryOptionAndPageHasText(string language)
    {
        foreach (string key in EditorBindings.All.Select(b => b.OptionName).Concat(new[]
        {
            EditorBindings.MasterOption, EditorBindings.FormatOnRunOption, EditorBindings.FormatOnSaveOption, EditorBindings.KeymapOption,
            EditorBindings.MasterOption + " tooltip", EditorBindings.FormatOnRunOption + " tooltip", EditorBindings.FormatOnSaveOption + " tooltip", EditorBindings.KeymapOption + " tooltip",
            "editor shortcut tooltip", "editor button format", "editor button copy", "editor button help", "editor copy label", "editor copied label", "editor format unsafe",
            "editor shortcuts title", "editor shortcuts intro", "editor shortcut unbound"
        }))
        {
            string text = ModStrings.Lookup(key, language);
            Assert.False(string.IsNullOrEmpty(text), key);
            Assert.DoesNotContain("\u2014", text);
            if (language == "KA")
            {
                Assert.Matches("[\u10D0-\u10FF]", text);
            }
        }
    }

    [Fact]
    public void TheShortcutsPageShowsCurrentKeysAndUnboundOnes()
    {
        string page = EditorBindings.ShortcutsPage("EN", b => b.DefaultKeys, o => "F5", o => "label " + o);
        Assert.Contains("# Editor shortcuts", page);
        Assert.Contains("`Ctrl Alt L`: Format code", page);
        Assert.Contains("Not set: Delete line", page);
        Assert.Contains("`F5`: label start execution", page);
        Assert.Contains("კოდის ფორმატირება", EditorBindings.ShortcutsPage("KA", b => b.DefaultKeys, o => "F5", o => o));
    }
}
