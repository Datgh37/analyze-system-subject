using ExpenseTracker.Maui.Models.Debts;

namespace ExpenseTracker.Maui.Repositories;

// Giao diện Repository quản lý danh mục nợ cá nhân và thanh toán nợ (Clean MVVM)
public interface IDebtRepository
{
    Task<List<DebtModel>> GetDebtsAsync();
    Task<DebtDetailModel?> GetDebtByIdAsync(Guid id);
    Task<DebtSummaryModel> GetDebtSummaryAsync();
    Task<DebtModel> CreateDebtAsync(CreateDebtRequest request);
    Task<DebtModel> UpdateDebtAsync(Guid id, UpdateDebtRequest request);
    Task<DebtTransactionModel> RecordPaymentAsync(Guid id, RecordDebtPaymentRequest request);
    Task<bool> ArchiveDebtAsync(Guid id);

    // Các phương thức tối ưu hóa và hướng dẫn tài chính bình dân hóa (UC-06.06)
    Task<DebtEducationalGuideModel> GetEducationalGuideAsync();
    Task<SafeExtraBudgetRecommendationModel> GetSafeBudgetRecommendationAsync();
    Task<DebtPayoffComparisonModel> CompareStrategiesAsync(decimal extraBudget, List<DebtSimulationItemModel>? customDebts = null);
    Task<DebtStrategySimulationResultModel> SimulateStrategyAsync(string strategy, decimal extraBudget, List<DebtSimulationItemModel>? customDebts = null);
}

