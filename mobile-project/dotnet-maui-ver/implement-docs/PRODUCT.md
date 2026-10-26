# PRODUCT TRUTH — SMART PERSONAL EXPENSE & FINANCIAL PLANNING (.NET MAUI)

> **Mục đích tài liệu:** Bản ghi nguồn sự thật sản phẩm (Durable Product Truth) theo chuẩn [Impeccable](https://github.com/pbakaus/impeccable).  
> Mọi quyết định thiết kế UI, điều hướng luồng, sao chép văn bản (UX Copy) và kịch bản tương tác BẮT BUỘC phải đối chiếu tài liệu này.

---

## 1. PURPOSE & VISION (MỤC ĐÍCH & TẦM NHÌN)
- **Tên sản phẩm:** Hệ thống Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh (Tích hợp AI).
- **Sứ mệnh cốt lõi:** Giúp người dùng trẻ, sinh viên và người đi làm vượt qua "nỗi đau thanh toán" (Pain of Paying) và ma sát nhận thức, tự động hóa quy trình ghi chép qua AI Vision OCR, bảo đảm kỷ luật ngân sách qua Hạn mức chi tiêu an toàn mỗi ngày (Daily Safe Spending - DSA) và cung cấp lộ trình thoát nợ cá nhân rõ ràng (Debt-Free Strategy).
- **Phạm vi nghiêm ngặt:** **100% Quản trị Tài chính Cá nhân (Personal Finance)** — Tuyệt đối không chia tiền nhóm, không quỹ du lịch tập thể.

---

## 2. TARGET AUDIENCE (CHÂN DUNG NGƯỜI DÙNG)
- **Nhóm 1 - Sinh viên & Người mới đi làm (Gen Z / Young Professionals):**
  - Thu nhập từ 4.000.000 đ – 20.000.000 đ/tháng.
  - Thường xuyên chi tiêu bằng quét mã QR và ví điện tử; dễ mất kiểm soát vào tuần cuối tháng trước ngày nhận lương.
  - Cần: Giao diện trực quan, nhập liệu siêu nhanh, thanh tiến độ ngân sách đổi màu cảnh báo, thông báo nổi nhắc nhở kịp thời.
- **Nhóm 2 - Người có nhiều nguồn tiền & Các khoản nợ tiêu dùng (Debt Payers):**
  - Sở hữu đồng thời nhiều ví: Tiền mặt, 2-3 thẻ ngân hàng, thẻ tín dụng (Credit Card).
  - Có các khoản nợ thẻ tín dụng hoặc vay trả góp cần tối ưu hóa lãi suất.
  - Cần: Sổ cái minh bạch, thuật toán so sánh Snowball vs. Avalanche, biểu đồ dòng tiền 6 tháng.

---

## 3. VOICE & TONE (GIỌNG VĂN & TÍNH CÁCH THIẾT KẾ)
- **Điềm tĩnh & Minh bạch (Calm & Transparent):** Số liệu tài chính phải luôn rõ ràng, font số chuẩn tabular (`123,456,789 đ`), không giấu phí hay làm mờ thông tin.
- **Động viên & Không phán xét (Encouraging, Non-judgmental):** Khi người dùng chi tiêu vượt hạn mức DSA, không dùng từ ngữ tiêu cực hay trách móc; sử dụng "cú hích hành vi" (Thaler Nudge) mang tính xây dựng: *"Bạn đã chi 85% hạn mức hôm nay. Hãy giữ lại 50.000 đ cho ngày mai nhé!"*.
- **Chính xác tuyệt đối (Absolute Mathematical Precision):** Sử dụng `decimal` 100%, không làm tròn cẩu thả gây sai lệch số dư ví.

---

## 4. CORE CONSTRAINTS & OPERATING REALITY (RÀNG BUỘC KỸ THUẬT)
- **Ngoại tuyến là mặc định (Offline-First):** Ứng dụng phải hoạt động mượt mà khi người dùng đứng trong thang máy hoặc tầng hầm siêu thị mất sóng (SQLite local cache).
- **Tốc độ phản hồi 0ms:** Cập nhật biến động số dư tức thời qua SignalR WebSocket và Two-way MVVM Data Binding.
- **Không có rung bàn phím:** Trải nghiệm gõ số mượt mà, không kích hoạt rung phản hồi haptic để tiết kiệm pin và tránh phiền toái.
- **Thông báo đẩy hệ thống thật:** Bắn thông báo nổi trên Notification Bar khi có biến động hoặc cảnh báo DSA.
