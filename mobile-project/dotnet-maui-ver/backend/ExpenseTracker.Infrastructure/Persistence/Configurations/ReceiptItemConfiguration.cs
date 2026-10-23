using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations;

// Entity configuration mapping ReceiptItem aggregate to PostgreSQL receipt_items table
public class ReceiptItemConfiguration : IEntityTypeConfiguration<ReceiptItem>
{
    public void Configure(EntityTypeBuilder<ReceiptItem> builder)
    {
        builder.ToTable("receipt_items");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.TransactionId)
            .IsRequired();

        builder.Property(r => r.ItemName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Quantity)
            .IsRequired()
            .HasPrecision(18, 3);

        builder.Property(r => r.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(r => r.TotalPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(r => r.CategoryId)
            .IsRequired(false);

        builder.Property(r => r.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        // Foreign keys and relationships
        builder.HasOne(r => r.Transaction)
            .WithMany(t => t.ReceiptItems)
            .HasForeignKey(r => r.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Category)
            .WithMany()
            .HasForeignKey(r => r.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(r => r.TransactionId);
    }
}
