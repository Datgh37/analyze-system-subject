# KẾ HOẠCH ĐIỀU HÀNH PLAN-23: CHUẨN HÓA CAPTION WORD TỰ ĐỘNG (SEQ BẢNG & SEQ HÌNH), TÍCH HỢP BOOKMARK CROSS-REFERENCE VÀ KÍCH HOẠT DANH MỤC TOF / TOT TRONG KHOÁ LUẬN TỐT NGHIỆP

> **Dự án:** Hệ thống Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh (Tích hợp AI)  
> **Thời gian lập kế hoạch:** 2026-10-08  
> **Tài liệu mục tiêu:** [`files/KhoaLuanTotNghiep.docx`](../../files/KhoaLuanTotNghiep.docx)  
> **Quy chuẩn đối chiếu:** [Rule 02: Cấu trúc & Mục lục KLTN DNTU](../../.agents/rules/02_dntu_kltn_structure_and_toc_rules.md), [Rule 03: Thiết kế Bảng biểu & Hình ảnh](../../.agents/rules/03_dntu_kltn_tables_and_figures_rules.md)

---

## 1. Bối Cảnh & Mục Tiêu

### 1.1. Hiện Trạng
- Toàn bộ 15 bảng biểu và 17 hình ảnh trong Khóa luận tốt nghiệp (`KhoaLuanTotNghiep.docx`) hiện đang được trình bày tiêu đề dưới dạng văn bản tĩnh thông thường (plain text paragraphs).
- Chưa được gắn trường đánh số tự động **Insert Caption** của Microsoft Word (trường mã `SEQ Bảng` và `SEQ Hình`).
- Chưa có hệ thống Bookmark OpenXML phục vụ tính năng Tham chiếu chéo (**Cross-reference**).
- Hai trang danh mục chuyên biệt tại Khối thủ tục đầu (Preamble):
  - **Trang 8: DANH MỤC SƠ ĐỒ, HÌNH ẢNH (`BM210-KT21`)** hiện đang chứa dòng văn bản giữ chỗ (placeholder).
  - **Trang 9: DANH MỤC BẢNG BIỂU (`BM211-KT21`)** hiện đang chứa dòng văn bản giữ chỗ (placeholder).
- Khi người dùng sử dụng tính năng tạo Danh mục tự động trong Microsoft Word (`References -> Insert Table of Figures`), Word báo lỗi không tìm thấy mục nào (*No table of figures entries found*).

### 1.2. Mục Tiêu Cần Đạt
1. **Gắn Caption Word chuẩn ECMA-376 OpenXML** cho toàn bộ 15 Bảng biểu và 17 Hình ảnh/Sơ đồ, sử dụng Style `Caption` và trường phức hợp Native `SEQ Bảng`, `SEQ Hình`.
2. **Cung cấp Bookmark OpenXML duy nhất** cho từng Caption (`_Ref_Bang_X_Y` và `_Ref_Hinh_X_Y`) để hỗ trợ trọn vẹn tính năng `Insert -> Cross-reference` trong Word.
3. **Kích hoạt Mã trường Danh mục Động (Dynamic Field Code) của Microsoft Word**:
   - Trang Danh mục Hình ảnh: Mã trường `{ TOC \h \z \c "Hình" }`.
   - Trang Danh mục Bảng biểu: Mã trường `{ TOC \h \z \c "Bảng" }`.
   - Kết hợp danh sách hiển thị trực quan ban đầu có Tab Stop dấu chấm căn phải `15.5 cm` theo kiến trúc **Mục lục hai lớp (Dual-Layer TOC Architecture)** quy định tại Rule 02.
4. **Bảo tồn tuyệt đối cấu trúc tài liệu**: Không làm sai lệch 5 chương nội dung, giữ nguyên format bảng Academic Navy Theme, không chạm vào các trang thủ tục đầu do người dùng quản lý.
5. **Đảm bảo 100% tests PASSED** trên toàn bộ test suite.

---

## 2. Danh Mục Các Đối Tượng Cần Xử Lý

### 2.1. 15 Bảng Biểu (Tables) Cần Gắn Caption `SEQ Bảng`
1. `Bảng 1.1: So sánh các phương pháp quản lý chi tiêu` (Chương 1 - Reset đếm về 1)
2. `Bảng 1.2: So sánh một số ứng dụng quản lý tài chính cá nhân` (Chương 1)
3. `Bảng 1.3: Tổng hợp khó khăn và nhu cầu của người dùng` (Chương 1)
4. `Bảng 3.1: Danh mục các tác nhân và vai trò trong hệ thống PFM` (Chương 3 - Reset đếm về 1)
5. `Bảng 3.2: Bảng tổng hợp phân loại 60 Use Cases theo 9 phân hệ chức năng` (Chương 3)
6. `Bảng 3.3: Đặc tả UC-01: Quản lý tài khoản và Đa Ví cá nhân` (Chương 3)
7. `Bảng 3.4: Đặc tả UC-02: Quản lý thu nhập cá nhân` (Chương 3)
8. `Bảng 3.5: Đặc tả UC-03: Quản lý chi tiêu và Bóc tách hóa đơn thông minh` (Chương 3)
9. `Bảng 3.6: Đặc tả UC-04: Quản lý danh mục thu chi và Quy tắc thông minh` (Chương 3)
10. `Bảng 3.7: Đặc tả UC-05: Quản lý ngân sách đa phương pháp và Hạn mức chi tiêu khả dụng thích ứng` (Chương 3)
11. `Bảng 3.8: Đặc tả UC-06: Quỹ tích lũy và Tối ưu hóa nghĩa vụ nợ cá nhân` (Chương 3)
12. `Bảng 3.9: Đặc tả UC-07: Báo cáo và Phân tích tài chính đa chiều` (Chương 3)
13. `Bảng 3.10: Đặc tả UC-08: Dự báo tài chính và Cảnh báo rủi ro thâm hụt` (Chương 3)
14. `Bảng 3.11: Đặc tả UC-09: Cú hích hành vi và Hỗ trợ quyết định tài chính` (Chương 3)
15. `Bảng 3.12: Bảng đối chiếu tiêu chí sàng lọc các lớp nghiệp vụ` (Chương 3)

### 2.2. 17 Hình Ảnh & Sơ Đồ (Figures) Cần Gắn Caption `SEQ Hình`
1. `Hình 3.1: Sơ đồ Use Case tổng thể hệ thống (UCD 00 - Tổng Quát)` (Reset đếm về 1)
2. `Hình 3.2: Sơ đồ Use Case Phân hệ Quản lý Tài khoản & Đa Ví cá nhân (UCD 01)`
3. `Hình 3.3: Sơ đồ Use Case Phân hệ Quản lý Thu nhập cá nhân (UCD 02)`
4. `Hình 3.4: Sơ đồ Use Case Phân hệ Chi tiêu & Bóc tách Hóa đơn AI OCR (UCD 03)`
5. `Hình 3.5: Sơ đồ Use Case Phân hệ Quản lý Danh mục Thu Chi & Quy tắc thông minh (UCD 04)`
6. `Hình 3.6: Sơ đồ Use Case Phân hệ Quản lý Ngân sách & Hạn mức chi tiêu DSA (UCD 05)`
7. `Hình 3.7: Sơ đồ Use Case Phân hệ Quỹ Tích lũy & Tối ưu hóa Nợ cá nhân (UCD 06)`
8. `Hình 3.8: Sơ đồ Use Case Phân hệ Báo cáo & Phân tích Tài chính Đa chiều (UCD 07)`
9. `Hình 3.9: Sơ đồ Use Case Phân hệ Dự báo Tài chính & Rủi ro Thâm hụt (UCD 08)`
10. `Hình 3.10: Sơ đồ Use Case Phân hệ Cú hích Hành vi & Hỗ trợ Quyết định Thaler (UCD 09)`
11. `Hình 3.11: Sơ đồ kiến trúc tổng thể 6 Bounded Contexts miền PFM`
12. `Hình 3.12: Context 1: Định danh & Hồ sơ cá nhân (Identity & Profile Context)`
13. `Hình 3.13: Context 2: Tài khoản & Đa ví cá nhân chuẩn ACID (Account & Multi-Wallet Context)`
14. `Hình 3.14: Context 3: Giao dịch, AI Vision OCR & Danh mục (Transactions, AI OCR & Categories Context)`
15. `Hình 3.15: Context 4: Ngân sách đa phương pháp & DSA (Budgeting & Adaptive DSA Context)`
16. `Hình 3.16: Context 5: Quỹ tích lũy & Tối ưu hóa Nợ cá nhân (Personal Debt & Financial Goals Context)`
17. `Hình 3.17: Context 6: Cú hích hành vi & Hỗ trợ quyết định Thaler (Decision Support & Behavioral Nudge Context)`

---

## 3. Kế Hoạch Triển Khai Kỹ Thuật

- **Bước 1:** Tạo bản sao lưu an toàn `files/KhoaLuanTotNghiep_backup_before_captions.docx`.
- **Bước 2:** Xây dựng script `scratch/apply_word_captions_and_references.py`:
  - Khởi tạo hàm chèn trường mã SEQ chuẩn Word (`fldChar` begin, `instrText`, `separate`, `t`, `end`).
  - Gán cặp thẻ Bookmark OpenXML duy nhất (`w:bookmarkStart`, `w:bookmarkEnd`).
  - Cập nhật định dạng đoạn văn theo đúng Rule 03 (Times New Roman, cỡ 12pt, căn giữa, in đậm số hiệu, in nghiêng tiêu đề, màu đen tự nhiên).
  - Thay thế 2 dòng placeholder ở Trang Danh mục Sơ đồ, Hình ảnh và Danh mục Bảng biểu bằng trường mã `{ TOC \h \z \c "Hình" }` và `{ TOC \h \z \c "Bảng" }` kèm danh mục hai lớp.
- **Bước 3:** Chạy script cập nhật vào `files/KhoaLuanTotNghiep.docx`.
- **Bước 4:** Xây dựng test case kiểm tra cấu trúc OpenXML trong `tests/test_docx_captions_and_references.py`.
- **Bước 5:** Chạy toàn bộ test suite để đảm bảo 100% test xanh (green).
- **Bước 6:** Ghi nhận Quyết Định 82 vào `project_context/decisions_and_changes_log.md` và báo cáo hoàn thành cho Người dùng.
