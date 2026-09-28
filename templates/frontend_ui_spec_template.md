# ĐẶC TẢ GIAO DIỆN & TRẢI NGHIỆM NGƯỜI DÙNG (FRONTEND UI/UX SPECIFICATION)
**Tên Tính Năng:** [TÊN TÍNH NĂNG]  
**Mã Ca Sử Dụng (Use Case ID):** [UC-ID]  
**Chuyên Gia Thiết Kế:** Brad Frost (Atomic Design System) & Dan Abramov (Client State & Architecture)  
**Tiêu Chuẩn Tuân Thủ:** WCAG 2.1 AA, Atomic Design, Mobile-First Responsive, Optimistic UI (0ms)  

---

## 1. Sơ Đồ Luồng Màn Hình & Điều Hướng (User Flow & Screen Map)

```text
[Màn hình Danh Sách / Dashboard]
       │
       ▼ (Bấm "+ Thao Tác")
[Màn hình Nhập Liệu / Modal Form] ──── (Lỗi Form Validation) ──► [Hiển thị Toast / Inline Alert]
       │ (Hợp lệ & Xác nhận)
       ▼ (Optimistic UI 0ms Update)
[Cập nhật State Store tức thì] ─────── (Background Network Sync) ──► [Đồng bộ CSDL Backend]
       │                                                                  │
       ▼                                                                  ▼
[Màn hình Chi Tiết / Biểu Đồ] ◄───────────────────────────────────── (Thành công)
```

---

## 2. Bản Vẽ Khung Nhìn Giao Diện (ASCII Wireframes)

### A. Màn Hình Di Động (Mobile Viewport - 375px)
```text
┌──────────────────────────────────────┐
│ [←] [TÊN TÍNH NĂNG]             [🔔] │
├──────────────────────────────────────┤
│ ┌──────────────────────────────────┐ │
│ │ 💰 TỔNG SỐ TIỀN / SỐ DƯ QUỸ      │ │
│ │ 24,500,000 VND                   │ │
│ │ [Nạp Tiền]       [Rút Tiền]      │ │
│ └──────────────────────────────────┘ │
│                                      │
│ 📋 HOẠT ĐỘNG GẦN ĐÂY                 │
│ ┌──────────────────────────────────┐ │
│ │ 🛒 Tiền Chợ Hôm Nay   -350,000đ  │ │
│ │ ⚡ Tiền Điện Tháng 8 -1,200,000đ  │ │
│ └──────────────────────────────────┘ │
│                                      │
│ [=========== (+) THAO TÁC ==========] │
└──────────────────────────────────────┘
```

### B. Màn Hình Máy Tính (Desktop / Web Viewport - 1280px Grid)
```text
┌─────────────────────────────────────────────────────────────────────────────┐
│ [Logo Fintech]  Dashboard   Quỹ Chung   Báo Cáo   Cài Đặt      [User: An Tran] │
├───────────────────────────────┬─────────────────────────────────────────────┤
│ 📊 THÔNG TIN TỔNG QUAN        │ 📈 BIỂU ĐỒ CHI TIÊU & ĐỒ THỊ NỢ             │
│ - Tên Quỹ: Quỹ Sinh Hoạt      │                                             │
│ - Số Dư: 24,500,000 VND       │   [ Biểu đồ cột phân bổ theo danh mục ]     │
│ - Thành Viên: 4 người         │   [ Sơ đồ luồng rút gọn nợ Mermaid ]        │
│                               │                                             │
│ 📋 DANH SÁCH KHOẢN CHI        │ ⚡ THAO TÁC NHANH                           │
│ [Bảng dữ liệu 3NF có phân trang, lọc]│ [Tạo Khoản Chi]  [Quét Hóa Đơn OCR] [VietQR]│
└───────────────────────────────┴─────────────────────────────────────────────┘
```

---

## 3. Hệ Thống Token Thiết Kế (Design Tokens Catalog)

### A. Bảng Màu Chuẩn HSL (Harmonious Color Palette)
* **Primary (Chủ đạo):** `hsl(221, 83%, 53%)` — `#2563EB` (Xanh Fintech Tin Cậy).
* **Success (Thành công / Tiết kiệm):** `hsl(142, 71%, 45%)` — `#16A34A` (Xanh Lá).
* **Warning (Cảnh báo vượt ngân sách):** `hsl(38, 92%, 50%)` — `#D97706` (Vàng Cam).
* **Danger (Lỗi / Chi quá mức):** `hsl(0, 84%, 60%)` — `#DC2626` (Đỏ).
* **Surface Light / Dark:** `hsl(0, 0%, 100%)` / `hsl(222, 47%, 11%)` (Giao diện Dark Mode).

### B. Thang Đo Typography & Spacing (4px / 8px Grid)
* **Font Family:** `Inter`, `-apple-system`, `sans-serif`.
* **Font Sizes:** Heading 1 (`24px/32px Bold`), Body (`14px/20px Regular`), Caption (`12px/16px Medium`).
* **Spacing:** `xs: 4px`, `sm: 8px`, `md: 16px`, `lg: 24px`, `xl: 32px`.

---

## 4. Cây Phân Cấp Linh Kiện Nguyên Tử (Atomic Component Hierarchy)

```text
├── Atoms (Nguyên tử)
│   ├── AmountDisplay (Hiển thị tiền tệ định dạng VND/USD)
│   ├── PrimaryButton (Nút bấm tương tác có trạng thái Loading/Disabled)
│   └── StatusBadge (Huy hiệu trạng thái: Pending, Approved, Settled)
│
├── Molecules (Phân tử)
│   ├── ExpenseItemRow (Dòng giao dịch gồm Icon + Tiêu đề + Giá tiền + Người nộp)
│   ├── FormCurrencyInput (Ô nhập tiền tệ kèm bàn phím số & gợi ý số tiền)
│   └── UserAvatarGroup (Nhóm avatar các thành viên đóng góp)
│
└── Organisms (Sinh vật)
    ├── JointPoolSummaryCard (Thẻ tóm tắt tình trạng quỹ kèm thanh tiến độ ngân sách)
    ├── DebtGraphVisualizer (Khung hiển thị trực quan các mũi tên chuyển tiền)
    └── ReceiptUploadDropzone (Khu vực kéo thả hóa đơn quét OCR)
```

---

## 5. Chiến Lược Thích Ứng Màn Hình (Responsive Breakpoints)

| Breakpoint | Chiều Rộng (Width) | Bố Cục Giao Diện (Layout Behavior) |
| :--- | :--- | :--- |
| **Mobile (`sm`)** | `375px - 640px` | 1 cột đơn, Bottom Sheet cho Form nhập liệu, nút bấm cố định đáy màn hình (Sticky CTA). |
| **Tablet (`md`)** | `768px - 1024px`| 2 cột (Trái: Danh sách, Phải: Chi tiết hoặc biểu đồ rút gọn nợ). |
| **Desktop (`lg`)**| `1280px+` | Dashboard 3 cột đa năng, Data Table đầy đủ bộ lọc, Modal popup giữa màn hình. |

---

## 6. Bảng Kiểm Tra Khả Năng Tiếp Cận (WCAG 2.1 AA Checklist)

- [x] **Độ tương phản màu sắc (Contrast Ratio):** Văn bản chính trên nền đạt $\ge 4.5:1$ (Thực tế: $15.3:1$).
- [x] **Tiếp cận bằng bàn phím (Keyboard Navigation):** Toàn bộ nút bấm và ô nhập liệu đều có `focus-visible: ring-2 ring-blue-500`.
- [x] **Trợ năng màn hình đọc (Screen Reader / ARIA):** Sử dụng đúng `aria-label`, `role="alert"` cho thông báo lỗi, `aria-live="polite"` cho cập nhật số dư.
- [x] **Vùng chạm cảm ứng (Touch Target Size):** Mọi nút bấm trên Mobile đều có chiều cao $\ge 44 \times 44\text{px}$.

---

## 7. Quy Tắc Xác Thực Dữ Liệu Form & State Mapping (Zod Schema / DTO)

### A. Schema Kiểm Tra Tính Hợp Lệ (Zod Schema)
```typescript
import { z } from 'zod';

export const ExpenseFormSchema = z.object({
  title: z.string().min(3, "Tên khoản chi phải từ 3 ký tự trở lên").max(100),
  amount: z.number().positive("Số tiền phải lớn hơn 0").max(100_000_000, "Vượt hạn mức tối đa"),
  categoryId: z.string().uuid("Danh mục không hợp lệ"),
  splitMethod: z.enum(["EQUAL", "PERCENTAGE", "EXACT_AMOUNT"]),
  participantIds: z.array(z.string().uuid()).min(1, "Phải có ít nhất 1 người tham gia"),
});
```

### B. Client Store Slice (Zustand / Framework-Agnostic State)
```typescript
interface JointFundState {
  balance: number;
  isSyncing: boolean;
  optimisticAddExpense: (expense: ExpenseInput) => void;
  rollbackExpense: (tempId: string) => void;
}
```

---

## 8. Mã Nguồn Linh Kiện Mẫu Sẵn Sàng Thực Thi (Production Component Implementation)

### Bản Triển Khai: React 19 / TypeScript + TailwindCSS
```tsx
import React, { useState } from 'react';

export interface JointPoolCardProps {
  fundName: string;
  balance: number;
  currency?: string;
  onAddExpense: () => void;
}

export const JointPoolCard: React.FC<JointPoolCardProps> = ({
  fundName,
  balance,
  currency = 'VND',
  onAddExpense,
}) => {
  return (
    <div className="p-6 bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-100 dark:border-slate-700 transition-all hover:shadow-xl">
      <div className="flex justify-between items-center mb-4">
        <h3 className="text-sm font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider">
          {fundName}
        </h3>
        <span className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-emerald-100 text-emerald-800 dark:bg-emerald-900/40 dark:text-emerald-300">
          Hoạt Động
        </span>
      </div>
      <div className="mb-6">
        <span className="text-3xl font-bold text-slate-900 dark:text-white tracking-tight">
          {new Intl.NumberFormat('vi-VN').format(balance)}
        </span>
        <span className="ml-2 text-sm font-medium text-slate-500 dark:text-slate-400">
          {currency}
        </span>
      </div>
      <button
        onClick={onAddExpense}
        className="w-full py-3 px-4 rounded-xl bg-blue-600 hover:bg-blue-700 text-white font-medium shadow-md shadow-blue-500/20 active:scale-[0.98] transition-all focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
      >
        + Thêm Khoản Chi Mới
      </button>
    </div>
  );
};
```
