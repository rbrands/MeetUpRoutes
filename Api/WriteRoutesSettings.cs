using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using BlazorApp.Shared;
using System.Web.Http;
using BlazorApp.Api.Repositories;
using BlazorApp.Api.Utils;

namespace BlazorApp.Api
{
    public class WriteRoutesSettings
    {
        private readonly ILogger _logger;
        private CosmosDBRepository<RoutesSettings> _cosmosRepository;
        private CosmosDBRepository<UserContactInfo> _cosmosUserRepository;
        private TenantSettingsRepository _tenantSettingsRepository;
        private ServerSettingsRepository _serverSettingsRepository;

        public WriteRoutesSettings(ILogger<WriteRoutesSettings> logger,
                         ServerSettingsRepository serverSettingsRepository,
                         TenantSettingsRepository tenantSettingsRepository,
                         CosmosDBRepository<UserContactInfo> cosmosUserRepository,
                         CosmosDBRepository<RoutesSettings> cosmosRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _tenantSettingsRepository = tenantSettingsRepository;
            _cosmosUserRepository = cosmosUserRepository;
            _cosmosRepository = cosmosRepository;
        }

        /// <summary>
        /// Writes user contact details to the database. 
        /// </summary>
        [FunctionName("WriteRoutesSettings")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "WriteRoutesSettings")] HttpRequest req
            )
        {
            try
            {
                CallingContext callingContext = await CallingContext.CreateCallingContext(req, _tenantSettingsRepository, _serverSettingsRepository, _cosmosUserRepository);
                callingContext.AssertTenantAdminAccess();

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                RoutesSettings routesSettings = JsonConvert.DeserializeObject<RoutesSettings>(requestBody);
                // Set tenant again to ensure that the data is written to the correct tenant!
                routesSettings.Tenant = callingContext.TenantSettings.TrackKey;

                routesSettings.LogicalKey = $"{callingContext.TenantSettings.TrackKey}-{Constants.KEY_ROUTES_SETTINGS}";
                RoutesSettings updatedSettings = await _cosmosRepository.UpsertItem(routesSettings);

                return new OkObjectResult(updatedSettings);
            }
            catch (Exception ex)
            {
                return new BadRequestErrorMessageResult(ex.Message);
            }
        }
    }
}
