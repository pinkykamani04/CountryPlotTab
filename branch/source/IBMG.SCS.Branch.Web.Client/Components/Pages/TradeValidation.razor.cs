// Copyright (c) IBMG. All rights reserved.

using Azure;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Constants;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Models;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Services;
using IBMG.SCS.Branch.Web.Client.Kerridge;
using Microsoft.AspNetCore.Components;
using Radzen;
using KerridgeApiClient = IBMG.SCS.Branch.Web.Client.Kerridge.Client;

namespace IBMG.SCS.Branch.Web.Client.Components.Pages
{
    public partial class TradeValidation() : ComponentBase
    {
        [Inject]
        public NotificationService NotificationServices { get; set; } = default!;

        [Inject]
        public KerridgeApiClient KerridgeApi { get; set; } = default!;

        [Inject]
        public KerridgeBranchService BranchService { get; set; } = default!;

        private ValidationState cardState = ValidationState.None;

        private List<CustomerItem> customerOptions = new();
        private bool isBusy = false;

        private bool isLoadingCustomers = false;
        private string validationErrorMessage;

        private CustomerItem? selectedCustomer;

        private TradeValidationCardDto cardDto = new();

        private TradeValidationModel model = new();

        protected override async Task OnInitializedAsync()
        {
            await this.LoadCustomersFromApi();
        }

        private async Task LoadCustomersFromApi()
        {
            this.isLoadingCustomers = true;

            this.BranchService.BranchCode = 9205;
            var customers = await this.KerridgeApi.ListCustomersAsync();

            if (customers.Success)
            {
                this.customerOptions = customers.Data
                    .Select(c => new CustomerItem
                    {
                        Text = c.Name,
                        Value = c.CustomerId.ToString(),
                        AccountNumbers = c.AccountNumber,
                    })
                    .ToList();
            }
            else
            {
                this.NotificationServices.Notify(NotificationSeverity.Error, "Failed to load customers");
            }

            this.isLoadingCustomers = false;
        }

        private void ResetValidation()
        {
            this.cardState = ValidationState.None;
            this.validationErrorMessage = string.Empty;
            this.cardDto = new TradeValidationCardDto();
        }

        private async Task Validate()
        {
            this.isBusy = true;
            this.validationErrorMessage = null;
            this.cardState = ValidationState.None;
            this.cardDto = new TradeValidationCardDto();

            this.BranchService.BranchCode = 9205;

            var accountNo = this.model.AccountNumbers.FirstOrDefault();

            var jobResponse = await this.KerridgeApi.ValidateJobNumberAsync(accountNo, this.model.JobNumber);

            if (!jobResponse.Success || jobResponse.Data == null || !jobResponse.Data.Any())
            {
                this.validationErrorMessage = "Invalid Job Number";
                this.SetErrorState();
                return;
            }

            var job = jobResponse.Data.First();

            var operativeResponse = await KerridgeApi.GetSpendLimitsByOperativeAsync(this.model.OperativeId);

            if (!operativeResponse.Success || operativeResponse.Data == null)
            {
                this.validationErrorMessage = "Operative not found";
                this.SetErrorState();
                return;
            }

            var tradeCardResponse = await this.KerridgeApi.GetSpendLimitsByTradeCardNumberAsync(this.model.TradeCardNumber);

            if (!tradeCardResponse.Success || tradeCardResponse.Data == null)
            {
                this.validationErrorMessage = "Trade card number not found";
                this.SetErrorState();
                return;
            }

            var customerName = this.customerOptions.FirstOrDefault(c => c.AccountNumbers.Contains(accountNo))?.Text ?? "-";

            this.cardDto = new TradeValidationCardDto
            {
                JobAddress = job.JobDescription,
                SpendRemaining = tradeCardResponse.Data.OverrideDailyLimit.HasValue && tradeCardResponse.Data.OverrideDailyLimit > 0 
                                 ? tradeCardResponse.Data.OverrideDailyLimit.Value : tradeCardResponse.Data.DailyLimit > 0
                                 ? tradeCardResponse.Data.DailyLimit : job.Spend,
                Status = job.Active ? "Active" : "Inactive",
                JobNumber = this.model.JobNumber,
                OperativeId = this.model.OperativeId,
                OperativeName = customerName,
                TxnLimit = (decimal)(operativeResponse.Data.OverrideTnxLimit ?? operativeResponse.Data.TnxLimit),
                DailyLimit = (decimal)(operativeResponse.Data.OverrideDailyLimit ?? operativeResponse.Data.DailyLimit),
                CardNumber = this.model.TradeCardNumber,
            };

            this.cardState = job.Active ? ValidationState.Success : ValidationState.Deactive;
            this.isBusy = false;
        }

        private void SetErrorState()
        {
            this.cardState = ValidationState.Error;
            this.isBusy = false;
        }

        private async Task AddOrder()
        {
            if (!this.model.AccountNumbers.Any() || this.BranchService.BranchCode == 0)
            {
                this.NotificationServices.Notify(NotificationSeverity.Error, "Account number or branch code is missing.");

                return;
            }

            // For now we only add order for test so currently i used static request header
            var requestDto = new RequestSalesOrderDto()
            {
                Header = new SalesOrderHeader()
                {
                    Account = "1030911",
                    Branch = 9205,
                },
            };

            var addOrder = await this.KerridgeApi.AddBranchOrderAsync(requestDto);

            if (addOrder.Success)
            {
                this.NotificationServices.Notify(NotificationSeverity.Success, "Order Placed Successfully!");
            }
            else
            {
                this.NotificationServices.Notify(NotificationSeverity.Error, addOrder.Message);
            }
        }

        private void OnCustomerChanged(object value)
        {
            var customer = value as CustomerItem;
            if (customer == null)
            {
                return;
            }

            this.model.Customer = customer.Value;

            var accountNumbers = customer.AccountNumbers;

            this.model.AccountNumbers = accountNumbers.ToList();

            this.cardState = ValidationState.None;
            this.cardDto = new TradeValidationCardDto();
        }

        public class TradeValidationModel
        {
            public string Customer { get; set; } = string.Empty;

            public List<string> AccountNumbers { get; set; } = new();

            public string JobNumber { get; set; } = string.Empty;

            public string TradeCardNumber { get; set; } = string.Empty;

            public string OperativeId { get; set; } = string.Empty;
        }
    }
}