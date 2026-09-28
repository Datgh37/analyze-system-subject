---
trigger: model_decision
description: Quy định hình thức, định dạng văn bản, căn lề trang, font chữ, giãn dòng, đề mục và bullet points cho Khóa luận Tốt nghiệp DNTU.
---

# BỘ QUY ĐỊNH HÌNH THỨC & ĐỊNH DẠNG VĂN BẢN KHOÁ LUẬN TỐT NGHIỆP (DNTU) - PHẦN 1
**ĐẠI HỌC CÔNG NGHỆ ĐỒNG NAI (DNTU)**

Tài liệu này quy định chi tiết chuẩn mực định dạng văn bản, căn lề trang, font chữ, hệ thống đề mục và quy cách sử dụng gạch đầu dòng của Khóa luận Tốt nghiệp (KLTN) theo Quyết định của Hiệu trưởng Trường Đại học Công nghệ Đồng Nai. Áp dụng bắt buộc cho các tác tử AI và công cụ soạn thảo trong workspace.

---

## 1. NGUYÊN TẮC HÀNH VĂN HỌC THUẬT & SỬ DỤNG BULLET / GẠCH ĐẦU DÒNG

> [!IMPORTANT]
> **Ưu tiên văn phong diễn giải chuẩn mực học thuật (Academic Writing Style):**
> - Toàn bộ nội dung khóa luận phải được triển khai dưới dạng **các đoạn văn hoàn chỉnh (paragraphs)** có câu mở đoạn (topic sentence), câu phát triển luận điểm, dẫn chứng số liệu/kỹ thuật và câu kết đoạn/liên kết ý.
> - Tuyệt đối không biến bài viết thành bản ghi chú tóm tắt (bullet-point notes) hoặc slide thuyết trình.

### Quy định sử dụng Bullet & Gạch đầu dòng:
1. **Chỉ sử dụng khi thực sự cần thiết:**
   - Dùng để liệt kê danh sách các đối tượng độc lập: danh sách tác nhân (Actor), các thuộc tính thực thể (Domain Entities & Value Objects), các tiêu chí kỹ thuật, các nhóm đối tượng khảo sát (Persona), hoặc các bước rời rạc trong một quy trình/thuật toán phức tạp.
   - Không dùng gạch đầu dòng để diễn giải các phân tích sâu hay các lập luận lý thuyết. Nếu nội dung có thể diễn đạt bằng câu văn liên tục, bắt buộc chuyển thành các đoạn văn học thuật hoàn chỉnh (Paragraphs) với thụt đầu dòng chuẩn `1.5 cm`, không dùng bullet.
2. **Ký hiệu Bullet thống nhất:**
   - **Thống nhất sử dụng duy nhất ký hiệu dấu gạch ngang:** `"- "` (hyphen space).
   - Tuyệt đối không dùng chấm tròn `•`, không dùng gạch dài en-dash `–` hay em-dash `—` để làm bullet.
3. **Luôn có câu dẫn dắt (Lead-in Sentence):**
   - Trước khi mở một danh sách liệt kê, bắt buộc phải có câu dẫn dắt trọn vẹn kết thúc bằng dấu hai chấm (`:`).
4. **Quy chuẩn định dạng thụt lề cho danh sách liệt kê (Dạng Phân Ý: Tab 1.5cm, Nội dung thụt 0.75cm):**
   - Áp dụng cấu trúc thụt đầu dòng giống như văn bản (dòng 1 thụt vào, các dòng sau lồi hơn dòng 1), nhưng khối nội dung vẫn thụt vào 1/2 tab so với lề trang để phân biệt rõ đây là nội dung phân ý con:
     * **Dòng đầu tiên (chứa dấu gạch ngang `"- "`):** Bắt đầu ở mốc **`1.5 cm`** (bằng đúng Tab đầu dòng của đoạn văn bản thông thường).
     * **Các dòng tiếp theo (từ dòng 2 trở đi khi rớt dòng):** Căn thẳng lề ở mốc **`0.75 cm`** (thụt vào đúng 1/2 Tab thông thường so với mép lề trái `0.0 cm`, không ngang bằng văn bản thường để phân biệt rõ tính phân cấp).
     * **Thông số kỹ thuật trong Word:** `Left Indent = 0.75 cm` (21.3 pt), `First Line Indent = 0.75 cm` (21.3 pt), bảo đảm Dòng 1 = `0.75 + 0.75 = 1.5 cm` và Dòng 2+ = `0.75 cm`.
   - Khoảng cách đoạn và dãn dòng (giữ nguyên đồng bộ toàn tài liệu): `Space Before = 6 pt`, `Space After = 6 pt`, `Line Spacing = 1.5 lines`.
   - **Cấu hình Default Tab Stop trong Word (`settings.xml`):** Bắt buộc thiết lập `<w:defaultTabStop w:val="850"/>` (tương đương `1.5 cm`), không để mặc định của Word là 720 dxa (1.27 cm / 1.25 cm).
5. **Quy tắc dấu câu trong danh sách:**
   - Nếu mỗi mục là một câu hoàn chỉnh: Viết hoa đầu dòng và kết thúc bằng dấu chấm (`.`).
   - Nếu mỗi mục là các vế liệt kê tiếp nối câu dẫn: Kết thúc bằng dấu chấm phẩy (`;`), và mục cuối cùng kết thúc bằng dấu chấm (`.`).

---

## 2. QUY ĐỊNH VỀ CĂN LỀ & ĐỊNH DẠNG TRANG (PAGE SETUP)

- **Khổ giấy:** A4 chuẩn (`210 mm x 297 mm`).
- **Căn lề (Page Margins):**
  - **Lề trên (Top Margin):** `2.0 cm` (56.7 pt).
  - **Lề dưới (Bottom Margin):** `2.0 cm` (56.7 pt).
  - **Lề trái (Left Margin):** `3.5 cm` (99.2 pt) *(khoảng cách bắt buộc để đóng gáy và may bìa mạ vàng)*.
  - **Lề phải (Right Margin):** `2.0 cm` (56.7 pt).
- **Header & Footer:**
  - **Tuyệt đối không sử dụng Header và Footer nội dung văn bản.**
  - Chỉ đặt duy nhất số trang ở Footer.

---

## 3. QUY ĐỊNH VỀ ĐÁNH SỐ TRANG (PAGINATION)

- **Vị trí số trang:** Đặt ở **chính giữa lề dưới** trang giấy (Bottom Center).
- **Phân định 2 vùng đánh số (Section Break):**
  1. **Từ Mục 01 đến Mục 10 (Khối thủ tục đầu):** Đánh số trang theo thứ tự số La Mã thường: `i`, `ii`, `iii`, `iv`, `v`, ...
  2. **Từ Mục 11 (Nội dung các Chương) đến hết tài liệu:** Bắt đầu đánh số trang lại từ **`1`** bằng hệ thống số Ả Rập (`1`, `2`, `3`, `4`,...).

---

## 4. QUY ĐỊNH VỀ FONT CHỮ & ĐOẠN VĂN (TYPOGRAPHY & PARAGRAPH)

- **Font chữ:** `Times New Roman`, bảng mã Unicode dựng sẵn.
- **Cỡ chữ nội dung (Body text):** `13 pt`.
- **Dãn dòng (Line spacing):** Đặt ở chế độ `1.5 lines`.
- **Khoảng cách đoạn (Paragraph spacing):**
  - `Before`: `6 pt`.
  - `After`: `6 pt`.
- **Thụt đầu dòng đoạn văn:**
  - Đầu mỗi paragraph sử dụng phím Tab = `1.5 cm` (First line indent = 1.5 cm).
- **Căn lề đoạn văn:** Căn đều hai bên (`Justified / Both`).

> [!CAUTION]
> **ĐIỀU CẤM NGHIÊM NGẶT — KHỐI THỦ TỤC ĐẦU (CÁC TRANG SỐ LA MÃ THƯỜNG - MỤC 01 ĐẾN MỤC 10):**
> Người dùng tự tay tinh chỉnh và quản lý toàn bộ các trang thuộc Khối thủ tục đầu (Trang bìa ngoài, Trang bìa lót, Nhiệm vụ KLTN `BM205`, Lời cam đoan `BM206`, Lời cảm ơn `BM207`, Nhận xét GVHD `BM208`, Mục lục `BM209`, Danh mục hình ảnh `BM210`, Danh mục bảng biểu `BM211`, Danh mục viết tắt `BM212`...).
> **TUYỆT ĐỐI KHÔNG ĐƯỢC TỰ Ý CHỈNH SỬA, GHI ĐÈ, CAN THIỆP ĐỊNH DẠNG HAY THAY ĐỔI LAYOUT CỦA CÁC TRANG NÀY NẾU KHÔNG CÓ LỆNH RÕ RÀNG TỪ NGƯỜI DÙNG.** Mọi hoạt động cập nhật nội dung hay format chỉ được phép thực hiện từ **Chương 1 trở đi (Mục 11 - các trang đánh số Ả Rập)**.

---

## 5. QUY ĐỊNH VỀ TIÊU ĐỀ & HỆ THỐNG PHÂN CẤP ĐỀ MỤC (HEADINGS)

- **Số thứ tự:** Sử dụng hoàn toàn bằng hệ thống **số Ả Rập** (`1`, `2`, `3`...), **TUYỆT ĐỐI KHÔNG DÙNG SỐ LA MÃ** (`I`, `II`, `III`...).
- **Quy tắc đặt tên:** Các đề mục **không có dấu chấm hoặc dấu câu ở cuối**, **không viết hoa toàn bộ** (trừ tên Chương).

### Bảng phân cấp Đề mục chuẩn:

| Cấp đề mục | Ký hiệu phân cấp | Cỡ chữ | Kiểu dáng | Căn lề | Khoảng cách |
| :--- | :--- | :---: | :---: | :---: | :---: |
| **Tiêu đề Chương** | `CHƯƠNG 1: [TÊN CHƯƠNG]` | `14 pt` | **In đậm, VIẾT HOA** | Căn giữa | Before: 18pt, After: 12pt |
| **Đề mục Cấp 1** | `1.1 [Tên đề mục cấp 1]` | `13 pt` | **In đậm, viết hoa đầu từ** | Căn trái | Before: 12pt, After: 6pt |
| **Đề mục Cấp 2** | `1.1.1 [Tên đề mục cấp 2]` | `13 pt` | **In đậm, viết hoa đầu từ** | Căn trái | Before: 6pt, After: 6pt |
| **Đề mục Cấp 3** | `1.1.1.1` hoặc Persona/Tiểu mục | `13 pt` | **In đậm, in nghiêng** | Căn trái | Before: 6pt, After: 4pt |

---

## 6. HỆ THỐNG CÁC QUY ĐỊNH LIÊN QUAN (MODULAR RULES DIRECTORY)

Để tối ưu hóa dung lượng ngữ cảnh, các quy định chi tiết khác đã được module hóa thành các tệp chuyên trách:
- **Cấu trúc, Mục lục 2 lớp & Bookmarks:** Xem [`02_dntu_kltn_structure_and_toc_rules.md`](02_dntu_kltn_structure_and_toc_rules.md).
- **Thiết kế Bảng biểu (Academic Navy) & Hình ảnh (Draw.io, Captions):** Xem [`03_dntu_kltn_tables_and_figures_rules.md`](03_dntu_kltn_tables_and_figures_rules.md).
- **Văn phong học thuật, Loại bỏ AI-isms & Báo cáo Markdown:** Xem [`04_dntu_kltn_academic_writing_style_rules.md`](04_dntu_kltn_academic_writing_style_rules.md).
