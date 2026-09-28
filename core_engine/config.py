"""
Module: core_engine.config
Mục đích: Cấu hình tập trung cho toàn bộ hệ thống AI Software Engineer
"""

import os
from dataclasses import dataclass


@dataclass
class EngineConfig:
    # Model cấu hình (hỗ trợ OpenAI, Anthropic, Gemini hoặc Mock LLM)
    provider: str = os.getenv("LLM_PROVIDER", "gemini")
    model_name: str = os.getenv("LLM_MODEL", "gemini-2.5-pro")
    api_key: str = os.getenv("LLM_API_KEY", "")
    temperature: float = 0.1

    # Cấu hình kiểm soát vòng lặp sửa lỗi (Claude Principle: Bounded Iteration)
    max_repair_iterations: int = 5
    command_timeout_seconds: int = 45
    
    # Workspace root an toàn
    workspace_root: str = os.getcwd()


# Khởi tạo singleton config mặc định
default_config = EngineConfig()
