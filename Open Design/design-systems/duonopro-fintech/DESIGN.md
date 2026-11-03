# Design System Inspired by DuoNoPro Fintech

> Category: Mobile Apps & Fintech
> Dark Luxury Fintech design system for personal wealth discipline, budget management, and multimodal AI financial assistance.

## Color Palette
- Canvas Background (`--bg`): `#0B0F19` (Deep Space Black) — The foundational dark canvas, providing high contrast and eye comfort.
- Primary Surface (`--surface`): `#111827` (Slate Gray) — Top-level cards, bottom navigation dock, and header bars.
- Secondary Card Surface (`--surface-warm`): `#1E293B` (Lifted Slate) — Action items, nested transaction rows, and modal dialogs.
- Border Divider (`--border`): `#334155` (Slate 700) — Ultra-thin 0.5pt-1pt structural lines separating information without clutter.
- Brand Primary (`--accent`): `#4F46E5` (Electric Indigo) — Key action buttons, active tab indicators, and progress high-water marks.
- Secondary Accent: `#38BDF8` (Sky Blue) — SignalR Realtime live badges, secondary actions, and Voice AI glow.
- Income & Wealth (`--success`): `#10B981` (Emerald Green) — Positive cash inflows, money deposits, and budget-safe indicators (< 70%).
- Expense & Deficit (`--danger`): `#EF4444` / `#F43F5E` (Rose Coral) — Cash outflows, deficit warnings, and over-budget alerts (> 90%).
- VIP & High Net-Worth (`--warn`): `#F59E0B` (Amber Gold) — Obsidian VIP card chip, DSA remaining allowance alert.
- Primary Text (`--fg`): `#F8FAFC` (Slate 50) — High readability primary text for balances, headers, and merchant titles.
- Secondary Text (`--fg-2`): `#E2E8F0` (Slate 200) — Transaction dates, category labels, and subtitles.
- Muted Text (`--muted`): `#94A3B8` (Slate 400) — Microcopy, metadata, timestamps, and input placeholders.

## Typography
- Display & Body Font Family: `"Plus Jakarta Sans", system-ui, -apple-system, sans-serif`
- Monospace Numbers & Metrics: `"JetBrains Mono", "SF Mono", Menlo, Consolas, monospace` with `font-variant-numeric: tabular-nums` to ensure numbers line up vertically and financial figures never jitter during updates.
- Typographic Hierarchy:
  - Hero Balance: 24pt-28pt (Bold 700), tracking `-0.02em`
  - Page Title: 18pt-20pt (Bold 700)
  - Card Title / Merchant Name: 14pt-15pt (SemiBold 600)
  - Body Text / Values: 13pt-14pt (Regular 400 & Medium 500)
  - Caption / Badges: 11pt-12pt (Medium 500 / SemiBold 600)

## Voice & Tone
- Precise, objective, reassuring, and completely devoid of robotic AI clichés.
- High-friction mindfulness on impulsive withdrawals (Thaler Nudge).
- Fast 1-tap confirmation for high-confidence autonomous actions ($\ge 0.85$).
- Clear currency formatting: `₫` or `VNĐ` with thousands commas (e.g. `12,450,000 ₫`).

## Imagery & Iconography
- Zero Emoji/Emote rule: Absolutely no Unicode emojis (💸, 👛, 📊) in production UI.
- Premium Vector Icons: Lucide / Feather stroke-based SVG icons (1.5px-2px stroke width) inside soft-tinted circular badges.
- Obsidian VIP Card: Deep dark gradient (`linear-gradient(135deg, #1E1B4B 0%, #0F172A 100%)`) with subtle golden holographic accent line and contactless chip icon.

## Layout & Spacing
- 8-Point Spacing Grid: Spacing tokens at 4px, 8px, 12px, 16px, 20px, 24px, 32px.
- Double-Bezel Concentric Radii:
  - Exterior container radius: `20px`
  - Nested card / content radius: `14px`
  - Pill badges & status dots: `9999px`
- Bottom Navigation Dock: Fixed dock with 4 balanced tabs (Wallets, Expenses, Budgets, Reports) + Elevated Center Voice AI FAB.

## Agent Prompt Guide
When designing for DuoNoPro Fintech, always maintain a luxurious, focused dark fintech aesthetic. Keep backgrounds deep slate/black, use glowing accent gradients sparingly, prioritize financial clarity with tabular numbers, and structure information into clean cards with hairline borders. Emphasize smart features like Adaptive DSA, Multimodal OCR receipts, and XAI behavioral nudges.

