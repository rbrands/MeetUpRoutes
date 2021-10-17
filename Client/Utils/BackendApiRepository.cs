using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using BlazorApp.Shared;
using Newtonsoft.Json;

namespace BlazorApp.Client.Utils
{
    public class BackendApiRepository
    {
        HttpClient _http;
        AppState _appState;

        private void PrepareHttpClient()
        {
            _http.DefaultRequestHeaders.Remove("x-meetup-tenant");
            if (!String.IsNullOrEmpty(_appState.TrackKey))
            {
                _http.DefaultRequestHeaders.Add("x-meetup-tenant", _appState.TrackKey);
            }
        }
        public BackendApiRepository(HttpClient http, AppState appState)
        {
            _http = http;
            _appState = appState;
        }

        public async Task<ClientPrincipal> GetClientPrincipal()
        {
            this.PrepareHttpClient();
            return await _http.GetFromJsonAsync<ClientPrincipal>($"/api/GetClientPrincipal");
        }
        public async Task<IEnumerable<TenantSettings>> GetTenants()
        {
            return await _http.GetFromJsonAsync<IEnumerable<TenantSettings>>($"/api/GetTenantSettings");
        }
        public async Task<User> GetUser()
        {
            this.PrepareHttpClient();
            return await _http.GetFromJsonAsync<User>($"/api/GetUser");
        }

        public async Task<string> GetFunctionsVersion()
        {
            this.PrepareHttpClient();
            return await _http.GetStringAsync($"/api/GetVersion");
        }

        public async Task<UserContactInfo> WriteUser(UserContactInfo user)
        {
            this.PrepareHttpClient();
            HttpResponseMessage response = await _http.PostAsJsonAsync<UserContactInfo>($"/api/WriteUser", user);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UserContactInfo>();
        }

        public async Task<IEnumerable<UserContactInfo>> GetUsers()
        {
            this.PrepareHttpClient();
            return await _http.GetFromJsonAsync<IEnumerable<UserContactInfo>>($"/api/GetUsers");
        }

    }
}
