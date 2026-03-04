// Copyright (c) IBMG. All rights reserved.

using BluQube.Constants;
using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using IBMG.SCS.Portal.Web.Client.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace IBMG.SCS.Portal.Web.Client.Components.OperativeManagement.TradeCardManagement
{
    public partial class TradeCardManagement : ComponentBase
    {
        [Inject]
        public DialogService DialogService { get; set; } = default!;

        [Inject]
        private IQuerier Querier { get; set; } = default!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        private string searchText = string.Empty;

        private bool showDownloadMenu = false;

        private List<CardModel> tradeCardModel = new();

        private IEnumerable<CardModel> FilteredCards => string.IsNullOrWhiteSpace(this.searchText)
                                    ? this.tradeCardModel
                                    : this.tradeCardModel.Where(card =>
                                        (card.CardNumber?.Contains(this.searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                        (card.AssigneeName?.Contains(this.searchText, StringComparison.OrdinalIgnoreCase) ?? false));

        protected override async Task OnInitializedAsync()
        {
            await this.LoadTradeCardAsync();
            await base.OnInitializedAsync();
        }

        private async Task LoadTradeCardAsync()
        {
            var result = await this.Querier.Send(new GetAllTradeCardsQuery());

            if (result.Data != null && result.Status == QueryResultStatus.Succeeded)
            {
                this.tradeCardModel = result.Data.TradeCards.ToList();
            }
        }

        private void OpenAdd()
        {
            var model = new CardModel();

            this.DialogService.Open<TradeCardSlideout>(
                string.Empty,
                new Dictionary<string, object>()
                {
                    { "Model", model },
                    { "IsEdit", false },
                    { "OnSubmit", EventCallback.Factory.Create<CardModel>(this, this.OnCardSaved) },
                },
                new DialogOptions
                {
                    Width = "350px",
                    Height = "100%",
                    CssClass = "custom-slide-out",
                    Style = "right:0; ",
                    CloseDialogOnOverlayClick = true,
                    ShowClose = true,
                });
        }

        private void OpenEdit(CardModel model)
        {
            var editModel = new CardModel
            {
                CardNumber = model.CardNumber,
                AssigneeId = model.AssigneeId,
                Status = model.Status,
                Id = model.Id,
            };

            this.DialogService.Open<TradeCardSlideout>(
                string.Empty,
                new Dictionary<string, object>()
                {
                { "Model", editModel },
                { "IsEdit", true },
                { "OnSubmit", EventCallback.Factory.Create<CardModel>(this, this.OnCardSaved) },
                },
                new DialogOptions
                {
                    Width = "350px",
                    Height = "100%",
                    CssClass = "custom-slide-out",
                    Style = "right:0; ",
                    CloseDialogOnOverlayClick = true,
                    ShowClose = true,
                });
        }

        private async Task OnCardSaved(CardModel model)
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
                sb.AppendLine($"\"{card.CardNumber}\",\"{card.AssigneeName}\",\"{card.Status}\"");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            await this.JSRuntime.InvokeVoidAsync("JsFunctions.downloadFileFromBytes", "TradeCards.csv", bytes);
        }
    }
}