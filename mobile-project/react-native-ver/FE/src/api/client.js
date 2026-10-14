import axios from 'axios';
import AsyncStorage from '@react-native-async-storage/async-storage';
import { API_CONFIG } from './config';

/**
 * AXIOS HTTP CLIENT TỰ ĐỘNG GẮN BEARER TOKEN
 * - Tự động đọc JWT Token từ AsyncStorage và đính kèm vào Header
 * - Triệt tiêu lỗi quên truyền token
 * - Bắt mã 401 Unauthorized để chuyển hướng đăng nhập
 * 
 * Tác giả: Trịnh Thành Đạt (Lead) & Trần Trọng Đại
 */

export const apiClient = axios.create({
  baseURL: API_CONFIG.BASE_URL,
  timeout: API_CONFIG.TIMEOUT_MS,
  headers: API_CONFIG.HEADERS
});

// 1. Request Interceptor: Luôn cập nhật dynamic baseURL và kẹp Bearer Token
apiClient.interceptors.request.use(
  async (config) => {
    try {
      // Đảm bảo baseURL luôn bám sát IP máy chủ Metro động
      if (typeof API_CONFIG.getBaseUrl === 'function') {
        config.baseURL = API_CONFIG.getBaseUrl();
      }

      const token = await AsyncStorage.getItem('@auth_token');
      if (token) {
        config.headers.Authorization = `Bearer ${token}`;
      }
    } catch (e) {
      console.warn('[API Client] Không thể lấy token từ AsyncStorage:', e);
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// 2. Response Interceptor: Chuẩn hóa dữ liệu trả về và xử lý lỗi phiên đăng nhập
apiClient.interceptors.response.use(
  (response) => {
    // Trả về trực tiếp payload từ backend (status, message, data)
    return response.data;
  },
  async (error) => {
    if (error.response) {
      const { status, data } = error.response;

      // Nếu bị từ chối 401: Token hết hạn hoặc không hợp lệ -> Xóa phiên đăng nhập
      if (status === 401) {
        console.warn('[API Client] Phiên làm việc hết hạn hoặc bị từ chối (HTTP 401).');
        await AsyncStorage.multiRemove(['@auth_token', '@user_profile']);
        // Trả lỗi kèm thông điệp rõ ràng để AuthContext xử lý đưa về Login
      }

      const errorMessage = data?.message || `Lỗi máy chủ (${status})`;
      return Promise.reject(new Error(errorMessage));
    } else if (error.request) {
      return Promise.reject(new Error('Không thể kết nối đến máy chủ WAMP. Vui lòng kiểm tra Wifi hoặc địa chỉ IP.'));
    }

    return Promise.reject(error);
  }
);

export default apiClient;
