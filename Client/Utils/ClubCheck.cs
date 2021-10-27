using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Shared;

namespace BlazorApp.Client.Utils
{
    public class ClubCheck
    {
        private AppState _appStateStore;
        private BackendApiRepository _backendApi;
        public ClubCheck(AppState appStateStore, BackendApiRepository backendApi)
        {
            _appStateStore = appStateStore;
            _backendApi = backendApi;
        }

        public async Task<Boolean> SetClub(string trackKey)
        {
            Boolean validTrackKey = false;
            // Read all tenant settings
            if (!_appStateStore.TenantsAlreadyRead)
            {
                _appStateStore.Tenants = await _backendApi.GetTenants();
                _appStateStore.TenantsAlreadyRead = true;
            }
            if (!String.IsNullOrEmpty(trackKey))
            { 
                _appStateStore.TrackKey = trackKey;
                validTrackKey = SetTenant(trackKey);
                if (!validTrackKey)
                {
                    _appStateStore.TrackKey = null;
                }
                _appStateStore.CurrentUser = await _backendApi.GetUser();
            }
            return validTrackKey;
        }
        private Boolean SetTenant(string trackKey)
        {
            Boolean validTrackKey = false;
            if (!String.IsNullOrEmpty(trackKey))
            {
                // If tenant key (TrackKey) is given as part of the query, check the list if a corresponding item can be found
                foreach (TenantSettings tenant in _appStateStore.Tenants)
                {
                    if (tenant.TracksEnabled && !String.IsNullOrEmpty(tenant.TrackKey) && String.Compare(tenant.TrackKey, trackKey) == 0)
                    {
                        _appStateStore.Tenant = tenant;
                        _appStateStore.TrackKey = tenant.TrackKey;
                        validTrackKey = true;
                        break;
                    }
                }
                if (!validTrackKey)
                {
                    _appStateStore.TrackKey = null;
                }
            }
            return validTrackKey;
        }

    }
}
