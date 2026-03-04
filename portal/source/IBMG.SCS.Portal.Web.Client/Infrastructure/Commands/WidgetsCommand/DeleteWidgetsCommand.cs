using BluQube.Attributes;
using BluQube.Commands;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.WidgetsCommand
{
    [BluQubeCommand(Path = "commands/widgets/delete")]
    public record DeleteWidgetsCommand(
        Guid Id
    ) : ICommand;
}