using System.Collections.Generic;
using System.Linq;

namespace JsMod.Lang
{
    /// <summary>The mod's own display text. It lives here because the game's language files reject keys they don't know.</summary>
    public static class ModStrings
    {
        public const string TooltipKey = "programming language tooltip";

        // English first, Georgian second. Other languages get English. Terms follow translations/glossary.md.
        private static readonly Dictionary<string, string[]> Table = new Dictionary<string, string[]>
        {
            [LanguageMode.OptionName] = new[] { "Programming language", "პროგრამირების ენა" },
            [TooltipKey] = new[]
            {
                "Choose which language you write code in. A file can override this choice with its first line: // @js or # @py.",
                "აირჩიე, რომელ ენაზე დაწერ კოდს. ფაილი შეიძლება ამ არჩევანს პირველი ხაზით შეცვალოს: // @js ან # @py."
            },
            ["editor enhancements"] = new[] { "Editor enhancements", "რედაქტორის გაუმჯობესებები" },
            ["editor enhancements tooltip"] = new[]
            {
                "Adds formatting, shortcuts and buttons to the code editor. Turn it off to get the stock editor.",
                "კოდის რედაქტორს ამატებს ფორმატირებას, მალსახმობებსა და ღილაკებს. გამორთე, თუ სტანდარტული რედაქტორი გინდა."
            },
            ["editor format on run"] = new[] { "Format on run", "ფორმატირება გაშვებისას" },
            ["editor format on run tooltip"] = new[]
            {
                "Formats a file each time you run it. Code with errors is left alone.",
                "ფაილს აფორმატებს ყოველ გაშვებაზე. შეცდომების მქონე კოდი უცვლელი რჩება."
            },
            ["editor format on save"] = new[] { "Format on save", "ფორმატირება შენახვისას" },
            ["editor format on save tooltip"] = new[]
            {
                "Formats the file you're editing when you press the save key. Autosave doesn't format.",
                "რედაქტირებად ფაილს აფორმატებს შენახვის კლავიშზე დაჭერისას. ავტომატური შენახვა არ აფორმატებს."
            },
            ["editor rainbow indents"] = new[] { "Rainbow indents", "ფერადი შეწევები" },
            ["editor rainbow indents tooltip"] = new[]
            {
                "Colours each level of indentation, so you can see where blocks start and end.",
                "შეწევის თითოეულ დონეს სხვა ფერით აჩვენებს, რომ ადვილად ნახო, სად იწყება და სად მთავრდება ბლოკი."
            },
            ["editor keymap"] = new[] { "Shortcut preset", "მალსახმობების ნაკრები" },
            ["editor keymap tooltip"] = new[]
            {
                "PhpStorm sets run, stop, step and delete line to PhpStorm's keys. Game puts the game's keys back.",
                "PhpStorm გაშვების, გაჩერების, ნაბიჯისა და ხაზის წაშლის კლავიშებს PhpStorm-ისაზე აყენებს. Game თამაშის კლავიშებს აბრუნებს."
            },
            ["editor shortcut tooltip"] = new[] { "A code editor shortcut. Click to change it.", "კოდის რედაქტორის მალსახმობი. დააჭირე მის შესაცვლელად." },
            ["editor run code"] = new[] { "Run code", "კოდის გაშვება" },
            ["editor format code"] = new[] { "Format code", "კოდის ფორმატირება" },
            ["editor duplicate line"] = new[] { "Duplicate line", "ხაზის დუბლირება" },
            ["editor delete line"] = new[] { "Delete line", "ხაზის წაშლა" },
            ["editor toggle comment"] = new[] { "Comment or uncomment line", "ხაზის დაკომენტარება ან კომენტარის მოხსნა" },
            ["editor move line up"] = new[] { "Move line up", "ხაზის აწევა" },
            ["editor move line down"] = new[] { "Move line down", "ხაზის ჩამოწევა" },
            ["editor new line below"] = new[] { "New line below", "ახალი ხაზი ქვემოთ" },
            ["editor join lines"] = new[] { "Join lines", "ხაზების გაერთიანება" },
            ["editor toggle case"] = new[] { "Toggle case", "რეგისტრის შეცვლა" },
            ["editor quick documentation"] = new[] { "Quick documentation", "მოკლე დოკუმენტაცია" },
            ["editor parameter info"] = new[] { "Parameter info", "პარამეტრების ინფორმაცია" },
            ["editor button format"] = new[] { "Format this code.", "დააფორმატე ეს კოდი." },
            ["editor button copy"] = new[] { "Copy this code.", "დააკოპირე ეს კოდი." },
            ["editor button undo"] = new[] { "Undo", "გაუქმება" },
            ["editor button redo"] = new[] { "Redo", "გამეორება" },
            ["editor button search"] = new[] { "Search", "ძებნა" },
            ["editor search twice"] = new[] { "Search (press Shift twice)", "ძებნა (ორჯერ დააჭირე Shift-ს)" },
            ["editor undo label"] = new[] { "Undo", "უკან" },
            ["editor redo label"] = new[] { "Redo", "წინ" },
            ["editor button help"] = new[] { "Editor shortcuts.", "რედაქტორის მალსახმობები." },
            ["editor copy label"] = new[] { "Copy", "ასლი" },
            ["editor copied label"] = new[] { "Copied", "დაკოპირდა" },
            ["editor format unsafe"] = new[]
            {
                "This code couldn't be formatted safely, so it was left as it is.",
                "ეს კოდი უსაფრთხოდ ვერ დაფორმატდა, ამიტომ უცვლელი დარჩა."
            },
            ["editor shortcuts title"] = new[] { "Editor shortcuts", "რედაქტორის მალსახმობები" },
            ["editor shortcuts intro"] = new[]
            {
                "You can change these in Options > Controls, or set run, stop and step to PhpStorm's keys with the shortcut preset in Options > General.",
                "მათი შეცვლა შეგიძლია პარამეტრებში, მართვის ჩანართზე. გაშვების, გაჩერებისა და ნაბიჯის კლავიშები PhpStorm-ისაზე შეგიძლია გადაიყვანო მალსახმობების ნაკრებით, პარამეტრების ზოგად ჩანართზე."
            },
            ["editor shortcut unbound"] = new[] { "Not set", "მინიჭებული არ არის" }
        };

        // Tooltips that end with the keys currently bound to these game options.
        private static readonly Dictionary<string, string[]> KeyedTooltips = new Dictionary<string, string[]>
        {
            ["editor button undo"] = new[] { "undo" },
            ["editor button redo"] = new[] { "redo 1", "redo 2" },
            ["editor button search"] = new[] { "search", "=Shift Shift" }
        };

        /// <summary>The keys bound to a game option, or empty when none. Set once the game's options are available.</summary>
        public static System.Func<string, string> KeysFor;

        /// <summary>The text for a key, or null when the key isn't one of ours.</summary>
        public static string Lookup(string key, string language)
        {
            if (key == null || !Table.TryGetValue(key, out string[] text))
            {
                return null;
            }
            string value = language == "KA" ? text[1] : text[0];
            if (KeysFor != null && KeyedTooltips.TryGetValue(key, out string[] options))
            {
                string keys = string.Join(" / ", options.Select(o => o.StartsWith("=") ? o.Substring(1) : KeysFor(o)).Where(k => !string.IsNullOrEmpty(k)));
                return keys.Length == 0 ? value : value + " (" + keys + ")";
            }
            return value;
        }
    }
}
