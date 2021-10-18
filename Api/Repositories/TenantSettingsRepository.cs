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
    public class TenantSettingsRepository : CosmosDBRepository<TenantSettings>
    {
        public TenantSettingsRepository(IConfiguration config, CosmosClient cosmosClient) : base(config, cosmosClient)
        {
        }
        public async Task<TenantSettings> GetTenantSettings(string tenant)
        {
            TenantSettings tenantSettings = await this.GetFirstItemOrDefault(t => t.TrackKey.CompareTo(tenant) == 0);

            return tenantSettings;
        }

    }
}
