using ExpenseTracker.Maui.Models.Receipts;
using ExpenseTracker.Maui.Models.Transactions;
using Refit;

namespace ExpenseTracker.Maui.Repositories;

// In-memory mock repository for AI Vision receipt parsing and confirmation
public class MockReceiptRepository : IReceiptRepository
{
    private readonly ITransactionRepository _transactionRepo;

    public MockReceiptRepository(ITransactionRepository transactionRepo)
    {
        _transactionRepo = transactionRepo;
    }

    public Task<ReceiptParsedDataModel> ParseReceiptAsync(StreamPart file)
    {
        var items = new List<ParsedReceiptItemModel>
        {
            new()
            {
                ItemName = "Sữa tươi tiệt trùng Vinamilk 1L",
                Quantity = 2,
                UnitPrice = 38_000m,
                TotalPrice = 76_000m,
                SuggestedCategoryId = Guid.Parse("33333333-3333-3333-3333-333333333301"),
                SuggestedCategoryName = "Ăn uống & Thực phẩm",
                Confidence = 0.98
            },
            new()
            {
                ItemName = "Bánh mì sandwich bơ Pháp",
                Quantity = 1,
                UnitPrice = 25_000m,
                TotalPrice = 25_000m,
                SuggestedCategoryId = Guid.Parse("33333333-3333-3333-3333-333333333301"),
                SuggestedCategoryName = "Ăn uống & Thực phẩm",
                Confidence = 0.95
            },
            new()
            {
                ItemName = "Nước giặt OMO Matic túi 3.6kg",
                Quantity = 1,
                UnitPrice = 185_000m,
                TotalPrice = 185_000m,
                SuggestedCategoryId = Guid.Parse("33333333-3333-3333-3333-333333333303"),
                SuggestedCategoryName = "Nhà cửa & Đời sống",
                Confidence = 0.97
            }
        };

        decimal subTotal = items.Sum(i => i.TotalPrice);
        decimal tax = 28_600m;
        decimal discount = 15_000m;
        decimal total = subTotal + tax - discount;

        var result = new ReceiptParsedDataModel(
            MerchantName: "Siêu thị WinMart+ Thảo Điền",
            InvoiceDate: DateTime.Now,
            Items: items,
            SubTotal: subTotal,
            TaxAmount: tax,
            DiscountAmount: discount,
            TotalAmount: total,
            CalculatedTotal: total,
            DiscrepancyAmount: 0m,
            IsMathDiscrepancy: false,
            AuditWarningMessage: null,
            ConfidenceScore: 0.97
        );

        return Task.FromResult(result);
    }

    public async Task<TransactionModel> ConfirmReceiptAsync(ConfirmReceiptModel request)
    {
        var createTx = new CreateExpenseModel(
            WalletId: request.WalletId,
            CategoryId: request.CategoryId,
            Amount: request.Amount,
            TransactionDate: request.TransactionDate,
            Note: $"[WinMart+] {request.Note ?? "Mua sắm bách hóa"}",
            IsEssential: request.IsEssential
        );

        return await _transactionRepo.CreateExpenseAsync(createTx);
    }
}
