using System.Collections.Generic;
using HarmonyLib;
using JsMod.Lang;

namespace JsMod.Patches
{
    /// <summary>In JS windows, a new line after '{' gets one more indent level. Python's rule looks for ':' instead.</summary>
    [HarmonyPatch(typeof(CodeWindow), "InsertNewLineIndentation")]
    internal static class AutoIndentPatch
    {
        private static bool Prefix(CodeWindow __instance, string inserted)
        {
            var input = __instance.CodeInput;
            if (!LanguageMode.IsJs(input.text, __instance))
            {
                return true;
            }
            if (inserted == "}")
            {
                string edited = input.text;
                int caret = input.stringPosition;
                if (Display.OutdentClosingBrace(ref edited, ref caret))
                {
                    input.text = edited;
                    input.stringPosition = caret;
                }
                return false;
            }
            if (!inserted.EndsWith("\n", System.StringComparison.Ordinal))
            {
                return false;
            }
            int newLine = input.stringPosition - 1;
            if (newLine <= 0 || newLine >= input.text.Length || input.text[newLine] != '\n')
            {
                return false;
            }
            Token previous = __instance.tokens.GetLastNewLine(newLine);
            string indent = previous != null ? previous.value.Substring(1) : "";
            if (newLine - 1 > 0 && input.text[newLine - 1] == '{')
            {
                indent += indent.Length > 0 && indent[0] == '\t' ? "\t" : "    ";
            }
            input.text = input.text.Insert(newLine + 1, indent);
            input.stringPosition += indent.Length;
            return false;
        }
    }

    /// <summary>Autocomplete in JS windows offers the JS words for the keywords unlocked so far, and hides Python-only ones.</summary>
    [HarmonyPatch(typeof(CodeWindow), "GetWordList")]
    internal static class WordListPatch
    {
        private static void Postfix(CodeWindow __instance, ref List<string> __result)
        {
            if (LanguageMode.IsJs(__instance.CodeInput.text, __instance))
            {
                __result = Display.JsWordList(__result);
            }
        }
    }

    /// <summary>Autocomplete offers file names in a JS import, where the Python rule looks for different words.</summary>
    [HarmonyPatch(typeof(CodeWindow), "IsImportStatement")]
    internal static class ImportCompletionPatch
    {
        private static void Postfix(CodeWindow __instance, string text, int stringIndex, ref bool __result)
        {
            if (LanguageMode.IsJs(text, __instance))
            {
                __result = Display.IsJsImportPosition(text, stringIndex);
            }
        }
    }

    /// <summary>A JS function's hover text includes the // comments above it.</summary>
    [HarmonyPatch(typeof(FunctionNode), nameof(FunctionNode.GetSignature))]
    internal static class SignaturePatch
    {
        private static void Postfix(FunctionNode __instance, ref string __result)
        {
            CodeWindow window = __instance.boxedParams.codeWindow;
            string source = window.CodeInput.text;
            if (LanguageMode.IsJs(source, window))
            {
                __result = Display.SignatureWithComments(source, __result, __instance.boxedParams.wordStart);
            }
        }
    }

    /// <summary>Hovering a JS keyword shows the tooltip of the Python keyword it replaces.</summary>
    [HarmonyPatch(typeof(TooltipUtils), nameof(TooltipUtils.KeywordTooltip))]
    internal static class KeywordTooltipPatch
    {
        private static void Prefix(ref string word)
        {
            word = Display.TooltipKeyword(word);
        }
    }
}
