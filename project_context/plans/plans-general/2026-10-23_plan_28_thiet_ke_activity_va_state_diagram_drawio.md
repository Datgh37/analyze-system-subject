# KẾ HOẠCH TRIỂN KHAI PLAN-28: THIẾT KẾ ACTIVITY DIAGRAM & STATE DIAGRAM (DRAW.IO)

> **Mã kế hoạch:** `PLAN-28`  
> **Ngày khởi tạo:** 2026-10-23  
> **Mục tiêu:** Nghiên cứu và thiết kế toàn diện bộ 3 Biểu đồ Hoạt động (Activity Diagram) và 3 Biểu đồ Trạng thái (State Machine Diagram) chuẩn UML 2.5 trên Draw.io, hoàn thiện 100% Mô hình Động cho đồ án PTTKHTTT.

---

## 1. Yêu Cầu & Phạm Vi Triển Khai

1. **Chuẩn hóa Ký hiệu UML 2.5 chuyên nghiệp:**
   * **Activity Diagram:**
     - `Initial Node` (Điểm bắt đầu - chấm tròn đen đặc).
     - `Activity Final Node` (Điểm kết thúc - vòng tròn có chấm đen ở giữa).
     - `Action / Activity` (Hộp bo tròn góc, nền màu nhã nhặn, viền nét đơn).
     - `Decision / Merge Node` (Hình thoi rẽ nhánh điều kiện và gộp luồng).
     - `Fork / Join Node` (Thanh đặc đồng bộ hóa xử lý song song).
     - `Swimlanes` (Rãnh bơi phân định trách nhiệm từng bên: Người dùng, Giao diện, Backend, AI Cloud, CSDL).
     - `Control Flow` (Mũi tên chỉ hướng luồng công việc kèm điều kiện `[guard]`).
   * **State Diagram:**
     - `Initial State` & `Final State`.
     - `State Node` (Hộp bo tròn có tên trạng thái in đậm, có thể kèm entry/do/exit action).
     - `Transition` (Mũi tên chuyển trạng thái có nhãn `Sự kiện [Điều kiện] / Hành động`).
     - `Choice / Junction Node` (Hình thoi rẽ nhánh trạng thái).

2. **Quy chuẩn Định dạng & Thẩm mỹ:**
   * Cỡ chữ chuẩn **15px** (`fontSize=15;`) to rõ, không cần zoom lớn.
   * Font chữ: `Segoe UI, Arial, sans-serif`.
   * Tránh hoàn toàn lỗi double-escaping XML (`&amp;lt;`).
   * Đồng bộ 100% phong cách thẩm mỹ với `UCD-DuoNoPro.drawio`, `ClassDiagram.drawio` và `SequenceDiagram.drawio`.

---

## 2. Danh Mục Các Sơ Đồ Cần Thiết Kế

### 🅰️ Tệp `files/ActivityDiagram.drawio` (Gồm 3 Tabs)

* **Tab 1 - AD-01: Quy trình Bóc Tách Hóa Đơn AI Vision OCR & Hậu Kiểm Toán Số Học**
  * *Layout Swimlanes:* Rãnh bơi dọc (Vertical Swimlanes) gồm 5 cột:
    1. `Người dùng cá nhân`
    2. `Giao diện Mobile (UI)`
    3. `Backend Receipt Service`
    4. `Dịch vụ AI Vision Cloud`
    5. `Cơ sở dữ liệu (PostgreSQL)`
  * *Luồng nghiệp vụ:* Chụp ảnh $\rightarrow$ Gửi ảnh $\rightarrow$ AI OCR bóc tách danh mục món và thuế $\rightarrow$ Động cơ kiểm toán tính $\Delta = |(\sum \text{Món} + \text{Thuế}) - \text{Tổng}|$ $\rightarrow$ Rẽ nhánh Decision $\Delta = 0$ (Khớp tiền) vs $\Delta > 0$ (Lệch tiền, bật cảnh báo nghi vấn) $\rightarrow$ Người dùng xem lại/sửa $\rightarrow$ Bấm lưu $\rightarrow$ Ghi nhận CSDL & Trừ ví.

* **Tab 2 - AD-02: Quy trình Đánh Giá Mua Sắm Lớn & Khoảng Trễ Tâm Lý Cooling-off 24h**
  * *Layout Swimlanes:* Rãnh bơi dọc gồm 4 cột:
    1. `Người dùng cá nhân`
    2. `Mobile App Client`
    3. `Backend Cooling-off Manager`
    4. `Cron Scheduler Worker`
  * *Luồng nghiệp vụ:* Nhập khoản mua sắm tùy chọn $\rightarrow$ Kiểm tra điều kiện chi tiêu $> 3$ triệu $\rightarrow$ Tính quy đổi giờ công lao động & mức tụt DSA $\rightarrow$ Lưu giao dịch trạng thái chờ khóa 24h (chưa trừ ví) $\rightarrow$ Bật đồng hồ đếm ngược 24h $\rightarrow$ Sau 24h Cron Worker đẩy Push Notification $\rightarrow$ Decision sau 24h: Bấm "Hủy mua" (Tiết kiệm thành công) vs Bấm "Xác nhận mua" (Chính thức trừ tiền ví).

* **Tab 3 - AD-03: Quy trình Hạn Mức Ngày DSA Thích Ứng & Bắn Cú Hích Thaler Nudge Kèm XAI**
  * *Layout Swimlanes:* Rãnh bơi dọc gồm 4 cột:
    1. `Sự kiện Chi tiêu (Event)`
    2. `Động cơ DSA Engine`
    3. `Động cơ Nudge Thaler`
    4. `Người dùng (Mobile UI)`
  * *Luồng nghiệp vụ:* Nhận sự kiện giao dịch mới $\rightarrow$ Tính lại $\text{DSA} = \text{Tiền còn} / \text{Ngày còn}$ $\rightarrow$ Tính tốc độ tiêu tiền $V = \text{Đã tiêu} / \text{DSA}$ $\rightarrow$ Decision: Nếu $V \le 1.2$ thì kết thúc; nếu $V > 1.2$ hoặc chi $> 85\%$ DSA $\rightarrow$ Tạo bản ghi Nudge $\rightarrow$ Gửi Push Notification vi mô $\rightarrow$ Người dùng chạm xem $\rightarrow$ Động cơ XAI truy xuất công thức và hiển thị Modal giải trình minh bạch.

---

### 🅱️ Tệp `files/StateDiagram.drawio` (Gồm 3 Tabs)

* **Tab 1 - State-01: Vòng Đời Trạng Thái Giao Dịch Chi Tiêu (`Expense / Transaction`)**
  * *Đối tượng:* Bảng dữ liệu `Expenses` trong CSDL.
  * *Các trạng thái chính:*
    - `[*] (Start)`
    - `DRAFT`: Đang trong quá trình chụp ảnh OCR hoặc nhập khẩu lệnh giọng nói.
    - `VERIFIED`: Đã qua kiểm toán số học, dữ liệu hợp lệ.
    - `PENDING_COOLING_OFF`: Khoản chi lớn bị chặn tâm lý, tạm khóa 24h (chưa khấu trừ ví).
    - `CANCELLED`: Người dùng bấm từ bỏ món đồ bốc đồng sau 24h $\rightarrow$ `[*] (End)`.
    - `COMPLETED`: Giao dịch được xác nhận, tiền đã chính thức khấu trừ khỏi ví $\rightarrow$ `[*] (End)`.

* **Tab 2 - State-02: Vòng Đời Trạng Thái Kế Hoạch Trả Nợ Cá Nhân (`DebtPlan`)**
  * *Đối tượng:* Kế hoạch xóa nợ của người dùng trong Bounded Context Debt Management.
  * *Các trạng thái chính:*
    - `[*] (Start)`
    - `PLANNING`: Người dùng liệt kê các món nợ và chọn chiến lược (Debt Snowball hoặc Debt Avalanche).
    - `ACTIVE`: Kế hoạch đang thực thi, người dùng thanh toán nợ định kỳ hàng tháng.
    - `ACCELERATED`: Phát hiện có nguồn thu nhập đột xuất/thặng dư ngân sách, tự động dồn thêm tiền trả sớm.
    - `OVERDUE_RISK`: Chậm trả nợ quá 15 ngày, kích hoạt chế độ báo động thắt chặt chi tiêu khác.
    - `SETTLED / PAID_OFF`: Toàn bộ dư nợ đã về 0, hoàn tất mục tiêu tài chính tự do nợ nần $\rightarrow$ `[*] (End)`.

* **Tab 3 - State-03: Vòng Đời Trạng Thái Cú Hích Hành Vi & Giải Trình Minh Bạch (`BehavioralNudge & XAI`)**
  * *Đối tượng:* Bản ghi cú hích tâm lý trong bảng `BehavioralNudges`.
  * *Các trạng thái chính:*
    - `[*] (Start)`
    - `TRIGGERED`: Động cơ Thaler phát hiện vi phạm quy tắc DSA hoặc tốc độ tiêu tiền quá cao.
    - `DISPATCHED`: Đã gửi gói tin qua Firebase Cloud Messaging (FCM) đến thiết bị.
    - `DISPLAYED`: Thông báo vi mô hiển thị trên màn hình khóa hoặc Notification Tray.
    - `EXPLAINED`: Người dùng bấm vào nút "Tại sao tôi nhận lời khuyên này?" và đọc Modal giải trình XAI.
    - `DISMISSED`: Người dùng gạt bỏ thông báo mà không hành động $\rightarrow$ `[*] (End)`.
    - `CONVERTED`: Người dùng nghe theo cú hích (dừng mua sắm hoặc hạ ngân sách), ghi nhận thành công $\rightarrow$ `[*] (End)`.

---

## 3. Kế Hoạch Triển Khai Chi Tiết

| Bước | Nhiệm vụ | Đầu ra | Trạng thái |
| :---: | :--- | :--- | :---: |
| **B1** | Lập Implementation Plan & Trình người dùng phê duyệt | `PLAN-28` & Artifact | **Đang thực hiện** |
| **B2** | Viết script Python tự động hóa sinh tệp `files/ActivityDiagram.drawio` (3 tabs) | `ActivityDiagram.drawio` | Chờ duyệt |
| **B3** | Viết script Python tự động hóa sinh tệp `files/StateDiagram.drawio` (3 tabs) | `StateDiagram.drawio` | Chờ duyệt |
| **B4** | Kiểm định toàn vẹn XML, kích thước font chữ 15px và kiểm tra hiển thị | Báo cáo kiểm thử | Chờ duyệt |
| **B5** | Cập nhật `decisions_and_changes_log.md` & Báo cáo hoàn tất | Quyết định 94 | Chờ duyệt |
