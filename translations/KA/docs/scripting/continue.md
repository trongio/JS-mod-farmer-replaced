[<- while ციკლი](docs/scripting/while.md)
---
# continue ინსტრუქცია
`continue` წყვეტს ციკლის მიმდინარე იტერაციას და გადადის ყველაზე შიდა ციკლის შემდეგ იტერაციაზე.

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
for i in range(10):
	print("this happens every time")
	continue
	print("this is never printed")
}}

ასე ციკლის `10`-ივე იტერაცია სრულდება, მაგრამ `print` ინსტრუქცია `continue`-ს შემდეგ ყოველთვის გამოიტოვება.

იგივე მუშაობს `while` ციკლებშიც.

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
plant(Entities.Tree)
#CODE
while True:
	if not can_harvest():
		continue

	harvest()
}}

ეს კოდი `harvest()`-ს მხოლოდ მაშინ იძახებს, როცა `can_harvest()` არის `True`.
შედეგი იგივეა, რაც ამ კოდის:

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
plant(Entities.Tree)
#CODE
while True:
	if can_harvest():
		harvest()
}}

ჩადგმულ ციკლებში `continue` ყოველთვის ყველაზე შიდა ციკლზე მოქმედებს.

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
for i in range(2):
	for j in range(2):
		print("this is printed 4 times")
		continue
		print("this is never printed")
	print("this is printed 2 times")
}}

---

[while ციკლი](docs/scripting/while.md)      [for ციკლი](docs/scripting/for.md)      [break ინსტრუქცია](docs/scripting/break.md)
