using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using JsMod.Lang;

namespace JsMod.Patches
{
    /// <summary>Code in the game's text passes through here on its way to the screen, in tooltips, errors and docs alike.</summary>
    [HarmonyPatch(typeof(CodeUtilities), nameof(CodeUtilities.ApplyCodeTags))]
    internal static class CodeTagsPatch
    {
        private static void Prefix(ref string code)
        {
            code = Display.CodeTags(code);
        }
    }

    [HarmonyPatch(typeof(Localizer), nameof(Localizer.LoadDoc))]
    internal static class LoadDocPatch
    {
        private static void Postfix(string doc, ref string __result)
        {
            __result = Display.Doc(Localizer.Lang, doc, __result);
        }
    }

    /// <summary>A docs example showing JavaScript has to parse as JavaScript. One that stayed Python, because nothing matched it, stays Python.</summary>
    [HarmonyPatch(typeof(CodeExample), nameof(CodeExample.Setup))]
    internal static class CodeExamplePatch
    {
        private static void Prefix(string code, CodeWindow ___codeWindow)
        {
            if (LanguageMode.SettingIsJs && JsContent.IsJsExample(code))
            {
                LanguageMode.MarkJsExample(___codeWindow);
            }
        }
    }

    /// <summary>print, str, hover values and errors all format values here, including ones inside lists.</summary>
    [HarmonyPatch(typeof(CodeUtilities), nameof(CodeUtilities.ToNiceString))]
    internal static class NiceStringPatch
    {
        private static bool Prefix(object obj, ref string __result)
        {
            if (Display.NiceString(obj, out string text))
            {
                __result = text;
                return false;
            }
            return true;
        }
    }

    /// <summary>The code shown on unlock boxes in the tech tree comes from the game's assets, not its text files.</summary>
    [HarmonyPatch(typeof(UnlockBox), nameof(UnlockBox.SetupRec))]
    internal static class UnlockBoxPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            FieldInfo field = AccessTools.Field(typeof(UnlockSO), nameof(UnlockSO.displayCode));
            bool found = false;
            foreach (CodeInstruction instruction in instructions)
            {
                yield return instruction;
                if (instruction.LoadsField(field))
                {
                    found = true;
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Display), nameof(Display.UnlockBoxCode)));
                }
            }
            if (!found)
            {
                throw new System.InvalidOperationException("UnlockBox.SetupRec no longer reads UnlockSO.displayCode.");
            }
        }
    }
}
