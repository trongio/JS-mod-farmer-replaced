#!/usr/bin/env python3
"""Validate translations/KA against the game's EN language folder.

Run from anywhere:
    uv run --no-project python -I tools/validate_ka.py --strings-only
    uv run --no-project python -I tools/validate_ka.py --self-test

Exit code is 1 when there are errors. Untranslated entries are listed
separately and do not fail the run.
"""
from __future__ import annotations

import argparse
import re
import sys
import tempfile
from collections import Counter
from pathlib import Path

DEFAULT_EN = Path(
    "/media/azael/Storage/SteamLibrary/steamapps/common/The Farmer Was Replaced/"
    "TheFarmerWasReplaced_Data/StreamingAssets/Languages/EN"
)
DEFAULT_KA = Path(__file__).resolve().parent.parent / "translations" / "KA"

# Matches the split in Localizer.SplitLanguageFileIntoKeyVals.
ENTRY_SPLIT = re.compile(r"\r\n\r\n@|\n\n@|\r\n@|\n@")
# Matches the split in CodeUtilities.ApplyCodeTags: odd segments are code.
CODE_SPLIT = re.compile(r"\r\n```|\n```|```|`")
PLACEHOLDER = re.compile(r"\{\{.*?\}\}+|\{\d+(?::[^{}]*)?\}")
LINK_TARGET = re.compile(r"\]\(([^)]*)\)")
TAG = re.compile(r"</?[A-Za-z][^<>]*>")
HEADING = re.compile(r"^(#{1,6}) ", re.M)
LATIN = re.compile(r"[A-Za-z]")

# Files that are meant to be identical, and the one key only KA has.
SKIP_UNTRANSLATED_FILES = {"languages.txt"}
EXTRA_KEYS = {"languages.txt": {"KA"}}
# Terms left in English on purpose. See translations/glossary.md.
INTENTIONAL_ENGLISH = {"join_discord", "1/4 vsync"}


def parse_entries(text: str) -> tuple[list[tuple[str, str]], list[str]]:
    """Parse a Strings file the way the game does. Returns (entries, problems)."""
    problems: list[str] = []
    if not text.startswith("@"):
        problems.append("file does not start with @ (BOM or stray text)")
        return [], problems
    entries: list[tuple[str, str]] = []
    for chunk in ENTRY_SPLIT.split(text[1:]):
        key, sep, value = chunk.partition(" = ")
        if not sep:
            problems.append(f"malformed entry or line-start @ inside a value: @{chunk[:50]!r}")
            continue
        entries.append((key, value))
    return entries, problems


def code_spans(value: str) -> list[str]:
    return CODE_SPLIT.split(value)[1::2]


SPOILER_LABEL = re.compile(r"<spoiler=[^>]*>")


def prose(value: str) -> str:
    return "".join(CODE_SPLIT.split(value)[0::2])


def compare_value(en: str, ka: str) -> list[str]:
    """Return every way a translated value breaks the rules."""
    problems = []
    en_spans, ka_spans = code_spans(en), code_spans(ka)
    if en_spans != ka_spans:
        for i, (a, b) in enumerate(zip(en_spans, ka_spans)):
            if a != b:
                problems.append(f"code span {i} changed: {a!r} -> {b!r}")
                break
        else:
            problems.append(f"code span count changed: {len(en_spans)} -> {len(ka_spans)}")
    if len(CODE_SPLIT.split(ka)) % 2 == 0 and len(CODE_SPLIT.split(en)) % 2 == 1:
        problems.append("unbalanced backticks")
    checks = (
        ("placeholders", PLACEHOLDER),
        ("link targets", LINK_TARGET),
        ("tags", TAG),
    )
    for label, pattern in checks:
        # Spoiler labels are shown to the player, so only their presence must match.
        a = Counter(pattern.findall(SPOILER_LABEL.sub("<spoiler=>", prose(en))))
        b = Counter(pattern.findall(SPOILER_LABEL.sub("<spoiler=>", prose(ka))))
        if a != b:
            problems.append(f"{label} changed: missing {sorted((a - b).elements())}, added {sorted((b - a).elements())}")
    for brace in "{}":
        if en.count(brace) != ka.count(brace):
            problems.append(f"{brace} count changed: {en.count(brace)} -> {ka.count(brace)}")
    a, b = Counter(HEADING.findall(prose(en))), Counter(HEADING.findall(prose(ka)))
    if a != b:
        problems.append("heading levels changed")
    return problems


def is_untranslated(en: str, ka: str) -> bool:
    if en != ka:
        return False
    text = PLACEHOLDER.sub("", prose(en))
    return bool(LATIN.search(TAG.sub("", text)))


def validate(en_root: Path, ka_root: Path, strings_only: bool) -> tuple[list[str], list[str], dict[str, int]]:
    errors: list[str] = []
    untranslated: list[str] = []
    stats = {"files": 0, "entries": 0, "code_spans": 0, "placeholders": 0, "intentional_english": 0}

    en_strings = en_root / "Strings"
    ka_strings = ka_root / "Strings"
    en_names = {p.name for p in en_strings.glob("*.txt")}
    ka_names = {p.name for p in ka_strings.glob("*.txt")}
    errors += [f"Strings/{n}: missing file" for n in sorted(en_names - ka_names)]
    errors += [f"Strings/{n}: extra file" for n in sorted(ka_names - en_names)]

    for name in sorted(en_names & ka_names):
        stats["files"] += 1
        en_text = (en_strings / name).read_text(encoding="utf-8")
        ka_bytes = (ka_strings / name).read_bytes()
        try:
            ka_text = ka_bytes.decode("utf-8")
        except UnicodeDecodeError as exc:
            errors.append(f"Strings/{name}: not valid UTF-8 ({exc})")
            continue
        en_entries, en_problems = parse_entries(en_text)
        ka_entries, ka_problems = parse_entries(ka_text)
        errors += [f"Strings/{name}: EN source: {p}" for p in en_problems]
        errors += [f"Strings/{name}: {p}" for p in ka_problems]
        if en_text.endswith("\n") != ka_text.endswith("\n"):
            errors.append(f"Strings/{name}: trailing newline differs from EN")

        en_map = dict(en_entries)
        ka_keys = [k for k, _ in ka_entries]
        for key, count in Counter(ka_keys).items():
            if count > 1:
                errors.append(f"Strings/{name}: duplicate key {key}")
        allowed_extra = EXTRA_KEYS.get(name, set())
        errors += [f"Strings/{name}: missing key {k}" for k in en_map if k not in ka_keys]
        errors += [f"Strings/{name}: extra key {k}" for k in ka_keys if k not in en_map and k not in allowed_extra]
        if [k for k in ka_keys if k in en_map] != [k for k in en_map if k in ka_keys]:
            errors.append(f"Strings/{name}: key order differs from EN")

        for key, ka_value in ka_entries:
            if key not in en_map:
                continue
            en_value = en_map[key]
            stats["entries"] += 1
            stats["code_spans"] += len(code_spans(en_value))
            stats["placeholders"] += len(PLACEHOLDER.findall(prose(en_value)))
            errors += [f"Strings/{name}: {key}: {p}" for p in compare_value(en_value, ka_value)]
            if name not in SKIP_UNTRANSLATED_FILES and is_untranslated(en_value, ka_value):
                if key in INTENTIONAL_ENGLISH:
                    stats["intentional_english"] += 1
                else:
                    untranslated.append(f"Strings/{name}: {key}")

    if not strings_only:
        en_docs = {p.relative_to(en_root) for p in en_root.rglob("*.md")}
        ka_docs = {p.relative_to(ka_root) for p in ka_root.rglob("*.md")}
        errors += [f"{p}: missing file" for p in sorted(en_docs - ka_docs)]
        errors += [f"{p}: extra file" for p in sorted(ka_docs - en_docs)]
        for rel in sorted(en_docs & ka_docs):
            stats["files"] += 1
            en_value = (en_root / rel).read_text(encoding="utf-8")
            ka_value = (ka_root / rel).read_text(encoding="utf-8")
            stats["entries"] += 1
            stats["code_spans"] += len(code_spans(en_value))
            stats["placeholders"] += len(PLACEHOLDER.findall(prose(en_value)))
            errors += [f"{rel}: {p}" for p in compare_value(en_value, ka_value)]
            if is_untranslated(en_value, ka_value):
                untranslated.append(str(rel))
    return errors, untranslated, stats


def self_test() -> int:
    """Prove each failure type is caught, using throwaway files."""
    en_text = (
        "@a = Hello {0} and `code()` here\n"
        "@b = See [page](docs/x.md) <right>\n"
        "@c = ### Cost {{{{itemblock cost {0}}}}}\n"
        "@d = Plain sentence.\n"
    )
    good = (
        "@a = გამარჯობა {0} და `code()` აქ\n"
        "@b = იხილე [გვერდი](docs/x.md) <right>\n"
        "@c = ### ფასი {{{{itemblock cost {0}}}}}\n"
        "@d = უბრალო წინადადება.\n"
    )
    cases = {
        "clean tree passes": (good, None),
        "missing key": (good.replace("@d = უბრალო წინადადება.\n", ""), "missing key d"),
        "extra key": (good + "@z = ზედმეტი\n", "extra key z"),
        "duplicate key": (good + "@d = კიდევ\n", "duplicate key d"),
        "key order": ("@b" + good.split("@b")[1].split("@c")[0] + "@a" + good.split("@a")[1].split("@b")[0] + "@c" + good.split("@c")[1], "key order"),
        "changed placeholder": (good.replace("{0} და", "{1} და"), "placeholders changed"),
        "changed code span": (good.replace("`code()`", "`კოდი()`"), "code span 0 changed"),
        "dropped code span": (good.replace("`code()` ", ""), "code span count"),
        "changed link": (good.replace("docs/x.md", "docs/y.md"), "link targets changed"),
        "changed tag": (good.replace("<right>", "<left>"), "tags changed"),
        "stray brace": (good.replace("უბრალო", "უბრალო {"), "{ count changed"),
        "heading level": (good.replace("### ფასი", "## ფასი"), "heading levels"),
        "line-start @": (good.replace("უბრალო", "უბრალო\n@სიტყვა"), "line-start @"),
        "BOM at start": ("﻿" + good, "does not start with @"),
        "trailing newline": (good.rstrip("\n"), "trailing newline"),
    }
    failures = 0
    with tempfile.TemporaryDirectory() as tmp:
        root = Path(tmp)
        en_dir = root / "EN" / "Strings"
        ka_dir = root / "KA" / "Strings"
        en_dir.mkdir(parents=True)
        ka_dir.mkdir(parents=True)
        (en_dir / "t.txt").write_text(en_text, encoding="utf-8")
        for label, (text, expected) in cases.items():
            (ka_dir / "t.txt").write_text(text, encoding="utf-8")
            errors, _, _ = validate(root / "EN", root / "KA", strings_only=True)
            ok = (not errors) if expected is None else any(expected in e for e in errors)
            print(f"{'ok  ' if ok else 'FAIL'} {label}" + ("" if ok else f" -> {errors}"))
            failures += not ok

        (ka_dir / "t.txt").write_text(en_text, encoding="utf-8")
        _, untranslated, _ = validate(root / "EN", root / "KA", strings_only=True)
        ok = "Strings/t.txt: d" in untranslated
        print(f"{'ok  ' if ok else 'FAIL'} untranslated entries are reported")
        failures += not ok

        (ka_dir / "t.txt").unlink()
        errors, _, _ = validate(root / "EN", root / "KA", strings_only=True)
        ok = any("missing file" in e for e in errors)
        print(f"{'ok  ' if ok else 'FAIL'} missing file")
        failures += not ok

        (ka_dir / "t.txt").write_text(good, encoding="utf-8")
        (ka_dir / "x.txt").write_text(good, encoding="utf-8")
        errors, _, _ = validate(root / "EN", root / "KA", strings_only=True)
        ok = any("extra file" in e for e in errors)
        print(f"{'ok  ' if ok else 'FAIL'} extra file")
        failures += not ok
        (ka_dir / "x.txt").unlink()

        (root / "EN" / "docs").mkdir()
        (root / "EN" / "docs" / "p.md").write_text("# Title\n`x()`\n", encoding="utf-8")
        errors, _, _ = validate(root / "EN", root / "KA", strings_only=True)
        ok = not errors
        print(f"{'ok  ' if ok else 'FAIL'} --strings-only tolerates missing docs")
        failures += not ok
        errors, _, _ = validate(root / "EN", root / "KA", strings_only=False)
        ok = any("docs/p.md: missing file" in e for e in errors)
        print(f"{'ok  ' if ok else 'FAIL'} full run requires docs")
        failures += not ok
    print("self-test:", "PASSED" if not failures else f"{failures} FAILED")
    return 1 if failures else 0


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--en", type=Path, default=DEFAULT_EN, help="EN language folder")
    parser.add_argument("--ka", type=Path, default=DEFAULT_KA, help="KA language folder")
    parser.add_argument("--strings-only", action="store_true", help="skip docs, which are not translated yet")
    parser.add_argument("--self-test", action="store_true", help="prove the checks catch each failure type")
    args = parser.parse_args()
    if args.self_test:
        return self_test()
    if not args.en.is_dir():
        print(f"EN folder not found: {args.en}", file=sys.stderr)
        return 2
    errors, untranslated, stats = validate(args.en, args.ka, args.strings_only)
    for e in errors:
        print(f"ERROR {e}")
    for u in untranslated:
        print(f"UNTRANSLATED {u}")
    print(
        f"files={stats['files']} entries={stats['entries']} code_spans={stats['code_spans']} "
        f"placeholders={stats['placeholders']} intentional_english={stats['intentional_english']} "
        f"untranslated={len(untranslated)} errors={len(errors)}"
    )
    return 1 if errors else 0


if __name__ == "__main__":
    sys.exit(main())
