/**
 * Hệ thống Design Tokens & Theme dùng chung cho toàn bộ ứng dụng Mobile
 * DNTU Personal Expense Tracker
 * 
 * Quy chuẩn: Sử dụng 100% StyleSheet.create({...}) và hằng số COLORS
 * Không sử dụng thư viện CSS trung gian (Tailwind/NativeWind/Paper) để phục vụ thi vấn đáp.
 * 
 * Phụ trách: Trần Trọng Đại (UI Lead) & Trịnh Thành Đạt (Architecture)
 */

export const COLORS = {
  // Màu thương hiệu chủ đạo (Academic Navy)
  primary: '#2B4C7E',
  primaryLight: '#4A6FA5',
  primaryDark: '#1A3358',

  // Màu biểu thị trạng thái tài chính
  income: '#2EC4B6',      // Xanh ngọc - Thu nhập dương
  expense: '#FF6B6B',     // Đỏ san hô - Chi tiêu âm
  transfer: '#4D96FF',    // Xanh dương - Chuyển khoản nội bộ
  warning: '#FF9F1C',     // Vàng hổ phách - Cảnh báo ngân sách
  danger: '#E63946',      // Đỏ đậm - Báo động vượt hạn mức
  success: '#38B000',     // Xanh lá - An toàn / Hoàn thành

  // Bảng màu nền & bề mặt (Neutral Colors)
  background: '#F8F9FA',  // Nền xám nhạt hiện đại
  surface: '#FFFFFF',     // Nền thẻ trắng (Card surface)
  card: '#FFFFFF',        // Alias thẻ trắng tương thích ngược
  surfaceVariant: '#F1F3F5',

  // Bảng màu chữ (Typography)
  textPrimary: '#1E293B', // Chữ tiêu đề / Số tiền đậm
  text: '#1E293B',        // Alias chữ tiêu đề tương thích ngược
  textSecondary: '#64748B', // Chữ ghi chú / Phụ đề
  textMuted: '#94A3B8',   // Chữ gợi ý mờ (Placeholder)
  textWhite: '#FFFFFF',
  white: '#FFFFFF',       // Alias màu trắng tương thích ngược

  // Đường viền & Phân cách
  border: '#E2E8F0',
  borderLight: '#F1F5F9',
  borderDark: '#CBD5E1',

  // Màu phụ trợ danh mục (Category Badges)
  food: '#FF6B6B',
  shopping: '#4ECDC4',
  transport: '#FFE66D',
  bills: '#1A535C',
  house: '#4D96FF',
  education: '#9B5DE5',
  health: '#F15BB5',
  salary: '#2EC4B6',
  bonus: '#38B000',
  other: '#70E000'
};

export const SPACING = {
  xs: 4,
  sm: 8,
  md: 16,
  lg: 24,
  xl: 32,
  xxl: 48
};

export const SIZES = {
  // Alias khoảng cách tương thích ngược
  xs: 4,
  sm: 8,
  md: 16,
  lg: 24,
  xl: 32,
  fontXs: 12,
  fontSm: 14,
  fontMd: 16,
  fontLg: 18,
  fontXl: 22,
  fontXxl: 28,
  fontDisplay: 34,
  radiusSm: 8,
  radiusMd: 12,
  radiusLg: 16,
  radiusFull: 999
};

export const SHADOWS = {
  small: {
    shadowColor: '#000',
    shadowOffset: { width: 0, height: 1 },
    shadowOpacity: 0.05,
    shadowRadius: 2.22,
    elevation: 2
  },
  card: {
    shadowColor: '#1E293B',
    shadowOffset: { width: 0, height: 2 },
    shadowOpacity: 0.08,
    shadowRadius: 4,
    elevation: 3
  },
  floating: {
    shadowColor: '#2B4C7E',
    shadowOffset: { width: 0, height: 4 },
    shadowOpacity: 0.15,
    shadowRadius: 8,
    elevation: 6
  }
};
