"""
Skill: core_engine.skills.debt_cashflow_optimizer_skill
Chuyên gia: Chuyên Gia Tối Ưu Hóa Nợ & Đồ Thị Dòng Tiền Cá Nhân (Agent 06 - Personal Debt & Cashflow Optimizer)
Lĩnh vực: Chiến lược Trả nợ Snowball vs Avalanche, Đồ thị Dòng tiền Cá nhân (Cashflow DAG) & Phân tích Đa Ví
"""

from typing import Dict, Any, List, Optional
import copy


class DebtCashflowOptimizerSkill:
    """Bộ công cụ tính toán và tối ưu hóa lộ trình thanh toán nợ cá nhân và dòng tiền."""

    @staticmethod
    def calculate_snowball_schedule(
        debts: List[Dict[str, Any]],
        monthly_extra_budget: float = 0.0,
        max_months: int = 120
    ) -> Dict[str, Any]:
        """
        Chiến lược Debt Snowball:
        - Sắp xếp các khoản nợ tăng dần theo dư nợ gốc (Principal).
        - Trả số tiền tối thiểu (Min Payment) cho tất cả các khoản nợ.
        - Dồn toàn bộ ngân sách thặng dư (monthly_extra_budget) vào khoản nợ có số dư nhỏ nhất.
        - Khi khoản nợ nhỏ nhất được trả xong, cộng dồn toàn bộ số tiền trả hàng tháng của nó sang khoản nợ tiếp theo.
        - Lợi ích: Tạo động lực tâm lý mạnh mẽ nhờ các chiến thắng nhanh chóng ban đầu.
        """
        active_debts = []
        for d in debts:
            active_debts.append({
                "name": d["name"],
                "balance": float(d.get("balance", d.get("principal", 0.0))),
                "interest_rate": float(d.get("interest_rate", 0.0)),  # % / năm
                "min_payment": float(d.get("min_payment", 0.0)),
            })

        # Sắp xếp tăng dần theo balance (dư nợ nhỏ nhất lên đầu)
        active_debts.sort(key=lambda x: x["balance"])

        total_interest_paid = 0.0
        month = 0
        timeline = []
        payoff_order = []

        while month < max_months:
            remaining = [d for d in active_debts if d["balance"] > 0.01]
            if not remaining:
                break

            month += 1
            month_interest = 0.0
            month_principal = 0.0

            # 1. Tính lãi hàng tháng cho từng khoản nợ
            for d in remaining:
                monthly_rate = (d["interest_rate"] / 100.0) / 12.0
                interest = round(d["balance"] * monthly_rate, 2)
                d["balance"] += interest
                month_interest += interest
                total_interest_paid += interest

            # 2. Phân bổ tiền trả nợ: min payment cho mọi khoản
            available_extra = monthly_extra_budget
            for d in remaining:
                payment = min(d["balance"], d["min_payment"])
                d["balance"] -= payment
                month_principal += payment

            # 3. Dồn extra budget vào khoản nợ nhỏ nhất còn lại
            target_debt = remaining[0]
            extra_pay = min(target_debt["balance"], available_extra)
            target_debt["balance"] -= extra_pay
            month_principal += extra_pay

            # Kiểm tra xem có khoản nào vừa được trả dứt điểm
            for d in remaining:
                if d["balance"] <= 0.01 and d["name"] not in payoff_order:
                    payoff_order.append(d["name"])
                    # Hiệu ứng hòn tuyết lăn: min payment của khoản vừa trả hết được giải phóng dồn sang extra budget
                    available_extra += d["min_payment"]

            timeline.append({
                "month": month,
                "interest_paid": round(month_interest, 2),
                "principal_paid": round(month_principal, 2),
                "remaining_total_balance": round(sum(d["balance"] for d in remaining), 2)
            })

        return {
            "strategy": "DEBT_SNOWBALL",
            "priority_order": [d["name"] for d in active_debts],
            "description": "Ưu tiên trả khoản nợ có số dư nhỏ nhất để tạo chiến thắng tâm lý.",
            "total_months": month,
            "total_interest_paid": round(total_interest_paid, 2),
            "payoff_order": payoff_order,
            "is_debt_free": len([d for d in active_debts if d["balance"] > 0.01]) == 0,
            "timeline": timeline
        }

    @staticmethod
    def calculate_avalanche_schedule(
        debts: List[Dict[str, Any]],
        monthly_extra_budget: float = 0.0,
        max_months: int = 120
    ) -> Dict[str, Any]:
        """
        Chiến lược Debt Avalanche:
        - Sắp xếp các khoản nợ giảm dần theo lãi suất thực tế (%/năm).
        - Trả số tiền tối thiểu cho mọi khoản nợ.
        - Dồn toàn bộ ngân sách thặng dư vào khoản nợ có LÃI SUẤT CAO NHẤT (ví dụ: Nợ thẻ tín dụng 28-36%/năm).
        - Lợi ích: Tối ưu hóa toán học, giảm thiểu tối đa tổng số tiền lãi phải trả cho ngân hàng.
        """
        active_debts = []
        for d in debts:
            active_debts.append({
                "name": d["name"],
                "balance": float(d.get("balance", d.get("principal", 0.0))),
                "interest_rate": float(d.get("interest_rate", 0.0)),
                "min_payment": float(d.get("min_payment", 0.0)),
            })

        # Sắp xếp giảm dần theo lãi suất (lãi cao nhất lên đầu)
        active_debts.sort(key=lambda x: x["interest_rate"], reverse=True)

        total_interest_paid = 0.0
        month = 0
        timeline = []
        payoff_order = []

        while month < max_months:
            remaining = [d for d in active_debts if d["balance"] > 0.01]
            if not remaining:
                break

            month += 1
            month_interest = 0.0
            month_principal = 0.0

            # 1. Tính lãi hàng tháng
            for d in remaining:
                monthly_rate = (d["interest_rate"] / 100.0) / 12.0
                interest = round(d["balance"] * monthly_rate, 2)
                d["balance"] += interest
                month_interest += interest
                total_interest_paid += interest

            # 2. Trả min payment
            available_extra = monthly_extra_budget
            for d in remaining:
                payment = min(d["balance"], d["min_payment"])
                d["balance"] -= payment
                month_principal += payment

            # 3. Dồn extra vào khoản nợ lãi suất cao nhất
            target_debt = remaining[0]
            extra_pay = min(target_debt["balance"], available_extra)
            target_debt["balance"] -= extra_pay
            month_principal += extra_pay

            # Kiểm tra sạch nợ từng khoản
            for d in remaining:
                if d["balance"] <= 0.01 and d["name"] not in payoff_order:
                    payoff_order.append(d["name"])
                    available_extra += d["min_payment"]

            timeline.append({
                "month": month,
                "interest_paid": round(month_interest, 2),
                "principal_paid": round(month_principal, 2),
                "remaining_total_balance": round(sum(d["balance"] for d in remaining), 2)
            })

        return {
            "strategy": "DEBT_AVALANCHE",
            "priority_order": [d["name"] for d in active_debts],
            "description": "Ưu tiên trả khoản nợ có lãi suất cao nhất để tiết kiệm tối đa tiền lãi.",
            "total_months": month,
            "total_interest_paid": round(total_interest_paid, 2),
            "payoff_order": payoff_order,
            "is_debt_free": len([d for d in active_debts if d["balance"] > 0.01]) == 0,
            "timeline": timeline
        }

    @staticmethod
    def compare_payoff_strategies(
        debts: List[Dict[str, Any]],
        monthly_extra_budget: float = 0.0
    ) -> Dict[str, Any]:
        """So sánh trực tiếp kết quả giữa phương pháp Snowball và Avalanche."""
        snowball = DebtCashflowOptimizerSkill.calculate_snowball_schedule(debts, monthly_extra_budget)
        avalanche = DebtCashflowOptimizerSkill.calculate_avalanche_schedule(debts, monthly_extra_budget)

        interest_difference = round(snowball["total_interest_paid"] - avalanche["total_interest_paid"], 2)
        months_difference = snowball["total_months"] - avalanche["total_months"]

        recommendation = (
            f"Avalanche giúp bạn tiết kiệm {interest_difference:,.0f}đ tiền lãi. "
            f"Nếu bạn cần động lực tâm lý để xóa sổ từng khoản nợ sớm, hãy chọn Snowball."
            if interest_difference > 0 else "Cả hai phương pháp cho kết quả tương đương."
        )

        return {
            "snowball_result": {
                "total_interest": snowball["total_interest_paid"],
                "total_months": snowball["total_months"],
                "payoff_order": snowball["payoff_order"]
            },
            "avalanche_result": {
                "total_interest": avalanche["total_interest_paid"],
                "total_months": avalanche["total_months"],
                "payoff_order": avalanche["payoff_order"]
            },
            "interest_savings_with_avalanche": max(0.0, interest_difference),
            "months_savings_with_avalanche": max(0, months_difference),
            "recommendation": recommendation
        }

    # Backward compatibility methods for Tarjan group graph
    @staticmethod
    def simplify_debts(transactions: List[Dict[str, Any]]) -> Dict[str, Any]:
        net_balance: Dict[str, float] = {}
        for tx in transactions:
            amount = float(tx.get("amount", 0.0))
            if amount <= 0:
                continue
            if "payer" in tx and "debtor" in tx:
                payer = tx["payer"]
                debtor = tx["debtor"]
                net_balance[payer] = net_balance.get(payer, 0.0) + amount
                net_balance[debtor] = net_balance.get(debtor, 0.0) - amount
            elif "from" in tx and "to" in tx:
                debtor = tx["from"]
                creditor = tx["to"]
                net_balance[debtor] = net_balance.get(debtor, 0.0) - amount
                net_balance[creditor] = net_balance.get(creditor, 0.0) + amount

        creditors = []
        debtors = []
        for person, balance in net_balance.items():
            rounded_bal = round(balance, 2)
            if rounded_bal > 0.01:
                creditors.append([person, rounded_bal])
            elif rounded_bal < -0.01:
                debtors.append([person, -rounded_bal])

        creditors.sort(key=lambda x: x[1], reverse=True)
        debtors.sort(key=lambda x: x[1], reverse=True)

        simplified_transfers = []
        i, j = 0, 0
        while i < len(debtors) and j < len(creditors):
            debtor_name, debt_amt = debtors[i]
            creditor_name, cred_amt = creditors[j]
            settle_amt = min(debt_amt, cred_amt)
            if settle_amt > 0.01:
                simplified_transfers.append({
                    "from": debtor_name,
                    "to": creditor_name,
                    "amount": round(settle_amt, 2)
                })
            debtors[i][1] -= settle_amt
            creditors[j][1] -= settle_amt
            if debtors[i][1] <= 0.01:
                i += 1
            if creditors[j][1] <= 0.01:
                j += 1

        total_original = len(transactions)
        total_simplified = len(simplified_transfers)
        red_pct = round((1 - (total_simplified / total_original)) * 100, 1) if total_original > 0 else 0.0

        return {
            "net_balances": {k: round(v, 2) for k, v in net_balance.items()},
            "original_transfer_count": total_original,
            "simplified_transfer_count": total_simplified,
            "reduction_percentage": red_pct,
            "simplified_transfers": simplified_transfers,
            "algorithm": "Greedy Net Balance Flow Matching O(V log V)"
        }

    @staticmethod
    def eliminate_debt_cycles(directed_edges: List[Dict[str, Any]]) -> List[Dict[str, Any]]:
        res = DebtCashflowOptimizerSkill.simplify_debts(directed_edges)
        return res["simplified_transfers"]

    @staticmethod
    def generate_mermaid_graph(
        simplified_transfers: List[Dict[str, Any]],
        title: str = "Đồ Thị Bù Trừ Nợ"
    ) -> str:
        """Sinh mã Mermaid Markdown trực quan hóa đồ thị dòng tiền chuyển khoản tối giản."""
        if not simplified_transfers:
            return f"```mermaid\nflowchart LR\n    %% {title}\n    AllCleared[Tất cả nợ đã hòa vốn hoặc đã được tất toán]\n```"

        lines = [
            "```mermaid",
            "flowchart LR",
            f"    %% {title}",
            "    classDef debtor fill:#FEE2E2,stroke:#DC2626,stroke-width:2px;",
            "    classDef creditor fill:#DCFCE7,stroke:#16A34A,stroke-width:2px;"
        ]

        for tx in simplified_transfers:
            payer = tx["from"].replace(" ", "_")
            receiver = tx["to"].replace(" ", "_")
            amount = tx["amount"]
            lines.append(f'    {payer}["{tx["from"]}"] -->|"{amount:,.0f}đ"| {receiver}["{tx["to"]}"]')

        lines.append("```")
        return "\n".join(lines)

# Alias for backward compatibility
TarjanGraphSkill = DebtCashflowOptimizerSkill
