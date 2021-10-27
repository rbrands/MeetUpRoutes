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
    public class DeleteTagSet
    {
        private readonly ILogger _logger;
        private CosmosDBRepository<TagSet> _cosmosRepository;
        private CosmosDBRepository<UserContactInfo> _cosmosUserRepository;
        private TenantSettingsRepository _tenantSettingsRepository;
        private ServerSettingsRepository _serverSettingsRepository;

        public DeleteTagSet(ILogger<DeleteTagSet> logger,
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

        [FunctionName("DeleteTagSet")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "DeleteTagSet")] HttpRequest req
            )
        {
            try
            {
                CallingContext callingContext = await CallingContext.CreateCallingContext(req, _tenantSettingsRepository, _serverSettingsRepository, _cosmosUserRepository);
                callingContext.AssertTenantAdminAccess();

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                TagSet tagSet = JsonConvert.DeserializeObject<TagSet>(requestBody);
                if (String.IsNullOrEmpty(tagSet.Id))
                {
                    return new BadRequestErrorMessageResult("Die Id des TagSet fehlt.");
                }
                await _cosmosRepository.DeleteItemAsync(tagSet.Id);

                return new OkResult();
            }
            catch (Exception ex)
            {
                return new BadRequestErrorMessageResult(ex.Message);
            }
        }
    }
}
