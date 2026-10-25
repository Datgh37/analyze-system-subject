# 🚀 KẾ HOẠCH CHIẾN LƯỢC: CHUYỂN ĐỔI & HIỆN ĐẠI HÓA CÔNG NGHỆ MOBILE APP
## (TECH STACK EVOLUTION & MIGRATION STRATEGY)

> **Mã kế hoạch:** `PLAN-22`  
> **Ngày khởi tạo:** 07/10/2026  
> **Trạng thái:** `ACTIVE - DRAFTING & READY FOR ACTIVATION`  
> **Người đề xuất:** Trịnh Thành Đạt (Lead & System Architect)  
> **Tham chiếu nghiệp vụ:** Đồ án PTTKHTTT, Khóa luận tốt nghiệp (`KhoaLuanTotNghiep.docx`), [`mobile-project/README.md`](../../mobile-project/README.md), [`mobile-project/ROADMAP.md`](../../mobile-project/ROADMAP.md).

---

## 1. Bối Cảnh & Động Lực Thay Đổi (Context & Drivers of Change)

1. **Thay đổi nhân sự phụ trách môn học:**  
   - Giảng viên phụ trách ban đầu đã rời đi, Giảng viên mới chính thức tiếp quản môn học *Lập trình Thiết bị Di động*.
2. **Dỡ bỏ toàn diện các rào cản công nghệ cũ:**  
   - **Trước đây (Giới hạn cũ):** Bị ràng buộc phải viết giao diện React Native đơn giản, cấm dùng thư viện CSS/UI bên thứ 3, chỉ dùng `useState`/`useEffect` thô sơ để đối phó các câu hỏi vấn đáp cơ bản; Backend bị ép buộc dùng PHP thuần và hosting WAMP/FTP trường cấp (`103.139.203.43 / nhom46.itimit.id.vn`).
   - **Hiện tại (Không gian tự do kỹ thuật mới):** Giảng viên mới không còn áp đặt các giới hạn công nghệ cũ. Nhóm được toàn quyền lựa chọn công nghệ tiên tiến, chuẩn công nghiệp để xây dựng một ứng dụng di động hoàn chỉnh, chuyên nghiệp và có chiều sâu thiết kế hệ thống.
3. **Định hướng chiến lược:**  
   - Không còn phải bóp méo hay hạ chuẩn kiến trúc hệ thống để thi vấn đáp nhập môn.
   - Tập trung 100% vào việc hiện thực hóa một ứng dụng di động hoàn chỉnh, đồng bộ với kiến trúc hướng miền (DDD) và tài liệu phân tích thiết kế của Khóa luận tốt nghiệp.
   - Đóng băng việc triển khai lên server hosting cũ của thầy trước.

---

## 2. Chiến Lược Hai Giai Đoạn (Two-Phase Transitional Strategy)

```text
┌────────────────────────────────────────────────────────────────────────────────────────┐
│                        CHIẾN LƯỢC CHUYỂN ĐỔI CÔNG NGHỆ 2 GIAI ĐOẠN                     │
├───────────────────────────────────────────┬────────────────────────────────────────────┤
│   GIAI ĐOẠN 1: BẢO TOÀN & KIỂM THỬ TẠM    │    GIAI ĐOẠN 2: TRÙNG TU & CHUYỂN ĐỔI      │
│          (TRANSITIONAL / CURRENT)         │            (TARGET MODERN STACK)           │
├───────────────────────────────────────────┼────────────────────────────────────────────┤
│ - Giữ nguyên stack tạm:                   │ - Kích hoạt khi giảng viên mới chốt định   │
│   + FE: React Native Expo SDK 54 (White)  │   hướng công nghệ chính thức.              │
│   + BE: PHP PDO (21 RESTful API + JWT)    │ - Lựa chọn 1 trong 2 Phương án hiện đại:   │
│   + DB: MySQL Localhost WAMP Server       │   [Phương án A]: .NET MAUI + .NET Web API  │
│ - Mục tiêu: Kiểm thử trọn vẹn nghiệp vụ   │   [Phương án B]: React Native + .NET / Node│
│   tài chính, logic tính toán, OTP, Delta  │ - Mục tiêu: Ứng dụng chuẩn Production, UI  │
│   và ma trận test trước khi chuyển đổi.   │   cao cấp, kiến trúc Clean Architecture.   │
└───────────────────────────────────────────┴────────────────────────────────────────────┘
```

---

## 3. Đánh Giá & So Sánh Các Phương Án Công Nghệ Tiềm Năng (Architectural Options)

### Phương Án A: Hệ Sinh Thái Thuần Microsoft (.NET MAUI + ASP.NET Core Web API)
* **Frontend:** **.NET MAUI** (C# 14 / .NET 10, XAML, MVVM CommunityToolkit - chính thức triển khai tại PLAN-26/PLAN-31).
* **Backend:** **ASP.NET Core Web API (.NET 10 / C# 14)**, Clean Architecture, Entity Framework Core (EF Core) PostgreSQL 16.
* **CSDL:** Microsoft SQL Server (hoặc PostgreSQL / MySQL).
* **Ưu điểm:**
  - Ngôn ngữ nhất quán 100% C# từ Client đến Server, tận dụng tối đa kiến trúc OOP, SOLID, Type-Safety tuyệt đối.
  - Phù hợp hoàn hảo với kiến trúc Domain-Driven Design (DDD) trong Chương 3 Khóa luận tốt nghiệp (`KhoaLuanTotNghiep.docx`).
  - Hỗ trợ biên dịch Native AOT, hiệu năng cực cao trên Android và iOS.
* **Thách thức:**
  - Bạn Đại (phụ trách UI) cần làm quen với cú pháp XAML và mô hình MVVM nếu trước đó chỉ quen làm JavaScript/React.

### Phương Án B: Lai Hiện Đại (Modern React Native Expo + ASP.NET Core Web API hoặc Node.js / NestJS)
* **Frontend:** **React Native (Expo SDK 54, TypeScript / JavaScript)**:
  - Tự do sử dụng các bộ UI Kit chuyên nghiệp: React Native Reanimated v3, Lucide Icons, FlashList, UI Kitten / Paper.
  - Quản lý trạng thái bằng Zustand hoặc Redux Toolkit.
* **Backend:** 
  - *Lựa chọn B1:* **ASP.NET Core Web API (.NET 8/9)** (Cực mạnh về tài chính, bảo mật, Clean Architecture).
  - *Lựa chọn B2:* **Node.js (NestJS / Express)** (Cùng hệ sinh thái TypeScript/JavaScript với React Native).
* **CSDL:** PostgreSQL hoặc MySQL.
* **Ưu điểm:**
  - Kế thừa lại 100% bộ khung Frontend đã xây dựng sẵn ở Giai đoạn C (`src/api`, `AuthContext`, 9 screens), không phải làm lại giao diện từ số không.
  - Bạn Đại tiếp tục phát huy thế mạnh React Native mà không gặp rào cản chuyển đổi ngôn ngữ.
  - Dễ dàng nâng cấp giao diện lung linh, hoạt ảnh mượt mà với Reanimated mà không sợ bị thầy cấm.

---

## 4. Kế Hoạch Bảo Toàn Nghiệp Vụ Trong Quá Trình Chuyển Đổi (Asset Preservation)

Dù chuyển đổi sang bất kỳ công nghệ nào, toàn bộ tài sản kỹ thuật đã nghiên cứu và hoàn thiện sẽ được **kế thừa 100%**:

1. **Hợp đồng API (API Contracts):** 
   - Bản đặc tả 21 RESTful API endpoints (Xác thực JWT, Ví ACID, Danh mục, Giao dịch bù trừ Delta, Ngân sách cảnh báo ngày, Báo cáo biểu đồ) sẽ giữ nguyên chuẩn JSON request/response.
   - Khi viết lại Backend bằng .NET hay Node.js, chỉ cần giữ nguyên format JSON là Frontend có thể cắm vào chạy ngay lập tức.
2. **Cơ sở Dữ liệu & Quy tắc Nghiệp vụ:**
   - 6 bảng quan hệ (`users`, `wallets`, `categories`, `transactions`, `budgets`, `password_resets`) cùng các ràng buộc toàn vẹn khóa ngoại sẽ được chuyển dịch sang Code-First Migration (EF Core hoặc TypeORM/Prisma).
   - Các thuật toán bù trừ Delta số dư ví, chuyển tiền ACID và công thức ngân sách an toàn mỗi ngày (`Daily Safe Spend = (Hạn mức - Đã tiêu) / Số ngày còn lại`) sẽ được bê nguyên vẹn sang Services mới.
3. **Bộ dữ liệu kiểm thử (Test Cases):**
   - Bộ kịch bản 21 bài kiểm thử trong `test_api.ps1` sẽ được tái sử dụng để kiểm thử nghiệm thu tự động cho Backend mới.

---

## 5. Lộ Trình Hành Động Từng Bước (Execution Steps)

* **Bước 1 (Ngay bây giờ):**
  - Cập nhật thông tin định hướng vào [`mobile-project/README.md`](../../mobile-project/README.md) và [`mobile-project/ROADMAP.md`](../../mobile-project/ROADMAP.md).
  - Giữ nguyên bộ mã nguồn PHP WAMP + React Native Expo SDK 54 để tiếp tục chạy test, phục vụ nghiên cứu và trao đổi nhóm.
* **Bước 2 (Khi có chỉ đạo từ Giảng viên mới):**
  - Trình bày đề tài và xin ý kiến giảng viên về công nghệ ưu tiên (.NET MAUI vs React Native + .NET/Node).
  - Chốt phương án kỹ thuật chính thức.
* **Bước 3 (Triển khai Trùng tu - Modernization Sprint):**
  - Khởi tạo dự án mới theo phương án được duyệt.
  - Porting CSDL & 21 API sang stack mới.
  - Nâng cấp giao diện Mobile đẹp mắt, mượt mà và đóng gói báo cáo hoàn chỉnh.
