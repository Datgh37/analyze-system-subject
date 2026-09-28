# Kế Hoạch Triển Khai: PLAN-15 — Báo Cáo Tiến Độ Giữa Kỳ Môn PTTKHTTT (.docx) & Bộ Câu Hỏi Vấn Đáp Q&A

> **Mã kế hoạch:** `PLAN-15 (Revision 4 - Final)`  
> **Thời điểm cập nhật:** `2026-09-17`  
> **Mục tiêu:** Soạn thảo tệp tài liệu Word (`files/BaoCao_TienDo_GiuaKy_PTTKHTTT.docx`) báo cáo tiến độ giữa kỳ môn Phân tích và Thiết kế Hệ thống Thông tin (Khoa CNTT - DNTU).  
> **Đặc điểm:** Bố cục súc tích, tiêu đề ngắn gọn tự nhiên, tập trung 100% vào các kết quả đã hoàn thành; bổ sung Khảo sát thị trường đối chiếu ứng dụng thực tế (Bảng 2); triệt tiêu hoàn toàn số liệu % giả định; tích hợp **Mục Q&A Vấn đáp kiến thức kinh tế, nghiệp vụ & giải thích toàn bộ thuật ngữ viết tắt (ACID, DDD, OOAD...)** ở cuối tài liệu.

---

## 1. Thông Tin Nhóm & Bảng Phân Công Đóng Góp

Bảng phân công nhiệm vụ đặt ngay sau tiêu đề báo cáo, áp dụng chuẩn **Academic Navy Theme (`#2B4C7E`)**:

| STT | Họ và tên sinh viên | Mã sinh viên | Vai trò | Đóng góp (%) | Tóm tắt công việc đã thực hiện |
| :---: | :--- | :---: | :---: | :---: | :--- |
| **01** | **Trịnh Thành Đạt** | **1923050149** | Trưởng nhóm | **70%** | - Định hướng kiến trúc hệ thống và phân tích bài toán PFM cá nhân 100%.<br>- Phân tích thị trường, đối chiếu ứng dụng hiện có và định vị khoảng trống nghiên cứu.<br>- Phân tích yêu cầu chức năng (9 phân hệ) và phi chức năng.<br>- Mô hình hóa Use Case, phân định quan hệ `<<include>>` / `<<extend>>`, thiết kế sơ đồ tổng quan trên Draw.io.<br>- Chủ trì quy trình 6 bước xây dựng Class Diagram: Sàng lọc lớp theo 6 tiêu chí, thiết kế thuộc tính, kiểu dữ liệu, phương thức Rich Domain và sơ đồ lớp trên Draw.io.<br>- Tổng hợp và biên tập nội dung báo cáo. |
| **02** | **Trần Trọng Đại** | **1923050745** | Thành viên | **30%** | - Hỗ trợ thu thập thông tin tính năng các ứng dụng thị trường (Money Manager, Spendee).<br>- Khảo sát đặc điểm hành vi của 3 nhóm chân dung người dùng (Sinh viên, Người mới đi làm).<br>- Hỗ trợ rà soát danh sách tên gọi các Use Case con và thuộc tính dữ liệu thô.<br>- Hỗ trợ kiểm tra định dạng bảng biểu và rà soát lỗi chính tả văn bản. |

---

## 2. Bố Cục Nội Dung Báo Cáo (Ngắn Gọn & Trọng Tâm)

1. **Trang tiêu đề:** Trường ĐH Công nghệ Đồng Nai - Khoa CNTT, Môn học, Tên bài báo cáo, Đề tài, GVHD, Thông tin 2 sinh viên.
2. **Bảng 1 — Phân công nhiệm vụ:** Đạt 70% - Đại 30%.
3. **Phần 1: Giới thiệu tổng quan đề tài**
   - Bối cảnh & Tính cấp thiết: Thanh toán số triệt tiêu ma sát thanh toán (Pain of Paying); người trẻ mất kiểm soát dòng tiền.
   - Mục tiêu hệ thống: Tự động hóa nhập liệu AI OCR, Ngân sách thích ứng đa phương pháp, Cú hích Nudge DSA, Tối ưu nợ Snowball/Avalanche.
   - Phạm vi nghiên cứu: 100% tài chính cá nhân độc lập (không phân chia nhóm, không quỹ chung).
4. **Phần 2: Tiến độ khảo sát hiện trạng**
   - 2.1. Khảo sát đối tượng người dùng (3 Chân dung Persona định tính: Sinh viên, Người mới đi làm, Người có mục tiêu tài chính).
   - 2.2. Khảo sát thị trường & Đối chiếu ứng dụng hiện có (Bảng 2: YNAB, Wallet by BudgetBakers, Spendee, Money Manager, Excel).
   - 2.3. Hạn chế cốt lõi thị trường & Khoảng trống nghiên cứu (Research Gap): Gánh nặng nhập tay, Cảnh báo thụ động, Thiếu tối ưu nợ $\rightarrow$ Tính mới: Ngân sách thích ứng (Adaptive Budgeting).
   - 2.4. Bảng 3: Tổng hợp yêu cầu chức năng (9 phân hệ nghiệp vụ cốt lõi).
   - 2.5. Yêu cầu phi chức năng: Bảo mật AES-256, Tính toàn vẹn ACID, Thời gian phản hồi AI OCR < 1.5s.
5. **Phần 3: Tiến độ phân tích Use Case**
   - 3.1. Danh sách tác nhân: Primary Actor (Người dùng cá nhân) & Secondary Actors (AI OCR, Cổng VietQR, Push Notification).
   - 3.2. Bảng 4: Đặc tả tổng quát các gói Use Case (9 phân hệ).
   - 3.3. Sơ đồ Use Case tổng thể hệ thống (Tạo tác chuẩn hóa trên Draw.io `files/UCD-DuoNoPro.drawio`).
6. **Phần 4: Tiến độ thiết kế Class Diagram**
   - 4.1. Tóm tắt quy trình 6 bước phân tích hướng đối tượng (OOAD) của Giảng viên.
   - 4.2. Bảng 5: Đối chiếu sàng lọc loại bỏ lớp theo 6 tiêu chí học thuật.
   - 4.3. Thiết kế Rich Domain Model & Value Object bất biến (`Money`).
   - 4.4. Sơ đồ Class Diagram tổng thể hệ thống (Tạo tác chuẩn hóa trên Draw.io `files/ClassDiagram.drawio`).
7. **Phần 5: Đánh giá tiến độ hiện tại**
   - 5.1. Bảng 6: Đối soát tiến độ thực tế (100% hoàn thành các mục tiêu khảo sát và mô hình hóa giữa kỳ).
   - 5.2. Kế hoạch giai đoạn tiếp theo: Sequence Diagram, Activity Diagram, CSDL quan hệ 3NF và Giao diện người dùng UI Mockup.
8. **Phần Q&A: Vấn đáp kiến thức kinh tế & Giải nghĩa thuật ngữ viết tắt (Phụ lục hỗ trợ bảo vệ)**
   - Mục A (Bảng 7): Bảng tra cứu & giải nghĩa 12 thuật ngữ viết tắt (ACID, DDD, OOAD, UML, PFM, UCD, OCR, DSA, ZBB, DAG, 3NF, IPO).
   - Mục B: Bộ 10 câu hỏi lý thuyết kinh tế học hành vi & tài chính cá nhân súc tích, chuẩn xác.

---

## 3. Quy Chuẩn Kỹ Thuật Định Dạng Văn Bản (Tuân Thủ Quy Tắc DNTU)
- Khổ A4, lề chuẩn: Top `2.0 cm`, Bottom `2.0 cm`, Left `3.5 cm`, Right `2.0 cm`.
- 100% Font `Times New Roman`, thân bài 13pt, dãn dòng 1.5 lines, Before/After 4pt.
- Toàn bộ 7 bảng biểu đồng nhất theo **Academic Navy Theme (`#2B4C7E`)**, dòng xen kẽ (`#FFFFFF`/`#F8FAFC`), viền xám mỏng `#CBD5E1`.
- Tiêu đề bảng biểu đặt phía trên bảng, **In nghiêng (`Italic = True`)**, cỡ chữ 11.5 pt.
- Triệt tiêu 100% số liệu khảo sát giả định; ngôn từ chuẩn xác, không dùng từ "ma trận".
