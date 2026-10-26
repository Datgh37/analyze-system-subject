# KẾ HOẠCH 53: CHUẨN HÓA UI CĂN BẢN, THIẾT KẾ BOTTOM NAVBAR THEO BẢN SKETCH TAY & TÁI CẤU TRÚC HAMBURGER SIDEBAR

> **Mã kế hoạch:** `PLAN-53` (Kế hoạch căn chỉnh UI & Bàn giao Frontend Handoff)  
> **Thời gian khởi tạo:** 2026-10-26 22:55  
> **Mục tiêu chiến lược:** Căn chỉnh bố cục UI cơ bản theo đúng bản sketch tay trên giấy của Trưởng nhóm, tích hợp thanh Bottom Navigation Bar 4 Tabs kèm Center Docked Floating Mic (🎙️), tái cấu trúc phân nhóm Hamburger Sidebar, hạ tỷ lệ Typography chống tràn layout và xuất bản Tài liệu bàn giao chuyên sâu cho thành viên phụ trách Frontend (`HUONG_DAN_TRIEN_KHAI_IMMERSIVE_FRONTEND.md`).

---

## 1. PHÂN TÍCH YÊU CẦU & BẢN SKETCH TAY CỦA NGƯỜI DÙNG

### 1.1 Giải Mã Bản Vẽ Sketch Trên Giấy Ô Ly
- **Vùng Content chính:** Chiếm toàn bộ phần thân màn hình từ trên xuống, cho phép cuộn nội dung mượt mà.
- **Thanh Bottom Navigation Bar (`App navbar ->`):**
  - Nằm ngay phía trên thanh điều hướng ảo của hệ điều hành (`Navbar trên mobile` - Android System Gesture/Button Bar), có khoảng cách an toàn (Bottom Inset).
  - Gồm **4 Tabs nghiệp vụ chính** chia đều 2 bên:
    - Bên trái: `[Tab 1: Ví & Tài Sản]` và `[Tab 2: Chi Tiêu]`
    - Bên phải: `[Tab 3: Ngân Sách]` và `[Tab 4: Báo Cáo]`
  - Chính giữa: **Nút tròn lớn nhô cao (Center Docked Floating Action Button - FAB)** có hình cây **Microphone (🎙️)**. Đây là phím tắt kích hoạt trực tiếp Trợ Lý Giọng Nói Thông Minh Voice AI (vừa hoàn thiện ở Kế hoạch 52).
- **Thiết kế công thái học (Ergonomics):** Giúp người dùng thao tác 100% bằng 1 tay (ngón cái) mà không cần phải vươn tay lên góc trên cùng bên trái để bấm Hamburger menu.

### 1.2 Các Vấn Đề Giao Diện Cần Khắc Phục Ngay
1. **Bể layout & Căn chỉnh không đều:**
   - Nhiều thẻ card có padding/margin chưa đồng bộ (chỗ 24, chỗ 12, chỗ sát mép).
   - Chưa có đệm lề đáy (`Padding Bottom >= 90px`), dẫn đến khi cuộn xuống đáy thì các nút Lưu/Xác nhận bị thanh điều hướng che khuất.
2. **Font chữ bị quá to và thô:**
   - Các tiêu đề và số tiền nhiều chỗ đặt kích thước `32-34pt` gây tràn dòng trên màn hình điện thoại có độ phân giải hẹp.
   - Cần chuẩn hóa lại thang kích cỡ chữ (Typography Scale): Tiêu đề chính `18-20pt`, Tiêu đề thẻ `13-14pt`, Nội dung `12pt`, Chú thích `10pt`.
3. **Hamburger Sidebar (Flyout Menu):**
   - Hiện tại là một danh sách phẳng dài 13 mục nối tiếp nhau, người dùng cuộn rất mệt và khó tìm chức năng.
   - Cần gom nhóm thành **5 phân hệ trực quan** có Header ngăn cách và icon sinh động.

---

## 2. LỘ TRÌNH TRIỂN KHAI 4 BƯỚC (STEP-BY-STEP ROADMAP)

### 📌 Bước 1: Thiết Kế & Hiện Thực Bottom Navigation Bar (Center Docked Voice FAB)
- Tạo component `BottomNavDockView.xaml` hoặc tích hợp cấu trúc Bottom Bar trong `AppShell.xaml`:
  - 4 nút điều hướng:
    - 👛 **Ví (`WalletsPage`)**
    - 💸 **Chi Tiêu (`ExpensesPage`)**
    - 📊 **Ngân Sách (`BudgetsPage`)**
    - 📑 **Báo Cáo (`ReportsPage`)**
  - 1 nút trung tâm nhô cao:
    - 🎙️ **Voice AI Mic Button:** Nút tròn đường kính 56px, nền Gradient Indigo/Cyan, đổ bóng tím rực rỡ, bấm vào là kích hoạt `VoiceMicModalPage`.

### 📌 Bước 2: Tái Cấu Trúc Hamburger Sidebar (Flyout Hierarchy)
- Cập nhật `AppShell.xaml`: Tái cấu trúc thành 5 nhóm có vạch ngăn cách và tiêu đề phụ tinh tế:
  1. 💎 **Tài Chính Cốt Lõi:** Đa Ví, Chi Tiêu, Thu Nhập, Thu Nhập Định Kỳ.
  2. 🤖 **AI Vision & Tiện Ích:** Quét Hóa Đơn AI, Bóc Tách Bảng Lương, Tiện Ích VietQR.
  3. 🎯 **Kỷ Luật & Mục Tiêu:** Ngân Sách DSA, Mục Tiêu Tích Lũy, Danh Mục Thu Chi, Quy Tắc Tự Động.
  4. 🧠 **Tâm Lý & Dự Báo:** Cú Hích Thaler Nudge, Khoảng Trễ 24h, Dự Báo Dòng Tiền, Báo Cáo.
  5. ⚙️ **Hệ Thống & Tài Khoản:** Lịch Sử Thông Báo, Tài Khoản & Đăng Nhập.

### 📌 Bước 3: Căn Chỉnh Layout & Chuẩn Hóa Typography
- Rà soát các view cốt lõi (`WalletsPage.xaml`, `ExpensesPage.xaml`, `BudgetsPage.xaml`, `ReportsPage.xaml`):
  - Hạ kích thước font chữ tổng thể xuống mức cân đối, thanh lịch chuẩn Dark Luxury Fintech.
  - Thêm padding đáy `Padding="..., ..., ..., 90"` trên tất cả các `ScrollView`.
  - Canh lề đều các Grid, giảm bớt các khoảng trống thừa.

### 📌 Bước 4: Soạn Thảo Tài Liệu Bàn Giao Frontend Handoff (`HUONG_DAN_TRIEN_KHAI_IMMERSIVE_FRONTEND.md`)
- Soạn thảo tài liệu bàn giao chuyên biệt cho thành viên làm Frontend:
  - Bản đồ cấu trúc thư mục Frontend.
  - Quy tắc Design System: Bảng màu Semantic Tokens, Typography, Radius, Shadows.
  - Hướng dẫn chi tiết cách tinh chỉnh XAML, Animation và Responsive Layout.
  - Danh sách checklist công việc cụ thể để bạn cùng nhóm tiếp quản và hoàn thiện vào ngày mai.

---

## 3. TIÊU CHÍ NGHIỆM THU (ACCEPTANCE CRITERIA)
- [x] 1. Giao diện có thanh Bottom Navigation Bar gồm 4 Tab chính và 1 Nút Micro Voice AI tròn nổi bật ở giữa đúng theo sketch tay.
- [x] 2. Hamburger Sidebar được phân cấp thành 5 nhóm chuyên đề rõ ràng, không còn dải phẳng dài thô sơ.
- [x] 3. Typography và bố cục các trang chính được thu gọn kích thước cân đối, không bị tràn viền (overflow).
- [x] 4. Tài liệu `HUONG_DAN_TRIEN_KHAI_IMMERSIVE_FRONTEND.md` được xuất bản đầy đủ, chi tiết, sẵn sàng cho bạn cùng nhóm tiếp tục phát triển.
- [x] 5. Toàn bộ mã nguồn biên dịch Android đạt **0 Warning, 0 Error**, 363/363 bài kiểm thử Unit Tests tiếp tục PASSED 100%.

