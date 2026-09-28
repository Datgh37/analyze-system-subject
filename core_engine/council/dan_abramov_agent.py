"""
Module: core_engine.council.dan_abramov_agent
Agent: 10_dan_abramov (Dan Abramov & Christopher Chedeau - Cross-Platform & Client State Specialist)
Nhiệm vụ: Thiết kế Kiến trúc Client-Side State, Optimistic UI 0ms & Offline-First Sync Queue
"""

from typing import Dict, Any, List
from core_engine.council.persona_agent import PersonaAgent
from core_engine.skills.abramov_client_state_skill import AbramovClientStateSkill
from core_engine.state import ProjectState


class DanAbramovAgent(PersonaAgent):
    def __init__(self, llm_client, filesystem, terminal, git):
        super().__init__(
            name="DanAbramovAgent",
            spec_file_path="core/10_dan_abramov.md",
            llm_client=llm_client,
            filesystem=filesystem,
            terminal=terminal,
            git=git,
        )
        self.skill = AbramovClientStateSkill()
        self._register_abramov_skills()

    def _register_abramov_skills(self):
        """Đăng ký các hàm kiến trúc Client State và Optimistic UI vào tools_map."""
        self.tools_map.update({
            "design_client_store_schema": self.skill.design_client_store_schema,
            "simulate_optimistic_ui_action": self.skill.simulate_optimistic_ui_action,
            "design_offline_sync_queue": self.skill.design_offline_sync_queue,
            "format_frontend_architecture_adr": self.skill.format_frontend_architecture_adr,
        })

    def step(self, state: ProjectState) -> ProjectState:
        """
        Thực hiện phân tích và thiết kế kiến trúc Client-Side State Management và Optimistic UI.
        """
        messages = [
            {"role": "system", "content": self.system_prompt},
            {
                "role": "user",
                "content": (
                    f"Đề bài tính năng: {state.task_description}\n"
                    f"Hãy đóng vai Dan Abramov & Christopher Chedeau (Vjeux): Thiết kế lược đồ Client State Store, "
                    f"mô phỏng cơ chế cập nhật tức thì Optimistic UI 0ms (kèm rollback khi lỗi) "
                    f"và chiến lược đồng bộ ngoại tuyến Offline-First Sync Queue."
                )
            }
        ]

        response = self.llm.call(messages=messages)
        client_content = response.get("content", "")

        state.requirements.append(f"[CLIENT_STATE_OPTIMISTIC_UI_ADR]:\n{client_content}")
        return state
