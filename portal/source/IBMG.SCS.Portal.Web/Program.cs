// Copyright (c) IBMG. All rights reserved.

using BluQube.Attributes;
using BluQube.Commands;
using BluQube.Queries;
using Finbuckle.MultiTenant.AspNetCore.Extensions;
using FluentValidation;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Portal.Web.Components;
using IBMG.SCS.Portal.Web.Infrastructure.Data;
using IBMG.SCS.Portal.Web.Infrastructure.Extensions;
using IBMG.SCS.Portal.Web.Infrastructure.Multitenancy;
using IBMG.SCS.Portal.Web.Infrastructure.ReverseProxy;
using IBMG.SCS.Portal.Web.Infrastructure.Services;
using IBMG.SCS.Portal.Web.Infrastructure.Settings;
using IBMG.SCS.Portal.Web.Services.Contracts;
using IBMG.SCS.Portal.Web.Shared;
using MediatR.Behaviors.Authorization.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using PearDrop.Authentication;
using PearDrop.Authentication.Contracts;
using PearDrop.Authentication.Data.ReadModels;
using PearDrop.Authentication.Multitenancy;
using PearDrop.Authentication.Multitenancy.Services;
using PearDrop.Authentication.RazorPages.Extensions;
using PearDrop.Domain.Contracts;
using PearDrop.Extensions;
using PearDrop.Multitenancy;
using Yarp.ReverseProxy.Transforms.Builder;

namespace IBMG.SCS.Portal.Web;

[BluQubeResponder]
public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration
            .AddJsonFile($"appsettings.{Environment.MachineName}.json", true, true);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveWebAssemblyComponents()
            .AddAuthenticationStateSerialization(
                options => options.SerializeAllClaims = true);

        builder.Services.AddCascadingAuthenticationState();

        builder.Services.AddRazorPages();
        builder.Services.AddPearDropAuthPageAliases();
        builder.Services.AddControllers();

        builder.Services.AddPearDropSingleTenant(builder.Configuration);

        builder.Services
                  .AddPearDropAuthentication(builder.Configuration)
                  .AddPearDropMultitenancy(builder.Configuration)
                  .AddPearDropAuthenticationMultitenancy();

        builder.Services.AddScoped<UserPrincipalNameTenantResolver>();
        builder.Services.AddScoped<PearDrop.Contracts.ITenantResolver>(sp => sp.GetRequiredService<UserPrincipalNameTenantResolver>());

        // Register claims transformation for adding TenantId claim
        builder.Services.AddScoped<IClaimsTransformation, TenantClaimsTransformation>();

        builder.Services.AddAntiforgery(options =>
        {
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });
        builder.Services
    .Configure<EmailSettings>(builder.Configuration.GetSection("Email"));

        builder.Services.AddScoped<ICommander, Commander>();
        builder.Services.AddScoped<IQuerier, Querier>();
        builder.Services.AddScoped<IDashboardService, DashboardService>();
        var emailBuilder = builder.Services.AddFluentEmail(builder.Configuration.GetValue<string>(ContentTypeConstants.FromEmailAddress)!);

        emailBuilder.AddRazorRenderer();

        if (!string.IsNullOrWhiteSpace(builder.Configuration["email:sendgridapiKey"]))
        {
            emailBuilder.AddSendGridSender(builder.Configuration["email:sendgridapiKey"]);
        }
        else
        {
            emailBuilder.AddSmtpSender(
                builder.Configuration["email:smtp:host"],
                int.Parse(builder.Configuration["email:smtp:port"]!));
        }

        emailBuilder.AddSendGridSender(builder.Configuration.GetValue<string>(ContentTypeConstants.SendgridApiKey)!);

        builder.Services.RemoveAll<IAuthenticationNotifier>();
        builder.Services.AddScoped<IAuthenticationNotifier, EmailAuthenticationNotifier>();
        builder.Services.AddScoped<IExtendedAuthenticationNotifier, EmailAuthenticationNotifier>();
        builder.Services.AddScoped<ITenantExecutionScope, TenantExecutionScope>();
        builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
        var primaryConnectionString = builder.Configuration["PearDrop:modules:core:PrimaryConnectionString"];
        builder.Services.AddDbContext<PortalDBContext>(options =>
            options.UseSqlServer(primaryConnectionString));
        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
        builder.Services.AddMediatorAuthorization(typeof(Program).Assembly);
        builder.Services.AddAuthorizersFromAssembly(typeof(Program).Assembly);

        builder.Services.AddSingleton<IKerridgeTokenProvider, KerridgeTokenProvider>();

        builder.Services.AddHttpClient();

        builder.Services.AddHttpContextAccessor();
        builder.Services.Configure<JsonOptions>(options =>
        {
            options.AddBluQubeJsonConverters();
            options.AddJsonConvertersForAuthentication();
            options.AddJsonConvertersForPearDropAuthenticationMultitenancy();
            options.AddJsonConvertersForMultitenancy();
        });

        builder.Services.AddScoped<IChangeProcessorAccessor, ChangeProcessorAccessor>();

        builder.Services.AddScoped<IAuthReadModels, AuthReadModels>();

        builder.Services.RegisterApplicationDbContextFactory<PortalDBContext>(typeof(PortalDBContext).Assembly.GetName().Name ?? string.Empty);

        builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
        builder.Services.AddSingleton<ITransformProvider, KerridgeAuthTransform>();
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("KerridgeProxyAuth", policy =>
            {
                policy.AddAuthenticationSchemes("peardrop");
                policy.RequireAuthenticatedUser();
            });
        });

        // Use Portal.Web as proxy for Kerridge API (handles authentication server-side)
        var kerridgeProxyUrl = builder.Configuration.GetSection("ReverseProxy") + "api/kerridge";

        builder.Services.AddScoped<ApiClient.IClient>(sp =>
        {
            var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient("kerridge");

            var client = new ApiClient.Client(kerridgeProxyUrl, http)
            {
                ReadResponseAsString = true,
            };

            return client;
        });

        var app = builder.Build();

        await app.Services.ApplyAuthenticationMigrationsAsync();

        await app.Services.ApplyMultitenancyMigrationsAsync();

        await app.Services.ApplyPortalMigrationsAsync();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseWebAssemblyDebugging();
        }
        else
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);

            app.UseHsts();
        }

        app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

        app.UseHttpsRedirection();

        app.UseRouting();
        app.UseMultiTenant();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseAntiforgery();

        app.UseStaticFiles();

        app.MapStaticAssets();

        app.MapRazorPages();

        app.MapReverseProxy().RequireAuthorization("KerridgeProxyAuth");

        app.MapControllers();

        app.MapPearRoutes(builder.Configuration);

        app.AddBluQubeApi();

        app.AddAuthenticationApi();

        app.AddMultitenancyApi()
            .AddPearDropAuthenticationMultitenancyApi();

        app.MapRazorComponents<App>()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(IBMG.SCS.Portal.Web.Client._Imports).Assembly);

        await app.RunAsync();
    }
}