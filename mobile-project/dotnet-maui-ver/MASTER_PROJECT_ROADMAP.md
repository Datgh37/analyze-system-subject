# 🗺️ BẢN ĐỒ LỘ TRÌNH XÂY DỰNG TOÀN DIỆN DỰ ÁN (MASTER PROJECT ROADMAP)
## HỆ THỐNG QUẢN LÝ CHI TIÊU & HOẠCH ĐỊNH TÀI CHÍNH CÁ NHÂN THÔNG MINH (PFM)
### (.NET MAUI 10 + ASP.NET Core 10 + PostgreSQL 16 + SQLite Offline-First)

> **Dự án:** Smart Personal Expense and Financial Planning Management System with AI Integration  
> **Nguyên tắc triển khai:** Full-Stack Vertical Slice (Khép kín từng phân hệ), Incremental Unit Testing, 100% PFM không chia nhóm.  
> **Quy mô bao quát:** 10 Sơ đồ Use Case (**chính xác 63 Ca sử dụng chi tiết từ UC-01.01 đến UC-09.05**), 6 Bounded Contexts, 40 REST Endpoints, 3 SignalR Hub Handlers.  
> **Tài liệu căn cứ:** [`TECHNICAL_ARCHITECTURE_SPEC.md`](TECHNICAL_ARCHITECTURE_SPEC.md), [`DESIGN.md`](DESIGN.md), [`PRODUCT.md`](PRODUCT.md), [`05_engineering_architecture_and_code_craft_rules.md`](../../.agents/rules/05_engineering_architecture_and_code_craft_rules.md).

---

## 1. QUY ƯỚC CẤU TRÚC THƯ MỤC CHÍNH THỨC (REPOSITORY STRUCTURE)

Thống nhất phân định rạch ròi 2 thư mục gốc **`backend/`** và **`frontend/`** bên trong phân hệ .NET MAUI:

```text
mobile-project/dotnet-maui-ver/
├── backend/                                  # Toàn bộ mã nguồn máy chủ Backend (ASP.NET Core 10)
│   ├── ExpenseTracker.Domain/               # Core Domain: Rich Entities, Value Objects, Enums
│   ├── ExpenseTracker.Application/          # Use Cases: CQRS Commands/Queries, DTOs, FluentValidation
│   ├── ExpenseTracker.Infrastructure/       # Data Access: EF Core PostgreSQL, SignalR, External Services
│   ├── ExpenseTracker.WebApi/               # Presentation: Controllers, FinanceHub, Auth Middlewares
│   ├── tests/                               # Kiểm thử tự động Backend
│   │   ├── ExpenseTracker.Domain.UnitTests/
│   │   └── ExpenseTracker.Application.UnitTests/
│   ├── docker-compose.yml                   # Khởi chạy PostgreSQL 16 Local trong 1 click
│   └── ExpenseTracker.sln                   # Solution file điều phối toàn bộ Solution
│
├── frontend/                                 # Toàn bộ mã nguồn ứng dụng di động (.NET MAUI 10)
│   ├── ExpenseTracker.Maui/                 # Ứng dụng di động Clean MVVM
│   │   ├── Views/                           # Giao diện XAML Pages & Custom ContentView Cards
│   │   ├── ViewModels/                      # Reactive ViewModels (CommunityToolkit.Mvvm)
│   │   ├── Services/                        # Refit Client, Local SQLite, SignalR Service
│   │   ├── Resources/                       # Design System: Colors.xaml, Styles.xaml (Double-Bezel)
│   │   └── Platforms/                       # Native platform code (Android, Windows)
│   └── tests/
│       └── ExpenseTracker.Maui.UnitTests/   # Unit Tests cho ViewModels & Local Database Service
│
├── MASTER_PROJECT_ROADMAP.md                 # Tài liệu này: Lộ trình và bảng theo dõi tiến độ tổng thể
├── TECHNICAL_ARCHITECTURE_SPEC.md           # Đặc tả kiến trúc, 40 API Endpoints, DDL 12 bảng CSDL
├── DESIGN.md                                # Quy chuẩn thẩm mỹ Impeccable UI, Typography, Spacing 8pt
├── PRODUCT.md                               # Sứ mệnh sản phẩm, chân dung người dùng PFM
├── README.md                                # Hướng dẫn cài đặt và khởi chạy hệ thống
└── BAO_CAO_CUOI_KY_DOTNET_MAUI.docx         # Báo cáo tổng kết đồ án chuẩn hình thức KLTN DNTU
```

---

## 2. LỘ TRÌNH 8 GIAI ĐOẠN TRIỂN KHAI CUỐN CHIẾU (8-STAGE ROADMAP)

---

### GIAI ĐOẠN 0: KHỞI TẠO BỘ KHUNG HẠ TẦNG (SOLUTION SCAFFOLDING & DEVOPS)
- **Mục tiêu:** Dựng khung Solution hoàn chỉnh, cấu hình nghiêm ngặt C# 14 / .NET 10, Docker CSDL.
- **Phạm vi thực hiện:**
  * **`backend/`:** Khởi tạo Solution và 4 projects Clean Architecture (`Domain`, `Application`, `Infrastructure`, `WebApi`). Cấu hình `<Nullable>enable</Nullable>`, `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`. Thiết lập `docker-compose.yml` cho PostgreSQL 16.
  * **`frontend/`:** Khởi tạo project `ExpenseTracker.Maui` (Target: Windows Native Desktop & Android).
  * **Packages:** Cài đặt toàn bộ NuGet packages đã phê duyệt (`Npgsql.EntityFrameworkCore.PostgreSQL`, `CommunityToolkit.Mvvm`, `Refit`, `Polly`, `sqlite-net-pcl`, `FluentAssertions`, `xUnit`).
- **Quality Gate 0:** `dotnet build` đạt **0 Error, 0 Warning**; Docker PostgreSQL chạy khỏe mạnh; App MAUI khởi động thành công màn hình Welcome.

---

### GIAI ĐOẠN 1: PHÂN HỆ 1 — TÀI KHOẢN, ĐA VÍ ACID & ĐỒNG BỘ CỤC BỘ
- **Use Cases bao phủ (10 Use Cases):** `UC-01.01` đến `UC-01.10` (Đăng ký, Đăng nhập, Hồ sơ, Đổi mật khẩu, Quên mật khẩu, Cấu hình Đa Ví, Chuyển tiền nội bộ ACID, Cân chỉnh số dư, Bóc tách ảnh chụp số dư ví).
- **Phạm vi thực hiện:**
  * **`backend/`:**
    - Domain: Entity `User`, `Wallet`, Value Object `Money`, Enum `WalletType`.
    - Use Cases: `RegisterCommand`, `LoginCommand`, `RefreshTokenCommand`, `CreateWalletCommand`, `TransferFundsCommand`.
    - Concurrency: Áp dụng **Pessimistic Locking (`SELECT ... FOR UPDATE`)** chống Lost Update khi chuyển ví.
    - CSDL: EF Core Migration cho bảng `users`, `wallets`, `internal_transfers`.
    - Realtime: Cấu hình `FinanceHub` (SignalR) bắn sự kiện `ReceiveBalanceUpdated`.
    - Unit Tests: `MoneyTests` (cộng trừ tiền bất biến), `TransferFundsConcurrencyTests`.
  * **`frontend/`:**
    - Hạ tầng: Cấu hình `LocalDatabaseService` (SQLite nhúng + SQLCipher AES-256).
    - Design System: Nạp `Colors.xaml`, `Styles.xaml` chuẩn Impeccable Double-Bezel và font Plus Jakarta Sans.
    - Màn hình: `LoginPage`, `RegisterPage`, `WalletsOverviewPage` (Thẻ ví tiền mặt, ngân hàng, thẻ tín dụng), `TransferFundsModal`.
    - Auth & Security: Refit Client tích hợp `DelegatingHandler` tự động làm mới JWT qua `SecureStorage` (KeyStore/Keychain).
- **Tích hợp & Kiểm thử:** Chuyển tiền giữa 2 ví trên app $\rightarrow$ Backend khóa bi quan trừ ví A cộng ví B $\rightarrow$ SignalR phát sóng $\rightarrow$ Mobile UI cập nhật số dư tức thì 0ms. Tắt mạng ghi offline vào SQLite $\rightarrow$ bật mạng tự đẩy sync.
- **Quality Gate 1:** 100% Unit Tests Passed, Chuyển tiền không bao giờ bị âm ví hay mất tiền, Offline-first hoạt động trơn tru.

---

### GIAI ĐOẠN 2: PHÂN HỆ 2, 3, 4 — DANH MỤC, SỔ CÁI & BÓC TÁCH HÓA ĐƠN AI OCR
- **Use Cases bao phủ (26 Use Cases):**
  * `UC-04.01` đến `UC-04.07` (Cây danh mục đệ quy Cha-Con, Ánh xạ 50/30/20 & Kakeibo, Smart Keyword Rules).
  * `UC-02.01` đến `UC-02.08` (Thu nhập phát sinh, thu định kỳ, bóc tách phiếu lương AI OCR).
  * `UC-03.01` đến `UC-03.11` (Chi tiêu thủ công, xem timeline, bóc tách hóa đơn AI OCR, Hậu kiểm toán số học, Giọng nói, Bẫy đăng ký, VietQR).
- **Phạm vi thực hiện:**
  * **`backend/`:**
    - Domain: `Category` (Cây phân cấp), `Transaction` (`IncomeTransaction`, `ExpenseTransaction`), `Receipt`, `ReceiptItem`.
    - AI Pipeline: `IReceiptOcrService` (Mock + Gemini Vision) + Thuật toán **Hậu kiểm toán số học (`verifyMathematicalAudit`)** đối soát: $\sum(\text{SL} \times \text{ĐG}) + \text{VAT} = \text{Tổng tiền}$.
    - CSDL & Seed Data: Migration bảng `categories`, `transactions`, `receipts`, `receipt_items`. Nạp bộ danh mục chuẩn.
    - Unit Tests: `MathematicalAuditEngineTests` (kiểm tra sai số làm tròn, thuế VAT), `CategoryHierarchyTests`.
  * **`frontend/`:**
    - UI Components: `TransactionItemCard` (Reusable `ContentView`), `CategoryTreePickerView`.
    - Màn hình: `TransactionTimelinePage` (Sổ cái timeline), `CreateTransactionPage` (Bàn phím tài chính), `ReceiptScannerPage` (Chụp ảnh qua `MediaPicker`).
    - Review Modal: Màn hình kiểm duyệt hóa đơn hiển thị chi tiết từng món hàng kèm tick xanh kiểm toán số học.
- **Tích hợp & Kiểm thử:** Chụp ảnh hóa đơn $\rightarrow$ AI bóc tách từng món $\rightarrow$ Hậu kiểm toán số học cân bằng $\rightarrow$ Bấm lưu $\rightarrow$ Trừ tiền ví và ghi sổ cái.
- **Quality Gate 2:** Sổ cái ghi nhận chính xác từng đồng, bóc tách hóa đơn AI nhận diện đúng tên món và đơn giá.

---

### GIAI ĐOẠN 3: PHÂN HỆ 5 — HOẠCH ĐỊNH NGÂN SÁCH ĐA PHƯƠNG PHÁP & ĐỘNG CƠ DSA
- **Use Cases bao phủ (6 Use Cases):** `UC-05.01` đến `UC-05.06` (Tạo ngân sách, phân bổ danh mục, theo dõi ngân sách, Hạn mức ngày DSA thích ứng, Tốc độ tiêu tiền Spending Velocity, Cảnh báo thâm hụt).
- **Phạm vi thực hiện:**
  * **`backend/`:**
    - Domain: `Budget` (Aggregate Root), `BudgetAllocation`, `DailySpendingAllowance` (DSA Log), `Subscription`.
    - Động cơ toán học: `CalculateDsa()` (chia đều số tiền còn lại cho số ngày còn lại, nhân hệ số cuối tuần), `ComputeSpendingVelocity()` (đo tốc độ tiêu tiền 7 ngày), `DetectSubscriptionTraps()` (quét bẫy gia hạn định kỳ).
    - SignalR: Bắn `ReceiveDsaAlert` khi Spending Velocity vượt ngưỡng an toàn.
    - Unit Tests: `DsaAdaptiveAdjustmentTests`, `SpendingVelocityTests`.
  * **`frontend/`:**
    - Dashboard Card: Thẻ **Hạn Mức Hôm Nay (Adaptive DSA)** lớn ở trung tâm trang chủ, kèm đồng hồ đo tốc độ tiêu tiền.
    - Màn hình: `BudgetSetupPage` (Cấu hình ngân sách theo 50/30/20 hoặc Kakeibo), `SubscriptionsManagerPage` (Quản lý cam kết định kỳ Netflix, Spotify, cước 4G).
    - Thông báo di động: Tích hợp `Plugin.LocalNotification` bắn thông báo đẩy mỗi buổi sáng nhắc hạn mức chi tiêu hôm nay.
- **Tích hợp & Kiểm thử:** Chi tiêu lố hôm nay $\rightarrow$ DSA tự động co lại êm ái cho các ngày còn lại trong tháng mà không làm vỡ ngân sách tổng.
- **Quality Gate 3:** DSA tự động thích ứng 100% toán học, Push Notification hoạt động thật trên thiết bị di động.

---

### GIAI ĐOẠN 4: PHÂN HỆ 6 — QUỸ KHẨN CẤP & TỐI ƯU HÓA NỢ CÁ NHÂN
- **Use Cases bao phủ (7 Use Cases):** `UC-06.01` đến `UC-06.07` (Tạo mục tiêu, theo dõi tiến độ, hoàn thành mục tiêu, Khởi tạo danh mục nợ, Quỹ dự phòng khẩn cấp 3-6 tháng, Đối sánh chiến lược trả nợ Debt Snowball vs Debt Avalanche, Lập lịch thanh toán nợ chi tiết).
- **Phạm vi thực hiện:**
  * **`backend/`:**
    - Domain: `FinancialGoal`, `EmergencyFundGoal` (khóa cứng bảo vệ tối thiểu 3-6 tháng lương), `DebtAccount`, `DebtPaymentPlan`.
    - Solver giải phóng nợ: Lập trình đối kháng 2 thuật toán:
      * `simulateSnowball()`: Sắp xếp theo số dư nợ tăng dần để trả dứt điểm nợ nhỏ trước lấy động lực.
      * `simulateAvalanche()`: Sắp xếp theo lãi suất APR giảm dần để triệt tiêu tiền lãi.
    - Unit Tests: `DebtSolverComparisonTests` (đối soát số tiền lãi tiết kiệm được giữa 2 chiến lược).
  * **`frontend/`:**
    - Màn hình: `SavingsGoalsPage` (Thước đo tiến độ tích lũy Quỹ khẩn cấp), `DebtOptimizerPage` (Thanh trượt tương tác số tiền trả thêm mỗi tháng).
    - Trực quan hóa: Hai đường cong biểu đồ thể hiện lộ trình sạch nợ giữa Snowball và Avalanche.
- **Tích hợp & Kiểm thử:** Nhập danh sách nợ $\rightarrow$ Solver mô phỏng chính xác ngày sạch nợ và số tiền lãi tiết kiệm được giữa 2 phương pháp.
- **Quality Gate 4:** Solver tính toán chuẩn xác từng đồng tiền lãi, quỹ khẩn cấp được bảo vệ nghiêm ngặt.

---

### GIAI ĐOẠN 5: PHÂN HỆ 7, 8 — BÁO CÁO TÀI CHÍNH ĐA CHIỀU & DỰ BÁO RỦI RO
- **Use Cases bao phủ (9 Use Cases):**
  * `UC-07.01` đến `UC-07.05` (Báo cáo Thu Chi, Báo cáo Tài sản ròng Net Worth, Dòng tiền Net Cashflow, Báo cáo cơ cấu danh mục, Xu hướng chi tiêu).
  * `UC-08.01` đến `UC-08.04` (Dự báo chi tiêu cuối kỳ, Dự phóng Ngày cạn tiền Out-of-Money Date, Cảnh báo nguy cơ thâm hụt, Mô phỏng What-If).
- **Phạm vi thực hiện:**
  * **`backend/`:**
    - Analytics Engine: Tính toán Giá trị tài sản ròng (**Net Worth = Assets - Debts**), Cơ cấu dòng tiền đa chiều.
    - Dự báo: Dự phóng dòng tiền 30-90 ngày tới, phát hiện ngày cạn tiền mặt (`Out-of-Money Date`).
    - Export: Xuất báo cáo tài chính cá nhân định dạng PDF / Excel.
  * **`frontend/`:**
    - Đồ họa: Tích hợp `SkiaSharp` vẽ biểu đồ Donut phân tích cơ cấu chi tiêu, biểu đồ cột thu chi 6 tháng và biểu đồ miền dự phóng tương lai.
    - Màn hình: `FinancialReportsPage`, `CashflowForecastPage`.
- **Tích hợp & Kiểm thử:** Biểu đồ hiển thị mượt mà, Net Worth phản ánh đúng tài sản thực.
- **Quality Gate 5:** Biểu đồ vẽ mượt mà 60/120fps, không drop frame khi vuốt lướt.

---

### GIAI ĐOẠN 6: PHÂN HỆ 9 — TÂM LÝ HÀNH VI THALER, COOLING-OFF 24H & VIETQR
- **Use Cases bao phủ (5 Use Cases):** `UC-09.01` đến `UC-09.05` (Nhận khuyến nghị Cú hích, Xem giải trình lý do XAI, Đề xuất tái cân đối, Phê duyệt Human-in-the-loop, Trì hoãn tâm lý Cooling-off 24h-48h).
- **Phạm vi thực hiện:**
  * **`backend/`:**
    - Behavioral Engine: Nhận diện giao dịch mua sắm bốc đồng/xa xỉ, sinh `FinancialNudge` kèm câu giải trình ngôn ngữ tự nhiên (Explainable AI - XAI).
    - Cơ chế Khoảng trễ: Kích hoạt `cooling_off_until = NOW() + 24 HOURS`.
    - VietQR Service: Sinh chuỗi payload và hình ảnh mã QR chuẩn NAPAS 247 / EMVCo.
  * **`frontend/`:**
    - Nudge Dialog: Popup can thiệp tâm lý hiển thị cái giá cơ hội (*"Món đồ này bằng 4 ngày làm việc của bạn"*).
    - Cooling-off Queue: Màn hình danh sách các món đồ đang tạm hoãn suy nghĩ 24 giờ.
    - VietQR Modal: Hiển thị mã QR ngân hàng để quét chuyển khoản thanh toán hóa đơn trong 3 giây.
- **Tích hợp & Kiểm thử:** Nghiệm thu hoàn tất 100% 63 Use Cases!
- **Quality Gate 6:** Toàn bộ 63 Use Cases đều có mã nguồn hiện thực và chạy thông suốt.

---

### GIAI ĐOẠN 7: TỔNG KIỂM TOÁN, ĐÓNG GÓI & NGHIỆM THU KHÓA LUẬN
- **Mục tiêu:** Kiểm toán tổng thể, xuất bản báo cáo và chuẩn bị bộ demo 1-click cho Giảng viên.
- **Nội dung thực hiện:**
  1. **Full-Stack Regression Testing:** Chạy 100% Unit Tests & Integration Tests; kiểm toán hiệu năng bộ nhớ và rò rỉ RAM.
  2. **Bộ kịch bản 1-Click cho Giảng viên:** Tạo script PowerShell tự động khởi chạy Backend, Docker CSDL và App Mobile để Giảng viên chấm điểm trong 1 phút.
  3. **Hoàn thiện Báo cáo Khóa luận Word:** Đồng bộ dữ liệu, ảnh chụp màn hình ứng dụng thật vào [`BAO_CAO_CUOI_KY_DOTNET_MAUI.docx`](BAO_CAO_CUOI_KY_DOTNET_MAUI.docx) đạt chuẩn 100% thể thức DNTU.

---

## 3. BẢNG THEO DÕI TIẾN ĐỘ TỔNG THỂ (PROGRESS TRACKER MATRIX)

| Giai Đoạn | Tên Phân Hệ & Phạm Vi | Use Cases Phụ Trách | Mã Nguồn Backend | Mã Nguồn Frontend | Trạng Thái Kiểm Thử |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Giai đoạn 0** | Khởi tạo Solution Scaffolding & DevOps | Nền tảng hạ tầng | `backend/` | `frontend/` | `CHỜ TRIỂN KHAI` |
| **Giai đoạn 1** | Phân hệ 1: Tài khoản, Đa Ví & Sync | `UC-01.01` $\rightarrow$ `UC-01.10` | `backend/` | `frontend/` | `CHỜ TRIỂN KHAI` |
| **Giai đoạn 2** | Phân hệ 2, 3, 4: Sổ cái & AI OCR | `UC-02`, `UC-03`, `UC-04` (26 UCs) | `backend/` | `frontend/` | `CHỜ TRIỂN KHAI` |
| **Giai đoạn 3** | Phân hệ 5: Ngân sách & Động cơ DSA | `UC-05.01` $\rightarrow$ `UC-05.06` | `backend/` | `frontend/` | `CHỜ TRIỂN KHAI` |
| **Giai đoạn 4** | Phân hệ 6: Quỹ khẩn cấp & Tối ưu nợ | `UC-06.01` $\rightarrow$ `UC-06.07` | `backend/` | `frontend/` | `CHỜ TRIỂN KHAI` |
| **Giai đoạn 5** | Phân hệ 7, 8: Báo cáo & Dự báo thâm hụt| `UC-07`, `UC-08` (9 UCs) | `backend/` | `frontend/` | `CHỜ TRIỂN KHAI` |
| **Giai đoạn 6** | Phân hệ 9: Thaler Nudges & VietQR | `UC-09.01` $\rightarrow$ `UC-09.05` | `backend/` | `frontend/` | `CHỜ TRIỂN KHAI` |
| **Giai đoạn 7** | Tổng kiểm toán, Đóng gói & Báo cáo Word | Toàn bộ 63 Use Cases | `backend/` | `frontend/` | `CHỜ TRIỂN KHAI` |
