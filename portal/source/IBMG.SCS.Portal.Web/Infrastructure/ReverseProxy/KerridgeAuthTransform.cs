// Copyright (c) IBMG. All rights reserved.

using System.Text.Json;
using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace IBMG.SCS.Portal.Web.Infrastructure.ReverseProxy;

/// <summary>
/// YARP transform that injects JWT authentication token for Kerridge API requests.
/// </summary>
public class KerridgeAuthTransform : ITransformProvider
{
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<KerridgeAuthTransform> _logger;
    private string? _cachedToken;
    private DateTime _tokenExpiry = DateTime.MinValue;

    public KerridgeAuthTransform(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<KerridgeAuthTransform> logger)
    {
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public void ValidateRoute(TransformRouteValidationContext context)
    {
        // No validation needed
    }

    public void ValidateCluster(TransformClusterValidationContext context)
    {
        // No validation needed
    }

    public void Apply(TransformBuilderContext context)
    {
        // Rewrite path: strip /api/kerridge/ prefix from request path
        context.AddRequestTransform(async transformContext =>
        {
            var originalUri = transformContext.ProxyRequest.RequestUri;
            var path = originalUri?.AbsolutePath ?? string.Empty;
            var rewrittenPath = path;

            if (path.StartsWith("/api/kerridge/", StringComparison.OrdinalIgnoreCase))
            {
                var newPath = path.Substring("/api/kerridge".Length); // Keeps the leading /
                var newUri = new UriBuilder(originalUri!)
                {
                    Path = newPath
                }.Uri;
                transformContext.ProxyRequest.RequestUri = newUri;
                rewrittenPath = newPath;
                this._logger.LogDebug("Rewrote path from {OriginalPath} to {NewPath}", path, newPath);
            }

            var proxiedUri = transformContext.ProxyRequest.RequestUri;
            this._logger.LogInformation(
                "Kerridge proxy {Method} {OriginalPath} -> {ProxiedPath} (host {Host})",
                transformContext.HttpContext.Request.Method,
                path,
                proxiedUri?.PathAndQuery ?? rewrittenPath,
                proxiedUri?.Authority ?? "(null)");

            var token = await this.GetOrRefreshTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                transformContext.ProxyRequest.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                this._logger.LogDebug("Added Bearer token to Kerridge API request");
            }
            else
            {
                this._logger.LogWarning("Failed to obtain JWT token for Kerridge API");
            }
        });
    }

    private async Task<string?> GetOrRefreshTokenAsync()
    {
        // Check if we have a valid cached token
        if (!string.IsNullOrEmpty(_cachedToken) && DateTime.UtcNow < _tokenExpiry.AddMinutes(-5))
        {
            return _cachedToken;
        }

        try
        {
            var kerridgeBaseUrl = _configuration["KerridgeApi:BaseUrl"] ?? "https://localhost:7017";
            var clientId = _configuration["KerridgeApi:ClientId"] ?? "service-client-1";
            var clientSecret = _configuration["KerridgeApi:ClientSecret"] ?? "secret-key-1";

            var httpClient = _httpClientFactory.CreateClient();
            var requestBody = new
            {
                clientId = clientId,
                clientSecret = clientSecret,
                scope = "customers.read customers.write"
            };

            var response = await httpClient.PostAsJsonAsync(
                $"{kerridgeBaseUrl}/api/token",
                requestBody);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (tokenResponse?.AccessToken != null)
                {
                    _cachedToken = tokenResponse.AccessToken;
                    _tokenExpiry = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 60); // Refresh 1 min early

                    _logger.LogInformation("Successfully obtained JWT token for Kerridge API");
                    return _cachedToken;
                }
            }
            else
            {
                _logger.LogError("Failed to obtain token. Status: {StatusCode}", response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error obtaining JWT token for Kerridge API");
        }

        return null;
    }

    private class TokenResponse
    {
        public string? AccessToken { get; set; }
        public string? TokenType { get; set; }
        public int ExpiresIn { get; set; }
    }
}
