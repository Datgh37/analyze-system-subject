# KẾ HOẠCH TRIỂN KHAI PLAN-59
## HỢP NHẤT GIAO DIỆN TINH HOA (BEST OF BOTH WORLDS) & CHUẨN HÓA API ENDPOINTS (TRIỆT TIÊU LỖI 404/400)

> **Mã kế hoạch:** `PLAN-59`  
> **Thời gian khởi tạo:** 23:10 Ngày 27/10/2026  
> **Chủ đề:** Hợp nhất giao diện di động .NET MAUI 10 giữa bản thiết kế của thành viên nhóm (`f014305`) và bản chuẩn hóa nội dung của Trưởng nhóm (Stash), chuẩn hóa toàn diện API Client để loại bỏ lỗi 404/400.  
> **Cam kết:** Giữ nguyên 100% tính toàn vẹn của Backend, Không dùng tiền tố `/v1`, 100% Personal Finance, Bảo toàn 82/82 Unit Tests.

---

### BỐI CẢNH & MỤC TIÊU CỐT LÕI

1. **Thành viên nhóm vừa đẩy lên commit `f014305`:**
   - **Ưu điểm lớn:** Bộ 28 file Icon Vector SVG rất đẹp trong `Resources/Images/`, thanh Bottom Navigation Dock (`BottomNavDockView.xaml`) có nút tròn Micro nhô cao công thái học chuẩn, đã sửa lỗi căn giữa nút bấm `<Border>`.
   - **Nhược điểm tồn đọng:** Nội dung các trang vẫn còn sót lại nhiều Emoji Unicode thô (`⚡`, `⚠️`, `🚨`), thuật ngữ hàn lâm kỹ thuật (`ACID`, `Z-Score`, `UC-XX`), và màn hình Báo cáo thiếu biểu đồ trực quan hóa dữ liệu.
2. **Bản Stash của Trưởng nhóm (`stash@{0}`):**
   - Nội dung cực kỳ súc tích, văn phong tài chính cá nhân thương mại (như Revolut/Apple Card), đã xóa sạch 100% `UC-XX` và thuật ngữ thô, có thanh tiến trình so sánh Thu - Chi và phân bổ 50/30/20 trên Báo cáo, cách ly công cụ demo xuống chân trang với viền nét đứt.
3. **Các lỗi kết nối đã soi ra trong Terminal Backend:**
   - Lỗi **404**: `IApiClients.cs` đang gọi thừa `/api/v1/nudges` và `/api/v1/cooling-off/pending` trong khi Backend là `/api/nudges` và `/api/cooling-off`.
   - Lỗi **400**: Endpoint `/api/forecasting/what-if` bị lỗi deserialize enum chuỗi JSON do Backend thiếu `JsonStringEnumConverter()`.

---

### LỘ TRÌNH 4 BƯỚC TRIỂN KHAI CHI TIẾT

```
┌────────────────────────────────────────────────────────────────────────┐
│ BƯỚC 1: Chuẩn hóa API Endpoints & Sửa dứt điểm lỗi 404 / 400           │
│  • Bỏ '/v1' trong IApiClients.cs (INudgeApi & ICoolingOffApi)          │
│  • Thêm JsonStringEnumConverter() vào AddControllers() ở Backend       │
├────────────────────────────────────────────────────────────────────────┤
│ BƯỚC 2: Hợp nhất Giao diện Tinh hoa (Select & Merge)                  │
│  • Giữ nguyên: BottomNavDockView.xaml & bộ 28 SVG Icons của bạn        │
│  • Lấy từ Stash: Bố cục trực quan, bỏ 100% emoji & thuật ngữ hàn lâm   │
│  • Chuẩn hóa: Thước đo phân bổ 50/30/20 & Cách ly Demo Tool chân trang │
├────────────────────────────────────────────────────────────────────────┤
│ BƯỚC 3: Kiểm thử Toàn diện & Xác thực Trải nghiệm                      │
│  • Chạy 82/82 MAUI Unit Tests (Bảo đảm Zero Regression)                │
│  • Chạy kiểm thử trên Máy ảo Android (Xác nhận 0 lỗi 404 / 400)        │
├────────────────────────────────────────────────────────────────────────┤
│ BƯỚC 4: Đóng gói Release APK & Hoàn thiện Báo cáo                     │
│  • Xuất bản ExpenseTracker.apk chính thức                             │
│  • Chụp ảnh màn hình mới chèn vào BaoCao.docx                          │
└────────────────────────────────────────────────────────────────────────┘
```

---

### CHI TIẾT CÁC ĐẦU VIỆC (TASK BREAKDOWN)

#### BƯỚC 1: Chuẩn Hóa API Endpoints & Sửa Lỗi 404 / 400
1. **Frontend (`frontend/ExpenseTracker.Maui/Services/Api/IApiClients.cs`):**
   - Đổi `[Get("/api/v1/nudges")]` $\rightarrow$ `[Get("/api/nudges")]`.
   - Đổi `[Get("/api/v1/nudges/{id}/xai")]` $\rightarrow$ `[Get("/api/nudges/{id}/xai")]`.
   - Đổi `[Get("/api/v1/nudges/rebalance/{deficitCategoryId}")]` $\rightarrow$ `[Get("/api/nudges/rebalance/{deficitCategoryId}")]`.
   - Đổi `[Post("/api/v1/nudges/{id}/approve")]` $\rightarrow$ `[Post("/api/nudges/{id}/approve")]`.
   - Đổi `[Post("/api/v1/nudges/{id}/dismiss")]` $\rightarrow$ `[Post("/api/nudges/{id}/dismiss")]`.
   - Đổi các route Cooling-Off: `[Post("/api/v1/cooling-off/evaluate")]` $\rightarrow$ `[Post("/api/cooling-off/evaluate")]`, `[Get("/api/v1/cooling-off/pending")]` $\rightarrow$ `[Get("/api/cooling-off/pending")]`, v.v...
2. **Backend (`backend/ExpenseTracker.WebApi/Program.cs`):**
   - Cấu hình `builder.Services.AddControllers().AddJsonOptions(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });`
   - Đảm bảo endpoint `/api/forecasting/what-if` chấp nhận mượt mà enum chuỗi từ mobile client.

---

#### BƯỚC 2: Hợp Nhất Giao Diện Tinh Hoa (Merge Best of Both Worlds)
1. **Nhóm thành phần GIỮ NGUYÊN từ commit mới (`f014305`):**
   - `frontend/ExpenseTracker.Maui/Views/Components/BottomNavDockView.xaml` (Navbar chuẩn với nút tròn nhô cao cradle ring).
   - Toàn bộ 28 tệp `ic_*.svg` trong `Resources/Images/`.
   - `ExpenseTracker.Maui.csproj` cấu hình resize SVG 24x24dp.
2. **Nhóm trang lấy nội dung tinh chỉnh từ bản Stash:**
   - **`WalletsPage.xaml`:** Giữ nguyên header navbar của bạn, lấy nội dung thẻ Hero số dư tinh tế, bỏ `(ACID)`, thay emoji `⚡` bằng thông báo thanh lịch.
   - **`ExpensesPage.xaml` & `IncomesPage.xaml`:** Giữ nguyên nút xóa vector `ic_trash.png` của bạn; lấy phần bố trí form chi tiêu tinh gọn, bỏ mã kỹ thuật và cách ly bộ giả lập biến động ngân hàng xuống chân trang với viền nét đứt.
   - **`BudgetsPage.xaml`:** Chuyển đổi các thuật ngữ DSA/Velocity sang ngôn ngữ tài chính thường nhật ("Hạn Mức Chi Tiêu Hôm Nay").
   - **`ReportsPage.xaml`:** Giữ nguyên 2 nút "Chụp Tài Sản" & "Làm Mới" của bạn; tích hợp thêm thanh tiến trình so sánh Thu - Chi và thước đo phân bổ 50/30/20 nhiều màu.
   - **`NudgeFeedPage.xaml` & `CoolingOffHubPage.xaml`:** Giữ nguyên các nút vector `ic_xai`, `ic_check` của bạn; làm sạch tiêu đề và giải thích lý do đề xuất gần gũi.
   - **`SmartUtilitiesPage.xaml` & `SavingsGoalsPage.xaml`:** Thay thế các emoji `🚨 Bẫy thuê bao`, `🐷` bằng icon vector đồng bộ và thuật ngữ chuẩn quản lý thuê bao định kỳ.
   - **`LoginPage.xaml` & `RegisterPage.xaml`:** Sử dụng logo kim cương vector `ic_diamond.png`, nút Google `ic_google.png`, chuẩn hóa các nút chuyển đổi Localhost/Tunnel/Ping.

---

#### BƯỚC 3: Kiểm Thử Fullstack & Đảm Bảo Không Hỏng Logic (Zero Regression)
1. **Chạy Unit Tests tự động:**
   - Chạy `dotnet test frontend/tests/ExpenseTracker.Maui.UnitTests/ExpenseTracker.Maui.UnitTests.csproj` $\rightarrow$ Đạt 82/82 Tests Passed.
2. **Kiểm tra trực tiếp kết nối với Backend:**
   - Kích hoạt app trên máy ảo Android, đăng nhập và điều hướng qua các trang Nudge, Cooling-Off, What-If Forecast.
   - Xác nhận trên Terminal Backend log không còn bất kỳ dòng `404` hay `400` nào.

---

#### BƯỚC 4: Đóng Gói Release APK & Hoàn Thiện Tài Liệu
1. **Đóng gói file cài đặt:**
   - Chạy `dotnet publish frontend/ExpenseTracker.Maui/ExpenseTracker.Maui.csproj -f net10.0-android -c Release /p:AndroidPackageFormat=apk`.
   - Copy file APK hoàn thiện ra thư mục gốc `ExpenseTracker.apk`.
2. **Chụp ảnh màn hình mới & Cập nhật Báo cáo:**
   - Dùng lệnh ADB chụp ảnh thực tế các màn hình đã hợp nhất hoàn hảo.
   - Cập nhật vào báo cáo `BaoCao.docx`.
