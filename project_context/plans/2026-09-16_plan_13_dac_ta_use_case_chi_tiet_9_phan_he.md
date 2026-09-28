# Kế Hoạch Triển Khai: PLAN-13 — Hoàn Thiện Đặc Tả Use Case Chi Tiết Chuẩn IEEE 29148 (Chương 3 Khóa Luận Tốt Nghiệp)

> **Mã kế hoạch:** `PLAN-13`  
> **Thời điểm lập:** `2026-09-16 19:15:00` (Cập nhật: `2026-09-17 07:35:00`)  
> **Phạm vi trọng tâm:** Mục 3.2 Chương 3 Khóa luận tốt nghiệp (`KhoaLuanTotNghiep.docx`) & 60 Use Cases hợp nhất  
> **Trạng thái:** `COMPLETED (Đã hoàn thành xuất sắc 100% 9/9 Phiên — Toàn bộ 60 Use Cases chuẩn IEEE 29148 Bảng Dọc 2xN đã được kiểm toán tự động)`

---

## 1. Tổng Quan & Phân Tích Hiện Trạng (System Synthesis)

Qua các giai đoạn triển khai trước, hệ thống **"Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh (PFM - Tích hợp AI)"** đã hoàn thành vững chắc các trụ cột nền tảng:
1. **Định hướng cốt lõi (Core Pivot - PLAN-09):** 100% Tài chính cá nhân độc lập (Zero group expenses, bảo vệ tính riêng tư tuyệt đối).
2. **Phát biểu bài toán chính thức (Mục 1.7 I-P-O):** Đã hoàn thiện toàn văn trong Chương 1 `files/KhoaLuanTotNghiep.docx`, phân định ranh giới 1 Tác nhân người dùng cá nhân (Primary) và 3 Tác nhân hệ thống ngoại vi (Secondary: AI Vision Engine, VietQR Gateway, Notification Service).
3. **Cơ sở lý thuyết & Công nghệ liên quan (Chương 2):** Đã khóa cứng 100% học thuật (Lý thuyết Thaler Nudge, Mental Accounting, 4 trường phái ngân sách, Snowball vs Avalanche, AI Vision OCR + Math Audit, VietQR/NAPAS 247, Domain-Driven Design, Clean Architecture).
4. **Mô hình Use Case (Use Case Diagram):** Đã thiết kế bộ sơ đồ chuẩn hóa 10 tabs, gồm 1 Sơ đồ Tổng quát và 9 Sơ đồ Phân hệ chi tiết chứa **50 Use Cases** tại `files/UCD-DuoNoPro.drawio`.
5. **Mô hình Lớp (Class Diagram - Lab 3):** Đã hoàn thành Sơ đồ Lớp 6 Bounded Contexts với 18 Domain Entities + 2 Value Objects, chuẩn hóa kiểu dữ liệu MSSQL T-SQL và định tuyến Zero-Collision tại `files/class_diagram_pfm_lab3.drawio`.

**Mục tiêu tiếp theo:**
Chuyển hóa toàn bộ 50 Use Cases từ mô hình tĩnh thành **Tài liệu Đặc tả Kịch bản Use Case chi tiết chuẩn IEEE 29148** (bao gồm thông tin tác nhân, điều kiện tiên quyết/hậu quyết, luồng sự kiện chính Happy Path, các luồng phụ Alternative, luồng ngoại lệ Exception, phi chức năng và kịch bản kiểm thử chấp nhận Gherkin BDD). Toàn bộ nội dung này sẽ hoàn thành nội dung cốt lõi của **Mục 3.2 trong Chương 3 Khóa luận tốt nghiệp**.

---

## 2. Chiến Lược Phân Kỳ Phiên Làm Việc (Roadmap by Modules)

Theo đúng định hướng của Người dùng: *"phân mỗi phiên làm việc tương ứng với số nhánh, module có trong hệ thống"*, toàn bộ 50 Use Cases sẽ được phân bổ thành **9 Phiên làm việc (Sessions)** chuyên sâu tương ứng với 9 phân hệ chức năng:

```text
┌────────────────────────────────────────────────────────────────────────────────────────┐
│ LỘ TRÌNH 9 PHIÊN LÀM VIỆC ĐẶC TẢ CHI TIẾT USE CASE (IEEE 29148)                        │
├────────────┬─────────────────────────────────────────────────┬───────────┬─────────────┤
│ Phiên (S)  │ Tên Phân Hệ / Module Chức Năng                  │ Số lượng  │ Use Case ID │
├────────────┼─────────────────────────────────────────────────┼───────────┼─────────────┤
│ Phiên 1    │ Phân hệ Quản lý Tài khoản & Đa Ví (Core Wallet)  │ 9 UCs     │ UC-01.01..09 (HOÀN THÀNH)   │
│ Phiên 2    │ Phân hệ Quản lý Thu nhập (Income Management)    │ 7 UCs     │ UC-02.01..07 (HOÀN THÀNH)   │
│ Phiên 3    │ Phân hệ Chi tiêu & Bóc tách Hóa đơn AI OCR      │ 10 UCs    │ UC-03.01..10 (HOÀN THÀNH)   │
│ Phiên 4    │ Phân hệ Danh mục Thu Chi & Quy tắc thông minh   │ 7 UCs     │ UC-04.01..07 (HOÀN THÀNH)   │
│ Phiên 5    │ Phân hệ Ngân sách Đa phương pháp (Adaptive DSA) │ 6 UCs     │ UC-05.01..06 (HOÀN THÀNH)   │
│ Phiên 6    │ Phân hệ Quỹ Tích lũy & Tối ưu hóa Nợ Cá nhân    │ 7 UCs     │ UC-06.01..07 (HOÀN THÀNH)   │
│ Phiên 7    │ Phân hệ Báo cáo & Phân tích Tài chính Đa chiều  │ 5 UCs     │ UC-07.01..05 (HOÀN THÀNH)   │
│ Phiên 8    │ Phân hệ Dự báo Tài chính & Rủi ro Thâm hụt      │ 4 UCs     │ UC-08.01..04 (HOÀN THÀNH)   │
│ Phiên 9    │ Phân hệ Cú hích Hành vi & Hỗ trợ Quyết định     │ 5 UCs     │ UC-09.01..05 (HOÀN THÀNH)   │
├────────────┴─────────────────────────────────────────────────┴───────────┴─────────────┤
│ TỔNG CỘNG: 9 PHIÊN LÀM VIỆC — 60 USE CASES ĐẶC TẢ TOÀN DIỆN 100% (HOÀN THÀNH XUẤT SẮC) │
└────────────────────────────────────────────────────────────────────────────────────────┘
```

---

## 3. Cấu Trúc Bản Đặc Tả Chuẩn Hóa Dạng Bảng Dọc 2 x N (IEEE 29148 / Cockburn Standard)

Theo đề xuất từ Người dùng kết hợp chuẩn quốc tế **IEEE 29148** và phương pháp luận **Alistair Cockburn (Agent 03 BA)**, mỗi Use Case trong hệ thống sẽ được trình bày dưới dạng một **Bảng Dọc 2 x N** (Cột trái: Tiêu đề ~25%, Cột phải: Nội dung chi tiết ~75%) gồm 13 mục chuẩn mực sau:

| Mục (Cột 1) | Nội dung đặc tả (Cột 2) | Ghi chú & Nguồn gốc |
| :--- | :--- | :--- |
| **1. Tên Use Case** | Tên hành vi nghiệp vụ rõ ràng (Động từ + Bổ ngữ) | Baseline GV |
| **2. Mã Use Case (UC ID)** | Mã số phân cấp duy nhất: `UC-XX.YY` | Baseline GV |
| **3. Mức độ ưu tiên** | Ghi ngắn gọn, dứt khoát 1 giá trị: `Bắt buộc`, `Cao`, `Trung bình`, hoặc `Thấp`. Tuyệt đối không viết rườm rà dạng chú thích (như tránh viết "Essential (Bắt buộc / Trụ cột ACID)"). | Chỉ đạo từ Người Dùng |
| **4. Tác nhân chính (Primary Actor)** | Chỉ liệt kê duy nhất Tác nhân chính khởi tạo/thực thi Use Case dựa trên bản vẽ UCD (Người dùng cá nhân). Loại bỏ hoàn toàn tác nhân phụ để tập trung vào nhân tố quyết định. | Chỉ đạo từ Người Dùng |
| **5. Mô tả tóm tắt** | Tóm tắt từ 1 đến 2 câu mục đích chính của Use Case | Baseline GV |
| **6. Điểm kích hoạt (Trigger)** | Sự kiện người dùng nhấn chọn hoặc sự kiện hệ thống tự động kích hoạt | Chuẩn Cockburn |
| **7. Tiền điều kiện (Pre-conditions)** | Các trạng thái bắt buộc hệ thống phải thỏa mãn trước khi Use Case thực thi | Baseline GV |
| **8. Hậu điều kiện (Post-conditions)** | - *Thành công:* Dữ liệu thay đổi cụ thể, số dư cập nhật, log nghiệp vụ được lưu.<br>- *Thất bại:* Hệ thống giữ nguyên trạng thái ban đầu, thông báo lỗi. | Baseline GV (mở rộng) |
| **9. Luồng chính (Basic Flow)** | Các bước tương tác tuần tự 1..N giữa Hành động của Tác nhân và Phản hồi của Hệ thống | Baseline GV |
| **10. Luồng phụ (Alternative Flows)** | Các nhánh rẽ hợp lệ (A1, A2...): Chọn phương thức khác, quét mã, dùng gợi ý tự động | Chuẩn IEEE 29148 |
| **11. Luồng ngoại lệ (Exception Flows)** | Các trường hợp lỗi nghiệp vụ / kỹ thuật (E1, E2...): Sai mật khẩu, thiếu số dư, lỗi mạng | Baseline GV |
| **12. Yêu cầu phi chức năng & Bảo mật** | Ràng buộc về Thời gian phản hồi, Bảo mật mã hóa, Giao dịch toàn vẹn ACID | Chuẩn IEEE 29148 |
| **13. Tiêu chí nghiệm thu (Acceptance Criteria)** | Kịch bản kiểm thử chấp nhận định dạng Gherkin BDD (`Given - When - Then`) | Chuẩn Agile / BDD |

### 3.1. Quy Chuẩn Văn Phong & Ngôn Ngữ Học Thuật (KLTN Standard)

Do toàn bộ nội dung đặc tả sẽ được tích hợp trực tiếp vào **Mục 3.2 trong Chương 3 Khóa Luận Tốt Nghiệp (`KhoaLuanTotNghiep.docx`)**, hệ thống bắt buộc tuân thủ 4 nguyên tắc văn phong sau:
1. **Văn phong tự nhiên & Học thuật:** Diễn đạt câu văn tiếng Việt trong sáng, gãy gọn, mạch lạc, chuẩn văn phong học thuật Việt Nam. Tránh lối viết cộc lốc hoặc máy móc.
2. **Không nửa Anh nửa Việt (No Vietglish):** Toàn bộ nội dung mô tả, luồng sự kiện, điều kiện và ngoại lệ sử dụng 100% tiếng Việt. Thuật ngữ kỹ thuật chỉ được phép mở ngoặc chú thích tiếng Anh ở tên thuộc tính (Cột 1) hoặc khi định nghĩa khái niệm kỹ thuật cốt lõi (như mã UUID, thuật toán Argon2id, câu lệnh SQL, mã chuẩn VietQR). Tuyệt đối không chêm từ tiếng Anh bừa bãi trong câu văn miêu tả.
3. **Tiêu đề & Tên Use Case tinh gọn:** Tiêu đề Use Case phải ngắn gọn, thuần Việt, đúng chuẩn nghiệp vụ (khớp 100% với bản vẽ UCD `files/UCD-DuoNoPro.drawio`). Không thêm các chú thích công nghệ rườm rà vào tiêu đề (ví dụ: viết `UC-01.07: Khởi tạo và cấu hình đa ví`, `UC-01.08: Chuyển tiền nội bộ giữa các ví`).
4. **Mức độ ưu tiên dứt khoát:** Chỉ sử dụng 4 mức phân loại chuẩn: `Bắt buộc`, `Cao`, `Trung bình`, `Thấp`. Không đính kèm các ghi chú dài dòng.

> [!TIP]
> **Định dạng lưu trữ:**
> 1. Mỗi phân hệ được xuất bản thành 1 tệp Markdown độc lập tại `project_context/reports/use_cases/` để dễ theo dõi và review.
> 2. Cấu trúc Bảng Dọc 2 x N đảm bảo hiển thị hoàn hảo trên Markdown và sẵn sàng chuyển đổi 1-1 sang bảng Word đẹp mắt khi tích hợp vào **Mục 3.2 trong Khóa luận tốt nghiệp**.

---

## 4. Kế Hoạch Triển Khai Chi Tiết Cho Từng Phiên

### **Phiên 1 (Session 1): Phân Hệ Quản Lý Tài Khoản & Đa Ví Cá Nhân (UC-01) — [ĐÃ HOÀN TẤT]**
- `UC-01.01`: Đăng ký tài khoản (User Registration & Risk Profile initialization)
- `UC-01.02`: Đăng nhập hệ thống (Authentication, JWT Rotation & MFA)
- `UC-01.03`: Đăng xuất (Logout & Invalidate Token)
- `UC-01.04`: Quản lý thông tin hồ sơ & Khẩu vị rủi ro
- `UC-01.05`: Đổi mật khẩu
- `UC-01.06`: Quên / Khôi phục mật khẩu (Secure Password Reset OTP)
- `UC-01.07`: Khởi tạo & Cấu hình Đa Ví (Tiền mặt, Ngân hàng, Thẻ tín dụng, Ví điện tử)
- `UC-01.08`: Chuyển tiền nội bộ giữa các ví (Internal Transfer with ACID Pessimistic Lock)
- `UC-01.09`: Đối soát & Cân chỉnh số dư thực tế (Reconcile Balance & Audit Trail)
- **Tệp xuất bản:** [`project_context/reports/use_cases/01_dac_ta_uc_tai_khoan_va_da_vi.md`](../reports/use_cases/01_dac_ta_uc_tai_khoan_va_da_vi.md) *(Đã kiểm toán tự động đạt 100% 9/9 Use Cases, 13/13 trường chuẩn)*.

### **Phiên 2 (Session 2): Phân Hệ Quản Lý Thu Nhập Cá Nhân (UC-02) — [ĐÃ HOÀN TẤT]**
- `UC-02.01`: Thêm khoản thu nhập mới (Gắn ví tiếp nhận & Tăng số dư tức thì)
- `UC-02.02`: Xem danh sách thu nhập (Bộ lọc theo thời gian, ví, nguồn thu)
- `UC-02.03`: Xem chi tiết khoản thu nhập
- `UC-02.04`: Chỉnh sửa khoản thu nhập (Tái tính toán số dư ví an toàn)
- `UC-02.05`: Xóa khoản thu nhập (Hoàn tác số dư)
- `UC-02.06`: Phân loại nguồn thu nhập (Lương chính, Thưởng, Thu nhập thụ động)
- `UC-02.07`: Quản lý thu nhập định kỳ (Recurring Incomes & Salary Day scheduler)
- **Tệp xuất bản:** [`project_context/reports/use_cases/02_dac_ta_uc_quan_ly_thu_nhap.md`](../reports/use_cases/02_dac_ta_uc_quan_ly_thu_nhap.md) *(Đã kiểm toán tự động đạt 100% 7/7 Use Cases, 13/13 trường chuẩn, 0 hàng vỡ dòng)*.

### **Phiên 3 (Session 3): Phân Hệ Chi Tiêu & Bóc Tách Hóa Đơn AI OCR (UC-03 — Cốt Lõi AI) — [ĐÃ HOÀN TẤT]**
- `UC-03.01`: Thêm khoản chi tiêu thủ công (Khấu trừ số dư ví thanh toán)
- `UC-03.02`: Xem danh sách chi tiêu
- `UC-03.03`: Xem chi tiết khoản chi tiêu
- `UC-03.04`: Chỉnh sửa khoản chi tiêu
- `UC-03.05`: Xóa khoản chi tiêu
- `UC-03.06`: Gắn chi tiêu với mục tiêu tích lũy / danh mục
- `UC-03.07`: Quét & Bóc tách hóa đơn bằng AI Multimodal Vision OCR
- `UC-03.08`: Hậu kiểm toán số học hóa đơn (Mathematical Audit: $\sum \text{Item} + \text{VAT} - \text{Discount} = \text{Total}$)
- `UC-03.09`: Quét & Phát hiện Bẫy đăng ký định kỳ (Zombie Subscription Leakage)
- `UC-03.10`: Tạo giao dịch thanh toán chuyển khoản VietQR (EMVCo chuẩn Napas 247)
- **Tệp xuất bản:** [`project_context/reports/use_cases/03_dac_ta_uc_chi_tieu_va_ai_ocr.md`](../reports/use_cases/03_dac_ta_uc_chi_tieu_va_ai_ocr.md) *(Đã kiểm toán tự động đạt 100% 10/10 Use Cases, 13/13 trường chuẩn, 0 hàng vỡ dòng)*.

### **Phiên 4 (Session 4): Phân Hệ Quản Lý Danh Mục Thu Chi & Quy Tắc Thông Minh (UC-04) — [ĐÃ HOÀN TẤT]**
- `UC-04.01`: Thêm danh mục mới (Tên, Biểu tượng icon, Mã màu)
- `UC-04.02`: Xem danh sách danh mục
- `UC-04.03`: Chỉnh sửa danh mục
- `UC-04.04`: Xóa danh mục (Kiểm tra ràng buộc khóa ngoại với các giao dịch cũ)
- `UC-04.05`: Thiết lập phân cấp Cha - Con (Parent-Child Recursive Hierarchy)
- `UC-04.06`: Ánh xạ danh mục chuẩn ngân sách (Pillar 50/30/20 & 4 Cột Kakeibo)
- `UC-04.07`: Đặt quy tắc phân loại tự động (Auto-Categorization Regex Rules)
- **Tệp xuất bản:** [`project_context/reports/use_cases/04_dac_ta_uc_danh_muc_va_quy_tac.md`](../reports/use_cases/04_dac_ta_uc_danh_muc_va_quy_tac.md) *(Đã kiểm toán tự động đạt 100% 7/7 Use Cases, 13/13 trường chuẩn, 0 hàng vỡ dòng)*.

### **Phiên 5 (Session 5): Phân Hệ Ngân Sách Đa Phương Pháp & DSA Thích Ứng (UC-05 — Điểm Sáng Nghiên Cứu) — [ĐÃ HOÀN TẤT]**
- `UC-05.01`: Thiết lập ngân sách chu kỳ (50/30/20, Zero-Based Budgeting, Kakeibo)
- `UC-05.02`: Theo dõi tiến độ chi tiêu theo ngân sách
- `UC-05.03`: Điều chỉnh hạn mức ngân sách
- `UC-05.04`: Tiếp nhận cảnh báo vượt ngưỡng ngân sách (Spending Micro-Alerts)
- `UC-05.05`: Tính toán Hạn mức chi tiêu mỗi ngày thích ứng (Adaptive DSA Algorithm)
- `UC-05.06`: Giám sát Tốc độ tiêu tiền (Spending Velocity Index)
- **Tệp xuất bản:** [`project_context/reports/use_cases/05_dac_ta_uc_ngan_sach_va_dsa.md`](../reports/use_cases/05_dac_ta_uc_ngan_sach_va_dsa.md) *(Đã kiểm toán tự động đạt 100% 6/6 Use Cases, 13/13 trường chuẩn, 0 hàng vỡ dòng)*.

### **Phiên 6 (Session 6): Phân Hệ Quỹ Tích Lũy & Tối Ưu Hóa Nợ Cá Nhân (UC-06 — Debt Solver) — [ĐÃ HOÀN TẤT]**
- `UC-06.01`: Thiết lập mục tiêu tích lũy (Mua sắm, Đầu tư, Du lịch)
- `UC-06.02`: Theo dõi tiến độ tích lũy
- `UC-06.03`: Đóng góp tiền vào mục tiêu (Khấu trừ ví thanh toán chuyển sang quỹ)
- `UC-06.04`: Thiết lập Quỹ Khẩn Cấp chuyên biệt (Emergency Fund 3-6 tháng)
- `UC-06.05`: Quản lý danh mục nghĩa vụ nợ cá nhân (Thẻ tín dụng, Vay tiêu dùng)
- `UC-06.06`: Mô phỏng chiến lược trả nợ tối ưu (Debt Snowball vs Avalanche)
- `UC-06.07`: Ghi nhận thanh toán khoản nợ (Hạch toán gốc và lãi vay thực tế)
- **Tệp xuất bản:** [`project_context/reports/use_cases/06_dac_ta_uc_tich_luy_va_tra_no.md`](../reports/use_cases/06_dac_ta_uc_tich_luy_va_tra_no.md) *(Đã kiểm toán tự động đạt 100% 7/7 Use Cases, 13/13 trường chuẩn, 0 hàng vỡ dòng)*.

### **Phiên 7 (Session 7): Phân Hệ Báo Cáo & Phân Tích Tài Chính Đa Chiều (UC-07) — [ĐÃ HOÀN TẤT]**
- `UC-07.01`: Báo cáo Thu - Chi theo thời gian (So sánh tuần, tháng, quý, năm)
- `UC-07.02`: Báo cáo Giá trị Tài sản ròng (Net Worth = Tổng Tài Sản Ví - Tổng Nợ)
- `UC-07.03`: Báo cáo Dòng tiền ròng (Net Cashflow = Thu Nhập - Chi Tiêu)
- `UC-07.04`: Phân tích cơ cấu danh mục chi tiêu theo tỷ trọng phần trăm
- `UC-07.05`: Phân tích xu hướng & Phát hiện biến động bất thường
- **Tệp xuất bản:** [`project_context/reports/use_cases/07_dac_ta_uc_bao_cao_tai_chinh.md`](../reports/use_cases/07_dac_ta_uc_bao_cao_tai_chinh.md) *(Đã kiểm toán tự động đạt 100% 5/5 Use Cases, 13/13 trường chuẩn, 0 hàng vỡ dòng)*.

### **Phiên 8 (Session 8): Phân Hệ Dự Báo Tài Chính & Rủi Ro Thâm Hụt (UC-08) — [ĐÃ HOÀN TẤT]**
- `UC-08.01`: Dự báo tổng chi tiêu cuối chu kỳ (Time-series projection)
- `UC-08.02`: Dự phóng Ngày cạn tiền khả dụng (Out-of-Money Date Forecast)
- `UC-08.03`: Dự báo nguy cơ thâm hụt danh mục thiết yếu
- `UC-08.04`: Mô phỏng kịch bản thu chi giả định (What-If Simulation Scenario)
- **Tệp xuất bản:** [`project_context/reports/use_cases/08_dac_ta_uc_du_bao_va_rui_ro.md`](../reports/use_cases/08_dac_ta_uc_du_bao_va_rui_ro.md) *(Đã kiểm toán tự động đạt 100% 4/4 Use Cases, 13/13 trường chuẩn, 0 hàng vỡ dòng)*.

### **Phiên 9 (Session 9): Phân Hệ Cú Hích Hành Vi & Hỗ Trợ Quyết Định (UC-09 — Thaler Nudge) — [ĐÃ HOÀN TẤT]**
- `UC-09.01`: Tiếp nhận Cú hích tài chính thời gian thực (Behavioral AI Nudge)
- `UC-09.02`: Xem căn cứ & Lý do giải trình của thuật toán AI (Explainable AI)
- `UC-09.03`: Đề xuất Tái cân đối ngân sách động (Dynamic Budget Rebalancing Nudge)
- `UC-09.04`: Xác nhận / Phê duyệt điều chỉnh (Human-in-the-loop Confirmation)
- `UC-09.05`: Đánh giá tác động mua sắm lớn & Kích hoạt Khoảng trễ tâm lý (Cooling-off Period)
- **Tệp xuất bản:** [`project_context/reports/use_cases/09_dac_ta_uc_cu_hich_thaler_nudge.md`](../reports/use_cases/09_dac_ta_uc_cu_hich_thaler_nudge.md) *(Đã kiểm toán tự động đạt 100% 5/5 Use Cases, 13/13 trường chuẩn, 0 hàng vỡ dòng)*.

---

## 5. Kế Hoạch Nghiệm Thu & Tích Hợp Vào Báo Cáo KLTN (Integration Phase)

Sau khi hoàn tất 9 phiên đặc tả:
1. **Tổng hợp Báo cáo Toàn văn:** Xuất bản tài liệu hợp nhất `project_context/reports/BaoCao_TongHop_DacTa_UseCase_PFM.md`.
2. **Cập nhật CHƯƠNG 3 trong Khóa Luận Tốt Nghiệp (`KhoaLuanTotNghiep.docx`):**
   - Đưa toàn bộ nội dung phân tích Use Case vào **Mục 3.2 (Đặc tả kịch bản Use Case)**.
   - Định dạng chuẩn DNTU (Times New Roman 13pt, Bảng biểu căn giữa, dãn dòng 1.5 lines, Before/After 6pt).
   - Thiết lập đầy đủ Bookmark XML OpenXML và liên kết hai lớp với Mục lục tự động F9.
3. **Dọn dẹp toàn bộ thư mục tạm & Artifact Cache (Scratch & Cache Cleanup):**
   - Xóa bỏ toàn bộ thư mục `scratch/` và các tệp tin script phụ trợ trong workspace dự án.
   - Dọn dẹp các tệp tin tạm trong thư mục artifact cache của Antigravity (`brain/<conversation-id>/scratch/`), bảo đảm hệ thống tuyệt đối tinh gọn, sạch sẽ theo đúng chỉ đạo của người dùng.
