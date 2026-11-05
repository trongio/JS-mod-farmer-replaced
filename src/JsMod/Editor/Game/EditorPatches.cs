using System;
using HarmonyLib;
using JsMod.Lang;
using TMPro;
using UnityEngine;

namespace JsMod.Editor.Game
{
    /// <summary>Runs the editor's shortcuts. A prefix on the input field, so that a handled key never reaches the text as a character.</summary>
    [HarmonyPatch(typeof(CodeInputField), "KeyPressed")]
    internal static class EditorKeysPatch
    {
        private static readonly CharacterSwallower Swallower = new CharacterSwallower();

        private static bool Prefix(CodeInputField __instance, Event evt)
        {
            try
            {
                if (!EditorOptions.Enabled)
                {
                    return true;
                }
                if (evt.keyCode == KeyCode.None)
                {
                    if (Swallower.ShouldSwallow(evt.character, Time.frameCount))
                    {
                        return false;
                    }
                }
                else
                {
                    Swallower.OnKey();
                    EditorActions.DismissWordInfo(__instance);
                }
                bool ctrl = evt.control;
                EditorBindings.Binding binding = EditorBindings.Find(EditorOptions.Current, evt.keyCode, ctrl, evt.alt, evt.shift);
                bool save = EditorBindings.Matches(OptionHolder.GetKeyCombination("save"), evt.keyCode, ctrl, evt.alt, evt.shift);
                if (binding == null && !save)
                {
                    return true;
                }
                CodeWindow window = __instance.GetComponentInParent<CodeWindow>();
                if ((object)window == null || !ReferenceEquals(window.CodeInput, __instance) || !EditorActions.CanEdit(window))
                {
                    return true;
                }
                if (binding == null)
                {
                    // The game saves this key itself, so this only formats first.
                    if (EditorOptions.IsOn(EditorBindings.FormatOnSaveOption))
                    {
                        EditorActions.Format(window, true);
                    }
                    return true;
                }
                Swallower.OnBindingFired(Time.frameCount);
                EditorActions.Perform(binding.Action, window);
                return false;
            }
            catch (Exception ex)
            {
                Plugin.Log.LogWarning("An editor shortcut failed: " + ex.Message);
                return true;
            }
        }
    }

    /// <summary>On the one call that both the run buttons and the run key go through. They've already parsed, so a change means parsing again.</summary>
    [HarmonyPatch(typeof(MainSim), nameof(MainSim.StartMainExecution))]
    internal static class FormatOnRunPatch
    {
        private static void Prefix(MainSim __instance, CodeWindow cw, ref Node syntaxTree)
        {
            if (!EditorOptions.IsOn(EditorBindings.FormatOnRunOption) || __instance.IsExecuting())
            {
                return;
            }
            string before = cw.CodeInput.text;
            EditorActions.Format(cw, true);
            if (cw.CodeInput.text != before)
            {
                syntaxTree = cw.Parse() ?? syntaxTree;
            }
        }
    }

    [HarmonyPatch(typeof(CodeWindow), nameof(CodeWindow.Load))]
    internal static class WindowButtonsPatch
    {
        private static void Postfix(CodeWindow __instance, bool readOnly)
        {
            try
            {
                if (EditorOptions.Enabled && !readOnly && LanguageMode.IsPlayerWindow(__instance))
                {
                    EditorButtons.AddToWindow(__instance);
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogWarning("Could not add the editor buttons: " + ex.Message);
            }
        }
    }

    [HarmonyPatch(typeof(CodeExample), nameof(CodeExample.Setup))]
    internal static class ExampleCopyPatch
    {
        private static void Postfix(CodeWindow ___codeWindow, GameObject ___executeButton)
        {
            try
            {
                if (EditorOptions.Enabled)
                {
                    EditorButtons.AddToExample(___codeWindow, ___executeButton);
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogWarning("Could not add a copy button: " + ex.Message);
            }
        }
    }

    /// <summary>The shortcuts page exists only here, and is written from the keys as they're bound right now.</summary>
    [HarmonyPatch(typeof(Localizer), nameof(Localizer.LoadDoc))]
    internal static class ShortcutsDocPatch
    {
        private static bool Prefix(string doc, ref string __result)
        {
            if (doc != EditorBindings.ShortcutsDoc || !EditorOptions.Enabled)
            {
                return true;
            }
            __result = EditorBindings.ShortcutsPage(
                Localizer.Lang,
                binding => EditorOptions.Current(binding).ToString(display: true),
                option => OptionHolder.GetKeyCombination(option).ToString(display: true),
                Localizer.Localize);
            return false;
        }
    }
}
