# 🛠️ QUY TẮC KỸ NGHỆ PHẦN MỀM, KIẾN TRÚC MÃ NGUỒN VÀ TIÊU CHUẨN CODE CRAFT
## (ENGINEERING ARCHITECTURE, CODE CRAFT & QUALITY STANDARDS RULE)

> [!IMPORTANT]
> **BẮT BUỘC ÁP DỤNG CHO TOÀN BỘ CÁC TÁC TỬ VÀ KỸ SƯ TRONG WORKSPACE:**
> Quy định này chi phối toàn bộ quá trình thiết kế, viết mã, kiểm thử và review mã nguồn cho phân hệ Backend ASP.NET Core 10 và Mobile .NET MAUI của đề tài Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh (PFM).

---

## 1. NGUYÊN TẮC THUẬT NGỮ KỸ THUẬT & CHÚ THÍCH MÃ NGUỒN (CODE COMMENTS & NOTING RULES)

1. **Giữ nguyên 100% Technical Terms bằng Tiếng Anh:**
   - Tuyệt đối KHÔNG dịch các thuật ngữ kỹ thuật tiêu chuẩn sang tiếng Việt gượng ép.
   - Các thuật ngữ bắt buộc giữ nguyên gốc: *Clean Architecture, Rich Domain Model, Anemic Domain Model, Value Object, Aggregate Root, Entity, Result Pattern, DTO, CQRS, Repository Pattern, Unit of Work, Pessimistic Locking, Optimistic Locking, Lost Update, Race Condition, Deadlock, Idempotency, Vector Clock, Fast-Fail, Defensive Programming, Dependency Injection, Source Generators, Two-way Data Binding, RelayCommand, ObservableProperty, Circuit Breaker, Exponential Backoff, CancellationToken*.
2. **Quy Tắc Ghi Chú Thích (Code Notes) Thực Dụng & Trọng Tâm:**
   - **Mục tiêu duy nhất:** Viết để **Code Reviewer** hoặc **Chính Người Dùng trong tương lai** khi mở lại file code là biết ngay đoạn code này **LÀM ĐƯỢC GÌ**, không phải tốn thời gian suy đoán.
   - **Nội dung:** CHỈ MÔ TẢ ĐÚNG CHỨC NĂNG VÀ MỤC ĐÍCH NGHIỆP VỤ (What it does), ngắn gọn, súc tích (1 dòng, tối đa 2 dòng), đi thẳng vào việc.
   - **Hạn chế tối đa trang trí (Zero-Decoration):** Tuyệt đối KHÔNG vẽ banner, khung viền, hoa văn ASCII art (như `// ==========================`, `// ********* METHOD START *********`).
   - **Không bình luận hiển nhiên:** Không note lại cú pháp ngôn ngữ hiển nhiên (ví dụ: `// Khởi tạo biến a = 1`).

```csharp
// ❌ BAD: Dài dòng, trang trí rườm rà, giải thích cú pháp hiển nhiên
// ========================================================
// METHOD: Rút tiền từ ví
// Date: 2026-10-21
// ========================================================
public void Withdraw(decimal amount) {
    // Trừ số tiền khỏi số dư
    Balance -= amount;
}

// ✅ GOOD: Ngắn gọn, không trang trí, mô tả ngay code làm được gì cho Reviewer/User tương lai
// Rút tiền khỏi ví có kiểm tra hạn mức âm và phát sự kiện WalletBalanceChangedEvent
public Result Withdraw(Money amount) { ... }

// Khóa bi quan (SELECT ... FOR UPDATE) cả 2 ví để chuyển tiền an toàn, chống Lost Update
public async Task<Result> TransferFundsAsync(TransferFundsRequest request, CancellationToken ct) { ... }
```

---

## 2. NGUYÊN TẮC THIẾT KẾ & PHƯƠNG PHÁP CODE (CORE METHODOLOGY)

1. **Rich Domain Model (Tuyệt đối chống Anemic Domain Model):**
   - Mọi quy tắc nghiệp vụ tài chính (tính toán số dư, kiểm tra hạn mức, hậu kiểm toán số học hóa đơn OCR, tính hạn mức ngày DSA) bắt buộc phải nằm bên trong Domain Entity và Value Object.
   - Entity không được phép là class rỗng chỉ chứa `{ get; set; }` để logic bị phân tán tự do ra ngoài Services.
2. **Result Pattern thay thế Exception-Driven Flow:**
   - Dùng `Result<T>` cho mọi tình huống xác thực và lỗi nghiệp vụ thông thường (*UserNotFound, InsufficientFunds, BudgetExceeded*).
   - Chỉ dùng `Exception` cho lỗi kỹ thuật bất khả kháng của hệ thống (mất kết nối CSDL, crash phần cứng, timeout mạng).
3. **Immutable Value Objects:**
   - Toàn bộ các khái niệm đo lường tiền tệ và khoảng thời gian (`Money`, `DateRange`) bắt buộc dùng `record` hoặc `readonly record struct`. Mọi phép toán cộng trừ tiền phải trả về instance mới, không bao giờ mutate dữ liệu cũ.
4. **Nguyên Tắc DRY (Don't Repeat Yourself) Bắt Buộc:**
   - **Backend:** Tái sử dụng `BaseEntity<TId>`, `ValueObject`, `Result<T>`, MediatR Validation Pipeline, Extension Methods.
   - **Mobile (.NET MAUI):** Trích xuất các UI Card lặp lại thành `ContentView` tái sử dụng; tập trung 100% màu sắc, kích thước, phông chữ vào `ResourceDictionary` (`Colors.xaml`, `Styles.xaml`). Cấm gõ inline styling trùng lặp.
5. **Cấu Hình Trình Biên Dịch Khắt Khe (.NET 10 / C# 14):**
   - Mọi project `.csproj` bắt buộc bật:
     ```xml
     <Nullable>enable</Nullable>
     <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
     <AnalysisLevel>latest-recommended</AnalysisLevel>
     <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
     ```

---

## 3. QUY TRÌNH KIỂM THỬ ĐƠN VỊ CUỐN CHIẾU (INCREMENTAL UNIT TESTING)

1. **Kiểm thử cuốn chiếu (Module-by-Module):**
   - Tuyệt đối KHÔNG dồn việc viết test đến cuối dự án.
   - Sau khi hoàn thành bất kỳ một Domain Entity, Value Object, Service hoặc Use Case Handler nào, bắt buộc phải viết ngay bộ Unit Test (`xUnit` + `FluentAssertions`) cho thành phần đó trước khi chuyển sang module tiếp theo.
2. **Trọng tâm kiểm thử:**
   - Tập trung vào các thuật toán tài chính cốt lõi: Tính hạn mức thích ứng DSA, Hậu kiểm toán số học hóa đơn OCR, Thuật toán trả nợ Snowball vs Avalanche, Chuyển tiền nội bộ giữa các ví.

---

## 4. CHIẾN LƯỢC TIẾT KIỆM TOKEN & QUẢN TRỊ PHẠM VI (SCOPED INSPECTION)

1. **Kiểm tra có trọng tâm (Targeted Scope):**
   - Khi thực hiện kiểm tra mã nguồn, fix bug hoặc chạy test, Agent chỉ được phép đọc và xử lý đúng các tệp/hàm liên quan trực tiếp đến yêu cầu của người dùng.
   - Tuyệt đối KHÔNG tự ý quét toàn bộ Solution hoặc đọc lại toàn bộ thư mục khi không có lệnh rõ ràng từ người dùng.
2. **Chống quét lặp lỗi (No Redundant Linting):**
   - Không kiểm tra lại cùng một lỗi hay phân tích lại một đoạn code không có sự thay đổi giữa các lượt hội thoại.

---

## 5. NGUYÊN TẮC ZERO-CODE FREEZE TRƯỚC KHI HOÀN THIỆN ĐẶC TẢ

- Không được phép tạo file mã nguồn logic ứng dụng khi các tài liệu kiến trúc kỹ thuật (`TECHNICAL_ARCHITECTURE_SPEC.md`), khế ước API (`API Contracts`) và thiết kế CSDL chưa được phê duyệt đầy đủ.
