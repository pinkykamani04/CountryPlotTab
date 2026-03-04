// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.Branch.Web.Client.Infrastructure.Models
{
    public class CustomerItem
    {
        public string? Text { get; set; }

        public string? Value { get; set; }

        public ICollection<string> AccountNumbers { get; set; } = new List<string>();
    }
}