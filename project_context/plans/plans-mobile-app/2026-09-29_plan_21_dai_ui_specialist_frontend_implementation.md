# 📋 KẾ HOẠCH THỰC THI (IMPLEMENTATION PLAN) - PLAN-21
## PHÂN HỆ: HOÀN THIỆN TOÀN DIỆN GIAO DIỆN FRONTEND REACT NATIVE & LINH KIỆN UI CHUẨN MÔN HỌC

> **Người thực hiện:** Trần Trọng Đại (MSSV: 1923050745 - UI Specialist & Frontend Screens)  
> **Người hỗ trợ & Duyệt:** Trịnh Thành Đạt (Lead - Architecture & Backend)  
> **Dự án:** Ứng dụng Di động Quản lý Chi tiêu Cá nhân (Personal Expense Tracker Mobile)  
> **Môn học:** Lập trình Thiết bị Di động (Mobile Application Development)  
> **Ngày lập:** 2026-09-29  
> **Trạng thái:** `IN-PROGRESS & APPROVED`

---

## 1. Mục Tiêu & Yêu Cầu Cốt Lõi

Giai đoạn hiện tại là **Giai đoạn Test và Hoàn thiện Giao diện Frontend (UI Testing & Completion)**.  
Mục tiêu là xây dựng toàn bộ các thành phần giao diện, các màn hình chức năng hoạt động mượt mà, trực quan, đáp ứng trọn vẹn yêu cầu của Giảng viên và đồ án môn học:

1. **Tuân thủ quy chuẩn Vấn đáp môn học:**
   - Sử dụng thuần túy `StyleSheet.create({...})` và bảng hằng số `COLORS`, `SIZES` trong `src/constants/theme.js`.
   - Tuyệt đối **KHÔNG sử dụng thư viện CSS bên thứ ba** (như TailwindCSS, NativeWind, Paper, Tamagui) để code trong sáng, dễ dàng giải thích bản chất với Giảng viên.
   - Sử dụng các thành phần native cốt lõi: `View`, `Text`, `TouchableOpacity`, `TextInput`, `FlatList`, `ScrollView`, `Modal`, `ActivityIndicator`.
   - Quản lý trạng thái bằng React Hooks cơ bản: `useState`, `useEffect`.
2. **Module hóa linh kiện dùng chung (`src/components/`):**
   - `WalletCard.js`: Thẻ số dư ví trực quan (Tiền mặt / Tài khoản ngân hàng).
   - `BudgetBar.js`: Thanh tiến độ hạn mức chi tiêu tháng, đổi màu động theo % (Xanh $\rightarrow$ Vàng $\rightarrow$ Đỏ).
   - `TransactionItem.js`: Thẻ dòng hiển thị giao dịch thu/chi với icon phân loại màu sắc và nút xóa.
   - `CustomButton.js`: Nút bấm thiết kế chuẩn theme ứng dụng.
3. **Hoàn thiện đầy đủ 6 màn hình chức năng:**
   - `LoginScreen.js` & `RegisterScreen.js`: Xác thực tài khoản, chuyển đổi mượt mà, có nút bypass test nhanh.
   - `HomeScreen.js`: Bảng điều khiển tài chính trung tâm (Số dư tổng hợp, danh sách ví cuộn ngang, tiến độ ngân sách, 5 giao dịch gần nhất, pull-to-refresh).
   - `AddTransactionScreen.js`: Tab chuyển đổi Thu/Chi, bàn phím nhập tiền format VND, chọn ví nguồn, lưới chọn danh mục (Grid View) có icon sinh động, chọn ngày, ghi chú.
   - `HistoryScreen.js`: Bộ lọc nhanh 3 chế độ (Tất cả / Chi / Thu), bộ lọc tháng, danh sách FlatList, xóa giao dịch có popup xác nhận và hoàn tiền ví.
   - `ReportScreen.js`: Biểu đồ tròn cơ cấu chi tiêu (`PieChart`), biểu đồ cột đối chiếu Thu - Chi 6 tháng (`BarChart`), bảng thống kê số dư ròng cuối kỳ.
4. **Cơ chế Dự phòng Thông minh (Smart Mock Data Fallback):**
   - Đảm bảo khi đang test giao diện trên máy ảo / máy thật, dù chưa kết nối được WAMP Server thì ứng dụng vẫn nạp dữ liệu mẫu phong phú của tháng 9/2026 để bạn Đại có thể lướt xem, bấm test và chụp toàn bộ ảnh màn hình phục vụ **Trang 9 – 10 của Báo cáo Word (`BaoCaoLTMB.docx`)**.

---

## 2. Lộ Trình 4 Bước Triển Khai Chi Tiết

```text
┌────────────────────────────────────────────────────────────────────────┐
│             LỘ TRÌNH HOÀN THIỆN GIAO DIỆN FRONTEND (ĐẠI)               │
├────────────────────────────────────────────────────────────────────────┤
│  [ BƯỚC 1: XÂY DỰNG BỘ LINH KIỆN UI DÙNG CHUNG (src/components/) ]     │
│  - WalletCard.js, BudgetBar.js, TransactionItem.js, CustomButton.js    │
│                                                                        │
│  [ BƯỚC 2: HOÀN THIỆN MÀN HÌNH ĐĂNG NHẬP & ĐĂNG KÝ (Auth Stack) ]      │
│  - Tạo RegisterScreen.js, cập nhật LoginScreen.js & AppNavigator.js   │
│                                                                        │
│  [ BƯỚC 3: NÂNG CẤP TOÀN DIỆN CÁC MÀN HÌNH CHÍNH (Main Tab Screens) ]  │
│  - HomeScreen.js (Ví ngang, Ngân sách, 5 giao dịch gần nhất)           │
│  - AddTransactionScreen.js (Tab Thu/Chi, Lưới danh mục có Icon, Số tiền)│
│  - HistoryScreen.js (Filter Thu/Chi/All, FlatList, Xóa giao dịch)      │
│  - ReportScreen.js (Biểu đồ Pie/Bar, Thống kê dòng tiền ròng)          │
│                                                                        │
│  [ BƯỚC 4: TÍCH HỢP MOCK FALLBACK & KIỂM THỬ GIAO DIỆN (UI Testing) ]  │
│  - Cấu hình dữ liệu mẫu an toàn, kiểm tra không lỗi cú pháp            │
│  - Sẵn sàng khởi chạy qua Expo và chụp ảnh cho báo cáo Word            │
└────────────────────────────────────────────────────────────────────────┘
```

---

## 3. Bảng Phân Nhiệm Deliverables & Tiêu Chí Nghiệm Thu

| STT | File / Thành phần | Mục đích | Tiêu chí hoàn thành |
| :---: | :--- | :--- | :--- |
| 1 | `src/components/WalletCard.js` | Thẻ hiển thị từng ví | Bo góc, hiển thị Icon ví, tên ví, số dư VND rõ nét. |
| 2 | `src/components/BudgetBar.js` | Thanh tiến độ ngân sách | Đổi màu theo % chi tiêu (Xanh $\rightarrow$ Vàng $\rightarrow$ Đỏ), hiển thị đã chi / hạn mức. |
| 3 | `src/components/TransactionItem.js` | Dòng hiển thị giao dịch | Icon tròn có màu, tên danh mục, ghi chú, ngày, số tiền xanh/đỏ, nút xóa. |
| 4 | `src/components/CustomButton.js` | Nút bấm dùng chung | Primary, Outline, bo góc chuẩn theme. |
| 5 | `src/screens/RegisterScreen.js` | Đăng ký tài khoản | Form đăng ký có kiểm tra mật khẩu, chuyển qua lại màn hình Login. |
| 6 | `src/screens/LoginScreen.js` | Đăng nhập tài khoản | Form đăng nhập, nút chuyển sang Đăng ký, nút bypass test nhanh. |
| 7 | `src/screens/HomeScreen.js` | Bảng điều khiển chính | Chào hỏi, số dư ròng, ví cuộn ngang, tiến độ ngân sách, 5 giao dịch gần nhất. |
| 8 | `src/screens/AddTransactionScreen.js`| Thêm thu / chi | Tab Thu/Chi, chọn ví, lưới danh mục có Icon (@expo/vector-icons), format tiền. |
| 9 | `src/screens/HistoryScreen.js` | Lịch sử giao dịch | Bộ lọc Thu/Chi/Tất cả, danh sách FlatList, xóa có xác nhận. |
| 10 | `src/screens/ReportScreen.js` | Biểu đồ báo cáo | Biểu đồ tròn tỷ trọng, biểu đồ cột 6 tháng, thẻ tổng kết số dư ròng. |
