"""
Module: tests/test_agent_11_bruce_schneier.py
Mục đích: Kiểm thử độc lập cho Agent 11: Bruce Schneier & OWASP và SchneierSecuritySkill
"""

import sys
import shutil
import tempfile
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).parent.parent))

from core_engine.skills.schneier_security_skill import SchneierSecuritySkill
from core_engine.council.bruce_schneier_agent import BruceSchneierAgent
from core_engine.tools.filesystem import FilesystemTools
from core_engine.tools.terminal import TerminalTools
from core_engine.tools.git_tools import GitTools
from core_engine.llm_client import MockLLMClient
from core_engine.state import ProjectState


class TestBruceSchneierAgent(unittest.TestCase):
    def setUp(self):
        self.skill = SchneierSecuritySkill()
        self.test_dir = tempfile.mkdtemp()
        self.fs = FilesystemTools(self.test_dir)
        self.term = TerminalTools(self.test_dir)
        self.git = GitTools(self.test_dir)

    def tearDown(self):
        shutil.rmtree(self.test_dir, ignore_errors=True)

    def test_design_stride_threat_model(self):
        model = self.skill.design_stride_threat_model("JointHouseholdFund")
        self.assertIn("threat_matrix", model)
        matrix = model["threat_matrix"]
        self.assertIn("S_Spoofing", matrix)
        self.assertIn("T_Tampering", matrix)
        self.assertIn("R_Repudiation", matrix)
        self.assertIn("I_Information_Disclosure", matrix)
        self.assertIn("D_Denial_of_Service", matrix)
        self.assertIn("E_Elevation_of_Privilege", matrix)
        self.assertEqual(matrix["S_Spoofing"]["severity"], "CRITICAL")

    def test_simulate_aes_encryption_workflow(self):
        plain_text = "STK: 0123456789 (Vietcombank) - So du: 25,000,000 VND"
        res = self.skill.simulate_aes_encryption_workflow(plain_text)

        self.assertEqual(res["algorithm"], "AES-256-GCM")
        self.assertEqual(res["key_size_bits"], 256)
        self.assertTrue(res["is_valid"])
        self.assertEqual(res["decrypted_validation"], plain_text)
        self.assertTrue(res["encrypted_payload"].startswith("enc:"))

    def test_design_jwt_token_lifecycle(self):
        lifecycle = self.skill.design_jwt_token_lifecycle()
        self.assertIn("token_specifications", lifecycle)
        self.assertIn("access_token", lifecycle["token_specifications"])
        self.assertIn("refresh_token", lifecycle["token_specifications"])
        self.assertEqual(lifecycle["token_specifications"]["access_token"]["lifespan"], "15 minutes")
        self.assertIn("Redis", lifecycle["revocation_mechanism"])

    def test_format_security_compliance_adr(self):
        adr = self.skill.format_security_compliance_adr("JointHouseholdFund")
        self.assertIn("ĐẶC TẢ AN NINH BẢO MẬT", adr)
        self.assertIn("STRIDE Threat Model", adr)
        self.assertIn("AES-256-GCM", adr)
        self.assertIn("JWT Rotation", adr)

    def test_bruce_schneier_agent_step(self):
        mock_llm = MockLLMClient([
            {
                "content": "Tôi đã hoàn thành phân tích STRIDE Threat Model và cơ chế mã hóa AES-256-GCM cho Quỹ gia đình.",
                "tool_calls": []
            }
        ])

        agent = BruceSchneierAgent(
            llm_client=mock_llm,
            filesystem=self.fs,
            terminal=self.term,
            git=self.git
        )

        state = ProjectState(
            task_id="TEST-11",
            task_description="Bảo Vệ An Ninh Quỹ Gia Đình & Mã Hóa Dữ Liệu"
        )

        final_state = agent.step(state)
        self.assertEqual(len(final_state.requirements), 1)
        self.assertIn("FINTECH_SECURITY_STRIDE_ADR", final_state.requirements[0])


if __name__ == "__main__":
    unittest.main(verbosity=2)
