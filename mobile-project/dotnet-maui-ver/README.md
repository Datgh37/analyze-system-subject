# Expense Tracker & Smart Personal Financial Planning System (.NET MAUI & ASP.NET Core Web API)

Hệ thống Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh (Tích hợp AI) - Dự án PTTKHTTT.

---

## 1. Cấu Trúc Tổng Thể Thư Mục

```text
mobile-project/dotnet-maui-ver/
├── backend/                                # Giải pháp Backend ASP.NET Core 10 (Clean Architecture)
│   ├── ExpenseTracker.slnx                 # Solution file mới (.NET 10 XML format)
│   ├── Directory.Build.props               # Cấu hình C# 14 / .NET 10, Nullable, Code Style toàn cục
│   ├── .editorconfig                       # Chuẩn format mã nguồn C#
│   ├── docker-compose.yml                  # Khởi chạy PostgreSQL 16 Alpine
│   ├── ExpenseTracker.Domain/              # Core Domain: Entities, Value Objects, Domain Events
│   ├── ExpenseTracker.Application/         # Service Interfaces, DTOs, Business Validators
│   ├── ExpenseTracker.Infrastructure/      # EF Core, PostgreSQL Context, Security, External APIs
│   ├── ExpenseTracker.WebApi/              # ASP.NET Core 10 Web API Controllers, SignalR Hub, Swagger
│   └── tests/
│       ├── ExpenseTracker.Domain.UnitTests/       # Unit tests nghiệp vụ lõi (xUnit, FluentAssertions)
│       └── ExpenseTracker.Application.UnitTests/  # Unit tests Service Layer (Moq)
│
└── frontend/                               # Giải pháp Ứng dụng Di động .NET MAUI 10
    └── ExpenseTracker.Maui/                # Dự án Clean MVVM .NET MAUI
        ├── Views/                          # XAML Pages và Data Templates
        ├── ViewModels/                     # ObservableObject & RelayCommand (CommunityToolkit.Mvvm)
        ├── Models/                         # SQLite Local Models & Client DTOs
        ├── Services/                       # Refit API Client, SQLite Service, SignalR Hub, SecureStorage
        ├── Converters/                     # Value Converters cho XAML Data Binding
        ├── Resources/                      # Styles, Fonts, Colors, App Icons, Splash Screen
        └── Platforms/                      # Native Code cho Android, iOS, Windows
```

---

## 2. Hướng Dẫn Khởi Chạy

### A. Khởi Chạy Cơ Sở Dữ Liệu (Docker)
```bash
cd backend
docker compose up -d
```
- PostgreSQL 16 chạy trên `localhost:5432`
- Database: `expensetracker_db`
- User: `expensetracker_user`
- Password: `expensetracker_password`

### B. Khởi Chạy Backend API
```bash
cd backend
dotnet restore
dotnet build
dotnet run --project ExpenseTracker.WebApi
```
- Swagger UI tài liệu API: `https://localhost:5001/swagger` hoặc `http://localhost:5000/swagger`
- Health check endpoint: `https://localhost:5001/api/health`

### C. Chạy Kiểm Thử Backend
```bash
cd backend
dotnet test
```

### D. Khởi Chạy Frontend .NET MAUI
```bash
cd frontend/ExpenseTracker.Maui
dotnet restore
dotnet build
dotnet run -f net10.0-windows10.0.19041.0
```
*(Để build cho Android: cấu hình `BuildingForAndroid=true` hoặc mở trong Visual Studio chọn thiết bị Android/Emulator).*
