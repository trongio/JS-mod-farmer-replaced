[<- ბრინჯი](docs/unlocks/rice.md) <right>[ფერადი ბლოკები ->](docs/unlocks/debug_place.md)
<right>[პირამიდები ->](docs/unlocks/pyramid.md)
---
# ბამბუკი
ბამბუკი მცენარეა, რომელიც ზემოთ იზრდება. ზრდა რომ გააგრძელოს, შენმა დრონმა მისი ყავისფერი ნაყოფი რეგულარულად უნდა მოწყვიტოს. ამაში ახლად განბლოკილი `hover` ბრძანება დაგვეხმარება.

ბამბუკი გადახნულ ნიადაგზე (`Grounds.Soil`) უნდა დარგო, დასარგავად კი ბრინჯი დაგჭირდება.

ეს კოდი რგავს ბამბუკს და ელოდება, სანამ პირველ ნაყოფამდე გაიზრდება.
{{codeexample
{
"camera_position": {"x": -1, "y": 0, "z": 10},
"show_image": true,
"image_size": {"x": 800, "y": 500},
"show_code": true,
"show_inventory": true,
"show_output": false,
"collapsing": true,
"autoplay": false,
"items": [{"item": "rice", "n": 100}],
"world_size": {"x": 4, "y": 4},
"execution_speed": 2,
"action_ticks": 200,
"operation_ticks": 200,
"seed": 1,
"exclude_unlocks": ["watering", "fertilizer", "mushrooms", "coal"]
}
#SETUP
move(North)
move(East)
#CODE
till()
plant(Entities.Bamboo)
move(East) # move away so the bamboo can grow
}}

## ნაყოფი
ბამბუკი წამში ზუსტად 1 ბლოკით იზრდება.

ბამბუკი ყოველ რამდენიმე ბლოკში ერთხელ ნაყოფს ისხამს. როცა ეს ხდება, ზრდას წყვეტს და ნაყოფის მოსაშორებლად დრონით პირდაპირ მასში უნდა შეფრინდე. ბამბუკი მესამე ნაყოფამდე იზრდება ზემოთ, მერე კი უფრო მაღლა აღარ წავა, მოწყვეტის შემდეგაც კი.

ნაყოფის მოსაწყვეტად ჯერ `hover`-ით საჭირო სიმაღლეზე უნდა ახვიდე. მერე `move`-ით ნაყოფში შედი, რომ მოწყვიტო, და ისევ `move`-ით გამოდი, რომ ბამბუკმა ზრდა გააგრძელოს.

`measure()`-ით შეგიძლია ბამბუკი გაზომო და გაიგო, რა სიმაღლეზე გამოჩნდება შემდეგი ნაყოფი. `measure()` თვლას `0`-დან იწყებს, ამიტომ `hover(measure())` დრონს ზუსტად იმ სიმაღლეზე მიიყვანს, სადაც ნაყოფი იქნება.

{{codeexample
{
"camera_position": {"x": -1, "y": 0, "z": 10},
"show_image": true,
"image_size": {"x": 800, "y": 500},
"show_code": true,
"show_inventory": true,
"show_output": false,
"collapsing": true,
"autoplay": false,
"items": [{"item": "rice", "n": 100}],
"world_size": {"x": 4, "y": 4},
"execution_speed": 2,
"action_ticks": 200,
"operation_ticks": 200,
"seed": 1,
"exclude_unlocks": ["watering", "fertilizer", "mushrooms", "coal"]
}
#SETUP
move(North)
move(East)
#CODE
till()
plant(Entities.Bamboo)
hover(measure()) # hover such that drone will be next to fruit

move(East) # move away and wait for it to grow
for i in range(3):
    do_a_flip()

move(West) # pluck fruit
move(East) # move away again
}}

## ფრენის სიმაღლე
`hover` ბრძანება ერთ არგუმენტს იღებს: სიმაღლეს, რომელზეც დრონმა უნდა იფრინოს. ეს არის რიცხვი `0`-დან `18`-მდე. დრონის ნაგულისხმევი ფრენის სიმაღლეა `1`. `hover` დრონს მყარ ობიექტებში ვერ ჩაიყვანს, ამიტომ თუ დრონი უკვე ბამბუკის ზემოთაა, `hover(0)`-ით მასში ვერ შეხვალ.

`hover` აბრუნებს `True`-ს, თუ დრონმა მითითებულ სიმაღლეს მიაღწია, წინააღმდეგ შემთხვევაში კი `False`-ს.

გარდა ამისა, არსებობს `get_hover` ბრძანება, რომელიც დრონის მიმდინარე ფრენის სიმაღლეს აბრუნებს.

თუ ჰაერში ყოფნისას `dig`-ს, `harvest`-ს ან `plant`-ს გამოიყენებ, დრონი ნაგულისხმევ სიმაღლეს (`1`) დაუბრუნდება.

და ბოლოს, `get_pos_z()`-ზე ფრენის სიმაღლე გავლენას არ ახდენს. ის ყოველთვის დრონის ქვეშ მყოფი ბლოკის z-პოზიციას აბრუნებს.

## ბამბუკის მოკრეფა
`harvest()`-იც გამოდგება, მაგრამ ბამბუკის მინდვრების მოკრეფის საუკეთესო გზაა, დრონით ფრენის `0` სიმაღლეზე (`hover(0)`) ბამბუკში შეხვიდე. ასე მთელ მცენარეს ერთბაშად მოკრეფ.

თუ გინდა ბამბუკი მოკრიფო ისე, რომ არ გაანადგურო, შეგიძლია `move`-ით ნებისმიერ სიმაღლეზე შეხვიდე მასში, მაგალითად `hover(3)`-ზე. ასე იმ სიმაღლიდან ზემოთ ბამბუკის ყველა ნაწილს მოკრეფ, თავად მცენარე კი ცოცხალი დარჩება. ასე ნაწილობრივ მოკრეფილ ბამბუკს ნაყოფი თავიდან გამოესხმება, ამიტომ ისევ მოწყვეტა მოგიწევს.

ორივე შემთხვევაში მოსავლიანობა განადგურებული ბამბუკის ნაწილების რაოდენობის კვადრატის ტოლია, ამიტომ რაც უფრო მაღალია ბამბუკი, მით უკეთესია მოსავლიანობა.

---

[სტატისტიკა](docs/stats.md)      [for ციკლი](docs/scripting/for.md)      [ცვლადები](docs/scripting/variables.md)      [if ინსტრუქცია](docs/scripting/if.md)      [ფუნქციები](docs/scripting/functions.md)      [ბრინჯი](docs/unlocks/rice.md)

[harvest()](functions/harvest)      [plant()](functions/plant)      [move()](functions/move)      [till()](functions/till)      [measure()](functions/measure)
