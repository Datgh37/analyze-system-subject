---
trigger: model_decision
description: Quy chuẩn văn phong học thuật, loại bỏ từ ngữ rập khuôn AI (Anti-Robotic Prose, cấm lạm dụng từ ma trận) và quy định báo cáo kỹ thuật Markdown.
---

# BỘ QUY CHUẨN VĂN PHONG HỌC THUẬT & BÁO CÁO KỸ THUẬT (DNTU) - PHẦN 4
**ĐẠI HỌC CÔNG NGHỆ ĐỒNG NAI (DNTU)**

Tài liệu này quy định quy chuẩn văn phong học thuật chính quy, các nguyên tắc loại bỏ từ ngữ rập khuôn của AI (Anti-Robotic Prose) và quy định xuất bản báo cáo kỹ thuật định dạng Markdown (`.md`) phục vụ đồ án Khóa luận Tốt nghiệp. Áp dụng bắt buộc cho các tác tử AI và công cụ soạn thảo trong workspace.

---

## 1. QUY CHUẨN VĂN PHONG HỌC THUẬT & LOẠI BỎ TỪ NGỮ RẬP KHUÔN / AI-ISMS (ANTI-ROBOTIC PROSE)

> [!CAUTION]
> **Tuyệt đối loại bỏ văn phong sáo rỗng của AI (AI-isms), từ ngữ phóng đại, tối nghĩa và rập khuôn:**
> Khóa luận tốt nghiệp là một văn bản học thuật kỹ thuật chính quy, cần thể hiện văn phong tự nhiên, khách quan, chính xác và trong sáng của sinh viên ngành Hệ thống Thông tin / Khoa học Máy tính.

1. **Tuyệt đối không lạm dụng từ "Ma trận" (Matrix) cho các bảng thông thường:**
   - Chỉ dùng từ "Ma trận" (Matrix) khi thực sự có bản chất toán học/đại số 2 chiều chặt chẽ (như Ma trận Traceability RTM, Ma trận nhầm lẫn Confusion Matrix, Ma trận hiệp phương sai...).
   - Với các bảng phân loại, tổng hợp, thống kê, đối chiếu: Phải dùng từ ngữ chuyên môn tự nhiên, trong sáng:
     - ❌ *Không dùng:* "Ma trận tổng hợp phân loại 60 Use Cases" ➔ ✅ *Dùng:* "Bảng tổng hợp danh mục ca sử dụng theo phân hệ" hoặc "Bảng phân loại ca sử dụng".
     - ❌ *Không dùng:* "Ma trận sàng lọc loại bỏ lớp" ➔ ✅ *Dùng:* "Bảng đối chiếu tiêu chí sàng lọc các lớp nghiệp vụ" hoặc "Bảng đánh giá loại bỏ lớp".
     - ❌ *Không dùng:* "Ma trận Pain Points" ➔ ✅ *Dùng:* "Bảng tổng hợp các vấn đề khó khăn của người dùng".
2. **Loại bỏ triệt để các từ ngữ sáo rỗng, phóng đại (AI Buzzwords & Fluff):**
   - Tránh các cụm từ hoa mỹ nhưng rỗng tuếch thường thấy ở AI sinh nội dung như: *"đột phá toàn diện"*, *"linh hồn trí tuệ nhân tạo"*, *"vượt bậc"*, *"hoàn hảo"*, *"vũ khí tối thượng"*, *"bức tranh đa chiều toàn cảnh"*, *"ma trận chiến lược"*, *"trụ cột then chốt không thể tách rời"*...
   - Diễn đạt trung thực, khiêm tốn, tập trung vào bản chất kỹ thuật: cấu trúc dữ liệu, giải thuật, tham số định lượng, độ phức tạp tính toán, luồng xử lý và kết quả kiểm thử.
3. **Phân biệt rạch ròi giữa Thuật ngữ chuyên môn chuẩn mực vs. Diễn đạt gượng gạo:**
   - Sử dụng thuật ngữ kỹ thuật chính quy đã được chuẩn hóa trong giáo trình đại học (ví dụ: *kiến trúc hướng miền*, *đối tượng giá trị*, *khóa chính/khóa ngoại*, *tính toàn vẹn tham chiếu*, *tính bất biến*, *thời gian đáp ứng*).
   - Câu cú cần gãy gọn, tự nhiên, mạch lạc; tránh ghép từ rườm rà, tối nghĩa gây cảm giác máy móc hoặc dịch thô từ công cụ dịch tự động.
4. **Cấu trúc đoạn văn học thuật chuẩn mực:**
   - Mỗi đoạn văn phải có cấu trúc logic: Câu chủ đề (Topic Sentence) $\rightarrow$ Phân tích & Luận điểm triển khai $\rightarrow$ Dẫn chứng kỹ thuật / Số liệu định lượng $\rightarrow$ Đánh giá & Câu kết đoạn. Tránh các đoạn văn một câu hoặc danh sách bullet cộc lốc thiếu bối cảnh.

---

## 2. QUY ĐỊNH VỀ BÁO CÁO KỸ THUẬT ĐỊNH DẠNG MARKDOWN (MARKDOWN TECHNICAL REPORTS)

> [!IMPORTANT]
> **Bắt buộc xuất Báo cáo kỹ thuật Markdown (`.md`) khi Người dùng yêu cầu:**
> - Mỗi khi Người dùng yêu cầu biên soạn báo cáo học thuật cho từng bài Lab (ví dụ: Lab 1, Lab 2, Lab 3) hoặc từng giai đoạn phân tích thiết kế, Agent **BẮT BUỘC** phải biên soạn một tệp báo cáo chuyên sâu hoàn chỉnh dưới định dạng Markdown (`.md`) lưu tại thư mục `project_context/reports/` (ví dụ: `project_context/reports/BaoCao_Lab3_ThietKe_Class_Diagram_PFM.md`).
> - **Nội dung bắt buộc của Báo cáo Markdown:**
>   1. Mục tiêu và cơ sở phương pháp luận của bài tập / giai đoạn.
>   2. Toàn văn chi tiết từng bước phân tích học thuật (ví dụ: 6 bước xây dựng Class Diagram của giảng viên).
>   3. Các bảng đối soát, bảng phân tích loại trừ, đặc tả thuộc tính/phương thức/quan hệ chi tiết.
>   4. Mã nguồn biểu đồ trực quan (Mermaid code và liên kết tới tệp `.drawio` tương ứng).
>   5. Kết luận học thuật và định hướng kết nối vào Khóa luận Tốt nghiệp.
> - **Mục đích:** Đảm bảo người dùng có một tài liệu văn bản chi tiết, bền vững, dễ đọc trên IDE/GitHub, dễ trích xuất nội dung vào báo cáo Word và phục vụ nộp bài tập cho giảng viên.
