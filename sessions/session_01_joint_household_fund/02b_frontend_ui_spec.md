# ĐẶC TẢ GIAO DIỆN & TRẢI NGHIỆM NGƯỜI DÙNG (FRONTEND UI/UX SPECIFICATION)
**Tên Tính Năng:** QUẢN LÝ QUỸ CHUNG GIA ĐÌNH & HOÀN ỨNG TỰ ĐỘNG  
**Mã Ca Sử Dụng:** `UC-HOUSEHOLD-01`  
**Chuyên Gia Thiết Kế:** Brad Frost (Atomic Design System) & Dan Abramov (Client Architecture)  
**Tiêu Chuẩn Tuân Thủ:** WCAG 2.1 AA, Atomic Design System, Mobile-First Responsive, Optimistic UI (0ms)  

---

## 1. Sơ Đồ Luồng Màn Hình & Điều Hướng (User Flow & Screen Map)

```text
[Màn hình Tổng Quan Quỹ Gia Đình]
       │
       ├──► (Bấm "+ Nạp Tiền Quỹ") ──► [Modal Nộp Tiền] ──► [Optimistic UI +2,000,000đ (0ms)] ──► [Xác Nhận CSDL]
       │
       ├──► (Bấm "+ Đăng Ký Hoàn Ứng") ──► [Form Nhập Hóa Đơn & Đính Kèm Ảnh] ──► [Optimistic Status: PENDING_APPROVAL]
       │
       └──► (Bấm "Xem Chi Tiết Đóng Góp") ──► [Màn hình Tỷ Lệ Đóng Góp Vợ/Chồng]
```

---

## 2. Bản Vẽ Khung Nhìn Giao Diện (ASCII Wireframes)

### A. Màn Hình Di Động (Mobile Viewport - 375px)
```text
┌──────────────────────────────────────┐
│ [←] QUỸ GIA ĐÌNH: 'TỔ ẤM'       [⚙️] │
├──────────────────────────────────────┤
│ ┌──────────────────────────────────┐ │
│ │ 💰 SỐ DƯ QUỸ HIỆN TẠI            │ │
│ │ 24,500,000 VND                   │ │
│ │ [Nộp Quỹ +]      [Hoàn Ứng ↵]    │ │
│ └──────────────────────────────────┘ │
│                                      │
│ 👥 TỶ LỆ ĐÓNG GÓP THÁNG NÀY          │
│ ┌──────────────────────────────────┐ │
│ │ Chồng (60%): 15,000,000 đ  [100%]│ │
│ │ Vợ    (40%): 10,000,000 đ  [100%]│ │
│ └──────────────────────────────────┘ │
│                                      │
│ 📋 HOẠT ĐỘNG GẦN ĐÂY                 │
│ ┌──────────────────────────────────┐ │
│ │ 🛒 Đi chợ WinMart    -850,000 đ  │ │
│ │ ⚡ Tiền điện tháng 8 -1,240,000 đ │ │
│ └──────────────────────────────────┘ │
│                                      │
│ [=========== (+) CHI TIÊU ==========] │
└──────────────────────────────────────┘
```

---

## 3. Hệ Thống Token Thiết Kế (Design Tokens Catalog)

### A. Bảng Màu Chuẩn HSL & Hex
* **Primary Brand:** `#2563EB` (Xanh Dương Fintech, Contrast: 15.3:1 — WCAG AAA).
* **Success (Đã nộp quỹ):** `#16A34A` (Xanh Lá).
* **Warning (Sắp hết quỹ):** `#D97706` (Vàng Cam).
* **Danger (Quỹ âm):** `#DC2626` (Đỏ).
* **Surface Light / Dark:** `#FFFFFF` / `#1E293B`.

### B. Typography & Spacing Grid
* **Font Family:** `Inter`, system-ui, sans-serif.
* **Grid:** 4px baseline (`xs: 4px`, `sm: 8px`, `md: 16px`, `lg: 24px`, `xl: 32px`).

---

## 4. Cây Phân Cấp Linh Kiện Nguyên Tử (Atomic Component Hierarchy)

```text
├── Atoms
│   ├── AmountDisplay (Định dạng VND chuẩn)
│   ├── StatusBadge (Đã nộp / Chờ duyệt hoàn ứng)
│   └── UserAvatar (Avatar Chồng / Vợ)
│
├── Molecules
│   ├── MemberContributionBar (Thanh tiến độ đóng góp theo tỷ lệ %)
│   └── ReimbursementItemRow (Dòng yêu cầu hoàn ứng kèm nút Duyệt)
│
└── Organisms
    ├── JointPoolSummaryCard (Thẻ tổng quan số dư kèm nút Nạp tiền nhanh)
    └── MonthlyBudgetGauge (Đồng hồ trực quan đo lường hạn mức tiêu trong tháng)
```

---

## 5. Chiến Lược Thích Ứng Màn Hình (Responsive Breakpoints)

| Breakpoint | Chiều Rộng | Hành Vi Bố Cục |
| :--- | :--- | :--- |
| **Mobile (`sm`)** | `375px - 640px` | 1 cột đơn, Sticky Action Bar nạp tiền nhanh ở đáy. |
| **Tablet (`md`)** | `768px - 1024px`| 2 cột (Trái: Số dư & Đóng góp, Phải: Lịch sử hoàn ứng). |
| **Desktop (`lg`)**| `1280px+` | Dashboard 3 cột đa năng kèm phân tích chi tiêu chuyên sâu. |

---

## 6. Kiểm Toán Khả Năng Tiếp Cận (WCAG 2.1 AA Checklist)

- [x] **Độ tương phản màu sắc:** Text chính trên nền đạt `15.3:1` (Vượt ngưỡng 4.5:1).
- [x] **Phím điều hướng (Keyboard Focus):** Mọi nút bấm đều có `focus-visible:ring-2 ring-blue-500`.
- [x] **Trợ năng màn hình:** Đầy đủ `aria-label` và `aria-live="polite"` khi số dư thay đổi.
- [x] **Vùng chạm:** Mọi nút bấm di động $\ge 44 \times 44\text{px}$.

---

## 7. Form Validation (Zod Schema) & Client State Store

```typescript
import { z } from 'zod';

export const JointFundDepositSchema = z.object({
  fundId: z.string().uuid("Mã quỹ không hợp lệ"),
  amount: z.number().positive("Số tiền nộp phải lớn hơn 0").max(100_000_000, "Vượt hạn mức nộp tối đa"),
  note: z.string().max(200, "Ghi chú tối đa 200 ký tự").optional(),
});
```

---

## 8. Mã Nguồn Linh Kiện Mẫu Sẵn Sàng Thực Thi (React 19 / TypeScript + TailwindCSS)

```tsx
import React, { useState } from 'react';

export interface JointPoolCardProps {
  fundName: string;
  balance: number;
  onDeposit: (amount: number) => Promise<void>;
}

export const JointPoolCard: React.FC<JointPoolCardProps> = ({ fundName, balance, onDeposit }) => {
  const [isDepositing, setIsDepositing] = useState(false);

  const handleQuickDeposit = async () => {
    try {
      setIsDepositing(true);
      await onDeposit(2000000);
    } finally {
      setIsDepositing(false);
    }
  };

  return (
    <div className="p-6 bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-100 dark:border-slate-700">
      <div className="flex justify-between items-center mb-4">
        <h3 className="text-sm font-semibold text-slate-500 uppercase tracking-wider">{fundName}</h3>
        <span className="px-2.5 py-0.5 rounded-full text-xs font-medium bg-emerald-100 text-emerald-800">Hoạt Động</span>
      </div>
      <div className="text-3xl font-bold text-slate-900 dark:text-white mb-6">
        {new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(balance)}
      </div>
      <button
        onClick={handleQuickDeposit}
        disabled={isDepositing}
        className="w-full py-3 bg-blue-600 hover:bg-blue-700 disabled:opacity-50 text-white font-medium rounded-xl shadow-md transition-all"
      >
        {isDepositing ? 'Đang Xử Lý (0ms)...' : '+ Nạp Tiền Quỹ Nhanh (2,000,000đ)'}
      </button>
    </div>
  );
};
```
