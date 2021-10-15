using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;
using BlazorApp.Api.Utils;

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
        public async Task<ServerSettings> GetServerSettings(string tenant)
        {
            string settingsKey = Constants.KEY_SERVER_SETTINGS;
            if (!String.IsNullOrWhiteSpace(tenant))
            {
                settingsKey += "-" + tenant;
            }
            ServerSettings serverSettings = await this.GetItemByKey(settingsKey);
            return serverSettings;
        }
    }
}
