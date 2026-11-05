# Editor upgrade

The mod adds these to the game's code editor. Turn them all off with **Editor enhancements** in Options > General, which puts the stock editor back without a restart.

## Formatting

Press `Ctrl Alt L` or the `{ }` button. With a selection, only the selected lines change. It works for JavaScript and Python, using the file's language (so `// @js` and `# @py` count).

It only changes whitespace: indentation, spacing inside a line, trailing spaces and long runs of blank lines. It keeps comments and strings as they are, and never moves code between lines. `{` stays on the line you wrote it on.

Your code is left alone unless all of these hold:

- it parses
- the result has the same tokens, comments and syntax tree
- formatting the result again changes nothing

Formatting is one undo step. If the window has breakpoints, blank lines stay so that breakpoints keep their lines. Indentation follows the game's Tabs to Spaces option.

**Format on run** and **Format on save** are options, both off by default. Format on run covers the run buttons, the run key and `Ctrl Enter`. Format on save applies to the save key, not autosave, and the game may write the file before formatting finishes, so the formatted text can reach the disk on the next save.

## Shortcuts

Modelled on PhpStorm's default keymap, and changeable in Options > Controls. The `?` button shows the current keys in game.

| Keys | Action |
|---|---|
| `Ctrl Enter` | Run the code (does nothing while it's already running) |
| `Tab` / `Shift Tab` | Indent or unindent lines (the game's own keys) |
| `Ctrl Alt L` | Format code |
| `Ctrl D` | Duplicate line or selection |
| not set | Delete line (`Ctrl Y` is the game's second redo key) |
| `Ctrl /` | Comment or uncomment lines |
| `Alt Shift Up` / `Alt Shift Down` | Move lines |
| `Shift Enter` | New line below |
| `Ctrl Shift J` | Join lines |
| `Ctrl Shift U` | Toggle case |
| `Ctrl Q` | Quick documentation for the word at the caret |
| `Ctrl P` | Parameter info for the call around the caret |

The **Shortcut preset** option in Options > General sets these to PhpStorm's keys, and **Game** puts them back:

| Action | Game | PhpStorm |
|---|---|---|
| Run | `F5` | `Shift F10` |
| Stop | `Shift F5` | `Ctrl F2` |
| Step | `F11` | `F8` |
| Delete line | not set | `Ctrl Y` |
| Second redo | `Ctrl Y` | not set |

Setting the preset back to Game restores the keys you had before choosing PhpStorm. If you remove the mod while the PhpStorm preset is on, those keys stay until you change them in the game's options.

The game checks its own keys, such as run and save, separately from the editor. If you bind an editor shortcut to one of them, both actions happen. On keyboard layouts where AltGr produces letters, AltGr counts as `Ctrl Alt`, so a key such as AltGr+L can trigger Format; rebind it if that gets in the way.

## Buttons

Code windows get `{ }` (format) beside the run and step buttons, plus `?` (shortcuts) and `Copy` beside minimize. Docs examples get `Copy`, which copies the code shown, in the current language, without any setup code.

## Not included

Multiple cursors and code folding aren't possible in the game's text field. Replace, go to line, block comments, rename, go to declaration, grow selection and a breakpoint shortcut aren't done yet.
