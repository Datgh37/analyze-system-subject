"""
Module: tests/test_agent_00_orchestrator.py
Mục đích: Kiểm thử độc lập cho Agent 00 - Council Orchestrator & Chu trình 4 Pha Hội Đồng
"""

import sys
import unittest
import tempfile
import shutil
from pathlib import Path

sys.path.insert(0, str(Path(__file__).parent.parent))

from core_engine.state import ProjectState
from core_engine.tools.filesystem import FilesystemTools
from core_engine.tools.terminal import TerminalTools
from core_engine.tools.git_tools import GitTools
from core_engine.llm_client import MockLLMClient
from core_engine.skills.orchestrator_rfc_skill import OrchestratorRFCSkill
from core_engine.council.council_orchestrator_agent import CouncilOrchestratorAgent


class TestOrchestratorRFCSkill(unittest.TestCase):
    def setUp(self):
        self.test_dir = tempfile.mkdtemp(prefix="rfc_skill_test_")
        self.session_path = Path(self.test_dir)

    def tearDown(self):
        shutil.rmtree(self.test_dir, ignore_errors=True)

    def test_compile_final_rfc(self):
        (self.session_path / "01_user_feedback.md").write_text("### Phản biện người dùng\n- Giảm UX Friction", encoding="utf-8")
        (self.session_path / "02_use_case_specs.md").write_text("### Use Case Specs\n- UC-01: Quản trị Quỹ", encoding="utf-8")
        (self.session_path / "03_technical_adr.md").write_text("### Quyết định Kỹ thuật\n- Clean Architecture 3NF", encoding="utf-8")

        rfc_text = OrchestratorRFCSkill.compile_final_rfc(
            str(self.session_path),
            feature_title="Tính Năng Quản Lý Quỹ Gia Đình"
        )

        self.assertIn("RFC: TÍNH NĂNG QUẢN LÝ QUỸ GIA ĐÌNH", rfc_text)
        self.assertIn("Giảm UX Friction", rfc_text)
        self.assertIn("UC-01: Quản trị Quỹ", rfc_text)
        self.assertIn("Clean Architecture 3NF", rfc_text)
        self.assertTrue((self.session_path / "04_final_rfc.md").exists())

    def test_verify_session_integrity_valid(self):
        # Đầy đủ 5 file nội dung hợp lệ
        for f in ["01_user_feedback.md", "02_use_case_specs.md", "02b_frontend_ui_spec.md", "03_technical_adr.md", "04_final_rfc.md"]:
            (self.session_path / f).write_text("Đây là nội dung hợp lệ đạt chuẩn trên 50 ký tự cho phiên làm việc hoàn tất.", encoding="utf-8")

        report = OrchestratorRFCSkill.verify_session_integrity(str(self.session_path))
        self.assertTrue(report["is_valid"])
        self.assertEqual(report["integrity_score"], 100)
        self.assertEqual(report["verdict"], "PHIÊN BÀN THẢO HỢP LỆ & TOÀN VẸN 100%")

    def test_verify_session_integrity_missing_files(self):
        # Thiếu 02_use_case_specs.md
        (self.session_path / "01_user_feedback.md").write_text("Nội dung hợp lệ.", encoding="utf-8")

        report = OrchestratorRFCSkill.verify_session_integrity(str(self.session_path))
        self.assertFalse(report["is_valid"])
        self.assertIn("02_use_case_specs.md", report["missing_files"])
        self.assertLess(report["integrity_score"], 100)

    def test_format_session_summary_table(self):
        table = OrchestratorRFCSkill.format_session_summary_table({})
        self.assertIn("01_user_feedback.md", table)
        self.assertIn("02b_frontend_ui_spec.md", table)
        self.assertIn("04_final_rfc.md", table)
        self.assertIn("Pha 4", table)


class TestCouncilOrchestratorAgent(unittest.TestCase):
    def setUp(self):
        self.test_dir = tempfile.mkdtemp(prefix="orchestrator_agent_test_")
        self.workspace = Path(self.test_dir)

        (self.workspace / "core").mkdir()
        for i, name in enumerate([
            "00_orchestrator.md",
            "01_richard_thaler.md",
            "02_group_leader.md",
            "03_cockburn_ba.md",
            "04_eric_evans.md",
            "05_martin_kleppmann.md",
            "06_tarjan_graph.md",
            "07_werner_vogels.md",
            "08_andrew_ng.md",
            "09_brad_frost.md",
            "10_dan_abramov.md",
            "11_bruce_schneier.md",
            "12_kent_beck.md",
            "13_julian_garnier.md",
        ]):
            (self.workspace / "core" / name).write_text(f"Spec agent {name}", encoding="utf-8")

        self.fs = FilesystemTools(workspace_root=str(self.workspace))
        self.term = TerminalTools(workspace_root=str(self.workspace))
        self.git = GitTools(workspace_root=str(self.workspace))

    def tearDown(self):
        shutil.rmtree(self.test_dir, ignore_errors=True)

    def test_run_full_session_flow(self):
        # Mock LLM trả về kết quả tuần tự cho 13 Agent + 1 Orchestrator
        mock_responses = [
            {"content": "[INSIGHT]: Tâm lý chi tiêu cần giảm ma sát thao tác.", "tool_calls": []}, # Thaler
            {"content": "[INSIGHT]: Quỹ chung cần hỗ trợ đóng theo tỷ lệ thu nhập.", "tool_calls": []}, # Group Leader
            {"content": "[IEEE_29148_USE_CASE_SPEC]: Use Case Quản Trị Quỹ Chung.", "tool_calls": []}, # Cockburn BA
            {"content": "[ATOMIC_DESIGN_UI_SPEC]: Design Tokens HSL & Component Tree.", "tool_calls": []}, # Brad Frost
            {"content": "[MOTION_CHOREOGRAPHY_SPEC]: Anime.js v4 Spring Physics & Doppelrand.", "tool_calls": []}, # Julian Garnier
            {"content": "[CLIENT_STATE_OPTIMISTIC_UI]: Zustand Store & Optimistic UI 0ms.", "tool_calls": []}, # Dan Abramov
            {"content": "[CLEAN_ARCHITECTURE_ADR]: Domain Core với Money Decimal.", "tool_calls": []}, # Evans
            {"content": "[DATABASE_3NF_POLYGLOT_SPEC]: PostgreSQL 3NF & Pessimistic Lock.", "tool_calls": []}, # Kleppmann
            {"content": "[GRAPH_DEBT_SOLVER_SPEC]: Thuật toán đồ thị O(V log V).", "tool_calls": []}, # Tarjan
            {"content": "[ASYNC_DEVOPS_ADR]: Message Queue 202 Accepted & Idempotency.", "tool_calls": []}, # Vogels
            {"content": "[MULTIMODAL_AI_PIPELINE_ADR]: Pipeline OCR & Mathematical Audit.", "tool_calls": []}, # Andrew Ng
            {"content": "[FINTECH_SECURITY_STRIDE_ADR]: AES-256-GCM & STRIDE Analysis.", "tool_calls": []}, # Bruce Schneier
            {"content": "[FULL_STACK_QA_TEST_STRATEGY_SPEC]: Test Pyramid (70-20-10) & Playwright.", "tool_calls": []}, # Kent Beck
            {"content": "[COUNCIL_FINAL_APPROVAL]: Hội đồng phê duyệt 100% Final RFC.", "tool_calls": []}, # Orchestrator
        ]
        mock_llm = MockLLMClient(mock_responses)

        orchestrator = CouncilOrchestratorAgent(mock_llm, self.fs, self.term, self.git)
        session_dir = self.workspace / "sessions" / "session_household_fund"

        state = orchestrator.run_full_session(
            session_dir_path=str(session_dir),
            task_description="Quản Lý Quỹ Chung Gia Đình & Chia Tiền Nhóm Thông Minh"
        )

        self.assertTrue((session_dir / "01_user_feedback.md").exists())
        self.assertTrue((session_dir / "02_use_case_specs.md").exists())
        self.assertTrue((session_dir / "02b_frontend_ui_spec.md").exists())
        self.assertTrue((session_dir / "03_technical_adr.md").exists())
        self.assertTrue((session_dir / "04_final_rfc.md").exists())

        # Kiểm tra tính toàn vẹn phiên
        integrity = OrchestratorRFCSkill.verify_session_integrity(str(session_dir))
        self.assertTrue(integrity["is_valid"])
        self.assertIn("COUNCIL_FINAL_APPROVAL", state.requirements[-1])



if __name__ == "__main__":
    unittest.main(verbosity=2)
