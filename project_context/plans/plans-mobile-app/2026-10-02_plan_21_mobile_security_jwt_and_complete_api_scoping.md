# 🛡️ KẾ HOẠCH ĐIỀU HÀNH HỘI ĐỒNG (COUNCIL MASTER PLAN) - PLAN-21
## KIỂM TRA TOÀN DIỆN KIẾN TRÚC MOBILE, THIẾT KẾ BẢO MẬT JWT AUTHENTICATION, XÁC ĐỊNH PHẠM VI MVP & CHUẨN HÓA TOÀN BỘ BACKEND RESTful API

> **Cơ quan ban hành:** Hội đồng Đa tác tử (Multi-Agent Council)  
> **Chủ tịch hội đồng (Orchestrator):** Agent 00 — Council Orchestrator  
> **Các chuyên gia tham gia thẩm định:**  
> - Agent 03: Alistair Cockburn & Debra Paul (Lead Business Analyst)  
> - Agent 11: Bruce Schneier & OWASP (Fintech Security Lead)  
> - Agent 05: Martin Kleppmann & Sadalage (Universal Database Lead)  
> - Agent 10: Dan Abramov & Christopher Chedeau (Frontend Architect Lead)  
> - Agent 02: Personal Budget Coach (Chuyên gia Kỷ luật Ngân sách)  
> - Agent 12: Kent Beck (QA & Full-Stack Test Strategy Lead)  
> **Người thực hiện:** Trịnh Thành Đạt (Lead - Architecture & Backend) & Trần Trọng Đại (Frontend UI & API Testing)  
> **Ngày phê chuẩn:** 2026-10-02  
> **Trạng thái:** `APPROVED & LOCKED BY COUNCIL`

---

## 1. Bối Cảnh, Đánh Giá Khuyết Tật & Lời Trấn An Từ Hội Đồng

### 1.1. Lời trấn an từ Chủ tịch Hội đồng (Agent 00)
Hội đồng ghi nhận tinh thần trách nhiệm cao của Trưởng nhóm (Trịnh Thành Đạt). Việc nhận ra hệ thống API ban đầu còn đơn sơ và thiếu lớp bảo mật không phải là "lỗi lầm", mà là **một bước chuyển biến tất yếu trong quy trình phát triển phần mềm Agile/Scrum**. 
- Trong Sprint 1 & 2 (PLAN-20), mục tiêu tối thượng là **Proof of Concept (PoC) & Scaffolding** (dựng khung sườn, kiểm tra thông suốt Apache WAMP, Expo Metro Bundler).
- Khi bước vào Sprint 3 & 4, việc nâng cấp lên **Kiến trúc Bảo mật Chuẩn Công Nghiệp (Production-Grade Security & Clean API Contract)** là hoàn toàn đúng thời điểm, giúp thành viên phụ trách Frontend (bạn Đại) có định hướng rõ ràng, không bị mông lung.

### 1.2. Kiểm toán hiện trạng kỹ thuật (Technical Audit)
Hội đồng đã kiểm tra toàn bộ mã nguồn `mobile-project/BE/` và chỉ ra 4 khuyết tật cốt tử cần khắc phục ngay:
1. **Lỗ hổng IDOR (Insecure Direct Object Reference) nghiêm trọng:**
   - Các API hiện tại đang nhận tham số `user_id` truyền trần qua Query String (`?user_id=1`) hoặc JSON Body.
   - Bất kỳ ai chỉ cần đổi số `?user_id=2` là có thể xem trộm toàn bộ số dư, lịch sử giao dịch và ví tiền của người khác mà không bị chặn.
2. **Cơ chế Token sơ sài (Mock Token):**
   - API Login hiện tại chỉ tạo chuỗi `base64_encode(id:email:timestamp)`. Chuỗi này không có chữ ký mật mã (Cryptographic Signature), rất dễ bị làm giả mạo danh tính (Impersonation Attack).
3. **Thiếu Middleware kiểm tra xác thực tập trung:**
   - Các file API nghiệp vụ (`wallets`, `categories`, `transactions`, `reports`) hoàn toàn mở toang, không kiểm tra Header `Authorization: Bearer <token>`.
4. **Bỏ quên Phân hệ Hạn mức Ngân sách (Budgets):**
   - Bảng `budgets` đã được thiết kế sẵn trong CSDL MySQL 5 bảng nhưng chưa hề có bất kỳ API nào để thiết lập và theo dõi tiến độ chi tiêu — đây là tính năng "trái tim" của một ứng dụng tài chính cá nhân.

---

## 2. Phân Định Ranh Giới Phạm Vi Đề Tài Mobile (Scope Scoping by Lead BA)

Chuyên gia BA Lead (Agent 03) cùng Personal Budget Coach (Agent 02) xác lập ranh giới rõ ràng giữa **Đồ án Khóa luận Tốt nghiệp Tổng thể (PTTKHTTT)** và **Đồ án Môn học Lập trình Di động (Mobile MVP)**:

```text
┌────────────────────────────────────────────────────────────────────────────────────────┐
│                        MA TRẬN PHÂN ĐỊNH PHẠM VI (SCOPE MATRIX)                        │
├──────────────────────────────────────────┬─────────────────────────────────────────────┤
│ 🟢 NẰM TRONG PHẠM VI APP MOBILE (MVP)    │ 🔴 NGOÀI PHẠM VI MOBILE (DÀNH CHO ĐỒ ÁN LỚN)│
├──────────────────────────────────────────┼─────────────────────────────────────────────┤
│ 1. Xác thực & Hồ sơ (JWT Auth & Profile)  │ 1. Bóc tách hóa đơn AI Multimodal Vision OCR│
│ 2. Quản lý Đa Ví (Tiền mặt, Ngân hàng)   │ 2. Thuật toán tối ưu nợ Snowball/Avalanche  │
│ 3. Danh mục Thu & Chi phân loại          │ 3. Dự báo thâm hụt tiền & Spending Velocity │
│ 4. Giao dịch Thu/Chi cập nhật số dư ACID │ 4. Quét bẫy gia hạn tự động (Subscriptions) │
│ 5. Hạn mức Ngân sách tháng & Cảnh báo    │ 5. Hạch toán tâm lý 6 Chiếc Hũ nâng cao     │
│ 6. Báo cáo Tài chính (PieChart, BarChart)│ 6. Đồng bộ Vector Clock đa thiết bị Offline │
└──────────────────────────────────────────┴─────────────────────────────────────────────┘
```

> **Quy chuẩn MVP Hoàn hảo:** Tập trung làm **thật chắc, thật mượt và bảo mật chuẩn mực** 6 phân hệ cốt lõi trên. Giảng viên mới chấm thi sẽ cực kỳ ấn tượng vì ứng dụng hoạt động trơn tru từ A đến Z, có Token bảo vệ, giao diện thuần khiết và dữ liệu nhất quán.

---

## 3. Kiến Trúc Bảo Mật Chuẩn (Fintech Security Architecture by Bruce Schneier)

### 3.1. Thiết kế JWT Token chuẩn HMAC-SHA256 (Không cần thư viện Composer cồng kềnh)
Để đảm bảo code nhẹ nhàng, dễ chạy trên mọi phiên bản PHP của WAMP và máy chủ trường, Agent 11 thiết kế bộ tạo/giải mã JWT thuần PHP an toàn:
- **Header:** `{"alg": "HS256", "typ": "JWT"}`
- **Payload:** `{"user_id": 1, "email": "...", "iat": <timestamp>, "exp": <timestamp + 7 ngày>}`
- **Secret Key:** Lưu trữ trong biến cấu hình bảo mật `JWT_SECRET_KEY = 'DNTU_LTMB_NHOM46_SECURE_JWT_SECRET_KEY_2026'`
- **Signature:** `hash_hmac('sha256', "$header_b64.$payload_b64", $secret, true)`

### 3.2. Hàm Middleware Xác thực Tập Trung: `require_authenticated_user()`
- Đặt tập trung trong `mobile-project/BE/config/helpers.php`.
- Tự động trích xuất chuỗi từ Header HTTP: `Authorization: Bearer <jwt_token>`.
- Kiểm tra tính hợp lệ của chữ ký số và thời hạn hết hạn (`exp`).
- **Triệt tiêu lỗ hổng IDOR:** Hàm trả về trực tiếp `$user_id` đã được xác minh từ Token. Mọi API nghiệp vụ sẽ dùng `$user_id` này để truy vấn CSDL, **ngăn chặn 100% việc user này xem trộm dữ liệu của user khác**.

---

## 4. Danh Mục Toàn Bộ 16 RESTful API Hoàn Chỉnh Của Đồ Án

```text
┌────────────────────────────────────────────────────────────────────────────────────────┐
│                      HỆ THỐNG 16 ENDPOINT RESTful API HOÀN CHỈNH                       │
├────┬────────┬───────────────────────────────────────┬────────────┬─────────────────────┤
│ STT│ METHOD │ URI ENDPOINT                          │ BẢO MẬT    │ MÔ TẢ CHỨC NĂNG     │
├────┼────────┼───────────────────────────────────────┼────────────┼─────────────────────┤
│ 1  │ POST   │ /api/auth/register.php                │ Public     │ Đăng ký tài khoản   │
│ 2  │ POST   │ /api/auth/login.php                   │ Public     │ Đăng nhập & cấp JWT │
│ 3  │ GET    │ /api/auth/profile.php                 │ JWT Bearer │ Lấy thông tin hồ sơ │
│ 4  │ POST   │ /api/auth/profile.php                 │ JWT Bearer │ Sửa tên / Đổi pass  │
├────┼────────┼───────────────────────────────────────┼────────────┼─────────────────────┤
│ 5  │ GET    │ /api/wallets/list.php                 │ JWT Bearer │ Danh sách ví & số dư│
│ 6  │ POST   │ /api/wallets/create.php               │ JWT Bearer │ Tạo ví tài chính mới│
├────┼────────┼───────────────────────────────────────┼────────────┼─────────────────────┤
│ 7  │ GET    │ /api/categories/list.php              │ JWT Bearer │ Danh mục Thu / Chi  │
│ 8  │ POST   │ /api/categories/create.php            │ JWT Bearer │ Tạo danh mục riêng  │
├────┼────────┼───────────────────────────────────────┼────────────┼─────────────────────┤
│ 9  │ GET    │ /api/transactions/list.php            │ JWT Bearer │ Lịch sử giao dịch   │
│ 10 │ POST   │ /api/transactions/create.php          │ JWT Bearer │ Ghi chép Thu / Chi  │
│ 11 │ POST   │ /api/transactions/delete.php          │ JWT Bearer │ Xóa & hoàn lại số dư│
├────┼────────┼───────────────────────────────────────┼────────────┼─────────────────────┤
│ 12 │ GET    │ /api/budgets/status.php               │ JWT Bearer │ Hạn mức & Tiến độ % │
│ 13 │ POST   │ /api/budgets/set.php                  │ JWT Bearer │ Thiết lập ngân sách │
├────┼────────┼───────────────────────────────────────┼────────────┼─────────────────────┤
│ 14 │ GET    │ /api/reports/monthly_summary.php      │ JWT Bearer │ Tổng quan tháng     │
│ 15 │ GET    │ /api/reports/category_chart.php       │ JWT Bearer │ Biểu đồ tròn chi    │
│ 16 │ GET    │ /api/reports/income_vs_expense_chart.php│ JWT Bearer │ Biểu đồ cột 6 tháng │
└────┴────────┴───────────────────────────────────────┴────────────┴─────────────────────┘
```

---

### 3.3. Thiết Kế Luồng Quên Mật Khẩu Chuẩn Công Nghiệp (3-Step OTP / Token Flow)
Chuyên gia Bảo mật (Agent 11) cùng Phân tích nghiệp vụ (Agent 03) thiết kế luồng khôi phục tài khoản hoàn chỉnh, nói KHÔNG với việc làm qua loa:
- **Nguyên tắc bảo mật:** Tuyệt đối không bao giờ gửi lại mật khẩu cũ vì mật khẩu đã được băm một chiều Bcrypt.
- **Quy trình 3 bước khép kín:**
  1. **Bước 1 — Yêu cầu đặt lại mật khẩu (`POST /api/auth/forgot_password.php`):**
     - Nhận `email`. Kiểm tra tồn tại trong bảng `users`.
     - Sinh mã OTP ngẫu nhiên 6 chữ số (ví dụ: `849201`) có hiệu lực trong 15 phút.
     - Lưu trữ vào bảng `password_resets` (`email`, `otp_hash`, `expires_at`, `created_at`).
     - **Cơ chế Dual-mode thực chiến:** Hỗ trợ gửi thư qua SMTP (Gmail/PHPMailer). Trong môi trường phát triển máy cá nhân/WAMP chưa mở cổng SMTP, API trả về trường `"debug_otp"` trong phản hồi JSON nếu `$is_local = true`. Giúp cả nhóm kiểm thử mượt mà trên Postman và Expo mà không bị lỗi đứt gánh.
  2. **Bước 2 — Xác thực mã OTP (`POST /api/auth/verify_otp.php`):**
     - Nhận `email`, `otp`.
     - Kiểm tra mã OTP chưa hết hạn và đúng mã.
     - Cấp một mã `reset_token` tạm thời (thời hạn 5 phút) để cho phép chuyển sang bước đổi mật khẩu.
  3. **Bước 3 — Đặt mật khẩu mới (`POST /api/auth/reset_password.php`):**
     - Nhận `email`, `reset_token`, `new_password`.
     - Xác thực `reset_token`. Băm mật khẩu mới bằng Bcrypt (`password_hash()`), cập nhật vào bảng `users`.
     - Hủy hiệu lực toàn bộ mã OTP và reset token cũ để chống tấn công Replay Attack.

---

## 4. Phân Tích Nghiệp Vụ Thực Tế, Trải Nghiệm Người Dùng (UX) & Tiện Ích Đời Sống (QOL)

Một ứng dụng Quản lý chi tiêu thực chiến (dùng được thật hằng ngày, không phải đồ án đồ chơi) bắt buộc phải đáp ứng 4 bài toán UX & QOL cốt tử:

1. **QOL #1 — Chuyển tiền nội bộ giữa các ví (`api/wallets/transfer.php`):**
   - *Tình huống thực tế:* Rút 2 triệu từ thẻ Vietcombank ra Tiền mặt tiêu xài, hoặc nạp 500k từ MoMo sang MB Bank.
   - *Vấn đề nếu thiếu:* Người dùng phải tự tạo 1 giao dịch Chi ở ví A và 1 giao dịch Thu ở ví B, làm báo cáo thu chi tháng bị đội lên gấp đôi (sai lệch thống kê tài chính).
   - *Giải pháp:* API Transfer xử lý nguyên tử (ACID Transaction): Trừ tiền ví nguồn, cộng tiền ví đích, ghi nhận 1 bản ghi chuyển tiền nội bộ mà không tính vào chi phí sinh hoạt.
2. **QOL #2 — Chỉnh sửa giao dịch kèm Tự động bù trừ số dư (`api/transactions/update.php`):**
   - *Tình huống thực tế:* Đi ăn trưa gõ nhầm 450.000đ thành 45.000đ.
   - *Giải pháp:* API Update tự động tính sai lệch chênh lệch (Delta = Số tiền mới - Số tiền cũ), bù trừ trực tiếp vào số dư ví tương ứng để số dư luôn chính xác 100%.
3. **UX #3 — Bộ lọc và Tìm kiếm giao dịch đa tiêu chí:**
   - Hỗ trợ lọc theo Tháng/Năm, lọc theo từng Ví, lọc theo Danh mục và tìm kiếm từ khóa ghi chú (`keyword`).
4. **UX #4 — Thẻ Tiến Độ Ngân Sách Trực Quan (Budget Progress Bar):**
   - Hiển thị trực tiếp trên trang chủ: Đã tiêu bao nhiêu %, còn lại bao nhiêu ngày trong tháng, tốc độ chi tiêu an toàn mỗi ngày (Daily Safe Spending).

---

## 5. Danh Mục Toàn Bộ 19 RESTful API Hoàn Chỉnh Của Đồ Án (Production-Ready)

```text
┌────┬────────┬────────────────────────────────────────┬────────────┬──────────────────────────────────┐
│ STT│ METHOD │ URI ENDPOINT                           │ BẢO MẬT    │ MÔ TẢ NGHIỆP VỤ & TÍNH NĂNG      │
├────┼────────┼────────────────────────────────────────┼────────────┼──────────────────────────────────┤
│ 1  │ POST   │ /api/auth/register.php                 │ Public     │ Đăng ký tài khoản mới            │
│ 2  │ POST   │ /api/auth/login.php                    │ Public     │ Đăng nhập & cấp JWT Access Token │
│ 3  │ POST   │ /api/auth/forgot_password.php          │ Public     │ Yêu cầu gửi OTP quên mật khẩu    │
│ 4  │ POST   │ /api/auth/verify_otp.php               │ Public     │ Xác thực OTP & nhận reset token  │
│ 5  │ POST   │ /api/auth/reset_password.php           │ Public     │ Đặt lại mật khẩu mới bằng token  │
│ 6  │ GET    │ /api/auth/profile.php                  │ JWT Bearer │ Lấy thông tin tài khoản & thống kê│
│ 7  │ POST   │ /api/auth/profile.php                  │ JWT Bearer │ Đổi tên hiển thị / Đổi mật khẩu  │
├────┼────────┼────────────────────────────────────────┼────────────┼──────────────────────────────────┤
│ 8  │ GET    │ /api/wallets/list.php                  │ JWT Bearer │ Danh sách ví & Tổng tài sản ròng │
│ 9  │ POST   │ /api/wallets/create.php                │ JWT Bearer │ Tạo ví mới (Tiền mặt, Ngân hàng) │
│ 10 │ POST   │ /api/wallets/transfer.php              │ JWT Bearer │ Chuyển tiền nội bộ giữa 2 ví ACID│
├────┼────────┼────────────────────────────────────────┼────────────┼──────────────────────────────────┤
│ 11 │ GET    │ /api/categories/list.php               │ JWT Bearer │ Danh mục Thu/Chi (Icons & Màu)   │
│ 12 │ POST   │ /api/categories/create.php             │ JWT Bearer │ Tạo danh mục riêng của người dùng│
├────┼────────┼────────────────────────────────────────┼────────────┼──────────────────────────────────┤
│ 13 │ GET    │ /api/transactions/list.php             │ JWT Bearer │ Lịch sử giao dịch (Phân trang/Lọc)│
│ 14 │ POST   │ /api/transactions/create.php           │ JWT Bearer │ Thêm giao dịch thu/chi (ACID)    │
│ 15 │ POST   │ /api/transactions/update.php           │ JWT Bearer │ Sửa giao dịch, tự bù trừ số dư ví│
│ 16 │ POST   │ /api/transactions/delete.php           │ JWT Bearer │ Xóa giao dịch, hoàn lại tiền ví  │
├────┼────────┼────────────────────────────────────────┼────────────┼──────────────────────────────────┤
│ 17 │ GET    │ /api/budgets/status.php                │ JWT Bearer │ Tiến độ ngân sách tháng & cảnh báo│
│ 18 │ POST   │ /api/budgets/set.php                   │ JWT Bearer │ Thiết lập hạn mức chi tiêu tháng │
├────┼────────┼────────────────────────────────────────┼────────────┼──────────────────────────────────┤
│ 19 │ GET    │ /api/reports/monthly_summary.php       │ JWT Bearer │ Báo cáo tổng quan thu - chi tháng│
│ 20 │ GET    │ /api/reports/category_chart.php        │ JWT Bearer │ Biểu đồ tròn tỷ trọng chi tiêu   │
│ 21 │ GET    │ /api/reports/income_vs_expense_chart.php│ JWT Bearer│ Biểu đồ cột so sánh 6 tháng      │
└────┴────────┴────────────────────────────────────────┴────────────┴──────────────────────────────────┘
```

---

## 6. Khuyến Nghị Ngăn Xếp Công Nghệ Chuẩn Cho Frontend (Node 24 & Android 14)

Chuyên gia Kiến trúc Client (Agent 10 - Dan Abramov) phân tích và đưa ra khuyến nghị dứt khoát:

* **Môi trường của người dùng:**
  - Node.js: `v24.19.0` (Node 24 LTS).
  - Thiết bị kiểm thử: **Android 14 (API Level 34 — Upside Down Cake)**.
  - Ứng dụng Expo Go trên thiết bị: **Phiên bản Expo SDK 54**.
* **Khuyến nghị dứt khoát về Ngăn xếp Công nghệ:**
  - **Khung ứng dụng:** **Expo SDK 54** (`expo: ~54.0.37`).
    - *Lý do:* Expo SDK 54 được xây dựng nhắm mục tiêu chuẩn xác vào **Android 14 (API 34) & Android 15 (API 35)**, hỗ trợ chính thức Node 20/22/24. Ứng dụng Expo Go hiện có trên điện thoại của bạn cũng yêu cầu SDK 54, nếu dùng bản thấp hơn (SDK 51 hay 52) điện thoại sẽ báo lỗi không tương thích phiên bản.
  - **Core Engines:** `react-native: 0.81.5`, `react: 19.1.0`.
  - **Điều hướng:** `@react-navigation/native: ^7.4.1`, `@react-navigation/bottom-tabs: ^7.19.2`, `@react-navigation/native-stack: ^7.19.2`.
  - **Quản lý phiên & Mạng:** `axios: ^1.7.9`, `@react-native-async-storage/async-storage: ~2.2.0`.
  - **Đồ họa & Biểu đồ:** `react-native-svg: 15.12.1`, `react-native-chart-kit: ^6.12.0`.
  - **Quy chuẩn Styling:** 100% `StyleSheet.create({...})` và hằng số `COLORS`, tuyệt đối không dùng thư viện CSS trung gian để phục vụ tốt nhất cho buổi thi vấn đáp.

---

## 7. Lộ Trình Triển Khai Thực Thi Chi Tiết (Action Steps)

### 🔹 Bước 1: Khởi tạo lại Bộ khung Frontend Chuẩn Expo SDK 54 (FE Clean Setup)
- Khởi tạo lại thư mục `mobile-project/FE/` hoàn toàn sạch sẽ.
- Thiết lập `package.json` với Expo SDK 54, `app.json`, `babel.config.js`, `.npmrc` (`node-linker=hoisted`), `.gitignore`.
- Cấu hình Axios Client kẹp tự động Header `Authorization: Bearer <token>` và bộ điều hướng Screen đầy đủ màn hình (bao gồm Login, Register, ForgotPassword, Tabs: Home, Transactions, Budget, Reports, Profile).

### 🔹 Bước 2: Nâng cấp Toàn bộ Bộ Lõi Backend & CSDL (BE Hardening)
- Bổ sung bảng `password_resets` vào `mobile-project/BE/sql/database_schema.sql` và MySQL WAMP.
- Xây dựng bộ sinh/giải mã JWT HMAC-SHA256 và middleware `require_authenticated_user()` trong `config/helpers.php`.
- Nâng cấp các API xác thực: `login.php` (cấp JWT), `register.php`, `forgot_password.php`, `verify_otp.php`, `reset_password.php`, `profile.php`.

### 🔹 Bước 3: Hoàn thiện Các API Nghiệp Vụ & QOL Cốt Tử
- Nâng cấp bảo vệ token và triệt tiêu IDOR cho `wallets/list.php`, `wallets/create.php`, `categories/*`, `transactions/*`, `reports/*`.
- Viết mới API chuyển tiền giữa các ví: `api/wallets/transfer.php`.
- Viết mới API cập nhật giao dịch tự bù trừ số dư: `api/transactions/update.php`.
- Viết mới phân hệ ngân sách: `api/budgets/status.php` và `api/budgets/set.php`.

### 🔹 Bước 4: Kiểm Thử Toàn Diện Tự Động & Bàn Giao
- Nâng cấp `test_api.ps1` kiểm tra tự động toàn bộ 21 endpoint (bao gồm luồng Quên mật khẩu OTP, Chuyển ví, Ngân sách, Báo cáo).
- Cập nhật tài liệu kỹ thuật bàn giao cho bạn Đại.

---

## 8. Lời Kết Từ Hội Đồng
Với việc bổ sung luồng **Quên Mật Khẩu Chuẩn OTP**, **Chuyển Tiền Ví Nội Bộ (Transfer)**, **Ngân Sách (Budgets)** và **Bảo Mật JWT Triệt Tiêu IDOR**, đồ án của bạn sẽ trở thành một sản phẩm Fintech cá nhân hoàn chỉnh ở đẳng cấp ứng dụng thương mại thực thụ (MVP Production-Ready). Hội đồng phê chuẩn và sẵn sàng đồng hành cùng bạn thực hiện từng bước!
