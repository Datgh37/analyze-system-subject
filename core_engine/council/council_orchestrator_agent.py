"""
Module: core_engine.council.council_orchestrator_agent
Agent: 00_orchestrator (Council Orchestrator - Chủ tịch Hội đồng)
Nhiệm vụ: Lãnh đạo Hội đồng, Điều phối Chu trình 4 Pha và Xuất bản Final RFC (14 Agents Full-Stack)
"""

from typing import Dict, Any, List, Optional
from pathlib import Path

from core_engine.council.persona_agent import PersonaAgent
from core_engine.council.richard_thaler_agent import RichardThalerAgent
from core_engine.council.personal_budget_agent import PersonalBudgetAgent, GroupLeaderAgent
from core_engine.council.cockburn_ba_agent import CockburnBAAgent
from core_engine.council.eric_evans_agent import EricEvansAgent
from core_engine.council.martin_kleppmann_agent import MartinKleppmannAgent
from core_engine.council.debt_optimizer_agent import DebtOptimizerAgent, TarjanGraphAgent
from core_engine.council.werner_vogels_agent import WernerVogelsAgent
from core_engine.council.andrew_ng_agent import AndrewNgAgent
from core_engine.council.brad_frost_agent import BradFrostAgent
from core_engine.council.julian_garnier_agent import JulianGarnierAgent
from core_engine.council.dan_abramov_agent import DanAbramovAgent
from core_engine.council.bruce_schneier_agent import BruceSchneierAgent
from core_engine.council.kent_beck_agent import KentBeckAgent

from core_engine.skills.orchestrator_rfc_skill import OrchestratorRFCSkill
from core_engine.state import ProjectState


class CouncilOrchestratorAgent(PersonaAgent):
    """
    Tác tử Chủ tịch Hội đồng chịu trách nhiệm điều phối toàn bộ 13 Chuyên gia Danh nhân Full-Stack.
    """

    def __init__(self, llm_client, filesystem, terminal, git):
        super().__init__(
            name="CouncilOrchestratorAgent",
            spec_file_path="core/00_orchestrator.md",
            llm_client=llm_client,
            filesystem=filesystem,
            terminal=terminal,
            git=git,
        )
        self.skill = OrchestratorRFCSkill()
        self._register_orchestrator_skills()

        # Khởi tạo toàn bộ 13 Agent thành viên của Hội đồng
        self.thaler = RichardThalerAgent(llm_client, filesystem, terminal, git)
        self.personal_budget = PersonalBudgetAgent(llm_client, filesystem, terminal, git)
        self.group_leader = self.personal_budget
        self.cockburn = CockburnBAAgent(llm_client, filesystem, terminal, git)
        self.evans = EricEvansAgent(llm_client, filesystem, terminal, git)
        self.kleppmann = MartinKleppmannAgent(llm_client, filesystem, terminal, git)
        self.debt_optimizer = DebtOptimizerAgent(llm_client, filesystem, terminal, git)
        self.tarjan = self.debt_optimizer
        self.vogels = WernerVogelsAgent(llm_client, filesystem, terminal, git)
        self.andrew_ng = AndrewNgAgent(llm_client, filesystem, terminal, git)
        self.brad_frost = BradFrostAgent(llm_client, filesystem, terminal, git)
        self.julian_garnier = JulianGarnierAgent(llm_client, filesystem, terminal, git)
        self.dan_abramov = DanAbramovAgent(llm_client, filesystem, terminal, git)
        self.bruce_schneier = BruceSchneierAgent(llm_client, filesystem, terminal, git)
        self.kent_beck = KentBeckAgent(llm_client, filesystem, terminal, git)

    def _register_orchestrator_skills(self):
        """Đăng ký các hàm điều phối và thẩm định RFC vào tools_map."""
        self.tools_map.update({
            "compile_final_rfc": self.skill.compile_final_rfc,
            "verify_session_integrity": self.skill.verify_session_integrity,
            "format_session_summary_table": self.skill.format_session_summary_table,
        })

    def run_full_session(self, session_dir_path: str, task_description: str) -> ProjectState:
        """
        Chạy tự động toàn bộ Chu trình 4 Pha của Phiên Bàn Thảo với 12 Chuyên gia Full-Stack:
        - Pha 1: Trải nghiệm & Nhu cầu (Thaler, Group Leader) -> 01_user_feedback.md
        - Pha 2: Đặc tả Nghiệp vụ & Use Cases (Cockburn BA) -> 02_use_case_specs.md
        - Pha 2.5: Thiết Kế Giao Diện & UI/UX (Brad Frost, Dan Abramov) -> 02b_frontend_ui_spec.md
        - Pha 3: Phản biện Kỹ thuật Toàn diện (Evans, Kleppmann, Tarjan, Vogels, Andrew Ng, Schneier, Beck) -> 03_technical_adr.md
        - Pha 4: Tổng hợp & Phê duyệt Final RFC (Council Orchestrator) -> 04_final_rfc.md
        """
        session_path = Path(session_dir_path)
        session_path.mkdir(parents=True, exist_ok=True)

        state = ProjectState(
            task_id=f"SESSION-{session_path.name.upper()}",
            task_description=task_description
        )

        # Pha 1: Khách hàng & Tâm lý chi tiêu
        state = self.thaler.step(state)
        state = self.group_leader.step(state)
        f1_content = "\n\n---\n\n".join([
            r for r in state.requirements 
            if any(k in r for k in ["THALER", "HOUSEHOLD", "INSIGHT"])
        ])
        (session_path / "01_user_feedback.md").write_text(f1_content or "# USER FEEDBACK\nHoàn tất Pha 1.", encoding="utf-8")

        # Pha 2: BA Chuẩn hóa Yêu Cầu
        state = self.cockburn.step(state)
        f2_content = "\n\n---\n\n".join([
            r for r in state.requirements 
            if "COCKBURN" in r or "USE_CASE" in r
        ])
        (session_path / "02_use_case_specs.md").write_text(f2_content or "# USE CASE SPECS\nHoàn tất Pha 2.", encoding="utf-8")

        # Pha 2.5: Thiết Kế Giao Diện UI/UX, Motion Choreography & Client State
        state = self.brad_frost.step(state)
        state = self.julian_garnier.step(state)
        state = self.dan_abramov.step(state)
        f2b_content = "\n\n---\n\n".join([
            r for r in state.requirements 
            if any(k in r for k in [
                "ATOMIC_DESIGN", "UI_SPEC", "MOTION_CHOREOGRAPHY",
                "SPRING_PHYSICS", "CLIENT_STATE", "OPTIMISTIC_UI"
            ])
        ])
        (session_path / "02b_frontend_ui_spec.md").write_text(f2b_content or "# FRONTEND UI/UX SPEC\nHoàn tất Pha 2.5.", encoding="utf-8")

        # Pha 3: Kỹ thuật Toàn diện (Domain, DB, Graph, Async, AI, Security, QA)
        state = self.evans.step(state)
        state = self.kleppmann.step(state)
        state = self.tarjan.step(state)
        state = self.vogels.step(state)
        state = self.andrew_ng.step(state)
        state = self.bruce_schneier.step(state)
        state = self.kent_beck.step(state)

        f3_content = "\n\n---\n\n".join([
            r for r in state.requirements 
            if any(k in r for k in [
                "EVANS", "KLEPPMANN", "TARJAN", "VOGELS", "ANDREW_NG",
                "CLEAN_ARCHITECTURE", "DATABASE", "GRAPH", "ASYNC", "AI",
                "FINTECH_SECURITY", "STRIDE", "FULL_STACK_QA", "TEST_STRATEGY"
            ])
        ])
        (session_path / "03_technical_adr.md").write_text(f3_content or "# TECHNICAL ADR\nHoàn tất Pha 3.", encoding="utf-8")

        # Pha 4: Xuất bản Final RFC
        state = self.step(state)
        self.skill.compile_final_rfc(session_dir_path, feature_title=task_description)

        return state


    def step(self, state: ProjectState) -> ProjectState:
        """
        Pha 4: Chủ tịch Hội đồng phê duyệt và tổng hợp RFC cuối cùng.
        """
        requirements_summary = "\n".join(state.requirements)
        messages = [
            {"role": "system", "content": self.system_prompt},
            {
                "role": "user",
                "content": (
                    f"Đề bài tính năng: {state.task_description}\n"
                    f"Tổng hợp phản biện từ cả 13 Chuyên gia Hội đồng Full-Stack:\n{requirements_summary}\n\n"
                    f"Hãy đóng vai Council Orchestrator: Tổng duyệt tính nhất quán, kết luận và phê duyệt Final RFC."
                )
            }
        ]

        response = self.llm.call(messages=messages)
        orchestrator_verdict = response.get("content", "")

        state.requirements.append(f"[COUNCIL_FINAL_APPROVAL]: {orchestrator_verdict[:300]}...")
        return state
