#!/usr/bin/env python3
"""
CLI Runner Chính Thức: run_council_session.py
Khởi chạy và điều phối Hội Đồng Đa Tác Tử cho Đồ Án Phân Tích Thiết Kế Hệ Thống Thông Tin.

Sử dụng:
    # 1. Chạy phiên mới với Mock Provider (Mô phỏng nhanh & Offline)
    python run_council_session.py --feature "Thiết Lập Ngân Sách 50/30/20 & Hạn Mức Ngày" --provider mock

    # 2. Chạy phiên mới với OpenAI (GPT-4o)
    python run_council_session.py --feature "Bóc Tách Hóa Đơn Mua Sắm OCR & Gán Nhãn Kakeibo" --provider openai

    # 3. Chạy phiên mới với Google Gemini (Miễn phí API)
    python run_council_session.py --feature "Tối Ưu Hóa Nợ Thẻ Tín Dụng (Snowball vs Avalanche)" --provider gemini

    # 4. Thẩm định tính toàn vẹn và chất lượng của một phiên
    python run_council_session.py --audit sessions/session_01_household_fund

    # 5. Xem danh sách tất cả các phiên bàn thảo
    python run_council_session.py --list
"""

import sys
import os
import argparse
import re
from pathlib import Path
from datetime import datetime
from typing import Optional, List, Dict, Any

# Cấu hình UTF-8 an toàn cho Windows Terminal
if sys.platform == "win32":
    try:
        sys.stdout.reconfigure(encoding="utf-8")
    except Exception:
        pass

# Đảm bảo import đúng core_engine
sys.path.insert(0, str(Path(__file__).parent))

from core_engine.tools.filesystem import FilesystemTools
from core_engine.tools.terminal import TerminalTools
from core_engine.tools.git_tools import GitTools
from core_engine.llm_client import create_llm_client, MockLLMClient
from core_engine.council.council_orchestrator_agent import CouncilOrchestratorAgent
from core_engine.skills.orchestrator_rfc_skill import OrchestratorRFCSkill


def print_banner():
    print("=" * 80)
    print("[COUNCIL] HOI DONG DA TAC TU (MULTI-AGENT COUNCIL) - FULL-STACK PTTKHTTT & DATN")
    print("          Dieu phoi 13 Chuyen Gia Danh Nhan Kiet Xuat theo Chu Trinh 4 Pha")
    print("=" * 80)



def sanitize_folder_name(name: str) -> str:
    slug = re.sub(r"[^\w\s-]", "", name.lower())
    return "session_" + re.sub(r"[-\s]+", "_", slug).strip("_")


def generate_mock_expert_responses(task_description: str):
    return [
        {
            "content": (
                f"### [INSIGHT TÂM LÝ & NUDGE — Richard Thaler]\n"
                f"- Đề bài: {task_description}\n"
                f"- UX Friction: Thao tác cần tối giản < 3 bước để tránh người dùng nản lòng bỏ cuộc.\n"
                f"- Spending Velocity: Cần có thanh hiển thị tốc độ tiêu tiền trực quan thay vì chỉ hiện con số khô khan.\n"
                f"- Nudge Message: Gửi thông báo khích lệ khi quỹ chung đạt mục tiêu tiết kiệm."
            ),
            "tool_calls": []
        },
        {
            "content": (
                f"### [INSIGHT QUẢN TRỊ TÀI CHÍNH TẬP THỂ — Personal Budget Coach]\n"
                f"- Phân bổ dòng tiền theo tỷ lệ thu nhập thực tế (ví dụ: Vợ 40% - Chồng 60%).\n"
                f"- Thiết lập cơ chế Hoàn Ứng (Reimbursement) tự động khi thành viên mua sắm đồ dùng chung.\n"
                f"- Tách bạch quỹ chi tiêu cố định (tiền nhà, điện nước) và quỹ linh hoạt (tiêu vặt, khẩn cấp)."
            ),
            "tool_calls": []
        },
        {
            "content": (
                f"# ĐẶC TẢ GIAO DIỆN & ATOMIC DESIGN SYSTEM — Brad Frost & Don Norman\n\n"
                f"## 1. Design Tokens: Primary `#2563EB`, Success `#16A34A`, Warning `#D97706`, Danger `#DC2626`\n"
                f"## 2. Atomic Components: Atoms (`AmountDisplay`), Molecules (`ExpenseItemRow`), Organisms (`JointPoolCard`)\n"
                f"## 3. WCAG Accessibility: Đạt chuẩn WCAG 2.1 AA (Contrast Ratio 15.3:1)"
            ),
            "tool_calls": []
        },
        {
            "content": (
                f"# ĐẶC TẢ YÊU CẦU PHẦN MỀM CHUẨN IEEE 29148 — Alistair Cockburn & Debra Paul\n\n"
                f"## 1. Thông Tin Ca Sử Dụng\n"
                f"- **Mã UC:** UC-HOUSEHOLD-01\n"
                f"- **Tên UC:** {task_description}\n"
                f"- **Tác nhân chính:** Thành viên gia đình / Nhóm\n"
                f"- **Mục tiêu:** Quản lý dòng tiền và chia sẻ chi phí minh bạch.\n\n"
                f"## 2. Luồng Sự Kiện Chính (Happy Path)\n"
                f"1. Người dùng mở ứng dụng và chọn chức năng Quản Trị Quỹ.\n"
                f"2. Hệ thống hiển thị số dư quỹ và tỷ lệ đóng góp của từng thành viên.\n"
                f"3. Người dùng nhập số tiền đóng góp và xác nhận giao dịch.\n"
                f"4. Hệ thống cập nhật số dư tức thời và gửi thông báo tới các thành viên.\n\n"
                f"## 3. Luồng Ngoại Lệ (Exception Flows)\n"
                f"- **3a. Số dư tài khoản không đủ:** Hệ thống thông báo lỗi và gợi ý đóng góp một phần.\n"
                f"- **4a. Mất kết nối CSDL:** Hệ thống rollback giao dịch và lưu trạng thái PENDING_RETRY.\n\n"
                f"## 4. Tiêu Chí Nghiệm Thu BDD (Gherkin Scenarios)\n"
                f"```gherkin\n"
                f"Feature: {task_description}\n"
                f"  Scenario: Đóng quỹ thành công\n"
                f"    Given Quỹ chung đang cần thu 10,000,000đ\n"
                f"    When Chồng nộp vào 6,000,000đ theo tỷ lệ 60%\n"
                f"    Then Số dư quỹ tăng thêm 6,000,000đ và trạng thái ghi nhận COMPLETED\n"
                f"```"
            ),
            "tool_calls": []
        },
        {
            "content": (
                f"# QUYẾT ĐỊNH KIẾN TRÚC: ADR-01 — CLEAN ARCHITECTURE & DOMAIN-DRIVEN DESIGN (Eric Evans)\n\n"
                f"## 1. Trạng Thái: ACCEPTED\n"
                f"## 2. Bối Cảnh: Cần bảo vệ logic tính toán tài chính độc lập hoàn toàn khỏi Framework và CSDL.\n"
                f"## 3. Quyết Định:\n"
                f"- Áp dụng Clean Architecture 4 lớp: Domain, Application, Infrastructure, Presentation.\n"
                f"- Sử dụng Value Object bất biến `Money(amount: Decimal, currency: 'VND')` triệt tiêu lỗi floating-point.\n"
                f"## 4. Hệ Quả & Đánh Đổi (Trade-offs & Consequences):\n"
                f"- **Ưu điểm:** Code sạch, dễ dàng test độc lập với 100% Mock, an toàn tuyệt đối về mặt tiền tệ.\n"
                f"- **Đánh đổi:** Cần viết thêm các lớp Mapper chuyển đổi giữa Entity và DTO."
            ),
            "tool_calls": []
        },
        {
            "content": (
                f"# ĐẶC TẢ CSDL CHUẨN 3NF & POLYGLOT PERSISTENCE (Martin Kleppmann)\n\n"
                f"## 1. Thiết Kế Bảng PostgreSQL Chuẩn 3NF\n"
                f"- Bảng `joint_pool_funds` (id, name, balance DECIMAL(15,2), created_at)\n"
                f"- Bảng `expenses` (id, fund_id, payer_id, amount DECIMAL(15,2), category_id, receipt_jsonb)\n"
                f"## 2. Khóa Tranh Chấp (Concurrency Lock)\n"
                f"- Áp dụng **Pessimistic Locking (`SELECT balance FROM joint_pool_funds FOR UPDATE`)** để chống Lost Update khi nhiều người cùng rút tiền quỹ.\n"
                f"## 3. Dữ Liệu Đa Hình (Polyglot):\n"
                f"- PostgreSQL JSONB lưu chi tiết các món hóa đơn OCR.\n"
                f"- Redis In-Memory lưu Khóa phân tán Redlock và Cache tốc độ chi tiêu."
            ),
            "tool_calls": []
        },
        {
            "content": (
                f"# GIẢI THUẬT ĐỒ THỊ RÚT GỌN NỢ NHÓM O(V log V) (Robert Tarjan & Jon Kleinberg)\n\n"
                f"## 1. Nguyên Lý Giải Thuật\n"
                f"- Tính Net Balance cho từng thành viên.\n"
                f"- Áp dụng Two-Pointer Greedy Matching trên 2 tập Creditors và Debtors.\n"
                f"- Giảm số giao dịch từ N*(N-1)/2 xuống tối đa N - 1 giao dịch và triệt tiêu chu trình nợ lặp.\n"
                f"## 2. Sơ Đồ Chuyển Khoản Tối Giản (Mermaid Flowchart)\n"
                f"```mermaid\n"
                f"flowchart LR\n"
                f"    Chồng[\"Chồng\"] -->|\"6,000,000đ\"| QuyChung[\"Quỹ Chung Gia Đình\"]\n"
                f"    Vợ[\"Vợ\"] -->|\"4,000,000đ\"| QuyChung[\"Quỹ Chung Gia Đình\"]\n"
                f"```"
            ),
            "tool_calls": []
        },
        {
            "content": (
                f"# KIẾN TRÚC BẤT ĐỒNG BỘ MESSAGE QUEUE & IDEMPOTENCY (Dr. Werner Vogels — CTO Amazon)\n\n"
                f"## 1. Xử Lý Bất Đồng Bộ (Non-blocking Queue)\n"
                f"- Endpoint upload hóa đơn OCR trả về ngay **HTTP 202 Accepted** kèm `job_id` UUID.\n"
                f"- Tác vụ bóc tách AI được đẩy vào Redis Streams để Background Worker xử lý ngầm.\n"
                f"## 2. Tính Bất Biến (Idempotency & Deduplication)\n"
                f"- Kiểm tra `Idempotency-Key` kết hợp mã băm SHA-256 Payload để chặn đứng 100% request gửi trùng lặp khi người dùng bấm nhiều lần.\n"
                f"## 3. Đóng Gói Docker Compose\n"
                f"- Cung cấp cấu hình Docker Compose hoàn chỉnh kèm Health Checks `GET /health`."
            ),
            "tool_calls": []
        },
        {
            "content": (
                f"# PIPELINE BÓC TÁCH HÓA ĐƠN MULTIMODAL AI & DATA-CENTRIC OCR (Prof. Andrew Ng)\n\n"
                f"## 1. Pipeline 3 Giai Đoạn\n"
                f"1. Tiền xử lý ảnh: Deskew (chỉnh góc nghiêng), khử nhiễu văn bản.\n"
                f"2. Vision LLM: Bóc tách cấu trúc theo chuẩn Pydantic JSON Schema.\n"
                f"3. Hậu kiểm toán học (Mathematical Audit): Đối soát tổng tiền món với tổng bill.\n"
                f"## 2. Cơ Chế Giám Sát Người Dùng (Human-in-the-Loop)\n"
                f"- Nếu phát hiện chênh lệch số học > 0đ, hệ thống hạ `confidence_score < 0.85` và kích hoạt giao diện cảnh báo để người dùng xác nhận thủ công."
            ),
            "tool_calls": []
        },
        {
            "content": (
                f"# KIẾN TRÚC CLIENT STATE & OPTIMISTIC UI 0MS — Dan Abramov & Christopher Chedeau\n\n"
                f"## 1. Global State Store: Unidirectional Slices (Auth, JointFund, Expense, DebtGraph)\n"
                f"## 2. Optimistic UI: Cập nhật tức thì vào Store 0ms kèm Idempotency-Key và tự động Rollback khi mất mạng\n"
                f"## 3. Offline-First: Hàng đợi SQLite / MMKV giải quyết xung đột bằng Last-Write-Wins + Vector Clock"
            ),
            "tool_calls": []
        },
        {
            "content": (
                f"# AN NINH BẢO MẬT FINTECH & STRIDE THREAT MODEL — Bruce Schneier & OWASP\n\n"
                f"## 1. STRIDE Analysis: Phòng chống Spoofing, Tampering, IDOR và DoS toàn diện\n"
                f"## 2. Mã hóa dữ liệu tài chính: AES-256-GCM (IV 12-byte + Auth Tag 16-byte) cho số tài khoản và số dư\n"
                f"## 3. Xác thực an toàn: JWT Token Rotation (Access 15p + Refresh 7d httpOnly Cookie) kèm Redis Revocation"
            ),
            "tool_calls": []
        },
        {
            "content": (
                f"# KẾ HOẠCH KIỂM THỬ TỰ ĐỘNG & QA STRATEGY — Kent Beck & Martin Fowler\n\n"
                f"## 1. Kim Tự Tháp Kiểm Thử: 70% Unit, 20% Integration, 10% E2E (Health Score: 95/100)\n"
                f"## 2. Playwright E2E: Tự động hóa toàn bộ luồng Người dùng đăng nhập -> Tạo Quỹ -> Chia Nợ -> Rút gọn đồ thị\n"
                f"## 3. k6 Load Testing: 500 VUs tải cao điểm, đảm bảo SLA p95 < 200ms"
            ),
            "tool_calls": []
        },
        {
            "content": (
                f"# PHÊ DUYỆT TỔNG THỂ FINAL RFC — Council Orchestrator\n"
                f"- Đánh giá: Cả 3 Pha và toàn bộ 12 Chuyên gia Full-Stack đều đạt chuẩn chất lượng cao nhất.\n"
                f"- Kết luận: Bản Đặc tả Final RFC được PHÊ DUYỆT (APPROVED). Sẵn sàng chuyển giao cho đội ngũ lập trình viên phát triển."
            ),
            "tool_calls": []
        }
    ]



def run_session(feature_name: str, provider: str = "mock", custom_dir: Optional[str] = None):
    print_banner()
    print(f"[*] TINH NANG: {feature_name}")
    print(f"[*] LLM PROVIDER: {provider.upper()}")
    print(f"[*] THOI DIEM KHOI CHAY: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}\n")

    workspace_dir = Path(__file__).parent
    session_dir = Path(custom_dir) if custom_dir else workspace_dir / "sessions" / sanitize_folder_name(feature_name)
    session_dir.mkdir(parents=True, exist_ok=True)

    # 1. Khởi tạo LLM Client
    if provider.lower() == "mock":
        mock_responses = generate_mock_expert_responses(feature_name)
        llm = create_llm_client(provider="mock", mock_responses=mock_responses)
    else:
        llm = create_llm_client(provider=provider)

    fs = FilesystemTools(workspace_root=str(workspace_dir))
    term = TerminalTools(workspace_root=str(workspace_dir))
    git = GitTools(workspace_root=str(workspace_dir))

    orchestrator = CouncilOrchestratorAgent(llm, fs, term, git)

    print(">>> Bat dau dieu phoi chu trinh 4 pha...")
    print("   [Pha 1/4] Richard Thaler & Personal Budget Coach: Dang phan tich tam ly & dong tien...")
    print("   [Pha 2/4] Alistair Cockburn: Dang chuan hoa Use Case IEEE 29148 & Gherkin...")
    print("   [Pha 3/4] Nhom 5 Kien truc su: Dang thiet ke Clean Arch, CSDL 3NF, Do thi no & AI OCR...")
    print("   [Pha 4/4] Council Orchestrator: Dang doi soat tinh nhat quan & Xuat ban Final RFC...\n")

    state = orchestrator.run_full_session(
        session_dir_path=str(session_dir),
        task_description=feature_name
    )

    # 2. Thẩm định chất lượng & tính toàn vẹn
    integrity = OrchestratorRFCSkill.verify_session_integrity(str(session_dir))
    quality = OrchestratorRFCSkill.verify_content_quality_gates(str(session_dir))

    print("=" * 80)
    print("[SUCCESS] PHIEN BAN THAO HOAN TAT XUAT SAC!")
    rel_path = session_dir.relative_to(workspace_dir) if session_dir.is_relative_to(workspace_dir) else session_dir
    print(f"Thu muc luu tru: {rel_path}")
    print(f"Diem toan ven cau truc: {integrity['integrity_score']}/100 - {integrity['verdict']}")
    print(f"Diem chat luong noi dung: {quality['quality_score']}/100 - {quality['verdict']}")
    print("=" * 80)
    print("\nDanh muc tai lieu da sinh thanh cong:")
    print(f"   1. [01_user_feedback.md] ({session_dir / '01_user_feedback.md'})")
    print(f"   2. [02_use_case_specs.md] ({session_dir / '02_use_case_specs.md'})")
    print(f"   3. [03_technical_adr.md]  ({session_dir / '03_technical_adr.md'})")
    print(f"   4. [04_final_rfc.md]      ({session_dir / '04_final_rfc.md'})")
    print("\nTai lieu 04_final_rfc.md da san sang chuyen giao cho lap trinh vien phat trien!\n")


def audit_session(session_path_str: str):
    print_banner()
    session_path = Path(session_path_str)
    print(f"[AUDIT] DANG THAM DINH PHIEN: {session_path}\n")

    if not session_path.exists():
        print(f"[ERROR] Loi: Thu muc phien '{session_path}' khong ton tai!")
        return

    integrity = OrchestratorRFCSkill.verify_session_integrity(str(session_path))
    quality = OrchestratorRFCSkill.verify_content_quality_gates(str(session_path))

    print(f"DIEM CAU TRUC: {integrity['integrity_score']}/100")
    print(f"   - Trang thai: {integrity['verdict']}")
    if integrity["missing_files"]:
        print(f"   - Tep bi thieu: {integrity['missing_files']}")
    if integrity["empty_files"]:
        print(f"   - Tep bi rong: {integrity['empty_files']}")

    print(f"\nDIEM NOI DUNG CHUYEN MON: {quality['quality_score']}/100")
    print(f"   - Trang thai: {quality['verdict']}")
    print("   - Tieu chi dat chuan:")
    for cp in quality["checks_passed"]:
        print(f"     [OK] {cp}")
    if quality["violations"]:
        print("   - Can bo sung them:")
        for v in quality["violations"]:
            print(f"     [WARN] {v}")


def list_sessions():
    print_banner()
    workspace_dir = Path(__file__).parent
    sessions_dir = workspace_dir / "sessions"
    print(f"[LIST] DANH SACH CAC PHIEN BAN THAO TRONG: sessions/\n")

    if not sessions_dir.exists():
        print("Chua co phien nao duoc khoi tao.")
        return

    sessions = [d for d in sessions_dir.iterdir() if d.is_dir()]
    if not sessions:
        print("Chua co phien nao duoc khoi tao.")
        return

    print("| STT | Ten Thu Muc Phien | Diem Cau Truc | Diem Noi Dung | Trang Thai |")
    print("| :--- | :--- | :---: | :---: | :--- |")
    for idx, s in enumerate(sessions, 1):
        integ = OrchestratorRFCSkill.verify_session_integrity(str(s))
        qual = OrchestratorRFCSkill.verify_content_quality_gates(str(s))
        status = "[OK] HOAN HAO" if (integ["is_valid"] and qual["is_passed"]) else "[WARN] CAN HOAN THIEN"
        print(f"| {idx:02d} | `{s.name}` | {integ['integrity_score']}/100 | {qual['quality_score']}/100 | {status} |")



def main():
    parser = argparse.ArgumentParser(description="Multi-Agent Council Session Runner CLI")
    parser.add_argument("--feature", type=str, help="Tên tính năng cần phân tích thiết kế")
    parser.add_argument("--provider", type=str, default="mock", choices=["mock", "openai", "gemini", "ollama"], help="LLM Provider (mặc định: mock)")
    parser.add_argument("--dir", type=str, help="Đường dẫn thư mục phiên tùy chỉnh")
    parser.add_argument("--audit", type=str, help="Thẩm định chất lượng một phiên bàn thảo cụ thể")
    parser.add_argument("--list", action="store_true", help="Liệt kê tất cả các phiên bàn thảo hiện có")

    args = parser.parse_args()

    if args.list:
        list_sessions()
    elif args.audit:
        audit_session(args.audit)
    elif args.feature:
        run_session(args.feature, provider=args.provider, custom_dir=args.dir)
    else:
        parser.print_help()


if __name__ == "__main__":
    main()
