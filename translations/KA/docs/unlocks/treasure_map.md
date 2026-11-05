[<- რკინა](docs/unlocks/iron.md)
---
# განძის რუკა
ლეგენდები მოგვითხრობს ძნელად მოსაპოვებელ განძზე, რომელიც დიდი ხნის წინ გარდაცვლილმა, გულმავიწყმა მეკობრემ დამალა, ან რაღაც ამის მსგავსზე. იქნებ განძისკენ მიმავალი გზა რუკაზე დაწერა და მერე დაკარგა? შეიძლება რუკა მაშინ დააგდო, როცა ლავის მდინარემ კუთხეში მიიმწყვდია, და ის ბაზალტის ფენის ქვეშ შემორჩა.

ბაზალტის ბლოკების გაზომვამ შეიძლება რამე გამოავლინოს!

<spoiler=მითხარი მეტი>
ყურადღება მიაქციე `Grounds.Basalt`-ის ფენას. სცადე ბაზალტის გაზომვა და ნახე, რას გეტყვის. მინიშნება შეიძლება განძის რუკამდე მიგიყვანოს, ხოლო რუკის გაზომვა აუცილებლად დაგიბრუნებს თავად განძამდე მიმავალ ბილიკს. მაგრამ შეძლებ კი რუკის მითითებების გაშიფვრას?
</spoiler>

<spoiler=მითხარი, როგორ>
კარგი, საქმე ასეა: იპოვე `Grounds.Basalt`-ის ფენა და პირდაპირ მის ქვეშ მყოფი `Grounds.Treasure_Map` ბლოკი. ბაზალტზე `measure()`-ით მიიღებ ქვემოთ მყოფი რუკის ბლოკის `(x, y)` პოზიციას.

რუკის ბლოკზე `measure()`-ის გამოძახებით მიიღებ განძის ბილიკს მიმართულებების ასოების სტრიქონის სახით. N, E, S და W შესაბამისად აღნიშნავს `North`, `East`, `South` და `West` მიმართულებებს, ხოლო D ნიშნავს "Down"-ს (ქვემოთ) ან "Dig"-ს (თხრა). ეს ასოები აღწერს `Grounds.Treasure_Path` ბლოკებისგან შემდგარ ბილიკს, რომელიც განძამდე მიდის.

{{codeexample 
{
    "camera_position": {"x": -1.5, "y": 0, "z": 6},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": true,
    "show_output": true,
    "collapsing": true,
    "autoplay": false,
    "items": [],
    "world_size": {"x": 4, "y": 4},
    "execution_speed": 2,
    "digging_speed": 10,
    "action_ticks": 200,
    "operation_ticks": 200,
    "seed": 3,
    "exclude_unlocks": ["watering", "fertilizer", "dynamite", "iron"]
}

#SETUP
jump(Unlocks.Treasure_Map)
#CODE
# Find the basalt layer
while get_ground_type() != Grounds.Basalt:
    dig()

# Measuring the basalt returns the position of the treasure map
map_pos = measure()

while get_pos_x() < map_pos[0]:
    move(East)
while get_pos_y() < map_pos[1]:
    move(North)

while get_ground_type() != Grounds.Treasure_Map:
    dig()

# Found the map, measure it to get the path
do_a_flip()
instructions = measure()
print("instructions: ", instructions)

# Dig into the map to start the treasure hunt
dig()
}}

სტრიქონს `for` ციკლით შეგიძლია გადაუყვე და ასო-ასო დაამუშაო:
{{codeexample 
{
    "camera_position": {"x": -1.5, "y": 0, "z": 6},
    "show_image": false,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": true,
    "show_output": true,
    "collapsing": true,
    "autoplay": false,
    "items": [],
    "world_size": {"x": 4, "y": 4},
    "execution_speed": 2,
    "digging_speed": 10,
    "action_ticks": 200,
    "operation_ticks": 200,
    "seed": 3,
    "exclude_unlocks": ["watering", "fertilizer", "dynamite", "iron"]
}
#SETUP
jump(Unlocks.Treasure_Map)
while get_ground_type() != Grounds.Basalt:
    dig()

map_pos = measure()

while get_pos_x() < map_pos[0]:
    move(East)
while get_pos_y() < map_pos[1]:
    move(North)

while get_ground_type() != Grounds.Treasure_Map:
    dig()
#CODE
# The map instructions are returned as a single string
instructions = measure()
# Use for-in to split the string into letters
for letter in instructions:
    print(letter)
}}

დრონი მთელი დროის განმავლობაში `Grounds.Treasure_Path` ბლოკების თავზე უნდა დარჩეს. თუ სხვა მიწაზე გადავა, ბილიკი გაწყდება და განძი დაიკარგება.

## მიზანი
ბილიკის ბოლოს `Grounds.Treasure_Goal` ბლოკია. თუ დრონს ბილიკიდან არ გადაუხვევია, მის თავზე `Entities.Underground_Treasure` გამოჩნდება. დრონს შეუძლია ზარდახშაზე გამოიძახოს `harvest()` და მისი ოქრო აიღოს.

## ჯილდო
ოქროს რაოდენობა განძის ბილიკის სიგრძის პროპორციულია. განძის რუკის განბლოკვის გაუმჯობესება ბილიკის სიღრმეს ზრდის, გაფართოების გაუმჯობესება კი მას გვერდებზე გადაადგილებისთვის მეტ ადგილს აძლევს. ორივე გაუმჯობესება ზრდის განძის ბილიკის სიგრძეს და ოქროს რაოდენობას, რომელსაც იპოვი.

## რამდენიმე დრონი
განძი იმ დრონმა უნდა იპოვოს, რომელმაც განძის რუკის ბლოკი ამოთხარა, და ისე, რომ ბილიკის ბლოკები არ დატოვოს.
</spoiler>

---

[სტატისტიკა](docs/stats.md)      [for ციკლი](docs/scripting/for.md)      [კორტეჟები](docs/scripting/tuples.md)      [მიწისქვეშა გრძნობები](docs/unlocks/underground_senses.md)

[harvest()](functions/harvest)      [move()](functions/move)      [dig()](functions/dig)      [measure()](functions/measure)
