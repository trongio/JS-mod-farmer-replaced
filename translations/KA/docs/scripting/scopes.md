[<- ფუნქციები](docs/scripting/functions.md)
---
# სახელების არეები
არეები განსაზღვრავს, სად შეგიძლია ცვლადებს მიწვდე. არე არსებითად სახელებისა და მნიშვნელობების შესაბამისობაა.
ისინი დაახლოებით ისევე მუშაობს, როგორც Python-ში.

არსებობს გლობალური არე, ხოლო თითოეულ ფუნქციას თავისი ლოკალური არე აქვს.
როცა ცვლადს განსაზღვრავ, ის მიმდინარე არეს ემატება.
ყველაფერი, რაც ფუნქციის განსაზღვრების გარეთაა, გლობალური არის ნაწილად ითვლება.

`x = 1`
გლობალურ არეში `1` მნიშვნელობას ანიჭებს `x` სახელს.

ეს `def` ინსტრუქცია გლობალურ არეში ფუნქციას ანიჭებს `f` სახელს.
`def f():
    `ანიჭებს `1` მნიშვნელობას `y` სახელს `f`-ის ლოკალურ არეში.`
    y = 1

    `ანიჭებს ფუნქციას `g` სახელს `f`-ის ლოკალურ არეში.`
    def g():
        pass`

`f()`
გლობალური არიდან იღებს `f`-ში შენახულ ფუნქციას და იძახებს მას.

`print(y)`
ეს `print` ინსტრუქცია გლობალურ არეში შეცდომას იწვევს, რადგან `y` გლობალურ არეში არასდროს გამოცხადებულა, ამიტომ აქ მის წაკითხვას ვერ შეძლებ.
ის მხოლოდ `f`-ის ლოკალურ არეში არსებობდა.

## დარეზერვებული სიტყვა global
ნაგულისხმევად ფუნქციებში ყველა ცვლადი ლოკალურ არეს უკავშირდება, მაშინაც კი, თუ გლობალურ არეში იმავე სახელის ცვლადი არსებობს.

{{codeexample 
{
    "show_image": false,
    "show_code": true,
    "show_inventory": false,
    "show_output": true,
    "collapsing": true,
    "autoplay": false,
    "execution_speed": 1,
    "digging_speed": 1,
    "action_ticks": 200,
    "operation_ticks": 100,
    "seed": 0,
    "exclude_unlocks": ["watering", "fertilizer"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
#CODE
x = 0

def f():
    x = 1
f()
print(x)
}}

ეს კოდი ბეჭდავს `0`-ს, რადგან ლოკალური `x` `f`-ის შიგნით იგივე ცვლადი არ არის, რაც გლობალური `x`, ამიტომ გლობალური `x` უცვლელი რჩება. ეს მნიშვნელოვანია, თორემ ფუნქციის გამოძახებამ შეიძლება შემთხვევით გადააწეროს გლობალურ ცვლადს, რომელსაც ფუნქციის რომელიმე ლოკალური ცვლადის სახელი აქვს.

თუ გლობალურ ცვლადში ჩაწერა გინდა, ეს ცხადად უნდა გააკეთო `global` დარეზერვებული სიტყვით.

{{codeexample 
{
    "show_image": false,
    "show_code": true,
    "show_inventory": false,
    "show_output": true,
    "collapsing": true,
    "autoplay": false,
    "execution_speed": 1,
    "digging_speed": 1,
    "action_ticks": 200,
    "operation_ticks": 100,
    "seed": 0,
    "exclude_unlocks": ["watering", "fertilizer"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
#CODE
x = 0

def f():
    global x
    x = 1
f()
print(x)
}}

ამ მაგალითში `global x` აკავშირებს `x`-ს ზემოთ განსაზღვრულ გლობალურ ცვლადთან `x`. ახლა კოდი დაბეჭდავს `1`-ს.
გაითვალისწინე, რომ გლობალური ცვლადების შეცვლა ხშირად სპაგეტი კოდისკენ გადადგმული პირველი ნაბიჯია, სადაც პროგრამის ყველა ნაწილი ყველა სხვა ნაწილზე მოქმედებს, ამიტომ ამას ნუ გადააჭარბებ.

## ციკლები და განშტოებები
ციკლები და განშტოებები საკუთარ არეს არ ქმნის, ამიტომ რასაც მათ შიგნით გამოაცხადებ, მათ გარეთაც შეგიძლია გამოიყენო.

{{codeexample 
{
    "show_image": false,
    "show_code": true,
    "show_inventory": false,
    "show_output": true,
    "collapsing": true,
    "autoplay": false,
    "execution_speed": 1,
    "digging_speed": 1,
    "action_ticks": 200,
    "operation_ticks": 100,
    "seed": 0,
    "exclude_unlocks": ["watering", "fertilizer"],
    "starting_chunk": 0,
    "dlc_enabled": false
}
#SETUP
#CODE
for i in range(3):
    pass
print(i)
}}

ეს ბეჭდავს `2`-ს, რადგან `for` ციკლის ბოლო იტერაციამ `2` მიანიჭა `i`-ს.

---

[ცვლადები](docs/scripting/variables.md)      [ფუნქციები](docs/scripting/functions.md)      [იმპორტი](docs/scripting/import.md)      [მეგაფერმა](docs/unlocks/megafarm.md)
