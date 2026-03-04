using Microsoft.AspNetCore.Components;

namespace IBMG.SCS.Branch.Web.Client.Components.Pages
{
    public partial class Widgets
    {
        [Parameter]
        public string CssClass { get; set; }

        [Parameter]
        public string Number { get; set; }

        [Parameter]
        public string Text { get; set; }

        [Parameter]
        public string Icon { get; set; } = "aircraft";

        [Parameter]
        public string IconColor { get; set; } = "#ff3f84";

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
}