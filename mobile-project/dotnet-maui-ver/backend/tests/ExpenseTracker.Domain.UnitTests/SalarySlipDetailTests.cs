using ExpenseTracker.Domain.Entities;
using Xunit;

namespace ExpenseTracker.Domain.UnitTests;

// Unit tests validating SalarySlipDetail invariants and mathematical audit formula (UC-02.08)
public class SalarySlipDetailTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _transactionId = Guid.NewGuid();

    [Fact]
    public void Constructor_WithBalancedBreakdown_ShouldPassMathAudit()
    {
        // Arrange & Act (UC-02.08 Acceptance Criteria 1)
        // Gross: 25,000,000
        // Deductions: BHXH 2,000,000 + BHYT 375,000 + BHTN 250,000 + Thuế 1,150,000 = 3,775,000
        // Net: 25,000,000 - 3,775,000 = 21,225,000
        var salarySlip = new SalarySlipDetail(
            userId: _userId,
            transactionId: _transactionId,
            employerName: "Công ty ABC",
            payPeriod: "10/2026",
            grossSalary: 25_000_000m,
            netSalary: 21_225_000m,
            socialInsurance: 2_000_000m,
            healthInsurance: 375_000m,
            unemploymentInsurance: 250_000m,
            personalIncomeTax: 1_150_000m
        );

        // Assert
        Assert.True(salarySlip.IsMathAudited);
        Assert.Equal(0m, salarySlip.DiscrepancyAmount);
        Assert.Equal("Công ty ABC", salarySlip.EmployerName);
        Assert.Equal("10/2026", salarySlip.PayPeriod);
    }

    [Fact]
    public void Constructor_WithAllowancesAndBonus_ShouldPassMathAudit()
    {
        // Arrange & Act
        // Gross: 30,000,000
        // Deductions: 3,000,000
        // Additions: Allowances 2,000,000 + Bonus 1,000,000 = 3,000,000
        // Expected Net: 30,000,000 - 3,000,000 + 3,000,000 = 30,000,000
        var salarySlip = new SalarySlipDetail(
            userId: _userId,
            transactionId: _transactionId,
            employerName: "Công ty XYZ",
            payPeriod: "11/2026",
            grossSalary: 30_000_000m,
            netSalary: 30_000_000m,
            socialInsurance: 2_000_000m,
            healthInsurance: 500_000m,
            unemploymentInsurance: 300_000m,
            personalIncomeTax: 200_000m,
            allowances: 2_000_000m,
            bonus: 1_000_000m
        );

        // Assert
        Assert.True(salarySlip.IsMathAudited);
        Assert.Equal(0m, salarySlip.DiscrepancyAmount);
    }

    [Fact]
    public void Constructor_WhenDiscrepancyExceeds1000Vnd_ShouldFailMathAudit_AndFlagDiscrepancy()
    {
        // Arrange & Act (UC-02.08 E2: Sai lệch số học giữa Gross và Net)
        // Gross: 25,000,000, Deductions: 3,775,000 -> Computed Net: 21,225,000
        // But Net provided is 20,000,000 (discrepancy = 1,225,000 VND > 1,000 VND)
        var salarySlip = new SalarySlipDetail(
            userId: _userId,
            transactionId: _transactionId,
            employerName: "Công ty ABC",
            payPeriod: "10/2026",
            grossSalary: 25_000_000m,
            netSalary: 20_000_000m,
            socialInsurance: 2_000_000m,
            healthInsurance: 375_000m,
            unemploymentInsurance: 250_000m,
            personalIncomeTax: 1_150_000m
        );

        // Assert
        Assert.False(salarySlip.IsMathAudited);
        Assert.Equal(1_225_000m, salarySlip.DiscrepancyAmount);
    }

    [Fact]
    public void Constructor_WhenNetSalaryIsNonPositive_ShouldThrowArgumentException()
    {
        // Act & Assert
        Assert.Throws<ArgumentException>(() => new SalarySlipDetail(
            userId: _userId,
            transactionId: _transactionId,
            employerName: "Công ty ABC",
            payPeriod: "10/2026",
            grossSalary: 25_000_000m,
            netSalary: 0m,
            socialInsurance: 0m,
            healthInsurance: 0m,
            unemploymentInsurance: 0m,
            personalIncomeTax: 0m
        ));
    }
}
