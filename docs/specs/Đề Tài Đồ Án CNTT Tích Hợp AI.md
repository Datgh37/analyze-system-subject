# Đề Tài Đồ Án CNTT Tích Hợp AI

_User prompt: Có thể đề xuất các đề tài ít vĩ mô hơn được không?_

Response: Dưới đây là 4 đề tài được thu gọn quy mô, tập trung trực tiếp vào các bài toán thực tế vừa sức cho đồ án cử nhân nhưng vẫn đảm bảo độ sâu về thiết kế hệ thống (SAD), kiến trúc Backend và tích hợp AI.

**1\. Hệ thống Quản lý Chi tiêu & Lập Ngân sách Cá nhân / Nhóm (Smart Expense & Budget Manager)**

- **Bài toán thực tế:** Xử lý bài toán ghi chép tài chính, chia tiền nhóm và quản lý hạn mức ngân sách gia đình/nhóm bạn.
- **Phân tích nghiệp vụ (BA):** Luồng chia tiền (split bill) theo tỷ lệ/phần trăm, quy trình duyệt giao dịch nhóm, cảnh báo vượt hạn mức ngân sách (Budget Alert) và báo cáo chu kỳ.
- **Backend & Kiến trúc (BE/SE):** Thiết kế CSDL quan hệ tối ưu cho lịch sử giao dịch; xử lý bài toán đồng bộ hóa dữ liệu khi nhiều người cùng nhập chi tiêu; áp dụng Caching cho báo cáo tài chính hàng tháng.
- **Ứng dụng AI:** Nhúng module OCR để tự động đọc hóa đơn/bản chụp chuyển khoản (bóc tách số tiền, ngày, đơn vị) và tự động gắn nhãn phân loại danh mục chi tiêu (Auto-categorization).
- **DevOps:** Đóng gói ứng dụng backend và database qua Docker, triển khai qua Cloudflare Tunnel để demo live trên thiết bị di động/web.

…

_User prompt: thôi chốt đề tài 1, nhưng có 1 vấn đề, bạn cũng biết là hiện có nhiều app cũng hỗ trợ quản lý chi tiêu, tôi cần có gì đó mới mẻ để thêm vào trong dự án, bạn thấy sao?_

Response: Điểm yếu lớn nhất của các app chi tiêu thông thường là bắt người dùng "nhập tay" quá nhiều và chỉ dừng lại ở mức thống kê quá khứ. Để đồ án cử nhân nổi bật trước hội đồng, hệ thống của bạn cần chuyển dịch từ **App Ghi Chép** thành **Hệ thống AI Tự động hóa & Dự báo Tài chính** với các điểm nhấn công nghệ backend sau:

**1\. Thuật toán Tối ưu hóa Nợ nhóm (Debt Simplification Graph Engine)**

- **Điểm mới:** Xử lý bài toán chia tiền nhóm (Splitwise model) nhưng tích hợp **Lý thuyết Đồ thị (Graph Theory / Min-Cost Max-Flow)**. Khi một nhóm đi du lịch có hàng chục giao dịch chéo nhau, thuật toán ở Backend sẽ tự động giản lược đồ thị nợ để số lượng giao dịch chuyển khoản đền bù giữa các thành viên là tối thiểu.
- **Giá trị BE/SE:** Xử lý bài toán thuật toán đồ thị, hỗ trợ đa tiền tệ (Multi-currency) với tỷ giá realtime và giải quyết bài toán Concurrency khi chốt sổ tài chính nhóm.

**2\. AI Agent Bóc tách & Đối soát Hóa đơn Đa luồng (Multi-modal Receipt & Recon Engine)**

- **Điểm mới:** Bỏ qua việc nhập tay. Người dùng chỉ cần gửi ảnh chụp hóa đơn dài (siêu thị, nhà hàng) hoặc ảnh chụp màn hình chuyển khoản (VietQR/Banking).
- **Điểm AI/BE:** Tích hợp OCR/Vision LLM để bóc tách **từng dòng mặt hàng (Itemized parsing)** thay vì chỉ lấy tổng tiền, tự động gán nhãn từng món vào danh mục tương ứng. Backend triển khai hàng chờ (Message Queue) để xử lý bóc tách bất đồng bộ và kiểm tra trùng lặp (Deduplication Check) giữa thông báo ngân hàng và hóa đơn.

**3\. Engine Dự báo Dòng tiền & Cảnh báo "Cháy túi" (Predictive Cashflow & Anomaly)**

- **Điểm mới:** Thay vì đặt hạn mức cố định dạng tĩnh, hệ thống tính toán **Tốc độ chi tiêu (Spending Velocity)**.
- **Điểm AI/BE:** Dựa trên chuỗi thời gian (Time-series data) và lịch sử thanh toán cố định (tiền nhà, hóa đơn điện nước), AI phát tín hiệu cảnh báo sớm: _"Với tốc độ chi tiêu hiện tại, bạn sẽ hết ngân sách vào ngày 21"_ hoặc phát hiện các chi phí ẩn/quên hủy gói dịch vụ định kỳ (Subscription Trap).

**4\. Mô phỏng Kịch bản Tài chính "What-If" (Financial Scenario Simulator)**

- **Điểm mới:** Cho phép người dùng chạy mô phỏng tương tác: _"Nếu tôi giảm 30% chi phí ăn ngoài, sau bao lâu tôi đủ tiền mua Laptop 25 triệu?"_.
- **Điểm BE:** Xây dựng Simulation Engine chạy trên Backend dựa trên chuỗi dữ liệu lịch sử và các biến số đầu vào linh hoạt.

_User prompt: Ok vậy ta chốt đề tài 1 , cho tôi thông tin tên đề tài, overview_

# Response: ĐỀ XUẤT ĐỒ ÁN TỐT NGHIỆP CỬ NHÂN CNTT

## I. Tên Đề Tài

- **Tên tiếng Việt:** Nghiên cứu, thiết kế và xây dựng **Hệ thống Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh (Tích hợp AI)**
- **Tên tiếng Anh:** **Smart Personal Expense and Financial Planning Management System with AI Integration**

## II. Tổng Quan Đề Tài (Project Overview)

### 1\. Bối Cảnh & Lý Do Chọn Đề Tài

Hiện nay, nhu cầu quản lý tài chính cá nhân và tài chính nhóm (sinh viên, người đi làm chung phòng, nhóm du lịch) ngày càng tăng cao. Tuy nhiên, các ứng dụng hiện có trên thị trường gặp 3 điểm nghẽn lớn:

1. **Phụ thuộc quá nhiều vào nhập liệu thủ công:** Người dùng phải tự nhập từng giao dịch, dẫn đến việc bỏ dở ứng dụng sau một thời gian ngắn.
2. **Quản lý nợ nhóm chưa tối ưu:** Khi một nhóm có hàng chục giao dịch chéo nhau, việc thanh toán hoàn tiền trở nên rối rắm do thiếu thuật toán giản lược nợ.
3. **Chỉ dừng lại ở mức thống kê quá khứ:** Các ứng dụng thường chỉ vẽ biểu đồ tròn/cột cho dữ liệu đã xảy ra, thiếu khả năng dự báo dòng tiền tương lai để đưa ra cảnh báo sớm.

Đề tài này được lựa chọn nhằm giải quyết triệt để các hạn chế trên bằng cách ứng dụng **Kiến trúc Backend hiện đại**, **Thuật toán Đồ thị** và **Trí tuệ Nhân tạo (AI)** vào lõi nghiệp vụ.

### 2\. Mục Tiêu Đề Tài

- **Về mặt Hệ thống & Nghiệp vụ (BA/SE):** Xây dựng một hệ thống thông tin hoàn chỉnh, đạt chuẩn từ khâu phân tích yêu cầu, mô hình hóa luồng nghiệp vụ, thiết kế kiến trúc phần mềm đến quản lý cơ sở dữ liệu và xử lý đồng thời (Concurrency).
- **Về mặt Tự động hóa & AI:** Tích hợp mô hình Vision LLM / OCR để bóc tách tự động từng mặt hàng trên hóa đơn và dự báo tốc độ chi tiêu (Spending Velocity).
- **Về mặt Thuật toán:** Triển khai **Debt Simplification Graph Engine** để tối ưu hóa số lượng giao dịch hoàn tiền trong nhóm về mức tối thiểu.
- **Về mặt Vận hành (DevOps):** Chuẩn hóa quy trình đóng gói container, CI/CD pipeline, logging tập trung và giám sát hiệu năng hệ thống.

### 3\. Phạm Vi Hệ Thống (System Scope)

#### A. Phân hệ Quản lý Tài chính Cá nhân

- **Quản lý Danh mục & Ngân sách:** Thiết lập hạn mức ngân sách linh hoạt theo chu kỳ (tuần/tháng/dự án).
- **Dự báo Dòng tiền & Cảnh báo Sớm (Cashflow Prediction):** Tính toán tốc độ tiêu dùng thực tế và phát tín hiệu cảnh báo nguy cơ vượt ngân sách trước khi chu kỳ kết thúc.
- **Mô phỏng Kịch bản "What-If":** Chạy mô phỏng cắt giảm chi tiêu để tính toán thời gian đạt mục tiêu tiết kiệm.

#### B. Phân hệ Quản lý Tài chính Nhóm (Group Financial Engine)

- **Chia tiền linh hoạt (Split-Bill):** Chia đều, chia theo tỷ lệ, chia theo món hoặc chia theo số tiền cố định.
- **Tối ưu hóa Luồng Nợ (Debt Simplification):** Ứng dụng lý thuyết đồ thị giản lược chuỗi nợ vòng tròn (*A*→*B*→*C*→*A*) thành các giao dịch bù trừ tối giản.
- **Quy trình Chốt sổ & Đối soát:** Luồng xác nhận chuyển khoản giữa các thành viên, khóa giao dịch kỳ cũ.

#### C. Phân hệ AI & Xử lý Bất đồng bộ (AI & Async Pipeline)

- **Bóc tách Hóa đơn Đa luồng (Multi-modal Receipt Parsing):** Đọc ảnh chụp hóa đơn/ảnh chuyển khoản banking, trích xuất cấu trúc danh mục từng món hàng (Itemized Data) và tự động gán nhãn (Auto-tagging).
- **Duyệt & Kiểm trùng Hóa đơn (Deduplication Check):** Phát hiện hóa đơn bị gửi lặp lại hoặc trùng với thông báo biến động số dư.

### 4\. Định Hướng Kiến Trúc & Công Nghệ Dự Kiến

| **Tầng Hệ Thống**           | **Công Nghệ / Mô Hình Định Hướng**                           | **Vai Trò & Điểm Nhấn**                                                             |
| --------------------------- | ------------------------------------------------------------ | ----------------------------------------------------------------------------------- |
| **Backend Architecture**    | Clean Architecture / DDD, RESTful API                        | Phân tách lớp chặt chẽ, dễ bảo trì và kiểm thử                                      |
| **Database & Cache**        | Relational DB (SQL Server / PostgreSQL), Redis               | ACID cho giao dịch tài chính, Redis Caching & Distributed Lock chống Race Condition |
| **Async Processing**        | Message Queue (RabbitMQ / Redis Streams / Background Worker) | Hàng chờ xử lý OCR/AI bất đồng bộ không gây nghẽn API                               |
| **AI Module**               | Vision LLM / OCR Pipeline                                    | Bóc tách hóa đơn, phân loại dữ liệu, tính toán time-series                          |
| **DevOps & Infrastructure** | Docker, Docker Compose, Nginx, Cloudflare Tunnel             | Containerization, Reverse Proxy, HTTPS, triển khai demo live                        |

### 5\. Điểm Nổi Bật Đạt Tiêu Chẩn Đồ Án Tốt Nghiệp

1. **Tính Thực Tiễn Cao:** Giải quyết đúng nỗi đau nhập liệu và chia tiền nhóm của người dùng trẻ.
2. **Bài Toán Backend Sâu:** Xử lý bất đồng bộ (Async Queue), khóa giao dịch (Pessimistic/Distributed Locking) khi chốt sổ nhóm, thiết kế Database chuẩn hóa 3NF/BCNF hỗ trợ querying linh hoạt.
3. **Giá Trị Thuật Toán:** Triển khai thuật toán đồ thị luồng (Min-Cost Max-Flow / Transitive Reduction) ngay tại Backend.
4. **Tích Hợp AI Đúng Bản Chất:** Nhúng AI vào pipeline xử lý dữ liệu thay vì chỉ gọi API dạng Chatbot bề nổi.

_User prompt: ủa khoan ,cái này nó hơi khác so với cái ban đầu là . Hệ thống Quản lý Chi tiêu & Lập Ngân sách Cá nhân / Nhóm (Smart Expense & Budget Manager) mà nhỉ? Vì quản lý dự báo tài chính nó hơi vĩ mô quá thì phải_

Response: Bạn phán đoán rất chính xác. Dùng từ "Dự báo tài chính" (Financial Forecasting) nghe rất vĩ mô, dễ khiến hội đồng hiểu nhầm sang các mô hình quản trị tài chính doanh nghiệp phức tạp (kiểu phân tích luồng tiền CFO hay đầu tư chứng khoán).

Thực chất ở quy mô **Cá nhân & Nhóm (Micro)**, bản chất của nó chỉ là **Cảnh báo tiến độ tiêu tiền (Budget Alert & Spending Velocity)** dựa trên các phép tính đại số đơn giản. Chúng ta sẽ tinh chỉnh tên đề tài và scope về đúng tinh thần gọn nhẹ, thực tế ban đầu:

**Tên Đề Tài Chuẩn Hóa**

- **Tên tiếng Việt:** Nghiên cứu, thiết kế và xây dựng **Hệ thống Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh (Tích hợp AI)**
- **Tên tiếng Anh:** **Smart Personal Expense and Financial Planning Management System with AI Integration**

**Phạm Vi Hệ Thống Tinh Gọn (Scope chuẩn cho Đồ án)**

**1\. Quản lý Chi tiêu & Ngân sách Cá nhân (Expense & Budget)**

- **Ghi chép & Phân loại:** Quản lý khoản thu/chi theo danh mục (Ăn uống, Tiền nhà, Giải trí,...).
- **Đặt hạn mức ngân sách:** Thiết lập ngân sách cố định cho từng danh mục theo tháng.
- **Cảnh báo chạm ngưỡng (Micro-Alert):** Đưa ra cảnh báo đơn giản dựa trên công thức tốc độ tiêu tiền (_Spending___Rate_\=_So_^ˊ__nga_ˋ_y_\_đ_a_~~_*quaTie*^ˋ_n_\_đ_a_~~__tie_^_u_​×*T_ổ_ng___nga_ˋ_y*). Nếu tốc độ này vượt hạn mức, hệ thống nhắc nhở _"Bạn có nguy cơ vỡ ngân sách vào ngày 20"_.

**2\. Quản lý & Chia tiền Nhóm (Group Split-Bill)**

- **Tạo nhóm & Chia tiền:** Hỗ trợ tạo quỹ nhóm, chia bill đi ăn/du lịch (chia đều, chia theo món, chia theo phần trăm).
- **Rút gọn chuỗi nợ (Debt Simplification):** Ứng dụng thuật toán đồ thị luồng ở Backend để giản lược các khoản nợ chéo nhau trong nhóm về số lượng giao dịch chuyển khoản ít nhất.
- **Chốt sổ & Xác nhận:** Luồng chốt sổ cuối kỳ và ghi vết đối soát chuyển khoản giữa các thành viên.

**3\. Module Tự động hóa bằng AI (AI Receipt Parser)**

- **Scan hóa đơn / Chuyển khoản:** Người dùng tải ảnh hóa đơn siêu thị hoặc ảnh chụp màn hình ngân hàng.
- **Bóc tách dữ liệu:** Module OCR/AI tự đọc các thông số: Số tiền, Ngày giờ, Tên cửa hàng và Danh mục món ăn để điền sẵn vào form, người dùng chỉ cần nhấn "Lưu" mà không phải gõ tay.

**Vì sao Scope này vừa vặn cho Đồ án Cử nhân?**

- **Đúng tầm Backend/SE:** Bạn tập trung giải quyết bài toán CSDL (quan hệ nhóm, hóa đơn, tài khoản), thiết kế API clean, xử lý bóc tách ảnh bất đồng bộ (Queue) và cài đặt thuật toán tối ưu nợ nhóm.
- **Không bị ngợp:** Bỏ hoàn toàn các khái niệm vĩ mô. AI chỉ đóng vai trò là "trợ lý nhập liệu tự động" giúp app thân thiện hơn.

_User prompt: Hãy một lần nữa hóa thân thành các chuyên gia ở trên, hãy vạch lộ trình đi từ đầu đến cuối cho 1 quá trình xây dựng dự án này được không?_

Response: Lộ trình 6 giai đoạn chuẩn hóa theo quy trình kĩ nghệ phần mềm (Software Engineering Lifecycle) để hoàn thành đồ án cử nhân từ khâu ý tưởng đến bảo vệ trước hội đồng:

**1Giai đoạn 1: Phân tích Nghiệp vụ & Chuẩn hóa Yêu cầu (BA)**

Tập trung vào phạm vi, Use Case và tài liệu SRS

- **Xác định Actors & Phạm vi:** Định nghĩa rõ các vai trò trong hệ thống (User cá nhân, Group Owner, Group Member, System Worker).
- **Mô hình hóa Use Case:** Xây dựng danh sách Use Case chi tiết, viết Use Case Specifications cho các luồng phức tạp (Tạo nhóm, Chia bill, Chốt sổ, OCR Hóa đơn).
- **Soạn thảo SRS (IEEE 29148):** Viết tài liệu Phân tích Yêu cầu Phần mềm gồm Yêu cầu chức năng (Functional) và Yêu cầu phi chức năng (Non-functional: độ trễ OCR dưới 3s, xử lý đồng thời, bảo mật JWT/OAuth2).

**2Giai đoạn 2: Thiết kế Kiến trúc & Cơ sở Dữ liệu (SAD / SE)**

Thiết kế Domain Model, ERD 3NF và Thuật toán cốt lõi

- **Kiến trúc Tổng quan:** Áp dụng Clean Architecture / DDD (Domain-Driven Design) chia lớp rõ ràng (Domain -> Application -> Infrastructure -> API).
- **Thiết kế CSDL Quan hệ (ERD):** Chuẩn hóa 3NF/BCNF cho các bảng Users, Expenses, Groups, Group_Members, Debt_Edges, Receipts, Receipt_Items.
- **Mô hình hóa Thuật toán:** Chi tiết hóa **Debt Simplification Graph Engine** (Lý thuyết đồ thị luồng tối ưu chuỗi nợ) và sơ đồ chuyển trạng thái (**State Machine**) khi nhóm chốt sổ tài chính.

**3Giai đoạn 3: Phát triển Backend Cốt lõi & Xử lý Tranh chấp (BE Dev)**

Xây dựng API, Concurrency Control và Graph Engine

- **Xây dựng API Framework:** Viết RESTful API chuẩn mực cho quản lý khoản thu chi, ngân sách cá nhân và quản lý nhóm.
- **Xử lý Đồng thời (Concurrency):** Cài đặt Distributed Lock (với Redis) hoặc Pessimistic Locking để tránh Race Condition khi nhiều thành viên cùng chốt sổ hoặc cập nhật chi tiêu nhóm.
- **Cài đặt Thuật toán Đồ thị:** Hiện thực hóa thuật toán Rút gọn Nợ nhóm tại tầng Application/Domain Service, hỗ trợ bù trừ nợ tự động giữa các thành viên.

**4Giai đoạn 4: Tích hợp Module AI & Hàng chờ Bất đồng bộ (Async AI Pipeline)**

Tự động hóa bóc tách hóa đơn bằng Vision LLM / OCR

- **Cấu hình Hàng chờ (Message Queue):** Xây dựng Background Worker (sử dụng Redis Streams / RabbitMQ) để tiếp nhận và xử lý ảnh hóa đơn bất đồng bộ, tránh làm nghẽn main API thread.
- **Nhúng AI Vision/OCR:** Kết nối API OCR/Vision LLM để bóc tách thông tin cấu trúc (Cửa hàng, Ngày giờ, Tên món, Giá tiền) từ ảnh chụp hóa đơn/màn hình ngân hàng.
- **Kiểm trùng Hóa đơn (Deduplication):** Viết logic đối soát mã hóa đơn/hash ảnh để cảnh báo khi người dùng tải lên hóa đơn bị trùng.

**5Giai đoạn 5: Đóng gói Hạ tầng & Tự động hóa (DevOps)**

Containerization, CI/CD Pipeline và Giám sát hệ thống

- **Container hóa:** Đóng gói ứng dụng Backend, Database, Redis, Background Worker bằng Docker và Docker Compose.
- **CI/CD Pipeline:** Thiết lập GitHub Actions tự động build, chạy Unit Tests và Linting mỗi khi tạo Pull Request/Push code.
- **Triển khai Live Demo:** Triển khai hạ tầng qua Reverse Proxy (Nginx), cấu hình HTTPS và Cloudflare Tunnel để sẵn sàng demo trực tiếp trên di động/web trước Hội đồng.

**6Giai đoạn 6: Kiểm thử, Viết Quyển & Chuẩn bị Bảo vệ (QA & Defense)**

Đo lường chỉ số thực nghiệm và hoàn thiện quyển Báo cáo Đồ án

- **Kiểm thử Hệ thống:** Viết Unit Test cho logic Domain/Thuật toán đồ thị và Integration Test cho các luồng API chính; thực hiện Load Test (K6/JMeter) để lấy số liệu chịu tải.
- **Soạn thảo Báo cáo Đồ án:** Hoàn thiện Quyển Đồ án Tốt nghiệp (khoảng 80 - 120 trang) theo đúng chuẩn mực academic (Sơ đồ PlantUML, ERD, Bảng kết quả thực nghiệm).
- **Chuẩn bị Slide & Demo:** Chuẩn bị kịch bản Demo live mượt mà, nhấn mạnh vào điểm sáng Backend và điểm mới về AI/Thuật toán đồ thị.

_User prompt: Vậy bây giờ nếu tôi làm việc trên Antigravity, tôi muốn xây dựng 1 hội đồng agents nhằm đóng vai các chuyên gia và khách hàng để có thể thu thập ý kiến khách hàng 1 cách giả lập, thì cần phải triển khai như thế nào?_

Response: Mô hình **Hội đồng Agents Giả lập (Simulated Multi-Agent Council)** là phương pháp hiệu quả để kiểm thử ý tưởng, thu thập phản biện đa chiều và phát hiện điểm nghẽn hệ thống trước khi gõ dòng code nào.

Dưới đây là kiến trúc triển khai hệ thống đa Agent (Multi-Agent System) trên môi trường Antigravity.

### 1\. Phân Cấp & Định Danh Đội Ngũ Agent (Agent Roster)

Bạn cần chia hệ thống thành 3 nhóm Agent chính với các System Prompt chứa các mục tiêu (Goals) và quy tắc (Constraints) riêng biệt:

**A. Nhóm Khách Hàng Giả Lập (User Personas)**

- **Agent 1 - Sinh viên lười gõ (Gen Z User):**
  - _Persona:_ Bận rộn, thích nhanh gọn, dị ứng với form nhập liệu dài, chỉ thích chụp ảnh hoặc forward tin nhắn chuyển khoản.
  - _Nhiệm vụ:_ Trải nghiệm luồng UX, phản ánh sự phiền phức khi thao tác, đánh giá tính tiện dụng của tính năng OCR Scan.
- **Agent 2 - Chuyên Gia Hoạch Định Ngân Sách Cá Nhân (Personal Budget Coach):**
  - _Persona:_ Chuyên gia phương pháp 50/30/20, Zero-Based Budgeting, Sổ Kakeibo và 6 Hũ tài chính.
  - _Nhiệm vụ:_ Đặt câu hỏi về kỷ luật tài chính, kiểm tra vượt hạn mức danh mục, hạn mức tiêu dùng mỗi ngày và quỹ khẩn cấp.

**B. Nhóm Chuyên Gia Kỹ Thuật & Nghiệp Vụ (Expert Panel)**

- **Agent 3 - BA Specialist (Phong cách Debra Paul / Angela Wick):**
  - _Nhiệm vụ:_ Soi các ngoại lệ (Edge Cases), làm rõ điều kiện chấp nhận (Acceptance Criteria), yêu cầu chuẩn hóa luồng Use Case.
- **Agent 4 - Software Architect (Phong cách Martin Fowler):**
  - _Nhiệm vụ:_ Đánh giá kiến trúc Clean Architecture, giữ tính đơn giản (KISS/YAGNI), ngăn chặn over-engineering.
- **Agent 5 - Backend & Data Engine Specialist (Phong cách Martin Kleppmann):**
  - _Nhiệm vụ:_ Thiết kế CSDL quan hệ chuẩn 3NF/BCNF cho Đa ví, Giao dịch, Hạn mức ngân sách; đảm bảo ACID khi điều chuyển ví; tối ưu đánh chỉ mục Time-series.
- **Agent 6 - Thuật Toán Tối Ưu Nợ & Dòng Tiền (Phong cách Robert Tarjan & David Ramsey):**
  - _Nhiệm vụ:_ Triển khai giải thuật Debt Snowball vs Debt Avalanche, mô phỏng đồ thị dòng tiền cá nhân và tối ưu hóa thời gian sạch nợ.
- **Agent 6 - DevOps & Security Specialist (Phong cách Patrick Debois):**
  - _Nhiệm vụ:_ Phản biện về bảo mật API, Dockerization, giới hạn tải (Rate Limiting) và chi phí vận hành dịch vụ AI.

**C. Agent Điều Phối (Orchestrator / Facilitator)**

- **Agent 0 - Chủ tịch Hội đồng:**
  - _Nhiệm vụ:_ Điều phối lượt nói, tóm tắt đồng thuận, cắt ngang khi tranh luận đi vào ngõ hẹp, tổng hợp phản hồi thành tài liệu kỹ thuật hoàn chỉnh (RFC/User Story).

### 2\. Luồng Tương Tác Giữa Các Agent (Workflow Protocol)

Triển khai luồng tương tác theo **Mô hình Tranh luận 4 Pha (4-Phase Debate Pipeline)**:

\[Nhập Ý Tưởng/Tính Năng\]  
│  
▼  
\[Pha 1: Phản Bằng Khách Hàng\] ──> Personas đưa ra Pain Points & Edge Cases  
│  
▼  
\[Pha 2: Chuẩn Hóa Yêu Cầu\] ──> BA Agent tổng hợp thành User Story & Acceptance Criteria  
│  
▼  
\[Pha 3: Phản Biện Kỹ Thuật\] ──> Architect / BE / DevOps soi CSDL, Thuật toán, Concurrency  
│  
▼  
\[Pha 4: Tổng Hợp Output\] ──> Facilitator xuất file RFC / Task Ticket cho Developer

### 3\. Quy Trình Triển Khai Kỹ Thuật Trên Antigravity

**Bước 1: Thiết lập Shared Memory (Bộ nhớ dùng chung)**

- Tạo một không gian bộ nhớ chung (Context / Scratchpad) để tất cả Agent cùng truy cập. Mỗi khi một Agent phát biểu, phản hồi sẽ được ghi vào Scratchpad kèm nhãn \[User-Persona\], \[BA-Expert\], \[BE-Architect\].

**Bước 2: Cài đặt Guardrails (Quy tắc kiểm soát)**

- **Turn Limit:** Giới hạn tối đa 2 lượt tranh luận cho mỗi pha để tránh lặp vô tận (Agent Loop).
- **Focus Enforcement:** Bắt buộc các Agent Khách hàng chỉ nói về _Trải nghiệm/Cảm xúc_, Chuyên gia kỹ thuật chỉ nói về _Giải pháp/Cấu trúc_.

**Bước 3: Mẫu Prompt Khởi Tạo Luồng (Sample Orchestrator Prompt)**

Bạn là Agent Điều phối Hội đồng. Bạn đang quản lý hội đồng phản biện tính năng: "Chia tiền nhóm & Bóc tách hóa đơn OCR".  
<br/>Nhiệm vụ:  
1\. Cho phép Agent \[Khách hàng Giả lập\] đóng góp ý kiến và nêu khó khăn thực tế trước.  
2\. Yêu cầu Agent \[BA Specialist\] chuyển đổi các khó khăn đó thành Use Case Specification.  
3\. Yêu cầu Agent \[Backend Specialist\] và \[Architect\] phản biện về mặt CSDL, Concurrency và Thuật toán.  
4\. Tóm tắt kết quả thành 1 bản RFC chuẩn để gửi cho lập trình viên.

### 4\. Kết Quả Đầu Ra Của Hội Đồng (Expected Output)

Sau mỗi phiên thảo luận giả lập, hội đồng sẽ trả về một bộ tài liệu thu gọn gồm:

- **User Feedback Summary:** Các rủi ro UX và trường hợp sử dụng thực tế chưa lường trước.
- **Refined User Story:** Đã kèm điều kiện biên (Edge cases) và Acceptance Criteria.
- **Technical Decision Record (ADR):** Đã được các chuyên gia BE/SE/DevOps đồng thuận về thiết kế CSDL, xử lý Concurrency và thuật toán.