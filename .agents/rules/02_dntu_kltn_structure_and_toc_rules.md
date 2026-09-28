---
trigger: model_decision
description: Quy định cấu trúc 14 mục bắt buộc, thiết lập Mục lục tự động hai lớp (TOC/TOF/TOT), Bookmarks OpenXML, chữ viết tắt và tài liệu tham khảo KLTN DNTU.
---

# BỘ QUY ĐỊNH CẤU TRÚC, MỤC LỤC & ĐIỀU HƯỚNG KHOÁ LUẬN TỐT NGHIỆP (DNTU) - PHẦN 2
**ĐẠI HỌC CÔNG NGHỆ ĐỒNG NAI (DNTU)**

Quy định cấu trúc 14 mục bắt buộc, quy chuẩn Mục lục tự động hai lớp (TOC, TOF, TOT), Bookmarks OpenXML, chữ viết tắt, trích dẫn tài liệu tham khảo và checklist kiểm toán Khóa luận Tốt nghiệp (KLTN).

---

## 1. CẤU TRÚC VÀ THỨ TỰ TRÌNH BÀY LUẬN VĂN (14 MỤC BẮT BUỘC)

Trình tự bắt buộc từ đầu đến cuối:

| STT | Thành phần | Biểu mẫu | Quy cách & Nội dung |
| :---: | :--- | :---: | :--- |
| **01** | **Trang bìa ngoài** | BM201-KT21 (1-4 SV) / BM203-KT21 (≥5 SV) | Bìa cứng màu xanh, in chữ nhũ mạ vàng, khổ 210 x 297 mm. |
| **02** | **Trang bìa lót** | BM202-KT21 / BM204-KT21 | In trên giấy A4 thường, nội dung tương tự trang bìa ngoài. |
| **03** | **Nhiệm vụ KLTN** | BM205-KT21 | Nhóm SV, chuyên ngành, tên đề tài, cấu trúc các chương, chữ ký GVHD, Trưởng bộ môn, Trưởng khoa. |
| **04** | **Lời cam đoan** | BM206-KT21 | Cam kết tính trung thực của số liệu, trích dẫn rõ ràng; ngày tháng và chữ ký nhóm SV. |
| **05** | **Lời cảm ơn** | BM207-KT21 | Tri ân nhà trường, khoa, GVHD, gia đình và bạn bè; chữ ký nhóm SV. |
| **06** | **Trang nhận xét của GVHD** | BM208-KT21 | Đánh giá tính cấp thiết, nội dung, thái độ, kết luận và điểm (/10); chữ ký GVHD. |
| **07** | **Trang mục lục** | BM209-KT21 | Liệt kê chi tiết các chương, mục phân cấp (`1.1`, `1.1.1`) kèm số trang. |
| **08** | **Trang danh mục sơ đồ, hình ảnh** | BM210-KT21 | Số hiệu, tên hình ảnh/sơ đồ và số trang (Hình 1.1, Hình 2.1...). |
| **09** | **Trang danh mục bảng biểu** | BM211-KT21 | Số hiệu, tên bảng và số trang (Bảng 1.1, Bảng 2.1...). |
| **10** | **Danh mục thuật ngữ & từ viết tắt** | BM212-KT21 | Bảng đối chiếu: Ký hiệu viết tắt – Thuật ngữ tiếng Anh – Ý nghĩa tiếng Việt. |
| **11** | **Nội dung các Chương** | - | Báo cáo nghiên cứu chính (50–80 trang, không tính trang phụ mục 01–10). |
| **12** | **Tiến độ thực hiện đề tài** | BM213-KT21 | Bảng phân kỳ công việc theo tuần/tháng và % hoàn thành. |
| **13** | **Tài liệu tham khảo** | BM214-KT21 | Xếp riêng theo từng ngôn ngữ (Việt, Anh...), xếp theo ABC tên tác giả. |
| **14** | **Phụ lục** | - | Đưa vào sau trang cuối: phiếu khảo sát, mã nguồn, logs kiểm thử... |

> [!CAUTION]
> **QUY ƯỚC QUẢN TRỊ KHỐI THỦ TỤC ĐẦU (MỤC 01 ĐẾN MỤC 10):**
> Người dùng trực tiếp quản lý và sửa đổi thủ công các trang này. **Tác tử AI TUYỆT ĐỐI KHÔNG can thiệp, không tự động sửa đổi, không ghi đè và không chạy script format nếu không có lệnh rõ ràng từ người dùng.**

---

## 2. QUY CHUẨN THIẾT LẬP MỤC LỤC & TỰ ĐỘNG HÓA TRUY VẾT

Mọi đề mục, bảng biểu và hình ảnh phải được liên kết chính xác, hỗ trợ cập nhật số trang tự động bằng phím **`F9`**. Tuyệt đối không gõ số trang và dấu chấm thủ công.

### 2.1. Cấu trúc và Phạm vi bao phủ của Trang Mục lục
1. **Khối thủ tục đầu (Preamble):** Đánh số La Mã thường (`iv`, `v`, `vi`...): LỜI CAM ĐOAN, LỜI CẢM ƠN, NHẬN XÉT GVHD, DANH MỤC HÌNH ẢNH, DANH MỤC BẢNG BIỂU, DANH MỤC TỪ VIẾT TẮT.
2. **Khối nội dung các Chương (Chương 1 đến 5):** Đánh số Ả Rập từ trang `1`:
   - Tiêu đề Chương (`Heading 1`): In đậm, thụt lề trái `0 cm`.
   - Đề mục Cấp 1 (`Heading 2` - `1.1`, `2.1`): In đậm, thụt lề trái `0.5 cm`.
   - Đề mục Cấp 2 (`Heading 3` - `1.1.1`): In thường, thụt lề trái `1.0 cm`.
3. **Khối kết thúc luận văn (End-matter):** TIẾN ĐỘ THỰC HIỆN KLTN, TÀI LIỆU THAM KHẢO, PHỤ LỤC.

### 2.2. Kiến trúc Mục lục hai lớp (Dual-Layer TOC Architecture)
1. **Lớp hiển thị trực quan (Tab Leader & Hyperlink):**
   - Định dạng: Tên đề mục + Tab (`\t`) + Số trang.
   - Tab Stop căn phải đặt tại `8790 dxa` (`15.5 cm` cho khổ A4 lề trái 3.5 cm, lề phải 2.0 cm) với `w:leader="dot"`.
   - Khoảng cách dòng: `1.3 lines` hoặc `1.5 lines`, `Space Before = 2 pt`, `Space After = 2 pt`.
   - Bọc thẻ `<w:hyperlink w:anchor="[Tên_Bookmark]" w:history="1">` để click chuột nhảy tới vị trí đề mục.
2. **Lớp mã trường động Microsoft Word (Dynamic Field Code):**
   - Mục lục chung (TOC): `<w:fldSimple w:instr="TOC \o &quot;1-3&quot; \h \z \u"/>`.
   - Cập nhật tự động: Nhấn `Ctrl + A` rồi bấm **`F9`** (hoặc chuột phải $\rightarrow$ *Update Field* $\rightarrow$ *Update entire table*).

### 2.3. Chuẩn hóa Bookmark OpenXML & Outline Level tại các Đề mục
- **Outline Level:** Heading 1 gán `<w:outlineLvl w:val="0"/>`; Heading 2 gán `val="1"`; Heading 3 gán `val="2"`.
- **Cặp thẻ Bookmark:** `<w:bookmarkStart w:id="[ID]" w:name="[Tên_Bookmark]"/>` đặt trước run văn bản, `<w:bookmarkEnd w:id="[ID]"/>` đặt cuối đoạn. Dùng tiền tố `_Toc_` chuẩn mực (`_Toc_Chuong1`, `_Toc_1_1`...). Mã `w:id` là số nguyên duy nhất.

### 2.4. Danh Mục Sơ Đồ, Hình Ảnh Tự Động (Table of Figures - TOF, BM210)
- **Cơ chế:** Quét toàn bộ đối tượng có Caption nhãn `"Hình"`.
- **Mã trường Word OpenXML:** `<w:fldSimple w:instr="TOC \h \z \c &quot;Hình&quot;"/>`.
- **Quy tắc:** Dùng tham số `\c "Hình"`, **tuyệt đối KHÔNG dùng `\o "1-3"`** (tránh ô nhiễm Navigation Pane). Không gõ tay số trang.
- **Hiển thị:** Tab Stop căn phải mép lề `15.5 cm` với `w:leader="dot"`, bọc thẻ `<w:hyperlink>` nội bộ.

### 2.5. Danh Mục Bảng Biểu Tự Động (Table of Tables - TOT, BM211)
- **Cơ chế:** Quét toàn bộ bảng biểu có Caption nhãn `"Bảng"`.
- **Mã trường Word OpenXML:** `<w:fldSimple w:instr="TOC \h \z \c &quot;Bảng&quot;"/>`.
- **Quy tắc:** Dùng tham số `\c "Bảng"`, **tuyệt đối KHÔNG dùng `\o "1-3"`**. Không gõ tay số trang.
- **Hiển thị:** Tab Stop căn phải mép lề `15.5 cm` với `w:leader="dot"`, bọc thẻ `<w:hyperlink>` nội bộ.
- **F9 toàn cục:** Bấm `F9` cập nhật đồng thời cả 3 danh mục: TOC, TOF và TOT.

---

## 3. QUY ĐỊNH VỀ CHỮ VIẾT TẮT

- Tuyệt đối không lạm dụng viết tắt bừa bãi.
- Bắt buộc có Bảng danh mục thuật ngữ và từ viết tắt (Mục 10) nếu sử dụng từ viết tắt.
- Khi xuất hiện lần đầu trong văn bản, phải viết đầy đủ tiếng Việt/tiếng Anh kèm từ viết tắt trong ngoặc đơn *(Ví dụ: "Nhận dạng ký tự quang học (OCR)...")*.

---

## 4. QUY ĐỊNH VỀ TRÍCH DẪN & TÀI LIỆU THAM KHẢO

- **Sắp xếp:** Xếp riêng khối tiếng Việt trước, tiếng nước ngoài sau. Xếp theo thứ tự ABC:
  - Tác giả nước ngoài: Xếp theo **Họ**.
  - Tác giả Việt Nam: Xếp theo **Tên** (giữ nguyên Họ + Đệm + Tên).
  - Không có tên tác giả: Xếp theo từ đầu tiên của cơ quan ban hành.
- **Định dạng:**
  - Sách, báo cáo: `[STT]. Tên tác giả (Năm), Tên sách/báo cáo (in nghiêng), NXB, Nơi xuất bản.`
  - Bài báo tạp chí: `[STT]. Tên tác giả (Năm), "Tên bài báo", Tên tạp chí (in nghiêng), Tập(số), trang-trang.`

---

## 5. CHECKLIST TỰ ĐỘNG KIỂM TRA TRƯỚC KHI NỘP

- [ ] Lề trang: Top = 2.0 cm, Bottom = 2.0 cm, Left = 3.5 cm, Right = 2.0 cm.
- [ ] Font chữ: 100% Times New Roman, cỡ 13 (tiêu đề chương cỡ 14).
- [ ] Dãn dòng: 1.5 lines, Before = 6pt, After = 6pt trên toàn bộ các đoạn văn.
- [ ] Thụt đầu dòng: Tab = 1.5 cm. Căn lề: Đều hai bên (Justified).
- [ ] Số trang: Khối đầu đánh số La Mã thường (`i`, `ii`...), Khối nội dung đánh số Ả Rập từ `1`, đặt ở giữa chân trang.
- [ ] Không có Header và Footer nội dung văn bản.
- [ ] Mục lục tự động (TOC): Mã trường `{ TOC \o "1-3" \h \z \u }`, tab leader dấu chấm 15.5 cm.
- [ ] Danh mục hình ảnh (TOF): Mã trường `{ TOC \h \z \c "Hình" }`, quét Caption nhãn `"Hình"`.
- [ ] Danh mục bảng biểu (TOT): Mã trường `{ TOC \h \z \c "Bảng" }`, quét Caption nhãn `"Bảng"`.
- [ ] Bookmarks OpenXML: 100% đề mục có cặp thẻ `bookmarkStart` / `bookmarkEnd` khớp ID và gắn Outline Level 0, 1, 2.
- [ ] Navigation Pane: Cây đề mục hiển thị phân tầng rõ ràng khi mở Word (`Ctrl + F`).
- [ ] Bảng biểu: Tiêu đề phía trên dạng `Bảng X.Y: ...` gắn Caption nhãn `"Bảng"`, thân bảng không in đậm.
- [ ] Hình ảnh: Tiêu đề phía dưới dạng `Hình X.Y: ...` gắn Caption nhãn `"Hình"`.
- [ ] Gạch đầu dòng: Câu dẫn dắt đầy đủ, thụt lề chuẩn `1.5 cm` hoặc `2.25 cm` (hanging `-0.35 cm`).
