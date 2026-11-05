[<- while ციკლი](docs/scripting/while.md)
---
# break ინსტრუქცია
`break`-ით ციკლის დროზე ადრე შეჩერება შეგიძლია. როცა შესრულება `break` ინსტრუქციას მიაღწევს, ის მაშინვე გამოდის ყველაზე შიდა ციკლიდან და იწყებს იმ კოდის შესრულებას, რომელიც ამ ციკლის შემდეგ მოდის.

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
	break
print(i)
}}

ეს `0`-ს დაბეჭდავს, რადგან პირველ იტერაციაზე `i` არის `0`, ხოლო ამის შემდეგ `break` ინსტრუქცია ციკლს ამთავრებს.

`while` ციკლებზეც მუშაობს.

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
		break
harvest()
}}

ეს კოდი `while` ციკლს მანამდე ასრულებს, სანამ `can_harvest()` `True` არ გახდება.
იგივე ეფექტი აქვს, რაც ამას:

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
while not can_harvest():
	pass
harvest()
}}

ჩადგმულ ციკლებში `break` ყოველთვის ყველაზე შიდა ციკლიდან გამოდის.

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
	for j in range(10):
		break
		print("this is never printed")
	print("this is printed 10 times")
}}

---

[while ციკლი](docs/scripting/while.md)      [for ციკლი](docs/scripting/for.md)      [continue ინსტრუქცია](docs/scripting/continue.md)
