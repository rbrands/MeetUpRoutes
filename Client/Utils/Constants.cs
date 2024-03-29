﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Client.Utils
{
    public static class Constants
    {
        public const string RoleAuthor = "author";
        public const string RoleReviewer = "reviewer";
        public const string RoleTenantAdmin = "tenantadmin";
        public const string RoleAdmin = "admin";
        public const string RoleAllAdmins = "admin,tenantadmin";
    }
}
