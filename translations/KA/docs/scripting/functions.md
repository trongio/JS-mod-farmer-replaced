[<- ცვლადები](docs/scripting/variables.md) <right>[იმპორტი ->](docs/scripting/import.md)
---
# ფუნქციები
ახალი ფუნქციის განსასაზღვრად გამოიყენე დარეზერვებული სიტყვა `def`:
`def f(arg1, arg2 = False):
	#function code`

ფუნქციის გამოსაძახებლად გამოიყენე გამოძახების ოპერატორი `()`:
`f(42)`

ფუნქციებში ლოკალური და გლობალური ცვლადების შესახებ რომ გაიგო, იხილე [სახელების არეები](docs/scripting/scopes.md).

## შესავალი
ჩაშენებული ფუნქციები, მაგალითად `harvest()`, უკვე ნახე.
შეგიძლია საკუთარი ფუნქციებიც განსაზღვრო, რაც კოდის მოდულურად აგებაში დაგეხმარება. ფუნქცია კოდის ბლოკს სახელს არქმევს, ასე რომ, მისი გამოძახება ყველგან შეგიძლია, სადაც დაგჭირდება.

## ფუნქციის განსაზღვრა
მაგალითად, შეგიძლია განსაზღვრო ფუნქცია, რომელიც დრონს რამდენჯერმე გადააადგილებს.

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
def move_n_dir(n, dir):
	for i in range(n):
		move(dir)
}}

დარეზერვებული სიტყვა `def` გვეუბნება, რომ ეს ფუნქციის განსაზღვრაა. 
`move_n_dir` არის სახელი, რომელსაც ფუნქცია მიებმება. ეს შეიძლება იყოს ცვლადის ნებისმიერი სწორი სახელი და მისით გამოიძახებ ფუნქციას.
`n` და `dir` პარამეტრებია. ეს ცვლადებია, რომლებიც ფუნქციაში გადაცემულ მნიშვნელობებს ინახავს; ამ მნიშვნელობებს არგუმენტებსაც უწოდებენ. ფუნქციის განსაზღვრაში იმდენი პარამეტრი შეგიძლია დაამატო, რამდენიც გინდა.
`:`-ის შემდეგ მოდის კოდის ბლოკი, რომელიც ფუნქციის გამოძახებისას შესრულდება.

შემდეგი კოდი დრონს `2` უჯრით `North` მიმართულებით და `2` უჯრით `East` მიმართულებით გადააადგილებს.

{{codeexample 
{
    "camera_position": {"x": -1, "y": 1.8, "z": 5},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": true,
    "show_output": true,
    "collapsing": true,
    "autoplay": false,
    "items": [],
    "world_size": {"x": 3, "y": 3},
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
def move_n_dir(n, dir):
	for i in range(n):
		move(dir)

move_n_dir(2, North)
move_n_dir(2, East)
}}

როცა `def function():`-ს ხედავ, ის სინამდვილეში ცვლადზე მინიჭებად უნდა წარმოიდგინო, აი, ასეთად:
`function = create_new_function_object()`
როგორც ყველა მინიჭებისას, ცვლადს ვერ გამოიყენებ, სანამ მას მნიშვნელობა არ მიენიჭება!
`def` ინსტრუქცია ფუნქციის ნებისმიერ გამოძახებამდე უნდა შესრულდეს.
ეს კოდი შეცდომას გამოიწვევს:

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
func()
def func():
	pass
}}

## დაბრუნებული მნიშვნელობები
ფუნქციამ მნიშვნელობა რომ დააბრუნოს, გამოიყენე დარეზერვებული სიტყვა `return`. 
მაგალითად, ეს ფუნქცია განსაზღვრავს გამომრიცხავ OR ოპერაციას. გამომრიცხავი OR აბრუნებს `True`-ს, თუ ერთი მნიშვნელობა `True` არის, მეორე კი `False`:

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
def xor(a, b):
	return a != b

if xor(True, False):
	do_a_flip()
}}

[კორტეჟები](docs/scripting/tuples.md) რამდენიმე მნიშვნელობის დაბრუნების საშუალებას იძლევა.

## ნაგულისხმევი არგუმენტები
შეგიძლია ნაგულისხმევი მნიშვნელობებიც მიუთითო, რომლებიც გამოიყენება, თუ შესაბამის არგუმენტებს გამოტოვებ.

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
def f(a = False):
	if a:
		do_a_flip()

f()

f(True)
}}

არგუმენტს, რომელსაც ნაგულისხმევი მნიშვნელობა აქვს, ვერ მოჰყვება არგუმენტი, რომელსაც ნაგულისხმევი მნიშვნელობა არ აქვს.

## ფუნქციების გაღრმავებული გამოყენება
ფუნქციები ისეთივე მნიშვნელობებია, როგორც ყველა სხვა, ხოლო `def` ინსტრუქცია უბრალოდ მინიჭების ინსტრუქციასავით მუშაობს: ფუნქციას ანიჭებს იმ სახელს, რომელსაც შენ დაარქმევ.
ეს ასეთი რამეების გაკეთების საშუალებას იძლევა:

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
def f():
	def d():
		do_a_flip()
	return d

f()()
}}

აქ `f()` იძახებს `f` ფუნქციას, რომელიც განსაზღვრავს და აბრუნებს ახალ ფუნქციას, `d`-ს. შემდეგ მეორე `()` ასრულებს დაბრუნებულ ფუნქციას და დრონი ტრიალს აკეთებს.
(ასეთი რამ ჩვეულებრივ კარგი იდეა არ არის, რადგან ძნელი გასაგებია, რა ხდება.)

ფუნქციები, რომლებიც არგუმენტებად სხვა ფუნქციებს იღებს, ფანტაზიის გაშლის საშუალებას გაძლევს:

{{codeexample 
{
    "camera_position": {"x": -2, "y": 1.5, "z": 4},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": true,
    "show_output": true,
    "collapsing": true,
    "autoplay": false,
    "items": [{"item": "fertilizer", "n": 4}],
    "world_size": {"x": 5, "y": 1},
    "execution_speed": 1,
    "digging_speed": 1,
    "action_ticks": 200,
    "operation_ticks": 100,
    "seed": 1,
    "exclude_unlocks": ["watering"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
#CODE
def f(g, arg):
	for _ in range(4):
		g(arg)

f(move, East)
plant(Entities.Tree)
f(use_item, Items.Fertilizer)
}}

---

[ცვლადები](docs/scripting/variables.md)      [სახელების არეები](docs/scripting/scopes.md)      [კორტეჟები](docs/scripting/tuples.md)      [იმპორტი](docs/scripting/import.md)
