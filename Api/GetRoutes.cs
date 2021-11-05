using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
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
        private CosmosDBRepository<Comment> _cosmosCommentRepository;
        private CosmosDBRepository<UserContactInfo> _cosmosUserRepository;

        private TenantSettingsRepository _tenantRepository;
        private ServerSettingsRepository _serverSettingsRepository;
        private CallingContext _callingContext;
        public GetRoutes(ILogger<GetRoutes> logger,
                        IConfiguration config,
                        ServerSettingsRepository serverSettingsRepository,
                        TenantSettingsRepository tenantRepository,
                        CosmosDBRepository<Comment> cosmosCommentRepository,
                        CosmosDBRepository<UserContactInfo> cosmosUserRepository,
                        CosmosDBRepository<Route> cosmosRepository
        )
        {
            _logger = logger;
            _config = config;
            _serverSettingsRepository = serverSettingsRepository;
            _tenantRepository = tenantRepository;
            _cosmosUserRepository = cosmosUserRepository;
            _cosmosCommentRepository = cosmosCommentRepository;
            _cosmosRepository = cosmosRepository;
        }

        [FunctionName(nameof(GetRoutes))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "GetRoutes")] HttpRequest req)
        {
            try
            {
                _callingContext = await CallingContext.CreateCallingContext(req, _tenantRepository, _serverSettingsRepository, _cosmosUserRepository);

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                RouteFilter filter = JsonConvert.DeserializeObject<RouteFilter>(requestBody);
                if (filter.ForReview)
                {
                    _callingContext.AssertReviewerAuthorization();
                }

                IEnumerable<Route> routes = null;
                if (_callingContext.IsUserConfirmed || _callingContext.ValidKeyWordInHeader)
                {
                    // Get routes for review (if requested those) or only already reviewed or authored by calling user
                    if (filter.ForReview)
                    {
                        if (!filter.OnlyOwn)
                        { 
                            routes = await _cosmosRepository.GetItems(r => r.Tenant.CompareTo(_callingContext.TenantSettings.TrackKey) == 0 && !r.IsReviewed);
                        }
                        else
                        {
                            routes = await _cosmosRepository.GetItems(r => r.Tenant.CompareTo(_callingContext.TenantSettings.TrackKey) == 0 && !r.IsReviewed && _callingContext.User.ContactInfo.Id.CompareTo(r.AuthorId) == 0);
                        }
                    } 
                    else 
                    {
                        if (!_callingContext.IsUserReviewer)
                        {
                            if (!filter.OnlyOwn)
                            { 
                                routes = await _cosmosRepository.GetItems(r => r.Tenant.CompareTo(_callingContext.TenantSettings.TrackKey) == 0 && (r.IsReviewed || true));
                            }
                            else
                            {
                                routes = await _cosmosRepository.GetItems(r => r.Tenant.CompareTo(_callingContext.TenantSettings.TrackKey) == 0 && _callingContext.CheckAuthor(r));
                            }
                        }
                        else 
                        {
                            if (!filter.OnlyOwn)
                            { 
                                routes = await _cosmosRepository.GetItems(r => r.Tenant.CompareTo(_callingContext.TenantSettings.TrackKey) == 0);
                            }
                            else
                            {
                                routes = await _cosmosRepository.GetItems(r => r.Tenant.CompareTo(_callingContext.TenantSettings.TrackKey) == 0 && _callingContext.CheckAuthor(r));
                            }
                        }
                    }
                }
                else
                {
                    // Get only public and reviewed routes
                    routes = await _cosmosRepository.GetItems(r => r.Tenant.CompareTo(_callingContext.TenantSettings.TrackKey) == 0 && !r.IsNonPublic && r.IsReviewed);
                }

                List<ExtendedRoute> extendedRoutes = new List<ExtendedRoute>();
                string scopeToCompare = null;
                if (null != filter.Scope)
                {
                    scopeToCompare = filter.Scope.ToLowerInvariant();
;                }
                foreach (Route r in routes)
                {
                    // Check filter
                    Boolean checksPassed = true;
                    foreach(IList<RouteTag> routeTagList in filter.Tags)
                    {
                        if (routeTagList.Count == 0)
                        {
                            continue;
                        }
                        checksPassed = false;
                        foreach(RouteTag rt in routeTagList)
                        {
                            RouteTag foundRouteTag = r.RouteTags.FirstOrDefault(x => x.TagId.CompareTo(rt.TagId) == 0);
                            if (null != foundRouteTag)
                            {
                                checksPassed = true;
                                break;
                            }
                        }
                        if (!checksPassed)
                        {
                            break;
                        }
                    }
                    if (!checksPassed)
                    {
                        continue;
                    }
                    if (filter.OnlyForMembers && !r.IsNonPublic)
                    {
                        continue;
                    }
                    // Check if scoped route is requested
                    if (!String.IsNullOrEmpty(scopeToCompare)) 
                    {
                        if (String.IsNullOrEmpty(r.Scope) || r.Scope.ToLowerInvariant().CompareTo(scopeToCompare) != 0)
                        {
                            continue;
                        }
                    }
                    // Build ExtendedRoute
                    ExtendedRoute extendedRoute = new ExtendedRoute(r);
                    if (!String.IsNullOrEmpty(r.AuthorId))
                    { 
                        UserContactInfo author = await _cosmosUserRepository.GetItem(r.AuthorId);
                        extendedRoute.AuthorDisplayName = author?.UserNickName;
                        if (_callingContext.IsUserReviewer)
                        {
                            extendedRoute.Author = author;
                        }
                    }
                    if (r.IsReviewed && !String.IsNullOrEmpty(r.ReviewerId))
                    {
                        UserContactInfo reviewer = await _cosmosUserRepository.GetItem(r.ReviewerId);
                        extendedRoute.ReviewerDisplayName = reviewer?.UserNickName;
                        if (_callingContext.IsUserReviewer)
                        {
                            extendedRoute.Reviewer = reviewer;
                        }
                    }
                    // Get all comments
                    IEnumerable<Comment> comments = await _cosmosCommentRepository.GetItems(c => c.ReferenceId.CompareTo(extendedRoute.Core.Id) == 0);
                    extendedRoute.CommentsList = (await ExpandCommentList(comments)).OrderByDescending(c => c.Core.CommentDate);
                    ExtendedComment newestComment = extendedRoute.CommentsList.FirstOrDefault();
                    if (null != newestComment)
                    {
                        extendedRoute.LastUpdate = newestComment.Core.CommentDate;
                    }
                    extendedRoute.LastUpdate = r.Date;
                    extendedRoutes.Add(extendedRoute);
                }

                // return ordered descending by date and max Constants.MAX_ROUTES_COUNT items of the list.
                return new OkObjectResult(extendedRoutes.OrderByDescending(r => r.LastUpdate).Take(Constants.MAX_ROUTES_COUNT));
            }
            catch (Exception ex)
            {
                return new BadRequestErrorMessageResult(ex.Message);
            }
        }
        private async Task<IEnumerable<ExtendedComment>> ExpandCommentList(IEnumerable<Comment> commentList)
        {
            List<ExtendedComment> extendedCommentList = new List<ExtendedComment>();
            foreach (Comment comment in commentList)
            { 
                ExtendedComment extendedComment = new ExtendedComment(comment);
                UserContactInfo author = null;
                if (!String.IsNullOrEmpty(comment.AuthorId))
                {
                    author = await _cosmosUserRepository.GetItem(comment.AuthorId);
                    extendedComment.Core.AuthorDisplayName = author?.UserNickName;
                }
                if (_callingContext.IsUserReviewer)
                {
                    extendedComment.Author = author;
                }
                extendedCommentList.Add(extendedComment);
            }
            return extendedCommentList;
        }
    }
}
