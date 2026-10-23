using System.Net.Http.Headers;
using ExpenseTracker.Maui.Services.Security;

namespace ExpenseTracker.Maui.Services.Api;

// HTTP delegating handler injecting Bearer authorization token into outgoing requests
public class AuthHeaderHandler : DelegatingHandler
{
    private readonly ITokenStorageService _tokenStorageService;

    public AuthHeaderHandler(ITokenStorageService tokenStorageService)
    {
        _tokenStorageService = tokenStorageService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await _tokenStorageService.GetAccessTokenAsync();
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
