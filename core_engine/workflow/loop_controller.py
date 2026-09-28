"""
Module: core_engine.workflow.loop_controller
Mục đích: Kiểm soát số vòng lặp sửa lỗi (Bounded Self-Correction Loop)
Tuân thủ nguyên tắc Claude & Spec: Ngăn chặn vòng lặp vô hạn, Bounded Retry (Max 5)
"""

from dataclasses import dataclass
from core_engine.state import ProjectState, Phase


@dataclass
class LoopController:
    max_repair_iterations: int = 5

    def check_and_increment(self, state: ProjectState) -> bool:
        """
        Tăng biến đếm vòng lặp và kiểm tra xem có được phép tiếp tục hay không.
        Trả về True nếu còn trong hạn mức, False nếu vượt quá giới hạn.
        """
        state.iteration_count += 1
        if state.iteration_count > self.max_repair_iterations:
            state.current_phase = Phase.FAILED_REQUIRING_HUMAN_REVIEW
            state.errors.append(
                f"[CIRCUIT_BREAKER] Đã chạm giới hạn tối đa {self.max_repair_iterations} vòng lặp sửa lỗi. Dừng lại để yêu cầu con người can thiệp."
            )
            return False
        return True
