using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;
using BlazorApp.Api.Utils;
using BlazorApp.Shared;

namespace BlazorApp.Api.Repositories
{
    public class ServerSettingsRepository : CosmosDBRepository<ServerSettings>
    {
        public ServerSettingsRepository(IConfiguration config, CosmosClient cosmosClient) : base(config, cosmosClient)
        {
        }

        public async Task<ServerSettings> GetServerSettings()
        {
            return await GetServerSettings(null);
        }
        public async Task<ServerSettings> GetServerSettings(TenantSettings tenantSettings)
        {
            string settingsKey = Constants.KEY_SERVER_SETTINGS;
            string tenant = tenantSettings.TenantKey;
            if (!String.IsNullOrWhiteSpace(tenant))
            {
                settingsKey += "-" + tenant;
            }
            ServerSettings serverSettings = await this.GetItemByKey(settingsKey);
            return serverSettings;
        }
    }
}
