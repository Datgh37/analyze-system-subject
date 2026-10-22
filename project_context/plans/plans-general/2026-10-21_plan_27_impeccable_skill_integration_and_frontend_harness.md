# 📋 KẾ HOẠCH TRIỂN KHAI: TÍCH HỢP BỘ SKILL IMPECCABLE VÀO ANTIGRAVITY AGENTS
## (IMPLEMENTATION PLAN: IMPECCABLE AI DESIGN HARNESS INTEGRATION)

> **Mã kế hoạch:** `PLAN-27` (General Roadmap)  
> **Thời điểm:** 21/10/2026  
> **Trạng thái:** `COMPLETED & ACTIVE - ĐÃ TRIỂN KHAI THÀNH CÔNG`  
> **Chủ trì:** Agent 00 (Council Orchestrator) kết hợp Bộ Ba Tam Giác Vàng Frontend (Agent 09, 10, 13)  
> **Tài liệu tham chiếu:** Repository [pbakaus/impeccable](https://github.com/pbakaus/impeccable), [`AGENTS.md`](../../AGENTS.md), [`mobile-project/dotnet-maui-ver/README.md`](../../mobile-project/dotnet-maui-ver/README.md).

---

## 1. MỤC TIÊU VÀ PHẠM VI (OBJECTIVES & SCOPE)

1. **Trang bị bộ kỹ năng chuyên nghiệp Impeccable v4.7+:**
   - Đưa trọn bộ 25 lệnh thiết kế chuyên sâu (`craft`, `audit`, `critique`, `polish`, `distill`, `harden`, `adapt`, `animate`, `colorize`, `typeset`, `layout`...) và 61 quy tắc kiểm định chống lỗi rập khuôn AI (AI Tells / Clichés) vào hệ thống Agent của Antigravity IDE.
2. **Ủy nhiệm quyền năng cho Bộ Ba Tam Giác Vàng Frontend:**
   - **Agent 09 (Brad Frost - Art Director):** Nắm giữ các lệnh tạo lập ngôn ngữ thị giác, tokens, bảng màu có hòa sắc (`colorize`), kiểu chữ (`typeset`) và quy tắc chống "Card Soup".
   - **Agent 10 (Dan Abramov - Client Architect):** Nắm giữ các lệnh gia cố chống vỡ layout (`harden`), co giãn đa màn hình (`adapt`), tối giản tinh gọn (`distill`) và kiểm định chất lượng (`audit`).
   - **Agent 13 (Julian Garnier - Motion Choreographer):** Nắm giữ các lệnh điều phối chuyển động có chủ đích (`animate`), loại bỏ hiệu ứng nảy lỗi thời, áp dụng Spring Physics.
3. **Thiết lập nền tảng thiết kế bền vững (Durable Design Context):**
   - Thiết lập `PRODUCT.md` và `DESIGN.md` cho phân hệ .NET MAUI để các Agent luôn ghi nhớ đối tượng người dùng, tính cách sản phẩm tài chính và các quy định thẩm mỹ bất di bất dịch.

---

## 2. ĐÁNH GIÁ RỦI RO & GIẢI PHÁP KIỂM SOÁT (RISK ASSESSMENT)

| Rủi Ro Nhận Diện | Tác Động Tiềm Ẩn | Giải Pháp Kiểm Soát & Khắc Phục |
| :--- | :--- | :--- |
| **1. Làm loãng Context Window** | Nạp hàng loạt tài liệu mô tả vào ngữ cảnh làm tràn bộ nhớ LLM. | Áp dụng cơ chế **On-demand Loading**: Skill chỉ được nạp khi có yêu cầu liên quan đến giao diện UI/UX hoặc gọi lệnh `/impeccable`. Các playbook nằm trong `reference/` chỉ đọc khi cần. |
| **2. Xung đột Rules KLTN DNTU** | Nhầm lẫn giữa quy định thiết kế UI và quy định trình bày Word. | **Cách ly phạm vi nghiêm ngặt (Strict Role Isolation):** Impeccable chỉ chi phối tầng giao diện ứng dụng (XAML / Mobile Screens), tuyệt đối không can thiệp vào các quy định format Khóa luận Tốt nghiệp trong Word/PDF. |
| **3. Khả năng tương thích nền tảng Windows** | Lệnh nhị phân launcher có thể gặp lỗi phân quyền hoặc thiếu Node. | Impeccable hỗ trợ cơ chế **Launcher Fallback**: Nếu binary không chạy được trên Windows, Agent sẽ đọc trực tiếp các playbook Markdown trong thư mục `reference/` mà không bị gián đoạn công việc. |

---

## 3. CÁC BƯỚC TRIỂN KHAI CHI TIẾT (STEP-BY-STEP IMPLEMENTATION)

### BƯỚC 1: Cài đặt và Tích hợp Gói Kỹ Năng vào `.agents/skills/impeccable/`
- Tải và cấu hình đầy đủ cấu trúc skill Impeccable chuẩn Antigravity / Gemini IDE:
  ```text
  .agents/skills/impeccable/
  ├── SKILL.md                          # File chỉ dẫn chính với YAML frontmatter chuẩn
  ├── scripts/                          # Launcher binary/cmd cho Windows
  │   ├── impeccable
  │   └── impeccable.cmd
  └── reference/                        # Trọn bộ 25 Playbooks chuyên sâu
      ├── craft-floor.md                # Sàn chất lượng và các lệnh cấm tuyệt đối
      ├── audit.md                      # Kịch bản kiểm toán kỹ thuật a11y, responsive
      ├── critique.md                   # Kịch bản phản biện trải nghiệm người dùng
      ├── polish.md                     # Kịch bản rà soát căn chỉnh chi tiết cuối cùng
      ├── harden.md                     # Kịch bản xử lý lỗi biên, text overflow
      ├── distill.md                    # Kịch bản lược bỏ chi tiết rườm rà
      ├── adapt.md                      # Kịch bản co giãn đa thiết bị (Phone/Tablet/Desktop)
      ├── animate.md                    # Kịch bản chuyển động vật lý có chủ đích
      ├── colorize.md                   # Kịch bản hòa sắc thương hiệu (Color tinting)
      ├── typeset.md                    # Kịch bản phân cấp kiểu chữ
      └── new-work.md                   # Kịch bản khởi tạo màn hình mới
  ```

### BƯỚC 2: Cấu hình Slash Commands trong `.agents/workflows/`
- Tạo các workflows tiện ích để người dùng và AI có thể gọi nhanh:
  - `.agents/workflows/impeccable.md`: Lệnh tổng `/impeccable <subcommand> <target>`.
  - `.agents/workflows/ui-audit.md`: Lệnh tắt kiểm định chất lượng giao diện.
  - `.agents/workflows/ui-polish.md`: Lệnh tắt đánh bóng và hoàn thiện giao diện.

### BƯỚC 3: Cập nhật Bản Đồ Quyền Năng Hội Đồng (`AGENTS.md`)
- Cập nhật bảng phân công nhiệm vụ và quyền năng ECC trong [`AGENTS.md`](../../AGENTS.md):
  - Bổ sung `impeccable (craft, audit, polish, harden, distill, adapt)` vào danh mục kỹ năng được ủy nhiệm của Agent 09, Agent 10 và Agent 13.
  - Cập nhật các file skill tương ứng: `.agents/skills/council-brad-frost/SKILL.md`, `.agents/skills/council-dan-abramov/SKILL.md`, `.agents/skills/council-julian-garnier/SKILL.md`.

### BƯỚC 4: Khởi tạo Nền Tảng Thiết Kế Cho Dự Án .NET MAUI
- Khởi tạo 2 tệp sự thật thiết kế (Design Source of Truth) tại thư mục `mobile-project/dotnet-maui-ver/`:
  - `PRODUCT.md`: Ghi nhận chân dung khách hàng mục tiêu, các bài toán tài chính cốt lõi, giọng văn và các ràng buộc nghiệp vụ.
  - `DESIGN.md`: Ghi nhận bảng Design Tokens chuẩn mực (Bảng màu Deep Indigo có tinting, Nhịp điệu khoảng trắng, Typography Hierarchy, Thước đo bo góc Double-Bezel, Luật cấm Card Soup).

### BƯỚC 5: Thẩm Định Tính Toàn Vẹn & Sẵn Sàng (Verification & Health Check)
- Chạy lệnh kiểm tra detector:
  ```powershell
  npx impeccable detect mobile-project/dotnet-maui-ver/
  ```
- Kiểm tra danh mục kỹ năng trong Antigravity IDE đảm bảo `impeccable` hiển thị đầy đủ và sẵn sàng được kích hoạt.

---

## 4. KẾT QUẢ ĐẦU RA MONG ĐỢI (DELIVERABLES)
1. Thư mục skill `.agents/skills/impeccable/` hoàn chỉnh 100%.
2. Tệp điều phối `AGENTS.md` được liên kết đồng bộ với bộ kỹ năng mới.
3. Tệp `PRODUCT.md` và `DESIGN.md` chuẩn bị sẵn sàng cho các màn hình .NET MAUI XAML.
4. Hội đồng bước vào phiên họp kỹ thuật tiếp theo với đầy đủ năng lực thiết kế đỉnh cao.
