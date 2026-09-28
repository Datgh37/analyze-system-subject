# ĐẶC TẢ GIAO DIỆN & ATOMIC DESIGN SYSTEM — MÀN HÌNH NGÂN SÁCH CÁ NHÂN

## 1. Design Tokens Catalog
- **Colors:**
  - Primary (Brand): `#2563EB` (Blue 600)
  - Needs Category: `#059669` (Emerald 600)
  - Wants Category: `#D97706` (Amber 600)
  - Savings Category: `#7C3AED` (Violet 600)
  - Alert Danger: `#DC2626` (Red 600)
- **Typography:** Font family Inter, Outfit. H1 28px/600, Body 14px/400.
- **Spacing:** 8px grid system (8px, 16px, 24px, 32px).

## 2. Atomic Component Hierarchy
- **Atoms:** `CurrencyInput`, `CategoryBadge`, `PercentagePill`.
- **Molecules:** `BudgetSliderRow`, `DailyAllowanceGauge`.
- **Organisms:** `Budget503020Card`, `KakeiboBreakdownPanel`.
- **Templates:** `PersonalBudgetPlanningLayout`.

## 3. Accessibility & WCAG 2.1 AA Checklist
- **Contrast Ratio:** Tất cả nhãn chữ và nút bấm đều đạt tỷ lệ tương phản tối thiểu 4.8:1 trên nền trắng và 12.5:1 trên nền tối.
- **Focus Indicators:** Viền xanh 2px outline rõ ràng cho người dùng điều hướng bằng bàn phím.
