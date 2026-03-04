using BluQube.Attributes;
using BluQube.Commands;
using IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.DeleteJobRoleCommandResult;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.DeleteJobRoleCommand
{
    [BluQubeCommand(Path = "commands/jobrole/delete")]
    public record DeleteJobRoleCommand(Guid Id, Guid UserId) : ICommand<DeleteJobRoleCommandResult>;
}