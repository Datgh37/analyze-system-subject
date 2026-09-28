"""
Module: core_engine.council.andrew_ng_agent
Agent: 08_andrew_ng (Prof. Andrew Ng - AI Pioneer — Multimodal AI Specialist)
Nhiệm vụ: Pipeline Bóc tách Hóa đơn Multimodal Vision AI, Data-Centric AI & Mathematical Audit
"""

from typing import Dict, Any, List
from core_engine.council.persona_agent import PersonaAgent
from core_engine.skills.andrew_ng_ai_skill import AndrewNgAISkill
from core_engine.state import ProjectState


class AndrewNgAgent(PersonaAgent):
    def __init__(self, llm_client, filesystem, terminal, git):
        super().__init__(
            name="AndrewNgAgent",
            spec_file_path="core/08_andrew_ng.md",
            llm_client=llm_client,
            filesystem=filesystem,
            terminal=terminal,
            git=git,
        )
        self.skill = AndrewNgAISkill()
        self._register_ai_skills()

    def _register_ai_skills(self):
        """Đăng ký các hàm kiểm toán AI vào tools_map."""
        self.tools_map.update({
            "validate_receipt_math_audit": self.skill.validate_receipt_math_audit,
            "generate_pydantic_receipt_schema": self.skill.generate_pydantic_receipt_schema,
            "format_ai_pipeline_adr": self.skill.format_ai_pipeline_adr,
        })

    def step(self, state: ProjectState) -> ProjectState:
        """
        Tiếp nhận yêu cầu kỹ thuật và thiết kế Pipeline Multimodal AI, Hậu kiểm toán học và cơ chế Human-in-the-Loop.
        """
        requirements_summary = "\n".join(state.requirements)
        messages = [
            {"role": "system", "content": self.system_prompt},
            {
                "role": "user",
                "content": (
                    f"Đề bài tính năng: {state.task_description}\n"
                    f"Đặc tả nghiệp vụ từ Hội đồng:\n{requirements_summary}\n\n"
                    f"Hãy đóng vai Prof. Andrew Ng: Thiết kế Pipeline bóc tách hóa đơn Multimodal AI 3 giai đoạn, "
                    f"ràng buộc JSON Schema và thuật toán Hậu kiểm toán học (Mathematical Audit)."
                )
            }
        ]

        response = self.llm.call(messages=messages)
        ai_content = response.get("content", "")

        state.requirements.append(f"[MULTIMODAL_AI_PIPELINE_ADR]:\n{ai_content}")
        return state

