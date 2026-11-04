# KẾ HOẠCH PLAN-72: TÍCH HỢP TOÀN DIỆN GIAO DIỆN WEB VỚI BACKEND ASP.NET CORE 10 WEB API (FULL-STACK WEB INTEGRATION)

> **Mục tiêu:** Gắn kết trọn vẹn giữa giao diện Web Frontend (Open Design / wwwroot) và Backend ASP.NET Core 10 Clean Architecture (`ExpenseTracker.WebApi`) để hệ thống web vận hành mượt mà, hỗ trợ cả 2 chế độ Live Backend API (`http://localhost:5018`) và Demo Ngoại Tuyến (Offline Mock).

---

## 1. Bối Cảnh & Hiện Trạng
1. **Frontend Web:** Đã hoàn thiện bộ nguyên mẫu trực quan 10 màn hình chuẩn Dark Luxury Fintech (`Open Design/duonopro-mobile-app/duonopro-mobile-app.html`).
2. **Backend Web API:** Đầy đủ 25 Controllers, 516/516 Unit Tests Passed, hỗ trợ tự động SQLite fallback (`expensetracker.db`) khi chưa có PostgreSQL, cấu hình CORS `AllowAll`, và SignalR Realtime Hub `/hubs/financial`.
3. **Nhu Cầu Cấp Thiết:**
   - Kết nối hai đầu: Cho phép giao diện Web thực hiện gọi API thật (Auth login, Wallets, Transactions, DSA Budgets, Autonomous Actions, Debts) khi backend hoạt động.
   - Triển khai trực tiếp Web App vào `ExpenseTracker.WebApi/wwwroot/index.html` để khi khởi chạy backend, người dùng chỉ cần mở `http://localhost:5018` là web chạy mượt mà ngay lập tức.
   - Tự động duy trì cơ chế Dual-Mode (Live API & Offline Mock Graceful Fallback) để đảm bảo không bao giờ bị gián đoạn trải nghiệm người dùng.

---

## 2. Các Bước Triển Khai Kỹ Thuật

### Bước 1: Xây dựng Lớp Tích Hợp API Client Trực Tiếp Trên Web Frontend
- Tự động xác định Base URL:
  - Nếu chạy từ port 5018 (`window.location.port === "5018"`): Sử dụng relative path `/api`.
  - Nếu chạy từ Open Design Preview (`http://127.0.0.1:55607/...`) hoặc file cục bộ: Trỏ về `http://localhost:5018`.
- Khởi tạo `DuoNoProApiGateway`:
  - `checkBackendHealth()`: Ping `/api/health`.
  - `authenticateDemoUser()`: Tự động đăng nhập `demo@expensetracker.vn` / `admin@expensetracker.com` lấy JWT Bearer token và lưu `localStorage`.
  - `loadWallets()`: Đồng bộ danh sách ví và số dư Net Worth từ `/api/wallets` và `/api/net-worth/current`.
  - `loadTransactions()`: Tải lịch sử chi tiêu từ `/api/transactions`.
  - `simulateBankNotification(bankCode)`: Gửi thông báo ngân hàng đến `/api/actions/from-notification` và hiển thị thẻ hành động thông minh.
  - `commitAction(actionId)`: Gửi lệnh lưu 1-chạm vào sổ cái qua `/api/actions/quick-commit`.
  - `loadBudgetsAndDsa()`: Tải trạng thái ngân sách và hạn mức ngày từ `/api/budgets/active/dsa`.
  - `loadDebtOptimization()`: Tải chiến lược Snowball vs Avalanche từ `/api/debts/optimization`.
- Kết nối Realtime SignalR: Tích hợp thư viện SignalR client lắng nghe sự kiện tài chính từ `/hubs/financial`.

### Bước 2: Đồng Bộ Vào `ExpenseTracker.WebApi/wwwroot/`
- Xuất bản bản web chính thức vào `mobile-project/dotnet-maui-ver/backend/ExpenseTracker.WebApi/wwwroot/index.html`.
- Đồng bộ `tokens.css` vào `wwwroot/css/tokens.css` và cập nhật đường dẫn stylesheet linh hoạt để tương thích cả Open Design lẫn Web Host.
- Đảm bảo `app.UseDefaultFiles()` và `app.UseStaticFiles()` trong `Program.cs` phục vụ trang `index.html` mặc định khi vào `http://localhost:5018`.

### Bước 3: Thử Nghiệm Khởi Chạy Backend & Kiểm Tra Toàn Trình (Verification)
- Khởi chạy Web API trên cổng `http://localhost:5018`.
- Kiểm tra tính năng Health check `/api/health`, Swagger docs `/swagger`, và tải trang chính `http://localhost:5018/`.
- Xác nhận các luồng gọi API và kiểm thử tự động toàn hệ thống 516/516 tests.

---

## 3. Tiêu Chí Nghiệm Thu (Acceptance Criteria)
- [x] Web chạy mượt mà trên trình duyệt cả từ Open Design Live Preview lẫn trực tiếp từ backend `http://localhost:5018`.
- [x] Có biểu tượng trạng thái kết nối trực quan (🟢 Live API / 🟡 Offline Mock) hiển thị độ trễ ping rõ ràng.
- [x] Dữ liệu ví và chi tiêu kết nối chính xác với CSDL SQLite/PostgreSQL của backend.
- [x] 516 / 516 Unit Tests tiếp tục Passed 100%.

