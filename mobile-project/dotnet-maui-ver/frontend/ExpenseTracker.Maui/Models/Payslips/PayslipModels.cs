namespace ExpenseTracker.Maui.Models.Payslips;

// Presentation model representing parsed payslip extraction results (UC-02.08)
public record PayslipParsedDataModel(
    string EmployerName,
    string PayPeriod,
    decimal GrossSalary,
    decimal NetSalary,
    decimal SocialInsurance,
    decimal HealthInsurance,
    decimal UnemploymentInsurance,
    decimal PersonalIncomeTax,
    decimal Allowances,
    decimal Bonus,
    decimal OtherDeductions,
    bool IsMathAudited,
    decimal DiscrepancyAmount,
    bool HasDiscrepancy,
    double ConfidenceScore,
    string? RawJson
);

// Model for confirming and persisting payslip income
public record ConfirmPayslipModel(
    Guid WalletId,
    Guid CategoryId,
    string EmployerName,
    string PayPeriod,
    decimal GrossSalary,
    decimal NetSalary,
    decimal SocialInsurance,
    decimal HealthInsurance,
    decimal UnemploymentInsurance,
    decimal PersonalIncomeTax,
    decimal Allowances = 0m,
    decimal Bonus = 0m,
    decimal OtherDeductions = 0m,
    DateTime? TransactionDate = null,
    string? Note = null,
    Guid? UpdateRecurringIncomeId = null
);

// Model representing stored salary slip detail record
public record SalarySlipDetailModel(
    Guid Id,
    Guid UserId,
    Guid TransactionId,
    string EmployerName,
    string PayPeriod,
    decimal GrossSalary,
    decimal NetSalary,
    decimal SocialInsurance,
    decimal HealthInsurance,
    decimal UnemploymentInsurance,
    decimal PersonalIncomeTax,
    decimal Allowances,
    decimal Bonus,
    decimal OtherDeductions,
    bool IsMathAudited,
    decimal DiscrepancyAmount,
    DateTime CreatedAt
);
