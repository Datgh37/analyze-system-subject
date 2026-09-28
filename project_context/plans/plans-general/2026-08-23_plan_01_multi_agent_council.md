# KẾ HOẠCH 01: XÂY DỰNG HỘI ĐỒNG AGENTS GIẢ LẬP (MULTI-AGENT COUNCIL)
**Ngày ghi nhận:** 2026-08-23  
**Trạng thái:** ĐÃ HOÀN THÀNH (COMPLETED)  

---

## 1. Mục Tiêu
Xây dựng một "Hội đồng Bàn tròn Giả lập" gồm các Persona Khách hàng và Chuyên gia kỹ thuật để phản biện, bóc tách Use Cases và thiết kế kiến trúc Clean Architecture phục vụ môn PTTKHTTT.

## 2. Các Thành Phần Đã Xây Dựng
1. **`AGENTS.md`:** Quy định luật chơi chung và quy trình 4 pha (Khách hàng $\rightarrow$ BA $\rightarrow$ Chuyên gia Kỹ thuật $\rightarrow$ Orchestrator).
2. **Thư mục `core/`:**
   - `00_orchestrator.md`: Chủ tịch hội đồng
   - `01_gen_z_user.md`: Sinh viên lười gõ, test UX
   - `02_group_leader.md`: Thủ quỹ nhóm, case chia tiền oái oăm
   - `03_ba_specialist.md`: Chuẩn hóa Use Case IEEE 29148 & Gherkin
   - `04_software_architect.md`: Clean Architecture & ADR
   - `05_backend_data_specialist.md`: CSDL 3NF & Thuật toán Đồ thị
   - `06_devops_security.md`: Bảo mật JWT & Docker Compose
3. **Thư mục `templates/`:** Bộ 4 templates chuẩn hóa đầu ra.
4. **Thư mục `tools/`:** Script Python thuật toán rút gọn nợ (`graph_debt_solver.py`) và tốc độ chi tiêu (`spending_velocity_calc.py`).
