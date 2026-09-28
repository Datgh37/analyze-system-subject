"""
Module: core_engine.llm_client
Mục đích: Adapter kết nối LLM đa nền tảng (Mock, OpenAI, Google Gemini, Ollama Local)
Xử lý Function/Tool Calling và giao tiếp API chuẩn xác.
"""

import os
import json
import urllib.request
import urllib.error
from typing import List, Dict, Any, Optional, Callable


class BaseLLMClient:
    def call(self, messages: List[Dict[str, str]], tools: Optional[List[Dict[str, Any]]] = None) -> Dict[str, Any]:
        raise NotImplementedError


class MockLLMClient(BaseLLMClient):
    """
    Mock LLM phục vụ kiểm thử đơn vị và benchmark offline.
    Trả về câu trả lời giả lập siêu tốc không tốn chi phí API và không cần internet.
    """
    def __init__(self, responses: Optional[List[Dict[str, Any]]] = None):
        self.responses = responses or []
        self.call_index = 0

    def add_response(self, response: Dict[str, Any]):
        self.responses.append(response)

    def call(self, messages: List[Dict[str, str]], tools: Optional[List[Dict[str, Any]]] = None) -> Dict[str, Any]:
        if self.call_index < len(self.responses):
            res = self.responses[self.call_index]
            self.call_index += 1
            return res
        return {
            "content": "[MOCK_COMPLETE] Hoàn tất phản hồi từ kịch bản kiểm thử giả lập.",
            "tool_calls": []
        }


class OpenAILLMClient(BaseLLMClient):
    """
    Adapter kết nối OpenAI API (GPT-4o, GPT-4o-mini).
    Sử dụng urllib chuẩn của Python, không bắt buộc cài thư viện bên ngoài.
    """
    def __init__(self, api_key: Optional[str] = None, model_name: str = "gpt-4o-mini", base_url: str = "https://api.openai.com/v1"):
        self.api_key = api_key or os.getenv("OPENAI_API_KEY", "")
        self.model_name = model_name
        self.base_url = base_url.rstrip("/")

    def call(self, messages: List[Dict[str, str]], tools: Optional[List[Dict[str, Any]]] = None) -> Dict[str, Any]:
        if not self.api_key:
            return {"content": "[LỖI]: Chưa cung cấp OPENAI_API_KEY.", "tool_calls": []}

        url = f"{self.base_url}/chat/completions"
        headers = {
            "Content-Type": "application/json",
            "Authorization": f"Bearer {self.api_key}"
        }
        payload: Dict[str, Any] = {
            "model": self.model_name,
            "messages": messages,
            "temperature": 0.2
        }
        if tools:
            payload["tools"] = tools

        try:
            req = urllib.request.Request(url, data=json.dumps(payload).encode("utf-8"), headers=headers, method="POST")
            with urllib.request.urlopen(req, timeout=60) as resp:
                data = json.loads(resp.read().decode("utf-8"))
                choice = data["choices"][0]["message"]
                return {
                    "content": choice.get("content", ""),
                    "tool_calls": choice.get("tool_calls", [])
                }
        except Exception as e:
            return {"content": f"[LỖI KẾT NỐI OPENAI]: {str(e)}", "tool_calls": []}


class GeminiLLMClient(BaseLLMClient):
    """
    Adapter kết nối Google Gemini API (Gemini 1.5 Pro / Flash).
    """
    def __init__(self, api_key: Optional[str] = None, model_name: str = "gemini-1.5-flash"):
        self.api_key = api_key or os.getenv("GEMINI_API_KEY", "")
        self.model_name = model_name

    def call(self, messages: List[Dict[str, str]], tools: Optional[List[Dict[str, Any]]] = None) -> Dict[str, Any]:
        if not self.api_key:
            return {"content": "[LỖI]: Chưa cung cấp GEMINI_API_KEY.", "tool_calls": []}

        url = f"https://generativelanguage.googleapis.com/v1beta/models/{self.model_name}:generateContent?key={self.api_key}"
        contents = []
        for m in messages:
            role = "user" if m.get("role") in ["user", "system"] else "model"
            contents.append({"role": role, "parts": [{"text": m.get("content", "")}]})

        payload = {"contents": contents}
        headers = {"Content-Type": "application/json"}

        try:
            req = urllib.request.Request(url, data=json.dumps(payload).encode("utf-8"), headers=headers, method="POST")
            with urllib.request.urlopen(req, timeout=60) as resp:
                data = json.loads(resp.read().decode("utf-8"))
                text = data["candidates"][0]["content"]["parts"][0]["text"]
                return {"content": text, "tool_calls": []}
        except Exception as e:
            return {"content": f"[LỖI KẾT NỐI GEMINI]: {str(e)}", "tool_calls": []}


class OllamaLLMClient(BaseLLMClient):
    """
    Adapter kết nối Ollama chạy Local trên máy (Hoàn toàn miễn phí & Offline).
    Ví dụ: Llama 3, DeepSeek-R1.
    """
    def __init__(self, base_url: str = "http://localhost:11434", model_name: str = "llama3"):
        self.base_url = base_url.rstrip("/")
        self.model_name = model_name

    def call(self, messages: List[Dict[str, str]], tools: Optional[List[Dict[str, Any]]] = None) -> Dict[str, Any]:
        url = f"{self.base_url}/v1/chat/completions"
        headers = {"Content-Type": "application/json"}
        payload = {
            "model": self.model_name,
            "messages": messages,
            "temperature": 0.2
        }

        try:
            req = urllib.request.Request(url, data=json.dumps(payload).encode("utf-8"), headers=headers, method="POST")
            with urllib.request.urlopen(req, timeout=90) as resp:
                data = json.loads(resp.read().decode("utf-8"))
                choice = data["choices"][0]["message"]
                return {
                    "content": choice.get("content", ""),
                    "tool_calls": choice.get("tool_calls", [])
                }
        except Exception as e:
            return {"content": f"[LỖI KẾT NỐI OLLAMA LOCAL]: Hãy kiểm tra xem Ollama đã chạy chưa (`ollama serve`). Chi tiết: {str(e)}", "tool_calls": []}


def create_llm_client(
    provider: str = "mock",
    model_name: Optional[str] = None,
    api_key: Optional[str] = None,
    mock_responses: Optional[List[Dict[str, Any]]] = None
) -> BaseLLMClient:
    """
    Factory khởi tạo LLM Client linh hoạt dựa trên cấu hình.
    """
    p = provider.lower()
    if p == "openai":
        return OpenAILLMClient(api_key=api_key, model_name=model_name or "gpt-4o-mini")
    elif p in ["gemini", "google"]:
        return GeminiLLMClient(api_key=api_key, model_name=model_name or "gemini-1.5-flash")
    elif p in ["ollama", "local"]:
        return OllamaLLMClient(model_name=model_name or "llama3")
    else:
        return MockLLMClient(responses=mock_responses)


def create_tool_schema(name: str, description: str, parameters: Dict[str, Any], required: List[str]) -> Dict[str, Any]:
    """Tạo schema chuẩn JSON Schema cho Tool Calling."""
    return {
        "type": "function",
        "function": {
            "name": name,
            "description": description,
            "parameters": {
                "type": "object",
                "properties": parameters,
                "required": required
            }
        }
    }
