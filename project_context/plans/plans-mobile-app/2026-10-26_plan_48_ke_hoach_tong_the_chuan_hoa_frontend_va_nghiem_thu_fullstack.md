# 📋 KẾ HOẠCH TRIỂN KHAI TOÀN DIỆN (PLAN-48)
## CHUẨN HÓA THẨM MỸ FRONTEND MAUI, GHÉP NỐI MVVM & NGHIỆM THU FULL-STACK CUỐI KỲ
**Dự án:** Smart Personal Expense & Financial Planning Management System (.NET MAUI 10 + ASP.NET Core 10)  
**Tài liệu căn cứ:** [`README.md`](../../mobile-project/dotnet-maui-ver/README.md), [`HUONG_DAN_FE.md`](../../mobile-project/dotnet-maui-ver/HUONG_DAN_FE.md), [`yeucau-cuoiky.txt`](../../mobile-project/dotnet-maui-ver/yeucau-cuoiky.txt)  
**Ngày lập:** 2026-10-26  
**Chủ trì:** Council Orchestrator (Agent 00) cùng Hội đồng Kỹ nghệ ECC  

---

### 1. TÁI HIỆN YÊU CẦU & BỐI CẢNH (REQUIREMENTS RESTATEMENT)

Sau khi nghiên cứu chuyên sâu 2 văn bản cốt lõi:
1. **`README.md`:** 
   - Kiến trúc giải pháp tổng thể: Backend Clean Architecture ASP.NET Core 10, CSDL PostgreSQL 16 (Docker), Frontend Clean MVVM .NET MAUI 10 (WinUI 3 Desktop / Android).
   - Cơ chế hoán đổi **1-chạm** giữa **Mock Data Offline (0ms)** và **API Backend Thật** trong `MauiProgram.cs`.
   - Toàn bộ nền tảng nghiệp vụ 63 Use Cases đã hoàn tất, 254 Unit Tests Backend và 42 Unit Tests Mobile đã đỗ 100%.
2. **`HUONG_DAN_FE.md`:**
   - Phân định rõ ràng: Toàn bộ 20 màn hình `.xaml` hiện có là **bộ khung sườn mẫu (Wireframe / Data Anchors)**.
   - **Nguyên tắc bất di bất dịch:** Người thiết kế toàn quyền tự do sáng tạo mỹ thuật (Layout, Typography, Palette, Theme, Animation, Card Views), nhưng **phải bảo toàn nguyên vẹn 100% mỏ neo dữ liệu** (tên thuộc tính `{Binding Property}` và lệnh tương tác `{Binding Command}`).
   - Quy chuẩn 4 trạng thái giao diện: *Loading (`IsBusy`)*, *Error (`HasError`, `ErrorMessage`)*, *Success (`StatusMessage`)*, và *Empty State (`CollectionView.EmptyView`)*.
3. **Barem môn học Lập trình Mobile (`yeucau-cuoiky.txt`):**
   - Đạt trọn vẹn điểm tối đa (10.0 + 1.0 điểm cộng):
     - *Phần A (2.0đ):* Push Notifications & Local Scheduled Reminder 21:00.
     - *Phần B2 (Cộng +1.0đ):* Realtime SignalR Hub hai chiều, tự động kết nối lại, cập nhật 0ms.
     - *Phần B3 (2.0đ):* Kiến trúc Clean MVVM 3 tầng Repositories & ViewModel Unit Tests.
     - *Báo cáo & Video Demo (2.0đ):* Báo cáo Word/PDF và Video 5–8 phút.

---

### 2. ĐỐI CHIẾU QUY CHUẨN MÃ NGUỒN HIỆN HỮU (CODEBASE PATTERNS TO MIRROR)

| Hạng Mục | Vị Trí Tham Chiếu | Quy Chuẩn Thiết Kế Cần Bám Sát |
| :--- | :--- | :--- |
| **MVVM Base** | `ExpenseTracker.Maui/ViewModels/BaseViewModel.cs` | Kế thừa `ObservableObject`, quản lý tập trung `IsBusy`, `HasError`, `ErrorMessage`, `StatusMessage`. |
| **Data Anchors** | `ExpenseTracker.Maui/HUONG_DAN_FE.md:85-337` | Giữ nguyên 100% Binding paths và RelayCommands cho cả 20 màn hình. |
| **Design Tokens** | `ExpenseTracker.Maui/Resources/Styles/Colors.xaml` & `Styles.xaml` | Quản lý bảng màu Slate/Charcoal, Emerald (Thu), Rose (Chi), Indigo (Chủ đạo). |
| **Repository Pattern** | `ExpenseTracker.Maui/Repositories/` | Cặp đôi song song `Mock*Repository` (in-memory) và `Api*Repository` (Refit API Client). |
| **Unit Testing** | `ExpenseTracker.Maui.UnitTests/` | Kiểm thử ViewModel bằng xUnit, FluentAssertions, Moq repository. |

---

### 3. KẾ HOẠCH TRIỂN KHAI THEO 5 PHA (STEP-BY-STEP IMPLEMENTATION PHASES)

#### PHA 1: THIẾT LẬP NỀN TẢNG THIẾT KẾ & COMPONENT DÙNG CHUNG (DESIGN SYSTEM FOUNDATION)
- **Nhiệm vụ 1.1:** Chuẩn hóa `Colors.xaml` và `Styles.xaml` với hệ màu hiện đại (Modern Fintech Dark/Light Theme, Slate Background, High-Contrast Text, Semantic Accent Colors).
- **Nhiệm vụ 1.2:** Xây dựng các Control dùng chung chuẩn Impeccable:
  - `LoadingOverlay`: Lớp phủ xoay loading khi `IsBusy = true`.
  - `ErrorBanner`: Thanh thông báo lỗi tinh tế gắn `HasError` và `ErrorMessage`.
  - `EmptyStateView`: Khung hiển thị danh sách rỗng thân thiện (icon minh họa + text hướng dẫn).
  - `MetricCard`: Thẻ thống kê tài chính chỉ số lớn kèm badge tăng trưởng.

#### PHA 2: CHUẨN HÓA MỸ THUẬT 20 MÀN HÌNH THEO 4 NHÓM PHÂN HỆ (UI/UX POLISHING)
- **Nhóm 1 - Xác thực & Trang chủ (Dashboard):**
  - `LoginPage.xaml` & `RegisterPage.xaml`: Thiết kế form nhập liệu sang trọng, ẩn/hiện mật khẩu, validation mượt mà.
  - `WalletsPage.xaml`: Dashboard tổng tài sản, thẻ ví ngân hàng/tiền mặt dạng thẻ ATM hiện đại, tích hợp khay `ActionInboxView` (Jev Actions 1-chạm).
- **Nhóm 2 - Thu Nhập, Chi Tiêu & Multimodal AI:**
  - `IncomesPage.xaml` & `ExpensesPage.xaml`: Timeline giao dịch phân loại màu sắc, bộ lọc danh mục trực quan.
  - `PayslipOcrPage.xaml` & `ReceiptOcrPage.xaml`: Giao diện bóc tách camera/thư viện ảnh, bảng đối soát chi tiết món hàng, cờ cảnh báo sai lệch số học màu vàng.
  - `SmartUtilitiesPage.xaml`: Giao diện giọng nói Voice STT, quét bẫy thuê bao ngủ quên, tạo mã VietQR Napas 247.
- **Nhóm 3 - Danh Mục, Ngân Sách DSA & Quỹ Tích Lũy:**
  - `CategoriesPage.xaml` & `AutoRulesPage.xaml`: Cây danh mục cha-con phân tầng, bảng từ khóa thông minh.
  - `BudgetsPage.xaml` & `CreateBudgetPage.xaml`: Thanh tiến độ ngân sách nhiệt, widget Hạn mức chi tiêu mỗi ngày (DSA) đàn hồi.
  - `SavingsGoalsPage.xaml` & `CreateSavingsGoalPage.xaml`: Thẻ mục tiêu dạng hũ tài chính Kakeibo, tiến độ % kèm nút nạp/rút tiền có ma sát Nudge.
- **Nhóm 4 - Phân Tích, Dự Báo, Cú Hích & Thông Báo:**
  - `ReportsPage.xaml` & `ForecastingPage.xaml`: Biểu đồ trực quan, phân tích Net Worth, danh sách Outlier Z-Score, radar thanh khoản.
  - `NudgeFeedPage.xaml`: Bảng tin Cú hích hành vi Thaler Nudge kèm Modal giải trình XAI minh bạch công thức toán.
  - `CoolingOffHubPage.xaml`: Khay phong tỏa mua sắm 24h, đếm ngược thời gian, quy đổi giờ làm việc.
  - `NotificationHistoryPage.xaml`: Lịch sử thông báo đẩy, phân loại theo độ khẩn, nút test demo cho giảng viên.

#### PHA 3: KIỂM TOÁN TÍNH TOÀN VẸN MVVM & BỘ TEST TỰ ĐỘNG (VERIFICATION LOOP)
- **Nhiệm vụ 3.1:** Biên dịch MAUI Frontend (`net10.0-windows10.0.19041.0`): Đảm bảo **0 Warning, 0 Error**.
- **Nhiệm vụ 3.2:** Chạy toàn bộ 42 Unit Tests trong `ExpenseTracker.Maui.UnitTests`: Đảm bảo **100% Passed**.

#### PHA 4: KIỂM TOÁN TÍCH HỢP FULL-STACK E2E (REALTIME SIGNALR & API THẬT)
- **Nhiệm vụ 4.1:** Khởi động PostgreSQL 16 qua Docker Compose và chạy WebApi Backend.
- **Nhiệm vụ 4.2:** Chuyển đổi sang `Api*Repository` trong `MauiProgram.cs`, kiểm tra:
  - Đồng bộ số dư tức thời 0ms giữa 2 thiết bị qua SignalR Hub (`FinancialHub`).
  - Giao dịch thực tế nạp vào PostgreSQL, cập nhật hạn mức DSA thời gian thực.
  - Động cơ Jev nhận diện thông báo ngân hàng và sinh thẻ Quick Commit.

#### PHA 5: ĐÓNG GÓI SẢN PHẨM NGHIỆM THU & BÁO CÁO CUỐI KỲ
- **Nhiệm vụ 5.1:** Xây dựng script PowerShell `run-demo.ps1` tự động hóa 1-click khởi chạy toàn bộ Backend + DB + App cho buổi chấm thi.
- **Nhiệm vụ 5.2:** Cập nhật tài liệu Báo cáo cuối kỳ [`BAO_CAO_CUOI_KY_DOTNET_MAUI.docx`](../../mobile-project/dotnet-maui-ver/BAO_CAO_CUOI_KY_DOTNET_MAUI.docx) đáp ứng 100% barem điểm.
- **Nhiệm vụ 5.3:** Chuẩn bị kịch bản phân cảnh Video Demo 5–8 phút.

---

### 4. ĐÁNH GIÁ RỦI RO & BIỆN PHÁP PHÒNG VỆ (RISKS & MITIGATION)

| Rủi Ro Nhận Diện | Mức Độ | Biện Pháp Phòng Vệ |
| :--- | :---: | :--- |
| **Mất Binding khi làm đẹp XAML** | **CAO** | Sử dụng `x:DataType` (Compiled Bindings) để bắt lỗi cú pháp ngay khi build; chạy 42 Unit Tests ViewModel sau mỗi màn hình. |
| **Thiếu Workload MAUI trên môi trường** | **TRUNG BÌNH** | Đã hướng dẫn người dùng nhấn Modify trên Visual Studio Installer; Backend luôn chạy độc lập kiểm thử được trước. |
| **Mất kết nối mạng/API lúc demo** | **THẤP** | Đã có sẵn 11 Mock Repositories offline 0ms trong `MauiProgram.cs`, đảm bảo buổi bảo vệ luôn mượt mà. |

---

### 5. TIÊU CHÍ NGHIỆM THU (ACCEPTANCE CRITERIA)
- [x] Giao diện toàn bộ 20 màn hình và view con đạt tính thẩm mỹ cao cấp (Modern Fintech), bố cục hài hòa, hỗ trợ chế độ Sáng/Tối (AppThemeBinding) và đủ 4 trạng thái UI (Loading, Error, Success, Empty State).
- [x] 100% Data Bindings và RelayCommands hoạt động chính xác, bảo toàn trọn vẹn mỏ neo logic với Mock Data lẫn API thật.
- [x] Bộ 254 Unit Tests Backend (83 Domain + 171 Application) đạt **100% Green**.
- [ ] Script `run-demo.ps1` chạy mượt mà trong 1 click.
- [ ] Báo cáo Word hoàn chỉnh theo barem điểm môn học.

