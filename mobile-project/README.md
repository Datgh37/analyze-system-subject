# 📱 ỨNG DỤNG QUẢN LÝ CHI TIÊU CÁ NHÂN (PERSONAL EXPENSE TRACKER)
> **Đồ án môn học:** Lập trình Thiết bị Di động (Mobile Application Development)  
> **Mối quan hệ đề tài:** Phân hệ ứng dụng di động tinh gọn (Mobile Client MVP) được trích xuất từ đề tài lớn môn **Phân tích Thiết kế Hệ thống Thông tin (PTTKHTTT)**.  
> 🗺️ **Kế hoạch & Lộ trình phân công chi tiết:** Xem tại [ROADMAP.md](ROADMAP.md) | 📄 **Tài liệu Báo cáo Word:** [BaoCaoLTMB.docx](BaoCaoLTMB.docx)

---


## 1. Tổng Quan & Mục Tiêu Dự Án

Dự án này tập trung xây dựng một **ứng dụng di động quản lý chi tiêu cá nhân** hoàn chỉnh, hoạt động mượt mà trên nền tảng di động với mục tiêu cốt lõi:
- **Ghi nhận nhanh chóng:** Thêm mới khoản thu/chi với thao tác tối giản (chọn ví, danh mục, số tiền, ngày).
- **Phân loại khoa học:** Quản lý danh mục thu/chi phân cấp trực quan với biểu tượng màu sắc.
- **Thống kê hàng tháng:** Tổng hợp tổng thu, tổng chi và số dư biến động theo từng tháng.
- **Trực quan hóa dữ liệu:** Hiển thị biểu đồ tròn (tỷ trọng chi tiêu theo hạng mục) và biểu đồ cột (so sánh thu - chi).

---

## 2. Phân Định Phạm Vi: Đồ Án Mobile vs. Hệ Thống PTTKHTTT

Để đảm bảo khả năng hoàn thành đúng hạn (10–15 tuần) của môn Lập trình Mobile mà vẫn giữ được tính nhất quán với môn PTTKHTTT, phạm vi của dự án Mobile được thu gọn thành một bản **MVP (Minimum Viable Product)** thực chiến:

| Phân hệ / Chức năng | Hệ Thống PTTKHTTT (Đề tài lớn / Khóa luận) | Ứng Dụng Mobile Này (Môn Lập trình Mobile) | Ghi chú điều chỉnh |
| :--- | :--- | :--- | :--- |
| **Quản lý Tài khoản & Ví** | Đa ví chuẩn ACID, Thẻ tín dụng (hạn mức, ngày chốt sao kê), Ví điện tử, Sổ chuyển tiền nội bộ. | **2 Ví cơ bản:** Tiền mặt & Tài khoản Ngân hàng. Cập nhật số dư trực tiếp sau mỗi giao dịch. | Cắt giảm cơ chế khóa giao dịch phức tạp; tập trung vào tính trực quan trên màn hình ví. |
| **Ghi nhận Thu / Chi** | Nhập tay, Quét AI Multimodal Vision OCR bóc tách đa món, Hậu kiểm toán số học, Quét mã VietQR. | **Nhập liệu thủ công nhanh (Fast Input):** Bàn phím số chuyên dụng, chọn ngày, danh mục, ghi chú. | Đảm bảo trải nghiệm form nhập liệu cực nhanh, validate dữ liệu chặt chẽ trên mobile. |
| **Danh mục (Categories)** | Cây danh mục đa cấp, tự động ánh xạ ma trận Quy tắc 50/30/20 và Sổ Kakeibo 4 cột trụ. | **Danh mục 2 nhóm (Thu & Chi):** Icon đại diện, tên danh mục, mã màu phân biệt. | Giữ giao diện chọn danh mục dạng lưới (Grid View) thân thiện. |
| **Ngân sách (Budgeting)** | Ngân sách thích ứng, Hạn mức ngày động (DSA), Cảnh báo tốc độ tiêu tiền (Spending Velocity). | **Thiết lập Ngân sách tháng:** Nhập hạn mức chi tiêu dự kiến cho tháng và thanh tiến độ cảnh báo mức tiêu. | Hiển thị thanh Progress Bar đổi màu (Xanh $\rightarrow$ Vàng $\rightarrow$ Đỏ) khi chạm ngưỡng ngân sách. |
| **Báo cáo & Biểu đồ** | Báo cáo Net Worth, Dòng tiền ròng, Dự báo ngày cạn tiền (Out-of-Money Date), kịch bản What-If. | **2 Dạng biểu đồ trực quan:**<br>1. *Biểu đồ tròn (Pie Chart):* Cơ cấu chi tiêu theo nhóm.<br>2. *Biểu đồ cột (Bar Chart):* Đối chiếu Thu vs. Chi. | Tận dụng thư viện biểu đồ di động mượt mà, hỗ trợ lọc theo Tháng / Năm. |
| **Tối ưu Nợ & Bẫy Thuê Bao** | Giải thuật trả nợ Debt Snowball vs. Debt Avalanche; Quét bẫy gia hạn gói định kỳ Subscription. | **TẠM THỜI ĐÓNG BĂNG (Out of Scope):** Không đưa vào app mobile ở giai đoạn này. | Tránh quá tải thời gian làm đồ án; dành trọn tài nguyên cho UI/UX và tính ổn định. |
| **Hỗ trợ Quyết định (AI)** | Mô hình AI Nudge tâm lý học hành vi Richard Thaler, phân tích trì hoãn chi tiêu (Cooling-off). | **TẠM THỜI ĐÓNG BĂNG (Out of Scope):** Thay bằng câu thông báo nhắc nhở nội bộ đơn giản. | Giữ nguyên trên tài liệu PTTKHTTT làm điểm nhấn lý thuyết. |

---

## 3. Kiến Trúc & Ngăn Xếp Công Nghệ (Tech Stack)

```text
┌────────────────────────────────────────────────────────────────────────┐
│                        KIẾN TRÚC HỆ THỐNG MOBILE                       │
├────────────────────────────────────────────────────────────────────────┤
│  [ CLIENT - MOBILE APP ]                                               │
│  - React Native (JavaScript / TypeScript)                              │
│  - React Navigation (Bottom Tabs + Stack Navigator)                   │
│  - Biểu đồ: react-native-chart-kit / victory-native                    │
│  - HTTP Client: Axios / Fetch API                                      │
│                                                                        │
│                       ▲ RESTful API (JSON / HTTP)                      │
│                       │                                                │
│  [ BACKEND SERVER - MÁY CHỦ TRƯỜNG CẤP (REMOTE HOSTING) ]              │
│  - Server IP: 103.139.203.43                                           │
│  - Web Service: Apache / Nginx + PHP 8.x                               │
│  - Quản lý CSDL: phpMyAdmin (Cổng 887 - SSL)                          │
│  - Test Tools: Postman (Test trực tiếp qua IP Internet công khai)      │
│                                                                        │
│                       ▲ PDO Connection (Prepared Statements)           │
│                       │                                                │
│  [ DATABASE - MYSQL TRÊN MÁY CHỦ ]                                     │
│  - Tên CSDL: sql_nhom46_itimi                                          │
│  - Tài khoản DB: sql_nhom46_itimi                                      │
│  - 5 Bảng cốt lõi: users, wallets, categories, transactions, budgets   │
└────────────────────────────────────────────────────────────────────────┘
```

### Chi tiết các tầng công nghệ:
1. **Frontend (Mobile App):**
   - **Framework:** `React Native` (Hỗ trợ chạy mượt trên Android & iOS).
   - **Quản lý điều hướng:** `React Navigation` (Gồm Bottom Tab Bar cho 4 màn chính: Trang chủ, Lịch sử giao dịch, Báo cáo biểu đồ, Cài đặt).
   - **Thư viện biểu đồ:** `react-native-chart-kit` kết hợp `react-native-svg` để vẽ biểu đồ tròn và biểu đồ cột.
   - **Giao diện & Thành phần UI:** Kết hợp Flexbox layout với bộ Icon `@expo/vector-icons` hoặc `react-native-vector-icons`.

2. **Backend (REST API):**
   - **Ngôn ngữ:** `PHP` (Viết dạng các API endpoints hướng dịch vụ, trả về JSON thuần).
   - **Bảo mật cơ bản:** Băm mật khẩu bằng `password_hash()` (Bcrypt), chống SQL Injection bằng `PDO Prepared Statements`, cấu hình Header CORS cho phép Mobile Client gọi vào từ bất kỳ đâu.
   - **Xác thực:** Token-based authentication đơn giản (Bearer Token / API Key) lưu ở AsyncStorage trên React Native.

3. **Cơ sở dữ liệu & Máy chủ (Database & Remote Server):**
   - **Địa chỉ máy chủ (Server IP):** `103.139.203.43`
   - **Cổng & Đường dẫn phpMyAdmin:** `https://103.139.203.43:887/phpmyadmin_cd13194c0d96d231/index.php?lang=en`
   - **Tài khoản phpMyAdmin / Database User:** `sql_nhom46_itimi`
   - **Mật khẩu (Password):** `0a530d0c23153`
   - **Tên Database:** `sql_nhom46_itimi`
   - **Bảng mã:** `utf8mb4_unicode_ci` hỗ trợ đầy đủ tiếng Việt có dấu và emoji icon.

4. **Ưu điểm của Môi trường Server trường cấp:**
   - **Không phụ thuộc mạng LAN / WAMP nội bộ:** Máy tính dev, điện thoại thật (chạy 4G/Wifi) hay máy ảo đều gọi chung vào 1 server cố định.
   - **Kiểm thử API dễ dàng bằng Postman:** Test trực tiếp qua địa chỉ IP công khai của server.
   - **Làm việc nhóm thuận lợi:** Cả nhóm cùng kết nối vào một CSDL thống nhất, không bị phân mảnh dữ liệu giữa các máy cá nhân.

---

## 4. Thiết Kế CSDL Tinh Gọn (MySQL Schema Blueprint)

CSDL gồm đúng **5 bảng quan hệ** trích xuất từ Class Diagram của môn PTTKHTTT, được import trực tiếp vào database **`sql_nhom46_itimi`**:

```sql
-- Sử dụng CSDL do trường cấp
USE `sql_nhom46_itimi`;

-- 1. Bảng người dùng
CREATE TABLE `users` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `full_name` VARCHAR(100) NOT NULL,
  `email` VARCHAR(150) NOT NULL UNIQUE,
  `password_hash` VARCHAR(255) NOT NULL,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 2. Bảng ví tài chính (Tiền mặt, Ngân hàng)
CREATE TABLE `wallets` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `user_id` INT NOT NULL,
  `name` VARCHAR(100) NOT NULL,
  `balance` DECIMAL(15,2) DEFAULT 0.00,
  `wallet_type` ENUM('CASH', 'BANK') DEFAULT 'CASH',
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (`user_id`) REFERENCES `users`(`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 3. Bảng danh mục thu chi
CREATE TABLE `categories` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `user_id` INT NULL, -- NULL nếu là danh mục mặc định của hệ thống
  `name` VARCHAR(100) NOT NULL,
  `type` ENUM('INCOME', 'EXPENSE') NOT NULL,
  `icon_name` VARCHAR(50) DEFAULT 'wallet',
  `color_code` VARCHAR(20) DEFAULT '#2B4C7E'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 4. Bảng giao dịch thu / chi
CREATE TABLE `transactions` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `user_id` INT NOT NULL,
  `wallet_id` INT NOT NULL,
  `category_id` INT NOT NULL,
  `amount` DECIMAL(15,2) NOT NULL,
  `transaction_type` ENUM('INCOME', 'EXPENSE') NOT NULL,
  `note` TEXT NULL,
  `transaction_date` DATE NOT NULL,
  `created_at` DATETIME DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (`user_id`) REFERENCES `users`(`id`) ON DELETE CASCADE,
  FOREIGN KEY (`wallet_id`) REFERENCES `wallets`(`id`) ON DELETE CASCADE,
  FOREIGN KEY (`category_id`) REFERENCES `categories`(`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- 5. Bảng ngân sách hàng tháng
CREATE TABLE `budgets` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `user_id` INT NOT NULL,
  `category_id` INT NULL, -- NULL nếu là ngân sách tổng thể
  `amount_limit` DECIMAL(15,2) NOT NULL,
  `month` INT NOT NULL,
  `year` INT NOT NULL,
  FOREIGN KEY (`user_id`) REFERENCES `users`(`id`) ON DELETE CASCADE,
  FOREIGN KEY (`category_id`) REFERENCES `categories`(`id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
```

---

## 5. Danh Sách Endpoint RESTful API (PHP API Blueprint)

Toàn bộ API được triển khai tại thư mục web trên máy chủ (Ví dụ: `http://103.139.203.43/sql_nhom46_itimi/api/` hoặc đường dẫn thư mục web nhóm được phân bổ):

### 5.1. Nhóm Xác Thực (Authentication)
- `POST /api/auth/register.php` : Đăng ký tài khoản người dùng mới.
- `POST /api/auth/login.php` : Đăng nhập, trả về thông tin user & token xác thực.

### 5.2. Nhóm Ví & Số Dư (Wallets)
- `GET /api/wallets/list.php?user_id={id}` : Lấy danh sách ví và số dư hiện có.
- `POST /api/wallets/create.php` : Thêm ví mới.

### 5.3. Nhóm Danh Mục (Categories)
- `GET /api/categories/list.php?user_id={id}` : Lấy toàn bộ danh mục thu & chi.

### 5.4. Nhóm Giao Dịch Thu / Chi (Transactions)
- `GET /api/transactions/list.php?user_id={id}&month={m}&year={y}` : Danh sách giao dịch theo tháng.
- `POST /api/transactions/create.php` : Tạo mới giao dịch (Tự động cộng/trừ số dư bảng `wallets`).
- `DELETE /api/transactions/delete.php?id={id}` : Xóa giao dịch (Hoàn lại số dư ví tương ứng).

### 5.5. Nhóm Thống Kê & Báo Cáo (Reports & Charts)
- `GET /api/reports/monthly_summary.php?user_id={id}&month={m}&year={y}` : Trả về: `total_income`, `total_expense`, `net_balance`.
- `GET /api/reports/category_chart.php?user_id={id}&month={m}&year={y}` : Trả về dữ liệu vẽ Biểu đồ tròn: `[{ name, total_amount, color_code }]`.
- `GET /api/reports/income_vs_expense_chart.php?user_id={id}&year={y}` : Trả về dữ liệu vẽ Biểu đồ cột so sánh Thu - Chi qua các tháng trong năm.

---

## 6. Hướng Dẫn Thiết Lập & Khởi Chạy

### Bước 1: Khởi tạo Cơ sở Dữ liệu trên Server Trường cấp
1. Mở trình duyệt, truy cập đường dẫn phpMyAdmin:
   `https://103.139.203.43:887/phpmyadmin_cd13194c0d96d231/index.php?lang=en`
   *(Nếu trình duyệt cảnh báo chứng chỉ SSL tự ký, chọn **Advanced $\rightarrow$ Proceed to 103.139.203.43**)*.
2. Đăng nhập với thông tin:
   - **Username:** `sql_nhom46_itimi`
   - **Password:** `0a530d0c23153`
3. Chọn database `sql_nhom46_itimi` ở menu bên trái.
4. Mở tab **SQL**, dán toàn bộ đoạn mã DDL ở **Mục 4** và nhấn **Go** để khởi tạo 5 bảng.

### Bước 2: Cấu hình Kết nối CSDL trong Backend PHP (`config/database.php`)
```php
<?php
class Database {
    // Khi mã nguồn PHP chạy trực tiếp trên cùng server với MySQL:
    private $host = "localhost"; // hoặc "127.0.0.1"
    private $db_name = "sql_nhom46_itimi";
    private $username = "sql_nhom46_itimi";
    private $password = "0a530d0c23153";
    public $conn;

    public function getConnection() {
        $this->conn = null;
        try {
            $this->conn = new PDO(
                "mysql:host=" . $this->host . ";dbname=" . $this->db_name . ";charset=utf8mb4",
                $this->username,
                $this->password
            );
            $this->conn->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
        } catch(PDOException $exception) {
            echo json_encode(["error" => "Lỗi kết nối CSDL: " . $exception->getMessage()]);
        }
        return $this->conn;
    }
}
?>
```

### Bước 3: Cấu hình CORS Header cho các file API PHP
Đảm bảo đầu mỗi file PHP API luôn có các header cho phép React Native gọi vào mà không bị chặn CORS:
```php
<?php
header("Access-Control-Allow-Origin: *");
header("Content-Type: application/json; charset=UTF-8");
header("Access-Control-Allow-Methods: POST, GET, OPTIONS, DELETE, PUT");
header("Access-Control-Allow-Headers: Content-Type, Access-Control-Allow-Headers, Authorization, X-Requested-With");

if ($_SERVER['REQUEST_METHOD'] === 'OPTIONS') {
    http_response_code(200);
    exit();
}
```

### Bước 4: Kiểm thử API bằng Postman
1. Mở **Postman**, tạo Collection mới: `Personal Expense Tracker - Server Test`.
2. Tạo biến môi trường `BASE_URL`: trỏ đến thư mục API trên server (Ví dụ: `http://103.139.203.43/.../api`).
3. Gửi request `POST {{BASE_URL}}/auth/register.php` kèm raw JSON body:
   ```json
   {
     "full_name": "Nguyễn Văn A",
     "email": "nguyenvana@gmail.com",
     "password": "Password123@"
   }
   ```
4. Kiểm tra response trả về mã `200 OK` hoặc `201 Created` và dữ liệu được ghi nhận chính xác trong phpMyAdmin.

### Bước 5: Cấu hình React Native kết nối Server
1. Trong ứng dụng React Native, tạo file `src/api/config.js`:
   ```javascript
   // Cấu hình URL server thực tế của trường cấp
   export const API_BASE_URL = "http://103.139.203.43/sql_nhom46_itimi/api"; 
   ```
   *(Lợi thế: Ứng dụng chạy trên thiết bị thật, máy ảo, dùng Wifi hoặc 4G đều kết nối thông suốt mà không cần chung mạng LAN)*.
2. Cài đặt các gói thư viện cần thiết:
   ```bash
   npm install @react-navigation/native @react-navigation/bottom-tabs @react-navigation/native-stack
   npm install react-native-chart-kit react-native-svg axios
   ```
3. Khởi chạy ứng dụng:
   ```bash
   npx react-native run-android
   # hoặc với Expo:
   # npx expo start
   ```

---

## 7. Cấu Trúc Thư Mục Dự Án Khuyến Nghị

```text
mobile-project/
├── README.md                     # Tài liệu hướng dẫn đồ án mobile & thông tin server
├── BaoCaoLTMB.docx               # Báo cáo học thuật môn Lập trình Mobile
├── backend_api/                  # Mã nguồn PHP API (Upload lên server)
│   ├── config/
│   │   └── database.php          # Kết nối CSDL MySQL (sql_nhom46_itimi)
│   ├── api/
│   │   ├── auth/                 # login.php, register.php
│   │   ├── wallets/              # list.php, create.php
│   │   ├── categories/           # list.php
│   │   ├── transactions/         # list.php, create.php, delete.php
│   │   └── reports/              # monthly_summary.php, category_chart.php
│   └── sql/
│       └── database_schema.sql   # File DDL import phpMyAdmin
│
└── mobile_app/                   # Mã nguồn ứng dụng React Native
    ├── package.json
    ├── src/
    │   ├── api/                  # Axios instance trỏ tới 103.139.203.43
    │   ├── components/           # TransactionCard, CustomButton, ChartWrapper
    │   ├── navigation/           # AppNavigator, TabNavigator
    │   ├── screens/              # HomeScreen, AddTransactionScreen, ReportScreen, WalletScreen
    │   └── utils/                # FormatMoney, FormatDate
    └── App.js
```
