using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations;

// Entity configuration mapping SalarySlipDetail aggregate to PostgreSQL salary_slip_details table (UC-02.08)
public class SalarySlipDetailConfiguration : IEntityTypeConfiguration<SalarySlipDetail>
{
    public void Configure(EntityTypeBuilder<SalarySlipDetail> builder)
    {
        builder.ToTable("salary_slip_details");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.UserId)
            .IsRequired();

        builder.Property(s => s.TransactionId)
            .IsRequired();

        builder.Property(s => s.EmployerName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.PayPeriod)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.GrossSalary)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.NetSalary)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.SocialInsurance)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.HealthInsurance)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.UnemploymentInsurance)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.PersonalIncomeTax)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.Allowances)
            .IsRequired()
            .HasPrecision(18, 2)
            .HasDefaultValue(0m);

        builder.Property(s => s.Bonus)
            .IsRequired()
            .HasPrecision(18, 2)
            .HasDefaultValue(0m);

        builder.Property(s => s.OtherDeductions)
            .IsRequired()
            .HasPrecision(18, 2)
            .HasDefaultValue(0m);

        builder.Property(s => s.IsMathAudited)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(s => s.DiscrepancyAmount)
            .IsRequired()
            .HasPrecision(18, 2)
            .HasDefaultValue(0m);

        builder.Property(s => s.RawOcrJson)
            .HasColumnType("text");

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        // Foreign keys and relationships
        builder.HasOne(s => s.User)
            .WithMany(u => u.SalarySlipDetails)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // 1-1 relationship with Transaction
        builder.HasOne(s => s.Transaction)
            .WithOne(t => t.SalarySlipDetail)
            .HasForeignKey<SalarySlipDetail>(s => s.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Highly optimized indexes
        builder.HasIndex(s => s.TransactionId).IsUnique();
        builder.HasIndex(s => new { s.UserId, s.PayPeriod });
    }
}
