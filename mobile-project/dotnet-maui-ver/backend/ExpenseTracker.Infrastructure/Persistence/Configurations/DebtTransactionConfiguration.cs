using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations;

public class DebtTransactionConfiguration : IEntityTypeConfiguration<DebtTransaction>
{
    public void Configure(EntityTypeBuilder<DebtTransaction> builder)
    {
        builder.ToTable("debt_transactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.DebtId)
            .IsRequired();

        builder.Property(t => t.WalletId)
            .IsRequired();

        builder.Property(t => t.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(t => t.PrincipalPaid)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(t => t.InterestPaid)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(t => t.PaymentDate)
            .IsRequired();

        builder.Property(t => t.Note)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.HasOne(t => t.Debt)
            .WithMany(d => d.Transactions)
            .HasForeignKey(t => t.DebtId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Wallet)
            .WithMany()
            .HasForeignKey(t => t.WalletId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => t.DebtId);
        builder.HasIndex(t => t.WalletId);
        builder.HasIndex(t => t.PaymentDate);
    }
}

