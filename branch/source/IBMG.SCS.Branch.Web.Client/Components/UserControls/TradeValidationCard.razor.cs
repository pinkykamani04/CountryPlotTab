// Copyright (c) IBMG. All rights reserved.

using IBMG.SCS.Branch.Web.Client.Infrastructure.Constants;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Models;
using Microsoft.AspNetCore.Components;

namespace IBMG.SCS.Branch.Web.Client.Components.UserControls;

public partial class TradeValidationCard : ComponentBase
{
    [Parameter]
    public TradeValidationCardDto Card { get; set; } = new();

    [Parameter]
    public ValidationState CardState { get; set; } = ValidationState.None;

    [Parameter]
    public string ValidationErrorMessage { get; set; } = string.Empty;

    private string GetCardCss()
    {
        return this.CardState switch
        {
            ValidationState.None => "tv-card-grey",
            ValidationState.Success => "tv-card-green",
            ValidationState.Error => "tv-card-red",
            ValidationState.Deactive => "tv-card-yellow",
            _ => "tv-card-grey",
        };
    }

    private string GetValue(string? val)
    {
        if (this.CardState == ValidationState.Success || this.CardState == ValidationState.Deactive)
        {
            return val;
        }
        else
        {
            return "-";
        }
    }
}