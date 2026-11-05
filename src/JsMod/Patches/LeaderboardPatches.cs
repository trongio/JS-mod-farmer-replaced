using HarmonyLib;

namespace JsMod.Patches
{
    /// <summary>
    /// While the mod is loaded, no leaderboard score is submitted, whatever language the run used. Score 0 is the game's own
    /// path for showing a leaderboard without uploading a result, so the screen still opens and shows existing ranks.
    /// </summary>
    [HarmonyPatch(typeof(Leaderboard), nameof(Leaderboard.FillLeaderboard))]
    internal static class FillLeaderboardPatch
    {
        private static void Prefix(ref int score)
        {
            if (score != 0)
            {
                Plugin.Log.LogInfo("Leaderboard score not submitted: JsMod is active.");
                score = 0;
            }
        }
    }

    [HarmonyPatch(typeof(SteamLeaderboard), nameof(SteamLeaderboard.LoadLeaderboard))]
    internal static class SteamLeaderboardPatch
    {
        private static void Prefix(ref int score)
        {
            score = 0;
        }
    }

    /// <summary>These two unlock only from finishing a leaderboard run, which a modded session shouldn't earn.</summary>
    [HarmonyPatch(typeof(Achievements), nameof(Achievements.UnlockAchievement))]
    internal static class LeaderboardAchievementPatch
    {
        private static bool Prefix(string achievement)
        {
            return achievement != "COMPETITIVE_FARMING" && achievement != "FULL_AUTOMATION";
        }
    }
}
