# KẾ HOẠCH TRIỂN KHAI GIAI ĐOẠN 5: PHÂN HỆ 6 — QUẢN LÝ MỤC TIÊU TÍCH LŨY THÔNG MINH & HŨ TÀI CHÍNH
> **Mã kế hoạch:** `PLAN-39`  
> **Phân hệ mục tiêu:** Phân hệ 6 — Quản lý Mục tiêu Tích lũy Thông minh & Hũ Tài Chính (Smart Savings Goals & Financial Jars - `UC-06.01` đến `UC-06.05`)  
> **Tiêu chuẩn tuân thủ:** IEEE 29148, Rich Domain Model DDD, TDD Unit Tests, Clean Architecture, .NET 10 / MAUI MVVM, Quy chuẩn Quyết định 113 & 116.  
> **Ngày khởi tạo:** 2026-10-23  

---

## 1. MỤC TIÊU & PHẠM VI NGHIỆP VỤ (5 USE CASES)

Phân hệ 6 hiện thực hóa nguyên tắc tài chính kinh điển *"Pay Yourself First"* (Hãy trả cho mình trước) và xây dựng bệ đỡ an toàn tài chính cá nhân bền vững thông qua 5 Use Cases:

1. **`UC-06.01` — Thiết lập mục tiêu tích lũy thông minh (Smart Savings Goal Setup):**
   - Khởi tạo mục tiêu: Tên mục tiêu, Số tiền mục tiêu (`TargetAmount`), Số tiền ban đầu, Ngày bắt đầu, Hạn chót hoàn thành (`TargetDate`).
   - Phân loại mục tiêu (`GoalType`):
     - `EmergencyFund` (Quỹ khẩn cấp: Ràng buộc tối thiểu 3 - 6 tháng chi phí thiết yếu Needs từ Phân hệ 5).
     - `MajorPurchase` (Mua sắm lớn: Xe, nhà, đồ công nghệ).
     - `RetirementOrInvestment` (Hưu trí / Tự do tài chính).
     - `EducationOrGrowth` (Học tập & Phát triển).
     - `TravelOrLeisure` (Du lịch / Trải nghiệm).
   - Kiểm tra tính khả thi tự động (Feasibility Check):
     $$\text{MonthlyDepositRequired} = \frac{\text{TargetAmount} - \text{CurrentAmount}}{\text{MonthsRemaining}}$$
     So sánh số tiền cần nạp mỗi tháng với hạn mức 20% Tích lũy (Savings) trong ngân sách 50/30/20 của Phân hệ 5 để cảnh báo nếu mục tiêu vượt quá khả năng tài chính thực tế.

2. **`UC-06.02` — Nạp tiền vào quỹ tích lũy từ ví thanh toán (Deposit to Savings Goal):**
   - Người dùng chuyển một số tiền từ ví thanh toán (Tiền mặt, Tài khoản ngân hàng, Ví điện tử) vào mục tiêu tích lũy.
   - Giao dịch liên module chuẩn ACID:
     - Khấu trừ số dư ví nguồn (`Wallet.Debit`).
     - Cộng dồn số tiền vào mục tiêu (`SavingsGoal.CurrentAmount += Amount`).
     - Tạo bản ghi lịch sử đóng góp quỹ (`SavingsGoalContribution` với `Type = Deposit`).
     - Tự động sinh giao dịch Sổ cái (`TransactionType = SavingsContribution`) để bảo đảm cân đối sổ sách kế toán.
     - Kiểm tra nếu `CurrentAmount >= TargetAmount` $\rightarrow$ Tự động chuyển trạng thái `IsCompleted = true` và ghi nhận `CompletedAt`.

3. **`UC-06.03` — Rút tiền từ quỹ tích lũy & Cú hích ma sát nhận thức (Withdrawal & Thaler Nudge):**
   - Cho phép rút tiền từ quỹ tích lũy về một ví thanh toán khi người dùng cần giải ngân.
   - **Cú hích ma sát tâm lý (Richard Thaler Cognitive Friction Nudge):**
     - Nếu mục tiêu chưa hoàn thành (`IsCompleted == false`), hệ thống hiển thị hộp thoại cảnh báo: Rút $X$ VNĐ sẽ làm chậm tiến độ đạt mục tiêu thêm $N$ ngày.
     - Yêu cầu bắt buộc người dùng nhập lý do rút tiền (`WithdrawalReason`).
     - Nếu là Quỹ khẩn cấp (`EmergencyFund`), áp dụng cảnh báo đặc biệt nghiêm ngặt: *"Quỹ khẩn cấp chỉ nên dùng cho các biến cố y tế, thất nghiệp hoặc tai nạn bất khả kháng"*.
   - Giao dịch ACID:
     - Trừ tiền khỏi mục tiêu (`SavingsGoal.CurrentAmount -= Amount`).
     - Cộng tiền vào ví đích (`Wallet.Credit`).
     - Ghi nhận lịch sử đóng góp âm (`Type = Withdrawal`).

4. **`UC-06.04` — Thiết lập lịch trích nạp tự động (Auto-Deposit Schedule):**
   - Cấu hình lịch nạp tự động định kỳ: Tần suất (Hàng tháng / Hàng tuần), Ngày trích nạp, Số tiền trích nạp mỗi kỳ, Ví nguồn trích nợ.
   - Liên kết trực tiếp với Phân hệ Thu nhập (Phân hệ 2): Khi có giao dịch lương về (`UC-02.04`), hệ thống gợi ý trích ngay phần tích lũy trước khi chi tiêu.

5. **`UC-06.05` — Hệ thống Hũ tài chính Kakeibo / 6 Chiếc Hũ (Financial Jars Allocation):**
   - Trực quan hóa tổng tài sản tích lũy của người dùng và cơ cấu phân bổ vào các Hũ tài chính: Hũ Tự do tài chính, Hũ Tiết kiệm dài hạn, Hũ Học vấn, Hũ Khẩn cấp.
   - Tự động liên kết với tỷ lệ 20% Tích lũy từ Ngân sách chu kỳ (Phân hệ 5).

---

## 2. THIẾT KẾ KIẾN TRÚC & CƠ SỞ DỮ LIỆU (DDD & UNIVERSAL DB 3NF)

### 2.1. Domain Entities & Enums (Tầng Domain)
- **`SavingsGoalEnums.cs`:**
  - `SavingsGoalType`: `EmergencyFund` (1), `MajorPurchase` (2), `Investment` (3), `Education` (4), `Travel` (5), `Other` (6).
  - `SavingsGoalStatus`: `Active` (1), `Completed` (2), `Paused` (3), `Cancelled` (4).
  - `SavingsGoalPriority`: `Low` (1), `Medium` (2), `High` (3).
  - `ContributionType`: `Deposit` (1), `Withdrawal` (2), `InterestOrBonus` (3).
- **`SavingsGoal.cs` (Aggregate Root):**
  - Thuộc tính: `Id`, `UserId`, `Name`, `Description`, `TargetAmount`, `CurrentAmount`, `StartDate`, `TargetDate`, `Type`, `Status`, `Priority`, `IconKey`, `ColorHex`, `CompletedAt`.
  - Tính năng trích tự động: `IsAutoDepositEnabled`, `AutoDepositAmount`, `AutoDepositDayOfMonth`, `AutoDepositWalletId`.
  - Rich Domain Methods:
    - `Deposit(decimal amount, Guid walletId, string? note)`
    - `Withdraw(decimal amount, Guid targetWalletId, string reason)`
    - `CalculateMonthlyDepositRequired(DateTime currentDate)`
    - `CalculateFeasibility(decimal monthlyAvailableSavings)`
    - `CheckCompletion()`
- **`SavingsGoalContribution.cs` (Entity lịch sử giao dịch quỹ):**
  - `Id`, `SavingsGoalId`, `WalletId`, `TransactionId` (nullable), `Amount`, `Type`, `ContributionDate`, `Note`, `CreatedAt`.

### 2.2. Persistence & EF Core
- Configurations: `SavingsGoalConfiguration.cs`, `SavingsGoalContributionConfiguration.cs`.
- Tích hợp vào `IApplicationDbContext` và `ApplicationDbContext`.
- Migration EF Core: `20261023_AddSavingsGoalsAndContributions`.

### 2.3. Application Layer
- DTOs: `SavingsGoalDto`, `CreateSavingsGoalDto`, `UpdateSavingsGoalDto`, `DepositGoalDto`, `WithdrawGoalDto`, `SavingsGoalContributionDto`, `GoalFeasibilityDto`.
- Interfaces: `ISavingsGoalService.cs`.
- Triển khai `SavingsGoalService.cs`:
  - CRUD mục tiêu tích lũy.
  - `DepositAsync`: Nạp tiền ví $\rightarrow$ quỹ (Giao dịch ACID bọc trong Transaction).
  - `WithdrawAsync`: Rút tiền quỹ $\rightarrow$ ví (Có lưu lý do và ghi sổ cái).
  - `GetFeasibilityAnalysisAsync`: Phân tích tính khả thi so với ngân sách active Phân hệ 5.
  - `GetJarsOverviewAsync`: Thống kê cơ cấu các hũ tài chính.

### 2.4. WebApi Presentation Layer
- `SavingsGoalsController.cs` (`api/savings-goals`):
  - `GET /api/savings-goals`: Lấy danh sách mục tiêu tích lũy.
  - `GET /api/savings-goals/{id}`: Xem chi tiết mục tiêu kèm lịch sử đóng góp.
  - `POST /api/savings-goals`: Tạo mục tiêu mới.
  - `PUT /api/savings-goals/{id}`: Cập nhật thông tin mục tiêu.
  - `POST /api/savings-goals/{id}/deposit`: Nạp tiền vào quỹ.
  - `POST /api/savings-goals/{id}/withdraw`: Rút tiền khỏi quỹ (Thaler Nudge).
  - `GET /api/savings-goals/{id}/feasibility`: Kiểm tra tính khả thi dòng tiền.
  - `GET /api/savings-goals/jars-summary`: Thống kê cơ cấu các hũ tài chính.

---

## 3. LỘ TRÌNH THI CÔNG CHI TIẾT (2 CHẶNG)

### CHẶNG 5A: BACKEND DOMAIN, PERSISTENCE, APPLICATION & API
1. Viết Enums và Domain Entities: `SavingsGoal.cs`, `SavingsGoalContribution.cs`.
2. Tạo EF Core Configurations và Migration `AddSavingsGoalsAndContributions`.
3. Tạo DTOs và Interface `ISavingsGoalService`.
4. Triển khai `SavingsGoalService`: Nạp/Rút tiền chuẩn ACID, tính toán tiến độ, ma sát rút tiền sớm, đối soát ngân sách.
5. Viết WebApi Controller: `SavingsGoalsController.cs`.
6. Viết Unit Tests toàn diện cho Domain & Application Services.

### CHẶNG 5B: FRONTEND .NET MAUI WIREFRAME
1. Tạo Models: `SavingsGoalModels.cs`.
2. Khai báo Refit API Client: `ISavingsGoalApi` trong `IApiClients.cs`.
3. Xây dựng `SavingsGoalsViewModel.cs` & `SavingsGoalsPage.xaml`:
   - Danh sách thẻ Mục tiêu tích lũy (Tiến độ thanh %, Số tiền hiện có / Số tiền mục tiêu, Số ngày còn lại).
   - Thẻ Thống kê Hũ tài chính Kakeibo / 6 Chiếc Hũ.
   - Nút Nạp tiền nhanh & Rút tiền (kèm Popup cảnh báo Richard Thaler).
4. Xây dựng `CreateSavingsGoalPage.xaml`:
   - Form tạo mục tiêu, chọn loại quỹ (Quỹ khẩn cấp, Mua sắm, Đầu tư...), hạn chót.
   - Máy tính tính toán số tiền cần tiết kiệm mỗi tháng và cảnh báo tính khả thi.
5. Đăng ký DI trong `MauiProgram.cs` và Routes trong `AppShell.xaml`.
6. Biên dịch .NET MAUI: Đảm bảo **0 Warning, 0 Error**.
7. Tổ chức Phiên Kiểm Toán Hệ Thống Hậu Giai Đoạn 5 (Post-Phase 5 Audit) theo Quyết định 116.

---

## 4. TIÊU CHÍ NGHIỆM THU CHẤT LƯỢNG (QUALITY GATES)
- Duy trì 100% pass rate cho 133 Unit Tests hiện có.
- Bổ sung tối thiểu 15+ Unit Tests mới cho Mục tiêu tích lũy và Nạp/Rút quỹ, nâng tổng số test lên $\ge 148$ tests.
- Frontend MAUI biên dịch đạt chuẩn: **0 Warning, 0 Error**.
- Báo cáo rõ các luồng phụ chờ giai đoạn sau bổ trợ (ví dụ: Auto-trigger background job nạp tiền tự động theo ngày lương).
