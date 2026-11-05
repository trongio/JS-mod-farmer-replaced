using System.Linq;
using System.Reflection;
using Xunit;

/// <summary>Harmony binds patches by method and parameter name at run time. These catch a game update that renames any of them.</summary>
public class PatchTargetTests
{
    private const BindingFlags Any = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

    private static void AssertTarget(System.Type type, string method, params string[] parameters)
    {
        var overloads = type.GetMethods(Any).Where(m => m.Name == method).ToList();
        Assert.Single(overloads);
        var names = overloads[0].GetParameters().Select(p => p.Name).ToList();
        foreach (string parameter in parameters)
        {
            Assert.Contains(parameter, names);
        }
    }

    [Fact]
    public void AllPatchTargetsExistWithTheExpectedParameters()
    {
        AssertTarget(typeof(Tokenizer), "Tokenize", "code", "stream");
        AssertTarget(typeof(Parser), "Parse", "stream", "f");
        AssertTarget(typeof(CodeUtilities), "SyntaxColor2", "code");
        AssertTarget(typeof(Leaderboard), "FillLeaderboard", "score");
        AssertTarget(typeof(SteamLeaderboard), "LoadLeaderboard", "score");
        AssertTarget(typeof(CodeWindow), "InsertNewLineIndentation", "inserted");
        AssertTarget(typeof(CodeWindow), "GetWordList");
    }

    [Fact]
    public void LeaderboardBlockCoversTheOnlyUploadCaller()
    {
        // SteamLeaderboard's upload happens in Load, on a score set only through the static LoadLeaderboard factory or its private constructor.
        var constructors = typeof(SteamLeaderboard).GetConstructors(BindingFlags.Instance | BindingFlags.Public);
        Assert.Empty(constructors);
    }
}
