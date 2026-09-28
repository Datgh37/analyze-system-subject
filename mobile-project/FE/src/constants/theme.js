/**
 * HẰNG SỐ MÀU SẮC & GIAO DIỆN DÙNG CHUNG (THEME CONSTANTS)
 * Môn học: Nhập môn Lập trình Thiết bị Di động
 * Tiêu chí: Dùng hằng số const thuần, không dùng thư viện CSS phức tạp
 */

export const COLORS = {
  // Màu sắc chủ đạo (Brand Colors)
  primary: '#2E7D32',      // Xanh lá tài chính (An toàn, tăng trưởng)
  primaryDark: '#1B5E20',  // Xanh lá đậm cho Header
  primaryLight: '#E8F5E9', // Xanh nhạt cho background thẻ ví

  // Màu sắc thể hiện nghiệp vụ (Thu & Chi)
  income: '#388E3C',       // Xanh thu nhập
  expense: '#D32F2F',      // Đỏ chi tiêu
  warning: '#F57C00',      // Cam cảnh báo ngân sách
  danger: '#C62828',       // Đỏ nguy hiểm khi vượt ngân sách

  // Màu trung tính & nền
  background: '#F8F9FA',   // Nền xám nhạt toàn app
  card: '#FFFFFF',         // Nền trắng cho các Card/Khung
  text: '#212121',         // Chữ chính (Đen đậm)
  textSecondary: '#757575',// Chữ phụ (Xám)
  border: '#E0E0E0',       // Đường kẻ viền phân cách
  white: '#FFFFFF',
  black: '#000000'
};

export const SIZES = {
  // Khoảng cách (Padding / Margin)
  xs: 4,
  sm: 8,
  md: 16,
  lg: 24,
  xl: 32,

  // Cỡ chữ (Font Size)
  fontSm: 12,
  fontMd: 14,
  fontLg: 16,
  fontXl: 20,
  fontTitle: 24,

  // Bo góc (Border Radius)
  radiusSm: 6,
  radiusMd: 12,
  radiusLg: 16,
  radiusFull: 999
};
