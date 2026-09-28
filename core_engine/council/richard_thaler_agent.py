"""
Module: core_engine.council.richard_thaler_agent
Agent: 01_richard_thaler (GS. Richard Thaler - Nobel Kinh Tế)
Nhiệm vụ: Đánh giá Tâm lý Chi tiêu, Thuyết Cú Hích (Nudge), Spending Velocity & UX Friction
"""

from typing import Dict, Any, List
from core_engine.council.persona_agent import PersonaAgent
from core_engine.skills.thaler_nudge_skill import ThalerNudgeSkill
from core_engine.state import ProjectState


class RichardThalerAgent(PersonaAgent):
    def __init__(self, llm_client, filesystem, terminal, git):
        super().__init__(
            name="RichardThalerAgent",
            spec_file_path="core/01_richard_thaler.md",
            llm_client=llm_client,
            filesystem=filesystem,
            terminal=terminal,
            git=git,
        )
        self.skill = ThalerNudgeSkill()
        self._register_thaler_skills()

    def _register_thaler_skills(self):
        """Đăng ký các hàm tính toán của Thaler vào tools_map."""
        self.tools_map.update({
            "calculate_spending_velocity": self.skill.calculate_spending_velocity,
            "generate_nudge_recommendation": self.skill.generate_nudge_recommendation,
            "evaluate_ux_friction": self.skill.evaluate_ux_friction,
            "detect_subscription_traps": self.skill.detect_subscription_traps,
        })

    def step(self, state: ProjectState) -> ProjectState:
        """
        Thực hiện đánh giá phản biện dưới góc độ Tâm lý học hành vi & UX Friction.
        """
        messages = [
            {"role": "system", "content": self.system_prompt},
            {
                "role": "user",
                "content": (
                    f"Đề bài tính năng: {state.task_description}\n"
                    f"Hãy đóng vai GS. Richard Thaler: Đánh giá ma sát thao tác người dùng, "
                    f"chỉ ra các rủi ro bỏ app của người lười và đưa ra gợi ý Cú Hích (Nudge)."
                )
            }
        ]

        response = self.llm.call(messages=messages)
        feedback_content = response.get("content", "")

        # Lưu lại feedback đầy đủ vào state
        state.requirements.append(f"[THALER_BEHAVIORAL_INSIGHT]:\n{feedback_content}")
        return state

