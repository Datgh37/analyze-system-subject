# data_attribute_tables.py
# Contains attribute data dictionaries for all 20 classes across 6 Bounded Contexts

ATTR_HEADERS = ["STT", "Tên thuộc tính", "Kiểu dữ liệu (OOAD / MSSQL)", "Ràng buộc", "Mô tả ý nghĩa nghiệp vụ"]
COL_WIDTHS = [0.45, 1.35, 1.50, 0.80, 2.00]

# CONTEXT 1
USER_ATTRS = [
    ["1", "- userId", "UUID / UNIQUEIDENTIFIER", "PK", "Khóa chính định danh duy nhất người dùng."],
    ["2", "- email", "String / NVARCHAR(100)", "UNIQUE", "Địa chỉ email đăng nhập và xác thực tài khoản."],
    ["3", "- passwordHash", "String / NVARCHAR(255)", "NOT NULL", "Chuỗi mật khẩu băm an toàn (Argon2id/bcrypt)."],
    ["4", "- fullName", "String / NVARCHAR(100)", "NOT NULL", "Họ và tên đầy đủ của người dùng."],
    ["5", "- defaultCurrency", "String / NVARCHAR(10)", "DEFAULT 'VND'", "Đơn vị tiền tệ hiển thị mặc định của hệ thống."],
    ["6", "- salaryDay", "Integer / TINYINT", "CHECK(1-31)", "Ngày nhận lương hàng tháng chốt chu kỳ tài chính."],
    ["7", "- riskTolerance", "RiskLevel / TINYINT", "ENUM(1,2,3)", "Khẩu vị rủi ro (1: Thấp, 2: Vừa, 3: Cao)."],
    ["8", "- isActive", "Boolean / BIT", "DEFAULT 1", "Cờ trạng thái tài khoản đang hoạt động."],
    ["9", "- createdAt", "DateTime / DATETIME2(3)", "DEFAULT GETDATE()", "Thời điểm người dùng khởi tạo tài khoản."]
]

VALUE_OBJECTS_ATTRS = [
    ["1", "- amount (Money)", "BigDecimal / DECIMAL(18, 2)", "NOT NULL", "Giá trị tiền tệ số thực định lượng, triệt tiêu sai số."],
    ["2", "- currency (Money)", "String / NVARCHAR(10)", "NOT NULL", "Mã đồng tiền chuẩn ISO (mặc định: 'VND')."],
    ["3", "- startDate (DateRange)", "LocalDate / DATE", "NOT NULL", "Ngày bắt đầu chu kỳ tài chính đóng."],
    ["4", "- endDate (DateRange)", "LocalDate / DATE", "NOT NULL", "Ngày kết thúc chu kỳ (ràng buộc: startDate <= endDate)."]
]

# CONTEXT 2
WALLET_ATTRS = [
    ["1", "# walletId (Wallet)", "UUID / UNIQUEIDENTIFIER", "PK", "Khóa chính định danh ví tài chính."],
    ["2", "# userId (Wallet)", "UUID / UNIQUEIDENTIFIER", "FK -> User", "Khóa ngoại trỏ đến người dùng sở hữu ví."],
    ["3", "# walletName (Wallet)", "String / NVARCHAR(100)", "NOT NULL", "Tên gợi nhớ của ví (ví dụ: Tiền mặt, VCB chính)."],
    ["4", "# balance (Wallet)", "Money / DECIMAL(18, 2)", "NOT NULL", "Số dư khả dụng hiện tại được bảo toàn ACID."],
    ["5", "# walletType (Wallet)", "WalletType / TINYINT", "ENUM(1..4)", "Phân loại ví (CASH, BANK, CREDIT, EWALLET)."],
    ["6", "# isActive (Wallet)", "Boolean / BIT", "DEFAULT 1", "Trạng thái ví đang được kích hoạt sử dụng."],
    ["7", "# createdAt (Wallet)", "DateTime / DATETIME2(3)", "DEFAULT GETDATE()", "Thời điểm mở ví tài sản trên hệ thống."],
    ["8", "- isSafeDeposit (CashWallet)", "Boolean / BIT", "DEFAULT 0", "Đánh dấu tiền mặt cất trong két sắt/tủ dự phòng."],
    ["9", "- bankCode (BankAccountWallet)", "String / NVARCHAR(20)", "NOT NULL", "Mã ngân hàng định danh (VCB, TCB, MB, ACB...)."],
    ["10", "- accountNumber (BankAccountWallet)", "String / NVARCHAR(50)", "NOT NULL", "Số tài khoản ngân hàng của người dùng."],
    ["11", "- branch (BankAccountWallet)", "String / NVARCHAR(100)", "NULL", "Chi nhánh mở tài khoản ngân hàng."],
    ["12", "- creditLimit (CreditCardWallet)", "Money / DECIMAL(18, 2)", "NOT NULL", "Hạn mức tín dụng tối đa ngân hàng cấp cho thẻ."],
    ["13", "- statementDay (CreditCardWallet)", "Integer / TINYINT", "CHECK(1-31)", "Ngày chốt sao kê giao dịch quẹt thẻ hàng tháng."],
    ["14", "- paymentDueDay (CreditCardWallet)", "Integer / TINYINT", "CHECK(1-31)", "Hạn chót thanh toán dư nợ thẻ được miễn lãi."],
    ["15", "- currentUtilized (CreditCardWallet)", "Money / DECIMAL(18, 2)", "DEFAULT 0", "Dư nợ thẻ đã chi tiêu trong chu kỳ sao kê hiện tại."],
    ["16", "- providerName (EWallet)", "EWalletProvider / TINYINT", "ENUM(1,2,3)", "Nhà cung cấp ví điện tử (MOMO, ZALOPAY, VNPAY)."],
    ["17", "- linkedPhoneNumber (EWallet)", "String / NVARCHAR(20)", "NOT NULL", "Số điện thoại đăng ký liên kết tài khoản ví."]
]

TRANSFER_ADJUSTMENT_ATTRS = [
    ["1", "- transferId (Transfer)", "UUID / UNIQUEIDENTIFIER", "PK", "Khóa chính giao dịch chuyển tiền nội bộ."],
    ["2", "- fromWalletId (Transfer)", "UUID / UNIQUEIDENTIFIER", "FK -> Wallet", "Ví nguồn bị trích trừ tiền."],
    ["3", "- toWalletId (Transfer)", "UUID / UNIQUEIDENTIFIER", "FK -> Wallet", "Ví đích được cộng tiền đồng thời."],
    ["4", "- amount (Transfer)", "Money / DECIMAL(18, 2)", "NOT NULL", "Số tiền luân chuyển giữa hai ví."],
    ["5", "- fee (Transfer)", "Money / DECIMAL(18, 2)", "DEFAULT 0", "Phí luân chuyển tiền nội bộ (nếu có)."],
    ["6", "- timestamp (Transfer)", "DateTime / DATETIME2(3)", "DEFAULT GETDATE()", "Thời điểm giao dịch chuyển tiền ACID hoàn tất."],
    ["7", "- note (Transfer)", "String / NVARCHAR(255)", "NULL", "Ghi chú mục đích luân chuyển dòng tiền."],
    ["8", "- adjustmentId (Adjustment)", "UUID / UNIQUEIDENTIFIER", "PK", "Khóa chính biên bản kiểm đếm cân chỉnh số dư."],
    ["9", "- walletId (Adjustment)", "UUID / UNIQUEIDENTIFIER", "FK -> Wallet", "Ví tài sản phát sinh chênh lệch kiểm kê."],
    ["10", "- recordedBalance (Adjustment)", "Money / DECIMAL(18, 2)", "NOT NULL", "Số dư trên hệ thống trước khi điều chỉnh."],
    ["11", "- actualBalance (Adjustment)", "Money / DECIMAL(18, 2)", "NOT NULL", "Số dư thực tế người dùng vừa kiểm đếm."],
    ["12", "- differenceAmount (Adjustment)", "Money / DECIMAL(18, 2)", "NOT NULL", "Chênh lệch số dư (actualBalance - recordedBalance)."],
    ["13", "- reason (Adjustment)", "String / NVARCHAR(255)", "NOT NULL", "Lý do sai lệch thực tế (quên ghi chép, rơi tiền)."],
    ["14", "- timestamp (Adjustment)", "DateTime / DATETIME2(3)", "DEFAULT GETDATE()", "Thời điểm lập biên bản cân chỉnh bất biến."]
]

# CONTEXT 3
TRANSACTION_ATTRS = [
    ["1", "# transactionId (Tx)", "UUID / UNIQUEIDENTIFIER", "PK", "Khóa chính định danh giao dịch thu chi."],
    ["2", "# walletId (Tx)", "UUID / UNIQUEIDENTIFIER", "FK -> Wallet", "Ví phát sinh biến động tăng hoặc giảm số dư."],
    ["3", "# categoryId (Tx)", "UUID / UNIQUEIDENTIFIER", "FK -> Category", "Danh mục phân loại giao dịch."],
    ["4", "# amount (Tx)", "Money / DECIMAL(18, 2)", "NOT NULL", "Giá trị khoản tiền biến động."],
    ["5", "# transactionDate (Tx)", "DateTime / DATETIME2(3)", "NOT NULL", "Ngày giờ phát sinh giao dịch thực tế."],
    ["6", "# note (Tx)", "String / NVARCHAR(500)", "NULL", "Ghi chú nội dung chi tiết của giao dịch."],
    ["7", "# transactionType (Tx)", "TransactionType / TINYINT", "ENUM(1:IN, 2:EX)", "Phân loại biến động (1: Thu nhập, 2: Chi tiêu)."],
    ["8", "- sourceType (IncomeTx)", "IncomeSourceType / TINYINT", "ENUM", "Nguồn thu (Lương, Thưởng, Freelance, Đầu tư)."],
    ["9", "- isRecurring (IncomeTx)", "Boolean / BIT", "DEFAULT 0", "Cờ đánh dấu khoản thu lặp lại định kỳ."],
    ["10", "- recurringInterval (IncomeTx)", "IntervalType / TINYINT", "ENUM", "Chu kỳ lặp của khoản thu (Tuần, Tháng)."],
    ["11", "- receiptId (ExpenseTx)", "UUID / UNIQUEIDENTIFIER", "FK -> Receipt, NULL", "Liên kết tới hóa đơn bán lẻ bóc tách AI OCR."],
    ["12", "- goalId (ExpenseTx)", "UUID / UNIQUEIDENTIFIER", "FK -> Goal, NULL", "Liên kết tới mục tiêu tích lũy nếu là khoản chi quỹ."],
    ["13", "- isRecurring (ExpenseTx)", "Boolean / BIT", "DEFAULT 0", "Cờ đánh dấu khoản chi phí định kỳ cố định."],
    ["14", "- coolingOffApplied (ExpenseTx)", "Boolean / BIT", "DEFAULT 0", "Đánh dấu giao dịch đã qua thời gian trễ tâm lý."],
    ["15", "- vietQRReference (ExpenseTx)", "String / NVARCHAR(100)", "NULL", "Mã đối soát giao dịch chuyển khoản Napas VietQR."]
]

CATEGORY_RULE_ATTRS = [
    ["1", "- categoryId (Category)", "UUID / UNIQUEIDENTIFIER", "PK", "Khóa chính danh mục thu chi."],
    ["2", "- userId (Category)", "UUID / UNIQUEIDENTIFIER", "FK -> User, NULL", "Null nếu là danh mục hệ thống mặc định."],
    ["3", "- parentCategoryId (Category)", "UUID / UNIQUEIDENTIFIER", "FK -> Category, NULL", "Tự tham chiếu tạo cây phân cấp Cha - Con."],
    ["4", "- categoryName (Category)", "String / NVARCHAR(100)", "NOT NULL", "Tên danh mục (Ăn uống, Tiền trọ, Siêu thị...)."],
    ["5", "- icon (Category)", "String / NVARCHAR(50)", "NULL", "Tên mã biểu tượng hiển thị trên giao diện."],
    ["6", "- colorCode (Category)", "String / NVARCHAR(20)", "NULL", "Mã màu nhận diện giao diện hex code (#0F2C59)."],
    ["7", "- budgetPillar503020 (Cat)", "Pillar503020 / TINYINT", "ENUM(1,2,3)", "Trụ cột quy tắc 50/30/20 (Needs, Wants, Savings)."],
    ["8", "- kakeiboPillar (Category)", "PillarKakeibo / TINYINT", "ENUM(1..4)", "Trụ cột sổ Kakeibo (Thiết yếu, Lựa chọn, Văn hóa...)."],
    ["9", "- isDefault (Category)", "Boolean / BIT", "DEFAULT 0", "Cờ phân biệt danh mục hệ thống hay người dùng tạo."],
    ["10", "- ruleId (Rule)", "UUID / UNIQUEIDENTIFIER", "PK", "Khóa chính quy tắc gán nhãn tự động."],
    ["11", "- userId (Rule)", "UUID / UNIQUEIDENTIFIER", "FK -> User", "Người dùng thiết lập quy tắc tự động hóa."],
    ["12", "- keywordPattern (Rule)", "String / NVARCHAR(100)", "NOT NULL", "Từ khóa hoặc biểu thức Regex nhận diện ('Grab', 'WinMart')."],
    ["13", "- targetCategoryId (Rule)", "UUID / UNIQUEIDENTIFIER", "FK -> Category", "Danh mục đích tự động gán khi khớp từ khóa."],
    ["14", "- priorityOrder (Rule)", "Integer / INT", "DEFAULT 1", "Thứ tự ưu tiên áp dụng quy tắc (số nhỏ ưu tiên trước)."]
]

RECEIPT_ATTRS = [
    ["1", "- receiptId (Receipt)", "UUID / UNIQUEIDENTIFIER", "PK", "Khóa chính hóa đơn bán lẻ bóc tách AI."],
    ["2", "- transactionId (Receipt)", "UUID / UNIQUEIDENTIFIER", "FK -> ExpenseTx, NULL", "Giao dịch chi tiêu gắn kết với hóa đơn."],
    ["3", "- imageStorageUrl (Receipt)", "String / NVARCHAR(500)", "NOT NULL", "Đường dẫn ảnh hóa đơn gốc trên Cloud Storage."],
    ["4", "- merchantName (Receipt)", "String / NVARCHAR(150)", "NULL", "Tên siêu thị / cửa hàng phát hành hóa đơn."],
    ["5", "- merchantTaxCode (Receipt)", "String / NVARCHAR(50)", "NULL", "Mã số thuế của đơn vị bán hàng."],
    ["6", "- purchaseDate (Receipt)", "DateTime / DATETIME2(3)", "NULL", "Ngày giờ mua sắm in trên hóa đơn."],
    ["7", "- subTotal (Receipt)", "Money / DECIMAL(18, 2)", "NULL", "Tổng tiền hàng trước thuế."],
    ["8", "- taxAmount (Receipt)", "Money / DECIMAL(18, 2)", "NULL", "Tổng tiền thuế giá trị gia tăng (VAT)."],
    ["9", "- discountAmount (Receipt)", "Money / DECIMAL(18, 2)", "DEFAULT 0", "Tổng tiền chiết khấu khuyến mãi."],
    ["10", "- grandTotal (Receipt)", "Money / DECIMAL(18, 2)", "NOT NULL", "Tổng tiền thanh toán cuối cùng của hóa đơn."],
    ["11", "- auditStatus (Receipt)", "AuditStatus / TINYINT", "ENUM(1..4)", "Trạng thái kiểm toán (MATCHED, DISCREPANCY, VERIFIED)."],
    ["12", "- scannedAt (Receipt)", "DateTime / DATETIME2(3)", "DEFAULT GETDATE()", "Thời điểm tải ảnh hóa đơn và kích hoạt AI OCR."],
    ["13", "- itemId (ReceiptItem)", "UUID / UNIQUEIDENTIFIER", "PK", "Khóa chính dòng mặt hàng bóc tách."],
    ["14", "- receiptId (ReceiptItem)", "UUID / UNIQUEIDENTIFIER", "FK -> Receipt", "Hóa đơn cha chứa mặt hàng (CASCADE DELETE)."],
    ["15", "- itemName (ReceiptItem)", "String / NVARCHAR(200)", "NOT NULL", "Tên mặt hàng bóc tách (Sữa tươi, Gạo, Dầu ăn...)."],
    ["16", "- quantity (ReceiptItem)", "BigDecimal / DECIMAL(10, 3)", "NOT NULL", "Số lượng mặt hàng mua (hỗ trợ số lẻ kg/lít)."],
    ["17", "- unitPrice (ReceiptItem)", "Money / DECIMAL(18, 2)", "NOT NULL", "Đơn giá của một đơn vị mặt hàng."],
    ["18", "- totalPrice (ReceiptItem)", "Money / DECIMAL(18, 2)", "NOT NULL", "Thành tiền dòng hàng (quantity * unitPrice)."],
    ["19", "- vatRate (ReceiptItem)", "BigDecimal / DECIMAL(5, 4)", "DEFAULT 0", "Thuế suất VAT của mặt hàng (ví dụ: 0.0800 tương đương 8%)."],
    ["20", "- matchedCategoryId (Item)", "UUID / UNIQUEIDENTIFIER", "FK -> Category, NULL", "Danh mục tự động gán cho mặt hàng con."]
]

SUBSCRIPTION_ATTRS = [
    ["1", "- subscriptionId", "UUID / UNIQUEIDENTIFIER", "PK", "Khóa chính dịch vụ thuê bao định kỳ."],
    ["2", "- userId", "UUID / UNIQUEIDENTIFIER", "FK -> User", "Người dùng đăng ký dịch vụ."],
    ["3", "- walletId", "UUID / UNIQUEIDENTIFIER", "FK -> Wallet", "Ví tài sản tự động trích nợ định kỳ."],
    ["4", "- serviceName", "String / NVARCHAR(150)", "NOT NULL", "Tên dịch vụ (Netflix, Spotify, iCloud, ChatGPT Plus)."],
    ["5", "- billingCycle", "BillingCycle / TINYINT", "ENUM(1,2,3)", "Chu kỳ trừ tiền (1: Tháng, 2: Quý, 3: Năm)."],
    ["6", "- recurringAmount", "Money / DECIMAL(18, 2)", "NOT NULL", "Số tiền thanh toán cố định mỗi chu kỳ."],
    ["7", "- nextBillingDate", "LocalDate / DATE", "NOT NULL", "Ngày gia hạn và trừ tiền kỳ tiếp theo."],
    ["8", "- lastUsedDate", "LocalDate / DATE", "NULL", "Ngày gần nhất người dùng thực tế tương tác dịch vụ."],
    ["9", "- isZombieDetected", "Boolean / BIT", "DEFAULT 0", "Cờ cảnh báo bẫy thuê bao ngủ quên lãng phí."],
    ["10", "- status", "SubscriptionStatus / TINYINT", "ENUM(1,2,3)", "Trạng thái dịch vụ (ACTIVE, PAUSED, CANCELLED)."]
]

# CONTEXT 4
BUDGET_ATTRS = [
    ["1", "- budgetId (Budget)", "UUID / UNIQUEIDENTIFIER", "PK", "Khóa chính kế hoạch ngân sách."],
    ["2", "- userId (Budget)", "UUID / UNIQUEIDENTIFIER", "FK -> User", "Người lập kế hoạch ngân sách."],
    ["3", "- budgetName (Budget)", "String / NVARCHAR(100)", "NOT NULL", "Tên kế hoạch (ví dụ: Ngân sách Tháng 10/2026)."],
    ["4", "- periodType (Budget)", "PeriodType / TINYINT", "ENUM(1,2)", "Chu kỳ phân bổ ngân sách (Tháng, Tuần)."],
    ["5", "- startDate (Budget)", "LocalDate / DATE", "NOT NULL", "Ngày bắt đầu áp dụng ngân sách."],
    ["6", "- endDate (Budget)", "LocalDate / DATE", "NOT NULL", "Ngày kết thúc áp dụng ngân sách."],
    ["7", "- totalBudgetAmount (Budget)", "Money / DECIMAL(18, 2)", "NOT NULL", "Tổng hạn mức tiền cấp cho toàn chu kỳ."],
    ["8", "- budgetMethod (Budget)", "BudgetMethod / TINYINT", "ENUM(1,2,3)", "Phương pháp ngân sách (50/30/20, ZBB, Kakeibo)."],
    ["9", "- status (Budget)", "BudgetStatus / TINYINT", "ENUM(1,2)", "Trạng thái ngân sách (1: ACTIVE, 2: CLOSED)."],
    ["10", "- allocationId (Alloc)", "UUID / UNIQUEIDENTIFIER", "PK", "Khóa chính dòng phân bổ danh mục."],
    ["11", "- budgetId (Alloc)", "UUID / UNIQUEIDENTIFIER", "FK -> Budget", "Ngân sách cha (CASCADE DELETE)."],
    ["12", "- categoryId (Alloc)", "UUID / UNIQUEIDENTIFIER", "FK -> Category", "Danh mục được cấp hạn mức chi tiêu."],
    ["13", "- allocatedAmount (Alloc)", "Money / DECIMAL(18, 2)", "NOT NULL", "Số tiền hạn mức tối đa cho danh mục."],
    ["14", "- currentSpent (Alloc)", "Money / DECIMAL(18, 2)", "DEFAULT 0", "Số tiền lũy kế đã tiêu thực tế trong chu kỳ."],
    ["15", "- alertThresholdPercent (Alloc)", "BigDecimal / DECIMAL(5, 2)", "DEFAULT 80.00", "Ngưỡng phần trăm chi tiêu kích hoạt cảnh báo vi mô."]
]

DSA_ATTRS = [
    ["1", "- dsaId", "UUID / UNIQUEIDENTIFIER", "PK", "Khóa chính bản ghi tính toán DSA hàng ngày."],
    ["2", "- budgetId", "UUID / UNIQUEIDENTIFIER", "FK -> Budget", "Kế hoạch ngân sách đang theo dõi."],
    ["3", "- calculationDate", "LocalDate / DATE", "NOT NULL", "Ngày tính toán hạn mức khả dụng."],
    ["4", "- dailyAllowanceAmount", "Money / DECIMAL(18, 2)", "NOT NULL", "Hạn mức tiền an toàn được phép tiêu trong ngày."],
    ["5", "- actualSpentToday", "Money / DECIMAL(18, 2)", "DEFAULT 0", "Tổng tiền thực tế đã chi trong ngày hôm nay."],
    ["6", "- remainingAllowanceToday", "Money / DECIMAL(18, 2)", "NOT NULL", "Hạn mức ngày còn lại (dailyAllowance - actualSpent)."],
    ["7", "- spendingVelocity", "BigDecimal / DECIMAL(5, 4)", "NOT NULL", "Chỉ số vận tốc tiêu tiền (V_actual / V_target)."],
    ["8", "- outOfMoneyDateForecast", "LocalDate / DATE", "NULL", "Ngày dự phóng cạn sạch tiền nếu giữ nguyên tốc độ tiêu."]
]

# CONTEXT 5
GOAL_ATTRS = [
    ["1", "# goalId (Goal)", "UUID / UNIQUEIDENTIFIER", "PK", "Khóa chính mục tiêu tài chính."],
    ["2", "# userId (Goal)", "UUID / UNIQUEIDENTIFIER", "FK -> User", "Người thiết lập mục tiêu tích lũy."],
    ["3", "# goalName (Goal)", "String / NVARCHAR(100)", "NOT NULL", "Tên mục tiêu (Mua laptop, Quỹ học thạc sĩ...)."],
    ["4", "# targetAmount (Goal)", "Money / DECIMAL(18, 2)", "NOT NULL", "Số tiền mục tiêu cần tích lũy."],
    ["5", "# currentAccumulated (Goal)", "Money / DECIMAL(18, 2)", "DEFAULT 0", "Số tiền hiện đã nạp tích lũy vào quỹ."],
    ["6", "# targetDate (Goal)", "LocalDate / DATE", "NOT NULL", "Hạn chót dự kiến hoàn thành mục tiêu."],
    ["7", "# priority (Goal)", "Integer / INT", "DEFAULT 1", "Mức độ ưu tiên tích lũy vốn."],
    ["8", "# status (Goal)", "GoalStatus / TINYINT", "ENUM(1,2,3)", "Trạng thái (IN_PROGRESS, COMPLETED, CANCELLED)."],
    ["9", "- targetLivingMonths (Emerg)", "Integer / TINYINT", "DEFAULT 6", "Số tháng sinh hoạt phí dự phòng (3 - 6 tháng)."],
    ["10", "- monthlyEssentialNeed (Emerg)", "Money / DECIMAL(18, 2)", "NOT NULL", "Chi phí sinh hoạt tối thiểu bắt buộc một tháng."],
    ["11", "- contributionId (Contrib)", "UUID / UNIQUEIDENTIFIER", "PK", "Khóa chính lượt nạp tiền vào mục tiêu."],
    ["12", "- goalId (Contrib)", "UUID / UNIQUEIDENTIFIER", "FK -> Goal", "Mục tiêu được tiếp vốn (CASCADE DELETE)."],
    ["13", "- walletId (Contrib)", "UUID / UNIQUEIDENTIFIER", "FK -> Wallet", "Ví tài sản trích tiền nạp."],
    ["14", "- amount (Contrib)", "Money / DECIMAL(18, 2)", "NOT NULL", "Số tiền trích nạp vào quỹ mục tiêu."],
    ["15", "- contributionDate (Contrib)", "DateTime / DATETIME2(3)", "DEFAULT GETDATE()", "Thời điểm thực hiện nạp tiền tích lũy."],
    ["16", "- note (Contrib)", "String / NVARCHAR(255)", "NULL", "Ghi chú động lực tiết kiệm."]
]

DEBT_ACCOUNT_ATTRS = [
    ["1", "- debtId", "UUID / UNIQUEIDENTIFIER", "PK", "Khóa chính hồ sơ khoản nợ cá nhân."],
    ["2", "- userId", "UUID / UNIQUEIDENTIFIER", "FK -> User", "Người chịu trách nhiệm trả nợ."],
    ["3", "- creditorName", "String / NVARCHAR(150)", "NOT NULL", "Tên chủ nợ / tổ chức tín dụng (FE Credit, VIB, Bạn bè)."],
    ["4", "- initialPrincipal", "Money / DECIMAL(18, 2)", "NOT NULL", "Số tiền nợ gốc ban đầu khi ký kết vay."],
    ["5", "- remainingPrincipal", "Money / DECIMAL(18, 2)", "NOT NULL", "Dư nợ gốc thực tế còn lại cần thanh toán."],
    ["6", "- nominalAnnualRate", "BigDecimal / DECIMAL(6, 4)", "NOT NULL", "Lãi suất danh nghĩa hàng năm (ví dụ: 0.1850 = 18.5%/năm)."],
    ["7", "- minimumMonthlyPayment", "Money / DECIMAL(18, 2)", "NOT NULL", "Số tiền tối thiểu bắt buộc thanh toán mỗi kỳ."],
    ["8", "- dueDayOfMonth", "Integer / TINYINT", "CHECK(1-31)", "Ngày hạn chót đóng tiền nợ hàng tháng."],
    ["9", "- status", "DebtStatus / TINYINT", "ENUM(1,2,3)", "Trạng thái khoản nợ (ACTIVE, PAID_OFF, DEFAULTED)."]
]

DEBT_PLAN_TRANSACTION_ATTRS = [
    ["1", "- planId (Plan)", "UUID / UNIQUEIDENTIFIER", "PK", "Khóa chính kế hoạch trả nợ tối ưu."],
    ["2", "- userId (Plan)", "UUID / UNIQUEIDENTIFIER", "FK -> User", "Người lập kế hoạch trả nợ."],
    ["3", "- strategyType (Plan)", "DebtStrategyType / TINYINT", "ENUM(1:SN, 2:AV)", "Chiến lược tối ưu (1: Snowball, 2: Avalanche)."],
    ["4", "- monthlyExtraCashflow (Plan)", "Money / DECIMAL(18, 2)", "NOT NULL", "Dòng tiền dôi dư hàng tháng dồn thêm trả nợ."],
    ["5", "- estimatedTotalInterest (Plan)", "Money / DECIMAL(18, 2)", "NOT NULL", "Tổng tiền lãi dự kiến trả trong suốt lộ trình."],
    ["6", "- estimatedPayoffMonths (Plan)", "Integer / INT", "NOT NULL", "Số tháng dự kiến để sạch nợ hoàn toàn."],
    ["7", "- generatedAt (Plan)", "DateTime / DATETIME2(3)", "DEFAULT GETDATE()", "Thời điểm chạy thuật toán mô phỏng kế hoạch."],
    ["8", "- scheduleItemId (Schedule)", "UUID / UNIQUEIDENTIFIER", "PK", "Khóa chính dòng lịch trình thanh toán."],
    ["9", "- planId (Schedule)", "UUID / UNIQUEIDENTIFIER", "FK -> Plan", "Kế hoạch trả nợ cha (CASCADE DELETE)."],
    ["10", "- debtId (Schedule)", "UUID / UNIQUEIDENTIFIER", "FK -> DebtAccount", "Khoản nợ được thanh toán trong kỳ này."],
    ["11", "- monthIndex (Schedule)", "Integer / INT", "NOT NULL", "Kỳ thanh toán thứ mấy (Tháng 1, Tháng 2...)."],
    ["12", "- paymentDate (Schedule)", "LocalDate / DATE", "NOT NULL", "Ngày dự kiến thực hiện thanh toán kỳ."],
    ["13", "- principalPortion (Schedule)", "Money / DECIMAL(18, 2)", "NOT NULL", "Số tiền khấu trừ trực tiếp vào nợ gốc."],
    ["14", "- interestPortion (Schedule)", "Money / DECIMAL(18, 2)", "NOT NULL", "Số tiền trả lãi cho chủ nợ trong kỳ."],
    ["15", "- totalPaymentAmount (Schedule)", "Money / DECIMAL(18, 2)", "NOT NULL", "Tổng tiền chi trả trong kỳ (gốc + lãi)."],
    ["16", "- remainingBalanceAfterPayment", "Money / DECIMAL(18, 2)", "NOT NULL", "Dư nợ gốc còn lại sau kỳ thanh toán."],
    ["17", "- paymentTransactionId (Tx)", "UUID / UNIQUEIDENTIFIER", "PK", "Khóa chính giao dịch thanh toán nợ thực tế."],
    ["18", "- debtId (DebtTx)", "UUID / UNIQUEIDENTIFIER", "FK -> DebtAccount", "Khoản nợ được thanh toán giảm trừ."],
    ["19", "- walletId (DebtTx)", "UUID / UNIQUEIDENTIFIER", "FK -> Wallet", "Ví tài sản bị trừ tiền trả nợ."],
    ["20", "- paymentAmount (DebtTx)", "Money / DECIMAL(18, 2)", "NOT NULL", "Tổng số tiền thực tế đã chi trả."],
    ["21", "- principalDeducted (DebtTx)", "Money / DECIMAL(18, 2)", "NOT NULL", "Phần tiền trừ vào nợ gốc."],
    ["22", "- interestPaid (DebtTx)", "Money / DECIMAL(18, 2)", "NOT NULL", "Phần tiền trả lãi cho chủ nợ."],
    ["23", "- paymentDate (DebtTx)", "DateTime / DATETIME2(3)", "DEFAULT GETDATE()", "Thời điểm thực hiện giao dịch trả nợ."]
]

# CONTEXT 6
NUDGE_ATTRS = [
    ["1", "- nudgeId", "UUID / UNIQUEIDENTIFIER", "PK", "Khóa chính thông điệp cú hích hành vi."],
    ["2", "- userId", "UUID / UNIQUEIDENTIFIER", "FK -> User", "Người dùng nhận thông điệp can thiệp tâm lý."],
    ["3", "- nudgeType", "NudgeType / TINYINT", "ENUM(1..4)", "Phân loại cú hích (OVERSPEND, ZOMBIE, REBALANCE, COOLING_OFF)."],
    ["4", "- title", "String / NVARCHAR(150)", "NOT NULL", "Tiêu đề ngắn gọn của cú hích (Cảnh báo vỡ DSA)."],
    ["5", "- message", "String / NVARCHAR(MAX)", "NOT NULL", "Nội dung thông điệp cú hích tâm lý tích cực."],
    ["6", "- rationaleExplanation", "String / NVARCHAR(MAX)", "NOT NULL", "Giải trình thuật toán minh bạch XAI (căn cứ số liệu)."],
    ["7", "- triggerEvent", "String / NVARCHAR(100)", "NOT NULL", "Sự kiện kích hoạt (VƯỢT_NGƯỠNG_DSA_80, THUÊ_BAO_ZOMBIE)."],
    ["8", "- isApprovedByUser", "Boolean / BIT", "DEFAULT 0", "Xác nhận của người dùng phê duyệt đề xuất (Human-in-the-loop)."],
    ["9", "- coolingOffUntil", "DateTime / DATETIME2(3)", "NULL", "Mốc thời gian kết thúc khoảng trễ tâm lý hạ nhiệt (24h-48h)."],
    ["10", "- status", "NudgeStatus / TINYINT", "ENUM(1..4)", "Trạng thái cú hích (PENDING, ACCEPTED, DISMISSED, EXPIRED)."]
]
