// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.Portal.Web.Client.Dtos
{
    public class JobRoleDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid? UserId { get; set; }

        public int UserCount { get; set; }
    }
}