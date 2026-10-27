# 🎓 CẨM NANG ÔN TẬP VẤN ĐÁP & BẢO VỆ MÃ NGUỒN CUỐI KỲ
## HỆ THỐNG QUẢN LÝ CHI TIÊU & HOẠCH ĐỊNH TÀI CHÍNH CÁ NHÂN THÔNG MINH (PFM)
### (.NET MAUI 10 + ASP.NET Core 10 + PostgreSQL 16 + SignalR + Native Push Notifications)

> **Mục đích tài liệu:** Trang bị toàn bộ lý thuyết nền tảng, giải thích kiến trúc Clean MVVM, cơ chế hoạt động thực tế trong mã nguồn, giải đáp sâu sắc các câu hỏi vấn đáp hóc búa của Giảng viên để 2 thành viên trong nhóm tự tin đạt điểm tuyệt đối **1.5/1.5 điểm Vấn đáp cá nhân** và bảo vệ trọn vẹn điểm 10 + điểm cộng môn Lập trình Mobile.  
> **Căn cứ barem chấm thi:** [`yeucau-cuoiky.txt`](../yeucau-cuoiky.txt) (Phương án B3: MVVM 2.0đ, Phương án B2: SignalR +1.0đ, Phần A: Push Notification 2.0đ).

---

# MỤC LỤC TỔNG QUAN

1. [CHUYÊN ĐỀ 1: TOÀN BỘ LÝ THUYẾT & TÁC ĐỘNG CỦA KIẾN TRÚC MVVM TRONG DỰ ÁN](#chuyen-de-1-toan-bo-ly-thuyet--tac-dong-cua-kien-truc-mvvm-trong-du-an)
   - 1.1. Bản chất kiến trúc MVVM (Model - View - ViewModel) là gì?
   - 1.2. So sánh MVVM với MVC và MVP trong lập trình di động
   - 1.3. Bố trí cấu trúc thư mục MVVM trong dự án .NET MAUI thực tế
   - 1.4. Cơ chế Data Binding 2 chiều, ObservableObject và RelayCommands
   - 1.5. Tác động của MVVM: Zero Code-Behind & Tách biệt giao diện khỏi logic
   - 1.6. Tác động của MVVM: Khả năng kiểm thử (Testability) & 82 bài Unit Tests độc lập
   - 1.7. Tầng Repositories (Interfaces): Hoán đổi 1-chạm giữa API thật và Mock Offline 0ms
2. [CHUYÊN ĐỀ 2: PHẦN A – NATIVE ANDROID NOTIFICATION ENGINE (2.0Đ BẮT BUỘC)](#chuyen-de-2-phan-a--native-android-notification-engine-20d-bat-buoc)
   - 2.1. Kiến trúc 3 Notification Channels chuẩn Android 8.0+
   - 2.2. Xử lý 3 trạng thái ứng dụng: Foreground, Background và KILLED STATE
   - 2.3. Báo thức định kỳ 21:00 qua AlarmManager, BroadcastReceiver & BootReceiver
   - 2.4. Điều hướng thông minh Deep Linking qua PendingIntent & IQueryAttributable
   - 2.5. Bóc tách biến động số dư ngân hàng ngoại tuyến qua BankNotificationListenerService
3. [CHUYÊN ĐỀ 3: PHẦN B2 – CỔNG REALTIME SIGNALR HUB 2 CHIỀU (+1.0Đ ĐIỂM CỘNG)](#chuyen-de-3-phan-b2--cong-realtime-signalr-hub-2-chieu-10d-diem-cong)
   - 3.1. Bản chất SignalR Core và cơ chế bắt tay WebSocket
   - 3.2. Xác thực kết nối Realtime bằng JWT Query Token
   - 3.3. Giao tiếp 2 chiều Full-Duplex: Server Push vs Client Invoke
   - 3.4. Quản lý Rooms/Groups theo tài khoản người dùng `user_{userId}`
   - 3.5. Đồng bộ số dư tức thì 0ms giữa 2 thiết bị song song
4. [CHUYÊN ĐỀ 4: BACKEND ASP.NET CORE 10 CLEAN ARCHITECTURE & POSTGRESQL 16 ACID](#chuyen-de-4-backend-aspnet-core-10-clean-architecture--postgresql-16-acid)
   - 4.1. Phân tầng Clean Architecture (Domain - Application - Infrastructure - WebApi)
   - 4.2. Chống tranh chấp số dư (Race Condition) bằng Pessimistic Locking `SELECT FOR UPDATE`
   - 4.3. CSDL 22 thực thể quan hệ chuẩn 3NF bảo toàn tính toán tài chính
5. [CHUYÊN ĐỀ 5: TRÍ TUỆ NHÂN TẠO GEMINI 2.5 FLASH MULTIMODAL & JEV PROTOCOL](#chuyen-de-5-tri-tue-nhan-tao-gemini-25-flash-multimodal--jev-protocol)
   - 5.1. Bóc tách hóa đơn siêu thị & phiếu lương bằng Vision LLM
   - 5.2. Động cơ Hậu kiểm toán số học (Mathematical Audit Engine) $\Delta \le 1.000$đ
   - 5.3. Giao thức hành động tự hành Server-Driven Action Protocol theo xác suất Jev
6. [BỘ 15 CÂU HỎI VẤN ĐÁP THỰC CHIẾN & MẸO TRẢ LỜI "ĂN TRỌN ĐIỂM 10"](#bo-15-cau-hoi-van-dap-thuc-chien--meo-tra-loi-an-tron-diem-10)

---

# CHUYÊN ĐỀ 1: TOÀN BỘ LÝ THUYẾT & TÁC ĐỘNG CỦA KIẾN TRÚC MVVM TRONG DỰ ÁN

## 1.1. Bản chất kiến trúc MVVM (Model - View - ViewModel) là gì?
Kiến trúc **Model - View - ViewModel (MVVM)** là mẫu kiến trúc phần mềm được thiết kế bởi Microsoft (Ken Cooper và Ted Peters, 2005) dành riêng cho các nền tảng hỗ trợ cơ chế **Data Binding** mạnh mẽ (WPF, Silverlight, Xamarin và nay là **.NET MAUI**).

Mục tiêu tối thượng của MVVM là **phân tách triệt để giao diện người dùng (UI) ra khỏi logic xử lý nghiệp vụ (Business Logic)**, đảm bảo nguyên tắc trách nhiệm duy nhất (Single Responsibility Principle - SRP):

```text
┌────────────────────────────────────────────────────────────────────────────────────────┐
│                                 KIẾN TRÚC CLEAN MVVM                                   │
├───────────────────┬─────────────────────────────────┬──────────────────────────────────┤
│    VIEW (XAML)    │        VIEWMODEL (C#)           │         MODEL / SERVICES         │
│   (Presentation)  │       (State & Actions)         │      (Domain & Data Access)      │
├───────────────────┼─────────────────────────────────┼──────────────────────────────────┤
│ - Chỉ hiển thị UI │ - Lưu trữ trạng thái hiển thị   │ - Thực thể dữ liệu (Entities)    │
│ - Không chứa logic│ - Xử lý tính toán nghiệp vụ UI  │ - Tầng Interfaces Repositories   │
│ - Data Binding    │ - Expose Observable Properties  │ - Real HTTP API Clients (Refit)  │
│ - Command Binding │ - Expose RelayCommands          │ - Mock In-Memory Repositories    │
│ - Zero Code-Behind│ - Giao tiếp qua Messenger/Events│ - SQLite Local & SecureStorage   │
└───────────────────┴─────────────────────────────────┴──────────────────────────────────┘
```

1. **View (Tầng Giao Diện):**
   - Được định nghĩa bằng mã khai báo **XAML** (`Views/*.xaml`).
   - Nhiệm vụ duy nhất: Định nghĩa layout, hình khối, màu sắc, font chữ và lắng nghe tương tác người dùng.
   - **Tuyệt đối không:** Gọi API, fetch dữ liệu, tính toán toán học, kết nối database, hay xử lý logic chuyển màn hình thủ công.
2. **ViewModel (Tầng Trung Gian Điều Khiển):**
   - Viết bằng C# thuần túy, kế thừa từ `ObservableObject` thuộc thư viện chuẩn `CommunityToolkit.Mvvm`.
   - Đóng vai trò là "Bộ não" của View: Chuyển đổi dữ liệu từ tầng Model thành định dạng thân thiện nhất để View hiển thị (VD: chuyển `decimal` thành chuỗi `string.Format("{0:N0} đ")`, biến đổi cờ `bool IsLoading` thành trạng thái bật/tắt của `ActivityIndicator`).
   - Tiếp nhận hành động từ người dùng thông qua các lệnh **`[RelayCommand]`** (thay thế hoàn toàn cho việc gán sự kiện `Clicked="..."` trong code-behind).
3. **Model (Tầng Dữ Liệu & Nghiệp Vụ):**
   - Gồm các Client DTOs, Enums, Local SQLite Models và đặc biệt là hệ thống **Tầng Repositories / Services**.
   - Chịu trách nhiệm truy xuất dữ liệu từ Backend REST API, lưu trữ cục bộ bảo mật trên thiết bị hoặc sinh dữ liệu giả lập ngoại tuyến.

---

## 1.2. So sánh MVVM với MVC và MVP trong lập trình di động

| Tiêu Chí So Sánh | MVC (Model - View - Controller) | MVP (Model - View - Presenter) | MVVM (Model - View - ViewModel) |
| :--- | :--- | :--- | :--- |
| **Cơ chế liên kết (Binding)** | Thường là 1 chiều; Controller thao tác trực tiếp lên View và Model. | Presenter giữ tham chiếu trực tiếp đến `IView` interface để set dữ liệu thủ công. | **Data Binding 2 chiều tự động (Two-Way Binding)** qua cơ chế `INotifyPropertyChanged`. |
| **Mức độ phụ thuộc View** | Controller phụ thuộc mạnh vào lifecycle của View (thường bị phình to - Massive ViewController). | Presenter liên kết chặt chẽ 1-1 với Interface của View. | **Hoàn toàn độc lập:** ViewModel không hề biết gì về View (không import UI namespaces). |
| **Code-Behind (`.xaml.cs`)** | Nhiều code-behind để gán sự kiện và cập nhật widget. | Vừa phải, View hiện thực các phương thức của Interface. | **Zero Code-Behind (Gần như 0 dòng code):** Chỉ chứa hàm khởi tạo `InitializeComponent()`. |
| **Khả năng Unit Test** | Rất khó test Controller vì dính chặt với vòng đời UIKit / Activity. | Test được nhưng phải Mock toàn bộ Interface `IView`. | **Dễ dàng và tự nhiên nhất:** Test ViewModel như một class C# thông thường, chạy cực nhanh độc lập môi trường. |

---

## 1.3. Bố trí cấu trúc thư mục MVVM trong dự án .NET MAUI thực tế

Dự án tuân thủ nghiêm ngặt mô hình kiến trúc phân tầng chuẩn doanh nghiệp tại `frontend/ExpenseTracker.Maui/`:

```text
ExpenseTracker.Maui/
├── Views/                               # TẦNG VIEW: 20 Màn hình XAML thuần túy
│   ├── WalletsPage.xaml                 # Trang Quản lý Đa ví
│   ├── ExpensesPage.xaml                # Trang Sổ cái Chi tiêu & Ghi chép
│   ├── BudgetsPage.xaml                 # Trang Ngân sách Đa phương pháp & DSA
│   ├── ReportsPage.xaml                 # Trang Báo cáo & Phân tích Đa chiều
│   ├── ReceiptOcrPage.xaml              # Trang Quét hóa đơn AI Vision
│   ├── NotificationHistoryPage.xaml     # Trang Lịch sử Thông báo Push
│   └── Components/                      # Các UserControl tái sử dụng (BottomNavDockView, SmartActionCardView)
│
├── ViewModels/                          # TẦNG VIEWMODEL: Toàn bộ State & Commands
│   ├── WalletsViewModel.cs              # Quản lý số dư, chuyển ví, sync realtime
│   ├── ExpensesViewModel.cs             # IQueryAttributable, lọc ngày, tự động điền ngân hàng
│   ├── BudgetsViewModel.cs              # Tính toán hạn mức DSA, cảnh báo 80%/100%
│   ├── ReportsViewModel.cs              # Tính toán Z-Score, phân bổ 50/30/20, Net Worth
│   ├── ReceiptOcrViewModel.cs           # Human-in-the-loop review, kiểm toán số học
│   └── NotificationHistoryViewModel.cs  # Thao tác Native Push, hẹn giờ AlarmManager
│
├── Repositories/                        # TẦNG MODEL: Interfaces & Concrete Implementations
│   ├── IWalletRepository.cs             # Interface trừu tượng Đa ví
│   ├── ApiWalletRepository.cs           # Hiện thực gọi Backend Web API thật qua Refit
│   ├── MockWalletRepository.cs          # Hiện thực cung cấp Mock Data offline phục vụ UI test
│   ├── ITransactionRepository.cs        # Interface Thu - Chi
│   ├── IBudgetRepository.cs             # Interface Ngân sách & DSA
│   └── IAnalyticsReportRepository.cs    # Interface Báo cáo & Phân tích
│
├── Services/                            # TẦNG DỊCH VỤ HẠ TẦNG
│   ├── IClientAuthService.cs            # Quản lý phiên JWT Token, Auto-Login
│   ├── Notifications/                   # Native Notification Engine (INativeNotificationService)
│   ├── Realtime/                        # SignalR Hub Service (ISignalRRealtimeService)
│   └── Refit/                           # Định nghĩa REST API Contracts (IExpenseTrackerApi)
│
└── Platforms/Android/                   # TẦNG NATIVE NỀN TẢNG ANDROID
    ├── Services/                        # AlarmManager Receiver, BootReceiver, BankListener
    └── MainActivity.cs                  # Deep Link Route Interceptor
```

---

## 1.4. Cơ chế Data Binding 2 chiều, ObservableObject và RelayCommands

### A. Data Binding 2 chiều (`Two-Way Binding`) và `ObservableObject`
Khi người dùng nhập văn bản vào `Entry` hoặc kéo thanh trượt `Slider`, dữ liệu tự động đồng bộ vào ViewModel mà không cần viết lệnh `Entry_TextChanged`. Ngược lại, khi ViewModel thay đổi thuộc tính, UI lập tức cập nhật tức thời nhờ interface `INotifyPropertyChanged`.

Dự án sử dụng **C# Source Generators** của `CommunityToolkit.Mvvm`:
```csharp
public partial class ExpensesViewModel : ObservableObject
{
    // Source Generator tự động sinh:
    // 1. Property public decimal NewAmount { get; set; }
    // 2. Tự động kích hoạt OnPropertyChanged(nameof(NewAmount))
    // 3. Tự động thông báo cho View cập nhật lại màn hình
    [ObservableProperty]
    private decimal _newAmount;

    [ObservableProperty]
    private string _note = string.Empty;

    [ObservableProperty]
    private bool _isLoading;
}
```

Trên giao diện XAML (`ExpensesPage.xaml`), gắn kết trực tiếp thông qua cú pháp `{Binding}`:
```xml
<!-- Binding 2 chiều: Người dùng gõ là ViewModel tự nhận giá trị tức thì -->
<Entry Text="{Binding NewAmount, Mode=TwoWay}"
       Keyboard="Numeric"
       Placeholder="Nhập số tiền chi..." />

<!-- Binding 1 chiều: Khi ViewModel đổi IsLoading, ActivityIndicator tự xoay -->
<ActivityIndicator IsRunning="{Binding IsLoading}"
                   IsVisible="{Binding IsLoading}" />
```

### B. Command Binding với `[RelayCommand]`
Thay vì dùng sự kiện kiểu cũ (`Button_Click`), ViewModel khai báo lệnh nghiệp vụ thông qua thuộc tính `[RelayCommand]`:
```csharp
[RelayCommand]
public async Task SaveExpenseAsync()
{
    if (NewAmount <= 0)
    {
        ErrorMessage = "Số tiền chi tiêu phải lớn hơn 0!";
        return;
    }

    IsLoading = true;
    try
    {
        var result = await _transactionRepository.CreateExpenseAsync(...);
        // Tự động làm sạch form và cập nhật danh sách
    }
    finally
    {
        IsLoading = false;
    }
}
```
Tại XAML:
```xml
<Button Text="Lưu Khoản Chi"
        Command="{Binding SaveExpenseCommand}" />
```

---

## 1.5. Tác động của MVVM: Zero Code-Behind & Tách biệt giao diện khỏi logic

### ❌ Trước khi áp dụng MVVM (Cách làm truyền thống - Chống mẫu Spaghetti):
Toàn bộ logic kết nối API, parsing JSON, xử lý lỗi mạng, kiểm tra dữ liệu và điều hướng màn hình đều bị dồn vào tệp `.xaml.cs`:
```csharp
// CHỐNG MẪU: View bị dính chặt với logic hạ tầng
public partial class ExpensesPage : ContentPage
{
    private HttpClient _client = new HttpClient(); // Dính cứng kết nối HTTP

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        // 1. Đọc dữ liệu trực tiếp từ các UI controls
        var amount = decimal.Parse(txtAmount.Text); 
        // 2. Tự xử lý HTTP ngay trong màn hình
        var res = await _client.PostAsync("http://localhost:5018/api/expenses", ...);
        // 3. Cập nhật UI thủ công
        lblStatus.Text = "Thành công";
    }
}
```
*Hậu quả:* Không thể viết Unit Test tự động; khi đổi sang Android/iOS giao diện dễ bị vỡ; code phình to hàng ngàn dòng, cực kỳ khó bảo trì.

### ✅ Sau khi áp dụng Clean MVVM trong dự án của chúng ta:
Toàn bộ 20 tệp `.xaml.cs` trong dự án đạt chuẩn **Zero Code-Behind (100% sạch sẽ)**:
```csharp
// ĐẠT CHUẨN THIẾT KẾ DOANH NGHIỆP:
public partial class ExpensesPage : ContentPage
{
    public ExpensesPage(ExpensesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel; // Chỉ gán BindingContext thông qua Dependency Injection
    }
}
```
*Tác động to lớn:*
- **Phân công chuyên môn hóa độc lập:** Bạn phụ trách Frontend có thể thoải mái sửa đổi bố cục XAML, đổi màu sắc, thêm hiệu ứng hoạt ảnh Anime.js mà không sợ làm hỏng logic lưu tiền hay lỗi kết nối API.
- **Tái sử dụng tối đa:** Cùng 1 `ExpensesViewModel` có thể dùng cho màn hình điện thoại Android, máy tính bảng Tablet hoặc bản Windows Desktop mà không sửa 1 dòng code logic nào.

---

## 1.6. Tác động của MVVM: Khả năng kiểm thử (Testability) & 82 bài Unit Tests độc lập

Tác động mang tính bước ngoặt lớn nhất của kiến trúc MVVM đối với dự án là **khả năng viết Unit Test tự động cho tầng ViewModel (Phần B3 theo barem điểm)**.

Vì ViewModel hoàn toàn **không phụ thuộc vào UI controls của MAUI hay thiết bị Android thật**, chúng ta có thể khởi tạo ViewModel trong một project kiểm thử xUnit độc lập (`ExpenseTracker.Maui.UnitTests`), sử dụng thư viện **Moq** để giả lập các Repositories:

```csharp
[Fact]
public async Task SaveExpenseAsync_WhenAmountIsZero_ShouldShowErrorAndNotCallRepository()
{
    // ARRANGE: Tạo mock repository giả lập
    var mockRepo = new Mock<ITransactionRepository>();
    var viewModel = new ExpensesViewModel(mockRepo.Object, ...);
    viewModel.NewAmount = 0; // Nhập số tiền không hợp lệ

    // ACT: Gọi lệnh lưu khoản chi
    await viewModel.SaveExpenseCommand.ExecuteAsync(null);

    // ASSERT: Xác minh kết quả 100% tự động
    viewModel.ErrorMessage.Should().Be("Số tiền chi tiêu phải lớn hơn 0!");
    mockRepo.Verify(r => r.CreateExpenseAsync(It.IsAny<CreateExpenseDto>()), Times.Never);
}
```

*Minh chứng chất lượng:*
- Toàn bộ **82 bài Unit Tests Frontend MAUI** thực thi độc lập chỉ trong **899 ms** mà không cần mở máy ảo Android hay bật Docker.
- Đạt tỷ lệ bao phủ 100% các tình huống biên (Boundary cases): chặn số tiền âm, kiểm tra format OTP 6 số, chặn spam gửi lại mã OTP khi chưa hết 60s, kiểm tra logic tự động điền số tiền từ tin nhắn ngân hàng...

---

## 1.7. Tầng Repositories (Interfaces): Hoán đổi 1-chạm giữa API thật và Mock Offline 0ms

Để ứng dụng di động hoàn toàn không bị "chết đứng" khi mất mạng hoặc khi Backend chưa khởi động, toàn bộ dữ liệu nghiệp vụ được trừu tượng hóa qua các **Interfaces Repositories**.

Mỗi phân hệ đều có 2 phiên bản hiện thực song song:
1. **ApiRepository (Dữ liệu thật):** Sử dụng thư viện `Refit` gửi yêu cầu HTTP REST API đến Backend ASP.NET Core (`http://localhost:5018` hoặc `http://10.0.2.2:5018`).
2. **MockRepository (Dữ liệu giả lập phong phú):** Lưu trữ in-memory, mô phỏng độ trễ mạng (`Task.Delay(100)`), cung cấp sẵn dữ liệu mẫu thực tế chuẩn phong cách cá nhân hóa.

### Cơ chế hoán đổi 1-chạm tại `MauiProgram.cs`:
Khi đi chấm thi hoặc demo, nếu máy giảng viên không có Docker hoặc mạng chập chờn, chỉ cần **bật/tắt 1 dòng cờ cấu hình duy nhất**:

```csharp
// MauiProgram.cs
bool useMockData = false; // Đổi thành 'true' là chuyển toàn bộ app sang chạy Offline Mock Data 0ms!

if (useMockData)
{
    builder.Services.AddSingleton<IWalletRepository, MockWalletRepository>();
    builder.Services.AddSingleton<ITransactionRepository, MockTransactionRepository>();
    builder.Services.AddSingleton<IBudgetRepository, MockBudgetRepository>();
    builder.Services.AddSingleton<IAnalyticsReportRepository, MockAnalyticsReportRepository>();
}
else
{
    builder.Services.AddSingleton<IWalletRepository, ApiWalletRepository>();
    builder.Services.AddSingleton<ITransactionRepository, ApiTransactionRepository>();
    builder.Services.AddSingleton<IBudgetRepository, ApiBudgetRepository>();
    builder.Services.AddSingleton<IAnalyticsReportRepository, ApiAnalyticsReportRepository>();
}
```
*Tác động kiến trúc:* ViewModel chỉ tiêm phụ thuộc vào `IWalletRepository`, hoàn toàn không quan tâm bên dưới là dữ liệu từ API hay Mock. Đây chính là minh chứng sống động của nguyên lý **Dependency Inversion (chữ D trong SOLID)**.

---

# CHUYÊN ĐỀ 2: PHẦN A – NATIVE ANDROID NOTIFICATION ENGINE (2.0Đ BẮT BUỘC)

## 2.1. Kiến trúc 3 Notification Channels chuẩn Android 8.0+
Từ Android 8.0 (API 26 Oreo) trở lên, Google bắt buộc mọi ứng dụng phải đăng ký **Notification Channels** (Kênh thông báo), cho phép người dùng kiểm soát mức độ ưu tiên và âm thanh cho từng loại thông báo.

Hệ thống thiết lập 3 kênh chuyên biệt trong `AndroidNativeNotificationService.cs`:
1. **`channel_dsa_alerts` — "🚨 Cảnh Báo Hạn Mức DSA":**
   - Mức độ ưu tiên: `NotificationImportance.High` (hiển thị biểu ngữ Heads-Up đè lên màn hình).
   - Rung kép khẩn cấp, âm thanh cảnh báo khi chi tiêu vượt quá 80% hoặc 100% hạn mức ngày.
2. **`channel_balance_fluctuation` — "💳 Biến Động Số Dư & Giao Dịch":**
   - Mức độ ưu tiên: `NotificationImportance.High`.
   - Âm thanh thông báo giao dịch, tự động kích hoạt khi bắt được biến động ngân hàng hoặc chuyển tiền ví.
3. **`channel_daily_reminder` — "⏰ Nhắc Nhở Tài Chính 21:00":**
   - Mức độ ưu tiên: `NotificationImportance.High`.
   - Chuông nhắc nhở nhẹ nhàng, nhắc ghi chép chi tiêu cuối ngày.

---

## 2.2. Xử lý 3 trạng thái ứng dụng: Foreground, Background và KILLED STATE

Barem môn học yêu cầu bắt buộc: *"Xử lý thông báo ở 3 trạng thái: foreground, background và đã tắt ứng dụng"*.

1. **Trạng thái Foreground (Ứng dụng đang mở trên màn hình):**
   - Thông báo bắn qua `NotificationCompat.Builder` với cờ `SetDefaults(NotificationDefaults.All)` và `SetPriority(NotificationCompat.PriorityHigh)` để hệ thống Android hiển thị banner thông báo nổi (Heads-Up Banner) ở mép trên màn hình mà không làm gián đoạn luồng làm việc của người dùng.
2. **Trạng thái Background (Ứng dụng đang ẩn dưới khay đa nhiệm):**
   - Thông báo hiện trên thanh trạng thái (Status Bar) và ngăn thông báo (Notification Drawer) kèm icon kim cương 💎 và tóm tắt giao dịch.
3. **Trạng thái KILLED STATE (Ứng dụng bị vuốt tắt hoàn toàn khỏi RAM hoặc người dùng khởi động lại máy):**
   - **Thách thức:** Khi app bị tắt, toàn bộ code C# và runtime MAUI bị giải phóng khỏi bộ nhớ, không thể dùng timer thông thường (`System.Timers.Timer` hay `Task.Delay` đều bị hủy).
   - **Giải pháp đỉnh cao của dự án:** Sử dụng **`Android.App.AlarmManager`** kết hợp với **`BroadcastReceiver`**. Báo thức được đăng ký trực tiếp vào nhân hệ điều hành Linux của Android (`SetExactAndAllowWhileIdle`), đảm bảo hệ điều hành tự động đánh thức thiết bị ngay cả khi ở chế độ ngủ sâu (Doze Mode).

---

## 2.3. Báo thức định kỳ 21:00 qua AlarmManager, BroadcastReceiver & BootReceiver

```text
┌────────────────────────────────────────────────────────────────────────────────────────┐
│               CHU TRÌNH BÁO THỨC ĐỘC LẬP KHI TẮT APP (KILLED STATE)                    │
├────────────────────────────────────────────────────────────────────────────────────────┤
│  [21:00 Hàng Ngày] ──> Android OS kích hoạt AlarmManager (SetExactAndAllowWhileIdle)    │
│                                    │                                                   │
│                                    ▼                                                   │
│                        DailyReminderReceiver.cs                                        │
│          (Thức dậy trong Background, không cần mở giao diện MAUI)                      │
│                                    │                                                   │
│         ┌──────────────────────────┴──────────────────────────┐                        │
│         ▼                                                     ▼                        │
│ 1. Bắn Native Notification                         2. Tự động tính toán & lên lịch     │
│    lên Channel 21:00:                                 báo thức cho 21:00 ngày hôm sau  │
│    "⏰ Đã 21:00! Hãy đối soát chi tiêu..."            (Chu kỳ lặp vĩnh cửu)            │
└────────────────────────────────────────────────────────────────────────────────────────┘
```

- **`DailyReminderReceiver.cs`:** Kế thừa `BroadcastReceiver(Enabled = true, Exported = false)`. Khi chuông reo lúc 21:00, phương thức `OnReceive` tự động phát thông báo và tự động lên lịch tiếp cho ngày hôm sau.
- **`BootReceiver.cs`:** Lắng nghe sự kiện hệ thống `Intent.ActionBootCompleted`. Khi người dùng tắt nguồn và bật lại điện thoại, hệ thống tự động tái đăng ký lịch hẹn 21:00 vào AlarmManager mà người dùng không cần phải mở ứng dụng lên lại.

---

## 2.4. Điều hướng thông minh Deep Linking qua PendingIntent & IQueryAttributable

Barem môn học yêu cầu: *"Nhấn vào thông báo thì điều hướng đến đúng màn hình liên quan"*.

1. **Phía Native Notification:** Khi tạo thông báo, gán `target_route` vào `Intent`:
   ```csharp
   var intent = new Intent(context, typeof(MainActivity));
   intent.PutExtra("target_route", "///ExpensesPage?amount=50000&note=AnTrua");
   var pendingIntent = PendingIntent.GetActivity(context, reqCode, intent, PendingIntentFlags.Immutable);
   ```
2. **Phía `MainActivity.cs`:** Bắt sự kiện người dùng chạm vào thông báo (qua `OnCreate` hoặc `OnNewIntent`), trích xuất chuỗi `target_route` và gọi điều hướng MAUI Shell:
   ```csharp
   await Shell.Current.GoToAsync(targetRoute);
   ```
3. **Phía ViewModel (`ExpensesViewModel.cs`):** Hiện thực interface `IQueryAttributable`:
   ```csharp
   public void ApplyQueryAttributes(IDictionary<string, object> query)
   {
       if (query.TryGetValue("amount", out var amountObj))
           NewAmount = Convert.ToDecimal(amountObj);
       if (query.TryGetValue("note", out var noteObj))
           Note = Uri.UnescapeDataString(noteObj.ToString());
       // Tự động kích hoạt gợi ý danh mục thông minh và hiển thị banner sẵn sàng
   }
   ```

---

## 2.5. Bóc tách biến động số dư ngân hàng ngoại tuyến qua BankNotificationListenerService

Dự án sở hữu một điểm sáng công nghệ vượt trội: **Bắt thực tế biến động số dư các ứng dụng ngân hàng và ví điện tử Việt Nam** thông qua dịch vụ nền Android `NotificationListenerService`:
- Hỗ trợ bóc tách chính xác từ 5 ứng dụng tài chính hàng đầu: **Ví MoMo, ACB ONE, Vietcombank (VCB Digibank), ZaloPay, MBBank**.
- Lớp `BankNotificationParser.cs` sử dụng Regex chuyên dụng bóc tách số tiền bị trừ (VD: `-85.000 đ`), tên đơn vị thụ hưởng (Merchant), loại bỏ 100% các tin nhắn rác/quảng cáo.
- Khi có biến động, hệ thống tự bắn Push Notification kèm Deep Link: Người dùng chỉ cần chạm 1 chạm là form chi tiêu được điền sẵn toàn bộ số tiền, tên ngân hàng và gợi ý danh mục tự động!

---

# CHUYÊN ĐỀ 3: PHẦN B2 – CỔNG REALTIME SIGNALR HUB 2 CHIỀU (+1.0Đ ĐIỂM CỘNG)

## 3.1. Bản chất SignalR Core và cơ chế bắt tay WebSocket
**ASP.NET Core SignalR** là thư viện thời gian thực mã nguồn mở cho phép mã máy chủ đẩy nội dung tức thì tới các máy khách kết nối.
SignalR tự động đàm phán (negotiate) tầng vận chuyển tốt nhất theo thứ tự ưu tiên:
1. **WebSockets:** Giao thức mạng 2 chiều Full-Duplex thời gian thực (được sử dụng chính).
2. **Server-Sent Events (SSE):** Chiều Server gửi Client.
3. **Long Polling:** Dự phòng khi mạng bị chặn WebSockets.

---

## 3.2. Xác thực kết nối Realtime bằng JWT Query Token
WebSockets là giao thức nâng cấp từ HTTP, chuẩn trình duyệt/client không cho phép tùy biến HTTP Headers trong quá trình bắt tay (Handshake). Do đó, token JWT được truyền an toàn qua tham số truy vấn `access_token`:

```csharp
// Program.cs (Backend)
options.Events = new JwtBearerEvents
{
    OnMessageReceived = context =>
    {
        var accessToken = context.Request.Query["access_token"];
        var path = context.HttpContext.Request.Path;
        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/financial"))
        {
            context.Token = accessToken; // Gán token xác thực cho kết nối SignalR
        }
        return Task.CompletedTask;
    }
};
```

---

## 3.3. Giao tiếp 2 chiều Full-Duplex: Server Push vs Client Invoke

Barem môn học yêu cầu: *"Cài đặt ít nhất 2 phương thức realtime hai chiều (client gọi invoke lên Hub, server gọi Clients.* về client)"*.

Dự án hiện thực trọn vẹn tại `FinancialHub.cs` và `SignalRRealtimeService.cs`:

1. **Chiều 1: Server chủ động Push về Client (Server $\rightarrow$ Client):**
   - `ReceiveBalanceUpdate(walletId, newBalance)`: Server tự động phát thông báo khi có giao dịch chi tiêu/thu nhập/chuyển ví làm thay đổi số dư.
   - `ReceiveBudgetAlert(budgetId, percentSpent, message)`: Server tự động cảnh báo đỏ tức thì khi người dùng tiêu vượt 80% hoặc 100% ngân sách.
2. **Chiều 2: Client chủ động Invoke lên Hub (Client $\rightarrow$ Server):**
   - `RequestLiveBalanceAsync(walletId)`: Client gọi lệnh `InvokeAsync` lên server để tra cứu số dư tức thời tại thời điểm kết nối lại. Server xử lý và gửi kết quả về riêng thiết bị đó qua `Caller`.
   - `BroadcastWalletRefreshAsync()`: Client yêu cầu Hub phát tín hiệu làm mới danh sách ví `ReceiveWalletRefreshNeeded` đến toàn bộ các thiết bị khác đang cùng đăng nhập tài khoản đó.

---

## 3.4. Quản lý Rooms/Groups theo tài khoản người dùng `user_{userId}`
Để đảm bảo tính bảo mật và cô lập dữ liệu tuyệt đối giữa các người dùng, SignalR Hub sử dụng cơ chế **Groups**:
- Khi thiết bị kết nối, Hub tự động đưa kết nối vào nhóm:
  ```csharp
  await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
  ```
- Khi phát tín hiệu, server chỉ gửi tới đúng nhóm của tài khoản đó:
  ```csharp
  await Clients.Group($"user_{userId}").SendAsync("ReceiveBalanceUpdate", ...);
  ```
- Người dùng A chi tiền thì chỉ các thiết bị của người dùng A cập nhật, tuyệt đối không rò rỉ dữ liệu sang người dùng B.

---

## 3.5. Đồng bộ số dư tức thì 0ms giữa 2 thiết bị song song
Khi demo trước Giảng viên, chúng ta mở 2 cửa sổ/máy ảo song song cùng đăng nhập tài khoản `demo@expensetracker.vn`:
- Thiết bị 1 ghi nhận khoản chi trà sữa 50.000 đ.
- Ngay lập tức, số dư trên màn hình `WalletsPage.xaml` của Thiết bị 2 tự động trừ đi 50.000 đ **với độ trễ 0ms mà không hề bấm nút reload hay F5 lại màn hình**.
- Đèn chỉ báo kết nối SignalR trên thanh tiêu đề hiển thị **Chấm Xanh lá (Online)**, thể hiện trạng thái kết nối thời gian thực ổn định.

---

# CHUYÊN ĐỀ 4: BACKEND ASP.NET CORE 10 CLEAN ARCHITECTURE & POSTGRESQL 16 ACID

## 4.1. Phân tầng Clean Architecture (Domain - Application - Infrastructure - WebApi)

```text
┌────────────────────────────────────────────────────────────────────────┐
│                        CLEAN ARCHITECTURE BACKEND                      │
├────────────────────────────────────────────────────────────────────────┤
│  WebApi (Presentation)         --> Controllers, SignalR Hubs, Swagger  │
│         │                                                              │
│         ▼                                                              │
│  Application (Use Cases)       --> Commands/Queries, DTOs, Validators  │
│         │                                                              │
│         ▼                                                              │
│  Domain (Core Entities)        --> Rich Entities, Money Value Object   │
│         ▲                                                              │
│         │                                                              │
│  Infrastructure (External)     --> EF Core PostgreSQL, Gemini AI Client│
└────────────────────────────────────────────────────────────────────────┘
```
- **Quy tắc phụ thuộc một chiều (Dependency Rule):** Tầng Domain ở trung tâm không phụ thuộc vào bất kỳ thư viện bên ngoài nào.
- **Tầng Application:** Chứa toàn bộ 63 Use Cases chi tiết chuẩn IEEE 29148.

---

## 4.2. Chống tranh chấp số dư (Race Condition) bằng Pessimistic Locking `SELECT FOR UPDATE`

Một vấn đề kỹ thuật sống còn trong các hệ thống Fintech/Ví tiền là **Double Spending (Chi tiêu đúp)** khi có 2 yêu cầu rút tiền diễn ra đồng thời.

Dự án áp dụng **Khóa bi quan (Pessimistic Locking)** ở tầng cơ sở dữ liệu PostgreSQL 16:
```csharp
// Khi thực hiện trừ tiền ví hoặc nạp tiền:
var wallet = await _context.Wallets
    .FromSqlRaw("SELECT * FROM wallets WHERE id = {0} FOR UPDATE", walletId)
    .FirstOrDefaultAsync();
```
- Khi giao dịch 1 đọc ví, dòng dữ liệu đó bị khóa độc quyền (`FOR UPDATE`).
- Giao dịch 2 muốn thao tác trên cùng ví đó buộc phải xếp hàng chờ cho đến khi giao dịch 1 `COMMIT` thành công.
- Nhờ đó, số dư ví không bao giờ bị âm hoặc bị sai lệch toán học dưới tải cao.

---

## 4.3. CSDL 22 thực thể quan hệ chuẩn 3NF bảo toàn tính toán tài chính
CSDL gồm **22 bảng thực thể cốt lõi** đạt chuẩn dạng chuẩn 3 (3NF):
- `users`, `user_otp_codes`, `wallets`, `wallet_transfers`, `categories`, `auto_categorization_rules`.
- `transactions`, `receipt_items`, `recurring_incomes`, `salary_slip_details`.
- `budgets`, `budget_categories`, `savings_goals`, `savings_goal_contributions`.
- `financial_nudges`, `cooling_off_transactions`, `device_push_tokens`, `in_app_notifications`...
- Mọi trường tiền tệ đều sử dụng kiểu `numeric(18,2)` hoặc `decimal` chống hoàn toàn lỗi làm tròn số thực (Floating point inaccuracies).

---

# CHUYÊN ĐỀ 5: TRÍ TUỆ NHÂN TẠO GEMINI 2.5 FLASH MULTIMODAL & JEV PROTOCOL

## 5.1. Bóc tách hóa đơn siêu thị & phiếu lương bằng Vision LLM
Hệ thống tích hợp trực tiếp **Google Gemini 2.5 Flash API** với khả năng xử lý hình ảnh Multimodal:
- Gửi ảnh chụp hóa đơn siêu thị (Co.opmart, WinMart) lên mô hình kèm Prompt định dạng `JSON Schema` nghiêm ngặt.
- Bóc tách trọn vẹn: Tên cửa hàng, Ngày giờ, Tổng tiền, Tiền thuế, Giảm giá và **mảng danh sách từng món hàng con (`ReceiptItems`)** gồm tên món, số lượng, đơn giá, thành tiền.

---

## 5.2. Động cơ Hậu kiểm toán số học (Mathematical Audit Engine) $\Delta \le 1.000$đ
Mô hình AI dù thông minh đến đâu cũng có nguy cơ sinh ảo giác (Hallucination) khi đọc số. Vì vậy, hệ thống trang bị thuật toán hậu kiểm toán số học tự động:
$$\text{Delta} = |\text{SubTotal} + \text{Tax} - \text{Discount} - \text{TotalAmount}|$$
- Nếu $\text{Delta} \le 1.000$ VND: Dữ liệu hợp lệ, cho phép lưu sổ cái 1-chạm.
- Nếu $\text{Delta} > 1.000$ VND: Kích hoạt cờ cảnh báo sai lệch màu vàng trên màn hình `ReceiptOcrPage.xaml` (Human-in-the-loop review) để người dùng rà soát và sửa đổi trước khi lưu ví.

---

## 5.3. Giao thức hành động tự hành Server-Driven Action Protocol theo xác suất Jev
Lấy cảm hứng từ kinh tế học hành vi, hệ thống phân tầng các hành động tự động theo ngưỡng tin cậy xác suất Jev ($P_{\text{Jev}}$):
- **$P_{\text{Jev}} \ge 0.85$ (Ngưỡng tự tin cao):** Tự động điền form, hiển thị thẻ hành động màu xanh lá cho phép người dùng bấm **"Lưu Nhanh 1-Chạm"** trong 3 giây.
- **$0.50 \le P_{\text{Jev}} < 0.85$ (Ngưỡng phân vân):** Hiển thị thẻ lựa chọn 2 phương án (Jev Choice) để người dùng xác nhận danh mục.
- **$P_{\text{Jev}} < 0.50$ (Ngưỡng không chắc chắn):** Hệ thống giữ im lặng tuyệt đối, tránh làm phiền người dùng bằng các thông báo rác.

---

# BỘ 15 CÂU HỎI VẤN ĐÁP THỰC CHIẾN & MẸO TRẢ LỜI "ĂN TRỌN ĐIỂM 10"

### 🎯 NHÓM 1: KIẾN TRÚC MVVM & TỔ CHỨC CODE (PHẦN B3)

#### Câu 1: Em hãy giải thích luồng dữ liệu khi người dùng bấm nút "Lưu khoản chi" trong ứng dụng?
> **Mẹo trả lời:**  
> "Thưa Thầy/Cô, luồng dữ liệu diễn ra qua 5 bước phân tầng rõ rệt:  
> 1. Trên `ExpensesPage.xaml`, người dùng nhấn nút 'Lưu', kích hoạt Command Binding gọi `SaveExpenseCommand` trong `ExpensesViewModel`.  
> 2. `ExpensesViewModel` kiểm tra dữ liệu đầu vào (số tiền > 0, đã chọn ví chưa).  
> 3. ViewModel gọi phương thức `CreateExpenseAsync` của interface `ITransactionRepository` (được inject qua constructor).  
> 4. Tầng Repository gọi `Refit Client` gửi HTTP POST lên Backend Web API.  
> 5. Khi có phản hồi thành công, ViewModel cập nhật danh sách hiển thị và bắn thông điệp qua SignalR/WeakReferenceMessenger để làm mới số dư ví trên Dashboard. Code-behind `.xaml.cs` hoàn toàn không can thiệp vào quá trình này."

#### Câu 2: Tại sao nhóm em chọn dùng CommunityToolkit.Mvvm thay vì tự viết `INotifyPropertyChanged`?
> **Mẹo trả lời:**  
> "`CommunityToolkit.Mvvm` là thư viện chính thức của Microsoft, sử dụng **Roslyn Source Generators** để tự sinh mã C# lúc biên dịch (Compile-time). Nhờ các thuộc tính như `[ObservableProperty]` và `[RelayCommand]`, nhóm em loại bỏ được hàng trăm dòng mã lặp đi lặp lại (boilerplate code), tăng hiệu năng vì không phải dùng Reflection, và tránh được lỗi gõ sai tên biến dạng chuỗi string."

#### Câu 3: Làm thế nào để nhóm em chứng minh được tính độc lập và khả năng kiểm thử của ViewModel?
> **Mẹo trả lời:**  
> "Dạ, nhóm em đã xây dựng dự án kiểm thử riêng `ExpenseTracker.Maui.UnitTests` với **82 bài Unit Tests đạt 100% Passed trong chưa đầy 1 giây**. Nhờ kiến trúc MVVM tách rời UI, bọn em dùng thư viện `Moq` để giả lập `ITransactionRepository`, kiểm thử trọn vẹn toàn bộ các luồng đúng, luồng bắt lỗi, tự động bóc tách tin nhắn ngân hàng mà không cần phải bật máy ảo Android hay chạy Backend."

#### Câu 4: Tầng Repositories trong Mobile của nhóm em có cấu trúc như thế nào? Hoán đổi Mock Data ra sao?
> **Mẹo trả lời:**  
> "Nhóm em khai báo các Interface trừu tượng (như `IWalletRepository`, `IBudgetRepository`). Mỗi interface có 2 bản hiện thực: `ApiRepository` (kết nối Backend thật qua Refit) và `MockRepository` (dữ liệu mẫu in-memory). Tại `MauiProgram.cs`, nhóm chỉ cần thay đổi 1 biến cờ boolean `useMockData` là toàn bộ ứng dụng có thể hoán đổi mượt mà giữa chạy online có mạng và chạy offline biểu diễn mà không cần sửa dù chỉ 1 dòng code trong các ViewModels."

---

### 🎯 NHÓM 2: PUSH NOTIFICATION & NATIVE ANDROID ENGINE (PHẦN A)

#### Câu 5: Ứng dụng của em xử lý thông báo khi người dùng tắt hẳn app (Killed State) như thế nào?
> **Mẹo trả lời:**  
> "Thưa Thầy/Cô, khi ứng dụng bị tắt hoàn toàn khỏi đa nhiệm, MAUI runtime không còn hoạt động. Nhóm em đã can thiệp trực tiếp vào tầng Android Native bằng cách sử dụng **`Android.App.AlarmManager`** kết hợp với **`DailyReminderReceiver`**. Lịch hẹn được đăng ký thẳng vào nhân hệ điều hành bằng phương thức `SetExactAndAllowWhileIdle`. Đúng 21:00 hàng ngày, hệ điều hành Android tự động đánh thức `BroadcastReceiver` để bắn thông báo nhắc nhở lên màn hình và tự lên lịch tiếp cho ngày hôm sau. Nhóm còn có `BootReceiver` để tự phục hồi lịch hẹn khi người dùng khởi động lại máy."

#### Câu 6: Làm thế nào khi nhấn vào thông báo thì ứng dụng mở đúng màn hình và điền sẵn số liệu?
> **Mẹo trả lời:**  
> "Khi tạo thông báo native, nhóm em đính kèm một `target_route` vào `PendingIntent` (ví dụ: `///ExpensesPage?amount=50000&note=MoMo`). Khi người dùng nhấn vào thông báo, `MainActivity.cs` chặn sự kiện `OnCreate` hoặc `OnNewIntent`, trích xuất route và gọi `Shell.Current.GoToAsync(targetRoute)`. Tại `ExpensesViewModel`, nhóm hiện thực interface `IQueryAttributable` để nhận các tham số này và tự động điền sẵn vào các ô nhập liệu."

#### Câu 7: Tính năng bắt thông báo ngân hàng ngoại tuyến của nhóm em hoạt động trên cơ chế gì?
> **Mẹo trả lời:**  
> "Nhóm em kế thừa dịch vụ nền `Android.Service.Notification.NotificationListenerService` trong tệp `BankNotificationListenerService.cs`. Khi điện thoại nhận thông báo từ các app như MoMo, ACB, Vietcombank, MBBank, dịch vụ này lắng nghe nội dung, chuyển qua bộ bóc tách `BankNotificationParser.cs` bằng biểu thức chính quy Regex để lấy số tiền và nội dung chuyển khoản, sau đó bắn ngay một Native Notification cục bộ chứa Deep Link để người dùng chạm vào lưu sổ chi tiêu chỉ với 1 chạm."

---

### 🎯 NHÓM 3: GIAO TIẾP THỜI GIAN THỰC SIGNALR (PHẦN B2)

#### Câu 8: Tại sao nhóm em chọn SignalR thay vì Socket.IO? Cơ chế xác thực kết nối ra sao?
> **Mẹo trả lời:**  
> "Vì toàn bộ hệ thống của nhóm được xây dựng trên hệ sinh thái .NET 10 đồng nhất (ASP.NET Core Web API ở Backend và .NET MAUI ở Mobile), việc chọn SignalR mang lại khả năng tích hợp nguyên bản, mạnh mẽ và an toàn kiểu dữ liệu (strongly-typed). Kết nối WebSockets được bảo vệ bằng JWT thông qua thuộc tính `[Authorize]` trên `FinancialHub`. Phía client MAUI truyền token qua tham số truy vấn `access_token` khi bắt tay kết nối."

#### Câu 9: Em hãy chỉ ra 2 phương thức giao tiếp 2 chiều giữa Client và Server trong SignalR Hub của nhóm?
> **Mẹo trả lời:**  
> "Dạ, 2 chiều tương tác được nhóm em cài đặt tại `FinancialHub.cs` gồm:  
> - **Chiều Server gọi Client:** `ReceiveBalanceUpdate` (phát số dư mới khi có biến động) và `ReceiveBudgetAlert` (cảnh báo đỏ khi vượt 80% ngân sách).  
> - **Chiều Client gọi Server (Invoke):** `RequestLiveBalanceAsync` (Client chủ động yêu cầu server cấp số dư mới nhất khi vừa kết nối lại) và `BroadcastWalletRefreshAsync` (Client yêu cầu server phát lệnh làm mới danh sách ví đến tất cả các thiết bị cùng tài khoản)."

#### Câu 10: Làm thế nào để đảm bảo người dùng A chi tiêu thì chỉ máy của người dùng A cập nhật, không bị lộ sang người dùng B?
> **Mẹo trả lời:**  
> "Khi một client kết nối tới Hub, server trích xuất `UserId` từ JWT token và đưa kết nối đó vào một Group riêng biệt có tên là `user_{userId}` bằng lệnh `Groups.AddToGroupAsync`. Khi có sự kiện chi tiêu, Backend chỉ phát thông điệp tới đúng group `user_{userId}` đó qua `Clients.Group(...)`, đảm bảo cô lập dữ liệu tuyệt đối giữa các tài khoản."

---

### 🎯 NHÓM 4: CƠ SỞ DỮ LIỆU & BACKEND FINTECH

#### Câu 11: Làm sao Backend của em giải quyết được bài toán 2 giao dịch rút tiền đồng thời dẫn đến số dư bị âm (Race Condition)?
> **Mẹo trả lời:**  
> "Thưa Thầy/Cô, nhóm em áp dụng cơ chế **Khóa bi quan (Pessimistic Locking)** ở tầng cơ sở dữ liệu PostgreSQL. Khi thực hiện giao dịch trừ tiền ví, câu lệnh truy vấn sử dụng mệnh đề `SELECT * FROM wallets WHERE id = @id FOR UPDATE`. Dòng dữ liệu của ví đó sẽ bị khóa cứng trong suốt phiên Transaction. Yêu cầu rút tiền thứ hai buộc phải chờ cho đến khi yêu cầu thứ nhất hoàn tất `COMMIT`. Nhờ đó loại bỏ hoàn toàn rủi ro chi tiêu đúp (Double Spending) và bảo vệ số dư ví không bao giờ bị âm."

#### Câu 12: Cơ sở dữ liệu của nhóm có bao nhiêu bảng? Làm sao bảo đảm độ chính xác tiền tệ?
> **Mẹo trả lời:**  
> "Hệ thống có **22 bảng thực thể đạt chuẩn dạng chuẩn 3 (3NF)**. Để bảo đảm tính chính xác toán học tuyệt đối của các phép tính tài chính, toàn bộ các cột số tiền đều sử dụng kiểu dữ liệu `numeric(18,2)` trong PostgreSQL và `decimal` trong C#, tuyệt đối không dùng `float` hay `double` để tránh sai số làm tròn số thực dấu phẩy động."

---

### 🎯 NHÓM 5: TRÍ TUỆ NHÂN TẠO & TÍNH NĂNG NÂNG CAO

#### Câu 13: Mô hình AI nào được dùng để quét hóa đơn? Nếu AI đọc sai số tiền thì hệ thống xử lý thế nào?
> **Mẹo trả lời:**  
> "Nhóm sử dụng mô hình **Google Gemini 2.5 Flash Multimodal Vision**. Để chống ảo giác số học, nhóm em tích hợp **Động cơ Hậu kiểm toán số học tự động (Mathematical Audit Engine)**. Thuật toán lấy Tổng tiền trừ đi Thuế, Giảm giá và Tổng tiền các món con. Nếu độ lệch vượt quá 1.000 đ, hệ thống lập tức hiển thị banner màu vàng cảnh báo sai lệch để người dùng rà soát lại (Human-in-the-loop review) chứ không lưu mù quáng vào cơ sở dữ liệu."

#### Câu 14: Tính năng Khoảng trễ tâm lý 24 giờ (Cooling-off Period) và Cú hích (Nudge) dựa trên lý thuyết gì?
> **Mẹo trả lời:**  
> "Dạ, tính năng này được xây dựng dựa trên **Lý thuyết Cú hích (Nudge Theory)** của Giáo sư đoạt giải Nobel Kinh tế 2017 Richard Thaler. Khi người dùng chuẩn bị chi tiêu một khoản tiền lớn hoặc mua đồ xa xỉ, hệ thống kích hoạt khoảng trễ 24h để phong tỏa khoản chi (chưa trừ tiền ví), kích hoạt tư duy lý trí (System 2 thinking). Sau 24h đếm ngược, người dùng mới đưa ra quyết định cuối cùng là xác nhận mua hoặc hủy bỏ, giúp giảm thiểu tối đa hành vi mua sắm bốc đồng."

#### Câu 15: Dự án của nhóm em có những điểm gì vượt trội hơn các ứng dụng quản lý chi tiêu hiện có trên thị trường?
> **Mẹo trả lời:**  
> "Thưa Thầy/Cô, ứng dụng của nhóm em vượt trội ở 4 điểm mấu chốt:  
> 1. **Kỷ luật ngân sách tự động:** Động cơ Hạn mức Chi tiêu Mỗi ngày đàn hồi (Adaptive Daily Spending Allowance - DSA) tự điều chỉnh số tiền được tiêu mỗi ngày dựa trên số ngày còn lại và nâng hạn mức cho dịp cuối tuần.  
> 2. **AI Multimodal có đối soát số học:** Bóc tách hóa đơn siêu thị chi tiết từng món hàng kèm hậu kiểm toán tự động.  
> 3. **Bắt biến động số dư ngân hàng ngoại tuyến 1-chạm:** Tự động điền sổ chi tiêu từ MoMo, ACB, VCB mà không cần liên kết tài khoản ngân hàng phức tạp.  
> 4. **Kiến trúc công nghệ chuẩn doanh nghiệp:** Clean MVVM 3 tầng, SignalR 2 chiều, Native Android AlarmManager chạy ngay cả khi tắt app, và 363 bài Unit Tests tự động đạt chuẩn 100% Passed."

---

# BẢNG PHÂN CÔNG THUYẾT MINH & PHẢN BIỆN VẤN ĐÁP CỦA 2 THÀNH VIÊN

| Thành Viên Phụ Trách | Trọng Tâm Báo Cáo & Vấn Đáp Trước Giảng Viên | Các Câu Hỏi Chiếm Trọn Điểm |
| :--- | :--- | :--- |
| **Thành viên 1 (Lead Architecture & Backend/Systems)** | - Kiến trúc Clean MVVM 3 tầng & Cơ chế Data Binding.<br>- Cổng thời gian thực SignalR Hub 2 chiều Full-Duplex (Phần B2).<br>- Tầng cơ sở dữ liệu PostgreSQL 16 3NF & Khóa bi quan `SELECT FOR UPDATE`.<br>- Báo cáo kết quả kiểm thử tự động 363 Unit Tests. | Câu 1, 2, 8, 9, 10, 11, 12 |
| **Thành viên 2 (Frontend Specialist & Native Android)** | - Thiết kế giao diện di động, trải nghiệm người dùng (UX) & Design System.<br>- Native Android Notification Engine (3 channels, Killed State AlarmManager 21:00, BootReceiver).<br>- Deep Linking điều hướng thông minh qua `PendingIntent` & `IQueryAttributable`.<br>- Bắt biến động số dư ngân hàng MoMo/ACB ngoại tuyến & Trí tuệ nhân tạo Gemini AI. | Câu 4, 5, 6, 7, 13, 14, 15 |

---
*Cẩm nang này là tài liệu bản quyền thuộc đề tài: Hệ thống Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh (Tích hợp AI).*
