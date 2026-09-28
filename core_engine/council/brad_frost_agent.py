"""
Module: core_engine.council.brad_frost_agent
Agent: 09_brad_frost (Brad Frost & Don Norman - Atomic Design & UI/UX Specialist)
Nhiệm vụ: Thiết kế Hệ thống Giao diện Atomic Design, Wireframe Layouts & Kiểm toán Công thái học WCAG
"""

from typing import Dict, Any, List
from core_engine.council.persona_agent import PersonaAgent
from core_engine.skills.frost_atomic_design_skill import FrostAtomicDesignSkill
from core_engine.state import ProjectState


class BradFrostAgent(PersonaAgent):
    def __init__(self, llm_client, filesystem, terminal, git):
        super().__init__(
            name="BradFrostAgent",
            spec_file_path="core/09_brad_frost.md",
            llm_client=llm_client,
            filesystem=filesystem,
            terminal=terminal,
            git=git,
        )
        self.skill = FrostAtomicDesignSkill()
        self._register_frost_skills()

    def _register_frost_skills(self):
        """Đăng ký các hàm thiết kế UI/UX và Atomic Design vào tools_map."""
        self.tools_map.update({
            "generate_design_tokens": self.skill.generate_design_tokens,
            "generate_atomic_component_tree": self.skill.generate_atomic_component_tree,
            "evaluate_wcag_accessibility": self.skill.evaluate_wcag_accessibility,
            "generate_wireframe_spec": self.skill.generate_wireframe_spec,
            "format_design_system_adr": self.skill.format_design_system_adr,
        })

    def step(self, state: ProjectState) -> ProjectState:
        """
        Thực hiện phân tích và thiết kế hệ thống giao diện Atomic Design System.
        """
        messages = [
            {"role": "system", "content": self.system_prompt},
            {
                "role": "user",
                "content": (
                    f"Đề bài tính năng: {state.task_description}\n"
                    f"Hãy đóng vai Brad Frost & Don Norman: Xây dựng bảng Design Tokens, "
                    f"phân rã cây linh kiện Atomic Components (Atoms -> Molecules -> Organisms), "
                    f"đặc tả khung dây Wireframe và kiểm toán độ tương phản WCAG 2.1 AA."
                )
            }
        ]

        response = self.llm.call(messages=messages)
        ui_content = response.get("content", "")

        state.requirements.append(f"[ATOMIC_DESIGN_UI_SPEC]:\n{ui_content}")
        return state
