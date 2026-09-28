"""
Module: core_engine.tools.filesystem
Mục đích: Tầng công cụ tương tác file an toàn cho AI Agent (Safe Filesystem Tools)
Tuân thủ nguyên tắc Claude: Least Privilege, Path Sandboxing, High-Signal Feedback
"""

import os
import re
from pathlib import Path
from typing import List, Dict, Any, Optional


class SecuritySandboxError(Exception):
    """Lỗi khi Agent cố tình truy cập hoặc ghi đè ra ngoài workspace được phép."""
    pass


class FilesystemTools:
    def __init__(self, workspace_root: Optional[str] = None):
        """
        Khởi tạo tool với một thư mục workspace gốc an toàn.
        Mặc định là thư mục hiện tại của dự án.
        """
        if workspace_root is None:
            workspace_root = os.getcwd()
        self.workspace_root = Path(workspace_root).resolve()

    def _resolve_safe_path(self, path_str: str) -> Path:
        """
        Chuyển đổi đường dẫn và kiểm tra xem có nằm trong workspace_root không.
        Ngăn chặn tấn công Path Traversal (vd: '../../Windows/System32').
        """
        target_path = (self.workspace_root / path_str).resolve()
        try:
            target_path.relative_to(self.workspace_root)
        except ValueError:
            raise SecuritySandboxError(
                f"[VIOLATION] Đường dẫn '{path_str}' nằm ngoài thư mục workspace an toàn: '{self.workspace_root}'"
            )
        return target_path

    def list_files(self, dir_path: str = ".", recursive: bool = True, max_files: int = 200) -> List[str]:
        """
        Liệt kê danh sách các tệp trong thư mục.
        Bỏ qua các thư mục tạm thời (.git, __pycache__, .venv, node_modules).
        """
        safe_dir = self._resolve_safe_path(dir_path)
        if not safe_dir.exists() or not safe_dir.is_dir():
            return []

        ignored_patterns = {".git", "__pycache__", ".venv", "venv", "node_modules", ".pytest_cache", ".idea"}
        results = []

        if recursive:
            for root, dirs, files in os.walk(safe_dir):
                # Lọc các thư mục bị bỏ qua ngay tại chỗ để tránh duyệt sâu
                dirs[:] = [d for d in dirs if d not in ignored_patterns and not d.startswith(".")]
                for f in files:
                    full_path = Path(root) / f
                    rel_path = full_path.relative_to(self.workspace_root).as_posix()
                    results.append(rel_path)
                    if len(results) >= max_files:
                        return results
        else:
            for entry in safe_dir.iterdir():
                if entry.name not in ignored_patterns:
                    rel_path = entry.relative_to(self.workspace_root).as_posix()
                    results.append(rel_path)
                    if len(results) >= max_files:
                        return results

        return sorted(results)

    def read_file(self, path: str, start_line: Optional[int] = None, end_line: Optional[int] = None) -> str:
        """
        Đọc nội dung của một tệp kèm số dòng.
        Hỗ trợ đọc một phần (start_line đến end_line, 1-indexed) để tiết kiệm token.
        """
        safe_path = self._resolve_safe_path(path)
        if not safe_path.exists():
            return f"[ERROR] File '{path}' không tồn tại."
        if safe_path.is_dir():
            return f"[ERROR] '{path}' là một thư mục, không phải là tệp văn bản."

        try:
            with open(safe_path, "r", encoding="utf-8", errors="replace") as f:
                lines = f.readlines()
        except Exception as e:
            return f"[ERROR] Không thể đọc file '{path}': {str(e)}"

        total_lines = len(lines)
        start = max(1, start_line) if start_line else 1
        end = min(total_lines, end_line) if end_line else total_lines

        if start > total_lines:
            return f"[INFO] File '{path}' chỉ có {total_lines} dòng. StartLine {start} vượt quá độ dài."

        output_lines = []
        for i in range(start - 1, end):
            output_lines.append(f"{i + 1:4d}: {lines[i].rstrip()}")

        header = f"--- File: {path} (Lines {start}-{end} of {total_lines}) ---\n"
        return header + "\n".join(output_lines)

    def search_code(self, query: str, dir_path: str = ".", file_extension: Optional[str] = None) -> List[Dict[str, Any]]:
        """
        Tìm kiếm chuỗi hoặc pattern trong các file code (tương tự ripgrep / grep).
        """
        safe_dir = self._resolve_safe_path(dir_path)
        matches = []
        files = self.list_files(dir_path, recursive=True)

        for rel_file in files:
            if file_extension and not rel_file.endswith(file_extension):
                continue
            
            full_path = self.workspace_root / rel_file
            try:
                with open(full_path, "r", encoding="utf-8", errors="replace") as f:
                    for line_num, line in enumerate(f, 1):
                        if query.lower() in line.lower():
                            matches.append({
                                "file": rel_file,
                                "line_number": line_num,
                                "line_content": line.strip()
                            })
                            if len(matches) >= 50:
                                return matches
            except Exception:
                continue

        return matches

    def write_file(self, path: str, content: str, overwrite: bool = True) -> str:
        """
        Tạo mới hoặc ghi đè một tệp. Tự động tạo thư mục cha nếu chưa tồn tại.
        """
        safe_path = self._resolve_safe_path(path)
        if safe_path.exists() and not overwrite:
            return f"[ERROR] File '{path}' đã tồn tại và overwrite=False."

        try:
            safe_path.parent.mkdir(parents=True, exist_ok=True)
            with open(safe_path, "w", encoding="utf-8") as f:
                f.write(content)
            return f"[SUCCESS] Đã ghi thành công vào file '{path}' ({len(content)} ký tự)."
        except Exception as e:
            return f"[ERROR] Lỗi khi ghi file '{path}': {str(e)}"

    def edit_file(self, path: str, target: str, replacement: str) -> str:
        """
        Thay thế một đoạn văn bản cụ thể trong file (Target -> Replacement).
        Bắt buộc target phải khớp chính xác để đảm bảo an toàn.
        """
        safe_path = self._resolve_safe_path(path)
        if not safe_path.exists():
            return f"[ERROR] File '{path}' không tồn tại."

        try:
            with open(safe_path, "r", encoding="utf-8", errors="replace") as f:
                content = f.read()

            occurrences = content.count(target)
            if occurrences == 0:
                return f"[ERROR] Không tìm thấy đoạn text 'target' cần thay thế trong file '{path}'."
            if occurrences > 1:
                return f"[ERROR] Đoạn text 'target' xuất hiện {occurrences} lần trong file '{path}'. Vui lòng cung cấp thêm ngữ cảnh xung quanh để xác định duy nhất."

            new_content = content.replace(target, replacement, 1)
            with open(safe_path, "w", encoding="utf-8") as f:
                f.write(new_content)

            return f"[SUCCESS] Đã thay thế thành công nội dung trong file '{path}'."
        except Exception as e:
            return f"[ERROR] Lỗi khi chỉnh sửa file '{path}': {str(e)}"
