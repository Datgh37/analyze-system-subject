using ExpenseTracker.Application.DTOs.Debts;
using FluentValidation;

namespace ExpenseTracker.Application.Validators;

public class CreateDebtDtoValidator : AbstractValidator<CreateDebtDto>
{
    public CreateDebtDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên khoản nợ không được để trống.")
            .MaximumLength(200).WithMessage("Tên khoản nợ tối đa 200 ký tự.");

        RuleFor(x => x.CreditorName)
            .NotEmpty().WithMessage("Tên bên cho vay / chủ nợ không được để trống.")
            .MaximumLength(200).WithMessage("Tên bên cho vay tối đa 200 ký tự.");

        RuleFor(x => x.PrincipalAmount)
            .GreaterThan(0).WithMessage("Số tiền gốc ban đầu phải lớn hơn 0.")
            .LessThanOrEqualTo(100_000_000_000m).WithMessage("Số tiền nợ vượt quá giới hạn hệ thống cho phép.");

        RuleFor(x => x.InterestRateAnnual)
            .GreaterThanOrEqualTo(0).WithMessage("Lãi suất năm không được âm.")
            .LessThanOrEqualTo(100).WithMessage("Lãi suất năm không được vượt quá 100%/năm.");

        RuleFor(x => x.MinimumPayment)
            .GreaterThanOrEqualTo(0).WithMessage("Khoản trả tối thiểu không được âm.");

        RuleFor(x => x.DueDateMonthly)
            .InclusiveBetween(1, 31).WithMessage("Ngày đến hạn hàng tháng phải nằm trong khoảng từ 1 đến 31.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Ngày bắt đầu vay không được để trống.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Ghi chú tối đa 1000 ký tự.");
    }
}

public class UpdateDebtDtoValidator : AbstractValidator<UpdateDebtDto>
{
    public UpdateDebtDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Tên khoản nợ không được để trống.")
            .MaximumLength(200).WithMessage("Tên khoản nợ tối đa 200 ký tự.");

        RuleFor(x => x.CreditorName)
            .NotEmpty().WithMessage("Tên bên cho vay / chủ nợ không được để trống.")
            .MaximumLength(200).WithMessage("Tên bên cho vay tối đa 200 ký tự.");

        RuleFor(x => x.InterestRateAnnual)
            .GreaterThanOrEqualTo(0).WithMessage("Lãi suất năm không được âm.")
            .LessThanOrEqualTo(100).WithMessage("Lãi suất năm không được vượt quá 100%/năm.");

        RuleFor(x => x.MinimumPayment)
            .GreaterThanOrEqualTo(0).WithMessage("Khoản trả tối thiểu không được âm.");

        RuleFor(x => x.DueDateMonthly)
            .InclusiveBetween(1, 31).WithMessage("Ngày đến hạn hàng tháng phải nằm trong khoảng từ 1 đến 31.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Ghi chú tối đa 1000 ký tự.");
    }
}

public class RecordDebtPaymentDtoValidator : AbstractValidator<RecordDebtPaymentDto>
{
    public RecordDebtPaymentDtoValidator()
    {
        RuleFor(x => x.WalletId)
            .NotEmpty().WithMessage("Vui lòng chọn ví nguồn để thanh toán nợ.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Số tiền thanh toán phải lớn hơn 0.");

        RuleFor(x => x.PrincipalPaid)
            .GreaterThanOrEqualTo(0).WithMessage("Tiền gốc trả không được âm.");

        RuleFor(x => x.InterestPaid)
            .GreaterThanOrEqualTo(0).WithMessage("Tiền lãi trả không được âm.");

        RuleFor(x => x)
            .Must(x => x.PrincipalPaid + x.InterestPaid == x.Amount)
            .WithMessage("Tổng tiền gốc trả và tiền lãi trả phải bằng chính xác tổng số tiền thanh toán.");

        RuleFor(x => x.PaymentDate)
            .NotEmpty().WithMessage("Ngày thanh toán không được để trống.")
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1)).WithMessage("Ngày thanh toán không được vượt quá 1 ngày trong tương lai.");

        RuleFor(x => x.Note)
            .MaximumLength(500).WithMessage("Ghi chú tối đa 500 ký tự.");
    }
}

