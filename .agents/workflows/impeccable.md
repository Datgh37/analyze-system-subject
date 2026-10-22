---
description: Run Impeccable AI design harness commands (audit, critique, polish, harden, distill, adapt, animate, typeset, layout) on UI components or pages.
---

# Impeccable Design Harness Workflow

Run design, redesign, shaping, UX critique, quality audit, and polish commands on mobile/web interfaces using the Impeccable design harness.

## Usage

```text
/impeccable <subcommand> [target]
```

### Supported Subcommands:
- `audit [target]`: Run technical quality checks (accessibility, performance, responsive layout, contrast).
- `critique [target]`: Evaluate UX design, visual hierarchy, clarity, scanability, and emotional resonance.
- `polish [target]`: Perform final quality pass fixing alignment, spacing, consistency, and design token adherence.
- `harden [target]`: Make interfaces production-ready: error handling, text overflow, auto-scaling numbers, edge cases.
- `distill [target]`: Strip unnecessary complexity, remove card-in-card nesting, emphasize core financial data.
- `adapt [target]`: Adapt layout to work seamlessly across mobile (phone), tablet, and desktop breakpoints.
- `animate [target]`: Enhance interactions with purposeful spring motion, smooth transitions, and charts animation.
- `colorize [target]`: Add strategic color tinting (Deep Indigo / Navy Slate), eliminate pure grays.
- `typeset [target]`: Refine typography, hierarchy, font weights, and tabular numbers for financial figures.
- `shape [target]`: Plan UI/UX structure before writing code.
- `init`: Inspect project and set up durable context (`PRODUCT.md` and `DESIGN.md`).

## Workflow Steps

1. **Load Context & Design Truth:**
   - Read `.agents/skills/impeccable/SKILL.md`.
   - Read target project's `PRODUCT.md` and `DESIGN.md` (e.g. in `mobile-project/dotnet-maui-ver/`).
2. **Execute Requested Subcommand Playbook:**
   - Read the corresponding playbook in `.agents/skills/impeccable/reference/<subcommand>.md`.
   - Follow its specific inspection checklist and craft floor rules (`reference/craft-floor.md`).
3. **Report Findings & Concrete Refinements:**
   - Provide clear, actionable before/after code snippets or token adjustments.
   - Verify zero regression in layout or academic compliance.
