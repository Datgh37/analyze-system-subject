"""
Skill: core_engine.skills.cockburn_use_case_skill
Chuyên gia: Alistair Cockburn & Debra Paul (Agent 03 - Lead BA)
Lĩnh vực: Phân tích Nghiệp vụ, Chuẩn hóa Use Case IEEE 29148 & Gherkin BDD Acceptance Criteria
"""

from typing import Dict, Any, List, Optional


class CockburnUseCaseSkill:
    """Bộ công cụ chuẩn hóa Phân tích Nghiệp vụ chuẩn IEEE 29148 & Gherkin BDD."""

    @staticmethod
    def format_ieee_use_case_spec(use_case: Dict[str, Any]) -> str:
        """
        Định dạng tài liệu Use Case Specification theo chuẩn quốc tế IEEE 29148.
        """
        uc_id = use_case.get("id", "UC-01")
        name = use_case.get("name", "Chưa đặt tên")
        goal_level = use_case.get("goal_level", "Sea-Level (Mục tiêu người dùng)")
        actor = use_case.get("primary_actor", "Người dùng đã xác thực")
        pre_conditions = use_case.get("pre_conditions", ["Người dùng đã đăng nhập hệ thống"])
        post_conditions = use_case.get("post_conditions", ["Trạng thái số dư được cập nhật"])
        main_flow = use_case.get("main_flow", [])
        exceptions = use_case.get("exceptions", [])
        nfrs = use_case.get("nfrs", ["Thời gian phản hồi < 500ms"])
        gherkin = use_case.get("gherkin_scenarios", "")

        pre_md = "\n".join(f"- {p}" for p in pre_conditions)
        post_md = "\n".join(f"- {p}" for p in post_conditions)
        main_md = "\n".join(f"{i+1}. {step}" for i, step in enumerate(main_flow))
        exc_md = "\n".join(f"- **{e.get('step', 'Ngoại lệ')}:** {e.get('handling', '')}" for e in exceptions)
        nfr_md = "\n".join(f"- {n}" for n in nfrs)

        doc = (
            f"# ĐẶC TẢ USE CASE: {uc_id} — {name.upper()}\n"
            f"*(Theo chuẩn quốc tế IEEE 29148 — Alistair Cockburn Methodology)*\n\n"
            f"## 1. Thông Tin Tổng Quan\n"
            f"- **Mã Use Case:** `{uc_id}`\n"
            f"- **Tên Use Case:** {name}\n"
            f"- **Cấp độ mục tiêu (Goal Level):** {goal_level}\n"
            f"- **Tác nhân chính (Primary Actor):** {actor}\n\n"
            f"## 2. Điều Kiện Tiên Quyết & Đảm Bảo (Pre & Post Conditions)\n"
            f"### Điều kiện tiên quyết (Pre-conditions):\n{pre_md}\n\n"
            f"### Đảm bảo sau khi hoàn thành (Post-conditions):\n{post_md}\n\n"
            f"## 3. Luồng Sự Kiện Chính (Main Success Scenario)\n{main_md}\n\n"
            f"## 4. Các Luồng Ngoại Lệ & Xử Lý Sự Cố (Extensions & Exceptions)\n{exc_md}\n\n"
            f"## 5. Yêu Cầu Phi Chức Năng (Non-Functional Requirements)\n{nfr_md}\n\n"
            f"## 6. Tiêu Chí Nghiệm Thu (Acceptance Criteria - Gherkin BDD)\n"
            f"```gherkin\n{gherkin}\n```\n"
        )
        return doc

    @staticmethod
    def generate_gherkin_scenarios(feature_name: str, scenarios: List[Dict[str, Any]]) -> str:
        """
        Tự động sinh kịch bản kiểm thử hành vi BDD chuẩn cú pháp Gherkin.
        """
        lines = [f"Feature: {feature_name}", ""]
        for sc in scenarios:
            title = sc.get("title", "Kịch bản kiểm thử")
            given = sc.get("given", "Hệ thống đang hoạt động")
            when = sc.get("when", "Người dùng thực hiện thao tác")
            then = sc.get("then", "Kết quả mong đợi hiển thị")
            lines.append(f"  Scenario: {title}")
            lines.append(f"    Given {given}")
            lines.append(f"    When {when}")
            lines.append(f"    Then {then}")
            lines.append("")
        return "\n".join(lines)

    @staticmethod
    def validate_use_case_completeness(use_case: Dict[str, Any]) -> Dict[str, Any]:
        """
        Kiểm tra độ hoàn thiện và tính chặt chẽ của một bản đặc tả Use Case.
        Điểm chuẩn tối đa 100 điểm.
        """
        score = 0
        missing = []

        if use_case.get("id") and use_case.get("name"):
            score += 15
        else:
            missing.append("Thiếu mã hoặc tên Use Case")

        if use_case.get("primary_actor"):
            score += 15
        else:
            missing.append("Thiếu Tác nhân chính (Primary Actor)")

        if use_case.get("pre_conditions"):
            score += 15
        else:
            missing.append("Thiếu Điều kiện tiên quyết (Pre-conditions)")

        main_flow = use_case.get("main_flow", [])
        if len(main_flow) >= 3:
            score += 25
        elif len(main_flow) > 0:
            score += 15
            missing.append("Luồng chính quá ngắn (< 3 bước)")
        else:
            missing.append("Thiếu Luồng sự kiện chính (Main Flow)")

        exceptions = use_case.get("exceptions", [])
        if len(exceptions) >= 1:
            score += 15
        else:
            missing.append("CẢNH BÁO NGUY HIỂM: Thiếu luồng ngoại lệ (Exceptions)")

        if use_case.get("gherkin_scenarios"):
            score += 15
        else:
            missing.append("Thiếu Tiêu chí nghiệm thu Gherkin BDD")

        return {
            "completeness_score": score,
            "is_ready_for_architecture": score >= 80,
            "missing_elements": missing,
            "verdict": "ĐẠT CHUẨN IEEE 29148" if score >= 80 else "CHƯA ĐẠT (CẦN BỔ SUNG)"
        }
