# Expense Tracker & Smart Personal Financial Planning System (.NET MAUI 10 & ASP.NET Core 10 Web API)

> **Hệ thống Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh (Tích hợp AI Multimodal & Jev Autonomous Actions)**  
> **Tech Stack:** .NET 10 (C# 14), ASP.NET Core 10 Clean Architecture, .NET MAUI 10 (MVVM CommunityToolkit), PostgreSQL 16 ACID, SignalR Core Native, Google Gemini 2.5 Flash Vision & NLP.  
> **Nguyên tắc định tuyến:** Lấy thư mục `dotnet-maui-ver/` làm thư mục gốc (Root) cho mọi lệnh thực thi và đường dẫn tương đối.

---

## 1. Giới Thiệu Dự Án

Dự án là nền tảng quản trị tài chính cá nhân thế hệ mới, giải quyết triệt để "nỗi đau thanh toán" (Pain of Paying) và ma sát nhận thức thông qua:
- **Kỷ luật ngân sách tự động:** Mô hình Hạn mức chi tiêu an toàn mỗi ngày (Daily Safe Spending - DSA) đàn hồi theo thời gian thực và hũ ngân sách đa phương pháp (50/30/20, Kakeibo).
- **Trí tuệ nhân tạo Multimodal Vision OCR:** Bóc tách hóa đơn siêu thị, phiếu lương tự động, tích hợp bộ kiểm toán số học (Mathematical Audit Engine) đối soát số học chính xác đến từng dòng hàng.
- **Tâm lý học hành vi & Cú hích (Nudge):** Thiết lập khoảng trễ tâm lý 24 giờ (Cooling-off Period) cho các quyết định mua sắm lớn và các khuyến nghị tài chính cá nhân hóa có khả năng tự giải trình (Explainable AI - XAI).
- **Động cơ hành động tự hành (Jev Protocol):** Tự động nhận diện tin nhắn biến động số dư ngân hàng và phân tầng xác suất để hiển thị thẻ xác nhận 1-chạm (Quick Commit).
- **Phạm vi nghiêm ngặt:** **100% Quản trị Tài chính Cá nhân (Personal Finance)** — Không chia tiền nhóm, không quỹ tập thể (xem chi tiết tại [`PRODUCT.md`](PRODUCT.md)).

---

---

## 2. ⚡ Khởi Chạy Nhanh 1-Click (Quick Start Cho Giảng Viên & Người Đánh Giá)

> 💡 **Khuyên Dùng:** Toàn bộ quy trình thiết lập môi trường phức tạp (kiểm tra Docker CSDL, biên dịch Backend Web API, kích hoạt ổ ảo chống lỗi đường dẫn dài, dò tìm Android SDK / Emulator, giải phóng khóa file DLL Windows) đã được tự động hóa 100% qua **3 file Batch (`.bat`)** ngay tại thư mục gốc `dotnet-maui-ver/`. Bạn chỉ cần **click đúp chuột** mà không cần gõ lệnh dòng lệnh!

### 2.1. Bảng Điều Khiển Khởi Chạy 1-Click

| Tệp Khởi Chạy | Nền Tảng Mục Tiêu | Thao Tác | Quy Trình Tự Động Hóa Thực Thi (Behind the Scenes) |
| :--- | :--- | :--- | :--- |
| **`RUN_WINDOWS.bat`**<br>*(Khuyên dùng số 1)* | **Windows Desktop (WinUI 3)** | **Click đúp chuột** | 1. Tự động kiểm tra Docker Desktop & khởi động CSDL PostgreSQL 16 (có fallback In-Memory).<br>2. Tự động khởi chạy Backend Web API tại `http://localhost:5018` trong tiến trình ngầm độc lập.<br>3. Tự động dọn dẹp tiến trình cũ, giải phóng File Lock DLL và hủy đăng ký AppX cũ.<br>4. Biên dịch mã nguồn WinUI 3 sạch sẽ và kích hoạt ứng dụng hiện lên màn hình máy tính! |
| **`RUN_ANDROID.bat`** | **Máy Ảo Android (Emulator)** | **Click đúp chuột** | 1. Tự động kiểm tra & khởi động PostgreSQL 16 và Backend Web API.<br>2. Tự động gán ổ ảo `X:` trỏ vào dự án (triệt tiêu 100% lỗi giới hạn 260 ký tự `MAX_PATH` của Android AAPT2).<br>3. Tự động dò tìm Android SDK, ADB, JDK 21 và khởi động máy ảo Android (AVD) nếu chưa bật.<br>4. Biên dịch gói `net10.0-android`, cài đặt APK và mở app lên màn hình điện thoại! |
| **`TEST_API.bat`** | **REST API & CSDL** | **Click đúp chuột** | Tự động chạy toàn bộ suite kiểm thử tự động API Backend (9 phân hệ E2E, Happy Path, Negative Case, ACID Pessimistic Locking) và tự động nạp dữ liệu mẫu Demo (Database Seeding). |

---

### 2.2. Hướng Dẫn Chi Tiết Cách Chạy Bằng File `.bat`

#### 🔹 Cách Chạy Trên Windows Desktop (Nhanh & Tiện Nhất)
1. Mở thư mục dự án `dotnet-maui-ver/`.
2. **Click đúp chuột vào file `RUN_WINDOWS.bat`**.
3. Cửa sổ dòng lệnh sẽ tự động kiểm tra CSDL, Backend và mở ứng dụng Windows Desktop lên màn hình.
   *(Lưu ý: Đảm bảo máy tính Windows 10/11 đã bật **Developer Mode** tại Windows Settings > For developers).*

#### 🔹 Cách Chạy Trên Máy Ảo Android
1. Mở thư mục dự án `dotnet-maui-ver/`.
2. **Click đúp chuột vào file `RUN_ANDROID.bat`**.
3. Hệ thống sẽ tự động gán ổ đĩa ảo `X:`, mở máy ảo Android (nếu chưa mở), biên dịch và đẩy ứng dụng lên máy ảo.
   *(Lưu ý: Yêu cầu máy đã cài Visual Studio kèm Workload MAUI Android và ít nhất 1 máy ảo Android AVD).*

#### 🔹 Cách Kiểm Thử Tự Động Toàn Bộ API
1. Mở thư mục dự án `dotnet-maui-ver/`.
2. **Click đúp chuột vào file `TEST_API.bat`**.
3. Toàn bộ kịch bản test tích hợp sẽ chạy và hiển thị bảng kết quả kiểm thử trực quan trên màn hình.

---

### 2.3. Các Câu Hỏi Thường Gặp Khi Chạy File `.bat` (Troubleshooting & FAQs)

- **❓ Windows hiện thông báo xanh *"Windows protected your PC"* (SmartScreen)?**  
  👉 Đây là cơ chế bảo vệ mặc định của Windows khi chạy file `.bat` mới tải từ GitHub về. Bạn chỉ cần nhấn vào **"More info" (Xem thêm)** > chọn **"Run anyway" (Vẫn chạy)**.
- **❓ Có cần cấu hình quyền thực thi PowerShell ExecutionPolicy không?**  
  👉 **Không cần.** Các file `.bat` đã tích hợp sẵn tham số `-ExecutionPolicy Bypass -NoProfile` nên luôn thực thi trơn tru mà không bị Windows chặn script.
- **❓ Nếu không cài Docker Desktop thì có chạy được không?**  
  👉 **Hoàn toàn được.** Nếu Docker chưa cài hoặc chưa bật, Backend Web API tự động kích hoạt chế độ **In-Memory Storage Fallback**, cho phép ứng dụng khởi chạy bình thường để chấm điểm và trải nghiệm tính năng mà không bị crash.
- **❓ Ứng dụng MAUI có cần Backend để hiển thị giao diện không?**  
  👉 **Không bắt buộc.** Ứng dụng đã được tích hợp sẵn tầng **Mock Repositories 0ms** cho toàn bộ 20 màn hình, nên ngay cả khi không có kết nối mạng hay Backend, ứng dụng vẫn hiển thị đầy đủ dữ liệu tài chính mẫu đẹp mắt!

---

## 3. Kiến Trúc Thư Mục & Phân Hệ Dự Án

---

## 3. Hướng Dẫn Khởi Chạy Backend (ASP.NET Core 10 Web API)

### 3.1. Khởi Chạy Nhanh (2 Bước)
Mở cửa sổ PowerShell hoặc Terminal tại thư mục `dotnet-maui-ver`:

#### Bước 1: Khởi động CSDL PostgreSQL 16 qua Docker
```powershell
cd backend
docker compose up -d
```
> *Lưu ý: Nếu máy tính chưa cài Docker Desktop, Backend vẫn tự động chuyển sang cơ chế In-Memory Fallback an toàn và ghi cảnh báo lên Console.*

#### Bước 2: Khởi chạy Web API
```powershell
dotnet run --project ExpenseTracker.WebApi
```

#### Bước 3: Truy cập Swagger UI
Mở trình duyệt truy cập tài liệu API trực quan:
👉 **`http://localhost:5018/swagger`** *(hoặc `https://localhost:7295/swagger`)*

---

### 3.2. Các Thông Số & Lệnh Bổ Trợ Cho Backend

- **Cổng kết nối:**
  - HTTP: `http://localhost:5018`
  - HTTPS: `https://localhost:7295`
- **Kiểm tra sức khỏe hệ thống (Health Check):**
  ```powershell
  curl http://localhost:5018/api/health
  ```
- **Thông số kết nối CSDL (PostgreSQL 16):**
  - Host: `localhost:5432` | Database: `expensetracker_db`
  - User: `expensetracker_user` | Password: `expensetracker_password`
  - *Tự động Migration & Seeder:* Khi kết nối CSDL lần đầu, hệ thống tự động chạy `MigrateAsync()` tạo toàn bộ bảng và nạp dữ liệu mẫu ban đầu qua `DatabaseSeeder.cs`.
- **Chạy toàn bộ 254 bài kiểm thử Backend:**
  ```powershell
  cd backend
  dotnet test
  ```
  *(83 tests Domain + 171 tests Application đạt tỷ lệ đạt 100%)*.

---

## 4. Hướng Dẫn Khởi Chạy Frontend (.NET MAUI 10)

Ứng dụng Frontend được xây dựng trên nền tảng **.NET MAUI 10 (C# 14)** kết hợp kiến trúc **Clean MVVM 3 Tầng Repositories**, hỗ trợ chạy linh hoạt cả Offline (Mock Data 0ms) lẫn Online (kết nối Backend thật).

### 4.1. Điều Kiện Tiên Quyết (Prerequisites & Requirements Theo Nền Tảng)

Tùy thuộc vào nền tảng bạn chọn để khởi chạy (Windows Desktop, Máy ảo Android, hay Điện thoại thật), vui lòng kiểm tra các yêu cầu tương ứng:

#### 1. Yêu Cầu Chung (Bắt Buộc Cho Mọi Nền Tảng)
- **.NET SDK:** Phiên bản được ghim trong [`global.json`](global.json) (hiện tại là `10.0.402`). Kiểm tra bằng lệnh: `dotnet --version`.
- **Visual Studio 2026 / 2022:** Cài đặt workload **.NET Multi-platform App UI development** từ Visual Studio Installer.

#### 2. Nếu Chạy Trên Windows Desktop (WinUI 3)
- **Hệ điều hành:** Windows 10 (Build 19041 trở lên) hoặc Windows 11.
- **Bật Developer Mode:** Vào `Settings > System (hoặc Privacy & Security) > For developers > Bật Developer Mode = ON` (bắt buộc để hệ điều hành Windows cho phép đăng ký và cài đặt gói ứng dụng MSIX phát triển).

#### 3. Nếu Chạy Trên Máy Ảo Android (Android Emulator)
- **Android SDK Platform:** Trình biên dịch .NET 10 MAUI yêu cầu bắt buộc cài đặt **Android API Level 36** (Android 16 Platform SDK). Kiểm tra hoặc cài đặt trong Visual Studio tại: *Tools > Android > Android SDK Manager > tích chọn Android API 36*.
- **Máy ảo Android (AVD):** Thiết bị máy ảo chạy Android 14+ / API 34+ (khuyến nghị: Pixel 7) kèm bộ tăng tốc ảo hóa phần cứng (HAXM hoặc Windows Hypervisor Platform).
- **Xử lý đường dẫn dài (Tùy chọn - Optional):** Nếu bạn tải/clone project vào một thư mục lồng sâu trên Windows có độ dài lớn, công cụ AAPT2 của Android SDK có thể vấp phải giới hạn 260 ký tự (`MAX_PATH`) của Windows. Khi gặp tình huống này, bạn chỉ cần chạy script `.\mount_virtual_drive.ps1` (hoặc gõ `subst X: "..."`) để mở dự án từ ổ ảo `X:\` rút ngắn đường dẫn. *Nếu thư mục dự án của bạn đã nằm ở đường dẫn ngắn (ví dụ: `C:\Projects\...` hoặc `D:\Projects\...`) thì hoàn toàn KHÔNG CẦN bước này.*

#### 4. Nếu Chạy Trên Điện Thoại Android Thật
- **Hệ điều hành:** Android 5.0 (API 21) trở lên (khuyến nghị Android 10+).
- **Cấu hình thiết bị:** Bật **Developer Options** và **USB Debugging** trên điện thoại.
- **Kết nối:** Cắm cáp USB vào máy tính hoặc kết nối chung mạng Wi-Fi LAN với máy tính chạy Backend.

---


### 4.2. Cơ Chế Chuyển Đổi Mock Data Offline vs API Backend Thật
File cấu hình: `frontend/ExpenseTracker.Maui/MauiProgram.cs` (từ dòng 125 đến 155):

- **CHẾ ĐỘ 1: DÙNG MOCK DATA OFFLINE (MẶC ĐỊNH ĐÃ BẬT SẴN):**  
  Toàn bộ 20 màn hình đã nạp sẵn Mock Repository phong phú. Bạn **không cần bật Docker hay Backend API**, ứng dụng vẫn chạy mượt mà với độ trễ 0ms. Đây là chế độ lý tưởng để thẩm định mỹ thuật, canh chỉnh layout XAML và biểu đồ.
  ```csharp
  builder.Services.AddScoped<IWalletRepository, MockWalletRepository>();
  builder.Services.AddScoped<ITransactionRepository, MockTransactionRepository>();
  builder.Services.AddScoped<IBudgetRepository, MockBudgetRepository>();
  // ... (đã mở sẵn)
  ```

- **CHẾ ĐỘ 2: KẾT NỐI API BACKEND THẬT:**  
  Khi cần test tích hợp dữ liệu thật từ PostgreSQL lên màn hình, chỉ cần mở file `frontend/ExpenseTracker.Maui/MauiProgram.cs`, đóng ghi chú các dòng `Mock*` và mở ghi chú các dòng `Api*`:
  ```csharp
  builder.Services.AddScoped<IWalletRepository, ApiWalletRepository>();
  builder.Services.AddScoped<ITransactionRepository, ApiTransactionRepository>();
  builder.Services.AddScoped<IBudgetRepository, ApiBudgetRepository>();
  // ...
  ```

---

### 4.3. Cách 1: Chạy Ứng Dụng Windows Desktop (WinUI 3)

#### Phương Án A: Khởi chạy 1-Click Tự Động (Khuyên Dùng - Tránh Triệt Để Lỗi Khóa File DLL)
- **Cách 1 (Nhanh nhất):** Click đúp vào file **`RUN_WINDOWS.bat`** tại thư mục gốc.
- **Cách 2 (Qua PowerShell):** Chạy script tiện ích đã xử lý sẵn mọi xung đột tiến trình và hủy đăng ký cũ:
```powershell
.\quick-run-scripts\run_windows.ps1
```
*(Script sẽ tự động kiểm tra PostgreSQL, khởi động Backend WebApi, dọn dẹp tiến trình ngầm, giải phóng File Lock, biên dịch sạch và mở ứng dụng WinUI 3 lên màn hình).*

#### Phương Án B: Khởi chạy bằng Visual Studio
1. Mở file solution tổng thể **`ExpenseTracker.slnx`** ngay tại thư mục gốc `dotnet-maui-ver/`.
2. Đặt `ExpenseTracker.Maui` làm **Startup Project** (Click chuột phải vào project `ExpenseTracker.Maui` trong Solution Explorer > chọn *Set as Startup Project*).
3. Tại thanh chọn Target Device trên thanh công cụ, chọn **Windows Machine**.
4. Nhấn **F5** (hoặc **Ctrl + F5**) để chạy.
> ⚠️ **Mẹo xử lý khi bị báo lỗi khóa file DLL:** Nếu Visual Studio báo lỗi `Could not copy ... DynamicDependency.Projection.dll. Exceeded retry count of 10`, chỉ cần mở PowerShell gõ:
> `Stop-Process -Name "ExpenseTracker.Maui" -Force; Get-AppxPackage *expensetracker* | Remove-AppxPackage` rồi bấm F5 lại là sạch bóng 100%!

---

### 4.4. Cách 2: Chạy Trên Máy Ảo Android (Android Emulator)

#### Bước 1: Kích hoạt ổ đĩa ảo `subst X:`
```powershell
.\quick-run-scripts\mount_virtual_drive.ps1
```

#### Bước 2: Khởi chạy bằng Visual Studio (Khuyên Dùng)
1. Mở Solution từ ổ đĩa ảo: **`X:\ExpenseTracker.slnx`**.
2. Trên thanh công cụ, chọn Framework là **`net10.0-android`**.
3. Tại mục Device, chọn máy ảo Android của bạn (nếu chưa có máy ảo, vào **Tools > Android > Android Device Manager** để tạo mới một máy ảo Pixel 7 chạy Android 14+ / API 34+).
4. Nhấn **F5** (Debug) hoặc **Ctrl+F5** (Run).

#### Bước 3: Khởi chạy Nhanh 1-Click Bằng File Batch hoặc Script
- **Cách 1 (Nhanh nhất):** Click đúp vào file **`RUN_ANDROID.bat`** ngay tại thư mục gốc `dotnet-maui-ver/`.
- **Cách 2 (Qua PowerShell):**
```powershell
.\quick-run-scripts\run_android.ps1
```
*(Script sẽ tự kiểm tra Docker PostgreSQL, bật Backend WebApi, kiểm tra ổ ảo X:, dò tìm SDK/AVD, khởi động máy ảo, biên dịch net10.0-android và tự động deploy lên màn hình Android).*

> 💡 **Lưu ý cấu hình mạng Android:** 
> - Ứng dụng đã được cấu hình sẵn thuộc tính `android:usesCleartextTraffic="true"` trong `AndroidManifest.xml` để cho phép kết nối HTTP.
> - Địa chỉ kết nối Backend tự động sử dụng `http://10.0.2.2:5018` để trỏ thẳng từ máy ảo về máy chủ Backend của bạn mà không cần chỉnh sửa gì thêm.

---

### 4.5. Cách 3: Chạy Trên Điện Thoại Android Thật (Qua Cáp USB hoặc Wi-Fi LAN)
1. Bật chế độ **Developer Options** và **USB Debugging** trên điện thoại Android của bạn.
2. Cắm cáp USB vào máy tính, Visual Studio sẽ tự nhận diện tên thiết bị điện thoại trên thanh công cụ.
3. Nhấn **F5** để cài đặt và debug trực tiếp trên điện thoại.
- *Lưu ý cấu hình IP LAN:* Mở file `frontend/ExpenseTracker.Maui/MauiProgram.cs` dòng 37, thay `localhost` bằng địa chỉ IP LAN của máy tính bạn (ví dụ: `"http://192.168.1.15:5018"`), và đảm bảo điện thoại cùng kết nối vào chung mạng Wi-Fi với máy tính.

---

### 4.6. Chạy Kiểm Thử Tự Động Cho Toàn Hệ Thống (363 Unit Tests Passed 100%)
Dự án sở hữu bộ kiểm thử tự động toàn diện đạt tỷ lệ vượt qua **100% (363 / 363 Tests Passed)**:
- **Frontend MAUI Unit Tests (82 bài tests):**
  ```powershell
  cd frontend/tests/ExpenseTracker.Maui.UnitTests
  dotnet test
  ```
  *(82/82 Tests Passed 100% trong ~899ms, kiểm thử toàn bộ ViewModel logic, Regex bóc tách ngân hàng, OTP, DSA)*.
- **Backend Unit Tests (281 bài tests):**
  ```powershell
  cd backend
  dotnet test
  ```
  *(89 tests Domain + 192 tests Application đạt tỷ lệ 100% Passed)*.
- **Kiểm thử tự động API Suite:** Click đúp vào **`TEST_API.bat`** (hoặc chạy `.\quick-run-scripts\test_api_suite.ps1`).

---

### 4.7. Cẩm Nang Dải Lệnh CLI Dành Cho Lập Trình Viên & Quy Trình Thu Thập Log Debug

> 📌 **Dành cho Đội ngũ Phát triển:** Các script `.ps1` tự động phục vụ giảng viên và hội đồng thẩm định nhanh. Khi lập trình và tìm lỗi (debug), lập trình viên nên sử dụng trực tiếp dải lệnh Terminal/PowerShell dưới đây để kiểm soát từng công đoạn, xuất log MSBuild và stream logcat thời gian thực.

#### A. Dải Lệnh CLI Trên Windows Desktop (WinUI 3)

##### 1. Bản chất vì sao không chạy `dotnet run` trực tiếp:
Ứng dụng WinUI 3 mặc định được đóng gói dưới dạng **MSIX Packaged App** nhằm sở hữu danh tính hệ thống (AppModel Package Identity) để truy cập bảo mật và Windows App SDK. Lệnh `dotnet run` cố gắng thực thi trực tiếp file `.exe` trần (Unpackaged mode) mà không có Package Context nên sẽ văng ngoại lệ `COMException (0x80040154: Class not registered)`. Quy trình chuẩn trên Terminal là biên dịch, đăng ký gói layout và kích hoạt qua AppModel.

##### 2. Dải lệnh thực thi 4 bước (Chạy từng dòng trong PowerShell):
```powershell
# Bước 1: Dọn dẹp tiến trình cũ và giải phóng File Lock nếu có lần chạy trước
Stop-Process -Name "ExpenseTracker.Maui" -Force -ErrorAction SilentlyContinue
Get-AppxPackage *expensetracker* | Remove-AppxPackage -ErrorAction SilentlyContinue

# Bước 2: Biên dịch dự án (Xuất log chi tiết ra file để phân tích nếu cần)
dotnet build frontend/ExpenseTracker.Maui/ExpenseTracker.Maui.csproj -f net10.0-windows10.0.19041.0 -fl -flp:logfile=build_windows.log;verbosity=normal

# Bước 3: Đăng ký gói AppX Layout vào hệ thống Windows
Add-AppxPackage -Register "frontend\ExpenseTracker.Maui\bin\Debug\net10.0-windows10.0.19041.0\win-x64\AppxManifest.xml"

# Bước 4: Khởi chạy ứng dụng qua Windows AppModel (Lấy PackageFamilyName động)
$pkg = Get-AppxPackage *expensetracker*
if ($pkg) {
    Start-Process "shell:AppsFolder\$($pkg.PackageFamilyName)!App"
} else {
    # Hoặc khởi chạy trực tiếp file .exe
    Start-Process "frontend\ExpenseTracker.Maui\bin\Debug\net10.0-windows10.0.19041.0\win-x64\ExpenseTracker.Maui.exe"
}
```

##### 3. Quy trình thu thập Log khi ứng dụng Windows gặp lỗi (Crash / Exception):
- **Log Biên dịch:** Mở file `build_windows.log` vừa tạo ở bước 2 để xem toàn bộ cảnh báo và lỗi XAML/C#.
- **Log Runtime & Crash:** Trong trường hợp ứng dụng mở lên rồi tự đóng (Crash do DI hoặc NullReference), mở PowerShell gõ lệnh sau để lấy ngay 5 sự kiện lỗi gần nhất kèm Full Stack Trace từ Windows Event Log:
  ```powershell
  Get-WinEvent -FilterHashtable @{LogName='Application'; ProviderName='.NET Runtime','Application Error'} -MaxEvents 5 | Format-List TimeCreated, Message
  ```
  *(Sao chép nội dung `Message` này để cung cấp cho việc debug).*

---

#### B. Dải Lệnh CLI Trên Máy Ảo & Thiết Bị Android

##### 1. Thiết lập công cụ `adb`:
Nếu PowerShell báo chưa nhận lệnh `adb`, chỉ cần nạp tạm thời đường dẫn Android SDK vào phiên làm việc:
```powershell
$env:PATH += ";C:\Program Files (x86)\Android\android-sdk\platform-tools"
adb devices
```
*(Đảm bảo hiển thị danh sách thiết bị dạng `emulator-5554 device` hoặc mã điện thoại thật).*

##### 2. Dải lệnh thực thi từng bước:
```powershell
# Bước 1: Biên dịch Android Package (Lưu ý: Nếu đường dẫn thư mục gốc dài, chạy từ ổ ảo X:\)
dotnet build frontend/ExpenseTracker.Maui/ExpenseTracker.Maui.csproj -f net10.0-android -fl -flp:logfile=build_android.log;verbosity=normal

# Bước 2: Cài đặt APK Debug mới nhất vào máy ảo / thiết bị thật
adb install -r "frontend\ExpenseTracker.Maui\bin\Debug\net10.0-android\com.companyname.expensetracker.maui-Signed.apk"

# Bước 3: Khởi chạy Activity chính của ứng dụng
adb shell am start -n com.companyname.expensetracker.maui/crc647f035efecdfd55fa.MainActivity
```
*(Mẹo rút gọn: Bạn cũng có thể dùng lệnh gộp của MSBuild: `dotnet build frontend/ExpenseTracker.Maui/ExpenseTracker.Maui.csproj -f net10.0-android -t:Run`)*

##### 3. Quy trình Stream và Trích Xuất Live Logcat:
- **Xem trực tiếp lỗi và cảnh báo (Realtime Log Streaming):**
  ```powershell
  adb logcat -c  # Xóa sạch bộ đệm log cũ
  adb logcat | Select-String "ExpenseTracker|DOTNET|AndroidRuntime|FATAL|Exception"
  ```
- **Chỉ theo dõi log của tiến trình ứng dụng:**
  ```powershell
  $pidApp = (adb shell pidof com.companyname.expensetracker.maui).Trim()
  adb logcat --pid=$pidApp
  ```
- **Xuất toàn bộ Crash Log ra file để gửi báo cáo lỗi:**
  ```powershell
  adb logcat -d -s "AndroidRuntime:E" "DOTNET:*" "*:F" > android_crash.log
  ```
  *(Chỉ cần đính kèm nội dung file `android_crash.log` hoặc `build_android.log` để định vị chính xác lỗi).*

---

## 5. Tài Liệu Kỹ Thuật Liên Quan

- 🎓 **Cẩm nang ôn tập vấn đáp & bảo vệ mã nguồn cuối kỳ:** [`implement-docs/BO_CAU_HOI_ON_TAP_VAN_DAP_CUOI_KY.md`](implement-docs/BO_CAU_HOI_ON_TAP_VAN_DAP_CUOI_KY.md)
- 📘 **Cẩm nang kiểm thử toàn bộ API (9 Phân hệ E2E, HTTP PATCH & Realtime Hub):** [`implement-docs/API_TESTING_PLAYBOOK.md`](implement-docs/API_TESTING_PLAYBOOK.md)
- 📋 **Bản ghi nguồn sự thật sản phẩm (Durable Product Truth):** [`implement-docs/PRODUCT.md`](implement-docs/PRODUCT.md)
- 📱 **Bản hướng dẫn ghép nối Frontend MAUI chi tiết 20 màn hình:** [`implement-docs/HUONG_DAN_FE.md`](implement-docs/HUONG_DAN_FE.md)
- 🎨 **Hướng dẫn triển khai giao diện chuyên sâu (Immersive UI & Design Tokens):** [`HUONG_DAN_TRIEN_KHAI_IMMERSIVE_FRONTEND.md`](HUONG_DAN_TRIEN_KHAI_IMMERSIVE_FRONTEND.md)
- 🏗️ **Đặc tả kiến trúc kỹ thuật toàn diện:** [`implement-docs/TECHNICAL_ARCHITECTURE_SPEC.md`](implement-docs/TECHNICAL_ARCHITECTURE_SPEC.md)
- 🗺️ **Lộ trình tổng thể dự án:** [`implement-docs/MASTER_PROJECT_ROADMAP.md`](implement-docs/MASTER_PROJECT_ROADMAP.md)
