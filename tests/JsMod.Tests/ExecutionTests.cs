using System.Linq;
using Xunit;
using static TestSupport;

/// <summary>Runs programs in the game's real interpreter nodes, comparing results and tick costs with Python.</summary>
public class ExecutionTests
{
    [Theory]
    [InlineData("let s = 0; let i = 0; while (i < 5) { s += i; i += 1; }", "s = 0\ni = 0\nwhile i < 5:\n    s += i\n    i += 1")]
    [InlineData("let s = 0; for (const x of [1, 2, 3]) { s += x; }", "s = 0\nfor x in [1, 2, 3]:\n    s += x")]
    [InlineData("let s = 0; function add(a, b = 2) { return a + b; }\ns = add(1) + add(2, 3);", "s = 0\ndef add(a, b = 2):\n    return a + b\ns = add(1) + add(2, 3)")]
    [InlineData("let s = 1; if (s > 5 && s < 9 || s == 1) { s = 2; } else { s = 3; }", "s = 1\nif s > 5 and s < 9 or s == 1:\n    s = 2\nelse:\n    s = 3")]
    [InlineData("let s = 0; let i = 0; while (true) { i += 1; if (i > 9) { break; } if (i % 2 == 0) { continue; } s += i; }", "s = 0\ni = 0\nwhile True:\n    i += 1\n    if i > 9:\n        break\n    if i % 2 == 0:\n        continue\n    s += i")]
    public void SameResultAndSameOpsAsPython(string js, string py)
    {
        var fromJs = RunJs(js);
        var fromPy = RunPy(py);
        Assert.Equal(fromPy.Number("s"), fromJs.Number("s"));
        Assert.Equal(fromPy.Ops, fromJs.Ops);
    }

    [Fact]
    public void CStyleForCostsTheSameAsTheEquivalentWhile()
    {
        var loop = RunJs("let s = 0; for (let i = 0; i < 5; i++) { s += i; }");
        var python = RunPy("s = 0\ni = 0\nwhile i < 5:\n    s += i\n    i += 1");
        Assert.Equal(10, loop.Number("s"));
        Assert.Equal(python.Ops, loop.Ops);
    }

    [Fact]
    public void ContinueInCStyleForStillRunsTheUpdate()
    {
        var run = RunJs("let s = 0; for (let i = 0; i < 5; i++) { if (i == 2) { continue; } s += i; }");
        Assert.Equal(8, run.Number("s"));
    }

    [Fact]
    public void BreakLeavesCStyleFor()
    {
        var run = RunJs("let s = 0; for (let i = 0; ; i++) { if (i == 3) { break; } s += 1; }");
        Assert.Equal(3, run.Number("s"));
    }

    [Fact]
    public void ReturnPassesThroughCStyleFor()
    {
        var run = RunJs("function f() { for (let i = 0; i < 9; i++) { if (i == 4) { return i; } } return -1; }\nlet s = f();");
        Assert.Equal(4, run.Number("s"));
    }

    [Fact]
    public void NestedCStyleForBreakOnlyLeavesInnerLoop()
    {
        var run = RunJs("let s = 0; for (let i = 0; i < 3; i++) { for (let j = 0; j < 9; j++) { if (j == 2) { break; } s += 1; } }");
        Assert.Equal(6, run.Number("s"));
    }

    [Theory]
    [InlineData("while (true) {}")]
    [InlineData("for (;;) {}")]
    [InlineData("while (true) ;")]
    public void EmptyInfiniteLoopsChargeOpsInsteadOfHanging(string js)
    {
        // Bounded steps, so a loop that stops charging ops fails this test instead of hanging the test run.
        var run = RunJs(js, opLimit: 500, maxSteps: 1000);
        Assert.True(run.Ops > 500);
    }

    [Theory]
    [InlineData("let d = 9\n  - 4;", "d", 5)]
    [InlineData("let d = 2\n\n  * 4;", "d", 8)]
    [InlineData("let d = 1 /*\n*/ + 2;", "d", 3)]
    [InlineData("let a = 1; let d = 2\n++a;", "a", 2)]
    [InlineData("let a = 1; let d = 2\n++a;", "d", 2)]
    [InlineData("let d = 0; function f() { return\n7; }\nif (f() == null) { d = 1; }", "d", 1)]
    public void LineBreaksFollowJsRules(string js, string name, double expected)
    {
        Assert.Equal(expected, RunJs(js).Number(name));
    }

    [Theory]
    [InlineData("var x = 5; var x;")]
    [InlineData("let x = 5; let x;")]
    [InlineData("var x = 5; if (x) { var x; }")]
    [InlineData("var x = 5; var y, x;")]
    public void RedeclaringWithoutAValueKeepsTheOldValue(string js)
    {
        Assert.Equal(5, RunJs(js).Number("x"));
    }

    [Fact]
    public void RedeclaringAParameterWithoutAValueKeepsTheArgument()
    {
        Assert.Equal(3, RunJs("function f(p) { var p; return p; }\nlet r = f(3);").Number("r"));
    }

    [Fact]
    public void ANewNameWithoutAValueStartsAsUndefined()
    {
        Assert.IsType<PyNone>(RunJs("let x;").Value("x"));
    }

    [Fact]
    public void AForInitWithoutAValueStartsAsUndefined()
    {
        var run = RunJs("let n = 0; for (let i; n < 2; n++) { if (i == null) { i = 7; } }\nlet r = i;");
        Assert.Equal(7, run.Number("r"));
    }

    [Fact]
    public void AnEmptyLoopBodyLeftByARedeclarationStillChargesOps()
    {
        var run = RunJs("var x = 1; while (true) { var x; }", opLimit: 500, maxSteps: 1000);
        Assert.True(run.Ops > 500);
    }

    [Fact]
    public void BuiltinNamesCanBeReassignedInBothLanguages()
    {
        Assert.Equal(RunPy("harvest = 1").Number("harvest"), RunJs("let harvest = 1;").Number("harvest"));
    }

    [Fact]
    public void PythonOnlyKeywordsAreOrdinaryNamesInJs()
    {
        Assert.Equal(1, RunJs("let from = 1;").Number("from"));
        Assert.Equal(2, RunJs("let pass = 2;").Number("pass"));
    }

    [Fact]
    public void UndeclaredAssignmentInAFunctionWritesTheOuterVariable()
    {
        var run = RunJs("let c = 0;\nfunction bump() { c = c + 1; }\nbump(); bump();");
        Assert.Equal(2, run.Number("c"));
    }

    [Fact]
    public void DeclaredVariableInAFunctionShadowsTheOuterOne()
    {
        var run = RunJs("let c = 1;\nfunction f() { let c = 5; c += 1; return c; }\nlet r = f();");
        Assert.Equal(1, run.Number("c"));
        Assert.Equal(6, run.Number("r"));
    }

    [Fact]
    public void ClosuresReadAndWriteEnclosingFunctionVariables()
    {
        var run = RunJs("function outer() { let n = 1; function inner() { n = n + 1; } inner(); return n; }\nlet r = outer();");
        Assert.Equal(2, run.Number("r"));
    }

    [Fact]
    public void LogicalOperatorsReturnOperandValues()
    {
        var run = RunJs("let a = 0 || 7; let b = 3 && 9; let c = 0 && 9;");
        Assert.Equal(7, run.Number("a"));
        Assert.Equal(9, run.Number("b"));
        Assert.Equal(0, run.Number("c"));
    }

    [Fact]
    public void JsPrecedenceForNotIsTighterThanComparison()
    {
        var run = RunJs("let a = 5; let r = 0; if (!a == false) { r = 1; }");
        Assert.Equal(1, run.Number("r"));
    }

    [Fact]
    public void DeepCopiedTreeRunsLikeTheOriginalForDrones()
    {
        var tree = ParseJs("let s = 0; for (let i = 0; i < 4; i++) { s += i; }\nlet t = s + 1;").syntaxTree;
        var copy = tree.DeepCopy(new System.Collections.Generic.Dictionary<object, object>());
        Assert.NotSame(tree, copy);
        Assert.Equal(Execute(tree).Ops, Execute(copy).Ops);
        Assert.Equal(7, Execute(copy).Number("t"));
    }

    /// <summary>The one place the tree differs from Python on purpose: the game's parser makes ** left associative, JS is right.</summary>
    [Fact]
    public void ExponentIsRightAssociativeAsInJs()
    {
        Assert.Equal(512, RunJs("let r = 2 ** 3 ** 2;").Number("r"));
        Assert.Equal(64, RunPy("r = 2 ** 3 ** 2").Number("r"));
    }

    [Fact]
    public void HexAndExponentNumbersParse()
    {
        var run = RunJs("let a = 0xFF; let b = 1e3; let c = .5;");
        Assert.Equal(255, run.Number("a"));
        Assert.Equal(1000, run.Number("b"));
        Assert.Equal(0.5, run.Number("c"));
    }

    [Theory]
    [InlineData("let a = 7; let r = Math.floor(a / 2);", "a = 7\nr = a // 2")]
    [InlineData("let a = 7.5; let r = Math.floor(a);", "a = 7.5\nr = a // 1")]
    [InlineData("let a = 7; let r = Math.floor(a * 0.3);", "a = 7\nr = a * 0.3 // 1")]
    [InlineData("let a = [1]; a.push(2); let r = a.length;", "a = [1]\na.append(2)\nr = len(a)")]
    [InlineData("let a = [1, 2]; let r = 0; if (a.includes(2)) { r = 1; }", "a = [1, 2]\nr = 0\nif 2 in a:\n    r = 1")]
    [InlineData("let a = set(); a.add(2); let r = 0; if (a.has(2)) { r = 1; }", "a = set()\na.add(2)\nr = 0\nif 2 in a:\n    r = 1")]
    [InlineData("let r = Math.abs(-3) + Math.max(1, 2) + Math.min(1, 2);", "r = abs(-3) + max(1, 2) + min(1, 2)")]
    public void AliasesGiveTheSameResultAndOpsAsTheGameSpelling(string js, string py)
    {
        var fromJs = RunJs(js);
        var fromPy = RunPy(py);
        Assert.Equal(fromPy.Number("r"), fromJs.Number("r"));
        Assert.Equal(fromPy.Ops, fromJs.Ops);
    }

    [Theory]
    [InlineData("let r = Math.floor(5 / 2);", "r = 5 // 2")]
    [InlineData("let a = [1]; a.push(2);", "a = [1]\na.append(2)")]
    [InlineData("let a = [1]; let r = a.includes(1);", "a = [1]\nr = 1 in a")]
    public void AliasesBuildTheGameTree(string js, string py)
    {
        Assert.Equal(Dump(ParsePy(py).syntaxTree), Dump(ParseJs(js).syntaxTree));
    }

    [Fact]
    public void DeclaredVariablesNamedLikeAliasesStayOrdinary()
    {
        Assert.Equal(4, RunJs("function g() { return 4; }\nlet Math = {\"floor\": g};\nlet r = Math[\"floor\"]();").Number("r"));
        var error = Assert.Throws<ParseException>(() => ParseJs("let r = Math.sin(1);"));
        Assert.Contains("Math.sin", error.Message);
    }

    [Fact]
    public void ArrayDestructuringAndSwapWork()
    {
        var run = RunJs("let [a, b] = [1, 2]; let [c, [d, e]] = [3, [4, 5]]; [a, b] = [b, a]; let r = a * 100 + e;");
        Assert.Equal(205, run.Number("r"));
    }

    [Fact]
    public void ArrayDestructuringReadsARealTuple()
    {
        Assert.Equal(12, JsTupleProgram().Number("r"));
    }

    private static Run JsTupleProgram()
    {
        // The function is defined in Python so it returns the game's real tuple, as measure() and get_companion() do.
        var pythonDefs = ParsePy("def pair():\n    return 1, (2, 3)").syntaxTree;
        var jsBody = ParseJs("let [a, [b, c]] = pair(); let r = a * 10 + b;").syntaxTree;
        var both = new SequenceNode((CodeWindow)null) { slots = { pythonDefs, jsBody } };
        return Execute(both);
    }

    [Fact]
    public void ElseIfMatchesPythonElif()
    {
        Assert.Equal(
            Dump(ParsePy("if a:\n    b()\nelif c:\n    d()\nelse:\n    e()").syntaxTree),
            Dump(ParseJs("if (a) { b(); } else if (c) { d(); } else { e(); }").syntaxTree));
    }

    [Theory]
    [InlineData("import \"lib\";", "import lib")]
    [InlineData("import * as lib from \"lib\";", "import lib")]
    [InlineData("import * from \"lib\";", "from lib import *")]
    [InlineData("import { a, b } from \"lib\";", "from lib import a, b")]
    [InlineData("import { a } from \"./lib.js\";", "from lib import a")]
    public void ImportsBuildTheSameTreeAsPython(string js, string py)
    {
        Assert.Equal(Dump(ParsePy(py).syntaxTree), Dump(ParseJs(js).syntaxTree));
        Assert.Equal(ParsePy(py).allVars.OrderBy(n => n), ParseJs(js).allVars.OrderBy(n => n));
        Assert.Equal(ParsePy(py).importedModules.OrderBy(n => n), ParseJs(js).importedModules.OrderBy(n => n));
    }

    [Theory]
    [InlineData("import * as other from \"lib\";", "other")]
    [InlineData("import lib from \"lib\";", "lib")]
    [InlineData("function f() { import \"lib\"; }", "import")]
    [InlineData("if (a) { export let b = 1; }", "export")]
    [InlineData("export 5;", "5")]
    public void BadImportsAndExportsReportAnError(string js, string word)
    {
        var error = Assert.Throws<ParseException>(() => ParseJs(js));
        string source = Marker + js;
        Assert.Contains(word, source.Substring(error.startIndex, error.endIndex - error.startIndex));
    }

    [Fact]
    public void ExportIsAcceptedOnDeclarations()
    {
        var run = RunJs("export function f() { return 2; }\nexport let a = f();\nexport const b = a + 1;");
        Assert.Equal(3, run.Number("b"));
    }

    [Fact]
    public void AModuleMemberNamedLikeAnAliasKeepsItsName()
    {
        string dump = Dump(ParseJs("import \"lib\";\nlet r = lib.length;\nlib.push(1);").syntaxTree);
        Assert.Contains("value=length", dump);
        Assert.Contains("value=push", dump);
        Assert.DoesNotContain("value=len,", dump);
    }

    [Fact]
    public void ComputedDictKeysUseTheValueNotItsName()
    {
        var run = RunJs("let d = {[North]: 1, \"a\": 2, b: 3}; let r = d[North] * 100 + d[\"a\"] * 10 + d[\"b\"];");
        Assert.Equal(123, run.Number("r"));
    }
}
