"""
Module: core_engine.council.bruce_schneier_agent
Agent: 11_bruce_schneier (Bruce Schneier & OWASP - Fintech Security & Cryptography Specialist)
Nhiệm vụ: Thiết kế An ninh Bảo mật Fintech, Mô hình hóa STRIDE, Mã hóa AES-256 GCM & JWT Rotation
"""

from typing import Dict, Any, List
from core_engine.council.persona_agent import PersonaAgent
from core_engine.skills.schneier_security_skill import SchneierSecuritySkill
from core_engine.state import ProjectState


class BruceSchneierAgent(PersonaAgent):
    def __init__(self, llm_client, filesystem, terminal, git):
        super().__init__(
            name="BruceSchneierAgent",
            spec_file_path="core/11_bruce_schneier.md",
            llm_client=llm_client,
            filesystem=filesystem,
            terminal=terminal,
            git=git,
        )
        self.skill = SchneierSecuritySkill()
        self._register_schneier_skills()

    def _register_schneier_skills(self):
        """Đăng ký các hàm an ninh bảo mật và mật mã học vào tools_map."""
        self.tools_map.update({
            "design_stride_threat_model": self.skill.design_stride_threat_model,
            "simulate_aes_encryption_workflow": self.skill.simulate_aes_encryption_workflow,
            "design_jwt_token_lifecycle": self.skill.design_jwt_token_lifecycle,
            "format_security_compliance_adr": self.skill.format_security_compliance_adr,
        })

    def step(self, state: ProjectState) -> ProjectState:
        """
        Thực hiện phân tích mối đe dọa STRIDE và thiết kế cơ chế bảo mật Fintech.
        """
        messages = [
            {"role": "system", "content": self.system_prompt},
            {
                "role": "user",
                "content": (
                    f"Đề bài tính năng: {state.task_description}\n"
                    f"Hãy đóng vai Bruce Schneier & OWASP: Phân tích ma trận mối đe dọa STRIDE 6 chiều, "
                    f"thiết kế cơ chế mã hóa AES-256-GCM cho dữ liệu tài chính nhạy cảm "
                    f"và chính sách JWT Token Rotation kèm RBAC."
                )
            }
        ]

        response = self.llm.call(messages=messages)
        security_content = response.get("content", "")

        state.requirements.append(f"[FINTECH_SECURITY_STRIDE_ADR]:\n{security_content}")
        return state
