"""
Module: tests/test_agent_07_werner_vogels.py
Mục đích: Kiểm thử độc lập cho Agent 07 - Dr. Werner Vogels & VogelsAsyncSkill
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
from core_engine.skills.vogels_async_skill import VogelsAsyncSkill
from core_engine.council.werner_vogels_agent import WernerVogelsAgent


class TestVogelsAsyncSkill(unittest.TestCase):
    def test_idempotency_new_request(self):
        cache_store = {}
        payload = {"amount": 500_000, "payer": "An", "group_id": "GRP-01"}
        key = "IDEMP-KEY-12345"

        res = VogelsAsyncSkill.validate_idempotency(cache_store, key, payload)
        self.assertFalse(res["is_duplicate"])
        self.assertFalse(res["is_conflict"])
        self.assertIn(key, cache_store)

    def test_idempotency_duplicate_blocking(self):
        cache_store = {}
        payload = {"amount": 500_000, "payer": "An", "group_id": "GRP-01"}
        key = "IDEMP-KEY-12345"

        # Lần 1: Thành công
        VogelsAsyncSkill.validate_idempotency(cache_store, key, payload)

        # Lần 2: Gửi lại y hệt -> Phải chặn và báo duplicate
        res_dup = VogelsAsyncSkill.validate_idempotency(cache_store, key, payload)
        self.assertTrue(res_dup["is_duplicate"])
        self.assertFalse(res_dup["is_conflict"])
        self.assertIn("gửi lặp lại", res_dup["message"])

    def test_idempotency_payload_conflict(self):
        cache_store = {}
        payload_1 = {"amount": 500_000, "payer": "An"}
        payload_2 = {"amount": 900_000, "payer": "An"}  # Khác số tiền
        key = "IDEMP-KEY-CONFLICT"

        VogelsAsyncSkill.validate_idempotency(cache_store, key, payload_1)
        res_conf = VogelsAsyncSkill.validate_idempotency(cache_store, key, payload_2)
        self.assertTrue(res_conf["is_conflict"])

    def test_simulate_async_job_submission(self):
        res = VogelsAsyncSkill.simulate_async_job_submission(
            task_name="PROCESS_OCR_RECEIPT",
            payload={"image_url": "https://storage/receipt.jpg"}
        )
        self.assertEqual(res["http_status"], 202)
        self.assertEqual(res["status"], "QUEUED")
        self.assertTrue(len(res["job_id"]) > 10)
        self.assertIn(res["job_id"], res["poll_endpoint"])

    def test_format_async_devops_adr(self):
        adr_data = {
            "id": "ADR-04",
            "title": "Kiến Trúc Message Queue & Idempotency",
            "status": "ACCEPTED",
            "context": "OCR xử lý lâu cần Message Queue.",
            "decision": "Áp dụng Redis Streams và Idempotency Key."
        }
        doc = VogelsAsyncSkill.format_async_devops_adr(adr_data)
        self.assertIn("ADR-04", doc)
        self.assertIn("Werner Vogels", doc)
        self.assertIn("Message Queue", doc)


class TestWernerVogelsAgent(unittest.TestCase):
    def setUp(self):
        self.test_dir = tempfile.mkdtemp(prefix="vogels_agent_test_")
        self.workspace = Path(self.test_dir)

        (self.workspace / "core").mkdir()
        (self.workspace / "core" / "07_werner_vogels.md").write_text(
            "Bạn là Dr. Werner Vogels CTO Amazon chuyên gia Async & DevOps.", encoding="utf-8"
        )

        self.fs = FilesystemTools(workspace_root=str(self.workspace))
        self.term = TerminalTools(workspace_root=str(self.workspace))
        self.git = GitTools(workspace_root=str(self.workspace))

    def tearDown(self):
        shutil.rmtree(self.test_dir, ignore_errors=True)

    def test_vogels_agent_step(self):
        mock_llm = MockLLMClient([{
            "content": "Tôi đã thiết kế kiến trúc Message Queue với HTTP 202 Accepted cho OCR và cơ chế Idempotency chống gửi lặp.",
            "tool_calls": []
        }])

        agent = WernerVogelsAgent(mock_llm, self.fs, self.term, self.git)
        self.assertIn("Werner Vogels", agent.system_prompt)
        self.assertIn("validate_idempotency", agent.tools_map)
        self.assertIn("simulate_async_job_submission", agent.tools_map)

        state = ProjectState(
            task_id="TASK-ASYNC-01",
            task_description="Thiết kế Hàng chờ Bất đồng bộ cho tác vụ AI OCR Hóa Đơn"
        )
        state.requirements.append("[GRAPH_DEBT_SOLVER_SPEC]: Đồ thị nợ nhóm")
        final_state = agent.step(state)

        self.assertTrue(len(final_state.requirements) > 1)
        self.assertIn("ASYNC_DEVOPS_ADR", final_state.requirements[-1])


if __name__ == "__main__":
    unittest.main(verbosity=2)
