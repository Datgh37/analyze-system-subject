using ExpenseTracker.Application.DTOs.Transactions;
using FluentValidation;

namespace ExpenseTracker.Application.Validators;

// Validator for creating an income transaction
public class CreateIncomeTransactionValidator : AbstractValidator<CreateIncomeTransactionRequest>
{
    public CreateIncomeTransactionValidator()
    {
        RuleFor(x => x.WalletId)
            .NotEmpty().WithMessage("Vui lòng chọn ví tiếp nhận thu nhập.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Vui lòng chọn danh mục thu nhập.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Số tiền thu nhập phải lớn hơn 0.")
            .LessThanOrEqualTo(100_000_000_000m).WithMessage("Số tiền vượt quá giới hạn hệ thống cho phép.");

        RuleFor(x => x.TransactionDate)
            .NotEmpty().WithMessage("Ngày giao dịch không được để trống.")
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(7)).WithMessage("Ngày giao dịch không được vượt quá 7 ngày trong tương lai.");

        RuleFor(x => x.Note)
            .MaximumLength(500).WithMessage("Ghi chú tối đa 500 ký tự.");
    }
}

// Validator for updating an income transaction
public class UpdateIncomeTransactionValidator : AbstractValidator<UpdateIncomeTransactionRequest>
{
    public UpdateIncomeTransactionValidator()
    {
        RuleFor(x => x.WalletId)
            .NotEmpty().WithMessage("Vui lòng chọn ví tiếp nhận thu nhập.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Vui lòng chọn danh mục thu nhập.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Số tiền thu nhập phải lớn hơn 0.")
            .LessThanOrEqualTo(100_000_000_000m).WithMessage("Số tiền vượt quá giới hạn hệ thống cho phép.");

        RuleFor(x => x.TransactionDate)
            .NotEmpty().WithMessage("Ngày giao dịch không được để trống.")
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(7)).WithMessage("Ngày giao dịch không được vượt quá 7 ngày trong tương lai.");

        RuleFor(x => x.Note)
            .MaximumLength(500).WithMessage("Ghi chú tối đa 500 ký tự.");
    }
}
