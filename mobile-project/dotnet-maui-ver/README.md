# 🚀 .NET MAUI ENTERPRISE CLIENT — HỆ THỐNG QUẢN LÝ CHI TIÊU & HOẠCH ĐỊNH TÀI CHÍNH CÁ NHÂN THÔNG MINH

> **Phân hệ:** Ứng dụng Di động Toàn diện Đa Nền tảng (Cross-Platform Enterprise Mobile App)  
> **Đề tài Khóa luận Tốt nghiệp & PTTKHTTT:** Hệ thống Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh (Tích hợp AI)  
> **Trường Đại học Công nghệ Đồng Nai (DNTU)**  
> **Kiến trúc:** Clean Architecture + Domain-Driven Design (DDD) + MVVM Pattern  
> **Phạm vi triển khai:** **HOÀN CHỈNH 100% (FULL SCOPE — KHÔNG GIỚI HẠN PHẠM VI)**

---

## 1. 🌟 Giới Thiệu & Phân Định Chiến Lược (Strategic Positioning)

Trong cơ cấu tổ chức mã nguồn của đồ án, thư mục `mobile-project/` được phân tách thành 2 phân hệ rõ ràng:

| Tiêu Chí So Sánh | 📱 Bản React Native (`react-native-ver/`) | ⚡ Bản .NET MAUI Này (`dotnet-maui-ver/`) |
| :--- | :--- | :--- |
| **Mục đích sử dụng** | **Đồ án môn học:** Lập trình Thiết bị Di động (10–15 tuần). | **Khóa luận Tốt nghiệp (KLTN) & PTTKHTTT:** Hệ thống hoàn chỉnh cấp doanh nghiệp. |
| **Phạm vi nghiệp vụ** | **Bản rút gọn (Scoped MVP):** 2 ví cơ bản, thu/chi nhanh, ngân sách tháng, WAMP PHP 21 APIs. | **Toàn diện 100% (Enterprise Full Scope):** Đa ví ACID, AI Multimodal Vision OCR, Tối ưu nợ Snowball/Avalanche, Ngân sách thích ứng DSA, Đồ thị dòng tiền DAG. |
| **Công nghệ Frontend** | React Native Expo SDK 54, React 19, JavaScript. | **.NET 9 MAUI (Multi-platform App UI), C# 13, XAML.** |
| **Hệ thống Backend** | WAMP Server (PHP PDO + MySQL). | **ASP.NET Core Web API (.NET 9, Clean Architecture, Entity Framework Core).** |
| **Hiệu năng & Nền tảng** | Mobile Hybrid (Android / iOS). | **Native Native Compilation:** Android 15+, iOS 18+, Windows 11 Desktop, macOS (Mac Catalyst). |
| **Độ an toàn kiểu dữ liệu** | Dynamic typing (JS). | **End-to-End Type Safety 100%:** Dùng chung Models / DTOs / Value Objects C# giữa Client và Server. |

---

## 2. ⚙️ Ngăn Xếp Công Nghệ Đỉnh Cao (.NET Ecosystem)

* **Nền tảng Cốt lõi (Core Framework):** **.NET 9 MAUI** (C# 13 / XAML) — Tối ưu hóa AOT (Ahead-Of-Time compilation) cho thời gian khởi động app dưới 500ms.
* **Mô hình Kiến trúc Giao diện:** **MVVM (Model-View-ViewModel)** kết hợp thư viện chính thức **`CommunityToolkit.Mvvm`** (Tận dụng Roslyn Source Generators: `[ObservableProperty]`, `[RelayCommand]`).
* **Quản lý Dữ liệu Ngoại tuyến (Offline-First):**
  - **SQLite-net-pcl** / **Entity Framework Core for SQLite** làm bộ đệm CSDL cục bộ trên thiết bị.
  - Cơ chế đồng bộ hóa dữ liệu hai chiều **Vector Clock Conflict Resolution** khi có mạng internet.
* **Giao tiếp Mạng & Khả năng Phục hồi (Network & Resilience):**
  - **Refit:** Thư viện REST Client hướng giao diện (Type-safe declarative HTTP API).
  - **Polly v8:** Chiến lược phục hồi mạng linh hoạt (Exponential Backoff, Circuit Breaker, Timeout, Cache).
* **Kết nối Thời Gian Thực (Realtime Full-Duplex):** **Microsoft.AspNetCore.SignalR.Client** (kết nối trực tiếp SignalR Hub hoặc WebSocket Gateway).
* **Bảo mật Cấp Ngân Hàng (Fintech Security):**
  - Mã hóa CSDL cục bộ **SQLCipher (AES-256)**.
  - Lưu trữ khóa và token trong **MAUI SecureStorage** (tích hợp phần cứng Android KeyStore & Apple Keychain).
  - Xác thực sinh trắc học **Biometric Authentication** (Vân tay, Face ID).

---

## 3. 🎯 Danh Mục Tính Năng Toàn Diện (Full Capabilities)

Khác với bản môn học, bản .NET MAUI này hiện thực hóa toàn bộ các bài toán nghiên cứu cốt lõi của đề tài PTTKHTTT:

### 3.1. Quản Trị Đa Ví & Giao Dịch Bất Biến (Core Wallets & Immutability)
- Quản lý đa tài khoản: Tiền mặt, Tài khoản Ngân hàng (Vietcombank, MB, Techcombank), Ví điện tử (Momo, ZaloPay), Thẻ tín dụng (Credit Card với chu kỳ sao kê và thời gian ân hạn miễn lãi 45 ngày).
- Sử dụng **Money Value Object bất biến** (Số tiền `decimal` chính xác tuyệt đối, tránh lỗi dấu phẩy động của float/double).

### 3.2. Bóc Tách Hóa Đơn Tự Động Bằng Multimodal Vision AI (AI OCR Pipeline)
- Tích hợp camera chụp hóa đơn siêu thị / nhà hàng đa món.
- Gọi pipeline AI Vision trích xuất danh sách món, đơn giá, VAT, giảm giá.
- **Thuật toán Hậu kiểm toán Số học (Mathematical Audit):** Tự động đối soát công thức:
  $$\sum \text{Tiền Món} + \text{VAT} - \text{Discount} = \text{Tổng Hóa Đơn}$$
- Đề xuất phân bổ chi tiêu vào đúng danh mục chỉ với 1 chạm xác nhận.

### 3.3. Tối Ưu Hóa Trả Nợ & Dự Báo Dòng Tiền (Debt & Cashflow Optimizer)
- Nhập danh mục các khoản nợ cá nhân (Số dư nợ, Lãi suất APR, Mức trả tối thiểu hàng tháng).
- Bộ giải thuật so sánh trực quan: **Debt Snowball** (trả nợ nhỏ trước để tạo cú hích tâm lý) vs. **Debt Avalanche** (trả nợ lãi suất cao trước để tiết kiệm tiền lãi tối đa).
- Tính toán chính xác **Ngày Tự Do Tài Chính (Debt-Free Date)** và số tiền lãi tiết kiệm được.

### 3.4. Hoạch Định Ngân Sách Thích Ứng & Cú Hích Hành Vi (Adaptive Budgeting & Nudges)
- Áp dụng các phương pháp ngân sách kinh điển: **50/30/20**, **Zero-Based Budgeting (ZBB)**, **Kakeibo 4 Cột Trụ**.
- **Hạn mức Chi tiêu An toàn Mỗi Ngày (Daily Safe Spending - DSA):** Tự động điều chỉnh hạn mức các ngày còn lại trong tháng dựa trên tốc độ tiêu tiền thực tế.
- **Cú hích Chi tiêu (Behavioral Nudge):** Đưa ra cảnh báo tâm lý học hành vi (Richard Thaler) khi người dùng có dấu hiệu vung tay quá trán trước ngày nhận lương.

---

## 4. 🗂️ Cấu Trúc Solution & Dự Án (.NET MAUI Architecture)

Cấu trúc dự án tuân thủ nghiêm ngặt **Clean Architecture** và **DDD**:

```text
mobile-project/dotnet-maui-ver/
├── README.md                              # Tài liệu kỹ thuật phân hệ .NET MAUI
├── ExpenseApp.sln                         # Solution tập tin .NET 9
│
└── src/
    ├── ExpenseApp.Core/                   # [Domain & Application Layer]
    │   ├── Entities/                      # Wallet, Transaction, Category, Budget, DebtPlan
    │   ├── ValueObjects/                  # Money, DateRange, Percentage, AuditLog
    │   ├── Enums/                         # TransactionType, WalletType, DebtStrategy
    │   ├── DTOs/                          # Request/Response Data Transfer Objects
    │   └── Interfaces/                    # IExpenseApiClient, ILocalDatabase, ISecureVault
    │
    ├── ExpenseApp.Infrastructure/        # [Data Access & External Services Layer]
    │   ├── Data/                          # SQLite LocalDbContext, Repositories, Migrations
    │   ├── Api/                           # Refit REST Clients, Polly Resilience Pipelines
    │   ├── Realtime/                      # SignalR Hub Client, Push Notification Listener
    │   └── Security/                      # AES-256 Storage Service, Biometric Auth Handler
    │
    └── ExpenseApp.Maui/                   # [Presentation Layer - MAUI Client UI]
        ├── Views/                         # Giao diện XAML Pages
        │   ├── DashboardPage.xaml         # Màn hình Tổng quan tài chính & Tài sản ròng
        │   ├── TransactionsPage.xaml      # Dòng thời gian giao dịch & Bộ lọc thông minh
        │   ├── AddTransactionPage.xaml    # Form nhập liệu bàn phím số & Phân loại AI
        │   ├── OcrScanPage.xaml           # Chụp ảnh hóa đơn & Hậu kiểm toán số học
        │   ├── BudgetPage.xaml            # Hạn mức tháng & Đồng hồ đo an toàn DSA
        │   ├── DebtOptimizerPage.xaml     # Bộ giải Snowball vs Avalanche
        │   └── ProfilePage.xaml           # Thiết lập tài khoản & Khóa sinh trắc học
        ├── ViewModels/                    # MVVM ViewModels (CommunityToolkit.Mvvm)
        │   ├── DashboardViewModel.cs
        │   ├── TransactionViewModel.cs
        │   ├── OcrScanViewModel.cs
        │   └── BudgetViewModel.cs
        ├── Controls/                      # Custom Controls (Biểu đồ SkiaSharp, Cards, BottomSheets)
        ├── Converters/                    # Value Converters cho XAML (CurrencyConverter, ColorConverter)
        ├── Resources/                     # AppIcon, Splash, Fonts (Plus Jakarta Sans), Styles
        ├── Platforms/                     # Code đặc thù nền tảng (Android, iOS, Windows, MacCatalyst)
        └── AppShell.xaml                  # Navigation Shell điều hướng hiện đại
```

---

## 5. 🛠️ Yêu Cầu Môi Trường & Hướng Dẫn Cài Đặt

### 5.1. Yêu cầu công cụ
1. **IDE:** [Visual Studio 2022](https://visualstudio.microsoft.com/) (Phiên bản 17.12 trở lên) hoặc **Visual Studio Code** với C# Dev Kit + .NET MAUI extension.
2. **Workload bắt buộc:** Trong Visual Studio Installer, tích chọn:  
   ✅ **.NET Multi-platform App UI development**
3. **.NET SDK:** .NET 9.0 SDK (`dotnet --version` $\ge$ 9.0.100).
4. **Android SDK:** Android API Level 34 (Android 14) hoặc 35 (Android 15).

### 5.2. Khởi tạo và biên dịch dự án
```bash
# Kiểm tra các workload MAUI đã cài đặt
dotnet workload list

# Khôi phục dependencies và build
dotnet restore ExpenseApp.sln

# Chạy trực tiếp trên máy ảo Android
dotnet build -t:Run -f net9.0-android

# Chạy trên Windows Desktop (để test logic siêu nhanh mà không cần máy ảo)
dotnet build -t:Run -f net9.0-windows10.0.19041.0
```

---

## 6. 🔗 Liên Kết Ngữ Cảnh Dự Án

* 📄 **Chiến lược tiến hóa công nghệ toàn diện:** [`PLAN-22`](../../project_context/plans/plans-mobile-app/2026-10-07_plan_22_tech_stack_evolution_and_migration_strategy.md)
* 📱 **Phiên bản môn học React Native (Scoped MVP):** [`../react-native-ver/README.md`](../react-native-ver/README.md)
* 📜 **Nhật ký quyết định kiến trúc:** [`decisions_and_changes_log.md`](../../project_context/decisions_and_changes_log.md) (Quyết định 82, 84 & 85)
