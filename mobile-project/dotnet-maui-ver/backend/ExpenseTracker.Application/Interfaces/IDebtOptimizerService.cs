using ExpenseTracker.Application.DTOs.Debts;

namespace ExpenseTracker.Application.Interfaces;

// Giao diện nghiệp vụ động cơ tối ưu hóa trả nợ và hướng dẫn bình dân hóa tài chính (UC-06.06)
public interface IDebtOptimizerService
{
    // Cung cấp cẩm nang hướng dẫn bình dân hóa tài chính, giải thích trực quan và câu hỏi trắc nghiệm định hướng
    DebtEducationalGuideDto GetEducationalGuide();

    // Tính toán và gợi ý ngân sách trả thêm an toàn dựa trên tình hình thu chi thực tế của người dùng
    Task<SafeExtraBudgetRecommendationDto> GetSafeExtraBudgetRecommendationAsync(
        Guid userId, 
        CancellationToken ct = default);

    // So sánh song song hai chiến lược Snowball vs Avalanche và đưa ra khuyến nghị hành vi đời thường
    Task<DebtPayoffComparisonDto> CompareStrategiesAsync(
        Guid userId, 
        DebtPayoffSimulationRequestDto request, 
        CancellationToken ct = default);

    // Mô phỏng chi tiết chiến lược Snowball (Hòn tuyết lăn - Ưu tiên dư nợ nhỏ nhất trước)
    Task<DebtStrategySimulationResultDto> SimulateSnowballAsync(
        Guid userId, 
        decimal monthlyExtraBudget, 
        List<DebtSimulationItemDto>? customDebts = null, 
        CancellationToken ct = default);

    // Mô phỏng chi tiết chiến lược Avalanche (Tuyết lở - Ưu tiên lãi suất cao nhất trước)
    Task<DebtStrategySimulationResultDto> SimulateAvalancheAsync(
        Guid userId, 
        decimal monthlyExtraBudget, 
        List<DebtSimulationItemDto>? customDebts = null, 
        CancellationToken ct = default);
}

