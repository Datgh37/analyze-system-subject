/**
 * ==============================================================================
 * CLI REALTIME CLIENT (TERMINAL LISTENER)
 * Đóng vai trò là một Mobile App ảo lắng nghe trực tiếp trong Terminal
 * ==============================================================================
 */

const { io } = require("socket.io-client");

const SERVER_URL = process.env.SERVER_URL || "http://localhost:3001";
const USER_ID = 1;

console.log("========================================================================");
console.log("📱 CLIENT DI DONG AO (TERMINAL REALTIME LISTENER)");
console.log(`   Ket noi toi: ${SERVER_URL} | Dang nhap: User #${USER_ID}`);
console.log("========================================================================");

const socket = io(SERVER_URL, {
  auth: {
    userId: USER_ID,
    deviceName: "Terminal Mobile Client"
  }
});

socket.on("connect", () => {
  const time = new Date().toLocaleTimeString("vi-VN");
  console.log(`\n🟢 [${time}] DA KET NOI VOI GATEWAY! (Socket ID: ${socket.id})`);
  console.log("   Dang lang nghe su kien trong phong: user_" + USER_ID + "...");
  console.log("   ------------------------------------------------------------------");
});

socket.on("disconnect", (reason) => {
  const time = new Date().toLocaleTimeString("vi-VN");
  console.log(`\n🔴 [${time}] MAT KET NOI VOI GATEWAY: ${reason}`);
});

socket.on("system:connected", (data) => {
  console.log(`\n🎉 [SERVER MESSAGE] ${data.message}`);
});

// 1. Lắng nghe cập nhật số dư
socket.on("wallet:balance_updated", (data) => {
  const time = new Date().toLocaleTimeString("vi-VN");
  console.log(`\n⚡ [${time}] SU KIEN: wallet:balance_updated`);
  console.log(`   ├─ Vi: ${data.walletName} (ID: ${data.walletId})`);
  console.log(`   ├─ Bien dong: ${data.delta > 0 ? "+" : ""}${data.delta.toLocaleString("vi-VN")} đ`);
  console.log(`   ├─ So du moi: ${data.newBalance.toLocaleString("vi-VN")} đ`);
  console.log(`   └─ Ly do: ${data.reason}`);
});

// 2. Lắng nghe cảnh báo ngân sách
socket.on("budget:nudge_alert", (data) => {
  const time = new Date().toLocaleTimeString("vi-VN");
  const icon = data.severity === "danger" ? "🚨" : "⚠️";
  console.log(`\n${icon} [${time}] CANH BAO NGAN SACH: budget:nudge_alert [${data.severity.toUpperCase()}]`);
  console.log(`   ├─ Tieu de: ${data.alertTitle}`);
  console.log(`   ├─ Danh muc: ${data.categoryName} (${data.percentage}% han muc)`);
  console.log(`   ├─ Da tieu: ${data.currentSpent.toLocaleString("vi-VN")} đ / ${data.budgetLimit.toLocaleString("vi-VN")} đ`);
  console.log(`   └─ Thong diep: ${data.alertMessage}`);
});

// 3. Lắng nghe lệnh đăng xuất cưỡng bức
socket.on("auth:force_logout", (data) => {
  const time = new Date().toLocaleTimeString("vi-VN");
  console.log(`\n🔒 [${time}] BAO MAT: auth:force_logout`);
  console.log(`   └─ Ly do: ${data.reason}`);
  console.log("   >> Phiên làm việc đã bị thu hồi!");
});
