using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace ExpenseTracker.Domain.UnitTests.Entities;

public class DebtTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _walletId = Guid.NewGuid();

    [Fact]
    public void Constructor_ValidParameters_SetsInitialStateCorrectly()
    {
        // Act
        var debt = new Debt(
            _userId,
            "Vay Mua Laptop Macbook Pro",
            DebtType.InstallmentLoan,
            "FE Credit",
            30_000_000m,
            12.5m,
            2_500_000m,
            15,
            DateTime.UtcNow,
            DateTime.UtcNow.AddMonths(12),
            30_000_000m,
            "Mua phục vụ học tập và lập trình"
        );

        // Assert
        debt.UserId.Should().Be(_userId);
        debt.Name.Should().Be("Vay Mua Laptop Macbook Pro");
        debt.DebtType.Should().Be(DebtType.InstallmentLoan);
        debt.CreditorName.Should().Be("FE Credit");
        debt.PrincipalAmount.Should().Be(30_000_000m);
        debt.CurrentBalance.Should().Be(30_000_000m);
        debt.InterestRateAnnual.Should().Be(12.5m);
        debt.MinimumPayment.Should().Be(2_500_000m);
        debt.DueDateMonthly.Should().Be(15);
        debt.Status.Should().Be(DebtStatus.Active);
        debt.IsDeleted.Should().BeFalse();
        debt.Transactions.Should().BeEmpty();
    }

    [Fact]
    public void Constructor_InitialBalanceZero_SetsStatusPaidOff()
    {
        // Act
        var debt = new Debt(
            _userId,
            "Mượn Bạn Tiền",
            DebtType.Personal,
            "Nguyễn Văn A",
            5_000_000m,
            0m,
            0m,
            1,
            DateTime.UtcNow,
            null,
            0m
        );

        // Assert
        debt.Status.Should().Be(DebtStatus.PaidOff);
        debt.CurrentBalance.Should().Be(0m);
    }

    [Theory]
    [InlineData("", "Chủ nợ", 1000, 10, 100, 15)] // Tên rỗng
    [InlineData("Nợ", "", 1000, 10, 100, 15)] // Chủ nợ rỗng
    [InlineData("Nợ", "Chủ nợ", 0, 10, 100, 15)] // Gốc = 0
    [InlineData("Nợ", "Chủ nợ", -100, 10, 100, 15)] // Gốc âm
    [InlineData("Nợ", "Chủ nợ", 1000, -1, 100, 15)] // Lãi âm
    [InlineData("Nợ", "Chủ nợ", 1000, 10, -50, 15)] // Trả tối thiểu âm
    [InlineData("Nợ", "Chủ nợ", 1000, 10, 100, 0)] // Ngày < 1
    [InlineData("Nợ", "Chủ nợ", 1000, 10, 100, 32)] // Ngày > 31
    public void Constructor_InvalidParameters_ThrowsArgumentException(
        string name,
        string creditor,
        decimal principal,
        decimal interest,
        decimal minPay,
        int dueDay)
    {
        // Act
        Action act = () => new Debt(
            _userId,
            name,
            DebtType.Personal,
            creditor,
            principal,
            interest,
            minPay,
            dueDay,
            DateTime.UtcNow
        );

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void RecordPayment_ValidAmounts_DecreasesBalanceAndAddsTransaction()
    {
        // Arrange
        var debt = new Debt(
            _userId,
            "Thẻ Tín Dụng HSBC",
            DebtType.CreditCard,
            "HSBC Bank",
            10_000_000m,
            24m,
            1_000_000m,
            20,
            DateTime.UtcNow
        );

        // Act
        var trans = debt.RecordPayment(
            _walletId,
            2_500_000m,
            2_000_000m,
            500_000m,
            DateTime.UtcNow,
            "Trả đợt 1"
        );

        // Assert
        trans.Should().NotBeNull();
        trans.DebtId.Should().Be(debt.Id);
        trans.Amount.Should().Be(2_500_000m);
        trans.PrincipalPaid.Should().Be(2_000_000m);
        trans.InterestPaid.Should().Be(500_000m);

        debt.CurrentBalance.Should().Be(8_000_000m);
        debt.Status.Should().Be(DebtStatus.Active);
    }

    [Fact]
    public void RecordPayment_PrincipalExceedsCurrentBalance_ThrowsInvalidOperationException()
    {
        // Arrange
        var debt = new Debt(
            _userId,
            "Vay Bạn Thân",
            DebtType.Personal,
            "Trần Thị B",
            2_000_000m,
            0m,
            0m,
            5,
            DateTime.UtcNow
        );

        // Act
        Action act = () => debt.RecordPayment(
            _walletId,
            3_000_000m,
            3_000_000m,
            0m,
            DateTime.UtcNow
        );

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*không được vượt quá dư nợ hiện tại*");
    }

    [Fact]
    public void RecordPayment_WhenFullyPaid_SetsStatusPaidOffAndBalanceZero()
    {
        // Arrange
        var debt = new Debt(
            _userId,
            "Vay Bạn Thân",
            DebtType.Personal,
            "Trần Thị B",
            2_000_000m,
            0m,
            0m,
            5,
            DateTime.UtcNow
        );

        // Act
        debt.RecordPayment(_walletId, 2_000_000m, 2_000_000m, 0m, DateTime.UtcNow);

        // Assert
        debt.CurrentBalance.Should().Be(0m);
        debt.Status.Should().Be(DebtStatus.PaidOff);
    }

    [Fact]
    public void RecordPayment_WhenAlreadyPaidOff_ThrowsInvalidOperationException()
    {
        // Arrange
        var debt = new Debt(
            _userId,
            "Vay Bạn Thân",
            DebtType.Personal,
            "Trần Thị B",
            2_000_000m,
            0m,
            0m,
            5,
            DateTime.UtcNow
        );
        debt.RecordPayment(_walletId, 2_000_000m, 2_000_000m, 0m, DateTime.UtcNow);

        // Act
        Action act = () => debt.RecordPayment(_walletId, 500_000m, 500_000m, 0m, DateTime.UtcNow);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*đã được tất toán hoàn toàn*");
    }

    [Fact]
    public void UpdateDetails_ValidParameters_UpdatesSuccessfully()
    {
        // Arrange
        var debt = new Debt(
            _userId,
            "Khoản Nợ Cũ",
            DebtType.Personal,
            "Chủ Nợ A",
            5_000_000m,
            5m,
            500_000m,
            10,
            DateTime.UtcNow
        );

        // Act
        debt.UpdateDetails(
            "Khoản Nợ Mới",
            DebtType.InstallmentLoan,
            "Ngân hàng VPBank",
            15m,
            1_000_000m,
            25,
            DateTime.UtcNow.AddMonths(6),
            "Ghi chú mới"
        );

        // Assert
        debt.Name.Should().Be("Khoản Nợ Mới");
        debt.DebtType.Should().Be(DebtType.InstallmentLoan);
        debt.CreditorName.Should().Be("Ngân hàng VPBank");
        debt.InterestRateAnnual.Should().Be(15m);
        debt.MinimumPayment.Should().Be(1_000_000m);
        debt.DueDateMonthly.Should().Be(25);
        debt.Notes.Should().Be("Ghi chú mới");
    }
}
