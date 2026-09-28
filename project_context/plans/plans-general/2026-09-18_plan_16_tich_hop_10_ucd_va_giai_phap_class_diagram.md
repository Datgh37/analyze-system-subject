# KẾ HOẠCH CHI TIẾT: TÍCH HỢP 10 SƠ ĐỒ USE CASE, TRỌN BỘ ĐẶC TẢ IEEE 29148 & PHÂN RÃ CLASS DIAGRAM THEO 6 BOUNDED CONTEXTS

- **Ngày ban hành:** 2026-09-18
- **Tác giả:** Hội đồng Đa tác tử & Nhóm Sinh viên Đề tài PFM
- **Tài liệu tác động:** `files/BaoCao_TienDo_GiuaKy_PTTKHTTT.docx`
- **Tài liệu tham chiếu:** `files/KhoaLuanTotNghiep.docx`, `files/ClassDiagram.drawio.png`, `project_context/reports/BaoCao_Lab3_ThietKe_Class_Diagram_PFM.md`

---

## 1. BỐI CẢNH VÀ ĐIỀU CHỈNH YÊU CẦU GIỮA KỲ
Giảng viên hướng dẫn yêu cầu báo cáo tiến độ tập trung chuyên sâu vào 3 trục trọng tâm:
1. **Khảo sát hiện trạng hệ thống:** Đã hoàn thành và chuẩn hóa tại Phần 2 của Báo cáo tiến độ -> **BẢO TOÀN NGUYÊN VẸN 100%, KHÔNG SỬA ĐỔI**.
2. **Use Case Diagram (UCD) & Bộ Đặc Tả Chi Tiết:**
   - Tích hợp trọn vẹn 10 hình ảnh sơ đồ Use Case từ Khóa luận tốt nghiệp (UCD 00 đến UCD 09).
   - Thuyết minh nghiệp vụ chi tiết cho từng sơ đồ.
   - **Bổ sung Danh mục phân loại 60 Use Cases theo 9 phân hệ (Bảng 3.2 từ Khóa Luận).**
   - **Bổ sung trọn bộ 9 Bảng Đặc Tả Chi Tiết Ca Sử Dụng Cốt Lõi chuẩn quốc tế IEEE 29148:2018 (Bảng 3.3 đến Bảng 3.11 từ Khóa Luận).**
3. **Class Diagram:**
   - Xử lý vấn đề sơ đồ quá lớn không đọc được trên A4 bằng phương pháp chia nhỏ theo **6 Bounded Contexts (Domain-Driven Design)**.
   - Chèn 1 sơ đồ tổng thể Context Map (vĩ mô).
   - Chèn **bộ 6 sơ đồ lớp chi tiết** (cắt siêu nét từ `files/ClassDiagram.drawio.png` 13.545 x 9.504 px) để chữ hiển thị to, rõ 10-12pt.
   - Thuyết minh Rich Domain Model & Value Object bất biến `Money`.

---

## 2. QUY TRÌNH AN TOÀN DỮ LIỆU
1. Tạo bản sao lưu an toàn `files/BaoCao_TienDo_GiuaKy_PTTKHTTT.backup_before_ucd10_and_specs.docx`.
2. Bảo toàn 100% Phần 1 và Phần 2 (Tên GVHD ThS. Nguyễn Thị Liệu, Lớp TH7010904, Bảng 1 thành viên, Bảng 2 khảo sát thị trường, Bảng 3 phân hệ).
3. Bảo toàn Phần 5 và Phần Q&A vấn đáp kiến thức.
4. Tái cấu trúc an toàn phạm vi Phần 3 và Phần 4.
