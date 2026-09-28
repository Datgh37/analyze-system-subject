# 📋 BỘ CÂU HỎI & HƯỚNG DẪN KHẢO SÁT HIỆN TRẠNG CHUYÊN BIỆT
## ĐỀ TÀI: HỆ THỐNG QUẢN LÝ CHI TIÊU & LẬP NGÂN SÁCH CÁ NHÂN / NHÓM (SMART EXPENSE & BUDGET MANAGER)
### 🏛️ HỘI ĐỒNG PHẢN BIỆN CHUYÊN GIA (THE 13 COUNCIL PERSONAS PERSPECTIVE)

> **Mục đích tài liệu:**  
> Tài liệu này được xây dựng dựa trên sự cố vấn và góc nhìn phản biện chuyên sâu từ **Hội đồng 13 Chuyên gia Danh nhân (Agent Council)** của dự án (`agents/core/`), nhằm cung cấp cho bạn một **Cẩm nang phỏng vấn & Bộ câu hỏi tự học khảo sát hiện trạng** toàn diện nhất cho bài **Lab 1 - Phân tích Thiết kế Hệ thống Hướng đối tượng (OOAD)**.

---

```
┌────────────────────────────────────────────────────────────────────────────────────────┐
│             HỘI ĐỒNG CHUYÊN GIA ĐỒNG HÀNH TRONG BỘ CÂU HỎI KHẢO SÁT                     │
├────────────────────────────────────────────────────────────────────────────────────────┤
│ 1. GS. Richard Thaler (Nobel Kinh tế)      : Tâm lý học hành vi, Nudge & Mental Accounting│
│ 2. Household Finance Specialist (Agent 02) : Quỹ chung gia đình, Co-living & Hoàn ứng   │
│ 3. Alistair Cockburn & Debra Paul (Agent 03): Use Case Hierarchy, Actor-Goals & Ngoại lệ │
│ 4. Eric Evans (Agent 04 - Cha đẻ DDD)     : Ubiquitous Language, Entities & Value Object│
│ 5. Martin Kleppmann (Agent 05 - DDIA)     : Toàn vẹn CSDL, Bù trừ công nợ & Concurrency │
│ 6. Robert Tarjan (Agent 06 - Đồ thị)      : Lý thuyết Đồ thị rút gọn nợ chéo             │
│ 7. Andrew Ng (Agent 08 - AI Vision)       : Bóc tách hóa đơn OCR & Trích xuất tự động    │
│ 8. Bruce Schneier (Agent 11 - Fintech Sec): Bảo mật tài chính, Xác thực VietQR 1-chạm   │
└────────────────────────────────────────────────────────────────────────────────────────┘
```

---

## 🧭 HƯỚNG DẪN CÁCH TỰ THỰC HIỆN KHẢO SÁT
Khi bạn tự khảo sát (hoặc đóng vai phỏng vấn bạn bè, sinh viên cùng phòng trọ, người đi làm):
1. **Góc nhìn quản lý & Đời sống thực tế:** Đặt câu hỏi về thói quen, cách xử lý đời thường, không dùng thuật ngữ kỹ thuật CNTT.
2. **Kỹ thuật đào sâu (5 Whys):** Luôn hỏi *"Tại sao?"*, *"Khi nào?"*, *"Ai làm?"*, *"Gặp khó khăn gì?"* sau mỗi câu trả lời.
3. **Tư duy hướng đối tượng (OOAD):** Luôn chú ý các **Danh từ** (gợi ý Đối tượng/Thực thể) và các **Động từ** (gợi ý Hành vi/Phương thức).

---

## PHẦN I: KHẢO SÁT CƠ CẤU TỔ CHỨC & VAI TRÒ DƯỚI GÓC NHÌN CHUYÊN GIA

### 1.1. Góc Nhìn Chuyên Gia Tài Chính Gia Đình & Nhóm Chi Tiêu (Agent 02 - Household Specialist)
* **Câu hỏi 1 (Đa dạng mô hình nhóm):** Trong thực tế, bạn đã từng tham gia vào những hình thái nhóm chi tiêu nào sau đây?
  * *Mô hình 1 - Phòng trọ / Co-living:* Tiền phòng riêng + tiền điện nước, dịch vụ chia chung.
  * *Mô hình 2 - Gia đình (Household):* Vợ chồng đóng góp Quỹ chung (Joint Pool) theo tỷ lệ 60-40, chi trả sinh hoạt phí và cấp hạn mức tiêu vặt (Allowance Quota) cho con cái.
  * *Mô hình 3 - Sự kiện & Du lịch (Trips):* Ăn uống chia theo món thực tế tiêu dùng (*Itemized Split*) hoặc chia đều.
  * *Mô hình 4 - CLB & Đội nhóm dự án:* Một cá nhân chi hộ việc chung, nộp hóa đơn về quỹ để được hoàn tiền (*Advance & Reimbursement*).
* **Câu hỏi 2 (Cơ chế Quỹ chung vs Chi hộ):** Nhóm của bạn thích hình thức nào hơn: (A) Mọi người đóng tiền vào một "Quỹ chung" trước rồi tiêu dần, hay (B) Đến bữa ai có tiền thì trả trước rồi chia nợ sau? Tại sao?

### 1.2. Phân Quyền & Vai Trò (Organizational Roles)
* **Câu hỏi 3:** Trong nhóm chi tiêu, ai là người đóng vai trò "Trưởng nhóm" (Group Admin) và "Thủ quỹ" (Treasurer)? Trách nhiệm của họ khác gì so với "Thành viên thông thường" (Group Member)?
* **Câu hỏi 4:** Nếu một thành viên nộp hóa đơn chi hộ cho nhóm, ai là người có thẩm quyền phê duyệt (Approve) khoản chi đó trước khi tiền được hoàn trả?
* **Câu hỏi 5:** Trong mô hình gia đình hoặc ban giám sát CLB, có cần vai trò "Người quan sát" (Auditor / Viewer - chỉ có quyền xem báo cáo thu chi mà không được sửa đổi) không?

---

## PHẦN II: [TRỌNG TÂM OOAD] BÓC TÁCH ĐỐI TƯỢNG, THUỘC TÍNH & HÀNH VI

> [!IMPORTANT]
> **Đây là phần cốt lõi của bài Lab 1:** Trả lời câu hỏi *"Hệ thống có những đối tượng nào? Mỗi đối tượng có vai trò gì?"*. Hãy đối chiếu với góc nhìn của **Eric Evans (DDD)** và **Alistair Cockburn (Use Case)** dưới đây.

### 2.1. Nhận Diện Tác Nhân Ngoài (External Actors) - Theo Alistair Cockburn (Agent 03)
* **Câu hỏi 6 (Primary & Secondary Actors):**
  * Những con người thực tế nào sẽ tương tác trực tiếp với hệ thống? *(Gợi ý: `IndividualUser`, `GroupAdmin`, `GroupMember`, `Treasurer`, `SystemAdmin`)*.
  * Có những hệ thống đối tác bên ngoài nào đóng vai trò tác nhân hỗ trợ? *(Gợi ý: Hệ thống Ngân hàng `VietQR Partner`, Dịch vụ trí tuệ nhân tạo `AI OCR Service`)*.

### 2.2. Nhận Diện Thực Thể Dữ Liệu (Entities & Domain Objects) - Theo Eric Evans (Agent 04 - DDD)
* **Câu hỏi 7 (Ngôn ngữ chung - Ubiquitous Language):**
  * Khi nói về chi tiêu, những danh từ (Entities) nào xuất hiện liên tục trong cuộc trò chuyện của người dùng?
  * *(Gợi ý: Tài khoản `UserAccount`, Ví tiền `Wallet`, Danh mục `Category`, Hạn mức `Budget`, Giao dịch `Transaction`, Nhóm chi tiêu `ExpenseGroup`, Khoản chi nhóm `GroupExpense`, Quy tắc chia `SplitRule`, Công nợ `DebtRelation`, Quyết toán `Settlement`, Minh chứng `PaymentProof`)*.

* **Câu hỏi 8 (Bóc tách Thuộc tính - Attributes):**
  * Để quản lý một Khoản chi nhóm (`GroupExpense`), bạn cần lưu lại những thông tin gì?
    * *Ai là người trả tiền? (`payerUserId`)*
    * *Tổng số tiền là bao nhiêu? (`amount`)*
    * *Hóa đơn đính kèm ở đâu? (`receiptImageUrl`)*
    * *Chia cho những ai và mỗi người chịu bao nhiêu? (`splitDetails`)*
    * *Trạng thái duyệt ra sao? (`approvalStatus`)*

* **Câu hỏi 9 (Bóc tách Hành vi / Trách nhiệm - Responsibilities & Methods):**
  * Đối tượng Ví tiền (`Wallet`) tự thực hiện được hành vi gì khi có phát sinh tài chính? *(Gợi ý: `debit(amount)` để trừ tiền, `credit(amount)` để nhận tiền, `getBalance()`)*.
  * Đối tượng Nhóm (`ExpenseGroup`) cần có những hành động gì? *(Gợi ý: `addMember()`, `removeMember()`, `calculateGroupBalance()`, `simplifyDebts()`)*.

* **Câu hỏi 10 (Đa hình trong Quy tắc Chia tiền - Strategy Pattern & Polymorphism):**
  * Khi chia một hóa đơn, có những cách toán học nào để phân bổ tiền?
    * *Cách 1:* Chia đều cho tất cả (`EqualSplitRule` $\rightarrow Total / N$).
    * *Cách 2:* Mỗi người trả đúng số tiền món mình ăn (`ExactSplitRule`).
    * *Cách 3:* Chia theo tỷ lệ phần trăm (`PercentageSplitRule` $\rightarrow$ Tổng % = 100%).
    * *Cách 4:* Chia theo suất/phần (`ShareSplitRule` $\rightarrow$ Người lớn 2 phần, trẻ em 1 phần).
  * *(Tư duy OOAD: Lớp cha `SplitRule` có phương thức trừu tượng `calculateShares()`, các lớp con kế thừa sẽ ghi đè logic toán học khác nhau)*.

### 2.3. Bóc Tách Bài Toán Đồ Thị Nợ - Theo Robert Tarjan (Agent 06)
* **Câu hỏi 11 (Cấn trừ nợ chéo - Multi-party Debt Simplification):**
  * Nếu A nợ B 100k, B lại nợ C 100k, làm thế nào để hệ thống tự động nhận diện và rút gọn thành: *"A chuyển thẳng cho C 100k, B không cần làm gì"*?
  * Đối tượng nào sẽ chịu trách nhiệm tính toán "Số dư ròng" (Net Balance = Tổng tiền đã trả hộ trừ Tổng tiền thụ hưởng) của từng thành viên trong nhóm? *(Gợi ý: `DebtSimplificationGraph`)*.

---

## PHẦN III: KHẢO SÁT QUY TRÌNH NGHIỆP VỤ HIỆN TẠI (BUSINESS PROCESSES)

### 3.1. Quy trình Thu Chi Cá Nhân & Tâm Lý Học Hành Vi - Theo GS. Richard Thaler (Agent 01)
* **Câu hỏi 12 (Kế toán tâm lý - Mental Accounting):** Người dùng có xu hướng chia tiền trong đầu thành các "ngăn vô hình" không (ví dụ: tiền ăn uống, tiền thuê nhà, tiền tiết kiệm, tiền mua sắm bốc đồng)? Khi một ngăn bị âm tiền, tâm lý họ phản ứng ra sao?
* **Câu hỏi 13 (Cú hích vi mô - Nudge vs. Nagging):** Khi người dùng sắp tiêu vượt ngân sách tháng:
  * Cách thông báo truyền thống: *"Cảnh báo! Bạn đã tiêu quá hạn mức!"* $\rightarrow$ Người dùng cảm thấy tội lỗi và có xu hướng xóa ứng dụng.
  * Cách tiếp cận Cú hích nhẹ (Nudge): *"Bạn còn 1.500.000đ cho 10 ngày tới. Nếu mỗi ngày bạn tiêu tối đa 150.000đ, bạn vẫn sẽ đạt mục tiêu tiết kiệm!"* $\rightarrow$ Bạn thấy cách nào hiệu quả hơn? Tại sao?
* **Câu hỏi 14 (Ma sát thao tác - Friction Score):** Một người mất trung bình bao nhiêu giây để mở sổ/ứng dụng và ghi xong một khoản chi 25.000đ tiền cà phê? Nếu bắt họ điền quá 3 trường thông tin, tỷ lệ họ bỏ cuộc sau 1 tuần là bao nhiêu %?

### 3.2. Quy trình Bóc Tách Hóa Đơn Tự Động - Theo Andrew Ng (Agent 08 - AI Vision)
* **Câu hỏi 15 (Trải nghiệm quét hóa đơn):** Khi ăn uống xong, thay vì phải gõ từng món vào ứng dụng, người dùng có sẵn sàng chụp ảnh hóa đơn để AI tự động nhận dạng không?
* **Câu hỏi 16 (Luồng ngoại lệ khi AI đọc sai):** Nếu hóa đơn bị mờ hoặc rách khiến AI đọc sai số tiền, quy trình nghiệp vụ cần cho phép người dùng chỉnh sửa nhanh tay (Human-in-the-loop) tại bước nào?

### 3.3. Quy trình Quyết Toán Nợ & Bảo Mật VietQR - Theo Bruce Schneier (Agent 11)
* **Câu hỏi 17 (Rủi ro chuyển khoản thủ công):** Khi trả nợ bằng cách gõ tay số tài khoản và số tiền vào app ngân hàng, người dùng hay gặp những rủi ro gì? *(Gợi ý: Nhầm số 0, chuyển nhầm số tài khoản, quên ghi nội dung chuyển khoản)*.
* **Câu hỏi 18 (Trải nghiệm VietQR 1-chạm):** Nếu ứng dụng tự sinh mã VietQR động chứa sẵn chính xác số tài khoản người nhận, đúng số tiền nợ và mã giao dịch, người dùng có cảm thấy an tâm và tiết kiệm thời gian hơn không?

---

## PHẦN IV: ĐÁNH GIÁ HIỆN TRẠNG NGHIỆP VỤ (VẤN ĐỀ, NGUYÊN NHÂN & GIẢI PHÁP)

### 4.1. Khảo Sát 5 Điểm Nghẽn Lớn Nhất (Pain Points)
* **Câu hỏi 19:** Vì sao việc ghi chép tài chính cá nhân lại được coi là "hành vi đi ngược lại bản năng lười biếng của con người"?
* **Câu hỏi 20:** Tại sao việc mở lời đòi nợ bạn bè sau một bữa ăn lại tạo ra rào cản tâm lý ngại ngùng, sợ bị coi là "tính toán chi li"?
* **Câu hỏi 21:** Trong một chuyến đi du lịch nhóm 10 người, điều gì gây ức chế nhất cho người làm "thủ quỹ bất đắc dĩ"?
* **Câu hỏi 22:** Có bao giờ bạn bị "cháy túi" trước ngày 20 hàng tháng mà không hiểu tiền của mình đã biến đi đâu không?
* **Câu hỏi 23:** Khi chuyển khoản trả nợ trong nhóm chat Zalo, tại sao ảnh chụp màn hình chuyển khoản (Screenshot Bill) lại hay bị trôi tin nhắn và gây tranh cãi "ai đã trả, ai chưa trả"?

### 4.2. Đề Xuất Giải Pháp Nghiệp Vụ (Chưa bàn đến công nghệ phần mềm)
* **Câu hỏi 24:** Nếu không dùng phần mềm, những quy ước ứng xử (Social Norms) nào có thể giúp giảm thiểu rủi ro nợ nần trong nhóm?
  * *(Gợi ý: Quy ước chụp ảnh hóa đơn ngay tại bàn ăn, quy ước tất toán nợ trong 24 giờ, quy ước khách quan hóa việc nhắc nợ...)*

---

## PHẦN V: KHẢO SÁT HIỆN TRẠNG TIN HỌC & MÔI TRƯỜNG VẬN HÀNH

### 5.1. Thiết Bị & Phần Cứng
* **Câu hỏi 25:** Người dùng mục tiêu chủ yếu sử dụng dòng máy nào? (iPhone iOS 16+, Android Samsung/Xiaomi...). Camera của họ có đủ rõ nét để chụp hóa đơn trong môi trường thiếu sáng (quán ăn, quán cà phê) không?
* **Câu hỏi 26:** Trưởng nhóm hoặc Thủ quỹ có nhu cầu xem báo cáo tổng kết trên màn hình lớn (Laptop / PC) không?

### 5.2. Phần Mềm & Thói Quen Đang Có
* **Câu hỏi 27:** Hiện tại họ đang dùng ứng dụng nào để thanh toán hàng ngày? (Vietcombank, MB Bank, Techcombank, MoMo, ZaloPay, Apple Pay...).
* **Câu hỏi 28:** Ứng dụng nhắn tin trao đổi công việc / bạn bè chính của họ là gì? (Zalo, Messenger, Telegram...).

### 5.3. Trình Độ Tin Học Của Con Người
* **Câu hỏi 29:** Người dùng thuộc độ tuổi nào? (Sinh viên 18-23 tuổi, Người đi làm 24-35 tuổi). Mức độ thành thạo quét mã QR và sử dụng ứng dụng di động của họ ở mức nào?
* **Câu hỏi 30:** Họ có sẵn sàng cấp quyền truy cập Camera và Nhận thông báo đẩy (Push Notifications) cho ứng dụng không?

---

## 📝 KHUNG MẪU NHẬT KÝ PHỎNG VẤN THỰC TẾ (INTERVIEW LOG TEMPLATE)

```markdown
### BIÊN BẢN PHỎNG VẤN KHẢO SÁT HIỆN TRẠNG
- Người thực hiện khảo sát: ................................................
- Người được phỏng vấn (Persona): (Sinh viên phòng trọ / Dân văn phòng / Trưởng nhóm du lịch)
- Thời gian & Địa điểm: .../.../2026 tại ...................................

1. Thói quen quản lý chi tiêu hiện tại:
   - Cách ghi chép: (Ghi sổ / Dùng Excel / Dùng app ngân hàng / Không ghi)
   - Tần suất: .............................................................

2. Trải nghiệm chia tiền nhóm gần đây nhất:
   - Loại sự kiện: (Bữa ăn lẩu / Chuyến đi Đà Lạt / Tiền phòng trọ tháng)
   - Ai trả tiền trước? ....................................................
   - Cách tính tiền cho từng người: ........................................
   - Có gặp khó khăn hay tranh cãi gì không? ................................

3. Các thực thể / thông tin quan trọng họ bắt buộc phải lưu giữ:
   - Danh sách: ............................................................

4. Đánh giá về sự bất tiện lớn nhất (Top 1 Pain Point):
   - Trả lời: ..............................................................

5. Ý kiến về tính năng mong muốn nhất:
   - [ ] Quét hóa đơn AI tự đọc số tiền
   - [ ] Tự động rút gọn nợ (không phải chuyển khoản vòng quanh)
   - [ ] Mã VietQR điền sẵn số tiền để trả nợ 1-chạm
   - [ ] Cảnh báo tốc độ tiêu tiền mỗi ngày
```

---
*Tài liệu được biên soạn đồng hành cùng Hội đồng Chuyên gia Kiệt xuất (`agents/core/`) phục vụ quá trình tự học và nghiên cứu môn PTTKHTTT / OOAD.*
