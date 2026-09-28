# ĐẶC TẢ YÊU CẦU PHẦN MỀM CHUẨN IEEE 29148 — UC-DEBT-01

## 1. Thông Tin Ca Sử Dụng
- **Mã UC:** `UC-DEBT-01`
- **Tên UC:** Lập Kế Hoạch & So Sánh Chiến Lược Trả Nợ Cá Nhân (Snowball vs Avalanche)
- **Tác nhân chính:** Người dùng cá nhân có khoản nợ
- **Mục tiêu:** Tính toán lộ trình sạch nợ và số tiền lãi tiết kiệm được.

## 2. Luồng Sự Kiện Chính (Happy Path)
1. Người dùng nhập danh sách các khoản nợ: Dư nợ gốc, Lãi suất (%/năm), Số tiền trả tối thiểu (Min Payment).
2. Người dùng nhập số tiền thặng dư có thể chi trả thêm hàng tháng (Extra Budget).
3. Hệ thống chạy song song 2 giải thuật: Snowball và Avalanche.
4. Hệ thống hiển thị bảng so sánh trực quan: Số tháng về đích và tổng tiền lãi chi trả.

## 3. Luồng Ngoại Lệ (Exception Flows)
- **3a. Thu nhập thặng dư bằng 0:** Hệ thống cảnh báo chỉ có thể trả số tiền tối thiểu và hiển thị thời gian trả nợ kéo dài kèm rủi ro lãi kép.
- **4a. Lãi suất nhập vào không hợp lệ (< 0% hoặc > 100%):** Hệ thống hiển thị thông báo lỗi và yêu cầu nhập lại.

## 4. Tiêu Chí Nghiệm Thu BDD (Gherkin Scenarios)
```gherkin
Feature: Tối ưu hóa chiến lược trả nợ cá nhân
  Scenario: So sánh chiến lược Snowball và Avalanche
    Given Người dùng có 3 khoản nợ và ngân sách trả thêm 1,000,000đ/tháng
    When Người dùng bấm So Sánh Chiến Lược
    Then Hệ thống hiển thị Avalanche tiết kiệm nhiều tiền lãi hơn và Snowball có thứ tự xóa nợ nhanh hơn
```
