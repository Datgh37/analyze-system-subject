# QUYẾT ĐỊNH KIẾN TRÚC: ADR-VELOCITY-01 — ĐỘNG CƠ DỰ BÁO CHUỖI THỜI GIAN & QUẢN LÝ DỊCH VỤ ĐỊNH KỲ

## 1. Trạng Thái: ACCEPTED

## 2. Bối Cảnh Nghiệp Vụ
Cần tính toán thời gian thực tốc độ tiêu tiền và tự động phát hiện các khoản trừ tiền định kỳ từ dữ liệu lịch sử giao dịch.

## 3. Quyết Định Kỹ Thuật
- Cài đặt module `spending_velocity_calc.py` tính toán tốc độ tiêu tiền dạng Time-series Extrapolation tuyến tính.
- Bảng CSDL chuẩn 3NF: `recurring_bills` (id UUID, user_id UUID, service_name VARCHAR(100), amount DECIMAL(15,2), billing_cycle VARCHAR(20), next_due_date DATE, auto_renew BOOLEAN).
- Đảm bảo kiểu dữ liệu tiền tệ **`DECIMAL(15, 2)`** cho mọi phép toán số học.
- Cơ chế Khóa Concurrency: Áp dụng **`Pessimistic Locking (SELECT * FROM recurring_bills WHERE id = $1 FOR UPDATE)`** khi worker định kỳ kích hoạt giao dịch tự động.

## 4. Hệ Quả & Đánh Đổi (Trade-offs & Consequences)
- **Ưu điểm:** Tính toán cực nhanh O(1), thông báo kịp thời trước khi người dùng thực sự hết tiền.
- **Đánh đổi:** Dự báo dựa trên giả định tốc độ chi tiêu giữ nguyên trong tương lai, cần cập nhật lại mỗi khi có giao dịch lớn phát sinh.
