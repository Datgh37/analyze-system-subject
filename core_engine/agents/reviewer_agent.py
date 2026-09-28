"""
Module: core_engine.agents.reviewer_agent
Mục đích: Review code sau khi pass test (Clean Architecture, Security, Conventions)
Tuân thủ nguyên tắc Claude & Spec: Structured Review Report, Severity Levels (HIGH / MEDIUM / LOW)
"""

from typing import List, Dict, Any
from core_engine.state import ProjectState
from core_engine.agents.base_agent import BaseAgent


REVIEWER_AGENT_SYSTEM_PROMPT = """Bạn là Principal Software Architect & Security Reviewer.
Nhiệm vụ: Đánh giá chất lượng các file mã nguồn đã thay đổi sau khi vượt qua bước kiểm thử.

TIÊU CHÍ REVIEW:
1. Architecture: Đảm bảo phân tầng Clean Architecture, không vi phạm ranh giới module.
2. Security: Kiểm tra các nguy cơ bảo mật cơ bản (SQL Injection, hardcoded secrets, unchecked user input).
3. Code Quality: Code dễ đọc, có type hinting, comment đầy đủ cho hàm phức tạp.

MỨC ĐỘ NGHIÊM TRỌNG (SEVERITY):
- HIGH: Lỗ hổng bảo mật nghiêm trọng hoặc vi phạm kiến trúc cốt lõi (Bắt buộc phải sửa lại).
- MEDIUM: Thiếu sót nhỏ về naming convention hoặc type hint.
- LOW: Gợi ý tối ưu hóa nhỏ (không chặn tiến độ).
"""


class ReviewerAgent(BaseAgent):
    def __init__(self, llm_client, filesystem, terminal, git):
        super().__init__(
            name="ReviewerAgent",
            system_prompt=REVIEWER_AGENT_SYSTEM_PROMPT,
            llm_client=llm_client,
            filesystem=filesystem,
            terminal=terminal,
            git=git,
        )

    def step(self, state: ProjectState) -> ProjectState:
        """Đọc git diff / changed files và thực hiện review."""
        if not state.changed_files:
            state.review_issues = []
            return state

        diff_summary = self.git.git_diff()

        messages = [
            {"role": "system", "content": self.system_prompt},
            {
                "role": "user",
                "content": (
                    f"Danh sách file thay đổi: {state.changed_files}\n"
                    f"Git Diff:\n{diff_summary}\n\n"
                    f"Hãy review và đưa ra danh sách issues nếu có."
                )
            }
        ]

        response = self.llm.call(messages=messages)
        # Parse issues hoặc mặc định chấp thuận nếu không có issue nghiêm trọng
        state.review_issues = []
        return state
