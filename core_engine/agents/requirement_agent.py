"""
Module: core_engine.agents.requirement_agent
Mục đích: Phân tích yêu cầu tự nhiên, cấu trúc hóa thành Functional Requirements (FR) và Use Case IEEE 29148
Tuân thủ nguyên tắc Claude & Spec: Không tự ý viết code, chỉ tập trung vào phân tích phạm vi và tiêu chí
"""

from typing import List, Dict, Any
from core_engine.state import ProjectState
from core_engine.agents.base_agent import BaseAgent


REQUIREMENT_AGENT_SYSTEM_PROMPT = """Bạn là Lead Business Analyst & Requirement Engineer.
Nhiệm vụ: Phân tích yêu cầu tự nhiên của người dùng và chuyển hóa thành danh sách Functional Requirements (FR) có cấu trúc chuẩn mực.

QUY TẮC BẮT BUỘC:
1. Bạn KHÔNG trực tiếp viết mã nguồn hay chỉnh sửa source code.
2. Mỗi yêu cầu phải có mã định danh (FR-001, FR-002, ...), mô tả rõ ràng, đo lường được.
3. Phân biệt rõ ràng giữa Yêu cầu Chức năng (Functional) và Phi chức năng (Non-Functional).
"""


class RequirementAgent(BaseAgent):
    def __init__(self, llm_client, filesystem, terminal, git):
        super().__init__(
            name="RequirementAgent",
            system_prompt=REQUIREMENT_AGENT_SYSTEM_PROMPT,
            llm_client=llm_client,
            filesystem=filesystem,
            terminal=terminal,
            git=git,
        )

    def step(self, state: ProjectState) -> ProjectState:
        """Phân tích task_description và trích xuất danh sách requirements."""
        messages = [
            {"role": "system", "content": self.system_prompt},
            {
                "role": "user",
                "content": f"Phân tích yêu cầu sau và trích xuất danh sách FR có cấu trúc:\n{state.task_description}"
            }
        ]
        response = self.llm.call(messages=messages)
        content = response.get("content", "")

        # Trích xuất các dòng bắt đầu bằng FR-
        extracted_frs = []
        for line in content.splitlines():
            line_str = line.strip()
            if line_str.startswith("FR-") or line_str.startswith("- FR-") or "FR-" in line_str:
                extracted_frs.append(line_str)

        if not extracted_frs:
            # Fallback nếu LLM trả dạng danh sách bullet points
            extracted_frs = [f"FR-{i+1:03d}: {line.lstrip('-* ')}" for i, line in enumerate(content.splitlines()) if line.strip()]

        state.requirements = extracted_frs
        return state
