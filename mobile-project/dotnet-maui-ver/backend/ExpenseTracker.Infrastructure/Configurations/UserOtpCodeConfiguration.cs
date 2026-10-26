using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Configurations;

public class UserOtpCodeConfiguration : IEntityTypeConfiguration<UserOtpCode>
{
    public void Configure(EntityTypeBuilder<UserOtpCode> builder)
    {
        builder.ToTable("user_otp_codes");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(o => o.PhoneNumber)
            .HasMaxLength(32);

        builder.Property(o => o.OtpCode)
            .HasMaxLength(16)
            .IsRequired();

        builder.Property(o => o.Purpose)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(o => new { o.Email, o.Purpose, o.IsUsed });
        builder.HasIndex(o => o.ExpiredAt);
    }
}
