[<- სიჩქარის გაუმჯობესება](docs/unlocks/speed.md) <right>[გაფართოება 2 ->](docs/unlocks/expand_2.md)
<right>[სამთო საქმე ->](docs/unlocks/mining.md)
---
# გაფართოება 1
შენი ფერმა გაიზარდა! ამ სივრცეს დიდი აზრი არ ექნება, თუ დრონს ვერ ამოძრავებ, ამიტომ გაჩნდა ახალი ფუნქცია `move()`, რომელიც დრონს გადააადგილებს. `move()`-ს უნდა მიუთითო, რომელი მიმართულებით გინდა დრონის გადაადგილება. ამისთვის ოთხი ახალი მუდმივი არსებობს: `North, East, South, West`

მაგალითად, `move(North)` დრონს ერთი უჯრით ჩრდილოეთით გადააადგილებს.

თუ ფერმის კიდეს გადასცდები, დრონი მოპირდაპირე მხარეს გამოჩნდება.

{{codeexample 
{
    "camera_position": {"x": 0, "y": 1.8, "z": 5},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": true,
    "show_output": false,
    "collapsing": true,
    "autoplay": false,
    "items": [],
    "world_size": {"x": 1, "y": 3},
    "execution_speed": 2,
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
while True:
	move(North)
}}

---

[while ციკლი](docs/scripting/while.md)      [ოპერატორები](docs/scripting/operators.md)      [გაფართოება 2](docs/unlocks/expand_2.md)

[move()](functions/move)
