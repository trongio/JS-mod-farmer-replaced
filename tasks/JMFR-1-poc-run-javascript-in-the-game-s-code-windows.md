---
id: JMFR-1
title: "PoC: run JavaScript in the game's code windows"
status: in-progress
milestone: true
size: L
area: mod
priority: 2
blocked: ""
depends: []
created: 2026-11-05
started: 2026-11-05
done:
branch: feat/js-poc
links: []
---

## Goal

A BepInEx 5 + Harmony plugin where a code window starting with `// @js` is
tokenized and parsed as JavaScript into the game's existing Node classes, so tick
costs, stepping, breakpoints, multi-drone and builtins keep working. Python files
keep working unchanged.

## Acceptance

- [x] Headless: `dotnet build` passes and the JS/Python tree parity tests pass
- [x] Headless: invalid JS raises `ParseException` at the right span
- [x] In game: BepInEx loads the plugin under Proton
- [ ] In game: `while (true) { harvest(); move(North); }` runs with the same tick cost as the Python equivalent
- [x] In game: a Python file in the same save still runs
- [ ] In game: invalid JS shows an error at the right position, and highlighting is usable
- [ ] In game: breakpoints, step mode and `spawn_drone(fn)` work with JS
- [ ] In game: a completed `leaderboard_run` uploads no score

## Notes

- Risk: standard (medium), mainly BepInEx on Unity 6 under Proton.
- BepInEx: `BepInEx_win_x64_5.4.23.5`, sha256 `82f9878551030f54657792c0740d9d51a09500eeae1fba21106b0c441e6732c4`.
- Do not copy from Lucy Was Replaced (no license found). It is only evidence that these patch points work.
- Workflow agents are unavailable until a session restart, so built-in agents stand in for the same roles.
- CoreCLR loads the game's `Core.dll`, so tests use the game's parser and interpreter nodes as the oracle. Builtins that need the farm or Unity (`harvest`, `Items.*`) are only compared as trees, or checked in game.
- Installed in the game folder (nothing existing was overwritten), to uninstall delete: `.doorstop_version`, `changelog.txt`, `doorstop_config.ini`, `winhttp.dll`, and the `BepInEx/` folder. Then remove the `WINEDLLOVERRIDES` launch option.
- Mod loading uses a `winhttp` native override in the game prefix `pfx/user.reg` (backup `user.reg.bak-jsmod`), so no Steam launch option is needed on this PC. Other users still need the launch option.
- First in-game run 2026-11-06: `harvest();` under `// @js` harvested, Python `harvest()` harvested, and `let = 3;` showed the error box on `=`. Loops, tick comparison, breakpoints, step mode, `spawn_drone` and `leaderboard_run` are still untested in game because they need unlocks.
