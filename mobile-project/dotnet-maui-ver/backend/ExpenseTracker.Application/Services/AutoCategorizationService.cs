using System.Text.RegularExpressions;
using ExpenseTracker.Application.DTOs.AutoRules;
using ExpenseTracker.Application.Interfaces;
using ExpenseTracker.Domain.Common;
using ExpenseTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExpenseTracker.Application.Services;

// Implementation of auto-categorization engine evaluating keyword patterns and regex rules (UC-04.07)
public class AutoCategorizationService : IAutoCategorizationService
{
    private readonly IApplicationDbContext _context;

    public AutoCategorizationService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<List<AutoRuleDto>>> GetRulesAsync(Guid userId, CancellationToken ct = default)
    {
        var rules = await _context.AutoCategorizationRules
            .AsNoTracking()
            .Include(r => r.TargetCategory)
            .Where(r => r.UserId == userId)
            .OrderBy(r => r.PriorityOrder)
            .ThenBy(r => r.KeywordPattern)
            .Select(r => new AutoRuleDto(
                r.Id,
                r.UserId,
                r.KeywordPattern,
                r.TargetCategoryId,
                r.TargetCategory.Name,
                r.PriorityOrder,
                r.IsRegex,
                r.IsCaseSensitive,
                r.CreatedAt
            ))
            .ToListAsync(ct);

        return Result<List<AutoRuleDto>>.Success(rules);
    }

    public async Task<Result<AutoRuleDto>> GetRuleByIdAsync(Guid userId, Guid ruleId, CancellationToken ct = default)
    {
        var rule = await _context.AutoCategorizationRules
            .AsNoTracking()
            .Include(r => r.TargetCategory)
            .FirstOrDefaultAsync(r => r.Id == ruleId && r.UserId == userId, ct);

        if (rule == null)
            return Result<AutoRuleDto>.Failure("Quy tắc không tồn tại hoặc bạn không có quyền truy cập.", 404);

        var dto = new AutoRuleDto(
            rule.Id,
            rule.UserId,
            rule.KeywordPattern,
            rule.TargetCategoryId,
            rule.TargetCategory.Name,
            rule.PriorityOrder,
            rule.IsRegex,
            rule.IsCaseSensitive,
            rule.CreatedAt
        );

        return Result<AutoRuleDto>.Success(dto);
    }

    public async Task<Result<AutoRuleDto>> CreateRuleAsync(Guid userId, CreateAutoRuleRequest request, CancellationToken ct = default)
    {
        var targetCategory = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.TargetCategoryId && (c.IsSystem || c.UserId == userId), ct);

        if (targetCategory == null)
            return Result<AutoRuleDto>.Failure("Danh mục đích không tồn tại hoặc không hợp lệ.");

        var trimmedPattern = request.KeywordPattern.Trim();
        var isDuplicate = await _context.AutoCategorizationRules
            .AnyAsync(r => r.UserId == userId 
                        && r.TargetCategoryId == request.TargetCategoryId
                        && r.KeywordPattern.ToLower() == trimmedPattern.ToLower(), ct);

        if (isDuplicate)
            return Result<AutoRuleDto>.Failure("Quy tắc cho từ khóa này đã được thiết lập trước đó.");

        if (request.IsRegex)
        {
            try
            {
                _ = new Regex(trimmedPattern);
            }
            catch (ArgumentException)
            {
                return Result<AutoRuleDto>.Failure("Biểu thức chính quy (Regex) không hợp lệ.");
            }
        }

        var rule = new AutoCategorizationRule(
            userId: userId,
            keywordPattern: trimmedPattern,
            targetCategoryId: targetCategory.Id,
            priorityOrder: request.PriorityOrder,
            isRegex: request.IsRegex,
            isCaseSensitive: request.IsCaseSensitive
        );

        await _context.AutoCategorizationRules.AddAsync(rule, ct);
        await _context.SaveChangesAsync(ct);

        var dto = new AutoRuleDto(
            rule.Id,
            rule.UserId,
            rule.KeywordPattern,
            rule.TargetCategoryId,
            targetCategory.Name,
            rule.PriorityOrder,
            rule.IsRegex,
            rule.IsCaseSensitive,
            rule.CreatedAt
        );

        return Result<AutoRuleDto>.Success(dto);
    }

    public async Task<Result<AutoRuleDto>> UpdateRuleAsync(Guid userId, Guid ruleId, UpdateAutoRuleRequest request, CancellationToken ct = default)
    {
        var rule = await _context.AutoCategorizationRules
            .Include(r => r.TargetCategory)
            .FirstOrDefaultAsync(r => r.Id == ruleId && r.UserId == userId, ct);

        if (rule == null)
            return Result<AutoRuleDto>.Failure("Quy tắc không tồn tại.", 404);

        var targetCategory = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.TargetCategoryId && (c.IsSystem || c.UserId == userId), ct);

        if (targetCategory == null)
            return Result<AutoRuleDto>.Failure("Danh mục đích không tồn tại.");

        var trimmedPattern = request.KeywordPattern.Trim();
        var isDuplicate = await _context.AutoCategorizationRules
            .AnyAsync(r => r.Id != ruleId
                        && r.UserId == userId
                        && r.TargetCategoryId == request.TargetCategoryId
                        && r.KeywordPattern.ToLower() == trimmedPattern.ToLower(), ct);

        if (isDuplicate)
            return Result<AutoRuleDto>.Failure("Quy tắc cho từ khóa này đã được thiết lập trước đó.");

        if (request.IsRegex)
        {
            try
            {
                _ = new Regex(trimmedPattern);
            }
            catch (ArgumentException)
            {
                return Result<AutoRuleDto>.Failure("Biểu thức chính quy (Regex) không hợp lệ.");
            }
        }

        rule.UpdateDetails(
            keywordPattern: trimmedPattern,
            targetCategoryId: request.TargetCategoryId,
            priorityOrder: request.PriorityOrder,
            isRegex: request.IsRegex,
            isCaseSensitive: request.IsCaseSensitive
        );

        await _context.SaveChangesAsync(ct);

        var dto = new AutoRuleDto(
            rule.Id,
            rule.UserId,
            rule.KeywordPattern,
            rule.TargetCategoryId,
            targetCategory.Name,
            rule.PriorityOrder,
            rule.IsRegex,
            rule.IsCaseSensitive,
            rule.CreatedAt
        );

        return Result<AutoRuleDto>.Success(dto);
    }

    public async Task<Result<bool>> DeleteRuleAsync(Guid userId, Guid ruleId, CancellationToken ct = default)
    {
        var rule = await _context.AutoCategorizationRules
            .FirstOrDefaultAsync(r => r.Id == ruleId && r.UserId == userId, ct);

        if (rule == null)
            return Result<bool>.Failure("Quy tắc không tồn tại.", 404);

        rule.MarkAsDeleted();
        await _context.SaveChangesAsync(ct);

        return Result<bool>.Success(true);
    }

    // Evaluates text against active user rules ordered by PriorityOrder
    public async Task<Result<MatchCategoryResponse>> MatchCategoryAsync(Guid userId, string text, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Result<MatchCategoryResponse>.Success(new MatchCategoryResponse(null, null, null));

        var rules = await _context.AutoCategorizationRules
            .AsNoTracking()
            .Include(r => r.TargetCategory)
            .Where(r => r.UserId == userId)
            .OrderBy(r => r.PriorityOrder)
            .ThenBy(r => r.KeywordPattern)
            .ToListAsync(ct);

        var cleanText = text.Trim();

        foreach (var rule in rules)
        {
            bool isMatch = false;

            if (rule.IsRegex)
            {
                try
                {
                    var options = rule.IsCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
                    isMatch = Regex.IsMatch(cleanText, rule.KeywordPattern, options);
                }
                catch
                {
                    // Invalid regex skipped safely
                    isMatch = false;
                }
            }
            else
            {
                var comparison = rule.IsCaseSensitive 
                    ? StringComparison.Ordinal 
                    : StringComparison.OrdinalIgnoreCase;

                isMatch = cleanText.Contains(rule.KeywordPattern, comparison);
            }

            if (isMatch)
            {
                return Result<MatchCategoryResponse>.Success(new MatchCategoryResponse(
                    rule.TargetCategoryId,
                    rule.TargetCategory?.Name,
                    rule.KeywordPattern
                ));
            }
        }

        return Result<MatchCategoryResponse>.Success(new MatchCategoryResponse(null, null, null));
    }
}
