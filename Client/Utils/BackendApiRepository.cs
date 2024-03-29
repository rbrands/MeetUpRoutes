﻿using System;
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
        public async Task<UserContactInfo> GetUserForEdit(string key)
        {
            this.PrepareHttpClient();
            return await _http.GetFromJsonAsync<UserContactInfo>($"/api/GetUserForEdit/{key}");
        }
        public async Task<UserContactInfo> WriteUserAfterEdit(UserContactInfo userContactInfo)
        {
            this.PrepareHttpClient();
            HttpResponseMessage response = await _http.PostAsJsonAsync<UserContactInfo>($"/api/WriteUserAfterEdit", userContactInfo);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UserContactInfo>();
        }
        public async Task DeleteUser(UserContactInfo user)
        {
            this.PrepareHttpClient();
            HttpResponseMessage response = await _http.PostAsJsonAsync<UserContactInfo>($"/api/DeleteUser", user);
            response.EnsureSuccessStatusCode();
        }
        public async Task<TagSet> WriteTagSet(TagSet tagSet)
        {
            this.PrepareHttpClient();
            HttpResponseMessage response = await _http.PostAsJsonAsync<TagSet>($"/api/WriteTagSet", tagSet);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TagSet>();
        }
        public async Task<RoutesSettings> WriteRoutesSettings(RoutesSettings settings)
        {
            this.PrepareHttpClient();
            HttpResponseMessage response = await _http.PostAsJsonAsync<RoutesSettings>($"/api/WriteRoutesSettings", settings);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<RoutesSettings>();
        }
        public async Task<Article> WriteArticle(Article article)
        {
            this.PrepareHttpClient();
            HttpResponseMessage response = await _http.PostAsJsonAsync<Article>($"/api/WriteArticle", article);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Article>();
        }
        public async Task DeleteTagSet(TagSet tagSet)
        {
            this.PrepareHttpClient();
            HttpResponseMessage response = await _http.PostAsJsonAsync<TagSet>($"/api/DeleteTagSet", tagSet);
            response.EnsureSuccessStatusCode();
        }
        public async Task DeleteRoute(Route route)
        {
            this.PrepareHttpClient();
            HttpResponseMessage response = await _http.PostAsJsonAsync<Route>($"/api/DeleteRoute", route);
            response.EnsureSuccessStatusCode();
        }
        public async Task DeleteComment(Comment comment)
        {
            this.PrepareHttpClient();
            HttpResponseMessage response = await _http.PostAsJsonAsync<Comment>($"/api/DeleteComment", comment);
            response.EnsureSuccessStatusCode();
        }
        public async Task<Route> WriteRoute(Route route)
        {
            this.PrepareHttpClient();
            HttpResponseMessage response = await _http.PostAsJsonAsync<Route>($"/api/WriteRoute", route);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Route>();
        }
        public async Task<Comment> WriteComment(Comment comment)
        {
            this.PrepareHttpClient();
            HttpResponseMessage response = await _http.PostAsJsonAsync<Comment>($"/api/WriteComment", comment);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Comment>();
        }
        public async Task<LinkPreview> GetLinkPreview(LinkPreview linkPreview)
        {
            this.PrepareHttpClient();
            HttpResponseMessage response = await _http.PostAsJsonAsync<LinkPreview>($"/api/GetLinkPreview", linkPreview);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<LinkPreview>();
        }
        public async Task<TagSet> GetTagSet(string id)
        {
            this.PrepareHttpClient();
            return await _http.GetFromJsonAsync<TagSet>($"/api/GetTagSet/{id}");
        }
        public async Task<ExtendedRoute> GetRoute(string id)
        {
            this.PrepareHttpClient();
            return await _http.GetFromJsonAsync<ExtendedRoute>($"/api/GetRoute/{id}");
        }
        public async Task<ExtendedComment> GetComment(string id)
        {
            this.PrepareHttpClient();
            return await _http.GetFromJsonAsync<ExtendedComment>($"/api/GetComment/{id}");
        }
        public async Task<Article> GetArticle(string key)
        {
            this.PrepareHttpClient();
            return await _http.GetFromJsonAsync<Article>($"/api/GetArticle/{key}");
        }
        public async Task<IEnumerable<ExtendedRoute>> GetRoutes(RouteFilter filter)
        {
            this.PrepareHttpClient();
            HttpResponseMessage response = await _http.PostAsJsonAsync<RouteFilter>($"/api/GetRoutes", filter);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<IEnumerable<ExtendedRoute>>();
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

        public async Task<IQueryable<UserContactInfo>> GetUsers()
        {
            this.PrepareHttpClient();
            return (await _http.GetFromJsonAsync<IEnumerable<UserContactInfo>>($"/api/GetUsers")).AsQueryable();
        }
        public async Task<IEnumerable<TagSet>> GetTagSets()
        {
            this.PrepareHttpClient();
            return await _http.GetFromJsonAsync<IEnumerable<TagSet>>($"/api/GetTagSets");
        }
        public async Task<RoutesSettings> GetRoutesSettings()
        {
            this.PrepareHttpClient();
            return await _http.GetFromJsonAsync<RoutesSettings>($"/api/GetRoutesSettings");
        }
        public async Task<IEnumerable<MeetingPlace>> GetMeetingPlaces()
        {
            this.PrepareHttpClient();
            return await _http.GetFromJsonAsync<IEnumerable<MeetingPlace>>($"/api/GetMeetingPlaces");
        }

    }
}
