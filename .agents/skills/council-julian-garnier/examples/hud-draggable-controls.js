/**
 * Ví dụ 2: Tạo điều khiển Game HUD tương tác (Kéo thả, Đo đếm thời gian, Tracker phản hồi)
 * Sử dụng createDraggable(), createTimer(), createAnimatable()
 */

import { createDraggable, createTimer, createAnimatable, snap, clamp } from 'animejs';

// 1. Thanh trượt điều chỉnh chỉ số cơ khí với bước nhảy bánh răng (Snap)
export function initGearSlider(handleElement, onValueChange) {
  return createDraggable(handleElement, {
    axis: 'x',
    bounds: { minX: 0, maxX: 200 },
    snap: 20, // Khóa vào từng nấc 20px
    onUpdate: (self) => {
      const percentage = Math.round((self.x / 200) * 100);
      onValueChange(percentage);
    }
  });
}

// 2. Bộ đếm nhịp hồi chiêu (Cooldown Timer) thay thế setInterval
export function startCooldownTimer(durationMs, onTick, onReady) {
  return createTimer({
    duration: durationMs,
    frameRate: 60,
    onUpdate: (self) => {
      // self.progress chạy mượt từ 0 đến 1
      onTick(self.progress);
    },
    onComplete: () => {
      onReady();
    }
  });
}

// 3. Con trỏ ngắm bắn / Radar Tracker liên tục 60fps không sinh rác bộ nhớ
export function initCrosshairTracker(crosshairElement) {
  const tracker = createAnimatable(crosshairElement, {
    x: { duration: 120, ease: 'linear' },
    y: { duration: 120, ease: 'linear' },
    scale: { duration: 200, ease: 'spring({ bounce: 0.3 })' }
  });

  function onPointerMove(e) {
    tracker.x(e.clientX);
    tracker.y(e.clientY);
  }

  function onPointerDown() {
    tracker.scale(1.3);
  }

  function onPointerUp() {
    tracker.scale(1.0);
  }

  window.addEventListener('pointermove', onPointerMove);
  window.addEventListener('pointerdown', onPointerDown);
  window.addEventListener('pointerup', onPointerUp);

  return () => {
    window.removeEventListener('pointermove', onPointerMove);
    window.removeEventListener('pointerdown', onPointerDown);
    window.removeEventListener('pointerup', onPointerUp);
  };
}
