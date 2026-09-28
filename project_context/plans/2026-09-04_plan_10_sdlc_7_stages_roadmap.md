# Kế Hoạch Triển Khai: PLAN-10 — Khung Vòng Đời SDLC 4 Giai Đoạn Trọng Tâm & Đặc Tả Mô Hình Use Case PFM

> **Mã kế hoạch:** `PLAN-10`  
> **Thời điểm lập:** `2026-09-04`  
> **Phạm vi trọng tâm:** Giai đoạn 1 đến 4 của chu trình SDLC (Học phần PTTKHTTT)  
> **Trạng thái:** `COMPLETED (100% OK)`

---

## 1. Mục Tiêu Kế Hoạch
1. **Thiết lập Khung Vòng đời SDLC 4 Giai đoạn trọng tâm:** Định hình rõ ràng lộ trình thực hiện cho học phần Phân tích Thiết kế Hệ thống Thông tin (PTTKHTTT), tập trung giải quyết triệt để từ Khảo sát hiện trạng đến Hiện thực nguyên mẫu phần mềm (Giai đoạn 1 đến 4). Các giai đoạn Thử nghiệm quy mô lớn, Triển khai hạ tầng và Bảo trì (Giai đoạn 5 đến 7) sẽ được kích hoạt ở giai đoạn làm Khóa luận tốt nghiệp chính thức.
2. **Linh hoạt hóa cấu trúc báo cáo:** Cho phép phân tách và mở rộng cấu trúc các chương linh hoạt theo chiều sâu nghiên cứu và yêu cầu thực tế của đề tài, không bị gò bó cứng nhắc vào khuôn mẫu 5 chương sơ lược.
3. **Chuyển hóa khoảng trống nghiên cứu Chương 1 thành Mô hình kỹ thuật:** Kế thừa trực tiếp các phát hiện về hạn chế của thị trường hiện hành (YNAB, Money Manager, BudgetBakers...) trong Chương 1 để thiết kế hệ thống Tác nhân (Actors) và 5 Gói Use Case (17 Use Cases) mang tính đột phá và cải tiến rõ rệt.
4. **Bàn giao tạo tác Draw.io chuẩn trực quan:** Cung cấp tệp thiết kế `files/use_case_diagram_pfm.drawio` tương thích hoàn toàn với Diagrams.net / Draw.io, sẵn sàng xuất ảnh chuẩn DPI để chèn vào báo cáo Word.

---

## 2. Ma Trận Ánh Xạ Chu Trình 4 Giai Đoạn & Hội Đồng 13 Tác Tử

| Giai đoạn SDLC | Trọng tâm công việc | Thành phần báo cáo tương ứng | Tác tử Hội đồng chủ trì | Sản phẩm bàn giao chính |
| :--- | :--- | :--- | :--- | :--- |
| **Giai đoạn 1: Nghiên cứu sơ bộ & Khảo sát hiện trạng** | Khảo sát các ứng dụng thực tế trên thị trường (YNAB, Money Manager...), phân tích 3 Persona và chỉ ra 4 hạn chế lớn cùng khoảng trống Adaptive Budgeting. | Chương 1: Khảo sát hiện trạng hệ thống | **Agent 01 (Thaler)**<br>**Agent 02 (Budget Coach)** | Báo cáo khảo sát, Bảng đối chiếu thị trường, Bảng tổng hợp Pain Points. |
| **Giai đoạn 2: Phân tích yêu cầu hệ thống** | Định danh 4 Actor (1 Primary, 3 Secondary); phân rã 5 Use Case Packages (17 Use Cases); xây dựng đặc tả Use Case chuẩn IEEE 29148. | Phần mở đầu Chương Phân tích & Thiết kế | **Agent 03 (Cockburn BA)** | Sơ đồ `use_case_diagram_pfm.drawio`, Bảng đặc tả kịch bản Use Case, Luồng sự kiện & Gherkin BDD. |
| **Giai đoạn 3: Thiết kế hệ thống** | Thiết kế Clean Architecture, Bounded Contexts, Class Diagram, CSDL quan hệ 3NF chuẩn ACID, và Atomic UI Design System. | Phần trọng tâm Chương Phân tích & Thiết kế | **Agent 04 (Evans)**<br>**Agent 05 (Kleppmann)**<br>**Agent 09 (Frost)**<br>**Agent 10 (Abramov)**<br>**Agent 11 (Schneier)** | Class Diagram, Sơ đồ ERD 3NF CSDL, UI Wireframes, Tokens Design System WCAG 2.1 AA. |
| **Giai đoạn 4: Xây dựng & Hiện thực phần mềm** | Hiện thực lõi thuật toán DSA, bộ giải nợ Snowball vs Avalanche, pipeline bóc tách hóa đơn AI Multimodal OCR + Math Audit. | Chương Hiện thực & Đánh giá kết quả | **Agent 06 (Debt Optimizer)**<br>**Agent 07 (Vogels)**<br>**Agent 08 (Andrew Ng)** | Mã nguồn nguyên mẫu (Python/React/FastAPI), API endpoints, Demo chức năng hoàn chỉnh. |

*(Giai đoạn 5: Thử nghiệm tải & QA toàn diện, Giai đoạn 6: Đóng gói Docker & Triển khai, Giai đoạn 7: Bảo trì & Nâng cấp sẽ được mở rộng ở giai đoạn Khóa luận tốt nghiệp chính thức dưới sự bảo trợ của Agent 07 và Agent 12).*

---

## 3. Bản Thiết Kế Hệ Thống Tác Nhân (Actors)
* **Primary Human Actor:**
  * `Người dùng cá nhân (Personal User / Individual Saver)`: Tác nhân duy nhất tương tác trực tiếp với giao diện để quản lý tài chính cá nhân độc lập (100% PFM). Hệ thống không có quyền Admin can thiệp vào số dư ví để bảo vệ tính riêng tư tuyệt đối của dữ liệu.
* **Secondary System Actors:**
  * `AI Vision OCR Engine`: Dịch vụ bóc tách đa món từ ảnh hóa đơn mua sắm.
  * `VietQR Gateway`: Cổng tạo mã QR động Napas 247 hỗ trợ chuyển khoản nhanh.
  * `Notification Service`: Dịch vụ đẩy cảnh báo vi mô (Micro-Alert) khi tốc độ chi tiêu vượt ngưỡng hoặc đến hạn thanh toán định kỳ.

---

## 4. Danh Mục 5 Gói Phân Hệ Use Case (17 Use Cases)

1. **Package 1: Quản lý Hồ sơ & Đa Ví (Core Wallet)**
   - `UC01`: Khởi tạo & Cấu hình Đa Ví (Tiền mặt, Ngân hàng, Thẻ tín dụng, MoMo).
   - `UC02`: Chuyển tiền nội bộ giữa các ví (Internal Transfer with ACID guarantee).
   - `UC03`: Đối soát & Cân chỉnh số dư thực tế (Reconcile Balance).
2. **Package 2: Ghi Nhận Thu Chi & Bóc Tách Hóa Đơn AI (Multimodal Pipeline)**
   - `UC04`: Ghi nhận thu/chi thủ công.
   - `UC05`: Quét & Bóc tách hóa đơn bằng AI Vision OCR.
   - `UC06`: Hậu kiểm toán số học hóa đơn (Mathematical Audit) *(`<<include>>` từ UC05)*.
   - `UC07`: Tự động phân loại danh mục thông minh (Smart Categorization) *(`<<include>>` từ UC05)*.
   - `UC08`: Tạo giao dịch thanh toán chuyển khoản VietQR.
3. **Package 3: Động Cơ Ngân Sách Thích Ứng (Adaptive Budgeting Engine - Điểm Sáng Đổi Mới)**
   - `UC09`: Thiết lập ngân sách đa phương pháp (50/30/20, Zero-Based Budgeting, Kakeibo).
   - `UC10`: Tính toán Hạn mức chi tiêu mỗi ngày (Daily Spending Allowance - DSA) *(`<<extend>>` từ UC04 & UC05)*.
   - `UC11`: Dự báo điểm cạn tiền theo Tốc độ tiêu tiền (Spending Velocity Projection).
   - `UC12`: Khuyến nghị tái cân đối ngân sách động (Dynamic Rebalancing Nudge).
4. **Package 4: Tối Ưu Hóa Nợ Cá Nhân & Tích Lũy Mục Tiêu (Debt Solver & Savings)**
   - `UC13`: Quản lý danh mục nợ cá nhân (Thẻ tín dụng, Khoản vay tiêu dùng).
   - `UC14`: Mô phỏng chiến lược trả nợ tối ưu (Debt Snowball vs Avalanche).
   - `UC15`: Thiết lập mục tiêu tích lũy & Quỹ khẩn cấp (Emergency Fund).
5. **Package 5: Giám Sát Rò Rỉ Dòng Tiền & Cảnh Báo Thông Minh (Leakage & Micro-Alerts)**
   - `UC16`: Quét & phát hiện Bẫy đăng ký định kỳ (Subscription Traps Leakage).
   - `UC17`: Tiếp nhận cảnh báo tức thì khi vượt ngưỡng chi tiêu (Spending Micro-Alert) *(`<<extend>>` từ UC11)*.

---

## 5. Tạo Tác Bàn Giao
* **Tệp mô hình XML trực quan:** [`files/use_case_diagram_pfm.drawio`](../../files/use_case_diagram_pfm.drawio) (Đã kiểm tra cấu trúc XML đạt chuẩn 100%).
* **Danh sách vị trí cần đồng bộ trong file Word gốc:** Đã bàn giao trong nhật ký quyết định và implementation plan.
