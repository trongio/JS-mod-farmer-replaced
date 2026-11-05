using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace JsMod.Lang
{
    /// <summary>
    /// Decides the language of a piece of code. A marker wins, then the setting, but the setting only reaches real player code
    /// windows and docs examples that were swapped to JavaScript. Anything else being run, such as a docs example's setup, is Python.
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

        // Docs example windows showing JavaScript. Weak, because they go when the page changes.
        private static readonly ConditionalWeakTable<CodeWindow, object> JsExamples = new ConditionalWeakTable<CodeWindow, object>();

        public static bool SettingIsJs => ReadSetting?.Invoke() == JavaScript;

        public static bool IsJs(string code, CodeWindow window)
        {
            if (JsMarker.IsJs(code))
            {
                return true;
            }
            return !JsMarker.IsPy(code) && (IsPlayerWindow(window) || IsJsExample(window)) && SettingIsJs;
        }

        public static bool IsJsWhileTokenizing(string code)
        {
            return IsJs(code, tokenizing);
        }

        public static bool IsJsWhileColouring(string code)
        {
            // With no window, this is code in tooltips and docs text, which is shown in the language of the setting.
            CodeWindow window = colouring;
            return (object)window != null ? IsJs(code, window) : JsMarker.IsJs(code) || (!JsMarker.IsPy(code) && SettingIsJs);
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

        /// <summary>Called when a docs example is created. Its code is run as JavaScript only if it's one of ours.</summary>
        public static void MarkJsExample(CodeWindow window)
        {
            JsExamples.Remove(window);
            JsExamples.Add(window, null);
        }

        private static bool IsJsExample(CodeWindow window)
        {
            return (object)window != null && JsExamples.TryGetValue(window, out _);
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
