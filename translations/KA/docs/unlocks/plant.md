[<- სიჩქარის გაუმჯობესება](docs/unlocks/speed.md) <right>[სტაფილო ->](docs/unlocks/carrots.md)
<right>[გამართვა ->](docs/scripting/debug.md)
<right>[ოპერატორები ->](docs/scripting/operators.md)
---
# დარგვა
ბალახი იმით არის კარგი, რომ თავისით იზრდება. ყველა სხვა მცენარე `plant()` ფუნქციით უნდა დარგო. ამ ეტაპზე ერთადერთი მცენარე, რომლის დარგვაც შეგიძლია, ბუჩქია.
ფუნქციას სასურველი მცენარის ტიპს ასე გადასცემ:

{{codeexample 
{
    "camera_position": {"x": 0, "y": 1.5, "z": 4},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": true,
    "show_output": false,
    "collapsing": true,
    "autoplay": false,
    "items": [],
    "world_size": {"x": 1, "y": 1},
    "execution_speed": 1,
    "digging_speed": 1,
    "action_ticks": 200,
    "operation_ticks": 100,
    "seed": 1,
    "exclude_unlocks": ["watering", "fertilizer"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
#CODE
plant(Entities.Bush)
}}

ეს დრონის ქვეშ ბუჩქს დარგავს.

`clear()`-ის გამოძახებით მთელი ფერმა ისევ ბალახით დაიფარება და დრონის პოზიციაც თავიდან დაყენდება.

როგორც ჩანს, თუ ფერმაში ერთდროულად ერთზე მეტი ტიპის მცენარეს მოიყვან, ზოგჯერ მოსავლიანობა იზრდება. მეტის გასაგებად პოლიკულტურა უნდა შეისწავლო.

---

[სტატისტიკა](docs/stats.md)      [if ინსტრუქცია](docs/scripting/if.md)      [გრძნობები](docs/unlocks/senses.md)      [პოლიკულტურა](docs/unlocks/polyculture.md)

[harvest()](functions/harvest)      [plant()](functions/plant)
