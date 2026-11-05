namespace JsMod.Lang
{
    /// <summary>Display text for the option. It lives here because the game's language files reject keys they don't know.</summary>
    public static class ModStrings
    {
        public const string TooltipKey = "programming language tooltip";

        /// <summary>The text for a key, or null when the key isn't one of ours. Languages other than Georgian get English.</summary>
        public static string Lookup(string key, string language)
        {
            bool georgian = language == "KA";
            switch (key)
            {
                case LanguageMode.OptionName:
                    return georgian ? "პროგრამირების ენა" : "Programming language";
                case TooltipKey:
                    return georgian
                        ? "აირჩიე, რომელ ენაზე დაწერ კოდს. ფაილი შეიძლება ამ არჩევანს პირველი ხაზით შეცვალოს: // @js ან # @py."
                        : "Choose which language you write code in. A file can override this choice with its first line: // @js or # @py.";
                case LanguageMode.Python:
                case LanguageMode.JavaScript:
                    // The values are names, so they stay the same in every language, even if a language file gains these keys.
                    return key;
                default:
                    return null;
            }
        }
    }
}
