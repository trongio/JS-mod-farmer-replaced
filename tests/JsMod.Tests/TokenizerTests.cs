using System.Linq;
using JsMod.Lang;
using Xunit;

public class TokenizerTests
{
    [Theory]
    [InlineData("// @js", true)]
    [InlineData("//@js", true)]
    [InlineData("  // @js\nfoo()", true)]
    [InlineData("\n\n// @js\nfoo()", true)]
    [InlineData("\uFEFF// @js\n", true)]
    [InlineData("// @js some note\n", true)]
    [InlineData("// @jsx\n", false)]
    [InlineData("# // @js\n", false)]
    [InlineData("x = 1\n// @js\n", false)]
    [InlineData("while True:\n    harvest()", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void DetectsMarker(string code, bool expected)
    {
        Assert.Equal(expected, JsMarker.IsJs(code));
    }

    [Fact]
    public void StreamStartsWithSentinelThenNewLine()
    {
        var tokens = JsTokenizer.Tokenize("// @js\nfoo()").ToList();
        Assert.Equal(JsTokenizer.Sentinel, tokens[0].type);
        Assert.Equal("", tokens[0].value);
        Assert.Equal(TokenType.NEW_LINE, tokens[1].type);
    }

    [Fact]
    public void NewLineTokensCarryIndentationAndOffsets()
    {
        string code = "// @js\nif (a) {\n    b();\n\tc();\n}";
        var lines = JsTokenizer.Tokenize(code).ToList().Where(t => t.type == TokenType.NEW_LINE).Skip(1).ToList();
        Assert.Equal(new[] { "\n", "\n    ", "\n\t", "\n" }, lines.Select(t => t.value));
        foreach (var line in lines)
        {
            Assert.Equal('\n', code[line.startIndex]);
        }
    }

    [Fact]
    public void EveryTokenIsAVerbatimSliceOfTheSource()
    {
        string code = "// @js\nlet a = [1, 'two'] /* c */ ; a[0] += 0x1F; if (a !== null) { f(a); }\n";
        foreach (var token in JsTokenizer.Tokenize(code).ToList().Skip(2))
        {
            Assert.Equal(token.value, code.Substring(token.startIndex, token.value.Length));
        }
    }

    [Fact]
    public void IdentifiersExcludeKeywordsForAutocomplete()
    {
        var identifiers = JsTokenizer.Tokenize("// @js\nlet total = 0; function add(n) { return total + n; }").ToList()
            .Where(t => t.type == TokenType.IDENTIFIER).Select(t => t.value).Distinct();
        Assert.Equal(new[] { "total", "add", "n" }, identifiers);
    }

    [Fact]
    public void TrailingNewLinesAreTrimmed()
    {
        var tokens = JsTokenizer.Tokenize("// @js\nfoo()\n\n\n").ToList();
        Assert.NotEqual(TokenType.NEW_LINE, tokens.Last().type);
    }

    [Theory]
    [InlineData("\"open")]
    [InlineData("'open\nmore")]
    [InlineData("`open")]
    [InlineData("/* open")]
    [InlineData("@#~\\")]
    [InlineData("0x")]
    [InlineData("1e")]
    [InlineData(".")]
    [InlineData("\v\r\t")]
    public void NeverThrowsWhileTyping(string code)
    {
        JsTokenizer.Tokenize("// @js\n" + code);
    }

    [Fact]
    public void LongestOperatorWins()
    {
        var values = JsTokenizer.Tokenize("// @js\na === b !== c ** d").ToList().Where(t => t.type == JsTokenizer.Punct).Select(t => t.value);
        Assert.Equal(new[] { "===", "!==", "**" }, values);
    }
}
