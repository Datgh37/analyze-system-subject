# BIÊN BẢN PHIÊN HỌP HỘI ĐỒNG ĐA TÁC TỬ: GIẢI PHÁP HẠ TẦNG BACKEND & CHIẾN LƯỢC KẾT NỐI CHO BẢN PHÁT HÀNH APK ANDROID
**Mã tài liệu:** `PLAN-56` | **Dự án:** Smart Personal Expense & Financial Planning (.NET MAUI 10 & ASP.NET Core 10 Web API)  
**Thời gian:** 2026-10-27 15:00  
**Chủ tọa phiên họp:** Agent 00 - Council Orchestrator  
**Thành phần Hội đồng tham luận:**
- **Agent 07:** Dr. Werner Vogels (CTO Amazon - Chuyên gia Hạ tầng Điện toán đám mây & DevOps)
- **Agent 10:** Dan Abramov (Lead Frontend Architect & Client State Specialist)
- **Agent 11:** Bruce Schneier (Chuyên gia An ninh Mạng & Mật mã học Fintech)
- **Agent 04:** Eric Evans (Domain-Driven Design & Clean Architecture Lead)
- **Agent 05:** Martin Kleppmann (Universal Database Architect - PostgreSQL & Offline Data Sync)
- **Agent 03:** Alistair Cockburn (Lead Business Analyst & User Experience Continuity)

---

## I. BỐI CẢNH VÀ BÀI TOÁN CỐT LÕI (PROBLEM STATEMENT)

### 1. Hiện trạng hệ thống:
- Hệ thống Full-Stack gồm phân hệ Backend ASP.NET Core 10 Web API kết nối CSDL PostgreSQL 16 qua Docker, hiện đang chạy hoàn toàn trên môi trường Localhost (`http://localhost:5018` trên máy dev, ánh xạ `http://10.0.2.2:5018` trên Android Emulator).
- Dự án sắp bước vào **Chặng đóng gói file cài đặt APK (`ExpenseTracker.apk`)** để nộp bài và cài trực tiếp lên điện thoại Android của Giảng viên và Hội đồng chấm thi.

### 2. Xung đột hạ tầng thực tế (The Deployment Paradox):
1. **Rào cản mạng Localhost & LAN:**
   - Khi cài file APK lên điện thoại thật của Giảng viên, địa chỉ `localhost` hay `10.0.2.2` sẽ trỏ vào chính chiếc điện thoại. Điện thoại không thể tự tìm thấy máy tính của sinh viên nếu không cùng dải mạng Wi-Fi LAN.
   - Thậm chí khi cùng Wi-Fi ở trường, hệ thống mạng Wi-Fi công cộng của đại học thường bật tính năng **AP Isolation (Client Isolation)**, ngăn chặn các thiết bị kết nối trực tiếp với nhau qua IP nội bộ.
2. **Tính khả dụng của Máy trạm cá nhân (Local Host Availability):**
   - Sinh viên không thể bật máy tính cá nhân liên tục 24/7 để làm máy chủ Backend. Giảng viên có thể cài đặt và chấm bài vào lúc nửa đêm, lúc di chuyển hoặc ở bất kỳ thời điểm nào máy tính sinh viên đang tắt.
3. **Rủi ro trải nghiệm người dùng (UX Failure Risk):**
   - Nếu bản APK phát hành mặc định trỏ vào Backend Web API mà máy chủ Backend đang tắt hoặc không kết nối được, ứng dụng sẽ bị treo (freeze), văng lỗi ngoại lệ `SocketException: Connection Refused`, hoặc hiện màn hình trắng. Giảng viên không thể duyệt qua 20 màn hình giao diện và nhóm sẽ bị trừ điểm nặng nề.
4. **Yêu cầu bảo vệ đề tài (Academic Defense Requirement):**
   - Nếu ứng dụng chỉ dùng 100% Mock Data thì Giảng viên có thể nghi ngờ sinh viên chưa hiện thực hóa Backend Web API ASP.NET Core 10 và CSDL 3NF ACID.

---

## II. THAM LUẬN CHUYÊN MÔN CỦA CÁC THÀNH VIÊN HỘI ĐỒNG

### 1. Tham luận từ Werner Vogels (CTO Amazon - DevOps & Hạ tầng):
> *"Everything fails all the time. Một ứng dụng di động chuẩn Enterprise không bao giờ được phép đặt giả định rằng máy chủ luôn luôn online. Chúng ta cần kết hợp giữa giải pháp Hạ tầng linh hoạt và Chiến lược Khả năng phục hồi (Resilience)."*

Werner đề xuất 3 cấp độ hạ tầng mạng:
- **Cấp độ 1 (Cloudflare Tunnel - Nhanh, Không mở Port, HTTPS Tự Động):**
  Sử dụng tiện ích `cloudflared` tạo đường hầm HTTPS an toàn trỏ thẳng vào `http://localhost:5018` của máy tính.
  *Lệnh:* `cloudflared tunnel --url http://localhost:5018`.
  *Ưu điểm:* Cung cấp URL HTTPS công khai (ví dụ `https://expensetracker-api.trycloudflare.com`), vượt qua 100% rào cản firewall, không cần router port forwarding. Dùng khi sinh viên đang trực tiếp thuyết trình trước lớp.
- **Cấp độ 2 (Deploy Cloud PaaS 24/7 Miễn Phí):**
  Triển khai Backend lên Render.com / Railway và CSDL PostgreSQL lên Supabase / Neon Cloud (Free Tier).
  *Ưu điểm:* Backend sống 24/7, Giảng viên cài APK ở nhà bất cứ lúc nào cũng gọi được API thật.
  *Thách thức:* Cần cấu hình biến môi trường kết nối cloud và tài khoản miễn phí có thể cần vài chục giây khởi động (Cold Start).

### 2. Tham luận từ Dan Abramov (Client Architecture & Offline Resilience):
> *"Đừng để số phận điểm số của bạn phụ thuộc vào một kết nối mạng Wi-Fi chập chờn trên giảng đường. UI phải 'Indestructible' (Không thể bị đánh sập)."*

Dan đề xuất kiến trúc **"Dual-Engine Smart Client" (Bộ chuyển mạch thông minh tại Client)**:
- **Giải pháp 1: Data Source Toggle & Dynamic Base URL trong Cài đặt (Settings):**
  Trong màn hình Cài đặt (`SettingsPage`) hoặc ngay tại màn hình Đăng nhập (`LoginPage`), cung cấp một khu vực cấu hình tiện ích:
  - Cho phép người dùng chuyển đổi giữa 2 chế độ:
    + 🟢 **Chế độ 1: Trải Nghiệm Ngoại Tuyến (Offline Mock Mode - Mặc định khi chấm bài độc lập):** Toàn bộ 20 màn hình hoạt động mượt mà 0ms, đầy đủ dữ liệu phong phú không lo rớt mạng.
    + 🔵 **Chế độ 2: Kết Nối Máy Chủ API (Live Backend Mode):** Cho phép nhập URL Backend tùy ý (ví dụ `https://expensetracker-api.trycloudflare.com` hoặc `http://192.168.1.15:5018`).
- **Giải pháp 2: Smart Auto-Fallback (Tự động thích ứng thông minh):**
  Khi ở chế độ Live Backend, nếu app thực hiện kiểm tra Health Check (`/api/health`) thất bại sau 2 giây timeout, app sẽ **tự động fallback sang Mock Data** và hiển thị một thanh thông báo tinh tế (Snack bar): *"Không thể kết nối Backend. Đã chuyển sang chế độ Demo Ngoại Tuyến để đảm bảo trải nghiệm"*.

### 3. Tham luận từ Bruce Schneier (Fintech Security):
> *"Cảnh báo nghiêm khắc: Tuyệt đối không sử dụng giải pháp mở Port Forwarding hay DMZ trên modem Wi-Fi gia đình."*

- **Bảo mật mạng cá nhân:** Mở port modem nhà riêng sẽ biến IP mạng nhà thành mục tiêu quét botnet tự động.
- **Android Network Security Cleartext:** Hệ điều hành Android từ Android 9 (API 28) trở lên mặc định **CHẶN HOÀN TOÀN các kết nối HTTP không mã hóa**. Nếu sinh viên cấu hình URL dạng `http://...` từ bên ngoài, Android OS sẽ tự ngắt kết nối trừ khi có cấu hình `android:usesCleartextTraffic="true"` hoặc sử dụng HTTPS (như qua Cloudflare Tunnel).
- Cần đảm bảo file `AndroidManifest.xml` duy trì cờ `usesCleartextTraffic="true"` cho mục đích đồ án môn học, và Backend WebApi cấu hình CORS cho phép `AllowAnyOrigin()` để mọi thiết bị gọi vào không bị chặn.

### 4. Tham luận từ Eric Evans (DDD & Clean Architecture):
- Kiến trúc MAUI của chúng ta hiện tại đã phân tách hoàn hảo qua các Interface trong `Repositories/` (`IWalletRepository`, `ITransactionRepository`...).
- Hiện tại `MauiProgram.cs` đang switch bằng tay giữa `Mock*` và `Api*`.
- Để chuyển đổi động trong runtime mà không cần recompile code, ta có thể xây dựng một **`HybridRepositoryProxy`** hoặc một cơ chế Resolver cho phép chuyển mạch nguồn dữ liệu dựa trên giá trị lưu trong `Preferences` hoặc `SecureStorage`.

### 5. Tham luận từ Alistair Cockburn (Lead BA):
- Đối tượng sử dụng bản APK là **Giảng viên chấm thi**:
  - Tình huống A: Giảng viên mở file APK ra xem lướt qua ở nhà để đánh giá giao diện, độ mượt, tính năng (Chiếm 80% trường hợp). Trường hợp này app PHẢI CHẠY NGAY LẬP TỨC với dữ liệu mẫu đẹp mắt.
  - Tình huống B: Giảng viên gọi nhóm lên bàn máy tính để đối chất, yêu cầu chứng minh Backend có ghi nhận dữ liệu thật vào PostgreSQL hay không (Chiếm 20% trường hợp). Lúc này sinh viên mở laptop, bật `RUN_WINDOWS.bat` hoặc bật Cloudflare Tunnel, trên app điện thoại gạt sang "Live Backend", thêm 1 giao dịch và chỉ cho Giảng viên thấy dòng dữ liệu xuất hiện trong Database/Swagger UI.

---

## III. TỔNG HỢP CÁC PHƯƠNG ÁN HÀNH ĐỘNG CHI TIẾT

Hội đồng đề xuất **4 Phương Án Chiến Lược** để Chủ tọa và Người dùng đưa ra lựa chọn tối ưu:

| Tiêu Chí So Sánh | Phương Án 1: Offline Mock Mặc Định + Hướng Dẫn Switch Khi Demo | Phương Án 2: Dynamic Server URL Switcher Trong App (Settings) | Phương Án 3: Tích Hợp Cloudflare Tunnel Miễn Phí (Live Demo) | Phương Án 4: Giải Pháp Toàn Diện (Kiềng 3 Chân: Đề Xuất Khuyên Dùng) |
| :--- | :--- | :--- | :--- | :--- |
| **Cách Thức Hoạt Động** | Bản APK đóng gói mặc định bật Mock Data. Khi Giảng viên cài đặt mở lên là chạy ngay 100%. Khi demo trực tiếp, sinh viên hướng dẫn Giảng viên hoặc tự đổi code/rebuild. | Bổ sung màn hình Cài đặt Máy Chủ cho phép Giảng viên/Sinh viên gõ URL Backend hoặc bấm nút gạt "Mock Mode / Real API". | Cung cấp file `RUN_TUNNEL.bat` trong thư mục project. Khi sinh viên bật máy, một URL HTTPS toàn cầu được tạo ra. | **Kết hợp cả 3:**<br>1. Mặc định APK mở lên là **Offline Mock Mode (An toàn 100%)**.<br>2. Thêm nút gạt **"Chế độ Nối Mạng Backend"** và ô nhập Server URL linh hoạt trong App.<br>3. Tạo sẵn script `RUN_TUNNEL.bat` để mở kết nối ra Internet khi Giảng viên muốn kiểm tra API thật! |
| **Độ An Toàn Điểm Số** | Rất cao (Không bao giờ văng app) | Rất cao | Phụ thuộc vào việc sinh viên có đang mở máy hay không | **Tuyệt đối 10/10** (Vừa không bao giờ văng app, vừa chứng minh được Backend thật 100%) |
| **Khối Lượng Triển Khai** | 0 giờ (Đã có sẵn) | ~30-45 phút cấu hình UI & Service Switcher | ~15 phút viết script `RUN_TUNNEL.bat` | ~45-60 phút |
| **Trải Nghiệm Giảng Viên** | Trải nghiệm mượt mà, nhưng cần giải thích phần Backend | Chủ động kiểm tra cả 2 chế độ | Thấy API thật kết nối từ xa | Giảng viên có thể tự do xem giao diện ngoại tuyến HOẶC kết nối trực tiếp vào máy chủ của nhóm. |

---

## IV. NGHỊ QUYẾT HỘI ĐỒNG & BƯỚC THỰC THI TIẾP THEO

1. Hội đồng khuyến nghị áp dụng **Phương án 4 (Kiềng 3 Chân Toàn Diện)**:
   - **Chân 1 (An toàn tuyệt đối cho file APK):** Cấu hình ứng dụng mặc định tự nạp Mock Data phong phú, đảm bảo Giảng viên cài APK độc lập lên điện thoại là trải nghiệm trơn tru toàn bộ 20 màn hình mà không bao giờ bị lỗi kết nối mạng.
   - **Chân 2 (Linh hoạt cấu hình Backend trong Runtime):** Xây dựng một Popup hoặc mục nhỏ tại màn hình Đăng nhập / Cài đặt cho phép chuyển sang "Chế độ Trực tuyến (Live API)" và đổi URL Backend (cho phép nhập IP LAN hoặc URL Cloudflare Tunnel).
   - **Chân 3 (Hạ tầng demo trực tuyến không cần thuê VPS):** Tích hợp script `RUN_TUNNEL.bat` (sử dụng Cloudflare Tunnel Quick Tunnel miễn phí) để khi Giảng viên yêu cầu kiểm tra Backend từ xa, sinh viên chỉ cần bấm 1 click là có ngay đường dẫn HTTPS an toàn toàn cầu.
2. Trình Chủ tọa và Người dùng phê duyệt trước khi chuyển sang **Chặng 4 Mới: Đóng gói bản cài đặt APK Android Release**.
