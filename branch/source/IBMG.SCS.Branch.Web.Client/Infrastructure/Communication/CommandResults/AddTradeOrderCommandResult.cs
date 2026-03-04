// Copyright (c) IBMG. All rights reserved.

using BluQube.Commands;

namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.CommandResults;

public record AddTradeOrderCommandResult(
    Guid CustomerId) : ICommandResult;