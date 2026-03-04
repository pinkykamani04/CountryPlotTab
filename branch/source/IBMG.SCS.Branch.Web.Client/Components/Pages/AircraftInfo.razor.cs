using IBMG.SCS.Branch.Web.Client.Infrastructure.Models;
using Microsoft.AspNetCore.Components;

namespace IBMG.SCS.Branch.Web.Client.Components.Pages
{
    public partial class AircraftInfo
    {
        [Parameter]
        public bool IsLoading { get; set; }

        [Parameter]
        public List<AircraftInfoModel> AircraftList { get; set; } = new();

        [Parameter]
        public AircraftInfoModel Aircraft { get; set; } = new();

        [Parameter]
        public Guid SelectedAircraftId { get; set; }

        [Parameter]
        public bool IsEditMode { get; set; }

        [Parameter]
        public bool IsAddMode { get; set; }

        [Parameter]
        public EventCallback<Guid> OnAircraftChanged { get; set; }

        [Parameter]
        public EventCallback OnSave { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }

        [Parameter]
        public EventCallback ToggleEditMode { get; set; }

        [Parameter]
        public EventCallback AddNew { get; set; }

        private async Task HandleAircraftChanged(Guid aircraftId)
        {
            await this.OnAircraftChanged.InvokeAsync(aircraftId);
        }

        private async Task HandleSave()
        {
            await this.OnSave.InvokeAsync();
        }

        private async Task HandleCancel()
        {
            await this.OnCancel.InvokeAsync();
        }

        private async Task HandleToggleEdit()
        {
            await this.ToggleEditMode.InvokeAsync();
        }

        private async Task HandleAddNew()
        {
            await this.AddNew.InvokeAsync();
        }
    }
}