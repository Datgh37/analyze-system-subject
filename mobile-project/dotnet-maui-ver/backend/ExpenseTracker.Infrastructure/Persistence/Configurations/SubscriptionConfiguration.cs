using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("subscriptions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.UserId)
            .IsRequired();

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.BillingCycle)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(s => s.NextBillingDate)
            .IsRequired();

        builder.Property(s => s.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(s => s.TrialEndDate)
            .IsRequired(false);

        builder.Property(s => s.AutoRenew)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.RemindDaysBefore)
            .IsRequired()
            .HasDefaultValue(3);

        builder.Property(s => s.Notes)
            .IsRequired(false)
            .HasMaxLength(1000);

        // Ignore computed properties
        builder.Ignore(s => s.AnnualCost);
        builder.Ignore(s => s.MonthlyCost);

        // Relationships
        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Wallet)
            .WithMany()
            .HasForeignKey(s => s.WalletId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(s => s.Category)
            .WithMany()
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => new { s.UserId, s.Status });
        builder.HasIndex(s => s.NextBillingDate);
    }
}

