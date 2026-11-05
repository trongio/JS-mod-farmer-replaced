[<- გოგრები](docs/unlocks/pumpkins.md) <right>[დინოზავრები ->](docs/unlocks/dinosaurs.md)
---
# კაქტუსი
სხვა მცენარეების მსგავსად, [კაქტუსებიც](objects/cactus) ნიადაგზე იზრდება და ჩვეულებრივად იკრიფება.

თუმცა ისინი სხვადასხვა ზომისაა და წესრიგის საკმაოდ უცნაური შეგრძნება აქვთ.

თუ სრულად გაზრდილ კაქტუსს მოკრეფ და ყველა მეზობელი კაქტუსი დალაგებულია, ისინიც რეკურსიულად მოიკრიფება.

კაქტუსი დალაგებულად ითვლება, თუ `North` და `East` მხარეს ყველა მეზობელი კაქტუსი სრულად გაზრდილია და ზომით მასზე დიდია ან მისი ტოლია, ხოლო `South` და `West` მხარეს ყველა მეზობელი კაქტუსი სრულად გაზრდილია და ზომით მასზე პატარაა ან მისი ტოლია.

მოკრეფა მხოლოდ მაშინ გავრცელდება, თუ ყველა მოსაზღვრე კაქტუსი სრულად გაზრდილი და დალაგებულია.
ეს ნიშნავს, რომ თუ გაზრდილი კაქტუსების კვადრატი ზომის მიხედვით დალაგებულია და ერთ კაქტუსს მოკრეფ, მთელი კვადრატი მოიკრიფება.

სრულად გაზრდილი კაქტუსი ყავისფერია, სანამ დალაგებული არ არის. დალაგების შემდეგ ისევ გამწვანდება.

იმდენ კაქტუსს მიიღებ, რამდენიც მოკრეფილი კაქტუსების რაოდენობის კვადრატია. თუ ერთდროულად `n` კაქტუსს მოკრეფ, მიიღებ `n**2` `Items.Cactus`-ს.

კაქტუსის ზომას `measure()`-ით გაზომავ.
ის ყოველთვის ერთ-ერთია ამ რიცხვებიდან: `0,1,2,3,4,5,6,7,8,9`.

`measure(direction)`-ს მიმართულებაც შეგიძლია გადასცე, რომ დრონის იმ მხარეს მდებარე მეზობელი უჯრა გაზომო.

`swap()` ბრძანებით კაქტუსს ნებისმიერი მიმართულებით მის მეზობელთან გაუცვლი ადგილს.
`swap(direction)` დრონის ქვეშ მყოფ ობიექტს ადგილს უცვლის იმ ობიექტთან, რომელიც დრონიდან `direction` მიმართულებით ერთი უჯრის მოშორებითაა.

{{codeexample 
{
    "camera_position": {"x": -1.5, "y": 2.4, "z": 5},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": true,
    "show_output": true,
    "collapsing": true,
    "autoplay": false,
    "items": [{"item": "pumpkin", "n": 32}],
    "world_size": {"x": 4, "y": 4},
    "execution_speed": 1,
    "digging_speed": 1,
    "action_ticks": 200,
    "operation_ticks": 100,
    "seed": 1,
    "exclude_unlocks": ["fertilizer", "watering"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
for _ in range(4):
    for _ in range(4):
        till()
        plant(Entities.Cactus)
        move(East)
    move(North)
for _ in range(4):
    for _ in range(4):
        for _ in range(4):
            x,y = get_pos_x(), get_pos_y()
            if x > 0 and measure(West) > measure():
                swap(West)
            if y > 0 and measure(South) > measure():
                swap(South)
            if x < 3 and measure(East) < measure():
                swap(East)
            if y < 3 and measure(North) < measure():
                swap(North)
            move(East)
        move(North)
move(East)
move(North)
move(North)
swap(West)
swap(South)
swap(East)
swap(North)
#CODE
swap(North)
swap(East)
swap(South)
swap(West)
harvest()
}}

## მაგალითები რიცხვებით
თითოეულ ამ ბადეში ყველა კაქტუსი დალაგებულია და მოკრეფა მთელ მინდორზე გავრცელდება:
`3 4 5    3 3 3    1 2 3    1 5 9
2 3 4    2 2 2    1 2 3    1 3 8
1 2 3    1 1 1    1 2 3    1 3 4`

ამ ბადეში მხოლოდ ქვედა მარცხენა კაქტუსია დალაგებული, მოკრეფის გასავრცელებლად კი ეს საკმარისი არ არის:
`1 5 3
4 9 7
3 3 2`

<spoiler=აჩვენე მინიშნება 1>
თუ ყველა რიგი უკვე დამოუკიდებლად დალაგებულია, თითოეული სვეტის დამოუკიდებლად დალაგება რიგებს არ აურევს.
</spoiler>
<spoiler=აჩვენე მინიშნება 2>
დალაგების ბევრი ჭკვიანური და ცნობილი ალგორითმი არსებობს. თუ არ იცნობ მათ, შეგიძლია მოიძიო და დაფიქრდე, რომელი მოერგება ამ ამოცანას. გაითვალისწინე, რომ ყველა მათგანი აქ არ გამოდგება, რადგან ადგილების გაცვლა მხოლოდ მეზობელ კაქტუსებს შორის შეგიძლია.
</spoiler>
<spoiler=აჩვენე მინიშნება 3>
"ბუშტულა დალაგება" (bubble sort) ალბათ დალაგების ყველაზე მარტივი ალგორითმია. იდეა ისაა, რომ ელემენტებს ისევ და ისევ გადაუყვები და არასწორად განლაგებულ მეზობელ ელემენტებს ადგილებს უცვლი, სანამ ასეთი აღარ დარჩება.

კაქტუსებით ეს ასე გამოიყურება:
{{codeexample 
{
    "camera_position": {"x": -4, "y": 1, "z": 6},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": false,
    "show_inventory": true,
    "show_output": false,
    "collapsing": false,
    "autoplay": true,
    "items": [{"item": "pumpkin", "n": 18}],
    "world_size": {"x": 9, "y": 1},
    "execution_speed": 2,
    "digging_speed": 1,
    "action_ticks": 200,
    "operation_ticks": 1,
    "seed": 1,
    "exclude_unlocks": ["fertilizer", "watering"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
#CODE
for _ in range(9):
    till()
    plant(Entities.Cactus)
    move(East)
sorted = False
while not sorted:
    sorted = True
    for _ in range(9):
        move(East)
        if get_pos_x() > 0 and measure(West) > measure():
            swap(West)
            sorted = False
harvest()
}}
რა თქმა უნდა, ამ სტრატეგიის გაუმჯობესების ბევრი გზა არსებობს!
როცა ერთი რიგის დალაგებას მოახერხებ, მინიშნება 1-ის დახმარებით მთელ მინდორს დაალაგებ.
</spoiler>

---

[სტატისტიკა](docs/stats.md)

[harvest()](functions/harvest)      [plant()](functions/plant)      [move()](functions/move)      [till()](functions/till)      [swap()](functions/swap)      [measure()](functions/measure)
