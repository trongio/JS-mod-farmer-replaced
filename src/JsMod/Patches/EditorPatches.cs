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

    /// <summary>Autocomplete in JS windows offers JS keywords and hides Python-only ones.</summary>
    [HarmonyPatch(typeof(CodeWindow), "GetWordList")]
    internal static class WordListPatch
    {
        private static readonly string[] PythonOnly = { "def", "elif", "pass", "and", "or", "not", "True", "False", "None", "global", "import", "from" };

        private static readonly string[] JsWords = { "let", "const", "function", "of", "true", "false", "null", "undefined", "return", "if", "else", "while", "for", "break", "continue" };

        private static void Postfix(CodeWindow __instance, ref List<string> __result)
        {
            if (!LanguageMode.IsJs(__instance.CodeInput.text, __instance))
            {
                return;
            }
            var words = new HashSet<string>(__result);
            words.ExceptWith(PythonOnly);
            words.UnionWith(JsWords);
            __result = new List<string>(words);
            __result.Sort();
        }
    }
}
