// Copyright (c) IBMG. All rights reserved.

using BluQube.Attributes;
using BluQube.Commands;
using BluQube.Queries;
using FluentValidation;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Services;
using IBMG.SCS.Branch.Web.Components;
using IBMG.SCS.Branch.Web.Infrastructure.ReverseProxy;
using MediatR.Behaviors.Authorization.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Identity.Web;
using System.Reflection;
using Yarp.ReverseProxy.Transforms.Builder;
using KerridgeApiClient = IBMG.SCS.Branch.Web.Client.Kerridge.Client;

namespace IBMG.SCS.Branch.Web;

[BluQubeResponder]
public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var physicalProvider = builder.Environment.ContentRootFileProvider;
        var embeddedProvider = new EmbeddedFileProvider(Assembly.GetEntryAssembly()!);
        var compositeProvider = new CompositeFileProvider(physicalProvider, embeddedProvider);

        builder.Services.AddSingleton<IFileProvider>(compositeProvider);

        builder.Configuration
            .AddJsonFile($"appsettings.{Environment.MachineName}.json", true, true);

        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveWebAssemblyComponents()
            .AddAuthenticationStateSerialization(
                options => options.SerializeAllClaims = true);

        builder.Services.AddCascadingAuthenticationState();

        builder.Services.AddRazorPages();
        builder.Services.AddControllers();

        builder.Services.AddAntiforgery(options =>
        {
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        });

        builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme).AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

        builder.Services.AddAuthorization(options =>
        {
            options.FallbackPolicy = options.DefaultPolicy;
        });

        builder.Services.AddScoped<ICommander, Commander>();
        builder.Services.AddScoped<IQuerier, Querier>();
        builder.Services.AddValidatorsFromAssembly(typeof(App).Assembly);

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(App).Assembly));
        builder.Services.AddMediatorAuthorization(typeof(App).Assembly);
        builder.Services.AddAuthorizersFromAssembly(typeof(App).Assembly);
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<KerridgeBranchService>();

        // YARP: load configuration and token transform
        builder.Services.AddReverseProxy()
            .AddTransforms<KerridgeAuthTransform>()
            .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

        builder.Services.AddScoped<KerridgeApiClient>(sp =>
        {
            var nav = sp.GetRequiredService<NavigationManager>();

            var baseUrl = new Uri(new Uri(nav.BaseUri), "api/kerridge");

            var http = new HttpClient
            {
                BaseAddress = baseUrl,
            };

            var branchService = sp.GetRequiredService<KerridgeBranchService>();
            return new KerridgeApiClient(http, branchService);
        });

        builder.Services.Configure<JsonOptions>(options =>
        {
            options.AddBluQubeJsonConverters();
        });

        var app = builder.Build();

        var fwd = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedFor,
        };

        app.UseForwardedHeaders(fwd);

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

        app.UseHttpsRedirection();

        app.UseStaticFiles();
        app.MapStaticAssets();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseAntiforgery();
        app.MapReverseProxy();

        app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

        app.MapRazorPages();

        app.MapRazorComponents<App>()
            .AddInteractiveWebAssemblyRenderMode()
            .AddAdditionalAssemblies(typeof(IBMG.SCS.Branch.Web.Client._Imports).Assembly);

        app.AddBluQubeApi();

        app.Run();
    }
}