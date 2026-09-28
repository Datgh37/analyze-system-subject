"""
Skill: core_engine.skills.vogels_async_skill
Chuyên gia: Dr. Werner Vogels (Agent 07 - CTO Amazon — Async & DevOps Specialist)
Lĩnh vực: Kiến trúc Bất đồng bộ Message Queue, Idempotency & Docker Containerization
"""

import uuid
import hashlib
from typing import Dict, Any, List, Optional


class VogelsAsyncSkill:
    """Bộ công cụ kiến trúc bất đồng bộ và kiểm tra tính bất biến Idempotency."""

    @staticmethod
    def calculate_payload_hash(payload: Dict[str, Any]) -> str:
        """Tính toán mã băm SHA-256 của payload để đối soát nội dung request."""
        payload_str = str(sorted(payload.items()))
        return hashlib.sha256(payload_str.encode("utf-8")).hexdigest()

    @staticmethod
    def validate_idempotency(
        cache_store: Dict[str, Any],
        idempotency_key: str,
        payload: Dict[str, Any]
    ) -> Dict[str, Any]:
        """
        Kiểm tra tính bất biến (Idempotency) để chống trùng lặp giao dịch (Deduplication):
        - Nếu khóa đã tồn tại -> Chặn xử lý lần 2 và trả về kết quả đã cache.
        - Nếu khóa mới -> Đăng ký vào cache và cho phép xử lý tiếp.
        """
        payload_hash = VogelsAsyncSkill.calculate_payload_hash(payload)

        if idempotency_key in cache_store:
            cached_entry = cache_store[idempotency_key]
            # Kiểm tra xem payload có bị thay đổi không
            if cached_entry.get("payload_hash") != payload_hash:
                return {
                    "is_duplicate": True,
                    "is_conflict": True,
                    "message": "CẢNH BÁO XUNG ĐỘT: Cùng Idempotency-Key nhưng nội dung payload khác nhau!"
                }
            return {
                "is_duplicate": True,
                "is_conflict": False,
                "cached_response": cached_entry.get("response", {}),
                "message": "Phát hiện request gửi lặp lại. Trả về kết quả đã lưu mà không xử lý lại."
            }

        # Đăng ký khóa mới vào cache
        cache_store[idempotency_key] = {
            "payload_hash": payload_hash,
            "status": "PROCESSING",
            "response": {"status": "SUCCESS", "message": "Giao dịch đã được ghi nhận an toàn."}
        }

        return {
            "is_duplicate": False,
            "is_conflict": False,
            "idempotency_key": idempotency_key,
            "message": "Khóa mới hợp lệ. Cho phép tiến hành giao dịch."
        }

    @staticmethod
    def simulate_async_job_submission(task_name: str, payload: Dict[str, Any]) -> Dict[str, Any]:
        """
        Giả lập cơ chế nạp tác vụ nặng (AI OCR, Xuất báo cáo) vào Hàng chờ (Message Queue):
        - Không chặn luồng HTTP.
        - Trả về ngay phản hồi chuẩn HTTP 202 Accepted kèm job_id dạng UUID.
        """
        job_id = str(uuid.uuid4())
        return {
            "http_status": 202,
            "job_id": job_id,
            "task_name": task_name,
            "status": "QUEUED",
            "poll_endpoint": f"/api/v1/jobs/{job_id}/status",
            "estimated_wait_seconds": 3,
            "message": f"Tác vụ '{task_name}' đã được đẩy vào Message Queue thành công. Client hãy dùng job_id để kiểm tra tiến độ."
        }

    @staticmethod
    def format_async_devops_adr(adr_data: Dict[str, Any]) -> str:
        """
        Định dạng bản Quyết định Kiến trúc Bất đồng bộ & DevOps theo mẫu ADR chuẩn.
        """
        adr_id = adr_data.get("id", "ADR-03")
        title = adr_data.get("title", "Kiến Trúc Bất Đồng Bộ Message Queue & Idempotency")
        status = adr_data.get("status", "ACCEPTED")
        context = adr_data.get("context", "Các tác vụ AI OCR mất 3-8 giây gây nghẽn luồng HTTP nếu xử lý đồng bộ.")
        decision = adr_data.get("decision", (
            "1. Áp dụng Message Queue (Redis Streams / RabbitMQ) cho tác vụ bóc tách OCR.\n"
            "2. Áp dụng Idempotency Key (SHA-256 Payload Hash) cho toàn bộ API giao dịch.\n"
            "3. Đóng gói toàn bộ hệ thống bằng Docker Compose kèm Health Checks."
        ))
        consequences = adr_data.get("consequences", "Hệ thống đáp ứng hàng ngàn request đồng thời mà không bị treo API chính.")

        return (
            f"# QUYẾT ĐỊNH KIẾN TRÚC: {adr_id} — {title.upper()}\n"
            f"*(Dr. Werner Vogels — Amazon CTO Methodology)*\n\n"
            f"## 1. Trạng Thái (Status)\n"
            f"**`{status}`**\n\n"
            f"## 2. Bối Cảnh (Context)\n"
            f"{context}\n\n"
            f"## 3. Quyết Định Kỹ Thuật (Decision)\n"
            f"{decision}\n\n"
            f"## 4. Hệ Quả & Đánh Đổi (Consequences)\n"
            f"{consequences}\n"
        )
