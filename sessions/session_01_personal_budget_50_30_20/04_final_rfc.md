# FINAL RFC: THIẾT LẬP NGÂN SÁCH CÁ NHÂN 50/30/20 & HẠN MỨC NGÀY

### 1. Tóm Tắt Quyết Định Hội Đồng
Hội đồng 13 Chuyên gia đã đồng thuận 100% phê duyệt tính năng Hoạch định Ngân sách Cá nhân theo quy tắc 50/30/20 và Zero-Based Budgeting.

### 2. Ma Trận Truy Vết Nguồn Gốc (Traceability Matrix)
| Mã Yêu Cầu (REQ ID) | Ca Sử Dụng (Use Case) | Giao Diện (UI Spec) | Quyết Định Kỹ Thuật (ADR) | Thực Thể CSDL / API | Kịch Bản Nghiệm Thu (Gherkin/Test) |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `BUDGET-REQ-01` | `UC-BUDGET-01` | `Budget503020Card` | `ADR-BUDGET-01` | `budgets`, `budget_allocations` | `Scenario: Thiết lập ngân sách thành công` |
| `BUDGET-REQ-02` | `UC-BUDGET-01 (Ngoại Lệ)` | `Inline Alert Overrun` | `ADR-BUDGET-01-LOCK` | `SELECT ... FOR UPDATE` | `Scenario: Chặn lưu khi vượt thu nhập` |
