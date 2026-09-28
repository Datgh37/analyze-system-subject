# KẾ HOẠCH 02: XÂY DỰNG BỘ NHÂN AI SOFTWARE ENGINEER (CORE ENGINE)
**Ngày ghi nhận:** 2026-08-23  
**Trạng thái:** ĐÃ HOÀN THÀNH VÀ VƯỢT QUA 16/16 BÀI TEST (COMPLETED & VERIFIED)  

---

## 1. Mục Tiêu
Hiện thực hóa tài liệu `ai_software_engineering_agent_project_spec.md` thành một bộ nhân Python hoàn chỉnh (`core_engine/`) theo mô hình phân tầng Bottom-Up và nguyên tắc sản xuất của Anthropic/Claude.

## 2. Các Tầng Kiến Trúc Đã Hoàn Thiện
1. **Tool Layer (`core_engine/tools/`):**
   - `filesystem.py`: List, Read, Search, Write, Edit kèm Path Sandboxing chống Path Traversal.
   - `terminal.py`: Safe command validator, timeout protection, cờ `-B` chống stale bytecode cache.
   - `git_tools.py`: Git status, diff, commit, rollback.
2. **State & Protocol (`core_engine/state.py`, `llm_client.py`):**
   - Dataclass `ProjectState` có cấu trúc, lưu vết `tool_history`.
   - Tool calling adapter & `MockLLMClient` cho kiểm thử deterministic.
3. **Agentic Loop (`core_engine/agents/`):**
   - `coding_agent.py`: Vòng lặp `Inspect -> Modify -> Test -> Self-Correction`.
   - `requirement_agent.py`: Trích xuất FR có cấu trúc.
   - `planner_agent.py`: Lập kế hoạch từng bước.
   - `tester_agent.py`: Chạy test suite và phân tích failure.
   - `reviewer_agent.py`: Soi Clean Architecture & Security.
4. **State Machine & Orchestrator (`core_engine/workflow/`):**
   - `orchestrator.py`: Quản lý chuyển pha `NEW -> ANALYZING -> PLANNING -> IMPLEMENTING -> TESTING -> REVIEWING -> DONE`.
   - `loop_controller.py`: Circuit Breaker `MAX_REPAIR_ITERATIONS = 5`.
5. **Memory & Evaluation (`core_engine/memory/`, `core_engine/evaluation/`):**
   - `project_memory.py`: Lưu trữ kiến trúc và conventions.
   - `metrics.py`: Tự động thu thập và xuất bảng chỉ số thực nghiệm chuẩn khoa học cho ĐATN.

## 3. Kết Quả Kiểm Thử
- **16/16 Unit & Integration Tests Passed:**
  - `tests/test_tools.py`: 9/9 tests
  - `tests/test_coding_agent.py`: 2/2 tests
  - `tests/test_orchestrator.py`: 2/2 tests
  - `tests/test_multi_agent.py`: 1/1 test
  - `tests/test_evaluation.py`: 2/2 tests
