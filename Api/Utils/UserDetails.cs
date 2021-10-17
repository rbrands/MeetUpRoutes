using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using BlazorApp.Shared;
using BlazorApp.Api.Repositories;

namespace BlazorApp.Api.Utils
{
    public static class UserDetails
    {
        public static ClientPrincipal GetClientPrincipal(HttpRequest req)
        {
            ClientPrincipal user = new ClientPrincipal()
            {
                IdentityProvider = "devtest",
                UserId = "test123",
                UserDetails = "rbrands",
                UserRoles = new String[] { "anonymous", "authenticated", "admin" }
            };

            string header = req.Headers["x-ms-client-principal"];
            if (!String.IsNullOrEmpty(header))
            {
                var decoded = System.Convert.FromBase64String(header);
                var json = System.Text.ASCIIEncoding.ASCII.GetString(decoded);
                user = JsonSerializer.Deserialize<ClientPrincipal>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            return user;
        }

        public static async Task<TenantSettings>  AssertTenantAdminAccess(HttpRequest req, TenantSettingsRepository tenantRepository)
        {
            ClientPrincipal clientPrincipal = Utils.UserDetails.GetClientPrincipal(req);
            TenantSettings tenantSettings = await GetTenantSettings(req, tenantRepository);
            if (!clientPrincipal.IsInRole(tenantSettings.AdminRole) && !clientPrincipal.IsInRole(Constants.ROLE_ADMIN))
            {
                throw new UnauthorizedAccessException($"User {clientPrincipal.UserDetails} not authorized for tenant {tenantSettings.TenantName}");
            }
            return tenantSettings;
        }
        public static async Task<TenantSettings> AssertTenantAuthenticatedAccess(HttpRequest req, TenantSettingsRepository tenantRepository)
        {
            ClientPrincipal clientPrincipal = Utils.UserDetails.GetClientPrincipal(req);
            TenantSettings tenantSettings = await GetTenantSettings(req, tenantRepository);
            if (!clientPrincipal.IsUserAuthenticated())
            {
                throw new UnauthorizedAccessException($"User {clientPrincipal.UserDetails} not authenticated");
            }
            return tenantSettings;
        }

        private static async Task<TenantSettings> GetTenantSettings(HttpRequest req, TenantSettingsRepository tenantRepository)
        {
            string tenant = req.Headers[Constants.HEADER_TENANT];
            if (String.IsNullOrEmpty(tenant))
            {
                throw new UnauthorizedAccessException($"Tenant in header {Constants.HEADER_TENANT} empty.");
            }
            TenantSettings tenantSettings = await tenantRepository.GetTenantSettings(tenant);
            if (null == tenant)
            {
                throw new UnauthorizedAccessException($"Tenant with key {tenant} not found.");
            }
            return tenantSettings;
        }
    }
}
