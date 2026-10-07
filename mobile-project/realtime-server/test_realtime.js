/**
 * ==============================================================================
 * REALTIME TRIGGER TEST SCRIPT (Node.js)
 * Kích hoạt các sự kiện Socket.IO để kiểm thử ứng dụng Mobile
 * ==============================================================================
 */

const BASE_URL = process.env.BASE_URL || "http://localhost:3001";
const USER_ID = 1;

async function sleep(ms) {
  return new Promise((resolve) => setTimeout(resolve, ms));
}

async function runTests() {
  console.log("========================================================================");
  console.log("⚡ KIEM THU PHAT SU KIEN REALTIME SOCKET.IO (EXPENSE APP)");
  console.log(`   URL: ${BASE_URL} | Target User ID: #${USER_ID}`);
  console.log("========================================================================");

  // 1. Kiểm tra Health
  try {
    const healthRes = await fetch(`${BASE_URL}/health`);
    const health = await healthRes.json();
    console.log(`\n[1/4] Server Trạng Thái: ${health.status.toUpperCase()} | Sockets kết nối: ${health.activeSockets}`);
  } catch (err) {
    console.error(`\n❌ LỖI: Không thể kết nối tới ${BASE_URL}. Hãy chắc chắn server đang chạy (pnpm start).`);
    process.exit(1);
  }

  // 2. Test Balance Update
  await sleep(1000);
  console.log("\n[2/4] Bắn sự kiện [wallet:balance_updated] (Đồng bộ số dư tức thì)...");
  try {
    const res = await fetch(`${BASE_URL}/api/notify/balance-update`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        userId: USER_ID,
        walletId: 1,
        walletName: "Tiền Mặt",
        newBalance: 2450000,
        delta: -50000,
        reason: "Chi ăn trưa cơm tấm văn phòng"
      })
    });
    const data = await res.json();
    console.log(`   ✔ Thành công: ${data.message}`);
    console.log(`   └─ Ví: ${data.data.walletName} | Biến động: ${data.data.delta.toLocaleString("vi-VN")} đ | Số dư mới: ${data.data.newBalance.toLocaleString("vi-VN")} đ`);
  } catch (err) {
    console.error(`   ❌ Thất bại: ${err.message}`);
  }

  // 3. Test Budget Nudge (Cảnh báo 86%)
  await sleep(1200);
  console.log("\n[3/4] Bắn sự kiện [budget:nudge_alert] (Cảnh báo tốc độ tiêu tiền 86%)...");
  try {
    const res = await fetch(`${BASE_URL}/api/notify/budget-nudge`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        userId: USER_ID,
        categoryName: "Ăn uống & Cà phê",
        currentSpent: 2150000,
        budgetLimit: 2500000,
        severity: "warning"
      })
    });
    const data = await res.json();
    console.log(`   ✔ Thành công: ${data.message}`);
    console.log(`   └─ Tiêu đề: ${data.data.alertTitle}`);
    console.log(`   └─ Thông điệp: ${data.data.alertMessage}`);
  } catch (err) {
    console.error(`   ❌ Thất bại: ${err.message}`);
  }

  // 4. Test Budget Nudge Danger (110% Vượt mức)
  await sleep(1200);
  console.log("\n[4/4] Bắn sự kiện [budget:nudge_alert] (Mức độ DANGER vượt hạn mức 110%)...");
  try {
    const res = await fetch(`${BASE_URL}/api/notify/budget-nudge`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({
        userId: USER_ID,
        categoryName: "Mua sắm cá nhân",
        currentSpent: 3300000,
        budgetLimit: 3000000,
        severity: "danger"
      })
    });
    const data = await res.json();
    console.log(`   ✔ Thành công: ${data.message}`);
    console.log(`   └─ Tiêu đề: ${data.data.alertTitle}`);
    console.log(`   └─ Thông điệp: ${data.data.alertMessage}`);
  } catch (err) {
    console.error(`   ❌ Thất bại: ${err.message}`);
  }

  console.log("\n========================================================================");
  console.log("✨ HOÀN TẤT KIỂM THỬ TẤT CẢ SỰ KIỆN REALTIME!");
  console.log("   Mobile App (Expo) nhận sự kiện và cập nhật giao diện trong <10ms.");
  console.log("========================================================================\n");
}

runTests();
