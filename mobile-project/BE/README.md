# 🛠️ HƯỚNG DẪN THIẾT LẬP BACKEND API (WAMP SERVER & JUNCTION LINK)

> **Dành cho:** Thành viên nhóm (Trịnh Thành Đạt, Trần Trọng Đại) và các **AI Coding Agent** hỗ trợ phát triển.  
> **Mục tiêu:** Giúp lập trình viên / Agent thiết lập môi trường chạy Backend PHP RESTful API trên **WampServer** cục bộ nhanh chóng trong **10 giây**, đồng bộ thời gian thực (0 giây) với mã nguồn Git mà **KHÔNG CẦN COPY/PASTE THỦ CÔNG**.

---

## 1. Cơ Chế Hoạt Động: NTFS Directory Junction (Cầu Nối Thư Mục 0 Giây)

### Tại sao KHÔNG nên copy thư mục `BE/` vào `www/` của WampServer?
- Nếu copy thủ công: Mỗi khi bạn sửa 1 dòng code trong VS Code / Git, bạn lại phải copy đè sang thư mục `www` của WampServer. Rất dễ quên, gây mất đồng bộ và làm sai lệch lịch sử Git.
- Nếu dùng Shortcut thường (`.lnk`): Apache của WampServer không đọc được file `.lnk`, sẽ báo lỗi `403 Forbidden` hoặc `404 Not Found`.

### Giải pháp chuẩn mực: NTFS Directory Junction (Liên kết cấp Kernel của Windows)
- **Directory Junction** là một tính năng của hệ điều hành Windows (định dạng ổ cứng NTFS).
- Khi tạo một Junction từ `D:\WampServer\www\expense_api` trỏ về thư mục `mobile-project/BE`:
  - Mã nguồn thật chỉ nằm **duy nhất 1 nơi** trong thư mục dự án Git.
  - Apache xem `expense_api` như một thư mục vật lý thực thụ nằm trong `www`.
  - **Sửa code ăn ngay (Real-time):** Bạn nhấn `Ctrl + S` lưu code trong IDE, Apache nạp file mới ngay lập tức.
  - **An toàn tuyệt đối:** Xóa thư mục link trong `www` không hề làm mất code gốc trong dự án.

---

## 2. Hướng Dẫn Tự Động Thiết Lập (Dành Cho Agent & Dev)

### Cách 1: Chạy Script PowerShell Tự Động (Khuyến nghị)
Trong thư mục `mobile-project/BE/` đã có sẵn file script tự động [`setup_wamp_junction.ps1`](setup_wamp_junction.ps1).

Chỉ cần mở PowerShell tại thư mục này và chạy:
```powershell
.\setup_wamp_junction.ps1
```
Script sẽ tự động:
1. Lấy đường dẫn hiện tại của thư mục `BE`.
2. Tự động dò tìm thư mục `www` của WampServer (kiểm tra các vị trí phổ biến: `D:\WampServer\www`, `C:\wamp64\www`, `C:\wamp\www`).
3. Khởi tạo Junction `expense_api` trỏ thẳng vào `BE`.

---

### Cách 2: Chạy Lệnh Thủ Công Bằng PowerShell

Mở PowerShell với quyền User bình thường (không cần Administrator) và chạy câu lệnh sau (chú ý thay đổi đường dẫn phù hợp với vị trí máy của bạn):

```powershell
# Cú pháp:
# New-Item -ItemType Junction -Path "<Đường_dẫn_Wamp_www>\expense_api" -Target "<Đường_dẫn_thư_mục_BE_của_bạn>"

# Ví dụ trên máy bạn Đạt (WAMP cài ổ D):
New-Item -ItemType Junction -Path "D:\WampServer\www\expense_api" -Target "D:\University Study Subjects\Information System Design Analysis\Project\Analyze System\mobile-project\BE"

# Ví dụ nếu bạn Đại cài WampServer mặc định ở ổ C:
New-Item -ItemType Junction -Path "C:\wamp64\www\expense_api" -Target "D:\Path\To\Your\Repo\mobile-project\BE"
```

---

### Cách 3: Chạy Lệnh Bằng Command Prompt (CMD)

Nếu bạn hoặc Agent sử dụng CMD (Command Prompt):
```cmd
mklink /J "D:\WampServer\www\expense_api" "D:\University Study Subjects\Information System Design Analysis\Project\Analyze System\mobile-project\BE"
```

---

## 3. Cách Kiểm Tra Liên Kết Đã Hoạt Động Chưa

### A. Kiểm tra bằng mắt thường (File Explorer):
1. Mở File Explorer, truy cập vào thư mục `www` của WampServer (ví dụ: `D:\WampServer\www`).
2. Bạn sẽ thấy một folder tên là `expense_api` có **biểu tượng mũi tên nhỏ** ở góc dưới bên trái icon thư mục (ký hiệu của Junction).
3. Bấm đúp vào trong sẽ thấy đầy đủ các thư mục `api/`, `config/`, `sql/`.

### B. Kiểm tra bằng dòng lệnh PowerShell:
```powershell
Get-Item "D:\WampServer\www\expense_api" | Select-Object Name, LinkType, Target
```
👉 Kết quả trả về `LinkType: Junction` và `Target` trỏ đúng vào thư mục `BE` là thành công 100%.

### C. Cách hủy bỏ liên kết (Nếu muốn đổi vị trí):
```powershell
Remove-Item "D:\WampServer\www\expense_api"
```
> [!IMPORTANT]
> Lệnh `Remove-Item` này **CHỈ XÓA ĐƯỜNG DẪN LIÊN KẾT ẢO**, toàn bộ mã nguồn PHP và CSDL trong `mobile-project/BE` **VẪN CÒN NGUYÊN VẸN 100%**.

---

## 4. Hướng Dẫn Khởi Tạo CSDL MySQL Trên WampServer

1. **Khởi động WampServer:** Mở `wampmanager.exe`, đợi icon khay hệ thống chuyển sang **màu xanh lá cây (Green)**.
2. **Truy cập phpMyAdmin:** Mở trình duyệt gõ `http://localhost/phpmyadmin` (User: `root`, Password để trống).
3. **Tạo CSDL mới:**
   - Tên CSDL: `sql_nhom46_itimi`
   - Bảng mã (Collation): `utf8mb4_unicode_ci`
4. **Nhập dữ liệu mẫu (Import Schema & Seed Data):**
   - Chọn database `sql_nhom46_itimi` ở cột bên trái.
   - Nhấn vào tab **Import** (hoặc mở tab **SQL**).
   - Mở file [`mobile-project/BE/sql/database_schema.sql`](sql/database_schema.sql), sao chép toàn bộ nội dung và dán vào tab SQL, rồi bấm nút **Go** (Thực hiện).
   - CSDL sẽ tự động tạo đủ 5 bảng: `users`, `wallets`, `categories`, `transactions`, `budgets` kèm dữ liệu mẫu ban đầu.

---

## 5. Kiểm Thử API Bằng Postman

Sau khi đã tạo Junction và bật WampServer, bạn mở Postman để test các endpoint cục bộ:

* **Base URL:** `http://localhost/expense_api/api`
* **Test Đăng nhập (Login):**
  - **Method:** `POST`
  - **URL:** `http://localhost/expense_api/api/auth/login.php`
  - **Headers:** `Content-Type: application/json`
  - **Body (raw JSON):**
    ```json
    {
      "email": "thanhdat@gmail.com",
      "password": "password123"
    }
    ```
* **Test Thống kê chi tiêu tháng (Monthly Summary):**
  - **Method:** `GET`
  - **URL:** `http://localhost/expense_api/api/reports/monthly_summary.php?user_id=1&month=2026-09`
* **Test Biểu đồ tròn cơ cấu chi tiêu (Category Chart):**
  - **Method:** `GET`
  - **URL:** `http://localhost/expense_api/api/reports/category_chart.php?user_id=1&month=2026-09`

---

## 6. Lưu Ý Dành Riêng Cho Bạn Đại Khi Viết Các API Tiếp Theo

Khi bạn Đại triển khai các API CRUD trong phân công:
- Các file API của `wallets/` (danh sách ví, tạo ví mới).
- Các file API của `transactions/` (thêm chi tiêu, danh sách lịch sử thu chi, xóa chi tiêu).
- Các file API của `categories/` (lấy danh mục chi tiêu/thu nhập).

👉 **Quy chuẩn bắt buộc trong mỗi file API:**
1. Đầu file luôn import 2 file cấu hình dùng chung:
   ```php
   require_once __DIR__ . '/../../config/database.php';
   require_once __DIR__ . '/../../config/helpers.php';
   ```
2. Gọi hàm chuẩn hóa Response để trả dữ liệu cho ứng dụng React Native:
   ```php
   // Thành công:
   send_json_response(200, "Lấy danh sách thành công", $data);

   // Thất bại:
   send_json_response(400, "Dữ liệu không hợp lệ");
   ```
3. Luôn dùng **PDO Prepared Statements** (`$stmt->prepare(...)` và `$stmt->execute(...)`) để đảm bảo không bị lỗi SQL Injection.
