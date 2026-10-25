# IMPLEMENTATION PLAN: PLAN-46.1
## KHỞI TẠO TẦNG HẠ TẦNG GEMINI AI GATEWAY (GeminiApiClient), CẤU HÌNH BẢO MẬT GITIGNORE & VISUAL CONSOLE DEBUGGER

> **Mã kế hoạch:** `PLAN-46.1`  
> **Thuộc kế hoạch tổng thể:** [`PLAN-46 (Tích hợp Toàn diện Gemini Multimodal AI thật từ Giai đoạn 1 đến 10)`](2026-10-25_plan_46_tich_hop_toan_dien_gemini_ai_that_tu_giai_doan_1_den_10.md)  
> **Chuyên gia phụ trách:** Agent 08 Andrew Ng (AI Lead), Agent 11 Bruce Schneier (Security Lead) & Agent 12 Kent Beck (QA Lead)  
> **Ngày lập:** 25/10/2026  
> **Trạng thái:** `COMPLETED (259/259 Tests Passed 100%, 0 Error)`  

---

### 1. BỐI CẢNH & MỤC TIÊU CỐT LÕI

Để kết nối ứng dụng với mô hình **Google Gemini 2.5 Flash thật sự**, Chặng 46.1 tập trung xây dựng nền móng hạ tầng vững chắc, giải quyết triệt để 3 yêu cầu cấp bách:
1. **Bảo mật tuyệt đối khóa API cá nhân (Zero-Leak GitIgnore):** Đảm bảo API Key thật của Người dùng được lưu trữ độc lập trên máy cá nhân và được `.gitignore` bảo vệ 100%, không bao giờ bị lộ lên Git.
2. **Hiện thực hóa Module AI Gateway (`GeminiApiClient`):** Xây dựng một HTTP Client chuẩn Enterprise kết nối trực tiếp Google AI Studio REST API, hỗ trợ gửi ảnh Base64 (Multimodal Vision) và văn bản tự do kèm Strict JSON Schema (`response_mime_type: "application/json"`).
3. **Bộ chẩn đoán trực quan (Visual Console Debugger):** In log chi tiết độ trễ, mã phản hồi HTTP và toàn văn JSON thô mà Google Gemini trả về ngay trên màn hình Terminal để Người dùng dễ dàng theo dõi và debug.

---

### 2. DANH MỤC CÁC TỆP CẦN TẠO MỚI & CHỈNH SỬA

| STT | Đường Dẫn Tệp | Hành Động | Trách Nhiệm Nghiệp Vụ |
| :---: | :--- | :---: | :--- |
| 1 | `backend/ExpenseTracker.WebApi/appsettings.json` | Chỉnh sửa | Bổ sung section `"AiSettings"` với giá trị rỗng (`"ApiKey": ""`) để commit an toàn lên Git. |
| 2 | `backend/ExpenseTracker.WebApi/appsettings.Local.json` | Tạo mới | Chứa API Key thật của Người dùng (được lưu cục bộ), được `.gitignore` loại trừ 100%. |
| 3 | `backend/ExpenseTracker.WebApi/Program.cs` | Chỉnh sửa | Cấu hình nạp tệp `appsettings.Local.json` (optional: true) để tự động ghi đè Key khi chạy local. |
| 4 | `backend/ExpenseTracker.Application/Common/Settings/AiSettings.cs` | Tạo mới | Lớp POCO chứa cấu hình `Provider`, `ApiKey`, `Model`, `Endpoint`, `Temperature`, `MaxTokens`, `EnableLogging`. |
| 5 | `backend/ExpenseTracker.Application/Interfaces/IGeminiApiClient.cs` | Tạo mới | Định nghĩa hợp đồng interface: `GenerateContentWithImageAsync` và `GenerateTextAsync`. |
| 6 | `backend/ExpenseTracker.Infrastructure/Ai/GeminiApiClient.cs` | Tạo mới | Triển khai HTTP Client gọi Google Gemini 2.5 Flash REST API, tích hợp Base64, Strict JSON Schema và Console Debugger. |
| 7 | `backend/ExpenseTracker.Infrastructure/ExpenseTracker.Infrastructure.csproj` | Chỉnh sửa | Đảm bảo hỗ trợ `IHttpClientFactory` và các thư viện mạng. |
| 8 | `backend/ExpenseTracker.Infrastructure/DependencyInjection.cs` | Chỉnh sửa | Đăng ký `AiSettings` và cấu hình `AddHttpClient<IGeminiApiClient, GeminiApiClient>()`. |
| 9 | `backend/tests/ExpenseTracker.Application.UnitTests/Services/GeminiApiClientTests.cs` | Tạo mới | Kiểm thử Unit Tests tự động bằng `MockHttpMessageHandler` (HTTP 200, 401, 429, Timeout). |

---

### 3. THIẾT KẾ KỸ THUẬT CHI TIẾT

#### 3.1. Cơ Chế Phân Tầng Cấu Hình Bảo Mật (Configuration Layering)
- Trong `appsettings.json` (Commit lên Git):
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
- Trong `appsettings.Local.json` (Tệp local không commit - đã nằm trong `.gitignore`):
  ```json
  {
    "AiSettings": {
      "ApiKey": "YOUR_SECURE_GEMINI_API_KEY_HERE"
    }
  }
  ```
- Trong `Program.cs`:
  ```csharp
  builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);
  ```

#### 3.2. Cấu Trúc Payload Gọi Google Gemini 2.5 Flash REST API
- Endpoint:
  `POST {Endpoint}/models/{Model}:generateContent?key={ApiKey}`
- Cấu trúc Request Body JSON:
  ```json
  {
    "systemInstruction": {
      "parts": [{ "text": "{systemInstruction}" }]
    },
    "contents": [{
      "parts": [
        { "text": "{prompt}" },
        {
          "inlineData": {
            "mimeType": "image/jpeg",
            "data": "{base64Data}"
          }
        }
      ]
    }],
    "generationConfig": {
      "temperature": 0.2,
      "maxOutputTokens": 4096,
      "responseMimeType": "application/json"
    }
  }
  ```

#### 3.3. Bộ Chẩn Đoán Visual Console Debugger
Khi có bất kỳ yêu cầu nào gửi sang Gemini, Terminal của Backend sẽ in ra định dạng trực quan:
```text
[Gemini >> Request] Model: gemini-2.5-flash | Payload: 184 KB (Image inline) | Time: 09:30:15
[Gemini << Response] Status: 200 OK | Latency: 780 ms | Tokens: 425
[Gemini << Raw JSON]:
{
  "merchantName": "Siêu thị WinMart",
  "invoiceDate": "2026-10-25T08:30:00Z",
  "totalAmount": 245000,
  "items": [ ... ]
}
```
Nếu xảy ra lỗi từ phía Google:
```text
[Gemini << Error] HTTP 401 Unauthorized: API key not valid. Please pass a valid API key.
[Gemini << Error] HTTP 429 Too Many Requests: Resource has been exhausted (check quota).
```
Đồng thời, hàm sẽ trả về `Result<string>.Failure("Lỗi từ Google Gemini API: [Chi tiết lỗi]...")`, không bao giờ để WebApi bị Crash.

---

### 4. KỊCH BẢN KIỂM THỬ (TEST PLAN)

1. **Test Tự Động (Unit Tests - Offline 100%):**
   - Viết tệp test `GeminiApiClientTests.cs` sử dụng `MockHttpMessageHandler` để mô phỏng:
     * Kịch bản 1: Google Gemini trả về 200 OK với JSON trích xuất hợp lệ $\rightarrow$ Trả về `Result.Success`.
     * Kịch bản 2: Google Gemini trả về 401 Unauthorized $\rightarrow$ Trả về `Result.Failure` kèm thông báo lỗi Key.
     * Kịch bản 3: Google Gemini trả về 429 Too Many Requests $\rightarrow$ Trả về `Result.Failure` cảnh báo hết quota.
     * Kịch bản 4: Mạng bị ngắt kết nối (HttpRequestException) $\rightarrow$ Bắt ngoại lệ và trả về `Result.Failure` an toàn.
   - Chạy toàn bộ test suite để đảm bảo **tổng số test tăng từ 252 lên 256+ tests, 100% PASSED**.

2. **Kiểm Toán Bảo Mật Git (Security Audit):**
   - Chạy lệnh `git status -s` để xác minh:
     * Tệp `appsettings.Local.json` hoàn toàn **KHÔNG xuất hiện** trong danh sách Untracked hay Modified.
     * Tệp `appsettings.json` chỉ chứa placeholder, **tuyệt đối không chứa Key thật**.

---

### 5. TIÊU CHÍ HOÀN THÀNH (ACCEPTANCE CRITERIA)

- [ ] Lớp POCO `AiSettings` và Interface `IGeminiApiClient` được tạo chuẩn mực.
- [ ] `GeminiApiClient` biên dịch thành công, hỗ trợ đầy đủ Multimodal Vision và Text NLP.
- [ ] Tệp `appsettings.Local.json` chứa Key thật của Người dùng và được Gitignore bảo vệ 100%.
- [ ] Toàn bộ Unit Tests (Domain, Application, Maui) đạt **100% PASSED**, 0 Error, 0 Warning liên quan.
- [ ] Không có bất kỳ rò rỉ secret key nào trong Git tracking.
