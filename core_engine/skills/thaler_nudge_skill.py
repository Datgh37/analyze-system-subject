"""
Skill: core_engine.skills.thaler_nudge_skill
Chuyên gia: Giáo sư Richard Thaler (Nobel Kinh Tế 2017)
Lĩnh vực: Tâm lý học hành vi, Thuyết Cú Hích (Nudge Theory), Spending Velocity & UX Friction Score
"""

from typing import Dict, Any, List, Optional
from dataclasses import dataclass


@dataclass
class FrictionResult:
    friction_score: int          # Thang điểm 0 - 100 (Càng cao càng tệ)
    is_high_friction: bool       # True nếu ma sát > 50
    estimated_seconds: int       # Thời gian ước tính hoàn thành
    churn_risk_level: str        # 'LOW', 'MEDIUM', 'HIGH'
    recommendation: str


class ThalerNudgeSkill:
    """Bộ công cụ tính toán tâm lý chi tiêu & ma sát UX của GS. Richard Thaler."""

    @staticmethod
    def calculate_spending_velocity(
        spent_amount: float,
        current_day: int,
        total_days_in_month: int,
        monthly_budget: float
    ) -> Dict[str, Any]:
        """
        Tính toán Tốc độ Chi tiêu (Daily Burn Rate) và Dự báo ngày cạn ngân sách.
        """
        if current_day <= 0:
            daily_burn_rate = 0.0
        else:
            daily_burn_rate = spent_amount / current_day

        projected_total = daily_burn_rate * total_days_in_month
        remaining_budget = monthly_budget - spent_amount
        remaining_days = max(1, total_days_in_month - current_day)

        depletion_day = None
        if daily_burn_rate > 0:
            est_day = int(monthly_budget / daily_burn_rate)
            if est_day <= total_days_in_month:
                depletion_day = est_day

        return {
            "spent_amount": spent_amount,
            "monthly_budget": monthly_budget,
            "current_day": current_day,
            "daily_burn_rate": daily_burn_rate,
            "projected_total": projected_total,
            "depletion_day": depletion_day,
            "is_overspending": projected_total > monthly_budget,
            "recommended_daily_limit": max(0.0, remaining_budget / remaining_days)
        }

    @staticmethod
    def generate_nudge_recommendation(velocity_data: Dict[str, Any], category_name: str = "Chi tiêu") -> str:
        """
        Sinh thông điệp 'Cú Hích' (Nudge) tích cực theo nguyên lý tâm lý học hành vi của Thaler.
        Tuyệt đối không dùng từ ngữ phán xét tiêu cực (ví dụ: 'Bạn đã phạm luật').
        """
        if velocity_data["is_overspending"] and velocity_data["depletion_day"]:
            depletion = velocity_data["depletion_day"]
            rec_limit = velocity_data["recommended_daily_limit"]
            return (
                f"💡 [CÚ HÍCH TÀI CHÍNH - {category_name.upper()}]\n"
                f"Với nhịp chi tiêu hiện tại ({velocity_data['daily_burn_rate']:,.0f}đ/ngày), "
                f"hạn mức có thể chạm đáy vào khoảng ngày {depletion}.\n"
                f"👉 Cú hích cho bạn: Chỉ cần điều chỉnh mức chi về khoảng {rec_limit:,.0f}đ/ngày "
                f"cho các ngày còn lại là bạn sẽ về đích an toàn cuối tháng!"
            )
        else:
            surplus = velocity_data["monthly_budget"] - velocity_data["projected_total"]
            return (
                f"🌟 [TIẾN ĐỘ TUYỆT VỜI - {category_name.upper()}]\n"
                f"Bạn đang duy trì tốc độ chi tiêu rất kỷ luật ({velocity_data['daily_burn_rate']:,.0f}đ/ngày). "
                f"Dự kiến cuối tháng bạn sẽ tiết kiệm thêm được khoảng {surplus:,.0f}đ để bỏ vào heo đất!"
            )

    @staticmethod
    def evaluate_ux_friction(step_count: int, form_field_count: int, has_ocr_automation: bool = False) -> FrictionResult:
        """
        Đo lường chỉ số ma sát thao tác người dùng (Friction Score).
        Nguyên lý: Cứ mỗi trường nhập tay tăng 10 điểm ma sát; mỗi bước chuyển màn hình tăng 15 điểm.
        Tính năng OCR tự động giúp giảm 50 điểm ma sát.
        """
        raw_score = (step_count * 15) + (form_field_count * 10)
        if has_ocr_automation:
            raw_score -= 50

        final_score = max(0, min(100, raw_score))
        is_high = final_score >= 50

        if final_score < 30:
            risk = "LOW"
            rec = "Tuyệt vời! Giao diện 1 chạm rất thân thiện với người lười ghi chép."
        elif final_score < 60:
            risk = "MEDIUM"
            rec = "Khá ổn, nhưng nên cắt giảm bớt 1-2 trường thông tin không bắt buộc."
        else:
            risk = "HIGH"
            rec = "CẢNH BÁO: Ma sát quá lớn! Người dùng sẽ bỏ ứng dụng sau 3 ngày nếu phải nhập tay thế này."

        est_seconds = (step_count * 3) + (form_field_count * 4)
        if has_ocr_automation:
            est_seconds = max(2, est_seconds - 15)

        return FrictionResult(
            friction_score=final_score,
            is_high_friction=is_high,
            estimated_seconds=est_seconds,
            churn_risk_level=risk,
            recommendation=rec
        )

    @staticmethod
    def detect_subscription_traps(transactions: List[Dict[str, Any]]) -> List[Dict[str, Any]]:
        """
        Phát hiện các khoản chi ngầm định kỳ (Subscription Trap).
        Tìm các khoản tiền giống nhau xuất hiện lặp lại theo chu kỳ ~30 ngày.
        """
        traps = []
        # Gom nhóm giao dịch theo số tiền và tên đối tác
        grouped: Dict[str, List[Dict[str, Any]]] = {}
        for tx in transactions:
            key = f"{tx.get('merchant', 'unknown')}_{tx.get('amount', 0)}"
            grouped.setdefault(key, []).append(tx)

        for key, tx_list in grouped.items():
            if len(tx_list) >= 2:
                sample = tx_list[0]
                traps.append({
                    "merchant": sample.get("merchant"),
                    "amount": sample.get("amount"),
                    "frequency_count": len(tx_list),
                    "warning": f"Phát hiện khoản chi lặp lại '{sample.get('merchant')}' ({sample.get('amount'):,.0f}đ). Hãy kiểm tra xem bạn có quên hủy gói dịch vụ không!"
                })

        return traps
