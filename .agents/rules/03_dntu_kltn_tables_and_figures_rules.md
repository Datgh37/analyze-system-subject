---
trigger: model_decision
description: Quy định thiết kế bảng biểu (Academic Navy Theme #2B4C7E), hình ảnh, sơ đồ Draw.io và tiêu đề/chú thích in nghiêng cỡ nhỏ cho Khóa luận Tốt nghiệp DNTU.
---

# BỘ QUY ĐỊNH THIẾT KẾ BẢNG BIỂU, HÌNH ẢNH & SƠ ĐỒ KHOÁ LUẬN TỐT NGHIỆP (DNTU) - PHẦN 3
**ĐẠI HỌC CÔNG NGHỆ ĐỒNG NAI (DNTU)**

Tài liệu này quy định chi tiết chuẩn mực thiết kế trực quan cho bảng biểu, hình ảnh, biểu đồ và sơ đồ kỹ thuật trong Khóa luận Tốt nghiệp (KLTN) theo Quyết định của Hiệu trưởng Trường Đại học Công nghệ Đồng Nai. Áp dụng bắt buộc cho các tác tử AI và công cụ soạn thảo trong workspace.

---

## 1. BẢNG BIỂU (TABLES) — QUY CHUẨN THIẾT KẾ ĐỒNG NHẤT (ACADEMIC NAVY THEME)

> [!IMPORTANT]
> **Quy định thiết kế bảng biểu duy nhất cho toàn bộ Khóa luận (Bắt buộc đồng nhất 100%):**
> Tuyệt đối không sử dụng các màu ngẫu nhiên hoặc mỗi bảng một mẫu thiết kế khác nhau. Mọi bảng biểu trong Khóa luận tốt nghiệp (`KhoaLuanTotNghiep.docx`) bắt buộc phải tuân theo bộ quy chuẩn giao diện học thuật duy nhất sau đây:
> - **Dòng tiêu đề (Header Row - Row 0):**
>   - Màu nền (Shading Fill): **Xanh Navy Học Thuật (`#2B4C7E`)**.
>   - Màu chữ (Font Color): **Trắng tinh khiết (`#FFFFFF`)**, in đậm (`Bold = True`).
>   - Căn lề: Căn giữa (`Center`).
>   - Đệm ô (Padding): `Top = 100 dxa` (~5 pt), `Bottom = 100 dxa`, `Left = 120 dxa` (~6 pt), `Right = 120 dxa`.
> - **Thân bảng & Dòng dữ liệu xen kẽ (Zebra Striping):**
>   - Dòng lẻ (Row 1, 3, 5...): Màu nền **Trắng (`#FFFFFF`)**.
>   - Dòng chẵn (Row 2, 4, 6...): Màu nền **Xanh băng nhạt (`#F8FAFC`)**.
>   - **Nội dung thân bảng (Data Rows):** **TUYỆT ĐỐI KHÔNG IN ĐẬM (`Bold = False`)** cho toàn bộ các ô dữ liệu (bao gồm cả cột STT, tên thuộc tính, mã chỉ mục và nội dung mô tả) theo đúng nguyên văn quy định DNTU (*"Nội dung trong bảng không in đậm và khoảng cách dòng 1,5 lines"*).
>   - **Cỡ chữ nội dung bảng:** Ưu tiên sử dụng đồng bộ cỡ chữ **`13 pt`** (hoặc `12 pt` - `13 pt` theo chỉ đạo của Người dùng để chữ to rõ nét, dễ đọc trên bản in và đồng nhất với thân bài; tối thiểu `11 pt` với bảng quá nhiều cột), chữ màu đen tự nhiên (`#000000`).
>   - Đệm ô: `Top = 80 dxa` (~4 pt), `Bottom = 80 dxa`, `Left = 120 dxa`, `Right = 120 dxa`.
> - **Định dạng viền bảng (Borders):**
>   - Đường kẻ đơn mỏng (`single`), màu xám nhạt tinh tế (`#CBD5E1` hoặc `#D3D3D3`), độ dày 0.5 pt (`sz="4"`), áp dụng cho toàn bộ đường biên ngoài và đường phân cách bên trong.
> - **Căn lề bảng:** Toàn bộ bảng bắt buộc căn giữa trang giấy (`w:jc w:val="center"`).
> - **Đánh số & Tiêu đề/Chú thích bảng (Table Captions):**
>   - Đánh số theo số thứ tự của Chương: `Bảng [Số Chương].[Số thứ tự bảng trong chương]` (Ví dụ: `Bảng 3.1`, `Bảng 3.2`...).
>   - Vị trí: Đặt **ngay phía trên bảng**, căn giữa trang giấy.
>   - Định dạng bắt buộc: **In nghiêng (`Italic = True`)**, dùng **cỡ chữ nhỏ hơn thân bài** (`11 pt` hoặc `12 pt`, so với thân bài 13 pt). Phần số hiệu có thể in đậm in nghiêng (Ví dụ: ***Bảng 3.1:*** *Danh mục các tác nhân và vai trò trong hệ thống* hoặc in nghiêng toàn bộ *Bảng 3.1: Danh mục các tác nhân...*).
>   - Khoảng cách: `Space Before = 12 pt`, `Space After = 6 pt`.
>   - **Gắn Caption Word Tự Động (Bắt buộc):** Đoạn tiêu đề bảng phải được gắn tính năng **Insert Caption** của Microsoft Word với nhãn Label là `"Bảng"` (sử dụng trường tự động `{ SEQ Bảng \* ARABIC }` hoặc kết hợp `{ STYLEREF 1 \s }.{ SEQ Bảng \* ARABIC \s 1 }`).
> - **Ghi chú/chú thích dưới bảng biểu (Table Notes):** Đặt ngay dưới bảng, in nghiêng, cỡ `10 pt` hoặc `11 pt`, màu xám sẫm, căn giữa.

---

## 2. HÌNH ẢNH & SƠ ĐỒ KỸ THUẬT (FIGURES & CHARTS)

- **Đánh số & Tiêu đề/Chú thích hình (Figure Captions):**
  - Đánh số theo số thứ tự của Chương: `Hình [Số Chương].[Số thứ tự hình trong chương]` (Ví dụ: `Hình 1.1`, `Hình 2.1`...).
  - Vị trí: Đặt **ngay phía dưới hình**, căn giữa trang giấy.
  - Định dạng bắt buộc: **In nghiêng (`Italic = True`)**, dùng **cỡ chữ nhỏ hơn thân bài** (`11 pt` hoặc `12 pt`, so với thân bài 13 pt). Phần số hiệu có thể in đậm in nghiêng (Ví dụ: ***Hình 3.1:*** *Sơ đồ Use Case tổng thể hệ thống* hoặc in nghiêng toàn bộ).
  - Khoảng cách: `Space Before = 6 pt`, `Space After = 12 pt`.
  - **Gắn Caption Word Tự Động (Bắt buộc):** Đoạn tiêu đề hình phải được gắn tính năng **Insert Caption** của Microsoft Word với nhãn Label là `"Hình"` (sử dụng trường tự động `{ SEQ Hình \* ARABIC }` hoặc kết hợp `{ STYLEREF 1 \s }.{ SEQ Hình \* ARABIC \s 1 }`).
- **Tiêu chuẩn công cụ vẽ sơ đồ (Mandatory Diagramming Tool):**
  - **Draw.io (`.drawio`) là công cụ ưu tiên hàng đầu và chuẩn hóa duy nhất** cho toàn bộ các biểu đồ thiết kế kỹ thuật trong đồ án: Sơ đồ Use Case (UCD), Sơ đồ Lớp (Class Diagram - CD), Sơ đồ Tuần tự (Sequence Diagram - SD), Sơ đồ Hoạt động (Activity Diagram - AD), Sơ đồ Gói (Package Diagram) và Sơ đồ Kiến trúc.
  - **Lý do & Tiêu chuẩn:** Giảng viên hướng dẫn yêu cầu sự thống nhất tuyệt đối về mặt phong cách thị giác (Visual Consistency), bảng màu (Color Palette), tỷ lệ hình khối và typography nét chữ (Segoe UI / Arial / Times New Roman rõ nét, không mờ vỡ khi phóng to thu nhỏ) trên toàn bộ báo cáo Khóa luận tốt nghiệp (`KhoaLuanTotNghiep.docx`).
  - **Lưu trữ mã nguồn sơ đồ:** Mọi biểu đồ phải được lưu tệp gốc định dạng `.drawio` (hoặc Draw.io XML) trong thư mục `files/` để có thể bảo trì, mở rộng và xuất ảnh độ phân giải cao (300 DPI, nền trong suốt hoặc trắng tinh khiết).
- **Chất lượng hình ảnh:** Hình ảnh/sơ đồ phải có độ phân giải cao, rõ chữ, không bị vỡ hạt hay méo tỷ lệ.
- **Ghi chú dưới hình:** Nếu có chú thích các bộ phận trong hình thì ghi chú ngay dưới tên hình bằng chữ in nghiêng, cỡ `10 pt` - `11 pt`.

---

## 3. QUY ĐỊNH CẤM NHẬP TAY DANH MỤC & PHÂN BIỆT VỚI MỤC LỤC VĂN BẢN

> [!CAUTION]
> **CẤM TUYỆT ĐỐI NHẬP TAY DANH MỤC HÌNH ẢNH & BẢNG BIỂU:**
> 1. **Không nhập số trang thủ công:** Trang Danh mục sơ đồ, hình ảnh (`BM210-KT21`) và Danh mục bảng biểu (`BM211-KT21`) tuyệt đối **KHÔNG ĐƯỢC NHẬP TAY** từng dòng tiêu đề và số trang.
> 2. **Không dùng Header Reference (`\o "1-3"`):** Tuyệt đối không gán Heading 1, 2, 3 cho tiêu đề bảng biểu/hình ảnh để lôi vào mục lục chung. Làm như vậy sẽ làm ô nhiễm cây điều hướng Navigation Pane (`Ctrl + F`).
> 3. **Cơ chế bắt buộc:** Phải dùng tính năng **Table of Figures (TOF)** và **Table of Tables (TOT)** của Word:
>    - Danh mục hình ảnh: Tạo bằng mã trường `{ TOC \h \z \c "Hình" }` (quét toàn bộ các đoạn có Caption nhãn `"Hình"`).
>    - Danh mục bảng biểu: Tạo bằng mã trường `{ TOC \h \z \c "Bảng" }` (quét toàn bộ các đoạn có Caption nhãn `"Bảng"`).
>    - Khi người dùng nhấn `F9` (hoặc click chuột phải chọn *Update Field*), Word sẽ tự động trích xuất danh sách và cập nhật số trang chính xác 100%.

