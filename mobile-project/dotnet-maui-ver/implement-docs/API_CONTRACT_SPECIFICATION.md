# 📋 TÀI LIỆU HỢP ĐỒNG KỸ THUẬT GIAO TIẾP API (API CONTRACT SPECIFICATION)
## HỆ THỐNG QUẢN LÝ CHI TIÊU & HOẠCH ĐỊNH TÀI CHÍNH CÁ NHÂN THÔNG MINH (PFM)
### (Chuẩn RESTful API, ApiResponse Envelope, Mã Lỗi Chuẩn & Cơ Chế Soft Delete)

> **Dự Án:** Smart Personal Expense and Financial Planning Management System with AI Integration  
> **Căn cứ kế hoạch:** [`PLAN-60`](../../project_context/plans/plans-mobile-app/2026-10-28_plan_60_master_roadmap_redesign_va_giai_doan_1_api_audit_soft_delete_signalr.md), [`PLAN-70`](../../project_context/plans/plans-mobile-app/2026-11-01_plan_70_cong_kiem_toan_backend_toan_he_thong.md), [Quyết Định 200](../../project_context/decisions_and_changes_log.md).  
> **Mục Đích:** Quy chuẩn toàn bộ 25 Controllers và hơn 115 Endpoints của Backend ASP.NET Core 10 WebApi. Ràng buộc tuyệt đối lập trình viên Mobile MAUI, Web Client và QA phải tuân thủ đúng 100% các định dạng Request/Response và xử lý lỗi khi debug và thi công UI.

---

## 1. NGUYÊN TẮC THIẾT KẾ & CẤU TRÚC PHONG BAO DỮ LIỆU (RESPONSE ENVELOPE)

Tất cả các API Endpoints trong hệ thống (kế thừa từ `BaseApiController`) bắt buộc trả về cấu trúc phong bì dữ liệu thống nhất `ApiResponse<T>`:

### 1.1. Cấu Trúc Trả Về Khi Thành Công (HTTP 200 OK / 201 Created)
```json
{
  "isSuccess": true,
  "statusCode": 200,
  "data": { ... },
  "message": "Thao tác thành công",
  "errors": null,
  "timestamp": "2026-11-01T14:30:00Z"
}
```
* `isSuccess`: Luôn là `true`.
* `statusCode`: Mã HTTP (200 cho truy vấn/cập nhật, 201 cho tạo mới).
* `data`: Đối tượng dữ liệu nghiệp vụ (DTO) hoặc mảng danh sách DTOs.
* `message`: Thông báo tóm tắt bằng tiếng Việt (nếu có).
* `errors`: Luôn là `null`.

### 1.2. Cấu Trúc Trả Về Khi Thất Bại / Lỗi (HTTP 4xx / 5xx)
```json
{
  "isSuccess": false,
  "statusCode": 400,
  "data": null,
  "message": "Dữ liệu đầu vào không hợp lệ.",
  "errors": [
    "Số tiền chi tiêu phải lớn hơn 0.",
    "Ví thanh toán không tồn tại hoặc đã bị khóa."
  ],
  "timestamp": "2026-11-01T14:30:00Z"
}
```

---

## 2. MA TRẬN MÃ LỖI CHUẨN MỰC & HƯỚNG DẪN XỬ LÝ (ERROR CODES MATRIX)

| Mã HTTP | Tên Lỗi | Khi Nào Xảy Ra? | Cấu Trúc Payload `errors` | Hành Vi Phía Mobile MAUI |
| :---: | :--- | :--- | :--- | :--- |
| **`400`** | **Bad Request (Lỗi Đầu Vào)** | Dữ liệu gửi lên không vượt qua lớp kiểm tra FluentValidation (Ví dụ: số tiền âm, tên danh mục trống, ngày ở tương lai). | Mảng các chuỗi mô tả lỗi vi phạm của từng trường (`errors: ["..."]`). | Hiển thị nhãn cảnh báo đỏ ngay dưới trường nhập liệu tương ứng trên Form. |
| **`401`** | **Unauthorized (Chưa Xác Thực)** | Header không có JWT Token, hoặc Token bị hỏng / hết hạn. | `errors: ["Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại."]` | Tự động gọi Refresh Token; nếu thất bại thì điều hướng thẳng về `LoginPage`. |
| **`403`** | **Forbidden (Không Có Quyền / IDOR)** | Người dùng cố tình truy cập hoặc chỉnh sửa tài nguyên (Ví, Giao dịch) của một User khác. | `errors: ["Bạn không có quyền truy cập tài nguyên này."]` | Hiển thị thông báo Toast cảnh báo vi phạm quyền riêng tư. |
| **`404`** | **Not Found (Không Tìm Thấy)** | Thực thể không tồn tại trong CSDL hoặc **đã bị Xóa Mềm (`IsDeleted = true`)**. | `errors: ["Giao dịch không tồn tại hoặc đã bị lưu trữ."]` | Hiển thị thông báo không tìm thấy, tự động gỡ item khỏi danh sách trên màn hình. |
| **`409`** | **Conflict (Xung Đột Trạng Thái)** | Trùng lặp dữ liệu duy nhất (Ví dụ: Danh mục trùng tên, Ví mặc định bị xóa khi chưa có ví thay thế). | `errors: ["Tên danh mục này đã tồn tại trong nhóm Chi tiêu."]` | Nhắc nhở người dùng chọn tên khác hoặc đổi trạng thái. |
| **`422`** | **Unprocessable Entity (Vi Phạm Nghiệp Vụ)** | Cú pháp JSON đúng nhưng vi phạm luật tài chính (Ví dụ: Số dư ví không đủ để trừ tiền và ví không cho phép thấu chi; hoặc chi tiêu vượt quá hạn mức nghiêm ngặt). | `errors: ["Số dư ví Tiền mặt (20.000đ) không đủ để thanh toán 50.000đ."]` | Kích hoạt Dialog đề xuất: Nạp thêm tiền, chọn ví khác hoặc xem Nudge cảnh báo. |
| **`500`** | **Internal Server Error (Lỗi Hệ Thống)** | Lỗi ngoài tầm kiểm soát (Crash Database, rớt mạng OCR Provider bên thứ 3). | `errors: ["Đã có lỗi xảy ra trong quá trình xử lý. Mã theo dõi: e3a1..."]` | Hiển thị màn hình Fallback thân thiện kèm nút "Thử lại". |

---

## 3. QUY ƯỚC BẮT BUỘC VỀ XÓA MỀM (SOFT DELETE & REVERSIBLE OPERATIONS)

1. **Tuyệt đối không dùng `DELETE /api/...` vĩnh viễn:** Tất cả hành vi người dùng muốn xóa bản ghi (Giao dịch, Ví, Danh mục, Ngân sách, Nợ, Thuê bao) đều được chuyển thành **Lưu trữ / Xóa mềm (Archive / Soft Delete)** bằng phương thức **`POST /api/{resource}/{id}/archive`**.
2. **Global Query Filter:** Entity Framework Core tự động áp dụng bộ lọc `e => !e.IsDeleted`. Phía Client không cần truyền thêm cờ lọc, dữ liệu trả về mặc định chỉ chứa các bản ghi đang hoạt động.
3. **Hoàn nguyên số dư ví khi xóa mềm giao dịch:**
   - Xóa mềm giao dịch Chi: Cộng lại tiền vào ví (`wallet.Credit(amount)`).
   - Xóa mềm giao dịch Thu: Trừ lại tiền khỏi ví (`wallet.Debit(amount)`).

---

## 4. CHI TIẾT DANH MỤC 25 CONTROLLERS & HỢP ĐỒNG API TOÀN HỆ THỐNG

### 4.1. `AuthController` (Xác Thực & Quản Lý Phiên)
* **`POST /api/auth/register`**: Đăng ký tài khoản người dùng mới.
  - *Req Body:* `{ "email": "user@gmail.com", "password": "Password123@", "displayName": "Nguyễn Văn A" }`
  - *Res Data:* `{ "userId": "guid", "email": "...", "displayName": "...", "accessToken": "...", "refreshToken": "..." }`
  - *Lỗi:* 400 (Email sai định dạng, mật khẩu yếu), 409 (Email đã tồn tại).
* **`POST /api/auth/login`**: Đăng nhập bằng Email/Password.
  - *Req Body:* `{ "email": "user@gmail.com", "password": "Password123@" }`
  - *Res Data:* `{ "accessToken": "...", "refreshToken": "...", "expiresIn": 3600, "user": { ... } }`
  - *Lỗi:* 400 (Trống trường), 401 (Sai email/mật khẩu).
* **`POST /api/auth/refresh-token`**: Tái cấp Access Token qua Refresh Token Rotation.
  - *Req Body:* `{ "refreshToken": "..." }`
  - *Res Data:* `{ "accessToken": "...", "refreshToken": "..." }`
  - *Lỗi:* 401 (Token hết hạn hoặc đã bị thu hồi do tấn công Replay).
* **`POST /api/auth/verify-otp`**: Xác thực mã kích hoạt 6 số.
  - *Req Body:* `{ "email": "...", "otpCode": "123456" }`
  - *Res Data:* `true`
  - *Lỗi:* 400 (Mã sai hoặc hết hạn 5 phút).
* **`POST /api/auth/resend-otp`**: Gửi lại mã OTP qua Email.
* **`POST /api/auth/forgot-password`**: Yêu cầu đặt lại mật khẩu qua OTP.
* **`POST /api/auth/reset-password-otp`**: Xác nhận OTP và đặt lại mật khẩu mới.
* **`POST /api/auth/google`**: Đăng nhập Single Sign-On bằng Google ID Token.
* **`POST /api/auth/logout`**: Thu hồi Refresh Token và hủy phiên đăng nhập.

---

### 4.2. `TransactionsController` (Sổ Giao Dịch Thu - Chi)
* **`GET /api/transactions`**: Lấy danh sách giao dịch có phân trang và lọc theo ngày/ví/danh mục.
  - *Query Params:* `page=1&pageSize=20&walletId=guid&categoryId=guid&fromDate=...&toDate=...`
  - *Res Data:* `PagedResult<TransactionDto>` (Tự động lọc bỏ các bản ghi `is_deleted = true`).
* **`GET /api/transactions/{id:guid}`**: Lấy chi tiết 1 giao dịch.
* **`POST /api/transactions/expenses`**: Thêm mới một khoản chi tiêu.
  - *Req Body:* `{ "walletId": "guid", "categoryId": "guid", "amount": 50000, "transactionDate": "2026-11-01T10:00:00Z", "note": "Ăn trưa", "receiptItems": [...] }`
  - *SignalR:* Broadcast `OnTransactionCreated` và `OnWalletBalanceChanged`.
* **`PUT /api/transactions/expenses/{id:guid}`**: Chỉnh sửa khoản chi tiêu (Hoàn tiền cũ, trừ tiền mới theo ACID).
* **`POST /api/transactions/expenses/{id:guid}/archive`**: Xóa mềm khoản chi tiêu (Hoàn tiền về ví).
  - *SignalR:* Broadcast `OnTransactionArchived` và `OnWalletBalanceChanged`.
* **`GET /api/transactions/expenses/{id:guid}/details`**: Lấy chi tiết kèm bóc tách từng món đồ hóa đơn OCR.
* **`GET /api/transactions/expenses/monthly-total`**: Tổng chi tiêu trong tháng hiện tại.
* **`POST /api/transactions/incomes`**: Thêm mới khoản thu nhập.
* **`PUT /api/transactions/incomes/{id:guid}`**: Chỉnh sửa khoản thu nhập.
* **`POST /api/transactions/incomes/{id:guid}/archive`**: Xóa mềm khoản thu nhập (Trừ tiền khỏi ví).
* **`GET /api/transactions/incomes/monthly-total`**: Tổng thu nhập trong tháng hiện tại.

---

### 4.3. `WalletsController` (Quản Lý Đa Ví & Hạch Toán ACID)
* **`GET /api/wallets`**: Lấy danh sách toàn bộ ví chưa bị xóa mềm của người dùng.
* **`GET /api/wallets/{id:guid}`**: Lấy thông tin chi tiết một ví.
* **`POST /api/wallets`**: Tạo ví mới (Tiền mặt, Ngân hàng, Thẻ tín dụng, Ví điện tử).
* **`PUT /api/wallets/{id:guid}`**: Cập nhật thông tin ví (Tên ví, Màu sắc, Biểu tượng).
* **`POST /api/wallets/{id:guid}/archive`**: Xóa mềm ví (Không thể xóa ví mặc định duy nhất).
* **`POST /api/wallets/transfer`**: Chuyển tiền nội bộ giữa 2 ví (Pessimistic Lock, ghi sổ kép).
  - *Req Body:* `{ "sourceWalletId": "guid", "destinationWalletId": "guid", "amount": 200000, "fee": 0, "note": "Rút tiền ATM" }`
  - *SignalR:* Broadcast `OnWalletBalanceChanged` cho cả 2 ví.
* **`POST /api/wallets/{id:guid}/adjust`**: Điều chỉnh số dư thực tế và ghi nhận chênh lệch.
* **`GET /api/wallets/{id:guid}/adjustments`**: Lịch sử các lần cân đối số dư ví.
* **`POST /api/wallets/scan-balance`**: Quét số dư tự động từ ảnh chụp ứng dụng ngân hàng.

---

### 4.4. `CategoriesController` (Danh Mục Thu Chi 2 Cấp & Ánh Xạ Chuẩn)
* **`GET /api/categories`**: Lấy danh sách danh mục theo loại (1 = Thu nhập, 2 = Chi tiêu).
* **`GET /api/categories/{id:guid}`**: Lấy chi tiết danh mục.
* **`POST /api/categories`**: Tạo danh mục mới (hỗ trợ phân cấp Cha - Con và gán chuẩn 50/30/20 hoặc Kakeibo).
* **`PUT /api/categories/{id:guid}`**: Cập nhật thông tin danh mục.
* **`POST /api/categories/{id:guid}/archive`**: Xóa mềm danh mục.

---

### 4.5. `BudgetsController` (Ngân Sách Chu Kỳ & Động Cơ DSA)
* **`GET /api/budgets/active`**: Lấy ngân sách chu kỳ hiện tại đang hoạt động.
* **`GET /api/budgets`**: Lịch sử các chu kỳ ngân sách đã qua.
* **`GET /api/budgets/{id:guid}`**: Lấy chi tiết một ngân sách cụ thể.
* **`POST /api/budgets`**: Thiết lập ngân sách mới và phân bổ hạn mức cho từng danh mục.
* **`PUT /api/budgets/{id:guid}`**: Cập nhật ngân sách và các hạn mức phân bổ.
* **`POST /api/budgets/{id:guid}/rebalance`**: Tái phân bổ ngân sách giữa các danh mục (Budget Rebalancing).
* **`POST /api/budgets/{id:guid}/archive`**: Xóa mềm chu kỳ ngân sách.
* **`GET /api/budgets/active/dsa`**: Tính toán Hạn mức chi tiêu mỗi ngày (Daily Safe Amount - DSA) thích ứng.
* **`GET /api/budgets/active/velocity`**: Giám sát Tốc độ tiêu tiền (Spending Velocity) cảnh báo vượt hạn mức.
* **`GET /api/budgets/active/progress`**: Tiến độ tiêu dùng trực quan theo danh mục.

---

### 4.6. `ReceiptsController` (AI Multimodal Vision OCR Hóa Đơn)
* **`POST /api/receipts/parse`**: Bóc tách hóa đơn mua sắm đa phương thức (Ảnh/Camera/PDF).
  - *Req Form-Data:* `file` (Image hoặc PDF), `walletId` (Guid tùy chọn).
  - *Res Data:* `OcrReceiptResultDto` gồm Tổng tiền, Tên người bán, Ngày mua, Danh mục gợi ý, Danh sách món hàng (`items`).
  - *SignalR:* Broadcast `OnOcrProgress` thời gian thực (0% -> 50% -> 100%).
* **`POST /api/receipts/confirm`**: Xác nhận lưu giao dịch chi tiêu sau khi người dùng đối soát hóa đơn OCR.

---

### 4.7. `PayslipsController` (AI Vision OCR Bóc Tách Bảng Lương)
* **`POST /api/payslips/parse`**: Bóc tách phiếu lương tự động từ ảnh chụp hoặc PDF.
  - *Res Data:* Lương cơ bản, Thưởng, Trợ cấp, Khấu trừ BHXH/BHYT, Thuế TNCN và Thực lĩnh (Net Income).
* **`POST /api/payslips/confirm`**: Xác nhận ghi nhận thu nhập lương định kỳ vào ví.
* **`GET /api/payslips/transaction/{transactionId:guid}`**: Lấy thông tin phiếu lương gắn với giao dịch thu nhập.

---

### 4.8. `NudgesController` (Kinh Tế Hành Vi Richard Thaler & Cú Hích Tài Chính)
* **`GET /api/nudges`**: Lấy danh sách toàn bộ các cú hích tài chính.
* **`GET /api/nudges/active`**: Lấy danh sách các Nudge đang hoạt động (Vượt DSA 85%, Lệch chuẩn Kakeibo, Chi tiêu bốc đồng).
* **`GET /api/nudges/{id:guid}/rationale`**: Giải trình lý do tài chính bằng ngôn ngữ tự nhiên (Explainable AI - XAI).
* **`GET /api/nudges/{id:guid}/xai`**: Cung cấp chi tiết phân tích toán học và căn cứ dữ liệu của cú hích.
* **`GET /api/nudges/rebalance-proposal`**: Đề xuất hoán đổi hạn mức ngân sách thông minh khi một nhóm bị thâm hụt.
* **`POST /api/nudges/{id:guid}/approve`**: Chấp thuận cú hích (Ví dụ: Đồng ý tái cân đối ngân sách).
* **`POST /api/nudges/{id:guid}/dismiss`**: Bỏ qua cú hích tài chính.

---

### 4.9. `CoolingOffController` (Khoảng Trễ Tâm Lý Chống Mua Sắm Bốc Đồng)
* **`POST /api/cooling-off/evaluate`**: Đánh giá chi tiêu lớn bất thường dựa trên quy tắc 24h/48h.
* **`POST /api/cooling-off`**: Kích hoạt Khoảng trễ tâm lý cho một khoản chi tiêu dự kiến.
* **`GET /api/cooling-off/pending`**: Danh sách các khoản chi tiêu đang bị trì hoãn chờ hết thời gian thử thách.
* **`POST /api/cooling-off/{id:guid}/confirm`**: Xác nhận chi tiêu sau khi đã hết thời gian làm nguội cảm xúc.
* **`POST /api/cooling-off/{id:guid}/cancel`**: Hủy bỏ khoản chi (Tiết kiệm thành công).
* **`POST /api/cooling-off/{id:guid}/bypass`**: Bỏ qua cảnh báo khoảng trễ (Ghi nhận lý do tự vượt rào).

---

### 4.10. `AnalyticsReportsController` (Báo Cáo Tài Chính Đa Chiều & Xuất File)
* **`GET /api/analytics-reports/income-expense`**: Thống kê Thu - Chi theo chu kỳ (MoM, YoY).
* **`GET /api/analytics-reports/cashflow`**: Báo cáo Dòng tiền thuần thác nước (Waterfall).
* **`GET /api/analytics-reports/expense-structure`**: Phân rã tỷ trọng 50/30/20 và Kakeibo.
* **`GET /api/analytics-reports/outliers`**: Radar phát hiện chi tiêu đột biến bằng phương pháp Z-Score.
* **`GET /api/analytics-reports/export-excel`**: Xuất báo cáo điều hành Excel qua ClosedXML.

---

### 4.11. `VoiceAgentController` (Trợ Lý Giọng Nói Xử Lý Giao Dịch Tự Nhiên)
* **`POST /api/voice-agent/parse-intent`**: Phân tích câu lệnh giọng nói tiếng Việt $\rightarrow$ Trích xuất Entity (Số tiền, Danh mục, Ví, Ngày) và Route điều hướng.
  - *Req Body:* `{ "rawTranscript": "Trưa nay ăn phở Thìn hết 65 nghìn trả bằng tiền mặt" }`
  - *Res Data:* `{ "intent": "RecordExpense", "confidence": 0.96, "entities": { "amount": 65000, "categoryName": "Ăn uống", "walletName": "Tiền mặt" }, "suggestedRoute": "/transactions/create" }`

---

### 4.12. `AutonomousActionsController` (Hộp Thư Hành Động Tự Hành & Giao Thức Jev)
* **`GET /api/actions/inbox`**: Lấy danh sách hành động tài chính tự hành chờ người dùng duyệt (Ngưỡng xác suất Jev $\ge 0.85$).
* **`POST /api/actions/from-notification`** (Alias: `POST /api/actions/process-raw`): Tiếp nhận chuỗi biến động số dư ngân hàng qua SMS / Android Push Notification và sinh đề xuất hành động.
* **`POST /api/actions/quick-commit`**: Phê duyệt 1-chạm hành động tự hành lưu vào CSDL.
* **`POST /api/actions/{actionId}/dismiss`**: Từ chối hành động tự hành.

---

### 4.13. `DebtsController` (Quản Lý Danh Mục Nợ & Tối Ưu Hóa Trả Nợ)
* **`GET /api/debts`**: Lấy danh sách toàn bộ các khoản nợ cá nhân kèm tỷ lệ hoàn thành (Progress %).
* **`GET /api/debts/summary`**: Tổng hợp chỉ số nợ tổng quan: Tổng dư nợ hiện tại, tổng gốc ban đầu, tổng tiền đã trả, tổng trả tối thiểu hàng tháng, số khoản active/paid off.
* **`GET /api/debts/{id:guid}`**: Lấy chi tiết khoản nợ kèm lịch sử các lần trả nợ (`debt_transactions`).
* **`POST /api/debts`**: Tạo mới khoản nợ cá nhân (`CreateDebtDto`).
* **`PUT /api/debts/{id:guid}`**: Cập nhật thông tin khoản nợ (`UpdateDebtDto`).
* **`POST /api/debts/{id:guid}/payments`**: Ghi nhận thanh toán trả nợ $\rightarrow$ Trừ tiền ví nguồn (`wallet.Debit`), giảm dư nợ gốc, tự động tất toán `PaidOff` khi nợ về 0 và phát SignalR realtime.
* **`POST /api/debts/{id:guid}/archive`**: Xóa mềm khoản nợ (`ISoftDeletable`), bảo toàn lịch sử kiểm toán tài chính.
* **`GET /api/debts/optimization/guide`**: Cung cấp cẩm nang hướng dẫn bình dân hóa tài chính, giải thích chi tiết Snowball vs Avalanche và câu hỏi trắc nghiệm tự định hướng.
* **`GET /api/debts/optimization/safe-budget`**: Gợi ý các mức ngân sách trả nợ thêm an toàn (Thận trọng 10%, Khuyên dùng 30%, Quyết liệt 50%) dựa trên thặng dư thu chi thực tế của người dùng.
* **`POST /api/debts/optimization/compare`**: Mô phỏng và so sánh song song hai chiến lược Snowball vs Avalanche kèm tính toán số tiền lãi tiết kiệm, số tháng rút ngắn, cảnh báo bẫy nợ Negative Amortization và sinh lời khuyên đời thường.
* **`POST /api/debts/optimization/simulate?strategy={snowball|avalanche}`**: Mô phỏng chi tiết dòng thời gian theo tháng (Timeline) của một chiến lược cụ thể.

---

### 4.14. `EmergencyFundsController` (Quỹ Dự Phòng Khẩn Cấp Chuẩn Chuyên Gia)
* **`GET /api/emergency-funds/recommendation`**: Tính toán đề xuất số tháng dự phòng lý tưởng (3, 6, 9 hoặc 12 tháng) dựa trên độ ổn định công việc và người phụ thuộc.
* **`GET /api/emergency-funds/status`**: Lấy trạng thái hiện tại của Quỹ dự phòng: Mục tiêu, số tiền hiện có, tỷ lệ phần trăm tích lũy và số tháng chi tiêu thiết yếu đã bảo đảm.
* **`POST /api/emergency-funds/setup`**: Thiết lập hoặc điều chỉnh mục tiêu quỹ khẩn cấp.
* **`POST /api/emergency-funds/deposit`**: Nạp tiền tích lũy vào Quỹ khẩn cấp từ ví chỉ định.
* **`POST /api/emergency-funds/withdraw`**: Rút tiền từ Quỹ khẩn cấp khi gặp sự cố đột xuất (kèm ghi nhận lý do sử dụng).

---

### 4.15. `SubscriptionsController` (Quét Bẫy Thuê Bao Định Kỳ & Chống Rò Rỉ Dòng Tiền)
* **`GET /api/subscriptions`**: Lấy danh sách các dịch vụ thuê bao định kỳ (Netflix, Spotify, iCloud, Gym, YouTube Premium).
* **`GET /api/subscriptions/{id:guid}`**: Chi tiết một dịch vụ thuê bao.
* **`POST /api/subscriptions`**: Thêm mới dịch vụ thuê bao theo dõi.
* **`PUT /api/subscriptions/{id:guid}`**: Cập nhật thông tin chu kỳ, số tiền hoặc ngày gia hạn.
* **`POST /api/subscriptions/{id:guid}/archive`**: Hủy theo dõi / Xóa mềm thuê bao.
* **`GET /api/subscriptions/audit`**: Báo cáo kiểm toán bẫy thuê bao: Tổng chi phí theo năm, phát hiện các khoản đăng ký trùng lặp hoặc ít sử dụng.
* **`POST /api/subscriptions/convert`**: Chuyển đổi giao dịch định kỳ thành thuê bao theo dõi.
* **`GET /api/subscriptions/upcoming`**: Danh sách các khoản thuê bao sắp đến hạn thanh toán trong 7-30 ngày tới.

---

### 4.16. `NetWorthController` (Theo Dõi Tổng Tài Sản Ròng & Snapshot Lịch Sử)
* **`GET /api/net-worth/current`**: Tính toán giá trị Tài Sản Ròng tức thời (Net Worth = Tổng Tài Sản Các Ví - Tổng Dư Nợ Hiện Hữu).
* **`GET /api/net-worth/history`**: Lấy chuỗi lịch sử biến động tài sản ròng theo tháng/năm.
* **`POST /api/net-worth/snapshot`**: Chụp và lưu trữ một mốc Snapshot tài sản ròng định kỳ.

---

### 4.17. `ForecastingController` (Dự Báo Tài Chính & Phân Tích Kịch Bản What-If)
* **`GET /api/forecasting/expenditure`**: Dự báo xu hướng chi tiêu tương lai dựa trên mô hình chuỗi thời gian.
* **`GET /api/forecasting/liquidity-depletion`**: Cảnh báo ngày cạn kiệt thanh khoản tiền mặt nếu giữ nguyên nhịp tiêu dùng.
* **`GET /api/forecasting/category-risks`**: Nhận diện các danh mục có nguy cơ vỡ ngân sách cao nhất.
* **`POST /api/forecasting/what-if`**: Mô phỏng kịch bản What-If: "Điều gì xảy ra nếu tôi mua xe máy 40 triệu hoặc đi du lịch 15 triệu?". Phân tích tác động tức thì tới dòng tiền, ngân sách và quỹ dự phòng.
* **`POST /api/forecasting/what-if/convert-to-goal`**: Chuyển đổi trực tiếp kế hoạch mua sắm từ kịch bản What-If thành Mục tiêu tích lũy có lộ trình khả thi.

---

### 4.18. `SavingsGoalsController` (Mục Tiêu Tiết Kiệm & 6 Chiếc Hũ Jars)
* **`GET /api/savings-goals`**: Lấy danh sách các mục tiêu tích lũy.
* **`GET /api/savings-goals/{id:guid}`**: Chi tiết mục tiêu tích lũy.
* **`POST /api/savings-goals`**: Tạo mới mục tiêu tiết kiệm.
* **`PUT /api/savings-goals/{id:guid}`**: Chỉnh sửa mục tiêu.
* **`POST /api/savings-goals/{id:guid}/deposit`**: Nạp tiền vào mục tiêu tiết kiệm từ ví.
* **`POST /api/savings-goals/{id:guid}/withdraw`**: Rút tiền khỏi mục tiêu.
* **`GET /api/savings-goals/{id:guid}/contributions`**: Lịch sử các lần đóng góp tích lũy.
* **`GET /api/savings-goals/{id:guid}/feasibility`**: Đánh giá tính khả thi và ngày hoàn thành dự kiến.
* **`GET /api/savings-goals/jars-summary`**: Tổng hợp mô hình 6 Chiếc Hũ phân bổ tài sản.
* **`POST /api/savings-goals/{id:guid}/auto-deposit`**: Cài đặt quy tắc trích tiền tự động định kỳ vào mục tiêu.
* **`POST /api/savings-goals/{id:guid}/archive`**: Xóa mềm mục tiêu tiết kiệm.

---

### 4.19. `RecurringIncomesController` (Quản Lý Nguồn Thu Nhập Định Kỳ)
* **`GET /api/recurring-incomes`**: Danh sách các nguồn thu nhập cố định (Lương, Cổ tức, Cho thuê nhà).
* **`POST /api/recurring-incomes`**: Thêm mới nguồn thu nhập định kỳ.
* **`PUT /api/recurring-incomes/{id:guid}`**: Chỉnh sửa thông tin nguồn thu.
* **`PATCH /api/recurring-incomes/{id:guid}/toggle`**: Bật/Tắt trạng thái tự động ghi nhận thu nhập.
* **`POST /api/recurring-incomes/{id:guid}/archive`**: Xóa mềm nguồn thu.
* **`POST /api/recurring-incomes/process-due`**: Quét và tự động ghi sổ các khoản thu nhập đã đến hạn.

---

### 4.20. `AutoRulesController` (Quy Tắc Tự Động Phân Loại Giao Dịch)
* **`GET /api/auto-rules`**: Danh sách quy tắc tự động hóa người dùng thiết lập.
* **`POST /api/auto-rules`**: Tạo quy tắc mới (Nếu từ khóa chứa "Grab" $\rightarrow$ Gán Danh mục "Đi lại").
* **`PUT /api/auto-rules/{id:guid}`**: Sửa đổi điều kiện và hành động của quy tắc.
* **`POST /api/auto-rules/{id:guid}/archive`**: Xóa mềm quy tắc.
* **`GET /api/auto-rules/match`**: Kiểm tra từ khóa xem có khớp quy tắc nào để áp dụng tự động.

---

### 4.21. `NotificationsController` (Thông Báo Nội Ứng Dụng & Push Notification)
* **`POST /api/notifications/push-token`**: Đăng ký FCM / APNs Device Token của thiết bị di động.
* **`GET /api/notifications`**: Lấy danh sách thông báo của người dùng.
* **`PATCH /api/notifications/{id:guid}/read`**: Đánh dấu 1 thông báo đã đọc.
* **`PATCH /api/notifications/read-all`**: Đánh dấu đã đọc tất cả thông báo.
* **`POST /api/notifications/test-push`**: Bắn thử nghiệm thông báo đẩy đến thiết bị.

---

### 4.22. `DataImportController` (Nhập Dữ Liệu Hàng Loạt Từ Bảng Tính & Sao Kê)
* **`POST /api/data-import/preview-spreadsheet`**: Tải file Excel/CSV sao kê ngân hàng lên và trích xuất bảng xem trước.
* **`POST /api/data-import/preview-text`**: Xem trước kết quả phân tích khối văn bản sao kê tự do.
* **`POST /api/data-import/commit-bulk`**: Xác nhận nhập hàng loạt danh sách giao dịch đã đối soát vào CSDL.

---

### 4.23. `SmartUtilitiesController` (Tiện Ích Tài Chính Thông Minh & Mã VietQR)
* **`GET /api/smart-utilities/subscriptions`**: Tóm tắt tiện ích thuê bao tích hợp.
* **`POST /api/smart-utilities/vietqr`**: Tạo mã VietQR chuẩn NAPAS 247 để chuyển khoản nhanh.
* **`POST /api/smart-utilities/voice-parse`**: Phân tích cú pháp văn bản nhanh từ giọng nói.

---

### 4.24. `SystemController` (Quản Trị Hệ Thống, Giám Sát CSDL & Dữ Liệu Mẫu)
* **`GET /api/system/database-status`**: Kiểm tra trạng thái kết nối CSDL PostgreSQL/SQLite và số lượng bảng.
* **`POST /api/system/seed-admin`**: Khởi tạo tài khoản quản trị hệ thống ban đầu.
* **`POST /api/system/reset-database`**: Xóa và thiết lập lại CSDL sạch (Chỉ mở trong môi trường Development/Test).
* **`POST /api/system/seed-demo-story`**: Nạp gói dữ liệu mẫu câu chuyện tài chính thực tế phục vụ Demo và nghiệm thu.

---

### 4.25. `UsersController` (Hồ Sơ Cá Nhân & Đổi Mật Khẩu)
* **`GET /api/users/profile`**: Lấy thông tin hồ sơ cá nhân của người dùng hiện tại.
* **`PUT /api/users/profile`**: Cập nhật họ tên, ảnh đại diện, tiền tệ ưa thích.
* **`PUT /api/users/change-password`**: Đổi mật khẩu đăng nhập với kiểm tra mật khẩu hiện tại.

---

## 5. CAM KẾT ĐỒNG BỘ SIGNALR WEBSOCKET (ZERO-REFRESH REALTIME)

Mọi thay đổi dữ liệu từ Backend đều phát tín hiệu qua WebSocket Hub `/hubs/financial` với các kênh sự kiện sau:

| Tên Sự Kiện SignalR | Payload Kèm Theo | Phản Ứng Phía Client Mobile MAUI |
| :--- | :--- | :--- |
| **`TransactionCreated`** | `TransactionDto` | Chèn ngay vào đầu danh sách `Transactions`, cập nhật nhãn số dư ví. |
| **`TransactionArchived`** | `{ transactionId, walletId, revertedAmount }` | Xóa item khỏi `Transactions` với hiệu ứng mờ 0ms, tự động cộng/trừ số dư ví. |
| **`WalletBalanceChanged`**| `{ walletId, newBalance }` | Nhảy số dư ví trên Card Dashboard ngay tức thì. |
| **`BudgetUpdated`**       | `BudgetDto` | Cập nhật thanh tiến độ % ngân sách và chỉ số DSA. |
| **`NudgeGenerated`**      | `FinancialNudgeDto` | Rung phản hồi Haptics và hiển thị chuông thông báo. |
| **`OcrProgress`**         | `{ stepText, percent }` | Cập nhật thanh tiến trình bóc tách hóa đơn theo thời gian thực. |
| **`DebtPaymentRecorded`** | `{ debtId, newRemainingAmount, isPaidOff }` | Cập nhật tiến độ nợ, hiển thị chúc mừng nếu tất toán thành công. |
| **`EmergencyFundUpdated`**| `{ currentAmount, targetAmount, progressPercentage }` | Đồng bộ vòng cung tích lũy quỹ khẩn cấp. |
| **`SubscriptionCreated`** | `SubscriptionDto` | Bổ sung dịch vụ vào danh sách theo dõi thuê bao định kỳ. |
| **`ActionInboxCreated`**  | `AutonomousActionDto` | Hiện chấm đỏ huy hiệu (Badge) trên Hộp thư hành động tự hành. |

---

*Tài liệu này là chuẩn mực giao tiếp duy nhất giữa Backend và Client. Mọi thay đổi về DTOs hoặc Endpoints phải cập nhật vào tệp này trước khi triển khai mã nguồn.*
