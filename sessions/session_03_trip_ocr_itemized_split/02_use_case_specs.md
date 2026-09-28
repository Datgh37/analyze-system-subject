[COCKBURN_USE_CASE_SPEC]:
# ĐẶC TẢ YÊU CẦU PHẦN MỀM CHUẨN IEEE 29148 — Alistair Cockburn & Debra Paul

## 1. Thông Tin Ca Sử Dụng
- **Mã UC:** UC-HOUSEHOLD-01
- **Tên UC:** Bóc Tách Hóa Đơn Nhà Hàng OCR & Rút Gọn Nợ Nhóm Đồ Thị O(V log V)
- **Tác nhân chính:** Thành viên gia đình / Nhóm
- **Mục tiêu:** Quản lý dòng tiền và chia sẻ chi phí minh bạch.

## 2. Luồng Sự Kiện Chính (Happy Path)
1. Người dùng mở ứng dụng và chọn chức năng Quản Trị Quỹ.
2. Hệ thống hiển thị số dư quỹ và tỷ lệ đóng góp của từng thành viên.
3. Người dùng nhập số tiền đóng góp và xác nhận giao dịch.
4. Hệ thống cập nhật số dư tức thời và gửi thông báo tới các thành viên.

## 3. Luồng Ngoại Lệ (Exception Flows)
- **3a. Số dư tài khoản không đủ:** Hệ thống thông báo lỗi và gợi ý đóng góp một phần.
- **4a. Mất kết nối CSDL:** Hệ thống rollback giao dịch và lưu trạng thái PENDING_RETRY.

## 4. Tiêu Chí Nghiệm Thu BDD (Gherkin Scenarios)
```gherkin
Feature: Bóc Tách Hóa Đơn Nhà Hàng OCR & Rút Gọn Nợ Nhóm Đồ Thị O(V log V)
  Scenario: Đóng quỹ thành công
    Given Quỹ chung đang cần thu 10,000,000đ
    When Chồng nộp vào 6,000,000đ theo tỷ lệ 60%
    Then Số dư quỹ tăng thêm 6,000,000đ và trạng thái ghi nhận COMPLETED
```