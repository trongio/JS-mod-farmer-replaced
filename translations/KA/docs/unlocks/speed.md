[<- while ციკლი](docs/scripting/while.md) <right>[გაფართოება 1 ->](docs/unlocks/expand_1.md)
<right>[დარგვა ->](docs/unlocks/plant.md)
---
# სიჩქარის გაუმჯობესება
შესრულების სიჩქარე გაორმაგდა. პრობლემა ისაა, რომ დრონი ახლა უფრო სწრაფად კრეფს, ვიდრე ბალახი იზრდება, ამიტომ მოსავალი საერთოდ არ გაქვს. ამის მოსაგვარებლად ახლა განბლოკილია [if ინსტრუქცია](docs/scripting/if.md) განშტოებები და [can_harvest()](functions/can_harvest) ფუნქცია.

## შემოწმება მოკრეფამდე
`if` ინსტრუქცია თავის კოდის ბლოკს ერთხელ ასრულებს, თუ მოცემული პირობა `True`-ა.

ახალი ფუნქცია `can_harvest()` სასარგებლო პირობას გვაძლევს. `can_harvest()` აბრუნებს `True`-ს, თუ დრონის ქვეშ მყოფი მცენარის მოკრეფა შესაძლებელია, წინააღმდეგ შემთხვევაში კი `False`-ს.

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
do_a_flip()
#CODE
if can_harvest():
	do_a_flip()
}}

დაბრუნებული მნიშვნელობა ასე წარმოიდგინე: ფუნქციის გამოძახების გამოსახულება `can_harvest()` დაბრუნებული `True` მნიშვნელობით იცვლება, როცა `if` გამოითვლება.

რა ხდება, როცა ზემოთ მოცემული კოდი სრულდება:
- სრულდება `if` ინსტრუქცია.
- გამოიძახება `can_harvest()`.
- `can_harvest()` აბრუნებს `True`-ს, რადგან ბალახი სრულად გაიზარდა.
- ინსტრუქცია ახლა ასეთია: `if True:`.
- განშტოება სრულდება, რადგან მნიშვნელობა `True`-ა.

ბალახი სრულად რომ არ ყოფილიყო გაზრდილი, დრონი ტრიალს არ გააკეთებდა.

ახლა `if`-ის დახმარებით დრონი ნაადრევად აღარ მოკრეფს.

---

[if ინსტრუქცია](docs/scripting/if.md)      [while ციკლი](docs/scripting/while.md)

[harvest()](functions/harvest)      [can_harvest()](functions/can_harvest)
