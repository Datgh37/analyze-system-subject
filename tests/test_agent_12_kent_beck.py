"""
Module: tests/test_agent_12_kent_beck.py
Mục đích: Kiểm thử độc lập cho Agent 12: Kent Beck & Martin Fowler và BeckQAStrategySkill
"""

import sys
import shutil
import tempfile
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).parent.parent))

from core_engine.skills.beck_qa_strategy_skill import BeckQAStrategySkill
from core_engine.council.kent_beck_agent import KentBeckAgent
from core_engine.tools.filesystem import FilesystemTools
from core_engine.tools.terminal import TerminalTools
from core_engine.tools.git_tools import GitTools
from core_engine.llm_client import MockLLMClient
from core_engine.state import ProjectState


class TestKentBeckAgent(unittest.TestCase):
    def setUp(self):
        self.skill = BeckQAStrategySkill()
        self.test_dir = tempfile.mkdtemp()
        self.fs = FilesystemTools(self.test_dir)
        self.term = TerminalTools(self.test_dir)
        self.git = GitTools(self.test_dir)

    def tearDown(self):
        shutil.rmtree(self.test_dir, ignore_errors=True)

    def test_calculate_test_pyramid_distribution(self):
        # 1. Balanced Pyramid (70 Unit, 20 Int, 10 E2E) -> High Health Score
        res_good = self.skill.calculate_test_pyramid_distribution(70, 20, 10)
        self.assertEqual(res_good["total_tests"], 100)
        self.assertGreaterEqual(res_good["health_score"], 80)
        self.assertIsNone(res_good["anti_pattern_detected"])

        # 2. Ice Cream Cone Anti-pattern (10 Unit, 20 Int, 70 E2E)
        res_bad = self.skill.calculate_test_pyramid_distribution(10, 20, 70)
        self.assertIn("Kem Ốc Quế", res_bad["anti_pattern_detected"])
        self.assertLess(res_bad["health_score"], 60)

    def test_generate_e2e_playwright_scenario(self):
        code = self.skill.generate_e2e_playwright_scenario("JointHouseholdFund")
        self.assertIn("@playwright/test", code)
        self.assertIn("test.describe", code)
        self.assertIn("await page.goto", code)
        self.assertIn("await expect(page", code)

    def test_design_load_test_profile(self):
        profile = self.skill.design_load_test_profile("ReceiptOCRScan", target_vus=1000)
        self.assertEqual(profile["target_concurrent_users"], 1000)
        self.assertIn("sla_thresholds", profile)
        self.assertIn("< 200ms", profile["sla_thresholds"]["http_req_duration_p95"])
        self.assertIn("k6_script_snippet", profile)

    def test_format_qa_verification_plan(self):
        plan = self.skill.format_qa_verification_plan("JointHouseholdFund")
        self.assertIn("KẾ HOẠCH KIỂM THỬ TỰ ĐỘNG", plan)
        self.assertIn("Kim Tự Tháp Kiểm Thử", plan)
        self.assertIn("Playwright Spec", plan)
        self.assertIn("k6 Performance SLA", plan)

    def test_kent_beck_agent_step(self):
        mock_llm = MockLLMClient([
            {
                "content": "Tôi đã hoàn thành thiết lập Kim tự tháp kiểm thử và kịch bản Playwright E2E cho Quỹ gia đình.",
                "tool_calls": []
            }
        ])

        agent = KentBeckAgent(
            llm_client=mock_llm,
            filesystem=self.fs,
            terminal=self.term,
            git=self.git
        )

        state = ProjectState(
            task_id="TEST-12",
            task_description="Thiết Kế Chiến Lược Kiểm Thử Quỹ Gia Đình"
        )

        final_state = agent.step(state)
        self.assertEqual(len(final_state.requirements), 1)
        self.assertIn("FULL_STACK_QA_TEST_STRATEGY_SPEC", final_state.requirements[0])


if __name__ == "__main__":
    unittest.main(verbosity=2)
