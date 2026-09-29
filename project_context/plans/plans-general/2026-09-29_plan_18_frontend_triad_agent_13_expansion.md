# 📋 KẾ HOẠCH TRIỂN KHAI PLAN-18: THÀNH LẬP "BỘ BA TAM GIÁC VÀNG FRONTEND" (AGENT 09 - 13 - 10), BỔ SUNG TÁC TỬ JULIAN GARNIER & TÁI PHÂN BỔ 8 KỸ NĂNG MỚI

> **Mã kế hoạch:** `PLAN-18`  
> **Ngày khởi tạo:** 2026-09-29  
> **Người phê duyệt:** Người dùng (User Approval - Phương án 1)  
> **Trọng tâm:** Mở rộng Hội đồng Đa tác tử từ 13 lên 14 Chuyên gia Danh nhân, chuyên biệt hóa toàn diện mảng Frontend (Thị giác & Thương hiệu $\leftrightarrow$ Động lực học & Chuyển động $\leftrightarrow$ Kiến trúc Trạng thái & Code), tích hợp triệt để 8 Antigravity Skills mới phục vụ thiết kế Chương 3 Khóa luận tốt nghiệp.

---

## 1. Bối Cảnh & Động Lực (Context & Motivation)

Trước khi bước vào triển khai sâu nội dung **Mục 3.6 (Thiết kế Giao diện Người dùng)** trong Chương 3 của Khóa luận tốt nghiệp, hệ thống đã nạp thêm 8 bộ kỹ năng Frontend đỉnh cao:
1. `animejs-uiux`: Làm chủ Anime.js v4, spring physics, layout morphing, draggable, 60fps.
2. `brandkit`: Nhận diện thương hiệu, thiết kế logo biểu tượng, brand guidelines board 3x3/2x3.
3. `design-taste-frontend`: Phản biện thiết kế Anti-Slop, 3 dials cân chỉnh, layout rhythm, hero viewport, typography pairings.
4. `high-end-visual-design`: Chuẩn mực Awwwards ($150k+ Agency), Double-Bezel (Doppelrand), Island Button, Fluid Island Nav.
5. `imagegen-frontend-mobile`: Sinh ảnh Mockup ứng dụng di động (iOS/Android) chuẩn native trong khung điện thoại tinh tế.
6. `imagegen-frontend-web`: Sinh ảnh Mockup giao diện Web / Dashboard từng section ngang 16:9/16:10 độc lập.
7. `minimalist-ui`: Tối giản thực dụng Notion/Linear, bảng màu ấm đơn sắc + pastel nhạt, bento grid phẳng 1px.
8. `redesign-existing-projects`: Quy trình kiểm toán (Audit) và nâng cấp giao diện toàn diện 8 khía cạnh.

Hội đồng hiện thời chỉ có 2 Agent phụ trách FE (Agent 09: Brad Frost và Agent 10: Dan Abramov), thiếu vắng một chuyên gia chuyên trách sâu sắc về **Động lực học Chuyển động & Hoạt ảnh Tương tác (Motion Choreography & Interactive Experience)**. Do đó, việc thành lập **Bộ Ba Tam Giác Vàng Frontend** bằng cách bổ sung **Agent 13: Julian Garnier & Bret Victor** là giải pháp tối ưu và chuyên biệt nhất.

---

## 2. Kiến Trúc Bộ Ba Tam Giác Vàng Frontend (The Frontend Triad Architecture)

```text
┌────────────────────────────────────────────────────────────────────────────────────────┐
│                        BỘ BA TAM GIÁC VÀNG THIẾT KẾ FRONTEND (PFM)                     │
├───────────────────────────────┬───────────────────────────────┬────────────────────────┤
│ AGENT 09: BRAD FROST &        │ AGENT 13: JULIAN GARNIER &    │ AGENT 10: DAN ABRAMOV  │
│ JONY IVE (DON NORMAN)         │ BRET VICTOR                   │ & CHRISTOPHER CHEDEAU  │
├───────────────────────────────┼───────────────────────────────┼────────────────────────┤
│ Vai trò: Lead Visual & Brand  │ Vai trò: Lead Motion          │ Vai trò: Lead Frontend │
│ Art Director (Design System)  │ Choreographer & Experience    │ Architect & Client State│
├───────────────────────────────┼───────────────────────────────┼────────────────────────┤
│ Bộ kỹ năng phụ trách:         │ Bộ kỹ năng phụ trách:         │ Bộ kỹ năng phụ trách:  │
│ • brandkit                    │ • animejs-uiux                │ • design-taste-frontend│
│ • imagegen-frontend-mobile    │ • high-end-visual-design      │ • redesign-existing-   │
│ • imagegen-frontend-web       │   (Doppelrand, Fluid Nav,     │   projects             │
│ • minimalist-ui               │    Spring Physics, Islands)   │ • abramov_client_state │
│ • frost_atomic_design         │ • garnier_motion_skill        │                        │
├───────────────────────────────┼───────────────────────────────┼────────────────────────┤
│ Đầu ra cốt lõi:               │ Đầu ra cốt lõi:               │ Đầu ra cốt lõi:        │
│ 1. Bộ nhận diện Brand Identity│ 1. Bản đặc tả hoạt ảnh        │ 1. Lược đồ Client Store│
│ 2. Bảng Design Tokens & Atomic│    (Motion & Micro-physics)   │    (Zustand Slices)    │
│    Component Hierarchy        │ 2. Mô hình gia tốc lò xo      │ 2. Cơ chế Optimistic UI│
│ 3. Bộ ảnh Mockup chất lượng   │    (Spring Parameters)        │    phản hồi trong 0ms  │
│    cao (Mobile Apps & Web)    │ 3. Chuyển đổi trạng thái FLIP │ 3. Hàng đợi Offline-   │
│ 4. Báo cáo kiểm toán độ tương │    Layout Morphing (Grid/List)│    first (Vector Clock)│
│    phản WCAG 2.1 AA (≥ 4.5:1) │ 4. Kiểm soát 60fps GPU-safe   │ 4. Kiểm toán Code Taste│
└───────────────────────────────┴───────────────────────────────┴────────────────────────┘
```

---

## 3. Các Giai Đoạn Triển Khai Chi Tiết (Implementation Roadmap)

### Giai Đoạn 1: Đặc Tả Persona Danh Nhân & Antigravity Skills
1. **Nâng cấp `core/09_brad_frost.md`:** Bổ sung hóa thân **Jony Ive** (cựu Giám đốc Thiết kế Apple), mở rộng phạm vi từ Atomic Design sang **Brand Art Direction & High-Fidelity Visual Mockup Generation**; liên kết các skill `brandkit`, `imagegen-frontend-mobile`, `imagegen-frontend-web`, `minimalist-ui`.
2. **Khởi tạo `core/13_julian_garnier.md`:** Đặc tả Persona **Julian Garnier** (Tác giả Anime.js) & **Bret Victor** (Huyền thoại Human-Interface của Apple). Trọng tâm: Motion Choreography, Spring physics, Micro-interactions, FLIP layout morphing, Double-Bezel nested architecture, Fluid Island nav.
3. **Nâng cấp `core/10_dan_abramov.md`:** Tích hợp bộ quy tắc Anti-Slop Frontend Taste, kỷ luật bố cục (`Hero viewport`, `eyebrow restraint`, `button wrap ban`) và quy trình kiểm toán 8 khía cạnh `redesign-existing-projects`.
4. **Khởi tạo Antigravity Skill `.agents/skills/council-julian-garnier/SKILL.md`:** Chuẩn hóa YAML Frontmatter, bảng Menu tương tác (Mã lệnh `[1] / MOTN`, `[2] / SPRG`, `[3] / FLIP`, `[4] / 60FP`) và liên kết Python Engine.
5. **Cập nhật `.agents/skills/council-brad-frost/SKILL.md` và `.agents/skills/council-dan-abramov/SKILL.md`:** Đồng bộ vai trò và menu kỹ năng mới.

### Giai Đoạn 2: Xây Dựng Bộ Nhân Python Engine & Tích Hợp Pha 2.5
1. **Xây dựng Python Skill `core_engine/skills/garnier_motion_skill.py`:**
   - Hàm `generate_motion_choreography_spec(component_name, interaction_type)`: Sinh đặc tả thông số chuyển động, đường cong `cubic-bezier` hoặc `spring({ bounce, mass, stiffness, damping })`.
   - Hàm `calculate_spring_physics(bounce, duration)`: Tính toán hệ số vật lý lò xo chuẩn.
   - Hàm `generate_flip_layout_spec(from_layout, to_layout)`: Đặc tả chuyển đổi cấu trúc DOM (Grid $\leftrightarrow$ List) theo nguyên lý FLIP với Anime.js v4.
   - Hàm `audit_60fps_gpu_performance(animated_properties)`: Kiểm toán chỉ cho phép can thiệp `transform` và `opacity`, cảnh báo nếu vi phạm layout repaint.
2. **Xây dựng Agent Class `core_engine/council/julian_garnier_agent.py`:**
   - Kế thừa `PersonaAgent`, kết nối `garnier_motion_skill.py` vào `tools_map`, hiện thực hóa hàm `step(state)`.
3. **Cập nhật `core_engine/council/__init__.py` & `core_engine/skills/__init__.py`:** Xuất bản `JulianGarnierAgent` và `GarnierMotionSkill`.
4. **Nâng cấp `CouncilOrchestratorAgent` (`core_engine/council/council_orchestrator_agent.py`):**
   - Khởi tạo `self.julian_garnier = JulianGarnierAgent(...)`.
   - Cập nhật Pha 2.5 trong `run_full_session`: Phối hợp cả 3 tác tử:
     ```python
     state = self.brad_frost.step(state)
     state = self.julian_garnier.step(state)
     state = self.dan_abramov.step(state)
     ```
   - Tổng hợp nội dung motion vào tệp sản phẩm bàn giao `02b_frontend_ui_spec.md`.

### Giai Đoạn 3: Cập Nhật Quy Chuẩn Quản Trị Hệ Thống (Governance & Master Context)
1. **Cập nhật `AGENTS.md`:**
   - Nâng tổng số tác tử từ 13 lên **14 Tác tử Full-Stack (Agent 00 đến Agent 13)**.
   - Bổ sung Agent 13 vào bảng danh mục và cập nhật mô tả quy trình Pha 2.5.
2. **Cập nhật `project_context/00_MASTER_CONTEXT.md`:**
   - Cập nhật Mục 3 Roster 14 Chuyên Gia Danh Nhân.
   - Cập nhật Trụ cột 2 phản ánh 14 Kỹ năng Hội đồng + 8 Kỹ năng Frontend mở rộng.
3. **Cập nhật `templates/frontend_ui_spec_template.md`:** Bổ sung Mục đặc tả Động lực học & Chuyển động (Motion & Micro-interactions Specifications).

### Giai Đoạn 4: Bộ Kiểm Thử Tự Động Toàn Diện (100% Unit Tests Pass)
1. **Khởi tạo `tests/test_agent_13_julian_garnier.py`:** Kiểm thử độc lập `JulianGarnierAgent` và `GarnierMotionSkill`.
2. **Cập nhật `tests/test_antigravity_skills.py`:** Thêm `council-julian-garnier` vào danh sách bắt buộc, kiểm tra YAML frontmatter, tính khả chuyển và liên kết Python engine.
3. **Cập nhật `tests/test_agent_00_orchestrator.py` & `tests/test_frontend_ui_spec_generation.py`:** Đảm bảo chu trình 4 pha của Orchestrator tích hợp trơn tru Agent 13.
4. **Chạy kiểm thử toàn bộ hệ thống:** `python -B -m unittest discover tests` đảm bảo đạt tỷ lệ vượt qua tuyệt đối 100%.

### Giai Đoạn 5: Ghi Nhật Ký Quyết Định Kiến Trúc (Mandatory Context Logging)
1. Cập nhật `project_context/decisions_and_changes_log.md` với **Quyết Định 73**.
2. Sẵn sàng khởi động thiết kế chi tiết Giao diện Người dùng phục vụ Mục 3.6 Chương 3.

---
*Kế hoạch đã sẵn sàng để tiến hành thực thi từng bước một cách chuẩn xác.*
