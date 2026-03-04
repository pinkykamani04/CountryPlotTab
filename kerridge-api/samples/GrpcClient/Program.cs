using Grpc.Core;
using Grpc.Net.Client;
using IBMG.SCS.KerridgeApi.Server;
using System.Net.Http.Json;

var serverUrl = Environment.GetEnvironmentVariable("GRPC_SERVER") ?? "https://localhost:5001";
var clientId = Environment.GetEnvironmentVariable("CLIENT_ID") ?? "service-client-1";
var clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET") ?? "secret-key-1";
var useAzureAd = Environment.GetEnvironmentVariable("USE_AZURE_AD") == "true";

Console.WriteLine($"Connecting to gRPC server at {serverUrl}...");
Console.WriteLine($"Auth mode: {(useAzureAd ? "Azure AD Service Principal" : "Client Credentials")}");

// Fetch JWT token
string? accessToken = null;
try
{
    using var httpClient = new HttpClient { BaseAddress = new Uri(serverUrl) };
    
    object tokenRequest;
    if (useAzureAd)
    {
        // Azure AD Service Principal flow
        // In production: get Azure AD token using Azure.Identity SDK
        // For demo: use client credentials with assertion parameter
        Console.WriteLine("Note: In production, use Azure.Identity.DefaultAzureCredential to get Azure AD token");
        tokenRequest = new 
        { 
            ClientId = clientId, 
            Assertion = "azure-ad-jwt-token-here",  // In production: actual Azure AD token
            Scope = "customers.read customers.write"
        };
    }
    else
    {
        // Client credentials flow
        tokenRequest = new 
        { 
            ClientId = clientId, 
            ClientSecret = clientSecret,
            Scope = "customers.read customers.write"
        };
    }
    
    var tokenResponse = await httpClient.PostAsJsonAsync("/api/token", tokenRequest);
    
    if (tokenResponse.IsSuccessStatusCode)
    {
        var tokenData = await tokenResponse.Content.ReadFromJsonAsync<TokenResponse>();
        accessToken = tokenData?.AccessToken;
        Console.WriteLine($"JWT token obtained successfully (client_id: {clientId}).");
    }
    else
    {
        Console.WriteLine($"Failed to get token: {tokenResponse.StatusCode}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Token fetch error: {ex.Message}");
}

var channel = GrpcChannel.ForAddress(serverUrl);
var client = new CustomersService.CustomersServiceClient(channel);

// Create call options with authorization header
var headers = new Metadata();
if (!string.IsNullOrEmpty(accessToken))
{
    headers.Add("Authorization", $"Bearer {accessToken}");
}
var callOptions = new CallOptions(headers);

var list = await client.ListCustomersAsync(new ListCustomersRequest { PageSize = 5 }, callOptions);
Console.WriteLine($"Customers: {list.Customers.Count} (Total {list.TotalCount})");
foreach (var c in list.Customers)
{
	Console.WriteLine($" - {c.Id}: {c.Name} [{c.AccountCode}]");
}

if (list.Customers.Count > 0)
{
	var first = list.Customers[0];
	var summary = await client.GetCustomerSpendSummaryAsync(new GetCustomerSpendSummaryRequest { CustomerId = first.Id }, callOptions);
	Console.WriteLine($"Summary for {first.Id}: total={summary.TotalSpend}, sales={summary.TotalSales}");
}

record TokenResponse(string AccessToken, string TokenType, int ExpiresIn);
