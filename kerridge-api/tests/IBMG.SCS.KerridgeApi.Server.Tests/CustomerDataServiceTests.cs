using FluentAssertions;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.KerridgeApi.Server.AppDbContext;
using IBMG.SCS.KerridgeApi.Server.KerridgeResponseModels;
using IBMG.SCS.KerridgeApi.Server.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class CustomerDataServiceTests
{
    private readonly CustomerDataService _service;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<IKerridgeRoutingService> _routingServiceMock;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<IConfiguration> _configMock;
    private Mock<IbmgDwhDbContext> _dbContextMock;
    private Mock<PortalDBContext> _apiDbContextMock;

    public CustomerDataServiceTests()
    {
        var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri("https://example.com");

        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _httpClientFactoryMock
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        _routingServiceMock = new Mock<IKerridgeRoutingService>();
        _routingServiceMock
            .Setup(x => x.GetBaseUrlForBranchAsync(It.IsAny<string>()))
            .ReturnsAsync("https://example.com");

        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        var context = new DefaultHttpContext();
        context.Items["BranchCode"] = "001";
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

        _configMock = new Mock<IConfiguration>();
        _configMock.Setup(x => x["KerridgeSettings:001"]).Returns("https://example.com");

        _dbContextMock = new Mock<IbmgDwhDbContext>();

        _service = new CustomerDataService(
            _httpClientFactoryMock.Object,
            _configMock.Object,
            _routingServiceMock.Object,
            _httpContextAccessorMock.Object,
            _dbContextMock.Object,
            _apiDbContextMock.Object
        );
    }

    [Fact]
    public async Task ListCustomersAsync_returns_list()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(
                    """
                {
                "response": {
                    "results": [
                        { "account_1": "CUST-001", "name_1": "John Doe", "active_0": 1 },
                        { "account_1": "CUST-002", "name_1": "Jane Smith", "active_0": 0 }
                    ]
                }
                }
                """,
                    Encoding.UTF8,
                    "application/json"),
            });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("https://dummy-url.com/")
        };

        _httpClientFactoryMock
            .Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        // Call method without parameter
        var result = await _service.ListCustomersAsync();

        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count.Should().Be(2);

        result.Data[0].AccountNumber[0].Should().Be("CUST-001");
        result.Data[0].Name.Should().Be("John Doe");
        result.Data[0].Active.Should().BeTrue();

        result.Data[0].AccountNumber[0].Should().Be("CUST-001");
        result.Data[1].Name.Should().Be("Jane Smith");
        result.Data[1].Active.Should().BeFalse();
    }

    [Fact]
    public async Task GetCustomerAsync_returns_customer()
    {
        var result = await _service.GetCustomerAsync(001);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task ListCustomerOrdersAsync_returns_list()
    {
        var result = await _service.ListCustomerOrdersAsync(001, null, null);

        result.Should().NotBeNull();
        result.Should().BeOfType<List<OrderDto>>();
    }

    [Fact]
    public async Task CountCustomerOrdersAsync_returns_count()
    {
        var result = await _service.CountCustomerOrdersAsync(001);

        result.Data.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetCustomerOrderAsync_returns_order()
    {
        var result = await _service.GetCustomerOrderAsync(001, "ORD-001");

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCustomerSpendSummaryAsync_returns_summary()
    {
        var result = await _service.GetCustomerSpendSummaryAsync("CUST-001");

        result.Should().NotBeNull();
        result.Should().BeOfType<SpendSummaryDto>();
    }

    [Fact]
    public async Task ListCustomerSpendAsync_returns_list()
    {
        var result = await _service.ListCustomerSpendAsync(001);

        result.Should().NotBeNull();
        result.Should().BeOfType<List<CustomerBySpendDto>>();
    }

    [Fact]
    public async Task GetCustomerOperativeSpendAsync_returns_spend()
    {
        var result = await _service.GetCustomerOperativeSpendAsync("CUST-001", "OP-001");

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task GetKpiOtifAsync_returns_kpi()
    {
        var result = await _service.GetCustomerOtifKpiAsync("CUST-001");

        result.Should().NotBeNull();
        result.Should().BeOfType<OtifKpiDto>();
    }

    [Fact]
    public async Task GetKpiInvoiceAccuracyAsync_returns_kpi()
    {
        var result = await _service.GetKpiInvoiceAccuracyAsync("CUST-001");

        result.Should().NotBeNull();
        result.Should().BeOfType<InvoiceAccuracyKpiDto>();
    }

    [Fact]
    public async Task GetKpiFaultyGoodsAsync_returns_kpi()
    {
        var result = await _service.GetKpiFaultyGoodsAsync("CUST-001");

        result.Should().NotBeNull();
        result.Should().BeOfType<FaultyGoodsKpiDto>();
    }
}