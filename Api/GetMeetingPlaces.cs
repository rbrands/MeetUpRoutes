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
using System.Collections.Generic;
using System.Linq;


namespace BlazorApp.Api
{
    public class GetMeetingPlaces
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private CosmosDBRepository<MeetingPlace> _cosmosRepository;
        private CosmosDBRepository<UserContactInfo> _cosmosUserRepository;
        private TenantSettingsRepository _tenantRepository;
        private ServerSettingsRepository _serverSettingsRepository;

        public GetMeetingPlaces(ILogger<GetMeetingPlaces> logger,
                        IConfiguration config,
                        ServerSettingsRepository serverSettingsRepository,
                        TenantSettingsRepository tenantRepository,
                        CosmosDBRepository<UserContactInfo> cosmosUserRepository,
                        CosmosDBRepository<MeetingPlace> cosmosRepository
        )
        {
            _logger = logger;
            _config = config;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosUserRepository = cosmosUserRepository;
            _cosmosRepository = cosmosRepository;
            _tenantRepository = tenantRepository;
        }

        [FunctionName(nameof(GetMeetingPlaces))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetMeetingPlaces")] HttpRequest req)
        {
            try
            {
                CallingContext callingContext = await CallingContext.CreateCallingContext(req, _tenantRepository, _serverSettingsRepository, _cosmosUserRepository);
                callingContext.AssertConfirmedAccess();
                string tenant = callingContext.TenantSettings.TenantKey;
                if (String.IsNullOrWhiteSpace(tenant))
                {
                    tenant = null;
                }

                IEnumerable<MeetingPlace> meetingPlaces;
                if (null == tenant)
                {
                    meetingPlaces = await _cosmosRepository.GetItems(d => (d.Tenant ?? String.Empty) == String.Empty);
                }
                else
                {
                    meetingPlaces = await _cosmosRepository.GetItems(d => d.Tenant.Equals(tenant));
                }

                return new OkObjectResult(meetingPlaces);
            }
            catch (Exception ex)
            {
                return new BadRequestErrorMessageResult(ex.Message);
            }
        }
    }
}
