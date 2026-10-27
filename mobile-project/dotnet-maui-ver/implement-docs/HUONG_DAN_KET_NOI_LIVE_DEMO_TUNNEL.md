# CẨM NANG BÍ KÍP NỘI BỘ: KẾT NỐI LIVE BACKEND DEMO & BIỂU DIỄN THỰC CHIẾN TRƯỚC GIẢNG VIÊN

## I. HƯỚNG DẪN THIẾT LẬP CLOUDFLARE NAMED TUNNEL (URL CỐ ĐỊNH 100%)

Nếu bạn muốn có một URL cố định vĩnh viễn (không bao giờ đổi) từ tài khoản Cloudflare bạn đã có:

### Bước 1: Tạo Tunnel trên Cloudflare Dashboard
1. Đăng nhập vào [Cloudflare Zero Trust Dashboard](https://one.dash.cloudflare.com/).
2. Chọn menu bên trái: **Networks** > **Tunnels** > bấm nút **Add a tunnel**.
3. Chọn loại: **Cloudflared** > Bấm Next.
4. Đặt tên Tunnel: ví dụ `expensetracker-backend` > Bấm Save tunnel.

### Bước 2: Lấy Tunnel Token
1. Tại trang hướng dẫn cài đặt, nhìn vào ô hệ điều hành **Windows (64-bit)**.
2. Tìm chuỗi Token nằm sau tham số `--token` (dạng một chuỗi mã hóa dài: `eyJhIjoi...`).
3. Sao chép chuỗi Token này.

### Bước 3: Cấu hình Đường Dẫn Công Khai (Public Hostname)
- **Subdomain:** `expense-api`
- **Domain:** `hobbyshop-ecommerce.me`
- **Type:** `HTTP`
- **URL:** `localhost:5018`

👉 **Kể từ lúc này:** Địa chỉ API cố định vĩnh viễn của bạn là: **`https://expense-api.hobbyshop-ecommerce.me`**!

### Bước 4: Khởi chạy 1-Click (Đã nạp sẵn Token)
Bạn chỉ cần **click đúp vào file `RUN_TUNNEL.bat`** tại thư mục gốc `dotnet-maui-ver/`.  
Hệ thống đã nạp sẵn Token của bạn, sẽ tự động bật Backend và kích hoạt đường hầm `https://expense-api.hobbyshop-ecommerce.me` lên mạng toàn cầu ngay lập tức!

---

## II. KỊCH BẢN BIỂU DIỄN THỰC CHIẾN TRƯỚC GIẢNG VIÊN (LIVE DEMO JOURNEY)

### PHÂN CẢNH 1: GIẢNG VIÊN CHẤM ĐỘC LẬP KHI KHÔNG CÓ MÁY CHỦ
- **Tình huống:** Giảng viên cài file APK trên điện thoại ở nhà hoặc lúc máy tính sinh viên đang tắt.
- **Hành vi hệ thống:**
  1. Khi mở app: App thử ping Backend $\rightarrow$ Không thấy server $\rightarrow$ Retry 3 lần (cách nhau 15s).
  2. Sau 3 lần: App nảy thông báo nhẹ: *"Server offline, đang chuyển sang chế độ offline"*.
  3. App chuyển sang **Offline Mock Mode (0ms)**: Hiển thị đầy đủ 20 màn hình, đồ thị DSA, lịch sử giao dịch mẫu cực kỳ mượt mà.
  4. 👉 *Kết quả:* Không bao giờ bị văng app (crash), Giảng viên xem được toàn bộ giao diện và chức năng!

---

### PHÂN CẢNH 2: BIỂU DIỄN FULL-STACK KẾT NỐI API THẬT TRƯỚC BÀN GIẢNG VIÊN
- **Tình huống:** Giảng viên yêu cầu: *"Em chứng minh là app có gọi Backend thật chứ không phải hardcode data ảo?"*
- **Quy trình biểu diễn 4 bước "ăn điểm 10":**
  1. **Bước 1 (Mở máy trạm):** Bạn mở laptop, click đúp vào **`RUN_TUNNEL.bat`** (hoặc mở Hotspot 5G từ điện thoại cho laptop). Màn hình console hiện link HTTPS: `https://expense-api.hobbyshop-ecommerce.me`.
  2. **Bước 2 (Cấu hình trên điện thoại):** 
     - Mở app `ExpenseTracker` trên điện thoại.
     - Vào menu Sidebar > chọn **⚙️ Cài Đặt Máy Chủ & Demo**.
     - Nhập link HTTPS vào ô **Server URL** > Bấm **Lưu URL**.
     - Bấm nút **📡 Ping Server** $\rightarrow$ Hệ thống hiện chữ xanh: `🟢 Kết nối máy chủ thành công! (HTTP 200 OK)`.
     - Bật công tắc: **Chế độ Backend Web API Thật = ON**.
  3. **Bước 3 (Thao tác trên app):**
     - Mở màn hình **Chi Tiêu (ExpensesPage)**.
     - Bấm thêm 1 khoản chi: *"Mua trà sữa 55.000đ"*.
     - Bấm **Lưu giao dịch**.
  4. **Bước 4 (Đối soát trên màn hình laptop):**
     - Chỉ tay vào màn hình laptop: Mở cửa sổ Terminal Backend $\rightarrow$ Cả hội đồng thấy dòng log Serilog:
       `[POST] /api/transactions - Status 201 Created`
     - Mở Swagger UI (`http://localhost:5018/swagger`) hoặc giao diện Docker Database $\rightarrow$ Thấy ngay dòng dữ liệu mới vừa được thêm vào!
  5. 👉 *Kết quả:* Giảng viên bị thuyết phục 100% nhóm tự tay code toàn bộ Full-Stack từ Database 3NF, API Clean Architecture đến Mobile MAUI.

---

### PHÂN CẢNH 3: BIỂU DIỄN BÓC TÁCH THÔNG BÁO NGÂN HÀNG (BANK NOTIFICATION DETECTION)
- **Tình huống:** Giảng viên hỏi: *"Tính năng bắt thông báo ngân hàng khi chuyển khoản hoạt động thế nào?"*
- **Cách biểu diễn trực quan:**
  1. Vào menu **⚙️ Cài Đặt Máy Chủ & Demo**.
  2. Cuộn xuống mục **3. Kiểm Chứng Bóc Tách Thông Báo Ngân Hàng**.
  3. Bấm nút màu tím: **⚡ Giả Lập Biến Động MoMo (-75.000đ)**.
  4. Ngay lập tức:
     - Trên màn hình hiện thông báo đối soát Regex: *"Hệ thống đã nhận diện thông báo từ Ví MoMo: -75.000 VNĐ tại Highlands Coffee"*.
     - Đồng thời thanh thông báo của điện thoại nảy một **Heads-Up Notification**:  
       `💳 Phát hiện chi tiêu: -75.000 đ`  
       `[Ví MoMo] tại Highlands Coffee. Chạm để tự động lưu ngay!`
  5. Bấm vào thông báo đó $\rightarrow$ App tự động Deep-Link mở màn hình Chi tiêu và điền sẵn số tiền 75.000đ cùng nội dung Highlands Coffee!
  6. 👉 *Kết quả:* Chứng minh được toàn bộ pipeline Native Notification Engine mà không cần phải tốn tiền chuyển khoản thật!
