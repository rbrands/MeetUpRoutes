using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Shared
{
    /// <summary>
    /// Contact info for user to be used as author reference. IdentityProvider/UserId is used as unique key.
    /// </summary>
    public class UserContactInfo : CosmosDBEntity
    {
        /// <summary>
        /// UserKey is "IdendityProvider"-"UserId"
        /// </summary>
        public string UserKey { get; set; }
        [JsonProperty(PropertyName = "userName", NullValueHandling = NullValueHandling.Ignore), Display(Name = "Name", Prompt = "Name"), MaxLength(120, ErrorMessage = "Name zu lang."), Required(ErrorMessage = "Bitte Namen eingeben.")]
        public string UserName { get; set; }
        [JsonProperty(PropertyName = "userMail", NullValueHandling = NullValueHandling.Ignore), Display(Name = "E-Mail", Prompt = "Mail"), MaxLength(120, ErrorMessage = "Bitte eine gültige Mail-Adresse angeben."), Required(ErrorMessage = "Bitte Mail-Adresse eingeben.")]
        public string UserMail { get; set; }
    }
}
