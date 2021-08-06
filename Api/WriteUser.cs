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

        public WriteUser(ILogger<WriteUser> logger,
                         CosmosDBRepository<UserContactInfo> cosmosRepository)
        {
            _logger = logger;
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

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            UserContactInfo userInfo = JsonConvert.DeserializeObject<UserContactInfo>(requestBody);
            userInfo.LogicalKey = clientPrincipal.GetUserKey();
            UserContactInfo updatedUserInfo = await _cosmosRepository.UpsertItem(userInfo);

            return new OkObjectResult(updatedUserInfo);
        }
    }
}
