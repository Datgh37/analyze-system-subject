using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations;

public class SavingsGoalConfiguration : IEntityTypeConfiguration<SavingsGoal>
{
    public void Configure(EntityTypeBuilder<SavingsGoal> builder)
    {
        builder.ToTable("savings_goals");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.UserId)
            .IsRequired();

        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(g => g.Description)
            .IsRequired(false)
            .HasMaxLength(1000);

        builder.Property(g => g.TargetAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(g => g.CurrentAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(g => g.StartDate)
            .IsRequired();

        builder.Property(g => g.TargetDate)
            .IsRequired();

        builder.Property(g => g.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(g => g.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(g => g.Priority)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(g => g.IconKey)
            .IsRequired(false)
            .HasMaxLength(50);

        builder.Property(g => g.ColorHex)
            .IsRequired(false)
            .HasMaxLength(50);

        builder.Property(g => g.CompletedAt)
            .IsRequired(false);

        builder.Property(g => g.IsAutoDepositEnabled)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(g => g.AutoDepositAmount)
            .IsRequired(false)
            .HasColumnType("decimal(18,2)");

        builder.Property(g => g.AutoDepositDayOfMonth)
            .IsRequired(false);

        builder.Property(g => g.AutoDepositWalletId)
            .IsRequired(false);

        builder.Property(g => g.CreatedAt)
            .IsRequired();

        builder.Property(g => g.UpdatedAt)
            .IsRequired(false);

        // Relationships
        builder.HasMany(g => g.Contributions)
            .WithOne(c => c.SavingsGoal)
            .HasForeignKey(c => c.SavingsGoalId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(g => new { g.UserId, g.Status });
        builder.HasIndex(g => new { g.UserId, g.TargetDate });
    }
}
