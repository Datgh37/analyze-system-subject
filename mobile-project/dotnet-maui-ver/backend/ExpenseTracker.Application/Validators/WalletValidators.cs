using ExpenseTracker.Application.DTOs.Wallets;
using ExpenseTracker.Domain.Enums;
using FluentValidation;

namespace ExpenseTracker.Application.Validators;

// Validator enforcing rules when creating a new wallet
public class CreateWalletValidator : AbstractValidator<CreateWalletRequest>
{
    public CreateWalletValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên ví không được để trống.")
            .MaximumLength(100).WithMessage("Tên ví tối đa 100 ký tự.");

        RuleFor(x => x.WalletType)
            .IsInEnum().WithMessage("Loại ví không hợp lệ.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Đơn vị tiền tệ không được để trống.")
            .Length(3).WithMessage("Mã tiền tệ phải gồm 3 ký tự (VD: VND, USD).");

        RuleFor(x => x.StatementDay)
            .InclusiveBetween(1, 31).When(x => x.StatementDay.HasValue)
            .WithMessage("Ngày chốt sao kê phải từ ngày 1 đến 31.");

        RuleFor(x => x.PaymentDueDay)
            .InclusiveBetween(1, 31).When(x => x.PaymentDueDay.HasValue)
            .WithMessage("Ngày đến hạn thanh toán phải từ ngày 1 đến 31.");
    }
}

// Validator enforcing rules when updating an existing wallet
public class UpdateWalletValidator : AbstractValidator<UpdateWalletRequest>
{
    public UpdateWalletValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên ví không được để trống.")
            .MaximumLength(100).WithMessage("Tên ví tối đa 100 ký tự.");

        RuleFor(x => x.StatementDay)
            .InclusiveBetween(1, 31).When(x => x.StatementDay.HasValue)
            .WithMessage("Ngày chốt sao kê phải từ ngày 1 đến 31.");

        RuleFor(x => x.PaymentDueDay)
            .InclusiveBetween(1, 31).When(x => x.PaymentDueDay.HasValue)
            .WithMessage("Ngày đến hạn thanh toán phải từ ngày 1 đến 31.");
    }
}

// Validator ensuring internal transfer validity and distinct accounts
public class TransferMoneyValidator : AbstractValidator<TransferMoneyRequest>
{
    public TransferMoneyValidator()
    {
        RuleFor(x => x.SourceWalletId)
            .NotEmpty().WithMessage("Ví nguồn không được để trống.");

        RuleFor(x => x.DestinationWalletId)
            .NotEmpty().WithMessage("Ví đích không được để trống.")
            .NotEqual(x => x.SourceWalletId).WithMessage("Ví nguồn và ví đích không được trùng nhau.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Số tiền chuyển phải lớn hơn 0.");

        RuleFor(x => x.TransferDate)
            .NotEmpty().WithMessage("Ngày chuyển tiền không được để trống.");
    }
}

// Validator for balance adjustments
public class AdjustBalanceValidator : AbstractValidator<AdjustBalanceRequest>
{
    public AdjustBalanceValidator()
    {
        RuleFor(x => x.Reason)
            .MaximumLength(255).WithMessage("Lý do cân chỉnh tối đa 255 ký tự.");
    }
}
