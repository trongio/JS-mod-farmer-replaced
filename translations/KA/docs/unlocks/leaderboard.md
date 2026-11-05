[<- სიმულაცია](docs/unlocks/simulation.md)
---
# რეიტინგი
თუ აქამდე მოაღწიე, ბევრ სირთულეს გაუმკლავდი. მაგრამ ეფექტურად კი გადაჭერი ისინი? 
სხვადასხვა რეიტინგში სხვა მოთამაშეებს შეეჯიბრები, ვისი მეურნეობის მეთოდი იქნება ყველაზე ეფექტური.

რეიტინგის გარბენს `leaderboard_run(leaderboard, filename, speedup)`-ის გამოძახებით იწყებ.
ეს იწყებს [სიმულაციას](docs/unlocks/simulation.md), რომელიც `simulate()`-ს ჰგავს, ოღონდ საწყისი პირობები ფიქსირებულია. რეიტინგის თითოეულ კატეგორიას საკუთარი საწყისი და წარმატების პირობები აქვს.

გარბენი წარმატებულია, თუ სიმულაციის დასრულებისას წარმატების პირობა `True`-ა. 

მიზნის მიღწევისას სიმულაცია ავტომატურად არ დასრულდება. პროგრამის დასრულებაზე შენ თვითონ უნდა იზრუნო.
თუ გარბენი წარმატებულია, შენი დრო რეიტინგში დაემატება.

შემთხვევითი გადახრების შესამცირებლად ყველა გარბენმა სიმულაციური დროის მინიმუმ 2 საათი უნდა მოიცვას. სიმულაციის აჩქარება შეგიძლია, ამიტომ რეალურ დროში ამდენს არ წაიღებს. თუ გარბენი უფრო ადრე დასრულდა, ის მანამდე განმეორდება, სანამ სიმულაციური დროის ჯამი 2 საათს არ მიაღწევს. შემდეგ ყველა გარბენის საშუალო დრო შენს შედეგად აიტვირთება.

აი მაგალითი, რომლითაც თივის რეიტინგში მოხვდები.
![|x400](LeaderboardSetup)

## უსწრაფესი გადატვირთვა
უსწრაფესი გადატვირთვა ყველაზე პრესტიჟული კატეგორიაა. აქ თამაშს მთლიანად ავტომატიზებ: იწყებ ფერმის ერთი უჯრით და ამთავრებ მაშინ, როცა რეიტინგს ხელახლა განბლოკავ.

ყველაფრის განბლოკვა საჭირო არ არის, უბრალოდ ეცადე, `Unlocks.Leaderboard` რაც შეიძლება სწრაფად განბლოკო.

გახსოვდეს, რომ `num_unlocked(unlock) > 0`-ით შეგიძლია შეამოწმო, განბლოკილია თუ არა რამე. განბლოკვებზე `get_cost()`-ის გამოყენებით მათ ფასსაც გაიგებ, ასე საჭირო ნივთებს ავტომატურად მოაგროვებ.

`unlock()`-ს ტექნოლოგიების ხის დამოკიდებულებები არ აინტერესებს. მაგალითად, `Unlocks.Fertilizer`-ს შეგიძლია `Unlocks.Water`-მდე განბლოკო.

ფუნქციის გამოძახება:
`leaderboard_run(Leaderboards.Fastest_Reset, filename, speedup)`

ექვივალენტური სიმულაცია:
`unlocks = {}
items = {}
globals = {}
#a negative seed value means a random seed
seed = -1
simulate(filename, unlocks, items, globals, seed, speedup)`

წარმატების პირობა:
`num_unlocked(Unlocks.Leaderboard) > 0`

## ლაბირინთი
ყველაფერი უკვე განბლოკილია, შენ კი `9863168` ოქრო რაც შეიძლება სწრაფად უნდა მოაგროვო. ზუსტად ამდენ ოქროს მიიღებ, თუ ერთ 32x32 ლაბირინთს `300`-ჯერ გამოიყენებ.

ფუნქციის გამოძახება:
`leaderboard_run(Leaderboards.Maze, filename, speedup)`

ექვივალენტური სიმულაცია:
`unlocks = Unlocks
items = {Items.Weird_Substance : 1000000000, Items.Power: 1000000000}
globals = {}
seed = -1
simulate(filename, unlocks, items, globals, seed, speedup)`

წარმატების პირობა:
`num_items(Items.Gold) >= 9863168`

## დინოზავრი
ყველაფერი უკვე განბლოკილია, შენ კი `33488928` ძვალი რაც შეიძლება სწრაფად უნდა მოაგროვო. ზუსტად ამდენ ძვალს მიიღებ, თუ 32x32 ფართობს დინოზავრის კუდით შეავსებ.

ფუნქციის გამოძახება:
`leaderboard_run(Leaderboards.Dinosaur, filename, speedup)`

ექვივალენტური სიმულაცია:
`unlocks = Unlocks
items = {Items.Cactus : 1000000000, Items.Power: 1000000000}
globals = {}
seed = -1
simulate(filename, unlocks, items, globals, seed, speedup)`

წარმატების პირობა:
`num_items(Items.Bone) >= 33488928`

## სხვა რესურსების რეიტინგები
თითოეულ მცენარეს საკუთარი რეიტინგი აქვს: ვინ მოიყვანს მას ყველაზე სწრაფად. იწყებ ყველა განბლოკვით, მცენარის მოსაყვანად საჭირო რესურსებით და დიდი რაოდენობით ენერგიით. მიზანია, ამ მცენარისგან მიღებული რესურსი განსაზღვრული რაოდენობით მოაგროვო.

როგორც ყოველთვის, უნდა იზრუნო, რომ მიზნის მიღწევისას პროგრამა დასრულდეს. სანამ პროგრამა არ დასრულდება, გარბენი დასრულებულად არ ჩაითვლება, მიზანი მიღწეული რომც იყოს.

### `Leaderboards.Cactus`
`leaderboard_run(Leaderboards.Cactus, filename, speedup)`
წარმატების პირობა: `num_items(Items.Cactus) >= 33554432`

### `Leaderboards.Sunflowers`
`leaderboard_run(Leaderboards.Sunflowers, filename, speedup)`
წარმატების პირობა: `num_items(Items.Power) >= 100000`

### `Leaderboards.Pumpkins`
`leaderboard_run(Leaderboards.Pumpkins, filename, speedup)`
წარმატების პირობა: `num_items(Items.Pumpkin) >= 200000000`

### `Leaderboards.Wood`
`leaderboard_run(Leaderboards.Wood, filename, speedup)`
წარმატების პირობა: `num_items(Items.Wood) >= 10000000000`

### `Leaderboards.Carrots`
`leaderboard_run(Leaderboards.Carrots, filename, speedup)`
წარმატების პირობა: `num_items(Items.Carrot) >= 2000000000`

### `Leaderboards.Hay`
`leaderboard_run(Leaderboards.Hay, filename, speedup)`
წარმატების პირობა: `num_items(Items.Hay) >= 2000000000`

## ერთი დრონის რეიტინგები
არის რეიტინგებიც მხოლოდ ერთი დრონით მეურნეობისთვის. გეძლევა მხოლოდ ერთი დრონი და 8x8 ფერმა, და რესურსების განსაზღვრული რაოდენობა რაც შეიძლება სწრაფად უნდა მოაგროვო.

### `Leaderboards.Maze_Single`
`leaderboard_run(Leaderboards.Maze_Single, filename, speedup)`
წარმატების პირობა: `num_items(Items.Gold) >= 616448`

### `Leaderboards.Cactus_Single`
`leaderboard_run(Leaderboards.Cactus_Single, filename, speedup)`
წარმატების პირობა: `num_items(Items.Cactus) >= 131072`

### `Leaderboards.Sunflowers_Single`
`leaderboard_run(Leaderboards.Sunflowers_Single, filename, speedup)`
წარმატების პირობა: `num_items(Items.Power) >= 10000`

### `Leaderboards.Pumpkins_Single`
`leaderboard_run(Leaderboards.Pumpkins_Single, filename, speedup)`
წარმატების პირობა: `num_items(Items.Pumpkin) >= 10000000`

### `Leaderboards.Wood_Single`
`leaderboard_run(Leaderboards.Wood_Single, filename, speedup)`
წარმატების პირობა: `num_items(Items.Wood) >= 500000000`

### `Leaderboards.Carrots_Single`
`leaderboard_run(Leaderboards.Carrots_Single, filename, speedup)`
წარმატების პირობა: `num_items(Items.Carrot) >= 100000000`

### `Leaderboards.Hay_Single`
`leaderboard_run(Leaderboards.Hay_Single, filename, speedup)`
წარმატების პირობა: `num_items(Items.Hay) >= 100000000`

---

[სიმულაცია](docs/unlocks/simulation.md)      [დროის აღრიცხვა](docs/unlocks/timing.md)      [ავტომატური განბლოკვა](docs/unlocks/auto_unlock.md)      [ფასები](docs/unlocks/costs.md)      [სტატისტიკა](docs/stats.md)

[get_cost()](functions/get_cost)      [num_unlocked()](functions/num_unlocked)      [leaderboard_run()](functions/leaderboard_run)
