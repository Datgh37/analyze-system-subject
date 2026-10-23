# KẾ HOẠCH PLAN-44: TRIỂN KHAI GIAI ĐOẠN 8 — PHÂN HỆ 9: CÚ HÍCH HÀNH VI THALER NUDGE, KHOẢNG TRỄ 24H & TRỢ LÝ XAI (UC-09.01 $\rightarrow$ UC-09.05)

> **Dự án:** Hệ thống Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh (Tích hợp AI)  
> **Giai đoạn:** Giai đoạn 8 / 12 Giai đoạn chuẩn hóa (Lộ trình [`PLAN-42`](2026-10-23_plan_42_dieu_chinh_roadmap_tach_phan_he_ai_va_giai_doan_kiem_dinh_ai.md) & [Quyết định 122, 125](../../decisions_and_changes_log.md))  
> **Căn cứ nghiệp vụ:** [`09_dac_ta_uc_cu_hich_thaler_nudge.md`](../../reports/use_cases/09_dac_ta_uc_cu_hich_thaler_nudge.md), Tab 9 bản vẽ `files/UCD-DuoNoPro.drawio`, Bounded Context 6 `files/ClassDiagram.drawio`  
> **Chuyên gia bảo trợ:** Agent 01 Richard Thaler (Nobel Kinh tế), Agent 08 Andrew Ng (Explainable AI Lead), Agent 03 Cockburn BA, Agent 04 Eric Evans (DDD Lead), Agent 10 Dan Abramov (Clean MVVM), Agent 12 Kent Beck (TDD Quality Gate)  
> **Phương pháp triển khai:** Tách đôi thành 2 chặng: **Chặng 8A (Backend TDD First)** và **Chặng 8B (Frontend Wireframe MAUI & MVVM Repositories)**

---

## 1. MỤC TIÊU & PHẠM VI 5 USE CASES CHUẨN IEEE 29148

| Mã UC | Tên Use Case | Bản Chất Thuật Toán & Ràng Buộc Nghiệp Vụ | Dữ Liệu Đầu Vào & Đầu Ra |
| :--- | :--- | :--- | :--- |
| **`UC-09.01`** | **Tiếp nhận Cú hích tài chính thời gian thực** | • Ứng dụng lý thuyết Cú hích (Nudge Theory của GS. Richard Thaler - Nobel 2017).<br>• Tự động kích hoạt 3 quy tắc hành vi vàng (Just-In-Time Nudge):<br>&nbsp;&nbsp;1. *Chạm trần ngày DSA:* Chi tiêu vượt quá $85\%$ hạn mức an toàn $\text{DSA}_t$.<br>&nbsp;&nbsp;2. *Bẫy thuê bao ngủ quên (Zombie Subscriptions):* Dịch vụ định kỳ trừ tiền nhưng không có tương tác trong 60 ngày.<br>&nbsp;&nbsp;3. *Cú hích trả cho mình trước (Pay Yourself First):* Khi nhận lương, gợi ý trích ngay $20\%$ vào Quỹ tích lũy.<br>• Thuật toán chống làm phiền (Nudge Fatigue Prevention): Chặn phát cùng loại trong 4 giờ. | **Input:** Sự kiện giao dịch chi tiêu/thu nhập, dữ liệu DSA, danh sách thuê bao.<br>**Output:** Bản ghi `FinancialNudge` (`Active`), thông điệp tích cực khích lệ, nhãn thẻ trực quan. |
| **`UC-09.02`** | **Xem căn cứ & Lý do giải trình của thuật toán AI (XAI)** | • Trí tuệ nhân tạo có thể giải thích được (Explainable AI - XAI), xóa bỏ hoàn toàn định kiến "AI Hộp Đen" (Black Box).<br>• Cung cấp đầy đủ 3 thành phần giải trình minh bạch:<br>&nbsp;&nbsp;1. *Hiện trạng kích hoạt:* Sự kiện thực tế vừa phát sinh.<br>&nbsp;&nbsp;2. *Bằng chứng số liệu & Công thức toán học:* Công thức áp dụng ($\text{DSA}$, $V$, tỷ lệ thâm hụt) và bảng dữ liệu lịch sử.<br>&nbsp;&nbsp;3. *Mô phỏng hậu quả nếu không điều chỉnh:* Phân tích định lượng tác động ngày mai nếu tiếp tục hành vi cũ. | **Input:** `nudgeId`.<br>**Output:** Cấu trúc giải trình `RationaleExplanation` 3 thành phần, bảng số liệu chứng minh, phản hồi Thumbs Up/Down. |
| **`UC-09.03`** | **Đề xuất Tái cân đối ngân sách động** | • Thuật toán tối ưu hóa phân bổ tài chính (Dynamic Rebalancing Solver).<br>• Khi danh mục Thiết yếu (`Needs`) thâm hụt một khoản $\Delta_{\text{deficit}}$, thuật toán quét tìm các danh mục Tùy chọn (`Wants`) đang dôi dư ngân sách $\text{Remaining} > 0$ có độ ưu tiên thấp nhất để đề xuất bù trừ.<br>• **Bảo toàn phương trình ngân sách (Budget Invariant):** Tổng hạn mức sau điều chỉnh bằng 100% ban đầu ($\sum \text{Allocated}' = \sum \text{Allocated}$). | **Input:** Kế hoạch ngân sách active, danh mục thâm hụt $\Delta_{\text{deficit}}$.<br>**Output:** Phiếu đề xuất điều chuyển (Transfer Slip) gồm danh mục nguồn, danh mục đích, số tiền bù trừ. |
| **`UC-09.04`** | **Xác nhận / Phê duyệt điều chỉnh (Human-in-the-loop)** | • **Nguyên tắc tối cao đạo đức AI:** Con người nắm quyền kiểm soát tối hậu (Human-in-the-loop); AI tuyệt đối không tự ý can thiệp CSDL thật nếu người dùng chưa bấm nút phê duyệt (No Silent Auto-Apply).<br>• Khi Phê duyệt: Mở Database Transaction nguyên tử (ACID), cập nhật hạn mức danh mục nguồn và đích trong `BudgetAllocations`, gán `status = Resolved`, kích hoạt tái tính toán lại DSA.<br>• Khi Bỏ qua: Gán `status = Dismissed`, giữ nguyên hiện trạng ngân sách 100%. | **Input:** `nudgeId`, quyết định (`Approve` / `Dismiss`).<br>**Output:** Cập nhật CSDL nguyên tử ACID, thông báo kết quả. |
| **`UC-09.05`** | **Đánh giá tác động mua sắm lớn & Kích hoạt Khoảng trễ tâm lý (Cooling-Off 24h)** | • Kiến tạo ma sát có lợi (Friction by Design): Chuyển đổi trạng thái tâm lý từ bốc đồng cảm xúc (Hệ thống tư duy 1) sang lý trí điềm tĩnh (Hệ thống tư duy 2).<br>• Điều kiện kích hoạt: Khoản chi thuộc danh mục Tùy chọn (`Wants`) có giá trị $\ge 3,000,000$ đ (hoặc $\ge 20\%$ thu nhập tháng).<br>• Tạm lưu giao dịch `PendingCoolingOff` (chưa trừ tiền ví) và khóa đồng hồ đếm ngược 24h.<br>• Bảng phân tích tác động tài chính sâu sắc:<br>&nbsp;&nbsp;1. Mức độ siết giảm hạn mức ngày $\text{DSA}$.<br>&nbsp;&nbsp;2. **Quy đổi sang Giờ công lao động:** Số tiền $\div$ Lương theo giờ ($M_{\text{income}} / 160\text{h}$).<br>&nbsp;&nbsp;3. Tác động làm chậm tiến độ Mục tiêu tích lũy.<br>• Hết 24h: Người dùng bấm "Xác nhận mua" (trừ tiền) hoặc "Hủy bỏ" (tiết kiệm thành công). Hỗ trợ mở khóa khẩn cấp có giải trình lý do (Bypass). | **Input:** Khoản chi tiêu tùy chọn lớn, mức thu nhập.<br>**Output:** Bản ghi `CoolingOffTransaction`, mốc khóa $T_{\text{cooldown}} = \text{Now} + 24\text{h}$, số giờ công quy đổi, bảng tác động DSA. |

---

## 2. KIẾN TRÚC KỸ THUẬT & DANH MỤC THÀNH PHẦN XÂY DỰNG

```
mobile-project/dotnet-maui-ver/
├── backend/
│   ├── ExpenseTracker.Domain/
│   │   └── Entities/
│   │       ├── FinancialNudge.cs                   # Aggregate Root: Cú hích tài chính Thaler, giải trình XAI, trạng thái duyệt
│   │       └── CoolingOffTransaction.cs            # Aggregate Root: Giao dịch hạ nhiệt 24h-48h, quy đổi giờ công, trạng thái khóa
│   ├── ExpenseTracker.Infrastructure/
│   │   ├── Configurations/
│   │   │   ├── FinancialNudgeConfiguration.cs       # Cấu hình EF Core & Composite Index (UserId, Status)
│   │   │   └── CoolingOffTransactionConfiguration.cs# Cấu hình EF Core & Index (UserId, Status, CoolingOffUntil)
│   │   └── Migrations/
│   │       └── YYYYMMDD_AddBehavioralNudgesAndCoolingOff.cs
│   ├── ExpenseTracker.Application/
│   │   ├── DTOs/Nudges/
│   │   │   ├── NudgeDtos.cs                        # DTOs: Cú hích, XAI Rationale, Đề xuất tái cân đối, Phê duyệt
│   │   │   └── CoolingOffDtos.cs                   # DTOs: Đánh giá tác động mua sắm, Giờ công, Quản lý giao dịch hạ nhiệt
│   │   ├── Interfaces/
│   │   │   ├── INudgeEngineService.cs              # Hợp đồng Cú hích Thaler, XAI Rationale & Dynamic Rebalance
│   │   │   └── ICoolingOffService.cs               # Hợp đồng Quản lý Khoảng trễ tâm lý 24h & Quy đổi giờ công
│   │   └── Services/
│   │       ├── NudgeEngineService.cs               # Động cơ Cú hích, thuật toán tái cân đối bù trừ, chống spam 4h
│   │       └── CoolingOffService.cs                # Động cơ kiểm soát ma sát tâm lý, tính giờ công, countdown 24h
│   ├── ExpenseTracker.WebApi/
│   │   └── Controllers/
│   │       ├── NudgesController.cs                 # API: api/nudges (active, details, rebalance, approve, dismiss)
│   │       └── CoolingOffController.cs             # API: api/cooling-off (evaluate, pending, confirm, cancel, bypass)
│   └── tests/
│       ├── ExpenseTracker.Domain.UnitTests/
│       │   └── Entities/
│       │       ├── FinancialNudgeTests.cs          # Tests bất biến Nudge, XAI, Status transitions
│       │       └── CoolingOffTransactionTests.cs   # Tests tính giờ công, cooldown lock, bypass invariants
│       └── ExpenseTracker.Application.UnitTests/
│           └── Services/
│               ├── NudgeEngineServiceTests.cs      # Tests phát hiện Nudge, Dynamic Rebalance bảo toàn invariant, XAI
│               └── CoolingOffServiceTests.cs       # Tests kích hoạt 24h, tính tác động DSA, confirm trừ tiền, cancel
│
└── frontend/
    ├── ExpenseTracker.Maui/
    │   ├── Models/Nudges/
    │   │   ├── NudgeModels.cs                      # Client models: Nudge, XAI Rationale, Rebalance Proposal
    │   │   └── CoolingOffModels.cs                 # Client models: CoolingOff item, Financial impact, Work hours
    │   ├── Services/Api/
    │   │   └── IApiClients.cs                      # Thêm Refit contracts: INudgeApi & ICoolingOffApi
    │   ├── Repositories/Nudges/
    │   │   ├── INudgeRepository.cs                 # Repository interface Cú hích & Tái cân đối
    │   │   ├── ApiNudgeRepository.cs               # WebApi client thật
    │   │   ├── MockNudgeRepository.cs              # Mock data phong phú chạy offline (DsaNudge, ZombieSub, Rebalance)
    │   │   ├── ICoolingOffRepository.cs            # Repository interface Khoảng trễ tâm lý
    │   │   ├── ApiCoolingOffRepository.cs          # WebApi client thật
    │   │   └── MockCoolingOffRepository.cs         # Mock data phong phú (Smartwatch 5M, 24h countdown, 41.6h công)
    │   ├── ViewModels/Nudges/
    │   │   ├── NudgesViewModel.cs                  # ViewModel quản lý Nudges, XAI Rationale Modal, Phê duyệt
    │   │   └── CoolingOffViewModel.cs              # ViewModel quản lý danh sách hạ nhiệt, đếm ngược, hủy/mua
    │   └── Views/Nudges/
    │       ├── NudgesPage.xaml & .cs               # Wireframe View hiển thị thẻ Cú hích, XAI Rationale, Rebalance Slip
    │       └── CoolingOffPage.xaml & .cs           # Wireframe View danh sách hạ nhiệt, đồng hồ countdown, thẻ giờ công
    └── tests/
        └── ExpenseTracker.Maui.UnitTests/
            ├── NudgesViewModelTests.cs             # Tests load nudges, xem XAI, duyệt đề xuất tái cân đối
            └── CoolingOffViewModelTests.cs         # Tests nạp danh sách hạ nhiệt, tick countdown, xác nhận/hủy mua
```

---

## 3. DANH MỤC TDD UNIT TESTS DỰ KIẾN (NÂNG TỪ 203 LÊN $\ge 223$ TESTS)

| STT | Tập Tin Test | Tên Test Case | Kịch Bản & Khẳng Định Kiểm Thử |
| :---: | :--- | :--- | :--- |
| 1 | `FinancialNudgeTests.cs` | `CreateNudge_WithValidParameters_ShouldInitializeCorrectly` | Khởi tạo Nudge với trạng thái Active, đầy đủ metadata và thông điệp. |
| 2 | `FinancialNudgeTests.cs` | `ApproveNudge_WhenActive_ShouldTransitionToResolved` | Phê duyệt Nudge chuyển sang trạng thái Resolved, ghi nhận IsApprovedByUser. |
| 3 | `CoolingOffTransactionTests.cs` | `CalculateEquivalentWorkHours_ShouldComputeAccurately` | Quy đổi số tiền mua sắm thành số giờ lao động thực tế dựa trên lương 160h/tháng. |
| 4 | `CoolingOffTransactionTests.cs` | `ConfirmPurchase_BeforeCooldownExpires_WithoutBypass_ShouldThrowException` | Chặn xác nhận mua hàng khi đồng hồ 24h chưa kết thúc và không có lý do khẩn cấp. |
| 5 | `NudgeEngineServiceTests.cs` | `EvaluateDsaThreshold_WhenSpentExceeds85Percent_ShouldGenerateDsaNudge` | Tự động sinh Cú hích cảnh báo DSA khi chi tiêu chạm ngưỡng $85\%$. |
| 6 | `NudgeEngineServiceTests.cs` | `EvaluateSpamProtection_WhenRecentNudgeExistsWithin4Hours_ShouldSuppressNudge` | Cơ chế chống làm phiền chặn phát Cú hích cùng loại trong vòng 4 giờ. |
| 7 | `NudgeEngineServiceTests.cs` | `ProposeDynamicRebalance_ShouldPreserveTotalBudgetAllocationInvariant` | Thuật toán tái cân đối cắt giảm danh mục Wants bù sang Needs, bảo toàn $100\%$ tổng hạn mức. |
| 8 | `NudgeEngineServiceTests.cs` | `ApproveRebalanceProposal_ShouldUpdateAllocationsInAtomicTransaction` | Phê duyệt tái cân đối cập nhật đồng thời 2 danh mục và tái tính toán DSA. |
| 9 | `CoolingOffServiceTests.cs` | `EvaluateCoolingOffTrigger_WithLargeOptionalExpense_ShouldActivate24HourCooldown` | Khoản chi $\ge 3,000,000$ đ kích hoạt đồng hồ đếm ngược 24h và lưu trạng thái Pending. |
| 10 | `CoolingOffServiceTests.cs` | `CancelCoolingOffTransaction_ShouldCongratulateAndRetainFunds` | Hủy bỏ giao dịch sau thời gian hạ nhiệt, hiển thị lời chúc mừng tiết kiệm tiền. |
| 11 | `NudgesViewModelTests.cs` | `LoadNudges_WhenSuccessful_ShouldPopulateActiveNudgesAndRationale` | ViewModel nạp danh sách Cú hích và nạp chi tiết giải trình XAI 3 thành phần. |
| 12 | `NudgesViewModelTests.cs` | `ApproveRebalanceCommand_ShouldExecuteSuccessfullyAndRefresh` | Kích hoạt lệnh phê duyệt điều chuyển hạn mức và làm mới giao diện. |
| 13 | `CoolingOffViewModelTests.cs` | `LoadCoolingOffItems_ShouldPopulateCardsAndComputeCountdown` | ViewModel nạp các khoản mua sắm đang hạ nhiệt kèm bảng phân tích giờ công. |
| 14 | `CoolingOffViewModelTests.cs` | `CancelPurchaseCommand_ShouldRemoveItemAndShowSuccessMessage` | Hủy mua hàng thành công và cập nhật số tiền tiết kiệm được. |

---

## 4. TIÊU CHÍ NGHIỆM THU (QUALITY GATES)
1. **Chặng 8A (Backend):**
   - 100% biên dịch sạch, 0 Error, 0 Warning trên `ExpenseTracker.slnx`.
   - Sinh migration EF Core cho `FinancialNudge` và `CoolingOffTransaction`.
   - Bổ sung 10+ Unit Tests backend, nâng tổng số test lên $\ge 213$ tests passed 100%.
2. **Chặng 8B (Frontend Wireframe):**
   - 100% biên dịch sạch, 0 Error, 0 Warning trên `ExpenseTracker.Maui.csproj`.
   - Bộ đôi Repositories Mock/Real cho cả Nudges và CoolingOff.
   - Bổ sung 4+ Unit Tests mobile ViewModel, nâng tổng số test lên $\ge 217$ - 223 tests passed 100%.
3. **Kiểm toán hậu giai đoạn:**
   - Tổ chức Phiên Kiểm Toán Hệ Thống Hậu Giai Đoạn 8 (Post-Phase 8 Audit Protocol) theo Quyết định 116 đối soát 5 Use Cases IEEE 29148.

---

## 5. THỰC THI CHẶNG 8A (BƯỚC ĐẦU TIÊN)
Sau khi Người dùng phê duyệt kế hoạch này:
- Bắt tay thực hiện **CHỈ Chặng 8A (Backend Phân hệ 9)**.
- Báo cáo kết quả Chặng 8A để Người dùng kiểm tra sơ bộ trước khi bước sang Chặng 8B.
