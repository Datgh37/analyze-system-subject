using ExpenseTracker.Maui.Models.RecurringIncomes;

namespace ExpenseTracker.Maui.Repositories;

// Abstract repository interface for recurring income schedules (Clean MVVM Architecture - UC-02.07)
public interface IRecurringIncomeRepository
{
    Task<List<RecurringIncomeModel>> GetRecurringIncomesAsync();
    Task<RecurringIncomeModel> GetRecurringIncomeByIdAsync(Guid id);
    Task<RecurringIncomeModel> CreateRecurringIncomeAsync(CreateRecurringIncomeModel request);
    Task<RecurringIncomeModel> UpdateRecurringIncomeAsync(Guid id, UpdateRecurringIncomeModel request);
    Task<RecurringIncomeModel> ToggleRecurringIncomeStatusAsync(Guid id);
    Task DeleteRecurringIncomeAsync(Guid id);
    Task<ProcessRecurringIncomeResultModel> ProcessDueRecurringIncomesAsync(ProcessRecurringIncomesModel? request = null);
}
