using ExpenseTracker.Maui.Models.Payslips;
using Refit;

namespace ExpenseTracker.Maui.Repositories;

// Abstract repository interface for payslip AI vision OCR and payroll reconciliation (Clean MVVM Architecture)
public interface IPayslipRepository
{
    Task<PayslipParsedDataModel> ParsePayslipAsync(StreamPart file);
    Task<SalarySlipDetailModel> ConfirmPayslipAsync(ConfirmPayslipModel request);
    Task<SalarySlipDetailModel> GetPayslipDetailAsync(Guid transactionId);
}
