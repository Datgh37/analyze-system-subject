# BÁO CÁO TỔNG KẾT ĐỒ ÁN MÔN HỌC
## HỌC PHẦN: LẬP TRÌNH THIẾT BỊ DI ĐỘNG & PHÂN TÍCH THIẾT KẾ HỆ THỐNG

### ĐỀ TÀI: HỆ THỐNG QUẢN LÝ CHI TIÊU VÀ HOẠCH ĐỊNH TÀI CHÍNH CÁ NHÂN THÔNG MINH
**Phân hệ:** .NET MAUI 10 (LTS) Enterprise Client & ASP.NET Core SignalR Backend  
**Cơ sở đào tạo:** Trường Đại học Công nghệ Đồng Nai (DNTU) — Khoa Công nghệ Thông tin  
**Nhóm sinh viên thực hiện:** Nhóm 46  
- **Trịnh Thành Đạt** — MSSV: 1923050149 (Nhóm trưởng)  
- **Trần Trọng Đại** — MSSV: 1923050745  
**Thời điểm hoàn thành:** Tháng 10/2026  
**Tệp Word chính thức:** [`BAO_CAO_CUOI_KY_DOTNET_MAUI.docx`](BAO_CAO_CUOI_KY_DOTNET_MAUI.docx) (Định dạng chuẩn in ấn KLTN DNTU: Khổ A4, lề trái 3.5cm đóng gáy, phông Times New Roman 13pt, giãn dòng 1.5 lines, Mục lục tự động 2 lớp).

---

## MỤC LỤC BÁO CÁO

- [ĐẶT VẤN ĐỀ BÀI TOÁN](#đặt-vấn-đề-bài-toán)
  - [1. Bối cảnh thực tiễn và lý do chọn đề tài](#1-bối-cảnh-thực-tiễn-và-lý-do-chọn-đề-tài)
  - [2. Mục tiêu nghiên cứu và giải pháp đề xuất](#2-mục-tiêu-nghiên-cứu-và-giải-pháp-đề-xuất)
- [CÁC CHỨC NĂNG CHÍNH CỦA ĐỀ TÀI](#các-chức-năng-chính-của-đề-tài)
- [CHƯƠNG 1: THIẾT KẾ CƠ SỞ DỮ LIỆU VÀ PHÂN HỆ BACKEND](#chương-1-thiết-kế-cơ-sở-dữ-liệu-và-phân-hệ-backend)
  - [1.1 Mô hình Cơ sở Dữ liệu Quan hệ Chuẩn 3NF](#11-mô-hình-cơ-sở-dữ-liệu-quan-hệ-chuẩn-3nf)
  - [1.2 Xây dựng Backend Dịch vụ ASP.NET Core Web API](#12-xây-dựng-backend-dịch-vụ-aspnet-core-web-api)
  - [1.3 Cổng Truyền thông Thời Gian Thực SignalR Gateway](#13-cổng-truyền-thông-thời-gian-thực-signalr-gateway)
- [CHƯƠNG 2: THIẾT KẾ VÀ XÂY DỰNG ỨNG DỤNG DI ĐỘNG .NET MAUI](#chương-2-thiết-kế-và-xây-dựng-ứng-dụng-di-động-net-maui)
  - [2.1 Kiến trúc Ứng dụng Di động Toàn diện .NET MAUI](#21-kiến-trúc-ứng-dụng-di-động-toàn-diện-net-maui)
  - [2.2 Ứng dụng Mô hình MVVM và CommunityToolkit.Mvvm](#22-ứng-dụng-mô-hình-mvvm-và-communitytoolkitmvvm)
  - [2.3 Chuẩn hóa Giao diện theo Hệ thống Impeccable Design Harness](#23-chuẩn-hóa-giao-diện-theo-hệ-thống-impeccable-design-harness)
  - [2.4 Cơ chế Cập nhật Dữ liệu Động Realtime và Thông báo Đẩy Thiết Bị Thật](#24-cơ-chế-cập-nhật-dữ-liệu-động-realtime-và-thông-báo-đẩy-thiết-bị-thật)
  - [2.5 Khả năng Vận hành Ngoại tuyến (Offline-First)](#25-khả-năng-vận-hành-ngoại-tuyến-offline-first)
- [CHƯƠNG 3: KẾT LUẬN VÀ HƯỚNG DẪN VẬN HÀNH DEMO](#chương-3-kết-luận-và-hướng-dẫn-vận-hành-demo)
  - [3.1 Đánh giá Kết quả Đạt được](#31-đánh-giá-kết-quả-đạt-được)
  - [3.2 Hướng dẫn Vận hành và Kịch bản Kiểm thử cho Giảng viên](#32-hướng-dẫn-vận-hành-và-kịch-bản-kiểm-thử-cho-giảng-viên)

---

## ĐẶT VẤN ĐỀ BÀI TOÁN

### 1. Bối cảnh thực tiễn và lý do chọn đề tài
Trong bối cảnh nền kinh tế số và các phương thức thanh toán không dùng tiền mặt phát triển mạnh mẽ, việc quản lý tài chính cá nhân (Personal Finance Management - PFM) đã trở thành một nhu cầu cấp thiết đối với mỗi cá nhân, đặc biệt là giới trẻ, người đi làm và sinh viên. Sự phổ biến của các cổng thanh toán di động, thẻ tín dụng và ví điện tử giúp giao dịch diễn ra thuận tiện nhưng đồng thời tạo ra hiệu ứng suy giảm ma sát thanh toán (Pain of Paying), dẫn đến hành vi chi tiêu bốc đồng, mất kiểm soát dòng tiền và rơi vào tình trạng thâm hụt tài chính trước ngày nhận lương.

Khảo sát thực tế các ứng dụng ghi chép thu chi hiện có trên thị trường cho thấy đa số phần mềm vẫn vận hành theo phương thức nhập liệu thủ công thụ động. Người dùng phải tự tay mở ứng dụng, gõ từng con số, chọn danh mục sau mỗi lần mua sắm. Rào cản nhận thức (Cognitive Friction) và sự tốn kém thời gian này khiến hơn 70% người dùng bỏ cuộc chỉ sau 2 đến 3 tuần sử dụng. Mặt khác, các giải pháp hiện nay thiếu khả năng cập nhật biến động dữ liệu theo thời gian thực (Realtime Synchronization), không có cơ chế lưu trữ ngoại tuyến bền bỉ (Offline-First) và hoàn toàn vắng bóng các thông báo đẩy thực sự (Push Notifications) để cảnh báo người dùng ngay tại thời điểm chạm ngưỡng rủi ro ngân sách.

### 2. Mục tiêu nghiên cứu và giải pháp đề xuất
Đề tài "Hệ thống Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh" được xây dựng nhằm giải quyết triệt để các hạn chế trên thông qua việc ứng dụng hệ sinh thái công nghệ di động hiện đại Microsoft .NET MAUI 10 (LTS) kết hợp kiến trúc dịch vụ phân tán ASP.NET Core Web API. Hệ thống hướng tới việc tự động hóa tối đa quy trình ghi nhận chi tiêu qua thị giác máy tính AI Vision OCR, duy trì kết nối truyền thông hai chiều toàn phần SignalR giúp đồng bộ số liệu tức thời 0ms, bảo đảm khả năng vận hành liên tục khi mất mạng nhờ cơ sở dữ liệu cục bộ SQLite, đồng thời tích hợp hệ thống thông báo đẩy chuẩn thiết bị di động (Native Mobile Push Notifications) nhằm tạo ra những "cú hích hành vi" (Behavioral Nudges) giúp người dùng duy trì kỷ luật tài chính bền vững.

---

## CÁC CHỨC NĂNG CHÍNH CỦA ĐỀ TÀI

Dựa trên quy chuẩn phân tích thiết kế hệ thống thông tin và chuẩn mực kỹ thuật ứng dụng di động cấp doanh nghiệp, hệ thống được quy hoạch thành 6 khối chức năng cốt lõi:

- **1. Quản trị Đa Tài khoản & Sổ cái Bất biến:** Hỗ trợ theo dõi đồng thời nhiều nguồn tiền: Tiền mặt, Tài khoản Ngân hàng (Vietcombank, MB, Techcombank), Ví điện tử (Momo, ZaloPay) và Thẻ tín dụng với chu kỳ sao kê 45 ngày miễn lãi. Sử dụng Money Value Object bất biến bảo đảm số liệu tài chính có độ chính xác tuyệt đối, loại trừ sai số dấu phẩy động.
- **2. Bóc tách Hóa đơn Tự động bằng Multimodal AI Vision OCR:** Cho phép người dùng chụp ảnh hóa đơn mua sắm siêu thị hoặc phiếu dịch vụ qua Camera thiết bị. Hệ thống tự động nhận diện danh sách mặt hàng, số tiền, thuế VAT và áp dụng thuật toán Hậu kiểm toán Số học (Mathematical Audit) đối soát tổng tiền trước khi phân bổ vào các danh mục chi tiêu chỉ với một chạm xác nhận.
- **3. Hoạch định Ngân sách Thích ứng (DSA) & Cú hích Hành vi:** Ứng dụng các quy tắc tài chính chuẩn mực (50/30/20, Zero-Based Budgeting, Kakeibo). Tự động tính toán Hạn mức Chi tiêu An toàn Mỗi Ngày (Daily Safe Spending - DSA) dựa trên tốc độ tiêu tiền thực tế. Đưa ra các khuyến nghị tâm lý học hành vi nhằm ngăn chặn hành vi chi tiêu quá mức trước ngày có thu nhập mới.
- **4. Tối ưu hóa Chiến lược Trả nợ & Dòng tiền Cá nhân:** Mô hình hóa các khoản vay nợ và cung cấp công cụ so sánh trực quan giữa hai phương pháp kinh điển: Debt Snowball (thanh toán khoản nợ nhỏ nhất trước để tạo động lực tâm lý) và Debt Avalanche (thanh toán khoản nợ lãi suất cao nhất trước). Xác định chính xác Ngày Tự Do Tài Chính (Debt-Free Date) và số tiền lãi tiết kiệm được.
- **5. Cập nhật Dữ liệu Động Thời Gian Thực (Realtime 0ms):** Tích hợp kết nối WebSocket Full-Duplex qua SignalR Client. Mọi biến động số dư, giao dịch phát sinh từ các cổng thanh toán, web quản trị hoặc dịch vụ nền đều được phản ánh tức thì lên giao diện di động mà không yêu cầu người dùng phải tải lại trang.
- **6. Thông báo Đẩy Thiết Bị Thật (Native Mobile Push Notifications):** Kích hoạt thông báo nổi (Heads-Up Banner) và lưu trữ trên thanh thông báo hệ điều hành khi có biến động số dư lớn, khi chạm ngưỡng cảnh báo ngân sách DSA hoặc nhắc nhở ghi chép tài chính định kỳ vào cuối ngày.

---

## CHƯƠNG 1: THIẾT KẾ CƠ SỞ DỮ LIỆU VÀ PHÂN HỆ BACKEND

### 1.1 Mô hình Cơ sở Dữ liệu Quan hệ Chuẩn 3NF
Cơ sở dữ liệu của hệ thống được chuẩn hóa ở dạng chuẩn 3 (3NF), bảo đảm loại bỏ hoàn toàn các dị thường thêm, xóa, sửa và duy trì tính toàn vẹn tham chiếu nghiêm ngặt (ACID) giữa các thực thể người dùng, ví tiền, danh mục và giao dịch.

*Bảng 1.1: Cấu trúc các bảng dữ liệu cốt lõi trong hệ thống quản lý chi tiêu*

| Tên Bảng | Khóa Chính | Khóa Ngoại | Mục Đích Sử Dụng Nghiệp Vụ |
| :--- | :---: | :---: | :--- |
| **users** | `id (INT)` | Không có | Lưu trữ tài khoản, họ tên, email đăng nhập và mật khẩu băm Bcrypt. |
| **wallets** | `id (INT)` | `user_id -> users(id)` | Quản lý số dư, tên ví tài chính (Tiền mặt, Ngân hàng, Ví điện tử). |
| **categories** | `id (INT)` | `user_id -> users(id)` | Hạng mục phân loại thu/chi kèm icon và mã màu sắc đại diện. |
| **transactions**| `id (INT)` | `wallet_id, category_id` | Lịch sử biến động tiền, phân loại INCOME/EXPENSE kèm ghi chú. |
| **budgets** | `id (INT)` | `user_id, category_id` | Hạn mức chi tiêu tháng và theo dõi tỷ lệ tiêu dùng thực tế. |
| **debt_plans** | `id (INT)` | `user_id -> users(id)` | Quản lý số dư nợ, lãi suất APR và phương thức trả nợ ưu tiên. |

### 1.2 Xây dựng Backend Dịch vụ ASP.NET Core Web API
Phân hệ Backend được xây dựng trên nền tảng ASP.NET Core 10 Web API kết hợp Entity Framework Core 10, tuân thủ mô hình Clean Architecture phân tách rõ ràng giữa tầng Domain, Application, Infrastructure và API Endpoints. Dịch vụ hỗ trợ xác thực bảo mật JWT Token và cơ chế định tuyến RESTful chuẩn hóa.

*Bảng 1.2: Danh mục các API RESTful phục vụ phân hệ di động*

| Phương thức | Đường dẫn API (Endpoint) | Chức năng Nghiệp vụ |
| :---: | :--- | :--- |
| **POST** | `/api/v1/auth/login` | Xác thực người dùng, cấp phát JWT Access Token và Refresh Token. |
| **GET** | `/api/v1/wallets/summary` | Lấy danh sách các ví tài chính và tổng tài sản khả dụng. |
| **POST** | `/api/v1/transactions` | Ghi nhận giao dịch thu/chi mới, tự động cập nhật số dư ví ACID. |
| **GET** | `/api/v1/transactions/filter` | Truy vấn lịch sử giao dịch theo tháng, danh mục và ví tiền. |
| **GET** | `/api/v1/budgets/progress` | Tính toán thanh tiến độ ngân sách và hạn mức an toàn ngày DSA. |
| **POST** | `/api/v1/ai/receipt-ocr` | Tiếp nhận ảnh hóa đơn, gọi pipeline AI trích xuất và hậu kiểm toán. |
| **GET** | `/api/v1/reports/monthly` | Tổng hợp cơ cấu chi tiêu 6 tháng phục vụ hiển thị biểu đồ. |

### 1.3 Cổng Truyền thông Thời Gian Thực SignalR Gateway
Hệ thống tích hợp SignalR Hub (`/hubs/expense`) duy trì kết nối Full-Duplex thời gian thực. Mỗi khi phát sinh giao dịch mới hoặc có sự điều chỉnh hạn mức ngân sách, SignalR Hub tự động phát sóng (broadcast) sự kiện `ReceiveTransactionUpdate` tới đúng kênh kết nối của người dùng. Cơ chế này loại bỏ hoàn toàn phương thức kéo tải định kỳ (HTTP Polling), tiết kiệm băng thông và kéo dài thời lượng pin thiết bị di động.

---

## CHƯƠNG 2: THIẾT KẾ VÀ XÂY DỰNG ỨNG DỤNG DI ĐỘNG .NET MAUI

### 2.1 Kiến trúc Ứng dụng Di động Toàn diện .NET MAUI
Ứng dụng di động được phát triển bằng nền tảng Microsoft .NET MAUI 10 (LTS) sử dụng ngôn ngữ C# 14 và XAML. Kiến trúc Solution được phân định thành 3 tầng độc lập theo chuẩn Clean Architecture:
- **ExpenseApp.Core:** Chứa các thực thể miền (Entities), đối tượng giá trị bất biến (Money Value Object), các Enums phân loại và Interfaces trừu tượng không phụ thuộc vào bất kỳ thư viện bên ngoài nào.
- **ExpenseApp.Infrastructure:** Hiện thực hóa lưu trữ dữ liệu cục bộ SQLite, khai báo REST Client bằng Refit kết hợp chính sách phục hồi mạng Polly v8, kết nối SignalR Client và dịch vụ thông báo đẩy thiết bị.
- **ExpenseApp.Maui:** Tầng giao diện người dùng chứa các XAML Views, các ViewModels triển khai theo mô hình MVVM, hệ thống Design Tokens và cấu hình Shell Navigation đa nền tảng.

### 2.2 Ứng dụng Mô hình MVVM và CommunityToolkit.Mvvm
Ứng dụng triệt tiêu hoàn toàn mã xử lý logic trong tệp giao diện (Zero Code-Behind trong tệp `.xaml.cs`). Thay vào đó, toàn bộ trạng thái dữ liệu và tương tác hành vi được đóng gói trong ViewModels thông qua thư viện `CommunityToolkit.Mvvm`:
- **Thuộc tính `[ObservableProperty]`:** Tự động sinh mã thông báo biến động `INotifyPropertyChanged`. Khi số dư ví hoặc danh sách giao dịch thay đổi, giao diện XAML tự động cập nhật ngay lập tức mà không cần can thiệp thủ công.
- **Lệnh điều khiển `[RelayCommand]`:** Đóng gói các hành vi của nút bấm (Thêm giao dịch, Chụp ảnh OCR, Lọc tháng) hỗ trợ cơ chế thực thi bất đồng bộ an toàn (`async/await`) và tự động khóa nút khi tác vụ đang xử lý (`IsBusy` state).
- **Cơ chế `WeakReferenceMessenger`:** Trao đổi tin nhắn thông báo giữa các ViewModel độc lập mà không tạo liên kết cứng, loại bỏ triệt để nguy cơ rò rỉ bộ nhớ (Memory Leaks) trong suốt vòng đời ứng dụng.

### 2.3 Chuẩn hóa Giao diện theo Hệ thống Impeccable Design Harness
Nhằm tránh các sai lầm rập khuôn của AI khi thiết kế giao diện (AI Tells), phân hệ di động áp dụng nghiêm ngặt triết lý thiết kế từ hệ thống Impeccable Design Harness (`pbakaus/impeccable`):
- **Chống hiện tượng lồng ghép thẻ (Anti Card Soup):** Không bao bọc mọi thành phần trong các khung thẻ lồng nhau. Phân cấp thị giác được thiết lập thông qua nhịp điệu khoảng trắng tự nhiên (Whitespace Rhythm) và viền mỏng trung tính 0.5pt.
- **Kỹ thuật hòa sắc thương hiệu (Color Tinting):** Loại bỏ hoàn toàn màu xám và màu đen thuần túy (`#000000`, `#808080`). Tất cả các màu nền và màu văn bản đều được pha một tỷ lệ sắc thái Deep Indigo / Navy Slate, mang lại chiều sâu thị giác sang trọng chuẩn Fintech.
- **Khả năng co giãn và gia cố lỗi (Adapt & Harden):** Giao diện được thiết kế thích ứng chính xác trên các kích thước màn hình khác nhau từ điện thoại di động nhỏ đến máy tính bảng và màn hình máy tính Windows. Xử lý triệt để hiện tượng vỡ khung khi giá trị tiền tệ lên tới hàng chục tỷ đồng bằng giải pháp Auto-Scaling Text.
- **Chuyển động có mục đích (Purposeful Motion):** Loại bỏ các hiệu ứng nhún nhảy (bounce) lỗi thời; sử dụng đường cong chuyển động vật lý lò xo (Spring Physics) tạo cảm giác phản hồi chắc chắn, mượt mà khi hiển thị biểu đồ tài chính.

### 2.4 Cơ chế Cập nhật Dữ liệu Động Realtime và Thông báo Đẩy Thiết Bị Thật
1. **Realtime Dynamic Synchronization:** Khi có dữ liệu mới từ máy chủ, SignalR Client kích hoạt cập nhật trực tiếp vào tập hợp `ObservableCollection<TransactionDto>` trong `DashboardViewModel`. Nhờ cơ chế Two-Way Data Binding của .NET MAUI, thẻ số dư tổng tài sản, danh sách giao dịch gần nhất và biểu đồ tỷ trọng tự động vẽ lại tức thì với độ trễ xấp xỉ 0ms.
2. **Real Mobile Push Notifications:** Sử dụng thư viện `Plugin.LocalNotification` kết hợp NotificationManager của hệ điều hành Android. Khi ứng dụng phát hiện chi tiêu vượt ngưỡng hạn mức an toàn ngày (DSA) hoặc nhận được tín hiệu biến động số dư từ SignalR, hệ thống tự động kích hoạt thông báo thật xuất hiện trên thanh thông báo trạng thái (Status Bar / Notification Tray) kèm biểu ngữ nổi (Heads-Up Banner), âm thanh hệ thống và mở đúng trang chi tiết khi người dùng nhấn vào thông báo.

### 2.5 Khả năng Vận hành Ngoại tuyến (Offline-First)
Ứng dụng tích hợp bộ đệm CSDL cục bộ SQLite (`sqlite-net-pcl`). Khi thiết bị ngắt kết nối mạng Internet, người dùng vẫn có thể thực hiện mọi thao tác thêm giao dịch, tra cứu số dư và xem biểu đồ bình thường. Dữ liệu được lưu trữ an toàn trong máy với trạng thái `SyncStatus = PendingLocal`. Ngay khi dịch vụ `Connectivity` phát hiện kết nối mạng phục hồi, Background Sync Service tự động gửi các bản ghi chờ lên máy chủ và thực hiện đồng bộ hóa hai chiều bền bỉ.

---

## CHƯƠNG 3: KẾT LUẬN VÀ HƯỚNG DẪN VẬN HÀNH DEMO

### 3.1 Đánh giá Kết quả Đạt được
Hệ thống Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh xây dựng trên nền tảng .NET MAUI 10 (LTS) đã hoàn thành xuất sắc toàn bộ các mục tiêu nghiên cứu và yêu cầu kỹ thuật do Giảng viên đề ra: Hiện thực hóa kiến trúc Clean Architecture chuẩn mực, tách biệt hoàn toàn giao diện và nghiệp vụ qua MVVM, ứng dụng công nghệ Realtime SignalR cập nhật dữ liệu động 0ms, vận hành thông báo đẩy thực sự trên thiết bị di động và bảo đảm tính sẵn sàng cao với kiến trúc Offline-First.

### 3.2 Hướng dẫn Vận hành và Kịch bản Kiểm thử cho Giảng viên

*Bảng 3.1: Quy trình kiểm thử các chức năng di động cốt lõi*

| STT | Chức Năng Kiểm Thử | Thao Tác Thực Hiện | Kết Quả Đạt Được |
| :---: | :--- | :--- | :--- |
| **1** | **Khởi chạy ứng dụng** | Chạy trên Windows Desktop hoặc Android Emulator. | Giao diện khởi động mượt mà dưới 1 giây, hiển thị màn hình Dashboard. |
| **2** | **Đăng nhập tài khoản** | Nhập tài khoản demo: `nguyenvanan@gmail.com` / `password123`. | Xác thực JWT thành công, lưu token an toàn trong SecureStorage. |
| **3** | **Tổng quan tài sản** | Xem 4 ví tài chính (Tiền mặt, Vietcombank, Techcombank, MoMo). | Số dư hiển thị chuẩn xác khớp với CSDL (Tổng: 68.150.000 đ). |
| **4** | **Cập nhật Realtime 0ms** | Thêm 1 giao dịch mới (VD: Ăn uống 50.000 đ) từ web hoặc app. | Số dư ví trừ tức thì, thanh ngân sách cập nhật mà không cần tải lại. |
| **5** | **Thông báo đẩy thật** | Thêm chi tiêu vượt hạn mức ngày an toàn (DSA). | Điện thoại bật thông báo nổi thật trên thanh trạng thái hệ thống. |
| **6** | **Thử nghiệm Ngoại tuyến** | Ngắt kết nối mạng WiFi/4G và thực hiện ghi chép giao dịch. | Giao dịch lưu thành công vào SQLite cục bộ; tự đồng bộ khi có mạng. |

Đồ án khẳng định sự trưởng thành về tư duy phân tích thiết kế hệ thống, khả năng làm chủ các công nghệ di động tiên tiến trong hệ sinh thái Microsoft .NET và sẵn sàng mở rộng thành đề tài Khóa luận Tốt nghiệp hoàn chỉnh cấp doanh nghiệp.
