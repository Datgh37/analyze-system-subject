# BÁO CÁO NGHIÊN CỨU & CHUẨN HÓA DANH SÁCH TÁC NHÂN (ACTOR) VÀ RANH GIỚI HỆ THỐNG (SYSTEM BOUNDARY)

> **Dự án:** Hệ thống Quản lý Chi tiêu và Lập Ngân sách Cá nhân Thông minh (PFM - Personal Finance Management)  
> **Thuộc kế hoạch:** Giai đoạn 1 / `PLAN-24` (`project_context/plans/plans-general/2026-10-09_plan_24_toan_dien_ucd_cd_usecase_kltn_redesign.md`)  
> **Chuyên gia phụ trách:** Alistair Cockburn & Debra Paul (Agent 03 - Lead BA) phối hợp cùng Eric Evans (Agent 04 - DDD Lead)  
> **Ngày phê duyệt:** 2026-10-16  

---

## 1. CƠ SỞ LÝ LUẬN & PHƯƠNG PHÁP LUẬN XÁC ĐỊNH TÁC NHÂN (ACTOR METHODOLOGY)

Theo tiêu chuẩn quốc tế **IEEE 29148 (Systems and software engineering - Life cycle processes - Requirements engineering)** và phương pháp luận viết Ca sử dụng của **Alistair Cockburn (Writing Effective Use Cases)**:
1. **Khái niệm Tác nhân (Actor):** Tác nhân là một vai trò (Role) do một thực thể bên ngoài (Con người, Thiết bị phần cứng hoặc Hệ thống phần mềm ngoại vi) đảm nhận khi tương tác trực tiếp với hệ thống đang được phân tích.
2. **Quy tắc phân loại rành mạch:**
   - **Tác nhân chính (Primary Actor):** Là tác nhân khởi phát tương tác với hệ thống nhằm đạt được một mục tiêu cụ thể (Goal). Tác nhân này là người hưởng lợi trực tiếp từ ca sử dụng.
   - **Tác nhân phụ / Ngoại vi (Secondary / Supporting / External System Actor):** Là các hệ thống bên ngoài cung cấp dịch vụ, dữ liệu hoặc cơ sở hạ tầng nền tảng để hệ thống hoàn thành mục tiêu của ca sử dụng.
3. **Quy tắc chống "Phình to Tác nhân" (Anti-Pollution Rule):**
   - **Tuyệt đối không đưa các module/thành phần nội bộ của hệ thống thành Actor:** Các thuật toán, cơ sở dữ liệu nội bộ (PostgreSQL, Redis), hay các module tính toán (như thuật toán DSA, thuật toán Snowball) là thành phần cấu thành hệ thống (System Under Design - SuD), không phải là tác nhân bên ngoài ranh giới.
   - **Hợp nhất các dịch vụ ngoài có cùng nhà cung cấp/bản chất:** Thay vì tạo 5-6 tác nhân AI rời rạc gây rối sơ đồ UML, toàn bộ năng lực AI đám mây (Vision, Voice, NLU) được đóng gói vào một Tác nhân dịch vụ chuẩn hóa duy nhất: `AI Multimodal Service`.

---

## 2. BẢNG CHUẨN HÓA 4 TÁC NHÂN CỐT LÕI (UNIFIED ACTOR TAXONOMY)

Hệ thống PFM xác định chính xác **4 Tác nhân cốt lõi**, bao gồm **1 Tác nhân con người chính** và **3 Tác nhân dịch vụ hệ thống ngoại vi**:

| STT | Tên Tác Nhân (Việt / Anh) | Phân Loại | Ký Hiệu UML | Bản Chất & Ranh Giới Trách Nhiệm Nghiệp Vụ | Giao Thức / Phương Thức Tương Tác |
| :---: | :--- | :--- | :---: | :--- | :--- |
| **01** | **Người dùng cá nhân**<br>*(Personal User)* | **Tác nhân chính**<br>*(Primary Human Actor)* | Standard UML Stickman | • Là chủ sở hữu tối cao của dữ liệu tài chính cá nhân.<br>• Trực tiếp kích hoạt và vận hành toàn bộ 9 phân hệ (UC-01 đến UC-09).<br>• Thực hiện ghi chép thu chi, thiết lập ngân sách, nhận diện cú hích và đưa ra quyết định tài chính cuối cùng (Human-in-the-loop).<br>• *Lưu ý:* Hệ thống định vị 100% tài chính cá nhân, không có tác nhân Quản trị viên (Admin) can thiệp vào sổ cái ví để bảo vệ tính riêng tư tuyệt đối (Zero-Knowledge Privacy). | Giao diện di động (Mobile App UI - React Native / Flutter / Native Client) |
| **02** | **Dịch vụ AI Đa phương thức**<br>*(AI Multimodal Service)* | **Tác nhân dịch vụ ngoài**<br>*(External System Actor)* | Box / Stereotype `<<system>>` / Actor | • Cung cấp năng lực AI đám mây ngoại vi đa phương thức (Google Cloud Gemini API / OpenAI API / Whisper):<br>1. *Thị giác (Vision Multimodal):* Bóc tách hóa đơn siêu thị đa món (`UC-03.07`), bóc tách ảnh chụp số dư ứng dụng ngân hàng (`UC-01.05`), bóc tách phiếu lương thu nhập (`UC-02.04`).<br>2. *Âm thanh & Giọng nói (Speech-to-Text & NLU):* Nhận diện khẩu lệnh giọng nói để nhập chi tiêu nhanh không hóa đơn (`UC-03.11`).<br>3. *Dự báo & Đề xuất (Recommendation & Nudge):* Cung cấp các mô hình chuỗi thời gian hỗ trợ dự báo rủi ro cạn tiền (`UC-08`) và sinh nội dung cú hích có giải thích (`UC-09`). | Giao tiếp bất đồng bộ qua HTTPS REST API / JSON Schema ràng buộc / Webhook (HTTP 202 Accepted) |
| **03** | **Cổng thanh toán VietQR**<br>*(VietQR Gateway - NAPAS 247)* | **Tác nhân dịch vụ ngoài**<br>*(External System Actor)* | Box / Stereotype `<<system>>` / Actor | • Cung cấp hạ tầng chuẩn hóa dữ liệu thanh toán liên ngân hàng quốc gia theo tiêu chuẩn NAPAS 247 và chuẩn quốc tế EMVCo Merchant-Presented Mode.<br>• Phân tích cú pháp chuỗi mã QR của bên bán để tự động bóc tách số tài khoản, tên ngân hàng và số tiền (nếu có Tag 54).<br>• Khởi tạo chuỗi mã VietQR động có kẹp sẵn số tiền và sinh Deeplink chuyển tiếp trực tiếp sang ứng dụng ngân hàng của người dùng (`UC-03.10`). | Chuẩn EMVCo QR Code Specification / Deep Link Scheme (App-to-App URL Schema) |
| **04** | **Dịch vụ Thông báo & Cảnh báo**<br>*(Notification Service)* | **Tác nhân dịch vụ ngoài**<br>*(External System Actor)* | Box / Stereotype `<<system>>` / Actor | • Hạ tầng dịch vụ đẩy thông báo thời gian thực (Firebase Cloud Messaging - FCM / Apple APNs).<br>• Đóng vai trò là kênh truyền tải thông điệp từ hệ thống PFM đến thiết bị di động của người dùng:<br>1. Cảnh báo vi mô vượt ngưỡng ngân sách danh mục (80% / 100%).<br>2. Cảnh báo tốc độ tiêu tiền quá nhanh ($V > 1.2$) từ động cơ DSA.<br>3. Phát Cú hích Thaler kèm khoảng trễ tâm lý (Cooling-off 24h-48h).<br>4. Nhắc nhở các khoản nợ đến hạn và bẫy thuê bao định kỳ (Zombie Subscription). | Google FCM / Apple APNs SDK qua HTTP/2 Push Protocol |

---

## 3. THIẾT LẬP RANH GIỚI HỆ THỐNG (SYSTEM BOUNDARY DEFINITION)

Ranh giới hệ thống (System Boundary) phân định rõ ràng giữa những thành phần **thuộc quyền quản lý của phần mềm PFM (In-Scope)** và những hệ thống **hoàn toàn độc lập bên ngoài (Out-of-Scope)**:

```mermaid
graph TB
    subgraph OUT_OF_SCOPE ["RANH GIỚI BÊN NGOÀI (EXTERNAL ECOSYSTEM)"]
        CB["Hệ thống Core Banking các Ngân hàng<br>(Vietcombank, MB, Techcombank...)"]
        E_WALLET["Các Ví điện tử độc lập<br>(MoMo, ZaloPay, ShopeePay)"]
        NAPAS["Mạng lưới chuyển mạch tài chính quốc gia<br>(NAPAS 247)"]
        AI_CLOUD["Hạ tầng AI Cloud quốc tế<br>(Google Gemini / OpenAI Whisper)"]
        PUSH_GW["Hạ tầng Cloud Push Message<br>(Google FCM / Apple APNs)"]
    end

    subgraph SYSTEM_BOUNDARY ["RANH GIỚI HỆ THỐNG PFM (SYSTEM UNDER DESIGN)"]
        direction TB
        subgraph CORE_MODULES ["9 PHÂN HỆ NGHIỆP VỤ NỘI TẠI"]
            M1["PH-01: Quản lý Sổ cái Đa ví ACID (Tiền mặt, Ngân hàng, Thẻ tín dụng, Ví điện tử)"]
            M2["PH-02: Quản lý Dòng tiền Thu nhập (Cố định, Biến động, Phiếu lương)"]
            M3["PH-03: Quản lý Chi tiêu Đa kênh (Thủ công, AI OCR Hóa đơn, Voice, VietQR)"]
            M4["PH-04: Cây Phân cấp Danh mục & Bộ quy tắc gán nhãn tự động"]
            M5["PH-05: Ngân sách Đa phương pháp (50/30/20, ZBB, Kakeibo) & Động cơ DSA"]
            M6["PH-06: Quỹ Tích lũy Khẩn cấp & Giải thuật Tối ưu Nợ (Snowball vs Avalanche)"]
            M7["PH-07: Báo cáo & Phân tích Đa chiều (Net Worth, Net Cashflow, Sankey)"]
            M8["PH-08: Động cơ Dự báo Rủi ro Cạn tiền & Mô phỏng What-If"]
            M9["PH-09: Động cơ Cú hích Hành vi Thaler & Khoảng trễ Tâm lý (Cooling-off)"]
        end
        DB[("Cơ sở dữ liệu Quan hệ 3NF<br>(PostgreSQL / MSSQL)")]
        CORE_MODULES <--> DB
    end

    USER(("Người dùng cá nhân<br>(Personal User)")) <==>|Thao tác trực tiếp qua Mobile App| SYSTEM_BOUNDARY

    SYSTEM_BOUNDARY -.->|Gửi ảnh & âm thanh / Nhận JSON bóc tách| AI_CLOUD
    SYSTEM_BOUNDARY -.->|Chuẩn hóa chuỗi EMVCo & Sinh Deeplink| NAPAS
    SYSTEM_BOUNDARY -.->|Đẩy cảnh báo vi mô & Cú hích| PUSH_GW
    PUSH_GW -.->|Hiển thị thông báo màn hình khóa| USER

    SYSTEM_BOUNDARY -.->|Deeplink chuyển khoản (Không giữ tiền)| CB
    SYSTEM_BOUNDARY -.->|Mở App thanh toán| E_WALLET

    style SYSTEM_BOUNDARY fill:#f0f7ff,stroke:#0055aa,stroke-width:3px,stroke-dasharray: 5 5
    style OUT_OF_SCOPE fill:#fff5f5,stroke:#cc0000,stroke-width:1px
    style CORE_MODULES fill:#ffffff,stroke:#333333,stroke-width:1px
```

### 3.1. Những gì NẰM TRONG Ranh giới Hệ thống (In-Scope):
1. **Sổ cái tài khoản đa ví mô phỏng (Ledger Accounts):** Quản lý trạng thái số dư, lịch sử biến động và các giao dịch cấn trừ/chuyển tiền nội bộ tuân thủ nghiêm ngặt tính toàn vẹn ACID.
2. **Thuật toán và Động cơ Nghiệp vụ Nội tại:**
   - Thuật toán Hậu kiểm toán số học hóa đơn 3 bước (Mathematical Audit).
   - Động cơ Hạn mức Chi tiêu Thích ứng mỗi ngày (DSA Engine).
   - Động cơ Mô phỏng Lộ trình Trả nợ (Snowball vs Avalanche Solver).
   - Động cơ Khoảng trễ Tâm lý (Cooling-off Period State Machine).
3. **Cơ sở dữ liệu quan hệ chuẩn 3NF:** Lưu trữ toàn bộ dữ liệu người dùng, phân loại danh mục, lịch sử giao dịch và kế hoạch tài chính.

### 3.2. Những gì NẰM NGOÀI Ranh giới Hệ thống (Out-of-Scope):
1. **Tiền thật và Tài khoản thanh toán tại Ngân hàng (Core Banking):** Hệ thống PFM không nắm giữ tiền mặt, không có quyền tự động khấu trừ tiền trong tài khoản ngân hàng của người dùng mà không có sự xác thực sinh trắc học/OTP của chính người dùng trên ứng dụng ngân hàng chính thức.
2. **Cơ chế Screen-scraping độc hại:** Hệ thống từ chối lưu trữ Tên đăng nhập và Mật khẩu ngân hàng của người dùng để tránh vi phạm Điều khoản dịch vụ ngân hàng và luật an ninh mạng.
3. **Mô hình đào tạo AI nền tảng (Foundation Model Training):** Hệ thống không tự huấn luyện các mô hình thị giác hay nhận diện giọng nói từ đầu, mà đóng vai trò là Client tiêu thụ dịch vụ API từ các nhà cung cấp AI tiên tiến thông qua kỹ thuật Prompt Engineering ràng buộc cấu trúc JSON.

---

## 4. MA TRẬN TƯƠNG TÁC TÁC NHÂN XUYÊN SUỐT 9 PHÂN HỆ (ACTOR INTERACTION MATRIX)

Bảng ma trận xác định rõ vai trò của từng tác nhân trong từng phân hệ nghiệp vụ:
- **[P] - Primary Initiator:** Tác nhân chính khởi phát hành động.
- **[S] - Supporting Service:** Tác nhân dịch vụ ngoại vi cung cấp dữ liệu / bóc tách thông tin.
- **[R] - Receiver:** Tác nhân tiếp nhận cảnh báo, thông báo hoặc dữ liệu điều hướng.

| Mã Phân Hệ | Tên Phân Hệ Nghiệp Vụ | Người dùng cá nhân *(Personal User)* | Dịch vụ AI Đa phương thức *(AI Multimodal Service)* | Cổng thanh toán VietQR *(VietQR Gateway)* | Dịch vụ Thông báo *(Notification Service)* | Ghi Chú Tương Tác Nghiệp Vụ Điển Hình |
| :---: | :--- | :---: | :---: | :---: | :---: | :--- |
| **PH-01** | **Quản lý Tài khoản & Đa ví ACID** | **[P]** | **[S]** | - | - | AI bóc tách ảnh chụp số dư app ngân hàng (`UC-01.05`) để cân chỉnh ví tự động. |
| **PH-02** | **Quản lý Dòng tiền Thu nhập** | **[P]** | **[S]** | - | - | AI bóc tách phiếu lương giấy/điện tử (`UC-02.04`) để ghi nhận lương Net và các khoản giảm trừ. |
| **PH-03** | **Quản lý Chi tiêu & Bóc tách Hóa đơn** | **[P]** | **[S]** | **[S]** / **[R]** | **[R]** | • AI bóc tách hóa đơn siêu thị (`UC-03.07`).<br>• AI nhận diện giọng nói tiếng Việt (`UC-03.11`).<br>• VietQR tạo mã thanh toán & Deeplink (`UC-03.10`).<br>• Notification cảnh báo bẫy thuê bao Zombie (`UC-03.09`). |
| **PH-04** | **Danh mục Chi tiêu & Quy tắc Phân loại** | **[P]** | - | - | - | Người dùng thiết lập cây phân cấp Cha - Con và các từ khóa tự động gán nhãn. |
| **PH-05** | **Ngân sách Đa phương pháp & Động cơ DSA** | **[P]** | - | - | **[R]** | Notification đẩy cảnh báo vi mô khi tốc độ tiêu tiền vượt chuẩn ($V > 1.2$) hoặc chạm 80% hạn mức. |
| **PH-06** | **Quỹ Tích lũy & Tối ưu hóa Nợ** | **[P]** | - | - | **[R]** | Notification nhắc lịch đến hạn thanh toán nợ theo chiến lược Snowball / Avalanche đã chọn. |
| **PH-07** | **Báo cáo Tài chính & Dòng tiền Đa chiều** | **[P]** | - | - | - | Người dùng trực tiếp tra cứu báo cáo Tài sản ròng (Net Worth) và lưu chuyển tiền tệ. |
| **PH-08** | **Dự báo Tài chính & Phân tích Rủi ro** | **[P]** | **[S]** | - | **[R]** | AI hỗ trợ mô phỏng chuỗi thời gian What-If và Notification phát cảnh báo ngày cạn tiền. |
| **PH-09** | **Cú hích Hành vi & Hỗ trợ Quyết định (Nudge)** | **[P]** | **[S]** | - | **[R]** | • AI sinh nội dung giải thích lý trí (Rationale Explanation).<br>• Notification đẩy thông điệp Cú hích.<br>• Người dùng nắm quyền quyết định tối cao sau thời gian hạ nhiệt (Cooling-off). |

---

## 5. PHÂN TÍCH TÁC ĐỘNG ĐẾN CÁC GIAI ĐOẠN KẾ TIẾP (IMPACT ANALYSIS)

Việc chuẩn hóa ranh giới và danh mục 4 Tác nhân tại Giai đoạn 1 mở đường trực tiếp cho các giai đoạn tiếp theo:
1. **Giai đoạn 2 (Sơ đồ Ca sử dụng Draw.io):**
   - Hợp nhất các Actor AI rời rạc trong `files/UCD-DuoNoPro.drawio` về chuẩn `AI Multimodal Service`.
   - Bổ sung 3 kết nối Use Case mới: `UC-01.05` (Scan số dư ví), `UC-02.04` (Scan phiếu lương), `UC-03.11` (Nhập chi tiêu giọng nói).
2. **Giai đoạn 3 (Đặc tả Use Case chi tiết):**
   - Viết mới đặc tả 13 mục IEEE 29148 cho 3 Use Case trên với Tác nhân chính là `Người dùng cá nhân` và Tác nhân phụ là `AI Multimodal Service`.
3. **Giai đoạn 4 (Class Diagram):**
   - Bổ sung các phương thức tiếp nhận dữ liệu từ AI Multimodal Service vào `Wallet` và `Transaction`.
4. **Giai đoạn 5 (Khóa luận DOCX):**
   - Cập nhật `Bảng 3.1: Danh mục các tác nhân và vai trò trong hệ thống PFM` trong file Word khớp 100% với bảng tại Mục 2 của báo cáo này.

---

## 6. KẾT LUẬN GIAI ĐOẠN 1

Giai đoạn 1 đã hoàn thành xuất sắc mục tiêu:
- Loại bỏ hoàn toàn sự mập mờ, chồng chéo giữa các tác nhân AI.
- Xác định rõ ràng ranh giới pháp lý, bảo mật và bản chất nghiệp vụ Sổ cái của hệ thống PFM.
- Sẵn sàng chuyển giao toàn bộ kết quả để tiến hành **Giai đoạn 2: Cập nhật Sơ đồ Ca sử dụng trong tệp Draw.io**.
