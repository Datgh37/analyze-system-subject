# 📋 KẾ HOẠCH TRIỂN KHAI TOÀN DIỆN (PLAN-50)
## CHUẨN HÓA TOÀN BỘ REPOSITORY LAYER, ĐỘC LẬP HOÀN TOÀN OFFLINE DEMO & KHẮC PHỤC TRIỆT ĐỂ KHIẾM KHUYẾT UI TRÊN .NET MAUI
**Dự án:** Smart Personal Expense & Financial Planning Management System (.NET MAUI 10)  
**Ngày lập:** 2026-10-26  
**Chủ trì:** Council Orchestrator (Agent 00), Agent 09 Brad Frost, Agent 10 Dan Abramov  

---

### 1. Bối Cảnh & Động Lực
1. **Yêu cầu người dùng:** *"tiếp tục sửa lỗi nếu vẫn còn lỗi kiểm thử xem UI có bị lỗi không có thì tiếp tục sửa"*.
2. **Khiếm khuyết phát hiện:**
   - Người dùng phát hiện tiêu đề bị cắt ngắn kèm mã đặc tả kỹ thuật thô: `Bóc Tách Phiếu Lương AI (UC-02....` trên thanh điều hướng.
   - Một số màn hình trực tiếp gọi Refit client ra cổng `http://10.0.2.2:5000` (không có Docker/backend chạy nền) dẫn đến tình trạng treo con quay `ActivityIndicator` suốt 100 giây trước khi báo lỗi `Connection failure` (`SavingsGoals`, `RecurringIncomes`, `SmartUtilities`, `ReceiptOcr`, `PayslipOcr`, `CreateBudget`, `CreateSavingsGoal`).
   - Các nhãn huy hiệu còn sót mã UC thô: `CẢNH BÁO SAI LỆCH SỐ HỌC (UC-03.08 E1)`, `CẢNH BÁO SAI LỆCH SỐ HỌC (UC-02.08 E2)`, `UC-03.11 NLP`, `UC-05.01`.
   - Lỗi chữ bị ép co ngắn trên tiêu đề widget DSA: `HẠN MỨC CHI TIÊU HÔM NAY (ADAPTIV`.

---

### 2. Hành Động Thực Hiện
1. **Thiết lập Bộ 7 Cặp Repository Độc Lập Cho Mọi Phân Hệ:**
   - `ICategoryRepository` + `MockCategoryRepository` + `ApiCategoryRepository`
   - `ISavingsGoalRepository` + `MockSavingsGoalRepository` + `ApiSavingsGoalRepository`
   - `IAutoRuleRepository` + `MockAutoRuleRepository` + `ApiAutoRuleRepository`
   - `IRecurringIncomeRepository` + `MockRecurringIncomeRepository` + `ApiRecurringIncomeRepository`
   - `ISmartUtilityRepository` + `MockSmartUtilityRepository` + `ApiSmartUtilityRepository`
   - `IReceiptRepository` + `MockReceiptRepository` + `ApiReceiptRepository`
   - `IPayslipRepository` + `MockPayslipRepository` + `ApiPayslipRepository`
2. **Refactor 7 ViewModels Sang Dependency Injection Chuẩn Clean MVVM:**
   - `SavingsGoalsViewModel.cs`
   - `RecurringIncomesViewModel.cs`
   - `SmartUtilitiesViewModel.cs`
   - `ReceiptOcrViewModel.cs`
   - `PayslipOcrViewModel.cs`
   - `CreateSavingsGoalViewModel.cs`
   - `CreateBudgetViewModel.cs`
3. **Đăng Ký Đầy Đủ Vào Container DI (`MauiProgram.cs`):**
   - Đảm bảo 100% màn hình khi chạy ở chế độ Mock Data đều phản hồi tức thì 0–10ms, không phụ thuộc mạng, không lỗi mạng, hiển thị dữ liệu sống động.
4. **Làm Sạch Toàn Bộ Chuỗi UI & Bố Cục:**
   - Loại bỏ toàn bộ tiền tố mã Use Case thô (`UC-02.08`, `UC-03.08`, `UC-03.11`, `UC-05.01`) khỏi nhãn người dùng.
   - Tối ưu hóa chuỗi tiêu đề không bị cắt ngắn trên mọi kích cỡ màn hình di động.

---

### 3. Nghiệm Thu & Đánh Giá
- Biên dịch thành công 0 Warning, 0 Error.
- Triển khai lên máy ảo Android `emulator-5554` và chụp ảnh kiểm chứng từng màn hình.
