// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using FluentValidation;
using IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.UpsertSupportEmailCommandResult;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.UpsertSupportEmailCommand;
using IBMG.SCS.Portal.Web.Services.Contracts;
using PearDrop.Authentication.Client.Contracts;
using PearDrop.Client.Contracts.Authentication;
using PearDrop.Contracts.Authentication;

namespace IBMG.SCS.Portal.Web.Infrastructure.CommandHandlers.UpsertSupportEmailCommandHandler
{
    public class ReportIssueCommandHandler : CommandHandler<ReportIssueCommand, ReportIssueCommandResult>
    {
        private readonly IExtendedAuthenticationNotifier _notifier;
        private readonly ICurrentAuthenticatedUserProvider _currentUser;
        private readonly ILogger<ReportIssueCommandHandler> _logger;

        public ReportIssueCommandHandler(
            IExtendedAuthenticationNotifier notifier,
            IEnumerable<IValidator<ReportIssueCommand>> validators,
            ICurrentAuthenticatedUserProvider currentUser,
            ILogger<ReportIssueCommandHandler> logger)
            : base(validators, logger)
        {
            _notifier = notifier;
            _currentUser = currentUser;
            _logger = logger;
        }

        protected override async Task<CommandResult<ReportIssueCommandResult>> HandleInternal(ReportIssueCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var (_, userName, userEmail) = this.GetUser();

                var body = $"""
                <p><strong>User Name:</strong> {userName}</p>
                <p><strong>User Email:</strong> {userEmail}</p>
                <p><strong>Issue Type:</strong> {request.IssueType}</p>
                <p><strong>Description:</strong></p>
                <p>{request.Description}</p>
                """;

                var emailResult = await _notifier.SendCustomSupportEmail(
                     supportEmail: request.SupportEmail,
                     subject: "New Support Issue Reported",
                     body: body,
                     cancellationToken: cancellationToken);

                if (!emailResult.IsSuccess)
                {
                    return CommandResult<ReportIssueCommandResult>.Failed(new BluQubeErrorData("Unable to send support email."));
                }

                return CommandResult<ReportIssueCommandResult>.Succeeded(new ReportIssueCommandResult());
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error while reporting issue");
                return CommandResult<ReportIssueCommandResult>.Failed(new BluQubeErrorData("Unexpected error occurred."));
            }
        }

        private (Guid UserId, string UserName, string Email) GetUser()
        {
            var current = this._currentUser.CurrentAuthenticatedUser;

            if (current.HasValue)
            {
                return current.Value switch
                {
                    AuthenticatedUser u => (u.UserId, $"{u.FirstName} {u.LastName}", u.ContactEmailAddress),
                    ISystemUser => (Guid.Empty, "System", "system@ibmg.com"),
                    _ => (Guid.Empty, "Unknown", "unknown@ibmg.com"),
                };
            }

            return (Guid.Empty, "Unknown", "unknown@ibmg.com");
        }
    }
}