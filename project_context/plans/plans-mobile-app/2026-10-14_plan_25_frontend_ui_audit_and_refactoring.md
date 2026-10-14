# KẾ HOẠCH PLAN-25: KIỂM TOÁN & TÁI CẤU TRÚC TOÀN DIỆN FRONTEND UI REACT NATIVE (EXPO SDK 54)
## DỰ ÁN: QUẢN LÝ CHI TIÊU & HOẠCH ĐỊNH TÀI CHÍNH CÁ NHÂN THÔNG MINH
> **Phiên bản:** 1.0  
> **Ngày khởi tạo:** 14/10/2026  
> **Chủ trì thẩm định:** Hội đồng Đa Tác tử (Agent 09 Brad Frost, Agent 10 Dan Abramov, Agent 01 Richard Thaler, Agent 13 Julian Garnier)  
> **Mục tiêu:** Khắc phục triệt để các lỗi hiển thị, lỗi tương phản màu, xung đột cảm ứng, hoàn thiện màn hình Quên mật khẩu còn dang dở và nâng tầm thẩm mỹ UX cho buổi demo và thi vấn đáp đồ án.

---

## 1. BẢNG PHÂN TÍCH HIỆN TRẠNG & CÁC VẤN ĐỀ CẦN KHẮC PHỤC

| Màn Hình / Linh Kiện | Vấn Đề Phát Hiện | Tác Tử Đánh Giá | Giải Pháp Refactor |
| :--- | :--- | :--- | :--- |
| **`ForgotPasswordScreen.js`** | **Nghiêm trọng:** Vẫn là Placeholder thô sơ, chưa hề có giao diện người dùng cho quy trình 3 bước OTP. | Dan Abramov & Brad Frost | Xây dựng hoàn chỉnh giao diện Wizard 3 bước: (1) Nhập Email $\rightarrow$ (2) Nhập mã OTP 6 số $\rightarrow$ (3) Đặt mật khẩu mới. |
| **`WalletCard.js`** | **Lỗi tương phản WCAG:** Khi `isSelected = true`, nền chuyển sang `primaryLight` (#4A6FA5) nhưng chữ vẫn giữ màu đen/slate (#1E293B) $\rightarrow$ Không thể đọc được chữ! | Brad Frost (Art Director) | Đổi màu chữ sang trắng (`COLORS.textWhite`) khi thẻ được chọn, bo viền sáng tinh tế, thêm icon check chọn. |
| **`TransactionItem.js`** | **Lỗi tương phản:** Huy hiệu `walletBadge` dùng chữ xanh đen trên nền xanh thẫm, khó nhận diện. | Brad Frost | Đổi background thành `#EEF2F6`, chữ `primaryDark` đậm nét rõ ràng, thanh thoát. |
| **`TransactionsScreen.js`** | **Xung đột cảm ứng (Touch Bubbling):** Dòng giao dịch bọc trong `TouchableOpacity` cha (sửa), bên trong lại có nút `TouchableOpacity` con (xóa) $\rightarrow$ Bấm xóa bị nhảy sang sửa. | Dan Abramov (Client Arch) | Tách riêng vùng bấm nội dung (xem/sửa) và vùng nút xóa độc lập, ngăn chặn nuốt sự kiện cảm ứng. |
| **`HomeScreen.js`** | 1. Subgreeting chìm màu trên nền Header Navy.<br>2. Banner Realtime cắt ngang giữa khoảng trắng và header.<br>3. Dữ liệu mock cứng tháng 09/2026. | Brad Frost & Julian Garnier | 1. Đổi màu chữ phụ sang `rgba(255,255,255,0.85)`.<br>2. Tích hợp thanh Realtime mượt mà vào đầu màn hình.<br>3. Khởi tạo state rỗng sạch sẽ, nạp dữ liệu động tháng hiện tại. |
| **`AddTransactionScreen.js`** | 1. Phải gõ ngày `YYYY-MM-DD` bằng tay dễ sai.<br>2. Ô nhập tiền thiếu nút gõ nhanh (+10k, +50k, +100k, +500k).<br>3. Lưới icon 23% bị cụt chữ trên màn hình nhỏ. | Richard Thaler & Brad Frost | 1. Thêm chip chọn nhanh ngày ("Hôm nay", "Hôm qua") + validate regex.<br>2. Thêm thanh Quick Amount chips giảm ma sát nhận thức.<br>3. Nâng cấp lưới Category tự co giãn nhãn, tránh tràn chữ. |
| **`BudgetScreen.js`** | 1. Ô nhập tiền hạn mức không format dấu phẩy khi gõ.<br>2. Trùng lặp thông điệp cảnh báo giữa `BudgetBar` và `alertBox`. | Richard Thaler & Dan Abramov | 1. Format tiền tệ tức thời trong Modal.<br>2. Loại bỏ thông điệp thừa, gộp thông tin an toàn chi tiêu mỗi ngày sắc nét. |
| **`ReportScreen.js`** | **Lỗi sập biểu đồ khi dữ liệu rỗng:** `PieChart` và `BarChart` không có Empty State, tài khoản mới tạo sẽ bị crash hoặc hiển thị NaN. | Dan Abramov | Bổ sung Empty State trực quan ("Chưa có giao dịch chi tiêu trong kỳ") với hình minh họa và nút thêm giao dịch. |

---

## 2. LỘ TRÌNH THỰC HIỆN 4 BƯỚC (EXECUTION ROADMAP)

### Bước 1: Khắc Phục Lỗi Linh Kiện Cơ Bản (Atomic Components Polish)
- Sửa `WalletCard.js`: Sửa contrast text trắng khi active, viền vàng kim hoặc navy nổi bật.
- Sửa `TransactionItem.js`: Sửa màu `walletBadge`, định dạng ngày tháng tiếng Việt thân thiện (`14 thg 10`).
- Sửa `BudgetBar.js`: Đảm bảo thanh tiến độ không bị giật, bo góc tròn trịa.

### Bước 2: Hoàn Thiện Màn Hình Còn Thiếu (`ForgotPasswordScreen.js`)
- Thiết kế giao diện Wizard 3 bước:
  - **Bước 1:** Header icon ổ khóa, hướng dẫn, ô nhập email, nút "Gửi mã xác nhận OTP".
  - **Bước 2:** Ô nhập 6 chữ số OTP to rõ, đồng hồ đếm ngược, hiển thị gợi ý test `debug_otp`, nút "Xác thực OTP".
  - **Bước 3:** Ô nhập mật khẩu mới, ô xác nhận mật khẩu, nút "Cập nhật mật khẩu & Đăng nhập".

### Bước 3: Tối Ưu Hóa Trải Nghiệm Màn Hình Thêm Giao Dịch & Sổ Giao Dịch
- `AddTransactionScreen.js`:
  - Thêm hàng phím tắt số tiền (+10k, +50k, +100k, +500k, +1Tr, Xóa hết).
  - Thêm chip chọn nhanh ngày (Hôm nay, Hôm qua).
  - Lưới Category hiển thị icon màu sắc trực quan, text căn giữa cân đối.
- `TransactionsScreen.js`:
  - Tách sự kiện click row và click nút xóa.
  - Sửa lại bộ lọc tháng / năm linh hoạt.

### Bước 4: Tinh Chỉnh Dashboard, Báo Cáo & Kiểm Thử Tổng Thể
- `HomeScreen.js`: Sửa subgreeting, làm đẹp thẻ số dư khả dụng, làm banner Realtime gọn gàng.
- `ReportScreen.js`: Thêm Empty State khi chưa có dữ liệu biểu đồ, chỉnh kích thước biểu đồ chống tràn màn hình.
- `BudgetScreen.js`: Tinh giản layout, format tiền tệ khi nhập hạn mức.
- Rà soát toàn bộ giao diện bằng mắt và chạy kiểm thử code.

---

## 3. TIÊU CHÍ NGHIỆM THU (ACCEPTANCE CRITERIA)
1. 100% các màn hình hiển thị đầy đủ, không còn bất kỳ dòng placeholder nào (`ForgotPasswordScreen.js` chạy mượt mà cả 3 bước).
2. Không còn bất kỳ lỗi tương phản chữ chìm vào nền (tất cả văn bản đạt chuẩn WCAG AA $\ge$ 4.5:1).
3. Thao tác xóa giao dịch trong `TransactionsScreen` không bị nhảy sang màn hình sửa.
4. Mở tab Báo cáo với tài khoản trắng không bị crash app.
5. Thao tác nhập tiền có phím tắt nhanh, trải nghiệm mượt mà không ma sát.
