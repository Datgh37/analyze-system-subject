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

## 2. Kiến Trúc Thư Mục & Giải Pháp

```text
dotnet-maui-ver/ (Root)
├── ExpenseTracker.slnx                     # Solution Full-Stack tổng thể (Bao gồm cả Backend & Frontend MAUI)
│
├── backend/                                # Phân hệ Backend ASP.NET Core 10 (Clean Architecture)
│   ├── ExpenseTracker.slnx                 # Solution riêng cho Backend Web API & Test Suites
│   ├── Directory.Build.props               # Cấu hình C# 14, .NET 10, Nullable toàn cục
│   ├── docker-compose.yml                  # Container PostgreSQL 16 Alpine
│   ├── ExpenseTracker.Domain/              # Core Domain: Entities, Value Objects, Domain Events
│   ├── ExpenseTracker.Application/         # Use Case Services, DTOs, Business Validators
│   ├── ExpenseTracker.Infrastructure/      # EF Core, PostgreSQL Context, Security, AI Gemini Client
│   ├── ExpenseTracker.WebApi/              # Web API Controllers, SignalR Hub, Swagger UI
│   └── tests/
│       ├── ExpenseTracker.Domain.UnitTests/       # 83 bài Unit Tests nghiệp vụ lõi (100% Passed)
│       └── ExpenseTracker.Application.UnitTests/  # 171 bài Unit Tests Service Layer (100% Passed)
│
└── frontend/                               # Phân hệ Ứng dụng Di động & Desktop .NET MAUI 10
    ├── ExpenseTracker.Maui.slnx            # Solution riêng cho Frontend MAUI & Tests
    ├── ExpenseTracker.Maui/                # Dự án Clean MVVM .NET MAUI 10 (ExpenseTracker.Maui.csproj)
    │   ├── Views/                          # 20 Màn hình XAML (Wireframe / Data Anchors)
    │   ├── ViewModels/                     # ObservableObject & RelayCommands (CommunityToolkit.Mvvm)
    │   ├── Models/                         # Client DTOs & SQLite Local Models
    │   ├── Services/                       # Refit Declarative API Clients, SignalR Hub, SecureStorage
    │   ├── Repositories/                   # Tầng MVVM Repositories (Chuyển đổi 1-chạm giữa Mock & Api)
    │   ├── Resources/                      # Bảng màu, Fonts, Styles dùng chung
    │   └── Platforms/                      # Native Code (Android Service lắng nghe thông báo, Windows)
    └── tests/
        └── ExpenseTracker.Maui.UnitTests/  # 42 bài Unit Tests ViewModels (100% Passed)
```

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

Ứng dụng Frontend được thiết kế theo kiến trúc **Clean MVVM 3 Tầng Repositories**, hỗ trợ chạy linh hoạt cả Offline lẫn Online:

### 4.1. Cơ Chế Chuyển Đổi Mock Data Offline vs API Backend Thật
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

### 4.2. Cách 1: Chạy Ứng Dụng Windows Desktop (WinUI 3)
Ứng dụng .NET MAUI trên Windows được đóng gói dưới dạng **Packaged Windows App (MSIX)**:

#### Phương Án A: Khởi chạy bằng Visual Studio (Khuyên Dùng)
Bạn có thể mở Visual Studio theo 1 trong 2 cách chính xác sau:
- **Cách 1 (Khuyên dùng - Full-Stack):** Mở file solution tổng thể **`ExpenseTracker.slnx`** ngay tại thư mục gốc `dotnet-maui-ver/`. Khi đó Solution Explorer sẽ hiển thị đồng thời cả thư mục `/backend/` và `/frontend/`.
- **Cách 2 (Chỉ mở riêng Frontend):** Mở file **`frontend/ExpenseTracker.Maui.slnx`** (hoặc mở trực tiếp file project `frontend/ExpenseTracker.Maui/ExpenseTracker.Maui.csproj`).
*(Lưu ý: Không mở tệp `backend/ExpenseTracker.slnx` để chạy Frontend, vì tệp này nằm trong thư mục con backend chỉ cấu hình riêng cho các project Web API Backend)*.

**Các bước chạy trên Visual Studio:**
1. Đặt `ExpenseTracker.Maui` làm **Startup Project** (Click chuột phải vào project `ExpenseTracker.Maui` trong Solution Explorer > chọn *Set as Startup Project*).
2. Tại thanh chọn Target Device trên thanh công cụ, chọn **Windows Machine**.
3. Nhấn **F5** (hoặc **Ctrl + F5**) để Visual Studio tự động biên dịch, đăng ký gói MSIX và mở cửa sổ app lên màn hình.

#### Phương Án B: Khởi chạy bằng PowerShell / Terminal (1-Click Không Cần Visual Studio)
Từ thư mục gốc `dotnet-maui-ver`, chạy các lệnh sau:
```powershell
cd frontend/ExpenseTracker.Maui
dotnet build -f net10.0-windows10.0.19041.0
Add-AppxPackage -Register .\bin\Debug\net10.0-windows10.0.19041.0\win-x64\AppxManifest.xml
explorer.exe shell:AppsFolder\com.companyname.expensetracker.maui_9zz4h110yvjzm!App
```
*(Sau khi chạy lệnh trên, ứng dụng đã được cài đặt vào máy. Bạn có thể nhấn phím **Windows** trên bàn phím, gõ tìm **ExpenseTracker.Maui** trong Start Menu để mở bất cứ lúc nào)*.

---

### 4.3. Cách 2: Chạy Trên Máy Ảo Android (Android Emulator)
1. Mở Visual Studio, vào menu **Tools > Android > Android Device Manager** để tạo máy ảo (khuyến nghị: Pixel 7 hoặc thiết bị chạy Android 14+ / API 34+).
2. Tại thanh chọn Target Device, chọn máy ảo vừa tạo.
3. Nhấn **F5** (Debug) hoặc **Ctrl+F5** (Run).
- *Lưu ý về IP:* `MauiProgram.cs` đã tự động thiết lập Base URL là `http://10.0.2.2:5018` để máy ảo kết nối thẳng về máy tính chạy Backend mà không cần cấu hình thêm.

---

### 4.4. Cách 3: Chạy Trên Điện Thoại Android Thật (Qua Cáp USB hoặc Wi-Fi)
1. Bật chế độ **Developer Options** và **USB Debugging** trên điện thoại Android của bạn.
2. Cắm cáp USB vào máy tính, Visual Studio sẽ nhận diện tên thiết bị điện thoại của bạn trên thanh công cụ.
3. Nhấn **F5** để cài đặt và debug trực tiếp trên điện thoại.
- *Lưu ý cấu hình IP LAN:* Mở file `frontend/ExpenseTracker.Maui/MauiProgram.cs` dòng 37, thay `localhost` bằng địa chỉ IP LAN của máy tính bạn (ví dụ: `"http://192.168.1.15:5018"`), và đảm bảo điện thoại cùng kết nối vào chung mạng Wi-Fi với máy tính.

---

### 4.5. Chạy Kiểm Thử Tự Động Cho Frontend (42 ViewModel Tests)
Dự án có sẵn bộ 42 Unit Tests kiểm tra tính toàn vẹn của ViewModel và logic xử lý dữ liệu:
```powershell
cd frontend/tests/ExpenseTracker.Maui.UnitTests
dotnet test
```
*Kết quả:* **`42/42 Tests Passed 100%`**.

---

## 5. Tài Liệu Kỹ Thuật Liên Quan

- 📘 **Cẩm nang kiểm thử toàn bộ API (9 Phân hệ E2E, HTTP PATCH & Realtime Hub):** [`API_TESTING_PLAYBOOK.md`](API_TESTING_PLAYBOOK.md)
- 📋 **Bản ghi nguồn sự thật sản phẩm (Durable Product Truth):** [`PRODUCT.md`](PRODUCT.md)
- 📱 **Bản hướng dẫn ghép nối Frontend MAUI chi tiết 20 màn hình:** [`HUONG_DAN_FE.md`](HUONG_DAN_FE.md)
- 🏗️ **Đặc tả kiến trúc kỹ thuật toàn diện:** [`TECHNICAL_ARCHITECTURE_SPEC.md`](TECHNICAL_ARCHITECTURE_SPEC.md)
- 🗺️ **Lộ trình tổng thể dự án:** [`MASTER_PROJECT_ROADMAP.md`](MASTER_PROJECT_ROADMAP.md)
