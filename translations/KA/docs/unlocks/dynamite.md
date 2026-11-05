[<- სოკო](docs/unlocks/mushroom.md) <right>[ფეთქებადი ბურღი ->](docs/unlocks/explosive_drill.md)
---
# დინამიტი
რევოლუციური ასაფეთქებელი, რომელიც წინა სამთო ექსპედიციებს დარჩათ. სამწუხაროდ, შენი ბურღი იდეალურია დინამიტის ასაფეთქებლად, რაც გარშემო ყველა ბლოკს ანადგურებს. მოპოვებული დინამიტი შეგიძლია გამოიყენო `use_item(Items.Dynamite)`-ით და ის დრონის ქვეშ მაშინვე აფეთქდება.

დინამიტის უსაფრთხოდ მოსაპოვებლად იპოვე `Grounds.Dynamite`-ის ფენა, რომლის ქვემოთაც პირდაპირ `Grounds.Soot`-ის ფენაა. ზედა დინამიტის ფენა ცოტა დაძველებულია და მისი ზოგი ბლოკი თხრისთვის უსაფრთხოა. თუმცა ზოგი ბლოკი ჯერ კიდევ აქტიურია და თხრისას აფეთქდება.

ეს მაგალითი თხრის დინამიტში და საწყისი თხრის წერტილის გარშემოც, აჩენს ჭვარტლის ფენას მასზე დაწერილი რიცხვებით და ბეჭდავს გაზომვის შედეგს:
{{codeexample 
{
    "camera_position": {"x": -3, "y": 0, "z": 15},
    "show_image": true,
    "image_size": {"x": 800, "y": 500},
    "show_code": true,
    "show_inventory": true,
    "show_output": false,
    "collapsing": true,
    "autoplay": false,
    "items": [],
    "world_size": {"x": 7, "y": 7},
    "execution_speed": 2,
    "digging_speed": 2,
    "action_ticks": 200,
    "operation_ticks": 200,
    "seed": 6,
    "exclude_unlocks": ["watering", "fertilizer", "mushrooms", "coal", "quartz", "petrified_pumpkins"]
}
#SETUP
jump(Unlocks.Dynamite)
for _ in range(3):
    move(East)
    move(North)
#CODE
# Dig down to the soot layer, through the dynamite layer above
while get_ground_type() != Grounds.Soot:
    dig()

# List of directions that make the drone move in a circle
circle_directions = [
    East, South, West, West, 
    North, North, East, East
]
for dir in circle_directions:
    move(dir)
    dig()
    print(measure()) # The number on the soot block
}}

## დინამიტის მაძიებელი
შეძლებ ყველა ფუჭი დინამიტის ბლოკის პოვნას და ამოთხრას ისე, რომ მხოლოდ აქტიური ბლოკები დარჩეს? ყოველი აქტიური ბლოკი, რომელიც მხოლოდ სხვა აქტიური ბლოკებით ან ჭვარტლის ბლოკებითაა გარშემორტყმული, დინამიტის ფენის გაქრობისას დამატებით დინამიტს მოგცემს.

საბედნიეროდ, ქვემოთ მდებარე ჭვარტლის ბლოკები დაგეხმარება: მათ შეუძლიათ დაადგინონ, რამდენი მეზობელი დინამიტის ბლოკი შეიცავს აქტიურ დინამიტს. ზუსტად რომელი ბლოკებია აქტიური, ამას არ გეტყვიან, მხოლოდ ჯამურ რაოდენობას, ამიტომ რამდენიმე ჭვარტლის ბლოკის ჩვენებები უნდა შეაჯერო და თავად გამოიანგარიშო.

`measure()`-ის გამოძახება ჭვარტლის ბლოკზე აბრუნებს ნაღმების რაოდენობას ზედა დინამიტის ფენის რვა მეზობელ უჯრაში. რიცხვი შეიძლება იყოს `0`-დან (აქტიური ნაღმი არ არის) `8`-მდე (ყველა მეზობელი აქტიურ ნაღმს შეიცავს).

დინამიტის ფენაში პირველი თხრა ყოველთვის ფუჭ ბლოკზე მოდის. დინამიტის ყოველი ამოთხრილი ბლოკი ცოტა დინამიტს იძლევა. როცა დინამიტის ფენა ნადგურდება, ყველა ფუჭი ბლოკის წარმატებით ამოთხრით ან აქტიურ დინამიტში შემთხვევით ჩათხრით, დამატებით მიიღებ იმდენ დინამიტს, რამდენიც ბოლომდე გამოჩენილი აქტიური ბლოკების რაოდენობის კვადრატია.

## ფენების აგებულება
აქტიური ნაღმების რაოდენობა დინამიტის ფენაში, და შესაბამისად მათი პოვნის სირთულეც, სიღრმესთან ერთად იზრდება. ნაღმების საერთო რაოდენობა ფერმის ზომასთან ერთად იზრდება, ასევე იზრდება დინამიტის ფენის გასუფთავებით მიღებული მაქსიმალური მოსავლიანობაც.

გააუმჯობესე დინამიტი, რომ გაიზარდოს დინამიტის ბლოკების თხრითა და აქტიური ბლოკების ბოლომდე გამოჩენით მიღებული მოსავლიანობა. გაუმჯობესება დინამიტის აფეთქებების ენერგიასაც 30%-ით ზრდის.

## წარმატებისა და წარუმატებლობის დადგენა
თუ აქტიურ დინამიტში ჩათხრი, ორივე ფენა აფეთქდება. ამის შემოწმება შეგიძლია `get_ground_type()`-ით `Grounds.Dynamite`-ში თხრის შემდეგ. თუ მიწა `Grounds.Soot` არ არის, თავსატეხი ვერ ამოხსენი. მეორე მხრივ, თუ ბოლო ფუჭ დინამიტის ბლოკსაც ამოთხრი, ყველა დინამიტის ბლოკი გაქრება და თავსატეხისთვის მაქსიმალურ მოსავლიანობას მიიღებ. ჭვარტლის ფენა რჩება, მაგრამ `measure()` აბრუნებს `None`-ს, რაც ნიშნავს, რომ თავსატეხი წარმატებით ამოიხსნა.

`# Dig into a dynamite block and check the puzzle state
def dig_dynamite():
    dig()
    if get_ground_type() != Grounds.Soot:
        # Dug into live dynamite, puzzle failed
        return False
    elif measure() == None:
        # Dug up final dud block, puzzle solved
        return True
    else:
        # Got a count from measuring, puzzle still ongoing
        return None
`

## სტრატეგია
<spoiler=მინიშნება>
ქვემოთ მოცემული კოდი თხრის დინამიტის ფენაში და ეძებს ჭვარტლის ბლოკებს, რომლებიც `0`-ს აჩვენებს. ყოველი ასეთი ბლოკისთვის ის გარშემო არსებულ ყველა დინამიტის ბლოკს ამოთხრის:
{{codeexample 
{
    "camera_position": {"x": -5, "y": 0, "z": 15},
    "show_image": true,
    "image_size": {"x": 800, "y": 500},
    "show_code": true,
    "show_inventory": true,
    "show_output": false,
    "collapsing": true,
    "autoplay": false,
    "items": [],
    "world_size": {"x": 7, "y": 7},
    "execution_speed": 4,
    "digging_speed": 8,
    "action_ticks": 200,
    "operation_ticks": 20,
    "seed": 6,
    "exclude_unlocks": ["watering", "fertilizer", "mushrooms", "coal", "quartz", "petrified_pumpkins"]
}
#SETUP
jump(Unlocks.Dynamite)
for _ in range(3):
    move(East)
    move(North)
#CODE
while get_ground_type() != Grounds.Soot:
    dig()

circle_directions = [
    East, South, West, West, North,
    North, East, East, South, West
]
# Go around the block we initially dug into
for dir in circle_directions:
    # If the soot measures 0,
    # we know all surrounding dynamite blocks are duds
    if get_ground_type() == Grounds.Soot and measure() == 0:
        # Do another circle through the surrounding blocks
        for dir2 in circle_directions:
            move(dir2)
            # Dig up the dynamite dud block
            while get_ground_type() != Grounds.Soot:
                dig()
    move(dir)
}}
</spoiler>

---

[სტატისტიკა](docs/stats.md)      [მიწისქვეშა გრძნობები](docs/unlocks/underground_senses.md)      [ლექსიკონები](docs/scripting/dicts.md)      [სიმრავლეები](docs/scripting/sets.md)

[move()](functions/move)      [dig()](functions/dig)      [measure()](functions/measure)      [use_item()](functions/use_item)
