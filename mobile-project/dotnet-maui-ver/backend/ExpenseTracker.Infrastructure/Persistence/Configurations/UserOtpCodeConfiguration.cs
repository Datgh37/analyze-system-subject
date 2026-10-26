using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations;

// Entity configuration mapping UserOtpCode entity to PostgreSQL user_otp_codes table
public class UserOtpCodeConfiguration : IEntityTypeConfiguration<UserOtpCode>
{
    public void Configure(EntityTypeBuilder<UserOtpCode> builder)
    {
        builder.ToTable("user_otp_codes");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(o => o.PhoneNumber)
            .HasMaxLength(32)
            .IsRequired(false);

        builder.Property(o => o.OtpCode)
            .IsRequired()
            .HasMaxLength(16);

        builder.Property(o => o.Purpose)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(o => o.ExpiredAt)
            .IsRequired();

        builder.Property(o => o.IsUsed)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(o => o.UsedAt)
            .IsRequired(false);

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.UpdatedAt)
            .IsRequired(false);

        builder.Property(o => o.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(o => o.DeletedAt)
            .IsRequired(false);

        // Fast lookup composite index for OTP verification queries
        builder.HasIndex(o => new { o.Email, o.Purpose, o.IsUsed });

        builder.HasIndex(o => o.UserId);

        builder.HasOne(o => o.User)
            .WithMany(u => u.OtpCodes)
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
