# BÁO CÁO TỔNG QUAN KIẾN TRÚC HỆ THỐNG, ĐẶC TẢ NGHIỆP VỤ & NGUYÊN MẪU HOÀN THIỆN
## HỆ THỐNG QUẢN LÝ CHI TIÊU VÀ HOẠCH ĐỊNH TÀI CHÍNH CÁ NHÂN THÔNG MINH (TÍCH HỢP TRÍ TUỆ NHÂN TẠO - AI)
*(Smart Personal Expense and Financial Planning Management System with AI Integration)*

---

> **Tài liệu Báo cáo Tổng quan Kiến trúc Toàn diện (System Architecture & Blueprint Summary)**  
> **Phiên bản:** 3.0 (Cập nhật sau khi hoàn thiện toàn bộ Nguyên mẫu Thực tế & Sơ đồ Thiết kế Kỹ thuật)  
> **Phạm vi đồ án:** Phân tích Thiết kế Hệ thống Thông tin (PTTKHTTT) & Khóa luận Tốt nghiệp Cử nhân CNTT/HTTT.

---

## MỤC LỤC TỔNG QUAN

1. [Tầm nhìn, Sứ mệnh & Bản chất Cốt lõi 100% Tài chính Cá nhân Độc lập](#1-tầm-nhìn-sứ-mệnh--bản-chất-cốt-lõi-100-tài-chính-cá-nhân-độc-lập)
2. [Điểm mới Đột phá & Đối chiếu Khoảng trống Nghiên cứu so với Thị trường](#2-điểm-mới-đột-phá--đối-chiếu-khoảng-trống-nghiên-cứu-so-với-thị-trường)
3. [Mô hình hóa Use Case Toàn diện (65 Use Cases & 4 Tác nhân Cốt lõi)](#3-mô-hình-hóa-use-case-toàn-diện-65-use-cases--4-tác-nhân-cốt-lõi)
4. [Kiến trúc Hướng miền Domain-Driven Design (DDD) & Class Diagram (6 Bounded Contexts)](#4-kiến-trúc-hướng-miền-domain-driven-design-ddd--class-diagram-6-bounded-contexts)
5. [Thiết kế Mô hình Động & Tương tác Hệ thống (11 Sơ đồ Động)](#5-thiết-kế-mô-hình-động--tương-tác-hệ-thống-11-sơ-đồ-động)
6. [Thiết kế Cơ sở Dữ liệu Quan hệ 3NF (PostgreSQL 16 ACID)](#6-thiết-kế-cơ-sở-dữ-liệu-quan-hệ-3nf-postgresql-16-acid)
7. [Kiến trúc Hiện thực hóa Nguyên mẫu & Kết quả Kiểm thử Thực tế](#7-kiến-trúc-hiện-thực-hóa-nguyên-mẫu--kết-quả-kiểm-thử-thực-tế)
8. [Cẩm nang Hướng dẫn Thuyết trình & Bảo vệ Trước Hội đồng Phản biện](#8-cẩm-nang-hướng-dẫn-thuyết-trình--bảo-vệ-trước-hội-đồng-phản-biện)

---

## 1. TẦM NHÌN, SỨ MỆNH & BẢN CHẤT CỐT LÕI 100% TÀI CHÍNH CÁ NHÂN ĐỘC LẬP

### 1.1 Triết lý 100% Personal Finance Management (PFM)
Hệ thống kiên quyết theo đuổi nguyên tắc thiết kế bất biến: **Tập trung tuyệt đối 100% vào bài toán Tài chính Cá nhân Độc lập**, loại bỏ hoàn toàn các chức năng chia tiền nhóm, quỹ phòng trọ hay đối soát nợ tập thể. Quyết định kiến trúc này dựa trên 3 nền tảng thực tiễn:
- **Tôn trọng sự riêng tư tối mật:** Quản lý tiền bạc cá nhân là không gian tế nhị. Người dùng chỉ trung thực ghi chép khi họ biết chắc chắn dữ liệu không bị ai phán xét hoặc dòm ngó.
- **Triệt tiêu nhiễu loạn phân tích:** Việc gộp chi tiêu bạn bè, người yêu vào sổ sách cá nhân làm biến dạng hoàn toàn đường cong tiêu dùng thực tế, khiến các thuật toán phân tích hạn mức ngày (DSA) và dự phóng thâm hụt mất độ chính xác.
- **Giải quyết bài toán gốc rễ:** Tập trung nguồn lực xử lý bài toán tâm lý chi tiêu bốc đồng, rèn luyện kỷ luật dòng tiền và hỗ trợ người dùng đạt được sự an tâm tài chính lâu dài.

### 1.2 Sự Chuyển dịch Mô hình: Từ Ghi chép Thụ động sang Hỗ trợ Quyết định Chủ động
- **Mô hình cũ (Passive Bookkeeping):** Các ứng dụng thế hệ trước chỉ là "chiếc gương chiếu hậu" thụ động ghi nhận những gì tiền bạc đã mất đi trong quá khứ, thông báo số dư khô khan và chỉ báo đỏ khi tiền đã cạn kiệt.
- **Mô hình mới (Proactive Decision Support System):** Hệ thống của đề tài đóng vai trò như một **Trợ lý Tài chính Số (Digital Financial Coach)** hoạt động liên tục 24/7:
  * Thu thập dữ liệu rảnh tay (Zero-Mental Friction): Lắng nghe thông báo biến động số dư ngân hàng qua Android Native Service, bóc tách hóa đơn AI OCR đa món, và ghi chép bằng khẩu lệnh giọng nói.
  * Tự động tính toán Hạn mức chi tiêu mỗi ngày thích ứng (ADSA): Trả lời chính xác câu hỏi *"Hôm nay tôi được phép tiêu bao nhiêu tiền để cuối tháng không bị rỗng túi?"*.
  * Can thiệp tâm lý bằng Cú hích hành vi Thaler Nudge: Tự động kích hoạt Khoảng trễ tâm lý (Cooling-off Period 24h) đối với các khoản mua sắm bốc đồng trước khi tiền thực sự rời khỏi ví.

---

## 2. ĐIỂM MỚI ĐỘT PHÁ & ĐỐI CHIẾU KHOẢNG TRỐNG NGHIÊN CỨU SO VỚI THỊ TRƯỜNG

| Trụ Cột / Tiêu Chí | Các Ứng Dụng Thị Trường (Money Lover, Spendee...) | Hệ Thống PFM Tích Hợp AI (Đề tài) | Giá Trị Thực Tiễn & Tính Đột Phá |
| :--- | :--- | :--- | :--- |
| **1. Tự động hóa Nhập liệu** | Nhập tay từng khoản hoặc OCR ký tự thô regex, tỷ lệ lỗi cao, không đối soát số học, ma sát thao tác lớn. | Pipeline Multimodal AI Vision (Gemini 2.5 Flash) kết hợp Thuật toán Hậu kiểm toán số học $\Delta = 0$ và Android Notification Listener tự động. | Nhận diện từng món hàng trên hóa đơn, tự đối soát: $\sum(\text{SL} \times \text{ĐG}) + \text{VAT} = \text{Tổng tiền}$. Tự động nhận diện biến động số dư từ 9 ngân hàng Việt Nam 0ms. |
| **2. Cơ chế Hoạch định Ngân sách** | Ngân sách tĩnh cứng nhắc theo tháng. Khi chi vượt mức đầu tháng, app chỉ đổi màu đỏ thụ động gây chán nản. | Động cơ Hạn mức Chi tiêu Mỗi ngày Thích ứng (Adaptive Daily Spending Allowance - ADSA). | Liên tục co giãn hạn mức ngày dựa trên số ngày còn lại trong kỳ và Tốc độ tiêu tiền thực tế (Spending Velocity Index). Giúp người dùng tái cân đối êm ái mà không vỡ ngân sách. |
| **3. Quản trị & Giải phóng Nợ** | Ghi chép nợ đơn giản như một giao dịch thu/chi, không có công cụ tính lãi, không có lộ trình trả nợ. | Động cơ Tối ưu hóa Nợ Cá nhân: So sánh đối kháng Debt Snowball vs Debt Avalanche Solver. | Mô phỏng chính xác lộ trình trả nợ: Snowball (trả nợ nhỏ trước lấy động lực) vs Avalanche (trả lãi cao trước tiết kiệm tiền). Ưu tiên bảo vệ Quỹ khẩn cấp 3 - 6 tháng sinh hoạt. |
| **4. Can thiệp Tâm lý Hành vi** | Thông báo đẩy khô khan, nhắc nhở ghi sổ định kỳ, không tác động được vào tâm lý bốc đồng lúc mua sắm. | Động cơ Cú hích Hành vi Tài chính (Thaler Behavioral Nudge) & Khoảng trễ tâm lý (Cooling-off Period 24h - 48h). | Áp dụng Kiến trúc lựa chọn phi cưỡng chế: Tạm khóa khoản chi xa xỉ trong 24h, quy đổi giá trị món hàng thành số giờ lao động thực tế, giúp người dùng suy ngẫm lý trí. |
| **5. Tính Toàn vẹn Dữ liệu Đa Ví** | Số dư ảo, cho phép sửa tùy tiện, chuyển ví không có khóa an toàn, dễ sai lệch số dư thực tế. | Kiến trúc Đa Ví chuẩn ngân hàng với Giao dịch ACID và Khóa bi quan (Pessimistic Locking). | Mọi biến động nạp, rút, chuyển ví nội bộ được bao bọc trong transaction CSDL PostgreSQL 3NF, bảo đảm tính toàn vẹn 100% khớp thực tế đời sống. |

---

## 3. MÔ HÌNH HÓA USE CASE TOÀN DIỆN (65 USE CASES & 4 TÁC NHÂN CỐT LÕI)

### 3.1 Chuẩn Hóa 4 Tác Nhân Cốt Lõi Hệ Thống (Core Actors)
Theo chuẩn phân tích hướng đối tượng UML (Alistair Cockburn & IEEE 29148), hệ thống chuẩn hóa về **đúng 4 tác nhân cốt lõi**, loại bỏ sự phân mảnh kỹ thuật:
1. **Người dùng cá nhân (Personal User)** *(Tác nhân chính - Primary Human Actor):* Chủ sở hữu dữ liệu tài chính, trực tiếp khởi tạo ví, lập ngân sách, xác nhận giao dịch và kích hoạt kế hoạch tài chính.
2. **Hệ thống Trí tuệ Nhân tạo Đa nhiệm (Multimodal AI System)** *(Tác nhân phụ - Secondary System Actor):* Nền tảng AI ngoại vi (Google Gemini 2.5 Flash / NLP Whisper) cung cấp năng lực nhận thức thông minh: OCR hóa đơn, bóc tách phiếu lương, nhận diện giọng nói tự nhiên, phân tích xu hướng và giải trình thuật toán minh bạch (Explainable AI).
3. **Cổng thanh toán VietQR (VietQR Gateway / NAPAS 247)** *(Tác nhân phụ - Secondary System Actor):* Dịch vụ cổng thanh toán quốc gia mã hóa thông tin giao dịch thành chuỗi mã QR chuẩn EMVCo Merchant-Presented Mode phục vụ thanh toán không tiền mặt.
4. **Hạ tầng Di động & Dịch vụ Thông báo (Mobile OS & Notification Infrastructure)** *(Tác nhân phụ - Secondary System Actor):* Kênh tương tác hai chiều với thiết bị di động:
   * *Chiều vào (Inbound):* Android `NotificationListenerService` bắt sự kiện biến động số dư từ các app ngân hàng (ACB, MoMo, VCB, TCB, MBBank...).
   * *Chiều ra (Outbound):* Đẩy các cảnh báo vi mô (Micro-alerts), nhắc nợ và phát tín hiệu Cú hích hành vi Nudge qua Firebase Cloud Messaging (FCM).

### 3.2 Danh Mục 65 Use Cases Phân Bổ Qua 9 Phân Hệ Chức Năng

```
┌────────────────────────────────────────────────────────────────────────┐
│               TỔNG THỂ 65 USE CASES THEO 9 PHÂN HỆ NGHIỆP VỤ           │
├────────────────────────────────────────────────────────────────────────┤
│ PH1: Quản lý Tài khoản & Đa Ví (10 UCs)   [UC-01.01 -> UC-01.10]       │
│ PH2: Quản lý Thu nhập Cá nhân (8 UCs)     [UC-02.01 -> UC-02.08]       │
│ PH3: Chi tiêu & Bóc tách AI OCR (12 UCs)  [UC-03.01 -> UC-03.12]       │
│ PH4: Danh mục Thu Chi & Quy tắc (7 UCs)   [UC-04.01 -> UC-04.07]       │
│ PH5: Ngân sách & Hạn mức DSA (6 UCs)      [UC-05.01 -> UC-05.06]       │
│ PH6: Quỹ Tích lũy & Quản trị Nợ (7 UCs)   [UC-06.01 -> UC-06.07]       │
│ PH7: Báo cáo & Phân tích Đa chiều (6 UCs) [UC-07.01 -> UC-07.06]       │
│ PH8: Dự báo Tài chính & Rủi ro (4 UCs)    [UC-08.01 -> UC-08.04]       │
│ PH9: Cú hích Hành vi Thaler Nudge (5 UCs) [UC-09.01 -> UC-09.05]       │
└────────────────────────────────────────────────────────────────────────┘
```

* **Các điểm mới bổ sung then chốt:**
  * `UC-01.10`: Đối soát số dư ví qua ảnh chụp màn hình ngân hàng (Balance Screenshot OCR).
  * `UC-02.08`: Bóc tách phiếu lương tự động bằng AI Vision OCR (Automated Payslip OCR Parsing).
  * `UC-03.11`: Ghi nhận chi tiêu nhanh bằng khẩu lệnh giọng nói (Voice-to-Expense).
  * `UC-03.12`: Tự động phát hiện biến động số dư qua Thông báo Android (`NotificationListenerService`).
  * `UC-07.06`: Nhập khẩu dữ liệu tài chính đa nguồn Hybrid (Excel/CSV & Unstructured Note AI).

---

## 4. KIẾN TRÚC HƯỚNG MIỀN DOMAIN-DRIVEN DESIGN (DDD) & CLASS DIAGRAM (6 BOUNDED CONTEXTS)

Kiến trúc hệ thống tuân thủ nghiêm ngặt phương pháp luận **Domain-Driven Design (DDD)** của Eric Evans, kiên quyết loại bỏ mô hình thiếu máu (Anemic Domain Model). Toàn bộ thực thể miền đều là các **Rich Domain Model** sở hữu hành vi và các quy tắc nghiệp vụ bất biến:

```
+---------------------------------------------------------------------------------------------------+
|                                      PFM BOUNDED CONTEXT MAP                                      |
+---------------------------------------------------------------------------------------------------+
|  [ CONTEXT 1: IDENTITY & PROFILE ] (Core Domain)                                                  |
|  - User, Money (Value Object bất biến), DateRange (Value Object)                                  |
+---------------------------------------------------------------------------------------------------+
               |                                 |                                 |
               v                                 v                                 v
+-----------------------------+   +-----------------------------+   +-----------------------------+
| [ CONTEXT 2: MULTI-WALLET ] |   | [ CONTEXT 4: BUDGET & DSA ] |   | [ CONTEXT 5: DEBT & GOALS ] |
| - Wallet (Abstract)         |   | - Budget (Aggregate Root)   |   | - FinancialGoal / Emergency |
| - Cash, Bank, Credit, EWal  |   | - BudgetAllocation          |   | - DebtAccount               |
| - InternalTransfer (ACID)   |   | - DailySpendingAllowance    |   | - DebtPaymentPlan (Solver)  |
| - BalanceAdjustment         |   |   (DSA Adaptive Engine)     |   | - DebtPaymentTransaction    |
+-----------------------------+   +-----------------------------+   +-----------------------------+
               ^                                 ^                                 ^
               | (Trừ/Cộng tiền ACID)            | (Cộng dồn chi tiêu thực)        | (Trích quỹ nợ/tích lũy)
+---------------------------------------------------------------------------------------------------+
|  [ CONTEXT 3: TRANSACTIONS, AI VISION OCR & CATEGORIES ]                                          |
|  - Transaction (Abstract) -> IncomeTransaction / ExpenseTransaction                               |
|  - Receipt *-- ReceiptItem (Composition - Hậu kiểm toán số học verifyMathematicalAudit)           |
|  - Category (Cây phân cấp đệ quy Cha - Con) & AutoCategorizationRule                              |
|  - Subscription (Giám sát bẫy thuê bao định kỳ)                                                   |
|  - Interfaces: IBankNotificationParserService & IHybridDataImportService                          |
+---------------------------------------------------------------------------------------------------+
                                                 | (Kích hoạt cú hích khi vượt ngưỡng)
                                                 v
+---------------------------------------------------------------------------------------------------+
|  [ CONTEXT 6: DECISION SUPPORT & BEHAVIORAL NUDGE (THALER) ]                                      |
|  - FinancialNudge (Cú hích hành vi, Khoảng trễ tâm lý Cooling-off 24h, Explainable AI Rationale)  |
+---------------------------------------------------------------------------------------------------+
```

### 4.1 Điểm Nhấn Thiết Kế Hướng Đối Tượng & Value Objects
1. **Value Object bất biến `Money`:** Đóng gói số tiền (`decimal` 18,2) và mã tiền tệ ISO-4217 (`string`). Đảm bảo không bao giờ bị thay đổi trạng thái sau khi khởi tạo, trang bị các phương thức an toàn `add()`, `subtract()`, `multiply()`, `allocate()` triệt tiêu lỗi mất mát số học.
2. **Value Object `DateRange`:** Đảm bảo bất biến `startDate <= endDate`, kiểm tra giao thoa chu kỳ với `includesDate()` và `overlapsWith()`.
3. **Quan hệ Hợp thành Hóa đơn `Receipt *-- ReceiptItem`:** Toàn bộ dòng mặt hàng thuộc quyền sở hữu sống còn của hóa đơn gốc. Phương thức `verifyMathematicalAudit()` tự động duyệt các món để đối soát phương trình kế toán:
   $$\text{Delta} = \left| \sum (\text{Quantity} \times \text{UnitPrice}) + \text{VAT} - \text{TotalAmount} \right|$$
   Chỉ khi $\text{Delta} == 0$, giao dịch mới được chuyển trạng thái `VERIFIED`.

---

## 5. THIẾT KẾ MÔ HÌNH ĐỘNG & TƯƠNG TÁC HỆ THỐNG (11 SƠ ĐỒ ĐỘNG)

Mô hình động làm rõ sự tương tác theo thời gian, phân định ranh giới xử lý bất đồng bộ và kiểm soát luân chuyển trạng thái:

### 5.1 Thiết Kế Sơ Đồ Tuần Tự (Sequence Diagrams - 5 Kịch Bản Trọng Yếu)
1. **SD-01: Bóc tách hóa đơn AI Vision OCR & Hậu kiểm toán số học:**
   * Mobile gửi ảnh $\to$ Backend cấp mã nháp và trả ngay mã `HTTP 202 Accepted` để giải phóng giao diện.
   * Worker chạy ngầm gọi Vision LLM trích xuất JSON đa món $\to$ `MathAuditEngine` đối soát độc lập $\Delta = 0$.
   * Nếu khớp: tự động ghi sổ cái `VERIFIED`. Nếu sai lệch: gắn cờ nghi vấn yêu cầu người dùng xác nhận trực quan.
2. **SD-02: Ghi nhận chi tiêu bằng giọng nói tự nhiên (Voice-to-Expense):**
   * Mobile nén luồng âm thanh $\to$ Whisper nhận dạng văn bản tiếng Việt $\to$ NLP Extractor bóc tách 4 thực thể cốt lõi (Số tiền, Danh mục, Ví nguồn, Thời gian).
   * Phân nhánh: Độ tin cậy $\ge 85\%$ tự động ghi sổ cái 0ms; $< 85\%$ hiển thị form điền sẵn để người dùng bổ sung.
3. **SD-03: Thanh toán VietQR & Hạch toán đa ví chuẩn ACID:**
   * Quét mã EMVCo $\to$ `PaymentService` khởi tạo khóa phân tán Redis (`Idempotency Key`) chống thanh toán trùng lặp.
   * Mở giao dịch CSDL với Khóa bi quan, kiểm tra số dư khả dụng, thực hiện trừ ví và sinh biên lai điện tử vĩnh viễn.
4. **SD-04: Động cơ tính hạn mức DSA & Phát tín hiệu Cú hích Nudge:**
   * Kích hoạt đầu ngày hoặc sau mỗi giao dịch $\to$ `ADSA Engine` chia ngân sách còn lại cho số ngày còn lại trong kỳ.
   * Đo lường Tốc độ tiêu tiền $V$. Nếu $V > 1.2$ (nguy cơ cạn tiền sớm), hệ thống kích hoạt thông điệp cảnh báo vi mô kèm giải trình minh bạch XAI.
5. **SD-05: Kiểm soát chi tiêu lớn với Khoảng trễ tâm lý 24 giờ:**
   * Phát hiện khoản chi $\ge 3.000.000$ VNĐ hoặc $\ge 30\%$ thu nhập tháng $\to$ Quy đổi số tiền thành giờ công lao động thực tế.
   * Chuyển trạng thái sang `PENDING_COOLING_OFF`, đóng băng 24h chưa trừ tiền ví. Sau 24h, người dùng suy ngẫm lý trí trước khi xác nhận chi hoặc hủy bỏ.

### 5.2 Thiết Kế Sơ Đồ Hoạt Động (Activity Diagrams - 3 Quy Trình Đa Rãnh Bơi)
1. **AD-01 (Hoạt động bóc tách hóa đơn):** Phân chia 5 rãnh bơi (Mobile Client $\to$ API Gateway & Queue $\to$ Vision LLM Worker $\to$ Math Audit Engine $\to$ Database & FCM Notification).
2. **AD-02 (Hoạt động kiểm soát chi tiêu lớn):** Luồng quyết định phân nhánh kiểm tra ngưỡng, hiển thị bảng quy đổi giờ công lao động và xác nhận sau khoảng trễ.
3. **AD-03 (Hoạt động tính toán DSA & Nudge):** Luồng thu thập dữ liệu ngân sách, tính toán ADSA, phân vùng rủi ro (Xanh - Vàng - Đỏ) và đóng gói payload cú hích.

### 5.3 Thiết Kế Sơ Đồ Máy Trạng Thái (State Machine Diagrams - 3 Vòng Đời Thực Thể Miền)
1. **STM-01 (Vòng đời Giao dịch Chi tiêu - Expense):**  
   $$\text{DRAFT} \xrightarrow{\text{Audit Pass}} \text{VERIFIED} \xrightarrow{\text{Chi lớn}} \text{PENDING\_COOLING\_OFF} \xrightarrow{\text{Xác nhận}} \text{COMPLETED} \quad (\text{hoặc } \text{CANCELLED})$$
2. **STM-02 (Vòng đời Kế hoạch Trả nợ - DebtPaymentPlan):**  
   $$\text{PLANNING} \xrightarrow{\text{Kích hoạt}} \text{ACTIVE} \xrightarrow{\text{Thặng dư dồn thêm}} \text{ACCELERATED} \xrightarrow{\text{Trễ hạn}} \text{OVERDUE\_RISK} \xrightarrow{\text{Hết nợ}} \text{SETTLED}$$
3. **STM-03 (Vòng đời Cú hích Hành vi - FinancialNudge):**  
   $$\text{TRIGGERED} \xrightarrow{\text{Đóng gói}} \text{DISPATCHED} \xrightarrow{\text{Hiển thị}} \text{DISPLAYED} \xrightarrow{\text{Xem XAI}} \text{EXPLAINED} \xrightarrow{\text{Tiếp thu}} \text{CONVERTED} \quad (\text{hoặc } \text{DISMISSED})$$

---

## 6. THIẾT KẾ CƠ SỞ DỮ LIỆU QUAN HỆ 3NF (POSTGRESQL 16 ACID)

### 6.1 Kiến Trúc Bố Cục ERD 5 Cột Logic Phân Kênh (Zero-Collision Routing)
Sơ đồ cơ sở dữ liệu vật lý bao gồm **22 bảng thực thể** và **25 đường liên kết quan hệ khóa ngoại**, được bố trí thành 5 cột chuyên môn hóa độc lập trong file [DatabaseERD.drawio](file:///d:/University%20Study%20Subjects/Information%20System%20Design%20Analysis/Project/Analyze%20System/files/DatabaseERD.drawio):
* **Cột 1 (Người dùng & Ví tiền cốt lõi):** `users`, `wallets`, `wallet_transfers`, `balance_adjustments`, `net_worth_snapshots`, `InAppNotifications`.
* **Cột 2 (Danh mục & Dịch vụ định kỳ):** `categories`, `auto_categorization_rules`, `subscriptions`, `recurring_incomes`, `CoolingOffTransactions`.
* **Cột 3 (Sổ cái Giao dịch & Bóc tách Hóa đơn):** `transactions`, `salary_slip_details`, `receipt_items`, `FinancialNudges`.
* **Cột 4 (Hạn mức Ngân sách & Trợ lý DSA):** `budgets`, `budget_allocations`, `daily_spending_allowances`.
* **Cột 5 (Nợ Cá nhân & Mục tiêu Tích lũy):** `debts`, `debt_transactions`, `savings_goals`, `savings_goal_contributions`.

### 6.2 Từ Điển Dữ Liệu 22 Bảng Thực Thể Vật Lý Chuẩn 3NF
Toàn bộ 22 bảng được chuẩn hóa đạt Chuẩn dạng 3 (3NF), khóa ngoại có ràng buộc toàn vẹn tham chiếu (`ON DELETE CASCADE` hoặc `RESTRICT`), kiểu dữ liệu tiền tệ chính xác cao `DECIMAL(18,2)` và định danh UUID độc lập:
1. `users`: Thông tin tài khoản, mật khẩu băm, lương cơ sở, khẩu vị rủi ro.
2. `wallets`: Các loại ví tài chính (Tiền mặt, Ngân hàng, Thẻ tín dụng, Ví điện tử).
3. `wallet_transfers`: Nhật ký chuyển tiền giữa 2 ví chuẩn ACID.
4. `balance_adjustments`: Lịch sử kiểm kê đối soát số dư thực tế.
5. `net_worth_snapshots`: Ảnh chụp giá trị tài sản ròng theo thời gian.
6. `InAppNotifications`: Thông báo đẩy và cảnh báo nội ứng dụng.
7. `categories`: Cây phân cấp danh mục thu chi (Parent-Child).
8. `auto_categorization_rules`: Quy tắc regex/keyword tự động gắn thẻ danh mục.
9. `subscriptions`: Quản lý dịch vụ trừ tiền định kỳ và bẫy thuê bao lãng phí.
10. `recurring_incomes`: Theo dõi các nguồn thu nhập định kỳ hàng tháng.
11. `CoolingOffTransactions`: Lưu trữ các giao dịch chi tiêu lớn đang đóng băng 24h.
12. `transactions`: Sổ cái trung tâm lưu trữ toàn bộ giao dịch tài chính.
13. `salary_slip_details`: Bóc tách chi tiết phiếu lương (Gross, Net, BHXH, Thuế TNCN).
14. `receipt_items`: Chi tiết từng mặt hàng trên hóa đơn bóc tách AI OCR.
15. `FinancialNudges`: Quản lý các cú hích hành vi và giải trình lý do XAI.
16. `budgets`: Kế hoạch ngân sách chu kỳ (50/30/20, Zero-Based, Kakeibo).
17. `budget_allocations`: Hạn mức chi tiết giao cho từng danh mục.
18. `daily_spending_allowances`: Hạn mức chi tiêu mỗi ngày thích ứng (DSA) và chỉ số Velocity.
19. `debts`: Danh mục nợ cá nhân, lãi suất phần trăm và kỳ hạn trả.
20. `debt_transactions`: Nhật ký thanh toán trả nợ (phân tách gốc và lãi).
21. `savings_goals`: Mục tiêu tích lũy tài sản và Quỹ khẩn cấp 3 - 6 tháng.
22. `savings_goal_contributions`: Nhật ký trích tiền đóng góp vào mục tiêu tích lũy.

---

## 7. KIẾN TRÚC HIỆN THỰC HÓA NGUYÊN MẪU & KẾT QUẢ KIỂM THỬ THỰC TẾ

### 7.1 Ngăn Xếp Công Nghệ Chuẩn Hóa (Production Tech Stack)
* **Backend:** **.NET 10 (C# 14)** xây dựng theo **Clean Architecture** (Domain, Application, Infrastructure, WebApi), Entity Framework Core 10, CSDL PostgreSQL 16 ACID, Redis Cache & Distributed Lock, SignalR Hub thời gian thực.
* **Frontend Mobile/Desktop:** **.NET MAUI 10** đa nền tảng (Android Native & Windows Desktop), kiến trúc MVVM CommunityToolkit, SkiaSharp biểu đồ tài chính, thiết kế vi thẩm mỹ Dark/Light theme chuẩn phong cách Apple HIG.
* **Bộ nhân AI & Nhận thức:** Google Gemini 2.5 Flash Multimodal Vision API, Whisper AI Speech-to-Text, Offline Regex Engine tiếng Việt tài chính.

### 7.2 Các Tính Năng Đột Phá Đã Hoàn Thiện Trên Nguyên Mẫu
1. **Android Native Notification Listener:** Tích hợp trực tiếp `NotificationListenerService` cấp hệ điều hành Android, tự động lắng nghe và giải mã biến động số dư trừ tiền từ **9 ứng dụng ngân hàng lớn tại Việt Nam** (ACB, MoMo, Vietcombank, Techcombank, MBBank, BIDV, VPBank, TPBank, ZaloPay).
2. **Thanh Thử Nghiệm Giả Lập 5 Ngân Hàng (Simulation Test Bar):** Trang bị thanh nút 1-chạm (ACB Emart -350k, MoMo Circle K -85k, Techcombank Highlands -120k, Vietcombank ShopeePay -500k, MBBank Petrolimex -200k) phục vụ trình diễn bảo vệ đồ án ngay tại bàn mà không phụ thuộc vào tiền thật trong tài khoản ngân hàng.
3. **Hòm Thư Hành Động Tự Hành (Action Inbox & Jev Protocol):** Phân tầng xác suất thông minh:
   * Confidence $\ge 0.85$: Nút 1-chạm xác nhận tức thì 0ms vào sổ cái.
   * $0.50 \le \text{Confidence} < 0.85$: Khối lựa chọn đồng cấp Jev Choice giải quyết nhập nhằng.
   * Confidence $< 0.50$: Thẻ chờ phân loại thủ công an toàn.
4. **AI Vision OCR Bóc Tách Hóa Đơn Đa Món & Phiếu Lương:** Trích xuất chi tiết từng món hàng, tỷ lệ chính xác cao, tự động đối soát cân bằng kế toán $\Delta = 0$.
5. **Trợ Lý Giọng Nói Hai Chiều (AI Voice Assistant):** Nhận diện khẩu lệnh tiếng Việt đời thường ("2 củ rưỡi", "năm xị", "hai trẹo"), định tuyến thông minh vào 5 phân hệ chuyên sâu.
6. **Mô Phỏng Trả Nợ Đối Kháng:** So sánh trực quan biểu đồ dòng tiền giữa Snowball và Avalanche, tính toán chính xác số tiền lãi tiết kiệm được và ngày sạch nợ dự kiến.

### 7.3 Kim Tự Tháp Kiểm Thử & Nghiệm Thu Chất Lượng Toàn Trình
Hệ thống đạt tiêu chuẩn kiểm thử tự động nghiêm ngặt nhất với độ bao phủ kiểm thử cao:
* **Backend Domain Tests:** **113 / 113 Tests PASSED (100%)** — Kiểm tra Money Value Object, Rich Domain Logic, Công thức DSA, Snowball/Avalanche Solver.
* **Backend Application Tests:** **264 / 264 Tests PASSED (100%)** — Kiểm tra Use Case Handlers, Pipeline OCR, Idempotency, Phân loại ngân hàng NLP.
* **Mobile Client Tests:** **139 / 139 Tests PASSED (100%)** — Kiểm tra ViewModels, Action Inbox Protocol, Điều hướng Shell, Offline Cache.
* **TỔNG CỘNG HỆ THỐNG:** **516 / 516 Tests PASSED 100% (0 Lỗi, 0 Warning)**.

---
*Tài liệu tổng quan kiến trúc này là tài sản kỹ thuật trung tâm, đồng bộ 100% với Khóa luận Tốt nghiệp, Sơ đồ Draw.io và Mã nguồn Nguyên mẫu Thực tế.*
