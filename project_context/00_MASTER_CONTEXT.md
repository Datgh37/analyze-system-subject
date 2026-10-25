# 🧭 MASTER CONTEXT: HỆ THỐNG PHÂN TÍCH THIẾT KẾ & AI SOFTWARE ENGINEER
## (QUẢN LÝ CHI TIÊU & HOẠCH ĐỊNH TÀI CHÍNH CÁ NHÂN THÔNG MINH - PFM)

> **Tài liệu Định hướng Ngữ cảnh Toàn cục (Universal Context Anchor)**  
> *Dành cho mọi Môi trường AI / IDE (Antigravity IDE, Claude Code, Cursor, Windsurf, ChatGPT, Copilot...)*  
> *Nếu cuộc trò chuyện bị mất trí nhớ (Context Loss) hoặc bạn mở project trên một máy/IDE mới, hãy nạp tệp này đầu tiên.*

---

## 1. Thông Tin Tổng Quan Đề Tài

* **Tên đề tài tiếng Việt:** Nghiên cứu, thiết kế và xây dựng **Hệ thống Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh (Tích hợp AI)**
* **Tên đề tài tiếng Anh:** **Smart Personal Expense and Financial Planning Management System with AI Integration**
* **Mục đích:** Phục vụ môn học **Phân tích Thiết kế Hệ thống Thông tin (PTTKHTTT)** và làm **Khóa luận Tốt nghiệp Cử nhân CNTT/HTTT**.
* **Định hướng cốt lõi (Core Pivot - PLAN-09):** Hệ thống tập trung **100% vào Quản lý Tài chính Cá nhân (Personal Finance Management - PFM)**, loại bỏ hoàn toàn việc chia tiền nhóm, quỹ chung và bạn cùng phòng để giải quyết sâu sắc các bài toán kỷ luật dòng tiền, tâm lý hành vi và tối ưu hóa tài chính cá nhân.
* **Ngăn xếp công nghệ chuẩn hóa (Production Tech Stack):** **.NET 10 (C# 14)** toàn diện từ Backend ASP.NET Core 10 Clean Architecture đến Frontend .NET MAUI 10 (Android / Desktop), CSDL PostgreSQL 16 ACID, Cổng Realtime SignalR Hub, Google Gemini 2.5 Flash Multimodal Vision & NLP.
* **Phương pháp luận phát triển:** **BMAD** (Agile AI-driven) & **Production Claude Agent Principles** (Bottom-up, Tool-first, Sandboxed, Traceable, Inspect before Act).

---

## 2. Hai Trụ Cột Kỹ Thuật Chính Của Dự Án

```text
┌────────────────────────────────────────────────────────────────────────┐
│ TRỤ CỘT 1: BÀI TOÁN NGHIỆP VỤ TÀI CHÍNH CÁ NHÂN (Personal Finance)     │
│ - Hoạch định Ngân sách Đa phương pháp (50/30/20, Zero-Based, Kakeibo). │
│ - Quản lý Đa Ví (Tiền mặt, Ngân hàng, Thẻ tín dụng, Ví MoMo) chuẩn ACID│
│ - Tối ưu hóa Chiến lược Trả nợ Cá nhân: Debt Snowball vs Avalanche.    │
│ - Cảnh báo Tốc độ Tiêu tiền (Spending Velocity) & Ngày cạn kiệt tiền.  │
│ - Bóc tách Hóa đơn siêu thị đa món bằng AI Vision OCR & Hậu kiểm toán. │
│ - Quét và cảnh báo Bẫy Đăng ký định kỳ (Subscription Traps Leakage).   │
│ - Chuẩn hóa CSDL quan hệ 3NF/BCNF, Caching và Khóa tranh chấp ví.      │
├────────────────────────────────────────────────────────────────────────┤
│ TRỤ CỘT 2: BỘ NHÂN AI SOFTWARE ENGINEER & HỆ SINH THÁI ECC UNIVERSAL   │
│ - Hội Đồng 14 Chuyên Gia Kiệt Xuất Full-Stack hóa thân các danh nhân.  │
│ - Bộ Ba Tam Giác Vàng Frontend (Agent 09 Visual, 13 Motion, 10 State). │
│ - Hệ Sinh Thái Kỹ Nghệ Toàn Diện Everything Claude Code (ECC Universal):│
│   * 18 Workflows chuẩn hóa quy trình (`.agents/workflows/`).          │
│   * 19 Custom Subagents chuyên sâu (`.agents/agents/`).                │
│   * 55 Kỹ năng (15 Council Skills + 40 ECC Specialized Skills).        │
│   * 21 Rules (5 Rules Học thuật KLTN DNTU + 16 Rules Lập trình `ecc-*`).│
│ - Hỗ trợ toàn diện 2 Tech Stacks chiến lược: Node.js/TypeScript & .NET │
│ - Tool Layer an toàn (Filesystem Sandbox, Terminal Runner, Git Diff).  │
│ - Chu trình làm việc chuẩn PTTKHTTT khép kín từ Khảo sát đến Final RFC.│
│ - Tự động sinh Ma Trận Truy Vết Nguồn Gốc (Traceability Matrix).       │
│ - Hệ thống thẩm định chất lượng sâu (Deep Content Quality Gates).      │
└────────────────────────────────────────────────────────────────────────┘
```

---

## 3. Bản Đồ Roster 14 Chuyên Gia Danh Nhân (Full-Stack Council Roster)

| Mã Agent | Tên File Quy Định | Chuyên Gia Hóa Thân | Vai Trò Chuyên Môn | Đầu Ra Chịu Trách Nhiệm |
| :--- | :--- | :--- | :--- | :--- |
| **Agent 00** | [`core/00_orchestrator.md`](core/00_orchestrator.md) | **Council Orchestrator** | Chủ tịch Hội đồng & Tổng duyệt 4 Pha | `04_final_rfc.md` |
| **Agent 01** | [`core/01_richard_thaler.md`](core/01_richard_thaler.md) | **Richard Thaler (Nobel)** | Kinh tế Hành vi, Nudge & Mental Accounting | `01_user_feedback.md` |
| **Agent 02** | [`core/02_personal_budget_coach.md`](core/02_personal_budget_coach.md) | **Personal Budget Coach** | Hoạch định Ngân sách (50/30/20, ZBB, Kakeibo) | `01_user_feedback.md` |
| **Agent 03** | [`core/03_cockburn_ba.md`](core/03_cockburn_ba.md) | **Alistair Cockburn / Debra Paul** | Phân tích Nghiệp vụ & Use Case (IEEE 29148) | `02_use_case_specs.md` |
| **Agent 04** | [`core/04_eric_evans.md`](core/04_eric_evans.md) | **Eric Evans (Cha đẻ DDD)** | Domain-Driven Design, Bounded Contexts & Money | `03_technical_adr.md` |
| **Agent 05** | [`core/05_martin_kleppmann.md`](core/05_martin_kleppmann.md) | **Martin Kleppmann & Sadalage** | Thiết kế CSDL 3NF, Multi-Wallet ACID & Indexing | `03_technical_adr.md` |
| **Agent 06** | [`core/06_personal_debt_cashflow_optimizer.md`](core/06_personal_debt_cashflow_optimizer.md) | **Debt & Cashflow Optimizer** | Thuật toán Trả nợ (Snowball vs Avalanche) | `03_technical_adr.md` |
| **Agent 07** | [`core/07_werner_vogels.md`](core/07_werner_vogels.md) | **Dr. Werner Vogels (Amazon)** | Kiến trúc Hướng Sự kiện, Async Queue OCR & Worker| `03_technical_adr.md` |
| **Agent 08** | [`core/08_andrew_ng.md`](core/08_andrew_ng.md) | **Prof. Andrew Ng (AI Pioneer)** | Vision LLM OCR Hóa Đơn & Hậu Kiểm Số Học | `03_technical_adr.md` |
| **Agent 09** | [`core/09_brad_frost.md`](core/09_brad_frost.md) | **Brad Frost, Norman & Jony Ive**| Lead Design System & Visual Art Director | `02b_frontend_ui_spec.md` |
| **Agent 13** | [`core/13_julian_garnier.md`](core/13_julian_garnier.md) | **Julian Garnier & Bret Victor** | Lead Motion Choreographer & Experience | `02b_frontend_ui_spec.md` |
| **Agent 10** | [`core/10_dan_abramov.md`](core/10_dan_abramov.md) | **Dan Abramov & Vjeux** | Lead Frontend Architect & Client State | `02b_frontend_ui_spec.md` & `03_technical_adr.md` |
| **Agent 11** | [`core/11_bruce_schneier.md`](core/11_bruce_schneier.md) | **Bruce Schneier & OWASP** | Bảo mật Dữ liệu Tài chính, Mã hóa AES-256 | `03_technical_adr.md` |
| **Agent 12** | [`core/12_kent_beck.md`](core/12_kent_beck.md) | **Kent Beck & Martin Fowler** | Kim Tự Tháp Kiểm Thử TDD & Verification Plan | `04_final_rfc.md` |

---

## 4. Bản Đồ Thư Mục & Vai Trò (System Directory Map)

| Thư mục / Tệp | Vai trò cốt lõi |
| :--- | :--- |
| [`README.md`](README.md) | Cẩm nang tổng quan dự án, kiến trúc hệ thống và hướng dẫn khởi chạy. |
| [`AGENTS.md`](AGENTS.md) | Luật chơi chung & quy ước điều phối Hội đồng Đa tác tử & ECC Roster. |
| [`run_council_session.py`](run_council_session.py) | CLI Runner chính thức điều phối phiên bàn thảo 4 pha. |
| [`.agents/workflows/`](.agents/workflows/) | 18 Workflows thực thi quy trình nhanh (`/plan`, `/quality-gate`, `/code-review`...). |
| [`.agents/agents/`](.agents/agents/) | 19 Custom Subagents chuyên sâu (`architect`, `csharp-reviewer`, `typescript-reviewer`...). |
| [`.agents/skills/`](.agents/skills/) | 55 Kỹ năng (15 Council Skills + 40 ECC Specialized Skills bao gồm .NET & Node/TS). |
| [`.agents/rules/`](.agents/rules/) | 5 Quy tắc học thuật KLTN DNTU + 16 Quy tắc lập trình ECC (`ecc-*`). |
| [`core/`](core/) | Đặc tả 14 Persona danh nhân chuẩn Production (Agent 00 đến Agent 13). |
| [`core_engine/`](core_engine/) | Bộ nhân Python Engine (Council, Skills, Tools Sandbox, Workflow, Memory). |
| [`docs/`](docs/) | Tài liệu đề tài gốc (`specs/`) và hướng dẫn tích hợp (`ANTIGRAVITY-ECC-SETUP-GUIDE.md`). |
| [`sessions/`](sessions/) | Các phiên bàn thảo 4 pha & 4 bộ Benchmark Sessions thực tế cá nhân (`session_01` đến `session_04`). |
| [`templates/`](templates/) | Bộ 5 mẫu tài liệu đầu ra chuẩn IEEE 29148, Frontend UI Spec, ADR, DB 3NF, Feedback. |
| [`tests/`](tests/) | Bộ Unit & Integration Tests tự động đạt chuẩn 100% Passed. |
| [`project_context/`](project_context/) | Nơi lưu trữ nhật ký quyết định, kế hoạch và snapshot ngữ cảnh bền vững. |

---

## 5. Trạng Thái Hoàn Thành Hiện Tại (Current Status)

- [x] **Tầng Công Cụ (Tool Layer):** Filesystem Sandbox, Terminal Runner (`python -B`), Git Diff.
- [x] **Hội Đồng 14 Chuyên Gia Danh Nhân:** Hoàn tất 100% 14 Persona chuẩn đề tài tài chính cá nhân.
- [x] **Bộ Ba Tam Giác Vàng Frontend (Pha 2.5):** Hoàn tất Agent 09 (Visual), Agent 13 (Motion), Agent 10 (State).
- [x] **Hệ Sinh Thái Kỹ Nghệ Toàn Diện ECC Universal (PLAN-21):** Tích hợp hoàn tất 18 Workflows, 19 Subagents, 40 Skills ECC (bao gồm .NET & Node/NestJS), 16 Programming Rules và phân quyền chuyên môn hóa cho 14 Agent Hội đồng.
- [x] **Tái Định Hướng Cá Nhân (PLAN-09):** Hoàn thành chuyển đổi Agent 02 (Personal Budget Coach) và Agent 06 (Debt & Cashflow Optimizer).
- [x] **Benchmark Sessions Chuẩn PFM:** Hoàn thành 4 phiên thực nghiệm tài chính cá nhân đạt 100% Quality Gates.
- [x] **Kiểm Thử Toàn Diện:** 100% Unit Tests đã pass không lỗi.
- [x] **Phân Hệ Mobile Giữa Kỳ (React Native Expo & Socket.IO):** Hoàn thành kiểm toán UI Refactor 9 màn hình, xuất bản Báo cáo Giữa kỳ [`mobile-project/react-native-ver/BAO_CAO_GIUA_KY.md`](../mobile-project/react-native-ver/BAO_CAO_GIUA_KY.md).
- [x] **Chiến Lược Doanh Nghiệp .NET MAUI & ASP.NET Core (PLAN-26):** Xác lập cơ chế phong tỏa phát triển (*Zero-Code Freeze*), quy định bắt buộc triệu tập Phiên họp Hội đồng Kickoff Governance thiết lập toàn diện tiêu chuẩn C# 14, MVVM, Clean Arch, Compiler & Linter Settings trước khi code.
- [x] **Giai Đoạn 0 Scaffolding Hạ Tầng (PLAN-31):** Khởi tạo Solution Clean Architecture backend và MAUI frontend, PostgreSQL 16 Docker Compose, Unit Test harness (100% Passed).
- [x] **Giai Đoạn 1 Phân Hệ 1 Tài Khoản & Đa Ví (PLAN-32 & PLAN-33):** Hoàn thành 100% Auth (JWT Refresh Token Rotation, BCrypt, Profile) và Đa Ví ACID (Pessimistic Locking `SELECT FOR UPDATE`, Balance Adjustments, Soft Delete, Timeline Audit). Toàn bộ 21/21 Unit Tests Passed.
- [x] **Tái Cấu Trúc Chiến Lược Backend-First & Wireframe Frontend (PLAN-35 / Quyết định 104):** Tách biệt 2 pha rõ ràng: Tập trung toàn lực hoàn thành 100% nghiệp vụ Backend trước; Frontend chỉ dựng khung Wireframe thô sơ kết nối Refit API. Toàn bộ thiết kế thẩm mỹ được dồn về một mối tại `Resources/Styles/` để đội ngũ Frontend chuyên biệt re-skin ở giai đoạn sau.
- [x] **Giai Đoạn 2 Phân Hệ Thu Nhập & Nguồn Thu Định Kỳ (PLAN-36 / Quyết Định 110):** Hoàn thành Thu nhập một lần, Thu nhập định kỳ (tự động cộng dồn ví khi đến hạn), và OCR Bảng lương đối soát công thức toán học (`78/78 Tests Passed`).
- [x] **Giai Đoạn 3 Phân Hệ Chi Tiêu, Danh Mục & AI OCR Hóa Đơn (PLAN-37 / Quyết Định 114 & 115):** Hoàn thành Sổ cái chi tiêu ACID, Động cơ quy tắc từ khóa thông minh UC-04.07, Multimodal Vision OCR bóc tách hóa đơn siêu thị, Hậu kiểm toán số học tự động, Quét bẫy thuê bao định kỳ, Tạo mã VietQR Napas 247 và Voice STT (`113/113 Tests Passed`).
- [x] **Giai Đoạn 4 Phân Hệ Ngân Sách Đa Phương Pháp & Động Cơ DSA Thích Ứng (PLAN-38 / Quyết Định 117):** Hoàn thành Hoạch định 50/30/20, Zero-Based Budgeting (ZBB), Sổ Kakeibo 4 Cột trụ, Động cơ Hạn mức chi tiêu mỗi ngày đàn hồi (Adaptive DSA), Giám sát tốc độ tiêu tiền (Spending Velocity), Dự phóng ngày cạn sạch tiền (Out-of-Money Date Forecast), Tái cân bằng hạn mức và Cảnh báo vi mô 80%/100% (`133/133 Tests Passed`, Frontend MAUI 0 Warning 0 Error).
- [x] **Giai Đoạn 5 Phân Hệ Quỹ Mục Tiêu Tích Lũy Thông Minh & Hũ Tài Chính Kakeibo (PLAN-39 / Quyết Định 118):** Hoàn thành Khởi tạo mục tiêu, Nạp/Rút quỹ tích lũy kèm Cú hích ma sát nhận thức Richard Thaler Nudge (bắt buộc nhập lý do), Đánh giá tính khả thi theo 50/30/20, Thống kê Hũ tài chính Kakeibo 4 nhóm hũ (`159/159 Tests Passed`, Frontend MAUI `SavingsGoalsPage` và `CreateSavingsGoalPage` biên dịch đạt 0 Warning 0 Error).
- [x] **Giai Đoạn 5B Chuẩn Hóa Barem Cuối Kỳ Lập Trình Mobile (PLAN-41 / Quyết Định 120):** Hoàn thành Kiến trúc MVVM 3 Tầng Repositories (Phần B3), Cổng Realtime SignalR Hub `FinancialHub` (Điểm cộng B2), Push Notifications & Local Scheduled Reminder 21:00 hàng ngày, màn hình `NotificationHistoryPage` Deep Linking (`104/104 Tests: 90 Backend + 14 Mobile ViewModel Tests Passed`, 0 Warning 0 Error).
- [x] **Giai Đoạn 6 Phân Hệ 7 Báo Cáo & Phân Tích Tài Chính Đa Chiều (PLAN-40 / Quyết Định 121):** Hoàn thành Động cơ phân tích CQRS Read Projections: Biến động Thu - Chi theo thời gian (MoM/YoY), Báo cáo Giá trị Tài sản ròng (Net Worth Snapshot), Báo cáo Dòng tiền ròng (Net Cashflow & 5 bước Thác nước Waterfall), Cơ cấu chi tiêu 50/30/20 & Kakeibo 4 Cột trụ, Phát hiện đột biến bằng kiểm định thống kê định lượng Z-Score Outlier ($\mu \pm 2\sigma$), Tầng MVVM Repositories (`IAnalyticsReportRepository` gồm Api & Mock Data offline) và Wireframe `ReportsPage.xaml` (`189/189 Tests Passed 100%`, 0 Warning 0 Error).
- [x] **Giai Đoạn 7 Phân Hệ 8 Dự Báo Tài Chính Chuỗi Thời Gian & Radar Rủi Ro Thâm Hụt (PLAN-43 / Quyết Định 123, 124, 125):** Hoàn thành Dự báo tổng chi cuối chu kỳ phân tách Weekday/Weekend velocity và Z-Score lọc Outlier, Dự phóng ngày cạn tiền lỏng đối chiếu với ngày nhận lương kế tiếp (`IsLiquidityCrisis`), Radar thâm hụt danh mục thiết yếu (`Needs`), Phòng thí nghiệm Sandbox What-If in-memory 0ms (Lump-sum, Vay trả góp DTI, Sụt giảm thu nhập), MVVM Repositories (`IForecastingRepository`) và Wireframe `ForecastingPage.xaml` (`203/203 Tests Passed 100%`, 0 Warning 0 Error).
- [x] **Giai Đoạn 8 Phân Hệ 9 Cú Hích Thaler Nudge, Khoảng Trễ 24h & Trợ Lý XAI (PLAN-44 / Quyết Định 126):** Hoàn thành Kích hoạt cú hích thời gian thực DSA 85% và Pay Yourself First 20%, Chống spam 4h, Trợ lý XAI giải trình 3 thành phần (Công thức toán học, Bằng chứng thực nghiệm, Hệ quả tương lai), Đề xuất tái cân đối ngân sách động bảo toàn 100% Budget Invariant, Phê duyệt Human-in-the-loop ACID Transaction, Khoảng trễ tâm lý 24h (Cooling-off Period) phong tỏa khoản chi $\ge 3$ triệu không trừ tiền ví, quy đổi giờ công lao động, đếm ngược 24h, Xác nhận mua/Hủy bỏ/Mở khóa khẩn cấp, MVVM Repositories (`INudgeRepository`, `ICoolingOffRepository`) và Wireframe `NudgeFeedPage.xaml`, `CoolingOffHubPage.xaml` (`233/233 Tests Passed 100%`, 0 Warning 0 Error).
- [x] **Giai Đoạn 10 Tinh Chỉnh, Kiểm Định Hành Vi & Hoàn Thiện Toàn Diện Hệ Thống AI (PLAN-45, PLAN-46 & PLAN-47 / Quyết Định 128 - 135):** Hoàn thành Phiên họp Hội đồng Toàn diện, kết nối Gemini 2.5 Flash Vision OCR hóa đơn/phiếu lương đối soát $\le 1.000$ đ, Voice Intent Router Agent, Hybrid Multi-Source Data Import (Excel + NLP), Android Native `BankNotificationListenerService` (`276/276 Tests Passed 100%`). Mở rộng **Chặng 10.5 (PLAN-47 / Quyết định 135)**: Triển khai Động cơ Hành động Tự hành Hướng máy chủ (Server-Driven Action Protocol), khay hành động Action Inbox, phân tầng ra quyết định theo ngưỡng xác suất Jev ($\ge 0.85$ Lưu nhanh 1-chạm, $0.5 - 0.85$ Jev Choice, $< 0.5$ Im lặng), loại bỏ triệt để văn phong robot AI; Frontend dựng Wireframe `SmartActionCard` và cập nhật tài liệu kỹ thuật bàn giao `HUONG_DAN_FE.md`.
- [ ] **Giai Đoạn 11 Tổng Kiểm Toán Full-Stack E2E, Đóng Gói Demo 1-Click, Báo Cáo Cuối Kỳ MAUI & Đóng Gói APK:** Bám sát Deadline 1 (Thứ Tư, 28/10/2026) nộp Video Demo (dành 1-2 phút phô diễn tính năng Bắt thực tế ngân hàng và Lưu nhanh 1-chạm), Báo cáo DOCX/PDF thuyết minh kiến trúc Server-Driven Action Protocol & Jev Probability, File APK release và nghiệm thu barem điểm cuối kỳ môn Di động.

---

## 6. Quy Tắc Bất Di Bất Dịch Về Quản Trị Tài Liệu (Document Governance Rules)

1. **Khóa Bất Khả Xâm Phạm Khối La Mã Thường (`KhoaLuanTotNghiep.docx`):**
   - Người dùng trực tiếp quản lý và sửa đổi thủ công toàn bộ các trang thuộc Khối thủ tục đầu (Trang bìa ngoài, Bìa lót, Nhiệm vụ KLTN `BM205`, Lời cam đoan `BM206`, Lời cảm ơn `BM207`, Nhận xét GVHD `BM208`, Mục lục `BM209`, Danh mục hình `BM210`, Danh mục bảng `BM211`, Danh mục viết tắt `BM212`...).
   - **Tác tử AI TUYỆT ĐỐI KHÔNG can thiệp, không tự động sửa đổi, không ghi đè và không chạy script format trên các trang này nếu không có lệnh rõ ràng từ người dùng.**
2. **Bảo Tồn Khảo Sát Hiện Trạng (`BaoCao_TienDo_GiuaKy_PTTKHTTT.docx`):**
   - Phần 1 (Giới thiệu tổng quan) và Phần 2 (Khảo sát hiện trạng & Ứng dụng thị trường) do Người dùng trực tiếp nghiệm thu, không tự ý chỉnh sửa nội dung.
3. **Quy Chuẩn Sao Lưu Bắt Buộc:**
   - Luôn duy trì bản sao lưu an toàn `.backup.docx` cho mỗi tài liệu chính trước và sau các thao tác quan trọng.
4. **Quy Chuẩn Thể Thức DNTU (`files/DNTU_quydinhKLTN.docx`):**
   - Dãn dòng 1.5 lines, Before = 6 pt, After = 6 pt, Thụt đầu dòng 1.5 cm, Font 13 pt Times New Roman, Căn đều hai bên (Justified), Thân bảng tuyệt đối không in đậm. Áp dụng cho toàn bộ khối nội dung học thuật từ Chương 1 trở đi.
5. **Quy Chuẩn Bullet & Danh Sách Liệt Kê (Dạng Phân Ý - Quyết định 53):**
   - Ký hiệu thống nhất duy nhất: Dấu gạch ngang `"- "` (hyphen), không dùng chấm tròn `•` hay gạch dài `–`.
   - Cấu trúc layout phân ý: Dòng đầu chứa dấu `"- "` bắt đầu ở mốc **`1.5 cm`** (bằng đúng Tab đầu dòng văn bản thường). Từ dòng thứ 2 trở đi khi rớt dòng thì căn lề ở mốc **`0.75 cm`** (thụt vào 1/2 Tab thông thường so với lề trái 0.0 cm, không ngang bằng văn bản thường để phân biệt rõ tính phân cấp nội dung).
   - Thông số kỹ thuật Word: `Left Indent = 0.75 cm`, `First Line Indent = 0.75 cm` (Dòng 1 = 1.5 cm, Dòng 2+ = 0.75 cm).
   - Khoảng cách đoạn giữ nguyên chuẩn toàn tài liệu: `Before = 6 pt`, `After = 6 pt`, `Line Spacing = 1.5 lines`.
   - Default Tab Stop trong Word (`settings.xml`): Bắt buộc thiết lập `<w:defaultTabStop w:val="850"/>` (`1.5 cm`), không để mặc định Word 720 dxa (1.27 cm / 1.25 cm).
   - Ưu tiên văn xuôi học thuật: Nếu nội dung có thể diễn đạt bằng câu văn liên tục, bắt buộc chuyển thành các đoạn văn học thuật hoàn chỉnh (`First line indent = 1.5 cm`, `Left = 0`, không dùng bullet).
6. **Thống Nhất Tên Đề Tài Chính Thức Toàn Hệ Thống:**
   - Tiếng Việt: **HỆ THỐNG QUẢN LÝ CHI TIÊU VÀ HOẠCH ĐỊNH TÀI CHÍNH CÁ NHÂN THÔNG MINH (TÍCH HỢP TRÍ TUỆ NHÂN TẠO - AI)**
   - Tiếng Anh: **SMART PERSONAL EXPENSE AND FINANCIAL PLANNING MANAGEMENT SYSTEM WITH AI INTEGRATION**
7. **Tài Liệu Thuyết Minh Kiến Trúc & Nghiệp Vụ Cốt Lõi:**
   - Xem toàn văn thuyết minh 10 Use Cases và 6 Bounded Contexts của Class Diagram tại: [`project_context/reports/TongQuan_KienTruc_NghiepVu_PFM.md`](project_context/reports/TongQuan_KienTruc_NghiepVu_PFM.md).

