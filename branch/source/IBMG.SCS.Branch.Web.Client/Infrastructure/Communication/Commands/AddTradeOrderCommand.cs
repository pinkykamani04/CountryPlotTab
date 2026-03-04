// Copyright (c) IBMG. All rights reserved.

using BluQube.Attributes;
using BluQube.Commands;
using IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.CommandResults;

namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Communication.Commands;

[BluQubeCommand(Path = "commands/trade/add-order")]
public record AddTradeOrderCommand(
    string Customer,
    string JobNumber,
    string TradeCardNumber,
    string OperativeCode,
    string BranchCode) : ICommand<AddTradeOrderCommandResult>;