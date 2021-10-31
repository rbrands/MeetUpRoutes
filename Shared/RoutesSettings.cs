using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Shared
{
    public class RoutesSettings : CosmosDBEntity
    {
        [JsonProperty(PropertyName = "logoLink", NullValueHandling = NullValueHandling.Ignore)]
        public string LogoLink { get; set; }
    }
}
