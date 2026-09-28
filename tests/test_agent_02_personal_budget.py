"""
Module: tests/test_agent_02_personal_budget.py
Mục đích: Kiểm thử độc lập cho Agent 02 - Personal Budget Coach & PersonalBudgetCasesSkill
Kiểm thử các nghiệp vụ: Quy tắc 50/30/20, Zero-Based Budgeting, Kakeibo và Hạn mức chi tiêu hàng ngày.
"""

import sys
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).parent.parent))

from core_engine.skills.personal_budget_cases_skill import PersonalBudgetCasesSkill
from core_engine.council.personal_budget_agent import PersonalBudgetAgent
from core_engine.state import ProjectState
from core_engine.tools.filesystem import FilesystemTools
from core_engine.tools.terminal import TerminalTools
from core_engine.tools.git_tools import GitTools
from core_engine.llm_client import MockLLMClient


class TestPersonalBudgetCasesSkill(unittest.TestCase):
    def test_50_30_20_budget_healthy(self):
        # Thu nhập 20,000,000đ/tháng
        # Needs 50%: 10tr (Tiền nhà 6tr, Ăn uống 3tr, Điện nước 1tr = 10tr)
        # Wants 30%: 6tr (Cà phê/ăn ngoài 4tr, Mua sắm 1tr = 5tr -> OK)
        # Savings 20%: 4tr (Quỹ khẩn cấp 3tr, Đầu tư 1tr = 4tr -> OK)
        needs = [
            {"name": "Tiền thuê nhà", "amount": 6_000_000},
            {"name": "Ăn uống cơ bản", "amount": 3_000_000},
            {"name": "Điện nước wifi", "amount": 1_000_000},
        ]
        wants = [
            {"name": "Cà phê & ăn ngoài", "amount": 4_000_000},
            {"name": "Xem phim / giải trí", "amount": 1_000_000},
        ]
        savings = [
            {"name": "Quỹ dự phòng khẩn cấp", "amount": 3_000_000},
            {"name": "Tích lũy", "amount": 1_000_000},
        ]

        res = PersonalBudgetCasesSkill.calculate_50_30_20_budget(
            monthly_income=20_000_000,
            needs_expenses=needs,
            wants_expenses=wants,
            savings_and_debt=savings
        )

        self.assertEqual(res["target_allocations"]["needs_50"], 10_000_000)
        self.assertEqual(res["target_allocations"]["wants_30"], 6_000_000)
        self.assertEqual(res["target_allocations"]["savings_20"], 4_000_000)
        self.assertEqual(res["actual_spending"]["needs"], 10_000_000)
        self.assertEqual(res["actual_spending"]["wants"], 5_000_000)
        self.assertEqual(res["actual_spending"]["savings"], 4_000_000)
        self.assertEqual(res["net_remaining"], 1_000_000)
        self.assertEqual(res["status"], "HEALTHY")

    def test_50_30_20_budget_overspend_wants(self):
        # Thu nhập 15,000,000đ nhưng ăn ngoài giải trí hết 6,000,000đ (40% > 30%)
        needs = [{"name": "Sinh hoạt phí", "amount": 7_500_000}]
        wants = [{"name": "Mua sắm & tiệc tùng", "amount": 6_000_000}]
        savings = [{"name": "Tiết kiệm", "amount": 1_500_000}]

        res = PersonalBudgetCasesSkill.calculate_50_30_20_budget(
            monthly_income=15_000_000,
            needs_expenses=needs,
            wants_expenses=wants,
            savings_and_debt=savings
        )

        self.assertIn("NEEDS_ADJUSTMENT", res["status"])
        self.assertTrue(any("vượt ngưỡng 30%" in alert for alert in res["alerts"]))

    def test_zero_based_budget_balanced(self):
        # Thu nhập 10tr, phân bổ đúng 10tr -> Còn 0đ
        allocations = [
            {"category": "Tiền trọ", "amount": 4_000_000},
            {"category": "Ăn uống", "amount": 3_000_000},
            {"category": "Xăng xe", "amount": 1_000_000},
            {"category": "Tiết kiệm", "amount": 2_000_000},
        ]
        res = PersonalBudgetCasesSkill.calculate_zero_based_budget(10_000_000, allocations)
        self.assertTrue(res["is_zero_balanced"])
        self.assertEqual(res["unallocated_balance"], 0.0)

    def test_zero_based_budget_deficit(self):
        allocations = [
            {"category": "Tiền trọ", "amount": 6_000_000},
            {"category": "Chi tiêu", "amount": 5_000_000},
        ]
        res = PersonalBudgetCasesSkill.calculate_zero_based_budget(10_000_000, allocations)
        self.assertFalse(res["is_zero_balanced"])
        self.assertEqual(res["unallocated_balance"], -1_000_000)

    def test_kakeibo_breakdown(self):
        expenses = [
            {"name": "Gạo, rau, thịt", "amount": 2_000_000, "kakeibo_category": "needs"},
            {"name": "Trà sữa với bạn", "amount": 300_000, "kakeibo_category": "wants"},
            {"name": "Mua sách Design Systems", "amount": 250_000, "kakeibo_category": "culture"},
            {"name": "Vá săm xe máy", "amount": 50_000, "kakeibo_category": "unexpected"},
        ]
        res = PersonalBudgetCasesSkill.calculate_kakeibo_breakdown(expenses)
        self.assertEqual(res["total_expense"], 2_600_000)
        self.assertEqual(res["breakdown"]["needs"], 2_000_000)
        self.assertEqual(res["breakdown"]["wants"], 300_000)
        self.assertEqual(res["breakdown"]["culture"], 250_000)
        self.assertEqual(res["breakdown"]["unexpected"], 50_000)
        self.assertEqual(len(res["kakeibo_reflection_questions"]), 4)

    def test_daily_spending_allowance(self):
        res = PersonalBudgetCasesSkill.calculate_daily_spending_allowance(
            remaining_flexible_budget=3_000_000,
            days_left_in_month=15
        )
        self.assertEqual(res["daily_allowance"], 200_000)

    def test_agent_integration(self):
        llm = MockLLMClient(responses=[{"content": "Ngân sách 50/30/20 đã được thiết lập chặt chẽ cho cá nhân."}])
        fs = FilesystemTools(Path("."))
        term = TerminalTools()
        git = GitTools(Path("."))

        agent = PersonalBudgetAgent(llm, fs, term, git)
        state = ProjectState(task_id="TEST-02", task_description="Thiết lập ngân sách cá nhân 50/30/20")
        state = agent.step(state)

        self.assertTrue(len(state.requirements) > 0)
        self.assertIn("PERSONAL_BUDGET_INSIGHT", state.requirements[0])


if __name__ == "__main__":
    unittest.main()
