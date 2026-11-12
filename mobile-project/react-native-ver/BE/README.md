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
- Khi tạo một Junction từ `D:\WampServer\www\expense_api` trỏ về thư mục `mobile-project/react-native-ver/BE`:
  - Mã nguồn thật chỉ nằm **duy nhất 1 nơi** trong thư mục dự án Git.
  - Apache xem `expense_api` như một thư mục vật lý thực thụ nằm trong `www`.
  - **Sửa code ăn ngay (Real-time):** Bạn nhấn `Ctrl + S` lưu code trong IDE, Apache nạp file mới ngay lập tức.
  - **An toàn tuyệt đối:** Xóa thư mục link trong `www` không hề làm mất code gốc trong dự án.

---

## 2. Hướng Dẫn Tự Động Thiết Lập (Dành Cho Agent & Dev)

### Cách 1: Chạy Script PowerShell Tự Động (Khuyến nghị)
Trong thư mục `mobile-project/react-native-ver/BE/` đã có sẵn file script tự động [`setup_wamp_junction.ps1`](setup_wamp_junction.ps1).

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

# Ví dụ trên máy Windows có WAMP cài ổ D:
New-Item -ItemType Junction -Path "D:\WampServer\www\expense_api" -Target "D:\Projects\ExpenseTracker\mobile-project\react-native-ver\BE"

# Ví dụ nếu cài WampServer mặc định ở ổ C:
New-Item -ItemType Junction -Path "C:\wamp64\www\expense_api" -Target "C:\Projects\ExpenseTracker\mobile-project\react-native-ver\BE"
```

---

### Cách 3: Chạy Lệnh Bằng Command Prompt (CMD)

Nếu bạn hoặc Agent sử dụng CMD (Command Prompt):
```cmd
mklink /J "D:\WampServer\www\expense_api" "D:\Projects\ExpenseTracker\mobile-project\react-native-ver\BE"
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
> Lệnh `Remove-Item` này **CHỈ XÓA ĐƯỜNG DẪN LIÊN KẾT ẢO**, toàn bộ mã nguồn PHP và CSDL trong `mobile-project/react-native-ver/BE` **VẪN CÒN NGUYÊN VẸN 100%**.

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
   - Mở file [`mobile-project/react-native-ver/BE/sql/database_schema.sql`](sql/database_schema.sql), sao chép toàn bộ nội dung và dán vào tab SQL, rồi bấm nút **Go** (Thực hiện).
   - CSDL sẽ tự động tạo đủ 5 bảng: `users`, `wallets`, `categories`, `transactions`, `budgets` kèm dữ liệu mẫu ban đầu.

---

## 5. Danh Mục Toàn Bộ 21 RESTful API Hoàn Chỉnh (JWT Bearer Token)

| STT | METHOD | URI ENDPOINT | BẢO MẬT | MÔ TẢ NGHIỆP VỤ & TÍNH NĂNG |
| :---: | :---: | :--- | :---: | :--- |
| **1** | `POST` | `/api/auth/register.php` | Public | Đăng ký tài khoản mới & tự động cấp 2 ví cơ bản |
| **2** | `POST` | `/api/auth/login.php` | Public | Đăng nhập & cấp Bearer JWT Access Token |
| **3** | `POST` | `/api/auth/forgot_password.php`| Public | Yêu cầu gửi OTP 6 số (Kèm debug_otp trên Localhost) |
| **4** | `POST` | `/api/auth/verify_otp.php` | Public | Xác thực OTP & cấp reset_token 64 ký tự |
| **5** | `POST` | `/api/auth/reset_password.php` | Public | Đặt lại mật khẩu mới bằng reset_token |
| **6** | `GET` | `/api/auth/profile.php` | JWT Bearer | Lấy thông tin tài khoản & thống kê cá nhân |
| **7** | `POST` | `/api/auth/profile.php` | JWT Bearer | Đổi tên hiển thị / Đổi mật khẩu |
| **8** | `GET` | `/api/wallets/list.php` | JWT Bearer | Danh sách ví & Tổng tài sản ròng khả dụng |
| **9** | `POST` | `/api/wallets/create.php` | JWT Bearer | Tạo ví mới (Tiền mặt, Ngân hàng) |
| **10**| `POST` | `/api/wallets/transfer.php` | JWT Bearer | Chuyển tiền nội bộ giữa 2 ví ACID (QOL #1) |
| **11**| `GET` | `/api/categories/list.php` | JWT Bearer | Danh mục Thu/Chi (Icons & Mã màu Hex) |
| **12**| `POST` | `/api/categories/create.php` | JWT Bearer | Tạo danh mục riêng của người dùng |
| **13**| `GET` | `/api/transactions/list.php` | JWT Bearer | Lịch sử giao dịch (Lọc đa tiêu chí UX #3) |
| **14**| `POST` | `/api/transactions/create.php` | JWT Bearer | Thêm giao dịch thu/chi (ACID & Cập nhật ví) |
| **15**| `POST` | `/api/transactions/update.php` | JWT Bearer | Sửa giao dịch, tự bù trừ Delta số dư ví (QOL #2)|
| **16**| `POST` | `/api/transactions/delete.php` | JWT Bearer | Xóa giao dịch, hoàn lại tiền vào ví |
| **17**| `GET` | `/api/budgets/status.php` | JWT Bearer | Tiến độ ngân sách tháng & cảnh báo (UX #4) |
| **18**| `POST` | `/api/budgets/set.php` | JWT Bearer | Thiết lập hạn mức chi tiêu tháng |
| **19**| `GET` | `/api/reports/monthly_summary.php` | JWT Bearer| Báo cáo tổng quan thu - chi tháng |
| **20**| `GET` | `/api/reports/category_chart.php` | JWT Bearer | Biểu đồ tròn tỷ trọng chi tiêu (Pie Chart) |
| **21**| `GET` | `/api/reports/income_vs_expense_chart.php` | JWT Bearer | Biểu đồ cột đối chiếu 6 tháng (Bar Chart) |

---

## 6. Quy Chuẩn Bảo Mật & Lệnh Kiểm Thử Tự Động

### A. Cơ chế Bảo Mật JWT (Triệt tiêu IDOR)
- Mọi API có bảo mật `JWT Bearer` **tuyệt đối không nhận `$user_id` từ client** gửi lên.
- Backend tự động bóc tách `$user_id` trực tiếp từ chuỗi chữ ký số Bearer Token trong Header:
  `Authorization: Bearer <jwt_token>`
- Nếu thiếu hoặc token không hợp lệ, hệ thống tự động trả về `HTTP 401 Unauthorized`.

### B. Kiểm thử tự động bằng PowerShell
Chạy file kiểm thử tự động toàn diện:
```powershell
.\test_api.ps1
```
Script sẽ tự động chạy qua Bước 0.5 dọn dẹp sạch CSDL và kiểm tra toàn bộ 21 endpoint (từ chối 401, cấp JWT token, luồng OTP 3 bước, bù trừ Delta và tạo báo cáo biểu đồ).  
*Nếu muốn chạy test mà giữ nguyên dữ liệu hiện tại không reset: gõ `.\test_api.ps1 -NoReset`.*

---

## 7. 🧹 API Nội Bộ Debug & Dọn Dẹp CSDL (Internal Reset API)

> ⚠️ **Lưu ý:** API này chỉ dùng cho mục đích kiểm thử phát triển nội bộ, **không xuất hiện trên giao diện ứng dụng di động**.

* **Endpoint:** `GET` hoặc `POST` `/api/debug/reset_database.php`
* **URL:** `http://localhost/expense_api/api/debug/reset_database.php`
* **Cơ chế hoạt động:**
  1. **Xóa thật (Hard Delete / TRUNCATE):** Làm sạch toàn bộ dữ liệu phát sinh khi chạy test (giao dịch mới, ví mới, OTP).
  2. **Đưa ID tự tăng về 1:** Reset Auto-Increment của tất cả các bảng.
  3. **Tái tạo nguyên trạng bộ dữ liệu mẫu chuẩn:**
     - 2 Tài khoản mẫu (`thanhdat@gmail.com` và `trongdai@gmail.com` / pass: `password123`).
     - 4 Ví gốc (Tiền mặt, Vietcombank, MB Bank) với số dư nguyên vẹn.
     - 10 Danh mục hệ thống mặc định (7 Chi tiêu, 3 Thu nhập).
     - 7 Giao dịch mẫu theo đúng tháng hiện tại.
     - 2 Ngân sách mẫu theo đúng tháng hiện tại.
     - Dọn sạch bảng `password_resets`.
* **Khóa an toàn:** Tự động chặn và trả về `HTTP 403 Forbidden` nếu phát hiện đang chạy trên máy chủ Production của trường.


