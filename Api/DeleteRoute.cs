using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
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
        private CosmosDBRepository<Comment> _cosmosCommentRepository;
        private TenantSettingsRepository _tenantSettingsRepository;
        private ServerSettingsRepository _serverSettingsRepository;

        public DeleteRoute(ILogger<DeleteRoute> logger,
                         ServerSettingsRepository serverSettingsRepository,
                         TenantSettingsRepository tenantSettingsRepository,
                         CosmosDBRepository<UserContactInfo> cosmosUserRepository,
                         CosmosDBRepository<Comment> cosmosCommentRepository,
                         CosmosDBRepository<Route> cosmosRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _tenantSettingsRepository = tenantSettingsRepository;
            _cosmosUserRepository = cosmosUserRepository;
            _cosmosCommentRepository = cosmosCommentRepository;
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
                if (String.IsNullOrEmpty(route.AuthorId) || route.AuthorId.CompareTo(callingContext.User.ContactInfo.Id) != 0)
                {
                    // another one authored this version
                    callingContext.AssertReviewerAuthorization();
                }
                await _cosmosRepository.DeleteItemAsync(route.Id);
                // Delete all comments
                IEnumerable<Comment> comments = await _cosmosCommentRepository.GetItems(c => c.ReferenceId.Equals(route.Id));
                foreach (Comment c in comments)
                {
                    await _cosmosCommentRepository.DeleteItemAsync(c.Id);
                }
                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteRoute failed.");
                return new BadRequestErrorMessageResult(ex.Message);
            }
        }
    }
}
