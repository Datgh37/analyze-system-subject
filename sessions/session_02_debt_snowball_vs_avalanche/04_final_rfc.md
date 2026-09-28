# FINAL RFC: TỐI ƯU HÓA CHIẾN LƯỢC TRẢ NỢ CÁ NHÂN (SNOWBALL VS AVALANCHE)

### 1. Tóm Tắt Quyết Định Hội Đồng
Hội đồng phê duyệt toàn diện giải thuật mô phỏng trả nợ song song Snowball và Avalanche, kết hợp giao diện tương tác trực quan.

### 2. Ma Trận Truy Vết Nguồn Gốc (Traceability Matrix)
| Mã Yêu Cầu (REQ ID) | Ca Sử Dụng (Use Case) | Giao Diện (UI Spec) | Quyết Định Kỹ Thuật (ADR) | Thực Thể CSDL / API | Kịch Bản Nghiệm Thu (Gherkin/Test) |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `DEBT-REQ-01` | `UC-DEBT-01` | `DebtPayoffComparisonMatrix` | `ADR-DEBT-01` | `personal_debts` | `Scenario: So sánh chiến lược trả nợ` |
| `DEBT-REQ-02` | `UC-DEBT-01 (Ngoại Lệ)` | `Inline Alert Invalid APR` | `ADR-DEBT-01-LOCK` | `SELECT ... FOR UPDATE` | `Scenario: Kiểm tra dữ liệu lãi suất hợp lệ` |
