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
│   [Ví]       [Chi Tiêu]    ( Micro )    [Ngân Sách] [Báo Cáo]│ <-- Bottom Nav Dock (Fixed)
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
   - **Chính giữa:** **Nút tròn Micro Voice AI Floating Action Button (FAB)** nhô cao hơn thanh bar 18px, đổ bóng phát sáng tím Cyan. Chạm vào mở ngay màn hình ra lệnh bằng giọng nói `VoiceMicModalPage`.
2. **Hamburger Sidebar (`AppShell.xaml`):**
   - Đã được tái cấu trúc từ danh sách phẳng 13 mục dài lê thê thành **5 Nhóm Chuyên Đề Phân Cấp Rõ Ràng**:
     * **Nhóm 1: Tài Chính Cốt Lõi (Core Finance)**
     * **Nhóm 2: AI Vision & Tiện Ích Thông Minh**
     * **Nhóm 3: Kỷ Luật Ngân Sách & Mục Tiêu Tích Lũy**
     * **Nhóm 4: Tâm Lý Hành Vi & Dự Báo Dòng Tiền**
     * **Nhóm 5: Hệ Thống & Bảo Mật Tài Khoản**
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

## 3. CHỈ THỊ THẨM MỸ BẮT BUỘC: LOẠI BỎ TRIỆT ĐỂ EMOJI/EMOTE – THAY BẰNG BỘ ICON VECTOR FINTECH CHUYÊN NGHIỆP

> [!CAUTION]
> **YÊU CẦU QUAN TRỌNG TỪ TRƯỞNG NHÓM:**  
> **TUYỆT ĐỐI BÃI BỎ TOÀN BỘ CÁC BIỂU TƯỢNG EMOJI/EMOTE UNICODE** (như 👛, 💸, 📊, 📑, 🎙️, 💎, 🤖, 🎯, 🧠, ⚙️, 🔔, ⏰, ☕, 🧋...) trên toàn bộ giao diện ứng dụng.  
> Các emote này là giải pháp tạm thời do AI gen sơ khởi, trông thiếu chuyên nghiệp, bị lệch nét trên các hệ điều hành khác nhau và làm giảm giá trị thẩm mỹ của đồ án tốt nghiệp. Phải thay thế 100% bằng **Bộ Icon Vector cao cấp chuẩn ngành**.

### 3.1. Vì Sao Phải Loại Bỏ Emoji & Dùng Icon Vector?
1. **Không đồng nhất trên các nền tảng (Platform Fragmentation):** Emoji trên Android (Google), Samsung OneUI, iOS và Windows có nét vẽ và màu sắc hoàn toàn khác nhau, không kiểm soát được.
2. **Không thể đổi màu động (No Dynamic Tinting):** Emoji có màu cố định của phông chữ hệ thống. Không thể chuyển sang màu xanh Sky Blue `#38BDF8` khi Tab đang được chọn (Active) hoặc màu xám Slate `#64748B` khi không chọn (Inactive).
3. **Mất chất "Dark Luxury Fintech":** Ứng dụng quản lý tài chính cao cấp (như Revolut, Wise, Monzo, Apple Card) luôn dùng icon vector đơn sắc, nét mỏng (1.5px - 2px) tinh tế, không bao giờ dùng emoji lòe loẹt.

---

### 3.2. Top 3 Bộ Icon Vector Khuyên Dùng Cho Dự Án

| Bộ Icon | Nguồn Tham Khảo | Phong Cách & Ưu Điểm | Khuyến Nghị Cho Dự Án |
| :--- | :--- | :--- | :--- |
| **1. Lucide Icons** ⭐ *(Khuyên dùng số 1)* | [lucide.dev](https://lucide.dev) | Nét vẽ mảnh 2.0px, tối giản, hiện đại, chuẩn Vercel / Apple. Cực kỳ phong phú về icon tài chính và công nghệ. | **ƯU TIÊN SỐ 1** cho toàn bộ Bottom Nav, Sidebar và Action Buttons. |
| **2. Google Material Symbols** | [fonts.google.com/icons](https://fonts.google.com/icons) | Phong cách Rounded hoặc Outlined. Tích hợp sâu vào Android, siêu nhẹ, chuẩn mực Material Design 3. | Rất tốt cho các nút hệ thống, điều hướng, bảo mật. |
| **3. Tabler Icons** | [tabler.io/icons](https://tabler.io/icons) | Hơn 5,000+ vector icons độ chi tiết cao, nét vẽ đồng đều tuyệt đối ở kích thước 24x24. | Lựa chọn dự phòng tuyệt vời. |

---

### 3.3. Bảng Ánh Xạ Chuyển Đổi Icon Cụ Thể (Icon Migration Mapping)

Bạn hãy đối chiếu bảng sau để thay thế chính xác từng vị trí trong XAML:

| Vị Trí / Chức Năng | Icon Emoji Tạm Thời (CẦN BỎ) | Tên Icon Vector Khuyên Dùng (Lucide / Material) | Mô Tả Trực Quan |
| :--- | :---: | :--- | :--- |
| **Tab 1: Quản lý Đa Ví** | `👛` | `wallet` / `account-balance-wallet` | Chiếc ví da gấp thanh lịch |
| **Tab 2: Sổ Chi Tiêu** | `💸` | `receipt` / `credit-card` / `arrow-down-left` | Hóa đơn chi tiết hoặc mũi tên chi ra |
| **Tab 3: Ngân Sách DSA** | `📊` | `pie-chart` / `scale` / `target` | Biểu đồ tròn ngân sách cân đối |
| **Tab 4: Báo Cáo Tài Chính**| `📑` | `bar-chart-3` / `trending-up` / `file-text` | Cột đồ thị xu hướng tăng trưởng |
| **Center FAB: Micro Voice AI**| `🎙️` | `mic` / `sparkles` | Micro podcast thanh mảnh kim loại |
| **Sidebar: Tài Chính Cốt Lõi**| `💎` | `wallet-cards` / `coins` / `layers` | Các lớp tài sản xếp tầng |
| **Sidebar: AI Vision & Scan** | `🤖` | `scan` / `scan-line` / `cpu` | Khung quét laser công nghệ cao |
| **Sidebar: Kỷ Luật & Mục Tiêu**| `🎯` | `target` / `compass` / `piggy-bank` | Bia mục tiêu chính xác |
| **Sidebar: Tâm Lý & Thaler** | `🧠` | `sparkles` / `lightbulb` / `activity` | Ánh sáng cú hích nhận thức |
| **Sidebar: Hệ Thống & Bảo Mật**| `⚙️` | `settings` / `shield-check` | Bánh răng hoặc khiên bảo mật |
| **Nút Thông Báo** | `🔔` | `bell` / `notifications` | Quả chuông viền mỏng |
| **Nút Báo Thức 21:00** | `⏰` | `alarm-clock` / `clock` | Đồng hồ kim tối giản |
| **Danh Mục Ăn Uống** | `☕` / `🧋` | `utensils` / `coffee` | Dao dĩa hoặc tách cà phê |

---

### 3.4. Hướng Dẫn Kỹ Thuật Tích Hợp Icon Vào .NET MAUI

Có 2 phương án tích hợp vector trong .NET MAUI. **Phương án 1 (Font Icon)** là giải pháp tối ưu và chuyên nghiệp nhất:

#### 🌟 Phương Án 1: Sử Dụng Font Icons (TrueType Font - Khuyên Dùng Nhất)
* **Cách thực hiện:**
  1. Tải file font `lucide.ttf` (từ Lucide release) hoặc `MaterialSymbolsOutlined.ttf`.
  2. Bỏ file `.ttf` vào thư mục: `frontend/ExpenseTracker.Maui/Resources/Fonts/`.
  3. Mở [`MauiProgram.cs`](file:///d:/University%20Study%20Subjects/Information%20System%20Design%20Analysis/Project/Analyze%20System/mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/MauiProgram.cs), thêm vào cấu hình `ConfigureFonts`:
     ```csharp
     fonts.AddFont("lucide.ttf", "LucideIcons");
     ```
  4. Tạo một lớp hằng số Glyph tĩnh [`IconFont.cs`](file:///d:/University%20Study%20Subjects/Information%20System%20Design%20Analysis/Project/Analyze%20System/mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/Helpers/IconFont.cs):
     ```csharp
     namespace ExpenseTracker.Maui.Helpers;
     public static class IconFont
     {
         public const string Wallet = "\ue901";
         public const string Receipt = "\ue902";
         public const string PieChart = "\ue903";
         public const string BarChart = "\ue904";
         public const string Mic = "\ue905";
         public const string Settings = "\ue906";
     }
     ```
  5. Trong XAML, sử dụng thẻ `FontImageSource` vô cùng tiện lợi:
     ```xml
     <Image WidthRequest="22" HeightRequest="22">
         <Image.Source>
             <FontImageSource 
                 FontFamily="LucideIcons" 
                 Glyph="{x:Static helpers:IconFont.Wallet}" 
                 Color="{StaticResource AccentActive}" 
                 Size="22" />
         </Image.Source>
     </Image>
     ```
  * **Ưu điểm vượt trội:**
    - Thay đổi màu sắc (`Color`) lập tức theo trạng thái Active/Inactive mà không cần tải nhiều file ảnh.
    - Sắc nét tuyệt đối trên mọi mật độ điểm ảnh (Retina / Ultra HD).
    - File font cực nhẹ (< 100KB cho hàng ngàn icon).

#### 🖼️ Phương Án 2: Sử Dụng File Vector SVG Trực Tiếp
* **Cách thực hiện:**
  1. Tải các file SVG đơn lẻ từ Lucide (`wallet.svg`, `receipt.svg`, `mic.svg`...).
  2. Đặt vào thư mục `frontend/ExpenseTracker.Maui/Resources/Images/`. .NET MAUI sẽ tự động biên dịch SVG thành vector native drawable.
  3. Trong XAML hiển thị trực tiếp:
     ```xml
     <Image Source="ic_wallet.svg" WidthRequest="22" HeightRequest="22" />
     ```

---

## 4. CẤU TRÚC THƯ MỤC FRONTEND ĐỂ BẠN LÀM VIỆC

```text
mobile-project/dotnet-maui-ver/frontend/ExpenseTracker.Maui/
├── Resources/
│   ├── Fonts/                             <-- NƠI ĐẶT FILE FONT VECTOR (lucide.ttf)
│   └── Images/                            <-- NƠI ĐẶT CÁC FILE VECTOR .SVG
├── Views/
│   ├── Components/
│   │   └── BottomNavDockView.xaml         <-- THANH ĐIỀU HƯỚNG ĐÁY CHÍNH (Đã có nút Voice FAB)
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

## 5. HƯỚNG DẪN CÁCH NHÚNG BOTTOM NAV DOCK VÀO CÁC TRANG MỚI

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

## 6. CHECKLIST CÔNG VIỆC GỢI Ý CHO BẠN TIẾP TỤC HOÀN THIỆN NGÀY MAI

- [ ] **🔥 ƯU TIÊN SỐ 1: Bãi bỏ toàn bộ emoji và thay bằng Icon Vector xịn:**
  - Tải bộ font `lucide.ttf` hoặc bộ file SVG của Lucide Icons / Material Symbols.
  - Thay thế 4 icon trên thanh `BottomNavDockView.xaml` và icon nút Mic trung tâm.
  - Thay thế các icon nhóm trong Hamburger Sidebar `AppShell.xaml`.
  - Thay thế các emoji trên các nút bấm Quick Action trong `WalletsPage.xaml`, `ExpensesPage.xaml` và `VoiceMicModalPage.xaml`.
- [ ] **2. Rà soát căn chỉnh lề (Padding/Margin) trên các trang phụ:**
  - Kiểm tra `IncomesPage.xaml`, `RecurringIncomesPage.xaml`, `SavingsGoalsPage.xaml` xem có thẻ nào bị sát viền hay không.
  - Đảm bảo tất cả các trang đều có `Padding="16,12,16,24"`.
- [ ] **3. Thẩm định kích thước font chữ trên màn hình nhỏ:**
  - Mở app trên máy ảo, lướt qua một lượt các trang để đảm bảo không có chữ nào bị rớt dòng đơn độc (orphan word).
  - Kiểm tra các nhãn số tiền lớn đều nằm gọn trong thẻ.
- [ ] **4. Trau chuốt thẻ Profile Người Dùng (Nếu muốn thêm):**
  - Hiện tại trang `LoginPage.xaml` đang dùng để quản lý đăng nhập/đăng xuất.
  - Bạn có thể thiết kế thêm một trang `ProfilePage.xaml` đơn giản hiển thị: Tên người dùng, Email đã xác thực, Avatar tròn và Nút đăng xuất.
- [ ] **5. Kiểm tra độ tương phản màu sắc (Accessibility):**
  - Chữ xám mờ trên nền tối nên dùng tối thiểu `#94A3B8` hoặc `#A5B4FC` để dễ đọc trong điều kiện ánh sáng mạnh.
  - Không dùng chữ xám quá tối (`#475569` trở xuống) cho các thông tin quan trọng.

---

## 7. LỆNH CHẠY & KIỂM TRA MÃ NGUỒN TRONG QUÁ TRÌNH LÀM VIỆC

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
