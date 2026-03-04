// Copyright (c) IBMG. All rights reserved.

using BluQube.Queries;
using PearDrop.Authentication.Client.Constants;

namespace IBMG.SCS.Portal.Web.Client.Data
{
    public sealed class QueryableUser : IQueryResult
    {
        public Guid Id { get; set; }

        public string Firstname { get; set; } = string.Empty;

        public string Lastname { get; set; } = string.Empty;

        public string UserPrincipalName { get; set; } = string.Empty;

        public bool IsLocked { get; set; }

        public bool IsDisabled { get; set; }

        public DateTime CreatedDate { get; set; }

        public UserPrincipalNameStatus Status { get; set; }
    }
}