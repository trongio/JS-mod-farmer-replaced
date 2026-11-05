[<- მორწყვა](docs/unlocks/watering.md) <right>[ლაბირინთები ->](docs/unlocks/mazes.md)
---
# სასუქი
რაღაც მომენტში მცენარეების გაზრდის ლოდინი საკმარისად ეფექტური აღარ არის.
წყლის მსგავსად, ყოველ 10 წამში ავტომატურად მიიღებ 1 სასუქს. ყოველი გაუმჯობესება ამ რაოდენობას აორმაგებს.

სასუქით მცენარეების მყისიერად გაზრდა შეგიძლია. `use_item(Items.Fertilizer)` დრონის ქვეშ მყოფი მცენარის დარჩენილ ზრდის დროს 2 წამით ამცირებს.

{{codeexample 
{
    "camera_position": {"x": 0, "y": 1, "z": 5},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": true,
    "show_output": false,
    "collapsing": true,
    "autoplay": false,
    "items": [{"item": "fertilizer", "n": 10}],
    "world_size": {"x": 1, "y": 1},
    "execution_speed": 2,
    "digging_speed": 1,
    "action_ticks": 200,
    "operation_ticks": 1,
    "seed": 1,
    "exclude_unlocks": ["watering"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
#CODE
plant(Entities.Tree)
use_item(Items.Fertilizer)
use_item(Items.Fertilizer)
use_item(Items.Fertilizer)
harvest()
}}

ამას გვერდითი მოვლენებიც აქვს.
სასუქით გაზრდილი მცენარეები ინფიცირდება.

როცა ინფიცირებულ მცენარეს მოკრეფ, მისი მოსავლიანობის ნახევარი `Items.Weird_Substance`-ად იქცევა.
უცნაური სუბსტანცია მცენარეებზეც შეგიძლია გამოიყენო: ის მცენარისა და ყველა მოსაზღვრე მცენარის ინფიცირების მდგომარეობას საპირისპიროდ ცვლის.

თუ `use_item(Items.Weird_Substance)`-ს ინფიცირებულ მცენარეზე გამოიძახებ, ის განიკურნება; თუ ჯანმრთელ მცენარეზე გამოიყენებ, დაინფიცირდება.

თუ მას ისეთ ინფიცირებულ მცენარეზე გამოიყენებ, რომელსაც ჯანმრთელი მეზობლები აქვს, მცენარე განიკურნება, მეზობლები კი დაინფიცირდება, და პირიქით.

{{codeexample 
{
    "camera_position": {"x": -1, "y": 1.8, "z": 5},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": true,
    "show_output": false,
    "collapsing": true,
    "autoplay": false,
    "items": [{"item": "weird_substance", "n": 10}],
    "world_size": {"x": 3, "y": 3},
    "execution_speed": 1,
    "digging_speed": 1,
    "action_ticks": 200,
    "operation_ticks": 1,
    "seed": 1,
    "exclude_unlocks": ["watering"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
for _ in range(3):
    for _ in range(3):
        plant(Entities.Tree)
        move(East)
    move(North)

move(North)
move(East)

for _ in range(60):
    do_a_flip()
#CODE
for _ in range(6):
    use_item(Items.Weird_Substance)
    move(East)
}}

---

[სტატისტიკა](docs/stats.md)      [მორწყვა](docs/unlocks/watering.md)      [ლაბირინთები](docs/unlocks/mazes.md)

[use_item()](functions/use_item)
