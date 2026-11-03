# KẾ HOẠCH 60: KHỞI TẠO & MỞ RỘNG TOÀN DIỆN HỆ THỐNG GIAO DIỆN FRONTEND DUONOPRO BẰNG OPEN DESIGN
**Thời gian lập:** 2026-11-03  
**Dự án:** Hệ Thống Quản Lý Chi Tiêu & Hoạch Định Tài Chính Cá Nhân Thông Minh DuoNoPro (PFM)  
**Quy chuẩn áp dụng:** Open Design MCP Architecture + BMAD Council + Impeccable Design Token Standards  

---

## 1. BỐI CẢNH & MỤC TIÊU CHIẾN LƯỢC

### 1.1. Bối cảnh
- Giai đoạn 1 đã khởi tạo thành công Design System `user:duonopro-fintech` và bộ nguyên mẫu 5 tab cốt lõi, chạy kiểm toán Preflight đạt 100% Passed và mở Live Preview trình duyệt.
- Người dùng yêu cầu tiếp tục triển khai các mục/phân hệ còn lại để bao quát toàn bộ 9 Phân hệ (63 Use Cases) của ứng dụng di động.

### 1.2. Mục tiêu mở rộng 10 Màn hình Toàn diện (Full 10-Screen Suite)
1. **Màn hình 1 — Dashboard & Ví Tiền (`WalletsPage.xaml`):** Thẻ Hero Obsidian VIP, Net Worth, Hạn mức ngày DSA, Quick Action Pills, Realtime SignalR Hub.
2. **Màn hình 2 — Sổ Chi Tiêu & Multimodal AI OCR (`ExpensesPage.xaml`):** Bóc tách đa món hóa đơn WinMart+, hậu kiểm toán số học sai lệch 0 đ, dòng thời gian phân nhóm ngày, bẫy thuê bao.
3. **Màn hình 3 — Ngân Sách Đa Phương Pháp (`BudgetsPage.xaml`):** 50/30/20, Kakeibo, ZBB, thước đo tốc độ tiêu tiền Spending Velocity, cảnh báo vi mô.
4. **Màn hình 4 — Báo Cáo Phân Tích Dòng Tiền Thác Nước (`ReportsPage.xaml`):** Thác nước 5 bước Waterfall Cashflow, kiểm định ngoại lai Z-Score Outlier ($\mu \pm 2\sigma$).
5. **Màn hình 5 — Cú Hích Thaler Nudge & Khoảng Trễ 24h (`NudgeFeedPage.xaml` & `CoolingOffHubPage.xaml`):** Thẻ XAI 3 thành phần, đếm ngược phong tỏa mua sắm lớn $\ge 3$ triệu, quy đổi giờ công lao động.
6. **Màn hình 6 — Thu Nhập & Nguồn Thu Định Kỳ (`IncomesPage.xaml` & `RecurringIncomesPage.xaml`):** Thẻ Hero Emerald Dòng tiền vào, Lương tháng Techcombank, Thưởng, Cổ tức, Lịch cộng dồn ví tự động khi đến hạn, OCR Bảng lương đối soát công thức.
7. **Màn hình 7 — Quỹ Mục Tiêu Tích Lũy & Hũ Kakeibo (`SavingsGoalsPage.xaml`):** Quỹ khẩn cấp 6 tháng (75M/100M), Mua xe điện VinFast VF3 (120M/300M), 4 Cột trụ Kakeibo (Needs, Wants, Culture, Unexpected), Thaler Nudge ma sát nhận thức rút tiền.
8. **Màn hình 8 — Phòng Thí Nghiệm What-If Sandbox & Radar Thâm Hụt (`ForecastingPage.xaml`):** Sandbox mô phỏng in-memory 0ms sụt giảm thu nhập -20%, Vay mua xe DTI, Thâm hụt danh mục thiết yếu, Đối chiếu ngày cạn tiền với ngày nhận lương kế tiếp (`IsLiquidityCrisis`).
9. **Màn hình 9 — Tiện Ích Thông Minh & Khay Hành Động Tự Hành (`SmartUtilitiesPage.xaml` & `AutoRulesPage.xaml`):** Server-Driven Action Protocol, Thẻ Jev Choice $\ge 0.85$ 1-chạm, Quét phát hiện bẫy đăng ký định kỳ, Tạo mã VietQR Napas 247 chuẩn EMVCo tức thì.
10. **Màn hình 10 — Trung Tâm Bảo Mật & Cài Đặt Hệ Thống (`SettingsPage.xaml` & `LoginPage.xaml`):** Két mã hóa AES-256-GCM, Khóa sinh trắc học FaceID/Vân tay, Bộ chuyển đổi Dynamic Repository Mode Switcher (Real API vs Offline Mock 0ms), JWT Token Rotation.

---

## 2. LỘ TRÌNH THỰC THI 3 BƯỚC

| Bước | Nội Dung | Sản Phẩm Đầu Ra |
| :--- | :--- | :--- |
| **Bước 1** | Mở rộng mã nguồn `Open Design/duonopro-mobile-app/duonopro-mobile-app.html` từ 5 màn hình lên trọn vẹn 10 màn hình tương tác, tích hợp thanh cuộn danh mục phân nhóm (Category Filter Chips) và bổ sung các icon vector sắc nét. | `Open Design/duonopro-mobile-app/duonopro-mobile-app.html` |
| **Bước 2** | Đăng ký cập nhật (`register_open_design_artifact`) và chạy kiểm toán Preflight (`check_open_design_artifact`) đảm bảo 0 lỗi giao diện. | Live Preview reload tự động |
| **Bước 3** | Cập nhật tài liệu kỹ thuật chuyển giao [OPEN_DESIGN_MAUI_MAPPING.md](file:///d:/bai%20tap%20ki%204/coLieu%20T6/analyze-system-subject/mobile-project/dotnet-maui-ver/implement-docs/OPEN_DESIGN_MAUI_MAPPING.md) cho đủ 10 màn hình đối soát. | `OPEN_DESIGN_MAUI_MAPPING.md` |

---

## 3. CHECKLIST KIỂM TOÁN CHẤT LƯỢNG
- [ ] 100% không dùng emoji rập khuôn; dùng vector icons Lucide.
- [ ] Đủ 10 màn hình tương ứng 9 Phân hệ Use Cases.
- [ ] Số liệu tài chính định dạng `N0 ₫` và dùng font tabular-nums chống giật giao diện.
- [ ] Preflight check đạt Passed không lỗi tràn viền.
