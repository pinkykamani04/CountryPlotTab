// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;
using FluentValidation;
using IBMG.SCS.Infrastructure.DBContext;
using IBMG.SCS.Infrastructure.Entities;
using IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.WidgetsCommand;
using IBMG.SCS.Portal.Web.Infrastructure.Data;

namespace IBMG.SCS.Portal.Web.Infrastructure.CommandHandlers.WidgetsCommandHandler
{
    public class AddWidgetsCommandHandler : CommandHandler<AddWidgetsCommand>
    {
        private readonly PortalDBContext _context;

        public AddWidgetsCommandHandler(
            PortalDBContext context,
            IEnumerable<IValidator<AddWidgetsCommand>> validators,
            ILogger<AddWidgetsCommandHandler> logger)
            : base(validators, logger)
        {
            _context = context;
        }

        protected override async Task<CommandResult> HandleInternal(AddWidgetsCommand request, CancellationToken cancellationToken)
        {
            var widget = new Widget
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                IsMandatory = request.IsMandatory,
                Icon = request.Icon,
            };

            _context.Widgets.Add(widget);
            await _context.SaveChangesAsync(cancellationToken);
            return CommandResult.Succeeded();
        }
    }
}