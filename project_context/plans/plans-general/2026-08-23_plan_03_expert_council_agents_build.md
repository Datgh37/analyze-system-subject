# KẾ HOẠCH TỔNG HỢP 03: XÂY DỰNG TOÀN DIỆN HỘI ĐỒNG 9 CHUYÊN GIA KIỆT XUẤT (AGENT 00 ĐẾN AGENT 08)
**Thời điểm khởi tạo:** 2026-08-23 23:37  
**Thời điểm hoàn tất:** 2026-08-24 22:15  
**Trạng thái:** ĐÃ HOÀN THÀNH & KIỂM THỬ XÁC MINH 100% (COMPLETED & VERIFIED - 63/63 TESTS OK)  

---

## 1. Mục Tiêu Tổng Quát
Nâng cấp và chuyển đổi toàn bộ hệ thống từ các Persona chung chung sang **Hội Đồng 9 Chuyên Gia Kiệt Xuất Hóa Thân Các Danh Nhân Thế Giới**, trang bị cho mỗi chuyên gia một bộ Kỹ năng Python (`core_engine/skills/`) độc lập và xây dựng chu trình 4 pha bàn thảo khép kín.

---

## 2. Nhật Ký Triển Khai Tuần Tự Từng Agent (Từ 03A đến 03I)

### 🔹 Sprint 03A: Agent 01 — Richard Thaler (Nobel Kinh Tế 2017)
- **Nghiệp vụ:** Tâm lý học Hành vi (Behavioral Finance), Thuyết Cú Hích (Nudge Theory), Cảnh báo Tốc độ Chi tiêu (Spending Velocity) và Đánh giá Rào cản Nhập liệu (UX Friction Score).
- **Thành phần kỹ thuật:**
  1. `core/01_richard_thaler.md`: Prompt đặc tả chuyên sâu GS. Richard Thaler.
  2. `core_engine/skills/thaler_nudge_skill.py`: Python Skill tính toán Nudge, Spending Velocity và UX Friction.
  3. `core_engine/council/richard_thaler_agent.py`: Thực thể Agent kế thừa từ `PersonaAgent`.
  4. `tests/test_agent_01_thaler.py`: Bộ 4 test cases độc lập (100% Passed).

---

### 🔹 Sprint 03B: Agent 02 — Chuyên Gia Quản Trị Tài Chính Nhóm & Hộ Gia Đình
- **Nghiệp vụ:** Quản trị Quỹ chung Gia đình (Joint Pool Fund 60-40, cấp tiền tiêu vặt cho con), Phân bổ chi phí Bạn cùng phòng (Co-living), Nghiệp vụ Tạm ứng & Hoàn ứng (Reimbursement).
- **Thành phần kỹ thuật:**
  1. `core/02_group_leader.md`: Prompt đặc tả chuyên gia Quản trị Tài chính Tập thể.
  2. `core_engine/skills/group_split_cases_skill.py`: Python Skill mô phỏng 3 mô hình dòng tiền thực tế.
  3. `core_engine/council/group_leader_agent.py`: Thực thể Agent kế thừa từ `PersonaAgent`.
  4. `tests/test_agent_02_group_leader.py`: Bộ 4 test cases độc lập (100% Passed).

---

### 🔹 Sprint 03C: Agent 03 — Alistair Cockburn & Debra Paul (Lead BA Specialist)
- **Nghiệp vụ:** Phân tích Nghiệp vụ (Business Analysis), chuẩn hóa Đặc tả Ca Sử Dụng theo chuẩn quốc tế **IEEE 29148** (bảng tương tác 2 cột Actor-System, Luồng ngoại lệ bắt buộc) và sinh kịch bản **Gherkin BDD (`Given-When-Then`)**.
- **Thành phần kỹ thuật:**
  1. `core/03_cockburn_ba.md`: Prompt đặc tả Alistair Cockburn & Debra Paul.
  2. `core_engine/skills/cockburn_use_case_skill.py`: Python Skill định dạng IEEE 29148 và sinh kịch bản Gherkin.
  3. `core_engine/council/cockburn_ba_agent.py`: Thực thể Agent kế thừa từ `PersonaAgent`.
  4. `tests/test_agent_03_cockburn_ba.py`: Bộ 4 test cases độc lập (100% Passed).

---

### 🔹 Sprint 03D: Agent 04 — Eric Evans (Cha Đẻ Domain-Driven Design)
- **Nghiệp vụ:** Mô hình hóa Miền Nghiệp vụ Tài chính phong phú (DDD), phân chia 4 Bounded Contexts độc lập, chuẩn hóa Value Object bất biến `Money` (dùng `Decimal` chống sai số) và soạn thảo **ADR Clean Architecture**.
- **Thành phần kỹ thuật:**
  1. `core/04_eric_evans.md`: Prompt đặc tả Eric Evans.
  2. `core_engine/skills/evans_domain_skill.py`: Python Skill cung cấp `Money` Decimal và thẩm định Bounded Contexts.
  3. `core_engine/council/eric_evans_agent.py`: Thực thể Agent kế thừa từ `PersonaAgent`.
  4. `tests/test_agent_04_eric_evans.py`: Bộ 5 test cases độc lập (100% Passed).

---

### 🔹 Sprint 03E: Agent 05 — Martin Kleppmann & Pramod Sadalage (Universal Database Architect)
- **Nghiệp vụ:** Thiết kế CSDL Toàn diện (Polyglot Persistence): PostgreSQL 3NF cho Sổ cái giao dịch, NoSQL JSONB cho hóa đơn OCR, In-Memory Redis cho Khóa phân tán và chiến lược **Pessimistic Lock (`SELECT ... FOR UPDATE`)** chống Lost Update.
- **Thành phần kỹ thuật:**
  1. `core/05_martin_kleppmann.md`: Prompt đặc tả Martin Kleppmann & Sadalage.
  2. `core_engine/skills/kleppmann_database_skill.py`: Python Skill thẩm định chuẩn 3NF và tư vấn Concurrency Lock.
  3. `core_engine/council/martin_kleppmann_agent.py`: Thực thể Agent kế thừa từ `PersonaAgent`.
  4. `tests/test_agent_05_kleppmann.py`: Bộ 5 test cases độc lập (100% Passed).

---

### 🔹 Sprint 03F: Agent 06 — Robert Tarjan & Jon Kleinberg (Graph Algorithm Specialist)
- **Nghiệp vụ:** Giải thuật Đồ thị Rút gọn Nợ Nhóm (Debt Simplification Engine): Greedy Net Balance Two-Pointer giảm số giao dịch xuống tối đa **$N-1$** với độ phức tạp **$O(V \log V)$**, triệt tiêu chu trình nợ lặp và sinh biểu đồ Mermaid trực quan.
- **Thành phần kỹ thuật:**
  1. `core/06_tarjan_graph.md`: Prompt đặc tả Robert Tarjan & Jon Kleinberg.
  2. `core_engine/skills/tarjan_graph_skill.py`: Python Skill rút gọn nợ $O(V \log V)$ và sinh mã Mermaid.
  3. `core_engine/council/tarjan_graph_agent.py`: Thực thể Agent kế thừa từ `PersonaAgent`.
  4. `tests/test_agent_06_tarjan_graph.py`: Bộ 4 test cases độc lập (100% Passed).

---

### 🔹 Sprint 03G: Agent 07 — Dr. Werner Vogels (CTO Amazon — Async & DevOps)
- **Nghiệp vụ:** Kiến trúc Bất đồng bộ Hướng sự kiện (Event-Driven & Message Queue HTTP 202 cho tác vụ AI OCR nặng), cơ chế **Idempotency (SHA-256 Hash)** chống gửi lặp giao dịch và đóng gói Docker Compose.
- **Thành phần kỹ thuật:**
  1. `core/07_werner_vogels.md`: Prompt đặc tả Dr. Werner Vogels.
  2. `core_engine/skills/vogels_async_skill.py`: Python Skill kiểm tra Idempotency và cấp job_id Message Queue.
  3. `core_engine/council/werner_vogels_agent.py`: Thực thể Agent kế thừa từ `PersonaAgent`.
  4. `tests/test_agent_07_werner_vogels.py`: Bộ 5 test cases độc lập (100% Passed).

---

### 🔹 Sprint 03H: Agent 08 — Prof. Andrew Ng (AI Pioneer — Multimodal AI & OCR)
- **Nghiệp vụ:** Pipeline bóc tách hóa đơn Multimodal AI theo phương pháp **Data-Centric AI**, ràng buộc Pydantic JSON Schema và thuật toán **Hậu kiểm toán học (Mathematical Audit)** đối soát tổng tiền món với tổng bill kèm cơ chế Human-in-the-Loop.
- **Thành phần kỹ thuật:**
  1. `core/08_andrew_ng.md`: Prompt đặc tả Prof. Andrew Ng.
  2. `core_engine/skills/andrew_ng_ai_skill.py`: Python Skill hậu kiểm toán học và sinh JSON Schema.
  3. `core_engine/council/andrew_ng_agent.py`: Thực thể Agent kế thừa từ `PersonaAgent`.
  4. `tests/test_agent_08_andrew_ng.py`: Bộ 5 test cases độc lập (100% Passed).

---

### 🔹 Sprint 03I: Agent 00 — Council Orchestrator (Chủ Tịch Hội Đồng)
- **Nghiệp vụ:** Điều phối toàn bộ quy trình 4 pha của một Phiên Bàn Thảo (Session Protocol), kết nối 8 Chuyên gia Kiệt xuất, tổng hợp thành tài liệu **`04_final_rfc.md`** hoàn chỉnh cho lập trình viên và dọn dẹp/chuẩn hóa danh sách tệp trong thư mục `core/`.
- **Thành phần kỹ thuật:**
  1. `core/00_orchestrator.md`: Cập nhật đặc tả Chủ tịch Hội đồng theo chuẩn 9 Agents.
  2. `core_engine/skills/orchestrator_rfc_skill.py`: Python Skill tổng hợp RFC và kiểm tra tính toàn vẹn phiên.
  3. `core_engine/council/council_orchestrator_agent.py`: Thực thể Agent Orchestrator điều phối cả 8 Agent thành viên qua 4 pha.
  4. `tests/test_agent_00_orchestrator.py`: Bộ 4 test cases độc lập (100% Passed).
  5. `core/` Cleanup: Dọn dẹp sạch sẽ để `core/` chỉ còn đúng 9 tệp chuẩn từ `00` đến `08`.
