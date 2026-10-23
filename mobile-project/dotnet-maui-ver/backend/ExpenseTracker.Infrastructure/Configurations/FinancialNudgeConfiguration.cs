using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Configurations;

public class FinancialNudgeConfiguration : IEntityTypeConfiguration<FinancialNudge>
{
    public void Configure(EntityTypeBuilder<FinancialNudge> builder)
    {
        builder.ToTable("FinancialNudges");
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(n => n.Message)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(n => n.TriggerEvent)
            .HasMaxLength(128);

        builder.Property(n => n.RationaleExplanationJson)
            .HasColumnType("text");

        builder.Property(n => n.RebalanceProposalJson)
            .HasColumnType("text");

        builder.HasOne(n => n.User)
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(n => new { n.UserId, n.Status });
        builder.HasIndex(n => new { n.UserId, n.NudgeType, n.CreatedAt });
    }
}
