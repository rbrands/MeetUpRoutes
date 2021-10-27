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
using System.Collections.Generic;
using BlazorApp.Api.Repositories;
using BlazorApp.Api.Utils;

namespace BlazorApp.Api
{
    public class GetTagSet
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private CosmosDBRepository<TagSet> _cosmosRepository;
        private CosmosDBRepository<UserContactInfo> _cosmosUserRepository;

        private TenantSettingsRepository _tenantRepository;
        private ServerSettingsRepository _serverSettingsRepository;
        public GetTagSet(ILogger<GetUserForEdit> logger,
                        IConfiguration config,
                        ServerSettingsRepository serverSettingsRepository,
                        TenantSettingsRepository tenantRepository,
                         CosmosDBRepository<UserContactInfo> cosmosUserRepository,
                        CosmosDBRepository<TagSet> cosmosRepository
        )
        {
            _logger = logger;
            _config = config;
            _serverSettingsRepository = serverSettingsRepository;
            _tenantRepository = tenantRepository;
            _cosmosUserRepository = cosmosUserRepository;
            _cosmosRepository = cosmosRepository;
        }

        [FunctionName(nameof(GetTagSet))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetTagSet/{id}")] HttpRequest req, string id)
        {
            try
            {
                CallingContext callingContext = await CallingContext.CreateCallingContext(req, _tenantRepository, _serverSettingsRepository, _cosmosUserRepository);
                callingContext.AssertTenantAdminAccess();

                TagSet tagSet = await _cosmosRepository.GetItem(id);

                return new OkObjectResult(tagSet);
            }
            catch (Exception ex)
            {
                return new BadRequestErrorMessageResult(ex.Message);
            }
        }
    }
}
