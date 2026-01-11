using System.Net.Http.Headers;

namespace FinancasApp.Mobile.Services;

public class AuthTokenHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var path = request.RequestUri?.AbsolutePath ?? "";

        // Não adiciona token para endpoints públicos
        if (!path.Contains("/auth/login", StringComparison.OrdinalIgnoreCase) &&
            !path.Contains("/auth/register", StringComparison.OrdinalIgnoreCase))
        {
            var token = await SecureStorage.Default.GetAsync("jwt_token");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        return await base.SendAsync(request, cancellationToken);
    }
}