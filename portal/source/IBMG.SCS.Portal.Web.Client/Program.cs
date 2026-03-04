// Copyright (c) IBMG. All rights reserved.

using BluQube.Attributes;
using BluQube.Commands;
using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Handlers;
using IBMG.SCS.Portal.Web.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PearDrop.Authentication.Client;
using PearDrop.Authentication.Multitenancy.Client;
using PearDrop.Multitenancy.Client;
using Radzen;

namespace IBMG.SCS.Portal.Web.Client;

[BluQubeRequester]
public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);

        builder.Services.AddAuthorizationCore();
        builder.Services.AddAuthenticationStateDeserialization();
        builder.Services.AddCascadingAuthenticationState();
        builder.Services.AddRadzenComponents();
        builder.Services.AddScoped<ICommander, Commander>();
        builder.Services.AddScoped<IQuerier, Querier>();
        builder.Services.AddHttpClient("bluqube", client =>
        {
            client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
        });

        // Use Portal.Web as proxy for Kerridge API (handles authentication server-side)
        var kerridgeProxyUrl = builder.HostEnvironment.BaseAddress + "api/kerridge";

        builder.Services.AddSingleton<KerridgeBranchService>();
        builder.Services.AddTransient<KerridgeBranchHandler>();

        builder.Services.AddHttpClient("kerridge", client =>
        {
            client.BaseAddress = new Uri(kerridgeProxyUrl);
        })
        .AddHttpMessageHandler<KerridgeBranchHandler>();

        builder.Services.AddScoped<ApiClient.IClient>(sp =>
        {
            var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient("kerridge");

            var client = new ApiClient.Client(kerridgeProxyUrl, http)
            {
                ReadResponseAsString = true,
            };

            return client;
        });
        builder.Services.AddTransient<CommandResultConverter>();
        builder.Services.AddMemoryCache();
        builder.Services.AddScoped<DashboardDataService>();
        builder.Services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(typeof(Routes).Assembly)
        );

        builder.Services.AddBluQubeRequesters()
            .AddPearDropAuthentication()
            .AddPearDropAuthenticationMultitenancy()
            .AddPeardropMultitenancy();

        await builder.Build().RunAsync();
    }
}
