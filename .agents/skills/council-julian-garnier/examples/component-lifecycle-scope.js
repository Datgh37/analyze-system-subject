/**
 * Ví dụ 1: Quản lý vòng đời Component với createScope() và createLayout() trong Anime.js v4
 * Phù hợp tích hợp vào React (useEffect), Vue (onMounted), hoặc Vanilla Web Components.
 */

import { createScope, createLayout, stagger } from 'animejs';

export function setupInventoryPanel(containerEl) {
  // 1. Khởi tạo Scope bọc toàn bộ component
  const scope = createScope({
    root: containerEl,
    mediaQueries: {
      isDesktop: '(min-width: 900px)',
      isMobile: '(max-width: 899px)',
    }
  });

  // 2. Khởi tạo FLIP Auto-Layout để chuyển đổi giao diện mượt mà
  const layout = createLayout(containerEl, {
    duration: 400,
    ease: 'cubicBezier(0.16, 1, 0.3, 1)'
  });

  // 3. Định nghĩa các hoạt ảnh bên trong Scope
  scope.add((self) => {
    // Entrance animation khi mở bảng đồ
    scope.animate('.inventory-header', {
      y: [-20, 0],
      opacity: [0, 1],
      duration: 350,
      ease: 'outQuad'
    });

    // Xuất hiện các ô đồ theo lưới 2 chiều
    scope.animate('.item-slot', {
      scale: [0.8, 1],
      opacity: [0, 1],
      delay: stagger(30, {
        grid: [5, 4],
        from: 'first',
        ease: 'outQuad'
      }),
      duration: 400
    });
  });

  // 4. Hàm chuyển đổi bố cục Grid <-> List
  function toggleLayoutMode(isGrid) {
    layout.record(); // Ghi nhớ bounding rect của mọi phần tử
    
    if (isGrid) {
      containerEl.classList.add('grid-mode');
      containerEl.classList.remove('list-mode');
    } else {
      containerEl.classList.add('list-mode');
      containerEl.classList.remove('grid-mode');
    }

    layout.animate(); // Anime.js tự tính delta FLIP và animate mượt mà
  }

  // 5. TRẢ VỀ HÀM TEARDOWN BẮT BUỘC ĐỂ CLEANUP
  return {
    toggleLayoutMode,
    destroy: () => {
      scope.revert(); // Hoàn trả DOM về nguyên bản, ngắt mọi RAF loop & listener, chống rò rỉ bộ nhớ
    }
  };
}
