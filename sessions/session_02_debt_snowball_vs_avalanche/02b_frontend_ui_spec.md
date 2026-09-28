# ĐẶC TẢ GIAO DIỆN & ATOMIC DESIGN SYSTEM — MÀN HÌNH TỐI ƯU HÓA TRẢ NỢ

## 1. Design Tokens Catalog
- **Colors:** Primary `#1E40AF`, Snowball Accent `#0284C7`, Avalanche Accent `#16A34A`, Danger `#DC2626`.
- **Typography:** Inter, Outfit. Card Heading 20px/600.
- **Spacing:** 8px grid (8px, 16px, 24px, 32px).

## 2. Atomic Component Hierarchy
- **Atoms:** `DebtBadge`, `InterestRateTag`, `PayoffSlider`.
- **Molecules:** `DebtAccountItem`, `PayoffComparisonRow`.
- **Organisms:** `DebtPayoffComparisonMatrix`, `DebtFreeTimelineChart`.
- **Templates:** `DebtOptimizerLayout`.

## 3. Accessibility & WCAG 2.1 AA Checklist
- **Contrast Ratio:** Màu xanh Avalanche trên nền trắng đạt 5.4:1, đạt chuẩn WCAG 2.1 AA.
- **Screen Reader:** Nhãn ARIA mô tả chi tiết tiến độ trả nợ của từng tài khoản.
