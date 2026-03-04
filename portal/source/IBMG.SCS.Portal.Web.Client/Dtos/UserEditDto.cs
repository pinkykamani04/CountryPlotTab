// Copyright (c) IBMG. All rights reserved.

using PearDrop.Authentication.Client.Constants;

namespace IBMG.SCS.Portal.Web.Client.Dtos
{
    public class UserEditDto
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = "";

        public string LastName { get; set; } = "";

        public string Email { get; set; } = "";

        public UserPrincipalNameStatus Status { get; set; }
    }
}