[<- სტაფილო](docs/unlocks/carrots.md) <right>[სასუქი ->](docs/unlocks/fertilizer.md)
<right>[მზესუმზირები ->](docs/unlocks/sunflowers.md)
---
# მორწყვა
მორწყული მცენარეები უფრო სწრაფად იზრდება. მიწას აქვს წყლის დონე `0`-დან `1`-მდე.
`get_water()` ფუნქცია დრონის ქვეშ მყოფი მიწის წყლის დონეს აბრუნებს.

მცენარის ზრდის სიჩქარე წრფივად იზრდება: წყლის 0 დონეზე 1x-დან წყლის 1 დონეზე 5x-მდე.

დროთა განმავლობაში მიწა შრება. საშუალოდ ის წამში თავისი მიმდინარე წყლის 1%-ს კარგავს, მცირე შემთხვევითი გადახრებით. მაღალი წყლის დონის შენარჩუნებას გაცილებით მეტი წყალი სჭირდება, ვიდრე დაბალისას.

წყალი მცენარეებზე შეგიძლია გამოიყენო. ყოველ 10 წამში შენს ინვენტარს ავტომატურად ემატება წყლის ერთი ავზი.
`Unlocks.Watering`-ის გაუმჯობესება აორმაგებს წყლის რაოდენობას, რომელსაც ყოველ 10 წამში იღებ.

ერთ ავზში `0.25` წყალია.

ნებისმიერი მიწის მოსარწყავად მის ზემოთ გამოიძახე `use_item(Items.Water)`.
{{codeexample 
{
    "camera_position": {"x": -4, "y": 1, "z": 6},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": true,
    "show_output": false,
    "collapsing": true,
    "autoplay": false,
    "items": [{"item": "water", "n": 10}],
    "world_size": {"x": 9, "y": 1},
    "execution_speed": 2,
    "digging_speed": 1,
    "action_ticks": 200,
    "operation_ticks": 1,
    "seed": 1,
    "exclude_unlocks": ["fertilizer"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
for _ in range(9):
    till()
    move(East)
#CODE
for i in range(5):
    if i > 0:
        use_item(Items.Water, i)
    plant(Entities.Tree)
    print(get_water())
    move(East)
    move(East)
}}

---

[სასუქი](docs/unlocks/fertilizer.md)

[use_item()](functions/use_item)      [get_water()](functions/get_water)
