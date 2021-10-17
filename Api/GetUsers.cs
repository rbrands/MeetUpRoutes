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
    public class GetUsers
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private CosmosDBRepository<UserContactInfo> _cosmosRepository;
        private TenantSettingsRepository _tenantRepository;
        private ServerSettingsRepository _serverSettingsRepository;

        public GetUsers(ILogger<GetUser> logger,
                        IConfiguration config,
                        ServerSettingsRepository serverSettingsRepository,
                        TenantSettingsRepository tenantRepository,
                        CosmosDBRepository<UserContactInfo> cosmosRepository
        )
        {
            _logger = logger;
            _config = config;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
            _tenantRepository = tenantRepository;
        }

        [FunctionName(nameof(GetUsers))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetUsers")] HttpRequest req)
        {
            try
            {
                TenantSettings tenantSettings = await UserDetails.AssertTenantAdminAccess(req, _tenantRepository);

                IEnumerable<UserContactInfo> userInfos = await _cosmosRepository.GetItems(u => u.Tenant.CompareTo(tenantSettings.TrackKey) == 0);

                return new OkObjectResult(userInfos);
            }
            catch (Exception ex)
            {
                return new BadRequestErrorMessageResult(ex.Message);
            }
        }
    }
}
