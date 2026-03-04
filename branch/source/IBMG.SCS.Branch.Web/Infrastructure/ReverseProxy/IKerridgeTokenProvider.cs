// Copyright (c) IBMG. All rights reserved.

using System.Net.Http.Json;

namespace IBMG.SCS.Branch.Web.Infrastructure.ReverseProxy
{
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
            if (IsTokenValid())
            {
                return cachedToken;
            }

            await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (IsTokenValid())
                {
                    return cachedToken;
                }

                var baseUrl = configuration["KerridgeApi:BaseUrl"];
                if (string.IsNullOrWhiteSpace(baseUrl))
                {
                    logger.LogWarning("KerridgeApi:BaseUrl is missing; cannot request token.");
                    return null;
                }

                var clientId = configuration["KerridgeApi:ClientId"];
                var clientSecret = configuration["KerridgeApi:ClientSecret"];

                if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
                {
                    logger.LogWarning("Kerridge API credentials are missing; cannot request token.");
                    return null;
                }

                var http = httpClientFactory.CreateClient();
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
                    logger.LogError("Failed to obtain Kerridge token. Status: {StatusCode}", response.StatusCode);
                    return null;
                }

                var token = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                if (string.IsNullOrWhiteSpace(token?.AccessToken))
                {
                    logger.LogError("Kerridge token response did not contain an access token.");
                    return null;
                }

                var lifetimeSeconds = token.ExpiresIn > 120 ? token.ExpiresIn - 60 : token.ExpiresIn;
                cachedToken = token.AccessToken;
                tokenExpiryUtc = DateTime.UtcNow.AddSeconds(lifetimeSeconds);
                return cachedToken;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error obtaining Kerridge JWT token");
                return null;
            }
            finally
            {
                gate.Release();
            }
        }

        private bool IsTokenValid() => !string.IsNullOrWhiteSpace(cachedToken) && DateTime.UtcNow < tokenExpiryUtc.AddMinutes(-5);

        private sealed record TokenResponse(string? AccessToken, string? TokenType, int ExpiresIn);
    }
}
