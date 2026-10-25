# 📋 TÀI LIỆU BÀN GIAO & PHẠM VI SỬA ĐỔI TÀI LIỆU THIẾT KẾ PTTKHTTT / KHÓA LUẬN TỐT NGHIỆP
## CHƯƠNG TRÌNH ĐỒNG BỘ NĂNG LỰC HỆ THỐNG AI MỞ RỘNG (AI-INVOLVED SYSTEM EXPANSION)
**Ngày ban hành:** 2026-10-24  
**Tác giả bàn giao:** Hội Đồng Đa Tác Tử (Mobile .NET MAUI Council)  
**Bên tiếp nhận:** Chuyên Viên / Agent Phân Tích Thiết Kế Hệ Thống Thông Tin (PTTKHTTT Session)  
**Mục tiêu bàn giao:** Cung cấp toàn bộ phạm vi, bối cảnh, đặc tả chi tiết các Use Cases mới và các hạng mục cần chỉnh lý trong Báo cáo Khóa luận / Hồ sơ Thiết kế PTTKHTTT phục vụ hạn chót nghiệm thu **Thứ Tư, ngày 04/11/2026**.

---

## I. BỐI CẢNH ĐỒNG BỘ HAI DỰ ÁN & HAI DEADLINE

Dự án phát triển hệ thống đang trải qua hai giai đoạn nghiệm thu quan trọng với hai hạn chót lệch nhau đúng 1 tuần:
1. **Deadline 1 (Thứ Tư, 28/10/2026):** Nghiệm thu môn **Lập trình Ứng dụng Di động (.NET MAUI)**. Sản phẩm nộp gồm: Video Demo (5-8 phút), Báo cáo PDF, File APK cài đặt, và bảo vệ mã nguồn Clean MVVM (B3), SignalR Hub (B2), Push Notifications (Phần A).
2. **Deadline 2 (Thứ Tư, 04/11/2026):** Nghiệm thu môn **Phân tích Thiết kế Hệ thống Thông tin (PTTKHTTT) & Báo cáo Khóa Luận Tốt Nghiệp**.

Trong Phiên họp Hội đồng Toàn diện ngày 24/10/2026, Người dùng (Project Owner) đã đưa ra các quyết định nâng cấp mang tính đột phá về AI nhằm giải quyết triệt để vấn đề "hội chứng kiệt sức vì nhập liệu thủ công" (Manual Logging Fatigue) và tối ưu hóa chi phí. 

Để đảm bảo tính nhất quán tuyệt đối giữa **Mã nguồn thực tế (Codebase)** và **Tài liệu học thuật (Specification Docs)**, tài liệu này chỉ rõ chính xác những gì phiên PTTKHTTT cần sửa đổi và bổ sung trong Chương 3 của Khóa luận.

---

## II. DANH MỤC CÁC HẠNG MỤC CẦN SỬA ĐỔI TRONG TÀI LIỆU THIẾT KẾ

### 1. Cập Nhật Bảng Tác Nhân Hệ Thống (System Actors - Mục 3.1)
Trong tài liệu phân tích ban đầu, AI chỉ được mô tả như một dịch vụ thứ cấp bên ngoài (`External AI Service`). Cần cập nhật lại bảng tác nhân tại Mục 3.1:

| Tên Tác Nhân | Loại Tác Nhân | Vai Trò & Trách Nhiệm Mới Trong Hệ Thống |
| :--- | :--- | :--- |
| **Người dùng cá nhân (Personal User)** | Tác nhân chính (Primary Actor) | Khởi tạo giao dịch, ra khẩu lệnh giọng nói, tải ảnh hóa đơn/file dữ liệu, kiểm tra đối soát (HITL) và tiếp nhận các cú hích tài chính. |
| **Trợ lý AI Thông minh (Intelligent AI Agent / Co-Pilot)** | Tác nhân hỗ trợ thông minh (Intelligent Secondary Actor) | Không chỉ bóc tách văn bản đơn thuần mà đóng vai trò Tác tử điều hướng ý định (Voice Intent Router), tự động phân tích ngữ cảnh hóa đơn/note vụn vặt, và đóng vai Cố vấn tài chính điều hành (Executive Advisor) theo triết lý Richard Thaler. |
| **Hệ thống Thông báo Ngân hàng & Ví Điện tử (Banking & E-Wallet Notification Provider)** *(Tác nhân mới)* | Tác nhân hỗ trợ thụ động (Passive External Actor) | Phát sinh các thông báo biến động số dư trên thanh trạng thái hệ điều hành Android (hỗ trợ ACB, MoMo, ZaloPay, Vietcombank, MBBank) để hệ thống lắng nghe và trích xuất. |

---

### 2. Tinh Chỉnh & Làm Sâu Sắc 3 Use Cases Hiện Có (Chuẩn IEEE 29148)

#### A. Use Case `UC-03.07`: Bóc tách hóa đơn bằng Multimodal AI Vision
* **Hiện trạng cũ:** Chỉ tập trung mô tả bóc tách hóa đơn bán lẻ siêu thị/cửa hàng tiện lợi.
* **Nội dung cần bổ sung vào Đặc tả:**
  - *Mở rộng phạm vi đầu vào:* Chính thức hỗ trợ bóc tách **Hóa đơn Tiền Điện (EVN) và Hóa đơn Cấp Nước điện tử (E-Invoice)** theo chuẩn Bộ Tài chính.
  - *Quy tắc nghiệp vụ:* Tự động trích xuất các trường đặc thù: Kỳ tiêu thụ (Tháng/Năm), Chỉ số cũ, Chỉ số mới, Sản lượng tiêu thụ ($m^3$ hoặc $kWh$), Đơn giá bậc thang, và Tổng tiền thanh toán.
  - *Ánh xạ tự động:* Tự động ánh xạ vào Danh mục hệ thống *"Hóa đơn & Tiện ích"* (thuộc nhóm Nhu cầu thiết yếu 50% trong chuẩn 50/30/20) và đưa sản lượng tiêu thụ vào trường Ghi chú (`Note`) để phục vụ vẽ biểu đồ theo dõi sản lượng điện nước hàng tháng.

#### B. Use Case `UC-03.11`: Nâng cấp từ "Ghi chép giọng nói" sang "Tác tử Giọng nói Điều hướng Ý định (Voice-Driven Intent Agent)"
* **Hiện trạng cũ:** Chỉ mô tả việc người dùng nói một câu thu/chi, hệ thống chuyển sang text và lưu giao dịch.
* **Nội dung cần bổ sung vào Đặc tả:**
  - *Nâng cấp vai trò:* Hệ thống tích hợp **Mô hình Phân loại Ý định (Intent Classification & Function Calling)**.
  - *Phân nhánh luồng xử lý:*
    + **Nhánh Ý định 1 (Giao dịch tài chính - `ADD_TRANSACTION`):** Nhận diện từ nói tắt tiếng Việt phong phú ("50k", "năm xị", "hai triệu rưỡi", "trẹo", "đi chợ"). Trích xuất số tiền, tự động chọn ví và danh mục, hiển thị popup xác nhận nhanh.
    + **Nhánh Ý định 2 (Điều hướng giao diện - `NAVIGATE_PAGE`):** Khi người dùng ra lệnh điều hướng (*"Tạo danh mục mới Nuôi Mèo"*, *"Mở báo cáo tháng này"*), AI nhận diện tham số và ra lệnh cho hệ thống điều hướng trực tiếp sang màn hình tương ứng, điền sẵn thông tin (Prefill Form) và dừng lại ở nút Xác nhận chờ người dùng bấm (Bảo đảm nguyên tắc Human-in-the-loop).

#### C. Use Case `UC-07.01`: Báo cáo Thu - Chi theo thời gian (Luồng phụ A1 - Xuất Báo Cáo)
* **Hiện trạng cũ:** Chỉ ghi chung chung là xuất file PDF/Excel bảng số liệu.
* **Nội dung cần bổ sung vào Đặc tả Luồng phụ A1:**
  - Bổ sung 2 chế độ xuất file rõ ràng:
    + *Chế độ 1 (Raw Data Export):* Xuất nhanh bảng tính thô, không dùng AI, tốn 0 token.
    + *Chế độ 2 (Smart Executive Report):* Tích hợp Cố vấn AI phân tích dữ liệu tổng hợp tháng, viết một đoạn **Nhận định Điều hành (Executive Commentary)** dài 3-5 câu (đánh giá khách quan sức khỏe tài chính, tuyên dương điểm sáng tiết kiệm và đưa ra khuyến nghị hành vi cho tháng tới). Đoạn nhận định này được trang hoàng trang trọng ở đầu bản PDF hoặc Sheet đầu tiên của file Excel.

---

### 3. Toàn Văn Đặc Tả 2 Use Cases Mới (Bảng Dọc 2 x N Chuẩn IEEE 29148)

Để phục vụ việc chèn trực tiếp vào Mục 3.2 Chương 3 của Khóa luận (nâng tổng số từ 63 lên 65 Use Cases), Hội đồng đã biên soạn sẵn toàn văn đặc tả chuẩn mực:

---

#### 📄 USE CASE MỚI 1: `UC-03.12`
### UC-03.12: Tự động phát hiện biến động số dư qua Thông báo Hệ thống Android

| Thuộc tính (Mục) | Nội dung đặc tả chi tiết |
| :--- | :--- |
| **1. Tên Use Case** | **Tự động phát hiện biến động số dư qua Thông báo Hệ thống Android** |
| **2. Mã Use Case (UC ID)** | `UC-03.12` |
| **3. Mức độ ưu tiên** | Cao (Điểm cộng công nghệ sáng tạo) |
| **4. Tác nhân chính (Primary Actor)** | Người dùng cá nhân (Personal User), Hệ thống Thông báo Ngân hàng (Banking Notification Provider) |
| **5. Mô tả tóm tắt** | Cho phép ứng dụng hoạt động dưới dạng dịch vụ nền lắng nghe (Notification Listener Service) trên hệ điều hành Android; tự động nhận diện các thông báo biến động số dư trừ tiền từ các ứng dụng ngân hàng và ví điện tử phổ biến tại Việt Nam (ACB, MoMo, ZaloPay, Vietcombank, MBBank); bóc tách số tiền, đơn vị thụ hưởng và thời gian giao dịch; tự động gửi thông báo nội bộ nhắc nhở người dùng ghi nhanh vào sổ cái chỉ với một chạm, triệt tiêu hoàn toàn sự phiền hà của việc nhập liệu thủ công. |
| **6. Điểm kích hoạt (Trigger)** | Ứng dụng ngân hàng hoặc ví điện tử phát sinh thông báo đẩy biến động giảm số dư trên thanh trạng thái thiết bị. |
| **7. Tiền điều kiện (Pre-conditions)** | 1. Ứng dụng đang chạy trên hệ điều hành Android.<br>2. Người dùng đã cấp quyền truy cập thông báo (`BIND_NOTIFICATION_LISTENER_SERVICE`) trong Cài đặt hệ thống.<br>3. Người dùng đã kích hoạt tính năng "Tự động bắt giao dịch ngân hàng" trong mục Cài đặt ứng dụng. |
| **8. Hậu điều kiện (Post-conditions)** | - *Thành công:* Hệ thống bóc tách chính xác số tiền và đơn vị thụ hưởng, tạo một thông báo nội bộ trên thiết bị mời người dùng xác nhận ghi chép.<br>- *Thất bại:* Bỏ qua thông báo nếu thông báo không thuộc danh sách ngân hàng hỗ trợ hoặc không chứa nội dung biến động giảm số dư. |
| **9. Luồng sự kiện chính (Basic Flow)** | 1. Dịch vụ nền `BankNotificationListenerService` nhận sự kiện thông báo mới từ hệ điều hành Android.<br>2. Hệ thống kiểm tra tên gói ứng dụng (`PackageName`): Xác định xem thông báo có xuất phát từ các ứng dụng được hỗ trợ hay không (ACB: `com.acb.mobile`, MoMo: `com.mservice.momotransfer`, ZaloPay: `vn.com.vng.zalopay`, Vietcombank: `com.VCB`, MBBank: `com.mbmobile`).<br>3. Hệ thống trích xuất chuỗi tiêu đề và nội dung thông báo.<br>4. Hệ thống áp dụng Bộ phân tích cú pháp Regex đặc thù của từng ứng dụng để tìm dấu hiệu biến động giảm số dư (`-`, `GD: -`, `thanh toan`, `chuyen tien`, `so du giam`).<br>5. Hệ thống bóc tách các trường dữ liệu: Số tiền chi tiêu, Đơn vị thụ hưởng / Nội dung thanh toán, Thời gian giao dịch.<br>6. Hệ thống đối chiếu và tự động gán Ví nguồn tương ứng (ví dụ: Ví ACB hoặc Ví MoMo đã tạo trong hệ thống).<br>7. Hệ thống tự động đoán danh mục chi tiêu dựa trên nội dung giao dịch qua `AutoCategorizationService`.<br>8. Hệ thống phát một thông báo nội bộ (Local Notification) lên màn hình: "Phát hiện Bạn vừa chi tiêu {Số_tiền} VND từ {Tên_Ngân_Hàng}. Chạm để ghi vào sổ cái!".<br>9. Người dùng chạm vào thông báo: Ứng dụng tự động mở ra màn hình Xác nhận giao dịch với các thông tin đã được điền sẵn 100%.<br>10. Người dùng bấm "Xác nhận lưu": Giao dịch chi tiêu được ghi vào CSDL và số dư ví được cập nhật chuẩn xác. |
| **10. Luồng phụ (Alternative Flows)** | - *A1 (Người dùng vuốt bỏ qua thông báo):* Nếu người dùng không chạm vào thông báo nội bộ, hệ thống tự động xóa thông báo sau 24 giờ và không can thiệp vào số dư ví.<br>- *A2 (Người dùng chỉnh sửa danh mục trước khi lưu):* Tại bước 9, người dùng có thể chạm đổi danh mục khác trước khi bấm Lưu. |
| **11. Luồng ngoại lệ (Exception Flows)** | - *E1 (Quyền truy cập thông báo bị người dùng thu hồi):* Nếu quyền bị tắt trong cài đặt Android, dịch vụ nền tự động tạm dừng và không phát sinh lỗi hệ thống.<br>- *E2 (Nội dung thông báo không chứa thông tin số tiền):* Nếu thông báo ngân hàng là tin quảng cáo hoặc khuyến mãi, hệ thống tự động loại bỏ. |
| **12. Yêu cầu phi chức năng & Bảo mật** | - *Bảo mật thông tin tối mật:* Dịch vụ nền chỉ đọc chuỗi văn bản thông báo, tuyệt đối không truy cập bộ nhớ hay thông tin đăng nhập ngân hàng; tuân thủ chính sách bảo mật dữ liệu cá nhân.<br>- *Tiết kiệm pin:* Quá trình lọc Regex diễn ra trong dưới 5 mili-giây, không gây hao pin thiết bị.<br>- *Giới hạn nền tảng:* Tính năng chỉ khả dụng trên Android, hiển thị trạng thái không hỗ trợ trên các nền tảng khác. |
| **13. Tiêu chí nghiệm thu (Acceptance Criteria)** | **Kịch bản 1: Bắt thông báo trừ tiền ACB thành công**<br>• *Điều kiện (Given):* Người dùng đã bật quyền lắng nghe thông báo và có Ví ACB trong ứng dụng.<br>• *Khi (When):* Ứng dụng ACB phát thông báo: "TK 123456 tại ACB: -50,000VND vao 14:30. ND: Thanh toan ca phe Highlands".<br>• *Thì (Then):* Ứng dụng PFM phát thông báo nội bộ phát hiện chi tiêu 50.000 VND từ ACB, chạm vào hiển thị đúng số tiền 50.000 VND và ví ACB. |

---

#### 📄 USE CASE MỚI 2: `UC-07.06`
### UC-07.06: Nhập khẩu dữ liệu tài chính đa nguồn Hybrid (Hybrid Multi-Source Financial Data Import)

| Thuộc tính (Mục) | Nội dung đặc tả chi tiết |
| :--- | :--- |
| **1. Tên Use Case** | **Nhập khẩu dữ liệu tài chính đa nguồn Hybrid (Hybrid Multi-Source Financial Data Import)** |
| **2. Mã Use Case (UC ID)** | `UC-07.06` |
| **3. Mức độ ưu tiên** | Trung bình - Cao |
| **4. Tác nhân chính (Primary Actor)** | Người dùng cá nhân (Personal User), Trợ lý AI Thông minh (Intelligent AI Agent) |
| **5. Mô tả tóm tắt** | Cung cấp cơ chế nhập khẩu hàng loạt dữ liệu lịch sử tài chính từ nhiều nguồn khác nhau; kết hợp linh hoạt kiến trúc Hybrid: Tự động sử dụng thư viện công nghiệp đọc nhanh các tệp bảng tính có cấu trúc chuẩn (.xlsx, .csv) với chi phí 0 đồng, hoặc kích hoạt mô hình ngôn ngữ lớn (LLM) để thấu hiểu ngữ cảnh và bóc tách các tệp ghi chú tự do phi cấu trúc (.txt, note vụn vặt, ảnh chụp màn hình ghi chép); toàn bộ dữ liệu được đổ về màn hình đối soát (Preview HITL) cho phép người dùng kiểm tra trước khi ghi hàng loạt vào cơ sở dữ liệu. |
| **6. Điểm kích hoạt (Trigger)** | Người dùng chọn tính năng "Nhập dữ liệu từ tệp" trong phân hệ Quản lý Giao dịch hoặc Báo cáo. |
| **7. Tiền điều kiện (Pre-conditions)** | 1. Người dùng đã đăng nhập thành công vào hệ thống.<br>2. Người dùng có sẵn tệp bảng tính (.xlsx, .csv), tệp văn bản ghi chú (.txt) hoặc ảnh chụp sổ tay ghi chép. |
| **8. Hậu điều kiện (Post-conditions)** | - *Thành công:* Các bản ghi giao dịch hợp lệ được lưu hàng loạt vào bảng `ExpenseTransactions` / `IncomeTransactions` và cập nhật tương ứng vào số dư các ví tài chính.<br>- *Thất bại:* Không có dữ liệu nào được ghi nhận nếu người dùng hủy bỏ tại màn hình Preview; hiển thị thông báo lỗi cụ thể. |
| **9. Luồng sự kiện chính (Basic Flow)** | 1. Người dùng truy cập tính năng "Nhập khẩu dữ liệu" và chọn tệp tin từ thiết bị.<br>2. Hệ thống kiểm tra định dạng tệp tải lên:<br>&nbsp;&nbsp;&nbsp;&nbsp;• *Trường hợp A (Tệp Excel/CSV có cấu trúc):* Hệ thống kích hoạt module `ClosedXML`, đọc trực tiếp các dòng và cột trong bộ nhớ.<br>&nbsp;&nbsp;&nbsp;&nbsp;• *Trường hợp B (Tệp văn bản tự do .txt hoặc ảnh chụp note):* Hệ thống gửi nội dung/ảnh lên Gemini 2.5 Flash kèm Strict JSON Schema để bóc tách ngữ cảnh từng dòng giao dịch.<br>3. Hệ thống chuẩn hóa dữ liệu đầu ra thành danh sách các đối tượng giao dịch: Ngày giao dịch, Số tiền, Loại (Thu/Chi), Danh mục gợi ý, Ghi chú.<br>4. Hệ thống hiển thị Màn hình Xem trước Đối soát (Preview HITL Screen):<br>&nbsp;&nbsp;&nbsp;&nbsp;• Hiển thị danh sách dạng bảng các giao dịch được bóc tách.<br>&nbsp;&nbsp;&nbsp;&nbsp;• Cho phép người dùng chạm sửa nhanh số tiền, đổi danh mục hoặc bỏ chọn những dòng không muốn nhập.<br>5. Người dùng kiểm tra danh sách và bấm nút "Xác nhận nhập tất cả".<br>6. Hệ thống mở một Database Transaction nguyên tử (ACID):<br>&nbsp;&nbsp;&nbsp;&nbsp;• Thêm hàng loạt bản ghi vào cơ sở dữ liệu.<br>&nbsp;&nbsp;&nbsp;&nbsp;• Tính toán tổng biến động và cập nhật số dư các ví tương ứng.<br>7. Hệ thống hiển thị thông báo "Nhập khẩu thành công {N} giao dịch vào sổ cái tài chính!". |
| **10. Luồng phụ (Alternative Flows)** | - *A1 (Tự động tạo danh mục mới nếu chưa tồn tại):* Nếu tệp nhập khẩu chứa danh mục chưa có trong hệ thống, hệ thống hỏi người dùng có muốn tự động tạo danh mục con đó không.<br>- *A2 (Hủy bỏ nhập khẩu):* Người dùng có thể nhấn nút "Hủy" tại màn hình Preview để hủy toàn bộ quá trình mà không làm biến động CSDL. |
| **11. Luồng ngoại lệ (Exception Flows)** | - *E1 (Tệp rỗng hoặc định dạng không hỗ trợ):* Hệ thống hiển thị thông báo yêu cầu tải lên tệp hợp lệ.<br>- *E2 (Lỗi trong quá trình bóc tách AI):* Nếu văn bản ghi chú quá mờ hoặc không có thông tin số tiền, hệ thống thông báo các dòng không hợp lệ để người dùng chỉnh sửa thủ công. |
| **12. Yêu cầu phi chức năng & Bảo mật** | - *Hiệu năng:* Nhập khẩu file Excel 500 dòng hoàn tất dưới 1 giây; bóc tách file note vụn vặt qua AI hoàn tất dưới 3 giây.<br>- *Toàn vẹn dữ liệu:* Áp dụng Bulk Insert trong Transaction bảo đảm không bị lưu dở dang khi có lỗi mạng giữa chừng. |
| **13. Tiêu chí nghiệm thu (Acceptance Criteria)** | **Kịch bản 1: Nhập khẩu file Excel thu chi thành công**<br>• *Điều kiện (Given):* Người dùng có file Excel 20 dòng ghi chép chi tiêu tiền ăn uống và xăng xe.<br>• *Khi (When):* Người dùng tải file lên và bấm xác nhận.<br>• *Thì (Then):* Hệ thống đọc đúng 20 dòng, hiển thị bảng Preview đầy đủ và nhập thành công vào CSDL. |

---

## III. HƯỚNG DẪN CẬP NHẬT CÁC MÔ HÌNH HÓA UML

Phiên PTTKHTTT chỉ cần cập nhật thêm vào hai sơ đồ đã có:
1. **Sơ đồ Ca sử dụng Tổng quát (`files/UCD-DuoNoPro.drawio`):**
   - *Tab Phân hệ 3:* Bổ sung thêm Use Case `UC-03.12: Tự động phát hiện biến động số dư qua thông báo Android`, kết nối với Actor `Personal User` và Actor mới `Banking Notification Provider`.
   - *Tab Phân hệ 7:* Bổ sung Use Case `UC-07.06: Nhập khẩu dữ liệu tài chính đa nguồn Hybrid`, kết nối với `Personal User` và `Intelligent AI Agent`.
2. **Sơ đồ Lớp Chi tiết (`files/ClassDiagram.drawio`):**
   - Trong Bounded Context Quản lý Chi tiêu & Báo cáo: Bổ sung 2 Interface dịch vụ:
     + `IBankNotificationParserService` với phương thức `ParseNotification(string packageName, string title, string content): ParsedBankTransactionDto`.
     + `IHybridDataImportService` với phương thức `ImportFromSpreadsheetAsync(...)` và `ImportFromUnstructuredTextAsync(...)`.

---
*Tài liệu được chuyển giao từ Hội đồng Mobile .NET MAUI để phục vụ hoàn thiện tài liệu PTTKHTTT nộp ngày 04/11/2026.*
