# KẾ HOẠCH TRIỂN KHAI GIAI ĐOẠN 2B: THU NHẬP ĐỊNH KỲ & BÓC TÁCH PHIẾU LƯƠNG AI VISION OCR

> **Mã kế hoạch:** `PLAN-36`  
> **Kế thừa & Căn cứ:** [`PLAN-34`](2026-10-22_plan_34_khoi_dong_giai_doan_2_danh_muc_va_thu_nhap.md), [`PLAN-35`](2026-10-22_plan_35_tai_cau_truc_chien_luoc_trien_khai_backend_first_va_wireframe_frontend.md), [Quyết định 107](../decisions_and_changes_log.md#L2298-L2320), [Quyết định 108](../decisions_and_changes_log.md#L2321-L2345), [Quyết định 109](../decisions_and_changes_log.md#L2346-L2370)  
> **Tài liệu đặc tả nguồn (Source of Truth):** [`reports/use_cases/02_dac_ta_uc_quan_ly_thu_nhap.md`](../reports/use_cases/02_dac_ta_uc_quan_ly_thu_nhap.md) (`UC-02.07 & UC-02.08`)  
> **Nguyên tắc chỉ đạo:** Backend-First, Wireframe Frontend thô, Làm từng bước cuốn chiếu vừa sức, không nhảy cóc, kiểm toán dòng-đối-dòng đạt 100% tài liệu đặc tả IEEE 29148.

---

## 1. Mục Tiêu & Phạm Vi Giai Đoạn 2B

Giai đoạn 2B tập trung giải quyết trọn vẹn 2 Use Cases nghiệp vụ tự động hóa và AI của Phân hệ Thu nhập Cá nhân:
1. **`UC-02.07`: Quản lý thu nhập định kỳ (Recurring Incomes)** — Lập lịch các khoản thu định kỳ (tiền lương, cho thuê nhà, lãi tiết kiệm), tự động căn chỉnh tháng ngắn (28/29/30 ngày đối với mốc ngày 31), tự động sinh giao dịch khi đến hạn (`AutoRecord`) và dự phóng dòng tiền vào.
2. **`UC-02.08`: Bóc tách phiếu lương tự động bằng AI Vision OCR (Payslip OCR Parsing)** — Trích xuất bảng lương từ ảnh chụp/PDF (Gross, Net, BHXH, BHYT, BHTN, Thuế TNCN, Phụ cấp), chạy thuật toán kiểm toán công thức lương $\text{Net} = \text{Gross} - \sum \text{Khấu trừ} + \sum \text{Phụ cấp}$, lưu chi tiết `SalarySlipDetail` và hỗ trợ cập nhật ngược vào lịch lương định kỳ.

---

## 2. Ma Trận Đối Chiếu Đặc Tả Nghiệp Vụ (Traceability Spec Matrix)

| Mã Use Case | Luồng & Ràng Buộc Đặc Tả IEEE 29148 | Yêu Cầu Kỹ Thuật Cần Triển Khai |
| :--- | :--- | :--- |
| **`UC-02.07`** | **Basic Flow:** Khởi tạo lịch thu định kỳ: Tên, Số tiền dự kiến, Ví nhận, Nguồn thu, Tần suất, Ngày nhận (`salaryDay` 1-31), Tùy chọn thực thi (`AutoRecord`). | Entity `RecurringIncome`, DTOs, FluentValidation. |
| **`UC-02.07 E1`** | **Căn chỉnh tháng ngắn:** Nếu ngày nhận là 31 mà tháng chỉ có 30 (hoặc tháng 2 có 28/29 ngày), tự động căn chỉnh ngày đến hạn về ngày cuối cùng của tháng đó. | Thuật toán `CalculateNextDueDate(salaryDay, frequency, fromDate)` sử dụng `DateTime.DaysInMonth`. |
| **`UC-02.07 E2`** | **Số tiền dự kiến không hợp lệ:** Số tiền $\le 0$ phải bị từ chối. | FluentValidation rule `GreaterThan(0)`. |
| **`UC-02.07 A1`** | **Tạm dừng / Kích hoạt lại:** Bật/tắt cờ `IsActive`. | Phương thức `ToggleStatusAsync(recurringIncomeId)`. |
| **`UC-02.07 A2`** | **Tự động ghi nhận khi đến hạn:** Tác vụ nền kiểm tra các lịch thu đến hạn có `AutoRecord == true` để tự động sinh `Transaction` và cộng tiền vào ví. | Service `ProcessDueRecurringIncomesAsync` hỗ trợ gọi thủ công hoặc qua Background Worker định kỳ. Bảo đảm tính Idempotent (không sinh trùng). |
| **`UC-02.08`** | **Basic Flow:** Tải ảnh/PDF phiếu lương, AI Vision bóc tách Gross, Net, Thuế TNCN, Bảo hiểm, Phụ cấp. Lưu vào `SalarySlipDetails` gắn với `Transaction`. | Entity `SalarySlipDetail`, `IPayslipOcrService`, lưu chi tiết bóc tách. |
| **`UC-02.08 Bước 6`** | **Kiểm toán công thức số học:** $\Delta = \|\text{Net} - (\text{Gross} - \sum \text{Khấu trừ} + \sum \text{Phụ cấp})\| \le 1,000$ VND. | Service kiểm toán toán học, trả về cờ `IsMathAudited` và độ lệch sai số. |
| **`UC-02.08 E2`** | **Cảnh báo sai lệch số học:** Nếu $\Delta > 1,000$ VND, đánh dấu cảnh báo màu vàng để người dùng xác nhận lại số tiền thực tế. | Cờ `HasDiscrepancy` và danh sách trường nghi ngờ. |
| **`UC-02.08 A1`** | **Lưu lịch định kỳ tự động:** Cập nhật số tiền Net Salary vào `expectedAmount` của `RecurringIncome`. | Tùy chọn `UpdateRecurringIncomeId` khi lưu giao dịch từ phiếu lương. |

---

## 3. Lộ Trình Phân Kỳ Thực Hiện (2 Chặng Cuốn Chiếu)

### 📌 Chặng 1 (Giai đoạn 2B.1): Quản Lý Thu Nhập Định Kỳ (`UC-02.07`) — [x] ĐÃ HOÀN THÀNH (100% Tests Passed)
1. **Domain:** [x]
   - Xây dựng Enum `RecurringFrequency` (`Daily = 1`, `Weekly = 2`, `Monthly = 3`, `Quarterly = 4`, `Yearly = 5`).
   - Xây dựng Entity `RecurringIncome` (kế thừa `BaseEntity`, liên kết `User`, `Wallet`, `Category`).
   - Viết phương thức nghiệp vụ: `CalculateNextDueDate` (xử lý căn chỉnh tháng ngắn 28/29/30 cho ngày 31 theo `UC-02.07 E1`), `MarkProcessed`, `ToggleStatus`, `Deactivate`, `Activate`, `UpdateDetails`.
2. **Application:** [x]
   - Tạo DTOs: `RecurringIncomeDto`, `CreateRecurringIncomeRequest`, `UpdateRecurringIncomeRequest`, `ProcessRecurringIncomesRequest`, `ProcessRecurringIncomeResultDto`.
   - Viết Validators: `CreateRecurringIncomeValidator`, `UpdateRecurringIncomeValidator`.
   - Cung cấp Interface `IRecurringIncomeService` và Service `RecurringIncomeService`:
     - CRUD lịch thu định kỳ.
     - Xử lý căn chỉnh tháng ngắn (`DateTime.DaysInMonth`).
     - Xử lý kích hoạt/tạm dừng (`ToggleRecurringIncomeStatusAsync`).
     - Xử lý quét và ghi nhận tự động (`ProcessDueRecurringIncomesAsync`) đảm bảo ACID ví và idempotency.
     - Đăng ký DI trong `DependencyInjection.cs`.
3. **Infrastructure:** [x]
   - Cấu hình EF Core `RecurringIncomeConfiguration`.
   - Thêm `DbSet<RecurringIncome> RecurringIncomes` vào `IApplicationDbContext` và `ApplicationDbContext`.
   - Sinh EF Core Migration `20261022145303_AddRecurringIncomes` và apply thành công lên PostgreSQL Docker.
4. **WebApi:** [x]
   - Xây dựng `RecurringIncomesController` với đầy đủ endpoints RESTful (`GET`, `GET {id}`, `POST`, `PUT {id}`, `PATCH {id}/toggle`, `DELETE {id}`, `POST process-due`).
5. **Kiểm thử Unit Tests:** [x]
   - Viết 100% tests cho Domain (`RecurringIncomeTests.cs`) và Application (`RecurringIncomeServiceTests.cs`):
     - Ràng buộc invariants, từ chối số tiền $\le 0$, tên rỗng, ngày ngoài 1-31.
     - Kiểm thử căn chỉnh tháng ngắn: ngày 31 tháng 1 sang tháng 2 căn chỉnh 28/02, tháng 3 sang tháng 4 căn chỉnh 30/04 (`UC-02.07 E1`).
     - Tự động sinh giao dịch khi đến hạn, cộng số dư ví, dời ngày kỳ kế tiếp (`UC-02.07 A2`).
     - Bỏ qua khi chưa đến hạn hoặc `AutoRecord == false`.
   - Toàn bộ **65/65 Unit Tests Passed 100%**.
6. **Frontend Wireframe:** [x]
   - Thêm Refit contract `IRecurringIncomeApi` trong `IApiClients.cs`.
   - Viết `RecurringIncomesViewModel` và Page `RecurringIncomesPage.xaml` (form thêm mới, danh sách lịch thu, nút bật/tắt, nút quét & ghi nhận ngay).
   - Đăng ký DI trong `MauiProgram.cs` và Route trong `AppShell.xaml`.
   - Biên dịch .NET MAUI thành công với **0 Error, 0 Warning**.

---

### 📌 Chặng 2 (Giai đoạn 2B.2): Bóc Tách Phiếu Lương AI Vision OCR & Hậu Kiểm Toán Lương (`UC-02.08`) — [x] ĐÃ HOÀN THÀNH (100% Tests Passed)
1. **Domain:** [x]
   - Xây dựng Entity `SalarySlipDetail` (kế thừa `BaseEntity`, liên kết 1-1 với `Transaction`).
   - Lưu trữ: `EmployerName`, `PayPeriod`, `GrossSalary`, `NetSalary`, `SocialInsurance`, `HealthInsurance`, `UnemploymentInsurance`, `PersonalIncomeTax`, `Allowances`, `Bonus`, `OtherDeductions`, `IsMathAudited`, `DiscrepancyAmount`, `RawOcrJson`.
   - Cài đặt thuật toán kiểm toán công thức toán học `AuditMathDiscrepancy()`: $\Delta = \|\text{Net} - (\text{Gross} - \sum \text{Deductions} + \sum \text{Additions})\| \le 1,000$ VND (`UC-02.08 Bước 6 & E2`).
2. **Application:** [x]
   - Tạo DTOs: `PayslipParsedDataDto`, `ConfirmPayslipRequest`, `SalarySlipDetailDto`.
   - Viết Validators: `ConfirmPayslipValidator`.
   - Cung cấp Interface `IPayslipOcrService` và `PayslipOcrService`:
     - Phân tích Vision OCR (hỗ trợ định dạng `.jpg`, `.jpeg`, `.png`, `.pdf` theo `UC-02.08 Bước 2`).
     - Thuật toán kiểm toán công thức số học, tính độ lệch sai số và bật cờ cảnh báo `HasDiscrepancy`.
     - Lưu giao dịch thu nhập và chi tiết phiếu lương trong cùng 1 Database Transaction nguyên tử (ACID), cộng số dư ví tương ứng.
     - Tùy chọn cập nhật ngược `expectedAmount` cho `RecurringIncome` theo `UC-02.08 A1`.
     - Đăng ký DI trong `DependencyInjection.cs`.
3. **Infrastructure:** [x]
   - Cấu hình EF Core `SalarySlipDetailConfiguration`.
   - Thêm `DbSet<SalarySlipDetail> SalarySlipDetails` vào `IApplicationDbContext` và `ApplicationDbContext`.
   - Sinh EF Core Migration `20261022150459_AddSalarySlipDetails` và apply thành công lên PostgreSQL Docker.
4. **WebApi:** [x]
   - Xây dựng `PayslipsController` với đầy đủ endpoints RESTful (`POST parse`, `POST confirm`, `GET transaction/{transactionId}`).
5. **Kiểm thử Unit Tests:** [x]
   - Viết 100% tests cho Domain (`SalarySlipDetailTests.cs`) và Application (`PayslipOcrServiceTests.cs`):
     - Kiểm toán công thức cân bằng, phát hiện sai lệch số học $\Delta > 1000$ VND (`UC-02.08 E2`).
     - Lưu giao dịch Net vào `Transaction` + `SalarySlipDetail` + cộng số dư ví.
     - Cập nhật số tiền lương cho `RecurringIncome` (`UC-02.08 A1`).
     - Toàn bộ **78/78 Unit Tests Passed 100%**.
6. **Frontend Wireframe (.NET MAUI):** [x]
   - Thêm Refit contract `IPayslipApi` trong `IApiClients.cs`.
   - Viết ViewModel `PayslipOcrViewModel` và Page `PayslipOcrPage.xaml` (chọn file qua `FilePicker`, hiển thị bảng phân tích Payslip Preview, banner cảnh báo vàng sai lệch số học, form chọn ví và lưu sổ cái).
   - Đăng ký DI trong `MauiProgram.cs` và Route trong `AppShell.xaml`.
   - Biên dịch .NET MAUI thành công với **0 Error, 0 Warning**.

---

## 4. Tiêu Chuẩn Nghiệm Thu Giai Đoạn 2B (Quality Gate) — [x] ĐÃ ĐẠT 100%
- **Kiểm toán dòng-đối-dòng:** 100% các điều kiện trong `UC-02.07` và `UC-02.08` đều có code và test chứng minh.
- **Unit Tests:** Toàn bộ **78/78 tests Passed 100%**.
- **Biên dịch:** WebApi và .NET MAUI đạt **0 Error, 0 Warning**.
- **Ghi chép nhật ký:** Cập nhật đầy đủ vào `decisions_and_changes_log.md`.
