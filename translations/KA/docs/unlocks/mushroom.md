[<- კვარცი](docs/unlocks/quartz.md) <right>[დინამიტი ->](docs/unlocks/dynamite.md)
---
# სოკო
სოკოს ბევრი სახეობა მიწისქვეშ კოლონიებად იზრდება. თხრისას ეძებე `Grounds.Mushroom` ფენა. შემდეგ შეგიძლია მიწა `measure()`-ით გაზომო და სოკოს ტიპს რიცხვის სახით მიიღებ, `0`-დან `5`-მდე.

ეს კოდი პოულობს რამდენიმე სოკოს და ბეჭდავს მათ ტიპს:
{{codeexample 
{
    "camera_position": {"x": -1, "y": 0, "z": 6},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": true,
    "show_output": false,
    "collapsing": true,
    "autoplay": false,
    "items": [],
    "world_size": {"x": 4, "y": 4},
    "execution_speed": 2,
    "digging_speed": 6,
    "action_ticks": 200,
    "operation_ticks": 200,
    "seed": 9,
    "exclude_unlocks": ["watering", "fertilizer", "dynamite", "iron"]
}
#SETUP
jump(Unlocks.Mushrooms)
#CODE
# Process a row of mushrooms
for i in range(get_world_size()):
    # Dig down to the top mushroom block
    while get_ground_type() != Grounds.Mushroom:
        dig()

    # Print the mushroom type number
    print(measure())

    move(East)
}}

## როგორ შევაგროვოთ სოკო
ერთი ტიპის სოკოებს ერთად ყოფნა უყვართ, მაგრამ ცოტა მეტისმეტად მორცხვები არიან იმისთვის, რომ ერთმანეთზე გაიზარდონ. დრონის ქვეშ მყოფი სოკოს ბლოკი `push(direction)`-ით გადაწიე.

ერთი ტიპის ორი სოკოს ბლოკი ერთმანეთზე დააწყვე და ბლოკები ავტომატურად მოიკრიფება, შენ კი სოკოს მიიღებ.

ქვემოთ მოცემული კოდი ადგილს ათავისუფლებს და შემდეგ სოკოს გადაწევს, სანამ ქვემოთ მყოფ სოკოს არ დაემთხვევა:
{{codeexample 
{
    "camera_position": {"x": -1, "y": 0, "z": 6},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": true,
    "show_output": false,
    "collapsing": true,
    "autoplay": false,
    "items": [],
    "world_size": {"x": 4, "y": 4},
    "execution_speed": 2,
    "digging_speed": 6,
    "action_ticks": 200,
    "operation_ticks": 200,
    "seed": 3,
    "exclude_unlocks": ["watering", "fertilizer", "dynamite", "iron", "special_soils"]
}
#SETUP
jump(Unlocks.Mushrooms)
#CODE
while get_ground_type() != Grounds.Mushroom:
    dig()

depth = get_pos_z()
for i in range(2):
    # Make space to push the mushroom block east
    move(East)
    while get_pos_z() > depth - 1:
        dig()
    # Move back, push, and follow the pushed block
    move(West)
    push(East) # Push the mushroom!
    move(East)
}}

# ბლოკების გადაწევა
`can_push(direction)`-ით შეამოწმებ, შეიძლება თუ არა დრონის ქვეშ მყოფი ბლოკის გადაწევა და ხომ არაფერი უშლის ხელს მითითებული მიმართულებით.

`push(direction)` ბლოკს გადაწევს და აბრუნებს, წარმატებით დასრულდა თუ არა გადაწევა.

მხოლოდ სოკოს ბლოკების გადაწევა შეიძლება, ბლოკებს ზემოთ ვერ გადაწევ და გადაწევა ვერ მოხერხდება, თუ გზაზე სხვა ბლოკია. ჰაერში გადაწეული ბლოკები ვარდება და ქვემოთ პირველივე ბლოკზე ჩერდება.

# სტრატეგია
<spoiler=მინიშნება>
გახსოვდეს, რომ `place(Grounds.Dirt)`-ის გამოძახებით დრონის ქვეშ ბლოკებს განათავსებ. ეს ხვრელების ამოსავსებად გამოგადგება, რომ მათ ზემოდან ბლოკების გადაწევა შეძლო.
</spoiler>

---

[სტატისტიკა](docs/stats.md)      [მიწისქვეშა გრძნობები](docs/unlocks/underground_senses.md)      [ლექსიკონები](docs/scripting/dicts.md)

[move()](functions/move)      [measure()](functions/measure)      [can_push()](functions/can_push)      [push()](functions/push)      [place()](functions/place)      [get_ground_type()](functions/get_ground_type)
