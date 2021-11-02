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
    public class DeleteComment
    {
        private readonly ILogger _logger;
        private CosmosDBRepository<Comment> _cosmosRepository;
        private CosmosDBRepository<UserContactInfo> _cosmosUserRepository;
        private TenantSettingsRepository _tenantSettingsRepository;
        private ServerSettingsRepository _serverSettingsRepository;

        public DeleteComment(ILogger<DeleteComment> logger,
                         ServerSettingsRepository serverSettingsRepository,
                         TenantSettingsRepository tenantSettingsRepository,
                         CosmosDBRepository<UserContactInfo> cosmosUserRepository,
                         CosmosDBRepository<Comment> cosmosRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _tenantSettingsRepository = tenantSettingsRepository;
            _cosmosUserRepository = cosmosUserRepository;
            _cosmosRepository = cosmosRepository;
        }

        [FunctionName("DeleteComment")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "DeleteComment")] HttpRequest req
            )
        {
            try
            {
                CallingContext callingContext = await CallingContext.CreateCallingContext(req, _tenantSettingsRepository, _serverSettingsRepository, _cosmosUserRepository);
                callingContext.AssertConfirmedAccess();

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                Comment comment = JsonConvert.DeserializeObject<Comment>(requestBody);
                if (String.IsNullOrEmpty(comment.Id))
                {
                    return new BadRequestErrorMessageResult("Die Id des Comment fehlt.");
                }
                if (String.IsNullOrEmpty(comment.AuthorId) || comment.AuthorId.CompareTo(callingContext.User.ContactInfo.Id) != 0)
                {
                    // another one authored this version
                    callingContext.AssertReviewerAuthorization();
                }
                await _cosmosRepository.DeleteItemAsync(comment.Id);

                return new OkResult();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new BadRequestErrorMessageResult(ex.Message);
            }
        }
    }
}
