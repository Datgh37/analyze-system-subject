using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations;

public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> builder)
    {
        builder.ToTable("budgets");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.UserId)
            .IsRequired();

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(b => b.PeriodType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(b => b.StartDate)
            .IsRequired();

        builder.Property(b => b.EndDate)
            .IsRequired();

        builder.Property(b => b.TotalAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(b => b.Method)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(b => b.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.Property(b => b.UpdatedAt)
            .IsRequired(false);

        // Relationships
        builder.HasMany(b => b.Allocations)
            .WithOne(a => a.Budget)
            .HasForeignKey(a => a.BudgetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(b => b.DailyAllowances)
            .WithOne(d => d.Budget)
            .HasForeignKey(d => d.BudgetId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(b => new { b.UserId, b.Status });
        builder.HasIndex(b => new { b.UserId, b.StartDate, b.EndDate });
    }
}
