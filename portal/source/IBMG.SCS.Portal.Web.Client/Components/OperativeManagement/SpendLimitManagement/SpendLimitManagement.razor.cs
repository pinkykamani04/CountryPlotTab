// Copyright (c) IBMG. All rights reserved.

using System.Globalization;
using BluQube.Constants;
using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Dtos;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace IBMG.SCS.Portal.Web.Client.Components.OperativeManagement.SpendLimitManagement
{
    public partial class SpendLimitManagement : ComponentBase
    {
        [Inject]
        public DialogService DialogService { get; set; } = default!;

        [Inject]
        private IQuerier Querier { get; set; } = default!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        private string searchText = string.Empty;

        private bool showDownloadMenu = false;

        private List<OperativeDto> tradeCardModel = new();

        private CultureInfo Uk = new CultureInfo("en-GB");

        private IEnumerable<OperativeDto> FilteredCards => string.IsNullOrWhiteSpace(this.searchText)
                                    ? this.tradeCardModel
                                    : this.tradeCardModel.Where(card =>
                                        (card.TradeCardNumber?.Contains(this.searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                        (card.FirstName?.Contains(this.searchText, StringComparison.OrdinalIgnoreCase) ?? false));

        protected override async Task OnInitializedAsync()
        {
            await this.LoadTradeCardAsync();
            await base.OnInitializedAsync();
        }

        private async Task LoadTradeCardAsync()
        {
            var result = await this.Querier.Send(new GetAllSpendLimitsQuery());

            if (result.Data != null && result.Status == QueryResultStatus.Succeeded)
            {
                this.tradeCardModel = result.Data.LimitDtos.ToList();
            }
        }

        private void OpenEdit(OperativeDto model)
        {
            var editModel = new OperativeDto
            {
                TradeCardNumber = model.TradeCardNumber,
                TradeCardId = model.TradeCardId,
                Status = model.Status,
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                TnxLimit = model.TnxLimit,
                DailyLimit = model.DailyLimit,
                WeeklyLimit = model.WeeklyLimit,
                MonthlyLimit = model.MonthlyLimit,
                EndDate = model.EndDate,
                OverrideEndDate = model.OverrideEndDate,
                OverrideMonthlyLimit = model.OverrideMonthlyLimit,
                OverrideWeeklyLimit = model.OverrideWeeklyLimit,
                OverrideDailyLimit = model.OverrideDailyLimit,
                OverrideTnxLimit = model.OverrideTnxLimit,
            };

            this.DialogService.Open<SpendLimitSlideOutCard>(
                string.Empty,
                new Dictionary<string, object>()
                {
                { "Model", editModel },
                { "IsEdit", true },
                { "OnSubmit", EventCallback.Factory.Create<OperativeDto>(this, this.OnCardSaved) },
                },
                new DialogOptions
                {
                    Width = "550px",
                    Height = "100%",
                    CssClass = "custom-slide-out",
                    Style = "right:0; ",
                    CloseDialogOnOverlayClick = true,
                    ShowClose = true,
                });
        }

        private async Task OnCardSaved(OperativeDto model)
        {
            await this.LoadTradeCardAsync();
            this.StateHasChanged();
        }

        private void ToggleDownloadMenu()
        {
            this.showDownloadMenu = !this.showDownloadMenu;
        }

        private async Task ExportPdf()
        {
            var html = await this.JSRuntime.InvokeAsync<string>("JsFunctions.getOuterHtml", "#tradecard-export");
            await this.JSRuntime.InvokeVoidAsync("JsFunctions.openPrintWindow", html);
        }

        private async Task ExportCsv()
        {
            var sb = new System.Text.StringBuilder();

            sb.AppendLine("Card Number,Assignee,Status");

            foreach (var card in this.tradeCardModel)
            {
                sb.AppendLine($"\"{card.TradeCardNumber}\",\"{card.FirstName}\",\"{card.Status}\"");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            await this.JSRuntime.InvokeVoidAsync("JsFunctions.downloadFileFromBytes", "TradeCards.csv", bytes);
        }
    }
}