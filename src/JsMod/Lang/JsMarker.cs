namespace JsMod.Lang
{
    /// <summary>Decides whether a code window holds JavaScript. A file is JS when its first non-blank text is a `// @js` comment.</summary>
    public static class JsMarker
    {
        public const string Default = "// @js";

        public static bool IsJs(string code)
        {
            if (code == null)
            {
                return false;
            }
            int i = 0;
            while (i < code.Length && (char.IsWhiteSpace(code[i]) || code[i] == '﻿'))
            {
                i++;
            }
            if (i + 2 > code.Length || code[i] != '/' || code[i + 1] != '/')
            {
                return false;
            }
            i += 2;
            while (i < code.Length && (code[i] == ' ' || code[i] == '\t'))
            {
                i++;
            }
            if (i + 3 > code.Length || string.CompareOrdinal(code, i, "@js", 0, 3) != 0)
            {
                return false;
            }
            i += 3;
            return i == code.Length || char.IsWhiteSpace(code[i]);
        }
    }
}
