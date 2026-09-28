/**
 * AXIOS HTTP CLIENT TẬP TRUNG
 * Tự động gắn Base URL và xử lý lỗi phản hồi
 */

import axios from 'axios';
import { BASE_URL } from './config';

const apiClient = axios.create({
  baseURL: BASE_URL,
  timeout: 10000,
  headers: {
    'Content-Type': 'application/json',
    'Accept': 'application/json'
  }
});

// Interceptor xử lý phản hồi
apiClient.interceptors.response.use(
  (response) => {
    return response.data;
  },
  (error) => {
    let errorMsg = 'Lỗi kết nối đến máy chủ.';
    if (error.response && error.response.data && error.response.data.message) {
      errorMsg = error.response.data.message;
    } else if (error.message) {
      errorMsg = error.message;
    }
    return Promise.reject(new Error(errorMsg));
  }
);

export default apiClient;
