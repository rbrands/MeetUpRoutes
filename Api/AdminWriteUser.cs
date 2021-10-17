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
    public class AdminWriteUser
    {
        private readonly ILogger _logger;
        private CosmosDBRepository<UserContactInfo> _cosmosRepository;
        private TenantSettingsRepository _tenantSettingsRepository;
        private ServerSettingsRepository _serverSettingsRepository;

        public AdminWriteUser(ILogger<WriteUser> logger,
                         ServerSettingsRepository serverSettingsRepository,
                         TenantSettingsRepository tenantSettingsRepository,
                         CosmosDBRepository<UserContactInfo> cosmosRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _tenantSettingsRepository = tenantSettingsRepository;
            _cosmosRepository = cosmosRepository;
        }

        [FunctionName("AdminWriteUser")]
        /// <summary>
        /// Writes user contact details to the database. 
        /// </summary>
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "AdminWriteUser")] HttpRequest req
            )
        {
            try
            {
                TenantSettings tenantSettings = await UserDetails.AssertTenantAdminAccess(req, _tenantSettingsRepository);
                ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings(tenantSettings);

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                UserContactInfo userInfo = JsonConvert.DeserializeObject<UserContactInfo>(requestBody);
                // Set tenant again to ensure that the data is written to the correct tenant!
                userInfo.Tenant = tenantSettings.TrackKey;
                userInfo.LogicalKey = tenantSettings.TrackKey + "-" + userInfo.UserKey;
                // Update last modified
                userInfo.LastModified = DateTime.UtcNow;

                UserContactInfo updatedUserInfo = await _cosmosRepository.UpsertItem(userInfo);

                return new OkObjectResult(updatedUserInfo);
            }
            catch (Exception ex)
            {
                return new BadRequestErrorMessageResult(ex.Message);
            }
        }
    }
}
