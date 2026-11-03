# CẨM NANG TOÀN DIỆN CHUYỂN GIAO THIẾT KẾ OPEN DESIGN SANG .NET MAUI 10
**Dự án:** DuoNoPro - Hệ Thống Quản Lý Chi Tiêu & Hoạch Định Tài Chính Cá Nhân Thông Minh (Tích Hợp AI)  
**Tập tin nguyên mẫu Open Design:** `Open Design/duonopro-mobile-app/duonopro-mobile-app.html`  
**Hệ thống Design System:** `user:duonopro-fintech` (Dark Luxury Fintech)  
**Địa chỉ Live Preview:** `http://127.0.0.1:55607/b28443b7e759539e9b19c9263ff4fdca/?entry=Open%20Design%2Fduonopro-mobile-app%2Fduonopro-mobile-app.html`  

---

## 1. HỆ THỐNG MÃ MÀU VÀ DESIGN TOKENS CHUẨN XAML (.NET MAUI)

Toàn bộ các biến CSS trong `tokens.css` đã được ánh xạ tương ứng vào `ExpenseTracker.Maui/Resources/Styles/Colors.xaml` và `Styles.xaml`:

| Token CSS | Hex / RGB | Resource Key XAML | Ứng Dụng Trong Giao Diện |
| :--- | :---: | :--- | :--- |
| `--bg` | `#0B0F19` | `ColorBackgroundDark` | Nền chính của ứng dụng (Deep Space Black) |
| `--surface` | `#111827` | `ColorSurfaceDark` | Nền thẻ Card chính, thanh Header, Bottom Bar |
| `--surface-warm` | `#1E293B` | `ColorCardDark` | Nền dòng giao dịch, ô nhập liệu, hộp thoại phụ |
| `--border` | `#334155` | `ColorBorderDark` | Viền ngăn cách giữa các khối (Stroke 0.5pt - 1pt) |
| `--accent` | `#4F46E5` | `ColorPrimaryBrand` | Nút hành động chính (Electric Indigo), Tab Active |
| `--success` | `#10B981` | `ColorIncomeEmerald`| Tiền vào, DSA an toàn (< 70%), trạng thái tốt |
| `--danger` | `#EF4444` / `#F43F5E`| `ColorExpenseRose` | Tiền ra, cảnh báo bội chi (> 90%), thâm hụt |
| `--warn` | `#F59E0B` | `ColorGoldChip` | Chip thẻ Obsidian VIP, cảnh báo vi mô |
| `--fg` | `#F8FAFC` | `ColorTextPrimary` | Số dư lớn, tiêu đề chính |
| `--muted` | `#94A3B8` | `ColorTextMuted` | Chú thích, metadata, nhãn phụ, placeholder |

---

## 2. MA TRẬN ĐỐI SOÁT KHẾ ƯỚC DỮ LIỆU (DATA BINDING CONTRACTS) CHO 10 MÀN HÌNH

Dưới đây là bảng đối soát chi tiết giữa giao diện nguyên mẫu Open Design và các thuộc tính/lệnh của ViewModel trong .NET MAUI để đảm bảo khi áp dụng style, **100% logic và dữ liệu hoạt động mượt mà không bị ngắt kết nối**:

### 1. `WalletsPage.xaml` (ViewModel: `WalletsViewModel`)
- **Số dư Net Worth Hero:** `{Binding TotalBalance, StringFormat='{0:N0} ₫'}`
- **Hạn mức DSA hôm nay:** `{Binding DailySpendingAllowance, StringFormat='{0:N0} ₫ / ngày'}`
- **Danh sách ví ACID:** `ItemsSource="{Binding Wallets}"`
  - Tên ví: `{Binding Name}`
  - Số dư từng ví: `{Binding Balance, StringFormat='{0:N0} ₫'}`
  - Loại ví: `{Binding WalletType}`
- **Lệnh tương tác:**
  - Nạp ví / Cân chỉnh: `Command="{Binding AdjustBalanceCommand}"`
  - Thêm ví mới: `Command="{Binding CreateWalletCommand}"`
  - Mở Voice AI FAB: `Command="{Binding OpenVoiceModalCommand}"`

### 2. `ExpensesPage.xaml` (ViewModel: `ExpensesViewModel`)
- **Danh sách chi tiêu:** `ItemsSource="{Binding Transactions}"`, `IsGrouped="True"`, Header: `{Binding Key}`
- **Chi tiết giao dịch:**
  - Tiêu đề: `{Binding Title}`
  - Số tiền: `{Binding Amount, StringFormat='-{0:N0} ₫'}`
  - Danh mục: `{Binding CategoryName}`
  - Ví thanh toán: `{Binding WalletName}`
- **AI OCR Hóa đơn:**
  - Nút quét camera: `Command="{Binding ScanReceiptOcrCommand}"`
  - Trạng thái kiểm toán: `IsVisible="{Binding IsMathematicallyAudited}"`
  - Bẫy thuê bao: `IsVisible="{Binding IsSubscriptionTrap}"`

### 3. `IncomesPage.xaml` & `RecurringIncomesPage.xaml` (ViewModel: `IncomesViewModel`)
- **Tổng thu nhập tháng Hero:** `{Binding FormattedMonthlyTotal}`
- **Lịch thu nhập định kỳ:** `ItemsSource="{Binding RecurringIncomes}"`
  - Nguồn thu: `{Binding SourceName}`
  - Số tiền định kỳ: `{Binding Amount, StringFormat='+{0:N0} ₫'}`
  - Ngày đến hạn: `{Binding DayOfMonth, StringFormat='Ngày {0} hàng tháng'}`
  - Tự động cộng ví: `{Binding TargetWalletName}`
- **OCR Bảng lương:** `Command="{Binding ScanPayslipOcrCommand}"`

### 4. `BudgetsPage.xaml` (ViewModel: `BudgetsViewModel`)
- **Quy tắc 50/30/20 & Kakeibo:** `ItemsSource="{Binding BudgetItems}"`
  - Tên nhóm ngân sách: `{Binding CategoryGroupName}`
  - Số tiền đã tiêu / Hạn mức: `{Binding SpentAmount, StringFormat='{0:N0} ₫'}` / `{Binding TargetLimit, StringFormat='{0:N0} ₫'}`
  - Tỷ lệ phần trăm thanh tiến độ: `{Binding Percentage}` (Màu xanh `< 70%`, vàng `70-90%`, đỏ `> 90%`)
- **Radar Tốc độ tiêu tiền:** `{Binding SpendingVelocity, StringFormat='{0:F2}x'}`
- **Dự phóng ngày cạn tiền:** `{Binding ForecastOutOfMoneyDate, StringFormat='{0:dd/MM/yyyy}'}`

### 5. `ReportsPage.xaml` (ViewModel: `ReportsViewModel`)
- **Thác nước 5 bước dòng tiền (Waterfall):**
  - Bước 1 (Tổng thu): `{Binding TotalIncome, StringFormat='+{0:N0} ₫'}`
  - Bước 2 (Chi cố định): `{Binding TotalFixedExpense, StringFormat='-{0:N0} ₫'}`
  - Bước 3 (Chi linh hoạt): `{Binding TotalVariableExpense, StringFormat='-{0:N0} ₫'}`
  - Bước 4 (Tích lũy): `{Binding TotalSavingsAllocated, StringFormat='-{0:N0} ₫'}`
  - Bước 5 (Dòng tiền ròng tự do): `{Binding NetFreeCashflow, StringFormat='{0:N0} ₫'}`
- **Kiểm định Z-Score:** `{Binding MaxZScoreValue}`, `{Binding IsAnomalyDetected}`

### 6. `SavingsGoalsPage.xaml` (ViewModel: `SavingsGoalsViewModel`)
- **Danh sách mục tiêu tích lũy:** `ItemsSource="{Binding Goals}"`
  - Tên mục tiêu: `{Binding Title}`
  - Tiến độ tích lũy: `{Binding CurrentAmount, StringFormat='{0:N0} ₫'}` / `{Binding TargetAmount, StringFormat='{0:N0} ₫'}`
  - Tỷ lệ %: `{Binding ProgressPercentage}`
- **Hũ Kakeibo 4 Cột Trụ:** `ItemsSource="{Binding KakeiboJars}"`
- **Nút nạp/rút tiền:** `Command="{Binding DepositCommand}"`, `Command="{Binding WithdrawWithNudgeCommand}"`

### 7. `ForecastingPage.xaml` (ViewModel: `ForecastingViewModel`)
- **Radar thâm hụt thanh khoản:**
  - Dự phóng ngày cạn tiền: `{Binding OutOfMoneyDate, StringFormat='{0:dd/MM/yyyy}'}`
  - Ngày lương kế tiếp: `{Binding NextSalaryDate, StringFormat='{0:dd/MM/yyyy}'}`
  - Khủng hoảng thanh khoản: `{Binding IsLiquidityCrisis}`
- **Phòng thí nghiệm What-If Sandbox:**
  - Mô phỏng sụt giảm thu nhập: `Command="{Binding SimulateIncomeDropCommand}"`
  - Mô phỏng vay mua xe DTI: `Command="{Binding SimulateLoanDtiCommand}"`
  - Mô phỏng chi lớn lump-sum: `Command="{Binding SimulateLumpSumCommand}"`

### 8. `NudgeFeedPage.xaml` & `CoolingOffHubPage.xaml` (ViewModel: `NudgeFeedViewModel` & `CoolingOffHubViewModel`)
- **Cú hích Thaler Nudge:**
  - Tiêu đề cú hích: `{Binding Title}`
  - Giải trình XAI 3 thành phần: `{Binding ExplanationSummary}` (Dữ liệu, Công thức DSA, Đề xuất)
  - Phê duyệt tái cân đối: `Command="{Binding ApproveRebalanceCommand}"`
- **Khoảng trễ tâm lý 24h:**
  - Danh sách khoản chi phong tỏa: `ItemsSource="{Binding FrozenExpenses}"`
  - Đồng hồ đếm ngược: `{Binding RemainingTimeFormatted}`
  - Giờ công quy đổi: `{Binding LaborHoursEquivalent, StringFormat='≈ {0:F1} giờ làm việc'}`
  - Hủy bỏ khoản chi: `Command="{Binding CancelExpenseCommand}"`
  - Mở khóa khẩn cấp: `Command="{Binding EmergencyUnlockCommand}"`

### 9. `SmartUtilitiesPage.xaml` & `AutoRulesPage.xaml` (ViewModel: `SmartUtilitiesViewModel` & `AutonomousActionViewModel`)
- **Khay hành động tự hành Server-Driven Action Protocol:**
  - Danh sách hành động: `ItemsSource="{Binding PendingActions}"`
  - Xác suất Jev: `{Binding JevProbability, StringFormat='{0:P0}'}`
  - Nút lưu nhanh 1-chạm: `Command="{Binding AutoExecuteActionCommand}"`
  - Nút lựa chọn (Jev Choice): `Command="{Binding ChooseCategoryCommand}"`
- **Tạo mã VietQR Napas 247:** `Command="{Binding GenerateVietQrCommand}"`, Ảnh QR: `{Binding QrCodeImageSource}`

### 10. `SettingsPage.xaml` & `LoginPage.xaml` (ViewModel: `SettingsViewModel` & `LoginViewModel`)
- **Trạng thái két an toàn:** `{Binding EncryptionStatus}` (AES-256-GCM)
- **Khóa sinh trắc học:** `{Binding IsBiometricEnabled}` (FaceID / Vân tay)
- **Chuyển đổi Dynamic Repository Mode Switcher:**
  - Cờ chuyển đổi: `{Binding IsLiveApiMode}`
  - Nhãn máy chủ: `{Binding ServerUrlLabel}` (`http://localhost:5018` vs `Offline Mock 0ms`)
- **Đăng xuất an toàn:** `Command="{Binding LogoutCommand}"`

---

## 3. CÁCH KIỂM TRA TRÊN BẢN PREVIEW
1. Mở liên kết Live Browser Preview:  
   `http://127.0.0.1:55607/b28443b7e759539e9b19c9263ff4fdca/?entry=Open%20Design%2Fduonopro-mobile-app%2Fduonopro-mobile-app.html`
2. Bấm vào bất kỳ nút nào trong 2 hàng tab phía trên để chuyển đổi tức thì giữa 10 màn hình.
3. Bản vẽ đã được tự động lưu và cập nhật thời gian thực vào trình duyệt.
