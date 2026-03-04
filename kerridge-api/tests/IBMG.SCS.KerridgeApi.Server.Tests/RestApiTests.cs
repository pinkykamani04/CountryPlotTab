using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using FluentAssertions;
using IBMG.SCS.KerridgeApi.Server;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Xunit;

public class RestApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public RestApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = "test-key-that-is-at-least-32-characters-long-for-unit-testing!",
                    ["Jwt:Issuer"] = "KerridgeGrpcServer",
                    ["Jwt:Audience"] = "KerridgeGrpcClient"
                });
            });
        });
    }

    private async Task<string> GetTokenAsync()
    {
        var httpClient = _factory.CreateClient();
        var tokenResponse = await httpClient.PostAsJsonAsync("/api/token", new 
        { 
            ClientId = "service-client-1", 
            ClientSecret = "secret-key-1",
            Scope = "customers.read customers.write"
        });
        tokenResponse.EnsureSuccessStatusCode();
        
        var tokenData = await tokenResponse.Content.ReadFromJsonAsync<TokenResponse>();
        return tokenData!.AccessToken;
    }

    private HttpClient CreateAuthenticatedClient(string token)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private HttpClient CreateUnauthenticatedClient()
    {
        return _factory.CreateClient();
    }

    [Fact]
    public async Task ListCustomers_returns_success()
    {
        var token = await GetTokenAsync();
        var client = CreateAuthenticatedClient(token);

        var response = await client.GetAsync("/api/v1/customers");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCustomer_returns_success()
    {
        var token = await GetTokenAsync();
        var client = CreateAuthenticatedClient(token);

        var response = await client.GetAsync("/api/v1/customers/CUST-001");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ListCustomerOrders_returns_success()
    {
        var token = await GetTokenAsync();
        var client = CreateAuthenticatedClient(token);

        var response = await client.GetAsync("/api/v1/customers/CUST-001/orders");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CountCustomerOrders_returns_success()
    {
        var token = await GetTokenAsync();
        var client = CreateAuthenticatedClient(token);

        var response = await client.GetAsync("/api/v1/customers/CUST-001/orders/count");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCustomerOrder_returns_success()
    {
        var token = await GetTokenAsync();
        var client = CreateAuthenticatedClient(token);

        var response = await client.GetAsync("/api/v1/customers/CUST-001/orders/ORD-001");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCustomerSpendSummary_returns_success()
    {
        var token = await GetTokenAsync();
        var client = CreateAuthenticatedClient(token);

        var response = await client.GetAsync("/api/v1/customers/CUST-001/spend/summary");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ListCustomerSpend_returns_success()
    {
        var token = await GetTokenAsync();
        var client = CreateAuthenticatedClient(token);

        var response = await client.GetAsync("/api/v1/customers/CUST-001/spend");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCustomerOperativeSpend_returns_success()
    {
        var token = await GetTokenAsync();
        var client = CreateAuthenticatedClient(token);

        var response = await client.GetAsync("/api/v1/customers/CUST-001/spend/operatives/OP-001");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetKpiOtif_returns_success()
    {
        var token = await GetTokenAsync();
        var client = CreateAuthenticatedClient(token);

        var response = await client.GetAsync("/api/v1/customers/CUST-001/kpis/otif");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetKpiInvoiceAccuracy_returns_success()
    {
        var token = await GetTokenAsync();
        var client = CreateAuthenticatedClient(token);

        var response = await client.GetAsync("/api/v1/customers/CUST-001/kpis/invoice-accuracy");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetKpiFaultyGoods_returns_success()
    {
        var token = await GetTokenAsync();
        var client = CreateAuthenticatedClient(token);

        var response = await client.GetAsync("/api/v1/customers/CUST-001/kpis/faulty-goods");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Unauthenticated_request_returns_unauthorized()
    {
        var client = CreateUnauthenticatedClient();
        
        var response = await client.GetAsync("/api/v1/customers");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Swagger_endpoint_is_accessible()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/swagger/v1/swagger.json");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}

public record TokenResponse(string AccessToken, string TokenType, int ExpiresIn);