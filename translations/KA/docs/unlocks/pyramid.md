[<- ბამბუკი](docs/unlocks/bamboo.md)
---
# პირამიდები
მიწისქვეშ ახლა უძველესი პირამიდული ნაგებობებია, რომლებიდანაც შეგიძლია ენერგია მოიპოვო და დრონი დააჩქარო!

პირამიდები ქვიშის ბლოკებისგან (`Grounds.Sand`) შედგება. თავად პირამიდის ქვემოთ კირქვის (`Grounds.Limestone`) საძირკვლის ფენაა. რადგან ისინი ძველი და გამოფიტულია, მათ მხოლოდ ნაწილობრივ დანგრეულ მდგომარეობაში იპოვი. საძირკვლის ფენა ყოველთვის ადგილზეა, მაგრამ ქვიშაში ხვრელები იქნება.

აი, როგორ გამოიყურება პირამიდის ნანგრევები, როცა ამოთხრი:
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
    "items": [{"item": "hay", "n": 100}, {"item": "block", "n": 100}],
    "world_size": {"x": 6, "y": 6},
    "execution_speed": 32,
    "digging_speed": 16,
    "action_ticks": 200,
    "operation_ticks": 200,
    "seed": 7,
    "exclude_unlocks": ["watering", "fertilizer", "mushrooms", "dynamite", "quartz"]
}
#SETUP
jump(Unlocks.Pyramid)
move(North)
#CODE
# Dig down to find a pyramid foundation
while get_ground_type() != Grounds.Limestone:
    dig()

z = get_pos_z() # Remember the depth of the foundation

# Dig away everything that's not part of the pyramid
for i in range(6):
    for j in range(6):
        while (get_ground_type() != Grounds.Limestone
                and get_ground_type() != Grounds.Sand
                and get_pos_z() > z):
            dig()
        move(North)
    move(East)
}}

## პირამიდის აღდგენა
როგორც ხედავ, ქვიშაში რამდენიმე ხვრელია, რომლებიც უნდა ამოივსოს. როცა ამ ხვრელებს ამოავსებ და პირამიდას პირვანდელ მდგომარეობაში აღადგენ, ის თავის თავს ანადგურებს და კირქვის საძირკვლის შუაში დიდი მზესუმზირა ჩნდება. მოკრიფე ის და მიიღებ `Items.Power`-ს. რაც უფრო დიდ პირამიდას დაასრულებ, მით მეტ ენერგიას მიიღებ.

პირამიდის აღდგენა ბლოკების განთავსებით შეგიძლია, ბრძანებით `place(Grounds.Sand)`. ქვიშა განსაკუთრებული ბლოკია, რომელიც მაშინვე ჩამოინგრევა, თუ სათანადო საყრდენი არ აქვს. ქვიშის ბლოკი ყოველთვის ან პირდაპირ კირქვაზე უნდა განათავსო, ან ქვიშის ბლოკების 3x3 ფენაზე. აი, პატარა, ხელით აშენებული პირამიდა საჩვენებლად. რა თქმა უნდა, მისი აშენება ენერგიას არ მოგცემს:

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
    "items": [{"item": "hay", "n": 100}, {"item": "block", "n": 100}],
    "world_size": {"x": 3, "y": 3},
    "execution_speed": 4,
    "digging_speed": 4,
    "action_ticks": 200,
    "operation_ticks": 200,
    "seed": 9,
    "exclude_unlocks": ["watering", "fertilizer", "mushrooms", "coal", "quartz"]
}
#CODE
# We need a limestone foundation to put sand on
for i in range(3):
    for j in range(3):
        place(Grounds.Limestone)
        move(North)
    move(East)

# Put a 3x3 layer of sand on the limestone
for i in range(3):
    for j in range(3):
        place(Grounds.Sand)
        move(North)
    move(East)

# Try to put another 3x3 layer
# Note that only the middle block is supported and remains
for i in range(3):
    for j in range(3):
        place(Grounds.Sand)
        move(North)
    move(East)
}}

სწორად აღდგენილი პირამიდა შედგება კირქვის კვადრატული საძირკვლისგან, რომლის გვერდის სიგრძე კენტი რიცხვია, მაგალითად 5x5. მის ზემოთ იმავე ზომის (5x5) ქვიშის ბლოკების ფენაა, შემდეგ კი თანდათან უფრო პატარა ფენები: 3x3, მერე 1x1.

პირამიდის ზემოთ ბლოკები ყოველთვის ტალახიანი მიწაა. თუ პირამიდის ეფექტურად ამოთხრა გინდა, ეს ფაქტი გამოიყენე.

## ენერგია
სანამ ენერგია გაქვს, დრონი მას იყენებს, რომ ორჯერ უფრო სწრაფად იმუშაოს.
ის 1 ენერგიას ხარჯავს ყოველ 30 მოქმედებაზე, მაგალითად გადაადგილებაზე, მოკრეფაზე ან დარგვაზე.
კოდის სხვა ინსტრუქციების შესრულებამაც შეიძლება დახარჯოს ენერგია, მაგრამ გაცილებით ნაკლები, ვიდრე დრონის მოქმედებებმა.

ზოგადად, ყველაფერი, რასაც სიჩქარის გაუმჯობესებები აჩქარებს, ენერგიითაც ჩქარდება.
ყველაფერი, რაც ენერგიით ჩქარდება, ენერგიასაც ხარჯავს მისი შესრულების დროის პროპორციულად, სიჩქარის გაუმჯობესებების გაუთვალისწინებლად.

ენერგია `dig()`-ზე არ მოქმედებს.

---

[სტატისტიკა](docs/stats.md)      [მიწისქვეშა გრძნობები](docs/unlocks/underground_senses.md)

[move()](functions/move)      [dig()](functions/dig)      [use_item()](functions/use_item)      [place()](functions/place)
