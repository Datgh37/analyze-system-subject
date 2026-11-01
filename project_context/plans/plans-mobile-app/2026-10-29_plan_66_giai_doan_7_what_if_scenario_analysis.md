# KẾ HOẠCH TRIỂN KHAI GIAI ĐOẠN MỚI 07: MÔ PHỎNG DÒNG TIỀN BIẾN CỐ & THỬ NGHIỆM SỨC CHỊU ĐỰNG TÀI CHÍNH (WHAT-IF SCENARIO ANALYSIS & STRESS TESTING — UC-08.04)

> **Mã kế hoạch:** `PLAN-66`  
> **Thời gian khởi tạo:** 29/10/2026  
> **Trạng thái:** Chờ phê duyệt (Pending Approval)  
> **Áp dụng theo:** Master Project Roadmap (Giai đoạn Mới 07), Đặc tả UC-08.04 (`project_context/reports/use_cases/08_dac_ta_uc_du_bao_va_rui_ro.md`), Sơ đồ UCD Tab 8 & Class Diagram Lab 3.

---

## 1. BỐI CẢNH & MỤC TIÊU NGHIỆP VỤ

### 1.1. Hiện trạng sau Giai đoạn Mới 06
- Hệ thống đã hoàn thành trọn vẹn 6 giai đoạn lớn với **469 / 469 Unit Tests PASSED 100%**:
  - Giai đoạn 01: Chuẩn hóa Soft Delete, Audit Logs, SignalR Notification Hub.
  - Giai đoạn 02: Sinh trắc học Biometrics, SecureStorage & Quick Login.
  - Giai đoạn 03: Phân hệ Quản lý Danh mục Nợ (`Debts`).
  - Giai đoạn 04: Động cơ Trả nợ Tối ưu Snowball vs Avalanche.
  - Giai đoạn 05: Quỹ Khẩn Cấp (`EmergencyFund`) & Tài Sản Ròng (`NetWorth`).
  - Giai đoạn 06: Quét & Cảnh báo Bẫy Đăng ký Định kỳ (`Subscriptions`).
- Tuy nhiên, tại phân hệ Dự báo & Phân tích rủi ro (`ForecastingService` — `UC-08.04`):
  1. **Nghĩa vụ nợ đang tính giả lập 5% hardcoded:** Công thức DTI hiện tại lấy `totalLiabilities * 0.05m` chứ chưa liên kết bảng dữ liệu nợ thật `Debts` (vốn đã có `CurrentBalance`, `MinimumPayment`, `InterestRate`).
  2. **Chưa stress-test Quỹ Khẩn Cấp:** Khi xảy ra biến cố, hệ thống chưa tính toán xem Quỹ dự phòng khẩn cấp (`EmergencyFund`) và số tháng sinh tồn (`RunwayMonths`) còn lại bao nhiêu trước khi cạn tiền.
  3. **Thiếu kịch bản thứ 4 theo đặc tả UC-08.04:** Hiện chỉ có 3 kịch bản (`LumpSumExpense`, `InstallmentLoan`, `IncomeReduction`), chưa có kịch bản **`EmergencyShock`** (Cú sốc khẩn cấp kết hợp sụt giảm thu nhập).
  4. **Chưa có Luồng phụ A1 (`Convert to Savings Goal`):** Khi người dùng mô phỏng một khoản mua sắm lớn, họ chưa có tính năng 1-chạm chuyển kịch bản này thành Mục tiêu tích lũy `SavingsGoal` để hạn chế mua bốc đồng.
  5. **Chưa có cảnh báo ngoại lệ E1:** Chưa chặn/cảnh báo trực quan khi khoản chi lớn vượt quá tổng tiền mặt thanh khoản hiện có trong ví.

### 1.2. Mục tiêu Giai đoạn Mới 07
Xây dựng hoàn thiện **Phòng thí nghiệm Tài chính Giả định (Interactive What-If Simulation Sandbox)**:
1. **Tính cô lập Sandbox tuyệt đối (Sandbox Isolation):** 100% thuật toán chạy In-Memory, tuyệt đối không ghi dữ liệu giả lập vào CSDL.
2. **Tích hợp dữ liệu liên phân hệ:** Lấy dữ liệu thực tế từ `Debts`, `SavingsGoals` (`IsEmergencyFund`), `Wallets`, `Budgets` để thiết lập đường cơ sở (Baseline).
3. **Mô phỏng 4 kịch bản biến cố tài chính trọng điểm:**
   - **Kịch bản 1: Mua sắm lớn trả thẳng (Lump Sum Expense):** Tác động lên DSA, Ngày cạn tiền, Net Worth.
   - **Kịch bản 2: Vay mua trả góp định kỳ (Installment Loan):** Tác động lên nghĩa vụ nợ hàng tháng, đội tỷ số DTI (ngưỡng nguy hiểm > 40%).
   - **Kịch bản 3: Sụt giảm thu nhập (Income Reduction Shock):** Giả định giảm lương 10%-50%, điều chỉnh mức chi tiêu DSA an toàn.
   - **Kịch bản 4: Cú sốc Biến cố khẩn cấp (Emergency Shock):** Đột ngột cần chi một khoản khẩn cấp lớn + giảm thu nhập, stress-test Quỹ Khẩn Cấp còn sống sót được bao nhiêu tháng (`RunwayMonthsAfter`).
4. **Luồng phụ A1 (Convert to Savings Goal):** Endpoint `POST /api/forecasting/what-if/convert-to-goal` cho phép chuyển số tiền giả định thành mục tiêu tiết kiệm.
5. **Ngoại lệ E1:** Phát hiện tức thì `LumpSumAmount > TotalLiquidFunds` và trả về cảnh báo nguy cơ âm ví khẩn cấp.
6. **Mobile MAUI Experience:** Giao diện Sandbox điều khiển thanh trượt mượt mà (0ms cảm nhận), so sánh trực quan Before vs After (DSA, DTI, Runway, NetWorth, OutOfMoneyDate).
7. **Kiểm thử tự động toàn trình:** Viết Unit Tests Backend & Mobile nâng tổng test suite lên **485+ tests PASSED 100%**.

---

## 2. KIẾN TRÚC & THIẾT KẾ CHI TIẾT

```
                                  [ WHAT-IF SIMULATION SANDBOX ]
                                    (100% In-Memory Isolation)
                                                │
         ┌──────────────────────────────┬───────┴──────────────────────────────┐
         ▼                              ▼                                      ▼
[ Bảng Dữ Liệu Nợ Thật ]      [ Quỹ Khẩn Cấp Thật ]                 [ Ví & Ngân Sách Thật ]
  - Debts.CurrentBalance        - SavingsGoals (IsEmergencyFund)      - Wallets.Balance (Liquid)
  - Debts.MinimumPayment        - RunwayMonthsBefore                  - Budgets.TotalRemaining (DSA)
         │                              │                                      │
         └──────────────────────────────┼──────────────────────────────────────┘
                                        │
                                        ▼
                   [ WhatIfSimulationEngine (Pure In-Memory) ]
                   ├── Kịch bản 1: Mua sắm lớn trả thẳng (LumpSumExpense)
                   ├── Kịch bản 2: Vay mua trả góp định kỳ (InstallmentLoan)
                   ├── Kịch bản 3: Sụt giảm thu nhập (IncomeReduction)
                   └── Kịch bản 4: Cú sốc Biến cố khẩn cấp (EmergencyShock)
                                        │
                                        ▼
                     [ Đối Chiếu Đa Chiều Trước vs Sau ]
                   ├── DSA: Hạn mức tiêu ngày DSA' (VND/ngày)
                   ├── DTI: Tỷ số Nợ / Thu nhập DTI' (%)
                   ├── Runway: Số tháng sinh tồn Quỹ khẩn cấp' (tháng)
                   ├── Net Worth: Tài sản ròng' (VND)
                   └── OutOfMoneyDate: Ngày cạn sạch tiền' (ngày)
                                        │
                      ┌─────────────────┴─────────────────┐
                      ▼                                   ▼
          [ Luồng Phụ A1: 1-Chạm ]               [ Ngoại Lệ E1 ]
        Convert to Savings Goal API            Cảnh báo Âm Ví Khẩn Cấp
```

---

## 3. KẾ HOẠCH TRIỂN KHAI TỪNG BƯỚC

### Bước 1: Mở rộng DTOs & Models Backend (`ExpenseTracker.Application`)
1. **Cập nhật `ForecastingDtos.cs`:**
   - Mở rộng Enum `WhatIfScenarioType`:
     ```csharp
     public enum WhatIfScenarioType
     {
         LumpSumExpense = 1,
         InstallmentLoan = 2,
         IncomeReduction = 3,
         EmergencyShock = 4 // Kịch bản 4 mới
     }
     ```
   - Mở rộng `WhatIfSimulationRequestDto`:
     - Thêm `EmergencyExpenseAmount` (khoản chi phát sinh khẩn cấp).
     - Thêm `EmergencyIncomeDropPercentage` (tỷ lệ giảm thu nhập đi kèm khi gặp biến cố, mặc định 0%).
   - Mở rộng `WhatIfSimulationResultDto`:
     - Thêm `EmergencyFundRemainingBefore` & `EmergencyFundRemainingAfter`.
     - Thêm `RunwayMonthsBefore` & `RunwayMonthsAfter`.
     - Thêm `IsLiquidDeficitRisk` (cảnh báo ngoại lệ E1 khi số tiền chi vượt tiền mặt khả dụng).
   - Thêm DTO cho Luồng phụ A1:
     - `ConvertToSavingsGoalRequestDto`: `GoalName`, `TargetAmount`, `TargetDate`, `Notes`.
     - `ConvertToSavingsGoalResultDto`: `GoalId`, `GoalName`, `TargetAmount`, `TargetDate`, `MonthlyCommitment`.

### Bước 2: Nâng cấp Động cơ Mô phỏng In-Memory trong `ForecastingService`
1. **Thiết lập Baseline chính xác từ CSDL:**
   - **Nợ thật:** Truy vấn `_context.Debts.Where(d => d.UserId == userId && d.Status == DebtStatus.Active)`. Tính tổng `CurrentBalance` và tổng `MinimumPayment` hàng tháng (thay vì nhân 5% giả định).
   - **Quỹ Khẩn Cấp thật:** Truy vấn `_context.SavingsGoals.Where(g => g.UserId == userId && g.IsEmergencyFund && g.Status == SavingsGoalStatus.Active)`. Tính tổng số tiền trong quỹ dự phòng và số tháng sinh tồn:
     $$\text{RunwayMonths} = \frac{\text{EmergencyFundBalance}}{\text{EssentialMonthlySpending}}$$
     *(với EssentialMonthlySpending tính từ chi phí bình quân 30 ngày gần nhất hoặc định mức Needs của Budget)*.
2. **Triển khai Logic 4 Kịch bản:**
   - **Kịch bản 1 (LumpSumExpense):**
     - Kiểm tra ngoại lệ E1: Nếu `LumpSumAmount > liquidFunds`, đánh dấu `IsLiquidDeficitRisk = true`, `IsHighRiskDecision = true`.
     - Tính $DSA_{after} = \frac{\max(0, Flexible - LumpSum)}{RemainingDays}$.
     - Tính $NetWorth_{after} = NetWorth_{before} - LumpSum$.
     - Tính ngày cạn tiền $OutOfMoneyDate_{after}$.
   - **Kịch bản 2 (InstallmentLoan):**
     - Nghĩa vụ nợ hàng tháng mới = `currentMonthlyDebt + monthlyPayment`.
     - $DTI_{after} = \frac{\text{Nghĩa vụ nợ mới}}{\text{MonthlyIncome}} \times 100\%$. Cảnh báo đỏ nếu $DTI > 40\%$.
     - Trừ bớt khoản trả góp tháng này vào ngân sách linh hoạt: $DSA_{after} = \max(0, \frac{Flexible - MonthlyPayment}{RemainingDays})$.
   - **Kịch bản 3 (IncomeReduction):**
     - Giảm thu nhập theo %: Thu nhập mới = $Income \times (1 - \frac{ReductionPct}{100})$.
     - $DTI_{after}$ tăng lên do mẫu số thu nhập giảm.
     - Điều chỉnh DSA tương ứng để không thâm hụt.
   - **Kịch bản 4 (EmergencyShock):**
     - Trừ chi phí khẩn cấp vào Quỹ Khẩn Cấp (hoặc tiền mặt): $\text{Fund}_{after} = \max(0, \text{Fund}_{before} - \text{EmergencyExpense})$.
     - Tính lại số tháng sinh tồn: $\text{Runway}_{after} = \frac{\text{Fund}_{after}}{\text{EssentialExpense}}$.
     - Cảnh báo đỏ nếu $\text{Runway}_{after} < 3$ tháng (ngưỡng an toàn tối thiểu).
3. **Triển khai Luồng phụ A1 (`ConvertWhatIfToSavingsGoalAsync`):**
   - Tạo mới thực thể `SavingsGoal` với `UserId`, `Name`, `TargetAmount`, `TargetDate`, `IsEmergencyFund = false`.
   - Lưu vào CSDL và trả về `ConvertToSavingsGoalResultDto`.

### Bước 3: Mở rộng Web API Controller (`ForecastingController`)
1. Cập nhật `POST /api/forecasting/what-if`: Tiếp nhận request mới và gọi `SimulateWhatIfScenarioAsync`.
2. Thêm endpoint `POST /api/forecasting/what-if/convert-to-goal`:
   - Xác thực quyền sở hữu `UserId`.
   - Gọi `ConvertWhatIfToSavingsGoalAsync`.
   - Trả về HTTP 201 Created kèm dữ liệu mục tiêu tiết kiệm vừa được thiết lập.

### Bước 4: Mở rộng Phía Mobile Client (`ExpenseTracker.Maui`)
1. **Cập nhật `ForecastingModels.cs`:**
   - Bổ sung `WhatIfScenarioType.EmergencyShock`.
   - Thêm các thuộc tính mới vào `WhatIfSimulationRequestModel` và `WhatIfSimulationResultModel` (`EmergencyExpenseAmount`, `EmergencyFundRemainingAfter`, `RunwayMonthsBefore`, `RunwayMonthsAfter`, `IsLiquidDeficitRisk`).
   - Thêm `ConvertToSavingsGoalRequestModel` và `ConvertToSavingsGoalResultModel`.
2. **Cập nhật Repositories & Refit API:**
   - Bổ sung phương thức `Task<ConvertToSavingsGoalResultModel> ConvertWhatIfToGoalAsync(...)` vào `IForecastingRepository`, `ApiForecastingRepository`, và `MockForecastingRepository` (đảm bảo hoạt động Offline 0ms).
3. **Nâng cấp `ForecastingViewModel`:**
   - Bổ sung thuộc tính observable: `EmergencyExpenseAmount`, `EmergencyIncomeDropPercentage`, `RunwayBefore`, `RunwayAfter`, `EmergencyFundBefore`, `EmergencyFundAfter`.
   - Bổ sung lệnh `SelectScenarioAsync("EmergencyShock")`.
   - Bổ sung lệnh `ConvertToSavingsGoalCommand`: Cho phép người dùng chuyển kịch bản mua sắm sang mục tiêu tiết kiệm và hiển thị thông báo thành công.
4. **Cập nhật Giao diện `ForecastingPage.xaml`:**
   - Thêm Tab / Button chọn kịch bản `EmergencyShock`.
   - Hiển thị card đối chiếu chỉ số Runway (Số tháng sinh tồn quỹ dự phòng).
   - Thêm nút hành động "Chuyển thành Mục tiêu tích lũy" (Luồng A1).

### Bước 5: Viết Unit Tests Toàn Diện & Xác Minh
1. **Backend Tests (`ExpenseTracker.Application.UnitTests`):**
   - Viết các test case mới trong `ForecastingServiceTests.cs`:
     * `SimulateWhatIf_LumpSum_AccurateDsaAndDeficitRisk`: Kiểm thử kịch bản 1, tính DSA và cảnh báo khi chi quá tiền ví (E1).
     * `SimulateWhatIf_InstallmentLoan_UsesRealDebtsForDti`: Kiểm thử kịch bản 2 với dữ liệu nợ thực tế từ bảng `Debts`.
     * `SimulateWhatIf_IncomeReduction_AdjustsDsaAndDti`: Kiểm thử kịch bản 3 khi giảm thu nhập 30%.
     * `SimulateWhatIf_EmergencyShock_StressTestsEmergencyFundAndRunway`: Kiểm thử kịch bản 4 khi sụt giảm quỹ dự phòng và tính lại Runway.
     * `ConvertWhatIfToSavingsGoal_ValidInput_CreatesSavingsGoal`: Kiểm thử luồng phụ A1 tạo mục tiêu thành công.
2. **Mobile Tests (`ExpenseTracker.Maui.UnitTests`):**
   - Viết các test case mới cho What-If trong ViewModel tests:
     * `ForecastingViewModel_EmergencyShockScenario_CalculatesRunwayCorrectly`
     * `ForecastingViewModel_ConvertToGoal_CallsRepositoryAndSetsFeedback`
3. **Thực thi toàn bộ Test Suite:**
   - Chạy `dotnet test ExpenseTracker.Domain.UnitTests`.
   - Chạy `dotnet test ExpenseTracker.Application.UnitTests`.
   - Chạy `dotnet test ExpenseTracker.Maui.UnitTests`.
   - **Cam kết:** 100% tests PASSED (Tổng số tests dự kiến: **485+ tests**).

---

## 4. MA TRẬN PHÂN CÔNG TÁC TỬ HỘI ĐỒNG

| Tác tử | Vai trò | Trách nhiệm cụ thể trong Giai đoạn 07 |
| :--- | :--- | :--- |
| **Agent 01 (Richard Thaler)** | Nudge & Tâm lý tài chính | Thiết kế Cú hích Thaler chuyển đổi mua bốc đồng sang Mục tiêu tích lũy (Luồng A1) và cảnh báo ma sát tâm lý khi DTI vượt ngưỡng. |
| **Agent 03 (Cockburn BA)** | Chuẩn hóa IEEE 29148 | Bảo đảm toàn bộ logic tuân thủ chính xác 4 kịch bản, luồng A1 và ngoại lệ E1 của `UC-08.04`. |
| **Agent 04 (Eric Evans DDD)** | Kiến trúc Domain & Service | Đảm bảo nguyên tắc Sandbox Isolation (không làm ô nhiễm CSDL) và phân định ranh giới giữa Forecasting và SavingsGoals. |
| **Agent 06 (Debt Optimizer)** | Tính toán Nợ & DTI | Tích hợp công thức DTI chính xác từ bảng nợ `Debts`. |
| **Agent 10 (Dan Abramov)** | Frontend Architecture | Đảm bảo tính toán What-If trên Mobile đạt phản hồi 0ms (In-Memory / Mock offline), quản lý state sạch sẽ không lỗi rò rỉ. |
| **Agent 12 (Kent Beck QA)** | Chiến lược Kiểm thử | Thiết lập bộ test cases bao phủ 100% các nhánh rẽ của What-If engine. |
| **Agent 00 (Orchestrator)** | Điều phối & Tổng hợp | Đánh giá chất lượng, ghi nhận quyết định và cập nhật Roadmap. |

---

## 5. KẾT QUẢ ĐẦU RA KỲ VỌNG & TIÊU CHÍ HOÀN THÀNH

1. Engine What-If Sandbox mô phỏng đầy đủ 4 kịch bản, tích hợp nợ thật (`Debts`) và Quỹ Khẩn Cấp (`EmergencyFund`).
2. Luồng phụ A1 (`POST /api/forecasting/what-if/convert-to-goal`) hoạt động trơn tru.
3. Luồng ngoại lệ E1 (cảnh báo âm ví tức thời) được bắt chuẩn xác.
4. Mobile MAUI hỗ trợ trọn vẹn cả 4 kịch bản với thanh trượt mượt mà và nút chuyển đổi sang Mục tiêu tích lũy.
5. Toàn bộ Unit Tests trên Backend và Mobile chạy thành công 100% (0 fail, 0 error).
6. Ghi nhận snapshot plan vào `project_context/plans/plans-mobile-app/` và cập nhật `decisions_and_changes_log.md`.

