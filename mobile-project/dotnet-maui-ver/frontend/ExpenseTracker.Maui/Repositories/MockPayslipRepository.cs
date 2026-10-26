using ExpenseTracker.Maui.Models.Payslips;
using ExpenseTracker.Maui.Models.Transactions;
using Refit;

namespace ExpenseTracker.Maui.Repositories;

// In-memory mock repository for AI Vision payslip parsing and payroll reconciliation
public class MockPayslipRepository : IPayslipRepository
{
    private readonly ITransactionRepository _transactionRepo;

    public MockPayslipRepository(ITransactionRepository transactionRepo)
    {
        _transactionRepo = transactionRepo;
    }

    public Task<PayslipParsedDataModel> ParsePayslipAsync(StreamPart file)
    {
        decimal gross = 25_000_000m;
        decimal bhxh = 25_000_000m * 0.08m; // 2,000,000
        decimal bhyt = 25_000_000m * 0.015m; // 375,000
        decimal bhtn = 25_000_000m * 0.01m;  // 250,000
        decimal pit = 850_000m;
        decimal allowances = 1_500_000m; // Phụ cấp ăn trưa & xăng xe
        decimal bonus = 2_000_000m;      // Thưởng hiệu suất KPI
        decimal deductions = bhxh + bhyt + bhtn + pit; // 3,475,000
        decimal net = gross + allowances + bonus - deductions; // 25M + 1.5M + 2M - 3.475M = 25,025,000

        var result = new PayslipParsedDataModel(
            EmployerName: "Công ty Cổ phần Công nghệ Fintech Toàn Cầu",
            PayPeriod: $"Tháng {DateTime.Now.Month:D2}/{DateTime.Now.Year}",
            GrossSalary: gross,
            NetSalary: net,
            SocialInsurance: bhxh,
            HealthInsurance: bhyt,
            UnemploymentInsurance: bhtn,
            PersonalIncomeTax: pit,
            Allowances: allowances,
            Bonus: bonus,
            OtherDeductions: 0m,
            IsMathAudited: true,
            DiscrepancyAmount: 0m,
            HasDiscrepancy: false,
            ConfidenceScore: 0.98,
            RawJson: null
        );

        return Task.FromResult(result);
    }

    public async Task<SalarySlipDetailModel> ConfirmPayslipAsync(ConfirmPayslipModel request)
    {
        var createTx = new CreateIncomeModel(
            WalletId: request.WalletId,
            CategoryId: request.CategoryId,
            Amount: request.NetSalary,
            TransactionDate: request.TransactionDate ?? DateTime.Now,
            Note: $"[Lương {request.PayPeriod}] {request.EmployerName}",
            IsWindfall: false,
            IsRecurring: true
        );

        var tx = await _transactionRepo.CreateIncomeAsync(createTx);

        var detail = new SalarySlipDetailModel(
            Id: Guid.NewGuid(),
            UserId: Guid.NewGuid(),
            TransactionId: tx.Id,
            EmployerName: request.EmployerName,
            PayPeriod: request.PayPeriod,
            GrossSalary: request.GrossSalary,
            NetSalary: request.NetSalary,
            SocialInsurance: request.SocialInsurance,
            HealthInsurance: request.HealthInsurance,
            UnemploymentInsurance: request.UnemploymentInsurance,
            PersonalIncomeTax: request.PersonalIncomeTax,
            Allowances: request.Allowances,
            Bonus: request.Bonus,
            OtherDeductions: request.OtherDeductions,
            IsMathAudited: true,
            DiscrepancyAmount: 0m,
            CreatedAt: DateTime.UtcNow
        );

        return detail;
    }

    public Task<SalarySlipDetailModel> GetPayslipDetailAsync(Guid transactionId)
    {
        var detail = new SalarySlipDetailModel(
            Id: Guid.NewGuid(),
            UserId: Guid.NewGuid(),
            TransactionId: transactionId,
            EmployerName: "Công ty Cổ phần Công nghệ Fintech Toàn Cầu",
            PayPeriod: $"Tháng {DateTime.Now.Month:D2}/{DateTime.Now.Year}",
            GrossSalary: 25_000_000m,
            NetSalary: 25_025_000m,
            SocialInsurance: 2_000_000m,
            HealthInsurance: 375_000m,
            UnemploymentInsurance: 250_000m,
            PersonalIncomeTax: 850_000m,
            Allowances: 1_500_000m,
            Bonus: 2_000_000m,
            OtherDeductions: 0m,
            IsMathAudited: true,
            DiscrepancyAmount: 0m,
            CreatedAt: DateTime.UtcNow
        );

        return Task.FromResult(detail);
    }
}
