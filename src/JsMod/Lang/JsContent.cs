using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace JsMod.Lang
{
    /// <summary>
    /// The JavaScript twins of the code in the game's text, kept in the plugin's js folder. Lookups are exact matches on the
    /// Python code, which is the same in every language the game text keeps its code unchanged in. A miss leaves the Python
    /// text in place, so a game update can make text look old but can't break it.
    /// </summary>
    public static class JsContent
    {
        private static readonly Regex CodeDelimiter = new Regex("(\r\n```|\n```|```|`)", RegexOptions.Compiled);

        private static readonly Regex ExampleBlock = new Regex(
            @"(\{\{codeexample\b.*?#CODE[ \t]*\r?\n)(.*?)(\s*\}\})",
            RegexOptions.Compiled | RegexOptions.Singleline);

        private static readonly Regex Placeholder = new Regex(@"\{\d+\}", RegexOptions.Compiled);

        private static readonly ConcurrentDictionary<string, string> TagCache = new ConcurrentDictionary<string, string>();
        private static readonly ConcurrentDictionary<string, byte> LoggedMisses = new ConcurrentDictionary<string, byte>();

        private static volatile string folder;
        private static volatile Dictionary<string, string> spans = new Dictionary<string, string>();
        private static volatile Dictionary<string, string> examples = new Dictionary<string, string>();
        private static volatile HashSet<string> exampleCode = new HashSet<string>();
        private static volatile HashSet<string> knownJs = new HashSet<string>();
        private static volatile HashSet<string> overridePages = new HashSet<string>();

        // Strings first, so words inside them don't count.
        private static readonly Regex QuotedText = new Regex("\"[^\"\n]*\"|'[^'\n]*'", RegexOptions.Compiled);

        private static readonly Regex PythonSyntax = new Regex(
            @"\b(def|elif|pass|global|True|False|None|and|or|not)\b|(^|\s)#|:[ \t]*($|\n)|\bfor\s+\w+(\s*,\s*\w+)*\s+in\b|(^|\n)\s*(import|from)\s+\w",
            RegexOptions.Compiled);

        /// <summary>Told once about each example or file that couldn't be mapped.</summary>
        public static volatile Action<string> Warn;

        public const string SpansFile = "spans.txt";
        public const string ExamplesFile = "examples.txt";

        public static void Load(string contentFolder)
        {
            folder = contentFolder;
            Dictionary<string, string> loadedSpans;
            Dictionary<string, string> loaded;
            if (Directory.Exists(contentFolder))
            {
                loadedSpans = ReadPairs(Path.Combine(contentFolder, SpansFile));
                loaded = ReadPairs(Path.Combine(contentFolder, ExamplesFile));
            }
            else
            {
                Warn?.Invoke("Missing the folder " + contentFolder + ", which has to sit beside JsMod.dll. Without it, code in the game's text stays Python.");
                loadedSpans = new Dictionary<string, string>();
                loaded = new Dictionary<string, string>();
            }
            spans = loadedSpans;
            examples = loaded;
            exampleCode = new HashSet<string>(loaded.Values);
            var js = new HashSet<string>(loaded.Values);
            js.UnionWith(loadedSpans.Values);
            knownJs = js;
            overridePages = ListOverridePages(contentFolder);
            TagCache.Clear();
            LoggedMisses.Clear();
        }

        /// <summary>The whole pages in the folder, as "language/doc". Read once, because the game asks for docs on every hover.</summary>
        private static HashSet<string> ListOverridePages(string contentFolder)
        {
            var pages = new HashSet<string>();
            if (Directory.Exists(contentFolder))
            {
                foreach (string language in Directory.GetDirectories(contentFolder))
                {
                    foreach (string file in Directory.GetFiles(language, "*", SearchOption.AllDirectories))
                    {
                        pages.Add(file.Substring(contentFolder.Length).Replace('\\', '/').TrimStart('/'));
                    }
                }
            }
            return pages;
        }

        private static Dictionary<string, string> ReadPairs(string path)
        {
            if (!File.Exists(path))
            {
                Warn?.Invoke("Missing " + path + ", so that code stays Python.");
                return new Dictionary<string, string>();
            }
            var pairs = new Dictionary<string, string>();
            foreach (Pair pair in ParsePairs(File.ReadAllText(path)))
            {
                pairs[pair.Python] = pair.Js;
            }
            return pairs;
        }

        public struct Pair
        {
            public string Name;
            public string Python;
            public string Js;
            public bool IsFragment;
        }

        /// <summary>The format is "@@py [name]", the Python code, "@@js [frag]", then the JavaScript. Text before the first @@py is a header.</summary>
        public static List<Pair> ParsePairs(string text)
        {
            var pairs = new List<Pair>();
            string[] lines = text.Replace("\r\n", "\n").Split('\n');
            var python = new List<string>();
            var js = new List<string>();
            List<string> into = null;
            var current = new Pair();
            void Finish()
            {
                if (into != null)
                {
                    current.Python = string.Join("\n", python);
                    current.Js = string.Join("\n", js);
                    pairs.Add(current);
                }
            }
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (line == "@@py" || line.StartsWith("@@py ", StringComparison.Ordinal))
                {
                    Finish();
                    python.Clear();
                    js.Clear();
                    into = python;
                    current = new Pair { Name = line.Length > 5 ? line.Substring(5) : null };
                }
                else if (into != null && (line == "@@js" || line == "@@js frag"))
                {
                    into = js;
                    current.IsFragment = line.Length > 4;
                }
                else if (into != null && !(i == lines.Length - 1 && line.Length == 0))
                {
                    into.Add(line);
                }
            }
            Finish();
            return pairs;
        }

        /// <summary>
        /// Whether code outside any window should be coloured as JavaScript: it has to be code this class supplied, or at least not
        /// look like Python. Without the content files, everything is still Python.
        /// </summary>
        public static bool ColoursAsJs(string code)
        {
            if (knownJs.Count == 0 || code == null)
            {
                return false;
            }
            return knownJs.Contains(code) || !PythonSyntax.IsMatch(QuotedText.Replace(code, "\"\""));
        }

        /// <summary>Swaps code between backticks, splitting the text the way the game's ApplyCodeTags does. The result depends only on the text, so it's cached by it.</summary>
        public static string MapCodeTags(string text)
        {
            if (text == null || text.IndexOf('`') < 0)
            {
                return text;
            }
            if (TagCache.TryGetValue(text, out string cached))
            {
                return cached;
            }
            string[] parts = CodeDelimiter.Split(text);
            bool formatted = Placeholder.IsMatch(text);
            bool inCode = false;
            for (int i = 0; i < parts.Length; i++)
            {
                if (i % 2 == 1)
                {
                    inCode = !inCode;
                }
                else if (inCode && spans.TryGetValue(parts[i].Replace("\r\n", "\n"), out string js))
                {
                    // Text with {0} goes through string.Format later, which would choke on a brace in the new code.
                    parts[i] = formatted ? js.Replace("{", "{{").Replace("}", "}}") : js;
                }
            }
            string mapped = string.Concat(parts);
            if (TagCache.Count > 10000)
            {
                TagCache.Clear();
            }
            TagCache[text] = mapped;
            return mapped;
        }

        /// <summary>The JavaScript for one piece of code, or the code itself when there is none.</summary>
        public static string MapCode(string code)
        {
            return code != null && spans.TryGetValue(code, out string js) ? js : code;
        }

        /// <summary>Swaps the code of each example in a docs page. Its setup code stays, because the game's setup is internal Python.</summary>
        public static string MapExamples(string doc, string docName)
        {
            if (string.IsNullOrEmpty(doc) || doc.IndexOf("{{codeexample", StringComparison.Ordinal) < 0)
            {
                return doc;
            }
            return ExampleBlock.Replace(doc, match =>
            {
                string code = match.Groups[2].Value.Replace("\r\n", "\n").TrimEnd();
                if (examples.TryGetValue(code, out string js))
                {
                    return match.Groups[1].Value + js + match.Groups[3].Value;
                }
                if (LoggedMisses.TryAdd(docName + "\n" + code, 0))
                {
                    Warn?.Invoke("No JavaScript for an example in " + docName + ", so it stays Python.");
                }
                return match.Value;
            });
        }

        /// <summary>True for code that came from this class. Anything else in a docs example is still Python and has to run as Python.</summary>
        public static bool IsJsExample(string code)
        {
            return code != null && exampleCode.Contains(code.Replace("\r\n", "\n").TrimEnd());
        }

        /// <summary>A whole page written for JavaScript, for pages that teach syntax. The display language first, then English.</summary>
        public static string PageOverride(string language, string doc)
        {
            string root = folder;
            HashSet<string> pages = overridePages;
            if (root == null || pages.Count == 0 || string.IsNullOrEmpty(doc))
            {
                return null;
            }
            foreach (string candidate in new[] { language, "EN" })
            {
                if (!string.IsNullOrEmpty(candidate) && pages.Contains(candidate + "/" + doc.Replace('\\', '/')))
                {
                    return File.ReadAllText(Path.Combine(root, candidate, doc));
                }
            }
            return null;
        }
    }
}
