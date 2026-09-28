Tổng hợp kết quả trao đổi – Smart Expense & Budget Manager

1. Bối cảnh

Đề tài: Hệ thống quản lý chi tiêu và lập ngân sách cá nhân/nhóm (Smart
Expense & Budget Manager).

Cấu trúc KLTN: 1. Chương 1 – Khảo sát hiện trạng hệ thống 2. Chương 2 –
Cơ sở lý thuyết và các công nghệ liên quan 3. Chương 3 – Phân tích và
thiết kế hệ thống 4. Chương 4 – Hiện thực và đánh giá kết quả 5. Chương
5 – Kết luận và hướng phát triển

Trọng tâm hiện tại là hoàn thiện Chương 1 và chuẩn bị chuyển sang phân
tích hệ thống.

2. Kết quả Chương 1

2.1. Đối tượng sử dụng

Các nhóm người dùng: - Sinh viên - Người mới đi làm - Người có thu nhập
ổn định hoặc có mục tiêu tài chính dài hạn

Ba nhóm này nên được xem là persona, không mặc định là ba Actor khác
nhau.

Actor nghiệp vụ chính: > User / Personal User – Người dùng

2.2. Nghiệp vụ cốt lõi

-   Quản lý thu nhập
-   Quản lý giao dịch chi tiêu
-   Phân loại chi tiêu
-   Quản lý danh mục
-   Lập ngân sách
-   Theo dõi ngân sách
-   Quản lý mục tiêu tài chính
-   Phân tích tình hình tài chính
-   Phát hiện nguy cơ vượt ngân sách
-   Hỗ trợ điều chỉnh ngân sách

CRUD là nền tảng, không phải điểm khác biệt chính.

3. Vấn đề và khoảng trống

Các hệ thống hiện có đã khá phổ biến ở: - ghi nhận giao dịch; - phân
loại; - thống kê; - theo dõi; - lập ngân sách; - báo cáo; - mục tiêu tài
chính.

Khoảng trống trọng tâm: > Người dùng biết mình đã chi bao nhiêu và ngân
sách đang ở trạng thái nào, nhưng vẫn phải tự đánh giá nguy cơ vượt ngân
sách và tự quyết định nên điều chỉnh như thế nào.

Các vấn đề: 1. Nhập và duy trì dữ liệu tốn công. 2. Khó xác định ngân
sách phù hợp cho kỳ tiếp theo từ dữ liệu lịch sử. 3. Phản hồi thường mô
tả trạng thái nhưng chưa nhất thiết giải thích nguyên nhân hoặc đề xuất
hành động. 4. Nhu cầu mỗi người khác nhau nên ngân sách cố định hạn chế
cá nhân hóa.

4. Điểm khác biệt / định hướng giá trị

Chưa nên khẳng định đây là tính mới học thuật nếu chưa có literature
review đủ sâu.

Nên gọi là: > Điểm khác biệt / định hướng giá trị của hệ thống

Định hướng: > Expense Tracking → Adaptive Budgeting → Decision Support

Hệ thống không chỉ ghi nhận và báo cáo chi tiêu mà sử dụng dữ liệu tài
chính thực tế để: - đánh giá tình trạng ngân sách; - dự báo nguy cơ sai
lệch/vượt ngân sách; - đưa ra khuyến nghị; - hỗ trợ người dùng điều
chỉnh kế hoạch.

Luồng chính:

    Dữ liệu tài chính
        ↓
    Phân tích
        ↓
    Đánh giá
        ↓
    Dự báo
        ↓
    Khuyến nghị
        ↓
    Người dùng quyết định
        ↓
    Điều chỉnh ngân sách/kế hoạch

Điểm cốt lõi không nằm ở AI/OCR/VietQR riêng lẻ mà ở cách hệ thống biến
dữ liệu thành hỗ trợ quyết định và điều chỉnh ngân sách thích ứng.

5. Vai trò của công nghệ

5.1. AI

Có thể dùng cho: - phân loại giao dịch; - phân tích hành vi; - hỗ trợ dự
báo; - phát hiện xu hướng; - tạo khuyến nghị.

AI là công nghệ hỗ trợ cơ chế thông minh, không mặc nhiên là điểm khác
biệt.

5.2. OCR

Mục tiêu chính là giảm công sức nhập liệu:

    Ảnh hóa đơn → OCR → Ngày/Cửa hàng/Số tiền → Tạo giao dịch

5.3. VietQR/API/NAPAS

Chỉ mô hình hóa hoặc mô tả ở mức phù hợp với phạm vi triển khai thực tế.
Có tên công nghệ trong danh mục thuật ngữ không đồng nghĩa với việc đó
là Actor.

6. Actor

Có thể khảo sát theo bốn nhóm: 1. Tác nhân con người 2. Tác nhân phần
mềm 3. Tác nhân hệ thống khác 4. Tác nhân phần cứng

Nhưng không bắt buộc hệ thống phải có đủ bốn loại.

Tiêu chí: > Actor là đối tượng nằm ngoài boundary của hệ thống và tương
tác với hệ thống để đạt một mục tiêu.

6.1. Actor chính

User / Personal User

User: - quản lý tài khoản; - quản lý thu nhập; - quản lý giao dịch; -
quản lý danh mục; - lập và theo dõi ngân sách; - quản lý mục tiêu; - xem
phân tích; - xem dự báo; - nhận khuyến nghị; - quyết định điều chỉnh
ngân sách.

User vừa là Data Operator vừa là Decision Maker.

6.2. External Software Actor

Có thể có: - OCR Service - AI Service - Notification Service -
Authentication Provider

Chỉ là Actor nếu thực sự nằm ngoài system boundary và có giao tiếp với
hệ thống.

6.3. External System Actor

Có thể có: - dịch vụ ngân hàng; - dịch vụ đồng bộ giao dịch; - hệ thống
thanh toán; - dịch vụ VietQR liên quan.

Chỉ đưa vào nếu có tương tác thực tế.

6.4. Hardware Actor

Camera/thiết bị có thể được xem xét nếu cần mô hình hóa giao tiếp trực
tiếp. Nếu chỉ là phương tiện của mobile app để chụp hóa đơn thì không
nhất thiết là Actor nghiệp vụ.

7. Thành phần nội bộ không phải Actor

Nếu nằm trong system boundary thì các thành phần sau không phải Actor: -
Budget Analysis Engine - Forecasting Engine - Recommendation Engine - AI
Module nội bộ - OCR Module nội bộ

Ví dụ:

    +----------------------------------------------+
    | Smart Expense & Budget Manager               |
    |                                              |
    | User                                         |
    |   ↓                                          |
    | Budget Management                            |
    |   ↓                                          |
    | Analysis Engine                              |
    |   ↓                                          |
    | Forecasting Engine                           |
    |   ↓                                          |
    | Recommendation Engine                        |
    +----------------------------------------------+

8. Mục 1.5 đã được định hướng sửa đổi

Tên: > 1.5. Các tác nhân và mô hình thực thể nghiệp vụ

Mục tiêu: làm cầu nối giữa khảo sát hiện trạng và Chương 3.

1.5.1. Xác định tác nhân

Cần: - xác định User là Actor chính; - phân biệt persona và Actor; - xác
định external actor theo giao tiếp thực tế; - cân nhắc hardware actor; -
phân biệt Actor với internal component.

1.5.2. Vai trò tác nhân và luồng tương tác

Luồng định hướng:

    User
      ↓
    Nhập/quản lý dữ liệu
      ↓
    Phân tích
      ↓
    Đánh giá ngân sách
      ↓
    Dự báo
      ↓
    Khuyến nghị
      ↓
    User xem xét
      ↓
    Điều chỉnh

1.5.3. Thực thể nghiệp vụ

Thực thể cốt lõi: - User - Income - Expense Transaction - Expense
Category - Budget - Budget Cycle - Financial Goal

Có thể bổ sung tùy phạm vi: - Wallet / Account - Financial History -
Budget Assessment - Recommendation History

Không phải mọi khái niệm nghiệp vụ đều phải là entity database.
Recommendation có thể là kết quả runtime; chỉ nên lưu thành dữ liệu nếu
cần quản lý lịch sử/tra cứu.

1.5.4. Quan hệ nghiệp vụ

    User
     ├── Income
     ├── Expense Transaction
     │      └── Expense Category
     ├── Budget
     │      └── Budget Cycle
     ├── Financial Goal
     └── Financial History
              ↓
        Budget Assessment
              ↓
        Spending Forecast
              ↓
        Recommendation
              ↓
        User Decision
              ↓
        Budget Adjustment

9. Chương 2 – định hướng tạm gác

Dự kiến sau này: - Cơ sở lý thuyết quản lý tài chính cá nhân - Các
phương pháp lập ngân sách: 50/30/20, Zero-Based, Envelope, Pay Yourself
First, Sinking Funds, Cash Flow Budgeting - Adaptive Budgeting và
Decision Support - Phân tích và dự báo chi tiêu - AI và OCR - VietQR/API
và tích hợp dịch vụ - Clean Architecture/DDD - CSDL quan hệ, 3NF, ACID,
Transaction

Hiện tại chưa triển khai Chương 2.

10. Chu trình phát triển tiếp theo

    Chương 1
      ↓
    Xác định vấn đề
      ↓
    Xác định phạm vi
      ↓
    Actor
      ↓
    Business Entity
      ↓
    Chương 2
      ↓
    Chương 3 – Phân tích hệ thống
      ↓
    Requirements
      ↓
    Actor
      ↓
    Goal
      ↓
    Business Flow
      ↓
    Use Case
      ↓
    Use Case Diagram
      ↓
    Use Case Specification
      ↓
    Activity Diagram
      ↓
    Business Rules
      ↓
    Class Diagram
      ↓
    Sequence Diagram
      ↓
    Database Design

Không nên vẽ Use Case Diagram ngay. Trước đó cần hoàn thiện bảng:

  Actor → Goal → Business Flow → Use Case

11. Nguyên tắc xuyên suốt

1.  Không xem công nghệ là điểm khác biệt nếu chưa chứng minh giá trị
    nghiệp vụ.
2.  Không gọi internal component là Actor.
3.  Không biến mọi persona thành Actor.
4.  Không đưa external service vào mô hình nếu hệ thống không thực sự
    giao tiếp.
5.  Không khẳng định novelty học thuật khi chưa có literature review đủ
    sâu.
6.  Điểm khác biệt hiện tại là Adaptive Budgeting + Decision Support.
7.  AI/OCR/VietQR là công nghệ hỗ trợ, không thay thế ý tưởng nghiệp vụ.
8.  Use Case nên thể hiện mục tiêu nghiệp vụ, không chỉ CRUD.
9.  Chương 1 → Chương 2 → Chương 3 → Chương 4 → Chương 5 phải liên kết
    logic.
10. Mọi chức năng đưa vào đồ án phải phù hợp phạm vi thực tế và có thể
    chứng minh trong phần hiện thực/đánh giá.

12. Việc tiếp theo

Bước tiếp theo: > Xây dựng bảng Actor → Goal → Business Flow → Use Case
cho toàn hệ thống.

Sau đó: 1. Chốt danh sách Use Case. 2. Xác định
include/extend/generalization khi thực sự cần. 3. Vẽ Use Case Diagram
tổng quan. 4. Tách Use Case Diagram chi tiết. 5. Viết đặc tả Use Case.
6. Xây dựng Activity Diagram và các mô hình phân tích tiếp theo.
