# 🗺️ LỘ TRÌNH TRIỂN KHAI DỰ ÁN (PROJECT DEVELOPMENT ROADMAP)
## ĐỒ ÁN MÔN HỌC: LẬP TRÌNH THIẾT BỊ DI ĐỘNG (MOBILE APPLICATION DEVELOPMENT)
> **Tên đề tài:** Ứng dụng Quản lý Chi tiêu Cá nhân (Personal Expense Tracker)  
> **Ngăn xếp công nghệ:** React Native (Frontend) + PHP RESTful API (Backend) + MySQL (WAMP Localhost / Server trường cấp `103.139.203.43:887` qua FTP `nhom46.itimit.id.vn`)  
> **Tài liệu tham chiếu:** [README.md](README.md) | [BaoCaoLTMB.docx](BaoCaoLTMB.docx)

---

## 1. Thông Tin Nhóm & Phân Định Trách Nhiệm (Team Roster & RACI Matrix)

### 1.1. Thành viên tham gia
- **Trịnh Thành Đạt** — MSSV: `1923050149` (Trưởng nhóm - Lead / Full-Stack & Architecture)
- **Trần Trọng Đại** — MSSV: `1923050745` (Thành viên - Frontend UI & API Testing)

### 1.2. Ma trận phân công trách nhiệm (RACI Matrix)
- **R (Responsible):** Người trực tiếp code/thực hiện.
- **A (Accountable):** Người chịu trách nhiệm rà soát cuối cùng, duyệt PR / Merge code.
- **C (Consulted):** Người đóng góp ý kiến, cùng thảo luận giải pháp.
- **I (Informed):** Người được thông báo kết quả.

| Hạng mục công việc | Trịnh Thành Đạt | Trần Trọng Đại | Sản phẩm bàn giao (Deliverables) |
| :--- | :---: | :---: | :--- |
| **Thiết kế & Khởi tạo CSDL MySQL** | **A / R** | **C** | File `database_schema.sql`, ERD Diagram trên Draw.io, CSDL trên WAMP & Server trường |
| **Xây dựng Backend PHP Core (Database, Auth)** | **A / R** | **C** | `config/database.php` (Đa môi trường), `api/auth/register.php`, `login.php` |
| **Xây dựng Backend PHP Nghiệp vụ (Wallets, Transactions)** | **A** | **R** | `api/wallets/*`, `api/categories/*`, `api/transactions/*` |
| **Xây dựng Backend PHP Báo cáo (Reports & Charts)** | **A / R** | **C** | `api/reports/monthly_summary.php`, `category_chart.php` |
| **Kiểm thử API trên Postman & Chụp minh chứng** | **A** | **R** | Postman Collection JSON, Bộ ảnh chụp màn hình JSON response (WAMP & Server) |
| **Triển khai Server trường qua FTP & phpMyAdmin** | **A / R** | **R** | Deploy code qua FileZilla (`nhom46.itimit.id.vn`), DB sống trên phpMyAdmin trường |
| **Khởi tạo React Native & Luồng Điều hướng Navigation** | **A / R** | **C** | Source React Native, BottomTabNavigator, StackNavigator |
| **Thiết kế Linh kiện UI dùng chung (Components)** | **C** | **A / R** | Header, Card Ví, TransactionItem, CustomButton, NumberKeypad |
| **Hiện thực các Màn hình (Dashboard, Add, History)** | **A / R** | **R** | HomeScreen, AddTransactionScreen, TransactionListScreen |
| **Hiện thực Màn hình Báo cáo Biểu đồ (Charts Screen)** | **A / R** | **C** | ReportScreen tích hợp `react-native-chart-kit` |
| **Hoàn thiện Báo cáo Word (`BaoCaoLTMB.docx`)** | **A / R** | **R** | Tệp Word nộp bài hoàn chỉnh, cập nhật mục lục tự động F9 |

---

## 2. Kế Hoạch 5 Giai Đoạn Chi Tiết (Phased Sprint Roadmap)

```text
┌────────────────────────────────────────────────────────────────────────────────────────┐
│                        TIẾN TRÌNH THỰC HIỆN DỰ ÁN (4 - 5 TUẦN)                         │
├─────────────────┬─────────────────┬─────────────────┬─────────────────┬────────────────┤
│   GIAI ĐOẠN 1   │   GIAI ĐOẠN 2   │   GIAI ĐOẠN 3   │   GIAI ĐOẠN 4   │   GIAI ĐOẠN 5  │
│  Thiết Kế CSDL  │  Backend PHP &  │ React Native UI │  Ghép Nối API & │ Báo Cáo Word & │
│  & WAMP Server  │  Postman Tests  │    Khung App    │ Hoàn Thiện App  │ Deploy Server  │
│    (Tuần 1)     │    (Tuần 2)     │    (Tuần 3)     │  (Tuần 4 - 5)   │  (Tuần 5 - 6)  │
└─────────────────┴─────────────────┴─────────────────┴─────────────────┴────────────────┘
```

---

### Giai Đoạn 1: Thiết Kế CSDL & Chuẩn Bị Môi Trường WAMP Local (Tuần 1)
> **Mục tiêu:** Có sẵn database chạy mượt trên WAMP Server máy cá nhân và Git repository sẵn sàng cộng tác. (Chưa can thiệp server trường do thầy chưa cho phép).

- [ ] **Task 1.1 (Đạt):** Vẽ sơ đồ thực thể liên kết (ERD) cho 5 bảng (`users`, `wallets`, `categories`, `transactions`, `budgets`) trên Draw.io, xuất file ảnh nét cao cho Trang 7 báo cáo.
- [ ] **Task 1.2 (Đạt):** Cài đặt/Khởi động WAMP Server, vào phpMyAdmin local (`http://localhost/phpmyadmin`), tạo CSDL `sql_nhom46_itimi` và chạy mã SQL tạo 5 bảng.
- [ ] **Task 1.3 (Đại):** Soạn thảo dữ liệu mẫu (Seed Data) gồm: 1 user test, 2 ví (Tiền mặt, Ngân hàng), 8 danh mục chi tiêu, 4 danh mục thu nhập, 5 giao dịch mẫu để kiểm tra hiển thị.
- [ ] **Task 1.4 (Đạt & Đại):** Đồng bộ mã nguồn lên Git repository GitHub (`analyze-system-subject`), thiết lập branch `main` và phân quyền cho cả 2 người.

---

### Giai Đoạn 2: Xây Dựng Backend PHP API & Kiểm Thử Postman Trên WAMP (Tuần 2)
> **Mục tiêu:** 100% các API hoạt động ổn định trên môi trường WAMP Local, có file Postman Collection để test độc lập trước khi gắn vào Mobile.

- [ ] **Task 2.1 (Đạt):** Viết module kết nối CSDL `config/database.php` đa môi trường (sử dụng cờ `$is_local` để chuyển đổi linh hoạt giữa WAMP và Server trường) và cấu hình Header CORS chung cho phép mọi nguồn gọi API.
- [ ] **Task 2.2 (Đạt):** Hiện thực API Xác thực người dùng:
  - `POST /api/auth/register.php`: Băm mật khẩu bằng `password_hash()`, kiểm tra trùng lặp email.
  - `POST /api/auth/login.php`: Kiểm tra `password_verify()`, trả về thông tin user.
- [ ] **Task 2.3 (Đại):** Hiện thực API Quản lý Ví và Danh mục:
  - `GET /api/wallets/list.php?user_id=...` & `POST /api/wallets/create.php`.
  - `GET /api/categories/list.php?user_id=...`.
- [ ] **Task 2.4 (Đại):** Hiện thực API Quản lý Giao dịch Thu / Chi:
  - `POST /api/transactions/create.php`: Thêm giao dịch, **đồng thời tự động cộng/trừ số dư** trong bảng `wallets`.
  - `GET /api/transactions/list.php?user_id=...&month=...&year=...`: Lấy lịch sử giao dịch theo tháng.
  - `DELETE /api/transactions/delete.php?id=...`: Xóa giao dịch và hoàn lại số dư ví tương ứng.
- [ ] **Task 2.5 (Đạt):** Hiện thực API Báo cáo & Thống kê:
  - `GET /api/reports/monthly_summary.php`: Trả về `total_income`, `total_expense`, `net_balance`.
  - `GET /api/reports/category_chart.php`: Gom nhóm chi tiêu theo danh mục để vẽ biểu đồ tròn.
- [ ] **Task 2.6 (Đại):** Tạo Postman Collection `Expense_Tracker_API.postman_collection.json`, chạy test 100% endpoints trên `http://localhost/expense_api/api`, **chụp ảnh toàn bộ màn hình kết quả JSON** lưu vào thư mục `assets/postman_screenshots/` để chuẩn bị cho Trang 8 báo cáo Word.

---

### Giai Đoạn 3: Khởi Tạo Ứng Dụng React Native & Dựng Khung Giao Diện (Tuần 3)
> **Mục tiêu:** Dựng xong bộ khung ứng dụng (Shell App), điều hướng mượt mà giữa các màn hình, có các linh kiện UI đẹp mắt.

- [ ] **Task 3.1 (Đạt):** Khởi tạo project React Native (sử dụng React Native CLI hoặc Expo), cài đặt các thư viện thiết yếu:
  ```bash
  npm install @react-navigation/native @react-navigation/bottom-tabs @react-navigation/native-stack
  npm install react-native-screens react-native-safe-area-context
  npm install axios react-native-chart-kit react-native-svg
  npm install @expo/vector-icons # hoặc react-native-vector-icons
  ```
- [ ] **Task 3.2 (Đạt):** Cấu hình tầng mạng (Network Layer):
  - Tạo file `src/api/config.js` hỗ trợ cấu hình đa môi trường:
    - Android Emulator: `http://10.0.2.2/expense_api/api`
    - Thiết bị thật chung Wifi: `http://192.168.1.X/expense_api/api`
    - Server trường cấp: `http://nhom46.itimit.id.vn/api`
  - Tạo Axios instance cấu hình timeout, error interceptor chung.
- [ ] **Task 3.3 (Đạt):** Thiết lập kiến trúc điều hướng React Navigation:
  - `AuthNavigator`: Màn hình Đăng nhập (LoginScreen) & Đăng ký (RegisterScreen).
  - `MainTabNavigator` (Bottom Tabs 4 tab):
    1. 🏠 **Trang chủ (HomeScreen):** Xem số dư ví, ngân sách và giao dịch gần đây.
    2. ➕ **Thêm mới (AddTransactionScreen):** Form nhập nhanh thu/chi.
    3. 📜 **Lịch sử (HistoryScreen):** Danh sách giao dịch theo tháng.
    4. 📊 **Báo cáo (ReportScreen):** Biểu đồ tròn và biểu đồ cột.
- [ ] **Task 3.4 (Đại):** Thiết kế bộ linh kiện UI tái sử dụng (`src/components/`):
  - `WalletCard.js`: Hiển thị thẻ số dư ví tiền mặt / ngân hàng.
  - `TransactionItem.js`: Dòng hiển thị giao dịch (Icon danh mục, tên, số tiền màu xanh/đỏ, ngày tháng).
  - `CustomButton.js`: Nút bấm bo góc chuẩn hiện đại.
  - `HeaderBar.js`: Thanh tiêu đề đầu trang.

---

### Giai Đoạn 4: Ghép Nối API & Hoàn Thiện Chức Năng Ứng Dụng (Tuần 4 - 5)
> **Mục tiêu:** App chạy thật trên máy ảo / điện thoại di động, gọi API trực tiếp vào WAMP Server, dữ liệu cập nhật tức thì.

- [ ] **Task 4.1 (Đại):** Hoàn thiện Luồng Đăng nhập / Đăng ký:
  - Form validation: Kiểm tra email hợp lệ, mật khẩu tối thiểu 6 ký tự.
  - Gọi API login/register, lưu thông tin user vào `AsyncStorage`, tự động đăng nhập khi mở lại app.
- [ ] **Task 4.2 (Đạt):** Hoàn thiện Màn hình Trang Chủ (HomeScreen):
  - Gọi API lấy danh sách ví và tổng số dư khả dụng.
  - Hiển thị thanh tiến độ ngân sách tháng (Progress Bar đổi màu: Xanh lá khi < 70%, Vàng khi 70-90%, Đỏ khi > 90%).
  - Danh sách 5 giao dịch phát sinh gần nhất kèm nút "Xem tất cả".
- [ ] **Task 4.3 (Đạt & Đại):** Hoàn thiện Màn hình Thêm Giao Dịch (AddTransactionScreen):
  - Chuyển đổi tab: "Khoản Chi" (Mặc định) vs "Khoản Thu".
  - Bàn phím số nhập tiền nhanh chóng, tự động format tiền tệ (Ví dụ: `50,000 đ`).
  - Lưới chọn danh mục (Grid View) có icon và màu sắc trực quan.
  - Chọn ngày giao dịch (DateTimePicker).
  - Bấm "Lưu giao dịch" $\rightarrow$ gọi API tạo mới $\rightarrow$ cập nhật ngay lập tức về Trang chủ.
- [ ] **Task 4.4 (Đại):** Hoàn thiện Màn hình Lịch Sử (HistoryScreen):
  - Bộ chọn Tháng / Năm để lọc dữ liệu.
  - Danh sách giao dịch gom nhóm theo ngày.
  - Thao tác vuốt/bấm để Xóa giao dịch (kèm hộp thoại xác nhận) $\rightarrow$ gọi API xóa.
- [ ] **Task 4.5 (Đạt):** Hoàn thiện Màn hình Báo Cáo Biểu Đồ (ReportScreen):
  - **Biểu đồ tròn (Pie Chart):** Thể hiện tỷ trọng % chi tiêu theo từng danh mục trong tháng bằng thư viện `react-native-chart-kit`.
  - **Biểu đồ cột (Bar Chart):** So sánh tổng Thu nhập vs Tổng Chi tiêu qua các tháng.
  - Bảng thống kê số dư ròng cuối kỳ.
- [ ] **Task 4.6 (Đại):** Kiểm thử toàn diện trên thiết bị di động thật và máy ảo, **chụp toàn bộ ảnh chụp màn hình các chức năng** lưu vào thư mục `assets/mobile_screenshots/`.

---

### Giai Đoạn 5: Triển Khai Server Trường, Báo Cáo Word & Đóng Gói (Tuần 5 - 6)
> **Mục tiêu:** Triển khai lên server trường (khi được phép), hoàn thiện tệp báo cáo Word `BaoCaoLTMB.docx` và sẵn sàng nộp cho Giảng viên.

- [ ] **Task 5.0 (Đạt & Đại) - KHI ĐƯỢC GIẢNG VIÊN CHO PHÉP:**
  - Truy cập phpMyAdmin server trường (`https://103.139.203.43:887/`, user `sql_nhom46_itimi`), import database `sql_nhom46_itimi`.
  - Dùng **FileZilla** (Host: `nhom46.itimit.id.vn`, User: `ftp_nhom46_itimit_id_vn`, Pass: `e5e971e19bbb18`) upload mã nguồn `backend_api/` lên thư mục web trên server.
  - Chuyển cấu hình `$is_local = false` trong `config/database.php`.
  - Test kiểm tra lại 1 lượt các API trên domain server qua Postman.
- [ ] **Task 5.1 (Đạt):** Mở tệp `mobile-project/BaoCaoLTMB.docx`:
  - Tại Trang 7 (*Xây dựng mô hình DB*): Chèn ảnh sơ đồ ERD, mô tả chi tiết 5 bảng dữ liệu MySQL.
  - Chèn ảnh chụp giao diện phpMyAdmin đã có đủ 5 bảng.
- [ ] **Task 5.2 (Đại):**
  - Tại Trang 8 (*API tương ứng các chức năng*): Chèn ảnh chụp Postman các API (Auth, Wallets, Transactions, Reports) kèm đoạn code logic PHP tiêu biểu.
- [ ] **Task 5.3 (Đạt & Đại):**
  - Tại Trang 9–10 (*Xây dựng Front-End React Native*): Chèn bộ ảnh chụp màn hình app di động thực tế (Dashboard, Thêm giao dịch, Lịch sử, Biểu đồ) kèm thuyết minh từng màn hình.
- [ ] **Task 5.4 (Đạt):** Cập nhật toàn bộ tài liệu bằng phím **`F9`** trong Microsoft Word để làm mới tự động:
  - **Mục lục chung (TOC)**
  - **Mục lục hình ảnh (TOF)** (tất cả các hình ERD, Postman, App UI tự động nhảy đúng trang)
  - **Mục lục bảng biểu (TOT)**
- [ ] **Task 5.5 (Đạt & Đại):** Xuất file PDF dự phòng, đóng gói mã nguồn và chuẩn bị slide thuyết trình demo trước Giảng viên.

---

## 3. Quy Ước Phối Hợp Làm Việc Nhóm (Git & Collaboration Protocol)

Để tránh xung đột mã nguồn (Merge Conflicts) và đảm bảo code luôn chạy được:

### 3.1. Phân nhánh Git chuẩn mực
```text
main (Nhánh ổn định chỉ merge khi tính năng đã test xong)
  └── develop (Nhánh tích hợp chung của nhóm)
        ├── feature/backend-api-auth (Đạt làm Auth API)
        ├── feature/backend-api-transactions (Đại làm Transaction API)
        ├── feature/ui-navigation (Đạt làm Navigation)
        └── feature/ui-screens (Đại làm các màn hình chức năng)
```

### 3.2. Quy tắc Commit & Push
1. **Luôn `git pull origin develop`** trước khi bắt đầu code tính năng mới để tránh xung đột.
2. Đặt thông điệp commit rõ ràng theo chuẩn:
   - `feat: thêm API tạo giao dịch thu chi`
   - `fix: sửa lỗi tính sai số dư ví khi xóa giao dịch`
   - `ui: hoàn thiện màn hình dashboard`
   - `docs: cập nhật hình ảnh báo cáo Word`
3. Tuyệt đối không commit các file nhạy cảm hoặc thư mục nặng lên Git:
   - Đưa `node_modules/`, `.expo/`, `.env` vào file `.gitignore`.

### 3.3. Họp nhóm & Đồng bộ tiến độ
- **Tần suất:** Họp nhanh 15–20 phút vào **Tối Thứ 4** và **Tối Chủ Nhật** hàng tuần (qua Google Meet / Discord).
- **Nội dung:**
  1. Tuần qua đã làm xong task nào?
  2. Có gặp vướng mắc kỹ thuật gì (lỗi API, lỗi layout) không?
  3. Tuần tới ai sẽ làm task nào theo Roadmap?
