"""
Skill: core_engine.skills.andrew_ng_ai_skill
Chuyên gia: Prof. Andrew Ng (Agent 08 - AI Pioneer — Multimodal AI Specialist)
Lĩnh vực: Pipeline Bóc tách Hóa đơn Multimodal Vision AI, Data-Centric AI & Mathematical Audit
"""

from typing import Dict, Any, List, Optional


class AndrewNgAISkill:
    """Bộ công cụ kiểm toán AI hóa đơn, JSON Schema và Data-Centric AI Pipeline."""

    @staticmethod
    def validate_receipt_math_audit(receipt_payload: Dict[str, Any]) -> Dict[str, Any]:
        """
        Hậu kiểm toán học (Mathematical Audit) cho dữ liệu hóa đơn do Vision AI trích xuất:
        - Tính tổng tiền các mặt hàng: items_sum = sum(quantity * unit_price)
        - Đối soát với total_amount trên hóa đơn: calculated_total == reported_total
        - Chấm điểm tin cậy (confidence_score) và đưa ra cảnh báo Human-in-the-Loop nếu sai lệch.
        """
        items = receipt_payload.get("items", [])
        tax_amount = float(receipt_payload.get("tax_amount", 0.0))
        reported_total = float(receipt_payload.get("total_amount", 0.0))

        items_sum = 0.0
        item_audits = []

        for item in items:
            name = item.get("name", "Mặt hàng")
            qty = float(item.get("quantity", 1))
            unit_price = float(item.get("unit_price", 0.0))
            item_total = float(item.get("total_price", qty * unit_price))

            expected_item_total = round(qty * unit_price, 2)
            has_item_error = abs(item_total - expected_item_total) > 0.01

            item_audits.append({
                "name": name,
                "quantity": qty,
                "unit_price": unit_price,
                "total_price": item_total,
                "is_valid": not has_item_error
            })
            items_sum += item_total

        calculated_total = round(items_sum + tax_amount, 2)
        difference = round(abs(calculated_total - reported_total), 2)

        is_math_valid = difference <= 0.01
        
        # Tính confidence score
        if is_math_valid:
            confidence_score = 1.0
        elif reported_total > 0:
            error_ratio = min(difference / reported_total, 1.0)
            confidence_score = round(max(0.0, 1.0 - error_ratio), 2)
        else:
            confidence_score = 0.0

        needs_human_review = (not is_math_valid) or (confidence_score < 0.85)

        return {
            "is_math_valid": is_math_valid,
            "reported_total": reported_total,
            "calculated_total": calculated_total,
            "tax_amount": tax_amount,
            "difference": difference,
            "confidence_score": confidence_score,
            "needs_human_review": needs_human_review,
            "item_audits": item_audits,
            "recommendation": (
                "Hóa đơn hợp lệ 100%. Cho phép ghi tự động vào CSDL."
                if not needs_human_review else
                f"CẢNH BÁO SAI LỆCH SỐ HỌC ({difference:,.0f}đ): Kích hoạt giao diện Human-in-the-Loop để người dùng xác nhận lại."
            )
        }

    @staticmethod
    def generate_pydantic_receipt_schema() -> Dict[str, Any]:
        """
        Xuất JSON Schema chuẩn cho các mô hình Vision LLM (Gemini 1.5 Pro / GPT-4o).
        """
        return {
            "$schema": "http://json-schema.org/draft-07/schema#",
            "title": "ReceiptExtractionSchema",
            "type": "object",
            "properties": {
                "merchant_name": {"type": "string", "description": "Tên cửa hàng / nhà hàng"},
                "date": {"type": "string", "format": "date", "description": "Ngày in hóa đơn YYYY-MM-DD"},
                "total_amount": {"type": "number", "description": "Tổng số tiền thanh toán cuối cùng"},
                "tax_amount": {"type": "number", "description": "Tiền thuế VAT (nếu có, mặc định 0)"},
                "items": {
                    "type": "array",
                    "items": {
                        "type": "object",
                        "properties": {
                            "name": {"type": "string", "description": "Tên món ăn / sản phẩm"},
                            "quantity": {"type": "number", "description": "Số lượng"},
                            "unit_price": {"type": "number", "description": "Đơn giá"},
                            "total_price": {"type": "number", "description": "Thành tiền (quantity * unit_price)"}
                        },
                        "required": ["name", "quantity", "unit_price", "total_price"]
                    }
                }
            },
            "required": ["merchant_name", "total_amount", "items"]
        }

    @staticmethod
    def format_ai_pipeline_adr(ai_data: Dict[str, Any]) -> str:
        """
        Định dạng bản Quyết định Kiến trúc AI Pipeline theo mẫu ADR chuẩn.
        """
        adr_id = ai_data.get("id", "ADR-05")
        title = ai_data.get("title", "Pipeline Bóc Tách Hóa Đơn Multimodal AI & Data-Centric Audit")
        status = ai_data.get("status", "ACCEPTED")
        context = ai_data.get("context", "Hệ thống cần tự động trích xuất các mặt hàng từ ảnh hóa đơn để chia tiền nhóm.")
        decision = ai_data.get("decision", (
            "1. Áp dụng Vision LLM với Pydantic JSON Schema bắt buộc.\n"
            "2. Áp dụng Hậu kiểm toán học (Mathematical Audit) phát hiện sai lệch số tiền.\n"
            "3. Kích hoạt cơ chế Human-in-the-Loop khi confidence_score < 0.85."
        ))
        consequences = ai_data.get("consequences", "Đảm bảo độ chính xác tài chính 100%, không bao giờ để AI ghi nhận nhầm tiền.")

        return (
            f"# QUYẾT ĐỊNH KIẾN TRÚC: {adr_id} — {title.upper()}\n"
            f"*(Prof. Andrew Ng — Data-Centric AI Methodology)*\n\n"
            f"## 1. Trạng Thái (Status)\n"
            f"**`{status}`**\n\n"
            f"## 2. Bối Cảnh (Context)\n"
            f"{context}\n\n"
            f"## 3. Quyết Định Kỹ Thuật (Decision)\n"
            f"{decision}\n\n"
            f"## 4. Hệ Quả & Đánh Đổi (Consequences)\n"
            f"{consequences}\n"
        )
