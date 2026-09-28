# QUYẾT ĐỊNH KIẾN TRÚC: ADR-DEBT-01 — GIẢI THUẬT TỐI ƯU HÓA NỢ CÁ NHÂN

## 1. Trạng Thái: ACCEPTED

## 2. Bối Cảnh Nghiệp Vụ
Cần một giải thuật Backend độc lập với Framework để tính toán lịch trả nợ tháng, số tiền lãi phát sinh hàng tháng theo công thức dư nợ giảm dần chính xác tuyệt đối.

## 3. Quyết Định Kỹ Thuật
- Cài đặt 2 giải thuật `calculate_snowball_schedule` và `calculate_avalanche_schedule` tại Domain Service.
- Sử dụng kiểu dữ liệu an toàn tiền tệ **`DECIMAL(15, 2)`** cho toàn bộ công thức lãi suất: `monthly_rate = (apr / 100) / 12`.
- Cơ chế Khóa Concurrency: Áp dụng **`Pessimistic Locking (SELECT * FROM personal_debts WHERE id = $1 FOR UPDATE)`** khi ghi nhận khoản tiền trả nợ từ ví sang tài khoản nợ.

## 4. Hệ Quả & Đánh Đổi (Trade-offs & Consequences)
- **Ưu điểm:** Độ chính xác số học 100%, bảo vệ người dùng khỏi bẫy lãi suất tín dụng đen.
- **Đánh đổi:** Cần lưu trữ bảng timeline chi tiết các tháng trả nợ nếu người dùng muốn xem toàn bộ lộ trình.
