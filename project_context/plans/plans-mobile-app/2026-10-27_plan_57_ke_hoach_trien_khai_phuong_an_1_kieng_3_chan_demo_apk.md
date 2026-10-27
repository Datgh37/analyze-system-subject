# KẾ HOẠCH TRIỂN KHAI PHƯƠNG ÁN 1 (ĐIỀU CHỈNH): MÔ HÌNH KIỀNG 3 CHÂN HẠ TẦNG DEMO & KIỂM CHỨNG NOTIFICATION DETECTION KILLED STATE
**Mã kế hoạch:** `PLAN-57 (Rev.2)` | **Dự án:** Smart Personal Expense & Financial Planning (.NET MAUI 10 & ASP.NET Core 10 Web API)  
**Mục đích:** Xây dựng cơ chế kết nối Online ưu tiên với Retry Policy thông minh (3 lần x 15s), chuyển mạch Runtime Server URL (Cloudflare Named Tunnel), và quy trình kiểm chứng thực tế kịch bản bắt thông báo biến động số dư ngân hàng (Bank Notification Detection) khi app đóng hoàn toàn.  
**Thời gian cập nhật:** 2026-10-27 15:30  
**Phạm vi áp dụng:** Công cụ kỹ thuật nội bộ bổ trợ phục vụ demo & bảo vệ đề tài (Không ghi vào `BaoCao.docx`).

---

## I. MỤC TIÊU & CƠ CHẾ KỸ THUẬT ĐIỀU CHỈNH

### 1. Chiến lược Kết Nối Thông Minh (Online First with Smart Resilience):
- **Chế độ Mặc Định:** **Ưu tiên chạy Online (Live Backend API)** để sẵn sàng gọi API thật ngay khi mở app.
- **Quy trình Kết Nối & Retry Policy:**
  ```
  [MỞ APP] ──> [LẦN 1: PING BACKEND (/api/health)]
                    │
                    ├──> (Thành công 200 OK) ──> KẾT NỐI ONLINE THÀNH CÔNG (Live API Mode)
                    │
                    └──> (Thất bại / Timeout) ──> CHỜ 15 GIÂY ──> [LẦN 2: PING LẠI]
                                                      │
                                                      ├──> (Thành công) ──> ONLINE
                                                      │
                                                      └──> (Thất bại) ──> CHỜ 15 GIÂY ──> [LẦN 3: PING LẠI]
                                                                              │
                                                                              ├──> (Thành công) ──> ONLINE
                                                                              │
                                                                              └──> (Thất bại sau 3 lần)
                                                                                       │
                                                                                       ▼
                                                                  TỰ ĐỘNG CHUYỂN SANG OFFLINE MOCK MODE
                                                                  (Hiển thị Toast: "Server offline, đang chuyển sang chế độ offline")
  ```
- **Chuyển Đổi Thủ Công:** Người dùng có thể chủ động gạt công tắc giữa `Live Backend` và `Offline Mock` tại màn hình Cài đặt bất kỳ lúc nào mà không cần chờ hết 3 lần retry.
- **Server Base URL Điền Sẵn:** Mặc định điền sẵn URL cấu hình (hỗ trợ Cloudflare Named Tunnel cố định từ tài khoản của bạn hoặc URL Hotspot).

### 2. Kịch Bản Bank Notification Detection (Bắt Thông Báo Chuyển Khoản Khi App Đóng/Ngầm):
- **Bản chất kỹ thuật:** 
  - `BankNotificationListenerService` kế thừa từ `Android.Service.Notification.NotificationListenerService`.
  - Đây là System Service do chính Kernel Android quản lý độc lập. Khi người dùng cấp quyền "Truy cập thông báo" (Notification Access), tiến trình lắng nghe này sống liên tục ngay cả khi app `ExpenseTracker.Maui` bị vuốt tắt (Killed State).
- **Quy trình hoạt động khi có thông báo ngân hàng/MoMo:**
  ```
  App Ngân Hàng (MoMo / ACB / VCB) bắn thông báo biến động số dư
                         │
                         ▼
  Android OS chuyển thông báo tới BankNotificationListenerService
                         │
                         ▼
  BankNotificationParser bóc tách: Số tiền (-150.000đ) & Nội dung (Phúc Long Coffee)
                         │
                         ▼
  Hệ thống bắn Native Heads-up Notification: "💳 Phát hiện chi tiêu: -150.000 đ"
                         │
                         ▼
  Người dùng chạm vào thông báo ──> Đánh thức app dậy & Tự động điền Form chi tiêu!
  ```

---

## II. BỐ TRÍ KIẾN TRÚC MÃ NGUỒN

### 1. Dịch Vụ Cấu Hình & Retry Service (`Services/Config/`):
- `IServerConfigService` & `ServerConfigService.cs`:
  - `bool IsLiveApiMode { get; set; }` (Mặc định: `true`).
  - `string ServerBaseUrl { get; set; }` (Mặc định lưu vào `Preferences`).
  - `int RetryCount { get; } = 3;`
  - `TimeSpan RetryDelay { get; } = TimeSpan.FromSeconds(15);`
  - `Task<bool> EnsureConnectionWithRetryAsync()`: Thực thi vòng lặp thử 3 lần x 15 giây, nếu thất bại tự động đặt `IsLiveApiMode = false`.
  - `Task<bool> PingServerAsync(string url = null)`: Gửi HTTP GET `/api/health` với timeout ngắn 3 giây.

### 2. Chuyển Mạch Tầng Dependency Injection (`MauiProgram.cs`):
- Đăng ký Factory Provider cho toàn bộ Repositories:
  ```csharp
  builder.Services.AddScoped<IWalletRepository>(sp => {
      var config = sp.GetRequiredService<IServerConfigService>();
      return config.IsLiveApiMode 
          ? (IWalletRepository)sp.GetRequiredService<ApiWalletRepository>() 
          : (IWalletRepository)sp.GetRequiredService<MockWalletRepository>();
  });
  ```
- Cấu hình Refit Client nạp dynamic handler đọc `ServerBaseUrl` linh hoạt.

### 3. Giao Diện Quản Lý Kết Nối (`SettingsPage.xaml` & `SettingsViewModel.cs`):
- Switch: *Chế độ kết nối máy chủ thật (Live API)*.
- Entry: *URL Máy chủ Backend*.
- Nút *Kiểm tra kết nối (Ping)*.
- Nút *Kiểm tra Bóc tách Thông báo Ngân hàng (Simulate Bank SMS/Notification)* để test nhanh chức năng mà không cần chuyển khoản tiền thật.

### 4. Thiết Lập Cloudflare Named Tunnel (URL Cố Định):
- Hướng dẫn cấu hình Named Tunnel cố định từ tài khoản Cloudflare của bạn (tạo tunnel 1 lần, lấy URL cố định dạng `https://api.domain.com` hoặc token cố định).
- Cập nhật script `RUN_TUNNEL.bat` chạy tunnel cố định này.

---

## III. KẾ HOẠCH PHÂN KỲ THỰC THI (STEP-BY-STEP)

| Bước | Nhiệm Vụ | Tệp Tin Tác Động |
| :---: | :--- | :--- |
| **Bước 1** | Xây dựng `IServerConfigService` với logic **Online First + Retry 3 lần x 15s** | `Services/Config/ServerConfigService.cs` |
| **Bước 2** | Nâng cấp `MauiProgram.cs` hỗ trợ Dynamic Repository Factory & Startup Connection Checker | `MauiProgram.cs` |
| **Bước 3** | Cập nhật UI Cài đặt: Điều khiển Live API, Server URL, Nút Ping & Nút Test Bank Notification | `Views/SettingsPage.xaml`, `ViewModels/SettingsViewModel.cs` |
| **Bước 4** | Thiết lập Cloudflare Named Tunnel cố định URL & kịch bản `RUN_TUNNEL.bat` | `RUN_TUNNEL.bat`, `quick-run-scripts/run_tunnel.ps1` |
| **Bước 5** | Kiểm chứng thực tế: Chạy test Bank Notification Detection (Killed State) và Online/Offline Switcher | Terminal / Android Emulator / Real Device |
| **Bước 6** | Hoàn tất cẩm nang hướng dẫn demo nội bộ | `implement-docs/HUONG_DAN_KET_NOI_LIVE_DEMO_TUNNEL.md` |
