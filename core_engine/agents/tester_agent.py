"""
Module: core_engine.agents.tester_agent
Mục đích: Chuyên trách chạy bộ kiểm thử, phân tích lỗi stacktrace và sinh test report có cấu trúc
Tuân thủ nguyên tắc Claude & Spec: Structured Test Report, Failure Analysis
"""

from typing import Dict, Any
from core_engine.state import ProjectState
from core_engine.agents.base_agent import BaseAgent


TESTER_AGENT_SYSTEM_PROMPT = """Bạn là Senior QA & Test Automation Engineer.
Nhiệm vụ: Thực thi bộ kiểm thử (Unit / Integration Tests) và phân tích nguyên nhân thất bại.
"""


class TesterAgent(BaseAgent):
    def __init__(self, llm_client, filesystem, terminal, git, test_target: str = "tests"):
        super().__init__(
            name="TesterAgent",
            system_prompt=TESTER_AGENT_SYSTEM_PROMPT,
            llm_client=llm_client,
            filesystem=filesystem,
            terminal=terminal,
            git=git,
        )
        self.test_target = test_target

    def step(self, state: ProjectState) -> ProjectState:
        """Chạy test suite và cập nhật kết quả vào state.test_results."""
        test_outcome = self.terminal.run_tests(test_target=self.test_target)
        state.test_results = test_outcome

        if not test_outcome["is_passed"]:
            state.errors.append(f"[TEST_FAILURE] Bộ kiểm thử thất bại:\n{test_outcome['raw_output']}")

        return state
