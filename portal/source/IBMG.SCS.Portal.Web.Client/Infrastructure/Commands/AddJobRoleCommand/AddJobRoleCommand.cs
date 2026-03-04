using BluQube.Attributes;
using BluQube.Commands;
using IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.AddJobRoleCommandResult;
using IBMG.SCS.Portal.Web.Client.Infrastructure.CommandResults.AddSpendLimitCommandResult;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.AddJobRoleCommand
{
    [BluQubeCommand(Path = "commands/jobrole/add")]

    public record AddJobRoleCommand(Guid Id, string Name, Guid? UserId) : ICommand<AddJobRoleCommandResult>;

}
