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
        private ServerSettingsRepository _serverSettingsRepository;


        public WriteUser(ILogger<WriteUser> logger,
                         ServerSettingsRepository serverSettingsRepository,
                         CosmosDBRepository<UserContactInfo> cosmosRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
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
            ClientPrincipal clientPrincipal = UserDetails.GetClientPrincipal(req);
            _logger.LogInformation($"WriteUser for {clientPrincipal.UserDetails}");
            if (!clientPrincipal.IsUserAuthenticated())
            {
                _logger.LogError($"User {clientPrincipal.UserDetails} is only a testuser and not authenticated");
                return new BadRequestErrorMessageResult($"User not authenticated.");
            }
            string tenant = req.Headers[Constants.HEADER_TENANT];
            if (String.IsNullOrWhiteSpace(tenant))
            {
                tenant = null;
            }
            ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings(tenant);

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            UserContactInfo userInfo = JsonConvert.DeserializeObject<UserContactInfo>(requestBody);
            userInfo.LogicalKey = tenant + "-" + clientPrincipal.GetUserKey();
            userInfo.Tenant = tenant;
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
                userInfo.IsConfirmed = serverSettings.IsUser(userInfo.RegistrationCode);
            }

            UserContactInfo updatedUserInfo = await _cosmosRepository.UpsertItem(userInfo);

            return new OkObjectResult(updatedUserInfo);
        }
    }
}
