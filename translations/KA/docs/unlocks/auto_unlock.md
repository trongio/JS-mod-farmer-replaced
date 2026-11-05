[<- ფასები](docs/unlocks/costs.md)
---
# ავტომატური განბლოკვა
თამაშის სრულად ავტომატიზებისთვის შეგიძლია გამოიყენო `unlock()` ფუნქცია, რომელიც შესაძლებლობებს ავტომატურად განბლოკავს.
მაგალითად, `unlock(Unlocks.Speed)` და `unlock(Unlocks.Expand)` განბლოკავს სიჩქარესა და გაფართოებას.

{{codeexample 
{
    "camera_position": {"x": 0, "y": 1, "z": 4},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": true,
    "show_output": false,
    "collapsing": true,
    "autoplay": false,
    "items": [{"item": "hay", "n": 300}, {"item": "wood", "n": 100000}],
    "world_size": {"x": 1, "y": 1},
    "execution_speed": 1,
    "digging_speed": 1,
    "action_ticks": 200,
    "operation_ticks": 1,
    "seed": 1,
    "exclude_unlocks": ["watering", "fertilizer"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
#CODE
for _ in range(6):
    harvest()
    unlock(Unlocks.Grass)
}}

განბლოკვის ფასის გასაგებად უბრალოდ გამოიყენე `get_cost()` ფუნქცია, ისევე როგორც მცენარისა ან ნივთის შემთხვევაში.
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
    "operation_ticks": 200,
    "seed": 1,
    "exclude_unlocks": ["watering", "fertilizer", "loops"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
#CODE
print(get_cost(Unlocks.Loops))
}}

---

[ფასები](docs/unlocks/costs.md)      [ლექსიკონები](docs/scripting/dicts.md)      [if ინსტრუქცია](docs/scripting/if.md)      [რეიტინგი](docs/unlocks/leaderboard.md)

[get_cost()](functions/get_cost)      [unlock()](functions/unlock)
