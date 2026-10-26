# BẢN ĐẶC TẢ KIẾN TRÚC KỸ THUẬT TOÀN DIỆN HỆ THỐNG PFM
## (TECHNICAL ARCHITECTURE & SPECIFICATION: ASP.NET CORE & .NET MAUI)

> **Dự án:** Hệ thống Quản lý Chi tiêu và Hoạch định Tài chính Cá nhân Thông minh (Tích hợp AI)  
> **Ngăn xếp công nghệ:** Backend ASP.NET Core 10 (Clean Architecture, C# 14) + Mobile .NET MAUI (Clean MVVM) + PostgreSQL 16 + SQLite Local  
> **Tài liệu tham chiếu:** [`00_MASTER_CONTEXT.md`](../../project_context/00_MASTER_CONTEXT.md), [`05_engineering_architecture_and_code_craft_rules.md`](../../.agents/rules/05_engineering_architecture_and_code_craft_rules.md)

---

## CHƯƠNG 1: BẢN ĐỒ KIẾN TRÚC TỔNG THỂ (SYSTEM ARCHITECTURE BLUEPRINT)

Hệ thống được thiết kế theo mô hình phân tách trách nhiệm cao (Separation of Concerns), phân chia thành 2 phân hệ độc lập giao tiếp qua giao thức bảo mật HTTPS REST API và WebSocket SignalR:

```text
┌────────────────────────────────────────────────────────────────────────┐
│                        .NET MAUI ENTERPRISE CLIENT                     │
│  ┌───────────────────────┐  ┌───────────────────────────────────────┐  │
│  │ Views (XAML)          │  │ ViewModels (CommunityToolkit.Mvvm)    │  │
│  │ - Theme: Impeccable   │◄─┤ - [ObservableProperty], [RelayCommand]│  │
│  │ - ResourceDictionary  │  │ - WeakReferenceMessenger              │  │
│  └───────────────────────┘  └───────────────────┬───────────────────┘  │
│                                                 │                      │
│  ┌──────────────────────────────────────────────┴───────────────────┐  │
│  │ Mobile Services Layer                                            │  │
│  │ - Refit Type-Safe API Client + Polly v8 Resilience Pipeline      │  │
│  │ - SignalR Client Hub Connection (Realtime 0ms Dynamic Sync)      │  │
│  │ - SQLite Local Cache (sqlite-net-pcl + SQLCipher AES-256)        │  │
│  │ - SecureStorage (Android KeyStore / Apple Keychain)              │  │
│  └──────────────────────────────────────┬───────────────────────────┘  │
└─────────────────────────────────────────┼──────────────────────────────┘
                                          │ HTTPS (REST) & WSS (SignalR)
┌─────────────────────────────────────────▼──────────────────────────────┐
│                     BACKEND ASP.NET CORE 10 WEB API                    │
│  ┌──────────────────────────────────────────────────────────────────┐  │
│  │ WebApi Layer: Thin Controllers, FinanceHub, Auth Middleware      │  │
│  ├──────────────────────────────────────────────────────────────────┤  │
│  │ Application Layer: CQRS (Commands/Queries), DTOs, Validators     │  │
│  ├──────────────────────────────────────────────────────────────────┤  │
│  │ Domain Layer (Core): Rich Entities, Immutable Value Objects      │  │
│  ├──────────────────────────────────────────────────────────────────┤  │
│  │ Infrastructure Layer:                                            │  │
│  │ - EF Core PostgreSQL 16 (Pessimistic Locking SELECT FOR UPDATE)  │  │
│  │ - IMemoryCache (In-Memory 0.001ms, 0-cost, No Redis needed)      │  │
│  │ - GeminiVisionOcrClient / MockReceiptOcrService                  │  │
│  │ - VietQrGeneratorService / MockVietQrService                     │  │
│  └──────────────────────────────────────────────────────────────────┘  │
└────────────────────────────────────────────────────────────────────────┘
```

---

## CHƯƠNG 2: LƯỢC ĐỒ CSDL 2 TẦNG (TWO-TIER POLYGLOT PERSISTENCE SPECIFICATION)

### 2.1. Tầng Client: SQLite Local Database (Offline-First Storage)
Mỗi thiết bị cài đặt ứng dụng sở hữu một file CSDL nhúng `app_local.db` tại `FileSystem.AppDataDirectory`.

```csharp
// Local SQLite Entity Schema
[Table("local_transactions")]
public sealed class LocalTransactionEntity
{
    [PrimaryKey]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid WalletId { get; set; }
    public int CategoryId { get; set; }
    public decimal Amount { get; set; }
    public string TransactionType { get; set; } = "EXPENSE"; // INCOME / EXPENSE
    public DateTime TransactionDate { get; set; }
    public string? Note { get; set; }
    public bool IsEssential { get; set; }
    public bool IsWindfall { get; set; }

    // Sync Management Columns
    public bool IsSynced { get; set; } = false; // 0: Pending Upload, 1: Synced with Server
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

---

### 2.2. Tầng Server: PostgreSQL 16 Central Schema (3NF DDL Scripts)

```sql
-- Kích hoạt extension sinh UUIDv7 / UUID ngẫu nhiên
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- 1. BẢNG USERS
CREATE TABLE users (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email VARCHAR(150) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    full_name VARCHAR(100) NOT NULL,
    salary_day SMALLINT NOT NULL DEFAULT 1 CHECK (salary_day BETWEEN 1 AND 31),
    budget_cycle VARCHAR(20) NOT NULL DEFAULT 'MONTHLY',
    risk_tolerance VARCHAR(20) NOT NULL DEFAULT 'MODERATE',
    default_currency VARCHAR(3) NOT NULL DEFAULT 'VND',
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- 2. BẢNG WALLETS (Đa ví chuẩn ACID)
CREATE TABLE wallets (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    name VARCHAR(100) NOT NULL,
    wallet_type VARCHAR(20) NOT NULL CHECK (wallet_type IN ('CASH', 'BANK', 'CREDIT_CARD', 'E_WALLET')),
    balance DECIMAL(15, 2) NOT NULL DEFAULT 0.00,
    currency VARCHAR(3) NOT NULL DEFAULT 'VND',
    credit_limit DECIMAL(15, 2) NULL,
    statement_closing_date SMALLINT NULL CHECK (statement_closing_date BETWEEN 1 AND 31),
    payment_due_date SMALLINT NULL CHECK (payment_due_date BETWEEN 1 AND 31),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    version INT NOT NULL DEFAULT 1,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- 3. BẢNG CATEGORIES (Cây danh mục phân cấp đệ quy cha-con)
CREATE TABLE categories (
    id SERIAL PRIMARY KEY,
    user_id UUID NULL REFERENCES users(id) ON DELETE CASCADE, -- NULL = System default
    parent_id INT NULL REFERENCES categories(id) ON DELETE RESTRICT,
    name VARCHAR(100) NOT NULL,
    type VARCHAR(10) NOT NULL CHECK (type IN ('INCOME', 'EXPENSE')),
    icon_name VARCHAR(50) NOT NULL DEFAULT 'wallet',
    color_code VARCHAR(20) NOT NULL DEFAULT '#1E293B',
    is_system BOOLEAN NOT NULL DEFAULT FALSE
);

-- 4. BẢNG RECEIPTS (Hóa đơn AI OCR)
CREATE TABLE receipts (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    image_url VARCHAR(500) NULL,
    merchant_name VARCHAR(150) NULL,
    invoice_date DATE NOT NULL DEFAULT CURRENT_DATE,
    total_amount DECIMAL(15, 2) NOT NULL DEFAULT 0.00,
    vat_amount DECIMAL(15, 2) NOT NULL DEFAULT 0.00,
    raw_ocr_payload JSONB NOT NULL DEFAULT '{}'::jsonb,
    audit_status VARCHAR(30) NOT NULL DEFAULT 'PENDING' 
        CHECK (audit_status IN ('PENDING', 'VERIFIED_MATH_OK', 'MATH_DISCREPANCY', 'REJECTED')),
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- 5. BẢNG RECEIPT_ITEMS (Chi tiết từng món trong hóa đơn - Composition)
CREATE TABLE receipt_items (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    receipt_id UUID NOT NULL REFERENCES receipts(id) ON DELETE CASCADE,
    item_name VARCHAR(200) NOT NULL,
    quantity DECIMAL(10, 2) NOT NULL DEFAULT 1.00,
    unit_price DECIMAL(15, 2) NOT NULL DEFAULT 0.00,
    total_item_amount DECIMAL(15, 2) NOT NULL DEFAULT 0.00,
    suggested_category_id INT NULL REFERENCES categories(id) ON DELETE SET NULL
);

-- 6. BẢNG TRANSACTIONS (Sổ cái giao dịch)
CREATE TABLE transactions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    wallet_id UUID NOT NULL REFERENCES wallets(id) ON DELETE RESTRICT,
    category_id INT NOT NULL REFERENCES categories(id) ON DELETE RESTRICT,
    amount DECIMAL(15, 2) NOT NULL CHECK (amount > 0),
    transaction_type VARCHAR(10) NOT NULL CHECK (transaction_type IN ('INCOME', 'EXPENSE')),
    transaction_date DATE NOT NULL,
    note TEXT NULL,
    is_essential BOOLEAN NOT NULL DEFAULT TRUE,
    is_windfall BOOLEAN NOT NULL DEFAULT FALSE,
    receipt_id UUID NULL REFERENCES receipts(id) ON DELETE SET NULL,
    client_local_id VARCHAR(64) NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- 7. BẢNG INTERNAL_TRANSFERS (Chuyển tiền nội bộ giữa 2 ví)
CREATE TABLE internal_transfers (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    from_wallet_id UUID NOT NULL REFERENCES wallets(id) ON DELETE RESTRICT,
    to_wallet_id UUID NOT NULL REFERENCES wallets(id) ON DELETE RESTRICT,
    amount DECIMAL(15, 2) NOT NULL CHECK (amount > 0),
    fee DECIMAL(15, 2) NOT NULL DEFAULT 0.00,
    transfer_date TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    note VARCHAR(255) NULL
);

-- 8. BẢNG BUDGETS & BUDGET_ALLOCATIONS
CREATE TABLE budgets (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    budget_method VARCHAR(30) NOT NULL CHECK (budget_method IN ('RULE_50_30_20', 'ZERO_BASED', 'KAKEIBO')),
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    total_amount_limit DECIMAL(15, 2) NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT chk_budget_dates CHECK (end_date >= start_date)
);

CREATE TABLE budget_allocations (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    budget_id UUID NOT NULL REFERENCES budgets(id) ON DELETE CASCADE,
    category_id INT NOT NULL REFERENCES categories(id) ON DELETE CASCADE,
    allocated_amount DECIMAL(15, 2) NOT NULL,
    CONSTRAINT uk_budget_category UNIQUE (budget_id, category_id)
);

-- 9. BẢNG DAILY_SPENDING_ALLOWANCES (Nhật ký DSA)
CREATE TABLE daily_spending_allowances (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    calculation_date DATE NOT NULL,
    dsa_amount DECIMAL(15, 2) NOT NULL,
    actual_spent DECIMAL(15, 2) NOT NULL DEFAULT 0.00,
    spending_velocity DECIMAL(6, 2) NOT NULL DEFAULT 1.00,
    projected_out_of_money_date DATE NULL,
    CONSTRAINT uk_user_dsa_date UNIQUE (user_id, calculation_date)
);

-- 10. BẢNG DEBT_ACCOUNTS & DEBT_PAYMENT_PLANS
CREATE TABLE debt_accounts (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    creditor_name VARCHAR(150) NOT NULL,
    principal_amount DECIMAL(15, 2) NOT NULL,
    current_balance DECIMAL(15, 2) NOT NULL,
    interest_rate_apr DECIMAL(5, 2) NOT NULL,
    minimum_monthly_payment DECIMAL(15, 2) NOT NULL,
    due_date SMALLINT NOT NULL CHECK (due_date BETWEEN 1 AND 31)
);

CREATE TABLE debt_payment_plans (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    strategy VARCHAR(20) NOT NULL CHECK (strategy IN ('SNOWBALL', 'AVALANCHE')),
    extra_monthly_budget DECIMAL(15, 2) NOT NULL DEFAULT 0.00,
    projected_payoff_date DATE NULL,
    total_interest_saved DECIMAL(15, 2) NOT NULL DEFAULT 0.00
);

-- 11. BẢNG FINANCIAL_NUDGES (Cú hích hành vi Thaler & Cooling-off)
CREATE TABLE financial_nudges (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    nudge_type VARCHAR(30) NOT NULL,
    title VARCHAR(200) NOT NULL,
    rationale_explanation TEXT NOT NULL,
    cooling_off_until TIMESTAMPTZ NULL,
    is_approved_by_user BOOLEAN NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

-- 12. BẢNG REFRESH_TOKENS (Bảo mật JWT)
CREATE TABLE refresh_tokens (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    token_hash VARCHAR(255) NOT NULL UNIQUE,
    expires_at TIMESTAMPTZ NOT NULL,
    is_revoked BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
```

---

### 2.3. Composite Indexing & Concurrency Locking

```sql
-- 1. Index phủ tối ưu truy vấn lịch sử sổ cái (< 5ms)
CREATE INDEX idx_transactions_user_date ON transactions (user_id, transaction_date DESC);

-- 2. Index hỗ trợ tính toán lũy kế ngân sách theo danh mục
CREATE INDEX idx_transactions_budget_calc ON transactions (user_id, category_id, transaction_date, amount);

-- 3. Index tra cứu nhanh các hóa đơn chờ kiểm toán
CREATE INDEX idx_receipts_user_audit ON receipts (user_id, audit_status);
```

```csharp
// Pessimistic Locking Implementation in EF Core C#
public async Task<Result> TransferFundsAsync(Guid fromWalletId, Guid toWalletId, decimal amount, CancellationToken ct)
{
    await using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted, ct);
    try
    {
        // Khóa đồng thời cả 2 dòng ví theo thứ tự ID tăng dần để chống Deadlock
        var sortedIds = new[] { fromWalletId, toWalletId }.OrderBy(x => x).ToList();
        var wallets = await _context.Wallets
            .FromSqlInterpolated($"SELECT * FROM wallets WHERE id IN ({sortedIds[0]}, {sortedIds[1]}) ORDER BY id FOR UPDATE")
            .ToDictionaryAsync(w => w.Id, ct);

        var fromWallet = wallets[fromWalletId];
        var toWallet = wallets[toWalletId];

        var withdrawResult = fromWallet.Withdraw(Money.FromVnd(amount));
        if (withdrawResult.IsFailure)
        {
            await transaction.RollbackAsync(ct);
            return withdrawResult;
        }

        toWallet.Deposit(Money.FromVnd(amount));
        await _context.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);

        return Result.Success();
    }
    catch (Exception ex)
    {
        await transaction.RollbackAsync(ct);
        return Result.Failure($"Transfer failed: {ex.Message}");
    }
}
```

---

## CHƯƠNG 3: KHẾ ƯỚC GIAO TIẾP API & SIGNALR HUB (MASTER API CATALOG)

Để hiện thực hóa toàn bộ 10 Sơ đồ Use Case (**chính xác 63 ca sử dụng chi tiết từ UC-01.01 đến UC-09.05**) và 6 Bounded Contexts trong hồ sơ PTTKHTTT, hệ thống được định hướng **triển khai cuốn chiếu dứt điểm theo từng phân hệ (Module-by-Module Incremental Delivery)**. Backend Web API cung cấp toàn diện **40 REST Endpoints** và **3 SignalR Realtime Handlers**, phân chia thành 7 phân hệ chuyên biệt:

### 3.1. Phân Hệ 1: Identity, Auth & Profile Context (4 Endpoints)
| HTTP Method | Endpoint URI | Mục Đích Nghiệp Vụ & DTO Contracts | Yêu Cầu Auth |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/v1/auth/register` | Đăng ký tài khoản mới (`RegisterRequest` $\rightarrow$ `UserDto`) | Không |
| `POST` | `/api/v1/auth/login` | Đăng nhập (`LoginRequest` $\rightarrow$ `AccessToken`, `RefreshToken`) | Không |
| `POST` | `/api/v1/auth/refresh-token` | Xoay vòng Refresh Token lấy Access Token mới (`RefreshTokenRequest`) | Không |
| `GET/PUT`| `/api/v1/users/profile` | Xem và cập nhật cài đặt tài chính (`salaryDay`, `budgetCycle`, `riskTolerance`) | Bearer JWT |

### 3.2. Phân Hệ 2: Account & Multi-Wallet Context (6 Endpoints)
| HTTP Method | Endpoint URI | Mục Đích Nghiệp Vụ & DTO Contracts | Yêu Cầu Auth |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/v1/wallets` | Lấy danh sách ví, số dư và **Tổng tài sản ròng (Net Worth = Assets - Debts)** | Bearer JWT |
| `POST` | `/api/v1/wallets` | Khởi tạo ví mới (`CASH`, `BANK`, `CREDIT_CARD`, `E_WALLET`) | Bearer JWT |
| `PUT` | `/api/v1/wallets/{id}` | Cập nhật thông tin ví hoặc hạn mức tín dụng (`creditLimit`) | Bearer JWT |
| `DELETE`| `/api/v1/wallets/{id}` | Đóng/Ẩn ví (Soft delete để bảo toàn vết kiểm toán sổ cái) | Bearer JWT |
| `POST` | `/api/v1/wallets/transfer` | **Chuyển tiền nội bộ giữa 2 ví (Pessimistic Locking `SELECT FOR UPDATE`)** | Bearer JWT |
| `POST` | `/api/v1/wallets/{id}/adjust` | Ghi nhận kiểm kê thực tế và cân đối số dư (`BalanceAdjustment`) | Bearer JWT |

### 3.3. Phân Hệ 3: Transactions, Categories & AI Vision OCR (10 Endpoints)
| HTTP Method | Endpoint URI | Mục Đích Nghiệp Vụ & DTO Contracts | Yêu Cầu Auth |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/v1/categories/tree` | Lấy cây danh mục phân cấp đệ quy cha-con (Ăn uống $\rightarrow$ Đi chợ, Cafe) | Bearer JWT |
| `POST` | `/api/v1/categories` | Tạo danh mục tùy biến mới | Bearer JWT |
| `GET` | `/api/v1/transactions` | Lấy sổ cái giao dịch phân trang, lọc theo khoảng ngày, ví, danh mục | Bearer JWT |
| `GET` | `/api/v1/transactions/{id}` | Xem chi tiết giao dịch (kèm danh sách mặt hàng nếu từ hóa đơn) | Bearer JWT |
| `POST` | `/api/v1/transactions` | Tạo giao dịch thu/chi thủ công | Bearer JWT |
| `PUT` | `/api/v1/transactions/{id}` | Chỉnh sửa giao dịch (Tự động hoàn trả và cập nhật lại số dư ví) | Bearer JWT |
| `DELETE`| `/api/v1/transactions/{id}` | Xóa giao dịch (Revert balance trên ví tương ứng) | Bearer JWT |
| `POST` | `/api/v1/receipts/scan` | Tải ảnh hóa đơn lên để AI Vision OCR bóc tách (Multipart/form-data) | Bearer JWT |
| `POST` | `/api/v1/receipts/{id}/audit`| **Hậu kiểm toán số học hóa đơn (`verifyMathematicalAudit`)** | Bearer JWT |
| `POST` | `/api/v1/receipts/{id}/confirm`| Phê duyệt kết quả bóc tách và tạo giao dịch chi tiêu tương ứng | Bearer JWT |

### 3.4. Phân Hệ 4: Budgeting, Adaptive DSA & Subscriptions (7 Endpoints)
| HTTP Method | Endpoint URI | Mục Đích Nghiệp Vụ & DTO Contracts | Yêu Cầu Auth |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/v1/budgets/current` | Lấy kế hoạch ngân sách chu kỳ hiện tại và tiến độ chi tiêu thực tế | Bearer JWT |
| `POST` | `/api/v1/budgets` | Thiết lập ngân sách mới (Chọn 50/30/20, Zero-Based hoặc Kakeibo) | Bearer JWT |
| `PUT` | `/api/v1/budgets/allocations`| Điều chỉnh hạn mức từng nhóm danh mục trong tháng | Bearer JWT |
| `GET` | `/api/v1/budgets/dsa/today` | **Lấy hạn mức chi tiêu hôm nay (DSA) & Hệ số tốc độ tiêu tiền (Spending Velocity)** | Bearer JWT |
| `GET` | `/api/v1/subscriptions` | Lấy danh sách dịch vụ đăng ký gia hạn định kỳ (Netflix, Spotify, 4G...) | Bearer JWT |
| `POST` | `/api/v1/subscriptions` | Thêm cam kết định kỳ kèm lịch nhắc trước 3 ngày | Bearer JWT |
| `GET` | `/api/v1/subscriptions/audit`| Báo cáo kiểm toán chi phí ẩn và đề xuất hủy dịch vụ lãng phí | Bearer JWT |

### 3.5. Phân Hệ 5: Financial Goals & Debt Optimizer Context (6 Endpoints)
| HTTP Method | Endpoint URI | Mục Đích Nghiệp Vụ & DTO Contracts | Yêu Cầu Auth |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/v1/goals` | Lấy danh sách mục tiêu tài chính & **Quỹ khẩn cấp (Emergency Fund)** | Bearer JWT |
| `POST` | `/api/v1/goals` | Khởi tạo mục tiêu tích lũy mới | Bearer JWT |
| `POST` | `/api/v1/goals/{id}/contribute`| Trích tiền từ ví sang mục tiêu tích lũy | Bearer JWT |
| `GET` | `/api/v1/debts` | Lấy danh mục các khoản nợ cá nhân (Thẻ tín dụng, vay ngân hàng) | Bearer JWT |
| `POST` | `/api/v1/debts` | Thêm khoản nợ mới (Số dư nợ, Lãi suất APR, Mức trả tối thiểu) | Bearer JWT |
| `POST` | `/api/v1/debts/simulate` | **Mô phỏng đối sánh chiến lược trả nợ (Debt Snowball vs. Debt Avalanche Solver)** | Bearer JWT |

### 3.6. Phân Hệ 6: Behavioral Nudges & Decision Support Thaler (5 Endpoints)
| HTTP Method | Endpoint URI | Mục Đích Nghiệp Vụ & DTO Contracts | Yêu Cầu Auth |
| :--- | :--- | :--- | :--- |
| `GET` | `/api/v1/nudges/active` | Lấy danh sách các cú hích đang chờ phản hồi (Kèm câu giải trình XAI) | Bearer JWT |
| `POST` | `/api/v1/nudges/{id}/decision` | Người dùng xác nhận: Đồng ý làm theo cú hích hoặc Bỏ qua | Bearer JWT |
| `POST` | `/api/v1/nudges/cooling-off` | **Kích hoạt Khoảng trễ tâm lý Cooling-off 24h-48h cho khoản mua sắm lớn bốc đồng** | Bearer JWT |
| `GET` | `/api/v1/analytics/forecast` | Dự báo dòng tiền 30-90 ngày tới & **Ngày cạn kiệt tiền mặt (Out-of-Money Date)** | Bearer JWT |
| `POST` | `/api/v1/payments/vietqr` | Sinh mã VietQR động để chuyển khoản thanh toán nhanh hóa đơn | Bearer JWT |

### 3.7. Phân Hệ 7: Two-Tier Offline-First Synchronization (2 Endpoints)
| HTTP Method | Endpoint URI | Mục Đích Nghiệp Vụ & DTO Contracts | Yêu Cầu Auth |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/v1/sync/push` | Mobile gửi mảng các giao dịch ngoại tuyến (`is_synced = 0`) lên Server | Bearer JWT |
| `GET` | `/api/v1/sync/pull` | Mobile kéo các biến động mới phát sinh từ Server kể từ `lastSyncTimestamp` | Bearer JWT |

---

### 3.8. SignalR Realtime Hub Specification (`FinanceHub`)

* **Hub Route:** `/hubs/finance`
* **Xác thực:** Gửi Access Token qua query string `?access_token={jwt}` khi thiết lập WebSocket.
* **Server-to-Client Event Handlers (Phía Mobile bắt sự kiện):**

```csharp
// 1. Nhận cập nhật số dư ví tức thời (0ms)
hubConnection.On<WalletBalanceUpdatedPayload>("ReceiveBalanceUpdated", payload =>
{
    // Cập nhật ObservableCollection trên UI
});

// 2. Nhận thông báo hoàn tất bóc tách hóa đơn AI
hubConnection.On<ReceiptOcrCompletedPayload>("ReceiveReceiptOcrCompleted", payload =>
{
    // Mở popup xác nhận hóa đơn cho người dùng
});

// 3. Nhận cảnh báo tốc độ tiêu tiền DSA
hubConnection.On<DsaAlertPayload>("ReceiveDsaAlert", payload =>
{
    // Hiển thị Cú hích Thaler Nudge
});
```

---

## CHƯƠNG 4: XÁC THỰC & BẢO MẬT PHẦN CỨNG DI ĐỘNG (AUTH & SECURITY)

### 4.1. Luồng Xác Thực Hai Lớp & Tự Động Xoay Vòng (Dual-Token & Silent Refresh)

```text
[Mobile .NET MAUI]                                          [ASP.NET Core Web API]
        │                                                              │
        │── 1. Gửi Email + Password ──────────────────────────────────►│
        │◄─ 2. Trả về AccessToken (15m) + RefreshToken (30d) ──────────│
        │                                                              │
[Lưu vào SecureStorage]                                                │
(Android KeyStore / iOS Keychain)                                      │
        │                                                              │
        │── 3. Gửi Request API kèm Authorization: Bearer {AccessToken}►│
        │◄─ 4. Khi Token hết hạn: Trả về HTTP 401 Unauthorized ────────│
        │                                                              │
[Refit DelegatingHandler bắt 401]                                      │
        │── 5. Gọi /api/v1/auth/refresh-token kèm RefreshToken ───────►│
        │◄─ 6. Trả về AccessToken mới + RefreshToken mới (Rotation) ───│
        │                                                              │
[Cập nhật lại SecureStorage]                                           │
        │── 7. Tự động gửi lại Request ban đầu (Seamless 0ms) ────────►│
        │◄─ 8. Trả về dữ liệu thành công ──────────────────────────────│
```

### 4.2. Đặc Tả Lớp Khóa Giao Diện (Mobile App Lock & Inactivity Timeout)
* **Phân loại ưu tiên:** **Độ ưu tiên Thấp (Low Priority / Future Backlog)** — Không nằm trong phạm vi bắt buộc của MVP hiện tại, ưu tiên sau khi hoàn tất kiểm thử API và tài liệu báo cáo.
* **Mục đích:** Bảo vệ quyền riêng tư số dư tài chính cá nhân khỏi người ngoài khi mượn thiết bị, đồng thời triệt tiêu ma sát đăng nhập lại bằng mật khẩu tài khoản dài dòng.
* **Cơ chế kỹ thuật:**
  * **Vòng đời ứng dụng (App Lifecycle):** Theo dõi sự kiện `OnSleep` (ứng dụng vào background) và `OnResume` (ứng dụng quay lại foreground).
  * **Bộ đếm thời gian không hoạt động (Inactivity Timer):** Nếu thời gian ở trạng thái background vượt quá ngưỡng cấu hình (ví dụ: 2 phút hoặc 5 phút), kích hoạt cờ trạng thái `IsAppLocked = true`.
  * **Giao diện mở khóa nhanh:** Hiển thị màn hình khóa nhẹ yêu cầu xác thực bằng **Sinh trắc học (FaceID / Vân tay)** hoặc **Mã PIN 4–6 số**.
  * **Nguyên tắc an toàn:** Phiên làm việc ngầm (`RefreshToken` trong `SecureStorage`) vẫn được giữ nguyên để tiến trình bóc tách thông báo ngân hàng chạy thông suốt; lớp khóa chỉ đóng vai trò che chắn màn hình giao diện (UI Curtain).

---

## CHƯƠNG 5: GIẢ LẬP DỊCH VỤ NGOẠI VI (SERVICE MOCKING ARCHITECTURE)

Để đảm bảo việc lập trình và kiểm thử Unit Test không bị phụ thuộc vào mạng ngoài hay quota API của Google Gemini / VietQR:

```csharp
// Abstraction Interface
public interface IReceiptOcrService
{
    Task<Result<ReceiptOcrResultDto>> ScanReceiptAsync(Stream imageStream, CancellationToken ct);
}

// 1. Mock Implementation (Dùng khi Offline / Dev / Testing)
public sealed class MockReceiptOcrService : IReceiptOcrService
{
    public Task<Result<ReceiptOcrResultDto>> ScanReceiptAsync(Stream imageStream, CancellationToken ct)
    {
        var mockResult = new ReceiptOcrResultDto(
            MerchantName: "Siêu thị WinMart+ Thảo Điền",
            InvoiceDate: DateTime.Today,
            TotalAmount: 245000m,
            VatAmount: 18000m,
            Items: new List<ReceiptItemDto>
            {
                new("Sữa tươi tiệt trùng Vinamilk 1L", 2, 38000m, 76000m),
                new("Bánh mì sandwich bơ đậu phộng", 1, 29000m, 29000m),
                new("Trái táo Envy New Zealand", 1.2m, 100000m, 120000m)
            }
        );
        return Task.FromResult(Result<ReceiptOcrResultDto>.Success(mockResult));
    }
}

// 2. Real Gemini Vision Implementation
public sealed class GeminiVisionOcrService : IReceiptOcrService
{
    // Gọi Google Gemini 1.5 Pro / Flash Multimodal API bóc tách JSON
}
```

* Cấu hình trong `appsettings.json`:
  ```json
  {
    "Features": {
      "UseMockOcr": true,
      "UseMockVietQr": true
    }
  }
  ```

---

## CHƯƠNG 6: QUY TRÌNH KIỂM THỬ ĐƠN VỊ CUỐN CHIẾU (TESTING & CODE CRAFT)

1. **Nguyên tắc cuốn chiếu:** Mỗi Entity / Value Object / Use Case được viết code xong, lập tức tạo tệp Test tương ứng trong thư mục `tests/`:
   - `MoneyTests.cs`: Kiểm thử cộng trừ tiền tệ bất biến, chia tỷ lệ, kiểm tra ngoại lệ âm tiền.
   - `ReceiptMathAuditTests.cs`: Kiểm thử thuật toán hậu kiểm toán số học ($\sum(\text{SL} \times \text{ĐG}) + \text{VAT} = \text{Total}$).
   - `DsaCalculatorTests.cs`: Kiểm thử tự động thích ứng hạn mức ngày và nhân hệ số ngày cuối tuần.
   - `DebtSnowballAvalancheTests.cs`: Kiểm thử đối sánh kết quả trả nợ giữa Quả cầu tuyết và Tuyết lở.
2. **Quy chuẩn Assertions:** Sử dụng `FluentAssertions` để câu lệnh kiểm thử mạch lạc, tự nhiên:
   ```csharp
   [Fact]
   public void Money_Add_DifferentCurrencies_ShouldFail()
   {
       var vnd = Money.FromVnd(100000);
       var usd = new Money(50, "USD");

       Action act = () => vnd.Add(usd);

       act.Should().Throw<InvalidOperationException>()
          .WithMessage("*Cannot operate on different currencies*");
   }
   ```
