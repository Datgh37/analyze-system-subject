using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ExpenseTracker.Maui.Models.SavingsGoals;
using ExpenseTracker.Maui.Models.Wallets;
using ExpenseTracker.Maui.Services.Api;

namespace ExpenseTracker.Maui.ViewModels;

// ViewModel managing savings goals, Kakeibo jars widget, deposits, Thaler nudge withdrawals, and feasibility checks (UC-06.01 - 06.05)
public partial class SavingsGoalsViewModel : BaseViewModel
{
    private readonly ISavingsGoalApi _savingsGoalApi;
    private readonly IWalletApi _walletApi;

    public ObservableCollection<SavingsGoalSummaryModel> Goals { get; } = new();
    public ObservableCollection<JarItemModel> Jars { get; } = new();

    [ObservableProperty]
    private JarsSummaryModel? _jarsSummary;

    [ObservableProperty]
    private decimal _totalSavingsBalance;

    [ObservableProperty]
    private int _activeGoalsCount;

    [ObservableProperty]
    private int _completedGoalsCount;

    [ObservableProperty]
    private string? _statusMessage;

    public SavingsGoalsViewModel(ISavingsGoalApi savingsGoalApi, IWalletApi walletApi)
    {
        _savingsGoalApi = savingsGoalApi;
        _walletApi = walletApi;
        Title = "Mục Tiêu Tích Lũy & Hũ Tài Chính";
    }

    [RelayCommand]
    public async Task LoadGoalsAndJarsAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ErrorMessage = null;
        StatusMessage = null;

        try
        {
            var goalsTask = _savingsGoalApi.GetSavingsGoalsAsync();
            var jarsTask = _savingsGoalApi.GetJarsSummaryAsync();

            await Task.WhenAll(goalsTask, jarsTask);

            var goalList = await goalsTask;
            JarsSummary = await jarsTask;

            TotalSavingsBalance = JarsSummary?.TotalSavingsBalance ?? 0m;
            ActiveGoalsCount = goalList.Count(g => g.Status == SavingsGoalStatus.Active);
            CompletedGoalsCount = goalList.Count(g => g.Status == SavingsGoalStatus.Completed);

            Goals.Clear();
            foreach (var goal in goalList)
            {
                Goals.Add(goal);
            }

            Jars.Clear();
            if (JarsSummary?.Jars != null)
            {
                foreach (var jar in JarsSummary.Jars)
                {
                    Jars.Add(jar);
                }
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi khi tải dữ liệu mục tiêu tích lũy: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task DepositToGoalAsync(SavingsGoalSummaryModel? goal)
    {
        if (goal == null) return;

        try
        {
            var wallets = await _walletApi.GetWalletsAsync();
            if (wallets == null || wallets.Count == 0)
            {
                await Shell.Current.DisplayAlertAsync("Thông báo", "Bạn chưa có ví tiền nào để nạp vào mục tiêu. Vui lòng tạo ví trước.", "OK");
                return;
            }

            var activeWallets = wallets.Where(w => w.Status == 1).ToList();
            var walletNames = activeWallets.Select(w => $"{w.Name} ({w.Balance:N0} đ)").ToArray();

            string selectedWalletName = await Shell.Current.DisplayActionSheetAsync("Chọn ví trích tiền", "Hủy", null, walletNames);
            if (string.IsNullOrWhiteSpace(selectedWalletName) || selectedWalletName == "Hủy") return;

            var selectedWallet = activeWallets.FirstOrDefault(w => selectedWalletName.StartsWith(w.Name));
            if (selectedWallet == null) return;

            string amountStr = await Shell.Current.DisplayPromptAsync(
                "Nạp tiền tích lũy",
                $"Nhập số tiền muốn nạp vào '{goal.Name}' (Số dư ví: {selectedWallet.Balance:N0} đ):",
                "Nạp tiền", "Hủy", "Ví dụ: 1000000", keyboard: Keyboard.Numeric);

            if (string.IsNullOrWhiteSpace(amountStr)) return;

            if (!decimal.TryParse(amountStr.Replace(".", "").Replace(",", ""), out decimal amount) || amount <= 0)
            {
                await Shell.Current.DisplayAlertAsync("Lỗi nhập liệu", "Số tiền nạp không hợp lệ.", "OK");
                return;
            }

            if (amount > selectedWallet.Balance)
            {
                await Shell.Current.DisplayAlertAsync("Không đủ số dư", $"Ví {selectedWallet.Name} chỉ còn {selectedWallet.Balance:N0} đ, không đủ nạp {amount:N0} đ.", "OK");
                return;
            }

            IsBusy = true;
            var request = new DepositSavingsGoalModel
            {
                WalletId = selectedWallet.Id,
                Amount = amount,
                Note = "Nạp tiền tích lũy từ ứng dụng di động"
            };

            await _savingsGoalApi.DepositAsync(goal.Id, request);
            StatusMessage = $"✅ Đã nạp thành công {amount:N0} đ vào mục tiêu '{goal.Name}'.";
            await LoadGoalsAndJarsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi nạp tiền tích lũy: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task WithdrawFromGoalAsync(SavingsGoalSummaryModel? goal)
    {
        if (goal == null) return;

        if (goal.CurrentAmount <= 0)
        {
            await Shell.Current.DisplayAlertAsync("Thông báo", "Mục tiêu này hiện không có số dư để rút.", "OK");
            return;
        }

        try
        {
            // Richard Thaler Cognitive Friction Nudge Alert (UC-06.03)
            bool proceed = await Shell.Current.DisplayAlertAsync(
                "⚠️ Cú Hích Tâm Lý (Thaler Nudge)",
                $"Bạn đang yêu cầu rút tiền khỏi quỹ '{goal.Name}'.\n\n" +
                $"Việc rút tiền sớm sẽ làm đứt gãy kỷ luật tích lũy và kéo dài thời gian hoàn thành mục tiêu. " +
                $"Bạn có chắc chắn đây là nhu cầu thực sự cấp bách không?",
                "Tôi hiểu, tiếp tục rút", "Hủy rút tiền");

            if (!proceed) return;

            var wallets = await _walletApi.GetWalletsAsync();
            var activeWallets = wallets.Where(w => w.Status == 1).ToList();
            var walletNames = activeWallets.Select(w => w.Name).ToArray();

            string selectedWalletName = await Shell.Current.DisplayActionSheetAsync("Chọn ví nhận tiền rút", "Hủy", null, walletNames);
            if (string.IsNullOrWhiteSpace(selectedWalletName) || selectedWalletName == "Hủy") return;

            var selectedWallet = activeWallets.FirstOrDefault(w => w.Name == selectedWalletName);
            if (selectedWallet == null) return;

            string amountStr = await Shell.Current.DisplayPromptAsync(
                "Số tiền muốn rút",
                $"Nhập số tiền rút (Tối đa: {goal.CurrentAmount:N0} đ):",
                "Tiếp tục", "Hủy", "Ví dụ: 500000", keyboard: Keyboard.Numeric);

            if (string.IsNullOrWhiteSpace(amountStr)) return;

            if (!decimal.TryParse(amountStr.Replace(".", "").Replace(",", ""), out decimal amount) || amount <= 0)
            {
                await Shell.Current.DisplayAlertAsync("Lỗi nhập liệu", "Số tiền rút không hợp lệ.", "OK");
                return;
            }

            if (amount > goal.CurrentAmount)
            {
                await Shell.Current.DisplayAlertAsync("Vượt số dư quỹ", $"Quỹ chỉ có {goal.CurrentAmount:N0} đ, không thể rút {amount:N0} đ.", "OK");
                return;
            }

            // Cognitive Friction step 2: Mandatory reason input
            string reason = await Shell.Current.DisplayPromptAsync(
                "Lý do rút tiền bắt buộc",
                "Vui lòng ghi rõ lý do để ghi nhận vào sổ nhật ký tài chính:",
                "Xác nhận rút", "Hủy", "Ví dụ: Chi phí sửa xe bất ngờ");

            if (string.IsNullOrWhiteSpace(reason))
            {
                await Shell.Current.DisplayAlertAsync("Yêu cầu nhập lý do", "Bạn bắt buộc phải nêu rõ lý do để hoàn tất thao tác rút tiền.", "OK");
                return;
            }

            IsBusy = true;
            var request = new WithdrawSavingsGoalModel
            {
                TargetWalletId = selectedWallet.Id,
                Amount = amount,
                Reason = reason.Trim()
            };

            await _savingsGoalApi.WithdrawAsync(goal.Id, request);
            StatusMessage = $"✅ Đã rút {amount:N0} đ về ví {selectedWallet.Name}.";
            await LoadGoalsAndJarsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi rút tiền quỹ tích lũy: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task CheckFeasibilityAsync(SavingsGoalSummaryModel? goal)
    {
        if (goal == null) return;

        IsBusy = true;
        try
        {
            var feasibility = await _savingsGoalApi.GetGoalFeasibilityAsync(goal.Id);
            string statusIcon = feasibility.IsFeasible ? "🟢 Khả thi cao" : "🔴 Cần điều chỉnh";

            await Shell.Current.DisplayAlertAsync(
                $"Đánh giá tính khả thi: {feasibility.GoalName}",
                $"{statusIcon} (Điểm: {feasibility.FeasibilityScorePercent:F0}%)\n\n" +
                $"• Cần nạp mỗi tháng: {feasibility.FormattedMonthlyRequired}\n" +
                $"• Quỹ tích lũy ngân sách (20%): {feasibility.FormattedSavingsBudgetPool}\n\n" +
                $"💡 Lời khuyên:\n{feasibility.AdviceMessage}",
                "Đóng");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi đánh giá tính khả thi: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task DeleteGoalAsync(SavingsGoalSummaryModel? goal)
    {
        if (goal == null) return;

        if (goal.CurrentAmount > 0)
        {
            await Shell.Current.DisplayAlertAsync(
                "Không thể xóa mục tiêu",
                $"Mục tiêu '{goal.Name}' vẫn còn số dư {goal.CurrentAmount:N0} đ. Vui lòng rút hết tiền về ví trước khi xóa.",
                "OK");
            return;
        }

        bool confirm = await Shell.Current.DisplayAlertAsync(
            "Xác nhận xóa",
            $"Bạn có chắc chắn muốn xóa mục tiêu tích lũy '{goal.Name}' không?",
            "Xóa", "Hủy");

        if (!confirm) return;

        IsBusy = true;
        try
        {
            await _savingsGoalApi.DeleteSavingsGoalAsync(goal.Id);
            StatusMessage = $"✅ Đã xóa mục tiêu '{goal.Name}'.";
            await LoadGoalsAndJarsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Lỗi khi xóa mục tiêu: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    public async Task NavigateToCreateGoalAsync()
    {
        await Shell.Current.GoToAsync("CreateSavingsGoalPage");
    }
}
