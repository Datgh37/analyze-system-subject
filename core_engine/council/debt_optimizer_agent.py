"""
Module: core_engine.council.debt_optimizer_agent
Agent: 06_personal_debt_cashflow_optimizer / 06_tarjan_graph
Nhiệm vụ: Tối ưu hóa Chiến lược Trả nợ Cá nhân & Đồ thị Dòng tiền
"""

from typing import Dict, Any, List
from pathlib import Path
from core_engine.council.persona_agent import PersonaAgent
from core_engine.skills.debt_cashflow_optimizer_skill import DebtCashflowOptimizerSkill
from core_engine.state import ProjectState


class DebtOptimizerAgent(PersonaAgent):
    def __init__(self, llm_client, filesystem, terminal, git):
        spec_path = "core/06_personal_debt_cashflow_optimizer.md"
        if filesystem and hasattr(filesystem, "workspace_root"):
            p1 = (filesystem.workspace_root / spec_path).resolve()
            p2 = (filesystem.workspace_root / "core/06_tarjan_graph.md").resolve()
            if not p1.exists() and p2.exists():
                spec_path = "core/06_tarjan_graph.md"

        super().__init__(
            name="DebtOptimizerAgent",
            spec_file_path=spec_path,
            llm_client=llm_client,
            filesystem=filesystem,
            terminal=terminal,
            git=git,
            default_prompt="Bạn là Robert Tarjan - Chuyên Gia Tối Ưu Hóa Nợ & Đồ Thị Dòng Tiền trong Hội đồng chuyên gia."
        )
        self.skill = DebtCashflowOptimizerSkill()
        self._register_debt_skills()

    def _register_debt_skills(self):
        self.tools_map.update({
            "calculate_snowball_schedule": self.skill.calculate_snowball_schedule,
            "calculate_avalanche_schedule": self.skill.calculate_avalanche_schedule,
            "compare_payoff_strategies": self.skill.compare_payoff_strategies,
            "simplify_debts": self.skill.simplify_debts,
            "eliminate_debt_cycles": self.skill.eliminate_debt_cycles,
            "generate_mermaid_graph": self.skill.generate_mermaid_graph,
        })

    def step(self, state: ProjectState) -> ProjectState:
        messages = [
            {"role": "system", "content": self.system_prompt},
            {
                "role": "user",
                "content": f"Đề bài: {state.task_description}\nPhân tích giải thuật tối ưu hóa nợ và dòng tiền."
            }
        ]
        response = self.llm.call(messages=messages)
        content = response.get("content", "")

        state.requirements.append(f"[DEBT_CASHFLOW_OPTIMIZER_SPEC]:\n[GRAPH_DEBT_SOLVER_SPEC]:\n{content}")
        return state


TarjanGraphAgent = DebtOptimizerAgent
