using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations;

public class SavingsGoalContributionConfiguration : IEntityTypeConfiguration<SavingsGoalContribution>
{
    public void Configure(EntityTypeBuilder<SavingsGoalContribution> builder)
    {
        builder.ToTable("savings_goal_contributions");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.SavingsGoalId)
            .IsRequired();

        builder.Property(c => c.WalletId)
            .IsRequired();

        builder.Property(c => c.TransactionId)
            .IsRequired(false);

        builder.Property(c => c.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(c => c.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(c => c.ContributionDate)
            .IsRequired();

        builder.Property(c => c.Note)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        // Relationships
        builder.HasOne(c => c.Wallet)
            .WithMany()
            .HasForeignKey(c => c.WalletId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Transaction)
            .WithMany()
            .HasForeignKey(c => c.TransactionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(c => new { c.SavingsGoalId, c.ContributionDate });
        builder.HasIndex(c => c.WalletId);
    }
}
