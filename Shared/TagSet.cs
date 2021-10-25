using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Shared
{
    public class TagSet : CosmosDBEntity
    {
        [JsonProperty(PropertyName = "name"), Display(Name = "Name"), Required(ErrorMessage = "Bitte Namen angeben."), MaxLength(120, ErrorMessage = "Name zu lang.")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "isMandatory")]
        public Boolean IsMandatory { get; set; } = false;
        [JsonProperty(PropertyName = "isMutuallyExclusive")]
        public Boolean IsMutuallyExclusive { get; set; } = false;
        [JsonProperty(PropertyName = "hasRestrictedAccess")]
        public Boolean HasRestrictedAccess { get; set; } = false;
        [JsonProperty(PropertyName = "tags")]
        public IList<Tag> Tags { get; set; } = new List<Tag>();

        public string GetAllTagsAsString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Tag tag in Tags)
            {
                sb.Append(tag.Label + " ");
            }
            return sb.ToString();
        }
     }

    public class Tag
    {
        [JsonProperty(PropertyName = "tagId"), Display(Name = "Tag Id")]
        public int TagId { get; set; } = 0;
        [JsonProperty(PropertyName = "label"), Display(Name = "Tag Label"), Required(ErrorMessage = "Bitte eine Bezeichnung für das Label eingeben."), MaxLength(80, ErrorMessage = "Label zu lang.")]
        [RegularExpression("[a-zA-Z0-9-]*", ErrorMessage = "Bitte nur Buchstaben und Zahlen für das Label.")]
        public string Label { get; set; }
    }
}
