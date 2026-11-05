using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using JsMod.Editor;
using Xunit;

/// <summary>Rainbow indents, double Shift, the embedded icons and the undo button's stepping.</summary>
public class EditorExtrasTests
{
    private static readonly Regex Tags = new Regex("</?noparse>|<color=[^>]*>|</color>|<mark=[^>]*>|</mark>");

    private static string Plain(string code)
    {
        return "<noparse>" + code + "</noparse>";
    }

    private static string Strip(string text)
    {
        return Tags.Replace(text, "");
    }

    private static int[] Levels(string code)
    {
        return RainbowIndents.Spans(code).Select(s => s.Level).ToArray();
    }

    [Theory]
    [InlineData("a\n    b\n        c", new[] { 0, 0, 1 })]
    [InlineData("a\n\tb\n\t\tc", new[] { 0, 0, 1 })]
    [InlineData("a\n  b\n    c", new[] { 0, 0, 1 })]
    [InlineData("a\n\t    b", new[] { 0, 1 })]
    [InlineData("a\n     b", new[] { 0 })]
    [InlineData("a\n   b", new int[0])]
    [InlineData("    \n\t\nb", new int[0])]
    [InlineData("", new int[0])]
    public void LevelsFollowTabsAndTheFilesIndentSize(string code, int[] expected)
    {
        Assert.Equal(expected, Levels(code));
    }

    [Theory]
    [InlineData("a\n  b\n    c", 2)]
    [InlineData("a\n    b\n        c", 4)]
    [InlineData("a\n   b", 4)]
    [InlineData("a\n  b\n   c", 4)]
    [InlineData("a\n", 4)]
    public void IndentSizeComesFromTheFile(string code, int expected)
    {
        Assert.Equal(expected, RainbowIndents.IndentUnit(code));
    }

    [Theory]
    [InlineData("while True:\n    if x:\n        y()\n\n\tz()\n")]
    [InlineData("")]
    [InlineData("no indent")]
    [InlineData("a\n     b\n")]
    public void TheVisibleTextNeverChanges(string code)
    {
        Assert.Equal(code, Strip(RainbowIndents.Apply(Plain(code), code)));
    }

    [Fact]
    public void EachLevelGetsItsOwnBalancedMark()
    {
        string code = "a\n    b\n        c";
        string result = RainbowIndents.Apply(Plain(code), code);
        Assert.Equal(3, Regex.Matches(result, "<mark=").Count);
        Assert.Equal(3, Regex.Matches(result, "</mark>").Count);
        Assert.Contains("</noparse><mark=" + RainbowIndents.Palette[0] + "><noparse>    </noparse></mark><noparse>", result);
        Assert.Contains("<mark=" + RainbowIndents.Palette[1] + ">", result);
    }

    [Fact]
    public void ThePaletteStartsOverAndIsTranslucent()
    {
        string code = "a\n" + new string('\t', RainbowIndents.Palette.Length + 1) + "b";
        string result = RainbowIndents.Apply(Plain(code), code);
        Assert.Equal(2, Regex.Matches(result, Regex.Escape("<mark=" + RainbowIndents.Palette[0] + ">")).Count);
        Assert.All(RainbowIndents.Palette, colour => Assert.Matches("^#[0-9A-F]{6}2E$", colour));
    }

    [Fact]
    public void TheInsideOfAMultiLineCommentStaysPlain()
    {
        string code = "/* a\n    b */\n    c";
        string highlighted = "<noparse></noparse><color=#111111><noparse>/* a\n    b */</noparse></color><noparse>\n    c</noparse>";
        string result = RainbowIndents.Apply(highlighted, code);
        Assert.Single(Regex.Matches(result, "<mark="));
        Assert.Equal(code, Strip(result));
        Assert.Contains("\n</noparse><mark=", result);
    }

    [Fact]
    public void ASearchMarkInsideAnIndentStaysBalanced()
    {
        string code = "a\n    b";
        string highlighted = "<noparse>a\n  </noparse><mark=#FFFF00FF from_search><noparse>  </noparse></mark><noparse>b</noparse>";
        string result = RainbowIndents.Apply(highlighted, code);
        Assert.Equal(code, Strip(result));
        Assert.Equal(Regex.Matches(result, "<mark=").Count, Regex.Matches(result, "</mark>").Count);
        Assert.Equal(3, Regex.Matches(result, "<mark=").Count);
    }

    [Fact]
    public void ALiteralEndOfNoparseInTheCodeLeavesTheTextAlone()
    {
        string code = "x = '</noparse>'\n    y";
        string highlighted = Plain(code);
        Assert.Equal(highlighted, RainbowIndents.Apply(highlighted, code));
    }

    [Fact]
    public void WorksOnTheGamesRealHighlighterOutput()
    {
        var theme = (ColorTheme)RuntimeHelpers.GetUninitializedObject(typeof(ColorTheme));
        theme.code = new ColorTheme.CodeColors { comment = "#111111FF", @string = "#222222FF", keyword = "#333333FF", function = "#444444FF", builtin = "#555555FF", number = "#666666FF" };
        string code = "while True:\n    # note\n    if x:\n        print('  a', 1)\n\n    y = 2";
        string coloured = CodeUtilities.SyntaxColor2(code, theme);
        string result = RainbowIndents.Apply(coloured, code);
        Assert.Equal(code, Strip(result));
        Assert.Equal(5, Regex.Matches(result, "<mark=").Count);
    }

    private static bool Press(DoubleShift d, float t) => d.Feed(t, true, false, false, false);

    private static void Release(DoubleShift d, float t) => d.Feed(t, false, true, false, false);

    [Theory]
    [InlineData(0.1f, true)]
    [InlineData(0.25f, true)]
    [InlineData(0.35f, false)]
    public void TwoQuickPressesFire(float gap, bool expected)
    {
        var d = new DoubleShift();
        Assert.False(Press(d, 1f));
        Release(d, 1f + gap / 2f);
        Assert.Equal(expected, Press(d, 1f + gap));
    }

    [Fact]
    public void ASlowPressCountsAsANewFirstPress()
    {
        var d = new DoubleShift();
        Press(d, 1f);
        Release(d, 1.1f);
        Assert.False(Press(d, 2f));
        Release(d, 2.1f);
        Assert.True(Press(d, 2.2f));
    }

    [Fact]
    public void TypingCapitalsDoesNotFire()
    {
        var d = new DoubleShift();
        Press(d, 1f);
        d.Feed(1.02f, false, false, true, false);
        Release(d, 1.05f);
        Assert.False(Press(d, 1.1f));
        d.Feed(1.12f, false, false, true, false);
        Release(d, 1.15f);
        Assert.False(Press(d, 1.2f));
    }

    [Fact]
    public void AKeyOrClickBetweenPressesStartsOver()
    {
        var d = new DoubleShift();
        Press(d, 1f);
        Release(d, 1.05f);
        d.Feed(1.1f, false, false, true, false);
        Assert.False(Press(d, 1.15f));
    }

    [Fact]
    public void RepeatOrTheOtherShiftWhileHeldDoesNotFire()
    {
        var d = new DoubleShift();
        Press(d, 1f);
        Assert.False(Press(d, 1.05f));
        Assert.False(Press(d, 1.1f));
    }

    [Fact]
    public void CtrlOrAltHeldDoesNotFire()
    {
        var d = new DoubleShift();
        d.Feed(1f, true, false, false, true);
        d.Feed(1.05f, false, true, false, true);
        Assert.False(d.Feed(1.1f, true, false, false, true));
        Release(d, 1.15f);
        var e = new DoubleShift();
        Press(e, 1f);
        Release(e, 1.05f);
        Assert.False(e.Feed(1.1f, true, false, false, true));
    }

    [Fact]
    public void AThirdPressNeedsItsOwnPair()
    {
        var d = new DoubleShift();
        Press(d, 1f);
        Release(d, 1.05f);
        Assert.True(Press(d, 1.1f));
        Assert.False(Press(d, 1.12f));
        Release(d, 1.15f);
        Assert.False(Press(d, 1.2f));
        Release(d, 1.25f);
        Assert.True(Press(d, 1.3f));
    }

    [Fact]
    public void AfterResetNothingIsRemembered()
    {
        var d = new DoubleShift();
        Press(d, 1f);
        Release(d, 1.05f);
        d.Reset();
        Assert.False(Press(d, 1.1f));
    }

    [Fact]
    public void UndoButtonStepsPastEntriesThatChangeNothing()
    {
        string text = "abc";
        int steps = 0;
        Assert.True(UndoSteps.StepUntilChanged(() => text, () =>
        {
            steps++;
            text = steps == 2 ? "ab" : text;
        }));
        Assert.Equal(2, steps);
    }

    [Fact]
    public void UndoButtonStopsWhenThereIsNothingToDo()
    {
        int steps = 0;
        Assert.False(UndoSteps.StepUntilChanged(() => "abc", () => steps++));
        Assert.Equal(UndoSteps.MaxSteps, steps);
    }

    private static string RepoFile(string relative)
    {
        for (DirectoryInfo dir = new DirectoryInfo(AppContext.BaseDirectory); dir != null; dir = dir.Parent)
        {
            string path = Path.Combine(dir.FullName, relative);
            if (File.Exists(path) || Directory.Exists(path))
            {
                return path;
            }
        }
        throw new FileNotFoundException(relative);
    }

    [Fact]
    public void EveryIconIsAWhite96PixelPngWithItsSourceAndLicense()
    {
        string icons = RepoFile("src/JsMod/icons");
        Assert.True(File.Exists(Path.Combine(icons, "LICENSE")));
        foreach (string name in IconNames.All)
        {
            Assert.True(File.Exists(Path.Combine(icons, "svg", name + ".svg")), name);
            byte[] png = File.ReadAllBytes(Path.Combine(icons, name + ".png"));
            Assert.Equal(new byte[] { 0x89, 0x50, 0x4E, 0x47 }, png.Take(4).ToArray());
            Assert.Equal(96, (png[16] << 24) | (png[17] << 16) | (png[18] << 8) | png[19]);
            Assert.Equal(96, (png[20] << 24) | (png[21] << 16) | (png[22] << 8) | png[23]);
            Assert.Equal(6, png[25]);
        }
    }
}
