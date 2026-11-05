[<- სიები](docs/scripting/lists.md) <right>[ფასები ->](docs/unlocks/costs.md)
---
# ლექსიკონები
ლექსიკონი მონაცემთა სტრუქტურაა, რომელიც გასაღებებს მნიშვნელობებს უკავშირებს, ისევე როგორც ნამდვილი ლექსიკონი უკავშირებს სიტყვებს მათ განმარტებებს. ამ მნიშვნელობების პოვნა ასე ძალიან სწრაფად შეგიძლია.

ლექსიკონი ასე შეიძლება შექმნა:

{{codeexample 
{
    "show_image": false,
    "show_code": true,
    "show_inventory": false,
    "show_output": true,
    "collapsing": true,
    "autoplay": false,
    "execution_speed": 1,
    "digging_speed": 1,
    "action_ticks": 200,
    "operation_ticks": 100,
    "seed": 0,
    "exclude_unlocks": ["watering", "fertilizer"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
#CODE
right_of = {North:East, East:South, South:West, West:North}
}}

ორწერტილის წინა გამოსახულება გასაღებია, ხოლო ორწერტილის შემდეგი გამოსახულება ის მნიშვნელობაა, რომელსაც გასაღები უკავშირდება.
ზემოთ მოცემული ლექსიკონი თითოეულ მიმართულებას უკავშირებს მის მარჯვნივ მდებარე მიმართულებას.

აი, კიდევ ერთი ლექსიკონი, რომელიც დრონის პოზიციას მის ქვეშ მყოფ ობიექტს უკავშირებს.
{{codeexample 
{
    "show_image": false,
    "show_code": true,
    "show_inventory": false,
    "show_output": true,
    "collapsing": true,
    "autoplay": false,
    "execution_speed": 1,
    "digging_speed": 1,
    "action_ticks": 200,
    "operation_ticks": 100,
    "seed": 0,
    "exclude_unlocks": ["watering", "fertilizer"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
#CODE
x, y = get_pos_x(), get_pos_y()
entity_dict = {(x,y):get_entity_type()}
}}

გასაღებთან დაკავშირებული მნიშვნელობის მიღება სიის ელემენტზე წვდომას ჰგავს:
`value = dict[key]`

{{codeexample 
{
    "show_image": false,
    "show_code": true,
    "show_inventory": false,
    "show_output": true,
    "collapsing": true,
    "autoplay": false,
    "execution_speed": 1,
    "digging_speed": 1,
    "action_ticks": 200,
    "operation_ticks": 100,
    "seed": 0,
    "exclude_unlocks": ["watering", "fertilizer"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
#CODE
right_of = {North:East, East:South, South:West, West:North}
print(right_of[South])
}}

ლექსიკონში ახალი გასაღები-მნიშვნელობის წყვილის დამატება ასე შეგიძლია:
`dict[key] = value`

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
    "items": [],
    "world_size": {"x": 3, "y": 1},
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
plant(Entities.Bush)
move(East)
plant(Entities.Tree)
move(East)
#CODE
entity_dict = {}
for _ in range(3):
	entity_dict[(get_pos_x(), get_pos_y())] = get_entity_type()
	move(East)
print(entity_dict)
}}

გასაღებები უნიკალურია, ამიტომ თუ ლექსიკონში უკვე არსებულ გასაღებს დაამატებ, წინა მნიშვნელობა გადაიწერება.

`dict.pop(key)` გასაღები-მნიშვნელობის წყვილს წაშლის `dict`-იდან.

`key in dict` გამოითვლება როგორც `True`, თუ `key` არის `dict`-ის გასაღები, წინააღმდეგ შემთხვევაში კი როგორც `False`.
ამიტომ `if key in dict:`-ით შეგიძლია შეამოწმო, შეიცავს თუ არა `dict` ამ გასაღებს.

თუ ლექსიკონს `for` ციკლში ჩასვამ, მის ყველა გასაღებს გადაუყვები:
{{codeexample 
{
    "show_image": false,
    "show_code": true,
    "show_inventory": false,
    "show_output": true,
    "collapsing": true,
    "autoplay": false,
    "execution_speed": 1,
    "digging_speed": 1,
    "action_ticks": 200,
    "operation_ticks": 100,
    "seed": 0,
    "exclude_unlocks": ["watering", "fertilizer"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
#CODE
right_of = {North:East, East:South, South:West, West:North}
print(North in right_of)
for key in right_of:
	value = right_of[key]
	print(key, ":", value)
}}

გარანტია არ არის, რა თანმიმდევრობით გადაუყვება ციკლი გასაღებებს.

იხილე აგრეთვე [სიმრავლეები](docs/scripting/sets.md)

---

[სიები](docs/scripting/lists.md)      [სიმრავლეები](docs/scripting/sets.md)      [კორტეჟები](docs/scripting/tuples.md)      [ფასები](docs/unlocks/costs.md)

[len()](functions/len)
