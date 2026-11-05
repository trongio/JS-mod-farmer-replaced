using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using JsMod.Lang;
using Xunit;

public class HighlighterTests
{
    private static string GroupOf(string code, string text)
    {
        foreach (Match match in Regex.Matches(code, JsHighlighter.Pattern))
        {
            if (match.Value == text)
            {
                foreach (string name in new[] { "comment", "string", "keyword", "function", "builtin", "number" })
                {
                    if (match.Groups[name].Success)
                    {
                        return name;
                    }
                }
            }
        }
        return null;
    }

    [Theory]
    [InlineData("// @js", "// @js", "comment")]
    [InlineData("a /* x\ny */ b", "/* x\ny */", "comment")]
    [InlineData("a /* open", "/* open", "comment")]
    [InlineData("a = 'hi';", "'hi'", "string")]
    [InlineData("a = \"hi\";", "\"hi\"", "string")]
    [InlineData("a = `h\ni`;", "`h\ni`", "string")]
    [InlineData("let a;", "let", "keyword")]
    [InlineData("const a;", "const", "keyword")]
    [InlineData("function f() {}", "function", "keyword")]
    [InlineData("for (x of y) {}", "of", "keyword")]
    [InlineData("y = true;", "true", "keyword")]
    [InlineData("y = null;", "null", "keyword")]
    [InlineData("harvest();", "harvest", "function")]
    [InlineData("move(North);", "North", "builtin")]
    [InlineData("x = Items.Hay;", "Hay", "builtin")]
    [InlineData("x = 1.5;", "1.5", "number")]
    [InlineData("x = 0xFF;", "0xFF", "number")]
    [InlineData("x = 1e3;", "1e3", "number")]
    public void ClassifiesTokens(string code, string text, string group)
    {
        Assert.Equal(group, GroupOf(code, text));
    }

    [Fact]
    public void KeywordsInsideCommentsAndStringsAreNotColouredAsKeywords()
    {
        Assert.Null(GroupOf("// let x = 1", "let"));
        Assert.Null(GroupOf("x = 'let'", "let"));
    }

    [Fact]
    public void PythonCodeKeepsTheOriginalPattern()
    {
        Assert.Equal("original", JsHighlighter.Select("original", "while True:\n    pass"));
        Assert.Equal(JsHighlighter.Pattern, JsHighlighter.Select("original", "// @js\nfoo()"));
    }

    /// <summary>The Harmony transpiler relies on this literal. If an update changes the game's method, this test says so.</summary>
    [Fact]
    public void GameHighlighterStillLoadsTheExpectedPatternFromALiteral()
    {
        MethodInfo method = typeof(CodeUtilities).GetMethod("SyntaxColor2");
        byte[] il = method.GetMethodBody().GetILAsByteArray();
        const byte ldstr = 0x72;
        bool found = false;
        for (int i = 0; i + 4 < il.Length && !found; i++)
        {
            if (il[i] != ldstr || il[i + 4] != 0x70)
            {
                continue;
            }
            try
            {
                string text = method.Module.ResolveString(System.BitConverter.ToInt32(il, i + 1));
                found = text.StartsWith("(?<comment>");
                if (found)
                {
                    foreach (string name in new[] { "comment", "string", "keyword", "function", "builtin", "number" })
                    {
                        Assert.Contains("(?<" + name + ">", text);
                        Assert.Contains("(?<" + name + ">", JsHighlighter.Pattern);
                    }
                }
            }
            catch (System.ArgumentException)
            {
            }
        }
        Assert.True(found);
    }
}
