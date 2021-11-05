using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using BlazorApp.Shared;

namespace BlazorApp.Api.Utils
{
    public class ServerSettings : CosmosDBEntity
    {
        [JsonProperty(PropertyName = "userKeyword")]
        [Required(ErrorMessage = "Bitte ein Schlüsselwort für den Zugriff vergeben.")]
        public string UserKeyword { get; set; } = "Demo";
        [JsonProperty(PropertyName = "adminKeyword")]
        [Required(ErrorMessage = "Bitte ein Schlüsselwort für den Admin-Zugriff vergeben.")]
        public string AdminKeyword { get; set; } = "DemoAdmin";
        /// <summary>
        /// Checks if the given keyword matches the admin keyword
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public bool IsAdmin(string keyword)
        {
            return AdminKeyword.Equals(keyword);
        }
        /// <summary>
        /// Checks if the given keyword matches the user keyword or the admin keyword
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public bool IsUser(string keyword)
        {
            return this.UserKeyword.CompareTo(keyword) == 0 || this.AdminKeyword.CompareTo(keyword) == 0;
        }
    }
}
