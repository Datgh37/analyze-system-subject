# ĐẶC TẢ GIAO DIỆN & ATOMIC DESIGN SYSTEM — MÀN HÌNH CẢNH BÁO TỐC ĐỘ CHI TIÊU

## 1. Design Tokens Catalog
- **Colors:** Safe Velocity `#10B981`, Moderate `#F59E0B`, Critical Depletion `#EF4444`.
- **Typography:** Inter, Outfit. Số ngày cảnh báo hiển thị to 36px/700 màu đỏ cảnh báo.
- **Spacing:** 8px grid (8px, 16px, 24px, 32px).

## 2. Atomic Component Hierarchy
- **Atoms:** `BurnRatePill`, `DepletionDayBadge`, `SubscriptionToggle`.
- **Molecules:** `VelocityAlertBanner`, `SubscriptionTrapRow`.
- **Organisms:** `SpendingVelocityCard`, `RecurringBillsManager`.
- **Templates:** `CashflowHealthDashboardLayout`.

## 3. Accessibility & WCAG 2.1 AA Checklist
- **Contrast:** Tỷ lệ tương phản chữ cảnh báo đỏ trên nền sáng đạt 4.9:1, đảm bảo đọc rõ ràng.
- **Motion Accessibility:** Tôn trọng cài đặt `prefers-reduced-motion` của hệ điều hành cho các hiệu ứng rung cảnh báo.
