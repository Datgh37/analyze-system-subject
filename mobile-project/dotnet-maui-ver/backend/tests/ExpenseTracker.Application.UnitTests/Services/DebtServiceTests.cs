using ExpenseTracker.Application.DTOs;
using ExpenseTracker.Application.DTOs.Debts;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Application.Services;
using ExpenseTracker.Application.Validators;
using ExpenseTracker.Domain.Entities;
using ExpenseTracker.Domain.Enums;
using ExpenseTracker.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ExpenseTracker.Application.UnitTests.Services;

public class DebtServiceTests
{
    private readonly ApplicationDbContext _dbContext;
    private readonly Mock<IFinancialRealtimeNotifier> _notifierMock;
    private readonly DebtService _debtService;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Wallet _wallet;

    public DebtServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _notifierMock = new Mock<IFinancialRealtimeNotifier>();

        var createValidator = new CreateDebtDtoValidator();
        var updateValidator = new UpdateDebtDtoValidator();
        var paymentValidator = new RecordDebtPaymentDtoValidator();

        _debtService = new DebtService(
            _dbContext,
            _notifierMock.Object,
            createValidator,
            updateValidator,
            paymentValidator
        );

        // Seed ví tiền 20,000,000 VND
        _wallet = new Wallet(_userId, "Ví Tiền Mặt", WalletType.Cash, 20_000_000m, "VND");
        _dbContext.Wallets.Add(_wallet);
        _dbContext.SaveChanges();
    }

    [Fact]
    public async Task CreateDebtAsync_WithValidData_CreatesDebtAndCalculatesProgress()
    {
        // Arrange
        var request = new CreateDebtDto(
            Name: "Vay Mua Xe Máy",
            DebtType: (int)DebtType.InstallmentLoan,
            CreditorName: "HD Saison",
            PrincipalAmount: 25_000_000m,
            InterestRateAnnual: 18.0m,
            MinimumPayment: 2_000_000m,
            DueDateMonthly: 10,
            StartDate: DateTime.UtcNow,
            ExpectedPayoffDate: DateTime.UtcNow.AddMonths(12),
            CurrentBalance: 20_000_000m,
            Notes: "Trả góp xe Wave Alpha"
        );

        // Act
        var result = await _debtService.CreateDebtAsync(_userId, request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Vay Mua Xe Máy");
        result.CreditorName.Should().Be("HD Saison");
        result.PrincipalAmount.Should().Be(25_000_000m);
        result.CurrentBalance.Should().Be(20_000_000m);
        result.PaidAmount.Should().Be(5_000_000m);
        result.ProgressPercent.Should().Be(20.0m);
        result.DebtTypeName.Should().Be("Vay tiêu dùng / Trả góp");
        result.StatusName.Should().Be("Đang trả nợ");

        var dbDebt = await _dbContext.Debts.FirstOrDefaultAsync(d => d.Id == result.Id);
        dbDebt.Should().NotBeNull();
        dbDebt!.UserId.Should().Be(_userId);
    }

    [Fact]
    public async Task CreateDebtAsync_WithInvalidPrincipal_ThrowsArgumentException()
    {
        // Arrange
        var request = new CreateDebtDto(
            Name: "Vay Mua Xe Máy",
            DebtType: (int)DebtType.InstallmentLoan,
            CreditorName: "HD Saison",
            PrincipalAmount: 0m, // Không hợp lệ
            InterestRateAnnual: 18.0m,
            MinimumPayment: 2_000_000m,
            DueDateMonthly: 10,
            StartDate: DateTime.UtcNow
        );

        // Act
        Func<Task> act = () => _debtService.CreateDebtAsync(_userId, request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Số tiền gốc ban đầu phải lớn hơn 0*");
    }

    [Fact]
    public async Task CreateDebtAsync_WithNegativeInterest_ThrowsArgumentException()
    {
        // Arrange
        var request = new CreateDebtDto(
            Name: "Vay Mua Xe Máy",
            DebtType: (int)DebtType.InstallmentLoan,
            CreditorName: "HD Saison",
            PrincipalAmount: 10_000_000m,
            InterestRateAnnual: -5.0m, // Lãi âm
            MinimumPayment: 2_000_000m,
            DueDateMonthly: 10,
            StartDate: DateTime.UtcNow
        );

        // Act
        Func<Task> act = () => _debtService.CreateDebtAsync(_userId, request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("*Lãi suất năm không được âm*");
    }

    [Fact]
    public async Task GetDebtsAsync_ReturnsAllUserDebts_OrderedByStatusAndDueDate()
    {
        // Arrange
        var debt1 = new Debt(_userId, "Thẻ Tín Dụng 1", DebtType.CreditCard, "Bank A", 5_000_000m, 20m, 500_000m, 25, DateTime.UtcNow);
        var debt2 = new Debt(_userId, "Vay Cá Nhân", DebtType.Personal, "Bạn B", 2_000_000m, 0m, 0m, 5, DateTime.UtcNow);
        _dbContext.Debts.AddRange(debt1, debt2);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _debtService.GetDebtsAsync(_userId);

        // Assert
        result.Should().HaveCount(2);
        result.First().DueDateMonthly.Should().Be(5); // Due date 5 trước 25
    }

    [Fact]
    public async Task GetDebtByIdAsync_ReturnsDebtDetailWithTransactions()
    {
        // Arrange
        var debt = new Debt(_userId, "Vay Ngân Hàng", DebtType.MortgageOrBankLoan, "BIDV", 50_000_000m, 9.5m, 3_000_000m, 15, DateTime.UtcNow);
        var trans = debt.RecordPayment(_wallet.Id, 3_000_000m, 2_500_000m, 500_000m, DateTime.UtcNow, "Kỳ 1");
        _dbContext.Debts.Add(debt);
        _dbContext.DebtTransactions.Add(trans);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _debtService.GetDebtByIdAsync(debt.Id, _userId);

        // Assert
        result.Should().NotBeNull();
        result!.Debt.Name.Should().Be("Vay Ngân Hàng");
        result.Transactions.Should().HaveCount(1);
        result.Transactions.First().PrincipalPaid.Should().Be(2_500_000m);
        result.Transactions.First().InterestPaid.Should().Be(500_000m);
    }

    [Fact]
    public async Task GetDebtByIdAsync_WhenNotFound_ReturnsNull()
    {
        // Act
        var result = await _debtService.GetDebtByIdAsync(Guid.NewGuid(), _userId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetDebtSummaryAsync_CalculatesTotalDebtAndMonthlyPaymentCorrectly()
    {
        // Arrange
        var debt1 = new Debt(_userId, "Khoản 1", DebtType.CreditCard, "Bank A", 10_000_000m, 10m, 1_000_000m, 10, DateTime.UtcNow, null, 8_000_000m);
        var debt2 = new Debt(_userId, "Khoản 2", DebtType.Personal, "Bạn B", 5_000_000m, 0m, 500_000m, 15, DateTime.UtcNow, null, 5_000_000m);
        var debtPaid = new Debt(_userId, "Khoản Đã Xong", DebtType.Personal, "Bạn C", 3_000_000m, 0m, 0m, 20, DateTime.UtcNow, null, 0m);

        _dbContext.Debts.AddRange(debt1, debt2, debtPaid);
        await _dbContext.SaveChangesAsync();

        // Act
        var summary = await _debtService.GetDebtSummaryAsync(_userId);

        // Assert
        summary.TotalCurrentDebt.Should().Be(13_000_000m); // 8tr + 5tr (khoản đã xong không tính)
        summary.TotalInitialPrincipal.Should().Be(18_000_000m); // 10tr + 5tr + 3tr
        summary.TotalPaidPrincipal.Should().Be(5_000_000m); // (10-8) + (5-5) + (3-0) = 2tr + 0 + 3tr = 5tr
        summary.TotalMonthlyMinimumPayment.Should().Be(1_500_000m); // 1tr + 500k
        summary.ActiveDebtsCount.Should().Be(2);
        summary.PaidOffDebtsCount.Should().Be(1);
    }

    [Fact]
    public async Task UpdateDebtAsync_WithValidData_UpdatesProperties()
    {
        // Arrange
        var debt = new Debt(_userId, "Tên Cũ", DebtType.Personal, "Chủ Nợ A", 10_000_000m, 5m, 500_000m, 10, DateTime.UtcNow);
        _dbContext.Debts.Add(debt);
        await _dbContext.SaveChangesAsync();

        var updateRequest = new UpdateDebtDto(
            Name: "Tên Mới",
            DebtType: (int)DebtType.InstallmentLoan,
            CreditorName: "Chủ Nợ B",
            InterestRateAnnual: 12m,
            MinimumPayment: 1_000_000m,
            DueDateMonthly: 20,
            Notes: "Ghi chú cập nhật"
        );

        // Act
        var updated = await _debtService.UpdateDebtAsync(debt.Id, _userId, updateRequest);

        // Assert
        updated.Name.Should().Be("Tên Mới");
        updated.CreditorName.Should().Be("Chủ Nợ B");
        updated.InterestRateAnnual.Should().Be(12m);
        updated.MinimumPayment.Should().Be(1_000_000m);
        updated.DueDateMonthly.Should().Be(20);
        updated.Notes.Should().Be("Ghi chú cập nhật");
    }

    [Fact]
    public async Task UpdateDebtAsync_WhenDebtNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var updateRequest = new UpdateDebtDto(
            Name: "Tên Mới",
            DebtType: (int)DebtType.Personal,
            CreditorName: "Chủ Nợ B",
            InterestRateAnnual: 12m,
            MinimumPayment: 1_000_000m,
            DueDateMonthly: 20
        );

        // Act
        Func<Task> act = () => _debtService.UpdateDebtAsync(Guid.NewGuid(), _userId, updateRequest);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task RecordPaymentAsync_ValidPayment_DebitsWalletAndReducesDebtBalance()
    {
        // Arrange
        var debt = new Debt(_userId, "Vay Mua Laptop", DebtType.InstallmentLoan, "FE Credit", 10_000_000m, 15m, 2_000_000m, 15, DateTime.UtcNow);
        _dbContext.Debts.Add(debt);
        await _dbContext.SaveChangesAsync();

        var initialWalletBalance = _wallet.Balance; // 20_000_000

        var paymentDto = new RecordDebtPaymentDto(
            WalletId: _wallet.Id,
            Amount: 2_500_000m,
            PrincipalPaid: 2_000_000m,
            InterestPaid: 500_000m,
            PaymentDate: DateTime.UtcNow,
            Note: "Trả đợt 1"
        );

        // Act
        var result = await _debtService.RecordPaymentAsync(debt.Id, _userId, paymentDto);

        // Assert
        result.Should().NotBeNull();
        result.Amount.Should().Be(2_500_000m);
        result.PrincipalPaid.Should().Be(2_000_000m);
        result.InterestPaid.Should().Be(500_000m);

        // Verify Debt
        debt.CurrentBalance.Should().Be(8_000_000m);
        debt.Status.Should().Be(DebtStatus.Active);

        // Verify Wallet
        _wallet.Balance.Should().Be(initialWalletBalance - 2_500_000m);

        // Verify SignalR broadcast
        _notifierMock.Verify(n => n.NotifyBalanceUpdateAsync(
            _userId,
            It.Is<BalanceUpdateNotificationDto>(d => d.WalletId == _wallet.Id && d.ChangeAmount == -2_500_000m),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task RecordPaymentAsync_WhenWalletNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var debt = new Debt(_userId, "Vay Mua Laptop", DebtType.Personal, "Chủ Nợ", 5_000_000m, 0m, 0m, 10, DateTime.UtcNow);
        _dbContext.Debts.Add(debt);
        await _dbContext.SaveChangesAsync();

        var paymentDto = new RecordDebtPaymentDto(
            WalletId: Guid.NewGuid(), // Ví ảo không tồn tại
            Amount: 1_000_000m,
            PrincipalPaid: 1_000_000m,
            InterestPaid: 0m,
            PaymentDate: DateTime.UtcNow
        );

        // Act
        Func<Task> act = () => _debtService.RecordPaymentAsync(debt.Id, _userId, paymentDto);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*Không tìm thấy ví thanh toán*");
    }

    [Fact]
    public async Task RecordPaymentAsync_WhenInsufficientWalletBalance_ThrowsInvalidOperationException()
    {
        // Arrange
        var debt = new Debt(_userId, "Vay Ngân Hàng", DebtType.MortgageOrBankLoan, "Vietcombank", 50_000_000m, 10m, 1_000_000m, 10, DateTime.UtcNow);
        _dbContext.Debts.Add(debt);
        await _dbContext.SaveChangesAsync();

        // Ví chỉ có 20 triệu, cố thanh toán 25 triệu
        var paymentDto = new RecordDebtPaymentDto(
            WalletId: _wallet.Id,
            Amount: 25_000_000m,
            PrincipalPaid: 20_000_000m,
            InterestPaid: 5_000_000m,
            PaymentDate: DateTime.UtcNow
        );

        // Act
        Func<Task> act = () => _debtService.RecordPaymentAsync(debt.Id, _userId, paymentDto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*không đủ để thực hiện giao dịch*");
    }

    [Fact]
    public async Task RecordPaymentAsync_WhenPrincipalPaidExceedsCurrentBalance_ThrowsInvalidOperationException()
    {
        // Arrange
        var debt = new Debt(_userId, "Vay Bạn Bè", DebtType.Personal, "Bạn Thân", 2_000_000m, 0m, 0m, 10, DateTime.UtcNow);
        _dbContext.Debts.Add(debt);
        await _dbContext.SaveChangesAsync();

        var paymentDto = new RecordDebtPaymentDto(
            WalletId: _wallet.Id,
            Amount: 3_000_000m,
            PrincipalPaid: 3_000_000m, // Vượt quá dư nợ 2 triệu
            InterestPaid: 0m,
            PaymentDate: DateTime.UtcNow
        );

        // Act
        Func<Task> act = () => _debtService.RecordPaymentAsync(debt.Id, _userId, paymentDto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*không được vượt quá dư nợ hiện tại*");
    }

    [Fact]
    public async Task RecordPaymentAsync_WhenDebtPaidOff_SetsStatusToPaidOffAndBroadcastsInAppNotification()
    {
        // Arrange
        var debt = new Debt(_userId, "Vay Mượn Nhanh", DebtType.Personal, "Anh Ba", 3_000_000m, 0m, 0m, 10, DateTime.UtcNow);
        _dbContext.Debts.Add(debt);
        await _dbContext.SaveChangesAsync();

        var paymentDto = new RecordDebtPaymentDto(
            WalletId: _wallet.Id,
            Amount: 3_000_000m,
            PrincipalPaid: 3_000_000m,
            InterestPaid: 0m,
            PaymentDate: DateTime.UtcNow,
            Note: "Tất toán toàn bộ nợ"
        );

        // Act
        var result = await _debtService.RecordPaymentAsync(debt.Id, _userId, paymentDto);

        // Assert
        debt.CurrentBalance.Should().Be(0m);
        debt.Status.Should().Be(DebtStatus.PaidOff);

        // Verify InApp Notification broadcast chúc mừng
        _notifierMock.Verify(n => n.NotifyInAppAsync(
            _userId,
            It.Is<InAppNotificationDto>(d => d.Type == "debt_paidoff" && d.Title.Contains("Chúc mừng")),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task RecordPaymentAsync_WhenDebtAlreadyPaidOff_ThrowsInvalidOperationException()
    {
        // Arrange
        var debt = new Debt(_userId, "Vay Mượn Nhanh", DebtType.Personal, "Anh Ba", 1_000_000m, 0m, 0m, 10, DateTime.UtcNow);
        debt.RecordPayment(_wallet.Id, 1_000_000m, 1_000_000m, 0m, DateTime.UtcNow);
        _dbContext.Debts.Add(debt);
        await _dbContext.SaveChangesAsync();

        var paymentDto = new RecordDebtPaymentDto(
            WalletId: _wallet.Id,
            Amount: 500_000m,
            PrincipalPaid: 500_000m,
            InterestPaid: 0m,
            PaymentDate: DateTime.UtcNow
        );

        // Act
        Func<Task> act = () => _debtService.RecordPaymentAsync(debt.Id, _userId, paymentDto);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*đã được tất toán hoàn toàn*");
    }

    [Fact]
    public async Task ArchiveDebtAsync_SoftDeletesDebt_EntityExcludedFromQueries_AndBroadcastsRealtime()
    {
        // Arrange
        var debt = new Debt(_userId, "Khoản Nợ Xóa Mềm", DebtType.Personal, "Người Vay", 2_000_000m, 0m, 0m, 10, DateTime.UtcNow);
        _dbContext.Debts.Add(debt);
        await _dbContext.SaveChangesAsync();

        // Act
        var success = await _debtService.ArchiveDebtAsync(debt.Id, _userId);

        // Assert
        success.Should().BeTrue();

        // Debt is soft deleted, query returns null due to Global Query Filter
        var foundDebt = await _dbContext.Debts.FirstOrDefaultAsync(d => d.Id == debt.Id);
        foundDebt.Should().BeNull();

        // Direct query ignoring query filter shows IsDeleted == true
        var rawDebt = await _dbContext.Debts.IgnoreQueryFilters().FirstOrDefaultAsync(d => d.Id == debt.Id);
        rawDebt.Should().NotBeNull();
        rawDebt!.IsDeleted.Should().BeTrue();
        rawDebt.DeletedAt.Should().NotBeNull();

        // Verify SignalR broadcast
        _notifierMock.Verify(n => n.NotifyEntityArchivedAsync(
            _userId,
            It.Is<EntityArchivedNotificationDto>(d => d.EntityType == "Debt" && d.EntityId == debt.Id),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task RecordPaymentAsync_WhenWalletArchived_ThrowsInvalidOperationException()
    {
        // Arrange
        var archivedWallet = new Wallet(_userId, "Ví Cũ Lưu Trữ", WalletType.Cash, 10_000_000m);
        archivedWallet.Archive();
        _dbContext.Wallets.Add(archivedWallet);

        var debt = new Debt(_userId, "Khoản Vay", DebtType.Personal, "Chủ Nợ", 5_000_000m, 0m, 0m, 10, DateTime.UtcNow);
        _dbContext.Debts.Add(debt);
        await _dbContext.SaveChangesAsync();

        var paymentDto = new RecordDebtPaymentDto(
            WalletId: archivedWallet.Id,
            Amount: 1_000_000m,
            PrincipalPaid: 1_000_000m,
            InterestPaid: 0m,
            PaymentDate: DateTime.UtcNow
        );

        // Act
        Func<Task> act = () => _debtService.RecordPaymentAsync(debt.Id, _userId, paymentDto);

        // Assert
        await act.Should().ThrowAsync<Exception>();
    }

    [Fact]
    public async Task RecordPaymentAsync_WhenSignalRFails_PaymentStillSuccessfullyCommitted()
    {
        // Arrange
        var debt = new Debt(_userId, "Vay Mua Điện Thoại", DebtType.InstallmentLoan, "FE", 8_000_000m, 10m, 1_000_000m, 15, DateTime.UtcNow);
        _dbContext.Debts.Add(debt);
        await _dbContext.SaveChangesAsync();

        // Giả lập SignalR bị ngắt kết nối ném Exception
        _notifierMock.Setup(n => n.NotifyBalanceUpdateAsync(It.IsAny<Guid>(), It.IsAny<BalanceUpdateNotificationDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new System.Net.WebSockets.WebSocketException("Connection reset"));

        var paymentDto = new RecordDebtPaymentDto(
            WalletId: _wallet.Id,
            Amount: 1_000_000m,
            PrincipalPaid: 1_000_000m,
            InterestPaid: 0m,
            PaymentDate: DateTime.UtcNow,
            Note: "Mạng rớt nhưng thanh toán vẫn thành công"
        );

        // Act
        var result = await _debtService.RecordPaymentAsync(debt.Id, _userId, paymentDto);

        // Assert
        result.Should().NotBeNull();
        result.PrincipalPaid.Should().Be(1_000_000m);
        debt.CurrentBalance.Should().Be(7_000_000m);
        _wallet.Balance.Should().Be(19_000_000m); // Đã trừ tiền thành công
    }

    [Fact]
    public async Task GetDebtsAsync_WhenPastExpectedPayoffDate_ReportsStatusAsOverdue()
    {
        // Arrange: Khoản nợ có ngày dự kiến tất toán trong quá khứ nhưng vẫn còn dư nợ
        var pastDate = DateTime.UtcNow.AddMonths(-2);
        var overdueDebt = new Debt(_userId, "Khoản Nợ Quá Hạn", DebtType.Personal, "Chủ Nợ X", 5_000_000m, 0m, 0m, 1, DateTime.UtcNow.AddMonths(-6), pastDate, 3_000_000m);
        _dbContext.Debts.Add(overdueDebt);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _debtService.GetDebtsAsync(_userId);

        // Assert
        var item = result.First(d => d.Id == overdueDebt.Id);
        item.Status.Should().Be((int)DebtStatus.Overdue);
        item.StatusName.Should().Be("Quá hạn");
    }
}
