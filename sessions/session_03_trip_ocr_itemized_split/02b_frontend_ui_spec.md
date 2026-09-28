# ĐẶC TẢ GIAO DIỆN & TRẢI NGHIỆM NGƯỜI DÙNG (FRONTEND UI/UX SPECIFICATION)
**Tên Tính Năng:** BÓC TÁCH HÓA ĐƠN CHUYẾN ĐI BẰNG MULTIMODAL AI OCR  
**Mã Ca Sử Dụng:** `UC-OCR-03`  
**Chuyên Gia Thiết Kế:** Brad Frost & Dan Abramov  
**Tiêu Chuẩn Tuân Thủ:** WCAG 2.1 AA, Atomic Design, Optimistic UI (0ms)  

---

## 1. Bản Vẽ Khung Nhìn Giao Diện (Wireframe) & Luồng Màn Hình
```text
[Chụp Ảnh Bill] ──► [Tiến Trình OCR % 0ms] ──► [Đối Soát & Chọn Món]
```

---

## 2. Hệ Thống Token Thiết Kế & Cây Linh Kiện
- **Tokens:** Primary `#2563EB`, Success `#16A34A`, Warning `#D97706`. Contrast `15.3:1`.
- **Components:** Atoms (`ConfidenceScoreBadge`), Molecules (`ReceiptItemRow`), Organisms (`ReceiptScanAuditPanel`).

---

## 3. Form Validation (Zod) & Mã Nguồn Mẫu (React 19 / TypeScript)
```typescript
import { z } from 'zod';
export const ItemSchema = z.object({ name: z.string().min(1), price: z.number().positive() });
```
