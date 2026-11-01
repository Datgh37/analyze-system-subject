using ExpenseTracker.Maui.Models.SavingsGoals;

namespace ExpenseTracker.Maui.Repositories;

public class MockEmergencyFundRepository : IEmergencyFundRepository
{
    private EmergencyFundStatusModel _status;
    private decimal _monthlyNeeds = 6_000_000m;

    public MockEmergencyFundRepository()
    {
        _status = new EmergencyFundStatusModel
        {
            SavingsGoalId = Guid.NewGuid(),
            HasFundConfigured = true,
            TargetMonths = 6,
            TargetAmount = 36_000_000m,
            CurrentAmount = 18_000_000m,
            MonthlyEssentialExpenses = 6_000_000m,
            MonthsCovered = 3.0m,
            ProgressPercent = 50.0m,
            IsLocked = true,
            Status = "Active",
            LastDepositAt = DateTime.UtcNow.AddDays(-5)
        };
    }

    public Task<EmergencyFundRecommendationModel> GetRecommendationAsync()
    {
        var model = new EmergencyFundRecommendationModel
        {
            MonthlyEssentialExpenses = _monthlyNeeds,
            ThreeMonthsAmount = _monthlyNeeds * 3m,
            SixMonthsAmount = _monthlyNeeds * 6m,
            TwelveMonthsAmount = _monthlyNeeds * 12m,
            CurrentEmergencyFundAmount = _status.CurrentAmount,
            CurrentMonthsCovered = _monthlyNeeds > 0 ? Math.Round(_status.CurrentAmount / _monthlyNeeds, 1) : 0m,
            RecommendationNote = "Mức chuẩn mực 6 tháng là khuyến nghị tối ưu nhất cho bạn."
        };
        return Task.FromResult(model);
    }

    public Task<EmergencyFundStatusModel> GetStatusAsync()
    {
        return Task.FromResult(_status);
    }

    public Task<EmergencyFundStatusModel> SetupFundAsync(SetupEmergencyFundRequest request)
    {
        _status.HasFundConfigured = true;
        _status.TargetMonths = request.TargetMonths;
        _status.TargetAmount = request.TargetAmount ?? (_monthlyNeeds * request.TargetMonths);
        if (request.InitialDepositAmount.HasValue && request.InitialDepositAmount.Value > 0)
        {
            _status.CurrentAmount += request.InitialDepositAmount.Value;
            _status.LastDepositAt = DateTime.UtcNow;
        }
        _status.MonthsCovered = _monthlyNeeds > 0 ? Math.Round(_status.CurrentAmount / _monthlyNeeds, 1) : 0m;
        _status.ProgressPercent = _status.TargetAmount > 0 
            ? Math.Clamp(Math.Round((_status.CurrentAmount / _status.TargetAmount) * 100m, 1), 0m, 100m) 
            : 0m;
        return Task.FromResult(_status);
    }

    public Task<EmergencyFundStatusModel> DepositAsync(DepositEmergencyFundRequest request)
    {
        _status.CurrentAmount += request.Amount;
        _status.LastDepositAt = DateTime.UtcNow;
        _status.MonthsCovered = _monthlyNeeds > 0 ? Math.Round(_status.CurrentAmount / _monthlyNeeds, 1) : 0m;
        _status.ProgressPercent = _status.TargetAmount > 0 
            ? Math.Clamp(Math.Round((_status.CurrentAmount / _status.TargetAmount) * 100m, 1), 0m, 100m) 
            : 0m;
        return Task.FromResult(_status);
    }

    public Task<EmergencyFundStatusModel> WithdrawAsync(WithdrawEmergencyFundRequest request)
    {
        if (request.Amount > _status.CurrentAmount)
            throw new InvalidOperationException("Số dư không đủ.");

        _status.CurrentAmount -= request.Amount;
        _status.MonthsCovered = _monthlyNeeds > 0 ? Math.Round(_status.CurrentAmount / _monthlyNeeds, 1) : 0m;
        _status.ProgressPercent = _status.TargetAmount > 0 
            ? Math.Clamp(Math.Round((_status.CurrentAmount / _status.TargetAmount) * 100m, 1), 0m, 100m) 
            : 0m;
        return Task.FromResult(_status);
    }
}

