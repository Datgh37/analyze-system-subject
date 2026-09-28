"""
Module: core_engine.agents.planner_agent
Mục đích: Đọc Requirements và cấu trúc hiện tại của dự án để lập Implementation Plan chi tiết
Tuân thủ nguyên tắc Claude & Spec: Inspect Project -> Step-by-Step Action Plan
"""

import json
from typing import List, Dict, Any
from core_engine.state import ProjectState
from core_engine.agents.base_agent import BaseAgent


PLANNER_AGENT_SYSTEM_PROMPT = """Bạn là Software System Planner & Architect.
Nhiệm vụ: Đọc yêu cầu chức năng (Requirements) và khảo sát mã nguồn hiện có để lập Kế hoạch Triển khai (Implementation Plan) từng bước rõ ràng.

QUY TẮC BẮT BUỘC:
1. Sử dụng tool list_files và search_code để khảo sát kiến trúc dự án trước khi lập kế hoạch.
2. Bản kế hoạch phải chia thành các bước tuần tự: (1) Kiểm tra file, (2) Thay đổi code, (3) Thêm unit test, (4) Chạy test.
3. Không tự viết code hoàn chỉnh trong bản kế hoạch, chỉ nêu rõ mục tiêu từng bước.
"""


class PlannerAgent(BaseAgent):
    def __init__(self, llm_client, filesystem, terminal, git):
        super().__init__(
            name="PlannerAgent",
            system_prompt=PLANNER_AGENT_SYSTEM_PROMPT,
            llm_client=llm_client,
            filesystem=filesystem,
            terminal=terminal,
            git=git,
        )

    def step(self, state: ProjectState) -> ProjectState:
        """Khảo sát project và tạo danh sách plan_steps."""
        existing_files = self.filesystem.list_files()
        
        messages = [
            {"role": "system", "content": self.system_prompt},
            {
                "role": "user",
                "content": (
                    f"Requirements:\n" + "\n".join(state.requirements) + "\n\n"
                    f"Files hiện có trong project:\n" + "\n".join(existing_files[:30]) + "\n\n"
                    f"Hãy lập danh sách các bước triển khai (Implementation Plan)."
                )
            }
        ]
        
        response = self.llm.call(messages=messages)
        content = response.get("content", "")

        # Trích xuất các dòng bước làm
        plan_steps = [
            line.strip().lstrip("-* ")
            for line in content.splitlines()
            if line.strip() and not line.strip().startswith("#")
        ]

        state.plan_steps = plan_steps if plan_steps else ["1. Triển khai mã nguồn", "2. Chạy kiểm thử"]
        return state
