using System;
using System.Reflection;
using HarmonyLib;
using JsMod.Lang;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace JsMod.Editor.Game
{
    /// <summary>Runs editor actions on a real window. Every edit is one undo step.</summary>
    internal static class EditorActions
    {
        private static readonly FieldInfo UndoState = AccessTools.Field(typeof(CodeWindow), "undoState");
        private static readonly FieldInfo UndoChangeTime = AccessTools.Field(typeof(CodeWindow), "undoChangeTime");
        private static readonly MethodInfo SetToCodeState = AccessTools.Method(typeof(CodeWindow), "SetToCodeState");
        private static readonly MethodInfo ShowError = AccessTools.Method(typeof(CodeWindow), "ShowError");
        private static readonly FieldInfo BreakpointPanel = AccessTools.Field(typeof(CodeWindow), "breakpointPanel");
        private static readonly FieldInfo BreakpointLines = AccessTools.Field(typeof(BreakPointPanel), "breakpointLines");

        private static bool tooltipShown;

        private static bool UseTabs => OptionHolder.GetString("tabs to spaces") == "disabled";

        internal static bool CanEdit(CodeWindow window)
        {
            return (object)window != null && LanguageMode.IsPlayerWindow(window) && !window.CodeInput.readOnly && !window.isExecuting;
        }

        internal static void Perform(EditorAction action, CodeWindow window)
        {
            try
            {
                CodeInputField input = window.CodeInput;
                string text = input.text;
                int start = Math.Min(input.selectionStringAnchorPosition, input.selectionStringFocusPosition);
                int end = Math.Max(input.selectionStringAnchorPosition, input.selectionStringFocusPosition);
                bool isJs = LanguageMode.IsJs(text, window);
                switch (action)
                {
                    case EditorAction.Run:
                        // Shortcuts only reach here while the window isn't running, so this always starts it.
                        window.PressExecuteOrStop();
                        return;
                    case EditorAction.Format:
                        Format(window, false);
                        return;
                    case EditorAction.QuickDoc:
                        ShowWordInfo(window, LineOps.WordAt(text, start, out int wordStart), wordStart);
                        return;
                    case EditorAction.ParameterInfo:
                        ShowWordInfo(window, LineOps.CalleeAt(text, start, out int calleeStart), calleeStart);
                        return;
                    case EditorAction.Duplicate:
                        Apply(window, LineOps.Duplicate(text, start, end));
                        return;
                    case EditorAction.DeleteLine:
                        Apply(window, LineOps.DeleteLines(text, start, end));
                        return;
                    case EditorAction.ToggleComment:
                        Apply(window, LineOps.ToggleComment(text, start, end, isJs));
                        return;
                    case EditorAction.MoveUp:
                        Apply(window, LineOps.MoveLines(text, start, end, true));
                        return;
                    case EditorAction.MoveDown:
                        Apply(window, LineOps.MoveLines(text, start, end, false));
                        return;
                    case EditorAction.NewLineBelow:
                        Apply(window, LineOps.NewLineBelow(text, start, end, isJs, UseTabs));
                        return;
                    case EditorAction.JoinLines:
                        Apply(window, LineOps.JoinLines(text, start, end, isJs));
                        return;
                    case EditorAction.ToggleCase:
                        Apply(window, LineOps.ToggleCase(text, start, end));
                        return;
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogWarning("Editor action " + action + " failed: " + ex.Message);
            }
        }

        /// <summary>What the game's undo and redo keys call, for this window. Those act on the active window, so it becomes active first.</summary>
        internal static void UndoOrRedo(CodeWindow window, bool undo)
        {
            try
            {
                if (!CanEdit(window))
                {
                    return;
                }
                window.workspace.activeWindow = window.GetComponent<Window>();
                if (undo)
                {
                    window.workspace.Undo();
                }
                else
                {
                    window.workspace.Redo();
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogWarning((undo ? "Undo" : "Redo") + " failed: " + ex.Message);
            }
        }

        /// <summary>Silent is for format on run and on save: bad code just stays as it is, and the run reports its own errors.</summary>
        internal static void Format(CodeWindow window, bool silent)
        {
            try
            {
                if (!CanEdit(window))
                {
                    return;
                }
                CodeInputField input = window.CodeInput;
                string text = input.text;
                int anchor = input.selectionStringAnchorPosition;
                int focus = input.selectionStringFocusPosition;
                bool isJs = LanguageMode.IsJs(text, window);
                bool useTabs = UseTabs;
                int firstLine = -1;
                int lastLine = -1;
                if (anchor != focus)
                {
                    firstLine = LineOf(text, Math.Min(anchor, focus));
                    int stop = Math.Max(anchor, focus);
                    lastLine = LineOf(text, stop > 0 && text[stop - 1] == '\n' ? stop - 1 : stop);
                }
                GateResult gate = FormatGate.Run(text, isJs, useTabs, HasBreakpoints(window), firstLine, lastLine);
                switch (gate.Outcome)
                {
                    case FormatOutcome.Formatted:
                        int newAnchor = CodeFormatter.MapPosition(text, gate.Result, anchor);
                        int newFocus = CodeFormatter.MapPosition(text, gate.Result, focus);
                        Apply(window, new EditResult(gate.Result.Text, newAnchor, newFocus));
                        window.Parse();
                        break;
                    case FormatOutcome.DoesNotParse:
                        if (!silent)
                        {
                            // The game's own message, at the error's own position.
                            window.Parse();
                            ShowFailure(window);
                        }
                        break;
                    case FormatOutcome.Unsafe:
                        Plugin.Log.LogWarning("Left " + window.fileName + " unformatted: the result wouldn't have been the same program.");
                        if (!silent)
                        {
                            window.SetErrorMessage(Localizer.Localize("editor format unsafe"), 0, Math.Min(1, text.Length));
                            ShowFailure(window);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogWarning("Formatting failed, so the code was left alone: " + ex.Message);
            }
        }

        /// <summary>The game only shows errors when the input isn't focused, so this lets go of it, as the game's own run key does.</summary>
        private static void ShowFailure(CodeWindow window)
        {
            EventSystem.current?.SetSelectedGameObject(null);
            ShowError.Invoke(window, null);
        }

        private static int LineOf(string text, int position)
        {
            int line = 0;
            for (int i = 0; i < position && i < text.Length; i++)
            {
                if (text[i] == '\n' || text[i] == '\v')
                {
                    line++;
                }
            }
            return line;
        }

        /// <summary>The panel forgets all breakpoints when the line count changes, so with any set, line numbers must stay.</summary>
        private static bool HasBreakpoints(CodeWindow window)
        {
            object panel = BreakpointPanel.GetValue(window);
            return panel != null && BreakpointLines.GetValue(panel) is System.Collections.ICollection lines && lines.Count > 0;
        }

        private static void Apply(CodeWindow window, EditResult edit)
        {
            if (edit == null)
            {
                return;
            }
            CodeInputField input = window.CodeInput;
            string before = input.text;
            if (edit.Text != before)
            {
                // Recent typing gets its own undo step, and so does this edit, so undoing goes back to exactly the text before it.
                if (!string.Equals((string)UndoState.GetValue(window), before))
                {
                    var restore = (Func<object, object>)Delegate.CreateDelegate(typeof(Func<object, object>), window, SetToCodeState);
                    window.workspace.undoHistory.AddChange(restore, UndoState.GetValue(window), window.gameObject);
                    UndoState.SetValue(window, before);
                }
                UndoChangeTime.SetValue(window, -1f);
                input.text = edit.Text;
                window.UpdateTokens();
                UndoChangeTime.SetValue(window, -1f);
            }
            int length = input.text.Length;
            input.stringPosition = Mathf.Clamp(edit.SelectionEnd, 0, length);
            input.selectionStringAnchorPosition = Mathf.Clamp(edit.SelectionStart, 0, length);
            input.selectionStringFocusPosition = Mathf.Clamp(edit.SelectionEnd, 0, length);
        }

        /// <summary>Shows what hovering the word would show, at the word, until the mouse moves away or another key is pressed.</summary>
        private static void ShowWordInfo(CodeWindow window, string word, int wordStart)
        {
            if (string.IsNullOrEmpty(word))
            {
                return;
            }
            TooltipInfo info = TooltipUtils.GetWordTooltip(word, window);
            if (info == null)
            {
                return;
            }
            CodeInputField input = window.CodeInput;
            Vector3 bottomLeft = input.GetCharacterAtSourceIndex(wordStart).bottomLeft;
            Vector3 offset = BlinkManager.GetCodeOffset(input.GetComponent<RectTransform>());
            info.fixedPosition = input.transform.TransformPoint(bottomLeft) + offset;
            info.anchor = TooltipInfo.Anchor.BottomRight;
            info.delay = 0f;
            Tooltip tooltip = window.workspace.tooltip;
            var traverse = Traverse.Create(tooltip);
            CustomStandaloneInputModule module = traverse.Field("inputModule").GetValue<CustomStandaloneInputModule>();
            GameObject hovered = module?.GetPointerData()?.pointerCurrentRaycast.gameObject;
            traverse.Field("info").SetValue(info);
            traverse.Field("tooltipGameObject").SetValue(hovered);
            traverse.Method("SetTooltip", new[] { typeof(GameObject) }).GetValue(hovered);
            tooltipShown = true;
        }

        internal static void DismissWordInfo(CodeInputField input)
        {
            if (!tooltipShown)
            {
                return;
            }
            tooltipShown = false;
            CodeWindow window = input.GetComponentInParent<CodeWindow>();
            if ((object)window != null && (object)window.workspace != null)
            {
                window.workspace.tooltip.CloseTooltip();
            }
        }
    }
}
