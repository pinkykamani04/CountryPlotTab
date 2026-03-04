using Microsoft.AspNetCore.Components;

namespace IBMG.SCS.Branch.Web.Client.Components.UserControls;

public partial class CustomCard
{
    [Parameter]
    public string CssClass { get; set; }

    [Parameter]
    public string CardNumber { get; set; }

    [Parameter]
    public string CardText { get; set; }

    [Parameter]
    public string Icon { get; set; } = "credit_card";

    [Parameter]
    public string IconColor { get; set; } = "#ff3f84";

    [Parameter]
    public string IconBackground { get; set; } = "#ffe6f0";

    [Parameter]
    public int? Size { get; set; }

    [Parameter]
    public int? SizeSM { get; set; }

    [Parameter]
    public int? SizeMD { get; set; }

    [Parameter]
    public int? SizeLG { get; set; }

    [Parameter]
    public int? SizeXL { get; set; }

    [Parameter]
    public int? SizeXS { get; set; }

    [Parameter]
    public int? SizeXX { get; set; }
}