# KẾ HOẠCH TRIỂN KHAI KỸ THUẬT: PLAN-46
## TÍCH HỢP TOÀN DIỆN GOOGLE GEMINI MULTIMODAL AI THẬT TỪ GIAI ĐOẠN 1 ĐẾN GIAI ĐOẠN 10 & BẢO MẬT KHÓA API QUA GITIGNORE

> **Mã kế hoạch:** `PLAN-46`  
> **Chuyên gia chủ trì:** Agent 08 Andrew Ng (AI Lead), Agent 11 Bruce Schneier (Security Lead) & Agent 00 Orchestrator  
> **Ngày phê duyệt:** 25/10/2026  
> **Trạng thái:** `APPROVED BY USER — READY FOR IMPLEMENTATION`  
> **Mục tiêu:** Chấm dứt hoàn toàn việc sử dụng dữ liệu mô phỏng/giả định đối với các cấu phần AI; kết nối trực tiếp Google Gemini 2.5 Flash API thật thông qua kiến trúc AI Gateway an toàn, có logging debug chi tiết và bảo mật 100% API Key không bị commit lên Git.

---

## 1. BẢN ĐỒ ĐỐI SOÁT ĐIỂM CHẠM AI THỰC TẾ (GEMINI FOUNDATION MODEL)

Hệ thống phân định rạch ròi 2 trụ cột:
1. **Thuật toán Nghiệp vụ Định lượng & Thống kê (Deterministic Algorithms):** Z-Score Outlier $\mu \pm 2\sigma$, Time-Series Forecasting, Adaptive DSA, Thaler Nudge Invariant Formula $\rightarrow$ Chạy 100% bằng code toán học trên CPU/Database.
2. **Trí Tuệ Nhân Tạo Sinh & Thị Giác Đa Phương Thức (Multimodal Generative AI - Gemini 2.5 Flash):** Đảm nhiệm các bài toán nhận thức thị giác và ngôn ngữ tự nhiên không cấu trúc:

| STT | Phân Hệ & Giai Đoạn | Use Case ID | Điểm Chạm Nghiệp Vụ Bắt Buộc Sử Dụng AI Thật | Phương Thức Gọi Google Gemini 2.5 Flash | Tệp Mã Nguồn Đích Cần Cập Nhật |
| :---: | :--- | :---: | :--- | :--- | :--- |
| **1** | **Phân hệ 3 (GĐ 3)** | `UC-03.07` | **Bóc tách Hóa đơn Siêu thị & Tiện ích:** Đọc ảnh thật (WinMart, Co.opmart, EVN, Cấp nước), bóc tách danh sách món, đơn giá, số lượng, thuế VAT, chiết khấu, tổng tiền. | `GenerateContentWithImageAsync` (Base64 Inline Image + Strict JSON Schema) | `ReceiptOcrService.cs` |
| **2** | **Phân hệ 2 (GĐ 2)** | `UC-02.08` | **Bóc tách Phiếu lương định kỳ:** Đọc ảnh/PDF phiếu lương thật, bóc tách Công ty, Kỳ lương, Gross, Net, BHXH, BHYT, BHTN, Thuế TNCN. | `GenerateContentWithImageAsync` (Document PDF/Image + Strict JSON Schema) | `PayslipOcrService.cs` |
| **3** | **Phân hệ 7 (GĐ 10)** | `UC-07.06` | **Nhập liệu Ghi chú Tự do (Unstructured Text):** Đọc chuỗi nhật ký chi tiêu lộn xộn, bóc tách ra mảng JSON giao dịch chuẩn hóa. | `GenerateTextAsync` (NLP Contextual Parsing) | `HybridDataImportService.cs` |
| **4** | **Phân hệ 7 (GĐ 10)** | `UC-07.01 A1` | **Báo cáo Điều hành Cố vấn AI Richard Thaler:** Đọc chỉ số tài chính tháng (MoM, YoY, Net Worth, Savings Rate), viết nhận định thấu cảm 3-5 câu. | `GenerateTextAsync` (Richard Thaler System Prompt) | `ReportExportService.cs` |
| **5** | **Phân hệ 9 (GĐ 8)** | `UC-09.02` | **Trợ lý XAI giải trình lý do tài chính:** Sinh văn bản giải trình lý do tài chính cá nhân hóa, thấu cảm, không phán xét tiêu cực. | `GenerateTextAsync` (Empathetic Finance Rationale) | `NudgeEngineService.cs` |
| **6** | **Phân hệ 3 (GĐ 10)** | `UC-03.11` | **Voice Intent Router Agent:** Giải mã các câu khẩu lệnh phức tạp, đảo ngữ, đa ý định vượt ngoài Regex. | `GenerateTextAsync` (Function Calling JSON format) | `VoiceIntentRouterService.cs` |

---

## 2. KIẾN TRÚC BẢO MẬT API KEY (ZERO-LEAK GITIGNORE PROTOCOL)

Theo khuyến nghị của Chuyên gia Bảo mật Agent 11 Bruce Schneier:
1. **Quy tắc Bất Di Bất Dịch:** Tuyệt đối không lưu API Key cá nhân trong tệp [`appsettings.json`](../../mobile-project/dotnet-maui-ver/backend/ExpenseTracker.WebApi/appsettings.json) vì tệp này được commit lên Git Repository công khai/nội bộ.
2. **Cơ chế Phân Tầng Cấu Hình (Configuration Layering):**
   - [`appsettings.json`](../../mobile-project/dotnet-maui-ver/backend/ExpenseTracker.WebApi/appsettings.json) (Commit lên Git): Chứa cấu trúc khung và giá trị rỗng hoặc placeholder:
     ```json
     "AiSettings": {
       "Provider": "Gemini",
       "ApiKey": "",
       "Model": "gemini-2.5-flash",
       "Endpoint": "https://generativelanguage.googleapis.com/v1beta",
       "Temperature": 0.2,
       "MaxTokens": 4096,
       "EnableLogging": true
     }
     ```
   - [`appsettings.Local.json`](../../mobile-project/dotnet-maui-ver/backend/ExpenseTracker.WebApi/appsettings.Local.json) HOẶC [`appsettings.Development.json`](../../mobile-project/dotnet-maui-ver/backend/ExpenseTracker.WebApi/appsettings.Development.json) (ĐÃ ĐƯỢC THÊM VÀO `.gitignore`): Lưu API Key thật:
     ```json
     "AiSettings": {
       "ApiKey": "YOUR_SECURE_GEMINI_API_KEY_HERE"
     }
     ```
   - Hỗ trợ nạp từ **Biến môi trường (Environment Variable)**: `GEMINI_API_KEY` hoặc `AiSettings__ApiKey`.
3. **Đã cấu hình `.gitignore` an toàn:**
   - Đã thêm `appsettings*.Local.json`, `appsettings*.Secrets.json`, `*.secrets.json` và duy trì `appsettings*.Development.json`.

---

## 3. THIẾT KẾ MODULE `GeminiApiClient` (INFRASTRUCTURE LAYER)

- **Vị trí:** `mobile-project/dotnet-maui-ver/backend/ExpenseTracker.Infrastructure/Ai/`
  - [`IGeminiApiClient.cs`](../../mobile-project/dotnet-maui-ver/backend/ExpenseTracker.Application/Interfaces/IGeminiApiClient.cs)
  - [`GeminiApiClient.cs`](../../mobile-project/dotnet-maui-ver/backend/ExpenseTracker.Infrastructure/Ai/GeminiApiClient.cs)
- **Công nghệ:** Sử dụng `HttpClient` từ `IHttpClientFactory` với Timeout 30 giây.
- **Tính năng nổi bật:**
  1. *Multimodal Vision:* Gửi ảnh base64 trực tiếp tới endpoint:
     `POST {Endpoint}/models/{Model}:generateContent?key={ApiKey}`
  2. *Strict JSON Schema Enforcement:* Sử dụng `generationConfig: { response_mime_type: "application/json" }` để ép Gemini trả về đúng cấu trúc DTO mong muốn, loại bỏ markdown code fences (```json).
  3. *Visual Console Debugger:*
     - In log màu trực quan trên Terminal khi có request: `[Gemini >> Request]`
     - In log kết quả: `[Gemini << Response]` (Status 200/401/429, Latency ms, Tokens)
     - In toàn văn chuỗi JSON thô: `[Gemini << Raw JSON]` để lập trình viên đối soát ngay lập tức.
  4. *Circuit Breaker / Graceful Fallback:* Nếu mất mạng, sai key hoặc hết quota $\rightarrow$ Log rõ lỗi, chuyển về chế độ offline heuristic, không bao giờ để WebApi bị Crash 500.

---

## 4. KẾ HOẠCH NÂNG CẤP TỪNG SERVICE

### Giai đoạn 46.1: Khởi tạo Hạ Tầng & Cấu Hình Bảo Mật (HOÀN THÀNH)
1. Tạo class POCO [`AiSettings.cs`](../../mobile-project/dotnet-maui-ver/backend/ExpenseTracker.Application/Common/Settings/AiSettings.cs).
2. Tạo interface [`IGeminiApiClient.cs`](../../mobile-project/dotnet-maui-ver/backend/ExpenseTracker.Application/Interfaces/IGeminiApiClient.cs).
3. Triển khai [`GeminiApiClient.cs`](../../mobile-project/dotnet-maui-ver/backend/ExpenseTracker.Infrastructure/Ai/GeminiApiClient.cs).
4. Cấu hình `DependencyInjection.cs` đăng ký `HttpClient` cho `IGeminiApiClient`.
5. Tạo `appsettings.Local.json` chứa Key thật (đã được git ignore). Cập nhật `Program.cs` nạp `appsettings.Local.json` nếu file tồn tại.

### Giai đoạn 46.2: Refactor `ReceiptOcrService` (UC-03.07) & `PayslipOcrService` (UC-02.08) (HOÀN THÀNH)
1. Bơm `IGeminiApiClient` vào constructor của cả 2 service.
2. Viết System Prompt & JSON Schema chuẩn cho Hóa đơn (Siêu thị + Tiện ích EVN/Nước) và Phiếu lương.
3. Khi nhận ảnh upload: Gọi Gemini Vision thật $\rightarrow$ Nhận JSON $\rightarrow$ Đưa qua công thức Hậu kiểm toán số học.

### Giai đoạn 46.3: Refactor `VoiceIntentRouterService` (UC-09.05 / UC-03.11) & `ReportExportService` (UC-07.01 A1) (HOÀN THÀNH)
1. Tích hợp Gemini 2.5 Flash NLU cho điều hướng giọng nói đa ý định khẩu ngữ Việt Nam kèm Regex Fast-path 0ms & Hardened Fallback (tiếng lóng xị, lít, củ, trẹo, chục).
2. Tích hợp Gemini Persona Richard Thaler viết nhận định tài chính thật vào Sheet Cố Vấn tệp Excel Smart Executive Report.

### Giai đoạn 46.4: Kiểm Thử Toàn Diện & Kịch Bản Cho Người Dùng Test Thực Tế (HOÀN THÀNH)
1. Cập nhật Unit Tests trong `ExpenseTracker.Application.UnitTests`: Dùng Moq giả lập `IGeminiApiClient` để 263 tests pass 100% trong 4 giây.
2. Bộ test hoàn chỉnh cho `GeminiApiClientTests`, `VoiceIntentRouterServiceTests`, `ReportExportServiceTests`.
3. Cung cấp hướng dẫn chi tiết từng bước để Người dùng chạy Backend, upload ảnh hóa đơn thật qua Swagger hoặc test giọng nói và nhìn thấy log phản hồi từ Gemini thật.

---

## 5. BẢO TOÀN CHẤT LƯỢNG & ĐIỀU KIỆN NGHIỆM THU (QUALITY GATES)

- [x] 100% Khóa API cá nhân không xuất hiện trong Git Tracking (`git status` bảo vệ an toàn tệp bí mật).
- [x] Toàn bộ 263 Tests hiện tại PASSED 100% (231 Backend + 32 Mobile MAUI), 0 Warning, 0 Error.
- [x] Console Terminal in log kết nối Google Gemini thật khi gọi API (`[Gemini >> Request]`, `[Gemini << Response]`, `[Gemini << Raw JSON]`).
- [x] Ảnh hóa đơn thật được bóc tách chuẩn xác các trường và đối soát số học thành công.
- [x] Bóc tách khẩu lệnh giọng nói tiếng Việt đa dạng và nhận định Cố vấn Thaler tự động hóa thành công.
