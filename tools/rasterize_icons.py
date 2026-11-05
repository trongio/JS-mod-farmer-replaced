"""Rasterizes src/JsMod/icons/svg to the white-on-transparent PNGs the plugin embeds.

    uv run --no-project --with cairosvg python -I tools/rasterize_icons.py

Lucide draws on a 24 unit grid with a stroke of 2. A heavier stroke matches the game's own icons at about 21 pixels.
"""
import pathlib

import cairosvg

ICONS = pathlib.Path(__file__).resolve().parent.parent / "src" / "JsMod" / "icons"
SIZE = 96
STROKE = "2.5"

for svg in sorted((ICONS / "svg").glob("*.svg")):
    source = svg.read_text(encoding="utf-8")
    white = source.replace('stroke="currentColor"', 'stroke="#ffffff"').replace('stroke-width="2"', f'stroke-width="{STROKE}"')
    if white == source:
        raise SystemExit(f"{svg.name}: expected Lucide's stroke attributes")
    cairosvg.svg2png(bytestring=white.encode("utf-8"), write_to=str(ICONS / (svg.stem + ".png")), output_width=SIZE, output_height=SIZE)
    print(svg.stem)
