# Kế Hoạch Triển Khai: PLAN-11 — Chuẩn Hóa Báo Cáo Khóa Luận Tốt Nghiệp Chuẩn DNTU & Khóa Nội Dung Học Thuật Mục 1.7 Và Toàn Bộ Chương 2

> **Mã kế hoạch:** `PLAN-11`  
> **Thời điểm lập:** `2026-09-11`  
> **Phạm vi trọng tâm:** Báo cáo Khóa luận tốt nghiệp (`files/KhoaLuanTotNghiep.docx`)  
> **Trạng thái:** `COMPLETED (100% OK)`

---

## 1. Mục Tiêu Kế Hoạch
1. **Hoàn thiện Mục 1.7 (Chương 1):** Biên soạn toàn văn học thuật cho `1.7.1 Kết luận khảo sát hiện trạng và nhận diện khoảng trống nghiên cứu` (5 khoảng trống cốt lõi: Đa ví ACID, Ngân sách thích ứng DSA, Tối ưu nợ cá nhân, Nhập liệu thủ công & kiểm toán số học OCR, Bẫy chi tiêu & thiếu Cú hích Nudge) và `1.7.2 Phát biểu bài toán chính thức của hệ thống (Problem Statement)` theo mô hình I-P-O toàn diện, ranh giới 100% tài chính cá nhân với 5 Actors. Loại bỏ mục 1.7.3 ra khỏi Chương 1 và chuyển tiêu chí Class Diagram sang backlog Chương 3.
2. **Khóa cứng kết cấu 5 Chương truyền thống:** Giữ nguyên 5 chương theo Nhiệm vụ KLTN, chính thức đổi tên Chương 4 thành **`CHƯƠNG 4: KIỂM THỬ VÀ ĐÁNH GIÁ HỆ THỐNG`**.
3. **Biên soạn hoàn chỉnh CHƯƠNG 2 (Cơ sở lý thuyết và các công nghệ liên quan):** Điền toàn văn đầy đủ 100% nội dung học thuật cho 4 phân hệ tri thức:
   - 2.1 Lý thuyết Kinh tế Hành vi Thaler, Mental Accounting, 4 trường phái ngân sách (50/30/20, ZBB, Kakeibo, 6 chiếc hũ), Chiến lược trả nợ (Debt Snowball vs Avalanche).
   - 2.2 Công nghệ AI Multimodal Vision OCR, Kỹ nghệ Prompt JSON Schema ràng buộc, Thuật toán Hậu kiểm toán số học hóa đơn (Mathematical Audit) và vòng lặp Human-in-the-loop.
   - 2.3 Tiêu chuẩn thanh toán VietQR (EMVCo) và Cổng chuyển mạch liên ngân hàng NAPAS 247.
   - 2.4 Domain-Driven Design (5 Bounded Contexts, Money Value Object), Clean Architecture (DIP), Event-Driven & Async Message Queue Worker.
4. **Khóa Tiêu đề Chương 3, 4, 5:** Đặt tiêu đề chuẩn `Heading 1` kèm đoạn văn bản giữ chỗ in nghiêng.
5. **Thiết lập Bookmark XML & Mục lục tự động:** Tích hợp 70 bookmark OpenXML cho mọi đề mục và cấu hình trường động Word `{ TOC \o "1-3" \h \z \u }` cùng bảng Mục lục hiển thị tĩnh có tab leader dấu chấm `.....`.
6. **Tuân thủ quy chuẩn học thuật DNTU:** 100% Times New Roman 13pt, dãn dòng 1.5 lines, Before/After 6pt, thụt lề 1.5 cm, lề trang 2.0/2.0/3.5/2.0 cm, không số La Mã ở đề mục, không dấu chấm cuối đề mục.

---

## 2. Các Tệp Tác Động & Bàn Giao
- `files/KhoaLuanTotNghiep.docx`: Tệp tài liệu Word chính thức đã được cập nhật hoàn chỉnh.
- `files/KhoaLuanTotNghiep.backup.docx`: Bản sao lưu an toàn nguyên trạng.
- `scratch/execute_update_docx_kltn.py`: Mã nguồn Python thực thi chuyển đổi và định dạng.
- `scratch/verify_docx.py`: Bộ script kiểm toán tự động các chỉ số DNTU và tính toàn vẹn OpenXML.
- `project_context/decisions_and_changes_log.md`: Ghi nhận Quyết định 23.
- `walkthrough.md`: Báo cáo nghiệm thu chi tiết.

---

## 3. Kết Quả Nghiệm Thu
- 100% tiêu chí DNTU đạt chuẩn tuyệt đối (Times New Roman 13pt, 1.5 line spacing, 6pt before/after, 1.5 cm indent, margins 2.0/2.0/3.5/2.0 cm).
- 70/70 bookmarks XML hợp lệ, khớp ID Start/End.
- Mục 1.7.3 vắng mặt tại Chương 1 (đã lưu trữ an toàn trong backlog Chương 3).
- Toàn văn Chương 2 đầy đủ, sâu sắc, tích hợp kiến thức từ 13 chuyên gia danh nhân.
