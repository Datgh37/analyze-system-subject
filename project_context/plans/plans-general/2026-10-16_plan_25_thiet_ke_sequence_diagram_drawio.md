# KẾ HOẠCH THIẾT KẾ & TRIỂN KHAI SƠ ĐỒ TUẦN TỰ (SEQUENCE DIAGRAMS - DRAW.IO)

> **Mã kế hoạch:** `PLAN-25`  
> **Ngày khởi tạo:** 2026-10-16  
> **Trạng thái:** IN-PROGRESS  
> **Căn cứ bối cảnh:** Tiếp thu định hướng từ Giảng viên hướng dẫn môn Phân tích Thiết kế Hệ thống Thông tin (PTTKHTTT) về việc lựa chọn 4 - 5 luồng hoạt động tiêu biểu nhất để mô hình hóa Sơ đồ Tuần tự (Sequence Diagram - SD); áp dụng cấp độ Thiết kế (Design Level) theo kiến trúc phân tầng Clean Architecture và Domain-Driven Design (DDD). Toàn bộ sơ đồ được tập trung vào một tệp duy nhất [`files/SequenceDiagram.drawio`](../../files/SequenceDiagram.drawio) nhằm bảo đảm tính đồng bộ công cụ và phong cách thẩm mỹ với sơ đồ ca sử dụng và sơ đồ lớp.

---

## 1. MỤC TIÊU VÀ NGUYÊN TẮC THIẾT KẾ CỐT LÕI (DESIGN PRINCIPLES)

### 1.1. Mục tiêu
1. Hiện thực hóa các kịch bản tương tác động của 5 luồng nghiệp vụ cốt lõi, minh họa luồng đi của dữ liệu từ giao diện người dùng, tầng điều khiển, tầng nghiệp vụ miền, các giải thuật toán học, cơ sở dữ liệu quan hệ ACID đến các dịch vụ ngoại vi (AI Multimodal, VietQR, Notification Service).
2. Xây dựng tệp mã nguồn Draw.io đa trang (Multi-diagram XML) chuẩn mực, dễ bảo trì, xuất ảnh độ phân giải cao phục vụ chèn trực tiếp vào **Mục 3.4 (Thiết kế tương tác và sơ đồ tuần tự hệ thống)** trong Chương 3 của Khóa luận Tốt nghiệp ([`KhoaLuanTotNghiep.docx`](../../files/KhoaLuanTotNghiep.docx)).

### 1.2. Nguyên tắc thiết kế Sequence Diagram cấp độ Thiết kế (Design Level)
1. **Phân tầng kiến trúc rành mạch (Clean Architecture Lifelines):**
   - **Tác nhân (Actor):** `Người dùng cá nhân (Personal User)` và các External Services.
   - **Boundary / Presentation Layer:** Giao diện Mobile App Client (Màn hình chức năng, Form nhập liệu).
   - **Interface Adapters Layer:** API Controllers tiếp nhận Request và trả về DTO Response.
   - **Application Core Layer:** Application Services điều phối luồng nghiệp vụ.
   - **Domain Logic / Engine Layer:** Các Entity cốt lõi và các động cơ tính toán độc lập (`MathematicalAuditEngine`, `DSAEngine`, `BehavioralNudgeEngine`, `CoolingOffManager`).
   - **Infrastructure / Data Access Layer:** Repositories thao tác CSDL quan hệ với giao dịch nguyên tử ACID.
   - **External Supporting Services:** Dịch vụ AI Đa phương thức đám mây, Cổng thanh toán VietQR, Cổng thông báo đẩy FCM.
2. **Tuân thủ chuẩn UML 2.5:**
   - Thông điệp là động từ gọi hàm rõ ràng kèm tham số kiểu dữ liệu: `scanReceipt(imageBytes)`, `verifyArithmetic(items, tax, total)`.
   - Phân biệt chính xác giữa lời gọi đồng bộ (Synchronous Call - mũi tên nét liền kín đầu), lời gọi bất đồng bộ (Asynchronous Call - mũi tên nét liền hở đầu), và thông điệp trả về (Return Message - mũi tên nét đứt).
   - Áp dụng các khung phân đoạn tương tác chuẩn: `alt` (rẽ nhánh điều kiện thành công vs ngoại lệ), `opt` (tùy chọn), `loop` (vòng lặp duyệt danh sách).
3. **Thống nhất thẩm mỹ đồ án (Design Aesthetic Consistency):**
   - Font chữ: `Segoe UI, Arial, sans-serif` hoặc `Times New Roman` chuẩn mực.
   - Kích thước: Rõ ràng, đường nét trực giao, màu sắc thanh kích hoạt và đường sinh mệnh hài hòa với bộ nhận diện đồ án.

---

## 2. DANH MỤC 5 TAB SƠ ĐỒ TUẦN TỰ TRONG `SequenceDiagram.drawio`

| Tab | Tên Tab / Mã Sơ Đồ | Tên Luồng Hoạt Động Tiêu Biểu | Use Cases Liên Quan | Các Điểm Nhấn Kiến Trúc & Giá Trị Đồ Án |
| :---: | :--- | :--- | :---: | :--- |
| **Tab 1** | **`SD-01: Bóc Tách Hóa Đơn AI OCR`** | Quét & Bóc tách Hóa đơn bằng AI OCR kết hợp Hậu kiểm toán số học | `UC-03.07`<br>`UC-03.08` | • Xử lý ảnh đa món hàng hóa đơn siêu thị.<br>• Gọi API AI Vision OCR trích xuất JSON Schema.<br>• Thuật toán Hậu kiểm toán số học $\Delta = 0$ (Khớp vs Lệch số học).<br>• Phê duyệt người dùng 1 chạm (Human-in-the-loop) & trừ tiền ví ACID. |
| **Tab 2** | **`SD-02: Ghi Nhận Giọng Nói Voice NLU`** | Ghi nhận Chi tiêu nhanh bằng Khẩu lệnh Giọng nói | `UC-03.11` | • Thu âm giọng nói hàng quán vỉa hè không có hóa đơn.<br>• Phân tích ngữ nghĩa Speech-to-Text & NLU.<br>• Bóc tách thực thể số tiền, danh mục, ví nguồn.<br>• Điền biểu mẫu xem trước và kích hoạt tính lại hạn mức ngày DSA. |
| **Tab 3** | **`SD-03: Thanh Toán VietQR NAPAS`** | Khởi tạo Giao dịch Thanh toán VietQR & Cập nhật Sổ cái Nội bộ | `UC-03.10` | • Chuẩn hóa dữ liệu EMVCo Merchant-Presented Mode.<br>• Sinh mã VietQR động kẹp số tiền.<br>• Sinh Deeplink chuyển tiếp người dùng sang Mobile Banking (không giữ tiền).<br>• Cấn trừ số dư ví ngân hàng trên Sổ cái kế hoạch (Planning Ledger). |
| **Tab 4** | **`SD-04: Động Cơ DSA & Cú Hích Thaler`** | Tính toán Hạn mức Ngày DSA Thích ứng & Bắn Cú hích Hành vi Thaler | `UC-05.05`<br>`UC-05.06`<br>`UC-09.01`<br>`UC-09.02` | • Kích hoạt tính lại hạn mức ngày DSA thích ứng sau chi tiêu.<br>• Phát hiện vi mô tốc độ tiêu tiền $V > 1.2$ hoặc chạm trần 85%.<br>• Đẩy thông báo tức thời Just-in-Time qua Notification Service.<br>• Trực quan hóa bản giải trình minh bạch Explainable AI (XAI). |
| **Tab 5** | **`SD-05: Khoảng Trễ Tâm Lý Cooling-off`** | Đánh giá Mua sắm Lớn & Kích hoạt Khoảng trễ Tâm lý 24h - 48h | `UC-09.05` | • Cơ chế kiến tạo ma sát có lợi (Friction by Design).<br>• Chặn chi tiêu bốc đồng $> 3$ triệu hoặc $> 20\%$ lương.<br>• Lưu giao dịch trạng thái `PENDING_COOLING_OFF` và hẹn giờ đếm ngược 24h.<br>• Background Worker nhắc nhở hết hạn trễ $\to$ Nhánh `alt`: Hủy bỏ tiết kiệm thành công vs Xác nhận mua. |

---

## 3. LỘ TRÌNH TRIỂN KHAI CHI TIẾT (IMPLEMENTATION PHASES)

### Pha 1: Chuẩn hóa Kịch bản chi tiết và Bảng ánh xạ đối tượng (Object Mapping)
- Xác định đầy đủ danh sách Lifelines (Class/Service) và các hàm tương ứng của từng sơ đồ khớp 100% với Sơ đồ lớp [`files/ClassDiagram.drawio`](../../files/ClassDiagram.drawio).
- Soạn thảo kịch bản tuần tự từng bước kèm mã hiệu thông điệp cho cả 5 sơ đồ.

### Pha 2: Khởi tạo và Xây dựng cấu trúc XML đa trang trong `SequenceDiagram.drawio`
- Khởi tạo tệp [`files/SequenceDiagram.drawio`](../../files/SequenceDiagram.drawio) gồm 5 thẻ `<diagram>` độc lập.
- Thiết lập tọa độ X cho các Lifeline bảo đảm khoảng cách thông thoáng (tối thiểu 160px - 220px giữa các Lifeline).
- Thiết lập các thanh kích hoạt (Activation Bars) liên tục, đúng thời gian sống.
- Vẽ các đường thông điệp kèm nhãn chú thích rõ ràng; đóng khung các phân đoạn `alt`, `opt`, `loop`.

### Pha 3: Thẩm định Kỹ thuật & Kiểm tra Toàn vẹn (Quality Gate)
- Kiểm tra cú pháp XML của tệp Draw.io bằng script Python bảo đảm không có lỗi parsing.
- Mở xem trước trực quan trên Draw.io để kiểm tra căn lề, không để chữ bị tràn khung.
- Đối chiếu tính nhất quán giữa Sequence Diagram, Class Diagram và Use Case Specification.

### Pha 4: Lập Tài liệu Hướng dẫn Chèn vào Khóa Luận Tốt Nghiệp
- Soạn thảo văn bản thuyết minh chi tiết cho từng sơ đồ để chèn vào **Mục 3.4 trong Chương 3 Khóa luận tốt nghiệp**.
- Hướng dẫn vị trí chèn ảnh `Hình 3.18` đến `Hình 3.22` trong file Word.
