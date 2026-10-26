using ExpenseTracker.Maui.Models.RecurringIncomes;
using ExpenseTracker.Maui.Services.Api;

namespace ExpenseTracker.Maui.Repositories;

// Production API repository connecting to backend ASP.NET Core WebApi
public class ApiRecurringIncomeRepository : IRecurringIncomeRepository
{
    private readonly IRecurringIncomeApi _api;

    public ApiRecurringIncomeRepository(IRecurringIncomeApi api)
    {
        _api = api;
    }

    public Task<List<RecurringIncomeModel>> GetRecurringIncomesAsync()
        => _api.GetRecurringIncomesAsync();

    public Task<RecurringIncomeModel> GetRecurringIncomeByIdAsync(Guid id)
        => _api.GetRecurringIncomeByIdAsync(id);

    public Task<RecurringIncomeModel> CreateRecurringIncomeAsync(CreateRecurringIncomeModel request)
        => _api.CreateRecurringIncomeAsync(request);

    public Task<RecurringIncomeModel> UpdateRecurringIncomeAsync(Guid id, UpdateRecurringIncomeModel request)
        => _api.UpdateRecurringIncomeAsync(id, request);

    public Task<RecurringIncomeModel> ToggleRecurringIncomeStatusAsync(Guid id)
        => _api.ToggleRecurringIncomeStatusAsync(id);

    public Task DeleteRecurringIncomeAsync(Guid id)
        => _api.DeleteRecurringIncomeAsync(id);

    public Task<ProcessRecurringIncomeResultModel> ProcessDueRecurringIncomesAsync(ProcessRecurringIncomesModel? request = null)
        => _api.ProcessDueRecurringIncomesAsync(request);
}
