"""
Module: tests/test_agent_09_brad_frost.py
Mục đích: Kiểm thử độc lập cho Agent 09: Brad Frost & Don Norman và FrostAtomicDesignSkill
"""

import sys
import shutil
import tempfile
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).parent.parent))

from core_engine.skills.frost_atomic_design_skill import FrostAtomicDesignSkill
from core_engine.council.brad_frost_agent import BradFrostAgent
from core_engine.tools.filesystem import FilesystemTools
from core_engine.tools.terminal import TerminalTools
from core_engine.tools.git_tools import GitTools
from core_engine.llm_client import MockLLMClient
from core_engine.state import ProjectState


class TestBradFrostAgent(unittest.TestCase):
    def setUp(self):
        self.skill = FrostAtomicDesignSkill()
        self.test_dir = tempfile.mkdtemp()
        self.fs = FilesystemTools(self.test_dir)
        self.term = TerminalTools(self.test_dir)
        self.git = GitTools(self.test_dir)

    def tearDown(self):
        shutil.rmtree(self.test_dir, ignore_errors=True)

    def test_generate_design_tokens(self):
        tokens = self.skill.generate_design_tokens()
        self.assertIn("colors", tokens)
        self.assertIn("typography", tokens)
        self.assertIn("spacing", tokens)
        self.assertIn("elevation", tokens)
        self.assertIn("primary", tokens["colors"]["brand"])
        self.assertEqual(tokens["spacing"]["xs"], "4px")

    def test_generate_atomic_component_tree(self):
        tree = self.skill.generate_atomic_component_tree("HouseholdFund")
        self.assertIn("atoms", tree)
        self.assertIn("molecules", tree)
        self.assertIn("organisms", tree)
        self.assertTrue(any("AmountDisplay" in a for a in tree["atoms"]))
        self.assertTrue(any("JointPoolFundCard" in o for o in tree["organisms"]))

    def test_evaluate_wcag_accessibility(self):
        # 1. Dark Slate on White -> Should pass AAA (> 7.0)
        res_pass = self.skill.evaluate_wcag_accessibility("#0F172A", "#FFFFFF")
        self.assertGreater(res_pass["contrast_ratio"], 7.0)
        self.assertTrue(res_pass["passes_aa_normal_text"])
        self.assertTrue(res_pass["passes_aaa"])

        # 2. Light Grey on White -> Should fail
        res_fail = self.skill.evaluate_wcag_accessibility("#CBD5E1", "#FFFFFF")
        self.assertLess(res_fail["contrast_ratio"], 3.0)
        self.assertFalse(res_fail["passes_aa_normal_text"])

    def test_generate_wireframe_spec(self):
        spec = self.skill.generate_wireframe_spec("HouseholdFund")
        self.assertIn("BẢN VẼ WIREFRAME", spec)
        self.assertIn("QUỸ GIA ĐÌNH", spec)
        self.assertIn("+--------------------------------------------------+", spec)

    def test_brad_frost_agent_step(self):
        mock_llm = MockLLMClient([
            {
                "content": "Tôi đã thiết kế xong bảng Design Tokens, Atomic Component Tree và Wireframe cho Quỹ gia đình.",
                "tool_calls": []
            }
        ])

        agent = BradFrostAgent(
            llm_client=mock_llm,
            filesystem=self.fs,
            terminal=self.term,
            git=self.git
        )

        state = ProjectState(
            task_id="TEST-09",
            task_description="Quản Lý Quỹ Gia Đình"
        )

        final_state = agent.step(state)
        self.assertEqual(len(final_state.requirements), 1)
        self.assertIn("ATOMIC_DESIGN_UI_SPEC", final_state.requirements[0])


if __name__ == "__main__":
    unittest.main(verbosity=2)
