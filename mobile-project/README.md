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
│  [ BACKEND API - GIAI ĐOẠN 1: DEV & TEST LOCAL (WAMP SERVER) ]         │
│  - Web Service: WAMP Server (Apache 2.4.x + PHP 8.x) trên Windows      │
│  - MySQL Local: localhost:3306 (Quản lý qua phpMyAdmin của WAMP)       │
│  - Test Tools: Postman (Test trên localhost / 10.0.2.2 / IP LAN)       │
│                                                                        │
│                       ▼ KHI ĐƯỢC GIẢNG VIÊN CHO PHÉP TRIỂN KHAI        │
│                                                                        │
│  [ BACKEND API - GIAI ĐOẠN 2: PRODUCTION SERVER TRƯỜNG CẤP ]           │
│  - Server IP: 103.139.203.43 (Domain: nhom46.itimit.id.vn)            │
│  - Quản lý CSDL: phpMyAdmin (Cổng 887 - CSDL: sql_nhom46_itimi)        │
│  - Upload mã nguồn: FileZilla FTP (Host: nhom46.itimit.id.vn)          │
│  - Test Tools: Postman (Test trực tiếp qua Internet công khai)         │
└────────────────────────────────────────────────────────────────────────┘
```

### Chi tiết các tầng công nghệ:
1. **Frontend (Mobile App):**
   - **Framework:** `React Native` (Hỗ trợ chạy mượt trên Android & iOS).
   - **Quản lý điều hướng:** `React Navigation` (Gồm Bottom Tab Bar cho 4 màn chính: Trang chủ, Lịch sử giao dịch, Báo cáo biểu đồ, Cài đặt).
   - **Thư viện biểu đồ:** `react-native-chart-kit` kết hợp `react-native-svg` để vẽ biểu đồ tròn và biểu đồ cột.
   - **Giao diện & Thành phần UI:** Sử dụng các thành phần cơ bản của React Native (`View`, `Text`, `TouchableOpacity`, `TextInput`, `FlatList`, `Modal`) kết hợp Flexbox layout và bộ Icon `@expo/vector-icons`.
   - **Định kiểu (Styling) - Phục vụ Vấn Đáp:** Sử dụng thuần `StyleSheet.create({...})` kết hợp các hằng số màu sắc `const COLORS`. Tuyệt đối **KHÔNG** dùng các thư viện CSS bên thứ 3 (TailwindCSS/NativeWind, Styled-Components, Paper...) để giữ code mộc mạc, chuẩn giáo trình nhập môn, giúp sinh viên tự tin giải thích trôi chảy khi Giảng viên hỏi vấn đáp.

2. **Backend (REST API):**
   - **Ngôn ngữ:** `PHP` (Viết dạng các API endpoints hướng dịch vụ, trả về JSON thuần).
   - **Bảo mật cơ bản:** Băm mật khẩu bằng `password_hash()` (Bcrypt), chống SQL Injection bằng `PDO Prepared Statements`, cấu hình Header CORS cho phép Mobile Client gọi vào từ bất kỳ đâu.
   - **Xác thực:** Token-based authentication đơn giản (Bearer Token / API Key) lưu ở AsyncStorage trên React Native.

3. **Môi trường Cơ sở Dữ liệu & Máy chủ (Chiến lược 2 Giai đoạn):**
   - **Giai đoạn 1 (Hiện tại - Dev & Test trên Local WAMP Server):**
     - Do giảng viên chưa cho phép thao tác trực tiếp trên server trường, nhóm chủ động dựng và test toàn bộ trên **WAMP Server** cục bộ.
     - CSDL MySQL chạy trên WAMP (`localhost:3306`), tạo database cùng tên `sql_nhom46_itimi` để khi migrate lên server trường không phải sửa tên trong code.
     - Kiểm thử API nội bộ qua Postman và chạy Mobile App qua máy ảo Android (`10.0.2.2`) hoặc điện thoại thật chung mạng Wifi (`192.168.x.x`).
   - **Giai đoạn 2 (Sau khi Giảng viên cho phép - Triển khai lên Server trường cấp):**
     - **Địa chỉ máy chủ (Server IP):** `103.139.203.43` | **Domain:** `nhom46.itimit.id.vn`
     - **Đường dẫn phpMyAdmin:** `https://103.139.203.43:887/phpmyadmin_cd13194c0d96d231/index.php?lang=en`
     - **Tài khoản phpMyAdmin / DB:** `sql_nhom46_itimi` | **Mật khẩu:** `0a530d0c23153`
     - **Kết nối FileZilla (FTP):**
       - **Host:** `nhom46.itimit.id.vn`
       - **Username:** `ftp_nhom46_itimit_id_vn`
       - **Password:** `e5e971e19bbb18`
       - **Port:** `21` (Mặc định)
     - **Bảng mã:** `utf8mb4_unicode_ci` hỗ trợ đầy đủ tiếng Việt có dấu và emoji icon.

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

## 6. Hướng Dẫn Thiết Lập & Khởi Chạy (2 Giai Đoạn)

### Bước 1: Khởi tạo Cơ sở Dữ liệu

#### 👉 Trường hợp 1: Chạy trên WAMP Server (Giai đoạn hiện tại)
1. Bật **WAMP Server** (đảm bảo icon WAMP chuyển màu xanh lá cây).
2. Mở trình duyệt, truy cập `http://localhost/phpmyadmin`.
3. Đăng nhập (thường User là `root`, Password để trống).
4. Tạo CSDL mới tên: `sql_nhom46_itimi` với Collation `utf8mb4_unicode_ci`.
5. Chọn CSDL vừa tạo, mở tab **SQL**, dán toàn bộ mã DDL ở **Mục 4** và nhấn **Go** để khởi tạo 5 bảng.

#### 👉 Trường hợp 2: Khi được Giảng viên cho phép triển khai lên Server trường
1. Truy cập phpMyAdmin máy chủ: `https://103.139.203.43:887/phpmyadmin_cd13194c0d96d231/index.php?lang=en`
   *(Nếu trình duyệt cảnh báo SSL, chọn **Advanced → Proceed to 103.139.203.43**)*.
2. Đăng nhập: Username: `sql_nhom46_itimi` | Password: `0a530d0c23153`.
3. Chọn database `sql_nhom46_itimi` bên trái, mở tab **SQL**, dán mã DDL và nhấn **Go**.

---

### Bước 2: Thiết Lập Cầu Nối Backend Vào WAMP Server (NTFS Directory Junction - 0 Giây Đồng Bộ)

> [!TIP]
> **Tuyệt đối KHÔNG copy/paste thư mục `BE/` sang `www/` của WampServer.** Hãy sử dụng **NTFS Directory Junction** (liên kết thư mục cấp Hệ điều hành của Windows). Mã nguồn chỉ nằm trong Repo Git, Apache tự động nhận diện thời gian thực (0 giây) và tuyệt đối an toàn (xóa link không mất code gốc).

* 📄 **Chi tiết hướng dẫn kỹ thuật:** Xem tại [`BE/README.md`](BE/README.md).

#### Cách 1: Chạy Script Tự Động (Khuyến nghị cho cả Dev & AI Agent)
Mở PowerShell tại thư mục `mobile-project/BE/` và chạy:
```powershell
.\setup_wamp_junction.ps1
```
*(Script sẽ tự phát hiện thư mục `www` của WampServer trên máy bạn và tạo link `expense_api` trỏ về `BE`)*.

#### Cách 2: Chạy Lệnh Thủ Công Bằng PowerShell
```powershell
# Chạy trên máy bạn Đạt (WAMP cài ổ D):
New-Item -ItemType Junction -Path "D:\WampServer\www\expense_api" -Target "D:\University Study Subjects\Information System Design Analysis\Project\Analyze System\mobile-project\BE"

# Hoặc trên máy bạn Đại (Nếu WAMP cài ổ C):
New-Item -ItemType Junction -Path "C:\wamp64\www\expense_api" -Target "D:\Path\To\Your\Repo\mobile-project\BE"
```

#### Cách 3: Chạy Lệnh Bằng Command Prompt (CMD)
```cmd
mklink /J "D:\WampServer\www\expense_api" "D:\University Study Subjects\Information System Design Analysis\Project\Analyze System\mobile-project\BE"
```

---

### Bước 3: Cấu hình Kết nối CSDL Đa Môi Trường (`config/database.php`)
Sử dụng cờ `$is_local` để chuyển đổi 1 chạm giữa máy cá nhân (WAMP) và Server trường mà không lo lỗi code:

```php
<?php
class Database {
    // Đổi thành FALSE khi upload code lên Server trường cấp
    private $is_local = true; 

    // Cấu hình WAMP Server (Chạy ở máy cá nhân)
    private $local_host = "localhost";
    private $local_db   = "sql_nhom46_itimi";
    private $local_user = "root";
    private $local_pass = ""; // Mặc định WAMP để trống, nếu có mật khẩu thì điền vào đây

    // Cấu hình Server trường cấp (Production Hosting)
    private $remote_host = "localhost";
    private $remote_db   = "sql_nhom46_itimi";
    private $remote_user = "sql_nhom46_itimi";
    private $remote_pass = "0a530d0c23153";

    public $conn;

    public function getConnection() {
        $this->conn = null;
        $host = $this->is_local ? $this->local_host : $this->remote_host;
        $db   = $this->is_local ? $this->local_db   : $this->remote_db;
        $user = $this->is_local ? $this->local_user : $this->remote_user;
        $pass = $this->is_local ? $this->local_pass : $this->remote_pass;

        try {
            $this->conn = new PDO(
                "mysql:host=" . $host . ";dbname=" . $db . ";charset=utf8mb4",
                $user,
                $pass
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

---

### Bước 4: Cấu hình CORS Header cho các file API PHP
Đảm bảo đầu mỗi file PHP API luôn có các header cho phép React Native gọi vào:
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

---

### Bước 5: Kiểm thử API bằng Postman
1. **Nếu test WAMP Local:**
   - Đảm bảo đã tạo Junction ở Bước 2.
   - Biến môi trường `BASE_URL`: `http://localhost/expense_api/api`.
2. **Nếu test Server trường cấp (khi đã upload):**
   - Biến môi trường `BASE_URL`: `http://nhom46.itimit.id.vn/api` (hoặc IP công khai `http://103.139.203.43/...`).
3. Gửi request `POST {{BASE_URL}}/auth/register.php` kèm raw JSON body:
   ```json
   {
     "full_name": "Nguyễn Văn A",
     "email": "nguyenvana@gmail.com",
     "password": "Password123@"
   }
   ```
4. Kiểm tra response trả về mã `200 OK` hoặc `201 Created` và dữ liệu được ghi nhận chính xác trong phpMyAdmin.

---

### Bước 6: Cấu hình & Khởi Chạy Frontend Mobile (Trong Thư Mục `FE`)

> 📄 **Tài liệu hướng dẫn chi tiết dành riêng cho UI Lead:** Xem ngay tại [`FE/README.md`](FE/README.md).

1. **Mở terminal và di chuyển vào thư mục:**
   ```powershell
   cd mobile-project/FE
   ```
2. **Cài đặt thư viện dependencies:**
   * **Dùng Expo CLI (Khuyên dùng nhất cho Đại):** `npx expo install`
   * **Dùng `npm`:** `npm install --legacy-peer-deps` *(bắt buộc cờ `--legacy-peer-deps` do React 19)*
   * **Dùng `pnpm` (trên máy của Đạt):** `pnpm install`
3. **Cấu hình IP Backend:** Sửa địa chỉ IPv4 máy tính tại [`src/api/config.js`](FE/src/api/config.js).
4. **Khởi chạy ứng dụng:**
   ```powershell
   npm start
   # hoặc khởi động xóa sạch cache: npx expo start -c
   ```

---

### Bước 7: Hướng Dẫn Đẩy Code Lên Server Trường Bằng FileZilla FTP (Khi Được Phép)
1. Tải và mở phần mềm **FileZilla Client**.
2. Nhập thông tin kết nối nhanh (Quickconnect):
   - **Host:** `nhom46.itimit.id.vn`
   - **Username:** `ftp_nhom46_itimit_id_vn`
   - **Password:** `e5e971e19bbb18`
   - **Port:** `21`
3. Nhấn **Quickconnect** để kết nối vào máy chủ.
4. Ở khung bên phải (Remote site): Di chuyển vào thư mục web gốc (`public_html` hoặc `www`).
5. Ở khung bên trái (Local site): Chọn toàn bộ mã nguồn thư mục `BE/` trên máy tính và kéo thả sang bên phải để tải lên máy chủ.
6. Mở file `config/database.php` trên máy chủ và chuyển sang chế độ server: `$is_local = false;`.


---

## 7. Cấu Trúc Thư Mục Dự Án Chuẩn

```text
mobile-project/
├── README.md                     # Tài liệu hướng dẫn đồ án mobile & thông tin server
├── ROADMAP.md                    # Kế hoạch lộ trình 5 Sprint và RACI Matrix
├── huongdanvietbaocao.txt        # Yêu cầu hướng dẫn viết báo cáo của Giảng viên
├── BaoCaoLTMB.docx               # Báo cáo học thuật môn Lập trình Mobile (Word)
│
├── BE/                           # Backend PHP RESTful API
│   ├── config/
│   │   ├── database.php          # Kết nối CSDL MySQL (Hỗ trợ WAMP & Server trường)
│   │   └── helpers.php           # CORS headers & Chuẩn hóa JSON Response
│   ├── api/
│   │   ├── auth/                 # login.php, register.php
│   │   ├── wallets/              # list.php, create.php
│   │   ├── categories/           # list.php
│   │   ├── transactions/         # list.php, create.php, delete.php
│   │   └── reports/              # monthly_summary.php, category_chart.php
│   └── sql/
│       └── database_schema.sql   # DDL khởi tạo 5 bảng & Dữ liệu mẫu (Seed Data)
│
└── FE/                           # Frontend Ứng dụng React Native
    ├── package.json
    ├── src/
    │   ├── api/                  # Axios instance & config.js (Đa môi trường)
    │   ├── components/           # TransactionCard, CustomButton, ChartWrapper
    │   ├── navigation/           # AppNavigator, TabNavigator
    │   ├── screens/              # HomeScreen, AddTransactionScreen, ReportScreen, WalletScreen
    │   └── utils/                # FormatMoney, FormatDate
    └── App.js
```
