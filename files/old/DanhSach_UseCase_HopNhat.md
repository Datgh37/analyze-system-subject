# DANH SÁCH USE CASE HỢP NHẤT TOÀN DIỆN (THE GRAND SYNTHESIS)
## HỆ THỐNG QUẢN LÝ CHI TIÊU & LẬP NGÂN SÁCH CÁ NHÂN THÔNG MINH (PFM)
### Tích hợp Chuẩn Kỹ thuật Phần mềm (BA / ChatGPT) & Kỹ nghệ Tài chính Chuyên sâu (Proto / Council)

---

> **Tài liệu tham chiếu chuẩn:**
> * **Đơn vị:** Trường Đại học Công nghệ Đồng Nai (DNTU) — Khoa Công nghệ Thông tin.
> * **Đề tài:** Nghiên cứu, thiết kế và xây dựng Hệ thống Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh (Tích hợp AI) — *Smart Expense & Budget Manager*.
> * **Nhóm tác giả:** Trịnh Thành Đạt (MSSV: 1923050149) & Trần Trọng Đại (MSSV: 1923050745) — Lớp 23DTH1.
> * **Định hướng cốt lõi:** 100% Tài chính cá nhân (Personal Finance Management - PFM), giải quyết trọn vẹn kỷ luật dòng tiền, tâm lý học hành vi và tối ưu hóa tài chính.

---

## 1. TỔNG QUAN HỆ THỐNG & RANH GIỚI TÁC TỬ (SYSTEM BOUNDARY & ACTORS)

Hệ thống được thiết kế tuân thủ nghiêm ngặt chuẩn UML 2.5 và tiêu chuẩn phân tích yêu cầu IEEE 29148. Ranh giới hệ thống phân định rõ giữa tác tử bên ngoài và các động cơ tính toán nội bộ.

### 1.1. Bản Đồ Tác Tử (Actor Roster)

| STT | Tên Tác Tử (Actor) | Phân Loại | Vai Trò Nghiệp Vụ Trong Hệ Thống |
| :---: | :--- | :--- | :--- |
| **1** | **Personal User** *(Người dùng cá nhân)* | **Primary Actor** | Tác nhân con người duy nhất trực tiếp tương tác với hệ thống: quản lý đa ví, ghi nhận thu chi, thiết lập ngân sách, quản lý nợ và đưa ra quyết định tài chính của chính mình. |
| **2** | **AI Vision OCR Service** | **Secondary Actor** *(External)* | Dịch vụ đám mây xử lý thị giác máy tính, bóc tách dữ liệu từ ảnh chụp hóa đơn mua sắm thành danh sách mặt hàng đa món (`itemized items`). |
| **3** | **AI Recommendation Service** | **Secondary Actor** *(External)* | Mô hình trí tuệ nhân tạo bên ngoài hỗ trợ phân tích xu hướng chi tiêu sâu, dự báo rủi ro thâm hụt và gợi ý các cú hích tài chính (Nudges). |
| **4** | **Notification Service** | **Secondary Actor** *(External)* | Hệ thống hạ tầng truyền thông (Firebase Cloud Messaging / Apple APNs / SMS Gateway) gửi thông báo cảnh báo tức thời khi vượt hạn mức hoặc sắp đến hạn thanh toán nợ/thuê bao. |
| **5** | **VietQR Gateway** | **Secondary Actor** *(External)* | Cổng dịch vụ thanh toán quốc gia tạo mã QR động/tĩnh chuẩn NAPAS 247 hỗ trợ người dùng thực hiện giao dịch chuyển khoản thanh toán nhanh. |

> [!NOTE]
> **Quy tắc bất biến về ranh giới hệ thống:**
> Cơ sở dữ liệu (PostgreSQL/Redis), Công cơ tính hạn mức ngày (DSA Engine), Bộ tối ưu hóa nợ (Snowball/Avalanche Solver) và Bộ kiểm toán số học (Mathematical Audit) hoàn toàn nằm **bên trong ranh giới hệ thống (System Boundary)**, do đó **tuyệt đối không được mô hình hóa thành Actor** trong sơ đồ Use Case.

---

## 2. BẢN ĐỒ TỔNG QUÁT 9 PHÂN HỆ USE CASE (MASTER OVERVIEW)

Toàn bộ hệ thống được phân rã thành 9 phân hệ Use Case cấp cao theo mô hình 3 tầng kiến trúc hướng giá trị:

```text
┌──────────────────────────────────────────────────────────────────────────────────┐
│ TẦNG 3: TRÍ TUỆ NHÂN TẠO & HỖ TRỢ QUYẾT ĐỊNH (Intelligence & Decision Loop)       │
│  [UC-07] Báo cáo & Phân tích Đa chiều (Net Worth, Spending Velocity, Xu hướng)    │
│  [UC-08] Dự báo Tài chính & Rủi ro (Dự phóng Điểm cạn tiền, Mô phỏng What-if)     │
│  [UC-09] Cú hích Hành vi & Hỗ trợ Quyết định (Dynamic Rebalancing, Human Approval)│
├──────────────────────────────────────────────────────────────────────────────────┤
│ TẦNG 2: HOẠCH ĐỊNH KỶ LUẬT & TỐI ƯU HÓA NỢ (Planning & Discipline Tier)          │
│  [UC-05] Quản lý Ngân sách Đa phương pháp (50/30/20, ZBB, Kakeibo, DSA thích ứng) │
│  [UC-06] Quỹ Tích lũy & Tối ưu hóa Nợ Cá nhân (Emergency Fund, Snowball/Avalanche)│
├──────────────────────────────────────────────────────────────────────────────────┤
│ TẦNG 1: NỀN TẢNG DỮ LIỆU & ĐA VÍ CÁ NHÂN (Core Data & Multi-Wallet Tier)         │
│  [UC-01] Quản lý Tài khoản & Đa Ví Cá nhân (Auth, Hồ sơ, ACID Multi-Wallet)       │
│  [UC-02] Quản lý Thu nhập Cá nhân (Thu nhập phát sinh, Lương định kỳ, Ví đích)    │
│  [UC-03] Quản lý Chi tiêu & Bóc tách Hóa đơn AI (Chi tiêu, OCR, Math Audit, VietQR)│
│  [UC-04] Quản lý Danh mục Thu Chi & Quy tắc (Phân cấp Cha-Con, Ánh xạ Kakeibo/50) │
└──────────────────────────────────────────────────────────────────────────────────┘
```

---

## 3. ĐẶC TẢ CHI TIẾT 9 PHÂN HỆ & CÁC USE CASE CON ĐÃ HỢP NHẤT

---

### PHÂN HỆ UC-01: QUẢN LÝ TÀI KHOẢN & ĐA VÍ CÁ NHÂN
**Mục tiêu:** Cung cấp định danh bảo mật cho người dùng cá nhân, đồng thời thiết lập nền tảng quản lý tài sản tài chính đa kênh (Tiền mặt, Ngân hàng, Thẻ tín dụng, Ví điện tử) với tính toàn vẹn giao dịch tuyệt đối (ACID).

| Mã UC Con | Tên Use Case Con | Actor Tương Tác | Quan Hệ UML | Ý Nghĩa Nghiệp Vụ & Cơ Chế Kỹ Thuật |
| :--- | :--- | :--- | :--- | :--- |
| **UC-01.01** | Đăng ký tài khoản | Personal User | Cơ sở | Tạo tài khoản mới bằng Email/SĐT kèm mật khẩu mã hóa bcrypt/Argon2. |
| **UC-01.02** | Đăng nhập hệ thống | Personal User | Cơ sở | Xác thực người dùng, cấp phát cặp JWT Token (Access Token 15m, Refresh Token 7d). |
| **UC-01.03** | Đăng xuất | Personal User | Cơ sở | Hủy phiên làm việc, đưa Refresh Token vào danh sách thu hồi (Redis Blacklist). |
| **UC-01.04** | Quản lý thông tin hồ sơ | Personal User | Cơ sở | Cập nhật họ tên, ảnh đại diện, tiền tệ mặc định (VND, USD) và chu kỳ tài chính (ngày nhận lương). |
| **UC-01.05** | Đổi mật khẩu | Personal User | Cơ sở | Thay đổi mật khẩu định kỳ với yêu cầu nhập mật khẩu hiện tại. |
| **UC-01.06** | Quên / Khôi phục mật khẩu | Personal User | Cơ sở | Gửi mã OTP xác thực qua Email để thiết lập mật khẩu mới an toàn. |
| **UC-01.07** | Khởi tạo & Cấu hình Đa Ví | Personal User | Cơ sở | Khởi tạo các loại ví độc lập: Tiền mặt, Tài khoản ngân hàng, Thẻ tín dụng (có hạn mức âm & ngày sao kê), Ví điện tử (MoMo, ZaloPay). |
| **UC-01.08** | Chuyển tiền nội bộ giữa các ví | Personal User | Cơ sở | Chuyển tiền qua lại giữa các ví của chính mình (Rút ATM, nạp tiền ví điện tử, thanh toán dư nợ thẻ tín dụng) tuân thủ tính toán vẹn ACID: Giảm ví nguồn $\leftrightarrow$ Tăng ví đích. |
| **UC-01.09** | Đối soát & Cân chỉnh số dư | Personal User | Cơ sở | Người dùng kiểm đếm tiền thực tế và cập nhật lại số dư ví; hệ thống tự động sinh giao dịch điều chỉnh chênh lệch (*Adjustment Transaction*). |

---

### PHÂN HỆ UC-02: QUẢN LÝ THU NHẬP CÁ NHÂN
**Mục tiêu:** Ghi nhận và theo dõi mọi dòng tiền vào (*Inflows*) của cá nhân, làm cơ sở định lượng khả năng chi tiêu và cấp vốn cho các quỹ ngân sách.

| Mã UC Con | Tên Use Case Con | Actor Tương Tác | Quan Hệ UML | Ý Nghĩa Nghiệp Vụ & Cơ Chế Kỹ Thuật |
| :--- | :--- | :--- | :--- | :--- |
| **UC-02.01** | Thêm khoản thu nhập mới | Personal User | Cơ sở | Nhập số tiền, ngày nhận, danh mục thu, ghi chú và chọn **Ví nhận tiền** (tự động cộng dồn số dư ví). |
| **UC-02.02** | Xem danh sách khoản thu | Personal User | Cơ sở | Tra cứu lịch sử thu nhập theo ngày, tuần, tháng, quý với bộ lọc theo nguồn thu hoặc theo ví. |
| **UC-02.03** | Xem chi tiết khoản thu nhập | Personal User | Cơ sở | Xem thông tin chi tiết: số tiền, nguồn thu, chứng từ đính kèm, ví tiếp nhận. |
| **UC-02.04** | Chỉnh sửa khoản thu nhập | Personal User | Cơ sở | Chỉnh sửa sai sót số tiền hoặc ngày nhận; hệ thống tự động hoàn tác và cập nhật lại số dư ví tương ứng. |
| **UC-02.05** | Xóa khoản thu nhập | Personal User | Cơ sở | Xóa khoản thu hợp lệ; hệ thống khấu trừ lại số tiền tương ứng khỏi số dư ví đích. |
| **UC-02.06** | Phân loại nguồn thu nhập | Personal User | `<<include>>` UC-02.01 | Gán nguồn thu vào danh mục: Lương chính thức, Thưởng, Thu nhập làm thêm (Freelance), Lợi nhuận đầu tư, Trợ cấp/Quà tặng. |
| **UC-02.07** | Quản lý thu nhập định kỳ | Personal User | Mở rộng | Thiết lập lịch thu nhập lặp lại (Lương ngày 05 hàng tháng); hệ thống tự động nhắc nhở hoặc tự động ghi nhận khi đến ngày. |

---

### PHÂN HỆ UC-03: QUẢN LÝ CHI TIÊU & BÓC TÁCH HÓA ĐƠN AI
**Mục tiêu:** Ghi nhận toàn bộ dòng tiền ra (*Outflows*) qua nhiều phương thức (nhập tay, chụp hóa đơn, quét mã), phát hiện bẫy rò rỉ dòng tiền và hỗ trợ thanh toán tức thời.

| Mã UC Con | Tên Use Case Con | Actor Tương Tác | Quan Hệ UML | Ý Nghĩa Nghiệp Vụ & Cơ Chế Kỹ Thuật |
| :--- | :--- | :--- | :--- | :--- |
| **UC-03.01** | Thêm khoản chi tiêu thủ công | Personal User | Cơ sở | Nhập số tiền, ngày chi, danh mục, ví thanh toán (trừ số dư ví ngay lập tức). |
| **UC-03.02** | Xem danh sách chi tiêu | Personal User | Cơ sở | Hiển thị dòng thời gian (*timeline*) chi tiêu, lọc theo ví, danh mục hoặc khoảng thời gian. |
| **UC-03.03** | Xem chi tiết khoản chi tiêu | Personal User | Cơ sở | Xem chi tiết: mặt hàng, thuế, hóa đơn gốc đính kèm, danh mục ngân sách bị trừ. |
| **UC-03.04** | Chỉnh sửa khoản chi tiêu | Personal User | Cơ sở | Sửa số tiền/danh mục/ví thanh toán; hệ thống tự động cân chỉnh lại số dư ví liên quan. |
| **UC-03.05** | Xóa khoản chi tiêu | Personal User | Cơ sở | Xóa giao dịch chi; hệ thống hoàn trả số tiền lại vào số dư ví tương ứng. |
| **UC-03.06** | Gắn chi tiêu với mục tiêu / danh mục | Personal User | `<<include>>` UC-03.01 | Liên kết khoản chi vào mục tiêu cụ thể (ví dụ: Mua sắm laptop cho mục tiêu công việc) để trừ vào quỹ mục tiêu. |
| **UC-03.07** | Quét & Bóc tách hóa đơn bằng AI OCR | Personal User, AI Vision OCR Service | Cơ sở | Chụp/tải ảnh hóa đơn siêu thị/nhà hàng; AI trích xuất tên cửa hàng, ngày giờ, danh sách từng món hàng (`itemized list`) và giá trị tương ứng. |
| **UC-03.08** | Hậu kiểm toán số học hóa đơn | Hệ thống nội bộ | `<<include>>` UC-03.07 | Tự động xác thực số học: $\sum \text{Món hàng} + \text{Thuế VAT} - \text{Khuyến mãi} \stackrel{?}{=} \text{Tổng tiền}$. Nếu có sai lệch, cảnh báo người dùng kiểm tra lại trước khi lưu. |
| **UC-03.09** | Quét & Phát hiện Bẫy đăng ký định kỳ | Personal User, Notification Service | `<<extend>>` UC-03.02 | Thuật toán nhận diện các khoản chi lặp lại chu kỳ cố định (Netflix, Spotify, Apple Music, SaaS); cảnh báo nguy cơ lãng phí nếu không còn sử dụng thường xuyên. |
| **UC-03.10** | Tạo giao dịch thanh toán VietQR | Personal User, VietQR Gateway | `<<extend>>` UC-03.01 | Tự động tạo mã VietQR chuẩn thanh toán nhanh 24/7 để người dùng quét chuyển khoản nhanh chóng không cần nhập STK thủ công. |

---

### PHÂN HỆ UC-04: QUẢN LÝ DANH MỤC THU CHI & QUY TẮC THÔNG MINH
**Mục tiêu:** Xây dựng hệ thống phân loại dữ liệu tài chính khoa học, hỗ trợ cấu trúc phân cấp linh hoạt và thiết lập ánh xạ vào các trường phái ngân sách kinh điển.

| Mã UC Con | Tên Use Case Con | Actor Tương Tác | Quan Hệ UML | Ý Nghĩa Nghiệp Vụ & Cơ Chế Kỹ Thuật |
| :--- | :--- | :--- | :--- | :--- |
| **UC-04.01** | Thêm danh mục mới | Personal User | Cơ sở | Tạo danh mục thu/chi với tên, biểu tượng (icon) và màu sắc nhận diện trực quan. |
| **UC-04.02** | Xem danh sách danh mục | Personal User | Cơ sở | Hiển thị toàn bộ cây danh mục dạng danh sách hoặc dạng sơ đồ phân nhánh. |
| **UC-04.03** | Chỉnh sửa danh mục | Personal User | Cơ sở | Đổi tên, thay icon, chuyển đổi vị trí trong cây phân cấp. |
| **UC-04.04** | Xóa danh mục | Personal User | Cơ sở | Xóa danh mục (chuyển các giao dịch cũ về danh mục mặc định "Khác" để bảo toàn lịch sử dữ liệu). |
| **UC-04.05** | Thiết lập phân cấp Cha - Con | Personal User | Cơ sở | Cấu trúc phân nhánh logic: Ví dụ Cha: *Ăn uống* $\rightarrow$ Con: *Đi chợ/Siêu thị*, *Cà phê*, *Ăn ngoài*. |
| **UC-04.06** | Ánh xạ danh mục chuẩn ngân sách | Personal User | Cơ sở | Gán danh mục vào các nhóm ngân sách: **Needs / Wants / Savings-Debt** (Chuẩn 50/30/20) hoặc **4 Cột trụ Kakeibo** (*Thiết yếu, Lãng phí, Học tập/Văn hóa, Bất thường*). |
| **UC-04.07** | Đặt quy tắc phân loại tự động | Personal User | Mở rộng | Tạo bộ lọc từ khóa: Nếu tên giao dịch chứa "WinMart" hoặc "Coopmart" $\rightarrow$ tự động gán vào danh mục *Đi chợ siêu thị*. |

---

### PHÂN HỆ UC-05: QUẢN LÝ NGÂN SÁCH ĐA PHƯƠNG PHÁP & HẠN MỨC NGÀY
**Mục tiêu:** Cung cấp bộ công cụ lập kế hoạch tài chính thích ứng đa phương pháp, ngăn chặn việc chi tiêu vượt tầm kiểm soát thông qua chỉ số Hạn mức ngày DSA thích ứng thời gian thực.

| Mã UC Con | Tên Use Case Con | Actor Tương Tác | Quan Hệ UML | Ý Nghĩa Nghiệp Vụ & Cơ Chế Kỹ Thuật |
| :--- | :--- | :--- | :--- | :--- |
| **UC-05.01** | Thiết lập ngân sách chu kỳ | Personal User | Cơ sở | Khởi tạo ngân sách tháng/tuần theo 1 trong 3 phương pháp: **Quy tắc 50/30/20**, **Zero-Based Budgeting (ZBB - Phân bổ từng đồng)** hoặc **Sổ Kakeibo Nhật Bản**. |
| **UC-05.02** | Theo dõi tiến độ ngân sách | Personal User | Cơ sở | Trực quan hóa tỷ lệ phần trăm đã tiêu so với hạn mức (Thanh tiến độ đổi màu: Xanh $<70\%$, Vàng $70-90\%$, Đỏ $>90\%$). |
| **UC-05.03** | Điều chỉnh hạn mức ngân sách | Personal User | Cơ sở | Tăng/giảm hạn mức của từng danh mục khi có biến động bất ngờ trong tháng. |
| **UC-05.04** | Tiếp nhận cảnh báo vượt ngân sách | Personal User, Notification Service | `<<extend>>` UC-05.02 | Hệ thống tự động bắn thông báo đẩy khi một danh mục chi chạm ngưỡng $80\%$ hoặc vượt $100\%$ hạn mức cho phép. |
| **UC-05.05** | Tính toán Hạn mức chi tiêu mỗi ngày (DSA) | Hệ thống nội bộ (hiển thị cho User) | `<<include>>` UC-05.02 | Tự động tính toán lại hạn mức mỗi ngày theo công thức: $\text{DSA} = \frac{\text{Ngân sách chi tiêu khả dụng còn lại}}{\text{Số ngày còn lại trong chu kỳ}}$. Nếu hôm nay tiêu lố, DSA ngày mai tự động siết lại. |
| **UC-05.06** | Giám sát Tốc độ tiêu tiền (Spending Velocity) | Hệ thống nội bộ (hiển thị cho User) | `<<include>>` UC-05.02 | Đo lường tỷ số giữa Tốc độ tiêu tiền thực tế ($V_{\text{actual}} = \frac{\text{Đã tiêu}}{\text{Số ngày đã qua}}$) và Tốc độ tiêu chuẩn ($V_{\text{target}} = \frac{\text{Tổng ngân sách}}{\text{Tổng số ngày}}$). Cảnh báo nếu $V_{\text{actual}} > 1.2 \times V_{\text{target}}$. |

---

### PHÂN HỆ UC-06: QUỸ TÍCH LŨY & TỐI ƯU HÓA NỢ CÁ NHÂN
**Mục tiêu:** Đảm bảo an toàn tài chính dài hạn thông qua hai trụ cột: Xây dựng Quỹ dự phòng khẩn cấp/Tích lũy mục tiêu và Tối ưu hóa lộ trình thanh toán nợ bằng các thuật toán toán học & tâm lý.

| Mã UC Con | Tên Use Case Con | Actor Tương Tác | Quan Hệ UML | Ý Nghĩa Nghiệp Vụ & Cơ Chế Kỹ Thuật |
| :--- | :--- | :--- | :--- | :--- |
| **UC-06.01** | Thiết lập mục tiêu tích lũy | Personal User | Cơ sở | Đặt mục tiêu: Tên mục tiêu (Mua xe, Du lịch, Quỹ khẩn cấp), Số tiền cần đạt, Hạn hoàn thành. |
| **UC-06.02** | Theo dõi tiến độ tích lũy | Personal User | Cơ sở | Xem tỷ lệ % hoàn thành mục tiêu, số tiền còn thiếu, thời gian dự kiến về đích theo tốc độ nạp tiền hiện tại. |
| **UC-06.03** | Đóng góp tiền vào mục tiêu | Personal User | Cơ sở | Trích tiền từ một ví cụ thể chuyển vào quỹ mục tiêu (tiền bị khóa tạm thời khỏi ngân sách chi tiêu hàng ngày). |
| **UC-06.04** | Thiết lập Quỹ Khẩn Cấp (Emergency Fund) | Personal User | `generalization` của UC-06.01 | Mục tiêu tích lũy đặc biệt được hệ thống gợi ý tính toán chuẩn: tương đương $3 - 6$ tháng chi phí sinh hoạt tối thiểu (*Needs*). |
| **UC-06.05** | Quản lý danh mục nợ cá nhân | Personal User | Cơ sở | Ghi nhận các khoản nợ: Tên chủ nợ/ngân hàng, Dư nợ gốc, Lãi suất hàng năm (%/năm), Kỳ hạn, Khoản trả tối thiểu hàng tháng (*Minimum Monthly Payment*). |
| **UC-06.06** | Mô phỏng chiến lược trả nợ tối ưu | Personal User | Cơ sở | Chạy thuật toán mô phỏng so sánh hai chiến lược: **Debt Snowball** (trả dứt điểm món nợ nhỏ nhất trước để tạo hưng phấn tâm lý) vs **Debt Avalanche** (trả món nợ lãi suất cao nhất trước để tiết kiệm tổng tiền lãi toán học). |
| **UC-06.07** | Ghi nhận thanh toán khoản nợ | Personal User | `<<include>>` UC-06.05 | Người dùng thanh toán nợ từ một ví cụ thể; hệ thống tự động cập nhật giảm dư nợ gốc và giảm số dư ví thanh toán. |

---

### PHÂN HỆ UC-07: BÁO CÁO & PHÂN TÍCH TÀI CHÍNH ĐA CHIỀU
**Mục tiêu:** Cung cấp góc nhìn toàn cảnh về sức khỏe tài chính cá nhân thông qua các chỉ số đo lường chuẩn mực: Dòng tiền ròng, Giá trị tài sản ròng và phân tích hành vi chi tiêu.

| Mã UC Con | Tên Use Case Con | Actor Tương Tác | Quan Hệ UML | Ý Nghĩa Nghiệp Vụ & Cơ Chế Kỹ Thuật |
| :--- | :--- | :--- | :--- | :--- |
| **UC-07.01** | Báo cáo Thu - Chi theo thời gian | Personal User | Cơ sở | Biểu đồ cột/đường so sánh Tổng thu vs Tổng chi theo chu kỳ tuần, tháng, quý, năm. |
| **UC-07.02** | Báo cáo Giá trị Tài sản ròng (Net Worth) | Personal User | Cơ sở | Biểu đồ tiến triển tài sản ròng qua từng tháng: $\text{Net Worth} = \sum \text{Số dư tất cả các ví} - \sum \text{Tổng dư nợ cá nhân}$. |
| **UC-07.03** | Báo cáo Dòng tiền ròng (Net Cashflow) | Personal User | Cơ sở | Đo lường dòng tiền ròng: $\text{Net Cashflow} = \text{Tổng thu} - \text{Tổng chi}$ và Tỷ lệ tiết kiệm ($\text{Savings Rate} = \frac{\text{Dòng tiền ròng}}{\text{Tổng thu}} \times 100\%$). |
| **UC-07.04** | Phân tích cơ cấu danh mục chi tiêu | Personal User | Cơ sở | Biểu đồ tròn (Donut chart) hiển thị tỷ trọng từng danh mục và tỷ trọng phân bổ theo nhóm Kakeibo/50-30-20. |
| **UC-07.05** | Phân tích xu hướng & Biến động bất thường | Personal User | Cơ sở | Phát hiện các khoản chi tăng đột biến bất thường so với mức trung bình 3 tháng gần nhất để người dùng nắm bắt nguyên nhân. |

---

### PHÂN HỆ UC-08: DỰ BÁO TÀI CHÍNH & CẢNH BÁO RỦI RO
**Mục tiêu:** Sử dụng mô hình chuỗi thời gian thống kê và AI để dự phóng tương lai tài chính, giúp người dùng chủ động phòng ngừa rủi ro thâm hụt trước khi xảy ra.

| Mã UC Con | Tên Use Case Con | Actor Tương Tác | Quan Hệ UML | Ý Nghĩa Nghiệp Vụ & Cơ Chế Kỹ Thuật |
| :--- | :--- | :--- | :--- | :--- |
| **UC-08.01** | Dự báo tổng chi tiêu cuối chu kỳ | Personal User, AI Recommendation Service | Cơ sở | Ước tính tổng số tiền người dùng sẽ tiêu hết khi chu kỳ kết thúc dựa trên tiến độ tiêu tiền hiện tại. |
| **UC-08.02** | Dự phóng Ngày cạn tiền (Out-of-Money Date) | Personal User | `<<include>>` UC-08.01 | Tính toán thời điểm số dư ví khả dụng sẽ chạm 0 nếu người dùng tiếp tục duy trì tốc độ tiêu tiền ($V_{\text{actual}}$) như hiện tại. |
| **UC-08.03** | Dự báo nguy cơ thâm hụt ngân sách danh mục | Personal User | `<<include>>` UC-08.01 | Chỉ rõ danh mục cụ thể nào (ví dụ: *Ăn ngoài*) đang có xác suất $>80\%$ sẽ bị vỡ ngân sách trước ngày 20 của tháng. |
| **UC-08.04** | Mô phỏng kịch bản thu chi (What-If Analysis) | Personal User | Cơ sở | Cho phép người dùng thử nghiệm: "Nếu tháng này mình mua iPhone 30 triệu thì cuối tháng quỹ tiết kiệm và dòng tiền còn lại bao nhiêu?". |

---

### PHÂN HỆ UC-09: CÚ HÍCH HÀNH VI & HỖ TRỢ QUYẾT ĐỊNH (HUMAN-IN-THE-LOOP)
**Mục tiêu:** Cung cấp các đề xuất thông minh mang tính cá nhân hóa (Nudges), áp dụng lý thuyết kinh tế hành vi của Richard Thaler, luôn đặt con người ở vị trí phê duyệt cuối cùng (Human-in-the-loop).

| Mã UC Con | Tên Use Case Con | Actor Tương Tác | Quan Hệ UML | Ý Nghĩa Nghiệp Vụ & Cơ Chế Kỹ Thuật |
| :--- | :--- | :--- | :--- | :--- |
| **UC-09.01** | Tiếp nhận Cú hích tài chính (AI Nudge) | Personal User, AI Recommendation Service, Notification Service | Cơ sở | Hiển thị các thông điệp gợi ý ngắn gọn, kịp thời (ví dụ: *"Bạn đã tiêu hết $90\%$ tiền Ăn ngoài, hãy thử nấu ăn tại nhà trong 4 ngày tới để bảo toàn ngân sách"*). |
| **UC-09.02** | Xem căn cứ & Lý do giải trình của đề xuất | Personal User | `<<extend>>` UC-09.01 | Cung cấp tính minh bạch (Explainable AI): Người dùng xem biểu đồ và lý do vì sao hệ thống đưa ra khuyến nghị đó. |
| **UC-09.03** | Đề xuất Tái cân đối ngân sách động | Personal User | `<<extend>>` UC-09.01 | Khi một danh mục Thiết yếu (*Needs*) bị vượt, AI đề xuất tự động trích hạn mức dư thừa từ danh mục Mong muốn (*Wants*) sang bù đắp để giữ nguyên tổng ngân sách. |
| **UC-09.04** | Xác nhận / Phê duyệt điều chỉnh ngân sách | Personal User | `<<extend>>` UC-09.03 | **Vòng lặp con người (Human-in-the-loop):** Hệ thống KHÔNG tự ý sửa đổi; chỉ khi người dùng bấm "Đồng ý điều chỉnh", ngân sách mới chính thức được cập nhật. |
| **UC-09.05** | Đánh giá tác động mua sắm lớn (Cooling-off) | Personal User | Cơ sở | Tạo "thời gian chờ tâm lý" (Cooling-off period 24h - 48h) cho các món đồ ngoài kế hoạch có giá trị lớn hơn hạn mức ngày DSA, giúp giảm thiểu quyết định chi tiêu bốc đồng. |

---

## 4. MA TRẬN TRUY VẾT NGUỒN GỐC & TỔNG HỢP HỢP NHẤT (TRACEABILITY MATRIX)

Bảng đối chiếu sau đây minh chứng tính kế thừa và sự hợp nhất toàn vẹn giữa hai nguồn dữ liệu gốc:

| Phân Hệ Cấp Cao | Đóng Góp Từ Bản Thảo Luận ChatGPT (Software Eng / BA) | Đóng Góp Từ Bản Vẽ Hội Đồng Proto (Financial Eng) | Kết Quả Đạt Được Tại Bản Hợp Nhất |
| :---: | :--- | :--- | :--- |
| **UC-01** | 6 chức năng cơ bản về xác thực tài khoản & hồ sơ. | Nghiệp vụ Đa Ví (Multi-Wallet), Chuyển tiền nội bộ ACID, Cân chỉnh số dư. | **9 Use Case con:** Đầy đủ Auth + Quản trị Đa Ví chuẩn ngân hàng ACID. |
| **UC-02** | Phân rã CRUD thu nhập, nguồn thu, thu định kỳ. | Khái niệm gắn nguồn thu trực tiếp vào Ví đích để tăng số dư. | **7 Use Case con:** Quản lý thu nhập chặt chẽ gắn liền với biến động số dư ví. |
| **UC-03** | CRUD chi tiêu, gắn mục tiêu, nhận dạng OCR cơ bản. | Hậu kiểm toán số học hóa đơn OCR, Quét bẫy đăng ký thuê bao, Cổng VietQR. | **10 Use Case con:** Bóc tách đa món, kiểm toán số học $\sum=\text{Total}$, quét rò rỉ thuê bao và thanh toán nhanh VietQR. |
| **UC-04** | CRUD danh mục, phân cấp Cha-Con, quy tắc tự động. | Ánh xạ danh mục chuẩn theo Kakeibo 4 cột và Quy tắc 50/30/20. | **7 Use Case con:** Cây danh mục linh hoạt có khả năng phục vụ trực tiếp các phương pháp ngân sách. |
| **UC-05** | Thiết lập ngân sách, theo dõi tiến độ, cảnh báo. | Đa phương pháp (50/30/20, ZBB, Kakeibo), Hạn mức ngày DSA, Tốc độ tiêu tiền Spending Velocity. | **6 Use Case con:** Ngân sách thích ứng động, tự điều chỉnh hạn mức ngày theo diễn biến thực tế. |
| **UC-06** | Mục tiêu tiết kiệm tiêu dùng cơ bản. | Quỹ Khẩn Cấp (Emergency Fund 3-6 tháng), Tối ưu hóa nợ cá nhân (Snowball vs Avalanche). | **7 Use Case con:** Giải quyết trọn vẹn cả 2 mặt của tài sản: Tích lũy mục tiêu và Thoát bẫy nợ. |
| **UC-07** | Báo cáo thu chi, tỷ trọng danh mục, xu hướng. | Báo cáo Tài sản ròng (Net Worth), Dòng tiền ròng (Net Cashflow) và Tỷ lệ tiết kiệm. | **5 Use Case con:** Báo cáo đa chiều chuẩn mực tài chính cá nhân chuyên nghiệp. |
| **UC-08** | Dự báo chi tiêu cuối kỳ, nguy cơ vượt hạn mức, What-if. | Dự phóng Ngày cạn tiền (Out-of-Money Date) dựa trên Spending Velocity. | **4 Use Case con:** Dự báo rủi ro có tính toán định lượng và mô phỏng kịch bản mua sắm lớn. |
| **UC-09** | Gợi ý chi tiêu, vòng lặp phê duyệt Human-in-the-loop. | Cú hích tái cân đối động (Dynamic Rebalancing), Ma sát tâm lý (Cooling-off period). | **5 Use Case con:** Hệ thống hỗ trợ quyết định thông minh, có giải trình và trao toàn quyền quyết định cho người dùng. |
| **TỔNG CỘNG** | **44 Use Case nguyên thủy (chưa có nghiệp vụ sâu)** | **17 Bong bóng gộp thô (chưa bóc tách CRUD)** | **50 Use Case con chi tiết, chuẩn hóa 100% UML & IEEE 29148** |

---

## 5. KẾT LUẬN & ĐỊNH HƯỚNG BƯỚC TIẾP THEO

Bản **Danh Sách Use Case Hợp Nhất Toàn Diện** này đã loại bỏ hoàn toàn các điểm thiếu nhất quán trước đây:
1. **Khắc phục sự hời hợt về mặt tài chính của bản ChatGPT:** Đã bổ sung đầy đủ Đa ví ACID, Tối ưu nợ Snowball/Avalanche, Hậu kiểm toán hóa đơn OCR, Bẫy đăng ký định kỳ, DSA và Net Worth.
2. **Khắc phục sự thiếu chuẩn mực công nghệ phần mềm của bản Proto:** Đã bóc tách rõ ràng từng chức năng CRUD, phân định ranh giới Actor chuẩn xác (loại bỏ CSDL và Engine nội bộ khỏi Actor), chuẩn hóa các mối quan hệ `<<include>>`, `<<extend>>` và `generalization`.

Tài liệu này là căn cứ vững chắc duy nhất để:
- Trình bày và thống nhất với đối tác/giảng viên hướng dẫn trong buổi làm việc ngày mai.
- Cập nhật đồng bộ vào tệp sơ đồ kỹ thuật `files/use_case_diagram_pfm_10_tabs.drawio`.
- Triển khai viết Bảng đặc tả Use Case chi tiết (IEEE 29148 Use Case Specifications) theo từng đợt.
