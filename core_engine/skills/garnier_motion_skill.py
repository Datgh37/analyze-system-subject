"""
Skill: core_engine.skills.garnier_motion_skill
Chuyên gia: Julian Garnier & Bret Victor (Agent 13 - Lead Motion Choreographer & Interactive Experience Engineer)
Lĩnh vực: Motion Choreography, Anime.js v4, Spring Physics, FLIP Layout & 60fps GPU Performance
"""

from typing import Dict, Any, List, Optional
import math


class GarnierMotionSkill:
    """Bộ công cụ đặc tả động lực học giao diện, tính toán gia tốc lò xo và kiểm toán hiệu năng 60fps."""

    @staticmethod
    def calculate_spring_physics(
        bounce: float = 0.2,
        duration: int = 600,
        stiffness: float = 100.0,
        damping: float = 20.0,
        mass: float = 1.0
    ) -> Dict[str, Any]:
        """
        Tính toán và cân chỉnh các thông số vật lý lò xo (Spring Physics) cho Anime.js v4.
        
        Công thức cơ học:
        - Tần số góc dao động riêng: omega_0 = sqrt(stiffness / mass)
        - Hệ số suy giảm tới hạn: zeta = damping / (2 * sqrt(stiffness * mass))
        - Độ vọt lố (overshoot / bounce) tương ứng với độ nảy xúc giác.
        """
        bounce = max(0.0, min(0.95, bounce))
        duration = max(100, min(3000, duration))
        stiffness = max(10.0, min(1000.0, stiffness))
        mass = max(0.1, min(10.0, mass))
        damping = max(1.0, min(100.0, damping))

        omega_0 = math.sqrt(stiffness / mass)
        zeta = damping / (2.0 * math.sqrt(stiffness * mass))

        # Phân loại trạng thái dao động
        if zeta < 1.0:
            regime = "Underdamped (Có dao động tắt dần, tạo độ nảy tự nhiên)"
            damping_ratio_desc = f"Zeta = {zeta:.2f} < 1 (Bouncy & Haptic)"
        elif math.isclose(zeta, 1.0, abs_tol=0.05):
            regime = "Critically Damped (Tắt dần tới hạn, về đích nhanh nhất không nảy)"
            damping_ratio_desc = f"Zeta = {zeta:.2f} ≈ 1 (Fast & Smooth)"
        else:
            regime = "Overdamped (Tắt dần chậm, êm dịu không nảy)"
            damping_ratio_desc = f"Zeta = {zeta:.2f} > 1 (Heavy & Gentle)"

        return {
            "parameters": {
                "bounce": bounce,
                "duration": duration,
                "stiffness": stiffness,
                "damping": damping,
                "mass": mass,
            },
            "physics_analysis": {
                "angular_frequency_rad_s": round(omega_0, 2),
                "damping_ratio_zeta": round(zeta, 3),
                "regime": regime,
                "damping_description": damping_ratio_desc,
            },
            "animejs_v4_code": f"spring({{ bounce: {bounce}, duration: {duration} }})",
            "recommended_use_cases": (
                "Hộp thoại cảnh báo vượt ngân sách, Thẻ nợ khi quẹt thanh toán, Nút bấm xác nhận giao dịch"
                if zeta < 1.0 else "Chuyển trang điều hướng, Dropdown menu, Thanh cuộn trang"
            )
        }

    @staticmethod
    def audit_60fps_gpu_performance(animated_properties: List[str]) -> Dict[str, Any]:
        """
        Kiểm toán danh sách thuộc tính CSS tham gia hoạt ảnh.
        Bảo đảm CHỈ can thiệp transform và opacity (GPU-safe).
        Cảnh báo nếu vi phạm các thuộc tính kích hoạt Layout Repaint (CPU-heavy).
        """
        gpu_safe_props = {
            "transform", "opacity", "x", "y", "z", "scale", "scaleX", "scaleY",
            "rotate", "rotateX", "rotateY", "rotateZ", "skew", "skewX", "skewY"
        }
        repaint_props = {
            "top", "left", "right", "bottom", "width", "height", "margin", "marginTop",
            "marginBottom", "marginLeft", "marginRight", "padding", "paddingTop",
            "paddingBottom", "paddingLeft", "paddingRight", "borderWidth", "borderRadius"
        }

        violations = []
        safe_used = []

        for prop in animated_properties:
            cleaned = prop.strip()
            if cleaned in repaint_props:
                violations.append(cleaned)
            elif cleaned in gpu_safe_props:
                safe_used.append(cleaned)
            else:
                # Các thuộc tính màu sắc hoặc filter
                violations.append(f"{cleaned} (Có nguy cơ trigger paint)")

        is_60fps_compliant = (len(violations) == 0)
        score = 100 if is_60fps_compliant else max(0, 100 - len(violations) * 25)

        return {
            "is_compliant": is_60fps_compliant,
            "performance_score": score,
            "gpu_safe_properties": safe_used,
            "layout_thrashing_violations": violations,
            "remedy": (
                "100% Thuộc tính đạt chuẩn GPU Compositor Layer."
                if is_60fps_compliant else
                f"Thay thế ngay các thuộc tính {violations} bằng 'transform' (translateX, translateY, scale) để loại bỏ Layout Thrashing."
            ),
            "will_change_directive": "will-change: transform, opacity;" if is_60fps_compliant else "none"
        }

    @staticmethod
    def generate_flip_layout_spec(
        component_name: str = "ExpenseListView",
        from_layout: str = "list",
        to_layout: str = "grid"
    ) -> Dict[str, Any]:
        """
        Đặc tả cơ chế chuyển đổi bố cục FLIP (First, Last, Invert, Play) sử dụng createLayout() của Anime.js v4.
        """
        return {
            "component": component_name,
            "transition": f"{from_layout.upper()} ➔ {to_layout.upper()}",
            "technique": "FLIP (First, Last, Invert, Play) via Anime.js v4 createLayout()",
            "steps": [
                {"step": 1, "name": "First", "action": "Ghi nhận vị trí bounding box ban đầu của các thẻ"},
                {"step": 2, "name": "Last", "action": "Thay đổi class CSS sang bố cục đích và ghi nhận vị trí mới"},
                {"step": 3, "name": "Invert", "action": "Tính toán ma trận chênh lệch dx, dy, dScale bằng transform ngược"},
                {"step": 4, "name": "Play", "action": "Kích hoạt animation đưa transform về 0 kèm gia tốc lò xo"}
            ],
            "sample_code": (
                f"const layout = createLayout('.{component_name.lower()}-container', {{\n"
                f"  duration: 450,\n"
                f"  ease: 'cubicBezier(0.16, 1, 0.3, 1)',\n"
                f"  onComplete: () => console.log('FLIP Transition Completed')\n"
                f"}});\n"
                f"layout.morph();"
            )
        }

    @staticmethod
    def generate_doppelrand_haptic_spec(component_name: str = "ExpenseSummaryCard") -> Dict[str, Any]:
        """
        Đặc tả cấu trúc vi thẩm mỹ bọc kép Double-Bezel (Doppelrand: Outer Shell + Inner Core)
        mang lại chiều sâu xúc giác cao cấp chuẩn $150k Agency.
        """
        return {
            "component": component_name,
            "architecture": "Double-Bezel (Doppelrand Nested Architecture)",
            "outer_shell": {
                "padding": "p-1.5 (6px)",
                "border_radius": "rounded-[2rem] (32px)",
                "border": "border border-white/10 dark:border-white/5",
                "background": "bg-slate-100/80 dark:bg-zinc-900/80",
                "shadow": "shadow-[0_8px_30px_rgb(0,0,0,0.06)]"
            },
            "inner_core": {
                "border_radius": "rounded-[calc(2rem-0.375rem)] (26px squircle đồng tâm)",
                "background": "bg-white dark:bg-zinc-950",
                "inner_highlight": "shadow-[inset_0_1px_1px_rgba(255,255,255,0.15)]",
                "padding": "p-6 (24px)"
            },
            "haptic_physics": {
                "active_state": "active:scale-[0.98] transition-transform duration-150 ease-out",
                "trailing_icon_island": "w-8 h-8 rounded-full bg-slate-100 dark:bg-zinc-900 flex items-center justify-center group-hover:translate-x-1 group-hover:-translate-y-[1px]"
            }
        }

    def generate_motion_choreography_spec(
        self,
        component_name: str = "TransactionCard",
        interaction_type: str = "press"
    ) -> Dict[str, Any]:
        """
        Sinh đặc tả chuyển động toàn diện cho một linh kiện tài chính.
        """
        spring_calc = self.calculate_spring_physics(bounce=0.25, duration=500)
        gpu_audit = self.audit_60fps_gpu_performance(["transform", "opacity"])
        doppelrand = self.generate_doppelrand_haptic_spec(component_name)

        return {
            "target_component": component_name,
            "interaction_type": interaction_type,
            "animejs_v4_module": "createScope() + animate()",
            "spring_physics": spring_calc,
            "gpu_audit": gpu_audit,
            "doppelrand_spec": doppelrand,
            "reduced_motion_fallback": (
                "if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) { "
                "animate(target, { opacity: [0, 1], duration: 150, ease: 'linear' }); }"
            )
        }

    @staticmethod
    def format_motion_spec_markdown(motion_data: Dict[str, Any]) -> str:
        """Định dạng đặc tả động học sang định dạng Markdown chuẩn đưa vào tài liệu 02b_frontend_ui_spec.md."""
        comp = motion_data.get("target_component", "Component")
        spring = motion_data.get("spring_physics", {}).get("parameters", {})
        analysis = motion_data.get("spring_physics", {}).get("physics_analysis", {})
        gpu = motion_data.get("gpu_audit", {})
        doppel = motion_data.get("doppelrand_spec", {})

        md = []
        md.append(f"### 9. Đặc Tả Động Lực Học & Chuyển Động (Motion Choreography - {comp})")
        md.append("")
        md.append(f"**Chuyên gia phụ trách:** Julian Garnier & Bret Victor (Agent 13 - Anime.js v4 Motion Lead)")
        md.append("")
        md.append("#### 9.1 Mô Hình Gia Tốc Lò Xo (Spring Physics Parameters)")
        md.append(f"- **Tham số vật lý lò xo:** Bounce = `{spring.get('bounce', 0.2)}`, Duration = `{spring.get('duration', 600)}ms`, Stiffness = `{spring.get('stiffness', 100)}`, Damping = `{spring.get('damping', 20)}`.")
        md.append(f"- **Phân tích cơ học:** Tần số dao động `{analysis.get('angular_frequency_rad_s', 0)} rad/s`, Tỷ số suy giảm `{analysis.get('damping_ratio_zeta', 0)}` ({analysis.get('regime', '')}).")
        md.append(f"- **Cú pháp Anime.js v4:** `spring({{ bounce: {spring.get('bounce', 0.2)}, duration: {spring.get('duration', 600)} }})`")
        md.append("")
        md.append("#### 9.2 Kiến Trúc Vi Thẩm Mỹ Xúc Giác Double-Bezel (Doppelrand)")
        md.append(f"- **Vỏ bọc ngoài (Outer Shell):** Bán kính `{doppel.get('outer_shell', {}).get('border_radius')}`, Viền `{doppel.get('outer_shell', {}).get('border')}`, Đệm `{doppel.get('outer_shell', {}).get('padding')}`.")
        md.append(f"- **Lõi chứa nội dung (Inner Core):** Bán kính `{doppel.get('inner_core', {}).get('border_radius')}`, Highlight viền trong `{doppel.get('inner_core', {}).get('inner_highlight')}`.")
        md.append(f"- **Nút đảo tương tác (Island Button):** `{doppel.get('haptic_physics', {}).get('trailing_icon_island')}`.")
        md.append("")
        md.append("#### 9.3 Kiểm Toán Hiệu Năng 60FPS GPU & Giảm Chuyển Động")
        md.append(f"- **Điểm số kiểm toán GPU-safe:** `{gpu.get('performance_score', 100)}/100` ({'ĐẠT CHUẨN 60FPS' if gpu.get('is_compliant') else 'CẦN KHẮC PHỤC'}).")
        md.append(f"- **Thuộc tính can thiệp:** `{', '.join(gpu.get('gpu_safe_properties', ['transform', 'opacity']))}` (100% nằm trên GPU Compositor Layer, không gây Layout Repaint).")
        md.append(f"- **Hỗ trợ Reduced-Motion:** Tự động fallback sang chuyển động mờ dần 150ms khi người dùng kích hoạt `prefers-reduced-motion` trên hệ điều hành.")
        md.append("")

        return "\n".join(md)
