# KẾ HOẠCH TỔNG HỢP 07: MỞ RỘNG HỘI ĐỒNG ĐA TÁC TỬ CHUẨN FULL-STACK 100% (AGENT 09 ĐẾN AGENT 12)
**Thời điểm khởi tạo:** 2026-08-24 23:20  
**Thời điểm hoàn tất:** 2026-08-24 23:45  
**Trạng thái:** ĐÃ HOÀN THÀNH & KIỂM THỬ XÁC MINH 100% (COMPLETED & VERIFIED - 91/91 TESTS OK)  

---

## 1. Mục Tiêu Tổng Quát
Mở rộng và hoàn thiện Hội Đồng Đa Tác Tử từ 9 Chuyên Gia ban đầu lên **Hội Đồng 13 Chuyên Gia Kiệt Xuất**, bao quát trọn vẹn 100% vòng đời phát triển phần mềm Full-Stack: từ Giao diện UI/UX Design System, Ứng dụng Đa nền tảng (Mobile/Web), An ninh Bảo mật Fintech cho đến Chiến lược Kiểm thử Tự động (QA Test Automation).

---

## 2. Nhật Ký Triển Khai Tuần Tự 4 Chuyên Gia Full-Stack

### 🔹 Sprint 07A: Agent 09 — Brad Frost & Don Norman (Atomic Design System & UI/UX Specs)
- **Nghiệp vụ:** Chuẩn hóa Design Tokens (Màu sắc HSL, Typography Inter/Outfit, 4px grid), Phân rã linh kiện 5 cấp bậc *Atoms $\rightarrow$ Molecules $\rightarrow$ Organisms $\rightarrow$ Templates $\rightarrow$ Pages*, Kiểm toán độ tương phản **WCAG 2.1 AA ($\ge 4.5:1$)** và Sinh bản vẽ Wireframe bố cục.
- **Thành phần kỹ thuật:**
  1. `core/09_brad_frost.md`: Prompt đặc tả chuyên sâu Brad Frost & Don Norman.
  2. `core_engine/skills/frost_atomic_design_skill.py`: Python Skill tính toán Design Tokens, Cây phân cấp linh kiện và WCAG Contrast.
  3. `core_engine/council/brad_frost_agent.py`: Thực thể `BradFrostAgent`.
  4. `.agents/skills/council-brad-frost/SKILL.md`: Antigravity IDE Skill (🎨).
  5. `tests/test_agent_09_brad_frost.py`: Bộ 5 test cases độc lập (100% Passed).

---

### 🔹 Sprint 07B: Agent 10 — Dan Abramov & Christopher Chedeau (Cross-Platform Mobile & Client State)
- **Nghiệp vụ:** Kiến trúc Ứng dụng Di động Đa Nền Tảng (iOS/Android/Web), Quản lý Trạng thái Client Store bất biến (Zustand/Redux Slices), Cơ chế cập nhật tức thì **Optimistic UI 0ms** (kèm tự động Rollback) và Hàng đợi ngoại tuyến **Offline-First Sync Queue** với thuật toán Vector Clock.
- **Thành phần kỹ thuật:**
  1. `core/10_dan_abramov.md`: Prompt đặc tả chuyên sâu Dan Abramov & Christopher Chedeau (Vjeux).
  2. `core_engine/skills/abramov_client_state_skill.py`: Python Skill sinh Store Slices, mô phỏng Optimistic UI 3 bước và Offline Queue.
  3. `core_engine/council/dan_abramov_agent.py`: Thực thể `DanAbramovAgent`.
  4. `.agents/skills/council-dan-abramov/SKILL.md`: Antigravity IDE Skill (📱).
  5. `tests/test_agent_10_dan_abramov.py`: Bộ 5 test cases độc lập (100% Passed).

---

### 🔹 Sprint 07C: Agent 11 — Bruce Schneier & OWASP (Fintech Security, AES-256 & STRIDE)
- **Nghiệp vụ:** Kiến trúc An ninh Bảo mật Fintech (Zero Trust), Mô hình hóa mối đe dọa 6 chiều **STRIDE**, Mã hóa dữ liệu nhạy cảm bằng **AES-256-GCM** (IV 12-byte + Auth Tag 16-byte), Vòng đời **JWT Token Rotation** (Access 15p + Refresh 7d httpOnly Cookie) kèm Redis Blacklist và chống lỗ hổng phân quyền IDOR.
- **Thành phần kỹ thuật:**
  1. `core/11_bruce_schneier.md`: Prompt đặc tả chuyên sâu Bruce Schneier & OWASP.
  2. `core_engine/skills/schneier_security_skill.py`: Python Skill phân tích STRIDE, mô phỏng mã hóa AES-256-GCM và JWT Rotation.
  3. `core_engine/council/bruce_schneier_agent.py`: Thực thể `BruceSchneierAgent`.
  4. `.agents/skills/council-bruce-schneier/SKILL.md`: Antigravity IDE Skill (🔒).
  5. `tests/test_agent_11_bruce_schneier.py`: Bộ 5 test cases độc lập (100% Passed).

---

### 🔹 Sprint 07D: Agent 12 — Kent Beck & Martin Fowler (Full-Stack Test Strategy & QA Automation)
- **Nghiệp vụ:** Thiết lập **Kim Tự Tháp Kiểm Thử (Test Pyramid: 70% Unit, 20% Integration, 10% E2E)**, Đo lường chỉ số sức khỏe kiểm thử (Health Score), Tự động sinh kịch bản **Playwright E2E TypeScript**, và Thiết kế hồ sơ kiểm thử tải hiệu năng **k6 Load Testing** (500 VUs, SLA p95 < 200ms).
- **Thành phần kỹ thuật:**
  1. `core/12_kent_beck.md`: Prompt đặc tả chuyên sâu Kent Beck & Martin Fowler.
  2. `core_engine/skills/beck_qa_strategy_skill.py`: Python Skill tính toán Test Pyramid, sinh mã Playwright E2E và hồ sơ k6.
  3. `core_engine/council/kent_beck_agent.py`: Thực thể `KentBeckAgent`.
  4. `.agents/skills/council-kent-beck/SKILL.md`: Antigravity IDE Skill (🧪).
  5. `tests/test_agent_12_kent_beck.py`: Bộ 5 test cases độc lập (100% Passed).

---

## 3. Nâng Cấp Điều Phối Viên & Toàn Bộ Hệ Thống
- `CouncilOrchestratorAgent`: Mở rộng điều phối trọn vẹn 12 Chuyên gia qua 4 pha.
- `OrchestratorRFCSkill`: Sinh Ma Trận Truy Vết Traceability Matrix và kế hoạch triển khai Full-Stack QA trong `04_final_rfc.md`.
- `run_council_session.py`: CLI Runner tích hợp đầy đủ 13 Agent.
- Bộ test toàn diện: **91/91 Unit Tests PASSED 100%**.
