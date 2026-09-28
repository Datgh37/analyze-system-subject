# Kế Hoạch Triển Khai: PLAN-14 — Tích Hợp Nội Dung Chương 3 Khóa Luận Tốt Nghiệp & Báo Cáo Tổng Hợp Toàn Diện Tiến Độ Dự Án

> **Mã kế hoạch:** `PLAN-14`  
> **Thời điểm lập:** `2026-09-17 11:35:00`  
> **Phạm vi trọng tâm:** Chương 3 & Chương 4 Khóa luận tốt nghiệp (`KhoaLuanTotNghiep.docx`), Báo cáo tổng kết toàn bộ tiến độ đã hoàn thành  
> **Trạng thái:** `PENDING_REVIEW`

---

## 1. Bối Cảnh & Chỉ Đạo Của Người Dùng

Sau khi Hội đồng Đa tác tử hoàn thành xuất sắc 100% việc nghiên cứu và đặc tả chi tiết 60 Use Cases chuẩn IEEE 29148 (410 KB) tại PLAN-13, Người Dùng chỉ đạo:
1. **Phân kỳ kế hoạch thành 2 giai đoạn:**
   - **Giai đoạn 1 (Triển khai ngay):** 
     - Xây dựng nội dung Chương 3 trong `files/KhoaLuanTotNghiep.docx` dựa trên toàn bộ kết quả đã hoàn thành (Mục 3.1 Yêu cầu & Use Case 10 Tabs, Mục 3.2 Đặc tả Use Case chi tiết, Mục 3.3 Class Diagram DDD 6 Bounded Contexts).
     - Điền trước và gắn Bookmark OpenXML `_Toc_...` cho tất cả các đề mục của Chương 3 để hỗ trợ cập nhật Mục lục Word tự động (phím F9).
     - Dự trù chính xác danh mục 17 hình ảnh minh họa (10 ảnh Use Case Diagram + 7 ảnh Class Diagram chia theo Bounded Context).
     - Cập nhật cấu trúc luận văn: Sử dụng phương án **gộp Chương 4** thành `CHƯƠNG 4: HIỆN THỰC, KIỂM THỬ VÀ ĐÁNH GIÁ HỆ THỐNG` (chuẩn 5 chương khớp với trang nhiệm vụ BM205).
     - Xuất 1 tệp Markdown tổng hợp toàn bộ những gì đã làm được từ giai đoạn đầu đến hiện tại (chỉ việc đã hoàn thành, không bao gồm kế hoạch tương lai).
   - **Giai đoạn 2 (Lập kế hoạch trước, triển khai sau):** 
     - Lập kế hoạch chi tiết cho các nội dung kỹ thuật còn lại của Chương 3 (Mục 3.4 Sơ đồ tuần tự Sequence Diagram, Mục 3.5 Thiết kế CSDL vật lý 3NF/DDL, Mục 3.6 Kiến trúc & Wireframes Atomic UI).

---

## 2. Chi Tiết Triển Khai Giai Đoạn 1

### 2.1. Cấu Trúc Nội Dung Chương 3 Trong Word (`files/KhoaLuanTotNghiep.docx`)

Tuân thủ nghiêm ngặt **Quy chuẩn Định dạng Khóa Luận Tốt Nghiệp DNTU** (`Times New Roman 13pt`, dãn dòng `1.5 lines`, khoảng cách đoạn `Before/After 6pt`, phân cấp Heading 1, 2, 3 và gắn mã trường Bookmark OpenXML chuẩn `_Toc_...` phục vụ phím F9 cập nhật Mục lục):

* **3.1. YÊU CẦU HỆ THỐNG VÀ MÔ HÌNH HÓA USE CASE**
  * *3.1.1. Khảo sát và xác định các tác nhân hệ thống (System Actors):* 
    * 1 Tác nhân người dùng cá nhân (Primary Human Actor: `Người dùng cá nhân / Individual Saver` — 100% PFM, bảo mật riêng tư tuyệt đối).
    * 3 Tác nhân hệ thống ngoại vi (Secondary System Actors: `AI Multimodal Vision OCR Engine`, `Cổng thanh toán VietQR EMVCo / NAPAS 247`, `Dịch vụ Thông báo & Cảnh báo Nudge`).
  * *3.1.2. Phân tích yêu cầu chức năng (FR) và phi chức năng (NFR):* 9 nhóm yêu cầu chức năng tương ứng 9 phân hệ và các yêu cầu phi chức năng nghiêm ngặt (ACID, SLA 200ms, AES-256).
  * *3.1.3. Mô hình Use Case tổng thể hệ thống (UCD 00):* Diễn giải cấu trúc 9 gói phân hệ + Khung placeholder & caption **Hình 3.1: Sơ đồ Use Case tổng thể hệ thống (UCD 00)**.
  * *3.1.4. Mô hình Use Case chi tiết 9 phân hệ chức năng:* Diễn giải 9 phân hệ kèm theo các khung placeholder ảnh từ **Hình 3.2 đến Hình 3.10**.

* **3.2. ĐẶC TẢ CHI TIẾT CÁC USE CASE (USE CASE SPECIFICATIONS)**
  * *3.2.1. Ma trận danh mục tổng quan 60 Use Cases theo 9 phân hệ:* Bảng tổng hợp đối chiếu mã UC, tên hành vi, tác nhân chính và mức độ ưu tiên.
  * *3.2.2. Đặc tả chi tiết các Use Case cốt lõi & cơ chế tham chiếu Phụ lục:*
    * Trình bày toàn văn các Use Case "linh hồn & cốt lõi học thuật" tiêu biểu nhất của từng phân hệ dưới dạng **Bảng Dọc 2 x N (13 mục chuẩn IEEE 29148)** ngay trong thân bài (Đăng nhập MFA, Chuyển tiền Đa ví ACID, AI OCR Hóa đơn, Hậu kiểm toán số học, Adaptive DSA, Tối ưu nợ Snowball/Avalanche, Báo cáo Net Worth, Thaler Nudge, Cooling-off Period).
    * Thiết lập hệ thống tham chiếu và liên kết dẫn thẳng xuống **PHỤ LỤC (APPENDIX)** nơi lưu trữ toàn văn 100% trọn vẹn 60 Use Cases (410 KB).

* **3.3. THIẾT KẾ MÔ HÌNH LỚP KHÁI NIỆM VÀ HƯỚNG MIỀN (DOMAIN CLASS DIAGRAM)**
  * *3.3.1. Phương pháp luận 5 giai đoạn thiết kế hướng đối tượng (OOAD) và 6 tiêu chí loại bỏ lớp học thuật:* Kế thừa toàn bộ phân tích loại trừ từ bài Lab 3.
  * *3.3.2. Phân rã 6 Bounded Contexts và đặc tả 20 Lớp (18 Domain Entities + 2 Value Objects):* Trình bày cấu trúc thuộc tính kiểu dữ liệu MSSQL và các phương thức nghiệp vụ phong phú (Rich Domain Methods).
  * *3.3.3. Hệ thống hình ảnh Class Diagram:* Phân rã thành 7 hình ảnh chuẩn khổ trang A4 (từ Hình 3.11 đến Hình 3.17).

* **3.4 đến 3.6. CÁC MỤC THIẾT KẾ TIẾP THEO:**
  * Điền sẵn khung Heading 2, 3 và Bookmark chờ sẵn, cùng các đoạn dẫn dắt phương pháp luận để sẵn sàng nạp nội dung ở Giai đoạn 2.

* **GỘP CHƯƠNG 4 (THEO CHỈ ĐẠO CỦA NGƯỜI DÙNG):**
  * Cập nhật tiêu đề: **`CHƯƠNG 4: HIỆN THỰC, KIỂM THỬ VÀ ĐÁNH GIÁ HỆ THỐNG`** (khớp với trang nhiệm vụ BM205).

---

### 2.2. Danh Mục Dự Trù 17 Hình Ảnh & Giải Pháp Class Diagram

```text
┌────────────────────────────────────────────────────────────────────────────────────────┐
│ DANH MỤC 17 HÌNH ẢNH DỰ TRÙ CHO CHƯƠNG 3 (CHÈN VÀO DOCX KÈM CAPTION CHUẨN DNTU)       │
├────────────┬─────────────────────────────────────────────────────────────┬─────────────┤
│ Ký hiệu    │ Tên Hình Ảnh / Sơ Đồ Minh Họa                               │ Nguồn Tệp   │
├────────────┼─────────────────────────────────────────────────────────────┼─────────────┤
│ Hình 3.1   │ Sơ đồ Use Case tổng thể hệ thống (UCD 00 - Tổng Quát)       │ Tab 0 UCD   │
│ Hình 3.2   │ Sơ đồ Use Case Phân hệ Quản lý Tài khoản & Đa Ví (UCD 01)   │ Tab 1 UCD   │
│ Hình 3.3   │ Sơ đồ Use Case Phân hệ Quản lý Thu nhập cá nhân (UCD 02)    │ Tab 2 UCD   │
│ Hình 3.4   │ Sơ đồ Use Case Phân hệ Chi tiêu & Bóc tách Hóa đơn AI (UCD 03)│ Tab 3 UCD │
│ Hình 3.5   │ Sơ đồ Use Case Phân hệ Danh mục Thu Chi & Quy tắc (UCD 04)  │ Tab 4 UCD   │
│ Hình 3.6   │ Sơ đồ Use Case Phân hệ Ngân sách & Hạn mức DSA (UCD 05)     │ Tab 5 UCD   │
│ Hình 3.7   │ Sơ đồ Use Case Phân hệ Quỹ Tích lũy & Tối ưu hóa Nợ (UCD 06)│ Tab 6 UCD   │
│ Hình 3.8   │ Sơ đồ Use Case Phân hệ Báo cáo & Phân tích Tài chính (UCD 07)│ Tab 7 UCD   │
│ Hình 3.9   │ Sơ đồ Use Case Phân hệ Dự báo Tài chính & Rủi ro (UCD 08)   │ Tab 8 UCD   │
│ Hình 3.10  │ Sơ đồ Use Case Phân hệ Cú hích Hành vi & Quyết định (UCD 09) │ Tab 9 UCD   │
├────────────┼─────────────────────────────────────────────────────────────┼─────────────┤
│ Hình 3.11  │ Sơ đồ kiến trúc tổng thể 6 Bounded Contexts miền PFM        │ Sơ đồ khối  │
│ Hình 3.12  │ Sơ đồ lớp Ngữ cảnh Định danh & Hồ sơ cá nhân (Context 1)    │ CD Context 1│
│ Hình 3.13  │ Sơ đồ lớp Ngữ cảnh Tài khoản & Đa ví cá nhân ACID (Context 2)│ CD Context 2│
│ Hình 3.14  │ Sơ đồ lớp Ngữ cảnh Giao dịch, AI OCR & Danh mục (Context 3)  │ CD Context 3│
│ Hình 3.15  │ Sơ đồ lớp Ngữ cảnh Ngân sách đa phương pháp & DSA (Context 4)│ CD Context 4│
│ Hình 3.16  │ Sơ đồ lớp Ngữ cảnh Quỹ tích lũy & Tối ưu hóa Nợ (Context 5) │ CD Context 5│
│ Hình 3.17  │ Sơ đồ lớp Ngữ cảnh Cú hích hành vi & Quyết định (Context 6) │ CD Context 6│
└────────────┴─────────────────────────────────────────────────────────────┴─────────────┘
```

**Giải pháp học thuật đưa Class Diagram vào luận văn:**
1. **Hình 3.11:** Sơ đồ kiến trúc tổng thể các Bounded Contexts (High-level Domain Map) cho cái nhìn bao quát.
2. **Hình 3.12 đến Hình 3.17:** Chia nhỏ bản vẽ thành 6 hình ảnh độc lập theo 6 Bounded Contexts. Mỗi hình vừa khít trang A4 dọc, font chữ 10-12pt rõ nét, đặt ngay trước bảng đặc tả chi tiết của ngữ cảnh đó.
3. Master Class Diagram toàn cảnh đặt ở Phụ lục để tra cứu.

---

### 2.3. Báo Cáo Tổng Hợp Toàn Bộ Tiến Độ Đã Hoàn Thành

* **Tệp xuất bản:** `project_context/reports/BaoCao_TongHop_ToanBo_TienDo_Da_Hoan_Thanh.md`.
* **Nội dung:** Tinh lọc và tổng kết toàn bộ thành tựu nghiên cứu và thiết kế đã hoàn thành 100%:
  1. Tuyên bố bài toán I-P-O & Ranh giới 100% PFM cá nhân (Chương 1).
  2. Kết quả khảo sát thực trạng, đối chiếu thị trường và 3 Personas (Chương 1).
  3. Cơ sở lý thuyết quản lý tài chính, kinh tế hành vi Thaler, AI Multimodal Vision OCR, DDD (Chương 2).
  4. Mô hình hóa Use Case 10 Tabs và 60 Use Case Specifications chuẩn IEEE 29148 (Chương 3).
  5. Thiết kế Sơ đồ Lớp DDD 6 Bounded Contexts, 20 Lớp (Lab 3) (Chương 3).
  6. Kết quả 4 Benchmark Sessions thực tế và cơ chế kiểm toán chất lượng Quality Gates.

---

## 3. Kế Hoạch Giai Đoạn 2: Các Nội Dung Còn Lại Của Chương 3

1. **Mục 3.4: Thiết kế động học hệ thống (Sequence Diagrams):**
   - Xây dựng 5 Sơ đồ tuần tự cốt lõi:
     - `SD-01`: Đăng nhập xác thực JWT Rotation & Chuyển tiền nội bộ đa ví với Khóa bi quan ACID (`UPDLOCK, ROWLOCK`).
     - `SD-02`: Pipeline AI Multimodal Vision OCR bóc tách hóa đơn & Thuật toán hậu kiểm toán số học $\sum = \text{Total}$.
     - `SD-03`: Thuật toán Hạn mức chi tiêu mỗi ngày thích ứng (Adaptive DSA Engine).
     - `SD-04`: Động cơ tối ưu hóa nợ cá nhân (Debt Solver: Snowball vs Avalanche).
     - `SD-05`: Cú hích tài chính Thaler Nudge thời gian thực & Khoảng trễ tâm lý Cooling-off Period 24h - 48h.
2. **Mục 3.5: Thiết kế Cơ sở dữ liệu vật lý (Database Physical Design 3NF/DDL):**
   - Chuyển hóa 18 Thực thể miền sang Lược đồ CSDL quan hệ 3NF (ERD vật lý).
   - Bộ từ điển dữ liệu (Data Dictionary): Định nghĩa khóa chính UUID/BIGINT, khóa ngoại, kiểu dữ liệu MSSQL/PostgreSQL, ràng buộc `CHECK`.
   - Chiến lược đánh chỉ mục Clustered/Non-Clustered Indexes phục vụ truy vấn dòng tiền tốc độ cao.
3. **Mục 3.6: Thiết kế Kiến trúc phần mềm & Giao diện người dùng (Architecture & Atomic UI):**
   - Sơ đồ kiến trúc Clean Architecture 4 tầng (Domain, Application, Infrastructure, Presentation).
   - Thiết kế Wireframes giao diện cốt lõi theo hệ thống Atomic Design (Atoms, Molecules, Organisms).
