// Copyright (c) IBMG. All rights reserved.

using BluQube.Attributes;
using BluQube.Commands;
using IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.UpsertSupportEmailCommandResult;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.UpsertSupportEmailCommand
{
    [BluQubeCommand(Path = "commands/report-issue/email")]
    public sealed record ReportIssueCommand(string SupportEmail,  string IssueType, string Description) : ICommand<ReportIssueCommandResult>;
}