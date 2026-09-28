# 🏛️ HỘI ĐỒNG ĐA TÁC TỬ PHÂN TÍCH THIẾT KẾ HỆ THỐNG THÔNG TIN
## (Autonomous Multi-Agent Council & AI Software Engineer Engine)

> **Tên Đề Tài Đồ Án:** Nghiên cứu, thiết kế và xây dựng **Hệ thống Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh (Tích hợp AI)**  
> **Mục Đích:** Phục vụ môn học **Phân tích Thiết kế Hệ thống Thông tin (PTTKHTTT)** và làm **Đồ án Tốt nghiệp Cử nhân CNTT/HTTT**.  
> **Phương Pháp Luận:** **BMAD** (Agile AI-driven) & **Production Claude Principles** (Bottom-up, Tool-first, Sandboxed, Traceable).

---

## 1. Hai Trụ Cột Kỹ Thuật Chính Của Dự Án

```text
┌────────────────────────────────────────────────────────────────────────┐
│ TRỤ CỘT 1: BÀI TOÁN NGHIỆP VỤ (Domain Application)                     │
│ - Quản lý chi tiêu cá nhân & Cảnh báo tốc độ tiêu tiền (Spending Velo).│
│ - Hoạch định Ngân sách 50/30/20, Zero-Based Budgeting & Sổ Kakeibo.      │
│ - Bóc tách hóa đơn ăn uống Multimodal AI OCR kèm Hậu kiểm toán học.    │
│ - Thuật toán Tối ưu hóa Nợ Cá nhân: Debt Snowball vs Debt Avalanche.     │
├────────────────────────────────────────────────────────────────────────┤
│ TRỤ CỘT 2: BỘ NHÂN AI SOFTWARE ENGINEER (Meta-Engineering Engine)      │
│ - Hệ thống Hội Đồng 13 Tác Tử Full-Stack hóa thân các danh nhân.       │
│ - Lớp công cụ an toàn: Filesystem Sandbox, Terminal Runner, Git Diff.  │
│ - Chu trình 4 pha khép kín: Khách Hàng -> BA -> Kiến Trúc -> Final RFC.│
│ - Tự động sinh Ma Trận Truy Vết Nguồn Gốc (Traceability Matrix).       │
│ - Kiểm soát chất lượng nội dung sâu (Deep Content Quality Gates).      │
│ - 14 Kỹ năng Antigravity IDE Skills (.agents/skills/) & CLI Runner.    │
└────────────────────────────────────────────────────────────────────────┘
```

---

## 2. Bản Đồ Roster 13 Chuyên Gia Kiệt Xuất Trong Hội Đồng

| Tác Tử | Chuyên Gia Hóa Thân | Vai Trò Chuyên Môn | Kỹ Năng Python Thực Thi | Đầu Ra Chịu Trách Nhiệm |
| :--- | :--- | :--- | :--- | :--- |
| **Agent 00** | **Council Orchestrator** | Chủ tịch Hội đồng & Tổng duyệt | [`OrchestratorRFCSkill`](core_engine/skills/orchestrator_rfc_skill.py) | `04_final_rfc.md` |
| **Agent 01** | **Richard Thaler (Nobel)** | Kinh tế Hành vi & Cú hích Nudge | [`ThalerNudgeSkill`](core_engine/skills/thaler_nudge_skill.py) | `01_user_feedback.md` |
| **Agent 02** | **Personal Budget Coach** | Hoạch định Ngân sách (50/30/20, ZBB, Kakeibo) | [`PersonalBudgetCasesSkill`](core_engine/skills/personal_budget_cases_skill.py) | `01_user_feedback.md` |
| **Agent 03** | **Alistair Cockburn / Debra Paul** | Lead BA (Chuẩn IEEE 29148) | [`CockburnUseCaseSkill`](core_engine/skills/cockburn_use_case_skill.py) | `02_use_case_specs.md` |
| **Agent 04** | **Eric Evans (Cha đẻ DDD)** | Domain-Driven Design & Clean Arch | [`EvansDomainSkill`](core_engine/skills/evans_domain_skill.py) | `03_technical_adr.md` |
| **Agent 05** | **Martin Kleppmann & Sadalage** | Universal Database (3NF & NoSQL) | [`KleppmannDatabaseSkill`](core_engine/skills/kleppmann_database_skill.py) | `03_technical_adr.md` |
| **Agent 06** | **Debt & Cashflow Optimizer** | Tối ưu hóa Trả nợ (Snowball vs Avalanche) | [`DebtCashflowOptimizerSkill`](core_engine/skills/debt_cashflow_optimizer_skill.py) | `03_technical_adr.md` |
| **Agent 07** | **Dr. Werner Vogels (Amazon)** | Async Queue HTTP 202 & DevOps | [`VogelsAsyncSkill`](core_engine/skills/vogels_async_skill.py) | `03_technical_adr.md` |
| **Agent 08** | **Prof. Andrew Ng (AI Pioneer)** | Multimodal AI OCR & Math Audit | [`AndrewNgAISkill`](core_engine/skills/andrew_ng_ai_skill.py) | `03_technical_adr.md` |
| **Agent 09** | **Brad Frost & Don Norman** | Atomic Design System & UI/UX | [`FrostAtomicDesignSkill`](core_engine/skills/frost_atomic_design_skill.py) | `01_user_feedback.md` & `03_technical_adr.md` |
| **Agent 10** | **Dan Abramov & Vjeux** | Cross-Platform & Client State 0ms | [`AbramovClientStateSkill`](core_engine/skills/abramov_client_state_skill.py) | `03_technical_adr.md` |
| **Agent 11** | **Bruce Schneier & OWASP** | Fintech Security, AES-256 & STRIDE | [`SchneierSecuritySkill`](core_engine/skills/schneier_security_skill.py) | `03_technical_adr.md` |
| **Agent 12** | **Kent Beck & Martin Fowler** | Full-Stack QA & Playwright E2E | [`BeckQAStrategySkill`](core_engine/skills/beck_qa_strategy_skill.py) | `04_final_rfc.md` |

---

## 3. Quy Trình Vận Hành 4 Pha Của Mỗi Phiên Bàn Thảo (Session Protocol)

```text
┌────────────────────────────────────────────────────────────────────────────────────────┐
│                       CHU TRÌNH BÀN THẢO 4 PHA (FULL-STACK SESSION)                    │
│                                                                                        │
│  [PHA 1: Khách Hàng & Design] ──► [PHA 2: BA Chuẩn Hóa] ──► [PHA 3: Kiến Trúc Sư]     │
│  - Richard Thaler (Nudge)         - Cockburn & Paul          - Evans (Domain & Money)  │
│  - Personal Budget Coach (50/30/20, ZBB, Kakeibo)    - IEEE 29148 Specs         - Kleppmann (DB 3NF)      │
│  - Brad Frost (Atomic Design)     - Gherkin Scenarios        - Debt Optimizer (Snowball vs Avalanche) │
│  - 01_user_feedback.md            - 02_use_case_specs.md     - Vogels (Async Queue)    │
│                                                              - Andrew Ng (AI OCR)      │
│                                                              - Abramov (Client State)  │
│                                                              - Schneier (Security)     │
│                                                              - Beck (Test Strategy)    │
│                                                              - 03_technical_adr.md     │
│                                                                       │                │
│                                                                       ▼                │
│  [PHA 4: Phê Duyệt & Xuất Bản] ◄──────────────────────────────────────┘                │
│  - Council Orchestrator                                                                │
│  - Sinh Ma Trận Truy Vết Traceability Matrix (REQ -> UC -> UI -> ADR -> DB -> Test)   │
│  - Xuất bản: 04_final_rfc.md (Sẵn sàng chuyển giao lập trình Full-Stack)               │
└────────────────────────────────────────────────────────────────────────────────────────┘
```

---

## 4. Hướng Dẫn Sử Dụng CLI Runner (`run_council_session.py`)


Hệ thống cung cấp công cụ dòng lệnh trực quan để chạy toàn bộ chu trình 4 pha:

```powershell
# 1. Khởi chạy phiên phân tích thiết kế tính năng mới (chế độ mô phỏng siêu tốc)
python run_council_session.py --feature "Quản Lý Quỹ Chung Gia Đình" --provider mock

# 2. Khởi chạy với OpenAI API (GPT-4o)
python run_council_session.py --feature "Bóc Tách Hóa Đơn OCR" --provider openai

# 3. Khởi chạy với Google Gemini API
python run_council_session.py --feature "Chia Tiền Điện Nước Phòng Trọ" --provider gemini

# 4. Thẩm định chất lượng và độ toàn vẹn của một phiên làm việc
python run_council_session.py --audit sessions/session_01_household_fund

# 5. Liệt kê danh sách tất cả các phiên bàn thảo
python run_council_session.py --list
```

---

## 5. Bộ 4 Benchmark Sessions Thực Tế Mẫu Trong Thư Mục [`sessions/`](sessions/)

Toàn bộ 4 phiên mẫu đều đạt **100/100 Điểm Toàn Vẹn Cấu Trúc & 100/100 Điểm Chất Lượng Nội Dung**:

1. [`sessions/session_01_household_fund/`](sessions/session_01_household_fund/): Quản lý Quỹ chung gia đình đóng theo tỷ lệ lương 60-40.
2. [`sessions/session_02_roommate_utilities/`](sessions/session_02_roommate_utilities/): Phân bổ chi phí bạn cùng phòng & Điện nước bậc thang.
3. [`sessions/session_03_trip_ocr_itemized_split/`](sessions/session_03_trip_ocr_itemized_split/): Bóc tách hóa đơn ăn uống OCR & Rút gọn nợ đồ thị $O(V \log V)$.
4. [`sessions/session_04_spending_velocity_nudge/`](sessions/session_04_spending_velocity_nudge/): Hạn mức ngân sách cá nhân & Cảnh báo tốc độ đốt tiền.

---

## 6. Cấu Trúc Thư Mục Dự Án (Repository Directory Map)

```text
Project/agents/
├── README.md                            # Cẩm nang tổng quan dự án & hướng dẫn chạy
├── AGENTS.md                            # Quy ước Hội đồng Đa tác tử 4 pha
├── run_council_session.py               # CLI Runner chính thức
├── .gitignore                           # Cấu hình loại bỏ cache và file tạm
│
├── .agents/skills/                      # 10 Kỹ năng Antigravity IDE Skills (council-*)
├── core/                                # 9 Persona danh nhân chuẩn Production (00 -> 08)
├── core_engine/                         # Lõi Python Engine (Council, Skills, Tools, Workflow)
│   ├── council/                         # 9 Thực thể PersonaAgent
│   ├── skills/                          # 9 Python Skills nghiệp vụ chuyên sâu
│   ├── tools/                           # Tool Sandbox (filesystem, terminal, git)
│   ├── workflow/                        # State Machine & Bounded Retry Loop
│   ├── agents/                          # Coding Agent & Subagents
│   ├── evaluation/                      # Thu thập chỉ số thực nghiệm cho ĐATN
│   ├── memory/                          # Project Memory & Snapshot
│   ├── llm_client.py                    # Multi-LLM Adapter (Mock, OpenAI, Gemini, Ollama)
│   └── state.py                         # ProjectState dataclass
│
├── docs/                                # Tài liệu gốc & đề tài nghiên cứu
│   ├── specs/                           # Đề Tài Đồ Án .docx, .md ban đầu
│   └── standalone_scripts/              # Script giải thuật tham khảo độc lập
│
├── sessions/                            # Các phiên bàn thảo 4 pha & Benchmark (01 -> 04)
├── tests/                               # Bộ 17 test files (91 Unit Tests tự động - 100% Passed)
├── project_context/                     # Ngữ cảnh bền vững (00_MASTER_CONTEXT.md, logs, plans)
└── README.md                            # Cẩm nang tổng quan dự án
```

---

## 7. Kiểm Thử Tự Động Toàn Hệ Thống

Chạy toàn bộ 91 bài Unit & Integration Tests:
```powershell
python -m unittest discover -s tests
```
**Kết quả:** `Ran 91 tests in 6.6s -> OK (100% Passed)`.
