// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.KerridgeApi.Server;
using IBMG.SCS.KerridgeApi.Server.AppDbContext;
using IBMG.SCS.KerridgeApi.Server.Middleware;
using IBMG.SCS.KerridgeApi.Server.Services;
using IBMG.SCS.KerridgeApi.Server.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("kerridgeappsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"kerridgeappsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

var MyCorsPolicy = "_myCorsPolicy";

builder.Services.AddCors(options =>
{
    options.AddPolicy(MyCorsPolicy, policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Add services to the container (REST only)

// Add REST API services
builder.Services.AddCustomerDataServices();

// Configure JWT authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "your-super-secret-key-min-32-chars-long!";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "KerridgeServer";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "KerridgeClient";

var connectionString = builder.Configuration.GetConnectionString("primaryConnectionString");
builder.Services.AddDbContext<PortalDBContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDbContext<IbmgDwhDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("IBMG_DWH"),
        sqlOptions => sqlOptions.CommandTimeout(120)));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        };
    });

builder.Services
    .Configure<KerridgeSettings>(builder.Configuration.GetSection("KerridgeSettings"));

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.UseAllOfToExtendReferenceSchemas();
    c.SupportNonNullableReferenceTypes();
    c.SchemaFilter<NullableRefSchemaFilter>();

    // JWT Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer",
        },
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
                Scheme = "Bearer",
                Name = "Authorization",
                In = ParameterLocation.Header,
            },
            new List<string>()
        },
    });

    c.AddSecurityDefinition("X-Branch-Code", new OpenApiSecurityScheme
    {
        Name = "X-Branch-Code",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Description = "Branch code header",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "X-Branch-Code",
        },
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "X-Branch-Code",
                },
                In = ParameterLocation.Header,
            },
            new List<string>()
        },
    });
});

builder.Services.Configure<Microsoft.AspNetCore.Server.Kestrel.Core.KestrelServerOptions>(options =>
{
    options.AllowSynchronousIO = true;
});

builder.Services.AddScoped<IKerridgeRoutingService, KerridgeRoutingService>();
builder.Services.AddMemoryCache();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<CustomerDataService>();

var app = builder.Build();
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Dev"))
{
    app.UseExceptionHandler(errorApp =>
    {
        errorApp.Run(async context =>
        {
            var ex = context.Features
                .Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;

            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new
            {
                message = ex?.Message,
                stackTrace = ex?.StackTrace
            });
        });
    });
}
    app.UseCors(MyCorsPolicy);

// Configure the HTTP request pipeline.
app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

// Map REST API endpoints
app.MapCustomerRestEndpoints();

app.UseMiddleware<BranchHeaderMiddleware>();

// Token endpoint supporting client credentials and Azure AD service principal
app.MapPost("/api/token", (TokenRequest request) =>
{
    // Validate client credentials
    if (string.IsNullOrEmpty(request.ClientId))
    {
        return Results.BadRequest(new { error = "client_id is required" });
    }

    // In production: validate client_id/client_secret against secure store (database, Azure Key Vault, etc.)
    // For demo purposes, accept any client_id with matching secret
    var validClients = new Dictionary<string, string>
    {
        ["service-client-1"] = "secret-key-1",
        ["service-client-2"] = "secret-key-2"
    };

    if (!string.IsNullOrEmpty(request.ClientSecret))
    {
        // Client credentials flow
        if (!validClients.TryGetValue(request.ClientId, out var expectedSecret) ||
            request.ClientSecret != expectedSecret)
        {
            return Results.Unauthorized();
        }
    }
    else if (!string.IsNullOrEmpty(request.Assertion))
    {
        // Azure AD service principal flow (client assertion)
        // In production, validate the Azure AD JWT token:
        // 1. Install: Microsoft.Identity.Web package
        // 2. Validate token signature using Azure AD public keys
        // 3. Verify issuer (https://sts.windows.net/{tenant-id}/)
        // 4. Verify audience matches your API app registration
        // 5. Check token expiration and not-before claims
        // Example:
        //   var validationParams = new TokenValidationParameters
        //   {
        //       ValidateIssuerSigningKey = true,
        //       IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
        //       {
        //           // Fetch Azure AD signing keys from https://login.microsoftonline.com/common/discovery/keys
        //       },
        //       ValidIssuer = $"https://sts.windows.net/{azureTenantId}/",
        //       ValidAudience = builder.Configuration["AzureAd:Audience"],
        //       ValidateLifetime = true
        //   };
        //   var handler = new JwtSecurityTokenHandler();
        //   var principal = handler.ValidateToken(request.Assertion, validationParams, out _);

        // For demo: accept if assertion is provided
        Console.WriteLine($"Azure AD assertion provided for client: {request.ClientId}");
    }
    else
    {
        return Results.BadRequest(new { error = "Either client_secret or assertion is required" });
    }

    var claims = new[]
    {
        new Claim("client_id", request.ClientId),
        new Claim(ClaimTypes.NameIdentifier, request.ClientId),
        new Claim("scope", request.Scope ?? "customers.read customers.write")
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: jwtIssuer,
        audience: jwtAudience,
        claims: claims,
        expires: DateTime.UtcNow.AddHours(1),
        signingCredentials: creds
    );

    var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

    // Manual JSON serialization to avoid PipeWriter UnflushedBytes issue under TestServer in .NET 10
    return Results.Text(System.Text.Json.JsonSerializer.Serialize(new { accessToken = tokenString, tokenType = "Bearer", expiresIn = 3600 }), "application/json");
});

app.MapGet("/", () => "Kerridge REST API is running.");

app.Run();

namespace IBMG.SCS.KerridgeApi.Server
{
    public partial class Program { }

    record TokenRequest(string ClientId, string? ClientSecret, string? Assertion, string? Scope);
}