# HƯỚNG DẪN TRIỂN KHAI ĐIỀU CHỈNH IMMERSIVE FRONTEND (.NET MAUI 10)
## DÀNH CHO THÀNH VIÊN PHỤ TRÁCH FRONTEND & ART DIRECTION

> **Dự án:** DuoNoPro - Hệ Thống Quản Lý Chi Tiêu & Hoạch Định Tài Chính Cá Nhân Thông Minh (Tích hợp AI)  
> **Phiên bản:** 2.0 (Hậu Kế hoạch 52 & Kế hoạch 53)  
> **Đối tượng sử dụng:** Bạn phụ trách Frontend tiếp quản và hoàn thiện giao diện ứng dụng di động.  
> **Nguyên tắc cốt lõi:** 100% Personal Finance (Không chia tiền nhóm, không quỹ chung), Trải nghiệm mượt mà, Thẩm mỹ Dark Luxury Fintech.

---

## 1. TỔNG QUAN KIẾN TRÚC GIAO DIỆN & BẢN SKETCH CỦA TRƯỞNG NHÓM

Mô hình bố cục chính của ứng dụng được xây dựng theo chuẩn công thái học (One-handed Ergonomics) dựa trên bản phác thảo tay của Trưởng nhóm:

```
┌────────────────────────────────────────────────────────┐
│  [≡] DuoNoPro                 [Hạn mức DSA: 450k/ngày] │ <-- Header Bar (Flyout & Status)
├────────────────────────────────────────────────────────┤
│                                                        │
│                                                        │
│                  VÙNG NỘI DUNG CHÍNH                   │
│             (ScrollView với Padding Đáy)               │
│                                                        │
│   • Thẻ Số Dư Hero Obsidian VIP                        │
│   • Lối tắt thao tác nhanh (Quick Action Pills)        │
│   • Danh sách giao dịch / Biểu đồ trực quan           │
│                                                        │
│                                                        │
├────────────────────────────────────────────────────────┤
│   [👛 Ví]    [💸 Chi]     ( 🎙️ )     [📊 Ngân]   [📑 Báo] │ <-- Bottom Nav Dock (Fixed)
├────────────────────────────────────────────────────────┤
│   \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\ │ <-- Phím điều hướng ảo Android OS
└────────────────────────────────────────────────────────┘
```

### Các Cấu Phần Cốt Lõi Đã Được Căn Chỉnh Cơ Bản:
1. **Thanh Bottom Navigation Dock (`BottomNavDockView.xaml`):**
   - Đặt cố định tại đáy màn hình, tách biệt hoàn toàn với vùng cuộn nội dung.
   - Gồm **4 Tab chức năng chính** chia đều 2 bên:
     * Cánh trái: **Ví** (`WalletsPage`) & **Chi Tiêu** (`ExpensesPage`).
     * Cánh phải: **Ngân Sách** (`BudgetsPage`) & **Báo Cáo** (`ReportsPage`).
   - **Chính giữa:** **Nút tròn Micro Voice AI Floating Action Button (FAB) (🎙️)** nhô cao hơn thanh bar 18px, đổ bóng phát sáng tím Cyan. Chạm vào mở ngay màn hình ra lệnh bằng giọng nói `VoiceMicModalPage`.
2. **Hamburger Sidebar (`AppShell.xaml`):**
   - Đã được tái cấu trúc từ danh sách phẳng 13 mục dài lê thê thành **5 Nhóm Chuyên Đề Phân Cấp Rõ Ràng**:
     * 💎 **Nhóm 1: Tài Chính Cốt Lõi (Core Finance)**
     * 🤖 **Nhóm 2: AI Vision & Tiện Ích Thông Minh**
     * 🎯 **Nhóm 3: Kỷ Luật Ngân Sách & Mục Tiêu Tích Lũy**
     * 🧠 **Nhóm 4: Tâm Lý Hành Vi & Dự Báo Dòng Tiền**
     * ⚙️ **Nhóm 5: Hệ Thống & Bảo Mật Tài Khoản**
   - Đã áp dụng `Shell.ItemTemplate` với chiều cao hàng `44px`, bo góc nhẹ, font chữ `13pt` vừa vặn, không bị tràn viền.

---

## 2. DESIGN TOKENS & TYPOGRAPHY SCALE (BỘ QUY CHUẨN THIẾT KẾ)

Để giữ cho ứng dụng đồng nhất, sang trọng và không bị "bể layout", bạn hãy tuân thủ nghiêm ngặt bộ thông số sau:

### 2.1. Bảng Màu Chuẩn (Color Palette - Dark Luxury Fintech)
| Tên Token | Mã Màu Hex | Mục Đích Sử Dụng |
| :--- | :---: | :--- |
| **Background Dark** | `#0B0F19` | Nền chính của toàn bộ trang (Deep Space Black) |
| **Surface Dark** | `#111827` | Nền thẻ phụ, thanh header phụ (Slate Gray) |
| **Card Dark** | `#1E293B` | Nền các thẻ Card nội dung chính |
| **Border Dark** | `#334155` | Viền ngăn cách giữa các phần tử |
| **Primary Brand** | `#4F46E5` | Màu thương hiệu chủ đạo (Electric Indigo) |
| **Accent Active** | `#38BDF8` | Màu nhấn kích hoạt, Tab đang chọn (Sky Blue) |
| **Income Emerald**| `#10B981` | Tiền vào, thu nhập, trạng thái an toàn |
| **Expense Rose**  | `#F43F5E` | Tiền ra, chi tiêu, cảnh báo bội chi |
| **Gold Chip**     | `#F59E0B` | Điểm nhấn thẻ VIP, hạn mức DSA |

### 2.2. Thang Kích Cỡ Chữ (Typography Scale)
> **Lưu ý quan trọng:** Không sử dụng font chữ lớn hơn `26pt` trên màn hình điện thoại để tránh bị tràn chữ (overflow) khi hiển thị số tiền hàng trăm triệu/hàng tỷ đồng.

| Cấp Bậc | FontSize Khuyên Dùng | FontAttributes | Áp Dụng Cho |
| :--- | :---: | :---: | :--- |
| **Hero Balance** | `24 - 26` | `Bold` | Số dư tổng tài sản, Tổng chi tiêu tháng |
| **Page Title** | `18 - 20` | `Bold` | Tiêu đề chính đầu trang |
| **Section Header** | `14 - 15` | `Bold` | Tiêu đề các nhóm danh sách ("Lịch sử giao dịch") |
| **Card Title / Item Name** | `13 - 14` | `Bold` | Tên ví, tên danh mục, tên khoản chi |
| **Body Text / Value** | `12` | `Regular` | Ghi chú, ngày tháng, thông tin chi tiết |
| **Caption / Metadata** | `10 - 11` | `Regular` | Subtitle, nhãn phụ ("Hạn mức DSA", "AES-256") |

### 2.3. Quy Chuẩn Đệm Lề (Padding & Margin Rhythm)
- **Container ngoài cùng của trang:** `Padding="16,12,16,24"` (hoặc tối đa `18`). Tránh đặt padding quá lớn (`24-36`) làm hẹp diện tích hiển thị nội dung trên máy có màn hình nhỏ.
- **Khoảng cách giữa các thẻ (Spacing):** Dùng `Spacing="12"` đến `Spacing="16"`.
- **Độ bo góc (CornerRadius / RoundRectangle):**
  - Thẻ Hero lớn: `RoundRectangle 20`
  - Thẻ Card nội dung: `RoundRectangle 14` hoặc `16`
  - Nút bấm (Button / Pill): `RoundRectangle 10` đến `18`

---

## 3. CẤU TRÚC THƯ MỤC FRONTEND ĐỂ BẠN LÀM VIỆC

```text
mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/
├── Views/
│   ├── Components/
│   │   └── BottomNavDockView.xaml         <-- THANH ĐIỀU HƯỚNG ĐÁY CHÍNH (Đã có sẵn nút Voice FAB)
│   ├── Autonomous/
│   │   └── ActionInboxView.xaml           <-- Hộp thư việc cần làm tự hành (Jev Protocol)
│   ├── AiAgent/
│   │   └── VoiceMicModalPage.xaml         <-- Modal Trợ lý Voice AI (Đã có 4 nút câu lệnh mẫu)
│   ├── WalletsPage.xaml                   <-- Màn hình Quản lý Đa Ví (Tab 1)
│   ├── ExpensesPage.xaml                  <-- Màn hình Chi tiêu & Giao dịch (Tab 2)
│   ├── BudgetsPage.xaml                   <-- Màn hình Ngân sách & Hạn mức DSA (Tab 3)
│   ├── ReportsPage.xaml                   <-- Màn hình Báo cáo tài chính (Tab 4)
│   ├── IncomesPage.xaml                   <-- Thu nhập cá nhân
│   ├── RecurringIncomesPage.xaml          <-- Thu nhập định kỳ
│   ├── ReceiptOcrPage.xaml                <-- Quét hóa đơn bằng Camera/Ảnh AI OCR
│   ├── PayslipOcrPage.xaml                <-- Bóc tách bảng lương
│   ├── SmartUtilitiesPage.xaml            <-- Tiện ích tạo mã VietQR & Quản lý gói Zombie Subscriptions
│   ├── SavingsGoalsPage.xaml              <-- Mục tiêu tích lũy & Hũ tiết kiệm
│   ├── CategoriesPage.xaml                <-- Cây danh mục Thu Chi
│   ├── AutoRulesPage.xaml                 <-- Quy tắc tự động phân loại
│   ├── NudgeFeedPage.xaml                 <-- Bảng tin Cú hích tài chính Thaler Nudge
│   ├── CoolingOffHubPage.xaml             <-- Không gian khoảng trễ 24h triệt tiêu mua sắm bốc đồng
│   ├── ForecastingPage.xaml               <-- Dự báo dòng tiền & Ngày cạn tiền
│   ├── NotificationHistoryPage.xaml       <-- Lịch sử thông báo & Nút test chuông báo thức 21:00
│   └── LoginPage.xaml / RegisterPage.xaml / VerifyOtpPage.xaml <-- Bộ 3 màn hình xác thực Auth OTP
│
├── ViewModels/                            <-- Tầng logic MVVM (Đã liên kết hoàn chỉnh với Repository)
├── Models/                                <-- DTO dữ liệu hiển thị
├── Repositories/                          <-- Mock & API Repositories (Đã có dữ liệu mẫu cực kỳ sống động)
└── AppShell.xaml                          <-- Cấu hình Menu Hamburger Sidebar & Điều hướng Shell
```

---

## 4. HƯỚNG DẪN CÁCH NHÚNG BOTTOM NAV DOCK VÀO CÁC TRANG MỚI

Nếu bạn tạo một trang mới hoặc muốn trang đó có thanh điều hướng đáy giống 4 trang chính:

### Bước 1: Khai báo namespace trong thẻ `<ContentPage>`
```xml
xmlns:components="clr-namespace:ExpenseTracker.Maui.Views.Components"
```

### Bước 2: Bọc nội dung trang vào `<Grid RowDefinitions="*, Auto">`
```xml
<Grid RowDefinitions="*, Auto">
    <!-- VÙNG NỘI DUNG CUỘN (Grid.Row="0") -->
    <ScrollView Grid.Row="0">
        <VerticalStackLayout Spacing="16" Padding="16,12,16,24">
            <!-- Toàn bộ nội dung của bạn đặt ở đây -->
        </VerticalStackLayout>
    </ScrollView>

    <!-- THANH BOTTOM NAV CỐ ĐỊNH (Grid.Row="1") -->
    <components:BottomNavDockView Grid.Row="1" ActiveTab="Expenses" />
</Grid>
```
*Ghi chú:* Thuộc tính `ActiveTab` có thể nhận các giá trị: `"Wallets"`, `"Expenses"`, `"Budgets"`, `"Reports"` để tự động làm sáng màu icon của tab tương ứng.

---

## 5. CHECKLIST CÔNG VIỆC GỢI Ý CHO BẠN TIẾP TỤC HOÀN THIỆN NGÀY MAI

- [ ] **1. Rà soát căn chỉnh lề (Padding/Margin) trên các trang phụ:**
  - Kiểm tra `IncomesPage.xaml`, `RecurringIncomesPage.xaml`, `SavingsGoalsPage.xaml` xem có thẻ nào bị sát viền hay không.
  - Đảm bảo tất cả các trang đều có `Padding="16,12,16,24"`.
- [ ] **2. Thẩm định kích thước font chữ trên màn hình nhỏ:**
  - Mở app trên máy ảo, lướt qua một lượt các trang để đảm bảo không có chữ nào bị rớt dòng đơn độc (orphan word).
  - Kiểm tra các nhãn số tiền lớn đều nằm gọn trong thẻ.
- [ ] **3. Trau chuốt thẻ Profile Người Dùng (Nếu muốn thêm):**
  - Hiện tại trang `LoginPage.xaml` đang dùng để quản lý đăng nhập/đăng xuất.
  - Bạn có thể thiết kế thêm một trang `ProfilePage.xaml` đơn giản hiển thị: Tên người dùng, Email đã xác thực, Avatar tròn và Nút đăng xuất.
- [ ] **4. Kiểm tra độ tương phản màu sắc (Accessibility):**
  - Chữ xám mờ trên nền tối nên dùng tối thiểu `#94A3B8` hoặc `#A5B4FC` để dễ đọc trong điều kiện ánh sáng mạnh.
  - Không dùng chữ xám quá tối (`#475569` trở xuống) cho các thông tin quan trọng.

---

## 6. LỆNH CHẠY & KIỂM TRA MÃ NGUỒN TRONG QUÁ TRÌNH LÀM VIỆC

1. **Khởi chạy máy ảo và app nhanh (1 lệnh duy nhất):**
   Mở PowerShell tại thư mục `mobile-project/dotnet-maui-ver/` và chạy:
   ```powershell
   .\run_android.ps1
   ```
2. **Kiểm tra biên dịch xem có lỗi XAML không:**
   ```powershell
   dotnet build X:\frontend\ExpenseTracker.Maui\ExpenseTracker.Maui.csproj -f net10.0-android -c Debug -nr:false
   ```
   *Yêu cầu:* Phải luôn đạt `0 Warning(s)`, `0 Error(s)`.
3. **Chạy toàn bộ bài test unit để đảm bảo không làm gãy logic ViewModel:**
   ```powershell
   dotnet test frontend\tests\ExpenseTracker.Maui.UnitTests\ExpenseTracker.Maui.UnitTests.csproj -v minimal
   ```
   *Yêu cầu:* 82/82 Tests Passed 100%.

Chúc bạn hoàn thiện giao diện thật lung linh và ấn tượng! 🚀
