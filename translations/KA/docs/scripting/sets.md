[<- ლექსიკონები](docs/scripting/dicts.md)
---
# სიმრავლეები
სიმრავლეები [ლექსიკონებს](docs/scripting/dicts.md) ჰგავს, ოღონდ მნიშვნელობების გარეშე. ეს უბრალოდ გასაღებების დაულაგებელი ერთობლიობაა. 

ისინი ლექსიკონებივით იქმნება, ოღონდ მნიშვნელობების გარეშე.
`set = {North, East, West}`

ცარიელი სიმრავლის შესაქმნელად გამოიყენე `set()`. გაითვალისწინე, რომ `{}` ცარიელ ლექსიკონს ქმნის.

სიმრავლეში ახალი ელემენტის დასამატებლად გამოიყენე `set.add(elem)`.

სიმრავლიდან ელემენტის წასაშლელად გამოიყენე `set.remove(elem)`.

იმის შესამოწმებლად, არის თუ არა ელემენტი სიმრავლეში, გამოიყენე `if elem in set:`.

სიმრავლის ყველა ელემენტის გადასაყოლად გამოიყენე `for elem in set:`.
დიდ სიმრავლეებზე `in` ოპერატორი გაცილებით სწრაფად მუშაობს, ვიდრე სიაზე.

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
my_set = set()
print(my_set)
my_set.add(1)
my_set.add(North)
print(my_set)
my_set.remove(North)
print(my_set)
if North in my_set:
    print("North is in set")
else:
    print("North is not in set")
for element in my_set:
    print(element)
}}

ლექსიკონების მსგავსად, სიმრავლეებიც დაულაგებელია, ამიტომ გარანტია არ არსებობს, რა თანმიმდევრობით გადაუყვები ელემენტებს.

ასევე, სიმრავლის ელემენტები უნიკალურია, ამიტომ თუ დაამატებ ელემენტს, რომელიც მასში უკვე არის, სიმრავლე არ შეიცვლება.

---

[ლექსიკონები](docs/scripting/dicts.md)      [სიები](docs/scripting/lists.md)      [for ციკლი](docs/scripting/for.md)

[len()](functions/len)
