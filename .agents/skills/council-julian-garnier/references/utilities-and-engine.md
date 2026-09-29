# Anime.js v4 — Utilities, Math & Engine Architecture

Tài liệu tra cứu hệ thống tiện ích phân phối (`stagger`), hàm toán học hỗ trợ game/UI, phương thức thao tác DOM và cấu hình Engine toàn cục trích xuất trực tiếp từ [animejs.com/documentation/utilities](https://animejs.com/documentation/utilities) và [animejs.com/documentation/engine](https://animejs.com/documentation/engine).

---

## 1. Hệ Thống Phân Phối Chuỗi `stagger()`

> *"Creates sequential effects by distributing values progressively across multiple targets."*

`stagger()` là công cụ kinh điển của Anime.js giúp tạo hiệu ứng domino xuất hiện cho danh sách, menu, ô item hoặc hạt hiệu ứng:

```javascript
import { animate, stagger } from 'animejs';

// 1. Phân phối delay cơ bản
animate('.list-item', {
  opacity: [0, 1],
  y: [20, 0],
  delay: stagger(40) // Mỗi item cách nhau 40ms
});

// 2. Phân phối theo dải giá trị (Range Values)
animate('.bar-column', {
  height: stagger(['20px', '200px']), // Item đầu 20px, item cuối 200px
  duration: 500
});

// 3. Phân phối lưới 2 chiều (2D Grid Stagger)
animate('.grid-cell', {
  scale: [0, 1],
  delay: stagger(50, {
    grid: [6, 4],     // 6 cột x 4 hàng
    from: 'center',   // Lan tỏa từ tâm (hỗ trợ: 'first', 'last', 'center', hoặc index cụ thể)
    axis: 'x',        // Chỉ lan tỏa theo trục hoành
    ease: 'outQuad'   // Gia tốc phân phối khoảng cách thời gian
  })
});

// 4. Jitter (Độ lệch ngẫu nhiên)
animate('.particle', {
  scale: [0, 1],
  delay: stagger(30, {
    start: 200,       // Bắt đầu sau 200ms
    jitter: 15        // Dao động ngẫu nhiên +/- 15ms tạo cảm giác tự nhiên
  })
});
```

---

## 2. Tiện Ích Thao Tác DOM & Trạng Thái

Thư viện cung cấp bộ helper tích hợp sẵn, loại bỏ sự phụ thuộc vào các thư viện bên ngoài:

```javascript
import { $, get, set, cleanInlineStyles, remove, sync } from 'animejs';

// $() — Selector nhanh trả về Element hoặc Array
const primaryBtn = $('.btn-primary');
const allCards = $$('.card-item');

// set() — Gán giá trị tức thì qua Anime.js transform matrix
set('.hero-title', {
  x: 100,
  opacity: 0
});

// get() — Đọc giá trị hiện tại (bao gồm cả transform nội bộ)
const currentScale = get('.hero-title', 'scale');

// cleanInlineStyles() — Xóa sạch các thuộc tính inline do animation tạo ra sau khi hoàn tất
cleanInlineStyles('.hero-title'); // Trả lại quyền kiểm soát thuần túy cho file CSS

// remove() — Hủy bỏ triệt để một target khỏi mọi animation đang chạy
remove('.enemy-boss');
```

---

## 3. Bộ Hàm Toán Học Game & UI (Math Utilities)

Được thiết kế tối ưu hóa tốc độ, không phân bổ bộ nhớ thừa (zero memory allocation):

```javascript
import { 
  clamp, 
  lerp, 
  damp, 
  mapRange, 
  snap, 
  round, 
  random, 
  createSeededRandom, 
  randomPick, 
  shuffle 
} from 'animejs';

// clamp(val, min, max): Giới hạn trong khoảng an toàn
const hp = clamp(rawHp, 0, 100);

// lerp(start, end, progress): Nội suy tuyến tính mượt mà
const smoothCameraX = lerp(camX, targetX, 0.05);

// damp(current, target, smoothing, delta): Nội suy chống phụ thuộc frame rate
const stabilizedPos = damp(currentPos, targetPos, 10, deltaTime);

// mapRange(val, inMin, inMax, outMin, outMax): Chuyển đổi thang đo
const volumePercent = mapRange(sliderValue, 0, 1024, 0, 100);

// snap(val, step): Khóa vào bước nhảy cơ khí
const gridCoordinate = snap(mousePos, 32); // Khóa vào ô 32px

// random(min, max): Số ngẫu nhiên an toàn
const damage = random(45, 90);

// randomPick(array) & shuffle(array): Chọn ngẫu nhiên hoặc tráo bài
const randomReward = randomPick(['Gold', 'Elixir', 'Sword']);
const shuffledDeck = shuffle([...cardDeck]);

// createSeededRandom(seed): Sinh số ngẫu nhiên theo hạt giống (cho Procedural Game World)
const seededRng = createSeededRandom(12345);
const value1 = seededRng(); // Luôn nhất quán cùng kết quả
```

---

## 4. Quản Lý Engine Toàn Cục (Engine Parameters & Methods)

Toàn bộ các thực thể `Timer`, `Animation`, `Timeline`, `Draggable` đều kết nối tới một Engine trung tâm duy nhất:

```javascript
import { engine } from 'animejs';

// 1. Tốc độ thời gian toàn cục (Slow-motion / Fast-forward)
engine.speed = 0.5; // Chạy chậm 50% (hiệu ứng trúng đòn / pause menu)
engine.speed = 2.0; // Tăng tốc gấp đôi

// 2. Khóa FPS tối đa
engine.fps = 60; // Mặc định tự thích ứng màn hình (120Hz/144Hz)

// 3. Tạm dừng toàn bộ hoạt ảnh khi tab trình duyệt bị ẩn (tiết kiệm pin & CPU)
engine.pauseOnDocumentHidden = true; // Mặc định là true

// 4. Dừng / Tiếp tục toàn bộ hệ thống
engine.pause();  // Đóng băng toàn bộ hoạt ảnh trong app/game
engine.resume(); // Tiếp tục chạy mượt mà
```

---

## 5. Web Animations API (WAAPI) Integration

Anime.js v4 hỗ trợ sử dụng engine WAAPI của trình duyệt để đạt hiệu năng phần cứng tối đa (Hardware Accelerated Compositor Thread) khi không cần các tính năng phức tạp của JavaScript:

```javascript
import { animate } from 'animejs';

// Kích hoạt WAAPI khi chỉ animate thuộc tính transform/opacity đơn giản
animate('.simple-spinner', {
  rotate: 360,
  duration: 1000,
  loop: true,
  ease: 'linear',
  useWAAPI: true // Chạy trực tiếp trên compositor thread của trình duyệt
});
```
