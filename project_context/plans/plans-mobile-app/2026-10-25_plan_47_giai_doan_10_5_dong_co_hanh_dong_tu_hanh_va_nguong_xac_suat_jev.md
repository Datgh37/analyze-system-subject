# KẾ HOẠCH TRIỂN KHAI KỸ THUẬT: PLAN-47
## MỞ RỘNG GIAI ĐOẠN 10: ĐỘNG CƠ HÀNH ĐỘNG TỰ HÀNH HƯỚNG MÁY CHỦ (SERVER-DRIVEN ACTION PROTOCOL), PHÂN TÍCH HÀNH VI & NGƯỠNG XÁC SUẤT JEV

> **Mã kế hoạch:** `PLAN-47` (Thuộc khuôn khổ **Giai đoạn 10: Tinh chỉnh & Hoàn thiện Hệ thống AI**)  
> **Chuyên gia chủ trì:** Agent 00 Orchestrator, Agent 01 Richard Thaler, Agent 04 Eric Evans & Agent 10 Dan Abramov  
> **Ngày phê duyệt:** 25/10/2026  
> **Trạng thái:** `APPROVED BY USER — READY FOR IMPLEMENTATION`  
> **Định hướng cốt lõi:**
> 1. Kết hợp Hybrid giữa Systematic Core (Sổ cái ACID 3NF) và AI Agent (Phân tích hành vi, bóc tách thực tế).
> 2. Giải quyết bài toán hiện sinh: Thu hẹp khoảng cách giữa "sự trung thực của người dùng" và "thực tế dòng tiền" bằng cơ chế **Bắt thực tế thụ động (Passive Reality Capture)** qua thông báo ngân hàng và **Interactive 1-Chạm**.
> 3. Tuyệt đối loại bỏ văn phong robot AI; không dùng Modal/Popup chặn màn hình gây ức chế.
> 4. Áp dụng tư duy xác suất của **Jev** (`confidenceScore` từ 0.0 đến 1.0) để quyết định mức độ can thiệp: $\ge 0.85$ (Lưu nhanh 1-chạm); $0.50 - 0.85$ (Lựa chọn ngang hàng); $< 0.50$ (Im lặng, đưa vào mục chờ phân loại, không spam thông báo rác).
> 5. **Quy tắc giao diện Frontend:** Chỉ dựng **Wireframe** (XAML + ViewModel + Binding) và cập nhật tài liệu kỹ thuật [`HUONG_DAN_FE.md`](../../mobile-project/dotnet-maui-ver/HUONG_DAN_FE.md) cho người bạn cùng nhóm phụ trách thiết kế giao diện tự do styling.

---

## 1. MÔ HÌNH DỮ LIỆU ĐA HÌNH HƯỚNG MÁY CHỦ (SERVER-DRIVEN ACTION PROTOCOL)

Backend đóng vai trò là nhạc trưởng sinh ra các gói hành động `AutonomousActionDto` đổ về Client:

```json
{
  "actionId": "act_882910",
  "type": "QUICK_TRANSACTION_CONFIRM",
  "urgency": "NORMAL",
  "confidenceScore": 0.92,
  "display": {
    "title": "Khoản chi Shopee 185.000 đ",
    "subtitle": "Tài khoản ACB lúc 23:15",
    "badge": "Mua sắm",
    "badgeColor": "#F59E0B"
  },
  "primaryAction": {
    "label": "Lưu nhanh",
    "actionType": "QUICK_SAVE",
    "endpoint": "/api/actions/quick-commit",
    "payload": {
      "amount": 185000,
      "categoryName": "Mua sắm",
      "walletName": "ACB",
      "sourceType": "BankNotification"
    }
  },
  "secondaryAction": {
    "label": "Đổi danh mục",
    "actionType": "NAVIGATE",
    "route": "AddTransactionPage?amount=185000&source=ACB"
  }
}
```

---

## 2. PHÂN TẦNG XỬ LÝ THEO NGƯỠNG XÁC SUẤT JEV (PROBABILISTIC GATING)

- **Ngưỡng Cao ($\ge 0.85$):** AI nhận diện với độ tin cậy áp đảo (từ khóa và lịch sử khớp hoàn toàn) $\rightarrow$ Sinh thẻ có 1 nút bấm sáng nhất **`[✓ Lưu nhanh]`**. Người dùng chạm 1 cái là giao dịch vào sổ cái.
- **Ngưỡng Trung Bình ($0.50 \le \text{Confidence} < 0.85$):** AI phân vân giữa 2–3 danh mục (ví dụ: băn khoăn giữa "Ăn uống" và "Đi chợ") $\rightarrow$ Sinh thẻ dạng **Jev Choice**: Hiển thị 2 lựa chọn ngang hàng `[Ăn uống]` | `[Đi chợ]`.
- **Ngưỡng Thấp ($< 0.50$):** AI thấy câu lệnh hoặc tin nhắn mơ hồ $\rightarrow$ **Tuyệt đối không đẩy thông báo làm phiền**. Lưu vào trạng thái "Chờ phân loại" trong app để người dùng xử lý khi có thời gian.

---

## 3. CÁC ĐẦU VIỆC THỰC THI CỤ THỂ

### Chặng 47.1: Xây Dựng Tầng Domain & Application Backend
1. Tạo thư mục `backend/ExpenseTracker.Application/DTOs/Autonomous/`:
   - `AutonomousActionDto.cs`: Đặc tả gói hành động đa hình (Display info, Badges, Primary/Secondary actions, Confidence score).
   - `QuickCommitActionRequest.cs`: DTO cho thao tác lưu nhanh 1 chạm.
2. Tạo Service Interface & Implementation:
   - `IAutonomousActionEngineService.cs`
   - `AutonomousActionEngineService.cs`: Chứa logic phân tích giao dịch/biến động số dư, tính toán confidence score theo nguyên lý Jev, sinh danh sách các hành động cần xử lý.
3. Đăng ký DI trong `ExpenseTracker.Application/DependencyInjection.cs`.

### Chặng 47.2: Xây Dựng WebApi Controller Backend
1. Tạo `AutonomousActionsController.cs` (`api/actions`):
   - `GET /api/actions/inbox`: Lấy danh sách các hành động đang chờ người dùng xử lý.
   - `POST /api/actions/quick-commit`: Tiếp nhận lệnh 1-chạm từ Client để tự động hạch toán giao dịch vào CSDL PostgreSQL và phát tín hiệu SignalR.
   - `POST /api/actions/{id}/dismiss`: Bỏ qua đề xuất.

### Chặng 47.3: Dựng Wireframe Client & ViewModel .NET MAUI
1. Models & Repositories:
   - `ExpenseTracker.Maui/Models/Autonomous/AutonomousActionModel.cs`
   - `IAutonomousActionRepository.cs` (gồm Api và Mock in-memory).
2. Wireframe Components:
   - Tạo ContentView `Views/Autonomous/SmartActionCardView.xaml`: Wireframe hiển thị thẻ hành động nhỏ gọn có badge và nút bấm.
   - Tạo ViewModel `ViewModels/Autonomous/ActionInboxViewModel.cs`: Quản lý danh sách hành động, thực thi lệnh `QuickCommitCommand` và `DismissCommand`.
   - Nhúng `ActionInboxView` lên đầu `DashboardPage.xaml` dưới dạng Wireframe (tự động ẩn khi không có việc cần xử lý).

### Chặng 47.4: Kiểm Toán Chất Lượng TDD & Cập Nhật HUONG_DAN_FE.md
1. Viết Unit Tests `AutonomousActionEngineServiceTests.cs` kiểm thử các ngưỡng xác suất Jev ($\ge 0.85$, $0.5 - 0.85$, $< 0.5$).
2. Cập nhật tài liệu [`HUONG_DAN_FE.md`](../../mobile-project/dotnet-maui-ver/HUONG_DAN_FE.md):
   - Mô tả cấu trúc dữ liệu `AutonomousActionModel`.
   - Hướng dẫn bạn cùng nhóm cách áp dụng Design System, màu sắc badge theo phân nhóm và hiệu ứng trượt/ẩn khi người dùng bấm Lưu nhanh.

---

## 4. CẬP NHẬT GIAI ĐOẠN 11 ĐỂ BAO QUÁT NỘI DUNG MỚI
Giai đoạn 11 sẽ được cập nhật để bao quát:
1. **Kiểm toán E2E Full-Stack:** Bổ sung kịch bản kiểm thử luồng bắt biến động số dư Android $\rightarrow$ Backend sinh Autonomous Action $\rightarrow$ Mobile nhận qua SignalR $\rightarrow$ Bấm Lưu nhanh 1-chạm $\rightarrow$ CSDL cập nhật số dư ví.
2. **Kịch bản Demo 1-Click & Video:** Dành 1-2 phút trong Video Demo để phô diễn tính năng "Bắt thực tế và Lưu nhanh 1-chạm" để gây ấn tượng mạnh với Giảng viên chấm thi.
3. **Báo cáo cuối kỳ:** Bổ sung phần giải trình kiến trúc Server-Driven Action Protocol và ngưỡng xác suất Jev vào tài liệu báo cáo DOCX/PDF.
