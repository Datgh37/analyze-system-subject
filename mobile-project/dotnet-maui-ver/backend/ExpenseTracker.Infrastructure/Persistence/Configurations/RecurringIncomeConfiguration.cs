using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations;

// Entity configuration mapping RecurringIncome aggregate to PostgreSQL recurring_incomes table
public class RecurringIncomeConfiguration : IEntityTypeConfiguration<RecurringIncome>
{
    public void Configure(EntityTypeBuilder<RecurringIncome> builder)
    {
        builder.ToTable("recurring_incomes");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.UserId)
            .IsRequired();

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(r => r.ExpectedAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(r => r.WalletId)
            .IsRequired();

        builder.Property(r => r.CategoryId)
            .IsRequired();

        builder.Property(r => r.Frequency)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(r => r.DayOfMonth)
            .IsRequired();

        builder.Property(r => r.AutoRecord)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(r => r.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(r => r.NextDueDate)
            .IsRequired();

        builder.Property(r => r.LastGeneratedDate);

        builder.Property(r => r.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        // Foreign keys and relationships
        builder.HasOne(r => r.User)
            .WithMany(u => u.RecurringIncomes)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Wallet)
            .WithMany()
            .HasForeignKey(r => r.WalletId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Category)
            .WithMany()
            .HasForeignKey(r => r.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Highly optimized indexes for queries & background auto-record scanner
        builder.HasIndex(r => new { r.UserId, r.IsDeleted });
        builder.HasIndex(r => new { r.IsActive, r.AutoRecord, r.NextDueDate });
    }
}
