# TỔNG HỢP TOÀN BỘ CÁC USE CASE ĐÃ THỐNG NHẤT (TỪ CHATGPT)

> Nguồn trích xuất: Cuộc trò chuyện ChatGPT ngày 05/09/2026

---



Tốt. Ta bắt đầu **chốt UC-01: Quản lý tài khoản**.

### UC-01 — Quản lý tài khoản

Ở cấp tổng quát, ta có:

**Actor chính:** `User`

**Mục tiêu:** Người dùng tạo và quản lý tài khoản để sử dụng hệ thống Smart Expense & Budget Manager.

Các UC con dự kiến:

| Mã | Use Case | Trạng thái |
|---|---|---|
| UC-01.01 | Đăng ký tài khoản | Chưa chốt |
| UC-01.02 | Đăng nhập | Chưa chốt |
| UC-01.03 | Đăng xuất | Chưa chốt |
| UC-01.04 | Quản lý thông tin cá nhân | Chưa chốt |
| UC-01.05 | Đổi mật khẩu / khôi phục mật khẩu | Chưa chốt |

Tuy nhiên, **không nên mặc định đưa tất cả vào UCD**. Ví dụ `Đăng xuất` có thể là chức năng rất đơn giản và tùy cách trình bày mà có thể không cần phân rã sâu.

Để chốt UC-01 một cách có căn cứ, ta cần xác định trước **phạm vi chức năng tài khoản thực tế của project**.

Tôi đề xuất chốt theo câu hỏi ngắn sau:

**Project của chúng ta có những chức năng nào về tài khoản?**

A. Đăng ký + Đăng nhập + Đăng xuất  
B. A + Quản lý thông tin cá nhân  
C. B + Đổi mật khẩu  
D. C + Quên/khôi phục mật khẩu  
E. Có thêm chức năng khác

Bạn chỉ cần chọn các chữ cái, ví dụ **A+B+C**, rồi tôi sẽ chốt UC-01 và chuyển sang **UC-02**.

---



Được. Vậy ta **chốt UC-01** với phạm vi đầy đủ như sau, chưa thêm chức năng ngoài những gì đã thống nhất.

## UC-01 — Quản lý tài khoản

**Actor chính:** User

### Các UC con

| Mã | Use Case | Trạng thái |
|---|---|---|
| **UC-01.01** | Đăng ký tài khoản | **Đã chốt** |
| **UC-01.02** | Đăng nhập | **Đã chốt** |
| **UC-01.03** | Đăng xuất | **Đã chốt** |
| **UC-01.04** | Quản lý thông tin cá nhân | **Đã chốt** |
| **UC-01.05** | Đổi mật khẩu | **Đã chốt** |
| **UC-01.06** | Quên / khôi phục mật khẩu | **Đã chốt** |

### Cấu trúc phân cấp

```text
UC-01 Quản lý tài khoản
│
├── UC-01.01 Đăng ký tài khoản
├── UC-01.02 Đăng nhập
├── UC-01.03 Đăng xuất
├── UC-01.04 Quản lý thông tin cá nhân
├── UC-01.05 Đổi mật khẩu
└── UC-01.06 Quên / khôi phục mật khẩu
```

### Actor

Ở thời điểm hiện tại:

```text
User
  │
  ├── Đăng ký tài khoản
  ├── Đăng nhập
  ├── Đăng xuất
  ├── Quản lý thông tin cá nhân
  ├── Đổi mật khẩu
  └── Quên / khôi phục mật khẩu
```

**Chưa thêm Authentication Provider, Email Service hay Notification Service** vào Actor. Những thành phần này chỉ được đưa vào nếu sau này phạm vi triển khai thực tế yêu cầu hệ thống tương tác với dịch vụ bên ngoài.

### Một điểm cần thống nhất

`Quản lý tài khoản` là **UC cấp nhóm**, còn 6 chức năng bên dưới là **UC cấp chi tiết**.

Khi vẽ UCD phân cấp, ta có thể thể hiện:

```text
                 ┌─────────────────────────┐
                 │    Quản lý tài khoản    │
                 └─────────────────────────┘
                           / | \
                          /  |  \
                         /   |   \
                  Đăng ký  Đăng nhập ...
```

Nhưng **không nên vẽ mũi tên tuần tự** kiểu:

`Đăng ký → Đăng nhập → Quản lý thông tin → ...`

vì đó là **luồng nghiệp vụ**, không phải quan hệ giữa các Use Case trong UCD.

---

### Trạng thái hiện tại

**UC-01: ĐÃ CHỐT**

Ta sẽ giữ nguyên cấu trúc này để đưa vào file Markdown cuối cùng.

Tiếp theo ta chuyển sang:

> **UC-02 — Quản lý thu nhập**

Ở UC này, ta sẽ cần quyết định xem chỉ có **Thêm / Xem / Sửa / Xóa thu nhập**, hay hệ thống còn cần các chức năng như **phân loại nguồn thu, thu nhập định kỳ, tổng hợp thu nhập...**.

---



Chuẩn. Vậy **UC-02 được chốt đầy đủ cả hai**, và tôi sẽ ghi nhận lý do nghiệp vụ là **phục vụ phân tích dữ liệu và làm đầu vào cho lập ngân sách thích ứng**.

## UC-02 — Quản lý thu nhập — ĐÃ CHỐT

```text
UC-02 Quản lý thu nhập
│
├── UC-02.01 Thêm khoản thu nhập
├── UC-02.02 Xem danh sách thu nhập
├── UC-02.03 Xem chi tiết khoản thu nhập
├── UC-02.04 Chỉnh sửa khoản thu nhập
├── UC-02.05 Xóa khoản thu nhập
├── UC-02.06 Phân loại nguồn thu nhập
└── UC-02.07 Quản lý thu nhập định kỳ
```

### Ý nghĩa dữ liệu

Hai UC quan trọng được giữ lại:

- **Phân loại nguồn thu nhập** → giúp phân tích cơ cấu và xu hướng nguồn thu.
- **Quản lý thu nhập định kỳ** → giúp hệ thống ước lượng nguồn tiền dự kiến trong từng chu kỳ.

Dữ liệu từ UC-02 sau này có thể trở thành đầu vào cho:

```text
Thu nhập thực tế
       │
       ├── Phân tích thu nhập
       │
       ├── Xác định khả năng chi tiêu
       │
       ├── Lập ngân sách
       │
       ├── Dự báo
       │
       └── Đề xuất điều chỉnh ngân sách
```

**Trạng thái: UC-02 ĐÃ CHỐT.**

---

# UC-03 — Quản lý chi tiêu

Đây sẽ là một **UC quan trọng hơn UC-02**, vì dữ liệu chi tiêu là nguồn dữ liệu chính để hệ thống phân tích hành vi và phát hiện nguy cơ vượt ngân sách.

Tôi đề xuất:

| Mã | Use Case | Trạng thái |
|---|---|---|
| **UC-03.01** | Thêm khoản chi tiêu | Chưa chốt |
| **UC-03.02** | Xem danh sách chi tiêu | Chưa chốt |
| **UC-03.03** | Xem chi tiết khoản chi tiêu | Chưa chốt |
| **UC-03.04** | Chỉnh sửa khoản chi tiêu | Chưa chốt |
| **UC-03.05** | Xóa khoản chi tiêu | Chưa chốt |
| **UC-03.06** | Phân loại khoản chi tiêu | Chưa chốt |
| **UC-03.07** | Nhận dạng thông tin từ hóa đơn | Chưa chốt |
| **UC-03.08** | Gắn khoản chi tiêu với mục đích / mục tiêu tài chính | Chưa chốt |

Cấu trúc dự kiến:

```text
UC-03 Quản lý chi tiêu
│
├── UC-03.01 Thêm khoản chi tiêu
├── UC-03.02 Xem danh sách chi tiêu
├── UC-03.03 Xem chi tiết khoản chi tiêu
├── UC-03.04 Chỉnh sửa khoản chi tiêu
├── UC-03.05 Xóa khoản chi tiêu
├── UC-03.06 Phân loại khoản chi tiêu
├── UC-03.07 Nhận dạng thông tin từ hóa đơn
└── UC-03.08 Gắn khoản chi tiêu với mục đích / mục tiêu tài chính
```

Ở đây có **hai UC cần đặc biệt cân nhắc**:

### UC-03.07 — Nhận dạng thông tin từ hóa đơn

Ví dụ:

> Người dùng chụp hóa đơn → hệ thống nhận dạng → lấy số tiền, ngày, cửa hàng... → người dùng xác nhận → tạo khoản chi.

Nếu project thực sự triển khai OCR thì tôi nghĩ **nên giữ**, vì nó giải quyết một pain point đã xác định ở Chương 1: **giảm công sức nhập dữ liệu**.

Nhưng OCR không nên được xem là điểm khác biệt cốt lõi của hệ thống; nó là **công nghệ hỗ trợ nhập liệu**.

### UC-03.08 — Gắn khoản chi với mục tiêu tài chính

Ví dụ:

> 500.000đ → Du lịch  
> 1.000.000đ → Mua laptop  
> 300.000đ → Quỹ khẩn cấp

UC này có thể rất hữu ích cho phần **Financial Goal + Decision Support**, nhưng nó cũng làm mô hình nghiệp vụ phức tạp hơn.

Tôi muốn **chưa tự chốt UC-03**, mà để bạn quyết định hai điểm này trước.

**Bạn chọn:**

1. `03.07` giữ hay bỏ?
2. `03.08` giữ hay bỏ?

Bạn có thể trả lời kiểu **“giữ cả 2”** như lúc nãy.

---



Đúng, **cả 03.07 và 03.08 đều nên giữ**. Và riêng OCR, tôi nghĩ bạn **không cần quá lo về phần camera**.

### 1. OCR có khó không?

Nếu project là **mobile app**, thì bài toán thực tế có thể tách thành 3 phần:

```text
Camera điện thoại
      ↓
Chụp ảnh hóa đơn
      ↓
OCR
      ↓
Trích xuất thông tin
      ↓
Người dùng kiểm tra / chỉnh sửa
      ↓
Tạo khoản chi tiêu
```

**Camera không nhất thiết phải là Actor của UCD.**

Đây là điểm quan trọng.

Camera là **phần cứng/khả năng của thiết bị**, còn Actor trong UCD phải xét theo việc nó có thực sự là một thực thể bên ngoài hệ thống có mục tiêu tương tác với hệ thống hay không.

Trong trường hợp của project:

```text
User
  │
  │ Chụp hóa đơn
  ↓
Ứng dụng
  │
  │ Gửi ảnh
  ↓
OCR Service
  │
  │ Trả kết quả nhận dạng
  ↓
Ứng dụng
  │
  ↓
User xác nhận
```

Nếu camera chỉ được ứng dụng gọi để **lấy ảnh**, tôi sẽ **không đưa Camera thành một Actor nghiệp vụ riêng** trong UCD.

Nếu OCR chạy bằng thư viện/model **ngay bên trong app/backend của chúng ta**, thì thậm chí `OCR Service` cũng không cần là Actor. Khi đó OCR là **thành phần nội bộ**.

Chỉ khi chúng ta sử dụng một dịch vụ OCR bên ngoài, chẳng hạn API OCR của bên thứ ba, thì mới có lý do để biểu diễn:

```text
User ──> Nhận dạng thông tin từ hóa đơn <── OCR Service
```

### 2. OCR có khả thi cho đồ án không?

**Có, và tôi đánh giá mức độ là trung bình**, không phải phần quá khó nếu ta giới hạn phạm vi.

Ta không cần làm OCR "hoàn hảo cho mọi loại hóa đơn".

Mục tiêu đồ án có thể chỉ cần:

- Nhận ảnh hóa đơn.
- Nhận dạng một số thông tin chính.
- Trích xuất số tiền.
- Ngày giao dịch.
- Tên cửa hàng/đơn vị.
- Một số nội dung hàng hóa nếu khả thi.
- Đề xuất danh mục chi tiêu.
- Cho người dùng **kiểm tra và sửa trước khi lưu**.

Điểm cuối cực kỳ quan trọng.

**Không nên thiết kế:**

```text
OCR → tự động lưu giao dịch
```

Mà nên:

```text
OCR
 ↓
Kết quả nhận dạng
 ↓
User kiểm tra
 ↓
User chỉnh sửa nếu cần
 ↓
Xác nhận
 ↓
Lưu giao dịch
```

Như vậy nếu OCR nhận sai `150.000` thành `180.000`, hệ thống vẫn không làm hỏng dữ liệu tài chính của người dùng.

---

# 3.8 — Gắn khoản chi với mục đích / mục tiêu tài chính

Phần này tôi đồng ý **giữ chắc chắn**.

Nó sẽ giúp liên kết ba module:

```text
Chi tiêu
   ↓
Danh mục / Tag
   ↓
Mục tiêu tài chính
```

Ví dụ:

```text
Chi 500.000đ
│
├── Category: Ăn uống
│
└── Goal: Chuyến du lịch Đà Lạt
```

Điều này rất có giá trị cho phần phân tích sau này:

> Người dùng không chỉ biết mình đã tiêu bao nhiêu, mà còn biết **tiền đang được sử dụng cho mục đích nào và việc chi tiêu đó ảnh hưởng thế nào đến mục tiêu tài chính**.

---

# UC-03 được chốt như sau

```text
UC-03 Quản lý chi tiêu
│
├── UC-03.01 Thêm khoản chi tiêu
├── UC-03.02 Xem danh sách chi tiêu
├── UC-03.03 Xem chi tiết khoản chi tiêu
├── UC-03.04 Chỉnh sửa khoản chi tiêu
├── UC-03.05 Xóa khoản chi tiêu
├── UC-03.06 Phân loại khoản chi tiêu
├── UC-03.07 Nhận dạng thông tin từ hóa đơn
└── UC-03.08 Gắn khoản chi tiêu với mục đích / mục tiêu tài chính
```

**UC-03: ĐÃ CHỐT.**

Và tôi sẽ ghi chú trong tài liệu cuối:

> `UC-03.07` có sử dụng OCR. Camera được xem là khả năng phần cứng của thiết bị, không mặc định biểu diễn thành Actor; `OCR Service` chỉ được biểu diễn là Actor nếu hệ thống thực sự tích hợp với dịch vụ OCR bên ngoài.

Điều này giúp UCD của chúng ta **không bị vẽ sai Actor chỉ vì thấy có camera**.

---

Bây giờ hợp lý nhất là sang **UC-04 — Quản lý danh mục**, vì `Category/Tag` đang là nền tảng cho cả chi tiêu, phân tích và ngân sách. **Bạn muốn UC-04 theo hướng đơn giản (CRUD danh mục) hay đầy đủ hơn với danh mục hệ thống + danh mục tùy chỉnh + tag?**

---



Về **mục đích nghiệp vụ**, chúng gần như giống nhau; khác nhau chủ yếu ở **nguồn cung cấp hình ảnh**.

### So sánh

| | Mobile: OCR bằng camera | Web: OCR từ hình ảnh |
|---|---|---|
| Nguồn ảnh | Camera điện thoại | File ảnh người dùng chọn/upload |
| Bước đầu | Mở camera → chụp | Chọn ảnh → upload |
| OCR | Xử lý ảnh vừa chụp | Xử lý ảnh đã upload |
| Kết quả | Văn bản/thông tin hóa đơn | Văn bản/thông tin hóa đơn |
| Mục đích | Tự động nhập giao dịch | Tự động nhập giao dịch |
| Actor nghiệp vụ | User | User |
| OCR Service | Có thể có | Có thể có |

Về mặt Use Case, ta **không nhất thiết phải tạo hai UC**:

```text
UC-03.07 Nhận dạng thông tin từ hóa đơn
```

Sau đó bên trong UC này có thể có hai cách kích hoạt:

```text
                 UC-03.07
        Nhận dạng thông tin từ hóa đơn
                  /       \
                 /         \
        Chụp ảnh           Chọn ảnh
        bằng camera        từ thiết bị
```

Hoặc nếu muốn mô hình chi tiết hơn:

```text
User
 │
 ├── Chụp hóa đơn
 │        ↓
 │      OCR
 │
 └── Chọn/upload hình ảnh
          ↓
        OCR
```

### Nhưng có một điểm rất đáng chú ý với project của chúng ta

Nếu bạn **chưa quyết định mobile app hay web app**, tôi khuyên **không đưa "Camera" vào tên UC**.

Nên gọi:

> **UC-03.07 — Nhận dạng thông tin từ hóa đơn**

Thay vì:

> ~~Quét hóa đơn bằng camera~~

Bởi vì tên UC nên mô tả **mục tiêu nghiệp vụ**, không nên phụ thuộc vào công nghệ/thiết bị.

Sau này:

**Mobile:**

```text
User → Chụp hóa đơn → OCR → Xác nhận → Tạo khoản chi
```

**Web:**

```text
User → Upload hình ảnh → OCR → Xác nhận → Tạo khoản chi
```

Nhưng cả hai đều thực hiện cùng một nghiệp vụ:

```text
Nhận dạng thông tin từ hóa đơn
```

### Và điều này còn có lợi cho thiết kế hệ thống

Ta có thể thiết kế abstraction:

```text
Image Input
     ↓
Image Processing
     ↓
OCR
     ↓
Information Extraction
     ↓
User Verification
     ↓
Expense Transaction
```

Như vậy sau này đổi **Camera → Upload → Gallery → Scan document** cũng không ảnh hưởng đến Use Case nghiệp vụ.

**Vì vậy tôi đề xuất giữ UC-03.07 như hiện tại: _Nhận dạng thông tin từ hóa đơn_.** Phương thức lấy ảnh sẽ được mô tả trong đặc tả UC hoặc Activity Diagram, không tách thành một UC mới.

Và câu hỏi này cũng giúp ta chốt một nguyên tắc cho toàn bộ project: **UCD tập trung vào nghiệp vụ; công nghệ thực hiện nghiệp vụ sẽ được mô tả ở các sơ đồ/đặc tả khác.**

---



Được, ta sang **UC-04 — Quản lý danh mục**.

Đây là UC khá quan trọng vì danh mục sẽ liên kết trực tiếp với **chi tiêu → phân tích → ngân sách → dự báo**. Tôi đề xuất không chỉ làm CRUD đơn thuần mà nên phân biệt **danh mục hệ thống** và **danh mục do người dùng tùy chỉnh**.

# UC-04 — Quản lý danh mục

### Các UC con đề xuất

| Mã | Use Case | Trạng thái |
|---|---|---|
| **UC-04.01** | Xem danh sách danh mục | Chưa chốt |
| **UC-04.02** | Thêm danh mục tùy chỉnh | Chưa chốt |
| **UC-04.03** | Chỉnh sửa danh mục | Chưa chốt |
| **UC-04.04** | Xóa danh mục | Chưa chốt |
| **UC-04.05** | Phân loại danh mục thu nhập/chi tiêu | Chưa chốt |
| **UC-04.06** | Quản lý danh mục mặc định | Chưa chốt |

Cấu trúc:

```text
UC-04 Quản lý danh mục
│
├── UC-04.01 Xem danh sách danh mục
├── UC-04.02 Thêm danh mục tùy chỉnh
├── UC-04.03 Chỉnh sửa danh mục
├── UC-04.04 Xóa danh mục
├── UC-04.05 Phân loại danh mục thu nhập/chi tiêu
└── UC-04.06 Quản lý danh mục mặc định
```

## Nhưng tôi muốn điều chỉnh một điểm

`UC-04.05 Phân loại danh mục thu nhập/chi tiêu` có thể **không cần tồn tại như một UC riêng**.

Ví dụ khi tạo danh mục:

> Tên: Ăn uống  
> Loại: Chi tiêu

hoặc:

> Tên: Lương  
> Loại: Thu nhập

Thì việc xác định loại chỉ là **một thuộc tính trong UC Thêm/Chỉnh sửa danh mục**, chứ chưa chắc là một mục tiêu nghiệp vụ độc lập.

Tương tự, `UC-04.06 Quản lý danh mục mặc định` cũng cần cân nhắc. Nếu danh mục mặc định được hệ thống tạo sẵn và người dùng chỉ sử dụng chúng, thì đây có thể là **quy tắc/khởi tạo dữ liệu**, không phải UC của User.

Vì vậy tôi nghiêng về cấu trúc gọn hơn:

```text
UC-04 Quản lý danh mục
│
├── UC-04.01 Xem danh sách danh mục
├── UC-04.02 Thêm danh mục tùy chỉnh
├── UC-04.03 Chỉnh sửa danh mục
└── UC-04.04 Xóa danh mục
```

Trong đó mỗi danh mục có thể có:

```text
Category
├── ID
├── Name
├── Type
│   ├── Income
│   └── Expense
├── Icon
├── Color
└── IsDefault
```

`Type = Income/Expense` là **thuộc tính của Category**, không cần biến thành UC.

`IsDefault` cũng là **thuộc tính/quy tắc hệ thống**, không nhất thiết là UC.

---

## Một vấn đề nữa: Category hay Tag?

Ở UC-03 chúng ta đã thống nhất:

> **Gắn khoản chi tiêu với mục đích / mục tiêu tài chính**

Tôi muốn phân biệt rõ:

**Category**

> Ăn uống, Di chuyển, Nhà ở, Giải trí...

→ dùng để **phân loại và thống kê**.

**Tag / Goal**

> Du lịch Đà Lạt, Mua laptop, Quỹ khẩn cấp...

→ dùng để **gắn ý nghĩa/mục đích cho giao dịch**.

Hai thứ này **không nên nhập làm một**, vì sau này phân tích sẽ rất khó.

Ví dụ:

```text
Chi tiêu: 500.000đ
│
├── Category: Ăn uống
│
└── Goal/Tag: Du lịch Đà Lạt
```

Như vậy hệ thống có thể trả lời cả:

> "Bạn đã chi bao nhiêu cho Ăn uống?"

và:

> "Bạn đã chi bao nhiêu cho mục tiêu Du lịch Đà Lạt?"

Đây sẽ rất hữu ích cho phần **phân tích và hỗ trợ quyết định** sau này.

---

### Tôi đề xuất chốt UC-04 theo phiên bản gọn:

```text
UC-04 Quản lý danh mục
│
├── UC-04.01 Xem danh sách danh mục
├── UC-04.02 Thêm danh mục tùy chỉnh
├── UC-04.03 Chỉnh sửa danh mục
└── UC-04.04 Xóa danh mục
```

Trong đó **Income/Expense là loại của Category**, còn **Tag/Financial Goal là khái niệm riêng**.

Nếu bạn đồng ý cấu trúc này, tôi sẽ **chốt UC-04** và chuyển sang **UC-05 — Quản lý ngân sách**, đây là một trong những UC quan trọng nhất của project vì nó bắt đầu chạm trực tiếp vào **Adaptive Budgeting**.

---



Đúng, tôi cũng nghĩ đây là cách **sạch hơn cả về nghiệp vụ lẫn thiết kế dữ liệu**.

Vậy ta chính thức **chốt UC-04**:

## UC-04 — Quản lý danh mục

```text
UC-04 Quản lý danh mục
│
├── UC-04.01 Xem danh sách danh mục
├── UC-04.02 Thêm danh mục tùy chỉnh
├── UC-04.03 Chỉnh sửa danh mục
└── UC-04.04 Xóa danh mục
```

### Nguyên tắc đã chốt

**Category (Danh mục)** dùng để trả lời:

> Khoản tiền này thuộc **nhóm chi/thu nào?**

Ví dụ:

```text
Ăn uống
Di chuyển
Nhà ở
Mua sắm
Giải trí
Lương
Thưởng
Đầu tư
```

**Tag / mục đích** sẽ là một khái niệm riêng, dùng để trả lời:

> Khoản tiền này **liên quan đến mục đích hoặc kế hoạch nào?**

Ví dụ:

```text
#DuLichDaLat
#MuaLaptop
#QuyKhanCap
#HocTap
```

Còn **Financial Goal** sẽ tiếp tục là một module riêng ở UC-06.

Như vậy một giao dịch có thể có cấu trúc kiểu:

```text
Khoản chi: 500.000đ
│
├── Category: Ăn uống
│
├── Tag: Du lịch Đà Lạt
│
└── Financial Goal: Chuyến du lịch Đà Lạt
```

Điều này sẽ rất có lợi cho phần phân tích sau này vì ta có thể phân tích **theo danh mục**, **theo tag**, hoặc **theo mục tiêu tài chính** mà không trộn lẫn ba khái niệm.

**UC-04: ĐÃ CHỐT.**

---

Tiếp theo chúng ta đến **UC-05 — Quản lý ngân sách**.

Đây là UC tôi muốn chúng ta làm kỹ nhất từ đầu, vì đây chính là nơi **Adaptive Budgeting** bắt đầu xuất hiện. Tôi sẽ không chỉ đưa CRUD ngân sách mà sẽ tách rõ:

**Tạo ngân sách → phân bổ → theo dõi → đánh giá → điều chỉnh**

để sau này không phải sửa lại UCD khi xây phần AI/phân tích.

---



Tốt. Ta vào **UC-05 — Quản lý ngân sách**. Đây là UC cốt lõi của project nên tôi sẽ thiết kế nó sao cho sau này ghép được với **phân tích, dự báo và Adaptive Budgeting** mà không phải đập lại cấu trúc.

# UC-05 — Quản lý ngân sách

Tôi đề xuất chia thành 6 UC con:

| Mã | Use Case | Trạng thái |
|---|---|---|
| **UC-05.01** | Tạo ngân sách | Chưa chốt |
| **UC-05.02** | Thiết lập ngân sách theo danh mục | Chưa chốt |
| **UC-05.03** | Xem ngân sách | Chưa chốt |
| **UC-05.04** | Theo dõi tình trạng ngân sách | Chưa chốt |
| **UC-05.05** | Điều chỉnh ngân sách | Chưa chốt |
| **UC-05.06** | Quản lý chu kỳ ngân sách | Chưa chốt |

Cấu trúc:

```text
UC-05 Quản lý ngân sách
│
├── UC-05.01 Tạo ngân sách
├── UC-05.02 Thiết lập ngân sách theo danh mục
├── UC-05.03 Xem ngân sách
├── UC-05.04 Theo dõi tình trạng ngân sách
├── UC-05.05 Điều chỉnh ngân sách
└── UC-05.06 Quản lý chu kỳ ngân sách
```

## 1. Tạo ngân sách

Ví dụ:

> Tháng 9/2026 → Ngân sách tổng 10.000.000đ.

Nhưng cần phân biệt **tạo Budget** với **phân bổ Budget**.

```text
Tạo ngân sách
      ↓
Thiết lập hạn mức
      ↓
Theo dõi
```

## 2. Thiết lập ngân sách theo danh mục

Ví dụ:

```text
Ngân sách tháng: 10.000.000đ

Ăn uống       2.500.000đ
Di chuyển     1.000.000đ
Nhà ở         3.000.000đ
Giải trí        500.000đ
Mua sắm       1.000.000đ
Khác          2.000.000đ
```

Điều này liên kết trực tiếp với **UC-04 Category**.

---

## 3. Xem ngân sách

Người dùng có thể xem:

- Tổng ngân sách.
- Hạn mức từng danh mục.
- Đã chi.
- Còn lại.
- Tỷ lệ đã sử dụng.

Ví dụ:

```text
Ăn uống
2.000.000 / 2.500.000
80%
```

---

## 4. Theo dõi tình trạng ngân sách

Đây là phần bắt đầu tạo giá trị cho project.

Không chỉ:

> "Đã chi 80%"

mà hệ thống cần có khả năng xác định trạng thái:

```text
Trong mức an toàn
       ↓
Có dấu hiệu tăng chi
       ↓
Có nguy cơ vượt ngân sách
       ↓
Đã vượt ngân sách
```

Tuy nhiên, **"đánh giá" và "dự báo" chuyên sâu** tôi vẫn muốn để dành cho UC-07/UC-08.

UC-05.04 chủ yếu là **theo dõi trạng thái hiện tại**.

---

## 5. Điều chỉnh ngân sách

Đây là UC rất quan trọng.

Ví dụ:

```text
Ăn uống
Ngân sách cũ: 2.000.000
       ↓
Điều chỉnh
       ↓
Ngân sách mới: 2.500.000
```

Hoặc phân bổ lại:

```text
Giải trí: -500.000
       ↓
Ăn uống: +500.000
```

Đây chính là điểm mà sau này **Recommendation Engine** có thể hỗ trợ:

```text
Hệ thống phát hiện:
Ăn uống có nguy cơ vượt

        ↓

Đề xuất:
Tăng ngân sách Ăn uống 500.000đ
và giảm ngân sách Giải trí 500.000đ

        ↓

User quyết định

        ↓

Điều chỉnh ngân sách
```

**Lưu ý:** `UC-05.05 Điều chỉnh ngân sách` là hành động của **User**. Hệ thống có thể đề xuất điều chỉnh ở UC hỗ trợ quyết định, nhưng **không tự ý thay đổi ngân sách**.

---

# 6. Quản lý chu kỳ ngân sách

Đây là phần tôi muốn giữ vì Adaptive Budgeting cần một khái niệm **Budget Cycle**.

Ví dụ:

```text
Chu kỳ tháng
01/09 → 30/09

Chu kỳ tiếp theo
01/10 → 31/10
```

Hoặc sau này có thể hỗ trợ:

- Hàng tuần.
- Hàng tháng.
- Hàng quý.
- Chu kỳ tùy chỉnh.

Chu kỳ này rất quan trọng vì các phân tích sau này sẽ cần trả lời:

> "Trong **chu kỳ hiện tại**, người dùng đã sử dụng bao nhiêu ngân sách và còn bao nhiêu thời gian?"

---

# Một điểm tôi muốn tránh

Ta **không nên đưa "AI tự động lập ngân sách" vào UC-05**.

Luồng của project nên là:

```text
Dữ liệu thu nhập
       +
Dữ liệu chi tiêu
       +
Lịch sử
       +
Mục tiêu
       ↓
Phân tích
       ↓
Đánh giá
       ↓
Dự báo
       ↓
Khuyến nghị
       ↓
USER QUYẾT ĐỊNH
       ↓
Điều chỉnh ngân sách
```

Như vậy quyền kiểm soát tài chính vẫn thuộc về User.

---

## Đề xuất chốt UC-05

Tôi nghiêng về việc **giữ cả 6 UC**:

```text
UC-05 Quản lý ngân sách
│
├── UC-05.01 Tạo ngân sách
├── UC-05.02 Thiết lập ngân sách theo danh mục
├── UC-05.03 Xem ngân sách
├── UC-05.04 Theo dõi tình trạng ngân sách
├── UC-05.05 Điều chỉnh ngân sách
└── UC-05.06 Quản lý chu kỳ ngân sách
```

Nhưng trước khi đóng UC-05, có **một quyết định quan trọng**:

### Ngân sách ban đầu được tạo như thế nào?

Tôi đề xuất cho phép **2 cách**:

**Cách 1 — Người dùng tự thiết lập**

```text
User nhập:
Thu nhập dự kiến = 15 triệu

→ Tự đặt ngân sách từng danh mục
```

**Cách 2 — Hệ thống đề xuất**

```text
Thu nhập
+ Lịch sử chi tiêu
+ Mục tiêu tài chính
+ Chi phí cố định
       ↓
Hệ thống đề xuất ngân sách
       ↓
User xem xét
       ↓
User xác nhận
```

Cách 2 rất hợp với **điểm khác biệt của project**, nhưng tôi đề nghị **không biến nó thành chức năng của UC-05**. Nó sẽ thuộc nhóm **Phân tích / Hỗ trợ quyết định**, rồi kết quả được đưa sang UC-05 để User xác nhận.

Nếu bạn đồng ý mô hình này, ta sẽ **chốt UC-05**, rồi sang **UC-06 — Quản lý mục tiêu tài chính**.

---



Đúng. Cách làm hiện tại nên xem là **baseline để thiết kế**, sau này khi khảo sát người dùng và nghiên cứu thêm thì ta có thể điều chỉnh. Quan trọng là lúc đó sửa có hệ thống, không làm rối toàn bộ UCD.

Ta sang **UC-06 — Quản lý mục tiêu tài chính**.

# UC-06 — Quản lý mục tiêu tài chính

UC này giúp người dùng xác định những mục tiêu tài chính cụ thể và theo dõi tiến độ đạt mục tiêu.

Ví dụ:

- Quỹ khẩn cấp: 30.000.000đ
- Mua laptop: 25.000.000đ
- Du lịch: 10.000.000đ
- Học tập: 15.000.000đ

Tôi đề xuất cấu trúc:

| Mã | Use Case | Trạng thái |
|---|---|---|
| **UC-06.01** | Tạo mục tiêu tài chính | Chưa chốt |
| **UC-06.02** | Xem danh sách mục tiêu | Chưa chốt |
| **UC-06.03** | Xem chi tiết mục tiêu | Chưa chốt |
| **UC-06.04** | Chỉnh sửa mục tiêu | Chưa chốt |
| **UC-06.05** | Xóa / hủy mục tiêu | Chưa chốt |
| **UC-06.06** | Theo dõi tiến độ mục tiêu | Chưa chốt |
| **UC-06.07** | Ghi nhận khoản tiền đóng góp cho mục tiêu | Chưa chốt |
| **UC-06.08** | Hoàn thành mục tiêu | Chưa chốt |

Cấu trúc:

```text
UC-06 Quản lý mục tiêu tài chính
│
├── UC-06.01 Tạo mục tiêu tài chính
├── UC-06.02 Xem danh sách mục tiêu
├── UC-06.03 Xem chi tiết mục tiêu
├── UC-06.04 Chỉnh sửa mục tiêu
├── UC-06.05 Xóa / hủy mục tiêu
├── UC-06.06 Theo dõi tiến độ mục tiêu
├── UC-06.07 Ghi nhận khoản tiền đóng góp cho mục tiêu
└── UC-06.08 Hoàn thành mục tiêu
```

## Có một điểm cần thiết kế kỹ: "đóng góp cho mục tiêu"

Tôi **không muốn tạo một loại giao dịch tài chính hoàn toàn riêng** cho việc này.

Ví dụ người dùng có:

> Mục tiêu mua laptop — 20 triệu.

Sau đó tiết kiệm được 2 triệu.

Ta có thể ghi nhận:

```text
Mục tiêu mua laptop
20.000.000đ
      │
      ├── Đã tích lũy: 5.000.000đ
      │
      └── Còn thiếu: 15.000.000đ
```

Nhưng về mặt hệ thống, khoản tiền đó có thể liên quan đến **thu nhập, chi tiêu, tài khoản/ví tiền và mục tiêu** tùy kiến trúc tài chính mà ta chọn.

Đây là vấn đề tôi nghĩ **chưa nên khóa quá sớm**.

---

## Một điểm khác: mục tiêu có thời hạn

Tôi đề xuất một Goal nên có tối thiểu:

```text
Financial Goal
├── Tên mục tiêu
├── Số tiền mục tiêu
├── Số tiền hiện tại
├── Ngày bắt đầu
├── Hạn hoàn thành
├── Trạng thái
└── Mô tả
```

Ví dụ:

```text
Mua laptop
Mục tiêu: 25.000.000đ
Đã có:    10.000.000đ
Còn thiếu:15.000.000đ

Bắt đầu: 01/09/2026
Hạn:     01/03/2027

Tiến độ: 40%
```

Thông tin **hạn hoàn thành** đặc biệt hữu ích về sau.

Hệ thống có thể phân tích:

```text
Mục tiêu
   +
Số tiền còn thiếu
   +
Thời gian còn lại
   +
Thu nhập
   +
Chi tiêu
   ↓
Khả năng đạt mục tiêu
```

Và từ đó hỗ trợ quyết định:

> "Để đạt mục tiêu này đúng hạn, người dùng nên dành khoảng X đồng mỗi tháng."

Đây chính là điểm mà **Financial Goal** có thể kết nối với **Adaptive Budgeting**.

---

# Tôi đề xuất chốt UC-06 theo cấu trúc này

```text
UC-06 Quản lý mục tiêu tài chính
│
├── UC-06.01 Tạo mục tiêu tài chính
├── UC-06.02 Xem danh sách mục tiêu
├── UC-06.03 Xem chi tiết mục tiêu
├── UC-06.04 Chỉnh sửa mục tiêu
├── UC-06.05 Xóa / hủy mục tiêu
├── UC-06.06 Theo dõi tiến độ mục tiêu
├── UC-06.07 Ghi nhận khoản tiền đóng góp cho mục tiêu
└── UC-06.08 Hoàn thành mục tiêu
```

Tôi chỉ muốn bạn xác nhận **một điểm trước khi chốt**:

**UC-06.07 có giữ không?**

Tôi nghiêng về **giữ**, bởi nếu không có cơ chế ghi nhận tiền đóng góp thì `Theo dõi tiến độ mục tiêu` sẽ phải lấy dữ liệu từ một cơ chế khác, trong khi hiện tại ta chưa xác định cơ chế đó.

Nếu bạn đồng ý **giữ cả 8 UC**, ta chốt UC-06 và chuyển sang **UC-07 — Phân tích tài chính**.

---



Được. Ta sang **UC-07 — Phân tích tài chính**.

Đây là UC bắt đầu chuyển hệ thống từ một ứng dụng **ghi chép chi tiêu** thành hệ thống **phân tích và hỗ trợ ra quyết định**. Vì vậy tôi nghĩ cần tách rõ **phân tích dữ liệu hiện tại** khỏi **dự báo** và **khuyến nghị**. Nếu trộn cả ba vào UC-07 thì sau này UCD sẽ bị chồng chéo.

# UC-07 — Phân tích tài chính

Mục tiêu:

> Cung cấp cho người dùng thông tin tổng hợp và phân tích về tình hình thu nhập, chi tiêu, cơ cấu chi tiêu và xu hướng tài chính dựa trên dữ liệu đã ghi nhận.

Tôi đề xuất:

| Mã | Use Case | Trạng thái |
|---|---|---|
| **UC-07.01** | Xem tổng quan tài chính | Chưa chốt |
| **UC-07.02** | Phân tích thu nhập | Chưa chốt |
| **UC-07.03** | Phân tích chi tiêu | Chưa chốt |
| **UC-07.04** | Phân tích theo danh mục | Chưa chốt |
| **UC-07.05** | Phân tích theo thời gian | Chưa chốt |
| **UC-07.06** | Phân tích xu hướng chi tiêu | Chưa chốt |
| **UC-07.07** | Đánh giá tình trạng ngân sách | Chưa chốt |
| **UC-07.08** | Phân tích tiến độ mục tiêu tài chính | Chưa chốt |

Cấu trúc:

```text id="p7t5qk"
UC-07 Phân tích tài chính
│
├── UC-07.01 Xem tổng quan tài chính
├── UC-07.02 Phân tích thu nhập
├── UC-07.03 Phân tích chi tiêu
├── UC-07.04 Phân tích theo danh mục
├── UC-07.05 Phân tích theo thời gian
├── UC-07.06 Phân tích xu hướng chi tiêu
├── UC-07.07 Đánh giá tình trạng ngân sách
└── UC-07.08 Phân tích tiến độ mục tiêu tài chính
```

## 1. Xem tổng quan tài chính

Đây là dashboard ở mức cao:

```text
Thu nhập
Chi tiêu
Số dư
Tỷ lệ tiết kiệm
Ngân sách đã sử dụng
Mục tiêu tài chính
```

Ví dụ:

```text
Thu nhập tháng:       15.000.000đ
Chi tiêu:               9.200.000đ
Còn lại:                5.800.000đ
Tỷ lệ tiết kiệm:              38,7%
Ngân sách đã sử dụng:         72%
```

UC này chủ yếu **hiển thị thông tin tổng hợp**, không nhất thiết đưa ra dự đoán hay lời khuyên.

---

## 2. Phân tích thu nhập

Cho phép phân tích:

- Tổng thu nhập.
- Nguồn thu nhập.
- Tỷ trọng từng nguồn.
- Biến động thu nhập theo thời gian.
- Thu nhập định kỳ và không định kỳ.

Ví dụ:

```text
Lương        12.000.000đ — 80%
Thưởng        2.000.000đ — 13%
Khác          1.000.000đ —  7%
```

Dữ liệu này lấy trực tiếp từ **UC-02**.

---

## 3. Phân tích chi tiêu

Phân tích:

- Tổng chi tiêu.
- Chi tiêu trung bình.
- Chi tiêu theo thời gian.
- Chi tiêu theo danh mục.
- Chi tiêu theo tag/mục đích.

Ví dụ:

```text
Ăn uống       2.500.000đ
Nhà ở         3.000.000đ
Di chuyển     1.000.000đ
Giải trí        800.000đ
Khác          1.900.000đ
```

---

## 4. Phân tích theo danh mục

UC này tập trung vào:

> **Tiền đang được sử dụng vào đâu?**

Ví dụ hệ thống có thể phát hiện:

> Chi tiêu cho Ăn uống chiếm 27% tổng chi tiêu trong 3 tháng gần nhất.

Đây là dữ liệu rất quan trọng cho **Budgeting**.

---

## 5. Phân tích theo thời gian

Cho phép xem dữ liệu theo:

- Ngày.
- Tuần.
- Tháng.
- Quý.
- Khoảng thời gian tùy chọn.

Ví dụ:

```text
Tháng 6 → 8.2 triệu
Tháng 7 → 9.1 triệu
Tháng 8 → 10.3 triệu
```

---

## 6. Phân tích xu hướng chi tiêu

Đây là phần có giá trị hơn việc chỉ vẽ biểu đồ.

Hệ thống có thể phát hiện:

```text
Chi tiêu ăn uống
Tháng 6 → 1.8 triệu
Tháng 7 → 2.1 triệu
Tháng 8 → 2.6 triệu

→ Xu hướng tăng
```

Nhưng tôi đề nghị **chưa gọi đây là "dự báo"**.

Phân biệt:

**Phân tích xu hướng:**

> "Chi tiêu của bạn đang tăng."

**Dự báo:**

> "Nếu xu hướng hiện tại tiếp tục, cuối tháng bạn có khả năng chi khoảng 3,2 triệu."

Hai cái này sẽ thuộc **UC-07 và UC-08** tương ứng.

---

# 7. Đánh giá tình trạng ngân sách

Đây là UC quan trọng.

Ví dụ:

```text
Ngân sách Ăn uống: 2.500.000đ

Đã chi: 2.000.000đ
Đã sử dụng: 80%

Ngày còn lại: 10 ngày
```

Hệ thống có thể xác định:

```text
Trạng thái hiện tại:
Đang sử dụng cao
```

Nhưng **không nên ở đây đưa ra khuyến nghị kiểu**:

> "Bạn nên giảm ăn uống 20%."

Khuyến nghị sẽ thuộc UC-09.

Như vậy:

```text
UC-07
   ↓
Đánh giá
   ↓
UC-08
Dự báo
   ↓
UC-09
Khuyến nghị
```

---

# 8. Phân tích tiến độ mục tiêu tài chính

Ví dụ:

```text
Mục tiêu: Mua laptop

Mục tiêu:       25 triệu
Đã tích lũy:    10 triệu
Tiến độ:        40%
Thời gian còn:  4 tháng
```

Hệ thống có thể phân tích:

> Người dùng đang đạt 40% mục tiêu.

Nhưng câu:

> "Bạn cần tiết kiệm 3,75 triệu/tháng để đạt mục tiêu"

sẽ thuộc phần **dự báo / hỗ trợ quyết định**, không nên nhét vào UC-07.

---

# Có một vấn đề tôi muốn chúng ta đặc biệt lưu ý

Một số UC ở đây có thể **không nhất thiết phải là UC độc lập khi vẽ UCD**.

Ví dụ:

```text
Phân tích theo danh mục
Phân tích theo thời gian
```

về bản chất có thể chỉ là **các cách xem dữ liệu của "Phân tích chi tiêu"**.

Nếu chúng ta tách quá nhỏ, UCD sẽ trở thành một đống oval và rất khó đọc.

Vì vậy tôi đề xuất **giữ chúng trong đặc tả chức năng**, nhưng khi vẽ UCD cấp phân cấp có thể gom:

```text
UC-07 Phân tích tài chính
│
├── Xem tổng quan tài chính
├── Phân tích thu nhập
├── Phân tích chi tiêu
│    ├── Theo danh mục
│    ├── Theo thời gian
│    └── Theo xu hướng
├── Đánh giá tình trạng ngân sách
└── Phân tích tiến độ mục tiêu
```

Tức là **không nhất thiết mỗi dòng đều phải trở thành một oval riêng**.

---

## Tôi đề xuất chốt UC-07 theo cấu trúc này

```text
UC-07 Phân tích tài chính
│
├── UC-07.01 Xem tổng quan tài chính
├── UC-07.02 Phân tích thu nhập
├── UC-07.03 Phân tích chi tiêu
│    ├── Theo danh mục
│    ├── Theo thời gian
│    └── Theo xu hướng
├── UC-07.04 Đánh giá tình trạng ngân sách
└── UC-07.05 Phân tích tiến độ mục tiêu tài chính
```

Tôi **nghiêng về cấu trúc rút gọn này** vì nó tránh over-modeling và vẫn giữ đầy đủ nghiệp vụ.

Nếu bạn đồng ý, **UC-07 sẽ được chốt theo phiên bản rút gọn**, sau đó ta sang **UC-08 — Dự báo chi tiêu và rủi ro vượt ngân sách**. Đây sẽ là UC bắt đầu thể hiện rõ phần "thông minh" của project.

---



Chính xác. Ta sẽ giữ **UCD ở mức nghiệp vụ**, còn khi sang đặc tả từng UC thì có thể phát sinh thêm UC con và quan hệ `<<include>>`, `<<extend>>`, generalization nếu có cơ sở. Không cần ép toàn bộ chi tiết vào UCD tổng quát ngay từ đầu.

Vậy **UC-07 đã chốt**, ta sang phần tiếp theo.

# UC-08 — Dự báo chi tiêu và rủi ro

Đây là UC mà tôi cho rằng nên thiết kế khá cẩn thận, vì nó là cầu nối:

**Dữ liệu lịch sử → Phân tích hiện tại → Dự báo tương lai → Hỗ trợ quyết định**

## 1. Mục tiêu

Hệ thống sử dụng dữ liệu tài chính đã ghi nhận để **ước tính xu hướng chi tiêu trong tương lai và phát hiện nguy cơ vượt ngân sách**, từ đó cung cấp dữ liệu đầu vào cho hệ thống khuyến nghị.

Ví dụ:

> Người dùng đã sử dụng 75% ngân sách Ăn uống nhưng chu kỳ còn 12 ngày. Với tốc độ chi tiêu hiện tại, hệ thống dự báo cuối kỳ có thể đạt 115% ngân sách.

Đây mới là phần "thông minh" mà project của chúng ta muốn hướng tới.

---

## 2. Các UC con đề xuất

| Mã | Use Case | Trạng thái |
|---|---|---|
| **UC-08.01** | Dự báo chi tiêu cuối chu kỳ | Chưa chốt |
| **UC-08.02** | Dự báo chi tiêu theo danh mục | Chưa chốt |
| **UC-08.03** | Dự báo nguy cơ vượt ngân sách | Chưa chốt |
| **UC-08.04** | Đánh giá mức độ rủi ro | Chưa chốt |
| **UC-08.05** | Dự báo khả năng đạt mục tiêu tài chính | Chưa chốt |

Cấu trúc:

```text
UC-08 Dự báo chi tiêu và rủi ro
│
├── UC-08.01 Dự báo chi tiêu cuối chu kỳ
├── UC-08.02 Dự báo chi tiêu theo danh mục
├── UC-08.03 Dự báo nguy cơ vượt ngân sách
├── UC-08.04 Đánh giá mức độ rủi ro
└── UC-08.05 Dự báo khả năng đạt mục tiêu tài chính
```

## 3. Tôi đề xuất phân biệt 3 tầng

### Tầng 1 — Forecast

Hệ thống dự đoán:

> **"Cuối kỳ có thể chi bao nhiêu?"**

Ví dụ:

```text
Ngân sách:       3.000.000đ
Đã chi:          2.100.000đ
Ngày còn lại:            10

Dự báo cuối kỳ:
≈ 3.400.000đ
```

### Tầng 2 — Risk

Sau khi có forecast:

```text
Dự báo:      3.400.000đ
Ngân sách:   3.000.000đ

→ Có nguy cơ vượt ngân sách
```

### Tầng 3 — Decision Support

Sau đó mới tới:

> "Nên làm gì?"

Ví dụ:

> Giảm chi tiêu ăn uống trong 10 ngày còn lại.

**Phần này không thuộc UC-08**, mà sẽ sang **UC-09 — Hỗ trợ quyết định**.

---

# 4. Có nên dùng AI không?

**Có thể, nhưng không nên định nghĩa UC là "Dùng AI để dự báo".**

UC nên mô tả:

> **Dự báo chi tiêu**

Còn bên trong implementation chúng ta có thể thử:

```text
Phương pháp thống kê
        hoặc
Machine Learning
        hoặc
AI/LLM
        hoặc
Hybrid
```

Điều này cực kỳ quan trọng cho đồ án.

Nếu sau khi nghiên cứu mà chúng ta nhận ra mô hình thống kê đơn giản đã đủ tốt, ta **không bị mắc kẹt vì UCD đã ghi "AI Forecasting"**.

AI là **phương pháp triển khai**, còn Forecasting là **nghiệp vụ**.

---

# 5. Một vấn đề tôi muốn giữ mở

**UC-08.05 — Dự báo khả năng đạt mục tiêu tài chính**

Nó rất thú vị:

```text
Mục tiêu: 30 triệu
Đã có:    10 triệu
Còn thiếu:20 triệu

Thời gian còn lại: 5 tháng

→ Cần tiết kiệm: 4 triệu/tháng
```

Sau đó hệ thống có thể phân tích:

> Với thu nhập và mức chi tiêu hiện tại, khả năng đạt mục tiêu đúng hạn là thấp/trung bình/cao.

Điều này kết nối rất đẹp:

```text
Income
+
Expense
+
Financial Goal
+
Budget
+
Historical Data
        ↓
Forecast
        ↓
Decision Support
```

Tuy nhiên nó cũng làm phần dự báo lớn hơn.

**Tôi vẫn nghiêng về giữ**, vì mục tiêu tài chính là một trong những dữ liệu đầu vào mà chúng ta đã xác định ngay từ Chương 1.

---

# 6. Cấu trúc tôi đề xuất chốt

```text
UC-08 Dự báo chi tiêu và rủi ro
│
├── UC-08.01 Dự báo chi tiêu cuối chu kỳ
├── UC-08.02 Dự báo chi tiêu theo danh mục
├── UC-08.03 Dự báo nguy cơ vượt ngân sách
├── UC-08.04 Đánh giá mức độ rủi ro
└── UC-08.05 Dự báo khả năng đạt mục tiêu tài chính
```

Trong đó:

```text
UC-08.01 + UC-08.02
        ↓
Dự báo
        ↓
UC-08.03
Nguy cơ vượt ngân sách
        ↓
UC-08.04
Mức độ rủi ro
        ↓
UC-09
Hỗ trợ quyết định
```

Đây là **quan hệ logic nghiệp vụ**, chưa phải tuyên bố rằng ta sẽ vẽ các mũi tên tuần tự này trong UCD.

---

Nếu bạn đồng ý giữ cấu trúc này, ta sẽ **chốt UC-08** và chuyển sang **UC-09 — Hỗ trợ quyết định**, cũng chính là UC cuối cùng và là nơi chúng ta sẽ làm rõ nhất phần **khuyến nghị + điều chỉnh ngân sách thích ứng**.

---



Được. Ta sang **UC-09 — Hỗ trợ quyết định tài chính**. Đây là UC cuối cùng trong bộ UC cấp cao hiện tại và cũng là nơi thể hiện rõ nhất hướng **Adaptive Budgeting** của project.

# UC-09 — Hỗ trợ quyết định tài chính

Mục tiêu của UC này không phải để hệ thống "tự quyết định thay người dùng", mà là:

> **Phân tích kết quả và đưa ra các khuyến nghị có căn cứ để người dùng lựa chọn và điều chỉnh kế hoạch tài chính.**

Luồng tư duy:

```text
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
USER QUYẾT ĐỊNH
      ↓
Điều chỉnh kế hoạch
```

---

## 1. Các UC con đề xuất

| Mã | Use Case | Trạng thái |
|---|---|---|
| **UC-09.01** | Nhận khuyến nghị tài chính | Chưa chốt |
| **UC-09.02** | Xem chi tiết khuyến nghị | Chưa chốt |
| **UC-09.03** | Xem nguyên nhân / cơ sở khuyến nghị | Chưa chốt |
| **UC-09.04** | Xem phương án điều chỉnh | Chưa chốt |
| **UC-09.05** | Xác nhận điều chỉnh ngân sách | Chưa chốt |
| **UC-09.06** | Từ chối / bỏ qua khuyến nghị | Chưa chốt |
| **UC-09.07** | Theo dõi kết quả sau điều chỉnh | Chưa chốt |

Cấu trúc ban đầu:

```text id="6q1a8f"
UC-09 Hỗ trợ quyết định tài chính
│
├── UC-09.01 Nhận khuyến nghị tài chính
├── UC-09.02 Xem chi tiết khuyến nghị
├── UC-09.03 Xem nguyên nhân / cơ sở khuyến nghị
├── UC-09.04 Xem phương án điều chỉnh
├── UC-09.05 Xác nhận điều chỉnh ngân sách
├── UC-09.06 Từ chối / bỏ qua khuyến nghị
└── UC-09.07 Theo dõi kết quả sau điều chỉnh
```

---

# 2. Điểm quan trọng nhất: khuyến nghị phải giải thích được

Tôi rất muốn chúng ta giữ **UC-09.03**.

Ví dụ hệ thống không nên chỉ nói:

> **"Bạn nên giảm chi tiêu ăn uống."**

Mà nên cho người dùng biết:

> Chi tiêu ăn uống hiện tại cao hơn mức trung bình 3 tháng gần nhất 18%. Ngân sách đã sử dụng 82% trong khi chu kỳ còn 10 ngày. Với tốc độ hiện tại, chi tiêu cuối kỳ có khả năng vượt ngân sách.

Sau đó mới:

> **Đề xuất giảm chi tiêu ăn uống trong những ngày còn lại.**

Như vậy hệ thống có:

```text id="o0z5m2"
Prediction
     ↓
Reasoning / Evidence
     ↓
Recommendation
```

Đây là điểm rất quan trọng nếu sau này chúng ta sử dụng AI.

---

# 3. UC-09.04 — Xem phương án điều chỉnh

Đây cũng là phần tôi muốn giữ.

Hệ thống không nhất thiết chỉ đưa ra **một lời khuyên**.

Ví dụ:

```text id="c8b2eg"
Ngân sách hiện tại

Ăn uống:      3.000.000đ
Giải trí:     1.500.000đ

      ↓

Phương án A
Giảm Ăn uống 300.000đ

Phương án B
Giảm Giải trí 500.000đ
Tăng Ăn uống 200.000đ

Phương án C
Giữ nguyên ngân sách
và giảm tốc độ chi tiêu
```

User tự chọn.

Điều này phù hợp với nguyên tắc:

> **Hệ thống hỗ trợ quyết định, không thay thế quyết định của người dùng.**

---

# 4. UC-09.05 — Xác nhận điều chỉnh ngân sách

Đây là điểm kết nối với **UC-05**.

Ví dụ:

```text id="x0z7w7"
Khuyến nghị
    ↓
User xem
    ↓
User chọn phương án
    ↓
Xác nhận
    ↓
UC-05.05 Điều chỉnh ngân sách
```

Ở đây có khả năng sử dụng quan hệ:

```text id="7t1h1x"
UC-09.04
   │
   └── <<include>> ──> UC-09.05
```

hoặc khi đi sâu vào đặc tả, chúng ta có thể xác định:

```text id="9v84kw"
UC-09 Hỗ trợ quyết định
        │
        └── <<include>>
                  ↓
       UC-05.05 Điều chỉnh ngân sách
```

**Nhưng tôi chưa muốn chốt quan hệ UML này ngay.**

Ta sẽ xác định chính xác `include/extend` khi viết đặc tả UC, vì lúc đó mới có đủ điều kiện để quyết định quan hệ nào thực sự phù hợp.

---

# 5. UC-09.06 — Từ chối / bỏ qua khuyến nghị

Tôi nghĩ nên giữ.

Một hệ thống hỗ trợ quyết định tốt phải cho phép:

```text
Khuyến nghị
   ↓
User
 ┌─┴──────────┐
 ↓            ↓
Chấp nhận    Bỏ qua
```

Không thể giả định rằng mọi khuyến nghị đều được áp dụng.

Việc lưu lại hành động này cũng có thể hữu ích cho phân tích sau này:

> Người dùng thường chấp nhận hay bỏ qua loại khuyến nghị nào?

Tuy nhiên **lưu lịch sử khuyến nghị** là vấn đề thiết kế dữ liệu, chưa nhất thiết biến thành một UC riêng.

---

# 6. UC-09.07 — Theo dõi kết quả sau điều chỉnh

Đây là phần khá thú vị.

Ví dụ:

```text id="1o4b89"
Khuyến nghị:
Giảm chi tiêu ăn uống

        ↓

User áp dụng

        ↓

Sau 10 ngày

        ↓

Chi tiêu thực tế giảm 15%
```

Hệ thống có thể đánh giá:

> Điều chỉnh đã giúp giảm tốc độ chi tiêu so với trước đó.

Nó tạo thành một vòng lặp:

```text id="m1g0k9"
Analyze
   ↓
Predict
   ↓
Recommend
   ↓
User Decision
   ↓
Adjust
   ↓
Observe Result
   ↓
Analyze again
```

Đây mới thực sự là tinh thần **Adaptive**.

---

# 7. Có nên gọi UC này là "AI khuyến nghị" không?

**Không.**

Tôi đề xuất giữ tên:

> **UC-09 — Hỗ trợ quyết định tài chính**

Thay vì:

> ~~AI đưa ra khuyến nghị~~

Bởi vì:

**Nghiệp vụ:**

> Hệ thống đưa ra khuyến nghị dựa trên dữ liệu.

**Công nghệ:**

> Có thể sử dụng AI/ML/rule-based/hybrid để tạo khuyến nghị.

Nếu sau này nghiên cứu cho thấy Rule Engine hiệu quả hơn LLM ở một số trường hợp, UCD vẫn không cần thay đổi.

---

# 8. Cấu trúc UC-09 tôi đề xuất chốt

```text id="6tq0y6"
UC-09 Hỗ trợ quyết định tài chính
│
├── UC-09.01 Nhận khuyến nghị tài chính
├── UC-09.02 Xem chi tiết khuyến nghị
├── UC-09.03 Xem nguyên nhân / cơ sở khuyến nghị
├── UC-09.04 Xem phương án điều chỉnh
├── UC-09.05 Xác nhận điều chỉnh ngân sách
├── UC-09.06 Từ chối / bỏ qua khuyến nghị
└── UC-09.07 Theo dõi kết quả sau điều chỉnh
```

Và toàn bộ hệ thống hiện tại đã hình thành được chuỗi:

```text id="f34qkm"
UC-02 Quản lý thu nhập
              │
UC-03 Quản lý chi tiêu
              │
UC-04 Quản lý danh mục
              │
UC-06 Mục tiêu tài chính
              │
              ▼
      UC-07 Phân tích tài chính
              │
              ▼
   UC-08 Dự báo & đánh giá rủi ro
              │
              ▼
   UC-09 Hỗ trợ quyết định
              │
              ▼
       UC-05.05 Điều chỉnh
          ngân sách
              │
              ▼
       Dữ liệu mới được
       tiếp tục phân tích
```

Đây là **xương sống nghiệp vụ** của project.

**Nếu bạn đồng ý cấu trúc trên, ta sẽ chốt UC-09.** Sau đó tôi đề xuất chúng ta **không vẽ ngay**, mà làm thêm một bước rất quan trọng: tôi sẽ tổng hợp **UC-01 → UC-09 thành một bảng master**, rà lại xem có UC nào trùng, thiếu, sai cấp hoặc có quan hệ `include/extend` tiềm năng trước khi đóng gói thành Markdown. Điều này sẽ giúp bộ UCD cuối cùng sạch hơn rất nhiều.

---



Được. Tôi sẽ **tổng rà soát toàn bộ UC-01 → UC-09**, kiểm tra tính nhất quán giữa các UC, actor, include/extend và luồng nghiệp vụ trước khi chúng ta xuất Markdown.

Tôi sẽ **chưa xuất file ngay**. Trước hết tôi sẽ chốt lại bản master để bạn có thể đưa các lưu ý/yêu cầu của mình vào bước cuối.

### Bản master hiện tại

| Mã | Use Case cấp cao | Trạng thái |
|---|---|---|
| UC-01 | Quản lý tài khoản | Đã chốt |
| UC-02 | Quản lý thu nhập | Đã chốt |
| UC-03 | Quản lý chi tiêu | Đã chốt |
| UC-04 | Quản lý danh mục | Đã chốt |
| UC-05 | Quản lý ngân sách | Đã chấp thuận |
| UC-06 | Quản lý mục tiêu tài chính | Đã chấp thuận theo cấu trúc đã thống nhất |
| UC-07 | Phân tích tài chính | **Đã chốt** |
| UC-08 | Dự báo chi tiêu và rủi ro | **Đã chấp thuận** |
| UC-09 | Hỗ trợ quyết định tài chính | **Đang chờ bạn chốt lần cuối** |

### Chuỗi nghiệp vụ cốt lõi

**Người dùng → Dữ liệu tài chính → Phân tích → Đánh giá ngân sách → Dự báo → Khuyến nghị → Người dùng quyết định → Điều chỉnh ngân sách → Theo dõi kết quả → Phân tích lại**

Đây sẽ là "xương sống" để sau này các UCD, Activity Diagram và đặc tả Use Case không bị rời rạc.

### Một số nguyên tắc tôi đã khóa

- **User** là actor chính.
- Persona như sinh viên, người mới đi làm... **không tách thành actor riêng**.
- AI/OCR nếu là module nội bộ thì **không phải actor**.
- Camera không mặc định là actor.
- OCR Service / AI Service / Notification Service chỉ trở thành actor nếu hệ thống thực sự tích hợp với dịch vụ bên ngoài.
- **Category ≠ Tag/Purpose ≠ Financial Goal**.
- Phân tích, dự báo và khuyến nghị là **ba lớp chức năng khác nhau**.
- Hệ thống **không tự ý thay đổi ngân sách**; người dùng phải xác nhận.
- Không đưa chữ "AI" vào tên UC nghiệp vụ chỉ vì bên trong dùng AI.
- UCD không biểu diễn thứ tự thực thi bằng mũi tên tuần tự; thứ tự nghiệp vụ sẽ dành cho Activity Diagram/Use Case Specification.
- `include`, `extend`, `generalization` chỉ đưa vào khi có quan hệ UML thực sự phù hợp, không dùng cho đẹp sơ đồ.

Đặc biệt, tôi sẽ kiểm tra UC-09 thật kỹ vì đây là phần tạo nên **điểm khác biệt của hệ thống**:  
**Analyze → Predict → Recommend → User Decision → Adaptive Adjustment**.

Bạn cứ gửi **toàn bộ các lưu ý và yêu cầu trước khi xuất file**. Sau khi nhận được, tôi sẽ cập nhật bản master lần cuối rồi mới tạo file Markdown cho bên khác dùng để vẽ UCD.

---



Có. Với những gì chúng ta đã chốt, tôi **không khuyên làm một UCD duy nhất chứa toàn bộ Use Case**. Đồ án này nên có **1 UCD tổng quát + các UCD phân cấp theo nhóm nghiệp vụ**.

Điểm quan trọng là UCD phải thể hiện được sự chuyển dịch của hệ thống:

> **Quản lý dữ liệu → Theo dõi → Phân tích → Dự báo → Khuyến nghị → Người dùng quyết định → Điều chỉnh ngân sách**

chứ không chỉ là một đống CRUD.

---

# 1. UCD tổng quát của toàn hệ thống

Ở mức cao nhất, System Boundary là:

```text
┌─────────────────────────────────────────────────────────────┐
│       SMART EXPENSE & BUDGET MANAGER                       │
│                                                             │
│  (Quản lý tài khoản)                                       │
│  (Quản lý thu nhập)                                        │
│  (Quản lý giao dịch chi tiêu)                              │
│  (Quản lý danh mục)                                        │
│  (Quản lý ngân sách)                                       │
│  (Theo dõi ngân sách)                                      │
│  (Quản lý mục tiêu tài chính)                              │
│  (Phân tích tài chính)                                     │
│  (Đánh giá tình trạng ngân sách)                           │
│  (Dự báo nguy cơ vượt ngân sách)                           │
│  (Nhận khuyến nghị tài chính)                              │
│  (Điều chỉnh ngân sách)                                    │
│                                                             │
└─────────────────────────────────────────────────────────────┘
          ↑
          │
       User
```

Nhưng tôi muốn chúng ta làm nó **đẹp hơn về mặt UML**, bằng cách nhóm các Use Case.

---

# 2. UCD tổng quát đề xuất

Tôi đề xuất Actor và Use Case cấp 1 như sau:

```text
                            ┌──────────────────────────────────────┐
                            │ SMART EXPENSE & BUDGET MANAGER       │
                            │                                      │
                            │                                      │
                            │   ┌──────────────────────────────┐   │
                            │   │ Quản lý tài khoản            │   │
                            │   └──────────────────────────────┘   │
                            │                                      │
                            │   ┌──────────────────────────────┐   │
                            │   │ Quản lý thu nhập              │   │
                            │   └──────────────────────────────┘   │
                            │                                      │
                            │   ┌──────────────────────────────┐   │
                            │   │ Quản lý chi tiêu              │   │
                            │   └──────────────────────────────┘   │
                            │                                      │
                            │   ┌──────────────────────────────┐   │
                            │   │ Quản lý ngân sách             │   │
                            │   └──────────────────────────────┘   │
                            │                                      │
                            │   ┌──────────────────────────────┐   │
                            │   │ Quản lý mục tiêu tài chính    │   │
                            │   └──────────────────────────────┘   │
                            │                                      │
                            │   ┌──────────────────────────────┐   │
                            │   │ Phân tích tài chính           │   │
                            │   └──────────────────────────────┘   │
                            │                                      │
                            │   ┌──────────────────────────────┐   │
                            │   │ Đánh giá ngân sách            │   │
                            │   └──────────────────────────────┘   │
                            │                                      │
                            │   ┌──────────────────────────────┐   │
                            │   │ Dự báo chi tiêu / rủi ro      │   │
                            │   └──────────────────────────────┘   │
                            │                                      │
                            │   ┌──────────────────────────────┐   │
                            │   │ Nhận khuyến nghị              │   │
                            │   └──────────────────────────────┘   │
                            │                                      │
                            │   ┌──────────────────────────────┐   │
                            │   │ Điều chỉnh ngân sách          │   │
                            │   └──────────────────────────────┘   │
                            │                                      │
                            └──────────────────────────────────────┘
                                      ↑
                                      │
                                    User
```

Ở đây **User là Actor chính duy nhất ở cấp tổng quát**.

Tôi cho rằng đây là cách an toàn nhất cho đồ án hiện tại.

---

# 3. Nhưng UCD này vẫn chưa thể hiện rõ "cái mới"

Chúng ta nên làm nổi bật chuỗi nghiệp vụ đặc trưng:

```text
                    ┌───────────────────┐
                    │ Quản lý dữ liệu   │
                    └─────────┬─────────┘
                              ↓
                    ┌───────────────────┐
                    │ Phân tích         │
                    └─────────┬─────────┘
                              ↓
                    ┌───────────────────┐
                    │ Đánh giá ngân sách│
                    └─────────┬─────────┘
                              ↓
                    ┌───────────────────┐
                    │ Dự báo            │
                    └─────────┬─────────┘
                              ↓
                    ┌───────────────────┐
                    │ Khuyến nghị       │
                    └─────────┬─────────┘
                              ↓
                           User
                              ↓
                    ┌───────────────────┐
                    │ Điều chỉnh        │
                    │ ngân sách         │
                    └───────────────────┘
```

Đây là **business flow**, không phải cách biểu diễn quan hệ giữa Use Case trong UML.

Do đó khi vẽ UCD thật, chúng ta **không nối các Use Case thành một flow bằng mũi tên tuần tự**.

Thay vào đó, flow này sẽ được thể hiện ở **Activity Diagram** và đặc tả Use Case.

Đây là một điểm cần giữ đúng chuẩn UML.

---

# 4. Phân cấp UCD

Tôi đề xuất chia thành **6 nhóm nghiệp vụ**.

```text
                    SMART EXPENSE & BUDGET MANAGER
                                │
        ┌───────────────────────┼────────────────────────┐
        │                       │                        │
        ↓                       ↓                        ↓
  1. Account              2. Financial Data       3. Budget
        │                       │                        │
        │                 ┌─────┴─────┐          ┌─────┴─────┐
        │                 ↓           ↓          ↓           ↓
        │              Income      Expense     Create      Monitor
        │
        └─────────────────────────────────────────────────────┐
                                                              │
        ┌──────────────────────────┬───────────────────────────┤
        ↓                          ↓                           ↓
  4. Financial Goals       5. Financial Analysis       6. Decision Support
                                   │                           │
                             ┌─────┼─────┐               ┌─────┼─────┐
                             ↓     ↓     ↓               ↓     ↓     ↓
                          Analyze Assess Forecast    Recommend Adjust
```

Tôi sẽ chi tiết hóa từng nhóm.

---

# 5. UCD phân cấp 1 – Quản lý tài khoản

### Actor

**User**

### Use Cases

```text
User
 │
 ├── (Đăng ký tài khoản)
 ├── (Đăng nhập)
 ├── (Đăng xuất)
 ├── (Quản lý thông tin cá nhân)
 └── (Quản lý cài đặt)
```

Nhóm này tương đối tiêu chuẩn.

Không cần cố làm phức tạp.

---

# 6. UCD phân cấp 2 – Quản lý dữ liệu tài chính

## 6.1. Quản lý thu nhập

```text
User
 │
 └── (Quản lý thu nhập)
          │
          ├── (Thêm thu nhập)
          ├── (Xem thu nhập)
          ├── (Sửa thu nhập)
          └── (Xóa thu nhập)
```

Tuy nhiên, về UCD chính thức tôi **không nhất thiết đưa cả 4 CRUD lên sơ đồ tổng**.

Có thể coi:

> `(Quản lý thu nhập)`

là Use Case cấp cao.

Sau đó sơ đồ chi tiết mới phân rã.

---

## 6.2. Quản lý chi tiêu

```text
User
 │
 └── (Quản lý chi tiêu)
          │
          ├── (Ghi nhận giao dịch)
          ├── (Xem giao dịch)
          ├── (Cập nhật giao dịch)
          ├── (Xóa giao dịch)
          └── (Phân loại giao dịch)
```

Nếu OCR được triển khai:

```text
User
 │
 └── (Ghi nhận giao dịch)
          │
          └── <<include>>
                  ↓
            (Nhận dạng hóa đơn)
                     ↑
                     │
                OCR Service
```

**OCR Service lúc này mới có lý do xuất hiện trên UCD.**

---

# 7. UCD phân cấp 3 – Quản lý ngân sách

Đây là một trong những nhóm quan trọng nhất.

```text
User
 │
 └── (Quản lý ngân sách)
          │
          ├── (Tạo ngân sách)
          ├── (Thiết lập ngân sách theo danh mục)
          ├── (Xem ngân sách)
          ├── (Theo dõi ngân sách)
          └── (Điều chỉnh ngân sách)
```

Nhưng `Theo dõi ngân sách` sẽ liên quan trực tiếp đến phần thông minh.

---

# 8. UCD phân cấp 4 – Phân tích và đánh giá

Đây là nhóm bắt đầu tạo nên **đặc trưng của đồ án**.

```text
User
 │
 └── (Phân tích tài chính)
          │
          ├── (Phân tích thu nhập)
          ├── (Phân tích chi tiêu)
          ├── (Phân tích xu hướng)
          ├── (Đánh giá tình trạng ngân sách)
          └── (Đánh giá tiến độ mục tiêu)
```

Ở đây:

```text
(Đánh giá tình trạng ngân sách)
```

có thể sử dụng:

- ngân sách;
- chi tiêu thực tế;
- lịch sử;
- số ngày còn lại;
- tốc độ chi tiêu.

Đây là nền cho bước dự báo.

---

# 9. UCD phân cấp 5 – Dự báo

Đây là Use Case mà tôi nghĩ chúng ta nên **đặc biệt chú ý**.

```text
User
 │
 └── (Dự báo chi tiêu)
          │
          ├── (Dự báo chi tiêu cuối kỳ)
          ├── (Dự báo nguy cơ vượt ngân sách)
          └── (Đánh giá mức độ rủi ro)
```

Ví dụ:

```text
Actual Spending
       +
Budget
       +
Spending Rate
       +
Days Remaining
       +
Historical Data
       ↓
Forecast
       ↓
Risk Assessment
```

Nếu AI Service bên ngoài được sử dụng:

```text
                   ┌───────────────────┐
                   │    AI Service     │
                   └─────────┬─────────┘
                             ↑
                             │
User → (Dự báo chi tiêu) ────┘
```

Nếu AI nằm trong hệ thống thì **không vẽ AI Service**.

---

# 10. UCD phân cấp 6 – Khuyến nghị và điều chỉnh

Đây là phần quan trọng nhất của "cái mới".

```text
User
 │
 └── (Nhận khuyến nghị tài chính)
          │
          ├── (Nhận khuyến nghị ngân sách)
          ├── (Nhận khuyến nghị điều chỉnh chi tiêu)
          └── (Xem lý do khuyến nghị)
```

Sau đó:

```text
User
 │
 └── (Điều chỉnh ngân sách)
          │
          ├── (Điều chỉnh hạn mức danh mục)
          ├── (Phân bổ lại ngân sách)
          └── (Xác nhận điều chỉnh)
```

Điểm rất hay ở đây là:

**Hệ thống không tự ý thay đổi ngân sách của User.**

Hệ thống:

```text
Analyze
   ↓
Predict
   ↓
Recommend
   ↓
User Decision
   ↓
Adjust
```

User vẫn là **Decision Maker**.

Điều này rất phù hợp với bản chất của hệ thống hỗ trợ quyết định.

---

# 11. Toàn bộ phân cấp tôi đề xuất

Nếu đưa vào tài liệu, tôi sẽ tổ chức như sau:

```text
UC-00 SMART EXPENSE & BUDGET MANAGER
│
├── UC-01 QUẢN LÝ TÀI KHOẢN
│   ├── UC-01.01 Đăng ký
│   ├── UC-01.02 Đăng nhập
│   ├── UC-01.03 Đăng xuất
│   └── UC-01.04 Quản lý thông tin cá nhân
│
├── UC-02 QUẢN LÝ THU NHẬP
│   ├── UC-02.01 Thêm thu nhập
│   ├── UC-02.02 Xem thu nhập
│   ├── UC-02.03 Cập nhật thu nhập
│   └── UC-02.04 Xóa thu nhập
│
├── UC-03 QUẢN LÝ CHI TIÊU
│   ├── UC-03.01 Ghi nhận giao dịch
│   ├── UC-03.02 Xem giao dịch
│   ├── UC-03.03 Cập nhật giao dịch
│   ├── UC-03.04 Xóa giao dịch
│   ├── UC-03.05 Phân loại giao dịch
│   └── UC-03.06 Nhận dạng hóa đơn [nếu triển khai OCR]
│
├── UC-04 QUẢN LÝ DANH MỤC
│   ├── UC-04.01 Tạo danh mục
│   ├── UC-04.02 Cập nhật danh mục
│   ├── UC-04.03 Xóa danh mục
│   └── UC-04.04 Xem danh mục
│
├── UC-05 QUẢN LÝ NGÂN SÁCH
│   ├── UC-05.01 Tạo ngân sách
│   ├── UC-05.02 Thiết lập ngân sách theo danh mục
│   ├── UC-05.03 Xem ngân sách
│   ├── UC-05.04 Theo dõi ngân sách
│   └── UC-05.05 Điều chỉnh ngân sách
│
├── UC-06 QUẢN LÝ MỤC TIÊU TÀI CHÍNH
│   ├── UC-06.01 Tạo mục tiêu
│   ├── UC-06.02 Cập nhật mục tiêu
│   ├── UC-06.03 Theo dõi tiến độ
│   └── UC-06.04 Hoàn thành/hủy mục tiêu
│
├── UC-07 PHÂN TÍCH TÀI CHÍNH
│   ├── UC-07.01 Phân tích thu nhập
│   ├── UC-07.02 Phân tích chi tiêu
│   ├── UC-07.03 Phân tích xu hướng
│   └── UC-07.04 Đánh giá tình trạng ngân sách
│
├── UC-08 DỰ BÁO
│   ├── UC-08.01 Dự báo chi tiêu cuối kỳ
│   ├── UC-08.02 Dự báo nguy cơ vượt ngân sách
│   └── UC-08.03 Đánh giá mức độ rủi ro
│
└── UC-09 HỖ TRỢ QUYẾT ĐỊNH
    ├── UC-09.01 Nhận khuyến nghị
    ├── UC-09.02 Xem lý do khuyến nghị
    ├── UC-09.03 Xem phương án điều chỉnh
    └── UC-09.04 Xác nhận điều chỉnh ngân sách
```

**Lưu ý:** Đây là **cây phân cấp Use Case đề xuất**, chưa phải một UML diagram hoàn chỉnh. Khi đưa vào luận văn, chúng ta sẽ chọn mức chi tiết phù hợp cho từng sơ đồ.

---

# 12. Còn các Actor bên ngoài thì đặt ở đâu?

Không nên nhét tất cả vào UCD tổng quát.

Ví dụ UCD xử lý hóa đơn:

```text
                 ┌─────────────────────────────┐
                 │ Smart Expense Manager       │
                 │                             │
User ──────────→ │ (Ghi nhận giao dịch)        │
                 │        │                    │
                 │        ↓ <<include>>        │
                 │ (Nhận dạng hóa đơn)         │
                 │        ↑                    │
                 └────────┼────────────────────┘
                          │
                    OCR Service
```

UCD tích hợp AI:

```text
                 ┌─────────────────────────────┐
User ──────────→ │ (Dự báo nguy cơ vượt       │
                 │  ngân sách)                │
                 │        ↑                    │
                 └────────┼────────────────────┘
                          │
                      AI Service
```

UCD thông báo:

```text
                 ┌─────────────────────────────┐
                 │ (Nhận cảnh báo ngân sách)   │
                 └──────────────↑──────────────┘
                                │
                       Notification Service
```

Nhưng **chỉ vẽ các Actor này nếu kiến trúc thực tế của chúng ta có các integration tương ứng**.

---

# 13. Tôi đề xuất bộ UCD cuối cùng cho KLTN

Thay vì 1 sơ đồ cực lớn, chúng ta nên có:

### Sơ đồ 1 – UCD tổng quát

**Actor:**
- User
- External Services nếu thực sự có integration quan trọng

**Nhóm Use Case:**

```text
Account
Financial Data
Budget
Financial Goals
Analysis
Forecast
Decision Support
```

---

### Sơ đồ 2 – UCD quản lý dữ liệu tài chính

```text
User
 ├── Quản lý thu nhập
 ├── Quản lý chi tiêu
 ├── Quản lý danh mục
 └── Nhận dạng hóa đơn
```

---

### Sơ đồ 3 – UCD quản lý ngân sách

```text
User
 ├── Tạo ngân sách
 ├── Thiết lập ngân sách
 ├── Theo dõi ngân sách
 ├── Đánh giá ngân sách
 └── Điều chỉnh ngân sách
```

---

### Sơ đồ 4 – UCD phân tích và dự báo

```text
User
 ├── Phân tích tài chính
 ├── Phân tích xu hướng
 ├── Đánh giá ngân sách
 ├── Dự báo chi tiêu
 └── Dự báo nguy cơ vượt ngân sách
```

---

### Sơ đồ 5 – UCD hỗ trợ quyết định

Đây là sơ đồ **đặc trưng nhất của đề tài**:

```text
                    ┌─────────────────────────────┐
                    │ Smart Expense Manager       │
                    │                             │
User ─────────────→ │ (Đánh giá ngân sách)        │
                    │          │                  │
                    │          ↓ <<include>>      │
                    │ (Dự báo nguy cơ)            │
                    │          │                  │
                    │          ↓                  │
                    │ (Tạo khuyến nghị)           │
                    │          │                  │
                    │          ↓                  │
                    │ (Xem phương án điều chỉnh)  │
                    │          │                  │
User ─────────────→ │ (Xác nhận điều chỉnh)       │
                    │          │                  │
                    │          ↓                  │
                    │ (Cập nhật ngân sách)        │
                    └─────────────────────────────┘
```

Tuy nhiên, **không phải tất cả mũi tên trong hình trên đều nên dùng `include`**. Khi thiết kế UML chính thức, chúng ta phải xác định quan hệ `include`, `extend`, association và generalization dựa trên điều kiện thực thi của từng Use Case, không dựa trên thứ tự flow.

---

# 14. Có một điều tôi muốn chốt trước khi chúng ta vẽ thật

Tôi nghĩ **Use Case quan trọng nhất của project không nên là "AI Recommendation"**.

Nên là:

> **Đánh giá và hỗ trợ điều chỉnh ngân sách dựa trên tình hình tài chính thực tế.**

AI chỉ là một cách triển khai một phần của quá trình đó.

Tức là kiến trúc nghiệp vụ nên là:

```text
                 USER
                   │
                   ↓
          ┌─────────────────┐
          │ Financial Data  │
          └────────┬────────┘
                   ↓
          ┌─────────────────┐
          │ Budget          │
          │ Assessment      │
          └────────┬────────┘
                   ↓
          ┌─────────────────┐
          │ Forecast        │
          └────────┬────────┘
                   ↓
          ┌─────────────────┐
          │ Recommendation  │
          └────────┬────────┘
                   ↓
                 USER
                   │
                   ↓
          ┌─────────────────┐
          │ Budget          │
          │ Adjustment      │
          └─────────────────┘
```

Đây sẽ là **xương sống của Chương 3**.

Còn các nhóm như đăng nhập, CRUD thu nhập, CRUD chi tiêu, danh mục... là **hệ thống hỗ trợ xương sống đó**.

### Vì vậy, bước tiếp theo tôi đề xuất không phải vẽ hình ngay.

Ta nên **chốt bảng UC-01 → UC-09 ở trên**, sau đó với từng Use Case xác định:

**Actor → Goal → Trigger → Preconditions → Main Flow → Alternative Flow → Business Rules → Postconditions → Include/Extend**

Khi bảng này chốt xong, tôi có thể dựng cho bạn **bộ UCD hoàn chỉnh theo đúng cấp độ: UCD tổng quát → UCD phân cấp → quan hệ include/extend → Actor bên ngoài**, và lúc đó chúng ta sẽ tránh được việc phải sửa sơ đồ nhiều lần khi sang Activity Diagram/Class Diagram.

---



Có. **Trong trường hợp của bạn, tôi khuyên nên cân nhắc OCR API và AI API bên ngoài là actor** — và thực ra đây là cách mô hình hóa hợp lý hơn nếu kiến trúc triển khai của bạn thực sự gọi các dịch vụ đó qua API.

Điểm quan trọng là phân biệt **“AI/OCR là công nghệ”** với **“AI/OCR Service là hệ thống bên ngoài mà hệ thống của bạn tương tác”**.

### 1. Nếu dùng API bên ngoài thì AI/OCR có thể là Actor

Ví dụ kiến trúc của bạn:

```text
                    ┌──────────────────────────────┐
                    │ Smart Expense &              │
                    │ Budget Manager               │
                    │                              │
User ──────────────►│  Quản lý chi tiêu            │
                    │       │                      │
                    │       ▼                      │
                    │  OCR Integration ────────────┼────► OCR API
                    │                              │
                    │  AI Analysis ────────────────┼────► AI API
                    └──────────────────────────────┘
```

Khi đó:

- **User**: Actor chính.
- **OCR Service**: Actor bên ngoài.
- **AI Service**: Actor bên ngoài.
- Hệ thống của bạn: system boundary.
- Các module bên trong như `OCR Integration`, `Forecasting Engine`, `Recommendation Engine` **không phải actor**.

Lý do rất đơn giản:

> Actor là một thực thể **nằm ngoài phạm vi hệ thống** nhưng có tương tác với hệ thống.

API OCR/AI của bên thứ ba nằm ngoài hệ thống của bạn và hệ thống của bạn gửi request/nhận response → hoàn toàn có cơ sở để biểu diễn thành actor.

---

## 2. Tôi đề xuất sửa lại actor của dự án

Thay vì bản trước chỉ có:

> **User**

Tôi nghĩ bản phù hợp hơn với kiến trúc dự kiến của bạn là:

### Actor chính

**A-01 — User**

Thực hiện:
- Quản lý tài khoản
- Quản lý thu nhập
- Quản lý chi tiêu
- Quản lý danh mục
- Quản lý ngân sách
- Quản lý mục tiêu
- Xem phân tích
- Xem dự báo
- Xem khuyến nghị
- Quyết định điều chỉnh ngân sách

### Actor phụ / hệ thống bên ngoài

**A-02 — OCR Service**

Tương tác với:
- `UC-03.07 Nhận dạng thông tin từ hóa đơn`

Luồng:

```text
User
  │
  │ Upload / chụp hóa đơn
  ▼
UC-03.07 Nhận dạng thông tin từ hóa đơn
  │
  │ Request
  ▼
OCR Service
  │
  │ Extracted data
  ▼
Hệ thống
  │
  ▼
User xác nhận / chỉnh sửa
```

**A-03 — AI Service**

Tương tác với các chức năng mà bạn thực sự dùng AI API, chẳng hạn:

- Phân tích dữ liệu tài chính
- Dự báo chi tiêu
- Đánh giá rủi ro
- Sinh khuyến nghị

Ví dụ:

```text
                    ┌───────────────────────────┐
                    │ Smart Expense Manager     │
                    │                           │
User ──────────────►│ Phân tích tài chính       │
User ──────────────►│ Dự báo chi tiêu           │
User ──────────────►│ Hỗ trợ quyết định         │
                    │            │              │
                    └────────────┼──────────────┘
                                 │
                                 ▼
                            AI Service
```

---

## 3. Nhưng có một điểm rất quan trọng

**Không nên biến mỗi API thành một actor chỉ vì bạn sử dụng API.**

Ví dụ:

```text
AI API
OCR API
Database API
REST API
JWT
React
Flutter
Spring Boot
```

không có nghĩa là tất cả đều trở thành actor.

Tiêu chí nên là:

> **Hệ thống của mình có thực sự giao tiếp với một hệ thống bên ngoài để hoàn thành một mục tiêu nghiệp vụ hay không?**

### Ví dụ

**OCR API → Có thể là Actor**

Vì:

> Hệ thống gửi ảnh hóa đơn → OCR API xử lý → trả về thông tin → hệ thống sử dụng kết quả.

Đây là một tương tác bên ngoài rõ ràng.

**AI API → Có thể là Actor**

Nếu:

> Hệ thống gửi dữ liệu tài chính → AI API phân tích/dự báo → trả kết quả → hệ thống hiển thị khuyến nghị.

Cũng là tương tác bên ngoài.

Trong khi:

**MySQL/PostgreSQL → thường không nên đưa thành Actor trong UCD nghiệp vụ.**

Database là hạ tầng lưu trữ của hệ thống, không phải một tác nhân thực hiện mục tiêu nghiệp vụ.

---

# 4. Điều này còn có lợi cho đồ án của bạn

Thực tế tôi **ủng hộ cách này hơn** đối với dự án cá nhân của bạn.

Bạn không cần chứng minh:

> "Tôi tự xây dựng một mô hình OCR."

hoặc:

> "Tôi tự huấn luyện mô hình AI dự báo tài chính."

Điều bạn cần chứng minh là:

> **Tôi thiết kế và xây dựng được một hệ thống có khả năng tích hợp các dịch vụ AI/OCR bên ngoài để giải quyết bài toán quản lý chi tiêu và ngân sách.**

Đó vẫn là một nội dung kỹ thuật hợp lệ và thực tế.

Bạn sẽ tập trung vào phần quan trọng nhất của đồ án:

```text
                 ┌──────────────┐
                 │    USER      │
                 └──────┬───────┘
                        │
                        ▼
             ┌─────────────────────┐
             │ Smart Expense       │
             │ Budget Manager      │
             └──────────┬──────────┘
                        │
          ┌─────────────┼──────────────┐
          ▼             ▼              ▼
       OCR API       AI API        Database
          │             │
          ▼             ▼
    Hóa đơn → Data   Phân tích
                    → Dự báo
                    → Khuyến nghị
                         │
                         ▼
                    User quyết định
                         │
                         ▼
                   Điều chỉnh ngân sách
```

**Phần "thông minh" của đồ án nằm ở cách bạn tổ chức và sử dụng kết quả AI/OCR trong nghiệp vụ**, chứ không nhất thiết nằm ở việc tự phát minh ra mô hình AI.

---

# 5. Tôi đề xuất chỉnh UC-03 và UC-07/08/09 theo hướng này

### UC-03

`UC-03.07 Nhận dạng thông tin từ hóa đơn`

Quan hệ dự kiến:

```text
User ───────────────► UC-03.07
                         │
                         │ <<include>>
                         ▼
                   OCR Service
```

Nhưng khi vẽ UML chính thức, tôi muốn chúng ta kiểm tra lại **hướng và bản chất `include`** thay vì mặc định hóa quan hệ này.

---

### UC-08

Ví dụ:

```text
User ───────────────► UC-08.01 Dự báo chi tiêu cuối chu kỳ
                              │
                              │ gọi dịch vụ AI
                              ▼
                         AI Service
```

Nhưng cũng cần lưu ý:

**Không phải cứ gọi AI API thì trong UCD nhất thiết phải nối `<<include>> AI Service`.**

UML Use Case Diagram mô tả **mục tiêu và tương tác**, còn việc hệ thống dùng AI API như thế nào là một phần của đặc tả/kiến trúc.

Do đó, khi làm bản Markdown cuối cùng, tôi sẽ ghi rõ cho người vẽ:

> AI Service và OCR Service là external actors **nếu triển khai dưới dạng dịch vụ API bên ngoài**; quan hệ cụ thể với từng UC cần được xác định theo nghiệp vụ, không tự động gắn `<<include>>` cho mọi UC có sử dụng API.

Điểm này rất quan trọng để UCD không bị biến thành sơ đồ kiến trúc.

---

# 6. Và tôi muốn chốt một nguyên tắc cho toàn bộ đồ án

Tôi đề xuất chúng ta sử dụng **3 tầng khái niệm**:

### Tầng 1 — Business

```text
Quản lý chi tiêu
Quản lý ngân sách
Phân tích
Dự báo
Hỗ trợ quyết định
```

### Tầng 2 — External Services

```text
OCR Service
AI Service
Notification Service
Authentication Provider
Banking/Payment Service
```

### Tầng 3 — Internal Implementation

```text
OCR Integration
AI Integration
Forecasting Engine
Recommendation Engine
Budget Analysis Engine
Database
```

**Tầng 1 → xuất hiện trong Use Case.**

**Tầng 2 → có thể xuất hiện dưới dạng Actor nếu thực sự tích hợp bên ngoài.**

**Tầng 3 → không phải Actor vì nằm bên trong system boundary.**

---

### Vì vậy, tôi cập nhật quyết định của chúng ta như sau

**Actor chính:**

> `User`

**Actor bên ngoài có khả năng đưa vào UCD:**

> `OCR Service`  
> `AI Service`

**Actor khác chỉ thêm nếu sau này bạn thực sự triển khai:**

> `Notification Service`  
> `Authentication Provider`  
> `Banking/Payment Service`  
> ...

Và tôi **không khuyến nghị thêm những actor này vào chỉ để sơ đồ trông đầy đủ**.

Đây là một thay đổi đáng đưa vào **bản master UC** trước khi xuất Markdown. Khi bạn gửi các lưu ý còn lại, tôi sẽ gom luôn thay đổi này vào bản cuối.

---

