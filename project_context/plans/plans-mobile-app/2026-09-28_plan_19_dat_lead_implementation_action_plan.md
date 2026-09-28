# 🎯 KẾ HOẠCH HÀNH ĐỘNG CHI TIẾT DÀNH CHO TRỊNH THÀNH ĐẠT (LEAD) - PLAN-19
## PHÂN HỆ: FULL-STACK ARCHITECTURE & CORE SYSTEM IMPLEMENTATION
> **Ngày lập:** 2026-09-28  
> **Người thực hiện:** Trịnh Thành Đạt (MSSV: 1923050149 - Trưởng nhóm)  
> **Tài liệu tham chiếu:** [`mobile-project/ROADMAP.md`](../../../mobile-project/ROADMAP.md) | [`mobile-project/README.md`](../../../mobile-project/README.md)  
> **Tài liệu báo cáo:** [`mobile-project/BaoCaoLTMB.docx`](../../../mobile-project/BaoCaoLTMB.docx)

---

## 1. Mục Tiêu & Nguyên Tắc Triển Khai Của Trưởng Nhóm

1. **Xây dựng Nền tảng Vững chắc (Foundation First):** Đạt đảm nhiệm phần "xương sống" của hệ thống (CSDL, Kiến trúc kết nối PDO, Cơ chế xác thực, API Báo cáo phức tạp, Khung điều hướng Mobile).
2. **Cung cấp Khuôn mẫu (Template & Boilerplate) cho Bạn Đại:** Khi Đạt hoàn thiện module CSDL và 2 API Auth mẫu (`register.php`, `login.php`), bạn Trần Trọng Đại sẽ có ngay "chuẩn code" để tự tin viết tiếp các API CRUD Ví và Giao dịch mà không lo lệch chuẩn.
3. **Độc lập kiểm thử trên WAMP Server Local:** Hoàn thiện 100% việc chạy thử trên localhost trước khi nghĩ đến việc deploy lên server trường.

---

## 2. Danh Sách 4 Chặng Hành Động (Action Checklist)

### 📌 CHẶNG 1: Thiết Kế & Đóng Gói CSDL MySQL (Sprint 1)
- [x] **Task 1.1:** Khởi tạo thư mục `mobile-project/BE/sql/`.
- [x] **Task 1.2:** Viết tệp DDL hoàn chỉnh [`mobile-project/BE/sql/database_schema.sql`](../../../mobile-project/BE/sql/database_schema.sql) gồm 5 bảng quan hệ chuẩn:
  - `users` (Định danh, email unique, password_hash)
  - `wallets` (Tiền mặt, Ngân hàng, balance DECIMAL 15,2)
  - `categories` (Icon, màu sắc, phân loại Thu / Chi)
  - `transactions` (Khóa ngoại wallet, category, user, CASCADE/RESTRICT chặt chẽ)
  - `budgets` (Hạn mức chi tiêu theo tháng)
- [x] **Task 1.3:** Tích hợp dữ liệu mẫu (Seed Data) chuẩn vào file SQL để khi import vào WAMP/phpMyAdmin là có ngay dữ liệu test trực quan.
- [ ] **Task 1.4:** Thiết kế sơ đồ ERD trực quan (Mermaid & hình ảnh) sẵn sàng đưa vào **Trang 7** của báo cáo Word.

---

### 📌 CHẶNG 2: Xây Dựng Backend PHP Core & API Báo Cáo (Sprint 2)
- [x] **Task 2.1:** Xây dựng module kết nối CSDL đa môi trường [`mobile-project/BE/config/database.php`](../../../mobile-project/BE/config/database.php) (cờ `$is_local`).
- [x] **Task 2.2:** Xây dựng module tiện ích dùng chung [`mobile-project/BE/config/helpers.php`](../../../mobile-project/BE/config/helpers.php) (CORS headers, chuẩn hóa JSON response `send_json_response($status, $message, $data, $code)`).
- [x] **Task 2.3:** Hiện thực API Xác thực (Auth Endpoints):
  - `BE/api/auth/register.php`: Validate đầu vào, mã hóa `password_hash()`, kiểm tra trùng lặp email.
  - `BE/api/auth/login.php`: Kiểm tra `password_verify()`, trả về thông tin user và token định danh.
- [x] **Task 2.4:** Hiện thực API Báo cáo & Thống kê (Reports Endpoints):
  - `BE/api/reports/monthly_summary.php`: Tính toán `total_income`, `total_expense`, `net_balance`.
  - `BE/api/reports/category_chart.php`: Gom nhóm chi tiêu theo danh mục để vẽ biểu đồ tròn (Pie Chart).
- [ ] **Task 2.5:** Viết API `BE/api/reports/income_vs_expense_chart.php`: Thống kê thu - chi theo tháng để vẽ biểu đồ cột (Bar Chart).

---

### 📌 CHẶNG 3: Khởi Tạo Ứng Dụng React Native & Khung Điều Hướng (Sprint 3 & 4)
- [ ] **Task 3.1:** Khởi tạo cấu trúc thư mục client `mobile-project/FE/src/`.
- [ ] **Task 3.2:** Viết cấu hình kết nối API đa môi trường `FE/src/api/config.js` (hỗ trợ `10.0.2.2`, IP Wifi LAN, và Domain server).
- [ ] **Task 3.3:** Xây dựng kiến trúc điều hướng React Navigation (BottomTabNavigator 4 tab + Auth Stack).
- [ ] **Task 3.4:** Hiện thực hóa màn hình chính **HomeScreen** (Hiển thị thẻ ví, thanh tiến độ ngân sách, giao dịch gần đây).
- [ ] **Task 3.5:** Hiện thực hóa màn hình **ReportScreen** (Tích hợp thư viện `react-native-chart-kit` vẽ Pie Chart & Bar Chart).

---

### 📌 CHẶNG 4: Điền Nội Dung Kỹ Thuật Vào Báo Cáo Word `BaoCaoLTMB.docx` (Sprint 5)
- [ ] **Task 4.1 (Trang 7):** Điền phần *Xây dựng mô hình DB* (Chèn sơ đồ ERD, bảng đặc tả chi tiết 5 bảng, ảnh chụp phpMyAdmin).
- [ ] **Task 4.2 (Trang 9–10):** Điền phần *Xây dựng Front-End (React Native)* (Chèn ảnh chụp màn hình HomeScreen, ReportScreen kèm mô tả luồng logic).
- [ ] **Task 4.3:** Nhấn **`F9`** trong Word để làm mới tự động Mục lục TOC, Danh mục hình ảnh TOF và Danh mục bảng biểu TOT.
