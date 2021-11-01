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
    public class GetComment
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private CosmosDBRepository<Comment> _cosmosRepository;
        private CosmosDBRepository<UserContactInfo> _cosmosUserRepository;

        private TenantSettingsRepository _tenantRepository;
        private ServerSettingsRepository _serverSettingsRepository;
        public GetComment(ILogger<GetRoute> logger,
                        IConfiguration config,
                        ServerSettingsRepository serverSettingsRepository,
                        TenantSettingsRepository tenantRepository,
                         CosmosDBRepository<UserContactInfo> cosmosUserRepository,
                        CosmosDBRepository<Comment> cosmosRepository
        )
        {
            _logger = logger;
            _config = config;
            _serverSettingsRepository = serverSettingsRepository;
            _tenantRepository = tenantRepository;
            _cosmosUserRepository = cosmosUserRepository;
            _cosmosRepository = cosmosRepository;
        }

        [FunctionName(nameof(GetComment))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetComment/{id}")] HttpRequest req, string id)
        {
            try
            {
                CallingContext callingContext = await CallingContext.CreateCallingContext(req, _tenantRepository, _serverSettingsRepository, _cosmosUserRepository);

                if (String.IsNullOrEmpty(id))
                {
                    throw new Exception("Missing id for call GetComment()");
                }
                Comment comment = await _cosmosRepository.GetItem(id);
                if (null == comment)
                {
                    throw new Exception($"Comment with id {id} not found.");
                }
                ExtendedComment extendedComment = new ExtendedComment(comment);

                UserContactInfo author = await _cosmosUserRepository.GetItem(comment.AuthorId);
                extendedComment.AuthorDisplayName = author?.UserNickName;
                if (callingContext.IsUserReviewer)
                {
                    extendedComment.Author = author;
                }

                return new OkObjectResult(extendedComment);
            }
            catch (Exception ex)
            {
                return new BadRequestErrorMessageResult(ex.Message);
            }
        }
    }
}
