// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.Infrastructure.Entities;

public class BranchKerridgeMap
{
    public int Id { get; set; }

    public string BranchCode { get; set; } = null!;

    public string BranchName { get; set; } = null!;

    public string PostCode { get; set; } = null!;

    public string KerridgeInstance { get; set; } = null!;
}