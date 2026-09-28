"""
Module: core_engine.tools.git_tools
Mục đích: Tầng quản lý phiên bản Git và Workspace Isolation cho Agent
Tuân thủ nguyên tắc Claude: Traceability, Rollback Capability, Change Inspection
"""

import os
from pathlib import Path
from typing import Dict, Any, Optional
from core_engine.tools.terminal import TerminalTools


class GitTools:
    def __init__(self, workspace_root: Optional[str] = None):
        if workspace_root is None:
            workspace_root = os.getcwd()
        self.workspace_root = Path(workspace_root).resolve()
        self.terminal = TerminalTools(workspace_root=str(self.workspace_root))

    def git_status(self) -> str:
        """Kiểm tra trạng thái thay đổi hiện tại của Git (git status --short)."""
        res = self.terminal.run_command("git status --short")
        if not res["success"]:
            return f"[GIT_ERROR] Không thể lấy trạng thái git: {res['stderr']}"
        return res["stdout"] if res["stdout"] else "[CLEAN] Workspace sạch sẽ, không có file nào bị thay đổi."

    def git_diff(self, file_path: Optional[str] = None) -> str:
        """Xem chi tiết thay đổi mã nguồn trước khi commit (git diff)."""
        cmd = f"git diff {file_path}" if file_path else "git diff"
        res = self.terminal.run_command(cmd)
        if not res["success"]:
            return f"[GIT_ERROR] Không thể lấy git diff: {res['stderr']}"
        return res["stdout"] if res["stdout"] else "[NO_DIFF] Không có sự khác biệt nào so với commit gần nhất."

    def git_commit(self, message: str) -> str:
        """Ghi nhận thay đổi thành một commit (git add -A && git commit -m '...')."""
        # Escape single/double quotes trong message
        clean_msg = message.replace('"', '\\"')
        cmd = f'git add -A && git commit -m "{clean_msg}"'
        res = self.terminal.run_command(cmd)
        if not res["success"]:
            return f"[GIT_ERROR] Lỗi khi commit: {res['stderr'] if res['stderr'] else res['stdout']}"
        return f"[SUCCESS] Đã commit thành công:\n{res['stdout']}"

    def git_rollback(self) -> str:
        """Hủy bỏ toàn bộ thay đổi chưa commit để khôi phục về trạng thái sạch (git restore .)."""
        res = self.terminal.run_command("git restore . && git clean -fd")
        if not res["success"]:
            return f"[GIT_ERROR] Không thể rollback: {res['stderr']}"
        return "[SUCCESS] Đã khôi phục toàn bộ workspace về trạng thái commit gần nhất."
