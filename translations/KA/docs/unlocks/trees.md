[<- სტაფილო](docs/unlocks/carrots.md) <right>[გოგრები ->](docs/unlocks/pumpkins.md)
---
# ხეები
ხე-ტყის მისაღებად [ხეები](objects/tree) ბუჩქებზე უკეთესია. თითოეული 5 ხე-ტყეს იძლევა. ბუჩქების მსგავსად, მათი დარგვა ბალახზეც შეიძლება და ნიადაგზეც.

ხეებს სივრცე უყვართ და ერთმანეთის გვერდით დარგვა მათ ზრდას ანელებს. ზრდის დრო ორმაგდება ყოველი ხისთვის, რომელიც პირდაპირ ჩრდილოეთით, აღმოსავლეთით, დასავლეთით ან სამხრეთით მდებარე უჯრაზეა. ასე რომ, თუ ყველა უჯრაზე ხეს დარგავ, ისინი `2*2*2*2 = 16`-ჯერ უფრო ნელა გაიზრდებიან.
{{codeexample 
{
    "camera_position": {"x": -1, "y": 1.8, "z": 5},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": false,
    "show_inventory": true,
    "show_output": false,
    "collapsing": false,
    "autoplay": true,
    "items": [],
    "world_size": {"x": 3, "y": 3},
    "execution_speed": 10,
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
for i in range(get_world_size()):
	for j in range(get_world_size()):
		plant(Entities.Tree)
		move(North)
	move(East)
}}

<spoiler=აჩვენე> 
აქ `%` ოპერატორი გამოგადგება. `%` ოპერატორი გაყოფის ნაშთს აბრუნებს. ლუწი რიცხვების `2`-ზე გაყოფისას ნაშთია `0`, კენტი რიცხვების `2`-ზე გაყოფისას კი ნაშთია `1`.
ასე რომ, რიცხვი ლუწია თუ არა, ასე შეგიძლია შეამოწმო:

{{codeexample 
{
    "camera_position": {"x": -0.5, "y": 1.5, "z": 4},
    "show_image": false,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": false,
    "show_output": true,
    "collapsing": true,
    "autoplay": false,
    "items": [],
    "world_size": {"x": 2, "y": 1},
    "execution_speed": 1,
    "digging_speed": 1,
    "action_ticks": 200,
    "operation_ticks": 200,
    "seed": 1,
    "exclude_unlocks": ["watering", "fertilizer"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
#CODE
def is_even(n):
	return n % 2 == 0

print("is_even(0): ", is_even(0))
print("is_even(1): ", is_even(1))
print("is_even(2): ", is_even(2))
print("is_even(5): ", is_even(5))
print("is_even(-1): ", is_even(-1))
print("is_even(x): ", is_even(get_pos_x()))
}}
</spoiler>

---

[სტატისტიკა](docs/stats.md)      [ოპერატორები](docs/scripting/operators.md)      [if ინსტრუქცია](docs/scripting/if.md)      [for ციკლი](docs/scripting/for.md)      [პოლიკულტურა](docs/unlocks/polyculture.md)

[harvest()](functions/harvest)      [plant()](functions/plant)      [move()](functions/move)
