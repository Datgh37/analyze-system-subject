using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExpenseTracker.Infrastructure.Persistence.Configurations;

// Entity configuration mapping RefreshToken to PostgreSQL refresh_tokens table
public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("refresh_tokens");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Token)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(t => t.Token);

        builder.Property(t => t.ExpiresAt)
            .IsRequired();

        builder.Property(t => t.RevokedAt)
            .IsRequired(false);

        builder.Property(t => t.ReplacedByToken)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt)
            .IsRequired(false);

        builder.Property(t => t.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(t => t.DeletedAt)
            .IsRequired(false);
    }
}
