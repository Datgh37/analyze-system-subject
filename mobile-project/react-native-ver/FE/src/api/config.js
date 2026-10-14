import { Platform, NativeModules } from 'react-native';


// 1. IP mạng nội bộ dự phòng (Fallback khi không đọc được scriptURL)
const LOCAL_LAN_IP = '192.168.1.9'; 

/**
 * Hàm tự động nhận diện Hostname/IP của máy tính đang chạy Metro Bundler
 */
export const getAutoHost = () => {
  if (Platform.OS === 'web') {
    if (typeof window !== 'undefined' && window.location?.hostname) {
      return window.location.hostname;
    }
    return 'localhost';
  }

  // Tự động lấy IP từ Metro Bundler script URL khi chạy trên Expo Go / Android / iOS
  const scriptURL = NativeModules.SourceCode?.scriptURL;
  if (scriptURL) {
    const match = scriptURL.match(/https?:\/\/([^:\/]+)/);
    if (match && match[1]) {
      return match[1];
    }
  }

  // Fallback nếu không xác định được scriptURL
  if (Platform.OS === 'android') {
    return LOCAL_LAN_IP;
  }

  return 'localhost';
};

/**
 * Hàm lấy Base URL động tùy theo thiết bị đang chạy (Local WAMP)
 */
export const getBaseUrl = () => {
  const host = getAutoHost();
  return `http://${host}/expense_api/api`;
};

export const getSocketUrl = () => {
  const host = getAutoHost();
  return `http://${host}:3001`;
};

export const API_CONFIG = {
  getBaseUrl,
  getSocketUrl,
  BASE_URL: getBaseUrl(),
  SOCKET_URL: getSocketUrl(),
  TIMEOUT_MS: 10000, // 10 giây timeout
  HEADERS: {
    'Content-Type': 'application/json',
    'Accept': 'application/json'
  }
};

