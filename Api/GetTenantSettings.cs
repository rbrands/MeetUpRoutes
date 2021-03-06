using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Web.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using BlazorApp.Shared;
using BlazorApp.Api.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace BlazorApp.Api
{
    public class GetTenantSettings
    {
        private readonly ILogger _logger;
        private CosmosDBRepository<TenantSettings> _cosmosRepository;

        public GetTenantSettings(ILogger<GetTenantSettings> logger, CosmosDBRepository<TenantSettings> cosmosRepository)
        {
            _logger = logger;
            _cosmosRepository = cosmosRepository;
        }
        [FunctionName("GetTenantSettings")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("GetTenantSettings");

            IEnumerable<TenantSettings> tenantSettings = (await _cosmosRepository.GetItems(t => t.TracksEnabled == true)).OrderBy(t => t.TenantKey);

            return new OkObjectResult(tenantSettings);
        }
    }
}
