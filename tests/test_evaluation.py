"""
Module: tests/test_evaluation.py
Mục đích: Kiểm thử tầng thu thập chỉ số Metrics và Memory
"""

import sys
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).parent.parent))

from core_engine.state import ProjectState, Phase
from core_engine.memory.project_memory import ProjectMemory
from core_engine.evaluation.metrics import MetricsCollector


class TestEvaluationAndMemory(unittest.TestCase):
    def test_project_memory_formatting(self):
        mem = ProjectMemory()
        prompt_text = mem.format_for_prompt()
        self.assertIn("Smart Expense & Group Budget System", prompt_text)
        self.assertIn("Clean Architecture", prompt_text)

    def test_metrics_collector(self):
        # Giả lập 3 tasks: 2 DONE, 1 FAILED
        s1 = ProjectState(task_id="T1", task_description="Task 1")
        s1.current_phase = Phase.DONE
        s1.iteration_count = 2
        s1.test_results = {"is_passed": True, "duration_seconds": 1.2}
        s1.record_tool_call("read_file", {}, "out")
        s1.record_tool_call("edit_file", {}, "out")

        s2 = ProjectState(task_id="T2", task_description="Task 2")
        s2.current_phase = Phase.DONE
        s2.iteration_count = 1
        s2.test_results = {"is_passed": True, "duration_seconds": 0.8}
        s2.record_tool_call("edit_file", {}, "out")

        s3 = ProjectState(task_id="T3", task_description="Task 3")
        s3.current_phase = Phase.FAILED_REQUIRING_HUMAN_REVIEW
        s3.iteration_count = 5
        s3.test_results = {"is_passed": False, "duration_seconds": 2.0}

        report = MetricsCollector.evaluate_states([s1, s2, s3])

        self.assertEqual(report.total_tasks, 3)
        self.assertEqual(report.successful_tasks, 2)
        self.assertEqual(report.failed_tasks, 1)
        self.assertAlmostEqual(report.task_success_rate, 2/3)
        self.assertAlmostEqual(report.test_pass_rate, 2/3)
        self.assertAlmostEqual(report.avg_iterations_per_task, (2 + 1 + 5) / 3)

        summary_md = report.generate_markdown_summary()
        self.assertIn("BẢNG KẾT QUẢ THỰC NGHIỆM", summary_md)
        self.assertIn("66.7%", summary_md)


if __name__ == "__main__":
    unittest.main(verbosity=2)
