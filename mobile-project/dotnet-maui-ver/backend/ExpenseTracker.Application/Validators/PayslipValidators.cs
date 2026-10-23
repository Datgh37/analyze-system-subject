using ExpenseTracker.Application.DTOs.Payslips;
using FluentValidation;

namespace ExpenseTracker.Application.Validators;

// Validator for confirming and persisting parsed payslip income (UC-02.08)
public class ConfirmPayslipValidator : AbstractValidator<ConfirmPayslipRequest>
{
    public ConfirmPayslipValidator()
    {
        RuleFor(x => x.WalletId)
            .NotEmpty().WithMessage("Vui lòng chọn ví nhận tiền lương thực tế.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Vui lòng chọn danh mục thu nhập (ví dụ: Lương chính).");

        RuleFor(x => x.GrossSalary)
            .GreaterThanOrEqualTo(0).WithMessage("Lương tổng (Gross) không thể âm.");

        RuleFor(x => x.NetSalary)
            .GreaterThan(0).WithMessage("Lương thực nhận (Net) phải lớn hơn 0.")
            .LessThanOrEqualTo(100_000_000_000m).WithMessage("Số tiền vượt quá giới hạn hệ thống cho phép.");

        RuleFor(x => x.SocialInsurance)
            .GreaterThanOrEqualTo(0).WithMessage("Khoản trích BHXH không thể âm.");

        RuleFor(x => x.HealthInsurance)
            .GreaterThanOrEqualTo(0).WithMessage("Khoản trích BHYT không thể âm.");

        RuleFor(x => x.UnemploymentInsurance)
            .GreaterThanOrEqualTo(0).WithMessage("Khoản trích BHTN không thể âm.");

        RuleFor(x => x.PersonalIncomeTax)
            .GreaterThanOrEqualTo(0).WithMessage("Thuế TNCN không thể âm.");

        RuleFor(x => x.EmployerName)
            .MaximumLength(200).WithMessage("Tên đơn vị chi trả tối đa 200 ký tự.");

        RuleFor(x => x.PayPeriod)
            .MaximumLength(50).WithMessage("Kỳ lương tối đa 50 ký tự.");
    }
}
