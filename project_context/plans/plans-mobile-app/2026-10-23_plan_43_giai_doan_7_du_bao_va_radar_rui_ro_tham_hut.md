# KẾ HOẠCH PLAN-43: TRIỂN KHAI GIAI ĐOẠN 7 — PHÂN HỆ 8: DỰ BÁO TÀI CHÍNH & RADAR RỦI RO THÂM HỤT (UC-08.01 $\rightarrow$ UC-08.04)

> **Dự án:** Hệ thống Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh (Tích hợp AI)  
> **Giai đoạn:** Giai đoạn 7 / 12 Giai đoạn chuẩn hóa (Lộ trình [`PLAN-42`](2026-10-23_plan_42_dieu_chinh_roadmap_tach_phan_he_ai_va_giai_doan_kiem_dinh_ai.md) & [Quyết định 122](../../decisions_and_changes_log.md))  
> **Căn cứ nghiệp vụ:** [`08_dac_ta_uc_du_bao_va_rui_ro.md`](../../reports/use_cases/08_dac_ta_uc_du_bao_va_rui_ro.md), Tab 8 bản vẽ `files/UCD-DuoNoPro.drawio`  
> **Chuyên gia bảo trợ:** Agent 08 Andrew Ng (AI Lead), Agent 03 Cockburn BA, Agent 04 Eric Evans (DDD), Agent 10 Dan Abramov (MVVM Clean Craft), Agent 12 Kent Beck (TDD Quality Gate)  
> **Phương pháp triển khai:** Tách đôi thành 2 chặng: **Chặng 7A (Backend TDD First)** và **Chặng 7B (Frontend Wireframe MAUI & MVVM Repositories)**

---

## 1. MỤC TIÊU & PHẠM VI 4 USE CASES CHUẨN IEEE 29148

| Mã UC | Tên Use Case | Bản Chất Thuật Toán & Ràng Buộc Nghiệp Vụ | Dữ Liệu Đầu Vào & Đầu Ra |
| :--- | :--- | :--- | :--- |
| **`UC-08.01`** | **Dự báo tổng chi tiêu cuối chu kỳ** | • Phân tích chuỗi thời gian bóc tách vận tốc tiêu ngày thường $V_{\text{weekday}}$ và cuối tuần $V_{\text{weekend}}$.<br>• Ước tính chi tiêu tương lai: $\widehat{\text{FutureSpent}} = (N_{\text{weekday}} \times V_{\text{weekday}}) + (N_{\text{weekend}} \times V_{\text{weekend}})$.<br>• Tổng chi dự phóng: $\widehat{\text{TotalExpense}} = \text{CurrentSpent}_t + \widehat{\text{FutureSpent}}$.<br>• Ngoại lệ $t < 3$ ngày: Hiển thị nhãn đang tích lũy dữ liệu.<br>• Ngoại lệ Z-Score Outlier: Tự động loại trừ khoản chi đột biến khỏi vận tốc tiêu để tránh méo mó xu hướng. | **Input:** Chu kỳ ngân sách active, lịch sử chi tiêu từng ngày.<br>**Output:** `EstimatedTotalSpent`, mức bội chi $\Delta_{\text{overspend}}$, nhãn rủi ro (An toàn / Nguy cơ). |
| **`UC-08.02`** | **Dự phóng Ngày cạn tiền khả dụng** | • Đo lường thanh khoản khả dụng $\text{LiquidFunds} = \sum \text{Cash} + \text{Bank}$ (loại trừ quỹ bị khóa).<br>• Vận tốc tiêu 7 ngày gần nhất $\overline{V}_{7\text{days}}$.<br>• Số ngày duy trì: $\text{DaysUntilDepletion} = \text{LiquidFunds} / \overline{V}_{7\text{days}}$.<br>• Ngày cạn tiền: $\text{OutOfMoneyDate} = \text{Today} + \text{DaysUntilDepletion}$.<br>• Đối soát $\text{OutOfMoneyDate}$ với $\text{NextSalaryDate}$:<br>&nbsp;&nbsp;- Nếu cạn trước ngày lương $\rightarrow$ **Báo động Đỏ Khủng hoảng Thanh khoản** (thiếu $\text{DeficitDays}$ ngày).<br>&nbsp;&nbsp;- Nếu cạn sau ngày lương $\rightarrow$ An toàn thanh khoản.<br>• Phòng vệ chia cho 0 an toàn khi $\overline{V} = 0$. | **Input:** Số dư các ví, ngày nhận lương `salaryDay`, vận tốc tiêu 7 ngày.<br>**Output:** `OutOfMoneyDate`, số ngày thiếu hụt `DeficitDays`, cờ `IsLiquidityCrisis`. |
| **`UC-08.03`** | **Dự báo nguy cơ thâm hụt danh mục thiết yếu** | • Quét toàn bộ dòng phân bổ ngân sách `BudgetAllocations`.<br>• Tốc độ tiêu từng danh mục $V_i = \text{Spent}_{i, t} / t$.<br>• Chi tiêu dự phóng: $\widehat{\text{Spent}}_{i, \text{end}} = \text{Spent}_{i, t} + (N - t) \times V_i$.<br>• Tỷ lệ vượt hạn mức: $\text{RiskRatio}_i = (\widehat{\text{Spent}}_{i, \text{end}} / \text{Allocated}_i) \times 100\%$.<br>• Ngày cạn sạch hạn mức danh mục: $\text{ExhaustionDate}_i = \text{Today} + (\text{Allocated}_i - \text{Spent}_i) / V_i$.<br>• Ưu tiên nhóm Sinh tồn (Needs/Survival) có $\text{RiskRatio} > 115\%$ lên hàng đầu. | **Input:** Phân bổ ngân sách danh mục, chi tiêu thực tế.<br>**Output:** Danh sách xếp hạng danh mục rủi ro cao, ngày dự kiến vỡ hạn mức. |
| **`UC-08.04`** | **Mô phỏng kịch bản thu chi giả định (What-If)** | • Phòng thí nghiệm tài chính Sandbox cô lập hoàn toàn trên In-Memory, tuyệt đối không chạm CSDL thật.<br>• Hỗ trợ 3 kịch bản:<br>&nbsp;&nbsp;1. Mua sắm lớn trả một lần ($X_{\text{whatif}}$).<br>&nbsp;&nbsp;2. Vay mua trả góp định kỳ ($P_{\text{monthly}}$, $N_{\text{months}}$).<br>&nbsp;&nbsp;3. Biến động thu nhập (giảm $10\% - 30\%$).<br>• Tái tính toán tức thời Before vs After:<br>&nbsp;&nbsp;- Hạn mức ngày mới $\text{DSA}'$.<br>&nbsp;&nbsp;- Ngày cạn tiền mới $\text{OutOfMoneyDate}'$.<br>&nbsp;&nbsp;- Tài sản ròng mới $\text{NetWorth}'$.<br>&nbsp;&nbsp;- Tỷ số nợ trên thu nhập mới $\text{DTI}'$. | **Input:** Loại kịch bản, tham số giả định.<br>**Output:** Bảng đối chiếu Trước / Sau, thông điệp đánh giá định lượng tác động. |

---

## 2. KIẾN TRÚC KỸ THUẬT & DANH MỤC THÀNH PHẦN XÂY DỰNG

```
mobile-project/dotnet-maui-ver/
├── backend/
│   ├── ExpenseTracker.Application/
│   │   ├── DTOs/Forecasting/
│   │   │   └── ForecastingDtos.cs                  # DTOs kết quả dự báo, radar rủi ro, what-if
│   │   ├── Interfaces/
│   │   │   └── IForecastingService.cs              # Hợp đồng nghiệp vụ dự báo & what-if
│   │   └── Services/
│   │       └── ForecastingService.cs               # Triển khai thuật toán chuỗi thời gian, suy kiệt thanh khoản
│   ├── ExpenseTracker.WebApi/
│   │   └── Controllers/
│   │       └── ForecastingController.cs            # API endpoints: expenditure, liquidity-depletion, category-risks, what-if
│   └── tests/
│       └── ExpenseTracker.Application.UnitTests/
│           └── Services/
│               └── ForecastingServiceTests.cs      # Suite 8+ TDD Unit Tests kiểm toán toán học
│
└── frontend/
    ├── ExpenseTracker.Maui/
    │   ├── Models/Forecasting/
    │   │   └── ForecastingModels.cs                # Client DTO models
    │   ├── Services/Api/
    │   │   └── IApiClients.cs                      # Refit client contract: IForecastingApi
    │   ├── Repositories/Forecasting/
    │   │   ├── IForecastingRepository.cs           # Tầng trừu tượng MVVM Repository
    │   │   ├── ApiForecastingRepository.cs         # Kết nối WebApi thật
    │   │   └── MockForecastingRepository.cs        # Dữ liệu giả lập chạy offline phục vụ UI Designer
    │   ├── ViewModels/Forecasting/
    │   │   └── ForecastingViewModel.cs             # Reactive ViewModel điều phối trạng thái, slider What-If
    │   └── Views/
    │       ├── ForecastingPage.xaml                # Wireframe View 4 phân vùng thẻ chức năng
    │       └── ForecastingPage.xaml.cs             # Code-behind định tuyến và gắn BindingContext
    └── tests/
        └── ExpenseTracker.Maui.UnitTests/
            └── ViewModels/
                └── ForecastingViewModelTests.cs    # Suite 4+ Unit Tests cho ViewModel
```

---

## 3. DANH MỤC TDD UNIT TESTS DỰ KIẾN (NÂNG TỪ 189 LÊN $\ge 205$ TESTS)

| STT | Tập Tin Test | Tên Test Case | Kịch Bản & Khẳng Định Kiểm Thử |
| :--- | :--- | :--- | :--- |
| 1 | `ForecastingServiceTests.cs` | `GetExpenditureForecast_WithWeekdayWeekendVariance_ShouldForecastAccurately` | Xác minh công thức $\widehat{\text{FutureSpent}} = N_{\text{wd}} V_{\text{wd}} + N_{\text{we}} V_{\text{we}}$. |
| 2 | `ForecastingServiceTests.cs` | `GetExpenditureForecast_WhenCycleUnder3Days_ShouldReturnInsufficientDataState` | Xử lý an toàn khi chu kỳ $< 3$ ngày. |
| 3 | `ForecastingServiceTests.cs` | `GetExpenditureForecast_ShouldExcludeOutliersFromVelocityCalculation` | Tự động loại trừ giao dịch đột biến Outlier khỏi việc tính vận tốc ngày thường/cuối tuần. |
| 4 | `ForecastingServiceTests.cs` | `GetLiquidityDepletion_WhenRunOutOfMoneyBeforeSalary_ShouldFlagCrisisAndDeficitDays` | Bắt đúng cờ khủng hoảng thanh khoản và số ngày thiếu hụt tiền trước kỳ lương. |
| 5 | `ForecastingServiceTests.cs` | `GetLiquidityDepletion_WhenVelocityZero_ShouldHandleZeroDivisionGracefully` | Phòng vệ chia cho 0 khi không có chi tiêu trong 7 ngày gần nhất. |
| 6 | `ForecastingServiceTests.cs` | `GetCategoryDeficitRisks_WithSpikeCategory_ShouldCalculateExhaustionDateAndRankHigh` | Bắt đúng danh mục thiết yếu có nguy cơ vỡ ngân sách và tính đúng ngày cạn hạn mức. |
| 7 | `ForecastingServiceTests.cs` | `SimulateWhatIf_LumpSumExpense_ShouldReduceDsaAndAdvanceDepletionDateWithoutDbMutation` | Mô phỏng mua sắm lớn làm siết giảm DSA, kéo sớm ngày cạn tiền và không thay đổi DB thật. |
| 8 | `ForecastingServiceTests.cs` | `SimulateWhatIf_InstallmentLoan_ShouldRecalculateDtiRatioAccurately` | Tái tính toán tỷ số DTI khi có khoản vay trả góp định kỳ. |
| 9 | `ForecastingViewModelTests.cs` | `LoadForecastData_WhenSuccessful_ShouldPopulateCardsAndChartData` | ViewModel nạp dữ liệu đầy đủ từ Repository. |
| 10 | `ForecastingViewModelTests.cs` | `RunWhatIfSimulation_WhenTriggered_ShouldUpdateComparisonResults` | Kích hoạt kịch bản mô phỏng cập nhật bảng Trước/Sau. |
| 11 | `ForecastingViewModelTests.cs` | `SliderValueChanged_ShouldDebounceAndRecalculateInstantly` | Kéo thanh trượt điều chỉnh số tiền mô phỏng phản hồi tức thì. |
| 12 | `ForecastingViewModelTests.cs` | `HandleRepositoryFailure_ShouldSetErrorMessageGracefully` | Bắt ngoại lệ và hiển thị thông báo lỗi thân thiện. |

---

## 4. TIÊU CHÍ NGHIỆM THU (QUALITY GATES)
1. **Chặng 7A (Backend):**
   - 100% biên dịch sạch, 0 Error, 0 Warning trên `ExpenseTracker.slnx`.
   - Bổ sung 8 Unit Tests backend, nâng tổng số test từ 189 lên $\ge 197$ tests passed.
2. **Chặng 7B (Frontend Wireframe):**
   - 100% biên dịch sạch, 0 Error, 0 Warning trên `ExpenseTracker.Maui.csproj`.
   - Trang bị bộ đôi `ApiForecastingRepository` và `MockForecastingRepository` với dữ liệu phong phú phục vụ UI designer.
   - Bổ sung 4 Unit Tests mobile, nâng tổng số test lên $\ge 201$ tests passed (100% Pass rate).
3. **Kiểm toán hậu giai đoạn:**
   - Tổ chức Phiên Kiểm Toán Hệ Thống Hậu Giai Đoạn 7 (Post-Phase 7 Audit Protocol) theo Quyết định 116 đối soát 4 Use Cases IEEE 29148.
