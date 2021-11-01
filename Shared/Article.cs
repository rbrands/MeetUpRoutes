using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Shared
{
    public class Article : CosmosDBEntity
    {
        [JsonProperty(PropertyName = "articleKey", NullValueHandling = NullValueHandling.Ignore)]
        public string ArticleKey { get; set; }
        [JsonProperty(PropertyName = "title", NullValueHandling = NullValueHandling.Ignore), MaxLength(120, ErrorMessage = "Titel zu lang.")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "articleText"), Display(Name = "Text"), MaxLength(5000, ErrorMessage = "Die Beschreibung ist zu lang.")]
        public string ArticleText { get; set; }
    }
}
