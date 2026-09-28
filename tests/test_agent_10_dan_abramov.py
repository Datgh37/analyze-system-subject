"""
Module: tests/test_agent_10_dan_abramov.py
Mục đích: Kiểm thử độc lập cho Agent 10: Dan Abramov & Christopher Chedeau và AbramovClientStateSkill
"""

import sys
import shutil
import tempfile
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).parent.parent))

from core_engine.skills.abramov_client_state_skill import AbramovClientStateSkill
from core_engine.council.dan_abramov_agent import DanAbramovAgent
from core_engine.tools.filesystem import FilesystemTools
from core_engine.tools.terminal import TerminalTools
from core_engine.tools.git_tools import GitTools
from core_engine.llm_client import MockLLMClient
from core_engine.state import ProjectState


class TestDanAbramovAgent(unittest.TestCase):
    def setUp(self):
        self.skill = AbramovClientStateSkill()
        self.test_dir = tempfile.mkdtemp()
        self.fs = FilesystemTools(self.test_dir)
        self.term = TerminalTools(self.test_dir)
        self.git = GitTools(self.test_dir)

    def tearDown(self):
        shutil.rmtree(self.test_dir, ignore_errors=True)

    def test_design_client_store_schema(self):
        schema = self.skill.design_client_store_schema()
        self.assertIn("slices", schema)
        self.assertIn("auth_slice", schema["slices"])
        self.assertIn("joint_fund_slice", schema["slices"])
        self.assertIn("expense_slice", schema["slices"])
        self.assertIn("debt_graph_slice", schema["slices"])

    def test_simulate_optimistic_ui_success(self):
        payload = {"amount": 500000, "category": "Đi chợ", "paid_by": "user_1"}
        res = self.skill.simulate_optimistic_ui_action("CREATE_EXPENSE", payload, server_error=False)

        self.assertIn("step_1_optimistic_commit", res)
        self.assertEqual(res["step_1_optimistic_commit"]["sync_status"], "SYNCING")
        self.assertTrue(res["step_1_optimistic_commit"]["is_optimistic"])

        self.assertIn("step_3_final_resolution", res)
        self.assertEqual(res["step_3_final_resolution"]["action"], "RECONCILE_SUCCESS")
        self.assertEqual(res["step_3_final_resolution"]["final_status"], "COMMITTED")
        self.assertTrue(res["step_3_final_resolution"]["reconciled_id"].startswith("real_"))

    def test_simulate_optimistic_ui_rollback(self):
        payload = {"amount": 500000, "category": "Đi chợ", "paid_by": "user_1"}
        res = self.skill.simulate_optimistic_ui_action("CREATE_EXPENSE", payload, server_error=True)

        self.assertIn("step_3_final_resolution", res)
        self.assertEqual(res["step_3_final_resolution"]["action"], "ROLLBACK_STATE")
        self.assertEqual(res["step_3_final_resolution"]["final_status"], "ERROR_ROLLBACK")
        self.assertIn("Hàng đợi Ngoại tuyến", res["step_3_final_resolution"]["user_notification"])

    def test_design_offline_sync_queue(self):
        offline = self.skill.design_offline_sync_queue()
        self.assertIn("local_storage_engine", offline)
        self.assertIn("priority_order", offline)
        self.assertIn("conflict_resolution", offline)
        self.assertIn("Vector Clock", offline["conflict_resolution"]["strategy"])

    def test_dan_abramov_agent_step(self):
        mock_llm = MockLLMClient([
            {
                "content": "Tôi đã thiết kế xong lược đồ Client State Store và cơ chế Optimistic UI 0ms.",
                "tool_calls": []
            }
        ])

        agent = DanAbramovAgent(
            llm_client=mock_llm,
            filesystem=self.fs,
            terminal=self.term,
            git=self.git
        )

        state = ProjectState(
            task_id="TEST-10",
            task_description="Quản Lý Quỹ Gia Đình & Cập Nhật Lạc Quan"
        )

        final_state = agent.step(state)
        self.assertEqual(len(final_state.requirements), 1)
        self.assertIn("CLIENT_STATE_OPTIMISTIC_UI_ADR", final_state.requirements[0])


if __name__ == "__main__":
    unittest.main(verbosity=2)
