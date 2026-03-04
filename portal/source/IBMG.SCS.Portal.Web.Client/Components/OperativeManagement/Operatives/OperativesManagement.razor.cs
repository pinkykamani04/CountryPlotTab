// Copyright (c) IBMG. All rights reserved.

using BluQube.Constants;
using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Components.OperativeManagement.TradeCardManagement;
using IBMG.SCS.Portal.Web.Client.Dtos;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using IBMG.SCS.Portal.Web.Client.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace IBMG.SCS.Portal.Web.Client.Components.OperativeManagement.Operatives
{
    public partial class OperativesManagement : ComponentBase
    {
        [Inject]
        public DialogService DialogService { get; set; } = default!;

        [Inject]
        private IQuerier Querier { get; set; } = default!;

        private string searchText = string.Empty;

        private List<OperativeDto> OperativeDtos { get; set; } = new();

        public List<JobRoleDto> Roles { get; set; } = new();

        private IEnumerable<OperativeDto> FilteredCards => string.IsNullOrWhiteSpace(this.searchText)
                                    ? this.OperativeDtos
                                    : this.OperativeDtos.Where(card =>
                                        (card.OperativeNumber?.Contains(this.searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                        (card.FirstName?.Contains(this.searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                        (card.LastName?.Contains(this.searchText, StringComparison.OrdinalIgnoreCase) ?? false));

        protected override async Task OnInitializedAsync()
        {
            var result = await this.Querier.Send(new GetAllJobRoleQuery());

            if (result != null)
            {
                this.Roles = result.Data.LimitDtos.ToList();
            }

            await this.LoadTradeCardAsync();
            await base.OnInitializedAsync();
        }

        private async Task LoadTradeCardAsync()
        {
            var result = await this.Querier.Send(new GetAllOperativesQuery());

            if (result.Data != null && result.Status == QueryResultStatus.Succeeded)
            {
                this.OperativeDtos = result.Data.OperativeDtos.ToList();
            }
        }

        private void OpenAdd()
        {
            var model = new OperativeDto();

            this.DialogService.Open<AddOperativeSlideout>(
                string.Empty,
                new Dictionary<string, object>()
                {
                    { "Operatives", model },
                    { "IsEdit", false },
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

        private void OpenEdit(OperativeDto model)
        {
            this.DialogService.Open<AddOperativeSlideout>(
                 string.Empty,
                 new Dictionary<string, object>()
                             {
                                { "Operatives", model },
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
    }
}