using ExpenseTracker.Maui.Models.Payslips;
using ExpenseTracker.Maui.Services.Api;
using Refit;

namespace ExpenseTracker.Maui.Repositories;

// API implementation of payslip repository delegating to Refit IPayslipApi
public class ApiPayslipRepository : IPayslipRepository
{
    private readonly IPayslipApi _api;

    public ApiPayslipRepository(IPayslipApi api)
    {
        _api = api;
    }

    public Task<PayslipParsedDataModel> ParsePayslipAsync(StreamPart file) => _api.ParsePayslipAsync(file);
    public Task<SalarySlipDetailModel> ConfirmPayslipAsync(ConfirmPayslipModel request) => _api.ConfirmPayslipAsync(request);
    public Task<SalarySlipDetailModel> GetPayslipDetailAsync(Guid transactionId) => _api.GetPayslipDetailAsync(transactionId);
}
