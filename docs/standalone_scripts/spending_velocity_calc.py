"""
Module: spending_velocity_calc.py
Mục đích: Tính toán Tốc độ Chi tiêu (Spending Velocity) & Cảnh báo Sớm (Micro-Budget Alert)
Áp dụng: Thống kê chuỗi thời gian đơn giản (Time-Series Extrapolation)
"""

import sys
from dataclasses import dataclass
from typing import Optional

if sys.platform == "win32" and hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8")



@dataclass
class BudgetStatus:
    category: str
    monthly_budget: float
    spent_so_far: float
    current_day: int
    total_days_in_month: int

    @property
    def remaining_budget(self) -> float:
        return self.monthly_budget - self.spent_so_far

    @property
    def daily_burn_rate(self) -> float:
        """Tốc độ tiêu tiền trung bình mỗi ngày (VND/ngày)"""
        if self.current_day <= 0:
            return 0.0
        return self.spent_so_far / self.current_day

    @property
    def projected_total_spend(self) -> float:
        """Dự báo tổng chi tiêu cả tháng nếu giữ nguyên tốc độ này"""
        return self.daily_burn_rate * self.total_days_in_month

    @property
    def estimated_depletion_day(self) -> Optional[int]:
        """Dự báo ngày sẽ cạn sạch ngân sách trong tháng"""
        if self.daily_burn_rate <= 0:
            return None
        depletion_day = int(self.monthly_budget / self.daily_burn_rate)
        return depletion_day if depletion_day <= self.total_days_in_month else None

    def generate_alert(self) -> str:
        depletion_day = self.estimated_depletion_day
        if depletion_day and depletion_day < self.total_days_in_month:
            overspend_amount = self.projected_total_spend - self.monthly_budget
            return (
                f"⚠️ [CẢNH BÁO NGUY CƠ VỠ NGÂN SÁCH - {self.category.upper()}]\n"
                f"  - Hạn mức tháng: {self.monthly_budget:,.0f} VND\n"
                f"  - Đã tiêu (ngày {self.current_day}/{self.total_days_in_month}): {self.spent_so_far:,.0f} VND ({self.spent_so_far/self.monthly_budget*100:.1f}%)\n"
                f"  - Tốc độ chi: {self.daily_burn_rate:,.0f} VND/ngày\n"
                f"  👉 DỰ BÁO: Với tốc độ này, bạn sẽ HẾT TIỀN VÀO NGÀY {depletion_day} của tháng!\n"
                f"  👉 Dự kiến vượt hạn mức: {overspend_amount:+,.0f} VND vào cuối tháng.\n"
                f"  💡 Lời khuyên: Giảm mức chi xuống dưới {(self.remaining_budget / (self.total_days_in_month - self.current_day)):,.0f} VND/ngày cho các ngày còn lại."
            )
        else:
            return (
                f"✅ [NGÂN SÁCH AN TOÀN - {self.category.upper()}]\n"
                f"  - Đã tiêu: {self.spent_so_far:,.0f} / {self.monthly_budget:,.0f} VND\n"
                f"  - Tốc độ an toàn ({self.daily_burn_rate:,.0f} VND/ngày). Dự kiến cuối tháng còn dư {self.monthly_budget - self.projected_total_spend:,.0f} VND."
            )


if __name__ == "__main__":
    # Test case: Người dùng đặt ngân sách ăn uống 5,000,000 VND / tháng (30 ngày)
    # Mới đến ngày 12 nhưng đã tiêu hết 3,200,000 VND
    status = BudgetStatus(
        category="Ăn uống",
        monthly_budget=5_000_000,
        spent_so_far=3_200_000,
        current_day=12,
        total_days_in_month=30
    )
    print(status.generate_alert())
