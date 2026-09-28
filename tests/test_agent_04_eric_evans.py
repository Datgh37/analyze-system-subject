"""
Module: tests/test_agent_04_eric_evans.py
Mục đích: Kiểm thử độc lập cho Agent 04 - Eric Evans & EvansDomainSkill
"""

from decimal import Decimal
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
from core_engine.skills.evans_domain_skill import EvansDomainSkill, Money
from core_engine.council.eric_evans_agent import EricEvansAgent


class TestEvansDomainSkill(unittest.TestCase):
    def test_money_arithmetic_precision(self):
        m1 = Money(Decimal("100000.50"), "VND")
        m2 = Money(Decimal("50000.25"), "VND")

        m_sum = m1.add(m2)
        self.assertEqual(m_sum.amount, Decimal("150000.75"))
        self.assertEqual(m_sum.currency, "VND")

        m_sub = m1.subtract(m2)
        self.assertEqual(m_sub.amount, Decimal("50000.25"))

        m_mul = m1.multiply(2)
        self.assertEqual(m_mul.amount, Decimal("200001.00"))

    def test_money_split_evenly_no_loss(self):
        # 100,000đ chia cho 3 người
        # Kết quả: [33333.34, 33333.33, 33333.33] -> Tổng đúng 100,000.00đ
        total_money = Money(Decimal("100000.00"), "VND")
        parts = total_money.split_evenly(3)

        self.assertEqual(len(parts), 3)
        self.assertEqual(parts[0].amount, Decimal("33333.34"))
        self.assertEqual(parts[1].amount, Decimal("33333.33"))
        self.assertEqual(parts[2].amount, Decimal("33333.33"))

        total_sum = sum(p.amount for p in parts)
        self.assertEqual(total_sum, Decimal("100000.00"))

    def test_money_currency_mismatch_error(self):
        m_vnd = Money(Decimal("100000"), "VND")
        m_usd = Money(Decimal("5"), "USD")

        with self.assertRaises(ValueError):
            m_vnd.add(m_usd)

        with self.assertRaises(ValueError):
            m_vnd.subtract(m_usd)

    def test_validate_bounded_contexts(self):
        # Trường hợp 1: 4 Bounded Contexts hợp lệ
        valid_contexts = {
            "ExpenseContext": ["IdentityContext"],
            "GroupSettlementContext": ["IdentityContext", "ExpenseContext"],
            "BudgetingContext": ["ExpenseContext"],
            "IdentityContext": []
        }
        res_valid = EvansDomainSkill.validate_bounded_contexts(valid_contexts)
        self.assertTrue(res_valid["is_valid"])
        self.assertEqual(res_valid["verdict"], "KIẾN TRÚC BOUNDED CONTEXT HỢP LỆ")

        # Trường hợp 2: Có phụ thuộc vòng tròn (Expense <-> GroupSettlement)
        circular_contexts = {
            "ExpenseContext": ["GroupSettlementContext"],
            "GroupSettlementContext": ["ExpenseContext"],
            "BudgetingContext": [],
            "IdentityContext": []
        }
        res_circ = EvansDomainSkill.validate_bounded_contexts(circular_contexts)
        self.assertFalse(res_circ["is_valid"])
        self.assertTrue(len(res_circ["circular_dependencies"]) > 0)

    def test_format_clean_architecture_adr(self):
        adr_data = {
            "id": "ADR-01",
            "title": "Clean Architecture & DDD",
            "status": "ACCEPTED",
            "context": "Cần phân tách logic nghiệp vụ khỏi CSDL.",
            "decision": "Áp dụng Clean Architecture 4 lớp.",
            "consequences": "Dễ dàng test độc lập."
        }
        adr_md = EvansDomainSkill.format_clean_architecture_adr(adr_data)
        self.assertIn("ADR-01", adr_md)
        self.assertIn("Clean Architecture", adr_md)
        self.assertIn("Domain Layer", adr_md)


class TestEricEvansAgent(unittest.TestCase):
    def setUp(self):
        self.test_dir = tempfile.mkdtemp(prefix="evans_agent_test_")
        self.workspace = Path(self.test_dir)

        (self.workspace / "core").mkdir()
        (self.workspace / "core" / "04_eric_evans.md").write_text(
            "Bạn là Eric Evans chuyên gia Domain-Driven Design.", encoding="utf-8"
        )

        self.fs = FilesystemTools(workspace_root=str(self.workspace))
        self.term = TerminalTools(workspace_root=str(self.workspace))
        self.git = GitTools(workspace_root=str(self.workspace))

    def tearDown(self):
        shutil.rmtree(self.test_dir, ignore_errors=True)

    def test_evans_agent_step(self):
        mock_llm = MockLLMClient([{
            "content": "Tôi đã thiết kế mô hình Domain với Bounded Contexts rõ ràng và Money Value Object bất biến.",
            "tool_calls": []
        }])

        agent = EricEvansAgent(mock_llm, self.fs, self.term, self.git)
        self.assertIn("Eric Evans", agent.system_prompt)
        self.assertIn("create_money", agent.tools_map)
        self.assertIn("validate_bounded_contexts", agent.tools_map)

        state = ProjectState(
            task_id="TASK-ARCH-01",
            task_description="Thiết kế Kiến trúc Domain cho Phân hệ Quỹ và Chia Tiền"
        )
        state.requirements.append("[IEEE_29148_USE_CASE_SPEC]: Use case Quản trị Quỹ Gia Đình")
        final_state = agent.step(state)

        self.assertTrue(len(final_state.requirements) > 1)
        self.assertIn("CLEAN_ARCHITECTURE_ADR", final_state.requirements[-1])


if __name__ == "__main__":
    unittest.main(verbosity=2)
