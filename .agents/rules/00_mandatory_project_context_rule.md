# 🧭 QUY TẮC BẮT BUỘC: NẠP NGỮ CẢNH & GHI NHẬT KÝ DỰ ÁN (MANDATORY CONTEXT & LOGGING RULES)

> [!IMPORTANT]
> **BẮT BUỘC THỰC HIỆN TRONG MỌI PHIÊN LÀM VIỆC (CONVERSATION):**
> Áp dụng cho mọi tác tử AI và công cụ trong Antigravity IDE / Gemini khi tương tác với workspace này.

---

## 1. NẠP NGỮ CẢNH DỰ ÁN ĐẦU TIÊN (MANDATORY CONTEXT INITIALIZATION)

Trước khi thực hiện bất kỳ phân tích, lập trình, thiết kế hay đề xuất nào, Agent BẮT BUỘC PHẢI NẠP NGỮ CẢNH từ thư mục `project_context/` đầu tiên:

1. **Tệp định hướng toàn cục:** Đọc [`project_context/00_MASTER_CONTEXT.md`](project_context/00_MASTER_CONTEXT.md) để nắm:
   - Phạm vi đề tài: **100% Quản lý Tài chính Cá nhân Thông minh (Tích hợp AI)**.
   - Danh sách 13 Chuyên gia Danh nhân trong Hội đồng (Đặc biệt: Agent 02 là Personal Budget Coach, Agent 06 là Personal Debt & Cashflow Optimizer).
   - Hai trụ cột kỹ thuật: Nghiệp vụ PFM (50/30/20, ZBB, Kakeibo, Đa ví ACID, Snowball/Avalanche, OCR đa món & kiểm toán số học, Spending Velocity) và Bộ nhân SWE đa tác tử.
2. **Tệp nhật ký quyết định kiến trúc:** Đọc [`project_context/decisions_and_changes_log.md`](project_context/decisions_and_changes_log.md) để nắm các quyết định đã khóa (đặc biệt là Quyết định 09 về Scope Pivot sang cá nhân, Quyết định 21 về Đại hợp nhất Use Case, Quyết định 22 về Draw.io 10 Tabs, Quyết định 23 về chuẩn hóa KLTN DNTU).
3. **Nguyên tắc bất di bất dịch:** Tuyệt đối KHÔNG giả định quay lại các bài toán chia tiền nhóm, quỹ chung gia đình hay nợ phòng trọ cũ. Mọi Use Case, CSDL, giao diện và mã nguồn đều phải xoay quanh bài toán tài chính cá nhân chuyên sâu.

---

## 2. GHI NHẬT KÝ KẾ HOẠCH & QUYẾT ĐỊNH BẮT BUỘC (MANDATORY CONTEXT & DECISION LOGGING)

Mỗi khi có một kế hoạch mới, điều chỉnh phạm vi, sửa đổi yêu cầu nghiệp vụ hoặc đưa ra quyết định kỹ thuật / kiến trúc quan trọng, Agent **BẮT BUỘC** phải thực hiện 2 hành động ghi chép bền vững sau đây:

### 2.1. Lưu Snapshot Kế Hoạch Vào `project_context/plans/`
- Khi tạo mới hoặc cập nhật lớn một kế hoạch thực thi (Implementation Plan / Planning Phase), Agent phải lưu một bản sao snapshot vào thư mục `project_context/plans/`.
- **Quy tắc đặt tên tệp:** `YYYY-MM-DD_plan_[XX]_[ten_ngan_gon_viet_thuong_khong_dau].md` (ví dụ: `2026-09-11_plan_11_chuan_hoa_kltn_docx_va_chuong_2.md`).
- **Nội dung snapshot:** Mục tiêu, phạm vi thay đổi, các nguyên tắc ràng buộc, danh sách tệp bị tác động và kế hoạch kiểm thử xác minh.

### 2.2. Cập Nhật Nhật Ký Quyết Định Vào `project_context/decisions_and_changes_log.md`
- Mỗi khi người dùng phê duyệt kế hoạch hoặc khi hệ thống khóa lại một quyết định kiến trúc / kỹ thuật / nghiệp vụ, Agent phải ghi nhận một mục nhật ký mới ở cuối tệp `project_context/decisions_and_changes_log.md`.
- **Cấu trúc mục nhật ký chuẩn:**
  ```markdown
  ---

  ## [YYYY-MM-DD HH:MM] - Quyết Định [N]: [Tiêu Đề Quyết Định Viết Hoa Đầu Từ]
  - **Vấn đề:** Nêu rõ bối cảnh, lý do phát sinh yêu cầu hoặc xung đột cần giải quyết.
  - **Quyết định / Sửa đổi yêu cầu:**
    1. Chi tiết hành động và giải pháp kỹ thuật đã thống nhất.
    2. Các tệp/thành phần bị tác động trực tiếp.
  - **Trạng thái:** `ACCEPTED & LOCKED`.
  ```
- **Nguyên tắc Durable Context:** Không lưu lại quyết định vào trí nhớ tạm thời; mọi quyết định đều phải được ghi thành văn bản Markdown để các phiên làm việc tiếp theo luôn đọc và kế thừa chính xác.
