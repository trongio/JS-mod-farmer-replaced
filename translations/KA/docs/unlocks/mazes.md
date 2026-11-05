[<- სასუქი](docs/unlocks/fertilizer.md) <right>[მეგაფერმა ->](docs/unlocks/megafarm.md)
---
# ლაბირინთები
`Items.Weird_Substance` ბუჩქებზე უცნაურად მოქმედებს. თუ დრონი ბუჩქის თავზეა და `use_item(Items.Weird_Substance, amount)`-ს გამოიძახებ, ბუჩქი ცოცხალი ღობეების ლაბირინთად გაიზრდება.
ლაბირინთის ზომა გამოყენებული `Items.Weird_Substance`-ის რაოდენობაზეა დამოკიდებული (`use_item()` გამოძახების მეორე არგუმენტი).
ლაბირინთის გაუმჯობესებების გარეშე `n` `Items.Weird_Substance`-ის გამოყენება `n`x`n` ლაბირინთს შექმნის. ლაბირინთის გაუმჯობესების ყოველი დონე აორმაგებს განძს, მაგრამ აორმაგებს საჭირო `Items.Weird_Substance`-ის რაოდენობასაც.
ასე რომ, მთელი მინდვრის ოდენა ლაბირინთის შესაქმნელად:

{{codeexample 
{
    "camera_position": {"x": -1, "y": 2.4, "z": 5},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": true,
    "show_output": false,
    "collapsing": true,
    "autoplay": false,
    "items": [{"item": "weird_substance", "n": 10}],
    "world_size": {"x": 4, "y": 4},
    "execution_speed": 1,
    "digging_speed": 1,
    "action_ticks": 200,
    "operation_ticks": 1,
    "seed": -1,
    "exclude_unlocks": ["watering"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
#CODE
plant(Entities.Bush)
size = get_world_size()
substance = size * 2**(num_unlocked(Unlocks.Mazes) - 1)
use_item(Items.Weird_Substance, substance)
}}


რატომღაც დრონი ცოცხალ ღობეებზე ვერ გადაფრინდება, თუმცა ისინი არც ისე მაღალი ჩანს.

ლაბირინთში სადღაც განძია დამალული. გამოიყენე `harvest()` განძზე და მიიღებ ლაბირინთის ფართობის ტოლ ოქროს. მაგალითად, 5x5 ლაბირინთი 25 ოქროს მოგცემს.

თუ `harvest()`-ს სხვაგან სადმე გამოიყენებ, ლაბირინთი უბრალოდ გაქრება.

`get_entity_type()` უდრის `Entities.Treasure`-ს, თუ დრონი განძის თავზეა, ლაბირინთის ყველა სხვა ადგილას კი `Entities.Hedge`-ს.

ლაბირინთებში ციკლური გზები არ არის, თუ მათ ხელახლა არ გამოიყენებ (იხილე ქვემოთ). ამიტომ დრონი იმავე ადგილას ვერ დაბრუნდება, თუ საკუთარ კვალს უკან არ გაჰყვება.

კედელი თუ არის, ისე შეამოწმებ, რომ მასში გავლას სცდი. 
`move()` აბრუნებს `True`-ს, თუ წარმატებით გადაადგილდა, სხვა შემთხვევაში კი `False`-ს.

`can_move()`-ით კედლის არსებობას გადაადგილების გარეშე შეამოწმებ.

{{codeexample 
{
    "camera_position": {"x": -1, "y": 2.4, "z": 5},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": true,
    "show_output": true,
    "collapsing": true,
    "autoplay": false,
    "items": [{"item": "weird_substance", "n": 10}],
    "world_size": {"x": 4, "y": 4},
    "execution_speed": 1,
    "digging_speed": 1,
    "action_ticks": 200,
    "operation_ticks": 100,
    "seed": 1,
    "exclude_unlocks": ["watering"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
move(East)
plant(Entities.Bush)
size = get_world_size()
substance = size * 2**(num_unlocked(Unlocks.Mazes) - 1)
use_item(Items.Weird_Substance, substance)
#CODE
quick_print(
    can_move(North), 
    can_move(East), 
    can_move(South), 
    can_move(West)
)
if move(North) and get_entity_type() == Entities.Treasure:
    harvest()
}}

თუ წარმოდგენა არ გაქვს, როგორ მიხვიდე განძამდე, ჩაიხედე მინიშნება 1-ში. იქ ნაჩვენებია, როგორ მიუდგე ასეთ ამოცანას.

`measure()` ლაბირინთის ნებისმიერ ადგილას განძის პოზიციას აბრუნებს.
`x, y = measure()`

დამატებითი გამოწვევისთვის შეგიძლია ლაბირინთი ხელახლა გამოიყენო: განძზე ისევ იმავე რაოდენობის `Items.Weird_Substance` გამოიყენე.
ეს განძს აგროვებს და ლაბირინთის შემთხვევით პოზიციაზე ახალ განძს აჩენს.

განძის ყოველი გადაადგილებისას ლაბირინთის ზოგიერთი კედელი შეიძლება შემთხვევით გაქრეს. ამიტომ ხელახლა გამოყენებულ ლაბირინთებში შეიძლება ციკლური გზები გაჩნდეს.

გაითვალისწინე, რომ ციკლური გზები ლაბირინთს ბევრად ართულებს, რადგან იმავე ადგილას უკან მობრუნების გარეშეც შეგიძლია მოხვდე.
ლაბირინთის ხელახლა გამოყენება მეტ ოქროს არ გაძლევს, ვიდრე უბრალოდ მოკრეფა და ახალი ლაბირინთის შექმნა.
ეს 100%-ით დამატებითი გამოწვევაა და შეგიძლია უბრალოდ გამოტოვო.
ის მხოლოდ მაშინ ღირს, თუ დამატებითი ინფორმაცია და მოკლე გზები ლაბირინთის უფრო სწრაფად გავლაში გეხმარება.

განძის გადაადგილება მაქსიმუმ 300-ჯერ შეიძლება. ამის შემდეგ მასზე უცნაური სუბსტანციის გამოყენება აღარც მასში არსებულ ოქროს გაზრდის და აღარც მას გადააადგილებს.

<spoiler=აჩვენე მინიშნება 1>
აი, ასეთი ამოცანის ამოხსნის ზოგადი მიდგომა:

შექმენი ლაბირინთი და წარმოიდგინე, რომ შენ ხარ დრონი.

დაფიქრდი, როგორ ეცდებოდი განძის პოვნას, ლაბირინთში რომ იყო.

ნაბიჯ-ნაბიჯ ჩაიწერე შენი სტრატეგია ისე, რომ სხვასაც შეეძლოს მისი შესრულება დაუფიქრებლად.

ახლა სცადე ეს ნაბიჯები კოდად აქციო.
</spoiler>
<spoiler=აჩვენე მინიშნება 2>
სანამ ციკლური გზები არ არის, ყველა კედელი ერთ დიდ გაბმულ კედელს ქმნის. თუ მარცხენა ხელს კედელზე დაადებ და გაჰყვები, მთელ ლაბირინთს ჩაივლი.
ამ მიდგომას ძალიან ცოტა კოდი სჭირდება და არც იმის დამახსოვრება გიწევს, სად იყავი უკვე. საკმარისია დაახლოებით 10 ხაზი კოდი.
{{codeexample 
{
    "camera_position": {"x": -1, "y": 2.4, "z": 5},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": false,
    "show_inventory": true,
    "show_output": true,
    "collapsing": false,
    "autoplay": true,
    "items": [{"item": "weird_substance", "n": 10}],
    "world_size": {"x": 4, "y": 4},
    "execution_speed": 2,
    "digging_speed": 1,
    "action_ticks": 200,
    "operation_ticks": 1,
    "seed": -1,
    "exclude_unlocks": ["watering"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
plant(Entities.Bush)
size = get_world_size()
substance = size * 2**(num_unlocked(Unlocks.Mazes) - 1)
use_item(Items.Weird_Substance, substance)
#CODE
directions = [North, East, South, West]
index = 0
while get_entity_type() != Entities.Treasure:
    index = (index - 1) % 4
    for _ in range(4):
        if move(directions[index]):
            break
        index = (index + 1) % 4
do_a_flip()
harvest()
}}
</spoiler>
<spoiler=აჩვენე მინიშნება 3>
დრონის აბსოლუტური მიმართულებებით, მაგალითად აღმოსავლეთით ან დასავლეთით, გადაადგილების ნაცვლად შეიძლება სასარგებლო იყოს ფარდობითი მიმართულებები, მაგალითად "მოუხვიე მარჯვნივ" ან "მოუხვიე მარცხნივ". ამისთვის უნდა იცოდე, რა მიმართულებით მოძრაობს დრონი ახლა. დრონი სინამდვილეში არასდროს ბრუნდება, მაგრამ კოდში "ვირტუალური" მობრუნების შენახვა შეგიძლია.
ამაში ინდექსის ეს ხრიკი დაგეხმარება:

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
    "items": [{"item": "weird_substance", "n": 10}],
    "world_size": {"x": 3, "y": 3},
    "execution_speed": 1,
    "digging_speed": 1,
    "action_ticks": 200,
    "operation_ticks": 200,
    "seed": 1,
    "exclude_unlocks": ["watering"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
#CODE
directions = [North, East, South, West]
index = 0
move(directions[index])

#turn right
index = (index + 1) % 4
move(directions[index])

#turn left
index = (index - 1) % 4
move(directions[index])
}}


`% 4` საჭიროა იმისთვის, რომ ბრუნვა "წრეზე" მოხდეს: `3 (West) + 1` ისევ `0 (North)` ხდება, რადგან `4 % 4 == 0` და `-1 % 4 == 3`.</spoiler>
<spoiler=აჩვენე მინიშნება 4>
თუ ვერ ხსნი, ამოცანა ყოველთვის შეგიძლია გაიმარტივო ნაკლებად ეფექტური მიდგომით.
`1`x`1` ლაბირინთის ამოხსნა სულ მარტივია.</spoiler>

---

[სტატისტიკა](docs/stats.md)      [სიები](docs/scripting/lists.md)      [ლექსიკონები](docs/scripting/dicts.md)      [კორტეჟები](docs/scripting/tuples.md)

[harvest()](functions/harvest)      [plant()](functions/plant)      [can_move()](functions/can_move)      [move()](functions/move)      [use_item()](functions/use_item)
