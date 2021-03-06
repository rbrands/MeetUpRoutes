using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using BlazorApp.Shared;
using System.Collections.Generic;

namespace BlazorApp.Api
{
    public static class GetClientPrincipal
    {
        [FunctionName("GetUserDetails")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("GetClientPrincipal called");
            ClientPrincipal user = Utils.UserDetails.GetClientPrincipal(req);

            return new OkObjectResult(user);
        }
    }
}