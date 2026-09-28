"""
Module: core_engine.agents.coding_agent
Mục đích: Coding Agent với vòng lặp tự sửa lỗi (Self-Correction Coding Loop)
Tuân thủ nguyên tắc Claude: Inspect Before Act, High-Signal Stderr Analysis, Bounded Repair
"""

import json
from typing import Dict, Any, List
from core_engine.state import ProjectState, Phase
from core_engine.agents.base_agent import BaseAgent


CODING_AGENT_SYSTEM_PROMPT = """Bạn là Senior AI Software Engineer.
Nhiệm vụ của bạn là hiện thực hóa tính năng hoặc sửa lỗi phần mềm trong dự án.

QUY TẮC BẮT BUỘC (CLAUDE PRODUCTION PRINCIPLES):
1. INSPECT BEFORE ACT: Bạn PHẢI đọc file nguồn (read_file / search_code) để nắm rõ cấu trúc TRƯỚC KHI thực hiện chỉnh sửa. Không đoán mò nội dung.
2. PRECISE EDITING: Khi sửa code (edit_file / write_file), chỉ sửa đúng đoạn cần thiết, giữ nguyên các comment và convention sẵn có.
3. TEST & VERIFY: Sau khi sửa, PHẢI chạy test (run_pytest / run_command).
4. SELF-CORRECTION: Nếu test FAIL, đọc kỹ stderr/stacktrace, phân tích nguyên nhân gốc rễ và tiếp tục sửa cho đến khi PASS.
5. STOP CONDITION: Khi test PASS, tóm tắt những gì đã làm và kết thúc.
"""


class CodingAgent(BaseAgent):
    def __init__(self, llm_client, filesystem, terminal, git):
        super().__init__(
            name="CodingAgent",
            system_prompt=CODING_AGENT_SYSTEM_PROMPT,
            llm_client=llm_client,
            filesystem=filesystem,
            terminal=terminal,
            git=git,
        )

    def step(self, state: ProjectState) -> ProjectState:
        """Thực hiện 1 lượt hành động coding được điều phối bởi Orchestrator."""
        messages = [
            {"role": "system", "content": self.system_prompt},
            {
                "role": "user",
                "content": (
                    f"Task: {state.task_description}\n"
                    f"Requirements: {state.requirements}\n"
                    f"Plan steps: {state.plan_steps}\n"
                    f"Lịch sử lỗi: {state.errors[-1] if state.errors else 'Chưa có lỗi.'}\n"
                    f"Hãy gọi tool phù hợp để thực thi hoặc sửa lỗi."
                )
            }
        ]
        response = self.llm.call(messages=messages)
        tool_calls = response.get("tool_calls", [])

        for call in tool_calls:
            tool_name = call.get("name")
            tool_args = call.get("arguments", {})
            self.execute_tool(tool_name, tool_args, state)
            if tool_name in ("write_file", "edit_file") and "path" in tool_args:
                if tool_args["path"] not in state.changed_files:
                    state.changed_files.append(tool_args["path"])

        return state


    def execute_self_repair_loop(self, state: ProjectState, test_target: str = "tests") -> ProjectState:
        """
        Vòng lặp tự sửa lỗi cốt lõi:
        1. Lặp qua các bước: Inspect -> Edit -> Test
        2. Nếu test FAIL -> Tăng iteration_count, đọc stderr, lặp lại
        3. Dừng lại khi: Test PASS hoặc chạm MAX_REPAIR_ITERATIONS
        """
        state.current_phase = Phase.IMPLEMENTING

        while state.can_retry():
            state.iteration_count += 1
            
            # Chuẩn bị messages context cho LLM
            messages = [
                {"role": "system", "content": self.system_prompt},
                {
                    "role": "user",
                    "content": (
                        f"Task: {state.task_description}\n"
                        f"Vòng lặp hiện tại: {state.iteration_count}/{state.max_iterations}\n"
                        f"Lịch sử lỗi gần nhất: {state.test_results.get('raw_output', 'Chưa có test nào chạy.')}\n"
                        f"Hãy quyết định hành động tiếp theo (gọi tool hoặc hoàn tất)."
                    )
                }
            ]

            # Gọi LLM
            response = self.llm.call(messages=messages)
            tool_calls = response.get("tool_calls", [])

            if not tool_calls:
                # LLM không gọi tool nào nữa, tiến hành chạy test kiểm chứng cuối cùng
                test_outcome = self.terminal.run_pytest(test_target=test_target)
                state.test_results = test_outcome
                if test_outcome["is_passed"]:
                    state.current_phase = Phase.DONE
                    return state
                else:
                    # Test vẫn fail, ghi nhận lỗi để vòng lặp sau sửa
                    state.errors.append(f"Vòng {state.iteration_count}: Test vẫn thất bại.\n{test_outcome['raw_output']}")

            # Thực thi danh sách tool calls
            for call in tool_calls:
                tool_name = call.get("name")
                tool_args = call.get("arguments", {})
                tool_result = self.execute_tool(tool_name, tool_args, state)
                
                # Ghi nhận file đã sửa đổi
                if tool_name in ("write_file", "edit_file") and "path" in tool_args:
                    if tool_args["path"] not in state.changed_files:
                        state.changed_files.append(tool_args["path"])

            # Sau khi thực hiện xong các tool, tự động chạy lại test để kiểm tra
            test_outcome = self.terminal.run_pytest(test_target=test_target)
            state.test_results = test_outcome

            if test_outcome["is_passed"]:
                state.current_phase = Phase.DONE
                return state

        # Nếu vượt quá giới hạn số lần sửa
        state.current_phase = Phase.FAILED_REQUIRING_HUMAN_REVIEW
        return state
