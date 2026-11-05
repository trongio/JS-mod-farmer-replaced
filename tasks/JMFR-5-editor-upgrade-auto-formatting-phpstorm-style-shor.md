---
id: JMFR-5
title: "Editor upgrade: auto formatting, PhpStorm-style shortcuts, more buttons"
status: in-progress
milestone: false
size: L
area: editor
priority: 2
blocked: ""
depends: [JMFR-4]
created: 2026-11-06
started: 2026-11-06
done:
branch: feat/js-everywhere
links: []
---

## Goal

Upgrade the in-game code editor: auto formatting for JavaScript and Python, more shortcuts modelled on PhpStorm's defaults, and buttons where they help.

## Acceptance

- [ ] Formatting works for JS and Python, and a formatted program behaves exactly as before
- [ ] Formatting never runs on code that doesn't parse, and can be undone in one step
- [ ] PhpStorm-style shortcuts work, are rebindable, and are listed in the game
- [ ] New buttons fit the window header, with tooltips in English and Georgian
- [ ] Copy buttons on docs code examples and code blocks, and on the code window, copy exactly the code shown in the current language
- [ ] With the mod's editor features off, the editor behaves as stock
