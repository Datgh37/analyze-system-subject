# 📱 TÀI LIỆU ĐẶC TẢ YÊU CẦU GIAO DIỆN & TÍCH HỢP FRONTEND (UI/UX REQUIREMENTS SPECIFICATION)
## HỆ THỐNG QUẢN LÝ CHI TIÊU & HOẠCH ĐỊNH TÀI CHÍNH CÁ NHÂN THÔNG MINH (PFM + AI)

> **Dự Án:** Smart Personal Expense and Financial Planning Management System with AI Integration  
> **Nền Tảng Kỹ Thuật:** .NET MAUI 10 (C# / XAML) — Kiến trúc MVVM (CommunityToolkit.Mvvm)  
> **Đối Tượng Tiếp Nhận:** Chuyên viên thiết kế & Lập trình viên Frontend (UI/UX Developer)  
> **Mục Đích Tài Liệu:** Định nghĩa toàn bộ **Nhu Cầu Giao Diện (UI Requirements), Các Thành Phần Thông Tin (Components) và Điểm Neo Dữ Liệu (Data Anchors)** của toàn bộ ứng dụng.  
> **Nguyên Tắc Thiết Kế Cốt Lõi:** **HOÀN TOÀN TỰ DO SÁNG TẠO NGHỆ THUẬT (DESIGN FREEDOM)** — Bạn có toàn quyền tự do lựa chọn màu sắc, bảng màu, typography, phong cách thiết kế (Dark/Light Mode, Glassmorphism, Neumorphism, Minimalist, Material 3...), bố cục lưới và hiệu ứng chuyển động, miễn là đảm bảo đầy đủ các thành phần dữ liệu và tương tác nghiệp vụ được đặc tả bên dưới.

---

## 1. ĐỊNH HƯỚNG SẢN PHẨM & TRIẾT LÝ ỨNG DỤNG

Ứng dụng này **không phải** là một cuốn sổ ghi chép thu chi thụ động thông thường, mà là một **"Huấn Luyện Viên Tài Chính Cá Nhân Thông Minh" (Smart Personal Financial Coach)** với 3 định hướng cốt lõi:

1. **100% Tài Chính Cá Nhân (Pure Personal Finance):**
   - Ứng dụng tập trung tuyệt đối vào việc tối ưu hóa sức khỏe tài chính của **một cá nhân**.
   - **Tuyệt đối KHÔNG có** các tính năng chia tiền nhóm, quỹ hội nhóm, hay thanh toán tập thể phức tạp làm loãng trải nghiệm người dùng.
2. **Kinh Tế Học Hành Vi (Behavioral Economics - Cú Hích Richard Thaler):**
   - Giúp người dùng thay đổi hành vi chi tiêu một cách tự nhiên bằng các "Cú hích" (Nudges) nhẹ nhàng thay vì cấm đoán cực đoan.
   - Tính toán **Hạn mức an toàn mỗi ngày (Daily Safe Amount - DSA)**: Người dùng mở app lên là biết ngay hôm nay mình được tiêu tối đa bao nhiêu tiền mà cuối tháng vẫn đủ tiền tiết kiệm.
   - **Khoảng trễ tâm lý (Cooling-off Period):** Khi người dùng muốn chi tiêu một khoản tiền lớn bất thường, hệ thống yêu cầu họ tạm dừng 24h - 48h để cảm xúc lắng xuống trước khi ra quyết định mua sắm.
3. **Trí Tuệ Nhân Tạo Đa Phương Thức & Tự Hành (Multimodal AI & Autonomous Agent):**
   - **AI Vision OCR:** Chụp ảnh hóa đơn mua sắm hoặc phiếu lương $\rightarrow$ Tự động bóc tách từng món đồ, số tiền, thuế VAT và gợi ý danh mục chi tiêu chuẩn xác.
   - **AI Voice Assistant:** Trợ lý giọng nói nhận diện tiếng Việt tự nhiên (nghìn, k, củ, chai, hôm qua, trưa nay) $\rightarrow$ Điền form tự động chỉ sau 1 câu nói rảnh tay.
   - **Hộp Thư Tự Hành (Action Inbox):** Nhận diện biến động số dư ngân hàng qua thông báo đẩy (Push Notification) $\rightarrow$ Người dùng chỉ cần trượt nhẹ (Swipe) để duyệt hoặc từ chối mà không cần gõ bàn phím.
4. **Kiến Trúc Hai Chế Độ (Dual-Mode: Offline Mock & Real Backend):**
   - Khi làm giao diện, bạn **không bắt buộc phải chạy Database hay Backend**. Toàn bộ 20+ ViewModels đã có sẵn **Mock Repositories** với dữ liệu mẫu sống động, mượt mà 0ms.
   - Khi cần kiểm thử dữ liệu thực tế, chỉ cần đổi 1 dòng cấu hình để kết nối tới Backend ASP.NET Core 10 WebApi với 25 Controllers và WebSocket SignalR thời gian thực.

---

## 2. HƯỚNG DẪN THIẾT LẬP MÔI TRƯỜNG & KHỞI CHẠY (DÀNH CHO BẠN FRONTEND)

### 2.1. Yêu Cầu Môi Trường Máy Tính
- **Hệ Điều Hành:** Windows 10/11 hoặc macOS.
- **Công Cụ Phát Triển:**
  - **Visual Studio 2022 / 2026 (v17.12+ hoặc bản Preview hỗ trợ .NET 10)** kèm workload **".NET Multi-platform App UI development"**.
  - Hoặc **Visual Studio Code** kèm bộ Extension: *C# Dev Kit*, *.NET MAUI*.
- **SDK Yêu Cầu:** [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).

### 2.2. Chế Độ 1: Làm Giao Diện Hoàn Toàn Độc Lập (Khuyên Dùng - Offline Mock)
Trong chế độ này, bạn không cần cài PostgreSQL, không cần Docker, không cần chạy Backend:
1. Mở solution `mobile-project/dotnet-maui-ver/backend/ExpenseTracker.slnx` hoặc mở thư mục `frontend/ExpenseTracker.Maui`.
2. Mở file [`MauiProgram.cs`](file:///mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/MauiProgram.cs).
3. Đảm bảo các dòng đăng ký Mock Repository đang được kích hoạt (đây là trạng thái mặc định của dự án):
   ```csharp
   // DÙNG DỮ LIỆU MẪU ĐỂ LÀM GIAO DIỆN (OFFLINE MOCK 0ms - KHÔNG CẦN BACKEND)
   builder.Services.AddScoped<IWalletRepository, MockWalletRepository>();
   builder.Services.AddScoped<ITransactionRepository, MockTransactionRepository>();
   builder.Services.AddScoped<IBudgetRepository, MockBudgetRepository>();
   builder.Services.AddScoped<IDebtRepository, MockDebtRepository>();
   builder.Services.AddScoped<IEmergencyFundRepository, MockEmergencyFundRepository>();
   builder.Services.AddScoped<ISubscriptionRepository, MockSubscriptionRepository>();
   builder.Services.AddScoped<INetWorthRepository, MockNetWorthRepository>();
   builder.Services.AddScoped<IForecastingRepository, MockForecastingRepository>();
   builder.Services.AddScoped<INudgeRepository, MockNudgeRepository>();
   builder.Services.AddScoped<ICoolingOffRepository, MockCoolingOffRepository>();
   ```
4. Bấm `F5` chạy ứng dụng trên **Windows Machine** (để debug giao diện siêu tốc) hoặc **Android Emulator**. Ứng dụng sẽ tải ngay dữ liệu mẫu sống động!

### 2.3. Chế Độ 2: Chạy Kết Nối Backend WebApi Thật & Điền Gemini API Key Cá Nhân
Nếu bạn muốn trải nghiệm đầy đủ các tính năng AI thật (Gemini AI Vision, AI Voice, PostgreSQL):
1. **Tạo file cấu hình Key riêng:**
   - Đi tới thư mục: `backend/ExpenseTracker.WebApi/`
   - Bạn sẽ thấy file mẫu: `appsettings.Local.json.example`
   - Tạo một bản sao và đổi tên thành: `appsettings.Local.json`
   - Mở file và dán Gemini API Key của bạn:
     ```json
     {
       "AiSettings": {
         "ApiKey": "DÁN_GEMINI_API_KEY_CỦA_BẠN_VÀO_ĐÂY"
       }
     }
     ```
   *(Lưu ý: File `appsettings.Local.json` đã được đưa vào `.gitignore`, sẽ không bao giờ bị lộ ra ngoài).*
2. **Khởi chạy Backend:**
   - Mở terminal tại thư mục `backend/ExpenseTracker.WebApi` và gõ:
     ```bash
     dotnet run
     ```
   - Backend sẽ tự động kiểm tra PostgreSQL: Nếu máy bạn có PostgreSQL thì sẽ dùng PostgreSQL; nếu máy bạn chưa cài PostgreSQL thì Backend sẽ tự động chạy SQLite local (`expensetracker.db`) mà không báo lỗi!
   - Swagger API Documentation sẽ xuất hiện tại: `https://localhost:7080/swagger` (hoặc `http://localhost:5080/swagger`).
3. **Chuyển Frontend sang dùng Real API:**
   - Trong `MauiProgram.cs`, chuyển các dòng đăng ký sang `ApiWalletRepository`, `ApiTransactionRepository`,... và thiết lập địa chỉ Server trong màn hình `SettingsPage`.

---

## 3. QUY ƯỚC GHÉP NỐI MVVM (DATA ANCHORS - NGUYÊN TẮC BẤT DI BẤT DỊCH)

Khi thiết kế lại bất kỳ file `.xaml` nào, bạn hoàn toàn tự do xóa toàn bộ giao diện cũ và viết lại mới từ đầu. Điều duy nhất bạn cần giữ lại để hệ thống hoạt động trơn tru là **Tên thuộc tính Binding** và **Tên Command**:

```xml
<!-- Bạn có thể chọn bất kỳ Control nào: Border, Frame, CardView, Label, Button... -->
<!-- Miễn là giữ đúng đường dẫn {Binding ...} -->
<Label Text="{Binding Balance, StringFormat='{0:N0} đ'}" />
<Button Text="Lưu Giao Dịch" Command="{Binding SaveTransactionCommand}" />
```

---

## 4. CHI TIẾT ĐẶC TẢ YÊU CẦU CHO TỪNG MÀN HÌNH (UI REQUIREMENTS SPECIFICATION)

Dưới đây là danh mục toàn bộ các màn hình chính trong ứng dụng, được mô tả dưới dạng **Yêu cầu Nghiệp vụ Giao diện (Requirement Specs)**:

---

### MÀN HÌNH 01: TRANG CHỦ / TỔNG QUAN (DASHBOARD & FINANCIAL HOME)
* **Mục đích:** Là màn hình người dùng nhìn thấy đầu tiên mỗi ngày. Cung cấp bức tranh toàn cảnh về sức khỏe tài chính trong tích tắc (Glanceable UI) và hướng dẫn họ hôm nay nên tiêu xài ra sao.
* **Các Thành Phần Cần Có (Components Required):**
  1. **Thẻ Tài Sản Ròng & Ví Nhanh (Net Worth & Wallet Carousel/List):**
     - Hiển thị Tổng tài sản ròng (`NetWorthAmount`), tỷ lệ biến động so với tháng trước (`NetWorthGrowthRate`).
     - Danh sách lướt các ví (`Wallets`): Thẻ tiền mặt, thẻ ngân hàng, ví điện tử kèm biểu tượng, tên ví, số dư hiện tại (`Balance`).
     - Nút ẩn/hiện số dư để bảo vệ riêng tư nơi công cộng (`IsBalanceVisible`, `ToggleBalanceVisibilityCommand`).
  2. **Thước Đo Hạn Mức An Toàn Hôm Nay (Daily Safe Amount - DSA Widget):**
     - Số tiền tối đa được chi hôm nay (`DailySafeAmount`).
     - Tình trạng tiêu dùng: Đã tiêu bao nhiêu (`SpentToday`), còn lại bao nhiêu (`RemainingSafeToday`).
     - Cảnh báo tốc độ chi tiêu (`SpendingVelocity`): Nhãn trực quan (Bình thường / Nhanh / Báo động đỏ) để nhắc nhở người dùng hãm phanh.
  3. **Hộp Thông Điệp Cú Hích Tài Chính (Richard Thaler Nudge Card):**
     - Hiển thị lời khuyên tài chính được cá nhân hóa (`ActiveNudgeTitle`, `ActiveNudgeMessage`).
     - Nút "Tìm hiểu lý do" (XAI Rationale - `ViewNudgeRationaleCommand`) để mở cửa sổ giải trình logic toán học đằng sau lời khuyên.
     - Nút "Chấp thuận" (`ApproveNudgeCommand`) hoặc "Bỏ qua" (`DismissNudgeCommand`).
  4. **Thanh Thao Tác Nhanh (Quick Actions):**
     - Nút Thêm chi tiêu (`NavigateToExpenseCommand`).
     - Nút Thêm thu nhập (`NavigateToIncomeCommand`).
     - Nút Quét hóa đơn AI Vision (`NavigateToOcrCommand`).
     - Nút Trợ lý giọng nói (`NavigateToVoiceCommand`).
  5. **Giao Dịch Gần Đây (Recent Transactions Stream):**
     - Danh sách 5-10 giao dịch mới nhất (`RecentTransactions`): Biểu tượng danh mục, tên khoản chi, thời gian, số tiền (âm hiển thị màu chi tiêu, dương hiển thị màu thu nhập), ví thanh toán.
* **Các Trạng Thái Giao Diện (States):**
  - *Loading:* Hiển thị khung xương (Skeleton loader) cho các thẻ số dư.
  - *Empty:* Khi người dùng mới chưa có ví/giao dịch $\rightarrow$ Hiển thị lời chào kèm nút kích hoạt "Khởi tạo ví đầu tiên".
* **Data Context:** [`DashboardViewModel`](file:///mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/ViewModels/DashboardViewModel.cs).

---

### MÀN HÌNH 02: SỔ GIAO DỊCH & THÊM/SỬA KHOẢN CHI TIÊU (TRANSACTIONS & EXPENSE FORM)
* **Mục đích:** Giúp người dùng tra cứu lịch sử chi tiêu và thêm mới một khoản chi tiêu một cách nhanh chóng nhất với tối thiểu số lần chạm (Frictionless Entry).
* **Các Thành Phần Cần Có (Components Required):**
  1. **Bộ Lọc & Tìm Kiếm Đa Năng:**
     - Ô tìm kiếm theo từ khóa ghi chú (`SearchKeyword`).
     - Bộ lọc theo Ví (`SelectedWalletFilter`), theo Danh mục (`SelectedCategoryFilter`), theo Khoảng thời gian (Hôm nay / Tuần này / Tháng này / Tùy chọn).
  2. **Danh Sách Giao Dịch Phân Nhóm Theo Ngày (Grouped List):**
     - Nhóm theo từng ngày (Ví dụ: "Hôm nay - 01/11/2026", "Hôm qua - 31/10/2026") kèm tổng tiền chi tiêu trong ngày đó.
     - Mỗi dòng giao dịch: Biểu tượng danh mục, tên ghi chú, số tiền, ví nguồn, giờ giao dịch.
     - Hỗ trợ thao tác vuốt (Swipe Action): Vuốt sang trái để Lưu trữ / Xóa mềm (`ArchiveTransactionCommand`), vuốt sang phải để Chỉnh sửa (`EditTransactionCommand`).
  3. **Biểu Mẫu Thêm Khoản Chi Tiêu (Expense Form Component):**
     - Bàn phím số nhập tiền lớn, nổi bật (`Amount`) kèm định dạng phân tách hàng nghìn tự động.
     - Lựa chọn Ví thanh toán (`WalletsList`, `SelectedWallet`).
     - Lựa chọn Danh mục 2 cấp (`CategoriesList`, `SelectedCategory`): Ví dụ: Danh mục cha "Ăn uống" $\rightarrow$ Danh mục con "Cà phê", "Cơm trưa".
     - Chọn Ngày & Giờ giao dịch (`TransactionDate`).
     - Ô nhập ghi chú (`Note`).
     - Khối đính kèm ảnh chụp hóa đơn (`ReceiptImageThumbnail`, `AttachReceiptCommand`).
     - Nút bấm lưu giao dịch (`SaveExpenseCommand`).
* **Các Trạng Thái Giao Diện (States):**
  - Khi số tiền chi tiêu vượt quá số dư ví $\rightarrow$ Báo lỗi đỏ không cho lưu hoặc gợi ý đổi ví.
  - Khi khoản chi tiêu lớn kích hoạt khoảng trễ $\rightarrow$ Tự động chuyển hướng hiển thị thông báo "Khoảng trễ tâm lý Cooling-off".
* **Data Context:** [`ExpensesViewModel`](file:///mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/ViewModels/ExpensesViewModel.cs) & [`TransactionsViewModel`](file:///mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/ViewModels/TransactionsViewModel.cs).

---

### MÀN HÌNH 03: QUẢN LÝ ĐA VÍ & ĐIỀU CHUYỂN TIỀN (WALLETS & MONEY TRANSFER)
* **Mục đích:** Quản lý toàn bộ các nguồn tiền của cá nhân (Tiền mặt, Tài khoản ngân hàng, Thẻ tín dụng, Ví MoMo, Sổ tiết kiệm) và điều chuyển qua lại.
* **Các Thành Phần Cần Có (Components Required):**
  1. **Danh Sách Thẻ Ví Trực Quan:**
     - Thiết kế dạng thẻ bài (Cards Grid / Stack): Mỗi ví hiển thị Tên ví (`WalletName`), Biểu tượng (`Icon`), Số dư hiện thời (`Balance`), Loại ví (`WalletType`: Cash, Bank, CreditCard, EWallet).
     - Đánh dấu ví mặc định (`IsDefault`).
     - Nút thêm ví mới (`AddNewWalletCommand`).
  2. **Thao Tác Điều Chuyển Nội Bộ (Transfer Modal/Component):**
     - Chọn Ví nguồn (`SourceWallet`).
     - Chọn Ví đích (`DestinationWallet`).
     - Nhập số tiền chuyển (`TransferAmount`) và Phí chuyển nếu có (`Fee`).
     - Ghi chú chuyển tiền (`TransferNote`).
     - Nút xác nhận chuyển tiền (`ExecuteTransferCommand`).
  3. **Tiện Ích Cân Đối Số Dư Ví (Adjust Balance):**
     - Cho phép nhập số dư thực tế đếm được $\rightarrow$ Hệ thống tự động sinh khoản thu/chi chênh lệch để số liệu trên app khớp 100% với thực tế.
* **Data Context:** [`WalletsViewModel`](file:///mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/ViewModels/WalletsViewModel.cs).

---

### MÀN HÌNH 04: NGÂN SÁCH CHU KỲ & TÁI CÂN ĐỐI (BUDGETS & REBALANCING)
* **Mục đích:** Giúp người dùng thiết lập hạn mức chi tiêu theo tháng và giám sát tiến độ tiêu dùng để không bị rỗng túi trước ngày nhận lương.
* **Các Thành Phần Cần Có (Components Required):**
  1. **Thanh Tiến Độ Ngân Sách Tổng Thể (Overall Budget Progress):**
     - Tổng ngân sách tháng (`TotalBudgetLimit`), Đã chi (`TotalSpent`), Còn lại (`RemainingBudget`), Tỷ lệ hoàn thành (`SpentPercentage`%).
     - Đổi màu sắc cảnh báo theo ngưỡng: Xanh (<70%), Vàng (70%-85%), Đỏ (>85%).
  2. **Danh Sách Hạn Mức Từng Danh Mục (Category Allocations):**
     - Tên danh mục, số tiền đã tiêu / hạn mức (Ví dụ: "Ăn uống: Đã tiêu 2.500.000đ / 3.000.000đ").
     - Thanh mini-progress bar cho từng mục.
  3. **Công Cụ Tái Cân Đối Ngân Sách Thông Minh (Budget Rebalance Component):**
     - Khi một danh mục bị tiêu vượt mức (Ví dụ Ăn uống bị âm 300.000đ), hệ thống hiển thị nút "Đề xuất Tái cân đối" (`OpenRebalanceProposalCommand`).
     - Hiển thị gợi ý: "Cắt giảm 300.000đ từ danh mục Mua sắm (đang còn dư) để bù cho Ăn uống".
     - Nút 1-chạm "Chấp nhận hoán đổi" (`ApplyRebalanceCommand`).
* **Data Context:** [`BudgetsViewModel`](file:///mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/ViewModels/BudgetsViewModel.cs).

---

### MÀN HÌNH 05: QUẢN LÝ NỢ CÁ NHÂN & TỐI ƯU HÓA TRẢ NỢ (DEBT CATALOG & OPTIMIZER)
* **Mục đích:** Xóa bỏ sự mơ hồ và lo lắng về nợ nần. Giúp người dùng nắm rõ từng khoản nợ và cung cấp lộ trình trả nợ khoa học nhất.
* **Các Thành Phần Cần Có (Components Required):**
  1. **Bảng Chỉ Số Nợ Tổng Quan (Debt Summary Header):**
     - Tổng dư nợ hiện tại (`TotalRemainingDebt`).
     - Tổng số tiền đã thanh toán (`TotalPaidAmount`).
     - Số tiền cần trả tối thiểu hàng tháng (`TotalMonthlyMinimum`).
     - Tỷ lệ hoàn thành trả nợ toàn diện (`OverallProgressPercentage`%).
  2. **Danh Sách Các Khoản Nợ Cá Nhân (Debt Cards List):**
     - Thẻ từng khoản nợ (`DebtsList`): Tên khoản nợ (Vay mua xe, Thẻ tín dụng, Nợ người thân), Người cho vay (`Creditor`), Lãi suất năm (`InterestRate`%), Dư nợ còn lại (`RemainingAmount`), Ngày đến hạn (`DueDate`).
     - Nút "Trả nợ ngay" (`RecordPaymentCommand`): Mở hộp thoại nhập tiền trả, chọn ví trích tiền $\rightarrow$ Tự động hạch toán giảm nợ gốc và trừ tiền ví theo chuẩn kế toán kép.
     - Đánh dấu huy hiệu "ĐÃ TẤT TOÁN" khi nợ về 0.
  3. **Cẩm Nang & So Sánh Chiến Lược Trả Nợ (Snowball vs Avalanche):**
     - Cẩm nang bình dân học vụ: Giải thích dễ hiểu phương pháp **Quả cầu tuyết (Snowball - Trả món nhỏ trước để lấy động lực)** và **Thác tuyết (Avalanche - Trả món lãi cao trước để tiết kiệm tiền)**.
     - Bộ so sánh song song (`CompareStrategiesCommand`):
       * Số tiền lãi tiết kiệm được (`InterestSavedAmount`).
       * Số tháng trả nợ rút ngắn được (`MonthsSaved`).
       * Biểu đồ mô phỏng dòng thời gian trả hết nợ (Timeline).
* **Data Context:** [`DebtOptimizerViewModel`](file:///mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/ViewModels/DebtOptimizerViewModel.cs).

---

### MÀN HÌNH 06: QUỸ DỰ PHÒNG KHẨN CẤP & TÀI SẢN RÒNG (EMERGENCY FUND & NET WORTH)
* **Mục đích:** Xây dựng tấm lá chắn an toàn tài chính vững chắc trước các rủi ro bất ngờ (mất việc, đau ốm, hỏng xe).
* **Các Thành Phần Cần Có (Components Required):**
  1. **Vòng Cung Quỹ Dự Phòng Khẩn Cấp (Emergency Fund Gauge/Ring):**
     - Hiển thị số tiền hiện có trong quỹ (`CurrentAmount`) so với Mục tiêu (`TargetAmount`).
     - Chỉ số **"Số Tháng Sinh Tồn Khả Dụng" (Survival Runway Months)**: Cho biết nếu ngày mai mất nguồn thu thì số tiền này giúp người dùng sống sót được bao nhiêu tháng (`RunwayMonths`).
     - Gợi ý mức dự phòng dựa trên khảo sát: 3 tháng (ổn định), 6 tháng (tiêu chuẩn), 12 tháng (tự do/kinh doanh).
  2. **Nút Thao Tác Nạp / Rút Quỹ Khẩn Cấp:**
     - Nút "Trích tiền vào quỹ" (`DepositToFundCommand`).
     - Nút "Rút tiền sự cố" (`WithdrawFromFundCommand`): Kèm yêu cầu nhập lý do khẩn cấp để nhắc nhở người dùng không tiêu lẹm vào quỹ này.
  3. **Biểu Đồ Tài Sản Ròng (Net Worth Evolution Chart):**
     - Biểu đồ đường hoặc cột thể hiện xu hướng tăng trưởng tài sản ròng theo các mốc thời gian (`NetWorthHistory`).
     - Công thức hiển thị minh bạch: Tài sản ròng = Tổng tiền trong ví - Tổng dư nợ.
* **Data Context:** [`EmergencyFundViewModel`](file:///mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/ViewModels/EmergencyFundViewModel.cs) & [`NetWorthViewModel`](file:///mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/ViewModels/NetWorthViewModel.cs).

---

### MÀN HÌNH 07: QUÉT BẪY THUÊ BAO ĐỊNH KỲ (SUBSCRIPTION TRAPS RADAR)
* **Mục đích:** Ngăn chặn tình trạng rò rỉ dòng tiền vi mô (Micro-drain) từ các dịch vụ đăng ký định kỳ bị lãng quên (Netflix, Spotify, iCloud, ChatGPT, Gym).
* **Các Thành Phần Cần Có (Components Required):**
  1. **Báo Cáo Kiểm Toán Hao Tổn Hàng Năm (Annual Leakage Card):**
     - Tổng chi phí thuê bao phải trả mỗi năm (`TotalAnnualCost`): Giúp người dùng "giật mình" khi thấy các khoản 59k, 260k hàng tháng cộng dồn thành con số tiền triệu mỗi năm.
     - Số lượng thuê bao đang kích hoạt (`ActiveSubscriptionsCount`).
  2. **Cảnh Báo Bẫy Dùng Thử Miễn Phí (Free Trial Trap Warning):**
     - Khối thông báo nổi bật nếu có dịch vụ đang trong thời gian dùng thử sắp tự động trừ tiền trong 3 ngày tới (`ExpiringFreeTrials`).
  3. **Danh Sách Dịch Vụ Thuê Bao:**
     - Mỗi dịch vụ hiển thị logo nhận diện, tên gói, chu kỳ (Tháng / Quý / Năm), số tiền, ngày gia hạn tiếp theo (`NextBillingDate`).
     - Nút "Hủy theo dõi / Đã hủy dịch vụ" (`ArchiveSubscriptionCommand`).
* **Data Context:** [`SubscriptionsViewModel`](file:///mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/ViewModels/SubscriptionsViewModel.cs).

---

### MÀN HÌNH 08: MÔ PHỎNG KỊCH BẢN TÀI CHÍNH WHAT-IF (WHAT-IF SCENARIOS)
* **Mục đích:** Cho phép người dùng thử nghiệm các quyết định mua sắm lớn trong tương lai trước khi thực sự tiêu tiền để thấy trước hậu quả.
* **Các Thành Phần Cần Có (Components Required):**
  1. **Khung Nhập Giả Định Mua Sắm:**
     - Tên món đồ dự kiến mua (`PlannedPurchaseName` - Ví dụ: "Mua iPhone 16 Pro Max", "Đi du lịch Nhật Bản").
     - Số tiền dự kiến (`PlannedAmount`).
     - Chọn ngày dự kiến chi tiêu (`PlannedDate`).
     - Lựa chọn hình thức: Trả thẳng một lần hay Trả góp hàng tháng.
  2. **Bảng Phân Tích Tác Động Tức Thì (Impact Analysis Results):**
     - Tác động tới quỹ khẩn cấp: Sẽ làm giảm bao nhiêu tháng sinh tồn?
     - Tác động tới ngân sách tháng: Có bị âm không?
     - Đánh giá khả thi: Điểm số an toàn (An toàn / Rủi ro cao / Báo động đỏ).
  3. **Nút Chuyển Đổi Thành Mục Tiêu Tích Lũy (Convert to Savings Goal):**
     - Nút 1-chạm "Chuyển thành Mục tiêu Tiết kiệm" (`ConvertToGoalCommand`): Nếu mua ngay lúc này là rủi ro, hệ thống tự động lập kế hoạch mỗi tháng cần để dành bao nhiêu tiền để 6 tháng nữa mua mà không phải gánh nợ.
* **Data Context:** [`ForecastingViewModel`](file:///mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/ViewModels/ForecastingViewModel.cs).

---

### MÀN HÌNH 09: HỘP THƯ HÀNH ĐỘNG TỰ HÀNH (AUTONOMOUS ACTION INBOX)
* **Mục đích:** Là nơi tiếp nhận các biến động số dư ngân hàng và đề xuất tài chính tự hành (Jev Protocol).
* **Các Thành Phần Cần Có (Components Required):**
  1. **Danh Sách Thẻ Hành Động Chờ Duyệt (Inbox Action Cards):**
     - Mỗi thẻ hiển thị: Nguồn biến động (Logo Vietcombank, Techcombank, MoMo, ACB...), Số tiền biến động (+/-), Tên nơi chi tiêu (Highlands Coffee, Grab, Shopee, Co.opmart), Thời gian.
     - Danh mục hệ thống tự động phán đoán (Độ tin cậy Confidence score $\ge 85\%$).
  2. **Thao Tác Duyệt Cực Nhanh (Frictionless Approval):**
     - Hỗ trợ vuốt sang phải hoặc bấm nút xanh "Duyệt 1-chạm" (`QuickCommitActionCommand`) $\rightarrow$ Giao dịch được lưu ngay vào CSDL, số dư ví tự nhảy.
     - Hỗ trợ vuốt sang trái hoặc bấm nút đỏ "Bỏ qua" (`DismissActionCommand`).
  3. **Thanh Thử Nghiệm Mô Phỏng Giả Lập (Simulator Tool):**
     - Nút thử nghiệm giả lập biến động số dư từ các ngân hàng để người làm giao diện dễ dàng test UI mà không cần tài khoản ngân hàng thật (`SimulateBankNotificationCommand`).
* **Data Context:** [`ActionInboxViewModel`](file:///mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/ViewModels/Autonomous/ActionInboxViewModel.cs).

---

### MÀN HÌNH 10: QUÉT HÓA ĐƠN & PHIẾU LƯƠNG AI MULTIMODAL (AI VISION SCANNER)
* **Mục đích:** Nhập liệu rảnh tay bằng cách chụp ảnh hóa đơn siêu thị/nhà hàng hoặc bảng lương.
* **Các Thành Phần Cần Có (Components Required):**
  1. **Khối Chụp / Chọn Ảnh:**
     - Nút Mở Camera (`CapturePhotoCommand`) và Nút Chọn Từ Thư Viện (`PickPhotoCommand`).
     - Khung hiển thị ảnh xem trước (`ReceiptImageSource`).
  2. **Thanh Tiến Trình Xử Lý AI Real-time (OCR Progress Bar):**
     - Hiển thị thanh tiến trình trực quan từ 0% đến 100% kèm nhãn trạng thái nhận diện thời gian thực qua SignalR WebSocket (`OcrProgressText`: Đang tải ảnh $\rightarrow$ Đang nhận diện văn bản $\rightarrow$ Đang bóc tách từng món đồ).
  3. **Bảng Đối Soát Dữ Liệu Sau Khi Bóc Tách (Review & Confirm):**
     - Tên người bán (`MerchantName`).
     - Ngày mua (`TransactionDate`).
     - Danh sách từng món hàng đã bóc tách (`ItemsList`): Tên món, số lượng, đơn giá, thành tiền.
     - Tiền thuế VAT và Tổng tiền cuối cùng (`TotalAmount`).
     - Nút "Xác nhận lưu giao dịch" (`ConfirmReceiptCommand`).
* **Data Context:** [`ReceiptOcrViewModel`](file:///mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/ViewModels/ReceiptOcrViewModel.cs).

---

### MÀN HÌNH 11: TRỢ LÝ GIỌNG NÓI THÔNG MINH (AI VOICE ASSISTANT MODAL)
* **Mục đích:** Thu âm câu lệnh tiếng Việt tự nhiên và tự động trích xuất thành giao dịch chi tiêu hoặc điều hướng ứng dụng.
* **Các Thành Phần Cần Có (Components Required):**
  1. **Nút Mic Động Học & Sóng Âm Thanh (Pulsing Mic & Waveform):**
     - Nút bấm micro to ở giữa màn hình (`ToggleRecordingCommand`).
     - Hiệu ứng sóng âm thanh hoặc vòng tròn lan tỏa khi đang lắng nghe (`IsListening`).
  2. **Khung Văn Bản Nhận Diện (Live Transcript):**
     - Hiển thị chữ người dùng vừa nói (Ví dụ: "Ăn phở Thìn hết 65 nghìn trả tiền mặt").
  3. **Khối Kết Quả Phân Tích Ý Định (Intent Result):**
     - Ý định nhận diện được: Ghi chép chi tiêu / Thu nhập / Mở báo cáo.
     - Các thực thể trích xuất: Số tiền (65.000đ), Danh mục (Ăn uống), Ví (Tiền mặt).
     - Nút "Xác nhận tạo ngay" (`ConfirmVoiceTransactionCommand`).
* **Data Context:** [`VoiceFloatingMicViewModel`](file:///mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/ViewModels/AiAgent/VoiceFloatingMicViewModel.cs).

---

### MÀN HÌNH 12: ĐĂNG NHẬP, ĐĂNG KÝ & QUICKLOGIN SINH TRẮC HỌC (AUTH & BIOMETRICS)
* **Mục đích:** Bảo vệ an toàn dữ liệu tài chính của người dùng với cơ chế đăng nhập nhanh hiện đại.
* **Các Thành Phần Cần Có (Components Required):**
  1. **Đăng Nhập Chuẩn (Standard Login):** Email, Mật khẩu, nút Đăng nhập (`LoginCommand`), Đăng nhập Google SSO (`GoogleLoginCommand`), Quên mật khẩu (`ForgotPasswordCommand`).
  2. **Đăng Ký Tài Khoản:** Họ tên, Email, Mật khẩu, Xác nhận mã OTP 6 số gửi qua Email (`VerifyOtpCommand`).
  3. **Màn Hình Đăng Nhập Nhanh (QuickLogin Page):**
     - Hiển thị ảnh đại diện và lời chào cá nhân: "Chào mừng bạn trở lại, Nguyễn Văn A!".
     - Bàn phím số nhập mã PIN 6 số tiện lợi.
     - Nút quét Vân tay / Nhận diện khuôn mặt (FaceID / Fingerprint) để mở khóa tức thì 0 giây (`AuthenticateBiometricsCommand`).
* **Data Context:** [`QuickLoginViewModel`](file:///mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/ViewModels/QuickLoginViewModel.cs).

---

## 5. TỔNG KẾT BẢNG ÁNH XẠ XAML VIEWS VÀ VIEWMODELS

Khi bạn thiết kế lại các file trong thư mục `Views/`, hãy tham chiếu bảng này để biết View nào gắn với ViewModel nào:

| Tên Màn Hình (.xaml) | ViewModel Tương Ứng | Vai Trò Chính |
| :--- | :--- | :--- |
| `DashboardPage.xaml` | `DashboardViewModel` | Trang chủ, tổng quan tài chính, thẻ DSA, Nudge |
| `ExpensesPage.xaml` | `ExpensesViewModel` | Danh sách & Form thêm chi tiêu, chọn danh mục |
| `IncomesPage.xaml` | `IncomesViewModel` | Danh sách & Form thêm thu nhập định kỳ |
| `TransactionsPage.xaml` | `TransactionsViewModel` | Sổ cái tra cứu toàn bộ giao dịch, bộ lọc ngày/ví |
| `WalletsPage.xaml` | `WalletsViewModel` | Quản lý ví, điều chuyển tiền nội bộ giữa các ví |
| `BudgetsPage.xaml` | `BudgetsViewModel` | Ngân sách chu kỳ, hạn mức danh mục, DSA velocity |
| `DebtOptimizerPage.xaml` | `DebtOptimizerViewModel` | Quản lý nợ cá nhân, so sánh Snowball vs Avalanche |
| `EmergencyFundPage.xaml`| `EmergencyFundViewModel` | Vòng cung quỹ khẩn cấp 3-6-12 tháng, Runway |
| `NetWorthPage.xaml` | `NetWorthViewModel` | Biểu đồ biến động tài sản ròng theo thời gian |
| `SubscriptionsPage.xaml`| `SubscriptionsViewModel` | Quét bẫy thuê bao định kỳ, cảnh báo dùng thử |
| `ForecastingPage.xaml` | `ForecastingViewModel` | Dự báo tài chính & Phân tích kịch bản What-If |
| `ActionInboxView.xaml` | `ActionInboxViewModel` | Hộp thư biến động số dư ngân hàng duyệt 1-chạm |
| `ReceiptOcrPage.xaml` | `ReceiptOcrViewModel` | Bóc tách hóa đơn siêu thị & phiếu lương AI OCR |
| `VoiceMicModalPage.xaml`| `VoiceFloatingMicViewModel`| Nút mic trợ lý giọng nói tiếng Việt tự nhiên |
| `QuickLoginPage.xaml` | `QuickLoginViewModel` | Mở khóa nhanh bằng mã PIN hoặc Vân tay/FaceID |
| `SettingsPage.xaml` | `SettingsViewModel` | Cài đặt ứng dụng, chọn máy chủ API, đổi mật khẩu |

---

> **Lời Nhắn Nhủ Dành Cho Bạn Thiết Kế:**  
> Đừng ngần ngại phá vỡ các bố cục cũ nếu bạn có ý tưởng thẩm mỹ tốt hơn! Bạn hoàn toàn làm chủ nghệ thuật, màu sắc và linh hồn thị giác của ứng dụng. Hãy biến ứng dụng tài chính này thành một trải nghiệm mượt mà, cuốn hút và truyền cảm hứng nhất cho người dùng!
