using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations;

public class NetWorthSnapshotConfiguration : IEntityTypeConfiguration<NetWorthSnapshot>
{
    public void Configure(EntityTypeBuilder<NetWorthSnapshot> builder)
    {
        builder.ToTable("net_worth_snapshots");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.UserId)
            .IsRequired();

        builder.Property(s => s.SnapshotDate)
            .IsRequired();

        builder.Property(s => s.TotalAssets)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.TotalLiabilities)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.NetWorth)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.Notes)
            .IsRequired(false)
            .HasMaxLength(500);

        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Fast temporal queries and upsert support
        builder.HasIndex(s => new { s.UserId, s.SnapshotDate })
            .HasDatabaseName("ix_net_worth_snapshots_user_date");
    }
}
