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
        TenantSettings _tenantSettings = null;

        public IEnumerable<TenantSettings> Tenants { get; set; } = new List<TenantSettings>();
        public Boolean TenantsAlreadyRead { get; set; } = false;
        public TenantSettings Tenant 
        {
            get { return _tenantSettings; } 
            set
            {
                _tenantSettings = value;
                NotifyStateChanged();
            }
        }
        public string TrackKey { get; set; }
        public User CurrentUser { get; set; }

        public event Action OnChange;
        public bool NotificationSubscriptionRequested { get; set; } = false;
        public void NotifyStateChanged() => OnChange?.Invoke();

        public AppState()
        {
            Tenant = new TenantSettings();
            Tenant.Tenant = "Demo";
            Tenant.AdminRole = "Demo";
        }
    }
}
