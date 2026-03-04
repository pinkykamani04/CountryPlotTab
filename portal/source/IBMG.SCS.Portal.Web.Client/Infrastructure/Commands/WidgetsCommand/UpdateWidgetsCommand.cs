using BluQube.Attributes;
using BluQube.Commands;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.WidgetsCommand
{
    [BluQubeCommand(Path = "commands/widgets/update")]
    public record UpdateWidgetsCommand(
        Guid Id,
        string Name,
        bool IsMandatory,
        string Icon
    ) : ICommand;
}