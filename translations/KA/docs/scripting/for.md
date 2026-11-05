[<- გაფართოება 2](docs/unlocks/expand_2.md)
---
# for ციკლი
`for` ციკლი ისევე მუშაობს, როგორც Python-ში. ზოგ ენაში მას foreach ციკლს ეძახიან და არ უნდა აგერიოს C-ს სტილის for ციკლში, რომელიც სხვანაირად მუშაობს.

`for i in sequence:
	#do something with i`

`while` ციკლის მსგავსად, `for` ციკლიც კოდის ბლოკს რამდენჯერმე იმეორებს. ოღონდ პირობაზე დაყრდნობის ნაცვლად, ციკლის ტანს მიმდევრობის თითოეული ელემენტისთვის ერთხელ ასრულებს.

## სინტაქსი
for ციკლი ასე გამოიყურება:

`for variable_name in sequence:
	#code block`

`variable_name` შეიძლება ნებისმიერი სახელი იყოს, რომელსაც აირჩევ. ეს არის ცვლადი, რომელიც მიმდევრობის მიმდინარე ელემენტს ინახავს. `sequence` უნდა იყოს ისეთი მნიშვნელობა, რომელსაც შეიძლება გადაუყვე, მაგალითად, რიცხვების დიაპაზონი. კოდის ბლოკი თითოეული ელემენტისთვის ერთხელ სრულდება და ციკლის ცვლადს ყოველ ჯერზე ეს ელემენტი ენიჭება.

## მიმდევრობები
[დიაპაზონები](functions/range)      <unlock=lists>[სიები](docs/scripting/lists.md)      </unlock><unlock=functions>[კორტეჟები](docs/scripting/tuples.md)      </unlock><unlock=dicts>[ლექსიკონები](docs/scripting/dicts.md)      </unlock><unlock=sets>[სიმრავლეები](docs/scripting/sets.md)</unlock>

## მაგალითი
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
do_a_flip()
#CODE
for i in range(5):
    harvest()
}}

ეს ციკლი ტანს ზუსტად იმდენჯერ ასრულებს, რამდენჯერაც მითითებულია. არსებითად იგივეა, რომ დაწერო

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
do_a_flip()
#CODE
i = 0
harvest()
i = 1
harvest()
i = 2
harvest()
i = 3
harvest()
i = 4
harvest()
}}


---

[while ციკლი](docs/scripting/while.md)      [break ინსტრუქცია](docs/scripting/break.md)      [continue ინსტრუქცია](docs/scripting/continue.md)

[range()](functions/range)
