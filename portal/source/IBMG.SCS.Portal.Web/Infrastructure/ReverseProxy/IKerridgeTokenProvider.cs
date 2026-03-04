// Copyright (c) IBMG. All rights reserved.

using System.Net.Http.Json;

namespace IBMG.SCS.Portal.Web.Infrastructure.ReverseProxy;

public interface IKerridgeTokenProvider
{
    Task<string?> GetTokenAsync(CancellationToken cancellationToken = default);
}

public sealed class KerridgeTokenProvider : IKerridgeTokenProvider
{
    private readonly IConfiguration configuration;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILogger<KerridgeTokenProvider> logger;
    private readonly SemaphoreSlim gate = new(1, 1);

    private string? cachedToken;
    private DateTime tokenExpiryUtc = DateTime.MinValue;

    public KerridgeTokenProvider(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<KerridgeTokenProvider> logger)
    {
        this.configuration = configuration;
        this.httpClientFactory = httpClientFactory;
        this.logger = logger;
    }

    public async Task<string?> GetTokenAsync(CancellationToken cancellationToken = default)
    {
        if (this.IsTokenValid())
        {
            return this.cachedToken;
        }

        await this.gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (this.IsTokenValid())
            {
                return this.cachedToken;
            }

            var baseUrl = this.configuration["KerridgeApi:BaseUrl"];
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                this.logger.LogWarning("KerridgeApi:BaseUrl is missing; cannot request token.");
                return null;
            }

            var clientId = this.configuration["KerridgeApi:ClientId"];
            var clientSecret = this.configuration["KerridgeApi:ClientSecret"];

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
            {
                this.logger.LogWarning("Kerridge API credentials are missing; cannot request token.");
                return null;
            }

            var http = this.httpClientFactory.CreateClient();
            var body = new
            {
                clientId,
                clientSecret,
                scope = "customers.read customers.write",
            };

            var response = await http.PostAsJsonAsync(
                $"{baseUrl.TrimEnd('/')}/api/token",
                body,
                cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                this.logger.LogError("Failed to obtain Kerridge token. Status: {StatusCode}", response.StatusCode);
                return null;
            }

            var token = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(token?.AccessToken))
            {
                this.logger.LogError("Kerridge token response did not contain an access token.");
                return null;
            }

            var lifetimeSeconds = token.ExpiresIn > 120 ? token.ExpiresIn - 60 : token.ExpiresIn;
            this.cachedToken = token.AccessToken;
            this.tokenExpiryUtc = DateTime.UtcNow.AddSeconds(lifetimeSeconds);
            return this.cachedToken;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error obtaining Kerridge JWT token");
            return null;
        }
        finally
        {
            this.gate.Release();
        }
    }

    private bool IsTokenValid() => !string.IsNullOrWhiteSpace(this.cachedToken) && DateTime.UtcNow < this.tokenExpiryUtc.AddMinutes(-5);

    private sealed record TokenResponse(string? AccessToken, string? TokenType, int ExpiresIn);
}
