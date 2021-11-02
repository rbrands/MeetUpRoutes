using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using BlazorApp.Shared;

namespace BlazorApp.Client.Utils
{
    /// <summary>
    /// Use AppState pattern to hold state across all components
    /// </summary>
    public class AppState
    {
        private TenantSettings _tenantSettings = null;
        private string _trackKey = null;
        private User _user = null;

        public IEnumerable<TenantSettings> Tenants { get; set; } = new List<TenantSettings>();
        public Boolean TenantsAlreadyRead { get; set; } = false;
        public Boolean SettingsAlreadyRead { get; set; } = false;
        public RoutesSettings Settings {get; set;} = new RoutesSettings();

        public TenantSettings Tenant
        {
            get { return _tenantSettings; }
            set
            {
                _tenantSettings = value;
                NotifyStateChanged();
            }
        }
        public string TrackKey
        {
            get { return _trackKey; }
            set
            {
                _trackKey = value;
                NotifyStateChanged();
            } 
        }
        public User CurrentUser
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
                NotifyStateChanged();
            }
        }

        public event Action OnChange;
        public bool NotificationSubscriptionRequested { get; set; } = false;
        public void NotifyStateChanged() => OnChange?.Invoke();

        public AppState()
        {
            Tenant = new TenantSettings();
            Tenant.Tenant = "Demo";
            Tenant.AdminRole = "Demo";
        }

        public bool IsUserConfirmed
        {
            get
            {
                return (null != CurrentUser && CurrentUser.IsAuthenticated && null != CurrentUser.ContactInfo && CurrentUser.ContactInfo.IsConfirmed);
            }
        }
        public void AssertUserIsConfirmed()
        {
            if (!IsUserConfirmed)
            {
                throw new Exception("Benutzer:in ist noch nicht als Mitglied bestätigt.");
            }
        }
        public bool IsUserAuthor
        {
            get
            {
                return (null != CurrentUser && CurrentUser.IsAuthenticated && null != CurrentUser.ContactInfo && CurrentUser.ContactInfo.IsConfirmed && CurrentUser.ContactInfo.IsAuthor);
            }
        }
        public bool IsUserReviewer
        {
            get
            {
                return (null != CurrentUser && CurrentUser.IsAuthenticated && null != CurrentUser.ContactInfo && CurrentUser.ContactInfo.IsConfirmed && CurrentUser.ContactInfo.IsReviewer);
            }
        }
        public bool IsDev
        {
            get
            {
                return (null != CurrentUser && CurrentUser.IsDev);
            }
        }
        public Boolean IsUserGivenAuthor(string id)
        {
            return (null != CurrentUser && 
                    CurrentUser.IsAuthenticated && 
                    null != CurrentUser.ContactInfo && 
                    CurrentUser.ContactInfo.IsConfirmed &&
                    !String.IsNullOrEmpty(id) &&
                    CurrentUser.ContactInfo.Id.CompareTo(id) == 0);
        }
    }
}
