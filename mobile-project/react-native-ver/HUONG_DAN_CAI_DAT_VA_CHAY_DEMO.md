# HƯỚNG DẪN CÀI ĐẶT VÀ VẬN HÀNH DEMO ĐỒ ÁN MOBILE

## HỌC PHẦN: LẬP TRÌNH THIẾT BỊ DI ĐỘNG (DNTU)
**Đề tài:** Hệ thống Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh  
**Nhóm thực hiện:** Nhóm 46  
- **Trịnh Thành Đạt** — MSSV: 1923050149 (Nhóm trưởng)  
- **Trần Trọng Đại** — MSSV: 1923050745  

---

## I. YÊU CẦU MÔI TRƯỜNG MÁY TÍNH
1. **WAMP Server (hoặc XAMPP):** Chạy Apache (Port 80) và MySQL (Port 3306 / 3308).
2. **Node.js:** Phiên bản 18.x trở lên (khuyến nghị v20 LTS).
3. **Thiết bị chạy App:**
   - **Cách 1 (Nhanh nhất):** Chạy trên Trình duyệt Web máy tính (Google Chrome, Microsoft Edge).
   - **Cách 2:** Chạy trên điện thoại di động (Cài ứng dụng **Expo Go** từ CH Play hoặc App Store, kết nối cùng mạng WiFi với máy tính).

---

## II. QUY TRÌNH CÀI ĐẶT & CHẠY DEMO (3 BƯỚC NHANH GỌN)

### BƯỚC 1: KHỞI TẠO CƠ SỞ DỮ LIỆU MYSQL TRÊN WAMP
1. Bật **WampServer** (Đảm bảo icon góc phải thanh taskbar chuyển màu **Xanh lá cây**).
2. Mở trình duyệt truy cập phpMyAdmin: `http://localhost/phpmyadmin`
3. Tạo mới một Database có tên: `sql_nhom46_itimi` (Collation: `utf8mb4_unicode_ci`).
4. Chọn Database vừa tạo -> Chọn tab **Import** (Nhập).
5. Bấm **Choose File** và chọn tệp:
   `BE/sql/database_schema.sql` (hoặc trong thư mục `02_CoSoDuLieu/database_schema.sql`).
6. Kéo xuống dưới cùng bấm nút **Import / Go**.
> **Lưu ý:** Script SQL đã tích hợp sẵn toàn bộ cấu trúc bảng và **92 giao dịch mẫu stress-test** trải dài 6 tháng (Tháng 05/2026 đến Tháng 10/2026) cùng 4 ví tiền, 13 danh mục thu/chi và hạn mức ngân sách tháng.

---

### BƯỚC 2: LIÊN KẾT BACKEND PHP VỚI WAMP WWW
Nhóm đã tạo sẵn script tự động liên kết (Directory Junction) không cần copy thủ công:
1. Nhấp đúp chuột chạy file: **`2_KET_NOI_WAMP.bat`**
2. Script sẽ tự động nhận diện thư mục `C:\wamp64\www` và tạo liên kết thư mục `expense_api`.
3. **Kiểm tra kết nối Backend:**
   Mở trình duyệt truy cập đường link sau:  
   `http://localhost/expense_api/api/reports/monthly_summary.php?user_id=1&month=2026-09`  
   Nếu thấy hiển thị chuỗi JSON chứa dữ liệu chi tiêu tháng 9 là Backend đã sẵn sàng 100%!

*(Nếu không dùng Wamp mặc định, Thầy/Cô chỉ cần copy nguyên thư mục `BE` dán vào thư mục `www` hoặc `htdocs` và đổi tên thành `expense_api`)*.

---

### BƯỚC 3: CÀI ĐẶT THƯ VIỆN VÀ KHỞI ĐỘNG HỆ THỐNG
1. **Cài đặt thư viện (Chỉ cần chạy lần đầu tiên):**
   - Nhấp đúp chạy file: **`1_CAI_DAT_THU_VIEN.bat`**  
   - Chờ hệ thống tự động tải gói `node_modules` cho cả Realtime Gateway và Frontend Mobile.

2. **Khởi động Realtime Gateway (Socket.IO):**
   - Nhấp đúp chạy file: **`3_CHAY_REALTIME_SERVER.bat`**  
   - Cửa sổ terminal hiện thông báo: `[Expense Realtime] Server is running on port 3001` (Giữ cửa sổ này mở).

3. **Khởi động Ứng dụng Mobile App (Expo):**
   - Nhấp đúp chạy file: **`4_CHAY_APP_MOBILE.bat`**  
   - Cửa sổ Metro Bundler xuất hiện:
     - **Để xem trên Web máy tính:** Bấm phím **`w`** trên bàn phím. Trình duyệt sẽ tự động mở giao diện ứng dụng.
     - **Để xem trên Điện thoại:** Mở app **Expo Go** trên điện thoại và quét mã QR Code hiển thị trên cửa sổ terminal.

---

## III. TÀI KHOẢN ĐĂNG NHẬP TRẢI NGHIỆM
Hệ thống đã nạp sẵn tài khoản người dùng thực tế với dữ liệu thu chi đầy đủ:
- **Email:** `nguyenvanan@gmail.com`
- **Mật khẩu:** `password123`

---

## IV. KỊCH BẢN CHẤM ĐIỂM & ĐÁNH GIÁ CHỨC NĂNG (DEMO SCRIPT)

| STT | Phân hệ / Tính năng | Thao tác kiểm thử | Kết quả mong đợi |
| :---: | :--- | :--- | :--- |
| **1** | **Xác thực người dùng** | Đăng nhập bằng `nguyenvanan@gmail.com` / `password123`. | Đăng nhập thành công, lưu Token, chuyển hướng mượt mà vào màn hình chính. |
| **2** | **Trang chủ & Tổng quan** | Xem thẻ số dư tổng hợp, danh sách 4 ví (Tiền mặt, Vietcombank, Techcombank, MoMo) và 5 giao dịch gần nhất. | Số liệu tổng tài sản khớp với cơ sở dữ liệu (`68.150.000 đ`). |
| **3** | **Quản lý Ngân sách** | Chuyển sang Tab **Ngân sách (Budget)**. | Hiển thị thanh tiến độ cảnh báo màu sắc theo tỷ lệ chi tiêu (Xanh $\rightarrow$ Vàng $\rightarrow$ Đỏ) và số tiền an toàn có thể chi mỗi ngày (DSA). |
| **4** | **Báo cáo trực quan** | Chuyển sang Tab **Báo cáo (Report)**. Bấm chuyển đổi giữa `[Chi tiêu]` và `[Thu nhập]`. | Biểu đồ cột hiển thị đối chiếu 6 tháng (T5 - T10/2026), biểu đồ tròn phân tích cơ cấu danh mục có phần trăm rõ ràng. Bảng tóm tắt dòng tiền cuối trang. |
| **5** | **Lịch sử giao dịch** | Chuyển sang Tab **Giao dịch (Transactions)**. Chọn xem theo từng tháng. | Danh sách 92 giao dịch hiển thị phân trang, lọc theo danh mục chuẩn xác. |
| **6** | **Thời gian thực (Realtime)** | Bấm nút **(+) Thêm giao dịch** -> Nhập chi tiêu (VD: 50.000 đ cho Ăn uống) -> Lưu. | Socket.IO bắn tín hiệu tức thời: Số dư ví trừ ngay lập tức, thanh ngân sách cập nhật và biểu đồ biến động theo mà **không cần bấm F5 / Reload**. |

---

## V. CẤU TRÚC GÓI NỘP BÀI
```text
Nhom46_MobileApp_QuanLyChiTieu/
├── 01_BaoCao/
│   ├── BAO_CAO_GIUA_KY.docx (Bản Word nộp giảng viên)
│   ├── BAO_CAO_GIUA_KY.pdf (Bản PDF xuất in)
│   ├── BAO_CAO_GIUA_KY.md (Bản Markdown chi tiết kiến trúc)
│   └── database_erd.drawio (Sơ đồ ERD thực thể quan hệ)
├── 02_CoSoDuLieu/
│   └── database_schema.sql (Script tạo CSDL + 92 giao dịch mẫu)
├── 03_MaNguon/
│   ├── BE/ (PHP RESTful API WAMP Server)
│   ├── realtime-server/ (Socket.IO Gateway Node.js)
│   └── FE/ (React Native Expo App)
├── 1_CAI_DAT_THU_VIEN.bat
├── 2_KET_NOI_WAMP.bat
├── 3_CHAY_REALTIME_SERVER.bat
├── 4_CHAY_APP_MOBILE.bat
└── HUONG_DAN_CAI_DAT_VA_CHAY_DEMO.txt
```
