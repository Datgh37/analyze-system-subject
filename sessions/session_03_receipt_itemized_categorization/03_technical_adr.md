# QUYẾT ĐỊNH KIẾN TRÚC: ADR-OCR-01 — HÀNG CHỜ XỬ LÝ ẢNH BẤT ĐỒNG BỘ & HẬU KIỂM TOÁN HỌC

## 1. Trạng Thái: ACCEPTED

## 2. Bối Cảnh Nghiệp Vụ
Quá trình gọi Vision LLM để đọc hóa đơn mất 2-4 giây. Nếu xử lý đồng bộ trên main API thread sẽ gây tắc nghẽn server và trải nghiệm người dùng kém.

## 3. Quyết Định Kỹ Thuật
- Triển khai Message Queue bất đồng bộ: Client gửi ảnh nhận ngay `Job ID (HTTP 202 Accepted)`. Background Worker xử lý ảnh và bắn kết quả qua SSE / WebSocket.
- Sử dụng kiểu dữ liệu **`DECIMAL(15, 2)`** cho từng mặt hàng trong bảng `receipt_items`.
- Kiểm tra tính bất biến và Chống Gửi Trùng Lặp (Idempotency) dựa trên mã băm nội dung ảnh (`SHA-256`).
- Cơ chế Khóa Concurrency: Áp dụng **`Pessimistic Locking (SELECT * FROM wallets WHERE id = $1 FOR UPDATE)`** khi khấu trừ số tiền hóa đơn vào ví người dùng.

## 4. Hệ Quả & Đánh Đổi (Trade-offs & Consequences)
- **Ưu điểm:** API phản hồi tức thì < 150ms, khả năng mở rộng chịu tải cao.
- **Đánh đổi:** Cần quản lý vòng đời của background worker và hạ tầng hàng đợi Redis/RabbitMQ.
