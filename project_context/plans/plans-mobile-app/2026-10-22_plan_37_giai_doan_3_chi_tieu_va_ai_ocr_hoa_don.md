# KẾ HOẠCH TRIỂN KHAI CHI TIẾT: GIAI ĐOẠN 3 — PHÂN HỆ 3: QUẢN LÝ CHI TIÊU CÁ NHÂN & BÓC TÁCH HÓA ĐƠN MULTIMODAL AI OCR (KÈM UC-04.07 SMART RULES)

> **Mã kế hoạch:** `PLAN-37`  
> **Giai đoạn trong lộ trình:** Giai đoạn 3 (Phân hệ 3 & Bổ sung Phân hệ 4)  
> **Mã phân hệ phụ trách:** `UC-03.01` đến `UC-03.11` (11 Use Cases) và `UC-04.07` (1 Use Case)  
> **Quy chuẩn kỹ thuật:** IEEE 29148, Alistair Cockburn BA, DDD Clean Architecture ASP.NET Core 10, .NET MAUI 10, PostgreSQL 16  
> **Nguyên tắc phối hợp:** Áp dụng nghiêm ngặt [Quyết định 113](../../decisions_and_changes_log.md) (Context Persistence, Minh bạch phụ thuộc liên module, Phê duyệt qua Artifact Proceed).

---

## 1. PHÂN TÍCH PHỤ THUỘC LIÊN MODULE & LÝ DO CHUYỂN GIAO

Trong thiết kế hệ thống PFM, có mối quan hệ phụ thuộc chặt chẽ giữa các Use Cases:
- **`UC-04.07` (Quy tắc thông minh so khớp từ khóa):** Vốn thuộc Phân hệ 4 (Danh mục thu chi). Tuy nhiên, giá trị sử dụng lớn nhất của quy tắc từ khóa là tự động gán danh mục cho **từng dòng món hàng (ReceiptItems)** sau khi quét hóa đơn siêu thị (ví dụ: bóc tách món "Sữa chua hoa quả" $\to$ tự động nhận diện từ khóa "Sữa chua" gán vào "Ăn uống / Thực phẩm"). Do đó, `UC-04.07` được tích hợp triển khai tại Chặng 3A làm nền tảng trực tiếp phục vụ cho bóc tách hóa đơn ở Chặng 3B.
- **Thứ tự triển khai chuẩn kỹ nghệ:**
  1. Xây dựng Sổ cái Chi tiêu cốt lõi ACID & Động cơ Quy tắc Thông minh (Chặng 3A).
  2. Xây dựng Bóc tách Hóa đơn Multimodal AI OCR, Kiểm toán Số học & Tiện ích Thông minh (Chặng 3B).

---

## 2. PHÂN RÃ HẠNG MỤC THI CÔNG CHI TIẾT

### CHẶNG 3A: LÕI SỔ CÁI CHI TIÊU ACID & ĐỘNG CƠ QUY TẮC THÔNG MINH (UC-03.01 - 03.06 & UC-04.07)

#### A. Backend Domain & Persistence
1. **Entity `AutoCategorizationRule.cs` (`UC-04.07`):**
   - Các trường: `Id`, `UserId`, `KeywordPattern` (từ khóa so khớp), `TargetCategoryId`, `PriorityOrder` (thứ tự ưu tiên), `IsRegex`, `IsCaseSensitive`.
   - Ràng buộc: `KeywordPattern` tối thiểu 2 ký tự, không rỗng; quan hệ Foreign Key với `Category`.
2. **Mở rộng/Xác lập Entity Chi tiêu (`ExpenseTransaction`):**
   - Đảm bảo `Transaction` loại `Expense` hỗ trợ các trường tài chính: `IsEssential` (Thiết yếu 50%), `IsRecurring` (Định kỳ), `IsWindfall`, `ExcludeFromReport`, liên kết danh mục `CategoryId` và ví thanh toán `WalletId`.
   - Hỗ trợ quan hệ 1-N với bảng `ReceiptItem` (danh sách món hàng chi tiết khi chi tiêu từ hóa đơn).
3. **Cập nhật EF Core & Migration:**
   - Cấu hình `AutoCategorizationRuleConfiguration`, `ReceiptItemConfiguration`.
   - Sinh migration `20261022_AddExpenseReceiptAndAutoRules` và apply lên PostgreSQL.

#### B. Backend Application & WebApi
1. **`AutoCategorizationService` & `CategoryMatchingEngine` (`UC-04.07`):**
   - Quản lý CRUD quy tắc từ khóa (`CreateRule`, `UpdateRule`, `DeleteRule`, `GetRules`).
   - Hàm `MatchCategoryAsync(string noteOrItemName)`: Quét danh sách quy tắc theo `PriorityOrder` tăng dần để tìm danh mục tương ứng trong thời gian $< 5$ ms.
2. **`ExpenseService` (`UC-03.01 - 03.06`):**
   - `CreateExpenseAsync`: Khấu trừ số dư ví thanh toán (`wallet.Debit(amount)`), ghi nhận giao dịch chi tiêu ACID, hỗ trợ cảnh báo ví âm (`E2`) và vượt hạn mức tín dụng (`E3`).
   - `GetExpensesAsync`: Truy vấn lịch sử chi tiêu timeline đa chiều (theo tháng, theo ví, theo danh mục) (`UC-03.02`).
   - `GetExpenseByIdAsync`: Chi tiết giao dịch kèm danh sách món hàng (`UC-03.03`).
   - `UpdateExpenseAsync`: Hoàn tác số dư ví cũ, khấu trừ số dư ví mới an toàn (`UC-03.04`).
   - `DeleteExpenseAsync`: Hoàn tác cộng lại số dư ví (`UC-03.05`).
   - `LinkExpenseToGoalAsync`: Gắn giao dịch chi tiêu với mục tiêu tích lũy (`UC-03.06`).
3. **API Controllers:**
   - `AutoRulesController`: Các endpoints quản lý quy tắc từ khóa.
   - Mở rộng `TransactionsController` hoặc tạo `ExpensesController` chuyên trách.

#### C. Backend Unit Tests
- `AutoCategorizationRuleTests.cs` (Domain) & `AutoCategorizationServiceTests.cs` (Application): Kiểm thử so khớp từ khóa, thứ tự ưu tiên, regex, và bảo vệ người dùng.
- `ExpenseServiceTests.cs`: Kiểm thử khấu trừ ví ACID, hoàn tác số dư khi sửa/xóa, lọc đa chiều, chặn số tiền $\le 0$.

#### D. Frontend Wireframe (.NET MAUI)
- `AutoCategorizationRulesPage.xaml` & `AutoCategorizationRulesViewModel.cs`: Danh sách quy tắc, form thêm/sửa từ khóa, sắp xếp thứ tự ưu tiên.
- `ExpensesPage.xaml` & `ExpensesViewModel.cs`: Danh sách chi tiêu dòng thời gian, bộ lọc đa tiêu chí, form thêm chi tiêu nhanh khấu trừ ví.
- `ExpenseDetailPage.xaml`: Chi tiết giao dịch chi tiêu, hiển thị danh mục và nút sửa/xóa.

---

### CHẶNG 3B: MULTIMODAL AI OCR HÓA ĐƠN SIÊU THỊ, KIỂM TOÁN SỐ HỌC & TIỆN ÍCH THÔNG MINH (UC-03.07 - 03.11)

#### A. Backend AI Vision OCR & Mathematical Audit
1. **`ReceiptOcrService` (`UC-03.07`):**
   - Tích hợp pipeline Vision OCR bóc tách ảnh chụp hóa đơn siêu thị/bán lẻ.
   - Trích xuất cấu trúc dữ liệu JSON: Tên người bán (`MerchantName`), Ngày giờ, Danh sách món hàng (`ReceiptItems`: Tên món, Số lượng, Đơn giá, Thành tiền), Tiền giảm giá (`DiscountAmount`), Thuế VAT (`TaxAmount`), Tổng tiền thanh toán (`TotalAmount`).
2. **Động cơ Kiểm toán Số học Xác định (`UC-03.08`):**
   - Kiểm toán công thức toán học thuần túy:
     $$\Delta_{\text{receipt}} = \left| \text{TotalAmount} - \left( \sum (\text{Quantity} \times \text{UnitPrice}) - \text{DiscountAmount} + \text{TaxAmount} \right) \right|$$
   - Ngưỡng an toàn: $\Delta \le 1,000$ VND. Nếu $\Delta > 1,000$ VND $\to$ gắn cờ cảnh báo sai lệch màu vàng `IsMathDiscrepancy = true` kèm mô tả sai lệch.
   - Tự động áp dụng `AutoCategorizationService` để gán danh mục mặc định cho từng món hàng.
3. **Tiện ích Thông minh:**
   - `SubscriptionTrapDetector` (`UC-03.09`): Phát hiện các khoản thanh toán định kỳ lặp lại (Netflix, Spotify, iCloud).
   - `VietQrService` (`UC-03.10`): Sinh mã chuyển khoản VietQR chuẩn Napas 247.
   - `VoiceExpenseParser` (`UC-03.11`): Bóc tách khẩu lệnh giọng nói tiếng Việt thành DTO chi tiêu.

#### B. Frontend Wireframe Review & Human-in-the-Loop
- `ReceiptOcrPage.xaml` & `ReceiptOcrViewModel.cs`: 
  - Chọn ảnh hóa đơn từ thư viện/camera.
  - Hiển thị bảng đối soát bóc tách (Tên người bán, Tổng tiền, Bảng chi tiết từng món hàng có ô chỉnh sửa tên/giá/danh mục).
  - Banner cảnh báo sai lệch số học màu vàng/đỏ.
  - Nút chọn ví thanh toán và nút **"Xác nhận ghi vào Sổ cái"** (Human-in-the-loop).
- `VietQrPaymentPage.xaml`: Hiển thị mã QR thanh toán nhanh.

---

## 3. TIÊU CHÍ NGHIỆM THU & ĐẢM BẢO CHẤT LƯỢNG (GATE CRITERIA)
1. **Chất lượng mã nguồn:** `dotnet build` đạt **0 Warning, 0 Error** trên cả Backend và Frontend (.NET MAUI).
2. **Kiểm thử tự động:** Toàn bộ Unit Tests hiện có (78/78 tests) giữ vững màu xanh, bổ sung tối thiểu 15-20 unit tests mới cho Phân hệ 3 và Phân hệ 4, nâng tổng số tests lên $\ge 95$ tests passed 100%.
3. **Tuân thủ DDD & ACID:** Mọi khoản chi tiêu đều khấu trừ số dư ví chính xác nguyên tử, không có hiện tượng mồ côi dữ liệu hoặc sai lệch số dư.
4. **Kiểm soát AI:** AI bóc tách hóa đơn luôn đi kèm kiểm toán công thức số học và màn hình soát xét Human-in-the-loop.
