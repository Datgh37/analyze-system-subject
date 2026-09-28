# FINAL RFC: DỰ BÁO TỐC ĐỘ TIÊU TIỀN, CẢNH BÁO CHÁY TÚI & BẪY ĐĂNG KÝ (SUBSCRIPTION TRAP)

### 1. Tóm Tắt Quyết Định Hội Đồng
Hội đồng phê duyệt toàn diện động cơ tính toán tốc độ chi tiêu thời gian thực và module quét bẫy chi tiêu định kỳ.

### 2. Ma Trận Truy Vết Nguồn Gốc (Traceability Matrix)
| Mã Yêu Cầu (REQ ID) | Ca Sử Dụng (Use Case) | Giao Diện (UI Spec) | Quyết Định Kỹ Thuật (ADR) | Thực Thể CSDL / API | Kịch Bản Nghiệm Thu (Gherkin/Test) |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `VELOCITY-REQ-01` | `UC-VELOCITY-01` | `SpendingVelocityCard` | `ADR-VELOCITY-01` | `recurring_bills`, `transactions` | `Scenario: Cảnh báo tốc độ chi tiêu vượt ngưỡng` |
| `VELOCITY-REQ-02` | `UC-VELOCITY-01 (Ngoại Lệ)` | `Inline Alert Subscription Price Change` | `ADR-VELOCITY-01-LOCK` | `SELECT ... FOR UPDATE` | `Scenario: Phát hiện bẫy đăng ký` |
