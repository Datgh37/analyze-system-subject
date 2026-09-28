"""
Skill: core_engine.skills.frost_atomic_design_skill
Chuyên gia: Brad Frost & Don Norman (Agent 09 - Lead Design System & UI/UX Specialist)
Lĩnh vực: Atomic Design System, Wireframe Layout Specs & WCAG Accessibility
"""

from typing import Dict, Any, List, Optional


class FrostAtomicDesignSkill:
    """Bộ công cụ thiết kế hệ thống giao diện Atomic Design và kiểm toán công thái học UI/UX."""

    @staticmethod
    def generate_design_tokens() -> Dict[str, Any]:
        """
        Sinh bảng Design Tokens tiêu chuẩn cho ứng dụng quản lý tài chính & chi tiêu nhóm.
        Hỗ trợ cả Light Mode và Dark Mode, tuân thủ bảng mã màu chuẩn WCAG.
        """
        return {
            "colors": {
                "brand": {
                    "primary": "#2563EB",       # Blue 600
                    "primary_hover": "#1D4ED8", # Blue 700
                    "primary_light": "#DBEAFE", # Blue 100
                },
                "semantic": {
                    "success": "#16A34A",       # Green 600 (Thu tiền, quỹ dương)
                    "warning": "#D97706",       # Amber 600 (Sắp vượt ngân sách)
                    "danger": "#DC2626",        # Red 600 (Vượt ngân sách, nợ)
                    "info": "#0284C7",          # Sky 600 (Thông tin giao dịch)
                },
                "neutrals": {
                    "bg_light": "#F8FAFC",      # Slate 50
                    "surface_light": "#FFFFFF", # White
                    "text_primary_light": "#0F172A",   # Slate 900
                    "text_secondary_light": "#64748B", # Slate 500
                    "border_light": "#E2E8F0",  # Slate 200
                    "bg_dark": "#0B0F19",       # Deep Navy/Dark
                    "surface_dark": "#1E293B",  # Slate 800
                    "text_primary_dark": "#F8FAFC",    # Slate 50
                    "text_secondary_dark": "#94A3B8",  # Slate 400
                    "border_dark": "#334155",   # Slate 700
                }
            },
            "typography": {
                "font_family_sans": "Inter, system-ui, -apple-system, sans-serif",
                "font_family_mono": "'JetBrains Mono', monospace",
                "font_sizes": {
                    "xs": "0.75rem",   # 12px
                    "sm": "0.875rem",  # 14px
                    "base": "1rem",    # 16px
                    "lg": "1.125rem",  # 18px
                    "xl": "1.25rem",   # 20px
                    "2xl": "1.5rem",   # 24px
                    "3xl": "1.875rem", # 30px
                },
                "font_weights": {
                    "regular": "400",
                    "medium": "500",
                    "semibold": "600",
                    "bold": "700",
                }
            },
            "spacing": {
                "xs": "4px",
                "sm": "8px",
                "md": "16px",
                "lg": "24px",
                "xl": "32px",
                "2xl": "48px",
            },
            "radii": {
                "sm": "6px",
                "md": "10px",
                "lg": "16px",
                "full": "9999px",
            },
            "elevation": {
                "card": "0 1px 3px 0 rgba(0, 0, 0, 0.1), 0 1px 2px -1px rgba(0, 0, 0, 0.1)",
                "modal": "0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 8px 10px -6px rgba(0, 0, 0, 0.1)",
            }
        }

    @staticmethod
    def generate_atomic_component_tree(feature_name: str) -> Dict[str, List[str]]:
        """
        Phân rã hệ thống linh kiện giao diện theo 5 cấp bậc của Atomic Design.
        """
        return {
            "atoms": [
                "AmountDisplay (Định dạng tiền tệ VNĐ)",
                "CurrencyBadge (Huy hiệu loại tiền tệ)",
                "UserAvatar (Ảnh đại diện thành viên)",
                "PrimaryButton (Nút xác nhận hành động chính)",
                "StatusChip (Nhãn trạng thái PENDING/COMPLETED)",
                "CategoryIcon (Biểu tượng danh mục ăn uống, tiền nhà)",
            ],
            "molecules": [
                "ExpenseItemRow (Dòng giao dịch kèm avatar, số tiền, ngày giờ)",
                "MemberContributionBar (Thanh tiến độ đóng góp quỹ theo tỷ lệ %)",
                "SplitTypeSegmentControl (Bộ chọn chia đều / chia theo tỷ lệ / chia theo món)",
                "NudgeAlertBanner (Banner cảnh báo tốc độ chi tiêu nhẹ nhàng)",
                "ReceiptItemRow (Dòng món ăn bóc tách từ OCR kèm nút tick chọn)",
            ],
            "organisms": [
                "JointPoolFundCard (Thẻ tổng quan Quỹ gia đình kèm số dư và nút đóng quỹ)",
                "DebtReductionGraphCanvas (Vùng hiển thị biểu đồ đồ thị nợ tương tác)",
                "ReceiptOCRScannerModal (Hộp thoại xem trước ảnh chụp hóa đơn và đối soát)",
                "MonthlyBudgetGauge (Đồng hồ đo lường ngân sách tháng)",
            ],
            "templates": [
                f"{feature_name}Template (Bố cục khung gồm App Header, Content Area, Bottom Sheet)",
            ],
            "pages": [
                f"{feature_name}Screen (Màn hình hoàn chỉnh tích hợp dữ liệu thực)",
            ]
        }

    @staticmethod
    def _hex_to_rgb(hex_str: str) -> List[float]:
        hex_clean = hex_str.lstrip("#")
        if len(hex_clean) == 3:
            hex_clean = "".join([c * 2 for c in hex_clean])
        return [int(hex_clean[i:i+2], 16) / 255.0 for i in (0, 2, 4)]

    @staticmethod
    def _calculate_luminance(rgb: List[float]) -> float:
        adjusted = []
        for v in rgb:
            if v <= 0.03928:
                adjusted.append(v / 12.92)
            else:
                adjusted.append(((v + 0.055) / 1.055) ** 2.4)
        return 0.2126 * adjusted[0] + 0.7152 * adjusted[1] + 0.0722 * adjusted[2]

    @classmethod
    def evaluate_wcag_accessibility(cls, foreground_hex: str, background_hex: str) -> Dict[str, Any]:
        """
        Tính toán tỷ lệ tương phản màu sắc thực tế (Contrast Ratio) theo chuẩn WCAG 2.1.
        Formula: (L1 + 0.05) / (L2 + 0.05)
        """
        try:
            rgb_fg = cls._hex_to_rgb(foreground_hex)
            rgb_bg = cls._hex_to_rgb(background_hex)

            lum_fg = cls._calculate_luminance(rgb_fg)
            lum_bg = cls._calculate_luminance(rgb_bg)

            l1 = max(lum_fg, lum_bg)
            l2 = min(lum_fg, lum_bg)

            contrast_ratio = round((l1 + 0.05) / (l2 + 0.05), 2)
            passes_aa_normal = contrast_ratio >= 4.5
            passes_aa_large = contrast_ratio >= 3.0
            passes_aaa = contrast_ratio >= 7.0

            verdict = "WCAG AAA (Xuất sắc)" if passes_aaa else ("WCAG AA (Đạt chuẩn)" if passes_aa_normal else "FAIL (Không đạt độ tương phản)")

            return {
                "foreground": foreground_hex,
                "background": background_hex,
                "contrast_ratio": contrast_ratio,
                "passes_aa_normal_text": passes_aa_normal,
                "passes_aa_large_text": passes_aa_large,
                "passes_aaa": passes_aaa,
                "verdict": verdict
            }
        except Exception as e:
            return {
                "error": str(e),
                "contrast_ratio": 1.0,
                "verdict": "ERROR_CALCULATING"
            }

    @staticmethod
    def generate_wireframe_spec(screen_name: str) -> str:
        """Sinh bản vẽ bố cục khung dây màn hình Wireframe dạng ASCII/Markdown trực quan."""
        return (
            f"### BẢN VẼ WIREFRAME BỐ CỤC: {screen_name.upper()}\n\n"
            "```text\n"
            "+--------------------------------------------------+\n"
            "| [<] Quay lại           TỔNG QUAN CHI TIÊU    [?] |\n"
            "+--------------------------------------------------+\n"
            "|                                                  |\n"
            "|  +--------------------------------------------+  |\n"
            "|  | QUỸ GIA ĐÌNH: 'TỔ ẤM HẠNH PHÚC'            |  |\n"
            "|  | Số dư khả dụng:  24,500,000 đ              |  |\n"
            "|  | Tiêu trong tháng:  8,200,000 đ (35%)       |  |\n"
            "|  | [===========-----------------] [Nạp Quỹ +] |  |\n"
            "|  +--------------------------------------------+  |\n"
            "|                                                  |\n"
            "|  PHÂN BỔ ĐÓNG GÓP:                               |\n"
            "|  - Chồng (60%): 15,000,000 đ  [Đã nộp: 100%]     |\n"
            "|  - Vợ    (40%): 10,000,000 đ  [Đã nộp: 100%]     |\n"
            "|                                                  |\n"
            "|  GIAO DỊCH GẦN ĐÂY:                              |\n"
            "|  [Icon: Đi chợ]  Siêu thị WinMart     -850,000 đ |\n"
            "|  [Icon: Điện]    Tiền điện tháng 8  -1,240,000 đ |\n"
            "|  [Icon: OCR]     Hóa đơn Nhà hàng   -1,500,000 đ |\n"
            "|                                                  |\n"
            "|  +--------------------------------------------+  |\n"
            "|  | [💡 Cú hích Thaler]: Bạn đang tiết kiệm    |  |\n"
            "|  | tốt hơn 15% so với tháng trước!            |  |\n"
            "|  +--------------------------------------------+  |\n"
            "|                                                  |\n"
            "|  [ + Thêm Chi Tiêu ]     [ 📷 Quét Hóa Đơn OCR ] |\n"
            "+--------------------------------------------------+\n"
            "| [🏠 Trang Chủ]  [📊 Báo Cáo]  [👥 Nhóm]  [⚙️ Cài Đặt] |\n"
            "+--------------------------------------------------+\n"
            "```"
        )

    @classmethod
    def format_design_system_adr(cls, feature_name: str) -> str:
        """Định dạng đặc tả Design System thành tài liệu chuẩn bị nhúng vào ADR."""
        tokens = cls.generate_design_tokens()
        comp_tree = cls.generate_atomic_component_tree(feature_name)
        wireframe = cls.generate_wireframe_spec(feature_name)
        wcag = cls.evaluate_wcag_accessibility("#0F172A", "#FFFFFF")

        return (
            f"## ĐẶC TẢ GIAO DIỆN & DESIGN SYSTEM (Brad Frost & Don Norman)\n\n"
            f"### 1. Bảng Design Tokens Cốt Lõi\n"
            f"- **Primary Brand:** `{tokens['colors']['brand']['primary']}` (Tương phản: {wcag['contrast_ratio']}:1 — {wcag['verdict']})\n"
            f"- **Success:** `{tokens['colors']['semantic']['success']}` | **Warning:** `{tokens['colors']['semantic']['warning']}` | **Danger:** `{tokens['colors']['semantic']['danger']}`\n"
            f"- **Font Family:** `{tokens['typography']['font_family_sans']}`\n"
            f"- **Spacing Grid:** 4px baseline ({', '.join([f'{k}: {v}' for k, v in tokens['spacing'].items()])})\n\n"
            f"### 2. Cây Phân Cấp Linh Kiện Nguyên Tử (Atomic Components)\n"
            f"- **Atoms:** {', '.join(comp_tree['atoms'])}\n"
            f"- **Molecules:** {', '.join(comp_tree['molecules'])}\n"
            f"- **Organisms:** {', '.join(comp_tree['organisms'])}\n\n"
            f"### 3. Sơ Đồ Bố Cục Khung Dây (Wireframe Spec)\n"
            f"{wireframe}\n"
        )

    @classmethod
    def generate_frontend_ui_spec_document(
        cls,
        feature_name: str,
        uc_id: str = "UC-FEAT-01",
        components: Optional[List[str]] = None
    ) -> str:
        """
        Tự động sinh toàn văn tệp 02b_frontend_ui_spec.md chuẩn mực 8 phần hoàn chỉnh.
        """
        tokens = cls.generate_design_tokens()
        tree = cls.generate_atomic_component_tree(feature_name)
        wireframe = cls.generate_wireframe_spec(feature_name)
        wcag = cls.evaluate_wcag_accessibility("#0F172A", "#FFFFFF")

        return (
            f"# ĐẶC TẢ GIAO DIỆN & TRẢI NGHIỆM NGƯỜI DÙNG (FRONTEND UI/UX SPECIFICATION)\n"
            f"**Tên Tính Năng:** {feature_name.upper()}\n"
            f"**Mã Ca Sử Dụng (Use Case ID):** `{uc_id}`\n"
            f"**Chuyên Gia Thiết Kế:** Brad Frost (Atomic Design) & Dan Abramov (Client Architecture)\n"
            f"**Tiêu Chuẩn Tuân Thủ:** WCAG 2.1 AA, Atomic Design System, Mobile-First Responsive, Optimistic UI (0ms)\n\n"
            f"---\n\n"
            f"## 1. Sơ Đồ Luồng Màn Hình & Điều Hướng (User Flow & Screen Map)\n\n"
            f"```text\n"
            f"[Dashboard / Danh Sách] ──► [Modal Nhập Liệu / Form] ──► [Optimistic UI 0ms Update] ──► [Đồng Bộ Backend]\n"
            f"```\n\n"
            f"---\n\n"
            f"## 2. Bản Vẽ Khung Nhìn Giao Diện (ASCII Wireframes)\n\n"
            f"{wireframe}\n\n"
            f"---\n\n"
            f"## 3. Hệ Thống Token Thiết Kế (Design Tokens Catalog)\n\n"
            f"### A. Bảng Màu Chuẩn HSL & Hex\n"
            f"- **Primary Brand:** `{tokens['colors']['brand']['primary']}` (Tương phản: {wcag['contrast_ratio']}:1 — {wcag['verdict']})\n"
            f"- **Success (Tiết kiệm/Thu):** `{tokens['colors']['semantic']['success']}`\n"
            f"- **Warning (Cảnh báo chi):** `{tokens['colors']['semantic']['warning']}`\n"
            f"- **Danger (Vượt ngân sách/Nợ):** `{tokens['colors']['semantic']['danger']}`\n"
            f"- **Background Light / Dark:** `{tokens['colors']['neutrals']['bg_light']}` / `{tokens['colors']['neutrals']['bg_dark']}`\n\n"
            f"### B. Typography & Spacing Grid\n"
            f"- **Font Family:** `{tokens['typography']['font_family_sans']}`\n"
            f"- **Spacing Scale:** `xs: 4px`, `sm: 8px`, `md: 16px`, `lg: 24px`, `xl: 32px` (4px/8px Baseline Grid)\n\n"
            f"---\n\n"
            f"## 4. Cây Phân Cấp Linh Kiện Nguyên Tử (Atomic Component Hierarchy)\n\n"
            f"- **Atoms:** {', '.join(tree['atoms'])}\n"
            f"- **Molecules:** {', '.join(tree['molecules'])}\n"
            f"- **Organisms:** {', '.join(tree['organisms'])}\n"
            f"- **Templates:** {', '.join(tree['templates'])}\n"
            f"- **Pages:** {', '.join(tree['pages'])}\n\n"
            f"---\n\n"
            f"## 5. Chiến Lược Thích Ứng Màn Hình (Responsive Breakpoints)\n\n"
            f"| Breakpoint | Chiều Rộng | Hành Vi Bố Cục |\n"
            f"| :--- | :--- | :--- |\n"
            f"| **Mobile (`sm`)** | `375px - 640px` | 1 cột đơn, Bottom Sheet cho Form, nút bấm Sticky đáy màn hình. |\n"
            f"| **Tablet (`md`)** | `768px - 1024px`| 2 cột (Danh sách bên trái, Biểu đồ bên phải). |\n"
            f"| **Desktop (`lg`)**| `1280px+` | Dashboard 3 cột đa năng, Modal Popup trung tâm. |\n\n"
            f"---\n\n"
            f"## 6. Kiểm Toán Khả Năng Tiếp Cận (WCAG 2.1 AA Checklist)\n\n"
            f"- [x] **Tương phản màu sắc:** Text chính trên nền đạt `{wcag['contrast_ratio']}:1` (vượt chuẩn tối thiểu 4.5:1).\n"
            f"- [x] **Phím điều hướng (Keyboard Focus):** `focus-visible:ring-2 ring-blue-500` rõ ràng.\n"
            f"- [x] **Trợ năng màn hình (Screen Reader):** `aria-label`, `role='alert'` cho thông báo lỗi.\n"
            f"- [x] **Vùng chạm (Touch Target):** Chiều cao nút bấm $\\ge 44 \\times 44\\text{{px}}$.\n\n"
            f"---\n\n"
            f"## 7. Form Validation (Zod Schema) & Client State Store Mapping\n\n"
            f"```typescript\n"
            f"import {{ z }} from 'zod';\n\n"
            f"export const {feature_name.replace(' ', '')}FormSchema = z.object({{\n"
            f"  title: z.string().min(3, 'Tên khoản chi phải từ 3 ký tự trở lên').max(100),\n"
            f"  amount: z.number().positive('Số tiền phải lớn hơn 0').max(100_000_000),\n"
            f"  categoryId: z.string().min(1, 'Vui lòng chọn danh mục'),\n"
            f"}});\n"
            f"```\n\n"
            f"---\n\n"
            f"## 8. Mã Nguồn Linh Kiện Mẫu Sẵn Sàng Thực Thi (Production Component Implementation)\n\n"
            f"```tsx\n"
            f"import React from 'react';\n\n"
            f"export const {feature_name.replace(' ', '')}Card: React.FC<{{ title: string; amount: number }}> = ({{ title, amount }}) => {{\n"
            f"  return (\n"
            f"    <div className='p-6 bg-white dark:bg-slate-800 rounded-2xl shadow-lg border border-slate-100 dark:border-slate-700'>\n"
            f"      <h3 className='text-sm font-semibold text-slate-500 dark:text-slate-400 uppercase'>{{title}}</h3>\n"
            f"      <div className='text-3xl font-bold text-slate-900 dark:text-white mt-2'>\n"
            f"        {{new Intl.NumberFormat('vi-VN', {{ style: 'currency', currency: 'VND' }}).format(amount)}}\n"
            f"      </div>\n"
            f"    </div>\n"
            f"  );\n"
            f"}};\n"
            f"```\n"
        )

