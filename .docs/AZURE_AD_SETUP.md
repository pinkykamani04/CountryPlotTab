# Azure AD Authentication Setup

This guide shows how to configure Azure AD service principal authentication for production deployments.

## Overview

The API supports two authentication modes:
1. **Client Credentials** (default) - Simple client_id/client_secret for development
2. **Azure AD Service Principal** - Enterprise-grade authentication using Azure AD

## Azure AD Setup Steps

### 1. Register the API Application

```bash
# Login to Azure
az login

# Create app registration for the API
az ad app create \
  --display-name "Kerridge gRPC API" \
  --identifier-uris "api://kerridge-grpc-api" \
  --sign-in-audience AzureADMyOrg

# Note the appId (client ID) from the output
```

### 2. Create App Roles (Optional)

Define permissions for your API in the Azure Portal:
- Go to **Azure Active Directory > App registrations > Your API**
- Select **App roles > Create app role**
- Add roles like:
  - `Customers.Read` - Read customer data
  - `Customers.Write` - Modify customer data

### 3. Register Client Application

```bash
# Create client application (service principal)
az ad app create \
  --display-name "Kerridge gRPC Client" \
  --sign-in-audience AzureADMyOrg

# Create service principal
az ad sp create --id <client-app-id>

# Create client secret
az ad app credential reset --id <client-app-id> --append
# Save the password (client secret) - it won't be shown again
```

### 4. Grant API Permissions

In Azure Portal:
1. Go to **Azure AD > App registrations > Client App**
2. Select **API permissions > Add a permission**
3. Choose **My APIs** tab
4. Select your API app
5. Select the app roles you created
6. Click **Grant admin consent**

### 5. Update Server Configuration

Add Azure AD configuration to `appsettings.Production.json`:

```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "TenantId": "your-tenant-id",
    "ClientId": "your-api-app-id",
    "Audience": "api://kerridge-grpc-api"
  },
  "Jwt": {
    "Key": "your-secret-key-from-keyvault",
    "Issuer": "KerridgeGrpcServer",
    "Audience": "KerridgeGrpcClient"
  }
}
```

**Security Best Practice**: Store sensitive values in Azure Key Vault:

```bash
# Store JWT signing key
az keyvault secret set \
  --vault-name "your-keyvault" \
  --name "JwtSigningKey" \
  --value "your-secret-key-minimum-32-characters"
```

Reference in `appsettings.json`:
```json
{
  "Jwt": {
    "Key": "@Microsoft.KeyVault(SecretUri=https://your-keyvault.vault.azure.net/secrets/JwtSigningKey/)"
  }
}
```

## Client Implementation

### Install Azure Identity SDK

```xml
<PackageReference Include="Azure.Identity" Version="1.10.4" />
```

### Get Azure AD Token

```csharp
using Azure.Core;
using Azure.Identity;

// Use DefaultAzureCredential for automatic credential discovery
// Works with: Managed Identity, Azure CLI, Visual Studio, Environment Variables
var credential = new DefaultAzureCredential();

var tokenRequestContext = new TokenRequestContext(
    new[] { "api://kerridge-grpc-api/.default" });

var tokenResult = await credential.GetTokenAsync(tokenRequestContext);
string azureAdToken = tokenResult.Token;

// Exchange Azure AD token for API JWT
var httpClient = new HttpClient { BaseAddress = new Uri(serverUrl) };
var tokenResponse = await httpClient.PostAsJsonAsync("/api/token", new
{
    client_id = "your-client-app-id",
    assertion = azureAdToken,
    scope = "customers.read customers.write"
});

var apiToken = await tokenResponse.Content.ReadFromJsonAsync<TokenResponse>();
// Use apiToken.AccessToken in gRPC calls
```

### Environment Variables for Client

```bash
# For local development
export AZURE_CLIENT_ID="your-client-app-id"
export AZURE_CLIENT_SECRET="your-client-secret"
export AZURE_TENANT_ID="your-tenant-id"
```

### Using Managed Identity in Azure

When deployed to Azure App Service, Container Apps, or AKS:

```csharp
// No credentials needed - DefaultAzureCredential automatically uses Managed Identity
var credential = new DefaultAzureCredential();
```

Enable Managed Identity:
```bash
# For App Service
az webapp identity assign --name your-app-name --resource-group your-rg

# For Container Apps
az containerapp identity assign --name your-app-name --resource-group your-rg
```

Grant permissions to Managed Identity:
```bash
# Get the managed identity's principal ID
PRINCIPAL_ID=$(az webapp identity show --name your-app-name --resource-group your-rg --query principalId -o tsv)

# Assign app role to the managed identity
az ad app role assignment create \
  --assignee-object-id $PRINCIPAL_ID \
  --app-id <your-api-app-id> \
  --role-id <your-app-role-id>
```

## Server-Side Azure AD Token Validation

To implement full Azure AD token validation, update `Program.cs`:

```csharp
// Install: Microsoft.Identity.Web
using Microsoft.Identity.Web;

// In Program.cs, replace the token endpoint validation with:
else if (!string.IsNullOrEmpty(request.Assertion))
{
    try
    {
        var tenantId = builder.Configuration["AzureAd:TenantId"];
        var audience = builder.Configuration["AzureAd:Audience"];
        
        var validationParams = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
            {
                // Microsoft.Identity.Web handles Azure AD key resolution
                var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                    $"https://login.microsoftonline.com/{tenantId}/v2.0/.well-known/openid-configuration",
                    new OpenIdConnectConfigurationRetriever());
                
                var config = configManager.GetConfigurationAsync().Result;
                return config.SigningKeys;
            },
            ValidIssuer = $"https://sts.windows.net/{tenantId}/",
            ValidAudience = audience,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5)
        };

        var handler = new JwtSecurityTokenHandler();
        var principal = handler.ValidateToken(request.Assertion, validationParams, out var validatedToken);
        
        // Extract client_id from validated token
        var azureClientId = principal.FindFirst("appid")?.Value ?? request.ClientId;
        
        // Now issue your API's JWT token with validated client identity
        // ... (rest of token generation code)
    }
    catch (SecurityTokenException ex)
    {
        return Results.Unauthorized();
    }
}
```

## Testing

### Local Testing with Azure AD

```bash
# Set environment variables
export USE_AZURE_AD=true
export CLIENT_ID="your-client-app-id"
export AZURE_CLIENT_SECRET="your-client-secret"
export AZURE_TENANT_ID="your-tenant-id"

# Run the client
dotnet run --project samples/GrpcClient
```

### Integration Tests with Azure AD

For CI/CD pipelines, use service principal credentials:

```yaml
# Azure DevOps / GitHub Actions
env:
  AZURE_CLIENT_ID: ${{ secrets.AZURE_CLIENT_ID }}
  AZURE_CLIENT_SECRET: ${{ secrets.AZURE_CLIENT_SECRET }}
  AZURE_TENANT_ID: ${{ secrets.AZURE_TENANT_ID }}
```

## Security Checklist

- [ ] API app registration created with appropriate identifier URI
- [ ] Client app registration created with service principal
- [ ] App roles defined for granular permissions
- [ ] Admin consent granted for API permissions
- [ ] JWT signing key stored in Azure Key Vault
- [ ] Managed Identity enabled for Azure deployments
- [ ] Token validation implemented with signature verification
- [ ] HTTPS enforced for all endpoints
- [ ] Logging configured for authentication events
- [ ] Token expiration and refresh logic implemented

## Troubleshooting

### "Invalid audience" error
- Verify the `audience` claim in the Azure AD token matches your API's app registration URI
- Check `ValidAudience` in `TokenValidationParameters`

### "Invalid issuer" error
- Ensure tenant ID is correct
- Verify issuer format: `https://sts.windows.net/{tenant-id}/`

### "Signature validation failed"
- Check Azure AD signing keys are being fetched correctly
- Verify network connectivity to `login.microsoftonline.com`
- Token may have expired - check token lifetime

### Managed Identity not working
- Verify Managed Identity is enabled and has correct permissions
- Check that app role assignments are configured
- Ensure `DefaultAzureCredential` is being used correctly

## Additional Resources

- [Microsoft Identity Platform](https://docs.microsoft.com/azure/active-directory/develop/)
- [Azure.Identity SDK](https://docs.microsoft.com/dotnet/api/overview/azure/identity-readme)
- [Microsoft.Identity.Web](https://github.com/AzureAD/microsoft-identity-web)
- [Managed Identities](https://docs.microsoft.com/azure/active-directory/managed-identities-azure-resources/)
