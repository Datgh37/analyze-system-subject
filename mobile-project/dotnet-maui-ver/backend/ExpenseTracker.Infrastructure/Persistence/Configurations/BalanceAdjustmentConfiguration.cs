using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations;

// Entity configuration mapping BalanceAdjustment to PostgreSQL balance_adjustments table
public class BalanceAdjustmentConfiguration : IEntityTypeConfiguration<BalanceAdjustment>
{
    public void Configure(EntityTypeBuilder<BalanceAdjustment> builder)
    {
        builder.ToTable("balance_adjustments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.WalletId)
            .IsRequired();

        builder.Property(a => a.OldBalance)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(a => a.NewBalance)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(a => a.DifferenceAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(a => a.Reason)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(a => a.AdjustmentDate)
            .IsRequired();

        builder.Property(a => a.ProofImageUrl)
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .IsRequired(false);

        builder.Property(a => a.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(a => a.DeletedAt)
            .IsRequired(false);

        builder.HasIndex(a => new { a.WalletId, a.AdjustmentDate });

        builder.HasOne(a => a.Wallet)
            .WithMany(w => w.BalanceAdjustments)
            .HasForeignKey(a => a.WalletId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
