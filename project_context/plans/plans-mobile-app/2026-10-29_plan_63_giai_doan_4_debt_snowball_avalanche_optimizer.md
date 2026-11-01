# KẾ HOẠCH TRIỂN KHAI GIAI ĐOẠN MỚI 04: TRỤ CỘT NỢ — ĐỘNG CƠ TỐI ƯU TRẢ NỢ (DEBT SNOWBALL VS AVALANCHE & TRỢ LÝ HƯỚNG DẪN BÌNH DÂN HÓA TÀI CHÍNH)

> **Mã Giai Đoạn:** `GIAI_DOAN_MOI_04`  
> **Use Case Trọng Tâm:** `UC-06.06: Lập Kế Hoạch & So Sánh Chiến Lược Trả Nợ Cá Nhân (Snowball vs Avalanche) & Trợ Năng Định Hướng Người Dùng Phổ Thông`  
> **Tác Tử Phụ Trách:** Agent 06 (Debt & Cashflow Optimizer), Agent 01 (Richard Thaler - Behavioral Nudge), Agent 02 (Personal Budget Coach), Agent 09/10 (Brad Frost & Dan Abramov - Layman UX & Client State), Agent 12 (Kent Beck - QA)  
> **Trạng Thái Phê Duyệt:** Đang chờ Người dùng xem xét & duyệt lại (Phiên bản v2 bổ sung Giáo dục & Hướng dẫn bình dân hóa)  

---

## 1. Mục Tiêu Kỹ Thuật & Giá Trị Nghiệp Vụ (Goal Description)

Xây dựng **Động cơ Tối ưu hóa Trả nợ Cá nhân kết hợp Trợ lý Hướng dẫn Bình dân hóa Tài chính (Layman Financial Guide)**:
1. **Bình dân hóa Thuật ngữ Kinh tế (Eliminate Financial Jargon):**
   - Đa số người dùng không hiểu các khái niệm học thuật như *Amortization, Dư nợ giảm dần, APR, Avalanche, Snowball, Negative Amortization*.
   - Hệ thống chuyển đổi toàn bộ ngôn ngữ hiển thị sang tiếng Việt bình dân, trực quan, dễ liên tưởng thực tế:
     - **Debt Snowball:** Đặt tên phụ *"Hòn Tuyết Lăn — Ưu tiên trả dứt điểm món nợ nhỏ để nhẹ đầu"*.
     - **Debt Avalanche:** Đặt tên phụ *"Tuyết Lở — Ưu tiên triệt tiêu món nợ lãi cao nhất để đỡ tốn tiền"*.
2. **Bộ Trắc Nghiệm Tự Định Hướng 1 Chạm (Interactive Strategy Selector Quiz):**
   - Cho người dùng chọn 1 trong 2 tình thế cảm xúc cá nhân:
     - 🧘 *"Tôi đang bị stress vì có quá nhiều bên đòi nợ cùng lúc, tôi muốn xóa bớt danh sách nợ càng sớm càng tốt"* &rarr; **Khuyên dùng Snowball**.
     - 💰 *"Tôi bình tĩnh, tôi muốn tiết kiệm từng đồng tiền lãi trả cho ngân hàng, không để họ ăn chặn lãi"* &rarr; **Khuyên dùng Avalanche**.
3. **Bộ Ước Lượng Ngân Sách Trả Thêm An Toàn (Safe Extra Budget Estimator):**
   - Người dùng thường lúng túng không biết nên điền bao nhiêu vào ô "Ngân sách trả thêm hàng tháng".
   - Hệ thống tự động phân tích dòng tiền thặng dư thực tế (Thu nhập - Chi tiêu thiết yếu - Nợ tối thiểu) và đưa ra 3 mức đề xuất dạng Chip chọn nhanh:
     - 🟢 **Thận trọng (10% thặng dư):** Trả thêm nhẹ nhàng, giữ lại tiền phòng thân an toàn.
     - 🟡 **Khuyên dùng (30% thặng dư):** Vừa rút ngắn đáng kể thời gian sạch nợ, vừa duy trì đời sống ổn định.
     - 🔴 **Quyết liệt (50% thặng dư):** Dành cho người muốn thoát nợ thần tốc.
     - ⚠️ **Cảnh báo an toàn (Safety Guardrail):** Chặn người dùng dồn 100% tiền dư vào trả nợ kẻo rơi vào tình trạng "cháy túi" khi gặp biến cố bất ngờ.
4. **Mô hình Toán học Lãi suất Dư nợ Giảm dần & Chốt chặn Bẫy nợ:**
   - Tính toán lãi suất từng tháng theo chuẩn Thông tư 14/2017/TT-NHNN với kiểu `decimal`, làm tròn số nguyên VND.
   - Giới hạn tối đa 360 tháng (30 năm) chống vòng lặp vô hạn.
   - Tự động phát hiện bẫy nợ **Khấu hao âm (Negative Amortization)** khi tiền trả không đủ bù tiền lãi hàng tháng, lập tức bật cảnh báo đỏ.
5. **Khả năng Vận hành Ngoại tuyến (Offline-First Resilience):**
   - Tích hợp toàn bộ logic mô phỏng và hướng dẫn trong `MockDebtRepository` để app chạy trơn tru 0ms trên điện thoại ngay cả khi không có kết nối backend.

---

## 2. Điểm Cần Người Dùng Duyệt (User Review Required)

> [!IMPORTANT]
> **Ngôn ngữ Bình dân hóa & Trợ năng Hướng dẫn:**
> - Trên giao diện ứng dụng, cạnh mỗi phương pháp sẽ luôn có biểu tượng dấu hỏi `(?)` hoặc nút "Xem giải thích dễ hiểu".
> - Bản so sánh luôn có một thẻ tóm tắt nổi bật: **"Bạn nên chọn cách nào?"** với kết luận rõ ràng bằng 2 dòng văn bản đời thường, không ép người dùng phải tự phân tích số liệu phức tạp.

> [!NOTE]
> **Quy chuẩn 4 Quyết định Kỹ thuật đã được chốt:**
> 1. Giới hạn mô phỏng tối đa **360 tháng (30 năm)**.
> 2. Chặn và bật cờ cảnh báo đỏ **Khấu hao âm (Negative Amortization)** nếu tiền trả hàng tháng $<$ tiền lãi phát sinh.
> 3. Ngưỡng Cú hích Nudge: Nếu Avalanche tiết kiệm được **$\ge 1.000.000$đ** tiền lãi và rút ngắn $\ge 2$ tháng $\rightarrow$ Khuyên chọn Avalanche; ngược lại khuyên chọn Snowball để nhẹ đầu.
> 4. Triển khai **Offline Engine** trong `MockDebtRepository` phục vụ chạy độc lập không phụ thuộc mạng.

---

## 3. Câu Hỏi Mở (Open Questions)

- *Không có câu hỏi mở chặn tiến độ*: Toàn bộ yêu cầu giải thích và bình dân hóa cho người dùng phổ thông đã được tích hợp đầy đủ vào kiến trúc v2.

---

## 4. Chi Tiết Đề Xuất Thay Đổi (Proposed Changes)

```mermaid
flowchart TD
    subgraph UI_UX [Tầng Giao Diện & Trợ Năng Hướng Dẫn]
        Quiz[Trắc Nghiệm Tâm Lý 1 Chạm: Stress hay Tiết Kiệm?]
        QuickChips[Chip Ngân Sách An Toàn: 500k | 1Tr | 2Tr | Khuyên Dùng]
        EducationCard[Thẻ Hướng Dẫn Dễ Hiểu: Snowball vs Avalanche là gì?]
        CompareView[Bảng So Sánh & Thẻ Khuyến Nghị: Bạn Nên Chọn Cách Nào?]
    end

    subgraph Logic_Engine [Động Cơ Tính Toán & Phân Tích]
        SafeBudget[Safe Extra Budget Estimator]
        AmortEngine[Amortization Engine: decimal VND]
        NegAmortCheck{Tiền trả < Lãi tháng?}
        NegAmortCheck -- Đúng --> AlertRed[Bật Cờ Cảnh Báo Đỏ Bẫy Nợ]
        NegAmortCheck -- Sai --> SimLoop[Chạy Song Song Snowball vs Avalanche]
        SimLoop --> NudgeRules[Quy Tắc Cú Hích Thaler: Ngưỡng 1.000.000đ]
    end

    UI_UX --> Logic_Engine
```

---

### Component 1: Application Layer — DTOs, Mô Hình Hướng Dẫn & Tối Ưu Nợ

#### [MODIFY] `mobile-project/dotnet-maui-ver/backend/ExpenseTracker.Application/DTOs/Debts/DebtDtos.cs`
Bổ sung các DTOs cho động cơ tối ưu và hướng dẫn bình dân hóa:
- **DTOs Giáo dục & Hướng dẫn (Educational & Guidance DTOs):**
  - `DebtMethodExplanationDto`: Giải thích chi tiết một phương thức bằng văn phong đời thường (`MethodKey`, `Title`, `SubTitle`, `HowItWorks`, `BestForWhom`, `RealLifeExample`, `Pros`, `Cons`).
  - `DebtEducationalGuideDto`: Cung cấp trọn gói dữ liệu hướng dẫn gồm:
    - Giải thích Snowball & Avalanche.
    - 2 câu hỏi trắc nghiệm tự định hướng ("Chọn tình huống của bạn").
    - Hướng dẫn chọn ngân sách trả thêm an toàn (Nguyên tắc 10% - 30% - 50% thặng dư, cảnh báo không dồn 100%).
  - `SafeExtraBudgetRecommendationDto`: Gợi ý số tiền trả thêm an toàn dựa trên thu chi thực tế của người dùng (`ConservativeAmount`, `RecommendedAmount`, `AggressiveAmount`, `MaxSafeAmount`, `AdviceText`).
- **DTOs Tính toán & Mô phỏng:**
  - `DebtSimulationItemDto`
  - `DebtPayoffSimulationRequestDto` (kèm `MonthlyExtraBudget`, danh sách tùy chọn `CustomDebts`)
  - `DebtMonthlyPaymentDetailDto`
  - `DebtPayoffTimelineMonthDto`
  - `DebtStrategySimulationResultDto`
  - `DebtPayoffComparisonDto` (kèm kết luận bằng văn phong bình dân `LaymanRecommendationText`).

---

### Component 2: Application Layer — Service Động Cơ Tối Ưu & Hướng Dẫn

#### [NEW] `mobile-project/dotnet-maui-ver/backend/ExpenseTracker.Application/Interfaces/IDebtOptimizerService.cs`
```csharp
public interface IDebtOptimizerService
{
    // Lấy nội dung hướng dẫn bình dân hóa và câu hỏi định hướng
    DebtEducationalGuideDto GetEducationalGuide();

    // Tính toán gợi ý ngân sách trả thêm an toàn dựa trên thu chi thực tế của người dùng
    Task<SafeExtraBudgetRecommendationDto> GetSafeExtraBudgetRecommendationAsync(
        Guid userId, 
        CancellationToken ct = default);

    // So sánh song song hai chiến lược kèm khuyến nghị hành vi đời thường
    Task<DebtPayoffComparisonDto> CompareStrategiesAsync(
        Guid userId, 
        DebtPayoffSimulationRequestDto request, 
        CancellationToken ct = default);

    // Mô phỏng chi tiết chiến lược Snowball
    Task<DebtStrategySimulationResultDto> SimulateSnowballAsync(
        Guid userId, 
        decimal monthlyExtraBudget, 
        List<DebtSimulationItemDto>? customDebts = null, 
        CancellationToken ct = default);

    // Mô phỏng chi tiết chiến lược Avalanche
    Task<DebtStrategySimulationResultDto> SimulateAvalancheAsync(
        Guid userId, 
        decimal monthlyExtraBudget, 
        List<DebtSimulationItemDto>? customDebts = null, 
        CancellationToken ct = default);
}
```

#### [NEW] `mobile-project/dotnet-maui-ver/backend/ExpenseTracker.Application/Services/DebtOptimizerService.cs`
Triển khai nghiệp vụ:
1. `GetEducationalGuide()`: Trả về nội dung giải thích chuẩn mực được biên soạn bởi Agent 01 & Agent 02:
   - *Hòn tuyết lăn (Snowball):* "Giống như đẩy một hòn tuyết từ đỉnh đồi, bạn giải quyết món nợ nhỏ nhất trước để ăn mừng chiến thắng sớm, giải tỏa áp lực tinh thần."
   - *Tuyết lở (Avalanche):* "Giống như chặn dòng tuyết lở nguy hiểm nhất trước, bạn đè bẹp món nợ có lãi suất cao nhất (thường là thẻ tín dụng) để ngân hàng không thể bòn rút tiền của bạn."
2. `GetSafeExtraBudgetRecommendationAsync()`:
   - Truy vấn thu nhập bình quân và chi phí thiết yếu gần nhất của người dùng.
   - Thặng dư khả dụng = $\text{Thu nhập} - \text{Chi phí thiết yếu} - \text{Tổng tiền trả nợ tối thiểu}$.
   - Nếu thặng dư $\le 0$: Khuyên người dùng tạm thời chỉ trả số tiền tối thiểu và tìm cách thắt chặt chi tiêu.
   - Nếu thặng dư $> 0$: Đề xuất 3 mốc (10%, 30%, 50%) và đặt trần bảo vệ 70% thặng dư để luôn giữ lại tiền dự phòng khẩn cấp.
3. `CompareStrategiesAsync()`:
   - Chạy mô phỏng song song Snowball và Avalanche.
   - So sánh chênh lệch tiền lãi tiết kiệm và thời gian.
   - Sinh khuyến nghị đời thường `LaymanRecommendationText`:
     - Nếu Avalanche tiết kiệm $\ge 1.000.000$đ: *"Avalanche là lựa chọn khôn ngoan nhất cho túi tiền của bạn! Bạn sẽ tiết kiệm được [X] đồng tiền lãi (đủ tiền mua sắm sinh hoạt cho nhiều tuần). Hãy tập trung dồn tiền đè bẹp khoản [Tên khoản lãi cao nhất] trước nhé!"*
     - Nếu chênh lệch ít hơn 1.000.000đ: *"Chênh lệch tiền lãi giữa 2 cách không đáng kể ([X] đồng). Chúng tôi khuyên bạn nên chọn Snowball để xóa sổ khoản [Tên khoản dư nợ nhỏ nhất] nhanh nhất, giúp bạn bớt đi một mối lo và nhẹ đầu hơn!"*
4. Kiểm tra bẫy nợ Khấu hao âm (`Negative Amortization`) và giới hạn 360 tháng.

---

### Component 3: WebApi Layer — Endpoints

#### [MODIFY] `mobile-project/dotnet-maui-ver/backend/ExpenseTracker.WebApi/Controllers/DebtsController.cs`
Bổ sung các endpoints:
- `GET /api/debts/optimization/guide`: Lấy toàn bộ nội dung hướng dẫn bình dân hóa, ví dụ thực tế và bộ câu hỏi trắc nghiệm định hướng.
- `GET /api/debts/optimization/safe-budget`: Lấy gợi ý ngân sách trả thêm an toàn dựa trên tình hình tài chính của người dùng.
- `POST /api/debts/optimization/compare`: Mô phỏng và so sánh Snowball vs Avalanche.
- `POST /api/debts/optimization/simulate`: Mô phỏng chi tiết một chiến lược.

#### [MODIFY] `mobile-project/dotnet-maui-ver/backend/ExpenseTracker.Infrastructure/DependencyInjection.cs`
Đăng ký `services.AddScoped<IDebtOptimizerService, DebtOptimizerService>();`.

#### [MODIFY] `mobile-project/dotnet-maui-ver/implement-docs/API_CONTRACT_SPECIFICATION.md`
Cập nhật tài liệu hợp đồng API mục 4.10 bổ sung đầy đủ 4 endpoints tối ưu và hướng dẫn.

---

### Component 4: Mobile MAUI — Models, Refit API, Repository & Offline Mode

#### [NEW] `mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/Models/Debts/DebtOptimizationModels.cs`
Bao gồm:
- `DebtEducationalGuideModel`, `DebtMethodExplanationModel`, `SafeExtraBudgetRecommendationModel`.
- `DebtPayoffComparisonModel`, `DebtStrategySimulationResultModel`, `DebtPayoffTimelineMonthModel`, `DebtPayoffSimulationRequest`.

#### [MODIFY] `mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/Services/Api/IApiClients.cs`
Thêm các endpoints vào `IDebtsApi`:
```csharp
[Get("/api/debts/optimization/guide")]
Task<ApiResponse<DebtEducationalGuideModel>> GetEducationalGuideAsync();

[Get("/api/debts/optimization/safe-budget")]
Task<ApiResponse<SafeExtraBudgetRecommendationModel>> GetSafeBudgetRecommendationAsync();

[Post("/api/debts/optimization/compare")]
Task<ApiResponse<DebtPayoffComparisonModel>> ComparePayoffStrategiesAsync([Body] DebtPayoffSimulationRequest request);
```

#### [MODIFY] `mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/Repositories/Debts/IDebtRepository.cs`
Khai báo 3 phương thức mới:
- `Task<DebtEducationalGuideModel> GetEducationalGuideAsync();`
- `Task<SafeExtraBudgetRecommendationModel> GetSafeBudgetRecommendationAsync();`
- `Task<DebtPayoffComparisonModel> CompareStrategiesAsync(decimal extraBudget, List<DebtSimulationItemModel>? customDebts = null);`

#### [MODIFY] `mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/Repositories/Debts/ApiDebtRepository.cs`
Triển khai gọi các API tương ứng từ Refit.

#### [MODIFY] `mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/Repositories/Debts/MockDebtRepository.cs`
- Tích hợp sẵn `DebtEducationalGuideModel` tiếng Việt chuẩn chỉnh.
- Thuật toán ước lượng ngân sách thặng dư an toàn offline dựa trên số dư các ví mẫu.
- Thuật toán mô phỏng Snowball & Avalanche offline 0ms.

#### [NEW] `mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/ViewModels/DebtOptimizerViewModel.cs`
ViewModel đóng gói toàn bộ tương tác người dùng:
- `EducationalGuide`: Dữ liệu bài hướng dẫn bình dân hóa.
- `SafeBudgetRecommendation`: Các mốc tiền gợi ý (Chip chọn nhanh: Thận trọng, Khuyên dùng, Quyết liệt).
- `SelectedPersonality`: Cờ trắc nghiệm ("Tôi muốn nhẹ đầu" vs "Tôi muốn tiết kiệm tiền").
- `ExtraMonthlyBudget`: Số tiền trả thêm hàng tháng do người dùng kéo slider hoặc bấm chip.
- `ComparisonResult`: Kết quả so sánh hai chiến lược.
- `LaymanAdvice`: Lời khuyên bằng tiếng Việt đời thường hiển thị nổi bật ở đầu trang.
- Các lệnh tương tác: `SelectPersonalityCommand`, `ApplySuggestedBudgetCommand`, `SimulateCommand`, `ToggleGuideCommand`.

---

### Component 5: Kiểm Thử Tự Động (Verification & Unit Tests)

#### [NEW] `mobile-project/dotnet-maui-ver/backend/tests/ExpenseTracker.Application.UnitTests/Services/DebtOptimizerServiceTests.cs`
Bao gồm tối thiểu 12 kịch bản kiểm thử:
1. `GetEducationalGuide_ReturnsClearLaymanExplanations`: Hướng dẫn có đầy đủ giải thích bình dân cho cả Snowball và Avalanche.
2. `GetSafeExtraBudgetRecommendation_CalculatesConservativeAndAggressiveBudgets`: Tính đúng 3 mức ngân sách dựa trên thặng dư thu chi.
3. `GetSafeExtraBudgetRecommendation_WhenDeficit_WarnsUserToOnlyPayMinimum`: Cảnh báo chỉ trả tối thiểu khi thặng dư $\le 0$.
4. `SimulateSnowball_PrioritizesLowestBalanceFirst`: Trả hết khoản nợ nhỏ nhất trước.
5. `SimulateAvalanche_PrioritizesHighestInterestFirst`: Trả hết khoản nợ lãi suất cao nhất trước.
6. `CompareStrategies_AvalancheSavesMoreOrEqualInterest`: Xác nhận về mặt toán học Avalanche luôn trả ít lãi hơn hoặc bằng Snowball.
7. `CompareStrategies_WhenExtraBudgetIsZero_CalculatesValidPayoff`: Vẫn tính toán chính xác khi người dùng không có tiền trả thêm.
8. `CompareStrategies_RollsOverMinimumPayment_WhenDebtPaidOff`: Hiệu ứng hòn tuyết lăn dồn tiền khi có một khoản tất toán.
9. `CompareStrategies_DetectsNegativeAmortization_WhenPaymentTooLow`: Cảnh báo bẫy nợ đỏ khi tiền trả $<$ tiền lãi phát sinh.
10. `CompareStrategies_WhenSavingsExceedOneMillion_RecommendsAvalanche`: Khuyến nghị Avalanche khi tiết kiệm $\ge 1.000.000$đ.
11. `CompareStrategies_WhenSavingsBelowOneMillion_RecommendsSnowball`: Khuyến nghị Snowball khi chênh lệch lãi nhỏ để giảm stress.
12. `CompareStrategies_CapsAtMax360Months`: Không bao giờ bị vòng lặp vô hạn.

#### [NEW] `mobile-project/dotnet-maui-ver/frontend/tests/ExpenseTracker.Maui.UnitTests/Repositories/DebtOptimizerRepositoryTests.cs`
- `MockDebtRepository_GetEducationalGuide_ReturnsCompleteGuide`: Dữ liệu hướng dẫn offline sẵn sàng 100%.
- `MockDebtRepository_CompareStrategies_ReturnsAccurateSimulation`: Mô phỏng offline tính đúng kết quả so sánh.
- `ApiDebtRepository_Methods_CallRefitEndpointsCorrectly`: Gọi đúng các endpoint tương ứng.

---

## 5. Kế Hoạch Xác Minh & Nghiệm Thu (Verification Plan)

### Automated Tests
1. Chạy toàn bộ Unit Tests của Backend:
   ```powershell
   dotnet test mobile-project/dotnet-maui-ver/backend/ExpenseTracker.slnx
   ```
   *Mục tiêu:* Đạt tối thiểu **329/329 tests PASSED 100%** (104 Domain + 225 Application).

2. Chạy toàn bộ Unit Tests của Mobile MAUI:
   ```powershell
   dotnet test mobile-project/dotnet-maui-ver/frontend/tests/ExpenseTracker.Maui.UnitTests/ExpenseTracker.Maui.UnitTests.csproj
   ```
   *Mục tiêu:* Đạt tối thiểu **99/99 tests PASSED 100%**.

3. Tổng hợp toàn hệ thống: Đạt tối thiểu **428/428 tests PASSED (0 Error, 0 Warning phát sinh)**.

### Manual Verification
- Kiểm tra nội dung hướng dẫn `EducationalGuide`: Đọc thử văn phong, đảm bảo không có câu từ khó hiểu hoặc thuật ngữ tiếng Anh chưa dịch nghĩa.
- Kiểm tra Trắc nghiệm tâm lý: Chọn "Tôi muốn nhẹ đầu" &rarr; Hệ thống lập tức highlight chiến lược Snowball; chọn "Tôi muốn tiết kiệm tiền" &rarr; Hệ thống highlight Avalanche.
- Thử nghiệm các mức Chip ngân sách trả thêm: Bấm `+500k`, `+1tr`, `Khuyên dùng` &rarr; Biểu đồ timeline lập tức cập nhật số tháng sạch nợ tức thì 0ms.
