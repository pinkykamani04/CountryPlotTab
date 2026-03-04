using BluQube.Attributes;
using BluQube.Commands;

namespace IBMG.SCS.Portal.Web.Client.Infrastructure.Commands.Auth;

// Wrapper for server endpoint: api/commands/auth/cross-tenant/create-user
// Uses BluQube path routing to invoke the server command processor.
[BluQubeCommand(Path = "api/commands/auth/cross-tenant/create-users")] 
public sealed record CreateUserInTenantCommand(
    Guid TenantId,
    string FirstName,
    string LastName,
    string Email
) : ICommand;
