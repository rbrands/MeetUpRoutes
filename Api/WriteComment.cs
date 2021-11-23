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
    public class WriteComment
    {
        private readonly ILogger _logger;
        private CosmosDBRepository<Comment> _cosmosRepository;
        private CosmosDBRepository<UserContactInfo> _cosmosUserRepository;
        private TenantSettingsRepository _tenantSettingsRepository;
        private ServerSettingsRepository _serverSettingsRepository;

        public WriteComment(ILogger<WriteComment> logger,
                         ServerSettingsRepository serverSettingsRepository,
                         TenantSettingsRepository tenantSettingsRepository,
                         CosmosDBRepository<UserContactInfo> cosmosUserRepository,
                         CosmosDBRepository<Comment> cosmosRepository
                         )
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _tenantSettingsRepository = tenantSettingsRepository;
            _cosmosUserRepository = cosmosUserRepository;
            _cosmosRepository = cosmosRepository;
        }

        [FunctionName("WriteComment")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "WriteComment")] HttpRequest req
            )
        {
            try
            {
                CallingContext callingContext = await CallingContext.CreateCallingContext(req, _tenantSettingsRepository, _serverSettingsRepository, _cosmosUserRepository);
                callingContext.AssertConfirmedOrValidKeyWordAccess();

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                Comment comment = JsonConvert.DeserializeObject<Comment>(requestBody);
                // Set tenant again to ensure that the data is written to the correct tenant!
                comment.Tenant = callingContext.TenantSettings.TrackKey;
                // Set create date and author
                comment.CommentDate = DateTime.UtcNow;
                if (!String.IsNullOrEmpty(callingContext.User?.ContactInfo?.Id))
                { 
                    comment.AuthorId = callingContext.User?.ContactInfo?.Id;
                }
                Comment updatedComment = await _cosmosRepository.UpsertItem(comment);

                return new OkObjectResult(updatedComment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WriteComment failed.");
                return new BadRequestErrorMessageResult(ex.Message);
            }
        }
    }
}
