using System;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using JsMod.Lang;
using JsMod.Patches;

namespace JsMod
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;
            var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            try
            {
                // The leaderboard block goes first: without it, the language patches must not load at all.
                harmony.CreateClassProcessor(typeof(FillLeaderboardPatch)).Patch();
                harmony.CreateClassProcessor(typeof(SteamLeaderboardPatch)).Patch();
            }
            catch (Exception ex)
            {
                harmony.UnpatchSelf();
                Log.LogError("Could not block leaderboard submission, so JsMod stays disabled: " + ex);
                return;
            }
            try
            {
                // Parse first: it ignores anything without the JS sentinel, which only the Tokenize patch can produce.
                harmony.CreateClassProcessor(typeof(ParsePatch)).Patch();
                harmony.CreateClassProcessor(typeof(TokenizePatch)).Patch();
            }
            catch (Exception ex)
            {
                // Both or neither. The leaderboard block stays on, because the session is still modded.
                harmony.Unpatch(AccessTools.Method(typeof(Tokenizer), nameof(Tokenizer.Tokenize)), HarmonyPatchType.All, harmony.Id);
                harmony.Unpatch(AccessTools.Method(typeof(Parser), nameof(Parser.Parse)), HarmonyPatchType.All, harmony.Id);
                Log.LogError("Could not patch the game's language pipeline, so JavaScript will not run: " + ex);
                return;
            }
            PatchOptional(harmony, typeof(LeaderboardAchievementPatch));
            PatchOptional(harmony, typeof(HighlightPatch));
            PatchOptional(harmony, typeof(AutoIndentPatch));
            PatchOptional(harmony, typeof(WordListPatch));
            PatchLanguageOption(harmony);
            JsContent.Warn = message => Log.LogWarning(message);
            JsContent.Load(Path.Combine(Path.GetDirectoryName(Info.Location), "js"));
            // Each of these is a no-op unless the setting is JavaScript, and on failure that text just stays Python.
            PatchOptional(harmony, typeof(CodeTagsPatch));
            PatchOptional(harmony, typeof(LoadDocPatch));
            PatchOptional(harmony, typeof(CodeExamplePatch));
            PatchOptional(harmony, typeof(NiceStringPatch));
            PatchOptional(harmony, typeof(UnlockBoxPatch));
            PatchOptional(harmony, typeof(ImportCompletionPatch));
            PatchOptional(harmony, typeof(SignaturePatch));
            PatchOptional(harmony, typeof(KeywordTooltipPatch));
            Log.LogInfo("JsMod loaded. Choose the programming language in Options > General, or start a file with // @js or # @py. Leaderboard submission is off.");
        }

        /// <summary>All or nothing. The setting stays off until every part is in, and without it the markers still work.</summary>
        private static void PatchLanguageOption(Harmony harmony)
        {
            try
            {
                harmony.CreateClassProcessor(typeof(TokenScopePatch)).Patch();
                harmony.CreateClassProcessor(typeof(ColourScopePatch)).Patch();
                harmony.CreateClassProcessor(typeof(LocalizePatch)).Patch();
                harmony.CreateClassProcessor(typeof(OptionMenuPatch)).Patch();
                LanguageOption.Enable();
            }
            catch (Exception ex)
            {
                MethodBase[] targets =
                {
                    AccessTools.Method(typeof(CodeWindow), nameof(CodeWindow.UpdateTokens)),
                    AccessTools.Method(typeof(CodeWindow), nameof(CodeWindow.UpdateSearch)),
                    AccessTools.Method(typeof(CodeWindow), "OnThemeChanged"),
                    AccessTools.Method(typeof(Localizer), nameof(Localizer.Localize)),
                    AccessTools.Method(typeof(OptionMenu), nameof(OptionMenu.Setup))
                };
                foreach (MethodBase target in targets)
                {
                    // Null when the failure was the game renaming this very method.
                    if (target != null)
                    {
                        harmony.Unpatch(target, HarmonyPatchType.All, harmony.Id);
                    }
                }
                Log.LogWarning("Could not add the programming language option, so plain code stays Python. The // @js marker still works: " + ex);
            }
        }

        private static void PatchOptional(Harmony harmony, Type patch)
        {
            try
            {
                harmony.CreateClassProcessor(patch).Patch();
            }
            catch (Exception ex)
            {
                Log.LogWarning("Optional patch " + patch.Name + " failed: " + ex.Message);
            }
        }
    }
}
