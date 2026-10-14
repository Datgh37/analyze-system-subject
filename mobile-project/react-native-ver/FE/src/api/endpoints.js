import apiClient from './client';

/**
 * DANH MỤC TẬP TRUNG TOÀN BỘ 21 RESTFUL API CỦA HỆ THỐNG
 * Được ánh xạ 1:1 tương thích chuẩn xác với Backend PHP WAMP
 * 
 * Đại chỉ cần import hàm cần dùng ở bất kỳ màn hình nào, không cần gõ URL thủ công!
 * Ví dụ:
 *   import { getWallets, createTransaction } from '../api/endpoints';
 *   const data = await getWallets();
 */

export const api = {
  // ============================================================================
  // 1. PHÂN HỆ XÁC THỰC & HỒ SƠ NGƯỜI DÙNG (AUTH & PROFILE)
  // ============================================================================
  
  /** 1. Đăng ký tài khoản mới */
  register: (fullName, email, password) => 
    apiClient.post('/auth/register.php', { full_name: fullName, email, password }),

  /** 2. Đăng nhập và nhận Bearer JWT Token */
  login: (email, password) => 
    apiClient.post('/auth/login.php', { email, password }),

  /** 3. Bước 1 Quên mật khẩu: Yêu cầu gửi mã OTP 6 số */
  forgotPassword: (email) => 
    apiClient.post('/auth/forgot_password.php', { email }),

  /** 4. Bước 2 Quên mật khẩu: Xác thực OTP và nhận reset_token */
  verifyOtp: (email, otpCode) => 
    apiClient.post('/auth/verify_otp.php', { email, otp_code: otpCode }),

  /** 5. Bước 3 Quên mật khẩu: Đặt lại mật khẩu mới */
  resetPassword: (email, resetToken, newPassword) => 
    apiClient.post('/auth/reset_password.php', { email, reset_token: resetToken, new_password: newPassword }),

  /** 6. Lấy thông tin hồ sơ & thống kê cá nhân (JWT Bearer) */
  getProfile: () => 
    apiClient.get('/auth/profile.php'),

  /** 7. Cập nhật họ tên hiển thị hoặc đổi mật khẩu (JWT Bearer) */
  updateProfile: ({ fullName, oldPassword, newPassword }) => 
    apiClient.post('/auth/profile.php', { 
      full_name: fullName, 
      old_password: oldPassword, 
      new_password: newPassword 
    }),

  // ============================================================================
  // 2. PHÂN HỆ VÍ TÀI CHÍNH (WALLETS)
  // ============================================================================

  /** 8. Lấy danh sách ví & tổng tài sản ròng khả dụng (JWT Bearer) */
  getWallets: () => 
    apiClient.get('/wallets/list.php'),

  /** 9. Tạo ví mới (Tiền mặt, Ngân hàng, ...) (JWT Bearer) */
  createWallet: ({ walletName, walletType = 'CASH', initialBalance = 0 }) => 
    apiClient.post('/wallets/create.php', { 
      wallet_name: walletName, 
      wallet_type: walletType, 
      initial_balance: initialBalance 
    }),

  /** 10. QOL #1: Chuyển tiền nội bộ giữa 2 ví ACID (JWT Bearer) */
  transferWallet: ({ fromWalletId, toWalletId, amount, note }) => 
    apiClient.post('/wallets/transfer.php', { 
      from_wallet_id: fromWalletId, 
      to_wallet_id: toWalletId, 
      amount, 
      note 
    }),

  // ============================================================================
  // 3. PHÂN HỆ DANH MỤC THU / CHI (CATEGORIES)
  // ============================================================================

  /** 11. Lấy danh sách danh mục thu/chi (Hệ thống + Tự tạo) (JWT Bearer) */
  getCategories: (type = null) => {
    const params = type ? `?type=${type}` : '';
    return apiClient.get(`/categories/list.php${params}`);
  },

  /** 12. Tạo danh mục riêng của người dùng (JWT Bearer) */
  createCategory: ({ name, type, iconName = 'pricetag-outline', colorCode = '#2B4C7E' }) => 
    apiClient.post('/categories/create.php', { 
      name, 
      type, 
      icon_name: iconName, 
      color_code: colorCode 
    }),

  // ============================================================================
  // 4. PHÂN HỆ GIAO DỊCH THU / CHI (TRANSACTIONS)
  // ============================================================================

  /** 13. UX #3: Lấy lịch sử giao dịch có bộ lọc đa tiêu chí (JWT Bearer) */
  getTransactions: ({ month, year, walletId, categoryId, type, keyword, limit = 50, offset = 0 } = {}) => {
    const query = new URLSearchParams();
    if (month) query.append('month', month);
    if (year) query.append('year', year);
    if (walletId) query.append('wallet_id', walletId);
    if (categoryId) query.append('category_id', categoryId);
    if (type) query.append('type', type);
    if (keyword) query.append('keyword', keyword);
    if (limit) query.append('limit', limit);
    if (offset) query.append('offset', offset);
    return apiClient.get(`/transactions/list.php?${query.toString()}`);
  },

  /** 14. Thêm giao dịch thu/chi mới (ACID & Cập nhật số dư ví) (JWT Bearer) */
  createTransaction: ({ walletId, categoryId, amount, transactionType, note, transactionDate }) => 
    apiClient.post('/transactions/create.php', { 
      wallet_id: walletId, 
      category_id: categoryId, 
      amount, 
      transaction_type: transactionType, 
      note, 
      transaction_date: transactionDate 
    }),

  /** 15. QOL #2: Sửa giao dịch & Tự bù trừ Delta số dư ví (JWT Bearer) */
  updateTransaction: ({ id, walletId, categoryId, amount, transactionType, note, transactionDate }) => 
    apiClient.post('/transactions/update.php', { 
      id, 
      wallet_id: walletId, 
      category_id: categoryId, 
      amount, 
      transaction_type: transactionType, 
      note, 
      transaction_date: transactionDate 
    }),

  /** 16. Xóa giao dịch và hoàn trả số dư ví (JWT Bearer) */
  deleteTransaction: (transactionId) => 
    apiClient.post('/transactions/delete.php', { transaction_id: transactionId }),

  // ============================================================================
  // 5. PHÂN HỆ HẠN MỨC NGÂN SÁCH (BUDGETS)
  // ============================================================================

  /** 17. UX #4: Xem tiến độ ngân sách tháng & cảnh báo (JWT Bearer) */
  getBudgetStatus: (month = null, year = null) => {
    const query = new URLSearchParams();
    if (month) query.append('month', month);
    if (year) query.append('year', year);
    return apiClient.get(`/budgets/status.php?${query.toString()}`);
  },

  /** 18. Thiết lập hạn mức ngân sách tháng hoặc theo danh mục (JWT Bearer) */
  setBudget: ({ categoryId = null, amountLimit, month, year }) => 
    apiClient.post('/budgets/set.php', { 
      category_id: categoryId, 
      amount_limit: amountLimit, 
      month, 
      year 
    }),

  // ============================================================================
  // 6. PHÂN HỆ BÁO CÁO TÀI CHÍNH & BIỂU ĐỒ (REPORTS)
  // ============================================================================

  /** 19. Báo cáo tổng quan thu - chi - số dư ròng tháng (JWT Bearer) */
  getMonthlySummary: (month = null, year = null) => {
    const query = new URLSearchParams();
    if (month) query.append('month', month);
    if (year) query.append('year', year);
    return apiClient.get(`/reports/monthly_summary.php?${query.toString()}`);
  },

  /** 20. Dữ liệu tỷ trọng chi tiêu chuẩn hóa cho PieChart (JWT Bearer) */
  getCategoryChart: (month = null, year = null) => {
    const query = new URLSearchParams();
    if (month) query.append('month', month);
    if (year) query.append('year', year);
    return apiClient.get(`/reports/category_chart.php?${query.toString()}`);
  },

  /** 21. Dữ liệu đối chiếu thu vs chi 6 tháng cho BarChart (JWT Bearer) */
  getIncomeVsExpenseChart: (monthsCount = 6) => 
    apiClient.get(`/reports/income_vs_expense_chart.php?months_count=${monthsCount}`)
};

export default api;
