"""
Module: core_engine.agents.base_agent
Mục đích: Lớp trừu tượng BaseAgent cho mọi Agent trong hệ thống
Tuân thủ nguyên tắc Claude: Strict System Prompt, Tool Execution Safety, High-Signal Tracing
"""

from typing import Dict, Any, List, Optional, Callable
from core_engine.state import ProjectState
from core_engine.llm_client import BaseLLMClient
from core_engine.tools.filesystem import FilesystemTools
from core_engine.tools.terminal import TerminalTools
from core_engine.tools.git_tools import GitTools


class BaseAgent:
    def __init__(
        self,
        name: str,
        system_prompt: str,
        llm_client: BaseLLMClient,
        filesystem: FilesystemTools,
        terminal: TerminalTools,
        git: GitTools,
    ):
        self.name = name
        self.system_prompt = system_prompt
        self.llm = llm_client
        self.filesystem = filesystem
        self.terminal = terminal
        self.git = git
        self.tools_map: Dict[str, Callable] = {}
        self.tool_schemas: List[Dict[str, Any]] = []
        self._register_default_tools()

    def _register_default_tools(self):
        """Đăng ký các tool cơ bản có sẵn."""
        self.tools_map = {
            "list_files": self.filesystem.list_files,
            "read_file": self.filesystem.read_file,
            "search_code": self.filesystem.search_code,
            "write_file": self.filesystem.write_file,
            "edit_file": self.filesystem.edit_file,
            "run_command": self.terminal.run_command,
            "run_tests": self.terminal.run_tests,
            "run_pytest": self.terminal.run_pytest,
            "git_status": self.git.git_status,

            "git_diff": self.git.git_diff,
        }

    def execute_tool(self, tool_name: str, args: Dict[str, Any], state: ProjectState) -> Any:
        """Thực thi một tool và ghi nhận vào lịch sử của ProjectState."""
        if tool_name not in self.tools_map:
            error_msg = f"[ERROR] Tool '{tool_name}' không tồn tại hoặc không được cấp quyền."
            state.record_tool_call(tool_name, args, error_msg)
            return error_msg

        try:
            func = self.tools_map[tool_name]
            result = func(**args)
            state.record_tool_call(tool_name, args, result)
            return result
        except Exception as e:
            error_msg = f"[TOOL_EXCEPTION] Lỗi khi thực thi '{tool_name}': {str(e)}"
            state.record_tool_call(tool_name, args, error_msg)
            return error_msg

    def step(self, state: ProjectState) -> ProjectState:
        """Thực hiện 1 bước phân tích/hành động của Agent trên ProjectState."""
        raise NotImplementedError
