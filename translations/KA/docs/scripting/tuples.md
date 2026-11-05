[<- ფუნქციები](docs/scripting/functions.md)
---
# კორტეჟები
კორტეჟები რამდენიმე მნიშვნელობის ერთ მნიშვნელობად გაერთიანების შესანიშნავი გზაა.
კორტეჟის შესაქმნელად უბრალოდ გამოყავი მნიშვნელობები მძიმეებით:

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
tuple = 1, 2
print(tuple)
}}

ასევე შეგიძლია გაშალო ისინი რამდენიმე ცვლადში. ქვემოთ მოცემულ კოდში `(1, 2)` კორტეჟი იშლება ორ ცვლადში, `a`-სა და `b`-ში.

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
a, b = 1, 2
print(a)
a, b = b, a
print(a)
}}

კორტეჟებს სიებივით შეგიძლია ინდექსით მიმართო, მაგრამ ისინი უცვლელია და შექმნის შემდეგ მათი შეცვლა შეუძლებელია.

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
tuple = 1, 2
print(tuple[1])

tuple[0] = 3
}}

<unlock=dicts>
სიებისგან განსხვავებით, კორტეჟები შეგიძლია ლექსიკონის გასაღებებად გამოიყენო.

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
d = {(1,2):(4,5)}

print(d[(1,2)])
}}</unlock>

ისინი ფუნქციიდან რამდენიმე მნიშვნელობის დასაბრუნებლადაც გამოგადგება.

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
def f():
    return 1, 2

a, b = f()
}}

---

[სიები](docs/scripting/lists.md)      [ლექსიკონები](docs/scripting/dicts.md)      [ფუნქციები](docs/scripting/functions.md)
