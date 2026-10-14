# 📋 KẾ HOẠCH ĐỒNG BỘ GIT & LỘ TRÌNH THỰC HIỆN CÔNG VIỆC (PLAN-22)
## DÀNH CHO: TRẦN TRỌNG ĐẠI (MSSV: 1923050745 - UI SPECIALIST & FRONTEND SCREENS)

> **Dự án:** Ứng dụng Di động Quản lý Chi tiêu Cá nhân (Personal Expense Tracker Mobile)  
> **Môn học:** Lập trình Thiết bị Di động (Mobile Application Development)  
> **Thành viên:** Trịnh Thành Đạt (Lead - Architecture & Backend) & Trần Trọng Đại (UI/FE & Word Report)  
> **Ngày lập:** 2026-10-07  
> **Trạng thái:** `PENDING APPROVAL & READY FOR SYNC`

---

## 1. Trả Lời Trực Tiếp 2 Câu Hỏi Của Bạn

### 1.1. Phần mới trên GitHub đã được kéo về máy bạn chưa?
* **TRẢ LỜI:** **CHƯA ĐƯỢC KÉO VỀ!**
* **Hiện trạng Git:**
  * Nhánh `main` trên máy bạn hiện đang **chậm hơn `origin/main` 4 commit** (do bạn Đạt `Datgh37` vừa hoàn thiện và đẩy lên từ ngày 02/10 đến trưa 07/10/2026).
  * 4 commit đang chờ kéo về gồm:
    1. `fe0dcc7` (02/10): Chuẩn hóa báo cáo học thuật KLTN Chapter 1.
    2. `3f52aa9` (02/10): **[RẤT QUAN TRỌNG]** Nâng cấp toàn diện Backend APIs và **tái cấu trúc thư mục Frontend** (chuyển các màn hình từ `src/screens/*.js` vào `src/screens/auth/` và `src/screens/main/`, bổ sung `AuthContext`, `endpoints.js`, `BudgetScreen`, `ProfileScreen`...).
    3. `ce7508f` (07/10): Tích hợp tài liệu Agent ECC.
    4. `7c61432` (12:55 trưa 07/10): Bổ sung hạ tầng Realtime WebSocket Socket.IO Server (`realtime-server/`, `RealtimeAlertBanner.js`, `RealtimeContext.js`) tự động nhảy số dư tức thì 0ms.

> [!CAUTION]
> **CẢNH BÁO NGUY CƠ XUNG ĐỘT (MERGE CONFLICT):**  
> Tuyệt đối **KHÔNG gõ `git pull` ngay lúc này!**  
> Lý do: Ở máy bạn hiện đang có rất nhiều code giao diện bạn vừa tự code trong `src/screens/` (`HomeScreen.js`, `AddTransactionScreen.js`, `HistoryScreen.js`, `ReportScreen.js`...) và `src/components/`.  
> Trong khi đó, commit `3f52aa9` trên remote đã xóa các file ở cấp `src/screens/` để đưa vào `src/screens/main/`. Nếu bạn `git pull` thẳng, Git sẽ báo lỗi chặn đứng hoặc làm mất các đoạn code UI bạn đã viết!

---

### 1.2. Tổng hợp toàn bộ phần việc bạn (Trần Trọng Đại) phải làm

Căn cứ theo tài liệu phân công [`mobile-project/ROADMAP.md`](../../../mobile-project/ROADMAP.md), [`2026-09-29_plan_21_dai_ui_specialist_frontend_implementation.md`](2026-09-29_plan_21_dai_ui_specialist_frontend_implementation.md) và các template code Đạt chuẩn bị sẵn:

Vai trò chính của bạn: **UI Specialist (Thiết kế & Hoàn thiện Giao diện App)**, **Kiểm thử API WAMP/Postman** và **Chụp ảnh viết nội dung Trang 8–10 Báo cáo Word**.

#### A. Nhiệm vụ Frontend React Native (Trọng tâm lớn nhất):
1. **Linh kiện UI dùng chung (`src/components/`):**
   * Giữ nguyên và bảo toàn 4 linh kiện bạn đã viết: `WalletCard.js`, `BudgetBar.js`, `TransactionItem.js`, `CustomButton.js`.
   * Tương thích với linh kiện mới của Đạt: `RealtimeAlertBanner.js`.
2. **Giao diện các Màn hình Xác thực (`src/screens/auth/`):**
   * `LoginScreen.js`: Form đăng nhập, nút chuyển qua Đăng ký, nút "Quên mật khẩu", nút test bypass nhanh.
   * `RegisterScreen.js`: Form đăng ký có kiểm tra trùng khớp mật khẩu.
   * `ForgotPasswordScreen.js`: Giao diện nhập email nhận OTP và đặt lại mật khẩu.
3. **Giao diện các Màn hình Chính (`src/screens/main/`):**
   * `HomeScreen.js` (Tổng quan): Thẻ số dư ví vuốt ngang (`WalletCard`), thanh tiến độ ngân sách (`BudgetBar`), danh sách 5 giao dịch gần nhất (`TransactionItem`), nút Xem tất cả và Banner WebSocket. *(Đạt đã viết sẵn 100% logic nạp API & Socket, chỉ cần bạn đưa JSX & Style vào)*.
   * `AddTransactionScreen.js` (Thêm/Sửa giao dịch): Tab Thu/Chi, bàn phím format tiền tệ VND, chọn ví nguồn, lưới chọn Icon danh mục sinh động (@expo/vector-icons), chọn ngày.
   * `TransactionsScreen.js` (Lịch sử giao dịch): Bộ lọc 3 chế độ (Tất cả / Chi / Thu), bộ lọc tháng, danh sách cuộn `FlatList`, nút xóa giao dịch có popup xác nhận.
   * `BudgetScreen.js` (Ngân sách): Thẻ hiển thị hạn mức tháng, thanh tiến độ % trực quan, modal thiết lập hạn mức chi tiêu mới.
   * `ReportScreen.js` (Báo cáo & Biểu đồ): Biểu đồ tròn tỷ trọng danh mục (`PieChart`), biểu đồ cột Thu vs Chi 6 tháng (`BarChart` dùng `react-native-chart-kit`), bảng tóm tắt số dư ròng.
   * `ProfileScreen.js` (Hồ sơ): Hiển thị tên, email, ngày tạo tài khoản, nút đổi mật khẩu, nút Đăng xuất.

#### B. Nhiệm vụ Kiểm thử & Minh chứng (Testing & Evidence):
1. **Kiểm thử API trên WAMP Local:**
   * Chạy script `mobile-project/BE/test_api.ps1` hoặc dùng Postman kiểm thử các endpoints.
   * Chụp ảnh màn hình kết quả trả về JSON lưu lại để chuẩn bị chèn vào báo cáo.
2. **Chạy thử App trên Expo & Chụp ảnh Mobile Screenshots:**
   * Khởi động app bằng `npm start` trong `mobile-project/FE/`.
   * Mở Expo Go trên điện thoại thật hoặc máy ảo Android Emulator.
   * Chụp đủ ảnh từng màn hình (Home, Add, History, Budget, Report, Profile, Login) lưu vào `assets/mobile_screenshots/`.

#### C. Nhiệm vụ Báo cáo Word (`mobile-project/BaoCaoLTMB.docx`):
* Đạt chịu trách nhiệm: Trang 7 (Sơ đồ ERD, Từ điển dữ liệu DB) và cập nhật phím `F9` (Mục lục TOC/TOF/TOT).
* **Đại chịu trách nhiệm viết nội dung:**
  * **Trang 8:** Bảng danh sách API và ảnh chụp JSON response minh chứng API hoạt động.
  * **Trang 9 & 10:** Chèn bộ ảnh chụp màn hình các chức năng ứng dụng thực tế và bài thuyết minh giải thích từng màn hình.
