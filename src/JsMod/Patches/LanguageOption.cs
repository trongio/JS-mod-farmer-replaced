using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JsMod.Lang;
using UnityEngine;

namespace JsMod.Patches
{
    /// <summary>
    /// The "programming language" option. Its row is a runtime copy of the language row, added only to the list the options menu
    /// reads. The value is saved by OptionHolder, which keeps option names it doesn't know as strings in options.txt.
    /// </summary>
    public static class LanguageOption
    {
        private const string TemplateName = "language";

        private static DropdownOptionSO option;
        private static bool rowFailed;
        private static bool seeding;
        private static FieldInfo dirty;
        private static FieldInfo parseLock;
        private static MethodInfo closeError;

        /// <summary>Throws if the game no longer has what re-parsing needs. Doesn't touch OptionHolder's values, which aren't safe to load this early.</summary>
        internal static void Enable()
        {
            dirty = Required(AccessTools.Field(typeof(CodeWindow), "dirty"), "CodeWindow.dirty");
            parseLock = Required(AccessTools.Field(typeof(CodeWindow), "parseLock"), "CodeWindow.parseLock");
            closeError = Required(AccessTools.Method(typeof(CodeWindow), "CloseError"), "CodeWindow.CloseError");
            OptionHolder.OnOptionChanged += OnOptionChanged;
            LanguageMode.ReadSetting = () => OptionHolder.GetString(LanguageMode.OptionName, LanguageMode.Python);
        }

        private static T Required<T>(T member, string name) where T : class
        {
            return member ?? throw new MissingMemberException(name);
        }

        /// <summary>Replaces the options menu's call to ResourceManager.GetAllOptions. Public, because patched game code calls it.</summary>
        public static IEnumerable<OptionSO> AllOptions()
        {
            IEnumerable<OptionSO> options = ResourceManager.GetAllOptions();
            if (rowFailed)
            {
                return options;
            }
            try
            {
                return WithOurOption(options.ToList());
            }
            catch (Exception ex)
            {
                // Without a row the player couldn't switch back, so a saved JavaScript must not stay in force.
                // Stays off for the session, so a later menu can't show a choice that no longer applies.
                rowFailed = true;
                LanguageMode.ReadSetting = null;
                Reparse();
                Plugin.Log.LogWarning("Could not add the programming language row, so plain code stays Python. The // @js marker still works: " + ex);
                return options;
            }
        }

        private static List<OptionSO> WithOurOption(List<OptionSO> options)
        {
            if (options.Any(o => o.optionName == LanguageMode.OptionName))
            {
                return options;
            }
            DropdownOptionSO template = options.OfType<DropdownOptionSO>().FirstOrDefault(o => o.optionName == TemplateName);
            if (template == null || !(template.optionUI is DropdownOptionUI))
            {
                throw new InvalidOperationException("The options list has no dropdown named '" + TemplateName + "' to copy.");
            }
            if (option == null)
            {
                option = ScriptableObject.CreateInstance<DropdownOptionSO>();
                option.hideFlags = HideFlags.HideAndDontSave;
                option.name = LanguageMode.OptionName;
                option.optionName = LanguageMode.OptionName;
                option.tooltip = ModStrings.TooltipKey;
                option.defaultValue = LanguageMode.Python;
                option.options = new List<string> { LanguageMode.Python, LanguageMode.JavaScript };
                option.optionUI = template.optionUI;
                option.category = template.category;
                option.importance = template.importance;
                option.dlc = template.dlc;
            }
            // The dropdown shows a blank box for a value it doesn't list, and OptionHolder doesn't know this option's default.
            if (!option.options.Contains(OptionHolder.GetString(LanguageMode.OptionName)))
            {
                // A missing or invalid value already reads as Python, so nothing needs re-parsing. Re-parsing here would mark the save as changed.
                seeding = true;
                try
                {
                    OptionHolder.SetOption(LanguageMode.OptionName, LanguageMode.Python);
                }
                finally
                {
                    seeding = false;
                }
            }
            options.Add(option);
            return options;
        }

        private static void OnOptionChanged(string item)
        {
            if (item == LanguageMode.OptionName && !seeding)
            {
                Reparse();
            }
        }

        private static void Reparse()
        {
            MainSim sim = MainSim.Inst;
            if (sim == null || sim.workspace == null)
            {
                return;
            }
            foreach (CodeWindow window in sim.workspace.codeWindows.Values)
            {
                try
                {
                    Reparse(window);
                }
                catch (Exception ex)
                {
                    Plugin.Log.LogWarning("Could not re-parse " + window.fileName + " after the language changed: " + ex);
                }
            }
        }

        private static void Reparse(CodeWindow window)
        {
            lock (parseLock.GetValue(window))
            {
                // Not by faking a text change: UpdateTokens would then stop a running program and add an undo step.
                CodeWindow outer = LanguageMode.EnterTokenizing(window);
                try
                {
                    Tokenizer.Tokenize(window.CodeInput.text, out window.tokens);
                }
                finally
                {
                    LanguageMode.ExitTokenizing(outer);
                }
                window.tokenizedIdentifiers.Clear();
                foreach (Token token in window.tokens)
                {
                    if (token.type == TokenType.IDENTIFIER)
                    {
                        window.tokenizedIdentifiers.Add(token.value);
                    }
                }
                dirty.SetValue(window, true);
                // Both calls below recolour, because UpdateTokens always refreshes the highlight.
                if (window.isExecuting)
                {
                    // The running program keeps its tree. The next Parse, such as pressing run again, picks up the new language.
                    window.UpdateTokens();
                    return;
                }
                closeError.Invoke(window, null);
                window.Parse();
            }
        }
    }

    /// <summary>Tells the Tokenize patch which window it's working for.</summary>
    [HarmonyPatch(typeof(CodeWindow), nameof(CodeWindow.UpdateTokens))]
    internal static class TokenScopePatch
    {
        private static void Prefix(CodeWindow __instance, out CodeWindow __state)
        {
            __state = LanguageMode.EnterTokenizing(__instance);
        }

        private static void Finalizer(CodeWindow __state)
        {
            LanguageMode.ExitTokenizing(__state);
        }
    }

    /// <summary>Tells the highlighter which window it's working for. These are the only two places a window colours its own code.</summary>
    [HarmonyPatch]
    internal static class ColourScopePatch
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(CodeWindow), nameof(CodeWindow.UpdateSearch));
            yield return AccessTools.Method(typeof(CodeWindow), "OnThemeChanged");
        }

        private static void Prefix(CodeWindow __instance, out CodeWindow __state)
        {
            __state = LanguageMode.EnterColouring(__instance);
        }

        private static void Finalizer(CodeWindow __state)
        {
            LanguageMode.ExitColouring(__state);
        }
    }

    [HarmonyPatch(typeof(Localizer), nameof(Localizer.Localize))]
    internal static class LocalizePatch
    {
        private static void Postfix(string key, ref string __result)
        {
            __result = ModStrings.Lookup(key, Localizer.Lang) ?? __result;
        }
    }

    /// <summary>
    /// A transpiler on the caller, because GetAllOptions is small enough for the runtime to inline, which would skip a patch on it.
    /// Throwing here turns the whole option off, leaving the menu as it was.
    /// </summary>
    [HarmonyPatch(typeof(OptionMenu), nameof(OptionMenu.Setup))]
    internal static class OptionMenuPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo original = AccessTools.Method(typeof(ResourceManager), nameof(ResourceManager.GetAllOptions));
            bool found = false;
            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.Calls(original))
                {
                    instruction.operand = AccessTools.Method(typeof(LanguageOption), nameof(LanguageOption.AllOptions));
                    found = true;
                }
                yield return instruction;
            }
            if (!found)
            {
                throw new InvalidOperationException("OptionMenu.Setup no longer calls ResourceManager.GetAllOptions.");
            }
        }
    }
}
