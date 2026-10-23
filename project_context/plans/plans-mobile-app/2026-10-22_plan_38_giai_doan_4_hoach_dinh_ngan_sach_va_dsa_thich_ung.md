# KẾ HOẠCH TRIỂN KHAI GIAI ĐOẠN 4: PHÂN HỆ 5 — HOẠCH ĐỊNH NGÂN SÁCH ĐA PHƯƠNG PHÁP & ĐỘNG CƠ HẠN MỨC DSA THÍCH ỨNG
> **Mã kế hoạch:** `PLAN-38`  
> **Phân hệ mục tiêu:** Phân hệ 5 — Quản lý Ngân sách Đa phương pháp & DSA Thích ứng (`UC-05.01` đến `UC-05.06`)  
> **Tiêu chuẩn tuân thủ:** IEEE 29148, DDD Clean Architecture, TDD Unit Tests, .NET 10 / MAUI MVVM, Quy chuẩn Quyết định 113 & 116.  
> **Ngày khởi tạo:** 2026-10-22

---

## 1. MỤC TIÊU & PHẠM VI NGHIỆP VỤ

Triển khai trọn vẹn 6 Use Cases cốt lõi của Phân hệ 5:
1. **`UC-05.01`:** Thiết lập ngân sách chu kỳ (Hàng tháng / Hàng tuần) theo 3 phương pháp chuẩn hóa:
   - Quy tắc 50/30/20 (50% Needs, 30% Wants, 20% Savings/Debt).
   - Zero-Based Budgeting (ZBB: Tổng thu nhập - Tổng ngân sách = 0).
   - Sổ Kakeibo 4 Cột trụ: Survival (Thiết yếu), Optional (Linh hoạt), Culture (Văn hóa), Extra (Bất thường).
2. **`UC-05.02`:** Theo dõi tiến độ giải ngân ngân sách thời gian thực với thanh tiến độ đổi màu công thái học (Xanh < 70%, Vàng 70-90%, Đỏ $\ge 90\%$).
3. **`UC-05.03`:** Điều chỉnh hạn mức ngân sách linh hoạt (Dynamic Rebalancing bù trừ giữa các danh mục hoặc cấp thêm ngân sách tổng).
4. **`UC-05.04`:** Tiếp nhận cảnh báo vượt ngưỡng ngân sách (Real-time Spending Micro-Alerts mốc 80%, 90%, 100% bội chi).
5. **`UC-05.05`:** Động cơ toán học tính toán Hạn mức chi tiêu mỗi ngày thích ứng (Adaptive Daily Spending Allowance - DSA) có khả năng tự co giãn (Self-correcting elasticity: tiêu lố hôm nay thì mai tự động siết, tiết kiệm hôm nay thì mai tự động nới lỏng).
6. **`UC-05.06`:** Giám sát Chỉ số Tốc độ tiêu tiền ($V_{\text{actual}} / V_{\text{target}}$) và dự phóng Ngày cạn sạch tiền (Out-of-Money Date Forecast).

---

## 2. KIẾN TRÚC KỸ THUẬT & CƠ SỞ DỮ LIỆU

### 2.1. Domain Entities & Enums
- Enums:
  - `BudgetPeriodType`: Monthly (1), Weekly (2).
  - `BudgetMethod`: Rule503020 (1), ZeroBased (2), Kakeibo (3).
  - `BudgetStatus`: Draft (1), Active (2), Closed (3).
- Entities:
  - `Budget` (Aggregate Root): `Id`, `UserId`, `Name`, `PeriodType`, `StartDate`, `EndDate`, `TotalAmount`, `Method`, `Status`, `CreatedAt`, `UpdatedAt`, danh sách `BudgetAllocations`.
  - `BudgetAllocation`: `Id`, `BudgetId`, `CategoryId`, `AllocatedAmount`, `CurrentSpent`, `AlertThresholdPercent`.
  - `DailySpendingAllowance`: `Id`, `BudgetId`, `Date`, `DailyAllowanceAmount`, `ActualSpentToday`, `RemainingAllowanceToday`, `VelocityIndex`, `OutOfMoneyForecastDate`.

### 2.2. Cơ sở dữ liệu & EF Core
- Cấu hình quan hệ 1-N giữa `Budget` và `BudgetAllocations`, 1-N giữa `Budget` và `DailySpendingAllowance`.
- Xây dựng Migration: `20261022_AddBudgetAndAdaptiveDSA`.
- Cập nhật tự động: Khi phát sinh giao dịch chi tiêu mới (`TransactionType = Expense`), tự động cập nhật `CurrentSpent` trong `BudgetAllocations` tương ứng và kích hoạt tính toán lại DSA.

### 2.3. Thuật toán Toán học Cốt lõi
- **Thuật toán DSA Thích ứng:**
  $$DSA_t = \frac{\text{RemainingFlexibleBudget}}{\text{RemainingDays}}$$
  $$\text{RemainingAllowanceToday} = DSA_t - \text{ActualSpentToday}$$
  *(Nếu $\text{RemainingFlexibleBudget} \le 0 \Rightarrow DSA_t = 0$ và kích hoạt cảnh báo đỏ).*
- **Chỉ số Tốc độ Tiêu tiền (Velocity Index):**
  $$V_{\text{actual}} = \frac{\text{CurrentSpent}}{t}, \quad V_{\text{target}} = \frac{\text{TotalBudget}}{N}, \quad \text{VelocityIndex} = \frac{V_{\text{actual}}}{V_{\text{target}}}$$
- **Dự phóng Ngày Cạn Tiền (Out-of-Money Date):**
  $$\text{DaysUntilDepletion} = \frac{\text{RemainingBudget}}{V_{\text{actual}}}, \quad \text{OutOfMoneyDate} = \text{CurrentDate} + \text{DaysUntilDepletion}$$

---

## 3. LỘ TRÌNH THI CÔNG CHI TIẾT (2 CHẶNG)

### CHẶNG 4A: BACKEND DOMAIN, APPLICATION, PERSISTENCE & API (ĐÃ HOÀN THÀNH 100%)
- [x] 1. Tạo Enums và Domain Entities: `Budget.cs`, `BudgetAllocation.cs`, `DailySpendingAllowance.cs`.
- [x] 2. Tạo EF Core Configurations và Migration `20261022161205_AddBudgetsAndAdaptiveDSA`, apply lên Docker PostgreSQL 16.
- [x] 3. Tạo DTOs và Interfaces: `IBudgetService`, `IAdaptiveDsaService`.
- [x] 4. Triển khai `BudgetService` (CRUD, Thiết lập 50/30/20, ZBB, Kakeibo, Rebalance, theo dõi tiến độ).
- [x] 5. Triển khai `AdaptiveDsaService` (Tính toán DSA, Velocity Index, Dự phóng ngày cạn tiền, Cảnh báo mốc 80%/100%).
- [x] 6. Tích hợp `TransactionService`: Tự động cộng dồn chi tiêu vào `BudgetAllocation` và hoàn trả hạn mức khi xóa chi tiêu.
- [x] 7. Viết WebApi Controller: `BudgetsController.cs` (6 RESTful fintech endpoints).
- [x] 8. Viết Unit Tests toàn diện cho Domain & Application Services (20 tests mới).

### CHẶNG 4B: FRONTEND .NET MAUI & TÍCH HỢP HỆ THỐNG (ĐÃ HOÀN THÀNH 100%)
- [x] 1. Tạo Models MAUI: `BudgetModels.cs` (kèm các computed presentation helpers).
- [x] 2. Cấu hình Refit interface: `IBudgetApi` trong `IApiClients.cs`.
- [x] 3. Xây dựng `BudgetsViewModel.cs` & `BudgetsPage.xaml`:
   - Thanh tiến độ giải ngân theo 3 dải màu công thái học (Xanh < 70%, Vàng 70-90%, Đỏ $\ge 90\%$).
   - Widget DSA Hạn mức an toàn hôm nay (có cơ chế co giãn linh hoạt).
   - Đồng hồ đo Tốc độ tiêu tiền (Velocity Index) và Hộp cảnh báo dự phóng ngày cạn sạch tiền.
- [x] 4. Xây dựng `CreateBudgetViewModel.cs` & `CreateBudgetPage.xaml`:
   - Wizard lập ngân sách hỗ trợ chọn 50/30/20 (tự tính 50%-30%-20%), ZBB (kiểm tra bất biến chênh lệch = 0 đ), Kakeibo (4 nhóm cột trụ).
- [x] 5. Đăng ký DI trong `MauiProgram.cs` và Routes trong `AppShell.xaml`.
- [x] 6. Biên dịch .NET MAUI: **0 Warning, 0 Error**.
- [x] 7. Tổ chức Phiên Kiểm Toán Hệ Thống Hậu Giai đoạn 4 (Post-Phase 4 Audit) theo Quyết định 116.

---

## 4. KẾT QUẢ NGHIỆM THU CHẤT LƯỢNG (QUALITY GATES) - ĐẠT CHUẨN XUẤT SẮC
- [x] Toàn bộ 113 Unit Tests cũ tiếp tục vượt qua 100%.
- [x] Bổ sung thêm 20 Unit Tests mới (12 Domain + 8 Application), nâng tổng số tests toàn hệ thống lên **133/133 Unit Tests Passed 100%** (54 Domain tests + 79 Application tests), 0 Error, 0 Warning.
- [x] Frontend MAUI biên dịch đạt chuẩn: **0 Warning, 0 Error**.
- [x] Ghi nhận Quyết định 117 và kết quả Phiên Kiểm Toán Hệ Thống Hậu Giai đoạn 4 vào `decisions_and_changes_log.md` và `00_MASTER_CONTEXT.md`.
- [x] **Trạng thái:** `GIAI ĐOẠN 4 HOÀN THÀNH 100% (COMPLETED & LOCKED)`. Sẵn sàng sang Giai đoạn 5.
