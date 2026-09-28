"""
Module: core_engine.council.persona_agent
Mục đích: Lớp cơ sở PersonaAgent tự động nạp System Prompt từ core/*.md và hỗ trợ các Skill chuyên biệt
"""

from pathlib import Path
from typing import Dict, Any, Optional
from core_engine.agents.base_agent import BaseAgent
from core_engine.state import ProjectState


class PersonaAgent(BaseAgent):
    def __init__(
        self,
        name: str,
        spec_file_path: str,
        llm_client,
        filesystem,
        terminal,
        git,
        default_prompt: str = ""
    ):
        self.name = name
        self.spec_file_path = spec_file_path
        loaded_prompt = self._load_prompt_from_spec(filesystem, spec_file_path, default_prompt)
        super().__init__(
            name=name,
            system_prompt=loaded_prompt,
            llm_client=llm_client,
            filesystem=filesystem,
            terminal=terminal,
            git=git,
        )


    def _load_prompt_from_spec(self, filesystem, path_str: str, default_prompt: str) -> str:
        """Đọc system prompt từ file markdown đặc tả trong core/."""
        if filesystem and hasattr(filesystem, "workspace_root"):
            spec_path = (filesystem.workspace_root / path_str).resolve()
            if spec_path.exists() and spec_path.is_file():
                try:
                    with open(spec_path, "r", encoding="utf-8", errors="replace") as f:
                        return f.read()
                except Exception:
                    pass
        return default_prompt if default_prompt else f"Bạn là {self.name} trong Hội đồng chuyên gia."

