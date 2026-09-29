---
name: animejs-uiux
description: >-
  Chuyên gia Thư viện Anime.js v4 & UI/UX Motion Engineering.
  Kích hoạt khi cần: xây dựng component tương tác, chuyển động giao diện (motion UI),
  quản lý vòng đời hoạt ảnh (createScope), layout morphing (createLayout), kéo thả vật lý (createDraggable),
  đồng bộ thời gian (createTimer), spring physics, staggered grid, hoặc tối ưu hóa hiệu năng animation 60fps theo chuẩn tài liệu chính thức animejs.com/documentation.
---

# Anime.js v4 Specialist — Motion & UI/UX Component Engineering

Skill chuyên biệt trang bị cho Agent năng lực làm chủ toàn diện **Anime.js v4** — thư viện hoạt ảnh và điều khiển tương tác giao diện hiện đại, trích xuất hoàn toàn từ tài liệu chính thức [animejs.com/documentation](https://animejs.com/documentation/) và mã nguồn [github.com/juliangarnier/anime](https://github.com/juliangarnier/anime/tree/master).

---

## 1. Cài Đặt & Quản Lý Gói (Package Management)

Khi áp dụng vào bất kỳ dự án web/game nào, **CHỈ CẦN CÀI QUA NPM**, tuyệt đối không cần tải hay clone repo GitHub:

```bash
npm i animejs
```

Anime.js v4 sử dụng cấu trúc **Named Imports (ES Modules)**, hỗ trợ Tree-shaking tối ưu:

```javascript
import { 
  animate, 
  createTimeline, 
  createTimer, 
  createDraggable, 
  createScope, 
  createLayout, 
  createAnimatable, 
  stagger 
} from 'animejs';
```

---

## 2. Hệ Thống Module & Tư Duy Thiết Kế Component (Core Architecture)

Khác với các công cụ CSS tween thông thường, Anime.js v4 được xây dựng như một **hệ điều hành tương tác (Interaction Engine)** chia thành các module chuyên biệt:

```
+---------------------------------------------------------------------------------------+
|                                    ANIME.JS V4 ENGINE                                 |
+-------------------+--------------------+--------------------+-------------------------+
| COMPONENT SCOPING | LAYOUT MORPHING    | TOUCH & DRAG       | REACTIVE ANIMATABLES    |
| `createScope()`   | `createLayout()`   | `createDraggable()`| `createAnimatable()`    |
| - DOM Encap       | - FLIP layout      | - Pointer physics  | - Cursor tracking       |
| - Media queries   | - Flex/Grid morph  | - Snapping & bounds| - High-freq game loop   |
| - Batch revert    | - DOM reordering   | - Momentum inertia | - Zero memory allocation|
+-------------------+--------------------+--------------------+-------------------------+
| TIMING & SYNC     | CHOREOGRAPHY       | SCROLL TRIGGERS    | GENERATIVE UTILITIES    |
| `createTimer()`   | `createTimeline()` | `onScroll()`       | `stagger()`, `lerp()`   |
| - Precision delta | - Chained offsets  | - ScrollObserver   | - `clamp()`, `snap()`   |
| - Replaces setInterval- Labels & loop  | - Autoplay scrub   | - `mapRange()`, `damp()`|
+-------------------+--------------------+--------------------+-------------------------+
```

---

## 3. Bản Đồ Tài Liệu Kỹ Thuật (References & Documentation)

Khi triển khai giải pháp kỹ thuật, Agent **PHẢI tra cứu** các tài liệu chuyên sâu tương ứng:

- [Core Modules & Component Architecture](./references/core-modules.md): Hướng dẫn chi tiết từng module chính thức (`createScope`, `createLayout`, `createDraggable`, `createAnimatable`, `createTimer`, `animate`, `createTimeline`, `onScroll`) kèm cú pháp chuẩn.
- [Easings & Spring Physics](./references/easings-and-physics.md): Danh mục toàn bộ các đường cong gia tốc, cách thiết lập thông số Spring Physics (`bounce`, `mass`, `stiffness`, `damping`), `cubicBezier`, và `steps`.
- [Utilities, Math & Global Engine](./references/utilities-and-engine.md): Hệ thống phân phối `stagger()`, các hàm toán học hỗ trợ game/UI (`clamp`, `lerp`, `damp`, `mapRange`, `snap`), DOM helpers, và cấu hình `engine` toàn cục.
- [Ví Dụ 1: Component Scope & Layout Lifecycle](./examples/component-lifecycle-scope.js): Mẫu code thực tế quản lý vòng đời component và FLIP layout.
- [Ví Dụ 2: HUD Draggable & Timer Controls](./examples/hud-draggable-controls.js): Mẫu code điều khiển thanh trượt cơ khí, bộ đếm hồi chiêu và con trỏ ngắm bắn.

---

## 4. Quy Trình 5 Bước Triển Khai Hoạt Ảnh Chuẩn (5-Step Protocol)

### Bước 1: Lựa Chọn Đúng Module Cho Vấn Đề
- Cần quản lý vòng đời component và responsive? → Dùng **`createScope()`**.
- Cần chuyển đổi giữa Grid và List, hoặc reorder DOM? → Dùng **`createLayout()`**.
- Cần thanh trượt, xoay la bàn, kéo thả? → Dùng **`createDraggable()`**.
- Cần theo dõi con trỏ chuột liên tục hoặc vòng lặp game? → Dùng **`createAnimatable()`**.
- Cần đếm giờ hồi chiêu, delay đồng bộ? → Dùng **`createTimer()`**.
- Cần chuỗi hoạt ảnh xuất hiện nhiều phần tử? → Dùng **`createTimeline()`** + **`stagger()`**.

### Bước 2: Thiết Lập Phạm Vi & Vòng Đời (Lifecycle Setup)
- Luôn khởi tạo hoạt ảnh bên trong `createScope()` đối với các component framework (React, Vue, Web Components).
- Định nghĩa các media queries trực tiếp trong scope để tự động phân nhánh logic trên Mobile / Desktop.

### Bước 3: Áp Dụng Gia Tốc Phù Hợp (Easing & Physics Selection)
- **Tương tác nút bấm / Hover:** `outQuad` hoặc `cubicBezier(0.16, 1, 0.3, 1)` với thời lượng nhanh (100ms - 150ms).
- **Xuất hiện Card / Dialog / Panel:** Spring physics `spring({ bounce: 0.2, duration: 600 })` để tạo độ nảy tự nhiên.
- **Danh sách / Lưới vật phẩm:** Dùng `stagger()` với `grid: [cols, rows]` và `from: 'center'` hoặc `from: 'first'`.

### Bước 4: Tối Ưu Hóa Hiệu Năng GPU (60FPS Performance)
- **Chỉ Animate Transform & Opacity:** Sử dụng các thuộc tính độc lập của Anime.js (`x`, `y`, `scale`, `rotateZ`, `opacity`).
- Tránh tuyệt đối animate `width`, `height`, `top`, `left`, `margin` vì sẽ gây Layout Thrashing và tụt khung hình.

### Bước 5: Kiểm Tra Dọn Dẹp (Cleanup & Reduced Motion)
- **Bắt buộc dọn dẹp:** Luôn gọi `scope.revert()` khi component unmount để hoàn trả DOM về trạng thái sạch sẽ và ngắt các frame loop.
- **Tôn trọng Reduced-Motion:** Tự động fallback tức thì khi người dùng bật chế độ giảm chuyển động trong hệ điều hành (`prefers-reduced-motion`).

---

## 5. Những Điều Cấm Kỵ Tuyệt Đối (Anti-Patterns)

- ❌ **Không dùng `setInterval` / `setTimeout` cho logic hoạt ảnh:** Luôn dùng `createTimer()` để đảm bảo nhịp đếm đồng bộ với frame rate của Anime.js Engine.
- ❌ **Không quên revert scope khi unmount:** Việc bỏ quên `scope.revert()` sẽ giữ lại các instance ngầm trong bộ nhớ và gây memory leak nghiêm trọng.
- ❌ **Không gọi liên tục `animate()` trong vòng lặp tần số cao (mousemove, game loop):** Khi giá trị thay đổi liên tục, bắt buộc dùng `createAnimatable()` để tránh áp lực rác bộ nhớ lên Garbage Collector.
- ❌ **Không animate các thuộc tính kích hoạt Layout Repaint:** Không can thiệp `margin`, `padding`, `top`, `left` trong các animation lặp lại; luôn ưu tiên `x`, `y`, `scale`, `rotate`.
- ❌ **Không lạm dụng thời gian chuyển động lê thê:** Micro-interactions vượt quá 300ms gây cảm giác giao diện bị trễ (lag). Giữ chuyển động phản hồi ở mức 100ms - 200ms.
