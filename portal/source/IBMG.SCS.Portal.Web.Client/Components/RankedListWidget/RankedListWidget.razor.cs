// Copyright (c) IBMG. All rights reserved.

using Microsoft.AspNetCore.Components;
using static IBMG.SCS.Portal.Web.Client.Components.Widgets.WidgetsPopup;

namespace IBMG.SCS.Portal.Web.Client.Components.RankedListWidget
{
    public partial class RankedListWidget<TItem>
    {
        [Parameter] public string Title { get; set; }

        [Parameter] public IEnumerable<TItem> Items { get; set; }

        [Parameter] public Func<TItem, int> RankField { get; set; }

        [Parameter] public Func<TItem, string> NameField { get; set; }

        [Parameter] public Func<TItem, decimal> AmountField { get; set; }
    }

}