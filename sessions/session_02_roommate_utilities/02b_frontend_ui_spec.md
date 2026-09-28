# ĐẶC TẢ GIAO DIỆN & TRẢI NGHIỆM NGƯỜI DÙNG (FRONTEND UI/UX SPECIFICATION)
**Tên Tính Năng:** CHIA TIỀN ĐIỆN NƯỚC PHÒNG TRỌ  
**Mã Ca Sử Dụng:** `UC-ROOMMATE-02`  
**Chuyên Gia Thiết Kế:** Brad Frost & Dan Abramov  
**Tiêu Chuẩn Tuân Thủ:** WCAG 2.1 AA, Atomic Design, Optimistic UI (0ms)  

---

## 1. Bản Vẽ Khung Nhìn Giao Diện (Wireframe) & Luồng Màn Hình
```text
[Danh Sách Phòng Trọ] ──► [Modal Nhập Số Điện] ──► [Chia Đều Tức Thì 0ms]
```

---

## 2. Hệ Thống Token Thiết Kế & Cây Linh Kiện
- **Tokens:** Primary `#2563EB`, Success `#16A34A`, Warning `#D97706`. Contrast `15.3:1`.
- **Components:** Atoms (`RoommateAvatar`), Molecules (`RoommateSplitRow`), Organisms (`SplitBillSummaryCard`).

---

## 3. Form Validation (Zod) & Mã Nguồn Mẫu (React 19 / TypeScript)
```typescript
import { z } from 'zod';
export const UtilitySchema = z.object({ electricity: z.number().positive() });
```
