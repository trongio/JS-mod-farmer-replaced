using System;
using System.Collections.Generic;
using System.Linq;

namespace JsMod.Editor
{
    public sealed class EditResult
    {
        public string Text;
        public int SelectionStart;
        public int SelectionEnd;

        public EditResult(string text, int start, int end)
        {
            Text = text;
            SelectionStart = start;
            SelectionEnd = end;
        }
    }

    /// <summary>Editor actions as pure text changes. Each takes the text and selection, and returns null when there's nothing to do.</summary>
    public static class LineOps
    {
        private static int LineStart(string text, int position)
        {
            return position <= 0 ? 0 : text.LastIndexOf('\n', position - 1) + 1;
        }

        private static int LineEnd(string text, int position)
        {
            int end = text.IndexOf('\n', position);
            return end < 0 ? text.Length : end;
        }

        /// <summary>The selected lines' span, not counting a line the selection only touches at its very start.</summary>
        private static void SelectedLines(string text, int start, int end, out int first, out int last)
        {
            first = LineStart(text, start);
            int stop = end > start && text[end - 1] == '\n' ? end - 1 : end;
            last = LineEnd(text, stop);
        }

        public static EditResult Duplicate(string text, int start, int end)
        {
            if (end > start)
            {
                string selection = text.Substring(start, end - start);
                return new EditResult(text.Insert(end, selection), end, end + selection.Length);
            }
            int lineStart = LineStart(text, start);
            int lineEnd = LineEnd(text, start);
            string line = text.Substring(lineStart, lineEnd - lineStart);
            int caret = start + line.Length + 1;
            return new EditResult(text.Insert(lineEnd, "\n" + line), caret, caret);
        }

        public static EditResult DeleteLines(string text, int start, int end)
        {
            if (text.Length == 0)
            {
                return null;
            }
            SelectedLines(text, start, end, out int first, out int last);
            int column = start - first;
            int removeFrom = first;
            int removeTo = last;
            if (last < text.Length)
            {
                removeTo = last + 1;
            }
            else if (first > 0)
            {
                removeFrom = first - 1;
            }
            string result = text.Remove(removeFrom, removeTo - removeFrom);
            int newLine = Math.Min(removeFrom == first ? first : LineStart(result, removeFrom), result.Length);
            int caret = Math.Min(newLine + column, LineEnd(result, newLine));
            return new EditResult(result, caret, caret);
        }

        public static EditResult MoveLines(string text, int start, int end, bool up)
        {
            SelectedLines(text, start, end, out int first, out int last);
            string block = text.Substring(first, last - first);
            if (up)
            {
                if (first == 0)
                {
                    return null;
                }
                int previous = LineStart(text, first - 1);
                string above = text.Substring(previous, first - 1 - previous);
                string moved = text.Substring(0, previous) + block + "\n" + above + text.Substring(last);
                int shift = above.Length + 1;
                return new EditResult(moved, start - shift, end - shift);
            }
            if (last >= text.Length)
            {
                return null;
            }
            int nextEnd = LineEnd(text, last + 1);
            string below = text.Substring(last + 1, nextEnd - last - 1);
            string result = text.Substring(0, first) + below + "\n" + block + text.Substring(nextEnd);
            int offset = below.Length + 1;
            return new EditResult(result, start + offset, end + offset);
        }

        public static EditResult ToggleComment(string text, int start, int end, bool isJs)
        {
            SelectedLines(text, start, end, out int first, out int last);
            string token = isJs ? "//" : "#";
            string[] lines = text.Substring(first, last - first).Split('\n');
            List<string> solid = lines.Where(l => l.Trim().Length > 0).ToList();
            if (solid.Count == 0)
            {
                return null;
            }
            bool remove = solid.All(l => l.TrimStart().StartsWith(token, StringComparison.Ordinal));
            int indent = solid.Min(l => l.Length - l.TrimStart().Length);
            var edits = new List<KeyValuePair<int, int>>();
            int lineAt = first;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                string changed = line;
                if (line.Trim().Length > 0)
                {
                    if (remove)
                    {
                        int at = line.Length - line.TrimStart().Length;
                        int length = token.Length + (at + token.Length < line.Length && line[at + token.Length] == ' ' ? 1 : 0);
                        changed = line.Remove(at, length);
                        edits.Add(new KeyValuePair<int, int>(lineAt + at, -length));
                    }
                    else
                    {
                        changed = line.Insert(indent, token + " ");
                        edits.Add(new KeyValuePair<int, int>(lineAt + indent, token.Length + 1));
                    }
                }
                lineAt += line.Length + 1;
                lines[i] = changed;
            }
            string result = text.Substring(0, first) + string.Join("\n", lines) + text.Substring(last);
            return new EditResult(result, Shift(start, edits), Shift(end, edits));
        }

        private static int Shift(int position, List<KeyValuePair<int, int>> edits)
        {
            int shifted = position;
            foreach (KeyValuePair<int, int> edit in edits)
            {
                if (edit.Value > 0 ? position >= edit.Key : position > edit.Key)
                {
                    shifted += edit.Value < 0 ? -Math.Min(-edit.Value, position - edit.Key) : edit.Value;
                }
            }
            return shifted;
        }

        /// <summary>Starts a line below the current one, with its indent, and one more after a line that opens a block.</summary>
        public static EditResult NewLineBelow(string text, int start, int end, bool isJs, bool useTabs)
        {
            int position = Math.Max(start, end);
            int lineStart = LineStart(text, position);
            int lineEnd = LineEnd(text, position);
            string line = text.Substring(lineStart, lineEnd - lineStart);
            string indent = line.Substring(0, line.Length - line.TrimStart().Length);
            string trimmed = line.TrimEnd();
            if (trimmed.EndsWith(isJs ? "{" : ":", StringComparison.Ordinal))
            {
                indent += useTabs ? "\t" : "    ";
            }
            string inserted = "\n" + indent;
            int caret = lineEnd + inserted.Length;
            return new EditResult(text.Insert(lineEnd, inserted), caret, caret);
        }

        /// <summary>Refuses when a line comment would swallow the code joined after it.</summary>
        public static EditResult JoinLines(string text, int start, int end, bool isJs)
        {
            SelectedLines(text, start, end, out int first, out int last);
            if (last >= text.Length && first == LineStart(text, last))
            {
                return null;
            }
            int stop = last;
            if (first == LineStart(text, last))
            {
                stop = LineEnd(text, last + 1);
            }
            string[] lines = text.Substring(first, stop - first).Split('\n');
            for (int i = 0; i < lines.Length - 1; i++)
            {
                if (MayEndInLineComment(lines[i], isJs))
                {
                    return null;
                }
            }
            string joined = lines[0].TrimEnd();
            int caret = first + joined.Length;
            for (int i = 1; i < lines.Length; i++)
            {
                string next = lines[i].Trim();
                if (next.Length == 0)
                {
                    continue;
                }
                bool tight = joined.Length == 0 || next[0] == ')' || next[0] == ']' || next[0] == ',' || next[0] == ';' || next[0] == '.' || joined[joined.Length - 1] == '(' || joined[joined.Length - 1] == '[';
                joined += (tight ? "" : " ") + next;
            }
            return new EditResult(text.Substring(0, first) + joined + text.Substring(stop), caret, caret);
        }

        /// <summary>Whether a line has a line comment outside its strings. Anything it can't follow counts as yes.</summary>
        private static bool MayEndInLineComment(string line, bool isJs)
        {
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '\'' || c == '"' || (isJs && c == '`'))
                {
                    int close = i + 1;
                    while (close < line.Length && line[close] != c)
                    {
                        close += isJs && line[close] == '\\' ? 2 : 1;
                    }
                    if (close >= line.Length)
                    {
                        return true;
                    }
                    i = close;
                }
                else if (!isJs && c == '#')
                {
                    return true;
                }
                else if (isJs && c == '/' && i + 1 < line.Length && line[i + 1] == '/')
                {
                    return true;
                }
                else if (isJs && c == '/' && i + 1 < line.Length && line[i + 1] == '*')
                {
                    int end = line.IndexOf("*/", i + 2, StringComparison.Ordinal);
                    if (end < 0)
                    {
                        return true;
                    }
                    i = end + 1;
                }
            }
            return false;
        }

        public static EditResult ToggleCase(string text, int start, int end)
        {
            if (end == start)
            {
                WordBounds(text, start, false, out start, out end);
                if (end == start)
                {
                    return null;
                }
            }
            string selection = text.Substring(start, end - start);
            string changed = selection.Any(char.IsLower) ? selection.ToUpperInvariant() : selection.ToLowerInvariant();
            return new EditResult(text.Substring(0, start) + changed + text.Substring(end), start, end);
        }

        private static bool IsWordChar(char c, bool dots)
        {
            return char.IsLetterOrDigit(c) || c == '_' || (dots && c == '.');
        }

        private static void WordBounds(string text, int position, bool dots, out int start, out int end)
        {
            start = Math.Max(0, Math.Min(position, text.Length));
            end = start;
            while (start > 0 && IsWordChar(text[start - 1], dots))
            {
                start--;
            }
            while (end < text.Length && IsWordChar(text[end], dots))
            {
                end++;
            }
        }

        /// <summary>The word at the caret, keeping dots so that Items.Carrot stays whole. Empty when there's none.</summary>
        public static string WordAt(string text, int position, out int wordStart)
        {
            WordBounds(text, position, true, out wordStart, out int end);
            return text.Substring(wordStart, end - wordStart);
        }

        /// <summary>The function whose call the caret is inside, for parameter info. Empty when there's none on this statement.</summary>
        public static string CalleeAt(string text, int position, out int wordStart)
        {
            int depth = 0;
            char quote = '\0';
            for (int i = Math.Min(position, text.Length) - 1; i >= 0; i--)
            {
                char c = text[i];
                if (quote != '\0')
                {
                    quote = c == quote ? '\0' : quote;
                }
                else if (c == '"' || c == '\'')
                {
                    quote = c;
                }
                else if (c == ')' || c == ']')
                {
                    depth++;
                }
                else if (c == '[' && depth > 0)
                {
                    depth--;
                }
                else if (c == '(')
                {
                    if (depth > 0)
                    {
                        depth--;
                        continue;
                    }
                    int end = i;
                    while (end > 0 && text[end - 1] == ' ')
                    {
                        end--;
                    }
                    return WordAt(text, end, out wordStart);
                }
                else if ((c == '\n' || c == ';' || c == '{' || c == '}') && depth == 0)
                {
                    break;
                }
            }
            wordStart = 0;
            return "";
        }
    }
}
