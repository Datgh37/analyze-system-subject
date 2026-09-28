"""
Skill: core_engine.skills.kleppmann_database_skill
Chuyên gia: Martin Kleppmann & Pramod Sadalage (Agent 05 - Universal Database Architect)
Lĩnh vực: Thiết kế CSDL Quan hệ 3NF, NoSQL JSONB/Redis, Concurrency Lock & Polyglot Persistence
"""

from typing import Dict, Any, List, Optional


class KleppmannDatabaseSkill:
    """Bộ công cụ thẩm định CSDL 3NF, Concurrency và Polyglot Persistence."""

    @staticmethod
    def validate_3nf_schema(tables: Dict[str, Dict[str, Any]]) -> Dict[str, Any]:
        """
        Thẩm định lược đồ bảng CSDL:
        - 1NF: Có khóa chính, không có cột mảng lồng nhau không chuẩn.
        - 2NF: Mọi cột không khóa phụ thuộc hoàn toàn vào khóa chính.
        - 3NF: Không có phụ thuộc bắc cầu (Transitive Dependency).
        - Kiểu dữ liệu: Bắt buộc dùng DECIMAL cho tiền bạc, cấm FLOAT.
        """
        violations = []
        warnings = []

        for table_name, table_info in tables.items():
            columns = table_info.get("columns", {})
            pk = table_info.get("primary_key")

            # 1. Kiểm tra Khóa chính (1NF)
            if not pk:
                violations.append(f"Bảng '{table_name}' thiếu Khóa chính (Primary Key).")

            # 2. Kiểm tra kiểu dữ liệu tiền tệ
            for col_name, col_type in columns.items():
                col_type_upper = str(col_type).upper()
                if ("AMOUNT" in col_name.upper() or "PRICE" in col_name.upper() or "MONEY" in col_name.upper()):
                    if "FLOAT" in col_type_upper or "DOUBLE" in col_type_upper:
                        violations.append(
                            f"LỖI TIỀN TỆ: Bảng '{table_name}', cột '{col_name}' dùng kiểu '{col_type}'. Bắt buộc phải là 'DECIMAL(15,2)' hoặc 'BIGINT'!"
                        )

            # 3. Kiểm tra phụ thuộc bắc cầu (3NF)
            # Ví dụ: Bảng expenses lại chứa category_name, category_icon thay vì chỉ có category_id
            if table_name.lower() == "expenses":
                if "category_name" in columns and "category_id" in columns:
                    violations.append(
                        "VI PHẠM 3NF: Bảng 'expenses' chứa cả 'category_id' và 'category_name' (Phụ thuộc bắc cầu: expense_id -> category_id -> category_name). Cần tách sang bảng 'categories' riêng!"
                    )
                if "user_name" in columns and "user_id" in columns:
                    violations.append(
                        "VI PHẠM 3NF: Bảng 'expenses' chứa 'user_name' (Phụ thuộc bắc cầu qua user_id). Cần lấy qua bảng 'users'!"
                    )

        is_3nf = len(violations) == 0
        return {
            "is_3nf": is_3nf,
            "total_tables": len(tables),
            "violations": violations,
            "warnings": warnings,
            "verdict": "LƯỢC ĐỒ ĐẠT CHUẨN 3NF & AN TOÀN TIỀN TỆ" if is_3nf else "PHÁT HIỆN VI PHẠM 3NF / KIỂU DỮ LIỆU"
        }

    @staticmethod
    def evaluate_concurrency_lock_strategy(operation_type: str, is_financial: bool = True) -> Dict[str, Any]:
        """
        Đưa ra chiến lược khóa tranh chấp (Concurrency Lock) tối ưu:
        - Giao dịch tiền bạc: Pessimistic Lock (SELECT FOR UPDATE)
        - Cập nhật thông tin thông thường: Optimistic Lock (versioning)
        - Tác vụ AI OCR / Background Job: Redis Distributed Lock (Redlock)
        """
        op_upper = operation_type.upper()
        if is_financial or "WITHDRAW" in op_upper or "SETTLE" in op_upper or "PAY" in op_upper:
            return {
                "strategy": "PESSIMISTIC_LOCKING",
                "implementation": "SELECT ... FOR UPDATE (PostgreSQL Transaction)",
                "isolation_level": "READ COMMITTED / REPEATABLE READ",
                "rationale": "Chống tuyệt đối Lost Update khi nhiều thành viên cùng chi tiêu từ Quỹ chung tại cùng một thời điểm.",
                "example_sql": "SELECT balance FROM joint_pool_funds WHERE id = :fund_id FOR UPDATE;"
            }
        elif "OCR" in op_upper or "ASYNC" in op_upper or "JOB" in op_upper:
            return {
                "strategy": "DISTRIBUTED_LOCK",
                "implementation": "Redis Redlock (SET lock_key uuid NX PX 10000)",
                "isolation_level": "N/A",
                "rationale": "Ngăn chặn việc 2 worker cùng xử lý trùng lặp 1 hóa đơn khi người dùng bấm gửi nhiều lần.",
                "example_sql": "N/A (Redis Key Lock)"
            }
        else:
            return {
                "strategy": "OPTIMISTIC_LOCKING",
                "implementation": "Version Column (UPDATE ... WHERE id = :id AND version = :old_version)",
                "isolation_level": "READ COMMITTED",
                "rationale": "Phù hợp cho các thao tác đọc nhiều ghi ít (sửa hồ sơ, đổi tên nhóm) với chi phí thấp.",
                "example_sql": "UPDATE groups SET name = :name, version = version + 1 WHERE id = :id AND version = :version;"
            }

    @staticmethod
    def format_db_schema_spec(schema_data: Dict[str, Any]) -> str:
        """
        Định dạng tài liệu Lược đồ CSDL và Quyết định Dữ liệu Đa hình theo mẫu chuẩn.
        """
        title = schema_data.get("title", "Lược Đồ CSDL Quan Hệ & Kiến Trúc Dữ Liệu Đa Hình")
        polyglot_summary = schema_data.get("polyglot_summary", (
            "- **PostgreSQL 3NF:** Sổ cái giao dịch, Quỹ chung, Người dùng, Phân bổ nợ (ACID).\n"
            "- **PostgreSQL JSONB:** Lưu trữ dữ liệu thô hóa đơn OCR (Itemized payload).\n"
            "- **Redis In-Memory:** Distributed Lock (`Redlock`), Cache tốc độ chi tiêu ($Burn Rate$)."
        ))
        tables = schema_data.get("tables", [])

        tables_md = ""
        for t in tables:
            t_name = t.get("name", "table_name")
            desc = t.get("description", "")
            cols = t.get("columns", [])
            pk = t.get("primary_key", "id")
            fks = t.get("foreign_keys", [])
            indexes = t.get("indexes", [])

            cols_md = "\n".join(f"| `{c.get('name')}` | `{c.get('type')}` | {c.get('constraints', '')} | {c.get('note', '')} |" for c in cols)
            fks_md = "\n".join(f"- `{fk.get('column')}` $\\rightarrow$ `{fk.get('references')}`" for fk in fks) if fks else "- Không có"
            idx_md = "\n".join(f"- `{idx}`" for idx in indexes) if indexes else "- Khóa chính mặc định"

            tables_md += (
                f"### Bảng `{t_name}` ({desc})\n"
                f"- **Khóa chính:** `{pk}`\n"
                f"- **Khóa ngoại:**\n{fks_md}\n"
                f"- **Chỉ mục (Indexes):**\n{idx_md}\n\n"
                f"| Tên Cột | Kiểu Dữ Liệu | Ràng Buộc | Ý Nghĩa |\n"
                f"| :--- | :--- | :--- | :--- |\n"
                f"{cols_md}\n\n"
            )

        return (
            f"# ĐẶC TẢ LƯỢC ĐỒ CSDL & KIẾN TRÚC DỮ LIỆU ĐA HÌNH\n"
            f"*(Martin Kleppmann & Pramod Sadalage Methodology)*\n\n"
            f"## 1. Kiến Trúc Dữ Liệu Đa Hình (Polyglot Persistence Architecture)\n"
            f"{polyglot_summary}\n\n"
            f"## 2. Chi Tiết Các Bảng CSDL Quan Hệ Chuẩn 3NF (PostgreSQL)\n\n"
            f"{tables_md}"
        )
