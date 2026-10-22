# BIÊN BẢN HỌP HỘI ĐỒNG AGENTS: KHỞI ĐỘNG PHÂN HỆ .NET MAUI ENTERPRISE CLIENT
## (COUNCIL SESSION MINUTES: .NET MAUI ARCHITECTURE & MANDATORY MOBILE TECH STACK)

> **Mã phiên:** `SESSION-DOTNET-MAUI-01`  
> **Thời điểm:** 21/10/2026  
> **Chủ trì:** Agent 00 (Council Orchestrator)  
> **Địa điểm:** Không gian Bàn Thảo Kỹ Thuật Dự Án (Durable Context)  

---

### PHẦN 1: KHAI MẠC PHIÊN HỌP (AGENT 00 - ORCHESTRATOR)
Hội đồng tuyên bố chính thức khép lại giai đoạn 1 (phân hệ môn học React Native Scoped MVP đã nộp bài thành công) và bước vào **Giai đoạn 2: Xây dựng Phân hệ Di động Toàn diện .NET MAUI 10 (LTS)** phục vụ Khóa luận Tốt nghiệp và Đồ án PTTKHTTT.
Nhiệm vụ trọng tâm của phiên họp hôm nay:
1. Tháo gỡ thắc mắc về môi trường cài đặt .NET MAUI của sinh viên.
2. Thiết lập danh mục các công nghệ di động bắt buộc theo yêu cầu của Giảng viên (Đặc biệt là **Realtime Dynamic Synchronization**).
3. Thống nhất kiến trúc Solution và lộ trình triển khai chi tiết.

---

### PHẦN 2: THAM LUẬN KỸ THUẬT CỦA CÁC CHUYÊN GIA

#### 1. Tham luận Môi trường & DevOps (Agent 07 - Werner Vogels)
> *"Tôi đã chạy kiểm tra trực tiếp (`dotnet --info`) trên máy tính của bạn và có tin cực vui: **Bạn đã có sẵn .NET SDK 10.0.402 (LTS) và workload `[maui-windows]`** được cấu hình từ Visual Studio!  
> Điều này có nghĩa là bạn có thể tạo và chạy ngay ứng dụng .NET MAUI dưới dạng **Windows 11 Native Desktop** mà không cần đợi cài đặt máy ảo Android.  
> Để hỗ trợ chạy trên Android (điện thoại hoặc máy ảo AVD), bạn chỉ cần chạy thêm lệnh:
> ```powershell
> dotnet workload install maui-android
> ```
> Hoặc mở **Visual Studio Installer** $\rightarrow$ bấm **Modify** $\rightarrow$ tích chọn **.NET Multi-platform App UI development**."*

#### 2. Tham luận Cơ chế Đồng bộ Động Thời Gian Thực (Agent 05 - Martin Kleppmann)
> *"Về yêu cầu của Giảng viên về **Realtime Dynamic Synchronization** (cập nhật dữ liệu tức thì khi có biến động):  
> Trong hệ sinh thái .NET, công nghệ chuẩn công nghiệp hàng đầu chính là **`Microsoft.AspNetCore.SignalR.Client`**.  
> - **Cơ chế 0ms:** Client MAUI duy trì kết nối WebSocket bền vững với SignalR Hub trên Server.
> - **Luồng sự kiện:** Khi có một giao dịch phát sinh (từ Web Portal, người dùng khác, hoặc dịch vụ AI OCR bóc tách xong) $\rightarrow$ Server bắn sự kiện `ReceiveTransactionUpdate` $\rightarrow$ MAUI Client lập tức nạp vào `ObservableCollection<TransactionDto>` $\rightarrow$ Giao diện XAML tự động render lại số dư và biểu đồ tức thì **mà người dùng không cần reload hay kéo vuốt**."*

#### 3. Tham luận Kiến trúc Client MVVM & Reactive UI (Agent 10 - Dan Abramov)
> *"Trên nền tảng .NET MAUI, chúng ta tuyệt đối không viết code-behind lộn xộn trong file `.xaml.cs`.  
> Chúng ta áp dụng chuẩn mực **MVVM hiện đại với `CommunityToolkit.Mvvm`**:
> - Sử dụng Source Generators: `[ObservableProperty]` tự động sinh mã thông báo biến động `INotifyPropertyChanged`.
> - Dùng `[RelayCommand]` cho các nút bấm bất đồng bộ (`async/await`).
> - Dùng `WeakReferenceMessenger` để trao đổi sự kiện giữa các ViewModels độc lập, loại bỏ triệt để nguy cơ rò rỉ bộ nhớ (Memory Leaks)."*

#### 4. Tham luận Ngoại Tuyến & Lưu Trữ Cục Bộ (Agent 05 - Martin Kleppmann)
> *"Một ứng dụng di động chuẩn mực không thể chết khi người dùng mất mạng 4G trong thang máy hay tầng hầm:  
> - Sử dụng **`sqlite-net-pcl`** làm bộ đệm CSDL cục bộ (Local Cache).
> - Mô hình **Offline-First**: Khi thêm giao dịch lúc không có mạng, giao dịch được ghi ngay vào SQLite với cờ `SyncStatus = Pending`.
> - Khi phát hiện có mạng trở lại (`Connectivity.Current.NetworkAccess == NetworkAccess.Internet`): Background Worker tự động gửi các giao dịch chưa đồng bộ lên Cloud và cập nhật lại trạng thái."*

#### 5. Tham luận An ninh & Phần cứng Di động (Agent 11 - Bruce Schneier)
> *"Ứng dụng tài chính cá nhân cần bảo mật cấp ngân hàng:  
> - Sử dụng **`MAUI SecureStorage`**: Tích hợp trực tiếp với phần cứng an toàn **Android KeyStore** (trên Android) và **Apple Keychain** (trên iOS) để lưu trữ Access Token JWT và khóa mã hóa AES-256.
> - Tích hợp **Sinh trắc học (`Plugin.Maui.Biometric`)**: Đòi hỏi xác thực vân tay hoặc nhận diện khuôn mặt (Face ID) khi người dùng mở ứng dụng hoặc truy cập các ví tiền nhạy cảm."*

#### 6. Tham luận Tích hợp Camera & Multimodal Vision AI (Agent 08 - Andrew Ng)
> *"Phân hệ di động sẽ tận dụng phần cứng Camera qua API **`MediaPicker.Default.CapturePhotoAsync`**:  
> Người dùng bấm chụp ảnh hóa đơn mua sắm siêu thị/nhà hàng $\rightarrow$ Nén ảnh tối ưu $\rightarrow$ Gửi lên Pipeline AI OCR bóc tách danh mục món $\rightarrow$ Chạy thuật toán Hậu kiểm toán số học (Mathematical Audit) đối soát tổng tiền khớp với hóa đơn."*

---

### PHẦN 3: KẾT LUẬN & NGHỊ QUYẾT HỘI ĐỒNG
1. Toàn bộ Hội đồng nhất trí phê chuẩn danh mục công nghệ .NET MAUI theo đúng tài liệu [`README.md`](../../mobile-project/dotnet-maui-ver/README.md).
2. Thiết lập lộ trình triển khai gồm 4 bước khởi động:
   - **Bước 1:** Hướng dẫn cấu hình môi trường (.NET SDK 10 + MAUI Workload).
   - **Bước 2:** Khởi tạo cấu trúc Solution `ExpenseApp.sln` theo Clean Architecture (Core, Infrastructure, Maui).
   - **Bước 3:** Cài đặt các gói NuGet bắt buộc.
   - **Bước 4:** Xây dựng Shell UI, Theme Fintech và màn hình Dashboard đầu tiên.
