# 🚀 KẾ HOẠCH HỘI ĐỒNG: KHỞI ĐỘNG PHÂN HỆ .NET MAUI ENTERPRISE CLIENT
## (COUNCIL KICKOFF: .NET MAUI ENTERPRISE ARCHITECTURE & MOBILE TECH STACK)

> **Mã kế hoạch:** `PLAN-29`  
> **Thời điểm:** 21/10/2026  
> **Trạng thái:** `APPROVED & IN-SESSION`  
> **Chủ trì phiên họp:** Agent 00 (Council Orchestrator)  
> **Thành viên tham gia:** 
> - Agent 07: Werner Vogels (DevOps & Environment Setup)
> - Agent 10: Dan Abramov (Client Architecture & MVVM Reactive UI)
> - Agent 04: Eric Evans (Domain-Driven Design & Clean Architecture)
> - Agent 05: Martin Kleppmann (Realtime Synchronization & Local SQLite Cache)
> - Agent 11: Bruce Schneier (Fintech Security & Mobile KeyStore)
> - Agent 08: Andrew Ng (Camera Vision OCR Pipeline)
> **Tài liệu căn cứ:** [`mobile-project/dotnet-maui-ver/README.md`](../../mobile-project/dotnet-maui-ver/README.md), [`PLAN-22`](2026-10-07_plan_22_tech_stack_evolution_and_migration_strategy.md).

---

## 1. MỤC TIÊU PHIÊN HỌP
1. **Giải quyết triệt để vấn đề Cài đặt & Môi trường .NET MAUI** trên máy trạm của sinh viên: Khảo sát hiện trạng SDK, hướng dẫn cài đặt bổ sung workload và chiến lược chạy đa nền tảng (Windows Native vs Android Emulator).
2. **Xác lập Bộ Công nghệ Bắt buộc cho Ứng dụng Di động (Mobile Tech Stack)** theo đúng yêu cầu nghiêm ngặt của Giảng viên:
   - **Realtime Synchronization (Cập nhật dữ liệu động):** Cơ chế 0ms qua SignalR Client + Two-way Data Binding.
   - **Offline-First & Local Caching:** Bộ đệm CSDL cục bộ SQLite-net / EF Core SQLite + Chiến lược hòa giải xung đột (Conflict Resolution).
   - **Tương tác Phần cứng Di động:** Chụp ảnh hóa đơn qua Camera, Quét sinh trắc học (Vân tay/Face ID), Mã hóa phần cứng (Android KeyStore / iOS Keychain).
   - **Kiến trúc MVVM Phản ứng (Reactive MVVM):** CommunityToolkit.Mvvm với Roslyn Source Generators.
   - **Khả năng Phục hồi Mạng (Network Resilience):** Refit Type-Safe Client + Polly v8.
3. **Phê duyệt Cấu trúc Solution 3 Tầng** (`ExpenseApp.Core`, `ExpenseApp.Infrastructure`, `ExpenseApp.Maui`) chuẩn Clean Architecture.

---

## 2. BÁO CÁO KHẢO SÁT HIỆN TRẠNG MÔI TRƯỜNG MÁY TÍNH (AGENT 07 - WERNER VOGELS)

### 2.1. Kết quả kiểm tra hệ thống thực tế
- **Hệ điều hành:** Windows 11 Pro 64-bit (`win-x64`).
- **.NET SDK:** Đã cài đặt sẵn **.NET SDK 10.0.402** (LTS) và .NET SDK 8.0.413.
- **Workload MAUI hiện có:** Đã có sẵn **`[maui-windows]`** (Tích hợp từ Visual Studio).
- **Template dự án:** Các template `maui`, `mauilib`, `maui-multiproject` đã sẵn sàng trong máy.

### 2.2. Hướng dẫn thiết lập bổ sung (Environment Setup Roadmap)
1. **Chế độ phát triển siêu tốc (Windows Desktop Native - Không cần cài thêm gì):**
   - Với workload `maui-windows` đã có sẵn, dự án có thể build và chạy native trực tiếp dưới dạng Windows 11 App (`net10.0-windows10.0.19041.0`).
   - *Ưu điểm:* Khởi động trong 1 giây, tốn cực ít RAM, debug logic MVVM, kết nối API, test Realtime SignalR cực mượt mà không cần máy ảo Android nặng nề.
2. **Chế độ phát triển Android (Dành cho nộp bài & chạy trên điện thoại/máy ảo):**
   - **Cách 1 (Khuyên dùng - Qua Visual Studio Installer):**
     - Mở *Visual Studio Installer* $\rightarrow$ Chọn *Modify* phiên bản VS hiện tại.
     - Tích chọn Workload: **".NET Multi-platform App UI development"** (sẽ tự động cấu hình Android SDK, Java OpenJDK, Android Device Manager và AVD emulator chuẩn chỉ).
   - **Cách 2 (Cài nhanh qua dòng lệnh PowerShell Administrator):**
     ```powershell
     dotnet workload install maui-android
     ```

---

## 3. MA TRẬN CÔNG NGHỆ MOBILE BẮT BUỘC (MANDATORY MOBILE TECH STACK)

| Nhóm Công Nghệ | Công Nghệ / Thư Viện Lựa Chọn | Vai Trò & Cơ Chế Hoạt Động Cụ Thể | Thành Viên Phụ Trách |
| :--- | :--- | :--- | :--- |
| **1. Cập nhật Dữ liệu Động (Realtime Sync)** | **Microsoft.AspNetCore.SignalR.Client** + **ObservableCollection** | Kết nối WebSocket Full-Duplex với Backend. Khi Server có biến động giao dịch/ngân sách $\rightarrow$ Bắn event qua Hub $\rightarrow$ ViewModel cập nhật `ObservableCollection` $\rightarrow$ XAML UI tự động vẽ lại tức thì 0ms mà người dùng không cần reload trang. | **Agent 05 & Agent 10** |
| **2. Ngoại tuyến & Bộ nhớ đệm (Offline-First)** | **SQLite-net-pcl** / **EF Core SQLite** + **Vector Clock** | Dữ liệu được lưu trữ hai tầng (Local Cache & Cloud DB). Mất mạng người dùng vẫn ghi chép thu chi bình thường. Khi có mạng trở lại, background sync tự động đẩy giao dịch lên server. | **Agent 05 (DB Lead)** |
| **3. Mô hình Kiến trúc Giao diện** | **MVVM (Model-View-ViewModel)** + **CommunityToolkit.Mvvm** | Sử dụng Roslyn Source Generators: `[ObservableProperty]`, `[RelayCommand]`, `WeakReferenceMessenger`. Ràng buộc dữ liệu hai chiều (Two-way Data Binding) sạch sẽ, không code-behind. | **Agent 10 (Client Lead)** |
| **4. Giao tiếp Mạng & Phục hồi** | **Refit** + **Polly v8** | Khai báo API dạng Type-Safe C# Interface (không viết raw `HttpClient`). Tích hợp Circuit Breaker, Exponential Retry khi mạng 4G/WiFi chập chờn. | **Agent 07 (DevOps Lead)** |
| **5. Bảo mật Cấp Ngân Hàng** | **MAUI SecureStorage** + **Plugin.Maui.Biometric** | Lưu Token JWT và Khóa mã hóa vào phần cứng an toàn (**Android KeyStore** / **iOS Keychain**). Mở khóa app bằng Vân tay / Face ID. | **Agent 11 (Security Lead)** |
| **6. Phần cứng & Đa phương tiện** | **MediaPicker** + **Local Notifications & Haptics** | Chụp ảnh hóa đơn thực tế để gửi AI Vision OCR. Rung phản hồi (Haptic) khi chạm nút và thông báo đẩy cảnh báo vượt ngưỡng ngân sách DSA. | **Agent 08 & Agent 13** |
| **7. Trực quan hóa Số liệu** | **SkiaSharp** / **Microcharts for MAUI** | Vẽ biểu đồ tròn cơ cấu chi tiêu, biểu đồ cột thu chi 6 tháng, đồng hồ đo áp lực tài chính mượt mà 60–120 FPS. | **Agent 09 (Art Director)** |

---

## 4. QUYẾT NGHỊ CỦA CHỦ TỊCH HỘI ĐỒNG (AGENT 00 - ORCHESTRATOR)
1. **Chấp thuận 100% phạm vi triển khai:** Bản .NET MAUI là sản phẩm hoàn chỉnh cấp doanh nghiệp cho Khóa luận Tốt nghiệp, hiện thực hóa đầy đủ các công nghệ di động tiên tiến nhất.
2. **Kế hoạch hành động 4 bước tiếp theo:**
   - **Bước 1:** Hướng dẫn sinh viên kiểm tra / cài đặt hoàn tất workload MAUI.
   - **Bước 2:** Khởi tạo Solution chuẩn Clean Architecture: `ExpenseApp.sln` gồm 3 projects (`ExpenseApp.Core`, `ExpenseApp.Infrastructure`, `ExpenseApp.Maui`).
   - **Bước 3:** Cài đặt các NuGet Packages cốt lõi (`CommunityToolkit.Mvvm`, `Refit`, `Polly`, `SignalR.Client`, `sqlite-net-pcl`, `SkiaSharp`).
   - **Bước 4:** Xây dựng Shell Navigation, Theme giao diện Modern Fintech và màn hình Dashboard đầu tiên.
