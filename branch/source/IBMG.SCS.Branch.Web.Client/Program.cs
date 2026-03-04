// Copyright (c) IBMG. All rights reserved.

using BluQube.Attributes;
using BluQube.Commands;
using BluQube.Queries;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Radzen;
using KerridgeApiClient = IBMG.SCS.Branch.Web.Client.Kerridge.Client;

namespace IBMG.SCS.Branch.Web.Client;

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

        builder.Services.AddHttpClient(
            "bluqube",
            client => { client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress); });

        // Use Portal.Web as proxy for Kerridge API (handles authentication server-side)
        var kerridgeProxyUrl = builder.HostEnvironment.BaseAddress + "api/kerridge";

        builder.Services.AddTransient<CommandResultConverter>();

        builder.Services.AddMediatR(
            configuration => configuration.RegisterServicesFromAssemblies(
                typeof(Routes).Assembly));

        builder.Services.AddBluQubeRequesters();
        builder.Services.AddScoped<KerridgeBranchService>();

        builder.Services.AddScoped<KerridgeApiClient>(sp =>
        {
            var nav = sp.GetRequiredService<NavigationManager>();

            var baseUrl = new Uri(new Uri(nav.BaseUri), "api/kerridge/");

            var http = new HttpClient
            {
                BaseAddress = baseUrl
            };

            var branchService = sp.GetRequiredService<KerridgeBranchService>();
            return new KerridgeApiClient(http, branchService);
        });

        // Info : Using HTTPCLient directly calling endpoint route not api

        //builder.Services.AddHttpClient("KerridgeApiServer", client =>
        //{
        //    client.BaseAddress = new Uri("https://localhost:7017/");
        //});

        //builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("KerridgeApiServer"));

        //builder.Services.AddScoped(sp =>
        //{
        //    var factory = sp.GetRequiredService<IHttpClientFactory>();
        //    var http = factory.CreateClient("KerridgeApiServer");

        //    http.DefaultRequestHeaders.Remove("X-Branch-Code");
        //    http.DefaultRequestHeaders.Add("X-Branch-Code", "LON01");

        //    return http;
        //});

        await builder.Build().RunAsync();
    }
}