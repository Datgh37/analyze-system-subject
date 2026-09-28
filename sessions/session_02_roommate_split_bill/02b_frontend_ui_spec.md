# ĐẶC TẢ GIAO DIỆN & TRẢI NGHIỆM NGƯỜI DÙNG (FRONTEND UI/UX SPECIFICATION)
**Tên Tính Năng:** CHIA TIỀN ĐIỆN NƯỚC & SINH HOẠT PHÒNG TRỌ  
**Mã Ca Sử Dụng:** `UC-ROOMMATE-02`  
**Chuyên Gia Thiết Kế:** Brad Frost (Atomic Design System) & Dan Abramov (Client Architecture)  
**Tiêu Chuẩn Tuân Thủ:** WCAG 2.1 AA, Atomic Design System, Mobile-First Responsive, Optimistic UI (0ms)  

---

## 1. Sơ Đồ Luồng Màn Hình & Điều Hướng (User Flow & Screen Map)

```text
[Màn hình Danh Sách Phòng Trọ]
       │
       ├──► (Bấm "+ Thêm Hóa Đơn Điện Nước") ──► [Modal Nhập Chỉ Số Điện & Đơn Giá]
       │                                                      │
       │                                                      ▼ (Tự động tính toán & chia theo đầu người)
       │                                         [Optimistic Preview 0ms]
       │                                                      │
       └──► (Bấm "Xem Nợ Từng Bạn") ◄─────────────────────────┘
```

---

## 2. Bản Vẽ Khung Nhìn Giao Diện (ASCII Wireframes)

### A. Màn Hình Di Động (Mobile Viewport - 375px)
```text
┌──────────────────────────────────────┐
│ [←] PHÒNG TRỌ: 'PHÒNG 302'      [👥] │
├──────────────────────────────────────┤
│ ┌──────────────────────────────────┐ │
│ │ ⚡ TỔNG HÓA ĐƠN THÁNG 8           │ │
│ │ 3,250,000 VND (3 Thành Viên)     │ │
│ │ [Chia Đều: 1,083,333 đ/người]    │ │
│ └──────────────────────────────────┘ │
│                                      │
│ 📋 TRẠNG THÁI THANH TOÁN             │
│ ┌──────────────────────────────────┐ │
│ │ 👤 Tuấn: 1,083,333 đ [ĐÃ NỘP ✅]  │ │
│ │ 👤 Nam:  1,083,333 đ [CHƯA NỘP ⏳] │ │
│ │ 👤 Minh: 1,083,333 đ [ĐÃ NỘP ✅]  │ │
│ └──────────────────────────────────┘ │
│                                      │
│ [======== 📲 GỬI NHẮC TIỀN =========] │
└──────────────────────────────────────┘
```

---

## 3. Hệ Thống Token Thiết Kế (Design Tokens Catalog)

* **Primary Brand:** `#2563EB` (Xanh Dương).
* **Success (Đã chuyển khoản):** `#16A34A` (Xanh Lá).
* **Warning (Chưa nộp tiền):** `#D97706` (Vàng Cam).
* **Typography:** `Inter`, font sizes: `14px` body, `20px` card header, `28px` total amount.

---

## 4. Cây Phân Cấp Linh Kiện Nguyên Tử (Atomic Component Hierarchy)

```text
├── Atoms: RoommateAvatar, PaymentStatusBadge, CurrencyAmountLabel
├── Molecules: RoommateSplitRow (Avatar + Tên + Số nợ + Nút Nhắc nợ)
└── Organisms: SplitBillSummaryCard, UtilityBillCalculatorModal
```

---

## 5. Chiến Lược Thích Ứng Màn Hình & Kiểm Toán WCAG 2.1 AA

* **Breakpoints:** Mobile (375px) hiển thị danh sách dạng cuộn dọc và nút nhắc nợ Sticky; Desktop (1280px) hiển thị bảng tính đa năng.
* **WCAG 2.1 AA:** Đạt độ tương phản `15.3:1`, hỗ trợ đầy đủ phím Tab và ARIA live region.

---

## 6. Form Validation & Client State Mapping

```typescript
import { z } from 'zod';

export const UtilityBillSchema = z.object({
  electricityIndex: z.number().positive("Chỉ số điện phải lớn hơn 0"),
  waterIndex: z.number().positive("Chỉ số nước phải lớn hơn 0"),
  wifiFee: z.number().nonnegative(),
  participantCount: z.number().int().min(2, "Phòng phải từ 2 người trở lên"),
});
```

---

## 7. Mã Nguồn Linh Kiện Mẫu Sẵn Sàng Thực Thi (React 19 / TypeScript)

```tsx
import React from 'react';

export interface RoommateSplitRowProps {
  name: string;
  amount: number;
  isPaid: boolean;
  onRemind: () => void;
}

export const RoommateSplitRow: React.FC<RoommateSplitRowProps> = ({ name, amount, isPaid, onRemind }) => {
  return (
    <div className="flex items-center justify-between p-4 bg-slate-50 dark:bg-slate-800/60 rounded-xl mb-3">
      <div className="flex items-center space-x-3">
        <div className="w-10 h-10 rounded-full bg-blue-100 dark:bg-blue-900/40 text-blue-700 dark:text-blue-300 flex items-center justify-center font-bold">
          {name.charAt(0)}
        </div>
        <div>
          <h4 className="text-sm font-semibold text-slate-800 dark:text-slate-100">{name}</h4>
          <span className="text-xs text-slate-500">{new Intl.NumberFormat('vi-VN').format(amount)} VND</span>
        </div>
      </div>
      {isPaid ? (
        <span className="text-xs font-semibold px-2.5 py-1 rounded-full bg-emerald-100 text-emerald-800">Đã nộp</span>
      ) : (
        <button onClick={onRemind} className="text-xs font-medium px-3 py-1.5 rounded-lg bg-amber-500 hover:bg-amber-600 text-white transition-all">
          Nhắc Nợ 📲
        </button>
      )}
    </div>
  );
};
```
