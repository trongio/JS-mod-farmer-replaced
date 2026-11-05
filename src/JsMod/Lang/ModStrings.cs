using System.Collections.Generic;

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

        /// <summary>The text for a key, or null when the key isn't one of ours.</summary>
        public static string Lookup(string key, string language)
        {
            if (key == null || !Table.TryGetValue(key, out string[] text))
            {
                return null;
            }
            return language == "KA" ? text[1] : text[0];
        }
    }
}
