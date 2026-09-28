"""
Module: core_engine.state
Mục đích: Định nghĩa trạng thái dự án có cấu trúc (Structured Project State)
Tuân thủ nguyên tắc Claude & Spec: Explicit State Machine, Traceability
"""

from enum import Enum
from dataclasses import dataclass, field
from typing import List, Dict, Any, Optional
import time


class Phase(str, Enum):
    NEW = "NEW"
    ANALYZING = "ANALYZING"
    PLANNING = "PLANNING"
    IMPLEMENTING = "IMPLEMENTING"
    TESTING = "TESTING"
    REVIEWING = "REVIEWING"
    DONE = "DONE"
    FAILED_REQUIRING_HUMAN_REVIEW = "FAILED_REQUIRING_HUMAN_REVIEW"


@dataclass
class ToolCallRecord:
    tool_name: str
    arguments: Dict[str, Any]
    output: Any
    timestamp: float = field(default_factory=time.time)


@dataclass
class ProjectState:
    task_id: str
    task_description: str
    current_phase: Phase = Phase.NEW
    
    # Dữ liệu tích lũy qua các pha
    requirements: List[str] = field(default_factory=list)
    plan_steps: List[str] = field(default_factory=list)
    changed_files: List[str] = field(default_factory=list)
    
    # Kết quả kiểm thử và review
    test_results: Dict[str, Any] = field(default_factory=dict)
    review_issues: List[Dict[str, Any]] = field(default_factory=list)
    
    # Kiểm soát vòng lặp sửa lỗi
    iteration_count: int = 0
    max_iterations: int = 5
    
    # Lịch sử gọi tool (Tracing)
    tool_history: List[ToolCallRecord] = field(default_factory=list)
    errors: List[str] = field(default_factory=list)

    def record_tool_call(self, tool_name: str, args: Dict[str, Any], output: Any):
        """Ghi nhận vết thực thi của tool phục vụ đánh giá và debug."""
        self.tool_history.append(
            ToolCallRecord(
                tool_name=tool_name,
                arguments=args,
                output=output
            )
        )

    def can_retry(self) -> bool:
        """Kiểm tra xem còn trong hạn mức số lần tự sửa lỗi không."""
        return self.iteration_count < self.max_iterations
