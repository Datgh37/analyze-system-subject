using System.Security.Claims;
using ExpenseTracker.Domain.Entities;

namespace ExpenseTracker.Application.Interfaces;

// Token generation service contract for issuing access and refresh tokens
public interface IJwtTokenGenerator
{
    (string AccessToken, DateTime ExpiresAt) GenerateAccessToken(User user);
    RefreshToken GenerateRefreshToken(Guid userId);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
