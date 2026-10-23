# KẾ HOẠCH TRIỂN KHAI PLAN-29: TÍCH HỢP MÔ HÌNH ĐỘNG HỆ THỐNG VÀO KHÓA LUẬN TỐT NGHIỆP (MỤC 3.4)

> **Mã kế hoạch:** `PLAN-29`  
> **Ngày khởi tạo:** 2026-10-23  
> **Tài liệu mục tiêu:** [`files/KhoaLuanTotNghiep.docx`](../../files/KhoaLuanTotNghiep.docx)  
> **Nội dung:** Tích hợp toàn diện Mô hình Động (Dynamic Model) gồm Sơ đồ Tuần tự (Sequence Diagrams), Sơ đồ Hoạt động (Activity Diagrams) và Sơ đồ Trạng thái (State Machine Diagrams) vào **Mục 3.4** của Chương 3 Khóa luận Tốt nghiệp theo Phương án 1 đã được phê duyệt.

---

## 1. Bối Cảnh & Mục Tiêu

1. **Bối cảnh hiện tại:**
   * Chương 1, Chương 2 và các Mục 3.1 (Use Case), 3.2 (Đặc tả 60 Use Case), 3.3 (Class Diagram & Bounded Contexts) trong [`KhoaLuanTotNghiep.docx`](../../files/KhoaLuanTotNghiep.docx) đã hoàn thành xuất sắc và vượt qua các bài kiểm thử tự động.
   * **Mục 3.4 (hiện tại mang tên "Thiết kế tương tác và sơ đồ tuần tự hệ thống")** đang để trống hoàn toàn (Paragraph 289-291).
   * Dự án đã chuẩn bị đầy đủ 3 bộ sơ đồ kỹ thuật chuẩn UML 2.5 chất lượng cao trên Draw.io:
     - `files/SequenceDiagram.drawio` (5 Tabs).
     - `files/ActivityDiagram.drawio` (3 Tabs Vertical Swimlanes).
     - `files/StateDiagram.drawio` (3 Tabs Zero-Crossing Lifecycle).

2. **Mục tiêu của PLAN-29:**
   * Tái cấu trúc và phát triển **Mục 3.4: Thiết kế mô hình động và tương tác hệ thống** thành 3 tiểu mục hoàn chỉnh:
     * `3.4.1 Thiết kế tương tác và Sơ đồ tuần tự hệ thống (Sequence Diagrams)` (5 sơ đồ).
     * `3.4.2 Thiết kế luồng quy trình nghiệp vụ với Sơ đồ hoạt động (Activity Diagrams)` (3 sơ đồ).
     * `3.4.3 Mô hình hóa vòng đời thực thể miền với Sơ đồ trạng thái (State Machine Diagrams)` (3 sơ đồ).
   * Giữ nguyên vẹn thứ tự các mục tiếp theo: `3.5 Thiết kế cơ sở dữ liệu` và `3.6 Thiết kế kiến trúc phần mềm và giao diện người dùng`.
   * Tuân thủ 100% Bộ quy chuẩn DNTU (`01_dntu_kltn_format_rules.md`, `02_dntu_kltn_structure_and_toc_rules.md`, `03_dntu_kltn_tables_and_figures_rules.md`, `04_dntu_kltn_academic_writing_style_rules.md`):
     - Tuyệt đối không can thiệp Khối thủ tục đầu (Mục 01 đến 10).
     - Font Times New Roman 13pt, căn đều hai bên, line 1.5, space 6pt/6pt, thụt đầu dòng 1.5 cm.
     - Captions tự động chuẩn Word OpenXML (`Hình 3.18` đến `Hình 3.28`).
     - Văn phong học thuật, loại bỏ hoàn toàn từ ngữ sáo rỗng AI.

---

## 2. Cấu Trúc Nội Dung Chi Tiết Mục 3.4 Được Bổ Sung

```text
3.4 Thiết kế mô hình động và tương tác hệ thống
├── Đoạn văn dẫn nhập lý thuyết Mô hình động UML 2.5 (Tương tác, Luồng công việc, Vòng đời trạng thái)
│
├── 3.4.1 Thiết kế tương tác và Sơ đồ tuần tự hệ thống (Sequence Diagrams)
│   ├── Đoạn văn lý thuyết đặc tả ngữ cảnh tương tác (Lifeline, Messages, Combined Fragments alt/opt/loop)
│   ├── Kịch bản SD-01: Bóc tách hóa đơn AI Vision OCR & Hậu kiểm toán số học
│   │   └── Caption: Hình 3.18: Sơ đồ tuần tự bóc tách hóa đơn AI Vision OCR và hậu kiểm toán số học
│   ├── Kịch bản SD-02: Ghi nhận chi tiêu bằng giọng nói (Voice AI NLP)
│   │   └── Caption: Hình 3.19: Sơ đồ tuần tự ghi nhận chi tiêu bằng giọng nói thông minh
│   ├── Kịch bản SD-03: Thanh toán VietQR & Ghi sổ cái kép ACID
│   │   └── Caption: Hình 3.20: Sơ đồ tuần tự thanh toán VietQR và hạch toán số dư đa ví
│   ├── Kịch bản SD-04: Động cơ Hạn mức DSA thích ứng & Cú hích Thaler Nudge
│   │   └── Caption: Hình 3.21: Sơ đồ tuần tự tính toán hạn mức DSA và kích hoạt cú hích hành vi
│   └── Kịch bản SD-05: Đánh giá chi tiêu lớn & Khoảng trễ tâm lý Cooling-off 24h
│       └── Caption: Hình 3.22: Sơ đồ tuần tự kiểm soát chi tiêu lớn với khoảng trễ tâm lý 24 giờ
│
├── 3.4.2 Thiết kế luồng quy trình nghiệp vụ với Sơ đồ hoạt động (Activity Diagrams)
│   ├── Đoạn văn lý thuyết mô hình hóa luồng xử lý phân vai (Vertical Swimlanes, Decision, Fork/Join)
│   ├── Quy trình AD-01: Bóc tách hóa đơn AI Vision OCR & Kiểm toán đối soát
│   │   └── Caption: Hình 3.23: Sơ đồ hoạt động quy trình bóc tách hóa đơn AI Vision OCR và kiểm toán số học
│   ├── Quy trình AD-02: Kiểm soát mua sắm lớn và Khoảng trễ tâm lý Cooling-off 24h
│   │   └── Caption: Hình 3.24: Sơ đồ hoạt động quy trình kiểm soát mua sắm lớn với khoảng trễ tâm lý 24 giờ
│   └── Quy trình AD-03: Tính toán hạn mức ngày DSA thích ứng và Bắn cú hích Nudge
│       └── Caption: Hình 3.25: Sơ đồ hoạt động quy trình tính toán hạn mức DSA và phát tín hiệu cú hích Nudge
│
└── 3.4.3 Mô hình hóa vòng đời thực thể miền với Sơ đồ trạng thái (State Machine Diagrams)
    ├── Đoạn văn lý thuyết vòng đời thực thể (Entity Lifecycle, Event [Guard] / Action, Zero-Crossing)
    ├── Thực thể State-01: Vòng đời Giao dịch Chi tiêu (Expense Entity Lifecycle)
    │   └── Caption: Hình 3.26: Sơ đồ trạng thái vòng đời thực thể giao dịch chi tiêu cá nhân
    ├── Thực thể State-02: Vòng đời Kế hoạch Trả nợ Cá nhân (Debt Plan Lifecycle)
    │   └── Caption: Hình 3.27: Sơ đồ trạng thái vòng đời kế hoạch trả nợ cá nhân và quản trị rủi ro nợ
    └── Thực thể State-03: Vòng đời Cú hích Hành vi và Giải trình Minh bạch (Behavioral Nudge & XAI Lifecycle)
        └── Caption: Hình 3.28: Sơ đồ trạng thái vòng đời cú hích hành vi và giải trình thuật toán minh bạch
```

---

## 3. Danh Mục 11 Hình Ảnh & Cấu Trúc Đánh Số Caption Tiếp Nối

Tất cả các hình ảnh được đánh số tuần tự tiếp nối từ `Hình 3.17` (Context 6) đến `Hình 3.28`:

| STT | Mã số | Tên chính thức của Hình ảnh | Nguồn tệp gốc Draw.io | Vị trí đề mục |
| :---: | :---: | :--- | :--- | :---: |
| 1 | **Hình 3.18** | *Sơ đồ tuần tự bóc tách hóa đơn AI Vision OCR và hậu kiểm toán số học* | `SequenceDiagram.drawio` (Tab 1) | Mục 3.4.1 |
| 2 | **Hình 3.19** | *Sơ đồ tuần tự ghi nhận chi tiêu bằng giọng nói thông minh* | `SequenceDiagram.drawio` (Tab 2) | Mục 3.4.1 |
| 3 | **Hình 3.20** | *Sơ đồ tuần tự thanh toán VietQR và hạch toán số dư đa ví* | `SequenceDiagram.drawio` (Tab 3) | Mục 3.4.1 |
| 4 | **Hình 3.21** | *Sơ đồ tuần tự tính toán hạn mức DSA và kích hoạt cú hích hành vi* | `SequenceDiagram.drawio` (Tab 4) | Mục 3.4.1 |
| 5 | **Hình 3.22** | *Sơ đồ tuần tự kiểm soát chi tiêu lớn với khoảng trễ tâm lý 24 giờ* | `SequenceDiagram.drawio` (Tab 5) | Mục 3.4.1 |
| 6 | **Hình 3.23** | *Sơ đồ hoạt động quy trình bóc tách hóa đơn AI Vision OCR và kiểm toán số học* | `ActivityDiagram.drawio` (Tab 1) | Mục 3.4.2 |
| 7 | **Hình 3.24** | *Sơ đồ hoạt động quy trình kiểm soát mua sắm lớn với khoảng trễ tâm lý 24 giờ* | `ActivityDiagram.drawio` (Tab 2) | Mục 3.4.2 |
| 8 | **Hình 3.25** | *Sơ đồ hoạt động quy trình tính toán hạn mức DSA và phát tín hiệu cú hích Nudge* | `ActivityDiagram.drawio` (Tab 3) | Mục 3.4.2 |
| 9 | **Hình 3.26** | *Sơ đồ trạng thái vòng đời thực thể giao dịch chi tiêu cá nhân* | `StateDiagram.drawio` (Tab 1) | Mục 3.4.3 |
| 10 | **Hình 3.27** | *Sơ đồ trạng thái vòng đời kế hoạch trả nợ cá nhân và quản trị rủi ro nợ* | `StateDiagram.drawio` (Tab 2) | Mục 3.4.3 |
| 11 | **Hình 3.28** | *Sơ đồ trạng thái vòng đời cú hích hành vi và giải trình thuật toán minh bạch* | `StateDiagram.drawio` (Tab 3) | Mục 3.4.3 |

---

## 4. Các Bước Triển Khai Kỹ Thuật

* **Bước 1 (Soạn thảo Báo cáo kỹ thuật Markdown nguồn):**
  * Biên soạn tệp [`project_context/reports/BaoCao_ThietKe_MoHinhDong_Dynamic_Modeling_PFM.md`](../reports/BaoCao_ThietKe_MoHinhDong_Dynamic_Modeling_PFM.md) chứa toàn văn phân tích học thuật sâu sắc của cả 3 tiểu mục (3.4.1, 3.4.2, 3.4.3) theo chuẩn IEEE và giáo trình đại học, loại bỏ AI buzzwords.
* **Bước 2 (Tạo bản sao lưu an toàn):**
  * Sao lưu tệp `files/KhoaLuanTotNghiep.docx` thành `files/KhoaLuanTotNghiep.bak_pre_plan29.docx`.
* **Bước 3 (Xây dựng script Python chèn nội dung OpenXML):**
  * Phát triển script `insert_dynamic_models_to_kltn.py` sử dụng thư viện `python-docx` để nạp chính xác các đoạn văn bản, đề mục Cấp 1 (`Heading 2`), đề mục Cấp 2 (`Heading 3`), và 11 đoạn Captions với Paragraph Style `Caption` và mã trường OpenXML `{ SEQ Hình \* ARABIC }` chuẩn.
* **Bước 4 (Kiểm thử tự động & Báo cáo kết quả):**
  * Mở rộng kiểm thử trong `tests/test_docx_academic_format.py` và `tests/test_docx_captions_and_references.py` để bảo đảm tổng số hình đạt 28 hình (`Hình 3.1` đến `Hình 3.28`), không vỡ định dạng trang, không ảnh hưởng Section 1 (Khối thủ tục đầu).
