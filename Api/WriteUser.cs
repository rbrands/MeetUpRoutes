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
    public class WriteUser
    {
        private readonly ILogger _logger;
        private CosmosDBRepository<UserContactInfo> _cosmosRepository;
        private TenantSettingsRepository _tenantSettingsRepository;
        private ServerSettingsRepository _serverSettingsRepository;


        public WriteUser(ILogger<WriteUser> logger,
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
        [FunctionName("WriteUser")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "WriteUser")] HttpRequest req
            )
        {
            try
            {
                CallingContext callingContext = await CallingContext.CreateCallingContext(req, _tenantSettingsRepository, _serverSettingsRepository, _cosmosRepository);
                callingContext.AssertAuthenticatedAccess();
                _logger.LogInformation($"WriteUser for {callingContext.User.Principal.UserDetails}");

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                UserContactInfo userInfo = JsonConvert.DeserializeObject<UserContactInfo>(requestBody);
                userInfo.LogicalKey = callingContext.TenantSettings.TrackKey + "-" + callingContext.User.Principal.GetUserKey();
                userInfo.UserKey = callingContext.User.Principal.GetUserKey();
                userInfo.Tenant = callingContext.TenantSettings.TrackKey;
                // Check if there are already UserContactInfo stored in database to ensure that the user doesn't overwrite his permissions on his own 
                UserContactInfo userInfoAlreadyStored = await _cosmosRepository.GetItemByKey(userInfo.LogicalKey);
                if (null != userInfoAlreadyStored)
                {
                    userInfo.IsConfirmed = userInfoAlreadyStored.IsConfirmed;
                    userInfo.IsAuthor = userInfoAlreadyStored.IsAuthor;
                    userInfo.IsReviewer = userInfoAlreadyStored.IsReviewer;
                }
                // Update last modified
                userInfo.LastModified = DateTime.UtcNow;
                // Check registration code
                if (!userInfo.IsConfirmed && !String.IsNullOrEmpty(userInfo.RegistrationCode))
                {
                    userInfo.IsConfirmed = callingContext.ServerSettings.IsUser(userInfo.RegistrationCode);
                }

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
