---
name: council-session-runner
description: Trình khởi chạy toàn bộ Phiên Bàn Thảo 4 Pha (Full Multi-Agent Session Runner). Dùng khi muốn tự động hóa từ A đến Z toàn bộ quy trình hội đồng cho một tính năng mới.
---

# 🚀 Council Session Runner — Khởi Chạy Trọn Vẹn Phiên 4 Pha

## 1. Tổng Quan & Vai Trò (Overview)
Kỹ năng này đóng vai trò là **Bộ Điều Khiển Trung Tâm (Master Execution Pipeline)** để tự động hóa toàn bộ quy trình phân tích thiết kế cho một Tính năng Mới (Feature Request) xuyên suốt qua 4 pha của Hội đồng Đa tác tử.

---

## 2. Quy Trình Vận Hành 4 Pha Tự Động

```text
[BƯỚC 1: NHẬN ĐỀ BÀI] ──► [PHA 1: KHÁCH HÀNG] ──► [PHA 2: LEAD BA]
                             (Thaler + Group)         (Cockburn & Paul)
                                     │                        │
                                     ▼                        ▼
[BƯỚC 4: FINAL RFC] ◄─── [PHA 4: TỔNG HỢP] ◄─── [PHA 3: 5 KIẾN TRÚC SƯ]
(Sẵn sàng lập trình)    (Council Orchestrator)    (Evans, Kleppmann, Tarjan,
                                                   Vogels, Andrew Ng)
```

---

## 3. Menu Tương Tác (Capabilities Menu)

Khi được kích hoạt, hãy chào người dùng với biểu tượng 🚀 và trình bày Menu lựa chọn sau:

| Mã (Code) | Thao Tác | Hành Động Kỹ Thuật |
| :---: | :--- | :--- |
| **`[1]` / `NEW`** | **Tạo Phiên Mới** | Nhập tên tính năng $\rightarrow$ Tạo thư mục `sessions/session_[tên]/` $\rightarrow$ Chạy trọn vẹn 4 pha. |
| **`[2]` / `STEP`**| **Chạy Từng Pha Một** | Cho phép người dùng dừng lại review kết quả sau mỗi pha trước khi sang pha kế tiếp. |
| **`[3]` / `STAT`**| **Xem Trạng Thái Phiên** | Quét các thư mục trong `sessions/` và in ra bảng ma trận tiến độ hoàn thành. |
| **`[4]` / `VERF`**| **Kiểm Tra Tính Toàn Vẹn** | Chạy `verify_session_integrity()` để chấm điểm chất lượng tài liệu phiên. |

---

## 4. Liên Kết Động Cơ Python (Python Engine Bridge)
Kỹ năng này trực tiếp điều phối `CouncilOrchestratorAgent` và `OrchestratorRFCSkill`:
```python
from core_engine.council.council_orchestrator_agent import CouncilOrchestratorAgent

# Khởi chạy tự động toàn bộ phiên
final_state = orchestrator.run_full_session(
    session_dir_path="sessions/session_[tên_tính_năng]",
    task_description="[Mô tả tính năng]"
)
```
