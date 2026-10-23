using ExpenseTracker.Application.DTOs.AutoRules;
using FluentValidation;

namespace ExpenseTracker.Application.Validators;

public class CreateAutoRuleValidator : AbstractValidator<CreateAutoRuleRequest>
{
    public CreateAutoRuleValidator()
    {
        RuleFor(x => x.KeywordPattern)
            .NotEmpty().WithMessage("Vui lòng nhập từ khóa nhận diện hợp lệ (tối thiểu 2 ký tự).")
            .MinimumLength(2).WithMessage("Vui lòng nhập từ khóa nhận diện hợp lệ (tối thiểu 2 ký tự).")
            .MaximumLength(100).WithMessage("Từ khóa không được vượt quá 100 ký tự.");

        RuleFor(x => x.TargetCategoryId)
            .NotEmpty().WithMessage("Vui lòng chọn danh mục đích.");

        RuleFor(x => x.PriorityOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Thứ tự ưu tiên không được âm.");
    }
}

public class UpdateAutoRuleValidator : AbstractValidator<UpdateAutoRuleRequest>
{
    public UpdateAutoRuleValidator()
    {
        RuleFor(x => x.KeywordPattern)
            .NotEmpty().WithMessage("Vui lòng nhập từ khóa nhận diện hợp lệ (tối thiểu 2 ký tự).")
            .MinimumLength(2).WithMessage("Vui lòng nhập từ khóa nhận diện hợp lệ (tối thiểu 2 ký tự).")
            .MaximumLength(100).WithMessage("Từ khóa không được vượt quá 100 ký tự.");

        RuleFor(x => x.TargetCategoryId)
            .NotEmpty().WithMessage("Vui lòng chọn danh mục đích.");

        RuleFor(x => x.PriorityOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Thứ tự ưu tiên không được âm.");
    }
}
