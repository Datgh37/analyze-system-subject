"""
Module: tests/test_orchestrator.py
Mục đích: Kiểm thử State Machine và luồng điều phối của Orchestrator
"""

import sys
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).parent.parent))

from core_engine.state import ProjectState, Phase
from core_engine.workflow.orchestrator import Orchestrator
from core_engine.workflow.loop_controller import LoopController


class MockAgent:
    def __init__(self, name: str, action=None):
        self.name = name
        self.action = action

    def step(self, state: ProjectState) -> ProjectState:
        if self.action:
            return self.action(state)
        return state


class TestOrchestrator(unittest.TestCase):
    def test_full_successful_workflow(self):
        """
        Kịch bản 1: Luồng hoàn hảo
        Analyzing -> Planning -> Implementing -> Testing (PASS) -> Reviewing (PASS) -> DONE
        """
        def req_step(state):
            state.requirements = ["FR-001: Xử lý chia nợ nhóm"]
            return state

        def plan_step(state):
            state.plan_steps = ["1. Viết code graph_solver", "2. Viết unit test"]
            return state

        def coder_step(state):
            state.changed_files = ["domain/graph_solver.py"]
            return state

        def tester_step(state):
            state.test_results = {"is_passed": True, "status": "PASS"}
            return state

        def reviewer_step(state):
            state.review_issues = []  # Không có issue nào
            return state

        orchestrator = Orchestrator(
            requirement_agent=MockAgent("Req", req_step),
            planner_agent=MockAgent("Plan", plan_step),
            coding_agent=MockAgent("Coder", coder_step),
            tester_agent=MockAgent("Tester", tester_step),
            reviewer_agent=MockAgent("Reviewer", reviewer_step)
        )

        state = ProjectState(task_id="T1", task_description="Implement graph solver")
        final_state = orchestrator.run_workflow(state)

        self.assertEqual(final_state.current_phase, Phase.DONE)
        self.assertEqual(final_state.iteration_count, 1)
        self.assertEqual(len(final_state.requirements), 1)
        self.assertEqual(len(final_state.plan_steps), 2)

    def test_reviewer_rejection_loops_back_to_coder(self):
        """
        Kịch bản 2: Tester PASS nhưng Reviewer phát hiện lỗi HIGH severity -> Quay lại Coder sửa tiếp.
        """
        attempts = 0

        def coder_step(state):
            nonlocal attempts
            attempts += 1
            return state

        def tester_step(state):
            state.test_results = {"is_passed": True, "status": "PASS"}
            return state

        def reviewer_step(state):
            nonlocal attempts
            if attempts == 1:
                # Vòng 1: Reviewer từ chối vì lỗi bảo mật
                state.review_issues = [{"severity": "HIGH", "description": "Lỗi SQL Injection"}]
            else:
                # Vòng 2: Reviewer duyệt
                state.review_issues = []
            return state

        orchestrator = Orchestrator(
            coding_agent=MockAgent("Coder", coder_step),
            tester_agent=MockAgent("Tester", tester_step),
            reviewer_agent=MockAgent("Reviewer", reviewer_step),
            loop_controller=LoopController(max_repair_iterations=3)
        )

        state = ProjectState(task_id="T2", task_description="Review rejection test")
        final_state = orchestrator.run_workflow(state)

        self.assertEqual(final_state.current_phase, Phase.DONE)
        self.assertEqual(final_state.iteration_count, 2)
        self.assertEqual(attempts, 2)


if __name__ == "__main__":
    unittest.main(verbosity=2)
