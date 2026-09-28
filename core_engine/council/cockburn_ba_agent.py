"""
Module: core_engine.council.cockburn_ba_agent
Agent: 03_cockburn_ba (Alistair Cockburn & Debra Paul - Lead BA Specialist)
Nhiệm vụ: Chuẩn hóa Use Case Specification IEEE 29148 và Gherkin BDD Acceptance Criteria
"""

from typing import Dict, Any, List
from core_engine.council.persona_agent import PersonaAgent
from core_engine.skills.cockburn_use_case_skill import CockburnUseCaseSkill
from core_engine.state import ProjectState


class CockburnBAAgent(PersonaAgent):
    def __init__(self, llm_client, filesystem, terminal, git):
        super().__init__(
            name="CockburnBAAgent",
            spec_file_path="core/03_cockburn_ba.md",
            llm_client=llm_client,
            filesystem=filesystem,
            terminal=terminal,
            git=git,
        )
        self.skill = CockburnUseCaseSkill()
        self._register_ba_skills()

    def _register_ba_skills(self):
        """Đăng ký các hàm phân tích nghiệp vụ vào tools_map."""
        self.tools_map.update({
            "format_ieee_use_case_spec": self.skill.format_ieee_use_case_spec,
            "generate_gherkin_scenarios": self.skill.generate_gherkin_scenarios,
            "validate_use_case_completeness": self.skill.validate_use_case_completeness,
        })

    def step(self, state: ProjectState) -> ProjectState:
        """
        Tổng hợp phản biện của Khách hàng/Gia đình và chuẩn hóa thành Use Case Spec IEEE 29148.
        """
        requirements_summary = "\n".join(state.requirements)
        messages = [
            {"role": "system", "content": self.system_prompt},
            {
                "role": "user",
                "content": (
                    f"Đề bài tính năng: {state.task_description}\n"
                    f"Tổng hợp các phản biện và nhu cầu trước đó:\n{requirements_summary}\n\n"
                    f"Hãy đóng vai Alistair Cockburn: Chuẩn hóa thành Đặc tả Use Case IEEE 29148, "
                    f"nêu rõ Luồng chính, Luồng ngoại lệ và Tiêu chí nghiệm thu Gherkin BDD."
                )
            }
        ]

        response = self.llm.call(messages=messages)
        spec_content = response.get("content", "")

        state.requirements.append(f"[IEEE_29148_USE_CASE_SPEC]:\n{spec_content}")
        return state

