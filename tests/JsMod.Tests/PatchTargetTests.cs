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
        AssertTarget(typeof(CodeUtilities), "ApplyCodeTags", "code");
        AssertTarget(typeof(CodeUtilities), "ToNiceString", "obj");
        AssertTarget(typeof(Localizer), "LoadDoc", "doc");
        AssertTarget(typeof(CodeExample), "Setup", "code");
        AssertTarget(typeof(CodeWindow), "IsImportStatement", "text", "stringIndex");
        AssertTarget(typeof(FunctionNode), "GetSignature");
        AssertTarget(typeof(TooltipUtils), "KeywordTooltip", "word");
        AssertTarget(typeof(UnlockBox), "SetupRec");
        AssertTarget(typeof(UnlockBox), "OnThemeChanged");
        AssertTarget(typeof(BuiltinFunctions), "StringConstructor", "exec", "droneId");
        AssertTarget(typeof(TMPTextLocalizer), "LocalizeText");
        AssertTarget(typeof(TMPro.CodeInputField), "KeyPressed", "evt");
        AssertTarget(typeof(CodeWindow), "StartExecution");
        AssertTarget(typeof(CodeWindow), "Load", "readOnly");
        AssertTarget(typeof(CodeWindow), "SetToCodeState");
        AssertTarget(typeof(CodeWindow), "ShowError");
        AssertTarget(typeof(CodeWindow), "SetErrorMessage");
        AssertTarget(typeof(Tooltip), "SetTooltip");
        AssertTarget(typeof(Tooltip), "CloseTooltip");
        AssertTarget(typeof(UndoHistory), "AddChange");
        AssertTarget(typeof(Workspace), "AddNewDocsWindow");
        AssertTarget(typeof(BlinkManager), "GetCodeOffset");
        Assert.NotNull(typeof(CodeWindow).GetField("undoState", Any));
        Assert.NotNull(typeof(CodeWindow).GetField("undoChangeTime", Any));
        Assert.NotNull(typeof(CodeWindow).GetField("breakpointPanel", Any));
        Assert.NotNull(typeof(CodeWindow).GetField("minWidth", Any));
        Assert.NotNull(typeof(BreakPointPanel).GetField("breakpointLines", Any));
        Assert.NotNull(typeof(CodeExample).GetField("executeButton", Any));
        Assert.NotNull(typeof(Tooltip).GetField("info", Any));
        Assert.NotNull(typeof(Tooltip).GetField("tooltipGameObject", Any));
        Assert.NotNull(typeof(Tooltip).GetField("inputModule", Any));
        Assert.NotNull(typeof(Workspace).GetField("tooltip", Any));
        Assert.NotNull(typeof(Workspace).GetField("undoHistory", Any));
        Assert.NotNull(typeof(ProgramState).GetProperty("CurrentExecutingNode", Any));
        Assert.NotNull(typeof(Execution).GetProperty("States", Any));
        Assert.NotNull(typeof(CodeExample).GetField("codeWindow", Any));
        Assert.NotNull(typeof(UnlockSO).GetField("displayCode", Any));
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
