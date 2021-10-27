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
    public class DeleteRoute
    {
        private readonly ILogger _logger;
        private CosmosDBRepository<Route> _cosmosRepository;
        private CosmosDBRepository<UserContactInfo> _cosmosUserRepository;
        private TenantSettingsRepository _tenantSettingsRepository;
        private ServerSettingsRepository _serverSettingsRepository;

        public DeleteRoute(ILogger<DeleteRoute> logger,
                         ServerSettingsRepository serverSettingsRepository,
                         TenantSettingsRepository tenantSettingsRepository,
                         CosmosDBRepository<UserContactInfo> cosmosUserRepository,
                         CosmosDBRepository<Route> cosmosRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _tenantSettingsRepository = tenantSettingsRepository;
            _cosmosUserRepository = cosmosUserRepository;
            _cosmosRepository = cosmosRepository;
        }

        [FunctionName("DeleteRoute")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "DeleteRoute")] HttpRequest req
            )
        {
            try
            {
                CallingContext callingContext = await CallingContext.CreateCallingContext(req, _tenantSettingsRepository, _serverSettingsRepository, _cosmosUserRepository);
                callingContext.AssertConfirmedAccess();

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                Route route = JsonConvert.DeserializeObject<Route>(requestBody);
                if (String.IsNullOrEmpty(route.Id))
                {
                    return new BadRequestErrorMessageResult("Die Id der Route fehlt.");
                }

                // Check if caller is allowed to delete this route (needs to be author or reviewer)
                if (!callingContext.CheckAuthor(route.AuthorId))
                {
                    callingContext.AssertReviewerAuthorization();
                }

                await _cosmosRepository.DeleteItemAsync(route.Id);

                return new OkResult();
            }
            catch (Exception ex)
            {
                return new BadRequestErrorMessageResult(ex.Message);
            }
        }
    }
}
