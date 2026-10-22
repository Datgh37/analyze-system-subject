---
description: Run an Impeccable technical quality audit (accessibility, responsive layout, edge cases, anti-patterns) on a target UI view or component.
---

# UI Audit Workflow

Inspect a target UI view, page, or component for accessibility, responsiveness, layout bugs, and AI cliches.

## Usage

```text
/ui-audit [target]
```

## Steps
1. Load `.agents/skills/impeccable/reference/audit.md` and `craft-floor.md`.
2. Inspect target XAML / component against 61 detector rules:
   - Check text contrast (WCAG 2.1 AA).
   - Check responsive co-adaptation on 360px mobile, 768px tablet, and 1280px desktop.
   - Check for nested card anti-patterns ("card soup").
   - Check text truncation / overflow on large financial currency values.
3. Emit PASS/WARN/FAIL findings with exact code remediation steps.
