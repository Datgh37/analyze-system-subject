using ExpenseTracker.Maui.Models.Debts;
using ExpenseTracker.Maui.Repositories;
using FluentAssertions;
using Xunit;

namespace ExpenseTracker.Maui.UnitTests.Repositories;

public class DebtRepositoryTests
{
    private readonly MockDebtRepository _repository;

    public DebtRepositoryTests()
    {
        _repository = new MockDebtRepository();
    }

    [Fact]
    public async Task GetDebtsAsync_ReturnsSeededDebts()
    {
        // Act
        var debts = await _repository.GetDebtsAsync();

        // Assert
        debts.Should().NotBeNull();
        debts.Should().HaveCountGreaterThanOrEqualTo(3);
        debts.Should().Contain(d => d.Name.Contains("Macbook"));
    }

    [Fact]
    public async Task GetDebtByIdAsync_ReturnsDebtWithTransactions()
    {
        // Arrange
        var debts = await _repository.GetDebtsAsync();
        var targetDebt = debts.First(d => d.Name.Contains("Macbook"));

        // Act
        var detail = await _repository.GetDebtByIdAsync(targetDebt.Id);

        // Assert
        detail.Should().NotBeNull();
        detail!.Debt.Name.Should().Be(targetDebt.Name);
        detail.Transactions.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetDebtSummaryAsync_CalculatesCorrectTotals()
    {
        // Act
        var summary = await _repository.GetDebtSummaryAsync();

        // Assert
        summary.Should().NotBeNull();
        summary.TotalCurrentDebt.Should().BeGreaterThan(0);
        summary.ActiveDebtsCount.Should().Be(2);
        summary.PaidOffDebtsCount.Should().Be(1);
    }

    [Fact]
    public async Task CreateDebtAsync_AddsNewDebt()
    {
        // Arrange
        var request = new CreateDebtRequest
        {
            Name = "Vay Mua Xe Đạp Điện",
            DebtType = (int)DebtType.InstallmentLoan,
            CreditorName = "Home Credit",
            PrincipalAmount = 12_000_000m,
            InterestRateAnnual = 15m,
            MinimumPayment = 1_200_000m,
            DueDateMonthly = 10,
            StartDate = DateTime.Today
        };

        // Act
        var created = await _repository.CreateDebtAsync(request);

        // Assert
        created.Should().NotBeNull();
        created.Name.Should().Be("Vay Mua Xe Đạp Điện");
        created.PrincipalAmount.Should().Be(12_000_000m);
        created.CurrentBalance.Should().Be(12_000_000m);
        created.Status.Should().Be(DebtStatus.Active);

        var allDebts = await _repository.GetDebtsAsync();
        allDebts.Should().Contain(d => d.Id == created.Id);
    }

    [Fact]
    public async Task UpdateDebtAsync_UpdatesDebtDetails()
    {
        // Arrange
        var debts = await _repository.GetDebtsAsync();
        var target = debts.First();

        var updateRequest = new UpdateDebtRequest
        {
            Name = "Tên Mới Sau Cập Nhật",
            DebtType = (int)DebtType.Personal,
            CreditorName = "Người Cho Vay Mới",
            InterestRateAnnual = 5m,
            MinimumPayment = 500_000m,
            DueDateMonthly = 20
        };

        // Act
        var updated = await _repository.UpdateDebtAsync(target.Id, updateRequest);

        // Assert
        updated.Name.Should().Be("Tên Mới Sau Cập Nhật");
        updated.CreditorName.Should().Be("Người Cho Vay Mới");
        updated.DueDateMonthly.Should().Be(20);
    }

    [Fact]
    public async Task RecordPaymentAsync_ReducesCurrentBalance()
    {
        // Arrange
        var debts = await _repository.GetDebtsAsync();
        var target = debts.First(d => d.Status == DebtStatus.Active);
        var initialBalance = target.CurrentBalance;

        var paymentRequest = new RecordDebtPaymentRequest
        {
            WalletId = Guid.NewGuid(),
            Amount = 2_500_000m,
            PrincipalPaid = 2_000_000m,
            InterestPaid = 500_000m,
            PaymentDate = DateTime.Now,
            Note = "Trả kỳ này"
        };

        // Act
        var transaction = await _repository.RecordPaymentAsync(target.Id, paymentRequest);

        // Assert
        transaction.Should().NotBeNull();
        transaction.PrincipalPaid.Should().Be(2_000_000m);
        target.CurrentBalance.Should().Be(initialBalance - 2_000_000m);
    }

    [Fact]
    public async Task RecordPaymentAsync_WhenBalanceReachesZero_SetsStatusToPaidOff()
    {
        // Arrange
        var debts = await _repository.GetDebtsAsync();
        var target = debts.First(d => d.Status == DebtStatus.Active);

        var paymentRequest = new RecordDebtPaymentRequest
        {
            WalletId = Guid.NewGuid(),
            Amount = target.CurrentBalance,
            PrincipalPaid = target.CurrentBalance,
            InterestPaid = 0m,
            PaymentDate = DateTime.Now,
            Note = "Tất toán sạch nợ"
        };

        // Act
        var transaction = await _repository.RecordPaymentAsync(target.Id, paymentRequest);

        // Assert
        target.CurrentBalance.Should().Be(0m);
        target.Status.Should().Be(DebtStatus.PaidOff);
        target.StatusName.Should().Be("Đã tất toán");
    }
}
