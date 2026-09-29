# Anime.js v4 — Core Modules & Component Architecture

Tài liệu này tổng hợp toàn bộ tư duy thiết kế component, các module cốt lõi và hướng dẫn sử dụng API chính thức trích xuất từ tài liệu [animejs.com/documentation](https://animejs.com/documentation/) và mã nguồn chính thức [github.com/juliangarnier/anime](https://github.com/juliangarnier/anime/tree/master).

---

## 1. Cài Đặt & Import Chuẩn (Package Management)

Khi tích hợp vào dự án khi gọi đến skill này, **CHỈ CẦN DÙNG NPM**, không cần tải hay clone repo github:

```bash
npm i animejs
```

Anime.js v4 chuyển dịch hoàn toàn sang kiến trúc **Named Imports (ES Modules)**, giúp tối ưu Tree-shaking:

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

## 2. Bản Đồ Module Component (Core Module Architecture)

Trong tài liệu chính thức của Anime.js v4, thư viện cung cấp một hệ thống Engine điều khiển trạng thái và component tương tác toàn diện:

```
+---------------------------------------------------------------------------------------+
|                                    ANIME.JS ENGINE                                    |
+-------------------+--------------------+--------------------+-------------------------+
| COMPONENT SCOPING | LAYOUT MORPHING    | TOUCH & DRAG       | REACTIVE ANIMATABLES    |
| `createScope()`   | `createLayout()`   | `createDraggable()`| `createAnimatable()`    |
| - DOM Encap       | - FLIP layout      | - Pointer physics  | - Cursor tracking       |
| - Media queries   | - Flex/Grid morph  | - Snapping & bounds| - High-freq game loop   |
| - Batch revert    | - DOM reordering   | - Inertia          | - No garbage collector  |
+-------------------+--------------------+--------------------+-------------------------+
| TIMING & SYNC     | CHOREOGRAPHY       | SCROLL TRIGGERS    | GENERATIVE UTILITIES    |
| `createTimer()`   | `createTimeline()` | `onScroll()`       | `stagger()`, `lerp()`   |
| - Precision delta | - Add / offsets    | - ScrollObserver   | - `clamp()`, `snap()`   |
| - Replaces setInterval- Labels & loop  | - Autoplay sync    | - `mapRange()`, `damp()`|
+-------------------+--------------------+--------------------+-------------------------+
```

---

## 3. Chi Tiết Các Module Thành Phần

### Module 1: `createScope()` — Đóng Gói Component & Tự Động Revert

> *"Anime.js instances declared inside a Scope can react to media queries, use custom root elements, share default parameters, and be reverted in batch, streamlining work in responsive and component-based environments."*

Đây là module quan trọng nhất khi tích hợp Anime.js vào các framework hiện đại (React, Vue, hoặc Vanilla Web Components) để **ngăn ngừa rò rỉ bộ nhớ (memory leaks)** và quản lý vòng đời (lifecycle):

```javascript
import { createScope } from 'animejs';

// Khởi tạo trong component mount hoặc class
export function initCharacterCard(containerEl) {
  const scope = createScope({
    root: containerEl, // Giới hạn selector trong container này
    mediaQueries: {
      isDesktop: '(min-width: 900px)',
      isMobile: '(max-width: 899px)',
    }
  });

  scope.add((self) => {
    // Tự động phân nhánh theo responsive
    if (self.matches.isDesktop) {
      scope.animate('.card-avatar', {
        scale: [0.9, 1],
        translateY: [20, 0],
        duration: 500,
        ease: 'spring({ bounce: 0.2 })'
      });
    }

    scope.animate('.stat-bar', {
      scaleX: [0, 1],
      delay: scope.stagger(50),
      duration: 600
    });
  });

  // HÀM DỌN DẸP BẮT BUỘC (Component Unmount / Route Change)
  return () => {
    scope.revert(); // Khôi phục trạng thái DOM ban đầu và huỷ mọi instances trong batch
  };
}
```

---

### Module 2: `createLayout()` — FLIP Layout Morphing Không Tưởng

> *"Automatically animates between two HTML layout states, allowing you to easily animate properties that are normally impossible or hard to animate like CSS display, flex direction, grid settings, and DOM order."*

Giải quyết bài toán nan giải nhất của CSS: chuyển đổi giữa các trạng thái layout (như đổi từ dạng Grid sang List, thay đổi `flex-direction`, reorder card, hoặc mở rộng item toàn màn hình):

```javascript
import { createLayout } from 'animejs';

const layout = createLayout('.inventory-container', {
  duration: 450,
  ease: 'cubicBezier(0.2, 0.8, 0.2, 1)'
});

// Khi thay đổi class CSS hoặc đảo vị trí các item trong DOM:
function toggleInventoryLayout(mode) {
  layout.record(); // Ghi nhớ vị trí cũ trước khi đổi state
  
  if (mode === 'grid') {
    inventoryEl.classList.add('grid-layout');
    inventoryEl.classList.remove('list-layout');
  } else {
    inventoryEl.classList.add('list-layout');
    inventoryEl.classList.remove('grid-layout');
  }

  layout.animate(); // Tự động tính toán delta FLIP và animate mượt mà!
}
```

---

### Module 3: `createDraggable()` — Điều Khiển Kéo Thả & Vật Lý Xúc Giác

> *"Adds draggable capabilities to DOM Elements with snapping, constraints, and physics."*

Dùng cho thanh gạt âm lượng, xoay la bàn, cửa sổ nổi, map di chuyển:

```javascript
import { createDraggable } from 'animejs';

export function setupWheelDraggable(element, onAngleChange) {
  return createDraggable(element, {
    trigger: '.drag-handle', // Phần tử nhận sự kiện kéo
    axis: 'x',              // 'x', 'y', hoặc 'all'
    bounds: { minX: -200, maxX: 200 },
    snap: 20,               // Bước nhảy răng cưa 20px cơ khí
    onUpdate: (self) => {
      onAngleChange(self.x);
    },
    onSettle: (self) => {
      console.log('Phần tử đã dừng tại:', self.x);
    }
  });
}
```

---

### Module 4: `createAnimatable()` — Tối Ưu Cho Con Trỏ & Vòng Lặp Tần Số Cao

> *"Efficiently animates target properties, making it an ideal replacement for animate() and utils.set() in situations where values change frequently, such as cursor events or animation loops."*

Khi người dùng lia chuột hoặc trong game loop liên tục 60fps, gọi `animate()` thông thường sẽ liên tục tạo object mới và gây gánh nặng cho Garbage Collector (rác bộ nhớ). `createAnimatable()` duy trì một đối tượng trạng thái thường trực để thay đổi mục tiêu tức thì:

```javascript
import { createAnimatable } from 'animejs';

const cursorTracker = createAnimatable('.custom-cursor', {
  x: { duration: 150, ease: 'linear' },
  y: { duration: 150, ease: 'linear' },
  scale: { duration: 250, ease: 'spring({ bounce: 0.3 })' }
});

// Bắt sự kiện mousemove mà không sợ giật lag DOM
window.addEventListener('pointermove', (e) => {
  cursorTracker.x(e.clientX);
  cursorTracker.y(e.clientY);
});
```

---

### Module 5: `createTimer()` — Đồng Hồ Chính Xác & Thay Thế setInterval

> *"Schedules and controls timed callbacks that can be used as an alternative to setTimeout() or setInterval(), keeping animations and callbacks synchronized."*

```javascript
import { createTimer } from 'animejs';

const cooldownTimer = createTimer({
  frameRate: 60,
  loop: 5,
  duration: 1000,
  onUpdate: (self) => {
    progressBar.style.width = `${self.progress * 100}%`;
  },
  onLoop: (self) => {
    console.log(`Đã hoàn thành vòng ${self.currentIteration}`);
  },
  onComplete: () => {
    console.log('Kỹ năng đã hồi xong!');
  }
});
```

---

### Module 6: `animate()` & `createTimeline()` — Core Tween Engine & Biên Đạo

```javascript
import { animate, createTimeline } from 'animejs';

// Tween độc lập với các thuộc tính CSS Transform tách biệt
animate('.hero-badge', {
  x: [50, 0],
  scale: [0.8, 1],
  opacity: [0, 1],
  duration: 400,
  ease: 'outQuad'
});

// Biên đạo dòng thời gian đa mục tiêu
const tl = createTimeline({
  defaults: { ease: 'cubicBezier(0.2, 0.8, 0.2, 1)' }
});

tl.add('.title', { y: [-30, 0], opacity: [0, 1], duration: 400 })
  .add('.content', { opacity: [0, 1], duration: 300 }, '-=150')
  .add('.cta-btn', { scale: [0.9, 1], opacity: [0, 1], duration: 250 }, '-=100');
```

---

### Module 7: `onScroll()` — ScrollObserver Đồng Bộ Hóa

> *"Triggers and synchronises Timer, Animation and Timeline instances on scroll."*

```javascript
import { animate, onScroll } from 'animejs';

animate('.cinematic-hero-card', {
  scale: [0.8, 1],
  opacity: [0, 1],
  autoplay: onScroll({
    target: '.scroll-container',
    enter: 'top top',
    leave: 'bottom bottom',
    sync: true // Scroll đến đâu animation chạy đến đó (scrubbing)
  })
});
```
