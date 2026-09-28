# Kế Hoạch Triển Khai Lab 3: Phân Tích & Xây Dựng Class Diagram Hệ Thống Quản Lý Chi Tiêu Cá Nhân (PFM)
*Snapshot thời điểm phê duyệt: 2026-09-11 10:31:00*

---

## 1. Bối Cảnh & Mục Tiêu Chuẩn Hóa (Standardized Context & Objectives)

Theo chỉ đạo mới nhất từ Người dùng và bộ tiêu chuẩn phương pháp luận học thuật của Giảng viên cho **Bài tập Lab 3 (Phân tích Thiết kế Hệ thống Thông tin)**:
1. **Thiết lập Class Diagram (CD) làm mục tiêu ưu tiên số 1**, áp dụng chuẩn mực mô hình hóa hướng đối tượng (OOAD) theo đúng 5 giai đoạn cốt lõi và 6 tiêu chí sàng lọc chặt chẽ của giảng viên.
2. **Quy chuẩn kỹ thuật biểu diễn Sơ đồ lớp (Diagram Modeling Standards):**
   - **Cấu trúc hộp lớp (3 ngăn chuẩn UML):**
     1. Ngăn 1: Tên lớp (*Class Name*) kèm Stereotype (`<<Abstract>>`, `<<Value Object>>`, `<<Interface>>`).
     2. Ngăn 2: Danh sách thuộc tính (*Attributes*) có chỉ định mức độ truy cập (`-` private, `#` protected, `+` public) và **kiểu dữ liệu tường minh** (`UUID`, `String`, `BigDecimal`, `Money`, `LocalDate`, `DateTime`, `Boolean`, `Enum`).
     3. Ngăn 3: Danh sách phương thức (*Methods*) có chỉ định phạm vi truy cập, **kiểu dữ liệu tham số đầu vào và kiểu dữ liệu trả về**.
   - **Hệ thống quan hệ liên kết đầy đủ (UML Relationships):**
     - **Generalization (Kế thừa):** Mũi tên tam giác rỗng nét liền ($\triangle \text{---}$).
     - **Realization (Hiện thực hóa Interface):** Mũi tên tam giác rỗng nét đứt ($\triangle \text{- - -}$).
     - **Composition (Hợp thành - Vòng đời phụ thuộc 1 - n):** Hình thoi đen đặc nét liền ($\blacklozenge \text{---}$).
     - **Aggregation (Thu nạp - Toàn thể / Bộ phận lỏng lẻo):** Hình thoi trắng rỗng nét liền ($\lozenge \text{---}$).
     - **Association (Liên kết ngữ nghĩa):** Đường nét liền có hoặc không có mũi tên chỉ hướng ($\longrightarrow$).
     - **Dependency (Phụ thuộc):** Đường nét đứt có mũi tên ($\dashrightarrow$).
3. **Định hướng CSDL: Thiết kế tương thích chuẩn Microsoft SQL Server (MSSQL):**
   - Theo chỉ đạo Người dùng, hệ thống kiểu dữ liệu thuộc tính trong Sơ đồ lớp và mô hình quan hệ sẽ được chuẩn hóa tương thích 100% với **chuẩn MSSQL T-SQL** (`UNIQUEIDENTIFIER`, `NVARCHAR`, `DECIMAL(18, 2)`, `DATETIME2`, `DATE`, `BIT`, `INT`).
   - Thiết kế đảm bảo tính linh hoạt (Portability): Dễ dàng chuyển đổi tương đương sang PostgreSQL (`UUID`, `VARCHAR`, `NUMERIC`, `TIMESTAMPTZ`, `BOOLEAN`) hoặc hệ CSDL khác trong các giai đoạn sau nếu người dùng yêu cầu.
4. **Tiêu chuẩn công cụ: DRAW.IO (`.drawio`) là công cụ chuẩn hóa ưu tiên duy nhất:**
   - Tạo tệp mã nguồn Draw.io kỹ thuật trực quan: `files/class_diagram_pfm_lab3.drawio`.
   - Bảo đảm tính đồng nhất thị giác (Visual Consistency) về bảng màu, tỷ lệ và typography nét chữ (Segoe UI) với bộ Use Case Diagram 10 tabs (`files/UCD-DuoNoPro.drawio`).
5. **Xuất bản Báo cáo học thuật định dạng Markdown (`.md`):**
   - Toàn bộ nội dung phân tích chi tiết của Lab 3 sẽ được xuất bản vào tệp:
     `project_context/reports/BaoCao_Lab3_ThietKe_Class_Diagram_PFM.md`.
   - Tệp báo cáo này phục vụ chấm điểm Lab 3 và tích hợp trực tiếp vào **Mục 3.4 (Thiết kế các lớp đối tượng)** trong Khóa luận tốt nghiệp (`KhoaLuanTotNghiep.docx`).

---

## 2. Tiêu Chí Đối Soát & Kiểm Tra Học Thuật Của Giảng Viên (Academic Verification Criteria)

Bộ tiêu chí này được áp dụng xuyên suốt để rà soát, đánh giá và tái cấu trúc mô hình lớp:

```text
┌──────────────────────────────────────────────────────────────────────────────────┐
│ TIÊU CHÍ PHÂN TÍCH & TÁI XÂY DỰNG CLASS DIAGRAM (CHUẨN GIẢNG VIÊN DNTU)           │
├──────────────────────────────────────────────────────────────────────────────────┤
│ 1. TÌM LỚP DỰ KIẾN (Candidate Identification):                                  │
│   • Dựa vào DANH TỪ trong các lời phát biểu của bài toán (Mục 1.7 I-P-O).        │
│   • Dựa vào mô hình Use Case để tìm ra các lớp dự kiến (50 Use Cases).           │
│   • Xác định thông tin có cần được LƯU TRỮ hay PHÂN TÍCH không?                  │
│   • Xác định có GIAO TIẾP VỚI HỆ THỐNG NGOẠI VI không?                           │
│   • Tham khảo các lớp, thư viện lớp mẫu của những hệ thống quản lý tài chính.     │
├──────────────────────────────────────────────────────────────────────────────────┤
│ 2. LOẠI BỎ LỚP KHÔNG PHÙ HỢP (6 Tiêu chí sàng lọc loại trừ):                     │
│   [C1] Có hơn 1 lớp cùng định nghĩa 1 đối tượng -> Giữ lại lớp tốt nhất.        │
│   [C2] Lớp định nghĩa đối tượng KHÔNG LIÊN QUAN đến thực tại hệ thống.           │
│   [C3] Lớp KHÔNG CÓ CHỨC NĂNG cụ thể -> Định nghĩa lại hoặc loại bỏ.             │
│   [C4] Lớp này chỉ là VAI TRÒ, CHỨC NĂNG của một lớp khác -> Gom vào thuộc tính. │
│   [C5] Lớp mang tính ĐỘNG TỪ -> Chuyển thành PHƯƠNG THỨC của một lớp.            │
│   [C6] Lớp KHÔNG CÓ PHƯƠNG THỨC (Anemic) -> Bổ sung phương thức hoặc loại bỏ.    │
├──────────────────────────────────────────────────────────────────────────────────┤
│ 3. TÌM THUỘC TÍNH: Xác định tên, kiểu dữ liệu chuẩn MSSQL, phạm vi truy cập.     │
│ 4. TÌM PHƯƠNG THỨC: Nghiệp vụ giàu tính toán, kiểm soát Invariants, đủ kiểu trả. │
│ 5. QUAN HỆ & LỚP PHÁT SINH: Association, Aggregation, Composition, Kế thừa, v.v. │
└──────────────────────────────────────────────────────────────────────────────────┘
```

---

## 3. Nội Dung Chi Tiết 5 Giai Đoạn Phân Tích & Thiết Kế

### GIAI ĐOẠN 1: TÌM LỚP DỰ KIẾN (CANDIDATE CLASSES IDENTIFICATION)
*Áp dụng 5 nguồn phát hiện ứng viên:*
1. **Nguồn 1: Danh từ trong Phát biểu bài toán Mục 1.7 (I-P-O):**
   - *Inputs:* Người dùng, Hồ sơ người dùng, Khẩu vị rủi ro, Danh mục ví, Ví tiền mặt, Tài khoản ngân hàng, Ví điện tử, Thẻ tín dụng, Dòng tiền thu nhập, Giao dịch chi tiêu, Hóa đơn mua sắm bán lẻ, Dòng mặt hàng, Thuế VAT, Chiết khấu, Mã VietQR, Lịch đăng ký trả phí định kỳ, Nghĩa vụ nợ cá nhân, Dư nợ gốc, Lãi suất, Hạn ngạch trả nợ tối thiểu, Ngân sách chu kỳ, Quỹ khẩn cấp.
   - *Processing Engine:* Xử lý giao dịch đa ví ACID, Bóc tách hóa đơn tự động, Kiểm toán số học, Hoạch định ngân sách thích ứng DSA, Giám sát tốc độ tiêu tiền, Định tuyến trả nợ tối ưu (Snowball, Avalanche), Cú hích hành vi Nudge, Phát hiện bẫy thuê bao ngủ quên (Zombie Subscriptions), Khoảng trễ tâm lý mua sắm (Cooling-off Period).
   - *Outputs & Decision Support:* Bảng điều khiển tài chính Dashboard, Chỉ số tài sản ròng Net Worth, Dòng tiền ròng Net Cashflow, Lịch trình trả nợ trực quan, Cảnh báo ngày cạn tiền, Giải trình lý do AI Nudge.
2. **Nguồn 2: Dựa vào mô hình 50 Use Case trong `DanhSach_UseCase_HopNhat.md`:**
   - Đăng ký, Đăng nhập, Hồ sơ, Đa Ví, Cân chỉnh số dư, Chuyển tiền nội bộ, Khoản thu nhập, Nguồn thu, Chi tiêu, Hóa đơn OCR, Danh mục, Cây danh mục Cha-Con, Quy tắc tự động, Ngân sách, Hạn mức ngày DSA, Mục tiêu tài chính, Nợ cá nhân, Kế hoạch nợ, Báo cáo tài chính, Dự báo rủi ro, Cú hích hành vi.
3. **Nguồn 3: Đánh giá nhu cầu Lưu trữ & Phân tích:**
   - Các thực thể mang trạng thái bền vững cần lưu trữ: Người dùng, Ví, Giao dịch, Hóa đơn, Dòng mặt hàng, Thuê bao định kỳ, Danh mục, Ngân sách, Phân bổ danh mục, DSA ngày, Mục tiêu, Khoản nợ, Kế hoạch trả nợ, Cú hích.
4. **Nguồn 4: Đánh giá giao tiếp với hệ thống ngoại vi:**
   - Các cổng giao tiếp: `AIVisionOCRService`, `AIRecommendationService`, `NotificationService`, `VietQRGateway` $\rightarrow$ Xác định là External Systems/Interfaces, không phải Domain Entity nội bộ.
5. **Nguồn 5: Tham khảo lớp mẫu từ các chuẩn PFM (YNAB, Money Lover, DDD):**
   - Value Object `Money` (bất biến, số thực chính xác cao), Value Object `DateRange`, Khái niệm Đa Ví chuẩn ngân hàng, Cơ chế kiểm toán hóa đơn.

---

### GIAI ĐOẠN 2: ĐỐI SOÁT & LOẠI BỎ LỚP THEO 6 TIÊU CHÍ HỌC THUẬT (ELIMINATION ANALYSIS)

| Tên Lớp Dự Kiến | Trạng Thái | Tiêu Chí Loại Trừ Của GV | Phân Tích & Lý Giải Học Thuật Chi Tiết |
| :--- | :---: | :---: | :--- |
| **PersonalUser** | **LOẠI** | **[C1] Trùng lặp đối tượng** | Cùng định nghĩa đối tượng người dùng với `User`. `PersonalUser` là tác nhân con người vật lý ngoài ranh giới; hệ thống giữ lại lớp thực thể tốt nhất là `User`. |
| **User** | **GIỮ** | *Đạt chuẩn* | Thực thể cốt lõi lưu trữ danh tính, hồ sơ, khẩu vị rủi ro và các cấu hình cá nhân. |
| **Account** | **LOẠI** | **[C1] Trùng lặp đối tượng** | Trùng lặp ngữ nghĩa với `User` (tài khoản đăng nhập) và `Wallet` (tài khoản tài chính). Tích hợp thuộc tính đăng nhập vào `User`. |
| **Password**, **JWTToken**, **Session** | **LOẠI** | **[C4] Chỉ là thuộc tính/vai trò** | Không phải thực thể nghiệp vụ độc lập; mật khẩu chuyển thành thuộc tính `passwordHash` trong `User`, Token/Session thuộc cơ chế hạ tầng xác thực. |
| **UserProfile**, **RiskProfile** | **LOẠI** | **[C4] Chỉ là thuộc tính của lớp khác** | Là các nhóm thông tin hồ sơ của `User`. Tích hợp thành các thuộc tính `fullName`, `riskTolerance: RiskLevel`, `salaryDay` trong thực thể `User`. |
| **Wallet** | **GIỮ** | *Đạt chuẩn* | Lớp cha trừu tượng (`<<Abstract>>`) quản lý số dư và tính toàn vẹn ACID của tài sản cá nhân. |
| **CashWallet**, **BankAccountWallet**, **CreditCardWallet**, **EWallet** | **GIỮ** | *Đạt chuẩn* | 4 lớp con kế thừa `Wallet` định nghĩa các loại ví có thuộc tính và hành vi nghiệp vụ đặc thù (hạn mức tín dụng, sao kê, STK...). |
| **InternalTransfer** | **GIỮ** | *Đạt chuẩn* | Thực thể liên kết giữa 2 ví, quản lý giao dịch luân chuyển dòng tiền nội bộ nguyên tử ACID. |
| **BalanceAdjustment** | **GIỮ** | *Đạt chuẩn* | Thực thể quản lý biên bản đối soát và cân chỉnh sai lệch giữa số dư thực tế và số dư sổ sách. |
| **Transaction** | **GIỮ** | *Đạt chuẩn* | Lớp cha trừu tượng (`<<Abstract>>`) của mọi biến động dòng tiền vào và ra. |
| **IncomeTransaction**, **ExpenseTransaction** | **GIỮ** | *Đạt chuẩn* | Kế thừa `Transaction`: Chứa thuộc tính và hành vi chuyên biệt cho dòng tiền thu và dòng tiền chi. |
| **Amount**, **Currency** | **LOẠI (lớp đơn)** | **[C6] Không có phương thức nếu đứng đơn lẻ** | Nếu để tách rời chỉ là kiểu dữ liệu cơ bản. Áp dụng DDD chuẩn hóa thành Value Object bất biến `Money` có đầy đủ phương thức tính toán chính xác cao. |
| **Money** | **GIỮ** | *Đạt chuẩn* | Value Object bất biến (`<<Value Object>>`) chứa `amount: BigDecimal` và `currency: String` kèm các phương thức số học an toàn. |
| **DateRange** | **GIỮ** | *Đạt chuẩn* | Value Object bất biến biểu diễn chu kỳ thời gian (`startDate`, `endDate`). |
| **Receipt** | **GIỮ** | *Đạt chuẩn* | Thực thể Aggregate Root quản lý ảnh chụp hóa đơn, siêu dữ liệu cửa hàng, tổng tiền và trạng thái kiểm toán. |
| **ReceiptItem** | **GIỮ** | *Đạt chuẩn* | Thành phần cấu thành (Composition) của `Receipt`, lưu chi tiết từng dòng hàng hóa, đơn giá, thuế VAT. |
| **MathematicalAudit** | **LOẠI** | **[C5] Mang tính ĐỘNG TỪ** | Kiểm toán số học là một hành động/quy trình tính toán, không phải thực thể lưu trữ; chuyển thành phương thức `verifyMathematicalAudit(): Boolean` trên thực thể `Receipt`. |
| **Subscription** | **GIỮ** | *Đạt chuẩn* | Thực thể quản lý các cam kết dịch vụ trả phí định kỳ, phát hiện bẫy gia hạn ngầm và zombie subscriptions. |
| **VietQRPayment** | **LOẠI** | **[C4] Chỉ là chức năng của lớp khác** | Là phương thức hỗ trợ thanh toán cho chi tiêu; chuyển thành trường `vietQRReference` và hành vi mở rộng trên `ExpenseTransaction`. |
| **Category** | **GIỮ** | *Đạt chuẩn* | Thực thể quản lý cây phân cấp danh mục Cha - Con và ánh xạ các trường phái ngân sách (50/30/20, Kakeibo). |
| **AutoCategorizationRule** | **GIỮ** | *Đạt chuẩn* | Thực thể quản lý quy tắc từ khóa tự động gán giao dịch vào danh mục. |
| **AutoCategorization** | **LOẠI** | **[C5] Mang tính ĐỘNG TỪ** | Là hành động phân loại; chuyển thành phương thức `matches(text): Boolean` trên `AutoCategorizationRule`. |
| **Budget** | **GIỮ** | *Đạt chuẩn* | Thực thể Aggregate Root quản lý kế hoạch ngân sách theo chu kỳ. |
| **BudgetAllocation** | **GIỮ** | *Đạt chuẩn* | Thực thể liên kết phát sinh giữa `Budget` và `Category`, lưu hạn mức phân bổ và tiến độ chi. |
| **DailySpendingAllowance (DSA)** | **GIỮ** | *Đạt chuẩn* | Thực thể tính toán hạn mức chi tiêu khả dụng an toàn mỗi ngày và giám sát Spending Velocity. |
| **FinancialGoal** | **GIỮ** | *Đạt chuẩn* | Lớp cha quản lý mục tiêu tích lũy tài chính. |
| **EmergencyFundGoal** | **GIỮ** | *Đạt chuẩn* | Kế thừa `FinancialGoal`: Quỹ khẩn cấp chuyên biệt (3 - 6 tháng sinh hoạt phí). |
| **GoalContribution** | **GIỮ** | *Đạt chuẩn* | Thực thể liên kết phát sinh giữa `FinancialGoal` và `Wallet` lưu vết các lần nạp tiền mục tiêu. |
| **DebtAccount** | **GIỮ** | *Đạt chuẩn* | Thực thể quản lý hồ sơ nghĩa vụ nợ cá nhân (gốc, lãi suất, ngày đến hạn, khoản trả tối thiểu). |
| **DebtPaymentPlan** | **GIỮ** | *Đạt chuẩn* | Thực thể quản lý kế hoạch mô phỏng lộ trình tất toán nợ theo chiến lược Snowball hoặc Avalanche. |
| **DebtRepaymentScheduleItem** | **GIỮ** | *Đạt chuẩn* | Thành phần cấu thành của `DebtPaymentPlan`, lưu chi tiết phân bổ trả nợ từng tháng. |
| **DebtPaymentTransaction** | **GIỮ** | *Đạt chuẩn* | Thực thể liên kết phát sinh giữa `DebtAccount` và `Wallet` lưu lịch sử thanh toán nợ thực tế. |
| **SnowballStrategy**, **AvalancheStrategy** | **LOẠI** | **[C3] Không có chức năng lưu trữ độc lập** | Chuyển thành kiểu liệt kê `DebtStrategyType: Enum` và các phương thức thuật toán mô phỏng trong `DebtPaymentPlan`. |
| **FinancialNudge** | **GIỮ** | *Đạt chuẩn* | Thực thể lưu trữ các cú hích tài chính, căn cứ giải trình và kiểm soát khoảng trễ Cooling-off. |
| **ExplainableRationale**, **CoolingOffPeriodTimer** | **LOẠI** | **[C4] Chỉ là thuộc tính của lớp khác** | Chuyển thành các thuộc tính `rationaleExplanation: String` và `coolingOffUntil: DateTime` trong thực thể `FinancialNudge`. |
| **IncomeExpenseReport**, **NetWorthReport**, **NetCashflowReport**, **OutOfMoneyDateForecast**, **WhatIfSimulationScenario** | **LOẠI** | **[C2] Không phải thực thể trạng thái gốc** | Là các mô hình hiển thị báo cáo (Query/Read Models / DTOs) được sinh ra từ các phép tính tổng hợp, không quản lý trạng thái giao dịch gốc. |
| **AIVisionOCRService**, **AIRecommendationService**, **NotificationService**, **VietQRGateway** | **LOẠI** | **[C2] Tác nhân ngoài ranh giới hệ thống** | Là các dịch vụ đám mây bên ngoài (External Services), được tích hợp qua tầng Service Client/Adapter, không phải Domain Class nội bộ. |
| **PostgreSQLDatabase**, **RedisCache**, **MessageQueue** | **LOẠI** | **[C2] Thành phần hạ tầng kỹ thuật** | Thuộc tầng cơ sở hạ tầng (Infrastructure Layer), không thuộc miền nghiệp vụ phân tích thiết kế hướng đối tượng. |
| **GroupExpense**, **RoommateSplitBill**, **SharedFlatFund** | **LOẠI** | **[C2] Không liên quan đến thực tại hệ thống** | Bị loại bỏ triệt để theo quyết định khóa phạm vi 100% Quản lý Tài chính Cá nhân (PLAN-09). |

---

### GIAI ĐOẠN 3: TÌM THUỘC TÍNH, KIỂU DỮ LIỆU & BẢNG ÁNH XẠ CHUẨN MSSQL

#### Bảng Ánh Xạ Kiểu Dữ Liệu Miền Nghiệp Vụ Sang Chuẩn MSSQL T-SQL:

| Kiểu Dữ Liệu Mô Hình Lớp (OOAD) | Kiểu Dữ Liệu Chuẩn MSSQL (T-SQL) | Ghi Chú Kỹ Thuật & Khả Năng Porting Sang PostgreSQL |
| :--- | :--- | :--- |
| `UUID` | `UNIQUEIDENTIFIER` | Khóa chính chuẩn toàn cầu (NEWID() / NEWSEQUENTIALID()); tương đương `UUID` bên Postgres. |
| `String` (Ngắn / Mã hiệu) | `NVARCHAR(50)` / `NVARCHAR(100)` | Chuỗi Unicode hỗ trợ tiếng Việt có dấu; tương đương `VARCHAR(n)` bên Postgres. |
| `String` (Mô tả / URL) | `NVARCHAR(255)` / `NVARCHAR(500)` | Lưu đường dẫn ảnh, ghi chú; tương đương `VARCHAR(n)` bên Postgres. |
| `String` (Văn bản dài / JSON) | `NVARCHAR(MAX)` | Lưu giải trình AI Nudge, cấu hình; tương đương `TEXT` hoặc `JSONB` bên Postgres. |
| `Money` (Thuộc tính amount) | `DECIMAL(18, 2)` | Độ chính xác cao chống sai số làm tròn; tương đương `NUMERIC(18, 2)` bên Postgres. |
| `BigDecimal` (Tỷ lệ / Thuế VAT) | `DECIMAL(5, 4)` / `DECIMAL(5, 2)` | Lưu tỷ lệ thuế (0.0800), tỷ lệ phần trăm (80.00%); tương đương `NUMERIC` bên Postgres. |
| `LocalDate` | `DATE` | Lưu ngày không kèm giờ (`YYYY-MM-DD`); tương đương `DATE` bên Postgres. |
| `DateTime` | `DATETIME2(3)` | Lưu thời gian chính xác mili-giây; tương đương `TIMESTAMPTZ` bên Postgres. |
| `Boolean` | `BIT` | Giá trị nhị phân `0` hoặc `1`; tương đương `BOOLEAN` bên Postgres. |
| `Integer` / `Enum` | `INT` / `TINYINT` | Lưu số nguyên hoặc mã số Enum; tương đương `INTEGER` / `SMALLINT` bên Postgres. |

---

### GIAI ĐOẠN 4: TÌM PHƯƠNG THỨC CHO LỚP (IDENTIFY METHODS & BUSINESS OPERATIONS)
Tất cả 18 lớp đều được trang bị phương thức nghiệp vụ giàu tính toán (Rich Domain Model), kiểm soát toàn vẹn Invariants, có đầy đủ kiểu dữ liệu tham số và giá trị trả về.

---

### GIAI ĐOẠN 5: XÁC ĐỊNH QUAN HỆ & CÁC LỚP PHÁT SINH
1. **Generalization:** `Wallet` $\leftarrow$ 4 lớp con; `Transaction` $\leftarrow$ 2 lớp con; `FinancialGoal` $\leftarrow$ `EmergencyFundGoal`.
2. **Composition:** `Receipt` $\blacklozenge\longrightarrow$ `ReceiptItem`; `Budget` $\blacklozenge\longrightarrow$ `BudgetAllocation`; `DebtPaymentPlan` $\blacklozenge\longrightarrow$ `DebtRepaymentScheduleItem`.
3. **Aggregation:** `User` $\lozenge\longrightarrow$ `Wallet`, `Budget`, `FinancialGoal`, `DebtAccount`, `Subscription`, `FinancialNudge`; `Category` cây đệ quy Cha - Con.
4. **Association:** `Wallet` $1 \longleftrightarrow *$ `Transaction`; `Category` $1 \longleftrightarrow *$ `Transaction`; `ExpenseTransaction` $0..1 \longleftrightarrow 0..1$ `Receipt`.
5. **Dependency:** `ExpenseTransaction` $\dashrightarrow$ `FinancialGoal`; `DebtPaymentPlan` $\dashrightarrow$ `DebtAccount`.
6. **Lớp Phát Sinh / Lớp Liên Kết:** `BudgetAllocation`, `GoalContribution`, `DebtPaymentTransaction`, `InternalTransfer`.

---

## 4. Danh Sách Tệp Đầu Ra
1. `project_context/plans/2026-09-11_plan_12_thiet_ke_class_diagram_lab_3.md` (Tệp này).
2. `project_context/decisions_and_changes_log.md` (Quyết định 26).
3. `project_context/reports/BaoCao_Lab3_ThietKe_Class_Diagram_PFM.md` (Báo cáo học thuật chi tiết).
4. `files/class_diagram_pfm_lab3.drawio` (Bản vẽ Draw.io kỹ thuật trực quan).
