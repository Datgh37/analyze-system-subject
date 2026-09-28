# ĐẶC TẢ YÊU CẦU PHẦN MỀM CHUẨN IEEE 29148 — UC-BUDGET-01

## 1. Thông Tin Ca Sử Dụng
- **Mã UC:** `UC-BUDGET-01`
- **Tên UC:** Thiết Lập Kế Hoạch Ngân Sách 50/30/20 & Phân Bổ Danh Mục Chi Tiêu
- **Tác nhân chính:** Người dùng cá nhân (Individual User)
- **Mục tiêu:** Thiết lập hạn mức chi tiêu tháng theo chuẩn 50/30/20 và theo dõi tiến độ chi tiêu theo ngày.

## 2. Luồng Sự Kiện Chính (Happy Path)
1. Người dùng mở ứng dụng và chọn "Thiết lập Ngân sách Tháng Mới".
2. Người dùng nhập mức thu nhập dự kiến trong tháng (ví dụ: 15,000,000 VND).
3. Hệ thống tự động đề xuất phân bổ: Needs 7.5tr (50%), Wants 4.5tr (30%), Savings 3.0tr (20%).
4. Người dùng điều chỉnh phân bổ chi tiết cho từng danh mục con (Tiền nhà, Ăn uống, Cà phê, Quỹ khẩn cấp).
5. Hệ thống tính toán Hạn mức chi tiêu mỗi ngày và kích hoạt bộ đếm thời gian thực.

## 3. Luồng Ngoại Lệ (Exception Flows)
- **3a. Tổng phân bổ vượt quá thu nhập:** Hệ thống hiển thị cảnh báo đỏ và chặn lưu cho đến khi cân bằng theo Zero-Based Budgeting.
- **4a. Người dùng nhập số tiền âm:** Hệ thống báo lỗi dữ liệu không hợp lệ tại ô nhập liệu.

## 4. Tiêu Chí Nghiệm Thu BDD (Gherkin Scenarios)
```gherkin
Feature: Thiết lập ngân sách cá nhân 50/30/20
  Scenario: Thiết lập ngân sách thành công khi cân bằng
    Given Người dùng có thu nhập 15,000,000đ
    When Phân bổ 7,500,000đ cho Needs, 4,500,000đ cho Wants và 3,000,000đ cho Savings
    Then Hệ thống lưu ngân sách thành công với trạng thái HEALTHY
```
