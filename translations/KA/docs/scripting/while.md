[<- პირველი პროგრამა](docs/first_program.md) <right>[სიჩქარის გაუმჯობესება ->](docs/unlocks/speed.md)
---
# while ციკლი
შენ განბლოკე `while` ციკლი და მნიშვნელობები `True` და `False`. `while` ციკლი ციკლის ტანს მანამ ასრულებს, სანამ პირობა `True` არის.

`while condition:
	#loop body`

უსასრულო ციკლების ნუ შეგეშინდება. დრონი შეიძლება გაიჭედოს, მაგრამ მთელი თამაში არ გაიყინება: უბრალოდ გააჩერე მიმდინარე პროგრამა და თავიდან სცადე.

## დამწყებთათვის
ალბათ უკვე სცადე `harvest()`-ის რამდენიმე გამოძახების ზედიზედ დაწერა:

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
    "operation_ticks": 200,
    "seed": 1,
    "exclude_unlocks": ["watering", "fertilizer"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
do_a_flip()
#CODE
harvest()
harvest()
harvest()
}}
ასე ერთი გაშვებისას რამდენჯერმე შეგიძლია მოკრეფა.
მაგრამ კარგი იქნება, თუ სამჯერზე მეტჯერ მოკრეფ, ერთი და იმავე კოდის რამდენჯერმე დაწერა კი პროგრამას მალე არეულობად აქცევს.
გამოსავალი ციკლია.
ციკლი ერთი და იმავე კოდის რამდენჯერმე შესრულების საშუალებას გაძლევს.

while ციკლი იღებს პირობას, ანუ მნიშვნელობას, რომელსაც მხოლოდ ორი მდგომარეობა შეიძლება ჰქონდეს: `True` ან `False`. 
ასეთ მნიშვნელობას ლოგიკური მნიშვნელობა ჰქვია.

შემდეგ ციკლი მასში მოთავსებულ კოდს მანამ ასრულებს, სანამ პირობა False არ გახდება.
while ციკლი ასე გამოიყურება:

`while condition:
	#loop body
	#loop body
	#...`

აქ "condition"-ის ნაცვლად უნდა ჩასვა ლოგიკური მნიშვნელობა, `#loop body`-ის ნაცვლად კი ის, რისი გაკეთებაც ციკლში გინდა.

ხელმისაწვდომია ორი მუდმივი ლოგიკური მნიშვნელობა. მუდმივები ისეთი მნიშვნელობებია, რომლებიც პროგრამის განმავლობაში არასდროს იცვლება.

მუდმივი ლოგიკური მნიშვნელობის შესაქმნელად უბრალოდ დაწერე `True` ან `False`.
ასე რომ, შეგიძლია დაწერო ან

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
    "operation_ticks": 200,
    "seed": 1,
    "exclude_unlocks": ["watering", "fertilizer"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
#CODE
while False:
	do_a_flip()
}}
ან

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
    "operation_ticks": 200,
    "seed": 1,
    "exclude_unlocks": ["watering", "fertilizer"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
#CODE
while True:
	do_a_flip()
}}
პირველი არასდროს გააკეთებს ტრიალს, მეორე კი სამუდამოდ გააგრძელებს ტრიალს (უსასრულო ციკლი).

ჩვეულებრივ უსასრულო ციკლის შექმნა ცუდი იდეაა, რადგან პროგრამას გაყინავს. ამ თამაშში კი იტერაციებს შორის დაყოვნებებია, ამიტომ დრონი ტრიალს მანამ გააგრძელებს, სანამ ხელით არ გააჩერებ შესრულების ღილაკზე ხელახლა დაჭერით.

შენიშნე, რომ ორწერტილის შემდეგი ხაზი შეწეულია. ასეთი შეწევა კოდის ბლოკების გამოსაყოფად გამოიყენება.
შეწევის დასამატებლად უბრალოდ დააჭირე Tab-ს, მოსახსნელად კი Shift + Tab-ს (ან Backspace-ს). თუ რამდენიმე ხაზია მონიშნული, Tab და Shift + Tab ყველა მათგანზე იმოქმედებს.

შენიშვნა: თუ თამაშს Steam-ით თამაშობ, Shift + Tab ამის ნაცვლად Steam-ის ოვერლეის გახსნის. შეწევის მოხსნის მალსახმობი შეგიძლია შეცვალო თამაშის პარამეტრებში, Steam-ის ოვერლეის მალსახმობი კი Steam-ის ოვერლეის პარამეტრებში.

აქ `do_a_flip()` და `pet_the_piggy()` განმეორებით გამოიძახება, რადგან შეწეულ `while` ბლოკში არიან. `harvest()` კი არასდროს სრულდება, რადგან ამ ბლოკის შემდეგ მოდის.
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
    "operation_ticks": 200,
    "seed": 1,
    "exclude_unlocks": ["watering", "fertilizer"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
#CODE
while True:
	do_a_flip()
	pet_the_piggy()
harvest()
}}

---

[for ციკლი](docs/scripting/for.md)      [if ინსტრუქცია](docs/scripting/if.md)      [break ინსტრუქცია](docs/scripting/break.md)      [continue ინსტრუქცია](docs/scripting/continue.md)      [გარე რედაქტორი](docs/external_editor.md)
