/**
 * CẤU HÌNH ĐỊA CHỈ API ĐA MÔI TRƯỜNG (MULTI-ENVIRONMENT CONFIG)
 * Môn học: Lập trình Thiết bị Di động
 * Tác giả: Trịnh Thành Đạt (Lead)
 */

// Chuyển đổi môi trường phát triển chỉ bằng 1 dòng:
// - 'WAMP_EMULATOR': Chạy trên máy ảo Android Studio (Loopback 10.0.2.2)
// - 'WAMP_DEVICE': Chạy trên điện thoại thật kết nối chung mạng Wifi
// - 'SERVER_PRODUCTION': Chạy trên máy chủ trường cấp (khi được phép)
export const CURRENT_ENV = 'WAMP_EMULATOR';

const API_ENDPOINTS = {
  // Máy ảo Android Studio trỏ về WAMP trên máy tính qua cổng loopback 10.0.2.2
  WAMP_EMULATOR: 'http://10.0.2.2/expense_api/api',

  // Điện thoại thật: Thay 192.168.1.X bằng địa chỉ IPv4 máy tính của bạn (xem bằng lệnh ipconfig)
  WAMP_DEVICE: 'http://192.168.1.15:80/expense_api/api',

  // Máy chủ production trường cấp
  SERVER_PRODUCTION: 'http://nhom46.itimit.id.vn/api'
};

export const BASE_URL = API_ENDPOINTS[CURRENT_ENV];
