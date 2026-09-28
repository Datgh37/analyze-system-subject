# 🧭 KẾ HOẠCH & LỘ TRÌNH TRIỂN KHAI ĐỒ ÁN MOBILE (PLAN-18)
## HỌC PHẦN: LẬP TRÌNH THIẾT BỊ DI ĐỘNG (MOBILE APPLICATION DEVELOPMENT)
> **Ngày lập:** 2026-09-28  
> **Người lập:** Trịnh Thành Đạt (Trưởng nhóm) & Council Orchestrator  
> **Đối tượng áp dụng:** Nhóm sinh viên thực hiện (Đạt & Đại)  
> **Tệp thực thi chính:** [`mobile-project/ROADMAP.md`](../../mobile-project/ROADMAP.md) | [`mobile-project/README.md`](../../mobile-project/README.md)

---

## 1. Mục Đích & Bối Cảnh Lập Kế Hoạch

1. **Đồng bộ hóa làm việc nhóm qua GitHub:** Nhóm sinh viên cần một tệp lộ trình chi tiết bằng Markdown đặt ngay trong thư mục `mobile-project/` để khi `git push` lên GitHub, thành viên (Trần Trọng Đại) chỉ cần clone/pull về là nắm rõ toàn bộ các công việc cần làm, thời hạn và phân vai trách nhiệm.
2. **Phân định trách nhiệm rõ ràng (Tránh đùn đẩy):**
   - **Trịnh Thành Đạt (Trưởng nhóm - 70%):** Chủ trì kiến trúc CSDL, Core Backend Auth & Báo cáo, Khung điều hướng React Navigation, Tích hợp & Review code.
   - **Trần Trọng Đại (Thành viên - 30%):** Hiện thực hóa các API CRUD Ví & Giao dịch, Kiểm thử Postman & Chụp minh chứng JSON, Xây dựng các UI Components dùng chung, Màn hình Lịch sử & Báo cáo Word.
3. **Cầu nối hai môn học:** Dùng kết quả thực thi của đồ án Mobile (MySQL DDL, API PHP, UI Screenshots) để nuôi lại Chương 3 và Chương 4 của môn Phân tích Thiết kế Hệ thống Thông tin (PTTKHTTT) & Khóa luận Tốt nghiệp (KLTN).

---

## 2. Tổng Hợp 5 Giai Đoạn Triển Khai (Sprint Breakdown)

| Giai đoạn | Nội dung trọng tâm | Trách nhiệm chính | Sản phẩm đầu ra (Deliverables) |
| :---: | :--- | :---: | :--- |
| **Giai đoạn 1 (Tuần 1)** | Thiết kế ERD CSDL & Khởi tạo môi trường Server | Đạt & Đại | File `database_schema.sql`, ERD Draw.io, DB sống trên server `103.139.203.43`, Git repo. |
| **Giai đoạn 2 (Tuần 2)** | Xây dựng Backend PHP REST API & Test Postman | Đại (CRUD) & Đạt (Auth/Reports) | Toàn bộ file PHP API trên server, Postman Collection JSON, bộ ảnh chụp kết quả JSON. |
| **Giai đoạn 3 (Tuần 3)** | Khởi tạo React Native & Dựng khung Navigation | Đạt (Lõi) & Đại (Components) | Shell App React Native chạy trên máy ảo/máy thật, thanh Bottom Tabs 4 tab mượt mà. |
| **Giai đoạn 4 (Tuần 4–5)** | Ghép nối API & Hoàn thiện các Màn hình App | Đạt & Đại | App hoàn chỉnh chạy thực tế trên điện thoại, đồng bộ số dư và vẽ biểu đồ tròn/cột. |
| **Giai đoạn 5 (Tuần 5–6)** | Tích hợp Báo cáo Word (`BaoCaoLTMB.docx`) & Nghiệm thu | Đạt & Đại | Tệp Word nộp bài hoàn chỉnh, cập nhật mục lục tự động F9, slide thuyết trình demo. |

---

## 3. Quy Ước Phối Hợp & Kiểm Soát Mã Nguồn (Git Protocol)

- **Nhánh chính (`main`):** Chỉ chứa mã nguồn đã test chạy ổn định không lỗi.
- **Nhánh phát triển (`develop`):** Nhánh tích hợp chung của nhóm.
- **Nhánh tính năng (`feature/*`):** Từng thành viên tạo nhánh riêng khi làm (`feature/backend-api-auth`, `feature/ui-screens`...).
- **Quy tắc Commit:** Bắt buộc viết commit message có ý nghĩa theo chuẩn (`feat: ...`, `fix: ...`, `ui: ...`, `docs: ...`).

---

## 4. Kiểm Toán Độc Lập & Xác Nhận Lưu Ngữ Cảnh

- Kế hoạch đã được xuất bản nguyên văn tại: [`mobile-project/ROADMAP.md`](../../mobile-project/ROADMAP.md).
- Đã liên kết tham chiếu vào [`mobile-project/README.md`](../../mobile-project/README.md).
- Ghi nhận quyết định phê duyệt vào [`project_context/decisions_and_changes_log.md`](../decisions_and_changes_log.md) (Quyết định 59).
