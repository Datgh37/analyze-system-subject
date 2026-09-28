"""
Skill: core_engine.skills.evans_domain_skill
Chuyên gia: Eric Evans (Agent 04 - Cha đẻ Domain-Driven Design & Clean Architecture)
Lĩnh vực: Domain Modeling, Bounded Contexts, Money Value Object bất biến & Clean Architecture ADR
"""

from decimal import Decimal, ROUND_HALF_UP
from dataclasses import dataclass
from typing import Dict, Any, List, Optional


@dataclass(frozen=True)
class Money:
    """
    Value Object Tiền Tệ bất biến (Immutable Money Value Object).
    Sử dụng kiểu Decimal để triệt tiêu hoàn toàn sai số dấu phẩy động (Floating Point Bug).
    """
    amount: Decimal
    currency: str = "VND"

    def __post_init__(self):
        if not isinstance(self.amount, Decimal):
            object.__setattr__(self, "amount", Decimal(str(self.amount)))
        if not isinstance(self.currency, str) or len(self.currency) != 3:
            raise ValueError("Đơn vị tiền tệ phải là mã ISO 3 ký tự (ví dụ: 'VND', 'USD').")

    def add(self, other: "Money") -> "Money":
        if self.currency != other.currency:
            raise ValueError(f"Không thể cộng 2 loại tiền tệ khác nhau: {self.currency} và {other.currency}")
        return Money(self.amount + other.amount, self.currency)

    def subtract(self, other: "Money") -> "Money":
        if self.currency != other.currency:
            raise ValueError(f"Không thể trừ 2 loại tiền tệ khác nhau: {self.currency} và {other.currency}")
        return Money(self.amount - other.amount, self.currency)

    def multiply(self, factor: float | int | Decimal) -> "Money":
        dec_factor = Decimal(str(factor))
        return Money((self.amount * dec_factor).quantize(Decimal("0.01"), rounding=ROUND_HALF_UP), self.currency)

    def split_evenly(self, parts: int) -> List["Money"]:
        """
        Chia đều một số tiền cho N phần mà KHÔNG bị thất thoát phần dư (Pennies/Dongs preservation).
        Ví dụ: 100,000đ chia cho 3 người -> [33,333.34, 33,333.33, 33,333.33] (Tổng = 100,000đ)
        """
        if parts <= 0:
            raise ValueError("Số phần chia phải lớn hơn 0.")
        
        base_amount = (self.amount / Decimal(parts)).quantize(Decimal("0.01"))
        remainder = self.amount - (base_amount * Decimal(parts))

        results = []
        for i in range(parts):
            extra = Decimal("0.01") if i < int(remainder * Decimal("100")) else Decimal("0")
            results.append(Money(base_amount + extra, self.currency))
        return results

    def __str__(self) -> str:
        return f"{self.amount:,.2f} {self.currency}"


class EvansDomainSkill:
    """Bộ công cụ thẩm định Kiến trúc Domain-Driven Design & Clean Architecture."""

    @staticmethod
    def create_money(amount: float | int | str | Decimal, currency: str = "VND") -> Money:
        return Money(Decimal(str(amount)), currency)

    @staticmethod
    def validate_bounded_contexts(contexts: Dict[str, List[str]]) -> Dict[str, Any]:
        """
        Thẩm định tính độc lập của các Bounded Contexts.
        Phát hiện phụ thuộc vòng lặp hoặc rò rỉ ranh giới nghiệp vụ.
        """
        required_contexts = ["ExpenseContext", "GroupSettlementContext", "BudgetingContext", "IdentityContext"]
        missing = [c for c in required_contexts if c not in contexts]

        dependencies = {}
        circular = []
        for ctx, deps in contexts.items():
            dependencies[ctx] = deps
            for dep in deps:
                if dep in contexts and ctx in contexts[dep]:
                    circular.append(f"{ctx} <---> {dep}")

        is_valid = len(missing) == 0 and len(circular) == 0
        return {
            "is_valid": is_valid,
            "total_contexts": len(contexts),
            "missing_contexts": missing,
            "circular_dependencies": circular,
            "verdict": "KIẾN TRÚC BOUNDED CONTEXT HỢP LỆ" if is_valid else "CÓ XUNG ĐỘT RANH GIỚI NGHIỆP VỤ"
        }

    @staticmethod
    def format_clean_architecture_adr(adr_data: Dict[str, Any]) -> str:
        """
        Định dạng bản Quyết định Kiến trúc Clean Architecture theo mẫu ADR chuẩn.
        """
        adr_id = adr_data.get("id", "ADR-01")
        title = adr_data.get("title", "Áp Dụng Clean Architecture & DDD")
        status = adr_data.get("status", "ACCEPTED")
        context = adr_data.get("context", "Hệ thống cần phân tách rõ ranh giới nghiệp vụ và hạ tầng.")
        decision = adr_data.get("decision", "Áp dụng Clean Architecture 4 lớp: Domain, Application, Infrastructure, Presentation.")
        consequences = adr_data.get("consequences", "Tăng khả năng test độc lập, dễ dàng thay thế database hoặc framework.")
        layers = adr_data.get("layers", [
            "1. Domain Layer (Entities, Value Objects, Aggregates) - 100% Thuần khiết",
            "2. Application Layer (Use Case Interactors, DTOs, Ports)",
            "3. Infrastructure Layer (Database Repositories, OCR Adapters, Redis Locks)",
            "4. Presentation Layer (REST Controllers, CLI, Mobile UI)"
        ])

        layers_md = "\n".join(f"- {l}" for l in layers)

        return (
            f"# QUYẾT ĐỊNH KIẾN TRÚC: {adr_id} — {title.upper()}\n"
            f"*(Eric Evans & Robert C. Martin Methodology)*\n\n"
            f"## 1. Trạng Thái (Status)\n"
            f"**`{status}`**\n\n"
            f"## 2. Bối Cảnh Nghiệp Vụ (Context)\n"
            f"{context}\n\n"
            f"## 3. Quyết Định Kiến Trúc (Decision)\n"
            f"{decision}\n\n"
            f"### Phân Tách Các Lớp Clean Architecture:\n{layers_md}\n\n"
            f"## 4. Hệ Quả & Đánh Đổi (Consequences)\n"
            f"{consequences}\n"
        )
