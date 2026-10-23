using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations;

// Entity configuration mapping Category aggregate to PostgreSQL categories table
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("categories");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.UserId)
            .IsRequired(false);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.TransactionType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(c => c.ParentId)
            .IsRequired(false);

        builder.Property(c => c.IconKey)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("folder");

        builder.Property(c => c.ColorHex)
            .IsRequired()
            .HasMaxLength(20)
            .HasDefaultValue("#10B981");

        builder.Property(c => c.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(c => c.IsSystem)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.IncomeGroup)
            .IsRequired(false)
            .HasConversion<int>();

        builder.Property(c => c.BudgetRuleMapping)
            .IsRequired(false)
            .HasConversion<int>();

        builder.Property(c => c.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        // Foreign keys and relationships
        builder.HasOne(c => c.User)
            .WithMany(u => u.Categories)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Parent)
            .WithMany(c => c.Children)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes for fast tree rendering and user lookup
        builder.HasIndex(c => new { c.UserId, c.TransactionType, c.IsDeleted });
        builder.HasIndex(c => c.ParentId);
        builder.HasIndex(c => c.IsSystem);
    }
}
