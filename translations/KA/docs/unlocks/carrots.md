[<- დარგვა](docs/unlocks/plant.md) <right>[მორწყვა ->](docs/unlocks/watering.md)
<right>[ხეები ->](docs/unlocks/trees.md)
---
# სტაფილო
სანამ `plant(Entities.Carrot)`-ით სტაფილოს დარგავ, ნიადაგი უნდა გადახნა. ამით მიწა `Grounds.Soil`-ად გადაიქცევა. გადასახნავად უბრალოდ გამოიძახე `till()`. `till()`-ის ხელახლა გამოძახება მას ისევ `Grounds.Grassland`-ად აქცევს.

სტაფილოს დასარგავად ხე-ტყე და თივა დაგჭირდება. ეს ნივთები `plant(Entities.Carrot)`-ის გამოძახებისას ავტომატურად გამოგაკლდება.
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
    "items": [{"item": "hay", "n": 1}, {"item": "wood", "n": 1}],
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
till()
plant(Entities.Carrot)
for _ in range(8):
    do_a_flip()
harvest()
till()
}}

ნებისმიერი მცენარის ფასს მის [საკუთარ გვერდზე](objects/carrot) ნახავ.

---

[სტატისტიკა](docs/stats.md)      [დარგვა](docs/unlocks/plant.md)      [მორწყვა](docs/unlocks/watering.md)      [if ინსტრუქცია](docs/scripting/if.md)      [გრძნობები](docs/unlocks/senses.md)      [პოლიკულტურა](docs/unlocks/polyculture.md)

[harvest()](functions/harvest)      [plant()](functions/plant)      [move()](functions/move)      [till()](functions/till)
