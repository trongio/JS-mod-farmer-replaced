using Xunit;
using static TestSupport;

/// <summary>Positions are source offsets, including the marker line, because the editor underlines from them.</summary>
public class ErrorTests
{
    private static ParseException ErrorIn(string js)
    {
        return Assert.Throws<ParseException>(() => ParseJs(js));
    }

    [Theory]
    [InlineData("foo() bar();", "bar")]
    [InlineData("let = 3;", "=")]
    [InlineData("y = 1 +;", ";")]
    [InlineData("if x { }", "x")]
    [InlineData("f(1 2);", "2")]
    [InlineData("y = a ? b : c;", "?")]
    [InlineData("y = a & b;", "&")]
    [InlineData("y = (x) => x;", "=>")]
    [InlineData("y = a ?? b;", "??")]
    [InlineData("y = f(x++);", "++")]
    [InlineData("if (a = 1) { }", "=")]
    [InlineData("a = b = 1;", "=", 1)]
    [InlineData("y = 'open;", "'open;")]
    [InlineData("y = 3abc;", "3abc")]
    [InlineData("/* never closed", "/* never closed")]
    [InlineData("class Foo { }", "class")]
    [InlineData("y = new Foo();", "new")]
    [InlineData("y = typeof x;", "typeof")]
    [InlineData("switch (a) { }", "switch")]
    [InlineData("y = function() {};", "function")]
    [InlineData("return 1;", "return")]
    [InlineData("break;", "break")]
    [InlineData("continue;", "continue")]
    [InlineData("const k = 1; k = 2;", "k", 1)]
    [InlineData("North = 1;", "North")]
    [InlineData("harvest;", "harvest")]
    [InlineData("for (x in y) { }", "for")]
    [InlineData("for (const x in y) { }", "in")]
    [InlineData("1 = x;", "1")]
    [InlineData("y = `a${b}`;", "`a${b}`")]
    [InlineData("y = a\n  ? b : c;", "?")]
    [InlineData("y = a\n  ?? b;", "??")]
    [InlineData("y = a\n  & b;", "&")]
    [InlineData("a = b\n  = 1;", "=", 1)]
    [InlineData("f()\n  , g();", ",")]
    public void ReportsErrorAtTheOffendingText(string js, string errorText, int occurrenceIndex = 0)
    {
        var ex = ErrorIn(js);
        string source = Marker + js;
        int start = source.IndexOf(errorText, System.StringComparison.Ordinal);
        for (int i = 0; i < occurrenceIndex; i++)
        {
            start = source.IndexOf(errorText, start + 1, System.StringComparison.Ordinal);
        }
        Assert.Equal((start, start + errorText.Length), (ex.startIndex, ex.endIndex));
        Assert.False(string.IsNullOrEmpty(ex.Message));
    }

    [Fact]
    public void MissingCloseBracePointsAtTheOpeningBrace()
    {
        var ex = ErrorIn("while (true) {\n  harvest();");
        int open = (Marker + "while (true) ").Length;
        Assert.Equal((open, open + 1), (ex.startIndex, ex.endIndex));
    }

    [Fact]
    public void CodeEndingEarlyPointsAtTheEndOfTheCode()
    {
        string js = "while (true";
        var ex = ErrorIn(js);
        int end = (Marker + js).Length;
        Assert.Equal((end, end), (ex.startIndex, ex.endIndex));
    }

    [Fact]
    public void ALetCanReuseTheNameOfAnEarlierConstInAnotherBlock()
    {
        ParseJs("if (a) { const c = 1; } else { let c = 2; c = 3; }");
    }

    [Fact]
    public void ForLoopsCanRedeclareTheirCounter()
    {
        ParseJs("for (let i = 0; i < 1; i++) { }\nfor (let i = 0; i < 1; i++) { }");
    }
}
