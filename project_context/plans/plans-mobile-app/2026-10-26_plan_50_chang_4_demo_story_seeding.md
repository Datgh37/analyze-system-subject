# KẾ HOẠCH TRIỂN KHAI CHẶNG 4 (PLAN-50)
## MASTER DEMO STORY SEEDING — XÂY DỰNG BỘ DỮ LIỆU KỂ CHUYỆN DEMO HOÀN HẢO

> **Kế hoạch thành phần:** Chặng 4 thuộc [PLAN-50: Điều Chỉnh, Tối Ưu Hóa & Khắc Phục Logic Backend Toàn Diện](2026-10-26_plan_50_dieu_chinh_toi_uu_va_khac_phuc_logic_backend_toan_dien.md).  
> **Người chủ trì:** Agent 02 (Personal Budget Coach) & Agent 01 (Richard Thaler).  
> **Người phối hợp:** Agent 05 (Martin Kleppmann - Universal Database) & Agent 12 (Kent Beck - QA/Testing).  
> **Kỷ luật dự án:** Giai đoạn 11 tiếp tục **ĐÓNG BĂNG**. Không viết mã nguồn nếu bản kế hoạch này chưa được Chủ tọa đề tài (Người dùng) xem xét và phê duyệt.  
> **Triết lý hiển thị:** Tuân thủ 100% nguyên tắc UX **"ĐÚNG VÀ ĐỦ"** — Không sử dụng thuật ngữ AI trừu tượng, dữ liệu chân thực với đời sống tài chính của người trẻ tại Việt Nam.

---

### I. MỤC TIÊU & TẦM NHÌN DEMO

Khi bước vào buổi báo cáo đồ án, quay video thuyết trình 5-8 phút hoặc trình diễn trực tiếp cho Giảng viên:
- **Nỗi đau hiện tại:** Một ứng dụng tài chính nếu dữ liệu rỗng (Empty States) hoặc chỉ có 1-2 giao dịch thô sơ thì các biểu đồ (PieChart, BarChart), hạn mức ngân sách, cảnh báo Cú hích, và tiến độ tiết kiệm sẽ không thể hiện được sức mạnh thực tế của hệ thống.
- **Giải pháp Chặng 4:** Xây dựng một kịch bản dữ liệu mẫu hoàn hảo được kích hoạt chỉ bằng 1 thao tác gọi API:
  $$\text{POST } \texttt{/api/system/seed-demo-story}$$
- **Kết quả đạt được:** Tài khoản `demo@expensetracker.vn` được làm mới hoàn toàn trong < 1 giây, chứa sẵn một "bức tranh tài chính sống động", có mâu thuẫn, có điểm chạm tâm lý, và sẵn sàng cho các tình huống biểu diễn trực quan:
  1. **Ngân sách Ăn uống ngấp nghé 85%:** Chỉ cần ghi thêm 1 ly cà phê 50.000đ là kích hoạt ngay Cảnh báo thời gian thực 0ms trước mắt Giảng viên!
  2. **Khoảng trễ tâm lý 24h (Cooling-Off):** Hiển thị một món đồ đắt tiền đang đếm ngược còn 18 giờ, minh chứng rõ nét lý luận kinh tế hành vi của Richard Thaler.
  3. **Hóa đơn siêu thị WinMart có bóc tách từng món:** Chứng minh năng lực phân tích chi tiết hóa đơn (Itemized Receipts).
  4. **Phiếu lương đầy đủ các khoản khấu trừ:** Minh chứng năng lực kiểm toán lương (Mathematical Audit).
  5. **Mục tiêu tiết kiệm đạt 48%:** Biểu đồ vòng tròn tiến độ rõ ràng, sẵn sàng cho thao tác nạp tiền tích lũy.

---

### II. KỊCH BẢN CHI TIẾT CÂU CHUYỆN TÀI CHÍNH (THE DEMO STORYLINE)

#### 1. Thông tin Tài khoản Demo
- **Email:** `demo@expensetracker.vn` (hằng số `DefaultDemoEmail`).
- **Mật khẩu:** `Demo@123` (hằng số `DefaultDemoPassword`).
- **Họ tên:** `Trịnh Thành Đạt` (người trẻ đi làm, thu nhập ổn định).
- **Tiền tệ gốc:** `VND`.

#### 2. Hệ sinh thái 3 Ví Tài chính Thực tế
| Tên Ví | Loại Ví | Số Tài Khoản / Ngân Hàng | Số Dư Ban Đầu | Màu Sắc & Icon | Ý Nghĩa Kịch Bản |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **Tài khoản Vietcombank** | `BankAccount` | `007100889988` (Vietcombank) | **15.500.000 đ** | `#0066FF` / `building-columns` | Ví thanh toán chính, nhận lương và chi tiêu thẻ. |
| **Ví Tiền Mặt** | `Cash` | Không | **1.200.000 đ** | `#10B981` / `cash` | Chi tiêu ăn sáng, gửi xe, chợ truyền thống. |
| **Quỹ Khẩn Cấp (Techcombank)** | `BankAccount` | `19033445566` (Techcombank) | **10.000.000 đ** | `#8B5CF6` / `shield` | Quỹ dự phòng tài chính bất khả xâm phạm. |

$$\text{Tổng tài sản khả dụng (Net Worth ban đầu): } 15.500.000 + 1.200.000 + 10.000.000 = \mathbf{26.700.000\text{ đ}}$$

#### 3. Ngân sách Tháng & Kịch bản Ngấp nghé Cảnh báo (85% Threshold)
Ngân sách tháng hiện tại theo phương pháp **50/30/20**:
- **Tổng hạn mức tháng:** `12.000.000 đ`.
- **Hạn mức theo từng danh mục then chốt:**
  * **Ăn uống & Thực phẩm (Needs):** Hạn mức `3.000.000 đ`. Đã chi **`2.550.000 đ` (đúng 85.0% - Cảnh báo màu cam)**.
    > *Kịch bản Demo:* Khi mở màn hình `Budget`, thanh tiến độ hiển thị màu cam cảnh báo ngấp nghé trần. Khi demo nhập một giao dịch cà phê `60.000 đ`, ngân sách tăng lên `2.610.000 đ` (87%), lập tức phát sóng WebSocket SignalR cảnh báo tức thì!
  * **Đi lại & Xăng xe (Needs):** Hạn mức `1.500.000 đ`. Đã chi **`650.000 đ` (43.3% - Vùng an toàn xanh)**.
  * **Nhà ở & Tiện ích (Needs):** Hạn mức `4.000.000 đ`. Đã chi **`3.500.000 đ` (87.5%)**.
  * **Mua sắm cá nhân (Wants):** Hạn mức `2.000.000 đ`. Đã chi **`1.200.000 đ` (60.0%)**.

#### 4. Khoản Chi Đắn Đo Tâm Lý Đang Đếm Ngược (Thaler 24h Cooling-Off)
- **Tên món đồ:** `"Tai nghe chống ồn Sony WH-1000XM5"` (Mua sắm công nghệ).
- **Số tiền:** `4.500.000 đ` (từ ví Vietcombank).
- **Thời gian khởi tạo:** Cách đây 6 giờ (`CreatedAt = UtcNow.AddHours(-6)`).
- **Thời hạn đắn đo (`CoolingOffUntil`):** 24 giờ sau khi tạo $\rightarrow$ **Hiện còn đúng 18 giờ đếm ngược**.
- **Chỉ số hành vi:**
  * Số giờ lao động quy đổi: `32.5 giờ` làm việc.
  * Mức độ giảm trừ chi tiêu an toàn hàng ngày (DSA): `-150.000 đ/ngày`.
- **Trạng thái:** `PendingCoolingOff` (chờ hết hạn hoặc người dùng bấm Hủy để chiến thắng cám dỗ).

#### 5. Mục tiêu Tiết kiệm (Savings Goals & Contributions)
- **Mục tiêu 1:** `"Mua Laptop Mới Làm Việc"`
  * Số tiền mục tiêu: `25.000.000 đ`.
  * Đã tích lũy: **`12.000.000 đ` (đạt 48.0%)**.
  * Hạn chót: 6 tháng tới.
  * Lịch sử tích lũy: 2 đợt đóng góp (`5.000.000 đ` tháng trước và `7.000.000 đ` tháng này).
- **Mục tiêu 2:** `"Quỹ Khẩn Cấp 3 Tháng"`
  * Số tiền mục tiêu: `20.000.000 đ`.
  * Đã tích lũy: **`10.000.000 đ` (đạt 50.0%)**.

#### 6. Giao dịch Có Hóa Đơn Bóc Tách (Itemized Receipt)
Giao dịch chi tiêu tại siêu thị WinMart:
- **Tổng tiền:** `385.000 đ` (thanh toán từ ví Vietcombank).
- **Danh mục:** Ăn uống & Thực phẩm.
- **Chi tiết 4 món hàng (`ReceiptItem`):**
  1. *Sữa tươi tiệt trùng Vinamilk 1L* — SL: 2, Đơn giá: 35.000đ $\rightarrow$ Thành tiền: 70.000đ.
  2. *Trứng gà tươi Ba Huân (hộp 10 quả)* — SL: 1, Đơn giá: 35.000đ $\rightarrow$ Thành tiền: 35.000đ.
  3. *Gạo lúa tôm ST25 Ông Cua túi 5kg* — SL: 1, Đơn giá: 180.000đ $\rightarrow$ Thành tiền: 180.000đ.
  4. *Nước mắm Nam Ngư Đệ Nhị 750ml* — SL: 1, Đơn giá: 100.000đ $\rightarrow$ Thành tiền: 100.000đ.

#### 7. Giao dịch Thu Nhập Có Kiểm Toán Phiếu Lương (Payslip Audit)
Giao dịch nhận lương tháng chuyển khoản:
- **Tổng tiền thực nhận (Net Salary):** `22.690.000 đ`.
- **Chi tiết kiểm toán phiếu lương (`SalarySlipDetail`):**
  * Tên công ty: `Công ty Cổ phần Công nghệ FPT`.
  * Kỳ lương: `Tháng 10/2026`.
  * Lương cơ bản (Gross): `22.000.000 đ`.
  * Thưởng hiệu suất dự án: `3.000.000 đ`.
  * Khấu trừ bảo hiểm xã hội (BHXH 8%): `1.760.000 đ`.
  * Khấu trừ bảo hiểm y tế (BHYT 1.5%): `330.000 đ`.
  * Khấu trừ bảo hiểm thất nghiệp (BHTN 1%): `220.000 đ`.
  * Kiểm toán toán học (`IsMathAudited`): `true`, Chênh lệch (`DiscrepancyAmount`): `0 đ`.

#### 8. Lịch sử Biến động Giá trị Ròng (Net Worth Trend Snapshots)
Tạo 3 mốc dữ liệu để biểu đồ Net Worth hiển thị đường cong tăng trưởng đều đặn:
- **Tháng N-2:** Tổng tài sản `23.500.000 đ`, Nợ `0 đ` $\rightarrow$ Net Worth `23.500.000 đ`.
- **Tháng N-1:** Tổng tài sản `25.200.000 đ`, Nợ `0 đ` $\rightarrow$ Net Worth `25.200.000 đ`.
- **Tháng hiện tại:** Tổng tài sản `26.700.000 đ`, Nợ `0 đ` $\rightarrow$ Net Worth `26.700.000 đ`.

#### 9. Hòm thư Thông báo & Cú hích Hành vi (In-App Notifications & Nudges)
- **Cú hích 1 (Nudge):** Cảnh báo ngân sách `"Ăn uống & Thực phẩm đã đạt 85% hạn mức tháng. Hãy cân nhắc nấu ăn tại nhà trong tuần này."`.
- **Cú hích 2 (Nudge):** Cú hích Pay Yourself First `"Bạn vừa nhận lương! Hãy trích ngay 10% (2.200.000đ) vào mục tiêu Mua Laptop Mới."`.
- **Thông báo khay hệ thống (3 InAppNotifications):**
  1. *[BudgetAlert]* `Cảnh báo: Ngân sách Ăn uống tháng này đã đạt mức 85%.`
  2. *[AiReceiptProcessed]* `Đã quét thành công hóa đơn WinMart: 4 món hàng (385.000 đ).`
  3. *[System]* `Chào mừng bạn đến với Hệ thống Quản lý Chi tiêu Thông minh!`

---

### III. KẾ HOẠCH THIẾT KẾ & TRIỂN KHAI MÃ NGUỒN

#### 1. Nâng cấp `DatabaseSeeder.cs`
- Thêm hằng số định danh tài khoản demo:
  ```csharp
  public const string DefaultDemoEmail = "demo@expensetracker.vn";
  public const string DefaultDemoPassword = "Demo@123";
  ```
- Xây dựng phương thức dọn dẹp và nạp mới nguyên tử (Atomic Clean & Re-seed):
  ```csharp
  public static async Task<User> SeedDemoStoryAsync(
      ApplicationDbContext context, 
      IPasswordHasher passwordHasher, 
      ILogger logger, 
      CancellationToken ct = default)
  ```
- **Quy trình làm sạch an toàn:**
  * Tìm `User` có email `DefaultDemoEmail`.
  * Nếu đã tồn tại: Xóa toàn bộ dữ liệu phụ thuộc (Transactions, ReceiptItems, SalarySlipDetails, Wallets, Budgets, SavingsGoals, CoolingOffTransactions, Notifications, Nudges, NetWorthSnapshots) và xóa User.
  * Tạo mới `User` demo cùng toàn bộ hệ sinh thái dữ liệu 9 phân hệ nêu ở Mục II.
  * `SaveChangesAsync(ct)` thành công trong < 1.5 giây.

#### 2. Nâng cấp `SystemController.cs`
- Thêm endpoint HTTP:
  ```csharp
  [HttpPost("seed-demo-story")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<IActionResult> SeedDemoStory(CancellationToken ct)
  ```
- Payload phản hồi: Trả về trạng thái `Success: true`, thông tin đăng nhập (`demo@expensetracker.vn` / `Demo@123`), số dư tổng quan 3 ví, các mốc ngân sách ngấp nghé, và tóm tắt kịch bản demo.

#### 3. Viết Unit Test Kiểm Thử Tự Động
- Cập nhật / mở rộng [`DatabaseSeederTests.cs`](../../mobile-project/dotnet-maui-ver/backend/tests/ExpenseTracker.Application.UnitTests/Services/DatabaseSeederTests.cs):
  * Test 1: `SeedDemoStoryAsync_WhenCalled_CreatesUserAndAllDemoEntitiesWithValidData`.
  * Test 2: `SeedDemoStoryAsync_WhenCalledMultipleTimes_CleansOldDataAndReseedsIdempotently`.
  * Kiểm tra tính toán học: Tổng số dư ví, tỷ lệ phần trăm ngân sách ăn uống (đúng 85%), số giờ làm việc quy đổi của Cooling-Off, và tổng tiền bóc tách hóa đơn khớp 100%.

---

### IV. BAREM NGHIỆM THU (QUALITY GATES)

- [ ] **Verification Gate 1:** Chạy `dotnet test backend/ExpenseTracker.slnx`, toàn bộ test suite (dự kiến 266+ tests) phải đạt **100% PASSED**, 0 failed.
- [ ] **Verification Gate 2:** Chạy `dotnet build X:\frontend\ExpenseTracker.Maui\ExpenseTracker.Maui.csproj -f net10.0-android -c Debug -nr:false`, đạt **0 Warning, 0 Error**.
- [ ] **Verification Gate 3:** Gọi thử endpoint `POST /api/system/seed-demo-story`, kết quả trả về `HTTP 200 OK` với đầy đủ thông tin tài khoản demo.
- [ ] **Verification Gate 4:** Ghi nhận Quyết Định 157 vào `decisions_and_changes_log.md` và cập nhật bản đồ tiến độ `PLAN-50`.
