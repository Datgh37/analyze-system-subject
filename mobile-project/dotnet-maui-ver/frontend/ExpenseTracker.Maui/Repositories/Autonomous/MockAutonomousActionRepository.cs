using ExpenseTracker.Maui.Models.Autonomous;

namespace ExpenseTracker.Maui.Repositories.Autonomous;

// In-memory mock repository facilitating offline UI preview and styling for frontend development
public class MockAutonomousActionRepository : IAutonomousActionRepository
{
    private readonly List<AutonomousActionModel> _mockInbox = new();

    public MockAutonomousActionRepository()
    {
        SeedDefaultMockActions();
    }

    public Task<AutonomousActionInboxResponseModel> GetInboxAsync()
    {
        var response = new AutonomousActionInboxResponseModel(
            TotalCount: _mockInbox.Count,
            Actions: _mockInbox.ToList()
        );
        return Task.FromResult(response);
    }

    public Task<AutonomousActionModel> ProcessNotificationAsync(RawBankNotificationRequestModel request)
    {
        decimal amount = request.DetectedAmount ?? 50000m;
        string actionId = $"act_mock_{Guid.NewGuid():N}";

        var newAction = new AutonomousActionModel(
            ActionId: actionId,
            ActionType: AutonomousActionTypeConstants.QuickTransactionConfirm,
            ConfidenceScore: 0.95,
            Display: new ActionDisplayModel(
                Title: $"Giao dịch: {amount:N0} đ",
                Subtitle: $"{request.Title} • Vừa xong",
                BadgeText: "Ăn uống",
                BadgeColor: "#10B981",
                Amount: amount
            ),
            PrimaryAction: new ActionCommandModel(
                Label: "Lưu nhanh",
                ActionKind: ActionKindConstants.QuickCommit,
                Endpoint: "/api/actions/quick-commit",
                Payload: new Dictionary<string, string>
                {
                    ["actionId"] = actionId,
                    ["amount"] = amount.ToString("G29"),
                    ["categoryName"] = "Ăn uống"
                }
            ),
            SecondaryAction: new ActionCommandModel(
                Label: "Đổi",
                ActionKind: ActionKindConstants.Navigate,
                Route: "AddTransactionPage"
            ),
            Choices: null,
            CreatedAt: DateTime.UtcNow
        );

        _mockInbox.Insert(0, newAction);
        return Task.FromResult(newAction);
    }

    public Task<AutonomousActionModel> QuickCommitAsync(QuickCommitActionRequestModel request)
    {
        var action = _mockInbox.FirstOrDefault(a => a.ActionId == request.ActionId);
        if (action != null)
        {
            _mockInbox.Remove(action);
        }

        var committed = (action != null)
            ? action with
            {
                ActionType = AutonomousActionTypeConstants.Committed,
                Display = action.Display with { Subtitle = "Đã lưu vào ví • Vừa xong" },
                PrimaryAction = null,
                SecondaryAction = null,
                Choices = null
            }
            : new AutonomousActionModel(
                ActionId: request.ActionId,
                ActionType: AutonomousActionTypeConstants.Committed,
                ConfidenceScore: 1.0,
                Display: new ActionDisplayModel("Đã lưu chi tiêu", "Đã lưu vào ví • Vừa xong", "Đã lưu", "#10B981", request.Amount ?? 0m)
            );

        return Task.FromResult(committed);
    }

    public Task<bool> DismissActionAsync(string actionId)
    {
        var item = _mockInbox.FirstOrDefault(a => a.ActionId == actionId);
        if (item != null)
        {
            _mockInbox.Remove(item);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    private void SeedDefaultMockActions()
    {
        // 1. High Confidence: Shopee 185,000 đ with 1-tap Quick Commit
        _mockInbox.Add(new AutonomousActionModel(
            ActionId: "mock_act_shopee_01",
            ActionType: AutonomousActionTypeConstants.QuickTransactionConfirm,
            ConfidenceScore: 0.92,
            Display: new ActionDisplayModel(
                Title: "Shopee: 185.000 đ",
                Subtitle: "ACB • 23:15",
                BadgeText: "Mua sắm",
                BadgeColor: "#F59E0B",
                Amount: 185000m
            ),
            PrimaryAction: new ActionCommandModel(
                Label: "Lưu nhanh",
                ActionKind: ActionKindConstants.QuickCommit,
                Endpoint: "/api/actions/quick-commit",
                Payload: new Dictionary<string, string>
                {
                    ["actionId"] = "mock_act_shopee_01",
                    ["amount"] = "185000",
                    ["categoryName"] = "Mua sắm",
                    ["walletName"] = "ACB"
                }
            ),
            SecondaryAction: new ActionCommandModel(
                Label: "Đổi",
                ActionKind: ActionKindConstants.Navigate,
                Route: "AddTransactionPage?amount=185000&category=Mua%20s%E1%BA%AFm&source=ACB"
            ),
            Choices: null,
            CreatedAt: DateTime.UtcNow.AddMinutes(-5)
        ));

        // 2. Medium Confidence (Jev Choice): Circle K 35,000 đ with peer options
        _mockInbox.Add(new AutonomousActionModel(
            ActionId: "mock_act_circlek_02",
            ActionType: AutonomousActionTypeConstants.ChoiceDisambiguation,
            ConfidenceScore: 0.65,
            Display: new ActionDisplayModel(
                Title: "Circle K: 35.000 đ",
                Subtitle: "MoMo • 14:20",
                BadgeText: "Cửa hàng tiện lợi",
                BadgeColor: "#3B82F6",
                Amount: 35000m
            ),
            PrimaryAction: null,
            SecondaryAction: new ActionCommandModel(
                Label: "Sửa chi tiết",
                ActionKind: ActionKindConstants.Navigate,
                Route: "AddTransactionPage?amount=35000&source=MoMo"
            ),
            Choices: new List<ActionChoiceModel>
            {
                new("c1", "Ăn uống", 0.60, "60%", new Dictionary<string, string> { ["categoryName"] = "Ăn uống" }),
                new("c2", "Vật dụng sinh hoạt", 0.40, "40%", new Dictionary<string, string> { ["categoryName"] = "Vật dụng sinh hoạt" })
            },
            CreatedAt: DateTime.UtcNow.AddHours(-1)
        ));
    }
}
