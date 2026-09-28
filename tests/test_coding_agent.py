"""
Module: tests/test_coding_agent.py
Mục đích: Kiểm thử độc lập cho CodingAgent và Vòng lặp tự sửa lỗi (Self-Correction Loop)
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
from core_engine.agents.coding_agent import CodingAgent


class TestCodingAgentSelfCorrection(unittest.TestCase):
    def setUp(self):
        self.test_dir = tempfile.mkdtemp(prefix="agent_test_workspace_")
        self.workspace = Path(self.test_dir)

        # 1. Tạo file code có lỗi logic
        # Lỗi: hàm tính tổng trả về sai (a - b thay vì a + b)
        src_dir = self.workspace / "src"
        src_dir.mkdir()
        self.buggy_code = "def calculate_total(a, b):\n    return a - b  # BUG: Cố tình sai logic\n"
        (src_dir / "calculator.py").write_text(self.buggy_code, encoding="utf-8")

        # 2. Tạo file test kiểm thử
        tests_dir = self.workspace / "tests"
        tests_dir.mkdir()
        test_content = (
            "import sys, os\n"
            "sys.path.insert(0, os.path.abspath('.'))\n"
            "import unittest\n"
            "from src.calculator import calculate_total\n\n"
            "class TestCalc(unittest.TestCase):\n"
            "    def test_sum(self):\n"
            "        self.assertEqual(calculate_total(10, 20), 30)\n\n"
            "if __name__ == '__main__':\n"
            "    unittest.main()\n"
        )
        (tests_dir / "test_calc.py").write_text(test_content, encoding="utf-8")


        self.fs = FilesystemTools(workspace_root=str(self.workspace))
        self.term = TerminalTools(workspace_root=str(self.workspace))
        self.git = GitTools(workspace_root=str(self.workspace))

    def tearDown(self):
        shutil.rmtree(self.test_dir, ignore_errors=True)

    def test_successful_self_correction(self):
        """
        Kịch bản 1: Coding Agent nhận bug, inspect code -> edit_file sửa thành a + b -> test PASS.
        """
        # Chuẩn bị Mock LLM kịch bản 2 bước:
        # Bước 1: Inspect code
        # Bước 2: Sửa file
        mock_llm = MockLLMClient([
            {
                "content": "Tôi sẽ đọc file src/calculator.py để tìm lỗi.",
                "tool_calls": [
                    {
                        "name": "read_file",
                        "arguments": {"path": "src/calculator.py"}
                    }
                ]
            },
            {
                "content": "Đã phát hiện lỗi: a - b. Tôi sẽ sửa thành a + b.",
                "tool_calls": [
                    {
                        "name": "edit_file",
                        "arguments": {
                            "path": "src/calculator.py",
                            "target": "return a - b",
                            "replacement": "return a + b"
                        }
                    }
                ]
            }
        ])

        agent = CodingAgent(
            llm_client=mock_llm,
            filesystem=self.fs,
            terminal=self.term,
            git=self.git
        )

        state = ProjectState(
            task_id="TASK-001",
            task_description="Sửa lỗi logic trong hàm calculate_total để test_calc.py pass",
            max_iterations=5
        )

        # Chạy vòng lặp tự sửa lỗi
        final_state = agent.execute_self_repair_loop(state, test_target="tests/test_calc.py")

        # Kiểm chứng kết quả
        debug_msg = f"Phase: {final_state.current_phase}, Raw Output: {final_state.test_results.get('raw_output')}, Errors: {final_state.errors}"
        self.assertEqual(final_state.current_phase, Phase.DONE, msg=debug_msg)
        self.assertTrue(final_state.test_results.get("is_passed", False), msg=debug_msg)
        self.assertIn("src/calculator.py", final_state.changed_files)

        
        # Kiểm tra nội dung file thực tế đã được sửa
        fixed_content = (self.workspace / "src" / "calculator.py").read_text(encoding="utf-8")
        self.assertIn("return a + b", fixed_content)

    def test_exceed_max_iterations_fails_gracefully(self):
        """
        Kịch bản 2: Vượt quá max_iterations (5 vòng) mà test vẫn không pass -> FAILED_REQUIRING_HUMAN_REVIEW.
        """
        # Mock LLM liên tục làm hành động vô nghĩa không sửa được lỗi
        useless_responses = [
            {
                "content": f"Vòng lặp {i}: Tôi đọc lại file...",
                "tool_calls": [{"name": "read_file", "arguments": {"path": "src/calculator.py"}}]
            }
            for i in range(5)
        ]
        mock_llm = MockLLMClient(useless_responses)

        agent = CodingAgent(
            llm_client=mock_llm,
            filesystem=self.fs,
            terminal=self.term,
            git=self.git
        )

        state = ProjectState(
            task_id="TASK-002",
            task_description="Task không thể tự giải quyết",
            max_iterations=3
        )

        final_state = agent.execute_self_repair_loop(state, test_target="tests/test_calc.py")

        # Phải chuyển sang FAILED_REQUIRING_HUMAN_REVIEW và dừng vòng lặp an toàn
        self.assertEqual(final_state.current_phase, Phase.FAILED_REQUIRING_HUMAN_REVIEW)
        self.assertEqual(final_state.iteration_count, 3)


if __name__ == "__main__":
    unittest.main(verbosity=2)
