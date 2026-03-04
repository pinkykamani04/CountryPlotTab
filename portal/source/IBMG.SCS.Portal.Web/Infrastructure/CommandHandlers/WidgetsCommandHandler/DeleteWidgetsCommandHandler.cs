// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using FluentValidation;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.WidgetsCommand;

namespace IBMG.SCS.Portal.Web.Infrastructure.CommandHandlers.WidgetsCommandHandler
{
    public class DeleteWidgetsCommandHandler : CommandHandler<DeleteWidgetsCommand>
    {
        private readonly PortalDBContext _context;

        public DeleteWidgetsCommandHandler(
            PortalDBContext context,
            IEnumerable<IValidator<DeleteWidgetsCommand>> validators,
            ILogger<DeleteWidgetsCommandHandler> logger)
            : base(validators, logger)
        {
            _context = context;
        }

        protected override async Task<CommandResult> HandleInternal(DeleteWidgetsCommand request, CancellationToken cancellationToken)
        {
            var widget = await _context.Widgets.FindAsync(new object[] { request.Id }, cancellationToken);
            if (widget == null)
            {
                return CommandResult.Failed(new BluQubeErrorData("Widget not found"));
            }

            _context.Widgets.Remove(widget);
            await _context.SaveChangesAsync(cancellationToken);
            return CommandResult.Succeeded();
        }
    }
}