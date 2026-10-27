# KẾ HOẠCH TRIỂN KHAI CHI TIẾT (PLAN-55)
## ĐIỀU CHỈNH TIMELINE GIAI ĐOẠN 11 (THỨ TỰ 4-3-HỌP BACKEND-5-2) & KHỞI ĐỘNG CHẶNG 2 MỚI: CHUẨN HÓA BỘ SCRIPT 1-CLICK KHỞI CHẠY VÀ KIỂM THỬ E2E

- **Dự án:** Hệ thống Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh (Tích hợp AI) - .NET MAUI 10 & ASP.NET Core 10 Web API.
- **Thời gian lập:** 2026-10-27.
- **Căn cứ chỉ đạo:** Chỉ đạo trực tiếp của Người dùng: Đảo thứ tự các chặng còn lại của Giai đoạn 11 sang mô hình `4 -> 3 -> Phiên họp Hạ tầng Backend -> 5 -> 2` do bạn cùng nhóm chưa hoàn thiện xong giao diện Frontend; bổ sung Phiên họp Hội đồng giải quyết bài toán hạ tầng Backend cho APK trước khi đóng gói phát hành.

---

### 1. BẢNG TÁI CẤU TRÚC TIMELINE GIAI ĐOẠN 11

```text
┌────────────────────────────────────────────────────────────────────────┐
│ GIAI ĐOẠN 11: TỔNG KIỂM TOÁN FULL-STACK, SCRIPT 1-CLICK, BÁO CÁO,     │
│              ÔN TẬP VẤN ĐÁP Q&A, HỌP HẠ TẦNG BACKEND & ĐÓNG GÓI APK   │
├────────────────────────────────────────────────────────────────────────┤
│ Chặng 1: Báo cáo Kỹ thuật Cuối kỳ (BaoCao.docx) [✅ HOÀN THÀNH 100%]    │
│    └── Khớp 100% barem điểm môn Di động (Quyết định 180)               │
├────────────────────────────────────────────────────────────────────────┤
│ Chặng 2 Mới (Cũ là 4): Chuẩn Hóa Bộ Script 1-Click Run & E2E Test     │
│    ├── run_android.ps1 (Tự động Docker, WebApi, Emulator, MAUI Build)  │
│    ├── run_windows.ps1 (Khởi chạy desktop mượt mà, giải phóng lock)     │
│    └── test_api_suite.ps1 (Kiểm thử tự động toàn diện Controllers)    │
│    └── [🟡 BẮT ĐẦU TRIỂN KHAI NGAY]                                    │
├────────────────────────────────────────────────────────────────────────┤
│ Chặng 3 Mới (Cũ là 3): Cẩm Nang Ôn Tập Vấn Đáp Q&A Cuối Kỳ             │
│    └── BO_CAU_HOI_ON_TAP_VAN_DAP_CUOI_KY.md (12+ câu hỏi sát sườn)     │
│    └── Chuẩn bị ăn trọn 1.5đ vấn đáp cá nhân từng thành viên          │
├────────────────────────────────────────────────────────────────────────┤
│ Chặng 3.5 (MỚI): Phiên Họp Hội Đồng Xử Lý Hạ Tầng Backend Cho APK      │
│    ├── Bài toán: Backend local không chạy 24/7, máy ngoài không kết nối│
│    ├── Phân tích các giải pháp: Cloudflare Tunnel, Render/Fly.io, LAN  │
│    └── Thống nhất phương án kết nối cho file APK Release               │
├────────────────────────────────────────────────────────────────────────┤
│ Chặng 4 Mới (Cũ là 5): Đóng Gói Bản Cài Đặt APK Android Release        │
│    ├── Build Release ExpenseTracker.apk theo giải pháp hạ tầng         │
│    └── Soạn thảo HUONG_DAN_CAI_DAT_APK.md                              │
├────────────────────────────────────────────────────────────────────────┤
│ Chặng 5 Mới (Cũ là 2): Kịch Bản Demo & Trình Diễn Thực Tế Cuối Kỳ      │
│    ├── Thực hiện sau cùng khi bạn cùng nhóm hoàn thiện 100% Frontend    │
│    └── KICH_BAN_DEMO_THUC_TE_CUOI_KY.md dẫn dắt 5 phân cảnh đắt giá     │
└────────────────────────────────────────────────────────────────────────┘
```

---

### 2. MỤC TIÊU VÀ KẾ HOẠCH HÀNH ĐỘNG CHẶNG 2 MỚI (CHUẨN HÓA SCRIPT 1-CLICK)

1. **Rà soát & Nâng cấp script `run_android.ps1`:**
   - Đảm bảo cơ chế tự động toàn trình: Kiểm tra Docker daemon $\rightarrow$ Bật PostgreSQL container $\rightarrow$ Khởi động WebApi ngầm $\rightarrow$ Khởi động Android Emulator $\rightarrow$ Chờ `sys.boot_completed` $\rightarrow$ Biên dịch và deploy ứng dụng .NET MAUI lên thiết bị.
   - Thêm thông báo màu sắc trực quan, bắt lỗi fail-fast thân thiện cho Giảng viên.
2. **Rà soát & Tối ưu script `run_windows.ps1`:**
   - Tự động hóa kiểm tra PostgreSQL $\rightarrow$ Giải phóng appx lock $\rightarrow$ Build và Launch Windows App.
3. **Rà soát bộ kiểm thử API `test_api_suite.ps1`:**
   - Đảm bảo script kiểm tra kết nối API, chạy thông suốt toàn bộ kịch bản nghiệp vụ và khôi phục CSDL về trạng thái dữ liệu mẫu chuẩn (Demo Story Seeding).
4. **Nghiệm thu Chặng 2 Mới:**
   - Chạy thử nghiệm thực tế trên terminal xác nhận tính hoạt động ổn định.
