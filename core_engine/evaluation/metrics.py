"""
Module: core_engine.evaluation.metrics
Mục đích: Thu thập và tính toán các chỉ số khoa học thực nghiệm phục vụ Báo cáo Đồ án Tốt nghiệp
Tuân thủ nguyên tắc Spec: Task Success Rate, Repair Success Rate, Tool Efficiency, Latency
"""

from dataclasses import dataclass, field
from typing import List, Dict, Any
from core_engine.state import ProjectState, Phase


@dataclass
class TaskMetric:
    task_id: str
    is_success: bool
    iterations: int
    tool_calls_count: int
    test_passed: bool
    duration_seconds: float
    changed_files_count: int


@dataclass
class EvaluationReport:
    total_tasks: int = 0
    successful_tasks: int = 0
    failed_tasks: int = 0
    total_tool_calls: int = 0
    avg_iterations_per_task: float = 0.0
    task_success_rate: float = 0.0
    test_pass_rate: float = 0.0
    metrics_list: List[TaskMetric] = field(default_factory=list)

    def generate_markdown_summary(self) -> str:
        """Tạo bảng báo cáo thực nghiệm chuẩn Markdown để đưa vào Quyển Đồ án."""
        return (
            "## BẢNG KẾT QUẢ THỰC NGHIỆM HỆ THỐNG AI SOFTWARE ENGINEER\n\n"
            f"| Chỉ số Đo lường (Metric) | Kết quả Đạt được | Tiêu chuẩn Đánh giá |\n"
            f"| :--- | :--- | :--- |\n"
            f"| **Tổng số Task thực nghiệm** | `{self.total_tasks}` tasks | Đa dạng từ sửa bug đến thêm tính năng |\n"
            f"| **Tỷ lệ Hoàn thành (Task Success Rate)** | **`{self.task_success_rate * 100:.1f}%`** | Mục tiêu: > 80% |\n"
            f"| **Tỷ lệ Test PASS (Test Pass Rate)** | **`{self.test_pass_rate * 100:.1f}%`** | Kiểm thử tự động qua Unittest/Pytest |\n"
            f"| **Số vòng lặp trung bình / task** | `{self.avg_iterations_per_task:.2f}` vòng | Giới hạn tối đa: 5 vòng |\n"
            f"| **Tổng số lượt gọi Tool** | `{self.total_tool_calls}` calls | Đọc, sửa file, kiểm thử |\n"
        )


class MetricsCollector:
    @staticmethod
    def evaluate_states(states: List[ProjectState]) -> EvaluationReport:
        """Tổng hợp danh sách các ProjectState thành một EvaluationReport hoàn chỉnh."""
        if not states:
            return EvaluationReport()

        report = EvaluationReport(total_tasks=len(states))
        total_iterations = 0

        for s in states:
            is_success = (s.current_phase == Phase.DONE)
            test_passed = s.test_results.get("is_passed", False)
            tool_count = len(s.tool_history)

            if is_success:
                report.successful_tasks += 1
            else:
                report.failed_tasks += 1

            total_iterations += s.iteration_count
            report.total_tool_calls += tool_count

            report.metrics_list.append(
                TaskMetric(
                    task_id=s.task_id,
                    is_success=is_success,
                    iterations=s.iteration_count,
                    tool_calls_count=tool_count,
                    test_passed=test_passed,
                    duration_seconds=s.test_results.get("duration_seconds", 0.0),
                    changed_files_count=len(s.changed_files)
                )
            )

        report.task_success_rate = report.successful_tasks / report.total_tasks
        report.test_pass_rate = sum(1 for m in report.metrics_list if m.test_passed) / report.total_tasks
        report.avg_iterations_per_task = total_iterations / report.total_tasks

        return report
