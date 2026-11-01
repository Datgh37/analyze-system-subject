using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations;

public class BudgetAllocationConfiguration : IEntityTypeConfiguration<BudgetAllocation>
{
    public void Configure(EntityTypeBuilder<BudgetAllocation> builder)
    {
        builder.ToTable("budget_allocations");

        builder.HasKey(ba => ba.Id);

        builder.Property(ba => ba.BudgetId)
            .IsRequired();

        builder.Property(ba => ba.CategoryId)
            .IsRequired();

        builder.Property(ba => ba.AllocatedAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(ba => ba.CurrentSpent)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(ba => ba.AlertThresholdPercent)
            .IsRequired()
            .HasColumnType("decimal(5,2)")
            .HasDefaultValue(80m);

        // Relationships
        builder.HasOne(ba => ba.Category)
            .WithMany()
            .HasForeignKey(ba => ba.CategoryId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ba => new { ba.BudgetId, ba.CategoryId })
            .IsUnique();
    }
}
