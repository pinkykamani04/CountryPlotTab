# Kerridge Customer API (gRPC + REST)

This repository includes a .NET 10 API with **JWT authentication** that provides both **gRPC** and **REST** interfaces for Customer queries, Orders, Spend, and KPIs.

Key locations:
- Solution file: `IBMG.SCS.KerridgeApi.sln`
- Server project: `source/IBMG.SCS.KerridgeApi.Server/IBMG.SCS.KerridgeApi.Server.csproj`
- Proto (gRPC): `source/IBMG.SCS.KerridgeApi.Server/Protos/customers.proto`
- REST API: `source/IBMG.SCS.KerridgeApi.Server/RestApiExtensions.cs`
- Data Service: `source/IBMG.SCS.KerridgeApi.Server/Services/CustomerDataService.cs`
- Sample gRPC client: `samples/GrpcClient`
- Tests: `tests/Kerridge.GrpcServer.Tests`

## API Endpoints

The API exposes customer data through both gRPC and REST interfaces:

### gRPC Service
The `.proto` defines `CustomersService` with these RPCs:
- `ListCustomers`, `GetCustomer`
- `ListCustomerOrders`, `CountCustomerOrders`, `GetCustomerOrder`
- `GetCustomerSpendSummary`, `ListCustomerSpend`, `GetCustomerOperativeSpend`
- `GetKpiOtif`, `GetKpiInvoiceAccuracy`, `GetKpiFaultyGoods`

### REST API
Minimal API endpoints with OpenAPI (Swagger) documentation:

**Customers:**
- `GET /api/customers` - List all customers
- `GET /api/customers/{id}` - Get customer by ID

**Orders:**
- `GET /api/customers/{customerId}/orders` - List customer orders
- `GET /api/customers/{customerId}/orders/count` - Count customer orders
- `GET /api/customers/{customerId}/orders/{orderId}` - Get specific order

**Spend:**
- `GET /api/customers/{customerId}/spend/summary` - Get spend summary
- `GET /api/customers/{customerId}/spend` - List detailed spend records
- `GET /api/customers/{customerId}/spend/operative` - Get operative spend

**KPIs:**
- `GET /api/customers/{customerId}/kpis/otif` - On-Time In-Full percentage
- `GET /api/customers/{customerId}/kpis/invoice-accuracy` - Invoice accuracy percentage
- `GET /api/customers/{customerId}/kpis/faulty-goods` - Faulty goods percentage

All REST endpoints require JWT authentication and include OpenAPI metadata with descriptions and response types.

## Prerequisites
- .NET SDK 10.0 (repo has `global.json`)
- Dev HTTPS cert for Kestrel:

```pwsh
dotnet dev-certs https --trust
```

## Security: JWT Authentication (Client Credentials)
All gRPC endpoints require a valid JWT Bearer token using **OAuth 2.0 Client Credentials** flow.

### Authentication Modes
The API supports two authentication modes:

#### 1. Client Credentials (Default)
Standard OAuth 2.0 client credentials for machine-to-machine authentication:

```pwsh
$response = Invoke-RestMethod -Method POST -Uri "https://localhost:5001/api/token" `
    -ContentType "application/json" `
    -Body '{"client_id":"service-client-1","client_secret":"secret-key-1","scope":"customers.read customers.write"}'
$token = $response.access_token
Write-Host "Token: $token"
```

Valid demo clients:
- `service-client-1` / `secret-key-1`
- `service-client-2` / `secret-key-2`

**⚠️ Production**: Store client credentials in secure vaults (Azure Key Vault, AWS Secrets Manager) and validate against a secure store.

#### 2. Azure AD Service Principal (for Azure deployments)
When deployed to Azure, use Azure AD service principal with managed identity:

```pwsh
# Set environment variable to enable Azure AD mode
$env:USE_AZURE_AD = "true"
$env:CLIENT_ID = "your-azure-app-id"

# In production code, use Azure.Identity.DefaultAzureCredential
# to automatically get Azure AD tokens
```

The server accepts Azure AD JWT assertions via the `assertion` parameter. In production:
- Use `Azure.Identity` SDK with `DefaultAzureCredential`
- Configure Azure AD app registration
- Grant appropriate API permissions
- Token validation happens automatically via Azure AD public keys

**📖 For detailed Azure AD setup instructions, see [.docs/AZURE_AD_SETUP.md](.docs/AZURE_AD_SETUP.md)**

### JWT Configuration
Configure JWT settings in `appsettings.json`:
```json
{
  "Jwt": {
    "Key": "your-secret-key-minimum-32-characters",
    "Issuer": "KerridgeGrpcServer",
    "Audience": "KerridgeGrpcClient"
  }
}
```

**⚠️ Production Security**:
- Use Azure Key Vault to store JWT signing key
- Rotate keys regularly
- Use strong random keys (min. 32 characters)
- For Azure AD mode, configure proper app registrations and permissions

## Run the Server

```pwsh
cd source/IBMG.SCS.KerridgeApi.Server
dotnet run --urls https://localhost:5001
```

The server hosts:
- **gRPC** over HTTP/2 on `https://localhost:5001`
- **REST API** with OpenAPI on `https://localhost:5001/api/*`
- **Swagger UI** at `https://localhost:5001/swagger` (Development mode only)

### Explore the REST API

Open Swagger UI in your browser:
```
https://localhost:5001/swagger
```

Or test endpoints directly:
```pwsh
# Get a token first
$response = Invoke-RestMethod -Method POST -Uri "https://localhost:5001/api/token" `
    -ContentType "application/json" `
    -Body '{"client_id":"service-client-1","client_secret":"secret-key-1","scope":"customers.read"}'
$token = $response.access_token

# Call REST endpoints
$headers = @{ Authorization = "Bearer $token" }
Invoke-RestMethod -Uri "https://localhost:5001/api/customers" -Headers $headers
Invoke-RestMethod -Uri "https://localhost:5001/api/customers/CUST001" -Headers $headers
Invoke-RestMethod -Uri "https://localhost:5001/api/customers/CUST001/orders" -Headers $headers
```

## Try the Client

The client automatically fetches a JWT token using client credentials:

```pwsh
# Client Credentials mode (default)
$env:GRPC_SERVER = "https://localhost:5001"
$env:CLIENT_ID = "service-client-1"
$env:CLIENT_SECRET = "secret-key-1"
cd samples/GrpcClient
dotnet run
```

For Azure AD service principal mode:
```pwsh
$env:GRPC_SERVER = "https://localhost:5001"
$env:CLIENT_ID = "your-azure-app-id"
$env:USE_AZURE_AD = "true"
cd samples/GrpcClient
dotnet run
```

The client:
1. Requests a JWT token from `/api/token` using client credentials or Azure AD assertion
2. Adds the token to gRPC metadata (`Authorization: Bearer <token>`)
3. Calls `ListCustomers` and `GetCustomerSpendSummary`

## Run Tests

```pwsh
dotnet test
```

Or test a specific project:
```pwsh
dotnet test tests/Kerridge.GrpcServer.Tests/Kerridge.GrpcServer.Tests.csproj
```

Tests include:
- **gRPC Service Tests**: Authenticated gRPC calls, unauthenticated rejection (401), service functionality
- **REST API Tests**: All 11 REST endpoints with authentication and authorization
- **Data Service Tests**: Business logic validation for customer, order, spend, and KPI operations

## Build Everything

```pwsh
dotnet build IBMG.SCS.KerridgeApi.sln
```

Or build individual projects:

```pwsh
dotnet build source/IBMG.SCS.KerridgeApi.Server/IBMG.SCS.KerridgeApi.Server.csproj
dotnet build samples/GrpcClient/GrpcClient.csproj
dotnet build tests/Kerridge.GrpcServer.Tests/Kerridge.GrpcServer.Tests.csproj
```

## Architecture Notes

### Dual API Design
- **Shared Business Logic**: `CustomerDataService` provides data access for both gRPC and REST
- **gRPC Service**: `CustomersService` wraps `CustomerDataService` with gRPC contracts
- **REST API**: Minimal API endpoints in `RestApiExtensions` also use `CustomerDataService`
- Both APIs share the same authentication, authorization, and data layer

### Sample Data
The `CustomerDataService` includes sample data for development and testing:
- 10 customers with various attributes
- 15 orders across different customers
- Spend records with date ranges
- KPI data (OTIF, invoice accuracy, faulty goods)

Replace with your actual data source (database, external API, etc.) in production.

### Security & Best Practices
- Auth is propagated via gRPC metadata (`authorization: Bearer <jwt>`) and REST headers
- All endpoints require JWT authentication with appropriate scopes
- Currency fields are doubles as placeholders; switch to fixed-point (e.g., `decimal` or `int64` minor units) for precision
- For large datasets, consider:
  - Server-streaming gRPC for orders/spend
  - Pagination on REST endpoints (query parameters)
  - Response compression

### OpenAPI Integration
- REST API includes full OpenAPI (Swagger) documentation
- Endpoints include descriptions, parameters, and response schemas
- Swagger UI available in Development mode at `/swagger`
- OpenAPI JSON available at `/openapi/v1.json`
