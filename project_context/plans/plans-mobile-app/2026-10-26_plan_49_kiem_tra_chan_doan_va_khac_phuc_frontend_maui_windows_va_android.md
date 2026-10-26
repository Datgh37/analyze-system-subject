# KẾ HOẠCH KIỂM TRA, CHẨN ĐOÁN & KHẮC PHỤC TRIỆT ĐỂ LỖI RUNTIME FRONTEND (.NET MAUI 10)
> **Dự án:** Hệ thống Quản lý Chi tiêu & Hoạch định Tài chính Cá nhân Thông minh (PFM)  
> **Mục tiêu:** Chẩn đoán và tháo gỡ toàn bộ rào cản ngăn ứng dụng .NET MAUI khởi chạy trên nền tảng **Windows App** và **Máy ảo Android**.  
> **Thời điểm xác lập:** 2026-10-26 06:55  
> **Mã kế hoạch:** Plan 49 (`project_context/plans/plans-mobile-app/2026-10-26_plan_49_kiem_tra_chan_doan_va_khac_phuc_frontend_maui_windows_va_android.md`)  
> **Tài liệu phối hợp:** [`mobile-project/dotnet-maui-ver/README.md`](../../mobile-project/dotnet-maui-ver/README.md)  

---

## I. TỔNG QUAN VẤN ĐỀ VÀ BẢN CHẤT KỸ THUẬT

Khác với các framework lai như React Native (vốn chỉ cần Node.js, Metro Bundler và máy ảo), **.NET MAUI (Multi-platform App UI)** là nền tảng Native đa mục tiêu được biên dịch trực tiếp ra mã nhị phân nền tảng:
- Trên **Windows**: Sử dụng WinUI 3 và Windows App SDK. Mặc định MAUI đóng gói ứng dụng dưới dạng gói **MSIX (Packaged App)**. Nếu hệ điều hành Windows chưa bật Developer Mode hoặc chưa cho phép đăng ký Appx không có chứng chỉ Store, Windows sẽ chặn triển khai (`DEP0700`). Ngoài ra, các tiến trình nền của Visual Studio / MSBuild thường xuyên giữ khóa tập tin (File Lock) đối với các DLL chiếu của Windows App SDK.
- Trên **Android**: Sử dụng Android SDK, AAPT2 (Android Asset Packaging Tool) và Mono runtime. Hệ điều hành Windows giới hạn độ dài đường dẫn chuẩn ở mức 260 ký tự (`MAX_PATH`). Do thư mục dự án nằm ở cấp lồng sâu kèm các gói thư viện AndroidX/Google, công cụ AAPT2 sẽ lập tức văng lỗi đường dẫn (`PathTooLongException` / `Could not find a part of the path`). Đồng thời, nếu file cấu hình `.csproj` ép điều kiện TargetFramework không linh hoạt, Visual Studio sẽ không hiển thị danh sách máy ảo Android.

---

## II. MA TRẬN 4 GIAI ĐOẠN KIỂM TRA & XỬ LÝ TRIỆT ĐỂ

```text
┌────────────────────────────────────────────────────────────────────────────────────────┐
│               KẾ HOẠCH KIỂM TRA VÀ XỬ LÝ RUNTIME FRONTEND .NET MAUI                    │
├────────────────────────────────────────────────────────────────────────────────────────┤
│ GIAI ĐOẠN 1: CHẨN ĐOÁN & THÁO GỠ NÚT THẮT WINDOWS APP (WINUI 3 / MSIX)                │
│    1.1. Kiểm tra trạng thái Windows Developer Mode & Registry AppModelUnlock          │
│    1.2. Đánh giá phương án Packaged (MSIX) vs Unpackaged (<WindowsPackageType>None)    │
│    1.3. Cơ chế xử lý triệt để File Lock DLL (Kill process & Purge bin/obj)            │
│                                                                                        │
│ GIAI ĐOẠN 2: CHẨN ĐOÁN & THÁO GỠ NÚT THẮT MÁY ẢO ANDROID (EMULATOR)                    │
│    2.1. Cấu hình ổ đĩa ảo `subst` rút ngắn triệt để đường dẫn (vượt rào MAX_PATH)     │
│    2.2. Hiệu chỉnh TargetFrameworks trong `.csproj` để Visual Studio nhận diện Android  │
│    2.3. Cấu hình `android:usesCleartextTraffic="true"` và Base URL mạng `10.0.2.2`     │
│    2.4. Kiểm tra Workload `maui-android` & Trình quản lý Android Emulator              │
│                                                                                        │
│ GIAI ĐOẠN 3: THỰC THI KIỂM THỬ KHỞI CHẠY (SMOKE TEST EXECUTION)                        │
│    3.1. Test 1: Khởi chạy Windows Machine từ Visual Studio / Terminal                  │
│    3.2. Test 2: Khởi chạy Android Emulator thông qua ổ đĩa ảo                          │
│                                                                                        │
│ GIAI ĐOẠN 4: NÂNG CẤP TÀI LIỆU HƯỚNG DẪN & BỔ SUNG PREREQUISITES VÀO README.MD         │
│    4.1. Bổ sung bảng Requirements chi tiết cho .NET MAUI                               │
│    4.2. Cẩm nang xử lý lỗi thường gặp (Troubleshooting Guide: Lock file, MAX_PATH)    │
└────────────────────────────────────────────────────────────────────────────────────────┘
```

---

## III. NỘI DUNG CHI TIẾT TỪNG GIAI ĐOẠN

### GIAI ĐOẠN 1: CHẨN ĐOÁN & THÁO GỠ NÚT THẮT WINDOWS APP

#### 1.1. Kiểm tra và Kích hoạt Developer Mode / Registry Unlock
* **Bản chất lỗi:** Khi chạy hồ sơ `"Windows Machine"` với `commandName: "MsixPackage"`, Visual Studio cố gắng đăng ký gói ứng dụng lỏng (Loose Layout Registration) vào hệ thống Windows. Nếu máy tính chưa bật quyền phát triển, Windows sẽ báo lỗi `DEP0700`.
* **Kế hoạch kiểm tra:**
  1. Kiểm tra xem máy tính đã bật Developer Mode chưa:
     - Cách 1 (Giao diện): `Settings > System (hoặc Privacy & Security) > For developers > Developer Mode = ON`.
     - Cách 2 (Registry an toàn, tạm thời): Kiểm tra khóa `HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock`:
       - Giá trị `AllowDevelopmentWithoutDevLicense = 1` (DWORD).
       - Giá trị `AllowAllTrustedApps = 1` (DWORD).
  2. Cung cấp lệnh PowerShell (Run as Administrator) để bật nhanh khi cần:
     ```powershell
     reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock" /t REG_DWORD /f /v "AllowDevelopmentWithoutDevLicense" /d "1"
     reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock" /t REG_DWORD /f /v "AllowAllTrustedApps" /d "1"
     ```

#### 1.2. Đánh giá Phương án Unpackaged App (`<WindowsPackageType>None</WindowsPackageType>`)
* **Bản chất kỹ thuật:** Nếu máy tính của người dùng gặp khó khăn về chính sách bảo mật không cho phép bật Developer Mode, ta có thể bổ sung cấu hình **Unpackaged Mode**. Khi đó ứng dụng chạy như một file `.exe` Win32 thông thường, không bị Windows quản lý dưới dạng MSIX Package.
* **Kế hoạch kiểm tra:**
  - Thêm cấu hình hỗ trợ chạy Unpackaged trong `launchSettings.json`:
    ```json
    "Windows Machine (Unpackaged)": {
      "commandName": "Project",
      "nativeDebugging": false
    }
    ```
  - Nếu chọn Unpackaged, kiểm tra xem máy đã cài `Windows App SDK Runtime` hay chưa.

#### 1.3. Xử lý triệt để Lỗi File Lock DLL của Windows App SDK
* **Bản chất lỗi:** 
  `Could not copy "...\Microsoft.Windows.ApplicationModel.DynamicDependency.Projection.dll" to "bin\Debug\...". Exceeded retry count of 10. Failed.`
  Nguyên nhân do tiến trình `ExpenseTracker.Maui.exe` chạy ngầm chưa tắt hẳn, hoặc Visual Studio Roslyn/MSBuild Compiler Server giữ file lock.
* **Kế hoạch kiểm tra:**
  - Tạo một lệnh/script dọn dẹp tiến trình triệt để trước khi biên dịch lại:
    ```powershell
    Stop-Process -Name "ExpenseTracker.Maui" -Force -ErrorAction SilentlyContinue
    Stop-Process -Name "msbuild" -Force -ErrorAction SilentlyContinue
    Stop-Process -Name "dotnet" -Force -ErrorAction SilentlyContinue
    ```

---

### GIAI ĐOẠN 2: CHẨN ĐOÁN & THÁO GỠ NÚT THẮT MÁY ẢO ANDROID

#### 2.1. Giải pháp Ổ Đĩa Ảo `subst` Xóa Bỏ Hoàn Toàn Rào Cản Đường Dẫn Dài (`MAX_PATH`)
* **Bản chất lỗi:** Thư mục hiện tại có độ dài lớn:
  `D:\University Study Subjects\Information System Design Analysis\Project\Analyze System\mobile-project\dotnet-maui-ver\frontend\ExpenseTracker.Maui\` (127 ký tự). Khi sinh các file tạm trung gian trong `obj\Debug\net10.0-android\...`, đường dẫn vượt quá 260 ký tự khiến AAPT2 lỗi.
* **Giải pháp ổ đĩa ảo tạm thời (`subst`):**
  - Gắn ổ đĩa ảo `X:` trỏ thẳng vào thư mục làm việc:
    ```powershell
    subst X: "D:\University Study Subjects\Information System Design Analysis\Project\Analyze System"
    ```
  - Khi đó, đường dẫn dự án rút ngắn chỉ còn:
    `X:\mobile-project\dotnet-maui-ver\frontend\ExpenseTracker.Maui\` (rút ngắn hơn 100 ký tự!).
  - **Ưu điểm vượt trội:** Ổ đĩa ảo này nằm hoàn toàn trên RAM phiên làm việc của người dùng, **tự động giải phóng biến mất khi khởi động lại máy tính**, hoặc có thể gỡ chủ động bằng lệnh:
    ```powershell
    subst X: /D
    ```

#### 2.2. Cấu hình Lại `TargetFrameworks` trong `ExpenseTracker.Maui.csproj`
* **Vấn đề hiện tại:**
  ```xml
  <TargetFrameworks Condition="'$(BuildingForAndroid)' == 'true'">net10.0-android</TargetFrameworks>
  <TargetFrameworks Condition="'$(BuildingForAndroid)' != 'true'">net10.0-windows10.0.19041.0</TargetFrameworks>
  ```
  Khi mở bằng Visual Studio, biến `$(BuildingForAndroid)` mặc định không có, nên Visual Studio **hoàn toàn không nhận diện được `net10.0-android`**, dẫn đến menu thả xuống thiết bị (Device Target Dropdown) không hiển thị Máy ảo Android!
* **Kế hoạch khắc phục:**
  - Cấu hình mở cả 2 target hoặc cấu hình thông minh:
    ```xml
    <TargetFrameworks>net10.0-windows10.0.19041.0;net10.0-android</TargetFrameworks>
    ```
  - Đồng thời thiết lập điều kiện biên dịch theo Platform để tránh xung đột mã nguồn giữa Windows và Android.

#### 2.3. Cấu hình Kết Nối Mạng Android Emulator (`10.0.2.2` & Cleartext Traffic)
* **Kế hoạch kiểm tra:**
  1. Trong `AndroidManifest.xml`: Kiểm tra và đảm bảo đã có thuộc tính `android:usesCleartextTraffic="true"` bên trong thẻ `<application>` để máy ảo Android được phép gọi HTTP không có SSL (`http://10.0.2.2:5018`).
  2. Trong `MauiProgram.cs`: Xác nhận logic `DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5018" : "http://localhost:5018"` đang hoạt động đúng.

#### 2.4. Kiểm tra Android Workload & Emulator
* **Kế hoạch kiểm tra:**
  - Kiểm tra xem máy tính đã cài đặt workload `maui-android` chưa bằng lệnh:
    ```powershell
    dotnet workload list
    ```
  - Nếu thiếu, cung cấp lệnh cài đặt: `dotnet workload install maui-android`.

---

### GIAI ĐOẠN 3: THỰC THI KIỂM THỬ KHỞI CHẠY (SMOKE TEST EXECUTION)

1. **Kiểm thử 1 (Windows Desktop):**
   - Đảm bảo app đang ở chế độ **Mock Data Offline** (trong `MauiProgram.cs`).
   - Biên dịch và chạy trên nền Windows: Xác nhận cửa sổ WinUI 3 mở lên hiển thị màn hình Đăng nhập / Trang chủ, các nút bấm hoạt động mượt mà.
2. **Kiểm thử 2 (Android Emulator):**
   - Kích hoạt ổ đĩa ảo `subst X:`.
   - Mở Solution từ ổ đĩa `X:` hoặc chạy lệnh `dotnet build -f net10.0-android` từ ổ `X:`.
   - Xác nhận biên dịch 0 error, AAPT2 hoàn tất trơn tru.

---

### GIAI ĐOẠN 4: CẬP NHẬT TÀI LIỆU & BỔ SUNG PREREQUISITES VÀO README.MD

Sau khi hoàn tất kiểm tra và giải pháp được xác nhận vận hành thành công:
1. Cập nhật [`mobile-project/dotnet-maui-ver/README.md`](../../mobile-project/dotnet-maui-ver/README.md).
2. Thêm một mục riêng biệt trước phần hướng dẫn chạy Frontend:
   - **Bảng so sánh Requirements giữa React Native và .NET MAUI.**
   - **Danh sách điều kiện tiên quyết (Prerequisites) bắt buộc:**
     - Cài đặt Workload: `.NET Multi-platform App UI development` trong Visual Studio Installer.
     - Bật Windows Developer Mode (hoặc chạy lệnh Registry unlock).
     - Thiết lập ổ đĩa ảo `subst` để xử lý triệt để lỗi đường dẫn dài trên Android.
     - Cài đặt Android SDK & HAXM / Hyper-V Emulator.
   - **Cẩm nang Xử lý sự cố (Troubleshooting):**
     - Cách xử lý khi bị lock DLL `DynamicDependency.Projection.dll`.
     - Cách xử lý khi máy ảo Android không gọi được API máy chủ (`10.0.2.2`).

---

## IV. BƯỚC ĐI TIẾP THEO

Kế hoạch này được đệ trình lên Chủ tọa để xem xét. Sau khi Chủ tọa đồng ý với kế hoạch:
1. Chúng ta sẽ tiến hành kiểm tra và thực hiện ngay **Giai đoạn 1 (Windows App)**.
2. Tiếp tục thực hiện **Giai đoạn 2 (Máy ảo Android & Ổ đĩa ảo `subst`)**.
3. Cập nhật toàn diện tài liệu vào `README.md`.
