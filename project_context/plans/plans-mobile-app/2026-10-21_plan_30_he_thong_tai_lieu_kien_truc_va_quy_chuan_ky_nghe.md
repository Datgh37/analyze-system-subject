# 📋 KẾ HOẠCH HỘI ĐỒNG: XÂY DỰNG HỆ THỐNG TÀI LIỆU KIẾN TRÚC & QUY CHUẨN KỸ NGHỆ
## (COUNCIL PLAN: ARCHITECTURAL DOCUMENTATION, CODE CRAFT STANDARDS & ZERO-CODE FREEZE)

> **Mã kế hoạch:** `PLAN-30`  
> **Thời điểm:** 21/10/2026  
> **Trạng thái:** `LOCKED & ACTIVE (PRE-FLIGHT DOCUMENTATION PHASE)`  
> **Chủ trì phiên họp:** Agent 00 (Council Orchestrator)  
> **Thành viên tham gia:** 
> - Agent 04: Eric Evans (Domain-Driven Design & Clean Architecture)
> - Agent 05: Martin Kleppmann & Pramod Sadalage (Universal Database & Concurrency)
> - Agent 10: Dan Abramov (Client Architecture, MVVM & Impeccable Craft)
> - Agent 11: Bruce Schneier (Fintech Security & Mobile KeyStore)
> - Agent 12: Kent Beck & Martin Fowler (Incremental Unit Testing & Clean Code)
> - Agent 07: Werner Vogels (DevOps & Environment Setup)
> **Tài liệu căn cứ:** [`PLAN-29`](2026-10-21_plan_29_dotnet_maui_enterprise_kickoff.md), [`decisions_and_changes_log.md`](../decisions_and_changes_log.md).

---

## 1. MỤC TIÊU CỐT LÕI (CORE OBJECTIVES)

1. **Thực thi nghiêm ngặt cơ chế "Zero-Code Freeze":** Tuyệt đối không viết code ứng dụng vội vã khi các tài liệu kiến trúc, khế ước giao tiếp và tiêu chuẩn kỹ nghệ chưa được văn bản hóa đầy đủ.
2. **Ban hành Quy chuẩn Kỹ nghệ Mã nguồn (.agents/rules):** Thiết lập quy định về phong cách code C# 14 / .NET 10, nguyên lý DRY, comment súc tích dành cho Code Reviewer (giữ nguyên 100% technical terms tiếng Anh), kiểm thử đơn vị cuốn chiếu (Incremental Unit Testing) và kiểm tra có trọng tâm tiết kiệm token.
3. **Xuất bản Bản Đặc tả Kiến trúc Kỹ thuật Toàn diện (`TECHNICAL_ARCHITECTURE_SPEC.md`):**
   - Bản đồ kiến trúc 2 tầng: Clean Architecture (Backend) + Clean MVVM (Mobile Client).
   - Thiết kế CSDL 2 tầng: SQLite Local (Offline-first) + PostgreSQL 16 Central (3NF & JSONB) + IMemoryCache nội tại (Không dùng Redis).
   - Khế ước giao tiếp chi tiết: Toàn bộ danh mục REST API Endpoints và SignalR Hub Events.
   - Cơ chế bảo mật: JWT Access Token + Refresh Token Rotation + .NET MAUI SecureStorage.
   - Chiến lược Mocking dịch vụ ngoại vi: Mock Gemini Vision OCR & Mock VietQR Gateway.
4. **Cập nhật và đồng bộ nhật ký quyết định (`decisions_and_changes_log.md`):** Ghi nhận Quyết định 97 chốt toàn bộ các thỏa thuận kỹ thuật của phiên họp.

---

## 2. LỘ TRÌNH TRIỂN KHAI CHI TIẾT (ACTION ROADMAP)

### Bước 1: Ban hành Quy tắc Kỹ nghệ `.agents/rules/05_engineering_architecture_and_code_craft_rules.md`
- Ràng buộc C# 14 / .NET 10: `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`, `<Nullable>enable</Nullable>`.
- Ràng buộc phong cách chú thích mã nguồn: Ngắn gọn (1-2 dòng), tập trung giải thích "Tại sao" (Why), không trang trí ASCII art, giữ nguyên 100% English technical terms cho Code Reviewer.
- Ràng buộc kiểm thử: Viết Unit Test (xUnit + FluentAssertions) ngay sau khi hoàn thành từng module/function nghiệp vụ.
- Ràng buộc quản trị Token: Kiểm tra mã nguồn có trọng tâm (Scoped Inspection), không quét lặp toàn bộ dự án khi không có yêu cầu.
- Ràng buộc DRY: Tái sử dụng tối đa ResourceDictionary, BaseEntity, Result Pattern, Custom ContentView.

### Bước 2: Xuất bản Bản Đặc tả Kiến trúc Kỹ thuật Toàn diện
- Tạo tệp [`mobile-project/dotnet-maui-ver/TECHNICAL_ARCHITECTURE_SPEC.md`](../../mobile-project/dotnet-maui-ver/TECHNICAL_ARCHITECTURE_SPEC.md).
- Bao quát 6 chương:
  1. Tổng quan Kiến trúc Hệ thống (System Architecture Blueprint).
  2. Lược đồ CSDL 2 Tầng & Khóa Concurrency (Database Schema & Concurrency Spec).
  3. Khế ước API & SignalR Hub Events (Master API Catalog 40 Endpoints bám sát 63 Use Cases).
  4. Cơ chế Xác thực & Bảo mật Phần cứng (Authentication & Security Flow).
  5. Chiến lược Giả lập Dịch vụ Ngoại vi (External Services Mocking).
  6. Khung Kiểm thử & Chuẩn Mực Code Craft (Testing & Code Craft Guidelines).

### Bước 3: Xuất bản Bản Đồ Lộ Trình Xây Dựng Dự Án Toàn Diện
- Tạo tệp [`mobile-project/dotnet-maui-ver/MASTER_PROJECT_ROADMAP.md`](../../mobile-project/dotnet-maui-ver/MASTER_PROJECT_ROADMAP.md).
- Thống nhất cấu trúc thư mục gốc: **`mobile-project/dotnet-maui-ver/backend/`** và **`mobile-project/dotnet-maui-ver/frontend/`**.
- Chi tiết hóa 8 Giai đoạn triển khai cuốn chiếu (Giai đoạn 0 đến Giai đoạn 7) bám sát 63 Use Cases chi tiết.

### Bước 4: Đồng bộ Nhật Ký Quyết Định Hệ Thống
- Thêm Quyết Định 97 và Quyết Định 98 vào [`project_context/decisions_and_changes_log.md`](../decisions_and_changes_log.md).
- Báo cáo kết quả tổng kết với người dùng và chuyển sang trạng thái sẵn sàng cho công đoạn Scaffolding Solution.

---

## 3. TIÊU CHÍ NGHIỆM THU (ACCEPTANCE CRITERIA)

- [x] Đã chốt phiên họp hội đồng theo lệnh người dùng.
- [x] Quy tắc `05_engineering_architecture_and_code_craft_rules.md` được tạo và kích hoạt trong hệ thống Agents.
- [x] Bản đặc tả `TECHNICAL_ARCHITECTURE_SPEC.md` được biên soạn hoàn chỉnh, không có placeholder `TODO`.
- [x] Bản đồ lộ trình `MASTER_PROJECT_ROADMAP.md` được biên soạn chi tiết 8 giai đoạn.
- [x] Thống nhất đặt tên thư mục gốc là `backend/` và `frontend/`.
- [x] Không có bất kỳ dòng code logic ứng dụng nào bị sinh ra trước khi hoàn tất tài liệu (Zero-Code Compliance).
- [x] `decisions_and_changes_log.md` được cập nhật chính xác, không làm sai lệch format.
