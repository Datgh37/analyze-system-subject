# 📹 KỊCH BẢN THAO TÁC QUAY VIDEO DEMO (5 – 8 PHÚT)
## ĐỀ TÀI SỐ 15: QUẢN LÝ CHI TIÊU CÁ NHÂN
> **Mục tiêu:** Video quay màn hình thao tác thực tế (Screen Recording), **tập trung 100% vào các thao tác bấm/nhập liệu**, không cần thu âm thuyết trình dài dòng.  
> **Căn cứ yêu cầu đồ án:** [`yeucau-cuoiky.txt`](../yeucau-cuoiky.txt) (Đề tài 15: Vượt ngân sách tháng, Nhắc ghi chép cuối ngày, Đồng bộ ví realtime giữa 2 thiết bị, Push notification killed state, Show code MVVM & chạy Unit Tests).  
> **Tổng thời lượng khuyến nghị:** ~ 06 phút (vừa vặn trong khoảng 5 - 8 phút).

---

## TỔNG HỢP CÁC BƯỚC THAO TÁC CHÍNH

```
[00:00 - 01:15] Bước 1: Đăng nhập & Nghiệp vụ nền tảng (Ví, Thêm Chi Tiêu, Thêm Thu Nhập)
[01:15 - 02:30] Bước 2: Thiết lập Ngân Sách & Bắn Thông Báo Vượt Ngân Sách (Foreground/Background/Deep Link)
[02:30 - 03:45] Bước 3: Thông báo Nhắc Ghi Chép Cuối Ngày khi ĐÃ TẮT ỨNG DỤNG (Killed State)
[03:45 - 05:00] Bước 4: Đồng bộ Ví Realtime SignalR giữa 2 thiết bị song song (0ms không cần reload)
[05:00 - 05:45] Bước 5: Chiếu nhanh cấu trúc mã nguồn Clean MVVM & Chạy lệnh `dotnet test` (82 tests xanh)
[05:45 - 06:30] Bước 6: Chức năng phụ trợ tạo ấn tượng (Quét hóa đơn AI Vision OCR & Báo cáo 50/30/20)
```

---

## CHI TIẾT TỪNG BƯỚC THAO TÁC TRÊN MÀN HÌNH

### BƯỚC 1: ĐĂNG NHẬP & QUẢN LÝ VÍ, CHI TIÊU CƠ BẢN (00:00 – 01:15)
* **Màn hình ghi hình:** Màn hình điện thoại Android / Giả lập Android.
* **Các thao tác cần bấm:**
  1. **Mở ứng dụng:** Thấy màn hình `Đăng Nhập Tài Khoản`.
  2. **Thao tác:** Bấm nút **"Đăng Nhập"** (hoặc chọn Demo Mode). Ứng dụng hiển thị hiệu ứng Loading và chuyển vào màn hình **"Danh Sách Ví" (`WalletsPage`)**.
  3. **Thao tác trên màn hình Ví:**
     - Lướt xem danh sách các ví (Ví Tiền Mặt, Thẻ Ngân Hàng ACB, Thẻ Tín Dụng, Ví MoMo).
     - Bấm nút **"Thêm Ví Mới"**, nhập tên ví: `Heo Đất Tiết Kiệm`, số dư ban đầu `500.000đ`, bấm **"Lưu Tài Khoản Ví"**.
     - Thấy ví mới xuất hiện ngay trên danh sách và tổng số dư khả dụng tự động cộng dồn.
  4. **Thao tác ghi một khoản Chi Tiêu:**
     - Dưới thanh Bottom Navigation, bấm vào tab **"Chi Tiêu" (`ExpensesPage`)**.
     - Nhập số tiền: `45.000`.
     - Chọn danh mục: `Ăn uống`.
     - Chọn ví chi: `Ví Tiền Mặt`.
     - Nhập ghi chú: `Ăn trưa bún bò`.
     - Bấm nút **"Lưu Khoản Chi Tiêu"**.
     - Quan sát danh sách bên dưới lập tức cập nhật giao dịch mới và số dư ví tiền mặt trừ đi 45.000đ.
  5. **Thao tác ghi một khoản Thu Nhập:**
     - Bấm sang tab **"Thu Nhập" (`IncomesPage`)**.
     - Nhập số tiền: `2.000.000`.
     - Chọn danh mục: `Lương & Thu Nhập Chính`.
     - Bấm **"Lưu Khoản Thu Nhập"**.

---

### BƯỚC 2: PHẦN A (SỰ KIỆN 1) – CẢNH BÁO VƯỢT NGÂN SÁCH THÁNG (01:15 – 02:30)
> *Yêu cầu Đề tài 15:* Sự kiện 1 gửi Push Notification khi vượt/chạm ngưỡng ngân sách tháng. Kiểm tra Foreground, Background và Deep Link.

* **Các thao tác cần bấm:**
  1. **Vào trang Ngân Sách:** Từ menu / bottom bar, chuyển đến **"Hạn Mức Ngân Sách" (`BudgetsPage`)**.
  2. **Xem hạn mức hiện tại:** Thấy ngân sách tháng cho danh mục `Ăn uống` hạn mức là `1.000.000đ`, hiện đã chi `950.000đ` (tiến độ 95% - Báo động vàng).
  3. **Tạo giao dịch làm vượt hạn mức:**
     - Nhấn nút thêm chi tiêu nhanh: Chi `100.000đ` cho `Ăn uống`.
     - Bấm **"Lưu Khoản Chi Tiêu"**.
  4. **Trực quan hóa Thông báo (Foreground / Heads-Up):**
     - Ngay lập tức, thanh thông báo Heads-Up Native từ đỉnh màn hình hạ xuống:  
       `"CẢNH BÁO VƯỢT NGÂN SÁCH: Danh mục Ăn uống đã chi 1.050.000đ / 1.000.000đ (Vượt 105%)!"`
  5. **Thử nghiệm ở trạng thái Background:**
     - Bấm nút **Home** trên điện thoại để đưa app xuống nền (Background).
     - Vào app / web gửi tiếp 1 giao dịch cảnh báo ngân sách.
     - Thanh Notification trên thanh trạng thái Android xuất hiện biểu tượng cảnh báo kèm âm thanh thông báo.
  6. **Kiểm tra Deep Link:**
     - Dùng ngón tay vuốt thanh thông báo xuống, **chạm trực tiếp vào thông báo cảnh báo ngân sách**.
     - Ứng dụng tự động mở lên và **nhảy thẳng vào trang `Hạn Mức Ngân Sách` (`BudgetsPage`)**, đúng danh mục bị vượt!

---

### BƯỚC 3: PHẦN A (SỰ KIỆN 2) – NHẮC GHI CHÉP CUỐI NGÀY KHI ĐÃ TẮT APP (02:30 – 03:45)
> *Yêu cầu Đề tài 15:* Thông báo nhắc ghi chép cuối ngày (Local Notification 21:00) và bắt buộc thể hiện khi ứng dụng **ĐÃ BỊ TẮT HOÀN TOÀN (KILLED STATE)**.

* **Các thao tác cần bấm:**
  1. **Vào trang Cài Đặt:** Bấm vào tab **"Cài Đặt" (`SettingsPage`)**.
  2. **Cuộn xuống Mục 4:** *"Kiểm Thử Thông Báo Khi Đã Tắt App (Killed State)"*.
  3. **Bấm nút kích hoạt:**
     - Bấm nút màu cam: **"Hẹn Giờ 10 Giây & Vuốt Tắt App (Demo)"**.
     - Một thông báo ngắn (Toast) hiện ra: *"Đã hẹn giờ báo thức nhắc nhở sau 10 giây qua Android AlarmManager..."*
  4. **THAO TÁC VUỐT TẮT ỨNG DỤNG (KILLED STATE):**
     - Bấm nút phím đa nhiệm của Android (nút ô vuông hoặc vuốt thanh điều hướng lên và giữ).
     - Thấy màn hình thu nhỏ của ứng dụng Expense Tracker.
     - **Dùng tay vuốt ứng dụng văng lên trên để TẮT HOÀN TOÀN KHỎI BỘ NHỚ RAM**.
     - Quay trở về màn hình chính (Home screen) của Android.
  5. **Đợi 10 giây:**
     - Giữ nguyên camera quay màn hình Home.
     - Đúng sau 10 giây: **Hệ thống Android OS tự động phát âm thanh, rung máy và hiện lên thông báo:**  
       `"Nhắc Nhở Cuối Ngày: Bạn đã ghi nhận các khoản chi tiêu hôm nay chưa? Mở app ngay để quản lý tài chính!"`
  6. **Chạm vào thông báo:**
     - Bấm vào thông báo -> Ứng dụng khởi động lại và mở trang chủ.
     - Vào trang **"Lịch Sử Thông Báo" (`NotificationHistoryPage`)** lướt xem danh sách các thông báo đã nhận.

---

### BƯỚC 4: PHẦN B2 – ĐỒNG BỘ VÍ REALTIME SIGNALR GIỮA 2 THIẾT BỊ (03:45 – 05:00)
> *Yêu cầu Đề tài 15:* Ví chi tiêu chung đồng bộ giữa nhiều thiết bị / thành viên, cập nhật tức thời không cần tải lại trang.

* **Màn hình ghi hình:** Mở song song 2 cửa sổ trên màn hình máy tính:
  - Cửa sổ Trái: Thiết bị A (Máy ảo Android hoặc 1 cửa sổ MAUI).
  - Cửa sổ Phải: Thiết bị B (Thiết bị thật thứ 2 hoặc trình duyệt/cửa sổ thứ 2).
  - Cả 2 cùng mở màn hình **Danh Sách Ví (`WalletsPage`)** của cùng 1 tài khoản.
* **Các thao tác cần bấm:**
  1. **Cho thấy trạng thái kết nối:** Cả 2 bên đều hiển thị badge xanh: *"Đã kết nối SignalR (Realtime Live)"*.
  2. **Quan sát số dư ban đầu:** Ví dụ ví ACB đang có `10.000.000đ` ở cả 2 thiết bị.
  3. **Thao tác trên Thiết bị A:**
     - Nhấn thêm nhanh khoản chi tiêu: `200.000đ` mua sắm từ ví ACB.
     - Bấm nút **"Lưu Khoản Chi Tiêu"**.
  4. **Quan sát Thiết bị B (KHÔNG ĐƯỢC CHẠM VÀO THIẾT BỊ B):**
     - Ngay trong tích tắc (0ms), số dư ví ACB bên Thiết bị B **tự động nhảy số giảm xuống còn `9.800.000đ`**!
     - Thanh phân bổ tổng tài sản bên Thiết bị B tự động cập nhật mà người dùng **hoàn toàn không cần kéo vuốt làm mới trang (Pull-to-refresh)**.
  5. **Thao tác ngược lại từ Thiết bị B sang Thiết bị A:**
     - Trên Thiết bị B, bấm nhận tiền chuyển vào: `500.000đ`.
     - Bên Thiết bị A ngay lập tức tự động cộng tiền `10.300.000đ` theo thời gian thực!

---

### BƯỚC 5: PHẦN B3 – SHOW CẤU TRÚC CODE MVVM & CHẠY 82 UNIT TESTS (05:00 – 05:45)
> *Yêu cầu Đề tài 15:* B3 Kiến trúc MVVM: trình bày cấu trúc mã nguồn 3 tầng (Model - ViewModel - View) và chạy Unit Tests xanh 100%.

* **Màn hình ghi hình:** Màn hình Visual Studio hoặc VS Code + Terminal.
* **Các thao tác cần làm:**
  1. **Chiếu cây thư mục dự án:**
     - Mở thư mục `ExpenseTracker.Maui`:
       * `Models/`: Các entity dữ liệu (`WalletModel.cs`, `ExpenseModel.cs`, `BudgetModel.cs`).
       * `ViewModels/`: Các class ViewModel kế thừa `ObservableObject` từ `CommunityToolkit.Mvvm` (`WalletsViewModel.cs`, `ExpensesViewModel.cs`).
       * `Views/`: Các trang giao diện XAML thuần túy.
       * `Repositories/`: Các interface `IWalletRepository.cs`, `IBudgetRepository.cs` tách biệt giữa `ApiWalletRepository` (gọi backend thật) và `MockWalletRepository` (offline).
  2. **Mở 1 cặp View - ViewModel mẫu:**
     - Mở `WalletsPage.xaml`: Cho thấy dùng Data Binding `{Binding Wallets}`, `{Binding AddWalletCommand}`.
     - Mở `WalletsPage.xaml.cs`: Cho thấy **Zero Code-Behind** (chỉ có dòng `InitializeComponent()`).
     - Mở `WalletsViewModel.cs`: Cho thấy dùng `[ObservableProperty]`, `[RelayCommand]`.
  3. **Mở Terminal chạy lệnh kiểm thử:**
     - Gõ lệnh:
       ```bash
       dotnet test frontend/tests/ExpenseTracker.Maui.UnitTests/ExpenseTracker.Maui.UnitTests.csproj
       ```
     - Nhấn Enter.
     - Quay cận cảnh kết quả terminal:  
       `Passed! - Failed: 0, Passed: 82, Skipped: 0, Total: 82, Duration: 960 ms`.

---

### BƯỚC 6: CHỨC NĂNG BỔ TRỢ ĐẮT GIÁ & KẾT THÚC VIDEO (05:45 – 06:30)
> *Mục đích:* Tạo ấn tượng xuất sắc với Giảng viên chấm thi bằng các tính năng thông minh vượt chuẩn.

* **Các thao tác cần bấm:**
  1. **Demo Quét Hóa Đơn AI Vision OCR (`ReceiptOcrPage`):**
     - Bấm vào biểu tượng Camera/OCR.
     - Bấm chọn 1 ảnh hóa đơn siêu thị mẫu có sẵn trong máy.
     - Bấm **"Quét & Bóc Tách Hóa Đơn"**.
     - Hệ thống bóc tách tự động: Tên cửa hàng, Ngày giờ, danh sách từng món hàng và tự động kiểm toán số học khớp 100%.
     - Bấm **"Lưu Thành Khoản Chi"** -> Khoản chi tự động được lưu vào sổ chi tiêu.
  2. **Demo Báo cáo Phân Bổ 50/30/20 (`ReportsPage`):**
     - Mở trang Báo cáo.
     - Lướt xem thanh phân bổ 3 dải màu: 50% Nhu cầu thiết yếu, 30% Chi tiêu linh hoạt, 20% Tích lũy.
  3. **Kết thúc video:** Quay lại màn hình tổng quan ví, dừng quay video.

---

## BẢNG TỔNG KẾT BẰNG CHỨNG KHỚP BAREM CHẤM THI

| Yêu Cầu Trong Barem Đề Tài 15 | Phân Cảnh Thể Hiện | Bằng Chứng Cụ Thể Trên Video |
| :--- | :--- | :--- |
| **Yêu cầu nền tảng (2.5đ)** | Bước 1 | Đăng nhập backend, Quản lý ví, Thêm khoản chi, Thêm khoản thu, Cập nhật số dư. |
| **Phần A: Vượt ngân sách tháng (2.0đ)** | Bước 2 | Ghi chi tiêu vượt 1.000.000đ -> Bật Native Notification -> Bấm vào nhảy đúng trang Ngân sách (Deep Link). |
| **Phần A: Nhắc ghi chép cuối ngày (2.0đ)** | Bước 3 | Hẹn giờ -> **Vuốt tắt app hoàn toàn (KILLED STATE)** -> Android OS tự đổ chuông báo thức sau 10s. |
| **Phần B2: Đồng bộ ví realtime (+1.0đ)** | Bước 4 | Mở 2 cửa sổ song song -> Chi bên này, bên kia tự nhảy số dư 0ms qua SignalR Hub. |
| **Phần B3: MVVM & Unit Test (2.0đ)** | Bước 5 | Show 3 tầng Model-View-ViewModel, Zero Code-Behind, chạy `dotnet test` **82/82 bài test xanh 100%**. |
| **Sáng tạo & Chất lượng mã (1.0đ)** | Bước 6 | Quét hóa đơn AI Vision OCR, Thước đo phân bổ 50/30/20, 28 Vector icons hoàn toàn không emoji. |
| **TỔNG ĐIỂM DỰ KIẾN** | **10.0 / 10 + 1.0 ĐIỂM CỘNG** | Đáp ứng vượt mức 100% tất cả tiêu chí của giảng viên. |
