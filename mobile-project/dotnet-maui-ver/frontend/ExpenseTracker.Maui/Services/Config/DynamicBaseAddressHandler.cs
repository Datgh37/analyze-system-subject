namespace ExpenseTracker.Maui.Services.Config;

// DelegatingHandler tự động định tuyến lại BaseAddress của mọi cuộc gọi API Refit 
// theo đúng địa chỉ ServerBaseUrl hiện hành đang được lưu trong IServerConfigService
public class DynamicBaseAddressHandler : DelegatingHandler
{
    private readonly IServerConfigService _serverConfigService;

    public DynamicBaseAddressHandler(IServerConfigService serverConfigService)
    {
        _serverConfigService = serverConfigService;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var currentBase = _serverConfigService.ServerBaseUrl;

        if (!string.IsNullOrWhiteSpace(currentBase) && request.RequestUri != null)
        {
            if (Uri.TryCreate(currentBase, UriKind.Absolute, out var currentBaseUri))
            {
                var builder = new UriBuilder(request.RequestUri)
                {
                    Scheme = currentBaseUri.Scheme,
                    Host = currentBaseUri.Host,
                    Port = currentBaseUri.Port
                };
                request.RequestUri = builder.Uri;
            }
        }

        return base.SendAsync(request, cancellationToken);
    }
}
