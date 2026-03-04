// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using BluQube.Constants;
using BluQube.Queries;
using IBMG.SCS.Portal.Web.Client.Dtos;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.UpsertValidationCommand;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Queries;
using IBMG.SCS.Portal.Web.Client.Models;
using Microsoft.AspNetCore.Components;

namespace IBMG.SCS.Portal.Web.Client.Pages.ValidationOptions
{
    public partial class ValidationOptions : ComponentBase
    {
        [Parameter]
        public int CustomerId { get; set; }

        [Inject]
        private ICommander Commander { get; set; } = default!;

        [Inject]
        private IQuerier? Querier { get; set; }

        private List<ValidationDto> validationOption { get; set; } = new();

        private List<ValidationDto> originalValidationOptions = new();

        private bool isSaving = false;

        private bool IsValid => this.validationOption.Any(x => x.IsMandatory);

        protected override async Task OnInitializedAsync()
        {
            this.validationOption = Enum.GetValues<ValidationType>()
                .Select(type => new ValidationDto
                {
                    ValidationType = (int)type,
                    IsEnabled = false,
                    IsMandatory = false,
                })
                .ToList();

            var result = await this.Querier.Send(new GetAllValidationQuery());

            if (result?.Status == QueryResultStatus.Succeeded && result.Data != null)
            {
                this.originalValidationOptions = result.Data.TradeCards.ToList();

                foreach (var dbOption in this.originalValidationOptions)
                {
                    var match = this.validationOption
                        .FirstOrDefault(v => v.ValidationType == dbOption.ValidationType);

                    if (match != null)
                    {
                        match.IsEnabled = dbOption.IsEnabled;
                        match.IsMandatory = dbOption.IsMandatory;
                    }
                }
            }
        }

        private void OnEnabledChanged(ValidationDto option)
        {
            if (!option.IsEnabled)
            {
                option.IsMandatory = false;
            }
        }

        private async Task SaveValidationOptions()
        {
            if (!this.IsValid || this.isSaving)
            {
                return;
            }

            this.isSaving = true;
            var result = await this.Querier.Send(new GetAllValidationQuery());

            if (result?.Status == QueryResultStatus.Succeeded && result.Data != null)
            {
                this.originalValidationOptions = result.Data.TradeCards.ToList();
            }

            var changedOptions = validationOption
     .Where(current =>
     {
         var original = originalValidationOptions
             .FirstOrDefault(o => o.ValidationType == current.ValidationType);

         if (original == null)
         {
             return current.IsEnabled;
         }

         return current.IsEnabled != original.IsEnabled
             || current.IsMandatory != original.IsMandatory;
     })
     .ToList();

            foreach (var validation in changedOptions)
            {
                await this.Commander.Send(
                    new UpsertValidationCommand(
                        validation.Id == Guid.Empty ? Guid.NewGuid() : validation.Id,
                        this.CustomerId,
                        validation.ValidationType,
                        validation.IsEnabled,
                        validation.IsMandatory,
                        DateTime.UtcNow,
                        null,
                        null
                    )
                );
            }

            this.originalValidationOptions = this.validationOption
                .Select(x => new ValidationDto
                {
                    ValidationType = x.ValidationType,
                    IsEnabled = x.IsEnabled,
                    IsMandatory = x.IsMandatory,
                })
                .ToList();

            this.isSaving = false;
            this.StateHasChanged();
        }

        private string GetValidationLabel(int validationType)
        {
            return (ValidationType)validationType switch
            {
                ValidationType.TradeCardNumber => "Trade Card Number",
                ValidationType.JobNumber => "Job Number",
                ValidationType.OperativeId => "Operative ID",
                _ => "Unknown",
            };
        }
    }
}