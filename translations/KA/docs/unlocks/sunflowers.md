[<- მორწყვა](docs/unlocks/watering.md)
---
# მზესუმზირები
[მზესუმზირები](objects/sunflower) მზის ენერგიას აგროვებს. ეს ენერგია შეგიძლია მოიპოვო. 

მათი დარგვა ზუსტად ისევე ხდება, როგორც სტაფილოსი ან გოგრისა. 

გაზრდილი მზესუმზირის მოკრეფა ენერგიას იძლევა.
თუ ფერმაში მინიმუმ 10 მზესუმზირაა და მოკრეფ ისეთს, რომელსაც ყველაზე მეტი ფურცელი აქვს, `8`-ჯერ მეტ ენერგიას მიიღებ!
თუ მზესუმზირას ისე მოკრეფ, რომ სხვა მზესუმზირას მეტი ფურცელი აქვს, შემდეგი მოკრეფილი მზესუმზირაც მხოლოდ ჩვეულებრივ რაოდენობას მოგცემს (8-ჯერადი ბონუსის გარეშე).

`measure()` აბრუნებს დრონის ქვეშ მყოფი მზესუმზირის ფურცლების რაოდენობას.
მზესუმზირას მინიმუმ `7` და მაქსიმუმ `15` ფურცელი აქვს.
სრულად გაზრდამდეც კი შეიძლება მზესუმზირების გაზომვა და ისინი 10 მზესუმზირის ლიმიტში ითვლება.

რამდენიმე მზესუმზირას შეიძლება ერთნაირი რაოდენობის ფურცელი ჰქონდეს, ამიტომ ყველაზე მეტი ფურცლის მქონეც შეიძლება რამდენიმე იყოს. ამ შემთხვევაში მნიშვნელობა არ აქვს, რომელს მოკრეფ.

{{codeexample 
{
    "camera_position": {"x": -1.5, "y": 2, "z": 4},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": true,
    "inventory_scale": 5,
    "show_output": false,
    "collapsing": true,
    "autoplay": false,
    "items": [{"item": "carrot", "n": 10}],
    "world_size": {"x": 5, "y": 2},
    "execution_speed": 2,
    "digging_speed": 1,
    "action_ticks": 200,
    "operation_ticks": 1,
    "seed": 4,
    "exclude_unlocks": ["fertilizer", "watering"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
move(North)
for _ in range(5):
    till()
    plant(Entities.Sunflower)
    move(East)
move(North)
do_a_flip()
do_a_flip()
do_a_flip()
do_a_flip()
do_a_flip()
#CODE
for _ in range(5):
    till()
    plant(Entities.Sunflower)
    print(measure())
    move(East)
for _ in range(5):
    while not can_harvest():
        pass
    harvest()
    move(East)
}}

სანამ ენერგია გაქვს, დრონი მას იყენებს, რომ ორჯერ უფრო სწრაფად იმუშაოს.
ის 1 ენერგიას ხარჯავს ყოველ 30 მოქმედებაზე, მაგალითად გადაადგილებაზე, მოკრეფაზე ან დარგვაზე.
კოდის სხვა ინსტრუქციების შესრულებამაც შეიძლება დახარჯოს ენერგია, მაგრამ გაცილებით ნაკლები, ვიდრე დრონის მოქმედებებმა.

ზოგადად, ყველაფერი, რასაც სიჩქარის გაუმჯობესებები აჩქარებს, ენერგიითაც ჩქარდება.
ყველაფერი, რაც ენერგიით ჩქარდება, ენერგიასაც ხარჯავს მისი შესრულების დროის პროპორციულად, სიჩქარის გაუმჯობესებების გაუთვალისწინებლად.

---

[სტატისტიკა](docs/stats.md)      [სიები](docs/scripting/lists.md)      [ლექსიკონები](docs/scripting/dicts.md)      [ცვლადები](docs/scripting/variables.md)      [for ციკლი](docs/scripting/for.md)      [if ინსტრუქცია](docs/scripting/if.md)      [ოპერატორები](docs/scripting/operators.md)

[harvest()](functions/harvest)      [plant()](functions/plant)      [move()](functions/move)      [till()](functions/till)      [measure()](functions/measure)
