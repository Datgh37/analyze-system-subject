**THỰC HÀNH PHÂN TÍCH THIẾT KẾ HƯỚNG ĐỐI TƯỢNG**

**LAB 1: KHẢO SÁT HIỆN TRẠNG QUẢN LÝ CHI TIÊU CÁ NHÂN & NHÓM**

**Đề tài:** Hệ thống Quản lý Chi tiêu & Lập Ngân sách Cá nhân / Nhóm Thông minh (Smart Expense & Budget Manager)

---

### Mục đích:

Giúp sinh viên hiểu biết sâu sắc về hiện trạng quản lý tài chính cá nhân, tâm lý học hành vi người dùng, và đa dạng các hình thái chi tiêu tập thể (phòng trọ, du lịch, hộ gia đình, câu lạc bộ/dự án) liên quan đến đề tài **Hệ thống Quản lý Chi tiêu & Lập Ngân sách Cá nhân / Nhóm (Tích hợp AI)**.

---

### Yêu cầu:

- **Khảo sát người dùng thực tế:** Tiến hành khảo sát hành vi quản lý chi tiêu của cá nhân (sinh viên, người đi làm) và các mô hình nhóm chi tiêu thực tế (phòng trọ sinh viên, nhóm bạn đi du lịch, quỹ chung gia đình, quỹ câu lạc bộ/nhóm dự án khởi nghiệp).

- **Khảo sát đối thủ cạnh tranh (Benchmarking):** Trải nghiệm, phân tích ưu/nhược điểm và điểm nghẽn trải nghiệm của các ứng dụng hiện có trên thị trường (Money Lover, Sổ Thu Chi, Splitwise, Spendee, Cashew, Tricount...).

- **Chuẩn hóa mô hình vận hành:** Giả định và thiết lập mô hình vận hành của một nền tảng Fintech thông minh phục vụ đồng thời dòng tiền cá nhân và bài toán chia sẻ chi phí nhóm.

---

### Nội dung:

Nhóm sinh viên khảo sát các hiện trạng nhằm làm rõ 4 Bounded Contexts nghiệp vụ cốt lõi:

#### 1. Sơ đồ cơ cấu tổ chức & Phân quyền nhóm (Dưới góc nhìn Chuyên gia Quản trị Nhóm & DDD):
- **Cơ cấu tổ chức của một nhóm chi tiêu thực tế:**
  - *Trưởng nhóm (Group Admin):* Khởi tạo không gian nhóm, cấu hình tỷ lệ đóng góp quỹ, duyệt thành viên, thiết lập quy tắc chia tiền và chốt quyết toán kỳ (Settle Up).
  - *Thủ quỹ / Người chi trả (Treasurer / Primary Payer):* Người quản lý quỹ chung hoặc đứng ra ứng tiền túi chi hộ cho tập thể, nộp hóa đơn chứng từ.
  - *Thành viên nhóm (Group Member / Contributor):* Đóng góp quỹ, tham gia chia tiền, nộp yêu cầu hoàn ứng (Reimbursement) và thanh toán nợ.
  - *Người quan sát / Thụ hưởng (Viewer / Auditor / Beneficiary):* Con cái trong gia đình hoặc thành viên giám sát quỹ (chỉ có quyền xem báo cáo thu chi, không được chỉnh sửa).
- **Cơ cấu vận hành nền tảng hệ thống:**
  - *Quản trị viên hệ thống (System Administrator):* Giám sát hạ tầng, chuẩn hóa danh mục tài chính toàn cục, audit log bảo mật.
  - *Đối tác Cổng thanh toán (VietQR / Banking Partner):* Cung cấp API sinh mã QR thanh toán động và xác thực giao dịch chuyển khoản.
  - *Dịch vụ Trí tuệ nhân tạo (AI OCR Engine):* Bóc tách dữ liệu hóa đơn đa phương thức (Multimodal Vision).
- **Phân định 4 Ngữ cảnh Giới hạn (Bounded Contexts):**
  1. `Expense Context`: Ghi nhận chi tiêu, danh mục, hóa đơn đính kèm.
  2. `Group Settlement Context`: Quỹ chung, quản lý thành viên, đồ thị cấn trừ nợ chéo.
  3. `Budgeting Context`: Hạn mức chu kỳ, tốc độ tiêu tiền (Spending Velocity).
  4. `Identity & Access Context`: Định danh người dùng, xác thực và phân quyền.

---

#### 2. Quy trình nghiệp vụ hiện có (Dưới góc nhìn Quản lý & Người dùng thực tế, không dùng thuật ngữ Tin học):
- **Nghiệp vụ 1: Quản lý Thu - Chi & Lập Ngân Sách Cá Nhân (Góc nhìn Tâm lý học Hành vi & Richard Thaler)**
  - *Hiện trạng thực hiện:* Ghi chép sổ tay, dùng Excel, ghi chú điện thoại hay chỉ ước lượng trong đầu?
  - *Kế toán tâm lý (Mental Accounting):* Người dùng chia tiền thành những "ngăn vô hình" nào (thiết yếu, hưởng thụ, tiết kiệm)? Khi một ngăn bị âm, tâm lý buông xuôi diễn ra thế nào?
  - *Bẫy chi tiêu định kỳ (Subscription Traps):* Các khoản trừ tiền tự động hàng tháng (Netflix, Spotify, Cloud, tập Gym...) có bị lãng quên không?
  - *Tần suất & Điểm ma sát (Friction Score):* Mất bao nhiêu giây và bao nhiêu thao tác để ghi 1 giao dịch? Nếu quá 3 bước, tỷ lệ bỏ cuộc sau 1 tuần là bao nhiêu?
- **Nghiệp vụ 2: Tạo Khoản Chi Chung & Đa Dạng Hóa Cơ Chế Chia Tiền (Split Bill)**
  - *Mô hình Quỹ chung gia đình (Joint Pool):* Vợ chồng đóng góp tỷ lệ 60-40, chi trả sinh hoạt phí và cấp tiền tiêu vặt (Allowance Quota) cho con cái.
  - *Mô hình Sinh hoạt phòng trọ (Co-living):* Tiền phòng cố định tính riêng từng người + tiền điện, nước, dịch vụ chia đều.
  - *Mô hình Tạm ứng & Hoàn ứng (Advance & Reimbursement):* Cá nhân bỏ tiền túi chi hộ việc chung của CLB/dự án $\rightarrow$ nộp hóa đơn về quỹ để nhận tiền hoàn ứng.
  - *Mô hình Ăn uống & Du lịch:* Chia theo món ăn thực tế tiêu dùng (Itemized Split), chia theo tỷ lệ % hoặc chia theo suất/phần (người lớn 2 phần, trẻ em 1 phần).
- **Nghiệp vụ 3: Cấn Trừ & Rút Gọn Chuỗi Nợ Nhóm (Góc nhìn Lý thuyết Đồ thị & Robert Tarjan)**
  - *Hiện trạng tính nợ:* Khi nhiều người cùng chi hộ trong chuyến đi 3 ngày (A trả bữa 1, B trả bữa 2, C mua vé xe...), nhóm tính toán công nợ thế nào?
  - *Điểm nghẽn giao dịch:* Cả nhóm có phải chuyển khoản lòng vòng $O(N^2)$ lần không?
  - *Cơ chế Số dư ròng (Net Balance Settlement):* Nguyên tắc bù trừ công nợ đa phương (Multilateral Netting) để rút gọn thành tối đa $N-1$ giao dịch thanh toán trực tiếp.
- **Nghiệp vụ 4: Quyết Toán Nợ (Settle Up) & Xác Thực Minh Chứng Chuyển Khoản**
  - *Hiện trạng:* Gõ tay số tài khoản ngân hàng $\rightarrow$ Rủi ro gõ nhầm số tiền hoặc nhầm người $\rightarrow$ Chụp màn hình bill gửi vào nhóm chat $\rightarrow$ Tin nhắn bị trôi và gây tranh cãi.
  - *Nhu cầu chuẩn hóa:* Sinh mã VietQR động chứa sẵn chính xác số tiền nợ, tự động kiểm tra chứng từ và cập nhật trạng thái nợ "Đã thanh toán".
- **Khảo sát các Luồng Ngoại Lệ (Exceptions - Theo Alistair Cockburn):**
  - *Ngoại lệ 1:* Quỹ chung nhóm bị âm/hết tiền khi phát sinh chi phí đột xuất.
  - *Ngoại lệ 2:* Ảnh chụp hóa đơn bị mờ, rách khiến AI OCR đọc sai số tiền (yêu cầu cơ chế Human-in-the-loop chỉnh sửa nhanh).
  - *Ngoại lệ 3:* Thành viên chuyển khoản sai số tiền hoặc chuyển nhầm vào tài khoản cá nhân thay vì quỹ nhóm.

---

#### 3. Đánh giá nghiệp vụ hiện tại:
- **Các khó khăn / Điểm nghẽn nhức nhối (Pain Points):**
  1. *Rào cản ghi chép:* Nhập liệu thủ công tốn thời gian khiến người dùng bỏ cuộc sau vài ngày.
  2. *Chia tiền thiếu công bằng:* Áp dụng công thức chia đều cào bằng ($Total/N$) gây thiệt thòi cho người ăn ít/về sớm.
  3. *Mạng lưới nợ chéo phức tạp:* Phải chuyển khoản qua lại nhiều lần, tốn phí và dễ nhầm lẫn.
  4. *Rào cản tâm lý khi đòi nợ:* Ngại mở lời nhắc nợ bạn bè, sợ bị coi là "tính toán chi li".
  5. *Vỡ ngân sách cuối tháng:* Cảnh báo tài chính truyền thống mang tính phán xét tiêu cực ("Bạn đã tiêu vượt mức!") thay vì đưa ra **Cú hích hành động vi mô (Actionable Nudge)** theo thời gian thực.
- **Nguyên nhân cốt lõi:** Dữ liệu tài chính phân mảnh; thiếu công cụ tự động hóa bóc tách hóa đơn; thiếu thuật toán tối ưu hóa đồ thị nợ; giao tiếp công nợ gắn liền với cảm xúc cá nhân.
- **Giải pháp nghiệp vụ (Độc lập công nghệ):** Ban hành bộ 4 quy chế chia tiền chuẩn mực; quy chuẩn hóa nguyên tắc bù trừ công nợ đa phương; quy ước chốt nợ trong 24-48 giờ; khách quan hóa việc nhắc nợ thành thông báo tự động mang tính hệ thống.

---

#### 4. Công ty, tổ chức và hệ thống có những đối tượng nào? Mỗi đối tượng có vai trò gì trong hệ thống (PHẦN NÀY LÀ PHẦN TRỌNG TÂM CỦA VIỆC PHÂN TÍCH THIẾT KẾ HƯỚNG ĐỐI TƯỢNG):

- **Các Tác nhân ngoài (Actors - Theo chuẩn Cockburn):**
  - `Individual User` (Người dùng cá nhân): Quản lý ví tiền cá nhân, ghi nhận thu/chi, thiết lập ngân sách và nhận các gợi ý điều chỉnh chi tiêu (Nudges).
  - `Group Admin` (Trưởng nhóm): Khởi tạo nhóm, phân quyền thành viên, cấu hình quy tắc chia tiền, duyệt hoàn ứng và chốt quyết toán.
  - `Group Member` (Thành viên nhóm): Tham gia nhóm, ghi nhận chi phí chung, thanh toán công nợ qua VietQR và nộp minh chứng.
  - `Treasurer / Primary Payer` (Thủ quỹ / Người chi trả): Ứng tiền thanh toán trước, chụp ảnh hóa đơn, xác nhận đã nhận tiền hoàn trả.
  - `Auditor / Viewer` (Người quan sát): Theo dõi báo cáo thu chi và số dư quỹ nhóm mà không có quyền thay đổi dữ liệu.
  - `System Administrator` (Quản trị viên hệ thống): Chuẩn hóa cây danh mục toàn cục, giám sát an toàn dữ liệu và xử lý khiếu nại.
  - `VietQR / Banking Gateway Partner` (Đối tác Ngân hàng): API sinh mã QR động chuẩn NAPAS và webhook xác thực chuyển khoản.
  - `AI OCR Receipt Engine` (Dịch vụ Trí tuệ nhân tạo): Bóc tách dữ liệu từ ảnh chụp hóa đơn đa phương thức.

- **Các Đối tượng Thực thể Nghiệp vụ (Domain Entities & Value Objects - Theo chuẩn Eric Evans DDD):**
  - `UserAccount` (Entity): Quản lý thông tin định danh, tài khoản, cấu hình tiền tệ mặc định.
  - `Wallet` (Entity): Quản lý nguồn tài chính (Tiền mặt, Ngân hàng, Ví điện tử) và số dư khả dụng (`debit`, `credit`, `transferTo`).
  - `Money` (Value Object bất biến): Lưu trữ số tiền (`Decimal` chính xác cao) và đơn vị tiền tệ (`Currency`), triệt tiêu hoàn toàn sai số làm tròn khi chia tiền lẻ.
  - `Category` & `SubCategory` (Entity): Cây phân loại mục đích chi tiêu (Ăn uống, Đi lại, Nhà ở, Chi thiết yếu vs Chi hưởng thụ).
  - `Budget` (Entity): Hạn mức chi tiêu theo chu kỳ và danh mục, tính toán số tiền được phép tiêu mỗi ngày (Daily Allowance).
  - `Transaction` (Lớp trừu tượng Base Class) $\rightarrow$ Đa hình kế thừa:
    - `PersonalExpense`: Khoản chi tiêu cá nhân thường nhật.
    - `IncomeTransaction`: Khoản thu nhập ghi nhận vào ví.
    - `GroupExpense`: Khoản chi chung của nhóm, xác định Người trả tiền (`Payer`) và Danh sách người cùng chia tiền.
  - `ExpenseGroup` (Aggregate Root): Đơn vị ranh giới quản lý không gian chi tiêu chung (quỹ chung, danh sách thành viên, quy chế chia tiền, trạng thái hoạt động).
  - `SplitRule` (Lớp chiến lược Strategy Pattern) & Các lớp con đa hình:
    - `EqualSplitRule`: Chia đều ($Total / N$).
    - `ExactSplitRule`: Chia theo số tiền đích danh từng người.
    - `PercentageSplitRule`: Chia theo tỷ lệ % (Tổng = 100%).
    - `ShareSplitRule`: Chia theo số phần/suất đóng góp.
  - `SplitDetail` (Entity): Ghi nhận số tiền và trách nhiệm chi trả của từng thành viên trong một khoản chi nhóm.
  - `DebtRelation` (Entity): Lưu trữ quan hệ công nợ có hướng giữa 2 cá nhân (Debtor $\rightarrow$ Creditor).
  - `DebtSimplificationGraph` (Domain Service / Object - Robert Tarjan): Đồ thị xử lý cấn trừ nợ chéo nhiều bên và rút gọn chuỗi nợ dựa trên Số dư ròng (Net Balance).
  - `Settlement` (Entity): Giao dịch thanh toán tất toán nợ thực tế giữa 2 thành viên trong nhóm.
  - `PaymentProof` (Entity): Lưu trữ ảnh chụp chuyển khoản, mã tham chiếu ngân hàng (Bank Reference Code) và trạng thái đối soát.
  - `SpendingVelocityAlert` (Domain Service / Object): Theo dõi tốc độ tiêu tiền (Burn Rate) theo thời gian thực và phát cảnh báo nguy cơ vỡ ngân sách trước ngày 20 hàng tháng.
  - `OCRReceipt` (Entity): Dữ liệu bóc tách từ ảnh hóa đơn (tên quán, ngày giờ, danh sách món, thuế VAT, tổng tiền).

---

### 3. Hiện trạng Tin học:

- **Phần cứng:**
  - *Thiết bị người dùng:* Smartphone Android / iOS có camera tối thiểu 8MP để chụp rõ nét hóa đơn trong điều kiện thiếu sáng; Laptop/PC để xem báo cáo phân tích tài chính dạng bảng lớn.
  - *Hạ tầng máy chủ:* Máy chủ ứng dụng (Application Server), Máy chủ CSDL (Database Server) và Lưu trữ đối tượng (Object Storage S3-compatible) lưu trữ hình ảnh hóa đơn và chứng từ thanh toán.
- **Phần mềm:**
  - *Phần mềm người dùng đang sử dụng:* Excel, Google Sheets, App ghi chú, App Mobile Banking (Vietcombank, MB, Techcombank...), Ví điện tử (MoMo, ZaloPay), Ứng dụng chat (Zalo, Messenger, Telegram).
  - *Hệ điều hành mục tiêu:* iOS, Android, Web App (Windows/macOS).
  - *Hệ quản trị CSDL dự kiến:* PostgreSQL / MySQL (chuẩn hóa 3NF, hỗ trợ Transactional Locking và ACID).
  - *Dịch vụ / API tích hợp:* Dịch vụ AI Vision OCR (Google Gemini / Vision API), Chuẩn sinh mã VietQR động của NAPAS.
- **Con người:**
  - *Trình độ Tin học:* Người dùng mục tiêu (học sinh, sinh viên, người đi làm trẻ tuổi) cực kỳ thành thạo smartphone, thanh toán không tiền mặt và quét mã QR; yêu cầu trải nghiệm 0-1 chạm (Frictionless), không chấp nhận các giao diện nhập liệu rườm rà.

---

### Kết quả:

Trước khi tiến hành khảo sát hiện trạng, nhóm sinh viên khai báo các thông tin sau:

**Các thành viên tham gia thực hiện nhóm:**

| STT | Họ tên | Khả năng / Vai trò chuyên môn | Ghi chú |
| :---: | :--- | :--- | :--- |
| 1 |  | Nhóm trưởng / Phân tích đối tượng Hướng đối tượng & DDD |  |
| 2 |  | Thành viên / Khảo sát Quy trình nghiệp vụ & Đánh giá hiện trạng |  |
| 3 |  | Thành viên / Khảo sát Hiện trạng tin học & Tổng hợp tài liệu |  |

---

### Kết thúc bài lab:

- Nhóm trưởng tạo một bảng ghi rõ các thành viên đã làm phần nào, bao nhiêu % ở trang đầu của file khảo sát rồi nộp bài trong mục **Lab 1** của bài thực hành theo deadline quy định:

| STT | Họ và tên | Phần việc được phân công | Mức độ hoàn thành (%) |
| :---: | :--- | :--- | :---: |
| 1 |  | Khảo sát cơ cấu tổ chức & Phân tích đối tượng hướng đối tượng (Actors, Entities, SplitRules, DebtGraph) | ... % |
| 2 |  | Khảo sát 4 quy trình nghiệp vụ thực tế, các luồng ngoại lệ & Đánh giá hiện trạng | ... % |
| 3 |  | Khảo sát hiện trạng tin học, môi trường vận hành & Tổng hợp báo cáo Lab 1 | ... % |
