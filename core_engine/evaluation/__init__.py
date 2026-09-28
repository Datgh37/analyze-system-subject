"""
Package: core_engine.evaluation
Hệ thống đo lường, benchmark và đánh giá thực nghiệm.
"""

from core_engine.evaluation.metrics import MetricsCollector, EvaluationReport, TaskMetric

__all__ = [
    "MetricsCollector",
    "EvaluationReport",
    "TaskMetric",
]
