"""
Skill: core_engine.skills.abramov_client_state_skill
Chuyên gia: Dan Abramov & Christopher Chedeau (Agent 10 - Lead Cross-Platform & Client State Specialist)
Lĩnh vực: Cross-Platform Mobile/Web Architecture, Client State Management & Optimistic UI
"""

import uuid
import time
import hashlib
from typing import Dict, Any, List, Optional


class AbramovClientStateSkill:
    """Bộ công cụ thiết kế kiến trúc Client-Side State Management, Optimistic UI và Offline-First Sync."""

    @staticmethod
    def design_client_store_schema() -> Dict[str, Any]:
        """
        Sinh lược đồ cấu trúc Global Client State Store (Zustand / Redux Toolkit Slices)
        đảm bảo tính bất biến và phân tách các miền nghiệp vụ rõ ràng.
        """
        return {
            "store_architecture": "Unidirectional Data Flow (Zustand / Redux Toolkit)",
            "slices": {
                "auth_slice": {
                    "state": {
                        "user_id": "Optional[str]",
                        "access_token": "Optional[str]",
                        "is_authenticated": "bool",
                        "biometric_enabled": "bool",
                    },
                    "actions": ["login()", "logout()", "refresh_token()", "enable_biometrics()"]
                },
                "joint_fund_slice": {
                    "state": {
                        "funds": "List[JointFund]",
                        "active_fund_id": "Optional[str]",
                        "contribution_stats": "Dict[str, float]",
                        "is_loading": "bool",
                    },
                    "actions": ["fetch_funds()", "deposit_fund()", "allocate_allowance()", "set_active_fund()"]
                },
                "expense_slice": {
                    "state": {
                        "items": "List[ExpenseItem]",
                        "filter_category": "Optional[str]",
                        "optimistic_queue": "List[OptimisticAction]",
                        "sync_status": "Literal['IDLE', 'SYNCING', 'OFFLINE', 'ERROR']",
                    },
                    "actions": ["create_expense_optimistic()", "delete_expense()", "retry_failed_sync()"]
                },
                "debt_graph_slice": {
                    "state": {
                        "simplified_debts": "List[DebtTransaction]",
                        "total_payable": "float",
                        "total_receivable": "float",
                        "last_computed_at": "str",
                    },
                    "actions": ["recompute_debt_graph()", "settle_debt_optimistic()", "generate_qr_payload()"]
                }
            }
        }

    @staticmethod
    def simulate_optimistic_ui_action(
        action_type: str,
        payload: Dict[str, Any],
        server_error: bool = False
    ) -> Dict[str, Any]:
        """
        Mô phỏng vòng đời hành động Optimistic UI 3 bước:
        1. Cập nhật tức thì vào Local Store (0ms latency, status: SYNCING)
        2. Gửi request ngầm lên server kèm Idempotency-Key
        3. Reconcile ID chính thức hoặc Tự động Rollback nếu server báo lỗi
        """
        temp_id = f"temp_{uuid.uuid4().hex[:8]}"
        timestamp = time.time()

        # Tạo idempotency key từ action + payload
        raw_key = f"{action_type}:{str(sorted(payload.items()))}:{timestamp}"
        idempotency_key = hashlib.sha256(raw_key.encode("utf-8")).hexdigest()[:16]

        # Bước 1: Trạng thái Optimistic ngay trên UI
        optimistic_state = {
            "id": temp_id,
            "action_type": action_type,
            "data": payload,
            "sync_status": "SYNCING",
            "is_optimistic": True,
            "created_at": timestamp,
            "idempotency_key": idempotency_key
        }

        # Bước 2 & 3: Phản hồi từ Server
        if server_error:
            # Rollback state
            return {
                "step_1_optimistic_commit": optimistic_state,
                "step_2_server_response": {"status_code": 500, "message": "Network timeout / Server error"},
                "step_3_final_resolution": {
                    "action": "ROLLBACK_STATE",
                    "reverted_id": temp_id,
                    "final_status": "ERROR_ROLLBACK",
                    "user_notification": "Giao dịch chưa thể lưu. Dữ liệu đã được đưa vào Hàng đợi Ngoại tuyến (Offline Queue)."
                }
            }
        else:
            real_id = f"real_{uuid.uuid4().hex[:12]}"
            return {
                "step_1_optimistic_commit": optimistic_state,
                "step_2_server_response": {"status_code": 200, "message": "Success", "server_id": real_id},
                "step_3_final_resolution": {
                    "action": "RECONCILE_SUCCESS",
                    "reconciled_id": real_id,
                    "temp_id_replaced": temp_id,
                    "final_status": "COMMITTED",
                    "is_optimistic": False
                }
            }

    @staticmethod
    def design_offline_sync_queue() -> Dict[str, Any]:
        """
        Thiết kế cơ chế hàng đợi đồng bộ ngoại tuyến (Offline-First Sync Queue)
        kết hợp lưu trữ SQLite/MMKV cục bộ và thuật toán giải quyết xung đột Vector Clock.
        """
        return {
            "local_storage_engine": "MMKV (Key-Value) & SQLite / WatermelonDB (Relational Cache)",
            "priority_order": [
                "1. SettleDebtAction (Ưu tiên cao nhất - Thanh toán tiền)",
                "2. CreateExpenseAction (Ưu tiên thứ 2 - Thêm chi tiêu / Quét OCR)",
                "3. UpdateFundAllocationAction (Ưu tiên thứ 3 - Cập nhật phân bổ quỹ)",
                "4. EditMetadataAction (Ưu tiên thấp nhất - Sửa danh mục, ghi chú)"
            ],
            "conflict_resolution": {
                "strategy": "Last-Write-Wins with Vector Clock",
                "clock_fields": ["device_id", "client_timestamp_utc", "sequence_number"],
                "rule": "Khi 2 người dùng cùng sửa 1 khoản chi tiêu lúc mất mạng, bản ghi có (Timestamp, Sequence) lớn hơn sẽ ghi đè; dữ liệu cũ được lưu vào Audit Log."
            },
            "network_reconnect_lifecycle": [
                "1. Network Listener phát hiện 'Online'",
                "2. Đọc tuần tự các task trong SQLite Queue theo thứ tự Priority",
                "3. Gửi batch request kèm Idempotency-Key",
                "4. Xóa các task đã sync thành công khỏi Local Queue",
                "5. Phát Toast: 'Đã đồng bộ thành công N giao dịch ngoại tuyến.'"
            ]
        }

    @classmethod
    def format_frontend_architecture_adr(cls, feature_name: str) -> str:
        """Định dạng đặc tả kiến trúc Frontend & Client State nhúng vào ADR."""
        store_schema = cls.design_client_store_schema()
        offline_spec = cls.design_offline_sync_queue()
        opt_demo = cls.simulate_optimistic_ui_action("CREATE_EXPENSE", {"amount": 250000, "note": "Đi chợ"})

        return (
            f"## ĐẶC TẢ KIẾN TRÚC CLIENT & TRẠNG THÁI (Dan Abramov & Christopher Chedeau)\n\n"
            f"### 1. Kiến Trúc Quản Lý Trạng Thái (Client State Store)\n"
            f"- **Framework & Pattern:** `{store_schema['store_architecture']}`\n"
            f"- **Các Slices chính:** {', '.join(store_schema['slices'].keys())}\n\n"
            f"### 2. Cơ Chế Cập Nhật Tức Thì (Optimistic UI 0ms)\n"
            f"```text\n"
            f"[User Bấm Lưu] ---> [Ghi Ngay Temp ID vào Store (0ms) -> UI Xanh (SYNCING)]\n"
            f"                          │\n"
            f"                          ▼ (Gửi ngầm Idempotency-Key)\n"
            f"                    [Backend API]\n"
            f"                     ├── Thành công (200) -> Reconcile Temp ID thành Real ID -> COMMITTED\n"
            f"                     └── Thất bại  (500) -> Rollback Store về bản gốc & đẩy vào Offline Queue\n"
            f"```\n\n"
            f"### 3. Chiến Lược Ngoại Tuyến (Offline-First Sync Strategy)\n"
            f"- **Local Storage:** `{offline_spec['local_storage_engine']}`\n"
            f"- **Giải Quyết Xung Đột:** `{offline_spec['conflict_resolution']['strategy']}`\n"
            f"- **Thứ Tự Ưu Tiên:**\n"
            + "\n".join([f"  - {p}" for p in offline_spec["priority_order"]])
            + "\n"
        )

    @staticmethod
    def generate_production_component_code(
        component_name: str,
        framework: str = "react"
    ) -> str:
        """
        Sinh mã nguồn linh kiện thực chiến chuẩn Production sẵn sàng copy-paste,
        hỗ trợ đa dạng nền tảng: 'react', 'angular', 'blazor'.
        """
        fw = framework.lower()
        clean_name = component_name.replace(" ", "").replace("_", "")

        if "angular" in fw:
            return (
                f"import {{ Component, Input, Output, EventEmitter, signal }} from '@angular/core';\n"
                f"import {{ CommonModule }} from '@angular/common';\n\n"
                f"@Component({{\n"
                f"  selector: 'app-{clean_name.lower()}',\n"
                f"  standalone: true,\n"
                f"  imports: [CommonModule],\n"
                f"  template: `\n"
                f"    <div class='p-6 bg-white dark:bg-slate-800 rounded-2xl shadow-md border border-slate-100 dark:border-slate-700'>\n"
                f"      <h3 class='text-sm font-semibold text-slate-500 dark:text-slate-400 uppercase'>{{{{ title }}}}</h3>\n"
                f"      <div class='text-3xl font-bold text-slate-900 dark:text-white mt-2'>{{{{ amount | currency:'VND':'symbol':'1.0-0' }}}}</div>\n"
                f"      <button (click)='actionClicked.emit()' class='mt-4 w-full py-2.5 bg-blue-600 hover:bg-blue-700 text-white font-medium rounded-xl transition-all'>\n"
                f"        Xác Nhận Thao Tác\n"
                f"      </button>\n"
                f"    </div>\n"
                f"  `\n"
                f"}})\n"
                f"export class {clean_name}Component {{\n"
                f"  @Input() title: string = '{clean_name}';\n"
                f"  @Input() amount: number = 0;\n"
                f"  @Output() actionClicked = new EventEmitter<void>();\n"
                f"  isLoading = signal<boolean>(false);\n"
                f"}}\n"
            )
        elif "blazor" in fw or "dotnet" in fw or "csharp" in fw:
            return (
                f"@* Blazor Component: {clean_name}.razor *@\n"
                f"<div class=\"p-6 bg-white dark:bg-slate-800 rounded-2xl shadow-md border border-slate-100 dark:border-slate-700\">\n"
                f"    <h3 class=\"text-sm font-semibold text-slate-500 uppercase\">@Title</h3>\n"
                f"    <div class=\"text-3xl font-bold text-slate-900 mt-2\">@Amount.ToString(\"N0\") VND</div>\n"
                f"    <button @onclick=\"HandleActionClick\" class=\"mt-4 w-full py-2.5 bg-blue-600 text-white rounded-xl hover:bg-blue-700 transition\">\n"
                f"        Xác Nhận Thao Tác\n"
                f"    </button>\n"
                f"</div>\n\n"
                f"@code {{\n"
                f"    [Parameter] public string Title {{ get; set; }} = \"{clean_name}\";\n"
                f"    [Parameter] public decimal Amount {{ get; set; }} = 0m;\n"
                f"    [Parameter] public EventCallback OnActionClick {{ get; set; }}\n\n"
                f"    private async Task HandleActionClick()\n"
                f"    {{\n"
                f"        if (OnActionClick.HasDelegate)\n"
                f"            await OnActionClick.InvokeAsync();\n"
                f"    }}\n"
                f"}}\n"
            )
        else:
            # Default: React 19 / TypeScript + TailwindCSS + Zustand Slices
            return (
                f"import React, {{ useState }} from 'react';\n\n"
                f"export interface {clean_name}Props {{\n"
                f"  title?: string;\n"
                f"  amount: number;\n"
                f"  currency?: string;\n"
                f"  onAction?: () => Promise<void> | void;\n"
                f"}}\n\n"
                f"export const {clean_name}: React.FC<{clean_name}Props> = ({{\n"
                f"  title = '{clean_name}',\n"
                f"  amount,\n"
                f"  currency = 'VND',\n"
                f"  onAction,\n"
                f"}}) => {{\n"
                f"  const [isSubmitting, setIsSubmitting] = useState(false);\n\n"
                f"  const handleClick = async () => {{\n"
                f"    if (!onAction) return;\n"
                f"    try {{\n"
                f"      setIsSubmitting(true);\n"
                f"      await onAction();\n"
                f"    }} finally {{\n"
                f"      setIsSubmitting(false);\n"
                f"    }}\n"
                f"  }};\n\n"
                f"  return (\n"
                f"    <div className='p-6 bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-100 dark:border-slate-700 transition-all hover:shadow-xl'>\n"
                f"      <div className='flex justify-between items-center mb-3'>\n"
                f"        <h3 className='text-sm font-semibold text-slate-500 dark:text-slate-400 uppercase tracking-wider'>\n"
                f"          {{title}}\n"
                f"        </h3>\n"
                f"        <span className='inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-50 text-blue-700 dark:bg-blue-900/30 dark:text-blue-300'>\n"
                f"          Hoạt Động\n"
                f"        </span>\n"
                f"      </div>\n"
                f"      <div className='text-3xl font-bold text-slate-900 dark:text-white'>\n"
                f"        {{new Intl.NumberFormat('vi-VN').format(amount)}} <span className='text-sm font-normal text-slate-500'>{{currency}}</span>\n"
                f"      </div>\n"
                f"      <button\n"
                f"        onClick={{handleClick}}\n"
                f"        disabled={{isSubmitting}}\n"
                f"        className='mt-5 w-full py-3 px-4 rounded-xl bg-blue-600 hover:bg-blue-700 disabled:opacity-50 text-white font-medium shadow-md shadow-blue-500/20 active:scale-[0.98] transition-all focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2'\n"
                f"      >\n"
                f"        {{isSubmitting ? 'Đang Xử Lý (0ms)...' : 'Xác Nhận Thao Tác'}}\n"
                f"      </button>\n"
                f"    </div>\n"
                f"  );\n"
                f"}};\n"
            )

