# 🚀 KẾ HOẠCH THI CÔNG: GIAI ĐOẠN 1B — QUẢN LÝ ĐA VÍ & CHUYỂN TIỀN NỘI BỘ ACID
## (IMPLEMENTATION PLAN: STAGE 1B — MULTI-WALLET ENGINE & ACID INTERNAL TRANSFERS)

> **Mã kế hoạch:** `PLAN-33`  
> **Thời điểm lập:** 22/10/2026  
> **Trạng thái:** `COMPLETED & VERIFIED (ĐÃ HOÀN TẤT & NGHIỆM THU XUẤT SẮC)`  
> **Chủ trì phiên họp:** Agent 00 (Council Orchestrator)  
> **Thành viên tham gia:**
> - Agent 04: Eric Evans (Domain Modeling, Wallet Aggregate Root & Money Value Object)
> - Agent 05: Martin Kleppmann (ACID Concurrency, Pessimistic Locking & Deadlock Prevention)
> - Agent 08: Andrew Ng (Account Balance Screenshot OCR Pipeline)
> - Agent 10: Dan Abramov (Client Multi-Wallet State & Offline Sync)
> - Agent 12: Kent Beck (Concurrency Tests & Invariant Integrity QA)
> **Tài liệu căn cứ:** 
> - [`TECHNICAL_ARCHITECTURE_SPEC.md`](../../mobile-project/dotnet-maui-ver/TECHNICAL_ARCHITECTURE_SPEC.md) (Chương 2, 3)
> - [`01_dac_ta_uc_tai_khoan_va_da_vi.md`](../reports/use_cases/01_dac_ta_uc_tai_khoan_va_da_vi.md) (`UC-01.07` đến `UC-01.10`)
> - [`files/UCD-DuoNoPro.drawio`](../../files/UCD-DuoNoPro.drawio) (Tab 1: Quản lý Tài khoản & Đa Ví)
> - [`.agents/rules/05_engineering_architecture_and_code_craft_rules.md`](../../.agents/rules/05_engineering_architecture_and_code_craft_rules.md)

---

## 1. ĐỐI TƯỢNG VÀ PHẠM VI USE CASE TRIỂN KHAI

Giai đoạn 1B hiện thực hóa 4 Use Cases chuyên sâu về sổ cái tài chính của người dùng:
1. **`UC-01.07`: Khởi tạo và cấu hình đa ví (Multi-Wallet Lifecycle)**
   - Hỗ trợ 4 loại ví: Tiền mặt (`Cash`), Ngân hàng (`BankAccount`), Ví điện tử (`EWallet`), Thẻ tín dụng (`CreditCard`).
   - Cấu hình hạn mức tín dụng (`CreditLimit`), ngày chốt sao kê (`StatementDay`), ngày đến hạn thanh toán (`PaymentDueDay`).
   - Tùy chọn loại trừ khỏi tổng tài sản (`IsExcludedFromTotal`).
2. **`UC-01.08`: Chuyển tiền nội bộ giữa các ví (ACID Internal Transfer)**
   - Bảo đảm tính toàn vẹn số dư tài chính: $Tổng\_Số\_Dư_{sau} = Tổng\_Số\_Dư_{trước}$.
   - Khóa bi quan (`SELECT FOR UPDATE`) ngăn ngừa Race Condition.
   - Cơ chế sắp xếp ID khóa theo thứ tự tăng dần (`Id_A < Id_B`) chống Deadlock tuyệt đối.
3. **`UC-01.09`: Đối soát và cân chỉnh số dư thực tế (Balance Reconciliation)**
   - Lập biên bản chênh lệch bất biến `BalanceAdjustment` phục vụ kiểm toán tài chính cá nhân.
   - Ghi nhận `OldBalance`, `NewBalance`, `DifferenceAmount` và lý do cân chỉnh.
4. **`UC-01.10`: Cân chỉnh số dư ví tự động bằng ảnh chụp tài khoản (Balance Screenshot OCR)**
   - Bóc tách số dư tài khoản ngân hàng từ ảnh chụp màn hình ứng dụng ngân hàng.
   - Cung cấp dịch vụ `IBalanceOcrService` (Mock + Gemini Vision OCR).

---

## 2. KIẾN TRÚC VÀ CÁC THÀNH PHẦN KỸ THUẬT CẦN XÂY DỰNG

### 2.1. Backend (Clean Architecture)
1. **Domain Layer:**
   - Enum `WalletType`: `Cash = 1`, `BankAccount = 2`, `EWallet = 3`, `CreditCard = 4`.
   - Entity `Wallet`: Kế thừa `BaseEntity` (đã có sẵn `IsDeleted`, `DeletedAt`, `CreatedAt`, `UpdatedAt`), quản lý số dư, logic ghi nợ/ghi có an toàn (`Debit`, `Credit`), kiểm tra hạn mức thẻ tín dụng.
   - Entity `BalanceAdjustment`: Kế thừa `BaseEntity`, lưu vết kiểm toán cân chỉnh.
2. **Application Layer:**
   - DTOs: `WalletDto`, `CreateWalletRequest`, `UpdateWalletRequest`, `TransferMoneyRequest`, `AdjustBalanceRequest`, `BalanceAdjustmentDto`, `OcrBalanceScanResponse`.
   - Validators: `CreateWalletValidator`, `TransferMoneyValidator`, `AdjustBalanceValidator`.
   - `IWalletService` & `WalletService`:
     * CRUD ví và quản lý trạng thái.
     * `TransferMoneyAsync`: Thực thi trong Database Transaction cấp độ Serializable/ReadCommitted kèm Pessimistic Locking.
     * `AdjustBalanceAsync`: Tạo `BalanceAdjustment` và cập nhật ví.
     * `ScanBalanceFromImageAsync`: Bóc tách số dư từ file ảnh stream.
3. **Infrastructure Layer:**
   - EF Core Configurations: `WalletConfiguration`, `BalanceAdjustmentConfiguration`.
   - EF Core Migration: `AddWalletsAndBalanceAdjustments`.
   - `MockBalanceOcrService` / `GeminiBalanceOcrService`: Nhận diện số tiền và tên tài khoản từ ảnh chụp.
4. **WebApi Layer:**
   - `WalletsController`: 8 endpoints chuẩn RESTful.
5. **Testing Harness:**
   - Unit tests tính toán số dư ví, nạp/rút/chuyển tiền.
   - Concurrency tests kiểm tra chuyển tiền đa luồng không lệch 1 xu.

### 2.2. Frontend (.NET MAUI MVVM)
1. **Mobile Models & Refit API:**
   - Refit `IWalletApi`: Ánh xạ toàn bộ API ví.
   - `WalletModel`: Trình bày thẻ ví trực quan.
2. **User Interface (XAML & ViewModels):**
   - `WalletsViewModel` & `WalletsPage.xaml`:
     * Danh sách ví hiển thị dạng thẻ đẹp mắt, phân loại màu sắc theo loại ví (Cash, Bank, E-Wallet, Credit Card).
     * Tổng tài sản khả dụng (Net Worth) tự động tính toán.
     * Nút thao tác nhanh: Thêm ví, Chuyển tiền nội bộ, Đối soát số dư.

---

## 3. LỘ TRÌNH THỰC THI (STEP-BY-STEP CHECKLIST)

- [x] **Bước 1 (Domain Core):** Tạo enum `WalletType`, `WalletStatus`, entity `Wallet`, entity `BalanceAdjustment`.
- [x] **Bước 2 (Application Layer):** Tạo DTOs, FluentValidation validators và interface `IWalletService`, `IBalanceOcrService`.
- [x] **Bước 3 (Infrastructure Persistence):** Thêm DbSet vào `IApplicationDbContext` & `ApplicationDbContext`, cấu hình EntityTypeConfiguration, tạo EF Core Migration `AddWalletsAndBalanceAdjustments`.
- [x] **Bước 4 (Service Implementation):** Viết `WalletService` với thuật toán chuyển tiền ACID (Pessimistic Lock & No Deadlock) + `MockBalanceOcrService`.
- [x] **Bước 5 (WebApi Controller):** Viết `WalletsController` với bảo vệ JWT Authorize.
- [x] **Bước 6 (Backend Tests):** Viết bộ Unit Tests cho Wallet và Transfer logic. Đảm bảo 100% test pass (21/21 tests passed).
- [x] **Bước 7 (Frontend MAUI):** Refit interface `IWalletApi`, `WalletsViewModel` và giao diện `WalletsPage.xaml`.
- [x] **Bước 8 (Quality Gate & Verification):** Chạy `dotnet test` Backend (100% pass, 21/21 tests) và `dotnet build` MAUI (0 error, 0 warning).

---

## 4. TIÊU CHÍ NGHIỆM THU (ACCEPTANCE CRITERIA)

1. CSDL có đầy đủ 2 bảng mới `wallets` và `balance_adjustments` với khóa ngoại, chỉ mục (index) và trường Audit/Xóa mềm đầy đủ.
2. Giao dịch chuyển tiền nội bộ giữa 2 ví bảo toàn tuyệt đối 100% tổng tiền, không xảy ra race condition hay deadlock khi chạy đồng thời.
3. Chức năng cân chỉnh số dư tạo ra bản ghi nhật ký `BalanceAdjustment` bất biến.
4. Giao diện MAUI hiển thị danh sách ví mượt mà, phân loại trực quan, hỗ trợ cache offline.
5. Toàn bộ mã nguồn tuân thủ Quy tắc Note ngắn gọn 1-2 dòng, không rườm rà.
