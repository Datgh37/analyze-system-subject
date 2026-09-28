"""
Module: tests/test_agent_03_cockburn_ba.py
Mục đích: Kiểm thử độc lập cho Agent 03 - Alistair Cockburn & Debra Paul & CockburnUseCaseSkill
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
from core_engine.skills.cockburn_use_case_skill import CockburnUseCaseSkill
from core_engine.council.cockburn_ba_agent import CockburnBAAgent


class TestCockburnUseCaseSkill(unittest.TestCase):
    def test_format_ieee_use_case_spec(self):
        uc_data = {
            "id": "UC-HOUSEHOLD-01",
            "name": "Quản Trị Quỹ Chung Gia Đình",
            "goal_level": "Sea-Level (Mục tiêu người dùng)",
            "primary_actor": "Thành viên gia đình (Vợ / Chồng)",
            "pre_conditions": ["Đã đăng nhập", "Đã tham gia vào nhóm Gia đình"],
            "post_conditions": ["Số dư quỹ chung được cập nhật", "Thông báo được gửi đến các thành viên"],
            "main_flow": [
                "Người dùng mở màn hình Quỹ Gia Đình",
                "Hệ thống hiển thị tỷ lệ đóng góp (60-40) và số tiền cần nộp",
                "Người dùng bấm nút Đóng Quỹ và xác nhận chuyển khoản",
                "Hệ thống ghi nhận giao dịch và cập nhật số dư"
            ],
            "exceptions": [
                {"step": "3a. Tài khoản không đủ số dư", "handling": "Hệ thống báo lỗi và gợi ý nộp một phần"},
                {"step": "4a. Lỗi kết nối ngân hàng", "handling": "Hệ thống lưu trạng thái PENDING và thử lại sau 30s"}
            ],
            "nfrs": ["Bảo mật giao dịch bằng chữ ký số", "Thời gian xử lý < 1 giây"],
            "gherkin_scenarios": "Scenario: Đóng quỹ thành công\n  Given Quỹ cần đóng 10tr\n  When Chồng nộp 6tr\n  Then Số dư quỹ tăng 6tr"
        }

        doc = CockburnUseCaseSkill.format_ieee_use_case_spec(uc_data)
        self.assertIn("IEEE 29148", doc)
        self.assertIn("UC-HOUSEHOLD-01", doc)
        self.assertIn("3a. Tài khoản không đủ số dư", doc)
        self.assertIn("Scenario: Đóng quỹ thành công", doc)

    def test_generate_gherkin_scenarios(self):
        scenarios = [
            {
                "title": "Hoàn ứng thành công khi quỹ đủ tiền",
                "given": "Quỹ chung đang có số dư 3,000,000đ",
                "when": "Thành viên yêu cầu hoàn ứng 500,000đ mua đồ chung",
                "then": "Hệ thống duyệt chi và số dư quỹ giảm còn 2,500,000đ"
            },
            {
                "title": "Cảnh báo khi quỹ không đủ tiền hoàn ứng",
                "given": "Quỹ chung đang có số dư 200,000đ",
                "when": "Thành viên yêu cầu hoàn ứng 800,000đ",
                "then": "Hệ thống chuyển trạng thái PENDING_FUND_INJECTION và thông báo thiếu 600,000đ"
            }
        ]

        gherkin_text = CockburnUseCaseSkill.generate_gherkin_scenarios("Phân hệ Hoàn Ứng Quỹ", scenarios)
        self.assertIn("Feature: Phân hệ Hoàn Ứng Quỹ", gherkin_text)
        self.assertIn("Scenario: Hoàn ứng thành công khi quỹ đủ tiền", gherkin_text)
        self.assertIn("Given Quỹ chung đang có số dư 3,000,000đ", gherkin_text)
        self.assertIn("Then Hệ thống chuyển trạng thái PENDING_FUND_INJECTION", gherkin_text)

    def test_validate_use_case_completeness(self):
        # Trường hợp 1: Use case đầy đủ chuẩn
        full_uc = {
            "id": "UC-01",
            "name": "Nộp Quỹ",
            "primary_actor": "User",
            "pre_conditions": ["Logged in"],
            "main_flow": ["B1", "B2", "B3", "B4"],
            "exceptions": [{"step": "3a", "handling": "Retry"}],
            "gherkin_scenarios": "Given... When... Then..."
        }
        res_full = CockburnUseCaseSkill.validate_use_case_completeness(full_uc)
        self.assertEqual(res_full["completeness_score"], 100)
        self.assertTrue(res_full["is_ready_for_architecture"])
        self.assertEqual(res_full["verdict"], "ĐẠT CHUẨN IEEE 29148")

        # Trường hợp 2: Use case thiếu luồng ngoại lệ (Exceptions)
        flawed_uc = {
            "id": "UC-02",
            "name": "Xem Báo Cáo",
            "primary_actor": "User",
            "pre_conditions": ["Logged in"],
            "main_flow": ["B1"]
        }
        res_flawed = CockburnUseCaseSkill.validate_use_case_completeness(flawed_uc)
        self.assertFalse(res_flawed["is_ready_for_architecture"])
        self.assertTrue(any("Thiếu luồng ngoại lệ" in err for err in res_flawed["missing_elements"]))


class TestCockburnBAAgent(unittest.TestCase):
    def setUp(self):
        self.test_dir = tempfile.mkdtemp(prefix="ba_agent_test_")
        self.workspace = Path(self.test_dir)

        (self.workspace / "core").mkdir()
        (self.workspace / "core" / "03_cockburn_ba.md").write_text(
            "Bạn là Alistair Cockburn chuyên gia Use Case IEEE 29148.", encoding="utf-8"
        )

        self.fs = FilesystemTools(workspace_root=str(self.workspace))
        self.term = TerminalTools(workspace_root=str(self.workspace))
        self.git = GitTools(workspace_root=str(self.workspace))

    def tearDown(self):
        shutil.rmtree(self.test_dir, ignore_errors=True)

    def test_ba_agent_step(self):
        mock_llm = MockLLMClient([{
            "content": "Tôi đã chuẩn hóa Use Case Quản Trị Quỹ Gia Đình theo chuẩn IEEE 29148 với đầy đủ Luồng chính và Ngoại lệ.",
            "tool_calls": []
        }])

        agent = CockburnBAAgent(mock_llm, self.fs, self.term, self.git)
        self.assertIn("Alistair Cockburn", agent.system_prompt)
        self.assertIn("format_ieee_use_case_spec", agent.tools_map)
        self.assertIn("generate_gherkin_scenarios", agent.tools_map)

        state = ProjectState(
            task_id="TASK-BA-01",
            task_description="Thiết kế đặc tả Use Case cho Quỹ Gia Đình"
        )
        state.requirements.append("[INSIGHT]: Cần đóng góp theo tỷ lệ 60-40")
        final_state = agent.step(state)

        self.assertTrue(len(final_state.requirements) > 1)
        self.assertIn("IEEE_29148_USE_CASE_SPEC", final_state.requirements[-1])


if __name__ == "__main__":
    unittest.main(verbosity=2)
