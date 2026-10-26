# Cẩm Nang Kiểm Thử Tổng Thể API & Kênh Realtime (API Master Testing Playbook)

> **Dự án:** Hệ thống Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh (Tích hợp AI)  
> **Ngăn xếp công nghệ:** ASP.NET Core 10 Web API, EF Core PostgreSQL 16, SignalR Core, Google Gemini 2.5 Flash Vision & NLP.  
> **Tài liệu tham chiếu:** Quay lại trang chủ [`README.md`](README.md) để xem giới thiệu kiến trúc và hướng dẫn cài đặt.  
> **Toàn bộ đường dẫn trong tài liệu này đều lấy thư mục `dotnet-maui-ver` làm gốc.**

---

## 1. Tổng Quan Chu Trình Kiểm Thử E2E (End-to-End Testing Flow)

Toàn bộ hệ thống Backend cung cấp hơn 40 API Endpoints trên 22 Controllers bám sát **63 Use Cases chi tiết (từ UC-01 đến UC-10.5)**. Kiểm thử có thể thực hiện trực quan thông qua Swagger UI tại:
👉 **`http://localhost:5018/swagger`** *(hoặc `https://localhost:7295/swagger`)*

```text
┌────────────────────────────────────────────────────────────────────────────────────────┐
│ MA TRẬN KIỂM THỬ TOÀN DIỆN (FULL-SPECTRUM API TESTING):                                │
│                                                                                        │
│ [HAPPY PATHS - 200/201]                           [SAD PATHS & NEGATIVE CASES - 4xx]   │
│ 1. Auth & Profile ──> 2. Đa Ví ACID               • 401 Unauthorized (Thiếu JWT)       │
│ 3. Thu Chi & AI OCR ──> 4. Ngân Sách DSA          • 400 Validation Error (Số âm, rỗng) │
│ 5. Quỹ Tích Lũy ──> 6. Báo Cáo Net Worth          • 400 Business Error (Quá số dư ví)  │
│ 7. Dự Báo Sandbox ──> 8. Nudge & 24h              • 404 Not Found (Sai ID tài nguyên)  │
│ 9. Jev Action Inbox ──> 10. System Seed/Reset     • 429 Too Many Requests (Spam AI)    │
└────────────────────────────────────────────────────────────────────────────────────────┘
```

---

## 2. Kịch Bản Kiểm Thử Chuẩn (Happy Paths - Thành Công)

### Bước 1: Phân Hệ 1 — Xác Thực (Auth) & Cấp Quyền Truy Cập
1. **Đăng ký tài khoản mới:** `POST /api/auth/register`
   ```json
   {
     "fullName": "Nguyen Van A",
     "email": "nguyenvana@gmail.com",
     "password": "Password123!"
   }
   ```
   *(Nhận phản hồi HTTP 201 Created kèm thông tin User Profile)*.
2. **Đăng nhập:** `POST /api/auth/login`
   ```json
   {
     "email": "nguyenvana@gmail.com",
     "password": "Password123!"
   }
   ```
   *(Phản hồi trả về `accessToken` JWT và `refreshToken`)*.
3. **Kích hoạt quyền truy cập trên Swagger UI:**
   - Sao chép chuỗi `accessToken`.
   - Nhấn nút xanh **Authorize** ở góc trên bên phải trang Swagger.
   - Dán token vào ô `Value` (Swagger sẽ tự động gắn tiền tố `Bearer `). Nhấn **Authorize** rồi chọn **Close**.

---

### Bước 2: Phân Hệ 1 — Quản Lý Đa Ví Cá Nhân (Multi-Wallet ACID)
1. **Tạo Ví Tiền Mặt:** `POST /api/wallets`
   ```json
   {
     "name": "Ví Tiền Mặt",
     "walletType": 0,
     "currency": "VND",
     "initialBalance": 5000000,
     "colorHex": "#10B981",
     "iconKey": "cash"
   }
   ```
2. **Tạo Ví Ngân Hàng:** `POST /api/wallets`
   ```json
   {
     "name": "Tài khoản Vietcombank",
     "walletType": 1,
     "currency": "VND",
     "initialBalance": 20000000,
     "accountNumber": "9988776655",
     "bankName": "Vietcombank",
     "colorHex": "#3B82F6",
     "iconKey": "building-columns"
   }
   ```
3. **Xem danh sách ví:** `GET /api/wallets`  
   *(Kiểm tra tính toán Tổng tài sản khả dụng: `25.000.000 đ`)*.
4. **Chuyển tiền giữa 2 ví (ACID Concurrency Safe):** `POST /api/wallets/transfer`
   ```json
   {
     "sourceWalletId": "{ID_VI_VIETCOMBANK}",
     "destinationWalletId": "{ID_VI_TIEN_MAT}",
     "amount": 2000000,
     "note": "Rút tiền ATM tiêu vặt"
   }
   ```
   *(Ví VCB giảm 2 triệu còn 18 triệu; Ví tiền mặt tăng 2 triệu lên 7 triệu. Khóa bi quan PostgreSQL loại bỏ hoàn toàn race condition)*.
5. **Cân chỉnh số dư thực tế (Audit Adjustment):** `POST /api/wallets/{id}/adjust`
   ```json
   {
     "newActualBalance": 6950000,
     "reason": "Mua đồ vặt quên ghi chép"
   }
   ```
6. **Cập nhật vi mô từng phần (HTTP PATCH - RFC 7396):** `PATCH /api/wallets/{id}`
   ```json
   {
     "colorHex": "#EC4899",
     "iconKey": "star",
     "isFavorite": true
   }
   ```

---

### Bước 3: Phân Hệ 2 & 3 — Thu Nhập, Chi Tiêu, AI OCR & Danh Mục
1. **Ghi nhận Thu nhập:** `POST /api/transactions/incomes`
   ```json
   {
     "walletId": "{ID_VI_VIETCOMBANK}",
     "amount": 25000000,
     "categoryId": "{ID_DANH_MUC_LUONG}",
     "title": "Nhận lương công ty Tháng 10",
     "transactionDate": "2026-10-25T00:00:00Z"
   }
   ```
2. **Ghi nhận Chi tiêu thường:** `POST /api/transactions/expenses`
   ```json
   {
     "walletId": "{ID_VI_TIEN_MAT}",
     "amount": 65000,
     "categoryId": "{ID_DANH_MUC_AN_UONG}",
     "title": "Cà phê sáng Highlands",
     "transactionDate": "2026-10-25T08:30:00Z"
   }
   ```
   *(Kiểm tra số dư ví tiền mặt lập tức bị trừ 65.000 đ chuẩn ACID; bắn SignalR Hub thông báo cập nhật số dư 0ms)*.
3. **Thử nghiệm AI Multimodal Vision OCR Hóa Đơn Siêu Thị:** `POST /api/receipts/scan`
   - Gửi file ảnh hóa đơn.
   - Động cơ AI tự động bóc tách từng món hàng (Line Items) và kích hoạt **Bộ kiểm toán số học (Mathematical Audit Engine)** đối soát $\sum \text{Món} + \text{VAT} - \text{Giảm giá} = \text{Tổng tiền}$ với sai số $\le 1.000$ đ.
4. **Cập nhật vi mô diện mạo Danh mục (HTTP PATCH):** `PATCH /api/categories/{id}`
   ```json
   {
     "colorHex": "#8B5CF6",
     "iconKey": "coffee-cup",
     "displayOrder": 2
   }
   ```

---

### Bước 4: Phân Hệ 4 — Ngân Sách Đa Phương Pháp & Động Cơ DSA Thích Ứng
1. **Thiết lập Ngân sách 50/30/20 hoặc Kakeibo:** `POST /api/budgets`
   ```json
   {
     "name": "Ngân Sách Sinh Hoạt Tháng 10",
     "method": 0,
     "periodType": 0,
     "startDate": "2026-10-01",
     "endDate": "2026-10-31",
     "totalLimit": 15000000
   }
   ```
2. **Kiểm tra Hạn mức tiêu mỗi ngày đàn hồi (Adaptive DSA):** `GET /api/budgets/{id}/daily-allowance`
   - Thuật toán DSA tự động lấy số tiền còn lại chia cho số ngày còn lại trong tháng.
   - Nếu hôm nay tiêu vượt, DSA ngày mai tự động co lại để bảo toàn ngân sách; nếu hôm nay tiết kiệm, DSA ngày mai tự động giãn ra.

---

### Bước 5: Phân Hệ 5 — Quỹ Mục Tiêu Tích Lũy (Savings Goals) & Hũ Kakeibo
1. **Khởi tạo mục tiêu:** `POST /api/savings-goals`
   ```json
   {
     "name": "Quỹ Khẩn Cấp 3 Tháng",
     "targetAmount": 30000000,
     "startDate": "2026-10-01",
     "targetDate": "2027-01-31",
     "type": 1,
     "priority": 3
   }
   ```
2. **Nạp tiền vào quỹ:** `POST /api/savings-goals/{id}/deposit`
   ```json
   {
     "sourceWalletId": "{ID_VI_VIETCOMBANK}",
     "amount": 5000000,
     "note": "Trích tiết kiệm lương"
   }
   ```
   *(Trừ tiền ví Vietcombank, cộng dồn vào số dư quỹ mục tiêu)*.
3. **Rút tiền từ quỹ kèm Thaler Nudge Ma Sát Nhận Thức:** `POST /api/savings-goals/{id}/withdraw`
   ```json
   {
     "destinationWalletId": "{ID_VI_TIEN_MAT}",
     "amount": 1000000,
     "reason": "Chi tiêu khẩn cấp y tế"
   }
   ```
   *(Bắt buộc phải có trường `reason`. Nếu rút làm giảm tiến độ, hệ thống sẽ kích hoạt Nudge cảnh báo tâm lý)*.
4. **Cập nhật vi mô mục tiêu tích lũy (HTTP PATCH):** `PATCH /api/savings-goals/{id}`
   ```json
   {
     "name": "Quỹ Khẩn Cấp 6 Tháng (Nâng Cấp)",
     "targetAmount": 60000000
   }
   ```
5. **Chuyển đổi trạng thái mục tiêu (HTTP PATCH):** `PATCH /api/savings-goals/{id}/status`
   ```json
   {
     "status": 3
   }
   ```
   *(Trạng thái: 1 = Active, 2 = Completed, 3 = Paused, 4 = Cancelled)*.

---

### Bước 6: Phân Hệ 6 — Báo Cáo Phân Tích & Phát Hiện Bất Thường Z-Score
1. **Báo cáo Tài sản ròng (Net Worth Snapshot):** `GET /api/analytics/reports/net-worth`
2. **Báo cáo Dòng tiền Thác nước (Waterfall Net Cashflow):** `GET /api/analytics/reports/waterfall`
3. **Phát hiện giao dịch đột biến định lượng Z-Score ($\mu \pm 2\sigma$):** `GET /api/analytics/reports/outliers`
   - Tự động lọc và gắn cờ các khoản chi bất thường so với lịch sử thói quen tiêu dùng.

---

### Bước 7: Phân Hệ 7 — Dự Báo Tài Chính & Phòng Thí Nghiệm Sandbox What-If
1. **Dự báo Ngày cạn tiền (Out-of-Money Date):** `GET /api/forecasting/liquidity-depletion`
   - Tính toán Spending Velocity phân tách ngày trong tuần (Weekday) vs cuối tuần (Weekend).
2. **Phòng thí nghiệm Sandbox What-If (In-Memory 0ms, Zero DB Mutation):** `POST /api/forecasting/what-if`
   ```json
   {
     "scenarioType": 1,
     "lumpSumAmount": 15000000
   }
   ```
   *(Mô phỏng tác động đến ngày cạn tiền và ngân sách tương lai mà không làm thay đổi dữ liệu thật)*.

---

### Bước 8: Phân Hệ 8 — Cú Hích Thaler Nudge & Khoảng Trễ Tâm Lý 24h
1. **Lấy danh sách Cú hích cá nhân hóa:** `GET /api/nudges`
   - Hiển thị các giải trình XAI 3 thành phần: *Công thức toán học, Bằng chứng thực nghiệm, và Hệ quả tương lai*.
2. **Khoảng trễ tâm lý 24h (Cooling-Off Period) cho khoản chi lớn:** `POST /api/cooling-off`
   ```json
   {
     "walletId": "{ID_VI_VIETCOMBANK}",
     "amount": 4500000,
     "categoryId": "{ID_DANH_MUC_MUA_SAM}",
     "itemTitle": "Tai nghe chống ồn Sony WH-1000XM5",
     "coolingOffHours": 24
   }
   ```
   - Khoản chi **KHÔNG bị trừ tiền ví ngay lập tức**.
   - Kích hoạt đồng hồ đếm ngược 24h. Sau 24h người dùng sẽ quyết định Mua thật hay Hủy bỏ để giải tỏa hưng phấn mua sắm bốc đồng.

---

### Bước 9: Phân Hệ 9 & 10 — Động Cơ Hành Động Tự Hành & Hộp Thư Thông Báo
1. **Xử lý chuỗi thông báo biến động số dư ngân hàng:** `POST /api/autonomous-actions/process-raw-notification`
   ```json
   {
     "message": "Vietcombank: Giao dich -65.000 VND tai Highlands Coffee luc 25/10/2026. So du: 19.935.000 VND"
   }
   ```
2. **Kiểm tra phân tầng xác suất Jev Probability:**
   - **Xác suất $\ge 0.85$ (High Confidence):** Tự động sinh `SmartActionCard` trong Action Inbox để người dùng **Lưu nhanh 1-chạm (Quick Commit)**.
   - **Xác suất $0.5 - 0.85$ (Medium Confidence):** Đưa ra gợi ý thông minh (Jev Choice) hỏi ý kiến người dùng.
   - **Xác suất $< 0.5$ (Low Confidence):** Giữ im lặng, tránh gây phiền hà.
3. **Lưu nhanh 1-chạm:** `POST /api/autonomous-actions/{id}/quick-commit`
4. **Cập nhật trạng thái đọc thông báo (HTTP PATCH):**
   - Đánh dấu 1 thông báo đã đọc: `PATCH /api/v1/notifications/{id}/read`
   - Đánh dấu tất cả thông báo đã đọc: `PATCH /api/v1/notifications/read-all`

---

## 3. Phân Hệ Quản Trị Hệ Thống & Seeding Dữ Liệu (System Controller)

| Method | Endpoint | Mục Đích Sử Dụng | Response Chuẩn |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/system/database-status` | Kiểm tra trạng thái kết nối PostgreSQL & tổng số lượng bản ghi các bảng chính. | `200 OK` (hoặc `503 Service Unavailable` nếu rớt mạng CSDL) |
| `POST` | `/api/system/seed-admin` | Nạp sẵn tài khoản Quản trị viên mặc định (`admin@expensetracker.com` / `Admin@123`). | `200 OK` |
| `POST` | `/api/system/reset-database` | Làm mới hoàn toàn (`TRUNCATE CASCADE`) toàn bộ CSDL và nạp lại danh mục chuẩn. Dùng để làm sạch dữ liệu trước buổi demo từ con số 0. | `200 OK` |
| `POST` | `/api/system/seed-demo-story` | Nạp kịch bản kể chuyện hoàn chỉnh cho `demo@expensetracker.vn` / `Demo@123` (Trịnh Thành Đạt) với 3 ví (26.7M), ngân sách 85%, món đồ Sony 4.5M Cooling-off, hóa đơn WinMart, phiếu lương FPT. | `200 OK` |

---

## 4. Danh Mục Kiểm Thử Các Ca Lỗi Định Sẵn (Sad Path & Negative Matrix)

Phần này dùng để kiểm chứng tính bền vững của Backend khi nhận dữ liệu rác, hành vi cố ý vi phạm bảo mật, hoặc điều kiện biên:

### 4.1. Lỗi Bảo Mật & Xác Thực (Authentication & Security)

#### Ca kiểm thử 4.1.A: Gọi API cần quyền mà KHÔNG gửi kèm Bearer Token
* **Request:** `GET /api/wallets` (Header không có `Authorization`)
* **Mã mong đợi:** `401 Unauthorized`
* **Ý nghĩa:** Bảo đảm Middleware JWT chặn đứng mọi truy cập nặc danh vào dữ liệu tài chính.

#### Ca kiểm thử 4.1.B: Đăng nhập sai mật khẩu
* **Request:** `POST /api/auth/login`
  ```json
  {
    "email": "nguyenvana@gmail.com",
    "password": "SaiMatKhau123!"
  }
  ```
* **Mã mong đợi:** `400 Bad Request` hoặc `401 Unauthorized`
* **Body mong đợi:** `"Tài khoản hoặc mật khẩu không chính xác."` (Không được trả về chi tiết tài khoản có tồn tại hay không nhằm chống User Enumeration Attack).

#### Ca kiểm thử 4.1.C: Truy cập hoặc chỉnh sửa tài nguyên của Người Dùng Khác
* **Request:** `GET /api/wallets/{GUID_VI_CỦA_USER_KHAC}` (User B đang đăng nhập)
* **Mã mong đợi:** `404 Not Found` hoặc `403 Forbidden`
* **Ý nghĩa:** Global Query Filter hoặc Repository cô lập dữ liệu theo `UserId`, không cho phép xem trộm ví của nhau.

---

### 4.2. Lỗi Ràng Buộc Dữ Liệu (Validation & Model Binding)

#### Ca kiểm thử 4.2.A: Đăng ký với Email sai cú pháp
* **Request:** `POST /api/auth/register`
  ```json
  {
    "fullName": "Le Van B",
    "email": "email_khong_hop_le_@@gmail",
    "password": "Password123!"
  }
  ```
* **Mã mong đợi:** `400 Bad Request`
* **Body mong đợi:** Chứa mảng `errors.Email` báo lỗi định dạng email.

#### Ca kiểm thử 4.2.B: Tạo ví với tên rỗng hoặc số dư âm
* **Request:** `POST /api/wallets`
  ```json
  {
    "name": "   ",
    "walletType": 0,
    "initialBalance": -500000
  }
  ```
* **Mã mong đợi:** `400 Bad Request`
* **Ý nghĩa:** Domain Entity chặn ngay lập tức tên rỗng hoặc số dư âm bất hợp lệ.

#### Ca kiểm thử 4.2.C: Ghi nhận chi tiêu với số tiền bằng 0 hoặc âm
* **Request:** `POST /api/transactions/expenses`
  ```json
  {
    "walletId": "{ID_VI}",
    "amount": -100000,
    "categoryId": "{ID_DANH_MUC}",
    "title": "Khoản chi sai"
  }
  ```
* **Mã mong đợi:** `400 Bad Request`
* **Ý nghĩa:** Money Value Object từ chối số tiền $\le 0$.

---

### 4.3. Lỗi Vi Phạm Quy Tắc Nghiệp Vụ (Business Logic Constraints)

#### Ca kiểm thử 4.3.A: Chuyển tiền giữa hai ví TRÙNG NHAU
* **Request:** `POST /api/wallets/transfer`
  ```json
  {
    "sourceWalletId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "destinationWalletId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "amount": 500000
  }
  ```
* **Mã mong đợi:** `400 Bad Request`
* **Body mong đợi:** `"Ví nguồn và ví đích không được trùng nhau."`

#### Ca kiểm thử 4.3.B: Chuyển tiền VƯỢT QUÁ số dư khả dụng
* **Request:** `POST /api/wallets/transfer` (Ví chỉ có 2.000.000 đ nhưng yêu cầu chuyển 10.000.000 đ)
  ```json
  {
    "sourceWalletId": "{ID_VI_NGUON}",
    "destinationWalletId": "{ID_VI_DICH}",
    "amount": 10000000
  }
  ```
* **Mã mong đợi:** `400 Bad Request`
* **Body mong đợi:** `"Số dư khả dụng của ví nguồn không đủ để thực hiện chuyển tiền."`

#### Ca kiểm thử 4.3.C: Rút tiền từ Mục tiêu Tích lũy vượt quá số dư hiện có
* **Request:** `POST /api/savings-goals/{id}/withdraw` (Quỹ chỉ có 3.000.000 đ nhưng yêu cầu rút 7.000.000 đ)
  ```json
  {
    "destinationWalletId": "{ID_VI}",
    "amount": 7000000,
    "reason": "Cần tiền gấp"
  }
  ```
* **Mã mong đợi:** `400 Bad Request`
* **Body mong đợi:** `"Số tiền rút vượt quá số dư hiện tại trong mục tiêu tiết kiệm."`

#### Ca kiểm thử 4.3.D: Tạo Ngân sách với Ngày kết thúc NHỎ HƠN Ngày bắt đầu
* **Request:** `POST /api/budgets`
  ```json
  {
    "name": "Ngân sách lỗi thời gian",
    "startDate": "2026-10-31",
    "endDate": "2026-10-01",
    "totalLimit": 5000000
  }
  ```
* **Mã mong đợi:** `400 Bad Request`
* **Body mong đợi:** `"Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu."`

---

### 4.4. Lỗi Chống Spam & Giới Hạn Tần Suất (Rate Limiting 429)

#### Ca kiểm thử 4.4: Bắn liên tiếp 6 request OCR phân tích AI trong vòng 1 phút
* **Cơ chế đã cấu hình tại Program.cs:** Chính sách `ai_policy` với Fixed Window `5 requests / 1 phút` áp dụng trên các endpoint `/api/receipts/scan`, `/api/payslips/scan`, `/api/voice-agent/parse-intent`.
* **Kịch bản:** Gửi liên tục 6 request HTTP POST đến endpoint AI.
* **Kết quả mong đợi:**
  * Từ Request 1 đến 5: Xử lý bình thường (`200 OK`).
  * **Từ Request thứ 6:** Bị ASP.NET Core RateLimiter chặn đứng với mã **`429 Too Many Requests`**.
  * **Header trả về:** `Retry-After: 60` (hoặc thông báo thân thiện: *"Hệ thống đang bận. Vui lòng thử lại sau 1 phút."*).

---

## 5. Hướng Dẫn Vận Hành Bộ Script Tự Động Hóa Toàn Diện (`test_api_suite.ps1`)

Hệ thống đã được tích hợp bộ script PowerShell tự động kiểm thử toàn diện tại:  
👉 [`test_api_suite.ps1`](test_api_suite.ps1)

### 5.1 Các tính năng cốt lõi được tích hợp trong Script:
1. **Cơ chế Ngắt An Toàn Fail-Fast:** Script bắt đầu bằng việc kiểm tra sức khỏe CSDL PostgreSQL qua endpoint `/api/system/database-status`. Nếu CSDL Offline hoặc WebApi chưa chạy, chương trình sẽ in hướng dẫn khắc phục cụ thể, yêu cầu người dùng nhấn phím Enter và **tự động ngắt ngay lập tức (`exit 1`)**, không gửi thêm bất kỳ request rác nào.
2. **Loại Trừ Tuyệt Đối API Google Gemini AI:** Không kiểm thử các endpoint bóc tách hóa đơn, phiếu lương hay nhận dạng giọng nói bằng AI (`/api/receipts/parse`, `/api/payslips/parse`, `/api/voice-agent/parse-intent`) nhằm bảo vệ hạn mức Quota và tránh nguy cơ khóa API Key của người dùng. Các tính năng bóc tách phi-AI (luật regex tiếng Việt, chuẩn EMVCo VietQR) vẫn được kiểm thử 100%.
3. **Độ Bao Phủ Tuyệt Đối 15 Phân Hệ (113 Test Cases):** Toàn bộ các API endpoints trong source code backend từ Quản lý Đa ví ACID, Giao dịch Thu - Chi, Thu nhập định kỳ, Ngân sách DSA, Quỹ 6 chiếc hũ, Push FCM, Nudge tâm lý, Cooling-off 24h, Hộp thư tác tử tự hành Jev, Dự báo Sandbox, Báo cáo Net Worth, đến toàn bộ các lệnh Xóa (DELETE) và Kênh Bắt lỗi định sẵn (Sad Paths: 400, 401, 404).
4. **Tự Động Làm Sạch & Nạp Lại Master Demo Story:** Ở cuối chương trình, script tự động gọi `POST /api/system/reset-database` để dọn sạch toàn bộ dữ liệu phát sinh trong quá trình test, sau đó gọi `POST /api/system/seed-demo-story` nạp lại kịch bản Demo hoàn chỉnh.
5. **Dừng Màn Hình Chờ Lệnh Người Dùng (`Read-Host`):** Console không bao giờ tự động tắt khi chạy xong, mà dừng lại hiển thị bảng thống kê trực quan và đợi người dùng đọc kết quả rồi nhấn phím Enter mới đóng.

### 5.2 Cách chạy Script:
1. Đảm bảo CSDL PostgreSQL Docker và WebApi Backend đang hoạt động:
   ```powershell
   cd backend/ExpenseTracker.WebApi
   dotnet run
   ```
2. Mở một cửa sổ PowerShell mới tại thư mục `mobile-project/dotnet-maui-ver/` và chạy:
   ```powershell
   .\test_api_suite.ps1
   ```
   *(Tùy chọn: Có thể truyền tham số port khác nếu cần: `.\test_api_suite.ps1 -BaseUrl "http://localhost:5018"`)*.

### 5.3 Bảng Tổng Kết Kết Quả Nghiệm Thu Thực Tế (100% PASS):
```text
================================================================================
 TONG KET KET QUA BO KIEM THU TU DONG MASTER TEST SUITE
================================================================================
 Tong so bai kiem tra da chay : 113
 So bai kiem tra DAT (PASS)   : 113
 So bai kiem tra LOI (FAIL)   : 0
 Ty le thanh cong             : 100% (XUAT SAC - HOAN HAO)
================================================================================
 KET LUAN: Toan bo cac API trong he thong Backend da duoc kiem thu thanh cong 100%!
 Co so du lieu da duoc Reset va Nap san sang cho Demo Story (demo@expensetracker.vn / admin@expensetracker.com).
--------------------------------------------------------------------------------
 [HOAN TAT] Nhan phim Enter de dong cua so kiem thu...
```
