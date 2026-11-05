[<- ბრინჯი](docs/unlocks/rice.md)
---
# გაქვავებული გოგრები
როგორც ჩანს, გოგრები მიწისქვეშაც გვხვდება. ისინი გამაგრდა და გაქვავდა, მაგრამ ჩვენი მიზნებისთვის ისევ გამოდგება. მათი ამოთხრა `dig()`-ით შეგიძლია და გოგრებს მიიღებ.

ასე გოგრებს მიწის ზემოთ მოყვანის გარეშეც იშოვი. თუ ფერმერობა გირჩევნია, გაქვავებული გოგრები შეგიძლია საერთოდ ყურადღება არ მიაქციო.

{{codeexample 
{
    "camera_position": {"x": -2, "y": 1, "z": 8},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": false,
    "show_inventory": true,
    "show_output": false,
    "collapsing": false,
    "autoplay": true,
    "items": [],
    "exclude_unlocks": ["mushrooms", "watering", "fertilizer", "pyramid", "dynamite", "quartz"],
    "world_size": {"x": 6, "y": 6},
    "execution_speed": 8,
    "digging_speed": 16,
    "action_ticks": 200,
    "operation_ticks": 1,
    "seed": 3
}
#SETUP
jump(Unlocks.Petrified_Pumpkins)
move(East)
move(North)
move(North)
move(North)
#CODE
while get_ground_type() != Grounds.Petrified_Pumpkin:
    dig()
z = get_pos_z()
for i in range(get_world_size()):
    for j in range(get_world_size()):
        while get_pos_z() > z - 2 and get_ground_type() != Grounds.Petrified_Pumpkin:
            dig()
        move(North)
    move(East)
for _ in range(3):
    dig()
    move(East)
    dig()
    move(South)
    dig()
    move(West)
    dig()
    move(West)
    dig()
    move(North)
    dig()
    move(North)
    dig()
    move(East)
    dig()
    move(East)
    dig()
    move(South)
    move(West)
}}

გაქვავებული გოგრები მიწისქვეშ 3x3x3 ან 5x5x5 ბლოკებად გვხვდება. მათი პოვნის გარანტირებული გზა არ არსებობს, ამიტომ მხოლოდ იმის იმედი უნდა გქონდეს, რომ დრონი ერთ-ერთს გადააწყდება, მაგრამ ისინი ქვისა და რკინის ფენის ქვემოთ, მაგარ ტალახიან მიწაში გვხვდება, დაახლოებით იმავე სიმაღლეზე, სადაც კვარცი (თუ განბლოკილი გაქვს).

---

[სტატისტიკა](docs/stats.md)      [სამთო საქმე](docs/unlocks/mining.md)      [მიწისქვეშა გრძნობები](docs/unlocks/underground_senses.md)

[move()](functions/move)      [dig()](functions/dig)      [get_ground_type()](functions/get_ground_type)
