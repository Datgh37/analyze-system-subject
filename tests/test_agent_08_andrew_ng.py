"""
Module: tests/test_agent_08_andrew_ng.py
Mục đích: Kiểm thử độc lập cho Agent 08 - Prof. Andrew Ng & AndrewNgAISkill
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
from core_engine.skills.andrew_ng_ai_skill import AndrewNgAISkill
from core_engine.council.andrew_ng_agent import AndrewNgAgent


class TestAndrewNgAISkill(unittest.TestCase):
    def test_receipt_math_audit_exact_match(self):
        receipt_data = {
            "merchant_name": "Nhà Hàng Cơm Niêu",
            "tax_amount": 20_000,
            "total_amount": 220_000,
            "items": [
                {"name": "Cơm niêu", "quantity": 2, "unit_price": 50_000, "total_price": 100_000},
                {"name": "Cá kho tộ", "quantity": 1, "unit_price": 100_000, "total_price": 100_000}
            ]
        }
        res = AndrewNgAISkill.validate_receipt_math_audit(receipt_data)

        self.assertTrue(res["is_math_valid"])
        self.assertEqual(res["confidence_score"], 1.0)
        self.assertFalse(res["needs_human_review"])
        self.assertEqual(res["difference"], 0.0)
        self.assertEqual(res["calculated_total"], 220_000.0)

    def test_receipt_math_audit_mismatch_detected(self):
        # AI bóc tách nhầm: Tổng tiền ghi 300,000đ nhưng thực tế món chỉ cộng lại 200,000đ
        flawed_receipt = {
            "merchant_name": "Quán Cafe",
            "tax_amount": 0,
            "total_amount": 300_000,
            "items": [
                {"name": "Cà phê sữa", "quantity": 2, "unit_price": 50_000, "total_price": 100_000},
                {"name": "Trà đào", "quantity": 2, "unit_price": 50_000, "total_price": 100_000}
            ]
        }
        res = AndrewNgAISkill.validate_receipt_math_audit(flawed_receipt)

        self.assertFalse(res["is_math_valid"])
        self.assertTrue(res["needs_human_review"])
        self.assertLess(res["confidence_score"], 0.85)
        self.assertEqual(res["difference"], 100_000.0)
        self.assertIn("Human-in-the-Loop", res["recommendation"])

    def test_generate_pydantic_receipt_schema(self):
        schema = AndrewNgAISkill.generate_pydantic_receipt_schema()

        self.assertEqual(schema["title"], "ReceiptExtractionSchema")
        self.assertIn("merchant_name", schema["properties"])
        self.assertIn("total_amount", schema["properties"])
        self.assertIn("items", schema["properties"])
        self.assertIn("required", schema)

    def test_format_ai_pipeline_adr(self):
        adr_data = {
            "id": "ADR-05",
            "title": "Pipeline Bóc Tách Hóa Đơn AI",
            "status": "ACCEPTED",
            "context": "Cần bóc tách chi tiết từng mặt hàng để chia bill.",
            "decision": "Áp dụng Vision LLM và Hậu kiểm toán học."
        }
        doc = AndrewNgAISkill.format_ai_pipeline_adr(adr_data)

        self.assertIn("ADR-05", doc)
        self.assertIn("Andrew Ng", doc)
        self.assertIn("Data-Centric AI", doc)


class TestAndrewNgAgent(unittest.TestCase):
    def setUp(self):
        self.test_dir = tempfile.mkdtemp(prefix="andrew_agent_test_")
        self.workspace = Path(self.test_dir)

        (self.workspace / "core").mkdir()
        (self.workspace / "core" / "08_andrew_ng.md").write_text(
            "Bạn là Prof. Andrew Ng chuyên gia Multimodal AI & Data-Centric OCR.", encoding="utf-8"
        )

        self.fs = FilesystemTools(workspace_root=str(self.workspace))
        self.term = TerminalTools(workspace_root=str(self.workspace))
        self.git = GitTools(workspace_root=str(self.workspace))

    def tearDown(self):
        shutil.rmtree(self.test_dir, ignore_errors=True)

    def test_andrew_ng_agent_step(self):
        mock_llm = MockLLMClient([{
            "content": "Tôi đã thiết kế Pipeline bóc tách hóa đơn Multimodal AI kèm bước Hậu kiểm toán học Mathematical Audit.",
            "tool_calls": []
        }])

        agent = AndrewNgAgent(mock_llm, self.fs, self.term, self.git)
        self.assertIn("Andrew Ng", agent.system_prompt)
        self.assertIn("validate_receipt_math_audit", agent.tools_map)
        self.assertIn("generate_pydantic_receipt_schema", agent.tools_map)

        state = ProjectState(
            task_id="TASK-AI-01",
            task_description="Thiết kế Pipeline AI OCR bóc tách hóa đơn nhà hàng"
        )
        state.requirements.append("[ASYNC_DEVOPS_ADR]: Message Queue nạp ảnh")
        final_state = agent.step(state)

        self.assertTrue(len(final_state.requirements) > 1)
        self.assertIn("MULTIMODAL_AI_PIPELINE_ADR", final_state.requirements[-1])


if __name__ == "__main__":
    unittest.main(verbosity=2)
