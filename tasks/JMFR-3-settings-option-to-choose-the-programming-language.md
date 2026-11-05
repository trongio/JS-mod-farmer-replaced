---
id: JMFR-3
title: "Settings option to choose the programming language"
status: in-progress
milestone: false
size: M
area: mod
priority: 2
blocked: ""
depends: []
created: 2026-11-06
started: 2026-11-06
done:
branch: feat/js-poc
links: []
---

## Goal

An option in the game's settings chooses Python or JavaScript as the programming language, independent of the display language, so Python + Georgian and JS + Georgian both work. Default is Python.

## Acceptance

- [ ] Option appears in Options > General, persists across restarts, and defaults to Python
- [ ] Python mode: plain code runs as Python, exactly as stock
- [ ] JavaScript mode: plain code runs as JS with no marker
- [ ] `// @js` first line forces JS in Python mode, and `# @py` forces Python in JS mode
- [ ] Docs examples, UISim setup code and other internal Python keep working in both modes
- [ ] Option label and values show in English and Georgian
- [ ] Switching the option re-parses and recolours open code windows without a restart
