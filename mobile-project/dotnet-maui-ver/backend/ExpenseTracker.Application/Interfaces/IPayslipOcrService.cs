using ExpenseTracker.Application.DTOs.Payslips;
using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Application.Interfaces;

// Service contract for AI Vision OCR payslip document extraction and mathematical audit (UC-02.08)
public interface IPayslipOcrService
{
    Task<Result<PayslipParsedDataDto>> ParsePayslipAsync(Stream fileStream, string fileName, CancellationToken ct = default);
    Task<Result<SalarySlipDetailDto>> ConfirmAndSavePayslipAsync(Guid userId, ConfirmPayslipRequest request, CancellationToken ct = default);
    Task<Result<SalarySlipDetailDto>> GetSalarySlipDetailByTransactionIdAsync(Guid userId, Guid transactionId, CancellationToken ct = default);
}
