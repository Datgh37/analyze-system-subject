# ĐẶC TẢ GIAO DIỆN & TRẢI NGHIỆM NGƯỜI DÙNG (FRONTEND UI/UX SPECIFICATION)
**Tên Tính Năng:** RÚT GỌN ĐỒ THỊ NỢ TỐI GIẢN & THANH TOÁN VIETQR NHANH  
**Mã Ca Sử Dụng:** `UC-VIETQR-04`  
**Chuyên Gia Thiết Kế:** Brad Frost (Atomic Design System) & Dan Abramov (Client Architecture)  
**Tiêu Chuẩn Tuân Thủ:** WCAG 2.1 AA, Atomic Design System, Mobile-First Responsive, Optimistic UI (0ms)  

---

## 1. Sơ Đồ Luồng Màn Hình & Điều Hướng (User Flow & Screen Map)

```text
[Màn hình Danh Sách Nợ Chằng Chịt (N*N-1/2 Giao Dịch)]
       │ (Bấm "⚡ Rút Gọn Nợ Bằng Thuật Toán Tarjan")
       ▼
[Màn hình Biểu Đồ Đồ Thị Nợ Tối Giản (N-1 Giao Dịch)]
       │ (Chọn giao dịch cần thanh toán)
       ▼
[Modal VietQR Chuẩn NAPAS 247 Kèm Mã Giao Dịch]
       ├──► (Quét mã trên App Ngân Hàng) ──► [Webhook / Polling xác nhận]
       └──► (Xác nhận thủ công) ──► [Optimistic UI Đánh Dấu 'ĐÃ XONG' (0ms)]
```

---

## 2. Bản Vẽ Khung Nhìn Giao Diện (ASCII Wireframes)

### A. Màn Hình Di Động (Mobile Viewport - 375px)
```text
┌──────────────────────────────────────┐
│ [←] BIỂU ĐỒ NỢ ĐÃ RÚT GỌN       [?] │
├──────────────────────────────────────┤
│ ┌──────────────────────────────────┐ │
│ │ 🎯 KẾT QUẢ RÚT GỌN NỢ:           │ │
│ │ Giảm từ 10 giao dịch xuống còn 3!│ │
│ └──────────────────────────────────┘ │
│                                      │
│ 📋 BẠN CẦN CHUYỂN KHOẢN:             │
│ ┌──────────────────────────────────┐ │
│ │ 👤 Chuyển cho: AN TRẦN           │ │
│ │ 💵 Số tiền:    450,000 VND       │ │
│ │ 🏦 Techcombank - 1903456789      │ │
│ │ [===== 📱 MỞ MÃ VIETQR =====]    │ │
│ └──────────────────────────────────┘ │
│                                      │
│ [=========== ĐÃ CHUYỂN TIỀN ========] │
└──────────────────────────────────────┘
```

---

## 3. Hệ Thống Token Thiết Kế & Cây Linh Kiện Atomic

* **Tokens:** Brand Blue `#2563EB`, QR Background `#FFFFFF`, Status Paid `#16A34A`.
* **Atomic Components:**
  - **Atoms:** `VietQRCodeCanvas`, `BankLogoIcon`, `CopyButton` (sao chép số tài khoản 1 chạm).
  - **Molecules:** `DebtTransferCard` (Avatar Người nhận + Số tiền + Thông tin ngân hàng + Nút mở QR).
  - **Organisms:** `DebtGraphMermaidViewer` (Khung vẽ đồ thị trực quan) và `VietQRModal` (Popup chứa mã QR động).

---

## 4. Form Validation & Client State Store Mapping

```typescript
import { z } from 'zod';

export const VietQRPaymentSchema = z.object({
  debtId: z.string().uuid(),
  recipientAccount: z.string().min(6).max(20),
  bankBin: z.string().length(6, "Mã BIN ngân hàng gồm 6 số"),
  amount: z.number().positive(),
  paymentRef: z.string().min(4),
});
```

---

## 5. Mã Nguồn Linh Kiện Mẫu Sẵn Sàng Thực Thi (React 19 / TypeScript)

```tsx
import React, { useState } from 'react';

export interface VietQRPaymentCardProps {
  recipientName: string;
  bankName: string;
  accountNo: string;
  amount: number;
  qrPayloadUrl: string;
  onConfirmSettled: () => Promise<void>;
}

export const VietQRPaymentCard: React.FC<VietQRPaymentCardProps> = ({
  recipientName,
  bankName,
  accountNo,
  amount,
  qrPayloadUrl,
  onConfirmSettled,
}) => {
  const [copied, setCopied] = useState(false);

  const copyAccount = () => {
    navigator.clipboard.writeText(accountNo);
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  return (
    <div className="p-5 bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-100 dark:border-slate-700">
      <div className="flex justify-between items-center mb-3">
        <h4 className="text-sm font-semibold text-slate-700 dark:text-slate-200">Chuyển cho: {recipientName}</h4>
        <span className="text-xs px-2 py-0.5 rounded-full bg-blue-50 text-blue-700 font-medium">{bankName}</span>
      </div>
      <div className="text-2xl font-bold text-blue-600 dark:text-blue-400 mb-2">
        {new Intl.NumberFormat('vi-VN').format(amount)} VND
      </div>
      <div className="flex justify-between items-center text-xs text-slate-500 mb-4 bg-slate-50 dark:bg-slate-900/50 p-2.5 rounded-lg">
        <span>STK: <strong className="text-slate-800 dark:text-slate-200">{accountNo}</strong></span>
        <button onClick={copyAccount} className="text-blue-600 font-medium hover:underline">
          {copied ? '✓ Đã chép' : 'Sao chép'}
        </button>
      </div>
      <button
        onClick={onConfirmSettled}
        className="w-full py-2.5 bg-emerald-600 hover:bg-emerald-700 text-white font-medium rounded-xl transition-all shadow-sm"
      >
        ✓ Đã Chuyển Khoản Xong
      </button>
    </div>
  );
};
```
