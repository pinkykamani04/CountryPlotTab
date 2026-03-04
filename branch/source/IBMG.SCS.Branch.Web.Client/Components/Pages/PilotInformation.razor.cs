using IBMG.SCS.Branch.Web.Client.Infrastructure.Models;
using Microsoft.AspNetCore.Components;
using Radzen;

namespace IBMG.SCS.Branch.Web.Client.Components.Pages
{
    public partial class PilotInformation
    {
        [Parameter]
        public PilotInfoModel Pilot { get; set; }

        [Parameter]
        public PilotInfoModel EditablePilot { get; set; }

        [Parameter]
        public bool IsEditMode { get; set; }

        [Parameter]
        public bool ShowFullDetails { get; set; }

        [Parameter]
        public string ProfileImageUrl { get; set; }

        [Parameter]
        public string ImagePreviewUrl { get; set; }

        [Parameter]
        public EventCallback OnLoadMore { get; set; }

        [Parameter]
        public EventCallback OnCloseFullDetails { get; set; }

        [Parameter]
        public EventCallback<bool> OnPreparePilotForEdit { get; set; }

        [Parameter]
        public EventCallback OnSave { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }

        [Parameter]
        public EventCallback<UploadChangeEventArgs> OnImageSelected { get; set; }

        private async Task HandleLoadMore()
        {
            await this.OnLoadMore.InvokeAsync();
        }

        private async Task HandleCloseFullDetails()
        {
            await this.OnCloseFullDetails.InvokeAsync();
        }

        private async Task HandlePreparePilotForEditQuick()
        {
            await this.OnPreparePilotForEdit.InvokeAsync(false);
        }

        private async Task HandlePreparePilotForEditFull()
        {
            await this.OnPreparePilotForEdit.InvokeAsync(true);
        }

        private async Task HandleSave()
        {
            await this.OnSave.InvokeAsync();
        }

        private async Task HandleCancel()
        {
            await this.OnCancel.InvokeAsync();
        }

        private async Task HandleImageSelected(UploadChangeEventArgs args)
        {
            await this.OnImageSelected.InvokeAsync(args);
        }

        private string GetInitials(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                return "PI";
            }

            var parts = fullName.Split(' ');
            return parts.Length >= 2
                ? $"{parts[0][0]}{parts[1][0]}".ToUpper()
                : fullName.Substring(0, 2).ToUpper();
        }
    }
}