using ExpenseTracker.Maui.Models.Auth;
using Refit;

namespace ExpenseTracker.Maui.Services.Api;

// Refit declarative HTTP client contract for authentication operations
public interface IAuthApi
{
    [Post("/api/auth/register")]
    Task<AuthResponse> RegisterAsync([Body] RegisterRequest request);

    [Post("/api/auth/login")]
    Task<AuthResponse> LoginAsync([Body] LoginRequest request);

    [Post("/api/auth/refresh-token")]
    Task<AuthResponse> RefreshTokenAsync([Body] RefreshTokenRequest request);

    [Post("/api/auth/verify-otp")]
    Task<AuthResponse> VerifyOtpAsync([Body] VerifyOtpRequest request);

    [Post("/api/auth/resend-otp")]
    Task<bool> ResendOtpAsync([Body] ResendOtpRequest request);

    [Post("/api/auth/forgot-password")]
    Task<bool> ForgotPasswordAsync([Body] ForgotPasswordRequest request);

    [Post("/api/auth/reset-password-otp")]
    Task<bool> ResetPasswordWithOtpAsync([Body] ResetPasswordWithOtpRequest request);

    [Post("/api/auth/google")]
    Task<AuthResponse> GoogleLoginAsync([Body] GoogleAuthRequest request);

    [Post("/api/auth/logout")]
    Task LogoutAsync([Body] object? payload = null);
}

// Refit declarative HTTP client contract for user profile operations
public interface IUserApi
{
    [Get("/api/users/profile")]
    Task<UserProfile> GetProfileAsync();

    [Put("/api/users/profile")]
    Task<UserProfile> UpdateProfileAsync([Body] UpdateProfileRequest request);

    [Put("/api/users/change-password")]
    Task ChangePasswordAsync([Body] ChangePasswordRequest request);
}

// Refit declarative HTTP client contract for multi-wallet and transfer operations
public interface IWalletApi
{
    [Get("/api/wallets")]
    Task<List<ExpenseTracker.Maui.Models.Wallets.WalletModel>> GetWalletsAsync();

    [Get("/api/wallets/{id}")]
    Task<ExpenseTracker.Maui.Models.Wallets.WalletModel> GetWalletByIdAsync(Guid id);

    [Post("/api/wallets")]
    Task<ExpenseTracker.Maui.Models.Wallets.WalletModel> CreateWalletAsync([Body] ExpenseTracker.Maui.Models.Wallets.CreateWalletModel request);

    [Put("/api/wallets/{id}")]
    Task<ExpenseTracker.Maui.Models.Wallets.WalletModel> UpdateWalletAsync(Guid id, [Body] ExpenseTracker.Maui.Models.Wallets.UpdateWalletModel request);

    [Patch("/api/wallets/{id}")]
    Task<ExpenseTracker.Maui.Models.Wallets.WalletModel> PatchWalletAsync(Guid id, [Body] ExpenseTracker.Maui.Models.Wallets.PatchWalletModel request);

    [Delete("/api/wallets/{id}")]
    Task DeleteWalletAsync(Guid id);

    [Post("/api/wallets/transfer")]
    Task TransferMoneyAsync([Body] ExpenseTracker.Maui.Models.Wallets.TransferMoneyModel request);

    [Post("/api/wallets/{id}/adjust")]
    Task<ExpenseTracker.Maui.Models.Wallets.BalanceAdjustmentModel> AdjustBalanceAsync(Guid id, [Body] ExpenseTracker.Maui.Models.Wallets.AdjustBalanceModel request);

    [Get("/api/wallets/{id}/adjustments")]
    Task<List<ExpenseTracker.Maui.Models.Wallets.BalanceAdjustmentModel>> GetAdjustmentHistoryAsync(Guid id);
}

// Refit declarative HTTP client contract for category hierarchy operations
public interface ICategoryApi
{
    [Get("/api/categories")]
    Task<List<ExpenseTracker.Maui.Models.Categories.CategoryModel>> GetCategoriesAsync([Query] int? type = null);

    [Get("/api/categories/{id}")]
    Task<ExpenseTracker.Maui.Models.Categories.CategoryModel> GetCategoryByIdAsync(Guid id);

    [Post("/api/categories")]
    Task<ExpenseTracker.Maui.Models.Categories.CategoryModel> CreateCategoryAsync([Body] ExpenseTracker.Maui.Models.Categories.CreateCategoryModel request);

    [Put("/api/categories/{id}")]
    Task<ExpenseTracker.Maui.Models.Categories.CategoryModel> UpdateCategoryAsync(Guid id, [Body] ExpenseTracker.Maui.Models.Categories.UpdateCategoryModel request);

    [Patch("/api/categories/{id}")]
    Task<ExpenseTracker.Maui.Models.Categories.CategoryModel> PatchCategoryAsync(Guid id, [Body] ExpenseTracker.Maui.Models.Categories.PatchCategoryModel request);

    [Delete("/api/categories/{id}")]
    Task DeleteCategoryAsync(Guid id, [Query] Guid? targetCategoryId = null);
}

// Refit declarative HTTP client contract for transactions and income operations
public interface ITransactionApi
{
    [Get("/api/transactions")]
    Task<List<ExpenseTracker.Maui.Models.Transactions.TransactionModel>> GetTransactionsAsync(
        [Query] DateTime? fromDate = null,
        [Query] DateTime? toDate = null,
        [Query] Guid? walletId = null,
        [Query] Guid? categoryId = null,
        [Query] int? transactionType = null,
        [Query] int page = 1,
        [Query] int pageSize = 50);

    [Get("/api/transactions/{id}")]
    Task<ExpenseTracker.Maui.Models.Transactions.TransactionModel> GetTransactionByIdAsync(Guid id);

    [Post("/api/transactions/incomes")]
    Task<ExpenseTracker.Maui.Models.Transactions.TransactionModel> CreateIncomeAsync([Body] ExpenseTracker.Maui.Models.Transactions.CreateIncomeModel request);

    [Put("/api/transactions/incomes/{id}")]
    Task<ExpenseTracker.Maui.Models.Transactions.TransactionModel> UpdateIncomeAsync(Guid id, [Body] ExpenseTracker.Maui.Models.Transactions.UpdateIncomeModel request);

    [Delete("/api/transactions/incomes/{id}")]
    Task DeleteIncomeAsync(Guid id);

    [Get("/api/transactions/incomes/monthly-total")]
    Task<decimal> GetMonthlyIncomeTotalAsync([Query] int month, [Query] int year);

    [Post("/api/transactions/expenses")]
    Task<ExpenseTracker.Maui.Models.Transactions.TransactionModel> CreateExpenseAsync([Body] ExpenseTracker.Maui.Models.Transactions.CreateExpenseModel request);

    [Put("/api/transactions/expenses/{id}")]
    Task<ExpenseTracker.Maui.Models.Transactions.TransactionModel> UpdateExpenseAsync(Guid id, [Body] ExpenseTracker.Maui.Models.Transactions.UpdateExpenseModel request);

    [Delete("/api/transactions/expenses/{id}")]
    Task DeleteExpenseAsync(Guid id);

    [Get("/api/transactions/expenses/{id}/details")]
    Task<ExpenseTracker.Maui.Models.Transactions.ExpenseDetailModel> GetExpenseDetailsAsync(Guid id);

    [Get("/api/transactions/expenses/monthly-total")]
    Task<decimal> GetMonthlyExpenseTotalAsync([Query] int month, [Query] int year);
}

// Refit declarative HTTP client contract for recurring income operations (UC-02.07)
public interface IRecurringIncomeApi
{
    [Get("/api/recurringincomes")]
    Task<List<ExpenseTracker.Maui.Models.RecurringIncomes.RecurringIncomeModel>> GetRecurringIncomesAsync();

    [Get("/api/recurringincomes/{id}")]
    Task<ExpenseTracker.Maui.Models.RecurringIncomes.RecurringIncomeModel> GetRecurringIncomeByIdAsync(Guid id);

    [Post("/api/recurringincomes")]
    Task<ExpenseTracker.Maui.Models.RecurringIncomes.RecurringIncomeModel> CreateRecurringIncomeAsync([Body] ExpenseTracker.Maui.Models.RecurringIncomes.CreateRecurringIncomeModel request);

    [Put("/api/recurringincomes/{id}")]
    Task<ExpenseTracker.Maui.Models.RecurringIncomes.RecurringIncomeModel> UpdateRecurringIncomeAsync(Guid id, [Body] ExpenseTracker.Maui.Models.RecurringIncomes.UpdateRecurringIncomeModel request);

    [Patch("/api/recurringincomes/{id}/toggle")]
    Task<ExpenseTracker.Maui.Models.RecurringIncomes.RecurringIncomeModel> ToggleRecurringIncomeStatusAsync(Guid id);

    [Delete("/api/recurringincomes/{id}")]
    Task DeleteRecurringIncomeAsync(Guid id);

    [Post("/api/recurringincomes/process-due")]
    Task<ExpenseTracker.Maui.Models.RecurringIncomes.ProcessRecurringIncomeResultModel> ProcessDueRecurringIncomesAsync([Body] ExpenseTracker.Maui.Models.RecurringIncomes.ProcessRecurringIncomesModel? request = null);
}

// Refit declarative HTTP client contract for payslip OCR and document extraction (UC-02.08)
public interface IPayslipApi
{
    [Multipart]
    [Post("/api/payslips/parse")]
    Task<ExpenseTracker.Maui.Models.Payslips.PayslipParsedDataModel> ParsePayslipAsync([AliasAs("file")] StreamPart file);

    [Post("/api/payslips/confirm")]
    Task<ExpenseTracker.Maui.Models.Payslips.SalarySlipDetailModel> ConfirmPayslipAsync([Body] ExpenseTracker.Maui.Models.Payslips.ConfirmPayslipModel request);

    [Get("/api/payslips/transaction/{transactionId}")]
    Task<ExpenseTracker.Maui.Models.Payslips.SalarySlipDetailModel> GetPayslipDetailAsync(Guid transactionId);
}

// Refit declarative HTTP client contract for smart auto-categorization rules (UC-04.07)
public interface IAutoRuleApi
{
    [Get("/api/auto-rules")]
    Task<List<ExpenseTracker.Maui.Models.AutoRules.AutoRuleModel>> GetRulesAsync();

    [Get("/api/auto-rules/{id}")]
    Task<ExpenseTracker.Maui.Models.AutoRules.AutoRuleModel> GetRuleByIdAsync(Guid id);

    [Post("/api/auto-rules")]
    Task<ExpenseTracker.Maui.Models.AutoRules.AutoRuleModel> CreateRuleAsync([Body] ExpenseTracker.Maui.Models.AutoRules.CreateAutoRuleModel request);

    [Put("/api/auto-rules/{id}")]
    Task<ExpenseTracker.Maui.Models.AutoRules.AutoRuleModel> UpdateRuleAsync(Guid id, [Body] ExpenseTracker.Maui.Models.AutoRules.UpdateAutoRuleModel request);

    [Delete("/api/auto-rules/{id}")]
    Task DeleteRuleAsync(Guid id);

    [Get("/api/auto-rules/match")]
    Task<ExpenseTracker.Maui.Models.AutoRules.MatchCategoryModel> MatchCategoryAsync([Query] string text);
}

// Refit declarative HTTP client contract for retail receipt OCR and ledger confirmation (UC-03.07/03.08)
public interface IReceiptApi
{
    [Multipart]
    [Post("/api/receipts/parse")]
    Task<ExpenseTracker.Maui.Models.Receipts.ReceiptParsedDataModel> ParseReceiptAsync([AliasAs("file")] StreamPart file);

    [Post("/api/receipts/confirm")]
    Task<ExpenseTracker.Maui.Models.Transactions.TransactionModel> ConfirmReceiptAsync([Body] ExpenseTracker.Maui.Models.Receipts.ConfirmReceiptModel request);
}

// Refit declarative HTTP client contract for smart financial utilities (UC-03.09 - 03.11)
public interface ISmartUtilityApi
{
    [Get("/api/smart/subscriptions")]
    Task<ExpenseTracker.Maui.Models.SmartUtilities.SubscriptionAnalysisResultModel> DetectSubscriptionsAsync();

    [Post("/api/smart/vietqr")]
    Task<ExpenseTracker.Maui.Models.SmartUtilities.VietQrResultModel> GenerateVietQrAsync([Body] ExpenseTracker.Maui.Models.SmartUtilities.GenerateVietQrModel request);

    [Post("/api/smart/voice-parse")]
    Task<ExpenseTracker.Maui.Models.SmartUtilities.ParsedVoiceExpenseModel> ParseVoiceExpenseAsync([Body] ExpenseTracker.Maui.Models.SmartUtilities.ParseVoiceExpenseModel request);
}

// Refit declarative HTTP client contract for multi-methodology budgeting and Adaptive DSA (UC-05.01 - 05.06)
public interface IBudgetApi
{
    [Get("/api/budgets/active")]
    Task<ExpenseTracker.Maui.Models.Budgets.BudgetModel> GetActiveBudgetAsync();

    [Get("/api/budgets")]
    Task<List<ExpenseTracker.Maui.Models.Budgets.BudgetSummaryModel>> GetBudgetsAsync();

    [Get("/api/budgets/{id}")]
    Task<ExpenseTracker.Maui.Models.Budgets.BudgetModel> GetBudgetByIdAsync(Guid id);

    [Post("/api/budgets")]
    Task<ExpenseTracker.Maui.Models.Budgets.BudgetModel> CreateBudgetAsync([Body] ExpenseTracker.Maui.Models.Budgets.CreateBudgetModel request);

    [Put("/api/budgets/{id}")]
    Task<ExpenseTracker.Maui.Models.Budgets.BudgetModel> UpdateBudgetAsync(Guid id, [Body] ExpenseTracker.Maui.Models.Budgets.UpdateBudgetModel request);

    [Post("/api/budgets/{id}/rebalance")]
    Task<ExpenseTracker.Maui.Models.Budgets.BudgetModel> RebalanceBudgetAsync(Guid id, [Body] ExpenseTracker.Maui.Models.Budgets.RebalanceBudgetModel request);

    [Delete("/api/budgets/{id}")]
    Task CloseBudgetAsync(Guid id);

    [Get("/api/budgets/active/dsa")]
    Task<ExpenseTracker.Maui.Models.Budgets.DsaStatusModel> GetCurrentDsaStatusAsync();

    [Get("/api/budgets/active/velocity")]
    Task<ExpenseTracker.Maui.Models.Budgets.VelocityStatusModel> GetSpendingVelocityAsync();

    [Get("/api/budgets/active/progress")]
    Task<ExpenseTracker.Maui.Models.Budgets.BudgetProgressModel> GetBudgetProgressOverviewAsync();
}

// Refit declarative HTTP client contract for smart savings goals and jars (UC-06.01 - 06.05)
public interface ISavingsGoalApi
{
    [Get("/api/savingsgoals")]
    Task<List<ExpenseTracker.Maui.Models.SavingsGoals.SavingsGoalSummaryModel>> GetSavingsGoalsAsync([Query] ExpenseTracker.Maui.Models.SavingsGoals.SavingsGoalStatus? status = null);

    [Get("/api/savingsgoals/{id}")]
    Task<ExpenseTracker.Maui.Models.SavingsGoals.SavingsGoalModel> GetSavingsGoalByIdAsync(Guid id);

    [Post("/api/savingsgoals")]
    Task<ExpenseTracker.Maui.Models.SavingsGoals.SavingsGoalModel> CreateSavingsGoalAsync([Body] ExpenseTracker.Maui.Models.SavingsGoals.CreateSavingsGoalModel request);

    [Put("/api/savingsgoals/{id}")]
    Task<ExpenseTracker.Maui.Models.SavingsGoals.SavingsGoalModel> UpdateSavingsGoalAsync(Guid id, [Body] ExpenseTracker.Maui.Models.SavingsGoals.UpdateSavingsGoalModel request);

    [Patch("/api/savingsgoals/{id}")]
    Task<ExpenseTracker.Maui.Models.SavingsGoals.SavingsGoalModel> PatchSavingsGoalAsync(Guid id, [Body] ExpenseTracker.Maui.Models.SavingsGoals.PatchSavingsGoalModel request);

    [Patch("/api/savingsgoals/{id}/status")]
    Task<ExpenseTracker.Maui.Models.SavingsGoals.SavingsGoalModel> PatchGoalStatusAsync(Guid id, [Body] ExpenseTracker.Maui.Models.SavingsGoals.PatchSavingsGoalStatusModel request);

    [Delete("/api/savingsgoals/{id}")]
    Task DeleteSavingsGoalAsync(Guid id);

    [Post("/api/savingsgoals/{id}/deposit")]
    Task<ExpenseTracker.Maui.Models.SavingsGoals.SavingsGoalContributionModel> DepositAsync(Guid id, [Body] ExpenseTracker.Maui.Models.SavingsGoals.DepositSavingsGoalModel request);

    [Post("/api/savingsgoals/{id}/withdraw")]
    Task<ExpenseTracker.Maui.Models.SavingsGoals.SavingsGoalContributionModel> WithdrawAsync(Guid id, [Body] ExpenseTracker.Maui.Models.SavingsGoals.WithdrawSavingsGoalModel request);

    [Get("/api/savingsgoals/{id}/contributions")]
    Task<List<ExpenseTracker.Maui.Models.SavingsGoals.SavingsGoalContributionModel>> GetGoalContributionsAsync(Guid id);

    [Put("/api/savingsgoals/{id}/auto-deposit")]
    Task<ExpenseTracker.Maui.Models.SavingsGoals.SavingsGoalModel> ConfigureAutoDepositAsync(Guid id, [Body] ExpenseTracker.Maui.Models.SavingsGoals.ConfigureAutoDepositModel request);

    [Get("/api/savingsgoals/{id}/feasibility")]
    Task<ExpenseTracker.Maui.Models.SavingsGoals.SavingsFeasibilityModel> GetGoalFeasibilityAsync(Guid id);

    [Get("/api/savingsgoals/jars-summary")]
    Task<ExpenseTracker.Maui.Models.SavingsGoals.JarsSummaryModel> GetJarsSummaryAsync();
}

// Refit declarative HTTP client contract for notifications and push token management
public interface INotificationsApi
{
    [Post("/api/notifications/push-token")]
    Task RegisterPushTokenAsync([Body] ExpenseTracker.Maui.Models.Notifications.RegisterPushTokenRequestModel request);

    [Get("/api/notifications")]
    Task<List<ExpenseTracker.Maui.Models.Notifications.InAppNotificationModel>> GetNotificationsAsync([Query] bool? unreadOnly = null, [Query] int take = 50);

    [Patch("/api/notifications/{id}/read")]
    Task MarkAsReadAsync(Guid id);

    [Patch("/api/notifications/read-all")]
    Task MarkAllAsReadAsync();

    [Post("/api/notifications/test-push")]
    Task<ExpenseTracker.Maui.Models.Notifications.InAppNotificationModel> SendTestNotificationAsync([Body] ExpenseTracker.Maui.Models.Notifications.SendTestNotificationRequestModel request);
}

// Refit declarative HTTP client contract for analytics reports and quantitative engine (UC-07.01 -> UC-07.05)
public interface IAnalyticsReportApi
{
    [Get("/api/analytics/reports/income-expense")]
    Task<ExpenseTracker.Maui.Models.Reports.IncomeExpenseComparisonModel> GetIncomeExpenseAsync(
        [Query] DateTime from,
        [Query] DateTime to,
        [Query] string groupBy = "month");

    [Get("/api/analytics/reports/net-worth")]
    Task<ExpenseTracker.Maui.Models.Reports.NetWorthReportModel> GetNetWorthAsync(
        [Query] DateTime? from = null,
        [Query] DateTime? to = null);

    [Post("/api/analytics/reports/net-worth/snapshot")]
    Task<ExpenseTracker.Maui.Models.Reports.NetWorthSnapshotModel> RecordNetWorthSnapshotAsync(
        [Body] ExpenseTracker.Maui.Models.Reports.RecordSnapshotRequestModel? request);

    [Get("/api/analytics/reports/cashflow")]
    Task<ExpenseTracker.Maui.Models.Reports.NetCashflowReportModel> GetCashflowAsync(
        [Query] DateTime from,
        [Query] DateTime to);

    [Get("/api/analytics/reports/expense-structure")]
    Task<ExpenseTracker.Maui.Models.Reports.ExpenseStructureReportModel> GetExpenseStructureAsync(
        [Query] DateTime from,
        [Query] DateTime to);

    [Get("/api/analytics/reports/outliers")]
    Task<ExpenseTracker.Maui.Models.Reports.OutlierDetectionReportModel> GetOutliersAsync(
        [Query] int days = 30,
        [Query] double threshold = 2.0);
}

public interface IForecastingApi
{
    [Get("/api/forecasting/expenditure")]
    Task<ExpenseTracker.Maui.Models.Forecasting.ExpenseForecastModel> GetExpenditureForecastAsync();

    [Get("/api/forecasting/liquidity-depletion")]
    Task<ExpenseTracker.Maui.Models.Forecasting.LiquidityDepletionModel> GetLiquidityDepletionAsync();

    [Get("/api/forecasting/category-risks")]
    Task<List<ExpenseTracker.Maui.Models.Forecasting.CategoryDeficitRiskModel>> GetCategoryRisksAsync();

    [Post("/api/forecasting/what-if")]
    Task<ExpenseTracker.Maui.Models.Forecasting.WhatIfSimulationResultModel> SimulateWhatIfAsync(
        [Body] ExpenseTracker.Maui.Models.Forecasting.WhatIfSimulationRequestModel request);

    [Post("/api/forecasting/what-if/convert-to-goal")]
    Task<ExpenseTracker.Maui.Models.Forecasting.ConvertToSavingsGoalResultModel> ConvertWhatIfToGoalAsync(
        [Body] ExpenseTracker.Maui.Models.Forecasting.ConvertToSavingsGoalRequestModel request);
}

// Refit declarative HTTP client contract for Behavioral Nudges &amp; Thaler Choice Architecture (UC-09.01 -> UC-09.04)
public interface INudgeApi
{
    [Get("/api/nudges")]
    Task<List<ExpenseTracker.Maui.Models.Behavioral.FinancialNudgeModel>> GetActiveNudgesAsync();

    [Get("/api/nudges/{id}/xai")]
    Task<ExpenseTracker.Maui.Models.Behavioral.XaiRationaleModel> GetXaiRationaleAsync(Guid id);

    [Get("/api/nudges/rebalance/{deficitCategoryId}")]
    Task<ExpenseTracker.Maui.Models.Behavioral.DynamicRebalanceProposalModel> GenerateRebalanceProposalAsync(Guid deficitCategoryId);

    [Post("/api/nudges/{id}/approve")]
    Task<bool> ApproveNudgeAsync(Guid id);

    [Post("/api/nudges/{id}/dismiss")]
    Task<bool> DismissNudgeAsync(Guid id);
}

// Refit declarative HTTP client contract for 24h Positive Friction Cooling-Off Mechanism (UC-09.05)
public interface ICoolingOffApi
{
    [Post("/api/cooling-off/evaluate")]
    Task<ExpenseTracker.Maui.Models.Behavioral.CoolingOffEvaluationResultModel> EvaluateExpenseAsync(
        [Body] ExpenseTracker.Maui.Models.Behavioral.CoolingOffEvaluationRequestModel request);

    [Post("/api/cooling-off")]
    Task<ExpenseTracker.Maui.Models.Behavioral.CoolingOffTransactionModel> CreateCoolingOffAsync(
        [Body] ExpenseTracker.Maui.Models.Behavioral.CreateCoolingOffRequestModel request);

    [Get("/api/cooling-off/pending")]
    Task<List<ExpenseTracker.Maui.Models.Behavioral.CoolingOffTransactionModel>> GetPendingCoolingOffAsync();

    [Post("/api/cooling-off/{id}/confirm")]
    Task<bool> ConfirmPurchaseAsync(Guid id);

    [Post("/api/cooling-off/{id}/cancel")]
    Task<bool> CancelPurchaseAsync(Guid id);

    [Post("/api/cooling-off/{id}/bypass")]
    Task<bool> BypassCoolingOffAsync(
        Guid id,
        [Body] ExpenseTracker.Maui.Models.Behavioral.BypassCoolingOffRequestModel request);
}

// Refit declarative HTTP client contract for Voice-Driven Intent Agent (UC-03.11 Enhancement)
public interface IVoiceAgentApi
{
    [Post("/api/voice-agent/parse-intent")]
    Task<ExpenseTracker.Maui.Models.AiAgent.VoiceIntentResultModel> ParseIntentAsync(
        [Body] ExpenseTracker.Maui.Models.AiAgent.VoiceIntentCommandRequest request);
}

// Refit declarative HTTP client contract for Hybrid Multi-source Import (UC-07.06)
public interface IDataImportApi
{
    [Multipart]
    [Post("/api/data-import/preview-spreadsheet")]
    Task<ExpenseTracker.Maui.Models.ImportExport.ImportPreviewResultModel> PreviewSpreadsheetAsync(
        [AliasAs("file")] StreamPart file);

    [Post("/api/data-import/preview-text")]
    Task<ExpenseTracker.Maui.Models.ImportExport.ImportPreviewResultModel> PreviewUnstructuredTextAsync(
        [Body] object request);

    [Post("/api/data-import/commit-bulk")]
    Task<ExpenseTracker.Maui.Models.ImportExport.BulkCommitImportResultModel> CommitBulkImportAsync(
        [Body] ExpenseTracker.Maui.Models.ImportExport.BulkCommitImportRequestModel request);
}

// Refit declarative HTTP client contract for Server-Driven Action Protocol (Chặng 10.5 / PLAN-47)
public interface IAutonomousActionsApi
{
    [Get("/api/actions/inbox")]
    Task<ExpenseTracker.Maui.Models.Autonomous.AutonomousActionInboxResponseModel> GetInboxAsync();

    [Post("/api/actions/from-notification")]
    Task<ExpenseTracker.Maui.Models.Autonomous.AutonomousActionModel> ProcessNotificationAsync(
        [Body] ExpenseTracker.Maui.Models.Autonomous.RawBankNotificationRequestModel request);

    [Post("/api/actions/quick-commit")]
    Task<ExpenseTracker.Maui.Models.Autonomous.AutonomousActionModel> QuickCommitAsync(
        [Body] ExpenseTracker.Maui.Models.Autonomous.QuickCommitActionRequestModel request);

    [Post("/api/actions/{actionId}/dismiss")]
    Task<bool> DismissActionAsync(string actionId);
}

// Refit declarative HTTP client contract for debt management (UC-06.05 & UC-06.07 / PLAN-62)
public interface IDebtsApi
{
    [Get("/api/debts")]
    Task<List<ExpenseTracker.Maui.Models.Debts.DebtModel>> GetDebtsAsync();

    [Get("/api/debts/summary")]
    Task<ExpenseTracker.Maui.Models.Debts.DebtSummaryModel> GetDebtSummaryAsync();

    [Get("/api/debts/{id}")]
    Task<ExpenseTracker.Maui.Models.Debts.DebtDetailModel> GetDebtByIdAsync(Guid id);

    [Post("/api/debts")]
    Task<ExpenseTracker.Maui.Models.Debts.DebtModel> CreateDebtAsync([Body] ExpenseTracker.Maui.Models.Debts.CreateDebtRequest request);

    [Put("/api/debts/{id}")]
    Task<ExpenseTracker.Maui.Models.Debts.DebtModel> UpdateDebtAsync(Guid id, [Body] ExpenseTracker.Maui.Models.Debts.UpdateDebtRequest request);

    [Post("/api/debts/{id}/payments")]
    Task<ExpenseTracker.Maui.Models.Debts.DebtTransactionModel> RecordPaymentAsync(Guid id, [Body] ExpenseTracker.Maui.Models.Debts.RecordDebtPaymentRequest request);

    [Post("/api/debts/{id}/archive")]
    Task<object> ArchiveDebtAsync(Guid id);

    [Get("/api/debts/optimization/guide")]
    Task<ExpenseTracker.Maui.Models.Debts.DebtEducationalGuideModel> GetEducationalGuideAsync();

    [Get("/api/debts/optimization/safe-budget")]
    Task<ExpenseTracker.Maui.Models.Debts.SafeExtraBudgetRecommendationModel> GetSafeBudgetRecommendationAsync();

    [Post("/api/debts/optimization/compare")]
    Task<ExpenseTracker.Maui.Models.Debts.DebtPayoffComparisonModel> CompareStrategiesAsync([Body] ExpenseTracker.Maui.Models.Debts.DebtPayoffSimulationRequest request);

    [Post("/api/debts/optimization/simulate")]
    Task<ExpenseTracker.Maui.Models.Debts.DebtStrategySimulationResultModel> SimulateStrategyAsync([Query] string strategy, [Body] ExpenseTracker.Maui.Models.Debts.DebtPayoffSimulationRequest request);
}

// Refit declarative HTTP client contract for Emergency Fund management (UC-06.04)
public interface IEmergencyFundApi
{
    [Get("/api/emergency-funds/recommendation")]
    Task<ExpenseTracker.Maui.Models.SavingsGoals.EmergencyFundRecommendationModel> GetRecommendationAsync();

    [Get("/api/emergency-funds/status")]
    Task<ExpenseTracker.Maui.Models.SavingsGoals.EmergencyFundStatusModel> GetStatusAsync();

    [Post("/api/emergency-funds/setup")]
    Task<ExpenseTracker.Maui.Models.SavingsGoals.EmergencyFundStatusModel> SetupFundAsync([Body] ExpenseTracker.Maui.Models.SavingsGoals.SetupEmergencyFundRequest request);

    [Post("/api/emergency-funds/deposit")]
    Task<ExpenseTracker.Maui.Models.SavingsGoals.EmergencyFundStatusModel> DepositAsync([Body] ExpenseTracker.Maui.Models.SavingsGoals.DepositEmergencyFundRequest request);

    [Post("/api/emergency-funds/withdraw")]
    Task<ExpenseTracker.Maui.Models.SavingsGoals.EmergencyFundStatusModel> WithdrawAsync([Body] ExpenseTracker.Maui.Models.SavingsGoals.WithdrawEmergencyFundRequest request);
}

// Refit declarative HTTP client contract for Net Worth calculation and reporting (UC-07.02)
public interface INetWorthApi
{
    [Get("/api/net-worth/current")]
    Task<ExpenseTracker.Maui.Models.Reports.NetWorthBreakdownModel> GetCurrentNetWorthAsync();

    [Get("/api/net-worth/history")]
    Task<List<ExpenseTracker.Maui.Models.Reports.NetWorthHistoryItemModel>> GetNetWorthHistoryAsync([Query] int limit = 12);

    [Post("/api/net-worth/snapshot")]
    Task<ExpenseTracker.Maui.Models.Reports.NetWorthHistoryItemModel> RecordSnapshotAsync([Body] ExpenseTracker.Maui.Models.Reports.RecordNetWorthSnapshotRequest request);
}

// Refit declarative HTTP client contract for Subscription management and audit (UC-03.09)
public interface ISubscriptionApi
{
    [Get("/api/subscriptions")]
    Task<List<ExpenseTracker.Maui.Models.SmartUtilities.SubscriptionModel>> GetSubscriptionsAsync([Query] int? status = null);

    [Get("/api/subscriptions/{id}")]
    Task<ExpenseTracker.Maui.Models.SmartUtilities.SubscriptionModel> GetSubscriptionByIdAsync(Guid id);

    [Post("/api/subscriptions")]
    Task<ExpenseTracker.Maui.Models.SmartUtilities.SubscriptionModel> CreateSubscriptionAsync([Body] ExpenseTracker.Maui.Models.SmartUtilities.CreateSubscriptionRequest request);

    [Put("/api/subscriptions/{id}")]
    Task<ExpenseTracker.Maui.Models.SmartUtilities.SubscriptionModel> UpdateSubscriptionAsync(Guid id, [Body] ExpenseTracker.Maui.Models.SmartUtilities.UpdateSubscriptionRequest request);

    [Post("/api/subscriptions/{id}/archive")]
    Task<bool> ArchiveSubscriptionAsync(Guid id);

    [Get("/api/subscriptions/audit")]
    Task<ExpenseTracker.Maui.Models.SmartUtilities.SubscriptionAuditReportModel> AuditSubscriptionsAsync();

    [Post("/api/subscriptions/convert")]
    Task<ExpenseTracker.Maui.Models.SmartUtilities.SubscriptionModel> ConvertCandidateAsync([Body] ExpenseTracker.Maui.Models.SmartUtilities.CreateSubscriptionRequest request);

    [Get("/api/subscriptions/upcoming")]
    Task<List<ExpenseTracker.Maui.Models.SmartUtilities.UpcomingRenewalModel>> GetUpcomingRenewalsAsync([Query] int daysAhead = 7);
}





