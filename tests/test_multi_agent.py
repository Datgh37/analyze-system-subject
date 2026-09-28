"""
Module: tests/test_multi_agent.py
Mục đích: Kiểm thử tích hợp toàn diện luồng Multi-Agent hoàn chỉnh
"""

import os
import sys
import unittest
import tempfile
import shutil
from pathlib import Path

sys.path.insert(0, str(Path(__file__).parent.parent))

from core_engine.state import ProjectState, Phase
from core_engine.tools.filesystem import FilesystemTools
from core_engine.tools.terminal import TerminalTools
from core_engine.tools.git_tools import GitTools
from core_engine.llm_client import MockLLMClient
from core_engine.agents.requirement_agent import RequirementAgent
from core_engine.agents.planner_agent import PlannerAgent
from core_engine.agents.coding_agent import CodingAgent
from core_engine.agents.tester_agent import TesterAgent
from core_engine.agents.reviewer_agent import ReviewerAgent
from core_engine.workflow.orchestrator import Orchestrator


class TestMultiAgentIntegration(unittest.TestCase):
    def setUp(self):
        self.test_dir = tempfile.mkdtemp(prefix="multi_agent_test_")
        self.workspace = Path(self.test_dir)

        # Tạo module bài toán quản lý chi tiêu
        src_dir = self.workspace / "src"
        src_dir.mkdir()
        (src_dir / "budget.py").write_text(
            "def calculate_burn_rate(spent: float, days: int) -> float:\n"
            "    if days <= 0: return 0.0\n"
            "    return spent * days  # BUG: Cố tình viết spent * days thay vì spent / days\n",
            encoding="utf-8"
        )

        # Tạo file test
        tests_dir = self.workspace / "tests"
        tests_dir.mkdir()
        (tests_dir / "test_budget.py").write_text(
            "import sys, os\n"
            "sys.path.insert(0, os.path.abspath('.'))\n"
            "import unittest\n"
            "from src.budget import calculate_burn_rate\n\n"
            "class TestBudget(unittest.TestCase):\n"
            "    def test_burn_rate(self):\n"
            "        self.assertEqual(calculate_burn_rate(3000000, 10), 300000.0)\n\n"
            "if __name__ == '__main__':\n"
            "    unittest.main()\n",
            encoding="utf-8"
        )

        self.fs = FilesystemTools(workspace_root=str(self.workspace))
        self.term = TerminalTools(workspace_root=str(self.workspace))
        self.git = GitTools(workspace_root=str(self.workspace))

    def tearDown(self):
        shutil.rmtree(self.test_dir, ignore_errors=True)

    def test_multi_agent_e2e_workflow(self):
        """
        Kiểm thử luồng Multi-Agent E2E:
        1. RequirementAgent trích xuất FR
        2. PlannerAgent lập kế hoạch
        3. CodingAgent sửa code
        4. TesterAgent kiểm thử
        5. ReviewerAgent phê duyệt
        """
        # Mock LLM cho từng agent
        req_llm = MockLLMClient([{
            "content": "FR-001: Tính toán tốc độ chi tiêu Spending Velocity chính xác theo ngày.",
            "tool_calls": []
        }])
        plan_llm = MockLLMClient([{
            "content": "1. Khảo sát src/budget.py\n2. Sửa công thức burn_rate sang spent / days\n3. Chạy test_budget.py",
            "tool_calls": []
        }])
        coder_llm = MockLLMClient([{
            "content": "Sửa lỗi spent * days thành spent / days",
            "tool_calls": [{
                "name": "edit_file",
                "arguments": {
                    "path": "src/budget.py",
                    "target": "return spent * days",
                    "replacement": "return spent / days"
                }
            }]
        }])
        reviewer_llm = MockLLMClient([{
            "content": "Code sạch, tuân thủ Clean Architecture, không có lỗi bảo mật.",
            "tool_calls": []
        }])

        req_agent = RequirementAgent(req_llm, self.fs, self.term, self.git)
        plan_agent = PlannerAgent(plan_llm, self.fs, self.term, self.git)
        coder_agent = CodingAgent(coder_llm, self.fs, self.term, self.git)
        tester_agent = TesterAgent(coder_llm, self.fs, self.term, self.git, test_target="tests/test_budget.py")
        reviewer_agent = ReviewerAgent(reviewer_llm, self.fs, self.term, self.git)

        orchestrator = Orchestrator(
            requirement_agent=req_agent,
            planner_agent=plan_agent,
            coding_agent=coder_agent,
            tester_agent=tester_agent,
            reviewer_agent=reviewer_agent
        )

        state = ProjectState(
            task_id="TASK-BUDGET-01",
            task_description="Sửa lỗi tính toán tốc độ chi tiêu trong phân hệ Budget Management"
        )

        final_state = orchestrator.run_workflow(state, test_target="tests/test_budget.py")

        # Kiểm chứng kết quả E2E
        self.assertEqual(final_state.current_phase, Phase.DONE)
        self.assertTrue(final_state.test_results.get("is_passed", False))
        self.assertEqual(len(final_state.requirements), 1)
        self.assertEqual(len(final_state.plan_steps), 3)
        self.assertIn("src/budget.py", final_state.changed_files)


if __name__ == "__main__":
    unittest.main(verbosity=2)
