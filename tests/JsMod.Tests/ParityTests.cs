using System.Linq;
using Xunit;
using static TestSupport;

/// <summary>The game's own parser is the oracle: JS and its Python equivalent must build the same tree.</summary>
public class ParityTests
{
    [Theory]
    [InlineData("while (true) { harvest(); move(North); }", "while True:\n    harvest()\n    move(North)")]
    [InlineData("while (true) {\n  harvest()\n  move(North)\n}", "while True:\n    harvest()\n    move(North)")]
    [InlineData("while (true) {}", "while True:\n    pass")]
    [InlineData("if (a) { f(); }", "if a:\n    f()")]
    [InlineData("if (a) f();", "if a:\n    f()")]
    [InlineData("if (a) { f(); } else { g(); }", "if a:\n    f()\nelse:\n    g()")]
    [InlineData("if (a) { f(); } else if (b) { g(); } else { h(); }", "if a:\n    f()\nelif b:\n    g()\nelse:\n    h()")]
    [InlineData("for (const x of range(3)) { f(x); }", "for x in range(3):\n    f(x)")]
    [InlineData("for (let [a, b] of pairs) { f(a); }", "for [a, b] in pairs:\n    f(a)")]
    [InlineData("function f(a, b = 1) { return a + b; }\nf(2);", "def f(a, b = 1):\n    return a + b\nf(2)")]
    [InlineData("function f() { return; }", "def f():\n    return")]
    [InlineData("function f() { let x = 1; x += 1; return x; }", "def f():\n    x = 1\n    x += 1\n    return x")]
    [InlineData("function f() { function g() { return 1; } return g(); }", "def f():\n    def g():\n        return 1\n    return g()")]
    [InlineData("if (a) { function g() { return 1; } }", "if a:\n    def g():\n        return 1")]
    [InlineData("let x = 1;", "x = 1")]
    [InlineData("var x = 1", "x = 1")]
    [InlineData("const x = 1\nlet y = x", "x = 1\ny = x")]
    [InlineData("let a = 1, b = 2;", "a = 1\nb = 2")]
    [InlineData("let [a, b] = c;", "[a, b] = c")]
    [InlineData("x += 1; x -= 2; x *= 3; x /= 4; x %= 5;", "x += 1\nx -= 2\nx *= 3\nx /= 4\nx %= 5")]
    [InlineData("x++;", "x += 1")]
    [InlineData("x--;", "x -= 1")]
    [InlineData("++x;", "x += 1")]
    [InlineData("a[0] = 1;", "a[0] = 1")]
    [InlineData("y = a + b * c - d / e % f;", "y = a + b * c - d / e % f")]
    [InlineData("y = (a + b) * c;", "y = (a + b) * c")]
    [InlineData("y = a - b - c;", "y = a - b - c")]
    [InlineData("y = -a ** b;", "y = -a ** b")]
    [InlineData("y = a && b || c && d;", "y = a and b or c and d")]
    [InlineData("y = !a;", "y = not a")]
    [InlineData("y = a < b;", "y = a < b")]
    [InlineData("y = a <= b;", "y = a <= b")]
    [InlineData("y = a > b;", "y = a > b")]
    [InlineData("y = a >= b;", "y = a >= b")]
    [InlineData("y = a == b;", "y = a == b")]
    [InlineData("y = a === b;", "y = a == b")]
    [InlineData("y = a != b;", "y = a != b")]
    [InlineData("y = a !== b;", "y = a != b")]
    [InlineData("y = a in b;", "y = a in b")]
    [InlineData("y = [1, 2, 3];", "y = [1, 2, 3]")]
    [InlineData("y = [];", "y = []")]
    [InlineData("y = [\n  1,\n  2,\n];", "y = [\n  1,\n  2,\n]")]
    [InlineData("y = a[0];", "y = a[0]")]
    [InlineData("y = a[i + 1][j];", "y = a[i + 1][j]")]
    [InlineData("y = a.b;", "y = a.b")]
    [InlineData("a.append(1);", "a.append(1)")]
    [InlineData("a\n  .append(1);", "a.append(1)")]
    [InlineData("f(a,\n  b);", "f(a,\n  b)")]
    [InlineData("y = a +\n  b;", "y = a + b")]
    [InlineData("let d = a\n  - b;", "d = a - b")]
    [InlineData("y = a\n  + b;", "y = a + b")]
    [InlineData("y = a\n\n// note\n  * b;", "y = a * b")]
    [InlineData("y = a\n  && b\n  || c;", "y = a and b or c")]
    [InlineData("y = a\n  == b;", "y = a == b")]
    [InlineData("y = a\n  in b;", "y = a in b")]
    [InlineData("y = a\n  .b;", "y = a.b")]
    [InlineData("f\n(1);", "f(1)")]
    [InlineData("a\n[0] = 1;", "a[0] = 1")]
    [InlineData("x\n  = 1;", "x = 1")]
    [InlineData("x\n  += 1;", "x += 1")]
    [InlineData("let x\n  = 1;", "x = 1")]
    [InlineData("let a = 1\n  , b = 2;", "a = 1\nb = 2")]
    [InlineData("let x\n(1);", "x = None\n(1)")]
    [InlineData("x = 1 /*\n*/ + 2;", "x = 1 + 2")]
    [InlineData("a /*\n*/ b;", "a\nb")]
    [InlineData("a\n++b;", "a\nb += 1")]
    [InlineData("a\n--b;", "a\nb -= 1")]
    [InlineData("y = a\n!b;", "y = a\nnot b")]
    [InlineData("y = a\nb;", "y = a\nb")]
    [InlineData("function f() { return\nx; }", "def f():\n    return\n    x")]
    [InlineData("function f() { return /*\n*/ x; }", "def f():\n    return\n    x")]
    [InlineData("function f() { return a\n  + b; }", "def f():\n    return a + b")]
    [InlineData("harvest = 1;", "harvest = 1")]
    [InlineData("y = {a: 1, \"b\": 2};", "y = {\"a\": 1, \"b\": 2}")]
    [InlineData("y = {};", "y = {}")]
    [InlineData("y = true;", "y = True")]
    [InlineData("y = false;", "y = False")]
    [InlineData("y = null;", "y = None")]
    [InlineData("y = undefined;", "y = None")]
    [InlineData("y = 'hi';", "y = 'hi'")]
    [InlineData("y = \"hi\";", "y = \"hi\"")]
    [InlineData("y = 1.5;", "y = 1.5")]
    [InlineData("move(East); move(West); move(South);", "move(East)\nmove(West)\nmove(South)")]
    [InlineData("// note\nf(); /* more */ g();", "# note\nf()\ng()")]
    [InlineData("while (a) { if (b) { break; } else { continue; } }", "while a:\n    if b:\n        break\n    else:\n        continue")]
    [InlineData("", "")]
    public void BuildsSameTreeAsPython(string js, string py)
    {
        Assert.Equal(Dump(ParsePy(py).syntaxTree), Dump(ParseJs(js).syntaxTree));
    }

    /// <summary>Only for constructs spelled the same in both languages. `def` and `True`, for example, can't match `function` and `true`.</summary>
    [Theory]
    [InlineData("while (a) { harvest(); move(North); }", "while a:\n    harvest()\n    move(North)")]
    [InlineData("if (a) { f(); } else { g(); }", "if a:\n    f()\nelse:\n    g()")]
    [InlineData("for (const x of y) { f(x); }", "for x in y:\n    f(x)")]
    [InlineData("x = a + b * c;", "x = a + b * c")]
    [InlineData("x += 1;", "x += 1")]
    [InlineData("y = a[0];", "y = a[0]")]
    [InlineData("y = a.b;", "y = a.b")]
    [InlineData("y = a < b;", "y = a < b")]
    public void NodesCoverTheSameSourceTextAsPython(string js, string py)
    {
        Assert.Equal(Spans(ParsePy(py).syntaxTree, py), Spans(ParseJs(js).syntaxTree, Marker + js));
    }

    [Fact]
    public void TopLevelFunctionsAreRegisteredWithTheWindow()
    {
        var window = FakeWindow();
        JsMod.Lang.JsParser.Parse(JsMod.Lang.JsTokenizer.Tokenize(Marker + "function f() { function inner() {} }\nif (a) { function g() {} }"), window);
        Assert.True(window.parsedFunctions.ContainsKey("f"));
        Assert.False(window.parsedFunctions.ContainsKey("inner"));
        Assert.False(window.parsedFunctions.ContainsKey("g"));
    }

    [Fact]
    public void AllVarsMatchPython()
    {
        var js = ParseJs("let a = 1;\nfunction f(p) { let q = p; return q; }\nfor (const x of y) { b = x; }");
        var py = ParsePy("a = 1\ndef f(p):\n    q = p\n    return q\nfor x in y:\n    b = x");
        Assert.Equal(py.allVars.OrderBy(v => v), js.allVars.OrderBy(v => v));
    }
}
