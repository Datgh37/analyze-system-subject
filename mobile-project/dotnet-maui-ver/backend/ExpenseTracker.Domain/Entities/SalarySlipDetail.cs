using ExpenseTracker.Domain.Common;

namespace ExpenseTracker.Domain.Entities;

// Entity capturing granular breakdown and mathematical audit of payslip document (UC-02.08)
public class SalarySlipDetail : BaseEntity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; } = null!;

    public Guid TransactionId { get; private set; }
    public Transaction Transaction { get; private set; } = null!;

    public string EmployerName { get; private set; } = string.Empty;
    public string PayPeriod { get; private set; } = string.Empty; // e.g. "10/2026"

    public decimal GrossSalary { get; private set; }
    public decimal NetSalary { get; private set; }

    // Statutory deductions
    public decimal SocialInsurance { get; private set; }
    public decimal HealthInsurance { get; private set; }
    public decimal UnemploymentInsurance { get; private set; }
    public decimal PersonalIncomeTax { get; private set; }
    public decimal OtherDeductions { get; private set; }

    // Additions & allowances
    public decimal Allowances { get; private set; }
    public decimal Bonus { get; private set; }

    // Mathematical audit fields (UC-02.08 Step 6 & E2)
    public bool IsMathAudited { get; private set; }
    public decimal DiscrepancyAmount { get; private set; }
    public string? RawOcrJson { get; private set; }

    // Parameterless constructor for EF Core materialization
    private SalarySlipDetail() { }

    public SalarySlipDetail(
        Guid userId,
        Guid transactionId,
        string employerName,
        string payPeriod,
        decimal grossSalary,
        decimal netSalary,
        decimal socialInsurance,
        decimal healthInsurance,
        decimal unemploymentInsurance,
        decimal personalIncomeTax,
        decimal allowances = 0m,
        decimal bonus = 0m,
        decimal otherDeductions = 0m,
        string? rawOcrJson = null)
    {
        if (grossSalary < 0)
            throw new ArgumentException("Lương tổng (Gross) không thể âm.", nameof(grossSalary));

        if (netSalary <= 0)
            throw new ArgumentException("Lương thực nhận (Net) phải lớn hơn 0.", nameof(netSalary));

        UserId = userId;
        TransactionId = transactionId;
        EmployerName = string.IsNullOrWhiteSpace(employerName) ? "Doanh nghiệp / Đơn vị chi trả" : employerName.Trim();
        PayPeriod = string.IsNullOrWhiteSpace(payPeriod) ? $"{DateTime.UtcNow:MM/yyyy}" : payPeriod.Trim();
        GrossSalary = grossSalary;
        NetSalary = netSalary;
        SocialInsurance = socialInsurance;
        HealthInsurance = healthInsurance;
        UnemploymentInsurance = unemploymentInsurance;
        PersonalIncomeTax = personalIncomeTax;
        Allowances = allowances;
        Bonus = bonus;
        OtherDeductions = otherDeductions;
        RawOcrJson = rawOcrJson;
        CreatedAt = DateTime.UtcNow;

        AuditMathDiscrepancy();
    }

    // Mathematical audit formula: Net = Gross - TotalDeductions + TotalAdditions (UC-02.08 Step 6)
    public void AuditMathDiscrepancy()
    {
        decimal totalDeductions = SocialInsurance + HealthInsurance + UnemploymentInsurance + PersonalIncomeTax + OtherDeductions;
        decimal totalAdditions = Allowances + Bonus;
        decimal computedNet = GrossSalary - totalDeductions + totalAdditions;

        DiscrepancyAmount = Math.Abs(NetSalary - computedNet);
        // Allowable tolerance threshold is 1,000 VND (UC-02.08 Step 6)
        IsMathAudited = DiscrepancyAmount <= 1000m;
    }

    public void UpdateBreakdown(
        string employerName,
        string payPeriod,
        decimal grossSalary,
        decimal netSalary,
        decimal socialInsurance,
        decimal healthInsurance,
        decimal unemploymentInsurance,
        decimal personalIncomeTax,
        decimal allowances,
        decimal bonus,
        decimal otherDeductions)
    {
        if (grossSalary < 0)
            throw new ArgumentException("Lương tổng (Gross) không thể âm.", nameof(grossSalary));

        if (netSalary <= 0)
            throw new ArgumentException("Lương thực nhận (Net) phải lớn hơn 0.", nameof(netSalary));

        EmployerName = string.IsNullOrWhiteSpace(employerName) ? "Doanh nghiệp / Đơn vị chi trả" : employerName.Trim();
        PayPeriod = string.IsNullOrWhiteSpace(payPeriod) ? $"{DateTime.UtcNow:MM/yyyy}" : payPeriod.Trim();
        GrossSalary = grossSalary;
        NetSalary = netSalary;
        SocialInsurance = socialInsurance;
        HealthInsurance = healthInsurance;
        UnemploymentInsurance = unemploymentInsurance;
        PersonalIncomeTax = personalIncomeTax;
        Allowances = allowances;
        Bonus = bonus;
        OtherDeductions = otherDeductions;
        UpdatedAt = DateTime.UtcNow;

        AuditMathDiscrepancy();
    }
}
