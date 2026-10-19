# 📘 QUY CHUẨN ĐỊNH DẠNG & THÔNG SỐ KỸ THUẬT VĂN BẢN KHÓA LUẬN TỐT NGHIỆP (DNTU)
## MASTER FORMAT SPECIFICATION & SOURCE OF TRUTH FOR `KhoaLuanTotNghiep.docx`

> **Tài liệu Nguồn Sự Thật (Canonical Specification):**  
> Ghi nhận toàn bộ thông số định dạng chuẩn mực từ tệp gốc [`files/KhoaLuanTotNghiep.bak_pre_plan24.docx`] và Bộ quy định đào tạo Đại học Công nghệ Đồng Nai ([`.agents/rules/`]).  
> **Mọi tác tử AI, quy trình tự động và người soạn thảo BẮT BUỘC phải đối chiếu tài liệu này trước khi chỉnh sửa tệp Word, tuyệt đối không được tự ý thay đổi.**

---

## 1. THÔNG SỐ THIẾT LẬP TRANG (PAGE SETUP & MARGINS)

Toàn bộ tài liệu tuân thủ chuẩn in ấn và đóng tập bìa cứng mạ vàng:

| Thông số | Giá trị chuẩn (cm) | Giá trị quy đổi Word (dxa / pt) | Ý nghĩa nghiệp vụ |
| :--- | :---: | :---: | :--- |
| **Khổ giấy (Paper Size)** | **A4** (21.0 cm x 29.7 cm) | `11906 dxa` x `16838 dxa` | Chuẩn văn bản hành chính Việt Nam |
| **Lề trên (Top Margin)** | **2.0 cm** | `1134 dxa` (56.7 pt) | Khoảng cách đỉnh trang |
| **Lề dưới (Bottom Margin)** | **2.0 cm** | `1134 dxa` (56.7 pt) | Khoảng cách đáy trang |
| **Lề trái (Left Margin)** | **3.5 cm** | `1984 dxa` (99.2 pt) | **Bắt buộc** để chừa gáy đóng bìa cứng |
| **Lề phải (Right Margin)** | **2.0 cm** | `1134 dxa` (56.7 pt) | Khoảng cách lề phải |
| **Hướng trang (Orientation)** | **Portrait** (Đứng) | `<w:pgSz w:orient="portrait"/>` | Toàn bộ các trang |
| **Header & Footer** | **Không có Header/Footer chữ** | - | Chỉ hiển thị duy nhất số trang ở Footer |

---

## 2. QUY CHUẨN ĐÁNH SỐ TRANG & SECTION BREAKS

Tài liệu được chia thành hai phân vùng độc lập (2 Section Breaks) với cơ chế đánh số trang khác biệt:

```text
┌────────────────────────────────────────────────────────────────────────┐
│ SECTION 1: KHỐI THỦ TỤC ĐẦU (MỤC 01 ĐẾN MỤC 10 - BM201 ĐẾN BM212)       │
│ - Trang bìa ngoài, Bìa lót, Nhiệm vụ KLTN, Cam đoan, Cảm ơn, Nhận xét  │
│ - Mục lục (BM209), Danh mục Hình (BM210), Danh mục Bảng (BM211), Từ viết tắt│
│ - Kiểu đánh số trang: Số La Mã thường (i, ii, iii, iv, v, vi,...)      │
│ - Vị trí: Chính giữa lề dưới trang giấy (Bottom Center)                │
│ - QUY TẮC BẢO VỆ: Người dùng trực tiếp quản lý thủ công. AI KHÔNG CAN  │
│   THIỆP, KHÔNG TỰ ĐỔI LAYOUT NẾU KHÔNG CÓ LỆNH RÕ RÀNG.                │
├────────────────────────────────────────────────────────────────────────┤
│ SECTION 2: KHỐI NỘI DUNG CHÍNH (TỪ CHƯƠNG 1 ĐẾN HẾT KHÓA LUẬN)        │
│ - Bắt đầu từ: "CHƯƠNG 1: KHẢO SÁT HIỆN TRẠNG HỆ THỐNG"                 │
│ - Kiểu đánh số trang: Số Ả Rập bắt đầu lại từ 1 (1, 2, 3, 4,...)        │
│ - Vị trí: Chính giữa lề dưới trang giấy (Bottom Center)                │
└────────────────────────────────────────────────────────────────────────┘
```

---

## 3. PHÔNG CHỮ & ĐỊNH DẠNG ĐOẠN VĂN (TYPOGRAPHY & PARAGRAPHS)

| Thuộc tính đoạn văn | Quy chuẩn bắt buộc | Mã thuộc tính Word OpenXML |
| :--- | :--- | :--- |
| **Phông chữ (Font Family)** | **Times New Roman** 100% | `<w:rFonts w:ascii="Times New Roman" w:hAnsi="Times New Roman" w:cs="Times New Roman"/>` |
| **Màu chữ (Font Color)** | **Chữ đen `#000000`** (hoặc `auto`) | `<w:color w:val="000000"/>` (CẤM tuyệt đối màu xanh) |
| **Cỡ chữ thân bài (Body)** | **13 pt** | `<w:sz w:val="26"/>` và `<w:szCs w:val="26"/>` |
| **Căn lề đoạn văn (Alignment)** | **Căn đều hai bên (Justified / Both)** | `<w:jc w:val="both"/>` |
| **Giãn dòng (Line Spacing)** | **1.5 lines** | `<w:spacing w:line="360" w:lineRule="auto"/>` |
| **Giãn đoạn (Spacing Before)** | **6 pt** | `<w:spacing w:before="120"/>` (120 dxa = 6 pt) |
| **Giãn đoạn (Spacing After)** | **6 pt** | `<w:spacing w:after="120"/>` (120 dxa = 6 pt) |
| **Thụt đầu dòng (First Line)** | **1.5 cm** | `<w:ind w:firstLine="850"/>` (850 dxa ≈ 1.5 cm) |

### Quy cách sử dụng Bullet & Gạch đầu dòng:
- Ký hiệu thống nhất duy nhất: `"- "` (dấu gạch ngang kèm khoảng trắng). Không dùng `•`, `–`, `—`.
- Luôn có câu dẫn dắt kết thúc bằng dấu hai chấm (`:`).
- Thụt lề gạch đầu dòng: Dòng 1 thụt `1.5 cm`, dòng 2+ (khi rớt dòng) thụt `0.75 cm` (`Left Indent = 0.75 cm`, `First Line Indent = 0.75 cm`).

---

## 4. HỆ THỐNG ĐỀ MỤC & PHÂN CẤP TIÊU ĐỀ (HEADINGS)

Tuyệt đối không dùng số La Mã trong đề mục (trừ tên phân hệ Use Case nếu có). Không có dấu chấm ở cuối đề mục.

| Cấp đề mục | Mẫu quy chuẩn | Cỡ chữ | Định dạng | Căn lề | Giãn dòng / Giãn đoạn |
| :--- | :--- | :---: | :---: | :---: | :--- |
| **Tiêu đề Chương** | `CHƯƠNG 1: KHẢO SÁT HIỆN TRẠNG HỆ THỐNG` | **14 pt** | **In đậm, VIẾT HOA** | Căn giữa | Before: 18 pt (`360 dxa`), After: 12 pt (`240 dxa`), Line: 1.5 lines |
| **Đề mục Cấp 1** | `1.1 Khảo sát các quy trình quản lý tài chính hiện có` | **13 pt** | **In đậm, Viết hoa đầu từ** | Căn trái | Before: 12 pt (`240 dxa`), After: 6 pt (`120 dxa`), Line: 1.5 lines |
| **Đề mục Cấp 2** | `1.1.1 Quy trình theo dõi dòng tiền` | **13 pt** | **In đậm, Viết hoa đầu từ** | Căn trái | Before: 6 pt (`120 dxa`), After: 6 pt (`120 dxa`), Line: 1.5 lines |
| **Đề mục Cấp 3** | `1.1.1.1 [Nội dung con]` hoặc Persona | **13 pt** | **In đậm, in nghiêng** | Căn trái | Before: 6 pt (`120 dxa`), After: 4 pt (`80 dxa`), Line: 1.5 lines |

---

## 5. THIẾT KẾ BẢNG BIỂU HỌC THUẬT (ACADEMIC TABLES)

> [!CAUTION]
> **LOẠI BỎ TRIỆT ĐỂ MÀU XANH DƯƠNG:**
> Không sử dụng màu xanh `#2B4C7E`, `#1F4E78` trong bảng biểu Word. Toàn bộ bảng sử dụng nền trung tính thanh lịch (Xám nhạt / Trắng), phông chữ Times New Roman, chữ đen `#000000`.

| Thành phần bảng | Quy định màu sắc & phông chữ | Căn lề & Đệm ô | Ghi chú kỹ thuật |
| :--- | :--- | :--- | :--- |
| **Căn lề toàn bảng** | Toàn bộ bảng căn giữa trang giấy | `w:jc w:val="center"` | Bảng không tràn lề 3.5cm/2.0cm |
| **Dòng Tiêu đề (Header)** | Nền xám nhạt `#F2F2F2`, Chữ **In đậm (`Bold = True`)**, màu đen `#000000`, cỡ 11-13 pt | Căn giữa ô (`Center`), Đệm: Top/Bottom 5 pt, Left/Right 6 pt | Lặp lại tiêu đề ở mỗi trang nếu ngắt bảng |
| **Thân bảng (Data rows)** | Nền trắng `#FFFFFF`, Chữ **Không in đậm (`Bold = False`)**, màu đen `#000000`, cỡ 11-13 pt | Căn trái (nội dung text) hoặc Căn giữa (STT, mã số, phân loại) | Tuyệt đối không bôi đậm cột STT hay tên thuộc tính |
| **Đường viền bảng (Borders)** | Đường kẻ đơn mỏng (`single`), màu xám nhạt `#D3D3D3` hoặc `#CBD5E1`, độ dày 0.5 pt (`sz="4"`) | Áp dụng biên ngoài và phân cách trong | Không dùng viền đen quá dày |

---

## 6. QUY CHUẨN ĐẶT TÊN, ĐÁNH SỐ & CẤU TRÚC CAPTION WORD TỰ ĐỘNG

### 6.1. Nguyên tắc đánh số theo Đề mục (Chương):
- **Bắt buộc đánh số theo số thứ tự của Chương:**
  - Bảng biểu: `Bảng [Số Chương].[Số thứ tự trong chương]: [Tên bảng]` (Ví dụ: `Bảng 1.1`, `Bảng 1.2`, `Bảng 3.1`, `Bảng 3.2`,..., `Bảng 3.12`).
  - Hình ảnh: `Hình [Số Chương].[Số thứ tự trong chương]: [Tên hình]` (Ví dụ: `Hình 3.1`, `Hình 3.2`,..., `Hình 3.17`).
- **TUYỆT ĐỐI KHÔNG ĐƯỢC:**
  - Đánh số tuần tự toàn tài liệu làm mất gốc chương (như `Bảng 4`, `Bảng 5`, `Hình 2`, `Hình 11`...).
  - Đổi dấu hai chấm `:` hoặc dấu chấm `.` tùy tiện làm lệch chuỗi khớp của người dùng.

### 6.2. Vị trí & Định dạng Caption:
- **Tiêu đề Bảng:** Đặt **ngay phía trên bảng**, căn giữa trang giấy.
- **Tiêu đề Hình:** Đặt **ngay phía dưới hình**, căn giữa trang giấy.
- **Phông chữ & Cỡ chữ:** Font **Times New Roman**, cỡ **11 pt - 12 pt**, **In nghiêng (`Italic = True`)**, màu đen `#000000`.

### 6.3. Cấu trúc Mã trường OpenXML cho Tính năng "Insert Table of Figures" trong Word:
Để người dùng có thể sử dụng trực tiếp tính năng `References` $\rightarrow$ `Insert Table of Figures` trong Microsoft Word mà không gặp lỗi *"No table of figures entries found"*:
1. Đoạn văn chứa Caption **bắt buộc mang Paragraph Style là `Caption`** (`<w:pStyle w:val="Caption"/>`).
2. Tên nhãn (Label Identifier) trong trường mã phải khớp chính xác:
   - Dành cho Bảng biểu: Sử dụng nhãn `"Bảng"` với trường `<w:fldSimple w:instr="SEQ Bảng \r [Số]"/>` (hoặc `SEQ Bảng`).
   - Dành cho Hình ảnh: Sử dụng nhãn `"Hình"` với trường `<w:fldSimple w:instr="SEQ Hình \r [Số]"/>` (hoặc `SEQ Hình`).
3. Sử dụng công tắc `\r 1` tại mục đầu tiên của mỗi chương để đồng bộ thứ tự số học theo chương mà không làm vỡ cơ chế quét chuỗi của Microsoft Word.

```xml
<!-- Cấu trúc chuẩn XML Caption Bảng biểu: Bảng 3.1: Tên bảng -->
<w:p>
  <w:pPr>
    <w:pStyle w:val="Caption"/>
    <w:jc w:val="center"/>
    <w:spacing w:before="120" w:after="120" w:line="360" w:lineRule="auto"/>
    <w:rPr>
      <w:rFonts w:ascii="Times New Roman" w:hAnsi="Times New Roman" w:cs="Times New Roman"/>
      <w:i/>
      <w:color w:val="000000"/>
      <w:sz w:val="24"/>
      <w:szCs w:val="24"/>
    </w:rPr>
  </w:pPr>
  <w:r>
    <w:rPr>
      <w:rFonts w:ascii="Times New Roman" w:hAnsi="Times New Roman" w:cs="Times New Roman"/>
      <w:i/>
      <w:color w:val="000000"/>
      <w:sz w:val="24"/>
    </w:rPr>
    <w:t xml:space="preserve">Bảng 3.</w:t>
  </w:r>
  <w:fldSimple w:instr="SEQ Bảng \r 1">
    <w:r>
      <w:rPr>
        <w:rFonts w:ascii="Times New Roman" w:hAnsi="Times New Roman" w:cs="Times New Roman"/>
        <w:i/>
        <w:color w:val="000000"/>
        <w:sz w:val="24"/>
      </w:rPr>
      <w:t>1</w:t>
    </w:r>
  </w:fldSimple>
  <w:r>
    <w:rPr>
      <w:rFonts w:ascii="Times New Roman" w:hAnsi="Times New Roman" w:cs="Times New Roman"/>
      <w:i/>
      <w:color w:val="000000"/>
      <w:sz w:val="24"/>
    </w:rPr>
    <w:t xml:space="preserve">: Danh mục các tác nhân và vai trò trong hệ thống PFM</w:t>
  </w:r>
</w:p>
```

---

## 7. DANH MỤC TRUY VẾT BẢNG BIỂU & HÌNH ẢNH TOÀN KHÓA LUẬN

| Ký hiệu | Loại đối tượng | Tên chính thức chuẩn mực trong tài liệu | Vị trí đề mục |
| :--- | :---: | :--- | :--- |
| **Bảng 1.1** | Table | So sánh các phương pháp quản lý chi tiêu | Mục 1.2.1 |
| **Bảng 1.2** | Table | So sánh một số ứng dụng quản lý tài chính cá nhân | Mục 1.2.2 |
| **Bảng 1.3** | Table | Tổng hợp khó khăn và nhu cầu của người dùng | Mục 1.3.2 |
| **Bảng 3.1** | Table | Danh mục các tác nhân và vai trò trong hệ thống PFM | Mục 3.1.1 |
| **Hình 3.1** | Figure | Sơ đồ Use Case tổng thể hệ thống (UCD 00 - Tổng Quát) | Mục 3.1.3 |
| **Hình 3.2** | Figure | Sơ đồ Use Case Phân hệ Quản lý Tài khoản & Đa Ví cá nhân (UCD 01) | Mục 3.1.4 |
| **Hình 3.3** | Figure | Sơ đồ Use Case Phân hệ Quản lý Thu nhập cá nhân (UCD 02) | Mục 3.1.4 |
| **Hình 3.4** | Figure | Sơ đồ Use Case Phân hệ Chi tiêu & Bóc tách Hóa đơn AI OCR (UCD 03) | Mục 3.1.4 |
| **Hình 3.5** | Figure | Sơ đồ Use Case Phân hệ Quản lý Danh mục Thu Chi & Quy tắc thông minh (UCD 04) | Mục 3.1.4 |
| **Hình 3.6** | Figure | Sơ đồ Use Case Phân hệ Quản lý Ngân sách & Hạn mức chi tiêu DSA (UCD 05) | Mục 3.1.4 |
| **Hình 3.7** | Figure | Sơ đồ Use Case Phân hệ Quỹ Tích lũy & Tối ưu hóa Nợ cá nhân (UCD 06) | Mục 3.1.4 |
| **Hình 3.8** | Figure | Sơ đồ Use Case Phân hệ Báo cáo & Phân tích Tài chính Đa chiều (UCD 07) | Mục 3.1.4 |
| **Hình 3.9** | Figure | Sơ đồ Use Case Phân hệ Dự báo Tài chính & Rủi ro Thâm hụt (UCD 08) | Mục 3.1.4 |
| **Hình 3.10** | Figure | Sơ đồ Use Case Phân hệ Cú hích Hành vi & Hỗ trợ Quyết định Thaler (UCD 09) | Mục 3.1.4 |
| **Bảng 3.2** | Table | Bảng tổng hợp phân loại 63 Use Cases theo 9 phân hệ chức năng | Mục 3.2.1 |
| **Bảng 3.3** | Table | Đặc tả UC-01: Quản lý tài khoản và Đa Ví cá nhân | Mục 3.2.2 |
| **Bảng 3.4** | Table | Đặc tả UC-02: Quản lý thu nhập cá nhân | Mục 3.2.2 |
| **Bảng 3.5** | Table | Đặc tả UC-03: Quản lý chi tiêu và Bóc tách hóa đơn thông minh | Mục 3.2.2 |
| **Bảng 3.6** | Table | Đặc tả UC-04: Quản lý danh mục thu chi và Quy tắc thông minh | Mục 3.2.2 |
| **Bảng 3.7** | Table | Đặc tả UC-05: Quản lý ngân sách đa phương pháp và Hạn mức chi tiêu khả dụng thích ứng | Mục 3.2.2 |
| **Bảng 3.8** | Table | Đặc tả UC-06: Quỹ tích lũy và Tối ưu hóa nghĩa vụ nợ cá nhân | Mục 3.2.2 |
| **Bảng 3.9** | Table | Đặc tả UC-07: Báo cáo và Phân tích tài chính đa chiều | Mục 3.2.2 |
| **Bảng 3.10** | Table | Đặc tả UC-08: Dự báo tài chính và Cảnh báo rủi ro thâm hụt | Mục 3.2.2 |
| **Bảng 3.11** | Table | Đặc tả UC-09: Cú hích hành vi và Hỗ trợ quyết định tài chính | Mục 3.2.2 |
| **Bảng 3.12** | Table | Bảng đối chiếu tiêu chí sàng lọc các lớp nghiệp vụ | Mục 3.3.1 |
| **Hình 3.11** | Figure | Sơ đồ kiến trúc tổng thể 6 Bounded Contexts miền PFM | Mục 3.3.2 |
| **Hình 3.12** | Figure | Context 1: Định danh & Hồ sơ cá nhân (Identity & Profile Context) | Mục 3.3.3 |
| **Hình 3.13** | Figure | Context 2: Tài khoản & Đa ví cá nhân chuẩn ACID (Account & Multi-Wallet Context) | Mục 3.3.3 |
| **Hình 3.14** | Figure | Context 3: Giao dịch, AI Vision OCR & Danh mục (Transactions, AI OCR & Categories Context) | Mục 3.3.3 |
| **Hình 3.15** | Figure | Context 4: Ngân sách đa phương pháp & DSA (Budgeting & Adaptive DSA Context) | Mục 3.3.3 |
| **Hình 3.16** | Figure | Context 5: Quỹ tích lũy & Tối ưu hóa Nợ cá nhân (Personal Debt & Financial Goals Context) | Mục 3.3.3 |
| **Hình 3.17** | Figure | Context 6: Cú hích hành vi & Hỗ trợ quyết định Thaler (Decision Support & Behavioral Nudge Context) | Mục 3.3.3 |

---

## 8. ĐIỀU KHOẢN BẤT DI BẤT DỊCH (IMMUTABLE CONSTRAINTS)

1. **Tuyệt đối không sử dụng màu xanh dương** cho bất kỳ thành phần văn bản, tiêu đề hay bảng biểu nào trong Word.
2. **Tuyệt đối không sửa đổi Khối thủ tục đầu (Preamble - Mục 01 đến 10)**. Để nguyên mẫu bảng biểu biểu mẫu DNTU do người dùng tự tay điền và quản lý.
3. **Mọi cập nhật nội dung chỉ thực hiện từ Mục 11 (Chương 1)** trở đi.
4. **Mọi Caption phải duy trì số hiệu theo Chương** (`Bảng X.Y` và `Hình X.Y`).
