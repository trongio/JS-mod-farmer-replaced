[<- სამთო საქმე](docs/unlocks/mining.md)
---
# მიწისქვეშა გრძნობები
მოდი, რამდენიმე სენსორი დავამატოთ, რომ შენმა დრონმა მიწისქვეშეთში გზა გაიკვლიოს.

## Z-პოზიცია
ახლა `get_pos_z()`-ით შეგიძლია დრონის სიმაღლე გაიგო (ის 0-დან იწყება და დრონის ჩასვლისას უარყოფითი ხდება).

{{codeexample 
{
    "camera_position": {"x": -1.5, "y": 0, "z": 10},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": true,
    "show_output": false,
    "collapsing": true,
    "autoplay": false,
    "exclude_unlocks": ["watering", "fertilizer"],
    "items": [],
    "world_size": {"x": 5, "y": 5},
    "execution_speed": 2,
    "action_ticks": 200,
    "operation_ticks": 200,
    "seed": 1
}
#SETUP
move(North)
move(North)
move(East)
move(East)
#CODE
print(get_pos_z())
dig()
dig()
dig()
print(get_pos_z())
}}

## მიწის ტიპი
`get_ground_type()` აბრუნებს დრონის ქვეშ მყოფი მიწის ტიპს. მეზობელი უჯრის მიწის ტიპის გასაგებად შეგიძლია მიმართულების არგუმენტი გადასცე, მაგალითად `get_ground_type(North)`.

აი ასე შეამოწმებ, ტალახიანი მიწაა თუ არა დრონის ქვეშ მყოფი ბლოკი:

{{codeexample 
{
    "camera_position": {"x": -1.5, "y": 0, "z": 8},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": true,
    "show_output": false,
    "collapsing": true,
    "autoplay": false,
    "items": [],
    "exclude_unlocks": ["watering", "fertilizer"],
    "world_size": {"x": 4, "y": 4},
    "execution_speed": 2,
    "action_ticks": 200,
    "operation_ticks": 200,
    "seed": 1
}
#CODE
dig()
if get_ground_type() == Grounds.Dirt:
    do_a_flip()
}}

## სიმაგრე და მდგრადობა
`get_hardness()` აბრუნებს დრონის ქვეშ მყოფი მიწის უჯრის სიმაგრეს. რაც უფრო მაგარია უჯრა, მით უფრო დიდხანს სჭირდება თხრა. შეგიძლია მიმართულების არგუმენტიც გადასცე, მაგალითად `get_hardness(North)`.

`get_stability()` აბრუნებს დრონის ქვეშ მყოფი მიწის უჯრის მდგრადობას. მდგრადობა 1 ნიშნავს, რომ ბლოკი ჩამონგრევამდე უძლებს 1 სიმაღლის სხვაობას. შეგიძლია მიმართულების არგუმენტიც გადასცე, მაგალითად `get_stability(North)`.

{{codeexample 
{
    "camera_position": {"x": -1, "y": 0, "z": 10},
    "show_image": true,
    "image_size": {"x": 800, "y": 250},
    "show_code": true,
    "show_inventory": true,
    "show_output": false,
    "collapsing": true,
    "autoplay": false,
    "items": [],
    "exclude_unlocks": ["watering", "fertilizer"],
    "world_size": {"x": 5, "y": 5},
    "execution_speed": 2,
    "action_ticks": 200,
    "operation_ticks": 20,
    "seed": 0
}
#SETUP
jump(Unlocks.Rice)
#CODE
print(get_hardness(), "|", get_stability())
while get_hardness() == 1:
    dig()

print(get_hardness(), "|", get_stability())
while get_hardness() == 2 and get_stability() == 3:
    dig()
}}

გაითვალისწინე, რომ როცა დრონი ქვემოთ თხრის, მის პირდაპირ გვერდით მყოფი ოთხი ბლოკი მდგრადობის მიუხედავად ქრება, თუ ბლოკს განსაკუთრებული ფუნქცია არ აქვს. მაგალითად, თიხა, რკინა და კვარცი ამ წესით არ ქრება. თუმცა მდგრადობის ნაკლებობის გამო მათი ჩამონგრევა ისევ შესაძლებელია.
---

[სამთო საქმე](docs/unlocks/mining.md)      [გრძნობები](docs/unlocks/senses.md)

[get_pos_z()](functions/get_pos_z)      [get_ground_type()](functions/get_ground_type)      [get_hardness()](functions/get_hardness)      [get_stability()](functions/get_stability)
