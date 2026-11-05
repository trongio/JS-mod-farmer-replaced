namespace JsMod.Lang
{
    /// <summary>
    /// Regex used to colour JS. The game's highlighter handles the search marks, IME underline, and tag wrapping; a transpiler
    /// swaps only its pattern, so the group names below must match the ones the game's highlighter reads.
    /// </summary>
    public static class JsHighlighter
    {
        public const string Pattern =
            @"(?<comment>//.*|/\*[\s\S]*?(?:\*/|$))" +
            @"|(?<string>(['""])(.*?)\1|`[^`]*`)" +
            @"|(?<keyword>\b(?:let|const|var|function|if|else|while|for|of|in|return|break|continue|true|false|null|undefined)\b)" +
            @"|(?<function>\b[a-zA-Z_$][\w$]*(?=\())" +
            @"|(?<builtin>\b(?:North|East|South|West|Entities|Items|Grounds|Unlocks|Leaderboards|Hats)\b|(?<=[a-zA-Z_$]\.)\w*)" +
            @"|(?<number>\b0[xX][0-9a-fA-F]+\b|\b(?:\d*\.)?\d+(?:[eE][+-]?\d+)?\b)";

        /// <summary>Called from the patched highlighter in place of its hardcoded pattern.</summary>
        public static string Select(string original, string code)
        {
            return LanguageMode.IsJsWhileColouring(code) ? Pattern : original;
        }
    }
}
