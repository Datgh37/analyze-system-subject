/**
 * ⚡ REALTIME SOCKET.IO SERVER (PROOF-OF-CONCEPT)
 * Ứng dụng Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh
 * 
 * Chức năng cốt lõi:
 * 1. Mở kênh kết nối 2 chiều (Full-Duplex) với Mobile Client (React Native Expo).
 * 2. Quản lý Rooms theo từng người dùng (user_${userId}) để đảm bảo tính riêng tư cá nhân.
 * 3. Bắn sự kiện thời gian thực (Realtime Events):
 *    - wallet:balance_updated  (Đồng bộ số dư ví tức thời trên nhiều thiết bị)
 *    - budget:nudge_alert       (Cảnh báo cú hích chi tiêu vượt ngưỡng ngân sách)
 *    - auth:force_logout        (Thu hồi phiên đăng nhập khi phát hiện rủi ro)
 * 4. Cung cấp HTTP Trigger Endpoints để PHP Backend / Postman / cURL kích hoạt event.
 * 
 * Tác giả: Trịnh Thành Đạt (Lead Architect) & Trần Trọng Đại
 */

const express = require("express");
const http = require("http");
const { Server } = require("socket.io");
const cors = require("cors");
const path = require("path");

const app = express();
const server = http.createServer(app);

// Cấu hình Middleware
app.use(cors());
app.use(express.json());
app.use(express.static(path.join(__dirname, "public")));

// Khởi tạo Socket.IO Server với CORS mở rộng cho Mobile & Dev Tools
const io = new Server(server, {
  cors: {
    origin: "*",
    methods: ["GET", "POST"]
  },
  pingTimeout: 10000,
  pingInterval: 5000
});

const PORT = process.env.PORT || 3001;

// Theo dõi số lượng kết nối đang hoạt động
const activeConnections = new Map();

// ==============================================================================
// 1. QUẢN LÝ VÒNG ĐỜI KẾT NỐI SOCKET.IO
// ==============================================================================
io.use((socket, next) => {
  // Lấy thông tin xác thực từ Handshake Auth hoặc Query
  const auth = socket.handshake.auth || {};
  const query = socket.handshake.query || {};

  const userId = auth.userId || query.userId || 1; // Mặc định User 1 nếu test nhanh
  const deviceName = auth.deviceName || query.deviceName || "Thiết bị di động";

  socket.userId = Number(userId);
  socket.deviceName = deviceName;
  next();
});

io.on("connection", (socket) => {
  const roomName = `user_${socket.userId}`;
  socket.join(roomName);

  activeConnections.set(socket.id, {
    userId: socket.userId,
    deviceName: socket.deviceName,
    connectedAt: new Date().toISOString()
  });

  const timestamp = new Date().toLocaleTimeString("vi-VN");
  console.log(`\n🟢 [${timestamp}] CLIENT KẾT NỐI:`);
  console.log(`   └─ User ID: #${socket.userId} | Phòng: ${roomName}`);
  console.log(`   └─ Thiết bị: ${socket.deviceName} | Socket ID: ${socket.id}`);
  console.log(`   └─ Tổng số kết nối hiện tại: ${activeConnections.size}`);

  // Gửi sự kiện chào mừng và xác nhận kết nối về Client
  socket.emit("system:connected", {
    status: "success",
    message: "Đã thiết lập kết nối thời gian thực với Realtime Server!",
    socketId: socket.id,
    userId: socket.userId,
    serverTime: new Date().toISOString()
  });

  // Client có thể gửi ping kiểm tra độ trễ (Latency Test)
  socket.on("ping:check", (data) => {
    socket.emit("pong:reply", {
      clientTimestamp: data?.timestamp,
      serverTimestamp: Date.now()
    });
  });

  // Xử lý khi Client ngắt kết nối
  socket.on("disconnect", (reason) => {
    activeConnections.delete(socket.id);
    const leaveTime = new Date().toLocaleTimeString("vi-VN");
    console.log(`\n🔴 [${leaveTime}] CLIENT NGẮT KẾT NỐI:`);
    console.log(`   └─ User #${socket.userId} (${socket.deviceName}) | Lý do: ${reason}`);
    console.log(`   └─ Còn lại: ${activeConnections.size} kết nối`);
  });
});

// ==============================================================================
// 2. HTTP REST TRIGGER ENDPOINTS (DÀNH CHO BACKEND / TEST SCRIPT KÍCH HOẠT)
// ==============================================================================

// Kiểm tra sức khỏe Server
app.get("/health", (req, res) => {
  res.json({
    status: "online",
    service: "Expense Realtime Socket.IO Gateway",
    port: PORT,
    activeSockets: activeConnections.size,
    timestamp: new Date().toISOString()
  });
});

// Trigger 1: Bắn event Cập nhật Số Dư Ví (Realtime Balance Sync)
app.post("/api/notify/balance-update", (req, res) => {
  const { userId = 1, walletId = 1, walletName = "Tiền Mặt", newBalance = 2450000, delta = -50000, reason = "Ăn trưa cơm văn phòng" } = req.body;
  const roomName = `user_${userId}`;

  const payload = {
    eventType: "WALLET_BALANCE_UPDATED",
    userId: Number(userId),
    walletId: Number(walletId),
    walletName,
    newBalance: Number(newBalance),
    delta: Number(delta),
    reason,
    timestamp: new Date().toISOString()
  };

  io.to(roomName).emit("wallet:balance_updated", payload);

  console.log(`\n⚡ [TRIGGER] Phát sự kiện 'wallet:balance_updated' tới [${roomName}]:`);
  console.log(`   └─ Ví: ${walletName} | Biến động: ${delta.toLocaleString("vi-VN")} đ ➔ Số dư mới: ${newBalance.toLocaleString("vi-VN")} đ`);

  res.json({
    status: "success",
    message: `Đã phát sự kiện cập nhật số dư tới người dùng #${userId}`,
    data: payload
  });
});

// Trigger 2: Bắn event Cảnh Báo Ngân Sách / Nudge (Realtime Spending Nudge)
app.post("/api/notify/budget-nudge", (req, res) => {
  const { 
    userId = 1, 
    categoryName = "Ăn uống & Cà phê", 
    currentSpent = 2150000, 
    budgetLimit = 2500000, 
    severity = "warning" // warning (80%) hoặc danger (100%+)
  } = req.body;

  const roomName = `user_${userId}`;
  const percentage = Math.round((currentSpent / budgetLimit) * 100);

  const payload = {
    eventType: "BUDGET_NUDGE_TRIGGERED",
    userId: Number(userId),
    categoryName,
    currentSpent,
    budgetLimit,
    percentage,
    severity,
    alertTitle: severity === "danger" ? "🚨 CẢNH BÁO VƯỢT NGÂN SÁCH!" : "⚠️ CẢNH BÁO TỐC ĐỘ TIÊU TIỀN!",
    alertMessage: `Bạn đã chi tiêu ${percentage}% hạn mức cho danh mục [${categoryName}]. Hãy cân nhắc các khoản chi tiếp theo trong tháng!`,
    timestamp: new Date().toISOString()
  };

  io.to(roomName).emit("budget:nudge_alert", payload);

  console.log(`\n📢 [TRIGGER] Phát cảnh báo 'budget:nudge_alert' tới [${roomName}]:`);
  console.log(`   └─ Danh mục: ${categoryName} (${percentage}% hạn mức) | Mức độ: ${severity}`);

  res.json({
    status: "success",
    message: `Đã phát cảnh báo ngân sách tới người dùng #${userId}`,
    data: payload
  });
});

// Trigger 3: Bắn event Đăng Xuất Cưỡng Bức (Force Logout Security)
app.post("/api/notify/force-logout", (req, res) => {
  const { userId = 1, reason = "Mật khẩu tài khoản vừa được thay đổi từ một thiết bị khác." } = req.body;
  const roomName = `user_${userId}`;

  const payload = {
    eventType: "SECURITY_FORCE_LOGOUT",
    userId: Number(userId),
    reason,
    timestamp: new Date().toISOString()
  };

  io.to(roomName).emit("auth:force_logout", payload);

  console.log(`\n🔒 [TRIGGER] Thu hồi phiên 'auth:force_logout' tới [${roomName}]:`);
  console.log(`   └─ Lý do: ${reason}`);

  res.json({
    status: "success",
    message: `Đã phát lệnh thu hồi phiên tới người dùng #${userId}`,
    data: payload
  });
});

// ==============================================================================
// 3. KHỞI CHẠY SERVER
// ==============================================================================
server.listen(PORT, () => {
  console.log("========================================================================");
  console.log(`🚀 EXPENSE REALTIME SOCKET.IO SERVER ĐANG CHẠY TẠI PORT: ${PORT}`);
  console.log(`   - 🌐 Web Client Simulator:  http://localhost:${PORT}`);
  console.log(`   - HTTP Health Check:        http://localhost:${PORT}/health`);
  console.log(`   - Socket.IO Endpoint:        ws://localhost:${PORT}`);
  console.log(`   - Trigger Balance:         POST http://localhost:${PORT}/api/notify/balance-update`);
  console.log(`   - Trigger Nudge:           POST http://localhost:${PORT}/api/notify/budget-nudge`);
  console.log(`   - Trigger Logout:          POST http://localhost:${PORT}/api/notify/force-logout`);
  console.log("========================================================================");
});
