# Kế Hoạch 20: Rà Soát, Tinh Chỉnh Văn Phong Tự Nhiên, Khắc Phục Lỗi Bố Cục Và Chuẩn Hóa Ngữ Nghĩa Chương 2 & Chương 3

> **Trạng thái:** `PENDING USER APPROVAL`  
> **Thời gian khởi tạo:** 2026-10-02  
> **Mục tiêu:** Rà soát câu chữ, tinh chỉnh văn phong học thuật tự nhiên, loại bỏ các đoạn paraphrase rườm rà, tối nghĩa; khắc phục lỗi lệch hình - chữ (off-by-one) nghiêm trọng tại Mục 3.1.4; cập nhật các liên kết chéo cũ và tối ưu hóa cấu trúc đoạn văn ở Chương 2 và Chương 3 trong [`files/KhoaLuanTotNghiep.docx`](../../files/KhoaLuanTotNghiep.docx).

---

## 1. Bối Cảnh Và Những Vấn Đề Tồn Đọng Cần Giải Quyết

Qua đợt khảo sát chi tiết cấu trúc XML và nội dung thực tế của Chương 2 và Chương 3, hệ thống đã phát hiện các vấn đề ngữ nghĩa và bố cục quan trọng sau:

### 1.1 Lỗi Lệch Hình - Chữ Nghiêm Trọng tại Mục 3.1.4 (Off-by-one Misalignment)
- **Hiện trạng:**
  * Toàn bộ 9 đoạn văn thuyết minh phân hệ Use Case chi tiết từ P[215] đến P[232] bị **lệch một bậc so với các hình**:
    - Trước `Hình 3.2: UCD 01 (Quản lý Tài khoản & Đa Ví)` lại là đoạn văn của *Phân hệ Thu nhập (UCD 02)*.
    - Trước `Hình 3.3: UCD 02 (Thu nhập cá nhân)` lại là đoạn văn của *Phân hệ Chi tiêu & OCR (UCD 03)*.
    - Trước `Hình 3.4: UCD 03 (Chi tiêu & OCR)` lại là đoạn văn của *Phân hệ Danh mục (UCD 04)*.
    - Trước `Hình 3.5: UCD 04 (Danh mục & Quy tắc)` lại là đoạn văn của *Phân hệ Ngân sách (UCD 05)*.
    - Trước `Hình 3.6: UCD 05 (Ngân sách & DSA)` lại là đoạn văn của *Phân hệ Quỹ tích lũy & Nợ (UCD 06)*.
    - Trước `Hình 3.7: UCD 06 (Quỹ tích lũy & Nợ)` lại là đoạn văn của *Phân hệ Báo cáo & Phân tích (UCD 07)*.
    - Trước `Hình 3.8: UCD 07 (Báo cáo & Phân tích)` lại là đoạn văn của *Phân hệ Dự báo Tài chính (UCD 08)*.
    - Trước `Hình 3.9: UCD 08 (Dự báo Tài chính)` lại là đoạn văn của *Phân hệ Cú hích Hành vi (UCD 09)*.
    - Trước `Hình 3.10: UCD 09 (Cú hích Hành vi)` lại là đoạn văn của *Phân hệ Cú hích Hành vi (BỊ DUPLICATE LẶP LẠI 2 LẦN)*.
  * **Hậu quả:** Hoàn toàn thiếu mất đoạn giới thiệu cho Phân hệ UCD 01 (Tài khoản & Đa ví), hình này nói một đằng chữ thuyết minh một nẻo, làm giảm tính chuyên nghiệp của bài báo cáo.

### 1.2 Dẫn Chiếu Chéo Cũ (Stale Cross-References) Sau Khi Chương 1 Đã Tinh Gọn
- Tại P[202]: *"Dựa trên mô hình Input - Process - Output (Mục 1.7)..."* $\rightarrow$ Cần sửa thành **Mục 1.4**.
- Tại P[252]: *"- Giai đoạn 1: Tìm lớp dự kiến: Trích xuất toàn bộ danh từ từ Phát biểu bài toán Mục 1.7 I-P-O..."* $\rightarrow$ Cần sửa thành **Mục 1.4**.

### 1.3 Văn Phong Chương 2: Quá Nhiều Câu Dài Ghép Nối & Thuật Ngữ Hàn Lâm Thiếu Trực Quan
- Nhiều đoạn văn có câu dài 4-5 dòng chỉ có 1 dấu chấm, chứa nhiều tầng mệnh đề quan hệ lồng nhau.
- Chèn từ tiếng Anh trong ngoặc kép hoặc ngoặc đơn giữa chừng câu dày đặc (`Homo Economicus`, `Human-in-the-loop`, `Spatial & Contextual Understanding`, `Schema-Constrained Generation`, `Fungibility`...).
- Một số khái niệm (như Hạch toán tâm lý, DDD chiến thuật) còn nặng tính lý thuyết trừu tượng, cần được diễn đạt bằng văn phong kỹ thuật gãy gọn, gắn liền với bản chất ứng dụng tài chính cá nhân.

### 1.4 Văn Phong & Trình Bày Chương 3: Khối Văn Bản Đặc Quánh Cần Tách Đoạn Mạch Lạc
- Tại Mục 3.1.2: Các yêu cầu chức năng (P[207]) và phi chức năng (P[208]) đang bị nhồi nhét thành hai khối văn bản đặc nghẹt có dấu chấm phẩy dài dòng.
- Tại Mục 3.3.3: Mỗi Bounded Context (từ P[265] đến P[281]) hiện là một đoạn văn duy nhất dài tới 12-15 dòng. Cần tách thành 2 đoạn có cấu trúc:
  * Đoạn 1: Ranh giới nghiệp vụ, các thực thể chính, cấu trúc phân cấp kế thừa hoặc hợp thành.
  * Đoạn 2: Logic vận hành, cơ chế toàn vẹn (ACID, khóa bi quan, bất biến) và các phương thức miền giàu hành vi (Rich Domain Methods).

---

## 2. Phương Án Triển Khai Chi Tiết

### 2.1 Tinh Chỉnh Văn Phong & Diễn Đạt Chương 2
1. **Mục 2.1 (Cơ sở lý thuyết tài chính & kinh tế hành vi):**
   - *2.1.1 Cú hích Thaler:* Viết lại trực diện, giải thích dễ hiểu mâu thuẫn giữa lý trí và cảm xúc, làm rõ tại sao việc tạo "khoảng thời gian trì hoãn cân nhắc (24 - 48 giờ)" lại ngăn chặn được việc mua sắm bốc đồng.
   - *2.1.2 Hạch toán tâm lý:* Dùng ví dụ thực tế sáng sủa (con người dễ tiêu hoang tiền thưởng bất ngờ hơn tiền lương mồ hôi nước mắt); giải thích cơ chế "gán nhãn tiền vào quỹ thiêng liêng" giúp bảo vệ nguồn lực ra sao.
   - *2.1.3 Các trường phái ngân sách:* Diễn đạt súc tích 4 phương pháp (50/30/20, Zero-based, Kakeibo, 6 chiếc hũ), làm nổi bật ưu - nhược điểm và lý do hệ thống cho phép người dùng linh hoạt kết hợp.
   - *2.1.4 Tối ưu hóa nợ (Snowball vs Avalanche):* Trình bày rõ ràng sự tương phản giữa "tối ưu toán học giảm tiền lãi" (Avalanche) và "tối ưu tâm lý kích thích động lực" (Snowball).
2. **Mục 2.2 (Công nghệ AI thị giác & Hậu kiểm toán):**
   - *2.2.1 AI Multimodal Vision OCR:* Nêu bật hạn chế của OCR cổ điển khi gặp hóa đơn nhiệt mờ/nhăn/nghiêng; giải thích khả năng "hiểu bố cục không gian" của mô hình đa phương thức hiện đại một cách trực quan, không dùng từ ngữ rườm rà.
   - *2.2.2 Kỹ thuật Prompt ràng buộc JSON Schema:* Trình bày ngắn gọn về việc ép mô hình AI chỉ xuất JSON hợp lệ để nạp thẳng vào CSDL.
   - *2.2.3 Thuật toán hậu kiểm toán số học:* Trình bày rõ 3 bước kiểm toán (tích từng dòng $\rightarrow$ tổng cộng toàn hóa đơn $\rightarrow$ phân nhánh tự động ghi nhận hoặc gửi cảnh báo người dùng kiểm tra).
3. **Mục 2.3 (Thanh toán số VietQR & NAPAS 247):**
   - Giữ vững tính chuẩn mực của chuẩn TLV (Tag-Length-Value) EMVCo và cơ chế đối soát liên ngân hàng NAPAS 247, cắt gọt các liên từ rườm rà.
4. **Mục 2.4 (DDD, Clean Architecture, Async Queue):**
   - Giải thích bản chất DDD dễ hiểu: Tránh mô hình "thiếu máu" (Anemic - lớp chỉ có dữ liệu mà không có hành vi); làm rõ vai trò của Value Object bất biến (Money), Entity và Aggregate Root.
   - Clean Architecture: Trình bày quy tắc phụ thuộc một chiều hướng tâm, đảm bảo logic tài chính không phụ thuộc framework hay CSDL.
   - Hàng đợi bất đồng bộ: Giải thích trực giác vì sao cuộc gọi AI mất 2-5 giây phải dùng HTTP 202 Accepted và Message Queue để không làm đơ ứng dụng.
5. **Mục 2.5 (Hệ sinh thái công nghệ đề xuất):**
   - Giữ nguyên cấu trúc 5 phân lớp kiến trúc nhưng gọt giũa câu từ cô đọng, dứt khoát.

---

### 2.2 Sửa Lỗi Lệch Hình - Chữ & Chuẩn Hóa Ngữ Nghĩa Chương 3
1. **Khắc phục triệt để lỗi lệch hình ảnh Mục 3.1.4:**
   - **Bổ sung đoạn văn thuyết minh chuẩn cho Phân hệ 1 (UCD 01 - Quản lý Tài khoản & Đa Ví cá nhân)** đặt ngay trước Hình 3.2: Nêu rõ vai trò quản lý tiền mặt, tài khoản ngân hàng, thẻ tín dụng và ví điện tử, áp dụng cơ chế khóa bi quan ACID.
   - **Đưa đoạn Phân hệ 2 (UCD 02 - Quản lý Thu nhập)** về đúng vị trí trước Hình 3.3.
   - **Đưa đoạn Phân hệ 3 (UCD 03 - Ghi nhận Chi tiêu & OCR)** về đúng vị trí trước Hình 3.4.
   - **Đưa đoạn Phân hệ 4 (UCD 04 - Danh mục Thu Chi & Quy tắc thông minh)** về đúng vị trí trước Hình 3.5.
   - **Đưa đoạn Phân hệ 5 (UCD 05 - Ngân sách & Hạn mức DSA)** về đúng vị trí trước Hình 3.6.
   - **Đưa đoạn Phân hệ 6 (UCD 06 - Quỹ Tích lũy & Tối ưu hóa Nợ)** về đúng vị trí trước Hình 3.7.
   - **Đưa đoạn Phân hệ 7 (UCD 07 - Báo cáo & Phân tích Đa chiều)** về đúng vị trí trước Hình 3.8.
   - **Đưa đoạn Phân hệ 8 (UCD 08 - Dự báo Tài chính & Thâm hụt)** về đúng vị trí trước Hình 3.9.
   - **Đưa đoạn Phân hệ 9 (UCD 09 - Cú hích Hành vi Thaler)** về đúng vị trí trước Hình 3.10.
   - **Xóa bỏ hoàn toàn đoạn duplicate lặp lại ở cuối.**
2. **Sửa các liên kết chéo cũ:**
   - Thay toàn bộ "Mục 1.7" thành **"Mục 1.4"** tại P[202] và P[252].
3. **Chuẩn hóa Mục 3.1.2 (Yêu cầu chức năng và phi chức năng):**
   - Viết lại 2 đoạn văn mạch lạc, ngắt câu rõ ràng, loại bỏ cảm giác đọc như một bản sớ liệt kê.
4. **Tối ưu hóa cấu trúc Mục 3.3.3 (Thuyết minh 6 Bounded Contexts):**
   - Chia mỗi Bounded Context thành 2 đoạn văn cân đối, đọc thoáng mắt:
     * Đoạn 1: Ranh giới nghiệp vụ, các lớp thực thể và cấu trúc liên kết kế thừa/hợp thành.
     * Đoạn 2: Logic xử lý miền, các phương thức nghiệp vụ giàu hành vi và quy tắc toàn vẹn dữ liệu.

---

## 3. Kế Hoạch Kiểm Thử & An Toàn Dữ Liệu

1. **Bảo toàn dữ liệu an toàn:**
   - Tạo bản sao lưu dự phòng: `files/KhoaLuanTotNghiep.bak_p20.docx`.
   - Viết và chạy script thử nghiệm trên file tạm `scratch/test_refactored_p20.docx`, kiểm tra toàn bộ thuộc tính XML trước khi ghi đè vào file chính.
2. **Kiểm tra tính bất khả xâm phạm:**
   - Khối thủ tục đầu (P[0] đến P[79]) phải giữ nguyên vẹn 100% (`Discrepancies = 0`).
   - Chương 1 đã tinh gọn (1.1 đến 1.4) và 3 bảng chuẩn (Bảng 1.1, 1.2, 1.3) phải giữ nguyên vẹn 100%.
3. **Mở rộng bộ unit tests tự động (`tests/test_docx_academic_format.py`):**
   - Bổ sung `test_chapter_3_usecase_alignment`: Kiểm tra chính xác 9 đoạn văn thuyết minh trong Mục 3.1.4 phải tương ứng 1:1 với 9 hình Use Case (Hình 3.2 đến 3.10), không có đoạn nào bị lặp.
   - Bổ sung `test_no_stale_references`: Kiểm tra không còn bất kỳ đoạn nào nhắc đến "Mục 1.7" hay "1.7.2".
   - Chạy toàn bộ test suite để đảm bảo **100% tests PASSED**.
4. **Ghi nhật ký quyết định:**
   - Ghi nhận chính thức **Quyết Định 75** vào `project_context/decisions_and_changes_log.md`.

---

## 4. Các Bước Thực Thi Cụ Thể

- [ ] **Bước 1:** Sao lưu file hiện tại thành `files/KhoaLuanTotNghiep.bak_p20.docx`.
- [ ] **Bước 2:** Xây dựng script Python biên tập và tái cấu trúc văn phong Chương 2 và Chương 3:
  * Biên tập lại câu từ của 5 tiểu mục Chương 2 (2.1 đến 2.5).
  * Sửa lỗi lệch hình Use Case tại Mục 3.1.4, bổ sung thuyết minh UCD 01, xóa duplicate UCD 09.
  * Cập nhật dẫn chiếu "Mục 1.7" thành "Mục 1.4".
  * Tách đoạn mạch lạc cho 6 Bounded Contexts tại Mục 3.3.3.
- [ ] **Bước 3:** Chạy thử nghiệm trên `scratch/test_refactored_p20.docx` và kiểm tra xác thực.
- [ ] **Bước 4:** Ghi đè file chính `files/KhoaLuanTotNghiep.docx`.
- [ ] **Bước 5:** Bổ sung unit tests vào `tests/test_docx_academic_format.py` và chạy toàn bộ kiểm thử.
- [ ] **Bước 6:** Ghi nhật ký Quyết Định 75 và cập nhật `walkthrough.md`.
