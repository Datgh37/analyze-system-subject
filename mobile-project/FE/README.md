# 📱 HƯỚNG DẪN PHÁT TRIỂN GIAO DIỆN REACT NATIVE (FRONTEND)

> **Dành cho:** Trần Trọng Đại (UI Specialist & Frontend Screens) & AI Agent hỗ trợ  
> **Kiến trúc & Setup:** Trịnh Thành Đạt (Lead)  
> **Quy chuẩn bắt buộc:** Không dùng thư viện CSS (Tailwind, Paper...), viết thuần `StyleSheet.create({...})` và `const COLORS`, code mộc mạc phục vụ thi vấn đáp.

---

## 1. Cấu Trúc Thư Mục Frontend Chuẩn

```text
mobile-project/FE/
├── package.json               # Danh sách thư viện cần thiết
├── App.js                     # Điểm khởi chạy app (NavigationContainer & Safe Area)
└── src/
    ├── api/
    │   ├── config.js          # URL kết nối WAMP đa môi trường (Emulator / Device / Server)
    │   └── client.js          # Axios instance tự động xử lý request/response
    ├── constants/
    │   └── theme.js           # Bảng màu COLORS và kích thước SIZES dùng chung
    ├── navigation/
    │   ├── AppNavigator.js    # Stack Navigator (Login -> MainTab)
    │   └── TabNavigator.js    # BottomTab 4 tab: Trang Chủ, Thêm Mới, Lịch Sử, Báo Cáo
    ├── components/            # Thư mục để Đại thiết kế các linh kiện UI tái sử dụng
    │   ├── WalletCard.js      # (Đại viết) Thẻ số dư ví tiền mặt / ngân hàng
    │   ├── BudgetBar.js       # (Đại viết) Thanh tiến độ ngân sách đổi màu
    │   └── TransactionRow.js  # (Đại viết) Hàng hiển thị giao dịch
    └── screens/               # Các màn hình chính (Đại hoàn thiện giao diện chi tiết)
        ├── LoginScreen.js     # Màn hình đăng nhập
        ├── HomeScreen.js      # Màn hình tổng quan Dashboard
        ├── AddTransactionScreen.js # Màn hình thêm thu / chi
        ├── HistoryScreen.js   # Màn hình lịch sử giao dịch (FlatList + Xóa)
        └── ReportScreen.js    # Màn hình báo cáo biểu đồ (PieChart & BarChart)
```

---

## 2. Hướng Dẫn Khởi Chạy Ứng Dụng Trên Máy Bạn Đại

### Bước 1: Cài đặt thư viện (Chỉ chạy 1 lần đầu)
Mở terminal/cmd tại thư mục `mobile-project/FE/`:
```bash
npm install
```

### Bước 2: Khởi động WampServer trên máy tính
- Bật WampServer, đảm bảo icon màu xanh lá cây.
- Đảm bảo đã chạy script liên kết Junction: `..\BE\setup_wamp_junction.ps1`.

### Bước 3: Khởi chạy Expo / React Native
```bash
npx expo start
```
- Nhấn phím `a` để mở trên Android Studio Emulator (máy ảo).
- Hoặc dùng điện thoại thật: Cài app **Expo Go** trên CH Play / App Store, quét mã QR trên màn hình terminal để chạy app trực tiếp trên điện thoại!

---

## 3. Quy Tắc Code Dành Cho Bạn Đại (Tránh Bị Trừ Điểm Vấn Đáp)

1. **Tuyệt đối không cài thêm thư viện CSS:** Mọi style viết trong `const styles = StyleSheet.create({...})` ở cuối file.
2. **Dùng màu tập trung:** Import và dùng màu từ `src/constants/theme.js` (ví dụ: `COLORS.primary`, `COLORS.income`, `COLORS.expense`).
3. **Gọi API bằng client có sẵn:** 
   ```javascript
   import apiClient from '../api/client';
   // Gọi GET:
   const res = await apiClient.get('/wallets/list.php?user_id=1');
   // Gọi POST:
   const res = await apiClient.post('/transactions/create.php', payload);
   ```
