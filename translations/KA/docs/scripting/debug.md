[<- დარგვა](docs/unlocks/plant.md) <right>[გამართვა 2 ->](docs/unlocks/debug2.md)
<right>[დროის აღრიცხვა ->](docs/unlocks/timing.md)
---
# გამართვა
ხანდახან კოდი უბრალოდ არ მუშაობს და უნდა გაარკვიო, რატომ. ამაში რამდენიმე ხელსაწყო დაგეხმარება.

პირველი ხერხია, პროგრამა ნაბიჯ-ნაბიჯ შეასრულო. 
ნაბიჯ-ნაბიჯ რეჟიმზე გადასვლა შეგიძლია შესრულების ღილაკის გვერდით მყოფი ღილაკით ან გაჩერების წერტილის დასმით.

გაჩერების წერტილის დასამატებლად დააჭირე გაჩერების წერტილების პანელს კოდის მარცხნივ.
![|x227](Breakpoints)
როცა შესრულება გაჩერების წერტილიან ხაზამდე მივა, ავტომატურად ნაბიჯ-ნაბიჯ რეჟიმზე გადავა.

თუ მაუსს ცვლადზე მიიტან, მის მიმდინარე მნიშვნელობას დაინახავ.

`print()` ფუნქციაც ძალიან გამოგადგება. ის ნებისმიერ გადაცემულ მნიშვნელობას პირდაპირ ჰაერში წერს.

მაგალითები:

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
print(0.24)
}}

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
print(can_harvest())
}}

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
print(get_pos_x(), get_pos_y())
}}

`print()` ფუნქცია მნიშვნელობას პირდაპირ ჰაერში და [გამოსავლის](docs/output.md) გვერდზე ბეჭდავს.

ჰაერში წერა ხანდახან ცოტა ნელია, თუ ბევრი მნიშვნელობის დაბეჭდვა გინდა.
ასეთ შემთხვევაში გამოიყენე `quick_print()` ფუნქცია, რომელიც მხოლოდ გამოსავლის ფანჯარაში ბეჭდავს.

გამოსავლის ფანჯარაში გაფრთხილებები და შეცდომებიც იწერება, ამიტომ კარგია, ჩაიხედო, როცა რამე ისე არ მუშაობს, როგორც ელოდი.

შესრულების გაჩერებისას გამოსავალი თამაშის საქაღალდეში [output.txt](persistent_data_path/output.txt) ფაილშიც იწერება.

---

[გამოსავალი](docs/output.md)      [კომენტარები](docs/scripting/comments.md)      [გამართვა 2](docs/unlocks/debug2.md)      [ფერადი ბლოკები](docs/unlocks/debug_place.md)      [სიმულაცია](docs/unlocks/simulation.md)

[print()](functions/print)      [quick_print()](functions/quick_print)
