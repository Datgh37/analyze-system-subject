"""
Module: graph_debt_solver.py
Mục đích: Thuật toán Tối ưu hóa & Rút gọn Đồ thị Nợ Nhóm (Debt Simplification Graph Engine)
Áp dụng: Lý thuyết đồ thị luồng / Greedy Balance Matching
Giúp giảm số lượng giao dịch chuyển khoản từ N*(N-1)/2 xuống tối đa (N-1) giao dịch.
"""

import sys
import os
from typing import List, Dict, Tuple
from dataclasses import dataclass
from decimal import Decimal, ROUND_HALF_UP

if sys.platform == "win32" and hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8")



@dataclass
class Transaction:
    payer: str
    payee: str
    amount: float

    def __str__(self):
        return f"{self.payer} -> {self.payee}: {self.amount:,.2f} VND"


class DebtSimplificationEngine:
    @staticmethod
    def calculate_net_balances(raw_transactions: List[Transaction]) -> Dict[str, float]:
        """
        Tính số dư ròng (Net Balance) của từng thành viên.
        Net = Tổng tiền đã trả hộ - Tổng tiền nợ phải chia
        - Net > 0: Thành viên cần ĐƯỢC NHẬN TIỀN (Creditor)
        - Net < 0: Thành viên CẦN PHẢI TRẢ TIỀN (Debtor)
        - Net = 0: Đã cân bằng, không cần làm gì
        """
        balances: Dict[str, float] = {}

        for tx in raw_transactions:
            balances[tx.payer] = balances.get(tx.payer, 0.0) + tx.amount
            balances[tx.payee] = balances.get(tx.payee, 0.0) - tx.amount

        # Làm tròn để tránh sai số số thực (floating point error)
        return {
            member: round(balance, 2)
            for member, balance in balances.items()
            if abs(round(balance, 2)) > 0.01
        }

    @classmethod
    def simplify_debts(cls, raw_transactions: List[Transaction]) -> List[Transaction]:
        """
        Thuật toán Greedy Two-Pointer tối ưu đồ thị bù trừ nợ.
        Độ phức tạp: O(V log V) với V là số lượng thành viên trong nhóm.
        """
        net_balances = cls.calculate_net_balances(raw_transactions)

        # Tách thành 2 nhóm: Người cần trả (Debtors) và Người cần nhận (Creditors)
        debtors: List[Tuple[str, float]] = []   # (name, amount_to_pay)
        creditors: List[Tuple[str, float]] = [] # (name, amount_to_receive)

        for member, balance in net_balances.items():
            if balance < 0:
                debtors.append((member, -balance))
            elif balance > 0:
                creditors.append((member, balance))

        # Sắp xếp giảm dần theo số tiền để ghép cặp tối ưu (Greedy approach)
        debtors.sort(key=lambda x: x[1], reverse=True)
        creditors.sort(key=lambda x: x[1], reverse=True)

        simplified_transactions: List[Transaction] = []

        i, j = 0, 0
        while i < len(debtors) and j < len(creditors):
            debtor_name, debt_amount = debtors[i]
            creditor_name, credit_amount = creditors[j]

            # Số tiền cấn trừ bằng giá trị nhỏ nhất giữa 2 bên
            settled_amount = min(debt_amount, credit_amount)
            settled_amount = round(settled_amount, 2)

            if settled_amount > 0.01:
                simplified_transactions.append(
                    Transaction(
                        payer=debtor_name,
                        payee=creditor_name,
                        amount=settled_amount
                    )
                )

            # Cập nhật số dư còn lại
            remaining_debt = round(debt_amount - settled_amount, 2)
            remaining_credit = round(credit_amount - settled_amount, 2)

            if remaining_debt == 0:
                i += 1
            else:
                debtors[i] = (debtor_name, remaining_debt)

            if remaining_credit == 0:
                j += 1
            else:
                creditors[j] = (creditor_name, remaining_credit)

        return simplified_transactions


def demo_run():
    print("================================================================")
    print(" DEMO THUẬT TOÁN RÚT GỌN NỢ NHÓM (DEBT SIMPLIFICATION ENGINE)")
    print("================================================================")
    
    # Kịch bản thực tế: Nhóm 5 người đi du lịch Vũng Tàu với các khoản chi lộn xộn
    # A nợ B 100k, B nợ C 100k, C nợ A 100k (Vòng tròn 3 người)
    # D nợ E 200k, B trả tiền ăn hộ D 300k, E trả tiền phòng hộ C 500k
    raw_txs = [
        # Chu kỳ nợ vòng tròn A -> B -> C -> A
        Transaction("An", "Binh", 100_000),
        Transaction("Binh", "Cuong", 100_000),
        Transaction("Cuong", "An", 100_000),
        # Các khoản chi tiêu thực tế khác
        Transaction("Dung", "Em", 200_000),
        Transaction("Dung", "Binh", 300_000),
        Transaction("Cuong", "Em", 500_000),
    ]

    print(f"\n[1] DANH SÁCH GIAO DỊCH GỐC ({len(raw_txs)} giao dịch):")
    for tx in raw_txs:
        print(f"  - {tx}")

    net_balances = DebtSimplificationEngine.calculate_net_balances(raw_txs)
    print(f"\n[2] SỐ DƯ RÒNG TỪNG THÀNH VIÊN (Net Balances):")
    for member, net in net_balances.items():
        role = "Cần nhận (+)" if net > 0 else "Cần trả (-)"
        print(f"  - {member:8s}: {net:+12,.2f} VND ({role})")

    simplified_txs = DebtSimplificationEngine.simplify_debts(raw_txs)
    print(f"\n[3] GIAO DỊCH SAU KHI RÚT GỌN BẰNG ĐỒ THỊ ({len(simplified_txs)} giao dịch):")
    for tx in simplified_txs:
        print(f"  👉 {tx}")

    print(f"\n=> KẾT QUẢ: Đã giảm từ {len(raw_txs)} giao dịch xuống còn {len(simplified_txs)} giao dịch!")
    print("   Vòng tròn nợ An -> Binh -> Cuong -> An đã được triệt tiêu hoàn toàn về 0đ.")
    print("================================================================")


if __name__ == "__main__":
    demo_run()
