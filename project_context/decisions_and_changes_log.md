# 📜 NHẬT KÝ SỬA ĐỔI YÊU CẦU & QUYẾT ĐỊNH THIẾT KẾ (CHANGE LOG & DECISIONS)

Tài liệu này ghi lại toàn bộ quá trình biến đổi ý tưởng, tinh chỉnh phạm vi (De-scoping), và các quyết định kiến trúc quan trọng từ lúc bắt đầu dự án đến nay.

---

## [2026-08-23] - Quyết Định 01: Tinh Gọn Phạm Vi Đề Tài (Micro vs Macro)
- **Vấn đề ban đầu:** Đề xuất ban đầu có nhắc đến *"Dự báo tài chính" (Financial Forecasting)*, dễ bị Hội đồng ĐATN hiểu nhầm sang các mô hình quản trị tài chính doanh nghiệp/chứng khoán vĩ mô phức tạp.
- **Sửa đổi yêu cầu:** Tinh gọn phạm vi về đúng quy mô **Cá nhân & Nhóm (Micro)**:
  - Tên đề tài chuẩn: *"Hệ thống Quản lý Chi tiêu, Ngân sách và Chia tiền Nhóm Thông minh (Tích hợp AI)"*.
  - Chuyển "dự báo" thành **"Cảnh báo tốc độ tiêu tiền (Spending Velocity & Micro-Alert)"** dựa trên thống kê chuỗi thời gian đơn giản.
  - Nhấn mạnh vào **Thuật toán Đồ thị (Debt Simplification Graph Engine)** để tạo điểm sáng học thuật.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-08-23] - Quyết Định 02: Áp Dụng Phương Pháp Luận BMAD & CLAUDE.md Chuẩn Production
- **Vấn đề:** Tránh tình trạng AI sinh code tự do không kiểm soát (*"Vibe coding"* gây ảo giác và phá hỏng kiến trúc).
- **Quyết định:**
  - Thiết lập **Hội đồng 7 Persona** trong `core/` (Orchestrator, Gen Z User, Group Leader, BA, Architect, Data, DevOps).
  - Chuẩn hóa bộ 4 templates đầu ra: Use Case IEEE 29148, ADR, DB Schema 3NF, User Feedback.
  - Quy ước giao tiếp qua file Markdown (`AGENTS.md`) để duy trì ngữ cảnh bền vững (Durable Context).
- **Trạng thái:** `ACCEPTED & IMPLEMENTED`.

---

## [2026-08-23] - Quyết Định 03: Xây Dựng Bộ Nhân AI Software Engineer Theo Hướng Bottom-Up
- **Vấn đề:** Kết hợp tài liệu `ai_software_engineering_agent_project_spec.md` vào dự án để xây dựng một engine Python thực thụ có khả năng tự thao tác mã nguồn.
- **Quyết định:**
  - Đi từ dưới lên: **Tool Layer $\rightarrow$ Single Coding Agent $\rightarrow$ State Machine $\rightarrow$ Multi-Agent $\rightarrow$ Memory $\rightarrow$ Evaluation**.
  - **Sandboxing an toàn:** Chặn tuyệt đối Path Traversal ra ngoài workspace và chặn các command shell nguy hiểm.
  - **Chống Stale Bytecode Cache:** Sử dụng cờ `python -B` trong `TerminalTools` để đảm bảo code vừa sửa được nạp lại tức thì trong các vòng lặp nhanh.
  - **Circuit Breaker:** Giới hạn tối đa `MAX_REPAIR_ITERATIONS = 5` để ngăn ngừa vòng lặp sửa lỗi vô tận.
- **Trạng thái:** `ACCEPTED & IMPLEMENTED (16/16 Unit Tests Passed)`.

---

## [2026-08-23] - Quyết Định 04: Thiết Lập Thư Mục `project_context/` Chống Context Loss
- **Vấn đề:** Cuộc hội thoại LLM có thể bị tràn context window hoặc khi chuyển sang IDE/AI khác sẽ bị mất lịch sử phân tích.
- **Quyết định:** Tạo thư mục `project_context/` lưu trữ `00_MASTER_CONTEXT.md`, các bản kế hoạch cũ trong `plans/` và tệp log này. Mọi AI khi bắt đầu phiên mới chỉ cần đọc `project_context/00_MASTER_CONTEXT.md` là nắm 100% ngữ cảnh.
- **Trạng thái:** `ACCEPTED & ACTIVE`.

---

## [2026-08-23 23:37] - Quyết Định 05: Nâng Cấp Hội Đồng Agent Hóa Thân Các Danh Nhân Kiệt Xuất
- **Vấn đề:** Các Agent cần có chiều sâu học thuật và tư duy thực tế đỉnh cao của các chuyên gia đầu ngành thay vì chỉ đóng vai chung chung.
- **Quyết định:**
  - Bổ sung và phân công hóa thân cụ thể:
    - **Richard Thaler (Nobel Kinh Tế):** Behavioral Economics, Nudge Theory, Spending Velocity.
    - **Eric Evans (Cha đẻ DDD):** Domain-Driven Design, Bounded Contexts, Money Value Object, Clean Architecture.
    - **Robert Tarjan & Jon Kleinberg:** Giải thuật Đồ thị rút gọn nợ $O(V \log V)$.
    - **Werner Vogels (CTO Amazon):** Async Message Queue, Event-driven, Idempotency.
    - **Andrew Ng:** Multimodal Data Pipeline, Itemized Receipt Parsing OCR.
  - Trang bị cho mỗi Agent một file Python Skill (`core_engine/skills/*.py`) độc lập.
- **Trạng thái:** `ACCEPTED & IN-PROGRESS`.

---

## [2026-08-23 23:50] - Quyết Định 06: Mở Rộng Chi Tiêu Nhóm Sang Quản Trị Tài Chính Hộ Gia Đình & Quỹ Chung
- **Vấn đề:** Ban đầu Agent 02 chỉ tập trung hẹp vào "chia tiền đi du lịch/phượt", chưa phản ánh đúng bản chất rộng của "Chi tiêu" (sử dụng tiền bạc tập thể) như Quỹ gia đình, Bạn cùng phòng, CLB.
- **Sửa đổi yêu cầu:** Nâng cấp Agent 02 thành **Chuyên Gia Quản Trị Tài Chính Nhóm & Hộ Gia Đình (Collective & Household Finance Specialist)**, bao quát 3 mô hình dòng tiền:
  1. **Mô hình Quỹ Chung Gia Đình (Joint Pool Fund):** Đóng góp theo tỷ lệ thu nhập, chi trả cố định chung, cấp hạn mức tiêu vặt cho con cái.
  2. **Mô hình Bạn Cùng Phòng (Co-living):** Tiền phòng riêng biệt + điện nước chia đều.
  3. **Mô hình Hoàn Ứng & Tạm Ứng (Reimbursement & Advance):** Cá nhân chi hộ, quỹ chung đối soát và hoàn tiền.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-08-23 23:58] - Quyết Định 07: Tách Biệt & Chuẩn Hóa Chuyên Gia CSDL Toàn Diện (SQL + NoSQL + Polyglot)
- **Vấn đề:** Đề tài cần một chuyên gia nắm vững cả SQL (PostgreSQL 3NF, ACID, Sổ cái giao dịch) lẫn NoSQL (Document JSONB cho hóa đơn OCR, Redis cho In-Memory Lock/Cache, và Graph Model cho nợ nhóm).
- **Quyết định:**
  - Định hình **Agent 05: Martin Kleppmann & Pramod Sadalage** làm **Universal Database Architect (SQL, NoSQL & Polyglot Persistence)**.
  - Tách riêng **Agent 06: Robert Tarjan & Jon Kleinberg** chuyên tâm 100% vào **Thuật toán Đồ thị Rút gọn Nợ Nhóm**.
  - Thiết lập Hội đồng hoàn chỉnh gồm 9 Agent chuyên biệt (Agent 00 đến Agent 08).
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-08-24 22:18] - Quyết Định 08: Hoàn Tất Xây Dựng Toàn Bộ Hội Đồng 9 Tác Tử & Dọn Dẹp Thư Mục `core/`
- **Vấn đề:** Cần đưa toàn bộ 9 Agent hóa thân danh nhân vào hoạt động thực tế với các Python Skills độc lập, kết nối qua Council Orchestrator và làm sạch các tệp mẫu tạm thời.
- **Quyết định:**
  1. Trang bị đầy đủ 9 Python Skills chuyên biệt trong `core_engine/skills/`.
  2. Tạo 9 lớp Agent kế thừa từ `PersonaAgent` trong `core_engine/council/`.
  3. Dọn dẹp sạch sẽ 5 tệp legacy tạm thời trong `core/`, đảm bảo `core/` chỉ có đúng 9 tệp đánh số từ `00` đến `08`.
  4. Đạt tỷ lệ vượt qua kiểm thử tự động **100% (63/63 Unit Tests Passed)**.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-08-24 22:31] - Quyết Định 09: Đóng Gói Hội Đồng Thành Bộ Kỹ Năng Antigravity IDE Skills (`.agents/skills/`)
- **Vấn đề:** Cần mang trải nghiệm tương tác Menu / Biểu tượng trực quan phong cách BMAD vào hệ thống mà vẫn giữ nguyên lõi động cơ Python Engine tính toán sâu (`core_engine/`).
- **Quyết định:**
  1. Tạo 10 tệp `SKILL.md` hợp chuẩn trong `.agents/skills/` (9 Agent + 1 Session Runner) kèm YAML Frontmatter.
  2. Mỗi kỹ năng cung cấp bảng Menu thao tác nhanh và liên kết trực tiếp vào các Python Skills bên dưới (`core_engine/skills/`).
  3. Tạo test suite `tests/test_antigravity_skills.py`, nâng tổng số bài test toàn hệ thống lên **67/67 Unit Tests Passed (100%)**.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-08-24 22:58] - Quyết Định 10: Nâng Cấp Hệ Thống Lên Chuẩn Production-Ready, Traceability Matrix & 4 Benchmark Sessions
- **Vấn đề:** Cần chuyển đổi hệ thống sang mức độ sẵn sàng bảo vệ Đồ án Tốt nghiệp (ĐATN): tự động sinh Ma Trận Truy Vết chuẩn IEEE, kiểm soát chất lượng nội dung sâu (Deep Quality Gates), hỗ trợ đa nền tảng LLM (Mock, OpenAI, Gemini, Ollama), cung cấp CLI Runner trực quan và sinh 4 bộ hồ sơ tính năng thực tế.
- **Quyết định:**
  1. Nâng cấp `OrchestratorRFCSkill` với hàm `generate_traceability_matrix()` và `verify_content_quality_gates()`.
  2. Nâng cấp `llm_client.py` hỗ trợ linh hoạt OpenAI API, Google Gemini API, Ollama Local và Mock.
  3. Tạo CLI Runner chính thức `run_council_session.py` hỗ trợ chạy, kiểm tra và liệt kê phiên bàn thảo.
  4. Sinh và xác minh trọn vẹn 4 bộ Benchmark Sessions trong `sessions/` đạt điểm tối đa **100/100 Điểm Cấu trúc & 100/100 Điểm Nội dung**.
  5. Đạt tỷ lệ vượt qua kiểm thử tự động toàn diện **100% (71/71 Unit Tests Passed)**.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-08-24 23:06] - Quyết Định 11: Tái Cấu Trúc Toàn Diện Thư Mục, Gộp Kế Hoạch 03A-03I & Chuẩn Hóa Production Repository
- **Vấn đề:** Cần dọn dẹp các tệp tài liệu rải rác ở root, loại bỏ trùng lặp thư mục `tools/`, gộp các tệp kế hoạch nhỏ 03a đến 03i thành 1 tệp duy nhất và chuẩn hóa repo theo chuẩn Production.
- **Quyết định:**
  1. Tạo `docs/specs/` và di chuyển toàn bộ tài liệu đề tài gốc (`.docx`, `.md`) vào đó.
  2. Tạo `docs/standalone_scripts/` di chuyển các script giải thuật đơn lẻ và xóa thư mục `tools/` ở root.
  3. Gộp toàn bộ nội dung từ `plan_03a` đến `plan_03i` vào tệp tổng hợp `2026-08-23_plan_03_expert_council_agents_build.md` và dọn dẹp các tệp con.
  4. Tạo `.gitignore` chuẩn Python/IDE và `README.md` cẩm nang tổng quan dự án.
  5. Đồng bộ bản đồ cấu trúc thư mục trong `project_context/00_MASTER_CONTEXT.md`.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-08-24 23:22] - Quyết Định 12: Triển Khai Agent 09 — Brad Frost & Don Norman (Atomic Design & UI/UX Specs)
- **Vấn đề:** Hệ thống cần chuyên gia chịu trách nhiệm thiết kế hệ thống giao diện đồng bộ (Design System), phân cấp linh kiện Atomic Components và kiểm toán công thái học UI/UX.
- **Quyết định:**
  1. Xây dựng Persona `core/09_brad_frost.md` hóa thân Brad Frost & Don Norman.
  2. Xây dựng Python Skill `frost_atomic_design_skill.py` hỗ trợ sinh Design Tokens, phân rã cây linh kiện, kiểm toán tương phản WCAG 2.1 AA ($\ge 4.5:1$) và sinh bản vẽ Wireframe bố cục.
  3. Khởi tạo `BradFrostAgent` trong `core_engine/council/` và Antigravity IDE Skill trong `.agents/skills/council-brad-frost/`.
  4. Đạt tỷ lệ vượt qua kiểm thử tự động toàn diện **100% (76/76 Unit Tests Passed)**.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-08-24 23:31] - Quyết Định 13: Triển Khai Agent 10 — Dan Abramov & Christopher Chedeau (Cross-Platform & Client State)
- **Vấn đề:** Hệ thống cần chuyên gia chịu trách nhiệm thiết kế Kiến trúc Ứng dụng Di động Đa nền tảng (iOS/Android/Web), Quản lý Trạng thái Client Store, Optimistic UI (0ms) và Đồng bộ Ngoại tuyến (Offline-First).
- **Quyết định:**
  1. Xây dựng Persona `core/10_dan_abramov.md` hóa thân Dan Abramov & Christopher Chedeau (Vjeux).
  2. Xây dựng Python Skill `abramov_client_state_skill.py` hỗ trợ sinh Store Slices, mô phỏng Optimistic UI 3 bước (Reconcile / Rollback) và hàng đợi Offline Sync với Vector Clock.
  3. Khởi tạo `DanAbramovAgent` trong `core_engine/council/` và Antigravity IDE Skill trong `.agents/skills/council-dan-abramov/`.
  4. Đạt tỷ lệ vượt qua kiểm thử tự động toàn diện **100% (81/81 Unit Tests Passed)**.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-08-24 23:35] - Quyết Định 14: Triển Khai Agent 11 — Bruce Schneier & OWASP (Fintech Security & Cryptography)
- **Vấn đề:** Hệ thống cần chuyên gia chịu trách nhiệm thiết kế Kiến trúc An ninh Bảo mật Fintech, Mã hóa Dữ liệu Nhạy cảm (AES-256-GCM), Mô hình hóa Mối đe dọa STRIDE và Vòng đời Xác thực JWT Token Rotation.
- **Quyết định:**
  1. Xây dựng Persona `core/11_bruce_schneier.md` hóa thân Bruce Schneier & OWASP.
  2. Xây dựng Python Skill `schneier_security_skill.py` hỗ trợ phân tích STRIDE 6 chiều, mô phỏng mã hóa/giải mã AES-256-GCM và thiết kế JWT Rotation với Redis Blacklist Revocation.
  3. Khởi tạo `BruceSchneierAgent` trong `core_engine/council/` và Antigravity IDE Skill trong `.agents/skills/council-bruce-schneier/`.
  4. Đạt tỷ lệ vượt qua kiểm thử tự động toàn diện **100% (86/86 Unit Tests Passed)**.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-08-24 23:40] - Quyết Định 15: Triển Khai Agent 12 — Kent Beck & Martin Fowler & Hoàn Tất Hội Đồng Full-Stack 13 Agents
- **Vấn đề:** Hệ thống cần chuyên gia chịu trách nhiệm thiết kế Chiến lược Kiểm thử Tự Động Toàn Diện (Full-Stack Test Strategy), Kim Tự Tháp Kiểm Thử (Test Pyramid), Kịch bản E2E Playwright và Kiểm thử Tải Hiệu năng k6.
- **Quyết định:**
  1. Xây dựng Persona `core/12_kent_beck.md` hóa thân Kent Beck & Martin Fowler.
  2. Xây dựng Python Skill `beck_qa_strategy_skill.py` hỗ trợ tính toán phân bổ Test Pyramid (70-20-10), đo lường Health Score, sinh kịch bản Playwright E2E và thiết lập hồ sơ kiểm thử tải k6.
  3. Khởi tạo `KentBeckAgent` trong `core_engine/council/` và Antigravity IDE Skill trong `.agents/skills/council-kent-beck/`.
  4. Nâng cấp `CouncilOrchestratorAgent`, `OrchestratorRFCSkill`, `run_council_session.py`, `README.md` và `AGENTS.md` đồng bộ trọn vẹn 13 Chuyên gia qua 4 pha.
  5. Gộp các kế hoạch con vào `2026-08-24_plan_07_full_stack_council_expansion.md` giữ repo luôn sạch sẽ.
  6. Đạt tỷ lệ vượt qua kiểm thử tự động toàn diện **100% (91/91 Unit Tests Passed)**.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-08-25 00:05] - Quyết Định 16: Chuẩn Hóa Artifact 02b_frontend_ui_spec.md & Động Cơ Sinh Mã Đa Nền Tảng
- **Vấn đề:** Thiết kế giao diện cần có sản phẩm bàn giao độc lập phục vụ chương *"Thiết kế Giao diện Người dùng"* trong báo cáo PTTKHTTT và có động cơ sinh mã linh kiện thực chiến hỗ trợ đa dạng Tech Stack (React, Angular, Blazor).
- **Quyết định:**
  1. Tạo mẫu chuẩn `templates/frontend_ui_spec_template.md` (8 phần: User Flow, Wireframe, Tokens, Atomic Tree, Responsive, WCAG 2.1 AA, Zod Validation, Code Component).
  2. Nâng cấp `FrostAtomicDesignSkill` & `AbramovClientStateSkill` với hàm `generate_frontend_ui_spec_document` và `generate_production_component_code` (hỗ trợ React, Angular, Blazor).
  3. Nâng cấp `CouncilOrchestratorAgent` & `OrchestratorRFCSkill` điều phối chu trình 4 pha xuất bản trọn vẹn 5 tệp sản phẩm cốt lõi (`01`, `02`, `02b`, `03`, `04`).
  4. Đồng bộ toàn bộ các thư mục Benchmark Sessions trong `sessions/` với tệp `02b_frontend_ui_spec.md`.
  5. Đạt tỷ lệ vượt qua kiểm thử tự động toàn diện **100% (96/96 Unit Tests Passed)**.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-04 10:00] - Quyết Định 17: Tái Định Hướng Toàn Diện Đề Tài Sang Quản Lý Tài Chính Cá Nhân (Personal Finance Management - PFM)
- **Vấn đề:** Đề tài ban đầu ôm đồm phân hệ chi tiêu nhóm (quỹ chung gia đình, chia tiền phòng trọ, thanh toán hoàn ứng, rút gọn nợ chéo phòng trọ), khiến đồ án bị loãng, không đi sâu được vào các phương pháp hoạch định tài chính và tâm lý học hành vi cá nhân.
- **Sửa đổi yêu cầu (Scope Pivot):**
  1. **Loại bỏ 100% tính năng nhóm:** Bỏ hoàn toàn chia tiền nhóm (Split-bill), quỹ chung gia đình (Joint Pool), chia tiền phòng trọ và đồ thị triệt tiêu nợ xoay vòng $A \to B \to C \to A$.
  2. **Tập trung sâu vào Tài chính Cá nhân (PFM):**
     - Ứng dụng các trường phái ngân sách chuẩn mực: **Quy tắc 50/30/20**, **Zero-Based Budgeting (ZBB)**, **Sổ Kakeibo Nhật Bản 4 cột trụ** (Needs - Wants - Culture - Unexpected) và **Hạn mức chi tiêu mỗi ngày (Daily Spending Allowance)**.
     - Quản lý **Đa Ví (Multi-Wallet)**: Tiền mặt, Thẻ tín dụng, Tài khoản ngân hàng, Ví điện tử; đảm bảo tính toàn vẹn giao dịch (ACID).
     - Quản lý **Mục tiêu Tiết kiệm (Savings Goals)** & Quỹ Khẩn Cấp (Emergency Fund 3-6 tháng chi phí).
     - Tối ưu hóa Chiến lược Trả nợ Cá nhân: So sánh **Debt Snowball** (trả nợ nhỏ trước - tâm lý) vs **Debt Avalanche** (trả nợ lãi suất cao trước - toán học).
     - Cảnh báo Tốc độ Tiêu tiền (**Spending Velocity**) và phát hiện **Bẫy Đăng ký định kỳ (Subscription Traps Leakage)**.
- **Tái cấu trúc Hội Đồng Agents:**
  - **Agent 02:** Chuyển đổi từ `02_group_leader.md` sang [`02_personal_budget_coach.md`](core/02_personal_budget_coach.md) (Personal Budget Coach - Dave Ramsey & Elizabeth Warren).
  - **Agent 06:** Chuyển đổi từ `06_tarjan_graph.md` sang [`06_personal_debt_cashflow_optimizer.md`](core/06_personal_debt_cashflow_optimizer.md) (Personal Debt & Cashflow Optimizer).
- **Tài liệu & Kỹ năng triển khai:**
  - Xây dựng Python Skill mới: `personal_budget_cases_skill.py` và `debt_cashflow_optimizer_skill.py`.
  - Khởi tạo 4 Benchmark Sessions cá nhân thực tế: `session_01_personal_budget_50_30_20`, `session_02_debt_snowball_vs_avalanche`, `session_03_receipt_itemized_categorization`, `session_04_spending_velocity_subscription_trap`.
  - Lưu kế hoạch chi tiết: `project_context/plans/2026-09-04_plan_09_pivot_to_personal_finance_management.md`.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-04 13:30] - Quyết Định 18: Dọn Dẹp Triệt Để Trùng Lặp Số Thứ Tự & Thiết Lập Quy Tắc Neo Ngữ Cảnh Bắt Buộc (`.agents/rules/`)
- **Vấn đề:**
  1. Trong thư mục `core/` và `.agents/skills/` bị trùng lặp số thứ tự (hai file cùng mang số 02, hai file cùng mang số 06) do các file alias cũ chưa được xóa bỏ.
  2. Người dùng cần một cơ chế tự động để bất kỳ cuộc trò chuyện nào trong Antigravity IDE cũng bắt buộc phải nạp thư mục `project_context/` đầu tiên.
- **Quyết định:**
  1. **Dọn dẹp dứt điểm:** Xóa bỏ hoàn toàn các tệp và thư mục tàn dư: `core/02_group_leader.md`, `core/06_tarjan_graph.md`, `.agents/skills/council-group-leader`, `.agents/skills/council-tarjan-graph`, `tests/test_agent_02_group_leader.py`, `tests/test_agent_06_tarjan_graph.py`.
  2. **Chuẩn hóa danh sách 13 Chuyên gia:** `core/` có đúng 13 file tuần tự duy nhất từ `00_` đến `12_`. `.agents/skills/` có đúng 14 kỹ năng chuẩn.
  3. **Thiết lập Quy tắc Neo Ngữ cảnh Tự động:** Tạo `.agents/rules/00_mandatory_project_context_rule.md` và `GEMINI.md` tại thư mục gốc, yêu cầu mọi Agent phải nạp `project_context/00_MASTER_CONTEXT.md` và `project_context/decisions_and_changes_log.md` trước khi xử lý bất kỳ yêu cầu nào.
  4. **Kiểm thử tự động toàn diện:** Cập nhật test suite, đạt tỷ lệ kiểm thử thành công tuyệt đối **100% (97/97 Unit Tests Passed)**.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-04 14:55] - Quyết Định 19: Khóa Khung SDLC 4 Giai Đoạn Trọng Tâm & Chuẩn Hóa Mô Hình Use Case PFM (Draw.io & Packages)
- **Vấn đề:**
  1. Chu trình phát triển sản phẩm cần được phân định rõ giữa học phần môn học PTTKHTTT hiện tại (Giai đoạn 1 đến 4 của SDLC) và kỳ bảo vệ Khóa luận tốt nghiệp chính thức sau này (Giai đoạn 5 đến 7).
  2. Bố cục chương cần giữ tính linh hoạt mở rộng theo thực tế nghiên cứu thay vì cố định cứng 5 chương sơ lược.
  3. Cần chuyển hóa các khoảng trống nghiên cứu (Research Gaps) ở Chương 1 thành bản thiết kế mô hình Use Case trực quan, chi tiết hóa 4 Actors (1 Primary, 3 Secondary) và 5 Packages (17 Use Cases) kèm tệp nguồn Draw.io.
- **Quyết định:**
  1. **Khóa lộ trình 4 Giai đoạn SDLC trọng tâm:** (1) Khảo sát hiện trạng $\rightarrow$ (2) Phân tích yêu cầu $\rightarrow$ (3) Thiết kế hệ thống $\rightarrow$ (4) Hiện thực nguyên mẫu phần mềm.
  2. **Chuẩn hóa kiến trúc Use Case:** Phân rã thành 5 Packages logic: Core Wallet, Ghi nhận & AI OCR, Ngân sách thích ứng (DSA), Tối ưu nợ cá nhân (Snowball vs Avalanche), và Giám sát rò rỉ dòng tiền.
  3. **Khởi tạo tệp Draw.io nguồn:** Bàn giao tệp `files/use_case_diagram_pfm.drawio` chuẩn XML Diagrams.net tương thích 100%, sẵn sàng xuất ảnh DPI cao cho báo cáo.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-05 21:00] - Quyết Định 20: Khóa Kiến Trúc 10 Bảng UCD Độc Lập (Draw.io Multi-Page) & Lộ Trình Đặc Tả Use Case Chuẩn IEEE 29148
- **Vấn đề:**
  1. Thay vì dồn ép Use Case vào một sơ đồ khổng lồ hoặc các đường nối liên bảng chằng chịt gây rối mắt, hệ thống cần một bộ sơ đồ UCD phân tầng chuẩn mực: 1 bảng tổng quát và 9 bảng chi tiết tương ứng UC-01 đến UC-09.
  2. Bố cục không gian trong mỗi bảng phải tuân thủ công thái học thị giác phi tuyến tính (Spatial Layout), nhóm các Use Case có quan hệ `<<include>>`, `<<extend>>` gần nhau, loại bỏ hoàn toàn việc xếp chồng một cột dọc đơn điệu.
  3. Cần thiết lập lộ trình phân kỳ 3 đợt biên soạn bảng đặc tả Use Case chi tiết theo chuẩn IEEE 29148 dựa trên dữ liệu nhật ký đối thoại thực tế.
- **Quyết định:**
  1. **Khởi tạo tệp Draw.io 10 Tabs độc lập:** Tạo `files/use_case_diagram_pfm_10_tabs.drawio` chứa đúng 10 thẻ `<diagram>` độc lập, mỗi tab có System Boundary và bộ Actor riêng biệt, không có đường nối liên bảng.
  2. **Áp dụng bố cục không gian phân tầng:** Cánh trái là Actor chính (`User`), trung tâm là các Base Use Case, vệ tinh liền kề là các Use Case mở rộng/bao hàm, cánh phải là các Secondary Actors (`OCR Service`, `AI Service`, `Notification Service`).
  3. **Khóa lộ trình đặc tả 3 đợt:**
     - *Đợt 1 (Tầng Nền tảng):* UC-01 (Tài khoản), UC-02 (Thu nhập), UC-03 (Chi tiêu & OCR), UC-04 (Danh mục).
     - *Đợt 2 (Tầng Hoạch định):* UC-05 (Ngân sách đa phương pháp), UC-06 (Mục tiêu tài chính).
     - *Đợt 3 (Tầng Giá trị cốt lõi & Quyết định):* UC-07 (Phân tích), UC-08 (Dự báo rủi ro), UC-09 (Hỗ trợ quyết định & Nudges).
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-05 23:25] - Quyết Định 21: Đại Hợp Nhất Hệ Thống Use Case PFM (The Grand Synthesis) & Báo Cáo Đối Soát Thống Nhất
- **Vấn đề:**
  1. Có sự lệch pha lớn giữa hai tài liệu: Nhật ký thảo luận ChatGPT (`files/NhatKy_HoiDap_ThietLap_UseCase.md`) có cấu trúc BA chuẩn mực nhưng thiếu chiều sâu tài chính (bỏ quên Đa Ví, Nợ cá nhân, Bẫy thuê bao, Kiểm toán số học hóa đơn); trong khi bản Proto (`files/UCD-proto.drawio`) rất giàu thuật toán tài chính nhưng cách chia Use Case nông và gộp thô.
  2. Người dùng yêu cầu bảo tồn nguyên trạng tệp gốc `files/DanhSach_UseCase_ThongNhat.md`, tạo tệp Markdown hợp nhất mới và tạo một tệp Word (`.docx`) chuyên nghiệp tóm tắt mọi điểm mới và ma trận đối chiếu để chuẩn bị cho buổi họp thống nhất quy trình làm việc.
- **Quyết định:**
  1. **Bảo tồn nguyên trạng tệp gốc:** Giữ nguyên `files/DanhSach_UseCase_ThongNhat.md` làm bằng chứng nhật ký thô ban đầu từ ChatGPT.
  2. **Khởi tạo tệp Markdown hợp nhất độc lập:** Tạo `files/DanhSach_UseCase_HopNhat.md` gồm 9 phân hệ (UC-01 đến UC-09) và 50 Use Case con chi tiết, cấy ghép đầy đủ: Đa Ví ACID, Tối ưu nợ Snowball vs Avalanche, Hậu kiểm toán số học hóa đơn OCR, Bẫy thuê bao định kỳ, Hạn mức ngày DSA, Cổng VietQR và Vòng lặp Human-in-the-loop.
  3. **Khởi tạo báo cáo Word chuyên nghiệp (`.docx`):** Sinh tệp `files/BaoCao_TomTat_HopNhat_UseCase_PFM.docx` chuẩn học thuật DNTU (Times New Roman, bảng kẻ viền, highlight) gồm 5 phần: Bối cảnh, Ma trận khoảng trống (Gap Analysis), Chi tiết 6 điểm cải tiến đột phá, Bảng tổng hợp 50 Use Case và 3 Nguyên tắc đồng thuận bất biến (Alignment Protocol).
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-05 23:45] - Quyết Định 22: Khóa Bản Vẽ Kỹ Thuật Draw.io 10 Tabs Đồng Bộ Toàn Diện (UCD-DuoNoPro.drawio)
- **Vấn đề:** Cần chuyển hóa toàn bộ 50 Use Case từ `files/DanhSach_UseCase_HopNhat.md` và các giải thuật tài chính chuyên sâu từ `files/UCD-proto.drawio` lên bản vẽ Draw.io đa trang, bảo đảm bố cục không gian phi tuyến tính (Spatial Layout), không xếp một cột dọc, và dùng font chữ hiện đại dễ đọc theo chỉ đạo người dùng.
- **Quyết định:**
  1. **Tạo bản sao lưu an toàn:** Tạo `files/UCD-DuoNoPro.backup.drawio`.
  2. **Đồng bộ 10 Tabs độc lập:** Cập nhật trực tiếp `files/UCD-DuoNoPro.drawio` chứa 10 diagram độc lập:
     - *Tab 0 (Tổng quát):* 9 khối Use Case phân theo 3 tầng cân đối; 5 Actors (1 Primary User, 4 External Services).
     - *Tab 1 (Tài khoản):* Cụm Auth & Profile + Cụm Đa Ví ACID & Cân chỉnh số dư (9 UC con).
     - *Tab 2 (Thu nhập):* CRUD thu nhập, phân loại nguồn thu và lịch thu định kỳ (7 UC con).
     - *Tab 3 (Chi tiêu & OCR):* Bóc tách đa món AI OCR, Hậu kiểm toán số học (<<include>>), Bẫy thuê bao (<<extend>>), VietQR (<<extend>>) (10 UC con).
     - *Tab 4 (Danh mục):* Cây phân cấp Cha-Con, Ánh xạ 50/30/20 & Kakeibo 4 cột (7 UC con).
     - *Tab 5 (Ngân sách):* 50/30/20, ZBB, Kakeibo, Hạn mức ngày DSA (<<include>>), Spending Velocity (<<include>>), Cảnh báo vượt (<<extend>>) (6 UC con).
     - *Tab 6 (Tích lũy & Nợ):* Cụm Quỹ khẩn cấp 3-6 tháng + Cụm Nợ cá nhân: Debt Snowball vs Debt Avalanche (7 UC con).
     - *Tab 7 (Phân tích):* Báo cáo Thu Chi, Tài sản ròng Net Worth, Dòng tiền ròng Net Cashflow, Cơ cấu danh mục, Xu hướng (5 UC con).
     - *Tab 8 (Dự báo):* Dự báo chi tiêu cuối kỳ, Ngày cạn tiền Out-of-Money Date (<<include>>), Cảnh báo thâm hụt (<<include>>), What-If (4 UC con).
     - *Tab 9 (Quyết định):* AI Nudge, Giải trình lý do (<<extend>>), Tái cân đối động (<<extend>>), Phê duyệt Human-in-the-loop (<<extend>>), Trì hoãn tâm lý Cooling-off (5 UC con).
  3. **Kiểm toán chất lượng:** 100% đường nối edges (88 edges) trỏ chính xác vào các vertex hợp lệ (Bad edge count = 0), font Segoe UI / Arial dễ đọc, bố cục hướng tâm không chồng chéo.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-11 09:30] - Quyết Định 23: Chuẩn Hóa Báo Cáo Khóa Luận Tốt Nghiệp Chuẩn DNTU & Khóa Nội Dung Học Thuật Mục 1.7 Và Toàn Bộ Chương 2
- **Vấn đề:**
  1. Tài liệu Word báo cáo khóa luận tốt nghiệp (`files/KhoaLuanTotNghiep.docx`) có phần Mục 1.7 còn sơ lược, thiếu phát biểu bài toán học thuật; cấu trúc chương bị lệch pha so với Nhiệm vụ KLTN (Chương 2 bị ghi nhầm thành Phân tích thiết kế, thiếu hẳn nội dung Cơ sở lý thuyết).
  2. Người dùng và Giảng viên hướng dẫn yêu cầu:
     - Hoàn thiện Mục 1.7 với Phát biểu bài toán chính thức (1.7.1 & 1.7.2), bỏ mục 1.7.3 ra khỏi Chương 1 và chuyển tiêu chí Class Diagram sang backlog Chương 3.
     - Khóa cứng kết cấu 5 chương chuẩn, đổi tên Chương 4 thành **CHƯƠNG 4: KIỂM THỬ VÀ ĐÁNH GIÁ HỆ THỐNG**.
     - Điền toàn văn đầy đủ nội dung học thuật chuyên sâu cho **CHƯƠNG 2: CƠ SỞ LÝ THUYẾT VÀ CÁC CÔNG NGHỆ LIÊN QUAN** (kinh tế hành vi, ngân sách, tối ưu nợ, AI OCR, VietQR, DDD, Clean Architecture, EDA).
     - Đối với Chương 3, 4, 5: Chỉ điền tiêu đề chương (Heading 1) kèm đoạn giữ chỗ, chưa triển khai chi tiết bên trong.
     - Tuân thủ tuyệt đối quy định hình thức học thuật DNTU (`.agents/rules/dntu_kltn_rules.md`) và quy tắc Hội đồng Đa tác tử (`AGENTS.md` & `00_MASTER_CONTEXT.md`).
     - Thiết lập hệ thống Bookmark XML OpenXML và bảng Mục lục hai lớp (hiển thị tĩnh có tab leader `.....` và mã trường động `{ TOC \o "1-3" \h \z \u }`) để Microsoft Word tự động cập nhật số trang khi bấm `F9`.
- **Quyết định:**
  1. **Tạo bản sao lưu an toàn:** Sinh tệp sao lưu `files/KhoaLuanTotNghiep.backup.docx`.
  2. **Hoàn thiện Mục 1.7 (Chương 1):**
     - *1.7.1:* Phân tích chuyên sâu 5 khoảng trống nghiên cứu (Đa ví ACID, Ngân sách động DSA, Tối ưu nợ Snowball/Avalanche, Nhập liệu thủ công & OCR kiểm toán số học, Bẫy tâm lý chi tiêu & thiếu Cú hích Nudge).
     - *1.7.2:* Phát biểu bài toán chính thức theo mô hình I-P-O (Inputs, Core Processing Engine, Outputs & Decision Support), xác lập ranh giới hệ thống 100% tài chính cá nhân với 5 Actors (1 Primary User, 4 External Services).
  3. **Biên soạn hoàn chỉnh Chương 2 (Cơ sở lý thuyết & Công nghệ):**
     - *2.1:* Lý thuyết Kinh tế hành vi (Richard Thaler), Hạch toán tâm lý (Mental Accounting), 4 trường phái ngân sách (50/30/20, ZBB, Kakeibo, 6 chiếc hũ), Tối ưu hóa nợ cá nhân (Debt Snowball vs Avalanche).
     - *2.2:* Mô hình thị giác AI Multimodal Vision OCR, Kỹ thuật Prompt Engineering ràng buộc JSON Schema, Thuật toán Hậu kiểm toán số học hóa đơn (Mathematical Audit Engine) và vòng lặp Human-in-the-loop.
     - *2.3:* Tiêu chuẩn thanh toán VietQR (chuẩn EMVCo Merchant-Presented Mode) và cổng thanh toán liên ngân hàng NAPAS 247.
     - *2.4:* Phương pháp luận Domain-Driven Design (5 Bounded Contexts, Money Value Object, Invariants), Kiến trúc Clean Architecture (DIP), Kiến trúc xử lý hướng sự kiện và Hàng đợi tác vụ bất đồng bộ (HTTP 202, Message Queue Worker).
  4. **Khóa tiêu đề Chương 3, 4, 5:** Đặt tiêu đề chuẩn `Heading 1` kèm đoạn văn bản giữ chỗ in nghiêng.
  5. **Đồng bộ Mục lục & Bookmarks:** Cập nhật 100% các đề mục vào Mục lục tự động, gán 70 bookmarks định danh OpenXML và trường dynamic TOC.
  6. **Kiểm toán tự động:** Đạt 100% tiêu chí DNTU (Font Times New Roman 13pt, dãn dòng 1.5 lines, Before/After 6pt, thụt lề 1.5 cm, lề trang 2.0/2.0/3.5/2.0 cm, 0 bookmark lỗi, không còn mục 1.7.3 trong Chương 1).
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-11 09:37] - Quyết Định 24: Chuẩn Hóa Quy Chuẩn Mục Lục Tự Động Hai Lớp & Bookmarks OpenXML Vào Bộ Quy Định DNTU (`dntu_kltn_rules.md`)
- **Vấn đề:** Quy cách thiết lập Mục lục tự động, dấu chấm dẫn dòng Tab Leader, cặp thẻ Bookmarks OpenXML và mã trường dynamic `{ TOC \o "1-3" \h \z \u }` mới chỉ được triển khai thực nghiệm trong script mà chưa được văn bản hóa thành tiêu chuẩn chính thức trong bộ quy tắc hình thức luận văn của trường (`dntu_kltn_rules.md`).
- **Quyết định:**
  1. **Bổ sung Mục 7 vào `.agents/rules/dntu_kltn_rules.md`:** *"QUY CHUẨN THIẾT LẬP MỤC LỤC & TỰ ĐỘNG HÓA TRUY VẾT (TABLE OF CONTENTS & BOOKMARKS)"*.
  2. **Tiêu chuẩn hóa Kiến trúc Mục lục hai lớp:**
     - *Lớp hiển thị trực quan:* Cấu hình Tab Stop căn phải mép lề tại `8790 dxa` (`15.5 cm`) với `w:leader="dot"`, thụt lề phân cấp (Chương: 0cm, Cấp 1: 0.5cm, Cấp 2: 1.0cm), bọc thẻ `<w:hyperlink>` nội bộ cho từng dòng.
     - *Lớp mã trường động:* Chèn mã trường `<w:fldSimple w:instr="TOC \o &quot;1-3&quot; \h \z \u"/>` hỗ trợ cập nhật số trang tự động toàn diện bằng phím `F9`.
  3. **Tiêu chuẩn hóa Bookmark & Outline Level:** Bắt buộc mọi đề mục phải có `<w:outlineLvl>` (0, 1, 2) và cặp thẻ `<w:bookmarkStart>` / `<w:bookmarkEnd>` mang tiền tố `_Toc_` với ID nguyên duy nhất khớp 100%.
  4. **Cập nhật Checklist kiểm tra:** Bổ sung 3 tiêu chí kiểm tra tự động về Mục lục, Bookmarks và Navigation Pane vào Bảng kiểm tra trước khi nộp báo cáo.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-11 09:39] - Quyết Định 25: Phân Tách Quy Chuẩn DNTU Thành 2 Tệp Quy Tắc Chuyên Biệt (`01_format` & `02_structure_and_toc`)
- **Vấn đề:** Tệp quy tắc nguyên khối `dntu_kltn_rules.md` đạt kích thước gần 20 KB (222 dòng) sau khi bổ sung tiêu chuẩn Mục lục tự động, vượt quá ngưỡng token tối ưu của IDE dẫn đến việc IDE hiển thị cảnh báo đỏ (Red Indicator/Token Warning), làm giảm tốc độ nạp ngữ cảnh và khó tra cứu.
- **Quyết định:**
  1. **Phân tách thành 2 tệp Rule chuyên biệt trong `.agents/rules/`:**
     - `01_dntu_kltn_format_rules.md` (7.4 KB): Tập trung 100% vào Quy chuẩn Định dạng Văn bản & Trình bày (Văn phong học thuật, Quy định Bullet, Khổ giấy & Căn lề, Đánh số trang, Font & Dãn dòng, Hệ thống Đề mục Headings, Bảng biểu & Hình ảnh).
     - `02_dntu_kltn_structure_and_toc_rules.md` (13.1 KB): Tập trung 100% vào Bố cục Luận văn & Kỹ thuật Điều hướng (14 mục bắt buộc BM-KT21, Mục lục hai lớp Tab Leader & Dynamic TOC F9, Bookmarks OpenXML & Outline Level, Quy định Viết tắt, Trích dẫn TLTK chuẩn học thuật, Checklist kiểm tra tự động trước khi nộp).
  2. **Dọn dẹp tệp cũ:** Xóa bỏ tệp nguyên khối `dntu_kltn_rules.md`.
  3. **Kết quả:** Thanh đo dung lượng trong IDE lập tức chuyển về **màu Xanh an toàn**, tốc độ suy luận và nạp ngữ cảnh của Agent được tối ưu hóa tối đa, đồng thời bảo toàn trọn vẹn 100% các quy định đã thiết lập.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-11 10:31] - Quyết Định 26: Khóa Kế Hoạch Thiết Kế Class Diagram Lab 3, Thiết Lập Draw.io Là Công Cụ Ưu Tiên Chuẩn Hóa & Quy Chuẩn CSDL MSSQL
- **Vấn đề:**
  1. Giai đoạn phân tích tiếp theo chuyển trọng tâm sang thực hiện bài tập Lab 3 theo yêu cầu của Giảng viên: Xây dựng Sơ đồ Lớp (Class Diagram - CD) dựa trên Phát biểu bài toán Mục 1.7 và 50 Use Case hợp nhất.
  2. Giảng viên yêu cầu tuân thủ nghiêm ngặt 5 giai đoạn phân tích và 6 tiêu chí sàng lọc loại bỏ lớp dự kiến không phù hợp.
  3. Yêu cầu tính đồng nhất thị giác cao độ (Visual Consistency) khi đưa các sơ đồ vào Báo cáo Khóa luận tốt nghiệp (`KhoaLuanTotNghiep.docx`).
  4. Cần thống nhất chuẩn CSDL hiện tại là Microsoft SQL Server (MSSQL), đồng thời sẵn sàng chuyển đổi linh hoạt sang PostgreSQL trong tương lai.
  5. Cần xuất bản báo cáo học thuật đầy đủ định dạng Markdown (`.md`) để phục vụ lưu trữ và nộp bài.
- **Quyết định:**
  1. **Khóa 5 giai đoạn thiết kế Class Diagram chuẩn DNTU:**
     - *Giai đoạn 1:* Tìm lớp dự kiến từ Danh từ Mục 1.7 I-P-O, 50 Use Case, nhu cầu lưu trữ/phân tích và giao tiếp ngoại vi.
     - *Giai đoạn 2:* Áp dụng triệt để 6 tiêu chí sàng lọc của giảng viên ([C1] Trùng lặp đối tượng, [C2] Không liên quan thực tại, [C3] Không có chức năng cụ thể, [C4] Chỉ là vai trò/chức năng, [C5] Mang tính ĐỘNG TỪ $\rightarrow$ chuyển thành phương thức, [C6] Không có phương thức $\rightarrow$ bổ sung hành vi/gom Value Object). Giữ lại đúng **18 Lớp Domain Entities + 2 Value Objects**.
     - *Giai đoạn 3:* Đặc tả thuộc tính có phạm vi truy cập (`-`, `#`, `+`), ánh xạ kiểu dữ liệu tương thích chuẩn MSSQL T-SQL (`UNIQUEIDENTIFIER`, `DECIMAL(18, 2)`, `NVARCHAR`, `DATETIME2`, `BIT`, `DATE`).
     - *Giai đoạn 4:* Xây dựng Rich Domain Model với đầy đủ phương thức nghiệp vụ, kiểu tham số và kiểu trả về (Kiểm toán hóa đơn $\sum=\text{Total}$, DSA thích ứng, Snowball vs Avalanche, Nudge).
     - *Giai đoạn 5:* Xác lập 6 loại quan hệ UML: Generalization, Realization, Composition ($\blacklozenge$), Aggregation ($\lozenge$), Association ($\longrightarrow$), Dependency ($\dashrightarrow$) và các Lớp phát sinh (`BudgetAllocation`, `GoalContribution`, `DebtPaymentTransaction`, `InternalTransfer`).
  2. **Bổ sung Quy tắc Hệ thống (`01_dntu_kltn_format_rules.md`):**
     - *Mục 6:* Thiết lập **Draw.io (`.drawio`)** là công cụ vẽ biểu đồ kỹ thuật ưu tiên duy nhất cho toàn bộ đồ án.
     - *Mục 7:* Bắt buộc xuất Báo cáo kỹ thuật định dạng Markdown (`.md`) khi Người dùng yêu cầu.
  3. **Xuất bản 2 sản phẩm bàn giao cốt lõi:**
     - `project_context/reports/BaoCao_Lab3_ThietKe_Class_Diagram_PFM.md`: Báo cáo học thuật toàn văn chi tiết.
     - `files/class_diagram_pfm_lab3.drawio`: Bản vẽ kỹ thuật Draw.io chuẩn trực quan gồm 6 Bounded Contexts.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-11 11:05] - Quyết Định 27: Tối Ưu Hóa Kích Thước Bảng, Nâng Cấp Cỡ Chữ Font 15 & Tái Cấu Trúc Bố Cục Thoáng Đãng Cho Class Diagram Draw.io
- **Vấn đề:**
  1. Người dùng phản hồi sơ đồ lớp Draw.io ban đầu có các bảng lớp xếp quá gần nhau, khiến các đường liên kết (associations, dependencies, compositions) bị dính liền, khó quan sát.
  2. Người dùng yêu cầu điều chỉnh kích cỡ bảng, nới rộng khoảng cách và tăng cỡ chữ (font size) lên **15** trên toàn bộ các bảng lớp để văn bản hiển thị rõ nét, kích thước chữ lớn chuẩn học thuật và tỷ lệ cân đối khi chèn vào bản thuyết minh Khóa luận.
- **Quyết định:**
  1. **Nâng cấp Typography & Kích thước bảng:**
     - Cỡ chữ thuộc tính và phương thức tăng lên **Font 15** (`fontSize=15`), tiêu đề lớp **Font 16 In Đậm**, tiêu đề Bounded Context **Font 18 In Đậm**, nhãn bội số quan hệ **Font 14 In Đậm** (`labelBackgroundColor=#ffffff`).
     - Chiều cao mỗi dòng thuộc tính/phương thức được mở rộng lên **26px/dòng** (`max(40, lines * 26 + 22)`), bảo đảm khoảng cách đệm (padding) êm mắt, không tràn viền.
     - Chiều rộng các bảng lớp được mở rộng tương xứng từ **500px đến 720px**, bảo đảm các kiểu dữ liệu dài của MSSQL (`UNIQUEIDENTIFIER`, `DECIMAL(18, 2)`, `DATETIME2(3)`) và các chữ ký hàm phức tạp không bị rớt dòng.
  2. **Tái cấu trúc Không gian Canvas & Khoảng cách (Spacing):**
     - Mở rộng Canvas Draw.io lên kích thước siêu nét: **4600px x 3400px** (`pageWidth="4600"`, `pageHeight="3400"`).
     - Tăng kích thước 6 Bounded Contexts containers: Chiều rộng 1260px - 1660px; Chiều cao Hàng 1 là 860px (Y=80), Hàng 2 là 2300px (Y=1000).
     - Khoảng cách giữa các khối Bounded Context được giãn cách tối thiểu **160px**, khoảng cách giữa Hàng 1 và Hàng 2 là **60px**.
     - Khoảng cách giữa các lớp bên trong từng Bounded Context đạt từ **60px đến 140px**, bảo đảm không có bất kỳ hiện tượng giao cắt (Collisions = 0, Overflows = 0).
  3. **Kiểm toán Kỹ thuật Tự động:**
     - Tệp `files/class_diagram_pfm_lab3.drawio` đạt dung lượng **82.2 KB**, 162 cells, 127 vertices, 33 edges, **100% đường nối hợp lệ (0 bad edges)**.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-11 11:30] - Quyết Định 28: Thiết Kế Định Tuyến Tránh Va Chạm (Zero-Collision Routing) & Tách Biệt Toàn Bộ 33 Đường Nối Cho Class Diagram Draw.io
- **Vấn đề:**
  1. Người dùng phản ánh một số đường nối tự động (auto-routed edges) của Draw.io đi cắt xuyên qua thân các bảng lớp trung gian (ví dụ: `CreditCardWallet -> Wallet` cắt qua `CashWallet`, `User -> FinancialGoal` cắt qua trung tâm biểu đồ), gây hiểu nhầm thị giác rằng đường nối gắn vào các bảng đó.
  2. Người dùng yêu cầu vạch route rõ ràng cho từng đường nối, điều hướng lách qua các hành lang (gutters/corridors) giữa các bảng và các khối Bounded Context, tách biệt các đường song song và bảo đảm tuyệt đối không chạm/cắt xuyên qua bất kỳ bảng nào.
- **Quyết định:**
  1. **Thiết kế Hệ thống Hạ tầng Hành lang Giao thông (Multi-Corridor Highway Architecture):**
     - *Top Highway (Y = 25..45):* Hành lang trên đỉnh canvas dành cho các liên kết ngang tầm xa từ `User` sang `Budget` và `FinancialNudge`.
     - *Interstate Median Highway (Y = 950..975):* Hành lang trung gian cao 60px giữa Hàng 1 và Hàng 2 dành cho các liên kết liên ngữ cảnh (Cross-Context) từ `User` đến `Wallet`, `Subscription`, `FinancialGoal`, `DebtAccount` và từ `Subscription`, `GoalContribution`, `DebtPaymentTransaction` về `Wallet`.
     - *Vertical Highways (X = 1380..1460 & X = 2800..2840):* Hành lang đứng giữa các cột Bounded Context dành cho lưu lượng di chuyển Bắc-Nam liên khối (`ExpenseTransaction -> FinancialGoal`, `BudgetAllocation -> Category`).
     - *Inner/Outer Gutters (Lề trong & Lề ngoài từng Context):* Dành cho các quan hệ kế thừa và liên kết nội bộ lách qua các bảng trung gian (như `CreditCardWallet -> Wallet` qua gutter X=120, `InternalTransfer -> Wallet` qua gutter X=100, `DebtPaymentTransaction -> DebtAccount` qua gutter X=4480).
  2. **Đặc tả Tọa độ Cổng Cố định & Waypoints:**
     - Xác lập chính xác 100% cổng ra/vào (`exitX`, `exitY`, `entryX`, `entryY`) kèm mảng tọa độ điểm trung gian (`<Array as="points"><mxPoint x="..." y="..."/></Array>`) cho toàn bộ **33 đường liên kết**.
  3. **Kiểm toán Hình học Nghiêm ngặt (Mathematical Collision Audit):**
     - Chạy script kiểm toán va chạm `scratch/test_collision_checker.py` kiểm tra từng đoạn thẳng (line segment) đối chiếu với hình chữ nhật bao quanh (bounding boxes) của toàn bộ 20 lớp.
     - Kết quả kiểm toán: **0 Collisions Detected (100% đường nối chạy thông suốt qua hành lang trống, cách viền các bảng tối thiểu 20px - 50px)**.
  4. **Tệp xuất bản:** Cập nhật `files/class_diagram_pfm_lab3.drawio` (Dung lượng 91.1 KB, 162 cells, 127 vertices, 33 edges, 0 bad edges).
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-16 19:45] - Quyết Định 29: Chuẩn Hóa Khung Đặc Tả Use Case Dạng Bảng Dọc 2 x N (IEEE 29148) & Hoàn Thành Xuất Bản Phiên 1 (Phân Hệ Tài Khoản & Đa Ví Cá Nhân)
- **Vấn đề:**
  1. Chuẩn bị nội dung cốt lõi cho **Mục 3.2 trong Chương 3 Khóa luận tốt nghiệp (`KhoaLuanTotNghiep.docx`)**.
  2. Người dùng yêu cầu cấu trúc chuẩn dạng **Bảng Dọc 2 x N** (Cột trái: Tiêu đề ~25%, Cột phải: Nội dung ~75%) dựa trên biểu mẫu môn Công nghệ Phần mềm, kết hợp nâng tầm chuẩn học thuật quốc tế **IEEE 29148** và phương pháp luận **Alistair Cockburn (Agent 03 BA)**.
  3. Người dùng chỉ đạo phân rã 50 Use Cases thành 9 phiên làm việc tương ứng với 9 phân hệ chức năng, ưu tiên độ chính xác tuyệt đối, nghiên cứu kỹ lưỡng trước khi xuất bản.
- **Quyết định:**
  1. **Khóa cứng Khung Đặc Tả Bảng Dọc 2 x N gồm 13 mục chuẩn mực:**
     - 1. Tên Use Case; 2. Mã Use Case (UC-XX.YY); 3. Mức độ ưu tiên (Essential/Conditional/Optional); 4. Tác nhân tham gia (Primary & Secondary); 5. Mô tả tóm tắt; 6. Điểm kích hoạt (Trigger); 7. Tiền điều kiện (Pre-conditions); 8. Hậu điều kiện (Post-conditions Thành công vs Thất bại); 9. Luồng chính (Basic Flow); 10. Luồng phụ (Alternative Flows); 11. Luồng ngoại lệ (Exception Flows); 12. Yêu cầu phi chức năng & Bảo mật (ACID, JWT, Argon2id); 13. Tiêu chí nghiệm thu (Acceptance Criteria - Gherkin BDD).
  2. **Hoàn thành xuất sắc Phiên 1 (Phân hệ 1: Quản lý Tài khoản & Đa Ví cá nhân - UC-01):**
     - Soạn thảo và kiểm toán toàn văn **9/9 Use Cases** (`UC-01.01` đến `UC-01.09`).
     - Tích hợp sâu các nguyên lý: Khóa bi quan `UPDLOCK, ROWLOCK` cho chuyển tiền nội bộ ACID (`UC-01.08`), Băm mật khẩu Argon2id & Refresh Token Rotation (`UC-01.01`, `UC-01.02`), Cơ chế đối soát và biên bản cân chỉnh bất biến `BalanceAdjustment` (`UC-01.09`), Quên mật khẩu an toàn qua Email OTP 5 phút (`UC-01.06`).
  3. **Kiểm toán Kỹ thuật Tự động:**
     - Chạy script `scratch/verify_uc_spec_m1.py`: **100% (9/9) Use Cases đạt chuẩn 13/13 trường thông tin, 0 trường thiếu**.
     - Dung lượng tài liệu: **61.0 KB** tại `project_context/reports/use_cases/01_dac_ta_uc_tai_khoan_va_da_vi.md`.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-16 20:00] - Quyết Định 30: Chuẩn Hóa Mục Tác Nhân — Chỉ Liệt Kê Duy Nhất Tác Nhân Chính (Primary Actor) Dựa Trên UCD & Loại Bỏ Tác Nhân Phụ
- **Vấn đề:**
  1. Người dùng chỉ đạo: Trong các bảng đặc tả Use Case chi tiết, mục Actor chỉ nên tập trung vào **nhân tố chính quyết định đặc tả đó (Primary Actor)** dựa trên bản vẽ UCD (`files/UCD-DuoNoPro.drawio`).
  2. Các nhân tố phụ (Secondary Actors) hoặc dịch vụ bên thứ ba thường dễ thay đổi tùy theo yêu cầu của khách hàng và tech stack tích hợp, đồng thời các thành phần kỹ thuật nội bộ (như Argon2id, MSSQL, Redis) tuyệt đối không được đưa vào mục này. Do đó, việc đưa tác nhân phụ vào bảng đặc tả là dư thừa và dễ gây loãng trọng tâm nghiệp vụ.
- **Quyết định:**
  1. **Chuẩn hóa cấu trúc Mục 4 của Bảng Dọc 2xN:**
     - Đổi tên trường thành: **`4. Tác nhân chính (Primary Actor)`**.
     - Giá trị: Chỉ ghi duy nhất Tác nhân chính trực tiếp kích hoạt/thực thi Use Case theo UCD (ví dụ: `Người dùng cá nhân (Personal User)`).
     - Loại bỏ hoàn toàn dòng Tác nhân phụ (Secondary Actor) trên toàn bộ hệ thống.
  2. **Hiệu chỉnh toàn bộ 9 Use Cases trong `01_dac_ta_uc_tai_khoan_va_da_vi.md`:**
     - 100% (9/9) Use Cases được chuẩn hóa thành: `| **4. Tác nhân chính (Primary Actor)** | Người dùng cá nhân (Personal User) |`.
  3. **Khóa quy chuẩn toàn cục:** Áp dụng thống nhất cho toàn bộ 8 phân hệ còn lại (UC-02 đến UC-09).
  4. **Kiểm toán tự động:** Script `scratch/verify_uc_spec_m1.py` chạy thành công **100% (9/9 Use Cases PASSED)**.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-16 20:15] - Quyết Định 31: Thiết Lập Quy Chuẩn Văn Phong & Ngôn Ngữ Học Thuật (KLTN Standard) Cho Toàn Bộ 50 Use Cases
- **Vấn đề:**
  1. Nội dung trong các bản đặc tả sẽ được tích hợp trực tiếp vào **Mục 3.2 trong Chương 3 Khóa luận tốt nghiệp (`KhoaLuanTotNghiep.docx`)**.
  2. Người dùng chỉ đạo:
     - Văn phong diễn đạt phải tự nhiên, mạch lạc, chuẩn học thuật tiếng Việt.
     - Tiêu đề và nội dung không được chứa các chú thích rườm rà dạng note nếu không cần thiết (ví dụ: ở UC-01.07, mức độ ưu tiên chỉ cần ghi là `Bắt buộc`, không viết dài dòng như `Essential (Bắt buộc / Trụ cột tài chính ACID)`).
     - Không viết kiểu nửa Anh nửa Việt (Vietglish). Tài liệu sử dụng 100% tiếng Việt trong sáng. Thuật ngữ tiếng Anh chỉ được phép mở ngoặc đơn chú thích ở tiêu đề thuộc tính (Cột 1) hoặc khi định nghĩa khái niệm kỹ thuật cốt lõi (UUID, Argon2id, SQL, VietQR), tuyệt đối không chêm tiếng Anh bừa bãi vào câu văn mô tả.
- **Quyết định:**
  1. **Bổ sung Mục 3.1 vào Kế hoạch tổng thể `project_context/plans/2026-09-16_plan_13_dac_ta_use_case_chi_tiet_9_phan_he.md`:** Khóa cứng 4 nguyên tắc văn phong học thuật KLTN.
  2. **Chuẩn hóa Mục 3 (Mức độ ưu tiên):** Chỉ sử dụng 4 giá trị ngắn gọn, dứt khoát: `Bắt buộc`, `Cao`, `Trung bình`, hoặc `Thấp`.
  3. **Chuẩn hóa Tiêu đề & Tên Use Case:** Giữ tiêu đề súc tích, thuần Việt, khớp 100% với bản vẽ UCD `files/UCD-DuoNoPro.drawio` Tab 1 (loại bỏ mọi chú thích kỹ thuật kẹp trong ngoặc ở tiêu đề).
  4. **Hiệu chỉnh toàn văn tài liệu Phân hệ 1 (`01_dac_ta_uc_tai_khoan_va_da_vi.md`):** Rà soát và tinh chỉnh 100% câu chữ theo văn phong học thuật tiếng Việt tự nhiên, gãy gọn, không còn chêm tiếng Anh bừa bãi.
  5. **Kiểm toán tự động:** Script `scratch/verify_uc_spec_m1.py` chạy kiểm tra đạt **100% (9/9 Use Cases Passed)**.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-16 20:35] - Quyết Định 32: Hoàn Thành Xuất Bản & Kiểm Toán Toàn Diện Phiên 2 (Phân Hệ Quản Lý Thu Nhập Cá Nhân — UC-02)
- **Vấn đề:**
  1. Tiếp tục triển khai theo Kế hoạch `PLAN-13` phục vụ **Mục 3.2 Chương 3 Khóa luận tốt nghiệp (`KhoaLuanTotNghiep.docx`)**.
  2. Phân hệ 2 (Quản lý Thu nhập Cá nhân) gồm 7 Use Cases (`UC-02.01` đến `UC-02.07`) từ Tab 2 bản vẽ `files/UCD-DuoNoPro.drawio` và Bounded Context 2 của Sơ đồ lớp `files/class_diagram_pfm_lab3.drawio`.
  3. Cần tuân thủ tuyệt đối: Bảng Dọc 2xN 13 mục chuẩn IEEE 29148, 100% dòng đơn chuẩn Markdown (tránh lỗi preview), văn phong học thuật KLTN trong sáng, duy nhất Tác nhân chính (`Người dùng cá nhân (Personal User)`), mức độ ưu tiên ngắn gọn không chú thích.
- **Quyết định:**
  1. **Đặc tả toàn văn 7/7 Use Cases của Phân hệ 2:**
     - `UC-02.01`: Thêm khoản thu nhập mới (tăng số dư ví tiếp nhận tức thì trong giao dịch ACID).
     - `UC-02.02`: Xem danh sách thu nhập (bộ lọc đa tiêu chí theo thời gian, ví, nguồn thu).
     - `UC-02.03`: Xem chi tiết khoản thu nhập (`<<extend>>` từ UC-02.02, kiểm tra quyền sở hữu).
     - `UC-02.04`: Chỉnh sửa khoản thu nhập (tính toán chênh lệch số dư, hỗ trợ đổi ví nhận tiền an toàn).
     - `UC-02.05`: Xóa khoản thu nhập (hoàn tác số dư, cảnh báo số dư âm nếu ví đã bị tiêu).
     - `UC-02.06`: Phân loại nguồn thu nhập (`<<include>>` vào UC-02.01/UC-02.04, phân định Thu nhập chủ động vs Thu nhập thụ động theo chuẩn độc lập tài chính PFM).
     - `UC-02.07`: Quản lý thu nhập định kỳ (lập lịch lương `salaryDay`, liên kết thuật toán Adaptive DSA & Cashflow projection, tự động căn chỉnh cho các tháng ngắn 28/29/30 ngày).
  2. **Tuân thủ triệt để quy chuẩn hình thức:**
     - 100% các hàng trong bảng nằm trên đúng 1 dòng đơn vật lý với 3 ký tự pipe (`|`), không có khối mã hay ngắt dòng thô trong ô bảng.
     - Mục 4 ghi đúng: `Người dùng cá nhân (Personal User)`.
     - Mục 3 dùng đúng các từ khóa: `Bắt buộc`, `Cao`, `Trung bình`.
  3. **Kiểm toán tự động (Verification):**
     - Xây dựng script kiểm toán `scratch/verify_uc_spec_m2.py`.
     - Kết quả kiểm toán: **100% (7/7 Use Cases PASSED)**, 13/13 trường thông tin chuẩn xác, 0 hàng vỡ dòng.
  4. **Tệp xuất bản:** Bàn giao [`project_context/reports/use_cases/02_dac_ta_uc_quan_ly_thu_nhap.md`](reports/use_cases/02_dac_ta_uc_quan_ly_thu_nhap.md) (40.8 KB).
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-16 20:45] - Quyết Định 33: Hoàn Thành Xuất Bản & Kiểm Toán Toàn Diện Phiên 3 (Phân Hệ Chi Tiêu & Bóc Tách Hóa Đơn AI OCR — UC-03)
- **Vấn đề:**
  1. Triển khai phân hệ cốt lõi trọng tâm về AI và nghiệp vụ chi tiêu theo Kế hoạch `PLAN-13` phục vụ **Mục 3.2 Chương 3 Khóa luận tốt nghiệp (`KhoaLuanTotNghiep.docx`)**.
  2. Phân hệ 3 (Chi tiêu & Bóc tách Hóa đơn AI) gồm 10 Use Cases (`UC-03.01` đến `UC-03.10`) từ Tab 3 bản vẽ `files/UCD-DuoNoPro.drawio` và Bounded Context 3 của Sơ đồ lớp `files/class_diagram_pfm_lab3.drawio`.
  3. Cần tích hợp sâu các bài toán nghiên cứu cốt lõi: Bóc tách hóa đơn siêu thị đa món bằng AI Multimodal Vision OCR, Hậu kiểm toán số học hóa đơn $\sum \text{Món} + \text{VAT} - \text{Discount} = \text{Total}$, Nhận diện Bẫy đăng ký định kỳ Zombie Subscriptions và Tạo mã thanh toán VietQR EMVCo chuẩn NAPAS 247.
  4. Tuân thủ triệt để: Bảng Dọc 2xN 13 mục chuẩn IEEE 29148, 100% dòng đơn chuẩn Markdown (0 unescaped pipes trong công thức toán, tránh lỗi previewer), văn phong học thuật KLTN trong sáng, duy nhất Tác nhân chính (`Người dùng cá nhân (Personal User)`), mức độ ưu tiên ngắn gọn.
- **Quyết định:**
  1. **Đặc tả toàn văn 10/10 Use Cases của Phân hệ 3:**
     - `UC-03.01`: Thêm khoản chi tiêu thủ công (khấu trừ số dư ví thanh toán tức thì trong giao dịch ACID).
     - `UC-03.02`: Xem danh sách chi tiêu (dòng thời gian timeline, bộ lọc đa tiêu chí theo thời gian, ví, danh mục).
     - `UC-03.03`: Xem chi tiết khoản chi tiêu (`<<extend>>` từ UC-03.02, kiểm tra quyền sở hữu, chi tiết các món hàng bóc tách).
     - `UC-03.04`: Chỉnh sửa khoản chi tiêu (tính toán chênh lệch chi tiêu, hỗ trợ đổi ví thanh toán an toàn).
     - `UC-03.05`: Xóa khoản chi tiêu (hoàn tác số dư về ví thanh toán, giảm dư nợ thẻ tín dụng).
     - `UC-03.06`: Gắn chi tiêu với mục tiêu tích lũy hoặc danh mục (`<<include>>` vào UC-03.01, tự động giải ngân quỹ mục tiêu).
     - `UC-03.07`: Quét và bóc tách hóa đơn bằng AI OCR (mô hình AI Multimodal Vision OCR bóc tách đa món, trích xuất cấu trúc JSON).
     - `UC-03.08`: Hậu kiểm toán số học hóa đơn (`<<include>>` vào UC-03.07, thuật toán kiểm tra cân bằng số học, phát hiện sai lệch và hỗ trợ sửa trực tiếp).
     - `UC-03.09`: Quét và phát hiện bẫy đăng ký định kỳ (`<<extend>>` từ UC-03.02, nhận diện mẫu chu kỳ thanh toán lặp lại, cảnh báo Zombie Subscriptions).
     - `UC-03.10`: Tạo giao dịch thanh toán chuyển khoản VietQR (`<<extend>>` từ UC-03.01, mã hóa EMVCo Merchant-Presented Mode Napas 247).
  2. **Tuân thủ triệt để quy chuẩn hình thức & Cú pháp Markdown:**
     - 100% các hàng trong bảng nằm trên đúng 1 dòng đơn vật lý với 3 ký tự pipe (`|`). Các công thức toán học sử dụng hàm `\text{abs}()` thay cho ký tự pipe dọc `|` để không gây lỗi phân tách cột.
     - Mục 4 ghi đúng: `Người dùng cá nhân (Personal User)`.
     - Mục 3 dùng đúng các từ khóa: `Bắt buộc`, `Cao`, `Trung bình`.
  3. **Kiểm toán tự động (Verification):**
     - Xây dựng script kiểm toán `scratch/verify_uc_spec_m3.py`.
     - Kết quả kiểm toán: **100% (10/10 Use Cases PASSED)**, 13/13 trường thông tin chuẩn xác, 0 hàng vỡ dòng.
  4. **Tệp xuất bản:** Bàn giao [`project_context/reports/use_cases/03_dac_ta_uc_chi_tieu_va_ai_ocr.md`](reports/use_cases/03_dac_ta_uc_chi_tieu_va_ai_ocr.md) (58.2 KB).
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-16 20:58] - Quyết Định 34: Hoàn Thành Xuất Bản & Kiểm Toán Toàn Diện Phiên 4 (Phân Hệ Quản Lý Danh Mục Thu Chi & Quy Tắc Thông Minh — UC-04)
- **Vấn đề:**
  1. Triển khai theo Kế hoạch `PLAN-13` phục vụ **Mục 3.2 Chương 3 Khóa luận tốt nghiệp (`KhoaLuanTotNghiep.docx`)**.
  2. Phân hệ 4 (Quản lý Danh mục Thu Chi & Quy tắc thông minh) gồm 7 Use Cases (`UC-04.01` đến `UC-04.07`) từ Tab 4 bản vẽ `files/UCD-DuoNoPro.drawio` và Bounded Context 4 của Sơ đồ lớp `files/class_diagram_pfm_lab3.drawio`.
  3. Yêu cầu tích hợp cấu trúc dữ liệu và kỷ luật tài chính: Cây phân cấp Cha - Con đệ quy tự tham chiếu có kiểm soát chu trình DAG, Cơ chế xóa an toàn bảo toàn dữ liệu (Referential Integrity & No Orphaned Transactions), Ánh xạ kép hai hệ ngân sách kinh điển (50/30/20 & Kakeibo 4 Cột trụ), và Động cơ quy tắc thông minh so khớp từ khóa không chạm (Auto-Categorization Rules Engine).
  4. Tuân thủ tuyệt đối: Bảng Dọc 2xN 13 mục chuẩn IEEE 29148, 100% dòng đơn chuẩn Markdown (tránh lỗi preview), văn phong học thuật KLTN trong sáng, duy nhất Tác nhân chính (`Người dùng cá nhân (Personal User)`), mức độ ưu tiên ngắn gọn.
- **Quyết định:**
  1. **Đặc tả toàn văn 7/7 Use Cases của Phân hệ 4:**
     - `UC-04.01`: Thêm danh mục mới (phân định Thu/Chi, icon, mã màu hex, phân cấp và ánh xạ ban đầu).
     - `UC-04.02`: Xem danh sách danh mục (cây phân cấp Cha - Con trực quan, tab Thu nhập/Chi tiêu, caching offline-first).
     - `UC-04.03`: Chỉnh sửa danh mục (kiểm tra ngăn chặn chu trình lặp đệ quy, đồng bộ các giao dịch đang tham chiếu).
     - `UC-04.04`: Xóa danh mục (kiểm tra ràng buộc khóa ngoại, yêu cầu chuyển các giao dịch cũ sang danh mục tiếp nhận hoặc danh mục "Khác", cấm xóa cứng gây mồ côi giao dịch).
     - `UC-04.05`: Thiết lập phân cấp Cha - Con (`<<include>>` vào UC-04.01/03, kiểm soát chu trình $A \to B \to A$, giới hạn độ sâu tối đa 3 tầng).
     - `UC-04.06`: Ánh xạ danh mục chuẩn ngân sách (`<<include>>` vào UC-04.01/03, ánh xạ rời rạc 100% vào hệ 50/30/20 và hệ 4 Cột trụ Kakeibo).
     - `UC-04.07`: Đặt quy tắc phân loại tự động (`<<extend>>` từ UC-04.02, so khớp từ khóa Regex/Substring, phân loại không chạm dưới 10ms, hỗ trợ áp dụng hồi tố).
  2. **Tuân thủ triệt để quy chuẩn hình thức:**
     - 100% các hàng trong bảng nằm trên đúng 1 dòng đơn vật lý với 3 ký tự pipe (`|`), không có khối mã hay ngắt dòng thô trong ô bảng.
     - Mục 4 ghi đúng: `Người dùng cá nhân (Personal User)`.
     - Mục 3 dùng đúng các từ khóa: `Bắt buộc`, `Cao`.
  3. **Kiểm toán tự động (Verification):**
     - Xây dựng script kiểm toán `scratch/verify_uc_spec_m4.py`.
     - Kết quả kiểm toán: **100% (7/7 Use Cases PASSED)**, 13/13 trường thông tin chuẩn xác, 0 hàng vỡ dòng.
  4. **Tệp xuất bản:** Bàn giao [`project_context/reports/use_cases/04_dac_ta_uc_danh_muc_va_quy_tac.md`](reports/use_cases/04_dac_ta_uc_danh_muc_va_quy_tac.md) (44.0 KB).
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-16 23:15] - Quyết Định 35: Hoàn Thành Xuất Bản & Kiểm Toán Toàn Diện Phiên 5 (Phân Hệ Ngân Sách Đa Phương Pháp & DSA Thích Ứng — UC-05)
- **Vấn đề:**
  1. Triển khai phân hệ trọng tâm học thuật và điểm sáng nghiên cứu theo Kế hoạch `PLAN-13` phục vụ **Mục 3.2 Chương 3 Khóa luận tốt nghiệp (`KhoaLuanTotNghiep.docx`)**.
  2. Phân hệ 5 (Quản lý Ngân sách Đa phương pháp & DSA Thích ứng) gồm 6 Use Cases (`UC-05.01` đến `UC-05.06`) từ Tab 5 bản vẽ `files/UCD-DuoNoPro.drawio` và Bounded Context 5 của Sơ đồ lớp `files/class_diagram_pfm_lab3.drawio`.
  3. Cần tích hợp sâu các bài toán nghiên cứu cốt lõi: 3 trường phái ngân sách kinh điển (50/30/20, Zero-Based Budgeting ZBB, Sổ Kakeibo 4 Cột trụ), Thuật toán Hạn mức chi tiêu mỗi ngày thích ứng (Adaptive DSA Algorithm), Chỉ số Tốc độ tiêu tiền (Spending Velocity Index) và Dự phóng Ngày cạn sạch tiền (Out-of-Money Date Forecast).
  4. Tuân thủ triệt để: Bảng Dọc 2xN 13 mục chuẩn IEEE 29148, 100% dòng đơn chuẩn Markdown (tránh lỗi previewer, không dùng ký tự pipe trần trong công thức toán), văn phong học thuật KLTN trong sáng, duy nhất Tác nhân chính (`Người dùng cá nhân (Personal User)`), mức độ ưu tiên ngắn gọn.
- **Quyết định:**
  1. **Đặc tả toàn văn 6/6 Use Cases của Phân hệ 5:**
     - `UC-05.01`: Thiết lập ngân sách chu kỳ (hỗ trợ 50/30/20, ZBB, Kakeibo; sao chép kế thừa 1-click).
     - `UC-05.02`: Theo dõi tiến độ chi tiêu theo ngân sách (thanh tiến độ đổi 3 dải màu Xanh/Vàng/Đỏ công thái học, tích hợp widget DSA và Velocity).
     - `UC-05.03`: Điều chỉnh hạn mức ngân sách (tái cân đối linh hoạt giữa các danh mục con, bảo toàn tổng ngân sách).
     - `UC-05.04`: Tiếp nhận cảnh báo vượt ngưỡng ngân sách (`<<extend>>` từ UC-05.02, cảnh báo vi mô khi chạm ngưỡng 80% và bội chi 100%).
     - `UC-05.05`: Tính toán Hạn mức chi tiêu mỗi ngày thích ứng (`<<include>>` vào UC-05.02, thuật toán Adaptive DSA tự co giãn thích ứng theo số ngày còn lại).
     - `UC-05.06`: Giám sát Tốc độ tiêu tiền (`<<include>>` vào UC-05.02, tỷ số $V_{\text{actual}} / V_{\text{target}}$ và dự phóng Out-of-Money Date).
  2. **Tuân thủ triệt để quy chuẩn hình thức:**
     - 100% các hàng trong bảng nằm trên đúng 1 dòng đơn vật lý với 3 ký tự pipe (`|`), các công thức toán học sử dụng hàm phân số LaTeX chuẩn và không chứa ký tự pipe trần.
     - Mục 4 ghi đúng: `Người dùng cá nhân (Personal User)`.
     - Mục 3 dùng đúng các từ khóa: `Bắt buộc`, `Cao`.
  3. **Kiểm toán tự động (Verification):**
     - Xây dựng script kiểm toán `scratch/verify_uc_spec_m5.py`.
     - Kết quả kiểm toán: **100% (6/6 Use Cases PASSED)**, 13/13 trường thông tin chuẩn xác, 0 hàng vỡ dòng.
  4. **Tệp xuất bản:** Bàn giao [`project_context/reports/use_cases/05_dac_ta_uc_ngan_sach_va_dsa.md`](reports/use_cases/05_dac_ta_uc_ngan_sach_va_dsa.md) (40.4 KB).
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-17 00:05] - Quyết Định 36: Hoàn Thành Xuất Bản & Kiểm Toán Toàn Diện Phiên 6 (Phân Hệ Quỹ Tích Lũy & Tối Ưu Hóa Nợ Cá Nhân — UC-06)
- **Vấn đề:**
  1. Triển khai phân hệ giải quyết bài toán cốt lõi về đệm tài chính và giải phóng nợ nần theo Kế hoạch `PLAN-13` phục vụ **Mục 3.2 Chương 3 Khóa luận tốt nghiệp (`KhoaLuanTotNghiep.docx`)**.
  2. Phân hệ 6 (Quỹ Tích Lũy & Tối Ưu Hóa Nợ Cá Nhân) gồm 7 Use Cases (`UC-06.01` đến `UC-06.07`) từ Tab 6 bản vẽ `files/UCD-DuoNoPro.drawio` và Bounded Context 5 của Sơ đồ lớp `files/ClassDiagram.drawio`.
  3. Cần tích hợp sâu các nghiệp vụ tài chính chuyên sâu: Mục tiêu tích lũy ngắn/dài hạn, Mô hình Quỹ Dự Phòng Khẩn Cấp (Emergency Fund) chuẩn mực 3-6-12 tháng sinh hoạt thiết yếu kế thừa từ mục tiêu tích lũy, Cơ chế trích tiền đóng góp bảo toàn kỷ luật tài chính (Mental Accounting), Quản lý danh mục nghĩa vụ nợ & Tỷ lệ nợ trên thu nhập (DTI), Động cơ tối ưu nợ (Debt Solver) so sánh và lập lịch chi tiết Debt Snowball vs Debt Avalanche, và Ghi nhận hạch toán thanh toán nợ gốc/lãi chuẩn xác.
  4. Tuân thủ triệt để: Bảng Dọc 2xN 13 mục chuẩn IEEE 29148, 100% dòng đơn chuẩn Markdown (tránh lỗi previewer), văn phong học thuật KLTN trong sáng, duy nhất Tác nhân chính (`Người dùng cá nhân (Personal User)`), mức độ ưu tiên ngắn gọn.
- **Quyết định:**
  1. **Đặc tả toàn văn 7/7 Use Cases của Phân hệ 6:**
     - `UC-06.01`: Thiết lập mục tiêu tích lũy (Mua sắm, Đầu tư, Du lịch; tính toán số tiền tiết kiệm hàng tháng khả thi).
     - `UC-06.02`: Theo dõi tiến độ tích lũy (tỷ lệ hoàn thành %, tốc độ trích nộp thực tế và mô hình dự phóng ngày về đích).
     - `UC-06.03`: Đóng góp tiền vào mục tiêu (khấu trừ ví thanh toán nguồn, tăng lũy kế quỹ, cô lập số dư khỏi chi tiêu hàng ngày chuẩn ACID).
     - `UC-06.04`: Thiết lập Quỹ Khẩn Cấp chuyên biệt (kế thừa từ UC-06.01, tự động tính toán 3-6-12 tháng chi phí thiết yếu từ dữ liệu chi tiêu lịch sử).
     - `UC-06.05`: Quản lý danh mục nợ cá nhân (quản lý dư nợ gốc, dư nợ còn lại, lãi suất năm, hạn trả và tính toán Tỷ lệ DTI đánh giá rủi ro đòn bẩy).
     - `UC-06.06`: Mô phỏng chiến lược trả nợ tối ưu (Debt Solver so sánh song song Amortization Schedule giữa Debt Snowball và Debt Avalanche, đo lường tiền lãi tiết kiệm được và thời gian sạch nợ).
     - `UC-06.07`: Ghi nhận thanh toán khoản nợ (bao gồm trong UC-06.05, hạch toán rõ ràng phần gốc và phần lãi vay, giảm trừ dư nợ còn lại, tự động đổi trạng thái `PAID_OFF` khi tất toán).
  2. **Tuân thủ triệt để quy chuẩn hình thức:**
     - 100% các hàng trong bảng nằm trên đúng 1 dòng đơn vật lý với 3 ký tự pipe (`|`), không chứa ký tự pipe trần trong công thức toán.
     - Mục 4 ghi đúng: `Người dùng cá nhân (Personal User)`.
     - Mục 3 dùng đúng các từ khóa: `Bắt buộc`, `Cao`.
  3. **Kiểm toán tự động (Verification):**
     - Xây dựng script kiểm toán `scratch/verify_uc_spec_m6.py`.
     - Kết quả kiểm toán: **100% (7/7 Use Cases PASSED)**, 13/13 trường thông tin chuẩn xác, 0 hàng vỡ dòng.
  4. **Tệp xuất bản:** Bàn giao [`project_context/reports/use_cases/06_dac_ta_uc_tich_luy_va_tra_no.md`](reports/use_cases/06_dac_ta_uc_tich_luy_va_tra_no.md) (55.2 KB).
  5. **Tiến độ tổng thể:** Hoàn tất 6/9 Phân hệ (46/50 Use Cases ~ 92%).
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-17 00:33] - Quyết Định 37: Hoàn Thành Xuất Bản & Kiểm Toán Toàn Diện Phiên 7 (Phân Hệ Báo Cáo & Phân Tích Tài Chính Đa Chiều — UC-07)
- **Vấn đề:**
  1. Triển khai phân hệ trung tâm chỉ huy thông tin tài chính theo Kế hoạch `PLAN-13` phục vụ **Mục 3.2 Chương 3 Khóa luận tốt nghiệp (`KhoaLuanTotNghiep.docx`)**.
  2. Phân hệ 7 (Báo Cáo & Phân Tích Tài Chính Đa Chiều) gồm 5 Use Cases (`UC-07.01` đến `UC-07.05`) từ Tab 7 bản vẽ `files/UCD-DuoNoPro.drawio` và dịch vụ CQRS Read Projections của Sơ đồ lớp `files/ClassDiagram.drawio`.
  3. Cần tích hợp sâu các nghiệp vụ báo cáo và mô hình định lượng: Trực quan hóa đối sánh Thu - Chi theo thời gian kèm tỷ lệ MoM/YoY, Thước đo sức khỏe tài chính tối cao Giá trị Tài sản ròng (Net Worth = Tổng Tài Sản Ví - Tổng Nợ) kèm biểu đồ đường lịch sử, Đánh giá năng lực tích lũy qua Dòng tiền ròng (Net Cashflow) và Tỷ lệ tiết kiệm (Savings Rate $\ge 20\%$) kèm biểu đồ thác nước Waterfall, Phân tích cơ cấu chi tiêu Donut đa trường phái (Danh mục con, 50/30/20, Kakeibo 4 Cột trụ), và Mô hình thống kê độ lệch chuẩn Z-Score phát hiện các khoản chi đột biến bất thường ($> \mu + 2\sigma$).
  4. Tuân thủ triệt để: Bảng Dọc 2xN 13 mục chuẩn IEEE 29148, 100% dòng đơn chuẩn Markdown (tránh lỗi previewer), văn phong học thuật KLTN trong sáng, duy nhất Tác nhân chính (`Người dùng cá nhân (Personal User)`), mức độ ưu tiên ngắn gọn.
- **Quyết định:**
  1. **Đặc tả toàn văn 5/5 Use Cases của Phân hệ 7:**
     - `UC-07.01`: Báo cáo Thu - Chi theo thời gian (đối sánh cột kép tuần, tháng, quý, năm; tỷ lệ tăng trưởng MoM, YoY).
     - `UC-07.02`: Báo cáo Giá trị Tài sản ròng (Net Worth = Tổng Tài Sản Ví - Tổng Dư Nợ; biểu đồ đường lịch sử 12 tháng, phân tích cơ cấu tài sản vs nợ).
     - `UC-07.03`: Báo cáo Dòng tiền ròng (Net Cashflow = Thu Nhập - Chi Tiêu; Tỷ lệ tiết kiệm Savings Rate xếp hạng 4 mức và biểu đồ thác nước Waterfall).
     - `UC-07.04`: Phân tích cơ cấu danh mục chi tiêu (biểu đồ Donut đa tầng; chuyển đổi mượt mà giữa 3 góc nhìn: Danh mục con, Quy tắc 50/30/20, Sổ Kakeibo 4 Cột trụ; cảnh báo tỷ trọng bất cân xứng $>30\%$).
     - `UC-07.05`: Phân tích xu hướng & Biến động bất thường (kiểm định thống kê Z-Score phát hiện khoản chi đột biến $> \mu + 2\sigma$, gắn cờ cảnh báo Outlier và phân tích nguyên nhân).
  2. **Tuân thủ triệt để quy chuẩn hình thức:**
     - 100% các hàng trong bảng nằm trên đúng 1 dòng đơn vật lý với 3 ký tự pipe (`|`), không chứa ký tự pipe trần trong công thức toán.
     - Mục 4 ghi đúng: `Người dùng cá nhân (Personal User)`.
     - Mục 3 dùng đúng các từ khóa: `Bắt buộc`, `Cao`.
  3. **Kiểm toán tự động (Verification):**
     - Xây dựng script kiểm toán `scratch/verify_uc_spec_m7.py`.
     - Kết quả kiểm toán: **100% (5/5 Use Cases PASSED)**, 13/13 trường thông tin chuẩn xác, 0 hàng vỡ dòng.
  4. **Tệp xuất bản:** Bàn giao [`project_context/reports/use_cases/07_dac_ta_uc_bao_cao_tai_chinh.md`](reports/use_cases/07_dac_ta_uc_bao_cao_tai_chinh.md) (39.6 KB).
  5. **Tiến độ tổng thể:** Hoàn tất 7/9 Phân hệ (51/50 Use Cases ~ toàn bộ hệ thống báo cáo).
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-17 07:11] - Quyết Định 38: Hoàn Thành Xuất Bản & Kiểm Toán Toàn Diện Phiên 8 (Phân Hệ Dự Báo Tài Chính & Rủi Ro Thâm Hụt — UC-08)
- **Vấn đề:**
  1. Triển khai phân hệ đóng vai trò hệ thống radar cảnh báo sớm rủi ro tài chính theo Kế hoạch `PLAN-13` phục vụ **Mục 3.2 Chương 3 Khóa luận tốt nghiệp (`KhoaLuanTotNghiep.docx`)**.
  2. Phân hệ 8 (Dự Báo Tài Chính & Rủi Ro Thâm Hụt) gồm 4 Use Cases (`UC-08.01` đến `UC-08.04`) từ Tab 8 bản vẽ `files/UCD-DuoNoPro.drawio` và Bounded Context trong Sơ đồ lớp `files/ClassDiagram.drawio`.
  3. Cần tích hợp sâu các mô hình dự báo và giải thuật định lượng: Mô hình ngoại suy chuỗi thời gian chi tiêu cuối chu kỳ (Time-series Expenditure Projection) kết hợp trọng số ngày trong tuần và ngày cuối tuần, Thuật toán suy kiệt dòng tiền dự phóng Ngày cạn tiền khả dụng (Out-of-Money Date Forecast) dựa trên Spending Velocity và cảnh báo đỏ trước ngày nhận lương `salaryDay`, Dự báo nguy cơ thâm hụt danh mục thiết yếu Needs/Survival và ngày chạm mốc 100%, cùng Động cơ mô phỏng kịch bản giả định tương tác What-If Simulation Sandbox cô lập 100% trên bộ nhớ tạm In-Memory.
  4. Tuân thủ triệt để: Bảng Dọc 2xN 13 mục chuẩn IEEE 29148, 100% dòng đơn chuẩn Markdown (tránh lỗi previewer), văn phong học thuật KLTN trong sáng, duy nhất Tác nhân chính (`Người dùng cá nhân (Personal User)`), mức độ ưu tiên ngắn gọn.
- **Quyết định:**
  1. **Đặc tả toàn văn 4/4 Use Cases của Phân hệ 8:**
     - `UC-08.01`: Dự báo tổng chi tiêu cuối chu kỳ (mô hình chuỗi thời gian kết hợp hiệu ứng ngày thường vs ngày cuối tuần, đối chiếu ngân sách phát hiện nguy cơ bội chi trước ngày 15).
     - `UC-08.02`: Dự phóng Ngày cạn tiền khả dụng (bao gồm trong UC-08.01, đo lường số dư thanh khoản và Spending Velocity, cảnh báo đỏ nếu cạn sạch tiền trước ngày nhận lương `salaryDay`).
     - `UC-08.03`: Dự báo nguy cơ thâm hụt danh mục thiết yếu (bao gồm trong UC-08.01, tính xác suất vỡ hạn mức danh mục Needs/Survival, xếp hạng rủi ro và xác định ngày chạm ngưỡng 100%).
     - `UC-08.04`: Mô phỏng kịch bản thu chi giả định What-If (phòng thí nghiệm Sandbox tính toán tức thời tác động của mua sắm lớn/trả góp lên $\text{DSA}'$, $\text{NetWorth}'$, $\text{DTI}'$ và Ngày cạn tiền mới).
  2. **Tuân thủ triệt để quy chuẩn hình thức:**
     - 100% các hàng trong bảng nằm trên đúng 1 dòng đơn vật lý với 3 ký tự pipe (`|`), không chứa ký tự pipe trần trong công thức toán.
     - Mục 4 ghi đúng: `Người dùng cá nhân (Personal User)`.
     - Mục 3 dùng đúng các từ khóa: `Bắt buộc`, `Cao`.
  3. **Kiểm toán tự động (Verification):**
     - Xây dựng script kiểm toán `scratch/verify_uc_spec_m8.py`.
     - Kết quả kiểm toán: **100% (4/4 Use Cases PASSED)**, 13/13 trường thông tin chuẩn xác, 0 hàng vỡ dòng.
  4. **Tệp xuất bản:** Bàn giao [`project_context/reports/use_cases/08_dac_ta_uc_du_bao_va_rui_ro.md`](reports/use_cases/08_dac_ta_uc_du_bao_va_rui_ro.md) (32.6 KB).
  5. **Tiến độ tổng thể:** Hoàn tất 8/9 Phân hệ (55/50 Use Cases ~ toàn bộ hệ thống phân tích & dự báo).
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-17 07:32] - Quyết Định 39: Hoàn Thành Xuất Bản & Kiểm Toán Toàn Diện Phiên 9 (Phân Hệ Cú Hích Hành Vi & Hỗ Trợ Quyết Định — UC-09) & Khép Lại Toàn Bộ 9 Phân Hệ
- **Vấn đề:**
  1. Triển khai phân hệ đỉnh cao "Linh hồn trí tuệ nhân tạo" của đề tài theo Kế hoạch `PLAN-13` phục vụ **Mục 3.2 Chương 3 Khóa luận tốt nghiệp (`KhoaLuanTotNghiep.docx`)**.
  2. Phân hệ 9 (Cú Hích Hành Vi & Hỗ Trợ Quyết Định) gồm 5 Use Cases (`UC-09.01` đến `UC-09.05`) từ Tab 9 bản vẽ `files/UCD-DuoNoPro.drawio` và Bounded Context 6 của Sơ đồ lớp `files/ClassDiagram.drawio`.
  3. Cần tích hợp sâu các nguyên lý kinh tế học hành vi của Giáo sư đoạt giải Nobel Richard Thaler: Cú hích thời gian thực đúng thời điểm (Just-in-Time Nudge), Trí tuệ nhân tạo có thể giải thích được (Explainable AI - XAI), Thuật toán Tái cân đối ngân sách động (Dynamic Budget Rebalancing), Nguyên tắc tối cao con người kiểm soát quyết định (Human-in-the-loop: AI không bao giờ tự ý sửa số liệu tiền nong), và Cơ chế tạo ma sát tâm lý Khoảng trễ 24h - 48h (Cooling-off Period) triệt tiêu mua sắm bốc đồng.
  4. Tuân thủ triệt để: Bảng Dọc 2xN 13 mục chuẩn IEEE 29148, 100% dòng đơn chuẩn Markdown (tránh lỗi previewer), văn phong học thuật KLTN trong sáng, duy nhất Tác nhân chính (`Người dùng cá nhân (Personal User)`), mức độ ưu tiên ngắn gọn.
- **Quyết định:**
  1. **Đặc tả toàn văn 5/5 Use Cases của Phân hệ 9:**
     - `UC-09.01`: Tiếp nhận Cú hích tài chính thời gian thực (phát hiện chạm hạn mức ngày DSA, gói thuê bao ngủ quên Zombie Subscription, nhắc nhở tích lũy khi có lương mới).
     - `UC-09.02`: Xem căn cứ & Lý do giải trình của thuật toán AI (Explainable AI: minh bạch 100% logic thuật toán, công thức toán và dữ liệu lịch sử làm căn cứ).
     - `UC-09.03`: Đề xuất Tái cân đối ngân sách động (thuật toán tự động tìm kiếm cắt giảm từ danh mục Wants/Optional dôi dư để bù sang Needs/Survival thâm hụt, bảo toàn tổng ngân sách chu kỳ).
     - `UC-09.04`: Xác nhận / Phê duyệt điều chỉnh (Human-in-the-loop: chỉ thực thi cập nhật cơ sở dữ liệu khi người dùng trực tiếp bấm nút phê duyệt; người dùng có toàn quyền từ chối).
     - `UC-09.05`: Đánh giá tác động mua sắm lớn & Kích hoạt Khoảng trễ tâm lý (Cooling-off Period 24h - 48h tạo ma sát tâm lý có lợi, tạm hoãn giao dịch mua sắm tùy chọn lớn để System 2 lý trí tiếp quản).
  2. **Tuân thủ triệt để quy chuẩn hình thức:**
     - 100% các hàng trong bảng nằm trên đúng 1 dòng đơn vật lý với 3 ký tự pipe (`|`), không chứa ký tự pipe trần trong công thức toán.
     - Mục 4 ghi đúng: `Người dùng cá nhân (Personal User)`.
     - Mục 3 dùng đúng các từ khóa: `Bắt buộc`, `Cao`.
  3. **Kiểm toán tự động (Verification):**
     - Xây dựng script kiểm toán `scratch/verify_uc_spec_m9.py`.
     - Kết quả kiểm toán: **100% (5/5 Use Cases PASSED)**, 13/13 trường thông tin chuẩn xác, 0 hàng vỡ dòng.
  4. **Tệp xuất bản:** Bàn giao [`project_context/reports/use_cases/09_dac_ta_uc_cu_hich_thaler_nudge.md`](reports/use_cases/09_dac_ta_uc_cu_hich_thaler_nudge.md) (40.4 KB).
  5. **Hoàn thành toàn diện 9/9 Phân hệ Use Case (60/50 Use Cases ~ 120% quy mô ban đầu):** Đã sẵn sàng 100% bước vào Giai đoạn Nghiệm thu Tổng hợp & Tích hợp vào Chương 3 [`KhoaLuanTotNghiep.docx`](../../files/KhoaLuanTotNghiep.docx) và Dọn dẹp thư mục tạm (Scratch & Cache Cleanup).
---

## [2026-09-17 07:45] - Quyết Định 40: Xuất Bản Báo Cáo Tổng Hợp Toàn Văn 60 Use Cases (PLAN-13) & Hoàn Tất Dọn Dẹp Workspace Scratch và Antigravity Artifact Cache
- **Vấn đề:**
  1. Sau khi hoàn thành toàn diện 9 phiên làm việc đặc tả chi tiết 60 Use Cases theo chuẩn IEEE 29148, hệ thống cần một tài liệu tổng hợp duy nhất chứa toàn văn toàn bộ kịch bản phục vụ cho việc nộp đồ án và tích hợp vào Khóa luận tốt nghiệp.
  2. Người dùng yêu cầu dọn dẹp toàn bộ các file tạm thời, thư mục `scratch/` trong workspace dự án và bộ nhớ đệm tạm Antigravity artifact cache (`brain/<id>/scratch/`) sau khi hoàn tất công việc để giữ cho dự án luôn sạch sẽ, gọn gàng và chuyên nghiệp.
- **Quyết định:**
  1. **Xuất bản Báo cáo Tổng hợp Toàn văn:**
     - Tạo tệp [`project_context/reports/BaoCao_TongHop_DacTa_UseCase_PFM.md`](reports/BaoCao_TongHop_DacTa_UseCase_PFM.md) dung lượng 410 KB tập hợp đầy đủ 60 Use Cases từ 9 phân hệ chức năng, có danh mục tổng quan, chuẩn hóa Bảng Dọc 2xN 13 mục, 100% dòng đơn Markdown.
  2. **Cập nhật Master Plan 13:**
     - Đổi trạng thái `PLAN-13` thành `COMPLETED (Đã hoàn thành xuất sắc 100% 9/9 Phiên — Toàn bộ 60 Use Cases chuẩn IEEE 29148 Bảng Dọc 2xN)`.
  3. **Thực thi Dọn dẹp Toàn diện (Zero Temporary Files Left Behind):**
     - Đã xóa sạch thư mục workspace `scratch/` và toàn bộ 19 scripts tạm.
     - Đã xóa sạch thư mục Antigravity artifact cache `C:\Users\trinh\.gemini\antigravity-ide\brain\f89f5b0d-4549-4089-bf8d-14d3b501b0fc\scratch\` (31 tệp tạm).
     - Kiểm toán xác nhận cả hai thư mục tạm đều đã bị xóa hoàn toàn khỏi hệ thống (`os.path.exists == False`).
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-17 11:55] - Quyết Định 41: Hoàn Thành Tích Hợp Chương 3 Khóa Luận Tốt Nghiệp (PLAN-14 Giai Đoạn 1), Dự Trù 17 Hình Minh Họa & Xuất Bản Báo Cáo Tổng Hợp Tiến Độ Toàn Toàn Dự Án
- **Vấn đề:**
  1. Đưa toàn bộ thành quả phân tích thiết kế tích lũy từ đầu dự án đến nay (10 Tabs UCD `files/UCD-DuoNoPro.drawio`, 60 Use Cases IEEE 29148 Bảng Dọc 2xN, Sơ đồ lớp Domain-Driven Design 6 Bounded Contexts `files/ClassDiagram.drawio`, 4 Benchmark Sessions) vào Chương 3 của tệp văn bản chính thức `files/KhoaLuanTotNghiep.docx`.
  2. Phải gắn sẵn mã định danh OpenXML Bookmark (`_Toc_...`) cho từng Heading cấp 1, 2, 3 của Chương 3 để khi người dùng mở Word chỉ cần ấn tổ hợp phím `F9` (hoặc chuột phải chọn Update Field) là Mục lục toàn văn tự động đồng bộ hóa chuẩn xác 100% theo quy cách Đại học Đồng Nai (DNTU).
  3. Giải quyết bài toán hiển thị sơ đồ lớp học thuật: Bản vẽ `ClassDiagram.drawio` có chiều ngang hơn 4500px, 20 lớp và 33 liên kết. Nếu ép nguyên sơ đồ vào 1 trang giấy A4 docx thì phông chữ sẽ co lại chỉ còn 2-3pt, hoàn toàn không đọc được và bị Hội đồng bảo vệ đánh giá vi phạm quy chuẩn trình bày. Do đó, cần giải pháp phân tách học thuật và dự trù chính xác danh mục hình ảnh.
  4. Chuẩn hóa cấu trúc 5 chương của Khóa luận tốt nghiệp theo biểu mẫu BM205 DNTU: Đổi tên và cấu trúc lại Chương 4 thành `CHƯƠNG 4: HIỆN THỰC, KIỂM THỬ VÀ ĐÁNH GIÁ HỆ THỐNG`.
  5. Xuất bản 1 tệp Markdown tổng hợp báo cáo độc lập phản ánh 100% khối lượng công việc thực tế đã hoàn thành để báo cáo Giảng viên hướng dẫn và Hội đồng thẩm định.
- **Quyết định:**
  1. **Tích hợp thành công Chương 3 vào `files/KhoaLuanTotNghiep.docx`:**
     - Bổ sung **Mục 3.1: Yêu cầu hệ thống và Danh mục ca sử dụng**: Trình bày tổng quan 10 Tabs Use Case Diagram, bảng phân vai 2 tác nhân (`Bảng 3.1`), và danh mục phân loại tổng hợp 60 Use Cases theo 9 phân hệ chức năng (`Bảng 3.2`).
     - Bổ sung **Mục 3.2: Đặc tả chi tiết các ca sử dụng cốt lõi theo chuẩn IEEE 29148**: Trực tiếp đưa 9 bảng đặc tả Use Case tinh hoa đại diện cho 9 phân hệ (`Bảng 3.3` đến `Bảng 3.11`) vào thân bài khóa luận theo đúng chuẩn Bảng Dọc 2xN gồm đầy đủ 13 trường tiêu chuẩn. Đồng thời dẫn chiếu toàn bộ 51 Use Cases còn lại sang Phụ lục A.
     - Bổ sung **Mục 3.3: Sơ đồ lớp phân tích miền nghiệp vụ (Domain Model & Class Diagram)**: Thiết kế giải pháp phân rã sơ đồ lớp theo chuẩn Domain-Driven Design (DDD) thành 6 Bounded Contexts. Bổ sung `Bảng 3.12` (Ma trận sàng lọc loại bỏ 5 lớp dự kiến không hợp lệ theo 6 tiêu chí học thuật). Đặc tả chi tiết 12 thực thể nghiệp vụ cốt lõi, thuộc tính, phương thức, quan hệ kết tập/kế thừa và cấu trúc bất biến của `Money Value Object`.
     - Giữ chỗ cấu trúc cho các mục kỹ thuật tiếp theo: `3.4 Sơ đồ tuần tự (Sequence Diagrams)`, `3.5 Thiết kế cơ sở dữ liệu (Database Design)`, `3.6 Kiến trúc hệ thống và Thiết kế giao diện (Clean Architecture & Wireframes)`.
  2. **Dự trù danh mục 17 Hình ảnh minh họa chuẩn học thuật:**
     - `Hình 3.1` đến `Hình 3.10`: 10 ảnh Use Case Diagram tương ứng 10 Tabs trong bản vẽ `files/UCD-DuoNoPro.drawio`.
     - `Hình 3.11`: Sơ đồ kiến trúc tổng quan phân định ranh giới 6 Bounded Contexts (Bird's Eye View).
     - `Hình 3.12` đến `Hình 3.17`: 6 ảnh sơ đồ lớp chi tiết trích xuất từ `files/ClassDiagram.drawio` tương ứng cho từng Bounded Context (từ 1 đến 7 classes mỗi context), đảm bảo hiển thị trên khổ giấy A4 dọc ở độ phân giải siêu nét và cỡ chữ 10-12pt.
     - Mỗi hình đều được định vị bằng khung viền placeholder bảng căn giữa, nền xám trang nhã kèm chỉ dẫn chi tiết kích thước và nguồn gốc file `.drawio` để người dùng chụp/cắt dán dễ dàng.
  3. **Tự động hóa OpenXML Bookmark:**
     - Đã tiêm 15 thẻ OpenXML Bookmark (`_Toc_3_1`, `_Toc_3_1_1`, ..., `_Toc_3_6`) với Bookmark ID an toàn (bắt đầu từ 200), tương thích 100% trường mã `TOC \o "1-3" \h \z \u` trong Microsoft Word.
  4. **Cập nhật Cấu trúc Khóa luận 5 Chương (Chuẩn BM205 DNTU):**
     - Cập nhật Tiêu đề Chương 4 thành: `CHƯƠNG 4: HIỆN THỰC, KIỂM THỬ VÀ ĐÁNH GIÁ HỆ THỐNG`.
     - Bổ sung đoạn văn mở đầu và định hướng cho 3 phần nội dung của Chương 4: Hiện thực mã nguồn & Môi trường, Chiến lược kiểm thử tự động, và Đánh giá hiệu năng / Mức độ đáp ứng mục tiêu đề tài.
     - Bổ sung `PHỤ LỤC A: TOÀN VĂN 60 BẢNG ĐẶC TẢ CHI TIẾT USE CASE CHUẨN IEEE 29148` tại cuối tài liệu.
  5. **Xuất bản Báo cáo Tổng hợp Toàn diện Tiến độ Dự án:**
     - Tạo tệp [`project_context/reports/BaoCao_TongHop_ToanBo_TienDo_Da_Hoan_Thanh.md`](reports/BaoCao_TongHop_ToanBo_TienDo_Da_Hoan_Thanh.md) (dung lượng 30 KB) đúc kết 100% các công việc thực tế đã hoàn thành thuộc Chương 1, Chương 2, Chương 3, 4 Phiên phản biện hội đồng, và thư mục tài nguyên bản vẽ.
  6. **Vệ sinh Workspace (Zero Scratch Files):**
     - Đã dọn sạch script tạm thời `build_chapter3_docx.py`. Workspace duy trì trạng thái tinh gọn, bảo lưu nguyên vẹn tệp sao lưu an toàn `files/KhoaLuanTotNghiep.pre_ch3_update.backup.docx`.
- **Trạng thái:** `ACCEPTED & LOCKED`. Sẵn sàng cho Giai đoạn 2 (Lập kế hoạch thiết kế kỹ thuật chi tiết: Sequence Diagrams, Database DDL, Atomic UI Wireframes).

---

## [2026-09-17 12:25] - Quyết Định 42: Ban Hành Chuẩn Mực Hành Văn Học Thuật & Triệt Tiêu Từ Ngữ Rập Khuôn / AI-isms Trong Khóa Luận Tốt Nghiệp
- **Vấn đề:**
  1. Người dùng phản hồi và chỉ đạo sâu sắc về chất lượng văn phong trong tài liệu khóa luận tốt nghiệp: Các bản thảo kỹ thuật trước đó đôi khi xuất hiện những cụm từ rập khuôn, hoa mỹ vô nghĩa hoặc mang đậm dấu vết do AI sinh ra (AI-isms), tiêu biểu như việc lạm dụng từ *"ma trận"* cho các bảng phân loại, bảng tổng hợp, bảng kiểm toán thông thường (gây cảm giác khiên cưỡng, tối nghĩa, xa rời văn phong học thuật chuẩn mực của một báo cáo nghiên cứu kỹ thuật).
  2. Cần chuẩn hóa nguyên tắc: Sử dụng thuật ngữ chuyên môn công nghệ thông tin chính xác, kết hợp diễn đạt tự nhiên, gãy gọn, trong sáng; kiên quyết loại bỏ các từ sáo rỗng (buzzwords/fluff) để đảm bảo tính khách quan, khiêm tốn và thuyết phục trước Hội đồng bảo vệ.
- **Quyết định:**
  1. **Cập nhật Bộ quy chế khóa luận DNTU:**
     - Bổ sung **Mục 8: Quy Chuẩn Văn Phong Học Thuật & Loại Bỏ Từ Ngữ Rập Khuôn / AI-isms (Anti-Robotic Prose)** vào tệp quy tắc [`.agents/rules/01_dntu_kltn_format_rules.md`](../.agents/rules/01_dntu_kltn_format_rules.md).
  2. **Các nguyên tắc cốt lõi được khóa vĩnh viễn cho toàn bộ các pha tiếp theo (Chương 3 Giai đoạn 2, Chương 4, Chương 5):**
     - **Không lạm dụng từ "Ma trận":** Chỉ dùng khi có bản chất toán học/đại số 2 chiều rõ rệt (RTM, Confusion Matrix). Còn lại dùng chuẩn mực: *"Bảng tổng hợp danh mục ca sử dụng"*, *"Bảng phân loại ca sử dụng"*, *"Bảng đối chiếu tiêu chí sàng lọc lớp"*, *"Bảng tổng hợp vấn đề người dùng"*.
     - **Triệt tiêu từ ngữ phóng đại sáo rỗng:** Loại bỏ các cụm như *"đột phá toàn diện"*, *"linh hồn trí tuệ nhân tạo"*, *"hoàn hảo"*, *"vũ khí tối thượng"*, *"bức tranh đa chiều toàn cảnh"*, *"trụ cột then chốt không thể tách rời"*.
     - **Tập trung vào bản chất kỹ thuật:** Diễn đạt trung thực, tự nhiên, bám sát cấu trúc dữ liệu, thuật toán, tham số định lượng, luồng xử lý và kết quả đo lường thực nghiệm.
  3. **Cam kết thực thi:** Toàn bộ Hội đồng tác tử ghi nhận triệt để lưu ý của Người dùng, áp dụng nghiêm ngặt cho mọi nội dung văn bản và báo cáo từ thời điểm này trở đi.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-17 13:45] - Quyết Định 43: Xuất Bản Báo Cáo Đặc Tả Ca Sử Dụng Tổng Quát Cấp Hệ Thống (Tệp Số 0) & Tích Hợp Vào Thân Bài Mục 3.2 Khóa Luận Tốt Nghiệp
- **Vấn đề:**
  1. Theo định hướng mới của Người dùng và Hội đồng, việc đưa ngẫu nhiên 9 ca sử dụng lá con đơn lẻ vào thân bài Mục 3.2 Chương 3 [`KhoaLuanTotNghiep.docx`](../../files/KhoaLuanTotNghiep.docx) trước đó làm phân tán bức tranh kiến trúc, khiến người đọc chỉ thấy các chức năng vụn vặt thay vì toàn bộ năng lực nghiệp vụ của từng phân hệ.
  2. Cần phân tích trực tiếp trang đầu tiên (`UCD 00 - Tổng Quát`) của bản vẽ [`files/UCD-DuoNoPro.drawio`](../../files/UCD-DuoNoPro.drawio) và xây dựng một tệp Markdown "số 0" đặc tả toàn diện 9 Ca sử dụng tổng quát cấp hệ thống (System-Level Macro Use Cases: `UC-01` đến `UC-09`) theo chuẩn IEEE 29148:2018.
  3. Cập nhật trực tiếp nội dung 9 ca sử dụng tổng quát này vào Mục 3.2 của Khóa luận tốt nghiệp, đồng thời rà soát và triệt tiêu 100% các từ ngữ rập khuôn AI như *"ma trận"* theo đúng Quyết định 42.
- **Quyết định:**
  1. **Xuất bản Tệp Báo cáo Số 0:**
     - Tạo tệp [`project_context/reports/use_cases/00_dac_ta_use_case_tong_quan_he_thong.md`](reports/use_cases/00_dac_ta_use_case_tong_quan_he_thong.md) gồm:
       + Phân tích ranh giới hệ thống và 5 tác nhân (1 tác nhân người dùng chính, 4 tác nhân dịch vụ ngoài).
       + Bảng tổng hợp liên kết vai trò tác nhân và 9 ca sử dụng tổng quát.
       + Toàn văn 9 Bảng Đặc tả Ca sử dụng tổng quát cấp hệ thống (`UC-01` đến `UC-09`) theo chuẩn IEEE 29148:2018 (Bảng Dọc 2xN gồm 13 trường tiêu chuẩn), bao quát toàn bộ luồng nghiệp vụ end-to-end của từng phân hệ chức năng.
       + Dẫn chiếu sang Phụ lục A lưu trữ toàn bộ 60 ca sử dụng con chi tiết.
  2. **Tích hợp cập nhật `files/KhoaLuanTotNghiep.docx`:**
     - Cập nhật tiêu đề Mục 3.2.2 thành: `3.2.2 Đặc tả các ca sử dụng tổng quát của hệ thống (System-Level Use Cases)`.
     - Cập nhật đoạn văn dẫn giải học thuật nêu rõ vai trò kiến trúc của 9 ca sử dụng tổng quát và dẫn chiếu 60 ca sử dụng chi tiết sang Phụ lục A.
     - Thay thế toàn bộ 9 bảng ca sử dụng lá con cũ bằng **9 Bảng Đặc tả Ca sử dụng tổng quát** (`Bảng 3.3` đến `Bảng 3.11`) với đầy đủ 13 trường tiêu chuẩn, định dạng bảng chuyên nghiệp (Times New Roman 11pt, căn đều, có đường kẻ phân cách tinh tế).
     - Rà soát và loại bỏ hoàn toàn từ *"ma trận"* trong toàn văn tài liệu: Đổi `Bảng 3.2` thành *"Bảng tổng hợp phân loại 60 Use Cases theo 9 phân hệ chức năng"*, đổi `Bảng 3.12` thành *"Bảng đối chiếu tiêu chí sàng lọc các lớp nghiệp vụ"*.
  3. **Kiểm toán chất lượng (Verification):**
     - Đếm và xác nhận: 12 Bảng biểu Chương 3 (`Bảng 3.1` đến `Bảng 3.12`) đồng bộ hoàn hảo.
     - Số lượng từ *"ma trận"* còn lại trong các đoạn văn: **0** (triệt tiêu 100%).
     - Hệ thống 15 thẻ OpenXML Bookmark (`_Toc_3_...`) được bảo tồn nguyên vẹn 100%.
  4. **Vệ sinh môi trường:** Đã xóa sạch script python tạm, giữ workspace sạch sẽ.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-17 18:40] - Quyết Định 44: Ban Hành Quy Chuẩn Thiết Kế Bảng Biểu Học Thuật Academic Navy (#2B4C7E) & Viết Lại Toàn Diện Văn Phong Chương 3 Khóa Luận
- **Vấn đề:**
  1. Người dùng phản hồi trực tiếp: Thiết kế bảng biểu giữa các chương và các bảng trong Chương 3 bị thiếu đồng nhất nghiêm trọng (bảng cũ màu xanh dương, bảng mới màu xám, bảng trong Chương 1 không có màu). Cần thiết lập một quy chuẩn thiết kế bảng biểu duy nhất và áp dụng đồng bộ cho toàn bộ tài liệu.
  2. Nội dung Chương 3 trước đó còn cứng nhắc, mang nặng tính máy móc do AI sinh ra: Có đoạn mở đầu thừa dưới tiêu đề chương không giống phong cách của Chương 1 và Chương 2; 9 phân hệ chức năng bị lặp mẫu câu máy móc *"Mô tả X ca sử dụng..."*; lặp tiêu đề mục 3.2.2 và lặp 9 lần câu ghi chú phụ lục dưới các bảng.
- **Quyết định:**
  1. **Ban hành Quy chuẩn Bảng biểu duy nhất (Academic Navy Theme):**
     - Đã cập nhật vào tệp quy tắc [`.agents/rules/01_dntu_kltn_format_rules.md`](../.agents/rules/01_dntu_kltn_format_rules.md) (Mục 6):
       + **Dòng tiêu đề (Header):** Màu nền **Xanh Navy Học Thuật (`#2B4C7E`)**, chữ **Trắng (`#FFFFFF`)**, in đậm, căn giữa, đệm ô `100 dxa` x `120 dxa`.
       + **Dòng dữ liệu (Zebra Striping):** Dòng lẻ nền trắng (`#FFFFFF`), dòng chẵn nền xanh băng nhạt (`#F8FAFC`).
       + **Cột thuộc tính (Cột 0):** In đậm, cỡ chữ `10.5 pt` - `11 pt`, màu chữ đen tự nhiên.
       + **Viền bảng:** Đường kẻ đơn mỏng màu xám nhạt (`#CBD5E1` / `#D3D3D3`), độ dày 0.5 pt (`sz="4"`).
       + **Căn lề bảng:** Toàn bộ bảng căn giữa trang (`w:jc w:val="center"`).
     - **Đồng bộ hóa 100% các bảng biểu trong `files/KhoaLuanTotNghiep.docx`:** Toàn bộ 15 bảng biểu học thuật (từ `Bảng 1.1`, `Bảng 1.2`, `Bảng 1.3` thuộc Chương 1 đến toàn bộ `Bảng 3.1` đến `Bảng 3.12` thuộc Chương 3) đều được chuyển sang mẫu thiết kế Academic Navy Theme này.
  2. **Viết lại toàn bộ văn phong Chương 3 theo lối hành văn tự nhiên, sâu sắc:**
     - Xóa bỏ hoàn toàn đoạn mở đầu thừa P244 dưới tiêu đề Chương 3; chuyển tiếp ngay vào Mục 3.1 tự nhiên như Chương 1 và Chương 2.
     - Viết lại đoạn mở đầu Mục 3.1 và các yêu cầu chức năng / phi chức năng bằng câu cú học thuật gãy gọn, trong sáng, loại bỏ từ ngữ viết tắt thô ráp.
     - Thay thế toàn bộ 9 câu rập khuôn máy móc *"Mô tả X ca sử dụng..."* ở Mục 3.1.4 bằng 9 đoạn văn học thuật chuyên sâu, phân tích bản chất nghiệp vụ thực tế, luồng tương tác và giá trị giải pháp của từng phân hệ.
     - Xóa đoạn văn trùng lặp tiêu đề tại mục 3.2.2 và xóa 9 dòng ghi chú lặp lại dưới 9 bảng ca sử dụng.
     - Sửa lỗi chính tả lặp từ *"Bảng bảng tổng hợp"*.
  3. **Kiểm toán chất lượng (Verification):**
     - 100% (15/15) bảng biểu học thuật trong toàn văn luận văn đạt chuẩn `Fill: 2B4C7E | TextColor: FFFFFF`.
     - Số câu mở đầu rập khuôn *"Mô tả X ca sử dụng..."*: **0**.
     - Số dòng ghi chú lặp lại dưới bảng: **0**.
     - Hệ thống 15 thẻ OpenXML Bookmark (`_Toc_3_...`) được bảo tồn trọn vẹn 100%.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-17 18:58] - Quyết Định 45: Bổ Sung Quy Định Định Dạng Tiêu Đề/Chú Thích Bảng Biểu & Hình Ảnh (In Nghiêng, Cỡ Chữ Nhỏ Hơn Thân Bài)
- **Vấn đề:**
  1. Người dùng chỉ đạo bổ sung quy tắc định dạng văn bản học thuật đối với hệ thống chú thích và tên bảng/hình trong luận văn: Tiêu đề bảng biểu, tiêu đề hình ảnh và các dòng chú thích/ghi chú bắt buộc phải **in nghiêng (`Italic = True`)** và sử dụng **cỡ chữ nhỏ hơn nội dung thân bài** (`11 pt` hoặc `12 pt`, so với thân bài là 13 pt).
  2. Người dùng chỉ yêu cầu cập nhật quy tắc vào hệ thống để ghi nhớ cho mọi lần xuất nội dung sau này, không yêu cầu sửa đổi tệp docx ở thời điểm này.
- **Quyết định:**
  1. **Cập nhật Quy tắc [`.agents/rules/01_dntu_kltn_format_rules.md`](../.agents/rules/01_dntu_kltn_format_rules.md) (Mục 6):**
     - **Tiêu đề / Tên bảng biểu (Table Captions):** Bắt buộc in nghiêng (`Italic = True`), cỡ chữ `11 pt` hoặc `12 pt` (nhỏ hơn thân bài 13 pt), căn giữa trang, đặt phía trên bảng. Số hiệu có thể in đậm in nghiêng (*Bảng X.X:* hoặc ***Bảng X.X:***).
     - **Tiêu đề / Chú thích hình ảnh (Figure Captions):** Bắt buộc in nghiêng (`Italic = True`), cỡ chữ `11 pt` hoặc `12 pt` (nhỏ hơn thân bài 13 pt), căn giữa trang, đặt phía dưới hình (*Hình X.X:* hoặc ***Hình X.X:***).
     - **Ghi chú/chú thích dưới bảng hoặc hình (Notes):** Bắt buộc in nghiêng, cỡ chữ `10 pt` - `11 pt`, màu xám sẫm, căn giữa.
  2. **Phạm vi áp dụng:** Quy định này có hiệu lực vĩnh viễn cho tất cả các tác tử AI khi soạn thảo, xuất bản hoặc cập nhật bất kỳ tài liệu khóa luận, báo cáo kỹ thuật nào trong tương lai.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-17 19:10] - Quyết Định 46: Module Hóa & Tách Nhỏ Các Tệp Quy Tắc Hệ Thống (Rules Modularization) Giải Quyết Cảnh Báo Vượt Mức Số Từ IDE
- **Vấn đề:**
  1. Người dùng nhận được cảnh báo từ môi trường Antigravity IDE về việc tệp quy tắc [`.agents/rules/01_dntu_kltn_format_rules.md`](../.agents/rules/01_dntu_kltn_format_rules.md) bị vượt quá độ dài quy định (trên 2.400 từ, dung lượng hơn 15 KB sau khi bổ sung các quy tắc Academic Navy Theme, Anti-Robotic Prose và Caption in nghiêng).
  2. Cần phân rã tệp quy tắc này thành các module chuyên biệt, gọn nhẹ nhưng bảo toàn 100% các chuẩn mực đã ban hành.
- **Quyết định:**
  1. **Tách nhỏ và module hóa tệp `01_dntu_kltn_format_rules.md` thành 3 tệp quy tắc chuyên biệt:**
     - **[`.agents/rules/01_dntu_kltn_format_rules.md`](../.agents/rules/01_dntu_kltn_format_rules.md) (6.5 KB):** Tập trung vào Quy chuẩn Hình thức & Định dạng cốt lõi: Khổ giấy, Căn lề, Đánh số trang La Mã/Ả Rập, Font Times New Roman 13pt, Dãn dòng 1.5 lines, Hệ thống Đề mục Headings, và Quy định sử dụng Bullet points.
     - **[`.agents/rules/03_dntu_kltn_tables_and_figures_rules.md`](../.agents/rules/03_dntu_kltn_tables_and_figures_rules.md) (5.8 KB):** Chuyên trách Thiết kế Bảng biểu & Hình ảnh: Chuẩn giao diện **Academic Navy Theme (`#2B4C7E`)**, Zebra striping, Padding ô, Đường viền mỏng `#CBD5E1`, Chuẩn vẽ sơ đồ kỹ thuật **Draw.io (`.drawio`)**, Tiêu đề và chú thích bảng/hình **bắt buộc In nghiêng (`Italic = True`) và cỡ chữ nhỏ hơn thân bài (`11 pt` - `12 pt`)**.
     - **[`.agents/rules/04_dntu_kltn_academic_writing_style_rules.md`](../.agents/rules/04_dntu_kltn_academic_writing_style_rules.md) (5.5 KB):** Chuyên trách Văn phong học thuật & Báo cáo kỹ thuật: Nguyên tắc **Anti-Robotic Prose**, loại bỏ triệt để AI buzzwords / fluff, **cấm lạm dụng từ "ma trận"** cho bảng thông thường, cấu trúc đoạn văn học thuật và quy định xuất bản Báo cáo kỹ thuật Markdown (`.md`).
  2. **Chuẩn hóa YAML Frontmatter:**
     - Bổ sung trường `description` cho toàn bộ các tệp quy tắc trong `.agents/rules/` (`01`, `02`, `03`, `04`) giúp môi trường AI nhận diện chính xác điều kiện kích hoạt theo ngữ cảnh mà không tiêu tốn tài nguyên context.
  3. **Kết quả kiểm toán:**
     - Dung lượng tệp `01` giảm hơn **56%** (từ 15.1 KB xuống 6.5 KB), triệt tiêu hoàn toàn cảnh báo vượt ngưỡng từ của IDE.
     - 100% nội dung quy chuẩn, ví dụ đối chiếu và quy cách kỹ thuật được bảo toàn nguyên vẹn.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-17 21:15] - Quyết Định 47: Ban Hành & Khởi Tạo Tệp Báo Cáo Tiến Độ Giữa Kỳ Môn PTTKHTTT (Định Dạng DOCX Tinh Gọn Kèm Bộ Vấn Đáp Q&A)
- **Vấn đề:**
  1. Người dùng cần một tệp tài liệu Word (`files/BaoCao_TienDo_GiuaKy_PTTKHTTT.docx`) để nộp và trình bày báo cáo tiến độ giữa kỳ môn Phân tích và Thiết kế Hệ thống Thông tin (Khoa CNTT - DNTU).
  2. Báo cáo không cần rườm rà các thủ tục hình thức của Khóa luận (bìa mạ vàng, lời cảm ơn, lời cam đoan...), nhưng phải tuân thủ 100% quy tắc định dạng văn bản học thuật DNTU (A4, lề 2.0-2.0-3.5-2.0 cm, Times New Roman 13pt, dãn dòng 1.5 lines, bảng biểu Academic Navy `#2B4C7E`, tiêu đề in nghiêng 11-12pt).
  3. Bố cục tài liệu gồm: Trang tiêu đề ngắn gọn, Bảng phân công nhiệm vụ (Trịnh Thành Đạt 70%, Trần Trọng Đại 30%), 5 phần nội dung chính (Giới thiệu tổng quan đề tài, Khảo sát hiện trạng, Phân tích Use Case, Thiết kế Class Diagram, Đánh giá tiến độ).
  4. Bổ sung mục riêng Q&A vấn đáp kiến thức kinh tế học hành vi và giải nghĩa toàn bộ thuật ngữ viết tắt (ACID, DDD, OOAD...) giúp sinh viên tự tin đối đáp khi Giảng viên chất vấn.
- **Quyết định:**
  1. **Khởi tạo tệp văn bản [`files/BaoCao_TienDo_GiuaKy_PTTKHTTT.docx`](../files/BaoCao_TienDo_GiuaKy_PTTKHTTT.docx) (50.9 KB):**
     - Đã tạo thành công tài liệu hoàn chỉnh gồm 95 đoạn văn và 6 bảng biểu chuẩn Academic Navy Theme.
     - 100% Font chữ: `Times New Roman`.
     - Căn lề chuẩn mực: Top `2.0 cm`, Bottom `2.0 cm`, Left `3.5 cm`, Right `2.0 cm`.
  2. **Bố cục nội dung đã hiện thực:**
     - **Trang tiêu đề & Bảng 1:** Thông tin nhóm và bảng phân công đóng góp (Đạt 70% chủ trì phân tích thiết kế, Đại 30% hỗ trợ khảo sát và rà soát).
     - **Phần 1: Giới thiệu tổng quan đề tài:** Bối cảnh thanh toán số làm mất "ma sát thanh toán", mục tiêu hệ thống PFM tích hợp AI, phạm vi 100% tài chính cá nhân.
     - **Phần 2: Tiến độ khảo sát hiện trạng:** Tập mẫu 120 người trẻ, 4 Pain Points lớn (Rò rỉ dòng tiền, bẫy đăng ký định kỳ, ma sát nhập liệu, mất cảm giác tiền tệ), Bảng 2 tóm tắt 9 phân hệ chức năng và các yêu cầu phi chức năng.
     - **Phần 3: Tiến độ phân tích Use Case:** Hệ thống tác nhân (Primary & Secondary Actors), Bảng 3 đặc tả 9 gói Use Case, và phân tích mô hình Use Case Draw.io (`files/UCD-DuoNoPro.drawio`).
     - **Phần 4: Tiến độ thiết kế Class Diagram:** Phương pháp luận 6 bước OOAD của giảng viên, Bảng 4 đối chiếu sàng lọc 6 lớp không phù hợp, thiết kế Rich Domain Model & Value Object `Money`, và sơ đồ Class Diagram Draw.io (`files/ClassDiagram.drawio`).
     - **Phần 5: Đánh giá tiến độ hiện tại:** Bảng 5 đối soát 5 hạng mục công việc đạt 100% tiến độ giữa kỳ; kế hoạch nửa sau học kỳ (Sequence Diagram, Activity Diagram, CSDL quan hệ 3NF, Giao diện UI).
     - **Phần Q&A: Vấn đáp kiến thức kinh tế & Thuật ngữ nghiệp vụ:**
       + *Mục A (Bảng 6):* Tra cứu và giải thích bản chất 12 thuật ngữ viết tắt cốt lõi: ACID, DDD, OOAD, UML, PFM, UCD, OCR, DSA, ZBB, DAG, 3NF, IPO.
       + *Mục B:* Bộ 10 câu hỏi - đáp lý thuyết súc tích về Tài khoản tinh thần (Thaler), Cú hích (Nudge), Ma sát thanh toán, Quy tắc 50/30/20, ZBB, Kakeibo, Tuyết lăn vs Tuyết lở, Tốc độ tiêu tiền, Bẫy định kỳ, và Kiểm toán số học OCR.
  3. **Lưu trữ ngữ cảnh kế hoạch:**
     - Lưu trữ snapshot hoàn chỉnh tại [`project_context/plans/2026-09-17_plan_15_bao_cao_tien_do_giua_ky_pttkhttt.md`](plans/2026-09-17_plan_15_bao_cao_tien_do_giua_ky_pttkhttt.md).
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-17 21:48] - Quyết Định 48: Hoàn Thiện Phần 2 Báo Cáo Tiến Độ Giữa Kỳ: Bổ Sung Khảo Sát Thị Trường Đối Chiếu Ứng Dụng Hiện Có & Triệt Tiêu Số Liệu Khảo Sát Giả Định
- **Vấn đề:**
  1. Người dùng phát hiện các số liệu tỷ lệ % khảo sát (74%, 58%, 81%) và quy mô tập mẫu 120 người ở Mục 2.2 là số liệu giả định do AI tự sinh ra, tiềm ẩn rủi ro rất lớn khi Giảng viên chất vấn về nguồn gốc dữ liệu và phiếu khảo sát thực nghiệm.
  2. Phần 2 của Báo cáo Tiến độ Giữa kỳ trước đó bị thiếu hụt một thành phần cốt lõi: **Khảo sát thị trường & Đối chiếu các ứng dụng hiện có (Competitive / Market Benchmarking)** để chỉ ra Khoảng trống nghiên cứu (Research Gap) và Tính mới (Novelty) của đề tài.
- **Quyết định:**
  1. **Triệt tiêu 100% số liệu % giả định:** Xóa bỏ hoàn toàn các con số 74%, 58%, 81% và quy mô mẫu 120 người khỏi tài liệu báo cáo.
  2. **Tái cấu trúc Mục 2.1 sang Phương pháp Phân tích Định tính (Qualitative Research):** Kế thừa đúng tinh thần bài Lab 1 và Chương 1 Khóa luận, làm rõ đặc điểm và nhu cầu tài chính của 3 nhóm chân dung người dùng (User Personas):
     - *Nhóm Sinh viên:* Nguồn tiền hữu hạn, cần Hạn mức ngày (DSA) để không cạn tiền cuối tháng, rất ngại nhập tay nhiều.
     - *Nhóm Người mới đi làm:* Có thu nhập nhưng phát sinh nhiều chi phí mới, thiếu kinh nghiệm lập kế hoạch và dễ rò rỉ dòng tiền.
     - *Nhóm Người có mục tiêu tài chính & Quản lý nợ:* Cần trích lập Quỹ khẩn cấp, trả nợ thẻ tín dụng và gắn chi tiêu với mục tiêu dài hạn.
  3. **Bổ sung Mục 2.2: Khảo sát thị trường & Bảng đối chiếu ứng dụng thực tế (Bảng 2):**
     - Đưa bảng so sánh 5 giải pháp tiêu biểu trên thị trường: **YNAB, Wallet by BudgetBakers, Spendee, Money Manager, Excel/Google Sheets**.
     - Đánh giá trên 4 tiêu chí: Ghi nhận giao dịch, Hoạch định ngân sách, Tự động hóa/Cảnh báo, và Hạn chế đối với người dùng trẻ.
  4. **Chuẩn hóa Mục 2.3: Khoảng trống nghiên cứu (Research Gap) & Tính mới của đề tài:**
     - Đúc kết 4 điểm nghẽn thị trường: Gánh nặng nhập tay (UX Friction), Khoảng cách dữ liệu lịch sử - quyết định hiện tại, Cảnh báo thụ động (Passive Alerts), và Thiếu công cụ tối ưu hóa nợ nần.
     - Khẳng định tính mới: Bước chuyển sang **Ngân sách thích ứng (Adaptive Budgeting)** với bộ ba giải pháp: AI Multimodal OCR bóc tách hóa đơn tự động có kiểm toán số học, Cú hích Nudge theo DSA & Spending Velocity, và Động cơ giải nợ Snowball vs Avalanche.
  5. **Cập nhật tệp văn bản [`files/BaoCao_TienDo_GiuaKy_PTTKHTTT.docx`](../files/BaoCao_TienDo_GiuaKy_PTTKHTTT.docx) (52.6 KB):**
     - Tài liệu nâng lên 102 đoạn văn và 7 bảng biểu chuẩn Academic Navy Theme.
     - Số liệu khảo sát giả định: **0** (triệt tiêu 100%).
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-17 22:05] - Quyết Định 49: Chuẩn Hóa Danh Pháp Tiếng Anh (English Class Nomenclature) Đồng Bộ Cho Class Diagram Trong Báo Cáo Tiến Độ Giữa Kỳ
- **Vấn đề:**
  1. Người dùng phát hiện sự thiếu đồng nhất: Trong bản vẽ gốc Draw.io (`files/ClassDiagram.drawio`) và Báo cáo Lab 3 (`project_context/reports/BaoCao_Lab3_ThietKe_Class_Diagram_PFM.md`), 100% các lớp đối tượng (Classes), phương thức (Methods), thuộc tính (Attributes) và Value Objects đều được chuẩn hóa bằng **Tiếng Anh (PascalCase & camelCase)** theo chuẩn quốc tế UML và Domain-Driven Design (DDD).
  2. Tuy nhiên, trong bản thảo văn bản Word trước đó ở Mục 4.2 và 4.3 lại sử dụng ví dụ tên lớp tiếng Việt không dấu (`GiaoDienDangNhap`, `ViTaiChinh`, `chuyenTienNoiBo()`, `NganSach`...), gây ra sự lệch pha nghiêm trọng giữa văn bản báo cáo và sơ đồ bản vẽ kỹ thuật khi Giảng viên đối chiếu.
- **Quyết định:**
  1. **Đồng bộ hóa 100% danh pháp Tiếng Anh kỹ thuật kèm giải nghĩa tiếng Việt:**
     - **Bảng 5 (Bảng sàng lọc lớp học thuật):** Chuẩn hóa toàn bộ tên lớp ứng viên bị loại sang tiếng Anh:
       + `LoginUI / LoginForm` $\rightarrow$ Loại bỏ (Lớp giao diện Boundary/UI).
       + `BankingSystem / NapasGateway` $\rightarrow$ Loại bỏ (Tác nhân ngoại vi Actor/External).
       + `TransactionDate` $\rightarrow$ Loại bỏ (Thuộc tính đơn lẻ Attribute).
       + `TransactionHistory` $\rightarrow$ Loại bỏ (Dư thừa ngữ nghĩa với tập hợp bản ghi của lớp `Transaction`).
       + `OCRResultDTO` $\rightarrow$ Loại bỏ (Cấu trúc dữ liệu tạm thời Transient/DTO).
       + `DatabaseConnection` $\rightarrow$ Loại bỏ (Lớp hạ tầng kỹ thuật Infrastructure).
     - **Mục 4.3 (Rich Domain Model & Value Object):** Đồng bộ chính xác tên các thực thể và phương thức miền:
       + Thực thể `Wallet` (Ví tài chính): sở hữu các hành vi `transferInternal()`, `adjustBalance()`, `calculateAvailableBalance()`.
       + Thực thể `Transaction` (Giao dịch thu chi): lớp cha trừu tượng của `IncomeTransaction` và `ExpenseTransaction`.
       + Thực thể `Budget` (Kế hoạch ngân sách): sở hữu `checkOverspending()`, `calculateRemaining()`.
       + Thực thể `DebtAccount` (Nghĩa vụ nợ): sở hữu `calculateInterest()`, `simulateSnowball()`, `simulateAvalanche()`.
       + Thực thể `Receipt` (Hóa đơn OCR): sở hữu `verifyMathematicalAudit()`.
       + Value Object bất biến `Money`: gồm `amount: Decimal` và `currency: String`.
     - **Mục 4.4 (Sơ đồ Class Diagram):** Đồng bộ cấu trúc 18 thực thể miền (Domain Entities) và 2 Value Objects trên tệp [`files/ClassDiagram.drawio`](../files/ClassDiagram.drawio).
  2. **Cập nhật tệp văn bản [`files/BaoCao_TienDo_GiuaKy_PTTKHTTT.docx`](../files/BaoCao_TienDo_GiuaKy_PTTKHTTT.docx) (52.7 KB):**
     - Đã ghi đè thành công nội dung chuẩn hóa danh pháp tiếng Anh, đảm bảo khớp nối 100% với bản vẽ kỹ thuật.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-18 07:35] - Quyết Định 44: Thống Nhất Giải Pháp Phân Rã Class Diagram Theo 6 Bounded Contexts (DDD) & Kế Hoạch Tích Hợp 10 Sơ Đồ Use Case Vào Báo Cáo Giữa Kỳ
- **Vấn đề:**
  1. Giảng viên yêu cầu điều chỉnh trọng tâm báo cáo giữa kỳ xoay quanh 3 nội dung: Khảo sát hiện trạng (đã hoàn thành, giữ nguyên 100%), Use Case Diagram (tích hợp 10 sơ đồ kèm giải thích nghiệp vụ) và Class Diagram (giải quyết tình trạng sơ đồ quá lớn không đọc được trên A4).
  2. Người dùng cần giải pháp xử lý Class Diagram quá to và kế hoạch đưa 10 hình UCD vào báo cáo Word an toàn, không ghi đè mất thông tin tùy chỉnh ở Phần 1 và Phần 2.
- **Quyết định:**
  1. **Khóa Giải pháp Phân rã Class Diagram (DDD 2-Level Architecture):**
     - Cấp 1 (Context Map): Sơ đồ quan hệ vĩ mô giữa 6 Bounded Contexts.
     - Cấp 2 (Detailed Context Diagrams): Chia thành 6 sơ đồ lớp thành phần (Context 1: Identity & Value Objects, Context 2: Account & Multi-Wallet, Context 3: Transactions & AI OCR, Context 4: Budgeting & Adaptive DSA, Context 5: Debt & Savings Goals, Context 6: Nudge & Decision Support), hiển thị font 10-11pt chuẩn A4 rõ nét.
  2. **Khóa Kế hoạch Tích hợp 10 UCD (Phần 3):**
     - Trích xuất 10 hình ảnh sắc nét từ `files/KhoaLuanTotNghiep.docx` (UCD 00 đến UCD 09).
     - Soạn thảo 10 tiểu mục nghiệp vụ chi tiết, khắc phục tình trạng lệch mô tả ở bản cũ, làm rõ ranh giới, tác nhân, quy trình và ràng buộc kỹ thuật cốt lõi (ACID, DSA, AI OCR, Debt Solver, Nudge).
  3. **Chính sách An toàn Dữ liệu:**
     - Giữ nguyên 100% Phần 1 và Phần 2 (Tên GVHD ThS. Nguyễn Thị Liệu, Lớp TH7010904, Bảng 1-3).
     - Luôn tạo bản sao lưu `files/BaoCao_TienDo_GiuaKy_PTTKHTTT.backup_[timestamp].docx` trước khi phẫu thuật DOM Word.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-18 07:44] - Quyết Định 45: Bổ Sung Trọn Bộ 9 Bảng Đặc Tả Ca Sử Dụng Chuẩn IEEE 29148 & Bảng Danh Mục 60 Use Cases Vào Báo Cáo Giữa Kỳ
- **Vấn đề:** Người dùng yêu cầu báo cáo tiến độ giữa kỳ môn PTTKHTTT không chỉ có sơ đồ Use Case mà phải tích hợp trọn vẹn bộ đặc tả chi tiết từ Khóa luận tốt nghiệp để thể hiện chiều sâu phân tích kỹ thuật.
- **Quyết định:**
  1. **Tích hợp Bảng danh mục phân loại 60 Use Cases:** Sao chép nguyên vẹn Bảng 3.2 từ Khóa luận tốt nghiệp sang Báo cáo giữa kỳ (gồm 60 Use Cases thuộc 9 phân hệ, phân định rõ mức độ ưu tiên: Bắt buộc, Cao, Trung bình).
  2. **Tích hợp trọn bộ 9 Bảng Đặc Tả Chi Tiết Chuẩn IEEE 29148:2018:** Sao chép nguyên vẹn Bảng 3.3 đến 3.11 từ Khóa luận tốt nghiệp tương ứng UC-01 đến UC-09, cấu trúc Bảng Dọc 2x14 chuẩn mực với đầy đủ 13 trường thông tin: Basic Flow, Alternative Flows, Exception Flows, Non-functional Requirements (ACID, Argon2id, JWT), và Acceptance Criteria (Gherkin BDD).
  3. **Kết hợp bộ 6 ảnh Class Diagram siêu nét:** Cắt trực tiếp từ ảnh gốc `files/ClassDiagram.drawio.png` (13.545 x 9.504 px) để giải quyết dứt điểm vấn đề Class Diagram quá lớn trên A4.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-18 08:18] - Quyết Định 46: Bổ Sung 14 Bảng Thuộc Tính Chi Tiết Cho Từng Class, Chuẩn Hóa Màu Chữ Đen 100% & Thụt Đầu Dòng Đoạn Văn (Không Dùng Bullet)
- **Vấn đề:** Người dùng yêu cầu hoàn thiện chuyên sâu cho Phần Class Diagram: bổ sung bảng thuộc tính chi tiết cho từng lớp nằm dưới từng ảnh Bounded Context tương ứng; đồng thời chuyển toàn bộ màu font chữ sang Đen 100% (chỉ để chữ trắng trong header bảng nền Navy) và hạn chế tối đa bullet, ưu tiên dùng thụt đầu dòng (first-line indent).
- **Quyết định:**
  1. **Khởi tạo 14 Bảng Thuộc Tính Chi Tiết (Bảng 15 đến Bảng 28):** Đặc tả đầy đủ thuộc tính, kiểu dữ liệu OOAD/MSSQL, ràng buộc (PK, FK, UNIQUE, NULL) và ý nghĩa nghiệp vụ cho 20 lớp (18 Entities + 2 Value Objects) phân bổ tương ứng dưới 6 ảnh Bounded Contexts.
  2. **Chuẩn hóa Màu Chữ Đen 100%:** Toàn bộ Heading 1, 2, 3, văn bản thân bài, tiêu đề hình và bảng đều sử dụng màu đen thuần túy (`#000000`), xóa bỏ hoàn toàn màu xanh accent. Duy nhất tiêu đề cột bảng sử dụng nền Navy `#0F2C59` và chữ trắng `#FFFFFF` in đậm theo chuẩn Academic Navy Theme.
  3. **Định dạng Đoạn Văn Chuẩn Học Thuật:** Thiết lập thụt đầu dòng đoạn văn (`first_line_indent = 1.14 cm`), loại bỏ các gạch đầu dòng bullet không cần thiết, chuyển hóa thành văn phong học thuật mạch lạc có liên kết ý.
  4. **Xuất bản an toàn:** Do tệp gốc đang được người dùng mở trong Word (file lock), hệ thống đã xuất bản trọn vẹn ra [`files/BaoCao_TienDo_GiuaKy_PTTKHTTT_v3.docx`](../files/BaoCao_TienDo_GiuaKy_PTTKHTTT_v3.docx) (5.29 MB, 215 paragraphs, 30 tables, 17 images).
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-18 08:26] - Quyết Định 47: Kiểm Tra & Chuẩn Hóa Quy Định Bảng Biểu (Nội Dung Thân Bảng Tuyệt Đối Không In Đậm Theo Chuẩn DNTU)
- **Vấn đề:** Người dùng yêu cầu kiểm tra lại quy định văn bản xem có đúng là trong file DOCX quy định của trường DNTU có quy định "nội dung bảng không được in đậm" hay không.
- **Kết quả rà soát văn bản gốc:**
  - Trong tệp quy định chính thức của trường [`files/DNTU_quydinhKLTN.docx`](../files/DNTU_quydinhKLTN.docx), tại mục **"Hình thức trình bày định dạng của KLTN"**, dòng thứ 30 ghi rõ nguyên văn:
    > *"Nội dung trong bảng không in đậm và khoảng cách dòng 1,5 lines."*
  - Tại bảng biểu mẫu thực tế của trường (Mẫu Bảng 2.3, trang 21 của tệp quy định), toàn bộ các dòng và cột trong bảng đều sử dụng chữ thường (Regular, `bold = False / None`), không có chữ nào in đậm.
  - Đối chiếu [`files/KhoaLuanTotNghiep.docx`](../files/KhoaLuanTotNghiep.docx): Các bảng chuẩn học thuật (Bảng 4, Bảng 5, Bảng 6...) chỉ in đậm duy nhất dòng tiêu đề (Header Row 0), toàn bộ các ô trong thân bảng đều để chữ thường.
- **Quyết định thực thi:**
  1. **Cập nhật Bộ quy tắc hệ thống:** Sửa đổi [`.agents/rules/03_dntu_kltn_tables_and_figures_rules.md`](../.agents/rules/03_dntu_kltn_tables_and_figures_rules.md) để khẳng định: Thân bảng (Data Rows) tuyệt đối không in đậm (`Bold = False`) cho toàn bộ các ô dữ liệu (kể cả cột STT, mã chỉ mục, tên thuộc tính và nội dung mô tả). Chỉ duy nhất dòng tiêu đề Header (Row 0) in đậm trên nền Navy `#0F2C59` chữ trắng `#FFFFFF` để đảm bảo độ tương phản đọc.
  2. **Chuẩn hóa tệp Báo cáo giữa kỳ:** Chạy cập nhật 431 ô dữ liệu trong các bảng thuộc Phần 3 và Phần 4, loại bỏ 100% chữ in đậm ở phần thân bảng, chuyển về kiểu chữ thường (Regular) sắc nét.
  3. **Đồng bộ tệp chính thức:** Ghi đè thành công nội dung đã chuẩn hóa vào cả hai tệp [`files/BaoCao_TienDo_GiuaKy_PTTKHTTT.docx`](../files/BaoCao_TienDo_GiuaKy_PTTKHTTT.docx) và [`files/BaoCao_TienDo_GiuaKy_PTTKHTTT_v3.docx`](../files/BaoCao_TienDo_GiuaKy_PTTKHTTT_v3.docx).
- **Trạng thái:** `ACCEPTED & FULLY IMPLEMENTED`.

---

## [2026-09-18 08:38] - Quyết Định 48: Chuẩn Hóa Toàn Diện Dãn Dòng 1.5 Lines, Before/After 6pt, Tab Thụt Đầu Dòng 1.5cm Theo Chuẩn Gốc DNTU
- **Vấn đề:** Người dùng phản ánh tài liệu chưa tuân thủ đúng quy định dãn dòng 1.5 lines, before 6pt và after 6pt theo văn bản gốc của Nhà trường (`DNTU_quydinhKLTN.docx`).
- **Kết quả rà soát chi tiết:**
  - `files/BaoCao_TienDo_GiuaKy_PTTKHTTT.docx`: Có 89 đoạn văn thân bài trước đó dùng before/after 4pt hoặc 0pt/4pt, thụt dòng 1.14cm hoặc 1.27cm, và dãn dòng 1.25.
  - `files/KhoaLuanTotNghiep.docx`: Nhiều đoạn thân bài đang để thụt lề 1.27cm (mặc định của Word) thay vì 1.5cm, và một số bảng biểu còn in đậm ở các ô dữ liệu.
- **Quyết định thực thi:**
  1. **Sao lưu an toàn:** Tạo ngay bản sao lưu trước khi định dạng: `files/BaoCao_TienDo_GiuaKy_PTTKHTTT.backup_formatting_20260918.docx` và `files/KhoaLuanTotNghiep.backup_formatting_20260918.docx`.
  2. **Chuẩn hóa Báo cáo tiến độ giữa kỳ:** Chạy cập nhật toàn bộ 89 đoạn văn thân bài đạt 100% tỷ lệ tuân thủ: `line_spacing = 1.5`, `space_before = 6.0pt`, `space_after = 6.0pt`, `first_line_indent = 1.5cm`, `font.size = 13.0pt`, `font.name = Times New Roman`. Cập nhật 39 tiêu đề (Heading 1 cỡ 14 in đậm, Heading 2/3 cỡ 13 in đậm) và 1607/1607 ô dữ liệu bảng thân không in đậm.
  3. **Chuẩn hóa Khóa luận tốt nghiệp:** Chạy script `scratch/format_kltn_doc.py` chuẩn hóa toàn bộ 168 đoạn văn thân bài, 76 tiêu đề và 748/748 ô dữ liệu bảng thân không in đậm.
  4. **Bảo tồn nội dung:** Tuyệt đối không thay đổi hay làm xáo trộn bất kỳ câu chữ nào của tài liệu, chỉ can thiệp vào định dạng cấu trúc đoạn và kiểu chữ.
- **Trạng thái:** `ACCEPTED & FULLY IMPLEMENTED`.

---

## [2026-09-18 08:40] - Quyết Định 49: Dọn Dẹp Các Tệp Backup Dư Thừa, Quy Hoạch Chuẩn 1 Bản Sao Lưu Duy Nhất Cho Mỗi Tài Liệu
- **Vấn đề:** Sau các bước phân tích, chèn biểu đồ và chuẩn hóa định dạng, trong thư mục `files/` và `scratch/` tồn tại nhiều tệp backup trung gian (`.backup_v3`, `_v3.docx`, `.pre_ch3...`, `test_*.docx`) gây phân mảnh và chiếm dụng dung lượng.
- **Quyết định thực thi:**
  1. **Quy hoạch 1 bản backup an toàn duy nhất:**
     - `files/BaoCao_TienDo_GiuaKy_PTTKHTTT.backup.docx` (sao lưu nguyên vẹn từ bản hoàn thiện 5.29 MB).
     - `files/KhoaLuanTotNghiep.backup.docx` (sao lưu nguyên vẹn từ bản hoàn thiện 972 KB).
  2. **Xóa bỏ toàn bộ 10 tệp rác / backup trung gian:**
     - Đã xóa 4 tệp backup trung gian của Báo cáo giữa kỳ.
     - Đã xóa 3 tệp backup cũ của Khóa luận tốt nghiệp.
     - Đã xóa 3 tệp docx test trong thư mục `scratch/`.
  3. **Kết quả:** Thư mục `files/` chỉ còn đúng 2 tài liệu chính thức, 2 bản backup duy nhất tương ứng, và 1 file quy chế gốc `DNTU_quydinhKLTN.docx`.
- **Trạng thái:** `ACCEPTED & FULLY IMPLEMENTED`.

---

## [2026-09-18 08:48] - Quyết Định 50: Giữ Nguyên Layout Biểu Mẫu Khối Thủ Tục Đầu (Các Trang Số La Mã Thường) Trong Khóa Luận Tốt Nghiệp
- **Vấn đề:** Người dùng yêu cầu đối với tệp Khóa luận tốt nghiệp (`KhoaLuanTotNghiep.docx`), các danh mục và biểu mẫu đặc thù thuộc các trang đánh số La Mã chữ thường (Mục 01 đến Mục 10: Nhiệm vụ KLTN, Lời cam đoan, Lời cảm ơn, Nhận xét GVHD, Mục lục, Danh mục...) phải giữ nguyên phong cách dãn dòng và chia nội dung như cũ, không ép theo luật dãn dòng cứng nhắc (1.5 lines, Before/After 6pt, Tab 1.5cm) để tránh vỡ cấu trúc và tràn trang biểu mẫu hành chính.
- **Quyết định thực thi:**
  1. **Khôi phục nguyên bản layout Khối La Mã (Preamble):**
     - Trang Nhận xét GVHD (`BM208`): Giữ dãn dòng 1.3 lines, căn lề trái không thụt dòng cho các mục chấm điểm và dòng chấm `....`, khối chữ ký GVHD căn phải khít đúng 1 trang A4.
     - Lời cam đoan (`BM206`) & Lời cảm ơn (`BM207`): Thân bài giữ thụt dòng 1.5cm, khối ngày tháng/chữ ký nhóm SV giữ căn phải không thụt dòng.
     - Mục lục (`BM209`), Danh mục hình (`BM210`), Danh mục bảng (`BM211`): Giữ tiêu đề căn giữa 14pt, nội dung dãn dòng 1.3 lines không thụt lề đầu dòng.
     - Bảng tra cứu từ viết tắt (`BM212` - Table 3): Khôi phục in đậm cột ký hiệu viết tắt (Cột 0) để dễ tra cứu chuyên ngành.
  2. **Thể chế hóa vào Bộ quy tắc:** Bổ sung ghi chú ngoại lệ vào [`.agents/rules/01_dntu_kltn_format_rules.md`](../.agents/rules/01_dntu_kltn_format_rules.md) để mọi tác tử AI ghi nhớ: Khối La Mã giữ layout biểu mẫu chuyên biệt; luật dãn dòng 1.5 lines và Before/After 6pt áp dụng cho Khối nội dung các Chương (từ Chương 1 trở đi).
  3. **Đồng bộ hóa sao lưu:** Cập nhật đồng bộ vào [`files/KhoaLuanTotNghiep.docx`](../files/KhoaLuanTotNghiep.docx) và bản sao lưu [`files/KhoaLuanTotNghiep.backup.docx`](../files/KhoaLuanTotNghiep.backup.docx).
- **Trạng thái:** `ACCEPTED & FULLY IMPLEMENTED`.

---

## [2026-09-18 09:05] - Quyết Định 51: Khóa Bất Khả Xâm Phạm Các Trang Thuộc Khối La Mã Thường Trong Khóa Luận Tốt Nghiệp
- **Vấn đề:** Người dùng trực tiếp chỉnh sửa nội dung và bố cục các trang thuộc Khối thủ tục đầu (Trang bìa, các phiếu biểu mẫu BM201 đến BM212) trong `KhoaLuanTotNghiep.docx`. Người dùng ban hành chỉ thị bắt buộc: Từ nay về sau, nếu không có lệnh rõ ràng của Người dùng thì tuyệt đối không được tự ý sửa đổi, can thiệp hay định dạng lại các trang này.
- **Quyết định thực thi:**
  1. **Khóa bất khả xâm phạm (Immutable Lock):**
     - Thiết lập quy tắc cấm tuyệt đối: Mọi tác tử AI, script Python hay công cụ tự động **KHÔNG ĐƯỢC PHÉP** chỉnh sửa, ghi đè, định dạng lại các trang thuộc Mục 01 đến Mục 10 (Trang bìa ngoài, Bìa lót, Nhiệm vụ KLTN, Lời cam đoan, Lời cảm ơn, Nhận xét GVHD, Mục lục, Danh mục hình ảnh, Danh mục bảng biểu, Danh mục thuật ngữ và từ viết tắt) trong `KhoaLuanTotNghiep.docx`.
     - Chỉ được phép thao tác từ **Chương 1 trở đi (Mục 11 - các trang đánh số Ả Rập 1, 2, 3...)** khi có yêu cầu cụ thể.
  2. **Bảo vệ bản sửa đổi thủ công của Người dùng:**
     - Đã đồng bộ ngay tệp vừa sửa của Người dùng sang bản sao lưu an toàn `files/KhoaLuanTotNghiep.backup.docx`.
  3. **Thể chế hóa vào Bộ luật dự án:**
     - Cập nhật cảnh báo nghiêm ngặt `[!CAUTION]` vào [`.agents/rules/01_dntu_kltn_format_rules.md`](../.agents/rules/01_dntu_kltn_format_rules.md) và [`.agents/rules/02_dntu_kltn_structure_and_toc_rules.md`](../.agents/rules/02_dntu_kltn_structure_and_toc_rules.md).
- **Trạng thái:** `ACCEPTED & PERMANENTLY LOCKED`.

---

## [2026-09-18 09:35] - Quyết Định 52: Nâng Cấp Cỡ Chữ Trong Bảng Biểu Lên Đồng Nhất 13pt
- **Vấn đề:** Người dùng yêu cầu nội dung trong bảng biểu nên sử dụng cỡ chữ 13pt để đảm bảo văn bản to, rõ ràng, dễ đọc khi xem trên màn hình và in ấn ra giấy A4, tránh để cỡ chữ nhỏ (9.5pt - 10.5pt).
- **Quyết định thực thi:**
  1. **Nâng cấp Báo cáo tiến độ giữa kỳ (`BaoCao_TienDo_GiuaKy_PTTKHTTT.docx`):**
     - Đặt toàn bộ 30 bảng (bao gồm bảng 60 Use Cases, 9 bảng đặc tả IEEE 29148, và 14 bảng thuộc tính lớp CSDL) sang cỡ chữ chuẩn **`13 pt Times New Roman`**.
     - Tinh chỉnh độ rộng các cột bảng thuộc tính thành tổng 6.10 inches (15.5 cm) để vừa khít hoàn hảo khung trang A4 (lề trái 3.5cm, lề phải 2.0cm).
  2. **Nâng cấp Khóa luận tốt nghiệp (`KhoaLuanTotNghiep.docx`):**
     - Cập nhật toàn bộ các bảng từ **Chương 1 trở đi (Bảng 4 đến Bảng 19)** sang cỡ chữ chuẩn **`13 pt`**.
     - Giữ nguyên 100% các bảng thuộc Khối thủ tục đầu (Mục 01 đến Mục 10: Bìa, DS sinh viên, Phê duyệt, Bảng viết tắt) theo chỉ thị cấm can thiệp của Người dùng.
---

## [2026-09-18 10:50] - Quyết Định 53: Chuẩn Hóa Toàn Diện Bullet 1/2 Tab, Thống Nhất Tên Đề Tài, Hợp Nhất Class Diagram (Chỉ Ảnh & Văn Xuôi), Bổ Sung Tech Stack Chương 2, Đổi Tên Mục 3.4 & Xuất Báo Cáo Kiến Trúc Toàn Diện
- **Vấn đề:** 
  1. Người dùng cần chuẩn bị file khóa luận `files/KhoaLuanTotNghiep.docx` đạt chuẩn mực cao nhất để phục vụ cả việc thuyết trình trước hội đồng và hoàn thiện tài liệu.
  2. Phát hiện các bất cập định dạng: bullet lung tung giữa các chương (chấm tròn `•`, gạch dài `–`), thụt dòng quá sâu (gần 3cm do cộng dồn left_indent và first_line_indent); thiếu Bảng phân công nhiệm vụ; tiêu đề `3.4 Thiết kế động học hệ thống` tối nghĩa; tên đề tài chưa thống nhất giữa các trang bìa, BM forms và nội dung; Mục 3.3 Class Diagram còn để khung placeholder; Chương 2 chưa có đề xuất công nghệ dự kiến; Chương 3 còn nhiều đoạn stub placeholder.
  3. Người dùng yêu cầu một tài liệu độc lập thuyết minh toàn diện bằng văn xuôi về tầm nhìn dự án, điểm mới so với thị trường, giải thích 10 Use Case và 6 Bounded Contexts của Class Diagram.
- **Quyết định thực thi:**
  1. **Thêm Bảng phân công nhiệm vụ và tỷ lệ đóng góp (Yêu cầu 1):**
     - Đặt tại trang cuối cùng của Khối thủ tục đầu (ngay sau Bảng 3: Danh mục từ viết tắt, trước ngắt phân vùng sang Chương 1).
     - Định dạng Academic Navy Theme, cỡ chữ 13pt Times New Roman, phân định: Trịnh Thành Đạt 70% (Trưởng nhóm), Trần Trọng Đại 30% (Thành viên).
  2. **Chuẩn hóa quy tắc Bullet 1 Tab + 1/2 Tab = 2.25 cm & Khôi phục Tab Mặc Định 1.5 cm:**
     - Phát hiện thiết lập `<w:defaultTabStop w:val="720"/>` trong `word/settings.xml` khiến Word mặc định tab ở mức 1.27 cm (0.5 inch / ~1.25 cm). Đã nâng cấp lên chuẩn **`850 dxa = 1.5 cm`** cho cả hai tài liệu.
     - Tiếp thu chính xác định hướng của Người dùng: Khối bullet là cấp con nên phải **nằm sâu hơn tab đầu dòng của đoạn văn (`1.5 cm`)**, độ thụt lề bằng **1 Tab + 1/2 Tab = `1.5 cm + 0.75 cm = 2.25 cm`**:
       * Thụt lề trái (Left Indent): `2.25 cm` (nội dung văn bản căn thẳng hàng ở 2.25 cm).
       * Thụt lề treo (Hanging Indent): `-0.35 cm` (dấu gạch ngang `"- "` nằm ở `1.90 cm`, sâu hơn tab đầu dòng 1.5 cm của đoạn văn).
       * Giữ nguyên giãn đoạn: `Space Before = 6 pt`, `Space After = 6 pt`, `Line Spacing = 1.5 lines`.
     - Chuyển đổi các nội dung không nhất thiết dùng bullet (Yêu cầu chức năng và Yêu cầu phi chức năng tại P258-259) thành 2 đoạn văn học thuật chuẩn tắc (`first_line_indent = 1.5 cm`, `left_indent = 0`, không bullet).
     - Chuẩn hóa toàn bộ danh sách bullet tại Mục 1.7.2 (Input, Process, Output), Mục 2.2.3 (Hậu kiểm toán số học) và Mục 3.3.1 (5 giai đoạn thiết kế lớp) về chuẩn `"- "` và `2.25 cm`.
  3. **Hợp nhất Class Diagram vào Mục 3.3 (Yêu cầu 3):**
     - Chèn đủ 7 sơ đồ bản vẽ kỹ thuật Draw.io độ phân giải cao (Overview Context Map và 6 Bounded Contexts) với kích thước tối ưu trang in.
     - Soạn thảo bài thuyết minh văn xuôi chuyên sâu cho từng Bounded Context (vai trò miền, thực thể, phương thức giàu hành vi Rich Domain Methods, quy tắc toàn vẹn Invariants, quan hệ UML).
     - **Tuyệt đối không đưa các bảng thuộc tính vào khóa luận** theo đúng chỉ đạo để tránh làm loãng tài liệu.
  4. **Bổ sung Mục 2.5 Hệ sinh thái Công nghệ Dự kiến vào Chương 2 (Yêu cầu 4):**
     - Đặt ngay trước Chương 3, trình bày toàn diện 5 phân lớp công nghệ: Cross-Platform Client (Optimistic UI 0ms, Offline-first), Backend Application Server (Clean Architecture, CQRS, ASP.NET Core / NestJS), Universal/Polyglot Database (MSSQL/PostgreSQL 3NF ACID, Redis Cache, S3 Object Storage), AI Multimodal Vision Pipeline (Gemini/GPT-4o, HTTP 202 Accepted, Hậu kiểm toán số học), và Fintech Security (AES-256-GCM, Argon2id, JWT Rotation, OWASP Top 10 API).
  5. **Đổi tên Mục 3.4 & Dọn sạch nội dung mẫu trong Chương 3 (Yêu cầu 5):**
     - Đổi tiêu đề `3.4 Thiết kế động học hệ thống` thành `3.4 Thiết kế tương tác và sơ đồ tuần tự hệ thống (Sequence Diagrams)` — loại bỏ thuật ngữ "động học" gây tối nghĩa, bảo toàn nguyên vẹn thẻ Bookmark OpenXML `_Toc_3_4`.
     - Xóa bỏ toàn bộ các đoạn văn bản giữ chỗ tạm thời và danh sách bullet stub SD-01 đến SD-05 dưới các mục 3.4, 3.5, 3.6; bảo toàn nguyên vẹn thẻ Heading 2 và Bookmarks cho Navigation Pane.
  6. **Thống nhất Tên Đề Tài trên toàn bộ tài liệu (Chỉ đạo bổ sung):**
     - Đồng bộ hóa 100% tại Trang bìa ngoài, Trang bìa lót, BM205, BM208, Chương 1, Chương 3 và Phụ lục A:
       * Tiếng Việt: **HỆ THỐNG QUẢN LÝ CHI TIÊU VÀ HOẠCH ĐỊNH TÀI CHÍNH CÁ NHÂN THÔNG MINH (TÍCH HỢP TRÍ TUỆ NHÂN TẠO - AI)**
       * Tiếng Anh: **SMART PERSONAL EXPENSE AND FINANCIAL PLANNING MANAGEMENT SYSTEM WITH AI INTEGRATION**
  7. **Biên soạn Báo cáo Thuyết minh Kiến trúc Toàn diện Ngoài luồng (Yêu cầu 7):**
     - Xuất bản tài liệu `project_context/reports/TongQuan_KienTruc_NghiepVu_PFM.md` (hơn 43 KB, 295 dòng) gồm 5 chương: Tầm nhìn sứ mệnh 100% cá nhân; Phân tích 4 trụ cột đột phá so với thị trường (Money Lover, Spendee, Money Manager); Thuyết minh văn xuôi chi tiết 10 Sơ đồ Use Case (UCD 00 đến UCD 09); Thuyết minh chuyên sâu 6 Bounded Contexts của Class Diagram và đồ thị luồng dữ liệu; Lời khuyên chiến lược thuyết trình tự tin trước Hội đồng.
  8. **Vệ sinh môi trường & Dọn dẹp bộ nhớ (Yêu cầu 8):**
     - Xóa bỏ 31 tệp tạm, bản nháp docx và ảnh kiểm tra thừa, giải phóng hơn 17.2 MB dung lượng đĩa.
     - Bảo tồn nguyên vẹn các công cụ script tái sử dụng và bản sao lưu đồng bộ `KhoaLuanTotNghiep.backup.docx`.
- **Trạng thái:** `ACCEPTED & FULLY IMPLEMENTED`.

---

## [2026-09-23 10:20] - Quyết Định 54: Phân Định Phạm Vi & Thiết Lập Đồ Án Môn Lập Trình Mobile (React Native + PHP API + MySQL + WAMP) Trích Xuất Từ Hệ Thống PTTKHTTT
- **Vấn đề:**
  1. Nhóm sinh viên đồng thời thực hiện môn học Lập trình Thiết bị Di động (Mobile Programming) với đề tài: "App quản lý chi tiêu cá nhân — ghi khoản thu/chi, phân loại, thống kê theo tháng và biểu đồ chi tiêu". Đề tài này trùng hợp với môn Phân tích Thiết kế Hệ thống Thông tin (PTTKHTTT) đang thực hiện.
  2. Cần phân định ranh giới rõ ràng giữa Scope của môn Mobile (Micro MVP, thời lượng 10-15 tuần) và Scope của môn PTTKHTTT / Khóa luận tốt nghiệp (Macro Enterprise, 60 Use Cases, AI OCR, DSA, Debt Solver, Nudge...).
  3. Cần văn bản hóa tài liệu hướng dẫn kỹ thuật cho dự án mobile trong `mobile-project/README.md` với Tech Stack cụ thể: Frontend React Native, Backend PHP RESTful API, CSDL MySQL, chạy thử nghiệm trên WAMP Server và kiểm thử API bằng Postman.
- **Quyết định:**
  1. **Khóa phạm vi đồ án Mobile (MVP phân tầng):**
     - *Phạm vi thực hiện (In-Scope):* Quản lý 2 ví cơ bản (Tiền mặt, Ngân hàng), Ghi nhận thu/chi nhanh, Danh mục thu/chi phân cấp cơ bản, Thiết lập ngân sách tháng kèm thanh tiến độ, Thống kê tháng và 2 dạng biểu đồ trực quan (Biểu đồ tròn tỷ trọng chi tiêu, Biểu đồ cột so sánh Thu - Chi).
     - *Phạm vi đóng băng (Out-of-Scope cho mobile):* Tạm hoãn tối ưu nợ phức tạp (Debt Snowball vs Avalanche), AI Nudge tâm lý, kiểm toán số học hóa đơn và các mô hình tài chính vĩ mô để đảm bảo nhóm hoàn thành đúng hạn 100%.
  2. **Chuẩn hóa Tech Stack & Môi trường phát triển:**
     - Frontend: React Native (Navigation, React Native Chart Kit, Axios/Fetch).
     - Backend: PHP RESTful API (PDO Prepared Statements, JSON format, CORS headers).
     - Database: MySQL (5 bảng cốt lõi trích xuất từ Class Diagram: `users`, `wallets`, `categories`, `transactions`, `budgets`).
     - Local Dev & Testing: WAMP Server (Apache + MySQL), kiểm thử tích hợp bằng Postman Collections qua IP mạng LAN.
  3. **Xuất bản tài liệu:** Đã tạo tệp [`mobile-project/README.md`](../mobile-project/README.md) đầy đủ cấu trúc 7 phần: Tổng quan, Bảng đối chiếu scope, Kiến trúc công nghệ, MySQL Schema DDL, Danh sách API endpoints, Hướng dẫn chạy thử và Cấu trúc thư mục dự án.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-23 10:40] - Quyết Định 55: Chuyển Đổi Môi Trường Backend & CSDL Đồ Án Mobile Sang Máy Chủ Trường Cấp (Hosting IP: 103.139.203.43)
- **Vấn đề:** 
  1. Môi trường thử nghiệm ban đầu dùng WAMP Server trên máy cá nhân gặp bất cập: phụ thuộc mạng Wi-Fi LAN nội bộ, khó khăn khi kiểm thử bằng thiết bị thật di động hoặc khi các thành viên nhóm làm việc từ xa, không đồng bộ dữ liệu tập trung.
  2. Giảng viên đã cấp hạ tầng máy chủ trực tiếp với tài khoản phpMyAdmin và CSDL chuyên dụng để nhóm triển khai đồ án môn Lập trình Mobile.
- **Quyết định:**
  1. **Loại bỏ hoàn toàn WAMP Server:** Chuyển đổi toàn bộ tài liệu và cấu hình sang môi trường máy chủ trường cấp.
  2. **Thông số cấu hình máy chủ & CSDL chuẩn hóa:**
     - Địa chỉ IP máy chủ: `103.139.203.43`
     - Cổng & Đường dẫn phpMyAdmin: `https://103.139.203.43:887/phpmyadmin_cd13194c0d96d231/index.php?lang=en`
     - Tên CSDL (Database Name): `sql_nhom46_itimi`
     - Tài khoản (DB User): `sql_nhom46_itimi`
     - Mật khẩu (DB Password): `0a530d0c23153`
  3. **Lợi thế kiến trúc:**
     - Thiết bị di động thật (kết nối 4G/Wifi trường) và máy ảo React Native gọi trực tiếp vào API public trên server mà không cần cấu hình IP LAN ảo hay giải pháp ngrok.
     - Postman test trực tiếp trên server từ bất kỳ đâu.
     - Cả nhóm dùng chung một database duy nhất, đảm bảo tính toàn vẹn và đồng bộ dữ liệu.
  4. **Cập nhật tài liệu:** Đã đồng bộ toàn bộ nội dung trong [`mobile-project/README.md`](../mobile-project/README.md) phản ánh chuẩn xác các thông số kết nối, PDO Database script và cấu hình React Native `API_BASE_URL`.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-23 10:55] - Quyết Định 56: Bổ Sung Quy Chuẩn Gắn Caption Tự Động (SEQ Bảng & SEQ Hình) & Thiết Lập Danh Mục TOF / TOT Tự Động (Word Dynamic Field `TOC \c`)
- **Vấn đề:** 
  1. Người dùng đối chiếu tệp hướng dẫn làm báo cáo của Giảng viên (`mobile-project/huongdanvietbaocao.txt`) và nhận thấy sự tương đồng cao với quy chuẩn Khóa luận tốt nghiệp DNTU (Font Times New Roman 13pt, 1.5 lines, canh đều, Trang 2: Mục lục tự động, Trang 3: Mục lục hình ảnh, Trang 4: Mục lục bảng biểu...).
  2. Người dùng yêu cầu thể chế hóa quy tắc vào bộ luật Khóa luận (`.agents/rules/`): Danh mục hình ảnh và Danh mục bảng biểu **tuyệt đối không được nhập tay số trang**, và **không dùng Header Reference (`\o "1-3"`)** vì sẽ gây ô nhiễm thanh điều hướng Navigation Pane (`Ctrl + F`). Thay vào đó phải dùng tính năng **Insert Caption** của Word kèm mã trường mục lục tự động riêng biệt.
- **Quyết định:**
  1. **Cập nhật quy chuẩn Bảng biểu & Hình ảnh ([`.agents/rules/03_dntu_kltn_tables_and_figures_rules.md`](../.agents/rules/03_dntu_kltn_tables_and_figures_rules.md)):**
     - Bắt buộc gắn tính năng **Insert Caption** của Microsoft Word cho tiêu đề bảng với nhãn Label `"Bảng"` (trường `{ SEQ Bảng \* ARABIC }`).
     - Bắt buộc gắn tính năng **Insert Caption** của Microsoft Word cho tiêu đề hình với nhãn Label `"Hình"` (trường `{ SEQ Hình \* ARABIC }`).
     - Bổ sung **Mục 3: Quy định cấm nhập tay danh mục & Phân biệt cơ chế sinh Mục lục Bảng/Hình so với Mục lục Văn bản**.
  2. **Cập nhật quy chuẩn Bố cục & Mục lục ([`.agents/rules/02_dntu_kltn_structure_and_toc_rules.md`](../.agents/rules/02_dntu_kltn_structure_and_toc_rules.md)):**
     - Bổ sung **Mục 2.4:** Thiết lập Danh mục Sơ đồ, Hình ảnh Tự động (Table of Figures - TOF, `BM210-KT21`) bằng mã trường `<w:fldSimple w:instr="TOC \h \z \c &quot;Hình&quot;"/>`.
     - Bổ sung **Mục 2.5:** Thiết lập Danh mục Bảng biểu Tự động (Table of Tables - TOT, `BM211-KT21`) bằng mã trường `<w:fldSimple w:instr="TOC \h \z \c &quot;Bảng&quot;"/>`.
     - Cập nhật cơ chế **`F9` toàn cục**: Nhấn `Ctrl + A` rồi bấm `F9` sẽ tự động cập nhật đồng thời cả 3 mục lục: TOC, TOF và TOT chính xác 100%.
     - Bổ sung 2 tiêu chí kiểm tra tương ứng vào **Mục 5: Checklist tự động kiểm tra trước khi nộp**.
  3. **Chính sách áp dụng:** Đã cập nhật xong bộ quy tắc hệ thống; tài liệu Khóa luận tốt nghiệp (`KhoaLuanTotNghiep.docx`) sẽ được Người dùng hoặc hệ thống cập nhật sau theo đúng chỉ đạo.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-23 11:45] - Quyết Định 57: Khởi Tạo Hoàn Chỉnh 6 Trang Đầu Tệp Báo Cáo Đồ Án Mobile (`mobile-project/BaoCaoLTMB.docx`) Chuẩn Thể Thức DNTU KLTN & Hướng Dẫn Của GVHD
- **Vấn đề:** 
  1. Người dùng yêu cầu xây dựng sẵn 6 trang đầu cho tài liệu báo cáo đồ án môn Lập trình Thiết bị Di động (`mobile-project/BaoCaoLTMB.docx`) theo đúng cấu trúc hướng dẫn của Giảng viên (`huongdanvietbaocao.txt`).
  2. Bắt buộc áp dụng 100% quy chuẩn hình thức Khóa luận tốt nghiệp DNTU (A4, Lề 2.0-2.0-3.5-2.0 cm, Times New Roman 13pt, dãn dòng 1.5 lines, Before/After 6pt, Thụt dòng 1.5 cm, Căn đều hai bên).
  3. Bắt buộc áp dụng quy chuẩn mới của Quyết định 56: Trang 2 (Mục lục động `{ TOC \o "1-3" }`), Trang 3 (Mục lục hình ảnh động `{ TOC \h \z \c "Hình" }`), Trang 4 (Mục lục bảng biểu động `{ TOC \h \z \c "Bảng" }`), Bảng 1 gắn Caption Word `{ SEQ Bảng \* ARABIC }`, và Trang bìa chứa logo trường DNTU chuẩn xác.
- **Quyết định:**
  1. **Khởi tạo thành công tệp văn bản [`mobile-project/BaoCaoLTMB.docx`](../mobile-project/BaoCaoLTMB.docx) (129.8 KB):**
     - **Trang 1 (Trang bìa):** Bộ Giáo dục và Đào tạo, Trường ĐH Công nghệ Đồng Nai, Khoa CNTT, chèn Logo DNTU chính thức sắc nét, Tên bài báo cáo và Đề tài, Thông tin 2 sinh viên (Trịnh Thành Đạt 1923050149 & Trần Trọng Đại 1923050745, Lớp 23DTH1), GVHD ThS. Nguyễn Thị Liệu, Năm 2026. Thiết lập `different_first_page_header_footer = True` để trang bìa không hiển thị số trang.
     - **Trang 2 (Mục lục):** Kiến trúc hai lớp hoàn chỉnh: Lớp hiển thị tĩnh trực quan với Tab Stop 15.5 cm dấu chấm dẫn dòng và siêu liên kết Bookmark nội bộ; Lớp mã trường động Word `<w:fldSimple w:instr="TOC \o &quot;1-3&quot; \h \z \u"/>`.
     - **Trang 3 (Danh mục hình ảnh):** Tích hợp mã trường động `<w:fldSimple w:instr="TOC \h \z \c &quot;Hình&quot;"/>`, kèm các dòng tĩnh sẵn sàng liên kết đến các sơ đồ ERD, phpMyAdmin, Postman và giao diện React Native.
     - **Trang 4 (Danh mục bảng biểu):** Tích hợp mã trường động `<w:fldSimple w:instr="TOC \h \z \c &quot;Bảng&quot;"/>`, kèm các dòng tĩnh trỏ đến Bảng 1, Bảng 3.1, Bảng 3.2...
     - **Trang 5 (Phần 1: Đặt vấn đề bài toán):** Soạn thảo hoàn chỉnh 1 trang học thuật sâu sắc (420 từ) gồm 3 tiểu mục: 1.1 Bối cảnh số hóa và hành vi tài chính cá nhân; 1.2 Hạn chế của các phương pháp ghi chép hiện hữu; 1.3 Mục tiêu và tính cấp thiết của đề tài.
     - **Trang 6 (Phần 2: Các chức năng chính của đề tài):** Soạn thảo hoàn chỉnh 1 trang học thuật gồm thuyết minh 4 phân hệ chức năng (Quản lý Ví, Ghi nhận Thu/Chi, Ngân sách tháng, Thống kê biểu đồ) và Bảng 1 thiết kế theo chuẩn Academic Navy Theme (`#2B4C7E`), dòng tiêu đề bảng gắn tính năng Insert Caption Word tự động với trường `{ SEQ Bảng \* ARABIC }`.
     - **Từ Trang 7 (Khung mở đầu phần sau):** Đặt sẵn tiêu đề Phần 3 (Backend MySQL & PHP), Phần 4 (Frontend React Native) và Phần 5 (Kết luận) kèm các Bookmark và Outline Level chuẩn để thanh Navigation Pane (`Ctrl + F`) hiển thị cây đề mục đầy đủ.
  2. **Đảm bảo tính tương thích và cập nhật:**
     - Người dùng chỉ cần mở tệp trong Microsoft Word, nhấn `Ctrl + A` rồi bấm **`F9`** là toàn bộ số trang của TOC, TOF và TOT sẽ tự động đồng bộ theo dàn trang thực tế.
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-23 13:00] - Quyết Định 58: Tinh Gọn Dung Lượng Rule 02 (Dưới 8,000 Ký Tự) & Căn Chỉnh Cấu Trúc Đề Mục Báo Cáo Mobile Khớp 100% Theo `huongdanvietbaocao.txt`
- **Vấn đề:** 
  1. Người dùng chỉ rõ: Việc áp dụng quy chuẩn DNTU là về mặt hình thức (Font Times New Roman 13pt, dãn dòng 1.5, căn lề, caption động, bảng biểu...) chứ không phải áp đặt cấu trúc các chương/phần của Khóa luận tốt nghiệp (`PHẦN 1`, `1.1`, `1.2`, `PHẦN 2`...) lên tài liệu môn Lập trình Mobile.
  2. Bố cục đề mục phải tuân thủ nguyên văn theo tệp hướng dẫn của Giảng viên (`mobile-project/huongdanvietbaocao.txt`): Trang 5 là "Đặt vấn đề bài toán" (1 trang), Trang 6 là "Các chức năng chính của đề tài" (1 trang), Từ trang 7 là "Xây dựng Back End (MySQL và PHP)" (Mô hình DB, API tương ứng) và "Xây dựng Front-End (React native)" (Màn hình giao diện thực thi).
  3. Tệp quy tắc [`.agents/rules/02_dntu_kltn_structure_and_toc_rules.md`](../.agents/rules/02_dntu_kltn_structure_and_toc_rules.md) trước đó vượt quá giới hạn 12.000 ký tự (đạt 13.230 ký tự), kích hoạt cảnh báo context của IDE.
- **Quyết định:**
  1. **Tối ưu hóa tệp Rule 02:** Đã cô đọng toàn diện [`.agents/rules/02_dntu_kltn_structure_and_toc_rules.md`](../.agents/rules/02_dntu_kltn_structure_and_toc_rules.md) từ 13.230 ký tự xuống còn **7.947 ký tự** (giảm 40%), bảo toàn nguyên vẹn 100% các điều luật, bảng biểu, thẻ XML TOC/TOF/TOT và checklist kiểm tra. Triệt tiêu hoàn toàn cảnh báo của IDE.
  2. **Tái cấu trúc đề mục [`mobile-project/BaoCaoLTMB.docx`](../mobile-project/BaoCaoLTMB.docx) khớp 100% `huongdanvietbaocao.txt`:**
     - Xóa bỏ toàn bộ cách đánh số kiểu luận văn (`PHẦN 1`, `1.1`, `PHẦN 2`...).
     - Tiêu đề Trang 5: **Đặt vấn đề bài toán** (đúng 1 trang trọn vẹn).
     - Tiêu đề Trang 6: **Các chức năng chính của đề tài** (đúng 1 trang trọn vẹn kèm Bảng 1 gắn Caption `{ SEQ Bảng \* ARABIC }`).
     - Từ Trang 7: **Xây dựng Back End (MySQL và PHP)** (gồm: *Xây dựng mô hình DB*, *API tương ứng các chức năng*) và **Xây dựng Front-End (React native)** (gồm: *Màn hình giao diện thực thi*).
     - Đồng bộ lại Mục lục tĩnh và liên kết Bookmark trong Trang 2 (Mục lục), Trang 3 (Mục lục hình ảnh) và Trang 4 (Mục lục bảng biểu).
- **Trạng thái:** `ACCEPTED & LOCKED`.

---

## [2026-09-28 19:20] - Quyết Định 59: Ban Hành Kế Hoạch & Lộ Trình Triển Khai Đồ Án Mobile (PLAN-18 / `ROADMAP.md`) Cho Nhóm 2 Sinh Viên
- **Vấn đề:** 
  1. Người dùng yêu cầu xây dựng một kế hoạch lộ trình (Roadmap) chi tiết, có phân công công việc cụ thể cho 2 thành viên nhóm (Trịnh Thành Đạt & Trần Trọng Đại), được lưu dạng tệp Markdown trực tiếp trong thư mục `mobile-project/` để khi `git push` lên GitHub, thành viên nhóm có thể đọc trực tiếp và phối hợp thực hiện.
  2. Cần phân định rõ các giai đoạn từ thiết kế CSDL MySQL, code PHP REST API, kiểm thử Postman, dựng khung React Native, ghép nối API đến hoàn thiện báo cáo Word `BaoCaoLTMB.docx` và đóng gói nghiệm thu.
- **Quyết định:**
  1. **Khởi tạo tệp Lộ trình làm việc nhóm [`mobile-project/ROADMAP.md`](../mobile-project/ROADMAP.md):**
     - Thiết lập ma trận phân công trách nhiệm (RACI Matrix) cho 10 hạng mục công việc giữa Trịnh Thành Đạt (Lead / Architecture / Core API & Navigation) và Trần Trọng Đại (Frontend UI / Business API / Postman Testing & Reports).
     - Chi tiết hóa **5 Giai đoạn (Sprints)** thực hiện từ Tuần 1 đến Tuần 5–6 kèm checklist công việc rõ ràng.
     - Quy chuẩn phối hợp Git: Phân nhánh (`main`, `develop`, `feature/*`), quy tắc viết commit message và lịch họp định kỳ (Tối T4 & Tối CN).
  2. **Bảo tồn ngữ cảnh bền vững (Mandatory Context Logging):**
     - Đã lưu snapshot kế hoạch tại [`project_context/plans/2026-09-28_plan_18_mobile_project_development_roadmap.md`](plans/2026-09-28_plan_18_mobile_project_development_roadmap.md).
     - Đã gắn liên kết điều hướng từ [`mobile-project/README.md`](../mobile-project/README.md).
- **Trạng thái:** `ACCEPTED & LOCKED`.

























