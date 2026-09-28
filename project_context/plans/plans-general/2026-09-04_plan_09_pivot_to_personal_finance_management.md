# Kế Hoạch Triển Khai: PLAN-09 — Tái Định Hướng Toàn Diện Sang Quản Lý Tài Chính Cá Nhân (PFM)

> **Mã kế hoạch:** `PLAN-09`  
> **Thời điểm lập:** `2026-09-04`  
> **Trạng thái:** `COMPLETED (100% OK)`

---

## 1. Mục Tiêu Kế Hoạch
- Chuyển đổi toàn bộ phạm vi hệ thống từ Quản lý Chi tiêu Cá nhân & Nhóm sang Quản lý Chi tiêu & Hoạch định Tài chính Cá nhân Thông minh (Tích hợp AI).
- Tái cấu trúc Agent 02 thành Chuyên gia Hoạch định Ngân sách Cá nhân (50/30/20, ZBB, Kakeibo).
- Tái cấu trúc Agent 06 thành Chuyên gia Tối ưu hóa Nợ & Dòng tiền Cá nhân (Debt Snowball vs Avalanche).
- Chuẩn hóa CSDL 3NF Đa ví, Giao dịch, Hạn mức, Mục tiêu tiết kiệm, Hóa đơn định kỳ.
- Xây dựng 4 bộ Benchmark Sessions cá nhân thực tế đạt 100/100 điểm toàn vẹn và 0 vi phạm chất lượng.
- Bảo đảm 100% Unit Tests Passed.

---

## 2. Kết Quả Bàn Giao
1. Tệp Persona mới: `core/02_personal_budget_coach.md` và `core/06_personal_debt_cashflow_optimizer.md`.
2. Python Skills mới: `personal_budget_cases_skill.py` và `debt_cashflow_optimizer_skill.py`.
3. Agent Classes mới: `PersonalBudgetAgent` và `DebtOptimizerAgent`.
4. Customization Skills: `council-personal-budget-coach` và `council-personal-debt-optimizer`.
5. 4 Benchmark Sessions: `session_01_personal_budget_50_30_20`, `session_02_debt_snowball_vs_avalanche`, `session_03_receipt_itemized_categorization`, `session_04_spending_velocity_subscription_trap`.
