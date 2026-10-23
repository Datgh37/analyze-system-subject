using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations;

// Entity Framework Core mapping configuration for WalletTransfer
public class WalletTransferConfiguration : IEntityTypeConfiguration<WalletTransfer>
{
    public void Configure(EntityTypeBuilder<WalletTransfer> builder)
    {
        builder.ToTable("WalletTransfers");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(t => t.TransferDate)
            .IsRequired();

        builder.Property(t => t.Note)
            .HasMaxLength(500);

        builder.HasOne(t => t.User)
            .WithMany()
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.SourceWallet)
            .WithMany()
            .HasForeignKey(t => t.SourceWalletId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.DestinationWallet)
            .WithMany()
            .HasForeignKey(t => t.DestinationWalletId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => new { t.UserId, t.TransferDate });
        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}
