using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using JsMod.Lang;

namespace JsMod.Patches
{
    [HarmonyPatch(typeof(Tokenizer), nameof(Tokenizer.Tokenize))]
    internal static class TokenizePatch
    {
        private static bool Prefix(string code, ref TokenStream stream, ref bool __result)
        {
            if (!LanguageMode.IsJsWhileTokenizing(code))
            {
                return true;
            }
            stream = JsTokenizer.Tokenize(code);
            __result = false;
            return false;
        }
    }

    [HarmonyPatch(typeof(Parser), nameof(Parser.Parse))]
    internal static class ParsePatch
    {
        private static bool Prefix(TokenStream stream, CodeWindow f, ref Program __result)
        {
            if (stream.Current == null || stream.Current.type != JsTokenizer.Sentinel)
            {
                return true;
            }
            __result = JsParser.Parse(stream, f);
            return false;
        }
    }

    /// <summary>Keeps the game's highlighter and swaps only its regex, so that search marks and IME underlines keep working.</summary>
    [HarmonyPatch(typeof(CodeUtilities), nameof(CodeUtilities.SyntaxColor2))]
    internal static class HighlightPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool found = false;
            foreach (CodeInstruction instruction in instructions)
            {
                yield return instruction;
                if (!found && instruction.opcode == OpCodes.Ldstr && instruction.operand is string s && s.StartsWith("(?<comment>"))
                {
                    found = true;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(JsHighlighter), nameof(JsHighlighter.Select)));
                }
            }
            if (!found)
            {
                Plugin.Log.LogWarning("Highlighter pattern not found, so JavaScript will be coloured as Python.");
            }
        }
    }
}
