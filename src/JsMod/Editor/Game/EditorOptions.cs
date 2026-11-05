using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JsMod.Editor.Game
{
    /// <summary>Settings rows and values for the editor upgrade. Each row is a runtime copy of a stock row, like the language option.</summary>
    public static class EditorOptions
    {
        private static readonly List<OptionSO> Rows = new List<OptionSO>();
        private static readonly Dictionary<string, string> StockDefaults = new Dictionary<string, string>();
        private static bool seeding;

        public static bool Enabled => OptionHolder.GetString(EditorBindings.MasterOption, "enabled") != "disabled";

        public static bool IsOn(string option)
        {
            return Enabled && OptionHolder.GetString(option, "disabled") == "enabled";
        }

        public static KeyCombination Current(EditorBindings.Binding binding)
        {
            object value = OptionHolder.GetOption(binding.OptionName);
            if (value == null)
            {
                return EditorBindings.Parse(binding.DefaultKeys);
            }
            return value is KeyCombination combination ? combination : EditorBindings.Parse(null);
        }

        internal static void AddRows(List<OptionSO> options)
        {
            if (Rows.Count == 0)
            {
                BuildRows(options);
            }
            options.AddRange(Rows.Where(row => options.All(o => o.optionName != row.optionName)));
            seeding = true;
            try
            {
                foreach (OptionSO row in Rows)
                {
                    if (OptionHolder.GetOption(row.optionName) == null)
                    {
                        OptionHolder.SetOption(row.optionName, row is KeyBindOptionSO ? (object)EditorBindings.Parse(row.defaultValue) : row.defaultValue);
                    }
                }
            }
            finally
            {
                seeding = false;
            }
        }

        private static void BuildRows(List<OptionSO> options)
        {
            foreach (KeyBindOptionSO stock in options.OfType<KeyBindOptionSO>())
            {
                StockDefaults[stock.optionName] = stock.defaultValue;
            }
            OptionSO toggle = Template<CycleOptionSO>(options, "autosave");
            OptionSO dropdown = Template<DropdownOptionSO>(options, "language");
            OptionSO keyBind = Template<KeyBindOptionSO>(options, "autocomplete");
            Rows.Add(Copy(toggle, EditorBindings.MasterOption, "enabled", EditorBindings.MasterOption + " tooltip"));
            Rows.Add(Copy(toggle, EditorBindings.FormatOnRunOption, "disabled", EditorBindings.FormatOnRunOption + " tooltip"));
            Rows.Add(Copy(toggle, EditorBindings.FormatOnSaveOption, "disabled", EditorBindings.FormatOnSaveOption + " tooltip"));
            var keymap = (DropdownOptionSO)Copy(dropdown, EditorBindings.KeymapOption, EditorBindings.KeymapGame, EditorBindings.KeymapOption + " tooltip");
            keymap.options = new List<string> { EditorBindings.KeymapGame, EditorBindings.KeymapPhpStorm };
            Rows.Add(keymap);
            foreach (EditorBindings.Binding binding in EditorBindings.All)
            {
                Rows.Add(Copy(keyBind, binding.OptionName, binding.DefaultKeys, "editor shortcut tooltip"));
            }
        }

        private static T Template<T>(List<OptionSO> options, string name) where T : OptionSO
        {
            return options.OfType<T>().FirstOrDefault(o => o.optionName == name) ?? throw new InvalidOperationException("No " + typeof(T).Name + " option named '" + name + "' to copy.");
        }

        private static OptionSO Copy(OptionSO template, string name, string defaultValue, string tooltip)
        {
            OptionSO row = UnityEngine.Object.Instantiate(template);
            row.hideFlags = HideFlags.HideAndDontSave;
            row.name = name;
            row.optionName = name;
            row.defaultValue = defaultValue;
            row.tooltip = tooltip;
            return row;
        }

        private static void ApplyPreset()
        {
            bool phpStorm = OptionHolder.GetString(EditorBindings.KeymapOption) == EditorBindings.KeymapPhpStorm;
            foreach ((string option, string game, string php) in EditorBindings.Preset)
            {
                string saved = EditorBindings.SavedKeysOption(option);
                if (phpStorm)
                {
                    object current = OptionHolder.GetOption(option);
                    if (current != null)
                    {
                        OptionHolder.SetOption(saved, current);
                    }
                    OptionHolder.SetOption(option, EditorBindings.Parse(php));
                }
                else
                {
                    StockDefaults.TryGetValue(option, out string stock);
                    OptionHolder.SetOption(option, EditorBindings.GameKeys(OptionHolder.GetOption(saved), stock, game));
                }
            }
        }

        internal static void OnOptionChanged(string item)
        {
            if (seeding)
            {
                return;
            }
            if (item == EditorBindings.KeymapOption)
            {
                if (Enabled)
                {
                    ApplyPreset();
                }
            }
            else if (item == EditorBindings.MasterOption)
            {
                EditorButtons.Refresh();
            }
        }
    }
}
