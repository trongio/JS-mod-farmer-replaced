[<- ლაბირინთები](docs/unlocks/mazes.md)
---
# მეგაფერმა
ეს წარმოუდგენლად ძლიერი განბლოკვა რამდენიმე დრონზე წვდომას გაძლევს. 
{{codeexample 
{
    "camera_position": {"x": -3, "y": 2.1, "z": 7},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": false,
    "show_inventory": true,
    "show_output": true,
    "collapsing": false,
    "autoplay": true,
    "items": [],
    "world_size": {"x": 7, "y": 7},
    "execution_speed": 21,
    "digging_speed": 1,
    "action_ticks": 200,
    "operation_ticks": 1,
    "seed": 1,
    "exclude_unlocks": ["fertilizer", "watering"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
move(North)
move(North)
move(North)
move(East)
move(East)
move(East)
change_hat(Hats.Wizard_Hat)
#CODE
def harvest_spiral(radius):
    for i in range(1, radius, 2):
        harvest()
        move(West)
        for j in range(i):
            harvest()
            move(South)
        for j in range(i+1):
            harvest()
            move(East)
        for j in range(i+1):
            harvest()
            move(North)
        for j in range(i+1):
            harvest()
            move(West)

while True:
    spawn_drone(harvest_spiral, 7)
    do_a_flip()
}}

როგორც ადრე, იწყებ მხოლოდ ერთი დრონით. დამატებითი დრონები ჯერ უნდა შექმნა, პროგრამის დასრულების შემდეგ კი ისინი გაქრებიან.
თითოეული დრონი თავის ცალკე პროგრამას ასრულებს. ახალი დრონების შესაქმნელად `spawn_drone(function)` ფუნქცია გამოიყენე.

{{codeexample 
{
    "camera_position": {"x": -0.5, "y": 1.5, "z": 4},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": true,
    "show_output": false,
    "collapsing": true,
    "autoplay": false,
    "items": [],
    "world_size": {"x": 2, "y": 1},
    "execution_speed": 1,
    "digging_speed": 1,
    "action_ticks": 200,
    "operation_ticks": 200,
    "seed": 1,
    "exclude_unlocks": ["watering", "fertilizer"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
#CODE
def drone_function():
    move(East)
    do_a_flip()

spawn_drone(drone_function)
do_a_flip()
}}

ასე ახალი დრონი იმავე პოზიციაზე შეიქმნება, სადაც არის დრონი, რომელმაც `spawn_drone(function)` ბრძანება შეასრულა. შემდეგ ახალი დრონი მითითებული ფუნქციის შესრულებას იწყებს. როცა დაასრულებს, ავტომატურად გაქრება, თუ ბოლო დარჩენილი დრონი არ არის.

დრონები ერთმანეთს არ ეჯახებიან. 

`max_drones()` გეტყვის, მაქსიმუმ რამდენი დრონი შეიძლება არსებობდეს ერთდროულად.
`num_drones()` გეტყვის, რამდენი დრონია უკვე ფერმაში.


## მაგალითი
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
    "items": [],
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
#CODE
def harvest_column():
    for _ in range(get_world_size()):
        harvest()
        move(North)

while True:
    if spawn_drone(harvest_column):
        move(East)
}}

ასე შენი პირველი დრონი ჰორიზონტალურად იმოძრავებს და ახალ დრონებს შექმნის. შექმნილი დრონები კი ვერტიკალურად იმოძრავებენ და გზაზე ყველაფერს მოკრეფენ.

თუ ყველა ხელმისაწვდომი დრონი უკვე შექმნილია, `spawn_drone()` არაფერს გააკეთებს და `None`-ს დააბრუნებს.

აი კიდევ ერთი მაგალითი, რომელიც თითოეულ დრონს სხვადასხვა მიმართულებას გადასცემს.
{{codeexample 
{
    "camera_position": {"x": -1, "y": 2, "z": 5},
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
    "operation_ticks": 100,
    "seed": 1,
    "exclude_unlocks": ["fertilizer", "watering"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
move(North)
move(East)
#CODE
for dir in [North, East, South, West]:
    def task():
        move(dir)
        do_a_flip()
    spawn_drone(task)
}}

## ყველა დრონი თანასწორია
განსაკუთრებული „მთავარი“ დრონი არ არსებობს. ყველა დრონს შეუძლია სხვა დრონების შექმნა და ყველა მათგანი დრონების ლიმიტში ითვლება. ყველა დრონი მუშაობის დასრულებისას ქრება. თუ პირველი დრონი პროგრამას ადრე დაასრულებს, კოდის მონიშვნებით სხვა დრონის შესრულება გამოჩნდება. ყველა დრონს შეუძლია გაჩერების წერტილების გააქტიურება. როცა ეს ხდება, კოდის მონიშვნები ამ დრონზე გადადის.

<spoiler=აჩვენე მინიშნება> 
ნახე ეს ძალიან სასარგებლო პარალელური `for_all` ფუნქცია, რომელიც ნებისმიერ ფუნქციას იღებს და ფერმის ყველა უჯრაზე ასრულებს. ამისთვის ყველა ხელმისაწვდომ დრონს იყენებს.

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
    "items": [],
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
#CODE
def for_all(f):
	def row():
		for _ in range(get_world_size()-1):
			f()
			move(East)
		f()
	for _ in range(get_world_size()):
		if not spawn_drone(row):
			row()
		move(North)

for_all(harvest)
}}

ერთი განსაკუთრებით სასარგებლო ხერხია: თუ დრონი ხელმისაწვდომია, შექმენი, თუ არა და საქმე თვითონ გააკეთე.

`if not spawn_drone(task):
	task()`
</spoiler>

## სხვა დრონის ლოდინი
სხვა დრონის დასრულებას `wait_for(drone)` ფუნქციით დაელოდე. `drone` დამმუშავებელს (handle) დრონის შექმნისას მიიღებ.
`wait_for(drone)` აბრუნებს იმ ფუნქციის დაბრუნებულ მნიშვნელობას, რომელსაც მეორე დრონი ასრულებდა.

{{codeexample 
{
    "camera_position": {"x": -0.5, "y": 1.5, "z": 4},
    "show_image": true,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": true,
    "show_output": false,
    "collapsing": true,
    "autoplay": false,
    "items": [],
    "world_size": {"x": 2, "y": 1},
    "execution_speed": 1,
    "digging_speed": 1,
    "action_ticks": 200,
    "operation_ticks": 100,
    "seed": 1,
    "exclude_unlocks": ["watering", "fertilizer"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
move(East)
plant(Entities.Tree)
move(West)
#CODE
def get_entity_type_in_direction(dir):
    move(dir)
    return get_entity_type()

drone = spawn_drone(get_entity_type_in_direction, East)
print(wait_for(drone))
}}

გაითვალისწინე, რომ დრონის შექმნას დრო სჭირდება, ამიტომ ყველა წვრილმანისთვის ახალი დრონის შექმნა კარგი აზრი არ არის.

`has_finished(drone)`-ით შეგიძლია შეამოწმო, დაასრულა თუ არა დრონმა მუშაობა, ისე რომ არ დაელოდო.

## საერთო მეხსიერება არ არსებობს
თითოეულ დრონს საკუთარი მეხსიერება აქვს და სხვა დრონის გლობალურ ცვლადებს პირდაპირ ვერც წაიკითხავს და ვერც მათში ჩაწერს.

{{codeexample 
{
    "camera_position": {"x": -0.5, "y": 1.5, "z": 4},
    "show_image": false,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": false,
    "show_output": true,
    "collapsing": true,
    "autoplay": false,
    "items": [],
    "world_size": {"x": 2, "y": 1},
    "execution_speed": 1,
    "digging_speed": 1,
    "action_ticks": 200,
    "operation_ticks": 200,
    "seed": 1,
    "exclude_unlocks": ["watering", "fertilizer"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
#CODE
x = 0

def increment():
    global x
    x += 1

wait_for(spawn_drone(increment))
print(x)
}}

ეს `0`-ს დაბეჭდავს, რადგან ახალმა დრონმა გლობალური `x`-ის საკუთარი ასლი გაზარდა, რაც პირველი დრონის `x`-ზე გავლენას არ ახდენს.

## არგუმენტების გადაცემა

`spawn_drone()` იღებს დამატებით არასავალდებულო არგუმენტებს, რომლებიც გამოძახებულ ფუნქციას გადაეცემა:

გაითვალისწინე, რომ საერთო მეხსიერების არარსებობის წესი აქაც მოქმედებს. ანუ გამოძახებული ფუნქცია არგუმენტების ასლთან მუშაობს:

{{codeexample 
{
    "camera_position": {"x": -0.5, "y": 1.5, "z": 4},
    "show_image": false,
    "image_size": {"x": 800, "y": 300},
    "show_code": true,
    "show_inventory": false,
    "show_output": true,
    "collapsing": true,
    "autoplay": false,
    "items": [],
    "world_size": {"x": 2, "y": 1},
    "execution_speed": 1,
    "digging_speed": 1,
    "action_ticks": 200,
    "operation_ticks": 200,
    "seed": 1,
    "exclude_unlocks": ["watering", "fertilizer"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
#CODE
def modify(list):
	list.append('green')
	print(list)

l = ['red']
wait_for(spawn_drone(modify, l))
print(l)
}}

## რბოლის პირობები
რამდენიმე დრონს შეუძლია ფერმის ერთსა და იმავე უჯრაზე ერთდროულად იმოქმედოს. თუ ორი დრონი ერთსა და იმავე ტიკში ერთ უჯრაზე მოქმედებს, ორივე მოქმედება შესრულდება, მაგრამ შედეგი შეიძლება მათ თანმიმდევრობაზე იყოს დამოკიდებული.

მაგალითად, წარმოიდგინე, რომ `0` და `1` დრონები ერთი და იმავე, თითქმის სრულად გაზრდილი ხის ზემოთ არიან.
დრონი `0` იძახებს
`use_item(Items.Fertilizer)`
დრონი `1` იძახებს
`harvest()`

თუ ეს მოქმედებები ერთდროულად მოხდა, ხე ჯერ სასუქს მიიღებს და შემდეგ მოიკრიფება. ამ შემთხვევაში მისგან ხე-ტყეს მიიღებ. მაგრამ თუ დრონი `1` ოდნავ უფრო სწრაფია, ხე სასუქის მიღებამდე მოიკრიფება და ხე-ტყეს ვერ მიიღებ.
ამას „რბოლის პირობა“ ეწოდება. ეს პარალელური პროგრამირების გავრცელებული პრობლემაა, როცა შედეგი ოპერაციების შესრულების თანმიმდევრობაზეა დამოკიდებული.

აი კიდევ ერთი პრობლემური სიტუაცია, რომელიც შეიძლება მოხდეს, როცა რამდენიმე დრონი ერთსა და იმავე პოზიციაზე ერთსა და იმავე კოდს ერთდროულად ასრულებს.
`if get_water() < 0.5:
    use_item(Items.Water)`

თუ ამას რამდენიმე დრონი ერთდროულად შეასრულებს, ყველა მათგანი პირველ ხაზს გაივლის და `if` ბლოკში შევა. შემდეგ ყველა წყალს გამოიყენებს და ბევრი წყალი ტყუილად დაიხარჯება.
სანამ დრონი მეორე ხაზამდე მივა, `get_water()` შეიძლება აღარ იყოს `0.5`-ზე ნაკლები, რადგან ამასობაში სხვა დრონმა უჯრა უკვე მორწყო.

---

[ფუნქციები](docs/scripting/functions.md)      [სახელების არეები](docs/scripting/scopes.md)      [სიმულაცია](docs/unlocks/simulation.md)

[spawn_drone()](functions/spawn_drone)      [num_drones()](functions/num_drones)      [max_drones()](functions/max_drones)      [wait_for()](functions/wait_for)      [has_finished()](functions/has_finished)
