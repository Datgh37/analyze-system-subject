"""
Module: tests/test_tools.py
Mục đích: Kiểm thử độc lập (Unit Test) cho Tầng Công Cụ An Toàn (Tool Layer)
Sử dụng thư viện chuẩn unittest (Không cần cài đặt thêm pip package).
"""

import os
import sys
import unittest
import tempfile
import shutil
from pathlib import Path

# Đảm bảo import được package core_engine
sys.path.insert(0, str(Path(__file__).parent.parent))

from core_engine.tools.filesystem import FilesystemTools, SecuritySandboxError
from core_engine.tools.terminal import TerminalTools
from core_engine.tools.git_tools import GitTools


class TestFilesystemTools(unittest.TestCase):
    def setUp(self):
        """Tạo thư mục workspace tạm thời để kiểm thử cô lập."""
        self.test_dir = tempfile.mkdtemp(prefix="sandbox_workspace_")
        self.workspace = Path(self.test_dir)
        
        # Tạo cấu trúc thư mục và file mẫu
        (self.workspace / "src").mkdir()
        (self.workspace / "src" / "main.py").write_text("def hello():\n    return 'world'\n", encoding="utf-8")
        (self.workspace / "README.md").write_text("# Test Project\nSample description line.\n", encoding="utf-8")
        
        self.fs = FilesystemTools(workspace_root=str(self.workspace))

    def tearDown(self):
        """Xóa thư mục tạm sau khi test xong."""
        shutil.rmtree(self.test_dir, ignore_errors=True)

    def test_list_files(self):
        files = self.fs.list_files()
        self.assertIn("README.md", files)
        self.assertIn("src/main.py", files)

    def test_read_file(self):
        content = self.fs.read_file("README.md")
        self.assertIn("Test Project", content)
        self.assertIn("1: # Test Project", content)

    def test_read_file_partial(self):
        content = self.fs.read_file("src/main.py", start_line=2, end_line=2)
        self.assertIn("return 'world'", content)
        self.assertNotIn("def hello", content)

    def test_write_and_edit_file(self):
        # Ghi file mới
        write_res = self.fs.write_file("docs/guide.txt", "Line 1\nLine 2 target_text\nLine 3\n")
        self.assertIn("[SUCCESS]", write_res)
        self.assertTrue((self.workspace / "docs" / "guide.txt").exists())

        # Sửa file
        edit_res = self.fs.edit_file("docs/guide.txt", "target_text", "replaced_text")
        self.assertIn("[SUCCESS]", edit_res)
        
        # Đọc lại kiểm tra
        updated_content = (self.workspace / "docs" / "guide.txt").read_text(encoding="utf-8")
        self.assertIn("replaced_text", updated_content)
        self.assertNotIn("target_text", updated_content)

    def test_search_code(self):
        matches = self.fs.search_code("hello")
        self.assertEqual(len(matches), 1)
        self.assertEqual(matches[0]["file"], "src/main.py")
        self.assertEqual(matches[0]["line_number"], 1)

    def test_path_traversal_security_sandbox(self):
        # Cố tình truy cập file bên ngoài workspace
        with self.assertRaises(SecuritySandboxError):
            self.fs.read_file("../../outside.txt")


class TestTerminalTools(unittest.TestCase):
    def setUp(self):
        self.test_dir = tempfile.mkdtemp(prefix="sandbox_term_")
        self.workspace = Path(self.test_dir)
        self.term = TerminalTools(workspace_root=str(self.workspace))

    def tearDown(self):
        shutil.rmtree(self.test_dir, ignore_errors=True)

    def test_run_safe_command(self):
        res = self.term.run_command('python -c "print(123 + 456)"')
        self.assertTrue(res["success"])
        self.assertEqual(res["stdout"].strip(), "579")
        self.assertEqual(res["exit_code"], 0)

    def test_block_dangerous_command(self):
        res = self.term.run_command("format C: /Q")
        self.assertFalse(res["success"])
        self.assertIn("[SECURITY_ALERT]", res["stderr"])
        self.assertEqual(res["exit_code"], -1)

    def test_command_timeout(self):
        # Chạy script ngủ 5s với timeout 1s
        res = self.term.run_command('python -c "import time; time.sleep(5)"', timeout_seconds=1)
        self.assertFalse(res["success"])
        self.assertIn("[TIMEOUT_ERROR]", res["stderr"])
        self.assertEqual(res["exit_code"], 124)


if __name__ == "__main__":
    unittest.main(verbosity=2)
