"""
Module: core_engine.council.werner_vogels_agent
Agent: 07_werner_vogels (Dr. Werner Vogels - CTO Amazon — Async & DevOps Specialist)
Nhiệm vụ: Kiến trúc Bất đồng bộ Message Queue, Chống trùng lặp Idempotency & Docker Containerization
"""

from typing import Dict, Any, List
from core_engine.council.persona_agent import PersonaAgent
from core_engine.skills.vogels_async_skill import VogelsAsyncSkill
from core_engine.state import ProjectState


class WernerVogelsAgent(PersonaAgent):
    def __init__(self, llm_client, filesystem, terminal, git):
        super().__init__(
            name="WernerVogelsAgent",
            spec_file_path="core/07_werner_vogels.md",
            llm_client=llm_client,
            filesystem=filesystem,
            terminal=terminal,
            git=git,
        )
        self.skill = VogelsAsyncSkill()
        self._register_vogels_skills()

    def _register_vogels_skills(self):
        """Đăng ký các hàm kiến trúc bất đồng bộ vào tools_map."""
        self.tools_map.update({
            "validate_idempotency": self.skill.validate_idempotency,
            "simulate_async_job_submission": self.skill.simulate_async_job_submission,
            "format_async_devops_adr": self.skill.format_async_devops_adr,
        })

    def step(self, state: ProjectState) -> ProjectState:
        """
        Tiếp nhận yêu cầu kỹ thuật và thiết kế kiến trúc Message Queue, Idempotency và Docker.
        """
        requirements_summary = "\n".join(state.requirements)
        messages = [
            {"role": "system", "content": self.system_prompt},
            {
                "role": "user",
                "content": (
                    f"Đề bài tính năng: {state.task_description}\n"
                    f"Đặc tả nghiệp vụ từ Hội đồng:\n{requirements_summary}\n\n"
                    f"Hãy đóng vai Dr. Werner Vogels: Thiết kế Kiến trúc Bất đồng bộ Message Queue "
                    f"cho các tác vụ AI nặng (202 Accepted), cơ chế Idempotency chống gửi lặp "
                    f"và đóng gói Docker Compose."
                )
            }
        ]

        response = self.llm.call(messages=messages)
        async_content = response.get("content", "")

        state.requirements.append(f"[ASYNC_DEVOPS_ADR]:\n{async_content}")
        return state

