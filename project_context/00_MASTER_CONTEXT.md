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
│ TRỤ CỘT 2: BỘ NHÂN AI SOFTWARE ENGINEER (core_engine/)                 │
│ - Hội Đồng 13 Chuyên Gia Kiệt Xuất Full-Stack hóa thân các danh nhân.  │
│ - Tool Layer an toàn (Filesystem Sandbox, Terminal Runner, Git Diff).  │
│ - Chu trình làm việc chuẩn PTTKHTTT khép kín từ Khảo sát đến Final RFC.│
│ - Tự động sinh Ma Trận Truy Vết Nguồn Gốc (Traceability Matrix).       │
│ - Hệ thống thẩm định chất lượng sâu (Deep Content Quality Gates).      │
│ - 14 Kỹ năng Antigravity IDE Skills (.agents/skills/) & CLI Runner.    │
└────────────────────────────────────────────────────────────────────────┘
```

---

## 3. Bản Đồ Roster 13 Chuyên Gia Danh Nhân (Full-Stack Council Roster)

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
| **Agent 09** | [`core/09_brad_frost.md`](core/09_brad_frost.md) | **Brad Frost & Don Norman** | Atomic Design System & UI/UX WCAG 2.1 AA | `02b_frontend_ui_spec.md` |
| **Agent 10** | [`core/10_dan_abramov.md`](core/10_dan_abramov.md) | **Dan Abramov & Vjeux** | Frontend Client State & Dashboard Net Worth | `02b_frontend_ui_spec.md` |
| **Agent 11** | [`core/11_bruce_schneier.md`](core/11_bruce_schneier.md) | **Bruce Schneier & OWASP** | Bảo mật Dữ liệu Tài chính, Mã hóa AES-256 | `03_technical_adr.md` |
| **Agent 12** | [`core/12_kent_beck.md`](core/12_kent_beck.md) | **Kent Beck & Martin Fowler** | Kim Tự Tháp Kiểm Thử TDD & Verification Plan | `04_final_rfc.md` |

---

## 4. Bản Đồ Thư Mục & Vai Trò (System Directory Map)

| Thư mục / Tệp | Vai trò cốt lõi |
| :--- | :--- |
| [`README.md`](README.md) | Cẩm nang tổng quan dự án, kiến trúc hệ thống và hướng dẫn khởi chạy. |
| [`AGENTS.md`](AGENTS.md) | Luật chơi chung & quy ước điều phối Hội đồng Đa tác tử. |
| [`run_council_session.py`](run_council_session.py) | CLI Runner chính thức điều phối phiên bàn thảo 4 pha. |
| [`.agents/skills/`](.agents/skills/) | 14 Kỹ năng Antigravity IDE Customization Skills (`council-*`). |
| [`.agents/rules/`](.agents/rules/) | Các quy tắc bắt buộc nạp tự động vào Antigravity IDE (`00_load_project_context_first.md`). |
| [`core/`](core/) | Đặc tả 13 Persona danh nhân chuẩn Production (Agent 00 đến Agent 12). |
| [`core_engine/`](core_engine/) | Bộ nhân Python Engine (Council, Skills, Tools Sandbox, Workflow, Memory). |
| [`docs/`](docs/) | Tài liệu đề tài gốc (`specs/`) và các script giải thuật (`standalone_scripts/`). |
| [`sessions/`](sessions/) | Các phiên bàn thảo 4 pha & 4 bộ Benchmark Sessions thực tế cá nhân (`session_01` đến `session_04`). |
| [`templates/`](templates/) | Bộ 5 mẫu tài liệu đầu ra chuẩn IEEE 29148, Frontend UI Spec, ADR, DB 3NF, Feedback. |
| [`tests/`](tests/) | Bộ Unit & Integration Tests tự động đạt chuẩn 100% Passed. |
| [`project_context/`](project_context/) | Nơi lưu trữ nhật ký quyết định, kế hoạch và snapshot ngữ cảnh bền vững. |

---

## 5. Trạng Thái Hoàn Thành Hiện Tại (Current Status)

- [x] **Tầng Công Cụ (Tool Layer):** Filesystem Sandbox, Terminal Runner (`python -B`), Git Diff.
- [x] **Hội Đồng 13 Chuyên Gia Danh Nhân:** Hoàn tất 100% 13 Persona chuẩn đề tài tài chính cá nhân.
- [x] **Sản Phẩm Bàn Giao Frontend Độc Lập (`02b_frontend_ui_spec.md`):** Chuẩn hóa luồng sinh mã linh kiện thực chiến đa nền tảng (React/Angular/Blazor).
- [x] **Tái Định Hướng Cá Nhân (PLAN-09):** Hoàn thành chuyển đổi Agent 02 (Personal Budget Coach) và Agent 06 (Debt & Cashflow Optimizer).
- [x] **Benchmark Sessions Chuẩn PFM:** Hoàn thành 4 phiên thực nghiệm tài chính cá nhân đạt 100% Quality Gates.
- [x] **Kiểm Thử Toàn Diện:** 100% Unit Tests đã pass không lỗi.

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

