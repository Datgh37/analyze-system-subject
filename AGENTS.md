# QUY ƯỚC ĐIỀU PHỐI HỘI ĐỒNG AGENTS (MULTI-AGENT COUNCIL RULES)

Hệ thống Đa Tác tử (Multi-Agent Council) trong thư mục này được thiết kế theo phương pháp luận **BMAD** và quy chuẩn **Production CLAUDE.md** nhằm phục vụ nghiên cứu, phân tích thiết kế và xây dựng đồ án:
**"Hệ thống Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh (Tích hợp AI)"**

---

## 1. Bản Đồ Danh Sách Tác Tử (Full-Stack Agent Roster)

| Mã Agent | Tên File Quy Định | Chuyên Gia Hóa Thân | Vai Trò Chuyên Môn | Đầu Ra Chịu Trách Nhiệm |
| :--- | :--- | :--- | :--- | :--- |
| **Agent 00** | [`core/00_orchestrator.md`](core/00_orchestrator.md) | **Council Orchestrator** | Chủ tịch Hội đồng & Điều phối 4 Pha | `04_final_rfc.md` |
| **Agent 01** | [`core/01_richard_thaler.md`](core/01_richard_thaler.md) | **Richard Thaler (Nobel)** | Tâm lý Chi tiêu & Cú Hích (Nudge) | `01_user_feedback.md` (Phần Nudge/UX) |
| **Agent 02** | [`core/02_personal_budget_coach.md`](core/02_personal_budget_coach.md) | **Personal Budget Coach** | Hoạch định Ngân sách (50/30/20, ZBB, Kakeibo) | `01_user_feedback.md` (Phần Ngân sách & Hạn mức) |
| **Agent 03** | [`core/03_cockburn_ba.md`](core/03_cockburn_ba.md) | **Alistair Cockburn / Debra Paul** | Phân tích Nghiệp vụ (IEEE 29148) | `02_use_case_specs.md` |
| **Agent 04** | [`core/04_eric_evans.md`](core/04_eric_evans.md) | **Eric Evans (Cha đẻ DDD)** | Domain-Driven Design & Clean Arch | `03_technical_adr.md` (Phần Domain/Arch) |
| **Agent 05** | [`core/05_martin_kleppmann.md`](core/05_martin_kleppmann.md) | **Martin Kleppmann & Sadalage** | Universal Database (SQL 3NF & NoSQL) | `03_technical_adr.md` (Phần Database/Polyglot) |
| **Agent 06** | [`core/06_personal_debt_cashflow_optimizer.md`](core/06_personal_debt_cashflow_optimizer.md) | **Debt & Cashflow Optimizer** | Tối ưu hóa Trả nợ (Snowball vs Avalanche) | `03_technical_adr.md` (Phần Debt Solver) |
| **Agent 07** | [`core/07_werner_vogels.md`](core/07_werner_vogels.md) | **Werner Vogels (CTO Amazon)** | Kiến trúc Bất đồng bộ & DevOps | `03_technical_adr.md` (Phần Async/DevOps) |
| **Agent 08** | [`core/08_andrew_ng.md`](core/08_andrew_ng.md) | **Andrew Ng (AI Pioneer)** | Pipeline Bóc tách Hóa đơn Multimodal AI | `03_technical_adr.md` (Phần AI Pipeline) |
| **Agent 09** | [`core/09_brad_frost.md`](core/09_brad_frost.md) | **Brad Frost & Don Norman** | Atomic Design System & UI/UX Specs | `01_user_feedback.md` & `03_technical_adr.md` |
| **Agent 10** | [`core/10_dan_abramov.md`](core/10_dan_abramov.md) | **Dan Abramov & Vjeux** | Cross-Platform & Client State Store | `03_technical_adr.md` (Phần Frontend/Client) |
| **Agent 11** | [`core/11_bruce_schneier.md`](core/11_bruce_schneier.md) | **Bruce Schneier & OWASP** | Fintech Security, AES-256 & STRIDE | `03_technical_adr.md` (Phần Security) |
| **Agent 12** | [`core/12_kent_beck.md`](core/12_kent_beck.md) | **Kent Beck & Martin Fowler** | Full-Stack Test Strategy & QA (E2E) | `04_final_rfc.md` (Phần Verification Plan) |

---

## 2. Quy Trình Vận Hành 4 Pha Của Mỗi Phiên Bàn Thảo (Session Protocol)

Mỗi khi có một yêu cầu tính năng mới (Feature Request), phiên làm việc sẽ được khởi tạo trong `sessions/session_[tên_tính_năng]/` và trải qua đúng 4 pha:

### Pha 1: Trải nghiệm, Tâm lý & UI/UX (Khách hàng & Design)
- **Tác tử thực thi:** `01_richard_thaler`, `02_personal_budget_coach`, `09_brad_frost`.
- **Hành động:** Phản biện mức độ tiện dụng, tâm lý chi tiêu, các tình huống đời thực và phác thảo khung giao diện ban đầu.
- **Tệp kết quả:** `01_user_feedback.md` (áp dụng theo [`templates/user_feedback_template.md`](templates/user_feedback_template.md)).

### Pha 2: Chuẩn hóa Yêu cầu Kỹ thuật (BA)
- **Tác tử thực thi:** `03_cockburn_ba`.
- **Hành động:** Đọc `01_user_feedback.md`, chuẩn hóa thành Use Case Specification theo chuẩn IEEE 29148, viết luồng chính, luồng phụ, ngoại lệ và Acceptance Criteria (Gherkin).
- **Tệp kết quả:** `02_use_case_specs.md` (áp dụng theo [`templates/use_case_spec_template.md`](templates/use_case_spec_template.md)).

### Pha 3: Phản biện Kỹ thuật Toàn diện (Full-Stack Architecture, DB, Security, QA)
- **Tác tử thực thi:** `04_eric_evans`, `05_martin_kleppmann`, `06_personal_debt_cashflow_optimizer`, `07_werner_vogels`, `08_andrew_ng`, `10_dan_abramov`, `11_bruce_schneier`, `12_kent_beck`.
- **Hành động:** Soạn thảo quyết định kiến trúc Clean Architecture, CSDL 3NF, giải thuật đồ thị, async queue, multimodal AI, client state 0ms, mã hóa AES-256 STRIDE và kim tự tháp kiểm thử.
- **Tệp kết quả:** `03_technical_adr.md` (áp dụng theo [`templates/adr_template.md`](templates/adr_template.md)).

### Pha 4: Tổng hợp & Phê duyệt Cuối cùng (Orchestrator)
- **Tác tử thực thi:** `00_orchestrator`.
- **Hành động:** Tổng hợp cả 3 pha trên thành bản RFC cuối cùng đầy đủ, rõ ràng để sẵn sàng chuyển giao cho lập trình viên.
- **Tệp kết quả:** `04_final_rfc.md`.


---

## 3. Các Nguyên Tắc Bất Di Bất Dịch (Immutable Rules)
1. **Durable Context (Ngữ cảnh bền vững):** Mọi giao tiếp giữa các Agent đều diễn ra thông qua việc đọc/ghi các file Markdown trong thư mục `sessions/` và `project_context/`. Không dựa vào trí nhớ hội thoại tạm thời.
2. **Strict Role Isolation:** Agent nào làm đúng việc của Agent đó. BA không bàn về Docker, Khách hàng không bàn về thuật toán đồ thị.
3. **Evidence-Based (Dựa trên bằng chứng):** Mọi kết luận kỹ thuật đều phải có dữ liệu chứng minh (sơ đồ Mermaid, script Python chạy test hoặc bảng so sánh Trade-offs).
4. **Mandatory Context Logging (Ghi chép kế hoạch bắt buộc):** Mỗi khi có kế hoạch mới, sửa đổi yêu cầu hoặc quyết định kỹ thuật, BẮT BUỘC phải lưu snapshot vào `project_context/plans/` và cập nhật `project_context/decisions_and_changes_log.md`.
5. **Portability (Tính khả chuyển):** Tuyệt đối KHÔNG sử dụng đường dẫn tuyệt đối của máy cá nhân trong các tài liệu markdown. Luôn sử dụng đường dẫn tương đối để đảm bảo dự án chạy tốt trên mọi thiết bị và hệ điều hành.
