# KẾ HOẠCH CHI TIẾT GIAI ĐOẠN 11: TỔNG KIỂM TOÁN, BÁO CÁO CUỐI KỲ, ÔN TẬP Q&A VẤN ĐÁP, SCRIPT 1-CLICK & ĐÓNG GÓI APK RELEASE
> **Dự án:** Hệ thống Quản lý Chi tiêu & Hoạch định Tài chính Cá nhân Thông minh (PFM)  
> **Phiên làm việc:** `sessions/session_giai_doan_11_tong_kiem_toan_va_dong_goi/`  
> **Thời điểm xác lập:** 2026-10-25 23:50  
> **Mốc thời gian áp dụng:** **DEADLINE 1 — THỨ TƯ, 28/10/2026 (Môn Lập trình Ứng dụng Di động .NET MAUI)**  
> **Căn cứ barem chấm điểm:** [`mobile-project/dotnet-maui-ver/yeucau-cuoiky.txt`](../../mobile-project/dotnet-maui-ver/yeucau-cuoiky.txt)  
> **Cờ kiểm soát điều phối:** ⚠️ **[TRẠNG THÁI: FROZEN / ĐÓNG BĂNG THỰC THI THEO CHỈ ĐẠO CỦA CHỦ TỌA]**  

---

## I. MỤC TIÊU VÀ NGUYÊN TẮC ĐIỀU PHỐI

1. **Tuân thủ tuyệt đối thứ tự ưu tiên của Chủ tọa đề tài:**
   - **Bước 1:** Xử lý Báo cáo cuối kỳ môn Lập trình Di động .NET MAUI (bám sát barem điểm 10).
   - **Bước 2:** Kịch bản Demo và Hướng dẫn thực hiện Demo (5-8 phút User Story Journey).
   - **Bước 3:** Ôn tập - Q&A để kiểm tra độ thông hiểu về project và công nghệ ăn điểm trong bài thi cuối kỳ (chuẩn bị vấn đáp 1.5đ cá nhân).
   - **Bước 4:** Script khởi chạy tự động 1-Click (`run_demo.ps1`).
   - **Bước 5:** Đóng gói bản cài đặt APK Android Release (`net10.0-android`).

2. **Chế độ Đóng Băng Thực Thi (Execution Freeze Policy):**
   - Kế hoạch này được lập và lưu trữ dưới dạng khung sườn chuẩn xác, hoàn chỉnh.
   - **Không triển khai thực thi bất kỳ bước nào trong 5 bước trên** cho đến khi:
     - Chủ tọa cùng Hội đồng rà soát và khắc phục triệt để các lỗi kỹ thuật/runtime còn tồn đọng khiến dự án "chưa chạy được".
     - Dự án có thể khởi động thực tế trơn tru trên môi trường máy tính/máy ảo của Người dùng.
     - Chủ tọa phát lệnh giải phóng đóng băng (**Unfreeze**).

---

## II. LỘ TRÌNH 5 BƯỚC CHI TIẾT GIAI ĐOẠN 11

```text
┌────────────────────────────────────────────────────────────────────────────────────────┐
│                   LỘ TRÌNH 5 BƯỚC THỰC HIỆN GIAI ĐOẠN 11                               │
├────────────────────────────────────────────────────────────────────────────────────────┤
│ 1. XỬ LÝ BÁO CÁO CUỐI KỲ DI ĐỘNG (.NET MAUI)                                          │
│    - Cấu trúc chuẩn theo barem: Nền tảng (2.5đ) + Push (2.0đ) + MVVM B3 (2.0đ)         │
│      + SignalR B2 (+1.0đ điểm cộng) + Mã nguồn (1.0đ) + Báo cáo (1.0đ)                 │
│                                                                                        │
│ 2. KỊCH BẢN DEMO & HƯỚNG DẪN DEMO (5 - 8 PHÚT)                                         │
│    - Luồng câu chuyện tài chính thực tế: Dashboard -> AI OCR -> Jev Action Inbox        │
│      -> SignalR đồng bộ 2 thiết bị -> Thaler Nudge 24h -> Push Notification            │
│                                                                                        │
│ 3. BỘ CÂU HỎI ÔN TẬP - Q&A THÔNG HIỂU CÔNG NGHỆ (VẤN ĐÁP 1.5Đ)                        │
│    - Hỏi - đáp sâu về Clean MVVM, SignalR Hub, Push Notification FCM/Local,            │
│      Clean Architecture, PostgreSQL ACID Locking, Thuật toán DSA & AI Multimodal       │
│                                                                                        │
│ 4. SCRIPT KHỞI CHẠY TỰ ĐỘNG 1-CLICK (`run_demo.ps1`)                                   │
│    - Script PowerShell tự kiểm tra Docker DB -> Migrate DB -> Chạy Backend 5018       │
│      -> Mở ứng dụng MAUI mượt mà chỉ với 1 cú click                                    │
│                                                                                        │
│ 5. ĐÓNG GÓI BẢN PHÁT HÀNH APK ANDROID RELEASE                                         │
│    - Lệnh dotnet publish -f net10.0-android -c Release, tạo Keystore & file APK độc lập │
└────────────────────────────────────────────────────────────────────────────────────────┘
```

---

### BƯỚC 1: XỬ LÝ BÁO CÁO CUỐI KỲ MÔN DI ĐỘNG (.NET MAUI)
* **Tác tử phụ trách:** Agent 03 (Cockburn BA Lead) & Agent 00 (Council Orchestrator).
* **Mục tiêu sản phẩm:** Bản báo cáo toàn diện [`BAO_CAO_CUOI_KY_DOTNET_MAUI.md`](../../mobile-project/dotnet-maui-ver/BAO_CAO_CUOI_KY_DOTNET_MAUI.md) (sẵn sàng chuyển đổi sang Word/PDF `CK_Nhom_XX_QuanLyChiTieu_NET_MAUI.pdf`).
* **Nội dung bắt buộc khớp trọn vẹn barem điểm 10 tại `yeucau-cuoiky.txt`:**
  1. **Trang bìa & Phân công nhiệm vụ (100% minh bạch):**
     - Đề tài số 15: Quản lý chi tiêu cá nhân (Mở rộng chuyên sâu tích hợp AI).
     - Phân định tỷ lệ phần trăm đóng góp của 2 thành viên nhóm theo đúng quy định.
  2. **Yêu cầu nền tảng (2,5 Điểm):**
     - Trình bày xác thực JWT: Đăng ký, Đăng nhập, Lưu token an toàn (`ISecureStorage`), tự động làm mới và tự động đăng nhập khi mở app.
     - Khung CRUD hoàn chỉnh 10 phân hệ nghiệp vụ qua REST API: Ví tiền, Thu nhập, Chi tiêu, Danh mục, Ngân sách, Hũ tài chính, Mục tiêu tiết kiệm, v.v.
     - Xử lý trạng thái UX: Loading indicators, lỗi mất kết nối mạng, danh sách rỗng (Empty States).
  3. **Phần A - Push Notification (2,0 Điểm Bắt Buộc):**
     - Xin quyền thông báo runtime, lấy Device Token và gửi đăng ký lên Backend (`/api/notifications/devices`).
     - Gửi thông báo tự động từ Server cho ít nhất 2 sự kiện nghiệp vụ cốt lõi:
       - *Sự kiện 1:* Cảnh báo chi tiêu vượt ngưỡng an toàn / Vượt ngân sách danh mục.
       - *Sự kiện 2:* Khoản chi chạm ngưỡng đắn đo cần khoảng trễ tâm lý (24h Cooling-Off Nudge) hoặc Gợi ý hành động tài chính tự hành (Jev Action Card).
     - Xử lý thông báo ở cả 3 trạng thái của ứng dụng: **Foreground** (Banner hiển thị trong app), **Background** (Khay thông báo hệ điều hành) và **Terminated** (Ứng dụng đã tắt hoàn toàn).
     - Điều hướng Deep Linking: Chạm vào notification tự động mở đúng trang chi tiết giao dịch hoặc Action Inbox tương ứng dựa vào trường `data` trong payload.
     - Có ít nhất 1 thông báo cục bộ lên lịch (`Scheduled Local Notification`): Nhắc nhở ghi chép chi tiêu cuối ngày lúc 21h00 hoặc nhắc hạn đóng tiền định kỳ.
     - Màn hình Lịch sử Thông báo (`NotificationsPage.xaml`): Đánh dấu đã đọc/chưa đọc (hỗ trợ HTTP PATCH), lọc theo phân loại và cài đặt bật/tắt từng loại cảnh báo.
  4. **Phần B - Phương án kỹ thuật chính B3 MVVM (2,0 Điểm):**
     - Tách bạch 3 tầng rành mạch:
       - **Model:** Entities, Data Transfer Objects, Repositories (`IWalletRepository`, `ITransactionRepository`, v.v.), Services.
       - **ViewModel:** Kế thừa `ObservableObject` (CommunityToolkit.Mvvm), quản lý state, lệnh `IRelayCommand`, validation nghiệp vụ, độc lập hoàn toàn với View.
       - **View:** 20 màn hình XAML thuần Data Binding, hoàn toàn không chứa logic nghiệp vụ hay trực tiếp gọi API/Storage.
     - Cơ chế Mock vs Real Repositories thông qua Interface chung: Cho phép chuyển đổi nguồn dữ liệu 0ms mà không phải sửa một dòng mã nào trong ViewModel.
     - Bằng chứng kiểm thử: 42 Unit Tests ViewModel chạy trên xUnit/Moq đạt 100% Passed.
  5. **Điểm cộng đột phá - Phần B2 SignalR Hub (+1,0 Điểm Tối Đa):**
     - Xây dựng Hub SignalR trên ASP.NET Core: Xác thực JWT bằng `accessTokenFactory`, bảo vệ bằng `[Authorize]`.
     - Kênh truyền thời gian thực 2 chiều: Client invoke phương thức lên Hub, Server phát sóng `Clients.User(userId)` hoặc `Clients.Group()`.
     - Ứng dụng nghiệp vụ: Đồng bộ biến động số dư ví tức thời giữa 2 thiết bị chạy song song; phát thông báo real-time khi có giao dịch mới mà không cần F5/tải lại trang.
     - Quản lý vòng đời kết nối: Tự động kết nối lại (`WithAutomaticReconnect`), quản lý trạng thái kết nối trên thanh trạng thái, tự hủy handler khi rời màn hình.
  6. **Chất lượng mã nguồn, Kiến trúc và Điểm sáng tạo (1,0 Điểm):**
     - Clean Architecture Backend (Domain, Application, Infrastructure, WebApi).
     - 3 Tính năng AI & Sáng tạo vượt barem: Multimodal OCR Gemini đối soát khớp $\le 1.000$đ, Voice Intent Router, và Server-Driven Jev Action Protocol.

---

### BƯỚC 2: KỊCH BẢN DEMO & HƯỚNG DẪN THỰC HIỆN DEMO (5 - 8 PHÚT)
* **Tác tử phụ trách:** Agent 10 (Dan Abramov) & Agent 12 (Kent Beck).
* **Mục tiêu sản phẩm:** Tài liệu kịch bản từng giây [`KICH_BAN_DEMO_VIDEO_CUOI_KY.md`](../../mobile-project/dotnet-maui-ver/KICH_BAN_DEMO_VIDEO_CUOI_KY.md).
* **Cấu trúc 5 phân cảnh đắt giá (User Story Journey):**
  - **Phân cảnh 1 (0:00 - 1:15) - Nền tảng & Khởi động:**
    - Mở ứng dụng, hiển thị Splash Screen và tự động đăng nhập an toàn.
    - Lướt màn hình Dashboard: Thẻ số dư Đa ví, biểu đồ thu chi phân loại, Hạn mức chi tiêu an toàn hôm nay (DSA Đàn hồi).
  - **Phân cảnh 2 (1:15 - 2:45) - Đột phá AI Multimodal OCR Hóa đơn:**
    - Nhấn quét hóa đơn camera/chọn ảnh hóa đơn mẫu.
    - AI Gemini Vision phân tích tự động, trích xuất danh sách món, thuế VAT, tiền thừa.
    - Kích hoạt cơ chế Kiểm toán Số học: Báo cáo khớp sai lệch $\le 1.000$đ và tự động điền vào form tạo giao dịch.
  - **Phân cảnh 3 (2:45 - 4:15) - Ngôi sao Công nghệ: Jev Action Protocol & SignalR Realtime (+1đ):**
    - Đặt 2 màn hình/2 thiết bị chạy song song (Thiết bị A và Thiết bị B cùng đăng nhập 1 tài khoản).
    - Giả lập nhận SMS biến động số dư ngân hàng $\rightarrow$ Thẻ hành động `SmartActionCard` xuất hiện trong Action Inbox với độ tin cậy $P_{jev} \ge 0.85$.
    - Người dùng bấm nút **"Lưu nhanh 1-chạm"** trên Thiết bị A $\rightarrow$ Ngay lập tức màn hình Thiết bị B tự nhảy số dư ví và biểu đồ cập nhật tức thời qua SignalR Hub mà không cần tải lại trang!
  - **Phân cảnh 4 (4:15 - 5:30) - Push Notification 3 Trạng thái & Tâm lý Hành vi Thaler Nudge:**
    - Thử nhập một khoản chi vượt hạn mức ngày $\rightarrow$ Kích hoạt Cú hích Nudge và Khoảng trễ 24h Cooling-Off.
    - Thu nhỏ app xuống Background hoặc Tắt app hoàn toàn (Terminated state) $\rightarrow$ Server bắn Push Notification cảnh báo $\rightarrow$ Chạm vào Notification từ khay hệ thống, ứng dụng lập tức mở thẳng đến màn hình chi tiết cảnh báo (Deep Link).
    - Trình diễn thông báo cục bộ được lên lịch (Scheduled Local Reminder).
  - **Phân cảnh 5 (5:30 - 6:30) - Minh chứng Kiến trúc MVVM & Kiểm thử tự động:**
    - Mở Visual Studio / Rider: Trình bày cấu trúc thư mục Model - View - ViewModel - Repositories.
    - Chạy toàn bộ 296 Unit Tests trên Test Explorer: Xanh lá 100% (42 Unit Tests cho MAUI ViewModels).

---

### BƯỚC 3: BỘ CÂU HỎI ÔN TẬP - Q&A THÔNG HIỂU DỰ ÁN & BẢO VỆ MÃ NGUỒN (ĂN ĐIỂM VẤN ĐÁP 1.5Đ)
* **Tác tử phụ trách:** Toàn thể Hội đồng (Agent 00, 03, 04, 05, 07, 08, 10, 11, 12).
* **Mục tiêu sản phẩm:** Cẩm nang ôn tập vấn đáp [`BO_CAU_HOI_ON_TAP_VAN_DAP_CUOI_KY.md`](../../mobile-project/dotnet-maui-ver/BO_CAU_HOI_ON_TAP_VAN_DAP_CUOI_KY.md).
* **Các chủ đề câu hỏi "sát sườn" giảng viên thường chất vấn:**
  1. **Nhóm câu hỏi Kiến trúc MVVM (Phần B3):**
     - *Câu 1:* Tại sao trong View (`.xaml.cs`) không được phép gọi trực tiếp HttpClient hoặc Database? ViewModel giao tiếp với View như thế nào?
     - *Câu 2:* Cơ chế Data Binding trong .NET MAUI hoạt động ra sao? `ObservableProperty` và `RelayCommand` của CommunityToolkit.Mvvm sinh mã gì bên dưới?
     - *Câu 3:* Làm thế nào để chuyển đổi giữa Mock Data và Real API mà không làm thay đổi logic trong ViewModel?
  2. **Nhóm câu hỏi Thời gian thực SignalR (Phần B2 - Điểm cộng):**
     - *Câu 4:* Giao thức WebSocket và SignalR bắt tay (handshake) như thế nào? Truyền JWT qua tham số nào khi kết nối (`accessTokenFactory`)?
     - *Câu 5:* Khi người dùng đóng ứng dụng hoặc chuyển trang, làm sao để tránh rò rỉ bộ nhớ (Memory Leak) với SignalR Handlers?
     - *Câu 6:* Tại sao lại dùng `Clients.User(userId)` thay vì broadcast toàn bộ `Clients.All`?
  3. **Nhóm câu hỏi Push Notifications & Background Service (Phần A):**
     - *Câu 7:* Phân biệt Remote Push Notification (FCM) và Local Scheduled Notification?
     - *Câu 8:* Làm sao để ứng dụng điều hướng đến đúng trang khi người dùng nhấn vào thông báo ở trạng thái Terminated (đã tắt ứng dụng)?
  4. **Nhóm câu hỏi Backend Clean Architecture & Cơ sở Dữ liệu ACID:**
     - *Câu 9:* Tại sao dùng PostgreSQL với Pessimistic Locking (`SELECT FOR UPDATE`) khi cập nhật số dư ví?
     - *Câu 10:* Sự khác nhau giữa `PUT` (thay thế toàn bộ) và `PATCH` (RFC 7396) mà hệ thống vừa triển khai cho 4 phân hệ?
  5. **Nhóm câu hỏi Trí tuệ Nhân tạo & Thuật toán Tài chính:**
     - *Câu 11:* Thuật toán DSA (Daily Safe-to-Spend) tính toán hạn mức đàn hồi như thế nào khi người dùng chi tiêu lố ở đầu tháng?
     - *Câu 12:* Cơ chế Server-Driven Action Protocol và ngưỡng xác suất Jev Probability ($P_{jev} \ge 0.85$) ngăn chặn rủi ro tự động hóa sai lầm ra sao?

---

### BƯỚC 4: SCRIPT KHỞI CHẠY TỰ ĐỘNG 1-CLICK (`run_demo.ps1`)
* **Tác tử phụ trách:** Agent 07 (Werner Vogels).
* **Mục tiêu sản phẩm:** File kịch bản PowerShell [`run_demo.ps1`](../../mobile-project/dotnet-maui-ver/run_demo.ps1).
* **Các tác vụ script tự động thực hiện tuần tự:**
  1. Kiểm tra Docker daemon đang chạy; nếu chưa chạy thì cảnh báo hướng dẫn người dùng.
  2. Kích hoạt container PostgreSQL DB (`docker compose up -d`).
  3. Kiểm tra cổng 5432 sẵn sàng kết nối.
  4. Tự động áp dụng EF Core Migrations lên CSDL.
  5. Khởi chạy Backend WebApi trên cổng `http://localhost:5018` trong một cửa sổ nền.
  6. Chờ Endpoint `/health` trả về HTTP 200 OK.
  7. Tự động mở Visual Studio / khởi chạy ứng dụng MAUI để sẵn sàng trình diễn trước hội đồng.

---

### BƯỚC 5: ĐÓNG GÓI BẢN PHÁT HÀNH APK ANDROID RELEASE
* **Tác tử phụ trách:** Agent 07 (Werner Vogels) & Agent 10 (Dan Abramov).
* **Mục tiêu sản phẩm:** File cài đặt độc lập `ExpenseTracker.Android.apk` và tài liệu hướng dẫn cài đặt [`HUONG_DAN_CAI_DAT_APK.md`](../../mobile-project/dotnet-maui-ver/HUONG_DAN_CAI_DAT_APK.md).
* **Quy trình đóng gói chuẩn:**
  1. Cấu hình `ExpenseTracker.Maui.csproj` với TargetFramework `net10.0-android`.
  2. Tạo chứng chỉ ký số tự cấp (Keystore) bằng `keytool`.
  3. Chạy lệnh xuất bản độc lập (Unsigned hoặc Signed APK):
     ```powershell
     dotnet publish -f net10.0-android -c Release -p:AndroidKeyStore=true ...
     ```
  4. Xuất file APK ra thư mục `bin/Release/net10.0-android/publish/` và copy ra thư mục gốc để người dùng nộp bài hoặc cài trực tiếp lên điện thoại Android.

---

## III. CHỈ THỊ ĐÓNG BĂNG & KẾ HOẠCH HÀNH ĐỘNG TIẾP THEO

> [!CAUTION]
> **LỆNH ĐÓNG BĂNG GIAI ĐOẠN 11 ĐÃ CÓ HIỆU LỰC!**  
> Hội đồng tạm dừng toàn bộ việc viết Báo cáo, viết Script và đóng gói APK.  
> Ngay lúc này, toàn bộ sự tập trung của Hội đồng được chuyển dịch 100% sang việc: **"Cùng Người dùng chẩn đoán và khắc phục toàn bộ các lỗi/vấn đề đang khiến dự án chưa chạy được".**
