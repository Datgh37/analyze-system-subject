# KẾ HOẠCH TRIỂN KHAI CHI TIẾT (PLAN-54)
## GIAI ĐOẠN 11 — CHẶNG 1: KIỂM TOÁN, RÀ SOÁT & NÂNG CẤP TOÀN DIỆN BÁO CÁO CUỐI KỲ (BaoCao.docx)

- **Dự án:** Hệ thống Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh (Tích hợp AI) - .NET MAUI 10 & ASP.NET Core 10 Web API.
- **Thời gian lập:** 2026-10-27.
- **Mục tiêu chặng:** 
  1. Thống nhất điều chỉnh Giai đoạn 11: Tạm thời **bỏ qua bước quay Video Demo** theo chỉ đạo người dùng.
  2. Rà soát, kiểm toán dòng-đối-dòng nội dung file báo cáo [`BaoCao.docx`](../../mobile-project/dotnet-maui-ver/BaoCao.docx), đối chiếu nghiêm ngặt với barem điểm 10 môn Lập trình Mobile ([`yeucau-cuoiky.txt`](../../mobile-project/dotnet-maui-ver/yeucau-cuoiky.txt)).
  3. Cập nhật, nâng cấp toàn diện các thiếu sót trong `BaoCao.docx` (Native Android Push Notification Engine, Killed State AlarmManager, SignalR 2 chiều Full-Duplex, 363 Unit Tests Clean MVVM, 22 thực thể CSDL 3NF, Bottom Navigation Dock + Voice Mic FAB, 6 kịch bản kiểm thử trực quan cho Giảng viên).
  4. Nghiệm thu, chốt Chặng 1 của Giai đoạn 11 và đồng bộ lên `MASTER_PROJECT_ROADMAP.md` cùng `decisions_and_changes_log.md`.

---

### 1. BẢNG ĐỐI CHIẾU TIÊU CHÍ BAREM ĐIỂM VÀ NỘI DUNG NÂNG CẤP

| Tiêu chí Barem (`yeucau-cuoiky.txt`) | Mức điểm | Hiện trạng trong `BaoCao.docx` cũ | Nội dung Nâng cấp Chuẩn mực trong `BaoCao.docx` mới |
| :--- | :--- | :--- | :--- |
| **Phân công & Phương án chọn** | Bắt buộc | Mới ghi chung chung vai trò của 2 thành viên | **Ghi rõ đăng ký:** Nhóm chọn **Phương án B3 – Kiến trúc MVVM (2.0đ)**, đồng thời hiện thực hóa thêm **Phương án B2 – Realtime SignalR (+1.0đ điểm cộng)** để đạt điểm tuyệt đối 10/10. |
| **Phần A – Push Notification** | **2,0 điểm (Bắt buộc)** | Mới mô tả sơ lược `LocalNotificationService` | **Thuyết minh Native Android Notification Engine:**<br>- 3 Notification Channels chuẩn Android 8.0+.<br>- Xử lý 3 trạng thái: Foreground (Heads-Up Banner), Background, và **KILLED STATE (App tắt hoàn toàn)** qua `AlarmManager` + `BroadcastReceiver` (`DailyReminderReceiver` nhắc 21:00 hàng ngày, `BootReceiver` tự khôi phục khi reboot).<br>- **Deep Linking Navigation:** Chạm vào thông báo tự điều hướng đến đúng trang và điền form qua `IQueryAttributable`.<br>- **Bank Notification Listener:** Bóc tách thông báo MoMo, ACB, VCB, MBBank.<br>- Màn hình `NotificationHistoryPage.xaml`. |
| **Phần B2 – Realtime SignalR** | **+1,0 điểm (Điểm cộng)** | Mới mô tả chiều Server gửi Client | **Thuyết minh SignalR 2 chiều Full-Duplex:**<br>- Endpoint `/hubs/financial`, bảo vệ bằng JWT `[Authorize]`.<br>- Nhóm người dùng `user_{userId}`.<br>- Hai chiều tương tác: Server gửi `ReceiveBalanceUpdate`/`ReceiveBudgetAlert`; Client invoke `RequestLiveBalanceAsync` và `BroadcastWalletRefreshAsync`.<br>- Tự phục hồi `WithAutomaticReconnect`, gỡ handler khi rời màn hình. |
| **Phần B3 – Kiến trúc Clean MVVM & Unit Testing** | **2,0 điểm** | Mô tả lý thuyết MVVM, chưa có số liệu kiểm thử | **Bổ sung Sơ đồ luồng & Kết quả kiểm thử thực tế:**<br>- Tách 3 tầng Model (Interfaces Repositories hoán đổi Api/Mock 0ms) - ViewModel (CommunityToolkit.Mvvm, Zero Code-Behind) - View (20 XAML Pages).<br>- **Báo cáo Kiểm thử Toàn diện: 82 bài Unit Test Frontend MAUI** (xUnit + Moq) + **281 bài Unit Test Backend**, tổng cộng **363/363 Tests Passed 100%**.<br>- Sơ đồ luồng dữ liệu 1 chức năng mẫu: Ghi nhận chi tiêu kèm bóc tách thông báo ngân hàng. |
| **Cơ sở Dữ liệu Quan hệ 3NF (Chương 1)** | Tiêu chuẩn | Bảng 1.1 mới liệt kê 13 bảng cơ bản | **Cập nhật Bảng 1.1 thành 22 thực thể CSDL PostgreSQL 16 3NF hoàn chỉnh** (bổ sung `user_otp_codes`, `auto_categorization_rules`, `cooling_off_transactions`, `financial_nudges`, `salary_slip_details`, `receipt_items`, `device_push_tokens`, `in_app_notifications`, `autonomous_actions`...). |
| **Danh mục Màn hình Ứng dụng (Chương 2)** | Tiêu chuẩn | Bảng 2.1 ghi nhận cấu trúc Wireframe cũ | **Cập nhật Bảng 2.1:** Bổ sung hệ thống **Bottom Navigation Dock** 4 tab chính, nút trung tâm **Center Voice FAB Microphone**, Sidebar 5 nhóm chuyên đề và trang Lịch sử Thông báo. |
| **Kịch bản Kiểm thử cho Giảng viên (Chương 3)** | **1,5 điểm (Vấn đáp/Demo)** | 3 bước kiểm thử console thô sơ | **Nâng cấp thành 6 Kịch bản Kiểm thử Trực quan trên Máy ảo:**<br>1. Khởi chạy 1-Click bằng `run_android.ps1`.<br>2. Đăng nhập & Nạp dữ liệu mẫu sinh động.<br>3. Trợ lý Giọng nói Voice AI (Center FAB).<br>4. Giả lập Biến động Ngân hàng ngoại tuyến (MoMo/ACB 1-chạm).<br>5. Kiểm thử Đồng bộ Realtime SignalR 2 chiều (2 cửa sổ song song).<br>6. Kiểm thử Hẹn giờ Thông báo khi Đóng ứng dụng (Killed State AlarmManager 21:00). |

---

### 2. KẾ HOẠCH HÀNH ĐỘNG KỸ THUẬT

1. **Bước 1:** Viết script Python tự động cập nhật file `BaoCao.docx` bằng thư viện `python-docx`:
   - Cập nhật Bảng 1 (Phân công nhiệm vụ & Đăng ký phương án B3 + B2).
   - Cập nhật đoạn văn và Bảng 1.1 (22 thực thể CSDL PostgreSQL 16 3NF).
   - Nâng cấp Mục 2.2 & 2.2.1: Thuyết minh Clean MVVM, sơ đồ luồng dữ liệu, báo cáo 363/363 Unit Tests Passed.
   - Cập nhật Bảng 2.1 (Danh mục màn hình Bottom Nav, Voice Mic, Notification History).
   - Nâng cấp Mục 2.4 thành 2.4.1 (Native Android Push Notification Engine, Killed State AlarmManager, Deep Linking, Bank Listener) và 2.4.2 (Realtime SignalR 2 chiều Full-Duplex).
   - Cập nhật Bảng 3.1 (6 Kịch bản kiểm thử trực quan trên máy ảo cho Giảng viên).
   - Đảm bảo giữ nguyên các định dạng font Times New Roman, cỡ chữ, bảng biểu, border, caption OpenXML.
2. **Bước 2:** Chạy script kiểm tra file `BaoCao.docx` sau khi cập nhật để xác nhận tính toàn vẹn.
3. **Bước 3:** Cập nhật `MASTER_PROJECT_ROADMAP.md` điều chỉnh Giai đoạn 11:
   - Ghi nhận bỏ bước quay Video Demo theo chỉ đạo của người dùng.
   - Đánh dấu hoàn tất **Chặng 1: Kiểm toán và Nâng cấp Báo cáo Kỹ thuật `BaoCao.docx`**.
4. **Bước 4:** Ghi nhận nhật ký quyết định vào `project_context/decisions_and_changes_log.md` (Quyết định 180).
5. **Bước 5:** Tổng kết và báo cáo kết quả Chặng 1 cho người dùng.
