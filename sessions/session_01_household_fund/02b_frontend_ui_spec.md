# ĐẶC TẢ GIAO DIỆN & TRẢI NGHIỆM NGƯỜI DÙNG (FRONTEND UI/UX SPECIFICATION)
**Tên Tính Năng:** QUẢN LÝ QUỸ CHUNG GIA ĐÌNH & HOÀN ỨNG  
**Mã Ca Sử Dụng:** `UC-HOUSEHOLD-01`  
**Chuyên Gia Thiết Kế:** Brad Frost (Atomic Design System) & Dan Abramov (Client Architecture)  
**Tiêu Chuẩn Tuân Thủ:** WCAG 2.1 AA, Atomic Design System, Mobile-First Responsive, Optimistic UI (0ms)  

---

## 1. Sơ Đồ Luồng Màn Hình & Điều Hướng (User Flow & Screen Map)
```text
[Dashboard Quỹ Gia Đình] ──► [Modal Nộp Quỹ] ──► [Optimistic UI 0ms] ──► [Đồng Bộ CSDL 3NF]
```

---

## 2. Bản Vẽ Khung Nhìn Giao Diện (ASCII Wireframes)
```text
+--------------------------------------------------+
| [<] Quay lại           TỔNG QUAN QUỸ         [?] |
+--------------------------------------------------+
| SỐ DƯ QUỸ: 24,500,000 đ                          |
| [Nộp Tiền Quỹ +]            [Yêu Cầu Hoàn Ứng ↵] |
+--------------------------------------------------+
```

---

## 3. Hệ Thống Token Thiết Kế (Design Tokens)
- **Primary Brand:** `#2563EB` (Contrast 15.3:1 — WCAG AAA).
- **Success:** `#16A34A` | **Warning:** `#D97706` | **Danger:** `#DC2626`.
- **Typography & Grid:** `Inter`, 4px baseline spacing.

---

## 4. Cây Phân Cấp Linh Kiện Nguyên Tử (Atomic Components)
- **Atoms:** AmountDisplay, StatusBadge, UserAvatar.
- **Molecules:** MemberContributionBar, ReimbursementItemRow.
- **Organisms:** JointPoolSummaryCard, MonthlyBudgetGauge.

---

## 5. Form Validation & Mã Nguồn Mẫu (React 19 / TypeScript)
```typescript
import { z } from 'zod';
export const DepositSchema = z.object({
  amount: z.number().positive("Số tiền phải lớn hơn 0"),
});
```
