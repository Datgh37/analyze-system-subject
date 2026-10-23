# 🚀 KẾ HOẠCH THI CÔNG: GIAI ĐOẠN 2A — CÂY DANH MỤC PHÂN CẤP & SỔ THU NHẬP CÁ NHÂN
## (IMPLEMENTATION PLAN: STAGE 2A — CATEGORY HIERARCHY & PERSONAL INCOME ENGINE)

> **Mã kế hoạch:** `PLAN-34`  
> **Thời điểm lập:** 22/10/2026  
> **Trạng thái:** `PENDING APPROVAL (CHỜ PHÊ DUYỆT CỦA NGƯỜI DÙNG)`  
> **Chủ trì phiên họp:** Agent 00 (Council Orchestrator)  
> **Thành viên tham gia:**
> - Agent 04: Eric Evans (Category Hierarchy Composite Pattern & Transaction Aggregate)
> - Agent 05: Martin Kleppmann (PostgreSQL Recursive CTE & Transaction ACID with Wallets)
> - Agent 09: Brad Frost (Category Iconography & Income Form Design System)
> - Agent 10: Dan Abramov (Client Categories/Income State & MVVM Binding)
> - Agent 12: Kent Beck (Category Tree Integrity & Income-Wallet Math Tests)
> **Tài liệu căn cứ:** 
> - [`2026-10-22_plan_35_tai_cau_truc_chien_luoc_trien_khai_backend_first_va_wireframe_frontend.md`](2026-10-22_plan_35_tai_cau_truc_chien_luoc_trien_khai_backend_first_va_wireframe_frontend.md) (`Quyết định 104` - Chiến lược Backend-First & Wireframe Frontend)
> - [`TECHNICAL_ARCHITECTURE_SPEC.md`](../../mobile-project/dotnet-maui-ver/TECHNICAL_ARCHITECTURE_SPEC.md) (Chương 2, 3)
> - [`02_dac_ta_uc_quan_ly_thu_nhap.md`](../reports/use_cases/02_dac_ta_uc_quan_ly_thu_nhap.md) (`UC-02.01` đến `UC-02.06`)
> - [`files/UCD-DuoNoPro.drawio`](../../files/UCD-DuoNoPro.drawio) (Tab 2: Quản lý Thu nhập & Tab 4: Danh mục)
> - [`.agents/rules/05_engineering_architecture_and_code_craft_rules.md`](../../.agents/rules/05_engineering_architecture_and_code_craft_rules.md)

---

## 1. ĐÁNH GIÁ KHỐI LƯỢNG & CHIẾN LƯỢC CHIA NHỎ GIAI ĐOẠN 2

### 1.1. Đánh giá quy mô Giai đoạn 2 gốc
Theo lộ trình tổng thể [`MASTER_PROJECT_ROADMAP.md`](../../mobile-project/dotnet-maui-ver/MASTER_PROJECT_ROADMAP.md), Giai đoạn 2 gốc bao gồm tới **26 Use Cases** thuộc 3 phân hệ:
1. Phân hệ 4: Danh mục Thu/Chi (`UC-04.01` đến `UC-04.07`).
2. Phân hệ 2: Quản lý Thu nhập & Bóc tách phiếu lương AI (`UC-02.01` đến `UC-02.08`).
3. Phân hệ 3: Quản lý Chi tiêu, Hóa đơn OCR & VietQR (`UC-03.01` đến `UC-03.11`).

> [!IMPORTANT]
> **Nghị quyết Hội đồng & Chỉ đạo Tái cấu trúc (`PLAN-35`):**
> 1. Chia Giai đoạn 2 thành **3 giai đoạn con tinh gọn**:
>    - **Giai đoạn 2A (Kế hoạch này - `PLAN-34`):** Cây Danh mục phân cấp & Quản lý Thu nhập cốt lõi (`UC-04.01 - 04.07` và `UC-02.01 - 02.06`).
>    - **Giai đoạn 2B:** Thu nhập định kỳ & Bóc tách phiếu lương AI Vision OCR (`UC-02.07` và `UC-02.08`).
>    - **Giai đoạn 2C:** Sổ chi tiêu, Pipeline bóc tách hóa đơn siêu thị AI OCR & Hậu kiểm toán số học (`UC-03.01 - 03.11`).
> 2. **Nguyên tắc Backend-First & Wireframe Frontend:**
>    - Toàn bộ thời gian và sự tập trung dồn vào Backend Clean Architecture, Entity Rules, PostgreSQL 3NF, ACID Trừ/Cộng số dư ví và Unit Tests.
>    - Phía Frontend MAUI **chỉ dựng khung Wireframe thô sơ (trắng đen)** để nối dây Refit API, không CSS màu mè, không trang trí. Mọi công việc thẩm mỹ dời sang **Giai đoạn 7**.

---

## 2. NỘI DUNG TRIỂN KHAI GIAI ĐOẠN 2A

### 2.1. Phân hệ Cây Danh Mục (`UC-04.01` đến `UC-04.07`)
- Mô hình danh mục cha - con phân cấp đệ quy (Self-referencing Foreign Key `parent_id`).
- Phân loại danh mục theo loại giao dịch (`INCOME`, `EXPENSE`).
- Ánh xạ nhóm tài chính: Phân loại theo quy tắc 50/30/20 (`NEEDS`, `WANTS`, `SAVINGS_INVESTMENTS`) và phương pháp Kakeibo (`SURVIVAL`, `OPTIONAL`, `CULTURE`, `EXTRA`).
- Nạp sẵn bộ danh mục mẫu chuẩn Việt Nam (Lương, Thưởng, Tiết kiệm, Đầu tư, Ăn uống, Nhà ở, Đi lại,...).

### 2.2. Phân hệ Ghi Nhận Thu Nhập (`UC-02.01` đến `UC-02.06`)
- `UC-02.01`: Thêm khoản thu nhập mới -> Tự động kích hoạt ACID Transaction cộng số dư ví tiếp nhận (`wallet.Credit(amount)`).
- `UC-02.02`: Xem danh sách thu nhập -> Hỗ trợ lọc theo tháng, theo ví, theo nguồn thu nhập (Active vs Passive).
- `UC-02.03`: Xem chi tiết khoản thu nhập.
- `UC-02.04`: Chỉnh sửa khoản thu nhập -> Tự động tính chênh lệch để cân đối số dư ví cũ và ví mới.
- `UC-02.05`: Xóa khoản thu nhập (Soft delete) -> Hoàn tác trừ số dư ví tiếp nhận an toàn.
- `UC-02.06`: Phân loại nguồn thu nhập (Thu nhập chủ động, Thu nhập thụ động, Khác).

---

## 3. THIẾT KẾ KIẾN TRÚC & CÁC THÀNH PHẦN KỸ THUẬT

### 3.1. Backend (Clean Architecture - Trọng Tâm 100%)
1. **Domain Layer:**
   - Enums: `TransactionType` (Income = 1, Expense = 2), `IncomeCategoryGroup` (Active = 1, Passive = 2, Other = 3).
   - Entity `Category`: `Name`, `TransactionType`, `ParentId`, `IconName`, `ColorCode`, `IsSystem`, `BudgetRuleMapping`.
   - Entity `Transaction`: `UserId`, `WalletId`, `CategoryId`, `Amount`, `TransactionType`, `TransactionDate`, `Note`, `IsEssential`, `IsWindfall`. Kế thừa `BaseEntity` (đầy đủ Soft Delete & Audit).
2. **Application Layer:**
   - DTOs: `CategoryDto`, `CreateCategoryRequest`, `IncomeTransactionDto`, `CreateIncomeRequest`, `UpdateIncomeRequest`.
   - Validators: `CreateIncomeValidator`, `UpdateIncomeValidator`.
   - Services: `ICategoryService` / `CategoryService`, `ITransactionService` / `TransactionService` (xử lý cập nhật số dư ví ACID).
3. **Infrastructure Layer:**
   - Configurations: `CategoryConfiguration`, `TransactionConfiguration`.
   - CSDL Migration EF Core: `AddCategoriesAndTransactions`.
   - Seed Data: Khởi tạo sẵn bộ danh mục tài chính mặc định.
4. **WebApi Layer:**
   - `CategoriesController` (Quản lý cây danh mục).
   - `TransactionsController` (Các endpoints thu nhập: thêm, sửa, xóa, lọc theo thời gian/ví).
5. **Testing Harness:**
   - Unit tests tính toán số dư ví khi thêm/sửa/xóa thu nhập.
   - Kiểm tra cây danh mục đệ quy cha-con không bị lặp vô tận.

### 3.2. Frontend (.NET MAUI - Wireframe Nối Dây Thuần Túy)
1. **Refit API:**
   - `ICategoryApi`, `ITransactionApi`.
2. **ViewModels & Views (Wireframe Layout):**
   - `IncomesViewModel`, `CategoriesViewModel`: Xử lý binding trạng thái (Empty, Loading, Success, Error).
   - `CategoriesPage.xaml` (Wireframe): Danh sách cây danh mục dạng text phân cấp.
   - `IncomesPage.xaml` (Wireframe): Danh sách các giao dịch thu nhập hiển thị dưới dạng TableView / ListView thô.
   - `AddIncomePage.xaml` (Wireframe): Form nhập liệu tối giản (Entry số tiền, Picker chọn Ví, Picker chọn Danh mục, DatePicker, Button Lưu).
   - **Quy chuẩn hiển thị:** Tuyệt đối không trang trí màu mè, không hardcode mã màu hex, toàn bộ styling để dành cho Giai đoạn 7.

---

## 4. LỘ TRÌNH THỰC THI (STEP-BY-STEP CHECKLIST)

- [x] **Bước 1 (Domain Core):** Tạo entity `Category`, entity `Transaction`, enums liên quan (`0 Warning, 0 Error`).
- [x] **Bước 2 (Application Layer):** Tạo DTOs, Validators và các interfaces `ICategoryService`, `ITransactionService` (`0 Warning, 0 Error`).
- [x] **Bước 3 (Infrastructure Persistence):** Cấu hình EntityTypeConfiguration, Seed Data danh mục mẫu, tạo EF Core Migration `AddCategoriesAndTransactions` và apply PostgreSQL DB thành công.
- [x] **Bước 4 (Service Implementation):** Viết `CategoryService` và `TransactionService` bảo đảm liên kết số dư ví toàn vẹn ACID.
- [x] **Bước 5 (WebApi Controllers):** Viết `CategoriesController` và `TransactionsController` bảo vệ bằng `[Authorize]`.
- [x] **Bước 6 (Backend Tests):** Viết Unit Tests cho Category và Income ACID. Đạt **30/30 Tests Passed 100%**.
- [x] **Bước 7 (Frontend MAUI Wireframe):** Khai báo Refit API, viết `IncomesViewModel`, `CategoriesViewModel` và các màn hình Wireframe kết nối API (`CategoriesPage.xaml`, `IncomesPage.xaml`).
- [x] **Bước 8 (Quality Gate & Verification):** Chạy `dotnet test` Backend (100% pass) và `dotnet build` MAUI (**0 error, 0 warning**).
