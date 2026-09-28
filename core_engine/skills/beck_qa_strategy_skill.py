"""
Skill: core_engine.skills.beck_qa_strategy_skill
Chuyên gia: Kent Beck & Martin Fowler (Agent 12 - Lead Full-Stack Test Strategist & QA Automation Specialist)
Lĩnh vực: Full-Stack Test Strategy, Test Pyramid, Playwright E2E Automation & k6 Performance Load Testing
"""

from typing import Dict, Any, List, Optional


class BeckQAStrategySkill:
    """Bộ công cụ thiết kế chiến lược kiểm thử tự động toàn diện, kim tự tháp kiểm thử và kịch bản E2E/Load Test."""

    @staticmethod
    def calculate_test_pyramid_distribution(
        unit_count: int,
        integration_count: int,
        e2e_count: int
    ) -> Dict[str, Any]:
        """
        Tính toán phân bổ Kim Tự Tháp Kiểm Thử thực tế (Test Pyramid Distribution)
        so với tỷ lệ vàng chuẩn Martin Fowler (70% Unit, 20% Integration, 10% E2E).
        """
        total = unit_count + integration_count + e2e_count
        if total == 0:
            return {
                "total_tests": 0,
                "health_score": 0,
                "verdict": "CHƯA CÓ TEST",
                "anti_pattern_detected": None
            }

        pct_unit = round((unit_count / total) * 100, 1)
        pct_int = round((integration_count / total) * 100, 1)
        pct_e2e = round((e2e_count / total) * 100, 1)

        # Tính Health Score dựa trên độ lệch so với tỷ lệ chuẩn (70-20-10)
        deviation = abs(pct_unit - 70.0) + abs(pct_int - 20.0) + abs(pct_e2e - 10.0)
        health_score = max(0, min(100, int(100 - (deviation * 0.75))))

        # Nhận diện Anti-patterns
        anti_pattern = None
        if pct_e2e > 40.0:
            anti_pattern = "Kem Ốc Quế (Ice Cream Cone Anti-pattern: Quá nhiều E2E Test chậm và dễ gãy, quá ít Unit Test)"
        elif pct_int < 5.0 and pct_unit > 40.0 and pct_e2e > 30.0:
            anti_pattern = "Đồng Hồ Cát (Hourglass Anti-pattern: Bỏ qua tầng Integration Test CSDL & API)"

        verdict = "XUẤT SẮC (Kim tự tháp cân đối)" if health_score >= 80 else ("ĐẠT YÊU CẦU" if health_score >= 60 else "CẦN TỐI ƯU HÓA")

        return {
            "total_tests": total,
            "unit_tests": {"count": unit_count, "percentage": pct_unit, "target": "70%"},
            "integration_tests": {"count": integration_count, "percentage": pct_int, "target": "20%"},
            "e2e_tests": {"count": e2e_count, "percentage": pct_e2e, "target": "10%"},
            "health_score": health_score,
            "verdict": verdict,
            "anti_pattern_detected": anti_pattern
        }

    @staticmethod
    def generate_e2e_playwright_scenario(
        feature_name: str,
        user_flow: Optional[List[str]] = None
    ) -> str:
        """
        Tự động sinh mã nguồn kịch bản kiểm thử E2E Playwright (TypeScript) chuẩn mực.
        """
        default_flow = [
            "1. Người dùng đăng nhập hệ thống",
            "2. Truy cập vào Quỹ chung gia đình hoặc Nhóm bạn",
            "3. Tạo mới một khoản chi tiêu hoặc quét hóa đơn OCR",
            "4. Kiểm tra cập nhật số dư tức thì trong 0ms (Optimistic UI)",
            "5. Xác nhận nợ được rút gọn trên biểu đồ đồ thị"
        ]
        flow = user_flow if user_flow else default_flow

        steps_code = []
        for step in flow:
            steps_code.append(f"    // Step: {step}")
            if "đăng nhập" in step.lower():
                steps_code.append("    await page.goto('/login');\n    await page.fill('#input-username', 'test_user');\n    await page.fill('#input-password', 'Secret@123');\n    await page.click('#btn-login');\n    await expect(page).toHaveURL('/dashboard');")
            elif "quỹ" in step.lower() or "nhóm" in step.lower():
                steps_code.append("    await page.click('#nav-joint-fund');\n    await expect(page.locator('#fund-title')).toBeVisible();")
            elif "chi tiêu" in step.lower() or "ocr" in step.lower():
                steps_code.append("    await page.click('#btn-add-expense');\n    await page.fill('#input-amount', '250000');\n    await page.selectOption('#select-category', 'Food');\n    await page.click('#btn-confirm-expense');")
            elif "số dư" in step.lower() or "optimistic" in step.lower():
                steps_code.append("    await expect(page.locator('#fund-balance')).toContainText('24,500,000');\n    await expect(page.locator('.toast-success')).toBeVisible();")
            elif "nợ" in step.lower() or "đồ thị" in step.lower():
                steps_code.append("    await page.click('#nav-debt-graph');\n    await expect(page.locator('#debt-canvas')).toBeVisible();")
            else:
                steps_code.append("    await page.waitForTimeout(500);")

        formatted_steps = "\n\n".join(steps_code)

        return (
            "import { test, expect } from '@playwright/test';\n\n"
            f"test.describe('E2E Verification: {feature_name}', () => {{\n"
            f"  test('Luồng người dùng hoàn chỉnh ({feature_name})', async ({{ page }}) => {{\n"
            f"{formatted_steps}\n"
            "  });\n"
            "});\n"
        )

    @staticmethod
    def design_load_test_profile(
        feature_name: str,
        target_vus: int = 500
    ) -> Dict[str, Any]:
        """
        Thiết kế hồ sơ kiểm thử tải hiệu năng bằng công cụ k6 / Locust.
        """
        return {
            "tool": "k6 (Grafana)",
            "feature": feature_name,
            "target_concurrent_users": target_vus,
            "stages": [
                {"duration": "2m", "target": int(target_vus * 0.5), "phase": "Ramp-up (Khởi động)"},
                {"duration": "3m", "target": target_vus, "phase": "Peak Load (Tải cao điểm)"},
                {"duration": "1m", "target": 0, "phase": "Ramp-down (Hạ nhiệt)"}
            ],
            "sla_thresholds": {
                "http_req_duration_p95": "< 200ms",
                "http_req_duration_p99": "< 500ms",
                "http_req_failed_rate": "< 1.0%",
                "database_connection_pool_saturation": "< 80%"
            },
            "k6_script_snippet": (
                "import http from 'k6/http';\n"
                "import { check, sleep } from 'k6';\n\n"
                "export const options = {\n"
                f"  stages: [\n"
                f"    {{ duration: '2m', target: {int(target_vus * 0.5)} }},\n"
                f"    {{ duration: '3m', target: {target_vus} }},\n"
                f"    {{ duration: '1m', target: 0 }},\n"
                f"  ],\n"
                f"  thresholds: {{\n"
                f"    http_req_duration: ['p(95)<200', 'p(99)<500'],\n"
                f"    http_req_failed: ['rate<0.01'],\n"
                f"  }},\n"
                "};\n"
            )
        }

    @classmethod
    def format_qa_verification_plan(cls, feature_name: str) -> str:
        """Định dạng kế hoạch kiểm thử QA toàn diện nhúng vào RFC chuyển giao."""
        pyramid = cls.calculate_test_pyramid_distribution(35, 10, 5)
        load = cls.design_load_test_profile(feature_name, target_vus=500)
        e2e_code = cls.generate_e2e_playwright_scenario(feature_name)

        return (
            f"## KẾ HOẠCH KIỂM THỬ TỰ ĐỘNG & ĐẢM BẢO CHẤT LƯỢNG QA (Kent Beck & Martin Fowler)\n\n"
            f"### 1. Phân Bổ Kim Tự Tháp Kiểm Thử (Test Pyramid Health Score: {pyramid['health_score']}/100 — {pyramid['verdict']})\n"
            f"- **Unit Tests (70%):** {pyramid['unit_tests']['count']} bài ({pyramid['unit_tests']['percentage']}%) — Kiểm thử logic Money, Graph, Nudge.\n"
            f"- **Integration Tests (20%):** {pyramid['integration_tests']['count']} bài ({pyramid['integration_tests']['percentage']}%) — Kiểm thử CSDL 3NF, API, Redis Lock.\n"
            f"- **E2E Tests (10%):** {pyramid['e2e_tests']['count']} bài ({pyramid['e2e_tests']['percentage']}%) — Kiểm thử luồng người dùng trọn vẹn.\n\n"
            f"### 2. Tiêu Chuẩn Kiểm Thử Tải Hiệu Năng (k6 Performance SLA)\n"
            f"- **Tải mục tiêu:** `{load['target_concurrent_users']}` Virtual Users (VUs)\n"
            f"- **Ngưỡng SLA p95:** `{load['sla_thresholds']['http_req_duration_p95']}`\n"
            f"- **Tỷ lệ lỗi chấp nhận:** `{load['sla_thresholds']['http_req_failed_rate']}`\n\n"
            f"### 3. Kịch Bản Kiểm Thử E2E Tự Động (Playwright Spec)\n"
            f"```typescript\n{e2e_code}```\n"
        )
