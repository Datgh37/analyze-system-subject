# IMPLEMENTATION PLAN: PLAN-46.2
## NÂNG CẤP TẦNG NGHIỆP VỤ BÓC TÁCH ĐA PHƯƠNG THỨC: HÓA ĐƠN (RECEIPT OCR) & PHIẾU LƯƠNG (PAYSLIP OCR) VỚI GOOGLE GEMINI 2.5 FLASH MULTIMODAL VISION

> **Mã kế hoạch:** `PLAN-46.2`  
> **Thuộc kế hoạch tổng thể:** [`PLAN-46 (Tích hợp Toàn diện Gemini Multimodal AI thật từ Giai đoạn 1 đến 10)`](2026-10-25_plan_46_tich_hop_toan_dien_gemini_ai_that_tu_giai_doan_1_den_10.md)  
> **Chuyên gia phụ trách:** Agent 08 Andrew Ng (AI Lead), Agent 03 Cockburn BA & Agent 12 Kent Beck (QA Lead)  
> **Ngày lập:** 25/10/2026  
> **Trạng thái:** `COMPLETED (261/261 Tests Passed 100%, 0 Error)`  

---

### 1. BỐI CẢNH & MỤC TIÊU CỐT LÕI

Sau khi Chặng 46.1 hoàn thành xuất sắc việc thiết lập hạ tầng `GeminiApiClient` và cơ chế bảo mật khóa bí mật Zero-Leak GitIgnore, Chặng 46.2 tiến hành bước nâng cấp nghiệp vụ quan trọng nhất của Đề tài: **Chuyển đổi toàn bộ logic bóc tách ảnh Hóa đơn và Phiếu lương từ mô phỏng sang sử dụng Google Gemini 2.5 Flash Multimodal Vision thật sự**.

Mục tiêu cụ thể:
1. **Kết nối `ReceiptOcrService` (UC-03.07/03.08) với Gemini Vision:**
   - Khi người dùng tải lên ảnh chụp hóa đơn (siêu thị WinMart, Co.opmart, Bách Hóa Xanh, nhà hàng, hoặc Hóa đơn điện tử EVN/Nước sinh hoạt dạng PDF/ảnh), service gửi file ảnh Base64 sang Gemini 2.5 Flash qua `IGeminiApiClient`.
   - Sử dụng System Prompt và Strict JSON Schema để yêu cầu Gemini trích xuất chính xác: Tên đơn vị bán lẻ (`merchantName`), Ngày hóa đơn (`invoiceDate`), Tiền thuế VAT (`taxAmount`), Tiền chiết khấu (`discountAmount`), Tổng tiền thanh toán (`totalAmount`), cùng mảng danh sách chi tiết từng mặt hàng (`items: [ { name, quantity, unitPrice, totalPrice } ]`).
2. **Kết nối `PayslipOcrService` (UC-02.08) với Gemini Vision:**
   - Trích xuất tự động từ ảnh phiếu lương/bảng lương các trường: Tên người sử dụng lao động (`employerName`), Kỳ lương (`payPeriod`), Lương gộp Gross (`grossSalary`), Lương thực nhận Net (`netSalary`), các khoản trích đóng bảo hiểm (BHXH, BHYT, BHTN), Thuế thu nhập cá nhân (TNCN), Phụ cấp (`allowances`), Thưởng (`bonus`) và Giảm trừ khác (`otherDeductions`).
3. **Cơ Chế Khử Ảo Giác Tuyệt Đối (Zero-Hallucination via Mathematical Audit):**
   - Dữ liệu bóc tách từ Gemini AI sẽ được chuyển qua tầng **Mathematical Audit Engine** của hệ thống để xác thực lại tính cân đối số học:
     * Hóa đơn: Kiểm tra sai lệch $\Delta = |Total - (\sum Items - Discount + Tax)| \le 1.000$ đ.
     * Phiếu lương: Kiểm tra sai lệch $\Delta = |Net - (Gross - TotalDeductions + TotalAdditions)| \le 1.000$ đ.
   - Nếu có chênh lệch $\rightarrow$ Tự động bật cờ cảnh báo `hasDiscrepancy = true` cho người dùng rà soát, đúng chuẩn quy định IEEE 29148.
4. **Cơ Chế Fallback Đa Tầng (Fault-Tolerant Resilience):**
   - Nếu API Key chưa được cấu hình, mạng mất kết nối hoặc hết Quota $\rightarrow$ Service tự động ghi nhận cảnh báo và rơi về chế độ xử lý an toàn (Safe Fallback), đảm bảo máy chủ không bao giờ bị Crash.
   - Giữ vững 100% tương thích ngược với các bộ dữ liệu kiểm thử mock hiện tại.

---

### 2. DANH MỤC CÁC TỆP MÃ NGUỒN LIÊN QUAN

| STT | Đường Dẫn Tệp | Hành Động | Trách Nhiệm Chi Tiết |
| :---: | :--- | :---: | :--- |
| 1 | `backend/ExpenseTracker.Application/Services/ReceiptOcrService.cs` | Refactor | Bơm `IGeminiApiClient`, xây dựng Prompt Vision hóa đơn, bóc tách JSON từ Gemini, đẩy qua Mathematical Audit & Auto-Categorization. |
| 2 | `backend/ExpenseTracker.Application/Services/PayslipOcrService.cs` | Refactor | Bơm `IGeminiApiClient`, xây dựng Prompt Vision bảng lương, bóc tách các khoản bảo hiểm/thuế/lương thực nhận, đẩy qua Mathematical Audit. |
| 3 | `backend/tests/ExpenseTracker.Application.UnitTests/Services/ReceiptOcrServiceTests.cs` | Bổ sung | Thêm các Unit Tests mới kiểm thử tích hợp `Mock<IGeminiApiClient>` với phản hồi trích xuất hóa đơn thật. |
| 4 | `backend/tests/ExpenseTracker.Application.UnitTests/Services/PayslipOcrServiceTests.cs` | Bổ sung | Thêm các Unit Tests mới kiểm thử tích hợp `Mock<IGeminiApiClient>` với phản hồi trích xuất phiếu lương thật. |

---

### 3. THIẾT KẾ KỸ THUẬT & PROMPT ENGINEERING

#### 3.1. Thiết Kế System Prompt Bóc Tách Hóa Đơn (`ReceiptOcrService`)
```text
Bạn là chuyên gia thị giác máy tính và kế toán tài chính (AI Vision OCR Auditor). Nhiệm vụ của bạn là phân tích hình ảnh hóa đơn mua sắm hoặc hóa đơn điện tử (EVN, Cấp nước, Siêu thị, Cửa hàng tiện lợi) và trích xuất dữ liệu với độ chính xác số học cao nhất.

YÊU CẦU TRẢ VỀ:
Trả về duy nhất định dạng JSON (không có markdown code block, không có lời dẫn) theo schema sau:
{
  "merchantName": "Tên cửa hàng / siêu thị / đơn vị xuất hóa đơn",
  "invoiceDate": "YYYY-MM-DDTHH:mm:ssZ (nếu không rõ giờ thì dùng 00:00:00Z)",
  "taxAmount": 0,
  "discountAmount": 0,
  "totalAmount": 0,
  "confidence": 0.95,
  "items": [
    {
      "name": "Tên sản phẩm/dịch vụ",
      "quantity": 1.0,
      "unitPrice": 0,
      "totalPrice": 0
    }
  ]
}
QUY TẮC SỐ HỌC:
- Nếu hóa đơn có thuế VAT hoặc chiết khấu, hãy tách riêng vào taxAmount và discountAmount.
- totalAmount phải là số tiền thanh toán cuối cùng của khách hàng.
```

#### 3.2. Thiết Kế System Prompt Bóc Tách Phiếu Lương (`PayslipOcrService`)
```text
Bạn là chuyên gia phân tích chứng từ nhân sự và bảng lương Việt Nam. Nhiệm vụ của bạn là bóc tách các trường tài chính từ ảnh chụp phiếu lương hoặc file PDF sao kê thu nhập.

YÊU CẦU TRẢ VỀ:
Trả về duy nhất định dạng JSON theo schema:
{
  "employerName": "Tên công ty / cơ quan chi trả lương",
  "payPeriod": "MM/YYYY (tháng/năm của kỳ lương)",
  "grossSalary": 0,
  "netSalary": 0,
  "socialInsurance": 0,
  "healthInsurance": 0,
  "unemploymentInsurance": 0,
  "personalIncomeTax": 0,
  "allowances": 0,
  "bonus": 0,
  "otherDeductions": 0,
  "confidence": 0.96
}
QUY TẮC:
- Tự động nhận diện các mục BHXH (8%), BHYT (1.5%), BHTN (1%) và Thuế TNCN.
- netSalary là thực lĩnh chuyển khoản.
```

---

### 4. KỊCH BẢN KIỂM THỬ (TEST PLAN) & TIÊU CHÍ CHẤP NHẬN

1. **Bảo Toàn Bộ Kiểm Thử Hiện Tại:**
   - Toàn bộ 259 tests hiện tại phải tiếp tục **PASSED 100%**.
2. **Kiểm Thử Mới Cho Luồng Gemini Vision:**
   - Viết các test case trong `ReceiptOcrServiceTests` mô phỏng Gemini trả về chuỗi JSON bóc tách hóa đơn $\rightarrow$ Kiểm tra service map đúng các trường DTO, tính toán số học chính xác.
   - Viết các test case trong `PayslipOcrServiceTests` mô phỏng Gemini trả về chuỗi JSON bảng lương $\rightarrow$ Kiểm tra các khoản khấu trừ BHXH/BHYT/Thuế TNCN và lương thực nhận.
   - Kiểm tra kịch bản biên khi Gemini trả về lỗi hoặc phản hồi chuỗi rỗng $\rightarrow$ Service fallback an toàn mà không bị crash.
3. **Thử Nghiệm Trực Quan Trên Swagger (Live Integration):**
   - Sau khi hoàn thành, Người dùng có thể bật Backend, mở Swagger (`POST /api/v1/receipts/ocr-parse` hoặc `POST /api/v1/recurring-incomes/ocr-payslip`), chọn một file ảnh hóa đơn thật trên máy và nhấn Execute.
   - Terminal sẽ in màu toàn văn Raw JSON mà Google Gemini thật trả về trực tiếp trước mắt Người dùng!
