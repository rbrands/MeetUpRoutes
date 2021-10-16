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

namespace BlazorApp.Api
{
    public class GetUsers
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private CosmosDBRepository<UserContactInfo> _cosmosRepository;
        private CosmosDBRepository<TenantSettings> _tenantRepository;
        private ServerSettingsRepository _serverSettingsRepository;

        public GetUsers(ILogger<GetUser> logger,
                        IConfiguration config,
                        ServerSettingsRepository serverSettingsRepository,
                        CosmosDBRepository<TenantSettings> tenantRepository,
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
            ClientPrincipal clientPrincipal = Utils.UserDetails.GetClientPrincipal(req);
            _logger.LogInformation($"GetUsers");
            string tenant = req.Headers[Constants.HEADER_TENANT];
            User user = new User();
            user.Principal = clientPrincipal;
            // Read UserDetails by assembling key
            if (clientPrincipal.IsUserAuthenticated())
            {
                string key = tenant + "-" + clientPrincipal.GetUserKey();
                _logger.LogInformation($"GetUserDetails for user {key}");
                user.ContactInfo = await _cosmosRepository.GetItemByKey(key);
            }

            return new OkObjectResult(user);
        }
    }
}
