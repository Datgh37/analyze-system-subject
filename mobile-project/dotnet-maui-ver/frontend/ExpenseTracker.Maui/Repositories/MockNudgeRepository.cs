using System.Text.Json;
using ExpenseTracker.Maui.Models.Behavioral;

namespace ExpenseTracker.Maui.Repositories;

public class MockNudgeRepository : INudgeRepository
{
    private readonly List<FinancialNudgeModel> _nudges = new();

    public MockNudgeRepository()
    {
        SeedMockData();
    }

    private void SeedMockData()
    {
        var xaiDsa = new XaiRationaleModel
        {
            NudgeType = NudgeType.DsaThreshold,
            Title = "Cảnh Báo Chạm Trần Hạn Mức Ngày (DSA)",
            TriggerContext = "Khoản chi ăn tối 185,000 đ vừa ghi nhận đưa tổng chi hôm nay lên 285,000 đ.",
            MathematicalFormula = "DSA_t = Ngân_sách_còn_lại / Số_ngày_còn_lại. Ngưỡng cảnh báo: Thực_chi >= 85% DSA_t.",
            EmpiricalEvidence = "Bạn đã sử dụng 95.0% hạn mức an toàn của ngày hôm nay (300,000 đ).",
            ProjectedConsequences = "Nếu tiếp tục chi tiêu thêm tối nay, DSA các ngày tới sẽ giảm xuống dưới 250,000 đ/ngày.",
            ProjectedDailyDsaAfter = 245000m
        };

        var proposal = new DynamicRebalanceProposalModel
        {
            ProposalId = Guid.NewGuid(),
            SourceCategoryId = Guid.NewGuid(),
            SourceCategoryName = "Giải trí & Mua sắm",
            TargetCategoryId = Guid.NewGuid(),
            TargetCategoryName = "Ăn uống hàng ngày",
            TransferAmount = 450000m,
            SourceCategoryOldLimit = 2000000m,
            SourceCategoryNewLimit = 1550000m,
            TargetCategoryOldLimit = 3500000m,
            TargetCategoryNewLimit = 3950000m
        };

        _nudges.Add(new FinancialNudgeModel
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            UserId = Guid.NewGuid(),
            NudgeType = NudgeType.DsaThreshold,
            Title = "Cảnh Báo Chạm Trần Hạn Mức Ngày (DSA)",
            Message = "Bạn đã sử dụng 95% hạn mức an toàn hôm nay (300,000 đ). Hãy tạm hoãn chi tiêu giải trí tối nay!",
            TriggerContext = "Chi tiêu ăn uống tối nay",
            Status = NudgeStatus.Active,
            RationaleExplanationJson = JsonSerializer.Serialize(xaiDsa),
            CreatedAt = DateTime.UtcNow.AddMinutes(-35)
        });

        _nudges.Add(new FinancialNudgeModel
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            UserId = Guid.NewGuid(),
            NudgeType = NudgeType.DeficitRebalance,
            Title = "Đề Xuất Tái Cân Đối Ngân Sách Ăn Uống",
            Message = "Danh mục Ăn uống đã thâm hụt 450,000 đ. Hệ thống gợi ý chuyển 450,000 đ từ quỹ Mua sắm còn dư để bù đắp.",
            TriggerContext = "Thâm hụt danh mục Ăn uống",
            Status = NudgeStatus.Active,
            RebalanceProposalJson = JsonSerializer.Serialize(proposal),
            CreatedAt = DateTime.UtcNow.AddHours(-2)
        });

        _nudges.Add(new FinancialNudgeModel
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            UserId = Guid.NewGuid(),
            NudgeType = NudgeType.PayYourselfFirst,
            Title = "Chi Trả Cho Bản Thân Trước (Pay Yourself First)",
            Message = "Bạn vừa nhận thu nhập lương 18,000,000 đ! Hãy trích ngay 20% (3,600,000 đ) vào Mục tiêu Quỹ Khẩn Cấp trước khi chi tiêu sinh hoạt.",
            TriggerContext = "Giao dịch nhận lương định kỳ",
            Status = NudgeStatus.Active,
            CreatedAt = DateTime.UtcNow.AddHours(-5)
        });
    }

    public Task<List<FinancialNudgeModel>> GetActiveNudgesAsync()
    {
        return Task.FromResult(_nudges.Where(n => n.Status == NudgeStatus.Active).ToList());
    }

    public Task<XaiRationaleModel> GetXaiRationaleAsync(Guid id)
    {
        var nudge = _nudges.FirstOrDefault(n => n.Id == id);
        if (nudge != null && !string.IsNullOrEmpty(nudge.RationaleExplanationJson))
        {
            var parsed = JsonSerializer.Deserialize<XaiRationaleModel>(nudge.RationaleExplanationJson);
            if (parsed != null) return Task.FromResult(parsed);
        }

        return Task.FromResult(new XaiRationaleModel
        {
            Title = "Giải thích thuật toán XAI",
            MathematicalFormula = "DSA_t = Ngân_sách_còn_lại / Số_ngày_còn_lại",
            EmpiricalEvidence = "Dựa trên lịch sử chi tiêu trung bình 7 ngày gần nhất",
            ProjectedConsequences = "Giúp bảo toàn kỷ luật tài chính cá nhân không âm ví",
            ProjectedDailyDsaAfter = 250000m
        });
    }

    public Task<DynamicRebalanceProposalModel> GenerateRebalanceProposalAsync(Guid deficitCategoryId)
    {
        return Task.FromResult(new DynamicRebalanceProposalModel
        {
            ProposalId = Guid.NewGuid(),
            SourceCategoryId = Guid.NewGuid(),
            SourceCategoryName = "Giải trí & Mua sắm",
            TargetCategoryId = deficitCategoryId,
            TargetCategoryName = "Danh mục thâm hụt",
            TransferAmount = 500000m,
            SourceCategoryOldLimit = 2000000m,
            SourceCategoryNewLimit = 1500000m,
            TargetCategoryOldLimit = 3000000m,
            TargetCategoryNewLimit = 3500000m
        });
    }

    public Task<bool> ApproveNudgeAsync(Guid id)
    {
        var nudge = _nudges.FirstOrDefault(n => n.Id == id);
        if (nudge != null)
        {
            nudge.Status = NudgeStatus.Resolved;
            nudge.IsApprovedByUser = true;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }

    public Task<bool> DismissNudgeAsync(Guid id)
    {
        var nudge = _nudges.FirstOrDefault(n => n.Id == id);
        if (nudge != null)
        {
            nudge.Status = NudgeStatus.Dismissed;
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}
