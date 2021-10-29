using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using BlazorApp.Shared;
using System.Web.Http;
using System.Collections.Generic;
using BlazorApp.Api.Repositories;
using BlazorApp.Api.Utils;

namespace BlazorApp.Api
{
    public class GetRoutes
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private CosmosDBRepository<Route> _cosmosRepository;
        private CosmosDBRepository<UserContactInfo> _cosmosUserRepository;

        private TenantSettingsRepository _tenantRepository;
        private ServerSettingsRepository _serverSettingsRepository;
        public GetRoutes(ILogger<GetRoutes> logger,
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

        [FunctionName(nameof(GetRoutes))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "GetRoutes")] HttpRequest req)
        {
            try
            {
                CallingContext callingContext = await CallingContext.CreateCallingContext(req, _tenantRepository, _serverSettingsRepository, _cosmosUserRepository);

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                RouteFilter filter = JsonConvert.DeserializeObject<RouteFilter>(requestBody);
                if (filter.ForReview)
                {
                    callingContext.AssertReviewerAuthorization();
                }

                IEnumerable<Route> routes = null;
                if (callingContext.IsUserConfirmed)
                { 
                    // Get routes for review (if requested those) or only already reviewed or authored by calling user
                    if (filter.ForReview)
                    {
                        routes = await _cosmosRepository.GetItems(r => !r.IsReviewed);
                    } 
                    else 
                    {
                        routes = await _cosmosRepository.GetItems(r => (r.IsReviewed || callingContext.User.ContactInfo.Id.CompareTo(r.AuthorId) == 0));
                    }
                }
                else
                {
                    // Get only public and reviewed routes
                    routes = await _cosmosRepository.GetItems(r => !r.IsNonPublic && r.IsReviewed);
                }

                List<ExtendedRoute> extendedRoutes = new List<ExtendedRoute>();
                foreach (Route r in routes)
                {
                    ExtendedRoute extendedRoute = new ExtendedRoute(r);
                    if (!String.IsNullOrEmpty(r.AuthorId))
                    { 
                        UserContactInfo author = await _cosmosUserRepository.GetItem(r.AuthorId);
                        extendedRoute.AuthorDisplayName = author.UserNickName;
                        if (callingContext.IsUserReviewer)
                        {
                            extendedRoute.Author = author;
                        }
                    }
                    if (r.IsReviewed && !String.IsNullOrEmpty(r.ReviewerId))
                    {
                        UserContactInfo reviewer = await _cosmosUserRepository.GetItem(r.ReviewerId);
                        extendedRoute.ReviewerDisplayName = reviewer.UserNickName;
                        if (callingContext.IsUserReviewer)
                        {
                            extendedRoute.Reviewer = reviewer;
                        }
                    }
                    extendedRoute.LastUpdate = r.Date;
                    extendedRoutes.Add(extendedRoute);
                }
                // Sort descending on date
                extendedRoutes.Sort((x, y) => y.LastUpdate.CompareTo(x.LastUpdate));

                return new OkObjectResult(extendedRoutes);
            }
            catch (Exception ex)
            {
                return new BadRequestErrorMessageResult(ex.Message);
            }
        }
    }
}
