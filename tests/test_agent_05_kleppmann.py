"""
Module: tests/test_agent_05_kleppmann.py
Mục đích: Kiểm thử độc lập cho Agent 05 - Martin Kleppmann & Pramod Sadalage & KleppmannDatabaseSkill
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
from core_engine.skills.kleppmann_database_skill import KleppmannDatabaseSkill
from core_engine.council.martin_kleppmann_agent import MartinKleppmannAgent


class TestKleppmannDatabaseSkill(unittest.TestCase):
    def test_validate_3nf_schema_success(self):
        # Lược đồ chuẩn 3NF: Tách riêng users, categories, expenses
        valid_tables = {
            "users": {
                "primary_key": "id",
                "columns": {"id": "UUID", "name": "VARCHAR(255)", "email": "VARCHAR(255)"}
            },
            "categories": {
                "primary_key": "id",
                "columns": {"id": "UUID", "name": "VARCHAR(100)", "icon": "VARCHAR(50)"}
            },
            "expenses": {
                "primary_key": "id",
                "columns": {
                    "id": "UUID",
                    "payer_id": "UUID",
                    "category_id": "UUID",
                    "amount": "DECIMAL(15, 2)",
                    "created_at": "TIMESTAMP"
                }
            }
        }
        res = KleppmannDatabaseSkill.validate_3nf_schema(valid_tables)
        self.assertTrue(res["is_3nf"])
        self.assertEqual(res["verdict"], "LƯỢC ĐỒ ĐẠT CHUẨN 3NF & AN TOÀN TIỀN TỆ")
        self.assertEqual(len(res["violations"]), 0)

    def test_validate_3nf_schema_violation_transitive(self):
        # Vi phạm 3NF: Để lẫn category_name vào bảng expenses (Phụ thuộc bắc cầu: expense -> category_id -> category_name)
        flawed_tables = {
            "expenses": {
                "primary_key": "id",
                "columns": {
                    "id": "UUID",
                    "category_id": "UUID",
                    "category_name": "VARCHAR(100)",
                    "amount": "DECIMAL(15, 2)"
                }
            }
        }
        res = KleppmannDatabaseSkill.validate_3nf_schema(flawed_tables)
        self.assertFalse(res["is_3nf"])
        self.assertTrue(any("Phụ thuộc bắc cầu" in v for v in res["violations"]))

    def test_validate_3nf_schema_violation_float_money(self):
        # Vi phạm kiểu dữ liệu tiền tệ: Dùng FLOAT thay vì DECIMAL
        float_tables = {
            "expenses": {
                "primary_key": "id",
                "columns": {
                    "id": "UUID",
                    "amount": "FLOAT"
                }
            }
        }
        res = KleppmannDatabaseSkill.validate_3nf_schema(float_tables)
        self.assertFalse(res["is_3nf"])
        self.assertTrue(any("LỖI TIỀN TỆ" in v for v in res["violations"]))

    def test_evaluate_concurrency_lock_strategy(self):
        # 1. Giao dịch rút tiền quỹ chung -> Pessimistic Lock
        lock_fin = KleppmannDatabaseSkill.evaluate_concurrency_lock_strategy("WITHDRAW_FROM_POOL", is_financial=True)
        self.assertEqual(lock_fin["strategy"], "PESSIMISTIC_LOCKING")
        self.assertIn("FOR UPDATE", lock_fin["implementation"])

        # 2. Xử lý OCR chạy ngầm -> Redis Distributed Lock
        lock_ocr = KleppmannDatabaseSkill.evaluate_concurrency_lock_strategy("ASYNC_OCR_PROCESS", is_financial=False)
        self.assertEqual(lock_ocr["strategy"], "DISTRIBUTED_LOCK")
        self.assertIn("Redlock", lock_ocr["implementation"])

        # 3. Sửa tên nhóm -> Optimistic Lock
        lock_opt = KleppmannDatabaseSkill.evaluate_concurrency_lock_strategy("UPDATE_GROUP_NAME", is_financial=False)
        self.assertEqual(lock_opt["strategy"], "OPTIMISTIC_LOCKING")

    def test_format_db_schema_spec(self):
        schema_data = {
            "title": "CSDL Phân Hệ Quản Lý Chi Tiêu & Quỹ",
            "tables": [
                {
                    "name": "expenses",
                    "description": "Khoản chi tiêu",
                    "primary_key": "id",
                    "foreign_keys": [{"column": "payer_id", "references": "users(id)"}],
                    "indexes": ["idx_expenses_group_created (group_id, created_at DESC)"],
                    "columns": [
                        {"name": "id", "type": "UUID", "constraints": "PRIMARY KEY", "note": "Mã định danh"},
                        {"name": "amount", "type": "DECIMAL(15,2)", "constraints": "NOT NULL", "note": "Số tiền"}
                    ]
                }
            ]
        }
        doc = KleppmannDatabaseSkill.format_db_schema_spec(schema_data)
        self.assertIn("Martin Kleppmann", doc)
        self.assertIn("Polyglot Persistence", doc)
        self.assertIn("expenses", doc)
        self.assertIn("DECIMAL(15,2)", doc)


class TestMartinKleppmannAgent(unittest.TestCase):
    def setUp(self):
        self.test_dir = tempfile.mkdtemp(prefix="kleppmann_agent_test_")
        self.workspace = Path(self.test_dir)

        (self.workspace / "core").mkdir()
        (self.workspace / "core" / "05_martin_kleppmann.md").write_text(
            "Bạn là Martin Kleppmann chuyên gia Universal Database & Polyglot Persistence.", encoding="utf-8"
        )

        self.fs = FilesystemTools(workspace_root=str(self.workspace))
        self.term = TerminalTools(workspace_root=str(self.workspace))
        self.git = GitTools(workspace_root=str(self.workspace))

    def tearDown(self):
        shutil.rmtree(self.test_dir, ignore_errors=True)

    def test_kleppmann_agent_step(self):
        mock_llm = MockLLMClient([{
            "content": "Tôi đã thiết kế lược đồ CSDL PostgreSQL chuẩn 3NF, kèm JSONB cho dữ liệu hóa đơn và Pessimistic Locking cho Quỹ chung.",
            "tool_calls": []
        }])

        agent = MartinKleppmannAgent(mock_llm, self.fs, self.term, self.git)
        self.assertIn("Martin Kleppmann", agent.system_prompt)
        self.assertIn("validate_3nf_schema", agent.tools_map)
        self.assertIn("evaluate_concurrency_lock_strategy", agent.tools_map)

        state = ProjectState(
            task_id="TASK-DB-01",
            task_description="Thiết kế CSDL 3NF và Concurrency Lock cho Phân hệ Quỹ & Chi Tiêu"
        )
        state.requirements.append("[CLEAN_ARCHITECTURE_ADR]: Domain model Quỹ và Chi tiêu")
        final_state = agent.step(state)

        self.assertTrue(len(final_state.requirements) > 1)
        self.assertIn("DATABASE_3NF_POLYGLOT_SPEC", final_state.requirements[-1])


if __name__ == "__main__":
    unittest.main(verbosity=2)
