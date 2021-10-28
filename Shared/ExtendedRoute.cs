using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Shared
{
    public class ExtendedRoute
    {
        public Route Core { get; set; }
        public UserContactInfo Author { get; set; }
        public string AuthorDisplayName { get; set; }
        public UserContactInfo Reviewer { get; set; }
        public string ReviewerDisplayName { get; set; }
        public DateTime LastUpdate {get;set;}

        public ExtendedRoute()
        {
            Core = new Route();
        }
        /// <summary>
        /// "Copy" constructor with instance of base class
        /// </summary>
        /// <param name="route"></param>
        public ExtendedRoute(Route route)
        {
            Core = route;
        }
    }
}
