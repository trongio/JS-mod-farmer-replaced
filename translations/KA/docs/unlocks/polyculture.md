[<- გოგრები](docs/unlocks/pumpkins.md)
---
# პოლიკულტურა
შეიძლება უკვე შეამჩნიე, რომ ზოგჯერ მცენარეები ერთად დარგვისას მეტ მოსავალს იძლევა.
ბალახი, ბუჩქები, ხეები და სტაფილო მეტ მოსავალს იძლევა, როცა სათანადო თანამდგომი მცენარე აქვს. თანამდგომის არჩევანი ყოველ ცალკეულ მცენარეს თავისი აქვს და წინასწარ ვერ გამოიცნობ. საბედნიეროდ, დრონის ქვეშ მყოფი მცენარის არჩევანის გაზომვა `get_companion()`-ით შეიძლება. ის აბრუნებს კორტეჟს, რომლის პირველი ელემენტია მცენარის ტიპი, რომელიც მას თანამდგომად უნდა, მეორე კი პოზიცია, სადაც ეს თანამდგომი უნდა იყოს. მოსავლიანობის ბონუსისთვის თანამდგომს ბოლომდე გაზრდა არ სჭირდება.

{{codeexample 
{
    "camera_position": {"x": -1, "y": 1.8, "z": 5},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": true,
    "show_output": true,
    "collapsing": true,
    "autoplay": false,
    "items": [],
    "world_size": {"x": 3, "y": 3},
    "execution_speed": 1,
    "digging_speed": 1,
    "action_ticks": 200,
    "operation_ticks": 20,
    "seed": 1,
    "exclude_unlocks": ["watering", "fertilizer"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
#CODE
plant(Entities.Bush)
plant_type, (x, y) = get_companion()
print(plant_type, (x, y))
move(East)
move(South)
plant(plant_type)
move(West)
move(North)
do_a_flip()
harvest()
}}

მცენარის არჩეული თანამდგომი შეიძლება იყოს `Entities.Grass`, `Entities.Bush`, `Entities.Tree` ან `Entities.Carrot`. თითოეული მცენარე შემთხვევით ირჩევს, მაგრამ ყოველთვის საკუთარი ტიპისგან განსხვავებულს. პოზიცია შეიძლება იყოს ნებისმიერ ადგილას მცენარიდან 3 სვლის ფარგლებში, გარდა თავად მცენარის პოზიციისა.

თუ დრონის ქვეშ არ არის მცენარე, რომელსაც თანამდგომი უნდა, `get_companion()` აბრუნებს `None`-ს.

პოლიკულტურის პირველად განბლოკვამდე მოსავლიანობის მამრავლი `5`-ია. ყოველი გაუმჯობესებისას ის ორმაგდება.

---

[სტატისტიკა](docs/stats.md)      [კორტეჟები](docs/scripting/tuples.md)      [ლექსიკონები](docs/scripting/dicts.md)      [გრძნობები](docs/unlocks/senses.md)      [დარგვა](docs/unlocks/plant.md)

[harvest()](functions/harvest)      [plant()](functions/plant)      [move()](functions/move)      [till()](functions/till)      [get_companion()](functions/get_companion)
