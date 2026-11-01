using ExpenseTracker.Application.DTOs.EmergencyFund;

namespace ExpenseTracker.Application.Interfaces;

// Service interface managing personal emergency fund calculation, lock, and anti-impulse discipline (UC-06.04)
public interface IEmergencyFundService
{
    Task<EmergencyFundRecommendationDto> GetRecommendationAsync(Guid userId, CancellationToken ct = default);
    Task<EmergencyFundStatusDto> GetStatusAsync(Guid userId, CancellationToken ct = default);
    Task<EmergencyFundStatusDto> SetupFundAsync(Guid userId, SetupEmergencyFundDto request, CancellationToken ct = default);
    Task<EmergencyFundStatusDto> DepositAsync(Guid userId, DepositEmergencyFundDto request, CancellationToken ct = default);
    Task<EmergencyFundStatusDto> WithdrawAsync(Guid userId, WithdrawEmergencyFundDto request, CancellationToken ct = default);
}

