using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
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
            Log.LogInfo("JsMod loaded. Start a code window with // @js to write JavaScript. Leaderboard submission is off.");
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
