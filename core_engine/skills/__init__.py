"""
Package: core_engine.skills
Tập hợp các Python Skills chuyên biệt cho từng Chuyên Gia trong Hội Đồng.
"""

from core_engine.skills.thaler_nudge_skill import ThalerNudgeSkill, FrictionResult
from core_engine.skills.personal_budget_cases_skill import PersonalBudgetCasesSkill, GroupSplitCasesSkill
from core_engine.skills.cockburn_use_case_skill import CockburnUseCaseSkill
from core_engine.skills.evans_domain_skill import EvansDomainSkill, Money
from core_engine.skills.kleppmann_database_skill import KleppmannDatabaseSkill
from core_engine.skills.debt_cashflow_optimizer_skill import DebtCashflowOptimizerSkill, TarjanGraphSkill
from core_engine.skills.vogels_async_skill import VogelsAsyncSkill
from core_engine.skills.andrew_ng_ai_skill import AndrewNgAISkill
from core_engine.skills.orchestrator_rfc_skill import OrchestratorRFCSkill
from core_engine.skills.frost_atomic_design_skill import FrostAtomicDesignSkill
from core_engine.skills.garnier_motion_skill import GarnierMotionSkill
from core_engine.skills.abramov_client_state_skill import AbramovClientStateSkill
from core_engine.skills.schneier_security_skill import SchneierSecuritySkill
from core_engine.skills.beck_qa_strategy_skill import BeckQAStrategySkill

__all__ = [
    "ThalerNudgeSkill",
    "FrictionResult",
    "PersonalBudgetCasesSkill",
    "GroupSplitCasesSkill",
    "CockburnUseCaseSkill",
    "EvansDomainSkill",
    "Money",
    "KleppmannDatabaseSkill",
    "DebtCashflowOptimizerSkill",
    "TarjanGraphSkill",
    "VogelsAsyncSkill",
    "AndrewNgAISkill",
    "OrchestratorRFCSkill",
    "FrostAtomicDesignSkill",
    "GarnierMotionSkill",
    "AbramovClientStateSkill",
    "SchneierSecuritySkill",
    "BeckQAStrategySkill",
]
