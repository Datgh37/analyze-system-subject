# QUYẾT ĐỊNH KIẾN TRÚC: ADR-BUDGET-01 — THIẾT KẾ CSDL ĐA VÍ & QUẢN LÝ HẠN MỨC (3NF)

## 1. Trạng Thái: ACCEPTED

## 2. Bối Cảnh Nghiệp Vụ
Hệ thống cần quản lý độc lập nhiều ví tiền (Tiền mặt, Tài khoản ngân hàng, Thẻ tín dụng, Ví MoMo) và gắn kết với các chu kỳ ngân sách hàng tháng mà không gây sai số tiền tệ.

## 3. Quyết Định Kiến Trúc & CSDL
- Sử dụng kiểu dữ liệu an toàn tuyệt đối **`DECIMAL(15, 2)`** cho toàn bộ số dư và giao dịch trong CSDL PostgreSQL.
- Bảng CSDL chuẩn 3NF:
  - `wallets` (id UUID, user_id UUID, wallet_name VARCHAR(100), wallet_type VARCHAR(20), balance DECIMAL(15,2))
  - `budgets` (id UUID, user_id UUID, month INT, year INT, total_income DECIMAL(15,2), method VARCHAR(30))
  - `budget_allocations` (id UUID, budget_id UUID, category_id UUID, allocated_amount DECIMAL(15,2), spent_amount DECIMAL(15,2))
- Cơ chế Khóa Tranh Chấp Concurrency: Áp dụng **`Pessimistic Locking (SELECT balance FROM wallets WHERE id = $1 FOR UPDATE)`** khi thực hiện chuyển tiền giữa các ví.

## 4. Hệ Quả & Đánh Đổi (Trade-offs & Consequences)
- **Ưu điểm:** Đảm bảo toàn vẹn giao dịch (ACID), chống mất mát tiền tệ khi có truy cập đồng thời.
- **Đánh đổi:** Tăng độ trễ khóa hàng đợi khoảng 15ms khi cập nhật cùng một ví.
