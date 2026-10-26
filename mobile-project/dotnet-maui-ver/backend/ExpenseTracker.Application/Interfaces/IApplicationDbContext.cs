using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ExpenseTracker.Application.Interfaces;

// Application database abstraction providing access to domain aggregates and transaction control
public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<RefreshToken> RefreshTokens { get; }
    DbSet<Wallet> Wallets { get; }
    DbSet<BalanceAdjustment> BalanceAdjustments { get; }
    DbSet<Category> Categories { get; }
    DbSet<Transaction> Transactions { get; }
    DbSet<WalletTransfer> WalletTransfers { get; }
    DbSet<RecurringIncome> RecurringIncomes { get; }
    DbSet<SalarySlipDetail> SalarySlipDetails { get; }
    DbSet<AutoCategorizationRule> AutoCategorizationRules { get; }
    DbSet<ReceiptItem> ReceiptItems { get; }
    DbSet<Budget> Budgets { get; }
    DbSet<BudgetAllocation> BudgetAllocations { get; }
    DbSet<DailySpendingAllowance> DailySpendingAllowances { get; }
    DbSet<SavingsGoal> SavingsGoals { get; }
    DbSet<SavingsGoalContribution> SavingsGoalContributions { get; }
    DbSet<InAppNotification> InAppNotifications { get; }
    DbSet<DevicePushToken> DevicePushTokens { get; }
    DbSet<NetWorthSnapshot> NetWorthSnapshots { get; }
    DbSet<FinancialNudge> FinancialNudges { get; }
    DbSet<CoolingOffTransaction> CoolingOffTransactions { get; }

    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<Wallet?> GetWalletForUpdateAsync(Guid walletId, Guid userId, CancellationToken cancellationToken = default);
}
