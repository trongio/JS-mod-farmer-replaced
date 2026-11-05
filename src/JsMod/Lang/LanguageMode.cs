using System;
using System.Collections.Generic;

namespace JsMod.Lang
{
    /// <summary>
    /// Decides the language of a piece of code. A marker wins, then the setting, but the setting only reaches real player code
    /// windows. Everything else, including text that comes with no window at all, is Python.
    /// </summary>
    public static class LanguageMode
    {
        public const string OptionName = "programming language";
        public const string Python = "Python";
        public const string JavaScript = "JavaScript";

        /// <summary>Reads the saved setting. Null, so always Python, unless all the patches the setting needs are in place.</summary>
        public static volatile Func<string> ReadSetting;

        // Tokenize and SyntaxColor2 aren't told which window they work for, so the window's patched caller leaves it here.
        // Per thread, because imports parse on the simulation thread.
        [ThreadStatic]
        private static CodeWindow tokenizing;

        [ThreadStatic]
        private static CodeWindow colouring;

        public static bool IsJs(string code, CodeWindow window)
        {
            if (JsMarker.IsJs(code))
            {
                return true;
            }
            return !JsMarker.IsPy(code) && IsPlayerWindow(window) && ReadSetting?.Invoke() == JavaScript;
        }

        public static bool IsJsWhileTokenizing(string code)
        {
            return IsJs(code, tokenizing);
        }

        public static bool IsJsWhileColouring(string code)
        {
            return IsJs(code, colouring);
        }

        /// <summary>
        /// Player windows are the ones a workspace registered as files. Docs examples are CodeWindows too, but a workspace never
        /// registers them. This reads only managed fields and a concurrent dictionary, so it's safe off the main thread.
        /// </summary>
        public static bool IsPlayerWindow(CodeWindow window)
        {
            if ((object)window == null || (object)window.workspace == null)
            {
                return false;
            }
            foreach (KeyValuePair<string, CodeWindow> file in window.workspace.codeWindows)
            {
                if (ReferenceEquals(file.Value, window))
                {
                    return true;
                }
            }
            return false;
        }

        public static CodeWindow EnterTokenizing(CodeWindow window)
        {
            CodeWindow outer = tokenizing;
            tokenizing = window;
            return outer;
        }

        public static void ExitTokenizing(CodeWindow outer)
        {
            tokenizing = outer;
        }

        public static CodeWindow EnterColouring(CodeWindow window)
        {
            CodeWindow outer = colouring;
            colouring = window;
            return outer;
        }

        public static void ExitColouring(CodeWindow outer)
        {
            colouring = outer;
        }
    }
}
