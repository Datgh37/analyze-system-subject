# 📱 HƯỚNG DẪN DỰ ÁN FRONTEND MOBILE (EXPO SDK 54)

> **Dành cho:** Trần Trọng Đại (UI & Mobile Frontend Lead)  
> **Kiến trúc & Backend:** Trịnh Thành Đạt (Lead)  
> **Đồ án môn học:** Lập trình Thiết bị Di động — Trường Đại học Công nghệ Đồng Nai (DNTU)  
> **Đề tài:** Ứng dụng Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh  

---

## 1. ⚙️ Thông Số Kỹ Thuật (Tech Stack)

* **Khung ứng dụng:** **Expo SDK 54** (`expo: ~54.0.36`) — Hỗ trợ chính thức **Android 14 (API 34) & Android 15 (API 35)** và Node 24 LTS.
* **Core Engines:** `react-native: 0.81.5`, `react: 19.1.0`.
* **Điều hướng:** `@react-navigation/native: ^7.5.0`, `@react-navigation/bottom-tabs: ^7.20.0`, `@react-navigation/native-stack: ^7.20.0`.
* **Quản lý phiên & Gọi API:** `axios: ^1.20.0`, `@react-native-async-storage/async-storage: 2.2.0`.
* **Biểu đồ & Vector Icons:** `react-native-chart-kit: ^7.0.4`, `react-native-svg: 15.12.1`, `@expo/vector-icons: ^15.1.1`.
* **Quy chuẩn Styling:** **100% `StyleSheet.create({...})` và hằng số `COLORS`** (Tuyệt đối không dùng Tailwind/NativeWind để bảo vệ đồ án trước buổi thi vấn đáp).

---

## 2. 🚀 Hướng Dẫn Khởi Chạy Ứng Dụng Trên Máy Của Đại

### Bước 1: Mở Terminal tại thư mục `mobile-project/react-native-ver/FE`
```powershell
cd mobile-project/react-native-ver/FE
```

### Bước 2: Cài đặt toàn bộ thư viện (Dependencies)
> [!IMPORTANT]
> Dự án sử dụng **React 19 & Expo SDK 54**. Bạn Đại vui lòng sử dụng lệnh sau để tải toàn bộ package về máy:

* **Cách 1 (Khuyên dùng nhất cho Đại — Chuẩn Expo):**
  ```powershell
  npx expo install
  ```
  *(Trình quản lý package chính thức của Expo sẽ tự động kiểm tra và cài đặt phiên bản tương thích nhất với Expo SDK 54)*.

* **Cách 2 (Dùng lệnh `npm`):**
  ```powershell
  npm install --legacy-peer-deps
  ```
  *(⚠️ **Lưu ý:** Bắt buộc phải có cờ `--legacy-peer-deps` để tránh lỗi xung đột peerDependencies của React 19 đối với một số thư viện như `react-native-chart-kit`)*.

* **Cách 3 (Nếu dùng `pnpm` trên máy của Đạt):**
  ```powershell
  pnpm install
  ```

### Bước 3: Chạy Expo Dev Server
```powershell
npm start

# Hoặc khởi động xóa sạch cache (khuyên dùng khi mới tải code về):
npx expo start -c
```

### Bước 4: Xem Ứng Dụng Trên Thiết Bị
* **Cách 1 (Khuyên dùng - Điện thoại thật Android 14):**
  - Mở app **Expo Go** trên điện thoại (đã kết nối cùng mạng Wifi với máy tính).
  - Quét mã QR code hiển thị trên terminal.
* **Cách 2 (Máy ảo Android Studio):**
  - Nhấn phím `a` trong terminal để mở máy ảo Android.

---

## 3. 🌐 Cấu Hình Địa Chỉ Kết Nối Backend WAMP Server

Toàn bộ cấu hình mạng nằm tại: [`src/api/config.js`](src/api/config.js)

1. **Nếu bạn test trên Điện thoại thật (cùng Wifi):**
   - Mở PowerShell gõ lệnh `ipconfig` để lấy IPv4 máy tính của bạn (Ví dụ: `192.168.1.15`).
   - Mở file `src/api/config.js`, sửa hằng số:
     ```javascript
     const LOCAL_LAN_IP = '192.168.1.15'; // Đổi thành IP máy của bạn
     ```
2. **Nếu bạn test trên Máy ảo Android Emulator:**
   - Hệ thống tự động nhận diện IP loopback `10.0.2.2`, không cần sửa gì thêm.
3. **Nếu test trên Server trường cấp:**
   - Đổi cờ `USE_PRODUCTION_SERVER = true`.

---

## 4. 🗂️ Cấu Trúc Thư Mục Mã Nguồn

```text
FE/
├── App.js                     # Root Component bọc SafeAreaProvider & AuthProvider
├── src/
│   ├── api/
│   │   ├── config.js          # Cấu hình IP và Base URL động
│   │   ├── client.js          # Axios Client tự động kẹp Bearer JWT Token vào mọi request
│   │   └── endpoints.js       # Toàn bộ 21 hàm gọi API đóng gói sẵn
│   ├── constants/
│   │   └── theme.js           # Bảng màu COLORS, SPACING, SIZES, SHADOWS
│   ├── context/
│   │   └── AuthContext.js     # Quản lý phiên đăng nhập, JWT token, user state
│   ├── navigation/
│   │   ├── AppNavigator.js    # Stack điều hướng (chuyển đổi giữa Auth và MainTabs)
│   │   └── TabNavigator.js    # Bottom Tabs (Tổng quan, Giao dịch, Ngân sách, Báo cáo, Hồ sơ)
│   └── screens/
│       ├── auth/
│       │   ├── LoginScreen.js          # Đăng nhập
│       │   ├── RegisterScreen.js       # Đăng ký (tự động tạo 2 ví cơ bản)
│       │   └── ForgotPasswordScreen.js # Quên mật khẩu 3 bước chuẩn OTP
│       └── main/
│           ├── HomeScreen.js           # Dashboard tổng tài sản & tiến độ ngân sách
│           ├── TransactionsScreen.js   # Lịch sử giao dịch & lọc đa tiêu chí
│           ├── AddTransactionScreen.js # Thêm / Sửa giao dịch (bù trừ Delta ví)
│           ├── BudgetScreen.js         # Ngân sách tháng & chi tiêu an toàn mỗi ngày
│           ├── ReportScreen.js         # Báo cáo tỷ trọng PieChart & đối chiếu BarChart
│           └── ProfileScreen.js        # Đổi tên hiển thị, đổi mật khẩu & đăng xuất
```

---

## 5. 🎨 Nhiệm Vụ Tiếp Theo Của Bạn Đại (UI & Design)

Toàn bộ logic kết nối API, state management, tính toán tiền tệ và cấu trúc khung sườn đã được cài đặt hoàn chỉnh và hoạt động 100%. 

Trong mỗi file màn hình tại thư mục `src/screens/`, tôi đã để lại khối chú thích:  
👉 **`HƯỚNG DẪN THIẾT KẾ GIAO DIỆN (DÀNH CHO ĐẠI - UI LEAD)`**

Đại chỉ cần:
1. Mở từng file màn hình để xem wireframe gợi ý.
2. Tùy chỉnh màu sắc, viền bo, icon, khoảng cách theo sở thích thẩm mỹ cá nhân bằng cách sử dụng các hằng số từ `COLORS`, `SPACING`, `SIZES` trong [`src/constants/theme.js`](src/constants/theme.js).
3. Đảm bảo giao diện đồng bộ, trực quan để chuẩn bị tốt nhất cho buổi bảo vệ đồ án!

---

## 6. ⚡ Hướng Dẫn Tích Hợp Realtime Socket.IO (Dành Riêng Cho Bạn Đại - UI Lead)

Hệ thống đã được trang bị sẵn cơ chế WebSocket thời gian thực (Full-Duplex) để nhận thông báo biến động số dư ví (0ms sync) và cảnh báo cú hích chi tiêu (Nudge Alert). Đại **không cần viết code kết nối socket phức tạp** vì mọi thứ đã được gói sẵn vào Context và Component:

### 6.1. Cách dùng Banner Cảnh báo có sẵn (RealtimeAlertBanner)
Đại chỉ cần import và đặt component này ở đầu màn hình:
```javascript
import RealtimeAlertBanner from '../../components/RealtimeAlertBanner';

export default function MyScreen() {
  return (
    <View style={{ flex: 1 }}>
      {/* Tự động hiển thị chấm xanh kết nối + banner vàng/đỏ khi vượt ngân sách */}
      <RealtimeAlertBanner />
      
      {/* Nội dung giao diện khác của Đại */}
    </View>
  );
}
```

### 6.2. Cách lấy dữ liệu Realtime trực tiếp qua Hook `useRealtime()`
Nếu Đại muốn tự vẽ UI thông báo riêng theo ý mình:
```javascript
import { useRealtime } from '../../context/RealtimeContext';

export default function MyCustomScreen() {
  const { 
    isConnected,       // boolean: true nếu đang kết nối WebSocket với server
    latencyMs,         // number: độ trễ mạng tính bằng ms (ví dụ: 12ms)
    lastBalanceEvent,  // object: { walletName, delta, newBalance, reason }
    activeNudge,       // object: { alertTitle, alertMessage, severity: 'warning' | 'danger' }
    clearNudge         // hàm: đóng thông báo cảnh báo
  } = useRealtime();

  // Ví dụ: Tự động reload lại danh sách khi có sự kiện tiền về/tiền ra
  useEffect(() => {
    if (lastBalanceEvent) {
      console.log('Có biến động ví:', lastBalanceEvent);
      loadMyDataAgain();
    }
  }, [lastBalanceEvent]);
}
```

### 6.3. Cách Test Thử Giao Diện Realtime Khi Đang Thiết Kế
1. Bật Realtime Server tại thư mục `mobile-project/react-native-ver/realtime-server`:
   ```bash
   pnpm start
   ```
2. Mở trình duyệt Web tại `http://localhost:3001` (hoặc chạy `pnpm test:trigger`).
3. Bấm các nút thử nghiệm như *"Chi ăn trưa (-50k)"*, *"Cảnh báo 86%"*, *"Cảnh báo ĐỎ 110%"* ➔ Màn hình app Expo trên điện thoại của Đại sẽ tự động nhảy số dư và hiện banner ngay trước mắt mà không cần nạp lại trang!

