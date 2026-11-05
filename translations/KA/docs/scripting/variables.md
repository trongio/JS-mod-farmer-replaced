[<- ოპერატორები](docs/scripting/operators.md) <right>[სიები ->](docs/scripting/lists.md)
<right>[ფუნქციები ->](docs/scripting/functions.md)
---
# ცვლადები
ცვლადი შეგიძლია წარმოიდგინო, როგორც სახელიანი კონტეინერი, რომელშიც მნიშვნელობის შენახვა შეიძლება.
`=` ოპერატორით ცვლადს აცხადებ და მასში მნიშვნელობას ინახავ.

`variable_name = value`

ოპერატორის მარცხენა მხარეს ცვლადის სახელია. შეგიძლია ნებისმიერი დასაშვები სახელი დაარქვა.
მარჯვენა მხარეს გამოსახულებაა, რომლის შედეგიც ცვლადში შეინახება.

გამოაცხადე ცვლადი სახელად `a` და შეინახე მასში მნიშვნელობა `5`:
`a = 5`
გამოაცხადე ცვლადი სახელად `b` და შეინახე მასში `can_harvest()`-ის დაბრუნებული მნიშვნელობა:
`b = can_harvest()`

არ აგერიოს `=` ოპერატორი `==` ოპერატორში. 
`==` ოპერატორი ამოწმებს, ტოლია თუ არა ორი მნიშვნელობა, და აბრუნებს `True`-ს ან `False`-ს.
`=` ოპერატორი მარჯვენა მხარეს მყოფ მნიშვნელობას ანიჭებს მარცხენა მხარეს მყოფ სახელს.

როცა ცვლადს მნიშვნელობა მიენიჭება, კოდში შეგიძლია გამოიყენო და მასში შენახულ მნიშვნელობას მიწვდე.

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
a = 5
for i in range(a):
	do_a_flip()
}}

ზემოთ მოცემული ციკლი 5-ჯერ სრულდება, რადგან `a`-ს მნიშვნელობა `5`-ია.
`i`, რომელიც `for` ციკლშია, ისიც ცვლადია. ყოველ იტერაციაზე მას ავტომატურად ენიჭება მიმდევრობის მიმდინარე მნიშვნელობა. აუცილებელი არ არის, რომ მას `i` ერქვას; შეგიძლია ცვლადის ნებისმიერი დასაშვები სახელი დაარქვა.

ცვლადებით იგივეს გაკეთება while ციკლითაც შეგიძლია:

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
a = 5
i = 0
while i < a:
	do_a_flip()
	i = i + 1
}}

ეს იგივეს აკეთებს, რასაც ზემოთ მოცემული `for` ციკლი, მაგრამ `i` ხელით უნდა გაზარდო.
`i`-ს გასაზრდელად მას ანიჭებ მის მიმდინარე მნიშვნელობას პლუს `1`. ცვლადის შეცვლა მისი წინა მნიშვნელობის მიხედვით ძალიან ხშირია.
ამის შემოკლება შეიძლება ამ ოპერატორებით: `+=, -=, *=, /=, %=`

`i = i + 1` იგივეა, რაც `i += 1`
`a = a / 3` იგივეა, რაც `a /= 3`

---

[ოპერატორები](docs/scripting/operators.md)      [while ციკლი](docs/scripting/while.md)      [for ციკლი](docs/scripting/for.md)      [ფუნქციები](docs/scripting/functions.md)      [სახელების არეები](docs/scripting/scopes.md)
