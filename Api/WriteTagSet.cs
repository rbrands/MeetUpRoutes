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
    public class WriteTagSet
    {
        private readonly ILogger _logger;
        private CosmosDBRepository<TagSet> _cosmosRepository;
        private CosmosDBRepository<UserContactInfo> _cosmosUserRepository;
        private TenantSettingsRepository _tenantSettingsRepository;
        private ServerSettingsRepository _serverSettingsRepository;

        public WriteTagSet(ILogger<WriteTagSet> logger,
                         ServerSettingsRepository serverSettingsRepository,
                         TenantSettingsRepository tenantSettingsRepository,
                         CosmosDBRepository<UserContactInfo> cosmosUserRepository,
                         CosmosDBRepository<TagSet> cosmosRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _tenantSettingsRepository = tenantSettingsRepository;
            _cosmosUserRepository = cosmosUserRepository;
            _cosmosRepository = cosmosRepository;
        }

        /// <summary>
        /// Writes user contact details to the database. 
        /// </summary>
        [FunctionName("WriteTagSet")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "WriteTagSet")] HttpRequest req
            )
        {
            try
            {
                CallingContext callingContext = await CallingContext.CreateCallingContext(req, _tenantSettingsRepository, _serverSettingsRepository, _cosmosUserRepository);
                callingContext.AssertTenantAdminAccess();

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                TagSet tagSet = JsonConvert.DeserializeObject<TagSet>(requestBody);
                // Set tenant again to ensure that the data is written to the correct tenant!
                tagSet.Tenant = callingContext.TenantSettings.TrackKey;

                TagSet updatedTagSet = await _cosmosRepository.UpsertItem(tagSet);

                return new OkObjectResult(updatedTagSet);
            }
            catch (Exception ex)
            {
                return new BadRequestErrorMessageResult(ex.Message);
            }
        }
    }
}
