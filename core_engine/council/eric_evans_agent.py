"""
Module: core_engine.council.eric_evans_agent
Agent: 04_eric_evans (Eric Evans - Cha đẻ Domain-Driven Design & Clean Architecture)
Nhiệm vụ: Mô hình hóa Miền Nghiệp vụ Tài chính, Bounded Contexts, Money Value Object & Clean Architecture ADR
"""

from typing import Dict, Any, List
from core_engine.council.persona_agent import PersonaAgent
from core_engine.skills.evans_domain_skill import EvansDomainSkill
from core_engine.state import ProjectState


class EricEvansAgent(PersonaAgent):
    def __init__(self, llm_client, filesystem, terminal, git):
        super().__init__(
            name="EricEvansAgent",
            spec_file_path="core/04_eric_evans.md",
            llm_client=llm_client,
            filesystem=filesystem,
            terminal=terminal,
            git=git,
        )
        self.skill = EvansDomainSkill()
        self._register_evans_skills()

    def _register_evans_skills(self):
        """Đăng ký các hàm thẩm định kiến trúc Domain vào tools_map."""
        self.tools_map.update({
            "create_money": self.skill.create_money,
            "validate_bounded_contexts": self.skill.validate_bounded_contexts,
            "format_clean_architecture_adr": self.skill.format_clean_architecture_adr,
        })

    def step(self, state: ProjectState) -> ProjectState:
        """
        Tiếp nhận Use Case nghiệp vụ và chuyển đổi thành bản Quyết định Kiến trúc Clean Architecture ADR.
        """
        requirements_summary = "\n".join(state.requirements)
        messages = [
            {"role": "system", "content": self.system_prompt},
            {
                "role": "user",
                "content": (
                    f"Đề bài tính năng: {state.task_description}\n"
                    f"Đặc tả nghiệp vụ từ BA:\n{requirements_summary}\n\n"
                    f"Hãy đóng vai Eric Evans: Thiết kế Kiến trúc Clean Architecture, "
                    f"phân định Bounded Contexts, chỉ rõ Aggregates và xuất Quyết định Kiến trúc (ADR)."
                )
            }
        ]

        response = self.llm.call(messages=messages)
        adr_content = response.get("content", "")

        state.requirements.append(f"[CLEAN_ARCHITECTURE_ADR]:\n{adr_content}")
        return state

