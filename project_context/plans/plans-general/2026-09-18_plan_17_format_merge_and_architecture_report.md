# KẾ HOẠCH THỰC THI & LƯU VẾT TIẾN ĐỘ: PLAN 17
## CHUẨN HÓA ĐỊNH DẠNG KHÓA LUẬN, HỢP NHẤT CLASS DIAGRAM VÀ XUẤT BẢN BÁO CÁO KIẾN TRÚC TOÀN DIỆN
*Ngày thực hiện: 18/09/2026*
*Mã quyết định: Quyết định 53*

---

### 1. Bối cảnh & Yêu cầu Người dùng
Nhằm chuẩn bị tốt nhất cho buổi thuyết trình trước Hội đồng Đánh giá và hoàn thiện tài liệu khóa luận tốt nghiệp, Người dùng chỉ đạo thực thi đồng bộ 8 yêu cầu:
1. Thêm Bảng phân công nhiệm vụ và tỷ lệ đóng góp (Trịnh Thành Đạt 70%, Trần Trọng Đại 30%) vào trang cuối Khối thủ tục đầu (sau Bảng 3 Danh mục từ viết tắt).
2. Chuẩn hóa quy tắc bullet: Dấu gạch ngang `"- "` duy nhất, thụt lề bằng 1/2 tab thông thường (`0.75 cm`), giữ nguyên giãn đoạn `6pt before, 6pt after, 1.5 lines`. Rà soát chuyển đổi các nội dung không cần thiết (Yêu cầu chức năng/Phi chức năng) sang văn xuôi học thuật.
3. Hợp nhất Class Diagram vào Mục 3.3 Khóa luận: Đưa đủ 7 sơ đồ độ phân giải cao và các đoạn văn xuôi luận giải học thuật chuyên sâu; hoàn toàn không đưa các bảng thuộc tính vào.
4. Bổ sung Mục 2.5 Định hướng và Đề xuất Hệ sinh thái Công nghệ Triển khai Dự án vào Chương 2.
5. Sửa tiêu đề Mục 3.4 thành `3.4 Thiết kế tương tác và sơ đồ tuần tự hệ thống (Sequence Diagrams)` để loại bỏ từ "động học" tối nghĩa; dọn sạch các đoạn stub placeholder trong Chương 3 nhưng bảo toàn Bookmarks OpenXML.
6. Thống nhất 100% tên đề tài trên toàn bộ tài liệu: `HỆ THỐNG QUẢN LÝ CHI TIÊU VÀ HOẠCH ĐỊNH TÀI CHÍNH CÁ NHÂN THÔNG MINH (TÍCH HỢP TRÍ TUỆ NHÂN TẠO - AI)`.
7. Xuất bản tài liệu độc lập thuyết minh bằng văn xuôi toàn bộ tầm nhìn dự án, điểm mới so với thị trường, 10 sơ đồ Use Case và 6 Bounded Contexts của Class Diagram.
8. Dọn dẹp cache, file scratch thừa và kiểm tra toàn vẹn tài liệu.

---

### 2. Kết quả Triển khai & Xác thực (Verification)
- **Tệp Khóa Luận Tốt Nghiệp (`files/KhoaLuanTotNghiep.docx`):**
  - Tên đề tài đã được cập nhật đồng bộ tại Trang bìa ngoài (P9-P11), Nhiệm vụ KLTN BM205 (P24), Nhận xét GVHD BM208 (P50), Mở đầu Chương 3 (P251).
  - Đã chèn Bảng phân công nhiệm vụ (Table index 4, Academic Navy Theme, 13pt Times New Roman, unbolded body).
  - 100% bullet trong toàn văn đã chuyển sang dấu `"- "` với `Left Indent = 0.75 cm`, `Hanging Indent = -0.35 cm`, `Before = 6 pt`, `After = 6 pt`, `Line = 1.5 lines`.
  - P258 và P259 đã được chuyển đổi thành 2 đoạn văn học thuật chuẩn tắc (`First line indent = 1.5 cm`, `Left = 0`, không bullet).
  - Đã chèn 7 sơ đồ bản vẽ kỹ thuật Draw.io độ phân giải cao vào Mục 3.3 kèm bài thuyết minh văn xuôi chuyên sâu cho từng Bounded Context; không chứa bất kỳ bảng thuộc tính nào.
  - Đã bổ sung Mục 2.5 với 5 phân lớp công nghệ vào cuối Chương 2.
  - Mục 3.4 được đổi tên thành `3.4 Thiết kế tương tác và sơ đồ tuần tự hệ thống (Sequence Diagrams)`, thẻ Bookmark `_Toc_3_4` được bảo tồn 100%. Đã dọn sạch 8 đoạn stub placeholder rác trong Chương 3.
- **Tệp Thuyết Minh Ngoài Luồng (`project_context/reports/TongQuan_KienTruc_NghiepVu_PFM.md`):**
  - Đã xuất bản tài liệu dài hơn 43 KB, 295 dòng, bao quát trọn vẹn 5 chương nội dung chuyên sâu phục vụ thuyết trình và phản biện.
- **Dọn Dẹp Môi Trường:**
  - Đã xóa 31 tệp tạm, docx nháp và file test thừa, giải phóng hơn 17.2 MB bộ nhớ.
  - Bảo tồn nguyên vẹn các bản sao lưu an toàn `files/KhoaLuanTotNghiep.backup.docx` và `files/KhoaLuanTotNghiep.pre_merge.docx`.
