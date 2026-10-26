using ExpenseTracker.Maui.Services.Notifications;
using FluentAssertions;

namespace ExpenseTracker.Maui.UnitTests;

public class BankNotificationParserTests
{
    [Theory]
    [InlineData("com.acb.mobile", true)]
    [InlineData("com.mservice.momotransfer", true)]
    [InlineData("vn.com.vng.zalopay", true)]
    [InlineData("com.VCB", true)]
    [InlineData("com.mbmobile", true)]
    [InlineData("com.facebook.katana", false)]
    [InlineData("com.google.android.youtube", false)]
    public void IsSupportedPackage_ShouldIdentifyTargetFinancialPackages(string packageName, bool expected)
    {
        var result = BankNotificationParser.IsSupportedPackage(packageName);
        result.Should().Be(expected);
    }

    [Fact]
    public void Parse_WithMoMoPaymentNotification_ShouldExtractAmountAndMerchant()
    {
        // Arrange
        string pkg = "com.mservice.momotransfer";
        string title = "Giao dịch thành công";
        string text = "Thanh toán 85.000đ cho Cửa hàng Circle K Nguyễn Văn Cừ";

        // Act
        var result = BankNotificationParser.Parse(pkg, title, text);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Amount.Should().Be(85000m);
        result.BankName.Should().Be("Ví MoMo");
        result.ExtractedNote.Should().Contain("Circle K");
    }

    [Fact]
    public void Parse_WithAcbOneDeductionNotification_ShouldExtractAmountAndTransferNote()
    {
        // Arrange
        string pkg = "com.acb.mobile";
        string title = "Thông báo biến động số dư";
        string text = "TK 12345678: -350,000 VND vao 12:30. ND: Sieu thi Emart Go Vap";

        // Act
        var result = BankNotificationParser.Parse(pkg, title, text);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Amount.Should().Be(350000m);
        result.BankName.Should().Be("Ngân hàng ACB");
        result.ExtractedNote.Should().Contain("Emart");
    }

    [Fact]
    public void Parse_WithVietcombankDeductionNotification_ShouldExtractAmount()
    {
        // Arrange
        string pkg = "com.VCB";
        string title = "VCB Digibank";
        string text = "SD TK 007100... -500,000VND vao 15:45 tai Shopee Pay";

        // Act
        var result = BankNotificationParser.Parse(pkg, title, text);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Amount.Should().Be(500000m);
        result.BankName.Should().Be("Vietcombank");
        result.ExtractedNote.Should().Contain("Shopee");
    }

    [Fact]
    public void Parse_WithMbBankDeductionNotification_ShouldExtractAmount()
    {
        // Arrange
        string pkg = "com.mbmobile";
        string title = "Biến động số dư MB";
        string text = "TK 0123456789|GD: -120.000VND|SD: 5.430.000VND|ND: Highlands Coffee";

        // Act
        var result = BankNotificationParser.Parse(pkg, title, text);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Amount.Should().Be(120000m);
        result.BankName.Should().Be("MBBank");
        result.ExtractedNote.Should().Contain("Highlands Coffee");
    }

    [Fact]
    public void Parse_WithZaloPayNotification_ShouldExtractAmount()
    {
        // Arrange
        string pkg = "vn.com.vng.zalopay";
        string title = "Chuyển tiền thành công";
        string text = "Bạn đã chuyển 45.000đ cho GrabFood";

        // Act
        var result = BankNotificationParser.Parse(pkg, title, text);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Amount.Should().Be(45000m);
        result.BankName.Should().Be("Ví ZaloPay");
        result.ExtractedNote.Should().Contain("GrabFood");
    }

    [Theory]
    [InlineData("Bạn vừa nhận được 2.000.000 VND từ Nguyễn Văn A")]
    [InlineData("Tài khoản của bạn được cộng 500,000đ từ hoàn tiền mua sắm")]
    [InlineData("Ưu đãi voucher 50% khi thanh toán bằng thẻ tín dụng")]
    [InlineData("Chúc mừng bạn được tặng voucher ăn uống trị giá 100.000đ")]
    public void Parse_WithIncomeOrPromotionNotification_ShouldReturnFailed(string notificationContent)
    {
        // Arrange
        string pkg = "com.mservice.momotransfer";

        // Act
        var result = BankNotificationParser.Parse(pkg, "MoMo", notificationContent);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }
}
