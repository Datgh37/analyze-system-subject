import { io } from 'socket.io-client';
import { getSocketUrl } from './config';

/**
 * MODULE KẾT NỐI REALTIME SOCKET.IO (MOBILE CLIENT)
 * Quản lý vòng đời kết nối, reconnect tự động và lắng nghe các sự kiện tài chính thời gian thực.
 */

let socket = null;

export const initSocket = (userId = 1, options = {}) => {
  // Nếu socket đã kết nối và đúng userId thì tái sử dụng
  if (socket && socket.connected) {
    return socket;
  }

  // Nếu socket cũ còn tồn tại nhưng chưa ngắt kết nối thì ngắt trước khi tạo mới
  if (socket) {
    socket.disconnect();
    socket = null;
  }

  const socketUrl = getSocketUrl();
  console.log(`[Socket.IO Client] Đang kết nối tới ${socketUrl} (User #${userId})...`);

  socket = io(socketUrl, {
    transports: ['websocket', 'polling'],
    autoConnect: true,
    auth: {
      userId: Number(userId),
      deviceName: options.deviceName || 'Expo Mobile Client'
    },
    reconnection: true,
    reconnectionAttempts: 10,
    reconnectionDelay: 2000,
    timeout: 10000
  });

  socket.on('connect', () => {
    console.log(`🟢 [Socket.IO Client] Kết nối thành công! Socket ID: ${socket.id}`);
  });

  socket.on('disconnect', (reason) => {
    console.log(`🔴 [Socket.IO Client] Đã ngắt kết nối: ${reason}`);
  });

  socket.on('connect_error', (error) => {
    console.warn(`⚠️ [Socket.IO Client] Lỗi kết nối Socket tới ${socketUrl}: ${error.message}`);
  });

  return socket;
};

export const getSocket = () => socket;

export const disconnectSocket = () => {
  if (socket) {
    socket.disconnect();
    socket = null;
    console.log('[Socket.IO Client] Đã chủ động đóng kết nối Socket.');
  }
};
