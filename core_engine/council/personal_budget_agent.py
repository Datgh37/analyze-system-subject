"""
Module: core_engine.council.personal_budget_agent
Agent: 02_personal_budget_coach / 02_group_leader
Nhiệm vụ: Phản biện và mô phỏng Hoạch định Ngân sách Cá nhân & Quản trị Tài chính
"""

from typing import Dict, Any, List
from pathlib import Path
from core_engine.council.persona_agent import PersonaAgent
from core_engine.skills.personal_budget_cases_skill import PersonalBudgetCasesSkill
from core_engine.state import ProjectState


class PersonalBudgetAgent(PersonaAgent):
    def __init__(self, llm_client, filesystem, terminal, git):
        # Kiểm tra file spec khả dụng: ưu tiên 02_personal_budget_coach.md, fallback 02_group_leader.md
        spec_path = "core/02_personal_budget_coach.md"
        if filesystem and hasattr(filesystem, "workspace_root"):
            p1 = (filesystem.workspace_root / spec_path).resolve()
            p2 = (filesystem.workspace_root / "core/02_group_leader.md").resolve()
            if not p1.exists() and p2.exists():
                spec_path = "core/02_group_leader.md"

        super().__init__(
            name="PersonalBudgetAgent",
            spec_file_path=spec_path,
            llm_client=llm_client,
            filesystem=filesystem,
            terminal=terminal,
            git=git,
            default_prompt="Bạn là Chuyên Gia Hoạch Định Ngân Sách Cá Nhân & Quản Trị Tài Chính trong Hội đồng chuyên gia."
        )
        self.skill = PersonalBudgetCasesSkill()
        self._register_budget_skills()

    def _register_budget_skills(self):
        """Đăng ký các hàm tính toán tài chính vào tools_map."""
        self.tools_map.update({
            "calculate_50_30_20_budget": self.skill.calculate_50_30_20_budget,
            "calculate_zero_based_budget": self.skill.calculate_zero_based_budget,
            "calculate_kakeibo_breakdown": self.skill.calculate_kakeibo_breakdown,
            "calculate_daily_spending_allowance": self.skill.calculate_daily_spending_allowance,
            "simulate_household_fund": self.skill.simulate_household_fund,
            "process_reimbursement": self.skill.process_reimbursement,
            "calculate_roommate_utilities": self.skill.calculate_roommate_utilities,
            "calculate_itemized_and_weighted_split": self.skill.calculate_itemized_and_weighted_split,
        })

    def step(self, state: ProjectState) -> ProjectState:
        messages = [
            {"role": "system", "content": self.system_prompt},
            {
                "role": "user",
                "content": f"Đề bài: {state.task_description}\nPhản biện tài chính ngân sách cá nhân và quản trị chi tiêu."
            }
        ]
        response = self.llm.call(messages=messages)
        feedback = response.get("content", "")

        state.requirements.append(f"[PERSONAL_BUDGET_INSIGHT]:\n[HOUSEHOLD_GROUP_FINANCE_INSIGHT]:\n{feedback}")
        return state


GroupLeaderAgent = PersonalBudgetAgent
