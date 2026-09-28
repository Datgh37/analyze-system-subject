"""
Module: core_engine.workflow.orchestrator
Mục đích: Bộ điều phối trung tâm quản lý State Machine và luồng chuyển giao giữa các Agent
Tuân thủ nguyên tắc Claude & Spec: Explicit Transitions, No Uncontrolled Calling, Traceability
"""

from typing import Dict, Any, Optional
from core_engine.state import ProjectState, Phase
from core_engine.workflow.loop_controller import LoopController


class Orchestrator:
    def __init__(
        self,
        requirement_agent=None,
        planner_agent=None,
        coding_agent=None,
        tester_agent=None,
        reviewer_agent=None,
        loop_controller: Optional[LoopController] = None
    ):
        self.requirement_agent = requirement_agent
        self.planner_agent = planner_agent
        self.coding_agent = coding_agent
        self.tester_agent = tester_agent
        self.reviewer_agent = reviewer_agent
        self.loop_controller = loop_controller or LoopController()

    def run_workflow(self, state: ProjectState, test_target: str = "tests") -> ProjectState:
        """
        Thực thi State Machine tuần tự:
        NEW -> ANALYZING -> PLANNING -> IMPLEMENTING -> TESTING -> REVIEWING -> DONE
        """
        state.current_phase = Phase.ANALYZING

        # Pha 1: Analyzing Requirements (nếu có Requirement Agent)
        if self.requirement_agent:
            state = self.requirement_agent.step(state)
        state.current_phase = Phase.PLANNING

        # Pha 2: Planning (nếu có Planner Agent)
        if self.planner_agent:
            state = self.planner_agent.step(state)
        state.current_phase = Phase.IMPLEMENTING

        # Pha 3: Implementation & Testing & Reviewing Loop
        while state.current_phase != Phase.DONE and state.current_phase != Phase.FAILED_REQUIRING_HUMAN_REVIEW:
            # Kiểm tra hạn mức vòng lặp
            if not self.loop_controller.check_and_increment(state):
                break

            # 3.1: Coding
            if self.coding_agent:
                state = self.coding_agent.step(state)

            # 3.2: Testing
            state.current_phase = Phase.TESTING
            if self.tester_agent:
                state = self.tester_agent.step(state)
            else:
                # Nếu chưa có Tester Agent riêng, dùng fallback terminal run_pytest
                if self.coding_agent:
                    test_res = self.coding_agent.terminal.run_tests(test_target=test_target)
                    state.test_results = test_res

            test_passed = state.test_results.get("is_passed", False)
            if not test_passed:
                state.current_phase = Phase.IMPLEMENTING
                continue

            # 3.3: Reviewing (khi test đã PASS)
            state.current_phase = Phase.REVIEWING
            if self.reviewer_agent:
                state = self.reviewer_agent.step(state)
                # Nếu reviewer phát hiện lỗi nghiêm trọng (severity: HIGH) -> Quay lại IMPLEMENTING
                has_high_issues = any(
                    issue.get("severity") == "HIGH" for issue in state.review_issues
                )
                if has_high_issues:
                    state.current_phase = Phase.IMPLEMENTING
                    continue

            # Nếu cả Test và Review đều đạt -> DONE
            state.current_phase = Phase.DONE

        return state
