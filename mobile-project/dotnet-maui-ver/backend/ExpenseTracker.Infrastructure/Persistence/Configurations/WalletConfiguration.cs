using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations;

// Entity configuration mapping Wallet aggregate to PostgreSQL wallets table
public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
{
    public void Configure(EntityTypeBuilder<Wallet> builder)
    {
        builder.ToTable("wallets");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.UserId)
            .IsRequired();

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.WalletType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(w => w.Balance)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(w => w.InitialBalance)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(w => w.Currency)
            .IsRequired()
            .HasMaxLength(10)
            .HasDefaultValue("VND");

        builder.Property(w => w.AccountNumber)
            .HasMaxLength(50)
            .IsRequired(false);

        builder.Property(w => w.BankName)
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(w => w.CreditLimit)
            .HasPrecision(18, 2)
            .IsRequired(false);

        builder.Property(w => w.StatementDay)
            .IsRequired(false);

        builder.Property(w => w.PaymentDueDay)
            .IsRequired(false);

        builder.Property(w => w.ColorHex)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("#0066FF");

        builder.Property(w => w.IconKey)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("wallet");

        builder.Property(w => w.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(w => w.IsExcludedFromTotal)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(w => w.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(w => w.Version)
            .IsRequired()
            .IsConcurrencyToken()
            .HasDefaultValue(1);

        builder.Property(w => w.CreatedAt)
            .IsRequired();

        builder.Property(w => w.UpdatedAt)
            .IsRequired(false);

        builder.Property(w => w.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(w => w.DeletedAt)
            .IsRequired(false);

        builder.HasIndex(w => new { w.UserId, w.Status });

        builder.HasOne(w => w.User)
            .WithMany(u => u.Wallets)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
