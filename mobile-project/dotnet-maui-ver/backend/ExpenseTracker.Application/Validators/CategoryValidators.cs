using ExpenseTracker.Application.DTOs.Categories;
using FluentValidation;

namespace ExpenseTracker.Application.Validators;

// Validator for creating category
public class CreateCategoryValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên danh mục không được để trống.")
            .MaximumLength(100).WithMessage("Tên danh mục tối đa 100 ký tự.");

        RuleFor(x => x.TransactionType)
            .IsInEnum().WithMessage("Loại giao dịch không hợp lệ.");

        RuleFor(x => x.ColorHex)
            .Matches(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$")
            .WithMessage("Mã màu phải là định dạng Hex hợp lệ (VD: #10B981).");
    }
}

// Validator for updating category
public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên danh mục không được để trống.")
            .MaximumLength(100).WithMessage("Tên danh mục tối đa 100 ký tự.");

        RuleFor(x => x.ColorHex)
            .Matches(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$")
            .WithMessage("Mã màu phải là định dạng Hex hợp lệ (VD: #10B981).");
    }
}
