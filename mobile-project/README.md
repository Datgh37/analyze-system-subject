# 📱 TRUNG TÂM KIẾN TRÚC ỨNG DỤNG DI ĐỘNG (MOBILE ARCHITECTURE HUB)

> **Đề tài:** Hệ thống Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh (Tích hợp AI)  
> **Trường Đại học Công nghệ Đồng Nai (DNTU)**  
> **Tác giả:** Trịnh Thành Đạt (Lead Architect) & Trần Trọng Đại (UI Lead)  

---

## 1. 🧭 Chiến Lược Phân Tách Hai Ngăn Xếp Công Nghệ (Dual-Stack Strategy)

Để giải quyết hài hòa giữa hai mục tiêu:
1. **Hoàn thành xuất sắc đồ án môn học Lập trình Thiết bị Di động** trong giới hạn thời gian (10–15 tuần) và phân công nhóm 2 người.
2. **Xây dựng hệ thống hoàn chỉnh cấp doanh nghiệp (Enterprise Grade)** phục vụ **Khóa luận Tốt nghiệp (KLTN)** và đề tài lớn môn **Phân tích Thiết kế Hệ thống Thông tin (PTTKHTTT)**.

Thư mục `mobile-project/` được cấu trúc thành **2 phân hệ độc lập, chuyên biệt**:

```text
mobile-project/
│
├── 📱 react-native-ver/          # PHÂN HỆ ĐỒ ÁN MÔN HỌC (SCOPED MVP)
│   ├── BE/                       # Backend PHP WAMP (21 RESTful APIs, JWT Bearer)
│   ├── FE/                       # Frontend React Native (Expo SDK 54, React 19)
│   ├── realtime-server/          # Cổng Gateway WebSocket thời gian thực (Socket.IO + Web Simulator)
│   ├── README.md                 # Hướng dẫn chi tiết chạy bản React Native
│   └── BaoCaoLTMB.docx           # Tài liệu báo cáo môn học Lập trình Mobile (Word)
│
└── ⚡ dotnet-maui-ver/           # PHÂN HỆ HỆ THỐNG HOÀN CHỈNH KLTN (FULL SCOPE)
    ├── README.md                 # Hướng dẫn kiến trúc chi tiết bản .NET MAUI
    └── src/                      # Source code .NET 9 MAUI (Clean Architecture + MVVM)
```

---

## 2. 📊 Bảng Đối Chiếu Tính Năng Giữa Hai Phiên Bản

| Phân Hệ / Tiêu Chí | 📱 Phiên Bản React Native (`react-native-ver/`) | ⚡ Phiên Bản .NET MAUI (`dotnet-maui-ver/`) |
| :--- | :--- | :--- |
| **Mục đích chính** | Phục vụ chấm điểm & vấn đáp **môn Lập trình Mobile**. | Phục vụ bảo vệ **Khóa luận Tốt nghiệp & PTTKHTTT**. |
| **Phân công phụ trách** | **Trần Trọng Đại (UI Lead)** & Trịnh Thành Đạt (BE). | **Trịnh Thành Đạt (Lead Architect)** & Nhóm KLTN. |
| **Ngăn xếp công nghệ** | React Native Expo SDK 54, React 19, JavaScript. | .NET 9 MAUI, C# 13, XAML, CommunityToolkit.Mvvm. |
| **Hạ tầng Backend** | WAMP Server (Apache, PHP PDO, MySQL). | ASP.NET Core Web API (.NET 9, EF Core, Clean Architecture). |
| **Giao thức Thời gian thực** | Socket.IO Node.js Gateway (Port 3001). | Microsoft.AspNetCore.SignalR Hub (hoặc WebSocket). |
| **Quản trị Tài khoản & Ví** | 2 Ví cơ bản (Tiền mặt, Tài khoản Ngân hàng). | Đa ví không giới hạn: Thẻ tín dụng, Ví điện tử, Sổ tiết kiệm. |
| **Bóc tách Hóa đơn AI** | Tạm đóng băng (Nhập liệu thủ công nhanh). | **AI Multimodal Vision OCR** + Hậu kiểm toán số học. |
| **Tối ưu Nợ & Dòng tiền** | Tạm đóng băng. | **Debt Snowball vs. Avalanche** + Đồ thị dòng tiền DAG. |
| **Ngân sách & Cú hích** | Ngân sách tháng cơ bản (Tiến độ % tiêu). | **DSA hạn mức an toàn ngày** + Cú hích hành vi Richard Thaler. |
| **Bảo mật & Mã hóa** | JWT Bearer Authentication. | AES-256 SQLCipher, Biometric (Face ID/Vân tay), JWT Rotation. |

---

## 3. 🚀 Lối Tắt Khởi Chạy Nhanh (Quick Navigation)

### 👉 Bạn muốn chạy Đồ án môn học React Native?
Xem hướng dẫn chi tiết tại: [`react-native-ver/README.md`](react-native-ver/README.md)
```bash
# 1. Bật WampServer (wampmanager.exe)
# 2. Tạo liên kết Junction:
cd "mobile-project/react-native-ver/BE"
powershell -ExecutionPolicy Bypass -File setup_wamp_junction.ps1

# 3. Bật Gateway Socket.IO (Port 3001):
cd "mobile-project/react-native-ver/realtime-server"
pnpm start

# 4. Bật Frontend Expo:
cd "mobile-project/react-native-ver/FE"
npm start
```

### 👉 Bạn muốn xem Kiến trúc Hệ thống Hoàn chỉnh .NET MAUI?
Xem tài liệu đặc tả kiến trúc tại: [`dotnet-maui-ver/README.md`](dotnet-maui-ver/README.md)
- Yêu cầu: Visual Studio 2022 v17.12+ (Workload: `.NET Multi-platform App UI development`) và .NET 9 SDK.

---

## 4. 📜 Liên Kết Ngữ Cảnh Dự Án

* 📄 **Chiến lược tiến hóa công nghệ hai giai đoạn:** [`PLAN-22`](../project_context/plans/plans-mobile-app/2026-10-07_plan_22_tech_stack_evolution_and_migration_strategy.md)
* 📜 **Nhật ký quyết định kiến trúc:** [`decisions_and_changes_log.md`](../project_context/decisions_and_changes_log.md) (Quyết định 82, 84 & 85)
