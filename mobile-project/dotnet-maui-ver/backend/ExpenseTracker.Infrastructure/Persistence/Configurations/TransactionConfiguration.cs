using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations;

// Entity configuration mapping Transaction aggregate to PostgreSQL transactions table
public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.ToTable("transactions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.UserId)
            .IsRequired();

        builder.Property(t => t.WalletId)
            .IsRequired();

        builder.Property(t => t.CategoryId)
            .IsRequired();

        builder.Property(t => t.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(t => t.TransactionType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.TransactionDate)
            .IsRequired();

        builder.Property(t => t.Note)
            .HasMaxLength(500);

        builder.Property(t => t.IsEssential)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.IsWindfall)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.IsRecurring)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.ExcludeFromReport)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        // Foreign keys and relationships
        builder.HasOne(t => t.User)
            .WithMany(u => u.Transactions)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Wallet)
            .WithMany(w => w.Transactions)
            .HasForeignKey(t => t.WalletId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Category)
            .WithMany(c => c.Transactions)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Highly optimized composite indexes for timeline queries and wallet balances
        builder.HasIndex(t => new { t.UserId, t.TransactionDate, t.IsDeleted });
        builder.HasIndex(t => new { t.WalletId, t.IsDeleted });
        builder.HasIndex(t => new { t.CategoryId, t.IsDeleted });
        builder.HasIndex(t => new { t.UserId, t.TransactionType, t.TransactionDate });
    }
}
