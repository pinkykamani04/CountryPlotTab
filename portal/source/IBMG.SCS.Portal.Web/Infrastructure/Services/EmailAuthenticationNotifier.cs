// Copyright (c) Shutter Portal. All rights reserved.

using System.Reflection;
using System.Web;
using FluentEmail.Core;
using IBMG.SCS.Portal.Web.Constants.Settings;
using IBMG.SCS.Portal.Web.Infrastructure.Settings;
using IBMG.SCS.Portal.Web.Services.Contracts;
using Microsoft.Extensions.Options;
using PearDrop.Authentication.Client.Constants;
using ResultMonad;

namespace IBMG.SCS.Portal.Web.Infrastructure.Services
{
    public class EmailAuthenticationNotifier : IExtendedAuthenticationNotifier
    {
        private readonly IFluentEmail _fluentEmail;
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailAuthenticationNotifier> _logger;
        private readonly IOptions<DocumentSetting> _documentSettings;

        public EmailAuthenticationNotifier(
            IFluentEmail fluentEmail, IOptions<EmailSettings> emailSettings, ILogger<EmailAuthenticationNotifier> logger, IOptions<DocumentSetting> documentSettings)
        {
            this._fluentEmail = fluentEmail;
            this._logger = logger;
            this._emailSettings = emailSettings.Value;
            this._documentSettings = documentSettings;
        }

        public async Task<Result> SendWelcomeNotificationMessage(
            string? firstName, string? lastName, string emailAddress, string token, CancellationToken cancellationToken = default)
        {
            var url = string.Format(this._emailSettings.ConfirmAccountUrl, HttpUtility.UrlEncode(token));

            return await this.SendEmail(firstName, lastName, emailAddress, "Welcome to IBMG",
                $"""
             <p>Dear {firstName},</p>
             <p>We're excited to welcome you to IBMG Portal! Your company has provided you with access to our platform, and we're confident that you'll find it a valuable tool for your work.</p>
             <p>To get started, please verify your email address by clicking the link below:</p>
             <p><a href="{url}">{url}</a></p>
             <p>Once your email is verified, you'll have full access to all the features of our service.</p>
             <p>If you have any questions or need assistance, please don't hesitate to contact us.</p>
             <p>Thank you,</p>
             <p>The IBMG Team</p>
             """, cancellationToken);
        }

        public async Task<Result> SendMfaNotificationMessage(string? firstName, string? lastName, string emailAddress, string token, CancellationToken cancellationToken = default)
        {
            return await this.SendEmail(firstName, lastName, emailAddress, "Your MFA Token for IBMG",
                $"""
             <p>Dear {firstName},</p>
             <p>We have recieved a request to acces your account. As a safety precuation, you'll need to enter the following code to complete login:</p>
             <p style="color:#00703C;">{token}</p>
             <p>This token is valid for a limited time only for security reasons.</p>
             <p>If you did not make this login attempt, please change your password immediately.</p>
             """, cancellationToken);
        }

        public async Task<Result> SendPasswordResetNotificationMessage(
            string? firstName, string? lastName, string emailAddress, string token, CancellationToken cancellationToken = default)
        {
            var url = string.Format(this._emailSettings.ForgottenPasswordUrl, HttpUtility.UrlEncode(token));

            return await this.SendEmail(firstName, lastName, emailAddress, "IBMG - Password Reset", $"" +
                $"""
            <p>Hi {firstName},</p>
            <p>We received a request to reset your IBMG password.</p>
            <p>Please click the link below to reset your password:</p>
            <p><a href="{url}">{url}</a></p>
            <p>This link will expire in 30 minutes.</p>
            <p>Kind regards,</p>
            <p>The IBMG Team</p>
            """, cancellationToken);
        }

        public async Task<Result> SendAccountDisabledNotificationMessage(
            string? firstName, string? lastName, string emailAddress, CancellationToken cancellationToken = new())
        {
            return await Task.FromResult(Result.Ok());
        }

        public async Task<Result> SendAccountEnabledNotificationMessage(
            string? firstName, string? lastName, string emailAddress, CancellationToken cancellationToken = new())
        {
            return await Task.FromResult(Result.Ok());
        }

        public async Task<Result> SendPasswordChangedNotificationMessage(
            string? firstName, string? lastName, string emailAddress, CancellationToken cancellationToken = new())
        {
            return await Task.FromResult(Result.Ok());
        }

        public async Task<Result> SendUserPrincipleNameVerificationMessage(string? firstName, string? lastName, string value,
                UserPrincipalNameType userPrincipalNameType, string token, CancellationToken cancellationToken = new())
        {
            return await Task.FromResult(Result.Ok());
        }

        public async Task<Result> SendMemberInviatationMessage(string firstName, string lastName, string token, string emailAddress, CancellationToken cancellationToken = default)
        {
            var url = string.Format(this._emailSettings.MemberInviatationUrl, HttpUtility.UrlEncode(token));

            return await this.SendEmail(firstName, lastName, emailAddress, "Welcome to IBMG",
                $"""
             <p>Dear {firstName},</p>
             <p>We're excited to welcome you to IBMG Portal! Your company has provided you with access to our platform, and we're confident that you'll find it a valuable tool for your work.</p>
             <p>To get started, please verify your email address by clicking the link below:</p>
             <p><a href="{url}"><button style="background-color:lightblue;padding:10px">Accept Inviatation</button></a></p>
             
             <p>Once your email is verified, you'll have full access to all the features of our service.</p>
             <p>If you have any questions or need assistance, please don't hesitate to contact us.</p>
             <p>Thank you,</p>
             <p>The IBMG Team</p>
             """, cancellationToken);
        }

        public async Task<Result> SendCustomSupportEmail(string supportEmail, string subject, string body, CancellationToken cancellationToken = default)
        {
            return await this.SendEmail(
                firstName: "Support",
                lastName: "Team",
                emailAddress: supportEmail,
                subject: subject,
                body: body,
                cancellationToken: cancellationToken);
        }

        private async Task<Result> SendEmail(string? firstName, string? lastName, string emailAddress, string subject, string body, CancellationToken cancellationToken = default)
        {
            var email = this._fluentEmail
                .To(emailAddress, $"{firstName} {lastName}")
                .Subject(subject)
                .UsingTemplateFromEmbedded(
                    "IBMG.SCS.Portal.Web.EmailTemplates.default-template.html",
                    new
                    {
                        BodyContent = body,
                        PreHeaderText = subject != null && subject.Contains("Your MFA Token for IBMG", StringComparison.OrdinalIgnoreCase)
            ? "Verification code sent by IBMG. Please, don’t reply."
            : string.Empty,
                    },
                    typeof(EmailAuthenticationNotifier).GetTypeInfo().Assembly);

            email.Data.Tags =
            [
                "IBMG",
            ];

            try
            {
                var sendResponse = await email.SendAsync(cancellationToken);

                return sendResponse.Successful ? Result.Ok() : Result.Fail();
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, message: ex.Message);
                return Result.Fail();
            }
        }
    }
}