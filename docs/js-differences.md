# How this JavaScript differs from real JavaScript

Start a code window with `// @js` to use it. JavaScript here is translated into the
game's own language, so it keeps the game's costs and rules. That leads to these
differences.

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

- Use the game's names: `a.append(x)` and `len(a)`, not `a.push(x)` and `a.length`.
- Names built into the game, such as `North` and `Items`, can't be reassigned. Functions
  such as `harvest` can, the same as in Python.

## Expressions

- `++` and `--` work only as a statement or in a `for` update, so `y = x++` is an error.
- Assignment is a statement, so `a = b = 1` and `if (a = 1)` are errors.

## Not supported yet

Each of these reports an error at its position: ternary `?:`, arrow functions and function
expressions, classes and `new`, `typeof`, `switch`, `do`, `try`, `??`, `?.`, spread, bitwise
operators, `${}` in templates, `for...in`, regular expressions, `import` and `export`.

## Same as real JavaScript, but easy to miss

A line starting with `(`, `[`, `+`, `-`, `/` or `.` continues the line before it, even
without a semicolon, so `f` followed by `(1)` on the next line calls `f(1)`. A line
starting with `++` or `--` does not. `return` alone on a line returns nothing.
