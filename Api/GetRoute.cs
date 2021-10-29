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
    public class GetRoute
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private CosmosDBRepository<Route> _cosmosRepository;
        private CosmosDBRepository<UserContactInfo> _cosmosUserRepository;

        private TenantSettingsRepository _tenantRepository;
        private ServerSettingsRepository _serverSettingsRepository;
        public GetRoute(ILogger<GetRoute> logger,
                        IConfiguration config,
                        ServerSettingsRepository serverSettingsRepository,
                        TenantSettingsRepository tenantRepository,
                         CosmosDBRepository<UserContactInfo> cosmosUserRepository,
                        CosmosDBRepository<Route> cosmosRepository
        )
        {
            _logger = logger;
            _config = config;
            _serverSettingsRepository = serverSettingsRepository;
            _tenantRepository = tenantRepository;
            _cosmosUserRepository = cosmosUserRepository;
            _cosmosRepository = cosmosRepository;
        }

        [FunctionName(nameof(GetRoute))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetRoute/{id}")] HttpRequest req, string id)
        {
            try
            {
                CallingContext callingContext = await CallingContext.CreateCallingContext(req, _tenantRepository, _serverSettingsRepository, _cosmosUserRepository);
            
                if (String.IsNullOrEmpty(id))
                {
                    throw new Exception("Missing id for call GetRoute()");
                }
                Route route = await _cosmosRepository.GetItem(id);
                if (null == route)
                {
                    throw new Exception($"Route with id {id} not found.");
                }
                ExtendedRoute extendedRoute = new ExtendedRoute(route);
                
                // Check if user is allowed to get this route
                if (route.IsNonPublic)
                {
                    callingContext.AssertConfirmedAccess();
                }
                if (!route.IsReviewed && !callingContext.CheckAuthor(route))
                {
                    callingContext.AssertReviewerAuthorization();
                }
                UserContactInfo author = await _cosmosUserRepository.GetItem(route.AuthorId);
                extendedRoute.AuthorDisplayName = author.UserNickName;
                if (callingContext.IsUserReviewer)
                {
                    extendedRoute.Author = author;
                }
                if (route.IsReviewed && !String.IsNullOrEmpty(route.ReviewerId))
                {
                    UserContactInfo reviewer = await _cosmosUserRepository.GetItem(route.ReviewerId);
                    extendedRoute.ReviewerDisplayName = reviewer.UserNickName;
                    if (callingContext.IsUserReviewer)
                    {
                        extendedRoute.Reviewer = reviewer;
                    }
                }

                return new OkObjectResult(extendedRoute);
            }
            catch (Exception ex)
            {
                return new BadRequestErrorMessageResult(ex.Message);
            }
        }
    }
}
