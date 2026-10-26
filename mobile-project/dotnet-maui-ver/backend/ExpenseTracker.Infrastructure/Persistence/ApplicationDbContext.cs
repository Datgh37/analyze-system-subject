using System.Linq.Expressions;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore.Storage;

namespace ExpenseTracker.Infrastructure.Persistence;

// EF Core database context implementation for PostgreSQL persistence
public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<BalanceAdjustment> BalanceAdjustments => Set<BalanceAdjustment>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<WalletTransfer> WalletTransfers => Set<WalletTransfer>();
    public DbSet<RecurringIncome> RecurringIncomes => Set<RecurringIncome>();
    public DbSet<SalarySlipDetail> SalarySlipDetails => Set<SalarySlipDetail>();
    public DbSet<AutoCategorizationRule> AutoCategorizationRules => Set<AutoCategorizationRule>();
    public DbSet<ReceiptItem> ReceiptItems => Set<ReceiptItem>();
    public DbSet<Budget> Budgets => Set<Budget>();
    public DbSet<BudgetAllocation> BudgetAllocations => Set<BudgetAllocation>();
    public DbSet<DailySpendingAllowance> DailySpendingAllowances => Set<DailySpendingAllowance>();
    public DbSet<SavingsGoal> SavingsGoals => Set<SavingsGoal>();
    public DbSet<SavingsGoalContribution> SavingsGoalContributions => Set<SavingsGoalContribution>();
    public DbSet<InAppNotification> InAppNotifications => Set<InAppNotification>();
    public DbSet<DevicePushToken> DevicePushTokens => Set<DevicePushToken>();
    public DbSet<NetWorthSnapshot> NetWorthSnapshots => Set<NetWorthSnapshot>();
    public DbSet<FinancialNudge> FinancialNudges => Set<FinancialNudge>();
    public DbSet<CoolingOffTransaction> CoolingOffTransactions => Set<CoolingOffTransaction>();
    public DbSet<UserOtpCode> UserOtpCodes => Set<UserOtpCode>();

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task<Wallet?> GetWalletForUpdateAsync(Guid walletId, Guid userId, CancellationToken cancellationToken = default)
    {
        if (Database.ProviderName?.Contains("Npgsql", StringComparison.OrdinalIgnoreCase) == true)
        {
            return await Wallets
                .FromSqlRaw("SELECT * FROM \"wallets\" WHERE \"Id\" = {0} AND \"UserId\" = {1} AND \"IsDeleted\" = false FOR UPDATE", walletId, userId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return await Wallets
            .FirstOrDefaultAsync(w => w.Id == walletId && w.UserId == userId, cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        // Global query filter to automatically exclude soft-deleted entities
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                var falseConstant = Expression.Constant(false);
                var lambda = Expression.Lambda(Expression.Equal(property, falseConstant), parameter);
                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    // Handles automatic audit timestamps and soft deletion on entity lifecycle changes
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.MarkAsDeleted();
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
