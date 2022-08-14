using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorApp.Api.Utils
{
    public class Constants
    {
        public const string HEADER_TENANT = "x-meetup-tenant";
        public const string HEADER_TENANT_URL = "x-meetup-tenant-url";

        public const string KEY_SERVER_SETTINGS = "serversettings";
        public const string KEY_ROUTES_SETTINGS = "routessettings";

        public const string VERSION = "2022-08-14";

        public const string ROLE_ADMIN = "admin";

        public static int MAX_ROUTES_COUNT = 80;


    }
}
