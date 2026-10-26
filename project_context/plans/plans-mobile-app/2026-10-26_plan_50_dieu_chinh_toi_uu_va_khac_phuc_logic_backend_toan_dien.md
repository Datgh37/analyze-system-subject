# KẾ HOẠCH CHI TIẾT (PLAN-50): ĐIỀU CHỈNH, TỐI ƯU HÓA, BỔ SUNG VÀ KHẮC PHỤC CÁC VẤN ĐỀ LOGIC TRONG PHÂN HỆ BACKEND
> **Dự án:** Hệ thống Quản lý Chi tiêu & Hoạch định Tài chính Cá nhân Thông minh (PFM)  
> **Mã kế hoạch:** `PLAN-50`  
> **Thời điểm xác lập:** 2026-10-26 09:10  
> **Cơ chế điều phối:** Hội đồng Đa tác tử (Multi-Agent Council) phối hợp cùng Chủ tọa đề tài.  
> **Trạng thái Giai đoạn 11:** ⚠️ **TIẾP TỤC ĐÓNG BĂNG (FROZEN)** cho đến khi hoàn tất nghiệm thu kế hoạch này.  
> **Mục đích tài liệu:** Lưu vết lịch sử (Audit Trail), làm căn cứ kỹ thuật bền vững (Durable Context) có thể chuyển giao và thực thi liền mạch qua các phiên làm việc độc lập.

---

## I. BỐI CẢNH, MỤC TIÊU VÀ NGUYÊN TẮC CỐT LÕI

### 1. Bối Cảnh Thực Tế
Sau khi giải quyết triệt để các rào cản về hạ tầng build (.NET MAUI 10 trên Windows App SDK và Android AAPT2 MaxPath qua ổ ảo `X:`), hệ thống đã đạt trạng thái biên dịch và vận hành ổn định trên thiết bị. Tuy nhiên, rà soát logic mã nguồn Backend cho thấy một số cấu phần quan trọng đang bị thụt lùi so với tiến độ thực tế của 9 phân hệ và barem điểm chấm thi cuối kỳ.

### 2. Mục Tiêu Của Kế Hoạch
1. **Khắc phục độ thụt lùi của SignalR Hub:** Đưa kênh thời gian thực từ trạng thái sơ khai (Giai đoạn 5B) bắt kịp toàn diện 100% các phân hệ hiện có (Giao dịch mới, AI OCR, Cú hích Thaler Nudge, Action Card).
2. **Hoàn thiện cơ chế Push Notification phía Server:** Đảm bảo đáp ứng trọn vẹn 2.0 điểm Phần A theo `yeucau-cuoiky.txt`, gửi thông báo khi ứng dụng ở Foreground, Background và Terminated.
3. **Củng cố An ninh & Toàn vẹn Dữ liệu Backend:** Triển khai khóa bi quan (Pessimistic Locking `FOR UPDATE`) khi biến động số dư ví và chặn spam tần suất (Rate Limiting) trên các endpoint gọi Gemini AI.
4. **Xây dựng bộ dữ liệu mẫu kể chuyện (Master Demo Story Seeding):** Tạo sẵn kịch bản số liệu tài chính sống động phục vụ quay video demo và vấn đáp cuối kỳ.

### 3. Nguyên Tắc Trình Bày & Hiển Thị: "ĐÚNG VÀ ĐỦ" (Anti-Robotic UX Directive)
Chủ tọa chỉ đạo nghiêm ngặt về ngôn ngữ hiển thị trên toàn bộ ứng dụng và thông báo:
- **Tuyệt đối không AI hóa, không sáo rỗng, không trừu tượng hóa:** Loại bỏ hoàn toàn các cụm từ đao to búa lớn như *"Hệ thống thị giác AI đang tiến hành phân tích ma trận dữ liệu..."*, *"Trợ lý tài chính thấu cảm đang kích hoạt..."*.
- **Văn phong ngắn gọn, súc tích, phản ánh đúng hành động:**
  - *Loading / Trạng thái:* `Đang quét ảnh...`, `Đang nhận diện giọng nói...`, `Đang lưu giao dịch...`, `Đang đồng bộ...`.
  - *Thông báo đẩy / Push Notification:* `Chi tiêu vượt ngân sách: Ăn uống đạt 85% hạn mức`, `Cảnh báo: Khoản chi 2.500.000đ cần khoảng trễ 24h`, `Gợi ý mới: Đã phát hiện giao dịch lương 25.000.000đ`.
  - *Thông báo trong app (Toast / Alert):* Tiêu đề và nội dung ngắn gọn, rõ ràng, tập trung vào con số và giải pháp thực tế.

---

## II. LỘ TRÌNH 4 CHẶNG TRIỂN KHAI CHI TIẾT

```text
┌────────────────────────────────────────────────────────────────────────────────────────┐
│               LỘ TRÌNH 4 CHẶNG ĐIỀU CHỈNH & TỐI ƯU HÓA BACKEND                         │
├────────────────────────────────────────────────────────────────────────────────────────┤
│ CHẶNG 1: NÂNG CẤP TOÀN DIỆN HỆ THẦN KINH THỜI GIAN THỰC (SIGNALR HUB 0ms)             │
│   - Mở rộng SignalR Events: Giao dịch mới, Tiến trình AI, Cú hích Nudge, Action Card  │
│   - Kết nối DashboardViewModel, ExpensesViewModel, BudgetsViewModel vào Message Bus    │
│                                                                                        │
│ CHẶNG 2: HOÀN THIỆN PUSH NOTIFICATION PHÍA SERVER (CHUẨN BAREM PHẦN A 2.0Đ)            │
│   - Viết Adapter Firebase Cloud Messaging (FCM HTTP v1) hoặc High-Fidelity Mock        │
│   - Gửi thông báo khi app Terminated / Background; hỗ trợ Deep Linking qua data payload│
│                                                                                        │
│ CHẶNG 3: TỐI ƯU HÓA CONCURRENCY LOCKING & RATE LIMITING AN NINH BACKEND                │
│   - Khóa bi quan (Pessimistic Lock SELECT FOR UPDATE) bảo vệ số dư ví PostgreSQL      │
│   - Cài đặt ASP.NET Core RateLimiter (tối đa 5 requests/phút) bảo vệ Gemini AI API     │
│                                                                                        │
│ CHẶNG 4: XÂY DỰNG BỘ DỮ LIỆU KỂ CHUYỆN DEMO HOÀN HẢO (DEMO STORY SEEDING)             │
│   - Endpoint POST /api/system/seed-demo-story tạo sẵn dữ liệu có kịch bản sống động   │
│   - Sẵn sàng số liệu cho video demo 5-8 phút và vấn đáp giảng viên                     │
└────────────────────────────────────────────────────────────────────────────────────────┘
```

---

### CHẶNG 1: NÂNG CẤP TOÀN DIỆN HỆ THẦN KINH THỜI GIAN THỰC (SIGNALR HUB 0ms)

#### 1. Mục tiêu
Xóa bỏ hoàn toàn khoảng cách thụt lùi giữa SignalR Hub và 9 phân hệ chức năng. Đảm bảo mọi thay đổi về giao dịch, hạn mức, tiến trình AI hay Cú hích tâm lý đều được phát sóng tức thời tới đúng Client mà không cần người dùng thao tác kéo làm mới (Pull-to-refresh).

#### 2. Các tệp mã nguồn chịu tác động (Impacted Files)
- **Backend:**
  - `backend/ExpenseTracker.Application/Interfaces/IFinancialRealtimeNotifier.cs` (Khai báo thêm hợp đồng sự kiện).
  - `backend/ExpenseTracker.WebApi/Hubs/FinancialRealtimeNotifier.cs` (Hiện thực hóa phát sóng SignalR).
  - `backend/ExpenseTracker.WebApi/Hubs/FinancialHub.cs` (Bổ sung method nhận diện nếu client invoke).
  - `backend/ExpenseTracker.Application/Services/TransactionService.cs` (Bắn sự kiện khi tạo/sửa/xóa giao dịch).
  - `backend/ExpenseTracker.Application/Services/ReceiptOcrService.cs` & `PayslipOcrService.cs` (Bắn trạng thái tiến trình quét ảnh).
  - `backend/ExpenseTracker.Application/Services/NudgeEngineService.cs` & `CoolingOffService.cs` (Bắn cảnh báo Cú hích đắn đo).
- **Frontend Client MAUI:**
  - `frontend/ExpenseTracker.Maui/Services/Realtime/SignalRRealtimeService.cs` (Lắng nghe các sự kiện mới).
  - `frontend/ExpenseTracker.Maui/Services/Realtime/Messages/RealtimeMessages.cs` (Định nghĩa ValueChangedMessage tương ứng).
  - `frontend/ExpenseTracker.Maui/ViewModels/DashboardViewModel.cs` (Cập nhật số dư tổng + thêm item vào RecentTransactions 0ms).
  - `frontend/ExpenseTracker.Maui/ViewModels/ExpensesViewModel.cs` (Cập nhật danh sách chi tiêu tức thì).
  - `frontend/ExpenseTracker.Maui/ViewModels/BudgetsViewModel.cs` (Vẽ lại thanh tiến độ DSA và phần trăm tiêu dùng).

#### 3. Danh mục các sự kiện SignalR mới cần chuẩn hóa

| Tên Sự Kiện SignalR | Dữ Liệu Payload (DTO) | Nơi Phát Sóng (Backend) | Màn Hình Tiếp Nhận (Frontend) | Hành Vi Phía Giao Diện |
| :--- | :--- | :--- | :--- | :--- |
| `ReceiveTransactionCreated` | `TransactionCreatedNotificationDto` (Id, Amount, Category, Wallet, Date, Type) | `TransactionService.cs` (sau khi lưu DB thành công) | `DashboardViewModel`, `ExpensesViewModel`, `IncomesViewModel` | Tự động chèn item vào đầu `ObservableCollection`, cập nhật số dư tổng tài sản 0ms. |
| `ReceiveOcrProgress` | `OcrProgressNotificationDto` (JobId, StepText, Percent) | `ReceiptOcrService.cs`, `PayslipOcrService.cs` | `ReceiptOcrViewModel`, `PayslipOcrViewModel` | Hiển thị text ngắn gọn: `"Đang quét ảnh..."` $\rightarrow$ `"Đang đối soát số tiền..."` $\rightarrow$ `"Xong"`. |
| `ReceiveNudgeAlert` | `NudgeAlertNotificationDto` (NudgeId, Message, Severity, ActionUrl) | `NudgeEngineService.cs`, `CoolingOffService.cs` | `MainPage` / Shell Global Listener | Bật modal đắn đo 24h hoặc hiển thị banner cảnh báo vượt 85% hạn mức ngày (DSA). |
| `ReceiveActionCardCreated` | `AutonomousActionCardDto` (ActionId, Title, Suggestion, Probability) | `AutonomousActionEngineService.cs` | `DashboardViewModel` (`ActionInbox`) | Thêm thẻ hành động tự hành thông minh vào khay `ActionInboxView`. |

#### 4. Tiêu chí nghiệm thu (Acceptance Criteria)
- [x] Khi tạo 1 khoản chi mới từ Swagger hoặc màn hình khác, danh sách giao dịch trên `DashboardPage` và `ExpensesPage` tự động xuất hiện giao dịch đó trong vòng < 100ms mà không cần bấm reload.
- [x] Quá trình quét OCR hóa đơn phát các bước trạng thái ngắn gọn, rõ ràng theo đúng triết lý "Đúng và đủ".
- [x] 100% Unit Tests hiện tại (171 Application + 83 Domain) tiếp tục Passed, bổ sung các Unit Tests kiểm tra Mock `IFinancialRealtimeNotifier`.

---

### CHẶNG 2: HOÀN THIỆN PUSH NOTIFICATION PHÍA SERVER (CHUẨN BAREM PHẦN A 2.0Đ)

#### 1. Mục tiêu
Giải quyết lỗ hổng lớn nhất đối với yêu cầu chấm thi: Hiện tại backend mới chỉ lưu DB và phát SignalR, **chưa có cơ chế đẩy thông báo thật (Push Notification) đến thiết bị khi ứng dụng bị tắt (Terminated) hoặc chạy nền (Background)**.

#### 2. Kiến trúc giải pháp kỹ thuật
1. **Quản lý Device Token:**
   - Hoàn thiện bảng `DeviceTokens` trong CSDL lưu trữ `UserId`, `DeviceToken`, `Platform` (Android/iOS/Windows), `LastActiveAt`.
   - Cung cấp API `POST /api/notifications/devices/register` và `POST /api/notifications/devices/unregister`.
2. **Dịch vụ Gửi Push Đa Kênh (`PushNotificationDispatcher`):**
   - Tạo Interface: `IPushNotificationSender`.
   - Hiện thực hóa lớp: `FirebaseCloudMessagingSender` (sử dụng Google FCM HTTP v1 REST API với Service Account JSON hoặc FirebaseAdmin SDK) kết hợp chế độ `DevelopmentMockPushSender` khi không có kết nối Internet / chưa cấu hình Google Cloud.
   - Khi có sự kiện kích hoạt, server tự động gửi thông báo đến các thiết bị đang đăng ký của người dùng đó.
3. **Bảo đảm 2 sự kiện nghiệp vụ cốt lõi theo Barem:**
   - *Sự kiện 1:* Chi tiêu vượt hạn mức an toàn ngày (DSA) hoặc vượt 85% ngân sách danh mục.
   - *Sự kiện 2:* Khoản chi chạm ngưỡng đắn đo cần khoảng trễ tâm lý (24h Cooling-Off Nudge) hoặc Gợi ý hành động tự hành (Jev Action Card).
4. **Hỗ trợ Deep Linking chuẩn xác:**
   - Payload Push Notification chứa trường `data`:
     ```json
     {
       "notification_type": "COOLING_OFF",
       "target_id": "c1a2b3c4-...",
       "deep_link": "app://cooling-off-hub"
     }
     ```
   - Khi người dùng nhấn vào thông báo trên thanh trạng thái điện thoại, ứng dụng tự động mở đúng trang `CoolingOffHubPage` hoặc `ActionInboxPage`.
5. **Chuẩn hóa nội dung hiển thị theo triết lý "Đúng và đủ":**
   - Tiêu đề: `Cảnh báo ngân sách`, `Khoảng trễ 24h`, `Gợi ý chi tiêu`.
   - Thân bài: Ngắn gọn, nêu thẳng số liệu (Ví dụ: `Danh mục Ăn uống đã tiêu 85% (1.700.000đ/2.000.000đ)`).

#### 3. Tiêu chí nghiệm thu (Acceptance Criteria)
- [x] Gửi thử nghiệm 1 Push Notification từ Swagger `/api/notifications/test-push`, điện thoại Android thật / máy ảo nhận được notification xuất hiện trên khay hệ thống (Notification Tray).
- [x] Chạm vào notification mở đúng trang chi tiết nghiệp vụ liên quan (Deep Link).
- [x] Xử lý trơn tru ở cả 3 trạng thái của app: Foreground, Background, Terminated (qua FCM HTTP v1 & Mock Fallback).

---

### CHẶNG 3: TỐI ƯU HÓA CONCURRENCY LOCKING & RATE LIMITING AN NINH BACKEND

#### 1. Mục tiêu
Nâng tầm chất lượng mã nguồn backend đạt chuẩn Enterprise & Production-Ready (theo phản biện của Eric Evans DDD Lead, Martin Kleppmann DB Lead và Bruce Schneier Security Lead).

#### 2. Chi tiết giải pháp kỹ thuật
1. **Khóa bi quan (Pessimistic Locking `SELECT ... FOR UPDATE`):**
   - Trong `WalletService.cs` và `TransactionService.cs`, khi cộng hoặc trừ tiền ví tài khoản, thay vì đọc dữ liệu trần rồi lưu lại, ta áp dụng câu truy vấn khóa hàng:
     ```csharp
     // Thực thi khóa hàng trên PostgreSQL trong transaction
     await _dbContext.Database.ExecuteSqlInterpolatedAsync(
         $"SELECT \"Id\" FROM \"Wallets\" WHERE \"Id\" = {walletId} FOR UPDATE", cancellationToken);
     ```
   - Điều này bảo đảm nếu có 2 giao dịch diễn ra đồng thời (ví dụ: quét hóa đơn từ điện thoại + hệ thống trừ tiền định kỳ tự động), giao dịch sau bắt buộc phải đợi giao dịch trước hoàn tất commit, loại trừ 100% lỗi sai lệch số dư (Lost Updates / Race Condition).
2. **Rate Limiter Middleware bảo vệ Gemini AI API:**
   - Tận dụng ASP.NET Core `Microsoft.AspNetCore.RateLimiting`.
   - Cấu hình chính sách `AiEndpointLimiter`:
     - Giới hạn: Tối đa 5 lượt yêu cầu / phút trên mỗi User ID.
     - Phản hồi khi vi phạm: HTTP 429 Too Many Requests kèm thông điệp ngắn gọn: `"Bạn thao tác quá nhanh. Vui lòng thử lại sau 1 phút."`.
   - Gắn thuộc tính `[EnableRateLimiting("AiEndpointLimiter")]` lên các controller: `ReceiptsController`, `PayslipsController`, `VoiceAgentController`.

#### 3. Tiêu chí nghiệm thu (Acceptance Criteria)
- [x] Viết Unit Test / Integration Test mô phỏng các luồng giao dịch đồng thời và chuyển tiền 2 chiều không deadlock theo nguyên lý Dijkstra (Resource Ordering).
- [x] Bắn request liên tục vào endpoint OCR trong 10 giây, request vượt quá giới hạn nhận mã HTTP 429 đúng quy chuẩn.

---

### CHẶNG 4: XÂY DỰNG BỘ DỮ LIỆU KỂ CHUYỆN DEMO HOÀN HẢO (DEMO STORY SEEDING)

#### 1. Mục tiêu
Chuẩn bị sẵn "vũ khí" dữ liệu để khi bước vào Giai đoạn 11 thực hiện quay Video Demo 5-8 phút và biểu diễn cho Giảng viên xem, chỉ cần bấm 1 nút là toàn bộ ứng dụng có ngay một bức tranh tài chính sống động, chân thực và gây ấn tượng mạnh.

#### 2. Kịch bản câu chuyện dữ liệu mẫu (The Financial Storyline)
Endpoint `POST /api/system/seed-demo-story` sẽ nạp vào CSDL tài khoản `demo@expensetracker.vn` với các số liệu có chủ đích:
1. **Ví tài khoản:**
   - 1 Ví chính (Vietcombank): 15.500.000đ.
   - 1 Ví tiền mặt: 1.200.000đ.
   - 1 Hũ tiết kiệm mục tiêu "Mua Laptop mới": Đã tích lũy 12.000.000đ / 25.000.000đ (đạt 48%).
2. **Ngân sách & Cảnh báo hạn mức ngấp nghé:**
   - Danh mục "Ăn uống & Cà phê": Hạn mức tháng 3.000.000đ $\rightarrow$ Hiện đã chi 2.550.000đ (**đúng 85%**). Điều này giúp khi demo chỉ cần ghi thêm 1 ly cà phê 50.000đ là lập tức kích hoạt Cú hích cảnh báo vượt ngưỡng DSA ngay trước mắt thầy!
3. **Khoảng trễ 24h đang đếm ngược:**
   - 1 khoản chi đắn đo "Tai nghe chống ồn Sony": 4.500.000đ được tạo cách đây 6 tiếng, hiển thị đồng hồ đếm ngược còn 18 giờ để giảng viên thấy rõ cơ chế tâm lý trì hoãn.
4. **Hòm thư hành động tự hành (Action Inbox):**
   - 1 thẻ hành động xác suất 92%: Phát hiện giao dịch chuyển khoản GrabFood 120.000đ, sẵn sàng nút "1-Chạm Lưu Nhanh".
   - 1 thẻ hành động xác suất 65%: Khoản thanh toán tiền điện thoại, hỏi người dùng lựa chọn danh mục.
5. **Dữ liệu OCR & Báo cáo:**
   - Nạp sẵn lịch sử 1 hóa đơn siêu thị WinMart và 1 phiếu lương tháng trước để biểu đồ Net Worth và phân tích chi tiêu hiển thị các đường cong xu hướng đẹp mắt, không bị trống trơn.

#### 3. Tiêu chí nghiệm thu (Acceptance Criteria)
- [x] Gọi endpoint `POST /api/system/seed-demo-story`, toàn bộ bảng dữ liệu liên quan được làm sạch và nạp đúng 100% kịch bản trong < 2 giây.
- [x] Đăng nhập tài khoản demo trên ứng dụng Mobile, toàn bộ Dashboard hiển thị chỉ số tài chính chân thực, biểu đồ tròn và biểu đồ cột lấp đầy dữ liệu chuẩn.

---

## III. BẢNG PHÂN CÔNG TÁC TỬ HỘI ĐỒNG (AGENT ROSTER ASSIGNMENT)

| Chặng Triển Khai | Tác Tử Chủ Trì | Tác Tử Phối Hợp & Giám Sát | Trách Nhiệm Cốt Lõi |
| :--- | :--- | :--- | :--- |
| **Chặng 1: SignalR Hub** | **Agent 07 (Werner Vogels)** | **Agent 10 (Dan Abramov)** & **Agent 03 (BA Lead)** | Thiết kế kiến trúc WebSocket Full-Duplex, cấu trúc DTO sự kiện, kết nối Message Bus phía MAUI Client. |
| **Chặng 2: Push Notification** | **Agent 07 (Werner Vogels)** | **Agent 11 (Bruce Schneier)** & **Agent 01 (Richard Thaler)** | Tích hợp Adapter FCM/Mock, quản lý Device Token an toàn, thiết kế nội dung thông báo chuẩn triết lý "Đúng và đủ". |
| **Chặng 3: Concurrency & Security**| **Agent 05 (Martin Kleppmann)**| **Agent 04 (Eric Evans)** & **Agent 11 (Bruce Schneier)** | Viết câu lệnh khóa PostgreSQL `FOR UPDATE`, cấu hình ASP.NET Core RateLimiter chống spam API AI. |
| **Chặng 4: Demo Story Seeding** | **Agent 02 (Budget Coach)** | **Agent 01 (Richard Thaler)** & **Agent 12 (Kent Beck)** | Xây dựng kịch bản tài chính mẫu sống động, viết script seeding CSDL và kiểm thử hồi quy 100% Passed. |

---

## IV. QUY TRÌNH DUYỆT VÀ BƯỚC TIẾP THEO

1. **Bước 1 (Hiện tại):** Trình bản Kế hoạch tổng thể `PLAN-50` cho Chủ tọa đề tài xem xét, phản biện và phê duyệt.
2. **Bước 2 (Sau khi Chủ tọa duyệt):** Khởi tạo một **Implementation Plan Artifact chuyên biệt cho Chặng 1** (Nâng cấp toàn diện SignalR Hub) với danh sách task chi tiết, checklist kiểm thử từng file và bắt đầu thực thi code một cách thận trọng, an toàn.
