"""
Module: tests/test_agent_06_debt_optimizer.py
Mục đích: Kiểm thử độc lập cho Agent 06 - Debt & Cashflow Optimizer & DebtCashflowOptimizerSkill
Kiểm thử giải thuật: Debt Snowball (Ưu tiên số dư nhỏ) vs Debt Avalanche (Ưu tiên lãi suất cao).
"""

import sys
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).parent.parent))

from core_engine.skills.debt_cashflow_optimizer_skill import DebtCashflowOptimizerSkill
from core_engine.council.debt_optimizer_agent import DebtOptimizerAgent
from core_engine.state import ProjectState
from core_engine.tools.filesystem import FilesystemTools
from core_engine.tools.terminal import TerminalTools
from core_engine.tools.git_tools import GitTools
from core_engine.llm_client import MockLLMClient


class TestDebtCashflowOptimizerSkill(unittest.TestCase):
    def setUp(self):
        # Giả lập 3 khoản nợ cá nhân điển hình:
        # 1. Nợ thẻ tín dụng: 10,000,000đ, Lãi suất 30%/năm, Min payment 1,000,000đ
        # 2. Vay mua xe máy: 30,000,000đ, Lãi suất 12%/năm, Min payment 2,000,000đ
        # 3. Vay người quen: 5,000,000đ, Lãi suất 0%/năm, Min payment 500,000đ
        self.debts = [
            {"name": "Nợ Thẻ Tín Dụng", "principal": 10_000_000, "interest_rate": 30.0, "min_payment": 1_000_000},
            {"name": "Vay Mua Xe Máy", "principal": 30_000_000, "interest_rate": 12.0, "min_payment": 2_000_000},
            {"name": "Vay Người Quen", "principal": 5_000_000, "interest_rate": 0.0, "min_payment": 500_000},
        ]

    def test_snowball_order(self):
        # Snowball: Sắp xếp theo số dư nhỏ nhất -> Vay Người Quen (5tr) -> Thẻ Tín Dụng (10tr) -> Mua Xe (30tr)
        res = DebtCashflowOptimizerSkill.calculate_snowball_schedule(
            debts=self.debts,
            monthly_extra_budget=1_000_000
        )
        self.assertEqual(res["strategy"], "DEBT_SNOWBALL")
        self.assertTrue(res["is_debt_free"])
        self.assertEqual(res["payoff_order"][0], "Vay Người Quen")
        self.assertEqual(res["payoff_order"][1], "Nợ Thẻ Tín Dụng")
        self.assertEqual(res["payoff_order"][2], "Vay Mua Xe Máy")

    def test_avalanche_order(self):
        # Avalanche: Sắp xếp theo lãi suất cao nhất -> Thẻ Tín Dụng (30%) -> Mua Xe (12%) -> Người Quen (0%)
        res = DebtCashflowOptimizerSkill.calculate_avalanche_schedule(
            debts=self.debts,
            monthly_extra_budget=1_000_000
        )
        self.assertEqual(res["strategy"], "DEBT_AVALANCHE")
        self.assertTrue(res["is_debt_free"])
        # Thứ tự ưu tiên dồn tiền trả nợ (Priority Order): Lãi cao nhất trước
        self.assertEqual(res["priority_order"][0], "Nợ Thẻ Tín Dụng")
        self.assertEqual(res["priority_order"][1], "Vay Mua Xe Máy")
        self.assertEqual(res["priority_order"][2], "Vay Người Quen")
        # Thứ tự sạch nợ thực tế: Thẻ Tín Dụng -> Người Quen (trả hết theo min pay) -> Mua Xe
        self.assertEqual(res["payoff_order"][0], "Nợ Thẻ Tín Dụng")
        self.assertEqual(res["payoff_order"][1], "Vay Người Quen")
        self.assertEqual(res["payoff_order"][2], "Vay Mua Xe Máy")

    def test_compare_strategies_avalanche_saves_interest(self):
        # Avalanche phải tiết kiệm tiền lãi hơn hoặc bằng Snowball
        comparison = DebtCashflowOptimizerSkill.compare_payoff_strategies(
            debts=self.debts,
            monthly_extra_budget=1_000_000
        )
        snowball_interest = comparison["snowball_result"]["total_interest"]
        avalanche_interest = comparison["avalanche_result"]["total_interest"]

        self.assertLessEqual(avalanche_interest, snowball_interest)
        self.assertGreaterEqual(comparison["interest_savings_with_avalanche"], 0.0)
        self.assertIn("Avalanche", comparison["recommendation"])

    def test_agent_integration(self):
        llm = MockLLMClient(responses=[{"content": "Đã hoàn tất phân tích tối ưu hóa Debt Snowball vs Avalanche."}])
        fs = FilesystemTools(Path("."))
        term = TerminalTools()
        git = GitTools(Path("."))

        agent = DebtOptimizerAgent(llm, fs, term, git)
        state = ProjectState(task_id="TEST-06", task_description="Tối ưu hóa chiến lược trả nợ thẻ tín dụng")
        state = agent.step(state)

        self.assertTrue(len(state.requirements) > 0)
        self.assertIn("DEBT_CASHFLOW_OPTIMIZER_SPEC", state.requirements[0])


if __name__ == "__main__":
    unittest.main()
