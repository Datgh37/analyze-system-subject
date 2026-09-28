# Bản Quyết Định Kiến Trúc (Architecture Decision Record - ADR)

**Mã ADR:** `ADR-[Số thứ tự, vd: ADR-01]`  
**Tiêu đề:** `[Tên quyết định, vd: Áp dụng Giải thuật Min-Cost Max-Flow cho Debt Simplification]`  
**Trạng thái:** `[PROPOSED / ACCEPTED / DEPRECATED / REPLACED]`  
**Ngày quyết định:** `YYYY-MM-DD`  
**Chuyên gia đề xuất:** `[Software Architect / Backend & Data Specialist]`  

---

## 1. Bối Cảnh & Vấn Đề (Context & Problem Statement)
- *Vấn đề kỹ thuật cần giải quyết là gì?*
- *Tại sao cách làm truyền thống không đáp ứng được yêu cầu (về hiệu năng, độ phức tạp, tính đúng đắn)?*
- *Các ràng buộc hiện tại (thời gian, hạ tầng, công nghệ).*

---

## 2. Các Phương Án Được Xem Xét (Options Considered)

| Phương Án | Mô tả ngắn gọn | Ưu điểm | Nhược điểm / Rủi ro |
| :--- | :--- | :--- | :--- |
| **Option 1 (Lựa chọn)** | `...` | `...` | `...` |
| **Option 2 (Thay thế A)** | `...` | `...` | `...` |
| **Option 3 (Thay thế B)** | `...` | `...` | `...` |

---

## 3. Quyết Định Được Chốt (Decision Outcome)
- **Phương án được chọn:** `[Tên phương án]`
- **Lý do quyết định (Rationale):**
  - `[Lý do 1 - Phù hợp với năng lực hệ thống]: ...`
  - `[Lý do 2 - Đảm bảo tính mở rộng và dễ bảo trì]: ...`
  - `[Lý do 3 - Điểm nhấn kỹ thuật cho Đồ án Tốt nghiệp]: ...`

---

## 4. Hệ Quả Kỹ Thuật (Consequences)
- **Tác động tích cực (+):**
  - `[Ví dụ: Giảm số lượng giao dịch chuyển khoản giữa các thành viên từ N^2 xuống tối đa N-1]`
  - `[Ví dụ: Tách biệt logic tính toán ra Domain Service, không làm bẩn tầng Controller]`
- **Tác động tiêu cực / Thách thức cần xử lý (-):**
  - `[Ví dụ: Tăng độ phức tạp thuật toán O(V * E), cần viết Unit Test độ phủ cao]`
  - `[Ví dụ: Cần quản lý trạng thái giao dịch cẩn thận khi có thành viên thanh toán một phần]`

---

## 5. Hướng Dẫn Triển Khai (Implementation Notes)
- **Tầng mã nguồn:** `[Domain / Application / Infrastructure / API]`
- **Công nghệ / Thư viện dự kiến:** `[...]`
- **Mã giả / Sơ đồ luồng (Pseudocode / Flowchart):**
```text
[Mã giả hoặc sơ đồ minh họa nếu có]
```
