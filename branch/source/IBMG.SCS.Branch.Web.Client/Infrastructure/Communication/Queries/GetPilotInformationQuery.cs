using BluQube.Attributes;
using BluQube.Queries;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.QueryResults;
using System;

namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.Queries
{
    [BluQubeQuery(Path = "queries/pilot/get")]
    public record GetPilotInformationQuery(
        Guid? Id,
        string? EmailAddress
    ) : IQuery<GetPilotInformationQueryResult>;
}
