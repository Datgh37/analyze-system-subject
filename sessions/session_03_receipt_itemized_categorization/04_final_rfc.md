# FINAL RFC: BÓC TÁCH HÓA ĐƠN MUA SẮM SIÊU THỊ ĐA MÓN & PHÂN LOẠI KAKEIBO

### 1. Tóm Tắt Quyết Định Hội Đồng
Hội đồng phê duyệt toàn diện pipeline OCR bất đồng bộ kết hợp Hậu kiểm toán học và phân bổ tự động vào các hũ chi tiêu cá nhân Kakeibo.

### 2. Ma Trận Truy Vết Nguồn Gốc (Traceability Matrix)
| Mã Yêu Cầu (REQ ID) | Ca Sử Dụng (Use Case) | Giao Diện (UI Spec) | Quyết Định Kỹ Thuật (ADR) | Thực Thể CSDL / API | Kịch Bản Nghiệm Thu (Gherkin/Test) |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `OCR-REQ-01` | `UC-OCR-01` | `ReceiptScanReviewModal` | `ADR-OCR-01` | `receipts`, `receipt_items` | `Scenario: Bóc tách hóa đơn thành công` |
| `OCR-REQ-02` | `UC-OCR-01 (Ngoại Lệ)` | `Inline Alert Math Discrepancy` | `ADR-OCR-01-LOCK` | `SELECT ... FOR UPDATE` | `Scenario: Đối soát sai lệch số học` |
