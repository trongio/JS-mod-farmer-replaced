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
            // The game asks for pages to find out whether they exist, so a page only we have must still look missing.
            if (!LanguageMode.SettingIsJs || string.IsNullOrEmpty(doc))
            {
                return doc;
            }
            return JsContent.PageOverride(language, docName) ?? JsContent.MapExamples(doc, docName);
        }

        // Inside str(), the calling file's language decides. Everywhere else, the setting does.
        private const int NoStrCall = 0;
        private const int StrInPython = 1;
        private const int StrInJs = 2;

        // Per thread, because each simulation runs str() on its own thread.
        [ThreadStatic]
        private static int strCall;

        /// <summary>True when str() called from this file spells values the JS way. Python mode is always stock, whatever the file says.</summary>
        public static bool StrIsJs(string code, CodeWindow window)
        {
            return (object)window != null && code != null && LanguageMode.SettingIsJs && LanguageMode.IsJs(code, window);
        }

        public static int EnterStr(bool isJs)
        {
            int outer = strCall;
            strCall = isJs ? StrInJs : StrInPython;
            return outer;
        }

        public static void ExitStr(int outer)
        {
            strCall = outer;
        }

        /// <summary>True when the value has a JS spelling, which is then in text.</summary>
        public static bool NiceString(object value, out string text)
        {
            text = null;
            bool js = strCall == NoStrCall ? LanguageMode.SettingIsJs : strCall == StrInJs;
            if (!js)
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
        /// A '}' typed first on a line lines up with the line that opened its block, and is left alone if it's already there or
        /// further left. With no opener in the text it moves back one indent, as the file measures one. Returns false when
        /// there's nothing to do. Otherwise text and caret are the edited values. The caret is straight after the brace.
        /// </summary>
        public static bool OutdentClosingBrace(ref string text, ref int caret)
        {
            int brace = caret - 1;
            if (brace < 1 || brace >= text.Length || text[brace] != '}')
            {
                return false;
            }
            int lineStart = LineStart(text, brace);
            int indent = brace - lineStart;
            if (indent == 0 || text.Substring(lineStart, indent).Trim().Length != 0)
            {
                return false;
            }
            int opener = OpeningBrace(text, brace);
            string target;
            if (opener >= 0)
            {
                target = LeadingWhitespace(text, LineStart(text, opener));
            }
            else
            {
                target = text.Substring(lineStart, indent - Math.Min(IndentUnit(text, lineStart), indent));
            }
            if (target.Length >= indent)
            {
                return false;
            }
            text = text.Substring(0, lineStart) + target + text.Substring(brace);
            caret -= indent - target.Length;
            return true;
        }

        private static int LineStart(string text, int position)
        {
            while (position > 0 && text[position - 1] != '\n')
            {
                position--;
            }
            return position;
        }

        private static string LeadingWhitespace(string text, int lineStart)
        {
            int end = lineStart;
            while (end < text.Length && (text[end] == ' ' || text[end] == '\t'))
            {
                end++;
            }
            return text.Substring(lineStart, end - lineStart);
        }

        /// <summary>The '{' this '}' closes, skipping strings and comments, or -1.</summary>
        private static int OpeningBrace(string text, int brace)
        {
            var open = new Stack<int>();
            for (int i = 0; i < brace; i++)
            {
                char c = text[i];
                if (c == '/' && i + 1 < brace && text[i + 1] == '/')
                {
                    while (i < brace && text[i] != '\n')
                    {
                        i++;
                    }
                }
                else if (c == '/' && i + 1 < brace && text[i + 1] == '*')
                {
                    int end = text.IndexOf("*/", i + 2, StringComparison.Ordinal);
                    if (end < 0 || end >= brace)
                    {
                        return -1;
                    }
                    i = end + 1;
                }
                else if (c == '"' || c == '\'' || c == '`')
                {
                    i++;
                    while (i < brace && text[i] != c && (c == '`' || text[i] != '\n'))
                    {
                        i++;
                    }
                }
                else if (c == '{')
                {
                    open.Push(i);
                }
                else if (c == '}' && open.Count > 0)
                {
                    open.Pop();
                }
            }
            return open.Count > 0 ? open.Peek() : -1;
        }

        /// <summary>A tab, or the smallest indent on any other line, or 4 if no other line has one.</summary>
        private static int IndentUnit(string text, int ownLineStart)
        {
            if (text[ownLineStart] == '\t')
            {
                return 1;
            }
            int unit = int.MaxValue;
            int start = 0;
            while (start < text.Length)
            {
                int end = text.IndexOf('\n', start);
                end = end < 0 ? text.Length : end;
                int width = LeadingWhitespace(text, start).Length;
                if (start != ownLineStart && width > 0 && width < end - start && width < unit)
                {
                    unit = width;
                }
                start = end + 1;
            }
            return unit == int.MaxValue ? 4 : unit;
        }
    }
}
