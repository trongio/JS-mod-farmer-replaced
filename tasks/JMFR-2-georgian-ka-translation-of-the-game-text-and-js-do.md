---
id: JMFR-2
title: "Georgian (KA) translation of the game text and JS docs"
status: in-progress
milestone: false
size: L
area: i18n
priority: 1
blocked: ""
depends: [JMFR-1]
created: 2026-11-05
started: 2026-11-05
done:
branch: feat/georgian
links: []
---

## Goal

Georgian (KA) as a selectable game language, with prose in Georgian and all code byte-identical to English, shipped as a BepInEx plugin with its own font.

## Acceptance

- [x] Validator passes with 0 errors and 0 untranslated UI strings
- [x] All 15 UI string files translated
- [x] In game: the plugin loads and the log shows all steps ok
- [x] In game: KA appears in the language dropdown
- [x] In game: Georgian renders in menus, options and tooltips with no boxes
- [x] In game: docs windows show English, not blank
- [ ] In game: Georgian runtime error messages, inventory and tech tree tooltips checked
- [ ] Owner has reviewed the glossary
- [x] Stage 2: docs translated (26k words) and shown in game

## Notes

- UI lives on branch feat/georgian, worktree .workflow/worktrees/JMFR-2.
- Follow-ups: JS parse errors are English only, and the error box header showed English `ERROR` in a Georgian session. `KaLanguage` alone does not block the two achievements `JsMod` blocks.
- Uninstall: delete `BepInEx/plugins/KaLanguage` and `TheFarmerWasReplaced_Data/StreamingAssets/Languages/KA`.
