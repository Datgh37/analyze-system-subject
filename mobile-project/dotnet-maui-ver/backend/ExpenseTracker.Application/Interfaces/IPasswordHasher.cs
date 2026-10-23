namespace ExpenseTracker.Application.Interfaces;

// Cryptographic service abstraction for hashing and verifying passwords
public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
