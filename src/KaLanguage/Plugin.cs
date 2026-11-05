using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

namespace KaLanguage
{
    /// Adds Georgian as a game language: installs the pack, adds a Georgian font,
    /// and shows the language's own name. Blocks leaderboard uploads, because the session
    /// is modded, and stays off entirely if it can't.
    [BepInPlugin("trongio.tfwr.kalanguage", "Georgian Language", "0.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        private const string LanguageCode = "KA";
        private const string LanguageName = "ქართული";
        private const string FontFile = "NotoSansGeorgian-Regular.ttf";

        private static ManualLogSource log;
        private static TMP_FontAsset font;

        private void Awake()
        {
            log = Logger;
            var harmony = new Harmony("trongio.tfwr.kalanguage");
            try
            {
                BlockLeaderboardUploads(harmony);
                log.LogInfo("leaderboard block: ok");
            }
            catch (Exception ex)
            {
                harmony.UnpatchSelf();
                log.LogError("leaderboard block: FAILED, so the Georgian plugin stays off: " + ex);
                return;
            }
            string pluginDir = Path.GetDirectoryName(Info.Location);
            Step("language pack", () => SyncPack(pluginDir));
            Step("language dropdown", AddToLoadedDropdown);
            Step("language name patch", () => harmony.Patch(
                AccessTools.Method(typeof(Localizer), nameof(Localizer.Localize)),
                postfix: new HarmonyMethod(typeof(Plugin), nameof(LocalizePostfix))));
            Step("Georgian font", () => AddFont(pluginDir));
        }

        /// Score 0 is the game's own path for showing a leaderboard without uploading. Both call
        /// sites are covered. This is safe beside JsMod, which forces the same value.
        private static void BlockLeaderboardUploads(Harmony harmony)
        {
            var prefix = new HarmonyMethod(typeof(Plugin), nameof(ForceScoreZero));
            harmony.Patch(AccessTools.Method(typeof(Leaderboard), nameof(Leaderboard.FillLeaderboard)), prefix: prefix);
            harmony.Patch(AccessTools.Method(typeof(SteamLeaderboard), nameof(SteamLeaderboard.LoadLeaderboard)), prefix: prefix);
        }

        private static void ForceScoreZero(ref int score)
        {
            score = 0;
        }

        private static void Step(string name, Action action)
        {
            try
            {
                action();
                log.LogInfo($"{name}: ok");
            }
            catch (Exception ex)
            {
                log.LogError($"{name}: FAILED: {ex}");
            }
        }

        /// Copies the pack into Languages/KA. Writes only inside that folder.
        /// Localizer.LoadDoc returns "" for a missing doc and has no English fallback,
        /// so docs not translated yet are copied from the player's own EN folder.
        private static void SyncPack(string pluginDir)
        {
            string languages = Path.Combine(Application.streamingAssetsPath, "Languages");
            string target = Path.Combine(languages, LanguageCode);
            string pack = Path.Combine(pluginDir, LanguageCode);
            if (!Directory.Exists(pack))
            {
                throw new DirectoryNotFoundException(pack);
            }
            int written = CopyTree(pack, target, skipExisting: null);
            string enDocs = Path.Combine(languages, "EN", "docs");
            int fallback = Directory.Exists(enDocs)
                ? CopyTree(enDocs, Path.Combine(target, "docs"), Path.Combine(pack, "docs"))
                : 0;
            log.LogInfo($"pack files written: {written}, English docs as fallback: {fallback}, target: {target}");
        }

        private static int CopyTree(string source, string target, string skipExisting)
        {
            int written = 0;
            foreach (string file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
            {
                string relative = file.Substring(source.Length).TrimStart('/', '\\');
                if (skipExisting != null && File.Exists(Path.Combine(skipExisting, relative)))
                {
                    continue;
                }
                string destination = Path.Combine(target, relative);
                if (File.Exists(destination) && File.ReadAllBytes(destination).SequenceEqual(File.ReadAllBytes(file)))
                {
                    continue;
                }
                Directory.CreateDirectory(Path.GetDirectoryName(destination));
                File.Copy(file, destination, overwrite: true);
                written++;
            }
            return written;
        }

        /// OptionHolder builds the dropdown lazily from the Languages folder, so KA is
        /// normally found on its own. This covers the game having built it before Awake.
        private static void AddToLoadedDropdown()
        {
            var assets = (OptionSO[])AccessTools.Field(typeof(OptionHolder), "optionAssets").GetValue(null);
            var language = assets?.OfType<DropdownOptionSO>().FirstOrDefault(o => o.optionName == "language");
            if (language == null || language.options.Contains(LanguageCode))
            {
                return;
            }
            if (!language.options.Contains(OptionHolder.UnofficialTranslationsSeparator))
            {
                language.options.Add("");
                language.options.Add(OptionHolder.UnofficialTranslationsSeparator);
            }
            language.options.Add(LanguageCode);
            log.LogWarning("Options loaded before this plugin, so KA was added to the dropdown afterwards.");
        }

        private static void LocalizePostfix(string key, ref string __result)
        {
            if (key == LanguageCode && __result == LanguageCode)
            {
                __result = LanguageName;
            }
        }

        /// None of the game's fonts have Georgian letters. Appending keeps Localizer's
        /// fallback slots intact: it swaps index 0 and reads captured indexes 0 to 3.
        private static void AddFont(string pluginDir)
        {
            string path = Path.Combine(pluginDir, "fonts", FontFile);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }
            EnsureSdfShader();
            font = TMP_FontAsset.CreateFontAsset(path, 0, 90, 9, GlyphRenderMode.SDFAA, 1024, 1024);
            if (font == null)
            {
                throw new InvalidOperationException("TextMeshPro could not load " + path);
            }
            font.name = "NotoSansGeorgian-Regular SDF";
            font.hideFlags = HideFlags.DontUnloadUnusedAsset;

            if (!font.HasCharacter('ა', tryAddCharacter: true))
            {
                throw new InvalidOperationException("The font loaded but has no glyph for U+10D0.");
            }
            var fallbacks = TMP_Settings.fallbackFontAssets;
            if (fallbacks == null)
            {
                throw new InvalidOperationException("TMP_Settings.fallbackFontAssets is null.");
            }
            fallbacks.Add(font);
            log.LogInfo($"global fallback fonts: {fallbacks.Count}, Georgian font at index {fallbacks.Count - 1}");
        }

        /// CreateFontAsset uses Shader.Find, which fails if the build stripped this shader.
        /// In that case, borrow the shader of a font the game already uses.
        private static void EnsureSdfShader()
        {
            if (Shader.Find("TextMeshPro/Mobile/Distance Field") != null)
            {
                return;
            }
            Shader shader = TMP_Settings.defaultFontAsset?.material?.shader;
            if (shader == null)
            {
                throw new InvalidOperationException("No SDF shader available.");
            }
            AccessTools.Field(typeof(ShaderUtilities), "k_ShaderRef_MobileSDF").SetValue(null, shader);
            log.LogWarning("The Mobile SDF shader is missing, so using " + shader.name);
        }
    }
}
