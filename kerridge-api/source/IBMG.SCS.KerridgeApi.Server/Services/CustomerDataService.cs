// Copyright (c) IBMG. All rights reserved.

using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Infrastructure.Entities;
using IBMG.SCS.KerridgeApi.Server.AppDbContext;
using IBMG.SCS.KerridgeApi.Server.KerridgeDwhModels;
using IBMG.SCS.KerridgeApi.Server.KerridgeRequestModels;
using IBMG.SCS.KerridgeApi.Server.KerridgeResponseModels;
using IBMG.SCS.KerridgeApi.Server.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace IBMG.SCS.KerridgeApi.Server.Services;

public class CustomerDataService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IKerridgeRoutingService _routingService;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IConfiguration _config;
    private readonly IbmgDwhDbContext _dbContext;
    private readonly PortalDBContext _testDbContext;

    public CustomerDataService(
    IHttpClientFactory httpClientFactory,
    IConfiguration config,
    IKerridgeRoutingService routingService,
    IHttpContextAccessor contextAccessor,
    IbmgDwhDbContext dbContext,
    PortalDBContext testDbContext)
    {
        this._httpClientFactory = httpClientFactory;
        this._config = config;
        this._routingService = routingService;
        this._contextAccessor = contextAccessor;
        this._dbContext = dbContext;
        this._testDbContext = testDbContext;
    }

    private async Task<HttpClient?> CreateClientAsync()
    {
        var branchCode = this._contextAccessor.HttpContext!.Items["BranchCode"]?.ToString();

        if (string.IsNullOrEmpty(branchCode))
        {
            return null;
        }

        var instanceCode = await this._routingService.GetBaseUrlForBranchAsync(branchCode);

        if (string.IsNullOrEmpty(instanceCode))
        {
            return null;
        }

        var baseUrl = this._config[$"KerridgeSettings:{instanceCode}"];

        if (string.IsNullOrEmpty(baseUrl))
        {
            return null;
        }

        var client = this._httpClientFactory.CreateClient();
        client.BaseAddress = new Uri(baseUrl);
        return client;
    }

    //kerridge apis

    public async Task<ApiResponse<List<CustomerDto>>> ListCustomersAsync()
    {
        var branchCode = this._contextAccessor.HttpContext!.Items["BranchCode"]?.ToString();

        var client = await this.CreateClientAsync();

        if (client == null)
        {
            return new ApiResponse<List<CustomerDto>>
            {
                Success = false,
                Message = "No kerridge instance found for branch.",
                Data = new List<CustomerDto>(),
            };
        }

        var req = new RequestCustomerListDto(
            Level: 11,
            Levelcode: "0001",
            Branchco: branchCode);

        var response = await client.PostAsJsonAsync("REPORTS/XI_SCSLISTCUSTOMERS", req);

        if (!response.IsSuccessStatusCode)
        {
            return new ApiResponse<List<CustomerDto>>
            {
                Success = false,
                Message = $"Kerridge API failed: {response.StatusCode}",
                Data = new List<CustomerDto>(),
            };
        }

        var result = await response.Content.ReadFromJsonAsync<ResponseCustomerListDto>();

        var mapped = result?.Response.Results.Select(x => new CustomerDto
        {
            AccountNumber = [x.Account_1],
            Name = x.Name_1,
            Active = x.Active_0 == 1,
        }).ToList();

        return new ApiResponse<List<CustomerDto>>
        {
            Success = true,
            Message = "OK",
            Data = mapped ?? new List<CustomerDto>(),
        };
    }

    public async Task<ApiResponse<List<SpendLimitResponseDto>>> NewSpendLimitAsync(string customerAccountNumber, decimal newSpendLimit)
    {
        var client = await this.CreateClientAsync();

        if (client == null)
        {
            return new ApiResponse<List<SpendLimitResponseDto>>
            {
                Success = false,
                Message = "No Kerridge instance found for the selected branch.",
                Data = new List<SpendLimitResponseDto>(),
            };
        }

        var req = new RequestNewSpendLimitDto(
            Level: 11,
            Levelcode: "0001",
            Custacc: customerAccountNumber,
            Spendlim: newSpendLimit);

        var response = await client.PostAsJsonAsync("REPORTS/XI_SCSNEWSPENDLIMIT", req);

        if (!response.IsSuccessStatusCode)
        {
            return new ApiResponse<List<SpendLimitResponseDto>>
            {
                Success = false,
                Message = $"Kerridge API failed: {response.StatusCode}",
                Data = new List<SpendLimitResponseDto>(),
            };
        }

        var result = await response.Content.ReadFromJsonAsync<ResponseSpenLimitDto>();

        var mapped = result?.Response?.Results?.Select(x => new SpendLimitResponseDto
        {
            CustomerId = x.Custacc_0,
            NewSpendLimit = x.Spendlim_0,
        }).ToList() ?? new List<SpendLimitResponseDto>();

        return new ApiResponse<List<SpendLimitResponseDto>>
        {
            Success = true,
            Message = "OK",
            Data = mapped,
        };
    }

    public async Task<ApiResponse<DaySpendDto>> GetTodaysSpendAsync(string customerAccountNumber)
    {
        var client = await this.CreateClientAsync();

        if (client == null)
        {
            return new ApiResponse<DaySpendDto>
            {
                Success = false,
                Message = "No Kerridge instance found for the selected branch.",
                Data = new DaySpendDto(),
            };
        }

        var req = new RequestTodaysSpendDto(
            Level: 11,
            Levelcode: "0001",
            Custacc: customerAccountNumber,
            Date: DateOnly.FromDateTime(DateTime.Now));

        var response = await client.PostAsJsonAsync("REPORTS/XI_SCSTODAYSSPEND", req);

        if (!response.IsSuccessStatusCode)
        {
            return new ApiResponse<DaySpendDto>
            {
                Success = false,
                Message = $"Kerridge API failed: {response.StatusCode}",
                Data = new DaySpendDto(),
            };
        }

        var result = await response.Content.ReadFromJsonAsync<ResponseDaySpendDto>();

        var mapped = new DaySpendDto
        {
            TotalSpendForDay = result?.Response?.Grandtotals?.Goods_2 ?? 0,
        };

        return new ApiResponse<DaySpendDto>
        {
            Success = true,
            Message = "OK",
            Data = mapped,
        };
    }

    public async Task<ApiResponse<List<ValidationDto>>> ValidateOperativeByOperativeIdAsync(string customerAccountNumber, string operativeId)
    {
        var client = await this.CreateClientAsync();

        if (client == null)
        {
            return new ApiResponse<List<ValidationDto>>
            {
                Success = false,
                Message = "No Kerridge instance found for the selected branch.",
                Data = new List<ValidationDto>(),
            };
        }

        var req = new RequestOperativeValidationDto(
            Level: 11,
            Levelcode: "0001",
            Custacc: customerAccountNumber,
            Opid: operativeId);

        var response = await client.PostAsJsonAsync($"REPORTS/XI_SCSVALIDATEOPERA", req);

        if (!response.IsSuccessStatusCode)
        {
            return new ApiResponse<List<ValidationDto>>
            {
                Success = false,
                Message = $"Kerridge API failed: {response.StatusCode}",
                Data = new List<ValidationDto>(),
            };
        }

        var rawJson = await response.Content.ReadAsStringAsync();
        Console.WriteLine(rawJson);

        var result = await response.Content.ReadFromJsonAsync<ResponseOperativeValidationDto>();

        var mapped = result?.Response?.Results?.Select(x => new ValidationDto
        {
            FirstName = x.Firstname_1,
            Surname = x.Surname_1,
            Valid = x.Activestatus_1,
        }).ToList() ?? new List<ValidationDto>();

        return new ApiResponse<List<ValidationDto>>
        {
            Success = true,
            Message = "OK",
            Data = mapped,
        };
    }

    public async Task<ApiResponse<List<ValidationDto>>> ValidateOperativeByTradeCardNumberAsync(string customerAccountNumber, string tradeCardNumbeer)
    {
        var client = await this.CreateClientAsync();

        if (client == null)
        {
            return new ApiResponse<List<ValidationDto>>
            {
                Success = false,
                Message = "No Kerridge instance found for the selected branch.",
                Data = new List<ValidationDto>(),
            };
        }

        var req = new RequestTradeCardValidationDto(
            Level: 11,
            Levelcode: "0001",
            Custacc: customerAccountNumber,
            Tradecar: tradeCardNumbeer);

        var response = await client.PostAsJsonAsync("REPORTS/XI_SCSVALIDATETRADE", req);

        if (!response.IsSuccessStatusCode)
        {
            return new ApiResponse<List<ValidationDto>>
            {
                Success = false,
                Message = $"Kerridge API failed: {response.StatusCode}",
                Data = new List<ValidationDto>(),
            };
        }

        var result = await response.Content.ReadFromJsonAsync<ResponseTradeCardValidationDto>();

        var mapped = result?.Response?.Results?.Select(x => new ValidationDto
        {
            FirstName = x.Firstname_1,
            Surname = x.Surname_1,
            Valid = x.Activestatus_1,
        }).ToList() ?? new List<ValidationDto>();

        return new ApiResponse<List<ValidationDto>>
        {
            Success = true,
            Message = "OK",
            Data = mapped,
        };
    }

    public async Task<ApiResponse<List<JobDetailsDto>>> ValidateJobDetailsAsync(string customerAccountNumber, string jobNumber)
    {
        var client = await this.CreateClientAsync();

        if (client == null)
        {
            return new ApiResponse<List<JobDetailsDto>>
            {
                Success = false,
                Message = "No Kerridge instance found for the selected branch.",
                Data = new List<JobDetailsDto>(),
            };
        }

        var req = new RequestValidateJobDetailsDto(
            Level: 11,
            Levelcode: "0001",
            Custacc: customerAccountNumber,
            Jobnum: jobNumber);

        var response = await client.PostAsJsonAsync("REPORTS/XI_SCSVALIDATEJOBN", req);

        if (!response.IsSuccessStatusCode)
        {
            return new ApiResponse<List<JobDetailsDto>>
            {
                Success = false,
                Message = $"Kerridge API failed: {response.StatusCode}",
                Data = new List<JobDetailsDto>(),
            };
        }

        var result = await response.Content.ReadFromJsonAsync<ResponseJobDetailsDto>();

        var mapped = result?.Response?.Results?.Select(x => new JobDetailsDto
        {
            JobDescription = x.Jobdesc_1,
            CustomerName = x.Name_2,
            Active = x.Active_0 == 1,
            Spend = x.Spend_1,
        }).ToList() ?? new List<JobDetailsDto>();

        if (mapped == null || mapped.Count == 0)
        {
            return new ApiResponse<List<JobDetailsDto>>
            {
                Success = false,
                Message = "No job details found for the given customer and job number.",
                Data = new List<JobDetailsDto>(),
            };
        }

        return new ApiResponse<List<JobDetailsDto>>
        {
            Success = true,
            Message = "OK",
            Data = mapped,
        };
    }

    public async Task<ApiResponse<List<ValidateAllDto>>> ValidateAllAsync(string customerAccountNumber, string jobNumber, string operativeId, string tradeCardNumber)
    {
        var client = await this.CreateClientAsync();

        if (client == null)
        {
            return new ApiResponse<List<ValidateAllDto>>
            {
                Success = false,
                Message = "No Kerridge instance found for the selected branch.",
                Data = new List<ValidateAllDto>(),
            };
        }

        var req = new RequestValidateaAllDto(
            Level: 11,
            Levelcode: "0001",
            Custacc: customerAccountNumber,
            Jobnumbe: jobNumber,
            Opid: operativeId,
            Tradecar: tradeCardNumber);

        var response = await client.PostAsJsonAsync("REPORTS/XI_TESTCOMBINEDREPO", req);

        if (!response.IsSuccessStatusCode)
        {
            return new ApiResponse<List<ValidateAllDto>>
            {
                Success = false,
                Message = $"Kerridge API failed: {response.StatusCode}",
                Data = new List<ValidateAllDto>(),
            };
        }

        var result = await response.Content.ReadFromJsonAsync<ResponseValidateAllDto>();

        var mapped = result?.Response?.Results?.Select(x => new ValidateAllDto
        {
            AccountCode = x.Account_1,
            FirstName = x.Firstname_1,
            Surname = x.Surname_1,
            TradeCardNumber = x.Tradecard_1,
            OperativeId = x.Operativeid_1,
            OperativeTradeStatus = x.Activestatus_1 == 1,
            JobReference = x.Jobref_2,
            JobDescription = x.Jobdesc_2,
            JobReferenceStatus = x.Active_2,
        }).ToList() ?? new List<ValidateAllDto>();

        return new ApiResponse<List<ValidateAllDto>>
        {
            Success = true,
            Message = "OK",
            Data = mapped,
        };
    }

    public async Task<ApiResponse<ResponseSalesOrderDto>> PlaceOrderAsync(RequestSalesOrderDto req)
    {
        var client = await this.CreateClientAsync();

        if (client == null)
        {
            return new ApiResponse<ResponseSalesOrderDto>
            {
                Success = false,
                Message = "No kerridge instance found for branch.",
                Data = new ResponseSalesOrderDto(),
            };
        }

        var response = await client.PostAsJsonAsync("GRANTSTONE/PlaceOrder", req);
        var rawResponse = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return new ApiResponse<ResponseSalesOrderDto>
            {
                Success = false,
                Message = $"Kerridge API failed: {response.StatusCode}",
                Data = new ResponseSalesOrderDto(),
            };
        }

        var result = JsonConvert.DeserializeObject<ResponseSalesOrderDto>(
            rawResponse,
            new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
            });

        return new ApiResponse<ResponseSalesOrderDto>
        {
            Success = true,
            Message = "OK",
            Data = result ?? new ResponseSalesOrderDto(),
        };
    }

    public async Task<ApiResponse<ResponseGetSalesOrderDto>> GetBranchOrderAsync(string customerAccountNumber)
    {
        var client = await this.CreateClientAsync();

        if (client == null)
        {
            return new ApiResponse<ResponseGetSalesOrderDto>
            {
                Success = false,
                Message = "No kerridge instance found for branch.",
                Data = new ResponseGetSalesOrderDto(),
            };
        }

        var req = new RequestBranchOrderDto(
            Account: customerAccountNumber);

        var response = await client.PostAsJsonAsync("GRANTSTONE/GetOrderTrackingHeader", req);

        var rawResponse = await response.Content.ReadAsStringAsync();

        Console.WriteLine("Kerridge raw response:");
        Console.WriteLine(rawResponse);

        if (!response.IsSuccessStatusCode)
        {
            return new ApiResponse<ResponseGetSalesOrderDto>
            {
                Success = false,
                Message = $"Kerridge API failed: {response.StatusCode}",
                Data = new ResponseGetSalesOrderDto(),
            };
        }

        var result = await response.Content.ReadFromJsonAsync<ResponseGetSalesOrderDto>();

        return new ApiResponse<ResponseGetSalesOrderDto>
        {
            Success = true,
            Message = "OK",
            Data = result ?? new ResponseGetSalesOrderDto(),
        };
    }

    //dwh apis

    public async Task<ApiResponse<SpendLimits>> GetSpendLimitsByOperativeAsync(string operativeId)
    {
        var limits = await this._testDbContext.Operatives
        .Where(x => x.OperativeNumber == operativeId)
        .Select(x => new SpendLimits
        {
            DailyLimit = x.DailyLimit,
            MonthlyLimit = x.MonthlyLimit,
            TnxLimit = x.TnxLimit,
            WeeklyLimit = x.WeeklyLimit,
            OverrideDailyLimit = x.OverrideDailyLimit,
            OverrideMonthlyLimit = x.OverrideMonthlyLimit,
            OverrideTnxLimit = x.OverrideTnxLimit,
            OverrideWeeklyLimit = x.OverrideWeeklyLimit,
        })
        .FirstOrDefaultAsync();

        if (limits == null)
        {
            return new ApiResponse<SpendLimits>
            {
                Success = false,
                Message = "Trade card or operative not found",
                Data = new SpendLimits(),
            };
        }

        return new ApiResponse<SpendLimits>
        {
            Success = true,
            Message = "OK",
            Data = limits,
        };
    }

    public async Task<ApiResponse<SpendLimits>> GetSpendLimitsByTradeCardAsync(string tradeCardNumber)
    {
        var limits = await this._testDbContext.Operatives
        .Where(o => this._testDbContext.TradeCards
            .Any(tc => tc.Id == o.TradeCardId && tc.TradeCardNumber == tradeCardNumber))
        .Select(o => new SpendLimits
        {
            DailyLimit = o.DailyLimit,
            MonthlyLimit = o.MonthlyLimit,
            TnxLimit = o.TnxLimit,
            WeeklyLimit = o.WeeklyLimit,
            OverrideDailyLimit = o.OverrideDailyLimit,
            OverrideMonthlyLimit = o.OverrideMonthlyLimit,
            OverrideTnxLimit = o.OverrideTnxLimit,
            OverrideWeeklyLimit = o.OverrideWeeklyLimit,
        })
        .FirstOrDefaultAsync();

        if (limits == null)
        {
            return new ApiResponse<SpendLimits>
            {
                Success = false,
                Message = "Trade card or operative not found",
                Data = new SpendLimits(),
            };
        }

        return new ApiResponse<SpendLimits>
        {
            Success = true,
            Message = "OK",
            Data = limits,
        };
    }

    public async Task<ApiResponse<List<ScsCustomer>>> ListCustomersForCustomerPortalAsync()
    {
        var customers = await this._dbContext.Customers
         .AsNoTracking()
        .Select(c => new ScsCustomer
        {
            Customer_ID = c.Customer_ID,
            Customer_Code = c.Customer_Code,
            Customer_Desc = c.Customer_Desc,
            Credit_Status = c.Credit_Status,
            Credit_Limit = c.Credit_Limit,
            Credit_Available = c.Credit_Available,
        })
        .ToListAsync();

        return new ApiResponse<List<ScsCustomer>>
        {
            Success = true,
            Message = "OK",
            Data = customers,
        };
    }

    public async Task<ApiResponse<ScsCustomer>> GetCustomerAsync(int customerId)
    {
        var customer = await this._dbContext.Customers
        .FirstOrDefaultAsync(x => x.Customer_ID == customerId);

        return new ApiResponse<ScsCustomer>
        {
            Success = true,
            Message = "OK",
            Data = customer,
        };
    }

    public async Task<ApiResponse<object>> ListCustomerOrdersAsync(
        int customerId,
        DateOnly? fromDate,
        DateOnly? toDate,
        string? filterBy = null,
        string? orderBy = null,
        string? groupBy = null)
    {
        var query = _dbContext.SalesAndOrders
            .Where(o =>
                o.Customer_ID == customerId &&
                o.Transaction_Date >= fromDate &&
                o.Transaction_Date <= toDate);
        if (!string.IsNullOrWhiteSpace(filterBy))
        {
            query = query.Where(o =>
                o.Order_Category!.Contains(filterBy) ||
                o.Branch_Desc!.Contains(filterBy) ||
                o.Operative_Desc!.Contains(filterBy) ||
                o.Order_Type.Contains(filterBy));
        }

        if (!string.IsNullOrWhiteSpace(orderBy))
        {
            query = orderBy.ToLower() switch
            {
                "category" => query.OrderBy(o => o.Order_Category),
                "branch" => query.OrderBy(o => o.Branch_Desc),
                "transactiontype" => query.OrderBy(o => o.Order_Type),
                "operativename" => query.OrderBy(o => o.Operative_Desc),
                _ => query,
            };
        }

        if (!string.IsNullOrWhiteSpace(groupBy))
        {
            var groupedQuery = groupBy.ToLower() switch
            {
                "category" => query.GroupBy(o => o.Order_Category),
                "branch" => query.GroupBy(o => o.Branch_Desc),
                "transactiontype" => query.GroupBy(o => o.Order_Type),
                "operativename" => query.GroupBy(o => o.Operative_Desc),
                _ => null,
            };

            if (groupedQuery != null)
            {
                var groupedData = await groupedQuery
                    .Select(g => new
                    {
                        GroupKey = g.Key,
                        TotalSpent = g.Sum(x => x.Invoiced_Sales_Total ?? 0),
                    })
                    .ToListAsync();

                return new ApiResponse<object>
                {
                    Success = true,
                    Message = "OK",
                    Data = groupedData,
                };
            }
        }

        var data = await query
            .Select(o => new
            {
                o.Transaction_Date,
                o.Order_Category,
                o.Branch_Desc,
                o.Order_Type,
                o.Operative_Desc,
                Total = o.Invoiced_Sales_Total ?? 0,
            })
            .ToListAsync();

        return new ApiResponse<object>
        {
            Success = true,
            Message = "OK",
            Data = data,
        };
    }

    public async Task<ApiResponse<int>> CountCustomerOrdersAsync(int customerId)
    {
        var query = this._dbContext.SalesAndOrders
        .Where(o => o.Customer_ID == customerId);

        var count = await query.CountAsync(o => o.Customer_ID == customerId);

        return new ApiResponse<int>
        {
            Success = true,
            Message = "OK",
            Data = count,
        };
    }

    public async Task<ApiResponse<ScsSalesAndOrdersLines>> GetCustomerOrderAsync(int customerId, string orderId)
    {
        var order = await this._dbContext.SalesAndOrdersLines
        .FirstOrDefaultAsync(x => x.Customer_ID == customerId && x.Order_Number.ToString() == orderId);

        return new ApiResponse<ScsSalesAndOrdersLines>
        {
            Success = true,
            Message = "OK",
            Data = order ?? new ScsSalesAndOrdersLines(),
        };
    }

    public async Task<ApiResponse<List<CustomerBySpendDto>>> ListCustomerSpendAsync(int customerId, string? dateFilter = null)
    {
        DateOnly? startDate = null;
        DateOnly? endDate = DateOnly.FromDateTime(DateTime.Today);

        if (!string.IsNullOrWhiteSpace(dateFilter))
        {
            switch (dateFilter.Trim().ToLower())
            {
                case "month to date":
                    startDate = new DateOnly(DateTime.Today.Year, DateTime.Today.Month, 1);
                    break;
                case "year to date":
                    startDate = new DateOnly(DateTime.Today.Year, 1, 1);
                    break;
                case "last year":
                    startDate = new DateOnly(DateTime.Today.Year - 1, 1, 1);
                    endDate = new DateOnly(DateTime.Today.Year - 1, 12, 31);
                    break;
                case "last year to date":
                    startDate = new DateOnly(DateTime.Today.Year - 1, 1, 1);
                    endDate = new DateOnly(DateTime.Today.Year - 1, DateTime.Today.Month, DateTime.Today.Day);
                    break;
                default:
                    startDate = null;
                    break;
            }
        }

        var query = this._dbContext.SalesAndOrders
            .AsNoTracking()
            .Where(b => b.Customer_ID == customerId);

        if (startDate.HasValue)
        {
            query = query.Where(b => b.Transaction_Date >= startDate.Value && b.Transaction_Date <= endDate.Value);
        }

        var spends = await query
        .OrderByDescending(x => x.Transaction_Date)
        .Select(x => new CustomerBySpendDto
        {
            CustomerId = x.Customer_ID,
            CustomerName = x.Customer_Desc,
            CustmomerSpend = x.Invoiced_Sales_Total ?? 0,
        })
        .ToListAsync();

        return new ApiResponse<List<CustomerBySpendDto>>
        {
            Success = true,
            Message = "OK",
            Data = spends,
        };
    }

    public async Task<ApiResponse<List<OperativesBySpend>>> GetOpertaiveBySpend(int customerId, string? dateFilter = null)
    {
        DateOnly? startDate = null;
        DateOnly? endDate = DateOnly.FromDateTime(DateTime.Today);

        if (!string.IsNullOrWhiteSpace(dateFilter))
        {
            switch (dateFilter.Trim().ToLower())
            {
                case "month to date":
                    startDate = new DateOnly(DateTime.Today.Year, DateTime.Today.Month, 1);
                    break;
                case "year to date":
                    startDate = new DateOnly(DateTime.Today.Year, 1, 1);
                    break;
                case "last year":
                    startDate = new DateOnly(DateTime.Today.Year - 1, 1, 1);
                    endDate = new DateOnly(DateTime.Today.Year - 1, 12, 31);
                    break;
                case "last year to date":
                    startDate = new DateOnly(DateTime.Today.Year - 1, 1, 1);
                    endDate = new DateOnly(DateTime.Today.Year - 1, DateTime.Today.Month, DateTime.Today.Day);
                    break;
                default:
                    startDate = null;
                    break;
            }
        }

        var query = this._dbContext.SalesAndOrders
            .AsNoTracking()
            .Where(b => b.Customer_ID == customerId);

        if (startDate.HasValue)
        {
            query = query.Where(b => b.Transaction_Date >= startDate.Value && b.Transaction_Date <= endDate.Value);
        }

        var operatives = await query
            .GroupBy(b => b.Operative_ID)
            .Select(g => new OperativesBySpend
            {
                OperativeId = g.Key,
                OperativeName = g
                .Where(x => x.Operative_Desc != null)
                .Select(x => x.Operative_Desc)
                .FirstOrDefault() ?? "Unknown",
                OperativeSpend = g.Sum(x => x.Invoiced_Sales_Total) ?? 0,
            })
            .OrderByDescending(x => x.OperativeSpend)
            .ToListAsync();

        return new ApiResponse<List<OperativesBySpend>>
        {
            Success = true,
            Message = "OK",
            Data = operatives ?? new List<OperativesBySpend>(),
        };
    }

    public async Task<ApiResponse<List<BranchesBySpend>>> GetBranchesBySpend(int customerId, string? dateFilter = null)
    {
        DateOnly? startDate = null;
        DateOnly? endDate = DateOnly.FromDateTime(DateTime.Today);

        if (!string.IsNullOrWhiteSpace(dateFilter))
        {
            switch (dateFilter.Trim().ToLower())
            {
                case "month to date":
                    startDate = new DateOnly(DateTime.Today.Year, DateTime.Today.Month, 1);
                    break;
                case "year to date":
                    startDate = new DateOnly(DateTime.Today.Year, 1, 1);
                    break;
                case "last year":
                    startDate = new DateOnly(DateTime.Today.Year - 1, 1, 1);
                    endDate = new DateOnly(DateTime.Today.Year - 1, 12, 31);
                    break;
                case "last year to date":
                    startDate = new DateOnly(DateTime.Today.Year - 1, 1, 1);
                    endDate = new DateOnly(DateTime.Today.Year - 1, DateTime.Today.Month, DateTime.Today.Day);
                    break;
                default:
                    startDate = null;
                    break;
            }
        }

        var query = this._dbContext.SalesAndOrders
            .AsNoTracking()
            .Where(b => b.Customer_ID == customerId);

        if (startDate.HasValue)
        {
            query = query.Where(b => b.Transaction_Date >= startDate.Value && b.Transaction_Date <= endDate.Value);
        }

        var branches = await query
            .GroupBy(b => b.Branch_ID)
            .Select(g => new BranchesBySpend
            {
                BranchId = g.Key,
                BranchName = g
                .Where(x => x.Branch_Desc != null)
                .Select(x => x.Branch_Desc)
                .FirstOrDefault() ?? "Unknown",
                BranchSpend = g.Sum(x => x.Invoiced_Sales_Total) ?? 0,
            })
            .OrderByDescending(x => x.BranchSpend)
            .ToListAsync();

        return new ApiResponse<List<BranchesBySpend>>
        {
            Success = true,
            Message = "OK",
            Data = branches ?? new List<BranchesBySpend>(),
        };
    }

    public async Task<ApiResponse<List<ProductsBySpend>>> GetProductsBySpend(int customerId, string? dateFilter = null)
    {
        DateOnly? startDate = null;
        DateOnly? endDate = DateOnly.FromDateTime(DateTime.Today);

        if (!string.IsNullOrWhiteSpace(dateFilter))
        {
            switch (dateFilter.Trim().ToLower())
            {
                case "month to date":
                    startDate = new DateOnly(DateTime.Today.Year, DateTime.Today.Month, 1);
                    break;

                case "year to date":
                    startDate = new DateOnly(DateTime.Today.Year, 1, 1);
                    break;

                case "last year":
                    startDate = new DateOnly(DateTime.Today.Year - 1, 1, 1);
                    endDate = new DateOnly(DateTime.Today.Year - 1, 12, 31);
                    break;

                case "last year to date":
                    startDate = new DateOnly(DateTime.Today.Year - 1, 1, 1);
                    endDate = new DateOnly(
                        DateTime.Today.Year - 1,
                        DateTime.Today.Month,
                        DateTime.Today.Day);
                    break;

                default:
                    startDate = null;
                    break;
            }
        }

        var query = this._dbContext.SalesAndOrders
            .AsNoTracking()
            .Where(x => x.Customer_ID == customerId);

        if (startDate.HasValue)
        {
            query = query.Where(b => b.Transaction_Date >= startDate.Value && b.Transaction_Date <= endDate.Value);
        }

        var products = await query
            .GroupBy(x => x.Product_Category)
            .Select(g => new ProductsBySpend
            {
                ProductCategory = g.Key,
                ProductName = g
                .Where(x => x.Product_Category_Desc != null)
                .Select(x => x.Product_Category_Desc)
                .FirstOrDefault() ?? "Unknown",
                ProductSpend = g.Sum(x => x.Invoiced_Sales_Total) ?? 0,
            })
            .OrderByDescending(x => x.ProductSpend)
            .ToListAsync();

        return new ApiResponse<List<ProductsBySpend>>
        {
            Success = true,
            Message = "OK",
            Data = products ?? new List<ProductsBySpend>(),
        };
    }

    public async Task<ApiResponse<List<CategoryBySpend>>> GetCategoryBySpend(int customerId, string? dateFilter = null)
    {
        DateOnly? startDate = null;
        DateOnly? endDate = DateOnly.FromDateTime(DateTime.Today);

        if (!string.IsNullOrWhiteSpace(dateFilter))
        {
            switch (dateFilter.Trim().ToLower())
            {
                case "month to date":
                    startDate = new DateOnly(DateTime.Today.Year, DateTime.Today.Month, 1);
                    break;

                case "year to date":
                    startDate = new DateOnly(DateTime.Today.Year, 1, 1);
                    break;

                case "last year":
                    startDate = new DateOnly(DateTime.Today.Year - 1, 1, 1);
                    endDate = new DateOnly(DateTime.Today.Year - 1, 12, 31);
                    break;

                case "last year to date":
                    startDate = new DateOnly(DateTime.Today.Year - 1, 1, 1);
                    endDate = new DateOnly(
                        DateTime.Today.Year - 1,
                        DateTime.Today.Month,
                        DateTime.Today.Day);
                    break;

                default:
                    startDate = null;
                    break;
            }
        }

        var query = this._dbContext.SalesAndOrders
            .AsNoTracking()
            .Where(x => x.Customer_ID == customerId);

        if (startDate.HasValue)
        {
            query = query.Where(b => b.Transaction_Date >= startDate.Value && b.Transaction_Date <= endDate.Value);
        }

        var categories = await query
            .GroupBy(x => x.Order_Category!)
            .Select(g => new CategoryBySpend
            {
                CategoryName = g.Key,
                CategorySpend = g.Sum(x => x.Invoiced_Sales_Total) ?? 0,
            })
            .OrderByDescending(x => x.CategorySpend)
            .ToListAsync();

        return new ApiResponse<List<CategoryBySpend>>
        {
            Success = true,
            Message = "OK",
            Data = categories ?? new List<CategoryBySpend>(),
        };
    }

    public async Task<ApiResponse<SpendSummaryDto>> GetCustomerSpendSummaryAsync(string customerId)
    {
        var summaries = new[]
            {
                new SpendSummaryDto{
                    CustomerId = Guid.Parse("b404d7bd-12f8-4ca2-a6aa-22e3338b9bd5"),
                    TotalSpend = 450.74,
                    CreditLimit = 75000,
                    AvailableCredit = 74549.26,
                    TotalSales = 2,
                },
                new SpendSummaryDto{
                    CustomerId = Guid.Parse("a77c955e-a64f-4d69-ab0f-3a9fb23efdc2"),
                    TotalSpend = 450.00,
                    CreditLimit = 75000,
                    AvailableCredit = 74550.00,
                    TotalSales = 1,
                },
            };

        var result = summaries.FirstOrDefault(s =>
            s.CustomerId.ToString() == customerId);

        return new ApiResponse<SpendSummaryDto>
        {
            Success = true,
            Message = "OK",
            Data = result,
        };
    }

    public async Task<ApiResponse<OperativeSpendDto>> GetCustomerOperativeSpendAsync(string customerId, string operativeId)
    {
        var spends = new[]
            {
                new OperativeSpendDto{
                    CustomerId = Guid.Parse("b404d7bd-12f8-4ca2-a6aa-22e3338b9bd5"),
                    OperativeId = "OP-1",
                    OperativeName = "John Doe",
                    TotalSpend = 1200.50m,
                    Currency = "GBP",
                    StartDate = DateTime.UtcNow.AddMonths(-2),
                    EndDate = DateTime.UtcNow,
                    Notes = "Regular monthly purchases",
                },
                new OperativeSpendDto{
                    CustomerId = Guid.Parse("a77c955e-a64f-4d69-ab0f-3a9fb23efdc2"),
                    OperativeId = "OP-2",
                    OperativeName = "Anna Miller",
                    TotalSpend = 845.10m,
                    Currency = "GBP",
                    StartDate = DateTime.UtcNow.AddMonths(-1),
                    EndDate = DateTime.UtcNow,
                    Notes = "High-value items",
                },
                new OperativeSpendDto{
                    CustomerId = Guid.Parse("a77c955e-a64f-4d69-ab0f-3a9fb23efdc2"),
                    OperativeId = "OP-3",
                    OperativeName = "Mark Lee",
                    TotalSpend = 450.00m,
                    Currency = "GBP",
                    StartDate = DateTime.UtcNow.AddMonths(-3),
                    EndDate = DateTime.UtcNow.AddDays(-5),
                    Notes = "Occasional orders",
                },
            };

        var result = spends.FirstOrDefault(s =>
            s.CustomerId.ToString() == customerId &&
            s.OperativeId == operativeId);

        return new ApiResponse<OperativeSpendDto>
        {
            Success = true,
            Message = "OK",
            Data = result ?? new OperativeSpendDto(),
        };
    }

    public async Task<ApiResponse<OtifKpiDto>> GetCustomerOtifKpiAsync(string customerId)
    {
        var kpis = new[]
            {
                new OtifKpiDto{
                    CustomerId = Guid.Parse("b404d7bd-12f8-4ca2-a6aa-22e3338b9bd5"),
                    Name = "OTIF KPI",
                    Value = 92.5,
                    Target = 95.0,
                    OnTimePercentage = 90.2m,
                    InFullPercentage = 94.1m,
                },
                new OtifKpiDto{
                    CustomerId = Guid.Parse("a77c955e-a64f-4d69-ab0f-3a9fb23efdc2"),
                    Name = "OTIF KPI",
                    Value = 88.0,
                    Target = 92.0,
                    OnTimePercentage = 85.5m,
                    InFullPercentage = 90.3m,
                },
            };

        var result = kpis.FirstOrDefault(k =>
            k.CustomerId.ToString() == customerId);

        return new ApiResponse<OtifKpiDto>
        {
            Success = true,
            Message = "OK",
            Data = result,
        };
    }

    public async Task<ApiResponse<InvoiceAccuracyKpiDto>> GetKpiInvoiceAccuracyAsync(string customerId)
    {
        var kpis = new[]
            {
                new InvoiceAccuracyKpiDto{
                    CustomerId = Guid.Parse("b404d7bd-12f8-4ca2-a6aa-22e3338b9bd5"),
                    Name = "Invoice Accuracy",
                    Value = 98.5,
                    Target = 99.0,
                    TotalInvoices = 120,
                    CorrectInvoices = 118,
                    AccuracyPercentage = 98.33m,
                },
                new InvoiceAccuracyKpiDto{
                    CustomerId = Guid.Parse("a77c955e-a64f-4d69-ab0f-3a9fb23efdc2"),
                    Name = "Invoice Accuracy",
                    Value = 95.2,
                    Target = 97.0,
                    TotalInvoices = 90,
                    CorrectInvoices = 86,
                    AccuracyPercentage = 95.55m,
                },
            };

        var result = kpis.FirstOrDefault(k =>
            k.CustomerId.ToString() == customerId);

        return new ApiResponse<InvoiceAccuracyKpiDto>
        {
            Success = true,
            Message = "OK",
            Data = result ?? new InvoiceAccuracyKpiDto(),
        };
    }

    public async Task<ApiResponse<FaultyGoodsKpiDto>> GetKpiFaultyGoodsAsync(string customerId)
    {
        var kpis = new[]
        {
        new FaultyGoodsKpiDto{
            CustomerId = Guid.Parse("b404d7bd-12f8-4ca2-a6aa-22e3338b9bd5"),
            Name = "Faulty Goods KPI",
            Value = 2.5,
            Target = 1.0,
            TotalOrders = 200,
            FaultyOrders = 5,
            FaultyPercentage = 2.5m,
        },
        new FaultyGoodsKpiDto{
            CustomerId = Guid.Parse("a77c955e-a64f-4d69-ab0f-3a9fb23efdc2"),
            Name = "Faulty Goods KPI",
            Value = 1.2,
            Target = 1.0,
            TotalOrders = 150,
            FaultyOrders = 2,
            FaultyPercentage = 1.33m,
        },
    };

        var result = kpis.FirstOrDefault(k =>
            k.CustomerId.ToString() == customerId);

        return new ApiResponse<FaultyGoodsKpiDto>
        {
            Success = true,
            Message = "OK",
            Data = result ?? new FaultyGoodsKpiDto(),
        };
    }

    public async Task<ApiResponse<OperativeStatusDto>> GetOperativeStatusAsync(string customerId, int operativeStatus)
    {
        var client = await this.CreateClientAsync();

        if (client == null)
        {
            return new ApiResponse<OperativeStatusDto>
            {
                Success = false,
                Message = "No Kerridge instance found for the selected branch.",
                Data = new OperativeStatusDto(),
            };
        }

        var response = await client.GetAsync($"api/v1/customer/{customerId}/operative/status/{operativeStatus}");

        if (!response.IsSuccessStatusCode)
        {
            return new ApiResponse<OperativeStatusDto>
            {
                Success = false,
                Message = $"Kerridge API failed: {response.StatusCode}",
                Data = new OperativeStatusDto(),
            };
        }

        var result = await response.Content.ReadFromJsonAsync<OperativeStatusDto>();

        return new ApiResponse<OperativeStatusDto>
        {
            Success = true,
            Message = "OK",
            Data = result ?? new OperativeStatusDto(),
        };
    }

    public async Task<ApiResponse<List<CustomerDto>>> GetCustomersByBranchAndSalespersonAsync(string branchCode, string salespersonCode)
    {
        var client = await this.CreateClientAsync();

        if (client == null)
        {
            return new ApiResponse<List<CustomerDto>>
            {
                Success = false,
                Message = "No Kerridge instance found for the selected branch.",
                Data = new List<CustomerDto>(),
            };
        }

        var response = await client.GetAsync($"api/v1/customers/by-branch/{branchCode}/salesperson/{salespersonCode}");

        if (!response.IsSuccessStatusCode)
        {
            return new ApiResponse<List<CustomerDto>>
            {
                Success = false,
                Message = $"Kerridge API failed: {response.StatusCode}",
                Data = new List<CustomerDto>(),
            };
        }

        var result = await response.Content.ReadFromJsonAsync<List<CustomerDto>>();

        return new ApiResponse<List<CustomerDto>>
        {
            Success = true,
            Message = "OK",
            Data = result ?? new List<CustomerDto>(),
        };
    }
}