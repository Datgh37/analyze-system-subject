"""
Module: tests/test_agent_01_thaler.py
Mục đích: Kiểm thử độc lập cho Agent 01 - Richard Thaler & ThalerNudgeSkill
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
from core_engine.skills.thaler_nudge_skill import ThalerNudgeSkill
from core_engine.council.richard_thaler_agent import RichardThalerAgent


class TestThalerNudgeSkill(unittest.TestCase):
    def test_spending_velocity_calculation(self):
        # Người dùng đặt ngân sách 6,000,000đ / tháng (30 ngày).
        # Mới đến ngày 10 đã tiêu 3,000,000đ -> Tốc độ: 300,000đ/ngày -> Dự báo cạn tiền ngày 20
        res = ThalerNudgeSkill.calculate_spending_velocity(
            spent_amount=3_000_000,
            current_day=10,
            total_days_in_month=30,
            monthly_budget=6_000_000
        )
        self.assertEqual(res["daily_burn_rate"], 300_000.0)
        self.assertEqual(res["projected_total"], 9_000_000.0)
        self.assertTrue(res["is_overspending"])
        self.assertEqual(res["depletion_day"], 20)
        self.assertEqual(res["recommended_daily_limit"], 150_000.0)

    def test_nudge_recommendation_positive(self):
        velocity_data = {
            "spent_amount": 3_000_000,
            "monthly_budget": 6_000_000,
            "daily_burn_rate": 300_000.0,
            "projected_total": 9_000_000.0,
            "depletion_day": 20,
            "is_overspending": True,
            "recommended_daily_limit": 150_000.0
        }
        nudge = ThalerNudgeSkill.generate_nudge_recommendation(velocity_data, "Ăn uống")
        self.assertIn("CÚ HÍCH TÀI CHÍNH", nudge)
        self.assertIn("ngày 20", nudge)
        self.assertIn("150,000đ/ngày", nudge)

    def test_ux_friction_score_comparison(self):
        # Trường hợp 1: Form nhập tay 4 bước, 6 trường thông tin không có OCR
        manual_res = ThalerNudgeSkill.evaluate_ux_friction(step_count=4, form_field_count=6, has_ocr_automation=False)
        self.assertTrue(manual_res.is_high_friction)
        self.assertEqual(manual_res.churn_risk_level, "HIGH")

        # Trường hợp 2: Chụp ảnh hóa đơn OCR 1 chạm
        ocr_res = ThalerNudgeSkill.evaluate_ux_friction(step_count=1, form_field_count=1, has_ocr_automation=True)
        self.assertFalse(ocr_res.is_high_friction)
        self.assertEqual(ocr_res.churn_risk_level, "LOW")

    def test_subscription_trap_detection(self):
        txs = [
            {"merchant": "Netflix", "amount": 260_000, "date": "2026-07-01"},
            {"merchant": "Netflix", "amount": 260_000, "date": "2026-08-01"},
            {"merchant": "Cà phê Highland", "amount": 45_000, "date": "2026-08-05"}
        ]
        traps = ThalerNudgeSkill.detect_subscription_traps(txs)
        self.assertEqual(len(traps), 1)
        self.assertEqual(traps[0]["merchant"], "Netflix")


class TestRichardThalerAgent(unittest.TestCase):
    def setUp(self):
        self.test_dir = tempfile.mkdtemp(prefix="thaler_agent_test_")
        self.workspace = Path(self.test_dir)

        # Copy file core/01_richard_thaler.md vào workspace tạm
        (self.workspace / "core").mkdir()
        (self.workspace / "core" / "01_richard_thaler.md").write_text(
            "Bạn là GS. Richard Thaler chuyên gia Nudge Theory.", encoding="utf-8"
        )

        self.fs = FilesystemTools(workspace_root=str(self.workspace))
        self.term = TerminalTools(workspace_root=str(self.workspace))
        self.git = GitTools(workspace_root=str(self.workspace))

    def tearDown(self):
        shutil.rmtree(self.test_dir, ignore_errors=True)

    def test_thaler_agent_step(self):
        mock_llm = MockLLMClient([{
            "content": "Tôi nhận thấy form nhập tay này có Friction Score quá cao. Đề xuất bổ sung OCR Scan 1 chạm.",
            "tool_calls": []
        }])

        agent = RichardThalerAgent(mock_llm, self.fs, self.term, self.git)
        self.assertIn("Richard Thaler", agent.system_prompt)
        self.assertIn("calculate_spending_velocity", agent.tools_map)
        self.assertIn("evaluate_ux_friction", agent.tools_map)

        state = ProjectState(
            task_id="TASK-UX-01",
            task_description="Thiết kế luồng ghi chép chi tiêu cá nhân"
        )
        final_state = agent.step(state)

        self.assertTrue(len(final_state.requirements) > 0)
        self.assertIn("THALER_BEHAVIORAL_INSIGHT", final_state.requirements[0])


if __name__ == "__main__":
    unittest.main(verbosity=2)
