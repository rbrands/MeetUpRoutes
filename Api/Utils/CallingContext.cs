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

    public class CallingContext
    {
        private User _user;
        private TenantSettings _tenantSettings;
        private ServerSettings _serverSettings;

        public User User
        {
            get { return _user; }
            set { _user = value; }
        }
        public TenantSettings TenantSettings
        {
            get { return _tenantSettings; }
            set { _tenantSettings = value; }
        }

        public ServerSettings ServerSettings
        { 
            get { return _serverSettings; }
            set { _serverSettings = value; }
        }

        public Boolean IsUserConfirmed
        {
            get
            {
                return (
                            null != _user.ContactInfo
                            && null != _user.Principal
                            && _user.IsAuthenticated
                            && _user.ContactInfo.IsConfirmed
                       );
            }
        }
        public Boolean IsUserAuthor
        {
            get
            {
                return (
                            null != _user.ContactInfo 
                            && null != _user.Principal 
                            && _user.IsAuthenticated 
                            && _user.ContactInfo.IsConfirmed 
                            && (_user.ContactInfo.IsAuthor || _user.Principal.IsInRole(_tenantSettings.AdminRole) || _user.Principal.IsInRole(Constants.ROLE_ADMIN))
                       );
            }
        }
        public Boolean IsUserReviewer
        {
            get
            {
                return (
                            null != _user.ContactInfo 
                            && null != _user.Principal 
                            && _user.IsAuthenticated
                            && _user.ContactInfo.IsConfirmed
                            && (_user.ContactInfo.IsReviewer || _user.Principal.IsInRole(_tenantSettings.AdminRole) || _user.Principal.IsInRole(Constants.ROLE_ADMIN))
                        );
            }
        }
        public Boolean IsDev
        {
            get
            {
                return _user.IsDev;
            }
        }

        public Boolean IsTenantAdmin
        {
            get
            {
                return (_user.Principal.IsInRole(_tenantSettings.AdminRole) || _user.Principal.IsInRole(Constants.ROLE_ADMIN));
            }
        }

        public static async Task<CallingContext> CreateCallingContext(HttpRequest req, TenantSettingsRepository tenantRepository, ServerSettingsRepository serverSettingsRepository, CosmosDBRepository<UserContactInfo> userRepository)
        {
            CallingContext callingContext = new CallingContext();

            callingContext.User = new User();
            callingContext.User.Principal = Utils.UserDetails.GetClientPrincipal(req);
            callingContext.TenantSettings = await GetTenantSettings(req, tenantRepository);
            callingContext.ServerSettings = await serverSettingsRepository.GetServerSettings(callingContext.TenantSettings);

            string key = callingContext.TenantSettings.TrackKey + "-" + callingContext.User.Principal.GetUserKey();
            callingContext.User.ContactInfo = await userRepository.GetItemByKey(key);

            return callingContext;
        }

        private static async Task<TenantSettings> GetTenantSettings(HttpRequest req, TenantSettingsRepository tenantRepository)
        {
            string tenant = req.Headers[Constants.HEADER_TENANT];
            if (String.IsNullOrEmpty(tenant))
            {
                throw new UnauthorizedAccessException($"Tenant in header {Constants.HEADER_TENANT} empty.");
            }
            TenantSettings tenantSettings = await tenantRepository.GetTenantSettings(tenant);
            if (null == tenantSettings)
            {
                throw new UnauthorizedAccessException($"Tenant with key {tenant} not found.");
            }
            return tenantSettings;
        }

        public void AssertTenantAdminAccess()
        {
            if (!IsTenantAdmin)
            {
                throw new UnauthorizedAccessException($"User {_user.Principal.UserDetails} not authorized for tenant {_tenantSettings.TenantName}");
            }
        }

        public void AssertAuthenticatedAccess()
        {
            if (!_user.IsAuthenticated)
            {
                throw new UnauthorizedAccessException($"User {_user.Principal.UserDetails} not authenticated");
            }
        }
        public void AssertConfirmedAccess()
        {
            if (!_user.IsAuthenticated)
            {
                throw new UnauthorizedAccessException($"User {_user.Principal.UserDetails} not authenticated");
            }
            if (null == _user.ContactInfo || !_user.ContactInfo.IsConfirmed)
            {
                throw new UnauthorizedAccessException($"User {_user.Principal.UserDetails} is austhenticated but not confirmed");
            }
        }
        public void AssertReviewerAuthorization()
        {
            if (!IsUserReviewer)
            {
                throw new UnauthorizedAccessException($"User {_user.Principal.UserDetails} has no rights as reviewer.");
            }
        }
        public bool CheckAuthor(string authorId)
        {
            return (
                         null != _user.ContactInfo
                         && null != _user.Principal
                         && _user.IsAuthenticated
                         && _user.ContactInfo.IsConfirmed
                         && (_user.ContactInfo.Id.CompareTo(authorId) == 0)
                     );
        }

    }
}
