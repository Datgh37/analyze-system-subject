# KẾ HOẠCH TRIỂN KHAI: CỔNG KIỂM TOÁN BACKEND TOÀN HỆ THỐNG
## (FULL BACKEND AUDIT GATE — CODE REVIEW & HARDENING, EF CORE MIGRATION POSTGRESQL, API CONTRACT SPECIFICATION & INTEGRATION TEST SUITE)

> **Mã kế hoạch:** `PLAN-70`  
> **Căn cứ quyết định:** [Quyết Định 195](../../decisions_and_changes_log.md), [Quyết Định 199](../../decisions_and_changes_log.md), Yêu cầu kiểm toán code toàn diện từ Người dùng.  
> **Trọng tâm:** Khóa chốt chất lượng toàn bộ tầng Backend sau chuỗi Giai đoạn 3 đến 10; rà soát và vá mọi lỗi tiềm ẩn trong mã nguồn trước khi bước sang giai đoạn Visual Design & UI Remake.

---

## 1. MỤC TIÊU & BỐI CẢNH (GOAL DESCRIPTION)

Trong suốt chuỗi phát triển từ **Giai đoạn Mới 03 đến Giai đoạn Mới 10**, hệ thống đã bổ sung nhiều thực thể miền, thuật toán và endpoints mới (Danh mục nợ cá nhân, Tối ưu trả nợ Snowball/Avalanche, Quỹ khẩn cấp, Báo cáo Net Worth, Quét bẫy thuê bao định kỳ, Mô phỏng biến cố What-If, AI Vision OCR, AI Voice Assistant và Android Push Bank Listener). 

Theo đúng chỉ đạo từ Người dùng và cam kết tại **Quyết định 195 & 199**, cổng kiểm toán này bao gồm **4 trụ cột trọng tâm**:
1. **Kiểm toán & Vá lỗi mã nguồn Backend toàn diện (Code Audit & Hardening):** Quét toàn bộ 25 Controllers, Services, Repositories, Entities và EF Core DbContext; xóa sổ triệt để các Compiler Warnings (null dereference CS8602, uninitialized properties CS8618), xử lý các cảnh báo Global Query Filter của EF Core, rà soát lỗ hổng nuốt lỗi (*Silent failures*) và vá toàn bộ lỗi phát hiện được.
2. **Migration CSDL PostgreSQL:** Đồng bộ CSDL thực tế, tạo migration cho các bảng mới (`debts`, `debt_transactions`, `subscriptions`), xuất file kịch bản SQL thuần idempotent tránh lỗi *Outdated Database Schema*.
3. **Đồng bộ tài liệu hợp đồng giao tiếp API** ([`API_CONTRACT_SPECIFICATION.md`](../../../../mobile-project/dotnet-maui-ver/implement-docs/API_CONTRACT_SPECIFICATION.md)) lên đủ 25 Controllers.
4. **Nâng cấp kịch bản kiểm thử API tự động** ([`test_api_suite.ps1`](../../../../mobile-project/dotnet-maui-ver/quick-run-scripts/test_api_suite.ps1)) bao phủ 100% endpoints mới.

---

## 2. NỘI DUNG CẦN NGƯỜI DÙNG DUYỆT (USER REVIEW REQUIRED)

> [!IMPORTANT]
> **1. Rà Soát & Vá Lỗi Mã Nguồn Backend Toàn Diện (Zero Warnings Target):**
> - Xử lý dứt điểm các cảnh báo CS8602 (Dereference of a possibly null reference) đang tồn tại trong `CoolingOffService.cs`, `NudgeEngineService.cs`, `VoiceIntentRouterServiceTests.cs`.
> - Tinh chỉnh cấu hình quan hệ EF Core trong `ApplicationDbContext.cs` để triệt tiêu cảnh báo liên kết giữa `Category` (Soft delete) với `BudgetAllocation` và `Wallet` (Soft delete) với `SavingsGoalContribution`.
> - Rà soát 25 Controllers bảo đảm 100% endpoints đều tuân thủ `ApiResponse<T>`, bắt lỗi hợp lệ, và có chú thích Swagger/OpenAPI rõ ràng.

> [!IMPORTANT]
> **2. Tạo EF Core Migration Đồng Bộ PostgreSQL:**
> - Mô hình CSDL hiện tại có các thực thể mới chưa được lưu vào Migration: `Debt` (bảng `debts`), `DebtTransaction` (bảng `debt_transactions`), và `Subscription` (bảng `subscriptions`).
> - Lệnh thực thi: `dotnet ef migrations add AddDebtsAndSubscriptionsTables` sẽ sinh mã khởi tạo bảng, quan hệ khóa ngoại (Foreign Keys) với `wallets`, `categories`, chỉ mục (Indexes) và bộ lọc xóa mềm (`IsDeleted`).
> - Đồng thời xuất file kịch bản SQL thuần idempotent (`database_migrations_full.sql`) để người dùng có thể nạp trực tiếp vào PostgreSQL qua pgAdmin hoặc Docker mà không cần cài đặt .NET SDK trên máy chủ CSDL.

---

## 3. CÁC HẠNG MỤC THAY ĐỔI CHI TIẾT (PROPOSED CHANGES)

### 3.1. Rà Soát, Kiểm Toán & Vá Lỗi Mã Nguồn Backend (Code Audit & Hardening)
- **Triệt tiêu Compiler Warnings CS8602 & CS8618:**
  * `CoolingOffService.cs`: Thêm null-check / guard clauses tại dòng 67 và 119 khi truy xuất quan hệ điều hướng `Transaction` hoặc `Wallet`.
  * `NudgeEngineService.cs`: Bổ sung null-coalescing hoặc kiểm tra null tại các dòng 197, 201, 252 khi xử lý metadata của cảnh báo tài chính.
- **Khắc phục EF Core Relationship Query Filter Warnings:**
  * Trong `ApplicationDbContext.cs`:
    - Khai báo rõ ràng query filter đồng bộ cho các thực thể phụ thuộc hoặc định nghĩa rõ ràng ràng buộc `IsRequired(false)` trên navigation properties của `BudgetAllocation` (tới `Category`) và `SavingsGoalContribution` (tới `Wallet`), triệt tiêu 2 cảnh báo Warning của EF Core khi sinh migration.
- **Kiểm toán Silent Failures & Exception Handling:**
  * Rà soát các khối `try ... catch` trong các Application Services (`DebtService`, `EmergencyFundService`, `SubscriptionService`, `ForecastingService`, `AutonomousActionEngineService`).
  * Bảo đảm mọi ngoại lệ kỹ thuật không bị nuốt âm thầm mà được log có cấu trúc và trả về `Result<T>.Failure(...)` với thông điệp rõ ràng, thân thiện.
- **Kiểm toán 25 Controllers:**
  * Đảm bảo mọi Controller kế thừa `BaseApiController`, trả về đúng phong bao `ApiResponse<T>`, kiểm tra `ModelState.IsValid`, xác thực `[Authorize]`.

---

### 3.2. Hạng Mục CSDL: EF Core Migration & SQL Script Generation
- **Thực thi lệnh sinh migration:**
  ```powershell
  dotnet ef migrations add AddDebtsAndSubscriptionsTables `
      --project "mobile-project\dotnet-maui-ver\backend\ExpenseTracker.Infrastructure\ExpenseTracker.Infrastructure.csproj" `
      --startup-project "mobile-project\dotnet-maui-ver\backend\ExpenseTracker.WebApi\ExpenseTracker.WebApi.csproj"
  ```
- **Xuất file SQL Idempotent độc lập:**
  ```powershell
  dotnet ef migrations script `
      --project "mobile-project\dotnet-maui-ver\backend\ExpenseTracker.Infrastructure\ExpenseTracker.Infrastructure.csproj" `
      --startup-project "mobile-project\dotnet-maui-ver\backend\ExpenseTracker.WebApi\ExpenseTracker.WebApi.csproj" `
      --idempotent `
      --output "mobile-project\dotnet-maui-ver\backend\database_migrations_full.sql"
  ```
- **Kiểm tra trạng thái:** Bảo đảm `dotnet ef migrations has-pending-model-changes` trả về mã 0 (No pending changes).

---

### 3.3. Hạng Mục Tài Liệu: Cập Nhật `API_CONTRACT_SPECIFICATION.md`
- Mở rộng file `mobile-project/dotnet-maui-ver/implement-docs/API_CONTRACT_SPECIFICATION.md`:
  - **Mục 4.23: `DebtsController` (`/api/debts`)**
    * `GET /api/debts`: Lấy danh sách nợ cá nhân, trạng thái quá hạn (*Overdue*), lãi suất %/năm.
    * `POST /api/debts`: Tạo khoản nợ mới (kỳ hạn sao kê, số tiền tối thiểu).
    * `GET /api/debts/{id}`: Chi tiết khoản nợ và lịch sử thanh toán.
    * `POST /api/debts/{id}/payment`: Giao tác trả nợ ACID (tự động trừ ví nguồn và giảm dư nợ).
    * `GET /api/debts/optimizer`: So sánh phương pháp Snowball vs Avalanche và ngày sạch nợ.
    * `GET /api/debts/guide`: Cẩm nang hướng dẫn trả nợ bình dân học vụ và câu hỏi trắc nghiệm tâm lý.
  - **Mục 4.24: `EmergencyFundsController` (`/api/emergency-funds`)**
    * `GET /api/emergency-funds`: Lấy tiến độ quỹ khẩn cấp 3-6-12 tháng chi tiêu thiết yếu.
    * `POST /api/emergency-funds/calculate`: Dự toán quy mô quỹ khẩn cấp dựa trên chi tiêu Needs thực tế.
    * `PUT /api/emergency-funds/lock` & `PUT /api/emergency-funds/unlock`: Khóa kỷ luật quỹ khẩn cấp chống tiêu lẹm.
  - **Mục 4.25: `SubscriptionsController` (`/api/subscriptions`)**
    * `GET /api/subscriptions`: Danh sách dịch vụ đăng ký định kỳ và hao tổn hàng năm (*AnnualCost*).
    * `POST /api/subscriptions`: Đăng ký theo dõi dịch vụ số (Netflix, Spotify, iCloud...).
    * `GET /api/subscriptions/audit`: Quét bẫy thuê bao đa tầng (Catalog, Heuristic recurring, Free trial trap).
    * `PUT /api/subscriptions/{id}/pause` & `PUT /api/subscriptions/{id}/cancel`: Quản lý trạng thái gói đăng ký.
  - **Mục 4.26: Cập nhật các Endpoints mới khác:**
    * `NetWorthController`: `GET /api/analytics/net-worth`, `POST /api/analytics/net-worth/snapshot`.
    * `ForecastingController`: `POST /api/forecasting/what-if/convert-to-goal`.
    * `AutonomousActionsController`: `POST /api/actions/process-raw`.
    * `VoiceAgentController`: `POST /api/ai-agent/voice/intent`.
    * `ReceiptsController`: `POST /api/receipts/ocr-upload`.

---

### 3.4. Hạng Mục Kịch Bản Kiểm Thử: Nâng Cấp `test_api_suite.ps1`
- Mở rộng file `mobile-project/dotnet-maui-ver/quick-run-scripts/test_api_suite.ps1` bổ sung các phân hệ kiểm thử tự động mới:
  - **Phần 16: Phân hệ Quản lý Nợ & Hạch Toán Trả Nợ ACID (Debts & Repayment):**
    * Test tạo khoản nợ thẻ tín dụng (`POST /api/debts`).
    * Test hạch toán trả nợ trừ tiền ví (`POST /api/debts/{id}/payment`).
    * Test động cơ tối ưu nợ Snowball vs Avalanche (`GET /api/debts/optimizer`).
    * Test cẩm nang hướng dẫn (`GET /api/debts/guide`).
  - **Phần 17: Phân hệ Quỹ Khẩn Cấp & Tài Sản Ròng (Emergency Fund & Net Worth):**
    * Test tính toán quy mô quỹ khẩn cấp (`POST /api/emergency-funds/calculate`).
    * Test khóa/mở khóa quỹ khẩn cấp (`PUT /api/emergency-funds/lock`, `unlock`).
    * Test báo cáo tài sản ròng và lưu snapshot (`GET /api/analytics/net-worth`, `POST snapshot`).
  - **Phần 18: Phân hệ Quét Bẫy Thuê Bao Định Kỳ (Subscription Traps):**
    * Test tạo thuê bao (`POST /api/subscriptions`).
    * Test động cơ kiểm toán bẫy đăng ký đa tầng (`GET /api/subscriptions/audit`).
    * Test tạm ngưng / hủy thuê bao (`PUT /api/subscriptions/{id}/pause`, `cancel`).
  - **Phần 19: Phân hệ Mô Phỏng Kịch Bản & Tự Hành Biến Động Số Dư:**
    * Test chuyển đổi chi tiêu giả định sang mục tiêu tích lũy (`POST /api/forecasting/what-if/convert-to-goal`).
    * Test xử lý thông báo biến động số dư thô (`POST /api/actions/process-raw`).

---

## 4. KẾ HOẠCH KIỂM CHỨNG & NGHIỆM THU (VERIFICATION PLAN)

### 4.1. Kiểm Thử Tự Động (Automated Testing)
1. **Kiểm tra biên dịch toàn bộ Solution:**
   ```powershell
   dotnet build "mobile-project\dotnet-maui-ver\backend\ExpenseTracker.slnx"
   ```
   *Yêu cầu:* Toàn bộ các project Backend (`Domain`, `Application`, `Infrastructure`, `WebApi`) biên dịch đạt **0 Warning, 0 Error**.
2. **Kiểm thử Unit Tests toàn hệ thống:**
   ```powershell
   dotnet test "mobile-project\dotnet-maui-ver\backend\tests\ExpenseTracker.Domain.UnitTests\ExpenseTracker.Domain.UnitTests.csproj"
   dotnet test "mobile-project\dotnet-maui-ver\backend\tests\ExpenseTracker.Application.UnitTests\ExpenseTracker.Application.UnitTests.csproj"
   dotnet test "mobile-project\dotnet-maui-ver\frontend\tests\ExpenseTracker.Maui.UnitTests\ExpenseTracker.Maui.UnitTests.csproj"
   ```
   *Yêu cầu:* **516 / 516 Tests PASSED 100%**.
3. **Kiểm tra trạng thái Migration CSDL:**
   ```powershell
   dotnet ef migrations has-pending-model-changes `
       --project "mobile-project\dotnet-maui-ver\backend\ExpenseTracker.Infrastructure\ExpenseTracker.Infrastructure.csproj" `
       --startup-project "mobile-project\dotnet-maui-ver\backend\ExpenseTracker.WebApi\ExpenseTracker.WebApi.csproj"
   ```
   *Yêu cầu:* Trả về "No pending model changes" (Mô hình CSDL và code đồng bộ tuyệt đối).

### 4.2. Ghi Nhận Hồ Sơ Dự Án
- Ghi nhận **Quyết Định 200** vào `project_context/decisions_and_changes_log.md`.
- Cập nhật tài liệu `walkthrough.md`.
