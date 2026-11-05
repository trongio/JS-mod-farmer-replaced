using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JsMod.Editor
{
    public sealed class FormatResult
    {
        public string Text;

        /// <summary>For each line of the source, the line it became. A dropped blank line maps to the line after it.</summary>
        public int[] LineMap;
    }

    /// <summary>
    /// A conservative whitespace-only formatter. Tokens never move between lines, and comments and strings aren't touched:
    /// only indentation, spacing inside a line, trailing whitespace and runs of blank lines change.
    /// Returns null for anything it can't read, so the caller leaves the code alone.
    /// </summary>
    public static class CodeFormatter
    {
        private enum Kind { Word, Number, Text, LineComment, BlockComment, Raw, Op }

        private sealed class Piece
        {
            public Kind Kind;
            public string Value;
            public bool Unary;
            public bool Prefix;

            public bool IsComment => Kind == Kind.LineComment || Kind == Kind.BlockComment || Kind == Kind.Raw;
            public bool IsOp(string op) => Kind == Kind.Op && Value == op;
            public bool IsOpener => Kind == Kind.Op && (Value == "(" || Value == "[" || Value == "{");
            public bool IsCloser => Kind == Kind.Op && (Value == ")" || Value == "]" || Value == "}");
        }

        private sealed class Line
        {
            /// <summary>Ends inside a block comment or template literal, so its trailing spaces belong to that piece.</summary>
            public bool ContinuesRaw;
            public readonly List<Piece> Pieces = new List<Piece>();
            public int Width;

            // Starts inside a multi-line comment or template, so its start is kept byte for byte.
            public bool StartsRaw;
        }

        private static readonly string[] JsOps =
        {
            ">>>=", "===", "!==", "**=", "<<=", ">>=", ">>>", "...", "&&=", "||=", "??=",
            "==", "!=", "<=", ">=", "&&", "||", "??", "?.", "++", "--", "+=", "-=", "*=", "/=", "%=", "**", "=>",
            "<<", ">>", "&=", "|=", "^=",
            "{", "}", "(", ")", "[", "]", ";", ",", ".", ":", "?", "=", "+", "-", "*", "/", "%", "<", ">", "!", "&", "|", "^", "~"
        };

        private static readonly string[] PyOps =
        {
            "//=", "**", "==", "!=", "<=", ">=", "->", "+=", "-=", "*=", "/=", "%=", "//",
            "(", ")", "[", "]", "{", "}", ",", ".", ":", "=", "+", "-", "*", "/", "%", "<", ">", "|"
        };

        private static readonly HashSet<string> JsSpacedKeywords = new HashSet<string>
        {
            "if", "else", "while", "for", "return", "in", "of", "typeof", "void", "delete", "case", "new", "throw", "import", "from", "export", "default", "let", "const", "var"
        };

        private static readonly HashSet<string> PySpacedKeywords = new HashSet<string>
        {
            "if", "elif", "else", "while", "for", "return", "in", "not", "and", "or", "import", "from", "global"
        };

        /// <summary>Formats the whole file. With collapseBlankLines off, every line stays on its line number.</summary>
        public static FormatResult Format(string source, bool isJs, bool useTabs, bool collapseBlankLines = true)
        {
            if (source == null)
            {
                return null;
            }
            List<Line> lines = Lex(source.Replace('\v', '\n'), isJs);
            return lines == null ? null : Emit(lines, isJs, useTabs, collapseBlankLines);
        }

        /// <summary>Formats only the given lines (0-based, inclusive), indenting them for where they sit in the whole file.</summary>
        public static FormatResult FormatLines(string source, bool isJs, bool useTabs, int firstLine, int lastLine)
        {
            FormatResult full = Format(source, isJs, useTabs, false);
            if (full == null)
            {
                return null;
            }
            string[] before = source.Replace('\v', '\n').Split('\n');
            string[] after = full.Text.Split('\n');
            if (before.Length != after.Length)
            {
                return null;
            }
            for (int i = 0; i < before.Length; i++)
            {
                if (i < firstLine || i > lastLine)
                {
                    after[i] = before[i];
                }
            }
            return new FormatResult { Text = string.Join("\n", after), LineMap = full.LineMap };
        }

        /// <summary>Every comment in reading order. Formatting must not change this list.</summary>
        public static List<string> Comments(string source, bool isJs)
        {
            List<Line> lines = Lex(source.Replace('\v', '\n'), isJs);
            return lines?.SelectMany(l => l.Pieces).Where(p => p.IsComment).Select(p => p.Value).ToList();
        }

        /// <summary>Where a position lands after formatting: same line, and the same number of non-space characters before it.</summary>
        public static int MapPosition(string source, FormatResult result, int position)
        {
            position = Math.Max(0, Math.Min(position, source.Length));
            int line = 0;
            int lineStart = 0;
            for (int i = 0; i < position; i++)
            {
                if (source[i] == '\n' || source[i] == '\v')
                {
                    line++;
                    lineStart = i + 1;
                }
            }
            int solid = 0;
            for (int i = lineStart; i < position; i++)
            {
                if (!char.IsWhiteSpace(source[i]))
                {
                    solid++;
                }
            }
            int newLine = line < result.LineMap.Length ? result.LineMap[line] : 0;
            int start = 0;
            for (int l = 0; l < newLine; l++)
            {
                int next = result.Text.IndexOf('\n', start);
                if (next < 0)
                {
                    break;
                }
                start = next + 1;
            }
            int end = result.Text.IndexOf('\n', start);
            end = end < 0 ? result.Text.Length : end;
            int at = start;
            while (at < end && char.IsWhiteSpace(result.Text[at]))
            {
                at++;
            }
            while (solid > 0 && at < end)
            {
                if (!char.IsWhiteSpace(result.Text[at]))
                {
                    solid--;
                }
                at++;
            }
            while (at < end && char.IsWhiteSpace(result.Text[at]))
            {
                at++;
            }
            return at;
        }

        private static List<Line> Lex(string source, bool isJs)
        {
            var lines = new List<Line>();
            string[] ops = isJs ? JsOps : PyOps;
            var line = new Line();
            lines.Add(line);
            int i = 0;
            MeasureIndent(source, ref i, line);
            while (i < source.Length)
            {
                char c = source[i];
                if (c == '\n')
                {
                    i++;
                    line = new Line();
                    lines.Add(line);
                    MeasureIndent(source, ref i, line);
                }
                else if (c == ' ' || c == '\t')
                {
                    i++;
                }
                else if (isJs ? (c == '/' && Peek(source, i + 1) == '/') : c == '#')
                {
                    int end = source.IndexOf('\n', i);
                    end = end < 0 ? source.Length : end;
                    line.Pieces.Add(new Piece { Kind = Kind.LineComment, Value = source.Substring(i, end - i).TrimEnd() });
                    i = end;
                }
                else if (isJs && c == '/' && Peek(source, i + 1) == '*')
                {
                    int close = source.IndexOf("*/", i + 2, StringComparison.Ordinal);
                    if (close < 0)
                    {
                        return null;
                    }
                    line = AddMultiline(lines, line, source.Substring(i, close + 2 - i), Kind.BlockComment);
                    i = close + 2;
                }
                else if (c == '"' || c == '\'' || (isJs && c == '`'))
                {
                    int end = StringEnd(source, i, isJs);
                    if (end < 0)
                    {
                        return null;
                    }
                    line = AddMultiline(lines, line, source.Substring(i, end + 1 - i), Kind.Text);
                    i = end + 1;
                }
                else if (char.IsLetter(c) || c == '_' || (isJs && c == '$'))
                {
                    int end = i + 1;
                    while (end < source.Length && (char.IsLetterOrDigit(source[end]) || source[end] == '_' || (isJs && source[end] == '$')))
                    {
                        end++;
                    }
                    line.Pieces.Add(new Piece { Kind = Kind.Word, Value = source.Substring(i, end - i) });
                    i = end;
                }
                else if (char.IsDigit(c) || (c == '.' && char.IsDigit(Peek(source, i + 1))))
                {
                    int end = NumberEnd(source, i);
                    line.Pieces.Add(new Piece { Kind = Kind.Number, Value = source.Substring(i, end - i) });
                    i = end;
                }
                else
                {
                    string op = MatchOp(source, i, ops);
                    if (op == null)
                    {
                        return null;
                    }
                    line.Pieces.Add(new Piece { Kind = Kind.Op, Value = op });
                    i += op.Length;
                }
            }
            return lines;
        }

        private static void MeasureIndent(string source, ref int i, Line line)
        {
            while (i < source.Length && (source[i] == ' ' || source[i] == '\t'))
            {
                line.Width += source[i] == '\t' ? 4 : 1;
                i++;
            }
        }

        /// <summary>A piece spanning lines becomes one piece per line, and the lines after the first keep their bytes.</summary>
        private static Line AddMultiline(List<Line> lines, Line line, string text, Kind kind)
        {
            string[] parts = text.Split('\n');
            line.Pieces.Add(new Piece { Kind = kind, Value = parts[0] });
            for (int p = 1; p < parts.Length; p++)
            {
                line.ContinuesRaw = true;
                line = new Line { StartsRaw = true };
                lines.Add(line);
                line.Pieces.Add(new Piece { Kind = kind == Kind.BlockComment ? Kind.Raw : Kind.Text, Value = parts[p] });
            }
            return line;
        }

        private static int StringEnd(string source, int start, bool isJs)
        {
            char quote = source[start];
            for (int i = start + 1; i < source.Length; i++)
            {
                char c = source[i];
                if (c == '\n' && quote != '`')
                {
                    return -1;
                }
                if (isJs && c == '\\')
                {
                    i++;
                }
                else if (c == quote)
                {
                    return i;
                }
            }
            return -1;
        }

        private static int NumberEnd(string source, int start)
        {
            int i = start;
            while (i < source.Length)
            {
                char c = source[i];
                if (char.IsLetterOrDigit(c) || c == '_' || (c == '.' && char.IsDigit(Peek(source, i + 1))))
                {
                    i++;
                }
                else if ((c == '+' || c == '-') && (source[i - 1] == 'e' || source[i - 1] == 'E') && char.IsDigit(Peek(source, i + 1)) && !source.Substring(start, i - start).StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    i++;
                }
                else if (c == '.' && !char.IsLetter(Peek(source, i + 1)) && Peek(source, i + 1) != '.' && Peek(source, i + 1) != '_')
                {
                    // A number like 1. ends with its point.
                    i++;
                    break;
                }
                else
                {
                    break;
                }
            }
            return i;
        }

        private static string MatchOp(string source, int i, string[] ops)
        {
            foreach (string op in ops)
            {
                if (string.CompareOrdinal(source, i, op, 0, op.Length) == 0)
                {
                    return op;
                }
            }
            return null;
        }

        private static char Peek(string source, int i)
        {
            return i < source.Length ? source[i] : '\0';
        }

        private static FormatResult Emit(List<Line> lines, bool isJs, bool useTabs, bool collapse)
        {
            MarkUnaries(lines, isJs);
            HashSet<string> spaced = isJs ? JsSpacedKeywords : PySpacedKeywords;
            var output = new List<string>();
            var map = new int[lines.Count];
            var open = new List<KeyValuePair<char, int>>();
            var widths = new List<int> { 0 };
            int statementLevel = 0;
            int pendingBlanks = 0;
            var pendingLines = new List<int>();
            bool previousEndsOperator = false;
            for (int i = 0; i < lines.Count; i++)
            {
                Line line = lines[i];
                if (line.Pieces.Count == 0)
                {
                    pendingBlanks++;
                    pendingLines.Add(i);
                    continue;
                }
                int level = 0;
                if (!line.StartsRaw)
                {
                    int closers = line.Pieces.TakeWhile(p => p.IsCloser).Count();
                    int depth = Depth(open, closers);
                    bool commentOnly = line.Pieces.All(p => p.IsComment);
                    if (isJs)
                    {
                        Piece first = line.Pieces[0];
                        bool startsOperator = !first.IsComment && first.Kind == Kind.Op && !first.Unary && !first.Prefix && !first.IsCloser && !first.IsOpener && first.Value != ";" && first.Value != ",";
                        level = depth + (!commentOnly && (previousEndsOperator || startsOperator) ? 1 : 0);
                    }
                    else if (open.Count > 0)
                    {
                        level = statementLevel + depth;
                    }
                    else if (commentOnly)
                    {
                        level = CommentLevel(lines, i, widths, line.Width);
                    }
                    else
                    {
                        if (line.Width > widths[widths.Count - 1])
                        {
                            widths.Add(line.Width);
                        }
                        while (widths[widths.Count - 1] > line.Width)
                        {
                            widths.RemoveAt(widths.Count - 1);
                        }
                        if (widths[widths.Count - 1] != line.Width)
                        {
                            return null;
                        }
                        statementLevel = widths.Count - 1;
                        level = statementLevel;
                    }
                }
                int keep = !collapse ? pendingBlanks : output.Count == 0 ? 0 : Math.Min(pendingBlanks, level == 0 && !line.StartsRaw ? 2 : 1);
                for (int b = 0; b < keep; b++)
                {
                    output.Add("");
                }
                for (int b = 0; b < pendingLines.Count; b++)
                {
                    map[pendingLines[b]] = collapse ? output.Count : output.Count - keep + b;
                }
                pendingBlanks = 0;
                pendingLines.Clear();
                map[i] = output.Count;
                string text = line.StartsRaw ? "" : new string(useTabs ? '\t' : ' ', useTabs ? level : level * 4);
                text = RenderLine(line, text, isJs, spaced, open, i);
                if (text == null)
                {
                    return null;
                }
                output.Add(line.StartsRaw || line.ContinuesRaw ? text : text.TrimEnd());
                Piece last = line.Pieces.LastOrDefault(p => !p.IsComment);
                if (last != null)
                {
                    previousEndsOperator = isJs && last.Kind == Kind.Op && !last.IsOpener && !last.IsCloser && last.Value != ";" && last.Value != "," && last.Value != ":" && last.Value != "++" && last.Value != "--" && open.Count == 0;
                }
            }
            if (pendingLines.Count > 0)
            {
                // Trailing blank lines. All are kept when line numbers must not change, and at most a final newline otherwise.
                bool endsWithNewLine = pendingLines[pendingLines.Count - 1] == lines.Count - 1;
                int keep = !collapse ? pendingLines.Count : endsWithNewLine && output.Count > 0 ? 1 : 0;
                for (int b = 0; b < pendingLines.Count; b++)
                {
                    map[pendingLines[b]] = collapse ? Math.Max(0, output.Count - (keep == 0 ? 1 : 0)) : output.Count + b;
                }
                for (int b = 0; b < keep; b++)
                {
                    output.Add("");
                }
            }
            if (output.Count == 0)
            {
                output.Add("");
            }
            return new FormatResult { Text = string.Join("\n", output), LineMap = map };
        }

        /// <summary>A comment lines up with the block it's in, and a comment before a block's first line belongs to that block.</summary>
        private static int CommentLevel(List<Line> lines, int index, List<int> widths, int width)
        {
            Line next = lines.Skip(index + 1).FirstOrDefault(l => l.Pieces.Count > 0 && !l.Pieces.All(p => p.IsComment));
            int top = widths[widths.Count - 1];
            int levels = widths.Count(w => w <= width);
            if (next != null && next.Width > top && next.Width <= width)
            {
                levels++;
            }
            return Math.Max(0, levels - 1);
        }

        /// <summary>Openers on one line count as one level, so f({ only indents its body once.</summary>
        private static int Depth(List<KeyValuePair<char, int>> open, int closers)
        {
            return open.Take(Math.Max(0, open.Count - closers)).Select(o => o.Value).Distinct().Count();
        }

        private static string RenderLine(Line line, string indent, bool isJs, HashSet<string> spaced, List<KeyValuePair<char, int>> open, int lineIndex)
        {
            var text = new StringBuilder(indent);
            Piece previous = null;
            foreach (Piece piece in line.Pieces)
            {
                char top = open.Count > 0 ? open[open.Count - 1].Key : '\0';
                if (previous != null)
                {
                    text.Append(Between(previous, piece, isJs, spaced, top));
                }
                text.Append(piece.Value);
                if (piece.IsOpener)
                {
                    open.Add(new KeyValuePair<char, int>(piece.Value[0], lineIndex));
                }
                else if (piece.IsCloser)
                {
                    if (open.Count == 0)
                    {
                        return null;
                    }
                    open.RemoveAt(open.Count - 1);
                }
                previous = piece;
            }
            return text.ToString();
        }

        private static string Between(Piece left, Piece right, bool isJs, HashSet<string> spaced, char top)
        {
            if (right.Kind == Kind.LineComment)
            {
                return "  ";
            }
            if (right.IsComment || left.IsComment)
            {
                return " ";
            }
            bool spacedWord = left.Kind == Kind.Word && spaced.Contains(left.Value);
            if (right.IsOp(",") || right.IsOp(";") || right.IsOp(")") || right.IsOp("]") || right.IsOp(".") || right.IsOp("?."))
            {
                return "";
            }
            if (left.IsOp("(") || left.IsOp("[") || left.IsOp(".") || left.IsOp("?.") || left.IsOp("..."))
            {
                return "";
            }
            if (right.IsOp("}"))
            {
                return left.IsOp("{") || !isJs ? "" : " ";
            }
            if (left.IsOp("{"))
            {
                return isJs ? " " : "";
            }
            if (left.IsOp(",") || left.IsOp(";"))
            {
                return " ";
            }
            if (right.IsOp(":"))
            {
                return "";
            }
            if (left.IsOp(":"))
            {
                return !isJs && top == '[' ? "" : " ";
            }
            if (left.Unary || left.Prefix)
            {
                return "";
            }
            if ((right.IsOp("++") || right.IsOp("--")) && !right.Prefix)
            {
                return "";
            }
            if (!isJs && top == '(' && (left.IsOp("=") || right.IsOp("=")))
            {
                return "";
            }
            if (right.IsOp("(") || right.IsOp("["))
            {
                bool operand = (left.Kind == Kind.Word && !spacedWord) || left.Kind == Kind.Text || left.Kind == Kind.Number || left.IsOp(")") || left.IsOp("]") || (isJs && left.IsOp("}") && right.IsOp("("));
                return operand ? "" : " ";
            }
            return " ";
        }

        private static void MarkUnaries(List<Line> lines, bool isJs)
        {
            HashSet<string> spaced = isJs ? JsSpacedKeywords : PySpacedKeywords;
            Piece previous = null;
            int depth = 0;
            foreach (Line line in lines)
            {
                if (!isJs && depth == 0)
                {
                    previous = null;
                }
                foreach (Piece piece in line.Pieces)
                {
                    if (piece.IsComment)
                    {
                        continue;
                    }
                    bool afterOperand = previous != null && (previous.Kind == Kind.Number || previous.Kind == Kind.Text || (previous.Kind == Kind.Word && !spaced.Contains(previous.Value)) || previous.IsOp(")") || previous.IsOp("]") || (previous.IsOp("}") && isJs) || (isJs && !previous.Prefix && (previous.IsOp("++") || previous.IsOp("--"))));
                    if (piece.Kind == Kind.Op && !afterOperand)
                    {
                        if (piece.Value == "-" || piece.Value == "+" || piece.Value == "!" || piece.Value == "~" || piece.Value == "..." || (!isJs && (piece.Value == "*" || piece.Value == "**")))
                        {
                            piece.Unary = true;
                        }
                        else if (piece.Value == "++" || piece.Value == "--")
                        {
                            piece.Prefix = true;
                        }
                    }
                    else if (isJs && piece.IsOp("!"))
                    {
                        piece.Unary = true;
                    }
                    depth += piece.IsOpener ? 1 : piece.IsCloser ? -1 : 0;
                    previous = piece;
                }
            }
        }
    }
}
