# Anime.js v4 — Easings & Spring Physics Reference

Hướng dẫn tra cứu toàn bộ hệ thống Easing và Spring Physics trích xuất trực tiếp từ tài liệu chính thức [animejs.com/documentation/easings](https://animejs.com/documentation/easings).

---

## 1. Spring Physics (Chuyển Động Lò Xo Vật Lý)

> *"Spring easings provide physics-based motion without rigid timing curves, producing natural recoil and settling behavior."*

Anime.js v4 hỗ trợ khai báo spring physics trực tiếp trong thuộc tính `ease`:

```javascript
import { animate } from 'animejs';

animate('.modal-dialog', {
  scale: [0.85, 1],
  opacity: [0, 1],
  duration: 650,
  ease: 'spring({ bounce: 0.25, duration: 650 })'
});
```

### Các Tham Số Cấu Hình Spring:
- **`bounce`** (`-1` đến `1`, mặc định `0`):
  - `0`: Không nảy (Critically damped - dừng êm không vượt ngưỡng).
  - `0.1` đến `0.3`: Nảy tinh tế, nhẹ nhàng cho UI buttons, cards.
  - `0.4` đến `0.7`: Nảy đàn hồi rõ rệt cho notification popups, game badges.
  - Giá trị âm (`-0.5`): Nén ngược lại trước khi dừng.
- **`duration`** (ms): Thời gian chuyển động tổng thể.
- **`mass`** (mặc định `1`): Khối lượng vật thể (càng nặng càng có đà quán tính lớn).
- **`stiffness`** (mặc định `100`): Độ cứng của lò xo (càng cao phản hồi càng gắt).
- **`damping`** (mặc định `10`): Lực cản giảm chấn (càng cao càng nhanh dừng).

---

## 2. Đường Cong Cubic Bézier (`cubicBezier`)

Dùng khi cần kiểm soát chính xác quỹ đạo gia tốc:

```javascript
ease: 'cubicBezier(0.16, 1, 0.3, 1)' // Curve kinh điển cho giao diện mượt mà (Out Expo)
ease: 'cubicBezier(0.4, 0, 0.2, 1)'  // Chuẩn Material Design Standard
```

---

## 3. Hệ Thống Built-in Eases

Anime.js cung cấp đầy đủ các họ hàm toán học từ nhẹ nhàng đến cực hạn với 3 biến thể: `in` (tăng tốc), `out` (giảm tốc), `inOut` (tăng rồi giảm tốc):

| Nhóm Easing | Tên Hàm (In / Out / InOut) | Đặc Trưng Chuyển Động | Ứng Dụng Khuyên Dùng |
| :--- | :--- | :--- | :--- |
| **Linear** | `linear` | Tốc độ đều, không gia tốc | Vòng quay radar, thanh tiến trình đều |
| **Quad** | `inQuad`, `outQuad`, `inOutQuad` | Bậc 2 nhẹ nhàng | Micro-interactions hover, tooltips |
| **Cubic** | `inCubic`, `outCubic`, `inOutCubic` | Bậc 3 đầm chắc | Mở dropdown, trượt panel |
| **Quart** | `inQuart`, `outQuart`, `inOutQuart` | Bậc 4 dứt khoát | Phóng to ảnh, chuyển slide |
| **Quint** | `inQuint`, `outQuint`, `inOutQuint` | Bậc 5 rất mạnh | Mở modal dialog toàn màn hình |
| **Sine** | `inSine`, `outSine`, `inOutSine` | Mềm mại, sóng hình sin | Hiệu ứng thở (breathing), trôi dạt nhẹ |
| **Expo** | `inExpo`, `outExpo`, `inOutExpo` | Bùng nổ cực hạn | Hiệu ứng vụ nổ, combo số trong game |
| **Circ** | `inCirc`, `outCirc`, `inOutCirc` | Quỹ đạo cung tròn | Xoay chuyển góc, menu tròn |
| **Back** | `inBack`, `outBack`, `inOutBack` | Kéo lùi lại lấy đà rồi vọt | Nút bấm phản hồi cơ học |
| **Elastic** | `inElastic`, `outElastic`, `inOutElastic` | Co giãn cao su nhiều nhịp | Game UI ngộ nghĩnh, loot rơi |
| **Bounce** | `inBounce`, `outBounce`, `inOutBounce` | Rơi đập xuống sàn nhiều nhịp | Rơi rương kho báu, đập bóng |

---

## 4. Steps & Irregular Eases

### 1. `steps(n)` — Bước Nhảy Đoạn Rời Rạc
Hoàn hảo cho sprite-sheet animation, font đếm số điện tử, hoặc retro 8-bit game:
```javascript
animate('.pixel-sprite', {
  backgroundPosition: ['0px', '-400px'],
  duration: 800,
  ease: 'steps(8)' // Chia đều 8 frame rời rạc
});
```

### 2. Irregular Easing
Mô phỏng rung lắc ngẫu nhiên, sấm sét, hoặc động đất trong game:
```javascript
ease: 'irregular(10, { randomness: 0.5 })'
```
