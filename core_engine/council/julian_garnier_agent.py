"""
Module: core_engine.council.julian_garnier_agent
Agent: 13_julian_garnier (Julian Garnier & Bret Victor - Motion Choreographer & Interactive Experience Engineer)
Nhiệm vụ: Đặc tả Động lực học Giao diện, Anime.js v4, Gia tốc Lò xo (Spring Physics), FLIP Layout và Vi thẩm mỹ Doppelrand
"""

from typing import Dict, Any, List
from core_engine.council.persona_agent import PersonaAgent
from core_engine.skills.garnier_motion_skill import GarnierMotionSkill
from core_engine.state import ProjectState


class JulianGarnierAgent(PersonaAgent):
    def __init__(self, llm_client, filesystem, terminal, git):
        super().__init__(
            name="JulianGarnierAgent",
            spec_file_path="core/13_julian_garnier.md",
            llm_client=llm_client,
            filesystem=filesystem,
            terminal=terminal,
            git=git,
        )
        self.skill = GarnierMotionSkill()
        self._register_garnier_skills()

    def _register_garnier_skills(self):
        """Đăng ký các hàm tính toán động lực học và vật lý lò xo vào tools_map."""
        self.tools_map.update({
            "calculate_spring_physics": self.skill.calculate_spring_physics,
            "audit_60fps_gpu_performance": self.skill.audit_60fps_gpu_performance,
            "generate_flip_layout_spec": self.skill.generate_flip_layout_spec,
            "generate_doppelrand_haptic_spec": self.skill.generate_doppelrand_haptic_spec,
            "generate_motion_choreography_spec": self.skill.generate_motion_choreography_spec,
            "format_motion_spec_markdown": self.skill.format_motion_spec_markdown,
        })

    def step(self, state: ProjectState) -> ProjectState:
        """
        Thực hiện phân tích và đặc tả động lực học chuyển động và xúc giác giao diện.
        """
        messages = [
            {"role": "system", "content": self.system_prompt},
            {
                "role": "user",
                "content": (
                    f"Đề bài tính năng: {state.task_description}\n"
                    f"Hãy đóng vai Julian Garnier & Bret Victor: Đặc tả kịch bản chuyển động vi mô (Motion Choreography), "
                    f"cân chỉnh thông số vật lý lò xo (Spring Physics), chuyển đổi trạng thái FLIP Layout morphing, "
                    f"áp dụng kiến trúc bọc kép Double-Bezel (Doppelrand) và kiểm toán hiệu năng 60fps GPU."
                )
            }
        ]

        response = self.llm.call(messages=messages)
        motion_content = response.get("content", "")

        state.requirements.append(f"[MOTION_CHOREOGRAPHY_SPEC]:\n{motion_content}")
        return state
