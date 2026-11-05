using System.Collections.Generic;
using System.Globalization;

namespace JsMod.Lang
{
    /// <summary>
    /// Turns JS source into the game's TokenStream. The stream starts with a sentinel that the Parser patch uses to spot JS.
    /// NEW_LINE and IDENTIFIER tokens use the game's real token types because the editor reads them for auto-indent and
    /// autocomplete. Never throws: anything unlexable becomes an Unknown token so the parser reports it with a position.
    /// </summary>
    public static class JsTokenizer
    {
        public const TokenType Sentinel = (TokenType)1000;
        public const TokenType Number = (TokenType)1001;
        public const TokenType Text = (TokenType)1002;
        public const TokenType Keyword = (TokenType)1003;
        public const TokenType Punct = (TokenType)1004;
        public const TokenType Unknown = (TokenType)1005;

        public static readonly HashSet<string> Keywords = new HashSet<string>
        {
            "let", "const", "var", "function", "if", "else", "while", "for", "in", "return", "break", "continue",
            "true", "false", "null", "undefined",
            // Reserved in JS and not supported yet. The parser reports each of these by name.
            "class", "new", "typeof", "switch", "case", "default", "do", "try", "catch", "finally", "throw",
            "delete", "void", "this", "super", "import", "export", "instanceof", "with", "extends", "enum", "debugger"
        };

        // Longest first, so that === wins over == and =.
        private static readonly string[] Puncts =
        {
            ">>>=", "===", "!==", "**=", "<<=", ">>=", ">>>", "...", "&&=", "||=", "??=",
            "==", "!=", "<=", ">=", "&&", "||", "??", "?.", "++", "--", "+=", "-=", "*=", "/=", "%=", "**", "=>",
            "<<", ">>", "&=", "|=", "^=",
            "{", "}", "(", ")", "[", "]", ";", ",", ".", ":", "?", "=", "+", "-", "*", "/", "%", "<", ">", "!", "&", "|", "^", "~"
        };

        public static TokenStream Tokenize(string source)
        {
            string code = source.Replace('\v', '\n');
            var stream = new TokenStream();
            stream.Add(new Token(Sentinel, "", 0));
            stream.Add(new Token(TokenType.NEW_LINE, "\n", 0));
            int i = 0;
            while (i < code.Length)
            {
                char c = code[i];
                if (c == '\n')
                {
                    i = AddNewLine(stream, code, i);
                }
                else if (char.IsWhiteSpace(c) || c == '﻿')
                {
                    i++;
                }
                else if (c == '/' && Peek(code, i + 1) == '/')
                {
                    while (i < code.Length && code[i] != '\n')
                    {
                        i++;
                    }
                }
                else if (c == '/' && Peek(code, i + 1) == '*')
                {
                    i = BlockComment(stream, code, i);
                }
                else if (IsIdentStart(c))
                {
                    int end = i + 1;
                    while (end < code.Length && IsIdentPart(code[end]))
                    {
                        end++;
                    }
                    string word = code.Substring(i, end - i);
                    stream.Add(new Token(Keywords.Contains(word) ? Keyword : TokenType.IDENTIFIER, word, i));
                    i = end;
                }
                else if (char.IsDigit(c) || (c == '.' && char.IsDigit(Peek(code, i + 1))))
                {
                    i = NumberToken(stream, code, i);
                }
                else if (c == '"' || c == '\'' || c == '`')
                {
                    i = StringToken(stream, code, i);
                }
                else
                {
                    i = PunctToken(stream, code, i);
                }
            }
            while (stream.Last != null && stream.Last.type == TokenType.NEW_LINE)
            {
                stream.RemoveLast();
            }
            return stream;
        }

        public static bool TryParseNumber(string text, out double value)
        {
            if (text.StartsWith("0x") || text.StartsWith("0X"))
            {
                value = 0;
                for (int i = 2; i < text.Length; i++)
                {
                    value = value * 16 + System.Convert.ToInt32(text[i].ToString(), 16);
                }
                return text.Length > 2;
            }
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }

        private static int AddNewLine(TokenStream stream, string code, int i)
        {
            int end = i + 1;
            while (end < code.Length && (code[end] == ' ' || code[end] == '\t'))
            {
                end++;
            }
            stream.Add(new Token(TokenType.NEW_LINE, code.Substring(i, end - i), i));
            return end;
        }

        private static int BlockComment(TokenStream stream, string code, int start)
        {
            int close = code.IndexOf("*/", start + 2, System.StringComparison.Ordinal);
            if (close < 0)
            {
                stream.Add(new Token(Unknown, code.Substring(start), start));
                return code.Length;
            }
            // A comment spanning lines still ends a statement, as in JS.
            for (int i = start; i < close; i++)
            {
                if (code[i] == '\n')
                {
                    stream.Add(new Token(TokenType.NEW_LINE, "\n", i));
                }
            }
            return close + 2;
        }

        private static int NumberToken(TokenStream stream, string code, int start)
        {
            int i = start;
            if (code[i] == '0' && (Peek(code, i + 1) == 'x' || Peek(code, i + 1) == 'X'))
            {
                i += 2;
                while (i < code.Length && System.Uri.IsHexDigit(code[i]))
                {
                    i++;
                }
            }
            else
            {
                while (i < code.Length && char.IsDigit(code[i]))
                {
                    i++;
                }
                if (Peek(code, i) == '.')
                {
                    i++;
                    while (i < code.Length && char.IsDigit(code[i]))
                    {
                        i++;
                    }
                }
                if (Peek(code, i) == 'e' || Peek(code, i) == 'E')
                {
                    int j = i + 1;
                    if (Peek(code, j) == '+' || Peek(code, j) == '-')
                    {
                        j++;
                    }
                    if (char.IsDigit(Peek(code, j)))
                    {
                        i = j;
                        while (i < code.Length && char.IsDigit(code[i]))
                        {
                            i++;
                        }
                    }
                }
            }
            bool malformed = i < code.Length && IsIdentPart(code[i]);
            while (i < code.Length && IsIdentPart(code[i]))
            {
                i++;
            }
            stream.Add(new Token(malformed ? Unknown : Number, code.Substring(start, i - start), start));
            return i;
        }

        private static int StringToken(TokenStream stream, string code, int start)
        {
            char quote = code[start];
            int i = start + 1;
            while (i < code.Length && code[i] != quote && (quote == '`' || code[i] != '\n'))
            {
                i++;
            }
            if (i >= code.Length || code[i] != quote)
            {
                stream.Add(new Token(Unknown, code.Substring(start, i - start), start));
                return i;
            }
            stream.Add(new Token(Text, code.Substring(start, i + 1 - start), start));
            return i + 1;
        }

        private static int PunctToken(TokenStream stream, string code, int i)
        {
            foreach (string p in Puncts)
            {
                if (string.CompareOrdinal(code, i, p, 0, p.Length) == 0)
                {
                    stream.Add(new Token(Punct, p, i));
                    return i + p.Length;
                }
            }
            stream.Add(new Token(Unknown, code[i].ToString(), i));
            return i + 1;
        }

        private static char Peek(string code, int i)
        {
            return i < code.Length ? code[i] : '\0';
        }

        private static bool IsIdentStart(char c)
        {
            return char.IsLetter(c) || c == '_' || c == '$';
        }

        private static bool IsIdentPart(char c)
        {
            return char.IsLetterOrDigit(c) || c == '_' || c == '$';
        }
    }
}
