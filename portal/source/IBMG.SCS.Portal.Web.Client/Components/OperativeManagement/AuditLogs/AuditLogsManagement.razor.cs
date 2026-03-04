// Copyright (c) IBMG. All rights reserved.

using BluQube.Constants;
using BluQube.Queries;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Queries;
using IBMG.SCS.Portal.Web.Client.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace IBMG.SCS.Portal.Web.Client.Components.OperativeManagement.AuditLogs
{
    public partial class AuditLogsManagement : ComponentBase
    {
        [Inject]
        public DialogService DialogService { get; set; } = default!;

        [Inject]
        private IQuerier Querier { get; set; } = default!;

        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        private string searchText = string.Empty;

        private bool showDownloadMenu = false;

        private List<AuditLogsModel> auditLogsModel = new();

        private IEnumerable<AuditLogsModel> FilteredCards => string.IsNullOrWhiteSpace(this.searchText)
                                    ? this.auditLogsModel.OrderByDescending(x => x.Date.ToDateTime(x.Time))
                                    : this.auditLogsModel.Where(logs =>
                                        (logs.Category?.Contains(this.searchText, StringComparison.OrdinalIgnoreCase) ?? false)).OrderByDescending(x => x.Date.ToDateTime(x.Time));

        protected override async Task OnInitializedAsync()
        {
            await this.LoadAuditLogsAsync();
            await base.OnInitializedAsync();
        }

        private async Task LoadAuditLogsAsync()
        {
            var result = await this.Querier.Send(new GetAllAuditLogsQuery());

            if (result.Data != null && result.Status == QueryResultStatus.Succeeded)
            {
                this.auditLogsModel = result.Data.AuditLogs.OrderByDescending(x => x.Date.ToDateTime(x.Time)).ToList();
            }
        }

        private void ToggleDownloadMenu()
        {
            this.showDownloadMenu = !this.showDownloadMenu;
        }

        private async Task ExportPdf()
        {
            var html = await this.JSRuntime.InvokeAsync<string>("JsFunctions.getOuterHtml", "#tradecard-export");
            await this.JSRuntime.InvokeVoidAsync("JsFunctions.openPrintWindow", html);
        }

        private async Task ExportCsv()
        {
            var sb = new System.Text.StringBuilder();

            sb.AppendLine("Date,Time,User,Category,Action");

            foreach (var card in this.auditLogsModel)
            {
                sb.AppendLine($"\"{card.Date}\",\"{card.Time}\",\"{card.User}\",\"{card.Category}\",\"{card.Action}\"");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            await this.JSRuntime.InvokeVoidAsync("JsFunctions.downloadFileFromBytes", "AuditLogsModel.csv", bytes);
        }
    }
}