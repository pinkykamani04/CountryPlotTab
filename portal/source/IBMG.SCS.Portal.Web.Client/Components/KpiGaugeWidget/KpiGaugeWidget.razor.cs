// Copyright (c) IBMG. All rights reserved.

using Microsoft.AspNetCore.Components;

namespace IBMG.SCS.Portal.Web.Client.Components.KpiGaugeWidget
{
    public partial class KpiGaugeWidget
    {
        [Parameter] 
        public string Title { get; set; }

        [Parameter]
        public string Description { get; set; }

        [Parameter]
        public double Value { get; set; }
    }
}