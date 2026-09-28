"""
Package: core_engine.agents
Tập hợp các Agent chuyên biệt theo chuẩn Claude & SWE Architecture.
"""

from core_engine.agents.base_agent import BaseAgent
from core_engine.agents.coding_agent import CodingAgent
from core_engine.agents.requirement_agent import RequirementAgent
from core_engine.agents.planner_agent import PlannerAgent
from core_engine.agents.tester_agent import TesterAgent
from core_engine.agents.reviewer_agent import ReviewerAgent

__all__ = [
    "BaseAgent",
    "CodingAgent",
    "RequirementAgent",
    "PlannerAgent",
    "TesterAgent",
    "ReviewerAgent",
]
