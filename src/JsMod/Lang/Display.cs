using System;
using System.Collections.Generic;
using System.Linq;

namespace JsMod.Lang
{
    /// <summary>
    /// What the display patches do, without Harmony so it can be tested. Each one leaves everything untouched unless the
    /// setting is JavaScript, so in Python mode the game shows exactly its own text.
    /// </summary>
    public static class Display
    {
        private static readonly Dictionary<string, string> KeywordToPython = new Dictionary<string, string>
        {
            { "function", "def" },
            { "true", "True" },
            { "false", "False" },
            { "null", "None" },
            { "undefined", "None" }
        };

        // Each Python keyword unlocks the JS words that do its job.
        private static readonly Dictionary<string, string[]> PythonToJsWords = new Dictionary<string, string[]>
        {
            { "def", new[] { "function" } },
            { "True", new[] { "true" } },
            { "False", new[] { "false" } },
            { "None", new[] { "null", "undefined" } },
            { "for", new[] { "for", "of" } }
        };

        private static readonly string[] PythonOnly = { "def", "elif", "pass", "and", "or", "not", "True", "False", "None", "global" };

        /// <summary>JS words that aren't tied to a keyword: declaring a variable is possible from the start of the game.</summary>
        private static readonly string[] AlwaysJsWords = { "let", "const" };

        public static string CodeTags(string text)
        {
            return LanguageMode.SettingIsJs ? JsContent.MapCodeTags(text) : text;
        }

        public static string UnlockBoxCode(string code)
        {
            return LanguageMode.SettingIsJs ? JsContent.MapCode(code) : code;
        }

        public static string Doc(string language, string docName, string doc)
        {
            if (!LanguageMode.SettingIsJs)
            {
                return doc;
            }
            return JsContent.PageOverride(language, docName) ?? JsContent.MapExamples(doc, docName);
        }

        /// <summary>True when the value has a JS spelling, which is then in text.</summary>
        public static bool NiceString(object value, out string text)
        {
            text = null;
            if (!LanguageMode.SettingIsJs)
            {
                return false;
            }
            if (value == null || value is PyNone)
            {
                text = "null";
            }
            else if (value is PyBool)
            {
                text = value.ToString() == "True" ? "true" : "false";
            }
            return text != null;
        }

        /// <summary>Hovering a JS keyword shows the tooltip of the Python keyword it replaces.</summary>
        public static string TooltipKeyword(string word)
        {
            return LanguageMode.SettingIsJs && word != null && KeywordToPython.TryGetValue(word, out string python) ? python : word;
        }

        /// <summary>The autocomplete list for a JS window: Python keywords become the JS words they've unlocked.</summary>
        public static List<string> JsWordList(IEnumerable<string> words)
        {
            var result = new HashSet<string>(words);
            foreach (KeyValuePair<string, string[]> pair in PythonToJsWords)
            {
                if (result.Contains(pair.Key))
                {
                    result.UnionWith(pair.Value);
                }
            }
            result.ExceptWith(PythonOnly);
            result.UnionWith(AlwaysJsWords);
            List<string> sorted = result.ToList();
            sorted.Sort();
            return sorted;
        }

        /// <summary>A function's hover text includes the comment lines above it, as the game does for # comments.</summary>
        public static string SignatureWithComments(string source, string signature, int functionStart)
        {
            if (functionStart <= 0 || functionStart > source.Length)
            {
                return signature;
            }
            string[] before = source.Substring(0, functionStart).Split('\n');
            var comments = new List<string>();
            for (int i = before.Length - 2; i >= 0 && before[i].TrimStart().StartsWith("//", StringComparison.Ordinal); i--)
            {
                comments.Insert(0, before[i]);
            }
            if (comments.Count == 0)
            {
                return signature;
            }
            return string.Join(Environment.NewLine, comments) + Environment.NewLine + signature;
        }

        /// <summary>True when the word being typed names a file: after import, from or as, with or without its opening quote.</summary>
        public static bool IsJsImportPosition(string text, int wordStart)
        {
            int i = wordStart - 1;
            if (i >= 0 && (text[i] == '"' || text[i] == '\''))
            {
                i--;
            }
            while (i >= 0 && (text[i] == ' ' || text[i] == '\t'))
            {
                i--;
            }
            int end = i;
            while (i >= 0 && (char.IsLetterOrDigit(text[i]) || text[i] == '_'))
            {
                i--;
            }
            string word = text.Substring(i + 1, end - i);
            return word == "import" || word == "from" || word == "as";
        }

        /// <summary>
        /// A '}' typed first on a line moves back one indent level. Returns false when there's nothing to do. Otherwise text and
        /// caret are the edited values. The caret is straight after the brace.
        /// </summary>
        public static bool OutdentClosingBrace(ref string text, ref int caret)
        {
            int brace = caret - 1;
            if (brace < 1 || brace >= text.Length || text[brace] != '}')
            {
                return false;
            }
            int lineStart = brace;
            while (lineStart > 0 && text[lineStart - 1] != '\n')
            {
                lineStart--;
            }
            int indent = brace - lineStart;
            if (indent == 0 || text.Substring(lineStart, indent).Trim().Length != 0)
            {
                return false;
            }
            int remove = text[brace - 1] == '\t' ? 1 : Math.Min(4, indent);
            text = text.Remove(brace - remove, remove);
            caret -= remove;
            return true;
        }
    }
}
