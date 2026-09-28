# ĐẶC TẢ GIAO DIỆN & TRẢI NGHIỆM NGƯỜI DÙNG (FRONTEND UI/UX SPECIFICATION)
**Tên Tính Năng:** CẢNH BÁO TỐC ĐỘ CHI TIÊU & CÚ HÍCH NUDGE  
**Mã Ca Sử Dụng:** `UC-NUDGE-04`  
**Chuyên Gia Thiết Kế:** Brad Frost & Dan Abramov  
**Tiêu Chuẩn Tuân Thủ:** WCAG 2.1 AA, Atomic Design, Optimistic UI (0ms)  

---

## 1. Bản Vẽ Khung Nhìn Giao Diện (Wireframe) & Luồng Màn Hình
```text
[Dashboard Cá Nhân] ──► [Banner Cảnh Báo Tốc Độ Tiêu Tiền] ──► [Gợi Ý Tiết Kiệm Nudge]
```

---

## 2. Hệ Thống Token Thiết Kế & Cây Linh Kiện
- **Tokens:** Primary `#2563EB`, Success `#16A34A`, Warning `#D97706`, Danger `#DC2626`. Contrast `15.3:1`.
- **Components:** Atoms (`SpeedometerGauge`), Molecules (`NudgeBannerCard`), Organisms (`MonthlyBudgetOptimizer`).

---

## 3. Form Validation (Zod) & Mã Nguồn Mẫu (React 19 / TypeScript)
```typescript
import { z } from 'zod';
export const NudgePrefSchema = z.object({ alertThreshold: z.number().min(50).max(100) });
```
