# 🚀 KẾ HOẠCH THI CÔNG: GIAI ĐOẠN 0 — DỰNG BỘ KHUNG HẠ TẦNG (SOLUTION SCAFFOLDING & DEVOPS)
## (IMPLEMENTATION PLAN: STAGE 0 — CLEAN ARCHITECTURE & MAUI SCAFFOLDING)

> **Mã kế hoạch:** `PLAN-31`  
> **Thời điểm:** 22/10/2026  
> **Trạng thái:** `COMPLETED & VERIFIED (ĐÃ HOÀN TẤT & NGHIỆM THU XUẤT SẮC)`  
> **Chủ trì phiên họp:** Agent 00 (Council Orchestrator)  
> **Thành viên tham gia:** 
> - Agent 07: Werner Vogels (DevOps & Environment Setup)
> - Agent 04: Eric Evans (Clean Architecture Layering)
> - Agent 10: Dan Abramov (Client MAUI Scaffolding)
> - Agent 12: Kent Beck (Unit Test Setup & Quality Gate 0)
> **Tài liệu căn cứ:** [`MASTER_PROJECT_ROADMAP.md`](../../mobile-project/dotnet-maui-ver/MASTER_PROJECT_ROADMAP.md), [`TECHNICAL_ARCHITECTURE_SPEC.md`](../../mobile-project/dotnet-maui-ver/TECHNICAL_ARCHITECTURE_SPEC.md), [`05_engineering_architecture_and_code_craft_rules.md`](../../.agents/rules/05_engineering_architecture_and_code_craft_rules.md).

---

## 1. MỤC TIÊU GIAI ĐOẠN 0
1. Tạo lập cấu trúc thư mục chuẩn **`backend/`** và **`frontend/`** bên trong `mobile-project/dotnet-maui-ver/`.
2. Khởi tạo Solution và 4 projects Clean Architecture cho Backend (`Domain`, `Application`, `Infrastructure`, `WebApi`) và 2 projects Test xUnit.
3. Khởi tạo project di động .NET MAUI 10 cho Frontend (`ExpenseTracker.Maui`).
4. Thiết lập cấu hình toàn cục `Directory.Build.props` (bắt buộc `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`, `<Nullable>enable</Nullable>`) và `.editorconfig`.
5. Tạo tệp `docker-compose.yml` khởi chạy PostgreSQL 16 cục bộ.
6. Nghiệm thu **Quality Gate 0:** Build toàn bộ Solution đạt **0 Error, 0 Warning**, chạy thành công 1 bài test khói (Smoke Test).

---

## 2. DANH SÁCH BƯỚC THỰC THI CHI TIẾT (STEP-BY-STEP EXECUTION)

### Bước 1: Thiết Lập Thư Mục & Solution File
- Tạo thư mục:
  * `mobile-project/dotnet-maui-ver/backend/`
  * `mobile-project/dotnet-maui-ver/frontend/`
- Khởi tạo file Solution điều phối: `ExpenseTracker.slnx` tại thư mục `backend/`.

### Bước 2: Khởi Tạo 4 Projects Clean Architecture Cho Backend
Sử dụng .NET SDK 10.0 (`net10.0`):
1. **`ExpenseTracker.Domain`** (`dotnet new classlib`):
   - Không chứa bất kỳ dependency bên ngoài nào.
   - Thư mục: `Entities/`, `ValueObjects/`, `Enums/`, `Common/`.
2. **`ExpenseTracker.Application`** (`dotnet new classlib`):
   - Tham chiếu `ExpenseTracker.Domain`.
   - Packages: `FluentValidation (>= 11.9.0)`.
   - Thư mục: `Services/`, `Interfaces/`, `DTOs/`, `Validators/`.
3. **`ExpenseTracker.Infrastructure`** (`dotnet new classlib`):
   - Tham chiếu `ExpenseTracker.Application` và `ExpenseTracker.Domain`.
   - Packages: `Npgsql.EntityFrameworkCore.PostgreSQL`, `Microsoft.EntityFrameworkCore.Design`.
   - Thư mục: `Persistence/` (DbContext, Configurations, Migrations), `Services/` (MockOcr, SignalR Emitters).
4. **`ExpenseTracker.WebApi`** (`dotnet new webapi`):
   - Tham chiếu `ExpenseTracker.Infrastructure` và `ExpenseTracker.Application`.
   - Packages: `Microsoft.AspNetCore.OpenApi`, `Swashbuckle.AspNetCore`.
   - Cấu hình Program.cs với Dependency Injection, Middlewares, SignalR Hub Route.

### Bước 3: Khởi Tạo 2 Projects Test Backend (xUnit)
1. `tests/ExpenseTracker.Domain.UnitTests` (`dotnet new xunit`):
   - Tham chiếu `ExpenseTracker.Domain`.
   - Packages: `FluentAssertions`, `Moq`.
2. `tests/ExpenseTracker.Application.UnitTests` (`dotnet new xunit`):
   - Tham chiếu `ExpenseTracker.Application`.
   - Packages: `FluentAssertions`, `Moq`.

### Bước 4: Khởi Tạo Project Di Động .NET MAUI Cho Frontend
Tại thư mục `frontend/`:
1. Khởi tạo project `ExpenseTracker.Maui` (`dotnet new maui -n ExpenseTracker.Maui`).
2. Cài đặt các gói NuGet cốt lõi đã phê duyệt:
   - `CommunityToolkit.Mvvm` (MVVM Source Generators)
   - `Refit` & `Refit.HttpClientFactory` (REST Client khai báo)
   - `sqlite-net-pcl` & `SQLitePCLRaw.bundle_green` (SQLite nhúng)
   - `Microsoft.AspNetCore.SignalR.Client` (Realtime Dynamic Sync 0ms)
3. Cấu trúc thư mục: `Views/`, `ViewModels/`, `Models/`, `Services/`, `Converters/`, `Resources/`.

### Bước 5: Cấu Hình Toàn Cục & Docker CSDL
1. Tạo `Directory.Build.props` tại gốc `backend/`.
2. Tạo `.editorconfig` (chuẩn hóa 4 spaces, xóa using thừa).
3. Tạo `docker-compose.yml` tại `backend/`:
   - Service: `postgres` image `postgres:16-alpine`.
   - Port: `5432:5432`.
   - Database: `expensetracker_db`.
   - Username: `expensetracker_user`, Password: `expensetracker_password`.
   - Data volume an toàn: `postgres_data`.

### Bước 6: Thẩm Định Nghiệm Thu (Quality Gate 0)
1. Chạy `dotnet restore` toàn bộ solution.
2. Chạy `dotnet build` kiểm tra đạt **0 Error, 0 Warning**.
3. Chạy 1 bài test khói xUnit mẫu trong `ExpenseTracker.Domain.UnitTests` $\rightarrow$ Pass 100%.

---

## 3. BẢNG TIÊU CHÍ HOÀN THÀNH (ACCEPTANCE CRITERIA)

- [x] Cấu trúc `backend/` và `frontend/` được tạo lập đúng quy ước.
- [x] 4 projects Backend + 2 projects Unit Test liên kết tham chiếu chính xác.
- [x] 1 project .NET MAUI khởi tạo thành công với đủ các NuGet packages.
- [x] `docker-compose.yml` sẵn sàng khởi chạy PostgreSQL 16.
- [x] Lệnh `dotnet build` hoàn thành với **0 Error, 0 Warning** trên cả Backend và Frontend.
- [x] Không có bất kỳ dòng code nghiệp vụ tài chính nào bị sinh ra vội vã (tuân thủ nghiêm ngặt Giai đoạn 0).

