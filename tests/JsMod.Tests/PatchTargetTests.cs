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
        AssertTarget(typeof(Achievements), "UnlockAchievement", "achievement");
        AssertTarget(typeof(CodeWindow), "UpdateTokens");
        AssertTarget(typeof(CodeWindow), "UpdateSearch");
        AssertTarget(typeof(CodeWindow), "OnThemeChanged");
        AssertTarget(typeof(CodeWindow), "CloseError");
        AssertTarget(typeof(CodeWindow), "Parse");
        AssertTarget(typeof(Localizer), "Localize", "key");
        AssertTarget(typeof(OptionMenu), "Setup");
    }

    [Fact]
    public void ReparseFindsTheFieldsItNeeds()
    {
        Assert.Equal(typeof(bool), typeof(CodeWindow).GetField("dirty", Any)?.FieldType);
        Assert.Equal(typeof(object), typeof(CodeWindow).GetField("parseLock", Any)?.FieldType);
        Assert.NotNull(typeof(Workspace).GetField("codeWindows", Any));
        Assert.Equal(typeof(string), typeof(Localizer).GetProperty("Lang", Any)?.PropertyType);
    }

    /// <summary>The option menu transpiler swaps this call. If an update moves it, the option turns itself off, and this test says so.</summary>
    [Fact]
    public void OptionMenuStillReadsItsListFromGetAllOptions()
    {
        MethodInfo setup = typeof(OptionMenu).GetMethod("Setup", Any);
        MethodInfo getAll = typeof(ResourceManager).GetMethod("GetAllOptions", Any);
        byte[] il = setup.GetMethodBody().GetILAsByteArray();
        const byte call = 0x28;
        bool found = false;
        for (int i = 0; i + 4 < il.Length && !found; i++)
        {
            if (il[i] != call)
            {
                continue;
            }
            try
            {
                found = setup.Module.ResolveMethod(System.BitConverter.ToInt32(il, i + 1)) == getAll;
            }
            catch (System.ArgumentException)
            {
            }
        }
        Assert.True(found);
    }

    /// <summary>The option row copies this type, and the game's own option lookups must keep ignoring names they don't know.</summary>
    [Fact]
    public void OptionTypesStillHaveWhatTheRowUses()
    {
        foreach (string field in new[] { "optionUI", "optionName", "tooltip", "defaultValue", "category", "importance", "dlc" })
        {
            Assert.NotNull(typeof(OptionSO).GetField(field));
        }
        Assert.NotNull(typeof(DropdownOptionSO).GetField("options"));
        Assert.True(typeof(DropdownOptionSO).IsSubclassOf(typeof(OptionSO)));
        Assert.NotNull(typeof(OptionHolder).GetEvent("OnOptionChanged"));
    }

    [Fact]
    public void LeaderboardBlockCoversTheOnlyUploadCaller()
    {
        // SteamLeaderboard's upload happens in Load, on a score set only through the static LoadLeaderboard factory or its private constructor.
        var constructors = typeof(SteamLeaderboard).GetConstructors(BindingFlags.Instance | BindingFlags.Public);
        Assert.Empty(constructors);
    }
}
