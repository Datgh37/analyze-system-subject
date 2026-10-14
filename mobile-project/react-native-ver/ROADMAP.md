# 🗺️ LỘ TRÌNH TRIỂN KHAI DỰ ÁN (PROJECT DEVELOPMENT ROADMAP)
## ĐỒ ÁN MÔN HỌC: LẬP TRÌNH THIẾT BỊ DI ĐỘNG (MOBILE APPLICATION DEVELOPMENT)
> **Ngăn xếp công nghệ chuyển tiếp (Transitional Stack):** React Native Expo SDK 54 + PHP PDO RESTful API (21 endpoints bảo mật JWT) + MySQL WAMP Server Localhost.  
> **Định hướng nâng cấp (Target Modern Stack):** Sẵn sàng chuyển đổi sang **.NET MAUI** hoặc **React Native** kết hợp **Backend ASP.NET Core Web API (.NET 8/9)** / **Node.js (NestJS)** theo chỉ đạo của Giảng viên mới.  
> **Tài liệu tham chiếu:** [README.md](README.md) | [BaoCaoLTMB.docx](BaoCaoLTMB.docx) | [Kế hoạch PLAN-22](../../project_context/plans/plans-mobile-app/2026-10-07_plan_22_tech_stack_evolution_and_migration_strategy.md)

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

| Hạng mục công việc | Trịnh Thành Đạt (Lead) | Trần Trọng Đại (UI/FE) | Sản phẩm bàn giao (Deliverables) |
| :--- | :---: | :---: | :--- |
| **Thiết kế & Khởi tạo CSDL MySQL** | **A / R** | **C** | File `database_schema.sql`, ERD Diagram, CSDL trên WAMP & Server trường |
| **Xây dựng Backend PHP Core (Database, Auth)** | **A / R** | **C** | `config/database.php` (Đa môi trường), `api/auth/register.php`, `login.php` |
| **Xây dựng Backend PHP Nghiệp vụ (Wallets, Transactions)** | **A** | **R** | `api/wallets/*`, `api/categories/*`, `api/transactions/*` |
| **Xây dựng Backend PHP Báo cáo (Reports & Charts API)** | **A / R** | **C** | `api/reports/monthly_summary.php`, `category_chart.php`, `income_vs_expense_chart.php` |
| **Kiểm thử API trên Postman & Chụp minh chứng** | **A** | **R** | Postman Collection JSON, Bộ ảnh chụp màn hình JSON response (Trang 8 báo cáo) |
| **Triển khai Server trường qua FTP & phpMyAdmin** | **A / R** | **R** | Deploy code qua FileZilla (`nhom46.itimit.id.vn`), DB sống trên phpMyAdmin trường |
| **Khởi tạo & Cấu hình Kiến trúc Frontend (Setup FE)** | **A / R** | **C** | Setup project React Native, cấu hình API network (`config.js`), Root & Tab Navigation |
| **Thiết kế Linh kiện UI dùng chung (Components)** | **C** | **A / R** | Card Ví, Thanh ngân sách BudgetBar, TransactionRow, CustomButton, HeaderBar |
| **Hiện thực các Màn hình Ứng dụng (UI Screens)** | **C** | **A / R** | LoginScreen, RegisterScreen, HomeScreen, AddTransactionScreen, HistoryScreen |
| **Hiện thực Màn hình Báo cáo Biểu đồ (ReportScreen UI)** | **C (Hỗ trợ data API)** | **A / R** | ReportScreen tích hợp `react-native-chart-kit` (PieChart & BarChart) |
| **Kiểm thử giao diện App & Chụp ảnh minh chứng Mobile** | **I** | **A / R** | Bộ ảnh chụp màn hình ứng dụng di động cho Trang 9–10 báo cáo Word |
| **Hoàn thiện Báo cáo Word (`BaoCaoLTMB.docx`)** | **A / R (Trang 7, Tổng hợp, F9)** | **R (Trang 8, 9, 10)** | Tệp Word nộp bài hoàn chỉnh, cập nhật mục lục tự động F9 |

### 1.3. 📢 THÔNG BÁO QUAN TRỌNG: DỠ BỎ GIỚI HẠN CÔNG NGHỆ & CHIẾN LƯỢC CHUYỂN ĐỔI

> [!IMPORTANT]
> **CẬP NHẬT ĐỊNH HƯỚNG THEO GIẢNG VIÊN MỚI TIẾP QUẢN MÔN HỌC (07/10/2026):**  
> Giảng viên cũ đã rời đi và Thầy mới đã chính thức tiếp quản môn học. Do đó:
> 1. **Dỡ bỏ 100% các rào cản công nghệ cũ:** Không còn bị ép buộc phải code các component đơn sơ để đối phó thi vấn đáp nhập môn. Nhóm được toàn quyền tự do sử dụng các thư viện UI hiện đại, animations mượt mà, công nghệ tiên tiến để xây dựng ứng dụng di động hoàn chỉnh, chuyên nghiệp và có chiều sâu thiết kế hệ thống.
> 2. **Dừng triển khai lên server hosting cũ của thầy trước:** Không sử dụng server `103.139.203.43 / nhom46.itimit.id.vn`.
> 3. **Chiến lược Hai Giai Đoạn (Two-Phase Strategy):**
>    - **Giai đoạn Hiện tại (Tạm thời):** Giữ nguyên ngăn xếp hiện tại (**React Native Expo SDK 54 + PHP WAMP 21 RESTful API**) để chạy test, kiểm chứng trọn vẹn toàn bộ luồng nghiệp vụ tài chính cá nhân, logic tính toán ví, ngân sách và báo cáo.
>    - **Giai đoạn Sắp tới (Trùng tu & Hiện đại hóa):** Khi Giảng viên mới chốt yêu cầu chính thức, nhóm sẽ tiến hành chuyển đổi sang stack công nghệ chuẩn công nghiệp:
>      - *Phương án 1:* **.NET MAUI (C# / XAML)** kết hợp **Backend ASP.NET Core Web API (.NET 8/9, Clean Architecture)**.
>      - *Phương án 2:* **React Native (Expo)** kết hợp **Backend .NET Web API** hoặc **Node.js (NestJS)**.
> 4. **Giữ vững giá trị cốt lõi:** Đề tài vẫn giữ nguyên 100% phạm vi Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh (không quỹ nhóm); toàn bộ 21 API contracts và cấu trúc CSDL 6 bảng quan hệ sẽ được kế thừa nguyên vẹn.

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

- [x] **Task 1.1 (Đạt):** Vẽ sơ đồ thực thể liên kết (ERD) cho 5 bảng (`users`, `wallets`, `categories`, `transactions`, `budgets`) trên Draw.io ([`database_erd.drawio`](database_erd.drawio)), xuất file ảnh nét cao cho Trang 7 báo cáo.
- [x] **Task 1.2 (Đạt):** Soạn thảo mã CSDL `BE/sql/database_schema.sql`, thiết lập cầu nối NTFS Directory Junction `www/expense_api` vào WAMP Server, sẵn sàng import phpMyAdmin.
- [ ] **Task 1.3 (Đại):** Soạn thảo dữ liệu mẫu (Seed Data) gồm: 1 user test, 2 ví (Tiền mặt, Ngân hàng), 8 danh mục chi tiêu, 4 danh mục thu nhập, 5 giao dịch mẫu để kiểm tra hiển thị. *(Đạt đã hỗ trợ viết sẵn mẫu trong database_schema.sql)*.
- [x] **Task 1.4 (Đạt & Đại):** Đồng bộ mã nguồn lên Git repository GitHub (`analyze-system-subject`), thiết lập branch `main` và liên kết remote `origin`.

---

### Giai Đoạn 2: Xây Dựng Backend PHP API & Kiểm Thử Postman Trên WAMP (Tuần 2)
> **Mục tiêu:** 100% các API hoạt động ổn định trên môi trường WAMP Local, có file Postman Collection để test độc lập trước khi gắn vào Mobile.

- [x] **Task 2.1 (Đạt):** Viết module kết nối CSDL `config/database.php` đa môi trường (sử dụng cờ `$is_local` để chuyển đổi linh hoạt giữa WAMP và Server trường) và cấu hình Header CORS chung `config/helpers.php` cho phép mọi nguồn gọi API.
- [x] **Task 2.2 (Đạt):** Hiện thực API Xác thực người dùng:
  - `POST /api/auth/register.php`: Băm mật khẩu bằng `password_hash()`, kiểm tra trùng lặp email.
  - `POST /api/auth/login.php`: Kiểm tra `password_verify()`, trả về thông tin user.
- [ ] **Task 2.3 (Đại):** Hiện thực API Quản lý Ví và Danh mục:
  - `GET /api/wallets/list.php?user_id=...` & `POST /api/wallets/create.php`.
  - `GET /api/categories/list.php?user_id=...`.
  *(Đạt đã tạo sẵn toàn bộ file boilerplate trong `BE/api/`)*.
- [ ] **Task 2.4 (Đại):** Hiện thực API Quản lý Giao dịch Thu / Chi:
  - `POST /api/transactions/create.php`: Thêm giao dịch, **đồng thời tự động cộng/trừ số dư** trong bảng `wallets`.
  - `GET /api/transactions/list.php?user_id=...&month=...&year=...`: Lấy lịch sử giao dịch theo tháng.
  - `DELETE /api/transactions/delete.php?id=...`: Xóa giao dịch và hoàn lại số dư ví tương ứng.
  *(Đạt đã tạo sẵn toàn bộ file boilerplate trong `BE/api/`)*.
- [x] **Task 2.5 (Đạt):** Hiện thực API Báo cáo & Thống kê:
  - `GET /api/reports/monthly_summary.php`: Trả về `total_income`, `total_expense`, `net_balance`.
  - `GET /api/reports/category_chart.php`: Gom nhóm chi tiêu theo danh mục để vẽ biểu đồ tròn.
  - `GET /api/reports/income_vs_expense_chart.php`: Thống kê Thu vs Chi qua các tháng vẽ biểu đồ cột.
- [ ] **Task 2.6 (Đại):** Tạo Postman Collection `Expense_Tracker_API.postman_collection.json`, chạy test 100% endpoints trên `http://localhost/expense_api/api` *(hoặc chạy script `BE/test_api.ps1`)*, **chụp ảnh toàn bộ màn hình kết quả JSON** lưu vào thư mục `assets/postman_screenshots/` để chuẩn bị cho Trang 8 báo cáo Word.

---

### Giai Đoạn 3: Khởi Tạo Ứng Dụng React Native & Dựng Khung Giao Diện (Tuần 3)
> **Mục tiêu:** Đạt hoàn tất thiết lập khung sườn dự án (FE Setup & Navigation Architecture), Đại xây dựng bộ linh kiện UI dùng chung.

- [x] **Task 3.1 (Đạt - Lead Setup FE):** Khởi tạo project React Native trong thư mục `mobile-project/react-native-ver/FE/`, cài đặt các thư viện thiết yếu:
  ```bash
  npm install @react-navigation/native @react-navigation/bottom-tabs @react-navigation/native-stack
  npm install react-native-screens react-native-safe-area-context
  npm install axios react-native-chart-kit react-native-svg
  npm install @expo/vector-icons # hoặc react-native-vector-icons
  ```
- [x] **Task 3.2 (Đạt - Lead Setup FE):** Cấu hình tầng mạng (Network Layer):
  - Tạo file `src/api/config.js` hỗ trợ cấu hình đa môi trường:
    - Android Emulator: `http://10.0.2.2/expense_api/api`
    - Thiết bị thật chung Wifi: `http://192.168.1.X/expense_api/api`
    - Server trường cấp: `http://nhom46.itimit.id.vn/api`
  - Tạo Axios instance `src/api/client.js` cấu hình timeout, error interceptor và hàm đính kèm Token tự động.
- [x] **Task 3.3 (Đạt - Lead Setup FE):** Thiết lập kiến trúc điều hướng React Navigation & Theme chuẩn vấn đáp:
  - Bảng màu & kích thước tập trung `src/constants/theme.js` (không dùng CSS ngoài).
  - `AuthNavigator`: Luồng Đăng nhập (`LoginScreen.js`).
  - `MainTabNavigator` (`TabNavigator.js` với 4 màn hình: `HomeScreen`, `AddTransactionScreen`, `HistoryScreen`, `ReportScreen`).
- [ ] **Task 3.4 (Đại - UI Specialist):** Thiết kế bộ linh kiện UI tái sử dụng (`src/components/`):
  - `WalletCard.js`: Hiển thị thẻ số dư ví tiền mặt / ngân hàng bo góc đẹp mắt.
  - `BudgetBar.js`: Thanh tiến độ ngân sách tháng (đổi màu Xanh -> Vàng -> Đỏ).
  - `TransactionItem.js`: Dòng hiển thị giao dịch (Icon danh mục, tên, số tiền màu xanh/đỏ, ngày tháng).
  - `CustomButton.js`: Nút bấm bo góc chuẩn hiện đại.
  - `HeaderBar.js`: Thanh tiêu đề đầu trang và nút quay lại.

---

### Giai Đoạn 4: Hiện Thực Toàn Bộ Giao Diện (UI) & Ghép Nối API (Tuần 4 - 5)
> **Mục tiêu:** Đại hiện thực toàn bộ các màn hình ứng dụng React Native, gọi API trực tiếp vào WAMP Server; Đạt hỗ trợ tích hợp luồng dữ liệu và rà soát kỹ thuật.

- [ ] **Task 4.1 (Đại - UI/FE):** Hoàn thiện Màn hình Đăng nhập / Đăng ký (`LoginScreen`, `RegisterScreen`):
  - Form validation: Kiểm tra email hợp lệ, mật khẩu tối thiểu 6 ký tự.
  - Gọi API login/register, lưu thông tin user vào `AsyncStorage`, tự động đăng nhập khi mở lại app.
- [ ] **Task 4.2 (Đại - UI/FE):** Hoàn thiện Màn hình Trang Chủ (`HomeScreen`):
  - Dựng layout hiển thị thẻ ví (`WalletCard`) và thanh ngân sách tháng (`BudgetBar`).
  - Gọi API lấy danh sách ví và tổng số dư khả dụng từ WAMP Server.
  - Hiển thị danh sách 5 giao dịch phát sinh gần nhất kèm nút "Xem tất cả".
- [ ] **Task 4.3 (Đại - UI/FE):** Hoàn thiện Màn hình Thêm Giao Dịch (`AddTransactionScreen`):
  - Chuyển đổi tab: "Khoản Chi" (Mặc định) vs "Khoản Thu".
  - Bàn phím số nhập tiền nhanh chóng, tự động format tiền tệ (Ví dụ: `50,000 đ`).
  - Lưới chọn danh mục (Grid View) có icon và màu sắc trực quan.
  - Chọn ngày giao dịch (DateTimePicker).
  - Bấm "Lưu giao dịch" $\rightarrow$ gọi API tạo mới $\rightarrow$ cập nhật ngay lập tức về Trang chủ.
- [ ] **Task 4.4 (Đại - UI/FE):** Hoàn thiện Màn hình Lịch Sử (`HistoryScreen`):
  - Bộ chọn Tháng / Năm để lọc dữ liệu.
  - Danh sách giao dịch gom nhóm theo ngày (`FlatList`).
  - Thao tác vuốt/bấm để Xóa giao dịch (kèm hộp thoại xác nhận) $\rightarrow$ gọi API xóa.
- [ ] **Task 4.5 (Đại - UI/FE):** Hoàn thiện Màn hình Báo Cáo Biểu Đồ (`ReportScreen`):
  - Lấy dữ liệu từ các API báo cáo của Đạt (`monthly_summary`, `category_chart`, `income_vs_expense_chart`).
  - **Biểu đồ tròn (Pie Chart):** Thể hiện tỷ trọng % chi tiêu theo từng danh mục trong tháng bằng thư viện `react-native-chart-kit`.
  - **Biểu đồ cột (Bar Chart):** So sánh tổng Thu nhập vs Tổng Chi tiêu qua các tháng.
  - Bảng thống kê số dư ròng cuối kỳ.
- [ ] **Task 4.6 (Đạt - Integration & Review):** Hỗ trợ kiểm thử tích hợp (End-to-End Integration):
  - Rà soát luồng gọi API giữa React Native và WAMP Server, xử lý lỗi định dạng ngày tháng hoặc CORS nếu phát sinh.
  - Tối ưu hóa hiệu năng render danh sách giao dịch.
- [ ] **Task 4.7 (Đại - UI Screenshots):** Kiểm thử toàn diện trên thiết bị di động thật và máy ảo, **chụp toàn bộ ảnh chụp màn hình các chức năng** lưu vào thư mục `assets/mobile_screenshots/` để đưa vào báo cáo Word.

---

### Giai Đoạn 5: Triển Khai Server Trường, Báo Cáo Word & Đóng Gói (Tuần 5 - 6)
> **Mục tiêu:** Triển khai lên server trường (khi được phép), hoàn thiện tệp báo cáo Word `BaoCaoLTMB.docx` và sẵn sàng nộp cho Giảng viên.

- [x] **Task 5.0 (Đạt & Đại) - ĐÓNG BĂNG TRIỂN KHAI SERVER CŨ (FROZEN):**
  - **Dừng triển khai** lên server FTP `nhom46.itimit.id.vn` và phpMyAdmin `103.139.203.43` do Giảng viên cũ đã rời đi.
  - Toàn bộ dữ liệu kiểm thử được chuyển sang chạy trên Localhost WAMP & script tự động `test_api.ps1`.
  - Sẵn sàng chuyển giao cấu hình sang server mới hoặc Docker container khi Giảng viên mới yêu cầu.
- [ ] **Task 5.1 (Đạt):** Mở tệp [`BaoCaoLTMB.docx`](BaoCaoLTMB.docx) (trong `mobile-project/react-native-ver/`):
  - Tại Trang 7 (*Xây dựng mô hình DB*): Chèn ảnh sơ đồ ERD, bảng từ điển dữ liệu mô tả chi tiết 5 bảng dữ liệu MySQL.
  - Chèn ảnh chụp giao diện phpMyAdmin đã có đủ 5 bảng.
- [ ] **Task 5.2 (Đại):**
  - Tại Trang 8 (*API tương ứng các chức năng*): Chèn ảnh chụp Postman các API (Auth, Wallets, Transactions, Reports) kèm đoạn code logic PHP tiêu biểu do Đại test.
- [ ] **Task 5.3 (Đại):**
  - Tại Trang 9–10 (*Xây dựng Front-End React Native*): Chèn bộ ảnh chụp màn hình app di động thực tế (Dashboard, Thêm giao dịch, Lịch sử, Biểu đồ do Đại code) kèm bài thuyết minh chi tiết từng màn hình.
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
