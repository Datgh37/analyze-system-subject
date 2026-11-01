# KẾ HOẠCH TRIỂN KHAI PLAN-60: TÁI THIẾT KẾ MASTER ROADMAP TOÀN DIỆN & CHI TIẾT GIAI ĐOẠN 1 (KIỂM TOÁN API, CHUẨN HÓA RES/REQ, XÓA MỀM SOFT DELETE & ĐỒNG BỘ SIGNALR 100%)

> **Mã kế hoạch:** `PLAN-60`  
> **Người chủ trì:** Council Orchestrator (Agent 00) cùng Hội Đồng Chuyên Gia (`Agent 04` Eric Evans - DDD, `Agent 05` Martin Kleppmann - DB, `Agent 07` Werner Vogels - Realtime, `Agent 10` Dan Abramov - Client Architecture, `Agent 11` Bruce Schneier - Security, `Agent 12` Kent Beck - QA).  
> **Căn cứ chỉ đạo:** Phiên họp Hội đồng ngày 28/10/2026 sau khi hoàn tất nộp bài môn Di động; định hướng băm thật nhỏ từng mục thành 1 giai đoạn độc lập khép kín, trau chuốt hệ thống chuẩn FinTech Enterprise và Khóa Luận Tốt Nghiệp.

---

## 1. Goal Description (Mô Tả Mục Tiêu)

1. **Bối cảnh:**
   * Dự án đã vượt qua giai đoạn gấp rút của môn Mobile (hoàn thành 11 giai đoạn ban đầu, 363 tests tự động, nộp bài thành công).
   * Dự án bước sang **Chương Mới: Hoàn thiện Toàn diện Hệ thống Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh (PFM)** phục vụ môn học Phân tích Thiết kế Hệ thống Thông tin (PTTKHTTT) và Khóa Luận Tốt Nghiệp (KLTN Cử nhân CNTT/HTTT).
   * Yêu cầu chiến lược của Người dùng:
     * **Băm thật nhỏ từng mục thành 1 giai đoạn độc lập** để đi thật sâu, triển khai hoàn chỉnh từng tính năng (Backend $\rightarrow$ Database $\rightarrow$ SignalR $\rightarrow$ Frontend $\rightarrow$ Test) xong mới sang giai đoạn tiếp theo.
     * Cấu trúc lộ trình có tính co giãn mở rộng (Extensible), sẵn sàng tiếp nhận các giai đoạn phụ/khẩn cấp.
     * Ở **Giai đoạn Mới 01**: Tập trung giải quyết dứt điểm các lỗi tồn đọng; rà soát và kiểm toán toàn bộ API Endpoints; lập bảng hợp đồng Req/Res (thành công & mọi trường hợp lỗi) để lập trình viên và QA tuân thủ tuyệt đối; loại bỏ hoàn toàn `DELETE` vật lý chuyển sang **Xóa mềm (Soft Delete)**; và khắc phục triệt để sự cố SignalR drop (đảm bảo 100% thay đổi dữ liệu đều phát tín hiệu Realtime tức thì về Mobile).

2. **Hai Trọng Tâm Cốt Lõi Của Kế Hoạch PLAN-60:**
   * **Trọng tâm 1:** Tái cấu trúc file [`MASTER_PROJECT_ROADMAP.md`](../../../mobile-project/dotnet-maui-ver/implement-docs/MASTER_PROJECT_ROADMAP.md): Đóng gói 11 giai đoạn cũ thành *Chương 1 (Archived)* và thiết lập *Chương 2 (12 Giai đoạn Băm nhỏ Mới)*.
   * **Trọng tâm 2:** Đặc tả kiến trúc kỹ thuật và kế hoạch thi công chi tiết cho **GIAI ĐOẠN MỚI 01**.

---

## 2. User Review Required (Nội Dung Cần Người Dùng Duyệt)

> [!IMPORTANT]
> **1. Quy Chuẩn Xóa Mềm (Soft Delete Standard):**
> * Toàn bộ các bảng tài chính nghiệp vụ (Transactions, Categories, Wallets, Budgets, SavingsGoals, AutoRules, RecurringIncomes) chuyển dịch 100% sang `ISoftDeletable` (`IsDeleted = true`, `DeletedAt = DateTime.UtcNow`, `DeletedBy = userId`).
> * Thay đổi HTTP Method: Loại bỏ các endpoint `DELETE /api/v1/.../{id}`. Chuyển sang `POST /api/v1/.../{id}/archive` (hoặc `PATCH /api/v1/.../{id}/status`) trả về `ApiResponse<bool>`.
> * Cấu hình EF Core Global Query Filter: `builder.Entity<T>().HasQueryFilter(e => !e.IsDeleted)`.
> 
> **2. Quy Chuẩn Hợp Đồng API Res/Req (API Contract Matrix):**
> * Chuẩn hóa Response Body nhất quán theo `ApiResponse<T>`:
>   - Thành công: `{ "isSuccess": true, "data": { ... }, "statusCode": 200, "message": null }`
>   - Thất bại: `{ "isSuccess": false, "data": null, "statusCode": 400|404|409|422, "message": "Mô tả lỗi tiếng Việt", "errors": [ "Chi tiết lỗi trường..." ] }`
> 
> **3. Quy Chuẩn SignalR Realtime Broadcast Bắt Buộc:**
> * Bất kỳ thao tác Create, Update, hoặc Archive (Soft Delete) nào thành công ở Backend **BẮT BUỘC phải gọi `IFinancialRealtimeNotifier`** để phát event WebSocket đến đúng `UserGroup`.
> * Client Mobile MAUI khi nhận event sẽ tự động cập nhật / loại bỏ item khỏi `ObservableCollection` ngay trên bộ nhớ (0ms latency), không bắt người dùng kéo refresh.

---

## 3. Kiến Trúc 12 Giai Đoạn Băm Nhỏ Mới (Micro-Stages Master Roadmap)

File `MASTER_PROJECT_ROADMAP.md` sẽ được tổ chức lại theo 2 Chương lớn:
* **CHƯƠNG 1: HOÀN THÀNH PHẠM VI MÔN DI ĐỘNG (GIAI ĐOẠN 0 ĐẾN GIAI ĐOẠN 11)** — Trạng thái: `COMPLETED & ARCHIVED` (Bảo lưu toàn bộ lịch sử 11 giai đoạn đã bảo vệ thành công).
* **CHƯƠNG 2: LỘ TRÌNH KỸ NGHỆ TOÀN DIỆN PFM ENTERPRISE & KHOÁ LUẬN TỐT NGHIỆP** gồm 12 Giai đoạn băm nhỏ:

```text
┌───────────────────────────────────────────────────────────────────────────────────────┐
│ 🔹 GIAI ĐOẠN MỚI 01: SỬA LỖI TỒN ĐỌNG, KIỂM TOÁN API RES/REQ, SOFT DELETE & SIGNALR    │
├───────────────────────────────────────────────────────────────────────────────────────┤
│ 🔹 GIAI ĐOẠN MỚI 02: BẢO MẬT SINH TRẮC HỌC (BIOMETRICS), SECURE STORAGE & QUICK LOGIN │
├───────────────────────────────────────────────────────────────────────────────────────┤
│ 🔹 GIAI ĐOẠN MỚI 03: ĐÓNG GÓI BẢN PHÁT HÀNH APK HỢP LỆ VƯỢT QUA GOOGLE PLAY PROTECT   │
├───────────────────────────────────────────────────────────────────────────────────────┤
│ 🔹 GIAI ĐOẠN MỚI 04: TRỤ CỘT NỢ — QUẢN LÝ DANH MỤC NỢ CÁ NHÂN (DEBT CATALOG)          │
├───────────────────────────────────────────────────────────────────────────────────────┤
│ 🔹 GIAI ĐOẠN MỚI 05: TRỤ CỘT NỢ — ĐỘNG CƠ TỐI ƯU TRẢ NỢ (SNOWBALL VS AVALANCHE DAG)   │
├───────────────────────────────────────────────────────────────────────────────────────┤
│ 🔹 GIAI ĐOẠN MỚI 06: QUỸ KHẨN CẤP (EMERGENCY FUND 3-6T NEEDS) & BÁO CÁO NET WORTH     │
├───────────────────────────────────────────────────────────────────────────────────────┤
│ 🔹 GIAI ĐOẠN MỚI 07: QUÉT VÀ CẢNH BÁO BẪY ĐĂNG KÝ ĐỊNH KỲ (SUBSCRIPTION TRAPS)        │
├───────────────────────────────────────────────────────────────────────────────────────┤
│ 🔹 GIAI ĐOẠN MỚI 08: NÂNG CẤP BIỂU ĐỒ BÁO CÁO ĐỘNG (HYBRID WEBVIEW CHART.JS/APEX)      │
├───────────────────────────────────────────────────────────────────────────────────────┤
│ 🔹 GIAI ĐOẠN MỚI 09: AI VISION OCR TOÀN TRÌNH — CAMERA NATIVE, LÀM NÉT & GEMINI THẬT  │
├───────────────────────────────────────────────────────────────────────────────────────┤
│ 🔹 GIAI ĐOẠN MỚI 10: TRỢ LÝ GIỌNG NÓI AI — MICROPHONE NATIVE & AGENTIC RAG / TOOLS     │
├───────────────────────────────────────────────────────────────────────────────────────┤
│ 🔹 GIAI ĐOẠN MỚI 11: MÔ PHỎNG DÒNG TIỀN BIẾN CỐ (WHAT-IF SCENARIO ANALYSIS)           │
├───────────────────────────────────────────────────────────────────────────────────────┤
│ 🔹 GIAI ĐOẠN MỚI 12: CHIẾN DỊCH TÁI THIẾT TOÀN DIỆN GIAO DIỆN (FULL UI/UX REMAKE)    │
└───────────────────────────────────────────────────────────────────────────────────────┘
```

---

## 4. Chi Tiết Kế Hoạch Triển Khai Giai Đoạn Mới 01 (Phase 01 Deep-Dive)

### 4.1. Hạng Mục 1: Kiểm Toán Toàn Diện 22 Controller API & Chuẩn Hóa Hợp Đồng Req/Res
1. **Kiểm kê & Lập Bảng Ma Trận API (API Contract Matrix):**
   * Rà soát toàn bộ 22 Controllers (`TransactionsController`, `WalletsController`, `CategoriesController`, `BudgetsController`, `SavingsGoalsController`, `AutoRulesController`, `NudgesController`, `CoolingOffController`, `RecurringIncomesController`, `AnalyticsReportsController`, `NotificationsController`, `AuthController`, `PayslipsController`, `ReceiptsController`, `SmartUtilitiesController`, `VoiceAgentController`, `AutonomousActionsController`, `DataImportController`, `SystemController`, `UsersController`).
   * Phân tích từng endpoint: Method, URL Route, Header (JWT Bearer), Request Payload DTO, Response Data DTO.
2. **Quy Chuẩn Cấu Trúc Trả Về (Response Envelope Standard):**
   * Tất cả Controller kế thừa `BaseApiController` trả về `ApiResponse<T>`:
     ```csharp
     public class ApiResponse<T>
     {
         public bool IsSuccess { get; set; }
         public int StatusCode { get; set; }
         public T? Data { get; set; }
         public string? Message { get; set; }
         public List<string>? Errors { get; set; }
         public DateTime Timestamp { get; set; } = DateTime.UtcNow;
     }
     ```
3. **Đặc Tả Chi Tiết Mọi Trường Hợp Lỗi (Error Cases Matrix):**
   * **400 Bad Request:** Lỗi dữ liệu đầu vào (Validation failure, FluentValidation). Response trả về mảng `Errors` chi tiết từng trường (ví dụ: `["Số tiền phải lớn hơn 0", "Ngày giao dịch không được vượt quá tương lai"]`).
   * **401 Unauthorized:** Thiếu Token hoặc Token hết hạn / bị thu hồi.
   * **403 Forbidden:** Người dùng cố truy cập dữ liệu của `userId` khác (IDOR Protection).
   * **404 Not Found:** Thực thể không tồn tại hoặc đã bị xóa mềm (`IsDeleted = true`).
   * **409 Conflict:** Xung đột trạng thái (ví dụ: Ví đã có giao dịch ràng buộc, Danh mục đang trùng tên).
   * **422 Unprocessable Entity:** Vi phạm luật nghiệp vụ (ví dụ: Số dư ví không đủ để trừ tiền và không cho phép thấu chi).
   * **500 Internal Server Error:** Lỗi hệ thống ngoài dự kiến, ẩn stacktrace môi trường production.
4. **Xuất Bản Tài Liệu Hợp Đồng Kỹ Thuật:**
   * Tạo tệp [`API_CONTRACT_SPECIFICATION.md`](../../../mobile-project/dotnet-maui-ver/implement-docs/API_CONTRACT_SPECIFICATION.md) ghi nhận toàn văn để khi lập trình viên Mobile viết Refit Interface hoặc ViewModel bắt buộc phải map đúng 100%.

---

### 4.2. Hạng Mục 2: Triệt Tiêu Hard Delete — Triển Khai Soft Delete (Xóa Mềm) Toàn Hệ Thống
1. **Định nghĩa Interface `ISoftDeletable` trong Domain:**
   ```csharp
   namespace ExpenseTracker.Domain.Common;
   public interface ISoftDeletable
   {
       bool IsDeleted { get; }
       DateTime? DeletedAt { get; }
       Guid? DeletedBy { get; }
       void MarkAsDeleted(Guid? deletedBy = null);
       void Restore();
   }
   ```
2. **Gắn `ISoftDeletable` vào các Thực Thể Nghiệp Vụ:**
   * `Transaction`, `Wallet`, `Category`, `Budget`, `BudgetAllocation`, `SavingsGoal`, `AutoCategorizationRule`, `RecurringIncome`, `ReceiptItem`, `FinancialNudge`, `CoolingOffTransaction`.
3. **Cấu hình EF Core Global Query Filter:**
   * Trong `ApplicationDbContext.cs`:
     ```csharp
     modelBuilder.Entity<Transaction>().HasQueryFilter(e => !e.IsDeleted);
     modelBuilder.Entity<Wallet>().HasQueryFilter(e => !e.IsDeleted);
     modelBuilder.Entity<Category>().HasQueryFilter(e => !e.IsDeleted);
     modelBuilder.Entity<Budget>().HasQueryFilter(e => !e.IsDeleted);
     modelBuilder.Entity<SavingsGoal>().HasQueryFilter(e => !e.IsDeleted);
     modelBuilder.Entity<AutoCategorizationRule>().HasQueryFilter(e => !e.IsDeleted);
     modelBuilder.Entity<RecurringIncome>().HasQueryFilter(e => !e.IsDeleted);
     ```
   * Hỗ trợ truy vấn dữ liệu đã xóa khi cần kiểm toán qua `.IgnoreQueryFilters()`.
4. **Chuyển Đổi API Endpoints Từ DELETE Sang Archive:**
   * `DELETE /api/v1/transactions/expenses/{id}` $\rightarrow$ `POST /api/v1/transactions/expenses/{id}/archive` (hoặc Soft Delete endpoint).
   * `DELETE /api/v1/transactions/incomes/{id}` $\rightarrow$ `POST /api/v1/transactions/incomes/{id}/archive`.
   * `DELETE /api/v1/wallets/{id}` $\rightarrow$ `POST /api/v1/wallets/{id}/archive`.
   * `DELETE /api/v1/categories/{id}` $\rightarrow$ `POST /api/v1/categories/{id}/archive`.
   * `DELETE /api/v1/budgets/{id}` $\rightarrow$ `POST /api/v1/budgets/{id}/archive`.
   * `DELETE /api/v1/savings-goals/{id}` $\rightarrow$ `POST /api/v1/savings-goals/{id}/archive`.
   * `DELETE /api/v1/auto-rules/{id}` $\rightarrow$ `POST /api/v1/auto-rules/{id}/archive`.
   * `DELETE /api/v1/recurring-incomes/{id}` $\rightarrow$ `POST /api/v1/recurring-incomes/{id}/archive`.

---

### 4.3. Hạng Mục 3: Khắc Phục Dứt Điểm Sự Cố SignalR Drop (100% Realtime Event Broadcast)
1. **Kiểm Tra & Chuẩn Hóa `IFinancialRealtimeNotifier`:**
   * Mở rộng interface với các sự kiện xóa mềm và cập nhật:
     ```csharp
     Task NotifyTransactionArchivedAsync(Guid userId, Guid transactionId, Guid walletId, decimal revertedAmount, CancellationToken ct = default);
     Task NotifyWalletBalanceChangedAsync(Guid userId, Guid walletId, decimal newBalance, CancellationToken ct = default);
     Task NotifyCategoryArchivedAsync(Guid userId, Guid categoryId, CancellationToken ct = default);
     Task NotifyBudgetUpdatedAsync(Guid userId, Guid budgetId, CancellationToken ct = default);
     ```
2. **Cắm Bộ Phát Tín Hiệu Vào Tất Cả Các Service Nghiệp Vụ:**
   * Trong `TransactionService.cs`: Tại hàm xóa mềm `DeleteExpenseAsync` / `DeleteIncomeAsync`:
     Ngay sau khi `CommitAsync`, **bắt buộc gọi**:
     ```csharp
     if (_realtimeNotifier != null)
     {
         await _realtimeNotifier.NotifyTransactionArchivedAsync(userId, transactionId, transaction.WalletId, transaction.Amount, ct);
         await _realtimeNotifier.NotifyWalletBalanceChangedAsync(userId, transaction.WalletId, transaction.Wallet.Balance, ct);
     }
     ```
   * Tương tự cho `WalletService`, `CategoryService`, `BudgetService`, `SavingsGoalService`.
3. **Phía Client Mobile MAUI (`ExpenseTracker.Maui`):**
   * Trong `ISignalRRealtimeService`: Bổ sung event handlers:
     `event Action<Guid, Guid, decimal>? OnTransactionArchived;`
     `event Action<Guid, decimal>? OnWalletBalanceChanged;`
   * Trong `ExpensesViewModel.cs` / `WalletsViewModel.cs`:
     Lắng nghe sự kiện `OnTransactionArchived`:
     ```csharp
     _realtimeService.OnTransactionArchived += (txId, walletId, amount) =>
     {
         MainThread.BeginInvokeOnMainThread(() =>
         {
             var itemToRemove = Transactions.FirstOrDefault(t => t.Id == txId);
             if (itemToRemove != null)
             {
                 Transactions.Remove(itemToRemove);
             }
             // Cập nhật số dư ví tương ứng ngay lập tức
             var targetWallet = Wallets.FirstOrDefault(w => w.Id == walletId);
             if (targetWallet != null)
             {
                 targetWallet.Balance += amount;
             }
         });
     };
     ```
   * **Hiệu quả đạt được:** Người dùng thực hiện thao tác xóa hoặc thiết bị thứ hai xóa giao dịch $\rightarrow$ Giao dịch trên màn hình tự động biến mất ngay tức thì (0ms), số dư ví tự nhảy số mà không cần kéo vuốt refresh trang!

---

## 5. Verification Plan (Kế Hoạch Kiểm Thử & Nghiệm Thu)

### 5.1. Automated Tests (Kiểm Thử Tự Động)
1. **Kiểm thử Soft Delete & Global Query Filter (Backend):**
   * Chạy lệnh: `dotnet test mobile-project/dotnet-maui-ver/backend/ExpenseTracker.slnx`
   * Viết bổ sung các bài test:
     - Xóa giao dịch: Kiểm tra `IsDeleted == true`, không bị xóa vật lý trong database.
     - Truy vấn danh sách: Không trả về các bản ghi có `IsDeleted == true`.
     - Số dư ví và ngân sách: Được hoàn tiền (revert) chuẩn xác 100%.
2. **Kiểm thử SignalR Dispatch (Unit Tests with Moq):**
   * Kiểm thử `TransactionServiceTests`: Verify hàm `NotifyTransactionArchivedAsync` và `NotifyWalletBalanceChangedAsync` được gọi chính xác 1 lần với đúng payload sau khi commit.
3. **Kiểm thử Client ViewModel Reactive Update (MAUI Unit Tests):**
   * Chạy lệnh: `dotnet test mobile-project/dotnet-maui-ver/frontend/tests/ExpenseTracker.Maui.UnitTests/`
   * Test case: Giả lập sự kiện SignalR bắn về $\rightarrow$ `ExpensesViewModel.Transactions` tự động giảm đi 1 phần tử mà không cần gọi API nạp lại danh sách.

### 5.2. Manual Verification (Kiểm Thử Thực Nghiệm Trên Máy Ảo)
1. Khởi chạy Backend WebApi + Docker PostgreSQL.
2. Khởi chạy Mobile MAUI trên Android Emulator (`RUN_ANDROID.bat`).
3. Thực hiện xóa 1 khoản chi tiêu trên giao diện $\rightarrow$ Quan sát trực quan:
   - Giao diện loại bỏ thẻ chi tiêu ngay lập tức.
   - Nhãn số dư ví tự động nhảy tăng tiền.
   - Kiểm tra log Serilog của WebApi: Thấy log SignalR broadcast 200 OK.
   - Kiểm tra trực tiếp trong PostgreSQL: Bản ghi vẫn còn trong bảng `transactions` nhưng có cờ `is_deleted = true` và cột `deleted_at` có mốc thời gian.

---

## 6. Các Tệp Sẽ Tạo & Chỉnh Sửa

| Hành Động | Đường Dẫn Tệp | Mục Đích |
| :---: | :--- | :--- |
| **[MODIFY]** | [`mobile-project/dotnet-maui-ver/implement-docs/MASTER_PROJECT_ROADMAP.md`](../../../mobile-project/dotnet-maui-ver/implement-docs/MASTER_PROJECT_ROADMAP.md) | Tái cấu trúc lộ trình: Đóng gói Chương 1 (Giai đoạn 0-11) và ban hành Chương 2 (12 Giai đoạn Băm nhỏ). |
| **[NEW]** | [`mobile-project/dotnet-maui-ver/implement-docs/API_CONTRACT_SPECIFICATION.md`](../../../mobile-project/dotnet-maui-ver/implement-docs/API_CONTRACT_SPECIFICATION.md) | Bảng đặc tả hợp đồng API của 22 Controllers, chuẩn hóa Req/Res và mọi kịch bản mã lỗi (400, 401, 403, 404, 409, 422, 500). |
| **[MODIFY]** | `backend/ExpenseTracker.Domain/Common/ISoftDeletable.cs` | Bổ sung Interface xóa mềm. |
| **[MODIFY]** | `backend/ExpenseTracker.Domain/Entities/*.cs` | Gắn `ISoftDeletable` vào các Entity. |
| **[MODIFY]** | `backend/ExpenseTracker.Infrastructure/Persistence/ApplicationDbContext.cs` | Cấu hình Global Query Filter `HasQueryFilter(e => !e.IsDeleted)`. |
| **[MODIFY]** | `backend/ExpenseTracker.WebApi/Controllers/*.cs` | Chuyển đổi các endpoint `HttpDelete` sang `HttpPost` Archive và chuẩn hóa ApiResponse. |
| **[MODIFY]** | `backend/ExpenseTracker.Application/Services/*.cs` | Bổ sung gọi SignalR Notifier khi xóa mềm / cập nhật. |
| **[MODIFY]** | `frontend/ExpenseTracker.Maui/Services/Realtime/SignalRRealtimeService.cs` | Bổ sung event handlers cho Archive / Soft Delete. |
| **[MODIFY]** | `frontend/ExpenseTracker.Maui/ViewModels/*.cs` | Xử lý cập nhật `ObservableCollection` tức thì 0ms khi nhận tín hiệu SignalR. |
| **[NEW]** | `project_context/plans/plans-mobile-app/2026-10-28_plan_60_master_roadmap_redesign_va_giai_doan_1_api_audit_soft_delete_signalr.md` | Snapshot lưu trữ kế hoạch bền vững. |
| **[MODIFY]** | `project_context/decisions_and_changes_log.md` | Ghi nhận Quyết Định 107 khóa phạm vi và thiết kế kỹ thuật. |

