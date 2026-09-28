"""
Module: core_engine.tools.terminal
Mục đích: Tầng thực thi command và chạy test an toàn (Safe Terminal & Test Runner Tools)
Tuân thủ nguyên tắc Claude: Safe Sandboxing, Timeout Protection, High-Signal Error Extraction
"""

import os
import sys
import subprocess
import time
from pathlib import Path
from typing import Dict, Any, Optional, List


class TerminalTools:
    # Danh sách các pattern/lệnh nguy hiểm bị cấm thực thi
    DANGEROUS_PATTERNS = [
        "format ",
        "diskpart",
        "rmdir /s /q c:\\",
        "rmdir /s /q \\",
        "del /f /s /q c:\\",
        "rm -rf /",
        ":(){ :|:& };:", # Fork bomb
        "shutdown",
        "reg delete",
        "drop database",
    ]

    def __init__(self, workspace_root: Optional[str] = None):
        if workspace_root is None:
            workspace_root = os.getcwd()
        self.workspace_root = Path(workspace_root).resolve()

    def _is_safe_command(self, command: str) -> bool:
        """Kiểm tra lệnh có chứa các mẫu mã độc hại hoặc nguy hiểm không."""
        cmd_lower = command.lower().strip()
        for dangerous in self.DANGEROUS_PATTERNS:
            if dangerous in cmd_lower:
                return False
        return True

    def run_command(
        self,
        command: str,
        cwd: Optional[str] = None,
        timeout_seconds: int = 45
    ) -> Dict[str, Any]:
        """
        Thực thi một câu lệnh shell trong workspace an toàn kèm giới hạn timeout.
        Trả về kết quả có cấu trúc: stdout, stderr, exit_code, success, duration.
        """
        if not self._is_safe_command(command):
            return {
                "success": False,
                "exit_code": -1,
                "stdout": "",
                "stderr": f"[SECURITY_ALERT] Câu lệnh bị chặn do chứa pattern nguy hiểm: '{command}'",
                "duration_seconds": 0.0
            }

        target_cwd = (self.workspace_root / cwd).resolve() if cwd else self.workspace_root
        if not target_cwd.exists() or not target_cwd.is_dir():
            target_cwd = self.workspace_root

        start_time = time.time()
        try:
            process = subprocess.run(
                command,
                cwd=str(target_cwd),
                stdout=subprocess.PIPE,
                stderr=subprocess.PIPE,
                text=True,
                shell=True,
                timeout=timeout_seconds,
                encoding="utf-8",
                errors="replace"
            )

            duration = round(time.time() - start_time, 2)
            return {
                "success": process.returncode == 0,
                "exit_code": process.returncode,
                "stdout": process.stdout.strip(),
                "stderr": process.stderr.strip(),
                "duration_seconds": duration
            }


        except subprocess.TimeoutExpired:
            duration = round(time.time() - start_time, 2)
            return {
                "success": False,
                "exit_code": 124,
                "stdout": "",
                "stderr": f"[TIMEOUT_ERROR] Câu lệnh vượt quá thời gian tối đa cho phép ({timeout_seconds}s).",
                "duration_seconds": duration
            }
        except Exception as e:
            duration = round(time.time() - start_time, 2)
            return {
                "success": False,
                "exit_code": -1,
                "stdout": "",
                "stderr": f"[EXECUTION_ERROR] {str(e)}",
                "duration_seconds": duration
            }

    def run_tests(
        self,
        test_target: str = "tests",
        cwd: Optional[str] = None,
        extra_args: Optional[List[str]] = None,
        timeout_seconds: int = 60
    ) -> Dict[str, Any]:
        """
        Chạy bộ kiểm thử (hỗ trợ cả unittest chuẩn và pytest nếu có).
        Trích xuất kết quả kiểm thử có cấu trúc (Pass/Fail count, Error trace).
        """
        target_path = Path(test_target)
        if target_path.suffix == ".py":
            # Nếu là file cụ thể, dùng discover với thư mục cha và pattern tên file (kèm cờ -B chống stale .pyc)
            search_dir = target_path.parent.as_posix() if target_path.parent.as_posix() != "." else "."
            pattern = target_path.name
            cmd = f"python -B -m unittest discover -s {search_dir} -p {pattern}"
        else:
            # Nếu là thư mục
            search_dir = target_path.as_posix()
            cmd = f"python -B -m unittest discover -s {search_dir}"

        result = self.run_command(cmd, cwd=cwd, timeout_seconds=timeout_seconds)




        # Phân tích kết quả đầu ra
        stdout = result["stdout"]
        stderr = result["stderr"]
        is_passed = (result["exit_code"] == 0)

        raw_output = stdout if stdout else stderr
        if stderr and stdout:
            raw_output = f"{stdout}\n{stderr}"

        return {
            "status": "PASS" if is_passed else "FAIL",
            "exit_code": result["exit_code"],
            "raw_output": raw_output,
            "duration_seconds": result["duration_seconds"],
            "is_passed": is_passed
        }

    def run_pytest(
        self,
        test_target: str = "tests",
        extra_args: Optional[List[str]] = None,
        timeout_seconds: int = 60
    ) -> Dict[str, Any]:
        """Alias cho run_tests."""
        return self.run_tests(test_target=test_target, extra_args=extra_args, timeout_seconds=timeout_seconds)

