# ĐẶC TẢ YÊU CẦU PHẦN MỀM CHUẨN IEEE 29148 — UC-VELOCITY-01

## 1. Thông Tin Ca Sử Dụng
- **Mã UC:** `UC-VELOCITY-01`
- **Tên UC:** Dự Báo Tốc Độ Tiêu Tiền & Cảnh Báo Sớm Nguy Cơ Cháy Túi Cá Nhân
- **Tác nhân chính:** Người dùng cá nhân
- **Mục tiêu:** Cảnh báo ngày cạn kiệt ngân sách và phát hiện các khoản chi tiêu định kỳ lãng phí.

## 2. Luồng Sự Kiện Chính (Happy Path)
1. Hệ thống tính toán Tốc độ tiêu tiền trung bình mỗi ngày (Daily Burn Rate): `spent_so_far / current_day`.
2. Hệ thống dự báo tổng chi tiêu cả tháng và tính Ngày cạn kiệt ngân sách (Depletion Day).
3. Nếu Ngày cạn kiệt < Tổng số ngày trong tháng, hệ thống kích hoạt Cú hích Nudge cảnh báo sớm.
4. Hệ thống quét lịch sử giao dịch phát hiện các khoản trừ tiền định kỳ (Subscription Traps) và gửi khuyến nghị cắt giảm.
5. Người dùng xác nhận điều chỉnh hạn mức chi tiêu hàng ngày hoặc hủy dịch vụ định kỳ.

## 3. Luồng Ngoại Lệ (Exception Flows)
- **3a. Người dùng chưa phát sinh giao dịch nào trong tháng:** Hệ thống hiển thị trạng thái an toàn tuyệt đối và hạn mức ngày tối đa.
- **4a. Khoản chi tiêu định kỳ có số tiền biến động:** Hệ thống gắn cờ nghi vấn và hỏi người dùng có phải gói dịch vụ thay đổi giá hay không.

## 4. Tiêu Chí Nghiệm Thu BDD (Gherkin Scenarios)
```gherkin
Feature: Dự báo tốc độ tiêu tiền và cảnh báo sớm
  Scenario: Kích hoạt cảnh báo khi tốc độ chi tiêu vượt ngưỡng
    Given Hạn mức ngân sách tháng là 5,000,000đ cho 30 ngày
    When Đến ngày 12 người dùng đã tiêu 3,200,000đ (tốc độ 266,667đ/ngày)
    Then Hệ thống phát cảnh báo: Dự kiến hết tiền vào ngày 18 và vượt hạn mức 3,000,000đ cuối tháng
```
