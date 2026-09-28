"""
Module: tests/test_production_readiness.py
Mục đích: Kiểm thử toàn diện các tính năng Production-Ready:
- Dynamic Timestamp & Deep Content Quality Gates
- Traceability Matrix (Ma trận truy vết nguồn gốc)
- Multi-LLM Factory (Mock, OpenAI, Gemini, Ollama)
- Tính toàn vẹn của 4 Benchmark Sessions
"""

import sys
import unittest
from pathlib import Path
from datetime import datetime

sys.path.insert(0, str(Path(__file__).parent.parent))

from core_engine.skills.orchestrator_rfc_skill import OrchestratorRFCSkill
from core_engine.llm_client import create_llm_client, MockLLMClient, OpenAILLMClient, GeminiLLMClient, OllamaLLMClient


class TestProductionReadiness(unittest.TestCase):
    def setUp(self):
        self.workspace_root = Path(__file__).parent.parent
        self.sessions_dir = self.workspace_root / "sessions"

    def test_multi_llm_factory(self):
        mock_cli = create_llm_client(provider="mock")
        self.assertIsInstance(mock_cli, MockLLMClient)

        openai_cli = create_llm_client(provider="openai", api_key="sk-test")
        self.assertIsInstance(openai_cli, OpenAILLMClient)

        gemini_cli = create_llm_client(provider="gemini", api_key="test-key")
        self.assertIsInstance(gemini_cli, GeminiLLMClient)

        ollama_cli = create_llm_client(provider="ollama")
        self.assertIsInstance(ollama_cli, OllamaLLMClient)

    def test_dynamic_timestamp_and_rfc_compilation(self):
        test_session = self.sessions_dir / "session_01_household_fund"
        if test_session.exists():
            rfc = OrchestratorRFCSkill.compile_final_rfc(str(test_session), feature_title="Quản Lý Quỹ Gia Đình")
            current_year = str(datetime.now().year)
            self.assertIn(current_year, rfc)
            self.assertIn("Ma Trận Truy Vết Nguồn Gốc", rfc)

    def test_traceability_matrix_generation(self):
        test_session = self.sessions_dir / "session_01_household_fund"
        if test_session.exists():
            matrix = OrchestratorRFCSkill.generate_traceability_matrix(str(test_session), feature_id="FEAT-01")
            self.assertIn("Ma Trận Truy Vết Nguồn Gốc", matrix)
            self.assertIn("Mã Yêu Cầu (REQ ID)", matrix)
            self.assertIn("Ca Sử Dụng (Use Case)", matrix)
            self.assertIn("Quyết Định Kỹ Thuật (ADR)", matrix)

    def test_all_4_benchmark_sessions_integrity_and_quality(self):
        expected_sessions = [
            "session_01_household_fund",
            "session_02_roommate_utilities",
            "session_03_trip_ocr_itemized_split",
            "session_04_spending_velocity_nudge",
            "session_01_personal_budget_50_30_20",
            "session_02_debt_snowball_vs_avalanche",
            "session_03_receipt_itemized_categorization",
            "session_04_spending_velocity_subscription_trap"
        ]

        for s_name in expected_sessions:
            s_path = self.sessions_dir / s_name
            self.assertTrue(s_path.exists(), f"Thiếu thư mục benchmark session: {s_name}")

            # 1. Kiểm tra tính toàn vẹn cấu trúc (Phải đủ 4 file, size > 50b, ko absolute path)
            integrity = OrchestratorRFCSkill.verify_session_integrity(str(s_path))
            self.assertTrue(integrity["is_valid"], f"Session {s_name} không đạt toàn vẹn: {integrity}")
            self.assertEqual(integrity["integrity_score"], 100)

            # 2. Kiểm tra chất lượng nội dung (Gherkin, Luồng lỗi, Trade-offs, DECIMAL)
            quality = OrchestratorRFCSkill.verify_content_quality_gates(str(s_path))
            self.assertTrue(quality["is_passed"], f"Session {s_name} không đạt chất lượng: {quality['violations']}")
            self.assertEqual(quality["quality_score"], 100)


if __name__ == "__main__":
    unittest.main(verbosity=2)
