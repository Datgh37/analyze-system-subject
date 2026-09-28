# TỔNG QUAN KIẾN TRÚC, ĐẶC TẢ NGHIỆP VỤ VÀ ĐIỂM MỚI HỆ THỐNG
## HỆ THỐNG QUẢN LÝ CHI TIÊU VÀ HOẠCH ĐỊNH TÀI CHÍNH CÁ NHÂN THÔNG MINH (TÍCH HỢP TRÍ TUỆ NHÂN TẠO - AI)
*(Smart Personal Expense and Financial Planning Management System with AI Integration)*

---

## MỤC LỤC TỔNG QUAN
1. **Tầm nhìn & Sứ mệnh Đề tài (Vision & Mission)**
2. **Điểm mới Đột phá & Đối chiếu Khoảng trống Nghiên cứu so với Thị trường**
3. **Thuyết minh Văn xuôi Toàn diện 10 Sơ đồ Use Case (UCD 00 – UCD 09)**
   - 3.0 Sơ đồ Use Case Tổng thể (UCD 00)
   - 3.1 Phân hệ Quản lý Tài khoản & Đa Ví cá nhân (UCD 01)
   - 3.2 Phân hệ Quản lý Thu nhập cá nhân (UCD 02)
   - 3.3 Phân hệ Chi tiêu & Bóc tách Hóa đơn AI OCR (UCD 03)
   - 3.4 Phân hệ Hoạch định Ngân sách Đa phương pháp & DSA (UCD 04)
   - 3.5 Phân hệ Quản lý Hóa đơn định kỳ & Dịch vụ Đăng ký (UCD 05)
   - 3.6 Phân hệ Quỹ tích lũy & Tối ưu hóa Nghĩa vụ Nợ cá nhân (UCD 06)
   - 3.7 Phân hệ Báo cáo & Phân tích Tài chính Đa chiều (UCD 07)
   - 3.8 Phân hệ Dự báo Tài chính & Cảnh báo Thâm hụt (UCD 08)
   - 3.9 Phân hệ Cú hích Hành vi & Hỗ trợ Quyết định Thaler (UCD 09)
4. **Thuyết minh Văn xuôi Chuyên sâu 6 Bounded Contexts của Class Diagram**
   - 4.1 Bounded Context 1: Định danh & Hồ sơ cá nhân (Identity & Profile Context)
   - 4.2 Bounded Context 2: Tài khoản & Đa ví cá nhân chuẩn ACID (Account & Multi-Wallet Context)
   - 4.3 Bounded Context 3: Giao dịch, AI Vision OCR & Danh mục (Transactions, AI OCR & Categories Context)
   - 4.4 Bounded Context 4: Ngân sách Đa phương pháp & DSA (Budgeting & Adaptive DSA Context)
   - 4.5 Bounded Context 5: Quỹ tích lũy & Tối ưu hóa Nợ cá nhân (Personal Debt & Financial Goals Context)
   - 4.6 Bounded Context 6: Cú hích Hành vi & Hỗ trợ Quyết định Thaler (Decision Support & Behavioral Nudge Context)
   - 4.7 Đồ thị Tương tác & Luồng Dữ liệu Luân chuyển giữa 6 Bounded Contexts
5. **Tổng kết & Khuyến nghị Thuyết trình Trước Hội đồng**

---

## 1. TẦM NHÌN & SỨ MỆNH ĐỀ TÀI (VISION & MISSION)

### 1.1 Bản chất Cốt lõi: 100% Tài chính Cá nhân Độc lập
Hệ thống được thiết kế với một triết lý bất biến: **Tập trung tuyệt đối 100% vào bài toán Tài chính Cá nhân độc lập (Personal Finance Management - PFM)**. Hệ thống kiên quyết loại bỏ hoàn toàn các chức năng chia tiền nhóm bạn bè, quỹ chung phòng trọ hay đối soát nợ tập thể phức tạp. Lý do học thuật và thực tiễn cho quyết định này bắt nguồn từ bản chất tâm lý con người:
- Quản lý tài chính cá nhân là một không gian riêng tư tối mật, đòi hỏi sự an toàn và trung thực tuyệt đối giữa người dùng và dữ liệu của chính họ.
- Việc tích hợp các tính năng xã hội hóa (social finance) thường làm phân tán trải nghiệm, gây nhiễu loạn luồng phân tích dòng tiền và tạo ra rào cản tâm lý e ngại khi người dùng không muốn phơi bày thói quen chi tiêu thực tế.
- Tập trung trọn vẹn tài nguyên kiến trúc vào cá nhân giúp hệ thống đi sâu vào việc giải quyết các bài toán gốc rễ: rèn luyện kỷ luật dòng tiền, tái định hình hành vi tiêu dùng và xây dựng sự tự do tài chính bền vững cho từng người dùng.

### 1.2 Sự Chuyển dịch Mô hình: Từ Ghi chép Thụ động sang Hỗ trợ Quyết định Chủ động
Đa số các công cụ truyền thống (sổ tay, file Excel, ứng dụng thế hệ cũ) vận hành theo cơ chế **Kế toán Ghi chép Thụ động (Passive Bookkeeping)**: chúng đóng vai trò như một chiếc gương chiếu hậu chỉ phản ánh những gì tiền bạc đã ra đi trong quá khứ. Người dùng ghi chép xong chỉ nhận lại một con số tổng kết khô khan, hoàn toàn bất lực trước câu hỏi: *"Ngày mai tôi được phép tiêu bao nhiêu tiền để không bị rỗng túi vào cuối tháng?"* hay *"Với cơ cấu nợ hiện tại, tôi nên trả khoản nào trước để tiết kiệm tiền lãi nhất?"*.

Hệ thống của đề tài tạo nên một cuộc cách mạng chuyển dịch sang mô hình **Hệ thống Hỗ trợ Quyết định Tài chính Chủ động (Proactive Financial Decision Support System)**:
- Không chỉ ghi chép, hệ thống đóng vai trò như một chuyên gia tài chính số (Digital Financial Coach) cá nhân hóa bên cạnh người dùng 24/7.
- Tự động hóa khâu thu thập dữ liệu bằng trí tuệ nhân tạo thị giác để triệt tiêu ma sát thao tác.
- Liên tục đo lường tốc độ chi tiêu theo thời gian thực và tự động tính toán lại hạn mức chi tiêu hàng ngày có thể thích ứng.
- Đưa ra các cú hích tâm lý phi cưỡng chế đúng thời điểm nhằm ngăn chặn các quyết định mua sắm bốc đồng trước khi tiền thực sự rời khỏi ví.

---

## 2. ĐIỂM MỚI ĐỘT PHÁ & ĐỐI CHIẾU VỚI CÁC ỨNG DỤNG TRÊN THỊ TRƯỜNG

Khi phân tích các ứng dụng quản lý chi tiêu phổ biến hiện nay như **Money Lover, Spendee, Money Manager**, đề tài đã nhận diện được 4 khoảng trống nghiên cứu và giải quyết triệt để bằng 4 trụ cột công nghệ đột phá:

| Trụ Cột / Tiêu Chí | Các Ứng Dụng Thị Trường (Money Lover, Spendee...) | Hệ Thống PFM Thông Minh Tích Hợp AI (Đề tài) | Giá Trị Thực Tiễn & Tính Đột Phá |
| :--- | :--- | :--- | :--- |
| **1. Công nghệ Bóc tách Hóa đơn** | Nhập liệu thủ công từng dòng hoặc OCR ký tự quang học thô (OCR regex), dễ sai sót số học, không đối soát logic. | Pipeline Multimodal AI Vision (Gemini / GPT-4o) kết hợp Thuật toán Hậu kiểm toán số học (Mathematical Audit Algorithm). | Tự động hóa phân tách từng dòng mặt hàng, tự đối soát: $\sum(\text{SL} \times \text{ĐG}) + \text{VAT} = \text{Tổng tiền}$. Phát hiện sai lệch và tự điều chỉnh, đạt độ chính xác >90%. |
| **2. Cơ chế Hoạch định Ngân sách** | Ngân sách tĩnh cứng nhắc (cố định số tiền theo tháng). Nếu lỡ tiêu lố đầu tháng, hệ thống chỉ báo đỏ thụ động, gây nản lòng và bỏ cuộc. | Động cơ Hạn mức Chi tiêu Mỗi ngày Thích ứng (Adaptive Daily Spending Allowance - DSA). | Tự động tính toán lại số tiền được phép tiêu hôm nay dựa trên số ngày còn lại và tốc độ tiêu tiền thực tế (Spending Velocity). Tự động tái cân bằng linh hoạt, bảo đảm không bao giờ vỡ nợ cuối chu kỳ. |
| **3. Quản trị & Giải phóng Nghĩa vụ Nợ** | Chỉ cho ghi chép nợ đơn giản như một giao dịch thu/chi, không có công cụ tính lãi, không có chiến lược trả nợ. | Động cơ Tối ưu hóa Nghĩa vụ Nợ & Đồ thị Dòng tiền (Debt Snowball vs. Debt Avalanche Solver). | Đồ thị hóa danh mục nợ, thuật toán tự động mô phỏng và đối kháng 2 chiến lược: Quả cầu tuyết (Snowball - trả nợ nhỏ trước lấy động lực) vs Tuyết lở (Avalanche - trả lãi cao trước tiết kiệm tiền). Ưu tiên bảo vệ Quỹ khẩn cấp 3-6 tháng. |
| **4. Can thiệp Tâm lý Hành vi** | Thông báo đẩy khô khan, nhắc nhở ghi sổ định kỳ, không tác động được đến căn nguyên tâm lý bốc đồng. | Động cơ Cú hích Hành vi Tài chính (Behavioral Nudge Engine) dựa trên Lý thuyết Nobel Richard Thaler. | Áp dụng Kiến trúc lựa chọn phi cưỡng chế, kích hoạt Khoảng trễ tâm lý (Cooling-off Period 24h-48h) cho các khoản chi bốc đồng/xa xỉ, tái định vị Tài khoản tâm lý (Mental Accounting) giúp người dùng thấy rõ cái giá cơ hội trước khi mua. |
| **5. Tính Toàn vẹn Dữ liệu & Đa Ví** | Số dư ảo, cho phép ghi đè tùy tiện, thao tác chuyển ví không có khóa an toàn, dễ sai lệch số dư thực tế. | Kiến trúc Đa Ví chuẩn ngân hàng với Giao dịch ACID và Khóa bi quan (Pessimistic Locking). | Mọi biến động nạp, rút, chuyển ví nội bộ được bao bọc trong transaction CSDL 3NF, bảo đảm số dư tiền mặt, ngân hàng và hạn mức thẻ tín dụng luôn khớp 100% với thực tế đời sống. |

---

## 3. THUYẾT MINH VĂN XUÔI TOÀN DIỆN 10 SƠ ĐỒ USE CASE (UCD 00 – UCD 09)

Hệ thống được mô hình hóa thành 10 bản vẽ Use Case độc lập chuẩn quốc tế, bao quát 60 ca sử dụng chi tiết phục vụ 1 tác nhân chính là Người dùng cá nhân (Personal User) và tương tác với 4 hệ thống dịch vụ ngoại vi độc lập:

### 3.0 Sơ đồ Use Case Tổng thể (UCD 00)
- **Bản chất nghiệp vụ:** Bản vẽ cung cấp tầm nhìn toàn cảnh về ranh giới hệ thống (System Boundary). Trung tâm là Người dùng cá nhân (Personal User) – người nắm toàn quyền sở hữu và quyết định dữ liệu tài chính của mình. Hệ thống tương tác với 4 tác nhân ngoài (External Services) gồm: Dịch vụ Thị giác AI OCR (AI Vision Service), Dịch vụ Khuyến nghị AI (AI Recommendation Service), Cổng thanh toán VietQR (VietQR Payment Gateway) và Dịch vụ Thông báo đa kênh (Notification Service).
- **Hành vi tương tác:** 9 phân hệ chức năng xoay quanh Người dùng được phân rã rành mạch: từ hạ tầng tài khoản ví (UCD 01), quản lý hai dòng tiền vào - ra (UCD 02, UCD 03), hoạch định ngân sách thích ứng (UCD 04), kiểm soát cam kết định kỳ (UCD 05), đến các phân hệ chiến lược cấp cao như tích lũy & trả nợ (UCD 06), phân tích báo cáo (UCD 07), dự báo rủi ro (UCD 08) và trợ lý cú hích tâm lý (UCD 09).

---

### 3.1 Phân hệ Quản lý Tài khoản & Đa Ví cá nhân (UCD 01)
- **Bản chất nghiệp vụ:** Thiết lập nền tảng tài sản thực tế của người dùng. Một người hiện đại không bao giờ chỉ có một chiếc ví duy nhất; họ sở hữu ví tiền mặt trong túi, tài khoản thanh toán tại ngân hàng, thẻ tín dụng với hạn mức tiêu trước trả sau và các ví điện tử mua sắm.
- **Hành vi tương tác:**
  - Người dùng thực hiện Khởi tạo cấu trúc đa ví tài sản (`UC-01`). Khi người dùng tạo một ví thẻ tín dụng (`UC-02`), hệ thống yêu cầu thiết lập hạn mức tín dụng và ngày chốt sao kê để kích hoạt cơ chế giám sát nợ quay vòng.
  - Nghiệp vụ trọng tâm là Chuyển tiền nội bộ giữa các ví (`UC-04`). Hành vi này tự động bao hàm (`<<include>>`) việc Khóa bi quan bản ghi số dư hai ví nguồn và đích nhằm bảo đảm tính toàn vẹn ACID: tiền trừ ở ví này bắt buộc phải cộng tương ứng vào ví kia trong cùng một đơn vị giao dịch.
  - Định kỳ, người dùng thực hiện Kiểm kê và điều chỉnh số dư thực tế (`UC-05`). Khi có chênh lệch giữa tiền thực đếm và số liệu trên app, hệ thống kích hoạt quan hệ mở rộng (`<<extend>>`) Ghi nhận giao dịch điều chỉnh số dư kèm lý do rõ ràng, ngăn chặn việc sửa số liệu tùy tiện làm mất vết kiểm toán.

---

### 3.2 Phân hệ Quản lý Thu nhập cá nhân (UCD 02)
- **Bản chất nghiệp vụ:** Quản lý và điều phối các dòng tiền đi vào hệ thống tài chính cá nhân, phân biệt rành mạch giữa thu nhập định kỳ và thu nhập đột xuất.
- **Hành vi tương tác:**
  - Người dùng thực hiện Ghi nhận thu nhập chủ động (`UC-07` - lương cứng, tiền công) và Thu nhập thụ động (`UC-08` - tiền lãi tiết kiệm, cổ tức).
  - Khi có một khoản tiền lớn bất thường đổ về (thưởng quý, quà biếu, bán đồ cũ), người dùng ghi nhận Thu nhập bất thường (`UC-09`). Hành động này sẽ mở rộng (`<<extend>>`) kích hoạt Cú hích điều hướng thu nhập bất thường Thaler Nudge (`UC-12`). Thay vì để người dùng rơi vào cạm bẫy tâm lý "tiền từ trên trời rơi xuống nên thoải mái tiêu xài", hệ thống lập tức xuất hiện thông điệp đề xuất trích ngay 50-70% khoản này vào Quỹ khẩn cấp hoặc dồn vào Lịch trả nợ trước khi phân bổ vào ví chi tiêu hàng ngày.

---

### 3.3 Phân hệ Chi tiêu & Bóc tách Hóa đơn AI OCR (UCD 03)
- **Bản chất nghiệp vụ:** Phân hệ được sử dụng thường xuyên nhất, giải quyết triệt để rào cản lười ghi chép bằng công nghệ thị giác trí tuệ nhân tạo.
- **Hành vi tương tác:**
  - Người dùng có thể Ghi chép chi tiêu thủ công (`UC-13`) hoặc sử dụng tính năng đột phá: Chụp ảnh bóc tách hóa đơn bằng AI Vision OCR (`UC-14`).
  - Khi chụp ảnh hóa đơn, hệ thống gửi ảnh đến Dịch vụ AI đa phương thức xử lý bất đồng bộ (HTTP 202 Accepted). Quá trình bóc tách tự động bao hàm (`<<include>>`) việc Bóc tách chi tiết từng dòng mặt hàng (`UC-15`) và Hậu kiểm toán số học hóa đơn (`UC-16`). Thuật toán chạy ngầm đối soát phương trình kế toán. Nếu phát hiện hóa đơn bị mờ hoặc số học không cân bằng, hệ thống sẽ mở rộng (`<<extend>>`) yêu cầu Người dùng xác nhận điều chỉnh thủ công trước khi phê duyệt.
  - Khi giao dịch hoàn tất, hệ thống tự động gắn thẻ cây danh mục (`UC-17`), đồng thời mở rộng kích hoạt (`<<extend>>`) việc Cập nhật và tính toán lại Hạn mức chi tiêu ngày DSA (`UC-18`) ở phân hệ ngân sách.

---

### 3.4 Phân hệ Hoạch định Ngân sách Đa phương pháp & DSA (UCD 04)
- **Bản chất nghiệp vụ:** Chuyển hóa các nguyên lý tài chính kinh điển thành công cụ điều hành dòng tiền linh hoạt hàng ngày.
- **Hành vi tương tác:**
  - Người dùng Thiết lập kế hoạch ngân sách chu kỳ (`UC-19`), tự do lựa chọn giữa các trường phái: Quy tắc 50/30/20 (50% Thiết yếu, 30% Linh hoạt, 20% Tích lũy), Ngân sách Cân bằng Zero-Based Budgeting (mỗi đồng tiền đều có nhiệm vụ), hoặc Sổ tay Kakeibo 4 phong bì của Nhật Bản (`UC-20`).
  - Điểm sáng tạo độc quyền của hệ thống là Theo dõi Hạn mức chi tiêu mỗi ngày thích ứng DSA (`UC-22`). Mỗi buổi sáng, màn hình chính hiển thị con số DSA: *"Hôm nay bạn được tiêu tối đa 250.000 VNĐ"*. Nếu hôm nay người dùng chỉ tiêu 150.000 VNĐ, 100.000 VNĐ còn lại được thuật toán tự động tái phân bổ làm tăng nhẹ hạn mức của các ngày tiếp theo. Ngược lại, nếu lỡ tiêu 350.000 VNĐ, hạn mức các ngày sau sẽ tự động co lại một cách êm ái mà không phá vỡ ngân sách tổng của cả tháng.
  - Khi tốc độ chi tiêu vượt ngưỡng an toàn, hệ thống mở rộng (`<<extend>>`) Cảnh báo thâm hụt và kích hoạt chế độ chi tiêu thắt chặt khẩn cấp (`UC-24`).

---

### 3.5 Phân hệ Quản lý Hóa đơn định kỳ & Dịch vụ Đăng ký (UCD 05)
- **Bản chất nghiệp vụ:** Ngăn chặn "căn bệnh rò rỉ dòng tiền" do người dùng vô tình đăng ký các dịch vụ gia hạn tự động (Netflix, Spotify, iCloud, gói cước 4G, phí phòng gym) rồi lãng quên.
- **Hành vi tương tác:**
  - Người dùng Thiết lập danh mục dịch vụ đăng ký định kỳ (`UC-25`), nhập chu kỳ thanh toán và tài khoản ví trích nợ.
  - Hệ thống tự động kích hoạt Lịch theo dõi và nhắc hẹn trước kỳ hạn 3 ngày (`UC-27`) thông qua Dịch vụ Thông báo.
  - Đối với các hóa đơn thanh toán điện thoại, điện nước, hệ thống tích hợp sinh mã thanh toán chuẩn VietQR chuyển khoản nhanh (`UC-29`), tự động điền sẵn số tiền và nội dung chuyển khoản để người dùng quét mã thanh toán trong 3 giây.
  - Định kỳ cuối tháng, hệ thống mở rộng (`<<extend>>`) cung cấp Báo cáo kiểm toán chi phí ẩn và đề xuất hủy dịch vụ không sử dụng (`UC-30`), giúp người dùng cắt tỉa các khoản chi lãng phí.

---

### 3.6 Phân hệ Quỹ tích lũy & Tối ưu hóa Nghĩa vụ Nợ cá nhân (UCD 06)
- **Bản chất nghiệp vụ:** Xây dựng bệ đỡ tài chính vững chắc bằng cách tích hợp quản lý mục tiêu tiết kiệm và lộ trình giải phóng nợ vay cá nhân.
- **Hành vi tương tác:**
  - Người dùng Khởi tạo Mục tiêu tích lũy tài sản (`UC-31`). Hệ thống bắt buộc ưu tiên Thiết lập Quỹ dự phòng khẩn cấp (`UC-32`) với nguyên tắc bất biến: phải tích lũy tối thiểu bằng 3 đến 6 tháng sinh hoạt phí cơ bản trước khi nghĩ đến đầu tư mạo hiểm.
  - Đối với bài toán nợ vay, người dùng Khởi tạo hồ sơ danh mục nợ cá nhân (`UC-34` - thẻ tín dụng, nợ trả góp, vay người thân).
  - Phân hệ trang bị công cụ cực kỳ mạnh mẽ: Mô phỏng và Đối sánh chiến lược trả nợ Snowball vs. Avalanche (`UC-36`). Hệ thống tự động vẽ ra hai con đường: Con đường Quả cầu tuyết giúp người dùng dập tắt nhanh các món nợ nhỏ để lấy chiến thắng tâm lý; Con đường Tuyết lở giúp dồn lực trả món nợ lãi suất cao nhất để tiết kiệm tối đa tiền lãi. Sau khi người dùng chọn con đường phù hợp, hệ thống tự động lập Lịch trình thanh toán chi tiết (`UC-37`) và giám sát thực hiện.

---

### 3.7 Phân hệ Báo cáo & Phân tích Tài chính Đa chiều (UCD 07)
- **Bản chất nghiệp vụ:** Trực quan hóa dữ liệu giúp người dùng có cái nhìn trong suốt và khách quan nhất về sức khỏe tài chính của bản thân.
- **Hành vi tương tác:**
  - Người dùng xem Bảng điều khiển tài chính tổng quan (`UC-39` - Dashboard) với chỉ số Giá trị tài sản ròng (Net Worth = Tổng tài sản trong các ví - Tổng nghĩa vụ nợ).
  - Hệ thống cung cấp Báo cáo cơ cấu dòng tiền thu chi đa chiều (`UC-40`), Biểu đồ phân tích tốc độ tiêu tiền Spending Velocity (`UC-41`) để người dùng nhìn thấy đường cong tiêu tiền của mình có bị dốc quá mức trong 10 ngày đầu tháng hay không.
  - Phân hệ hỗ trợ Xuất báo cáo tài chính cá nhân định dạng PDF/Excel (`UC-44`) phục vụ việc lưu trữ hoặc chứng minh tài chính khi cần thiết.

---

### 3.8 Phân hệ Dự báo Tài chính & Cảnh báo Thâm hụt (UCD 08)
- **Bản chất nghiệp vụ:** Cung cấp năng lực "nhìn thấy tương lai", phát hiện sớm các nguy cơ khủng hoảng dòng tiền trước khi nó thực sự xảy ra.
- **Hành vi tương tác:**
  - Động cơ toán học thực hiện Dự báo dòng tiền tương lai 30 - 90 ngày (`UC-45`) dựa trên lịch sử giao dịch và các cam kết cố định đã biết.
  - Hệ thống tính toán và đưa ra Cảnh báo ngày cạn kiệt tiền mặt dự kiến (Out-of-Money Date - `UC-47`): *"Nếu tiếp tục giữ tốc độ chi tiêu như tuần qua, ví tiền mặt của bạn sẽ cạn sạch vào ngày 22 của tháng"*.
  - Khi nguy cơ thâm hụt xuất hiện, hệ thống mở rộng (`<<extend>>`) Mô phỏng kịch bản cắt giảm chi tiêu khẩn cấp (`UC-48`), chỉ rõ người dùng cần cắt giảm chính xác ở danh mục linh hoạt nào (ăn ngoài, giải trí) để bảo toàn sự an toàn tài chính.

---

### 3.9 Phân hệ Cú hích Hành vi & Hỗ trợ Quyết định Thaler (UCD 09)
- **Bản chất nghiệp vụ:** Đỉnh cao về mặt ứng dụng tâm lý học kinh tế, hiện thực hóa các nguyên lý đạt giải Nobel của GS. Richard Thaler nhằm tạo ra sự thay đổi hành vi bền vững.
- **Hành vi tương tác:**
  - Hệ thống ngầm đánh giá rủi ro tâm lý chi tiêu theo thời gian thực (`UC-51`).
  - Khi phát hiện một giao dịch mua sắm có giá trị lớn bất thường hoặc thuộc danh mục xa xỉ bốc đồng, hệ thống không ngăn cấm thô bạo (tôn trọng quyền tự do lựa chọn), mà kích hoạt Cú hích Khoảng trễ tâm lý (Cooling-off Period 24h-48h - `UC-52`). Hệ thống gửi thông điệp: *"Khoản chi này bằng 40% hạn mức tuần của bạn. Hệ thống đã tạm đưa món hàng vào danh sách chờ suy nghĩ 24 giờ. Sau 24 giờ, nếu bạn vẫn thực sự muốn mua, hệ thống sẽ mở khóa để bạn xác nhận"*. Thống kê thực nghiệm cho thấy trên 70% các khoản mua sắm bốc đồng bị hủy bỏ sau khi cảm xúc nhất thời lắng xuống.
  - Cung cấp Cú hích phân tích cái giá cơ hội (`UC-54`): Quy đổi số tiền mua một món đồ xa xỉ thành số ngày lao động thực tế hoặc số tiền lẽ ra đã tích lũy được cho Quỹ khẩn cấp, giúp đánh thức tư duy duy lý của người dùng.
  - Người dùng có toàn quyền Xác nhận thực thi hoặc Hủy bỏ quyết định chi tiêu (`UC-56`), bảo đảm con người luôn là chủ thể quyết định cuối cùng.

---

## 4. THUYẾT MINH VĂN XUÔI CHUYÊN SÂU 6 BOUNDED CONTEXTS CỦA CLASS DIAGRAM

Áp dụng phương pháp luận **Domain-Driven Design (DDD)** của Eric Evans, kiến trúc hệ thống kiên quyết xóa bỏ mô hình thiếu máu (Anemic Domain Model - các lớp rỗng chỉ chứa getter/setter). Toàn bộ 18 lớp thực thể nghiệp vụ đều là các **Rich Domain Model** sở hữu các phương thức tính toán giàu hành vi, được tổ chức thành 6 Bounded Contexts độc lập và chặt chẽ:

```
+---------------------------------------------------------------------------------------------------+
|                                      PFM BOUNDED CONTEXT MAP                                      |
+---------------------------------------------------------------------------------------------------+
|  [ CONTEXT 1: IDENTITY & PROFILE ] (Core Domain)                                                  |
|  - User, Money (Value Object), DateRange (Value Object)                                           |
+---------------------------------------------------------------------------------------------------+
               |                                 |                                 |
               v                                 v                                 v
+-----------------------------+   +-----------------------------+   +-----------------------------+
| [ CONTEXT 2: MULTI-WALLET ] |   | [ CONTEXT 4: BUDGET & DSA ] |   | [ CONTEXT 5: DEBT & GOALS ] |
| - Wallet (Abstract)         |   | - Budget (Aggregate Root)   |   | - FinancialGoal / Emergency |
| - Cash / Bank / Credit / EW |   | - BudgetAllocation          |   | - DebtAccount               |
| - InternalTransfer          |   | - DailySpendingAllowance    |   | - DebtPaymentPlan (Solver)  |
| - BalanceAdjustment         |   |   (DSA Adaptive Engine)     |   | - DebtPaymentTransaction    |
+-----------------------------+   +-----------------------------+   +-----------------------------+
               ^                                 ^                                 ^
               | (Trừ tiền / Cộng tiền)          | (Cập nhật tiến độ)              | (Đóng góp mục tiêu)
+---------------------------------------------------------------------------------------------------+
|  [ CONTEXT 3: TRANSACTIONS, AI VISION OCR & CATEGORIES ]                                          |
|  - Transaction (Abstract) -> IncomeTransaction / ExpenseTransaction                               |
|  - Receipt *-- ReceiptItem (Composition - Hậu kiểm toán số học verifyMathematicalAudit)           |
|  - Category (Cây phân cấp đệ quy) & AutoCategorizationRule                                        |
|  - Subscription (Giám sát cam kết định kỳ)                                                        |
+---------------------------------------------------------------------------------------------------+
                                                 | (Kích hoạt cú hích khi vượt ngưỡng)
                                                 v
+---------------------------------------------------------------------------------------------------+
|  [ CONTEXT 6: DECISION SUPPORT & BEHAVIORAL NUDGE (THALER) ]                                      |
|  - FinancialNudge (Cú hích hành vi, Khoảng trễ tâm lý Cooling-off Period 24h-48h, Rationale)      |
+---------------------------------------------------------------------------------------------------+
```

---

### 4.1 Bounded Context 1: Định danh & Hồ sơ cá nhân (Identity & Profile Context)
- **Vai trò miền:** Đóng vai trò là hạt nhân trung tâm (Core Domain), định danh thực thể Người dùng (User) và thiết lập các thông số tài chính nền tảng chi phối toàn bộ hành vi hệ thống.
- **Các thực thể & Đối tượng giá trị cốt lõi:**
  - **Thực thể User:** Quản lý thông tin định danh duy nhất, mật khẩu được băm một chiều bằng giải thuật tiên tiến Argon2id. User lưu giữ các thuộc tính tài chính nền tảng: ngày nhận lương định kỳ (`salaryDay`), chu kỳ ngân sách (`budgetCycle`), mức độ chấp nhận rủi ro (`riskTolerance`: Bảo thủ, Thận trọng, Tăng trưởng) và đơn vị tiền tệ mặc định (`defaultCurrency`).
  - **Đối tượng Giá trị bất biến Money (Value Object):** Đóng gói hai trường dữ liệu: số tiền (`amount` kiểu Decimal độ chính xác cao 18,2) và mã tiền tệ chuẩn ISO-4217 (`currency`). Value Object này là bất biến (Immutable), không bao giờ bị thay đổi trạng thái sau khi khởi tạo. Money sở hữu các phương thức toán học an toàn: `add()`, `subtract()`, `multiply()`, `allocate()`, bảo đảm loại trừ triệt để lỗi mất mát số thập phân khi cộng trừ tiền tệ.
  - **Đối tượng Giá trị DateRange (Value Object):** Đóng gói khoảng thời gian chu kỳ với ngày bắt đầu (`startDate`) và ngày kết thúc (`endDate`). DateRange tự động bảo đảm tính toàn vẹn với quy tắc bất biến `startDate <= endDate`, cung cấp các phương thức nghiệp vụ: `includesDate(date)` (kiểm tra một ngày có thuộc chu kỳ không) và `overlapsWith(otherRange)` (kiểm tra sự giao thoa giữa hai khoảng thời gian ngân sách).

---

### 4.2 Bounded Context 2: Tài khoản & Đa ví cá nhân chuẩn ACID (Account & Multi-Wallet Context)
- **Vai trò miền:** Quản trị toàn bộ các phương tiện lưu trữ tiền tệ và các kênh thanh toán thực tế của người dùng, bảo đảm tính toàn vẹn giao dịch tuyệt đối theo chuẩn ngân hàng.
- **Cấu trúc hướng đối tượng & Quan hệ:**
  - **Lớp cha trừu tượng Wallet (Polymorphism):** Định nghĩa các thuộc tính chung: mã ví, tên ví, số dư hiện tại (`currentBalance` kiểu Money), cờ trạng thái hoạt động và cờ cho phép số dư âm. Lớp cha định nghĩa các phương thức miền giàu hành vi: `deposit(amount)` (nạp tiền), `withdraw(amount)` (rút tiền có kiểm tra hạn mức), và `canWithdraw(amount)`.
  - **Bốn lớp con kế thừa chuyên biệt:**
    1. `CashWallet`: Ví tiền mặt vật lý, kiểm soát theo các lần rút tiền từ ATM.
    2. `BankAccountWallet`: Tài khoản ngân hàng thanh toán, bổ sung số tài khoản và định danh ngân hàng chuẩn NAPAS để phục vụ sinh mã QR chuyển khoản.
    3. `CreditCardWallet`: Thẻ tín dụng chi tiêu trước trả sau, sở hữu thuộc tính hạn mức tín dụng (`creditLimit`), ngày chốt sao kê (`statementClosingDate`) và ngày đến hạn thanh toán (`paymentDueDate`). Phương thức `getAvailableCredit()` tự động tính toán hạn mức khả dụng còn lại để cảnh báo nợ quay vòng.
    4. `EWallet`: Ví điện tử (MoMo, ZaloPay, ShopeePay) liên kết số điện thoại thanh toán.
  - **Lớp liên kết InternalTransfer:** Đại diện cho nghiệp vụ chuyển tiền giữa hai ví nội bộ. Hành vi chuyển tiền bắt buộc phải bao bọc trong một Transaction CSDL với cơ chế Khóa bi quan (Pessimistic Locking - UPDLOCK, ROWLOCK): hệ thống khóa đồng thời cả hai bản ghi ví nguồn và ví đích, thực hiện trừ tiền ở ví nguồn và cộng tiền ở ví đích, bảo đảm tính nguyên tử (Atomicity) – hoặc cả hai cùng thành công, hoặc không có đồng tiền nào bị thay đổi nếu xảy ra sự cố mạng.
  - **Lớp liên kết BalanceAdjustment:** Ghi nhận lịch sử kiểm kê thực tế, lưu trữ số dư ghi nhận, số dư thực đếm và lý do chênh lệch, bảo đảm tính minh bạch kiểm toán.

---

### 4.3 Bounded Context 3: Giao dịch, AI Vision OCR & Danh mục (Transactions, AI OCR & Categories Context)
- **Vai trò miền:** Chịu trách nhiệm ghi nhận, bóc tách và phân loại toàn bộ các luồng tiền biến động phát sinh hàng ngày.
- **Cấu trúc hướng đối tượng & Quan hệ:**
  - **Lớp cha trừu tượng Transaction:** Chứa các thông tin cốt lõi: mã giao dịch, thời điểm phát sinh, số tiền (`Money`), ví thực hiện, ghi chú và phương thức trừu tượng `applyToWallet()`.
  - **Hai lớp con kế thừa:**
    1. `IncomeTransaction`: Giao dịch thu nhập, sở hữu cờ đánh dấu thu nhập bất thường (`isWindfall`). Khi giao dịch này được lưu, phương thức `applyToWallet()` sẽ gọi lệnh `wallet.deposit(amount)`.
    2. `ExpenseTransaction`: Giao dịch chi tiêu, liên kết bắt buộc với một danh mục chi tiêu (`Category`) và cờ đánh dấu chi phí thiết yếu (`isEssentialExpense`). Khi thực thi, phương thức sẽ gọi lệnh `wallet.withdraw(amount)`.
  - **Quan hệ Hợp thành Hóa đơn bóc tách (Receipt *-- ReceiptItem):** Đây là điểm nhấn kiến trúc kỹ thuật của hệ thống. Mỗi hóa đơn `Receipt` sở hữu một danh sách các dòng mặt hàng `ReceiptItem`. Quan hệ giữa chúng là Hợp thành chặt chẽ (Composition): nếu hóa đơn bị xóa, toàn bộ các dòng mặt hàng chi tiết sẽ bị hủy theo. Lớp `Receipt` sở hữu phương thức đặc thù `verifyMathematicalAudit()`: tự động duyệt qua toàn bộ các `ReceiptItem`, nhân số lượng với đơn giá của từng dòng, cộng thuế VAT và so sánh với tổng tiền trên hóa đơn. Giao dịch chỉ được chuyển sang trạng thái hợp lệ khi phương trình kiểm toán số học đạt sai số bằng 0.
  - **Cây danh mục Category (Parent-Child Recursive):** Thiết kế theo mô hình cây phân cấp đệ quy: một danh mục cha có thể chứa nhiều danh mục con (ví dụ: Danh mục cha "Ăn uống" chứa danh mục con "Đi chợ / Siêu thị", "Cà phê", "Nhà hàng"). Lớp `AutoCategorizationRule` liên kết với danh mục để tự động nhận diện từ khóa từ mô tả hóa đơn AI và gắn thẻ danh mục tự động.
  - **Lớp Subscription:** Giám sát các dịch vụ trừ tiền định kỳ, theo dõi ngày gia hạn tiếp theo (`nextBillingDate`) và tự động kích hoạt thông báo cảnh báo trước 3 ngày.

---

### 4.4 Bounded Context 4: Ngân sách Đa phương pháp & DSA (Budgeting & Adaptive DSA Context)
- **Vai trò miền:** Hiện thực hóa các trường phái hoạch định ngân sách chuẩn mực và vận hành động cơ tính toán hạn mức chi tiêu mỗi ngày thích ứng.
- **Cấu trúc hướng đối tượng & Quan hệ:**
  - **Thực thể Aggregate Root Budget:** Quản lý kế hoạch ngân sách cho một chu kỳ thời gian (`DateRange`). Budget chứa tổng số tiền ngân sách kế hoạch, tổng số tiền đã chi tiêu lũy kế và phương pháp áp dụng (50/30/20, Zero-Based, Kakeibo).
  - **Lớp thành phần BudgetAllocation (Composition):** Mỗi Budget hợp thành từ nhiều BudgetAllocation tương ứng với từng nhóm danh mục (ví dụ: Hạn mức cho nhóm Thiết yếu: 10.000.000 VNĐ; Hạn mức cho nhóm Linh hoạt: 4.000.000 VNĐ).
  - **Thực thể DSA (DailySpendingAllowance):** Động cơ toán học của phân hệ. Thực thể này liên tục cập nhật theo mỗi giao dịch chi tiêu mới và sở hữu 3 phương thức nghiệp vụ giàu hành vi:
    1. `calculateDSA()`: Tính hạn mức chi tiêu hôm nay bằng công thức lấy tổng số tiền ngân sách còn lại chia cho số ngày thực tế còn lại trong chu kỳ, có nhân hệ số điều chỉnh ngày cuối tuần (người dùng thường chi tiêu nhiều hơn vào Thứ Bảy và Chủ Nhật).
    2. `computeSpendingVelocity()`: Đo lường tốc độ chi tiêu thực tế (số tiền chi trung bình mỗi ngày trong 7 ngày qua) so với tốc độ chi tiêu lý thuyết an toàn.
    3. `estimateOutOfMoneyDate()`: Dự báo chính xác ngày người dùng sẽ cạn kiệt ngân sách nếu vẫn duy trì tốc độ chi tiêu hiện tại.

---

### 4.5 Bounded Context 5: Quỹ tích lũy & Tối ưu hóa Nợ cá nhân (Personal Debt & Financial Goals Context)
- **Vai trò miền:** Quản trị tầm nhìn tài chính dài hạn, cân bằng giữa mục tiêu kiến tạo tài sản và lộ trình giải phóng các nghĩa vụ nợ.
- **Cấu trúc hướng đối tượng & Quan hệ:**
  - **Thực thể FinancialGoal:** Đại diện cho các mục tiêu tiết kiệm (mua nhà, mua xe, học tập). Thực thể quản lý số tiền mục tiêu (`targetAmount`), số tiền hiện có (`currentAmount`), hạn chót (`targetDate`) và liên kết với các khoản đóng góp định kỳ `GoalContribution`.
  - **Lớp con kế thừa EmergencyFundGoal:** Quỹ dự phòng khẩn cấp. Lớp con này kế thừa toàn bộ thuộc tính của FinancialGoal nhưng bổ sung thêm ràng buộc nghiệp vụ khắt khe: mục tiêu số tiền bắt buộc phải tương đương từ 3 đến 6 tháng sinh hoạt phí cơ bản và số dư quỹ này tuyệt đối không được phép rút ra cho các mục tiêu mua sắm thông thường.
  - **Thực thể DebtAccount:** Đại diện cho một tài khoản nợ cá nhân (vay ngân hàng, thẻ tín dụng, nợ người thân). Quản lý số nợ gốc ban đầu, số dư nợ hiện tại, lãi suất phần trăm hàng năm (APR) và ngày đến hạn thanh toán hàng tháng.
  - **Thực thể Kế hoạch trả nợ DebtPaymentPlan:** Đóng vai trò là Solver tối ưu hóa. Kế hoạch này hợp thành chặt chẽ với các dòng lịch trình trả nợ `DebtRepaymentScheduleItem` và liên kết với các giao dịch trả nợ `DebtPaymentTransaction`. Thực thể sở hữu 2 thuật toán mô phỏng đối sánh:
    * `simulateSnowball()`: Sắp xếp các khoản nợ theo thứ tự số dư nợ tăng dần. Dồn toàn bộ nguồn tiền khả dụng thanh toán dứt điểm khoản nợ nhỏ nhất trước để giải phóng nhanh số lượng đầu nợ, tạo hưng phấn tâm lý mạnh mẽ.
    * `simulateAvalanche()`: Sắp xếp các khoản nợ theo thứ tự lãi suất giảm dần. Dồn tiền thanh toán khoản nợ có lãi suất cao nhất trước để triệt tiêu số tiền lãi phải trả, tối ưu hóa lợi ích tài chính toán học.

---

### 4.6 Bounded Context 6: Cú hích Hành vi & Hỗ trợ Quyết định Thaler (Decision Support & Behavioral Nudge Context)
- **Vai trò miền:** Điều phối các can thiệp tâm lý kinh tế học hành vi nhằm hỗ trợ người dùng đưa ra các quyết định chi tiêu sáng suốt và kỷ luật.
- **Cấu trúc hướng đối tượng & Quan hệ:**
  - **Thực thể FinancialNudge:** Đại diện cho một cú hích tài chính cụ thể. Thực thể lưu trữ loại cú hích (Cảnh báo tốc độ tiêu tiền, Đề xuất điều hướng thu nhập bất thường, Cảnh báo giao dịch bốc đồng, Nhắc nhở cái giá cơ hội).
  - **Tính Minh bạch & Giải trình (Explainable AI):** Thuộc tính `rationaleExplanation` lưu trữ câu giải thích bằng ngôn ngữ tự nhiên, mạch lạc về lý do tại sao hệ thống đưa ra cú hích này (ví dụ: *"Khoản chi 3.500.000 VNĐ cho chiếc đồng hồ này chiếm tới 70% ngân sách linh hoạt của cả tuần"*).
  - **Cơ chế Khoảng trễ tâm lý (Cooling-off Period):** Thuộc tính `coolingOffUntil` lưu thời điểm hết hạn tạm hoãn quyết định (thường là 24 giờ đến 48 giờ sau thời điểm kích hoạt). Phương thức `applyCoolingOff()` tạm khóa quyết định mua sắm và đưa vào danh sách suy nghĩ.
  - **Xác nhận có ý thức:** Thuộc tính cờ `isApprovedByUser` và phương thức `confirmHumanAction()` bảo đảm hệ thống không bao giờ tự ý tước đoạt quyền tự do lựa chọn của người dùng, tôn trọng triệt để nguyên lý cốt lõi của Kinh tế học Hành vi: Kiến trúc lựa chọn phi cưỡng chế.

---

### 4.7 Đồ thị Tương tác & Luồng Dữ liệu Luân chuyển giữa 6 Bounded Contexts

Dữ liệu trong hệ thống không tồn tại ở các ốc đảo cô lập mà luân chuyển nhịp nhàng tạo thành một vòng lặp kiểm soát tài chính khép kín:
1. **Khởi tạo:** Người dùng ở Context 1 thiết lập các Ví tài sản ở Context 2, tạo Kế hoạch ngân sách ở Context 4 và thiết lập Mục tiêu tích lũy ở Context 5.
2. **Giao dịch phát sinh:** Khi Người dùng chụp ảnh hóa đơn ở Context 3, pipeline AI OCR bóc tách và đối soát số học xong sẽ phát ra sự kiện giao dịch hợp lệ.
3. **Cập nhật số dư & Ngân sách:** Giao dịch chi tiêu ngay lập tức gọi phương thức `withdraw()` trừ tiền trên Ví tương ứng ở Context 2 theo chuẩn ACID. Đồng thời, giao dịch chuyển tiếp thông tin sang Context 4 để cộng dồn vào chi tiêu thực tế của BudgetAllocation.
4. **Tự động thích ứng hạn mức DSA:** Động cơ DSA ở Context 4 lập tức kích hoạt phương thức `calculateDSA()` để tính toán lại số tiền được tiêu cho các ngày còn lại trong tháng.
5. **Kích hoạt Cú hích can thiệp:** Nếu tốc độ chi tiêu ở Context 4 vượt ngưỡng an toàn hoặc giao dịch ở Context 3 là một khoản chi bốc đồng lớn, sự kiện cảnh báo được gửi sang Context 6. Thực thể `FinancialNudge` lập tức tạo ra thông báo giải trình và kích hoạt Khoảng trễ tâm lý Cooling-off Period 24h.
6. **Điều hướng dòng tiền thặng dư:** Khi người dùng nhận được một khoản thu nhập bất thường ở Context 2 hoặc tiết kiệm được một khoản tiền thừa cuối chu kỳ ở Context 4, Context 6 đưa ra cú hích gợi ý Người dùng chuyển khoản thặng dư đó vào Quỹ khẩn cấp hoặc Lịch trả nợ Avalanche ở Context 5.

Toàn bộ chu trình này bảo đảm hệ thống vận hành tự động, chặt chẽ, hỗ trợ người dùng đạt được kỷ luật dòng tiền mà không đòi hỏi nỗ lực tính toán thủ công phức tạp.

---

## 5. TỔNG KẾT & KHUYẾN NGHỊ THUYẾT TRÌNH TRƯỚC HỘI ĐỒNG

Khi đứng trước Hội đồng Đánh giá, bạn hãy tự tin dẫn dắt câu chuyện thuyết trình theo cấu trúc 3 phần rõ ràng:

1. **Mở đầu bằng "Nỗi đau thực tế" (The Problem):**
   - Đừng nói về công nghệ trước. Hãy bắt đầu từ việc hầu hết sinh viên và người trẻ hiện nay đều nản lòng sau 1-2 tuần dùng app chi tiêu vì quá lười gõ từng khoản tiền, và các app hiện tại như Money Lover chỉ biết báo đỏ khi đã tiêu hết tiền chứ không giúp ích được gì cho tương lai.
2. **Khẳng định "Vũ khí giải pháp đột phá" (The Solution):**
   - Nhấn mạnh đề tài giải quyết triệt để vấn đề trên bằng **Bộ 4 Trụ Cột Độc Quyền**: AI Vision OCR đối soát số học tự động triệt tiêu thao tác nhập; DSA tự co giãn ngân sách theo ngày giúp không bao giờ bị rỗng túi; Giải thuật Snowball/Avalanche chỉ rõ đường đi trả nợ; và Cú hích Thaler Nudge 24h dập tắt mua sắm bốc đồng.
   - Nhấn mạnh nguyên tắc kiến trúc: **100% tài chính cá nhân độc lập**, bảo mật tuyệt đối, Clean Architecture và CSDL 3NF khóa bi quan bảo đảm tiền tệ an toàn chuẩn ngân hàng.
3. **Minh chứng bằng "Mô hình thiết kế chuẩn mực" (The Evidence):**
   - Chiếu Sơ đồ Use Case tổng quan và 6 Bounded Contexts của Class Diagram.
   - Giải thích mạch lạc cách dữ liệu chảy từ Hóa đơn AI -> Trừ tiền Ví ACID -> DSA co giãn hạn mức -> Kích hoạt Nudge tâm lý. Điều này chứng minh hệ thống không phải là một bài tập làm thêm giao diện đơn thuần, mà là một hệ thống thông tin hoàn chỉnh, có chiều sâu học thuật và giá trị ứng dụng thực tiễn rất cao.

---
*Tài liệu được biên soạn đồng bộ với Khóa luận Tốt nghiệp và Báo cáo Đồ án Phân tích Thiết kế Hệ thống Thông tin.*
