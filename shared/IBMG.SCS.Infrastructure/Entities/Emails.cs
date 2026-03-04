// Copyright (c) IBMG. All rights reserved.

namespace IBMG.SCS.Infrastructure.Entities
{
    public class Emails : BaseEntity
    {
        public Guid Id { get; set; }

        public string Email { get; set; }
    }
}