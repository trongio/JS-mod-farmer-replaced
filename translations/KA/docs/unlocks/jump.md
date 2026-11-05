[<- ქვანახშირი](docs/unlocks/coal.md)
---
# გადახტომა
შენმა დრონმა `jump()` ბრძანება განბლოკა. გამოყენებისას არგუმენტად გადაეცი განბლოკვა, მაგალითად `Unlocks.Iron`.

ეს ბრძანება საშუალებას გაძლევს, აირჩიო მიწაში დამალული გარკვეული რესურსები და მყისიერად ჩახტე მათთან. განსაკუთრებით გამოსადეგია გამართვისთვის ან იმ მადნის ძარღვზე გადასახტომად, რომელიც ახლახან გამოგრჩა.

გადახტომის შემდეგ სამიზნე რესურსი ბლოკების ზედა ზედაპირიდან მაქსიმუმ 3 ბლოკის დაშორებით უნდა იყოს. `jump()`-ის გამოყენება პროგრამის ერთ შესრულებაზე მხოლოდ ერთხელ შეიძლება, შემდგომი მცდელობები შეცდომას გამოიწვევს.

ქვემოთ მოცემული მაგალითი ჩახტება [ბრინჯთან](docs/unlocks/rice.md), იპოვის თიხის ფენას და დარგავს ბრინჯის ტერასების ერთ რიგს:
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
    "items": [{"item": "hay", "n": 10}],
    "world_size": {"x": 4, "y": 4},
    "execution_speed": 1,
    "digging_speed": 4,
    "action_ticks": 200,
    "operation_ticks": 20,
    "seed": 1,
    "exclude_unlocks": ["watering", "fertilizer", "mushrooms"],
    "starting_chunk": 0
}
#CODE
# Jump close to the rice resource
jump(Unlocks.Rice)

# Plant a row of rice terraces
for i in range(get_world_size()):
    # Jump is not exact, dig away the last blocks
    while get_ground_type() != Grounds.Clay:
        dig()

    plant(Entities.Rice)

    move(East)
}}

## თავსებადი განბლოკვები
`jump()` მხოლოდ ამ სამთო განბლოკვებთან მუშაობს:
- Coal
- Iron
- Quartz
- Rice
- Petrified_Pumpkins
- Special_Soils
- Pyramid
- Treasure_Map
- Mushrooms
- Dynamite

---

[jump()](functions/jump)
