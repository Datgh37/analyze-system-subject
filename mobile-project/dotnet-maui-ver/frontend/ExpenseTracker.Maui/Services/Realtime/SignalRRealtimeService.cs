using CommunityToolkit.Mvvm.Messaging;
using ExpenseTracker.Maui.Models.Notifications;
using ExpenseTracker.Maui.Services.Realtime.Messages;
using ExpenseTracker.Maui.Services.Security;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;

namespace ExpenseTracker.Maui.Services.Realtime;

public class SignalRRealtimeService : ISignalRRealtimeService, IAsyncDisposable
{
    private readonly ITokenStorageService _tokenStorage;
    private readonly ILogger<SignalRRealtimeService> _logger;
    private HubConnection? _hubConnection;
    private readonly string _hubUrl;

    public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;
    public string ConnectionStatusText { get; private set; } = "Chưa kết nối";
    public string ConnectionColorHex { get; private set; } = "#9CA3AF"; // Gray

    public SignalRRealtimeService(
        ITokenStorageService tokenStorage,
        ILogger<SignalRRealtimeService> logger)
    {
        _tokenStorage = tokenStorage;
        _logger = logger;

        var baseApiUrl = DeviceInfo.Platform == DevicePlatform.Android 
            ? "http://10.0.2.2:5018" 
            : "http://localhost:5018";

        _hubUrl = $"{baseApiUrl}/hubs/financial";
    }

    public async Task ConnectAsync()
    {
        if (_hubConnection != null && _hubConnection.State == HubConnectionState.Connected)
        {
            return;
        }

        try
        {
            UpdateStatus(false, "Đang kết nối Realtime...", "#F59E0B");

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(_hubUrl, options =>
                {
                    options.AccessTokenProvider = async () =>
                    {
                        return await _tokenStorage.GetAccessTokenAsync();
                    };
                })
                .WithAutomaticReconnect(new[]
                {
                    TimeSpan.Zero,
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(30)
                })
                .Build();

            // Register connection lifecycle callbacks
            _hubConnection.Reconnecting += error =>
            {
                _logger.LogWarning(error, "SignalR reconnecting...");
                UpdateStatus(false, "Đang kết nối lại...", "#F59E0B");
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += connectionId =>
            {
                _logger.LogInformation("SignalR reconnected: {ConnectionId}", connectionId);
                UpdateStatus(true, "Đã kết nối Realtime", "#10B981");
                return Task.CompletedTask;
            };

            _hubConnection.Closed += error =>
            {
                _logger.LogWarning(error, "SignalR connection closed");
                UpdateStatus(false, "Mất kết nối Realtime", "#EF4444");
                return Task.CompletedTask;
            };

            // Register event listeners
            _hubConnection.On<WalletBalanceUpdateInfo>("ReceiveBalanceUpdate", info =>
            {
                _logger.LogInformation("Realtime: Received balance update for wallet {WalletId}: {NewBalance}", info.WalletId, info.NewBalance);
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    WeakReferenceMessenger.Default.Send(new WalletBalanceUpdatedMessage(info));
                });
            });

            _hubConnection.On<BudgetAlertInfo>("ReceiveBudgetAlert", info =>
            {
                _logger.LogInformation("Realtime: Received budget alert: {Category} - {PercentUsed}%", info.CategoryName, info.PercentUsed);
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    WeakReferenceMessenger.Default.Send(new BudgetAlertReceivedMessage(info));
                });
            });

            _hubConnection.On<InAppNotificationModel>("ReceiveNotification", notification =>
            {
                _logger.LogInformation("Realtime: Received in-app notification: {Title}", notification.Title);
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    WeakReferenceMessenger.Default.Send(new InAppNotificationReceivedMessage(notification));
                });
            });

            await _hubConnection.StartAsync();
            UpdateStatus(true, "Đã kết nối Realtime", "#10B981");
            _logger.LogInformation("SignalR successfully connected to {HubUrl}", _hubUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to SignalR hub at {HubUrl}", _hubUrl);
            UpdateStatus(false, "Lỗi kết nối Realtime", "#EF4444");
        }
    }

    public async Task DisconnectAsync()
    {
        if (_hubConnection != null)
        {
            try
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error while stopping SignalR connection");
            }
            finally
            {
                _hubConnection = null;
                UpdateStatus(false, "Đã ngắt kết nối", "#9CA3AF");
            }
        }
    }

    private void UpdateStatus(bool isConnected, string statusText, string colorHex)
    {
        ConnectionStatusText = statusText;
        ConnectionColorHex = colorHex;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            WeakReferenceMessenger.Default.Send(new RealtimeConnectionChangedMessage(
                new RealtimeConnectionInfo(isConnected, statusText, colorHex)));
        });
    }

    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync();
    }
}
