# TÀI LIỆU KỸ THUẬT GHÉP NỐI FRONTEND & BACKEND (MAUI MVVM INTEGRATION GUIDE)
**Dự án:** Hệ Thống Quản Lý Chi Tiêu Và Hoạch Định Tài Chính Cá Nhân Thông Minh (Tích Hợp AI)  
**Công nghệ:** .NET MAUI 10 (C# / XAML) — Kiến trúc Clean MVVM (CommunityToolkit.Mvvm)  
**Mục đích tài liệu:** Hướng dẫn chi tiết cách ghép nối an toàn giữa giao diện người dùng (Frontend View) và hệ thống xử lý (ViewModel / Backend API), bảo đảm bạn có thể **toàn quyền tự do sáng tạo thiết kế mỹ thuật, layout, màu sắc và phong cách** mà **không làm đứt gãy kết nối dữ liệu logic**.

---

## 1. NGUYÊN TẮC CỐT LÕI KHI THIẾT KẾ & GHÉP NỐI

Toàn bộ hệ thống Backend, API, ViewModels và Mock Repositories cho **9 Phân hệ (63 Use Cases)** đã được xây dựng và kiểm thử hoàn chỉnh 100%. Các màn hình `.xaml` hiện có trong thư mục `Views/` đóng vai trò là **bộ khung sườn mẫu (Wireframe / Data Anchors)**.

Khi bạn tiến hành thiết kế lại giao diện, hãy lưu ý **3 nguyên tắc vàng** sau:

### 1.1. Bạn toàn quyền quyết định về mặt Thiết kế & Thẩm mỹ
- Bạn được **tự do 100%** trong việc:
  - Chọn phong cách thiết kế (Dark Theme, Light Theme, Neumorphism, Glassmorphism, Material 3...).
  - Chọn bảng màu, font chữ, icon, kích thước, khoảng cách (`Padding`, `Margin`).
  - Thay đổi cấu trúc bố cục (dùng `Grid`, `StackLayout`, `FlexLayout`, `Border`, `Frame`, `CardView`...).
  - Thêm hoạt ảnh, hiệu ứng chuyển động, thư viện biểu đồ tùy ý bạn.

### 1.2. Giữ nguyên vẹn "Mỏ neo dữ liệu" (Binding Names & Commands)
- Để giao diện hiển thị đúng dữ liệu từ Backend và người dùng bấm nút có phản hồi, **tuyệt đối KHÔNG xóa hoặc đổi tên** các thuộc tính đang được Binding:
  - **Thuộc tính hiển thị dữ liệu:** `{Binding TênThuộcTính}` (Ví dụ: `{Binding Balance}`, `{Binding Title}`).
  - **Lệnh tương tác nút bấm:** `{Binding TênLệnhCommand}` (Ví dụ: `{Binding LoadDataCommand}`, `{Binding SubmitCommand}`).
  - **Tham số dòng danh sách:** `CommandParameter="{Binding .}"` (Dùng khi bấm nút trên một dòng của `CollectionView` để truyền item đó vào ViewModel).
- Bạn có thể chuyển đổi Control thoải mái (ví dụ: đổi `Label` thành `Button`, đổi `Entry` thành `Editor`, đổi thẻ `Border` thành `Frame`), miễn là giữ nguyên đường dẫn `{Binding ...}` bên trong.

### 1.3. Chế độ kiểm thử giao diện độc lập (Offline Mock vs Real Backend)
- Trong quá trình làm giao diện, bạn **không cần cài đặt database hay bật Backend**. Hệ thống đã có sẵn Mock Repository chứa dữ liệu mẫu sống động cho toàn bộ 20 màn hình.
- Để kiểm tra hoặc chuyển đổi, bạn mở file `ExpenseTracker.Maui/MauiProgram.cs` (từ dòng 98 đến 105):
  ```csharp
  // DÙNG DỮ LIỆU MẪU ĐỂ LÀM GIAO DIỆN OFFLINE (KHUYÊN DÙNG):
  builder.Services.AddScoped<IWalletRepository, MockWalletRepository>();
  builder.Services.AddScoped<ITransactionRepository, MockTransactionRepository>();
  builder.Services.AddScoped<IBudgetRepository, MockBudgetRepository>();
  builder.Services.AddScoped<INotificationRepository, MockNotificationRepository>();
  builder.Services.AddScoped<IAnalyticsReportRepository, MockAnalyticsReportRepository>();
  builder.Services.AddScoped<IForecastingRepository, MockForecastingRepository>();
  builder.Services.AddScoped<INudgeRepository, MockNudgeRepository>();
  builder.Services.AddScoped<ICoolingOffRepository, MockCoolingOffRepository>();
  builder.Services.AddScoped<IAutonomousActionRepository, MockAutonomousActionRepository>();

  // KHI CẦN KẾT NỐI BACKEND THẬT THÌ ĐỔI 'Mock' THÀNH 'Api':
  // builder.Services.AddScoped<IWalletRepository, ApiWalletRepository>();
  // builder.Services.AddScoped<IAutonomousActionRepository, ApiAutonomousActionRepository>();
  ```
- **Cấu hình địa chỉ máy chủ Backend** (trong `MauiProgram.cs`):
  - Chạy trên Windows Desktop: `"http://localhost:5018"`.
  - Chạy trên Máy ảo Android (Android Emulator): `"http://10.0.2.2:5018"`.
  - Chạy trên Điện thoại thật: thay địa chỉ trong mã nguồn bằng IP Wi-Fi cục bộ của máy chạy backend, ví dụ `"http://192.168.1.15:5018"`.
  - Backend hiện lắng nghe HTTP tại cổng `5018`; không dùng cổng `5000`.

---

## 2. BỘ QUY CHUẨN XỬ LÝ TRẠNG THÁI GIAO DIỆN (UI STATES)

Mỗi ViewModel đều kế thừa từ `BaseViewModel` và cung cấp sẵn 3 cờ trạng thái chuẩn mực để bạn thiết kế phản hồi cho người dùng:

1. **Trạng thái đang tải (Loading State):**
   - Thuộc tính: `IsBusy` (`bool`).
   - Ghép nối: Dùng để bật/tắt vòng xoay loading:
     ```xml
     <ActivityIndicator IsRunning="{Binding IsBusy}" IsVisible="{Binding IsBusy}" />
     ```
2. **Trạng thái thông báo lỗi (Error State):**
   - Thuộc tính: `HasError` (`bool`) và `ErrorMessage` (`string`).
   - Ghép nối: Dùng để hiển thị biểu ngữ hoặc popup báo lỗi khi nhập sai hoặc mất kết nối:
     ```xml
     <Border IsVisible="{Binding HasError}">
         <Label Text="{Binding ErrorMessage}" />
     </Border>
     ```
3. **Trạng thái thông báo thành công (Success / Status State):**
   - Thuộc tính: `StatusMessage` (`string`).
   - Ghép nối: Hiển thị dòng thông báo khi giao dịch thành công hoặc cần nhắc nhở nhẹ:
     ```xml
     <Label Text="{Binding StatusMessage}" />
     ```
4. **Trạng thái danh sách rỗng (Empty State):**
   - Trong thẻ `<CollectionView>`, luôn thiết kế phần `<CollectionView.EmptyView>` để khi người dùng chưa có giao dịch/ví/thông báo nào thì màn hình hiển thị hình minh họa và lời nhắn thân thiện thay vì để màn hình trắng trơn.

---

## 3. BẢN ĐỒ CHI TIẾT KHẾ ƯỚC GHÉP NỐI (DATA BINDING CONTRACTS) CHO TỪNG MÀN HÌNH

Dưới đây là danh sách toàn bộ 20 màn hình trong `Views/`, liệt kê chính xác các biến dữ liệu và lệnh xử lý của ViewModel để bạn đối soát khi thiết kế:

---

### PHÂN HỆ 1: TÀI KHOẢN & ĐA VÍ CÁ NHÂN (`UC-01`)

#### 1. `LoginPage.xaml` & `RegisterPage.xaml` (ViewModel: `LoginViewModel` & `RegisterViewModel`)
- **Dữ liệu 2 chiều (Mode=TwoWay):**
  - Form đăng nhập: `{Binding Username}`, `{Binding Password}`.
  - Form đăng ký: `{Binding FullName}`, `{Binding Email}`, `{Binding Username}`, `{Binding Password}`, `{Binding ConfirmPassword}`.
- **Lệnh tương tác (Commands):**
  - Đăng nhập: `{Binding LoginCommand}`
  - Đăng ký: `{Binding RegisterCommand}`
  - Chuyển trang qua lại: `{Binding NavigateToRegisterCommand}`, `{Binding NavigateToLoginCommand}`

#### 2. `WalletsPage.xaml` (ViewModel: `WalletsViewModel`)
- **Dữ liệu hiển thị:**
  - Tổng số dư tài sản: `{Binding TotalBalance, StringFormat='{0:N0} đ'}`
  - Danh sách ví: `ItemsSource="{Binding Wallets}"` (Các trường con: `Name`, `Balance`, `WalletTypeDisplay`, `Currency`, `AccountNumber`, `BankName`, `ColorHex`).
- **Lệnh tương tác (Commands):**
  - Tải lại danh sách ví: `{Binding LoadWalletsCommand}`
  - Mở form tạo ví mới: `{Binding OpenCreateWalletCommand}`
  - Mở form chuyển tiền nội bộ: `{Binding OpenTransferCommand}`
  - Mở form cân chỉnh số dư: `{Binding OpenAdjustBalanceCommand}`
  - Xóa ví: `{Binding DeleteWalletCommand}` kèm `CommandParameter="{Binding .}"`

---

### PHÂN HỆ 2: QUẢN LÝ THU NHẬP & AI OCR PHIẾU LƯƠNG (`UC-02`)

#### 3. `IncomesPage.xaml` (ViewModel: `IncomesViewModel`)
- **Dữ liệu hiển thị:**
  - Tổng thu nhập tháng: `{Binding TotalIncomeThisMonth}`
  - Danh sách khoản thu: `ItemsSource="{Binding Incomes}"` (Các trường con: `Amount`, `IncomeDate`, `CategoryName`, `WalletName`, `Note`).
- **Lệnh tương tác (Commands):**
  - Tải dữ liệu: `{Binding LoadIncomesCommand}`
  - Tạo khoản thu mới: `{Binding CreateIncomeCommand}`

#### 4. `RecurringIncomesPage.xaml` (ViewModel: `RecurringIncomesViewModel`)
- **Dữ liệu hiển thị:**
  - Danh sách thu định kỳ: `ItemsSource="{Binding RecurringItems}"` (Trường con: `Name`, `ExpectedAmount`, `FrequencyDisplay`, `NextDueDate`, `IsActive`).
- **Lệnh tương tác:**
  - Bật/Tắt trạng thái hoạt động: `{Binding ToggleStatusCommand}` kèm `CommandParameter="{Binding .}"`
  - Tạo khoản thu định kỳ: `{Binding CreateRecurringCommand}`

#### 5. `PayslipOcrPage.xaml` (ViewModel: `PayslipOcrViewModel`)
- **Dữ liệu hiển thị:**
  - Đường dẫn ảnh/PDF phiếu lương đã chọn: `{Binding SelectedImagePath}`
  - Kết quả AI bóc tách: `{Binding ParsedPayslip}` (Trường con: `GrossSalary`, `NetSalary`, `Bonuses`, `InsuranceDeduction`, `TaxDeduction`, `ConfidenceScore`).
  - Cờ trạng thái: `{Binding IsProcessingOcr}` (Dùng để hiện vòng xoay AI đang đọc phiếu lương).
- **Lệnh tương tác:**
  - Chọn ảnh từ thư viện thiết bị: `{Binding PickImageCommand}`
  - Chụp ảnh phiếu lương từ camera: `{Binding CaptureImageCommand}`
  - Xác nhận lưu vào hệ thống: `{Binding ConfirmAndSaveCommand}`

---

### PHÂN HỆ 3: QUẢN LÝ CHI TIÊU, HÓA ĐƠN AI OCR & TIỆN ÍCH (`UC-03`)

#### 6. `ExpensesPage.xaml` (ViewModel: `ExpensesViewModel`)
- **Dữ liệu hiển thị:**
  - Tổng chi tiêu tháng: `{Binding TotalExpenseThisMonth}`
  - Danh sách chi tiêu: `ItemsSource="{Binding Expenses}"` (Trường con: `Amount`, `TransactionDate`, `CategoryName`, `WalletName`, `Note`, `IsEssential`).
- **Lệnh tương tác:**
  - Tải danh sách: `{Binding LoadExpensesCommand}`
  - Thêm chi tiêu: `{Binding CreateExpenseCommand}`
  - Lọc theo danh mục/thời gian: `{Binding FilterExpensesCommand}`

#### 7. `ReceiptOcrPage.xaml` (ViewModel: `ReceiptOcrViewModel`)
- **Dữ liệu hiển thị:**
  - Ảnh hóa đơn: `{Binding ReceiptPreviewImage}`
  - Danh sách từng món hàng AI bóc tách: `ItemsSource="{Binding ReceiptItems}"` (Trường con: `ItemName`, `Quantity`, `UnitPrice`, `TotalPrice`, `SuggestedCategory`).
  - Tổng tiền hóa đơn: `{Binding TotalReceiptAmount}`
  - Cờ cảnh báo sai lệch số học: `{Binding HasArithmeticDiscrepancy}`
- **Lệnh tương tác:**
  - Chụp ảnh hóa đơn: `{Binding CaptureReceiptCommand}`
  - Chọn ảnh hóa đơn: `{Binding PickReceiptCommand}`
  - Lưu thành giao dịch chi tiêu: `{Binding SaveReceiptAsExpenseCommand}`

#### 8. `SmartUtilitiesPage.xaml` (ViewModel: `SmartUtilitiesViewModel`)
- **Dữ liệu hiển thị:**
  - Văn bản nhận diện giọng nói: `{Binding VoiceSpokenText}`
  - Chuỗi mã thanh toán VietQR Napas 247: `{Binding VietQrPayload}`
  - Danh sách gói thuê bao ngủ quên (Zombie): `ItemsSource="{Binding ZombieSubscriptions}"`
- **Lệnh tương tác:**
  - Nhấn giữ ghi âm giọng nói: `{Binding StartVoiceRecordingCommand}` / `{Binding StopVoiceRecordingCommand}`
  - Tạo mã VietQR: `{Binding GenerateVietQrCommand}`
  - Đánh dấu đã hủy thuê bao: `{Binding CancelSubscriptionCommand}`

---

### PHÂN HỆ 4: DANH MỤC THU CHI & QUY TẮC THÔNG MINH (`UC-04`)

#### 9. `CategoriesPage.xaml` (ViewModel: `CategoriesViewModel`)
- **Dữ liệu hiển thị:**
  - Cây danh mục Cha - Con: `ItemsSource="{Binding Categories}"` (Trường con: `Name`, `IconKey`, `ColorHex`, `TransactionType`, `BudgetRuleMappingDisplay`, `ParentCategoryId`, `SubCategories`).
- **Lệnh tương tác:**
  - Thêm danh mục: `{Binding CreateCategoryCommand}`
  - Sửa danh mục: `{Binding UpdateCategoryCommand}`
  - Xóa danh mục: `{Binding DeleteCategoryCommand}`

#### 10. `AutoRulesPage.xaml` (ViewModel: `AutoRulesViewModel`)
- **Dữ liệu hiển thị:**
  - Danh sách quy tắc gán nhãn: `ItemsSource="{Binding Rules}"` (Trường con: `KeywordPattern`, `TargetCategoryName`, `Priority`, `IsActive`).
- **Lệnh tương tác:**
  - Thêm quy tắc mới: `{Binding CreateRuleCommand}`
  - Áp dụng hồi tố cho các chi tiêu cũ: `{Binding ApplyRetroactiveCommand}`

---

### PHÂN HỆ 5: NGÂN SÁCH ĐA PHƯƠNG PHÁP & HẠN MỨC DSA (`UC-05`)

#### 11. `BudgetsPage.xaml` & `CreateBudgetPage.xaml` (ViewModel: `BudgetsViewModel` & `CreateBudgetViewModel`)
- **Dữ liệu hiển thị:**
  - Tên kế hoạch & Tổng ngân sách: `{Binding ActiveBudget.Name}`, `{Binding ActiveBudget.TotalAmount}`
  - Tổng thực chi & Tỷ lệ %: `{Binding ActiveBudget.CurrentSpent}`, `{Binding ActiveBudget.SpentPercent}`
  - Hạn mức chi tiêu mỗi ngày thích ứng (DSA): `{Binding ActiveBudget.AdaptiveDailyAllowance}`
  - Chỉ số tốc độ tiêu tiền: `{Binding ActiveBudget.SpendingVelocityIndex}`
  - Dự phóng ngày cạn tiền: `{Binding ActiveBudget.OutOfMoneyForecastDate}`
  - Danh sách phân bổ từng danh mục: `ItemsSource="{Binding ActiveBudget.Allocations}"` (Trường con: `CategoryName`, `AllocatedAmount`, `CurrentSpent`, `SpentPercent`, `IsOverBudget`).
- **Lệnh tương tác:**
  - Tải ngân sách: `{Binding LoadActiveBudgetCommand}`
  - Mở trang tạo ngân sách chu kỳ mới: `{Binding NavigateToCreateBudgetCommand}`
  - Tái cân đối hạn mức danh mục: `{Binding RebalanceCategoryCommand}`

---

### PHÂN HỆ 6: QUỸ TÍCH LŨY & NGHĨA VỤ NỢ (`UC-06`)

#### 12. `SavingsGoalsPage.xaml` & `CreateSavingsGoalPage.xaml` (ViewModel: `SavingsGoalsViewModel` & `CreateSavingsGoalViewModel`)
- **Dữ liệu hiển thị:**
  - Danh sách mục tiêu / Hũ tài chính: `ItemsSource="{Binding Goals}"` (Trường con: `Title`, `TargetAmount`, `CurrentAmount`, `ProgressPercentage`, `TargetDate`, `CategoryJarTypeDisplay`).
  - Bảng nghĩa vụ nợ & Tỷ lệ DTI: `{Binding DebtToIncomeRatio}`
- **Lệnh tương tác:**
  - Nạp tiền vào quỹ tích lũy: `{Binding DepositFundsCommand}` kèm `CommandParameter="{Binding .}"`
  - Rút tiền từ quỹ (kèm cú hích nhập lý do): `{Binding WithdrawFundsCommand}` kèm `CommandParameter="{Binding .}"`
  - Chạy mô phỏng trả nợ (Snowball vs Avalanche): `{Binding SimulateDebtPayoffCommand}`

---

### PHÂN HỆ 7: BÁO CÁO & PHÂN TÍCH TÀI CHÍNH ĐA CHIỀU (`UC-07`)

#### 13. `ReportsPage.xaml` (ViewModel: `ReportsViewModel`)
- **Dữ liệu hiển thị:**
  - Báo cáo Thu - Chi đối sánh: `{Binding IncomeExpenseReport.TotalIncome}`, `{Binding IncomeExpenseReport.TotalExpense}`, `{Binding IncomeExpenseReport.NetSavings}`
  - Báo cáo Giá trị Tài sản ròng: `{Binding NetWorthReport.CurrentNetWorth}`, `{Binding NetWorthReport.TotalAssets}`, `{Binding NetWorthReport.TotalLiabilities}`
  - Dòng tiền ròng (Net Cashflow): `{Binding CashflowReport.OperatingCashflow}`, `{Binding CashflowReport.SavingsRate}`
  - Danh sách khoản chi đột biến (Z-Score Outlier): `ItemsSource="{Binding OutlierExpenses}"`
- **Lệnh tương tác:**
  - Đổi khoảng thời gian lọc (Tuần / Tháng / Quý / Năm): `{Binding SelectPeriodCommand}`
  - Ghi nhận ảnh chụp nhanh Net Worth: `{Binding TakeNetWorthSnapshotCommand}`

---

### PHÂN HỆ 8: DỰ BÁO CHUỖI THỜI GIAN & RADAR THÂM HỤT (`UC-08`)

#### 14. `ForecastingPage.xaml` (ViewModel: `ForecastingViewModel`)
- **Dữ liệu hiển thị:**
  - Dự báo tổng chi cuối kỳ: `{Binding Forecast.EstimatedTotalSpent}`, `{Binding Forecast.ProjectedOverspendAmount}`, `{Binding Forecast.RiskLevel}`
  - Vận tốc tiêu tiền ngày thường vs cuối tuần: `{Binding Forecast.WeekdayDailyVelocity}`, `{Binding Forecast.WeekendDailyVelocity}`
  - Radar thanh khoản: `{Binding Liquidity.DaysUntilDepletion}`, `{Binding Liquidity.DeficitDaysBeforeSalary}`, `{Binding Liquidity.IsLiquidityCrisis}`
  - Danh sách danh mục có nguy cơ thâm hụt: `ItemsSource="{Binding CategoryRisks}"` (Trường con: `CategoryName`, `RiskRatio`, `DaysUntilDepletion`, `RiskLevelDisplay`).
  - Dữ liệu Sandbox What-If thử nghiệm:
    - Số tiền giả định nhập vào: `{Binding LumpSumAmount}` (TwoWay với Slider hoặc Entry).
    - Kết quả mô phỏng Trước vs Sau: `{Binding WhatIfResult.BeforeNetWorth}`, `{Binding WhatIfResult.AfterNetWorth}`, `{Binding WhatIfResult.BeforeDsa}`, `{Binding WhatIfResult.AfterDsa}`.
- **Lệnh tương tác:**
  - Tải dữ liệu dự báo: `{Binding LoadForecastDataCommand}`
  - Kích hoạt chạy mô phỏng Sandbox What-If: `{Binding RunWhatIfSimulationCommand}`

---

### PHÂN HỆ 9: CÚ HÍCH THALER NUDGE, KHOẢNG TRỄ 24H & TRỢ LÝ XAI (`UC-09`)

#### 15. `NudgeFeedPage.xaml` (ViewModel: `NudgeFeedViewModel`)
- **Dữ liệu hiển thị:**
  - Danh sách Cú hích đang hoạt động: `ItemsSource="{Binding Nudges}"` (Trường con: `Title`, `Message`, `NudgeTypeDisplay`, `NudgeBadgeColor`, `TriggerContext`, `HasRebalanceProposal`).
  - Điều khiển bật/tắt Modal giải trình XAI: `IsVisible="{Binding IsXaiModalVisible}"`
  - Dữ liệu chi tiết bên trong Modal XAI:
    - Tiêu đề & Hiện trạng kích hoạt: `{Binding SelectedNudge.Title}`, `{Binding ActiveXaiRationale.TriggerContext}`
    - Công thức toán học minh bạch: `{Binding ActiveXaiRationale.MathematicalFormula}`
    - Bằng chứng thực nghiệm: `{Binding ActiveXaiRationale.EmpiricalEvidence}`
    - Dự phóng hệ quả tương lai: `{Binding ActiveXaiRationale.ProjectedConsequences}`, `{Binding ActiveXaiRationale.ProjectedDailyDsaAfter, StringFormat='{0:N0} đ/ngày'}`
- **Lệnh tương tác:**
  - Tải danh sách cú hích: `{Binding LoadNudgesCommand}`
  - Mở Modal xem giải trình XAI: `{Binding ViewXaiRationaleCommand}` kèm `CommandParameter="{Binding .}"`
  - Đóng Modal XAI: `{Binding CloseXaiModalCommand}`
  - Phê duyệt áp dụng đề xuất tái cân đối: `{Binding ApproveNudgeCommand}` kèm `CommandParameter="{Binding .}"`
  - Bỏ qua cú hích: `{Binding DismissNudgeCommand}` kèm `CommandParameter="{Binding .}"`

#### 16. `CoolingOffHubPage.xaml` (ViewModel: `CoolingOffHubViewModel`)
- **Dữ liệu hiển thị:**
  - Danh sách khoản chi lớn bị phong tỏa 24h: `ItemsSource="{Binding PendingTransactions}"` (Trường con: `Description`, `CategoryName`, `Amount`, `RemainingTimeDisplay`, `EquivalentWorkHours`, `ProjectedDsaReduction`, `IsExpired`).
  - Thử nghiệm đánh giá khoản chi (Sandbox):
    - Số tiền nhập thử: `{Binding TestExpenseAmount}` (TwoWay)
    - Tên món đồ: `{Binding TestDescription}` (TwoWay)
  - Điều khiển Modal kết quả đánh giá ma sát: `IsVisible="{Binding IsEvaluateModalVisible}"`
  - Điều khiển Modal mở khóa khẩn cấp: `IsVisible="{Binding IsBypassModalVisible}"`
    - Lý do khẩn cấp nhập vào: `{Binding BypassReason}` (TwoWay với Editor)
- **Lệnh tương tác:**
  - Tải danh sách hạ nhiệt: `{Binding LoadPendingTransactionsCommand}`
  - Đánh giá thử nghiệm khoản chi lớn: `{Binding EvaluateTestExpenseCommand}`
  - Đóng modal đánh giá: `{Binding CloseEvaluateModalCommand}`
  - Xác nhận mua (trừ tiền khi hết giờ): `{Binding ConfirmPurchaseCommand}` kèm `CommandParameter="{Binding .}"`
  - Hủy mua hàng (tiết kiệm tiền thành công): `{Binding CancelPurchaseCommand}` kèm `CommandParameter="{Binding .}"`
  - Mở modal giải trình khẩn cấp: `{Binding OpenBypassModalCommand}` kèm `CommandParameter="{Binding .}"`
  - Đóng modal khẩn cấp: `{Binding CloseBypassModalCommand}`
  - Gửi lý do mở khóa khẩn cấp: `{Binding SubmitBypassCommand}`

---

### PHÂN HỆ 10: ĐỘNG CƠ HÀNH ĐỘNG TỰ HÀNH & PHÂN TẦNG XÁC SUẤT JEV (CHẶNG 10.5 / PLAN-47)

#### 17. `ActionInboxView.xaml` & `SmartActionCardView.xaml` (ViewModel: `ActionInboxViewModel`, tích hợp tại đầu `WalletsPage.xaml`)
- **Mục đích & Cơ chế hoạt động:**
  - Khay chứa các thẻ hành động tự hành được sinh ra khi máy chủ nhận diện biến động số dư ngân hàng hoặc ví điện tử.
  - Tự động co lại / ẩn hoàn toàn (`IsVisible = false`) khi khay rỗng (`HasPendingActions = false`), bảo đảm không làm phiền người dùng khi không có việc cần xử lý.
- **Dữ liệu hiển thị (Data Context: `ActionInboxViewModel`):**
  - Số lượng việc đang chờ: `{Binding ActionCount}` (dùng cho tiêu đề: `⚡ Việc Cần Xử Lý ({0})`).
  - Thông báo phản hồi sau khi lưu: `{Binding ActionStatusMessage}`.
  - Danh sách thẻ hành động: `ItemsSource="{Binding PendingActions}"` (phần tử: `AutonomousActionModel`).
- **Chi tiết dữ liệu từng thẻ (`SmartActionCardView.xaml` - Data Context: `AutonomousActionModel`):**
  - Nhãn phân loại: `{Binding Display.BadgeText}` (Mã màu gợi ý: `{Binding Display.BadgeColor}`).
  - Nguồn giao dịch & Thời gian: `{Binding Display.Subtitle}` (Ví dụ: `"ACB • 23:15"`, `"MoMo • 14:20"`).
  - Tiêu đề & Số tiền: `{Binding Display.Title}` (Ví dụ: `"Shopee: 185.000 đ"`, `"Circle K: 35.000 đ"`).
  - Số tiền nguyên bản: `{Binding Display.Amount, StringFormat='{0:N0} đ'}`.
  - **Nhánh A - Xác suất cao (Confidence $\ge 0.85$):**
    - Điều kiện hiển thị: `IsVisible="{Binding PrimaryAction, Converter={StaticResource NullToFalseConverter}}"`.
    - Nút Lưu nhanh 1-chạm: `{Binding PrimaryAction.Label}` (gọi `{Binding QuickCommitCommand}` kèm `CommandParameter="{Binding .}"`).
    - Nút Đổi chi tiết: `{Binding SecondaryAction.Label}` (gọi `{Binding NavigateCommand}` kèm `CommandParameter="{Binding .}"`).
  - **Nhánh B - Phân tầng Jev Choice ($0.50 \le \text{Confidence} < 0.85$):**
    - Điều kiện hiển thị: `IsVisible="{Binding Choices, Converter={StaticResource NullToFalseConverter}}"`.
    - Danh sách lựa chọn: `BindableLayout.ItemsSource="{Binding Choices}"` (mỗi phần tử có `{Binding Label}` và `{Binding FormattedProbability}`).
    - Lệnh khi bấm vào 1 lựa chọn: gọi `{Binding SelectChoiceCommand}` kèm `CommandParameter="{Binding .}"`.
  - **Nút Bỏ qua (Dismiss):** Bấm nút `[✕]` gọi `{Binding DismissCommand}` kèm `CommandParameter="{Binding .}"` để dọn sạch thẻ khỏi khay.
- **Lệnh tương tác trên khay:**
  - Tải lại danh sách: `{Binding LoadInboxCommand}`.

---

### TIỆN ÍCH CHUNG & ĐIỀU HƯỚNG

#### 18. `NotificationHistoryPage.xaml` (ViewModel: `NotificationHistoryViewModel`)
- **Dữ liệu hiển thị:**
  - Danh sách thông báo đẩy & nhắc nhở 21:00: `ItemsSource="{Binding Notifications}"` (Trường con: `Title`, `Message`, `CreatedAt`, `IsRead`, `SeverityColor`).
- **Lệnh tương tác:**
  - Đánh dấu tất cả đã đọc: `{Binding MarkAllAsReadCommand}`
  - Xóa thông báo: `{Binding DeleteNotificationCommand}` kèm `CommandParameter="{Binding .}"`

#### 19. `AppShell.xaml` & `AppShell.xaml.cs`
- Chứa cấu trúc điều hướng toàn ứng dụng (`TabBar`, `FlyoutItem`, `ShellContent`).
- Các Route đã được đăng ký đầy đủ: `LoginPage`, `WalletsPage`, `IncomesPage`, `RecurringIncomesPage`, `PayslipOcrPage`, `CategoriesPage`, `ExpensesPage`, `ReceiptOcrPage`, `SmartUtilitiesPage`, `BudgetsPage`, `CreateBudgetPage`, `SavingsGoalsPage`, `CreateSavingsGoalPage`, `ReportsPage`, `ForecastingPage`, `NudgeFeedPage`, `CoolingOffHubPage`, `NotificationHistoryPage`.
- Khi cần tạo thanh điều hướng chân trang (Bottom Navigation Bar) hoặc Menu trượt cạnh bên (Flyout Drawer), bạn thoải mái tinh chỉnh lại thẻ `<TabBar>` hoặc `<FlyoutItem>` trong file `AppShell.xaml`.

---

## 4. TỔNG KẾT & LỜI NHẮN

Mọi kết nối dữ liệu đã được mắc nối sẵn sàng. Khi bạn thiết kế:
1. **Thoải mái sáng tạo giao diện:** Bạn là người làm chủ 100% phần nhìn, trải nghiệm và mỹ thuật của ứng dụng.
2. **Đối chiếu mỏ neo Binding:** Trước khi xóa một control cũ trong wireframe, hãy nhìn xem nó đang bind thuộc tính hay command nào, sau đó gắn lại đúng tên đó vào control mới của bạn.
3. Nếu bạn muốn thêm một trường hiển thị mới hoặc muốn đổi cách định dạng dữ liệu, bạn chỉ cần báo nhóm, nhóm sẽ hỗ trợ cập nhật ViewModel hoặc Converter ngay lập tức!

Chúc bạn có một trải nghiệm thiết kế giao diện thật hứng khởi và cùng nhau tạo nên đồ án DuoNoPro xuất sắc nhất! 🚀
