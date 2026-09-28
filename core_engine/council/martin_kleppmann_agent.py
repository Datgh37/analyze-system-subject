"""
Module: core_engine.council.martin_kleppmann_agent
Agent: 05_martin_kleppmann (Martin Kleppmann & Pramod Sadalage - Universal Database Architect)
Nhiệm vụ: Thiết kế CSDL Quan hệ 3NF, NoSQL JSONB/Redis, Concurrency Lock & Polyglot Persistence
"""

from typing import Dict, Any, List
from core_engine.council.persona_agent import PersonaAgent
from core_engine.skills.kleppmann_database_skill import KleppmannDatabaseSkill
from core_engine.state import ProjectState


class MartinKleppmannAgent(PersonaAgent):
    def __init__(self, llm_client, filesystem, terminal, git):
        super().__init__(
            name="MartinKleppmannAgent",
            spec_file_path="core/05_martin_kleppmann.md",
            llm_client=llm_client,
            filesystem=filesystem,
            terminal=terminal,
            git=git,
        )
        self.skill = KleppmannDatabaseSkill()
        self._register_kleppmann_skills()

    def _register_kleppmann_skills(self):
        """Đăng ký các hàm thẩm định CSDL vào tools_map."""
        self.tools_map.update({
            "validate_3nf_schema": self.skill.validate_3nf_schema,
            "evaluate_concurrency_lock_strategy": self.skill.evaluate_concurrency_lock_strategy,
            "format_db_schema_spec": self.skill.format_db_schema_spec,
        })

    def step(self, state: ProjectState) -> ProjectState:
        """
        Tiếp nhận Domain Model và Use Case để thiết kế lược đồ CSDL 3NF, khóa Concurrency và kiến trúc Polyglot.
        """
        requirements_summary = "\n".join(state.requirements)
        messages = [
            {"role": "system", "content": self.system_prompt},
            {
                "role": "user",
                "content": (
                    f"Đề bài tính năng: {state.task_description}\n"
                    f"Đặc tả nghiệp vụ & Kiến trúc Domain:\n{requirements_summary}\n\n"
                    f"Hãy đóng vai Martin Kleppmann & Pramod Sadalage: Thiết kế lược đồ CSDL PostgreSQL 3NF, "
                    f"chỉ rõ kiểu dữ liệu DECIMAL cho tiền tệ, chiến lược khóa Concurrency chống Lost Update "
                    f"và mô hình lưu trữ đa hình (PostgreSQL + JSONB + Redis)."
                )
            }
        ]

        response = self.llm.call(messages=messages)
        db_content = response.get("content", "")

        state.requirements.append(f"[DATABASE_3NF_POLYGLOT_SPEC]:\n{db_content}")
        return state

