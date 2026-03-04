using BluQube.Attributes;
using BluQube.Commands;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.WidgetsCommand
{
    [BluQubeCommand(Path = "commands/widgets/add")]
    public record AddWidgetsCommand(
        string Name,
        bool IsMandatory,
        string Icon
    ) : ICommand;
}