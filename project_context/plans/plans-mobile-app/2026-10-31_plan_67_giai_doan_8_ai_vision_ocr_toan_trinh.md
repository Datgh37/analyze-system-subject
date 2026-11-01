# KẾ HOẠCH TRIỂN KHAI GIAI ĐOẠN MỚI 08: AI VISION OCR HÓA ĐƠN TOÀN TRÌNH — CAMERA NATIVE, LÀM NÉT & GEMINI THẬT (UC-03.07 & UC-03.08)

> **Mã kế hoạch:** `PLAN-67`  
> **Thời gian khởi tạo:** 31/10/2026  
> **Trạng thái:** Chờ phê duyệt (Pending Approval)  
> **Áp dụng theo:** Master Project Roadmap (Giai đoạn Mới 08, Khối 3), Đặc tả UC-03.07 & UC-03.08 (`project_context/reports/use_cases/03_dac_ta_uc_chi_tieu_va_ai_ocr.md`), Sơ đồ UCD Tab 3 & Class Diagram Lab 3 (`ReceiptItem`, `Transaction`, `AutoCategorizationRule`).

---

## 1. BỐI CẢNH & MỤC TIÊU NGHIỆP VỤ

### 1.1. Hiện trạng sau Giai đoạn Mới 07
- Hệ thống đã hoàn thành 100% Khối 1 (Nền tảng & Xác thực) và Khối 2 (Lõi Nghiệp vụ Tài chính PFM Backend & Toán học) với **475 / 475 Unit Tests PASSED 100%**:
  - Giai đoạn 01: Chuẩn hóa Soft Delete, Audit Logs, SignalR Notification Hub.
  - Giai đoạn 02: Sinh trắc học Biometrics, SecureStorage & Quick Login.
  - Giai đoạn 03: Phân hệ Quản lý Danh mục Nợ (`Debts`).
  - Giai đoạn 04: Động cơ Trả nợ Tối ưu Snowball vs Avalanche DAG.
  - Giai đoạn 05: Quỹ Khẩn Cấp (`EmergencyFund`) & Báo cáo Net Worth.
  - Giai đoạn 06: Quét & Cảnh báo Bẫy Đăng ký Định kỳ (`Subscriptions`).
  - Giai đoạn 07: Mô phỏng Dòng tiền Biến cố & Stress-testing (`What-If`).
- Tuy nhiên, tại phân hệ Trí tuệ Nhân tạo Thị giác OCR Hóa đơn (`ReceiptOcrService` & `ReceiptOcrPage` — `UC-03.07` & `UC-03.08`):
  1. **Chưa có tính năng chụp ảnh Camera Native:** Trên Mobile MAUI hiện chỉ có duy nhất nút "Chọn Tệp" qua `FilePicker.Default.PickAsync()`, thiếu hoàn toàn nút mở Camera điện thoại chụp hóa đơn trực tiếp (`MediaPicker.Default.CapturePhotoAsync()`) và chưa cấp quyền Camera trong `AndroidManifest.xml`.
  2. **Thiếu hiển thị ảnh xem trước (Receipt Preview Image):** Khi người dùng chụp hoặc chọn ảnh hóa đơn, giao diện chưa hiển thị ảnh thumbnail/preview để người dùng đối chiếu mắt thường với bảng bóc tách.
  3. **Tiền xử lý ảnh còn là stub:** Lớp `ImagePreProcessingService` trên Backend hiện tại chỉ sao chép Stream cơ bản, chưa có khâu thẩm định tính hợp lệ của tệp ảnh, định dạng MIME, kích thước giới hạn và chuẩn hóa trước khi đẩy sang AI Vision.
  4. **Chưa có giao diện chỉnh sửa linh hoạt khi có sai lệch số học (UC-03.08 Flow A1):** Khi thuật toán kiểm toán phát hiện chênh lệch $\Delta > 1,000$ VND, người dùng cần có khả năng chạm sửa trực tiếp số lượng, đơn giá, thành tiền của từng món hàng hoặc xóa/thêm dòng để hệ thống tự động tính lại $\Delta$ tức thì (0ms cảm nhận) mà không cần gọi lại OCR.
  5. **Chưa có bộ Unit Tests trên Frontend Mobile:** Chưa có `ReceiptOcrViewModelTests.cs` trong `ExpenseTracker.Maui.UnitTests` để bảo đảm độ tin cậy của toàn bộ luồng bóc tách, chụp ảnh, kiểm toán và xác nhận lưu sổ cái.

### 1.2. Mục tiêu Giai đoạn Mới 08
Xây dựng hoàn chỉnh **Pipeline AI Vision OCR Toàn Trình từ Camera Native đến Sổ Cái Tài Chính**:
1. **Camera Native Capture (`MediaPicker`):**
   - Hỗ trợ 2 phương thức nhập liệu trực quan: (1) Bấm "Chụp Ảnh Hóa Đơn" mở Camera điện thoại chụp trực tiếp; (2) Bấm "Tải Hóa Đơn" chọn ảnh/PDF từ thư viện máy.
   - Quản lý quyền truy cập Camera (`Permissions.Camera`) theo chuẩn Android Runtime Permissions.
   - Bổ sung cấu hình quyền Camera trong `AndroidManifest.xml`.
2. **Hiển thị Ảnh Chụp & Tiền Xử Lý Ảnh (Preview & Pre-processing):**
   - Hiển thị ảnh chụp hóa đơn xem trước trên Mobile UI với góc bo mềm mại, cho phép người dùng quan sát rõ hóa đơn gốc.
   - Nâng cấp `ImagePreProcessingService` trên Backend: Kiểm tra kích thước tối đa (10MB), xác thực định dạng ảnh (.jpg, .jpeg, .png, .pdf), chuẩn hóa buffer stream và ghi log kiểm toán xử lý.
3. **Động cơ Trích Xuất Gemini 2.5 Flash Vision Thật & Fallback Thông Minh:**
   - Hoàn thiện prompt và system instruction chuyên sâu cho thị giác máy tính tài chính:
     * Hóa đơn siêu thị bán lẻ: WinMart, Co.opmart, Bách Hóa Xanh, Lotte Mart, Big C, Circle K, GS25... bóc tách chi tiết từng dòng mặt hàng, đơn giá, số lượng, thuế VAT, chiết khấu và tổng thanh toán.
     * Hóa đơn tiền điện EVN & Hóa đơn nước sạch: Bóc tách đơn vị phát hành, chỉ số tiêu thụ (kWh, $m^3$), kỳ hóa đơn và tự động map vào danh mục Tiện ích/Hóa đơn.
   - Cơ chế Graceful Fallback: Khi có API Key hợp lệ sẽ gọi Gemini REST API thật; khi không có API Key hoặc gặp lỗi mạng/quota, tự động kích hoạt bộ parser heuristic dự phòng chuẩn xác, bảo đảm ứng dụng không bao giờ bị văng hay gián đoạn.
4. **Hậu Kiểm Toán Số Học Thời Gian Thực (UC-03.08 Deterministic Mathematical Audit):**
   - Thuật toán kiểm tra 2 bước:
     * Bước 1: $\text{totalPrice}[i] = \text{quantity}[i] \times \text{unitPrice}[i]$.
     * Bước 2: $\Delta = |\sum \text{totalPrice}[i] + \text{taxAmount} - \text{discountAmount} - \text{totalAmount}|$.
   - Nếu $\Delta \le 1,000$ VND: Gán `auditStatus = MATCHED` (Huy hiệu xanh lá: "Khớp số học 100%").
   - Nếu $\Delta > 1,000$ VND: Gán `auditStatus = DISCREPANCY` (Huy hiệu cảnh báo: Chỉ rõ số tiền lệch và highlight các dòng nghi vấn).
   - Ngoại lệ E1 (Hóa đơn rách/mờ tổng tiền): Tự động lấy tổng các món hàng cộng thuế làm giá trị đề xuất.
5. **Giao Diện Chỉnh Sửa Món Hàng Tương Tác 0ms (In-Place Item Editing):**
   - Cho phép người dùng chỉnh sửa trực tiếp số lượng, giá tiền, xóa món hoặc thêm món mới ngay trên giao diện Mobile.
   - ViewModel tự động tính toán lại $CalculatedTotal$ và $\Delta$ ngay trên bộ nhớ (0ms), tự động chuyển trạng thái sang xanh khi số liệu khớp.
6. **Xác Nhận Vào Sổ Cái & Vòng Lặp Học Luật Tự Động (`RememberRules`):**
   - Khởi tạo giao dịch chi tiêu chính thức, trừ số dư ví nguồn với khóa bi quan ACID.
   - Nếu bật `RememberRules`, tự động tạo các quy tắc phân loại tự động (`AutoCategorizationRule`) cho các từ khóa mặt hàng mới.
7. **Kiểm Thử Toàn Trình:**
   - Viết thêm bài kiểm thử Backend cho tiền xử lý ảnh và bóc tách hóa đơn.
   - Tạo mới `ReceiptOcrViewModelTests.cs` (6+ bài tests bao phủ Camera, Picker, Math Audit, Item Edit, Confirm Ledger).
   - Nâng tổng số tests toàn hệ thống lên **485+ tests PASSED 100%**.

---

## 2. KIẾN TRÚC & QUY TRÌNH VẬN HÀNH (PIPELINE FLOW)

```
[ NGƯỜI DÙNG ]
    │
    ├── 1. Bấm [Chụp Ảnh Hóa Đơn] ──> MediaPicker (Camera Native) ──┐
    │                                                               │
    └── 2. Bấm [Tải Từ Thư Viện]  ──> FilePicker (Gallery/PDF)   ──┤
                                                                   ▼
                                                   [ Lưu Stream & Hiển Thị Preview ]
                                                                   │
                                                                   ▼
                                                  [ Gửi Multipart/form-data ]
                                                  POST /api/receipts/parse
                                                                   │
                                                                   ▼
                                              [ ImagePreProcessingService ]
                                            (Validate Format, Size, Normalize)
                                                                   │
                                                                   ▼
                                               [ Gemini 2.5 Flash Vision ]
                                             (Multimodal AI REST API Gateway)
                                                                   │
                                                                   ▼
                                             [ Mathematical Audit Engine ]
                                        Δ = |(Σ Items + Tax - Discount) - Total|
                                                                   │
                                 ┌─────────────────────────────────┴─────────────────────────────────┐
                                 ▼                                                                   ▼
                    [ Δ ≤ 1,000 đ : MATCHED ]                                        [ Δ > 1,000 đ : DISCREPANCY ]
                   Huy hiệu xanh: Khớp 100%                                         Huy hiệu vàng: Cảnh báo lệch
                                 │                                                                   │
                                 │                                                    [ Người Dùng Sửa Ô Món Hàng ]
                                 │                                                    (Recalculate In-Memory 0ms)
                                 │                                                                   │
                                 └─────────────────────────────────┬─────────────────────────────────┘
                                                                   ▼
                                                     [ Xác Nhận Lưu Chi Tiêu ]
                                                    POST /api/receipts/confirm
                                                                   │
                                                                   ▼
                                             [ TransactionService & ACID Debit Wallet ]
                                             [ Auto-Learning: Create AutoRules ]
```

---

## 3. KẾ HOẠCH TRIỂN KHAI TỪNG BƯỚC

### Bước 1: Cấu hình Quyền Native Android (`AndroidManifest.xml`)
- Mở tệp `mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/Platforms/Android/AndroidManifest.xml`.
- Bổ sung quyền Camera và khai báo phần cứng camera (không bắt buộc để chạy được trên máy ảo):
  ```xml
  <!-- Native Camera Permission for Receipt AI Vision OCR (UC-03.07) -->
  <uses-permission android:name="android.permission.CAMERA" />
  <uses-feature android:name="android.hardware.camera" android:required="false" />
  <uses-feature android:name="android.hardware.camera.autofocus" android:required="false" />
  ```

### Bước 2: Nâng cấp Backend Image Pre-Processing & Robust AI Vision
1. **Nâng cấp `ImagePreProcessingService.cs`:**
   - Thẩm định luồng đầu vào: Kiểm tra `stream.Length`, giới hạn dung lượng tối đa 10MB (tránh DoS tệp quá lớn).
   - Xác thực phần mở rộng tệp hợp lệ (`.jpg`, `.jpeg`, `.png`, `.pdf`).
   - Chuẩn hóa stream, ghi log kích thước ảnh và thời gian chuẩn bị.
2. **Nâng cấp `ReceiptOcrService.cs`:**
   - Tối ưu hóa prompt bóc tách: Yêu cầu Gemini phân biệt rõ các dòng sản phẩm, đơn giá, số lượng, tiền thuế VAT, chiết khấu và tổng thanh toán.
   - Xử lý mượt mà hóa đơn tiền điện (EVN) và nước sinh hoạt (Sawaco/Cấp nước), tự động phân loại vào nhóm danh mục Tiện ích/Hóa đơn.
   - Hoàn thiện xử lý Ngoại lệ E1: Nếu tổng thanh toán bị rách mờ, tự động gán $TotalAmount = \sum \text{TotalPrice} + \text{TaxAmount} - \text{DiscountAmount}$ và đính kèm cờ cảnh báo đề xuất để người dùng xác nhận.
   - Cơ chế Fallback an toàn: Khi không có ApiKey hoặc lỗi mạng bên thứ 3, bóc tách dựa trên mẫu hóa đơn bách hóa thông minh, đảm bảo test suite và ứng dụng luôn hoạt động trơn tru.

### Bước 3: Mở rộng Mobile ViewModel & Repositories (`ExpenseTracker.Maui`)
1. **Mở rộng `ReceiptOcrViewModel.cs`:**
   - Bổ sung thuộc tính quan sát:
     * `ImageSource? CapturedImagePreview`: Hiển thị ảnh hóa đơn vừa chụp/chọn.
     * `string? CapturedImageBase64`: Chuỗi base64 ảnh phục vụ lưu trữ tạm.
     * `bool HasCapturedImage`: Cờ hiển thị khung ảnh xem trước.
   - Thêm lệnh **`CaptureReceiptWithCameraAsync`**:
     * Kiểm tra trạng thái quyền Camera (`Permissions.CheckStatusAsync<Permissions.Camera>()`), nếu chưa cấp thì yêu cầu quyền (`Permissions.RequestAsync<Permissions.Camera>()`).
     * Gọi `MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions { Title = "Chụp ảnh hóa đơn mua sắm" })`.
     * Khi người dùng chụp xong: Đọc stream ảnh, gán vào `CapturedImagePreview`, tạo `StreamPart` và gọi bóc tách OCR.
   - Thêm các lệnh tương tác trên bảng món hàng (In-Place Item Editing):
     * `UpdateItemLine(ParsedReceiptItemModel item)`: Cập nhật thành tiền $TotalPrice = Quantity \times UnitPrice$ và tự động tính lại tổng $CalculatedTotal$, $\Delta$ tức thì 0ms.
     * `RemoveItemLine(ParsedReceiptItemModel item)`: Xóa một món thừa và tính lại $\Delta$.
     * `AddItemLine()`: Thêm một dòng món hàng trống để người dùng nhập bổ sung.
2. **Cập nhật `IReceiptRepository`, `ApiReceiptRepository`, `MockReceiptRepository`:**
   - Bảo đảm hỗ trợ đầy đủ `ParseReceiptAsync(StreamPart file)` và `ConfirmReceiptAsync(ConfirmReceiptModel request)`.
   - `MockReceiptRepository`: Cung cấp dữ liệu mẫu WinMart chuẩn xác phục vụ chạy demo Offline 0ms.

### Bước 4: Nâng cấp Giao diện `ReceiptOcrPage.xaml`
1. **Thiết kế lại Khu vực Hành động Chụp/Chọn Ảnh:**
   - Thiết kế 2 nút hành động lớn trực quan cạnh nhau:
     * Nút 1: **"Chụp Hóa Đơn"** (Camera icon, màu tím gradient Fintech, kích hoạt `CaptureReceiptWithCameraCommand`).
     * Nút 2: **"Tải Từ Thư Viện"** (Folder/Image icon, màu xanh viền mờ, kích hoạt `PickAndParseReceiptCommand`).
2. **Thêm Thẻ Ảnh Xem Trước (Receipt Image Preview Card):**
   - Khung hình bo tròn mềm mại (`RoundRectangle 16`), hiển thị ảnh hóa đơn vừa chụp kèm nút bấm "Chụp lại / Đổi ảnh".
3. **Nâng cấp Bảng Món Hàng & Thẻ Kiểm Toán Số Học:**
   - Huy hiệu trạng thái kiểm toán: Xanh lá ("✅ Kiểm toán số học khớp 100%") hoặc Vàng cam ("⚠️ Phát hiện chênh lệch X đ").
   - Hiển thị danh sách món hàng dạng Card/Grid với các ô Entry cho phép sửa trực tiếp số lượng và đơn giá.
   - Nút xóa dòng (thùng rác nhỏ) và nút "Thêm món hàng" linh hoạt.
   - Thẻ xác nhận lưu vào sổ cái kèm nút chọn ví thanh toán và checkbox `RememberRules`.

### Bước 5: Xây dựng Bộ Kiểm Thử Tự Động (Unit Tests)
1. **Backend Tests (`ExpenseTracker.Application.UnitTests`):**
   - Bổ sung trong `ReceiptOcrServiceTests.cs`:
     * `ParseReceiptAsync_WithUtilityBill_ShouldMapToUtilityCategoryAndAuditMath`: Kiểm thử bóc tách hóa đơn điện/nước và map đúng danh mục.
     * `ParseReceiptAsync_WhenTotalAmountMissing_ShouldFallbackToSumOfItems_ExceptionE1`: Kiểm thử ngoại lệ E1 khi tổng tiền bị mờ.
     * `ImagePreProcessingService_WithValidImage_ShouldReturnSanitizedStream`: Kiểm thử dịch vụ tiền xử lý ảnh.
2. **Frontend Tests (`ExpenseTracker.Maui.UnitTests`):**
   - Tạo mới `ReceiptOcrViewModelTests.cs`:
     * `LoadReferenceData_ShouldPopulateWalletsAndCategories`: Kiểm thử nạp dữ liệu ví và danh mục.
     * `PickAndParseReceipt_WhenSuccessful_ShouldSetParsedDataAndValidateMath`: Kiểm thử chọn tệp và bóc tách thành công.
     * `CaptureReceiptWithCamera_WhenCameraAvailable_ShouldParseAndSetPreview`: Kiểm thử luồng chụp camera.
     * `ItemLineEdited_ShouldRecalculateTotalsAndDiscrepancyInstantly`: Kiểm thử chỉnh sửa món hàng tính lại $\Delta$ tức thì 0ms.
     * `ConfirmReceipt_WhenValid_ShouldCallRepoAndNavigateSuccess`: Kiểm thử xác nhận lưu sổ cái và tạo rule.
3. **Thực thi toàn bộ Test Suite:**
   - Chạy `dotnet test ExpenseTracker.Domain.UnitTests`.
   - Chạy `dotnet test ExpenseTracker.Application.UnitTests`.
   - Chạy `dotnet test ExpenseTracker.Maui.UnitTests`.
   - **Cam kết:** 100% tests PASSED (Tổng số tests dự kiến: **485+ tests**).

---

## 4. MA TRẬN PHÂN CÔNG TÁC TỬ HỘI ĐỒNG

| Tác tử | Vai trò | Trách nhiệm cụ thể trong Giai đoạn 08 |
| :--- | :--- | :--- |
| **Agent 08 (Andrew Ng)** | AI Pioneer & Multimodal Vision | Thiết kế cấu trúc JSON Schema trích xuất, Prompt engineering cho Gemini 2.5 Flash Vision, thuật toán tiền xử lý ảnh và cơ chế phân loại hóa đơn tiện ích. |
| **Agent 03 (Cockburn BA)** | Phân tích Nghiệp vụ Lead BA | Bảo đảm quy trình bóc tách và hậu kiểm toán số học tuân thủ 100% đặc tả `UC-03.07` và `UC-03.08` (Basic Flow, Alternative Flow A1 và Exception Flow E1). |
| **Agent 04 (Eric Evans DDD)** | Kiến trúc Domain & Service | Quản lý mối quan hệ giữa `ReceiptItem`, `Transaction` và `AutoCategorizationRule`, bảo đảm tính toàn vẹn nghiệp vụ hạch toán kép. |
| **Agent 09 & 10 (Brad Frost & Dan Abramov)** | UI/UX & Frontend Architecture | Thiết kế giao diện chụp ảnh Camera Native, thẻ xem trước hóa đơn, tương tác chỉnh sửa món hàng 0ms và xử lý quyền Android Runtime Permissions. |
| **Agent 12 (Kent Beck QA)** | Chiến lược Kiểm thử | Thiết lập bộ test cases bao phủ toàn bộ pipeline từ tiền xử lý, kiểm toán số học đến ViewModel trên Mobile. |
| **Agent 00 (Orchestrator)** | Điều phối & Tổng hợp | Tổng duyệt chất lượng, ghi nhận quyết định vào nhật ký và cập nhật Roadmap. |

---

## 5. KẾT QUẢ ĐẦU RA KỲ VỌNG & TIÊU CHÍ HOÀN THÀNH

1. Ứng dụng Mobile MAUI mở được Camera Native trên điện thoại để chụp trực tiếp hóa đơn và hiển thị ảnh xem trước sắc nét.
2. Pipeline AI Vision bóc tách thành công cả hóa đơn siêu thị bán lẻ lẫn hóa đơn điện nước EVN, có fallback thông minh khi mất mạng/thiếu key.
3. Thuật toán kiểm toán số học $\Delta$ chạy tức thời, phát hiện chuẩn xác chênh lệch và hỗ trợ người dùng chỉnh sửa từng ô món hàng với phản hồi 0ms.
4. Ghi nhận giao dịch vào sổ cái tài chính thành công, trừ số dư ví chuẩn xác và tự động học luật từ khóa khi bật `RememberRules`.
5. Toàn bộ Unit Tests trên Backend và Mobile chạy thành công 100% (0 fail, 0 error), đưa tổng số test suite đạt **485+ tests**.
6. Ghi nhận snapshot plan vào `project_context/plans/plans-mobile-app/` và cập nhật `decisions_and_changes_log.md`.

