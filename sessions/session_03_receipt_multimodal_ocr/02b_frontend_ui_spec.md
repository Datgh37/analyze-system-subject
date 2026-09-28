# ĐẶC TẢ GIAO DIỆN & TRẢI NGHIỆM NGƯỜI DÙNG (FRONTEND UI/UX SPECIFICATION)
**Tên Tính Năng:** BÓC TÁCH HÓA ĐƠN MULTIMODAL AI OCR & ĐỐI SOÁT TOÁN HỌC  
**Mã Ca Sử Dụng:** `UC-OCR-03`  
**Chuyên Gia Thiết Kế:** Brad Frost (Atomic Design System) & Dan Abramov (Client Architecture)  
**Tiêu Chuẩn Tuân Thủ:** WCAG 2.1 AA, Atomic Design System, Mobile-First Responsive, Optimistic UI (0ms)  

---

## 1. Sơ Đồ Luồng Màn Hình & Điều Hướng (User Flow & Screen Map)

```text
[Màn hình Chụp Ảnh / Tải Hóa Đơn]
       │ (Chọn ảnh hóa đơn)
       ▼
[Modal Xem Trước Ảnh & Tiến Trình OCR] ──── (Worker xử lý ngầm 3s) ──► [Tiến trình % sống động]
       │
       ▼ (Hoàn tất bóc tách)
[Màn hình Đối Soát & Chọn Món Trực Quan]
       ├──► (Tất cả món khớp tổng tiền: Toán học 100%) ──► [Nút "Xác Nhận" màu Xanh]
       └──► (Phát hiện lệch tiền: Mathematical Audit Warning) ──► [Highlighter màu Vàng cảnh báo sửa tay]
```

---

## 2. Bản Vẽ Khung Nhìn Giao Diện (ASCII Wireframes)

### A. Màn Hình Di Động (Mobile Viewport - 375px)
```text
┌──────────────────────────────────────┐
│ [←] ĐỐI SOÁT HÓA ĐƠN OCR        [?] │
├──────────────────────────────────────┤
│ ┌──────────────────────────────────┐ │
│ │ 🖼️ [ẢNH HÓA ĐƠN: NHÀ HÀNG SEN]    │ │
│ │ Độ chính xác AI: 96.5% [KHỚP ✅] │ │
│ └──────────────────────────────────┘ │
│                                      │
│ 📋 DANH SÁCH MÓN ĂN ĐÃ BÓC TÁCH:     │
│ ┌──────────────────────────────────┐ │
│ │ [✓] 1. Lẩu Thái Tomyum  350,000đ │ │
│ │     (Ai ăn? [An] [Bình] [Chi])   │ │
│ │ [✓] 2. Bò Mỹ Cuộn Nấm   180,000đ │ │
│ │     (Ai ăn? [An] [Chi])          │ │
│ │ [✓] 3. Trà Đào (x3)      90,000đ │ │
│ └──────────────────────────────────┘ │
│                                      │
│ 💰 TỔNG CỘNG: 620,000 VND [KHỚP BILL]│
│ [=========== XÁC NHẬN CHIA =========] │
└──────────────────────────────────────┘
```

---

## 3. Hệ Thống Token Thiết Kế & Cây Linh Kiện Atomic

* **Tokens:** Primary `#2563EB`, Success `#16A34A`, Warning `#D97706` (khi lệch tổng tiền), Neutral Background `#F8FAFC`.
* **Atomic Components:**
  - **Atoms:** `ConfidenceScoreBadge`, `CurrencyAmountInput`, `ItemCheckbox`.
  - **Molecules:** `ReceiptItemRow` (Tên món + Giá tiền + Bộ chọn người ăn + Nút chỉnh sửa).
  - **Organisms:** `ReceiptScanAuditPanel` (Vùng đối soát tổng tiền món với tổng bill) và `ReceiptImagePreviewer`.

---

## 4. Form Validation & Client State Store Mapping

```typescript
import { z } from 'zod';

export const ReceiptItemSchema = z.object({
  name: z.string().min(1, "Tên món không được để trống"),
  quantity: z.number().int().positive(),
  unitPrice: z.number().positive(),
  totalPrice: z.number().positive(),
  assignedMemberIds: z.array(z.string().uuid()).min(1, "Chọn ít nhất 1 người ăn món này"),
});
```

---

## 5. Mã Nguồn Linh Kiện Mẫu Sẵn Sàng Thực Thi (React 19 / TypeScript)

```tsx
import React from 'react';

export interface ReceiptItemRowProps {
  name: string;
  price: number;
  assignedCount: number;
  isSelected: boolean;
  onToggle: () => void;
}

export const ReceiptItemRow: React.FC<ReceiptItemRowProps> = ({
  name,
  price,
  assignedCount,
  isSelected,
  onToggle,
}) => {
  return (
    <div
      onClick={onToggle}
      className={`p-3.5 rounded-xl border flex justify-between items-center cursor-pointer transition-all ${
        isSelected ? 'bg-blue-50/60 border-blue-500/80 dark:bg-blue-950/30' : 'bg-white border-slate-200'
      }`}
    >
      <div className="flex items-center space-x-3">
        <input type="checkbox" checked={isSelected} readOnly className="w-4 h-4 text-blue-600 rounded" />
        <div>
          <p className="text-sm font-medium text-slate-800 dark:text-slate-100">{name}</p>
          <span className="text-xs text-slate-400">{assignedCount} người chia món này</span>
        </div>
      </div>
      <span className="font-semibold text-slate-900 dark:text-white">
        {new Intl.NumberFormat('vi-VN').format(price)} đ
      </span>
    </div>
  );
};
```
