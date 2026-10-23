# 🚀 KẾ HOẠCH THI CÔNG: GIAI ĐOẠN 1A — XÁC THỰC, JWT ROTATION & QUẢN LÝ HỒ SƠ
## (IMPLEMENTATION PLAN: STAGE 1A — AUTHENTICATION, JWT ROTATION & USER PROFILE)

> **Mã kế hoạch:** `PLAN-32`  
> **Thời điểm:** 22/10/2026  
> **Trạng thái:** `COMPLETED & VERIFIED (ĐÃ HOÀN TẤT & NGHIỆM THU XUẤT SẮC)`  
> **Chủ trì phiên họp:** Agent 00 (Council Orchestrator)  
> **Thành viên phản biện:**
> - Agent 11: Bruce Schneier (Fintech Security, Password Hashing & JWT Rotation)
> - Agent 04: Eric Evans (Domain Modeling & User Aggregate Root)
> - Agent 10: Dan Abramov (Client Auth State & SecureStorage Token Manager)
> - Agent 12: Kent Beck (Unit Tests TDD & Verification)
> **Tài liệu căn cứ:** 
> - [`TECHNICAL_ARCHITECTURE_SPEC.md`](../../mobile-project/dotnet-maui-ver/TECHNICAL_ARCHITECTURE_SPEC.md) (Mục 1, 3.1 & 3.2)
> - [`MASTER_PROJECT_ROADMAP.md`](../../mobile-project/dotnet-maui-ver/MASTER_PROJECT_ROADMAP.md) (Giai đoạn 1)
> - [`01_dac_ta_uc_tai_khoan_va_da_vi.md`](../reports/use_cases/01_dac_ta_uc_tai_khoan_va_da_vi.md) (`UC-01.01` đến `UC-01.06`)
> - [`.agents/rules/05_engineering_architecture_and_code_craft_rules.md`](../../.agents/rules/05_engineering_architecture_and_code_craft_rules.md)

---

## 1. ĐÁNH GIÁ KHỐI LƯỢNG & CHIẾN LƯỢC CHIA NHỎ (SUB-STAGING STRATEGY)

### 1.1. Đánh giá quy mô Giai đoạn 1 gốc
Giai đoạn 1 trong bản vẽ Use Case bao gồm **10 Use Cases chi tiết** (`UC-01.01` đến `UC-01.10`) với 2 cụm nghiệp vụ hoàn toàn khác biệt:
1. **Cụm 1: Xác thực, Bảo mật & Hồ sơ người dùng** (`UC-01.01` đến `UC-01.06`).
2. **Cụm 2: Quản lý Đa Ví & Giao dịch chuyển tiền nội bộ ACID** (`UC-01.07` đến `UC-01.10`).

> [!IMPORTANT]
> **Kết luận Hội đồng:** Nếu làm gộp cả 10 Use Cases, số lượng file mã nguồn cần viết sẽ vượt quá 45 files cùng lúc. Việc phân kỳ làm 2 giai đoạn con giúp đảm bảo chất lượng từng dòng code, kiểm thử tự động khép kín và không gây lỗi phát sinh.

### 1.2. Phân kỳ thành 2 Giai đoạn con tinh gọn
- **Giai đoạn 1A (Kế hoạch này - `PLAN-32`):** Hoàn thiện trọn vẹn **Khối Xác Thực, Bảo Mật & Hồ Sơ** (`UC-01.01` đến `UC-01.06`).
- **Giai đoạn 1B (Kế hoạch tiếp theo - `PLAN-33`):** Hoàn thiện trọn vẹn **Khối Đa Ví & Chuyển Tiền ACID** (`UC-01.07` đến `UC-01.10`).

---

## 2. MỤC TIÊU CỐT LÕI CỦA GIAI ĐOẠN 1A (ĐÃ ĐẠT ĐƯỢC 100%)

1. **Backend Domain & Persistence:** Đã tạo `User`, `RefreshToken`, `BaseEntity`, `Result<T>`, EF Core Migration `InitialCreate_UsersAndAuth`.
2. **Backend Security & Service Layer:** `BCrypt.Net-Next`, `JwtTokenGenerator` hỗ trợ Token Rotation, `AuthService`, `UserService`, `AuthController`, `UsersController`.
3. **Backend Testing Harness:** 8/8 Unit Tests Passed (100%).
4. **Frontend .NET MAUI Client:** `SecureStorageTokenService`, `AuthHeaderHandler`, Refit `IAuthApi` & `IUserApi`, `ClientAuthService`, `LoginViewModel`, `RegisterViewModel`, `LoginPage.xaml`, `RegisterPage.xaml`.
5. **Nghiệm thu Quality Gate 1A:** Build 0 Error, 0 Warning.

---

## 3. TIÊU CHÍ HOÀN THÀNH (ACCEPTANCE CRITERIA)

- [x] Entity `User` và `RefreshToken` được ánh xạ chuẩn xác vào CSDL PostgreSQL 16 qua EF Core Migration (`InitialCreate_UsersAndAuth`).
- [x] Dịch vụ mã hóa mật khẩu bảo mật (BCrypt) và JWT Token Generator hoạt động chuẩn xác.
- [x] Cơ chế **Refresh Token Rotation** một lần dùng bảo vệ tài khoản chống lại tấn công Replay Attack.
- [x] 7 REST Endpoints Auth & Profile hoạt động chuẩn mực (`register`, `login`, `refresh-token`, `logout`, `profile`, `change-password`).
- [x] Toàn bộ Unit Tests cho AuthService đạt tỷ lệ thành công **100% (8/8 Passed)**.
- [x] MAUI Client lưu trữ an toàn Token trong `SecureStorage` và tự động gắn header Authorization.
- [x] Giao diện Đăng nhập / Đăng ký hiển thị trực quan, mượt mà trên nền tảng di động/desktop.
- [x] Toàn bộ mã nguồn tuân thủ triệt để Quy tắc Ghi chú súc tích 1-2 dòng, giữ 100% thuật ngữ tiếng Anh, không dùng CQRS/MediatR thừa thãi.

