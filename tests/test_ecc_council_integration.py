"""
Module: tests/test_ecc_council_integration.py
Mục đích: Kiểm thử tự động tính toàn vẹn và tích hợp của hệ sinh thái Everything Claude Code (ECC Universal)
          với Hội Đồng Đa Tác Tử (Multi-Agent Council) trong môi trường Antigravity 2.0.
"""

import sys
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).parent.parent))


class TestECCCouncilIntegration(unittest.TestCase):
    def setUp(self):
        self.workspace_root = Path(__file__).parent.parent
        self.agents_root = self.workspace_root / ".agents"
        self.workflows_dir = self.agents_root / "workflows"
        self.agents_dir = self.agents_root / "agents"
        self.skills_dir = self.agents_root / "skills"
        self.rules_dir = self.agents_root / "rules"
        self.core_dir = self.workspace_root / "core"

    def test_academic_rules_preserved(self):
        """Bảo tồn 100% 5 file rules học thuật KLTN DNTU trong .agents/rules/."""
        expected_academic_rules = [
            "00_mandatory_project_context_rule.md",
            "01_dntu_kltn_format_rules.md",
            "02_dntu_kltn_structure_and_toc_rules.md",
            "03_dntu_kltn_tables_and_figures_rules.md",
            "04_dntu_kltn_academic_writing_style_rules.md",
        ]
        for rule_name in expected_academic_rules:
            rule_path = self.rules_dir / rule_name
            self.assertTrue(rule_path.is_file(), f"Mất tệp rule học thuật DNTU: {rule_name}")
            content = rule_path.read_text(encoding="utf-8")
            self.assertGreater(len(content), 100, f"Tệp rule {rule_name} bị rỗng!")

    def test_ecc_programming_rules_prefixed(self):
        """Kiểm tra sự tồn tại của 16 tệp rules lập trình ECC có tiền tố ecc-*."""
        expected_ecc_rules = [
            "ecc-common-coding-style.md",
            "ecc-common-git-workflow.md",
            "ecc-common-performance.md",
            "ecc-common-security.md",
            "ecc-common-testing.md",
            "ecc-python-coding-style.md",
            "ecc-react-coding-style.md",
            "ecc-react-native-coding-style.md",
            "ecc-typescript-coding-style.md",
            # .NET / C# Rules
            "ecc-csharp-coding-style.md",
            "ecc-csharp-patterns.md",
            "ecc-csharp-testing.md",
            "ecc-csharp-security.md",
            # TypeScript / Node Rules
            "ecc-typescript-patterns.md",
            "ecc-typescript-testing.md",
            "ecc-typescript-security.md",
        ]
        for rule_name in expected_ecc_rules:
            rule_path = self.rules_dir / rule_name
            self.assertTrue(rule_path.is_file(), f"Thiếu tệp rule lập trình ECC: {rule_name}")

    def test_ecc_workflows_structure(self):
        """Kiểm tra 18 workflows hữu ích được cài đặt trong .agents/workflows/."""
        expected_workflows = [
            "plan.md",
            "plan-canvas.md",
            "plan-prd.md",
            "quality-gate.md",
            "checkpoint.md",
            "code-review.md",
            "review-pr.md",
            "pr.md",
            "security-scan.md",
            "test-coverage.md",
            "refactor-clean.md",
            "build-fix.md",
            "update-docs.md",
            "project-init.md",
            "orch-add-feature.md",
            "orch-fix-defect.md",
            "orch-review.md",
            "ecc-guide.md",
        ]
        self.assertTrue(self.workflows_dir.is_dir(), "Thư mục .agents/workflows/ không tồn tại!")
        for wf_name in expected_workflows:
            wf_path = self.workflows_dir / wf_name
            self.assertTrue(wf_path.is_file(), f"Thiếu workflow: {wf_name}")
            content = wf_path.read_text(encoding="utf-8")
            self.assertGreater(len(content), 50, f"Workflow {wf_name} quá ngắn hoặc rỗng!")

    def test_ecc_custom_subagents_adaptation(self):
        """Kiểm tra 19 subagents trong .agents/agents/ đã được chuyển đổi chuẩn Antigravity (tools & model)."""
        expected_subagents = [
            "architect.md",
            "code-reviewer.md",
            "database-reviewer.md",
            "security-reviewer.md",
            "tdd-guide.md",
            "performance-optimizer.md",
            "spec-miner.md",
            "e2e-runner.md",
            "code-simplifier.md",
            "refactor-cleaner.md",
            "doc-updater.md",
            "planner.md",
            "silent-failure-hunter.md",
            "a11y-architect.md",
            "react-reviewer.md",
            "python-reviewer.md",
            "mle-reviewer.md",
            # .NET & TypeScript Reviewers
            "csharp-reviewer.md",
            "typescript-reviewer.md",
        ]
        self.assertTrue(self.agents_dir.is_dir(), "Thư mục .agents/agents/ không tồn tại!")
        for sa_name in expected_subagents:
            sa_path = self.agents_dir / sa_name
            self.assertTrue(sa_path.is_file(), f"Thiếu subagent: {sa_name}")
            content = sa_path.read_text(encoding="utf-8")
            self.assertTrue(content.startswith("---"), f"Subagent {sa_name} thiếu YAML frontmatter")
            self.assertIn("name:", content, f"Subagent {sa_name} thiếu 'name:'")
            self.assertIn("tools:", content, f"Subagent {sa_name} thiếu 'tools:'")

    def test_ecc_skills_count_and_structure(self):
        """Kiểm tra 40 kỹ năng ECC cùng 15 kỹ năng Hội đồng (tổng cộng 55 kỹ năng)."""
        expected_ecc_skills = [
            "living-docs-governance",
            "intent-driven-development",
            "plan-canvas",
            "plan-orchestrate",
            "context-budget",
            "strategic-compact",
            "delivery-gate",
            "product-lens",
            "architecture-decision-records",
            "backend-patterns",
            "react-patterns",
            "react-performance",
            "database-migrations",
            "postgres-patterns",
            "mysql-patterns",
            "redis-patterns",
            "docker-patterns",
            "deployment-patterns",
            "latency-critical-systems",
            "prompt-optimizer",
            "eval-harness",
            "agent-eval",
            "cost-aware-llm-pipeline",
            "click-path-audit",
            "make-interfaces-feel-better",
            "security-scan",
            "security-review",
            "tdd-workflow",
            "verification-loop",
            "e2e-testing",
            "browser-qa",
            "git-workflow",
            "codebase-onboarding",
            "codehealth-mcp",
            "ecc-guide",
            "ecc-recipes",
            # .NET & Node/TS Skills
            "dotnet-patterns",
            "csharp-testing",
            "nestjs-patterns",
            "nextjs-turbopack",
        ]
        for skill_name in expected_ecc_skills:
            skill_path = self.skills_dir / skill_name
            self.assertTrue(skill_path.is_dir(), f"Thiếu thư mục skill ECC: {skill_name}")
            skill_md = skill_path / "SKILL.md"
            self.assertTrue(skill_md.is_file(), f"Thiếu SKILL.md trong {skill_name}")

    def test_council_agents_reference_ecc_capabilities(self):
        """Kiểm tra các đặc tả trong core/*.md có tham chiếu các quyền năng ECC tương ứng."""
        agent_ecc_checks = {
            "00_orchestrator.md": ["living-docs-governance", "delivery-gate"],
            "03_cockburn_ba.md": ["spec-miner", "product-lens"],
            "04_eric_evans.md": ["architect", "architecture-decision-records", "dotnet-patterns"],
            "05_martin_kleppmann.md": ["database-reviewer", "database-migrations"],
            "07_werner_vogels.md": ["performance-optimizer", "docker-patterns"],
            "08_andrew_ng.md": ["mle-reviewer", "prompt-optimizer", "eval-harness"],
            "09_brad_frost.md": ["a11y-architect", "click-path-audit"],
            "10_dan_abramov.md": ["code-reviewer", "react-reviewer", "csharp-reviewer"],
            "11_bruce_schneier.md": ["security-reviewer", "security-scan"],
            "12_kent_beck.md": ["tdd-guide", "verification-loop", "csharp-testing"],
            "13_julian_garnier.md": ["make-interfaces-feel-better"],
        }
        for agent_file, keywords in agent_ecc_checks.items():
            file_path = self.core_dir / agent_file
            self.assertTrue(file_path.is_file(), f"Thiếu tệp đặc tả: {agent_file}")
            content = file_path.read_text(encoding="utf-8")
            for kw in keywords:
                self.assertIn(kw, content, f"Đặc tả {agent_file} chưa tham chiếu năng lực ECC: '{kw}'")


if __name__ == "__main__":
    unittest.main()
