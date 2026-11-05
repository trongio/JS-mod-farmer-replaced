[<- რკინა](docs/unlocks/iron.md)
---
# რკინის ძიება
ალბათ შეამჩნიე, რომ რკინის ძარღვების პოვნა ზოგჯერ რთულია. შეგიძლია აქეთ-იქით თხარო და შემთხვევით იპოვო, მაგრამ უკეთესი გზაც არსებობს: რკინა მაგნიტურია, ამიტომ მისი პოვნა შორიდანაც შეგვიძლია.

`prospect_iron()` ბრძანება სწორედ ამას აკეთებს. `prospect_iron()` აბრუნებს მიმართულებას (`North`, `East`, `South` ან `West`), რომლითაც დრონმა უნდა გადადგას ნაბიჯი უახლოესი რკინის მადნისკენ.

ბრძანების შესრულება 1 ქვანახშირი ჯდება.

`prospect_iron()` აბრუნებს `None`-ს, თუ დრონი უკვე უახლოესი რკინის მადნის პირდაპირ ზემოთაა, თუ ბრძანების შესასრულებლად საკმარისი ქვანახშირი არ გაქვს, ან თუ მოქმედების არეში რკინა არ არის.

ამ კოდით მომდევნო რკინის მადანს ერთი ნაბიჯით მიუახლოვდები:

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
    "items": [{"item": "coal", "n": 100}],
    "world_size": {"x": 5, "y": 5},
    "execution_speed": 1,
    "digging_speed": 10,
    "action_ticks": 200,
    "operation_ticks": 50,
    "seed": 8,
    "exclude_unlocks": ["watering", "fertilizer", "mushrooms", "quartz"]
}
#SETUP
jump(Unlocks.Iron)
#CODE
# Make sure prospect returns a direction
if prospect_iron() != None:
    move(prospect_iron()) # Move closer to the iron vein
}}

ამ კოდის ცვლადით ოპტიმიზაცია ღირს, რადგან ახლა ის `prospect_iron()`-ს ორჯერ იძახებს, რაც 2 ქვანახშირი ჯდება.

## მანძილის გამოთვლა
უახლოესი რკინის მადანი იმით განისაზღვრება, რამდენი ნაბიჯი უნდა გადადგას დრონმა. თუ რკინის მადანი დრონიდან 2 ბლოკით აღმოსავლეთით და 3 ბლოკით ჩრდილოეთითაა, ეს 5 მანძილად ითვლება, რადგან იქამდე მისასვლელად დრონს ხუთი ნაბიჯი დასჭირდება.

თხრაც ნაბიჯად ითვლება. ასე რომ, თუ რკინა ამასთან ერთად მიწის ქვეშ 7 ბლოკის სიღრმეზეა, ეს 12 მანძილად ითვლება, რადგან დრონს ხუთი ნაბიჯი დასჭირდება რკინის ზემოთ დასადგომად და შემდეგ შვიდი თხრა მასთან მისასვლელად.

---

[რკინა](docs/unlocks/iron.md)      [მიწისქვეშა გრძნობები](docs/unlocks/underground_senses.md)

[move()](functions/move)      [dig()](functions/dig)      [prospect_iron()](functions/prospect_iron)
