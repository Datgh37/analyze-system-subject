# KẾ HOẠCH TỔNG THỂ 05: NÂNG CẤP PRODUCTION-READY, TRACEABILITY MATRIX, CLI RUNNER & BỘ 4 BENCHMARK SESSIONS
**Thời điểm ghi nhận:** 2026-08-24 22:42  
**Trạng thái:** ĐÃ HOÀN THÀNH & KIỂM THỬ XÁC MINH 100% (COMPLETED & VERIFIED - 71 TESTS OK)  

---

## 1. Mục Tiêu
Chuyển hóa toàn bộ hệ thống từ trạng thái "Khung mã nguồn hoàn chỉnh" sang trạng thái **"Production-Ready & Sẵn sàng Bảo vệ ĐATN"**:

1. Tự động sinh **Ma Trận Truy Vết (Traceability Matrix: REQ $\rightarrow$ UC $\rightarrow$ ADR $\rightarrow$ DB $\rightarrow$ Test)**.
2. Tăng cường **Deep Content Quality Gates** (soi sâu nội dung: Gherkin, luồng lỗi, Trade-offs, DECIMAL).
3. Chuẩn hóa ngày giờ động (`datetime.now()`) và nạp linh hoạt **OpenAI / Gemini / Ollama / Mock LLM**.
4. Xây dựng Script dòng lệnh chính thức **`run_council_session.py`**.
5. Sinh trọn vẹn **4 Bộ Hồ Sơ Tính Năng Thực Tế (Benchmark Sessions)** trong `sessions/` kẹp vào báo cáo đồ án.

## 2. Các Thành Phần Kỹ Thuật
1. `core_engine/skills/orchestrator_rfc_skill.py`: Bổ sung `generate_traceability_matrix`, `verify_content_quality_gates`, dynamic timestamp.
2. `core_engine/llm_client.py`: Bổ sung `create_llm_client` hỗ trợ OpenAI, Gemini, Ollama, Mock.
3. `run_council_session.py`: CLI Runner tương tác trực quan.
4. `tests/test_production_readiness.py`: Bộ kiểm thử tự động cho toàn bộ tính năng mới.
5. `sessions/`: Tạo 4 thư mục benchmark (`session_01_household_fund`, `session_02_roommate_utilities`, `session_03_trip_ocr_itemized_split`, `session_04_spending_velocity_nudge`).
