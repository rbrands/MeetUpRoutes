using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using BlazorApp.Shared;
using System.Web.Http;
using BlazorApp.Api.Repositories;
using BlazorApp.Api.Utils;
using System.Collections.Generic;
using System.Linq;


namespace BlazorApp.Api
{
    public class GetRoutesSettings
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private CosmosDBRepository<RoutesSettings> _cosmosRepository;
        private CosmosDBRepository<UserContactInfo> _cosmosUserRepository;
        private TenantSettingsRepository _tenantRepository;
        private ServerSettingsRepository _serverSettingsRepository;

        public GetRoutesSettings(ILogger<GetRoutesSettings> logger,
                        IConfiguration config,
                        ServerSettingsRepository serverSettingsRepository,
                        TenantSettingsRepository tenantRepository,
                        CosmosDBRepository<UserContactInfo> cosmosUserRepository,
                        CosmosDBRepository<RoutesSettings> cosmosRepository
        )
        {
            _logger = logger;
            _config = config;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosUserRepository = cosmosUserRepository;
            _cosmosRepository = cosmosRepository;
            _tenantRepository = tenantRepository;
        }

        [FunctionName(nameof(GetRoutesSettings))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetRoutesSettings")] HttpRequest req)
        {
            try
            {
                CallingContext callingContext = await CallingContext.CreateCallingContext(req, _tenantRepository, _serverSettingsRepository, _cosmosUserRepository);

                RoutesSettings settings = await _cosmosRepository.GetItemByKey($"{callingContext.TenantSettings.TrackKey}-{Constants.KEY_ROUTES_SETTINGS}");
                if (null == settings)
                {
                    settings = new RoutesSettings();
                }
                return new OkObjectResult(settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetRoutesSettings failed.");
                return new BadRequestErrorMessageResult(ex.Message);
            }
        }
    }
}
