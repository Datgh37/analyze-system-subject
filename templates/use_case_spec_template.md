# Đặc Tả Use Case Chuẩn PTTKHTTT (IEEE 29148 Standard)

## 1. Thông Tin Chung (Use Case Overview)
- **Mã Use Case (UC-ID):** `UC-[Mã số, vd: UC-01]`
- **Tên Use Case:** `[Động từ + Bổ ngữ, vd: Chia Tiền Hóa Đơn Nhóm]`
- **Mức độ ưu tiên (Priority):** `[High / Medium / Low]`
- **Tác nhân chính (Primary Actor):** `[User cá nhân / Thủ quỹ nhóm / Thành viên nhóm]`
- **Tác nhân phụ (Secondary Actor):** `[AI Vision Worker / Hệ thống Ngân hàng]`
- **Mô tả ngắn gọn (Brief Description):** `[1-2 câu tóm tắt mục đích chính]`

---

## 2. Điều Kiện Tiên Quyết & Hậu Quyết (Pre/Post Conditions)
- **Điều kiện tiên quyết (Pre-conditions):**
  - `[Ví dụ: Người dùng đã đăng nhập và đang là thành viên của Nhóm X]`
  - `[Ví dụ: Nhóm có trạng thái 'Active' và chưa bị khóa chốt sổ]`
- **Điều kiện hậu quyết thành công (Post-conditions - Success):**
  - `[Ví dụ: Giao dịch chi tiêu được tạo thành công, các khoản nợ thành viên được cập nhật]`
  - `[Ví dụ: Đồ thị nợ nhóm được tính toán lại và lưu trạng thái mới]`
- **Điều kiện hậu quyết thất bại (Post-conditions - Failure):**
  - `[Ví dụ: Dữ liệu được rollback về trạng thái trước khi chia tiền]`

---

## 3. Luồng Sự Kiện Chính (Main Flow / Happy Path)
| Bước | Hành động của Tác nhân (Actor Action) | Phản hồi của Hệ thống (System Response) |
| :--- | :--- | :--- |
| 1 | Tác nhân chọn chức năng `[Tên chức năng]` | Hệ thống hiển thị form yêu cầu nhập thông tin |
| 2 | Tác nhân nhập/tải lên thông tin `[...]` | Hệ thống kiểm tra tính hợp lệ dữ liệu đầu vào |
| 3 | Tác nhân xác nhận thực hiện | Hệ thống thực thi xử lý nghiệp vụ `[...]` |
| 4 | | Hệ thống ghi nhận kết quả vào CSDL và thông báo thành công |

---

## 4. Các Luồng Phụ & Luồng Ngoại Lệ (Alternative & Exception Flows)

### 4.1. Luồng Phụ (Alternative Flows - A)
- **A1 - [Tên luồng phụ, vd: Tải ảnh hóa đơn thay vì nhập tay]:**
  - *A1.1:* Tại bước 2 của luồng chính, Tác nhân tải ảnh chụp hóa đơn lên.
  - *A1.2:* Hệ thống kích hoạt Background Worker bóc tách dữ liệu OCR.
  - *A1.3:* Hệ thống điền trước dữ liệu đã trích xuất vào form để Tác nhân xem lại.

### 4.2. Luồng Ngoại Lệ (Exception Flows - E)
- **E1 - [Tên ngoại lệ, vd: Tranh chấp dữ liệu khi chốt sổ (Race Condition)]:**
  - *E1.1:* Nếu phát hiện có thành viên khác đang cập nhật chi tiêu trong cùng nhóm, hệ thống từ chối khóa sổ.
  - *E1.2:* Hệ thống hiển thị thông báo lỗi `[ERR_GROUP_LOCKED_OR_MODIFIED]` và yêu cầu refresh dữ liệu.
- **E2 - [Tên ngoại lệ, vd: Hóa đơn bị trùng lặp]:**
  - *E2.1:* Hệ thống phát hiện hash ảnh hoặc mã giao dịch đã tồn tại.
  - *E2.2:* Hệ thống hiển thị cảnh báo trùng lặp và yêu cầu xác nhận ghi đè hoặc hủy.

---

## 5. Yêu Cầu Phi Chức Năng (Non-Functional Requirements)
- **Độ trễ (Latency):** Thời gian phản hồi nghiệp vụ < 500ms; bóc tách OCR < 3s.
- **Tính toàn vẹn (ACID):** Mọi giao dịch tài chính nhóm phải bọc trong Transaction.
- **Bảo mật (Security):** Chỉ thành viên trong nhóm mới có quyền xem chi tiết khoản nợ của nhóm đó.

---

## 6. Tiêu Chí Nghiệm Thu (Acceptance Criteria - Gherkin Format)
```gherkin
Scenario: [Tên kịch bản thành công]
  Given [Trạng thái ban đầu]
  When [Hành động kích hoạt]
  Then [Kết quả kỳ vọng của hệ thống]

Scenario: [Tên kịch bản ngoại lệ]
  Given [Trạng thái có xung đột hoặc dữ liệu sai]
  When [Hành động kích hoạt]
  Then [Hệ thống bắt lỗi và thông báo an toàn]
```
