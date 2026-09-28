# ĐẶC TẢ YÊU CẦU PHẦN MỀM CHUẨN IEEE 29148 — UC-OCR-01

## 1. Thông Tin Ca Sử Dụng
- **Mã UC:** `UC-OCR-01`
- **Tên UC:** Bóc Tách Hóa Đơn Mua Sắm Đa Món Bằng AI & Tự Động Phân Bổ Hũ Kakeibo
- **Tác nhân chính:** Người dùng cá nhân
- **Mục tiêu:** Tự động hóa trích xuất danh sách mặt hàng và gán nhãn danh mục chi tiêu.

## 2. Luồng Sự Kiện Chính (Happy Path)
1. Người dùng chụp hoặc tải lên ảnh chụp hóa đơn siêu thị (Co.opmart, WinMart, Lotte).
2. Hệ thống đưa ảnh vào Message Queue để xử lý Vision LLM bất đồng bộ (Async).
3. AI bóc tách danh sách món hàng: Tên món, Số lượng, Đơn giá, Tổng tiền.
4. Hệ thống chạy thuật toán Hậu Kiểm Toán Học (Math Audit): Tổng tiền các món + Thuế VAT phải bằng Tổng tiền thanh toán trên hóa đơn.
5. AI tự động gán nhãn từng món vào 4 nhóm Kakeibo (Needs, Wants, Culture, Unexpected).
6. Người dùng xem lại, chỉnh sửa nếu cần và xác nhận lưu giao dịch vào ví tương ứng.

## 3. Luồng Ngoại Lệ (Exception Flows)
- **3a. Ảnh chụp bị mờ hoặc rách không đọc được:** Hệ thống thông báo chất lượng ảnh kém và yêu cầu chụp lại.
- **4a. Sai lệch toán học giữa tổng món và tổng hóa đơn (> 1,000đ):** Hệ thống đánh dấu dòng nghi vấn và yêu cầu người dùng xác nhận lại trước khi lưu.

## 4. Tiêu Chí Nghiệm Thu BDD (Gherkin Scenarios)
```gherkin
Feature: Bóc tách hóa đơn mua sắm đa món bằng AI OCR
  Scenario: Bóc tách thành công và khớp số học
    Given Người dùng tải lên hóa đơn siêu thị hợp lệ
    When Hệ thống bóc tách được 10 mặt hàng và tổng tiền khớp với hóa đơn
    Then Giao dịch được lưu và số dư ví chi tiêu được cập nhật chính xác
```
