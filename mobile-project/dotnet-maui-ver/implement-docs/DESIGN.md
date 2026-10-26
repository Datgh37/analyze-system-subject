# DESIGN TRUTH & VISUAL SYSTEM — .NET MAUI ENTERPRISE FINTECH

> **Mục đích tài liệu:** Bản ghi nguồn sự thật thiết kế (Durable Design System & Tokens) chuẩn [Impeccable](https://github.com/pbakaus/impeccable).  
> Chi phối toàn bộ XAML Styles, Resources Dictionary, Custom Controls và quy chuẩn thẩm mỹ của ứng dụng di động .NET MAUI.

---

## 1. PHÒNG THỦ LỖI RẬP KHUÔN (ANTI-PATTERN DEFENSE LAWS)

Tuân thủ nghiêm ngặt các quy tắc kiểm định của Impeccable:
1. **LUẬT 1: CẤM "CARD SOUP" (KHÔNG LỒNG THẺ TRONG THẺ):**
   - Tuyệt đối không bao bọc thẻ con trong thẻ mẹ nhiều tầng (`Frame inside Frame` hoặc `Border inside Border`).
   - Phân định các khối thông tin bằng **khoảng trắng tự nhiên (Whitespace Rhythm)**, đường kẻ phân cách mỏng `0.5pt` (`#334155` / `#E2E8F0`) hoặc màu nền bề mặt khác biệt nhẹ (`Surface Level 1` vs `Surface Level 2`).
2. **LUẬT 2: KỸ THUẬT HÒA SẮC BẮT BUỘC (COLOR TINTING - NO PURE GRAYS):**
   - Cấm sử dụng màu đen thuần `#000000` hoặc xám vô hồn `#808080`.
   - Mọi màu nền tối, màu chữ phụ và đường viền đều được pha sắc thái **Deep Indigo / Slate** để tạo độ sâu thị giác sang trọng chuẩn ứng dụng tài chính cao cấp.
3. **LUẬT 3: GIA CỐ SỐ TIỀN & CHỐNG TRÀN VĂN BẢN (HARDEN & AUTO-SCALING):**
   - Mọi nhãn số tiền lớn (ví dụ: `150,000,000 đ`) phải được thiết kế với cơ chế co giãn kích thước chữ hoặc dùng `font-variant-numeric: tabular-nums` để các con số luôn thẳng cột và không bị vỡ layout khi số tiền lên hàng tỷ đồng.
4. **LUẬT 4: CHUYỂN ĐỘNG CÓ CHỦ ĐÍCH (NO DATED BOUNCE EASING):**
   - Cấm dùng hiệu ứng nảy lỗi thời (Bounce / Elastic).
   - Áp dụng mô hình gia tốc lò xo (Spring Physics) hoặc `CubicInOut` mượt mà cho các thanh tiến độ ngân sách và biểu đồ tài chính.
5. **LUẬT 5: KHÔNG RUNG BÀN PHÍM (ZERO UNNECESSARY HAPTICS):**
   - Thao tác bấm số tiền trên bàn phím ảo không kích hoạt rung phản hồi phần cứng, giữ cho ứng dụng nhẹ nhàng, êm ái và tiết kiệm pin.

---

## 2. BẢNG MÃ MÀU CHUẨN (FINTECH COLOR TOKENS)

### 2.1. Màu sắc Chủ đạo & Bề mặt (Core & Surfaces)
```xml
<!-- Brand Identity Colors -->
<Color x:Key="ColorPrimary">#3B82F6</Color>          <!-- Royal Blue: Nút hành động chính, điểm nhấn -->
<Color x:Key="ColorPrimaryDark">#1D4ED8</Color>      <!-- Deep Blue: Trạng thái nhấn (Pressed) -->
<Color x:Key="ColorPrimaryLight">#DBEAFE</Color>     <!-- Soft Blue Tint: Nền thẻ thông tin nổi bật -->

<!-- Dark Mode Surfaces (Deep Indigo Tinted - No pure black) -->
<Color x:Key="SurfaceBackground">#0F172A</Color>     <!-- Slate 900: Nền ứng dụng chính -->
<Color x:Key="SurfaceLevel1">#1E293B</Color>         <!-- Slate 800: Nền thẻ, thanh điều hướng -->
<Color x:Key="SurfaceLevel2">#334155</Color>         <!-- Slate 700: Nền nút phụ, ô nhập liệu -->
<Color x:Key="SurfaceBorder">#334155</Color>         <!-- Slate 700: Viền mỏng 0.5pt phân cách -->
```

### 2.2. Màu sắc Trạng thái Tài chính (Financial Semantic Colors)
```xml
<!-- Income (Thu nhập / Tăng trưởng) -->
<Color x:Key="ColorIncome">#10B981</Color>           <!-- Emerald 500: Số tiền cộng, dòng tiền dương -->
<Color x:Key="ColorIncomeSurface">#064E3B</Color>    <!-- Dark Emerald Tint: Nền nhẹ thẻ thu nhập -->

<!-- Expense (Chi tiêu / Biến động âm) -->
<Color x:Key="ColorExpense">#EF4444</Color>          <!-- Coral Red 500: Số tiền trừ, chi phí -->
<Color x:Key="ColorExpenseSurface">#7F1D1D</Color>   <!-- Dark Red Tint: Nền nhẹ thẻ chi tiêu -->

<!-- DSA & Budget Alerts (Hạn mức ngân sách an toàn) -->
<Color x:Key="ColorBudgetSafe">#10B981</Color>       <!-- Xanh: Dưới 70% hạn mức tháng (An toàn) -->
<Color x:Key="ColorBudgetWarning">#F59E0B</Color>    <!-- Vàng: 70% - 90% hạn mức (Cảnh báo nhẹ) -->
<Color x:Key="ColorBudgetDanger">#EF4444</Color>     <!-- Đỏ: Trên 90% hoặc bội chi (Nguy hiểm) -->
```

---

## 3. THANG KÍCH THƯỚC & KHOẢNG TRẮNG (8-POINT SPACING GRID)

Mọi padding, margin và khoảng cách giữa các thành phần đều tuân theo bội số của **8pt** (hoặc nửa bước 4pt cho vi tinh chỉnh):

| Token | Giá trị | Ứng dụng thực tế |
| :--- | :---: | :--- |
| `Space2XS` | **4 pt** | Khoảng cách giữa icon và nhãn chữ đi kèm |
| `SpaceXS` | **8 pt** | Đệm trong ô nhập liệu nhỏ, khoảng cách item danh sách |
| `SpaceSM` | **12 pt**| Khoảng cách giữa các nhãn tài chính liên quan |
| `SpaceMD` | **16 pt**| Padding chuẩn của các thẻ nội dung (Card Container) |
| `SpaceLG` | **24 pt**| Khoảng cách giữa các khối chức năng chính trên màn hình |
| `SpaceXL` | **32 pt**| Khoảng cách từ đỉnh trang tới tiêu đề tổng tài sản |

---

## 4. HỆ THỐNG PHÔNG CHỮ & THANG TYPOGRAPHY (TYPOGRAPHIC SCALE)

Phông chữ hiển thị trên .NET MAUI: **Plus Jakarta Sans** (hoặc hệ thống fallback: `Segoe UI` trên Windows, `Roboto` trên Android).

| Cấp bậc | Cỡ chữ (pt) | Độ dày (Weight) | Màu sắc áp dụng | Mục đích |
| :--- | :---: | :---: | :--- | :--- |
| `DisplayLarge` | **32 pt** | **Bold (700)** | `#F8FAFC` (Slate 50) | Tổng tài sản hiển thị trên đầu Dashboard |
| `HeadingMedium`| **20 pt** | **SemiBold (600)**| `#F1F5F9` (Slate 100)| Tiêu đề trang, Tên ví tiền lớn |
| `BodyLarge` | **16 pt** | **Medium (500)** | `#E2E8F0` (Slate 200)| Tên giao dịch, Hạng mục thu chi |
| `BodyMedium` | **14 pt** | **Regular (400)**| `#94A3B8` (Slate 400)| Ghi chú giao dịch, Tỷ lệ phần trăm |
| `CaptionSmall`| **12 pt** | **Medium (500)** | `#64748B` (Slate 500)| Thời gian phát sinh giao dịch, Badge phụ |

---

## 5. THIẾT KẾ BO GÓC & BỀ MẶT KÉP (DOUBLE-BEZEL CONCENTRIC RADII)

Quy tắc bán kính đồng tâm cho các bề mặt lồng nhau:
$$\text{Bán kính ngoài} = \text{Bán kính trong} + \text{Padding đệm}$$
- Thẻ lớn ngoài: `CornerRadius = 20`
- Khối nội dung con bên trong thẻ (nếu có): `CornerRadius = 12` (với Padding 8)
- Nút bấm hành động (Action Button): `CornerRadius = 12`
- Nhãn thông tin nhỏ (Pill Badge): `CornerRadius = 999` (Viên thuốc tròn đều)
