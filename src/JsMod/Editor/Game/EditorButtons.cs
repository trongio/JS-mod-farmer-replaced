using System;
using System.Collections;
using HarmonyLib;
using JsMod.Lang;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace JsMod.Editor.Game
{
    /// <summary>Marks a button this mod added, so that turning the feature off can find it.</summary>
    public sealed class EditorButtonTag : MonoBehaviour
    {
        public CodeWindow Widened;
        public float ExtraWidth;
        public RectTransform MovedName;
        public float NameShift;
    }

    /// <summary>
    /// Buttons copied from the game's own, and labelled with text. Format follows run and step on the left. On the right,
    /// positions come from the prefabs: header buttons sit 50 apart from the top right corner, and the two rightmost are the game's close and minimize.
    /// </summary>
    internal static class EditorButtons
    {
        private const float WidthForHeaderButtons = 190f;

        internal static void AddToWindow(CodeWindow window)
        {
            if (window.GetComponentInChildren<EditorButtonTag>(true) != null)
            {
                return;
            }
            Transform source = window.transform.Find("MinimizeButton");
            if (source == null)
            {
                throw new InvalidOperationException("The code window has no MinimizeButton to copy.");
            }
            float leftWidth = AddLeftButtons(window, source);
            AddButton(source, window.transform, "?", "editor button help", new Vector2(-116f, source.GetComponent<RectTransform>().anchoredPosition.y), 35f, () => window.workspace.AddNewDocsWindow(EditorBindings.ShortcutsDoc));
            ColoredButton copy = null;
            copy = AddButton(source, window.transform, Localizer.Localize("editor copy label"), "editor button copy", new Vector2(-176f, source.GetComponent<RectTransform>().anchoredPosition.y), 65f, () => Copy(window, copy));
            var tag = copy.GetComponent<EditorButtonTag>();
            var minWidth = Traverse.Create(window).Field("minWidth");
            minWidth.SetValue(minWidth.GetValue<float>() + WidthForHeaderButtons + leftWidth);
            tag.Widened = window;
            tag.ExtraWidth = WidthForHeaderButtons + leftWidth;
        }

        /// <summary>Format, undo and redo follow the run and step buttons at the same spacing, and the file name moves over to make room. Returns the extra width past format.</summary>
        private static float AddLeftButtons(CodeWindow window, Transform source)
        {
            ModStrings.KeysFor = option => OptionHolder.GetKeyCombination(option).ToString().Trim();
            var traverse = Traverse.Create(window);
            var run = (RectTransform)traverse.Field("executeButton").GetValue<ColoredButton>().transform;
            var step = (RectTransform)traverse.Field("stepByStepButton").GetValue<ColoredButton>().transform;
            float gap = step.anchoredPosition.x - run.anchoredPosition.x;
            ColoredButton format = AddBesideStep(source, window, step, gap, "{ }", "editor button format", () => EditorActions.Format(window, false));
            AddBesideStep(source, window, step, gap * 2f, Localizer.Localize("editor undo label"), "editor button undo", () => EditorActions.UndoOrRedo(window, true));
            AddBesideStep(source, window, step, gap * 3f, Localizer.Localize("editor redo label"), "editor button redo", () => EditorActions.UndoOrRedo(window, false));
            var name = (RectTransform)traverse.Field("fileNameText").GetValue<TMP_InputField>().transform;
            name.offsetMin += new Vector2(gap * 3f, 0f);
            var tag = format.GetComponent<EditorButtonTag>();
            tag.MovedName = name;
            tag.NameShift = gap * 3f;
            return gap * 2f;
        }

        private static ColoredButton AddBesideStep(Transform source, CodeWindow window, RectTransform step, float offset, string label, string tooltip, Action clicked)
        {
            ColoredButton button = AddButton(source, window.transform, label, tooltip, Vector2.zero, step.sizeDelta.x, clicked);
            var rect = (RectTransform)button.transform;
            rect.anchorMin = step.anchorMin;
            rect.anchorMax = step.anchorMax;
            rect.pivot = step.pivot;
            rect.sizeDelta = step.sizeDelta;
            rect.anchoredPosition = step.anchoredPosition + new Vector2(offset, 0f);
            return button;
        }

        internal static void AddToExample(CodeWindow codeWindow, GameObject executeButton)
        {
            if (codeWindow.GetComponentInChildren<EditorButtonTag>(true) != null)
            {
                return;
            }
            ColoredButton copy = null;
            copy = AddButton(executeButton.transform, codeWindow.transform, Localizer.Localize("editor copy label"), "editor button copy", new Vector2(-8f, -6f), 70f, () => Copy(codeWindow, copy));
            var rect = (RectTransform)copy.transform;
            rect.anchorMin = Vector2.one;
            rect.anchorMax = Vector2.one;
            rect.pivot = Vector2.one;
            rect.sizeDelta = new Vector2(70f, 34f);
            rect.anchoredPosition = new Vector2(-8f, -6f);
            LayoutElement layout = copy.GetComponent<LayoutElement>() ?? copy.gameObject.AddComponent<LayoutElement>();
            layout.ignoreLayout = true;
        }

        private static ColoredButton AddButton(Transform source, Transform parent, string label, string tooltip, Vector2 position, float width, Action clicked)
        {
            GameObject clone = UnityEngine.Object.Instantiate(source.gameObject, parent);
            clone.name = "JsModEditorButton";
            clone.AddComponent<EditorButtonTag>();
            clone.SetActive(true);
            Button button = clone.GetComponent<Button>();
            // Replacing the event also drops the listeners saved in the prefab, which RemoveAllListeners keeps.
            button.onClick = new Button.ButtonClickedEvent();
            button.onClick.AddListener(() => clicked());
            ColoredButton colored = clone.GetComponent<ColoredButton>();
            // The source's hold action would otherwise still fire on a long press.
            colored.onHeld = new UnityEvent();
            Traverse.Create(colored).Field("holdDuration").SetValue(0f);
            colored.SetTooltipDescription(tooltip);
            Transform icon = clone.transform.Find("Image");
            if (icon != null)
            {
                icon.gameObject.SetActive(false);
            }
            TextMeshProUGUI text = clone.GetComponentInChildren<TextMeshProUGUI>(true);
            if (text != null)
            {
                // It would otherwise treat our text as a language key and replace it.
                TMPTextLocalizer localizer = text.GetComponent<TMPTextLocalizer>();
                if (localizer != null)
                {
                    UnityEngine.Object.DestroyImmediate(localizer);
                }
                text.gameObject.SetActive(true);
                text.textWrappingMode = TextWrappingModes.NoWrap;
                text.enableAutoSizing = true;
                text.fontSizeMin = 10f;
                text.fontSizeMax = 24f;
                text.alignment = TextAlignmentOptions.Center;
                var textRect = text.rectTransform;
                textRect.anchorMin = Vector2.zero;
                textRect.anchorMax = Vector2.one;
                textRect.offsetMin = new Vector2(2f, 0f);
                textRect.offsetMax = new Vector2(-2f, 0f);
                text.text = label;
            }
            var rect = (RectTransform)clone.transform;
            rect.anchoredPosition = position;
            rect.sizeDelta = new Vector2(width, rect.sizeDelta.y);
            return colored;
        }

        private static void Copy(CodeWindow window, ColoredButton button)
        {
            try
            {
                // The input holds plain source. Colours live in a separate text mesh, and a docs example's setup was split off before it got here.
                GUIUtility.systemCopyBuffer = window.CodeInput.text;
                button.Text = Localizer.Localize("editor copied label");
                button.StartCoroutine(RestoreLabel(button));
            }
            catch (Exception ex)
            {
                Plugin.Log.LogWarning("Copying failed: " + ex.Message);
            }
        }

        private static IEnumerator RestoreLabel(ColoredButton button)
        {
            yield return new WaitForSecondsRealtime(1f);
            if (button != null)
            {
                button.Text = Localizer.Localize("editor copy label");
            }
        }

        /// <summary>Follows the master switch without a restart.</summary>
        internal static void Refresh()
        {
            try
            {
                foreach (EditorButtonTag tag in UnityEngine.Object.FindObjectsByType<EditorButtonTag>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                {
                    if (tag.Widened != null)
                    {
                        var minWidth = Traverse.Create(tag.Widened).Field("minWidth");
                        minWidth.SetValue(minWidth.GetValue<float>() - tag.ExtraWidth);
                    }
                    if (tag.MovedName != null)
                    {
                        tag.MovedName.offsetMin -= new Vector2(tag.NameShift, 0f);
                    }
                    // Now, not at the end of the frame, so that adding them back below sees that they're gone.
                    UnityEngine.Object.DestroyImmediate(tag.gameObject);
                }
                if (!EditorOptions.Enabled)
                {
                    return;
                }
                MainSim sim = MainSim.Inst;
                if (sim != null && sim.workspace != null)
                {
                    foreach (CodeWindow window in sim.workspace.codeWindows.Values)
                    {
                        AddToWindow(window);
                    }
                }
                foreach (CodeExample example in UnityEngine.Object.FindObjectsByType<CodeExample>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                {
                    var traverse = Traverse.Create(example);
                    AddToExample(traverse.Field("codeWindow").GetValue<CodeWindow>(), traverse.Field("executeButton").GetValue<GameObject>());
                }
            }
            catch (Exception ex)
            {
                Plugin.Log.LogWarning("Could not update the editor buttons: " + ex.Message);
            }
        }
    }
}
