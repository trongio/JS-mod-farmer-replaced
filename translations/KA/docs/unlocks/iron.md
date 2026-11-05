[<- ქვანახშირი](docs/unlocks/coal.md) <right>[კვარცი ->](docs/unlocks/quartz.md)
<right>[რკინის ძიება ->](docs/unlocks/prospecting.md)
<right>[განძის რუკა ->](docs/unlocks/treasure_map.md)
---
# რკინა
`Grounds.Dirt`-ში ქვანახშირის პოვნის შემდეგ შეგიძლია უფრო ღრმად ჩათხარო, `Grounds.Clay` ფენა გაიარო და `Grounds.Rock`-ში რკინა იპოვო.

ქვანახშირის ძარღვებისგან განსხვავებით, რკინის ძარღვები ვერტიკალურადაც ვრცელდება. მაგრამ ძარღვში რკინის ყველა ბლოკი ყოველთვის უშუალოდ ეხება ერთმანეთს, ბლოკები მხოლოდ დიაგონალურად არასდროს უერთდება ერთმანეთს.

თუმცა რკინის ძარღვები თავიდან პატარაა და მათ საპოვნელად ცოტა იღბალი დაგჭირდება. რკინის შემდგომი განბლოკვა ძარღვის მაქსიმალურ ზომას გაზრდის და შემთხვევით პოვნასაც ცოტათი გაგიადვილებს. მაგრამ თუ საიმედოდ გინდა მათი პოვნა, [რკინის ძიება](docs/unlocks/prospecting.md) უნდა განბლოკო.

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
    "items": [{"item": "hay", "n": 100}, {"item": "wood", "n": 100}],
    "world_size": {"x": 5, "y": 5},
    "execution_speed": 1,
    "digging_speed": 10,
    "action_ticks": 200,
    "operation_ticks": 50,
    "seed": 5,
    "exclude_unlocks": ["watering", "fertilizer", "mushrooms", "quartz"]
}
#SETUP
#CODE
# Dig until an iron block appears
while get_ground_type() != Grounds.Iron:
    dig()

# Mine the iron
dig()
}}

---

[სტატისტიკა](docs/stats.md)      [სამთო საქმე](docs/unlocks/mining.md)      [რკინის ძიება](docs/unlocks/prospecting.md)      [მიწისქვეშა გრძნობები](docs/unlocks/underground_senses.md)

[move()](functions/move)      [dig()](functions/dig)
