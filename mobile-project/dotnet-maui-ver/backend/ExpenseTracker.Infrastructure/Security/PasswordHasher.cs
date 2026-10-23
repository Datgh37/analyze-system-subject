using ExpenseTracker.Application.Interfaces;

namespace ExpenseTracker.Infrastructure.Security;

// BCrypt implementation for cryptographic password hashing and verification
public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password, workFactor: 11);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(password, passwordHash);
    }
}
