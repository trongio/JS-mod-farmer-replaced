[<- სამთო საქმე](docs/unlocks/mining.md) <right>[ბამბუკი ->](docs/unlocks/bamboo.md)
<right>[გაქვავებული გოგრები ->](docs/unlocks/petrified_pumpkins.md)
<right>[პერლიტი და თიხნარი ->](docs/unlocks/special_soils.md)
---
# ბრინჯი
ზედაპირისა და `Grounds.Dirt` ფენის ქვემოთ `Grounds.Clay`-ის თხელ ფენას იპოვი. როგორც აღმოჩნდა, ეს ნაყოფიერი და წყლით გაჟღენთილი მიწა ბრინჯის ჩითილების დასარგავად იდეალურია.

შეიძლება ეს კოდი გამოგადგეს, რომ თიხამდე ჩათხარო და ბრინჯი დარგო:
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
    "items": [],
    "exclude_unlocks": ["watering", "fertilizer"],
    "world_size": {"x": 4, "y": 4},
    "execution_speed": 8,
    "digging_speed": 4,
    "action_ticks": 200,
    "operation_ticks": 200,
    "seed": 1
}
#SETUP
move(East)
#CODE
while get_ground_type() != Grounds.Clay:
    dig()
plant(Entities.Rice)
}}

სანამ დრონი `Grounds.Clay` ბლოკის თავზეა, შეგიძლია `plant(Entities.Rice)`-ით ბრინჯის ჩითილები თიხაში დარგო, მერე `can_harvest()`-ით შეამოწმო, სრულად გაიზარდა თუ არა ბრინჯი, და ბოლოს `harvest()`-ით ბრინჯი მიიღო.

## ტერასები
როგორც კი ბრინჯს დარგავ, მიწა `Grounds.Rice_Terrace`-ად იქცევა, რომ ბრინჯის დასამწიფებლად იდეალური გარემო შექმნას. როცა ბრინჯი სრულად გაიზრდება, მიწა გაშრება, `Grounds.Dry_Rice_Terrace`-ად იქცევა და მასზე ბრინჯის დარგვა აღარ შეიძლება. საბედნიეროდ, ფენა რამდენიმე ბლოკის სისქისაა, ამიტომ შეგიძლია ბლოკი `dig()`-ით ამოთხარო და ქვემოთ მდებარე `Grounds.Clay`-ზე ახალი ბრინჯი დარგო. და თუ ფენა ამოიწურება, ყოველთვის შეგიძლია სამყარო `clear()`-ით გაასუფთავო და ახალ ფენამდე ჩათხარო.

---

[სტატისტიკა](docs/stats.md)      [სამთო საქმე](docs/unlocks/mining.md)      [მიწისქვეშა გრძნობები](docs/unlocks/underground_senses.md)      [if ინსტრუქცია](docs/scripting/if.md)      [for ციკლი](docs/scripting/for.md)

[harvest()](functions/harvest)      [plant()](functions/plant)      [move()](functions/move)      [dig()](functions/dig)      [get_ground_type()](functions/get_ground_type)
