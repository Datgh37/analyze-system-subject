# IMPLEMENTATION PLAN: PLAN-46.3
## NÂNG CẤP TẦNG TRỢ LÝ AI NLU: ĐIỀU HƯỚNG GIỌNG NÓI ĐA Ý ĐỊNH (`VoiceIntentRouterService`) & CỐ VẤN TÀI CHÍNH RICHARD THALER (`ReportExportService`) VỚI GOOGLE GEMINI NLP

> **Mã kế hoạch:** `PLAN-46.3`  
> **Thuộc kế hoạch tổng thể:** [`PLAN-46 (Tích hợp Toàn diện Gemini Multimodal AI thật từ Giai đoạn 1 đến 10)`](2026-10-25_plan_46_tich_hop_toan_dien_gemini_ai_that_tu_giai_doan_1_den_10.md)  
> **Chuyên gia phụ trách:** Agent 08 Andrew Ng (AI Lead), Agent 01 Richard Thaler (Behavioral Nudge Lead) & Agent 12 Kent Beck (QA Lead)  
> **Ngày lập:** 25/10/2026  
> **Trạng thái:** `COMPLETED (263/263 TESTS PASSED 100%)`  

---

### 1. BỐI CẢNH & MỤC TIÊU CỐT LÕI

Tiếp nối thành công của Chặng 46.2 (Đã kích hoạt Gemini Multimodal Vision cho Hóa đơn & Phiếu lương với 261/261 tests PASSED), Chặng 46.3 thực hiện nhiệm vụ hoàn thiện 2 phân hệ AI dựa trên ngôn ngữ tự nhiên (NLP/NLU) tiên tiến của Đề tài:

1. **Nâng Cấp Trợ Lý Giọng Nói Đa Ý Định Theo Kiến Trúc Hybrid AI (`VoiceIntentRouterService` - UC-09.05 & Kiến Trúc AI Agent Mobile):**
   - **Tư duy Kiến trúc Hybrid AI (Edge/Heuristic First & Smart LLM Routing):**
     * **Local Fast-Path (0ms, 0 Token, Tiết kiệm Pin & Băng thông):** Với các khẩu lệnh ngắn gọn, mẫu mực (như *"Ăn sáng 30k"*, *"Đổ xăng 50k"*, *"Xem báo cáo"*, *"Mở ngân sách"*), hệ thống kích hoạt bộ **Regex & Heuristic Engine chỉn chu** để xử lý tức thì với độ trễ 0ms mà không cần gọi mạng.
     * **Cloud Smart-Path (Google Gemini 2.5 Flash NLU):** Với các câu thoại khẩu ngữ phức tạp, đa mệnh đề, ngôn ngữ đời thường (như *"Hôm nay ăn tiệc liên hoan với công ty hết 1 củ 2, ghi nhận chi tiêu vào ví Momo nhé"*, *"Tháng này thấy tiêu hơi quá tay, mở giúp tôi radar thâm hụt xem sao"*), hệ thống tự động định tuyến sang Gemini để bóc tách sâu ý định, ngữ nghĩa và dữ liệu điền trước form.
     * **Hardened Fallback Safe-Path (Kháng lỗi & Hỗ trợ Offline toàn diện):** Nâng cấp bộ Regex và Heuristics cực kỳ chỉn chu, bao quát toàn diện các phương ngữ Việt Nam:
       - Đơn vị tiền tệ đời thường: `k`, `ka`, `nghìn`, `ngàn`.
       - Tiếng lóng miền Nam & miền Bắc: `xị` (1 xị = 100k, 5 xị = 500k), `lít` (1 lít = 100k, 2 lít = 200k, 5 lít = 500k, nửa lít = 50k), `củ`/`triệu`/`trẹo` (1 củ, 2 củ rưỡi, 2 trẹo 5), `chục` (3 chục = 30k, 5 chục = 50k).
       - Chữ số bằng lời: "năm mươi ngàn", "hai trăm ngàn", "một triệu", "năm trăm nghìn".
       - Mở rộng toàn diện các Navigation Routes trên Mobile: `BudgetHubPage`, `DebtTrackerPage`, `NudgeFeedPage`, `CoolingOffHubPage`, `WalletsPage`, `ReceiptOcrPage`, `AnalyticsReportPage`, `CreateGoalPage`, `CreateCategoryPage`.
     * Giúp hệ thống hoạt động mượt mà ngay cả khi thiết bị mất kết nối mạng hoặc không có API Key!

2. **Nâng Cấp Bản Nhận Định Cố Vấn Tài Chính Cá Nhân Richard Thaler (`ReportExportService` - UC-07.01 Alternative Flow A1):**
   - Hiện tại: Nhận định cố vấn trong file Excel Smart Executive Report đang sử dụng chuỗi ghép nối có sẵn.
   - Nâng cấp: Truyền toàn bộ bức tranh tài chính tháng của người dùng (Thu nhập, Chi tiêu, Thặng dư, Tỷ lệ tiết kiệm và Top 3 nhóm chi tiêu nhiều nhất) sang Google Gemini 2.5 Flash với System Prompt hóa thân thành **Giáo sư Richard Thaler (Nobel Kinh Tế 2017)**.
   - Gemini sinh ra bản nhận định điều hành tài chính sâu sắc, văn phong ấm áp, đồng cảm, không phán xét, kèm 1 Cú hích hành vi (Nudge) cụ thể để gắn trực tiếp vào Executive Box của Sheet `⭐ TongQuan_CoVanAI` trong tệp Excel `.xlsx`.
   - Giữ cơ chế Fallback về nhận định tĩnh nếu Gemini offline.

---

### 2. DANH MỤC CÁC TỆP MÃ NGUỒN LIÊN QUAN

| STT | Đường Dẫn Tệp | Hành Động | Trách Nhiệm Chi Tiết |
| :---: | :--- | :---: | :--- |
| 1 | `backend/ExpenseTracker.Application/Services/VoiceIntentRouterService.cs` | Refactor | Bơm `IGeminiApiClient`, gọi Gemini NLU phân tích khẩu lệnh tiếng Việt, parse JSON ý định và fallback Regex. |
| 2 | `backend/ExpenseTracker.Application/Services/ReportExportService.cs` | Refactor | Bơm `IGeminiApiClient`, gọi Gemini NLP hóa thân Cố vấn Thaler sinh nhận định tài chính nhúng vào Excel Executive Box. |
| 3 | `backend/tests/ExpenseTracker.Application.UnitTests/Services/VoiceIntentRouterServiceTests.cs` | Bổ sung | Thêm Unit Test mới kiểm thử tích hợp `Mock<IGeminiApiClient>` với khẩu lệnh giọng nói tự nhiên phức tạp. |
| 4 | `backend/tests/ExpenseTracker.Application.UnitTests/Services/ReportExportServiceTests.cs` | Bổ sung | Thêm Unit Test mới kiểm thử tích hợp `Mock<IGeminiApiClient>` sinh nhận định Cố vấn Thaler và nhúng vào file Excel. |

---

### 3. THIẾT KẾ KỸ THUẬT & PROMPT ENGINEERING

#### 3.1. Thiết Kế System Prompt NLU Cho Trợ Lý Giọng Nói (`VoiceIntentRouterService`)
```text
Bạn là trợ lý điều hướng giọng nói tài chính cá nhân thông minh (Voice Intent Router AI Agent).
Nhiệm vụ của bạn là phân tích khẩu lệnh tiếng Việt tự nhiên của người dùng (bao gồm tiếng lóng đời thường như '50k', '2 củ rưỡi', 'năm xị', 'hai trẹo', 'đi chợ', 'trả tiền trọ', 'mở báo cáo', 'tạo quỹ tiết kiệm mua xe', 'tạo danh mục Nuôi Mèo'...).

YÊU CẦU ĐỊNH DẠNG:
Trả về DUY NHẤT một chuỗi JSON thuần túy (không kèm markdown code block, không có lời dẫn) theo schema:
{
  "detectedIntent": "AddTransaction" | "NavigateTo" | "QueryReport" | "Unknown",
  "actionType": "Expense" | "Income" | "Navigation",
  "targetRoute": "AddTransactionPage" | "CreateCategoryPage" | "CreateGoalPage" | "AnalyticsReportPage",
  "extractedAmount": 0.0,
  "suggestedCategory": "Tên danh mục phù hợp",
  "confirmationMessage": "Lời phản hồi thân thiện, xác nhận ngắn gọn",
  "confidence": 0.95,
  "prefillData": {
    "key": "value"
  }
}
```

#### 3.2. Thiết Kế System Prompt Cho Cố Vấn Richard Thaler (`ReportExportService`)
```text
Bạn là Giáo sư Richard Thaler, Chuyên gia Kinh tế Hành vi (Nobel Kinh Tế 2017) và Cố vấn Tài chính Cá nhân Cấp cao (Empathetic Behavioral Finance Advisor).
Dựa trên số liệu tài chính tháng của người dùng, hãy viết một đoạn nhận định điều hành tài chính cá nhân (khoảng 3-4 câu) với văn phong chuyên nghiệp, ấm áp, đồng cảm, không phán xét. Đưa ra 1 cú hích hành vi (Nudge) nhẹ nhàng giúp người dùng duy trì kỷ luật hoặc giảm thiểu thâm hụt mà không gây ma sát tâm lý.

YÊU CẦU: Trả về trực tiếp đoạn văn bản ngắn gọn, súc tích (không kèm markdown code block hoặc tiêu đề thừa).
```

---

### 4. KỊCH BẢN KIỂM THỬ (TEST PLAN) & TIÊU CHÍ CHẤP NHẬN

1. **Bảo Toàn Bộ Kiểm Thử Hiện Tại:**
   - Toàn bộ 261 tests hiện tại phải tiếp tục **PASSED 100%**.
2. **Kiểm Thử Mới Cho Luồng Gemini NLP/NLU:**
   - Viết test case trong `VoiceIntentRouterServiceTests` mô phỏng Gemini NLU phân tích câu nói chứa tiếng lóng và ngữ cảnh chi tiêu $\rightarrow$ Kiểm tra trích xuất đúng `TargetRoute`, `ExtractedAmount` và `PrefillData`.
   - Viết test case trong `ReportExportServiceTests` mô phỏng Gemini NLP sinh nhận định $\rightarrow$ Đọc lại file Excel sinh ra, kiểm tra ô tính `A4` trong sheet `TongQuan_CoVanAI` chứa đúng nhận định của AI.
3. **Mục Tiêu Chất Lượng:**
   - Đạt **263 / 263 Tests PASSED 100%**, biên dịch đạt **0 Warning, 0 Error**.
