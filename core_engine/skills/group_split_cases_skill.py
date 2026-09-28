"""
Skill: core_engine.skills.personal_budget_cases_skill
Chuyên gia: Chuyên Gia Hoạch Định & Kỷ Luật Ngân Sách Cá Nhân (Agent 02 - Personal Budget Coach)
Lĩnh vực: Quy tắc 50/30/20, Zero-Based Budgeting (ZBB), Sổ Kakeibo Nhật Bản, 6 Chiếc Hũ & Hạn mức chi tiêu ngày
"""

from typing import Dict, Any, List, Optional


class PersonalBudgetCasesSkill:
    """Bộ công cụ hoạch định ngân sách và kỷ luật tài chính cá nhân toàn diện."""

    @staticmethod
    def calculate_50_30_20_budget(
        monthly_income: float,
        needs_expenses: List[Dict[str, Any]],
        wants_expenses: List[Dict[str, Any]],
        savings_and_debt: List[Dict[str, Any]]
    ) -> Dict[str, Any]:
        """
        Quy tắc Ngân Sách 50/30/20:
        - 50% Thu nhập: Nhu cầu thiết yếu (Needs: tiền nhà, điện nước, ăn uống, y tế).
        - 30% Thu nhập: Mong muốn linh hoạt (Wants: ăn ngoài, giải trí, mua sắm).
        - 20% Thu nhập: Tiết kiệm & Trả nợ (Savings/Debt: quỹ khẩn cấp, trả nợ, đầu tư).
        """
        target_needs = round(monthly_income * 0.50, 2)
        target_wants = round(monthly_income * 0.30, 2)
        target_savings = round(monthly_income * 0.20, 2)

        actual_needs = round(sum(item.get("amount", 0.0) for item in needs_expenses), 2)
        actual_wants = round(sum(item.get("amount", 0.0) for item in wants_expenses), 2)
        actual_savings = round(sum(item.get("amount", 0.0) for item in savings_and_debt), 2)

        total_spent = actual_needs + actual_wants + actual_savings
        net_remaining = round(monthly_income - total_spent, 2)

        needs_pct = round((actual_needs / monthly_income) * 100, 1) if monthly_income > 0 else 0.0
        wants_pct = round((actual_wants / monthly_income) * 100, 1) if monthly_income > 0 else 0.0
        savings_pct = round((actual_savings / monthly_income) * 100, 1) if monthly_income > 0 else 0.0

        alerts = []
        if actual_needs > target_needs:
            alerts.append(f"CẢNH BÁO: Chi phí thiết yếu ({needs_pct}%) vượt ngưỡng 50% (+{actual_needs - target_needs:,.0f}đ).")
        if actual_wants > target_wants:
            alerts.append(f"CẢNH BÁO: Chi tiêu linh hoạt ({wants_pct}%) vượt ngưỡng 30% (+{actual_wants - target_wants:,.0f}đ).")
        if actual_savings < target_savings:
            alerts.append(f"NHẮC NHỞ: Tiết kiệm ({savings_pct}%) chưa đạt mức 20% (Thiếu {target_savings - actual_savings:,.0f}đ).")

        status = "HEALTHY" if not alerts else ("CRITICAL_OVERSPEND" if net_remaining < 0 else "NEEDS_ADJUSTMENT")

        return {
            "monthly_income": monthly_income,
            "target_allocations": {
                "needs_50": target_needs,
                "wants_30": target_wants,
                "savings_20": target_savings
            },
            "actual_spending": {
                "needs": actual_needs,
                "wants": actual_wants,
                "savings": actual_savings,
                "total": total_spent
            },
            "actual_percentages": {
                "needs_pct": needs_pct,
                "wants_pct": wants_pct,
                "savings_pct": savings_pct
            },
            "net_remaining": net_remaining,
            "alerts": alerts,
            "status": status
        }

    @staticmethod
    def calculate_zero_based_budget(
        monthly_income: float,
        allocations: List[Dict[str, Any]]
    ) -> Dict[str, Any]:
        """
        Phương pháp Zero-Based Budgeting (ZBB):
        Mỗi đồng thu nhập đều phải được phân bổ mục đích cụ thể: Thu nhập - Tổng phân bổ = 0.
        """
        total_allocated = sum(item.get("amount", 0.0) for item in allocations)
        difference = round(monthly_income - total_allocated, 2)

        is_zero_balanced = abs(difference) < 0.01
        if is_zero_balanced:
            message = "Ngân sách cân bằng hoàn hảo theo Zero-Based Budgeting (Mọi đồng tiền đều có mục đích)."
        elif difference > 0:
            message = f"Còn dư {difference:,.0f}đ chưa được phân bổ. Hãy đưa vào Quỹ Tiết Kiệm hoặc Trả Nợ."
        else:
            message = f"Vượt ngân sách {abs(difference):,.0f}đ! Bạn đang lên kế hoạch chi nhiều hơn thu nhập."

        return {
            "monthly_income": monthly_income,
            "total_allocated": total_allocated,
            "unallocated_balance": difference,
            "is_zero_balanced": is_zero_balanced,
            "message": message,
            "allocations_count": len(allocations)
        }

    @staticmethod
    def calculate_kakeibo_breakdown(
        expenses: List[Dict[str, Any]]
    ) -> Dict[str, Any]:
        """
        Sổ chi tiêu Kakeibo Nhật Bản:
        Phân loại chi phí thành 4 cột trụ:
        1. Needs (Sinh hoạt thiết yếu: ăn uống, nhà cửa, sức khỏe)
        2. Wants (Mong muốn cá nhân: cà phê, xem phim, mua sắm)
        3. Culture (Nuôi dưỡng tâm hồn: sách, khóa học, triển lãm)
        4. Unexpected (Phát sinh bất ngờ: sửa xe, thuốc men đột xuất)
        """
        kakeibo_map = {
            "needs": 0.0,
            "wants": 0.0,
            "culture": 0.0,
            "unexpected": 0.0
        }

        for exp in expenses:
            cat = str(exp.get("kakeibo_category", "needs")).lower()
            amt = float(exp.get("amount", 0.0))
            if cat in kakeibo_map:
                kakeibo_map[cat] += amt
            else:
                kakeibo_map["needs"] += amt

        total = sum(kakeibo_map.values())
        percentages = {
            k: round((v / total * 100), 1) if total > 0 else 0.0
            for k, v in kakeibo_map.items()
        }

        reflections = [
            "1. Bạn đã tiêu bao nhiêu tiền trong tháng này?",
            "2. Bạn thực sự muốn tiết kiệm bao nhiêu tiền?",
            "3. Khoản tiền nào thuộc nhóm 'Wants' mà bạn có thể cắt giảm vào tháng sau?",
            "4. Bạn đã đầu tư đủ cho nhóm 'Culture' để nâng cao giá trị bản thân chưa?"
        ]

        return {
            "total_expense": round(total, 2),
            "breakdown": {k: round(v, 2) for k, v in kakeibo_map.items()},
            "percentages": percentages,
            "kakeibo_reflection_questions": reflections
        }

    @staticmethod
    def calculate_daily_spending_allowance(
        remaining_flexible_budget: float,
        days_left_in_month: int
    ) -> Dict[str, Any]:
        """Tính hạn mức chi tiêu khả dụng trung bình mỗi ngày để không bị cháy túi."""
        if days_left_in_month <= 0:
            return {"daily_allowance": 0.0, "status": "MONTH_ENDED"}

        daily_allowance = round(max(0.0, remaining_flexible_budget / days_left_in_month), 2)
        return {
            "remaining_budget": remaining_flexible_budget,
            "days_left": days_left_in_month,
            "daily_allowance": daily_allowance,
            "formatted_text": f"Hạn mức an toàn mỗi ngày: {daily_allowance:,.0f}đ/ngày cho {days_left_in_month} ngày còn lại."
        }

    # Backward compatibility helper methods
    @staticmethod
    def simulate_household_fund(incomes: Dict[str, float], joint_pool_target: float, fixed_expenses: List[Dict[str, Any]], allowances: Optional[Dict[str, float]] = None):
        total_income = sum(incomes.values())
        contributions: Dict[str, float] = {}
        if total_income > 0:
            for member, income in incomes.items():
                contributions[member] = round((income / total_income) * joint_pool_target, 2)
        else:
            count = len(incomes) if incomes else 1
            for member in incomes:
                contributions[member] = round(joint_pool_target / count, 2)

        total_fixed_spent = sum(item.get("amount", 0.0) for item in fixed_expenses)
        total_allowances = sum(allowances.values()) if allowances else 0.0
        total_spent_from_pool = total_fixed_spent + total_allowances
        remaining_pool_balance = joint_pool_target - total_spent_from_pool

        return {
            "joint_pool_target": joint_pool_target,
            "member_contributions": contributions,
            "total_fixed_expenses": total_fixed_spent,
            "total_allowances": total_allowances,
            "total_spent_from_pool": total_spent_from_pool,
            "remaining_pool_balance": remaining_pool_balance,
            "is_pool_deficit": remaining_pool_balance < 0
        }

    @staticmethod
    def process_reimbursement(current_fund_balance: float, claim_amount: float, claimant: str, purpose: str):
        if current_fund_balance >= claim_amount:
            new_balance = current_fund_balance - claim_amount
            return {
                "status": "APPROVED",
                "claimant": claimant,
                "claim_amount": claim_amount,
                "purpose": purpose,
                "previous_fund_balance": current_fund_balance,
                "new_fund_balance": new_balance,
                "message": f"Hoàn ứng thành công {claim_amount:,.0f}đ cho '{claimant}' (Lý do: {purpose}). Số dư quỹ còn lại: {new_balance:,.0f}đ."
            }
        else:
            deficit = claim_amount - current_fund_balance
            return {
                "status": "PENDING_FUND_INJECTION",
                "claimant": claimant,
                "claim_amount": claim_amount,
                "purpose": purpose,
                "previous_fund_balance": current_fund_balance,
                "new_fund_balance": current_fund_balance,
                "deficit": deficit,
                "message": f"Quỹ chung không đủ số dư để hoàn ứng (Thiếu {deficit:,.0f}đ). Cần kêu gọi thành viên đóng thêm quỹ."
            }

    @staticmethod
    def calculate_roommate_utilities(room_rents: Dict[str, float], shared_utilities_total: float, occupants_per_room: Optional[Dict[str, int]] = None):
        total_occupants = sum(occupants_per_room.values()) if occupants_per_room else len(room_rents)
        utility_per_person = shared_utilities_total / total_occupants if total_occupants > 0 else 0.0
        bill_breakdown = {}
        for room, rent in room_rents.items():
            occupants = occupants_per_room.get(room, 1) if occupants_per_room else 1
            utility_share = round(utility_per_person * occupants, 2)
            bill_breakdown[room] = rent + utility_share
        return bill_breakdown

    @staticmethod
    def calculate_itemized_and_weighted_split(
        items: List[Dict[str, Any]],
        shared_service_fee: float = 0.0,
        tip_percentage: float = 0.0
    ) -> Dict[str, float]:
        """
        Chia tiền theo từng món thực tế (Itemized Split) có kèm phí dịch vụ & tip:
        - Mỗi người chỉ trả cho món mình có dùng (chia đều theo số người cùng dùng món đó).
        - Phí dịch vụ chung (shared_service_fee) được chia đều cho tất cả người tham gia.
        - Tip (tip_percentage) được nhân theo tỷ lệ tổng tiền của từng người.
        """
        individual_subtotals: Dict[str, float] = {}
        all_participants = set()

        for item in items:
            cost = float(item.get("cost", item.get("price", 0.0))) * float(item.get("quantity", 1))
            consumers = item.get("consumers", [])
            if not consumers:
                continue
            all_participants.update(consumers)
            share_per_consumer = cost / len(consumers)
            for person in consumers:
                individual_subtotals[person] = individual_subtotals.get(person, 0.0) + share_per_consumer

        total_people = len(all_participants) if all_participants else 1
        service_per_person = shared_service_fee / total_people

        final_shares: Dict[str, float] = {}
        tip_factor = 1.0 + (tip_percentage / 100.0)

        for person in all_participants:
            subtotal_with_service = individual_subtotals.get(person, 0.0) + service_per_person
            final_shares[person] = round(subtotal_with_service * tip_factor, 2)

        return final_shares


# Alias for backward compatibility
GroupSplitCasesSkill = PersonalBudgetCasesSkill
