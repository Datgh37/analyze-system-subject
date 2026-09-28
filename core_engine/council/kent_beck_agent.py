"""
Module: core_engine.council.kent_beck_agent
Agent: 12_kent_beck (Kent Beck & Martin Fowler - Full-Stack Test Strategy & QA Specialist)
Nhiệm vụ: Thiết kế Kim tự tháp kiểm thử Test Pyramid, Playwright E2E Automation & k6 Performance QA
"""

from typing import Dict, Any, List
from core_engine.council.persona_agent import PersonaAgent
from core_engine.skills.beck_qa_strategy_skill import BeckQAStrategySkill
from core_engine.state import ProjectState


class KentBeckAgent(PersonaAgent):
    def __init__(self, llm_client, filesystem, terminal, git):
        super().__init__(
            name="KentBeckAgent",
            spec_file_path="core/12_kent_beck.md",
            llm_client=llm_client,
            filesystem=filesystem,
            terminal=terminal,
            git=git,
        )
        self.skill = BeckQAStrategySkill()
        self._register_beck_skills()

    def _register_beck_skills(self):
        """Đăng ký các hàm chiến lược kiểm thử và QA vào tools_map."""
        self.tools_map.update({
            "calculate_test_pyramid_distribution": self.skill.calculate_test_pyramid_distribution,
            "generate_e2e_playwright_scenario": self.skill.generate_e2e_playwright_scenario,
            "design_load_test_profile": self.skill.design_load_test_profile,
            "format_qa_verification_plan": self.skill.format_qa_verification_plan,
        })

    def step(self, state: ProjectState) -> ProjectState:
        """
        Thực hiện thiết kế chiến lược kiểm thử tự động toàn diện và kế hoạch QA.
        """
        messages = [
            {"role": "system", "content": self.system_prompt},
            {
                "role": "user",
                "content": (
                    f"Đề bài tính năng: {state.task_description}\n"
                    f"Hãy đóng vai Kent Beck & Martin Fowler: Thiết lập Kim tự tháp kiểm thử (70% Unit, 20% Integration, 10% E2E), "
                    f"sinh kịch bản kiểm thử E2E Playwright và hồ sơ kiểm thử tải k6."
                )
            }
        ]

        response = self.llm.call(messages=messages)
        qa_content = response.get("content", "")

        state.requirements.append(f"[FULL_STACK_QA_TEST_STRATEGY_SPEC]:\n{qa_content}")
        return state
