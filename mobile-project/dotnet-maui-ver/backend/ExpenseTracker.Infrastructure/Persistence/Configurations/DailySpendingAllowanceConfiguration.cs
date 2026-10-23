using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations;

public class DailySpendingAllowanceConfiguration : IEntityTypeConfiguration<DailySpendingAllowance>
{
    public void Configure(EntityTypeBuilder<DailySpendingAllowance> builder)
    {
        builder.ToTable("daily_spending_allowances");

        builder.HasKey(dsa => dsa.Id);

        builder.Property(dsa => dsa.BudgetId)
            .IsRequired();

        builder.Property(dsa => dsa.Date)
            .IsRequired();

        builder.Property(dsa => dsa.DailyAllowanceAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(dsa => dsa.ActualSpentToday)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(dsa => dsa.RemainingAllowanceToday)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(dsa => dsa.VelocityIndex)
            .IsRequired()
            .HasColumnType("decimal(8,4)")
            .HasDefaultValue(1.0m);

        builder.Property(dsa => dsa.OutOfMoneyForecastDate)
            .IsRequired(false);

        builder.Property(dsa => dsa.CreatedAt)
            .IsRequired();

        builder.Property(dsa => dsa.UpdatedAt)
            .IsRequired(false);

        builder.HasIndex(dsa => new { dsa.BudgetId, dsa.Date })
            .IsUnique();
    }
}
