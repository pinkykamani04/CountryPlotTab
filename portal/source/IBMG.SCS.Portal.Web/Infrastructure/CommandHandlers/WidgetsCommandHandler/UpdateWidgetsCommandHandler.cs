// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using FluentValidation;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.WidgetsCommand;

namespace IBMG.SCS.Portal.Web.Infrastructure.CommandHandlers.WidgetsCommandHandler
{
    public class UpdateWidgetsCommandHandler : CommandHandler<UpdateWidgetsCommand>
    {
        private readonly PortalDBContext _context;

        public UpdateWidgetsCommandHandler(
            PortalDBContext context,
            IEnumerable<IValidator<UpdateWidgetsCommand>> validators,
            ILogger<UpdateWidgetsCommandHandler> logger)
            : base(validators, logger)
        {
            _context = context;
        }

        protected override async Task<CommandResult> HandleInternal(UpdateWidgetsCommand request, CancellationToken cancellationToken)
        {
            var widget = await _context.Widgets.FindAsync(new object[] { request.Id }, cancellationToken);
            if (widget == null)
            {
                return CommandResult.Failed(new BluQubeErrorData("Widget not found"));
            }

            widget.Name = request.Name;
            widget.IsMandatory = request.IsMandatory;
            widget.Icon = request.Icon;

            await _context.SaveChangesAsync(cancellationToken);
            return CommandResult.Succeeded();
        }
    }
}
