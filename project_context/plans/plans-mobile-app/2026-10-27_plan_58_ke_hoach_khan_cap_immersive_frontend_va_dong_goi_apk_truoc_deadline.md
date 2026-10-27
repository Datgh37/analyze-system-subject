# ⚡ KẾ HOẠCH TRIỂN KHAI KHẨN CẤP (EMERGENCY IMPLEMENTATION PLAN - PLAN-58)
## TÁC CHIẾN ĐÊM 27/10: HIỆN THỰC HÓA 100% HƯỚNG DẪN IMMERSIVE FRONTEND, ĐÓNG GÓI RELEASE APK & HOÀN TẤT HỒ SƠ ĐỒ ÁN
- **Dự án:** DuoNoPro - Quản Lý Chi Tiêu & Hoạch Định Tài Chính Cá Nhân Thông Minh (Tích hợp AI)
- **Tài liệu kim chỉ nam:** [`HUONG_DAN_TRIEN_KHAI_IMMERSIVE_FRONTEND.md`](file:///d:/University%20Study%20Subjects/Information%20System%20Design%20Analysis/Project/Analyze%20System/mobile-project/dotnet-maui-ver/HUONG_DAN_TRIEN_KHAI_IMMERSIVE_FRONTEND.md)
- **Thời điểm kích hoạt:** 21:20 ngày 27/10/2026
- **Thời hạn chót (Deadline):** Sáng ngày 28/10/2026 (~ 5 - 6 tiếng tác chiến tập trung)
- **Mục tiêu tối thượng:** Bám sát từng dòng, từng bảng mã của bản hướng dẫn thiết kế do Trưởng nhóm biên soạn, biến DuoNoPro thành sản phẩm thương mại đẳng cấp (Dark Luxury Fintech), sẵn sàng đóng gói Release APK và đạt điểm tối đa bảo vệ đồ án!

---

## 1. BA CHỈ THỊ SỐNG CÒN CỦA TRƯỞNG NHÓM CẦN TRIỂN KHAI NGAY

### 🎯 CHỈ THỊ 1: BÃI BỎ TRIỆT ĐỂ EMOJI/EMOTE – THAY BẰNG ICON VECTOR CHUẨN FINTECH
- **Vấn đề:** Ứng dụng hiện tại còn sót các emoji/emote Unicode (`👛`, `💸`, `📊`, `📑`, `🎙️`, `💎`, `🤖`, `🎯`, `🧠`, `⚙️`, `🔔`, `⏰`, `☕`...) làm app trông như "AI gen sơ khởi", bị lệch màu trên Android/Windows và không hỗ trợ đổi màu động (`Dynamic Tinting`).
- **Nhiệm vụ cụ thể:**
  1. Thay thế toàn bộ 4 icon trên thanh đáy [`BottomNavDockView.xaml`](file:///d:/University%20Study%20Subjects/Information%20System%20Design%20Analysis/Project/Analyze%20System/mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/Views/Components/BottomNavDockView.xaml) và nút tròn Micro Voice AI trung tâm bằng vector shapes/icon chuyên nghiệp.
  2. Thay thế toàn bộ các icon phân nhóm trong Sidebar Hamburger [`AppShell.xaml`](file:///d:/University%20Study%20Subjects/Information%20System%20Design%20Analysis/Project/Analyze%20System/mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/AppShell.xaml).
  3. Thay thế các emote trên các nút bấm Quick Action trong [`WalletsPage.xaml`](file:///d:/University%20Study%20Subjects/Information%20System%20Design%20Analysis/Project/Analyze%20System/mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/Views/WalletsPage.xaml) và [`ExpensesPage.xaml`](file:///d:/University%20Study%20Subjects/Information%20System%20Design%20Analysis/Project/Analyze%20System/mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/Views/ExpensesPage.xaml).

---

### 🎯 CHỈ THỊ 2: XÓA SẠCH MÃ KỸ THUẬT `(UC-XX)` & THANH LỌC VĂN PHONG DEMO/AI
- **Vấn đề:** Các tiêu đề trang và nhãn thẻ vẫn còn dính mã ca sử dụng kỹ thuật (`UC-03.07`, `UC-02.08`, `UC-09.05`, `UC-08.01`...) và các thuật ngữ trừu tượng kiểu AI ("kiểm toán toán học E2", "ma sát nhận thức", "thác nước waterfall flow"...).
- **Nhiệm vụ cụ thể (Chuyển đổi 100% sang ngôn ngữ tài chính thực tế):**
  - `Bóc tách hóa đơn AI (UC-03.07/03.08)` $\rightarrow$ **Quét Hóa Đơn Thông Minh**
  - `Bóc tách bảng lương (UC-02.08)` $\rightarrow$ **Bóc Tách & Đối Soát Bảng Lương**
  - `Khoảng trễ 24h (UC-09.05)` $\rightarrow$ **Khoảng Trễ Mua Sắm 24 Giờ**
  - `Cú hích tài chính & Trợ lý XAI` $\rightarrow$ **Trợ Lý Gợi Ý Tài Chính**
  - `Dự báo dòng tiền (UC-08.01 - 08.04)` $\rightarrow$ **Dự Báo & Mô Phỏng Tài Chính**
  - `Tiện ích & Trợ lý AI` $\rightarrow$ **Tiện Ích VietQR & Quản Lý Gói Đăng Ký**
  - `Hạn mức thích ứng DSA quy hoạch động` $\rightarrow$ **Hạn Mức Chi Tiêu Hôm Nay**
  - `Bẫy thuê bao Zombie Subscriptions Trap` $\rightarrow$ **Quản Lý Gói Dịch Vụ Định Kỳ**
  - `Hộp thư tự hành Jev Protocol Action Inbox` $\rightarrow$ **Việc Cần Xử Lý**
- **Bố trí nút Test/Demo:** Gom toàn bộ các nút test chuông, giả lập MoMo về khung phụ viền nét đứt tinh tế: `"🛠️ Công Cụ Trải Nghiệm Demo (Dành Cho Giám Khảo)"` ở chân trang.

---

### 🎯 CHỈ THỊ 3: TÁI CẤU TRÚC TRANG BÁO CÁO VỚI BỘ BIỂU ĐỒ TRỰC QUAN HÓA
- **Vấn đề:** [`ReportsPage.xaml`](file:///d:/University%20Study%20Subjects/Information%20System%20Design%20Analysis/Project/Analyze%20System/mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/Views/ReportsPage.xaml) hiện tại quá nhiều chữ và bảng số khô khan, thiếu biểu đồ trực quan để gây ấn tượng với Hội đồng.
- **Nhiệm vụ cụ thể:**
  1. **Khối Biểu đồ Donut / Cơ cấu Danh mục Chi tiêu:** Tích hợp biểu đồ phân bổ trực quan (Ăn uống, Nhà ở, Mua sắm, Di chuyển...) kèm bảng chú thích màu sắc (Color Legend).
  2. **Khối Biểu đồ Cột So Sánh Thu - Chi:** Trực quan hóa tiền vào (Xanh Emerald `#10B981`) vs tiền ra (Đỏ Rose `#F43F5E`) theo các tháng gần nhất.
  3. **Khối Biểu đồ Tiến độ Mô hình 50/30/20:** Thanh Multi-segment Gradient phân tầng Nhu cầu thiết yếu - Mong muốn cá nhân - Tiết kiệm.
  4. **Thẻ Tài sản ròng & Dòng tiền thuần:** Bố trí 2 thẻ KPI Hero song song ngay đầu trang.

---

## 2. BẢN ĐỒ LỘ TRÌNH 4 BƯỚC TÁC CHIẾN ĐÊM NAY (EXECUTION TIMELINE)

```text
┌────────────────────────────────────────────────────────────────────────────────────────┐
│             LỘ TRÌNH TÁC CHIẾN ĐÊM 27/10: HOÀN THIỆN ĐỒ ÁN TRƯỚC SÁNG 28/10           │
│                                                                                        │
│  [BƯỚC 1: 21:30 - 22:30] CHUẨN HÓA BẢNG MÀU, XÓA MÃ UC VÀ ICON VECTOR CHO SHELL        │
│  - Cập nhật Colors.xaml & Styles.xaml theo chuẩn Dark Luxury Fintech (#0B0F19, #1E293B)│
│  - Bãi bỏ emoji trên BottomNavDockView.xaml & Hamburger Sidebar AppShell.xaml          │
│  - Thay thế toàn bộ mã (UC-XX) và câu từ AI sáo rỗng trên toàn bộ 20 màn hình          │
│  - Kiểm toán biên dịch & chạy 82 Unit Tests MAUI                                       │
│                                                                                        │
│  [BƯỚC 2: 22:30 - 23:30] SPRINT THẨM MỸ 4 TRANG TAB CHÍNH & BIỂU ĐỒ BÁO CÁO           │
│  - WalletsPage.xaml: Hero Obsidian Card, Action Inbox, Danh sách ví Apple Card         │
│  - ExpensesPage.xaml & IncomesPage.xaml: Timeline giao dịch mượt mà                    │
│  - BudgetsPage.xaml: DSA Widget nhiệt kế hạn mức chi tiêu                              │
│  - ReportsPage.xaml: Tái cấu trúc thành Trung tâm Điều hành Tài chính với Biểu đồ      │
│  - Kiểm toán biên dịch & chạy 82 Unit Tests MAUI                                       │
│                                                                                        │
│  [BƯỚC 3: 23:30 - 00:30] TÚT TÁT CÁC TRANG CÒN LẠI (OCR, NUDGE, SETTINGS) & BUILD APK   │
│  - ReceiptOcrPage & PayslipOcrPage: Bảng đối soát chi tiết, cờ cảnh báo số học         │
│  - CoolingOffHubPage & NudgeFeedPage: Khóa mua sắm 24h & Thaler Nudge                  │
│  - Đóng gói bản phát hành chính thức: ExpenseTracker.apk (Release signed)              │
│                                                                                        │
│  [BƯỚC 4: 00:30 - 02:00] HỒ SƠ ĐỒ ÁN: CẬP NHẬT ẢNH BÁO CÁO & KỊCH BẢN VIDEO DEMO       │
│  - Chụp ảnh màn hình các trang UI mới trên Android Emulator đưa vào BaoCao.docx        │
│  - Soạn kịch bản phân cảnh Video Demo 5-8 phút chuẩn barem yeucau-cuoiky.txt           │
│  - Tổng kết & Bàn giao bộ sản phẩm hoàn chỉnh                                          │
└────────────────────────────────────────────────────────────────────────────────────────┘
```

---

## 3. CHECKLIST KIỂM TOÁN TỪNG MÀN HÌNH THEO HUONG_DAN_TRIEN_KHAI_IMMERSIVE_FRONTEND.MD

| Màn Hình (View) | Tiêu Đề Cũ / Lỗi Cần Xóa | Tiêu Đề & Yêu Cầu Mới Chuẩn Fintech | Trạng Thái |
| :--- | :--- | :--- | :---: |
| [`BottomNavDockView.xaml`](file:///d:/University%20Study%20Subjects/Information%20System%20Design%20Analysis/Project/Analyze%20System/mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/Views/Components/BottomNavDockView.xaml) | Emoji `👛`, `💸`, `🎙️`, `📊`, `📑` | Vector Icon: Ví da, Hóa đơn, Micro tròn FAB, Biểu đồ | ⏳ Chờ chạy |
| [`AppShell.xaml`](file:///d:/University%20Study%20Subjects/Information%20System%20Design%20Analysis/Project/Analyze%20System/mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/AppShell.xaml) | Emoji `💎`, `🤖`, `🎯`, `🧠`, `⚙️` | Vector Icon cho 5 nhóm phân cấp, item height 44px | ⏳ Chờ chạy |
| [`WalletsPage.xaml`](file:///d:/University%20Study%20Subjects/Information%20System%20Design%20Analysis/Project/Analyze%20System/mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/Views/WalletsPage.xaml) | Emoji nút action, text kỹ thuật | Thẻ Obsidian VIP, "Việc cần xử lý", ẩn/hiện số dư | ⏳ Chờ chạy |
| [`ExpensesPage.xaml`](file:///d:/University%20Study%20Subjects/Information%20System%20Design%20Analysis/Project/Analyze%20System/mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/Views/ExpensesPage.xaml) | Emoji, chữ số quá to | Timeline ngày, tiền đỏ `-50.000đ`, badge danh mục | ⏳ Chờ chạy |
| [`BudgetsPage.xaml`](file:///d:/University%20Study%20Subjects/Information%20System%20Design%20Analysis/Project/Analyze%20System/mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/Views/BudgetsPage.xaml) | `DSA quy hoạch động` | **Hạn Mức Chi Tiêu Hôm Nay**, nhiệt kế ngân sách | ⏳ Chờ chạy |
| [`ReportsPage.xaml`](file:///d:/University%20Study%20Subjects/Information%20System%20Design%20Analysis/Project/Analyze%20System/mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/Views/ReportsPage.xaml) | Khô khan, toàn số, mã `UC-07.05` | **Báo Cáo & Phân Tích**, Biểu đồ Donut & Cột kép | ⏳ Chờ chạy |
| [`ReceiptOcrPage.xaml`](file:///d:/University%20Study%20Subjects/Information%20System%20Design%20Analysis/Project/Analyze%20System/mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/Views/ReceiptOcrPage.xaml) | `Bóc Tách Hóa Đơn AI (UC-03.07/03.08)` | **Quét Hóa Đơn Thông Minh**, Bảng đối soát món hàng | ⏳ Chờ chạy |
| [`PayslipOcrPage.xaml`](file:///d:/University%20Study%20Subjects/Information%20System%20Design%20Analysis/Project/Analyze%20System/mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/Views/PayslipOcrPage.xaml) | `Bóc Tách Bảng Lương (UC-02.08)` | **Bóc Tách & Đối Soát Bảng Lương** | ⏳ Chờ chạy |
| [`CoolingOffHubPage.xaml`](file:///d:/University%20Study%20Subjects/Information%20System%20Design%20Analysis/Project/Analyze%20System/mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/Views/CoolingOffHubPage.xaml) | `Khoảng Trễ 24h (UC-09.05)` | **Khoảng Trễ Mua Sắm 24 Giờ**, đếm ngược thời gian | ⏳ Chờ chạy |
| [`NudgeFeedPage.xaml`](file:///d:/University%20Study%20Subjects/Information%20System%20Design%20Analysis/Project/Analyze%20System/mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/Views/NudgeFeedPage.xaml) | `Cú Hích Tài Chính & Trợ Lý XAI` | **Trợ Lý Gợi Ý Tài Chính** | ⏳ Chờ chạy |
| [`ForecastingPage.xaml`](file:///d:/University%20Study%20Subjects/Information%20System%20Design%20Analysis/Project/Analyze%20System/mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/Views/ForecastingPage.xaml) | `Dự Báo Dòng Tiền (UC-08.01 - 08.04)` | **Dự Báo & Mô Phỏng Tài Chính** | ⏳ Chờ chạy |
| [`SmartUtilitiesPage.xaml`](file:///d:/University%20Study%20Subjects/Information%20System%20Design%20Analysis/Project/Analyze%20System/mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/Views/SmartUtilitiesPage.xaml) | `Bẫy thuê bao Zombie Subscriptions` | **Tiện Ích VietQR & Quản Lý Gói Đăng Ký** | ⏳ Chờ chạy |
| [`SavingsGoalsPage.xaml`](file:///d:/University%20Study%20Subjects/Information%20System%20Design%20Analysis/Project/Analyze%20System/mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/Views/SavingsGoalsPage.xaml) | `Mục Tiêu Tích Lũy (UC-06.01 - 06.05)` | **Hũ Tiết Kiệm & Mục Tiêu Tích Lũy** | ⏳ Chờ chạy |
| [`SettingsPage.xaml`](file:///d:/University%20Study%20Subjects/Information%20System%20Design%20Analysis/Project/Analyze%20System/mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/Views/SettingsPage.xaml) | Các nút test phân tán | Khung viền nét đứt tinh tế dành riêng cho Giám Khảo | ⏳ Chờ chạy |

---

Bạn hãy bấm **Proceed** hoặc gửi xác nhận để chúng ta nổ phát súng đầu tiên: **Bước 1 - Chuẩn hóa bảng màu, thay icon vector cho BottomNavDockView & AppShell, và xóa sạch mã UC-XX!**
