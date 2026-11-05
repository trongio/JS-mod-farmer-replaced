[<- ლექსიკონები](docs/scripting/dicts.md) <right>[ავტომატური განბლოკვა ->](docs/unlocks/auto_unlock.md)
---
# ფასები
ნებისმიერი ფასი შეიძლება ლექსიკონის სახით წარმოვადგინოთ, რომელიც ნივთებს რიცხვებთან აკავშირებს.

`get_cost()` ფუნქცია სწორედ ასეთ ლექსიკონს აბრუნებს: მცენარის ან განბლოკვის ფასს.

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
    "seed": 1,
    "exclude_unlocks": ["watering", "fertilizer"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
#CODE
print(get_cost(Entities.Pumpkin))
}}

განბლოკვებისთვის შეგიძლია გადასცე არასავალდებულო მეორე არგუმენტი, რომელიც მიუთითებს, რომელი დონის ფასი გაინტერესებს. ნაგულისხმევად გამოიყენება განბლოკვის მიმდინარე დონე.

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
    "seed": 1,
    "exclude_unlocks": ["watering", "fertilizer"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
#CODE
print(get_cost(Unlocks.Loops, 0))
print(get_cost(Unlocks.Loops, 1))
}}

თუ განბლოკვა უკვე მაქსიმალურ დონეზეა, `get_cost()` ცარიელ ლექსიკონს აბრუნებს.

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
    "seed": 1,
    "exclude_unlocks": ["watering", "fertilizer"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
#CODE
cost = get_cost(Entities.Carrot)
for item in cost:
	if num_items(item) < cost[item]:
		print("missing", cost[item] - num_items(item), item)
}}

---

[ლექსიკონები](docs/scripting/dicts.md)      [ავტომატური განბლოკვა](docs/unlocks/auto_unlock.md)      [რეიტინგი](docs/unlocks/leaderboard.md)

[get_cost()](functions/get_cost)
