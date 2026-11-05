# How this JavaScript differs from real JavaScript

JavaScript here is translated into the game's own language, so it keeps the game's costs
and rules. That leads to the differences below.

## Choosing the language

**Options > General > Programming language** sets the language of your code windows, either
Python (the default) or JavaScript. It takes effect straight away, with no restart: open
windows are parsed and coloured again. A program that is already running keeps the code it
started with, and its windows pick up the new language the next time you run them.

The first non-blank line of a file can override the setting for that file:

- `// @js` makes it JavaScript.
- `# @py` makes it Python.

A file keeps its own language when another file imports it, so a `# @py` file can import a
JavaScript one. The setting only applies to your own files. Docs examples and the game's
internal code are always Python. Python files with no marker show errors in JavaScript mode,
so add `# @py` to the ones you want to keep.

## Scope

- **No block scope.** `let`, `const` and `var` all belong to the whole function, or to the
  file at top level. In `const c = 1; { let c = 2; }`, both are the same `c`.
- **Redeclaring without a value keeps the old value.** This matches `var`, but also applies
  to `let` in a later block, where real JS would start again from `undefined`.
- **`var x;` first seen inside a loop resets each time round.** It is only skipped once the
  name has been declared earlier in the source.
- **No hoisting.** Declare a function above the code that calls it.
- **Closures see the latest value** of a loop variable, not one per iteration.

## Values

- **Truthiness follows the game.** Empty lists, dictionaries, sets and strings are false.
- **No type coercion.** `"a" + 1` is an error, so use `"a" + str(1)`. `==` behaves like `===`.
- **`null` and `undefined` are the same value.**
- **`%` follows the game's rule,** so `-1 % 3` is `2`, not `-1`.
- **String escapes are not processed.** `"\n"` is a backslash and an `n`.

## Methods and names

- The game's names work everywhere: `a.append(x)`, `len(a)`, `print(x)`, `x in a`.
- Familiar spellings compile to exactly the same thing and cost the same: `a.push(x)`,
  `a.length`, `a.includes(x)`, `s.has(x)` and `console.log(x)`.
- `Math.floor(a / b)` is the game's floor division. `Math.floor(x)`, `Math.abs`, `Math.min`,
  `Math.max` and `Math.random` are there too, but nothing else on `Math`.
- Names built into the game, such as `North` and `Items`, can't be reassigned. Functions
  such as `harvest` can, the same as in Python.

## Expressions

- `++` and `--` work only as a statement or in a `for` update, so `y = x++` is an error.
- Assignment is a statement, so `a = b = 1` and `if (a = 1)` are errors.

## Not supported yet

Each of these reports an error at its position: ternary `?:`, arrow functions and function
expressions, classes and `new`, `typeof`, `switch`, `do`, `try`, `??`, `?.`, spread, bitwise
operators, `${}` in templates, `for...in` and regular expressions.

## Imports

Imports work between files, whichever language each one is in:

- `import * as lib from "lib";` then `lib.f()`. The name has to match the file. `import "lib";`
  does the same.
- `import { a, b } from "lib";` brings in those names.
- `import * from "lib";` brings in every name. This form isn't real JavaScript, but the game has it.

Every top level name can be imported, so `export` isn't needed. It's accepted on a function or
variable and ignored. Imports go at the top level of a file.

## Tuples

The game's tuples have no literal in JavaScript. Use arrays:

- `return [a, b];` and `let [x, y] = f();`, which also reads what `measure()` and
  `get_companion()` return, including nested ones: `let [type, [x, y]] = get_companion();`.
- Arrays can be changed but the tuples the game returns can't, so `pos[0] = 1` on one is an error.
- Arrays can't be dictionary keys. Use a string such as `str(x) + "," + str(y)`.

## Dictionary keys

As in JS, `{North: 1}` has the string `"North"` as its key. Use `{[North]: 1}` for the direction,
or for values such as `{[Items.Hay]: 100}`.

## Same as real JavaScript, but easy to miss

A line starting with `(`, `[`, `+`, `-`, `/` or `.` continues the line before it, even
without a semicolon, so `f` followed by `(1)` on the next line calls `f(1)`. A line
starting with `++` or `--` does not. `return` alone on a line returns nothing.
