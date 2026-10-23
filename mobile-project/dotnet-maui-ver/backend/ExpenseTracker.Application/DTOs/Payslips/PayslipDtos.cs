namespace ExpenseTracker.Application.DTOs.Payslips;

// DTO holding raw parsed extraction results from AI Vision OCR with mathematical audit metadata (UC-02.08)
public record PayslipParsedDataDto(
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

// Request to confirm and persist a parsed payslip into transaction ledger and salary slip table
public record ConfirmPayslipRequest(
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
    Guid? UpdateRecurringIncomeId = null // UC-02.08 A1: Optional link to update recurring income schedule
);

// DTO representing stored salary slip detail record
public record SalarySlipDetailDto(
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
