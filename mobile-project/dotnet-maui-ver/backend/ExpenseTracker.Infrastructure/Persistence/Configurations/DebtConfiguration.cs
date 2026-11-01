using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations;

public class DebtConfiguration : IEntityTypeConfiguration<Debt>
{
    public void Configure(EntityTypeBuilder<Debt> builder)
    {
        builder.ToTable("debts");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.UserId)
            .IsRequired();

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.CreditorName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.DebtType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(d => d.PrincipalAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(d => d.CurrentBalance)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(d => d.InterestRateAnnual)
            .IsRequired()
            .HasColumnType("decimal(5,2)");

        builder.Property(d => d.MinimumPayment)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(d => d.DueDateMonthly)
            .IsRequired();

        builder.Property(d => d.StartDate)
            .IsRequired();

        builder.Property(d => d.ExpectedPayoffDate)
            .IsRequired(false);

        builder.Property(d => d.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(d => d.Notes)
            .IsRequired(false)
            .HasMaxLength(1000);

        builder.HasOne(d => d.User)
            .WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.Transactions)
            .WithOne(t => t.Debt)
            .HasForeignKey(t => t.DebtId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(d => new { d.UserId, d.Status });
        builder.HasIndex(d => d.UserId);
    }
}

