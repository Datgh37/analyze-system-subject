# KẾ HOẠCH TRIỂN KHAI PLAN-51: NÂNG CẤP TOÀN DIỆN API TESTING PLAYBOOK & BỘ SCRIPT KIỂM THỬ TỰ ĐỘNG (METHOD 2)

> **Mã kế hoạch:** `PLAN-51`  
> **Ngày lập:** 2026-10-26  
> **Người đề xuất:** Chủ tọa đề tài & Hội đồng Đa tác tử (Kent Beck, Bruce Schneier, Werner Vogels)  
> **Mục tiêu:** Xây dựng giải pháp kiểm thử API toàn diện, siêu nhẹ (Zero Heavy Overhead) bằng cách hoàn thiện cẩm nang `API_TESTING_PLAYBOOK.md` với đầy đủ ma trận Happy/Sad Path và cung cấp script kiểm thử tự động `test_api_suite.ps1`.

---

## 1. Bối Cảnh & Định Hướng Kiến Trúc (Decision Rationale)

1. **Từ chối giải pháp tạo Project .NET C# Integration Test riêng (Method 1):**
   * *Lý do:* Tạo thêm một project test C# với `WebApplicationFactory`, Microsoft.AspNetCore.Mvc.Testing, EntityFrameworkCore.InMemory/PostgreSQL gây tốn dung lượng ổ đĩa, tăng thời gian build `dotnet build`, và phức tạp hóa cấu trúc source code của sinh viên khi nộp bài cho Hội đồng.
2. **Lựa chọn giải pháp Tự Động Hóa Thực Tế (Method 2):**
   * *Lợi ích:* Sử dụng script PowerShell native (`test_api_suite.ps1`) chạy thẳng vào Backend API đang mở trên cổng `localhost:5018`.
   * Gửi HTTP Request thật, kiểm tra mã HTTP Status Code thật, kiểm tra Body JSON trả về thật.
   * Kiểm thử được cả các tính năng đặc thù chỉ có trên runtime: **ASP.NET Core Rate Limiting (429 Too Many Requests)**, **PostgreSQL Row-level Locking (FOR UPDATE)**, **SignalR Realtime Notifier** và **Database Reset/Seeding**.

---

## 2. Các Hạng Mục Triển Khai Chi Tiết

### Chặng 1: Nâng cấp Toàn Diện Cẩm Nang `API_TESTING_PLAYBOOK.md`
- **Mở rộng phạm vi bao phủ:** Toàn bộ 22 Controllers với hơn 40 Endpoints.
- **Bổ sung Ma trận Kiểm Thử Hai Chiều (Dual-Path Matrix):**
  * **Happy Path (200 OK / 201 Created / 204 No Content):** Luồng chuẩn hoạt động mượt mà.
  * **Sad Path & Negative Cases (400 / 401 / 403 / 404 / 429):**
    * *Security 401:* Truy cập không gửi Bearer JWT Token.
    * *Validation 400:* Gửi email sai cú pháp, số tiền âm, tên rỗng.
    * *Business Logic 400:* Chuyển tiền cùng 1 ví, chuyển quá số dư khả dụng, rút quỹ tích lũy quá số dư.
    * *Rate Limiting 429:* Spam gọi AI OCR vượt ngưỡng 5 req/phút per IP/User.
    * *System 200:* Kiểm tra Database Status, Reset Database, Seed Master Demo Story.

### Chặng 2: Xây Dựng Bộ Script Tự Động Hóa `test_api_suite.ps1`
- **Vị trí file:** `mobile-project/dotnet-maui-ver/test_api_suite.ps1`.
- **Cơ chế vận hành:**
  1. Kiểm tra kết nối Backend `http://localhost:5018/api/system/database-status`. Nếu chưa bật, thông báo hướng dẫn bật `dotnet run`.
  2. Tự động gọi `POST /api/system/reset-database` để dọn sạch CSDL về trạng thái chuẩn.
  3. Thực hiện kịch bản Auth: Đăng ký user test $\rightarrow$ Đăng nhập lấy Token.
  4. Thực hiện các ca kiểm thử Happy Path: Tạo ví $\rightarrow$ Nạp thu nhập $\rightarrow$ Chi tiêu $\rightarrow$ Tạo ngân sách $\rightarrow$ Tạo mục tiêu tiết kiệm $\rightarrow$ Nạp tiền vào quỹ.
  5. Thực hiện các ca kiểm thử Sad Path (Negative Cases):
     - Test không token $\rightarrow$ Bắt mã 401.
     - Test validation số tiền âm $\rightarrow$ Bắt mã 400.
     - Test chuyển tiền cùng ví $\rightarrow$ Bắt mã 400.
     - Test rút quỹ quá số tiền hiện có $\rightarrow$ Bắt mã 400.
     - Test spam AI OCR 6 lần $\rightarrow$ Bắt mã 429 Too Many Requests.
  6. In ra màn hình console định dạng ANSI màu sắc rực rỡ, tổng kết: Số test chạy, Số test Pass, Số test Fail, Tỷ lệ thành công (%).

---

## 3. Tiêu Chí Nghiệm Thu (Acceptance Criteria)
1. `API_TESTING_PLAYBOOK.md` được cập nhật đầy đủ, rõ ràng, chi tiết từng tham số Request/Response và các mã lỗi dự kiến.
2. File `test_api_suite.ps1` hoạt động trơn tru trên Windows PowerShell, không yêu cầu cài thêm module ngoài (sử dụng `Invoke-RestMethod` native).
3. Ghi nhận nhật ký quyết định vào `project_context/decisions_and_changes_log.md` (Quyết Định 158).
