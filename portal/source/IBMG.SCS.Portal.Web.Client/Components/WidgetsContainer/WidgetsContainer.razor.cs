using IBMG.SCS.Portal.Web.Client.Components.Dashboard;
using IBMG.SCS.Portal.Web.Client.Components.Widgets;
using IBMG.SCS.Portal.Web.Client.Dtos;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace IBMG.SCS.Portal.Web.Client.Components.WidgetsContainer
{
    public partial class WidgetsContainer : ComponentBase
    {
        [Parameter]
        public EventCallback<int> OnDelete { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public bool IsEditMode { get; set; }

        [Parameter]
        public EventCallback<WidgetSelection> OnWidgetAdded { get; set; }

        [Parameter]
        public int RowOrder { get; set; }

        [Inject]
        private DialogService? DialogService { get; set; }

        [Parameter]
        public bool IsEmpty { get; set; }

        [Parameter]
        public EventCallback<(int rowOrder, DashboardRowLayout layout)> OnRowLayoutChanged { get; set; }

        [Parameter]
        public List<DashboardWidgetViewModel> Widgets { get; set; } = new();

        [Parameter]
        public Dictionary<int, DashboardRowLayout?> ActiveLayouts { get; set; } = new();

        [Parameter]
        public EventCallback OnAddRow { get; set; }

        private string ContainerClass =>
            this.IsEditMode ? "widget-group-container edit" : "widget-group-container";

        private DashboardRowLayout? ActiveLayout;

        protected override async Task OnParametersSetAsync()
        {
            if (IsEditMode)
            {
                foreach (var row in this.Widgets.Select(w => w.RowOrder).Distinct())
                {
                    SetActiveLayoutFromWidgets(row);
                }
            }

            await base.OnParametersSetAsync();
        }

        private async Task AddNewRow()
        {
            if (this.OnAddRow.HasDelegate)
            {
                await this.OnAddRow.InvokeAsync();
            }
        }

        private bool CanAddWidget
        {
            get
            {
                if (!IsEditMode)
                    return false;

                return Widgets.Count < 4;
            }
        }

        private async Task OpenAddWidgetDialog()
{
    await DialogService.OpenAsync<WidgetsPopup>(
        "Add Widget",
        new Dictionary<string, object>
        {
            { "OnWidgetSelected", OnWidgetAdded },
            { "RowOrder", RowOrder }
        },
        new DialogOptions
        {
            Width = "1200px",
            Height = "1000px",
            CloseDialogOnOverlayClick = true,
        });
}

        private async Task ChangeLayout(DashboardRowLayout layout)
        {
            ActiveLayouts[RowOrder] = layout;

            if (OnRowLayoutChanged.HasDelegate)
            {
                await OnRowLayoutChanged.InvokeAsync((RowOrder, layout));
            }
        }

        private int RemainingWidth =>
               12 - Widgets.Sum(w =>
                   int.TryParse(w.RowLayoutType, out var wdt) ? wdt : 0);

        private void SetActiveLayoutFromWidgets(int rowOrder)
        {
            var rowWidgets = Widgets
                .Where(w => w.RowOrder == rowOrder)
                .OrderBy(w => w.Position)
                .ToList();

            if (!rowWidgets.Any())
            {
                ActiveLayouts[rowOrder] = null;
                return;
            }

            var widths = rowWidgets
                .Select(w => w.RowLayoutType)
                .ToList();

            ActiveLayouts[rowOrder] =
                widths.All(w => w == "12") ? DashboardRowLayout.OneColumn :
                widths.All(w => w == "6") ? DashboardRowLayout.TwoEqual :
                widths.All(w => w == "4") ? DashboardRowLayout.ThreeEqual :
                widths.All(w => w == "3") ? DashboardRowLayout.FourEqual :
                 (widths.First() == "8" && widths.Skip(1).All(w => w == "4"))
            ? DashboardRowLayout.TwoUneven :

        (widths.First() == "6" && widths.Skip(1).All(w => w == "3"))
            ? DashboardRowLayout.ThreeUneven :

                null;
        }

    }
}