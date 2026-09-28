"""
Module: tests/test_frontend_ui_spec_generation.py
Mục đích: Kiểm thử độc lập khả năng sinh mã Frontend UI/UX Spec (02b), Component Code đa nền tảng và Quality Gate
"""

import sys
import shutil
import tempfile
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).parent.parent))

from core_engine.skills.frost_atomic_design_skill import FrostAtomicDesignSkill
from core_engine.skills.abramov_client_state_skill import AbramovClientStateSkill
from core_engine.skills.orchestrator_rfc_skill import OrchestratorRFCSkill


class TestFrontendUISpecGeneration(unittest.TestCase):
    def setUp(self):
        self.frost = FrostAtomicDesignSkill()
        self.abramov = AbramovClientStateSkill()
        self.orchestrator = OrchestratorRFCSkill()
        self.test_dir = tempfile.mkdtemp()

    def tearDown(self):
        shutil.rmtree(self.test_dir, ignore_errors=True)

    def test_generate_frontend_ui_spec_document(self):
        doc = self.frost.generate_frontend_ui_spec_document(
            feature_name="JointHouseholdFund",
            uc_id="UC-HOUSEHOLD-01"
        )
        self.assertIn("FRONTEND UI/UX SPECIFICATION", doc)
        self.assertIn("Design Tokens Catalog", doc)
        self.assertIn("Atomic Component Hierarchy", doc)
        self.assertIn("WCAG 2.1 AA Checklist", doc)
        self.assertIn("Zod Schema", doc)
        self.assertIn("Production Component Implementation", doc)

    def test_generate_production_component_code_react(self):
        code = self.abramov.generate_production_component_code("JointPoolCard", framework="react")
        self.assertIn("import React", code)
        self.assertIn("export const JointPoolCard", code)
        self.assertIn("Intl.NumberFormat", code)

    def test_generate_production_component_code_angular(self):
        code = self.abramov.generate_production_component_code("JointPoolCard", framework="angular")
        self.assertIn("@Component", code)
        self.assertIn("export class JointPoolCardComponent", code)
        self.assertIn("signal<boolean>", code)

    def test_generate_production_component_code_blazor(self):
        code = self.abramov.generate_production_component_code("JointPoolCard", framework="blazor")
        self.assertIn("@code", code)
        self.assertIn("[Parameter]", code)
        self.assertIn("decimal Amount", code)

    def test_session_integrity_with_5_artifacts(self):
        session_path = Path(self.test_dir)
        (session_path / "01_user_feedback.md").write_text("# USER FEEDBACK\nValid feedback content > 50 bytes for test suite verification.", encoding="utf-8")
        (session_path / "02_use_case_specs.md").write_text("# USE CASE SPECS\nValid use case specs content > 50 bytes for test suite verification. Exception flow: 3a. Ngoại lệ.", encoding="utf-8")
        (session_path / "02b_frontend_ui_spec.md").write_text("# FRONTEND UI SPEC\nValid frontend tokens color and WCAG contrast ratio > 50 bytes.", encoding="utf-8")
        (session_path / "03_technical_adr.md").write_text("# TECHNICAL ADR\nValid technical ADR with Trade-offs, DECIMAL(15,2), and SELECT FOR UPDATE locks.", encoding="utf-8")
        (session_path / "04_final_rfc.md").write_text("# FINAL RFC\nValid final RFC content > 50 bytes for test suite verification.", encoding="utf-8")

        result = self.orchestrator.verify_session_integrity(self.test_dir)
        self.assertTrue(result["is_valid"])
        self.assertEqual(result["integrity_score"], 100)
        self.assertEqual(len(result["missing_files"]), 0)


if __name__ == "__main__":
    unittest.main(verbosity=2)
