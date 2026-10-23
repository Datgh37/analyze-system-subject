# KẾ HOẠCH TRIỂN KHAI CHI TIẾT (PLAN-40)
## GIAI ĐOẠN 6: PHÂN HỆ 7 — BÁO CÁO & PHÂN TÍCH TÀI CHÍNH ĐA CHIỀU (UC-07.01 $\rightarrow$ UC-07.05)

---

## 1. BỐI CẢNH & MỤC TIÊU CHIẾN LƯỢC

- **Tên phân hệ:** Phân hệ 7 — Báo cáo & Phân tích Tài chính Đa chiều (Financial Reporting & Analytics Engine).
- **Vị trí trong lộ trình:** Kế thừa trọn vẹn kết quả của 5 giai đoạn nền tảng:
  - Giai đoạn 1: Xác thực & Quản lý Đa Ví chuẩn ACID (`UC-01`, `UC-02`).
  - Giai đoạn 2: Danh mục & Quy tắc Tự động phân loại (`UC-04`).
  - Giai đoạn 3: Thu nhập định kỳ, Chi tiêu hóa đơn OCR, Tiện ích tài chính (`UC-03`).
  - Giai đoạn 4: Hoạch định Ngân sách đa phương pháp & Động cơ DSA thích ứng (`UC-05`).
  - Giai đoạn 5: Mục tiêu Tích lũy thông minh & Hũ tài chính Kakeibo (`UC-06`).
  - Giai đoạn 5B (PLAN-41): Chuẩn hóa MVVM 3 Tầng Repositories (Phương án B3 môn Mobile), Realtime SignalR Hub và Push Notifications.
- **Mục tiêu Giai đoạn 6:** Xây dựng một **Động Cơ Phân Tích Tài Chính Đa Chiều (Analytics & Intelligence Engine)** có chiều sâu học thuật và giá trị thực tiễn cao, đáp ứng toàn diện 5 Use Cases chuẩn IEEE 29148 và hỗ trợ người bạn cùng làm đồ án MAUI với kiến trúc Repositories (Real/Mock) độc lập.

---

## 2. DANH SÁCH 5 USE CASES CHI TIẾT (CHUẨN IEEE 29148)

| Mã Use Case | Tên Nghiệp Vụ Cốt Lõi | Mô Tả Nghiệp Vụ & Thuật Toán Áp Dụng |
| :--- | :--- | :--- |
| **`UC-07.01`** | **Báo cáo Thu - Chi theo thời gian** | So sánh biến động giữa Tổng Thu nhập vs Tổng Chi tiêu theo Tuần, Tháng, Quý, Năm. Tính mức thặng dư/thâm hụt $\Delta = \text{Thu} - \text{Chi}$ và tốc độ tăng trưởng so với kỳ trước: $\text{MoM} = \frac{\text{Cur} - \text{Prev}}{\text{Prev}} \times 100\%$, $\text{YoY} = \frac{\text{Cur} - \text{SamePeriodLastYear}}{\text{SamePeriodLastYear}} \times 100\%$. Bảo vệ chia cho 0 khi $\text{Prev} = 0$. |
| **`UC-07.02`** | **Báo cáo Giá trị Tài sản ròng (Net Worth)** | Thước đo sức khỏe tài chính tối thượng: $\text{NetWorth} = \text{TotalAssets} - \text{TotalLiabilities}$ (Tổng số dư ví + Quỹ tích lũy - Tổng dư nợ thẻ tín dụng). Lưu snapshot lịch sử để vẽ đường biến thiên tài sản ròng qua thời gian. Gắn cờ cảnh báo rủi ro khi Net Worth âm. |
| **`UC-07.03`** | **Báo cáo Dòng tiền ròng (Net Cashflow)** | Đo lường năng lực tích lũy: $\text{NetCashflow} = \text{Income} - \text{Expense}$, Tỷ lệ tiết kiệm $\text{SavingsRate} = \frac{\text{NetCashflow}}{\text{TotalIncome}} \times 100\%$. Xếp hạng 4 cấp độ sức khỏe dòng tiền (Xuất sắc $\ge 20\%$, Khá 10-20%, Bấp bênh 0-10%, Nguy hiểm $< 0\%$). Mô hình phân rã thác nước (Waterfall Flow). |
| **`UC-07.04`** | **Phân tích cơ cấu danh mục chi tiêu** | Phân tích tỷ trọng phần trăm theo 3 góc nhìn học thuật linh hoạt: 1. Danh mục con cấp thấp; 2. Nhóm Quy tắc 50/30/20 (Needs, Wants, Savings/Debt); 3. Sổ Kakeibo Nhật Bản 4 Cột trụ (Thiết yếu, Tùy chọn, Văn hóa, Bất thường). Phát cờ cảnh báo mất cân đối khi một danh mục chiếm $> 30\%$ tổng chi. |
| **`UC-07.05`** | **Phân tích xu hướng & Phát hiện đột biến (Z-Score Outlier)** | Ứng dụng kiểm định thống kê định lượng tài chính: Tính kỳ vọng trung bình $\mu_{\text{cat}}$ và độ lệch chuẩn mẫu $\sigma_{\text{cat}}$ của từng danh mục. Quét các giao dịch hiện tại, tính điểm chuẩn hóa: $\text{Z-Score} = \frac{X - \mu_{\text{cat}}}{\sigma_{\text{cat}}}$. Gắn cờ cảnh báo đỏ với các khoản chi đột biến bất thường ($\text{Z-Score} > 2.0$, vượt ngoài khoảng tin cậy 95%). Cảnh báo danh mục tăng vọt $> 50\%$ so với mức trung bình. Xử lý an toàn khi $\sigma = 0$. |

---

## 3. THIẾT KẾ KỸ THUẬT CHI TIẾT (TECHNICAL SPECIFICATIONS)

### 3.1. Domain & Persistence: `NetWorthSnapshot`
- **Tập tin:** `ExpenseTracker.Domain/Entities/NetWorthSnapshot.cs`
- **Thuộc tính:**
  - `Guid Id`, `Guid UserId`, `User User`
  - `DateTime SnapshotDate` (chỉ lưu Date phần UTC)
  - `decimal TotalAssets` ($\ge 0$, tổng số dư các ví tài sản + mục tiêu tích lũy)
  - `decimal TotalLiabilities` ($\ge 0$, tổng dư nợ các thẻ tín dụng / vay)
  - `decimal NetWorth` ($= \text{TotalAssets} - \text{TotalLiabilities}$)
  - `string? Notes`
  - Kế thừa `BaseEntity` (`CreatedAt`, `UpdatedAt`, `IsDeleted`)
- **Configuration & Indexing:** `NetWorthSnapshotConfiguration.cs`
  - Tên bảng: `NetWorthSnapshots`
  - Composite Index: `(UserId, SnapshotDate)` để truy vấn dải thời gian nhanh và hỗ trợ upsert snapshot trong ngày.
  - Precision: `decimal(18,2)`.

### 3.2. Application Layer: DTOs & Service Contracts
- **Tập tin DTOs:** `ExpenseTracker.Application/DTOs/Reports/AnalyticsReportDtos.cs`
  - `IncomeExpenseComparisonDto`: `TotalIncome`, `TotalExpense`, `NetSurplus`, `GrowthMoM`, `GrowthYoY`, `Breakdown` (`List<PeriodSummaryDto>`).
  - `NetWorthReportDto`: `CurrentNetWorth`, `TotalAssets`, `TotalLiabilities`, `Snapshots` (`List<NetWorthSnapshotDto>`), `IsNegativeNetWorthWarning`.
  - `NetCashflowReportDto`: `TotalIncome`, `TotalExpense`, `NetCashflow`, `SavingsRate`, `HealthStatus` (`CashflowHealthStatus`: Excellent, Good, Fragile, Dangerous), `WaterfallSteps` (`List<WaterfallStepDto>`).
  - `ExpenseStructureReportDto`: `TotalExpense`, `BySubCategory` (`List<CategoryShareDto>`), `By503020` (`Expense503020SummaryDto`), `ByKakeibo` (`ExpenseKakeiboSummaryDto`), `DominantCategoryAlerts` (`List<string>`).
  - `OutlierDetectionReportDto`: `AnalyzedTransactionCount`, `Outliers` (`List<OutlierTransactionDto>`), `CategorySurgeAlerts` (`List<CategorySurgeAlertDto>`).
- **Interface & Service:**
  - `IAnalyticsReportService.cs`:
    - `Task<IncomeExpenseComparisonDto> GetIncomeExpenseComparisonAsync(Guid userId, DateTime from, DateTime to, string groupBy, CancellationToken ct);`
    - `Task<NetWorthReportDto> GetNetWorthReportAsync(Guid userId, DateTime? from, DateTime? to, CancellationToken ct);`
    - `Task<NetWorthSnapshotDto> RecordNetWorthSnapshotAsync(Guid userId, string? notes, CancellationToken ct);`
    - `Task<NetCashflowReportDto> GetNetCashflowReportAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct);`
    - `Task<ExpenseStructureReportDto> GetExpenseStructureReportAsync(Guid userId, DateTime from, DateTime to, CancellationToken ct);`
    - `Task<OutlierDetectionReportDto> DetectSpendingOutliersAsync(Guid userId, int days, double threshold, CancellationToken ct);`
  - `AnalyticsReportService.cs`: Hiện thực thuật toán và cơ chế bảo vệ toán học (chia cho 0, $\sigma = 0$).

### 3.3. WebApi Endpoints (`AnalyticsReportsController.cs`)
- Route: `api/analytics/reports` (Yêu cầu `[Authorize]`)
  - `GET income-expense?from=...&to=...&groupBy=month`
  - `GET net-worth?from=...&to=...`
  - `POST net-worth/snapshot` (Chụp snapshot hiện thời)
  - `GET cashflow?from=...&to=...`
  - `GET expense-structure?from=...&to=...`
  - `GET outliers?days=30&threshold=2.0`

### 3.4. Frontend .NET MAUI Repositories Pattern (Phương án B3)
- **Tập tin:**
  - Models: `ExpenseTracker.Maui/Models/Reports/AnalyticsReportModels.cs`
  - Refit Client: `IAnalyticsReportApi` trong `IApiClients.cs`
  - Interface: `IAnalyticsReportRepository.cs` trong `Repositories/Interfaces/`
  - Implementation thật: `ApiAnalyticsReportRepository.cs` trong `Repositories/Implementations/`
  - Mock Implementation: `MockAnalyticsReportRepository.cs` trong `Repositories/Implementations/` (cung cấp Mock Data sống động cho UI designer/bạn cùng nhóm làm việc offline độc lập)
  - ViewModel: `ReportsViewModel.cs` trong `ViewModels/Reports/` kế thừa `BaseViewModel`
  - View: `ReportsPage.xaml` & `ReportsPage.xaml.cs` trong `Views/Reports/`

---

## 4. PHÂN KỲ THI CÔNG KỸ THUẬT (2 CHẶNG)

```text
┌─────────────────────────────────────────────────────────────────────────────────────────┐
│ CHẶNG 6A: BACKEND DOMAIN, PERSISTENCE, CQRS ANALYTICS ENGINE & UNIT TESTS               │
├─────────────────────────────────────────────────────────────────────────────────────────┤
│ 1. Domain & Persistence:                                                                │
│    • Entity NetWorthSnapshot.cs (Snapshot lịch sử tài sản ròng).                        │
│    • NetWorthSnapshotConfiguration.cs (EF Core mapping & indexing).                     │
│    • Cập nhật IApplicationDbContext.cs & ApplicationDbContext.cs.                       │
│    • Migration EF Core: AddNetWorthSnapshots.                                           │
│ 2. Application Layer (CQRS Read Projections & Analytics Engine):                        │
│    • DTOs: AnalyticsReportDtos.cs.                                                      │
│    • Interface: IAnalyticsReportService.cs.                                             │
│    • Service: AnalyticsReportService.cs (Thuật toán Z-Score, Mean, StdDev,              │
│      Waterfall decomposition, MoM/YoY growth rate, bảo vệ chia cho 0).                  │
│    • Đăng ký DI trong DependencyInjection.cs.                                           │
│ 3. Presentation Layer:                                                                  │
│    • Controller: AnalyticsReportsController.cs (RESTful endpoints chuẩn Open API).      │
│ 4. Kent Beck TDD Unit Tests:                                                            │
│    • NetWorthSnapshotTests.cs (Domain invariants: calculate, invariants).               │
│    • AnalyticsReportServiceTests.cs (Application tests: Z-Score toán học,              │
│      MoM/YoY, Net Worth, Waterfall flow, bảo vệ chia cho 0 khi income = 0 / prev = 0).  │
│    • Mục tiêu: Nâng tổng số tests từ 173 lên >= 189 tests (100% Passed).                │
├─────────────────────────────────────────────────────────────────────────────────────────┤
│ CHẶNG 6B: FRONTEND .NET MAUI HARNESS (MVVM REPOSITORY PATTERN & WIREFRAME CONNECTIVITY) │
├─────────────────────────────────────────────────────────────────────────────────────────┤
│ 1. Models: AnalyticsReportModels.cs trong Models/Reports/.                              │
│ 2. Refit Client API: IAnalyticsReportApi trong Services/Api/IApiClients.cs.             │
│ 3. MVVM Repositories (Tuân thủ chuẩn Phương án B3 môn Mobile):                         │
│    • Interface: IAnalyticsReportRepository.cs                                           │
│    • Implementation thật: ApiAnalyticsReportRepository.cs                              │
│    • Mock Testing: MockAnalyticsReportRepository.cs                                     │
│ 4. MVVM ViewModels & Testing:                                                           │
│    • ReportsViewModel.cs (sử dụng IAnalyticsReportRepository, độc lập HTTP client).     │
│    • Bổ sung Unit Tests cho ReportsViewModel trong ExpenseTracker.Maui.UnitTests.       │
│ 5. Views: ReportsPage.xaml & ReportsPage.xaml.cs (Khung giao diện kết nối các tab).     │
│ 6. Đăng ký DI trong MauiProgram.cs và route trong AppShell.xaml.                        │
│ 7. Biên dịch kiểm thử MAUI: 0 Warning, 0 Error.                                         │
│ 8. Phiên Kiểm Toán Hệ Thống Hậu Giai Đoạn 6: Đối soát 5 Use Cases, ghi Quyết định 121.  │
└─────────────────────────────────────────────────────────────────────────────────────────┘
```

---

## 5. DANH MỤC 16+ UNIT TESTS MỚI (TDD VERIFICATION MATRIX)

| STT | Tập Tin Kiểm Thử | Tên Test Case | Mục Tiêu & Kịch Bản Kiểm Thử |
| :--- | :--- | :--- | :--- |
| 1 | `NetWorthSnapshotTests.cs` | `Constructor_WithValidData_ShouldCalculateNetWorthAccurately` | Xác minh $\text{NetWorth} = \text{TotalAssets} - \text{TotalLiabilities}$. |
| 2 | `NetWorthSnapshotTests.cs` | `Constructor_WithNegativeAssetsOrLiabilities_ShouldThrowArgumentException` | Bảo vệ invariant không cho phép giá trị âm cho tài sản/nợ thô. |
| 3 | `NetWorthSnapshotTests.cs` | `UpdateNotes_ShouldSetNotesAndModifyUpdatedAt` | Xác minh chức năng ghi chú bổ sung cho snapshot. |
| 4 | `NetWorthSnapshotTests.cs` | `NegativeNetWorth_WhenLiabilitiesExceedAssets_ShouldBeAllowed` | Net Worth được phép âm khi dư nợ vượt tài sản. |
| 5 | `AnalyticsReportServiceTests.cs`| `GetIncomeExpense_WhenPreviousPeriodZero_ShouldReturnSafeGrowthRateWithoutDivisionByZero` | Xử lý MoM/YoY an toàn khi kỳ trước $= 0$. |
| 6 | `AnalyticsReportServiceTests.cs`| `GetIncomeExpense_WithValidData_ShouldCalculateMoMAndYoYAccurately` | Đối soát công thức MoM/YoY toán học chính xác. |
| 7 | `AnalyticsReportServiceTests.cs`| `GetNetWorth_ShouldAggregateWalletsAndSavingsGoalsAccurately` | Tính đúng tổng số dư ví active và quỹ tích lũy. |
| 8 | `AnalyticsReportServiceTests.cs`| `GetNetCashflow_WhenIncomeZero_ShouldHandleZeroDivisionGracefully` | Tính Savings Rate an toàn khi Income $= 0$. |
| 9 | `AnalyticsReportServiceTests.cs`| `GetNetCashflow_ShouldClassifyHealthStatusAccurately` | Kiểm thử 4 ngưỡng xếp hạng sức khỏe dòng tiền. |
| 10 | `AnalyticsReportServiceTests.cs`| `GetExpenseStructure_ShouldCategorize503020AndKakeiboCorrectly` | Phân bổ chính xác tỷ trọng 50/30/20 và Kakeibo 4 cột trụ. |
| 11 | `AnalyticsReportServiceTests.cs`| `DetectSpendingOutliers_WhenVarianceZero_ShouldReturnZeroZScoreWithoutException` | Xử lý ngoại lệ $\sigma = 0$ khi tất cả các giao dịch bằng tiền nhau. |
| 12 | `AnalyticsReportServiceTests.cs`| `DetectSpendingOutliers_WithSpikeTransaction_ShouldFlagOutlierWhenZScoreExceedsThreshold` | Bắt đúng ngoại lệ chi tiêu khi $\text{Z-Score} > 2.0$. |
| 13 | `ReportsViewModelTests.cs` | `LoadReports_WhenSuccessful_ShouldPopulateAllReportData` | ViewModel nạp dữ liệu đầy đủ từ Repository. |
| 14 | `ReportsViewModelTests.cs` | `ChangeReportType_ShouldUpdateActiveTabAndTriggerReload` | Chuyển tab báo cáo cập nhật state chính xác. |
| 15 | `ReportsViewModelTests.cs` | `TakeSnapshot_WhenSuccessful_ShouldUpdateCurrentNetWorth` | Kích hoạt command chụp snapshot thành công. |
| 16 | `ReportsViewModelTests.cs` | `LoadReports_WhenRepositoryFails_ShouldSetErrorMessageAndResetLoading` | Xử lý lỗi ngoại lệ từ repository và hiển thị thông báo. |

---

## 6. TIÊU CHÍ NGHIỆM THU CHẤT LƯỢNG (QUALITY GATES)

1. **Chặng 6A Backend [HOÀN THÀNH]:**
   - Đã tạo Entity `NetWorthSnapshot.cs`, Migration `AddNetWorthSnapshots`.
   - Đã tạo `AnalyticsReportDtos.cs`, `IAnalyticsReportService.cs`, `AnalyticsReportService.cs`, `AnalyticsReportsController.cs`.
   - Đã bổ sung 12 Unit Tests mới (4 Domain + 8 Application).
   - **Kết quả kiểm thử:** Toàn bộ **185 / 185 Unit Tests PASSED 100%** (73 Domain + 98 Application + 14 Mobile).
   - **Biên dịch Backend Solution:** **0 Warning, 0 Error**.
   - **Bảo vệ Toán học:** Đảm bảo 100% không bị chia cho 0 khi `income = 0` hoặc $\sigma = 0$.
2. **Chặng 6B Frontend [HOÀN THÀNH]:**
   - Đã triển khai Client Models `AnalyticsReportModels.cs` và Refit API contract `IAnalyticsReportApi`.
   - Đã xây dựng `IAnalyticsReportRepository` gồm `ApiAnalyticsReportRepository` và `MockAnalyticsReportRepository` (dữ liệu mẫu phong phú chạy offline phục vụ UI designer).
   - Đã tạo `ReportsViewModel.cs` độc lập HTTP client và Wireframe `ReportsPage.xaml` gắn sẵn Data Binding.
   - Bổ sung 4 Unit Tests cho Mobile ViewModel (`ReportsViewModelTests.cs`), nâng tổng số test lên **189 / 189 tests PASSED 100%**.
   - Biên dịch MAUI Project: **0 Warning, 0 Error**.
3. **Phiên Kiểm Toán Hệ Thống Hậu Giai Đoạn 6 [HOÀN THÀNH]:** Đối soát toàn diện 5 Use Cases IEEE 29148 và ghi Quyết định 121 vào `decisions_and_changes_log.md`.
