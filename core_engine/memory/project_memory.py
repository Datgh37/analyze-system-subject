"""
Module: core_engine.memory.project_memory
Mục đích: Lưu trữ ngữ cảnh bền vững (Project Memory & Architecture Specs)
Tuân thủ nguyên tắc Claude & Spec: Durable Context, Coding Conventions, Database Schemas
"""

import json
from dataclasses import dataclass, field
from typing import Dict, Any, List, Optional
from pathlib import Path


@dataclass
class ProjectMemory:
    project_name: str = "Smart Expense & Group Budget System"
    architecture_style: str = "Clean Architecture / DDD"
    tech_stack: Dict[str, str] = field(default_factory=lambda: {
        "backend": "Python / FastAPI",
        "database": "PostgreSQL (3NF)",
        "cache_locking": "Redis",
        "async_worker": "Redis Streams / RabbitMQ"
    })
    business_rules: List[str] = field(default_factory=lambda: [
        "Mọi giao dịch tài chính nhóm phải bọc trong Transaction (ACID)",
        "Sử dụng thuật toán Min-Cost Flow để triệt tiêu nợ vòng tròn nhóm",
        "Mọi khoản chi tiêu cá nhân đều được tính toán Spending Velocity hàng ngày"
    ])
    coding_conventions: List[str] = field(default_factory=lambda: [
        "Sử dụng Type Annotations cho toàn bộ functions",
        "Không dùng hardcoded secrets trong code",
        "Giữ nguyên tắc KISS và YAGNI"
    ])

    def to_dict(self) -> Dict[str, Any]:
        return {
            "project_name": self.project_name,
            "architecture_style": self.architecture_style,
            "tech_stack": self.tech_stack,
            "business_rules": self.business_rules,
            "coding_conventions": self.coding_conventions,
        }

    def format_for_prompt(self) -> str:
        """Định dạng context ngắn gọn để nạp vào prompt cho các Agent."""
        return (
            f"=== PROJECT MEMORY ({self.project_name}) ===\n"
            f"Kiến trúc: {self.architecture_style}\n"
            f"Tech Stack: {json.dumps(self.tech_stack, ensure_ascii=False)}\n"
            f"Quy tắc nghiệp vụ: {', '.join(self.business_rules[:2])}\n"
            f"Quy ước code: {', '.join(self.coding_conventions[:2])}\n"
        )
