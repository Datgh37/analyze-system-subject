# KẾ HOẠCH TRIỂN KHAI PLAN-35: TÁI CẤU TRÚC CHIẾN LƯỢC TRIỂN KHAI DỰ ÁN (BACKEND-FIRST & WIREFRAME FRONTEND)

- **Ngày ban hành:** 2026-10-22
- **Chủ trì lập kế hoạch:** Council Orchestrator (Agent 00)
- **Căn cứ chỉ đạo:** Ý kiến chỉ đạo trực tiếp của Chủ đầu tư tại phiên họp khẩn ngày 2026-10-22
- **Tình trạng:** `ACCEPTED & LOCKED (Quyết định 104)`

---

## 1. Bối Cảnh & Động Lực Tái Cấu Trúc

### 1.1. Bất cập của phương án cũ (Song song Backend & Frontend)
- Việc vừa thiết kế nghiệp vụ Backend vừa cố gắng hoàn thiện giao diện Frontend cùng lúc làm phân tán sự tập trung của đội ngũ, giảm hiệu suất của cả 2 mảng.
- Kết quả Frontend sinh ra mang tính "nửa vời", dùng controls mặc định chắp vá, thiếu ngôn ngữ thiết kế nhất quán và gây thất vọng về mặt thẩm mỹ.

### 1.2. Định hướng chiến lược mới từ Chủ đầu tư
1. **Tách biệt 2 pha rõ ràng:**
   - **Pha 1 (Backend Dominant & UI Wireframe Only):** Tập trung toàn lực, toàn tâm toàn ý vào Backend (Clean Architecture, Domain Entities, Business Rules, PostgreSQL 3NF, Refit APIs, Unit Tests). Frontend ở pha này **chỉ dựng khung nội dung thô (Wireframe)**, không CSS, không trang trí màu mè, thuần túy để nối dây (wire) và kiểm thử dữ liệu thực tế từ Backend.
   - **Pha 2 (Dedicated Frontend Crafting & Re-skinning):** Dành riêng một giai đoạn để các tác tử chuyên sâu Frontend (`09_brad_frost`, `13_julian_garnier`, `10_dan_abramov`) tập trung 100% thời gian xử lý thẩm mỹ, Art Direction, Design System và Micro-animations.
2. **Quản lý thiết kế tập trung một chỗ (Centralized ResourceDictionary):**
   - Toàn bộ màu sắc, typography, component styles được quản lý tập trung trong `Resources/Styles/` của .NET MAUI.
   - Khi bước vào pha thiết kế, chỉ cần tinh chỉnh bộ token và style tập trung này là toàn bộ các trang tự động đồng bộ.
3. **Môi trường thử nghiệm:**
   - Sử dụng Windows Desktop để kiểm thử wireframe nhanh (0s delay).
   - Khắc phục JDK 21 cho Android Emulator khi chuyển sang pha thiết kế di động chuyên biệt.

---

## 2. Phân Kỳ Chi Tiết Các Giai Đoạn Dự Án Mới

```text
┌───────────────────────────────────────────────────────────────────────────────────┐
│                    LỘ TRÌNH TÁI CẤU TRÚC MỚI (BACKEND-FIRST)                      │
│                                                                                   │
│  [GIAI ĐOẠN 1 -> 6: BACKEND CHUYÊN SÂU & WIREFRAME NỐI DÂY]                      │
│  - Giai đoạn 2: Danh mục (UC-04), Thu nhập (UC-02), Chi tiêu & OCR (UC-03)         │
│  - Giai đoạn 3: Ngân sách đa mô hình 50/30/20, ZBB, Kakeibo (UC-05)                │
│  - Giai đoạn 4: Quản lý Nợ & Đồ thị Snowball/Avalanche (UC-06)                    │
│  - Giai đoạn 5: Quản lý Tiết kiệm & Tích lũy (UC-07)                              │
│  - Giai đoạn 6: Báo cáo, Dòng tiền & Cú hích Richard Thaler Nudge (UC-08)         │
│  * Phía FE: Chỉ tạo ViewModels + Wireframe XAML trắng đen để test chức năng       │
│                                                                                   │
│  [GIAI ĐOẠN HOÀN THIỆN: DEDICATED FRONTEND ART DIRECTION & DESIGN SYSTEM]         │
│  - Xây dựng Centralized Design Tokens (Fintech Palette, Type Scale, Doppelrand)   │
│  - Bọc khung chuẩn Mobile, Animation Anime.js v4, Haptics 60fps GPU               │
│  - Thiết kế lại 100% các màn hình chuẩn Apple Design Award                        │
└───────────────────────────────────────────────────────────────────────────────────┘
```

---

## 3. Quy Chuẩn Kỹ Thuật Cho Frontend Wireframe (Trong Các Giai Đoạn Backend)
- **Giao diện:** Chỉ dùng các thẻ bố cục cơ bản: `VerticalStackLayout`, `Grid`, `Entry`, `Button`, `Label`.
- **Màu sắc & Styling:** Giữ nguyên mặc định hoặc trắng đen tối giản, tuyệt đối không chèn icon, emoji, hoặc hard-code mã màu hex phân tán.
- **Mục tiêu duy nhất:** Kiểm tra đúng 4 trạng thái tương tác:
  1. Dữ liệu rỗng (Empty State).
  2. Dữ liệu đang tải (Loading / ActivityIndicator).
  3. Dữ liệu thành công (Hiển thị đúng trường dữ liệu từ API).
  4. Xử lý lỗi (Hiển thị ErrorMessage từ Backend).

---

## 4. Kế Hoạch Bước Tiếp Theo Ngay Lập Tức
- Khởi động **Giai đoạn 2A: Cây Danh mục phân cấp & Quản lý Thu nhập cốt lõi** (`UC-04` và `UC-02`).
- Backend: Tạo Domain Entities, Migration PostgreSQL, Application Services, Controllers, Unit Tests.
- Frontend: Tạo `CategoriesPage.xaml` và `IncomesPage.xaml` dưới dạng Wireframe thuần túy kết nối Refit API.
