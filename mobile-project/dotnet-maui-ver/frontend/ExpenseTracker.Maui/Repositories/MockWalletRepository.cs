using ExpenseTracker.Maui.Models.Wallets;

namespace ExpenseTracker.Maui.Repositories;

// In-memory mock implementation of IWalletRepository for standalone testing and offline demo
public class MockWalletRepository : IWalletRepository
{
    private readonly List<WalletModel> _wallets = new();

    public MockWalletRepository()
    {
        var userId = Guid.NewGuid();

        _wallets.Add(new WalletModel(
            Id: Guid.Parse("11111111-1111-1111-1111-111111111111"),
            UserId: userId,
            Name: "Ví Tiền Mặt",
            WalletType: MobileWalletType.Cash,
            Balance: 2500000m,
            InitialBalance: 2500000m,
            Currency: "VND",
            AccountNumber: null,
            BankName: null,
            CreditLimit: null,
            StatementDay: null,
            PaymentDueDay: null,
            ColorHex: "#10B981",
            IconKey: "wallet",
            DisplayOrder: 1,
            IsExcludedFromTotal: false,
            Status: 1,
            Version: 1,
            CreatedAt: DateTime.UtcNow.AddDays(-30),
            UpdatedAt: null
        ));

        _wallets.Add(new WalletModel(
            Id: Guid.Parse("22222222-2222-2222-2222-222222222222"),
            UserId: userId,
            Name: "Tài khoản Vietcombank",
            WalletType: MobileWalletType.BankAccount,
            Balance: 15800000m,
            InitialBalance: 15800000m,
            Currency: "VND",
            AccountNumber: "9988776655",
            BankName: "Vietcombank",
            CreditLimit: null,
            StatementDay: null,
            PaymentDueDay: null,
            ColorHex: "#0066FF",
            IconKey: "bank",
            DisplayOrder: 2,
            IsExcludedFromTotal: false,
            Status: 1,
            Version: 1,
            CreatedAt: DateTime.UtcNow.AddDays(-30),
            UpdatedAt: null
        ));

        _wallets.Add(new WalletModel(
            Id: Guid.Parse("33333333-3333-3333-3333-333333333333"),
            UserId: userId,
            Name: "Ví Điện Tử MoMo",
            WalletType: MobileWalletType.EWallet,
            Balance: 1200000m,
            InitialBalance: 1200000m,
            Currency: "VND",
            AccountNumber: "0901234567",
            BankName: "MoMo",
            CreditLimit: null,
            StatementDay: null,
            PaymentDueDay: null,
            ColorHex: "#D82D8B",
            IconKey: "momo",
            DisplayOrder: 3,
            IsExcludedFromTotal: false,
            Status: 1,
            Version: 1,
            CreatedAt: DateTime.UtcNow.AddDays(-30),
            UpdatedAt: null
        ));
    }

    public Task<List<WalletModel>> GetWalletsAsync()
    {
        return Task.FromResult(_wallets.ToList());
    }

    public Task<WalletModel> GetWalletByIdAsync(Guid id)
    {
        var wallet = _wallets.FirstOrDefault(w => w.Id == id)
            ?? throw new KeyNotFoundException($"Không tìm thấy ví {id}");
        return Task.FromResult(wallet);
    }

    public Task<WalletModel> CreateWalletAsync(CreateWalletModel request)
    {
        var newWallet = new WalletModel(
            Id: Guid.NewGuid(),
            UserId: Guid.NewGuid(),
            Name: request.Name,
            WalletType: request.WalletType,
            Balance: request.InitialBalance,
            InitialBalance: request.InitialBalance,
            Currency: request.Currency,
            AccountNumber: request.AccountNumber,
            BankName: request.BankName,
            CreditLimit: request.CreditLimit,
            StatementDay: request.StatementDay,
            PaymentDueDay: request.PaymentDueDay,
            ColorHex: request.ColorHex,
            IconKey: request.IconKey,
            DisplayOrder: _wallets.Count + 1,
            IsExcludedFromTotal: request.IsExcludedFromTotal,
            Status: 1,
            Version: 1,
            CreatedAt: DateTime.UtcNow,
            UpdatedAt: null
        );
        _wallets.Add(newWallet);
        return Task.FromResult(newWallet);
    }

    public Task<WalletModel> UpdateWalletAsync(Guid id, UpdateWalletModel request)
    {
        var existing = _wallets.FirstOrDefault(w => w.Id == id)
            ?? throw new KeyNotFoundException($"Không tìm thấy ví {id}");
        var updated = existing with
        {
            Name = request.Name,
            AccountNumber = request.AccountNumber,
            BankName = request.BankName,
            CreditLimit = request.CreditLimit,
            StatementDay = request.StatementDay,
            PaymentDueDay = request.PaymentDueDay,
            ColorHex = request.ColorHex,
            IconKey = request.IconKey,
            IsExcludedFromTotal = request.IsExcludedFromTotal,
            UpdatedAt = DateTime.UtcNow
        };
        _wallets.Remove(existing);
        _wallets.Add(updated);
        return Task.FromResult(updated);
    }

    public Task DeleteWalletAsync(Guid id)
    {
        _wallets.RemoveAll(w => w.Id == id);
        return Task.CompletedTask;
    }

    public Task TransferMoneyAsync(TransferMoneyModel request)
    {
        var source = _wallets.FirstOrDefault(w => w.Id == request.SourceWalletId)
            ?? throw new KeyNotFoundException("Ví nguồn không tồn tại");
        var dest = _wallets.FirstOrDefault(w => w.Id == request.DestinationWalletId)
            ?? throw new KeyNotFoundException("Ví đích không tồn tại");

        if (source.Balance < request.Amount)
        {
            throw new InvalidOperationException("Số dư ví nguồn không đủ để thực hiện chuyển tiền");
        }

        _wallets.Remove(source);
        _wallets.Remove(dest);
        _wallets.Add(source with { Balance = source.Balance - request.Amount });
        _wallets.Add(dest with { Balance = dest.Balance + request.Amount });
        return Task.CompletedTask;
    }

    public Task<BalanceAdjustmentModel> AdjustBalanceAsync(Guid id, AdjustBalanceModel request)
    {
        var wallet = _wallets.FirstOrDefault(w => w.Id == id)
            ?? throw new KeyNotFoundException($"Không tìm thấy ví {id}");
        var diff = request.NewBalance - wallet.Balance;
        _wallets.Remove(wallet);
        _wallets.Add(wallet with { Balance = request.NewBalance });

        return Task.FromResult(new BalanceAdjustmentModel(
            Id: Guid.NewGuid(),
            WalletId: id,
            OldBalance: wallet.Balance,
            NewBalance: request.NewBalance,
            DifferenceAmount: diff,
            Reason: request.Reason,
            AdjustmentDate: request.AdjustmentDate ?? DateTime.UtcNow,
            ProofImageUrl: request.ProofImageUrl,
            CreatedAt: DateTime.UtcNow
        ));
    }
}
