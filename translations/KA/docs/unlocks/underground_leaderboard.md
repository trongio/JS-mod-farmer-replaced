# მიწისქვეშა რეიტინგი
მიწისქვეშეთში კიდევ უფრო დიდი გამოწვევები გელოდება. მზად ხარ, შენი სამთო და მეურნეობის პროგრამების ოპტიმიზაციისთვის?
ამ განბლოკვით შეგიძლია სიღრმეში ყველაზე ეფექტური მეთოდებისთვის იასპარეზო.

მიწისქვეშა რეიტინგის გარბენის დასაწყებად გამოიძახე `leaderboard_run(leaderboard, filename, speedup)`.
ეს იწყებს `simulate()`-ის მსგავს [სიმულაციას](docs/unlocks/simulation.md), ოღონდ საწყისი პირობები ფიქსირებულია. რეიტინგის თითოეულ კატეგორიას საკუთარი საწყისი და წარმატების პირობები აქვს.

რეიტინგის გარბენი წარმატებულია, თუ სიმულაციის დასრულებისას წარმატების პირობა `True`-ა.

სიმულაცია მიზნის მიღწევისას თავისით არ დასრულდება. შენ თვითონ უნდა უზრუნველყო, რომ პროგრამა დასრულდეს.
თუ გარბენი წარმატებულია, შენი დრო რეიტინგში დაემატება.

შედეგების გაფანტვის შესამცირებლად ყველა გარბენმა სიმულირებული დროის მინიმუმ 2 საათი უნდა მოიცვას. სიმულაცია შეგიძლია დააჩქარო, ამიტომ რეალურ დროში ამდენს არ წაიღებს. თუ გარბენი უფრო ადრე დასრულდება, ის განმეორდება, სანამ ჯამური სიმულირებული დრო 2 საათს არ მიაღწევს. შემდეგ ყველა გარბენის საშუალო დრო აიტვირთება შენს შედეგად.

აი, კონფიგურაციის მაგალითი, რომლითაც `Leaderboards.Rice_Single` რეიტინგში მოხვდები.

![|x400](UndergroundLeaderboardSetup)

## მიწისქვეშეთის უსწრაფესი გადატვირთვა
მიწისქვეშეთის გადატვირთვა ყველაზე პრესტიჟული კატეგორიაა. ამ კატეგორიაში სრულად ავტომატიზებ სამთო განბლოკვების ხეს: იწყებ ფერმის ერთი ნაკვეთით და ამთავრებ მაშინ, როცა `Unlocks.Underground_Leaderboard`-ს ისევ განბლოკავ.

ყველაფრის განბლოკვა საჭირო არ არის, უბრალოდ შეეცადე `Unlocks.Underground_Leaderboard` რაც შეიძლება სწრაფად განბლოკო.

გახსოვდეს, რომ `num_unlocked(unlock) > 0`-ით შეგიძლია შეამოწმო, განბლოკილია თუ არა რამე. ასევე შეგიძლია განბლოკვებზე გამოიყენო `get_cost()`, რომ ნახო მათი ფასი და საჭირო ნივთები ავტომატურად მოიპოვო.

`unlock()`-ს არ აინტერესებს ტექნოლოგიების ხის დამოკიდებულებები. მაგალითად, შესაძლებელია `Unlocks.Fertilizer`-ის განბლოკვა `Unlocks.Water`-მდე.

ფუნქციის გამოძახება:
`leaderboard_run(Leaderboards.Fastest_Underground_Reset, filename, speedup)`

ეკვივალენტური სიმულაცია:
`unlocks = {}
items = {}
globals = {}
#a negative seed value means a random seed
seed = -1
simulate(filename, unlocks, items, globals, seed, speedup)`

წარმატების პირობა:
`num_unlocked(Unlocks.Underground_Leaderboard) > 0`

## მადნები
ეს რეიტინგი ამოწმებს მადნების, განსაკუთრებით კვარცის, მოპოვების უნარს. ის ცვლის მიწის გენერაციას ისე, რომ მიწის ფენები ამ თანმიმდევრობით მოგცემს ქვანახშირს, შემდეგ რკინას, შემდეგ კვარცს.

იწყებ ყველაფერი განბლოკილით და რაც შეიძლება სწრაფად უნდა დააგროვო `20000` კვარცი.

ფუნქციის გამოძახება:
`leaderboard_run(Leaderboards.Ores, filename, speedup)`

ეკვივალენტური სიმულაცია:
`unlocks = Unlocks
items = {Items.Power: 1000000000}
globals = {}
seed = -1
simulate(filename, unlocks, items, globals, seed, speedup)`

წარმატების პირობა:
`num_items(Items.Quartz) >= 20000`

## განძის რუკა
ეს რეიტინგი იწყება ყველაფერი განბლოკილით და ბევრი ენერგიით. ის ცვლის მიწის გენერაციას ისე, რომ ბევრი განძის რუკა გაჩნდეს.

ის 8x8 ზომის სამყაროში მიმდინარეობს, ერთი დრონით. მიზანია, რაც შეიძლება სწრაფად დააგროვო `115000` ოქრო.

ფუნქციის გამოძახება:
`leaderboard_run(Leaderboards.Treasure_Map, filename, speedup)`

ეკვივალენტური სიმულაცია:
`unlocks = Unlocks
items = {Items.Power: 1000000000}
globals = {}
seed = -1
simulate(filename, unlocks, items, globals, seed, speedup)`

წარმატების პირობა:
`num_items(Items.Gold) >= 115000`

## რესურსების რეიტინგები
ეს რეიტინგები კონკრეტული რესურსების რაც შეიძლება სწრაფ მოპოვებაზეა. იწყებ ყველაფერი განბლოკილით და ბევრი ენერგიით. ერთადერთი გამონაკლისია პირამიდების რეიტინგი, სადაც ენერგია თავად უნდა აწარმოო პირამიდების აღდგენით.

მიზანია რესურსების განსაზღვრული რაოდენობის დაგროვება.

როგორც ყოველთვის, უნდა დარწმუნდე, რომ პროგრამა მიზნის მიღწევისას სრულდება. გარბენი არ მთავრდება, სანამ პროგრამა არ დასრულდება, მაშინაც კი, თუ მიზანი მიღწეულია.

ეს რეიტინგები ცვლის მიწის გენერაციას ისე, რომ სასურველი რესურსი კვლავ და კვლავ გვხვდებოდეს. ეს ნიშნავს, რომ `clear()` საჭირო არ არის. მაგალითად, ბრინჯის რეიტინგი მხოლოდ თიხის ფენებისგან შედგება.

### `Leaderboards.Rice`
`leaderboard_run(Leaderboards.Rice, filename, speedup)`
წარმატების პირობა: `num_items(Items.Rice) >= 2000000`

### `Leaderboards.Bamboo`
გაითვალისწინე, რომ ბამბუკის რეიტინგში თხრა აკრძალულია.

`leaderboard_run(Leaderboards.Bamboo, filename, speedup)`
წარმატების პირობა: `num_items(Items.Bamboo) >= 500000000`

### `Leaderboards.Dynamite`
`leaderboard_run(Leaderboards.Dynamite, filename, speedup)`
წარმატების პირობა: `num_items(Items.Dynamite) >= 10000000`

### `Leaderboards.Mushrooms`
`leaderboard_run(Leaderboards.Mushrooms, filename, speedup)`
წარმატების პირობა: `num_items(Items.Mushroom) >= 240000`

### `Leaderboards.Pyramids`
`leaderboard_run(Leaderboards.Pyramids, filename, speedup)`
წარმატების პირობა: `num_items(Items.Power) >= 150000`

## ერთი დრონის მიწისქვეშა რეიტინგები
ასევე არსებობს რეიტინგები ერთი დრონით მოპოვებისთვის. გაქვს მხოლოდ ერთი დრონი და 8x8 ფერმა და რესურსების გარკვეული რაოდენობა რაც შეიძლება სწრაფად უნდა მოიპოვო.

### `Leaderboards.Rice_Single`
`leaderboard_run(Leaderboards.Rice_Single, filename, speedup)`
წარმატების პირობა: `num_items(Items.Rice) >= 100000`

### `Leaderboards.Ores_Single`
`leaderboard_run(Leaderboards.Ores_Single, filename, speedup)`
წარმატების პირობა: `num_items(Items.Quartz) >= 4000`

### `Leaderboards.Bamboo_Single`
`leaderboard_run(Leaderboards.Bamboo_Single, filename, speedup)`
წარმატების პირობა: `num_items(Items.Bamboo) >= 25000000`

### `Leaderboards.Dynamite_Single`
`leaderboard_run(Leaderboards.Dynamite_Single, filename, speedup)`
წარმატების პირობა: `num_items(Items.Dynamite) >= 100000`

### `Leaderboards.Mushrooms_Single`
`leaderboard_run(Leaderboards.Mushrooms_Single, filename, speedup)`
წარმატების პირობა: `num_items(Items.Mushroom) >= 30000`

### `Leaderboards.Pyramids_Single`
`leaderboard_run(Leaderboards.Pyramids_Single, filename, speedup)`
წარმატების პირობა: `num_items(Items.Power) >= 30000`

