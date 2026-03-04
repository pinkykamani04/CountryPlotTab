// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.Infrastructure.Entities
{
    public class JobRoles : BaseEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid? UserId { get; set; }

    }
}