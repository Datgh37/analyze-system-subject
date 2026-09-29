"""
Module: tests/test_antigravity_skills.py
Mục đích: Kiểm thử tự động tính toàn vẹn và hợp chuẩn của bộ kỹ năng .agents/skills/
"""

import sys
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).parent.parent))


class TestAntigravitySkills(unittest.TestCase):
    def setUp(self):
        self.workspace_root = Path(__file__).parent.parent
        self.skills_dir = self.workspace_root / ".agents" / "skills"

    def test_skills_directory_structure(self):
        expected_skills = [
            "council-orchestrator",
            "council-richard-thaler",
            "council-personal-budget-coach",
            "council-cockburn-ba",
            "council-eric-evans",
            "council-martin-kleppmann",
            "council-personal-debt-optimizer",
            "council-werner-vogels",
            "council-andrew-ng",
            "council-brad-frost",
            "council-julian-garnier",
            "council-dan-abramov",
            "council-bruce-schneier",
            "council-kent-beck",
            "council-session-runner",
        ]

        self.assertTrue(self.skills_dir.exists(), f"Thư mục {self.skills_dir} không tồn tại!")

        for skill_name in expected_skills:
            skill_path = self.skills_dir / skill_name
            skill_file = skill_path / "SKILL.md"
            self.assertTrue(skill_path.is_dir(), f"Thiếu thư mục skill: {skill_name}")
            self.assertTrue(skill_file.is_file(), f"Thiếu tệp SKILL.md trong: {skill_name}")

    def test_skills_yaml_frontmatter(self):
        for skill_path in self.skills_dir.iterdir():
            if skill_path.is_dir():
                skill_file = skill_path / "SKILL.md"
                content = skill_file.read_text(encoding="utf-8")

                self.assertTrue(content.startswith("---"), f"{skill_file.name} không bắt đầu bằng YAML frontmatter '---'")
                self.assertIn("name:", content, f"{skill_file.name} thiếu trường 'name:'")
                self.assertIn("description:", content, f"{skill_file.name} thiếu trường 'description:'")
                self.assertTrue(content.count("---") >= 2, f"{skill_file.name} thiếu thẻ đóng YAML frontmatter '---'")

    def test_skills_portability_no_absolute_paths(self):
        for skill_path in self.skills_dir.iterdir():
            if skill_path.is_dir():
                skill_file = skill_path / "SKILL.md"
                content = skill_file.read_text(encoding="utf-8").lower()

                self.assertNotIn("c:\\", content, f"{skill_file.name} chứa đường dẫn tuyệt đối C:\\")
                self.assertNotIn("d:\\", content, f"{skill_file.name} chứa đường dẫn tuyệt đối D:\\")
                self.assertNotIn("file:///d:", content, f"{skill_file.name} chứa URI tuyệt đối file:///d:")

    def test_skills_python_engine_linkage(self):
        # Kiểm tra tính khả dụng của toàn bộ các class trong core_engine được SKILL.md tham chiếu
        from core_engine.council import (
            CouncilOrchestratorAgent,
            RichardThalerAgent,
            GroupLeaderAgent,
            CockburnBAAgent,
            EricEvansAgent,
            MartinKleppmannAgent,
            TarjanGraphAgent,
            WernerVogelsAgent,
            AndrewNgAgent,
            BradFrostAgent,
            JulianGarnierAgent,
            DanAbramovAgent,
            BruceSchneierAgent,
            KentBeckAgent,
        )
        from core_engine.skills import (
            ThalerNudgeSkill,
            GroupSplitCasesSkill,
            CockburnUseCaseSkill,
            EvansDomainSkill,
            KleppmannDatabaseSkill,
            TarjanGraphSkill,
            VogelsAsyncSkill,
            AndrewNgAISkill,
            OrchestratorRFCSkill,
            FrostAtomicDesignSkill,
            GarnierMotionSkill,
            AbramovClientStateSkill,
            SchneierSecuritySkill,
            BeckQAStrategySkill,
        )

        self.assertIsNotNone(CouncilOrchestratorAgent)
        self.assertIsNotNone(ThalerNudgeSkill)
        self.assertIsNotNone(FrostAtomicDesignSkill)
        self.assertIsNotNone(GarnierMotionSkill)
        self.assertIsNotNone(AbramovClientStateSkill)
        self.assertIsNotNone(SchneierSecuritySkill)
        self.assertIsNotNone(BeckQAStrategySkill)
        self.assertIsNotNone(BradFrostAgent)
        self.assertIsNotNone(JulianGarnierAgent)
        self.assertIsNotNone(DanAbramovAgent)
        self.assertIsNotNone(BruceSchneierAgent)
        self.assertIsNotNone(KentBeckAgent)






if __name__ == "__main__":
    unittest.main(verbosity=2)
