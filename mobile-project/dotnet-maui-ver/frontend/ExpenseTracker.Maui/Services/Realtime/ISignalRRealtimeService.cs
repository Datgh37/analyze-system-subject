namespace ExpenseTracker.Maui.Services.Realtime;

public interface ISignalRRealtimeService
{
    bool IsConnected { get; }
    string ConnectionStatusText { get; }
    string ConnectionColorHex { get; }

    Task ConnectAsync();
    Task DisconnectAsync();
}
