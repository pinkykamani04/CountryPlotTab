// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using IBMG.SCS.Portal.Web.Client.Dtos;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.AddSpendLimitCommand;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace IBMG.SCS.Portal.Web.Client.Components.OperativeManagement.SpendLimitManagement
{
    public partial class SpendLimitSlideOutCard : ComponentBase
    {
        [Parameter]
        public OperativeDto Model { get; set; }

        [Parameter]
        public bool IsEdit { get; set; }

        [Inject]
        public DialogService DialogService { get; set; } = default!;

        [Inject]
        public ICommander Commander { get; set; } = default!;

        [Inject]
        public NotificationService NotificationService { get; set; } = default!;

        [Parameter]
        public EventCallback<OperativeDto> OnSubmit { get; set; }

        public List<string> Statuses = new() { "Active", "Inactive" };

        private bool IsOverrideChecked { get; set; } = false;

        private void OnOverrideChanged(bool value)
        {
            IsOverrideChecked = value;
        }

        private async Task OnSave()
        {
            if (string.IsNullOrWhiteSpace(Model.TradeCardNumber))
                return;

            var id = this.Model.Id == Guid.Empty ? Guid.NewGuid() : this.Model.Id;

            var result = await this.Commander.Send(new AddSpendLimitCommand(id, this.Model.TradeCardNumber, this.Model.FirstName, this.Model.LastName, this.Model.Status,
                    this.Model.TnxLimit, this.Model.DailyLimit, this.Model.WeeklyLimit, this.Model.MonthlyLimit, this.Model.EndDate, this.Model.OverrideTnxLimit, this.Model.OverrideDailyLimit,
                    this.Model.OverrideWeeklyLimit, this.Model.OverrideMonthlyLimit, this.Model.OverrideEndDate));

            if (result.Status != BluQube.Constants.CommandResultStatus.Succeeded)
            {
                this.NotificationService.Notify(NotificationSeverity.Error, result.ErrorData.Message);
                return;
            }

            await this.OnSubmit.InvokeAsync(this.Model);

            this.DialogService.Close(this.Model);
        }
    }
}