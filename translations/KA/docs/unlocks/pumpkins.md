[<- ხეები](docs/unlocks/trees.md) <right>[პოლიკულტურა ->](docs/unlocks/polyculture.md)
<right>[კაქტუსი ->](docs/unlocks/cactus.md)
---
# გოგრები
[გოგრები](objects/pumpkin) სტაფილოსავით გადახნულ ნიადაგზე იზრდება. მათი დარგვა სტაფილო ღირს.

როცა კვადრატში ყველა გოგრა ბოლომდე გაიზრდება, ისინი ერთად გაერთიანდება და გიგანტურ გოგრად იქცევა. სამწუხაროდ, ბოლომდე გაზრდილი გოგრა 20%-იანი ალბათობით კვდება, ამიტომ მკვდრების ხელახლა დარგვა მოგიწევს, თუ გინდა, რომ გაერთიანდეს.

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
    "items": [{"item": "carrot", "n": 11}],
    "world_size": {"x": 3, "y": 3},
    "execution_speed": 5,
    "digging_speed": 1,
    "action_ticks": 200,
    "operation_ticks": 1,
    "seed": 3,
    "exclude_unlocks": ["watering", "fertilizer"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
#CODE
for i in range(get_world_size()):
	for j in range(get_world_size()):
		till()
		plant(Entities.Pumpkin)
		move(North)
	move(East)
do_a_flip()
do_a_flip()
move(North)
move(North)
plant(Entities.Pumpkin)
do_a_flip()
plant(Entities.Pumpkin)
do_a_flip()
do_a_flip()
do_a_flip()
do_a_flip()
harvest()
}}

როცა გოგრა კვდება, ტოვებს მკვდარ გოგრას, რომელიც მოკრეფისას არაფერს იძლევა. მის ადგილას ახალი მცენარის დარგვა მკვდარ გოგრას ავტომატურად აშორებს, ამიტომ მისი მოკრეფა საჭირო არ არის. `can_harvest()` მკვდარ გოგრებზე ყოველთვის აბრუნებს `False`-ს.

გიგანტური გოგრის მოსავლიანობა მის ზომაზეა დამოკიდებული.

1x1 გოგრა იძლევა `1*1*1 = 1` გოგრას.
2x2 გოგრა იძლევა `2*2*2 = 8` გოგრას `4`-ის ნაცვლად.
3x3 გოგრა იძლევა `3*3*3 = 27` გოგრას `9`-ის ნაცვლად.
4x4 გოგრა იძლევა `4*4*4 = 64` გოგრას `16`-ის ნაცვლად.
5x5 გოგრა იძლევა `5*5*5 = 125` გოგრას `25`-ის ნაცვლად.
`n`x`n` გოგრა იძლევა `n*n*6` გოგრას, როცა `n >= 6`.

კარგი იქნება, თუ გოგრებს მინიმუმ 6x6 ზომამდე გაზრდი, რომ სრული მამრავლი მიიღო.

ეს ნიშნავს, რომ კვადრატის ყველა უჯრაზე გოგრის დარგვის შემთხვევაშიც კი ერთ-ერთი გოგრა შეიძლება მოკვდეს და მეგა გოგრის გაზრდას ხელი შეუშალოს.

---

[სტატისტიკა](docs/stats.md)      [ოპერატორები](docs/scripting/operators.md)      [ცვლადები](docs/scripting/variables.md)      [გრძნობები](docs/unlocks/senses.md)

[harvest()](functions/harvest)      [can_harvest()](functions/can_harvest)      [plant()](functions/plant)      [move()](functions/move)      [till()](functions/till)
