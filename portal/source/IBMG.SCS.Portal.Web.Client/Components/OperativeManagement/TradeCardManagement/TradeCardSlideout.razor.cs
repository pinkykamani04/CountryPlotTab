// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using BluQube.Constants;
using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.AddTradeCardCommand;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using IBMG.SCS.Portal.Web.Client.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Radzen;

namespace IBMG.SCS.Portal.Web.Client.Components.OperativeManagement.TradeCardManagement
{
    public partial class TradeCardSlideout : ComponentBase
    {
        [Parameter]
        public CardModel Model { get; set; }

        [Inject]
        public IQuerier Querier { get; set; } = default!;

        [Parameter]
        public bool IsEdit { get; set; }

        [Inject]
        public DialogService DialogService { get; set; } = default!;

        [Inject]
        public ICommander Commander { get; set; } = default!;

        [Parameter]
        public EventCallback<CardModel> OnSubmit { get; set; }

        public List<string> Statuses = new() { "Active", "Inactive" };

        public List<AssigneeDto> AllAssignees = new();

        public List<AssigneeDto> FilteredAssignees = new();

        protected override async void OnInitialized()
        {
            var result = await this.Querier.Send(new GetAllOperativesQuery());

            if (result?.Status == QueryResultStatus.Succeeded && result.Data != null)
            {
                this.AllAssignees = result.Data.OperativeDtos
      .Where(o => o.TradeCardId == Guid.Empty || o.Id == this.Model.AssigneeId)
      .Select(o => new AssigneeDto
      {
          Id = o.Id,
          Name = $"{o.FirstName} {o.LastName}",
      })
      .ToList();

                if (this.IsEdit && this.Model.AssigneeId != null)
                {
                    var currentAssignee = result.Data.OperativeDtos
                        .FirstOrDefault(o => o.Id == this.Model.AssigneeId);

                    if (currentAssignee != null)
                    {
                        this.AllAssignees.Add(new AssigneeDto
                        {
                            Id = currentAssignee.Id,
                            Name = $"{currentAssignee.FirstName} {currentAssignee.LastName}",
                        });
                    }
                }

                this.FilteredAssignees = this.AllAssignees;
                this.StateHasChanged();
            }
        }

        private void OnAssigneeLoadData(LoadDataArgs args)
        {
            var search = args.Filter;

            if (string.IsNullOrWhiteSpace(search))
            {
                this.FilteredAssignees = AllAssignees;
            }
            else
            {
                this.FilteredAssignees = AllAssignees
                    .Where(a => a.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            this.StateHasChanged();
        }

        private async Task OnSave()
        {
            if (!string.IsNullOrWhiteSpace(Model.CardNumber))
            {
                var id = Model.Id == Guid.Empty ? Guid.NewGuid() : Model.Id;

                var result = await this.Commander.Send(new AddTradeCardCommand(id, Model.CardNumber, Model.AssigneeId, Model.Status));

                if (result.Status != BluQube.Constants.CommandResultStatus.Succeeded)
                {
                    return;
                }

                await this.OnSubmit.InvokeAsync(Model);
                this.DialogService.Close(Model);
            }
        }
    }
}