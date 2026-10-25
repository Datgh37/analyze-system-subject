using ExpenseTracker.Domain.Enums;

namespace ExpenseTracker.Domain.Entities;

// Aggregate Root managing personal savings goal, auto-deposit schedule, and progress tracking (UC-06.01 - 06.05)
public class SavingsGoal
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public decimal TargetAmount { get; private set; }
    public decimal CurrentAmount { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime TargetDate { get; private set; }
    public SavingsGoalType Type { get; private set; } = SavingsGoalType.Other;
    public SavingsGoalStatus Status { get; private set; } = SavingsGoalStatus.Active;
    public SavingsGoalPriority Priority { get; private set; } = SavingsGoalPriority.Medium;
    public string? IconKey { get; private set; }
    public string? ColorHex { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    // Auto-deposit schedule configuration (UC-06.04 "Pay yourself first")
    public bool IsAutoDepositEnabled { get; private set; }
    public decimal? AutoDepositAmount { get; private set; }
    public int? AutoDepositDayOfMonth { get; private set; }
    public Guid? AutoDepositWalletId { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Navigation collection
    private readonly List<SavingsGoalContribution> _contributions = new();
    public IReadOnlyCollection<SavingsGoalContribution> Contributions => _contributions.AsReadOnly();

    // Presentation & Business computed metrics
    public decimal ProgressPercent => TargetAmount > 0 
        ? Math.Clamp(Math.Round((CurrentAmount / TargetAmount) * 100m, 2), 0m, 100m) 
        : 0m;

    public decimal RemainingAmount => Math.Max(0m, TargetAmount - CurrentAmount);

    public bool IsCompleted => Status == SavingsGoalStatus.Completed || CurrentAmount >= TargetAmount;

    public bool IsEmergencyFund => Type == SavingsGoalType.EmergencyFund;

    private SavingsGoal() { } // EF Core constructor

    public SavingsGoal(
        Guid userId,
        string name,
        decimal targetAmount,
        DateTime startDate,
        DateTime targetDate,
        SavingsGoalType type = SavingsGoalType.Other,
        SavingsGoalPriority priority = SavingsGoalPriority.Medium,
        decimal initialAmount = 0m,
        string? description = null,
        string? iconKey = null,
        string? colorHex = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tên mục tiêu tích lũy không được để trống.", nameof(name));

        if (targetAmount <= 0)
            throw new ArgumentException("Số tiền mục tiêu tích lũy phải lớn hơn 0 đ.", nameof(targetAmount));

        if (initialAmount < 0)
            throw new ArgumentException("Số tiền khởi tạo ban đầu không được âm.", nameof(initialAmount));

        if (targetDate.Date < startDate.Date)
            throw new ArgumentException("Hạn chót hoàn thành mục tiêu phải sau hoặc bằng ngày bắt đầu.", nameof(targetDate));

        Id = Guid.NewGuid();
        UserId = userId;
        Name = name.Trim();
        Description = description?.Trim();
        TargetAmount = targetAmount;
        CurrentAmount = initialAmount;
        StartDate = startDate.Date;
        TargetDate = targetDate.Date;
        Type = type;
        Priority = priority;
        IconKey = iconKey ?? (type == SavingsGoalType.EmergencyFund ? "shield-check" : "piggy-bank");
        ColorHex = colorHex ?? (type == SavingsGoalType.EmergencyFund ? "#DC2626" : "#2563EB");
        Status = (initialAmount >= targetAmount) ? SavingsGoalStatus.Completed : SavingsGoalStatus.Active;
        CompletedAt = (initialAmount >= targetAmount) ? DateTime.UtcNow : null;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(
        string name,
        string? description,
        decimal targetAmount,
        DateTime targetDate,
        SavingsGoalPriority priority,
        string? iconKey = null,
        string? colorHex = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tên mục tiêu tích lũy không được để trống.", nameof(name));

        if (targetAmount <= 0)
            throw new ArgumentException("Số tiền mục tiêu tích lũy phải lớn hơn 0 đ.", nameof(targetAmount));

        if (targetDate.Date < StartDate.Date)
            throw new ArgumentException("Hạn chót hoàn thành mục tiêu phải sau hoặc bằng ngày bắt đầu.", nameof(targetDate));

        Name = name.Trim();
        Description = description?.Trim();
        TargetAmount = targetAmount;
        TargetDate = targetDate.Date;
        Priority = priority;
        if (!string.IsNullOrWhiteSpace(iconKey)) IconKey = iconKey;
        if (!string.IsNullOrWhiteSpace(colorHex)) ColorHex = colorHex;

        CheckCompletion();
        UpdatedAt = DateTime.UtcNow;
    }

    // Deposit money into savings goal (UC-06.02)
    public SavingsGoalContribution Deposit(decimal amount, Guid walletId, string? note = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Số tiền nạp vào quỹ tích lũy phải lớn hơn 0 đ.", nameof(amount));

        if (Status == SavingsGoalStatus.Cancelled)
            throw new InvalidOperationException("Không thể nạp tiền vào mục tiêu đã bị hủy.");

        CurrentAmount += amount;
        CheckCompletion();

        var contribution = new SavingsGoalContribution(
            Id,
            walletId,
            amount,
            ContributionType.Deposit,
            DateTime.UtcNow,
            note ?? "Nạp tiền vào quỹ tích lũy");

        _contributions.Add(contribution);
        UpdatedAt = DateTime.UtcNow;
        return contribution;
    }

    // Withdraw money from savings goal with Richard Thaler Cognitive Friction Nudge (UC-06.03)
    public SavingsGoalContribution Withdraw(decimal amount, Guid targetWalletId, string reason)
    {
        if (amount <= 0)
            throw new ArgumentException("Số tiền rút khỏi quỹ tích lũy phải lớn hơn 0 đ.", nameof(amount));

        if (amount > CurrentAmount)
            throw new InvalidOperationException($"Số tiền rút ({amount:N0} đ) vượt quá số dư hiện có trong quỹ ({CurrentAmount:N0} đ).");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Vui lòng nêu rõ lý do rút tiền (Cú hích ma sát nhận thức Richard Thaler Nudge).", nameof(reason));

        CurrentAmount -= amount;

        // If goal was completed but balance dropped below target, reopen status
        if (Status == SavingsGoalStatus.Completed && CurrentAmount < TargetAmount)
        {
            Status = SavingsGoalStatus.Active;
            CompletedAt = null;
        }

        var contribution = new SavingsGoalContribution(
            Id,
            targetWalletId,
            -amount,
            ContributionType.Withdrawal,
            DateTime.UtcNow,
            reason.Trim());

        _contributions.Add(contribution);
        UpdatedAt = DateTime.UtcNow;
        return contribution;
    }

    // Calculate monthly deposit required to achieve goal by target date (UC-06.01)
    public decimal CalculateMonthlyDepositRequired(DateTime currentDate)
    {
        if (IsCompleted || RemainingAmount <= 0) return 0m;

        int totalMonths = ((TargetDate.Year - currentDate.Year) * 12) + TargetDate.Month - currentDate.Month;
        if (totalMonths <= 0) totalMonths = 1; // Due in current month

        return Math.Round(RemainingAmount / totalMonths, 0);
    }

    // Configure Auto-deposit schedule (UC-06.04)
    public void ConfigureAutoDeposit(bool isEnabled, decimal? amount, int? dayOfMonth, Guid? walletId)
    {
        if (isEnabled)
        {
            if (amount is null or <= 0)
                throw new ArgumentException("Số tiền trích nạp tự động phải lớn hơn 0 đ.", nameof(amount));

            if (dayOfMonth is null or < 1 or > 31)
                throw new ArgumentException("Ngày trích nạp trong tháng phải từ ngày 1 đến 31.", nameof(dayOfMonth));

            if (walletId is null || walletId == Guid.Empty)
                throw new ArgumentException("Vui lòng chọn ví nguồn trích nợ tự động.", nameof(walletId));

            IsAutoDepositEnabled = true;
            AutoDepositAmount = amount;
            AutoDepositDayOfMonth = dayOfMonth;
            AutoDepositWalletId = walletId;
        }
        else
        {
            IsAutoDepositEnabled = false;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsCompleted()
    {
        Status = SavingsGoalStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void PauseGoal()
    {
        if (Status == SavingsGoalStatus.Active)
        {
            Status = SavingsGoalStatus.Paused;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void ResumeGoal()
    {
        if (Status == SavingsGoalStatus.Paused)
        {
            Status = CurrentAmount >= TargetAmount ? SavingsGoalStatus.Completed : SavingsGoalStatus.Active;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void CancelGoal()
    {
        Status = SavingsGoalStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(SavingsGoalStatus newStatus)
    {
        switch (newStatus)
        {
            case SavingsGoalStatus.Active:
                ResumeGoal();
                break;
            case SavingsGoalStatus.Paused:
                PauseGoal();
                break;
            case SavingsGoalStatus.Completed:
                MarkAsCompleted();
                break;
            case SavingsGoalStatus.Cancelled:
                CancelGoal();
                break;
        }
    }

    // Partially updates savings goal metadata and status via HTTP PATCH (RFC 7396)
    public void PatchDetails(
        string? name = null,
        string? description = null,
        decimal? targetAmount = null,
        DateTime? targetDate = null,
        SavingsGoalPriority? priority = null,
        string? iconKey = null,
        string? colorHex = null,
        SavingsGoalStatus? status = null)
    {
        if (name is not null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Tên mục tiêu tích lũy không được để trống.", nameof(name));
            Name = name.Trim();
        }

        if (description is not null)
            Description = description.Trim();

        if (targetAmount.HasValue)
        {
            if (targetAmount.Value <= 0)
                throw new ArgumentException("Số tiền mục tiêu tích lũy phải lớn hơn 0 đ.", nameof(targetAmount));
            TargetAmount = targetAmount.Value;
        }

        if (targetDate.HasValue)
        {
            if (targetDate.Value.Date < StartDate.Date)
                throw new ArgumentException("Hạn chót hoàn thành mục tiêu phải sau hoặc bằng ngày bắt đầu.", nameof(targetDate));
            TargetDate = targetDate.Value.Date;
        }

        if (priority.HasValue)
            Priority = priority.Value;

        if (!string.IsNullOrWhiteSpace(iconKey))
            IconKey = iconKey.Trim();

        if (!string.IsNullOrWhiteSpace(colorHex))
            ColorHex = colorHex.Trim();

        if (status.HasValue)
            UpdateStatus(status.Value);

        CheckCompletion();
        UpdatedAt = DateTime.UtcNow;
    }

    private void CheckCompletion()
    {
        if (CurrentAmount >= TargetAmount)
        {
            Status = SavingsGoalStatus.Completed;
            CompletedAt ??= DateTime.UtcNow;
        }
        else if (Status == SavingsGoalStatus.Completed && CurrentAmount < TargetAmount)
        {
            Status = SavingsGoalStatus.Active;
            CompletedAt = null;
        }
    }
}
