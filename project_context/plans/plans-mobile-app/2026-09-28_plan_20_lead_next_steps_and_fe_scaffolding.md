# 📋 KẾ HOẠCH THỰC THI CÁC BƯỚC TIẾP THEO (IMPLEMENTATION PLAN) - PLAN-20
## PHÂN HỆ: HOÀN TẤT BACKEND REPORT API, KIỂM THỬ WAMP, ERD BÁO CÁO & KHỞI TẠO FRONTEND (FE)

> **Người thực hiện:** Trịnh Thành Đạt (MSSV: 1923050149 - Trưởng nhóm / Full-stack Lead)  
> **Dự án:** Ứng dụng Di động Quản lý Chi tiêu Cá nhân (React Native + PHP REST API + MySQL)  
> **Ngày lập:** 2026-09-28  
> **Trạng thái:** `PENDING REVIEW & APPROVAL`

---

## 1. Bối Cảnh & Đánh Giá Hiện Trạng

Cho đến thời điểm hiện tại, chúng ta đã hoàn tất vững chắc nền móng Backend:
- ✅ **CSDL & Seed Data:** Tệp DDL [`mobile-project/BE/sql/database_schema.sql`](../../../mobile-project/BE/sql/database_schema.sql) gồm 5 bảng quan hệ và dữ liệu mẫu tháng 9/2026 an toàn trên cả Local WAMP và Server trường.
- ✅ **Cầu nối WAMP 0 giây:** Đã thiết lập xong NTFS Directory Junction `D:\WampServer\www\expense_api` trỏ về `mobile-project/BE`.
- ✅ **Tài liệu cho đồng đội & Agent:** Đã có [`BE/README.md`](../../../mobile-project/BE/README.md) và script tự động [`BE/setup_wamp_junction.ps1`](../../../mobile-project/BE/setup_wamp_junction.ps1).
- ✅ **Core Backend APIs đã có:**
  - `BE/config/database.php` (Đa môi trường)
  - `BE/config/helpers.php` (CORS + JSON response chuẩn)
  - `BE/api/auth/register.php` & `BE/api/auth/login.php`
  - `BE/api/reports/monthly_summary.php` & `BE/api/reports/category_chart.php`

---

## 2. Lộ Trình 4 Giai Đoạn Tiếp Theo Của Trưởng Nhóm

```text
┌────────────────────────────────────────────────────────────────────────┐
│                   LỘ TRÌNH THỰC THI TIẾP THEO (LEAD)                   │
├────────────────────────────────────────────────────────────────────────┤
│  [ GIAI ĐOẠN 1: HOÀN TẤT 100% BACKEND REPORT & BOILERPLATE CHO ĐẠI ]   │
│  - Viết API reports/income_vs_expense_chart.php (Bar Chart thu - chi)  │
│  - Tạo khung sẵn cho các API CRUD của Đại (wallets, transactions)      │
│                                                                        │
│  [ GIAI ĐOẠN 2: KIỂM THỬ TOÀN DIỆN CSDL & API TRÊN WAMP LOCAL ]        │
│  - Hướng dẫn nhập database_schema.sql vào phpMyAdmin localhost         │
│  - Chạy script kiểm thử tự động (PowerShell/curl) kiểm tra mã 200 OK   │
│                                                                        │
│  [ GIAI ĐOẠN 3: XUẤT BẢN SƠ ĐỒ ERD & TỪ ĐIỂN CSDL CHO BÁO CÁO WORD ]   │
│  - Sơ đồ quan hệ thực thể ERD chuẩn Mermaid cho Trang 7 BaoCaoLTMB     │
│  - Bảng từ điển dữ liệu (Data Dictionary) của 5 bảng                   │
│                                                                        │
│  [ GIAI ĐOẠN 4: KHỞI TẠO BỘ KHUNG CLIENT REACT NATIVE (FE) ]           │
│  - Thiết lập thư mục FE/src/ (api, components, screens, navigation)    │
│  - Cấu hình Axios / API Base URL đa môi trường (src/api/config.js)     │
│  - Dựng khung điều hướng BottomTab: Home, Transactions, Reports, Wallet│
│  - Hiện thực màn hình chính HomeScreen & Báo cáo ReportScreen          │
└────────────────────────────────────────────────────────────────────────┘
```

---

## 3. Chi Tiết Từng Hạng Mục Công Việc

### 🔹 Giai đoạn 1: Hoàn Tất 100% Backend Core & Tạo Khuôn Mẫu (Boilerplate)
1. **Hoàn thiện API Biểu đồ cột Thu - Chi:**
   - Tạo file `mobile-project/BE/api/reports/income_vs_expense_chart.php`.
   - Truy vấn gom nhóm `SUM(amount)` theo từng tháng trong 6 tháng gần nhất, tách riêng `type = 'INCOME'` và `type = 'EXPENSE'`.
   - Trả về cấu trúc JSON sẵn sàng cắm thẳng vào thư viện `react-native-chart-kit` BarChart:
     ```json
     {
       "labels": ["T5", "T6", "T7", "T8", "T9", "T10"],
       "datasets": [
         { "data": [12000000, 15000000, ...], "color": "green", "legend": "Thu nhập" },
         { "data": [8500000, 9200000, ...], "color": "red", "legend": "Chi tiêu" }
       ]
     }
     ```
2. **Tạo Boilerplate / File khung cho bạn Đại:**
   - Khởi tạo sẵn các file rỗng kèm chú thích tham số trong `mobile-project/BE/api/wallets/list.php`, `create.php`, `transactions/list.php`, `create.php`, `delete.php`, `categories/list.php` để bạn Đại chỉ việc viết câu lệnh SQL mà không lo sai lệch định dạng JSON.

---

### 🔹 Giai đoạn 2: Kiểm Thử Toàn Diện API & CSDL Trên WampServer
1. **Import CSDL:**
   - Mở WampServer, truy cập `http://localhost/phpmyadmin`.
   - Tạo CSDL `sql_nhom46_itimi` (Collation `utf8mb4_unicode_ci`).
   - Copy nội dung `database_schema.sql` dán vào tab SQL và chạy.
2. **Chạy Script kiểm thử tự động (Smoke Test API):**
   - Viết một script kiểm thử tự động `test_api.ps1` gửi HTTP requests trực tiếp tới WAMP:
     - `POST /expense_api/api/auth/login.php` -> Kiểm tra mã 200, lấy JWT/Token.
     - `GET /expense_api/api/reports/monthly_summary.php?user_id=1&month=2026-09` -> Kiểm tra tổng thu, tổng chi.
     - `GET /expense_api/api/reports/category_chart.php?user_id=1&month=2026-09` -> Kiểm tra dữ liệu biểu đồ tròn.
     - `GET /expense_api/api/reports/income_vs_expense_chart.php?user_id=1` -> Kiểm tra dữ liệu biểu đồ cột.

---

### 🔹 Giai đoạn 3: Xuất Bản Sơ Đồ ERD & Từ Điển Dữ Liệu Cho Báo Cáo Word
1. **Sơ đồ ERD (Entity Relationship Diagram):**
   - Vẽ sơ đồ quan hệ 5 thực thể bằng Mermaid với đầy đủ Cardinality (1-N, N-1), Primary Key, Foreign Key.
   - Sẵn sàng xuất ảnh chèn vào **Trang 7** của file Word `BaoCaoLTMB.docx`.
2. **Bảng từ điển dữ liệu (Data Dictionary):**
   - Liệt kê kiểu dữ liệu, ràng buộc (Null/Not Null), mô tả ý nghĩa nghiệp vụ của từng trường trong 5 bảng.

---

### 🔹 Giai đoạn 4: Khởi Tạo Bộ Khung Ứng Dụng React Native (Setup FE Scaffolding)
1. **Khởi tạo kiến trúc dự án `mobile-project/FE/`:**
   - Cài đặt các gói thư viện cốt lõi:
     - React Navigation (`@react-navigation/native`, `@react-navigation/bottom-tabs`, `@react-navigation/native-stack`).
     - Safe Area & Screens (`react-native-screens`, `react-native-safe-area-context`).
     - Network & Charts (`axios`, `react-native-chart-kit`, `react-native-svg`).
   - Thiết lập cấu trúc thư mục chuẩn sẵn sàng bàn giao cho bạn Đại:
   ```text
   mobile-project/FE/
   ├── package.json
   ├── App.js                   # Điểm khởi chạy ứng dụng
   └── src/
       ├── api/
       │   ├── config.js        # Cấu hình BASE_URL đa môi trường (WAMP_EMULATOR, WAMP_DEVICE, SERVER)
       │   └── client.js        # Axios instance tự động đính kèm Token xác thực
       ├── navigation/
       │   ├── AppNavigator.js  # Root Stack điều hướng (Auth Stack + Main Tab)
       │   └── TabNavigator.js  # Bottom Tab Bar 4 màn hình khung (Shell)
       ├── components/          # Thư mục chứa linh kiện UI (do bạn Đại triển khai)
       └── screens/             # Thư mục chứa các màn hình chức năng (do bạn Đại triển khai)
   ```
2. **Bàn giao nhiệm vụ UI/FE cho bạn Đại:**
   - Đạt bàn giao khung điều hướng chạy mượt và file cấu hình API đã test thông suốt với WAMP Server.
   - Bạn Đại tiến hành vẽ linh kiện UI (`BalanceCard.js`, `BudgetBar.js`, `TransactionItem.js`,...) và hoàn thiện các màn hình (`LoginScreen`, `RegisterScreen`, `HomeScreen`, `AddTransactionScreen`, `HistoryScreen`, `ReportScreen`).
   - ⚠️ **Nguyên tắc viết code phục vụ thi Vấn đáp:** 
     - Tuyệt đối **KHÔNG dùng thư viện CSS / UI bên ngoài** (như Tailwind/NativeWind, Paper, Styled-Components).
     - Định kiểu giao diện hoàn toàn bằng **`StyleSheet.create({...})` thuần** kết hợp các hằng số màu sắc `const COLORS`.
     - Tận dụng tối đa các component cơ bản (`View`, `Text`, `TouchableOpacity`, `TextInput`, `FlatList`) và hooks cơ bản (`useState`, `useEffect`) để code mộc mạc, trong sáng, dễ dàng giải thích cho Giảng viên khi thi vấn đáp.

---

## 4. Bảng Tiêu Chí Nghiệm Thu Của Lead (Acceptance Criteria)

| Hạng mục | Điều kiện hoàn thành | Người phụ trách | Trạng thái thực tế |
| :--- | :--- | :--- | :---: |
| **API Thu - Chi Cột** | Endpoint trả về JSON 6 tháng đúng định dạng cho `react-native-chart-kit` BarChart. | Trịnh Thành Đạt | **HOÀN THÀNH (`reports/income_vs_expense_chart.php`)** |
| **Boilerplate CRUD** | Bộ 6 API CRUD chuẩn RESTful có chú thích cho Đại (`wallets/*`, `transactions/*`, `categories/*`). | Trịnh Thành Đạt | **HOÀN THÀNH (Đã sẵn sàng trong `BE/api/`)** |
| **Kiểm thử WAMP** | Script kiểm thử tự động toàn bộ API trên WampServer localhost. | Trịnh Thành Đạt | **HOÀN THÀNH (`BE/test_api.ps1`)** |
| **ERD Báo cáo** | Sơ đồ ERD 5 bảng chuẩn Mermaid & Data Dictionary sẵn sàng cho Trang 7 Word. | Trịnh Thành Đạt | **HOÀN THÀNH (`BE/sql/ERD_AND_DATA_DICTIONARY.md`)** |
| **Frontend Shell (Setup FE)** | App React Native có đủ cấu trúc thư mục, cấu hình API, theme const và điều hướng sẵn sàng cho Đại làm UI. | Trịnh Thành Đạt | **HOÀN THÀNH (`mobile-project/FE/`)** |
| **Hiện thực UI & Màn hình** | Toàn bộ linh kiện UI, màn hình chức năng, form nhập liệu và kiểm thử trên máy thật. | Trần Trọng Đại | *Đang chờ bàn giao cho Đại* |
