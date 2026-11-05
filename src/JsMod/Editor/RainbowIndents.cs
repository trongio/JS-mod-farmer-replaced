using System.Collections.Generic;
using System.Text;

namespace JsMod.Editor
{
    /// <summary>
    /// Colours each level of a line's indentation behind the code. It works on the highlighter's finished output, adding TextMeshPro
    /// marks only around leading whitespace, so the characters shown never change and the text stays lined up with the input field.
    /// </summary>
    public static class RainbowIndents
    {
        private const int DefaultUnit = 4;

        /// <summary>Low alpha, so that it reads on the dark and the light themes. Levels past the end start over.</summary>
        public static readonly string[] Palette =
        {
            "#FF5F5F2E", "#FFB3472E", "#E6D2352E", "#5FD9672E", "#5FB0FF2E", "#C58BFF2E"
        };

        public struct Span
        {
            public int Start;
            public int Length;
            public int Level;
        }

        /// <summary>Two spaces if every line works in twos but not fours, otherwise four, the size the game's editor uses.</summary>
        public static int IndentUnit(string code)
        {
            bool fours = true;
            bool twos = true;
            int i = 0;
            while (i < code.Length)
            {
                int spaces = 0;
                while (i + spaces < code.Length && code[i + spaces] == ' ')
                {
                    spaces++;
                }
                int end = code.IndexOf('\n', i);
                end = end < 0 ? code.Length : end;
                if (i + spaces < end && code[i + spaces] != '\t' && code[i + spaces] != '\r')
                {
                    fours &= spaces % 4 == 0;
                    twos &= spaces % 2 == 0;
                }
                i = end + 1;
            }
            return !fours && twos ? 2 : DefaultUnit;
        }

        /// <summary>The coloured runs of leading whitespace. A tab is one level, and spaces that don't fill a level, or lines with nothing else on them, stay plain.</summary>
        public static List<Span> Spans(string code)
        {
            var spans = new List<Span>();
            int unit = IndentUnit(code);
            int i = 0;
            while (i < code.Length)
            {
                int end = code.IndexOf('\n', i);
                end = end < 0 ? code.Length : end;
                int text = i;
                while (text < end && (code[text] == ' ' || code[text] == '\t'))
                {
                    text++;
                }
                if (text > i && text < end && code[text] != '\r')
                {
                    int level = 0;
                    int run = i;
                    while (run < text)
                    {
                        if (code[run] == '\t')
                        {
                            spans.Add(new Span { Start = run, Length = 1, Level = level++ });
                            run++;
                            continue;
                        }
                        int spaces = run;
                        while (spaces < text && code[spaces] == ' ')
                        {
                            spaces++;
                        }
                        for (; run + unit <= spaces; run += unit)
                        {
                            spans.Add(new Span { Start = run, Length = unit, Level = level++ });
                        }
                        run = spaces;
                    }
                }
                i = end + 1;
            }
            return spans;
        }

        /// <summary>
        /// Adds the marks to what the highlighter made from <paramref name="code"/>. Whitespace inside a coloured token is the inside of
        /// a multi-line comment or string, and stays plain. Anything unexpected, such as code containing a literal end-of-noparse tag, leaves the text as it was.
        /// </summary>
        public static string Apply(string highlighted, string code)
        {
            List<Span> spans = Spans(code);
            if (spans.Count == 0)
            {
                return highlighted;
            }
            var result = new StringBuilder(highlighted.Length + spans.Count * 48);
            const string Open = "<noparse>";
            const string Close = "</noparse>";
            int next = 0;
            int visible = 0;
            int colours = 0;
            int i = 0;
            while (i < highlighted.Length)
            {
                if (string.CompareOrdinal(highlighted, i, Open, 0, Open.Length) == 0)
                {
                    int close = highlighted.IndexOf(Close, i + Open.Length, System.StringComparison.Ordinal);
                    if (close < 0)
                    {
                        return highlighted;
                    }
                    int start = i + Open.Length;
                    int length = close - start;
                    result.Append(Open);
                    int done = 0;
                    while (next < spans.Count && spans[next].Start < visible + length)
                    {
                        Span span = spans[next];
                        int from = System.Math.Max(span.Start, visible + done);
                        int to = System.Math.Min(span.Start + span.Length, visible + length);
                        if (from < to)
                        {
                            result.Append(highlighted, start + done, from - visible - done);
                            string piece = highlighted.Substring(start + from - visible, to - from);
                            if (colours == 0)
                            {
                                result.Append(Close).Append("<mark=").Append(Palette[span.Level % Palette.Length]).Append('>').Append(Open);
                                result.Append(piece).Append(Close).Append("</mark>").Append(Open);
                            }
                            else
                            {
                                result.Append(piece);
                            }
                            done = to - visible;
                        }
                        if (span.Start + span.Length > visible + length)
                        {
                            break;
                        }
                        next++;
                    }
                    result.Append(highlighted, start + done, length - done).Append(Close);
                    visible += length;
                    i = close + Close.Length;
                    continue;
                }
                if (highlighted[i] != '<')
                {
                    return highlighted;
                }
                int tagEnd = highlighted.IndexOf('>', i);
                if (tagEnd < 0)
                {
                    return highlighted;
                }
                if (string.CompareOrdinal(highlighted, i, "<color=", 0, 7) == 0)
                {
                    colours++;
                }
                else if (string.CompareOrdinal(highlighted, i, "</color>", 0, 8) == 0)
                {
                    colours--;
                }
                result.Append(highlighted, i, tagEnd + 1 - i);
                i = tagEnd + 1;
            }
            return visible == code.Length ? result.ToString() : highlighted;
        }
    }
}
