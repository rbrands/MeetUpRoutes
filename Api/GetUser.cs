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
    public class GetUser
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private CosmosDBRepository<UserContactInfo> _cosmosRepository;
        public GetUser(ILogger<GetUser> logger,
                       IConfiguration config,
                       CosmosDBRepository<UserContactInfo> cosmosRepository
        )
        {
            _logger = logger;
            _config = config;
            _cosmosRepository = cosmosRepository;
        }

        [FunctionName(nameof(GetUser))]
        public  async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetUser")] HttpRequest req)
        {
            ClientPrincipal clientPrincipal = Utils.UserDetails.GetClientPrincipal(req);
            _logger.LogInformation($"GetUser for user {clientPrincipal.UserDetails} with IdentityProvider {clientPrincipal.IdentityProvider}");
            User user = new User();
            user.Principal = clientPrincipal;
            // Read UserDetails by assembling key
            if (clientPrincipal.IsUserAuthenticated())
            { 
                string key = clientPrincipal.GetUserKey();
                _logger.LogInformation($"GetUserDetails for user {key}");
                user.ContactInfo = await _cosmosRepository.GetItemByKey(key);
            }

            return new OkObjectResult(user);
        }
    }
}