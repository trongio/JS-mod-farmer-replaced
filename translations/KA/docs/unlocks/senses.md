[<- ოპერატორები](docs/scripting/operators.md)
---
# გრძნობები
დრონი ახლა ხედავს! 

`get_pos_x()` და `get_pos_y()` ფუნქციები დრონის მიმდინარე x და y კოორდინატებს აბრუნებს. საწყის პოზიციაზე ორივე `0`-ია. x კოორდინატი `1`-ით იზრდება ყოველ უჯრაზე `East`-ის მიმართულებით, y კოორდინატი კი `1`-ით იზრდება ყოველ უჯრაზე `North`-ის მიმართულებით.
{{codeexample 
{
    "camera_position": {"x": -1, "y": 1.8, "z": 4},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": true,
    "show_output": true,
    "collapsing": true,
    "autoplay": false,
    "items": [],
    "world_size": {"x": 2, "y": 2},
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
move(East)
#CODE
print("x =", get_pos_x(), "y =", get_pos_y())
if get_pos_x() == 1 and get_pos_y() == 0:
    do_a_flip()
}}

`num_items(item)` აბრუნებს, რამდენი გაქვს მოცემული ნივთი.
{{codeexample 
{
    "camera_position": {"x": 0, "y": 1.5, "z": 4},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": true,
    "show_output": true,
    "collapsing": true,
    "autoplay": false,
    "items": [{"item": "hay", "n": 10}],
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
print(num_items(Items.Hay))
}}

`get_entity_type()` და `get_ground_type()` აბრუნებს დრონის ქვეშ მყოფი ობიექტის ან მიწის ტიპს.
{{codeexample 
{
    "camera_position": {"x": 0, "y": 1.5, "z": 6},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": true,
    "show_output": true,
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
plant(Entities.Bush)
#CODE
print(get_entity_type())
if get_entity_type() == Entities.Bush:
	do_a_flip()

print(get_ground_type())
if get_ground_type() != Grounds.Soil:
	do_a_flip()
}}

ახლა `None` დარეზერვებული სიტყვაც განბლოკილია! `None` არის მნიშვნელობა, რომელიც ნიშნავს, რომ მნიშვნელობა არ არსებობს.
მაგალითად, ფუნქცია, რომელსაც `return` ინსტრუქცია არ აქვს, სინამდვილეში `None`-ს აბრუნებს.

`get_entity_type()` აბრუნებს `None`-ს, თუ დრონის ქვეშ ობიექტი არ არის.


თუ გინდა გაიგო, რამდენი გაქვს კონკრეტული განბლოკვიდან, გამოიყენე `num_unlocked(unlock)` ფუნქცია.

მაგალითად, `num_unlocked(Unlocks.Speed)` დააბრუნებს, რამდენი სიჩქარის გაუმჯობესება გაქვს.

`num_unlocked(Unlocks.Senses)` დააბრუნებს `1`-ს, თუ გრძნობები განბლოკილია, და `0`-ს, თუ არა.

`num_unlocked()` ნივთებსა და ობიექტებზეც შეგიძლია გამოიყენო. ის აბრუნებს `1`-ს, თუ ნივთი ან ობიექტი განბლოკილია, წინააღმდეგ შემთხვევაში კი `0`-ს.

ფრთხილად: `num_unlocked(Unlocks.Carrots)` აბრუნებს, რამდენჯერ განიბლოკა ან გაუმჯობესდა ეს განბლოკვა.
`num_unlocked(Items.Carrot)` კი მხოლოდ `0`-ს ან `1`-ს აბრუნებს. იგივე ეხება სხვა მცენარეებსაც.

---

[if ინსტრუქცია](docs/scripting/if.md)      [ოპერატორები](docs/scripting/operators.md)      [ცვლადები](docs/scripting/variables.md)      [კორტეჟები](docs/scripting/tuples.md)      [ლექსიკონები](docs/scripting/dicts.md)      [მიწისქვეშა გრძნობები](docs/unlocks/underground_senses.md)

[get_pos_x()](functions/get_pos_x)      [get_pos_y()](functions/get_pos_y)      [get_entity_type()](functions/get_entity_type)      [get_ground_type()](functions/get_ground_type)      [num_items()](functions/num_items)      [num_unlocked()](functions/num_unlocked)
