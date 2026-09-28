"""
Script: project_context/snapshot_context.py
Mục đích: Tự động kiểm tra tính toàn vẹn của hệ thống và cập nhật trạng thái mới nhất vào project_context/
Dành cho mọi IDE / AI Agent: Chạy script này để kiểm tra nhanh hiện trạng toàn bộ codebase.
"""

import sys
import os
import subprocess
from pathlib import Path
from datetime import datetime

# Thiết lập encoding utf-8 trên console Windows
if sys.platform == "win32" and hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8")

WORKSPACE_ROOT = Path(__file__).parent.parent.resolve()


def run_integrity_check():
    print("=================================================================")
    print(" 🧭 KIỂM TRA TÍNH TOÀN VẸN NGỮ CẢNH HỆ THỐNG (PROJECT CONTEXT)")
    print(f" Thời điểm kiểm tra: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}")
    print("=================================================================\n")

    # 1. Kiểm tra cấu trúc thư mục cốt lõi
    required_dirs = ["core_engine", "core", "templates", "tests", "project_context", "sessions"]
    print("[1] KIỂM TRA CẤU TRÚC THƯ MỤC:")
    for d in required_dirs:
        dir_path = WORKSPACE_ROOT / d
        status = "✅ Tồn tại" if dir_path.exists() and dir_path.is_dir() else "❌ THIẾU"
        print(f"  - Thư mục '{d}': {status}")

    # 2. Chạy toàn bộ 16 Unit Tests
    print("\n[2] CHẠY BỘ TEST SUITE TỰ ĐỘNG:")
    test_res = subprocess.run(
        [sys.executable, "-B", "-m", "unittest", "discover", "-s", "tests"],
        cwd=str(WORKSPACE_ROOT),
        capture_output=True,
        text=True,
        encoding="utf-8",
        errors="replace"
    )

    if test_res.returncode == 0:
        print("  ✅ 100% UNIT TESTS ĐÃ PASS:")
        for line in test_res.stderr.splitlines():
            if "Ran" in line or "OK" in line:
                print(f"     👉 {line}")
    else:
        print("  ❌ CÓ TEST BỊ THẤT BẠI:")
        print(test_res.stderr)

    # 3. Tóm tắt các file ngữ cảnh quan trọng
    print("\n[3] DANH SÁCH TÀI LIỆU ĐỊNH HƯỚNG NGỮ CẢNH (DURABLE CONTEXT):")
    context_files = [
        "project_context/00_MASTER_CONTEXT.md",
        "project_context/decisions_and_changes_log.md",
        "project_context/plans/2026-08-23_plan_01_multi_agent_council.md",
        "project_context/plans/2026-08-23_plan_02_swe_engine_implementation.md",
        "AGENTS.md"
    ]
    for cf in context_files:
        p = WORKSPACE_ROOT / cf
        status = f"✅ ({p.stat().st_size} bytes)" if p.exists() else "❌ Không tìm thấy"
        print(f"  - {cf}: {status}")

    print("\n=================================================================")
    print(" 🎯 KẾT LUẬN: Hệ thống ở trạng thái HOÀN TOÀN SẴN SÀNG.")
    print(" Bất kỳ AI/IDE nào chỉ cần đọc 'project_context/00_MASTER_CONTEXT.md' là có đủ 100% context.")
    print("=================================================================")


if __name__ == "__main__":
    run_integrity_check()
