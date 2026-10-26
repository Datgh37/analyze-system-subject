# KẾ HOẠCH 52: NÂNG CẤP XÁC THỰC ĐA PHƯƠNG THỨC (OTP/GOOGLE), PUSH NOTIFICATION NGOẠI TUYẾN NGÂN HÀNG & TRỢ LÝ VOICE AI
*(Kế hoạch chen giữa Giai đoạn 11 - Phục vụ Đáp ứng Trọn vẹn Barem Chấm điểm & Tính năng Sáng tạo Đồ án Di động)*

---

## 1. BỐI CẢNH VÀ MỤC TIÊU CHIẾN LƯỢC

### 1.1 Bối Cảnh
- Trong khuôn khổ Giai đoạn 11 ([`2026-10-26_plan_50_dieu_chinh_toi_uu_va_khac_phuc_logic_backend_toan_dien.md`](2026-10-26_plan_50_dieu_chinh_toi_uu_va_khac_phuc_logic_backend_toan_dien.md)), nhóm đã hoàn thành xuất sắc Chặng 1: Chuẩn hóa 100% tài liệu báo cáo cuối kỳ theo quy cách học thuật DNTU (`BAO_CAO_CUOI_KY_DOTNET_MAUI_CHUAN_DNTU.docx`).
- Qua rà soát chi tiết văn bản yêu cầu chính thức của Giảng viên ([`yeucau-cuoiky.txt`](../../../mobile-project/dotnet-maui-ver/yeucau-cuoiky.txt)) và Biên bản hội đàm Giai đoạn 10 ([`sessions/session_giai_doan_10_ai_comprehensive_audit/01_toan_van_hoi_thoai_qa_phien_hop_giai_doan_10.md`](../../../sessions/session_giai_doan_10_ai_comprehensive_audit/01_toan_van_hoi_thoai_qa_phien_hop_giai_doan_10.md)), Hội đồng và Trưởng nhóm thống nhất:
  - **TẠM THỜI ĐÓNG BĂNG (FREEZE)** các chặng tiếp theo của Kế hoạch 50 (Chặng 2, 3, 4, 5 bao gồm cả việc đóng gói APK).
  - **MỞ KẾ HOẠCH CHEN GIỮA (KẾ HOẠCH 52)** nhằm bổ sung và giải quyết dứt điểm các tính năng quan trọng về Xác thực nâng cao, Push Notification ngoại tuyến khi tắt app, Bắt biến động số dư ngân hàng và Trợ lý giọng nói AI.

### 1.2 Mục Tiêu
1. **Hoàn thiện Xác thực Đa phương thức:** Kích hoạt tài khoản bằng mã OTP qua Email/SĐT (giao diện `VerifyOtpPage.xaml`), Đăng nhập nhanh bằng Google OAuth 2.0 (`WebAuthenticator`), Quên mật khẩu qua OTP, và Tự động đăng nhập lại (Auto-login session).
2. **Xử lý Triệt để Push Notification Ngoại tuyến (Offline & Killed App):** Bổ sung quyền Android 13+, tích hợp Native Android `NotificationManager`, chuyển bộ hẹn giờ nhắc nhở chi tiêu cuối ngày 21:00 sang `AlarmManager` để **dù ứng dụng đã bị vuốt tắt hoàn toàn (Kill App)**, thiết bị vẫn nổ chuông và hiện thông báo.
3. **Kích hoạt Tính năng Bắt Biến Động Số Dư Ngân Hàng (`BankNotificationListenerService`):** Lắng nghe thông báo từ **ACB ONE, MoMo, ZaloPay, Vietcombank, MBBank**, bóc tách số tiền trừ và bắn Native Notification. Chạm vào thông báo sẽ mở App và nhảy thẳng vào `AddTransactionPage` điền sẵn số tiền.
4. **Nối Toàn Diện Trợ Lý Voice AI & SignalR 2 Chiều:** Điều khiển hệ thống bằng giọng nói tiếng Việt qua Gemini NLP Intent Router, và bổ sung 2 phương thức Client `InvokeAsync` lên `FinancialHub` để ăn trọn điểm Phương án B2.

---

## 2. BẢNG MA TRẬN HỢP NHẤT TOÀN DIỆN (10 ĐẦU VIỆC BẮT BUỘC)

| STT | Phân loại | Hạng mục công việc chi tiết | Chặng thực thi |
| :---: | :--- | :--- | :---: |
| **1** | Người dùng yêu cầu | **Xác thực OTP Email/SĐT:** Đăng ký nhận OTP 6 số, giao diện `VerifyOtpPage.xaml` có đồng hồ đếm ngược 60s, Quên mật khẩu qua OTP. | **Chặng 1** |
| **2** | Người dùng yêu cầu | **Đăng nhập Google OAuth:** Nút "Tiếp tục với Google" trên `LoginPage.xaml`, xác thực qua `WebAuthenticator` & API Backend. | **Chặng 1** |
| **3** | Barem kỹ thuật | **Tự động Đăng nhập (Auto-Login):** Kiểm tra JWT token trong `SecureStorage` khi khởi động app, nếu còn hạn thì vào thẳng `//WalletsPage`. | **Chặng 1** |
| **4** | Barem kỹ thuật | **Quyền Android 13+ & Push Token:** Khai báo `POST_NOTIFICATIONS`, `SCHEDULE_EXACT_ALARM`, `WAKE_LOCK`, `VIBRATE` và gọi API đăng ký Push Token. | **Chặng 2** |
| **5** | Barem kỹ thuật | **Thông báo định kỳ khi ĐÃ TẮT ỨNG DỤNG:** Chuyển bộ hẹn giờ nhắc chi tiêu 21:00 từ RAM Timer sang Native Android `AlarmManager`. | **Chặng 2** |
| **6** | Người dùng yêu cầu | **Bắt biến động số dư Ngân hàng Offline:** Nâng cấp `BankNotificationListenerService` lắng nghe app ACB, MoMo, VCB, MBBank... bắn Native Notification tức thì khi có biến động tiền. | **Chặng 3** |
| **7** | Người dùng yêu cầu | **Chạm thông báo mở App (Deep Link):** Cấu hình `PendingIntent` chạm vào thông báo ngân hàng $\rightarrow$ Mở app $\rightarrow$ Nhảy vào `AddTransactionPage` tự điền tiền và ví. | **Chặng 3** |
| **8** | Người dùng yêu cầu | **Trợ lý Voice AI điều khiển hệ thống:** Kết nối `VoiceMicModalPage` $\rightarrow$ Gemini NLP `VoiceIntentRouterService` $\rightarrow$ Tự động điều hướng và thao tác bằng giọng nói. | **Chặng 4** |
| **9** | Barem kỹ thuật | **SignalR 2 chiều (Phương án B2):** Bổ sung 2 phương thức Client gọi `InvokeAsync` lên Server (`RequestLiveBalanceAsync` và `BroadcastWalletRefreshAsync`). | **Chặng 4** |
| **10** | Barem kỹ thuật | **Bảo toàn Chất lượng & Kiểm thử:** Chạy toàn bộ 42 Unit Tests ViewModel + 183 Backend Tests đảm bảo 100% Pass. | **Chặng 4** |

---

## 3. LỘ TRÌNH 4 CHẶNG TRIỂN KHAI CHI TIẾT

### 🟢 Chặng 1: Nâng Cấp Hệ Thống Xác Thực Toàn Diện (Auth Multi-Method & Auto-Login)
- **Backend (ASP.NET Core 10 Web API):**
  - Tạo entity `UserOtpCode` (`Id`, `UserId`, `Email`, `PhoneNumber`, `OtpCode`, `Purpose`, `ExpiredAt`, `IsUsed`).
  - Triển khai `IEmailService` gửi OTP qua SMTP kết hợp `MockEmailSender` hiển thị OTP ra log/toast để test nhanh lúc không có internet.
  - Viết các API Endpoints trong `AuthController.cs`:
    + `POST /api/auth/verify-otp`: Xác thực mã OTP 6 số và kích hoạt tài khoản (`IsEmailVerified = true`).
    + `POST /api/auth/resend-otp`: Gửi lại mã OTP mới.
    + `POST /api/auth/forgot-password`: Yêu cầu mã OTP khôi phục mật khẩu.
    + `POST /api/auth/reset-password-otp`: Đặt lại mật khẩu mới kèm mã OTP.
    + `POST /api/auth/google`: Tiếp nhận Google Token, xác thực và cấp JWT Token hệ thống.
- **Frontend (.NET MAUI 10):**
  - Xây dựng màn hình `VerifyOtpPage.xaml` và ViewModel `VerifyOtpViewModel.cs` (6 ô nhập OTP, timer đếm ngược 60 giây, tự động focus ô tiếp theo).
  - Cập nhật luồng Đăng ký trên `RegisterViewModel.cs`: Khi đăng ký thành công $\rightarrow$ Điều hướng sang `VerifyOtpPage?email=...`.
  - Bổ sung nút đăng nhập Google chuẩn thương hiệu trên `LoginPage.xaml` kết nối qua `WebAuthenticator`.
  - Cập nhật `AppShell.xaml.cs`: Khi khởi động, kiểm tra phiên làm việc qua `IClientAuthService.InitializeAsync()`. Nếu hợp lệ $\rightarrow$ Tự động chuyển hướng vào `//WalletsPage`.

### 🟢 Chặng 2: Thiết Lập Hạ Tầng Native Android Notification & Lập Lịch Khi Tắt App
- Cập nhật `AndroidManifest.xml` bổ sung các quyền bắt buộc:
  ```xml
  <uses-permission android:name="android.permission.POST_NOTIFICATIONS" />
  <uses-permission android:name="android.permission.SCHEDULE_EXACT_ALARM" />
  <uses-permission android:name="android.permission.WAKE_LOCK" />
  <uses-permission android:name="android.permission.VIBRATE" />
  ```
- Xây dựng Native Android Notification Helper trong `Platforms/Android/`:
  - Khởi tạo Notification Channel chuẩn Android (`financial_alerts_channel`, mức ưu tiên cao `IMPORTANCE_HIGH`).
  - Sử dụng `NotificationCompat.Builder` hỗ trợ hiển thị Heads-up Banner, âm thanh và rung.
- Tích hợp Android `AlarmManager` + `BroadcastReceiver`: Chuyển tính năng nhắc nhở chi tiêu 21:00 sang `AlarmManager` để **dù người dùng đã vuốt tắt ứng dụng hoàn toàn (Kill App)**, đến giờ hệ thống vẫn tự nổ thông báo.
- Client tự động gọi `RegisterPushTokenAsync` gửi Device Token lên Server sau khi đăng nhập thành công.

### 🟢 Chặng 3: Hoàn Thiện Bắt Biến Động Số Dư Ngân Hàng Ngoại Tuyến (Bank Listener Killer Feature)
- Nâng cấp `Platforms/Android/Services/BankNotificationListenerService.cs`:
  - Khi bắt được thông báo biến động số dư từ ACB, MoMo, ZaloPay, Vietcombank, MBBank: Bóc tách số tiền và lập tức kích hoạt Notification Helper bắn một Native Notification lên màn hình với tiêu đề *"💳 Phát hiện chi tiêu mới từ [Ngân hàng]"*.
- Cấu hình Deep Link (`PendingIntent`): Chạm vào thông báo $\rightarrow$ Khởi động App $\rightarrow$ Chuyển thẳng đến `AddTransactionPage`, tự động điền sẵn số tiền trừ và tên ngân hàng, người dùng chỉ cần chọn danh mục và bấm Lưu.

### 🟢 Chặng 4: Trợ Lý Voice AI Điều Khiển Hệ Thống & Hoàn Thiện SignalR 2 Chiều
- **Trợ lý Voice AI:**
  - Nối luồng từ Micro nổi (`VoiceMicModalPage.xaml`) $\rightarrow$ Chuyển text tiếng Việt vào `VoiceIntentRouterService.cs`.
  - Tự động điều hướng giao diện (Ví dụ nói: *"Thêm 50k ăn trưa"* $\rightarrow$ Mở `AddTransactionPage` điền sẵn dữ liệu; nói: *"Tạo danh mục Nuôi mèo"* $\rightarrow$ Mở `CreateCategoryPage`).
- **SignalR 2 Chiều (Phương án B2):**
  - Trong `FinancialHub.cs` và `SignalRRealtimeService.cs`: Bổ sung 2 phương thức Client gọi `InvokeAsync` lên Server (`RequestLiveBalanceAsync` và `BroadcastWalletRefreshAsync`).
- **Kiểm thử Toàn diện:** Chạy toàn bộ 42 Unit Tests ViewModel và 183 Backend Tests, đảm bảo hệ thống đạt 100% Green.

---

## 4. TIÊU CHÍ HOÀN THÀNH (DEFINITION OF DONE)
1. Đăng ký tài khoản nhận được mã OTP kích hoạt và xác thực thành công trên giao diện `VerifyOtpPage.xaml`.
2. Đăng nhập bằng tài khoản Google hoạt động trơn tru.
3. Tắt ứng dụng hoàn toàn (Kill App), đến giờ hẹn nhắc nhở chi tiêu thông báo vẫn nổ chuông và hiện banner trên thanh Status Bar của Android.
4. Giả lập thông báo từ ngân hàng (ACB/MoMo/VCB) $\rightarrow$ App tự động bắn Local Notification $\rightarrow$ Bấm vào thông báo mở thẳng màn hình ghi chép chi tiêu với số tiền đã điền sẵn.
5. Trợ lý giọng nói nhận lệnh tiếng Việt và tự động điều hướng đúng trang.
6. Đạt chuẩn SignalR 2 chiều (Client Invoke -> Server phản hồi).
7. 100% Backend Tests (183 tests) và Frontend Tests (42 tests) đều PASS.
