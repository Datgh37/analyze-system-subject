# Kế Hoạch Triển Khai: PLAN-19 — Tái Cấu Trúc Tinh Gọn Chương 1, Chuẩn Hóa Tiêu Đề Không Ngoặc Đơn & Thanh Lọc Văn Phong Học Thuật

> **Tài liệu mục tiêu:** `files/KhoaLuanTotNghiep.docx`  
> **Thời điểm lập:** `2026-10-02 11:25:00`  
> **Tuân thủ quy tắc:**  
> - Bất khả xâm phạm các trang La Mã thường (Khối thủ tục đầu).  
> - Áp dụng triệt để Quy chuẩn văn phong học thuật DNTU (`.agents/rules/04_dntu_kltn_academic_writing_style_rules.md`).  
> - Loại bỏ 100% từ ngữ "điểm đau / pain points", loại bỏ ngoặc đơn tiếng Anh trong tiêu đề.  
> - Giữ vững font Times New Roman 13pt, dãn dòng 1.5 lines, Before/After 6pt, thụt lề 0.75 cm cho bullet.  

---

## 1. Bối Cảnh & Mục Tiêu Cải Tiến

Qua rà soát thực tế toàn bộ văn bản `KhoaLuanTotNghiep.docx`:
1. **Chương 1 đang bị phình to và lặp lại nghiêm trọng:** Có tới 7 đề mục cấp 2 (1.1 đến 1.7) và 30 đề mục cấp 3. Các nội dung về đối tượng người dùng bị phân tán ở 3 nơi (1.1, 1.4, 1.5); các khó khăn/khoảng trống lặp lại ở 4 nơi (1.3, 1.4, 1.6, 1.7); mục 1.5 đưa Tác nhân và Thực thể vào quá sớm (dẫm chân lên Chương 3).
2. **Tiêu đề còn chứa ngoặc đơn tiếng Anh không cần thiết:** Xuất hiện ở các mục như `(Problem Statement)`, `(Mathematical Audit Algorithm)`, `(Sequence Diagrams)`...
3. **Thuật ngữ dịch thô, tối nghĩa:** Xuất hiện cụm từ *"pain point"* (P[138]), dịch thành *"điểm đau"* mang tính tiếp thị/startup, không phù hợp với văn phong báo cáo khoa học của trường đại học.
4. **Văn phong còn vết tích rườm rà của AI:** Một số đoạn diễn đạt vòng vo, cố tình paraphrase kéo dài trang thay vì tập trung vào trọng tâm kỹ thuật.

**Mục tiêu:**
- Tái cấu trúc Chương 1 từ 7 mục cồng kềnh thành **4 mục mạch lạc, súc tích**, đánh trúng trọng tâm bài toán.
- Rà soát toàn bộ các tiêu đề trong toàn file, **bỏ toàn bộ các nội dung trong ngoặc đơn**.
- Xóa bỏ triệt để từ ngữ "pain point", "điểm đau", thay bằng các khái niệm học thuật chính xác: *"khó khăn, vướng mắc thực tế"*, *"bất cập hiện hành"*, *"rào cản tâm lý"*.
- Viết lại câu từ tự nhiên, mạch lạc, tôn trọng người đọc, bảo đảm tính học thuật khách quan của sinh viên ngành Hệ thống Thông tin.

---

## 2. Chi Tiết Phương Án Tái Cấu Trúc Chương 1

### 2.1. So Sánh Cấu Trúc Cũ vs Cấu Trúc Mới

| Cấu Trúc Cũ (7 Mục - 30 Tiểu Mục Lặp Ý) | Cấu Trúc Đề Xuất Mới (4 Mục - 9 Tiểu Mục Tinh Gọn) | Rationale & Hành Động Kỹ Thuật |
| :--- | :--- | :--- |
| **1.1 Phân tích đối tượng sử dụng và phân quyền**<br>- 1.1.1 Đối tượng sử dụng<br>- 1.1.2 Vai trò của người dùng<br>- 1.1.3 Phân quyền hệ thống | *(Gộp nội dung phân quyền vào 1.1; gộp đối tượng vào 1.3)* | Loại bỏ sự phân tán đối tượng người dùng. |
| **1.2 Quy trình nghiệp vụ hiện có**<br>- 1.2.1 Thu nhập<br>- 1.2.2 Chi tiêu<br>- 1.2.3 Phân loại<br>- 1.2.4 Lập ngân sách<br>- 1.2.5 Theo dõi ngân sách<br>- 1.2.6 Mục tiêu tài chính<br>- 1.2.7 Xử lý vượt ngân sách | **1.1 Khảo sát các quy trình quản lý tài chính hiện có**<br>- **1.1.1 Quy trình theo dõi dòng tiền:** Gom dòng tiền thu/chi, phân loại danh mục.<br>- **1.1.2 Quy trình lập và giám sát ngân sách:** Gom việc lập ngân sách, theo dõi và xử lý vượt trần. | Gom 7 quy trình vụn vặt thành 2 cụm quy trình tự nhiên của đời thực: (1) Quản lý dòng tiền hàng ngày; (2) Hoạch định ngân sách & mục tiêu. |
| **1.3 Đánh giá các nghiệp vụ hiện tại**<br>- 1.3.1 Khảo sát phương pháp<br>- 1.3.2 Khảo sát ứng dụng<br>- 1.3.3 So sánh hệ thống<br>- 1.3.4 Vấn đề và hạn chế<br>- 1.3.5 Nguyên nhân và tác động<br>- 1.3.6 Phân tích khoảng trống | **1.2 Đánh giá các phương pháp và ứng dụng trên thị trường**<br>- **1.2.1 Khảo sát các phương pháp truyền thống:** Sổ tay, Excel (Bảng 1.1).<br>- **1.2.2 Khảo sát và đối chiếu các ứng dụng phổ biến:** Money Lover, Spendee, Money Manager, YNAB (Bảng 1.2).<br>- **1.2.3 Những hạn chế cốt lõi và nguyên nhân tồn đọng:** Tinh lọc 3 nguyên nhân kỹ thuật (ma sát nhập liệu, ngân sách tĩnh, tách rời quản trị nợ). | Gộp các tiểu mục đánh giá, hạn chế, nguyên nhân vào một bức tranh so sánh thị trường sắc bén, trung thực; giữ lại 2 bảng so sánh giá trị. |
| **1.4 Báo cáo khảo sát và đa góc nhìn người dùng**<br>- 1.4.1 Nhóm người dùng<br>- 1.4.2 Sinh viên<br>- 1.4.3 Người mới đi làm<br>- 1.4.4 Người có mục tiêu<br>- 1.4.5 Tổng hợp pain point<br>- 1.4.6 Đánh giá ưu tiên | **1.3 Nghiên cứu người dùng mục tiêu và các khó khăn thực tế**<br>- **1.3.1 Đặc điểm các nhóm người dùng tiêu biểu:** Trình bày 3 chân dung sắc nét (Sinh viên, Người mới đi làm, Người tích lũy & trả nợ).<br>- **1.3.2 Tổng hợp khó khăn và nhu cầu cấp thiết của người dùng:** Bảng 1.3 (sửa caption: xóa "pain point", dùng "khó khăn, vướng mắc"). | Xóa bỏ hoàn toàn chữ "pain point / điểm đau". Tích hợp toàn diện chân dung người dùng vào đúng một mục duy nhất. |
| **1.5 Các tác nhân và mô hình thực thể nghiệp vụ**<br>- 1.5.1 Xác định tác nhân<br>- 1.5.2 Vai trò & luồng tương tác<br>- 1.5.3 Xác định thực thể<br>- 1.5.4 Mối quan hệ thực thể | *(Chuyển toàn bộ sang Chương 3)* | **Loại bỏ khỏi Chương 1.** Chuyển sang đúng vị trí của nó: Tác nhân $\rightarrow$ Mục 3.1; Thực thể $\rightarrow$ Mục 3.3. Khắc phục lỗi "nói trước" phương pháp luận. |
| **1.6 Hiện trạng tin học**<br>- 1.6.1 Phần cứng, 1.6.2 Phần mềm<br>- 1.6.3 Con người, 1.6.4 Dữ liệu<br>- 1.6.5 Hạn chế tin học | *(Tinh giản và tích hợp vào 1.2.2)* | Loại bỏ cấu trúc rập khuôn kiểu văn phòng doanh nghiệp máy chủ/PC; tích hợp 1 đoạn văn xuôi ngắn gọn về môi trường di động của người dùng vào mục 1.2.2. |
| **1.7 Kết luận khảo sát và phát biểu bài toán**<br>- 1.7.1 Kết luận & Khoảng trống<br>- 1.7.2 Phát biểu bài toán (Problem Statement) | **1.4 Kết luận khảo sát hiện trạng và phát biểu bài toán**<br>- **1.4.1 Nhận diện các khoảng trống nghiên cứu:** 5 khoảng trống cô đọng.<br>- **1.4.2 Phát biểu bài toán của hệ thống:** Bỏ ngoặc đơn `(Problem Statement)`, trình bày I-P-O mạch lạc, súc tích. | Tinh giản, loại bỏ lặp từ, tập trung vào tuyên bố bài toán kỹ thuật I-P-O chính xác. |

---

## 3. Rà Soát & Xóa Bỏ Ngoặc Đơn Ở Tất Cả Các Tiêu Đề

Thực hiện rà soát tự động và thủ công trên toàn bộ tài liệu:

| Vị Trí Cũ | Tiêu Đề Hiện Tại (Có Ngoặc Đơn) | Tiêu Đề Đã Sửa Đổi (Chuẩn Học Thuật) |
| :---: | :--- | :--- |
| **P[180]** | `1.7.2 Phát biểu bài toán chính thức của hệ thống (Problem Statement)` | `1.4.2 Phát biểu bài toán của hệ thống` |
| **P[227]** | `2.2.3 Thuật toán Hậu kiểm toán số học hóa đơn (Mathematical Audit Algorithm)` | `2.2.3 Thuật toán hậu kiểm toán số học hóa đơn` |
| **P[344]** | `3.4 Thiết kế tương tác và sơ đồ tuần tự hệ thống (Sequence Diagrams)` | `3.4 Thiết kế tương tác và sơ đồ tuần tự hệ thống` |
| **Toàn văn** | Kiểm tra bất kỳ tiêu đề Heading 1, 2, 3 nào khác có chứa `(...)` | Loại bỏ phần chú giải tiếng Anh trong ngoặc, chỉ giữ tiếng Việt chuẩn. |

---

## 4. Chuẩn Hóa Văn Phong & Từ Ngữ Toàn Văn (Prose Purification)

1. **Khử Bỏ Từ Ngữ Tiếp Thị / Dịch Thô:**
   - Thay thế triệt để cụm từ *"pain point"*, *"điểm đau"* thành: *"khó khăn, vướng mắc thực tế"*, *"bất cập hiện hành"*, *"rào cản tâm lý của người dùng"*.
   - Đổi tên **Bảng 1.3**: Từ *"Bảng 1.3. Tổng hợp các vấn đề của người dùng"* hoặc *"pain point"* thành: **`Bảng 1.3. Tổng hợp khó khăn và nhu cầu của người dùng`**.
2. **Khử Bỏ Lối Hành Văn Phóng Đại (Anti-AI-Slop):**
   - Rà soát các từ ngữ hoa mỹ rỗng tuếch: *"đột phá toàn diện"*, *"linh hồn trí tuệ nhân tạo"*, *"bức tranh đa chiều toàn cảnh"*, *"vũ khí tối thượng"*, *"trụ cột then chốt không thể tách rời"*...
   - Thay bằng câu văn khiêm tốn, khách quan, chính xác: nêu rõ cấu trúc dữ liệu, giải thuật, bảng biểu đối chiếu và tham số định lượng.
3. **Cắt Gọt Paraphrase Kéo Dài Trang:**
   - Biên tập lại các đoạn văn bị lặp ý ở Chương 1 thành các đoạn văn xuôi gãy gọn, có câu chủ đề rõ ràng, dẫn chứng kỹ thuật cụ thể và câu kết đoạn dứt khoát.

---

## 5. Quy Trình Thực Thi 5 Bước An Toàn

```text
┌────────────────────────────────────────────────────────────────────────┐
│ BƯỚC 1: TẠO BẢN SAO LƯU AN TOÀN (BACKUP)                               │
│ - Sao lưu files/KhoaLuanTotNghiep.docx -> KhoaLuanTotNghiep.bak_p19.docx│
├────────────────────────────────────────────────────────────────────────┤
│ BƯỚC 2: XÂY DỰNG SCRIPT PYTHON BIÊN TẬP NỘI DUNG CHƯƠNG 1 (DOCX)       │
│ - Viết lại các đoạn văn Chương 1 theo 4 đề mục mới tinh gọn.           │
│ - Gắn mã Bookmark OpenXML chuẩn (_Toc_1_1, _Toc_1_2, _Toc_1_3, _Toc_1_4)│
│ - Bảo toàn 3 bảng dữ liệu Bảng 1.1, 1.2, 1.3 (chuẩn Academic Navy 13pt) │
├────────────────────────────────────────────────────────────────────────┤
│ BƯỚC 3: RÀ SOÁT VÀ XÓA NGOẶC ĐƠN TIÊU ĐỀ TOÀN VĂN                      │
│ - Quét sạch ngoặc đơn ở Mục 1.4.2, Mục 2.2.3, Mục 3.4.                 │
├────────────────────────────────────────────────────────────────────────┤
│ BƯỚC 4: RÀ SOÁT TỪ NGỮ VÀ VĂN PHONG HỌC THUẬT                         │
│ - Quét sạch cụm từ "pain point", "điểm đau" và các từ ngữ AI sáo rỗng. │
├────────────────────────────────────────────────────────────────────────┤
│ BƯỚC 5: KIỂM TOÁN TÍNH TOÀN VẸN & CHẠY LẠI BỘ TEST SUITE               │
│ - Kiểm tra số lượng đoạn văn, bảo vệ khối La Mã thường.                │
│ - Chạy `python -B -m unittest discover tests` đảm bảo 100% tests PASS. │
│ - Ghi nhận Quyết Định 74 vào decisions_and_changes_log.md.             │
└────────────────────────────────────────────────────────────────────────┘
```

---

## 6. Tiêu Chí Nghiệm Thu (Acceptance Criteria)

1. [ ] **Chương 1 cô đọng:** Chỉ còn 4 mục cấp 2 (`1.1` đến `1.4`), không còn tình trạng trùng lặp chân dung người dùng hay lặp lại danh sách hạn chế/khoảng trống.
2. [ ] **100% tiêu đề sạch:** Không còn ngoặc đơn `(...)` ở bất kỳ đề mục Heading 1, 2, 3 nào trong toàn bộ tài liệu.
3. [ ] **Sạch bóng "pain points / điểm đau":** Không còn từ "pain point" hoặc "điểm đau" trong toàn bộ văn bản và bảng biểu.
4. [ ] **Văn phong tự nhiên:** Các câu văn trôi chảy, đúng thuật ngữ chuyên môn Hệ thống Thông tin, súc tích, không paraphrase kéo trang.
5. [ ] **Khối La Mã thường an toàn:** Toàn bộ trang bìa, nhiệm vụ, lời cảm ơn, mục lục giữ nguyên vẹn 100%.
6. [ ] **Kiểm thử tự động:** Toàn bộ 106/106 unit tests vượt qua thành công.
