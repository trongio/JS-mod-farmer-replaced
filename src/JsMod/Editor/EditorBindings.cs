using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JsMod.Lang;
using UnityEngine;

namespace JsMod.Editor
{
    public enum EditorAction
    {
        Run,
        Format,
        Duplicate,
        DeleteLine,
        ToggleComment,
        MoveUp,
        MoveDown,
        NewLineBelow,
        JoinLines,
        ToggleCase,
        QuickDoc,
        ParameterInfo
    }

    /// <summary>
    /// A key arrives twice: as a key, and then in the same frame as its character, which for Ctrl combinations can be a control
    /// character such as a newline. So after a shortcut, characters are dropped until the next key or the next frame.
    /// </summary>
    public sealed class CharacterSwallower
    {
        private int frame = -1;

        public void OnBindingFired(int currentFrame)
        {
            frame = currentFrame;
        }

        public void OnKey()
        {
            frame = -1;
        }

        public bool ShouldSwallow(char character, int currentFrame)
        {
            return character != '\0' && currentFrame == frame;
        }
    }

    /// <summary>The editor's shortcuts, modelled on PhpStorm's default keymap. Option names are also the keys of their labels.</summary>
    public static class EditorBindings
    {
        public sealed class Binding
        {
            public EditorAction Action;
            public string OptionName;
            public string DefaultKeys;
        }

        public const string MasterOption = "editor enhancements";
        public const string FormatOnRunOption = "editor format on run";
        public const string FormatOnSaveOption = "editor format on save";
        public const string RainbowOption = "editor rainbow indents";
        public const string KeymapOption = "editor keymap";
        public const string KeymapGame = "Game";
        public const string KeymapPhpStorm = "PhpStorm";
        public const string ShortcutsDoc = "docs/editor_shortcuts.md";

        public static readonly IReadOnlyList<Binding> All = new[]
        {
            new Binding { Action = EditorAction.Run, OptionName = "editor run code", DefaultKeys = "Ctrl Return" },
            new Binding { Action = EditorAction.Format, OptionName = "editor format code", DefaultKeys = "Ctrl Alt L" },
            new Binding { Action = EditorAction.Duplicate, OptionName = "editor duplicate line", DefaultKeys = "Ctrl D" },
            // PhpStorm's Ctrl Y is the game's second redo key, so this starts unbound. The PhpStorm preset swaps them.
            new Binding { Action = EditorAction.DeleteLine, OptionName = DeleteLineOption, DefaultKeys = "" },
            new Binding { Action = EditorAction.ToggleComment, OptionName = "editor toggle comment", DefaultKeys = "Ctrl Slash" },
            new Binding { Action = EditorAction.MoveUp, OptionName = "editor move line up", DefaultKeys = "Alt Shift UpArrow" },
            new Binding { Action = EditorAction.MoveDown, OptionName = "editor move line down", DefaultKeys = "Alt Shift DownArrow" },
            new Binding { Action = EditorAction.NewLineBelow, OptionName = "editor new line below", DefaultKeys = "Shift Return" },
            new Binding { Action = EditorAction.JoinLines, OptionName = "editor join lines", DefaultKeys = "Ctrl Shift J" },
            new Binding { Action = EditorAction.ToggleCase, OptionName = "editor toggle case", DefaultKeys = "Ctrl Shift U" },
            new Binding { Action = EditorAction.QuickDoc, OptionName = "editor quick documentation", DefaultKeys = "Ctrl Q" },
            new Binding { Action = EditorAction.ParameterInfo, OptionName = "editor parameter info", DefaultKeys = "Ctrl P" }
        };

        public const string DeleteLineOption = "editor delete line";

        /// <summary>The stock options and our own that a preset sets, with the keys for each preset.</summary>
        public static readonly IReadOnlyList<(string option, string game, string phpStorm)> Preset = new[]
        {
            ("start execution", "F5", "Shift F10"),
            ("stop execution", "Shift F5", "Ctrl F2"),
            ("next step", "F11", "F8"),
            ("redo 2", "Ctrl Y", ""),
            (DeleteLineOption, "", "Ctrl Y")
        };

        /// <summary>Where a key is kept while the PhpStorm preset replaces it.</summary>
        public static string SavedKeysOption(string option)
        {
            return "editor keymap saved " + option;
        }

        /// <summary>The key the Game preset puts back: what the player had, else the game's default, else our own table's.</summary>
        public static KeyCombination GameKeys(object saved, string stockDefault, string tableDefault)
        {
            if (saved is KeyCombination combination)
            {
                return combination;
            }
            // An unbound key reads back from options.txt as an empty string.
            return Parse(saved as string ?? stockDefault ?? tableDefault);
        }

        public static KeyCombination Parse(string keys)
        {
            if (string.IsNullOrEmpty(keys) || !KeyCombination.TryParse(keys, out KeyCombination combination))
            {
                return new KeyCombination(KeyCode.None, false, false, false);
            }
            return combination;
        }

        public static bool Matches(KeyCombination combination, KeyCode key, bool ctrl, bool alt, bool shift)
        {
            return combination.key != KeyCode.None && combination.key == key && combination.ctrl == ctrl && combination.alt == alt && combination.shift == shift;
        }

        public static Binding Find(Func<Binding, KeyCombination> current, KeyCode key, bool ctrl, bool alt, bool shift)
        {
            return key == KeyCode.None ? null : All.FirstOrDefault(b => Matches(current(b), key, ctrl, alt, shift));
        }

        /// <summary>The in-game page listing the shortcuts as they're currently bound.</summary>
        public static string ShortcutsPage(string language, Func<Binding, string> keys, Func<string, string> stockKeys, Func<string, string> label)
        {
            var page = new StringBuilder();
            page.Append("# ").Append(ModStrings.Lookup("editor shortcuts title", language)).Append("\n\n");
            page.Append(ModStrings.Lookup("editor shortcuts intro", language)).Append("\n\n");
            string unbound = ModStrings.Lookup("editor shortcut unbound", language);
            foreach (Binding binding in All)
            {
                string bound = keys(binding);
                page.Append(string.IsNullOrEmpty(bound) ? unbound : "`" + bound + "`").Append(": ").Append(ModStrings.Lookup(binding.OptionName, language)).Append('\n');
            }
            page.Append('\n');
            foreach (string option in new[] { "start execution", "stop execution", "pause execution", "next step", "search", "autocomplete", "indent selection", "unindent selection", "undo", "redo 1", "save" })
            {
                string bound = stockKeys(option);
                page.Append(string.IsNullOrEmpty(bound) ? unbound : "`" + bound + "`").Append(": ").Append(label(option)).Append('\n');
            }
            page.Append("`Shift Shift`: ").Append(ModStrings.Lookup("editor search twice", language)).Append("\n");
            return page.ToString();
        }
    }
}
