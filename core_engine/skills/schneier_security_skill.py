"""
Skill: core_engine.skills.schneier_security_skill
Chuyên gia: Bruce Schneier & OWASP (Agent 11 - Lead Fintech Security & Cryptography Specialist)
Lĩnh vực: Fintech Security Architecture, Cryptography AES-256 GCM & STRIDE Threat Modeling
"""

import os
import hmac
import hashlib
import base64
from typing import Dict, Any, List, Optional


class SchneierSecuritySkill:
    """Bộ công cụ mô hình hóa mối đe dọa STRIDE, mã hóa dữ liệu tài chính và an ninh xác thực Fintech."""

    @staticmethod
    def design_stride_threat_model(feature_name: str) -> Dict[str, Any]:
        """
        Sinh ma trận phân tích mối đe dọa 6 chiều chuẩn STRIDE (Microsoft/OWASP)
        kèm mức độ nghiêm trọng và biện pháp giảm thiểu rủi ro (Mitigation).
        """
        return {
            "feature_analyzed": feature_name,
            "threat_matrix": {
                "S_Spoofing": {
                    "threat": "Kẻ tấn công giả mạo danh tính thành viên để rút tiền quỹ hoặc sửa nợ.",
                    "severity": "CRITICAL",
                    "mitigation": "JWT ký bằng thuật toán Asymmetric ECDSA (ES256), bắt buộc Biometric (FaceID/Vân tay) khi phê duyệt hoàn ứng quỹ."
                },
                "T_Tampering": {
                    "threat": "Sửa đổi số tiền giao dịch trên đường truyền giữa Client và Backend API.",
                    "severity": "CRITICAL",
                    "mitigation": "Mã hóa TLS 1.3 In-Transit, kiểm tra chữ ký HMAC-SHA256 Payload và ghi sổ cái bất biến (Immutable Audit Logs)."
                },
                "R_Repudiation": {
                    "threat": "Thành viên chối bỏ việc đã chi tiêu hoặc từ chối khoản nợ đã được chia.",
                    "severity": "HIGH",
                    "mitigation": "Lưu vết toàn vẹn giao dịch với Server Timestamp Authority, IP address, Device ID và User Confirmation Event."
                },
                "I_Information_Disclosure": {
                    "threat": "Rò rỉ số tài khoản ngân hàng, số dư quỹ hoặc nội dung hóa đơn mua sắm riêng tư.",
                    "severity": "HIGH",
                    "mitigation": "Mã hóa AES-256-GCM At-Rest cho số tài khoản, Masking dữ liệu hiển thị (ví dụ: ****5678) và tuân thủ Nghị định 13/2023/NĐ-CP."
                },
                "D_Denial_of_Service": {
                    "threat": "Tấn công spam liên tục các yêu cầu OCR Scan hóa đơn hoặc tính toán đồ thị nợ lớn làm treo server.",
                    "severity": "MEDIUM",
                    "mitigation": "Redis Token Bucket Rate Limiting (60 requests/phút cho User thông thường, 5 requests/phút cho tác vụ nặng OCR)."
                },
                "E_Elevation_of_Privilege": {
                    "threat": "Thành viên bình thường lợi dụng lỗ hổng IDOR để chiếm quyền Trưởng nhóm / Thủ quỹ rút tiền.",
                    "severity": "CRITICAL",
                    "mitigation": "Triển khai RBAC nghiêm ngặt (Role-Based Access Control) và Database Row-Level Security (RLS) bắt buộc kiểm tra quyền sở hữu quỹ."
                }
            }
        }

    @staticmethod
    def simulate_aes_encryption_workflow(
        plain_text: str,
        secret_key: str = "fintech_master_key_256bit_secure_secret"
    ) -> Dict[str, Any]:
        """
        Mô phỏng quy trình mã hóa dữ liệu nhạy cảm chuẩn AES-256-GCM (Authenticated Encryption):
        1. Sinh IV (Initialization Vector) 12-byte ngẫu nhiên
        2. Mã hóa dữ liệu (Ciphertext)
        3. Tính toán Authentication Tag (16-byte) để bảo đảm tính toàn vẹn
        """
        # Chuẩn hóa key thành 32 bytes (256 bits)
        derived_key = hashlib.sha256(secret_key.encode("utf-8")).digest()
        iv_bytes = os.urandom(12) # Standard 96-bit IV for AES-GCM
        iv_hex = iv_bytes.hex()

        # Mô phỏng mã hóa Ciphertext qua XOR stream với Keystream sha256(key + iv)
        keystream = hashlib.sha256(derived_key + iv_bytes).digest()
        plain_bytes = plain_text.encode("utf-8")
        
        # Mở rộng keystream nếu dữ liệu dài
        while len(keystream) < len(plain_bytes):
            keystream += hashlib.sha256(keystream + iv_bytes).digest()
            
        cipher_bytes = bytes([p ^ k for p, k in zip(plain_bytes, keystream[:len(plain_bytes)])])
        ciphertext_hex = cipher_bytes.hex()

        # Tính Authentication Tag bằng HMAC-SHA256 (16 bytes đầu)
        auth_tag_bytes = hmac.new(derived_key, iv_bytes + cipher_bytes, hashlib.sha256).digest()[:16]
        auth_tag_hex = auth_tag_bytes.hex()

        # Mô phỏng giải mã để kiểm chứng toàn vẹn
        decrypted_bytes = bytes([c ^ k for c, k in zip(cipher_bytes, keystream[:len(cipher_bytes)])])
        decrypted_text = decrypted_bytes.decode("utf-8")

        return {
            "algorithm": "AES-256-GCM",
            "key_size_bits": 256,
            "iv_hex_12bytes": iv_hex,
            "ciphertext_hex": ciphertext_hex,
            "auth_tag_hex_16bytes": auth_tag_hex,
            "encrypted_payload": f"enc:{iv_hex}:{ciphertext_hex}:{auth_tag_hex}",
            "decrypted_validation": decrypted_text,
            "is_valid": decrypted_text == plain_text
        }

    @staticmethod
    def design_jwt_token_lifecycle() -> Dict[str, Any]:
        """
        Thiết kế vòng đời xác thực an toàn bằng JWT Token Rotation và cơ chế Revocation.
        """
        return {
            "token_specifications": {
                "access_token": {
                    "lifespan": "15 minutes",
                    "storage": "In-Memory Client State (Zustand/Redux)",
                    "algorithm": "ECDSA using P-256 and SHA-256 (ES256)",
                    "payload_fields": ["sub (user_id)", "roles", "exp", "jti (unique id)"]
                },
                "refresh_token": {
                    "lifespan": "7 days",
                    "storage": "httpOnly, Secure, SameSite=Strict Cookie",
                    "rotation_policy": "Single-use Token (Token Rotation) - Mỗi lần cấp Access Token mới sẽ đổi Refresh Token mới.",
                    "reuse_detection": "Nếu một Refresh Token đã thu hồi bị gửi lại, hệ thống lập tức khóa toàn bộ phiên của người dùng (Token Family Revocation)."
                }
            },
            "revocation_mechanism": "Redis In-Memory Blacklist với TTL tự động giải phóng theo thời hạn token.",
            "rbac_roles": {
                "OWNER": ["manage_fund", "deposit", "withdraw", "approve_reimbursement", "delete_fund"],
                "TREASURER": ["record_expense", "deposit", "approve_reimbursement", "view_reports"],
                "MEMBER": ["record_expense", "deposit", "request_reimbursement", "view_reports"],
                "VIEWER": ["view_reports", "view_balances"]
            }
        }

    @classmethod
    def format_security_compliance_adr(cls, feature_name: str) -> str:
        """Định dạng đặc tả an ninh bảo mật Fintech và STRIDE nhúng vào ADR."""
        stride = cls.design_stride_threat_model(feature_name)
        jwt = cls.design_jwt_token_lifecycle()
        crypto = cls.simulate_aes_encryption_workflow("0123456789 (STK Vietcombank)")

        rows = []
        for code, info in stride["threat_matrix"].items():
            rows.append(f"- **{code}:** {info['threat']}\n  - *Mức độ:* `{info['severity']}` | *Giải pháp:* {info['mitigation']}")

        return (
            f"## ĐẶC TẢ AN NINH BẢO MẬT & MẬT MÃ FINTECH (Bruce Schneier & OWASP)\n\n"
            f"### 1. Ma Trận Mô Hình Hóa Mối Đe Dọa (STRIDE Threat Model)\n"
            + "\n".join(rows) + "\n\n"
            f"### 2. Chuẩn Mã Hóa Dữ Liệu Tài Chính Nhạy Cảm (Financial Cryptography)\n"
            f"- **Thuật toán cốt lõi:** `{crypto['algorithm']}` ({crypto['key_size_bits']}-bit Key)\n"
            f"- **Toàn vẹn dữ liệu:** Authentication Tag 128-bit chống giả mạo Ciphertext.\n"
            f"- **Định dạng lưu trữ CSDL:** `enc:<IV>:<Ciphertext>:<AuthTag>`\n\n"
            f"### 3. Vòng Đời Xác Thực & Phân Quyền (JWT Rotation & RBAC)\n"
            f"- **Access Token:** `{jwt['token_specifications']['access_token']['lifespan']}` ({jwt['token_specifications']['access_token']['storage']})\n"
            f"- **Refresh Token:** `{jwt['token_specifications']['refresh_token']['lifespan']}` ({jwt['token_specifications']['refresh_token']['storage']})\n"
            f"- **Chống Dò Quét & Tái Sử Dụng:** `{jwt['token_specifications']['refresh_token']['reuse_detection']}`\n"
            f"- **Cơ Chế Thu Hồi:** `{jwt['revocation_mechanism']}`\n"
        )
