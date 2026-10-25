# Cẩm Nang Kiểm Thử Tổng Thể API & Kênh Realtime (API Master Testing Playbook)

> **Dự án:** Hệ thống Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh (Tích hợp AI)  
> **Ngăn xếp công nghệ:** ASP.NET Core 10 Web API, EF Core PostgreSQL 16, SignalR Core, Google Gemini 2.5 Flash Vision & NLP.  
> **Tài liệu tham chiếu:** Quay lại trang chủ [`README.md`](README.md) để xem giới thiệu kiến trúc và hướng dẫn cài đặt.  
> **Toàn bộ đường dẫn trong tài liệu này đều lấy thư mục `dotnet-maui-ver` làm gốc.**

---

## 1. Tổng Quan Chu Trình Kiểm Thử E2E (End-to-End Testing Flow)

Toàn bộ hệ thống Backend cung cấp hơn 40 API Endpoints bám sát **63 Use Cases chi tiết (từ UC-01 đến UC-10)**. Kiểm thử có thể thực hiện trực quan thông qua Swagger UI tại:
👉 **`http://localhost:5018/swagger`** *(hoặc `https://localhost:7295/swagger`)*

```text
┌─────────────────────────────────────────────────────────────────────────────┐
│ LUỒNG KIỂM THỬ E2E CHUẨN:                                                   │
│ 1. Auth ──> 2. Đa Ví ──> 3. Thu Nhập / Chi Tiêu / OCR ──> 4. Ngân Sách DSA  │
│      │                                                            │         │
│      ▼                                                            ▼         │
│ 5. Quỹ Tích Lũy ──> 6. Báo Cáo Phân Tích ──> 7. Dự Báo ──> 8. Nudge & 24h  │
│                                                                  │         │
│                                                                  ▼         │
│                                                   9. Jev Action Inbox       │
└─────────────────────────────────────────────────────────────────────────────┘
```

---

## 2. Kịch Bản Kiểm Thử Chi Tiết Từng Phân Hệ

### Bước 1: Phân Hệ 1 — Xác Thực (Auth) & Cấp Quyền Truy Cập
1. **Đăng ký tài khoản mới:** `POST /api/auth/register`
   ```json
   {
     "fullName": "Nguyen Van A",
     "email": "nguyenvana@gmail.com",
     "username": "nguyenvana",
     "password": "Password123!"
   }
   ```
   *(Nhận phản hồi HTTP 201 Created kèm thông tin User Profile)*.
2. **Đăng nhập:** `POST /api/auth/login`
   ```json
   {
     "username": "nguyenvana",
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
     "walletName": "Ví Tiền Mặt",
     "walletType": 0,
     "currency": "VND",
     "initialBalance": 5000000,
     "colorHex": "#10B981"
   }
   ```
2. **Tạo Ví Ngân Hàng:** `POST /api/wallets`
   ```json
   {
     "walletName": "Vietcombank",
     "walletType": 1,
     "currency": "VND",
     "initialBalance": 20000000,
     "colorHex": "#3B82F6"
   }
   ```
3. **Xem danh sách ví:** `GET /api/wallets`  
   *(Kiểm tra tính toán Tổng tài sản khả dụng)*.
4. **Cân chỉnh số dư thực tế (Audit Adjustment):** `POST /api/wallets/{id}/adjust`
   ```json
   {
     "newActualBalance": 4950000,
     "reason": "Mua đồ vặt quên ghi chép"
   }
   ```
5. **Cập nhật vi mô từng phần (HTTP PATCH - RFC 7396):** `PATCH /api/wallets/{id}`
   ```json
   {
     "colorHex": "#EC4899",
     "iconKey": "star",
     "isFavorite": true
   }
   ```
   *(Chỉ gửi những trường cần sửa, không ghi đè số dư hay loại tiền)*.

---

### Bước 3: Phân Hệ 2 & 3 — Thu Nhập, Chi Tiêu, AI OCR & Danh Mục
1. **Ghi nhận Thu nhập:** `POST /api/incomes`
   ```json
   {
     "walletId": "{ID_VI_VIETCOMBANK}",
     "amount": 25000000,
     "categoryId": "{ID_DANH_MUC_LUONG}",
     "title": "Nhận lương công ty Tháng 10",
     "transactionDate": "2026-10-25T00:00:00Z"
   }
   ```
2. **Ghi nhận Chi tiêu thường:** `POST /api/expenses`
   ```json
   {
     "walletId": "{ID_VI_TIEN_MAT}",
     "amount": 65000,
     "categoryId": "{ID_DANH_MUC_AN_UONG}",
     "title": "Cà phê sáng Highlands",
     "transactionDate": "2026-10-25T08:30:00Z"
   }
   ```
   *(Kiểm tra số dư ví tiền mặt lập tức bị trừ 65.000 đ chuẩn ACID)*.
3. **Thử nghiệm AI Multimodal Vision OCR Hóa Đơn Siêu Thị:** `POST /api/ocr/receipt`
   - Gửi ảnh hóa đơn hoặc file base64.
   - Động cơ AI tự động bóc tách từng món hàng (Line Items) và kích hoạt **Bộ kiểm toán số học (Mathematical Audit Engine)** đối soát $\sum \text{Món} + \text{VAT} - \text{Giảm giá} = \text{Tổng tiền}$ với sai số $\le 1.000$ đ.
4. **Cập nhật vi mô diện mạo Danh mục (HTTP PATCH - RFC 7396):** `PATCH /api/categories/{id}`
   ```json
   {
     "colorHex": "#8B5CF6",
     "iconKey": "coffee-cup",
     "displayOrder": 2
   }
   ```
   *(Chỉ cập nhật mã màu và biểu tượng mà không làm xáo trộn cấu trúc phân cấp cây danh mục)*.

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
   - Nếu hôm nay tiêu vượt, DSA ngày mai tự động co lại để bảo toàn ngân sách; nếu hôm nay tiết kiệm, DSA ngày mai tự động giãn ra!

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
2. **Nạp tiền vào quỹ:** `POST /api/savings-goals/{id}/deposit` (Trừ tiền ví nguồn, cộng dồn vào quỹ).
3. **Rút tiền từ quỹ kèm Thaler Nudge Ma Sát Nhận Thức:** `POST /api/savings-goals/{id}/withdraw`
   - Bắt buộc phải có trường `reason` (Lý do rút). Nếu rút làm giảm tiến độ, hệ thống sẽ kích hoạt Nudge cảnh báo tâm lý.
4. **Cập nhật vi mô mục tiêu tích lũy (HTTP PATCH - RFC 7396):** `PATCH /api/savings-goals/{id}`
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
   *(Trạng thái: 1 = Active, 2 = Completed, 3 = Paused, 4 = Cancelled — chuyển trạng thái tức thì không cần gửi lại toàn bộ thông tin mục tiêu)*.
6. **Bật/Tắt lịch trích nạp tự động (HTTP PATCH):** `PATCH /api/savings-goals/{id}/auto-deposit`
   ```json
   {
     "isEnabled": true,
     "amount": 2000000,
     "dayOfMonth": 10,
     "walletId": "{ID_VI_NGUON}"
   }
   ```

---

### Bước 6: Phân Hệ 6 — Báo Cáo Phân Tích & Phát Hiện Bất Thường Z-Score
1. **Báo cáo Tài sản ròng (Net Worth Snapshot):** `GET /api/reports/net-worth`
2. **Báo cáo Dòng tiền Thác nước (Waterfall Net Cashflow):** `GET /api/reports/waterfall`
3. **Phát hiện giao dịch đột biến định lượng Z-Score ($\mu \pm 2\sigma$):** `GET /api/reports/outliers`
   - Tự động lọc và gắn cờ các khoản chi bất thường so với lịch sử thói quen tiêu dùng.

---

### Bước 7: Phân Hệ 7 — Dự Báo Tài Chính & Phòng Thí Nghiệm Sandbox What-If
1. **Dự báo Ngày cạn tiền (Out-of-Money Date):** `GET /api/forecasting/liquidity-runway`
   - Tính toán Spending Velocity phân tách ngày trong tuần (Weekday) vs cuối tuần (Weekend).
2. **Phòng thí nghiệm Sandbox What-If (In-Memory 0ms):** `POST /api/forecasting/what-if-sandbox`
   ```json
   {
     "scenarioType": "LumpSumPurchase",
     "amount": 20000000,
     "description": "Mua iPhone mới"
   }
   ```
   *(Mô phỏng tác động đến ngày cạn tiền và ngân sách tương lai mà không làm thay đổi dữ liệu thật)*.

---

### Bước 8: Phân Hệ 8 — Cú Hích Thaler Nudge & Khoảng Trễ Tâm Lý 24h
1. **Lấy danh sách Cú hích cá nhân hóa:** `GET /api/nudges`
   - Hiển thị các giải trình XAI 3 thành phần: *Công thức toán học, Bằng chứng thực nghiệm, và Hệ quả tương lai*.
2. **Khoảng trễ tâm lý 24h (Cooling-Off Period) cho khoản chi lớn:** `POST /api/expenses/cooling-off`
   ```json
   {
     "walletId": "{ID_VI}",
     "amount": 5000000,
     "categoryId": "{ID_DANH_MUC}",
     "title": "Mua đồng hồ thông minh",
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
   - Lập tức hạch toán vào sổ cái chi tiêu và cập nhật số dư ví tức thì!
4. **Cập nhật trạng thái đọc thông báo (HTTP PATCH):**
   - Đánh dấu 1 thông báo đã đọc: `PATCH /api/v1/notifications/{id}/read`
   - Đánh dấu tất cả thông báo đã đọc: `PATCH /api/v1/notifications/read-all`
   *(Chỉ lật cờ `isRead = true` ở cấp độ vi mô mà không bắt client phải gửi lại toàn bộ nội dung title hay body)*.

---

## 3. Đánh Giá Hiện Trạng Kênh Thời Gian Thực Realtime SignalR Hub

> [!NOTE]
> **Giải đáp kỹ thuật về tiến độ cập nhật SignalR:**  
> Kênh SignalR Realtime Hub (`/hubs/financial`) được khởi dựng ở **Giai đoạn 5B (PLAN-41 / Quyết Định 120)** nhằm đáp ứng barem điểm cộng B2 môn Lập trình Di động. Tại thời điểm đó, Hub tập trung phục vụ 3 sự kiện nền tảng:
> 1. `ReceiveBalanceUpdate`: Cập nhật số dư ví thời gian thực giữa 2 thiết bị.
> 2. `ReceiveBudgetAlert`: Phát cảnh báo khi chi tiêu vượt 80% / 100% ngân sách.
> 3. `ReceiveNotification`: Đẩy thông báo in-app thời gian thực.
>
> Khi hệ thống hoàn thiện thêm các phân hệ nâng cao (Giai đoạn 6 đến 10.5: What-If, Cooling-off 24h, Jev Autonomous Action), các phân hệ này **đã được kết nối trực tiếp vào `IFinancialRealtimeNotifier`** để gửi thông báo in-app.  
> Đối với màn hình Frontend MAUI, các ViewModel `WalletsViewModel` và `BudgetsViewModel` đã tích hợp lắng nghe SignalR qua `WeakReferenceMessenger`. Các màn hình còn lại hoạt động theo cơ chế **Pull-on-Demand** kết hợp **Optimistic UI 0ms** thông qua MVVM Repositories.
