namespace JsMod.Lang
{
    /// <summary>Per-file language markers. A marker counts only as the first non-blank text of the file.</summary>
    public static class JsMarker
    {
        public const string Default = "// @js";

        /// <summary>First non-blank text is `// @js`: this file is JavaScript whatever the setting says.</summary>
        public static bool IsJs(string code)
        {
            return HasMarker(code, "//", "@js");
        }

        /// <summary>
        /// First non-blank text is `# @py`: this file is Python whatever the setting says. It can't misfire: to Python it's a
        /// comment, so nothing changes, and JavaScript has no `#` line, so in JS mode the file couldn't have parsed anyway.
        /// </summary>
        public static bool IsPy(string code)
        {
            return HasMarker(code, "#", "@py");
        }

        private static bool HasMarker(string code, string comment, string tag)
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
            if (string.CompareOrdinal(code, i, comment, 0, comment.Length) != 0)
            {
                return false;
            }
            i += comment.Length;
            while (i < code.Length && (code[i] == ' ' || code[i] == '\t'))
            {
                i++;
            }
            if (string.CompareOrdinal(code, i, tag, 0, tag.Length) != 0)
            {
                return false;
            }
            i += tag.Length;
            return i == code.Length || char.IsWhiteSpace(code[i]);
        }
    }
}
