using ExpenseTracker.Application.DTOs.Transactions;
using FluentValidation;

namespace ExpenseTracker.Application.Validators;

public class CreateExpenseTransactionValidator : AbstractValidator<CreateExpenseTransactionRequest>
{
    public CreateExpenseTransactionValidator()
    {
        RuleFor(x => x.WalletId)
            .NotEmpty().WithMessage("Ví thanh toán không được để trống.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Danh mục chi tiêu không được để trống.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Số tiền chi tiêu phải lớn hơn 0.");

        RuleFor(x => x.TransactionDate)
            .NotEmpty().WithMessage("Ngày giao dịch không được để trống.");

        RuleFor(x => x.Note)
            .MaximumLength(500).WithMessage("Ghi chú không được vượt quá 500 ký tự.");

        RuleForEach(x => x.Items).ChildRules(items =>
        {
            items.RuleFor(i => i.ItemName)
                .NotEmpty().WithMessage("Tên món hàng không được để trống.");
            items.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Số lượng món hàng phải lớn hơn 0.");
            items.RuleFor(i => i.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Đơn giá không được âm.");
            items.RuleFor(i => i.TotalPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Thành tiền không được âm.");
        });
    }
}

public class UpdateExpenseTransactionValidator : AbstractValidator<UpdateExpenseTransactionRequest>
{
    public UpdateExpenseTransactionValidator()
    {
        RuleFor(x => x.WalletId)
            .NotEmpty().WithMessage("Ví thanh toán không được để trống.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Danh mục chi tiêu không được để trống.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Số tiền chi tiêu phải lớn hơn 0.");

        RuleFor(x => x.TransactionDate)
            .NotEmpty().WithMessage("Ngày giao dịch không được để trống.");

        RuleFor(x => x.Note)
            .MaximumLength(500).WithMessage("Ghi chú không được vượt quá 500 ký tự.");

        RuleForEach(x => x.Items).ChildRules(items =>
        {
            items.RuleFor(i => i.ItemName)
                .NotEmpty().WithMessage("Tên món hàng không được để trống.");
            items.RuleFor(i => i.Quantity)
                .GreaterThan(0).WithMessage("Số lượng món hàng phải lớn hơn 0.");
            items.RuleFor(i => i.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Đơn giá không được âm.");
            items.RuleFor(i => i.TotalPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Thành tiền không được âm.");
        });
    }
}
