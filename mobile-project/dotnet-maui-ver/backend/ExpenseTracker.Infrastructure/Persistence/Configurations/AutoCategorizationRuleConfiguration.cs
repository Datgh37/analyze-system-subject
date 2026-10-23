using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations;

// Entity configuration mapping AutoCategorizationRule aggregate to PostgreSQL auto_categorization_rules table
public class AutoCategorizationRuleConfiguration : IEntityTypeConfiguration<AutoCategorizationRule>
{
    public void Configure(EntityTypeBuilder<AutoCategorizationRule> builder)
    {
        builder.ToTable("auto_categorization_rules");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.UserId)
            .IsRequired();

        builder.Property(r => r.KeywordPattern)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.TargetCategoryId)
            .IsRequired();

        builder.Property(r => r.PriorityOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(r => r.IsRegex)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(r => r.IsCaseSensitive)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(r => r.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        // Foreign keys and relationships
        builder.HasOne(r => r.User)
            .WithMany(u => u.AutoCategorizationRules)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.TargetCategory)
            .WithMany()
            .HasForeignKey(r => r.TargetCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Performance indexes for rule evaluation
        builder.HasIndex(r => new { r.UserId, r.PriorityOrder });
        builder.HasIndex(r => new { r.UserId, r.KeywordPattern });
    }
}
