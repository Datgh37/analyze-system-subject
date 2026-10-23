# BÁO CÁO THIẾT KẾ KỸ THUẬT: MÔ HÌNH ĐỘNG VÀ TƯƠNG TÁC HỆ THỐNG (UML 2.5 DYNAMIC MODELING)
## HỆ THỐNG QUẢN LÝ CHI TIÊU VÀ HOẠCH ĐỊNH TÀI CHÍNH CÁ NHÂN THÔNG MINH (PFM)

> **Mục tiêu Khóa luận:** Cung cấp toàn văn nội dung học thuật chuẩn mực để tích hợp trực tiếp vào **Mục 3.4 (Thiết kế mô hình động và tương tác hệ thống)** trong Chương 3 của Khóa luận Tốt nghiệp ([`KhoaLuanTotNghiep.docx`](../../files/KhoaLuanTotNghiep.docx)).  
> **Căn cứ thiết kế:** Chuẩn quốc tế **UML 2.5 (OMG Unified Modeling Language)** và các nguyên lý Kiến trúc Hướng miền (Domain-Driven Design).  
> **Tập hợp sơ đồ nguồn:** [`files/SequenceDiagram.drawio`](../../files/SequenceDiagram.drawio), [`files/ActivityDiagram.drawio`](../../files/ActivityDiagram.drawio), và [`files/StateDiagram.drawio`](../../files/StateDiagram.drawio).

---

# 3.4 Thiết kế mô hình động và tương tác hệ thống

Nếu như Sơ đồ Lớp (Class Diagram) và các Ranh giới Ngữ cảnh (Bounded Contexts) ở các mục trước phản ánh khía cạnh cấu trúc tĩnh (Static View) của hệ thống—bao gồm các thực thể dữ liệu, thuộc tính, phương thức và mối quan hệ cấu trúc giữa các đối tượng—thì Mô hình Động (Dynamic Model) giữ vai trò then chốt trong việc mô tả hành vi thời gian chạy (Runtime Behavioral View) của hệ thống phần mềm. Trong thực tế vận hành của một hệ thống quản lý tài chính cá nhân thông minh, các đối tượng không tồn tại biệt lập mà liên tục phối hợp, trao đổi thông điệp, thực thi các quy trình nghiệp vụ phức tạp và chuyển đổi trạng thái nhằm đáp ứng tức thì các hành vi tài chính của người dùng.

Để phác họa toàn diện bức tranh động lực học của hệ thống, chuẩn mô hình hóa UML 2.5 cung cấp ba công cụ biểu diễn chuyên sâu tương ứng với ba góc nhìn bổ trợ lẫn nhau:
- **Góc nhìn tương tác hướng đối tượng (Inter-object Interaction):** Được thể hiện qua **Sơ đồ Tuần tự (Sequence Diagram)**, tập trung làm rõ trình tự thời gian của các thông điệp trao đổi giữa các thành phần giao diện, các tầng dịch vụ ứng dụng, các đối tượng nghiệp vụ miền và cơ sở dữ liệu để hiện thực hóa từng kịch bản ca sử dụng cốt lõi.
- **Góc nhìn luồng công việc nghiệp vụ (Business Workflow):** Được thể hiện qua **Sơ đồ Hoạt động (Activity Diagram)**, tập trung làm rõ các bước xử lý liên tục, các điểm rẽ nhánh điều kiện và sự phân công trách nhiệm rõ ràng giữa các bên liên quan thông qua cơ chế phân vùng rãnh bơi (Swimlanes).
- **Góc nhìn vòng đời thực thể miền (Entity Lifecycle):** Được thể hiện qua **Sơ đồ Trạng thái (State Machine Diagram)**, tập trung theo dõi các trạng thái tồn tại hữu hạn, các biến cố kích hoạt và các điều kiện ràng buộc bảo đảm tính toàn vẹn dữ liệu của các thực thể cốt lõi trong suốt chu kỳ sống.

---

## 3.4.1 Thiết kế tương tác và Sơ đồ tuần tự hệ thống (Sequence Diagrams)

Sơ đồ Tuần tự (Sequence Diagram) là một trong những biểu đồ tương tác quan trọng nhất của ngôn ngữ UML, biểu diễn thứ tự theo trục thời gian (từ trên xuống dưới) của các lời gọi phương thức và thông điệp truyền thông giữa các đối tượng tham gia vào một ca sử dụng. Cấu trúc chuẩn của một sơ đồ tuần tự trong hệ thống bao gồm:
- **Đường đời đối tượng (Lifeline):** Biểu diễn sự tồn tại của một đối tượng cụ thể trong suốt phiên tương tác, kéo dài theo phương thẳng đứng.
- **Thanh kích hoạt (Activation Bar):** Thể hiện khoảng thời gian một đối tượng trực tiếp nắm quyền điều khiển và thực thi tác vụ xử lý.
- **Hệ thống thông điệp (Messages):** Phân định rạch ròi giữa lời gọi hàm đồng bộ (Synchronous Message - mũi tên nét liền đầu tam giác đặc), lời gọi bất đồng bộ không phong tỏa (Asynchronous Message - mũi tên nét liền đầu mở) và thông điệp trả lời/phản hồi kết quả (Reply/Return Message - mũi tên nét đứt).
- **Các khối tương tác kết hợp (Combined Fragments):** Sử dụng chuẩn cú pháp UML 2.5 với toán tử điều kiện `alt` (rẽ nhánh điều kiện loại trừ tương tự if-else), `opt` (khối xử lý tùy chọn chỉ thực thi khi thỏa mãn điều kiện) và `loop` (vòng lặp duyệt danh sách phần tử).

Hệ thống lựa chọn thiết kế 5 sơ đồ tuần tự đại diện cho 5 kịch bản tương tác quan trọng và phức tạp nhất, bao quát từ bóc tách hóa đơn AI, xử lý giọng nói, giao dịch thanh toán tài chính đến các thuật toán kinh tế hành vi.

---

### Kịch bản 1: Sơ đồ tuần tự Bóc tách Hóa đơn AI Vision OCR và Hậu kiểm toán Số học (SD-01)

Kịch bản SD-01 hiện thực hóa ca sử dụng bóc tách hóa đơn tự động (UC-03.02) và hậu kiểm toán đối soát số học (UC-03.03). Đây là quy trình phối hợp bất đồng bộ kết hợp giữa tầng Client trên thiết bị di động, Dịch vụ bóc tách hóa đơn tại Backend, Mô hình thị giác AI Cloud và Cơ sở dữ liệu quan hệ.

```text
[Người dùng] ──> [Mobile UI] ──> [ReceiptService] ──> [AI Cloud] ──> [MathAudit] ──> [Database]
```

* **Trình tự tương tác chi tiết:**
  1. Người dùng kích hoạt máy ảnh trên ứng dụng di động để chụp ảnh hóa đơn mua sắm siêu thị hoặc hóa đơn dịch vụ, sau đó xác nhận gửi ảnh.
  2. `Mobile UI` tiếp nhận tệp ảnh, thực hiện tiền xử lý nén cục bộ nhằm tối ưu băng thông mạng và gửi yêu cầu tải lên bất đồng bộ `POST /api/v1/receipts/scan` kèm tệp ảnh tới `ReceiptService`.
  3. `ReceiptService` tiếp nhận yêu cầu, cấp phát mã định danh giao dịch nháp (`ReceiptId`) và gửi trả mã trạng thái `HTTP 202 Accepted` về client để không làm đóng băng giao diện người dùng, đồng thời đẩy tệp ảnh vào hàng đợi xử lý ngầm.
  4. Bộ điều phối hàng đợi gọi tới dịch vụ `VisionLLMCloud` thông qua phương thức `analyzeReceipt(imageBytes, jsonSchemaPrompt)`. Tại đây, mô hình thị giác áp dụng kỹ thuật Prompt Engineering có cấu trúc để trích xuất hóa đơn thành định dạng JSON chuẩn gồm: Tên đơn vị bán lẻ, ngày giao dịch, mảng danh mục các món hàng (`ItemName`, `Quantity`, `UnitPrice`, `LineTotal`), tiền thuế VAT và tổng tiền thanh toán ghi trên hóa đơn (`RawTotal`).
  5. Khi nhận được payload JSON từ AI Cloud, `ReceiptService` khởi tạo thực thể nghiệp vụ và gọi phương thức kiểm toán `auditArithmeticSum(extractedData)` thuộc lớp dịch vụ miền `MathAuditEngine`.
  6. `MathAuditEngine` duyệt vòng lặp qua toàn bộ danh sách các món hàng để tính toán tổng tiền lũy kế:
     $$\text{CalculatedSum} = \sum_{i=1}^{n} (\text{Quantity}_i \times \text{UnitPrice}_i) + \text{TaxAmount} - \text{DiscountAmount}$$
     Sau đó xác định độ lệch số học $\Delta = |\text{CalculatedSum} - \text{RawTotal}|$.
  7. Khung điều kiện `alt` xử lý rẽ nhánh đối soát:
     - **Nhánh hợp lệ ($\Delta = 0$):** Hệ thống đánh giá dữ liệu hóa đơn đạt độ tin cậy tuyệt đối, tự động khớp các món hàng vào cây Danh mục chi tiêu chuẩn và lưu bản ghi vào `Database` với trạng thái `VERIFIED`.
     - **Nhánh nghi vấn ($\Delta > 0$):** Hệ thống đánh dấu cờ cảnh báo sai lệch (`ARITHMETIC_MISMATCH_SUSPECT`), lưu bản ghi ở trạng thái `DRAFT` và gửi thông báo đẩy về `Mobile UI` kèm dữ liệu trực quan để người dùng kiểm tra và hiệu chỉnh thủ công các dòng tiền bị nhận dạng sai trước khi chính thức ghi sổ.

***Hình 3.18:*** *Sơ đồ tuần tự bóc tách hóa đơn AI Vision OCR và hậu kiểm toán số học*

---

### Kịch bản 2: Sơ đồ tuần tự Ghi nhận Chi tiêu bằng Giọng nói Thông minh (SD-02)

Kịch bản SD-02 hiện thực hóa tính năng ghi chép chi tiêu rảnh tay thông qua giọng nói tự nhiên (Voice-to-Expense), giúp triệt tiêu hoàn toàn rào cản nhập liệu thủ công—vốn là nguyên nhân hàng đầu khiến người dùng từ bỏ ứng dụng quản lý tài chính.

```text
[Người dùng] ──> [Mobile UI] ──> [VoiceService] ──> [Whisper AI] ──> [NLPExtractor] ──> [Expense]
```

* **Trình tự tương tác chi tiết:**
  1. Người dùng nhấn giữ nút ghi âm trên màn hình chính và đọc khẩu ngữ tự nhiên (Ví dụ: *"Vừa ăn phở bò sáng nay hết 45 nghìn trả bằng tiền mặt"*).
  2. `Mobile UI` thu nhận luồng âm thanh dạng sóng (`AudioBuffer`), chuyển đổi sang định dạng PCM/WAV và đẩy dữ liệu tới `VoiceExpenseService` qua kết nối an toàn.
  3. `VoiceExpenseService` chuyển tiếp luồng âm thanh tới dịch vụ chuyển đổi giọng nói thành văn bản `SpeechToTextCloud` (sử dụng mô hình Whisper) để thu về chuỗi văn bản hoàn chỉnh.
  4. Chuỗi văn bản thô được chuyển tiếp sang bộ phân tích ngữ nghĩa chuyên biệt `NLPExtractorEngine`. Động cơ này áp dụng các mẫu biểu thức chính quy (Regex) và mô hình ngôn ngữ nhỏ được tinh chỉnh (Fine-tuned Small LLM) nhằm bóc tách 4 thực thể thông tin cốt lõi (Named Entity Recognition - NER):
     - Số tiền giao dịch: `45,000 VND`.
     - Nhóm danh mục chi tiêu: `Ăn uống / Bữa sáng`.
     - Ví thanh toán sử dụng: `Ví tiền mặt`.
     - Thời gian phát sinh: Ngày và giờ hiện tại.
  5. Bộ phân tích tính toán điểm số tin cậy ngữ nghĩa (`ConfidenceScore`). Khung điều kiện `alt` xử lý rẽ nhánh:
     - **Nếu $\text{ConfidenceScore} \ge 0.85$:** Dịch vụ tự động tạo thực thể giao dịch hợp lệ, lưu vào CSDL và phản hồi form xác nhận đã điền đầy đủ dữ liệu lên màn hình `Mobile UI` trong vòng chưa đầy 1 giây.
     - **Nếu $\text{ConfidenceScore} < 0.85$ (thiếu dữ liệu ví hoặc số tiền không rõ ràng):** Hệ thống giữ lại các thông tin đã nhận dạng được, đồng thời mở hộp thoại gợi ý thông minh trên giao diện để người dùng chọn bổ sung thông tin còn thiếu chỉ với một thao tác chạm.

***Hình 3.19:*** *Sơ đồ tuần tự ghi nhận chi tiêu bằng giọng nói thông minh*

---

### Kịch bản 3: Sơ đồ tuần tự Thanh toán VietQR và Hạch toán Số dư Đa ví ACID (SD-03)

Kịch bản SD-03 hiện thực hóa quy trình thanh toán liên ngân hàng và trích xuất giao dịch thông qua tiêu chuẩn VietQR (Napas 247), bảo đảm tính toàn vẹn giao dịch tài chính tuyệt đối theo chuẩn ACID (Atomicity, Consistency, Isolation, Durability).

```text
[Người dùng] ──> [Mobile UI] ──> [PaymentService] ──> [VietQR/Napas] ──> [WalletManager] ──> [Ledger]
```

* **Trình tự tương tác chi tiết:**
  1. Người dùng thực hiện quét mã VietQR tĩnh hoặc động tại quầy thu ngân thông qua camera ứng dụng.
  2. `Mobile UI` giải mã chuỗi ký tự chuẩn EMVCo Merchant-Presented Mode để trích xuất các trường thông tin: Mã ngân hàng thụ hưởng (BIN Code), Số tài khoản người nhận, Số tiền yêu cầu và Nội dung thanh toán.
  3. Sau khi người dùng xác nhận nguồn tiền thanh toán (chọn Ví Ngân hàng hoặc Ví Tín dụng), `PaymentService` khởi tạo một phiên giao dịch chuyển khoản với khóa phân tán (Distributed Lock) trên Redis nhằm triệt tiêu nguy cơ trùng lặp giao dịch (Idempotency Key).
  4. Hệ thống mở kết nối giao dịch cấp CSDL (`Begin Transaction`) với mức cô lập `Read Committed` (hoặc `Serializable` đối với ví dùng chung nhiều phiên) và gọi phương thức kiểm tra số dư khả dụng `checkAvailableBalance()` trên thực thể ví nguồn.
  5. Khung điều kiện `alt` xử lý an toàn số dư:
     - **Nếu số dư khả dụng không đủ:** Hệ thống thực hiện hủy phiên (`Rollback Transaction`) và gửi thông báo từ chối thanh toán tức thì về giao diện kèm gợi ý chuyển đổi ví nguồn.
     - **Nếu số dư hợp lệ:** Hệ thống thực hiện đồng thời hai thao tác ghi sổ kép:
       - Trừ số dư trên thực thể `SourceWallet`.
       - Khởi tạo bản ghi giao dịch chi tiêu mới `ExpenseTransaction` liên kết với mã đối soát VietQR và ghi nhận vào sổ cái `LedgerEntry`.
  6. Lệnh `Commit Transaction` được thực thi thành công, khóa phân tán được giải phóng và thông báo giao dịch thành công hiển thị trên điện thoại người dùng cùng hóa đơn điện tử đính kèm.

***Hình 3.20:*** *Sơ đồ tuần tự thanh toán VietQR và hạch toán số dư đa ví*

---

### Kịch bản 4: Sơ đồ tuần tự Động cơ Hạn mức DSA Thích ứng và Kích hoạt Cú hích Thaler Nudge (SD-04)

Kịch bản SD-04 mô tả quá trình vận hành liên tục của Động cơ Hạn mức Chi tiêu Mỗi ngày Thích ứng (Adaptive Daily Spending Allowance - DSA Engine) và hệ thống Cú hích Hành vi (Behavioral Nudge Engine) dựa trên nguyên lý Kinh tế học Hành vi của Giáo sư Richard Thaler.

```text
[ExpenseEvent] ──> [DSAEngine] ──> [SpendingVelocity] ──> [NudgeService] ──> [PushGateway] ──> [User]
```

* **Trình tự tương tác chi tiết:**
  1. Ngay khi một giao dịch chi tiêu mới được ghi nhận thành công, sự kiện miền `ExpenseCreatedEvent` được phát ra trên bus sự kiện nội bộ.
  2. `DSAEngine` bắt sự kiện và truy vấn ngân sách khả dụng còn lại của kỳ hiện tại ($R_{\text{remain}}$) cùng số ngày còn lại đến chu kỳ lương tiếp theo ($D_{\text{remain}}$).
  3. Động cơ tính toán lại hạn mức chi tiêu định mức cho ngày hôm nay theo công thức thích ứng:
     $$\text{DSA}_{\text{today}} = \frac{R_{\text{remain}}}{D_{\text{remain}}}$$
  4. `DSAEngine` chuyển tiếp thông số tới `SpendingVelocityService` để tính toán Chỉ số Tốc độ Tiêu tiền tức thời ($V$):
     $$V = \frac{\text{Tổng chi tiêu thực tế tích lũy từ đầu ngày}}{\text{DSA}_{\text{today}}}$$
  5. Khung điều kiện `alt` đánh giá chỉ số vận tốc $V$:
     - **Trường hợp an toàn ($V \le 1.0$):** Hệ thống ghi nhận tiến độ dòng tiền bình thường, cập nhật thanh dải màu xanh trên Dashboard người dùng mà không gửi bất kỳ thông báo phiền nhiễu nào.
     - **Trường hợp cảnh báo sớm ($1.0 < V \le 1.2$):** Hệ thống chuyển trạng thái dải màu sang vàng cam, phản ánh người dùng đã tiêu hết định mức một ngày nhưng chưa gây tổn hại nghiêm trọng đến quỹ tháng.
     - **Trường hợp vi phạm nghiêm trọng ($V > 1.2$ hoặc chi tiêu vượt 85% DSA):** `SpendingVelocityService` gửi tín hiệu cảnh báo khẩn cấp sang `BehavioralNudgeService`.
  6. `BehavioralNudgeService` kích hoạt thuật toán lựa chọn cú hích kiến trúc (Choice Architecture):
     - Tạo thông điệp can thiệp vi mô (Micro-intervention) mang tính khích lệ, phi cưỡng chế (Ví dụ: *"Bạn đã tiêu 140% định mức ngày hôm nay. Nếu giữ tốc độ này, ngân sách tháng sẽ cạn kiệt trước ngày 22. Hãy cân nhắc nấu ăn tại nhà tối nay nhé!"*).
     - Chuẩn bị dữ liệu minh bạch thuật toán (Explainable AI - XAI) giải trình rõ công thức tính và ngày cạn kiệt tiền dự kiến nếu duy trì đà chi tiêu.
  7. Dịch vụ gọi `PushNotificationGateway` (Firebase Cloud Messaging) để bắn thông báo dạng biểu ngữ (Banner Toast) lên thiết bị di động của người dùng ngay tại thời điểm nhạy cảm của quyết định chi tiêu (Just-in-Time Nudge).

***Hình 3.21:*** *Sơ đồ tuần tự tính toán hạn mức DSA và kích hoạt cú hích hành vi*

---

### Kịch bản 5: Sơ đồ tuần tự Kiểm soát Chi tiêu Lớn với Khoảng trễ Tâm lý 24 giờ (SD-05)

Kịch bản SD-05 hiện thực hóa cơ chế Khoảng trễ Tâm lý (Cooling-off Period) từ 24 đến 48 giờ đối với các quyết định mua sắm tùy chọn có giá trị lớn, áp dụng lý thuyết Hệ thống 1 (Cảm xúc bốc đồng) và Hệ thống 2 (Tư duy phản biện chậm) của Daniel Kahneman.

```text
[Người dùng] ──> [Mobile UI] ──> [CoolingOffManager] ──> [CronScheduler] ──> [AlertService]
```

* **Trình tự tương tác chi tiết:**
  1. Người dùng nhập thông tin một khoản chi tiêu mua sắm tùy chọn (Ví dụ: Mua đồng hồ thông minh hoặc thiết bị công nghệ) với số tiền vượt ngưỡng quy định (mặc định > 3.000.000 VNĐ hoặc chiếm trên 50% quỹ chi tiêu tùy ý Wants).
  2. `CoolingOffManager` tiếp nhận yêu cầu, phân tích tác động tài chính:
     - Tính toán quy đổi số tiền mua sắm thành số giờ công lao động thực tế dựa trên mức lương thực nhận: $\text{Số giờ làm việc} = \frac{\text{Số tiền chi mua}}{\text{Mức lương mỗi giờ}}$.
     - Tính toán mức sụt giảm của hạn mức DSA các ngày còn lại trong tháng.
  3. Thay vì trừ tiền ví ngay lập tức, hệ thống hiển thị màn hình đối thoại phản chiếu tâm lý (Reflection Modal), đưa ra dữ liệu quy đổi giờ công và đề xuất người dùng đưa khoản chi vào **Khoảng trễ tâm lý 24 giờ**.
  4. Khi người dùng đồng ý kích hoạt khoảng trễ, `CoolingOffManager` tạo bản ghi giao dịch ở trạng thái chờ đặc biệt `PENDING_COOLING_OFF`, ghi nhận mốc thời gian mở khóa `coolingOffUntil = Now() + 24 Hours`, và đăng ký một lịch trình nhắc nhở với `CronSchedulerService`. Tiền trong ví thanh toán hoàn toàn **chưa bị trích trừ**.
  5. Ứng dụng di động hiển thị đồng hồ đếm ngược 24:00:00 và tạm thời ẩn nút "Thanh toán ngay".
  6. Sau khi hết 24 giờ, tiến trình ngầm `CronSchedulerService` quét thấy hạn khóa đã mãn, tự động kích hoạt `AlertService` gửi thông báo đẩy mời người dùng mở lại ứng dụng để đưa ra quyết định lý trí cuối cùng (Human-in-the-loop):
     - Khung điều kiện `alt`:
       - **Lựa chọn 1 (Hủy mua sắm):** Người dùng nhận thấy khoản chi không thực sự cần thiết và bấm "Hủy bỏ". Giao dịch chuyển sang trạng thái `CANCELLED`, hệ thống hiển thị lời chúc mừng người dùng đã bảo toàn thành công số tiền và tự động chuyển số tiền này vào gợi ý nạp Quỹ Mục Tiêu Tích Lũy.
       - **Lựa chọn 2 (Tiếp tục mua):** Người dùng xác nhận đây là nhu cầu thiết yếu đã được cân nhắc kỹ. Hệ thống chuyển giao dịch sang trạng thái `COMPLETED`, thực thi lệnh trừ tiền ví và ghi sổ cái bình thường.

***Hình 3.22:*** *Sơ đồ tuần tự kiểm soát chi tiêu lớn với khoảng trễ tâm lý 24 giờ*

---

## 3.4.2 Thiết kế luồng quy trình nghiệp vụ với Sơ đồ hoạt động (Activity Diagrams)

Nếu như Sơ đồ Tuần tự làm nổi bật các thông điệp trao đổi giữa các thành phần đối tượng kỹ thuật, thì Sơ đồ Hoạt động (Activity Diagram) trong UML 2.5 lại giữ vai trò mô hình hóa toàn diện luồng công việc nghiệp vụ (Business Process Workflow). Sơ đồ hoạt động thể hiện sự biến chuyển liên tục từ hoạt động này sang hoạt động khác, mô tả các điểm rẽ nhánh quyết định logic, các phân đoạn xử lý đồng thời và các điểm kết thúc của quy trình.

Đặc biệt, để làm rõ ranh giới trách nhiệm kiến trúc, toàn bộ các sơ đồ hoạt động trong hệ thống đều được thiết kế theo chuẩn **Rãnh bơi dọc (Vertical Swimlanes / Partitions)**. Mỗi rãnh bơi đại diện cho một bên chịu trách nhiệm độc lập:
- Rãnh bơi **Người dùng cá nhân (Personal User):** Thể hiện các hành vi tương tác vật lý trực tiếp của con người (chụp ảnh, đọc giọng nói, xác nhận form, ra quyết định tài chính).
- Rãnh bơi **Giao diện Ứng dụng Di động (Mobile Client UI):** Chịu trách nhiệm hiển thị trực quan, thu nhận sự kiện, tiền xử lý dữ liệu và phản hồi kết quả tới người dùng.
- Rãnh bơi **Dịch vụ Backend (Application & Domain Services):** Chịu trách nhiệm thực thi các quy tắc nghiệp vụ, tính toán giải thuật, điều phối hàng đợi và kiểm soát giao dịch.
- Rãnh bơi **Dịch vụ Đám mây / Ngoại vi (AI Cloud Services & Gateway):** Đóng vai trò xử lý các tác vụ tính toán chuyên sâu (Vision LLM OCR, Speech-to-Text, Push Notification Gateway).
- Rãnh bơi **Cơ sở Dữ liệu & Lưu trữ (Database & Cache Layer):** Chịu trách nhiệm bảo đảm tính bền vững dữ liệu, quản lý khóa giao dịch và kiểm soát toàn vẹn tham chiếu.

---

### Quy trình 1: Sơ đồ hoạt động Bóc tách Hóa đơn AI OCR và Hậu kiểm toán Số học (AD-01)

Sơ đồ AD-01 mô hình hóa toàn bộ chu trình xử lý của chức năng số hóa hóa đơn tự động qua 5 rãnh bơi độc lập, từ khâu tiếp nhận ảnh thô đến khi hoàn tất hạch toán vào sổ cái CSDL.

```text
[Người dùng] ──> [Mobile UI] ──> [Backend Service] ──> [AI Cloud] ──> [CSDL PostgreSQL]
```

* **Phân tích chi tiết các bước xử lý:**
  1. *Khởi tạo luồng (Initial Node):* Bắt đầu tại rãnh bơi `Người dùng` bằng hành động chụp ảnh hóa đơn mua sắm.
  2. *Tiền xử lý tại Client:* Rãnh bơi `Mobile UI` tiếp nhận ảnh, thực hiện kiểm tra độ nét và kích thước tệp, sau đó đóng gói tải lên dịch vụ Backend.
  3. *Tiếp nhận & Cấp phát:* `Backend Service` tiếp nhận tệp ảnh, khởi tạo bản ghi giao dịch ở trạng thái sơ khởi `DRAFT` và phát sinh lời gọi bất đồng bộ sang rãnh bơi `AI Cloud`.
  4. *Trích xuất nhận dạng AI:* `AI Cloud` kích hoạt mô hình Vision LLM kết hợp OCR đa ngôn ngữ, phân tích tọa độ các khối văn bản (Bounding Boxes) để cấu trúc hóa hóa đơn thành danh mục các món hàng, đơn giá, số lượng, tiền thuế và tổng tiền.
  5. *Động cơ Hậu kiểm toán số học:* Dữ liệu cấu trúc được chuyển giao về `Backend Service` để thực thi thuật toán kiểm tra tính nhất quán toán học. Hệ thống tính tổng tiền độc lập từ các dòng chi tiết món và so sánh với tổng tiền in trên hóa đơn để xác định sai số $\Delta$.
  6. *Điểm rẽ nhánh quyết định (Decision Node):*
     - **Nhánh Khớp số học ($\Delta = 0$):** Hệ thống đánh giá dữ liệu đạt độ tin cậy tuyệt đối, chuyển trạng thái sang `VERIFIED`, tự động phân loại danh mục chi tiêu và gửi bản ghi xuống rãnh bơi `CSDL PostgreSQL` để lưu trữ bền vững và khấu trừ số dư ví thanh toán.
     - **Nhánh Lệch số học ($\Delta > 0$):** Hệ thống đánh dấu cờ nghi vấn sai lệch nhận dạng, đóng gói dữ liệu phản hồi về rãnh bơi `Mobile UI`. Giao diện di động bật màn hình đối soát tương tác, làm nổi bật các dòng hóa đơn có dấu hiệu nhận dạng sai bằng dải màu cảnh báo vàng-cam để `Người dùng` trực tiếp kiểm tra và hiệu chỉnh thủ công các con số chỉ trong vài giây trước khi nhấn nút "Xác nhận ghi sổ".
  7. *Kết thúc luồng (Activity Final Node):* Bản ghi giao dịch chính thức được lưu vào CSDL, hoàn tất ca sử dụng.

***Hình 3.23:*** *Sơ đồ hoạt động quy trình bóc tách hóa đơn AI Vision OCR và kiểm toán số học*

---

### Quy trình 2: Sơ đồ hoạt động Kiểm soát Mua sắm Lớn và Khoảng trễ Tâm lý Cooling-off 24h (AD-02)

Sơ đồ AD-02 mô hình hóa quy trình can thiệp hành vi có ý thức đối với các khoản chi tiêu lớn vượt hạn mức, phối hợp nhịp nhàng giữa 4 rãnh bơi: `Người dùng cá nhân`, `Mobile App Client`, `Backend Cooling-off Manager` và `Cron Scheduler Worker`.

```text
[Người dùng] ──> [Mobile App] ──> [Backend Manager] ──> [Cron Scheduler]
```

* **Phân tích chi tiết các bước xử lý:**
  1. *Khởi tạo luồng:* Bắt đầu khi `Người dùng` nhập một khoản chi tiêu mua sắm tùy chọn (Shopping / Luxury).
  2. *Thẩm định ngưỡng giá trị (Decision Node):* `Backend Manager` kiểm tra quy tắc: Số tiền chi tiêu có vượt ngưỡng an toàn (> 3.000.000 VNĐ hoặc > 50% quỹ chi tiêu tùy ý Wants) hay không?
     - **Nếu Không vượt ngưỡng:** Giao dịch được ghi nhận bình thường, trừ tiền ví tức thì và kết thúc quy trình.
     - **Nếu Vượt ngưỡng:** Hệ thống kích hoạt cơ chế can thiệp tâm lý.
  3. *Quy đổi giá trị & Phản chiếu tâm lý:* `Backend Manager` tính toán số giờ làm việc cần thiết để bù đắp khoản chi tiêu này dựa trên thu nhập thực tế của người dùng, đồng thời dự báo mức sụt giảm của hạn mức DSA trong những ngày còn lại.
  4. *Kích hoạt Khoảng trễ:* `Mobile App Client` hiển thị hộp thoại cảnh báo phản chiếu tâm lý. Khi người dùng đồng ý áp dụng khoảng trễ, `Backend Manager` lưu giao dịch ở trạng thái khóa tạm thời `PENDING_COOLING_OFF` (tiền trong ví chưa bị trừ) và thiết lập thời hạn khóa 24 giờ.
  5. *Đếm ngược & Nhắc nhở định kỳ:* `Mobile App` hiển thị đồng hồ đếm ngược trực quan. Phía ngầm, rãnh bơi `Cron Scheduler Worker` chịu trách nhiệm theo dõi thời gian thực.
  6. *Quyết định sau 24 giờ (Decision Node):* Khi hết hạn 24 giờ, tiến trình Cron bắn thông báo mời người dùng mở lại giao diện để xác nhận quyết định cuối cùng:
     - **Nhánh Hủy bỏ giao dịch:** Người dùng nhận ra đây là ham muốn nhất thời và bấm "Hủy bỏ". Giao dịch chuyển sang trạng thái `CANCELLED`, hệ thống lưu trữ bài học tiết kiệm thành công và đề xuất chuyển số tiền này vào Quỹ Mục Tiêu Tích Lũy.
     - **Nhánh Xác nhận tiếp tục mua:** Người dùng khẳng định đây là nhu cầu thực sự đã được suy xét chín chắn. Giao dịch chuyển sang trạng thái `COMPLETED`, tiền được khấu trừ khỏi ví và ghi nhận vào lịch sử chi tiêu.
  7. *Kết thúc luồng:* Cả hai nhánh đều dẫn về nút kết thúc quy trình một cách an toàn và minh bạch.

***Hình 3.24:*** *Sơ đồ hoạt động quy trình kiểm soát mua sắm lớn với khoảng trễ tâm lý 24 giờ*

---

### Quy trình 3: Sơ đồ hoạt động Tính toán Hạn mức DSA và Phát tín hiệu Cú hích Nudge (AD-03)

Sơ đồ AD-03 mô hình hóa luồng công việc liên tục của động cơ kinh tế hành vi, phân tách trách nhiệm qua 4 rãnh bơi: `Sự kiện Chi tiêu (Event Source)`, `Động cơ DSA Engine`, `Động cơ Nudge Thaler` và `Người dùng (Mobile UI)`.

```text
[Sự kiện Giao dịch] ──> [DSA Engine] ──> [Nudge Engine] ──> [Người dùng Mobile]
```

* **Phân tích chi tiết các bước xử lý:**
  1. *Kích hoạt sự kiện:* Quy trình được kích hoạt tự động mỗi khi có một giao dịch phát sinh thành công từ bất kỳ nguồn nào (ghi thủ công, quét hóa đơn AI, quét mã VietQR).
  2. *Tính toán hạn mức thích ứng:* `DSA Engine` lấy dữ liệu ngân sách khả dụng còn lại của tháng và số ngày còn lại để tính toán lại giá trị $\text{DSA}_{\text{today}}$.
  3. *Xác định Vận tốc tiêu tiền:* Động cơ tính toán tỷ số vận tốc $V = \frac{\text{Chi tiêu thực tế tích lũy}}{\text{DSA}_{\text{today}}}$.
  4. *Thẩm định ngưỡng an toàn (Decision Node):*
     - **Nếu $V \le 1.2$ (Vận tốc an toàn):** Hệ thống chỉ cập nhật chỉ số lên giao diện Dashboard ở chế độ dải màu xanh/vàng bình thường, không làm phiền người dùng và kết thúc luồng xử lý.
     - **Nếu $V > 1.2$ (Vận tốc tiêu tiền nguy hiểm):** Tín hiệu bất thường được chuyển giao sang rãnh bơi `Động cơ Nudge Thaler`.
  5. *Đóng gói Cú hích & Giải trình XAI:* `Động cơ Nudge Thaler` soạn thảo thông điệp can thiệp hành vi vi mô phù hợp với ngữ cảnh, đồng thời liên kết với dữ liệu giải trình thuật toán (Explainable AI - công thức tính và ngày cạn ví dự kiến).
  6. *Tương tác phản hồi tại Client:* Rãnh bơi `Người dùng (Mobile UI)` tiếp nhận thông báo đẩy. Người dùng có hai hướng tương tác:
     - Bấm xem chi tiết giải trình XAI để hiểu rõ nguyên nhân cảnh báo $\rightarrow$ Thực hiện hành động tự điều chỉnh (hạ hạn mức các ngày tiếp theo hoặc hoãn các khoản chi tiêu giải trí cuối tuần).
     - Gạt bỏ thông báo nếu cảm thấy không cần thiết (bảo đảm quyền tự do lựa chọn định hướng - Libertarian Paternalism).
  7. *Kết thúc luồng:* Hệ thống lưu vết tương tác để phục vụ việc tự học và tinh chỉnh tần suất bắn cú hích cho các chu kỳ tiếp theo.

***Hình 3.25:*** *Sơ đồ hoạt động quy trình tính toán hạn mức DSA và phát tín hiệu cú hích Nudge*

---

## 3.4.3 Mô hình hóa vòng đời thực thể miền với Sơ đồ trạng thái (State Machine Diagrams)

Trong kiến trúc hướng miền (Domain-Driven Design), các thực thể cốt lõi (Core Entities) sở hữu danh tính duy nhất (Identity) và trải qua nhiều trạng thái biến đổi nghiệp vụ liên tục trong suốt vòng đời. Việc quản lý lỏng lẻo các trạng thái này bằng các cờ nhị phân phân tán (boolean flags) sẽ dẫn đến nguy cơ dữ liệu rơi vào trạng thái bất hợp lệ (Invalid State), ví dụ: một khoản chi chưa được xác minh số học nhưng đã tự ý trừ tiền ví, hoặc một khoản nợ đã thanh toán xong nhưng vẫn bị tính lãi phạt.

Sơ đồ Trạng thái (State Machine Diagram) chuẩn UML 2.5 giải quyết triệt để bài toán trên bằng cách đặc tả máy trạng thái hữu hạn (Finite State Machine - FSM) khép kín. Cấu trúc chuẩn hóa của một sơ đồ trạng thái bao gồm:
- **Trạng thái ban đầu (Initial State):** Ký hiệu bằng chấm tròn đen đặc, là điểm khởi thủy khi thực thể vừa được khởi tạo trong bộ nhớ.
- **Các trạng thái nghiệp vụ (States):** Ký hiệu bằng hình chữ nhật bo tròn góc, thể hiện tình trạng lưu trữ hiện thời của đối tượng trong CSDL. Bên trong mỗi trạng thái có thể khai báo các hành vi nội tại:
  - `entry /`: Hành động tự động thực thi ngay khi đối tượng vừa bước vào trạng thái.
  - `do /`: Hành động kéo dài liên tục trong suốt thời gian đối tượng duy trì trạng thái.
  - `exit /`: Hành động thực thi ngay trước khi đối tượng rời khỏi trạng thái.
  - `alert /`: Tín hiệu phát thông báo đến hệ thống bên ngoài.
- **Sự chuyển trạng thái (Transitions):** Biểu diễn bằng mũi tên có hướng từ trạng thái nguồn sang trạng thái đích, tuân thủ nghiêm ngặt cú pháp chuẩn mực quốc tế:
  $$\text{Tên\_Sự\_Kiện} \; [\text{Điều\_Kiện\_Bảo\_Vệ}] \; / \; \text{Hành\_Động\_Thực\_Thi} \quad (\text{Event } [\text{Guard}] \text{ / Action})$$
- **Nút rẽ nhánh điều kiện (Choice Pseudo-State):** Ký hiệu hình thoi, cho phép luồng chuyển trạng thái phân tách thành nhiều nhánh dựa trên giá trị động của điều kiện bảo vệ.
- **Trạng thái kết thúc (Final State):** Ký hiệu vòng tròn có tâm đen đặc (Bullseye), thể hiện vòng đời của thực thể đã kết thúc trọn vẹn và chuyển vào kho lưu trữ lịch sử.

Đặc biệt, để khắc phục triệt để tình trạng các mũi tên giao cắt chéo nhau gây khó đọc, toàn bộ 3 sơ đồ trạng thái trong hệ thống được tái cấu trúc theo **Kiến trúc Không Cắt Chéo (Zero-Crossing Spatial Architecture)**, các nhãn điều kiện được đặt trên nền trắng ngắt quãng đường kẻ (`labelBackgroundColor=#ffffff`) bảo đảm tính thẩm mỹ và độ rõ nét tối đa.

---

### Thực thể 1: Sơ đồ trạng thái Vòng đời Giao dịch Chi tiêu (Expense Entity Lifecycle)

Thực thể `Expense` là trung tâm dữ liệu của hệ thống, phản ánh toàn bộ các biến đổi của một khoản chi tiêu từ khi mới nạp ảnh hóa đơn đến khi được trừ ví thành công hoặc bị hủy bỏ.

```text
[*] ──> DRAFT ──> VERIFIED ──> [Choice 1] ──> COMPLETED ──> [*]
                                   │
                                   └──> PENDING_COOLING_OFF ──> [Choice 2] ──> CANCELLED ──> [*]
                                                                     │
                                                                     └──> COMPLETED
```

* **Ý nghĩa các trạng thái nghiệp vụ:**
  - `DRAFT (Nháp)`: Trạng thái sơ khởi khi người dùng vừa chụp ảnh hóa đơn hoặc mở form ghi chép. Tại đây, dữ liệu số học chưa được kiểm chứng.
  - `VERIFIED (Đã Xác Minh)`: Trạng thái đạt được khi hệ thống đã hoàn tất đối soát số học với độ lệch $\Delta = 0$, dữ liệu danh mục và số tiền đã được chuẩn hóa hợp lệ.
  - `PENDING_COOLING_OFF (Chờ Khoảng Trễ)`: Trạng thái đóng băng tạm thời đối với khoản chi tiêu lớn (> 3 triệu VNĐ). Tiền trong ví chưa bị trích trừ, đồng hồ đếm ngược 24 giờ được kích hoạt.
  - `COMPLETED (Hoàn Tất)`: Trạng thái giao dịch tài chính chính thức thành công, hệ thống thực thi trừ tiền ví và ghi sổ cái vĩnh viễn.
  - `CANCELLED (Đã Hủy)`: Trạng thái khoản chi bị người dùng chủ động hủy bỏ sau khoảng thời gian suy ngẫm lý trí.

* **Bảng ma trận chuyển dịch trạng thái thực thể Giao dịch Chi tiêu:**

| Trạng thái nguồn | Sự kiện kích hoạt (Event) | Điều kiện bảo vệ [Guard] | Hành động thực thi (/ Action) | Trạng thái đích |
| :--- | :--- | :--- | :--- | :--- |
| **`[*]` (Initial)** | Chụp ảnh hóa đơn / Mở form | Không có | Khởi tạo Form & nạp OCR thô | `DRAFT` |
| **`DRAFT`** | Kiểm toán đối soát hoàn tất | `[Delta == 0]` | Gắn cờ hợp lệ dữ liệu | `VERIFIED` |
| **`VERIFIED`** | Người dùng bấm xác nhận form | `[Số tiền <= 3 triệu]` | Khấu trừ số dư ví & ghi sổ cái | `COMPLETED` |
| **`VERIFIED`** | Người dùng bấm xác nhận form | `[Số tiền > 3 triệu]` | Kích hoạt khóa tạm & hẹn giờ 24h | `PENDING_COOLING_OFF` |
| **`PENDING_COOLING_OFF`**| Hết hạn 24 giờ (Cron kích hoạt)| Không có | Mở hộp thoại hỏi quyết định cuối | `[Choice 2]` |
| **`[Choice 2]`** | Người dùng chọn "Hủy mua" | `[Không cần thiết]` | Hủy giao dịch & mừng tiết kiệm | `CANCELLED` |
| **`[Choice 2]`** | Người dùng chọn "Xác nhận" | `[Nhu cầu thực sự]` | Khấu trừ số dư ví & ghi sổ cái | `COMPLETED` |
| **`COMPLETED`** | Ghi sổ thành công | Không có | Cập nhật số dư & đóng tiến trình | `[*]` (Final) |
| **`CANCELLED`** | Hủy giao dịch thành công | Không có | Lưu lịch sử & đóng tiến trình | `[*]` (Final) |

***Hình 3.26:*** *Sơ đồ trạng thái vòng đời thực thể giao dịch chi tiêu cá nhân*

---

### Thực thể 2: Sơ đồ trạng thái Vòng đời Kế hoạch Trả nợ Cá nhân (Debt Plan Lifecycle)

Thực thể `DebtPaymentPlan` quản lý chiến lược thanh toán nợ cá nhân, mô hình hóa sự chuyển đổi giữa các giai đoạn lập kế hoạch, thực thi định kỳ, tăng tốc tài chính và kiểm soát rủi ro quá hạn.

```text
               ┌─────────────> ACCELERATED ─────────────┐
               │                      │                 │
[*] ──> PLANNING ──> ACTIVE ──────────┼──────────────> SETTLED ──> [*]
               │                      │
               └─────────────> OVERDUE_RISK
```

* **Ý nghĩa các trạng thái nghiệp vụ:**
  - `PLANNING (Khởi Tạo)`: Người dùng kê khai danh mục các khoản nợ (gốc, lãi suất, hạn chót) và lựa chọn chiến lược tối ưu hóa (Debt Snowball - Quả cầu tuyết hoặc Debt Avalanche - Tuyết lở).
  - `ACTIVE (Đang Thực Thi)`: Kế hoạch bước vào chu kỳ thanh toán định kỳ hàng tháng. Hệ thống tự động phân bổ tiền trả nợ tối thiểu cho mọi khoản nợ và dồn phần tiền dôi dư cho khoản nợ ưu tiên hàng đầu.
  - `ACCELERATED (Tăng Tốc)`: Trạng thái đạt được khi người dùng nhận được nguồn tiền bất thường (thưởng, hoàn thuế, thặng dư ngân sách) và quyết định dồn thêm tiền trả sớm nhằm rút ngắn thời gian chịu lãi.
  - `OVERDUE_RISK (Báo Động Rủi Ro)`: Trạng thái kích hoạt khi có khoản nợ bị trễ hạn thanh toán quá 15 ngày, hệ thống phát tín hiệu cảnh báo thắt lưng buộc bụng và tái cấu trúc dòng tiền khẩn cấp.
  - `SETTLED / PAID_OFF (Xóa Nợ Thành Công)`: Toàn bộ các khoản nợ trong danh mục đã được thanh toán sạch sẽ, tổng dư nợ trở về đúng 0 VNĐ.

* **Bảng ma trận chuyển dịch trạng thái Kế hoạch Trả nợ:**

| Trạng thái nguồn | Sự kiện kích hoạt (Event) | Điều kiện bảo vệ [Guard] | Hành động thực thi (/ Action) | Trạng thái đích |
| :--- | :--- | :--- | :--- | :--- |
| **`[*]` (Initial)** | Thiết lập mục tiêu xóa nợ | Không có | Khởi tạo danh mục nợ & chọn chiến lược | `PLANNING` |
| **`PLANNING`** | Người dùng phê duyệt kế hoạch | `[Bắt đầu chu kỳ]` | Kích hoạt lịch trả nợ hàng tháng | `ACTIVE` |
| **`ACTIVE`** | Có thặng dư ngân sách / thưởng | `[Dồn tiền thêm]` | Tăng số tiền thanh toán khoản nợ số 1 | `ACCELERATED` |
| **`ACCELERATED`** | Hết nguồn tiền thặng dư | Không có | Trở về hạn mức trả nợ định kỳ | `ACTIVE` |
| **`ACTIVE`** | Không trả tiền đúng hạn | `[Trễ hạn > 15 ngày]`| Bật cảnh báo & rà soát ngân sách | `OVERDUE_RISK` |
| **`OVERDUE_RISK`** | Đã thanh toán bổ sung | `[Bình thường hóa]` | Xóa cảnh báo & tiếp tục chu kỳ | `ACTIVE` |
| **`ACTIVE`** | Thanh toán khoản nợ cuối cùng | `[Dư nợ = 0 VNĐ]` | Hạch toán hoàn tất & chúc mừng tự do | `SETTLED` |
| **`ACCELERATED`** | Thanh toán dứt điểm nợ sớm | `[Dư nợ = 0 VNĐ]` | Hạch toán hoàn tất & chúc mừng tự do | `SETTLED` |
| **`SETTLED`** | Đóng hồ sơ kế hoạch | Không có | Chuyển dữ liệu vào kho lưu trữ | `[*]` (Final) |

***Hình 3.27:*** *Sơ đồ trạng thái vòng đời kế hoạch trả nợ cá nhân và quản trị rủi ro nợ*

---

### Thực thể 3: Sơ đồ trạng thái Vòng đời Cú hích Hành vi và Giải trình Minh bạch (Behavioral Nudge & XAI Lifecycle)

Thực thể `FinancialNudge` thể hiện vòng đời của một can thiệp kiến trúc lựa chọn tâm lý học, phản ánh toàn bộ tiến trình từ lúc hệ thống phát hiện hành vi chi tiêu bốc đồng đến khi người dùng tương tác, tiếp nhận giải trình và chuyển đổi hành vi tài chính tích cực.

```text
[*] ──> TRIGGERED ──> DISPATCHED ──> DISPLAYED ──> DISMISSED ──> [*]
                                        │             ▲
                                        ▼             │
                                    EXPLAINED ────────┘
                                        │
                                        ▼
                                    CONVERTED ──> [*]
```

* **Ý nghĩa các trạng thái nghiệp vụ:**
  - `TRIGGERED (Khởi Tạo)`: Trạng thái sinh ra khi hệ thống phát hiện vi phạm quy tắc chi tiêu (Tốc độ tiêu tiền $V > 1.2$ hoặc chi tiêu vượt quá 85% định mức ngày DSA).
  - `DISPATCHED (Đã Gửi)`: Cú hích được đóng gói payload vi mô và chuyển tiếp thành công qua cổng thông báo đẩy FCM Gateway.
  - `DISPLAYED (Hiển Thị)`: Thông điệp biểu ngữ (Banner Toast) hiển thị trực tiếp trên màn hình thiết bị di động của người dùng, sẵn sàng tiếp nhận tương tác.
  - `EXPLAINED (Đã Giải Trình)`: Người dùng chạm vào thông báo để mở Modal Explainable AI. Tại đây, hệ thống minh bạch hóa toàn bộ công thức tính toán và số liệu dự phóng ngày cạn ví.
  - `CONVERTED (Chuyển Đổi Thành Công)`: Người dùng tiếp thu lời khuyên và có hành động tài chính tích cực (hủy khoản mua bốc đồng, hạ hạn mức ăn uống ngoài, hoặc phân bổ tiền vào quỹ tiết kiệm).
  - `DISMISSED (Gạt Bỏ / Bỏ Qua)`: Người dùng quyết định gạt bỏ thông báo hoặc đọc giải trình nhưng vẫn tiếp tục chi tiêu theo ý muốn chủ quan (bảo đảm tính phi cưỡng chế của thuyết Cú hích).

* **Bảng ma trận chuyển dịch trạng thái Cú hích Hành vi & Giải trình:**

| Trạng thái nguồn | Sự kiện kích hoạt (Event) | Điều kiện bảo vệ [Guard] | Hành động thực thi (/ Action) | Trạng thái đích |
| :--- | :--- | :--- | :--- | :--- |
| **`[*]` (Initial)** | Vi phạm quy tắc chi tiêu | `[V > 1.2 hoặc > 85% DSA]` | Khởi tạo bản ghi Cú hích ACTIVE | `TRIGGERED` |
| **`TRIGGERED`** | Gửi gói tin qua FCM | `[Gateway Available]` | Đóng gói payload & phát tín hiệu đẩy | `DISPATCHED` |
| **`DISPATCHED`** | Thiết bị di động nhận gói tin | Không có | Hiển thị biểu ngữ trên màn hình điện thoại| `DISPLAYED` |
| **`DISPLAYED`** | Người dùng bấm "Tại sao?" | Không có | Mở hộp thoại Explainable AI minh bạch | `EXPLAINED` |
| **`DISPLAYED`** | Người dùng gạt bỏ biểu ngữ | `[Không xem]` | Ghi nhận phản hồi không tiếp nhận | `DISMISSED` |
| **`EXPLAINED`** | Người dùng đồng ý điều chỉnh | `[Dừng mua / Hạ hạn mức]` | Ghi nhận chuyển đổi hành vi thành công | `CONVERTED` |
| **`EXPLAINED`** | Người dùng đọc xong bỏ qua | `[Giữ nguyên chi tiêu]` | Ghi nhận phản hồi không hành động | `DISMISSED` |
| **`DISMISSED`** | Đóng tiến trình can thiệp | Không có | Lưu lịch sử để học máy tần suất | `[*]` (Final) |
| **`CONVERTED`** | Lưu kết quả tích cực | Không có | Cập nhật điểm kỷ luật tài chính | `[*]` (Final) |

***Hình 3.28:*** *Sơ đồ trạng thái vòng đời cú hích hành vi và giải trình thuật toán minh bạch*

---

## 3.4.4 Tổng kết Mô hình Động và Mối liên kết với Kiến trúc Hệ thống

Việc hoàn thiện toàn diện bộ 11 sơ đồ thuộc Mô hình Động (gồm 5 Sơ đồ Tuần tự, 3 Sơ đồ Hoạt động và 3 Sơ đồ Trạng thái) đã khép lại bức tranh thiết kế hướng đối tượng (OOAD) của hệ thống:
1. **Khớp nối hoàn hảo với Ca sử dụng (Use Cases):** Mỗi kịch bản tuần tự và hoạt động đều phản ánh trung thực các bước xử lý trong luồng chính và luồng ngoại lệ của 60 ca sử dụng đã đặc tả tại Mục 3.2.
2. **Khớp nối hoàn hảo với Sơ đồ Lớp (Class Diagram):** Các đối tượng tham gia đường đời (Lifelines), các phương thức được kích hoạt và các thuộc tính trạng thái đều tồn tại 100% trong 18 Lớp Domain Entities và 6 Bounded Contexts đã thiết kế tại Mục 3.3.
3. **Tiền đề vững chắc cho Thiết kế Cơ sở Dữ liệu (Mục 3.5):** Các trạng thái nghiệp vụ và các thuộc tính kiểm toán trong máy trạng thái cung cấp đầy đủ thông số cho việc thiết kế các trường trạng thái (`status`), ràng buộc kiểm tra (`CHECK Constraint`), khóa ngoại (`Foreign Key`) và các bảng nhật ký kiểm toán (Audit Trail) trong hệ quản trị CSDL quan hệ.
