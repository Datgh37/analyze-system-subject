# Kế Hoạch 21: Nghiên Cứu & Chiến Lược Tích Hợp Toàn Diện Bộ Kỹ Năng ECC (Everything Claude Code) Vào Hội Đồng Đa Tác Tử

> **Trạng thái:** `PROPOSED & PENDING USER REVIEW`  
> **Thời gian khởi tạo:** 2026-10-07  
> **Mục tiêu:** Huy động Hội đồng 14 Tác tử (`.agents`) nghiên cứu sâu kho tàng kỹ năng của repository Everything Claude Code (`ecc-universal` 2.2.3), phân tích sự tương thích với chuẩn Antigravity 2.0 theo hướng dẫn [`docs/ANTIGRAVITY-ECC-SETUP-GUIDE.md`](../../docs/ANTIGRAVITY-ECC-SETUP-GUIDE.md), và thiết lập ma trận phân bổ quyền năng "Cộng Sinh Chuyên Môn Hóa" (Specialized Symbiosis) nhằm bao quát toàn bộ vòng đời xây dựng phần mềm (Software Engineering Lifecycle) cho Đồ án Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh (PFM).

---

## 1. Kết Quả Khảo Sát & Giải Mã Kho Kỹ Năng ECC (`ecc-universal`)

Thông qua việc khảo sát trực tiếp gói phần mềm `ecc-universal` (phiên bản 2.2.3) và chạy thử nghiệm quy trình cài đặt sandbox chuẩn Antigravity 2.0, Hội đồng ghi nhận:

### 1.1 Cấu Trúc Thành Phần Của ECC
ECC là một bộ toolkit kỹ nghệ đa tác tử (Agentic Engineering Toolkit) toàn diện bao gồm:
1. **69 Custom Subagents (`agents/*.md`):** Các tác tử chuyên môn hóa sâu (như `architect`, `code-reviewer`, `database-reviewer`, `security-reviewer`, `tdd-guide`, `performance-optimizer`, `spec-miner`, `e2e-runner`, `react-reviewer`, `python-reviewer`...). Mỗi agent đều được cấu hình frontmatter chuẩn Antigravity (`tools`, `model: pro/flash`, `description`).
2. **Hơn 100 Agent Skills (`skills/<name>/SKILL.md`):** Các kỹ năng thực chiến bao quát từ kiến trúc hệ thống (`architecture-decision-records`, `api-design`, `hexagonal-architecture`), cơ sở dữ liệu (`database-migrations`, `postgres-patterns`, `redis-patterns`), trí tuệ nhân tạo (`prompt-optimizer`, `eval-harness`), bảo mật (`security-scan`, `the-security-guide`), đến kiểm thử tự động (`tdd-workflow`, `verification-loop`, `browser-qa`).
3. **Workflows Slash Commands (`commands/*.md` $\rightarrow$ `.agents/workflows/`):** Các kịch bản thực thi lệnh tắt như `/plan`, `/code-review`, `/checkpoint`, `/quality-gate`, `/test`.
4. **Hệ thống Quy tắc Workspace (`rules/`):** Các quy chuẩn mã nguồn theo từng ngôn ngữ và nền tảng.

### 1.2 Cảnh Báo Nguy Cơ Khi Cài Đặt Không Chọn Lọc (Wholesale Unfiltered Risk)
- Nếu chạy lệnh cài đặt toàn bộ profile hoặc full ECC vào dự án:
  * Hơn 100 tệp rules tiếng Anh của các ngôn ngữ hoàn toàn xa lạ với đồ án (như `arkts`, `perl`, `ruby`, `swift`, `csharp`, `fsharp`, `harmonyos`...) sẽ tràn vào thư mục `.agents/rules/`.
  * Các rules này sẽ làm loãng context window của LLM và gây xung đột với 5 quy định học thuật khắt khe về format Khóa luận Tốt nghiệp DNTU hiện tại (`01_dntu_kltn_format_rules.md` đến `04_dntu_kltn_academic_writing_style_rules.md`).
  * Việc đưa hàng chục agent và skill không cần thiết vào sẽ làm rối danh mục kỹ năng của Antigravity IDE.

$\rightarrow$ **Kết luận từ Hội đồng:** Cần áp dụng chiến lược **Cài đặt chọn lọc kết hợp phân bổ chuyên môn hóa (Selective Install & Council Capability Symbiosis)** tương tự như đợt tích hợp thành công Bộ Ba Tam Giác Vàng Frontend (Agent 09, 13, 10 ở Quyết định 73).

---

## 2. Ý Kiến Phản Biện Chuyên Môn Từ Các Tác Tử Hội Đồng

Hội đồng đã tổ chức phiên họp kín giữa các Tác tử liên quan để đánh giá giá trị thực tế của ECC đối với đề tài PFM:

### 2.1 Agent 00 (Council Orchestrator) - Điều Phối Vòng Đời & Quản Trị Ngữ Cảnh
> *"ECC giải quyết được 'nút thắt cổ chai' lớn nhất của các phiên làm việc dài: hiện tượng Context Drifting (trôi dạt ngữ cảnh) và Stale Documentation (tài liệu sống bị lỗi thời). Tôi đề xuất tiếp nhận ngay các kỹ năng `living-docs-governance`, `intent-driven-development`, `context-budget` và `strategic-compact`. Điều này sẽ giúp Hội đồng kiểm soát chặt chẽ ngân sách token, tự động nén ngữ cảnh thông minh và bảo đảm tài liệu thiết kế luôn đồng bộ 100% với mã nguồn."*

### 2.2 Agent 03 (Alistair Cockburn & Debra Paul - Lead BA) - Khai Thác Đặc Tả
> *"Kỹ năng `spec-miner` của ECC là một công cụ phi thường. Khi dự án bước sang giai đoạn code app hoặc sinh tài liệu, `spec-miner` cho phép quét ngược toàn bộ codebase để bóc tách lại ma trận nghiệp vụ, đối chiếu ngược với 60 Use Cases đã đặc tả ở Chương 3. Tôi cũng đề xuất nhận thêm `product-lens` để phân tích sâu hơn giá trị người dùng."*

### 2.3 Agent 04 (Eric Evans) & Agent 10 (Dan Abramov) - Kiến Trúc DDD & Anti-Slop Code
> *"Chúng tôi rất tán thành việc tích hợp `architecture-decision-records` (ADR), `code-reviewer`, `refactor-cleaner` và `code-simplifier`. Thay vì chỉ thiết kế trên giấy, các kỹ năng này sẽ đóng vai trò như 'người gác cổng' trong các đợt sinh code, tự động phát hiện mã nguồn bị phình to (bloat), vi phạm tính bất biến của Value Object Money, hoặc vi phạm nguyên lý Clean Architecture."*

### 2.4 Agent 05 (Martin Kleppmann) - Kiểm Toán CSDL & Migration
> *"Các kỹ năng `database-reviewer`, `database-migrations`, `postgres-patterns`, `mysql-patterns` và `redis-patterns` cực kỳ thiết thực. Chúng bổ sung cho tôi khả năng tự động kiểm tra xem các câu lệnh SQL có vi phạm chuẩn 3NF không, các index khóa ngoại đã được tối ưu chưa, và các migration có giữ vững tính toàn vẹn ACID của tài khoản đa ví hay không."*

### 2.5 Agent 07 (Werner Vogels) - DevOps & Tối Ưu Độ Trễ
> *"ECC cung cấp `docker-patterns`, `deployment-patterns` và `latency-critical-systems`. Đây chính là mảnh ghép còn thiếu để tôi hoàn thiện thiết kế kiến trúc Chương 4: triển khai hạ tầng Docker Compose chuẩn production, cấu hình hàng đợi RabbitMQ/BullMQ và giám sát độ trễ xử lý bất đồng bộ HTTP 202 Accepted dưới 200ms."*

### 2.6 Agent 08 (Andrew Ng) - Tối Ưu Pipeline AI & Benchmark
> *"Kỹ năng `prompt-optimizer` và `eval-harness` của ECC sẽ giúp tôi thiết lập bộ khung đánh giá định lượng (Mathematical Benchmark) cho pipeline OCR hóa đơn. Chúng ta có thể tự động đo lường tỷ lệ bóc tách chính xác trên tập dữ liệu hóa đơn thực tế và tối ưu hóa chi phí token (`cost-aware-llm-pipeline`)."*

### 2.7 Agent 11 (Bruce Schneier) - An Ninh Thông Tin Fintech
> *"Tôi nhiệt liệt ủng hộ việc tiếp nhận `security-reviewer`, `security-scan` và `the-security-guide`. Trong một ứng dụng tài chính cá nhân, bất kỳ lỗ hổng nào liên quan đến rò rỉ JWT, sai sót mã hóa AES-256 hay injection đều là thảm họa. Kỹ năng quét bảo mật tự động của ECC sẽ là công cụ kiểm toán độc lập đắc lực cho tôi."*

### 2.8 Agent 12 (Kent Beck & Martin Fowler) - TDD & Verification Loop
> *"Đây chính là vũ khí tối thượng cho QA: `tdd-guide`, `tdd-workflow`, `verification-loop`, `e2e-runner` và `silent-failure-hunter`. Với bộ kỹ năng này, quy trình phát triển phần mềm của chúng ta sẽ thực sự vận hành theo chuẩn mực Extreme Programming (XP): viết test trước khi viết code, tự động phát hiện lỗi ngầm và chạy verification loop khép kín."*

---

## 3. Ma Trận Phân Bổ Kỹ Năng ECC Cho 14 Tác Tử Hội Đồng (Council Capability Matrix)

Nhằm đảm bảo tính chuyên môn hóa cao, không gây xung đột và phát huy tối đa sức mạnh của từng tác tử, Hội đồng đề xuất ma trận phân bổ độc quyền:

```
┌───────────────────────────────────────────────────────────────────────────────┐
│                      HỘI ĐỒNG ĐA TÁC TỬ (14 AGENTS)                          │
├─────────────────────┬───────────────────────────┬─────────────────────────────┤
│ Mã Tác Tử           │ Chuyên Gia Hóa Thân       │ Gói Kỹ Năng / Subagents ECC │
├─────────────────────┼───────────────────────────┼─────────────────────────────┤
│ Agent 00            │ Council Orchestrator      │ living-docs-governance      │
│                     │                           │ intent-driven-development   │
│                     │                           │ plan-canvas & orchestrate   │
│                     │                           │ context-budget & compact    │
├─────────────────────┼───────────────────────────┼─────────────────────────────┤
│ Agent 03            │ Cockburn & Debra Paul     │ spec-miner                  │
│                     │ (Lead BA)                 │ product-lens                │
│                     │                           │ market-research             │
├─────────────────────┼───────────────────────────┼─────────────────────────────┤
│ Agent 04 & 10       │ Eric Evans & Dan Abramov  │ architecture-decision-recs  │
│                     │ (DDD & Code Craftsman)    │ code-reviewer & simplifier  │
│                     │                           │ refactor-cleaner            │
│                     │                           │ backend-patterns            │
│                     │                           │ react-reviewer & patterns   │
├─────────────────────┼───────────────────────────┼─────────────────────────────┤
│ Agent 05            │ Martin Kleppmann          │ database-reviewer           │
│                     │ (Universal Database)      │ database-migrations         │
│                     │                           │ postgres/mysql/redis-patts  │
├─────────────────────┼───────────────────────────┼─────────────────────────────┤
│ Agent 07            │ Werner Vogels             │ docker-patterns             │
│                     │ (DevOps & Async Arch)     │ deployment-patterns         │
│                     │                           │ latency-critical-systems    │
├─────────────────────┼───────────────────────────┼─────────────────────────────┤
│ Agent 08            │ Andrew Ng                 │ prompt-optimizer            │
│                     │ (AI Pioneer)              │ eval-harness & agent-eval   │
│                     │                           │ cost-aware-llm-pipeline     │
├─────────────────────┼───────────────────────────┼─────────────────────────────┤
│ Agent 09 & 13       │ Brad Frost & J. Garnier   │ a11y-architect              │
│                     │ (Design System & Motion)  │ click-path-audit            │
│                     │                           │ make-interfaces-feel-better │
├─────────────────────┼───────────────────────────┼─────────────────────────────┤
│ Agent 11            │ Bruce Schneier            │ security-reviewer           │
│                     │ (Fintech Security Lead)   │ security-scan & review      │
│                     │                           │ the-security-guide          │
├─────────────────────┼───────────────────────────┼─────────────────────────────┤
│ Agent 12            │ Kent Beck & M. Fowler     │ tdd-guide & tdd-workflow    │
│                     │ (Test Lead & QA)          │ verification-loop           │
│                     │                           │ e2e-runner & e2e-testing    │
│                     │                           │ silent-failure-hunter       │
└─────────────────────┴───────────────────────────┴─────────────────────────────┘
```

---

## 4. Phương Án Triển Khai Kỹ Thuật (Implementation Roadmap)

Quá trình tích hợp sẽ được thực hiện theo 4 giai đoạn chặt chẽ:

### Giai Đoạn 1: Lọc & Cài Đặt Chọn Lọc Vào Không Gian Workspace (`.agents/`)
- **Bảo vệ tuyệt đối:** Bảo toàn 100% các file rules học thuật DNTU hiện có trong `.agents/rules/` (`01_dntu_kltn_format_rules.md` đến `04_...`).
- **Thực thi cài đặt chọn lọc:**
  * Cài đặt các **Workflows** hữu ích vào `.agents/workflows/`: `/plan`, `/code-review`, `/checkpoint`, `/quality-gate`, `/test`.
  * Cài đặt danh mục các **Custom Subagents** cốt lõi vào `.agents/agents/` (đã được chuyển đổi model `pro`/`flash` và toolset chuẩn Antigravity).
  * Cài đặt danh mục các **Skills** được chọn lọc vào `.agents/skills/` (giữ nguyên quy cách chuẩn Antigravity 2.0 có `SKILL.md`).
- **Loại trừ rác:** Chặn tuyệt đối các rules/skills của các ngôn ngữ không sử dụng trong dự án (`arkts`, `perl`, `ruby`, `swift`, `csharp`, `fsharp`, `harmonyos`).

### Giai Đoạn 2: Đồng Bộ & Nâng Cấp Hội Đồng Đa Tác Tử (`core/*.md` & `.agents/skills/council-*/`)
- Cập nhật đặc tả của các tác tử trong `core/` (Agent 00, 03, 04, 05, 07, 08, 10, 11, 12, 13) để ghi nhận các kỹ năng ECC mới được tích hợp.
- Cập nhật các tệp `SKILL.md` của từng `council-*` tương ứng để Antigravity nhận biết quyền sở hữu và kích hoạt tự động khi cần.

### Giai Đoạn 3: Nâng Cấp Core Engine Python (`core_engine/`)
- Mở rộng các lớp Python trong `core_engine/skills/` và `core_engine/council/` để các Agent có thể trực tiếp gọi các script kiểm tra chất lượng của ECC (như `verify_mathematical_audit`, `quality_gate`, `security_scan`).

### Giai Đoạn 4: Kiểm Thử Toàn Diện & Đóng Gói
- Xây dựng unit tests mới kiểm tra tính toàn vẹn của hệ thống sau khi tích hợp ECC.
- Bảo đảm **100% tests PASSED**.
- Cập nhật nhật ký dự án `decisions_and_changes_log.md` (Quyết Định 76) và `00_MASTER_CONTEXT.md`.
