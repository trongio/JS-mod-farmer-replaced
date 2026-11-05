[<- რკინა](docs/unlocks/iron.md) <right>[სოკო ->](docs/unlocks/mushroom.md)
---
# კვარცი
`Grounds.Rock`-ის რკინის ძარღვების ქვემოთ კვარცს კლდის ფენის ქვედა ნაწილში და მის ქვეშ მდებარე მაგარ `Grounds.Dirt`-ში იპოვი (სიმაგრე შეამოწმე `get_hardness()`-ით, რომელსაც [მიწისქვეშა გრძნობების](docs/unlocks/underground_senses.md) განბლოკვა გაძლევს).

ეს კოდი ტალახიან მიწას გაივლის და კლდის ფენაში ჩადის. იქ თხრისას განუწყვეტლივ ეძებს კვარცს, სანამ მანძილი 2-ზე ნაკლები არ გახდება. ბოლოს ბეჭდავს მანძილს, რომელზეც აღმოჩნდა.
{{codeexample 
{
    "camera_position": {"x": -1, "y": 0, "z": 8},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": true,
    "show_output": false,
    "collapsing": true,
    "autoplay": false,
    "items": [{"item": "block", "n": 1}, {"item": "iron", "n": 100}],
    "world_size": {"x": 3, "y": 3},
    "execution_speed": 8,
    "digging_speed": 16,
    "action_ticks": 200,
    "operation_ticks": 200,
    "seed": 9,
    "exclude_unlocks": ["watering", "fertilizer", "mushrooms", "dynamite"]
}
#SETUP
move(East)
move(North)
#CODE
# Dig down to the rock layer
while get_ground_type() != Grounds.Rock:
    dig()

# Continue digging until a quartz vein is close
while prospect_quartz() == None or prospect_quartz() >= 2:
    for i in range(10):
        dig()

# Print the final distance to the vein
print(prospect_quartz())
}}

## ძარღვის აგებულება
კვარცი ნემსისებრ ძარღვებად იზრდება, რომლებიც ვერტიკალურ სვეტებს ქმნის, ამიტომ მათი ცდისა და შეცდომის გზით პოვნა საკმაოდ რთულია.

## კვარცის საბადოს ძიება
საბედნიეროდ, ჩვენი რკინა შეგვიძლია მოვღუნოთ და მივიღოთ რაღაც მაძიებელი წკეპლის მსგავსი, რომლითაც კვარცის ამ ძარღვებს უფრო მარტივად მივაგნებთ. ამის ბრძანებაა `prospect_quartz()`.

`prospect_quartz()` განსხვავებულად მუშაობს, ვიდრე `prospect_iron()`. უახლოესი რკინის მადნისკენ მიმართულების ნაცვლად, ის აბრუნებს ზუსტ 3D მანძილს უახლოეს კვარცის ბლოკსა და დრონს შორის (ევკლიდურ მანძილს).

`prospect_quartz()`-ის შესრულება 1 რკინა ჯდება, ამიტომ ეს ბრძანება ზომიერად გამოიყენე.

თუ `prospect_quartz()` ახლომახლო კვარცს ვერ იპოვის, ან ძიებისთვის საკმარისი რკინა არ გაქვს, ის `None`-ს აბრუნებს.

## სტრატეგია
<spoiler=აჩვენე მინიშნება>შეგიძლია განმეორებით ეძებო და იმოძრაო, რომ ნახო, როგორ იცვლება მანძილი. ბრტყელ ზედაპირზე მანძილის შემცირების მცდელობა შეიძლება კვარცის ძარღვის ზემოთ მიგიყვანოს. `prospect_quartz()`-ის გამოძახებებისა და დახარჯული რკინის რაოდენობის შესამცირებლად შეგიძლია ორობითი ძიების მსგავსი სტრატეგია გამოიყენო.

უფრო რთულ სტრატეგიებში შეგიძლია 2D გეომეტრიით ორი ან მეტი წრეწირის გადაკვეთა იპოვო, ან რთული გზა აირჩიო და სრული ტრილატერაცია გააკეთო: გადაკვეთო სამი სფერო და ძარღვის ზუსტი 3D პოზიცია იპოვო.

გაითვალისწინე, რომ კვადრატული ფესვის ამოსაღებად ახარისხების ოპერატორი გამოდგება: მაგალითად, 16-ის კვადრატული ფესვისთვის გამოიყენე `16**0.5`. ტრიგონომეტრიისა და ვექტორებისთვის ჩაშენებული მათემატიკური ფუნქციები არ არსებობს, ისინი თვითონ უნდა დაწერო. 

ისიც გაითვალისწინე, რომ სიახლოვეს შეიძლება რამდენიმე ძარღვი იყოს, ამიტომ ერთმანეთთან ახლოს აღებული გაზომვები შეიძლება ერთსა და იმავე ძარღვზე არ მიუთითებდეს.</spoiler>

---

[სტატისტიკა](docs/stats.md)      [სამთო საქმე](docs/unlocks/mining.md)      [მიწისქვეშა გრძნობები](docs/unlocks/underground_senses.md)      [ცვლადები](docs/scripting/variables.md)      [ოპერატორები](docs/scripting/operators.md)

[move()](functions/move)      [dig()](functions/dig)      [get_ground_type()](functions/get_ground_type)      [prospect_quartz()](functions/prospect_quartz)
