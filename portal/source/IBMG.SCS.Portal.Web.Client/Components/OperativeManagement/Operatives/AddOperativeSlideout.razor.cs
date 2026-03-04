// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using BluQube.Constants;
using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Dtos;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.AddTradeCardCommand;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.UpsertOperativeCommand;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using IBMG.SCS.Portal.Web.Client.Models;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace IBMG.SCS.Portal.Web.Client.Components.OperativeManagement.Operatives
{
    public partial class AddOperativeSlideout : ComponentBase
    {
        [Parameter]
        public OperativeDto Operatives { get; set; } = new();

        [Parameter]
        public bool IsEdit { get; set; }

        [Inject]
        public DialogService DialogService { get; set; } = default!;

        [Inject]
        public ICommander Commander { get; set; } = default!;

        [Inject]
        public IQuerier Querier { get; set; } = default!;

        [Parameter]
        public EventCallback<OperativeDto> OnSubmit { get; set; }

        public IEnumerable<object> StatusOptions { get; set; } = Enum.GetValues<Status>().Select(s => new { Value = s, Text = s.ToString() });

        private IEnumerable<CardModel> FilteredTradeCards { get; set; } = new List<CardModel>();

        private bool showCreateTradeCard = false;

        private string lastTradeCardSearch = string.Empty;

        private string selectedAssignedCardInfo = string.Empty;

        public List<JobRoleDto> Roles { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            var result = await this.Querier.Send(new GetAllJobRoleQuery());
            this.Roles = result.Data.LimitDtos.ToList();
            await this.LoadAvailableCards();
            await base.OnInitializedAsync();
        }

        private async Task OnSave()
        {
            var id = this.Operatives.Id == Guid.Empty ? Guid.NewGuid() : this.Operatives.Id;

            var upsertCommand = new UpsertOperativeCommand(id, this.Operatives.FirstName, this.Operatives.LastName, this.Operatives.JobRole,
                this.Operatives.OperativeNumber, this.Operatives.TnxLimit, this.Operatives.DailyLimit, this.Operatives.WeeklyLimit, this.Operatives.MonthlyLimit,
                this.Operatives.StartDate, this.Operatives.EndDate, this.Operatives.Status, this.Operatives.TradeCardId);

            var result = await this.Commander.Send(upsertCommand);

            if (result.Status != BluQube.Constants.CommandResultStatus.Succeeded)
            {
                return;
            }

            await this.OnSubmit.InvokeAsync(this.Operatives);
            this.DialogService.Close(this.Operatives);
        }

        private async Task LoadAvailableCards()
        {
            try
            {
                var allCardsResult = await this.Querier.Send(new GetAllTradeCardsQuery());

                var allCards = (allCardsResult?.Status == QueryResultStatus.Succeeded && allCardsResult.Data != null)
                    ? allCardsResult.Data.TradeCards.ToList()
                    : new List<CardModel>();

                // 1️⃣ Unassigned cards
                var availableCards = allCards.Where(c => c.AssigneeId == null).ToList();

                // 2️⃣ Add ALL cards assigned to this operative
                var myCards = allCards.Where(c => c.AssigneeId == this.Operatives.Id).ToList();

                foreach (var card in myCards)
                {
                    if (!availableCards.Any(c => c.Id == card.Id))
                    {
                        availableCards.Add(card);
                    }
                }

                // 3️⃣ Display currently assigned card info
                if (this.Operatives.TradeCardId != Guid.Empty)
                {
                    var assignedCard = myCards.FirstOrDefault(c => c.Id == this.Operatives.TradeCardId);

                    if (assignedCard != null)
                    {
                        this.selectedAssignedCardInfo = $"Currently assigned: {assignedCard.CardNumber}";
                    }
                }

                this.FilteredTradeCards = availableCards;
            }
            catch
            {
                this.FilteredTradeCards = Enumerable.Empty<CardModel>();
            }
        }

        private async void OnTradeCardLoadData(LoadDataArgs args)
        {
            this.lastTradeCardSearch = args.Filter ?? string.Empty;

            await this.LoadAvailableCards();
            var cards = this.FilteredTradeCards.ToList();

            if (string.IsNullOrWhiteSpace(this.lastTradeCardSearch))
            {
                this.showCreateTradeCard = false;
                return;
            }

            this.FilteredTradeCards = cards.Where(x => (x.CardNumber ?? string.Empty)
                                           .Contains(this.lastTradeCardSearch, StringComparison.OrdinalIgnoreCase)).ToList();

            this.showCreateTradeCard = !this.FilteredTradeCards.Any();

            this.StateHasChanged();
        }

        private async Task CreateNewTradeCard()
        {
            if (string.IsNullOrWhiteSpace(this.lastTradeCardSearch))
            {
                return;
            }

            var newNumber = this.lastTradeCardSearch.Trim();
            var newId = Guid.NewGuid();

            var addResult = await this.Commander.Send(new AddTradeCardCommand(newId, newNumber, null, "Active"));

            if (addResult?.Status != CommandResultStatus.Succeeded)
            {
                return;
            }

            this.Operatives.TradeCardId = newId;

            await this.LoadAvailableCards();

            this.showCreateTradeCard = false;
            this.selectedAssignedCardInfo = $"Assigned: {newNumber}";

            this.StateHasChanged();
        }
    }
}