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
    public class WriteUserAfterEdit
    {
        private readonly ILogger _logger;
        private CosmosDBRepository<UserContactInfo> _cosmosRepository;
        private TenantSettingsRepository _tenantSettingsRepository;
        private ServerSettingsRepository _serverSettingsRepository;

        public WriteUserAfterEdit(ILogger<WriteUserAfterEdit> logger,
                         ServerSettingsRepository serverSettingsRepository,
                         TenantSettingsRepository tenantSettingsRepository,
                         CosmosDBRepository<UserContactInfo> cosmosRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _tenantSettingsRepository = tenantSettingsRepository;
            _cosmosRepository = cosmosRepository;
        }

        /// <summary>
        /// Writes user contact details to the database. 
        /// </summary>
        [FunctionName("WriteUserAfterEdit")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "WriteUserAfterEdit")] HttpRequest req
            )
        {
            try
            {
                CallingContext callingContext = await CallingContext.CreateCallingContext(req, _tenantSettingsRepository, _serverSettingsRepository, _cosmosRepository);
                callingContext.AssertTenantAdminAccess();

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                UserContactInfo userInfo = JsonConvert.DeserializeObject<UserContactInfo>(requestBody);
                // Set tenant again to ensure that the data is written to the correct tenant!
                userInfo.Tenant = callingContext.TenantSettings.TrackKey;
                userInfo.LogicalKey = callingContext.TenantSettings.TrackKey + "-" + userInfo.UserKey;
                // Update last modified
                userInfo.LastModified = DateTime.UtcNow;

                UserContactInfo updatedUserInfo = await _cosmosRepository.UpsertItem(userInfo);

                return new OkObjectResult(updatedUserInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WriteUserAfterEdit failed");
                return new BadRequestErrorMessageResult(ex.Message);
            }
        }
    }
}
