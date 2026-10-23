using ExpenseTracker.Application.DTOs.RecurringIncomes;
using FluentValidation;

namespace ExpenseTracker.Application.Validators;

// Validator for creating recurring income schedule (UC-02.07)
public class CreateRecurringIncomeValidator : AbstractValidator<CreateRecurringIncomeRequest>
{
    public CreateRecurringIncomeValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên khoản thu định kỳ không được để trống.")
            .MaximumLength(150).WithMessage("Tên khoản thu định kỳ không được vượt quá 150 ký tự.");

        RuleFor(x => x.ExpectedAmount)
            .GreaterThan(0).WithMessage("Số tiền dự kiến phải lớn hơn 0.")
            .LessThanOrEqualTo(100_000_000_000m).WithMessage("Số tiền vượt quá giới hạn hệ thống cho phép.");

        RuleFor(x => x.WalletId)
            .NotEmpty().WithMessage("Vui lòng chọn ví nhận tiền.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Vui lòng chọn danh mục thu nhập.");

        RuleFor(x => x.Frequency)
            .IsInEnum().WithMessage("Tần suất lặp lại không hợp lệ.");

        RuleFor(x => x.DayOfMonth)
            .InclusiveBetween(1, 31).WithMessage("Ngày nhận tiền trong tháng phải từ ngày 1 đến ngày 31.");
    }
}

// Validator for updating recurring income schedule
public class UpdateRecurringIncomeValidator : AbstractValidator<UpdateRecurringIncomeRequest>
{
    public UpdateRecurringIncomeValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên khoản thu định kỳ không được để trống.")
            .MaximumLength(150).WithMessage("Tên khoản thu định kỳ không được vượt quá 150 ký tự.");

        RuleFor(x => x.ExpectedAmount)
            .GreaterThan(0).WithMessage("Số tiền dự kiến phải lớn hơn 0.")
            .LessThanOrEqualTo(100_000_000_000m).WithMessage("Số tiền vượt quá giới hạn hệ thống cho phép.");

        RuleFor(x => x.WalletId)
            .NotEmpty().WithMessage("Vui lòng chọn ví nhận tiền.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Vui lòng chọn danh mục thu nhập.");

        RuleFor(x => x.Frequency)
            .IsInEnum().WithMessage("Tần suất lặp lại không hợp lệ.");

        RuleFor(x => x.DayOfMonth)
            .InclusiveBetween(1, 31).WithMessage("Ngày nhận tiền trong tháng phải từ ngày 1 đến ngày 31.");
    }
}
