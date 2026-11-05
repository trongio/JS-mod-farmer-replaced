# Georgian glossary

Terms used in `translations/KA`. Use these so UI text and docs stay consistent.
Items marked "review" were a judgment call and would benefit from a native
speaker's check.

## Rules

- Prose is Georgian. Everything inside backticks stays in English, byte for byte,
  including comments and string literals.
- Never change placeholders (`{0}`, `{{ x }}`), link targets, tags or keys.
- Georgian attaches case endings with a hyphen after code: `` `True`-ს ``. After a
  `{0}` placeholder, prefer wording that needs no ending, since the value is not
  known in advance.
- Numbers take a singular noun: `200` ტიკი, 5 გოგრა.
- Address the player informally (შენ): აიღე, დააგროვე.

## Programming

| English | ქართული | Note |
|---|---|---|
| code | კოდი | |
| program | პროგრამა | |
| function | ფუნქცია | |
| method | მეთოდი | |
| variable | ცვლადი | |
| local / global variable | ლოკალური / გლობალური ცვლადი | |
| argument | არგუმენტი | |
| default argument | ნაგულისხმევი არგუმენტი | |
| loop | ციკლი | |
| iteration | იტერაცია | |
| to iterate | გადაყოლა | review |
| condition | პირობა | |
| statement | ინსტრუქცია | review |
| expression | გამოსახულება | |
| assignment | მინიჭება | |
| operator / operand | ოპერატორი / ოპერანდი | |
| boolean | ლოგიკური მნიშვნელობა | |
| truthy / falsy | ჭეშმარიტად / მცდარად ჩათვლილი | review |
| number | რიცხვი | |
| string | სტრიქონი | |
| list | სია | |
| dictionary | ლექსიკონი | |
| set | სიმრავლე | |
| tuple | კორტეჟი | |
| sequence | მიმდევრობა | |
| collection | კოლექცია | |
| element | ელემენტი | |
| key / value | გასაღები / მნიშვნელობა | |
| index | ინდექსი | |
| module | მოდული | |
| import | იმპორტი | |
| name | სახელი | |
| reserved keyword | დარეზერვებული სიტყვა | |
| code block | კოდის ბლოკი | |
| indentation | შეწევა | |
| colon | ორწერტილი | |
| comma | მძიმე | |
| bracket | ფრჩხილი | |
| backslash | უკუდახრილი ხაზი | |
| tab | ტაბი | |
| execute / execution | შესრულება | |
| step by step | ნაბიჯ-ნაბიჯ | |
| call (a function) | გამოძახება | |
| return | დაბრუნება | |
| call stack | გამოძახებების სტეკი | |
| recursive | რეკურსიული | |
| debugging | გამართვა | |
| error / warning | შეცდომა / გაფრთხილება | |
| random seed | შემთხვევითობის მარცვალი (seed) | review |
| tick | ტიკი | game unit |
| handle (drone) | დამმუშავებელი (handle) | review |
| file | ფაილი | |
| save (game) | შენახვა / შენახული თამაში | |
| autocomplete | ავტოდასრულება | |
| keybind | კლავიში | |
| HUD | ინტერფეისი (HUD) | |

## Game world

| English | ქართული | Note |
|---|---|---|
| drone | დრონი | |
| farm | ფერმა | |
| tile | უჯრა | |
| grid | ბადე | |
| entity | ობიექტი | review |
| item | ნივთი | |
| inventory | ინვენტარი | |
| unlock (noun / verb) | განბლოკვა | |
| upgrade | გაუმჯობესება | |
| tech tree, research tree | ტექნოლოგიების ხე | |
| cost | ფასი | |
| yield | მოსავლიანობა | |
| harvest | მოკრეფა | |
| plant (noun / verb) | მცენარე / დარგვა | |
| grow time | ზრდის დრო | |
| ground | მიწა | |
| grassland | ბალახნარი | |
| soil | ნიადაგი | |
| till | გადახვნა | |
| dirt | ტალახიანი მიწა | review |
| clay | თიხა | |
| rock | კლდე | |
| block | ბლოკი | |
| hardness | სიმაგრე | |
| stability | მდგრადობა | |
| leaderboard | რეიტინგი | |
| simulation | სიმულაცია | |
| run (leaderboard) | გარბენი | review |
| speedup | აჩქარება | |
| hat | ქუდი | |
| maze | ლაბირინთი | |
| hedge | ცოცხალი ღობე | |
| treasure | განძი | |
| dinosaur | დინოზავრი | |
| underground | მიწისქვეშეთი | |
| polyculture | პოლიკულტურა | |
| companion plant | თანამდგომი მცენარე | review |
| prospecting | საბადოს ძიება | |
| drill | ბურღი | |
| dig | თხრა | |

## Plants and items

| English | ქართული | English | ქართული |
|---|---|---|---|
| grass | ბალახი | hay | თივა |
| bush | ბუჩქი | tree | ხე |
| wood | ხე-ტყე | carrot | სტაფილო |
| rice | ბრინჯი | pumpkin | გოგრა |
| sunflower | მზესუმზირა | petal | ფურცელი |
| cactus | კაქტუსი | bamboo | ბამბუკი |
| mushroom | სოკო | apple | ვაშლი |
| water | წყალი | fertilizer | სასუქი |
| power | ენერგია | gold | ოქრო |
| bone | ძვალი | coal | ქვანახშირი |
| iron | რკინა | quartz | კვარცი |
| dynamite | დინამიტი | weird substance | უცნაური სუბსტანცია |
| limestone | კირქვა | sand | ქვიშა |
| bedrock | ფუძე ქანი | soot | ჭვარტლი |
| basalt | ბაზალტი | loam | თიხნარი |
| perlite | პერლიტი | | |

## Left in English on purpose

- Discord, Python, Vsync: product and technical names.
- Everything in code, such as `Items.Hay`, `North`, `True`.

## Added with the docs

| English | ქართული | Note |
|---|---|---|
| scope | არე | |
| breakpoint | გაჩერების წერტილი | |
| step-by-step mode | ნაბიჯ-ნაბიჯ რეჟიმი | |
| constant | მუდმივი | |
| output | გამოსავალი | |
| loop body | ციკლის ტანი | |
| reference | მიმართვა | |
| unpack | გაშლა | |
| parameter | პარამეტრი | |
| side effect | გვერდითი ეფექტი | |
| external editor | გარე რედაქტორი | |
| backup | სარეზერვო ასლი | |
| floating-point | მცურავმძიმიანი | review |
| remainder | ნაშთი | |
| binary search | ორობითი ძიება | |
| race condition | რბოლის პირობა | review |
| shared memory | საერთო მეხსიერება | |
| bubble sort | ბუშტულა დალაგება | review |
| push (a block) | გადაწევა | review |
| vein | ძარღვი | |
| ore | მადანი | |
| cave-in | ჩამონგრევა | |
| dowsing rod | მაძიებელი წკეპლა | review |
| Dynamite Sweeper | დინამიტის მაძიებელი | pun on Minesweeper, review |
| seedling | ჩითილი | |
| spoiler labels | აჩვენე მინიშნება | translated, the game shows them to the player |

Page titles for code keywords keep the keyword in lowercase English: `if` ინსტრუქცია, `while` ციკლი. Written without backticks in headings and link text.

## JavaScript parser errors

Used by `src/JsMod/Lang/JsErrors.cs`. Code stays in English there too.

| English | ქართული | Note |
|---|---|---|
| arrow function | ისრიანი ფუნქცია | review |
| ternary operator | სამნაწილიანი ოპერატორი | review |
| bitwise operator | ბიტური ოპერატორი | |
| destructuring | დაშლა | same word as unpack |
| assignment | მინიჭება | |
| expression | გამოსახულება | |
| string | სტრიქონი | |
