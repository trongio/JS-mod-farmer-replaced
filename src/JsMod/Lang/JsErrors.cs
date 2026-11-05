using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace JsMod.Lang
{
    /// <summary>
    /// Georgian text for the JS parser's errors. The game looks errors up by key and these aren't in its files, so they're
    /// translated here. Code in them stays English. Anything unknown stays English too.
    /// </summary>
    public static class JsErrors
    {
        private static readonly (string english, string georgian)[] Table =
        {
            ("Arrow functions are not supported yet. Declare a named function instead.", "ისრიანი ფუნქციები ჯერ არ არის მხარდაჭერილი. ამის ნაცვლად გამოაცხადე სახელიანი ფუნქცია."),
            ("The ternary operator is not supported yet. Use if/else instead.", "სამნაწილიანი ოპერატორი ჯერ არ არის მხარდაჭერილი. ამის ნაცვლად გამოიყენე if/else."),
            ("The ?? operator is not supported yet. Use || instead.", "?? ოპერატორი ჯერ არ არის მხარდაჭერილი. ამის ნაცვლად გამოიყენე ||."),
            ("Optional chaining (?.) is not supported yet.", "?. ოპერატორი ჯერ არ არის მხარდაჭერილი."),
            ("Spread syntax (...) is not supported yet.", "... სინტაქსი ჯერ არ არის მხარდაჭერილი."),
            ("Bitwise operators are not supported yet.", "ბიტური ოპერატორები ჯერ არ არის მხარდაჭერილი."),
            ("++ can only be used as a statement or in a for loop update.", "++ მხოლოდ ცალკე ინსტრუქციად ან for ციკლის განახლების ნაწილში გამოიყენება."),
            ("-- can only be used as a statement or in a for loop update.", "-- მხოლოდ ცალკე ინსტრუქციად ან for ციკლის განახლების ნაწილში გამოიყენება."),
            ("Missing '}' for the block opened here.", "აქ გახსნილ ბლოკს აკლია '}'."),
            ("Missing '}' for the function body opened here.", "აქ გახსნილ ფუნქციის ტანს აკლია '}'."),
            ("Expected a statement.", "აქ ინსტრუქცია უნდა იყოს."),
            ("A const needs a value", "const-ს მნიშვნელობა სჭირდება"),
            ("Destructuring needs a value", "დაშლას მნიშვნელობა სჭირდება"),
            ("Expected a function name", "აქ ფუნქციის სახელი უნდა იყოს"),
            ("A parameter without a default cannot follow one with a default.", "ნაგულისხმევის გარეშე პარამეტრი ნაგულისხმევიანის შემდეგ ვერ მოვა."),
            ("Expected '{' to start the function body", "ფუნქციის ტანი '{'-ით უნდა დაიწყოს"),
            ("Assignment is not allowed in a condition. Did you mean '=='?", "პირობაში მინიჭება არ შეიძლება. ხომ არ გულისხმობდი '=='-ს?"),
            ("for...in is not supported yet. Use for...of instead", "for...in ჯერ არ არის მხარდაჭერილი. ამის ნაცვლად გამოიყენე for...of"),
            ("for...in is not supported yet. Use for...of instead.", "for...in ჯერ არ არის მხარდაჭერილი. ამის ნაცვლად გამოიყენე for...of."),
            ("'return' can only be used inside a function.", "'return' მხოლოდ ფუნქციის შიგნით გამოიყენება."),
            ("'break' can only be used inside a loop.", "'break' მხოლოდ ციკლის შიგნით გამოიყენება."),
            ("'continue' can only be used inside a loop.", "'continue' მხოლოდ ციკლის შიგნით გამოიყენება."),
            ("'import' can only be used at the top level of a file.", "'import' მხოლოდ ფაილის ზედა დონეზე გამოიყენება."),
            ("'export' can only be used at the top level of a file.", "'export' მხოლოდ ფაილის ზედა დონეზე გამოიყენება."),
            ("Expected a function or variable after 'export'", "'export'-ის შემდეგ ფუნქცია ან ცვლადი უნდა იყოს"),
            ("A file must be imported under its own name, so write: import * as {0} from \"{0}\".", "ფაილი თავისივე სახელით უნდა შემოიტანო, ასე დაწერე: import * as {0} from \"{0}\"."),
            ("Expected 'from'", "აქ 'from' უნდა იყოს"),
            ("Expected a file name in quotes", "აქ ბრჭყალებში ფაილის სახელი უნდა იყოს"),
            ("Expected a file name in quotes, '*' or '{' after 'import'", "'import'-ის შემდეგ ბრჭყალებში ფაილის სახელი, '*' ან '{' უნდა იყოს"),
            ("This file name is empty.", "ფაილის სახელი ცარიელია."),
            ("**= is not supported. Write x = x ** y instead", "**= არ არის მხარდაჭერილი. ამის ნაცვლად დაწერე x = x ** y"),
            ("{0} does nothing without brackets. Did you mean {0}()?", "{0} ფრჩხილების გარეშე არაფერს აკეთებს. ხომ არ გულისხმობდი {0}()-ს?"),
            ("Chained assignment is not supported. Use one assignment per statement", "ჯაჭვური მინიჭება არ არის მხარდაჭერილი. ერთ ინსტრუქციაში ერთი მინიჭება გამოიყენე"),
            ("You can't assign to this expression.", "ამ გამოსახულებას მნიშვნელობას ვერ მიანიჭებ."),
            ("{0} is a const and can't be reassigned.", "{0} არის const და ხელახლა ვერ მიანიჭებ."),
            ("{0} is a reserved name in this game.", "{0} ამ თამაშში დარეზერვებული სახელია."),
            ("Expected a name after '.'", "'.'-ის შემდეგ სახელი უნდა იყოს"),
            ("{0}() takes exactly one value.", "{0}() ზუსტად ერთ მნიშვნელობას იღებს."),
            ("Math.floor() takes exactly one value.", "Math.floor() ზუსტად ერთ მნიშვნელობას იღებს."),
            ("Math.{0} is not available here. Math has abs, floor, max, min and random.", "Math.{0} აქ არ არის. Math-ს აქვს abs, floor, max, min და random."),
            ("The code ended where an expression was expected.", "კოდი იქ დასრულდა, სადაც გამოსახულება უნდა ყოფილიყო."),
            ("Invalid number.", "არასწორი რიცხვი."),
            ("Template interpolation (${...}) is not supported yet. Use + and str() instead.", "${...} ჯერ არ არის მხარდაჭერილი. ამის ნაცვლად გამოიყენე + და str()."),
            ("Function expressions are not supported yet. Declare a named function instead.", "ფუნქცია გამოსახულებად ჯერ არ არის მხარდაჭერილი. ამის ნაცვლად გამოაცხადე სახელიანი ფუნქცია."),
            ("'{0}' is not supported here.", "'{0}' აქ არ არის მხარდაჭერილი."),
            ("The line ended where an expression was expected.", "ხაზი იქ დასრულდა, სადაც გამოსახულება უნდა ყოფილიყო."),
            ("Unexpected '{0}'.", "მოულოდნელი '{0}'."),
            ("Expected '{0}'", "აქ '{0}' უნდა იყოს"),
            ("Expected a name", "აქ სახელი უნდა იყოს"),
            ("Expected ';' or a new line", "აქ ';' ან ახალი ხაზი უნდა იყოს"),
            ("This comment is never closed. Add */ to end it.", "ეს კომენტარი არ იხურება. დასასრულს */ დაუმატე."),
            ("This string is never closed.", "ეს სტრიქონი არ იხურება."),
            ("Invalid number '{0}'.", "არასწორი რიცხვი '{0}'."),
            ("Unexpected character '{0}'.", "მოულოდნელი სიმბოლო '{0}'."),
        };

        private static readonly List<(Regex pattern, string georgian)> Compiled = Table
            .Select(row => (new Regex("^" + Regex.Escape(row.english).Replace(@"\{0}", "(.+)") + "$", RegexOptions.Singleline), row.georgian))
            .ToList();

        private static readonly Regex Found = new Regex(@"^(.*), found '(.*)'\.$", RegexOptions.Singleline);

        public static string Translate(string message, string language)
        {
            if (language != "KA" || message == null)
            {
                return message;
            }
            Match found = Found.Match(message);
            if (found.Success)
            {
                return Lookup(found.Groups[1].Value) + ", მაგრამ აქ არის '" + found.Groups[2].Value + "'.";
            }
            const string codeEnded = ", but the code ended.";
            if (message.EndsWith(codeEnded, System.StringComparison.Ordinal))
            {
                return Lookup(message.Substring(0, message.Length - codeEnded.Length)) + ", მაგრამ კოდი დასრულდა.";
            }
            const string lineEnded = ", but the line ended.";
            if (message.EndsWith(lineEnded, System.StringComparison.Ordinal))
            {
                return Lookup(message.Substring(0, message.Length - lineEnded.Length)) + ", მაგრამ ხაზი დასრულდა.";
            }
            return Lookup(message);
        }

        private static string Lookup(string message)
        {
            foreach ((Regex pattern, string georgian) in Compiled)
            {
                Match match = pattern.Match(message);
                if (match.Success)
                {
                    return match.Groups.Count > 1 ? georgian.Replace("{0}", match.Groups[1].Value) : georgian;
                }
            }
            return message;
        }
    }
}
