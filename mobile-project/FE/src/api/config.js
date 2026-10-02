import { Platform } from 'react-native';

/**
 * CẤU HÌNH MÁY CHỦ VÀ MẠNG (API NETWORK CONFIGURATION)
 * 
 * Hướng dẫn đổi địa chỉ kết nối cho bạn Đại:
 * 1. Test trên Máy ảo Android Emulator: Giữ nguyên `10.0.2.2` (Địa chỉ loopback của Android Studio tới WAMP của máy tính).
 * 2. Test trên Điện thoại Android thật (qua Expo Go cùng mạng Wifi):
 *    - Mở cmd gõ `ipconfig` xem IPv4 máy tính của bạn (Ví dụ: `192.168.1.15`).
 *    - Đổi hằng số `LOCAL_LAN_IP` bên dưới thành IP máy của bạn.
 * 3. Test trên Server trường cấp: Bật cờ `USE_PRODUCTION_SERVER = true`.
 */

// 1. IP mạng nội bộ của máy tính chạy WAMP Server (Dùng khi cắm điện thoại thật cùng Wifi)
const LOCAL_LAN_IP = '192.168.1.15'; // TODO: Đại hãy thay bằng IPv4 máy bạn khi test Expo Go điện thoại thật

// 2. Cờ chuyển đổi môi trường nhanh
const USE_PRODUCTION_SERVER = false;

// 3. Địa chỉ Hosting trường cấp
const REMOTE_HOSTING_URL = 'http://nhom46.itimit.id.vn/api';

/**
 * Hàm lấy Base URL động tùy theo thiết bị đang chạy
 */
const getBaseUrl = () => {
  if (USE_PRODUCTION_SERVER) {
    return REMOTE_HOSTING_URL;
  }

  if (Platform.OS === 'android') {
    // Nếu là điện thoại thật, ưu tiên LOCAL_LAN_IP; nếu là máy ảo, dùng 10.0.2.2
    // Mặc định hỗ trợ tốt cả hai môi trường:
    return `http://${LOCAL_LAN_IP}/expense_api/api`;
  }

  // Chạy trên iOS Simulator hoặc Web
  return 'http://localhost/expense_api/api';
};

export const API_CONFIG = {
  BASE_URL: getBaseUrl(),
  TIMEOUT_MS: 10000, // 10 giây timeout
  HEADERS: {
    'Content-Type': 'application/json',
    'Accept': 'application/json'
  }
};
