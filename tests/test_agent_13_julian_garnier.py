"""
Module: tests/test_agent_13_julian_garnier.py
Mục đích: Kiểm thử độc lập cho Agent 13 - Julian Garnier & Bret Victor (Motion Choreographer & Interactive Experience)
"""

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
from core_engine.skills.garnier_motion_skill import GarnierMotionSkill
from core_engine.council.julian_garnier_agent import JulianGarnierAgent


class TestGarnierMotionSkill(unittest.TestCase):
    def setUp(self):
        self.skill = GarnierMotionSkill()

    def test_calculate_spring_physics(self):
        res = self.skill.calculate_spring_physics(bounce=0.25, duration=500, stiffness=120.0, damping=15.0)
        self.assertIn("parameters", res)
        self.assertIn("physics_analysis", res)
        self.assertEqual(res["parameters"]["bounce"], 0.25)
        self.assertEqual(res["parameters"]["duration"], 500)
        self.assertGreater(res["physics_analysis"]["angular_frequency_rad_s"], 0)
        self.assertIn("Underdamped", res["physics_analysis"]["regime"])
        self.assertIn("spring({ bounce: 0.25, duration: 500 })", res["animejs_v4_code"])

    def test_audit_60fps_gpu_performance_compliant(self):
        props = ["transform", "opacity", "x", "y", "scale"]
        res = self.skill.audit_60fps_gpu_performance(props)
        self.assertTrue(res["is_compliant"])
        self.assertEqual(res["performance_score"], 100)
        self.assertEqual(len(res["layout_thrashing_violations"]), 0)
        self.assertIn("will-change: transform, opacity;", res["will_change_directive"])

    def test_audit_60fps_gpu_performance_violation(self):
        props = ["top", "left", "width", "height", "margin"]
        res = self.skill.audit_60fps_gpu_performance(props)
        self.assertFalse(res["is_compliant"])
        self.assertLess(res["performance_score"], 100)
        self.assertEqual(len(res["layout_thrashing_violations"]), 5)
        self.assertIn("top", res["layout_thrashing_violations"])

    def test_generate_flip_layout_spec(self):
        res = self.skill.generate_flip_layout_spec("ExpenseList", "list", "grid")
        self.assertEqual(res["component"], "ExpenseList")
        self.assertEqual(res["transition"], "LIST ➔ GRID")
        self.assertEqual(len(res["steps"]), 4)
        self.assertIn("createLayout", res["sample_code"])

    def test_generate_doppelrand_haptic_spec(self):
        res = self.skill.generate_doppelrand_haptic_spec("BudgetCard")
        self.assertEqual(res["component"], "BudgetCard")
        self.assertIn("outer_shell", res)
        self.assertIn("inner_core", res)
        self.assertIn("haptic_physics", res)
        self.assertIn("rounded-[2rem]", res["outer_shell"]["border_radius"])
        self.assertIn("rounded-[calc(2rem-0.375rem)]", res["inner_core"]["border_radius"])

    def test_generate_motion_choreography_spec(self):
        res = self.skill.generate_motion_choreography_spec("TransactionItem", "press")
        self.assertEqual(res["target_component"], "TransactionItem")
        self.assertIn("spring_physics", res)
        self.assertIn("gpu_audit", res)
        self.assertIn("doppelrand_spec", res)
        self.assertIn("prefers-reduced-motion", res["reduced_motion_fallback"])

    def test_format_motion_spec_markdown(self):
        data = self.skill.generate_motion_choreography_spec("QuickPayButton", "click")
        md = self.skill.format_motion_spec_markdown(data)
        self.assertIn("### 9. Đặc Tả Động Lực Học & Chuyển Động", md)
        self.assertIn("Julian Garnier & Bret Victor", md)
        self.assertIn("Spring Physics", md)
        self.assertIn("Doppelrand", md)
        self.assertIn("60FPS GPU", md)


class TestJulianGarnierAgent(unittest.TestCase):
    def setUp(self):
        self.test_dir = tempfile.mkdtemp(prefix="garnier_agent_test_")
        self.workspace = Path(self.test_dir)
        (self.workspace / "core").mkdir()
        (self.workspace / "core" / "13_julian_garnier.md").write_text(
            "# Spec Julian Garnier & Bret Victor\nMotion Choreography, Anime.js v4 & Spring Physics.",
            encoding="utf-8"
        )

        self.fs = FilesystemTools(workspace_root=str(self.workspace))
        self.term = TerminalTools(workspace_root=str(self.workspace))
        self.git = GitTools(workspace_root=str(self.workspace))

    def tearDown(self):
        shutil.rmtree(self.test_dir, ignore_errors=True)

    def test_agent_initialization_and_tools(self):
        mock_llm = MockLLMClient([])
        agent = JulianGarnierAgent(mock_llm, self.fs, self.term, self.git)
        self.assertEqual(agent.name, "JulianGarnierAgent")
        self.assertIn("calculate_spring_physics", agent.tools_map)
        self.assertIn("audit_60fps_gpu_performance", agent.tools_map)
        self.assertIn("generate_flip_layout_spec", agent.tools_map)
        self.assertIn("generate_doppelrand_haptic_spec", agent.tools_map)
        self.assertIn("generate_motion_choreography_spec", agent.tools_map)

    def test_agent_step(self):
        mock_response = {
            "content": "[MOTION_CHOREOGRAPHY_SPEC]: Đặc tả Anime.js v4 Spring Physics (bounce=0.2, duration=450ms) và Doppelrand.",
            "tool_calls": []
        }
        mock_llm = MockLLMClient([mock_response])
        agent = JulianGarnierAgent(mock_llm, self.fs, self.term, self.git)

        state = ProjectState(
            task_id="TASK-MOTION-01",
            task_description="Đặc tả hoạt ảnh cho phân hệ Quản lý Đa Ví và Ghi nhận Giao dịch"
        )
        new_state = agent.step(state)

        self.assertTrue(any("MOTION_CHOREOGRAPHY_SPEC" in r for r in new_state.requirements))
        self.assertIn("Anime.js v4 Spring Physics", new_state.requirements[-1])


if __name__ == "__main__":
    unittest.main(verbosity=2)
