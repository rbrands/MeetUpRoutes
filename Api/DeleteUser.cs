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
    public class DeleteUser
    {
        private readonly ILogger _logger;
        private CosmosDBRepository<UserContactInfo> _cosmosRepository;
        private TenantSettingsRepository _tenantSettingsRepository;
        private ServerSettingsRepository _serverSettingsRepository;


        public DeleteUser(ILogger<WriteUser> logger,
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
        [FunctionName("DeleteUser")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "DeleteUser")] HttpRequest req
            )
        {
            try
            {
                CallingContext callingContext = await CallingContext.CreateCallingContext(req, _tenantSettingsRepository, _serverSettingsRepository, _cosmosRepository);
                callingContext.AssertTenantAdminAccess();
                _logger.LogInformation($"DeleteUser for {callingContext.User.Principal.UserDetails}");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                UserContactInfo userInfo = JsonConvert.DeserializeObject<UserContactInfo>(requestBody);
                userInfo.LogicalKey = callingContext.TenantSettings.TrackKey + "-" + callingContext.User.Principal.GetUserKey();
                userInfo.UserKey = callingContext.User.Principal.GetUserKey();
                userInfo.Tenant = callingContext.TenantSettings.TrackKey;
                await _cosmosRepository.DeleteItemAsync(userInfo.Id);

                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete failed.");
                return new BadRequestErrorMessageResult(ex.Message);
            }
        }
    }
}
