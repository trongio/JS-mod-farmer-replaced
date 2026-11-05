using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using JsMod.Lang;

namespace JsMod.Editor
{
    public enum FormatOutcome
    {
        Formatted,
        Unchanged,
        DoesNotParse,
        Unsafe
    }

    public sealed class GateResult
    {
        public FormatOutcome Outcome;
        public FormatResult Result;
    }

    /// <summary>
    /// The only way formatted code reaches the player. The source must parse, and the result must have the same tokens,
    /// comments and tree, and format to itself. Anything else leaves the code alone, including any exception.
    /// </summary>
    public static class FormatGate
    {
        public static GateResult Run(string source, bool isJs, bool useTabs, bool keepLines = false, int firstLine = -1, int lastLine = -1)
        {
            return Check(source, isJs, text => Apply(text, isJs, useTabs, keepLines, firstLine, lastLine));
        }

        /// <summary>The gate itself, around any formatter, so that tests can feed it results a good formatter wouldn't produce.</summary>
        public static GateResult Check(string source, bool isJs, Func<string, FormatResult> format)
        {
            try
            {
                Program before;
                try
                {
                    before = ParseProgram(source, isJs);
                    before.syntaxTree.CheckForNonsensicalCode();
                }
                catch (ParseException)
                {
                    return new GateResult { Outcome = FormatOutcome.DoesNotParse };
                }
                FormatResult result = format(source);
                if (result == null)
                {
                    return Unsafe();
                }
                if (result.Text == source)
                {
                    return new GateResult { Outcome = FormatOutcome.Unchanged, Result = result };
                }
                if (!Tokens(source, isJs).SequenceEqual(Tokens(result.Text, isJs)) || !Enumerable.SequenceEqual(CodeFormatter.Comments(source, isJs), CodeFormatter.Comments(result.Text, isJs)))
                {
                    return Unsafe();
                }
                if (format(result.Text)?.Text != result.Text)
                {
                    return Unsafe();
                }
                if (before.syntaxTree.ToString() != ParseProgram(result.Text, isJs).syntaxTree.ToString())
                {
                    return Unsafe();
                }
                return new GateResult { Outcome = FormatOutcome.Formatted, Result = result };
            }
            catch (Exception)
            {
                return Unsafe();
            }
        }

        private static GateResult Unsafe()
        {
            return new GateResult { Outcome = FormatOutcome.Unsafe };
        }

        private static FormatResult Apply(string source, bool isJs, bool useTabs, bool keepLines, int firstLine, int lastLine)
        {
            return firstLine >= 0
                ? CodeFormatter.FormatLines(source, isJs, useTabs, firstLine, lastLine)
                : CodeFormatter.Format(source, isJs, useTabs, !keepLines);
        }

        private static TokenStream Stream(string code, bool isJs)
        {
            if (isJs)
            {
                return JsTokenizer.Tokenize(code);
            }
            Tokenizer.Tokenize(code, out TokenStream stream);
            return stream;
        }

        private static Program ParseProgram(string code, bool isJs)
        {
            // A stand-in window, so that trial parses can't touch the real window's list of functions.
            var window = (CodeWindow)FormatterServices.GetUninitializedObject(typeof(CodeWindow));
            window.parsedFunctions = new ConcurrentDictionary<string, FunctionNode>();
            TokenStream stream = Stream(code, isJs);
            return isJs ? JsParser.Parse(stream, window) : Parser.Parse(stream, window);
        }

        /// <summary>Tokens as text, ignoring indentation and blank lines. Spaces inside `not in` don't count either.</summary>
        private static List<string> Tokens(string code, bool isJs)
        {
            var tokens = new List<string>();
            foreach (Token token in Stream(code, isJs).ToList())
            {
                if (token.type == TokenType.NEW_LINE)
                {
                    if (tokens.Count == 0 || tokens[tokens.Count - 1] != "\n")
                    {
                        tokens.Add("\n");
                    }
                }
                else
                {
                    tokens.Add((int)token.type + ":" + (token.type == TokenType.IN ? Regex.Replace(token.value, @"\s+", " ") : token.value));
                }
            }
            return tokens;
        }
    }
}
