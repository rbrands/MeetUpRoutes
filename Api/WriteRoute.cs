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
    public class WriteRoute
    {
        private readonly ILogger _logger;
        private CosmosDBRepository<Route> _cosmosRepository;
        private CosmosDBRepository<UserContactInfo> _cosmosUserRepository;
        private TenantSettingsRepository _tenantSettingsRepository;
        private ServerSettingsRepository _serverSettingsRepository;

        public WriteRoute(ILogger<WriteRoute> logger,
                         ServerSettingsRepository serverSettingsRepository,
                         TenantSettingsRepository tenantSettingsRepository,
                         CosmosDBRepository<UserContactInfo> cosmosUserRepository,
                         CosmosDBRepository<Route> cosmosRepository
                         )
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _tenantSettingsRepository = tenantSettingsRepository;
            _cosmosUserRepository = cosmosUserRepository;
            _cosmosRepository = cosmosRepository;
        }

        [FunctionName("WriteRoute")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "WriteRoute")] HttpRequest req
            )
        {
            try
            {
                CallingContext callingContext = await CallingContext.CreateCallingContext(req, _tenantSettingsRepository, _serverSettingsRepository, _cosmosUserRepository);
                _logger.LogInformation("WriteRoute for tenant {tenant} and user {user}", callingContext.TenantSettings.TenantKey, callingContext.User.ContactInfo.UserName);
                callingContext.AssertConfirmedAccess();

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                Route route = JsonConvert.DeserializeObject<Route>(requestBody);
                // Set tenant again to ensure that the data is written to the correct tenant!
                route.Tenant = callingContext.TenantSettings.TrackKey;
                // Set create date if it is a new entry
                if (String.IsNullOrEmpty(route.Id))
                {
                    route.Date = DateTime.UtcNow;
                    route.AuthorId = callingContext.User.ContactInfo.Id;
                    if (callingContext.IsUserAuthor)
                    {
                        route.IsReviewed = true;
                    }
                }
                else if (!String.IsNullOrEmpty(route.AuthorId) && route.AuthorId.CompareTo(callingContext.User.ContactInfo.Id) == 0)
                {
                    // Author is the same as the original author
                    route.Date = DateTime.UtcNow;
                    if (callingContext.IsUserAuthor && !callingContext.IsUserReviewer)
                    {
                        route.IsReviewed = true;
                    }
                }
                else
                {
                    // another one authored this version
                    callingContext.AssertReviewerAuthorization();
                    route.ReviewerId = callingContext.User.ContactInfo.Id;
                    route.ReviewDate = DateTime.UtcNow;
                    if (String.IsNullOrEmpty(route.AuthorId))
                    {
                        route.AuthorId = callingContext.User.ContactInfo.Id;
                    }
                }
                // Set scope if not already set
                if (String.IsNullOrEmpty(route.Scope))
                {
                    route.Scope = route.GetUrlFriendlyTitle();
                }
                Route updatedRoute = await _cosmosRepository.UpsertItem(route);

                return new OkObjectResult(updatedRoute);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WriteRoute failed.");
                return new BadRequestErrorMessageResult(ex.Message);
            }
        }
    }
}
