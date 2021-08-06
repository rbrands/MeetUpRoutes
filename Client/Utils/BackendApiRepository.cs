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

        public BackendApiRepository(HttpClient http)
        {
            _http = http;
        }

        public async Task<ClientPrincipal> GetClientPrincipal()
        {
            return await _http.GetFromJsonAsync<ClientPrincipal>($"/api/GetClientPrincipal");
        }
        public async Task<User> GetUser()
        {
            return await _http.GetFromJsonAsync<User>($"/api/GetUser");
        }
    }
}
