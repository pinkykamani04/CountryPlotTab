// Copyright (c) IBMG. All rights reserved.

using IBMG.SCS.KerridgeApi.Server.KerridgeRequestModels;
using IBMG.SCS.KerridgeApi.Server.Models;
using IBMG.SCS.KerridgeApi.Server.Services;
using Microsoft.Graph.Communications.CallRecords.MicrosoftGraphCallRecordsGetDirectRoutingCallsWithFromDateTimeWithToDateTime;

namespace IBMG.SCS.KerridgeApi.Server;
/// <summary>
/// Extension methods for configuring REST API and services.
/// </summary>
public static class RestApiExtensions
{
    /// <summary>
    /// Adds customer data services and OpenAPI configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddCustomerDataServices(this IServiceCollection services)
    {
        services.AddScoped<CustomerDataService>();
        return services;
    }

    /// <summary>
    /// Maps customer REST API endpoints.
    /// </summary>
    /// <param name="app">The web application.</param>
    /// <returns>The web application for chaining.</returns>
    public static WebApplication MapCustomerRestEndpoints(this WebApplication app)
    {
        // REST Endpoints (Minimal API)
        app.MapGet("/api/v1/customers", async (CustomerDataService svc) =>
            await svc.ListCustomersAsync())
            .WithName("ListCustomers")
            .WithTags("Customers")
            .WithSummary("Lists all customers for branch portal")
            .WithDescription("Retrieve a list of all customers")
            .RequireAuthorization();

        app.MapGet("/api/v1/customer/portal/customers", async (CustomerDataService svc) =>
           await svc.ListCustomersForCustomerPortalAsync())
           .WithName("ListCustomerPortalCustomers")
           .WithTags("Customers")
           .WithSummary("Lists all customers for customer portal")
           .WithDescription("Retrieve a list of all customers")
           .RequireAuthorization();

        app.MapGet("/api/v1/customer/{customerId}", async (int customerId, CustomerDataService svc) =>
            await svc.GetCustomerAsync(customerId))
            .WithName("GetCustomer")
            .WithTags("Customers")
            .WithSummary("Get customer details")
            .WithDescription("Details of a single customer")
            .RequireAuthorization();

        app.MapGet("/api/v1/customer/{customerId}/orders", async (int customerId, DateOnly? fromDate, DateOnly? toDate, string? filterBy, string? orderBy, string? groupBy, CustomerDataService svc) =>
            await svc.ListCustomerOrdersAsync(customerId, fromDate, toDate, filterBy, orderBy, groupBy))
            .WithName("ListCustomerOrders")
            .WithTags("Orders")
            .WithSummary("Lists orders for a customer")
            .WithDescription("Lists the orders for a single customer")
            .RequireAuthorization();

        app.MapPost("/api/v1/order/add-branch-order", async (RequestSalesOrderDto request, CustomerDataService svc) =>
            await svc.PlaceOrderAsync(request))
            .WithName("AddBranchOrder")
            .WithTags("Orders")
            .WithSummary("Add order after successful validation")
            .WithDescription("Add order after the validation is successful")
            .RequireAuthorization();

        app.MapGet("/api/v1/customer/{customerAccountNumber}/order/get-branch-Order", async (string customerAccountNumber, CustomerDataService svc) =>
            await svc.GetBranchOrderAsync(customerAccountNumber))
            .WithName("GetBranchOrder")
            .WithTags("Orders")
            .WithSummary("Get order for the branch portal")
            .WithDescription("Get ordes for the branch portal")
            .RequireAuthorization();

        app.MapGet("/api/v1/customer/{customerId}/orders/count", async (int customerId, CustomerDataService svc) =>
            await svc.CountCustomerOrdersAsync(customerId))
            .WithName("CountCustomerOrders")
            .WithTags("Orders")
            .WithSummary("Count customer orders")
            .WithDescription("Lists the count of orders for a single customer")
            .RequireAuthorization();

        app.MapGet("/api/v1/customer/{customerId}/order/{orderId}", async (int customerId, string orderId, CustomerDataService svc) =>
            await svc.GetCustomerOrderAsync(customerId, orderId))
            .WithName("GetCustomerOrder")
            .WithTags("Orders")
            .WithSummary("Get order details")
            .WithDescription("Details of a single order")
            .RequireAuthorization();

        app.MapGet("/api/v1/customer/{customerId}/spend/summary", async (string customerId, CustomerDataService svc) =>
            await svc.GetCustomerSpendSummaryAsync(customerId))
            .WithName("GetCustomerSpendSummary")
            .WithTags("Spend")
            .WithSummary("Get spend summary")
            .WithDescription("Gets the spend details for a specific customer (total spend, credit limit, available credit, total sales)")
            .RequireAuthorization();

        app.MapGet("/api/v1/customer/{customerId}/spend", async (int customerId, string ? dateFilter, CustomerDataService svc) =>
            await svc.ListCustomerSpendAsync(customerId, dateFilter))
            .WithName("ListCustomerSpend")
            .WithTags("Spend")
            .WithSummary("List customer spend")
            .WithDescription("Lists the spend for a single customer by day for a time period between two datetimes")
            .RequireAuthorization();

        app.MapGet("/api/v1/customer/{customerId}/spend/operative/{operativeId}", async (string customerId, string operativeId, CustomerDataService svc) =>
            await svc.GetCustomerOperativeSpendAsync(customerId, operativeId))
            .WithName("GetCustomerOperativeSpend")
            .WithTags("Spend")
            .WithSummary("Get operative spend details")
            .WithDescription("Details of the spend of a single customer specific operative (using order point account id)")
            .RequireAuthorization();

        app.MapGet("/api/v1/operative/{operativeId}/spend-limits", async (string operativeId, CustomerDataService svc) =>
            await svc.GetSpendLimitsByOperativeAsync(operativeId))
            .WithName("GetSpendLimitsByOperative")
            .WithTags("Spend")
            .WithSummary("Lists spend for the operative")
            .WithDescription("Retrieve a list of spend limits for the given opearative")
            .RequireAuthorization();

        app.MapGet("/api/v1/trade-Card-number/{tradeCardNumber}/spend-limits", async (string tradeCardNumber, CustomerDataService svc) =>
            await svc.GetSpendLimitsByTradeCardAsync(tradeCardNumber))
            .WithName("GetSpendLimitsByTradeCardNumber")
            .WithTags("Spend")
            .WithSummary("Lists spend for the trade card number")
            .WithDescription("Retrieve a list of spend limits for the given trade crad number")
            .RequireAuthorization();

        app.MapGet("/api/v1/customer/{customerId}/kpis/otif", async (string customerId, CustomerDataService svc) =>
            await svc.GetCustomerOtifKpiAsync(customerId))
            .WithName("GetKpiOtif")
            .WithTags("KPIs")
            .WithSummary("Get OTIF KPI")
            .WithDescription("OTIF KPI Widget")
            .RequireAuthorization();

        app.MapGet("/api/v1/customer/{customerId}/kpis/invoice-accuracy", async (string customerId, CustomerDataService svc) =>
            await svc.GetKpiInvoiceAccuracyAsync(customerId))
            .WithName("GetKpiInvoiceAccuracy")
            .WithTags("KPIs")
            .WithSummary("Get Invoice Accuracy KPI")
            .WithDescription("Invoice Accuracy KPI")
            .RequireAuthorization();

        app.MapGet("/api/v1/customer/{customerId}/kpis/faulty-goods", async (string customerId, CustomerDataService svc) =>
            await svc.GetKpiFaultyGoodsAsync(customerId))
            .WithName("GetKpiFaultyGoods")
            .WithTags("KPIs")
            .WithSummary("Get Faulty Goods KPI")
            .WithDescription("Faulty Goods Report KPI")
            .RequireAuthorization();

        app.MapGet("/api/v1/customer/{customerId}/operatives/status/{operativeStatus}", async (string customerId, int operativeStatus, CustomerDataService svc) =>
            await svc.GetOperativeStatusAsync(customerId, operativeStatus))
            .WithName("GetOperativeStatus")
            .WithTags("Spend")
            .WithSummary("Get Operative Status")
            .WithDescription("Operative Status")
            .RequireAuthorization();

        app.MapGet("/api/v1/customer/{customerAccountNumber}/spend/today", async (string customerAccountNumber, CustomerDataService svc) =>
            await svc.GetTodaysSpendAsync(customerAccountNumber))
            .WithName("GetTodaysSpend")
            .WithTags("Spend")
            .WithSummary("Get today's spend for a customer")
            .WithDescription("Returns the total spend for a customer for the current day.")
            .RequireAuthorization();

        app.MapGet("/api/v1/customer/{customerAccountNumber}/spend-limit/{newSpendLimit}", async (string customerAccountNumber, decimal newSpendLimit, CustomerDataService svc) =>
            await svc.NewSpendLimitAsync(customerAccountNumber, newSpendLimit))
            .WithName("NewSpendLimit")
            .WithTags("Spend")
            .WithSummary("New spend limit for a customer")
            .WithDescription("Sets a new spend limit for a customer.")
            .RequireAuthorization();

        app.MapGet("/api/v1/customer/{customerAccountNumber}/operative/{operativeId}/validate", async (string customerAccountNumber, string operativeId, CustomerDataService svc) =>
            await svc.ValidateOperativeByOperativeIdAsync(customerAccountNumber, operativeId))
            .WithName("ValidateOperative")
            .WithTags("Validate")
            .WithSummary("Validate operative for a customer")
            .WithDescription("Checks if an operative ID is valid for a given customer account code.")
            .RequireAuthorization();

        app.MapGet("/api/v1/customer/{customerAccountNumber}/trade-card/{tradeCardNumber}/validate", async (string customerAccountNumber, string tradeCardNumber, CustomerDataService svc) =>
            await svc.ValidateOperativeByTradeCardNumberAsync(customerAccountNumber, tradeCardNumber))
            .WithName("ValidateTradeCardNumber")
            .WithTags("Validate")
            .WithSummary("Validate trade card for a customer")
            .WithDescription("Checks if a trade card number is valid for a given customer account code.")
            .RequireAuthorization();

        app.MapGet("/api/v1/customers/by-branch/{branchCode}/salesperson/{salespersonCode}", async (string branchCode, string salespersonCode, CustomerDataService svc) =>
            await svc.GetCustomersByBranchAndSalespersonAsync(branchCode, salespersonCode))
            .WithName("GetCustomersByBranchAndSalesperson")
            .WithTags("Customers")
            .WithSummary("Get customers for a branch and salesperson")
            .WithDescription("Returns a list of customer accounts for a given branch code and salesperson code.")
            .RequireAuthorization();

        app.MapGet("/api/v1/customer/{customerAccountNumber}/job/{jobNumber}", async (string customerAccountNumber, string jobNumber, CustomerDataService svc) =>
            await svc.ValidateJobDetailsAsync(customerAccountNumber, jobNumber))
            .WithName("ValidateJobNumber")
            .WithTags("Validate")
            .WithSummary("Validate job number for a customer")
            .WithDescription("Checks if an job number is valid for a given customer account code.")
            .RequireAuthorization();

        app.MapGet("/api/v1/customer/{customerAccountNumber}/job/{jobNumber}/operative/{operativeId}/trade-crad/{tradeCardNumber}", async (string customerAccountNumber, string jobNumber, string operativeId, string tradeCardNumber, CustomerDataService svc) =>
            await svc.ValidateAllAsync(customerAccountNumber, jobNumber, operativeId, tradeCardNumber))
            .WithName("ValidataeAll")
            .WithTags("Validate")
            .WithSummary("Validate all")
            .WithDescription("Checks if an operative ID is valid for a given customer account code.")
            .RequireAuthorization();

        app.MapGet("api/v1/customer/operatives-by-spend", async (int customerId, string? dateFilter, CustomerDataService svc) =>
            await svc.GetOpertaiveBySpend(customerId,dateFilter))
            .WithName("GetOpertaiveBySpend")
            .WithTags("Spend")
            .WithSummary("Get operatives sorted by spend")
            .WithDescription("Returns a list for operatives and their spend details.")
            .RequireAuthorization();

        app.MapGet("api/v1/customer/branches-by-spend", async (int customerId, string? dateFilter, CustomerDataService svc) =>
            await svc.GetBranchesBySpend(customerId, dateFilter))
            .WithName("GetBranchesBySpend")
            .WithTags("Spend")
            .WithSummary("Get branches sorted by spend")
            .WithDescription("Returns a list for branches and their spend details.")
            .RequireAuthorization();

        app.MapGet("api/v1/customer/products-by-spend", async (int customerId, string? dateFilter, CustomerDataService svc) =>
            await svc.GetProductsBySpend(customerId, dateFilter))
            .WithName("GetProductsBySpend")
            .WithTags("Spend")
            .WithSummary("Get products sorted by spend")
            .WithDescription("Returns a list for products and their spend details.")
            .RequireAuthorization();

        app.MapGet("api/v1/customer/categories-by-spend", async (int customerId, string? dateFilter, CustomerDataService svc) =>
            await svc.GetCategoryBySpend(customerId, dateFilter))
            .WithName("GetCategoryBySpend")
            .WithTags("Spend")
            .WithSummary("Get categories sorted by spend")
            .WithDescription("Returns a list for categories and their spend details.")
            .RequireAuthorization();

        return app;
    }
}