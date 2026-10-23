using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Configurations;

public class CoolingOffTransactionConfiguration : IEntityTypeConfiguration<CoolingOffTransaction>
{
    public void Configure(EntityTypeBuilder<CoolingOffTransaction> builder)
    {
        builder.ToTable("CoolingOffTransactions");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.BypassReason)
            .HasMaxLength(500);

        builder.Property(c => c.EquivalentWorkHours)
            .HasPrecision(18, 2);

        builder.Property(c => c.ProjectedDsaReduction)
            .HasPrecision(18, 2);

        builder.HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Wallet)
            .WithMany()
            .HasForeignKey(c => c.WalletId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Category)
            .WithMany()
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => new { c.UserId, c.Status });
        builder.HasIndex(c => new { c.UserId, c.CoolingOffUntil });
    }
}
