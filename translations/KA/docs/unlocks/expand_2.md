[<- გაფართოება 1](docs/unlocks/expand_1.md)
---
# გაფართოება 2
შენი ფერმა ისევ გაფართოვდა! ახლა უჯრები ლამაზ რიგში აღარ დგას, ამიტომ კვადრატული ბადის შემოვლის გზა უნდა მოძებნო.

`while` ციკლით ეს შეუძლებელია, სანამ გრძნობებსა და ოპერატორებს არ განბლოკავ.
დროა, გაიცნო `for` ციკლი.

`for` ციკლის შესახებ ყველაფერს წაიკითხავ გვერდზე [for ციკლი](docs/scripting/for.md), მაგრამ ახლა ის მხოლოდ იმისთვის დაგჭირდება, რომ კოდი განსაზღვრული რაოდენობით გაიმეორო.

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
#CODE
for i in range(5):
	do_a_flip()
}}

`range(n)` ქმნის `n` რიცხვის მიმდევრობას `0`-დან `n - 1`-მდე. `for` ციკლი თავის ტანს მიმდევრობის ყოველი ელემენტისთვის ერთხელ ასრულებს. ამ მაგალითში `do_a_flip()` `5`-ჯერ გამოიძახება.

ახლა ხელმისაწვდომია `get_world_size()` ფუნქციაც. ის შენი ფერმის გვერდის სიგრძეს აბრუნებს. ასე შეგიძლია დაწერო კოდი, რომელიც გაფართოების შემდეგი გაუმჯობესებისას არ გაფუჭდება.

{{codeexample 
{
    "camera_position": {"x": -1, "y": 1.8, "z": 5},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": true,
    "show_output": false,
    "collapsing": true,
    "autoplay": false,
    "items": [],
    "world_size": {"x": 3, "y": 3},
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
do_a_flip()
#CODE
for i in range(get_world_size()):
	harvest()
	move(North)
}}

ეს მაგალითი ფერმის ერთ სვეტს მოკრეფს, ფერმის ნებისმიერი ზომისთვის.

თუ ვერ ხვდები, როგორ ატარო დრონი ფერმაში, ჩაიხედე ქვემოთ მოცემულ მინიშნებაში.
<spoiler=აჩვენე მინიშნება>ფერმაში გადაადგილების, რა თქმა უნდა, რამდენიმე გზა არსებობს.
ჩვენ ვეძებთ სისტემურ გზას, რომელიც ფერმის შემდეგი ზრდისას არ გაფუჭდება.
ფერმის ყველა ადგილამდე მისვლის სისტემური გზაა ამ ორი ნაბიჯის უსასრულოდ გამეორება:

1. იმოძრავე `North` მიმართულებით, სანამ დრონი ფერმის კიდეს არ გადაივლის და მეორე მხრიდან არ გამოჩნდება.
2. იმოძრავე `East` მიმართულებით.

`for i in range(get_world_size()):` შეიძლება დაგეხმაროს ამ იდეის კოდად გადაქცევაში.
</spoiler>
<spoiler=აჩვენე შესაძლო ამოხსნა>
{{codeexample 
{
    "camera_position": {"x": -1, "y": 1.8, "z": 5},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": true,
    "show_output": false,
    "collapsing": true,
    "autoplay": false,
    "items": [],
    "world_size": {"x": 3, "y": 3},
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
do_a_flip()
#CODE
for i in range(get_world_size()):
	for j in range(get_world_size()):
		#do a flip on every tile
		do_a_flip()
		move(North)
	move(East)
}}
</spoiler>

---

[for ციკლი](docs/scripting/for.md)      [while ციკლი](docs/scripting/while.md)      [ცვლადები](docs/scripting/variables.md)

[move()](functions/move)      [get_world_size()](functions/get_world_size)
